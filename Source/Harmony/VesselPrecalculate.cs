using HarmonyLib;
using UnityEngine;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(VesselPrecalculate))]
    internal class PatchVesselPrecalculate
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(VesselPrecalculate.SetLandedPosRot))]
        internal static bool Prefix_SetLandedPosRot(VesselPrecalculate __instance)
        {
            Vessel vessel = __instance.vessel;
            if (vessel.LandedOrSplashed && vessel.packed && __instance.railsSetPosRot)
            {
                __instance.worldSurfaceRot = vessel.mainBody.bodyTransform.rotation * vessel.srfRelRotation;
                __instance.worldSurfacePos = vessel.mainBody.GetWorldSurfacePosition(vessel.latitude, vessel.longitude, vessel.altitude);
                VesselModuleRotationRO vmr = VesselModuleRotationRO.GetModule(vessel);
                if (vmr == null)
                {
                    vessel.SetRotation(__instance.worldSurfaceRot, false);
                    vessel.SetPosition(__instance.worldSurfacePos, true);
                }
                else
                {
                    vmr.StoreRotAndTime(__instance.worldSurfaceRot);
                    vmr.SetPosRot(__instance.worldSurfaceRot, __instance.worldSurfacePos);
                }
            }
            return false;
        }
    }
}
