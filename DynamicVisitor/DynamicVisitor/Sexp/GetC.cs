using System;
using System.IO;
using Util;

namespace Sexp {
    public class Reader : IDisposable {
        TextReader m_reader;

        string m_line;
        int m_index;
        int m_peek;

        TxtLocation m_loc;
        public TxtLocation loc { get { return m_loc; } }

        int m_tabsize = 4;
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
            m_reader = new StreamReader(path);
            m_loc = new TxtLocation();
            m_loc.path = path;

            init_read();
        }

        void init_read()
        {
            m_line = m_reader.ReadLine();
            m_index = 0;
            m_peek = read();
        }

        int read()
        {
            if (null == m_line) return -1;

            if (m_index >= m_line.Length) {
                string raw = m_reader.ReadLine();

                if (null == raw) {
                    m_loc.context = null;
                    m_line = null;
                    return -1;
                }

                m_loc.context = raw.Replace("\t", m_tab);
                m_line = raw + System.Environment.NewLine;
                m_index = 0;
            }

            return m_line[m_index++];
        }

        public int getc()
        {
            int cin = m_peek;

            m_peek = read();

            if (cin != -1) {
                m_loc.column++;

                if ('\n' == cin) {
                    m_loc.line++;
                    m_loc.column = 0;
                } else if ('\t' == cin) {
                    m_loc.column += m_tabsize;
                }
            }

            return cin;
        }

        public int peek()
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
