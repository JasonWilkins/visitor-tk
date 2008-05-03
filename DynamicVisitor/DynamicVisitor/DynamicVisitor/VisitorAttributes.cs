using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicVisitor {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    class VisitType : System.Attribute {
        public bool Inline;
        public bool Terminal;

        public VisitType()
        {
            Inline = false;
            Terminal = false;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    class VisitDefault : System.Attribute {
        Type Default;

        public VisitDefault(Type Default)
        {
            this.Default = Default;
        }
    }
}
