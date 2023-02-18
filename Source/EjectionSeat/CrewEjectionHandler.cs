using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RealismOverhaul
{
    public class CrewEjectionHandler : MonoBehaviour
    {
        private const double HorzSpeedToEnsure = 3;
        private const double MinSrfAltitudeForHorzSpeedCheck = 10;

        public float EjectionForce;
        public float EjectionForceDuration;
        public float ChuteMinPressureOverride;
        public float ChuteDeployAltitudeOverride;
        public float ChuteDelay;
        public Vector3 ForceDirection;

        private ModuleEvaChute _evaChute;

        public KerbalEVA KerbalEVA { get; private set; }

        private static readonly List<CrewEjectionHandler> _instances = new List<CrewEjectionHandler>();

        public static CrewEjectionHandler CreateInstance(KerbalEVA eva)
        {
            var go = new GameObject("ROCrewEjectionHandler");
            var handler = go.AddComponent<CrewEjectionHandler>();
            handler.KerbalEVA = eva;
            _instances.Add(handler);
            return handler;
        }

        public static bool TryGetInstanceForKerbalEVA(KerbalEVA eva, out CrewEjectionHandler handler)
        {
            handler = _instances.Find(i => i.KerbalEVA == eva);
            return !ReferenceEquals(handler, null);
        }

        public void StartProcessing()
        {
            StartCoroutine(AddForcesRoutine());
            StartCoroutine(DelayedDeployChuteRoutine());
        }

        /// <summary>
        /// If horizontal speed drops too low then the kerbal will fall out of air like a brick.
        /// </summary>
        /// <returns>Whether the horizontal speed should be increased to avoid stalling</returns>
        public bool ShouldIncreaseHorizontalSpeed()
        {
            return KerbalEVA != null && _evaChute != null &&
                   _evaChute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED &&
                   KerbalEVA.vessel.horizontalSrfSpeed < HorzSpeedToEnsure &&
                   KerbalEVA.vessel.radarAltitude > MinSrfAltitudeForHorzSpeedCheck;
        }

        internal void OnDestroy()
        {
            _instances.Remove(this);
        }

        private IEnumerator AddForcesRoutine()
        {
            while (!KerbalEVA.Ready)
                yield return new WaitForFixedUpdate();

            bool firstFrame = true;
            float remainingTime = EjectionForceDuration;
            while (remainingTime > 0)
            {
                float scale = remainingTime < Time.fixedDeltaTime ? remainingTime / Time.fixedDeltaTime : 1;
                remainingTime -= Time.fixedDeltaTime;
                float scaledForce = scale * EjectionForce;
                KerbalEVA.part.AddForce(ForceDirection * scaledForce);

                if (firstFrame)
                {
                    SetIgnoreGForcesFrames(0);
                    firstFrame = false;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator DelayedDeployChuteRoutine()
        {
            yield return new WaitForSeconds(ChuteDelay);

            _evaChute = KerbalEVA.GetComponent<ModuleEvaChute>();
            if (_evaChute != null && _evaChute.enabled && _evaChute.deploymentState == ModuleParachute.deploymentStates.STOWED)
            {
                // Will only pre-deploy when air pressure is above the threshold and fully deploy below a preset altitude
                _evaChute.minAirPressureToOpen = ChuteMinPressureOverride;
                _evaChute.deployAltitude = ChuteDeployAltitudeOverride;
                _evaChute.Deploy();
            }
        }

        /// <summary>
        /// Gotta compress those spines! KSP will by default ignore 10 frames which means that the G-meter won't budge at all.
        /// This is probably necessary as a workaround to the kerbals getting excessive amounts of forces applied to them on spawn.
        /// 10 frames is too much though so we try to override it to a smaller value.
        /// </summary>
        /// <param name="frames">Value to set ignoreGeeforceFrames field to</param>
        private void SetIgnoreGForcesFrames(int frames)
        {
            var fInf = typeof(Vessel).GetField("ignoreGeeforceFrames", BindingFlags.Instance | BindingFlags.NonPublic);
            fInf.SetValue(KerbalEVA.vessel, frames);
        }
    }
}
