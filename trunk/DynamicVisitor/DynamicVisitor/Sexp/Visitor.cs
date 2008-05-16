using System;

namespace Sexp {
    public abstract class DatumVisitor {
        public virtual void visit() {}
        public virtual void visitEnd() {}
    }

    public abstract class VectorVisitor : DatumVisitor {
        public abstract AtomVisitor visitItem_Atom();
        public abstract ConsVisitor visitItem_Cons();
        public abstract VectorVisitor visitItem_Vector();
        public abstract void visitItem(object o);
    }

    public abstract class AtomVisitor : DatumVisitor {
        public abstract void visit_value(Boolean o);
        public abstract void visit_value(Int64 o);
        public abstract void visit_value(Double o);
        public abstract void visit_value(String o);
        public abstract void visit_value(Char o);
        public abstract void visit_value(Symbol o);
    }

    public abstract class ConsVisitor : DatumVisitor {
        public abstract AtomVisitor visit_Atom_car();
        public abstract ConsVisitor visit_Cons_car();
        public abstract VectorVisitor visit_Vector_car();
        public abstract AtomVisitor visit_Atom_cdr();
        public abstract ConsVisitor visit_Cons_cdr();
        public abstract VectorVisitor visit_Vector_cdr();
        public abstract void visit_car(object o);
        public abstract void visit_cdr(object o);
    }
}
