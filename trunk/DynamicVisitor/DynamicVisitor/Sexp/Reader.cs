using System;
using System.IO;

using Util;

namespace Sexp {
    public class Reader : IDisposable {
        readonly TextReader m_reader;

        readonly TxtLocation m_loc;
        public TxtLocation loc { get { return m_loc; } }

        string m_line;
        int m_index;
        int m_peek;

        int m_tabsize;
        string m_tab;

        public Reader(string path)
            : this(path, 4)
        { }

        public Reader(string path, int tabsize)
        {
            m_reader = new StreamReader(path);
            m_loc = new TxtLocation(path);

            m_tabsize = tabsize;
            m_tab = StringUtil.repeat(' ', tabsize);

            next();
            m_peek = read();
        }

        bool next()
        {
            do {
                m_line = m_reader.ReadLine();

                if (m_line == null) {
                    m_loc.column++;
                    return false;
                } else {
                    m_loc.line++;
                }
            } while (m_line.Length == 0);

            m_loc.column = 0;
            m_loc.context = m_line.Replace("\t", m_tab);

            if (m_reader.Peek() != -1) m_line += System.Environment.NewLine;

            m_index = 0;

            return true;
        }

        int read()
        {
            if (m_line != null && (m_index < m_line.Length || next())) {
                return m_line[m_index++];
            } else {
                return -1;
            }
        }

        public int GetChar()
        {
            int c = m_peek;

            m_peek = read();

            if (c != -1) {
                if (c != '\n' && c != '\r') {
                    m_loc.column++;
                } else if ('\t' == c) {
                    m_loc.column += m_tabsize;
                }
            }

            return c;
        }

        public int Peek()
        {
            return m_peek;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            m_reader.Close();
        }

        #endregion
    }
}
