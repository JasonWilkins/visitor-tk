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

    public class Print {
        public void print(List<object> objs)
        {
            if (objs.Count == 1) {
                object o = objs[0];

                if (o != null) {
                    Console.WriteLine(" => <{0}> {1}", o.GetType().FullName, Literal.format(o));
                } else {
                    Console.WriteLine(" => nil");
                }
            } else {
                throw new Exception();
            }
        }
    }

    public class Interpreter : VectVisitor {
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

        public override VectVisitor visitItem_Vect()
        {
            return new TopLevelVectInterpreter(m_env, m_loc, m_print, new List<object>());
        }

        public override void visitItem()
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

        public override void visit(object o)
        {
            base.visit(o);
            m_print.print(m_args);
        }

        //public override void visitEnd()
        //{
        //    base.visitEnd();
        //    m_print.print(m_args);
        //}
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

        public override void visit(object o)
        {
            if (o is Symbol) {
                m_args.Add(m_env.lookup(m_loc, (Symbol)o));
            } else {
                m_args.Add(o);
            }
        }

        //public override void visit_value(bool o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(char o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(double o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(long o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(Symbol o)
        //{
        //    m_args.Add(m_env.lookup(m_loc, o));
        //}

        //public override void visit_value(string o)
        //{
        //    m_args.Add(o);
        //}
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
                    throw new Exception("cannot apply object <" + m_new_args[0].GetType().FullName + "> " + Literal.format(m_new_args[0]));
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

        public override VectVisitor visit_Vect_car()
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

        public override VectVisitor visit_Vect_cdr()
        {
            throw new Exception("combination must be a proper list");
        }

        public override void visit_car()
        {
            m_new_args.Add(null);
        }

        public override void visit_cdr()
        { }
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

        public override VectVisitor visit_Vect_car()
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

        public override VectVisitor visit_Vect_cdr()
        {
            throw new Exception("combination must be a proper list");
        }

        public override void visit_car()
        {
            m_args.Add(null);
        }

        public override void visit_cdr()
        { }
    }

    class VectInterpreter : VectVisitor {
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
            m_args.Add(m_vect.ToArray());
        }

        public override AtomVisitor visitItem_Atom()
        {
            return new AtomInterpreter(m_env, m_loc, m_vect);
        }

        public override ConsVisitor visitItem_Cons()
        {
            return new CombinationInterpreter(m_env, m_loc, m_vect);
        }

        public override VectVisitor visitItem_Vect()
        {
            return new VectInterpreter(m_env, m_loc, m_vect);
        }

        public override void visitItem()
        {
            m_args.Add(null);
        }
    }
}
