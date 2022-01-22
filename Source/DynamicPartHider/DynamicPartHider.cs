using UnityEngine;
using KSP.UI.Screens;
using System;
using RealFuels;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class DynamicPartHider : MonoBehaviour
    {
        public static EditorPartListFilter<AvailablePart> searchFilterParts;
        public static RDTechFilters.Filter RDFilter;
        private RealismOverhaulSpeculative specLevel;
        public void Start()
        {
            GameEvents.onLevelWasLoadedGUIReady.Add(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Add(new EventData<RDTechTree>.OnEvent(OnUpdateRnD));

            specLevel = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
            GameEvents.OnGameSettingsApplied.Add(onGameSettingsApplied);
            // Is this logging needed?
            Debug.Log($"[RealismOverhaulSpecLevel] Started specLevelhandler with level {specLevel}");
            AttachFilters();
        }

        public void Destroy()
        {
            GameEvents.onLevelWasLoadedGUIReady.Remove(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Remove(new EventData<RDTechTree>.OnEvent(OnUpdateRnD));

            GameEvents.OnGameSettingsApplied.Remove(onGameSettingsApplied);
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
            Debug.Log("[RealismOverhaulSpecLevel] Attached filters");
            string partFilterID = "SpeculativeFilter";
            Func<AvailablePart, bool> _criteria = (_aPart) => HelperFuncs.IsPartAvailable(_aPart);
            searchFilterParts = new EditorPartListFilter<AvailablePart>(partFilterID, _criteria);
            if (EditorPartList.Instance != null)
            {
                EditorPartList.Instance.ExcludeFilters.AddFilter(searchFilterParts);
            }

            RDFilter = new RDTechFilters.Filter(partFilterID, _criteria);
            RDTechFilters.Instance.filters.AddFilter(RDFilter);

            string rfFilterID = "SpeculativeRFFilter";
            Func<ConfigNode, bool> _filterRF = (_cfg) => HelperFuncs.IsRFConfigAvailable(_cfg);
            if (!ConfigFilters.Instance.configDisplayFilters.ContainsKey(rfFilterID))
                ConfigFilters.Instance.configDisplayFilters.Add(rfFilterID, _filterRF);
        }

        // Is this logging needed?
        private void onGameSettingsApplied()
        {
            RealismOverhaulSpeculative oldSpecLevel = specLevel;
            specLevel = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
            if (oldSpecLevel != specLevel)
            {
                Debug.Log($"[RealismOverhaulSpecLevel] Spec level changed from {oldSpecLevel} to {specLevel}");
            }
        }

        private void OnUpdateRnD(RDTechTree tree)
        {
            Debug.Log($"[RealismOverhaulSpecLevel] TechTree updated");
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