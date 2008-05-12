using System;
using System.Collections.Generic;
using System.Text;

namespace Sexp {
    public class Interpreter : VectorVisitor {
        public override AtomVisitor visitItem_Atom()
        {
            return base.visitItem_Atom();
        }

        public override ConsVisitor visitItem_Cons()
        {
            return base.visitItem_Cons();
        }

        public override VectorVisitor visitItem_Vector()
        {
            return base.visitItem_Vector();
        }
    }

    public class ListInterpreter : ConsVisitor {
        public override AtomVisitor visit_Atom_car()
        {
            return base.visit_Atom_car();
        }

        public override ConsVisitor visit_Cons_car()
        {
            return base.visit_Cons_car();
        }

        public override VectorVisitor visit_Vector_car()
        {
            return base.visit_Vector_car();
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            return base.visit_Atom_cdr();
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            return base.visit_Cons_cdr();
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            return base.visit_Vector_cdr();
        }
    }

    public class AtomInterpreter : AtomVisitor {
    }

    public class ConsInterpreter : ConsVisitor {
    }

    public class VectorInterpreter : VectorVisitor {
    }
}
