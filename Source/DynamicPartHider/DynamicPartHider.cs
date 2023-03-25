using UnityEngine;
using KSP.UI.Screens;
using System;
using RealFuels;

namespace RealismOverhaul
{
    public class Filters
    {
        public class Filter
        {
            public string name;
            public Func<AvailablePart, bool> criteria;
            public Func<ConfigNode, bool> RFcriteria;
            public Filter(string name, Func<AvailablePart, bool> criteria, Func<ConfigNode, bool> RFcriteria)
            {
                this.name = name;
                this.criteria = criteria;
                this.RFcriteria = RFcriteria;
            }
        }

        private static Filter[] _filters;
        public static Filter[] Instance
        {
            get
            {
                if (_filters == null)
                {
                    Filter SciFi = new Filter("SpeculativeFilter", (aPart) => SciFiHider.IsPartAvailable(aPart), (cfg) => SciFiHider.IsRFConfigAvailable(cfg));
                    Filter Deprecated = new Filter("DeprecatedFilter", (aPart) => DeprecatedHider.IsPartAvailable(aPart), (cfg) => DeprecatedHider.IsRFConfigAvailable(cfg));
                    _filters = new Filter[] {SciFi, Deprecated};
                }
                return _filters;
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class DynamicPartHider : MonoBehaviour
    {
        public void Awake()
        {
            GameEvents.onLevelWasLoadedGUIReady.Add(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Add(OnUpdateRnD);
        }

        public void Start()
        {
            AttachFilters();
        }

        public void OnDestroy()
        {
            GameEvents.onLevelWasLoadedGUIReady.Remove(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Remove(OnUpdateRnD);
        }

        private void OnLevelLoaded(GameScenes scene)
        {
            Debug.Log("[RODynamicPartHider] Called OnLevelLoaded");
            if (scene == GameScenes.EDITOR)
            {
                Debug.Log("[RODynamicPartHider] Called AttachFilters");
                AttachFilters();
            }
        }

        private void AttachFilters()
        {
            Debug.Log("[RODynamicPartHider] Attached filters");
            foreach (Filters.Filter filter in Filters.Instance)
            {
                Debug.Log($"Attaching {filter.name}");
                Debug.Log($"instance is not null: {EditorPartList.Instance != null}");
                if (EditorPartList.Instance != null)
                    Debug.Log($"{filter.name} not in list: {EditorPartList.Instance.ExcludeFilters[filter.name] == null}");
                if (EditorPartList.Instance != null && EditorPartList.Instance.ExcludeFilters[filter.name] == null)
                {
                    Debug.Log($"Sucessfully attached {filter.name}");
                    EditorPartList.Instance.ExcludeFilters.AddFilter(new EditorPartListFilter<AvailablePart>(filter.name, filter.criteria));
                }

                if (!RDTechFilters.Instance.filters.ContainsKey(filter.name))
                    RDTechFilters.Instance.filters.Add(filter.name, filter.criteria);

                if (!ConfigFilters.Instance.configDisplayFilters.ContainsKey(filter.name))
                    ConfigFilters.Instance.configDisplayFilters.Add(filter.name, filter.RFcriteria);
            }
        }

        private void OnUpdateRnD(RDTechTree tree)
        {
            Debug.Log($"[RODynamicPartHider] TechTree updated");
            foreach (RDNode node in tree.controller.nodes)
            {
                // This filtering doesn't get persisted, but is done to avoid
                // confusion with other mods that interact with the tech tree
                RDTechFilters.Instance.FilterRDNode(node.tech);

                // Attach the actual filtering component that does the filtering
                node.gameObject.AddComponent<RDTechFixer>();
            }
        }
    }
}