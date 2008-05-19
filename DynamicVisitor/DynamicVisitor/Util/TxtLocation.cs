using System.Text;

namespace Util {
    public class TxtLocation {
        public string path;
        public int column;
        public int line;
        public string context;

        public TxtLocation()
        {
            this.path = "";
            this.column = 0;
            this.line = 1;
            this.context = "";
        }

        public TxtLocation(string path, int column, int line, string context)
        {
            this.path = path;
            this.column = column;
            this.line = line;
            this.context = context;
        }

        public void copy(TxtLocation loc)
        {
            path = loc.path;
            column = loc.column;
            line = loc.line;
            context = loc.context;
        }

        public TxtLocation clone()
        {
            return new TxtLocation(path, column, line, context);
        }

        public string PathPoint()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(path).Append("(").Append(line).Append(",").Append(column).Append(")");
            return sb.ToString();
        }

        public override string ToString()
        {
            return PathPoint();
        }
    }
}
