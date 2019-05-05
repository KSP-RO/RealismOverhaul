using System;
using System.Linq;
using UnityEngine;

namespace RealismOverhaul.Communications
{
    class ModuleAntennaRO : ModuleDataTransmitter, IPartMassModifier
    {
        private const double BASE_POWER = 84610911.3771648;
        private const int MAX_RATE_EXPONENT = 20;
        private const float MAX_RATE_MULTIPLIER = 1 << MAX_RATE_EXPONENT;
        private const int HALF_SCALE_STEPS = 8;
        private const float ANTENNA_MASS_SCALING_EXPONENT = 2.0f;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Tx Power")]
        public string TxPowerString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Total Power")]
        public string TotalPowerString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Min. Data Rate")]
        public string MinDataRateString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Range to DSN")]
        public string RangeDsnString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Range to Self")]
        public string RangeSelfString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Electronics Mass")]
        public string ElectronicsMassString;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Total Mass")]
        public string TotalMassString;

        [KSPField(isPersistant = true, guiActiveEditor = false, guiName = "Data Rate"), UI_ChooseOption(scene = UI_Scene.Flight)]
        public int DataRateExponent = 0;
        private UI_ChooseOption DataRateExponentEdit => (UI_ChooseOption)Fields[nameof(DataRateExponent)].uiControlFlight;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tx Power (dBW)", guiUnits = "dBW", guiFormat = "F0"), UI_FloatRange(minValue = -12, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float TxPowerDbw = 0;
        private UI_FloatRange TxPowerDbwEdit => (UI_FloatRange)Fields[nameof(TxPowerDbw)].uiControlEditor;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Tech Level", guiUnits = "#", guiFormat = "F0"), UI_FloatRange(minValue = 0, stepIncrement = 1, scene = UI_Scene.Editor)]
        public float TechLevel = 99;
        private UI_FloatRange TechLevelEdit => (UI_FloatRange)Fields[nameof(TechLevel)].uiControlEditor;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Scale"), UI_ChooseOption(scene = UI_Scene.Editor)]
        public int ScaleIndex = HALF_SCALE_STEPS;
        private UI_ChooseOption ScaleEdit => (UI_ChooseOption)Fields[nameof(ScaleIndex)].uiControlEditor;
        public float Scale => GetScaleFromIndex(ScaleIndex);

        [KSPField]
        public int maxTechLevel = 0;

        [KSPField]
        public float referenceGain = 0f;

        [KSPField]
        public float referenceFrequency = 8415f;

        [KSPField]
        public AntennaShape antennaShape = AntennaShape.Auto;

        private bool _isKerbalismLoaded;
        private Vector3 defaultTransformScale = new Vector3(0f, 0f, 0f);

        private TechLevel TechLevelInstance => Communications.TechLevel.GetTechLevel((int)TechLevel);

        private float TxPower => FromDB(TxPowerDbw);
        private float TotalPower => TxPower / TechLevelInstance.Efficiency + TechLevelInstance.BasePower;
        private float MinDataRate => TechLevelInstance.MinDataRate * FromLog2(DataRateExponent);
        private double DsnRange => GameVariables.Instance.GetDSNRange(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation));
        private float ElectronicsMass => (TechLevelInstance.BaseMass + TechLevelInstance.MassPerWatt * TxPower) / 1000;
        private float AntennaMass => part.prefabMass * Mathf.Pow(Scale, ANTENNA_MASS_SCALING_EXPONENT);
        private float TotalMass => AntennaMass + ElectronicsMass;

        private Part PartPrefab => part.partInfo.partPrefab;


        public void Start()
        {
            Debug.Log("[MARO] Start");
            _isKerbalismLoaded = AssemblyLoader.loadedAssemblies.Select(x => x.name).Any(x => x.StartsWith("Kerbalism"));
            SetupInitialState();
            UpdateConfiguration();
            SetupPaw();
        }

        private void SetupInitialState()
        {
            SetupRangeCurve();
            SetAntennaShape();
            SetMaxTechLevel();
        }

        public override void OnStart(StartState state)
        {
            Debug.Log("[MARO] ONSTART");
            base.OnStart(state);
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

            ScaleEdit.options = CreateScaleOptions();
        }

        private string[] CreateDataRateOptions()
        {
            var result = new string[MAX_RATE_EXPONENT + 1];
            for (int i = 0; i <= MAX_RATE_EXPONENT; ++i)
            {
                result[i] = Format(MinDataRate * (1 << i), "b/s", 1024);
            }
            return result;
        }

        private string[] CreateScaleOptions()
        {
            var stepCount = HALF_SCALE_STEPS * 2 + 1;
            var result = new string[stepCount];
            for (int i = 0; i < stepCount; ++i)
            {
                result[i] = Format(GetScaleFromIndex(i) * 100, "%");
            }
            return result;
        }

        private float GetScaleFromIndex(int i) => Mathf.Pow(2, ((float) i / HALF_SCALE_STEPS - 1) / 2);

        public override void OnAwake()
        {
            Debug.Log("[MARO] ONAWAKE");
            base.OnAwake();
        }

        public override void OnInitialize()
        {
            Debug.Log("[MARO] ONINITIALIZE");
            base.OnInitialize();
        }

        public override void OnLoad(ConfigNode node)
        {
            Debug.Log("[MARO] ONLOAD");
            base.OnLoad(node);
            SetupUnloadedVessels();
        }

        private void SetupUnloadedVessels()
        {
            SetupInitialState();
            SetupBaseFields();
        }

        private void SetupChangeListeners()
        {
            Debug.Log("[MARO] SETUP CL");
            DataRateExponentEdit.onFieldChanged = OnFieldChanged;
            DataRateExponentEdit.onSymmetryFieldChanged = OnFieldChanged;
            TxPowerDbwEdit.onFieldChanged = OnFieldChanged;
            TxPowerDbwEdit.onSymmetryFieldChanged = OnFieldChanged;
            TechLevelEdit.onFieldChanged = OnFieldChanged;
            TechLevelEdit.onSymmetryFieldChanged = OnFieldChanged;
            ScaleEdit.onFieldChanged = OnFieldChanged;
            ScaleEdit.onSymmetryFieldChanged = OnFieldChanged;
        }

        private void SetAntennaShape()
        {
            if (antennaShape == AntennaShape.Auto)
            {
                antennaShape = referenceGain > 8 ? AntennaShape.Dish : AntennaShape.Omni;
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

        private void UpdateConfiguration()
        {
            ReScale(Scale);
            SetMaxTxPower();
            SetupBaseFields();
            UpdatePawFields();
        }

        private void ReScale(float scale)
        {
            Debug.Log($"[MARO] Changing partScale to: {scale}");
            Debug.Log($"[MARO] prefab rescaleFactor: {part.partInfo.partPrefab.rescaleFactor}");
            part.scaleFactor = PartPrefab.scaleFactor * scale;
            var modelTransform = GetModelTransform(part);
            if (modelTransform != null)
            {
                Debug.Log($"[MARO] Old Scale: {modelTransform.localScale}");
                if (defaultTransformScale.x == 0.0f)
                {
                    defaultTransformScale = GetModelTransform(PartPrefab).localScale;
                    Debug.Log($"[MARO] Setting def transform: {defaultTransformScale}");
                }
                Debug.Log($"[MARO] Setting transform: to {scale * defaultTransformScale}");
                modelTransform.localScale = scale * defaultTransformScale;
                modelTransform.hasChanged = true;
                part.partTransform.hasChanged = true;
            }
        }

        private static Transform GetModelTransform(Part p) => p.partTransform.Find("model");

        private void UpdatePawFields()
        {
            TxPowerString = Format(TxPower, "W");
            TotalPowerString = Format(TotalPower, "W");
            MinDataRateString = Format(MinDataRate, "b/s", 1024);
            RangeDsnString = Format(GetRange(antennaPower, DsnRange), "m");
            RangeSelfString = Format(GetRange(antennaPower, antennaPower), "m");
            ElectronicsMassString = Format(ElectronicsMass * 1000000, "g");
            TotalMassString = Format(TotalMass * 1000000, "g");
        }

        private float GetRange(double a, double b) => (float)Math.Sqrt(a * b);

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

        private void OnFieldChanged(BaseField field, object oldValueObj)
        {
            Debug.Log($"[MARO] Field changed");
            UpdateConfiguration();
        }

        private void SetMaxTechLevel()
        {
            Debug.Log($"[MARO] max tl: {maxTechLevel}");
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
            antennaCombinableExponent = antennaShape == AntennaShape.Dish ? 2f : 1f;

            Debug.Log($"[MARO] mass: {(TechLevelInstance.BaseMass + TechLevelInstance.MassPerWatt * TxPower) / 1000}");
        }

        private double GetMdtAntennaPower(TechLevel tl)
        {
            return BASE_POWER * FromDB(referenceGain + tl.Gain) * Scale * Scale * TxPower / MinDataRate * GetFrequencyFactor(tl);
        }

        private double GetFrequencyFactor(TechLevel tl)
        {
            return antennaShape == AntennaShape.Omni ? 1 : Math.Pow(tl.Frequency / referenceFrequency, 2);
        }

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit)
        {
            return TotalMass - defaultMass;
        }

        public ModifierChangeWhen GetModuleMassChangeWhen() => ModifierChangeWhen.FIXED;

        private float ToLog2(float value) => Mathf.Log(value) / Mathf.Log(2);
        private float FromLog2(float value) => Mathf.Pow(2, value);

        private float ToDB(float value) => Mathf.Log10(value) * 10;
        private float FromDB(float value) => Mathf.Pow(10, value / 10f);
    }
}