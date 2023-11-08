using HarmonyLib;
using ROUtils;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(KSPUtil))]
    internal class PatchKSPUtil
    {
        [HarmonyPrefix]
        [HarmonyPatch("PrintSI")]
        internal static bool Prefix_PrintSI(double amount, string unitName, int sigFigs, bool longPrefix, ref string __result)
        {
            __result = ResourceUnits.PrintSI(amount, unitName, sigFigs, longPrefix);
            return false;
        }
    }
}
