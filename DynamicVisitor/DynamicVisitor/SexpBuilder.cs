using System;
using System.Collections.Generic;
using System.Text;

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

    public class Vector : Datum, IEnumerable<object> {
        List<object> list = new List<object>();

        public void Add(object o)
        {
            list.Add(o);
        }

        #region IEnumerable<object> Members

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion
    }

    public class VectorBuilder : VectorVisitor {
        List<object> tmp = new List<object>();

        public Vector getVector()
        {
            Vector vect = new Vector();

            foreach (object o in tmp) {
                vect.Add(Helper.getValue(o));
            }

            return vect;
        }

        public override AtomVisitor visitItem_Atom()
        {
            AtomBuilder atom = new AtomBuilder();
            tmp.Add(atom);
            return atom;
        }

        public override ConsVisitor visitItem_Cons()
        {
            ConsBuilder cons = new ConsBuilder();
            tmp.Add(cons);
            return cons;
        }

        public override VectorVisitor visitItem_Vector()
        {
            VectorBuilder vect = new VectorBuilder();
            tmp.Add(vect);
            return vect;
        }
    }

    public class AtomBuilder : AtomVisitor {
        object tmp;

        public Datum getAtom()
        {
            if (tmp is SymbolBuilder) {
                return new Atom((tmp as SymbolBuilder).getSymbol());
            } else {
                return new Atom(tmp);
            }
        }

        public override void visit_value(Boolean o)
        {
            tmp = o;
        }

        public override void visit_value(Int64 o)
        {
            tmp = o;
        }

        public override void visit_value(Double o)
        {
            tmp = o;
        }

        public override void visit_value(String o)
        {
            tmp = o;
        }

        public override void visit_value(Char o)
        {
            tmp = o;
        }

        public override SymbolVisitor visit_Symbol_value()
        {
            SymbolBuilder sym = new SymbolBuilder();
            tmp = sym;
            return sym;
        }
    }

    class Helper {
        public static Datum getValue(object o)
        {
            if (o != null) {
                if (o is ConsBuilder) {
                    return (o as ConsBuilder).getCons();
                } else if (o is VectorBuilder) {
                    return (o as VectorBuilder).getVector();
                } else {
                    return (o as AtomBuilder).getAtom();
                }
            } else {
                return null;
            }
        }
    }

    public class ConsBuilder : ConsVisitor {
        object tmp_car;
        object tmp_cdr;

        public Cons getCons()
        {
            return new Cons(Helper.getValue(tmp_car), Helper.getValue(tmp_cdr));
        }

        public override AtomVisitor visit_Atom_car()
        {
            AtomBuilder atom = new AtomBuilder();
            tmp_car = atom;
            return atom;
        }

        public override ConsVisitor visit_Cons_car()
        {
            ConsBuilder cons = new ConsBuilder();
            tmp_car = cons;
            return cons;
        }

        public override VectorVisitor visit_Vector_car()
        {
            VectorBuilder vect = new VectorBuilder();
            tmp_car = vect;
            return vect;
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            AtomBuilder atom = new AtomBuilder();
            tmp_cdr = atom;
            return atom;
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            ConsBuilder cons = new ConsBuilder();
            tmp_cdr = cons;
            return cons;
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            VectorBuilder vect = new VectorBuilder();
            tmp_cdr = vect;
            return vect;
        }

    }

    public class SymbolBuilder : SymbolVisitor {
        Symbol sym;

        public Symbol getSymbol()
        {
            return sym;
        }

        public override void visit_name(string name)
        {
            sym = new Symbol(name);
        }
    }
}
