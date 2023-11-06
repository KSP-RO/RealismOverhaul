using HarmonyLib;
using UnityEngine;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(Vessel))]
    internal class PatchVessel
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Vessel.SetRotation), typeof(Quaternion), typeof(bool))]
        internal static bool Prefix_SetRotation(Vessel __instance, Quaternion rotation, bool setPos)
        {
            VesselModuleRotationRO vmr = VesselModuleRotationRO.GetModule(__instance);
            if (vmr == null)
                return true;

            vmr.StoreRotAndTime(rotation);
            if (!setPos)
                return true;

            vmr.SetPosRot(rotation, __instance.transform.position);

            return false;
        }
    }
}
