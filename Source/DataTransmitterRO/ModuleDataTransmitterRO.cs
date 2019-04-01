using System;
using System.Linq;
using UnityEngine;

namespace RealismOverhaul.DataTransmitterRO
{
    class ModuleDataTransmitterRO : ModuleDataTransmitter, IPartMassModifier
    {
        private const double BASE_POWER = 84610911.3771648;
        private const float MAX_RATE_MULTIPLIER = 1000f;
        private readonly float DATA_RATE_EXPONENT_RANGE = 12;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Tx Power")]
        public string TxPowerString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Total Power")]
        public string TotalPowerString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Min Data Rate")]
        public string MinDataRateString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Range to DSN")]
        public string RangeString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Electronics Mass")]
        public string ElectronicsMassString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Total Mass")]
        public string TotalMassString;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Data Rate (Exponent)", guiUnits = "#", guiFormat = "F0"), UI_FloatRange(minValue = 0, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float DataRateExponent = 0;
        private UI_FloatRange DataRateExponentEdit => (UI_FloatRange)Fields[nameof(DataRateExponent)].uiControlEditor;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tx Power (dBW)", guiUnits = "dBW", guiFormat = "F0"), UI_FloatRange(minValue = -12, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float TxPowerDbw = 0;
        private UI_FloatRange TxPowerDbwEdit => (UI_FloatRange)Fields[nameof(TxPowerDbw)].uiControlEditor;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tech Level", guiUnits = "#", guiFormat = "F0"), UI_FloatRange(minValue = 0, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float TechLevel = 99;
        private UI_FloatRange TechLevelEdit => (UI_FloatRange)Fields[nameof(TechLevel)].uiControlEditor;

        [KSPField]
        public int maxTechLevel = 0;

        [KSPField]
        public float referenceGain = 0f;

        [KSPField]
        public float referenceFrequency = 8415f;

        [KSPField]
        public AntennaShape antennaShape = AntennaShape.Auto;

        private bool _isKerbalismLoaded;

        private TechLevel TechLevelInstance => DataTransmitterRO.TechLevel.GetTechLevel((int)TechLevel);

        private float TxPower => FromDB(TxPowerDbw);
        private float TotalPower => TxPower / TechLevelInstance.Efficiency + TechLevelInstance.BasePower;
        private float MinDataRate => FromLog2(DataRateExponent);
        private double DsnRange => GameVariables.Instance.GetDSNRange(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation));
        private float ElectronicsMass => (TechLevelInstance.BaseMass + TechLevelInstance.MassPerWatt * TxPower) / 1000;
        private float TotalMass => part.prefabMass + ElectronicsMass;

        public override void OnStart(StartState state)
        {
            SetupRangeCurve();
            SetMaxTechLevel();
            SetAntennaShape();
            UpdateConfiguration();
            SetupChangeListeners();
            base.OnStart(state);
            Fields[nameof(powerText)].guiActiveEditor = false;
            Fields[nameof(powerText)].guiActive = false;
            _isKerbalismLoaded = AssemblyLoader.loadedAssemblies.Select(x => x.name).Any(x => x.StartsWith("Kerbalism"));
        }

        private void SetupChangeListeners()
        {
            DataRateExponentEdit.onFieldChanged += OnFieldChanged;
            TxPowerDbwEdit.onFieldChanged += OnFieldChanged;
            TechLevelEdit.onFieldChanged += OnFieldChanged;
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
            SetMaxTxPower();
            SetDataRateLimits();
            SetupBaseFields();
            UpdatePawFields();
        }

        private void UpdatePawFields()
        {
            TxPowerString = Format(TxPower, "W");
            TotalPowerString = Format(TotalPower, "W");
            MinDataRateString = Format(MinDataRate, "b/s", 1024);
            var range = (float)Math.Sqrt(antennaPower * DsnRange);
            RangeString = Format(range, "m");
            ElectronicsMassString = Format(ElectronicsMass * 1000000, "g");
            TotalMassString = Format(TotalMass * 1000000, "g");
        }

        private string Format(float value, string unit, int logBase = 1000)
        {
            var prefixes = new[] { "m", "", "k", "M", "G", "T" };
            var prefixNumber = (int)Mathf.Floor(Mathf.Log(value) / Mathf.Log(logBase));
            value /= Mathf.Pow(logBase, prefixNumber);
            var digits = (int)Mathf.Log10(value);
            return $"{value:G3}\u2009{prefixes[prefixNumber + 1]}{unit}";
        }

        private void SetMaxTxPower()
        {
            var maxTxPowerDbw = ToDB(TechLevelInstance.MaxPower);
            TxPowerDbwEdit.maxValue = maxTxPowerDbw;
            TxPowerDbw = Mathf.Clamp(TxPowerDbw, -13, maxTxPowerDbw);
        }

        private void SetDataRateLimits()
        {
            var minRateExponent = ToLog2(TechLevelInstance.MinDataRate);
            var maxRateExponent = minRateExponent + DATA_RATE_EXPONENT_RANGE;
            DataRateExponentEdit.minValue = minRateExponent;
            DataRateExponentEdit.maxValue = maxRateExponent;
            DataRateExponent = Mathf.Clamp(DataRateExponent, minRateExponent, maxRateExponent);
        }

        private void OnFieldChanged(BaseField field, object oldValueObj)
        {
            Debug.Log($"[MDTRO] Field changed");
            UpdateConfiguration();
        }

        private void SetMaxTechLevel()
        {
            Debug.Log($"[MDTRO] max tl: {maxTechLevel}");
            TechLevelEdit.maxValue = maxTechLevel;
            TechLevel = Mathf.Clamp(TechLevel, 0f, maxTechLevel);
        }

        private void SetupBaseFields()
        {
            antennaPower = GetMdtAntennaPower(TechLevelInstance);
            packetInterval = 1;
            packetSize = MinDataRate * (_isKerbalismLoaded ? MAX_RATE_MULTIPLIER : 1) / 1024 / 1024;
            packetResourceCost = TotalPower / 1000;
            antennaType = AntennaType.RELAY;

            Debug.Log($"[MDTRO] mass: {(TechLevelInstance.BaseMass + TechLevelInstance.MassPerWatt * TxPower) / 1000}");
        }

        private double GetMdtAntennaPower(TechLevel tl)
        {
            return BASE_POWER * FromDB(referenceGain + tl.Gain) * TxPower / MinDataRate * GetFrequencyFactor(tl);
        }

        private double GetFrequencyFactor(TechLevel tl)
        {
            return antennaShape == AntennaShape.Omni ? 1 : Math.Pow(tl.Frequency / referenceFrequency, 2);
        }

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit)
        {
            return ElectronicsMass;
        }

        public ModifierChangeWhen GetModuleMassChangeWhen() => ModifierChangeWhen.FIXED;

        private float ToLog2(float value) => Mathf.Log(value) / Mathf.Log(2);
        private float FromLog2(float value) => Mathf.Pow(2, value);

        private float ToDB(float value) => Mathf.Log10(value) * 10;
        private float FromDB(float value) => Mathf.Pow(10, value / 10f);
    }
}
