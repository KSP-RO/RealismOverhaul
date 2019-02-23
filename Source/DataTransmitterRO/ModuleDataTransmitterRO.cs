using System;
using UnityEngine;

namespace RealismOverhaul.DataTransmitterRO
{
    class ModuleDataTransmitterRO : ModuleDataTransmitter, IPartMassModifier
    {
        private const double BASE_POWER = 84610911.3771648;
        private const float MAX_RATE_MULTIPLIER = 1000f;

        private TechLevel TechLevelInstance => DataTransmitterRO.TechLevel.GetTechLevel((int)TechLevel);
        private float MinDataRate => TechLevelInstance.MinDataRate * Mathf.Pow(2, DataRateExponent);

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Tx Power", guiFormat = "F1", guiUnits = "\u2009W")]
        public float TxPower;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Data Rate Exponent", guiUnits = "#", guiFormat = "F0"), UI_FloatRange(minValue = 0, maxValue = 5, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float DataRateExponent = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "TX Power Exponent", guiUnits = "#", guiFormat = "F0"), UI_FloatRange(minValue = -5, maxValue = 0, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float TxPowerExponent = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tech Level", guiUnits = "#", guiFormat = "F0"), UI_FloatRange(minValue = 0, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float TechLevel = 99;

        [KSPField]
        public int maxTechLevel = 0;

        [KSPField]
        public float referenceGain = 0f;

        [KSPField]
        public float referenceFrequency = 8415f;

        [KSPField]
        public AntennaShape antennaShape = AntennaShape.Auto;

        public override void OnStart(StartState state)
        {
            SetupRangeCurve();
            SetMaxTechLevel();
            SetAntennaShape();
            UpdateConfiguration();
            ((UI_FloatRange)Fields[nameof(DataRateExponent)].uiControlEditor).onFieldChanged += OnFieldChanged;
            ((UI_FloatRange)Fields[nameof(TxPowerExponent)].uiControlEditor).onFieldChanged += OnFieldChanged;
            ((UI_FloatRange)Fields[nameof(TechLevel)].uiControlEditor).onFieldChanged += OnFieldChanged;
            base.OnStart(state);
        }

        private void SetAntennaShape()
        {
            if(antennaShape == AntennaShape.Auto)
            {
                antennaShape = referenceGain > 8 ? AntennaShape.Dish : AntennaShape.Omni;
            }
        }

        private void SetupRangeCurve()
        {
            rangeCurve = new DoubleCurve();
            var key = 1 / Math.Sqrt(MAX_RATE_MULTIPLIER);
            rangeCurve.Add(1 - key, 1, 0, 0);
            var steps = 20;
            var factor = Math.Pow(MAX_RATE_MULTIPLIER, 0.5 / steps);
            for(int i = 0; i < 19; ++i)
            {
                key = key * factor;
                var value = 1 / (key * key) / MAX_RATE_MULTIPLIER;
                rangeCurve.Add(1 - key, value);
            }
            rangeCurve.Add(0, 1 / MAX_RATE_MULTIPLIER);
        }

        private void UpdateConfiguration()
        {
            TxPower = TechLevelInstance.MaxPower * Mathf.Pow(2, TxPowerExponent);
            SetupBaseFields();
        }

        private void OnFieldChanged(BaseField field, object oldValueObj)
        {
            //GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
            UpdateConfiguration();
        }

        private void SetMaxTechLevel()
        {
            Debug.Log($"[MDTRO] max tl: {maxTechLevel}");
            ((UI_FloatRange)Fields[nameof(TechLevel)].uiControlEditor).maxValue = maxTechLevel;
            TechLevel = Mathf.Clamp(TechLevel, 0f, maxTechLevel);
        }

        private void SetupBaseFields()
        {
            antennaPower = GetMdtAntennaPower(TechLevelInstance);
            powerText = KSPUtil.PrintSI(antennaPower, string.Empty, 3, false);
            packetInterval = 1;
            packetSize = MinDataRate * MAX_RATE_MULTIPLIER / 1000000;
            packetResourceCost = TxPower / TechLevelInstance.Efficiency / 1000;
            antennaType = AntennaType.RELAY;
            Debug.Log($"[MDTRO] mass: {(TechLevelInstance.BaseMass + TechLevelInstance.MassPerWatt * TxPower) / 1000}");
        }

        private double GetMdtAntennaPower(TechLevel tl)
        {
            var frequencyFactor = GetFrequencyFactor(tl);
            return BASE_POWER * Mathf.Pow(10, (referenceGain + tl.Gain) / 10.0f) * TxPower / MinDataRate * frequencyFactor;
        }

        private double GetFrequencyFactor(TechLevel tl) => antennaShape == AntennaShape.Omni ? 1 : Math.Pow(tl.Frequency / referenceFrequency, 2);

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit)
        {
            return (TechLevelInstance.BaseMass + TechLevelInstance.MassPerWatt * TxPower) / 1000;
        }

        public ModifierChangeWhen GetModuleMassChangeWhen() => ModifierChangeWhen.FIXED;
    }
}
