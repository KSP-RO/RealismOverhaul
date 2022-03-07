using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class RainbowPloom : MonoBehaviour
    {
        private const string ConfigPatches = @"
            COLORMODIFIER
            {
                controllerName = __rainbow_rand
                combinationType = REPLACE
                rCurve
                {
                    key = 0.00 0.9 0 0
                    key = 0.25 0.9 0 0
                    key = 0.45 0.0 0 0
                    key = 0.55 0.0 0 0
                    key = 0.75 0.9 0 0
                    key = 1.00 0.9 0 0

                }
                gCurve
                {
                    key = 0.0 0.0 0 0
                    key = 0.1 0.0 0 0
                    key = 0.3 0.9 0 0
                    key = 0.7 0.0 0 0
                    key = 0.9 0.0 0 0
                    key = 1.0 0.0 0 0
                }
                bCurve
                {
                    key = 0.0 0.0 0 0
                    key = 0.1 0.0 0 0
                    key = 0.3 0.0 0 0
                    key = 0.7 0.9 0 0
                    key = 0.9 0.0 0 0
                    key = 1.0 0.0 0 0
                }
                aCurve {}
            }
            CONTROLLER
            {
                name = __rainbow_rand
                linkedTo = random
                noiseType = perlin
                minimum = 0
                scale = 1
                speed = 0.3
            }";

        private static ConfigNode s_modifierBase;
        private static ConfigNode s_controller;

        private static bool EscapeHatch => File.Exists(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "PluginData",
            "DisableRainbowPloom"));

        public void Awake()
        {
            var patches = ConfigNode.Parse(ConfigPatches);
            s_modifierBase = patches.GetNode("COLORMODIFIER");
            s_controller = patches.GetNode("CONTROLLER");
        }

        public void ModuleManagerPostLoad()
        {
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1
                && AssemblyLoader.loadedAssemblies.Any(asm => asm.name == "Waterfall")
                && !EscapeHatch)
            {
                StartCoroutine(Patch());
            }
            Destroy(this);
        }

        private static IEnumerator Patch()
        {
            var WaterfallTemplates = Type.GetType("Waterfall.WaterfallTemplates, Waterfall");
            var Library = (IDictionary)WaterfallTemplates.GetField("Library").GetValue(null);
            while (Library.Count == 0) yield return null;

            PatchTemplates();
            Library.Clear();
            WaterfallTemplates.GetMethod("LoadTemplates").Invoke(null, null);

            PatchEngines();
        }

        private static void PatchTemplates()
        {
            foreach (var template in GameDatabase.Instance.GetConfigNodes("EFFECTTEMPLATE"))
            {
                foreach (var effect in template.GetNodes("EFFECT"))
                {
                    string transform = effect.GetNode("MODEL")?.GetNode("MATERIAL")?.GetValue("transform");
                    if (transform == null) continue;

                    var modifierStart = s_modifierBase.CreateCopy();
                    modifierStart.AddValue("name", "__rainbowploom_start");
                    modifierStart.AddValue("transformName", transform);
                    modifierStart.AddValue("colorName", "_StartTint");
                    effect.AddNode(modifierStart);

                    var modifierEnd = s_modifierBase.CreateCopy();
                    modifierEnd.AddValue("name", "__rainbowploom_end");
                    modifierEnd.AddValue("transformName", transform);
                    modifierEnd.AddValue("colorName", "_EndTint");
                    effect.AddNode(modifierEnd);
                }
            }
        }

        private static void PatchEngines()
        {
            foreach (var part in GameDatabase.Instance.GetConfigNodes("PART"))
            {
                foreach (var mod in part.GetNodes("MODULE", "name", "ModuleWaterfallFX"))
                    mod.AddNode(s_controller);
            }
        }
    }
}
