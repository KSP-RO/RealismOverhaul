using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(ModuleGimbal))]
    internal class PatchModuleGimbal
    {
        private static readonly MethodInfo _updatePAW = typeof(ModuleGimbal).GetMethod("UpdatePAWUI", AccessTools.all);

        internal static void UpdatePAW(ModuleGimbal mod)
        {
            bool hasGimbal = mod.gimbalRange > 0 || mod.gimbalRangeXN > 0 || mod.gimbalRangeXP > 0 || mod.gimbalRangeYN > 0 || mod.gimbalRangeYP > 0;
            if (!hasGimbal)
            {
                bool oldToggles = mod.showToggles;
                mod.showToggles = false;
                bool oldEnabled = mod.moduleIsEnabled;
                mod.moduleIsEnabled = false;
                // leave this alone so that if we come back to enabled,
                // the state is preserved -- mod.currentShowToggles = false;
                _updatePAW.Invoke(mod, null);
                // restore normal state now that we've hidden things.
                mod.showToggles = oldToggles;
                mod.moduleIsEnabled = oldEnabled;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnLoad")]
        internal static void Postfix_OnLoad(ModuleGimbal __instance)
        {
            UpdatePAW(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnStart")]
        internal static void Postfix_OnStart(ModuleGimbal __instance)
        {
            UpdatePAW(__instance);
        }
    }
}
