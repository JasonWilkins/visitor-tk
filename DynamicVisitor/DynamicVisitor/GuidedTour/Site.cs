using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Util;

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

        public SiteList(Site site)
            : this(site.value)
        { }

        public SiteList(object value)
        {
            if (value != null) {
                Type type = value.GetType();

                Type face = type.GetInterface("IEnumerable`1");

                if (null == face) {
                    face = type.GetInterface("IEnumerable");
                }

                if (face != null) {
                    foreach (object part in value as IEnumerable) {
                        m_list.Add(new Site(null, part != null ? part.GetType() : null, part));
                    }
                } else if (!type.FullName.StartsWith("System")) {
                    MemberInfo[] members = type.FindMembers(MemberTypes.Property|MemberTypes.Field, BindingFlags.Public|BindingFlags.Instance, null, null);

                    foreach (MemberInfo mi in members) {
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
                                continue;
                            }
                        }

                        m_list.Add(new Site(mi.Name, part != null ? part.GetType() : null, part));
                    }
                } else {
                    Trace.Warning("refusing to break system type {0} into parts", type.FullName);
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
