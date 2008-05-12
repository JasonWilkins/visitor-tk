#define WARNING
#define VERBOSE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicVisitor {
    static class DynamicVisitor {
        public static void accept(object graph, object visitor)
        {
            Guide ov = new Guide();
            ov.tour(graph, new VisitSiteseer(visitor));
        }
#if false
        private static Type[] no_params = new Type[0];
        private static object[] no_args = new object[0];

        private static void inner_call(string visit_method, Type t, object v, object visitor, Type visitor_type)
        {
            if (t.Namespace.StartsWith("System")) {
                Type[] types = new Type[1];
                types[0] = t;
                MethodInfo mi = visitor_type.GetMethod(visit_method, types);

                if (mi != null) {
                    object[] args = new object[1];
                    args[0] = v;

                    mi.Invoke(visitor, args);
                } else {
                    Trace.Warning("debug: could not find method {0}.{1}({2}), skipping", visitor_type.FullName, visit_method, t.FullName);
                }
            } else {
                MethodInfo mi = visitor_type.GetMethod(visit_method, no_params);

                if (mi != null) {
                    object new_visitor = mi.Invoke(visitor, no_args);

                    if (null == new_visitor) {
                        Trace.Verbose("debug: {0} did not return a new visitor", visit_method);
                    }

                    accept(v, new_visitor);
                } else {
                    Trace.Warning("debug: could not find method {0}.{1}(), skipping", visitor_type.FullName, visit_method);
                }
            }
        }

        public static void accept(object graph, object visitor)
        {
            if (graph != null && visitor != null) {
                Type visitor_type = visitor.GetType();

                {
                    MethodInfo mi = visitor_type.GetMethod("visit", no_params);

                    if (mi != null) {
                        mi.Invoke(visitor, no_args);
                    } else {
                        Trace.Verbose("debug: could not find method {0}.visit(), skipping", visitor_type.FullName);
                    }
                }

                Type graph_type = graph.GetType();

                Type face = graph_type.GetInterface("IEnumerable`1");
                if (face != null) {
                    Type[] args = face.GetGenericArguments();
                    foreach (object v in graph as IEnumerable) {
                        if (v != null) {
                            Type t = v.GetType();

                            string visit_method;

                            if (t.FullName.StartsWith("System") || args[0].IsSealed) {
                                visit_method = "visitItem";
                            } else {
                                visit_method = "visitItem_" + t.Name;
                            }

                            inner_call(visit_method, t, v, visitor, visitor_type);
                        } else {
                            Trace.Verbose("debug: item in {0}<{1}> is null, skipping", graph_type.FullName, args[0]);
                        }
                    }
                } else {
                    MemberInfo[] graph_props = graph_type.FindMembers(MemberTypes.Property|MemberTypes.Field, BindingFlags.Public|BindingFlags.Instance, null, null);

                    foreach (MemberInfo p in graph_props) {
                        Type pt;
                        object v;

                        if (MemberTypes.Field == p.MemberType) {
                            FieldInfo fi = graph_type.GetField(p.Name);
                            pt = fi.FieldType;
                            v = fi.GetValue(graph);
                        } else {
                            PropertyInfo pi = graph_type.GetProperty(p.Name);
                            pt = pi.PropertyType;
                            v = pi.GetValue(graph, null);
                        }

                        if (v != null) {
                            string visit_method;

                            Type t = v.GetType();

                            if (t.FullName.StartsWith("System") || pt.IsSealed) {
                                visit_method = "visit_" + p.Name;
                            } else {
                                visit_method = "visit_" + t.Name + "_" + p.Name;
                            }

                            inner_call(visit_method, t, v, visitor, visitor_type);
                        } else {
                            Trace.Verbose("debug: property/field {0}.{1} is null, skipping", graph_type.FullName, p.Name);
                        }
                    }
                }

                {
                    MethodInfo mi = visitor_type.GetMethod("visitEnd", no_params);

                    if (mi != null) {
                        mi.Invoke(visitor, no_args);
                    } else {
                        Trace.Verbose("debug: could not find method {0}.visitEnd(), skipping", visitor_type.FullName);
                    }
                }
            } else {
                if (null == graph) {
                    Trace.Warning("debug: graph null, skipping");
                } else {
                    Trace.Warning("debug: visitor null, skipping");
                }
            }
        }
#endif
    }
}
