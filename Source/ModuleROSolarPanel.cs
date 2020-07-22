using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealismOverhaul
{
    public class ModuleROSolarPanel : PartModule
    {
        private static readonly HashSet<string> cbOptions = new HashSet<string>();
        public const string groupName = "solarCellPlanner";
        public const string groupDisplayName = "Solar Cell Planner";

        [KSPField(guiActiveEditor = true, guiName = "Celestial Body", groupName = groupName, groupDisplayName = groupDisplayName),
            UI_ChooseOption(suppressEditorShipModified = true, options = new[] { "Choose One" })]
        public string selectedBody = string.Empty;

        [KSPField(guiActiveEditor = true, guiName = "Days Elapsed", groupName = groupName),
            UI_FloatRange(maxValue = 7300, minValue = 1, stepIncrement = 1, suppressEditorShipModified = true)]
        public float daysElapsed = 1f;

        [KSPField(guiActiveEditor = true, guiName = "Efficiency", guiFormat = "F0", guiUnits = "%", groupName = groupName)]
        public float solarEfficiency = 100.0f;

        [KSPField(guiActiveEditor = true, guiName = "Output at Pe", guiFormat = "F2", guiUnits = " W", groupName = groupName)]
        public float solarOutputPe = 0;

        [KSPField(guiActiveEditor = true, guiName = "Output at Ap", guiFormat = "F2", guiUnits = " W", groupName = groupName)]
        public float solarOutputAp = 0;

        public override void OnStart(StartState state)
        {
            Fields[nameof(daysElapsed)].uiControlEditor.onFieldChanged = PlanningChange;
            Fields[nameof(selectedBody)].uiControlEditor.onFieldChanged = PlanningChange;
            (Fields[nameof(selectedBody)].uiControlEditor as UI_ChooseOption).options = PlanetWalk();
            selectedBody = Planetarium.fetch.Home.name;
            GameEvents.onEditorShipModified.Add(OnEditorShipModified);
        }

        public void OnDestroy() => GameEvents.onEditorShipModified.Remove(OnEditorShipModified);

        /// <summary>
        /// Return names of all direct children of Planetarium.fetch.Sun
        /// </summary>
        public string[] PlanetWalk()
        {
            cbOptions.Clear();
            CelestialBody theSun = Planetarium.fetch.Sun;
            foreach (CelestialBody body in FlightGlobals.Bodies.Where(x => x.referenceBody == theSun && x != theSun))
            {
                cbOptions.Add(body.name);
            }
            return cbOptions.ToArray();
        }

        private void CalculateRates()
        {
            ModuleDeployableSolarPanel pm = part.Modules.GetModule<ModuleDeployableSolarPanel>();
            float timeEfficEvaluated = pm.timeEfficCurve.Evaluate(daysElapsed);
            solarEfficiency = timeEfficEvaluated * 100;
            float currentOutputW = pm.chargeRate * timeEfficEvaluated * 1000;

            float currentPeAU = ConvertToAU(FlightGlobals.GetBodyByName(selectedBody).orbit.PeA);
            float currentApAU = ConvertToAU(FlightGlobals.GetBodyByName(selectedBody).orbit.ApA);
            solarOutputPe = DistanceScaling(currentPeAU) * currentOutputW;
            solarOutputAp = DistanceScaling(currentApAU) * currentOutputW;
        }

        private void PlanningChange(BaseField f, object obj) => CalculateRates();
        private void OnEditorShipModified(ShipConstruct _) => CalculateRates();

        private float DistanceScaling(float AU) => 1 / Mathf.Pow(AU, 2);

        private float ConvertToAU(double orbitParam)
        {
            return Convert.ToSingle(orbitParam / FlightGlobals.GetHomeBody().orbit.semiMajorAxis);
        }
    }
}
