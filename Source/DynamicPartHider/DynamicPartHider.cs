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
        private SpeculativeLevel specLevel;

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
            Func<AvailablePart, bool> criteria = (aPart) => Utilities.IsPartAvailable(aPart);
            searchFilterParts = new EditorPartListFilter<AvailablePart>(partFilterID, criteria);
            if (EditorPartList.Instance != null)
            {
                EditorPartList.Instance.ExcludeFilters.AddFilter(searchFilterParts);
            }

            if (!RDTechFilters.Instance.filters.ContainsKey(partFilterID))
                RDTechFilters.Instance.filters.Add(partFilterID, criteria);

            string rfFilterID = "SpeculativeRFFilter";
            Func<ConfigNode, bool> filterRF = (cfg) => Utilities.IsRFConfigAvailable(cfg);
            if (!ConfigFilters.Instance.configDisplayFilters.ContainsKey(rfFilterID))
                ConfigFilters.Instance.configDisplayFilters.Add(rfFilterID, filterRF);
        }

        // Is this logging needed?
        private void onGameSettingsApplied()
        {
            SpeculativeLevel oldSpecLevel = specLevel;
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