using System;
using System.Collections.Generic;
using System.Text;

namespace Sexp {
    public interface DatumBuilder {
        Datum getDatum();
    }

    public class VectorBuilder : VectorVisitor, DatumBuilder {
        List<DatumVisitor> m_datum = new List<DatumVisitor>();
        Vector m_vect;

        public Datum getDatum()
        {
            return m_vect;
        }

        public override void visitEnd()
        {
            m_vect = new Vector();

            foreach (DatumBuilder o in m_datum) {
                if (o != null) {
                    m_vect.Add(o.getDatum());
                } else {
                    m_vect.Add(null);
                }
            }
        }

        public override AtomVisitor visitItem_Atom()
        {
            AtomBuilder atom = new AtomBuilder();
            m_datum.Add(atom);
            return atom;
        }

        public override ConsVisitor visitItem_Cons()
        {
            ConsBuilder cons = new ConsBuilder();
            m_datum.Add(cons);
            return cons;
        }

        public override VectorVisitor visitItem_Vector()
        {
            VectorBuilder vect = new VectorBuilder();
            m_datum.Add(vect);
            return vect;
        }

        public override void visitItem(object o)
        {
            if (o == null) {
                m_datum.Add(null);
            } else {
                throw new Exception();
            }
        }
    }

    public class TopLevelBuilder : VectorBuilder {
        public Vector getTopLevel()
        {
            return getDatum() as Vector;
        }
    }

    public class AtomBuilder : AtomVisitor, DatumBuilder {
        object m_value;
        Atom m_atom;

        public Datum getDatum()
        {
            return m_atom;
        }

        public override void visitEnd()
        {
            m_atom = new Atom(m_value);
        }

        public override void visit_value(Boolean o)
        {
            m_value = o;
        }

        public override void visit_value(Int64 o)
        {
            m_value = o;
        }

        public override void visit_value(Double o)
        {
            m_value = o;
        }

        public override void visit_value(Char o)
        {
            m_value = o;
        }

        public override void visit_value(String o)
        {
            m_value = o;
        }

        public override void visit_value(Symbol o)
        {
            m_value = o;
        }
    }

    public class ConsBuilder : ConsVisitor, DatumBuilder {
        DatumBuilder m_car;
        DatumBuilder m_cdr;
        Cons m_cons;

        public Datum getDatum()
        {
            return m_cons;
        }

        public override void visitEnd()
        {
            m_cons = new Cons(
                m_car != null ? m_car.getDatum() : null,
                m_cdr != null ? m_cdr.getDatum() : null);
        }

        public override AtomVisitor visit_Atom_car()
        {
            m_car = new AtomBuilder();
            return m_car as AtomBuilder;
        }

        public override ConsVisitor visit_Cons_car()
        {
            m_car = new ConsBuilder();
            return m_car as ConsBuilder;
        }

        public override VectorVisitor visit_Vector_car()
        {
            m_car = new VectorBuilder();
            return m_car as VectorBuilder;
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            m_cdr = new AtomBuilder();
            return m_cdr as AtomBuilder;
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            m_cdr = new ConsBuilder();
            return m_cdr as ConsBuilder;
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            m_cdr = new VectorBuilder();
            return m_cdr as VectorBuilder;
        }

        public override void visit_car(object o)
        {
            if (o != null) throw new Exception();
        }

        public override void visit_cdr(object o)
        {
            if (o != null) throw new Exception();
        }
    }
}
