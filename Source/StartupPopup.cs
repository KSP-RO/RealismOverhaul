using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class StartupPopup : MonoBehaviour
    {
        private const string PreferenceFileName = "RORotationPopupLock";
        private static string PreferenceFilePath => Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "PluginData",
            PreferenceFileName);

        public void Start()
        {
            if (File.Exists(PreferenceFilePath)) return;

            bool hasPR = false;
            bool hasMRCS = false;

            foreach (var a in AssemblyLoader.loadedAssemblies)
            {
                if (a.name == "PersistentRotation" || a.name == "PersistentRotationUpgraded")
                {
                    hasPR = true;
                }
                if (a.name == "MandatoryRCS")
                {
                    hasMRCS = true;
                }
            }


            if (hasPR || hasMRCS)
            {
                String message = "Realism Overhaul now contains its own, light, implementation of continuing vessel rotation during timewarp for when Principia is not installed. We've detected you have:\n\n";
                if (hasPR)
                    message += "* Persistent Rotation\n";
                if (hasMRCS)
                    message += "* MandatoryRCS\n";
                message += "\ninstalled. It is suggested to quit, remove " + ((hasPR && hasMRCS) ? "them" : "it") + ", and relaunch KSP.\n\nBut don't worry, RO's own implementation will be disabled until you do.";


                PopupDialog.SpawnPopupDialog(
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new MultiOptionDialog(
                        "ROStartupDialog",
                        message,
                        "Realism Overhaul",
                        HighLogic.UISkin,
                        new DialogGUIVerticalLayout(
                            new DialogGUIButton("Don't show again", RememberPreference, true),
                            new DialogGUIButton("Ok", () => {}, true )
                            )
                        ),
                true,
                    HighLogic.UISkin);
            }
        }

        private static void RememberPreference()
        {
            // create empty file
            File.Create(PreferenceFilePath).Close();
        }
    }
}
