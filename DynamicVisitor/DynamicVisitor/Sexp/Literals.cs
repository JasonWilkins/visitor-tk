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
    }
}
