using System;
using System.Collections.Generic;

namespace RealismOverhaul
{
    public class RDTechFilters
    {
        public Dictionary<string, Func<AvailablePart, bool>> filters;

        public RDTechFilters()
        {
            this.filters = new Dictionary<string, Func<AvailablePart, bool>>();
        }

        public void FilterRDNode(RDTech tech)
        {
            foreach (var filter in filters)
            {
                PruneRDNode(tech, filter.Value);
            }
        }

        private static void PruneRDNode(RDTech tech, Func<AvailablePart, bool> filter)
        {
            int count = tech.partsAssigned.Count;
            while (count-- > 0)
            {
                if (!filter(tech.partsAssigned[count]))
                    tech.partsAssigned.RemoveAt(count);
            }
        }

        private static RDTechFilters _instance;
        public static RDTechFilters Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RDTechFilters();
                return _instance;
            }
        }
    }
}