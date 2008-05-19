using System;
using System.Collections.Generic;
using System.Text;

using Writer = Util.Writer;

namespace Pirate {
    class PirateWriter {
        Writer m_writer;

        public PirateWriter(Writer writer)
        {
            m_writer = writer;
        }

        public SubWriter visitItem_Sub()
        {
            return new SubWriter(m_writer);
        }

        public CommentWriter visitItem_Comment()
        {
            return new CommentWriter(m_writer);
        }
    }

    class CommentWriter {
        Writer m_writer;

        public CommentWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_text(string text)
        {
            m_writer.Begin().Append("# ").Append(text).End();
        }
    }

    class SubWriter {
        Writer m_writer;

        public SubWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Begin().Append(".sub ");
        }

        public void visit_name(string name)
        {
            m_writer.Append(name).End();
            m_writer.Indent();
        }

        public StmtListWriter visit_stmt_list()
        {
            return new StmtListWriter(m_writer);
        }

        public void visitEnd()
        {
            // TODO: there is a bug where unindent throws its exception if visit_name is not called
            try {
                m_writer.Unindent();
                m_writer.Begin().Append(".end").End();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }

    class StmtListWriter {
        Writer m_writer;

        public StmtListWriter(Writer writer)
        {
            m_writer = writer;
        }

        public CallStmtWriter visitItem_CallStmt()
        {
            return new CallStmtWriter(m_writer);
        }

        public ReturnStmtWriter visitItem_ReturnStmt()
        {
            return new ReturnStmtWriter(m_writer);
        }

        public LocalDeclWriter visitItem_LocalDecl()
        {
            return new LocalDeclWriter(m_writer);
        }

        public ParamDeclWriter visitItem_ParamDecl()
        {
            return new ParamDeclWriter(m_writer);
        }

        public AssignWriter visitItem_Assign()
        {
            return new AssignWriter(m_writer);
        }

        public AssignWriter visitItem_AssignAdd()
        {
            return new AssignWriter(m_writer, "+=");
        }

        public AssignWriter visitItem_AssignSub()
        {
            return new AssignWriter(m_writer, "-=");
        }

        public AssignWriter visitItem_AssignMul()
        {
            return new AssignWriter(m_writer, "*=");
        }

        public AssignWriter visitItem_AssignDiv()
        {
            return new AssignWriter(m_writer, "/=");
        }

        public AssignWriter visitItem_AssignCat()
        {
            return new AssignWriter(m_writer, ".=");
        }
    }

    class AssignWriter {
        Writer m_writer;
        string m_op;

        public AssignWriter(Writer writer)
        {
            m_op = "=";
            m_writer = writer;
        }

        public AssignWriter(Writer writer, string op)
        {
            m_op = op;
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Begin();
        }

        public NamedRegWriter visit_NamedReg_lval()
        {
            return new NamedRegWriter(m_writer);
        }

        public TmpNumRegWriter visit_TmpNumReg_lval()
        {
            return new TmpNumRegWriter(m_writer);
        }
        
        public TmpStringRegWriter visit_TmpStringReg_lval()
        {
            return new TmpStringRegWriter(m_writer);
        }

        public RegListWriter visit_RegList_lval()
        {
            return new RegListWriter(m_writer);
        }

        public IntLiteralWriter visit_IntLiteral_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new IntLiteralWriter(m_writer);
        }

        public NumLiteralWriter visit_NumLiteral_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new NumLiteralWriter(m_writer);
        }

        public NamedRegWriter visit_NamedReg_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new NamedRegWriter(m_writer);
        }

        public TmpNumRegWriter visit_TmpNumReg_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new TmpNumRegWriter(m_writer);
        }

        public TmpIntRegWriter visit_TmpIntReg_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new TmpIntRegWriter(m_writer);
        }

        public TmpStringRegWriter visit_TmpStringReg_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new TmpStringRegWriter(m_writer);
        }

        public UnaryExprWriter visit_UnaryNeg_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new UnaryExprWriter(m_writer, "-");
        }

        public BinaryExprWriter visit_BinaryMul_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new BinaryExprWriter(m_writer, "*");
        }

        public BinaryExprWriter visit_BinaryDiv_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new BinaryExprWriter(m_writer, "/");
        }

        public BinaryExprWriter visit_BinaryAdd_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new BinaryExprWriter(m_writer, "+");
        }

        public BinaryExprWriter visit_BinarySub_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new BinaryExprWriter(m_writer, "-");
        }

        public BinaryExprWriter visit_BinaryCat_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new BinaryExprWriter(m_writer, ".");
        }

        public StringLiteralWriter visit_StringLiteral_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new StringLiteralWriter(m_writer);
        }

        public CallWriter visit_Call_rval()
        {
            m_writer.Append(' ').Append(m_op).Append(' ');
            return new CallWriter(m_writer);
        }

        public void visitEnd()
        {
            m_writer.End();
        }
    }

    class TmpNumRegWriter {
        Writer m_writer;

        public TmpNumRegWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_number(int number)
        {
            m_writer.Append("$N").Append(number.ToString());
        }
    }

    class TmpStringRegWriter {
        Writer m_writer;

        public TmpStringRegWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_number(int number)
        {
            m_writer.Append("$S").Append(number.ToString());
        }
    }

    class TmpIntRegWriter {
        Writer m_writer;

        public TmpIntRegWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_number(int number)
        {
            m_writer.Append("$I").Append(number.ToString());
        }
    }

    class TmpPmcRegWriter {
        Writer m_writer;

        public TmpPmcRegWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_number(int number)
        {
            m_writer.Append("$P").Append(number.ToString());
        }
    }

    class UnaryExprWriter {
        Writer m_writer;
        string m_op;

        public UnaryExprWriter(Writer writer, string op)
        {
            m_writer = writer;
            m_op = op;
        }

        public void visit()
        {
            m_writer.Append(m_op);
        }

        public TmpNumRegWriter visit_TmpNumReg_arg()
        {
            return new TmpNumRegWriter(m_writer);
        }

        public NamedRegWriter visit_NamedReg_arg()
        {
            return new NamedRegWriter(m_writer);
        }
    }

    class BinaryExprWriter {
        Writer m_writer;
        string m_op;

        public BinaryExprWriter(Writer writer, string op)
        {
            m_writer = writer;
            m_op = op;
        }

        public TmpStringRegWriter visit_TmpStringReg_lhs()
        {
            return new TmpStringRegWriter(m_writer);
        }

        public TmpStringRegWriter visit_TmpStringReg_rhs()
        {
            m_writer.Append(" ").Append(m_op).Append(" ");
            return new TmpStringRegWriter(m_writer);
        }

        public TmpNumRegWriter visit_TmpNumReg_lhs()
        {
            return new TmpNumRegWriter(m_writer);
        }

        public TmpNumRegWriter visit_TmpNumReg_rhs()
        {
            m_writer.Append(" ").Append(m_op).Append(" ");
            return new TmpNumRegWriter(m_writer);
        }

        public NamedRegWriter visit_NamedReg_lhs()
        {
            return new NamedRegWriter(m_writer);
        }

        public NamedRegWriter visit_NamedReg_rhs()
        {
            m_writer.Append(" ").Append(m_op).Append(" ");
            return new NamedRegWriter(m_writer);
        }

        public NumLiteralWriter visit_NumLiteral_lhs()
        {
            return new NumLiteralWriter(m_writer);
        }

        public NumLiteralWriter visit_NumLiteral_rhs()
        {
            m_writer.Append(" ").Append(m_op).Append(" ");
            return new NumLiteralWriter(m_writer);
        }

        public IntLiteralWriter visit_IntLiteral_lhs()
        {
            return new IntLiteralWriter(m_writer);
        }

        public IntLiteralWriter visit_IntLiteral_rhs()
        {
            m_writer.Append(" ").Append(m_op).Append(" ");
            return new IntLiteralWriter(m_writer);
        }
    }

    class NumLiteralWriter {
        Writer m_writer;

        public NumLiteralWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_value(double value)
        {
            m_writer.Append(value.ToString());
        }
    }

    class IntLiteralWriter {
        Writer m_writer;

        public IntLiteralWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_value(int value)
        {
            m_writer.Append(value.ToString());
        }
    }

    class CallStmtWriter {
        Writer m_writer;

        public CallStmtWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Begin();
        }

        public CallWriter visit_call()
        {
            return new CallWriter(m_writer);
        }

        public void visitEnd()
        {
            m_writer.End();
        }
    }

    class ReturnStmtWriter {
        Writer m_writer;

        public ReturnStmtWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Begin().Append(".return");
        }

        public AtomExprListWriter visit_AtomExprList_rv()
        {
            m_writer.Append(" ");
            return new AtomExprListWriter(m_writer);
        }

        public void visitEnd()
        {
            m_writer.End();
        }
    }

    class AtomExprListWriter {
        Writer m_writer;
        bool m_is_first;

        public AtomExprListWriter(Writer writer)
        {
            m_writer = writer;
            m_is_first = true;
        }

        public void visit()
        {
            m_writer.Append("(");
        }

        public NamedRegWriter visitItem_NamedReg()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new NamedRegWriter(m_writer);
        }

        public NumLiteralWriter visitItem_NumLiteral()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new NumLiteralWriter(m_writer);
        }

        public TmpIntRegWriter visitItem_TmpIntReg()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new TmpIntRegWriter(m_writer);
        }

        public TmpIntRegWriter visitItem_TmpNumReg()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new TmpIntRegWriter(m_writer);
        }

        public IntLiteralWriter visitItem_IntLiteral()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new IntLiteralWriter(m_writer);
        }

        public StringLiteralWriter visitItem_StringLiteral()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new StringLiteralWriter(m_writer);
        }

        public void visitEnd()
        {
            m_writer.Append(")");
        }
    }

    class RegListWriter {
        Writer m_writer;
        bool m_is_first;

        public RegListWriter(Writer writer)
        {
            m_writer = writer;
            m_is_first = true;
        }

        public void visit()
        {
            m_writer.Append("(");
        }

        public NamedRegWriter visitItem_NamedReg()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new NamedRegWriter(m_writer);
        }

        public TmpIntRegWriter visitItem_TmpIntReg()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new TmpIntRegWriter(m_writer);
        }

        public TmpIntRegWriter visitItem_TmpNumReg()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new TmpIntRegWriter(m_writer);
        }

        public void visitEnd()
        {
            m_writer.Append(")");
        }
    }

    class LocalDeclWriter {
        Writer m_writer;

        public LocalDeclWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Begin().Append(".local");
        }

        public IntTypeWriter visit_IntType_type()
        {
            m_writer.Append(" ");
            return new IntTypeWriter(m_writer);
        }

        public NumTypeWriter visit_NumType_type()
        {
            m_writer.Append(" ");
            return new NumTypeWriter(m_writer);
        }

        public StringTypeWriter visit_StringType_type()
        {
            m_writer.Append(" ");
            return new StringTypeWriter(m_writer);
        }

        public IdListWriter visit_IdList_ids()
        {
            m_writer.Append(" ");
            return new IdListWriter(m_writer);
        }

        public NamedRegWriter visit_NamedReg_ids()
        {
            m_writer.Append(" ");
            return new NamedRegWriter(m_writer);
        }

        public void visitEnd()
        {
            m_writer.End();
        }
    }

    class ParamDeclWriter {
        Writer m_writer;

        public ParamDeclWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Begin().Append(".param");
        }

        public IntTypeWriter visit_IntType_type()
        {
            m_writer.Append(" ");
            return new IntTypeWriter(m_writer);
        }

        public NumTypeWriter visit_NumType_type()
        {
            m_writer.Append(" ");
            return new NumTypeWriter(m_writer);
        }

        public StringTypeWriter visit_StringType_type()
        {
            m_writer.Append(" ");
            return new StringTypeWriter(m_writer);
        }

        public IdListWriter visit_IdList_ids()
        {
            m_writer.Append(" ");
            return new IdListWriter(m_writer);
        }

        public NamedRegWriter visit_NamedReg_ids()
        {
            m_writer.Append(" ");
            return new NamedRegWriter(m_writer);
        }

        public void visitEnd()
        {
            m_writer.End();
        }
    }

    class IntTypeWriter {
        Writer m_writer;

        public IntTypeWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Append("int");
        }
    }

    class NumTypeWriter {
        Writer m_writer;

        public NumTypeWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Append("num");
        }
    }

    class StringTypeWriter {
        Writer m_writer;

        public StringTypeWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit()
        {
            m_writer.Append("string");
        }
    }

    class IdListWriter {
        Writer m_writer;
        bool m_is_first;

        public IdListWriter(Writer writer)
        {
            m_writer = writer;
            m_is_first = true;
        }

        public NamedRegWriter visitItem()
        {
            if (!m_is_first) {
                m_writer.Append(", ");
            }

            m_is_first = false;

            return new NamedRegWriter(m_writer);
        }
    }

    class NamedRegWriter {
        Writer m_writer;

        public NamedRegWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_name(string name)
        {
            m_writer.Append(name);
        }
    }

    class CallWriter {
        Writer m_writer;

        public CallWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_func(string name)
        {
            m_writer.Append(name);
        }

        public StringLiteralWriter visit_StringLiteral_args()
        {
            m_writer.Append(" ");
            return new StringLiteralWriter(m_writer);
        }

        public NamedRegWriter visit_NamedReg_args()
        {
            m_writer.Append(" ");
            return new NamedRegWriter(m_writer);
        }

        public TmpStringRegWriter visit_TmpStringReg_args()
        {
            m_writer.Append(" ");
            return new TmpStringRegWriter(m_writer);
        }

        public AtomExprListWriter visit_AtomExprList_args()
        {
            return new AtomExprListWriter(m_writer);
        }
    }

    class StringLiteralWriter {
        Writer m_writer;

        public StringLiteralWriter(Writer writer)
        {
            m_writer = writer;
        }

        public void visit_value(string value)
        {
            m_writer.Append("\"").EscapeString(value).Append("\"");
        }
    }
}
