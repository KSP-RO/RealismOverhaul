using System;
using UnityEngine;
using System.Collections.Generic;


// Modified version of the VesselModule of the same name in MandatoryRCS by gotmachine
// https://github.com/gotmachine/MandatoryRCS
namespace RealismOverhaul
{
    public class VesselModuleRotationRO : VesselModule
    {
        [KSPField(isPersistant = true)]
        public Vector3 angularVelocity;

        [KSPField(isPersistant = true)]
        public Quaternion vesselRot;

        private static int VERSION = 3;
        [KSPField(isPersistant = true)]
        public int loadedVersion = 0;

        // True if we are keeping the vessel oriented toward the SAS target
        [KSPField(isPersistant = true)]
        public bool autopilotTargetHold;

        // Enum VesselAutopilot.AutopilotMode
        [KSPField(isPersistant = true)]
        public int autopilotMode;

        // Enum FlightGlobals.SpeedDisplayModes
        // 0=orbit, 1=surface, 2=target
        [KSPField(isPersistant = true)]
        public int autopilotContext;

        [KSPField(isPersistant = true)]
        public Vector3 maneuverDir;

        [KSPField(isPersistant = true)]
        public double lastUT = 0d;

        [KSPField(isPersistant = true)]
        public Vector3 referenceUpLocal;
        [KSPField(isPersistant = true)]
        public Vector3 referencePosLocal;


        // variables used to check if things have changed since last fixedUpdate
        // or when loading / switching vessels
        public double _lastManeuverParameters;
        private ITargetable _lastTarget = null;

        // Restore the angular velocity when going off rails
        private bool _restoreAngularVelocity = false;
        // Restore SAS when going off rails
        private bool _restoreSAS = false;
        // Restore vessel target in FixedUpdate when VM or vessel loads (grabs from protovessel if not loaded)
        private bool _restoreTarget = false;
        // Restore maneuver node hash in FixedUpdate when VM or vessel loads
        private bool _restoreManeuverHash = false;

        // Var used to retry setting the SAS selection when loading / switching vessels
        private bool _retrySAS = false;
        private int _retrySASCount;
        private int _setSASMode;

        private bool _isInitialized = false;

        

        private const float RotationThreshold = 0.1f * Mathf.Deg2Rad;
        private const float RotationThresholdSAS = 0.5f * Mathf.Deg2Rad;

        private static Vector3d ToPlanetarium(Vector3d vec)
        {
            return Planetarium.Zup.LocalToWorld(vec.xzy);
        }

        private static Vector3 ToUnity(Vector3d vec)
        {
            return Planetarium.Zup.WorldToLocal(vec).xzy;
        }

        private static bool _isEnabled = true;
        private static bool _shouldCheckEnabled = true;

        private static bool CheckEnabled()
        {
            _shouldCheckEnabled = false;
            foreach (var a in AssemblyLoader.loadedAssemblies)
            {
                // ksp_plugin_adapter is Principia
                if (a.name == "PersistentRotation" || a.name == "PersistentRotationUpgraded" || a.name == "ksp_plugin_adapter" || a.name == "MandatoryRCS")
                {
                    _isEnabled = false;
                    break;
                }
            }
            return _isEnabled;
        }

        public static bool IsEnabled => _shouldCheckEnabled ? CheckEnabled() : _isEnabled;

        private bool IsOverThreshold(Vector3 rot)
        {
            float thresh = vessel.Autopilot.Enabled ? RotationThresholdSAS : RotationThreshold;
            return Mathf.Abs(rot.x) > thresh || Mathf.Abs(rot.y) > thresh || Mathf.Abs(rot.z) > thresh;
        }

        public void StoreRotAndTime()
        {
            StoreRotAndTime(vessel.transform.rotation);
        }

        public void StoreRotAndTime(QuaternionD rot)
        {
            vesselRot = Planetarium.Zup.Rotation * rot.swizzle;
            lastUT = Planetarium.GetUniversalTime();
        }

        private Quaternion UnityRot()
        {
            return (QuaternionD.Inverse(Planetarium.Zup.Rotation) * (QuaternionD)vesselRot).swizzle;
        }

        private void SetRot()
        {
            if (IgnoreRot(vessel))
                return;

            SetPosRot(UnityRot(), vessel.transform.position);
        }

        public void SetPosRot(Quaternion rotation, Vector3 position)
        {
            if (!vessel.loaded)
            {
                vessel.vesselTransform.SetPositionAndRotation(position, rotation);
                return;
            }

            List<Part> parts = vessel.parts;
            for (int i = parts.Count; i-- > 0;)
            {
                Part part = parts[i];
                part.transform.SetPositionAndRotation(position + rotation * part.orgPos, rotation * part.orgRot);
            }
        }

        public static bool IgnoreRot(Vessel vessel) => vessel.situation == Vessel.Situations.PRELAUNCH || vessel.situation == Vessel.Situations.LANDED || vessel.situation == Vessel.Situations.SPLASHED;

        private void Initialize()
        {
            _isInitialized = true;
            StoreRotAndTime();
            referenceUpLocal = vessel.transform.InverseTransformDirection(vessel.GetTransform().up);
            loadedVersion = VERSION;
            _restoreSAS = false;
            _restoreTarget = false;
            _restoreManeuverHash = false;
            _restoreAngularVelocity = false;
        }

        protected override void OnLoad(ConfigNode node)
        {
            _isInitialized = true;
            _restoreTarget = true;
            _restoreManeuverHash = true;

            if (loadedVersion < VERSION)
            {
                if (loadedVersion < 2)
                {
                    angularVelocity = Vector3d.zero;
                    StoreRotAndTime();
                }
                if (loadedVersion < 3)
                {
                    referenceUpLocal = vessel.transform.InverseTransformDirection(vessel.GetTransform().up);
                }
                loadedVersion = VERSION;
            }
            if (!IsEnabled)
                return;

            SetRot();
        }

        public void RestoreState()
        {
            if (_restoreManeuverHash)
                StoreManeuverHash();

            if (vessel.loaded)
            {
                if (_restoreTarget)
                {
                    _restoreTarget = false;
                    _lastTarget = vessel.targetObject;
                }

                if (!vessel.packed && FlightGlobals.ready)
                {
                    // Restoring previous SAS selection
                    if (_restoreSAS)
                    {
                        _restoreSAS = false;
                        if (!RestoreSASMode(autopilotMode))
                        {
                            _retrySAS = true;
                            _setSASMode = autopilotMode;
                            _retrySASCount = 10;
                        }
                    }
                    // When a vessel loads, VesselAutopilot gets enabled in Update based on SAS actiongroup being on
                    // (the actiongroup is what's persisted). So we're going to run (in FixedUpdate) before that happens.
                    else if (_retrySAS)
                    {
                        if (_retrySASCount > 0)
                        {
                            if (RestoreSASMode(_setSASMode))
                            {
                                _retrySAS = false;
                                // Debug.Log("[US] autopilot mode " + setSASMode + " set at count " + retrySASCount);
                            }
                            --_retrySASCount;
                        }
                        else
                        {
                            _retrySAS = false;
                            // Debug.Log("[US] can't set autopilot mode.");
                        }
                    }
                }
            }
            else
            {
                if (_restoreTarget)
                {
                    _restoreTarget = false;
                    _lastTarget = vessel.protoVessel.targetInfo?.FindTarget();
                }
            }
        }

        private void FixedUpdate()
        {
            if (!IsEnabled)
                return;

            if (!_isInitialized)
                Initialize();

            RestoreState();

            if (vessel.loaded)
            {
                // packed:
                // Vessel is loaded but not in physics, either because
                // - It is in the physics bubble but in non-physics timewarp
                // - It has gone outside of the physics bubble
                // - It was just loaded, is in the physics bubble and will be unpacked in a few frames

                // unpacked:
                if (!vessel.packed && FlightGlobals.ready) // The vessel is in physics simulation and fully loaded
                {
                    bool okToSaveAngularVelocity = true;

                    if (_restoreAngularVelocity) // Restoring saved rotation if it was above the threshold
                    {
                        okToSaveAngularVelocity = false;
                        if (vessel.rootPart.rb != null && !vessel.rootPart.rb.isKinematic)
                        {
                            // Debug.Log("[US] " + vessel.vesselName + " going OFF rails : restoring angular velocity, angvel=" + angularVelocity.magnitude);
                            ApplyAngularVelocity();
                            _restoreAngularVelocity = false;
                        }
                    }

                    // Saving angular velocity (if we can), SAS mode, and check target hold status
                    SaveOffRailsStatus(okToSaveAngularVelocity);
                }
            }

            // Saving this FixedUpdate target, autopilot context and maneuver node, to check if they have changed in the next FixedUpdate
            SaveLastUpdateStatus();
        }

        public void RailsUpdate(Vector3d pos)
        {
            if (!_isInitialized)
                Initialize();

            // Check if target / maneuver is modified/deleted during timewarp
            bool holdValid = false;
            if (autopilotTargetHold)
            {
                holdValid = TargetHoldValidity();
                if (TimeWarp.WarpMode == TimeWarp.Modes.HIGH && TimeWarp.CurrentRateIndex > 0)
                    autopilotTargetHold = holdValid;
            }

            // if we don't have over-thresh angular velocity and we do
            // have a target, orient to the target
            if (angularVelocity == Vector3.zero && autopilotTargetHold && holdValid)
            {
                RotateTowardTarget(pos);
                StoreRotAndTime();
            }
            else
            {
                RotatePacked(pos);
                // We don't store rot here, it relies on lastUT
                // and original orientation
            }
        }

        // Vessel is entering physics simulation, either by being loaded or getting out of timewarp
        // Don't restore rotation/angular velocity here because the vessel/scene isn't fully loaded
        // Mark it to be done in a latter FixedUpdate, where we can check for FlightGlobals.ready
        public override void OnGoOffRails()
        {
            if (!IsEnabled)
                return;

            _restoreSAS = autopilotTargetHold;
            _restoreAngularVelocity = angularVelocity != Vector3.zero;
        }

        public override void OnLoadVessel()
        {
            if (!IsEnabled)
                return;

            _restoreTarget = true; // this fires before the vessel itself loads target, so we have to wait for next fixedupdate
        }

        public override void OnUnloadVessel()
        {
            if (!IsEnabled)
                return;

            StoreRotAndTime();
        }

        private void ApplyAngularVelocity()
        {
            if (IgnoreRot(vessel))
            {
                return;
            }

            // Debug.Log("[US] Restoring " + vessel.vesselName + "rotation after timewarp/load" );
            Vector3 COM = vessel.CoM;
            Vector3 angVel = ToUnity(angularVelocity);

            // Applying force on every part
            foreach (Part p in vessel.parts)
            {
                if (p.rb == null || p.rb.isKinematic) continue;
                p.rb.AddTorque(angVel, ForceMode.VelocityChange);
                p.rb.AddForce(Vector3.Cross(angVel, (p.transform.position - COM)), ForceMode.VelocityChange);

                // Doing this through rigidbody is deprecated but I can't find a reliable way to use the 1.2 part.addforce/addtorque so they provide reliable results
                // see 1.2 patchnotes and unity docs for ForceMode.VelocityChange/ForceMode.Force
            }
        }

        private void RotateTowardTarget(Vector3d pos)
        {
            if (IgnoreRot(vessel))
            {
                return;
            }

            Vector3 tgt = AutopilotTargetDirection();
            Vector3 up = vessel.transform.TransformDirection(referenceUpLocal);
            float dot = Vector3.Dot(tgt, up);
            if (!vessel.loaded || dot < 0.99999f)
                SetPosRot(FromToRotation(up, tgt) * vessel.transform.rotation, pos);
        }

        private void RotatePacked(Vector3d pos)
        {
            if (IgnoreRot(vessel))
            {
                return;
            }

            Quaternion unityRot = UnityRot();
            // If we don't have angular velocity, just update our rotation
            if (angularVelocity == Vector3.zero)
            {
                SetPosRot(unityRot, pos);
                return;
            }

            // Otherwise calculate our current rotation based off angular velocity and time
            double elapsed = Planetarium.GetUniversalTime() - lastUT;
            double rotAngle = (double)angularVelocity.magnitude * elapsed * UtilMath.Rad2Deg;
            double fullRotations = Math.Floor(rotAngle * (1d / 360d));
            rotAngle -= fullRotations * 360d;

            SetPosRot(Quaternion.AngleAxis((float)rotAngle, ToUnity(angularVelocity)) * UnityRot(), pos);
        }

        private bool RestoreSASMode(int mode)
        {
            if (vessel.Autopilot.Enabled)
            {
                return vessel.Autopilot.SetMode((VesselAutopilot.AutopilotMode)mode);
            }
            else
            {
                return false;
            }
        }

        private enum AngularVelocityThresholdCheck
        {
            Below,
            Above,
            Unknown
        }

        private void SaveOffRailsStatus(bool okToSaveAngularVelocity)
        {
            AngularVelocityThresholdCheck threshCheck = AngularVelocityThresholdCheck.Unknown;
            // Saving the current angular velocity, zeroing it if negligible
            // Only do this if the vessel is fully unpacked (and we've restored angular velocity)
            // otherwise we might zero it out by mistake.
            if (okToSaveAngularVelocity)
            {
                if (IsOverThreshold(vessel.angularVelocity))
                {
                    threshCheck = AngularVelocityThresholdCheck.Above;
                    angularVelocity = ToPlanetarium(vessel.ReferenceTransform.rotation * vessel.angularVelocity);
                }
                else
                {
                    threshCheck = AngularVelocityThresholdCheck.Below;
                    angularVelocity = Vector3d.zero;
                }
            }
            Vector3 vecUp = vessel.GetTransform().up;
            referenceUpLocal = vessel.transform.InverseTransformDirection(vecUp);
            referencePosLocal = vessel.transform.InverseTransformPoint(vessel.GetTransform().position);

            if (_retrySAS)
                return;

            maneuverDir.Zero();

            // Checking if the autopilot hold mode should be enabled
            if (vessel.Autopilot.Enabled
                && !(vessel.Autopilot.Mode.Equals(VesselAutopilot.AutopilotMode.StabilityAssist))
                && (threshCheck != AngularVelocityThresholdCheck.Above && (threshCheck == AngularVelocityThresholdCheck.Below || !IsOverThreshold(vessel.angularVelocity))) // The vessel isn't rotating too much
                && vessel.Autopilot.SAS.targetOrientation.normalized is Vector3 dir && Vector3.Dot(dir, vecUp) > 0.999f) // about 2.5 degrees
            {
                autopilotTargetHold = true;
                if (vessel.patchedConicSolver != null && vessel.patchedConicSolver.maneuverNodes.Count > 0)
                    maneuverDir = ToPlanetarium(vessel.patchedConicSolver.maneuverNodes[0].GetBurnVector(vessel.orbit).normalized);
                else if (vessel.Autopilot.Mode == VesselAutopilot.AutopilotMode.Maneuver)
                    autopilotTargetHold = false;
            }
            else
            {
                autopilotTargetHold = false;
            }

            // Saving the current SAS mode
            autopilotMode = (int)vessel.Autopilot.Mode;

            StoreRotAndTime();
        }

        private void StoreManeuverHash()
        {
            if (vessel.patchedConicSolver != null)
            {
                _restoreManeuverHash = false;

                if (vessel.patchedConicSolver.maneuverNodes.Count > 0)
                {
                    _lastManeuverParameters = vessel.patchedConicSolver.maneuverNodes[0].DeltaV.magnitude + vessel.patchedConicSolver.maneuverNodes[0].UT;
                }
            }
        }

        private void SaveLastUpdateStatus()
        {
            if (vessel.loaded && vessel == FlightGlobals.ActiveVessel)
            {
                // Saving the current target
                _lastTarget = vessel.targetObject;
                // Saving the current autopilot context
                autopilotContext = (int)FlightGlobals.speedMode;
            }

            // Saving the maneuver vector magnitude
            StoreManeuverHash();
        }

        private bool TargetHoldValidity()
        {
            // Disable target hold if target was modified
            if ((autopilotMode == 7 || autopilotMode == 8 || autopilotContext == 2) && (vessel.loaded ? (vessel.targetObject != _lastTarget) : _lastTarget == null))
            {
                return false;
            }

            // Disable target hold if the maneuver node was modified or deleted
            if (autopilotMode == 9)
            {
                if (vessel.patchedConicSolver == null)
                    return maneuverDir != Vector3.zero;

                if (vessel.patchedConicSolver.maneuverNodes.Count == 0)
                {
                    return false;
                }
                else if (Math.Abs(vessel.patchedConicSolver.maneuverNodes[0].DeltaV.magnitude + vessel.patchedConicSolver.maneuverNodes[0].UT) - Math.Abs(_lastManeuverParameters) > 0.01f)
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
                    target = vessel.obt_velocity;
                else if (autopilotContext == 1) // Surface prograde
                    target = vessel.srf_velocity;
                else if (autopilotContext == 2) // Target prograde
                {
                    if (_lastTarget != null)
                        target = -(_lastTarget.GetObtVelocity() - vessel.obt_velocity);
                    else
                        return vessel.transform.TransformDirection(referenceUpLocal);
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
                Vector3 planetUp = (vessel.rootPart.transform.position - vessel.mainBody.position).normalized;

                // Get normal vector
                Vector3 normal = new Vector3();
                if (autopilotContext == 0) // Orbit
                    normal = Vector3.Cross(vessel.obt_velocity, planetUp).normalized;
                else if (autopilotContext == 1 || autopilotContext == 2) // Surface/Target (seems to be the same for normal/radial)
                    normal = Vector3.Cross(vessel.srf_velocity, planetUp).normalized;

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
                        radial = Vector3.Cross(vessel.obt_velocity, normal).normalized;
                    else if (autopilotContext == 1 || autopilotContext == 2) // Surface/Target (seems to be the same for normal/radial)
                        radial = Vector3.Cross(vessel.srf_velocity, normal).normalized;

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
                if (_lastTarget != null)
                {
                    if (autopilotMode == 7) // Target
                        target = _lastTarget.GetTransform().position - vessel.transform.TransformPoint(referencePosLocal);
                    else if (autopilotMode == 8) // AntiTarget
                        target = -(_lastTarget.GetTransform().position - vessel.transform.TransformPoint(referencePosLocal));
                }
                else
                {
                    // No orientation keeping if target is null
                    return vessel.transform.TransformDirection(referenceUpLocal);
                }
            }

            // Maneuver
            else if (autopilotMode == 9)
            {
                if (!vessel.loaded || vessel.patchedConicSolver == null)
                    return ToUnity(maneuverDir);

                if (vessel.patchedConicSolver.maneuverNodes.Count > 0)
                {
                    target = vessel.patchedConicSolver.maneuverNodes[0].GetBurnVector(vessel.orbit);
                }
                else
                {
                    // No orientation keeping if there is no more maneuver node
                    return vessel.transform.TransformDirection(referenceUpLocal);
                }
            }

            //
            else
            {
                // Abort orientation keeping
                // autopilotTargetHold = false;
                return vessel.transform.TransformDirection(referenceUpLocal);
            }

            return target.normalized;
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

        public static VesselModuleRotationRO GetModule(Vessel v)
        {
            int c = v.vesselModules.Count;
            for (int i = c; i-- > 0;)
            {
                var vmr = v.vesselModules[i] as VesselModuleRotationRO;
                // We're testing the cast, so we don't need to do the Unity memory management check
                // (other places will fully check for null)
                if ((object)vmr != null)
                {
                    if (i != c - 1)
                    {
                        // put it at the back so the next time we find it first.
                        v.vesselModules.RemoveAt(i);
                        v.vesselModules.Add(vmr);
                    }
                    return vmr;
                }
            }

            return null;
        }
    }
}