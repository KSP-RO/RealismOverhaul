using System.Linq;
using UnityEngine;

namespace RealismOverhaul
{
    public static class SpecFuncs
    {
        public static bool IsPartAvailable(AvailablePart ap, RealismOverhaulSpeculative specLevel)
        {
            if (ap == null)
            {
                return false;
            }
            RealismOverhaulSpeculative level = GetSpecLevelFromTags(ap);

            var module = ap.partPrefab.Modules.GetModule<SpeculativePartModule>();
            level = module != null ? module.specLevel : RealismOverhaulSpeculative.real;

            if (level > specLevel)
            {
                Debug.Log($"[RealismOverhaulSpecLevel] Part excluded: {ap.name}, specLevel was {level}, compared to {specLevel}");
                return false;
            }
            return true;
        }

        public static bool IsRFConfigAvailable(ConfigNode cfg, RealismOverhaulSpeculative specLevel)
        {
            if (cfg == null)
            {
                return false;
            }

            // TODO: Implement actual checking

            return true;
        }

        public static void PruneRDNode(RDTech tech, RealismOverhaulSpeculative specLevel)
        {
            foreach (AvailablePart ap in tech.partsAssigned.Where(ap => !IsPartAvailable(ap, specLevel)).ToArray())
            {
                Debug.Log($"[RealismOverhaulSpecLevel] RnD inner check: checked part {ap.name}, is available: {IsPartAvailable(ap, specLevel)}");
                tech.partsAssigned.Remove(ap);
            }
        }

        public static RealismOverhaulSpeculative GetSpecLevelSetting()
        {
            RealismOverhaulSettings _settings = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>();
            return _settings.speculativeLevel;
        }

        public static RealismOverhaulSpeculative GetSpecLevelFromTags(AvailablePart ap)
        {
            string tagsString = ap.tags;
            if (tagsString.Contains("speclevelreal")) { return RealismOverhaulSpeculative.real; }
            if (tagsString.Contains("speclevelprototype")) { return RealismOverhaulSpeculative.prototype; }
            if (tagsString.Contains("speclevelconcept")) { return RealismOverhaulSpeculative.concept; }
            if (tagsString.Contains("speclevelspeculative")) { return RealismOverhaulSpeculative.speculative; }
            if (tagsString.Contains("speclevelfictional")) { return RealismOverhaulSpeculative.fictional; }
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