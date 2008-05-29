using System;
using System.Text;

using Util;

namespace Sexp {
    public class SyntaxError : Exception {
        static string fancy_token(Token t)
        {
            return '<' + Enum.GetName(t.GetType(), t).ToLower().Replace('_', '-') + '>';
        }

        static string token_string(Token[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            bool is_first = true;

            foreach (Token t in tokens) {
                if (is_first) {
                    is_first = false;
                } else {
                    sb.Append(", ");
                }

                sb.Append(fancy_token(t));
            }

            return sb.ToString();
        }

        static string make_message(TxtLocation loc, Token lookahead, Token[] tokens)
        {
            return String.Format(
                "{0}: expected {1} got {2}\n{3}\n",
                loc.PathPoint(),
                token_string(tokens),
                fancy_token(lookahead), loc.FancyContext());
        }

        public SyntaxError(TxtLocation loc, Token lookahead, params Token[] tokens)
            : base(make_message(loc, lookahead, tokens))
        {
            Data.Add("location", loc);
            Data.Add("tokens", tokens);
            Data.Add("lookahead", lookahead);
        }
    }

    public class Parser {
        readonly Scanner m_scanner;
        Attributes m_attrib;
        readonly VectVisitor m_visitor;
        TxtLocation m_loc;

        public Parser(Reader reader, VectVisitor visitor)
            : this(reader, visitor, null)
        { }

        public Parser(Reader reader, VectVisitor visitor, TxtLocation loc)
        {
            m_scanner = new Scanner(reader);
            m_visitor = visitor;
            m_loc = loc;
        }

        public void Read()
        {
            start();
        }

        public bool SafeRead()
        {
            try {
                start();
                return true;
            } catch (SyntaxError e) {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }

        Token lookahead
        {
            get { return m_attrib.token; }
        }

        void next()
        {
            m_attrib = m_scanner.scan();

            if (m_loc != null) m_loc.copy(m_attrib.loc);
        }

        void match(Token tok)
        {
            if (lookahead == tok) {
                next();
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, tok);
            }
        }

        void start()
        {
            next();
            m_visitor.visit();
            top_level(m_visitor);
            m_visitor.visitEnd();
            match(Token.EOF);
        }

        void top_level(VectVisitor vec)
        {
            datum_list(vec);

            if (Token.EOF != lookahead) {
                // ERROR
                throw new SyntaxError(m_attrib.loc, lookahead, Token.EOF);
            }
        }

        void datum_list(VectVisitor vec)
        {
            while (
                Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead ||
                Token.OPEN_PAREN == lookahead ||
                Token.QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                top_datum(vec);
            }
        }

        void top_datum(VectVisitor top)
        {
            if (Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead) {

                AtomVisitor atom = top.visitItem_Atom();
                simple_datum(atom);
            } else if (Token.OPEN_PAREN == lookahead) {
                match(Token.OPEN_PAREN);
                top_list(top);
            } else if (
                Token.QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead) {

                ConsVisitor cons = top.visitItem_Cons();
                cons.visit();
                abbreviation(cons);
                cons.visitEnd();
            } else if (Token.VECTOR == lookahead) {
                VectVisitor vec = top.visitItem_Vect();
                vec.visit();
                vector(vec);
                vec.visitEnd();
            } else {
                // ERROR
                throw new SyntaxError(m_attrib.loc, lookahead, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE);
            }
        }

        void top_list(VectVisitor top)
        {
            if (Token.CLOSE_PAREN == lookahead) {
                match(Token.CLOSE_PAREN);
                top.visitItem();
            } else if (
                Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead ||
                Token.OPEN_PAREN == lookahead ||
                Token.QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                ConsVisitor cons = top.visitItem_Cons();
                cons.visit();
                list_contents(cons);
                cons.visitEnd();
                match(Token.CLOSE_PAREN);
            } else {
                // ERROR
                throw new SyntaxError(m_attrib.loc, lookahead, Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE, Token.VECTOR);
            }
        }

        void datum(ConsVisitor cons, bool set_car)
        {
            if (Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead) {

                AtomVisitor atom = set_car ? cons.visit_Atom_car() : cons.visit_Atom_cdr();
                //atom.visit();
                simple_datum(atom);
                //atom.visitEnd();
            } else if (Token.OPEN_PAREN == lookahead) {
                match(Token.OPEN_PAREN);
                list(cons, set_car);
            } else if (
                Token.QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead) {

                ConsVisitor new_cons = set_car ? cons.visit_Cons_car() : cons.visit_Cons_cdr();
                new_cons.visit();
                abbreviation(new_cons);
                new_cons.visitEnd();
            } else if (Token.VECTOR == lookahead) {
                VectVisitor vec = set_car ? cons.visit_Vect_car() : cons.visit_Vect_cdr();
                vec.visit();
                vector(vec);
                vec.visitEnd();
            } else {
                // ERROR
                throw new SyntaxError(m_attrib.loc, lookahead, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE);
            }
        }

        void simple_datum(AtomVisitor atom)
        {
            if (Token.BOOL == lookahead) {
                if (m_attrib.value is bool) {
                    atom.visit((Boolean)m_attrib.value);
                } else {
                    throw new Exception();
                }

                match(Token.BOOL);
            } else if (Token.NUM == lookahead) {
                if (m_attrib.value is long) {
                    atom.visit((long)m_attrib.value);
                } else if (m_attrib.value is double) {
                    atom.visit((double)m_attrib.value);
                } else {
                    throw new Exception();
                }

                match(Token.NUM);
            } else if (Token.CHAR == lookahead) {
                if (m_attrib.value is char) {
                    atom.visit((char)m_attrib.value);
                } else {
                    throw new Exception();
                }

                match(Token.CHAR);
            } else if (Token.STRING == lookahead) {
                if (m_attrib.value is string) {
                    atom.visit((string)m_attrib.value);
                } else {
                    throw new Exception();
                }

                match(Token.STRING);
            } else if (Token.ID == lookahead) {
                if (m_attrib.value is Symbol) {
                    atom.visit((Symbol)m_attrib.value);
                } else {
                    throw new Exception();
                }

                match(Token.ID);
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID);
            }
        }

        void list(ConsVisitor cons, bool set_car)
        {
            if (Token.CLOSE_PAREN == lookahead) {
                match(Token.CLOSE_PAREN);

                if (set_car) {
                    cons.visit_car();
                } else {
                    cons.visit_cdr();
                }

            } else if (
                Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead ||
                Token.OPEN_PAREN == lookahead ||
                Token.QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                ConsVisitor new_cons = set_car ? cons.visit_Cons_car() : cons.visit_Cons_cdr();
                new_cons.visit();
                list_contents(new_cons);
                new_cons.visitEnd();
                match(Token.CLOSE_PAREN);
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE, Token.VECTOR);
            }
        }

        void list_contents(ConsVisitor cons)
        {
            if (Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead ||
                Token.OPEN_PAREN == lookahead ||
                Token.QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                datum(cons, true);
                list_contents_tail(cons);
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE);
            }
        }

        void list_contents_tail(ConsVisitor cons)
        {
            if (Token.CLOSE_PAREN == lookahead) {
                cons.visit_cdr();
            } else if (
                Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead ||
                Token.OPEN_PAREN == lookahead ||
                Token.QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                ConsVisitor cdr = cons.visit_Cons_cdr();
                cdr.visit();
                datum(cdr, true);
                list_contents_tail(cdr);
                cdr.visitEnd();
            } else if (Token.DOT == lookahead) {
                dot_tail(cons);
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.DOT, Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE);
            }
        }

        void dot_tail(ConsVisitor cons)
        {
            if (Token.DOT == lookahead) {
                match(Token.DOT);
                datum(cons, false);
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.DOT);
            }
        }

        void abbreviation(ConsVisitor cons)
        {
            if (Token.QUOTE == lookahead ||                
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead) {

                abbrev_prefix(cons);

                ConsVisitor cdr = cons.visit_Cons_cdr();
                cdr.visit();
                datum(cdr, true);
                cdr.visit_cdr();
                cdr.visitEnd();
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE);
            }
        }

        void abbrev_prefix(ConsVisitor abbrev)
        {
            abbrev.visit_Atom_car().visit(Abbrev.get_symbol(lookahead));

            if (Token.QUOTE == lookahead) {
                match(Token.QUOTE);
            } else if (Token.BACKQUOTE == lookahead) {
                match(Token.BACKQUOTE);
            } else if (Token.COMMA == lookahead) {
                match(Token.COMMA);
            } else if (Token.SPLICE == lookahead) {
                match(Token.SPLICE);
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE);
            }
        }

        void vector(VectVisitor vec)
        {
            if (Token.VECTOR == lookahead) {
                match(Token.VECTOR);
                vector_contents(vec);
                match(Token.CLOSE_PAREN);
            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.VECTOR);
            }
        }

        void vector_contents(VectVisitor vec)
        {
            if (Token.CLOSE_PAREN == lookahead) {
                // EPSILON
            } else if (
                Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead ||
                Token.OPEN_PAREN == lookahead ||
                Token.QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                datum_list(vec);

                if (Token.CLOSE_PAREN != lookahead) {
                    throw new SyntaxError(m_attrib.loc, lookahead, Token.CLOSE_PAREN);
                }

            } else {
                throw new SyntaxError(m_attrib.loc, lookahead, Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE, Token.VECTOR);
            }
        }
    }
}
