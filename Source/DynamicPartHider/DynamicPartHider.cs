using UnityEngine;
using KSP.UI.Screens;
using System;
using RealFuels;

namespace RealismOverhaul
{
    public class Filters
    {
        public interface IFilter
        {
            public string Name { get; }

            public bool IsPartAvailable(AvailablePart ap);

            public bool IsRFConfigAvailable(ConfigNode cfg);

            public bool IsUpgradeAvaliable(PartUpgradeHandler.Upgrade upgrade);
        }

        private static IFilter[] _filters;

        public static IFilter[] Instance
        {
            get
            {
                _filters ??= new IFilter[]
                {
                    new SciFiHider(),
                    new DeprecatedHider(),
                };
                return _filters;
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class DynamicPartHider : MonoBehaviour
    {
        public void Awake()
        {
            GameEvents.onLevelWasLoadedGUIReady.Add(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Add(OnUpdateRnD);
        }

        public void Start()
        {
            DontDestroyOnLoad(this);
        }

        public void OnDestroy()
        {
            GameEvents.onLevelWasLoadedGUIReady.Remove(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Remove(OnUpdateRnD);
        }

        private void OnLevelLoaded(GameScenes scene)
        {
            if (scene == GameScenes.EDITOR)
            {
                AttachFilters();
            }
        }

        private void AttachFilters()
        {
            Debug.Log("[RODynamicPartHider] Attached filters");
            foreach (Filters.IFilter filter in Filters.Instance)
            {
                if (EditorPartList.Instance != null && EditorPartList.Instance.ExcludeFilters[filter.Name] == null)
                    EditorPartList.Instance.ExcludeFilters.AddFilter(new EditorPartListFilter<AvailablePart>(filter.Name, filter.IsPartAvailable));

                if (!RDTechFilters.Instance.filters.ContainsKey(filter.Name))
                    RDTechFilters.Instance.filters.Add(filter.Name, filter.IsPartAvailable);

                if (!ConfigFilters.Instance.configDisplayFilters.ContainsKey(filter.Name))
                    ConfigFilters.Instance.configDisplayFilters.Add(filter.Name, filter.IsRFConfigAvailable);
            }
        }

        private void OnUpdateRnD(RDTechTree tree)
        {
            AttachFilters();
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