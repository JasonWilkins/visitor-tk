using System;
using System.Text;

namespace Sexp {
    public class Parser {
        Scanner m_scanner;
        Attributes m_attrib;
        VectorVisitor m_visitor;
        int m_errors = 0;

        Token lookahead
        {
            get { return m_attrib.token; }
        }

        public int errors
        {
            get { return m_errors; }
        }

        public Parser(Reader reader, VectorVisitor visitor)
        {
            m_scanner = new Scanner(reader);
            m_visitor = visitor;
        }

        public void read()
        {
            start_read();
        }

        Token[] pack(params Token[] tokens)
        {
            return tokens;
        }

        string token_string(Token[] tokens)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Token t in tokens) {
                sb.Append(Enum.GetName(t.GetType(), t));
                sb.Append(" ");
            }

            return sb.ToString();
        }

        void expecting(string context, Token[] tokens)
        {
            m_errors++;

            Console.WriteLine(
                "{0}[{2}, {3}] {1}: expecting {4}got {5}",
                m_attrib.path,
                context,
                m_attrib.line,
                m_attrib.column,
                token_string(tokens),
                Enum.GetName(lookahead.GetType(), lookahead));
        }

        void next()
        {
            m_attrib = m_scanner.scan();
        }

        void match(Token tok)
        {
            if (lookahead == tok) {
                next();
            } else {
                expecting("match", pack(tok));
            }
        }

        void start_read()
        {
            next();
            m_visitor.visit();
            top_level(m_visitor);
            m_visitor.visitEnd();
            match(Token.EOF);
        }

        void top_level(VectorVisitor vec)
        {
            datum_list(vec);

            if (Token.EOF != lookahead) {
                // ERROR
                expecting("top_level", pack(Token.EOF));
            }
        }

        void datum_list(VectorVisitor vec)
        {
            while (
                Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead ||
                Token.OPEN_PAREN == lookahead ||
                Token.SINGLE_QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                top_datum(vec);
            }
        }

        void top_datum(VectorVisitor top)
        {
            if (Token.BOOL == lookahead ||
                Token.NUM == lookahead ||
                Token.CHAR == lookahead ||
                Token.STRING == lookahead ||
                Token.ID == lookahead) {

                AtomVisitor atom = top.visitItem_Atom();
                atom.visit();
                simple_datum(atom);
                atom.visitEnd();
            } else if (
                Token.OPEN_PAREN == lookahead ||
                Token.SINGLE_QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead) {

                ConsVisitor cons = top.visitItem_Cons();
                cons.visit();
                compound_datum(cons);
                cons.visitEnd();
            } else if (Token.VECTOR == lookahead) {
                VectorVisitor vec = top.visitItem_Vector();
                vec.visit();
                vector(vec);
                vec.visitEnd();
            } else {
                // ERROR
                expecting("top_datum", pack(Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE));
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
                atom.visit();
                simple_datum(atom);
                atom.visitEnd();
            } else if (
                Token.OPEN_PAREN == lookahead ||
                Token.SINGLE_QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead) {

                ConsVisitor new_cons = set_car ? cons.visit_Cons_car() : cons.visit_Cons_cdr();
                new_cons.visit();
                compound_datum(new_cons);
                new_cons.visitEnd();
            } else if (Token.VECTOR == lookahead) {
                VectorVisitor vec = set_car ? cons.visit_Vector_car() : cons.visit_Vector_cdr();
                vec.visit();
                vector(vec);
                vec.visitEnd();
            } else {
                // ERROR
                expecting("datum", pack(Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE));
            }
        }

        void simple_datum(AtomVisitor atom)
        {
            if (Token.BOOL == lookahead) {
                atom.visit_value((Boolean)m_attrib.value);
                match(Token.BOOL);
            } else if (Token.NUM == lookahead) {
                if (m_attrib.value is Int64) {
                    atom.visit_value((Int64)m_attrib.value);
                } else if (m_attrib.value is Double) {
                    atom.visit_value((Double)m_attrib.value);
                } else {
                    throw new InvalidOperationException();
                }

                match(Token.NUM);
            } else if (Token.CHAR == lookahead) {
                atom.visit_value((Char)m_attrib.value);
                match(Token.CHAR);
            } else if (Token.STRING == lookahead) {
                atom.visit_value((String)m_attrib.value);
                match(Token.STRING);
            } else if (Token.ID == lookahead) {
                symbol(atom);
            } else {
                // ERROR
                expecting("simple_datum", pack(Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID));
            }
        }

        void symbol(AtomVisitor atom)
        {
            if (Token.ID == lookahead) {
                SymbolVisitor symbol = atom.visit_Symbol_value();
                symbol.visit();
                symbol.visit_name(((Symbol)m_attrib.value).name);
                symbol.visitEnd();
                match(Token.ID);
            } else {
                // ERROR
                expecting("symbol", pack(Token.ID));
            }
        }

        void compound_datum(ConsVisitor cons)
        {
            if (Token.OPEN_PAREN == lookahead ||
                Token.SINGLE_QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead) {

                list(cons);
            } else {
                // ERROR
                expecting("compound_datum", pack(Token.OPEN_PAREN, Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE, Token.VECTOR));
            }

        }

        void list(ConsVisitor cons)
        {
            if (Token.OPEN_PAREN == lookahead) {
                match(Token.OPEN_PAREN);
                list_contents(cons);
                match(Token.CLOSE_PAREN);
            } else if (
                Token.SINGLE_QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead) {

                abbreviation(cons);
            } else {
                // ERROR
                expecting("list", pack(Token.OPEN_PAREN, Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE));
            }
        }

        void list_contents(ConsVisitor cons)
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
                Token.SINGLE_QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                datum(cons, true);
                list_contents_tail(cons);
            } else {
                // ERROR
                expecting("list_contents", pack(Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE));
            }
        }

        void list_contents_tail(ConsVisitor cons)
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
                Token.SINGLE_QUOTE == lookahead ||
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
                // ERROR
                expecting("list_contents", pack(Token.DOT, Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE));
            }
        }

        void dot_tail(ConsVisitor cons)
        {
            if (Token.DOT == lookahead) {
                match(Token.DOT);
                datum(cons, false);
            } else {
                // ERROR
                expecting("dot_tail", pack(Token.DOT));
            }
        }

        void abbreviation(ConsVisitor cons)
        {
            if (Token.SINGLE_QUOTE == lookahead ||                
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead) {

                abbrev_prefix(cons);

                ConsVisitor cdr = cons.visit_Cons_cdr();
                cdr.visit();
                datum(cdr, true);
                cdr.visitEnd();
            } else {
                // ERROR
                expecting("abbreviation", pack(Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE));
            }
        }

        void abbrev_prefix(ConsVisitor abbrev)
        {
            if (Token.SINGLE_QUOTE == lookahead) {
                abbrev_helper(abbrev, "quote");
                match(Token.SINGLE_QUOTE);
            } else if (Token.BACKQUOTE == lookahead) {
                abbrev_helper(abbrev, "quasiquotation");
                match(Token.BACKQUOTE);
            } else if (Token.COMMA == lookahead) {
                abbrev_helper(abbrev, "unquote");
                match(Token.COMMA);
            } else if (Token.SPLICE == lookahead) {
                abbrev_helper(abbrev, "unquote-splicing");
                match(Token.SPLICE);
            } else {
                // ERROR
                expecting("abbrev_prefix", pack(Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE));
            }
        }

        void abbrev_helper(ConsVisitor abbrev, string name)
        {
            AtomVisitor atom = abbrev.visit_Atom_car();
            atom.visit();

            SymbolVisitor symbol = atom.visit_Symbol_value();
            symbol.visit();

            symbol.visit_name(name);

            symbol.visitEnd();

            atom.visitEnd();
        }

        void vector(VectorVisitor vec)
        {
            if (Token.VECTOR == lookahead) {
                match(Token.VECTOR);
                vector_contents(vec);
                match(Token.CLOSE_PAREN);
            } else {
                // ERROR;
                expecting("vector", pack(Token.VECTOR));
            }
        }

        void vector_contents(VectorVisitor vec)
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
                Token.SINGLE_QUOTE == lookahead ||
                Token.BACKQUOTE == lookahead ||
                Token.COMMA == lookahead ||
                Token.SPLICE == lookahead ||
                Token.VECTOR == lookahead) {

                datum_list(vec);

                if (Token.CLOSE_PAREN != lookahead) {
                    // ERROR
                    expecting("vector_contents", pack(Token.CLOSE_PAREN));
                }

            } else {
                // ERROR
                expecting("vector_contents", pack(Token.CLOSE_PAREN, Token.BOOL, Token.NUM, Token.CHAR, Token.STRING, Token.ID, Token.OPEN_PAREN, Token.SINGLE_QUOTE, Token.BACKQUOTE, Token.COMMA, Token.SPLICE, Token.VECTOR));
            }
        }
    }
}