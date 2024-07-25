using KSP.UI.Screens;
using UnityEngine;

namespace RealismOverhaul
{
    /// <summary>
    /// Removes stock part tags that aren't relevant in RO context.
    /// Cannot be done in a Harmony patch because the relevant code runs before plugins are initialized.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class PartTagNuker : MonoBehaviour
    {
        internal void Start()
        {
            NukeTags();
        }

        internal static void NukeTags()
        {
            BasePartCategorizer.size0Tags = BasePartCategorizer.size1Tags = BasePartCategorizer.size1p5Tags =
                BasePartCategorizer.size2Tags = BasePartCategorizer.size3Tags = BasePartCategorizer.size4Tags =
                BasePartCategorizer.srfTags = BasePartCategorizer.radialTag = new string[0];
        }
    }
}
