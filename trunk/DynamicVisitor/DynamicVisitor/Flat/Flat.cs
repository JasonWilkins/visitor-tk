using System;
using System.Collections.Generic;

namespace Flat {
    public abstract class ListWrapper<T> : IEnumerable<T> {
        List<T> list = new List<T>();

        protected ListWrapper(params T[] items)
        {
            AddEach(items);
        }

        internal void Add(T item)
        {
            list.Add(item);
        }

        private void AddEach(T[] items)
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

    public class TypeList : ListWrapper<Type> { public TypeList(params Type[] list) : base(list) { } }

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

    public class PrototypeList : ListWrapper<Prototype> { internal PrototypeList() { } }

    public abstract class Operand {
        public string name;
        public Type type;

        internal Operand(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class OperandList : ListWrapper<Operand> { public OperandList(params Operand[] list) : base(list) { } }

    public abstract class Lvalue : Operand {
        internal Lvalue(string name, Type type) : base(name, type) { }
    }

    public class LvalueList : ListWrapper<Lvalue> { public LvalueList(params Lvalue[] list) : base(list) { } }

    public class Global : Lvalue {
        internal Global(string name, Type type) : base(name, type) { }
    }

    public class GlobalList : ListWrapper<Global> { }

    public class Local : Lvalue {
        internal Local(string name, Type type) : base(name, type) { }
    }

    public class LocalList : ListWrapper<Local> { }

    public class Parameter : Lvalue {
        internal Parameter(string name, Type type) : base(name, type) { }
    }

    public class ParameterList : ListWrapper<Parameter> { }

    public class Constant : Operand {
        public object value;

        internal Constant(string name, Type type, object value) : base(name, type)
        {
            this.value = value;
        }
    }

    public class ConstantList : ListWrapper<Constant> { }

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

    public class OperatorList : ListWrapper<Operator> { }

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

    public class LambdaList : ListWrapper<Lambda> { }

    public abstract class Statement {
        public string label;
        public LvalueList lvalues;

        internal Statement(string label, LvalueList lvalues)
        {
            this.label = label;
            this.lvalues = lvalues;
        }
    }

    public class StatementList : ListWrapper<Statement> { }

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

    public class RelationList : ListWrapper<Relation> { }

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
}