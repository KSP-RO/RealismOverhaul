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

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tx Power (dBmW)", guiUnits = "dBmW", guiFormat = "F0"), UI_FloatRange(minValue = 18, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float txPowerDbmw = TechLevel.GetTechLevel(0).MaxPower.ToDBm();
        private UI_FloatRange TxPowerDbmwEdit => (UI_FloatRange)Fields[nameof(txPowerDbmw)].uiControlEditor;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tech Level", guiUnits = "#", guiFormat = "F0"), UI_FloatRange(minValue = 0, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float techLevel = -1;
        private UI_FloatRange TechLevelEdit => (UI_FloatRange)Fields[nameof(techLevel)].uiControlEditor;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Scale"), UI_ChooseOption(scene = UI_Scene.Editor)]
        public int ScaleIndex = HALF_SCALE_STEPS;
        private UI_ChooseOption ScaleEdit => (UI_ChooseOption)Fields[nameof(ScaleIndex)].uiControlEditor;
        public float Scale => GetScaleFromIndex(ScaleIndex);

        [KSPField]
        public int maxTechLevel = 0;

        public int DefaultTechLevel { get; set; } = -1;

        [KSPField]
        public float diameter = 0f;

        [KSPField]
        public float referenceGain = 0f;

        [KSPField]
        public float referenceFrequency = 8415f;

        [KSPField]
        public AntennaShape antennaShape = AntennaShape.Auto;

        [KSPField]
        public bool isScalable = true;

        private bool _isKerbalismLoaded;

        private TechLevel TechLevelInstance => TechLevel.GetTechLevel((int)techLevel);
        private Part PartPrefab => part.partInfo.partPrefab;

        private float TxPower => txPowerDbmw.FromDBm();
        private float TotalUsedPower => TxUsedPower + TechLevelInstance.BasePower;
        public float TxUsedPower => GetTxUsedPower(txPowerDbmw, TechLevelInstance);
        public float MinDataRate => TechLevelInstance.MinDataRate * DataRateExponent.FromLog2();
        public float MaxDataRate => TechLevelInstance.MaxDataRate;
        private double DsnRange => GameVariables.Instance.GetDSNRange(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation));
        private float ElectronicsMass => GetElectronicsMass(TechLevelInstance, TxPower);
        private float AntennaMass => PartPrefab.mass * Mathf.Pow(Scale, ANTENNA_MASS_SCALING_EXPONENT);
        private float TotalMass => AntennaMass + ElectronicsMass;
        private bool IsScalable => isScalable && antennaShape != AntennaShape.Omni;
        private float MaxScale => IsScalable ? Mathf.Pow(SCALE_RANGE, 0.5f) : 1;

        private float GetElectronicsMass(TechLevel techLevel, float txPower) => (techLevel.BaseMass + techLevel.MassPerWatt * txPower) / 1000;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            SetupLoadedState();
        }

        public void Start()
        {
            _isKerbalismLoaded = AssemblyLoader.loadedAssemblies.Any(x => x.name.StartsWith("Kerbalism"));
            SetInitialScale();
            SetDefaultTechLevel();
            SetMaxTechLevel();
            UpdateConfiguration();
            SetupPaw();
        }


        private void SetDefaultTechLevel() => DefaultTechLevel = (int)techLevel;

        private void SetInitialScale()
        {
            if (IsScalable)
            {
                ReScale(false);
            }
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
                referenceFrequency = 1;
                referenceGain = (ANTENNA_EFFICIENCY * Mathf.Pow(Mathf.PI * referenceFrequency * 1e6f / SPEED_OF_LIGHT * diameter, 2)).ToDBm();
            }
        }

        private void ApplyUpgrades()
        {
            if (techLevel == -1 && maxTechLevel > 0)
            {
                techLevel = maxTechLevel;
                txPowerDbmw = TechLevelInstance.MaxPower.ToDBm();
            }
        }

        double ICommAntenna.CommPowerUnloaded(ProtoPartModuleSnapshot mSnap)
        {
            return GetAntennaSpecs(mSnap).CommNetPower;
        }

        public AntennaSpecs GetAntennaSpecs(ProtoPartModuleSnapshot mSnap)
        {
            var values = mSnap.moduleValues;
            var techLevel = values.GetInt(nameof(ModuleConfigurableAntenna.techLevel));
            var scaleIndex = values.GetInt(nameof(ScaleIndex));
            var dataRateExponent = values.GetInt(nameof(DataRateExponent));
            var txPowerDbmw = values.GetFloat(nameof(ModuleConfigurableAntenna.txPowerDbmw));
            var tl = TechLevel.GetTechLevel(techLevel);
            var frequencyFactor = GetFrequencyFactor(tl.Frequency, antennaShape, referenceFrequency);
            var txUsedPower = GetTxUsedPower(txPowerDbmw, tl);
            var commPower = GetMdtAntennaPower(referenceGain, tl.Gain, GetScaleFromIndex(scaleIndex), txPowerDbmw.FromDBm(), dataRateExponent.FromLog2() * tl.MinDataRate, frequencyFactor);
            return new AntennaSpecs(txUsedPower, txUsedPower + tl.BasePower, commPower, tl.MinDataRate, tl.MaxDataRate);
        }

        private static float GetTxUsedPower(float txPowerDbmw, TechLevel tl) => txPowerDbmw.FromDBm() / tl.Efficiency;

        public AntennaSpecs GetAntennaSpecs()
        {
            var tl = TechLevelInstance;
            var frequencyFactor = GetFrequencyFactor(tl.Frequency, antennaShape, referenceFrequency);
            var txUsedPower = GetTxUsedPower(txPowerDbmw, tl);
            var commPower = GetMdtAntennaPower(referenceGain, tl.Gain, GetScaleFromIndex(ScaleIndex), txPowerDbmw.FromDBm(), MinDataRate, frequencyFactor);
            return new AntennaSpecs(txUsedPower, txUsedPower + tl.BasePower, commPower, tl.MinDataRate, tl.MaxDataRate);
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
                Fields[nameof(TotalMassString)].guiActiveEditor = false;
                Fields[nameof(TotalMassString)].guiActive = false;
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
            TxPowerDbmwEdit.onFieldChanged = OnFieldChanged;
            TxPowerDbmwEdit.onSymmetryFieldChanged = OnFieldChanged;
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
            rangeCurve.Add(0, 0, 0, 1);
            rangeCurve.Add(1, 1, 1, 0);
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
            SetupIdlePower();
            UpdatePawFields();
        }

        private void SetupIdlePower()
        {
            var electricCharge = resHandler.inputResources.First(x => x.id == PartResourceLibrary.ElectricityHashcode);
            electricCharge.rate = TechLevelInstance.BasePower / 1000;
        }

        public void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                ConsumeEc();
            }
        }

        private void ConsumeEc()
        {
            string status = "";
            resHandler.UpdateModuleResourceInputs(ref status, 1, 1, true, false);
        }

        private void UpdatePawFields()
        {
            TxPowerString = TxPower.Format("W");
            TotalPowerString = TotalUsedPower.Format("W");
            MinDataRateString = MinDataRate.Format("b/s", 1024);
            RangeDsnString = GetRange(antennaPower, DsnRange).Format("m");
            RangeSelfString = GetRange(antennaPower, antennaPower).Format("m");
            ElectronicsMassString = (ElectronicsMass * 1e6f).Format("g");
            TotalMassString = (TotalMass * 1e6f).Format("g");
        }

        private float GetRange(double a, double b) => (float)Math.Sqrt(a * b);

        private void SetMaxTxPower()
        {
            var maxTxPowerDbmw = TechLevelInstance.MaxPower.ToDBm();
            txPowerDbmw = Mathf.Clamp(txPowerDbmw, -13, maxTxPowerDbmw);
            TxPowerDbmwEdit.maxValue = maxTxPowerDbmw;
        }

        private void OnFieldChanged(BaseField field, object oldValueObj)
        {
            UpdateConfiguration();
        }

        private void SetMaxTechLevel()
        {
            techLevel = Mathf.Clamp(techLevel, 0f, maxTechLevel);
            TechLevelEdit.maxValue = maxTechLevel;
        }

        private void SetupBaseFields()
        {
            antennaPower = GetMdtAntennaPower(TechLevelInstance);
            packetInterval = 0.5f;
            packetSize = MinDataRate / 1024 / 1024 / 2;
            packetResourceCost = TxUsedPower / 1000;
            antennaType = AntennaType.RELAY;
            antennaCombinableExponent = antennaShape == AntennaShape.Dish ? 2f : 1f;
        }

        private double GetMdtAntennaPower(TechLevel tl) => GetMdtAntennaPower(tl, Scale);
        private double GetMdtAntennaPower(TechLevel tl, float scale) => GetMdtAntennaPower(referenceGain, tl.Gain, scale, TxPower, MinDataRate, GetFrequencyFactor(tl.Frequency, antennaShape, referenceFrequency));
        private double GetMdtAntennaPower(float refGain, float gain, float scale, float txPower, float minDataRate, float frequencyFactor) => BASE_POWER * (refGain + gain).FromDBm() * scale * scale * txPower / minDataRate * frequencyFactor;
        private float GetFrequencyFactor(float frequency, AntennaShape antennaShape, float refFreq) => antennaShape == AntennaShape.Omni ? 1 : Mathf.Pow(frequency / refFreq, 2);

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit) => TotalMass - defaultMass - GetDefaultConfigurationMass();

        private float GetDefaultConfigurationMass()
        {
            var prefabAntenna = PartPrefab.FindModuleImplementing<ModuleConfigurableAntenna>();
            return prefabAntenna.DefaultTechLevel > -1 ? GetElectronicsMass(TechLevel.GetTechLevel((int)prefabAntenna.techLevel), prefabAntenna.txPowerDbmw.FromDBm()) : 0;
        }

        public float GetModuleCost(float defaultCost, ModifierStagingSituation sit) => GetElectronicsCost(TechLevelInstance, TxPower) + defaultCost * (Scale * Scale - 1) - GetDefaultConfigurationCost();

        private float GetDefaultConfigurationCost()
        {
            var prefabAntenna = PartPrefab.FindModuleImplementing<ModuleConfigurableAntenna>();
            return prefabAntenna.DefaultTechLevel > -1 ? GetElectronicsCost(TechLevel.GetTechLevel((int)prefabAntenna.techLevel), prefabAntenna.txPowerDbmw.FromDBm()) : 0;
        }

        private float GetElectronicsCost(TechLevel techLevel, float txPower) => techLevel.BaseCost + techLevel.CostPerWatt * txPower;

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