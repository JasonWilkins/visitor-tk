namespace Sexp {
    public class Symbol {
        public string name;

        public Symbol(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }

        public override bool Equals(object obj)
        {
            if (obj is Symbol) {
                return (obj as Symbol).name == this.name;
            } else {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
