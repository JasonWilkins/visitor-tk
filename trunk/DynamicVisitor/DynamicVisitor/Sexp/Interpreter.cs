using System;
using System.Collections.Generic;
using System.Text;

using Util;

namespace Sexp {
    public delegate object Closure(List<object> args);

    public class Environment {
        Environment m_parent;
        Dictionary<Symbol, Closure> closures = new Dictionary<Symbol, Closure>();

        public Environment()
            : this(null)
        { }

        public Environment(Environment parent)
        {
            m_parent = parent;
        }

        public object apply(TxtLocation loc, Symbol sym, params object[] args)
        {
            return null;
        }

        public object apply(TxtLocation loc, Symbol sym, List<object> args)
        {
            Closure fn;

            if (sym != null) {
                if (closures.TryGetValue(sym, out fn)) {
                    return fn(args);
                } else if (m_parent != null) {
                    try {
                        return m_parent.apply(loc, sym, args);
                    } catch (Exception e) {
                        if (loc != null) {
                            throw new Exception(loc.path + " [" + loc.line + ", " + loc.column + "] " + e.Message);
                        } else {
                            throw e;
                        }
                    }
                } else {
                    throw new Exception("cannot apply undefined symbol: " + ((Symbol)(args[0])).name);
                }
            } else {
                throw new Exception("cannot apply nil");
            }
        }

        public void Add(Symbol sym, Closure fn)
        {
            closures.Add(sym, fn);
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
            if (args.Count != 2) throw new ArgumentException("eqv has been called with " + args.Count + " argument, but it takes exactly 2 arguments");

            if (args[0].Equals(args[1])) {
                return true;
            } else {
                return false;
            }
        }

        static object fn_eq_pred(List<object> args)
        {
            if (args.Count != 2) throw new ArgumentException("eq has been called with " + args.Count + " argument, but it takes exactly 2 arguments");

            if (args[0] == args[1]) {
                return true;
            } else {
                return false;
            }
        }

        static object fn_add(List<object> args)
        {
            Object total = 0L;

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
            return null;
        }

        static object fn_mul(List<object> args)
        {
            return null;
        }

        static object fn_div(List<object> args)
        {
            return null;
        }

        public TestEnvironment()
        {
            Add(new Symbol("eqv?"), fn_eqv_pred);
            Add(new Symbol("eq?"), fn_eq_pred);
            Add(new Symbol("="), fn_eq);
            Add(new Symbol("+"), fn_add);
            Add(new Symbol("-"), fn_sub);
            Add(new Symbol("*"), fn_mul);
            Add(new Symbol("/"), fn_div);
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
            m_args.Add(o);
        }

        public override void visit_value(string o)
        {
            m_args.Add(o);
        }
    }

    public class CombinationInterpreter : ConsVisitor {
        Environment m_env;
        TxtLocation m_loc;

        protected Symbol m_sym;
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
                if (m_new_args[0] is Symbol) {
                    m_sym = (Symbol)m_new_args[0];
                    m_new_args.RemoveAt(0);
                    m_args.Add(m_env.apply(m_loc, m_sym, m_new_args));
                } else {
                    throw new Exception("cannot apply object <" + m_new_args[0].GetType().FullName + "> " + Literal.literal(m_new_args[0]));
                }
            } else {
                m_args.Add(new Cons(null, null));
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
    }
}
