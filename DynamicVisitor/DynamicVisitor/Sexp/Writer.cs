using System;

namespace Sexp {
    public class VectorWriter : VectVisitor {
        readonly Formatter m_formatter;
        readonly Config m_config;

        bool is_first = true;
        readonly bool m_is_abbrev_body;

        internal VectorWriter(Formatter formatter, Config config)
            : this(formatter, config, false) { }

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

        public override VectVisitor visitItem_Vect()
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
        readonly ConsFormatter m_formatter;
        readonly Config m_config;

        AtomBox m_atom;
        bool m_is_abbrev;
        readonly bool m_is_abbrev_body;

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
            m_atom = new AtomBox();
            return new AtomBuilder(m_atom);
        }

        public override ConsVisitor visit_Cons_car()
        {
            m_formatter.AppendHeader(m_is_abbrev_body);

            if (m_config.cons == m_formatter) {
                return new ConsWriter(m_config.head_cons, m_config);
            } else if (m_config.appl_cons_cdr == m_formatter || m_config.data_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.cons, m_config);
            } else if (m_config.vect_cons == m_formatter || m_config.vect_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.vect_cons, m_config);
            } else if (m_config.head_cons == m_formatter || m_config.head_cons_cdr == m_formatter || m_config.top_cons == m_formatter) {
                return new ConsWriter(m_config.head_cons, m_config);
            } else {
                throw new Exception();
            }
        }

        public override VectVisitor visit_Vect_car()
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
            if (m_atom != null) m_formatter.AppendAbbrev(m_atom.value, m_is_abbrev_body, out m_is_abbrev);

            if (m_config.cons == m_formatter || m_config.top_cons == m_formatter) {
                if (m_atom != null) {
                    Config config;
                    ConsFormatter formatter;

                    if (m_config.GetNewConfig(m_atom.value, out config, out formatter)) {
                        return new ConsWriter(formatter, config, m_is_abbrev);
                    }
                }

                if (null == m_atom || m_atom.value is Symbol) {
                    return new ConsWriter(m_config.appl_cons_cdr, m_config, m_is_abbrev);
                } else {
                    return new ConsWriter(m_config.data_cons_cdr, m_config, m_is_abbrev);
                }
            } else if (m_config.appl_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.appl_cons_cdr, m_config, m_is_abbrev);
            } else if (m_config.data_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.data_cons_cdr, m_config, m_is_abbrev);
            } else if (m_config.head_cons == m_formatter || m_config.head_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.head_cons_cdr, m_config, m_is_abbrev);
            } else if (m_config.vect_cons == m_formatter || m_config.vect_cons_cdr == m_formatter) {
                return new ConsWriter(m_config.vect_cons_cdr, m_config, m_is_abbrev);
            } else {
                throw new Exception();
            }
        }

        public override VectVisitor visit_Vect_cdr()
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
        readonly Formatter m_formatter;

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
