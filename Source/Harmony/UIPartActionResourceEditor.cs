using HarmonyLib;
using UnityEngine;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(UIPartActionResourceEditor))]
    internal class PatchUIPartActionResourceEditor
    {
        [HarmonyPostfix]
        [HarmonyPatch("Setup")]
        internal static void Postfix_Setup(UIPartActionResource __instance)
        {
            int resID = __instance.resource.info._id;
            __instance.resourceMax.text = ResourceUnits.PrintAmount(__instance.resource.maxAmount, resID, 3, __instance.resource.maxAmount < 100 ? "F2" : "F0");
        }

        [HarmonyPrefix]
        [HarmonyPatch("onSliderChangeProcess")]
        internal static bool Prefix_onSliderChangeProcess(UIPartActionResourceEditor __instance)
        {
            int resID = __instance.resource.info._id;

            float num = __instance.slider.value;
            if (!__instance.bypassSliderRounding)
            {
                num = Mathf.Round(__instance.slider.value * 10f) / 10f;
            }
            __instance.bypassSliderRounding = false;
            __instance.resource.amount = (double)num * __instance.resource.maxAmount;
            if (__instance.scene == UI_Scene.Editor)
            {
                __instance.SetSymCounterpartsAmount(__instance.resource.amount);
            }

            __instance.resourceAmnt.text = ResourceUnits.PrintAmount(__instance.resource.amount, resID, 3, "F1");

            return false;
        }
    }
}
