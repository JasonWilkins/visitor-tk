using System;
using System.Text.RegularExpressions;

using Util;

namespace Sexp {
    public class Number {
        static readonly Regex regex = init_regex();

        static Regex init_regex()
        {
            string radix = @"(\#(?<radix>{0}))";

            string radix2 = String.Format(radix, @"b");
            string radix8 = String.Format(radix, @"o");
            string radix10 = String.Format(radix, @"d") + '?';
            string radix16 = String.Format(radix, @"x");

            string exactness = @"(\#(?<exact>[ie]))?";

            string prefix = @"({0}{1}|{1}{0})";

            string prefix2 = String.Format(prefix, radix2, exactness);
            string prefix8 = String.Format(prefix, radix8, exactness);
            string prefix10 = String.Format(prefix, radix10, exactness);
            string prefix16 = String.Format(prefix, radix16, exactness);

            string digit2 = @"[01]";
            string digit8 = @"[0-7]";
            string digit10 = @"[0-9]";
            string digit16 = @"[0-9a-f]";

            string uinteger = @"({0}+)";

            string uinteger2 = String.Format(uinteger, digit2);
            string uinteger8 = String.Format(uinteger, digit8);
            string uinteger10 = String.Format(uinteger, digit10);
            string uinteger16 = String.Format(uinteger, digit16);

            string suffix = String.Format(@"
                (
                    (?<r_expmark>[esfdl])
                    (?<r_expsign>[\+\-])?
                    (?<r_exp>{0}+)
                )", digit10);

            string decimal10 = String.Format(@"
                (
                    (?<r_whole>{0}+)(?<r_whole_hash>\#*){1}|
                    \.(?<r_fract>{0}+)(?<r_fract_hash>\#*){1}?|
                    (?<r_whole>{0}+)\.(?<r_fract>{0}*)(?<r_fract_hash>\#*){1}?|
                    (?<r_whole>{0}+)(?<r_whole_hash>\#+)\.(?<r_fract_hash>\#*){1}?
                )", digit10, suffix);

            string ureal = @"
                (
                    (?<r_int>{0})(?<r_int_hash>\#*)|
                    (?<r_numer>{0})(?<r_numer_hash>\#*)/(?<r_denom>{0})(?<r_denom_hash>\#*)
                )";

            string ureal2 = String.Format(ureal, uinteger2);
            string ureal8 = String.Format(ureal, uinteger8);
            string ureal10 = '(' + String.Format(ureal, uinteger10) + '|' + decimal10 + ')';
            string ureal16 = String.Format(ureal, uinteger16);

            string real = @"((?<r_sign>[\+\-])?{0})";

            string real2 = String.Format(real, ureal2);
            string real8 = String.Format(real, ureal8);
            string real10 = String.Format(real, ureal10);
            string real16 = String.Format(real, ureal16);

            string isign = @"(?<i_sign>[\+\-])";

            string iureal2 = ureal2.Replace("<r_", "<i_");
            string iureal8 = ureal8.Replace("<r_", "<i_");
            string iureal10 = ureal10.Replace("<r_", "<i_");
            string iureal16 = ureal16.Replace("<r_", "<i_");

            string complex = @"
                (
                    {0}|
                    {0}(?<polar>@){2}{1}|
                    {0}{2}{1}i|
                    {0}{2}(?<i_int>i)|
                    {2}{1}i|
                    {2}(?<i_int>i)
                )";

            string complex2 = String.Format(complex, real2, iureal2, isign);
            string complex8 = String.Format(complex, real8, iureal8, isign);
            string complex10 = String.Format(complex, real10, iureal10, isign);
            string complex16 = String.Format(complex, real16, iureal16, isign);

            string num = @"(^{0}{1}$)";

            string num2 = String.Format(num, prefix2, complex2);
            string num8 = String.Format(num, prefix8, complex8);
            string num10 = String.Format(num, prefix10, complex10);
            string num16 = String.Format(num, prefix16, complex16);

            string number = String.Format("{0}|{1}|{2}|{3}", num2, num8, num10, num16);

            return new Regex(
                number,
                RegexOptions.IgnorePatternWhitespace|
                RegexOptions.IgnoreCase|
                RegexOptions.Compiled);
        }

        public static bool parse_number(string input, out object value)
        {
            Match m = regex.Match(input);

            if (!m.Success) {
                value = null;
                return false;
            }

            bool? is_exact = null;

            if (m.Groups["exact"].Length > 0) {
                if (m.Groups["exact"].Value == "e") {
                    is_exact = true;
                } else if (m.Groups["exact"].Value == "i") {
                    is_exact = false;
                }
            }

            int radix;

            if (m.Groups["radix"].Value == "b") {
                radix = 2;
            } else if (m.Groups["radix"].Value == "o") {
                radix = 8;
            } else if (m.Groups["radix"].Value == "x") {
                radix = 16;
            } else {
                radix = 10;
            }

            object real = parse_real(is_exact, radix, "r_", m.Groups);

            if (m.Groups["i_sign"].Length > 0) {
                object imaginary = parse_real(is_exact, radix, "i_", m.Groups);

                if (m.Groups["polar"].Length > 0) {
                    value = make_polar(real, imaginary);
                } else {
                    value = make_complex(real, imaginary);
                }
            } else {
                value = real;
            }

            return true;
        }

        static object parse_real(bool? is_exact, int radix, string prefix, GroupCollection g)
        {
            if (g[prefix+"int"].Length > 0) {
                return parse_fixed(is_exact, radix, prefix, g);
            } else if (g[prefix+"whole"].Length > 0 || g[prefix+"fract"].Length > 0) {
                return parse_float(is_exact, prefix, g);
            } else if (g[prefix+"numer"].Length > 0 && g[prefix+"denom"].Length > 0) {
                return parse_ratio(is_exact, radix, prefix, g);
            } else {
                return 0L;
            }
        }

        static object parse_fixed(bool? is_exact, int radix, string prefix, GroupCollection g)
        {
            string value = g[prefix+"int"].Value + StringUtil.repeat('0', g[prefix+"int_hash"].Length);

            if ("i" == value) {
                return 1L;
            } else {
                long rv = Convert.ToInt64(value, radix);

                if (g[prefix+"sign"].Value == "-") rv = -rv;

                if (is_exact ?? true) {
                    return rv;
                } else {
                    return Convert.ToDouble(rv);
                }
            }
        }

        static object parse_float(bool? is_exact, string prefix, GroupCollection g)
        {
            if (!is_exact ?? false) {
                string num = 
                    g[prefix+"sign"].Value + 
                    g[prefix+"whole"].Value + 
                    StringUtil.repeat('0', g[prefix+"whole_hash"].Length) +
                    '.' + 
                    StringUtil.repeat('0', g[prefix+"fract_hash"].Length) +
                    g[prefix+"fract"].Value;

                if (g[prefix+"exp"].Length > 0) {
                    num += 'e' + g[prefix+"expsign"].Value + g[prefix+"exp"].Value;
                }

                object rv;

                if (g[prefix+"expmark"].Length == 0 ||
                    g[prefix+"expmark"].Value.ToLower() == "d" ||
                    g[prefix+"expmark"].Value.ToLower() == "l") {

                    rv = Convert.ToDouble(num);
                } else {
                    rv = Convert.ToSingle(num);
                }

                return rv;
            } else {
                long len = g[prefix+"fract"].Length;
                long fract = len == 0 ? 0 : Convert.ToInt64(StringUtil.repeat('0', g[prefix+"fract_hash"].Length) + g[prefix+"fract"].Value);

                if (0 == fract) {
                    long n = Convert.ToInt64(
                        g[prefix+"sign"].Value + 
                        g[prefix+"whole"].Value +
                        StringUtil.repeat('0', g[prefix+"whole_hash"].Length));

                    if (g[prefix+"exp"].Length > 0) {
                        long exp = Convert.ToInt64(g[prefix+"exp"].Value);

                        if (g[prefix+"expsign"].Value == "-") {
                            return make_ratio(n, int_pow(10, exp));
                        } else {
                            return n * int_pow(10, exp);
                        }
                    }

                    return n;
                } else {
                    long numer = Convert.ToInt64(
                        g[prefix+"sign"].Value + 
                        g[prefix+"whole"].Value  +
                        StringUtil.repeat('0', g[prefix+"whole_hash"].Length)) + fract;

                    long denom = int_pow(10, len);
                    return make_ratio(numer, denom);
                }
            }
        }

        static long int_pow(long n, long exp)
        {
            long rv = 1;

            for (int i = 0; i < exp; i++) {
                rv = rv * n;
            }

            return rv;
        }

        static object parse_ratio(bool? is_exact, int radix, string prefix, GroupCollection g)
        {
            long numer = Convert.ToInt64(
                g[prefix+"sign"].Value + 
                g[prefix+"numer"].Value +
                StringUtil.repeat('0', g[prefix+"numer_hash"].Length), radix);

            long denom = Convert.ToInt64(
                g[prefix+"denom"].Value +
                StringUtil.repeat('0', g[prefix+"denom_hash"].Length), radix);

            return (is_exact ?? true) ?  make_ratio(numer, denom) : (double)numer / (double)denom;
        }

        static object make_complex(object real_part, object imaginary_part)
        {
            if (imaginary_part is long) {
                if ((long)imaginary_part == 0) return real_part;
            } else if (imaginary_part is float) {
                if ((float)imaginary_part == 0) return real_part;
            } else if (imaginary_part is double) {
                if ((double)imaginary_part == 0) return real_part;
            } else if (imaginary_part is Rational) {
                if (((Rational)imaginary_part).CompareTo(0) == 0) return real_part;
            }

            return new Complex(real_part, imaginary_part);
        }

        static object make_polar(object magnitude, object angle)
        {
            return Complex.from_polar(magnitude, angle);
        }

        static object make_ratio(long numer, long denom)
        {
            Rational rv = new Rational(numer, denom);

            if (rv.denom == 1) {
                return rv.numer;
            } else {
                return rv;
            }
        }
    }
}
