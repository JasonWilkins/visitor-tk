using System;

namespace Sexp {
    public class Complex {
        public object real_part;
        public object imaginary_part;

        public Complex(object real_part, object imaginary_part)
        {
            this.real_part = real_part;
            this.imaginary_part = imaginary_part;
        }

        public static Complex from_polar(object angle, object distance)
        {
            return new Complex(0L,0L);
            //throw new NotImplementedException();
        }
    }
}
