using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using Util;
using Symbols;

namespace Sexp {
    public class LexicalError : TxtException {
        public char cin { get { return (char)Data["cin"]; } }
        public char peek { get { return (char)Data["peek"]; } }
        public bool is_eof { get { return (bool)Data["is_eof"]; } }

        public LexicalError(TxtLocation loc, char cin, char peek, string message)
            : base(loc, message)
        {
            Data.Add("cin", cin);
            Data.Add("peek", peek);
            Data.Add("is_eof", '\0' == cin);
        }
    }

    public class Scanner {
        readonly Reader m_reader;

        char cin;
        char peek;

        public Scanner(Reader reader)
        {
            m_reader = reader;
        }

        void next()
        {
            int next_cin = m_reader.GetChar();
            int next_peek = m_reader.Peek();

            cin = next_cin > 0 ? (char)next_cin : '\0';
            peek = next_peek > 0 ? (char)next_peek : '\0';
        }

        LexicalError lexical_error(string message)
        {
            return new LexicalError(m_reader.loc, cin, peek, message);
        }

        static bool is_whitespace(char cin)
        {
            return char.IsWhiteSpace(cin);
        }

        static bool is_delimiter(char cin)
        {
            return
                is_whitespace(cin) ||
                '(' == cin ||
                ')' == cin ||
                '"' == cin ||
                '\'' == cin ||
                '`' == cin ||
                '|' == cin ||
                ';' == cin ||
                is_reserved_delimiter(cin);
        }

        static bool is_reserved_delimiter(char cin)
        {
            return
                '[' == cin ||
                ']' == cin ||
                '{' == cin ||
                '}' == cin;
        }

        static bool is_letter(char cin)
        {
            return char.IsLetter(cin);
        }

        static int? get_radix(char prefix)
        {
            if ('x' == prefix || 'X' == prefix) {
                return 16;
            } else if ('d' == prefix || 'D' == prefix) {
                return 10;
            } else if ('o' == prefix || 'O' == prefix) {
                return 8;
            } else if ('b' == prefix || 'B' == prefix) {
                return 2;
            } else {
                return null;
            }
        }

        static bool? get_exact(char prefix)
        {
            if ('e' == prefix || 'E' == prefix) {
                return true;
            } else if ('i' == prefix || 'I' == prefix) {
                return false;
            } else {
                return null;
            }
        }

        static char? get_precision(char prefix)
        {
            if ('s' == prefix || 'S' == prefix) {
                return 's';
            } else if ('f' == prefix || 'F' == prefix) {
                return 'f';
            } else if ('d' == prefix || 'D' == prefix) {
                return 'd';
            } else if ('l' == prefix || 'L' == prefix) {
                return 'l';
            } else {
                return null;
            }
        }

        static bool parse_hash_number(string num, out object result)
        {
            foo(num, out result);

            int? radix = get_radix(num[1]);
            bool? is_exact = get_exact(num[1]);

            string num_part;

            if ('#' == num[2]) {
                if (radix.HasValue) {
                    is_exact = get_exact(num[3]);
                } else {
                    radix = get_radix(num[3]);
                }

                num_part = num.Substring(4);
            } else {
                num_part = num.Substring(2);
            }

            if (is_exact.HasValue) {
                if (is_exact.Value) {
                    if (!radix.HasValue || 10 == radix.Value) {
                        return parse_fixnum(num_part, out result);
                    } else {
                        return parse_hex(num_part, radix.Value, out result);
                    }
                } else {
                    if (!radix.HasValue || 10 == radix) {
                        return parse_flonum(num_part, out result);
                    } else {
                        object rv;

                        if (parse_hex(num_part, radix.Value, out rv)) {
                            result = Convert.ToDouble(rv);
                            return true;
                        } else {
                            result = null;
                            return false;
                        }
                    }
                }
            } else if (radix.HasValue) {
                if (10 == radix.Value) {
                    return try_parse_number(num_part, out result);
                } else {
                    return parse_hex(num_part, radix.Value, out result);
                }
            } else {
                result = null;
                return false;
            }
        }

        static bool parse_fixnum(string num, out object result)
        {
            long rv;
            if (Int64.TryParse(num, NumberStyles.Integer|NumberStyles.AllowDecimalPoint, null, out rv)) {
                result = rv;
                return true;
            } else {
                result = null;
                return false;
            }
        }

        static bool parse_hex(string num, int radix, out object result)
        {
            try {
                if ('-' == num[0]) {
                    result = -Convert.ToInt64(num.Substring(1), radix);
                } else {
                    result = Convert.ToInt64(num, radix);
                }

                return true;
            } catch {
                result = null;
                return false;
            }
        }

        static bool parse_flonum(string num, out object result)
        {
            double rv;
            if (Double.TryParse(num, NumberStyles.Float, null, out rv)) {
                result = rv;
                return true;
            } else {
                result = null;
                return false;
            }
        }

        static bool try_parse_number2(string num, out object value)
        {
            foo(num, out value);
            return try_parse_number(num, out value);
        }

        static bool try_parse_number(string num, out object value)
        {
            long result1;
            double result2;

            if (Int64.TryParse(num, NumberStyles.Integer|NumberStyles.AllowDecimalPoint, null, out result1)) {
                value = result1;
                return true;
            } else if (Double.TryParse(num, NumberStyles.Float, null, out result2)) {
                value = result2;
                return true;
            } else {
                value = null;
                return false;
            }
        }

        static void foo(string input, out object value)
        {
            value = 0;

            string radix = @"
                (
                    \#
                    (?<radix>{0})
                )";

            string radix2 = String.Format(radix, @"b");
            string radix8 = String.Format(radix, @"o");
            string radix10 = String.Format(radix, @"d") + '?';
            string radix16 = String.Format(radix, @"x");

            string exactness = @"
                (
                    \#
                    (?<exact>[ie])
                )?";

            string prefix = @"
                (
                    {0}{1}|
                    {1}{0}
                )";

            string prefix2 = String.Format(prefix, radix2, exactness);
            string prefix8 = String.Format(prefix, radix8, exactness);
            string prefix10 = String.Format(prefix, radix10, exactness);
            string prefix16 = String.Format(prefix, radix16, exactness);

            string digit2 = @"[01]";
            string digit8 = @"[0-7]";
            string digit10 = @"[0-9]";
            string digit16 = @"[0-9a-f]";

            string uinteger = @"
                (
                    {0}+
                    \#*
                )";

            string uinteger2 = String.Format(uinteger, digit2);
            string uinteger8 = String.Format(uinteger, digit8);
            string uinteger10 = String.Format(uinteger, digit10);
            string uinteger16 = String.Format(uinteger, digit16);

            string suffix = String.Format(@"
                (
                    (?<expmark>[esfdl])
                    (?<expsign>[\+\-])?
                    (?<exp>{0}+)
                )", digit10);

            string decimal10 = String.Format(@"
                (
                    (?<whole>{0}+)\#*{1}|
                    \.(?<fract>{0}+)\#*{1}?|
                    (?<whole>{0}+)\.(?<fract>{0}*)\#*{1}?|
                    (?<whole>{0}+)\#+\.\#*{1}?
                )", digit10, suffix);

            string ureal = @"
                (?<int>
                    {0})|
                    (?<numer>{0})/(?<denom>{0}
                )";

            string ureal2 = String.Format(ureal, uinteger2);
            string ureal8 = String.Format(ureal, uinteger8);
            string ureal10 = '(' + String.Format(ureal, uinteger10) + '|' + decimal10 + ')';
            string ureal16 = String.Format(ureal, uinteger16);

            string real = @"((?<sign>[\+\-])?{0})";

            string real2 = String.Format(real, ureal2);
            string real8 = String.Format(real, ureal8);
            string real10 = String.Format(real, ureal10);
            string real16 = String.Format(real, ureal16);

            string isign = @"(?<i_sign>[\+\-])";

            string iureal2 = real2.Replace("<", "<i_");
            string iureal8 = real8.Replace("<", "<i_");
            string iureal10 = real10.Replace("<", "<i_");
            string iureal16 = real16.Replace("<", "<i_");

            string complex = @"
                (
                    {0}|
                    {0}(?<polar>@){2}?{1}|
                    {0}{2}{1}i|
                    {0}{2}i|
                    {2}{1}i|
                    {2}(?<i_int>i)|
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

            Regex ex = new Regex(number, RegexOptions.IgnorePatternWhitespace|RegexOptions.IgnoreCase);

            Match m = ex.Match(input);

            bool? exact2 = null;

            if (m.Groups["exact"].Length > 0) {
                if (m.Groups["exact"].Value == "e") {
                    exact2 = true;
                } else if (m.Groups["exact"].Value == "i") {
                    exact2 = false;
                }
            }

            int radix22;

            if (m.Groups["radix"].Length > 0) {
                if (m.Groups["radix"].Value == "b") {
                    radix22 = 2;
                } else if (m.Groups["radix"].Value == "o") {
                    radix22 = 8;
                } else if (m.Groups["radix"].Value == "x") {
                    radix22 = 16;
                } else {
                    radix22 = 10;
                }
            }

            bool is_negative = (m.Groups["sign"].Value == "-");

            if (m.Groups["int"].Length > 0) {
            } else if (m.Groups["whole"].Length > 0 || m.Groups["fract"].Length > 0) {
            } else if (m.Groups["numer"].Length > 0 && m.Groups["denom"].Length > 0) {
            } else {
                throw new Exception();
            }

            //if (m.Success) {
            //    Console.WriteLine("exact={0}", m.Groups["exact"]);
            //    Console.WriteLine("radix={0}", m.Groups["radix"]);

            //    Console.WriteLine("sign={0}", m.Groups["sign"]);
            //    Console.WriteLine("int={0}", m.Groups["int"]);
            //    Console.WriteLine("whole={0}", m.Groups["whole"]);
            //    Console.WriteLine("fract={0}", m.Groups["fract"]);
            //    Console.WriteLine("expmark={0}", m.Groups["expmark"]);
            //    Console.WriteLine("expsign={0}", m.Groups["expsign"]);
            //    Console.WriteLine("exp={0}", m.Groups["exp"]);
            //    Console.WriteLine("numer={0}", m.Groups["numer"]);
            //    Console.WriteLine("denum={0}", m.Groups["denom"]);

            //    Console.WriteLine("i_sign={0}", m.Groups["i_sign"]);
            //    Console.WriteLine("i_int={0}", m.Groups["i_int"]);
            //    Console.WriteLine("i_whole={0}", m.Groups["i_whole"]);
            //    Console.WriteLine("i_fract={0}", m.Groups["i_fract"]);
            //    Console.WriteLine("i_expmark={0}", m.Groups["i_expmark"]);
            //    Console.WriteLine("i_expsign={0}", m.Groups["i_expsign"]);
            //    Console.WriteLine("i_exp={0}", m.Groups["i_exp"]);
            //    Console.WriteLine("i_numer={0}", m.Groups["i_numer"]);
            //    Console.WriteLine("i_denum={0}", m.Groups["i_denom"]);
            //} else {
            //    throw new Exception();
            //    //Console.WriteLine("symbol={0}", input);
            //}
        }

        void line_comment()
        {
            if (';' == cin) {
                do {
                    next();

                    if ('\0' == cin) return;

                } while (cin != '\n');

                next(); // always advance to the character after the comment
            }
        }

        bool nested_comment()
        {
            if ('#' == cin && '|' == peek) {
                next();

                do {
                    next();

                    if ('\0' == cin || !nested_comment()) return false;

                } while (!('|' == cin && '#' == peek));

                next();
                next(); // always advance to the character after the comment
            }

            return true;
        }

        void slurp(StringBuilder sb)
        {
            while (peek != '\0' && !is_delimiter(peek)) {
                next();
                sb.Append(cin);
            }
        }

        public Attributes read()
        {
            next();

            Attributes attrib = new Attributes();
            attrib.loc = m_reader.loc.clone();

            while (';' == cin || ('#' == cin && '|' == peek) || is_whitespace(cin)) {
                line_comment();

                if (!nested_comment()) {
                    throw lexical_error("<eof> in comment");
                }

                if (is_whitespace(cin)) {
                    next();
                }
            }

            attrib.loc.copy(m_reader.loc);

            if ('\0' == cin) {
                attrib.token = Token.EOF;
            } else if ('(' == cin) {
                attrib.literal = "(";
                attrib.token = Token.OPEN_PAREN;
            } else if (')' == cin) {
                attrib.literal = ")";
                attrib.token = Token.CLOSE_PAREN;
            } else if ('\'' == cin) {
                attrib.literal = "'";
                attrib.token = Token.QUOTE;
            } else if ('`' == cin) {
                attrib.literal = "`";
                attrib.token = Token.BACKQUOTE;
            } else if ('"' == cin) {
                StringBuilder sb = new StringBuilder();
                sb.Append(cin);

                while (peek != '"') {
                    if ('\0' == cin) throw lexical_error("<eof> in string literal");

                    if ('\\' == peek) {
                        next();
                        sb.Append(cin);
                    }

                    next();
                    sb.Append(cin);
                }

                next();
                sb.Append(cin);

                attrib.literal = sb.ToString();
                attrib.token = Token.STRING;
            } else if ('#' == cin) {
                StringBuilder sb = new StringBuilder();
                sb.Append(cin);

                if ('t' == peek || 'f' == peek) {
                    next();
                    sb.Append(cin);
                    attrib.token = Token.BOOL;
                } else if ('\\' == peek) {
                    next();
                    sb.Append(cin);

                    if (is_letter(peek)) {
                        slurp(sb);
                    } else {
                        next();
                        sb.Append(cin);
                    }

                    attrib.token = Token.CHAR;
                } else if ('(' == peek) {
                    next();
                    sb.Append(cin);
                    attrib.token = Token.VECTOR;
                } else if (
                    'b' == peek || 'B' == peek ||
                    'e' == peek || 'E' == peek ||
                    'i' == peek || 'I' == peek ||
                    'd' == peek || 'D' == peek ||
                    'o' == peek || 'O' == peek ||
                    'x' == peek || 'X' == peek) {

                    slurp(sb);
                    attrib.token = Token.NUM;
                } else if ('!' == peek) {
                    slurp(sb);
                    attrib.token = Token.NAMED_CONSTANT;
                } else if ('*' == peek) {
                    slurp(sb);
                    attrib.token = Token.BIT_STRING;
                } else if ('[' == peek) {
                    next();
                    sb.Append(cin);
                    attrib.token = Token.EXTERNAL;
                } else if ('@' == peek) {
                    slurp(sb);
                    attrib.token = Token.HASH_NUMBER;
                } else if ('=' == peek) {
                    next();
                    sb.Append(cin);
                    attrib.token = Token.CIRCULAR_INPUT;
                } else if ('#' == peek) {
                    next();
                    sb.Append(cin);
                    attrib.token = Token.CIRCULAR_OUTPUT;
                } else {
                    throw lexical_error("invalid # sequence");
                }

                attrib.literal = sb.ToString();
            } else if (',' == cin) {
                if ('@' == peek) {
                    next();
                    attrib.literal = ",@";
                    attrib.token = Token.SPLICE;
                } else {
                    attrib.literal = ",";
                    attrib.token = Token.COMMA;
                }
            } else if (']' == cin) {
                attrib.literal = "]";
                attrib.token = Token.CLOSE_SQUARE;
            } else if (
                '|' == cin ||
                '[' == cin ||
                '{' == cin ||
                '}' == cin) {

                attrib.literal = char.ToString(cin);
                throw lexical_error("reserved token");
            } else {
                StringBuilder sb = new StringBuilder();
                sb.Append(cin);
                slurp(sb);
                attrib.literal = sb.ToString();
                attrib.token = Token.ID;
            }

            return attrib;
        }

        public Attributes interpret(Attributes attrib)
        {
            if (Token.STRING == attrib.token) {
                StringBuilder sb = new StringBuilder();

                int i = 1;
                while (i < attrib.literal.Length-1) {
                    if ('\\' == attrib.literal[i]) {
                        if ('"' == attrib.literal[i+1] || '\\' == attrib.literal[i+1]) {
                            i++;
                        }
                    }

                    sb.Append(attrib.literal[i]);
                    i++;
                }

                attrib.value = sb.ToString();
            } else if (Token.BOOL == attrib.token) {
                if ("#t" == attrib.literal) {
                    attrib.value = true;
                } else {
                    attrib.value = false;
                }
            } else if (Token.CHAR == attrib.token) {
                string value = attrib.literal.Substring(2);

                if (1 == value.Length) {
                    attrib.value = value[0];
                } else {
                    if (value.Equals("space", StringComparison.OrdinalIgnoreCase)) {
                        attrib.value = ' ';
                    } else if (value.Equals("newline", StringComparison.OrdinalIgnoreCase)) {
                        attrib.value = '\n';
                    } else {
                        throw lexical_error("invalid named character");
                    }
                }
            } else if (Token.NUM == attrib.token) {
                if (!parse_hash_number(attrib.literal, out attrib.value)) { attrib.value = 0L; }// throw lexical_error(String.Format("invalid number literal '{0}'", attrib.literal));
            } else if (Token.NAMED_CONSTANT == attrib.token) {
                if ("#!optional" == attrib.literal) {
                    attrib.value = "optional";
                } else if ("#!rest" == attrib.literal) {
                    attrib.value = "rest";
                } else {
                    throw lexical_error("invalid named constant");
                }
            } else if (Token.BIT_STRING == attrib.token) {
                throw lexical_error("bit strings not implemented");
            } else if (Token.HASH_NUMBER == attrib.token) {
                throw lexical_error("hash numbers not implemented");
            } else if (Token.ID == attrib.token) {
                object value;
                if (attrib.literal == ".") {
                    attrib.value = attrib.literal;
                    attrib.token = Token.DOT;
                } else if (try_parse_number2(attrib.literal, out value)) {
                    attrib.value = value;
                    attrib.token = Token.NUM;
                } else {
                    attrib.value = Symbol.get_symbol(attrib.literal);
                }
            } else {
                attrib.value = attrib.literal;
            }

            return attrib;
        }

        public Attributes scan()
        {
            return interpret(read());
        }
    }
}
