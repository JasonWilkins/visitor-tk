using Util;
using Symbols;

namespace Sexp {
    public class Format {
        public int line_spacing = 2;
        public bool format_vect = false;
        public bool format_appl = true;
        public bool format_head = false;
        public bool format_data = false;
        public bool do_abbrev = true;
        public bool dot_cdr = false;
        public bool dot_nil = false;
        public bool do_debug = false;
    }

    public delegate bool GetConfig(object atom, out Config config, out ConsFormatter formatter);

    public class Config {
        internal readonly VectFormatter file;
        internal readonly VectFormatter top_vect;
        internal readonly ConsFormatter top_cons;
        internal readonly VectFormatter vect;
        internal readonly VectFormatter vect_cdr;
        internal readonly ConsFormatter vect_cons;
        internal readonly ConsFormatter vect_cons_cdr;
        internal readonly ConsFormatter head_cons;
        internal readonly ConsFormatter head_cons_cdr;
        internal readonly ConsFormatter cons;
        internal readonly ConsFormatter appl_cons_cdr;
        internal readonly ConsFormatter data_cons_cdr;
        internal readonly AtomFormatter atom;
        internal readonly AtomFormatter atom_cdr;

        readonly GetConfig m_get_new_config;

        readonly Format m_fmt;

        public Config(Writer writer)
            : this(writer, new Format())
        { }

        public Config(Writer writer, Format fmt)
            : this(writer, fmt, no_new_config)
        { }

        public Config(Writer writer, Format fmt, GetConfig get_new_config)
        {
            m_fmt = fmt;

            m_get_new_config = get_new_config ?? no_new_config;

            file = new VectFormatter(writer);
            file.seperator          = fmt.line_spacing > 0 ? "" : " ";
            file.seperator_spacing  = fmt.line_spacing;
            file.footer_spacing     = 1;

            top_vect = new_vect(writer, fmt.format_vect, "#(", "TV");
            vect     = new_vect(writer, fmt.format_vect, "#(", "V");
            vect_cdr = new_vect(writer, fmt.format_vect, " . #(", "-V");

            cons      = new_cons(writer, fmt.do_abbrev, true, "C");
            top_cons  = new_cons(writer, fmt.do_abbrev, true, "TC");
            vect_cons = new_cons(writer, fmt.do_abbrev, fmt.format_vect, "VC");
            head_cons = new_cons(writer, fmt.do_abbrev, fmt.format_head, "HC");

            vect_cons_cdr = new_cons_cdr(writer, fmt.format_vect, fmt.dot_cdr, fmt.dot_nil, "-VC");
            head_cons_cdr = new_cons_cdr(writer, fmt.format_head, fmt.dot_cdr, fmt.dot_nil, "-HC");
            appl_cons_cdr = new_cons_cdr(writer, fmt.format_appl, fmt.dot_cdr, fmt.dot_nil, "-AC");
            data_cons_cdr = new_cons_cdr(writer, fmt.format_data, fmt.dot_cdr, fmt.dot_nil, "-DC");

            atom = new AtomFormatter(writer);

            atom_cdr = new AtomFormatter(writer);
            atom_cdr.header = " . ";
        }

        string debug_str(string db)
        {
            return m_fmt.do_debug ? "#|" + db + "|#" : "";
        }

        VectFormatter new_vect(Writer writer, bool format, string head, string db)
        {
            VectFormatter f = new VectFormatter(writer);

            f.do_indent         = format;
            f.seperator_spacing = format ? 1 : 0;
            f.seperator         = format ? "" : " ";
            f.header            = debug_str(db) + head;
            f.footer            = ")";

            return f;
        }

        ConsFormatter new_cons(Writer writer, bool do_abbrev, bool do_indent, string db)
        {
            ConsFormatter f = new ConsFormatter(writer);

            f.do_abbrev = do_abbrev;
            f.do_indent = do_indent;
            f.header    = debug_str(db) + "(";
            f.footer    = ")";

            return f;
        }

        ConsFormatter new_cons_cdr(Writer writer, bool format, bool dot_cdr, bool dot_nil, string db)
        {
            ConsFormatter f = new ConsFormatter(writer);

            f.header_spacing = format ? 1 : 0;

            string delimiter = format ? "" : " ";

            if (dot_cdr) {
                f.header = debug_str(db) + delimiter + ". (";
                f.footer = ")";
            } else {
                f.header = debug_str(db) + delimiter;
            }

            if (dot_nil) {
                f.nil_cdr = debug_str(db) + " . ()";
            }

            return f;
        }

        static bool no_new_config(object atom, out Config config, out ConsFormatter formatter)
        {
            config = null;
            formatter = null;
            return false;
        }

        public bool GetNewConfig(object atom, out Config config, out ConsFormatter formatter)
        {
            return m_get_new_config(atom, out config, out formatter);
        }
    }

    public abstract class Formatter {
        internal bool do_abbrev = false;
        internal bool do_indent = false;
        internal string header = "";
        internal int header_spacing = 0;
        internal string seperator = " ";
        internal int seperator_spacing = 0;
        internal string footer = "";
        internal int footer_spacing = 0;
        internal string nil = "()";
        internal string nil_cdr = "";

        readonly Writer m_writer;

        protected Formatter(Writer writer)
        {
            m_writer = writer;
        }

        public Formatter AppendNil()
        {
            m_writer.Append(nil);
            return this;
        }

        public Formatter AppendNilCdr(bool is_abbrev_body)
        {
            if (!is_abbrev_body) m_writer.Append(nil_cdr);
            return this;
        }

        public Formatter AppendHeader(bool is_abbrev_body)
        {
            if (!is_abbrev_body) m_writer.BeginLine(header_spacing).Append(header);

            if (do_indent) m_writer.Indent();

            return this;
        }

        public Formatter AppendSeperator(ref bool is_first)
        {
            if (is_first) {
                is_first = false;
            } else {
                m_writer.Append(seperator).BeginLine(seperator_spacing);
            }

            return this;
        }

        public Formatter AppendFooter(bool is_abbrev_body)
        {
            if (do_indent) m_writer.Unindent();

            if (!is_abbrev_body) m_writer.Append(footer).BeginLine(footer_spacing);

            return this;
        }

        public Formatter AppendAtom(object o, bool is_abbrev_body)
        {
            AppendHeader(is_abbrev_body);
            m_writer.Append(Literal.format(o));
            return this;
        }

        public Formatter AppendAbbrev(object o, bool is_abbrev_body, out bool is_abbrev)
        {
            is_abbrev = do_abbrev && o is Symbol && write_abbrev((Symbol)o);

            if (!is_abbrev) AppendAtom(o, is_abbrev_body);

            return this;
        }

        bool write_abbrev(Symbol sym)
        {
            string abbrev = Abbrev.get_abbrev(sym);

            if (abbrev != null) {
                m_writer.Append(abbrev);
                return true;
            } else {
                return false;
            }
        }
    }

    public class VectFormatter : Formatter { internal VectFormatter(Writer writer) : base(writer) { } }
    public class ConsFormatter : Formatter { internal ConsFormatter(Writer writer) : base(writer) { } }
    public class AtomFormatter : Formatter { internal AtomFormatter(Writer writer) : base(writer) { } }
}
