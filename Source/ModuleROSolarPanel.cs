using System;
using KSP;
using UnityEngine;
using Kopernicus.Components;

namespace RealismOverhaul
{
    public class ModuleROSolarPanel : KopernicusSolarPanel
    {
        public double currentOrbit = 0;

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
            UI_ChooseOption(suppressEditorShipModified = true, options = new[] { "Earth", "Mercury", "Venus", "Mars", "Vesta", "Ceres", "Jupiter", "Saturn" })]
        public string celestialBody = "Earth";

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Output at Pe")]
        public string solarOutputPe = "";

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Output at Ap")]
        public string solarOutputAp = "";

        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = true, guiName = "Expected Output", guiFormat = "F2")]
        public string futureOutput = "";

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            GameEvents.onVesselSOIChanged.Add(OnVesselSOIChanged);
            GameEvents.onVesselSituationChange.Add(OnVesselSituationChange);
            UIElements();
        }

        public void OnDestroy()
        {
            GameEvents.onVesselSOIChanged.Remove(OnVesselSOIChanged);
            GameEvents.onVesselSituationChange.Remove(OnVesselSituationChange);
        }

        private void UIElements()
        {
            BaseField theField = Fields[nameof(daysElapsed)];
            theField.uiControlEditor.onFieldChanged = (a, b) =>
            {
                CalculateRates();
            };
            Fields[nameof(celestialBody)].uiControlEditor.onFieldChanged = (a, b) =>
            {
                CalculateRates();
            };
            theField.uiControlFlight.onFieldChanged = OnFieldChanged;
        }

        private void CalculateRates()
        {
            double planetPe, planetAp, currentPe, currentAp, theOrbit = 0.0;

            float timeEfficEvaluated = this.timeEfficCurve.Evaluate(daysElapsed);
            solarEfficiency = timeEfficEvaluated * 100;
            double currentOutput = this.resHandler.UpdateModuleResourceOutputs(1, 0.0) * timeEfficEvaluated;

            if (HighLogic.LoadedSceneIsEditor)
            {
                switch (celestialBody)
                {
                    case "Mercury":
                        planetPe = GetModifier(0.3075);
                        planetAp = GetModifier(0.4667);
                        currentPe = Math.Round(planetPe * currentOutput * 1000, 2);
                        currentAp = Math.Round(planetAp * currentOutput * 1000, 2);
                        break;
                    case "Venus":
                        planetPe = GetModifier(0.7184);
                        planetAp = GetModifier(0.7283);
                        currentPe = Math.Round(planetPe * currentOutput * 1000, 2);
                        currentAp = Math.Round(planetAp * currentOutput * 1000, 2);
                        break;
                    case "Mars":
                        planetPe = GetModifier(1.3816);
                        planetAp = GetModifier(1.6659);
                        currentPe = Math.Round(planetPe * currentOutput * 1000, 2);
                        currentAp = Math.Round(planetAp * currentOutput * 1000, 2);
                        break;
                    case "Vesta":
                        planetPe = GetModifier(2.1489);
                        planetAp = GetModifier(2.5750);
                        currentPe = Math.Round(planetPe * currentOutput * 1000, 2);
                        currentAp = Math.Round(planetAp * currentOutput * 1000, 2);
                        break;
                    case "Ceres":
                        planetPe = GetModifier(2.5462);
                        planetAp = GetModifier(2.9852);
                        currentPe = Math.Round(planetPe * currentOutput * 1000, 2);
                        currentAp = Math.Round(planetAp * currentOutput * 1000, 2);
                        break;
                    case "Jupiter":
                        planetPe = GetModifier(4.9484);
                        planetAp = GetModifier(5.4553);
                        currentPe = Math.Round(planetPe * currentOutput * 1000, 2);
                        currentAp = Math.Round(planetAp * currentOutput * 1000, 2);
                        break;
                    case "Saturn":
                        planetPe = GetModifier(9.0152);
                        planetAp = GetModifier(10.0337);
                        currentPe = Math.Round(planetPe * currentOutput * 1000, 2);
                        currentAp = Math.Round(planetAp * currentOutput * 1000, 2);
                        break;
                    default:
                        planetPe = GetModifier(0.9839);
                        planetAp = GetModifier(1.0161);
                        currentPe = Math.Round(planetPe * currentOutput * 1000, 2);
                        currentAp = Math.Round(planetAp * currentOutput * 1000, 2);
                        break;
                }
                solarOutputPe = currentPe.ToString() + " Watts";
                solarOutputAp = currentAp.ToString() + " Watts";
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                double mod = GetModifier(currentOrbit);
                theOrbit = Math.Round(mod * currentOutput * 1000, 2);
                futureOutput = theOrbit.ToString() + " Watts";
            }
        }

        private double GetModifier(double AU)
        {
            return (1 / Math.Pow(AU, 2));
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
                currentOrbit = (cb.orbit.altitude / 149597870700);
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
            CalculateRates();
        }
    }
}
