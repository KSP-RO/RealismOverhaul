using System;
using System.Collections.Generic;
using System.Reflection;
using KSP;
using UnityEngine;

namespace RealismOverhaul
{
    public class ModuleROSolarPanel : PartModule
    {
        private double currentOrbit = 0;
        private static HashSet<string> cbOptions = new HashSet<string>();

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "<b>SOLAR CELL DEGRADATION AT</b>")]
        public string spCalc = String.Empty;

        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = true, guiName = "Assumes Orbit Stays the Same")]
        public string orbCalc = String.Empty;

        [KSPField(isPersistant = false, guiActiveEditor = true, guiActive = true, guiName = "Days Elapsed"),
            UI_FloatEdit(minValue = 1, maxValue = 36500, incrementLarge = 100.0f, incrementSmall = 10.0f, incrementSlide = 1.0f, requireFullControl = false, suppressEditorShipModified = true, sigFigs = 0)]
        public float daysElapsed = 1f;

        [KSPField(isPersistant = false, guiActiveEditor = true, guiActive = true, guiName = "Efficiency", guiFormat = "F0", guiUnits = "%")]
        public float solarEfficiency = 100.0f;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Celestial Body"),
            UI_ChooseOption(suppressEditorShipModified = true, options = new[] { "Choose One" })]
        public string selectedBody = "Choose One";

        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = true, guiName = "Current AU")]
        public string currentAU = "";

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Output at Pe")]
        public string solarOutputPe = "";

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Output at Ap")]
        public string solarOutputAp = "";

        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = true, guiName = "Expected Output", guiFormat = "F2")]
        public string futureOutput = "";

        public override void OnAwake()
        {
            base.OnAwake();
        }
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            GameEvents.onVesselSOIChanged.Add(OnVesselSOIChanged);
            GameEvents.onVesselSituationChange.Add(OnVesselSituationChange);

            BaseField field = Fields[nameof(selectedBody)];
            UI_ChooseOption choose = (UI_ChooseOption)field.uiControlEditor;
            choose.options = PlanetWalk();

            UIElements();
        }

        /*
        public override void OnDestroy()
        {
            GameEvents.onVesselSOIChanged.Remove(OnVesselSOIChanged);
            GameEvents.onVesselSituationChange.Remove(OnVesselSituationChange);
        }
        */

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
            theField.uiControlFlight.onFieldChanged = OnFieldChanged;
        }

        private void CalculateRates()
        {
            double currentPeMeters, currentApMeters, theOrbit = 0.0;

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

                //solarOutputPe = currentPe.ToString() + " Watts";
                //solarOutputAp = currentAp.ToString() + " Watts";
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                double mod = _getModifier(currentOrbit);
                theOrbit = Math.Round(mod * currentOutput * 1000, 2);
                futureOutput = theOrbit.ToString() + " Watts";
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

        public void UpdateData()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                int currentMETdays = (int)vessel.missionTime / 86400;
                BaseField field = Fields["daysElapsed"];
                field.guiActiveEditor = false;
                daysElapsed = currentMETdays;
                UI_FloatEdit edit = (UI_FloatEdit)field.uiControlFlight;
                if (edit != null)
                {
                    edit.minValue = currentMETdays;
                }
                CelestialBody cb = vessel.mainBody;
                while (cb.referenceBody != Planetarium.fetch.Sun)
                {
                    cb = cb.referenceBody;
                }
                currentOrbit = cb.orbit.altitude / FlightGlobals.GetHomeBody().orbit.semiMajorAxis;
                currentAU = Math.Round(currentOrbit, 3).ToString();
                CalculateRates();
            }
        }

        public void OnVesselSOIChanged(GameEvents.HostedFromToAction<Vessel, CelestialBody> evt)
        {
            UpdateData();
        }

        public void OnVesselSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> evt)
        {
            UpdateData();
        }

        public void OnFieldChanged(BaseField field, object obj)
        {
            UpdateData();
        }
    }
}
