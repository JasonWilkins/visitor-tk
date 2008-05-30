using System.Collections;
using System.Collections.Generic;

using GuidedTour;
using Sexp;

namespace ConstructLang {
    static class ConstructLang {
        public static object[] tour(object graph)
        {
            Guide guide = new Guide();
            AtomBox outbox = new AtomBox();
            ListBox collection = new ListBox();
            ConstructSiteseer construct = new ConstructSiteseer(outbox, collection);
            guide.tour(graph, construct);
            return ((object[])outbox.value);
        }
    }

    class ConstructSiteseer : ISiteseer {
        readonly IBox m_collection;
        readonly IBox m_outbox;
        
        string m_name = null;
        
        public ConstructSiteseer(IBox outbox, IBox collection)
        {
            m_collection = collection;
            m_outbox = outbox;
        }

        public void begin()
        { }

        public void end()
        {
            if (m_name != null) {
                m_outbox.put(new Cons(Abbrev.quote, new Cons(new Cons(Symbol.get_symbol(m_name), new Cons(m_collection.get())))));
            } else {
                m_outbox.put(m_collection.get());
                //m_outbox.put(new Cons(Abbrev.quote, new Cons(m_collection.get())));
            }
        }

        public bool view_part(Site site, out ISiteseer new_siteseer)
        {
            if (Literal.is_atom_type(site.value)) {
                new_siteseer = null;

                if (site.name != null) {
                    m_collection.put(new Cons(Abbrev.quote, new Cons(new Cons(Symbol.get_symbol(site.name), new Cons(site.value)))));
                } else {
                    m_collection.put(new Cons(Abbrev.quote, new Cons(site.value)));
                }

                return true;
            } else {
                if (site.value is IEnumerable) {
                    ProperListBox list = new ProperListBox();
                    if (site.type != null) ((IBox)list).put(Symbol.get_symbol(site.type.Name));
                    new_siteseer = new ConstructSiteseer(m_collection, list);
                } else {
                    ProperListBox list = new ProperListBox();
                    if (site.type != null) ((IBox)list).put(Symbol.get_symbol(site.type.Name));
                    new_siteseer = new ConstructSiteseer(m_collection, list);
                }

                return true;
            }
        }

        public bool view_whole(Site site)
        {
            m_name = site.name;
            return false;
        }
    }
}