using UnityEngine;

namespace RealismOverhaul
{
    /// <summary>
    /// Use for enabling a Descent mode on capsules or for balancing out the torque on probe.
    /// Has 2 modes: 
    /// * When the DescentModeCoM field is set with a MM patch, the module is assumed to be configured for descent mode.
    /// * Otherwise the module will show 3-axis CoM offset configuration sliders inside the VAB scene.
    /// </summary>
    public class AdjustableCoMShifter : PartModule
    {
        public const string groupName = "CoMShifter";
        public const string groupDisplayName = "CoM Shifter";

        [KSPField]
        public Vector3 DescentModeCoM = Vector3.zero;   // If configured, disable customization

        /// <summary>
        /// Maximum allowed offset as a percentage of the part's dimension.
        /// </summary>
        [KSPField]
        public float maxDimensionOffsetPercent = 0.75f;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiName = "CoM X-offset", guiFormat = "F2", guiUnits = "m", groupName = groupName, groupDisplayName = groupDisplayName)]
        [UI_FloatEdit(scene = UI_Scene.Editor, minValue = -1f, maxValue = 1f, incrementLarge = 0.1f, incrementSmall = 0.01f, incrementSlide = 0.001f, sigFigs = 3)]
        public float offsetX = 0;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiName = "CoM Y-offset", guiFormat = "F2", guiUnits = "m", groupName = groupName)]
        [UI_FloatEdit(scene = UI_Scene.Editor, minValue = -1f, maxValue = 1f, incrementLarge = 0.1f, incrementSmall = 0.01f, incrementSlide = 0.001f, sigFigs = 3)]
        public float offsetY = 0;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiName = "CoM Z-offset", guiFormat = "F2", guiUnits = "m", groupName = groupName)]
        [UI_FloatEdit(scene = UI_Scene.Editor, minValue = -1f, maxValue = 1f, incrementLarge = 0.1f, incrementSmall = 0.01f, incrementSlide = 0.001f, sigFigs = 3)]
        public float offsetZ = 0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiFormat = "P0", guiName = "CoM Offset Limit", groupName = groupName, groupDisplayName = groupDisplayName)]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = 0.01f)]
        public float offsetPercent = 1;

        [KSPField(isPersistant = true)]
        public bool IsDescentMode;

        [KSPField(guiName = "CoM", groupName = groupName, groupDisplayName = groupDisplayName)]
        public string comString;

        protected Vector3 defaultCoM;
        public bool ConfiguredForDescentMode => DescentModeCoM != Vector3.zero;
        
        [KSPEvent(guiName = "Turn Descent Mode On", guiActive = true, guiActiveEditor = true, groupName = groupName)]
        public virtual void ToggleMode()
        {
            IsDescentMode = !IsDescentMode;
            DescentModeChanged(IsDescentMode);
        }

        [KSPAction("Toggle Descent Mode")]
        public void Toggle(KSPActionParam _)
        {
            IsDescentMode = !IsDescentMode;
            DescentModeChanged(IsDescentMode);
        }

        public override void OnStartFinished(StartState state)
        {
            defaultCoM = part.CoMOffset;
            BindUI();
            DescentModeChanged(IsDescentMode);
        }

        internal void Update()
        {
            Fields[nameof(comString)].guiActive = PhysicsGlobals.ThermalDataDisplay;
        }

        internal void OnDestroy()
        {
            GameEvents.onEditorShipModified.Remove(OnEditorShipModified);
        }

        protected void BindUI()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                Fields[nameof(offsetPercent)].uiControlFlight.onFieldChanged += OffsetPercentChanged;
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                Fields[nameof(offsetPercent)].uiControlEditor.onFieldChanged += OffsetPercentChanged;
                Fields[nameof(offsetX)].uiControlEditor.onFieldChanged += CoMOffsetChanged;
                Fields[nameof(offsetY)].uiControlEditor.onFieldChanged += CoMOffsetChanged;
                Fields[nameof(offsetZ)].uiControlEditor.onFieldChanged += CoMOffsetChanged;

                UpdateSliderLimits();
                GameEvents.onEditorShipModified.Add(OnEditorShipModified);
            }

            Actions[nameof(Toggle)].active = ConfiguredForDescentMode;
            Events[nameof(ToggleMode)].active = ConfiguredForDescentMode;

            Fields[nameof(offsetPercent)].guiActive = ConfiguredForDescentMode;
            Fields[nameof(offsetPercent)].guiActiveEditor = ConfiguredForDescentMode;

            Fields[nameof(offsetX)].guiActiveEditor = !ConfiguredForDescentMode;
            Fields[nameof(offsetY)].guiActiveEditor = !ConfiguredForDescentMode;
            Fields[nameof(offsetZ)].guiActiveEditor = !ConfiguredForDescentMode;
        }

        private void OnEditorShipModified(ShipConstruct sc)
        {
            UpdateSliderLimits();
        }

        protected void UpdateSliderLimits()
        {
            Vector3 extents = PartGeometryUtil.MergeBounds(part.GetRendererBounds(), part.partTransform).extents;
            var xEdit = Fields[nameof(offsetX)].uiControlEditor as UI_FloatEdit;
            var yEdit = Fields[nameof(offsetY)].uiControlEditor as UI_FloatEdit;
            var zEdit = Fields[nameof(offsetZ)].uiControlEditor as UI_FloatEdit;
            if (xEdit != null && yEdit != null && zEdit != null)
            {
                xEdit.minValue = -extents.x * maxDimensionOffsetPercent;
                xEdit.maxValue = extents.x * maxDimensionOffsetPercent;
                yEdit.minValue = -extents.y * maxDimensionOffsetPercent;
                yEdit.maxValue = extents.y * maxDimensionOffsetPercent;
                zEdit.minValue = -extents.z * maxDimensionOffsetPercent;
                zEdit.maxValue = extents.z * maxDimensionOffsetPercent;

                Mathf.Clamp(offsetX, xEdit.minValue, xEdit.maxValue);
                Mathf.Clamp(offsetY, yEdit.minValue, yEdit.maxValue);
                Mathf.Clamp(offsetZ, zEdit.minValue, zEdit.maxValue);
            }
        }

        protected void OffsetPercentChanged(BaseField bf, object o) => UpdateCoM();
        protected void CoMOffsetChanged(BaseField bf, object o) => UpdateCoM();

        protected void DescentModeChanged(bool isEnabled)
        {
            Events[nameof(ToggleMode)].guiName = $"Turn Descent Mode {(isEnabled ? "Off" : "On")}";
            UpdateCoM();
        }

        protected void UpdateCoM()
        {
            part.CoMOffset = defaultCoM;
            if (ConfiguredForDescentMode)
                part.CoMOffset += IsDescentMode ? DescentModeCoM * offsetPercent : Vector3.zero;
            else
                part.CoMOffset += new Vector3(offsetX, offsetY, offsetZ);

            comString = $"({part.CoMOffset.x:F2},{part.CoMOffset.y:F2},{part.CoMOffset.z:F2})";
        }
    }
}
