using System;
using System.Collections.Generic;
using System.Text;

using Util;
using Symbols;

namespace Sexp {
    public class InterpreterException : TxtException {
        public InterpreterException(TxtLocation loc, string message)
            : base(loc, message)
        { }

        public InterpreterException(TxtLocation loc, string message, Exception innerException)
            : base(loc, message, innerException)
        { }
    }

    public class Print {
        public void print(IBox box)
        {
            object o = box.get();
            string typename = o != null ? o.GetType().FullName : typeof(void).FullName;
            Console.WriteLine(" => <{0}> {1}", typename, Literal.try_format(o));
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
            return new TopLevelAtomInterpreter(m_env, m_loc, m_print, new AtomBox());
        }

        public override ConsVisitor visitItem_Cons()
        {
            return new TopLevelConsInterpreter(m_env, m_loc, m_print, new AtomBox());
        }

        public override VectVisitor visitItem_Vect()
        {
            return new TopLevelVectInterpreter(m_env, m_loc, m_print, new AtomBox());
        }

        public override void visitItem()
        {
            m_print.print(new AtomBox());
        }
    }

    // TODO: These three classes are identical, how do I unify them?

    class TopLevelAtomInterpreter : AtomInterpreter {
        Print m_print;

        public TopLevelAtomInterpreter(Environment env, TxtLocation loc, Print print, IBox outbox)
            : base(env, loc, outbox)
        {
            m_print = print;
        }

        public override void visit(object o)
        {
            base.visit(o);
            m_print.print(outbox);
        }

        public override void visit(Symbol o)
        {
            base.visit(o);
            m_print.print(outbox);
        }
    }

    class TopLevelConsInterpreter : CombinationInterpreter {
        Print m_print;

        public TopLevelConsInterpreter(Environment env, TxtLocation loc, Print print, IBox outbox)
            : base(env, loc, outbox)
        {
            m_print = print;
        }

        public override void visitEnd()
        {
            base.visitEnd();
            m_print.print(outbox);
        }
    }

    class TopLevelVectInterpreter : VectInterpreter {
        Print m_print;

        public TopLevelVectInterpreter(Environment env, TxtLocation loc, Print print, IBox outbox)
            : base(env, loc, outbox)
        {
            m_print = print;
        }

        public override void visitEnd()
        {
            base.visitEnd();
            m_print.print(m_outbox);
        }
    }

    public class CombinationInterpreter : ConsBuilder {
        readonly Environment m_env;
        readonly TxtLocation m_loc;

        ListBox m_appl_args;

        public CombinationInterpreter(Environment env, TxtLocation loc, IBox outbox)
            : base(outbox)
        {
            m_env = env;
            m_loc = loc;
        }

        public override void visitEnd()
        {
            if (car.value is SpecialForm) {
                outbox.put(m_env.transform(m_loc, (SpecialForm)car.value, cdr.value));
            } else if (car.value is Closure) {
                outbox.put(m_env.apply(m_loc, (Closure)car.value, m_appl_args.array));
            } else {
                throw new InterpreterException(m_loc, "cannot apply object <" + car.value.GetType().FullName + "> " + Literal.try_format(car.value));
            }
        }

        public override AtomVisitor visit_Atom_car()
        {
            return new AtomInterpreter(m_env, m_loc, car);
        }

        public override ConsVisitor visit_Cons_car()
        {
            return new CombinationInterpreter(m_env, m_loc, car);
        }

        public override VectVisitor visit_Vect_car()
        {
            throw new InterpreterException(m_loc, "cannot apply vector");
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            if (car.value is SpecialForm) {
                return base.visit_Atom_cdr();
            } else {
                throw new InterpreterException(m_loc, "list tail was an atom, cannot apply an improper list");
            }
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            if (car.value is SpecialForm) {
                return base.visit_Cons_cdr();
            } else {
                m_appl_args = new ListBox();
                return new ArgsInterpreter(m_env, m_loc, m_appl_args);
            }
        }

        public override VectVisitor visit_Vect_cdr()
        {
            if (car.value is SpecialForm) {
                return base.visit_Vect_cdr();
            } else {
                throw new InterpreterException(m_loc, "list tail was a vector, cannot apply an improper list");
            }
        }
    }

    class ArgsInterpreter : ConsVisitor {
        readonly Environment m_env;
        readonly TxtLocation m_loc;

        ListBox m_args;

        public ArgsInterpreter(Environment env, TxtLocation loc, ListBox args)
        {
            m_env = env;
            m_loc = loc;
            m_args = args;
        }

        public override AtomVisitor visit_Atom_car() { return new AtomInterpreter(m_env, m_loc, m_args); }
        public override ConsVisitor visit_Cons_car() { return new CombinationInterpreter(m_env, m_loc, m_args); }
        public override VectVisitor visit_Vect_car() { return new VectInterpreter(m_env, m_loc, m_args); }
        public override void visit_car() { ((IBox)m_args).put(null); }

        public override AtomVisitor visit_Atom_cdr()
        {
            throw new InterpreterException(m_loc, "list tail was an atom, cannot apply an improper list");
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            return new ArgsInterpreter(m_env, m_loc, m_args);
        }

        public override VectVisitor visit_Vect_cdr()
        {
            throw new InterpreterException(m_loc, "list tail was a vector, cannot apply an improper list");
        }
    }

    class VectInterpreter : VectBuilder {
        readonly Environment m_env;
        readonly TxtLocation m_loc;

        public VectInterpreter(Environment env, TxtLocation loc, IBox outbox)
            : base(outbox)
        {
            m_env = env;
            m_loc = loc;
        }

        public override AtomVisitor visitItem_Atom() { return new AtomInterpreter(m_env, m_loc, vect); }
        public override ConsVisitor visitItem_Cons() { return new CombinationInterpreter(m_env, m_loc, vect); }
        public override VectVisitor visitItem_Vect() { return new VectInterpreter(m_env, m_loc, vect); }
    }

    class AtomInterpreter : AtomBuilder {
        readonly Environment m_env;
        readonly TxtLocation m_loc;

        public AtomInterpreter(Environment env, TxtLocation loc, IBox outbox)
            : base(outbox)
        {
            m_env = env;
            m_loc = loc;
        }

        public override void visit(object o) { outbox.put(o); }
        public override void visit(Symbol o) { outbox.put(m_env.lookup(m_loc, o)); }
    }
}
