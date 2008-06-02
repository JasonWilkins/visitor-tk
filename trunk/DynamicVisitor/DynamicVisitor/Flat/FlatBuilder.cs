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

        public Prototype definePrototype(string name, Types parameter_types, Types return_types)
        {
            if (null == name) name = ".P"+m_code.prototypes.Count;
            Prototype proto = new Prototype(name, parameter_types, return_types);
            m_code.prototypes.Add(proto);
            return proto;
        }

        public Operator defineOperator(string name, Types result_type, Types operand_types)
        {
            if (null == name) name = ".O"+m_code.operators.Count;
            Operator op = new Operator(name, result_type, operand_types);
            m_code.operators.Add(op);
            return op;
        }

        public Relation defineRelation(string name, Types element_types)
        {
            if (null == name) name = ".R"+m_code.relations.Count;
            Relation r = new Relation(name, element_types);
            m_code.relations.Add(r);
            return r;
        }

        public Global defineGlobal(string name, Type type)
        {
            if (null == name) name = ".G"+m_code.globals.Count;
            Global g = new Global(name, type);
            m_code.globals.Add(g);
            return g;
        }

        public Literal defineLiteral(Type type, object value)
        {
            Literal l = new Literal(type, value);
            m_code.literals.Add(l);
            return l;
        }

        public Constant defineConstant(string name, Type type, object value)
        {
            if (null == name) name = ".C"+m_code.constants.Count;
            Constant c = new Constant(name, type, value);
            m_code.constants.Add(c);
            return c;
        }

        public LambdaBuilder getLambdaBuilder(string name, Types return_types)
        {
            if (null == name) name = ".L"+m_code.lambdas.Count;
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

        internal LambdaBuilder(string name, Types return_types, Code code, Lambda parent)
        {
            m_lambda = new Lambda(name, return_types, parent);
            m_code = code;
        }

        public Lambda getLambda(Operands return_values)
        {
            return m_lambda;
        }

        public LambdaBuilder getLambdaBuilder(string name, Types return_types)
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

        public void addDo(Operator op, Lvalues lvalues, Operands arguments)
        {
            Do op_stamp = new Do(lvalues, op, arguments);
            m_lambda.statements.Add(op_stamp);
        }

        public void addLambda(Lambda lambda, Lvalues lvalues, Operands arguments)
        {
            DoLambda lambda_stamp = new DoLambda(lvalues, lambda, arguments);
            m_lambda.statements.Add(lambda_stamp);
        }

        public void addCall(Operand lambda_reference, Lvalues lvalues, Operands arguments)
        {
            Call call = new Call(lvalues, lambda_reference, arguments);
            m_lambda.statements.Add(call);
        }

        public void addIf(Lvalues lvalues, Relation conditional, Lambda consequent, Lambda alternate)
        {
            If _if = new If(lvalues, conditional, consequent, alternate);
            m_lambda.statements.Add(_if);
        }

        public void addMove(Lvalues lvalues, Operands rvalues)
        {
            Move m = new Move(lvalues, rvalues);
            m_lambda.statements.Add(m);
        }
    }
}
