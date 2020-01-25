using System;
using System.Collections.Generic;
using System.Reflection;
using KSP;
using UnityEngine;

namespace RealismOverhaul
{
    public class ModuleROSolarPanel : PartModule
    {
        private static HashSet<string> cbOptions = new HashSet<string>();

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "<b>SOLAR CELL DEGRADATION AT</b>", groupName = "solarCellPlanner", groupDisplayName = "Solar Cell Planner")]
        public string spCalc = String.Empty;

        [KSPField(isPersistant = false, guiActiveEditor = true, guiActive = false, guiName = "Days Elapsed", groupName = "solarCellPlanner"),
            UI_FloatEdit(minValue = 1, maxValue = 36500, incrementLarge = 100.0f, incrementSmall = 10.0f, incrementSlide = 1.0f, requireFullControl = false, suppressEditorShipModified = true, sigFigs = 0)]
        public float daysElapsed = 1f;

        [KSPField(isPersistant = false, guiActiveEditor = true, guiActive = false, guiName = "Efficiency", guiFormat = "F0", guiUnits = "%", groupName = "solarCellPlanner")]
        public float solarEfficiency = 100.0f;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Celestial Body", groupName = "solarCellPlanner"),
            UI_ChooseOption(suppressEditorShipModified = true, options = new[] { "Choose One" })]
        public string selectedBody = "Choose One";

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Output at Pe", groupName = "solarCellPlanner")]
        public string solarOutputPe = "";

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Output at Ap", groupName = "solarCellPlanner"]
        public string solarOutputAp = "";

        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = false, guiName = "Expected Output", guiFormat = "F2", groupName = "solarCellPlanner")]
        public string futureOutput = "";

        public override void OnAwake()
        {
            base.OnAwake();
        }
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            BaseField field = Fields[nameof(selectedBody)];
            UI_ChooseOption choose = (UI_ChooseOption)field.uiControlEditor;
            choose.options = PlanetWalk();

            UIElements();
        }

        /// <summary>
        /// Get all CB's from the game and add the planets to the bodyOptions List
        /// </summary>
        public String[] PlanetWalk()
        {
            if (cbOptions.Count < 1)
            {
                CelestialBody theSun = Planetarium.fetch.Sun;
                foreach (CelestialBody body in FlightGlobals.Bodies)
                {
                    if (body.referenceBody == theSun && body != theSun)
                    {
                        cbOptions.Add(body.name);
                    }
                }
            }
            String[] bodyOptions = new string[cbOptions.Count];
            cbOptions.CopyTo(bodyOptions);
            return bodyOptions;
        }

        private void UIElements()
        {
            BaseField theField = Fields[nameof(daysElapsed)];
            theField.uiControlEditor.onFieldChanged = (a, b) =>
            {
                CalculateRates();
            };
            Fields[nameof(selectedBody)].uiControlEditor.onFieldChanged = (a, b) =>
            {
                CalculateRates();
            };
        }

        private void CalculateRates()
        {
            double currentPeMeters, currentApMeters = 0.0;

            ModuleDeployableSolarPanel pm = part.Modules.GetModule<ModuleDeployableSolarPanel>();
            float timeEfficEvaluated = pm.timeEfficCurve.Evaluate(daysElapsed);
            solarEfficiency = timeEfficEvaluated * 100;
            double currentOutput = pm.chargeRate * timeEfficEvaluated;

            if (HighLogic.LoadedSceneIsEditor)
            {
                currentPeMeters = _getAU(FlightGlobals.GetBodyByName(selectedBody).orbit.PeA);
                currentApMeters = _getAU(FlightGlobals.GetBodyByName(selectedBody).orbit.ApA);
                solarOutputPe = Math.Round(_getModifier(currentPeMeters) * currentOutput * 1000, 2).ToString() + " Watts";
                solarOutputAp = Math.Round(_getModifier(currentApMeters) * currentOutput * 1000, 2).ToString() + " Watts";
            }
        }

        private double _getModifier(double AU)
        {
            return (1 / Math.Pow(AU, 2));
        }

        private double _getAU(double orbitParam)
        {
            return orbitParam / FlightGlobals.GetHomeBody().orbit.semiMajorAxis;
        }
    }
}
