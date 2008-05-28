using System;
using System.Text;

namespace Sexp {
    public static class Literal {
        public static string escape(string input)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in input) {
                switch (c) {
                    case '"':
                        sb.Append("\\\"");
                        break;

                    case '\\':
                        sb.Append("\\\\");
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        public static string format(Boolean v)
        {
            return v ? "#t" : "#f";
        }

        public static string format(Int64 v)
        {
            return v.ToString();
        }
        
        public static string format(Double v)
        {
            return v.ToString();
        }
        
        public static string format(Char v)
        {
            if ('\n' == v) {
                return "#\\newline";
            } else if (' ' == v) {
                return "#\\space";
            } else {
                return "#\\"+v;
            }
        }
        
        public static string format(String v)
        {
            return '"'+escape(v)+'"';
        }
        
        public static string format(Symbol v)
        {
            return v.name;
        }

        public static string format(Object[] v)
        {
            bool is_first = true;

            StringBuilder sb = new StringBuilder("#(");

            foreach (object o in v) {
                if (is_first) {
                    is_first = false;
                } else {
                    sb.Append(" ");
                }

                sb.Append(format(o));
            }

            sb.Append(")");

            return sb.ToString();
        }

        public static string format(object o)
        {
            if (o is Boolean) {
                return format((Boolean)o);
            } else if (o is Char) {
                return format((Char)o);
            } else if (o is Int64) {
                return format((Int64)o);
            } else if (o is Double) {
                return format((Double)o);
            } else if (o is String) {
                return format((String)o);
            } else if (o is Symbol) {
                return format((Symbol)o);
            } else if (o is Object[]) {
                return format((Object[])o);
            } else {
                if (o != null) {
                    return o.ToString();
                } else {
                    return "()";
                }
            }
        }
    }
}
