#define WARNING
#define VERBOSE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DynamicVisitor {
    using Trace;

    public class ObjectVisitorMethod {
        private static Type[] no_params = new Type[0];
        private static object[] no_args = new object[0];
        
        public void invokeVisit(Object visitor, Type visitor_type)
        {
            MethodInfo mi = visitor_type.GetMethod("visit", no_params);

            if (mi != null) {
                mi.Invoke(visitor, no_args);
            } else {
                Trace.Verbose("debug: could not find method {0}.visit(), skipping", visitor_type.FullName);
            }
        }

        public void invokeVisitEnd(Object visitor, Type visitor_type)
        {
            MethodInfo mi = visitor_type.GetMethod("visitEnd", no_params);

            if (mi != null) {
                mi.Invoke(visitor, no_args);
            } else {
                Trace.Verbose("debug: could not find method {0}.visitEnd(), skipping", visitor_type.FullName);
            }
        }

        bool is_terminal(Type type)
        {
            return type.FullName.StartsWith("System");
        }

        void invokeTerminal(Object visitor, Type visitor_type, string method_name, Type type, object value)
        {
            Type[] types = new Type[1];

            while (type != null) {
                types[0] = type;
                MethodInfo mi = visitor_type.GetMethod(method_name, types);

                if (mi != null) {
                    object[] args = new object[1];
                    args[0] = value;
                    mi.Invoke(visitor, args);
                    return;
                } else {
                    Trace.Warning("debug: could not find method {0}.{1}({2}), skipping", visitor_type.FullName, method_name, type.FullName);
                    type = type.BaseType;
                }
            }
        }

        bool innerCall(Object visitor, Type visitor_type, string method_name, out Object new_visitor, bool is_last)
        {
            MethodInfo mi = visitor_type.GetMethod(method_name, no_params);

            if (mi != null) {
                new_visitor = mi.Invoke(visitor, no_args) as Object;

                if (null == new_visitor) {
                    Trace.Verbose("debug: {0} did not return a new visitor", method_name);
                }

                return true;
            } else {
                if (is_last) {
                    Trace.Warning("debug: could not find method {0}.{1}(), skipping", visitor_type.FullName, method_name);
                }
                new_visitor = null;
                return false;
            }
        }

        Object invokeNonTerminal(Object visitor, Type visitor_type, string prefix, Type type, string name)
        {
            Object new_visitor = null;

            while (type.BaseType != null) {
                if (!type.IsGenericType) {
                    if (innerCall(visitor, visitor_type, prefix+"_"+type.Name+(name!=null?"_"+name:""), out new_visitor, false)) {
                        return new_visitor;
                    }
                }

                type = type.BaseType;
            }

            innerCall(visitor, visitor_type, prefix+(name!=null?"_"+name:""), out new_visitor, true);
            return new_visitor;
        }

        public Object invokeVisit(Object visitor, Type visitor_type, Type type, string name, object value)
        {
            if (null == type) throw new ArgumentNullException("type");

            string prefix;

            if (name != null) {
                prefix = "visit";
            } else {
                prefix = "visitItem";
            }

            if (is_terminal(type)) {
                invokeTerminal(visitor, visitor_type, prefix+"_"+name, type, value);
                return null;
            } else {
                return invokeNonTerminal(visitor, visitor_type, prefix, type, name);
            }
        }
    }

    class NameTypeValue {
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

    class NameTypeValueList : IEnumerable<NameTypeValue> {
        List<NameTypeValue> m_list = new List<NameTypeValue>();

        public NameTypeValueList(object graph)
        {
            Type graph_type = graph.GetType();

            Type face = graph_type.GetInterface("IEnumerable`1");

            if (face != null) {
                foreach (object v in graph as IEnumerable) {
                    if (v != null) {
                        Type t = v.GetType();
                        m_list.Add(new NameTypeValue(null, t, v));
                    } else {
                        Type[] args = face.GetGenericArguments();
                        Trace.Verbose("debug: item in {0}<{1}> is null, skipping", graph_type.FullName, args[0]);
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

    class ObjectVisitor {
        ObjectVisitorMethod m_method;

        public ObjectVisitor(ObjectVisitorMethod method)
        {
            m_method = method;
        }

        public void accept(object value, Object visitor)
        {
            if (null != value && null != visitor) {
                Type visitor_type = visitor.GetType();

                m_method.invokeVisit(visitor, visitor_type);

                foreach (NameTypeValue child in new NameTypeValueList(value)) {
                    Object new_visitor = m_method.invokeVisit(visitor, visitor_type, child.type, child.name, child.value);

                    if (new_visitor != null) {
                        accept(child.value, new_visitor);
                    }
                }

                m_method.invokeVisitEnd(visitor, visitor_type);
            } else {
                if (null == value) {
                    Trace.Warning("debug: value null, skipping");
                } else {
                    Trace.Warning("debug: visitor null, skipping");
                }
            }
        }
    }
}
