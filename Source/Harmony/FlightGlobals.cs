using HarmonyLib;
using UnityEngine;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(FlightGlobals))]
    internal class PatchFlightGlobals
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(FlightGlobals.OnSceneChange))]
        internal static void Postfix_OnSceneChange()
        {
            VesselModuleRotationRO.ClearCache();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FlightGlobals.RemoveVessel))]
        internal static void Postfix_RemoveVessel(Vessel vessel)
        {
            VesselModuleRotationRO.RemoveVessel(vessel);
        }
    }
}
