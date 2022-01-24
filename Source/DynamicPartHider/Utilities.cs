using System;
using UnityEngine;

namespace RealismOverhaul
{
    public static class Utilities
    {
        public static bool IsPartAvailable(AvailablePart ap)
        {
            if (ap == null)
            {
                return false;
            }
            SpeculativeLevel specLevel = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
            SpeculativeLevel level = GetSpecLevelFromTags(ap);

            if (level > specLevel)
            {
                // TODO: Delete the debug print
                Debug.Log($"[RealismOverhaulSpecLevel] Part excluded: {ap.name}, specLevel was {level}, compared to {specLevel}");
                return false;
            }
            return true;
        }

        public static bool IsRFConfigAvailable(ConfigNode cfg)
        {
            if (cfg == null)
            {
                return false;
            }

            if (cfg.HasValue("specLevel"))
            {
                string value = cfg.GetValue("specLevel");
                if (SpeculativeLevel.TryParse(value, out SpeculativeLevel valueEnum))
                {
                    SpeculativeLevel specLevel = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
                    if (valueEnum > specLevel)
                    {
                        // TODO: Delete the debug print
                        Debug.Log($"[RealismOverhaulSpecLevel] Engine Config excluded: {cfg.GetValue("name")}, specLevel was {valueEnum}, compared to {specLevel}");
                        return false;
                    }
                    return true;
                }
                Debug.Log($"[RealismOverhaulSpecLevel] Parsing specLevel failed on Engine Config: {cfg.GetValue("name")} ");
            }

            return true;
        }

        public static SpeculativeLevel GetSpecLevelFromTags(AvailablePart ap)
        {
            string tagsString = ap.tags;
            if (tagsString.IndexOf("ro_speculativetag_real", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Real; }
            if (tagsString.IndexOf("ro_speculativetag_prototype", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Prototype; }
            if (tagsString.IndexOf("ro_speculativetag_concept", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Concept; }
            if (tagsString.IndexOf("ro_speculativetag_speculative", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Speculative; }
            if (tagsString.IndexOf("ro_speculativetag_fictional", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Fictional; }
            return SpeculativeLevel.Real;
        }
    }

    public enum SpeculativeLevel
    {
        Real,
        Prototype,
        Concept,
        Speculative,
        Fictional
    }
}