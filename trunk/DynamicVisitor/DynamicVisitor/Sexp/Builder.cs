using System;
using System.Collections.Generic;
using System.Text;

namespace Sexp {
#if false
    public class Cell<T> {
        public T Value;
    }

    public class VectorBuilder : VectorVisitor {
        List<object> m_vect = new List<object>();

        public object[] Value
        {
            get { return m_vect.ToArray(); }
        }

        public override AtomVisitor visitItem_Atom()
        {
            return new AtomBuilder1(m_vect);
        }

        public override ConsVisitor visitItem_Cons()
        {
            return new ConsBuilder1(m_vect);
        }

        public override VectorVisitor visitItem_Vector()
        {
            return new VectorBuilder1(m_vect);
        }

        public override void visitItem()
        {
            m_vect.Add(null);
        }
    }

    public class VectorBuilder1 : VectorBuilder {
        List<object> m_args;

        public VectorBuilder1(List<object> args)
        {
            m_args = args;
        }

        public override void visitEnd()
        {
            m_args.Add(Value);
        }
    }

    public class VectorBuilder2 : VectorBuilder {
        Cell<object> m_arg;

        public VectorBuilder2(Cell<object> arg)
        {
            m_arg = arg;
        }

        public override void visitEnd()
        {
            m_arg.Value = Value;
        }
    }

    public class AtomBuilder1 : AtomVisitor {
        List<object> m_args;

        public AtomBuilder1(List<object> args)
        {
            m_args = args;
        }

        public override void visit(object o)
        {
            m_args.Add(o);
        }
    }

    public class AtomBuilder2 : AtomVisitor {
        Cell<object> m_arg;

        public AtomBuilder2(Cell<object> arg)
        {
            m_arg = arg;
        }

        public override void visit(object o)
        {
            m_arg.Value = o;
        }
    }

    public class ConsBuilder : ConsVisitor {
        Cell<object> m_car = new Cell<object>();
        Cell<object> m_cdr = new Cell<object>();

        public Cons Value
        {
            get { return new Cons(m_car.Value, m_cdr.Value); }
        }

        public override AtomVisitor visit_Atom_car()
        {
            return new AtomBuilder2(m_car);
        }

        public override ConsVisitor visit_Cons_car()
        {
            return new ConsBuilder2(m_car);
        }

        public override VectorVisitor visit_Vector_car()
        {
            return new VectorBuilder2(m_car);
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            return new AtomBuilder2(m_cdr);
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            return new ConsBuilder2(m_cdr);
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            return new VectorBuilder2(m_cdr);
        }
    }

    public class ConsBuilder1 : ConsBuilder {
        List<object> m_args;

        public ConsBuilder1(List<object> args)
        {
            m_args = args;
        }

        public override void visitEnd()
        {
            m_args.Add(Value);
        }
    }

    public class ConsBuilder2 : ConsBuilder {
        Cell<object> m_arg;

        public ConsBuilder2(Cell<object> arg)
        {
            m_arg = arg;
        }

        public override void visitEnd()
        {
            m_arg.Value =  Value;
        }
    }

#else

    public interface Ctor {
        object value { get; }
        void pass(object o);
    }

    public class ListCtor : Ctor {
        List<object> m_list = new List<object>();
        public object value { get { return m_list; } }
        public void pass(object o) { m_list.Add(o); }
    }

    public class VectCtor : Ctor {
        List<object> m_vect = new List<object>();
        public object value { get { return m_vect.ToArray(); } }
        public void pass(object o) { m_vect.Add(o); }
    }

    public class AtomCtor : Ctor {
        object m_object;
        public object value { get { return m_object; } }
        public void pass(object o) { m_object = o; }
    }

    public class VectorBuilder : VectorVisitor {
        VectCtor m_vect = new VectCtor();
        Ctor m_args;

        public VectorBuilder(Ctor args)
        {
            m_args = args;
        }

        public override void visitEnd()
        {
            m_args.pass(m_vect.value);
        }

        public override AtomVisitor visitItem_Atom()
        {
            return new AtomBuilder(m_vect);
        }

        public override ConsVisitor visitItem_Cons()
        {
            return new ConsBuilder(m_vect);
        }

        public override VectorVisitor visitItem_Vector()
        {
            return new VectorBuilder(m_vect);
        }

        public override void visitItem()
        {
            m_vect.pass(null);
        }
    }

    public class AtomBuilder : AtomVisitor {
        Ctor m_args;

        public AtomBuilder(Ctor args)
        {
            m_args = args;
        }

        public override void visit(object o)
        {
            m_args.pass(o);
        }
    }

    public class ConsBuilder : ConsVisitor {
        AtomCtor m_car = new AtomCtor();
        AtomCtor m_cdr = new AtomCtor();
        Ctor m_args;

        public ConsBuilder(Ctor args)
        {
            m_args = args;
        }

        public override void visitEnd()
        {
            m_args.pass(new Cons(m_car.value, m_cdr.value));
        }

        public override AtomVisitor visit_Atom_car()
        {
            return new AtomBuilder(m_car);
        }

        public override ConsVisitor visit_Cons_car()
        {
            return new ConsBuilder(m_car);
        }

        public override VectorVisitor visit_Vector_car()
        {
            return new VectorBuilder(m_car);
        }

        public override AtomVisitor visit_Atom_cdr()
        {
            return new AtomBuilder(m_cdr);
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            return new ConsBuilder(m_cdr);
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            return new VectorBuilder(m_cdr);
        }
    }
#endif
    //public class ListBuilder : ConsVisitor {
    //    List<object> m_list = new List<object>();
    //    List<object> m_tail;
    //    List<object> m_args;

    //    public ListBuilder(List<object> args)
    //    {
    //        m_args = args;
    //    }

    //    public override void visitEnd()
    //    {
    //        if (m_tail != null) {
    //            int last = m_list.Count-1;
    //            Cons cons = new Cons(m_list[last], m_tail[0]);
    //            m_list.RemoveAt(last);
    //            m_list.Add(cons);
    //        }

    //        m_args.Add(m_list);
    //    }

    //    public override AtomVisitor visit_Atom_car()
    //    {
    //        return new AtomBuilder(m_list);
    //    }

    //    public override ConsVisitor visit_Cons_car()
    //    {
    //        return new ConsBuilder(m_list);
    //    }

    //    public override VectorVisitor visit_Vector_car()
    //    {
    //        return new VectorBuilder(m_list);
    //    }

    //    public override AtomVisitor visit_Atom_cdr()
    //    {
    //        m_tail = new List<object>();
    //        return new AtomBuilder(m_tail);
    //    }

    //    public override ConsVisitor visit_Cons_cdr()
    //    {
    //        return new ConsBuilder(m_list);
    //    }

    //    public override VectorVisitor visit_Vector_cdr()
    //    {
    //        m_tail = new List<object>();
    //        return new VectorBuilder(m_tail);
    //    }

    //    public override void visit_car()
    //    {
    //        m_args.Add(null);
    //    }

    //    public override void visit_cdr()
    //    {
    //        m_args.Add(null);
    //    }
    //}
}
