using System;
using System.Collections.Generic;

namespace Pirate {

    class TypePirateBuilder : TypeVisitor {
        string m_name;

        public Type getPirType()
        {
            if ("string" == m_name) {
                return new StringType();
            } else if ("int32" == m_name) {
                return new IntType();
            } else if ("single" == m_name) {
                return new NumType();
            } else {
                return new PmcType();
            }
        }

        public override void visit_name(string name)
        {
            m_name = name;
        }
    }

    class TypeListPirateBuilder : TypeListVisitor {
        public override TypeVisitor visitItem_Type() { return new TypePirateBuilder(); }
        //public override PrototypeVisitor visitItem_Prototype() { return new PrototypePirateBuilder(); }
    }

    //class PrototypePirateBuilder : PrototypeVisitor {
    //    //public override void visit_name(string name) { }
    //    public override TypeListVisitor visit_parameter_types() { return new TypeListPirateBuilder(); }
    //    public override TypeListVisitor visit_return_types() { return new TypeListPirateBuilder(); }
    //}

    //class PrototypeListPirateBuilder : PrototypeListVisitor {
    //    public override PrototypeVisitor visitItem() { return new PrototypePirateBuilder(); }
    //}

    class FuncNamePirateBuilder : OperandVisitor {
        string m_name;

        public string getName()
        {
            return m_name;
        }

        public override void visit_name(string name)
        {
            m_name = name;
        }
    }

    class OperandPirateBuilder : OperandVisitor {
        AtomExprList m_list;

        public OperandPirateBuilder(AtomExprList list)
        {
            m_list = list;
        }

        public override void visit_name(string name)
        {
            m_list.Add(new NamedReg(name));
        }
    }

    class LvaluePirateBuilder : LvalueVisitor {
        RegList m_regs;

        public LvaluePirateBuilder(RegList regs)
        {
            m_regs = regs;
        }

        public override void visit_name(string name)
        {
            m_regs.Add(new NamedReg(name));
        }
    }

    class LvalueListPirateBuilder : LvalueListVisitor {
        RegList m_regs = new RegList();
        Lvalue m_lval;

        public Lvalue getLval()
        {
            return m_lval;
        }

        public override void visitEnd()
        {
            if (1 == m_regs.Count) {
                m_lval = m_regs[0];
            } else {
                m_lval = m_regs;
            }
        }

        public override LvalueVisitor visitItem() { return new LvaluePirateBuilder(m_regs); }
    }

    class OperandListPirateBuilder : OperandListVisitor {
        AtomExprList m_list = new AtomExprList();
        Arguments m_args;

        public Arguments getArgs()
        {
            return m_args;
        }

        public override void visitEnd()
        {
            if (1 == m_list.Count) {
                m_args = m_list[0];
            } else {
                m_args = m_list;
            }
        }

        public override OperandVisitor visitItem() { return new OperandPirateBuilder(m_list); }
    }

    //class GlobalPirateBuilder : GlobalVisitor {
    //    //public override void visit_name(string name) { }
    //    public override TypeVisitor visit_Type_type() { return new TypePirateBuilder(); }
    //    //public override PrototypeVisitor visit_Prototype_type() { return new PrototypePirateBuilder(); }
    //}

    //class GlobalListPirateBuilder : GlobalListVisitor {
    //    public override GlobalVisitor visitItem() { return new GlobalPirateBuilder(); }
    //}

    class LocalPirateBuilder : LocalVisitor {
        StmtList m_stmt_list;

        NamedReg m_reg;
        TypePirateBuilder m_type;

        public LocalPirateBuilder(StmtList stmt_list)
        {
            m_stmt_list = stmt_list;
        }

        public override void visitEnd()
        {
            LocalDecl local_decl = new LocalDecl(m_type.getPirType(), m_reg);
            m_stmt_list.Add(local_decl);
        }

        public override void visit_name(string name)
        {
            m_reg = new NamedReg(name);
        }

        public override TypeVisitor visit_Type_type()
        {
            m_type = new TypePirateBuilder();
            return m_type;
        }
    }

    class LocalListPirateBuilder : LocalListVisitor {
        StmtList m_stmt_list;

        public LocalListPirateBuilder(StmtList stmt_list)
        {
            m_stmt_list = stmt_list;
        }

        public override LocalVisitor visitItem() { return new LocalPirateBuilder(m_stmt_list); }
    }

    class ParameterPirateBuilder : ParameterVisitor {
        StmtList m_stmt_list;

        NamedReg m_reg;
        TypePirateBuilder m_type;

        public ParameterPirateBuilder(StmtList stmt_list)
        {
            m_stmt_list = stmt_list;
        }

        public override void visitEnd()
        {
            ParamDecl param_decl = new ParamDecl(m_type.getPirType(), m_reg);
            m_stmt_list.Add(param_decl);
        }

        public override void visit_name(string name)
        {
            m_reg = new NamedReg(name);
        }

        public override TypeVisitor visit_Type_type()
        {
            m_type = new TypePirateBuilder();
            return m_type;
        }
    }

    class ParameterListPirateBuilder : ParameterListVisitor {
        StmtList m_stmt_list;

        public ParameterListPirateBuilder(StmtList stmt_list)
        {
            m_stmt_list = stmt_list;
        }

        public override ParameterVisitor visitItem() { return new ParameterPirateBuilder(m_stmt_list); }
    }

    //class ConstantPirateBuilder : ConstantVisitor {
    //    //public override void visit_name(string name) { }
    //    public override TypeVisitor visit_Type_type() { return new TypePirateBuilder(); }
    //    public override PrototypeVisitor visit_Prototype_type() { return new PrototypePirateBuilder(); }
    //}

    //class ConstantListPirateBuilder : ConstantListVisitor {
    //    public override ConstantVisitor visitItem() { return new ConstantPirateBuilder(); }
    //}

    class OperatorPirateBuilder : OperatorVisitor {
        string m_name;

        public string getName()
        {
            return m_name;
        }

        public override void visit_name(string name)
        {
            m_name = name;
        }
    }

    //class OperatorListPirateBuilder : OperatorListVisitor {
    //    public override OperatorVisitor visitItem() { return new OperatorPirateBuilder(); }
    //}

    class LambdaPirateBuilder : LambdaVisitor {
        Pirate m_pir;
        string m_name;
        StmtList m_stmt_list;

        public LambdaPirateBuilder(Pirate pir)
        {
            m_pir = pir;
        }

        public override void visit()
        {
            m_stmt_list = new StmtList();
        }

        public override void visitEnd()
        {
            m_pir.Add(new Sub(m_name, m_stmt_list));
        }

        public override void visit_name(string name)
        {
            m_name = name;
        }

        public override ParameterListVisitor visit_parameters() { return new ParameterListPirateBuilder(m_stmt_list); }
        public override LocalListVisitor visit_locals() { return new LocalListPirateBuilder(m_stmt_list); }
        public override StatementListVisitor visit_statements() { return new StatementListPirateBuilder(m_stmt_list); }
    }

    class LambdaListPirateBuilder : LambdaListVisitor {
        Pirate m_pir;

        public LambdaListPirateBuilder(Pirate pir)
        {
            m_pir = pir;
        }

        public override LambdaVisitor visitItem() { return new LambdaPirateBuilder(m_pir); }
    }

    class StatementListPirateBuilder : StatementListVisitor {
        StmtList m_stmt_list;

        public StatementListPirateBuilder(StmtList stmt_list)
        {
            m_stmt_list = stmt_list;
        }

        public override OperatorStampVisitor visitItem_OperatorStamp() { return new OperatorStampPirateBuilder(m_stmt_list); }
        //public override LambdaStampVisitor visitItem_LambdaStamp() { return new LambdaStampPirateBuilder(m_stmt_list); }
        public override CallVisitor visitItem_Call() { return new CallPirateBuilder(m_stmt_list); }
        //public override IfVisitor visitItem_If() { return new IfPirateBuilder(m_stmt_list); }
        //public override MoveVisitor visitItem_Move() { return new MovePirateBuilder(m_stmt_list); }
    }

    class OperatorStampPirateBuilder : OperatorStampVisitor {
        StmtList m_stmt_list;
        LvalueListPirateBuilder m_lval;
        OperatorPirateBuilder m_op;
        OperandListPirateBuilder m_args;

        public OperatorStampPirateBuilder(StmtList stmt_list)
        {
            m_stmt_list = stmt_list;
        }

        public override void visitEnd()
        {
            Lvalue lval = m_lval != null ? m_lval.getLval() : null;
            string op = m_op != null ? m_op.getName() : null;
            Arguments args = m_args != null ? m_args.getArgs() : null;

            if ("neg" == op) {
                m_stmt_list.Add(new Assign(lval, new UnaryNeg(args as AtomExpr)));
            } else if ("+" == op) {
                m_stmt_list.Add(new Assign(lval, new BinaryAdd((args as AtomExprList)[0], (args as AtomExprList)[1])));
            } else if ("-" == op) {
                m_stmt_list.Add(new Assign(lval, new BinarySub((args as AtomExprList)[0], (args as AtomExprList)[1])));
            } else if ("*" == op) {
                m_stmt_list.Add(new Assign(lval, new BinaryMul((args as AtomExprList)[0], (args as AtomExprList)[1])));
            } else if ("/" == op) {
                m_stmt_list.Add(new Assign(lval, new BinaryDiv((args as AtomExprList)[0], (args as AtomExprList)[1])));
            } else if ("." == op) {
                m_stmt_list.Add(new Assign(lval, new BinaryCat((args as AtomExprList)[0], (args as AtomExprList)[1])));
            } else if ("+=" == op) {
                m_stmt_list.Add(new AssignDiv(lval as Reg, args as AtomExpr));
            } else if ("-=" == op) {
                m_stmt_list.Add(new AssignSub(lval as Reg, args as AtomExpr));
            } else if ("*=" == op) {
                m_stmt_list.Add(new AssignMul(lval as Reg, args as AtomExpr));
            } else if ("/=" == op) {
                m_stmt_list.Add(new AssignDiv(lval as Reg, args as AtomExpr));
            } else if (".=" == op) {
                m_stmt_list.Add(new AssignCat(lval as Reg, args as AtomExpr));
            }
        }

        //public override void visit_label(string label) { }

        public override LvalueListVisitor visit_lvalues()
        {
            m_lval = new LvalueListPirateBuilder();
            return m_lval;
        }

        public override OperatorVisitor visit_op()
        {
            m_op = new OperatorPirateBuilder();
            return m_op;
        }

        public override OperandListVisitor visit_arguments()
        {
            m_args = new OperandListPirateBuilder();
            return m_args;
        }
    }

    //class LambdaStampPirateBuilder : LambdaStampVisitor {
    //    StmtList m_stmt_list;

    //    public LambdaStampPirateBuilder(StmtList stmt_list)
    //    {
    //        m_stmt_list = stmt_list;
    //    }

    //    //public override void visit_label(string label) { }
    //    public override LvalueListVisitor visit_lvalues() { return new LvalueListPirateBuilder(); }
    //    public override LambdaVisitor visit_lambda() { return null; }
    //    public override OperandListVisitor visit_arguments() { return new OperandListPirateBuilder(); }
    //}

    class CallPirateBuilder : CallVisitor {
        StmtList m_stmt_list;
        LvalueListPirateBuilder m_lval;
        FuncNamePirateBuilder m_func;
        OperandListPirateBuilder m_args;

        public CallPirateBuilder(StmtList stmt_list)
        {
            m_stmt_list = stmt_list;
        }

        public override void visitEnd()
        {
            Lvalue lval = m_lval != null ? m_lval.getLval() : null;
            string func = m_func != null ? m_func.getName() : null;
            Arguments args = m_args != null ? m_args.getArgs() : null;

            if (m_lval != null) {
                m_stmt_list.Add(new Assign(lval, new Call(func, args)));
            } else {
                m_stmt_list.Add(new CallStmt(new Call(func, args)));
            }
        }

        public override void visit_label(string label) { }

        public override LvalueListVisitor visit_lvalues()
        {
            m_lval = new LvalueListPirateBuilder();
            return m_lval;
        }

        public override OperandVisitor visit_lambda_ref()
        {
            m_func = new FuncNamePirateBuilder();
            return m_func;
        }

        public override OperandListVisitor visit_arguments()
        {
            m_args = new OperandListPirateBuilder();
            return m_args;
        }
    }

    //class RelationPirateBuilder : RelationVisitor {
    //    public override TypeListVisitor visit_element_types() { return new TypeListPirateBuilder(); }
    //}

    //class RelationListPirateBuilder : RelationListVisitor {
    //    public override RelationVisitor visitItem() { return new RelationPirateBuilder(); }
    //}

    //class IfPirateBuilder : IfVisitor {
    //    StmtList m_stmt_list;

    //    public IfPirateBuilder(StmtList stmt_list)
    //    {
    //m_stmt_list = stmt_list;
    //    }

    //    //public override void visit_label(string label) { }
    //    public override LvalueListVisitor visit_lvalues() { return new LvalueListPirateBuilder(); }
    //    public override RelationVisitor visit_conditional() { return new RelationPirateBuilder(); }
    //    public override LambdaVisitor visit_consequent() { return null; }
    //    public override LambdaVisitor visit_alternate() { return null; }
    //}

    class MovePirateBuilder : MoveVisitor {
        StmtList m_stmt_list;

        public MovePirateBuilder(StmtList stmt_list)
        {
            m_stmt_list = stmt_list;
        }

        //public override void visit_label(string label) { }
        public override LvalueListVisitor visit_lvalues() { return new LvalueListPirateBuilder(); }
        public override OperandListVisitor visit_rvalues() { return new OperandListPirateBuilder(); }
    }

    class PirateCodeBuilder : CodeVisitor {
        Pirate m_pir = new Pirate();

        public Pirate getPirate()
        {
            return m_pir;
        }

        //public override TypeListVisitor visit_types() { return new TypeListPirateBuilder(); }
        //public override ConstantListVisitor visit_constants() { return new ConstantListPirateBuilder(); }
        //public override GlobalListVisitor visit_globals() { return new GlobalListPirateBuilder(); }
        //public override OperatorListVisitor visit_operators() { return new OperatorListPirateBuilder(); }
        //public override PrototypeListVisitor visit_prototypes() { return new PrototypeListPirateBuilder(); }
        //public override RelationListVisitor visit_relations() { return new RelationListPirateBuilder(); }
        public override LambdaListVisitor visit_lambdas() { return new LambdaListPirateBuilder(m_pir); }
    }
}
