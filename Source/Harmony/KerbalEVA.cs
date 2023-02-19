using HarmonyLib;
using UnityEngine;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(KerbalEVA))]
    internal class PatchKerbalEVA
    {
        [HarmonyPostfix]
        [HarmonyPatch("HandleMovementInput")]
        internal static void Postfix_HandleMovementInput(KerbalEVA __instance, ref Vector3 ___parachuteInput)
        {
            if (__instance.vessel.state == Vessel.State.ACTIVE && ___parachuteInput.sqrMagnitude > 0)    // Do not override players inputs
                return;

            if (CrewEjectionHandler.TryGetInstanceForKerbalEVA(__instance, out CrewEjectionHandler handler) &&
                handler.ShouldIncreaseHorizontalSpeed())
            {
                ___parachuteInput = new Vector3(1, 0, 0);    // Same as pressing the 'W' key
            }
            else
            {
                ___parachuteInput = Vector3.zero;
            }
        }
    }
}
