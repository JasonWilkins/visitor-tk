using System;
using System.Collections.Generic;
using System.Text;

using Util;

namespace Sexp {

    public class Attributes {
        public TxtLocation loc;
        public string literal;
        public object value;
        public string error;
        public Token token;
    }

    public enum Token {
        ERROR,
        EOF,
        OPEN_PAREN,
        CLOSE_PAREN,
        QUOTE,
        BACKQUOTE,
        ID,
        STRING,
        BOOL,
        CHAR,
        VECTOR,
        NUM,
        NAMED_CONSTANT,
        BIT_STRING,
        EXTERNAL,
        HASH_NUMBER,
        CIRCULAR_INPUT,
        CIRCULAR_OUTPUT,
        SPLICE,
        COMMA,
        CLOSE_SQUARE,
        DOT,
    }
}
