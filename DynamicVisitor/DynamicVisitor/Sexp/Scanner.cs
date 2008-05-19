using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Util;

namespace Sexp {
    public class Scanner {
        Reader m_reader;

        char cin;
        char peek;
        bool is_eof = false;

        public Scanner(Reader reader)
        {
            m_reader = reader;
        }

        char getc()
        {
            int new_cin = m_reader.getc();
            int new_peek = m_reader.peek();

            if (new_peek != -1) {
                peek = (char)new_peek;
            } else {
                peek = '\0';
            }

            if (new_cin != -1) {
                cin = (char)new_cin;
            } else {
                is_eof = true;
                cin = '\0';
            }

            return cin;
        }

        bool is_whitespace(char cin)
        {
            return char.IsWhiteSpace(cin);
        }

        bool is_delimiter(char cin)
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

        bool is_reserved_delimiter(char cin)
        {
            return
                '[' == cin ||
                ']' == cin ||
                '{' == cin ||
                '}' == cin;
        }

        bool is_letter(char cin)
        {
            return char.IsLetter(cin);
        }

        bool parse_number(string num, out object value)
        {
            long result1;
            double result2;

            if ('#' == num[0]) {
                if ('x' == num[1] || 'X' == num[1]) {
                    if (Int64.TryParse(num.Substring(2), NumberStyles.HexNumber, null, out result1)) {
                        value = new Int64();
                        value = result1;
                        return true;
                    } else {
                        value = null;
                        return false;
                    }
                } else {
                    value = null;
                    return false;
                }
            } else {
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
        }

        void line_comment()
        {
            if (';' == cin) {
                do {
                    getc();

                    if (is_eof) return;

                } while (cin != '\n');

                getc(); // always advance to the character after the comment
            }
        }

        bool nested_comment()
        {
            if ('#' == cin && '|' == peek) {
                getc();

                do {
                    getc();

                    if (is_eof || !nested_comment()) return false;

                } while (!('|' == cin && '#' == peek));

                getc();
                getc(); // always advance to the character after the comment
            }

            return true;
        }

        void slurp(StringBuilder sb)
        {
            while (!is_eof && !is_delimiter(peek)) {
                sb.Append(getc());
            }
        }

        public Attributes read()
        {
            getc();

            Attributes attrib = new Attributes();
            attrib.loc = m_reader.loc.clone();

            while (';' == cin || ('#' == cin && '|' == peek) || is_whitespace(cin)) {
                line_comment();

                if (!nested_comment()) {
                    attrib.error = "EOF in comment";
                    attrib.token = Token.ERROR;
                }

                if (is_whitespace(cin)) {
                    getc();
                }
            }

            attrib.loc.copy(m_reader.loc);

            if (is_eof) {
                attrib.token = Token.EOF;
            } else if ('(' == cin) {
                attrib.literal = "(";
                attrib.token = Token.OPEN_PAREN;
            } else if (')' == cin) {
                attrib.literal = ")";
                attrib.token = Token.CLOSE_PAREN;
            } else if ('\'' == cin) {
                attrib.literal = "'";
                attrib.token = Token.SINGLE_QUOTE;
            } else if ('`' == cin) {
                attrib.literal = "`";
                attrib.token = Token.BACKQUOTE;
            } else if ('"' == cin) {
                StringBuilder sb = new StringBuilder();
                sb.Append(cin);

                while (peek != '"') {
                    if (is_eof) {
                        attrib.error = "EOF in string literal";
                        attrib.token = Token.ERROR;
                    }

                    if ('\\' == peek) {
                        getc();
                        sb.Append(cin);

                        if (peek != '\\' && peek != '"') {
                            attrib.error = "invalid escape sequence in string literal";
                        }
                    }

                    getc();
                    sb.Append(cin);
                }

                getc();
                sb.Append(cin);

                attrib.literal = sb.ToString();
                attrib.token = Token.STRING;
            } else if ('#' == cin) {
                StringBuilder sb = new StringBuilder();
                sb.Append(cin);

                if ('t' == peek || 'f' == peek) {
                    getc();
                    sb.Append(cin);
                    attrib.token = Token.BOOL;
                } else if ('\\' == peek) {
                    getc();
                    sb.Append(cin);

                    if (is_letter(peek)) {
                        slurp(sb);
                    } else {
                        getc();
                        sb.Append(cin);
                    }

                    attrib.token = Token.CHAR;
                } else if ('(' == peek) {
                    getc();
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
                    getc();
                    sb.Append(cin);
                    attrib.token = Token.EXTERNAL;
                } else if ('@' == peek) {
                    slurp(sb);
                    attrib.token = Token.HASH_NUMBER;
                } else if ('=' == peek) {
                    getc();
                    sb.Append(cin);
                    attrib.token = Token.CIRCULAR_INPUT;
                } else if ('#' == peek) {
                    getc();
                    sb.Append(cin);
                    attrib.token = Token.CIRCULAR_OUTPUT;
                } else {
                    attrib.error = "invalid # sequence";
                    attrib.token = Token.ERROR;
                }

                attrib.literal = sb.ToString();
            } else if (',' == cin) {
                if ('@' == peek) {
                    getc();
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
                attrib.error = "reserved token";
                attrib.token = Token.ERROR;
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
                        attrib.error = "invalid named character";
                        attrib.token = Token.ERROR;
                    }
                }
            } else if (Token.NUM == attrib.token) {
                object value;
                char type = attrib.literal[1];

                if (!('x' == type || 'X' == type)) {
                    attrib.error = "unimplemented number literal";
                    attrib.token = Token.ERROR;
                } else if (parse_number(attrib.literal, out value)) {
                    attrib.value = value;
                } else {
                    attrib.error = "invalid number literal";
                    attrib.token = Token.ERROR;
                }
            } else if (Token.NAMED_CONSTANT == attrib.token) {
                if ("#!optional" == attrib.literal) {
                    attrib.value = "optional";
                } else if ("#!rest" == attrib.literal) {
                    attrib.value = "rest";
                } else {
                    attrib.error = "invalid named constant";
                    attrib.token = Token.ERROR;
                }
            } else if (Token.BIT_STRING == attrib.token) {
                attrib.error = "bit strings not implemented";
                attrib.token = Token.ERROR;
            } else if (Token.HASH_NUMBER == attrib.token) {
                attrib.error = "hash numbers not implemented";
                attrib.token = Token.ERROR;
            } else if (Token.ID == attrib.token) {
                object value;
                if (attrib.literal == ".") {
                    attrib.value = attrib.literal;
                    attrib.token = Token.DOT;
                } else if (parse_number(attrib.literal, out value)) {
                    attrib.value = value;
                    attrib.token = Token.NUM;
                } else {
                    attrib.value = Symbol.get_symbol(attrib.literal);
                }
            } else if (Token.ERROR == attrib.token) {
                // leave it alone
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
