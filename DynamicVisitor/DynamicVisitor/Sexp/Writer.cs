using System;
using System.Text;
using System.Collections.Generic;

using Utils;

namespace Sexp {

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
                w.write(writer);
                writer.End().Begin();
                writer.End().Begin();
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

                w.write(writer);
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
    }

    public class AtomWriter : AtomVisitor, IWritable {
        string m_literal;
        public object value;

        public void write(Writer writer)
        {
            writer.Append(m_literal);
        }

        public override void visit_value(Boolean v)
        {
            m_literal = Literal.literal(v);
            value = v;
        }

        public override void visit_value(Int64 v)
        {
            m_literal = Literal.literal(v);
            value = v;
        }

        public override void visit_value(Double v)
        {
            m_literal = Literal.literal(v);
            value = v;
        }

        public override void visit_value(Char v)
        {
            m_literal = Literal.literal(v);
            value = v;
        }

        public override void visit_value(String v)
        {
            m_literal = Literal.literal(v);
            value = v;
        }

        public override void visit_value(Symbol v)
        {
            m_literal = Literal.literal(v);
            value = v;
        }

        public override void visit_value(Object o)
        {
            throw new InvalidOperationException("values of type " + o.GetType().Name + " are not supported");
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

                if (null != car) {
                    car.write(writer);
                }

                if (cdr != null) {
                    if (!(cdr is ConsWriter)) {
                        writer.Append(" . ");
                    } else {
                        writer.End().Begin();
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
            }

            if (cdr != null) {
                writer.End().Begin();
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
}
