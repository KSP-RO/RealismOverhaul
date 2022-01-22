using System.Linq;
using UnityEngine;

namespace RealismOverhaul
{
    public static class SpecFuncs
    {
        public static bool IsPartAvailable(AvailablePart ap)
        {
            if (ap == null)
            {
                return false;
            }
            var specLevel = GetSpecLevelSetting();
            RealismOverhaulSpeculative level = GetSpecLevelFromTags(ap);

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
                if(RealismOverhaulSpeculative.TryParse(value, out RealismOverhaulSpeculative valueEnum))
                {
                    var specLevel = GetSpecLevelSetting();
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

        public static RealismOverhaulSpeculative GetSpecLevelSetting()
        {
            RealismOverhaulSettings _settings = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>();
            return _settings.speculativeLevel;
        }

        public static RealismOverhaulSpeculative GetSpecLevelFromTags(AvailablePart ap)
        {
            string tagsString = ap.tags;
            if (tagsString.Contains("ro_speculativetag_real")) { return RealismOverhaulSpeculative.real; }
            if (tagsString.Contains("ro_speculativetag_prototype")) { return RealismOverhaulSpeculative.prototype; }
            if (tagsString.Contains("ro_speculativetag_concept")) { return RealismOverhaulSpeculative.concept; }
            if (tagsString.Contains("ro_speculativetag_speculative")) { return RealismOverhaulSpeculative.speculative; }
            if (tagsString.Contains("ro_speculativetag_fictional")) { return RealismOverhaulSpeculative.fictional; }
            return RealismOverhaulSpeculative.real;
        }
    }

    public enum RealismOverhaulSpeculative
    {
        real,
        prototype,
        concept,
        speculative,
        fictional
    }
}