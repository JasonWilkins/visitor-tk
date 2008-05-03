using System;
using System.Collections.Generic;
using System.Text;

namespace Pirate {
    class Collection<T> : IEnumerable<T> {
        List<T> m_list = new List<T>();

        public void Add(T item)
        {
            m_list.Add(item);
        }

        public int Count
        {
            get { return m_list.Count; }
        }

        public T this [int index]
        {
            get { return m_list[index]; }
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #endregion
    }

    sealed class Pirate : Collection<TopLevel> { }

    interface TopLevel { }

    sealed class Sub : TopLevel {
        public string name;
        public StmtList stmt_list;

        public Sub(string name, StmtList stmt_list)
        {
            this.name = name;
            this.stmt_list = stmt_list;
        }
    }

    sealed class StmtList : Collection<Stmt> { }

    abstract class Reg : Lvalue, AtomExpr { }

    abstract class NumberedReg : Reg {
        public int number;

        public NumberedReg(int number)
        {
            this.number = number;
        }
    }

    abstract class PvmReg : NumberedReg { public PvmReg(int number) : base(number) { } }

    abstract class TmpReg : NumberedReg { public TmpReg(int number) : base(number) { } }

    sealed class PvmIntReg : PvmReg { public PvmIntReg(int number) : base(number) { } }
    sealed class PvmNumReg : PvmReg { public PvmNumReg(int number) : base(number) { } }
    sealed class PvmStringReg : PvmReg { public PvmStringReg(int number) : base(number) { } }
    sealed class PvmPmcReg : PvmReg { public PvmPmcReg(int number) : base(number) { } }

    sealed class TmpIntReg : TmpReg { public TmpIntReg(int number) : base(number) { } }
    sealed class TmpNumReg : TmpReg { public TmpNumReg(int number) : base(number) { } }
    sealed class TmpStringReg : TmpReg { public TmpStringReg(int number) : base(number) { } }
    sealed class TmpPmcReg : TmpReg { public TmpPmcReg(int number) : base(number) { } }

    sealed class NamedReg : Reg, Ids {
        public string name;

        public NamedReg(string name)
        {
            this.name = name;
        }
    }

    abstract class RegDecl : Stmt {
        public Type type;
        public Ids ids;

        public RegDecl(Type type, Ids ids)
        {
            this.type = type;
            this.ids = ids;
        }
    }

    interface Ids { }

    class Type { }
    sealed class IntType : Type { }
    sealed class NumType : Type { }
    sealed class StringType : Type { }
    sealed class PmcType : Type { }

    sealed class IdList : Collection<NamedReg>, Ids { }

    sealed class LocalDecl : RegDecl { public LocalDecl(Type type, Ids ids) : base(type, ids) { } }
    sealed class ParamDecl : RegDecl { public ParamDecl(Type type, Ids ids) : base(type, ids) { } }

    sealed class RegList : Collection<Reg>, Lvalue { }

    interface Lvalue { }

    interface ReturnValues { }

    sealed class ReturnStmt : Stmt {
        public ReturnValues rv;

        public ReturnStmt(ReturnValues rv)
        {
            this.rv = rv;
        }
    }

    interface Literal : AtomExpr { }

    interface AtomExpr : ReturnValues, Arguments, Expr { }

    sealed class AtomExprList : Collection<AtomExpr>, ReturnValues, Arguments { }

    sealed class IntLiteral : Literal {
        public int value;

        public IntLiteral(int value)
        {
            this.value = value;
        }
    }

    sealed class NumLiteral : Literal {
        public double value;

        public NumLiteral(double value)
        {
            this.value = value;
        }
    }

    sealed class StringLiteral : Literal {
        public string value;

        public StringLiteral(string value)
        {
            this.value = value;
        }
    }

    interface Stmt { }

    sealed class Call : Expr {
        public string func;
        public Arguments args;

        public Call(string func, Arguments args)
        {
            this.func = func;
            this.args = args;
        }
    }

    sealed class CallStmt : Stmt {
        public Call call;

        public CallStmt(Call call)
        {
            this.call = call;
        }
    }

    interface Arguments { }

    sealed class Label {
        public string name;

        public Label(string name)
        {
            this.name = name;
        }
    }

    sealed class Goto {
        public string target;

        public Goto(string target)
        {
            this.target = target;
        }
    }

    abstract class If {
        public string target;

        public If(string target)
        {
            this.target = target;
        }
    }

    abstract class RegIf : If {
        public Reg reg;

        public RegIf(Reg reg, string target)
            : base(target)
        {
            this.reg = reg;
        }
    }

    sealed class ExprIf : If {
        public IfExpr expr;

        public ExprIf(IfExpr expr, string target)
            : base(target)
        {
            this.expr = expr;
        }
    }

    sealed class AtomIf : RegIf { public AtomIf(Reg reg, string target) : base(reg, target) { } }

    sealed class NullIf : RegIf { public NullIf(Reg reg, string target) : base(reg, target) { } }

    interface IfExpr : Expr { }

    interface Expr { }

    sealed class Assign : Stmt {
        public Lvalue lval;
        public Expr rval;

        public Assign(Lvalue lval, Expr rval)
        {
            this.lval = lval;
            this.rval = rval;
        }
    }

    abstract class AssignOp : Stmt {
        public Reg lval;
        public AtomExpr rval;

        public AssignOp(Reg lval, AtomExpr rval)
        {
            this.lval = lval;
            this.rval = rval;
        }
    }

    sealed class AssignAdd : AssignOp { public AssignAdd(Reg lhs, AtomExpr rhs) : base(lhs, rhs) { } }
    sealed class AssignSub : AssignOp { public AssignSub(Reg lhs, AtomExpr rhs) : base(lhs, rhs) { } }
    sealed class AssignMul : AssignOp { public AssignMul(Reg lhs, AtomExpr rhs) : base(lhs, rhs) { } }
    sealed class AssignDiv : AssignOp { public AssignDiv(Reg lhs, AtomExpr rhs) : base(lhs, rhs) { } }
    sealed class AssignCat : AssignOp { public AssignCat(Reg lhs, AtomExpr rhs) : base(lhs, rhs) { } }

    abstract class UnaryExpr : Expr {
        public AtomExpr arg;

        public UnaryExpr(AtomExpr arg)
        {
            this.arg = arg;
        }
    }

    sealed class UnaryNeg : UnaryExpr {
        public UnaryNeg(AtomExpr arg) : base(arg) { }
    }

    abstract class BinaryExpr : Expr {
        public AtomExpr lhs;
        public AtomExpr rhs;

        public BinaryExpr(AtomExpr lhs, AtomExpr rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }
    }

    sealed class BinaryAdd : BinaryExpr {
        public BinaryAdd(AtomExpr lhs, AtomExpr rhs) : base(lhs, rhs) { }
    }

    sealed class BinarySub : BinaryExpr { public BinarySub(AtomExpr lhs, AtomExpr rhs) : base(lhs, rhs) { } }
    sealed class BinaryMul : BinaryExpr { public BinaryMul(AtomExpr lhs, AtomExpr rhs) : base(lhs, rhs) { } }
    sealed class BinaryDiv : BinaryExpr { public BinaryDiv(AtomExpr lhs, AtomExpr rhs) : base(lhs, rhs) { } }
    sealed class BinaryCat : BinaryExpr { public BinaryCat(AtomExpr lhs, AtomExpr rhs) : base(lhs, rhs) { } }

    sealed class Include : Stmt, TopLevel {
        public string filename;

        public Include(string filename)
        {
            this.filename = filename;
        }
    }

    sealed class Comment : Stmt, TopLevel {
        public string text;

        public Comment(string text)
        {
            this.text = text;
        }
    }
}
