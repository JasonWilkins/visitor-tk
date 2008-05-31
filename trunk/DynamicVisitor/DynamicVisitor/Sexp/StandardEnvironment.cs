using System;
using System.Collections.Generic;

using Symbols;

namespace Sexp {
    public class StandardEnvironment : Environment {
        static object fn_eq(List<object> args)
        {
            for (int i = 0; i < args.Count-1; i++) {
                object a = args[i];
                object b = args[i+1];

                if ((a is Int64) && (b is Int64)) {
                    if ((Int64)a != (Int64)b) return false;
                } else if ((a is Double) && (b is Double)) {
                    if ((Double)a != (Double)b) return false;
                } else {
                    if (a is Double) {
                        if ((Double)a != (Double)(Int64)b) return false;
                    } else {
                        if ((Double)(Int64)a != (Double)b) return false;
                    }
                }
            }

            return true;
        }

        static object fn_eqv_pred(List<object> args)
        {
            object a = args[0];
            object b = args[1];

            return a == b || (a != null && a.Equals(b));
        }

        static object fn_eq_pred(List<object> args)
        {
            object a = args[0];
            object b = args[1];

            return a == b || (a is ValueType && a.Equals(b));
        }

        static object fn_add(List<object> args)
        {
            object total = 0L;

            foreach (object o in args) {
                if ((total is Int64) && (o is Int64)) {
                    total = (Int64)total + (Int64)o;
                } else if ((total is Double) && (o is Double)) {
                    total = (Double)total + (Double)o;
                } else {
                    if (total is Double) {
                        total = (Double)total + (Int64)o;
                    } else {
                        total = (Int64)total + (Double)o;
                    }
                }
            }

            return total;
        }

        static object fn_sub(List<object> args)
        {
            if (args.Count > 1) {
                object total = args[0];
                args.RemoveAt(0);

                foreach (object o in args) {
                    if ((total is Int64) && (o is Int64)) {
                        total = (Int64)total - (Int64)o;
                    } else if ((total is Double) && (o is Double)) {
                        total = (Double)total - (Double)o;
                    } else {
                        if (total is Double) {
                            total = (Double)total - (Int64)o;
                        } else {
                            total = (Int64)total - (Double)o;
                        }
                    }
                }

                return total;
            } else {
                object rv = args[0];

                if (rv is Int64) {
                    return -(Int64)rv;
                } else {
                    return -(Double)rv;
                }
            }
        }

        static object fn_mul(List<object> args)
        {
            object total = 1L;

            foreach (object o in args) {
                if ((total is Int64) && (o is Int64)) {
                    total = (Int64)total * (Int64)o;
                } else if ((total is Double) && (o is Double)) {
                    total = (Double)total * (Double)o;
                } else {
                    if (total is Double) {
                        total = (Double)total * (Int64)o;
                    } else {
                        total = (Int64)total * (Double)o;
                    }
                }
            }

            return total;
        }

        static object fn_div(List<object> args)
        {
            if (args.Count > 1) {
                object total = args[0];
                args.RemoveAt(0);

                foreach (object o in args) {
                    if ((total is Int64) && (o is Int64)) {
                        total = (Int64)total / (Int64)o;
                    } else if ((total is Double) && (o is Double)) {
                        total = (Double)total / (Double)o;
                    } else {
                        if (total is Double) {
                            total = (Double)total / (Int64)o;
                        } else {
                            total = (Int64)total / (Double)o;
                        }
                    }
                }

                return total;
            } else {
                object rv = args[0];

                if (rv is Int64) {
                    return 1/(Int64)rv;
                } else {
                    return 1/(Double)rv;
                }
            }
        }

        static object fn_boolean_pred(List<object> args)
        {
            return args[0] is Boolean;
        }

        static object fn_char_pred(List<object> args)
        {
            return args[0] is Char;
        }

        static object fn_null_pred(List<object> args)
        {
            return args[0] == null;
        }

        static object fn_number_pred(List<object> args)
        {
            return args[0] is Int64 || args[0] is Double;
        }

        static object fn_pair_pred(List<object> args)
        {
            return args[0] is Pair || args[0] is List<object>;
        }

        static object fn_list_pred(List<object> args)
        {
            return (bool)fn_null_pred(args) || (bool)fn_pair_pred(args);
        }

        static object fn_procedure_pred(List<object> args)
        {
            return args[0] is Closure;
        }

        static object fn_string_pred(List<object> args)
        {
            return args[0] is String;
        }

        static object fn_symbol_pred(List<object> args)
        {
            return args[0] is Symbol;
        }

        static object fn_vector_pred(List<object> args)
        {
            return args[0] is Object[];
        }

        static object fn_integer_pred(List<object> args)
        {
            return args[0] is Int64;
        }

        static object fn_real_pred(List<object> args)
        {
            return args[0] is Int64 || args[0] is Double;
        }

        static object fn_exact_pred(List<object> args)
        {
            return args[0] is Int64;
        }

        static object fn_inexact_pred(List<object> args)
        {
            return args[0] is Double;
        }

        static object fn_zero_pred(List<object> args)
        {
            object num = args[0];
            if (num is Int64) {
                return (Int64)num == 0;
            } else {
                return (Double)num == 0;
            }
        }

        static object fn_complex_pred(List<object> args)
        {
            return args[0] is Int64 || args[0] is Double;
        }

        static object fn_rational_pred(List<object> args)
        {
            return args[0] is Int64;
        }

        static List<object> pack(params object[] args)
        {
            List<object> rv = new List<object>();

            foreach (object o in args) {
                rv.Add(o);
            }

            return rv;
        }

        static object fn_equal_pred(List<object> args)
        {
            object a = args[0];
            object b = args[1];

            if (a == b) return true;

            if (a == null || b == null) return false;

            if (a.GetType() == b.GetType()) {
                if (a is Pair) {
                    return (bool)fn_equal_pred(pack(((Pair)a).head, ((Pair)b).head)) && (bool)fn_equal_pred(pack(((Pair)a).tail, ((Pair)b).tail));
                } else if (a is object[] || a is List<object>) {
                    IEnumerator<object> ai = ((IEnumerable<object>)a).GetEnumerator();
                    IEnumerator<object> bi = ((IEnumerable<object>)b).GetEnumerator();

                    int al, bl;

                    if (a is object[]) {
                        al = ((object[])a).Length;
                        bl = ((object[])b).Length;
                    } else {
                        al = ((List<object>)a).Count;
                        bl = ((List<object>)b).Count;
                    }

                    if (al == bl) {
                        while (ai.MoveNext() && bi.MoveNext()) {
                            if (!(bool)fn_eqv_pred(pack(ai.Current, bi.Current))) return false;
                        }

                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return fn_eqv_pred(args);
                }
            } else {
                return false;
            }
        }

        static object fn_list(List<object> args)
        {
            return args;
        }

        public StandardEnvironment()
        {
            Add(Symbol.get_symbol("eqv?"), fn_eqv_pred);
            Add(Symbol.get_symbol("eq?"), fn_eq_pred);
            Add(Symbol.get_symbol("equal?"), fn_equal_pred);
            Add(Symbol.get_symbol("="), fn_eq);
            Add(Symbol.get_symbol("+"), fn_add);
            Add(Symbol.get_symbol("-"), fn_sub);
            Add(Symbol.get_symbol("*"), fn_mul);
            Add(Symbol.get_symbol("/"), fn_div);
            Add(Symbol.get_symbol("boolean?"), fn_boolean_pred);
            Add(Symbol.get_symbol("char?"), fn_char_pred);
            Add(Symbol.get_symbol("complex?"), fn_complex_pred);
            Add(Symbol.get_symbol("number?"), fn_number_pred);
            Add(Symbol.get_symbol("rational?"), fn_rational_pred);
            Add(Symbol.get_symbol("null?"), fn_null_pred);
            Add(Symbol.get_symbol("pair?"), fn_pair_pred);
            Add(Symbol.get_symbol("list?"), fn_list_pred);
            Add(Symbol.get_symbol("procedure?"), fn_procedure_pred);
            Add(Symbol.get_symbol("string?"), fn_string_pred);
            Add(Symbol.get_symbol("symbol?"), fn_symbol_pred);
            Add(Symbol.get_symbol("vector?"), fn_vector_pred);
            Add(Symbol.get_symbol("real?"), fn_real_pred);
            Add(Symbol.get_symbol("integer?"), fn_integer_pred);
            Add(Symbol.get_symbol("inexact?"), fn_inexact_pred);
            Add(Symbol.get_symbol("exact?"), fn_exact_pred);
            Add(Symbol.get_symbol("zero?"), fn_zero_pred);
            Add(Symbol.get_symbol("list"), fn_list);
        }
    }
}