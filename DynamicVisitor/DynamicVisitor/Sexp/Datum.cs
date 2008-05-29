using System;
using System.Collections;

namespace Sexp {
    public class Cons {
        public object car;
        public object cdr;

        public Cons()
        { }

        public Cons(object car)
        {
            this.car = car;
        }

        public Cons(object car, object cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }
    }
}
