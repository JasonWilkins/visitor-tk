using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using GuidedTour;
using Sexp;
using Symbols;

namespace ConstructLang {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class RepresentativeFieldAttribute : Attribute {
        public readonly string field;
        public RepresentativeFieldAttribute(string field) { this.field = field; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class LiteralFieldAttribute : Attribute {
        public readonly string field;
        public LiteralFieldAttribute(string field) { this.field = field; }
    }

    static class ConstructLang {
        static void add_namespace_directives(List<string> ns, Dictionary<string, string> aliases, ListBox collection)
        {
            foreach (string n in ns) {
                ((IBox)collection).put(new Cons(Symbol.get_symbol("namespace"), new Cons(n)));
            }

            foreach (KeyValuePair<string, string> a in aliases) {
                ((IBox)collection).put(
                    new Cons(
                        Symbol.get_symbol("alias"),
                        new Cons(
                            a.Key,
                            new Cons(
                                new Cons(
                                    Abbrev.quote,
                                    new Cons(Symbol.get_symbol(a.Value)))))));
            }
        }

        public static object[] tour(object graph, List<string> ns, Dictionary<string, string> aliases)
        {
            Guide guide = new Guide();
            AtomBox outbox = new AtomBox();
            ListBox collection = new ListBox();
            Dictionary<object, object> symtab = new Dictionary<object, object>();
            add_namespace_directives(ns, aliases, collection);
            ConstructSiteseer construct = new ConstructSiteseer(outbox, collection, ns, symtab, aliases);
            guide.tour(graph, construct);
            return ((object[])outbox.value);
        }
    }

    class ConstructSiteseer : ISiteseer {
        readonly IBox m_collection;
        readonly IBox m_outbox;

        string m_name = null;

        List<string> m_ns;
        Dictionary<object, object> m_symtab;
        Dictionary<string, string> m_aliases;

        public ConstructSiteseer(IBox outbox, IBox collection, List<string> ns, Dictionary<object, object> symtab, Dictionary<string, string> aliases)
        {
            m_collection = collection;
            m_outbox = outbox;
            m_ns = ns;
            m_symtab = symtab;
            m_aliases = aliases;
        }

        public void begin()
        { }

        public void end()
        {
            m_outbox.put(m_collection.get());
        }

        public bool view_part(Site site, out ISiteseer new_siteseer)
        {
            if (site.type != null) {
                object[] attrib = site.type.GetCustomAttributes(typeof(LiteralFieldAttribute), true);

                if (attrib.Length > 0) {
                    FieldInfo fi = site.type.GetField(((LiteralFieldAttribute)attrib[0]).field);

                    if (fi != null) {
                        m_collection.put(fi.GetValue(site.value));
                        new_siteseer = null;
                        return true;
                    }
                }
            }

            if (Literal.is_atom_type(site.value)) {

                new_siteseer = null;

                if (site.value is Symbol) {
                    m_collection.put(new Cons(Abbrev.quote, new Cons(site.value)));
                } else if (site.value != null) {
                    m_collection.put(site.value);
                }

                return true;
            } else {
                if (site.type != null) {
                    object[] attrib = site.type.GetCustomAttributes(typeof(RepresentativeFieldAttribute), true);

                    FieldInfo fi;

                    if (attrib.Length > 0) {
                        fi = site.type.GetField(((RepresentativeFieldAttribute)attrib[0]).field);

                        if (fi != null) {
                            object name = fi.GetValue(site.value);

                            if (name != null) {
                                object sym;
                                bool was_defined = m_symtab.TryGetValue(name, out sym);

                                if (was_defined) {
                                    if (sym == site.value) {
                                        m_collection.put(name);
                                        new_siteseer = null;
                                        return true;
                                    } else {
                                        throw new Exception();
                                    }
                                } else {
                                    m_symtab.Add(name, site.value);
                                }
                            }
                        }
                    }

                    if (site.value is object[]) {
                        ListBox list = new ListBox();
                        new_siteseer = new ConstructSiteseer(m_collection, list, m_ns, m_symtab, m_aliases);
                        return true;
                    } else if (site.value is Cons) {
                        m_collection.put(site.value);
                        new_siteseer = null;
                        return true;
                    } else {
                        ProperListBox list = new ProperListBox();
                        ((IBox)list).put(Symbol.get_symbol(resolve(site.type.Namespace, site.type.FullName, site.type.Name)));
                        new_siteseer = new ConstructSiteseer(m_collection, list, m_ns, m_symtab, m_aliases);
                        return true;
                    }
                } else {
                    new_siteseer = null;
                    return true;
                }
            }
        }

        public bool view_whole(Site site)
        {
            m_name = site.name;
            return false;
        }

        string resolve(string ns, string fullname, string basename)
        {
            string alias;

            if (m_aliases.TryGetValue(fullname, out alias)) {
                return alias;
            } else {
                return use_fullname(ns) ? fullname : basename;
            }
        }

        bool use_fullname(string ns)
        {
            if (null == m_ns) return true;

            int count = 0;

            foreach (string s in m_ns) {
                if (ns == s) count++;
            }

            return count != 1;
        }
    }
}