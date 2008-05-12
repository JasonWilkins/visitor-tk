using System;

namespace GuidedTour {
    class Guide {
        ISiteseer m_default_siteseer;

        public void tour(object site, ISiteseer siteseer)
        {
            tour(site, siteseer, null);
        }

        public void tour(object site, ISiteseer siteseer, ISiteseer default_siteseer)
        {
            if (null == site) throw new ArgumentNullException("site");
            if (null == siteseer && null == default_siteseer) throw new ArgumentNullException("siteseer and default_siteseer cannot both be null");

            m_default_siteseer = default_siteseer;

            start_tour(new Site(site), siteseer ?? default_siteseer);
        }

        void start_tour(Site site, ISiteseer siteseer)
        {
            if (!siteseer.view_whole(site)) {
                siteseer.begin();
                tour_each(site, siteseer);
                siteseer.end();
            }
        }

        void tour_each(Site site, ISiteseer siteseer)
        {
            foreach (Site part in new SiteList(site)) {
                ISiteseer new_siteseer;

                if (!(siteseer.view_part(part, out new_siteseer) || tour_default(part, out new_siteseer) || siteseer.view_whole(part))) {
                    tour_each(part, siteseer);
                } else if (new_siteseer != null) {
                    start_tour(part, new_siteseer);
                }
            }
        }

        bool tour_default(Site site, out ISiteseer new_siteseer)
        {
            if (m_default_siteseer != null) {
                return m_default_siteseer.view_part(site, out new_siteseer);
            } else {
                new_siteseer = null;
                return false;
            }
        }
    }
}
