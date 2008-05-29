using System;
using System.Collections.Generic;
using System.Text;

namespace Sexp {
    public interface IBox {
        void put(object o);
    }

    public class Box<T> : IBox where T :class {
        T m_object;
        public T value { get { return m_object; } }
        void IBox.put(object o) { m_object = (T)o; }
    }

    public class ListBox : IBox {
        readonly List<object> m_list = new List<object>();
        public List<object> list { get { return m_list; } }
        public object[] array { get { return m_list.ToArray(); } }
        void IBox.put(object o) { m_list.Add(o); }
    }

    public class AtomBox : Box<object> { }
    public class ConsBox : Box<Cons> { }
    public class VectBox : Box<object[]> { }

    public class VectBuilder : VectVisitor {
        readonly ListBox m_vect = new ListBox();

        readonly IBox m_outbox;

        public VectBuilder(IBox outbox)
        {
            m_outbox = outbox;
        }

        public override void visitEnd()
        {
            m_outbox.put(m_vect.array);
        }

        public override AtomVisitor visitItem_Atom() { return new AtomBuilder(m_vect); }
        public override ConsVisitor visitItem_Cons() { return new ConsBuilder(m_vect); }
        public override VectVisitor visitItem_Vect() { return new VectBuilder(m_vect); }

        public override void visitItem()
        {
            ((IBox)m_vect).put(null);
        }
    }

    public class AtomBuilder : AtomVisitor {
        readonly IBox m_outbox;

        public AtomBuilder(IBox outbox)
        {
            m_outbox = outbox;
        }

        public override void visit(object o)
        {
            m_outbox.put(o);
        }
    }

    public class ConsBuilder : ConsVisitor {
        readonly AtomBox m_car = new AtomBox();
        readonly AtomBox m_cdr = new AtomBox();

        readonly IBox m_outbox;

        public ConsBuilder(IBox outbox)
        {
            m_outbox = outbox;
        }

        public override void visitEnd()
        {
            m_outbox.put(new Cons(m_car.value, m_cdr.value));
        }

        public override AtomVisitor visit_Atom_car() { return new AtomBuilder(m_car); }
        public override ConsVisitor visit_Cons_car() { return new ConsBuilder(m_car); }
        public override VectVisitor visit_Vect_car() { return new VectBuilder(m_car); }
        public override AtomVisitor visit_Atom_cdr() { return new AtomBuilder(m_cdr); }
        public override ConsVisitor visit_Cons_cdr() { return new ConsBuilder(m_cdr); }
        public override VectVisitor visit_Vect_cdr() { return new VectBuilder(m_cdr); }
    }
}
