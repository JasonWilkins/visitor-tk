using System;
using System.Collections.Generic;
using System.Text;

namespace Writer
{
    public class StringWriter : Writer
    {
        private StringBuilder m_sb;

        public StringWriter()
            : this(new StringBuilder())
        { }

        public StringWriter(StringBuilder sb)
        {
            m_sb = sb;
        }

        public override Writer Begin()
        {
            for (int i = 0; i < m_indent_count; i++)
            {
                m_sb.Append(m_indent_string);
            }

            return this;
        }

        public override Writer End()
        {
            m_sb.AppendLine();

            return this;
        }

        public override Writer Append(string s)
        {
            m_sb.Append(s);

            return this;
        }

        public override Writer Append(char c)
        {
            m_sb.Append(c);

            return this;
        }

        public override string ToString()
        {
            return m_sb.ToString();
        }
    }
}
