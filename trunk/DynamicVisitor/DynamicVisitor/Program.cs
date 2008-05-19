using System;
using System.Collections.Generic;
using System.Text;

namespace Main {
    //using Test;
    using GuidedTour;
    using Pirate;
    using PirateType=Pirate.Type;
    using Util;
    using Sexp;
    using Flat;
    using Flat2Pirate;
    using FlatType=Flat.Type;
    using DynamicVisitor;

    class Program {
#if false
        static string Example1()
        {
            Pirate p = new Pirate();
            StringLiteral s1 = new StringLiteral("H\ae\bl\fl\vo\0 \'P\"arrot!\n");
            Call c1 = new Call("print", s1);
            StmtList sl = new StmtList();
            CallStmt cs1 = new CallStmt(c1);
            sl.Add(cs1);
            Sub main = new Sub("main", sl);
            p.Add(main);

            StringWriter sw = new StringWriter();
            PirateWriter pv = new PirateWriter(sw);

            DynamicVisitor.accept(p, pv);

            return sw.ToString();
        }

        static string Example2()
        {
            NamedReg a = new NamedReg("a");
            NamedReg b = new NamedReg("b");
            NamedReg c = new NamedReg("c");
            NamedReg det = new NamedReg("det");

            IdList rl1 = new IdList();
            rl1.Add(a);
            rl1.Add(b);
            rl1.Add(c);
            rl1.Add(det);

            LocalDecl ld1 = new LocalDecl(new NumType(), rl1);

            IntLiteral il3 = new IntLiteral(2);
            Assign a12 = new Assign(a, il3);

            IntLiteral il4 = new IntLiteral(-3);
            Assign a13 = new Assign(b, il4);

            IntLiteral il5 = new IntLiteral(-2);
            Assign a14 = new Assign(c, il5);

            UnaryNeg un1 = new UnaryNeg(b);
            TmpNumReg tnr0 = new TmpNumReg(0);
            Assign a1 = new Assign(tnr0, un1);

            TmpNumReg tnr1 = new TmpNumReg(1);
            BinaryMul bm1 = new BinaryMul(b, b);
            Assign a2 = new Assign(tnr1, bm1);

            TmpNumReg tnr2 = new TmpNumReg(2);
            IntLiteral il1 = new IntLiteral(4);
            BinaryMul bm2 = new BinaryMul(il1, a);
            Assign a3 = new Assign(tnr2, bm2);

            BinaryMul bm3 = new BinaryMul(tnr2, c);
            Assign a4 = new Assign(tnr2, bm3);

            TmpNumReg tnr3 = new TmpNumReg(3);
            IntLiteral il2 = new IntLiteral(2);
            BinaryMul bm4 = new BinaryMul(il2, a);
            Assign a5 = new Assign(tnr3, bm4);

            BinarySub bs1 = new BinarySub(tnr1, tnr2);
            Assign a6 = new Assign(det, bs1);

            TmpNumReg tnr4 = new TmpNumReg(4);
            Call sqrt = new Call("sqrt", det);
            Assign a7 = new Assign(tnr4, sqrt);

            NamedReg x1 = new NamedReg("x1");
            NamedReg x2 = new NamedReg("x2");

            IdList rl2 = new IdList();
            rl2.Add(x1);
            rl2.Add(x2);

            LocalDecl ld2 = new LocalDecl(new NumType(), rl2);

            BinaryAdd ba1 = new BinaryAdd(tnr0, tnr4);
            Assign a8 = new Assign(x1, ba1);

            BinaryDiv bd1 = new BinaryDiv(x1, tnr3);
            Assign a9 = new Assign(x1, bd1);

            BinarySub bs2 = new BinarySub(tnr0, tnr4);
            Assign a10 = new Assign(x2, bs2);

            AssignDiv a11 = new AssignDiv(x2, tnr3);

            StringLiteral s1 = new StringLiteral("Answers to ABC formula are:\n");
            Call c1 = new Call("print", s1);
            CallStmt print1 = new CallStmt(c1);

            StringLiteral s2 = new StringLiteral("x1 = ");
            Call c2 = new Call("print", s2);
            CallStmt print2 = new CallStmt(c2);

            Call c3 = new Call("print", x1);
            CallStmt print3 = new CallStmt(c3);

            StringLiteral s4 = new StringLiteral("\nx2 = ");
            Call c4 = new Call("print", s4);
            CallStmt print4 = new CallStmt(c4);

            Call c5 = new Call("print", x2);
            CallStmt print5 = new CallStmt(c5);

            StringLiteral s6 = new StringLiteral("\n");
            Call c6 = new Call("print", s6);
            CallStmt print6 = new CallStmt(c6);

            StmtList sl1 = new StmtList();
            sl1.Add(ld1);
            sl1.Add(a12);
            sl1.Add(a13);
            sl1.Add(a14);
            sl1.Add(a1);
            sl1.Add(a2);
            sl1.Add(a3);
            sl1.Add(a4);
            sl1.Add(a5);
            sl1.Add(a6);
            sl1.Add(a7);
            sl1.Add(ld2);
            sl1.Add(a8);
            sl1.Add(a9);
            sl1.Add(a10);
            sl1.Add(a11);
            sl1.Add(print1);
            sl1.Add(print2);
            sl1.Add(print3);
            sl1.Add(print4);
            sl1.Add(print5);
            sl1.Add(print6);

            Sub foo = new Sub("foo", sl1);

            Pirate p = new Pirate();
            p.Add(foo);

            StringWriter sw = new StringWriter();
            PirateWriter pv = new PirateWriter(sw);

            DynamicVisitor.accept(p, pv);

            return sw.ToString();
        }

#if false
        static string Example3()
        {
            Pirate p = new Pirate();

            StmtList sl1 = new StmtList();

            Sub joe = new Sub("joe", sl1);

            p.Add(joe);

            LocalDecl ld1 = new LocalDecl();
            ld1.type = new StringType();

            NamedReg name = new NamedReg();
            name.name = "name";
            IdList idl1 = new IdList();
            idl1.Add(name);

            ld1.id_list = idl1;

            sl1.Add(ld1);

            Assign a1 = new Assign();
            a1.lval = name;

            StringLiteral s1 = new StringLiteral();
            s1.value = " Joe!";

            a1.rval = s1;

            sl1.Add(a1);

            Assign a2 = new Assign();
            StringLiteral s2 = new StringLiteral();
            s2.value = "Hi!";

            TmpStringReg tsr0 = new TmpStringReg();
            tsr0.number = 0;

            a2.lval = tsr0;
            a2.rval = s2;

            sl1.Add(a2);

            Assign a3 = new Assign();
            TmpStringReg tsr1 = new TmpStringReg();
            tsr1.number = 1;

            BinaryCat bc1 = new BinaryCat();
            bc1.a = tsr0;
            bc1.b = name;

            a3.lval = tsr1;
            a3.rval = bc1;

            sl1.Add(a3);

            AssignCat a4 = new AssignCat();
            a4.lval = tsr1;
            StringLiteral s3 = new StringLiteral();
            s3.value = "\n";

            a4.rval = s3;

            sl1.Add(a4);

            CallStmt cs1 = new CallStmt();
            Call c1 = new Call();
            c1.func = "print";
            c1.args = tsr1;
            cs1.call = c1;
            sl1.Add(cs1);

            StringWriter sw = new StringWriter();
            PirateWriter pv = new PirateWriter(sw);

            DynamicVisitor.accept(p, pv);

            return sw.ToString();
        }

        static string Example4()
        {
            StmtList sl1 = new StmtList();

            Sub foo = new Sub("foo", sl1);

            Pirate p = new Pirate();
            p.Add(foo);

            ParamDecl pd1 = new ParamDecl();

            pd1.type = new IntType();

            IdList idl1 = new IdList();
            NamedReg n = new NamedReg();
            n.name = "n";
            idl1.Add(n);

            pd1.id_list = idl1;
            sl1.Add(pd1);

            ParamDecl pd2 = new ParamDecl();

            pd2.type = new StringType();

            IdList idl2 = new IdList();
            NamedReg message = new NamedReg();
            message.name = "message";
            idl2.Add(message);

            pd2.id_list = idl2;
            sl1.Add(pd2);

            StringWriter sw = new StringWriter();
            PirateWriter pv = new PirateWriter(sw);

            DynamicVisitor.accept(p, pv);

            return sw.ToString();
        }

        static string Example5()
        {
            NamedReg x1 = new NamedReg();
            x1.name = "x1";

            NamedReg x2 = new NamedReg();
            x2.name = "x2";

            IdList idl1 = new IdList();
            idl1.Add(x1);
            idl1.Add(x2);

            LocalDecl ld1 = new LocalDecl();
            ld1.type = new NumType();
            ld1.id_list = idl1;

            AtomExprList ael1 = new AtomExprList();
            ael1.Add(x1);
            ael1.Add(x2);

            ReturnStmt rs1 = new ReturnStmt();
            rs1.rv = ael1;

            StmtList sl1 = new StmtList();
            sl1.Add(ld1);
            sl1.Add(rs1);

            Sub abc = new Sub("abc", sl1);

            Pirate p = new Pirate();
            p.Add(abc);

            StringWriter sw = new StringWriter();
            PirateWriter pv = new PirateWriter(sw);

            DynamicVisitor.accept(p, pv);

            return sw.ToString();
        }
#endif
        static string Example6()
        {
            AtomExprList ael1 = new AtomExprList();
            Call c1 = new Call("foo", ael1);

            CallStmt cs1 = new CallStmt(c1);

            NumLiteral n1 = new NumLiteral(3.14);
            TmpNumReg tnr0 = new TmpNumReg(0);
            Assign a1 = new Assign(tnr0, n1);

            TmpIntReg tir0 = new TmpIntReg(0);

            IntLiteral i1 = new IntLiteral(42);
            StringLiteral s1 = new StringLiteral("hi");

            AtomExprList ael2 = new AtomExprList();
            ael2.Add(tir0);
            ael2.Add(i1);
            ael2.Add(s1);

            Call c2 = new Call("bar", ael2);
            CallStmt cs2 = new CallStmt(c2);

            NamedReg a = new NamedReg("a");
            LocalDecl ld1 = new LocalDecl(new IntType(), a);

            NamedReg b = new NamedReg("b");
            LocalDecl ld2 = new LocalDecl(new NumType(), b);

            NamedReg c = new NamedReg("c");
            LocalDecl ld3 = new LocalDecl(new StringType(), c);

            TmpNumReg tnr2 = new TmpNumReg(2);
            NumLiteral n2 = new NumLiteral(2.7);
            Assign a2 = new Assign(tnr2, n2);

            StringLiteral s2 = new StringLiteral("hello yourself");
            AtomExprList ael3 = new AtomExprList();
            ael3.Add(tnr2);
            ael3.Add(s2);
            Call c3 = new Call("baz", ael3);

            RegList rl4 = new RegList();
            rl4.Add(a);
            rl4.Add(b);
            rl4.Add(c);

            Assign a3 = new Assign(rl4, c3);

            StmtList sl1 = new StmtList();
            sl1.Add(cs1);
            sl1.Add(a1);
            sl1.Add(cs2);
            sl1.Add(ld1);
            sl1.Add(ld2);
            sl1.Add(ld3);
            sl1.Add(a2);
            sl1.Add(a3);

            Sub main = new Sub("main", sl1);

            StringLiteral s3 = new StringLiteral("Foo!\n");
            Call c4 = new Call("print", s3);
            CallStmt cs3 = new CallStmt(c4);

            StmtList sl2 = new StmtList();
            sl2.Add(cs3);

            Sub foo = new Sub("foo", sl2);

            NamedReg i = new NamedReg("i");
            ParamDecl pd1 = new ParamDecl(new NumType(), i);

            NamedReg answer = new NamedReg("answer");
            ParamDecl pd2 = new ParamDecl(new IntType(), answer);

            NamedReg message = new NamedReg("message");
            ParamDecl pd3 = new ParamDecl(new StringType(), message);

            StringLiteral s4 = new StringLiteral("Bar!\n");
            Call print1 = new Call("print", s4);
            CallStmt cs4 = new CallStmt(print1);

            Call print2 = new Call("print", i);
            CallStmt cs5 = new CallStmt(print2);

            StringLiteral s5 = new StringLiteral("\n");
            Call print3 = new Call("print", s5);
            CallStmt cs6 = new CallStmt(print3);

            Call print4 = new Call("print", answer);
            CallStmt cs7 = new CallStmt(print4);

            CallStmt cs8 = new CallStmt(print3);

            Call print5 = new Call("print", message);
            CallStmt cs9 = new CallStmt(print5);

            StmtList sl3 = new StmtList();
            sl3.Add(pd1);
            sl3.Add(pd2);
            sl3.Add(pd3);
            sl3.Add(cs4);
            sl3.Add(cs5);
            sl3.Add(cs6);
            sl3.Add(cs7);
            sl3.Add(cs8);
            sl3.Add(cs9);

            Sub bar = new Sub("bar", sl3);

            NamedReg e = new NamedReg("e");
            ParamDecl pd4 = new ParamDecl(new NumType(), e);

            NamedReg msg = new NamedReg("msg");
            ParamDecl pd5 = new ParamDecl(new StringType(), msg);

            StringLiteral s6 = new StringLiteral("Baz!\n");
            Call print7 = new Call("print", s6);
            CallStmt cs10 = new CallStmt(print7);

            Call print8 = new Call("print", e);
            CallStmt cs11 = new CallStmt(print8);

            Call print9 = new Call("print", s5);
            CallStmt cs12 = new CallStmt(print9);

            Call print10 = new Call("print", msg);
            CallStmt cs13 = new CallStmt(print10);

            AtomExprList ael4 = new AtomExprList();
            ael4.Add(new IntLiteral(1000));
            ael4.Add(new NumLiteral(1.23));
            ael4.Add(new StringLiteral("hi from baz"));
            ReturnStmt rs1 = new ReturnStmt(ael4);

            StmtList sl4 = new StmtList();
            sl4.Add(pd4);
            sl4.Add(pd5);
            sl4.Add(cs10);
            sl4.Add(cs11);
            sl4.Add(cs12);
            sl4.Add(cs13);
            sl4.Add(rs1);

            Sub baz = new Sub("baz", sl4);

            Pirate p = new Pirate();
            p.Add(main);
            p.Add(foo);
            p.Add(bar);
            p.Add(baz);

            StringWriter sw = new StringWriter();
            PirateWriter pv = new PirateWriter(sw);

            DynamicVisitor.accept(p, pv);

            return sw.ToString();
        }
#endif
#if true
        static void test_scan(string path)
        {
            using (Reader file = new Reader(path)) {
                Scanner scanner = new Scanner(file);
                Attributes attrib;

                Console.WriteLine("scanning: {0}", path);
                do {
                    attrib = scanner.scan();

                    if (attrib.token != Token.ERROR) {
                        if (Token.NUM == attrib.token) {
                            Console.WriteLine("{0} {1} {3} {4}", attrib.loc.PathPoint(), Enum.GetName(attrib.token.GetType(), attrib.token), (attrib.literal==null)?(""):("= "+attrib.literal), (attrib.error == null)?(""):("warning: "+attrib.error));
                        }
                    } else {
                        Console.WriteLine("{0} error: {1} = {2}", attrib.loc.PathPoint(), attrib.error, attrib.literal);
                    }

                } while (attrib.token != Token.EOF);
            }
        }

        static void test_parse(string path)
        {
            using (Reader file = new Reader(path)) {
                StringWriter writer = new StringWriter();
                TopLevelWriter visitor = new TopLevelWriter(writer);
                Parser parser = new Parser(file, visitor);

                Console.WriteLine("parsing: {0}", path);
                parser.read();
                Console.WriteLine(writer.ToString());
                Console.WriteLine("{0} error{1}", parser.errors, parser.errors!=1?"s":"");
            }
        }

        static void test_safe_parse(string path)
        {
            using (Reader file = new Reader(path)) {
                //StringWriter writer = new StringWriter();
                //TopLevelWriter visitor = new TopLevelWriter(writer);
                VectorVisitor visitor = new SafeVectorVisitor(null);
                Parser parser = new Parser(file, visitor);

                Console.WriteLine("parsing: {0}", path);
                parser.read();
                //Console.WriteLine(writer.ToString());
                Console.WriteLine("{0} error{1}", parser.errors, parser.errors!=1?"s":"");
            }
        }

        static void double_parse(string path)
        {
            using (Reader file = new Reader(path)) {
                StringWriter writer = new StringWriter();
                TopLevelWriter visitor = new TopLevelWriter(writer);
                Parser parser = new Parser(file, visitor);

                parser.read();
                Console.WriteLine("{0} error{1}", parser.errors, parser.errors!=1?"s":"");

                if (parser.errors == 0) {
                    using (System.IO.StreamWriter output1 = new System.IO.StreamWriter(path + ".parsed.txt", false)) {
                        output1.Write(writer.ToString());
                    }
                }
            }
        }

        static void parse_and_write(string in_path, string out_path)
        {
            Console.WriteLine("parsing {0} and writing it out to {1}", in_path, out_path);
            using (Reader file = new Reader(in_path)) {
                using (FileWriter writer = new FileWriter(out_path)) {
                    TopLevelWriter visitor = new TopLevelWriter(writer);
                    Parser parser = new Parser(file, visitor);
                    parser.read();
                    Console.WriteLine("{0} error{1}", parser.errors, parser.errors!=1?"s":"");
                    Console.WriteLine(parser.errors == 0 ? "OK" : "FAILED!");
                }
            }
            Console.WriteLine();
        }

        static void compare_files(string path1, string path2)
        {
            Console.WriteLine("comparing the token stream of {0} and {1}", path1, path2);
            using (Reader file1 = new Reader(path1)) {
                using (Reader file2 = new Reader(path2)) {
                    Scanner s1 = new Scanner(file1);
                    Scanner s2 = new Scanner(file2);
                    Comparer comp = new Comparer(s1, s2);
                    comp.run();
                }
            }
            Console.WriteLine();
        }

        static object[] build(string path)
        {
            using (Reader file = new Reader(path)) {
                TopLevelBuilder visitor = new TopLevelBuilder();
                Parser parser = new Parser(file, visitor);

                Console.WriteLine("parsing: {0}", path);
                parser.read();
                Console.WriteLine("{0} error{1}", parser.errors, parser.errors!=1?"s":"");
                Console.WriteLine(parser.errors == 0 ? "OK" : "FAILED!");
                return visitor.getTopLevel();
            }
        }

        static void parse_build_write(string in_path, string out_path)
        {
            Console.WriteLine("parsing {0} into memory then writing it out to {1}", in_path, out_path);
            using (FileWriter writer = new FileWriter(out_path)) {
                object[] top = build(in_path);
                DynamicVisitor.accept(top, new TopLevelWriter(writer));
            }
            Console.WriteLine();
        }

        static void compare_logs(string path1, string path2)
        {
            Console.WriteLine("comparing the visitation logs for files {0} and {1}", path1, path2);
            using (Reader file1 = new Reader(path1)) {
                using (Reader file2 = new Reader(path2)) {
                    Log log1 = new Log();
                    Log log2 = new Log();
                    Util.TxtLocation loc = new Util.TxtLocation();
                    VectorLogger logger1 = new VectorLogger(log1, loc, new SafeVectorVisitor(null));
                    VectorLogger logger2 = new VectorLogger(log2, loc, new SafeVectorVisitor(null));
                    Parser p1 = new Parser(file1, logger1, loc);
                    Parser p2 = new Parser(file2, logger2, loc);
                    p1.read();
                    p2.read();
                    LogComparer.compare_logs(log1, log2);
                }
            }
            Console.WriteLine();
        }

        static void compare_logs2(string path1, string path2)
        {
            Console.WriteLine("testing the multivisitor with files {0} and {1}", path1, path2);
            using (Reader file1 = new Reader(path1)) {
                using (Reader file2 = new Reader(path2)) {
                    Log log1 = new Log();
                    Log log2 = new Log();
                    Util.TxtLocation loc = new Util.TxtLocation();
                    VectorLogger logger1 = new VectorLogger(log1, loc, new SafeVectorVisitor(null));
                    VectorLogger logger2 = new VectorLogger(log2, loc, new SafeVectorVisitor(null));
                    VectorMultiVisitor mv = new VectorMultiVisitor(logger1, logger2);
                    Parser p1 = new Parser(file1, mv, loc);
                    p1.read();
                    LogComparer.compare_logs(log1, log2);
                }
            }
            Console.WriteLine();
        }

        static void compare_logs(string path)
        {
            Console.WriteLine("comparing the visitation logs for the parser and dynamic visitor for file {0}", path);

            Log log1 = new Log();

            using (Reader file = new Reader(path)) {
                Util.TxtLocation loc = new Util.TxtLocation();
                VectorLogger logger = new VectorLogger(log1, loc, new SafeVectorVisitor(null));
                Parser p = new Parser(file, logger, loc);
                p.read();
            }

            Log log2 = new Log();

            using (Reader file = new Reader(path)) {
                TopLevelBuilder builder = new TopLevelBuilder();
                Parser p = new Parser(file, builder);
                Util.TxtLocation loc = new Util.TxtLocation();
                p.read();

                object[] top = builder.getTopLevel();
                VectorLogger logger = new VectorLogger(log2, loc, new SafeVectorVisitor(null));
                DynamicVisitor.accept(top, logger);
            }

            LogComparer.compare_logs(log1, log2);

            Console.WriteLine();
        }
#endif
        static void code_builder_test1()
        {
#if false
            ; hello world
            (code
                (type string)
                (prototype print_prototype (string) ())
                (global print print_prototype)
                (constant "Hello World!\\n" string)

                (lambda main
                    (call print_prototype print () ("Hello World!\\n"))))
#endif
            CodeBuilder cb = new CodeBuilder();
            FlatType string_type = cb.defineType("string");
            Prototype print_prototype = cb.definePrototype(null, new TypeList(string_type), null);
            Global print_global = cb.defineGlobal("print", print_prototype);
            Constant hello_world = cb.defineConstant(null, string_type, "Hello World!\n");
            LambdaBuilder lb = cb.getLambdaBuilder("main", null);
            lb.addCall(null, print_prototype, print_global, null, new OperandList(hello_world));
            cb.defineLambda(lb.getLambda(null));
            PirateCodeBuilder pir = new PirateCodeBuilder();
            DynamicVisitor.accept(cb.getCode(), pir);
            StringWriter sw = new StringWriter();
            DynamicVisitor.accept(pir.getPirate(), new PirateWriter(sw));
            Console.Write(sw.ToString());
        }

        static void code_builder_test2()
        {
            CodeBuilder cb = new CodeBuilder();

            FlatType string_type = cb.defineType("string");
            FlatType single_type = cb.defineType("single");

            Prototype print_prototype1 = cb.definePrototype(null, new TypeList(string_type), null);
            Prototype print_prototype2 = cb.definePrototype(null, new TypeList(single_type), null);
            Prototype sqrt_prototype = cb.definePrototype(null, new TypeList(single_type), new TypeList(single_type));

            Global print_global1 = cb.defineGlobal("print", print_prototype1);
            Global print_global2 = cb.defineGlobal("print", print_prototype2);
            Global sqrt_global = cb.defineGlobal("sqrt", sqrt_prototype);

            Operator neg = cb.defineOperator("neg", new TypeList(single_type), new TypeList(single_type));
            Operator add = cb.defineOperator("+", new TypeList(single_type), new TypeList(single_type, single_type));
            Operator sub = cb.defineOperator("-", new TypeList(single_type), new TypeList(single_type, single_type));
            Operator mul = cb.defineOperator("*", new TypeList(single_type), new TypeList(single_type, single_type));
            Operator div = cb.defineOperator("/", new TypeList(single_type), new TypeList(single_type, single_type));
            Operator div_assign = cb.defineOperator("/=", new TypeList(single_type), new TypeList(single_type));

            LambdaBuilder lb = cb.getLambdaBuilder("foo", null);
            Local a = lb.defineLocal("a", single_type);
            Local b = lb.defineLocal("b", single_type);
            Local c = lb.defineLocal("c", single_type);
            Local det = lb.defineLocal("det", single_type);
            Local n0 = lb.defineLocal("n0", single_type);
            Local n1 = lb.defineLocal("n1", single_type);
            Local n2 = lb.defineLocal("n2", single_type);
            Local n3 = lb.defineLocal("n3", single_type);
            Local n4 = lb.defineLocal("n4", single_type);

            Constant _2 = cb.defineConstant(null, single_type, 2);
            lb.addMove(null, new LvalueList(a), new OperandList(_2));

            Constant _n3 = cb.defineConstant(null, single_type, -3);
            lb.addMove(null, new LvalueList(b), new OperandList(_n3));

            Constant _n2 = cb.defineConstant(null, single_type, -2);
            lb.addMove(null, new LvalueList(c), new OperandList(_n2));

            lb.addOperator(null, neg, new LvalueList(n0), new OperandList(b));
            lb.addOperator(null, mul, new LvalueList(n1), new OperandList(b, b));

            Constant _4 = cb.defineConstant(null, single_type, 4);
            lb.addOperator(null, mul, new LvalueList(n2), new OperandList(_4, a));

            lb.addOperator(null, mul, new LvalueList(n2), new OperandList(n2, c));
            lb.addOperator(null, sub, new LvalueList(det), new OperandList(n1, n2));

            lb.addCall(null, sqrt_prototype, sqrt_global, new LvalueList(n4), new OperandList(det));

            Local x1 = lb.defineLocal("x1", single_type);
            Local x2 = lb.defineLocal("x2", single_type);

            lb.addOperator(null, add, new LvalueList(x1), new OperandList(n0, n4));
            lb.addOperator(null, div, new LvalueList(x1), new OperandList(x1, n3));
            lb.addOperator(null, sub, new LvalueList(x2), new OperandList(n0, n4));
            lb.addOperator(null, div_assign, new LvalueList(x2), new OperandList(n3));

            Constant answer1 = cb.defineConstant(null, string_type, "Answers to ABC formular are:\n");
            Constant answer2 = cb.defineConstant(null, string_type, "x1 = ");
            Constant answer3 = cb.defineConstant(null, string_type, "\nx2 = ");
            Constant answer4 = cb.defineConstant(null, string_type, "\n");

            lb.addCall(null, print_prototype1, print_global1, null, new OperandList(answer1));
            lb.addCall(null, print_prototype1, print_global1, null, new OperandList(answer2));
            lb.addCall(null, print_prototype2, print_global2, null, new OperandList(x1));
            lb.addCall(null, print_prototype1, print_global1, null, new OperandList(answer3));
            lb.addCall(null, print_prototype2, print_global2, null, new OperandList(x2));
            lb.addCall(null, print_prototype1, print_global1, null, new OperandList(answer4));

            cb.defineLambda(lb.getLambda(null));

            PirateCodeBuilder pir = new PirateCodeBuilder();
            DynamicVisitor.accept(cb.getCode(), pir);

            StringWriter sw = new StringWriter();
            DynamicVisitor.accept(pir.getPirate(), new PirateWriter(sw));
            Console.Write(sw.ToString());
        }

        class IntVisitor {
            Int64 i = 0;
            int scount = 0;

            public void visit()
            {
                Console.WriteLine("summing ints");
            }

            public void visit(long v)
            {
                i += v;
            }

            public void visit_value(Symbol s)
            {
                scount++;
            }

            public void visitEnd()
            {
                Console.WriteLine("total is {0}", i);
                Console.WriteLine("symbol count = {0}", scount);
            }
        }

        static void Main(string[] args)
        {
#if false
            Parent p = new Parent("Karen");
            p.children.Add(null);
            p.children.Add(new Son("Jason"));
            p.children.Add(new Daughter("April"));

            ParentVisitor pv = new ParentVisitor();

            DynamicVisitor.accept(p, pv);
#endif
#if false
            Console.WriteLine(Example1());
            //Console.WriteLine(Example2());
            ////Console.WriteLine(Example3());
            ////Console.WriteLine(Example4());
            ////Console.WriteLine(Example5());
            //Console.WriteLine(Example6());
            
            //test_parse("test.txt");
            //test_parse("test2.txt");
            //test_safe_parse("test.txt");
            //test_safe_parse("test2.txt");
#endif

#if true
            parse_and_write("test.txt", "test-output1.txt");
            parse_and_write("test-output1.txt", "test-output2.txt");
            compare_files("test.txt", "test-output1.txt");
            compare_files("test-output1.txt", "test-output2.txt");
            compare_logs("test.txt", "test-output1.txt");
            compare_logs("test-output1.txt", "test-output2.txt");
            parse_build_write("test.txt", "test-output3.txt");
            compare_files("test.txt", "test-output3.txt");
            compare_logs("test.txt", "test-output3.txt");
            compare_logs2("test.txt", "test-output3.txt");
            compare_logs("test.txt");
            compare_logs("test-output1.txt");
            compare_logs("test-output2.txt");
            compare_logs("test-output3.txt");
            //compare_logs("test.txt", "test2.txt");
            code_builder_test1();
            code_builder_test2();
#endif
#if true
            //System.IO.TextWriter my_out = new System.IO.StreamWriter("output.txt");
            //Console.SetOut(my_out);
            Reader f = new Reader("test3.txt");
            Util.TxtLocation loc = new Util.TxtLocation();
            Parser p = new Parser(f, new SafeVectorVisitor(new Interpreter(new StandardEnvironment(), loc)), loc);
            p.read();
            //Vector v = build("test.txt");
            //DynamicVisitor.accept(v, new IntVisitor());
            //DynamicVisitor.accept(v, new Interpreter(new TestEnvironment(), null));
#endif
        }
    }
}
