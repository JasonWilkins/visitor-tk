#define WARNING

using System;

namespace Flat2Pirate {
    using Trace;

    abstract class TypeVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual void visit_name(string name) { Trace.Warning("default {0}.visit_name({1})", GetType().Name, name); }
    }

    abstract class TypeListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual TypeVisitor visitItem_Type() { return null; }
        public virtual PrototypeVisitor visitItem_Prototype() { return null; }
    }

    abstract class PrototypeVisitor : TypeVisitor {
        public virtual TypeListVisitor visit_parameter_types() { return null; }
        public virtual TypeListVisitor visit_return_types() { return null; }
    }

    abstract class PrototypeListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual PrototypeVisitor visitItem() { return null; }
    }

    abstract class OperandVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual void visit_name(string name) { Trace.Warning("default {0}.visit_name({1})", GetType().Name, name); }
        public virtual TypeVisitor visit_Type_type() { return null; }
        public virtual PrototypeVisitor visit_Prototype_type() { return null; }
    }

    abstract class LvalueVisitor : OperandVisitor { }

    abstract class LvalueListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual LvalueVisitor visitItem() { return null; }
    }

    abstract class OperandListVisitor : LvalueListVisitor {
        public new virtual OperandVisitor visitItem() { return null; }
    }

    abstract class GlobalVisitor : OperandVisitor { }

    abstract class GlobalListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual GlobalVisitor visitItem() { return null; }
    }

    abstract class LocalVisitor : OperandVisitor { }

    abstract class LocalListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual LocalVisitor visitItem() { return null; }
    }

    abstract class ParameterVisitor : OperandVisitor { }

    abstract class ParameterListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual ParameterVisitor visitItem() { return null; }
    }

    abstract class ConstantVisitor : OperandVisitor { }

    abstract class ConstantListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual ConstantVisitor visitItem() { return null; }
    }

    abstract class OperatorVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual void visit_name(string name) { Trace.Warning("default {0}.visit_name({1})", GetType().Name, name); }
        public virtual TypeListVisitor visit_result_types() { return null; }
        public virtual TypeListVisitor visit_operand_types() { return null; }
    }

    abstract class OperatorListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual OperatorVisitor visitItem() { return null; }
    }

    abstract class LambdaVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual LambdaVisitor visit_parent() { return null; }
        public virtual void visit_name(string name) { Trace.Warning("default {0}.visit_name({1})", GetType().Name, name); }
        public virtual TypeListVisitor visit_return_types() { return null; }
        public virtual ParameterListVisitor visit_parameters() { return null; }
        public virtual LocalListVisitor visit_locals() { return null; }
        public virtual LambdaListVisitor visit_lambdas() { return null; }
        public virtual StatementListVisitor visit_statements() { return null; }
    }

    abstract class LambdaListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual LambdaVisitor visitItem() { return null; }
    }

    abstract class StatementVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual void visit_label(string label) { Trace.Warning("default {0}.visit_label({1})", GetType().Name, label); }
        public virtual LvalueListVisitor visit_lvalues() { return null; }
    }

    abstract class StatementListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual OperatorStampVisitor visitItem_OperatorStamp() { return null; }
        public virtual LambdaStampVisitor visitItem_LambdaStamp() { return null; }
        public virtual CallVisitor visitItem_Call() { return null; }
        public virtual IfVisitor visitItem_If() { return null; }
        public virtual MoveVisitor visitItem_Move() { return null; }
    }

    abstract class OperatorStampVisitor : StatementVisitor {
        public virtual OperatorVisitor visit_op() { return null; }
        public virtual OperandListVisitor visit_arguments() { return null; }
    }

    abstract class LambdaStampVisitor : StatementVisitor {
        public virtual LambdaVisitor visit_lambda() { return null; }
        public virtual OperandListVisitor visit_arguments() { return null; }
    }

    abstract class CallVisitor : StatementVisitor {
        public virtual PrototypeVisitor visit_prototype() { return null; }
        public virtual OperandVisitor visit_lambda_ref() { return null; }
        public virtual OperandListVisitor visit_arguments() { return null; }
    }

    abstract class RelationVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual void visit_name(string name) { Trace.Warning("default {0}.visit_name({1})", GetType().Name, name); }
        public virtual TypeListVisitor visit_element_types() { return null; }
    }

    abstract class RelationListVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual RelationVisitor visitItem() { return null; }
    }

    abstract class IfVisitor : StatementVisitor {
        public virtual RelationVisitor visit_conditional() { return null; }
        public virtual LambdaVisitor visit_consequent() { return null; }
        public virtual LambdaVisitor visit_alternate() { return null; }
    }

    abstract class MoveVisitor : StatementVisitor {
        public virtual OperandListVisitor visit_rvalues() { return null; }
    }

    abstract class CodeVisitor {
        public virtual void visit() { }
        public virtual void visitEnd() { }

        public virtual TypeListVisitor visit_types() { return null; }
        public virtual ConstantListVisitor visit_constants() { return null; }
        public virtual GlobalListVisitor visit_globals() { return null; }
        public virtual OperatorListVisitor visit_operators() { return null; }
        public virtual PrototypeListVisitor visit_prototypes() { return null; }
        public virtual RelationListVisitor visit_relations() { return null; }
        public virtual LambdaListVisitor visit_lambdas() { return null; }
    }
}
