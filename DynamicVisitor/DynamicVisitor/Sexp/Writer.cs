using System;
using System.Text;
using System.Collections.Generic;

using Util;

namespace Sexp {
    public static class GetTopLevelWriter {
        public static VectorWriter create(Writer writer)
        {
            return new VectorWriter(MakeConfig.make_basic_config(writer).top, State.TOP);
        }
    }

    public class SexpWriterConfig {
        public SexpWriter top;
        public SexpWriter top_vect;
        public SexpWriter top_cons;
        public SexpWriter vect;
        public SexpWriter vect_cdr;
        public SexpWriter vect_cons;
        public SexpWriter vect_cons_cdr;
        public SexpWriter first_cons;
        public SexpWriter first_cons_cdr;
        public SexpWriter cons;
        public SexpWriter appl_cons_cdr;
        public SexpWriter data_cons_cdr;
        public SexpWriter atom;
        public SexpWriter atom_cdr;
    }

    public static class MakeConfig {
        public static SexpWriterConfig make_basic_config(Writer writer)
        {
            SexpWriterConfig rv = new SexpWriterConfig();

            rv.top = new SexpWriter(writer, rv);
            rv.top.sep = "";
            rv.top.sepline = 2;
            rv.top.footline = 1;

            rv.top_vect = new SexpWriter(writer, rv);
            rv.top_vect.head = "#(";
            rv.top_vect.foot = ")";

            rv.top_cons = new SexpWriter(writer, rv);
            rv.top_cons.indent = true;
            rv.top_cons.head = "(";
            rv.top_cons.foot = ")";

            rv.vect = new SexpWriter(writer, rv);
            rv.vect.head = "#(";
            rv.vect.foot = ")";

            rv.vect_cdr = new SexpWriter(writer, rv);
            rv.vect_cdr.head = " . #(";
            rv.vect_cdr.foot = ")";

            rv.vect_cons = new SexpWriter(writer, rv);
            rv.vect_cons.head = "(";
            rv.vect_cons.foot = ")";

            rv.vect_cons_cdr = new SexpWriter(writer, rv);
            rv.vect_cons_cdr.head = " ";

            rv.first_cons = new SexpWriter(writer, rv);
            rv.first_cons.head = "(";
            rv.first_cons.foot = ")";

            rv.cons = new SexpWriter(writer, rv);
            rv.cons.indent = true;
            rv.cons.head = "(";
            rv.cons.foot = ")";

            rv.first_cons_cdr = new SexpWriter(writer, rv);
            rv.first_cons_cdr.head = " ";

            rv.appl_cons_cdr = new SexpWriter(writer, rv);
            rv.appl_cons_cdr.head = "";
            rv.appl_cons_cdr.headline = 1;

            rv.data_cons_cdr = new SexpWriter(writer, rv);
            rv.data_cons_cdr.head = " ";

            rv.atom = new SexpWriter(writer, rv);

            rv.atom_cdr = new SexpWriter(writer, rv);
            rv.atom_cdr.head = " . ";

            return rv;
        }
    }

    public enum State {
        TOP,
        TOP_VECT,
        TOP_CONS,
        VECT,
        VECT_CDR,
        VECT_CONS,
        VECT_CONS_CDR,
        FIRST_CONS,
        FIRST_CONS_CDR,
        CONS,
        APPL_CONS_CDR,
        DATA_CONS_CDR,
        ATOM,
        ATOM_CDR,
    }

    public class SexpWriter {
        public bool abbreviate = true;
        public bool indent = false;
        public string head = "";
        public int headline = 0;
        public string sep = " ";
        public int sepline = 0;
        public string foot = "";
        public int footline = 0;

        Writer m_writer;

        SexpWriterConfig m_cfg;

        public SexpWriter(Writer writer, SexpWriterConfig cfg)
        {
            m_writer = writer;
            m_cfg = cfg;
        }

        public VectorWriter nextItemVectorWriter(State s)
        {
            if (State.TOP == s) {
                return new VectorWriter(m_cfg.top_vect, State.TOP_VECT);
            } else {
                return new VectorWriter(m_cfg.vect, State.VECT);
            }
        }

        public ConsWriter nextItemConsWriter(State s)
        {
            if (State.TOP == s) {
                return new ConsWriter(m_cfg.top_cons, State.TOP_CONS);
            } else {
                return new ConsWriter(m_cfg.vect_cons, State.VECT_CONS);
            }
        }

        public AtomWriter nextItemAtomWriter(State s)
        {
            return new AtomWriter(m_cfg.atom);
        }

        public VectorWriter nextCarVectorWriter(State s)
        {
            return new VectorWriter(m_cfg.vect, State.VECT);
        }

        public ConsWriter nextCarConsWriter(State s)
        {
            if (State.CONS == s) {
                return new ConsWriter(m_cfg.cons, State.CONS);
            } else if (State.APPL_CONS_CDR == s) {
                return new ConsWriter(m_cfg.cons, State.CONS);
            } else if (State.DATA_CONS_CDR == s) {
                return new ConsWriter(m_cfg.cons, State.CONS);
            } else if (State.VECT_CONS == s) {
                return new ConsWriter(m_cfg.vect_cons, State.VECT_CONS);
            } else if (State.VECT_CONS_CDR == s) {
                return new ConsWriter(m_cfg.vect_cons, State.VECT_CONS);
            } else if (State.FIRST_CONS == s) {
                return new ConsWriter(m_cfg.first_cons, State.FIRST_CONS);
            } else if (State.FIRST_CONS_CDR == s) {
                return new ConsWriter(m_cfg.first_cons, State.FIRST_CONS);
            } else if (State.TOP_CONS == s) {
                return new ConsWriter(m_cfg.first_cons, State.FIRST_CONS);
            } else {
                throw new Exception();
            }
        }

        public AtomVisitor nextCarAtomWriter(State s, out AtomCtor atom)
        {
            atom = new AtomCtor();
            return new AtomBuilder(atom);
        }

        public VectorWriter nextCdrVectorWriter(State s)
        {
            return new VectorWriter(m_cfg.vect_cdr, State.VECT_CDR);
        }

        public ConsWriter nextCdrConsWriter(bool is_quote, State s, AtomCtor atom)
        {
            if (State.CONS == s) {
                if (atom != null && atom.value is Symbol) {
                    return new ConsWriter(m_cfg.appl_cons_cdr, State.APPL_CONS_CDR, is_quote);
                } else {
                    return new ConsWriter(m_cfg.data_cons_cdr, State.DATA_CONS_CDR, is_quote);
                }
            } else if (State.FIRST_CONS_CDR == s) {
                return new ConsWriter(m_cfg.first_cons_cdr, State.FIRST_CONS_CDR, is_quote);
            } else if (State.APPL_CONS_CDR == s) {
                return new ConsWriter(m_cfg.appl_cons_cdr, State.APPL_CONS_CDR, is_quote);
            } else if (State.DATA_CONS_CDR == s) {
                return new ConsWriter(m_cfg.data_cons_cdr, State.DATA_CONS_CDR, is_quote);
            } else if (State.VECT_CONS == s) {
                return new ConsWriter(m_cfg.vect_cons_cdr, State.VECT_CONS_CDR, is_quote);
            } else if (State.VECT_CONS_CDR == s) {
                return new ConsWriter(m_cfg.vect_cons_cdr, State.VECT_CONS_CDR, is_quote);
            } else if (State.FIRST_CONS == s) {
                return new ConsWriter(m_cfg.first_cons_cdr, State.FIRST_CONS_CDR, is_quote);
            } else if (State.TOP_CONS == s) {
                if (atom != null && atom.value is Symbol) {
                    return new ConsWriter(m_cfg.appl_cons_cdr, State.APPL_CONS_CDR, is_quote);
                } else {
                    return new ConsWriter(m_cfg.data_cons_cdr, State.DATA_CONS_CDR, is_quote);
                }
            } else {
                throw new Exception();
            }
        }

        public AtomWriter nextCdrAtomWriter(State s)
        {
            return new AtomWriter(m_cfg.atom_cdr);
        }

        public SexpWriter Append(string s)
        {
            m_writer.Append(s);
            return this;
        }

        void lines(int c)
        {
            for (int i = 0; i < c; i++) {
                m_writer.End();
            }

            if (c > 0) m_writer.Begin();
        }

        public SexpWriter Head(bool abbreviated)
        {
            m_writer.Append(abbreviated ? head.Trim() : head);

            if (!abbreviated) lines(headline);

            if (indent) m_writer.Indent();

            return this;
        }

        public SexpWriter Delimiter(ref bool is_first)
        {
            if (is_first) {
                is_first = false;
            } else {
                m_writer.Append(sep);
                lines(sepline);
            }

            return this;
        }

        public SexpWriter Foot()
        {
            if (indent) m_writer.Unindent();

            m_writer.Append(foot);
            lines(footline);

            return this;
        }

        //public SexpWriter Car(AtomCtor atom)
        //{
        //    return Car(atom, false);
        //}

        public SexpWriter Car(AtomCtor atom, bool abbreviated)
        {
            if (atom != null) {
                return Head(abbreviated).Append(Literal.literal(atom.value));
            }

            return this;
        }

        public SexpWriter Abbrev(AtomCtor atom, bool abbreviated, out bool is_quote)
        {
            if (abbreviate) {
                if (atom != null && atom.value is Symbol) {
                    if (((Symbol)atom.value).name == "quote") {
                        m_writer.Append("'");
                        is_quote = true;
                        return this;
                    } else if (((Symbol)atom.value).name == "quasiquotation") {
                        m_writer.Append("`");
                        is_quote = true;
                        return this;
                    } else if (((Symbol)atom.value).name == "unquote") {
                        m_writer.Append(",");
                        is_quote = true;
                        return this;
                    } else if (((Symbol)atom.value).name == "unquote-splicing") {
                        m_writer.Append(",@");
                        is_quote = true;
                        return this;
                    }
                }
            }

            is_quote = false;
            return Car(atom, abbreviated);
        }
    }

    public class VectorWriter : VectorVisitor {
        SexpWriter m_writer;
        State m_state;

        bool is_first = true;
        bool m_abbreviated;

        public VectorWriter(SexpWriter writer, State s)
            : this(writer, s, false)
        { }

        public VectorWriter(SexpWriter writer, State s, bool abbreviated)
        {
            m_state = s;
            m_writer = writer;
            m_abbreviated = abbreviated;
        }

        public override void visit()
        {
            m_writer.Head(m_abbreviated);
        }

        public override void visitEnd()
        {
            m_writer.Foot();
        }

        public override AtomVisitor visitItem_Atom()
        {
            m_writer.Delimiter(ref is_first);
            return m_writer.nextItemAtomWriter(m_state);
        }

        public override ConsVisitor visitItem_Cons()
        {
            m_writer.Delimiter(ref is_first);
            return m_writer.nextItemConsWriter(m_state);
        }

        public override VectorVisitor visitItem_Vector()
        {
            m_writer.Delimiter(ref is_first);
            return m_writer.nextItemVectorWriter(m_state);
        }

        public override void visitItem()
        {
            m_writer.Delimiter(ref is_first).Append("()");
        }
    }

    public class ConsWriter : ConsVisitor {
        SexpWriter m_writer;
        State m_state;

        bool m_is_quote = false;
        AtomCtor m_atom = null;
        bool m_abbreviated = false;

        public ConsWriter(SexpWriter writer, State s)
            : this(writer, s, false)
        { }

        public ConsWriter(SexpWriter writer, State s, bool abbreviated)
        {
            m_state = s;
            m_writer = writer;
            m_abbreviated = abbreviated;
        }

        public override void visitEnd()
        {
            if (!m_is_quote) m_writer.Foot();
        }

        public override AtomVisitor visit_Atom_car()
        {
            return m_writer.nextCarAtomWriter(m_state, out m_atom);
        }

        public override ConsVisitor visit_Cons_car()
        {
            m_writer.Head(m_abbreviated);
            return m_writer.nextCarConsWriter(m_state);
        }

        public override VectorVisitor visit_Vector_car()
        {
            m_writer.Head(m_abbreviated);
            return m_writer.nextCarVectorWriter(m_state);
        }

        public override void visit_car()
        {
            m_writer.Head(m_abbreviated).Append("()");
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            m_writer.Car(m_atom, m_abbreviated);
            return m_writer.nextCdrAtomWriter(m_state);
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            m_writer.Abbrev(m_atom, m_abbreviated, out m_is_quote);
            return m_writer.nextCdrConsWriter(m_is_quote, m_state, m_atom);
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            m_writer.Car(m_atom, m_abbreviated);
            return m_writer.nextCdrVectorWriter(m_state);
        }

        public override void visit_cdr()
        {
            m_writer.Car(m_atom, m_abbreviated);
        }
    }

    public class AtomWriter : AtomVisitor {
        SexpWriter m_writer;
        bool m_abbreviated;

        public AtomWriter(SexpWriter writer)
            : this(writer, false)
        {}

        public AtomWriter(SexpWriter writer, bool abbreviated)
        {
            m_writer = writer;
            m_abbreviated = abbreviated;
        }

        public override void visit(object o)
        {
            m_writer.Head(m_abbreviated).Append(Literal.literal(o)).Foot();
        }
    }

#if false
    public class TopLevelWriter : VectorVisitor {
        Writer m_writer;

        public TopLevelWriter(Writer writer)
        {
            m_writer = writer;
        }

        public override AtomVisitor visitItem_Atom()
        {
            return new TopAtomWriter(m_writer);
        }

        public override ConsVisitor visitItem_Cons()
        {
            return new ConsCarWriter(m_writer, StringUtil.repeat(System.Environment.NewLine, 2));// TopConsWriter(m_writer);
        }

        public override VectorVisitor visitItem_Vector()
        {
            return new TopVectorWriter(m_writer);
        }

        public override void visitItem()
        {
            m_writer.Append("()").End().End();
        }
    }

    public class TopAtomWriter : AtomVisitor {
        Writer m_writer;

        public TopAtomWriter(Writer writer)
        {
            m_writer = writer;
        }

        public override void visit(object o)
        {
            m_writer.Append(Literal.literal(o)).End().End();
        }
    }

    public class TopVectorWriter : VectorVisitor {
        Writer m_writer;
        bool is_first = true;

        public TopVectorWriter(Writer writer)
        {
            m_writer = writer;
        }

        public override void visit()
        {
            m_writer.Append("#(");
        }

        public override void visitEnd()
        {
            m_writer.Append(")").End().End();
        }

        public void space()
        {
            if (is_first) {
                is_first = false;
            } else {
                m_writer.Append(' ');
            }
        }

        public override AtomVisitor visitItem_Atom()
        {
            space();
            return new OldAtomWriter(m_writer);
        }

        public override ConsVisitor visitItem_Cons()
        {
            space();
            return new ConsCarWriter(m_writer, "");
        }

        public override VectorVisitor visitItem_Vector()
        {
            space();
            return new OldVectorWriter(m_writer);
        }

        public override void visitItem()
        {
            space();
            m_writer.Append("()");
        }
    }

    //public class TopConsWriter : ConsVisitor {
    //    Writer m_writer;

    //    public TopConsWriter(Writer writer)
    //    {
    //        m_writer = writer;
    //    }

    //    public override void visit()
    //    {
    //        m_writer.Append("(");
    //        m_writer.Indent();
    //    }

    //    public override void visitEnd()
    //    {
    //        m_writer.Append(")").End().End();
    //        m_writer.Unindent();
    //    }

    //    public override AtomVisitor visit_Atom_car()
    //    {
    //        return new AtomWriter(m_writer);
    //    }

    //    public override ConsVisitor visit_Cons_car()
    //    {
    //        return new ConsCarWriter(m_writer, "");
    //    }

    //    public override VectorVisitor visit_Vector_car()
    //    {
    //        return new VectorWriter(m_writer);
    //    }

    //    public override AtomVisitor visit_Atom_cdr()
    //    {
    //        m_writer.Append(" . ");
    //        return new AtomWriter(m_writer);
    //    }

    //    public override ConsVisitor visit_Cons_cdr()
    //    {
    //        return new ConsCdrWriter(m_writer);
    //    }

    //    public override VectorVisitor visit_Vector_cdr()
    //    {
    //        m_writer.Append(" . ");
    //        return new VectorWriter(m_writer);
    //    }

    //    public override void visit_car()
    //    {
    //        m_writer.Append("()");
    //    }
    //}

    public class OldVectorWriter : VectorVisitor {
        Writer m_writer;
        bool is_first = true;

        public OldVectorWriter(Writer writer)
        {
            m_writer = writer;
        }

        public override void visit()
        {
            m_writer.Append("#(");
        }

        public override void visitEnd()
        {
            m_writer.Append(")");
        }

        public void space()
        {
            if (is_first) {
                is_first = false;
            } else {
                m_writer.Append(' ');
            }
        }

        public override AtomVisitor visitItem_Atom()
        {
            space();
            return new OldAtomWriter(m_writer);
        }

        public override ConsVisitor visitItem_Cons()
        {
            space();
            return new ConsCarWriter(m_writer, "");
        }

        public override VectorVisitor visitItem_Vector()
        {
            space();
            return new OldVectorWriter(m_writer);
        }

        public override void visitItem()
        {
            space();
            m_writer.Append("()");
        }
    }

    //public class QuoteVectorWriter : VectorVisitor {
    //    Writer m_writer;
    //    bool is_first = true;

    //    public QuoteVectorWriter(Writer writer)
    //    {
    //        m_writer = writer;
    //    }

    //    public override void visit()
    //    {
    //        m_writer.Append("#(");
    //    }

    //    public override void visitEnd()
    //    {
    //        m_writer.Append(")");
    //    }

    //    public void space()
    //    {
    //        if (is_first) {
    //            is_first = false;
    //        } else {
    //            m_writer.Append(' ');
    //        }
    //    }

    //    public override AtomVisitor visitItem_Atom()
    //    {
    //        space();
    //        return new OldAtomWriter(m_writer);
    //    }

    //    public override ConsVisitor visitItem_Cons()
    //    {
    //        space();
    //        return new QuoteCarWriter(m_writer);
    //    }

    //    public override VectorVisitor visitItem_Vector()
    //    {
    //        space();
    //        return new QuoteVectorWriter(m_writer);
    //    }

    //    public override void visitItem()
    //    {
    //        space();
    //        m_writer.Append("()");
    //    }
    //}

    public class ConsCarWriter : ConsVisitor {
        Writer m_writer;
        AtomCtor m_args;
        bool m_is_quote = false;
        string m_eol;

        public ConsCarWriter(Writer writer, string eol)
        {
            m_writer = writer;
            m_eol = eol;
        }

        public override void visitEnd()
        {
            if (!m_is_quote) {
                m_writer.Unindent();
                m_writer.Append(")");
            }

            m_writer.Append(m_eol);
        }

        public override AtomVisitor visit_Atom_car()
        {
            m_args = new AtomCtor();
            return new AtomBuilder(m_args);
        }

        public override ConsVisitor visit_Cons_car()
        {
            m_writer.Append("(");
            m_writer.Indent();
            return new ConsCarWriter(m_writer, "");
        }

        public override VectorVisitor visit_Vector_car()
        {
            m_writer.Append("(");
            m_writer.Indent();
            return new OldVectorWriter(m_writer);
        }

        void write_head()
        {
            if (m_args != null) {
                m_writer.Append("(").Append(Literal.literal(m_args.value));
                m_writer.Indent();
            }
        }

        void write_abbrev()
        {
            if (m_args != null) {
                if (m_args.value is Symbol) {
                    if (((Symbol)m_args.value).name == "quote") {
                        m_writer.Append("'");
                        m_is_quote = true;
                        return;
                    } else if (((Symbol)m_args.value).name == "quasiquotation") {
                        m_writer.Append("`");
                        m_is_quote = true;
                        return;
                    } else if (((Symbol)m_args.value).name == "unquote") {
                        m_writer.Append(",");
                        m_is_quote = true;
                        return;
                    } else if (((Symbol)m_args.value).name == "unquote-splicing") {
                        m_writer.Append(",@");
                        m_is_quote = true;
                        return;
                    }
                }

                m_writer.Append("(").Append(Literal.literal(m_args.value));
                m_writer.Indent();
            }
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            write_head();
            m_writer.Append(" . ");
            return new OldAtomWriter(m_writer);
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            write_abbrev();

            if (m_is_quote) {
                return new FooCdrWriter(m_writer);
            } else {
                return new ConsCdrWriter(m_writer);
            }
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            write_head();
            m_writer.Append(" . ");
            return new OldVectorWriter(m_writer);
        }

        public override void visit_car()
        {
            m_writer.Append("()");
        }

        public override void visit_cdr()
        {
            write_head();
        }
    }

    public class ConsCdrWriter : ConsVisitor {
        Writer m_writer;

        public ConsCdrWriter(Writer writer)
        {
            m_writer = writer;
        }

        public override AtomVisitor visit_Atom_car()
        {
            m_writer.End().Begin();// Append(' ');
            return new OldAtomWriter(m_writer);
        }

        public override ConsVisitor visit_Cons_car()
        {
            m_writer.End().Begin();// Append(' ');
            return new ConsCarWriter(m_writer, "");
        }

        public override VectorVisitor visit_Vector_car()
        {
            m_writer.End().Begin();// Append(' ');
            return new OldVectorWriter(m_writer);
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            m_writer.Append(" . ");
            return new OldAtomWriter(m_writer);
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            return new ConsCdrWriter(m_writer);
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            m_writer.Append(" . ");
            return new OldVectorWriter(m_writer);
        }

        public override void visit_car()
        {
            m_writer.Append(" ()");
        }
    }

    public class OldAtomWriter : AtomVisitor {
        Writer m_writer;

        public OldAtomWriter(Writer writer)
        {
            m_writer = writer;
        }

        public override void visit(object o)
        {
            m_writer.Append(Literal.literal(o));
        }
    }

    //public class QuoteCarWriter : ConsVisitor {
    //    Writer m_writer;

    //    public QuoteCarWriter(Writer writer)
    //    {
    //        m_writer = writer;
    //    }

    //    public override void visit()
    //    {
    //        m_writer.Append("(");
    //    }

    //    public override void visitEnd()
    //    {
    //        m_writer.Append(")");
    //    }

    //    public override AtomVisitor visit_Atom_car()
    //    {
    //        return new OldAtomWriter(m_writer);
    //    }

    //    public override ConsVisitor visit_Cons_car()
    //    {
    //        return new QuoteCarWriter(m_writer);
    //    }

    //    public override VectorVisitor visit_Vector_car()
    //    {
    //        return new QuoteVectorWriter(m_writer);
    //    }

    //    public override AtomVisitor visit_Atom_cdr()
    //    {
    //        m_writer.Append(" . ");
    //        return new OldAtomWriter(m_writer);
    //    }

    //    public override ConsVisitor visit_Cons_cdr()
    //    {
    //        return new QuoteCdrWriter(m_writer);
    //    }

    //    public override VectorVisitor visit_Vector_cdr()
    //    {
    //        m_writer.Append(" . ");
    //        return new QuoteVectorWriter(m_writer);
    //    }

    //    public override void visit_car()
    //    {
    //        m_writer.Append("()");
    //    }
    //}

    //public class QuoteCdrWriter : ConsVisitor {
    //    Writer m_writer;

    //    public QuoteCdrWriter(Writer writer)
    //    {
    //        m_writer = writer;
    //    }

    //    public override void visit()
    //    {
    //    }

    //    public override void visitEnd()
    //    {
    //    }

    //    public override AtomVisitor visit_Atom_car()
    //    {
    //        m_writer.Append(' ');
    //        return new OldAtomWriter(m_writer);
    //    }

    //    public override ConsVisitor visit_Cons_car()
    //    {
    //        m_writer.Append(' ');
    //        return new QuoteCarWriter(m_writer);
    //    }

    //    public override VectorVisitor visit_Vector_car()
    //    {
    //        m_writer.Append(' ');
    //        return new QuoteVectorWriter(m_writer);
    //    }

    //    public override AtomVisitor visit_Atom_cdr()
    //    {
    //        m_writer.Append(" . ");
    //        return new OldAtomWriter(m_writer);
    //    }

    //    public override ConsVisitor visit_Cons_cdr()
    //    {
    //        return new QuoteCdrWriter(m_writer);
    //    }

    //    public override VectorVisitor visit_Vector_cdr()
    //    {
    //        m_writer.Append(" . ");
    //        return new QuoteVectorWriter(m_writer);
    //    }

    //    public override void visit_car()
    //    {
    //        m_writer.Append(" ()");
    //    }
    //}

    public class FooCdrWriter : ConsVisitor {
        Writer m_writer;

        public FooCdrWriter(Writer writer)
        {
            m_writer = writer;
        }

        public override AtomVisitor visit_Atom_car()
        {
            return new OldAtomWriter(m_writer);
        }

        public override ConsVisitor visit_Cons_car()
        {
            return new ConsCarWriter(m_writer, "");
        }

        public override VectorVisitor visit_Vector_car()
        {
            return new OldVectorWriter(m_writer);
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            throw new Exception();
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            throw new Exception();
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            throw new Exception();
        }

        public override void visit_car()
        {
            m_writer.Append("()");
        }
    }

#if false
    public interface IWritable {
        void write(Writer writer);
    }

    public class TopLevelWriter : VectorVisitor, IWritable {
        Writer m_writer;
        List<object> top = new List<object>();

        public TopLevelWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void write(Writer writer)
        {
            foreach (IWritable w in top) {
                if (w != null) {
                    w.write(writer);
                } else {
                    writer.Append("()");
                }

                writer.End();
                writer.End();
            }
        }

        public override void visitEnd()
        {
            write(m_writer);
        }

        public override AtomVisitor visitItem_Atom()
        {
            AtomWriter atom = new AtomWriter();
            top.Add(atom);
            return atom;
        }

        public override ConsVisitor visitItem_Cons()
        {
            ListWriter cons = new ListWriter();
            top.Add(cons);
            return cons;
        }

        public override VectorVisitor visitItem_Vector()
        {
            VectorWriter vect = new VectorWriter();
            top.Add(vect);
            return vect;
        }

        public override void visitItem()
        {
            top.Add(null);
        }
    }

    public class VectorWriter : VectorVisitor, IWritable {
        List<object> vect = new List<object>();

        public void write(Writer writer)
        {
            bool is_first = true;

            writer.Append("#(");

            foreach (IWritable w in vect) {
                if (is_first) {
                    is_first = false;
                } else {
                    writer.Append(' ');
                }

                if (w != null) {
                    w.write(writer);
                } else {
                    writer.Append("()");
                }
            }

            writer.Append(")");
        }

        public override AtomVisitor visitItem_Atom()
        {
            AtomWriter atom = new AtomWriter();
            vect.Add(atom);
            return atom;
        }

        public override ConsVisitor visitItem_Cons()
        {
            ListWriter list = new ListWriter();
            vect.Add(list);
            return list;
        }

        public override VectorVisitor visitItem_Vector()
        {
            VectorWriter new_vect = new VectorWriter();
            vect.Add(new_vect);
            return new_vect;
        }

        public override void visitItem()
        {
            vect.Add(null);
        }
    }

    public class AtomWriter : AtomVisitor, IWritable {
        string m_literal;
        public object value;

        public void write(Writer writer)
        {
            writer.Append(m_literal);
        }

        public override void visit(object v)
        {
            m_literal = Literal.literal(v);
            value = v;
        }
    }

    public class ListWriter : ConsVisitor, IWritable {
        IWritable car;
        IWritable cdr;

        public void write(Writer writer)
        {
            bool is_quote = false;
            string quote_sym = "";
            string quote_ch = "";

            // TODO - clean up and generalize this 
            if (car is AtomWriter) {
                AtomWriter atom = (AtomWriter)car;
                if (atom.value is Symbol) {
                    Symbol sym = atom.value as Symbol;
                    if (sym.name == "quote") {
                        is_quote = true;
                        quote_sym = "quote";
                        quote_ch = "'";
                    } else if (sym.name == "quasiquotation") {
                        is_quote = true;
                        quote_sym = "quasiquotation";
                        quote_ch = "`";
                    } else if (sym.name == "unquote") {
                        is_quote = true;
                        quote_sym = "unquote";
                        quote_ch = ",";
                    } else if (sym.name == "unquote-splicing") {
                        is_quote = true;
                        quote_sym = "unquote-splicing";
                        quote_ch = ",@";
                    }
                }
            }

            if (is_quote) {
                if (cdr == null) {
                    writer.Append("("+quote_sym+")");
                } else {
                    writer.Append(quote_ch);
                    cdr.write(writer);
                }
            } else {
                writer.Indent();
                writer.Append("(");

                if (car != null) {
                    car.write(writer);
                } else {
                    writer.Append("()");
                }

                if (cdr != null) {
                    if (!(cdr is ConsWriter)) {
                        writer.Append(" . ");
                    } else {
                        writer.End();
                    }

                    cdr.write(writer);
                }

                writer.Unindent();
                writer.Append(")");
            }
        }

        public override AtomVisitor visit_Atom_car()
        {
            AtomWriter atom = new AtomWriter();
            car = atom;
            return atom;
        }

        public override ConsVisitor visit_Cons_car()
        {
            ListWriter list = new ListWriter();
            car = list;
            return list;
        }

        public override VectorVisitor visit_Vector_car()
        {
            VectorWriter vect = new VectorWriter();
            car = vect;
            return vect;
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            AtomWriter atom = new AtomWriter();
            cdr = atom;
            return atom;
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            ConsWriter cons = new ConsWriter();
            cdr = cons;
            return cons;
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            VectorWriter vect = new VectorWriter();
            cdr = vect;
            return vect;
        }
    }

    public class ConsWriter : ConsVisitor, IWritable {
        IWritable car;
        IWritable cdr;

        public void write(Writer writer)
        {
            if (car != null) {
                car.write(writer);
            } else {
                writer.Append("()");
            }

            if (cdr != null) {
                writer.End();
                cdr.write(writer);
            }
        }

        public override AtomVisitor visit_Atom_car()
        {
            AtomWriter atom = new AtomWriter();
            car = atom;
            return atom;
        }

        public override ConsVisitor visit_Cons_car()
        {
            ListWriter list = new ListWriter();
            car = list;
            return list;
        }

        public override VectorVisitor visit_Vector_car()
        {
            VectorWriter vect = new VectorWriter();
            car = vect;
            return vect;
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            AtomWriter atom = new AtomWriter();
            cdr = atom;
            return atom;
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            ConsWriter cons = new ConsWriter();
            cdr = cons;
            return cons;
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            VectorWriter vect = new VectorWriter();
            cdr = vect;
            return vect;
        }
    }
#endif
#endif

}
