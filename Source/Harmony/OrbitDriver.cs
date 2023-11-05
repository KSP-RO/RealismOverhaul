using HarmonyLib;
using UnityEngine;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(OrbitDriver))]
    internal class PatchOrbitDriver
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(OrbitDriver.updateFromParameters), typeof(bool))]
        internal static bool Prefix_updateFromParameters(OrbitDriver __instance, bool setPosition)
        {
            if (!setPosition
                || !VesselModuleRotationRO.IsEnabled
                || __instance.vessel == null
                || (__instance.vessel.loaded &&  !__instance.vessel.packed)
                || (VesselModuleRotationRO.IgnoreRot(__instance.vessel)))
                return true;

            __instance.updateUT = Planetarium.GetUniversalTime();
            __instance.orbit.UpdateFromUT(__instance.updateUT);
            __instance.pos = __instance.orbit.pos;
            __instance.vel = __instance.orbit.vel;
            __instance.pos.Swizzle();
            __instance.vel.Swizzle();
            if (double.IsNaN(__instance.pos.x))
            {
                MonoBehaviour.print("ObT : " + __instance.orbit.ObT + "\nM : " + __instance.orbit.meanAnomaly + "\nE : " + __instance.orbit.eccentricAnomaly + "\nV : "
                    + __instance.orbit.trueAnomaly + "\nRadius: " + __instance.orbit.radius + "\nvel: " + __instance.vel.ToString() + "\nAN: " + __instance.orbit.an.ToString()
                    + "\nperiod: " + __instance.orbit.period + "\n");

                Debug.LogWarning("[OrbitDriver Warning!]: " + __instance.vessel.GetDisplayName() + " had a NaN Orbit and was removed.");
                __instance.vessel.Unload();
                UnityEngine.Object.Destroy(__instance.vessel.gameObject);
                return false;
            }
            VesselModuleRotationRO mod = null;
            foreach (var vm in __instance.vessel.vesselModules)
            {
                if (vm is VesselModuleRotationRO vmr)
                {
                    mod = vmr;
                    break;
                }
            }
            if (!__instance.reverse)
            {
                Vector3d offset = (QuaternionD)__instance.driverTransform.rotation * (Vector3d)__instance.vessel.localCoM;
                Vector3d pos = __instance.referenceBody.position + __instance.pos - offset;
                if (mod != null)
                    mod.RailsUpdate(pos);
                else
                    __instance.vessel.SetPosition(pos);
            }
            else
            {
                __instance.referenceBody.position = ((Vector3d)__instance.driverTransform.position) - __instance.pos;
                if (mod != null)
                    mod.RailsUpdate(__instance.vessel.vesselTransform.position);
            }

            return false;
        }
    }
}
