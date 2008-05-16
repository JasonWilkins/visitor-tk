using System;
using System.Collections.Generic;

using Util;

namespace Sexp {
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
}
