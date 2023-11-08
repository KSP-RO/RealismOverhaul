using HarmonyLib;
using KSP.Localization;
using ROUtils;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(ModuleEngines))]
    internal class PatchModuleEngines
    {
        private static string GetInfo(ModuleEngines mod)
        {
            string output = mod.GetInfoThrust(mainInfoWindow: true);
            output += Localizer.Format("#autoLOC_220745", mod.atmosphereCurve.Evaluate(1f).ToString("0.###"), mod.atmosphereCurve.Evaluate(0f).ToString("0.###"));
            output += ModuleEngines.cacheAutoLOC_220748;
            double massFlow = 0d;
            foreach (var propellant in mod.propellants)
            {
                double flow = mod.getMaxFuelFlow(propellant);
                massFlow += flow * propellant.resourceDef.density;
                output += ResourceUnits.PrintRate(flow, propellant.id, true, null, propellant, true);
            }
            if (massFlow > 0d)
                output += Localizer.Format("#autoLOC_900654") + " " + Localizer.Format(ResourceUnits.PerSecLocString, ResourceUnits.PrintMass(massFlow)) + "\n";

            output += Localizer.Format("#autoLOC_220759", (mod.ignitionThreshold * 100f).ToString("0.#"));
            if (!mod.allowShutdown)
            {
                output += ModuleEngines.cacheAutoLOC_220761;
            }
            if (!mod.allowRestart)
            {
                output += ModuleEngines.cacheAutoLOC_220762;
            }
            return output;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetInfo")]
        internal static bool Prefix_GetInfo(ModuleEngines __instance, ref string __result)
        {
            __result = GetInfo(__instance);
            return false;
        }
    }
}
