//#define WARNING
//#define VERBOSE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Utils;

namespace DynamicVisitor {
    public class Operator {
        private static Type[] no_params = { };
        private static object[] no_args = { };

        Object visitor;
        Type visitor_type;

        public Operator(Object visitor)
        {
            if (null == visitor) throw new ArgumentNullException("visitor");

            this.visitor = visitor;
            this.visitor_type = visitor.GetType();
        }

        public bool parent(NameTypeValue node)
        {
            Type[] param = { node.type };
            MethodInfo mi = visitor_type.GetMethod("visit", param);

            if (mi != null) {
                Object[] arg = { node.value };
                mi.Invoke(visitor, arg);
                return true;
            } else {
                Trace.Verbose("debug: could not find method {0}.visit({1}), skipping", visitor_type.FullName, node.type.FullName);
                return false;
            }
        }

        public void begin()
        {
            MethodInfo mi = visitor_type.GetMethod("visit", no_params);

            if (mi != null) {
                mi.Invoke(visitor, no_args);
            } else {
                Trace.Verbose("debug: could not find method {0}.visit(), skipping", visitor_type.FullName);
            }
        }

        public void end()
        {
            MethodInfo mi = visitor_type.GetMethod("visitEnd", no_params);

            if (mi != null) {
                mi.Invoke(visitor, no_args);
            } else {
                Trace.Verbose("debug: could not find method {0}.visitEnd(), skipping", visitor_type.FullName);
            }
        }

        bool invokeTerminal(string method_name, Type type, object value, out Operator new_op)
        {
            while (type != null) {
                Type[] types = { type };
                MethodInfo mi = visitor_type.GetMethod(method_name, types);

                if (mi != null) {
                    object[] args = { value };
                    Object new_visitor = mi.Invoke(visitor, args);
                    
                    if (null == new_visitor) {
                        new_op = null;
                        Trace.Warning("debug: {0}.{1}({2}) did not return a new visitor", visitor_type.FullName, method_name, type.FullName);
                    } else {
                        new_op = new Operator(new_visitor);
                    }

                    return true;
                } else {
                    Trace.Warning("debug: could not find method {0}.{1}({2}), skipping", visitor_type.FullName, method_name, type.FullName);
                    type = type.BaseType;
                }
            }

            new_op = null;

            return false;
        }

        bool innerCall(string method_name, out Operator new_method, bool is_last)
        {
            MethodInfo mi = visitor_type.GetMethod(method_name, no_params);

            if (mi != null) {
                Object new_visitor = mi.Invoke(visitor, no_args);

                if (null == new_visitor) {
                    new_method = null;
                    Trace.Verbose("debug: {0} did not return a new visitor", method_name);
                } else {
                    new_method = new Operator(new_visitor);
                }

                return true;
            } else {
                if (is_last) {
                    Trace.Warning("debug: could not find method {0}.{1}(), skipping", visitor_type.FullName, method_name);
                }
                new_method = null;
                return false;
            }
        }

        bool invokeNonTerminal(string prefix, Type type, string name, out Operator new_method)
        {
            while (type.BaseType != null) {
                if (!type.IsGenericType) {
                    if (innerCall(prefix+"_"+type.Name+(name!=null?"_"+name:""), out new_method, false)) {
                        return true;
                    }
                }

                type = type.BaseType;
            }

            return innerCall(prefix+(name!=null?"_"+name:""), out new_method, true);
        }

        public bool child(NameTypeValue node, out Operator new_op)
        {
            string prefix;

            if (node.name != null) {
                prefix = "visit";
            } else {
                prefix = "visitItem";
            }

            return
                invokeNonTerminal(prefix, node.type, node.name, out new_op) ||
                invokeTerminal(prefix+(node.name!=null?"_"+node.name:""), node.type, node.value, out new_op);
        }
    }

    public class NameTypeValue {
        public string name;
        public Type type;
        public object value;

        public NameTypeValue(string name, Type type, object value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }

    // TODO: remake NameTypeValueList into an iterator so that the whole structure does not have to be in memory at once

    class NameTypeValueList : IEnumerable<NameTypeValue> {
        List<NameTypeValue> m_list = new List<NameTypeValue>();

        public NameTypeValueList(NameTypeValue graph)
            : this(graph.value)
        { }

        public NameTypeValueList(object graph)
        {
            Type graph_type = graph.GetType();

            Type face = graph_type.GetInterface("IEnumerable`1");

            if (null == face) {
                face = graph_type.GetInterface("IEnumerable");
            }

            if (face != null) {
                foreach (object v in graph as IEnumerable) {
                    if (v != null) {
                        Type t = v.GetType();
                        m_list.Add(new NameTypeValue(null, t, v));
                    } else {
                        Type[] args = face.GetGenericArguments();
                        Trace.Verbose("debug: item in {0}{1} is null, skipping", graph_type.FullName, args.Length > 0 ? "<"+args[0]+">" : "");
                    }
                }
            } else {
                MemberInfo[] graph_props = graph_type.FindMembers(MemberTypes.Property|MemberTypes.Field, BindingFlags.Public|BindingFlags.Instance, null, null);

                foreach (MemberInfo p in graph_props) {
                    object v;

                    if (MemberTypes.Field == p.MemberType) {
                        FieldInfo fi = graph_type.GetField(p.Name);
                        v = fi.GetValue(graph);
                    } else {
                        PropertyInfo pi = graph_type.GetProperty(p.Name);
                        v = pi.GetValue(graph, null);
                    }

                    if (v != null) {
                        Type t = v.GetType();
                        m_list.Add(new NameTypeValue(p.Name, t, v));
                    } else {
                        Trace.Verbose("debug: property/field {0}.{1} is null, skipping", graph_type.FullName, p.Name);
                    }
                }
            }
        }

        #region IEnumerable<NameTypeValue> Members

        IEnumerator<NameTypeValue> IEnumerable<NameTypeValue>.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #endregion
    }

    class Walkabout {
        public void accept(Object parent, Operator op)
        {
            if (null == parent) throw new ArgumentNullException("parent");

            accept(new NameTypeValue(null, parent.GetType(), parent), op);
        }

        void accept(NameTypeValue parent, Operator op)
        {
            if (!op.parent(parent)) {

                op.begin();

                foreach (NameTypeValue child in new NameTypeValueList(parent)) {
                    Operator new_op;

                    if (!op.child(child, out new_op)) {
                        accept(child, op);
                    } else if (new_op != null) {
                        accept(child, new_op);
                    }
                }

                op.end();
            }
        }
    }
}
