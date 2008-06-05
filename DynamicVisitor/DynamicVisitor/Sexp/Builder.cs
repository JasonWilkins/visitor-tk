using System;
using System.Collections.Generic;
using System.Text;

namespace Sexp {
    public interface IBox {
        void put(object o);
        object get();
    }

    public class Box<T> : IBox where T : class {
        T m_object;
        public T value { get { return m_object; } }
        void IBox.put(object o) { m_object = (T)o; }
        object IBox.get() { return m_object; }
    }

    public class ListBox : IBox {
        readonly List<object> m_list = new List<object>();
        public List<object> list { get { return m_list; } }
        public object[] array { get { return m_list.ToArray(); } }
        void IBox.put(object o) { m_list.Add(o); }
        object IBox.get() { return m_list.ToArray(); }
    }

    public class ProperListBox : IBox {
        Cons m_top;
        Cons m_end;

        public Cons cons { get { return m_top; } }

        void IBox.put(object o)
        {
            if (m_end !=  null) {
                m_end.cdr = new Cons(o);
                m_end = (Cons)m_end.cdr;
            } else {
                m_top = new Cons(o);
                m_end = m_top;
            }
        }

        object IBox.get() { return m_top; }
    }

    public class AtomBox : Box<object> { }
    public class ConsBox : Box<Cons> { }
    public class VectBox : Box<object[]> { }

    public class VectBuilder : VectVisitor {
        protected readonly ListBox vect = new ListBox();

        protected readonly IBox m_outbox;

        public VectBuilder(IBox outbox)
        {
            m_outbox = outbox;
        }

        public override void visitEnd()
        {
            m_outbox.put(vect.array);
        }

        public override AtomVisitor visitItem_Atom() { return new AtomBuilder(vect); }
        public override ConsVisitor visitItem_Cons() { return new ConsBuilder(vect); }
        public override VectVisitor visitItem_Vect() { return new VectBuilder(vect); }

        public override void visitItem()
        {
            ((IBox)vect).put(null);
        }
    }

    public class AtomBuilder : AtomVisitor {
        protected readonly IBox outbox;

        public AtomBuilder(IBox outbox)
        {
            this.outbox = outbox;
        }

        public override void visit(object o) { outbox.put(o); }
    }

    public class ConsBuilder : ConsVisitor {
        protected readonly AtomBox car = new AtomBox();
        protected readonly AtomBox cdr = new AtomBox();

        protected readonly IBox outbox;

        public ConsBuilder(IBox outbox)
        {
            this.outbox = outbox;
        }

        public override void visitEnd()
        {
            outbox.put(new Cons(car.value, cdr.value));
        }

        public override AtomVisitor visit_Atom_car() { return new AtomBuilder(car); }
        public override ConsVisitor visit_Cons_car() { return new ConsBuilder(car); }
        public override VectVisitor visit_Vect_car() { return new VectBuilder(car); }
        public override AtomVisitor visit_Atom_cdr() { return new AtomBuilder(cdr); }
        public override ConsVisitor visit_Cons_cdr() { return new ConsBuilder(cdr); }
        public override VectVisitor visit_Vect_cdr() { return new VectBuilder(cdr); }
    }
}
