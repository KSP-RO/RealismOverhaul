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
        [KSPField]
        public Vector3 DescentModeCoM = new Vector3(0f, 0f, 0f);

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "CoM X-offset")]
        [UI_FloatEdit(scene = UI_Scene.Editor, minValue = -100f, maxValue = 100f, incrementLarge = 10f, incrementSmall = 1f, incrementSlide = 0.01f, sigFigs = 2)]
        public float offsetX = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "CoM Y-offset")]
        [UI_FloatEdit(scene = UI_Scene.Editor, minValue = -100f, maxValue = 100f, incrementLarge = 10f, incrementSmall = 1f, incrementSlide = 0.01f, sigFigs = 2)]
        public float offsetY = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "CoM Z-offset")]
        [UI_FloatEdit(scene = UI_Scene.Editor, minValue = -100f, maxValue = 100f, incrementLarge = 10f, incrementSmall = 1f, incrementSlide = 0.01f, sigFigs = 2)]
        public float offsetZ = 0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "CoM Offset Limit")]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = 0.01f)]
        public float offsetPercent = 1;

        [KSPField(isPersistant = true)]
        public bool IsDescentMode;

        [KSPField]
        public string comString;

        protected Vector3 _offsetCoM = new Vector3(0f, 0f, 0f);
        protected Vector3 _defaultCoM;
        
        [KSPEvent(guiName = "Turn Descent Mode On", guiActive = true, guiActiveEditor = true)]
        public virtual void ToggleMode()
        {
            IsDescentMode = !IsDescentMode;
            DescentModeChanged(IsDescentMode);
        }

        [KSPAction("Toggle Descent Mode")]
        public void Toggle(KSPActionParam param)
        {
            IsDescentMode = !IsDescentMode;
            DescentModeChanged(IsDescentMode);
        }

        public override void OnAwake()
        {
            base.OnAwake();
            _defaultCoM = part.CoMOffset;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            BindUI();
            DescentModeChanged(IsDescentMode);
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
            }

            bool configuredForDescentMode = DescentModeCoM != Vector3.zero;

            Actions[nameof(Toggle)].active = configuredForDescentMode;
            Events[nameof(ToggleMode)].active = configuredForDescentMode;

            Fields[nameof(offsetPercent)].guiActive = configuredForDescentMode;
            Fields[nameof(offsetPercent)].guiActiveEditor = configuredForDescentMode;
            Fields[nameof(offsetX)].guiActiveEditor = !configuredForDescentMode;
            Fields[nameof(offsetY)].guiActiveEditor = !configuredForDescentMode;
            Fields[nameof(offsetZ)].guiActiveEditor = !configuredForDescentMode;
        }

        protected void OffsetPercentChanged(BaseField bf, object o)
        {
            UpdateCoM();
        }

        protected void CoMOffsetChanged(BaseField bf, object o)
        {
            UpdateCoM();
        }

        protected void DescentModeChanged(bool isEnabled)
        {
            Events[nameof(ToggleMode)].guiName = isEnabled ? "Turn Descent Mode Off" : "Turn Descent Mode On";
            UpdateCoM();
        }

        protected void UpdateCoM()
        {
            if (DescentModeCoM != Vector3.zero)
            {
                if (IsDescentMode)
                    part.CoMOffset = DescentModeCoM * offsetPercent + _defaultCoM;
                else
                    part.CoMOffset = _defaultCoM;
            }
            else
            {
                _offsetCoM.x = offsetX / 100f;
                _offsetCoM.y = offsetY / 100f;
                _offsetCoM.z = offsetZ / 100f;
                part.CoMOffset = _defaultCoM + _offsetCoM;
            }

            Fields[nameof(comString)].guiActive = PhysicsGlobals.ThermalDataDisplay;
            if (PhysicsGlobals.ThermalDataDisplay)
            {
                comString = part.CoMOffset.x.ToString("N3") + "," + part.CoMOffset.y.ToString("N3") + "," + part.CoMOffset.z.ToString("N3");
            }
        }
    }
}
