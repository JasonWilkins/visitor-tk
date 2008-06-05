using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Util;

namespace GuidedTour {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class SiteListAttribute : Attribute {
        public readonly string[] fields;

        public SiteListAttribute(params string[] fields)
        {
            this.fields = fields;
        }
    }

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
        static readonly Type[] no_params = new Type[0];

        List<Site> m_list = new List<Site>();

        public SiteList(Site site)
            : this(site.value)
        { }

        public SiteList(object value)
        {
            if (value != null) {
                Type type = value.GetType();

                if (value is IEnumerable) {
                    foreach (object part in value as IEnumerable) {
                        m_list.Add(new Site(null, part != null ? part.GetType() : null, part));
                    }
                } else if (!type.FullName.StartsWith("System")) {
                    object[] attrib = type.GetCustomAttributes(typeof(SiteListAttribute), false);

                    if (attrib.Length > 0) {
                        string[] sitelist = ((SiteListAttribute)attrib[0]).fields;

                        foreach (string field in sitelist) {
                            MemberInfo mi = type.GetField(field, BindingFlags.Public|BindingFlags.Instance);

                            if (null == mi) mi = type.GetProperty(field, BindingFlags.Public|BindingFlags.Instance, null, null, no_params, null);

                            if (null == mi) throw new Exception(String.Format("{0} not found", field));

                            add_member(value, type, mi);
                        }
                    } else {
                        MemberInfo[] members = type.FindMembers(MemberTypes.Property|MemberTypes.Field, BindingFlags.Public|BindingFlags.Instance, null, null);

                        foreach (MemberInfo mi in members) {
                            add_member(value, type, mi);
                        }
                    }
                } else {
                    Trace.Warning("refusing to break system type {0} into parts", type.FullName);
                }
            }
        }

        private void add_member(object value, Type type, MemberInfo mi)
        {
            object part;

            if (MemberTypes.Field == mi.MemberType) {
                FieldInfo fi = type.GetField(mi.Name);
                part = fi.GetValue(value);
            } else {
                PropertyInfo pi = type.GetProperty(mi.Name);

                // TODO: write a filter delegate for FindMembers that checks this
                if (0 == pi.GetIndexParameters().Length) {
                    part = pi.GetValue(value, null);
                } else {
                    return;
                }
            }

            m_list.Add(new Site(mi.Name, part != null ? part.GetType() : null, part));
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
