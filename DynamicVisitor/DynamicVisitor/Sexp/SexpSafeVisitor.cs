using System;

namespace Sexp {
    public class SafeVectorVisitor : VectorVisitor {
        VectorVisitor m_vect;

        public SafeVectorVisitor(VectorVisitor vect)
        {
            m_vect = vect;
        }

        public override void visit() { if (m_vect != null) m_vect.visit(); }
        public override void visitEnd() { if (m_vect != null) m_vect.visitEnd(); }
        public override AtomVisitor visitItem_Atom() { return new SafeAtomVisitor(m_vect != null ? m_vect.visitItem_Atom() : null); }
        public override ConsVisitor visitItem_Cons() { return new SafeConsVisitor(m_vect != null ? m_vect.visitItem_Cons() : null); }
        public override VectorVisitor visitItem_Vector() { return new SafeVectorVisitor(m_vect != null ? m_vect.visitItem_Vector() : null); }
    }

    public class SafeAtomVisitor : AtomVisitor {
        AtomVisitor m_atom;

        public SafeAtomVisitor(AtomVisitor atom)
        {
            m_atom = atom;
        }

        public override void visit() { if (m_atom != null) m_atom.visit(); }
        public override void visitEnd() { if (m_atom != null) m_atom.visitEnd(); }
        public override void visit_value(Boolean o) { if (m_atom != null) m_atom.visit_value(o); }
        public override void visit_value(Int64 o) { if (m_atom != null) m_atom.visit_value(o); }
        public override void visit_value(Double o) { if (m_atom != null) m_atom.visit_value(o); }
        public override void visit_value(String o) { if (m_atom != null) m_atom.visit_value(o); }
        public override void visit_value(Char o) { if (m_atom != null) m_atom.visit_value(o); }
        public override void visit_value(Object o) { if (m_atom != null) m_atom.visit_value(o); }
        public override void visit_value(Symbol o) { if (m_atom != null) m_atom.visit_value(o); }
    }

    public class SafeConsVisitor : ConsVisitor {
        ConsVisitor m_cons;

        public SafeConsVisitor(ConsVisitor cons)
        {
            m_cons = cons;
        }

        public override void visit() { if (m_cons != null) m_cons.visit(); }
        public override void visitEnd() { if (m_cons != null) m_cons.visitEnd(); }
        public override AtomVisitor visit_Atom_car() { return new SafeAtomVisitor(m_cons != null ? m_cons.visit_Atom_car() : null); }
        public override ConsVisitor visit_Cons_car() { return new SafeConsVisitor(m_cons != null ? m_cons.visit_Cons_car() : null); }
        public override VectorVisitor visit_Vector_car() { return new SafeVectorVisitor(m_cons != null ? m_cons.visit_Vector_car() : null); }
        public override AtomVisitor visit_Atom_cdr() { return new SafeAtomVisitor(m_cons != null ? m_cons.visit_Atom_cdr() : null); }
        public override ConsVisitor visit_Cons_cdr() { return new SafeConsVisitor(m_cons != null ? m_cons.visit_Cons_cdr() : null); }
        public override VectorVisitor visit_Vector_cdr() { return new SafeVectorVisitor(m_cons != null ? m_cons.visit_Vector_cdr() : null); }
    }

    //public class SafeSymbolVisitor : SymbolVisitor {
    //    SymbolVisitor m_sym;

    //    public SafeSymbolVisitor(SymbolVisitor sym)
    //    {
    //        m_sym = sym;
    //    }

    //    public override void visit() { if (m_sym != null) m_sym.visit(); }
    //    public override void visitEnd() { if (m_sym != null) m_sym.visitEnd(); }
    //    public override void visit_name(string name) { if (m_sym != null) m_sym.visit_name(name); }
    //}
}
