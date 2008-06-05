using System;

namespace Util {
    public class TxtException : Exception {
        public TxtLocation loc { get { return (TxtLocation)Data["loc"]; } }

        public TxtException(TxtLocation loc, string message, Exception innerException)
            : base(message, innerException)
        { }

        public TxtException(TxtLocation loc, string message)
            : base(message)
        {
            Data.Add("loc", loc);
        }
    }

    [Serializable]
    public class TxtLocation {
        string m_path;
        public string path { get { return m_path; } set { m_path = value ?? ""; } }

        public int column;
        public int line;

        string m_context;
        public string context { get { return m_context; } set { m_context = value ?? ""; } }

        public TxtLocation(string path)
        {
            m_path    = path;
            column    = 0;
            line      = 0;
            m_context = "";
        }

        public TxtLocation(string path, int column, int line, string context)
        {
            this.path    = path;
            this.column  = column;
            this.line    = line;
            this.context = context;
        }

        public void copy(TxtLocation loc)
        {
            m_path    = loc.m_path;
            column    = loc.column;
            line      = loc.line;
            m_context = loc.m_context;
        }

        public TxtLocation clone()
        {
            return new TxtLocation(path, column, line, context);
        }

        public string PathPoint()
        {
            return String.Format("{0}({1},{2})", path, line, column);
        }

        public string FancyContext(int width)
        {
            int half = width/2;

            int start = half < column ? column-half : 0;
            int arrow = start > 0 ? column-start : column;

            if (start+width > context.Length) {
                width = context.Length-start;
            }

            string snippet = context.Substring(start, width);

            return String.Format("{0}\n{1}\n", snippet, "^".PadLeft(arrow, '_'));
        }

        public string FancyContext() { return FancyContext(78); }

        public override string ToString() { return PathPoint(); }
    }
}
