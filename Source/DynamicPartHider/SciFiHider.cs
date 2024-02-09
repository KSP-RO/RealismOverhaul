using System;
using UnityEngine;

namespace RealismOverhaul
{
    public class SciFiHider : Filters.IFilter
    {
        public string Name => "SpeculativeFilter";

        public bool IsPartAvailable(AvailablePart ap)
        {
            if (ap == null)
            {
                return false;
            }
            SpeculativeLevel setting = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
            SpeculativeLevel level = GetSpecLevelFromTags(ap);

            return level <= setting;
        }

        // Passed to RF to validate if a engine config should be available
        public bool IsRFConfigAvailable(ConfigNode cfg)
        {
            if (cfg == null)
                return false;

            string value = null;
            if (cfg.TryGetValue("specLevel", ref value))
            {
                if (SpeculativeLevel.TryParse(value, true, out SpeculativeLevel valueEnum))
                {
                    SpeculativeLevel setting = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
                    return valueEnum <= setting;
                }
                Debug.Log($"[RODynamicPartHider] Parsing specLevel failed on Engine Config: {cfg.GetValue("name")} ");
            }

            return true;
        }

        public static SpeculativeLevel GetSpecLevelFromTags(AvailablePart ap)
        {
            string tagsString = ap.tags;
            if (tagsString.IndexOf("ro_specleveltag_operational", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Operational; }
            if (tagsString.IndexOf("ro_specleveltag_prototype", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Prototype; }
            if (tagsString.IndexOf("ro_specleveltag_concept", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Concept; }
            if (tagsString.IndexOf("ro_specleveltag_speculative", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Speculative; }
            if (tagsString.IndexOf("ro_specleveltag_althist", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.AltHist; }
            if (tagsString.IndexOf("ro_specleveltag_scifi", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.SciFi; }
            return SpeculativeLevel.Operational;
        }

        public bool IsUpgradeAvaliable(PartUpgradeHandler.Upgrade upgrade)
        {
            if (upgrade == null)
            {
                return false;
            }
            SpeculativeLevel setting = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().speculativeLevel;
            SpeculativeLevel level = GetSpeculativeLevelFromUpgradeDescription(upgrade);

            return level <= setting;
        }

        public static SpeculativeLevel GetSpeculativeLevelFromUpgradeDescription(PartUpgradeHandler.Upgrade upgrade)
        {
            string description = upgrade.description;
            if (description.IndexOf("available at specLevel operational", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Operational; }
            if (description.IndexOf("available at specLevel prototype", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Prototype; }
            if (description.IndexOf("available at specLevel concept", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Concept; }
            if (description.IndexOf("available at specLevel speculative", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.Speculative; }
            if (description.IndexOf("available at specLevel althist", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.AltHist; }
            if (description.IndexOf("available at specLevel scifi", StringComparison.OrdinalIgnoreCase) >= 0) { return SpeculativeLevel.SciFi; }
            return SpeculativeLevel.Operational;
        }
    }

    public enum SpeculativeLevel
    {
        Operational,
        Prototype,
        Concept,
        Speculative,
        AltHist,
        SciFi
    }
}