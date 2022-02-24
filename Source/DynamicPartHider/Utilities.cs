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
            SpeculativeLevel setting = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
            SpeculativeLevel level = GetSpecLevelFromTags(ap);

            if (level > setting)
            {
                // TODO: Delete the debug print
                Debug.Log($"[RODynamicPartHider] Part excluded: {ap.name}, specLevel was {level}, compared to {setting}");
                return false;
            }
            return true;
        }

        // Passed to RF to validate if a engine config should be available
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
                    SpeculativeLevel setting = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
                    if (valueEnum > setting)
                    {
                        // TODO: Delete the debug print
                        Debug.Log($"[RODynamicPartHider] Engine Config excluded: {cfg.GetValue("name")}, specLevel was {valueEnum}, compared to {setting}");
                        return false;
                    }
                    return true;
                }
                Debug.Log($"[RODynamicPartHider] Parsing specLevel failed on Engine Config: {cfg.GetValue("name")} ");
            }

            return true;
        }

        public static SpeculativeLevel GetSpecLevelFromTags(AvailablePart ap)
        {
            string tagsString = ap.tags;
            if (tagsString.IndexOf("ro_specleveltag_real", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Real; }
            if (tagsString.IndexOf("ro_specleveltag_prototype", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Prototype; }
            if (tagsString.IndexOf("ro_specleveltag_concept", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Concept; }
            if (tagsString.IndexOf("ro_specleveltag_speculative", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Speculative; }
            if (tagsString.IndexOf("ro_specleveltag_fictional", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Fictional; }
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