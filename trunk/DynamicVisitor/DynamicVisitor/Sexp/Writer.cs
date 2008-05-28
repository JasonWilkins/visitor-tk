using System;
using System.Text;
using System.Collections.Generic;

using Util;

namespace Sexp {
    public static class GetTopLevelWriter {
        public static VectorWriter create(Writer writer)
        {
            Config config = new Config(writer);
            return new VectorWriter(config.file, config);
        }
    }

    public class Format {
        public int line_spacing = 2;
        public bool format_vector = false;
        public bool format_appl = true;
        public bool format_first = false;
        public bool format_data = false;
        public bool do_abbrev = true;
        public bool dot_cdr = false;
        public bool dot_nil = false;
    }

    class Config {
        public VectFormatter file;
        public VectFormatter top_vect;
        public ConsFormatter top_cons;
        public VectFormatter vect;
        public VectFormatter vect_cdr;
        public ConsFormatter vect_cons;
        public ConsFormatter vect_cons_cdr;
        public ConsFormatter first_cons;
        public ConsFormatter first_cons_cdr;
        public ConsFormatter cons;
        public ConsFormatter appl_cons_cdr;
        public ConsFormatter data_cons_cdr;
        public AtomFormatter atom;
        public AtomFormatter atom_cdr;

        public Config(Writer writer)
            : this(writer, new Format())
        { }

        public Config(Writer writer, Format fmt)
        {
            {
                file = new VectFormatter(writer);
                file.seperator      = fmt.line_spacing > 0 ? "" : " ";
                file.seperator_spacing  = fmt.line_spacing;
                file.footer_spacing = 1;
            }

            top_vect = new_vect(writer, fmt.format_vector, "#(");
            vect     = new_vect(writer, fmt.format_vector, "#(");
            vect_cdr = new_vect(writer, fmt.format_vector, fmt.format_vector ? ". #(" : " . #(");

            cons       = new_cons(writer, fmt.do_abbrev, true);
            top_cons   = new_cons(writer, fmt.do_abbrev, true);
            vect_cons  = new_cons(writer, fmt.do_abbrev, fmt.format_vector);
            first_cons = new_cons(writer, fmt.do_abbrev, fmt.format_first);

            vect_cons_cdr  = new_cons_cdr(writer, fmt.format_vector, fmt.dot_cdr, fmt.dot_nil);
            first_cons_cdr = new_cons_cdr(writer, fmt.format_first, fmt.dot_cdr, fmt.dot_nil);
            appl_cons_cdr  = new_cons_cdr(writer, fmt.format_appl, fmt.dot_cdr, fmt.dot_nil);
            data_cons_cdr  = new_cons_cdr(writer, fmt.format_data, fmt.dot_cdr, fmt.dot_nil);

            atom = new AtomFormatter(writer);

            {
                atom_cdr = new AtomFormatter(writer);
                atom_cdr.header = " . ";
            }
        }

        VectFormatter new_vect(Writer writer, bool format, string head)
        {
            VectFormatter f = new VectFormatter(writer);

            f.do_indent         = format;
            f.seperator_spacing = format ? 1 : 0;
            f.seperator         = format ? "" : " ";
            f.header            = head;
            f.footer            = ")";

            return f;
        }

        ConsFormatter new_cons(Writer writer, bool abbreviate, bool indent)
        {
            ConsFormatter f = new ConsFormatter(writer);

            f.do_abbrev = abbreviate;
            f.do_indent = indent;
            f.header    = "(";
            f.footer    = ")";

            return f;
        }

        ConsFormatter new_cons_cdr(Writer writer, bool format, bool dot_cdr, bool dot_nil)
        {
            ConsFormatter f = new ConsFormatter(writer);

            f.header_spacing = format ? 1 : 0;

            string delimiter = format ? "" : " ";

            if (dot_cdr) {
                f.header = delimiter + ". (";
                f.footer = ")";
            } else {
                f.header = delimiter;
            }

            if (dot_nil) {
                f.nil_cdr = " . ()";
            }

            return f;
        }
    }

    abstract class Formatter {
        public bool do_abbrev = true;
        public bool do_indent = false;
        public string header = "";
        public int header_spacing = 0;
        public string seperator = " ";
        public int seperator_spacing = 0;
        public string footer = "";
        public int footer_spacing = 0;
        public string nil = "()";
        public string nil_cdr = "";

        Writer m_writer;

        public Formatter(Writer writer)
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
            if (!is_abbrev_body) {
                append_newlines(header_spacing);
                m_writer.Append(header);
            }

            if (do_indent) m_writer.Indent();

            return this;
        }

        public Formatter AppendSeperator(ref bool is_first)
        {
            if (is_first) {
                is_first = false;
            } else {
                m_writer.Append(seperator);
                append_newlines(seperator_spacing);
            }

            return this;
        }

        public Formatter AppendFooter(bool is_abbrev_body)
        {
            if (do_indent) m_writer.Unindent();

            if (!is_abbrev_body) {
                m_writer.Append(footer);
                append_newlines(footer_spacing);
            }

            return this;
        }

        public Formatter AppendAtom(object o, bool is_abbrev_body)
        {
            AppendHeader(is_abbrev_body);
            m_writer.Append(Literal.format(o));
            return this;
        }

        public Formatter AppendAbbrev(AtomCtor atom, bool is_abbrev_body, out bool is_abbrev)
        {
            if (do_abbrev) {
                string abbrev = get_abbreviation(atom);

                if (abbrev != null) {
                    m_writer.Append(abbrev);
                    is_abbrev = true;
                    return this;
                }
            }

            is_abbrev = false;

            return AppendAtom(atom.value, is_abbrev_body);
        }

        void append_newlines(int c)
        {
            for (int i = 0; i < c; i++) {
                m_writer.End();
            }

            if (c > 0) m_writer.Begin();
        }

        string get_abbreviation(AtomCtor atom)
        {
            if (atom.value is Symbol) {
                string name = ((Symbol)atom.value).name;

                if (name == "quote") {
                    return "'";
                } else if (name == "quasiquotation") {
                    return "`";
                } else if (name == "unquote") {
                    return ",";
                } else if (name == "unquote-splicing") {
                    return ",@";
                }
            }

            return null;
        }
    }

    class VectFormatter : Formatter {
        public VectFormatter(Writer writer)
            : base(writer)
        { }
    }

    class ConsFormatter : Formatter {
        public ConsFormatter(Writer writer)
            : base(writer)
        { }
    }

    class AtomFormatter : Formatter {
        public AtomFormatter(Writer writer)
            : base(writer)
        { }
    }

    public class VectorWriter : VectorVisitor {
        Formatter m_formatter;
        Config m_config;

        bool is_first = true;
        bool m_is_abbrev_body;

        internal VectorWriter(Formatter formatter, Config config)
            : this(formatter, config, false)
        { }

        internal VectorWriter(Formatter formatter, Config config, bool is_abbrev_body)
        {
            m_is_abbrev_body = is_abbrev_body;
            m_formatter      = formatter;
            m_config         = config;
        }

        public override void visit()
        {
            m_formatter.AppendHeader(m_is_abbrev_body);
        }

        public override void visitEnd()
        {
            m_formatter.AppendFooter(m_is_abbrev_body);
        }

        public override AtomVisitor visitItem_Atom()
        {
            m_formatter.AppendSeperator(ref is_first);
            return new AtomWriter(m_config.atom);
        }

        public override ConsVisitor visitItem_Cons()
        {
            m_formatter.AppendSeperator(ref is_first);

            if (m_config.file == m_formatter) {
                return new ConsWriter(m_config.top_cons, m_config);
            } else {
                return new ConsWriter(m_config.vect_cons, m_config);
            }
        }

        public override VectorVisitor visitItem_Vector()
        {
            m_formatter.AppendSeperator(ref is_first);

            if (m_config.file == m_formatter) {
                return new VectorWriter(m_config.top_vect, m_config);
            } else {
                return new VectorWriter(m_config.vect, m_config);
            }
        }

        public override void visitItem()
        {
            m_formatter.AppendSeperator(ref is_first).AppendNil();
        }
    }

    public class ConsWriter : ConsVisitor {
        ConsFormatter m_formatter;
        Config m_config;

        AtomCtor m_atom;
        bool m_is_abbrev;
        bool m_is_abbrev_body;

        internal ConsWriter(ConsFormatter formatter, Config config)
            : this(formatter, config, false)
        { }

        internal ConsWriter(ConsFormatter formatter, Config config, bool is_abbrev_body)
        {
            m_formatter = formatter;
            m_config = config;
            m_is_abbrev_body = is_abbrev_body;
        }

        public override void visitEnd()
        {
            if (!m_is_abbrev) m_formatter.AppendFooter(m_is_abbrev_body);
        }

        public override AtomVisitor visit_Atom_car()
        {
            m_atom = new AtomCtor();
            return new AtomBuilder(m_atom);
        }

        public override ConsVisitor visit_Cons_car()
        {
            m_formatter.AppendHeader(m_is_abbrev_body);

            if (m_config.cons == m_formatter) {
                return new ConsWriter(m_config.first_cons, m_config);
            } else if (m_config.appl_cons_cdr == m_formatter || m_config.data_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.cons, m_config);
            } else if (m_config.vect_cons == m_formatter || m_config.vect_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.vect_cons, m_config);
            } else if (m_config.first_cons == m_formatter || m_config.first_cons_cdr == m_formatter || m_config.top_cons == m_formatter) {
                return new ConsWriter(m_config.first_cons, m_config);
            } else {
                throw new Exception();
            }
        }

        public override VectorVisitor visit_Vector_car()
        {
            m_formatter.AppendHeader(m_is_abbrev_body);
            return new VectorWriter(m_config.vect, m_config);
        }

        public override void visit_car()
        {
            m_formatter.AppendHeader(m_is_abbrev_body).AppendNil();
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            if (m_atom != null) m_formatter.AppendAtom(m_atom.value, m_is_abbrev_body);

            return new AtomWriter(m_config.atom_cdr);
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            if (m_atom != null) m_formatter.AppendAbbrev(m_atom, m_is_abbrev_body, out m_is_abbrev);

            if (m_config.cons == m_formatter || m_config.top_cons == m_formatter) {
                if (m_atom == null || m_atom.value is Symbol) {
                    return new ConsWriter(m_config.appl_cons_cdr, m_config, m_is_abbrev);
                } else {
                    return new ConsWriter(m_config.data_cons_cdr, m_config, m_is_abbrev);
                }
            } else if (m_config.appl_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.appl_cons_cdr, m_config, m_is_abbrev);
            } else if (m_config.data_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.data_cons_cdr, m_config, m_is_abbrev);
            } else if (m_config.first_cons == m_formatter || m_config.first_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.first_cons_cdr, m_config, m_is_abbrev);
            } else if (m_config.vect_cons == m_formatter || m_config.vect_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.vect_cons_cdr, m_config, m_is_abbrev);
            } else {
                throw new Exception();
            }
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            if (m_atom != null) m_formatter.AppendAtom(m_atom.value, m_is_abbrev_body);

            return new VectorWriter(m_config.vect_cdr, m_config);
        }

        public override void visit_cdr()
        {
            if (m_atom != null) m_formatter.AppendAtom(m_atom.value, m_is_abbrev_body);

            m_formatter.AppendNilCdr(m_is_abbrev_body);
        }
    }

    public class AtomWriter : AtomVisitor {
        Formatter m_formatter;

        internal AtomWriter(Formatter formatter)
        {
            m_formatter = formatter;
        }

        public override void visit(object o)
        {
            m_formatter.AppendAtom(o, false).AppendFooter(false);
        }
    }
}
