using UnityEngine;
using KSP.UI.Screens;
using System;
using RealFuels;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class DynamicPartHider : MonoBehaviour
    {
        private string partFilterID = "SpeculativeFilter";
        private static Func<AvailablePart, bool> criteria = (aPart) => Utilities.IsPartAvailable(aPart);
        private string rfFilterID = "SpeculativeRFFilter";
        private static Func<ConfigNode, bool> filterRF = (cfg) => Utilities.IsRFConfigAvailable(cfg);

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
            if (scene == GameScenes.EDITOR)
            {
                AttachFilters();
            }
        }

        private void AttachFilters()
        {
            Debug.Log("[RODynamicPartHider] Attached filters");
            if (EditorPartList.Instance != null && EditorPartList.Instance.ExcludeFilters[partFilterID] == null)
                EditorPartList.Instance.ExcludeFilters.AddFilter(new EditorPartListFilter<AvailablePart>(partFilterID, criteria));

            if (!RDTechFilters.Instance.filters.ContainsKey(partFilterID))
                RDTechFilters.Instance.filters.Add(partFilterID, criteria);

            if (!ConfigFilters.Instance.configDisplayFilters.ContainsKey(rfFilterID))
                ConfigFilters.Instance.configDisplayFilters.Add(rfFilterID, filterRF);
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