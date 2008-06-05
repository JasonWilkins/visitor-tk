using System;
using System.Collections.Generic;

namespace Sexp {
    public class Closure {
        public delegate object Fn(object[] args);

        public Fn fn;

        public Closure(Fn fn)
        {
            this.fn = fn;
        }
    }
}
