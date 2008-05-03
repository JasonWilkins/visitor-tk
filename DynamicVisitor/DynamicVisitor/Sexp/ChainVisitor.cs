using System;

namespace Sexp {
    public class VectorMultiVisitor : VectorVisitor {
        VectorVisitor car;
        VectorVisitor cdr;

        public VectorMultiVisitor(VectorVisitor car, VectorVisitor cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }

        public override void visit() { car.visit(); cdr.visit(); }
        public override void visitEnd() { car.visitEnd(); cdr.visitEnd(); }
        public override AtomVisitor visitItem_Atom() { return new AtomMultiVisitor(car.visitItem_Atom(), cdr.visitItem_Atom()); }
        public override ConsVisitor visitItem_Cons() { return new ConsMultiVisitor(car.visitItem_Cons(), cdr.visitItem_Cons()); }
        public override VectorVisitor visitItem_Vector() { return new VectorMultiVisitor(car.visitItem_Vector(), cdr.visitItem_Vector()); }
    }

    public class AtomMultiVisitor : AtomVisitor {
        AtomVisitor car;
        AtomVisitor cdr;

        public AtomMultiVisitor(AtomVisitor car, AtomVisitor cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }

        public override void visit() { car.visit(); cdr.visit(); }
        public override void visitEnd() { car.visitEnd(); cdr.visitEnd(); }
        public override void visit_value(Boolean o) { car.visit_value(o); cdr.visit_value(o); }
        public override void visit_value(Int64 o) { car.visit_value(o); cdr.visit_value(o); }
        public override void visit_value(Double o) { car.visit_value(o); cdr.visit_value(o); }
        public override void visit_value(String o) { car.visit_value(o); cdr.visit_value(o); }
        public override void visit_value(Char o) { car.visit_value(o); cdr.visit_value(o); }
        public override void visit_value(Object o) { car.visit_value(o); cdr.visit_value(o); }
        public override SymbolVisitor visit_Symbol_value() { return new SymbolMultiVisitor(car.visit_Symbol_value(), cdr.visit_Symbol_value()); }
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
        public override VectorVisitor visit_Vector_car() { return new VectorMultiVisitor(car.visit_Vector_car(), cdr.visit_Vector_car()); }
        public override AtomVisitor visit_Atom_cdr() { return new AtomMultiVisitor(car.visit_Atom_cdr(), cdr.visit_Atom_cdr()); }
        public override ConsVisitor visit_Cons_cdr() { return new ConsMultiVisitor(car.visit_Cons_cdr(), cdr.visit_Cons_cdr()); }
        public override VectorVisitor visit_Vector_cdr() { return new VectorMultiVisitor(car.visit_Vector_cdr(), cdr.visit_Vector_cdr()); }
    }

    public class SymbolMultiVisitor : SymbolVisitor {
        SymbolVisitor car;
        SymbolVisitor cdr;

        public SymbolMultiVisitor(SymbolVisitor car, SymbolVisitor cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }

        public override void visit() { car.visit(); cdr.visit(); }
        public override void visitEnd() { car.visitEnd(); cdr.visitEnd(); }
        public override void visit_name(string name) { car.visit_name(name); cdr.visit_name(name); }
    }
}
