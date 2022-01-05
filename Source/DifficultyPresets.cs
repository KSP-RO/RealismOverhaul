using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class DifficultyPresetChanger : MonoBehaviour
    {
        public void Awake()
        {
            ConfigNode paramsNode = null;
            foreach (ConfigNode n in GameDatabase.Instance.GetConfigNodes("GAMEPARAMETERS"))
                paramsNode = n;

            if (paramsNode == null)
            {
                Debug.LogError("[RealismOverhaul] Could not find GAMEPARAMETERS node.");
                return;
            }

            GameParameters.SetDifficultyPresets();

            foreach (KeyValuePair<GameParameters.Preset, GameParameters> kvp in GameParameters.DifficultyPresets)
            {
                ConfigNode n = paramsNode.GetNode(kvp.Key.ToString());
                if (n != null)
                    kvp.Value.Load(n);
            }

            Debug.Log("[RealismOverhaul] Reset difficulty presets.");
        }
    }

    public class RealismOverhaulSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "General Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "RealismOverhaul"; } }
        public override string DisplaySection { get { return "RealismOverhaul"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }

        [GameParameters.CustomParameterUI("Speculative Level", toolTip = "What parts are available.\nReal = Only real hardware is available.\nProposal = Real proposals that were never built are available.\nScifi = Scifi parts are available.")]
        public RealismOverhaulspeculative speculativeLevel = RealismOverhaulspeculative.REAL;
        

    }

    public enum RealismOverhaulspeculative
    {
        REAL,
        PROPOSAL,
        SCIFI
    }
}
