using System;
using System.Text;

using Symbols;

namespace Sexp {
    public static class Literal {
        public static bool is_atom_type(object o)
        {
            return
                o is bool     ||
                o is char     ||
                o is long     ||
                o is double   ||
                o is string   ||
                o is Symbol   ||
                o is Rational ||
                o is Complex  ||
                o == null;
        }

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

        public static string format(Rational r)
        {
            return r.ToString();
        }

        public static string format(Complex c)
        {
            return format(c.real_part) + format_imaginary(c.imaginary_part);
        }

        static string format_imaginary<T> (T i) where T : IComparable 
        {
            T ci = (T)i;

            if (ci.Equals(1)) {
                return "+i";
            } else if (ci.Equals(-1)) {
                return "-i";
            } else if (ci.CompareTo(0) > 0) {
                return "+"+format(ci)+'i';
            } else if (ci.CompareTo(0) < 0) {
                return format(ci)+'i';
            } else {
                return "";
            }
        }

        static string format_imaginary(object i)
        {
            if (i is long) {
                return format_imaginary<long>((long)i);
            } else if (i is float) {
                return format_imaginary<float>((float)i);
            } else if (i is double) {
                return format_imaginary<double>((double)i);
            } else if (i is Rational) {
                Rational ci = (Rational)i;

                if (ci.Equals(1)) {
                    return "+i";
                } else if (ci.Equals(-1)) {
                    return "-i";
                } else if (ci.CompareTo(0) > 0) {
                    return "+"+format(ci);
                } else {
                    return format(ci);
                }
            } else {
                throw new Exception();
            }
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
            } else if (o == null) {
                return "()";
            } else {
                throw new Exception();
            }
        }

        public static string try_format(object o)
        {
            try {
                return format(o);
            } catch {
                return String.Empty;
            }
        }
    }
}
