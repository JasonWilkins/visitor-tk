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

    public class Symbol {
        public string name;

        public Symbol(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }

        public override bool Equals(object obj)
        {
            if (obj is Symbol) {
                return (obj as Symbol).name == this.name;
            } else {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }

    public class Vector : Datum, IEnumerable {
        ArrayList list = new ArrayList();

        public void Add(object o)
        {
            list.Add(o);
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion
    }
}
