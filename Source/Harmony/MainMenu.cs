using HarmonyLib;
using UnityEngine;
using KSP.Localization;
using UnityEngine.UI;
using System.Reflection;
using System.Linq;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class PatchMainMenu
    {
        private static bool _needCheckRP1 = true;
        private static bool _hasRP1;

        [HarmonyPrefix]
        [HarmonyPatch("ConfirmNewGame")]
        internal static bool Prefix_ConfirmNewGame(MainMenu __instance, Game.Modes ___newGameMode, PopupDialog ___createGameDialog)
        {
            if (_needCheckRP1)
            {
                _hasRP1 = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "RP-0") != null;
                _needCheckRP1 = false;
            }
            
            if (_hasRP1)
                return true;

            if (___newGameMode != Game.Modes.SCIENCE_SANDBOX && ___newGameMode != Game.Modes.CAREER)
                return true;
            
            Callback cb = () => { if (___createGameDialog != null) ___createGameDialog.gameObject.SetActive(true); };

            PopupDialog dlg = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new MultiOptionDialog("ROSandboxOnly",
                    Localizer.Format("#ro_MainMenu_NoCareer_Text"),
                    Localizer.Format("#ro_MainMenu_NoCareer_Title"),
                    null,
                    new DialogGUIButton(Localizer.Format("#autoLOC_190905"),
                        cb, dismissOnSelect: true)), persistAcrossScenes: false, null);
            dlg.OnDismiss = cb;
            MenuNavigation.SpawnMenuNavigation(dlg.gameObject, Navigation.Mode.Vertical, limitCheck: true);
            return false;
        }
    }
}
