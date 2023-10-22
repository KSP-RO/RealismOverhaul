using HarmonyLib;
using KSP.Localization;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(ModuleRCS))]
    internal class PatchModuleRCS
    {
        private static string GetInfo(ModuleRCS mod)
        {
            string output = Localizer.Format("#autoLOC_217936", mod.thrusterPower.ToString("0.0###"));
            output += Localizer.Format("#autoLOC_217937", mod.atmosphereCurve.Evaluate(1f).ToString("0.###"), mod.atmosphereCurve.Evaluate(0f).ToString("0.###"));
            output += ModuleRCS.cacheAutoLOC_217939;
            double massFlow = 0d;
            foreach (var propellant in mod.propellants)
            {
                double flow = mod.getMaxFuelFlow(propellant, mod.atmosphereCurve.Evaluate(0f));
                massFlow += flow * propellant.resourceDef.density;
                output += ResourceUnits.PrintRate(flow, propellant.id, false, null, propellant, true);
            }
            if (massFlow > 0d)
                output += Localizer.Format("#autoLOC_900654") + " " + Localizer.Format("#autoLOC_6001048", ResourceUnits.PrintMassRate(massFlow)) + "\n";

            if (!mod.moduleIsEnabled)
            {
                output += ModuleRCS.cacheAutoLOC_217950;
            }
            return output;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetInfo")]
        internal static bool Prefix_GetInfo(ModuleRCS __instance, ref string __result)
        {
            __result = GetInfo(__instance);
            return false;
        }
    }
}
