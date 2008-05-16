using System.Text;

namespace Util {
    public class TxtLocation {
        public string path = "";
        public int column = 1;
        public int line = 1;
        public string context = "";

        public TxtLocation()
        {  }

        public TxtLocation(string path, int column, int line, string context)
        {
            this.path = path;
            this.column = column;
            this.line = line;
            this.context = context;
        }

        public TxtLocation clone()
        {
            return new TxtLocation(path, column, line, context);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(path).Append("(").Append(line).Append(",").Append(column).Append(")");
            return sb.ToString();
        }
    }
}
