using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Utils;

namespace GuidedTour {
    public class Site {
        public string name;
        public Type type;
        public object value;

        public Site(object value)
            : this(null, value.GetType(), value)
        { }

        public Site(string name, Type type, object value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }

    // TODO: remake NameTypeValueList into an iterator so that the whole structure does not have to be in memory at once

    class SiteList : IEnumerable<Site> {
        List<Site> m_list = new List<Site>();

        public SiteList(Site graph)
            : this(graph.value)
        { }

        public SiteList(object graph)
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
                        m_list.Add(new Site(null, t, v));
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
                        if (0 == pi.GetIndexParameters().Length) {
                            v = pi.GetValue(graph, null);
                        } else {
                            continue;
                        }
                    }

                    if (v != null) {
                        Type t = v.GetType();
                        m_list.Add(new Site(p.Name, t, v));
                    } else {
                        Trace.Verbose("debug: property/field {0}.{1} is null, skipping", graph_type.FullName, p.Name);
                    }
                }
            }
        }

        #region IEnumerable<NameTypeValue> Members

        IEnumerator<Site> IEnumerable<Site>.GetEnumerator()
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
}
