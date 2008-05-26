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
        //public SexpWriter[] cfg = new SexpWriter[(int)State.COUNT];
        public SexpWriter top;
        public SexpWriter top_vect;
        public SexpWriter top_cons;
        public SexpWriter top_atom;
        public SexpWriter car_vect;
        public SexpWriter car_cons;
        public SexpWriter car_atom;
        public SexpWriter cdr_cons;
        public SexpWriter cdr_vect;
        public SexpWriter cdr_atom;
        public SexpWriter quote_cons;
        public SexpWriter quote_vect;
        public SexpWriter quote_atom;
        public SexpWriter item_vect;
        public SexpWriter item_cons;
        public SexpWriter item_atom;
        public SexpWriter car_item_vect;
        public SexpWriter car_item_cons;
        public SexpWriter car_item_atom;
        public SexpWriter cdr_item_vect;
        public SexpWriter cdr_item_cons;
        public SexpWriter cdr_item_atom;
        public SexpWriter first_car_cons;
        public SexpWriter first_cdr_cons;
        public SexpWriter quote_cdr_cons;
    }

    public static class MakeConfig {
        public static SexpWriterConfig make_basic_config(Writer writer)
        {
            SexpWriterConfig rv = new SexpWriterConfig();

            SexpWriter top = new SexpWriter(writer, rv);
            top.sep = "";
            top.sepline = 2;
            top.footline = 1;

            SexpWriter cons = new SexpWriter(writer, rv);
            cons.indent = true;
            cons.abbreviate = true;
            cons.head = "(";
            cons.foot = ")";

            SexpWriter first_car_cons = new SexpWriter(writer, rv);
            first_car_cons.indent = false;
            first_car_cons.abbreviate = true;
            first_car_cons.head = "(";
            first_car_cons.foot = ")";

            SexpWriter first_cdr_cons = new SexpWriter(writer, rv);
            first_cdr_cons.head = " ";
            first_cdr_cons.foot = "";

            SexpWriter cdr_cons = new SexpWriter(writer, rv);
            cdr_cons.head = "";
            cdr_cons.headline = 1;
            cdr_cons.foot = "";

            SexpWriter top_vect = new SexpWriter(writer, rv);
            top_vect.head = "#(";
            top_vect.foot = ")";

            SexpWriter vect = new SexpWriter(writer, rv);
            vect.head = " #(";
            vect.foot = ")";

            SexpWriter cdr_vect = new SexpWriter(writer, rv);
            cdr_vect.head = " . #(";
            cdr_vect.foot = ")";

            SexpWriter atom = new SexpWriter(writer, rv);

            SexpWriter cdr_atom = new SexpWriter(writer, rv);
            cdr_atom.head = " . ";

            SexpWriter quote_cons = new SexpWriter(writer, rv);
            quote_cons.abbreviate = true;

            SexpWriter quote_vect = new SexpWriter(writer, rv);
            quote_vect.head = "#(";
            quote_vect.foot = ")";

            SexpWriter quote_cdr_cons = new SexpWriter(writer, rv);

            rv.top = top;
            rv.top_vect = top_vect;
            rv.top_cons = cons;
            rv.top_atom = atom;
            rv.car_vect = vect;
            rv.car_cons = cons;
            rv.car_atom = atom;
            rv.cdr_cons = cdr_cons;
            rv.cdr_vect = cdr_vect;
            rv.cdr_atom = cdr_atom;
            rv.quote_cons = quote_cons;
            rv.quote_vect = quote_vect;
            rv.quote_atom = atom;
            rv.item_vect = vect;
            rv.item_cons = cons;
            rv.item_atom = atom;
            rv.car_item_vect = vect;
            rv.car_item_cons = cons;
            rv.car_item_atom = atom;
            rv.cdr_item_vect = cdr_vect;
            rv.cdr_item_cons = cdr_cons;
            rv.cdr_item_atom = cdr_atom;
            rv.first_car_cons = first_car_cons;
            rv.first_cdr_cons = first_cdr_cons;
            rv.quote_cdr_cons = quote_cdr_cons;
            return rv;
        }
    }

    public enum State {
        TOP,
        TOP_VECT,
        CAR_VECT,
        CDR_VECT,
        ITEM_VECT,
        TOP_CONS,
        CAR_CONS,
        CDR_CONS,
        ITEM_CONS,
        QUOTE_CONS,
        QUOTE_ATOM,
        QUOTE_VECT,
        //CAR_ATOM,
        //TOP_ATOM,
        //ITEM_ATOM,
        CDR_ITEM_VECT,
        CDR_ITEM_CONS,
        //CDR_ITEM_ATOM,
        CDR_ATOM,
        CAR_ITEM_VECT,
        CAR_ITEM_CONS,
        //CAR_ITEM_ATOM,
        FIRST_CAR_CONS,
        FIRST_CDR_CONS,
        QUOTE_CDR_CONS,
        COUNT
    }

    public class SexpWriter {
        public bool abbreviate = false;
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
            } else if (
                State.TOP_VECT == s ||
                State.ITEM_VECT == s ||
                State.CAR_VECT == s ||
                State.CAR_ITEM_VECT == s ||
                State.CDR_VECT == s ||
                State.CDR_ITEM_VECT == s ||
                State.QUOTE_VECT == s) {

                return new VectorWriter(m_cfg.item_vect, State.ITEM_VECT);
            } else {
                throw new Exception();
            }
        }

        public ConsWriter nextItemConsWriter(State s)
        {
            if (State.TOP == s) {
                return new ConsWriter(m_cfg.top_cons, State.TOP_CONS);
            } else if (
                State.TOP_VECT == s ||
                State.ITEM_VECT == s ||
                State.CAR_VECT == s ||
                State.CAR_ITEM_VECT == s ||
                State.CDR_VECT == s ||
                State.CDR_ITEM_VECT == s ||
                State.QUOTE_VECT == s) {

                return new ConsWriter(m_cfg.item_cons, State.ITEM_CONS);
            } else {
                throw new Exception();
            }
        }

        public AtomWriter nextItemAtomWriter(State s)
        {
            if (State.TOP == s) {
                return new AtomWriter(m_cfg.top_atom);
            } else if (
                State.TOP_VECT == s ||
                State.ITEM_VECT == s ||
                State.CAR_VECT == s ||
                State.CAR_ITEM_VECT == s ||
                State.CDR_VECT == s ||
                State.CDR_ITEM_VECT == s ||
                State.QUOTE_VECT == s) {

                return new AtomWriter(m_cfg.item_atom);
            } else {
                throw new Exception();
            }
        }

        public VectorWriter nextCarVectorWriter(State s)
        {
            if (State.QUOTE_CONS == s) {
                return new VectorWriter(m_cfg.quote_vect, State.QUOTE_VECT);
            } else if (State.ITEM_CONS == s) {
                return new VectorWriter(m_cfg.item_vect, State.ITEM_VECT);
            } else if (State.TOP_CONS == s || State.FIRST_CAR_CONS == s || State.FIRST_CDR_CONS == s || State.QUOTE_CDR_CONS == s || State.CAR_CONS == s || State.CDR_CONS == s) {
                return new VectorWriter(m_cfg.car_vect, State.CAR_VECT);
            } else {
                throw new Exception();
            }
        }

        public ConsWriter nextCarConsWriter(State s)
        {
            if (State.ITEM_CONS == s) {
                return new ConsWriter(m_cfg.item_cons, State.ITEM_CONS);
            } else if (State.TOP_CONS == s || State.CAR_CONS == s || State.FIRST_CAR_CONS == s || State.FIRST_CDR_CONS == s || State.QUOTE_CDR_CONS == s) {
                return new ConsWriter(m_cfg.first_car_cons, State.FIRST_CAR_CONS);
            } else if (State.CDR_CONS == s || State.QUOTE_CONS == s) {
                return new ConsWriter(m_cfg.car_cons, State.CAR_CONS);
            } else {
                throw new Exception();
            }
        }

        public AtomVisitor nextCarAtomWriter(State s, out AtomCtor atom)
        {
            //if (abbreviate) {
            atom = new AtomCtor();
            return new AtomBuilder(atom);
            //} else {
            //if (State.ABBREV == s) {
            //    return new AtomWriter(m_cfg.quote_atom, State.QUOTE_ATOM);
            //} else if (State.ITEM_CONS == s) {
            //    return new AtomWriter(m_cfg.item_atom, State.ITEM_ATOM);
            //} else if (State.TOP_CONS == s || State.CAR_CONS == s || State.CDR_CONS == s || State.QUOTE_CONS == s) {
            //    return new AtomWriter(m_cfg.car_atom, State.CAR_ATOM);
            //} else {
            //    throw new Exception();
            //}
            //}
        }

        public VectorWriter nextCdrVectorWriter(State s)
        {
            if (State.ITEM_CONS == s) {
                return new VectorWriter(m_cfg.cdr_item_vect, State.CDR_ITEM_VECT);
            } else if (State.TOP_CONS == s || State.FIRST_CAR_CONS == s || State.FIRST_CDR_CONS == s || State.QUOTE_CDR_CONS == s || State.CAR_CONS == s || State.CDR_CONS == s) {
                return new VectorWriter(m_cfg.cdr_vect, State.CDR_VECT);
            } else {
                throw new Exception();
            }
        }

        public ConsWriter nextCdrConsWriter(bool is_quote, State s)
        {
            if (State.ITEM_CONS == s) {
                return new ConsWriter(m_cfg.cdr_item_cons, State.CDR_ITEM_CONS);
            } else if (State.FIRST_CAR_CONS == s|| State.FIRST_CDR_CONS == s || State.QUOTE_CDR_CONS == s) {
                if (is_quote) {
                    return new ConsWriter(m_cfg.quote_cdr_cons, State.QUOTE_CDR_CONS);
                } else {
                    return new ConsWriter(m_cfg.first_cdr_cons, State.FIRST_CDR_CONS);
                }
            } else if (State.TOP_CONS == s || State.CAR_CONS == s || State.CDR_CONS == s || State.QUOTE_CONS == s) {
                if (is_quote) {
                    return new ConsWriter(m_cfg.quote_cons, State.QUOTE_CONS);
                } else {
                    return new ConsWriter(m_cfg.cdr_cons, State.CDR_CONS);
                }
            } else {
                throw new Exception();
            }
        }

        public AtomWriter nextCdrAtomWriter(State s)
        {
            if (State.ITEM_CONS == s) {
                return new AtomWriter(m_cfg.cdr_item_atom);
            } else if (State.TOP_CONS == s || State.CAR_CONS == s || State.CDR_CONS == s || State.QUOTE_CONS == s || State.FIRST_CAR_CONS == s || State.FIRST_CDR_CONS == s) {
                return new AtomWriter(m_cfg.cdr_atom);
            } else {
                throw new Exception();
            }
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

        public SexpWriter Head()
        {
            m_writer.Append(head);
            lines(headline);

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

        public SexpWriter Car(AtomCtor atom)
        {
            if (atom != null) {
                return Head().Append(Literal.literal(atom.value));
            }

            return this;
        }

        public SexpWriter Abbrev(AtomCtor atom, out bool is_quote)
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
            return Car(atom);
        }
    }

    public class VectorWriter : VectorVisitor {
        SexpWriter m_writer;
        State m_state;

        bool is_first = true;

        public VectorWriter(SexpWriter writer, State s)
        {
            m_state = s;
            m_writer = writer;
        }

        public override void visit()
        {
            m_writer.Head();
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

        public ConsWriter(SexpWriter writer, State s)
        {
            m_state = s;
            m_writer = writer;
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
            m_writer.Head();
            return m_writer.nextCarConsWriter(m_state);
        }

        public override VectorVisitor visit_Vector_car()
        {
            m_writer.Head();
            return m_writer.nextCarVectorWriter(m_state);
        }

        public override void visit_car()
        {
            m_writer.Head().Append("()");
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            m_writer.Car(m_atom);
            return m_writer.nextCdrAtomWriter(m_state);
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            m_writer.Abbrev(m_atom, out m_is_quote);
            return m_writer.nextCdrConsWriter(m_is_quote, m_state);
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            m_writer.Car(m_atom);
            return m_writer.nextCdrVectorWriter(m_state);
        }

        public override void visit_cdr()
        {
            m_writer.Car(m_atom);
        }
    }

    public class AtomWriter : AtomVisitor {
        SexpWriter m_writer;

        public AtomWriter(SexpWriter writer)
        {
            m_writer = writer;
        }

        public override void visit(object o)
        {
            m_writer.Head().Append(Literal.literal(o)).Foot();
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
