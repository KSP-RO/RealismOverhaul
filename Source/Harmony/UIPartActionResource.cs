using HarmonyLib;
using ROUtils;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(UIPartActionResource))]
    internal class PatchUIPartActionResource
    {
        [HarmonyPostfix]
        [HarmonyPatch("Setup")]
        internal static void Postfix_Setup(UIPartActionResource __instance)
        {
            int resID = __instance.resource.info._id;
            __instance.resourceMax.text = ResourceUnits.PrintAmount(__instance.resource.maxAmount, resID, 3, __instance.resource.maxAmount < 100 ? "F2" : "F0");
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateItem")]
        internal static bool Prefix_UpdateItem(UIPartActionResource __instance)
        {
            int resID = __instance.resource.info._id;

            if (__instance.flowBtn.state != __instance.resource.flowState)
            {
                __instance.SetButtonState(__instance.resource.flowState, true);
            }

            __instance.resourceAmnt.text = ResourceUnits.PrintAmount(__instance.resource.amount, resID, 3, __instance.resource.amount < 100 ? "F2" : "F0");
            __instance.resourceMax.text = ResourceUnits.PrintAmount(__instance.resource.maxAmount, resID, 3, __instance.resource.maxAmount < 100 ? "F2" : "F0");
            __instance.progBar.value = (float)(__instance.resource.amount / __instance.resource.maxAmount);

            return false;
        }
    }
}
