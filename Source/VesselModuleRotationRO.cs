using System;
using UnityEngine;


// Modified version of the VesselModule of the same name in MandatoryRCS by gotmachine
// https://github.com/gotmachine/MandatoryRCS
namespace RealismOverhaul
{
    public class VesselModuleRotationRO : VesselModule
    {
        [KSPField(isPersistant = true)]
        public Vector3 angularVelocity;

        // True if we are keeping the vessel oriented toward the SAS target
        [KSPField(isPersistant = true)]
        public bool autopilotTargetHold;

        // Enum VesselAutopilot.AutopilotMode
        [KSPField(isPersistant = true)]
        public int autopilotMode;

        // Enum FlightGlobals.SpeedDisplayModes
        // 0=orbit, 1=surface, 2=target, updated trough SpeedModeListener
        [KSPField(isPersistant = true)]
        public int autopilotContext;

        [KSPField(isPersistant = true)]
        public double lastUT = -1d;

        // Restore the angular velocity when loading / switching vessels
        private bool restoreAngularVelocity = false;

        // Apply the rotation toward the SAS selection when loading / switching vessels
        private bool restoreAutopilotTarget = false;

        // If set true by OnVesselChange event, we will try to restore the previous SAS selection
        public bool vesselSASHasChanged = false;

        // Var used to retry setting the SAS selection when loading / switching vessels
        private bool retrySAS = false;
        private int retrySASCount;
        private int setSASMode;

        // variable used to check if things have changed since last fixedUpdate
        // or when loading / switching vessels
        private double lastManeuverParameters;
        private object lastTarget = null;
        public int autopilotContextCurrent;

        // SAS target direction is made available for the ModuleTorqueController
        public Vector3 targetDirection;

        // Note this is the magnitude, so 0.5 implies 0.3deg/sec in each axis, give or take.
        private const float RotationThreshold = 0.1f * Mathf.Deg2Rad;
        private const float RotationThresholdSAS = 0.5f * Mathf.Deg2Rad;

        private static double GetUT() => HighLogic.LoadedSceneIsEditor ? HighLogic.CurrentGame.UniversalTime : Planetarium.GetUniversalTime();

        private bool isEnabled = true;
        private bool shouldCheckEnabled = true;

        private bool CheckEnabled()
        {
            shouldCheckEnabled = false;
            foreach (var a in AssemblyLoader.loadedAssemblies)
            {
                // ksp_plugin_adapter is Principia
                if (a.name == "PersistentRotation" || a.name == "PersistentRotationUpgraded" || a.name == "ksp_plugin_adapter" || a.name == "MandatoryRCS")
                {
                    isEnabled = false;
                    break;
                }
            }
            return isEnabled;
        }

        private bool IsEnabled => shouldCheckEnabled ? CheckEnabled() : isEnabled;

        private bool IsOverThreshold(Vector3 rot)
        {
            float thresh = Vessel.Autopilot.Enabled ? RotationThresholdSAS : RotationThreshold;
            return Mathf.Abs(rot.x) > thresh || Mathf.Abs(rot.y) > thresh || Mathf.Abs(rot.z) > thresh;
        }

        private void FixedUpdate()
        {
            if (!IsEnabled)
                return;

            if (Vessel.loaded)
            {
                targetDirection = AutopilotTargetDirection();

                // Vessel is loaded but not in physics, either because
                // - It is in the physics bubble but in non-physics timewarp
                // - It has gone outside of the physics bubble
                // - It was just loaded, is in the physics bubble and will be unpacked in a few frames
                if (Vessel.packed)
                {
                    // Check if target / maneuver is modified/deleted during timewarp
                    if (autopilotTargetHold && TimeWarp.WarpMode == TimeWarp.Modes.HIGH && TimeWarp.CurrentRateIndex > 0)
                    {
                        autopilotTargetHold = TargetHoldValidity();
                    }

                    // If angular velocity is over the threshold, rotate--even if we have SAS on. No cheating Newton!
                    // Note this is the magnitude, so 0.5 implies 0.3deg/sec in each axis, give or take.
                    if (IsOverThreshold(angularVelocity))
                    {
                        RotatePacked();
                    }
                    // We keep the vessel rotated toward the SAS target
                    else if (autopilotTargetHold)
                    {
                        RotateTowardTarget();
                    }
                }
                else if (FlightGlobals.ready) // The vessel is in physics simulation and fully loaded
                {
                    bool okToSaveAngularVelocity = true;
                    // Restoring previous SAS selection after a vessel change
                    if (vesselSASHasChanged)
                    {
                        vesselSASHasChanged = false;
                        if (!RestoreSASMode(autopilotMode))
                        {
                            retrySAS = true;
                            setSASMode = autopilotMode;
                            retrySASCount = 10;
                        }
                    }

                    // Restoring angular velocity or rotation after entering physics
                    if (restoreAutopilotTarget) // Rotate to face SAS target
                    {
                        if (autopilotContext == autopilotContextCurrent) // Abort if the navball context (orbit/surface/target) has changed
                        {
                            // Debug.Log("[US] " + Vessel.vesselName + " going OFF rails : applying rotation toward SAS target, autopilotMode=" + autopilotMode + ", targetMode=" + autopilotContext);
                            RotateTowardTarget();
                        }
                        restoreAutopilotTarget = false;
                    }
                    if (restoreAngularVelocity) // Restoring saved rotation if it was above the threshold
                    {
                        // Debug.Log("[US] " + Vessel.vesselName + " going OFF rails : restoring angular velocity, angvel=" + angularVelocity.magnitude);
                        if (IsOverThreshold(angularVelocity))
                        {
                            ApplyAngularVelocity();
                            okToSaveAngularVelocity = false;
                        }
                        restoreAngularVelocity = false;
                    }

                    // Sometimes the autopilot wasn't loaded fast enough, so we retry setting the SAS mode a few times
                    if (retrySAS)
                    {
                        if (retrySASCount > 0)
                        {
                            if (RestoreSASMode(setSASMode))
                            {
                                retrySAS = false;
                                // Debug.Log("[US] autopilot mode " + setSASMode + " set at count " + retrySASCount);
                            }
                            retrySASCount--;
                        }
                        else
                        {
                            retrySAS = false;
                            // Debug.Log("[US] can't set autopilot mode.");
                        }
                    }

                    // Saving angular velocity (if we can), SAS mode, and check target hold status
                    SaveOffRailsStatus(okToSaveAngularVelocity);
                }
                lastUT = GetUT();
            }
            // Saving this FixedUpdate target, autopilot context and maneuver node, to check if they have changed in the next FixedUpdate
            SaveLastUpdateStatus();
        }

        // Vessel is entering physics simulation, either by being loaded or getting out of timewarp
        // Don't restore rotation/angular velocity here because the vessel/scene isn't fully loaded
        // Mark it to be done in a latter FixedUpdate, where we can check for FlightGlobals.ready
        public override void OnGoOffRails()
        {
            if (!IsEnabled)
                return;

            restoreAutopilotTarget = autopilotTargetHold;
            restoreAngularVelocity = !autopilotTargetHold;
        }

        private void ApplyAngularVelocity()
        {
            if (Vessel.situation == Vessel.Situations.PRELAUNCH || Vessel.situation == Vessel.Situations.LANDED || Vessel.situation == Vessel.Situations.SPLASHED)
            {
                return;
            }

            // Debug.Log("[US] Restoring " + Vessel.vesselName + "rotation after timewarp/load" );
            Vector3 COM = Vessel.CoM;
            Quaternion rotation = Vessel.ReferenceTransform.rotation;

            // Applying force on every part
            foreach (Part p in Vessel.parts)
            {
                if (!p.GetComponent<Rigidbody>()) continue;
                p.GetComponent<Rigidbody>().AddTorque(rotation * angularVelocity, ForceMode.VelocityChange);
                p.GetComponent<Rigidbody>().AddForce(Vector3.Cross(rotation * angularVelocity, (p.transform.position - COM)), ForceMode.VelocityChange);

                // Doing this through rigidbody is deprecated but I can't find a reliable way to use the 1.2 part.addforce/addtorque so they provide reliable results
                // see 1.2 patchnotes and unity docs for ForceMode.VelocityChange/ForceMode.Force
            }
        }

        private void RotateTowardTarget()
        {
            if (Vessel.situation == Vessel.Situations.PRELAUNCH || Vessel.situation == Vessel.Situations.LANDED || Vessel.situation == Vessel.Situations.SPLASHED)
            {
                return;
            }

            Vessel.SetRotation(Quaternion.FromToRotation(Vessel.GetTransform().up, targetDirection) * Vessel.transform.rotation, true); // SetPos=false seems to break the game on some occasions...
        }

        private void RotatePacked()
        {
            if (Vessel.situation == Vessel.Situations.PRELAUNCH || Vessel.situation == Vessel.Situations.LANDED || Vessel.situation == Vessel.Situations.SPLASHED)
            {
                return;
            }
            double timestep = lastUT < 0 ? TimeWarp.fixedDeltaTime : (GetUT() - lastUT);
            timestep *= 50d; // for some reason we need to divide out normal fixed delta time of 0.02s
            double rotAngle = (double)angularVelocity.magnitude * timestep;
            int subMult = (int)(rotAngle / 360d);
            if (subMult > 0)
            {
                rotAngle -= subMult * 360d;
            }

            Vessel.SetRotation(Quaternion.AngleAxis((float)rotAngle, Vessel.ReferenceTransform.rotation * angularVelocity) * Vessel.transform.rotation, true); // false seems to fix the "infinite roll bug"
        }

        private bool RestoreSASMode(int mode)
        {
            if (Vessel.Autopilot.Enabled)
            {
                return Vessel.Autopilot.SetMode((VesselAutopilot.AutopilotMode)mode);
            }
            else
            {
                return false;
            }
        }

        private void SaveOffRailsStatus(bool okToSaveAngularVelocity)
        {
            // Saving the current angular velocity, zeroing it if negligible
            // Only do this if the vessel is fully unpacked (and we've restored angular velocity)
            // otherwise we might zero it out by mistake.
            if (okToSaveAngularVelocity)
            {
                if (IsOverThreshold(Vessel.angularVelocity))
                {
                    angularVelocity = Vessel.angularVelocity;
                }
                else
                {
                    angularVelocity = Vector3.zero;
                }
            }

            // Checking if the autopilot hold mode should be enabled
            if (Vessel.Autopilot.Enabled
                && !(Vessel.Autopilot.Mode.Equals(VesselAutopilot.AutopilotMode.StabilityAssist))
                && !IsOverThreshold(angularVelocity) // The vessel isn't rotating too much
                && Vector3.Dot(Vessel.Autopilot.SAS.targetOrientation.normalized, Vessel.GetTransform().up.normalized) > 0.999f) // about 2.5 degrees
            {
                autopilotTargetHold = true;
            }
            else
            {
                autopilotTargetHold = false;
            }

            // Saving the current SAS mode
            autopilotMode = (int)Vessel.Autopilot.Mode;
        }

        private void SaveLastUpdateStatus()
        {
            // Saving the current target
            lastTarget = Vessel.targetObject;
            // Saving the current autopilot context
            autopilotContext = autopilotContextCurrent;
            // Saving the maneuver vector magnitude
            if (Vessel.patchedConicSolver != null)
            {
                if (Vessel.patchedConicSolver.maneuverNodes.Count > 0)
                {
                    lastManeuverParameters = Vessel.patchedConicSolver.maneuverNodes[0].DeltaV.magnitude + Vessel.patchedConicSolver.maneuverNodes[0].UT;
                }
            }
        }

        private bool TargetHoldValidity()
        {
            // Disable target hold if navball context is changed
            if (autopilotContextCurrent != autopilotContext)
            {
                return false;
            }

            // Disable target hold if target was modified
            if ((autopilotMode == 7 || autopilotMode == 8 || autopilotContext == 2) && Vessel.targetObject != lastTarget)
            {
                return false;
            }

            // Disable target hold if the maneuver node was modified or deleted
            if (autopilotMode == 9)
            {
                if (Vessel.patchedConicSolver.maneuverNodes.Count == 0)
                {
                    return false;
                }
                else if (Math.Abs(Vessel.patchedConicSolver.maneuverNodes[0].DeltaV.magnitude + Vessel.patchedConicSolver.maneuverNodes[0].UT) - Math.Abs(lastManeuverParameters) > 0.01f)
                {
                    return false;
                }
            }
            return true;
        }


        // Return the orientation vector of the saved SAS mode and context
        private Vector3 AutopilotTargetDirection()
        {
            Vector3 target = new Vector3();

            // Prograde/Retrograde
            if (autopilotMode == 1 || autopilotMode == 2)
            {
                if (autopilotContext == 0) // Orbit prograde
                    target = Vessel.obt_velocity;
                else if (autopilotContext == 1) // Surface prograde
                    target = Vessel.srf_velocity;
                else if (autopilotContext == 2) // Target prograde
                {
                    if (Vessel.targetObject != null)
                        target = -(Vessel.targetObject.GetObtVelocity() - Vessel.obt_velocity);
                    else
                        return Vessel.GetTransform().up;
                }

                if (autopilotMode == 2) // Invert vector for retrograde
                {
                    target = -target;
                }
            }

            // Normal/Radial
            else if (autopilotMode == 3 || autopilotMode == 4 || autopilotMode == 5 || autopilotMode == 6)
            {
                // Get body up vector
                Vector3 planetUp = (Vessel.rootPart.transform.position - Vessel.mainBody.position).normalized;

                // Get normal vector
                Vector3 normal = new Vector3();
                if (autopilotContext == 0) // Orbit
                    normal = Vector3.Cross(Vessel.obt_velocity, planetUp).normalized;
                else if (autopilotContext == 1 || autopilotContext == 2) // Surface/Target (seems to be the same for normal/radial)
                    normal = Vector3.Cross(Vessel.srf_velocity, planetUp).normalized;

                // Return normal/antinormal or calculate radial
                if (autopilotMode == 3) // Normal
                    target = normal;
                else if (autopilotMode == 4) // AntiNormal
                    target = -normal;
                else
                {
                    // Get RadialIn vector
                    Vector3 radial = new Vector3();
                    if (autopilotContext == 0) // Orbit
                        radial = Vector3.Cross(Vessel.obt_velocity, normal).normalized;
                    else if (autopilotContext == 1 || autopilotContext == 2) // Surface/Target (seems to be the same for normal/radial)
                        radial = Vector3.Cross(Vessel.srf_velocity, normal).normalized;

                    // Return radial vector
                    if (autopilotMode == 5) // Radial In
                        target = -radial;
                    else if (autopilotMode == 6) // Radial Out
                        target = radial;
                }
            }

            // Target/Antitarget
            else if (autopilotMode == 7 || autopilotMode == 8)
            {
                if (Vessel.targetObject != null)
                {
                    if (autopilotMode == 7) // Target
                        target = Vessel.targetObject.GetTransform().position - Vessel.transform.position;
                    else if (autopilotMode == 8) // AntiTarget
                        target = -(Vessel.targetObject.GetTransform().position - Vessel.transform.position);
                }
                else
                {
                    // No orientation keeping if target is null
                    return Vessel.GetTransform().up;
                }
            }

            // Maneuver
            else if (autopilotMode == 9)
            {
                if (Vessel.patchedConicSolver.maneuverNodes.Count > 0)
                {
                    target = Vessel.patchedConicSolver.maneuverNodes[0].GetBurnVector(Vessel.orbit);
                }
                else
                {
                    // No orientation keeping if there is no more maneuver node
                    return Vessel.GetTransform().up;
                }
            }

            //
            else
            {
                // Abort orientation keeping
                // autopilotTargetHold = false;
                return Vessel.GetTransform().up;
            }

            return target;
        }

        // Copypasted from PersistentRotation main.cs
        private Quaternion FromToRotation(Vector3d fromv, Vector3d tov) //Stock FromToRotation() doesn't work correctly
        {
            Vector3d cross = Vector3d.Cross(fromv, tov);
            double dot = Vector3d.Dot(fromv, tov);
            double wval = dot + Math.Sqrt(fromv.sqrMagnitude * tov.sqrMagnitude);
            double norm = 1.0 / Math.Sqrt(cross.sqrMagnitude + wval * wval);
            return new QuaternionD(cross.x * norm, cross.y * norm, cross.z * norm, wval * norm);
        }
    }
}
