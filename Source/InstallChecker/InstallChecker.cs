using System;
using UniLinq;
using UnityEngine;

namespace ROInstallChecker
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class InstallChecker : MonoBehaviour
    {
        protected void Start()
        {
            var assembliesToCheck = new[] { "RealismOverhaul" };    // These are values of KSPAssembly attribute
            if (assembliesToCheck.Any(an => !AssemblyLoader.loadedAssemblies.Any(a => a.name.Equals(an, StringComparison.OrdinalIgnoreCase))))
            {
                string titleText = "Incorrect Realism Overhaul Install";
                string contentText = "This could be caused by downloading the RO repo or some specific branch directly from GitHub, or by not installing or not updating dependencies. " +
                    "Make sure to install via CKAN or use the RP-1 Install Guide.";
                ShowErrorDialog(titleText, contentText);
                return;
            }

            var commonBadPathSymbols = new[] { "'", "+", "&"};
            if (commonBadPathSymbols.Any(s => KSPUtil.ApplicationRootPath.Contains(s)))
            {
                string titleText = "Bad symbols in installation path";
                string contentText = $"Make sure that folder names do not contain special characters like <b>{string.Join(" ", commonBadPathSymbols)}</b>";
                ShowErrorDialog(titleText, contentText);
                return;
            }
        }

        private static void ShowErrorDialog(string titleText, string contentText)
        {
            string titleColor = "red";

            PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "ROInstallCheckerErr",
                $"<color={titleColor}>{titleText}</color>",
                contentText, KSP.Localization.Localizer.GetStringByTag("#autoLOC_190905"), true, HighLogic.UISkin,
                true, string.Empty);
        }
    }
}
