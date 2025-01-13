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

        [HarmonyPostfix]
        [HarmonyPatch("CheckVersion", typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int))]
        internal static void CheckVersion(int version_major, int version_minor, int version_revision, int lastMajor, int lastMinor, int lastRev, ref VersionCompareResult __result)
        {
            // it's silly to differentiate between KSP 1.12 revisions
            if (__result == VersionCompareResult.INCOMPATIBLE_TOO_LATE &&
                version_major == 1 && version_minor == 12)
            {
                __result = VersionCompareResult.COMPATIBLE;
            }
        }
    }
}
