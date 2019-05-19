using RealismOverhaul.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace RealismOverhaul.Communications
{
    class ModuleConfigurableAntenna : ModuleDataTransmitter, IPartMassModifier, IPartCostModifier, ICommAntenna
    {
        private const double BASE_POWER = 84610911.3771648;
        private const int MAX_RATE_EXPONENT = 20;
        private const float MAX_RATE_MULTIPLIER = 1 << MAX_RATE_EXPONENT;
        private const int HALF_SCALE_STEPS = 8;
        private const int SCALE_RANGE = 2;
        private const float ANTENNA_MASS_SCALING_EXPONENT = 2.0f;
        private const float ANTENNA_EFFICIENCY = 0.636f;
        private const float SPEED_OF_LIGHT = 299_792_458;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Tx Power")]
        public string TxPowerString;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Total Power")]
        public string TotalPowerString;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Min. Data Rate")]
        public string MinDataRateString;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Range to DSN")]
        public string RangeDsnString;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Range to Self")]
        public string RangeSelfString;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Electronics Mass")]
        public string ElectronicsMassString;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Total Mass")]
        public string TotalMassString;

        [KSPField(isPersistant = true, guiActiveEditor = false, guiName = "Data Rate"), UI_ChooseOption(scene = UI_Scene.Flight)]
        public int DataRateExponent = 0;
        private UI_ChooseOption DataRateExponentEdit => (UI_ChooseOption)Fields[nameof(DataRateExponent)].uiControlFlight;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tx Power (dBW)", guiUnits = "dBW", guiFormat = "F0"), UI_FloatRange(minValue = -12, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float TxPowerDbw = Communications.TechLevel.GetTechLevel(0).MaxPower.ToDB();
        private UI_FloatRange TxPowerDbwEdit => (UI_FloatRange)Fields[nameof(TxPowerDbw)].uiControlEditor;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tech Level", guiUnits = "#", guiFormat = "F0"), UI_FloatRange(minValue = 0, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float TechLevel = -1;
        private UI_FloatRange TechLevelEdit => (UI_FloatRange)Fields[nameof(TechLevel)].uiControlEditor;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Scale"), UI_ChooseOption(scene = UI_Scene.Editor)]
        public int ScaleIndex = HALF_SCALE_STEPS;
        private UI_ChooseOption ScaleEdit => (UI_ChooseOption)Fields[nameof(ScaleIndex)].uiControlEditor;
        public float Scale => GetScaleFromIndex(ScaleIndex);

        [KSPField]
        public int maxTechLevel = -1;

        [KSPField]
        public float diameter = 0f;

        [KSPField]
        public float referenceGain = 0f;

        [KSPField]
        public float referenceFrequency = 8415f;

        [KSPField]
        public AntennaShape antennaShape = AntennaShape.Auto;

        private bool _isKerbalismLoaded;

        private TechLevel TechLevelInstance => Communications.TechLevel.GetTechLevel((int)TechLevel);
        private Part PartPrefab => part.partInfo.partPrefab;

        private float TxPower => TxPowerDbw.FromDB();
        private float TotalPower => TxPower / TechLevelInstance.Efficiency + TechLevelInstance.BasePower;
        private float MinDataRate => TechLevelInstance.MinDataRate * DataRateExponent.FromLog2();
        private double DsnRange => GameVariables.Instance.GetDSNRange(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation));
        private float ElectronicsMass => (TechLevelInstance.BaseMass + TechLevelInstance.MassPerWatt * TxPower) / 1000;
        private float AntennaMass => PartPrefab.mass * Mathf.Pow(Scale, ANTENNA_MASS_SCALING_EXPONENT);
        private float TotalMass => AntennaMass + ElectronicsMass;
        private bool IsScalable => antennaShape != AntennaShape.Omni;
        private float MaxScale => IsScalable ? Mathf.Pow(SCALE_RANGE, 0.5f) : 1;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            SetupLoadedState();
        }

        public void Start()
        {
            _isKerbalismLoaded = AssemblyLoader.loadedAssemblies.Select(x => x.name).Any(x => x.StartsWith("Kerbalism"));
            ReScale(false);
            SetMaxTechLevel();
            UpdateConfiguration();
            SetupPaw();
        }

        private void SetupLoadedState()
        {
            SetupRangeCurve();
            SetGain();
            SetAntennaShape();
            ApplyUpgrades();
            SetupBaseFields();
        }

        private void SetGain()
        {
            if (diameter > 0)
            {
                referenceGain = (ANTENNA_EFFICIENCY * Mathf.Pow(Mathf.PI * referenceFrequency * 1e6f / SPEED_OF_LIGHT * diameter, 2)).ToDB();
            }
        }

        private void ApplyUpgrades()
        {
            if (TechLevel == -1 && maxTechLevel > 0)
            {
                TechLevel = maxTechLevel;
                TxPowerDbw = TechLevelInstance.MaxPower.ToDB();
            }
        }

        double ICommAntenna.CommPowerUnloaded(ProtoPartModuleSnapshot mSnap)
        {
            var values = mSnap.moduleValues;
            var techLevel = values.GetInt(nameof(TechLevel));
            var scaleIndex = values.GetInt(nameof(ScaleIndex));
            var dataRateExponent = values.GetInt(nameof(DataRateExponent));
            var txPowerDbw = values.GetFloat(nameof(TxPowerDbw));
            var tl = Communications.TechLevel.GetTechLevel(techLevel);
            var frequencyFactor = GetFrequencyFactor(tl.Frequency, antennaShape, referenceFrequency);
            return GetMdtAntennaPower(referenceGain, tl.Gain, GetScaleFromIndex(scaleIndex), txPowerDbw.FromDB(), dataRateExponent.FromLog2() * tl.MinDataRate, frequencyFactor);
        }

        private void SetupPaw()
        {
            SetupChangeListeners();
            SetupEditors();
        }

        private void SetupEditors()
        {
            Fields[nameof(powerText)].guiActiveEditor = false;
            Fields[nameof(powerText)].guiActive = false;

            Fields[nameof(DataRateExponent)].guiActive = !_isKerbalismLoaded;
            DataRateExponentEdit.scene = _isKerbalismLoaded ? UI_Scene.None : UI_Scene.Flight;
            DataRateExponentEdit.options = CreateDataRateOptions();

            if (IsScalable)
            {
                ScaleEdit.options = CreateScaleOptions();
            }
            else
            {
                Fields[nameof(ScaleIndex)].guiActiveEditor = false;
                ScaleEdit.scene = UI_Scene.None;
            }
        }

        private string[] CreateDataRateOptions()
        {
            var result = new string[MAX_RATE_EXPONENT + 1];
            for (int i = 0; i <= MAX_RATE_EXPONENT; ++i)
            {
                result[i] = (MinDataRate * (1 << i)).Format("b/s", 1024);
            }
            return result;
        }

        private string[] CreateScaleOptions()
        {
            var stepCount = HALF_SCALE_STEPS * 2 + 1;
            var result = new string[stepCount];
            for (int i = 0; i < stepCount; ++i)
            {
                result[i] = (GetScaleFromIndex(i) * 100).Format("%");
            }
            return result;
        }

        private float GetScaleFromIndex(int i) => Mathf.Pow(SCALE_RANGE, ((float)i / HALF_SCALE_STEPS - 1) / 2);

        private void SetupChangeListeners()
        {
            DataRateExponentEdit.onFieldChanged = OnFieldChanged;
            DataRateExponentEdit.onSymmetryFieldChanged = OnFieldChanged;
            TxPowerDbwEdit.onFieldChanged = OnFieldChanged;
            TxPowerDbwEdit.onSymmetryFieldChanged = OnFieldChanged;
            TechLevelEdit.onFieldChanged = OnFieldChanged;
            TechLevelEdit.onSymmetryFieldChanged = OnFieldChanged;
            ScaleEdit.onFieldChanged = OnScaleChanged;
            ScaleEdit.onSymmetryFieldChanged = OnScaleChanged;
        }

        private void SetAntennaShape()
        {
            if (antennaShape == AntennaShape.Auto)
            {
                antennaShape = diameter > 0 || referenceGain > 8 ? AntennaShape.Dish : AntennaShape.Omni;
            }
        }

        private void SetupRangeCurve()
        {
            rangeCurve = new DoubleCurve();
            var key = 1 / Math.Sqrt(MAX_RATE_MULTIPLIER);
            rangeCurve.Add(1 - key, 1, 0, 0);
            var steps = 50;
            var factor = Math.Pow(MAX_RATE_MULTIPLIER, 0.5 / steps);
            for (int i = 0; i < steps - 1; ++i)
            {
                key = key * factor;
                var value = 1 / (key * key) / MAX_RATE_MULTIPLIER;
                rangeCurve.Add(1 - key, value);
            }
            rangeCurve.Add(0, 1 / MAX_RATE_MULTIPLIER);
        }

        private void LogRangeCurve()
        {
            var cn = new ConfigNode();
            rangeCurve.Save(cn);
            Debug.Log("[MARO]\n" + cn.ToString());
        }

        private void OnScaleChanged(BaseField arg1, object arg2)
        {
            ReScale(true);
            UpdateConfiguration();
        }

        private void ReScale(bool translateParts) => part.Scale(Scale, translateParts);

        private void UpdateConfiguration()
        {
            SetMaxTxPower();
            SetupBaseFields();
            UpdatePawFields();
        }

        private void UpdatePawFields()
        {
            TxPowerString = TxPower.Format("W");
            TotalPowerString = TotalPower.Format("W");
            MinDataRateString = MinDataRate.Format("b/s", 1024);
            RangeDsnString = GetRange(antennaPower, DsnRange).Format("m");
            RangeSelfString = GetRange(antennaPower, antennaPower).Format("m");
            ElectronicsMassString = (ElectronicsMass * 1e6f).Format("g");
            TotalMassString = (TotalMass * 1e6f).Format("g");
        }

        private float GetRange(double a, double b) => (float)Math.Sqrt(a * b);

        private void SetMaxTxPower()
        {
            var maxTxPowerDbw = TechLevelInstance.MaxPower.ToDB();
            TxPowerDbw = Mathf.Clamp(TxPowerDbw, -13, maxTxPowerDbw);
            TxPowerDbwEdit.maxValue = maxTxPowerDbw;
        }

        private void OnFieldChanged(BaseField field, object oldValueObj)
        {
            UpdateConfiguration();
        }

        private void SetMaxTechLevel()
        {
            TechLevel = Mathf.Clamp(TechLevel, 0f, maxTechLevel);
            TechLevelEdit.maxValue = maxTechLevel;
        }

        private void SetupBaseFields()
        {
            antennaPower = GetMdtAntennaPower(TechLevelInstance);
            packetInterval = 0.5f;
            packetSize = MinDataRate * (_isKerbalismLoaded ? MAX_RATE_MULTIPLIER : 1) / 1024 / 1024 / 2;
            packetResourceCost = TotalPower / 1000;
            antennaType = AntennaType.RELAY;
            antennaCombinableExponent = antennaShape == AntennaShape.Dish ? 2f : 1f;
        }

        private double GetMdtAntennaPower(TechLevel tl) => GetMdtAntennaPower(tl, Scale);
        private double GetMdtAntennaPower(TechLevel tl, float scale) => GetMdtAntennaPower(referenceGain, tl.Gain, scale, TxPower, MinDataRate, GetFrequencyFactor(tl.Frequency, antennaShape, referenceFrequency));
        private double GetMdtAntennaPower(float refGain, float gain, float scale, float txPower, float minDataRate, float frequencyFactor) => BASE_POWER * (refGain + gain).FromDB() * scale * scale * txPower / minDataRate * frequencyFactor;
        private float GetFrequencyFactor(float frequency, AntennaShape antennaShape, float refFreq) => antennaShape == AntennaShape.Omni ? 1 : Mathf.Pow(frequency / refFreq, 2);

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit) => TotalMass - defaultMass;
        public float GetModuleCost(float defaultCost, ModifierStagingSituation sit) => TechLevelInstance.BaseCost + TechLevelInstance.CostPerWatt * TxPower + defaultCost * (Scale * Scale - 1);

        public ModifierChangeWhen GetModuleMassChangeWhen() => ModifierChangeWhen.FIXED;
        public ModifierChangeWhen GetModuleCostChangeWhen() => ModifierChangeWhen.FIXED;

        public override string GetModuleDisplayName() => "Configurable Antenna";

        public override string GetInfo()
        {
            var mdtPower = GetMdtAntennaPower(TechLevelInstance, MaxScale);
            return $"Max Range to DSN: <pos=66%><b><color=green>{GetRange(mdtPower, DsnRange).Format("m")}</color></b>\n" +
                $"Max Range to self: <pos=66%><b><color=green>{GetRange(mdtPower, mdtPower).Format("m")}</color></b>\n";
        }
    }
}