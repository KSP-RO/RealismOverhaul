using UnityEngine;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class HarmonyPatcher : MonoBehaviour
    {
        internal void Start()
        {
            var harmony = new HarmonyLib.Harmony("RO.HarmonyPatcher");
            harmony.PatchAll();
        }
    }
}
