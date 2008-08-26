using Symbols;

namespace Sexp {
    public abstract class AtomVisitor {
        public abstract void visit(object o);
        public virtual void visit(bool o) { visit((object)o); }
        public virtual void visit(long o) { visit((object)o); }
        public virtual void visit(float o) { visit((object)o); }
        public virtual void visit(double o) { visit((object)o); }
        public virtual void visit(Rational o) { visit((object)o); }
        public virtual void visit(Complex o) { visit((object)o); }
        public virtual void visit(string o) { visit((object)o); }
        public virtual void visit(char o) { visit((object)o); }
        public virtual void visit(Symbol o) { visit((object)o); }
    }

    public abstract class VectVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }
        public abstract AtomVisitor visitItem_Atom();
        public abstract ConsVisitor visitItem_Cons();
        public abstract VectVisitor visitItem_Vect();
        public virtual void visitItem() { }
    }

    public abstract class ConsVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }
        public abstract AtomVisitor visit_Atom_car();
        public abstract ConsVisitor visit_Cons_car();
        public abstract VectVisitor visit_Vect_car();
        public abstract AtomVisitor visit_Atom_cdr();
        public abstract ConsVisitor visit_Cons_cdr();
        public abstract VectVisitor visit_Vect_cdr();
        public virtual void visit_car() { }
        public virtual void visit_cdr() { }
    }
}
