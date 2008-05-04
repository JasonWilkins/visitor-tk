using System;
using System.Text;
using System.Collections.Generic;

namespace Sexp {
    public class Entry {
        public int sequence;
        public string class_name;
        public string func_name;

        public Entry(int sequence, string class_name, string func_name)
        {
            this.sequence = sequence;
            this.class_name = class_name;
            this.func_name = func_name;
        }
    }

    public class Log : IEnumerable<Entry> {
        List<Entry> log = new List<Entry>();

        public void Add(int sequence, string class_name, string func_name)
        {
            Entry item = new Entry(sequence, class_name, func_name);
            log.Add(item);
        }

        #region IEnumerable<Entry> Members

        IEnumerator<Entry> IEnumerable<Entry>.GetEnumerator()
        {
            return log.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return log.GetEnumerator();
        }

        #endregion
    }

    public static class LogComparer {
        public static void compare_logs(Log l1, Log l2) {
            int count = 0;
            IEnumerator<Entry> el1 = (l1 as IEnumerable<Entry>).GetEnumerator();
            IEnumerator<Entry> el2 = (l2 as IEnumerable<Entry>).GetEnumerator();

            while (true) {
                el1.MoveNext();
                el2.MoveNext();

                Entry e1 = el1.Current;
                Entry e2 = el2.Current;
                count++;

                if (null == e1 && e2 != null) {
                    Console.WriteLine("Log 1 terminates before Log 2");
                    break;
                }

                if (null == e1 && e2 != null) {
                    Console.WriteLine("Log 2 terminates before Log 1");
                    break;
                }

                if (null == e1 || null == e2) {
                    Console.WriteLine("{0} entries compared.", count);
                    Console.WriteLine("no differences found");
                    break;
                }

                if (e1.class_name != e2.class_name) {
                    Console.WriteLine("class names do not match: {0} << >> {1}", e1.class_name, e2.class_name);
                    break;
                }

                if (e1.func_name != e2.func_name) {
                    Console.WriteLine("function names do not match: {0} << >> {1}", e1.func_name, e2.func_name);
                    break;
                }

                if (e1.sequence != e2.sequence) {
                    Console.WriteLine("sequence numbers do not match: {0} << >> {1}", e1.sequence, e2.sequence);
                    break;
                }
            }
        }
    }

    public class VectorLogger : VectorVisitor {
        VectorVisitor m_next;
        int m_sequence = 0;
        string m_class_name = "VectorLogger";
        Log m_log = new Log();

        public VectorLogger(Log log, VectorVisitor next)
        {
            m_log = log;
            m_next = next;
        }

        public override void visit()
        {
            m_log.Add(m_sequence++, m_class_name, "visit()");
            m_next.visit();
        }

        public override void visitEnd()
        {
            m_log.Add(m_sequence++, m_class_name, "visitEnd()");
            m_next.visitEnd();
        }

        public override AtomVisitor visitItem_Atom()
        {
            m_log.Add(m_sequence++, m_class_name, "visitItem_Atom");
            return new AtomLogger(m_log, m_next.visitItem_Atom());
        }

        public override ConsVisitor visitItem_Cons()
        {
            m_log.Add(m_sequence++, m_class_name, "visitItem_Cons");
            return new ConsLogger(m_log, m_next.visitItem_Cons());
        }

        public override VectorVisitor visitItem_Vector()
        {
            m_log.Add(m_sequence++, m_class_name, "visitItem_Vector");
            return new VectorLogger(m_log, m_next.visitItem_Vector());
        }
    }

    public class AtomLogger : AtomVisitor {
        AtomVisitor m_next;
        int m_sequence = 0;
        Log m_log = new Log();
        string m_class_name = "AtomLogger";

        public AtomLogger(Log log, AtomVisitor next)
        {
            m_log = log;
            m_next = next;
        }

        public override void visit()
        {
            m_log.Add(m_sequence++, m_class_name, "visit()");
            m_next.visit();
        }

        public override void visitEnd()
        {
            m_log.Add(m_sequence++, m_class_name, "visitEnd()");
            m_next.visitEnd();
        }

        public override void visit_value(Boolean o)
        {
            m_log.Add(m_sequence++, m_class_name, "visit_value(Boolean)");
            m_next.visit_value(o);
        }

        public override void visit_value(Int64 o)
        {
            m_log.Add(m_sequence++, m_class_name, "visit_value(Int64)");
            m_next.visit_value(o);
        }

        public override void visit_value(Double o)
        {
            m_log.Add(m_sequence++, m_class_name, "visit_value(Double)");
            m_next.visit_value(o);
        }

        public override void visit_value(String o)
        {
            m_log.Add(m_sequence++, m_class_name, "visit_value(String)");
            m_next.visit_value(o);
        }

        public override void visit_value(Char o)
        {
            m_log.Add(m_sequence++, m_class_name, "visit_value(Char)");
            m_next.visit_value(o);
        }

        public override void visit_value(Object o)
        {
            m_log.Add(m_sequence++, m_class_name, "visit_value(Object)");
            m_next.visit_value(o);
        }

        public override void visit_value(Symbol o)
        {
            m_log.Add(m_sequence++, m_class_name, "visit_value(Symbol)");
            m_next.visit_value(o);
        }
    }

    public class ConsLogger : ConsVisitor {
        ConsVisitor m_next;
        int m_sequence = 0;
        string m_class_name = "ConsLogger";
        Log m_log = new Log();

        public ConsLogger(Log log, ConsVisitor next)
        {
            m_log = log;
            m_next = next;
        }

        public override void visit()
        {
            m_log.Add(m_sequence++, m_class_name, "visit()");
            m_next.visit();
        }

        public override void visitEnd()
        {
            m_log.Add(m_sequence++, m_class_name, "visitEnd()");
            m_next.visitEnd();
        }

        public override AtomVisitor visit_Atom_car()
        {
            m_log.Add(m_sequence++, m_class_name, "visit_Atom_car()");
            return new AtomLogger(m_log, m_next.visit_Atom_car());
        }
        public override ConsVisitor visit_Cons_car()
        {
            m_log.Add(m_sequence++, m_class_name, "visit_Cons_car()");
            return new ConsLogger(m_log, m_next.visit_Cons_car());
        }
        public override VectorVisitor visit_Vector_car()
        {
            m_log.Add(m_sequence++, m_class_name, "visit_Vector_car()");
            return new VectorLogger(m_log, m_next.visit_Vector_car());
        }
        public override AtomVisitor visit_Atom_cdr()
        {
            m_log.Add(m_sequence++, m_class_name, "visit_Atom_cdr()");
            return new AtomLogger(m_log, m_next.visit_Atom_cdr());
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            m_log.Add(m_sequence++, m_class_name, "visit_Cons_cdr()");
            return new ConsLogger(m_log, m_next.visit_Cons_cdr());
        }

        public override VectorVisitor visit_Vector_cdr()
        {
            m_log.Add(m_sequence++, m_class_name, "visit_Vector_cdr()");
            return new VectorLogger(m_log, m_next.visit_Vector_cdr());
        }
    }

    //public class SymbolLogger : SymbolVisitor {
    //    SymbolVisitor m_next;
    //    int m_sequence = 0;
    //    string m_class_name = "SymbolLogger";
    //    Log m_log = new Log();

    //    public SymbolLogger(Log log, SymbolVisitor next)
    //    {
    //        m_log = log;
    //        m_next = next;
    //    }

    //    public override void visit()
    //    {
    //        m_log.Add(m_sequence++, m_class_name, "visit()");
    //        m_next.visit();
    //    }

    //    public override void visitEnd()
    //    {
    //        m_log.Add(m_sequence++, m_class_name, "visitEnd()");
    //        m_next.visitEnd();
    //    }

    //    public override void visit_name(String name)
    //    {
    //        m_log.Add(m_sequence++, m_class_name, "visit_name(String)");
    //        m_next.visit_name(name);
    //    }
    //}
}
