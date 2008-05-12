using GuidedTour;

namespace DynamicVisitor {
    static class DynamicVisitor {
        public static void accept(object graph, object visitor)
        {
            Guide guide = new Guide();
            guide.tour(graph, new VisitSiteseer(visitor));
        }
    }
}
