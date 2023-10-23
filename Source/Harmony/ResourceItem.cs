using HarmonyLib;
using KSP.Localization;
using KSP.UI.Screens;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(ResourceItem))]
    internal class PatchResourceItem
    {
        [HarmonyTranspiler]
        [HarmonyPatch("FixedUpdate")]
        internal static IEnumerable<CodeInstruction> Transpiler_FixedUpdate(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);
            for (int i = 1; i < code.Count; ++i)
            {
                if (code[i].opcode == OpCodes.Call && (MethodInfo)code[i].operand == AccessTools.Method(typeof(UnityEngine.Time), "get_fixedDeltaTime"))
                {
                    code[i].operand = AccessTools.Method(typeof(TimeWarp), "get_fixedDeltaTime");
                    break; // let the smoothing still occur in realtime
                }
            }

            return code;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        internal static bool Prefix_Update(ResourceItem __instance)
        {
            int resID = __instance.resourceID;

            __instance.resourceBar.value = (float)__instance.resourceValue;

            __instance.amountText.text = "<color=#000000>" + ResourceUnits.PrintAmount(__instance.vesselResourceCurrent, resID, 5, __instance.vesselResourceCurrent > 100 ? "F0" : "F2") + "</color>";
            __instance.maxText.text = "<color=#000000>" + ResourceUnits.PrintAmount(__instance.vesselResourceTotal, resID, 5, __instance.vesselResourceCurrent > 100 ? "F0" : "F2") + "</color>";

            if (__instance.deltaSmoothed == 0f)
                __instance.deltaText.text = "<color=#000000>(0)</color>";
            else if (__instance.deltaSmoothed > 0f)
                __instance.deltaText.text = "<color=#110000>(-" + ResourceUnits.PrintRatePerSecBare(-__instance.deltaSmoothed, resID, 3, "F2") + ")</color>";
            else
                __instance.deltaText.text = "<color=#000011>(" + ResourceUnits.PrintRatePerSecBare(-__instance.deltaSmoothed, resID, 3, "F2") + ")</color>";

            return false;
        }
    }
}
