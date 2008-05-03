using System;
using System.Diagnostics;

namespace Utils {
    public static class Trace {
        [Conditional("WARNING")]
        public static void Warning(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

        [Conditional("VERBOSE")]
        public static void Verbose(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }
    }
}
