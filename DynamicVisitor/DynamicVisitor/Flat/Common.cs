using System;
using System.Collections.Generic;
using System.Text;

namespace Flat {
    public static class CommonTypes {
        public static Type INT32 = new Type("int32");
        public static Type INT64 = new Type("int64");
        public static Type SINGLE = new Type("single");
        public static Type DOUBLE = new Type("double");

        internal static Type[] TYPE_LIST = {
            INT32,
            INT64,
            SINGLE,
            DOUBLE,
        };
    }

    public static class CommonOperators {
        internal static Operator[] OPERATOR_LIST = {
        };
    }

}
