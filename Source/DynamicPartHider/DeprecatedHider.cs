using System;
using UnityEngine;

namespace RealismOverhaul
{
    public static class DeprecatedHider
    {
        public static bool IsPartAvailable(AvailablePart ap)
        {
            if (ap == null)
                return false;

            if (HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().showDeprecated)
                return true;

            // Invert because the config says if it IS deprecated, the function wants NOT deprecated
            return !(ap.tags.IndexOf("ro_deprecated", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // Passed to RF to validate if a engine config should be available
        public static bool IsRFConfigAvailable(ConfigNode cfg)
        {
            if (cfg == null)
                return false;

            string value = null;
            if (HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().showDeprecated)
                return true;

            if (cfg.TryGetValue("RODeprecated", ref value))
            {
                if (Boolean.TryParse(value, out bool result))
                {
                    // Invert because the config says if it IS deprecated, the function wants NOT deprecated
                    return !result;
                }
            }
            return true;
        }
    }
}