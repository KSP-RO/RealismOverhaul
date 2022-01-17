using UnityEngine;
using KSP.UI.Screens;
using System;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class SpeculativeHider : MonoBehaviour
    {
        public static EditorPartListFilter<AvailablePart> searchFilterParts;
        private RealismOverhaulSpeculative specLevel;
        public void Start()
        {
            GameEvents.onLevelWasLoadedGUIReady.Add(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Add(new EventData<RDTechTree>.OnEvent(OnUpdateRnD));

            specLevel = SpecFuncs.GetSpecLevelSetting();
            GameEvents.OnGameSettingsApplied.Add(OnSpecLevelChanged);
            Debug.Log($"[RealismOverhaulSpecLevel] Started specLevelhandler with level {specLevel}");
        }

        public void Destroy()
        {
            GameEvents.onLevelWasLoadedGUIReady.Remove(OnLevelLoaded);
            RDTechTree.OnTechTreeSpawn.Remove(new EventData<RDTechTree>.OnEvent(OnUpdateRnD));

            GameEvents.OnGameSettingsApplied.Remove(OnSpecLevelChanged);
        }

        public void OnLevelLoaded(GameScenes scene)
        {
            if (scene == GameScenes.EDITOR)
            {
                specLevel = SpecFuncs.GetSpecLevelSetting();
                Func<AvailablePart, bool> _criteria = (_aPart) => SpecFuncs.IsPartAvailable(_aPart, specLevel);
                searchFilterParts = new EditorPartListFilter<AvailablePart>("SpeculativeLevel", _criteria);
                EditorPartList.Instance.ExcludeFilters.AddFilter(searchFilterParts);
            }
        }

        void OnSpecLevelChanged()
        {
            RealismOverhaulSpeculative oldSpecLevel = specLevel;
            specLevel = SpecFuncs.GetSpecLevelSetting();
            if (oldSpecLevel != specLevel)
            {
                Debug.Log($"[RealismOverhaulSpecLevel] Spec level changed from {oldSpecLevel} to {specLevel}");
            }
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