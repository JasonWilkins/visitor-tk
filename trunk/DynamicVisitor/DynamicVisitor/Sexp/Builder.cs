using System;
using System.Collections.Generic;
using System.Text;

namespace Sexp {
    public class VectorBuilder : VectorVisitor {
        protected List<object> m_vect = new List<object>();
        protected List<object> m_args;

        public VectorBuilder(List<object> args)
        {
            m_args = args;
        }

        public override void visitEnd()
        {
            m_args.Add(m_vect.ToArray());
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
            m_vect.Add(null);
        }
    }

    public class TopLevelBuilder : VectorBuilder {
        public TopLevelBuilder()
            : base(new List<object>())
        { }

        public object[] getTopLevel()
        {
            return m_args.Count > 0 ? (object[])m_args[0] : null;
        }
    }

    public class AtomBuilder : AtomVisitor {
        List<object> m_args;

        public AtomBuilder(List<object> args)
        {
            m_args = args;
        }

        public override void visit(object o)
        {
            if (o == null) throw new Exception();
            m_args.Add(o);
        }

        //public override void visit_value(Boolean o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(Int64 o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(Double o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(Char o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(String o)
        //{
        //    m_args.Add(o);
        //}

        //public override void visit_value(Symbol o)
        //{
        //    m_args.Add(o);
        //}
    }

    public class ConsBuilder : ConsVisitor {
        List<object> m_car = new List<object>();
        List<object> m_cdr = new List<object>();
        List<object> m_args;

        public ConsBuilder(List<object> args)
        {
            m_args = args;
        }

        public override void visitEnd()
        {
            m_args.Add(new Cons(
                /*m_car.Count > 0 ?*/ m_car[0] /*: null*/,
                /*m_cdr.Count > 0 ?*/ m_cdr[0] /*: null*/));
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

        public override void visit_car()
        {
            m_car.Add(null);
        }

        public override void visit_cdr()
        {
            m_cdr.Add(null);
        }
    }

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

    //    public override void visit_car(object o)
    //    {
    //        if (o != null)
    //            throw new Exception();

    //        m_args.Add(o);
    //    }

    //    public override void visit_cdr(object o)
    //    {
    //        if (o != null)
    //            throw new Exception();

    //        m_args.Add(o);
    //    }
    //}
}
