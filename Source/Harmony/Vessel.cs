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
            if (setPos)
            {
                vmr.SetPosRot(rotation, __instance.transform.position);
                return false;
            }

            if (!__instance.loaded)
            {
                __instance.vesselTransform.rotation = rotation;
                return false;
            }

            var parts = __instance.parts;
            // Have to iterate forwards to preserve hierarchy
            int c = parts.Count;
            for (int i = 0; i < c; ++i)
            {
                Part part = parts[i];
                part.partTransform.rotation = rotation * part.orgRot;
            }

            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Vessel.Awake))]
        internal static void Postfix_Awake(Vessel __instance)
        {
            // will reorder vessel module list if needed.
            VesselModuleRotationRO.GetModule(__instance);
        }
    }
}
