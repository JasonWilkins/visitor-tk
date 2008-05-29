using System;

namespace Sexp {
    public class VectorMultiVisitor : VectVisitor {
        VectVisitor car;
        VectVisitor cdr;

        public VectorMultiVisitor(VectVisitor car, VectVisitor cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }

        public override void visit() { car.visit(); cdr.visit(); }
        public override void visitEnd() { car.visitEnd(); cdr.visitEnd(); }
        public override AtomVisitor visitItem_Atom() { return new AtomMultiVisitor(car.visitItem_Atom(), cdr.visitItem_Atom()); }
        public override ConsVisitor visitItem_Cons() { return new ConsMultiVisitor(car.visitItem_Cons(), cdr.visitItem_Cons()); }
        public override VectVisitor visitItem_Vect() { return new VectorMultiVisitor(car.visitItem_Vect(), cdr.visitItem_Vect()); }
        public override void visitItem() { car.visitItem(); cdr.visitItem(); }
    }

    public class AtomMultiVisitor : AtomVisitor {
        AtomVisitor car;
        AtomVisitor cdr;

        public AtomMultiVisitor(AtomVisitor car, AtomVisitor cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }

        //public override void visit() { car.visit(); cdr.visit(); }
        //public override void visitEnd() { car.visitEnd(); cdr.visitEnd(); }
        public override void visit(object o) { car.visit(o); cdr.visit(o); }
        //public override void visit_value(Boolean o) { car.visit_value(o); cdr.visit_value(o); }
        //public override void visit_value(Int64 o) { car.visit_value(o); cdr.visit_value(o); }
        //public override void visit_value(Double o) { car.visit_value(o); cdr.visit_value(o); }
        //public override void visit_value(String o) { car.visit_value(o); cdr.visit_value(o); }
        //public override void visit_value(Char o) { car.visit_value(o); cdr.visit_value(o); }
        //public override void visit_value(Symbol o) { car.visit_value(o); cdr.visit_value(o); }
    }

    public class ConsMultiVisitor : ConsVisitor {
        ConsVisitor car;
        ConsVisitor cdr;

        public ConsMultiVisitor(ConsVisitor car, ConsVisitor cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }

        public override void visit() { car.visit(); cdr.visit(); }
        public override void visitEnd() { car.visitEnd(); cdr.visitEnd(); }
        public override AtomVisitor visit_Atom_car() { return new AtomMultiVisitor(car.visit_Atom_car(), cdr.visit_Atom_car()); }
        public override ConsVisitor visit_Cons_car() { return new ConsMultiVisitor(car.visit_Cons_car(), cdr.visit_Cons_car()); }
        public override VectVisitor visit_Vect_car() { return new VectorMultiVisitor(car.visit_Vect_car(), cdr.visit_Vect_car()); }
        public override AtomVisitor visit_Atom_cdr() { return new AtomMultiVisitor(car.visit_Atom_cdr(), cdr.visit_Atom_cdr()); }
        public override ConsVisitor visit_Cons_cdr() { return new ConsMultiVisitor(car.visit_Cons_cdr(), cdr.visit_Cons_cdr()); }
        public override VectVisitor visit_Vect_cdr() { return new VectorMultiVisitor(car.visit_Vect_cdr(), cdr.visit_Vect_cdr()); }
        public override void visit_car() { car.visit_car(); cdr.visit_car(); }
        public override void visit_cdr() { car.visit_cdr(); cdr.visit_cdr(); }
    }
}
