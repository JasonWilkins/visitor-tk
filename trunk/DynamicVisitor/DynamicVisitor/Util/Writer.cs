using System;
using System.Collections.Generic;
using System.Text;

namespace Util {
    public abstract class Writer {
        protected string m_indent_string;
        protected int m_indent_count;

        public Writer()
            : this("    ")
        { }

        public Writer(string indent_string)
        {
            m_indent_string = indent_string;
        }

        public abstract Writer Begin();
        public abstract Writer End();
        public abstract Writer Append(string s);
        public abstract Writer Append(char c);

        protected string make_indent()
        {
            return StringUtil.repeat(m_indent_string, m_indent_count);
        }

        public void Indent()
        {
            m_indent_count++;
        }

        public void Unindent()
        {
            if (m_indent_count <= 0) return;// throw new InvalidOperationException();

            m_indent_count--;
        }

        public Writer Join(List<string> value, string seperator)
        {
            return Join(value.ToArray(), seperator);
        }

        public Writer Join(List<string> value)
        {
            return Join(value.ToArray(), ", ");
        }

        public Writer Join(string[] value)
        {
            return Join(value, ", ");
        }

        public Writer Join(string[] value, string seperator)
        {
            if (value != null) Append(String.Join(seperator, value));
            return this;
        }

        public Writer EscapeChar(char input)
        {
            if ('\'' == input) {
                return Append("\\'");
            } else if ('"' == input) {
                return Append("\"");
            } else {
                return EscapeString(input.ToString());
            }
        }

        public Writer EscapeString(string input)
        {
            foreach (char c in input) {
                switch (c) {
                    case '"':
                        Append("\\\"");
                        break;

                    case '\\':
                        Append("\\\\");
                        break;

                    case '\0':
                        Append("\\0");
                        break;

                    case '\a':
                        Append("\\a");
                        break;

                    case '\b':
                        Append("\\b");
                        break;

                    case '\f':
                        Append("\\f");
                        break;

                    case '\n':
                        Append("\\n");
                        break;

                    case '\r':
                        Append("\\r");
                        break;

                    case '\t':
                        Append("\\t");
                        break;

                    case '\v':
                        Append("\\v");
                        break;

                    default:
                        Append(c);
                        break;
                }
            }

            return this;
        }
    }
}
