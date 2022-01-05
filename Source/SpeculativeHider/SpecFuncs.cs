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
            RealismOverhaulSpeculative level = GetSpecLevel(ap);
            if (level > specLevel)
            {
                Debug.Log($"[RealismOverhaulSpecLevel] Part excluded: {ap.name}, specLevel was {level}, compared to {specLevel}");
                return false;
            }
            return true;
        }

        public static void PruneRDNode(RDTech tech, RealismOverhaulSpeculative specLevel)
        {
            foreach (AvailablePart ap in tech.partsAssigned.ToArray())
            {
                Debug.Log($"[RealismOverhaulSpecLevel] RnD inner check: checked part {ap.name}, is available: {IsPartAvailable(ap, specLevel)}");
                if (!IsPartAvailable(ap, specLevel))
                {
                    tech.partsAssigned.Remove(ap);
                }
            }
        }

        public static RealismOverhaulSpeculative GetSpecLevelSetting()
        {
            RealismOverhaulSettings _settings = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>();
            RealismOverhaulSpeculative setting = _settings.speculativeLevel;
            return setting;
        }

        public static RealismOverhaulSpeculative GetSpecLevel(AvailablePart ap)
        {
            string tagsString = ap.tags;
            if (tagsString.Contains("speclevelreal")) { return RealismOverhaulSpeculative.real; }
            if (tagsString.Contains("speclevelproto")) { return RealismOverhaulSpeculative.proposal; }
            if (tagsString.Contains("speclevelscifi")) { return RealismOverhaulSpeculative.scifi; }
            return RealismOverhaulSpeculative.none;
        }
    }
}