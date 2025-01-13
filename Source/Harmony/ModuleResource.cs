using HarmonyLib;
using ROUtils;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(ModuleResource))]
    internal class PatchModuleResource
    {
        [HarmonyPrefix]
        [HarmonyPatch("PrintRate", typeof(bool), typeof(double))]
        internal static bool Prefix_PrintRate(ModuleResource __instance, bool showFlowMode, double mult, ref string __result)
        {
            __result = ResourceUnits.PrintRate(__instance.rate * mult, __instance.id, showFlowMode, __instance);
            return false;
        }
    }
}
