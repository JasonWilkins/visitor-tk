using System.Collections.Generic;

namespace Sexp {
    public class Symbol {
        public string name;

        static Dictionary<string, Symbol> symbol_world = new Dictionary<string,Symbol>();

        public static Symbol get_symbol(string name)
        {
            Symbol sym;

            if (!symbol_world.TryGetValue(name, out sym)) {
                sym = new Symbol(name);
                symbol_world.Add(name, sym);
            }

            return sym;
        }

        Symbol(string name)
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
                return this == obj;
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
