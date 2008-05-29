using System;

namespace Sexp {
    static class Abbrev {
        static readonly Symbol quote = Symbol.get_symbol("quote");
        static readonly Symbol quasiquotation = Symbol.get_symbol("quasiquotation");
        static readonly Symbol unquote = Symbol.get_symbol("unquote");
        static readonly Symbol unquote_splicing = Symbol.get_symbol("unquote-splicing");

        public static string get_abbrev(Symbol sym)
        {
            if (sym == quote) {
                return "'";
            } else if (sym == quasiquotation) {
                return "`";
            } else if (sym == unquote) {
                return ",";
            } else if (sym == unquote_splicing) {
                return ",@";
            } else {
                return null;
            }
        }

        public static Symbol get_symbol(Token tok)
        {
            switch (tok) {
                case Token.QUOTE:
                    return quote;

                case Token.BACKQUOTE:
                    return quasiquotation;

                case Token.COMMA:
                    return unquote;

                case Token.SPLICE:
                    return unquote_splicing;

                default:
                    throw new Exception();
            }
        }
    }
}
