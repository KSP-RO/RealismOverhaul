using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLinq;

namespace RealismOverhaul
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HSClobberer : MonoBehaviour
    {
        private static bool? _dreInstalled;
        private static HSClobbererSettings _settings = null;
        private readonly Dictionary<Part, ModuleAblator> _partAblatorDict = new Dictionary<Part, ModuleAblator>();
        private readonly HashSet<Part> _clobberedParts = new HashSet<Part>();
        private readonly List<Part> _tempRemoveList = new List<Part>();

        internal void Awake()
        {
            _dreInstalled ??= FetchDREInstalled();
            if (_dreInstalled.Value)
            {
                Destroy(this);
                return;
            }
            _settings ??= HSClobbererSettings.LoadHSClobbererSettings();

            // Start is too late to attach to events
            GameEvents.onVesselCreate.Add(OnVesselLoaded);    // Launching straight from editor fires this event...
            GameEvents.onVesselLoaded.Add(OnVesselLoaded);    // ... but not this one
            GameEvents.onVesselUnloaded.Add(OnVesselUnloaded);
            GameEvents.onPartDestroyed.Add(OnPartDestroyed);
            GameEvents.OnEVAConstructionModePartAttached.Add(OnConstructionPartAttached);
        }

        internal void OnDestroy()
        {
            GameEvents.onVesselCreate.Remove(OnVesselLoaded);
            GameEvents.onVesselLoaded.Remove(OnVesselLoaded);
            GameEvents.onVesselUnloaded.Remove(OnVesselUnloaded);
            GameEvents.onPartDestroyed.Remove(OnPartDestroyed);
            GameEvents.OnEVAConstructionModePartAttached.Remove(OnConstructionPartAttached);
        }

        internal void FixedUpdate()
        {
            foreach (ModuleAblator pm in _partAblatorDict.Values)
            {
                if (pm.ablativeAmount < _settings.ablatorThreshold && !_clobberedParts.Contains(pm.part))
                {
                    StartCoroutine(RunNerfRoutine(pm));
                }
            }
        }

        private void OnVesselLoaded(Vessel vessel)
        {
            StartCoroutine(ProcessVesselAtEndOfFrame(vessel));
        }

        private void OnVesselUnloaded(Vessel vessel)
        {
            foreach (Part part in _partAblatorDict.Keys)
            {
                if (ReferenceEquals(part.vessel, vessel))
                {
                    _tempRemoveList.Add(part);
                }
            }

            foreach (Part p in _tempRemoveList)
            {
                _partAblatorDict.Remove(p);
                _clobberedParts.Remove(p);
            }
            _tempRemoveList.Clear();
        }

        private void OnPartDestroyed(Part p)
        {
            _partAblatorDict.Remove(p);
            _clobberedParts.Remove(p);
        }

        private void OnConstructionPartAttached(Vessel vessel, Part part)
        {
            StartCoroutine(ProcessVesselAtEndOfFrame(vessel));
        }

        private IEnumerator ProcessVesselAtEndOfFrame(Vessel vessel)
        {
            // Wait for Start and the other events to run on vessel and PMs
            yield return new WaitForEndOfFrame();

            if (vessel == null) yield break;

            var pms = vessel.FindPartModulesImplementing<ModuleAblator>();
            foreach (ModuleAblator pm in pms)
            {
                if (!pm.useAblator ||
                    _settings.excludedParts.Contains(pm.part.partName) ||
                    _partAblatorDict.ContainsKey(pm.part))
                {
                    continue;
                }

                _partAblatorDict.Add(pm.part, pm);

                if (pm.ablativeAmount == 1)
                {
                    // Stock code sets the amount to 1 in Start() if it's actually 0.
                    // The FixedUpdate() is supposed to update that value afterwards but it only happens while above ablation threshold temp.
                    pm.part.GetConnectedResourceTotals(pm.ablativeID, pm.ablativeFlowMode, out double newAblativeAmount, out _);
                    pm.ablativeAmount = newAblativeAmount;
                }

                if (pm.ablativeAmount < _settings.ablatorThreshold)
                {
                    RunNerfRoutine(pm, immediate: true);
                }
            }
        }

        private IEnumerator RunNerfRoutine(PartModule pm, bool immediate = false)
        {
            _clobberedParts.Add(pm.part);

            float origSkinInternalConductionMult = (float)pm.part.skinInternalConductionMult;
            float origSkinSkinConductionMult = (float)pm.part.skinSkinConductionMult;

            if (!immediate)
            {
                float start = Time.fixedTime;
                float end = start + _settings.effectTime;

                yield return new WaitForFixedUpdate();

                while (Time.fixedTime < end)
                {
                    float t = (Time.fixedTime - start) / _settings.effectTime;
                    ClobberPartStats(pm, t, origSkinInternalConductionMult, origSkinSkinConductionMult);
                    yield return new WaitForFixedUpdate();
                }
            }

            ClobberPartStats(pm, 1, origSkinInternalConductionMult, origSkinSkinConductionMult);
        }

        private static void ClobberPartStats(PartModule pm, float t, float skinInternalConductionMult, float skinSkinConductionMult)
        {
            pm.part.skinInternalConductionMult = Mathf.Lerp(skinInternalConductionMult, skinInternalConductionMult * _settings.skinIntCondIncreaseMult, t);
            pm.part.skinSkinConductionMult = Mathf.Lerp(skinSkinConductionMult, skinSkinConductionMult * _settings.skinSkinCondIncreaseMult, t);
        }

        private bool FetchDREInstalled()
        {
            return AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "DeadlyReentry");
        }
    }
}
