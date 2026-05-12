using HarmonyLib;
using KSP.UI.Screens.DebugToolbar.Screens.Physics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(ScreenPhysics))]
    internal class PatchScreenPhysics
    {
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        internal static void Prefix_Start(ScreenPhysics __instance)
        {
            var containerObject = Object.Instantiate(__instance.legacyOrbitTargetingToggle.transform.parent.gameObject, __instance.transform);
            var toggle = containerObject.GetComponentInChildren<Toggle>();
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(OnPersistentRotationToggled);
            toggle.isOn = !VesselModuleRotationRO.ForceDisabled;
            containerObject.GetComponentInChildren<TextMeshProUGUI>().text = "Persistent Rotation";
        }

        private static void OnPersistentRotationToggled(bool on)
        {
            VesselModuleRotationRO.ForceDisabled = !on;
        }
    }
}
