using System;
using System.Collections.Generic;

using Util;
using Symbols;

namespace Sexp {
    public class Environment {
        Environment m_parent;
        Dictionary<Symbol, object> definitions = new Dictionary<Symbol, object>();

        public delegate bool Trap(Symbol sym, out object def);

        Trap m_trap;

        public Environment()
            : this(null, no_trap)
        { }

        public Environment(Environment parent)
            : this(parent, no_trap)
        { }

        public Environment(Environment parent, Trap trap)
        {
            m_trap = trap;
            m_parent = parent;
        }

        public object lookup(TxtLocation loc, Symbol sym)
        {
            object def;

            if (definitions.TryGetValue(sym, out def)) {
                return def;
            } else if (m_parent != null) {
                return m_parent.lookup(loc, sym);
            } else if (m_trap(sym, out def)) {
                return def;
            } else {
                throw new InterpreterException(loc, sym.name + " is undefined");
            }
        }

        public object apply(TxtLocation loc, Closure fn, object[] args)
        {
            try {
                return fn.fn(args);
            } catch (Exception e) {
                throw new InterpreterException(loc, "exception occured in method: " + fn.fn.Method.Name, e);
            }
        }

        public object transform(TxtLocation loc, SpecialForm m, object source)
        {
            try {
                return m.m(source);
            } catch (Exception e) {
                throw new InterpreterException(loc, "exception occured in special form: " + m.m.Method.Name, e);
            }
        }

        public void Add(Symbol sym, object def)
        {
            definitions.Add(sym, def);
        }

        public void AddFn(Symbol sym, Closure.Fn fn)
        {
            definitions.Add(sym, new Closure(fn));
        }

        public void AddMacro(Symbol sym, SpecialForm.Macro m)
        {
            definitions.Add(sym, new SpecialForm(m));
        }

        static bool no_trap(Symbol sym, out object def)
        {
            def = null;
            return false;
        }
    }
}
