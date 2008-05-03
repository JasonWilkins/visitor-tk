using System;

namespace Flat {
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

        public Constant defineConstant(string name, Type type, object value)
        {
            Constant c = new Constant(name, type, value);
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
