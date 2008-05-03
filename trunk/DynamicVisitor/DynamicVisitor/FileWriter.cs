using System;
using System.IO;
using System.Text;

namespace Writer {
    public class FileWriter : Writer, IDisposable {
        private StreamWriter m_sw;

        public FileWriter(string path)
            : this(path, false)
        { }

        public FileWriter(string path, bool append)
        {
            m_sw = new StreamWriter(path, append);
        }

        public override Writer Begin()
        {
            for (int i = 0; i < m_indent_count; i++) {
                m_sw.Write(m_indent_string);
            }

            return this;
        }

        public override Writer End()
        {
            m_sw.WriteLine();

            return this;
        }

        public override Writer Append(string s)
        {
            m_sw.Write(s);

            return this;
        }

        public override Writer Append(char c)
        {
            m_sw.Write(c);

            return this;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            m_sw.Close();
        }

        #endregion
    }
}
