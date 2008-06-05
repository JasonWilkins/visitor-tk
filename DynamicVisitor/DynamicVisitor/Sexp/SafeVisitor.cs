using System;

using Symbols;

namespace Sexp {
    public class SafeVectorVisitor : VectVisitor {
        VectVisitor m_vect;

        public SafeVectorVisitor(VectVisitor vect)
        {
            m_vect = vect;
        }

        public override void visit() { if (m_vect != null) m_vect.visit(); }
        public override void visitEnd() { if (m_vect != null) m_vect.visitEnd(); }
        public override AtomVisitor visitItem_Atom() { return new SafeAtomVisitor(m_vect != null ? m_vect.visitItem_Atom() : null); }
        public override ConsVisitor visitItem_Cons() { return new SafeConsVisitor(m_vect != null ? m_vect.visitItem_Cons() : null); }
        public override VectVisitor visitItem_Vect() { return new SafeVectorVisitor(m_vect != null ? m_vect.visitItem_Vect() : null); }
        public override void visitItem() { if (m_vect != null) m_vect.visitItem(); }
    }

    public class SafeAtomVisitor : AtomVisitor {
        AtomVisitor m_atom;

        public SafeAtomVisitor(AtomVisitor atom)
        {
            m_atom = atom;
        }

        public override void visit(object o) { if (m_atom != null) m_atom.visit(o); }
        public override void visit(Boolean o) { if (m_atom != null) m_atom.visit(o); }
        public override void visit(Int64 o) { if (m_atom != null) m_atom.visit(o); }
        public override void visit(Double o) { if (m_atom != null) m_atom.visit(o); }
        public override void visit(String o) { if (m_atom != null) m_atom.visit(o); }
        public override void visit(Char o) { if (m_atom != null) m_atom.visit(o); }
        public override void visit(Symbol o) { if (m_atom != null) m_atom.visit(o); }
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
        public override VectVisitor visit_Vect_car() { return new SafeVectorVisitor(m_cons != null ? m_cons.visit_Vect_car() : null); }
        public override AtomVisitor visit_Atom_cdr() { return new SafeAtomVisitor(m_cons != null ? m_cons.visit_Atom_cdr() : null); }
        public override ConsVisitor visit_Cons_cdr() { return new SafeConsVisitor(m_cons != null ? m_cons.visit_Cons_cdr() : null); }
        public override VectVisitor visit_Vect_cdr() { return new SafeVectorVisitor(m_cons != null ? m_cons.visit_Vect_cdr() : null); }
        public override void visit_car() { if (m_cons != null) m_cons.visit_car(); }
        public override void visit_cdr() { if (m_cons != null) m_cons.visit_cdr(); }
    }
}
