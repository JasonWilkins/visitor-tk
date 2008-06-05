using System;
using System.Text;
using System.Collections.Generic;

using Util;

namespace Sexp {
    public class Entry {
        public int sequence;
        public string class_name;
        public string func_name;
        public TxtLocation loc;

        public Entry(int sequence, TxtLocation loc, string class_name, string func_name)
        {
            this.sequence = sequence;
            this.class_name = class_name;
            this.func_name = func_name;
            this.loc = loc.clone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(loc.ToString()).Append(" ").Append(sequence).Append(" ").Append(class_name).Append(" ").Append(func_name).Append(" ").Append(func_name);
            return sb.ToString();
        }
    }

    public class Log : IEnumerable<Entry> {
        List<Entry> log = new List<Entry>();

        public void Add(int sequence, TxtLocation loc, string class_name, string func_name)
        {
            Entry item = new Entry(sequence, loc, class_name, func_name);
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

                if (null == e1 && null == e2) {
                    Console.WriteLine("{0} entries compared.", count);
                    Console.WriteLine("no differences found");
                    Console.WriteLine("OK");
                    return;
                }

                count++;

                if (null == e1) {
                    Console.WriteLine("Log 1 terminates before Log 2");
                    Console.WriteLine(e2.ToString());
                    break;
                }

                if (null == e2) {
                    Console.WriteLine("Log 2 terminates before Log 1");
                    Console.WriteLine(e1.ToString());
                    break;
                }

                if (e1.class_name != e2.class_name) {
                    Console.WriteLine("class names do not match: {0} << >> {1}", e1.class_name, e2.class_name);
                    Console.WriteLine(e1.ToString());
                    Console.WriteLine(e2.ToString());
                    break;
                }

                if (e1.func_name != e2.func_name) {
                    Console.WriteLine("class name: {0}", e1.class_name);
                    Console.WriteLine("function names do not match: {0} << >> {1}", e1.func_name, e2.func_name);
                    Console.WriteLine(e1.ToString());
                    Console.WriteLine(e2.ToString());
                    break;
                }

                if (e1.sequence != e2.sequence) {
                    Console.WriteLine("sequence numbers do not match: {0} << >> {1}", e1.sequence, e2.sequence);
                    Console.WriteLine(e1.ToString());
                    Console.WriteLine(e2.ToString());
                    break;
                }
            }

            Console.WriteLine("FAILED!");
        }
    }

    public class VectorLogger : VectVisitor {
        VectVisitor m_next;
        int m_sequence = 0;
        string m_class_name = "VectorLogger";
        Log m_log = new Log();
        TxtLocation m_loc;

        public VectorLogger(Log log, TxtLocation loc, VectVisitor next)
        {
            m_log = log;
            m_next = next;
            m_loc = loc;
        }

        public override void visit()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visit()");
            m_next.visit();
        }

        public override void visitEnd()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visitEnd()");
            m_next.visitEnd();
        }

        public override AtomVisitor visitItem_Atom()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visitItem_Atom");
            return new AtomLogger(m_log, m_loc, m_next.visitItem_Atom());
        }

        public override ConsVisitor visitItem_Cons()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visitItem_Cons");
            return new ConsLogger(m_log, m_loc, m_next.visitItem_Cons());
        }

        public override VectVisitor visitItem_Vect()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visitItem_Vector");
            return new VectorLogger(m_log, m_loc, m_next.visitItem_Vect());
        }

        public override void visitItem()
        {
            m_log.Add(m_sequence++, m_loc, m_class_name, "visitItem");
            m_next.visitItem();
        }
    }

    public class AtomLogger : AtomVisitor {
        AtomVisitor m_next;
        int m_sequence = 0;
        Log m_log = new Log();
        string m_class_name = "AtomLogger";
        TxtLocation m_loc;

        public AtomLogger(Log log, TxtLocation loc, AtomVisitor next)
        {
            m_log = log;
            m_next = next;
            m_loc =  loc;
        }

        //public override void visit()
        //{
        //    m_log.Add(m_sequence++, m_cons_loc,  m_class_name, "visit()");
        //    m_next.visit();
        //}

        //public override void visitEnd()
        //{
        //    m_log.Add(m_sequence++, m_cons_loc,  m_class_name, "visitEnd()");
        //    m_next.visitEnd();
        //}

        public override void visit(object o)
        {
            m_log.Add(m_sequence++, m_loc, m_class_name, "visit_value(object)");
            m_next.visit(o);
        }

        //public override void visit_value(Boolean o)
        //{
        //    m_log.Add(m_sequence++, m_cons_loc,  m_class_name, "visit_value(Boolean)");
        //    m_next.visit_value(o);
        //}

        //public override void visit_value(Int64 o)
        //{
        //    m_log.Add(m_sequence++, m_cons_loc,  m_class_name, "visit_value(Int64)");
        //    m_next.visit_value(o);
        //}

        //public override void visit_value(Double o)
        //{
        //    m_log.Add(m_sequence++, m_cons_loc,  m_class_name, "visit_value(Double)");
        //    m_next.visit_value(o);
        //}

        //public override void visit_value(String o)
        //{
        //    m_log.Add(m_sequence++, m_cons_loc,  m_class_name, "visit_value(String)");
        //    m_next.visit_value(o);
        //}

        //public override void visit_value(Char o)
        //{
        //    m_log.Add(m_sequence++, m_cons_loc,  m_class_name, "visit_value(Char)");
        //    m_next.visit_value(o);
        //}

        //public override void visit_value(Symbol o)
        //{
        //    m_log.Add(m_sequence++, m_cons_loc,  m_class_name, "visit_value(Symbol)");
        //    m_next.visit_value(o);
        //}
    }

    public class ConsLogger : ConsVisitor {
        ConsVisitor m_next;
        int m_sequence = 0;
        string m_class_name = "ConsLogger";
        Log m_log = new Log();
        TxtLocation m_loc;

        public ConsLogger(Log log, TxtLocation loc, ConsVisitor next)
        {
            m_log = log;
            m_next = next;
            m_loc = loc;
        }

        public override void visit()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visit()");
            m_next.visit();
        }

        public override void visitEnd()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visitEnd()");
            m_next.visitEnd();
        }

        public override AtomVisitor visit_Atom_car()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visit_Atom_car()");
            return new AtomLogger(m_log, m_loc, m_next.visit_Atom_car());
        }
        public override ConsVisitor visit_Cons_car()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visit_Cons_car()");
            return new ConsLogger(m_log, m_loc, m_next.visit_Cons_car());
        }
        public override VectVisitor visit_Vect_car()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visit_Vector_car()");
            return new VectorLogger(m_log, m_loc, m_next.visit_Vect_car());
        }
        public override AtomVisitor visit_Atom_cdr()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visit_Atom_cdr()");
            return new AtomLogger(m_log, m_loc, m_next.visit_Atom_cdr());
        }

        public override ConsVisitor visit_Cons_cdr()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visit_Cons_cdr()");
            return new ConsLogger(m_log, m_loc, m_next.visit_Cons_cdr());
        }

        public override VectVisitor visit_Vect_cdr()
        {
            m_log.Add(m_sequence++, m_loc,  m_class_name, "visit_Vector_cdr()");
            return new VectorLogger(m_log, m_loc, m_next.visit_Vect_cdr());
        }

        public override void visit_car()
        {
            m_log.Add(m_sequence++, m_loc, m_class_name, "visit_car()");
            m_next.visit_car();
        }

        public override void visit_cdr()
        {
            m_log.Add(m_sequence++, m_loc, m_class_name, "visit_cdr()");
            m_next.visit_cdr();
        }
    }
}
