using HarmonyLib;
using KSP.UI.Screens;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(BasePartCategorizer))]
    internal class PatchBasePartCategorizer
    {
        /// <summary>
        /// Called through GameEvents.onLanguageSwitched
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("LoadAutoTags")]
        internal static void LoadAutoTags()
        {
            PartTagNuker.NukeTags();
        }
    }
}
