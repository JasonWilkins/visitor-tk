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

        public static string literal(Boolean v)
        {
            return v ? "#t" : "#f";
        }

        public static string literal(Int64 v)
        {
            return v.ToString();
        }
        
        public static string literal(Double v)
        {
            return v.ToString();
        }
        
        public static string literal(Char v)
        {
            if ('\n' == v) {
                return "#\\newline";
            } else if (' ' == v) {
                return "#\\space";
            } else {
                return "#\\"+v;
            }
        }
        
        public static string literal(String v)
        {
            return '"'+escape(v)+'"';
        }
        
        public static string literal(Symbol v)
        {
            return v.name;
        }

        public static string literal(Object[] v)
        {
            bool is_first = true;

            StringBuilder sb = new StringBuilder("#(");

            foreach (object o in v) {
                if (is_first) {
                    is_first = false;
                } else {
                    sb.Append(" ");
                }

                sb.Append(literal(o));
            }

            sb.Append(")");

            return sb.ToString();
        }

        public static string literal(object o)
        {
            if (o is Boolean) {
                return literal((Boolean)o);
            } else if (o is Char) {
                return literal((Char)o);
            } else if (o is Int64) {
                return literal((Int64)o);
            } else if (o is Double) {
                return literal((Double)o);
            } else if (o is String) {
                return literal((String)o);
            } else if (o is Symbol) {
                return literal((Symbol)o);
            } else if (o is Object[]) {
                return literal((Object[])o);
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
