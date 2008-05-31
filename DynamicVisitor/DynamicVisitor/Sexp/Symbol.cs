using System.Collections.Generic;

namespace Symbols {
    public class Symbol {
        public string name;

        static Dictionary<string, Symbol> symbol_world = new Dictionary<string,Symbol>();

        public static Symbol get_symbol(string name)
        {
            bool ignore;
            return get_symbol(name, out ignore);
        }

        public static Symbol get_symbol(string name, out bool was_defined)
        {
            Symbol sym;

            was_defined = symbol_world.TryGetValue(name, out sym);

            if (!was_defined) {
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
