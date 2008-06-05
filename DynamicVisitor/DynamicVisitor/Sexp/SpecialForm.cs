namespace Sexp {
    public class SpecialForm {
        public delegate object Macro(object o);

        public Macro m;

        public SpecialForm(Macro m)
        {
            this.m = m;
        }
    }
}