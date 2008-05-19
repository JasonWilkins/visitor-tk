using System;

namespace Sexp {
    public class Comparer {
        Scanner m_s1;
        Scanner m_s2;

        public Comparer(Scanner s1, Scanner s2)
        {
            m_s1 = s1;
            m_s2 = s2;
        }

        void differ(string what, Attributes a1, Attributes a2)
        {
            System.Console.WriteLine("{0} {1} << {2} >> {3} {4}",
                a1.loc.PathPoint(),
                Enum.GetName(a1.token.GetType(), a1.token),
                what,
                a2.loc.PathPoint(),
                Enum.GetName(a2.token.GetType(), a2.token));
        }

        public void run()
        {
            while (true) {
                Attributes a1;
                Attributes a2;

                a1 = m_s1.scan();
                a2 = m_s2.scan();

                if (a1.token != a2.token) {
                    differ("token", a1, a2);
                    Console.WriteLine("FAILED!");
                    break; // if tokens differ it is probably impossible to continue sanely
                }

                if (a1.value != null && a2.value != null) {
                    if (!a1.value.Equals(a2.value)) {
                        Console.WriteLine("a1.value={0} a2.value={1}", a1.value, a2.value);
                        differ("value", a1, a2);
                    }
                }

                if (a1.literal != null && a2.literal != null) {
                    if (!a1.literal.Equals(a2.literal)) {
                        Console.WriteLine("a1.literal={0} a2.literal={1}", a1.literal, a2.literal);
                        differ("literal", a1, a2);
                    }
                }

                if (Token.EOF == a1.token || Token.EOF == a2.token) {
                    Console.WriteLine("{0} and {1} scan the same", a1.loc.path, a2.loc.path);
                    Console.WriteLine("OK");
                    break;
                }
            }
        }

    }
}
