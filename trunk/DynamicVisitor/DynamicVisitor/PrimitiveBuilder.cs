using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBuilder {
    public abstract class Collection<T> : IEnumerable<T> {
        List<T> list = new List<T>();

        public Collection(params T[] items)
        {
            AddEach(items);
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public void AddEach(T[] items)
        {
            foreach (T item in items) {
                list.Add(item);
            }
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return list.GetEnumerator() as IEnumerator<T>;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion
    }

    public class Type {
        public string name;

        internal Type(string name)
        {
            this.name = name;
        }
    }

    public class TypeList : Collection<Type> { public TypeList(params Type[] list) : base(list) { } }

    public class Prototype : Type {
        public TypeList parameter_types;
        public TypeList return_types;

        internal Prototype(string name, TypeList parameter_types, TypeList return_types)
            : base(name)
        {
            this.parameter_types = parameter_types;
            this.return_types = return_types;
        }
    }

    public class PrototypeList : Collection<Prototype> { }

    public abstract class Operand {
        public string name;
        public Type type;

        internal Operand(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class OperandList : Collection<Operand> { public OperandList(params Operand[] list) : base(list) { } }

    public abstract class Lvalue : Operand {
        internal Lvalue(string name, Type type) : base(name, type) { }
    }

    public class LvalueList : Collection<Lvalue> { public LvalueList(params Lvalue[] list) : base(list) { } }

    public class Global : Lvalue {
        internal Global(string name, Type type) : base(name, type) { }
    }

    public class GlobalList : Collection<Global> { }

    public class Local : Lvalue {
        internal Local(string name, Type type) : base(name, type) { }
    }

    public class LocalList : Collection<Local> { }

    public class Parameter : Lvalue {
        internal Parameter(string name, Type type) : base(name, type) { }
    }

    public class ParameterList : Collection<Parameter> { }

    public class Constant : Operand {
        internal Constant(string literal, Type type) : base(literal, type) { }
    }

    public class ConstantList : Collection<Constant> { }

    public class Operator {
        public string name;
        public TypeList result_types;
        public TypeList operand_types;

        internal Operator(string name, TypeList result_types, TypeList operand_types)
        {
            this.name = name;
            this.result_types = result_types;
            this.operand_types = operand_types;
        }
    }

    public class OperatorList : Collection<Operator> { }

    public class Lambda {
        public string name;
        public TypeList return_types;
        public Lambda parent;

        public ParameterList parameters = new ParameterList();
        public LocalList locals = new LocalList();
        public LambdaList lambdas = new LambdaList();
        public StatementList statements = new StatementList();

        internal Lambda(string name, TypeList return_types, Lambda parent)
        {
            this.name = name;
            this.return_types = return_types;
            this.parent = parent;
        }
    }

    public class LambdaList : Collection<Lambda> { }

    public abstract class Statement {
        public string label;
        public LvalueList lvalues;

        internal Statement(string label, LvalueList lvalues)
        {
            this.label = label;
            this.lvalues = lvalues;
        }
    }

    public class StatementList : Collection<Statement> { }

    public class OperatorStamp : Statement {
        public Operator op;
        public OperandList arguments;

        internal OperatorStamp(string label, LvalueList lvalues, Operator op, OperandList arguments)
            : base(label, lvalues)
        {
            this.op = op;
            this.arguments = arguments;
        }
    }

    public class LambdaStamp : Statement {
        Lambda lambda;
        OperandList arguments;

        internal LambdaStamp(string label, LvalueList lvalues, Lambda lambda, OperandList arguments)
            : base(label, lvalues)
        {
            this.lambda = lambda;
            this.arguments = arguments;
        }
    }

    public class Call : Statement {
        public Prototype prototype;
        public Operand lambda_ref;
        public OperandList arguments;

        internal Call(string label, LvalueList lvalues, Prototype prototype, Operand lambda_reference, OperandList arguments)
            : base(label, lvalues)
        {
            this.prototype = prototype;
            this.lambda_ref = lambda_reference;
            this.arguments = arguments;
        }
    }

    public class Relation {
        public string name;
        public TypeList element_types;

        internal Relation(string name, TypeList element_types)
        {
            this.name = name;
            this.element_types = element_types;
        }
    }

    public class RelationList : Collection<Relation> { }

    public class If : Statement {
        public Relation conditional;
        public Lambda consequent;
        public Lambda alternate;

        internal If(string label, LvalueList lvalues, Relation conditional, Lambda consequent, Lambda alternate)
            : base(label, lvalues)
        {
            this.conditional = conditional;
            this.consequent = consequent;
            this.alternate = alternate;
        }
    }

    public class Move : Statement {
        public OperandList rvalues;

        internal Move(string label, LvalueList lvalues, OperandList rvalues)
            : base(label, lvalues)
        {
            this.rvalues = rvalues;
        }
    }

    public class Code {
        public TypeList types = new TypeList();
        public ConstantList constants = new ConstantList();
        public GlobalList globals = new GlobalList();
        public OperatorList operators = new OperatorList();
        public PrototypeList prototypes = new PrototypeList();
        public RelationList relations = new RelationList();
        public LambdaList lambdas = new LambdaList();
    }

/*    public static class Pack {
        public static Type[] pack(params Type[] types)
        {
            return types;
        }

        public static Operand[] pack(params Operand[] parameters)
        {
            return parameters;
        }
    }
    */
    public class CodeBuilder {
        Code m_code = new Code();

        public Code getCode()
        {
            return m_code;
        }

        public Type defineType(string name)
        {
            Type t = new Type(name);
            m_code.types.Add(t);
            return t;
        }

        public Prototype definePrototype(string name, TypeList parameter_types, TypeList return_types)
        {
            Prototype proto = new Prototype(name, parameter_types, return_types);
            m_code.prototypes.Add(proto);
            return proto;
        }

        public Operator defineOperator(string name, TypeList result_type, TypeList operand_types)
        {
            Operator op = new Operator(name, result_type, operand_types);
            m_code.operators.Add(op);
            return op;
        }

        public Relation defineRelation(string name, TypeList element_types)
        {
            Relation r = new Relation(name, element_types);
            m_code.relations.Add(r);
            return r;
        }

        public Global defineGlobal(string name, Type type)
        {
            Global g = new Global(name, type);
            m_code.globals.Add(g);
            return g;
        }

        public Constant defineConstant(string literal, Type type)
        {
            Constant c = new Constant(literal, type);
            m_code.constants.Add(c);
            return c;
        }

        public LambdaBuilder getLambdaBuilder(string name, TypeList return_types)
        {
            return new LambdaBuilder(name, return_types, m_code, null);
        }

        public void defineLambda(Lambda lambda)
        {
            m_code.lambdas.Add(lambda);
        }
    }

    public class LambdaBuilder {
        Lambda m_lambda;
        Code m_code;
        int m_label = 0;

        string next_label()
        {
            m_label++;
            return ".S"+m_label.ToString();
        }

        internal LambdaBuilder(string name, TypeList return_types, Code code, Lambda parent)
        {
            m_lambda = new Lambda(name, return_types, parent);
            m_code = code;
        }

        public Lambda getLambda(OperandList return_values)
        {
            return m_lambda;
        }

        public LambdaBuilder getLambdaBuilder(string name, TypeList return_types)
        {
            return new LambdaBuilder(name, return_types, m_code, m_lambda);
        }

        public void defineLambda(Lambda lambda)
        {
            m_lambda.lambdas.Add(lambda);
        }

        public Local defineLocal(string name, Type type)
        {
            Local l = new Local(name, type);
            m_lambda.locals.Add(l);
            return l;
        }

        public Parameter defineParameter(string name, Type type)
        {
            Parameter p = new Parameter(name, type);
            m_lambda.parameters.Add(p);
            return p;
        }

        public void addOperator(string bookmark, Operator op, LvalueList lvalues, OperandList arguments)
        {
            if (null == bookmark) bookmark = next_label();
            OperatorStamp op_stamp = new OperatorStamp(bookmark, lvalues, op, arguments);
            m_lambda.statements.Add(op_stamp);
        }

        public void addLambda(string bookmark, Lambda lambda, LvalueList lvalues, OperandList arguments)
        {
            if (null == bookmark) bookmark = next_label();
            LambdaStamp lambda_stamp = new LambdaStamp(bookmark, lvalues, lambda, arguments);
            m_lambda.statements.Add(lambda_stamp);
        }

        public void addCall(string bookmark, Prototype prototype, Operand lambda_reference, LvalueList lvalues, OperandList arguments)
        {
            if (null == bookmark) bookmark = next_label();
            Call call = new Call(bookmark, lvalues, prototype, lambda_reference, arguments);
            m_lambda.statements.Add(call);
        }

        public void addIf(string bookmark, LvalueList lvalues, Relation conditional, Lambda consequent, Lambda alternate)
        {
            if (null == bookmark) bookmark = next_label();
            If _if = new If(bookmark, lvalues, conditional, consequent, alternate);
            m_lambda.statements.Add(_if);
        }

        public void addMove(string bookmark, LvalueList lvalues, OperandList rvalues)
        {
            if (null == bookmark) bookmark = next_label();
            Move m = new Move(bookmark, lvalues, rvalues);
            m_lambda.statements.Add(m);
        }
    }
}
