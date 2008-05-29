using System;
using System.IO;

using Util;

namespace Sexp {
    public class Reader : IDisposable {
        readonly TextReader m_reader;

        string m_line;
        int m_index;
        int m_peek;

        readonly TxtLocation m_loc;
        public TxtLocation loc { get { return m_loc; } }

        int m_tabsize;
        string m_tab;

        public int tabsize
        {
            get { return m_tabsize; }

            set
            {
                m_tabsize = value;
                m_tab = StringUtil.repeat(' ', m_tabsize);
            }
        }

        public Reader(string path)
        {
            tabsize = 4;

            m_reader = new StreamReader(path);
            m_loc = new TxtLocation(path);

            init();
        }

        void init()
        {
            next();
            m_peek = read();
        }

        bool next()
        {
            string raw = m_reader.ReadLine();

            if (null == raw) {
                m_line = null;
                return false;
            } else {
                m_line = raw;

                m_loc.context = raw.Replace("\t", m_tab);

                if (m_reader.Peek() != -1) m_line += System.Environment.NewLine;

                m_index = 0;

                return true;
            }
        }

        int read()
        {
            if (null == m_line) return -1;

            if (m_index >= m_line.Length && !next()) return -1;

            return m_line[m_index++];
        }

        public int GetChar()
        {
            int cin = m_peek;

            m_peek = read();

            m_loc.column++;

            if (cin != -1) {
                if ('\n' == cin) {
                    m_loc.line++;
                    m_loc.column = 0;
                } else if ('\t' == cin) {
                    m_loc.column += m_tabsize;
                }
            }

            return cin;
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
