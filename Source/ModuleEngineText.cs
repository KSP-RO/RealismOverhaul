using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealismOverhaul
{
    public class ModuleEngineText : PartModule
    {
        private const string activateStr = "#roEngineActivate";
        private const string shutdownStr = "#roEngineShutdown";
        private const string toggleStr = "#roEngineToggle";

        public List<string> engineIDs = new List<string>();
        public List<string> engineNames = new List<string>();

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            ConfigNode n = node.GetNode("NAMES");
            if (n != null)
            {
                engineIDs.Clear();
                engineNames.Clear();
                foreach (ConfigNode.Value v in n.values)
                {
                    engineIDs.Add(v.name);
                    engineNames.Add(v.value);
                }
            }
            else
            {
                if (engineIDs.Count == 0 && part.partInfo != null && part.partInfo.partPrefab != null)
                {
                    ModuleEngineText met = part.partInfo.partPrefab.FindModuleImplementing<ModuleEngineText>();
                    if (met != null)
                    {
                        engineIDs.AddRange(met.engineIDs);
                        engineNames.AddRange(met.engineNames);
                    }
                }
            }

            if (engineIDs.Count == 0)
                return;

            List<ModuleEngines> engines = part.FindModulesImplementing<ModuleEngines>();
            foreach (var engine in engines)
            {
                for (int i = 0; i < engineIDs.Count; ++i)
                {
                    if (engineIDs[i] == engine.engineID)
                    {
                        UpdateStrings(engine, KSP.Localization.Localizer.GetStringByTag(engineNames[i]));
                        break;
                    }
                }
            }
        }

        private void UpdateStrings(ModuleEngines e, string localizedName)
        {
            e.Actions["ActivateAction"].guiName = e.Events["Activate"].guiName = KSP.Localization.Localizer.Format(activateStr, localizedName);
            e.Actions["ShutdownAction"].guiName = e.Events["Shutdown"].guiName = KSP.Localization.Localizer.Format(shutdownStr, localizedName);
            e.Actions["OnAction"].guiName = KSP.Localization.Localizer.Format(toggleStr, localizedName);
        }
    }
}
