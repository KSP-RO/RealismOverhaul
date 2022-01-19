using System;
using System.Collections.Generic;

namespace RealismOverhaul
{
    public class RDTechFilters
    {
        public class Filter
        {
            public string filterID;
            public Func<AvailablePart, bool> criteria;

            public Filter(string filterID, Func<AvailablePart, bool> criteria)
            {
                this.filterID = filterID;
                this.criteria = criteria;
            }
        }

        public class FilterList
        {
            private List<Filter> filterList;

            public FilterList()
            {
                this.filterList = new List<Filter>();
            }

            public void AddFilter(Filter filter)
            {
                if (this.filterList.Contains(filter))
                {
                    return;
                }
                this.filterList.Add(filter);
            }

            public void RemoveFilter(string id)
            {
                RemoveFilter(GetFilterFromID(id));
            }

            public void RemoveFilter(Filter filter)
            {
                if (!this.filterList.Contains(filter))
                {
                    return;
                }
                this.filterList.Remove(filter);
            }

            public Filter GetFilterFromID(string id)
            {
                foreach (var filter in this.filterList)
                {
                    if (filter.filterID == id)
                    {
                        return filter;
                    }
                }
                return null;
            }

            public List<Filter>.Enumerator GetEnumerator() => this.filterList.GetEnumerator();

        }

        public FilterList filters;

        public RDTechFilters()
        {
            this.filters = new FilterList();
        }

        public void FilterRDNode(RDTech tech)
        {
            foreach (var filter in filters)
            {
                PruneRDNode(tech, filter);
            }
        }

        private static void PruneRDNode(RDTech tech, Filter filter)
        {
            int count = tech.partsAssigned.Count;
            while(count-- > 0)
            {
                if (!filter.criteria(tech.partsAssigned[count]))
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