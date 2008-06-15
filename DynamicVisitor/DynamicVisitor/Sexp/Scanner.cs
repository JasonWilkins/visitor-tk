using System;
using System.Text;

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
                if (!Number.parse_number(attrib.literal, out attrib.value)) {
                    throw lexical_error(String.Format("invalid number literal '{0}'", attrib.literal));
                }
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
                } else if (Number.parse_number(attrib.literal, out value)) {
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
