using System;
using System.Collections;

namespace Sexp {
    //public class Atom {
    //    public object value;

    //    public Atom(object value)
    //    {
    //        this.value = value;
    //    }
    //}

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

    /*    public class Vector : Datum, IEnumerable {
            ArrayList list = new ArrayList();

            public void Add(object o)
            {
                list.Add(o);
            }

            public int Count { get { return list.Count; } }

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return list.GetEnumerator();
            }

            #endregion
        }*/
}
