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
            VesselModuleRotationRO mod = null;
            foreach (var vm in __instance.vesselModules)
            {
                if (vm is VesselModuleRotationRO vmr)
                {
                    mod = vmr;
                    break;
                }
            }
            if (mod == null)
                return true;

            mod.StoreRot(rotation);
            if (!setPos)
                return true;

            mod.SetPosRot(rotation, __instance.transform.position);

            return false;
        }
    }
}
