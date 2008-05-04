using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DynamicVisitor {
    //internal class VisitorMethod {
    //    private static Type[] no_params = new Type[0];
    //    private static object[] no_args = new object[0];

    //    object m_visitor;
    //    Type m_visitor_type;

    //    Type[] get_types(params object[] param_list)
    //    {
    //        Type[] type_list = new Type[param_list.GetLength(0)];
    //        int i = 0;

    //        foreach (object o in param_list) {
    //            type_list[i++] = o.GetType();
    //        }

    //        return type_list;
    //    }

    //    object[] pack(params object[] list)
    //    {
    //        return list;
    //    }

    //    public VisitorMethod(object visitor)
    //    {
    //        m_visitor = visitor;
    //        m_visitor_type = visitor.GetType();
    //    }

    //    string make_type_string(params Type[] types)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        bool is_first = true;

    //        foreach (Type t in types) {
    //            if (is_first) {
    //                is_first = false;
    //            } else {
    //                sb.Append(", ");
    //            }

    //            sb.Append(t.Name);
    //        }

    //        return sb.ToString();
    //    }

    //    MethodInfo lookup(string method_name, Type[] param_types)
    //    {
    //        MethodInfo mi = m_visitor_type.GetMethod(method_name, param_types);

    //        if (null == mi) {
    //            Console.WriteLine("debug: could not find method {0}.{1}({2})", m_visitor_type.FullName, method_name, make_type_string(param_types));
    //        }

    //        return mi;
    //    }

    //    public void visitTerminal(string name, object terminal)
    //    {
    //        Type terminal_type = terminal.GetType();
    //        Type[] param_types = get_types(terminal);

    //        MethodInfo mi;
    //        string method_name;

    //        method_name = "visit_" + terminal_type.Name + "_" + name;
    //        mi = lookup(method_name, param_types);

    //        if (null == mi) {
    //            method_name = "visit_" + name;
    //            mi = lookup(method_name, param_types);
    //        }

    //        if (mi != null) {
    //            mi.Invoke(m_visitor, pack(terminal));
    //            Console.WriteLine("debug: invoking method {0}({1})", method_name, make_type_string(param_types));
    //        } else {
    //            Console.WriteLine("debug: skipping terminal member {0} of type {1}", terminal_type.FullName);
    //        }
    //    }

    //    public object visitNonTerminal(string method_name)
    //    {
    //        MethodInfo mi = m_visitor_type.GetMethod(method_name, no_params);

    //        if (mi != null) {
    //            return mi.Invoke(m_visitor, no_args);
    //        } else {
    //            Console.WriteLine("debug: could not find method {0}.{1}(), skipping", m_visitor_type.FullName, method_name);
    //            return null;
    //        }
    //    }
    //}
}
