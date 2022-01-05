using UnityEngine;

namespace RealismOverhaul
{
    public static class SpecFuncs
    {
        public static bool IsPartAvailable(AvailablePart ap, int specLevel)
        {
            if (ap == null)
            {
                return false;
            }
            int level = SpecFuncs.GetSpecLevel(ap);
            if (level > specLevel)
            {
                Debug.Log($"[RealismOverhaulSpecLevel] Part excluded: {ap.name}, specLevel was {level}, compared to {specLevel}");
                return false;
            }
            return true;
        }

        public static void PruneRDNode(RDTech tech, int specLevel)
        {
            foreach (AvailablePart ap in tech.partsAssigned.ToArray())
            {
                Debug.Log($"[RealismOverhaulSpecLevel] RnD inner check: checked part {ap.name}, is available: {SpecFuncs.IsPartAvailable(ap, specLevel)}");
                if (!SpecFuncs.IsPartAvailable(ap, specLevel))
                {
                    tech.partsAssigned.Remove(ap);
                }
            }
        }

        public static int GetCompInt()
        {
            RealismOverhaulSettings _settings = HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>();
            RealismOverhaulspeculative setting = _settings.speculativeLevel;
            return SetSpecLevel(setting);
        }

        public static int SetSpecLevel(RealismOverhaulspeculative setting)
        {
            if (setting == RealismOverhaulspeculative.real)
            {
                return 0;
            }
            if (setting == RealismOverhaulspeculative.proposal)
            {
                return 1;
            }
            if (setting == RealismOverhaulspeculative.scifi)
            {
                return 2;
            }
            return -1;
        }

        public static int GetSpecLevel(AvailablePart ap)
        {
            string tagsString = ap.tags;
            if (tagsString.Contains("speclevelreal"))
            {
                return 0;
            }
            if (tagsString.Contains("speclevelproto"))
            {
                return 1;
            }
            if (tagsString.Contains("speclevelscifi"))
            {
                return 2;
            }
            return -1;
        }
    }
}