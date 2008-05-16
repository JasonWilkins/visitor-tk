using System;
using System.Collections;

namespace Sexp {
    public interface Datum { }

    public class Atom : Datum {
        public object value;

        public Atom(object value)
        {
            this.value = value;
        }
    }

    public class Cons : Datum {
        public Datum car;
        public Datum cdr;

        public Cons(Datum car, Datum cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }
    }

    public class Vector : Datum, IEnumerable {
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
    }
}
