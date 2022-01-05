using UnityEngine;
using KSP.UI.Screens;
using System;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class SpeculativeHider : MonoBehaviour
    {
        public static EditorPartListFilter<AvailablePart> searchFilterParts;
        private int specLevel;
        public void Start()
        {
            GameEvents.onLevelWasLoadedGUIReady.Add(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Add(new EventData<RDTechTree>.OnEvent(OnUpdateRnD));

            specLevel = SpecFuncs.GetCompInt();
            GameEvents.OnGameSettingsApplied.Add(OnSpecLevelChanged);
            Debug.Log($"[RealismOverhaulSpecLevel] Started specLevelhandler with level {specLevel}");
        }

        public void Destroy()
        {
            GameEvents.onLevelWasLoadedGUIReady.Remove(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Remove(new EventData<RDTechTree>.OnEvent(OnUpdateRnD));
        }

        public void OnLevelLoaded(GameScenes scene)
        {
            Debug.Log("[RealismOverhaulSpecLevel] Level Loaded");
            if (scene == GameScenes.EDITOR)
            {
                specLevel = SpecFuncs.GetCompInt();
                Func<AvailablePart, bool> _criteria = (_aPart) => SpecFuncs.IsPartAvailable(_aPart, specLevel);
                searchFilterParts = new EditorPartListFilter<AvailablePart>("SpeculativeLevel", _criteria);
                EditorPartList.Instance.ExcludeFilters.AddFilter(searchFilterParts);
            }
        }

        void OnSpecLevelChanged()
        {
            int oldCompInt = specLevel;
            specLevel = SpecFuncs.GetCompInt();
            Debug.Log($"[RealismOverhaulSpecLevel] Spec level changed from {oldCompInt} to {specLevel}");
        }

        void OnUpdateRnD(RDTechTree tree)
        {
            Debug.Log($"[RealismOverhaulSpecLevel] TechTree updated");
            foreach (RDNode node in tree.controller.nodes)
            {
                SpecFuncs.PruneRDNode(node.tech, specLevel);
                node.gameObject.AddComponent<RDTechFixer>();
            }
        }
    }
}