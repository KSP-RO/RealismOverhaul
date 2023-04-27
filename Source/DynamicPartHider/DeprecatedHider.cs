using System;
using UnityEngine;

namespace RealismOverhaul
{
    public class DeprecatedHider : Filters.IFilter
    {
        public string Name => "DeprecatedFilter";

        public Func<AvailablePart, bool> IsPartAvailable =>
        (AvailablePart ap) =>
        {
            if (ap == null)
                return false;

            if (HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().showDeprecated)
                return true;

            // Check for not present because the config says if it IS deprecated, the function wants NOT deprecated
            return ap.tags.IndexOf("ro_deprecated", StringComparison.OrdinalIgnoreCase) < 0;
        };

        // Passed to RF to validate if a engine config should be available
        public Func<ConfigNode, bool> IsRFConfigAvailable =>
        (ConfigNode cfg) =>
        {
            if (cfg == null)
                return false;

            if (HighLogic.CurrentGame.Parameters.CustomParams<RealismOverhaulSettings>().showDeprecated)
                return true;

            bool value = false;
            if (cfg.TryGetValue("RODeprecated", ref value))
            {
                // Invert because the config says if it IS deprecated, the function wants NOT deprecated
                return !value;
            }
            return true;
        };
    }
}