using UnityEngine;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class FlightEventsHandler : MonoBehaviour
    {
        private bool vesselLoadOnSceneChange;

        private void Start()
        {
            GameEvents.onSetSpeedMode.Add(onSetSpeedMode);
            GameEvents.onVesselChange.Add(onVesselChange);
            vesselLoadOnSceneChange = true;
        }

        // Detect active vessel changed by switching to an unloaded vessel (flight scene was rebuilt)
        private void FixedUpdate()
        {
            if (vesselLoadOnSceneChange && FlightGlobals.ActiveVessel != null)
            {
                foreach (VesselModule vm in FlightGlobals.ActiveVessel.vesselModules)
                {
                    VesselModuleRotationRO vmr = vm as VesselModuleRotationRO;
                    if (vmr != null)
                    {
                        vmr.vesselSASHasChanged = true;
                        vesselLoadOnSceneChange = false;
                        break;
                    }
                }
            }
        }


        // Detect active vessel change when switching vessel in the physics bubble
        private void onVesselChange(Vessel v)
        {
            foreach (VesselModule vm in v.vesselModules)
            {
                VesselModuleRotationRO vmr = vm as VesselModuleRotationRO;
                if (vmr != null)
                {
                    vmr.vesselSASHasChanged = true;
                    break;
                }
            }
        }

        // Detect navball context (orbit/surface/target) changes
        private void onSetSpeedMode(FlightGlobals.SpeedDisplayModes mode)
        {
            foreach (VesselModule vm in FlightGlobals.ActiveVessel.vesselModules)
            {
                VesselModuleRotationRO vmr = vm as VesselModuleRotationRO;
                if (vmr != null)
                {
                    vmr.autopilotContextCurrent = (int)mode;
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            GameEvents.onSetSpeedMode.Remove(onSetSpeedMode);
            GameEvents.onVesselChange.Remove(onVesselChange);
        }
    }

    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    class GameScenesEventsHandler : MonoBehaviour
    {
        private void Start()
        {
            GameEvents.onVesselSOIChanged.Add(onVesselSOIChanged);
        }

        // On SOI change, if target hold is in a body relative mode, set it to false and reset autopilot mode to stability assist
        private void onVesselSOIChanged(GameEvents.HostedFromToAction<Vessel, CelestialBody> data)
        {
            if (data.host.loaded)
            {
                if (data.host.vesselModules != null)
                {
                    foreach (VesselModule vm in data.host.vesselModules)
                    {
                        VesselModuleRotationRO vmr = vm as VesselModuleRotationRO;
                        if (vmr != null)
                        {
                            if (vmr.autopilotTargetHold && vmr.autopilotMode >= 1 && vmr.autopilotMode <= 6)
                            {
                                vmr.autopilotTargetHold = false;
                                vmr.autopilotMode = 0;
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                if (data.host.protoVessel.vesselModules == null)
                    return;

                ConfigNode vmrNode = data.host.protoVessel.vesselModules.GetNode("VesselModuleRotationRO");
                if (vmrNode == null)
                    return;

                bool autopilotTargetHoldCurrent = false;
                int autopilotModeCurrent = 0;
                if (!vmrNode.TryGetValue("autopilotTargetHold", ref autopilotTargetHoldCurrent))
                    return;
                if (!vmrNode.TryGetValue("autopilotMode", ref autopilotModeCurrent))
                    return;

                if (autopilotTargetHoldCurrent
                    && autopilotModeCurrent >= 1
                    && autopilotModeCurrent <= 6)
                {
                    vmrNode.SetValue("autopilotTargetHold", false);
                    vmrNode.SetValue("autopilotMode", 0);
                }
            }
        }

        private void OnDestroy()
        {
            GameEvents.onVesselSOIChanged.Remove(onVesselSOIChanged);
        }
    }
}
