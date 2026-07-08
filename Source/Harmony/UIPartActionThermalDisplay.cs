using HarmonyLib;
using UnityEngine;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(UIPartActionWindow))]
    internal class PatchUIPartActionWindow_Thermal
    {
        [HarmonyPrefix]
        [HarmonyPatch("AddThermalControl")]
        internal static bool Prefix_AddThermalControl(UIPartActionWindow __instance)
        {
            UIPartActionThermalDisplay prefab = UIPartActionController.Instance.debugThermalItemPrefab;
            if (prefab == null)
                return false;

            // Instantiate the stock prefab. On an active prefab Unity calls Awake
            // immediately, setting stockDisplay's pawGroup and (via serialization)
            // all [SerializeField] text field references.
            UIPartActionThermalDisplay stockDisplay = Object.Instantiate(prefab);

            // Add our derived component. On an active GO its Awake also fires
            // here; on an inactive one it fires during SetActive below.
            UIPartActionThermalDisplayExpanded roDisplay = stockDisplay.gameObject.AddComponent<UIPartActionThermalDisplayExpanded>();

            // Delegate all field copying / pawGroup inheritance to the component.
            roDisplay.Initialize(stockDisplay);

            // Drop the stock component — child GOs (the text rows) are unaffected.
            Object.DestroyImmediate(stockDisplay);

            // SetActive triggers Awake for roDisplay if the prefab was inactive.
            // pawGroup is guaranteed to be non-null after this point.
            roDisplay.gameObject.SetActive(true);

            __instance.AddGroup(roDisplay.transform, roDisplay.pawGroup);

            roDisplay.transform.SetSiblingIndex(__instance.controlIndex++);
            roDisplay.Setup(__instance, __instance._part, __instance.scene);
            __instance.AddItem(roDisplay, __instance.controlIndex - 1);

            return false;
        }
    }
}
