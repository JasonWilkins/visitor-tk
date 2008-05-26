using System;

namespace Sexp {
    public abstract class AtomVisitor {
        public abstract void visit(Object o);

        public virtual void visit(Boolean o) { visit((Object)o); }
        public virtual void visit(Int64 o) { visit((Object)o); }
        public virtual void visit(Double o) { visit((Object)o); }
        public virtual void visit(String o) { visit((Object)o); }
        public virtual void visit(Char o) { visit((Object)o); }
        public virtual void visit(Symbol o) { visit((Object)o); }
    }

    public abstract class CompoundDatumVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }
    }

    public abstract class VectorVisitor : CompoundDatumVisitor {
        public abstract AtomVisitor visitItem_Atom();
        public abstract ConsVisitor visitItem_Cons();
        public abstract VectorVisitor visitItem_Vector();
        public virtual void visitItem() { }
    }

    public abstract class ConsVisitor : CompoundDatumVisitor {
        public abstract AtomVisitor visit_Atom_car();
        public abstract ConsVisitor visit_Cons_car();
        public abstract VectorVisitor visit_Vector_car();
        public abstract AtomVisitor visit_Atom_cdr();
        public abstract ConsVisitor visit_Cons_cdr();
        public abstract VectorVisitor visit_Vector_cdr();
        public virtual void visit_car() { }
        public virtual void visit_cdr() { }
    }
}
