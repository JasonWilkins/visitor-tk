using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicVisitor.Test {
    class NameProperty {
        string m_name;

        public NameProperty(string name)
        {
            m_name = name;
        }

        public string name
        {
            get
            {
                return m_name;
            }
        }
    }

    class Collection<T> : IEnumerable<T> {
        List<T> m_list = new List<T>();

        public void Add(T item)
        {
            m_list.Add(item);
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #endregion
    }

    class Parent : NameProperty {
        Children m_children = new Children();

        public string test_field = null;

        public Children test_field2 = new Children();

        public Children children
        {
            get
            {
                return m_children;
            }
        }

        public Parent(string name)
            : base(name)
        { }
    }

    class Children : Collection<Child> { }

    class Son : NameProperty, Child {
        public Son(string name)
            : base(name)
        { }
    }

    class Daughter : NameProperty, Child {
        public Daughter(string name)
            : base(name)
        { }
    }

    interface Child { }

    class ParentVisitor {
        public void visit()
        {
            Console.WriteLine("ParentVisitor.visit");
        }

        public void visit_name(string name)
        {
            Console.WriteLine("ParentVisitor.visit_name");
            Console.WriteLine("Parent's Name: {0}", name);
        }

        public void visit_test_field(string v)
        {
            Console.WriteLine("ParentVisitor.visit_test_field");
            Console.WriteLine("value: {0}", v);
        }

        public ChildrenVisitor visit_test_field2()
        {
            Console.WriteLine("ParentVisitor.visit_test_field2");
            return new ChildrenVisitor();
        }

        public ChildrenVisitor visit_children()
        {
            Console.WriteLine("ParentVisitor.visit_children");
            return new ChildrenVisitor();
        }

        public void visitEnd()
        {
            Console.WriteLine("ParentVisitor.visitEnd");
        }
    }

    class ChildrenVisitor {
        public void visit()
        {
            Console.WriteLine("ChildrenVisitor.visit");
        }

        public SonVisitor visitItem_Son()
        {
            Console.WriteLine("ChildrenVisitor.visitItem_Son");
            return new SonVisitor();
        }

        public DaughterVisitor visitItem_Daughter()
        {
            Console.WriteLine("ChildrenVisitor.visitItem_Daughter");
            return new DaughterVisitor();
        }

        public void visitEnd()
        {
            Console.WriteLine("ChildrenVisitor.visitEnd");
        }
    }

    class SonVisitor {
        public void visit()
        {
            Console.WriteLine("SonVisitor.visit");
        }

        public void visit_name(string name)
        {
            Console.WriteLine("SonVisitor.visit_name");
            Console.WriteLine("Son's Name: {0}", name);
        }

        public void visitEnd()
        {
            Console.WriteLine("SonVisitor.visitEnd");
        }
    }

    class DaughterVisitor {
        public void visit()
        {
            Console.WriteLine("DaughterVisitor.visit");
        }

        public void visit_name(string name)
        {
            Console.WriteLine("DaughterVisitor.visit_name");
            Console.WriteLine("Daughter's Name: {0}", name);
        }

        public void visitEnd()
        {
            Console.WriteLine("DaughterVisitor.visitEnd");
        }
    }

    class Siblings2 {
        Child m_child;
        Children m_next;

        public Child Child
        {
            get
            {
                return m_child;
            }
        }

        public Children Next
        {
            get
            {
                return m_next;
            }
        }

        public Siblings2(Child child)
            : this(child, null)
        { }

        public Siblings2(Child child, Children next)
        {
            if (null == child) throw new ArgumentNullException("child", "not optional");

            m_child = child;
            m_next = next;
        }
    }
}
