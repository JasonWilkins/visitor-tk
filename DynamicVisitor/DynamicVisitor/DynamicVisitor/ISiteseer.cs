using System;

namespace DynamicVisitor {
    public interface ISiteseer {
        void begin();
        void end();
        bool view_part(Site site, out ISiteseer new_siteseer);
        bool view_whole(Site site);
    }
}
