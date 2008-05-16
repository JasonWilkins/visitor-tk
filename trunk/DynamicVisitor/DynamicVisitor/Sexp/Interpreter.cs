using System;
using System.Collections.Generic;
using System.Text;

using Util;

namespace Sexp {
    public class InterpreterException : Exception {
        public InterpreterException(string message)
            : this(message, null, null)
        { }

        public InterpreterException(string message, TxtLocation loc)
            : this(message, null, loc)
        { }

        public InterpreterException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public InterpreterException(string message, Exception innerException, TxtLocation loc)
            : base(message, innerException)
        {
            if (loc != null) {
                Data.Add("path", loc.path);
                Data.Add("column", loc.column);
                Data.Add("line", loc.line);
                Data.Add("context", loc.context);
            }
        }
    }

    public class Closure {
        public delegate object Fn(List<object> args);

        public Fn fn;

        public Closure(Fn fn)
        {
            this.fn = fn;
        }
    }

    public class Environment {
        Environment m_parent;
        Dictionary<Symbol, object> definitions = new Dictionary<Symbol, object>();

        public Environment()
            : this(null)
        { }

        public Environment(Environment parent)
        {
            m_parent = parent;
        }

        public object apply(TxtLocation loc, Closure fn, params object[] args)
        {
            return null;
        }

        public object lookup(TxtLocation loc, Symbol sym)
        {
            object def;

            if (definitions.TryGetValue(sym, out def)) {
                return def;
            } else if (m_parent != null) {
                return m_parent.lookup(loc, sym);
            } else {
                throw new InterpreterException(sym.name + " is undefined", loc);
            }
        }

        public object apply(TxtLocation loc, Closure fn, List<object> args)
        {
            try {
                return fn.fn(args);
            } catch (Exception e) {
                throw new InterpreterException("exception occured in method: " + fn.fn.Method.Name, e, loc);
            }
        }

        public void Add(Symbol sym, object def)
        {
            definitions.Add(sym, def);
        }

        public void Add(Symbol sym, Closure.Fn fn)
        {
            definitions.Add(sym, new Closure(fn));
        }
    }

    public class TestEnvironment : Environment {
        static object fn_eq(List<object> args)
        {
            for (int i = 0; i < args.Count-1; i++) {
                object a = args[i];
                object b = args[i+1];

                if ((a is Int64) && (b is Int64)) {
                    if ((Int64)a != (Int64)b) return false;
                } else if ((a is Double) && (b is Double)) {
                    if ((Double)a != (Double)b) return false;
                } else {
                    if (a is Double) {
                        if ((Double)a != (Double)(Int64)b) return false;
                    } else {
                        if ((Double)(Int64)a != (Double)b) return false;
                    }
                }
            }

            return true;
        }

        static object fn_eqv_pred(List<object> args)
        {
            if (args[0].Equals(args[1])) {
                return true;
            } else {
                return false;
            }
        }

        static object fn_eq_pred(List<object> args)
        {
            if (args[0] == args[1]) {
                return true;
            } else {
                return false;
            }
        }

        static object fn_add(List<object> args)
        {
            object total = 0L;

            foreach (object o in args) {
                if ((total is Int64) && (o is Int64)) {
                    total = (Int64)total + (Int64)o;
                } else if ((total is Double) && (o is Double)) {
                    total = (Double)total + (Double)o;
                } else {
                    if (total is Double) {
                        total = (Double)total + (Int64)o;
                    } else {
                        total = (Int64)total + (Double)o;
                    }
                }
            }

            return total;
        }

        static object fn_sub(List<object> args)
        {
            if (args.Count > 1) {
                object total = args[0];
                args.RemoveAt(0);

                foreach (object o in args) {
                    if ((total is Int64) && (o is Int64)) {
                        total = (Int64)total - (Int64)o;
                    } else if ((total is Double) && (o is Double)) {
                        total = (Double)total - (Double)o;
                    } else {
                        if (total is Double) {
                            total = (Double)total - (Int64)o;
                        } else {
                            total = (Int64)total - (Double)o;
                        }
                    }
                }

                return total;
            } else {
                object rv = args[0];

                if (rv is Int64) {
                    return -(Int64)rv;
                }
                else {
                    return -(Double)rv;
                }
            }
        }

        static object fn_mul(List<object> args)
        {
            object total = 1L;

            foreach (object o in args) {
                if ((total is Int64) && (o is Int64)) {
                    total = (Int64)total * (Int64)o;
                } else if ((total is Double) && (o is Double)) {
                    total = (Double)total * (Double)o;
                } else {
                    if (total is Double) {
                        total = (Double)total * (Int64)o;
                    } else {
                        total = (Int64)total * (Double)o;
                    }
                }
            }

            return total;
        }

        static object fn_div(List<object> args)
        {
            if (args.Count > 1) {
                object total = args[0];
                args.RemoveAt(0);

                foreach (object o in args) {
                    if ((total is Int64) && (o is Int64)) {
                        total = (Int64)total / (Int64)o;
                    } else if ((total is Double) && (o is Double)) {
                        total = (Double)total / (Double)o;
                    } else {
                        if (total is Double) {
                            total = (Double)total / (Int64)o;
                        } else {
                            total = (Int64)total / (Double)o;
                        }
                    }
                }

                return total;
            } else {
                object rv = args[0];

                if (rv is Int64) {
                    return 1/(Int64)rv;
                } else {
                    return 1/(Double)rv;
                }
            }
        }

        static object fn_boolean_pred(List<object> args)
        {
            return args[0] is Boolean;
        }

        static object fn_char_pred(List<object> args)
        {
            return args[0] is Char;
        }

        static object fn_null_pred(List<object> args)
        {
            return args[0] == null;
        }

        static object fn_number_pred(List<object> args)
        {
            return args[0] is Int64 || args[0] is Double;
        }

        static object fn_pair_pred(List<object> args)
        {
            return args[0] is Cons;
        }

        static object fn_procedure_pred(List<object> args)
        {
            return args[0] is Closure;
        }

        static object fn_string_pred(List<object> args)
        {
            return args[0] is String;
        }

        static object fn_symbol_pred(List<object> args)
        {
            return args[0] is Symbol;
        }

        static object fn_vector_pred(List<object> args)
        {
            return args[0] is Object[];
        }

        static object fn_integer_pred(List<object> args)
        {
            return args[0] is Int64;
        }

        static object fn_real_pred(List<object> args)
        {
            return args[0] is Int64 || args[0] is Double;
        }

        static object fn_exact_pred(List<object> args)
        {
            return args[0] is Int64;
        }

        static object fn_inexact_pred(List<object> args)
        {
            return args[0] is Double;
        }

        static object fn_zero_pred(List<object> args)
        {
            object num = args[0];
            if (num is Int64) {
                return (Int64)num == 0;
            } else {
                return (Double)num == 0;
            }
        }

        static object fn_complex_pred(List<object> args)
        {
            return args[0] is Int64 || args[0] is Double;
        }

        static object fn_rational_pred(List<object> args)
        {
            return args[0] is Int64;
        }

        public TestEnvironment()
        {
            Add(Symbol.get_symbol("eqv?"), fn_eqv_pred);
            Add(Symbol.get_symbol("eq?"), fn_eq_pred);
            Add(Symbol.get_symbol("="), fn_eq);
            Add(Symbol.get_symbol("+"), fn_add);
            Add(Symbol.get_symbol("-"), fn_sub);
            Add(Symbol.get_symbol("*"), fn_mul);
            Add(Symbol.get_symbol("/"), fn_div);
            Add(Symbol.get_symbol("boolean?"), fn_boolean_pred);
            Add(Symbol.get_symbol("char?"), fn_char_pred);
            Add(Symbol.get_symbol("complex?"), fn_complex_pred);
            Add(Symbol.get_symbol("number?"), fn_number_pred);
            Add(Symbol.get_symbol("rational?"), fn_rational_pred);
            Add(Symbol.get_symbol("null?"), fn_null_pred);
            Add(Symbol.get_symbol("pair?"), fn_pair_pred);
            Add(Symbol.get_symbol("procedure?"), fn_procedure_pred);
            Add(Symbol.get_symbol("string?"), fn_string_pred);
            Add(Symbol.get_symbol("symbol?"), fn_symbol_pred);
            Add(Symbol.get_symbol("vector?"), fn_vector_pred);
            Add(Symbol.get_symbol("real?"), fn_real_pred);
            Add(Symbol.get_symbol("integer?"), fn_integer_pred);
            Add(Symbol.get_symbol("inexact?"), fn_inexact_pred);
            Add(Symbol.get_symbol("exact?"), fn_exact_pred);
            Add(Symbol.get_symbol("zero?"), fn_zero_pred);
        }
    }

    public class Print {
        public void print(List<object> objs)
        {
            if (objs.Count == 1) {
                object o = objs[0];

                if (o != null) {
                    Console.WriteLine(" => <{0}> {1}", o.GetType().FullName, Literal.literal(o));
                } else {
                    Console.WriteLine(" => nil");
                }
            } else {
                throw new Exception();
            }
        }
    }

    public class Interpreter : VectorVisitor {
        Environment m_env;
        TxtLocation m_loc;
        Print m_print = new Print();

        public Interpreter(Environment env, TxtLocation loc)
        {
            m_env = env;
            m_loc = loc;
        }

        public override AtomVisitor visitItem_Atom()
        {
            return new TopLevelAtomInterpreter(m_env, m_loc, m_print, new List<object>());
        }

        public override ConsVisitor visitItem_Cons()
        {
            return new TopLevelConsInterpreter(m_env, m_loc, m_print, new List<object>());
        }

        public override VectorVisitor visitItem_Vector()
        {
            return new TopLevelVectInterpreter(m_env, m_loc, m_print, new List<object>());
        }

        public override void visitItem(object o)
        {
            List<object> m_args = new List<object>();
            m_args.Add(null);
            m_print.print(m_args);
        }
    }

    // TODO: These three classes are identical, how do I unify them?

    class TopLevelAtomInterpreter : AtomInterpreter {
        Print m_print;

        public TopLevelAtomInterpreter(Environment env, TxtLocation loc, Print print, List<object> args)
            : base(env, loc, args)
        {
            m_print = print;
        }

        public override void visitEnd()
        {
            base.visitEnd();
            m_print.print(m_args);
        }
    }

    class TopLevelConsInterpreter : CombinationInterpreter {
        Print m_print;

        public TopLevelConsInterpreter(Environment env, TxtLocation loc, Print print, List<object> args)
            : base(env, loc, args)
        {
            m_print = print;
        }

        public override void visitEnd()
        {
            base.visitEnd();
            m_print.print(m_args);
        }
    }

    class TopLevelVectInterpreter : VectInterpreter {
        Print m_print;

        public TopLevelVectInterpreter(Environment env, TxtLocation loc, Print print, List<object> args)
            : base(env, loc, args)
        {
            m_print = print;
        }

        public override void visitEnd()
        {
            base.visitEnd();
            m_print.print(m_args);
        }
    }

    class AtomInterpreter : AtomVisitor {
        Environment m_env;
        TxtLocation m_loc;

        protected List<object> m_args;

        public AtomInterpreter(Environment env, TxtLocation loc, List<object> args)
        {
            m_env = env;
            m_loc = loc;
            m_args = args;
        }

        public override void visit_value(bool o)
        {
            m_args.Add(o);
        }

        public override void visit_value(char o)
        {
            m_args.Add(o);
        }

        public override void visit_value(double o)
        {
            m_args.Add(o);
        }

        public override void visit_value(long o)
        {
            m_args.Add(o);
        }

        public override void visit_value(Symbol o)
        {
            m_args.Add(m_env.lookup(m_loc, o));
        }

        public override void visit_value(string o)
        {
            m_args.Add(o);
        }
    }

    public class CombinationInterpreter : ConsVisitor {
        Environment m_env;
        TxtLocation m_loc;

        protected List<object> m_args;

        List<object> m_new_args = new List<object>();
        TxtLocation m_new_loc;

        public CombinationInterpreter(Environment env, TxtLocation loc, List<object> args)
        {
            m_env = env;
            m_loc = loc.clone();
            m_new_loc = loc;
            m_args = args;
        }

        public override void visitEnd()
        {
            if (m_new_args.Count > 0) {
                if (m_new_args[0] is Closure) {
                    Closure fn = (Closure)m_new_args[0];
                    m_new_args.RemoveAt(0);
                    m_args.Add(m_env.apply(m_loc, fn, m_new_args));
                } else {
                    throw new Exception("cannot apply object <" + m_new_args[0].GetType().FullName + "> " + Literal.literal(m_new_args[0]));
                }
            } else {
                m_args.Add(null);
            }
        }

        public override AtomVisitor visit_Atom_car()
        {
            return new AtomInterpreter(m_env, m_new_loc, m_new_args);
        }

        public override ConsVisitor visit_Cons_car()
        {
            return new CombinationInterpreter(m_env, m_new_loc, m_new_args);
        }

        public override VectorVisitor visit_Vector_car()
        {
            return new VectInterpreter(m_env, m_new_loc, m_new_args);
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            throw new Exception("combination must be a proper list");
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            return new ConsInterpreter(m_env, m_new_loc, m_new_args);
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            throw new Exception("combination must be a proper list");
        }

        public override void visit_car(object o)
        {
            m_new_args.Add(o);
        }

        public override void visit_cdr(object o)
        {
            throw new Exception("combination must be a proper list");
        }
    }

    class ConsInterpreter : ConsVisitor {
        Environment m_env;
        TxtLocation m_loc;

        protected List<object> m_args;

        public ConsInterpreter(Environment env, TxtLocation loc, List<object> args)
        {
            m_env = env;
            m_loc = loc;
            m_args = args;
        }

        public override AtomVisitor visit_Atom_car()
        {
            return new AtomInterpreter(m_env, m_loc, m_args);
        }

        public override ConsVisitor visit_Cons_car()
        {
            return new CombinationInterpreter(m_env, m_loc, m_args);
        }

        public override VectorVisitor visit_Vector_car()
        {
            return new VectInterpreter(m_env, m_loc, m_args);
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            throw new Exception("combination must be a proper list");
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            return new ConsInterpreter(m_env, m_loc, m_args);
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            throw new Exception("combination must be a proper list");
        }

        public override void visit_car(object o)
        {
            m_args.Add(o);
        }

        public override void visit_cdr(object o)
        {
            throw new Exception("combination must be a proper list");
        }
    }

    class VectInterpreter : VectorVisitor {
        Environment m_env;
        TxtLocation m_loc;

        protected List<object> m_args;

        List<object> m_vect = new List<object>();

        public VectInterpreter(Environment env, TxtLocation loc, List<object> args)
        {
            m_env = env;
            m_loc = loc;
            m_args = args;
        }

        public override void visitEnd()
        {
            object[] rv = new object[m_vect.Count];
            m_vect.CopyTo(rv);
            m_args.Add(rv);
        }

        public override AtomVisitor visitItem_Atom()
        {
            return new AtomInterpreter(m_env, m_loc, m_vect);
        }

        public override ConsVisitor visitItem_Cons()
        {
            return new CombinationInterpreter(m_env, m_loc, m_vect);
        }

        public override VectorVisitor visitItem_Vector()
        {
            return new VectInterpreter(m_env, m_loc, m_vect);
        }

        public override void visitItem(object o)
        {
            m_args.Add(o);
        }
    }
}
