using System.Text;

namespace Util {
    public static class StringUtil {
        public static string repeat(string str, int count)
        {
            StringBuilder sb = new StringBuilder(count);

            for (int i = 0; i < count; i++) {
                sb.Append(str);
            }

            return sb.ToString();
        }

        public static string repeat(char ch, int count)
        {
            StringBuilder sb = new StringBuilder(count);

            for (int i = 0; i < count; i++) {
                sb.Append(ch);
            }

            return sb.ToString();
        }

        public static string space(int count)
        {
            return repeat(' ', count);
        }
    }
}