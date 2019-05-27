using CommNet;
using RealismOverhaul.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace RealismOverhaul.Communications
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames | ScenarioCreationOptions.AddToAllMissionGames, new GameScenes[] { GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPACECENTER, GameScenes.EDITOR })]
    class KerbalismIntegration : ScenarioModule
    {
        private static Cache cache = new Cache();
        private static Assembly _kerbalismAssembly;

        public void Start()
        {
            StartKerbalismIntegration();
        }

        private class Cache {
            private Storage<Guid, ICollection<CachedAntenna>> _antennaCache = new Storage<Guid, ICollection<CachedAntenna>>();
            public Storage<CommNode, float> _dataRateCache = new Storage<CommNode, float>();

            public StorageView<Vessel, Guid, ICollection<CachedAntenna>> AntennaCache { get; }

            public Cache()
            {
                AntennaCache = StorageView.Create(_antennaCache, (Vessel x) => x.id);
            }

            public void Clear()
            {
                _antennaCache.Clear();
                _dataRateCache.Clear();
            }

            public void Remove(Vessel vessel)
            {
                _antennaCache.Remove(vessel.id);
                if (vessel.connection.Comm != null)
                {
                    _dataRateCache.Remove(vessel.connection.Comm);
                }
            }

            public void Remove(ProtoVessel protoVessel)
            {
                _antennaCache.Remove(protoVessel.vesselID);
            }
        }

        private struct CachedAntenna
        {
            public ProtoPartSnapshot Snapshot { get; }
            public ModuleConfigurableAntenna Antenna { get; }

            public CachedAntenna(ModuleConfigurableAntenna antenna, ProtoPartSnapshot snapshot)
            {
                Snapshot = snapshot;
                Antenna = antenna;
            }
        }

        private class AntennaInfo
        {
            public double Strength { get; set; }
            public double Rate { get; set; }
            public int Status { get; set; }
            public bool Linked { get; set; }
            public double PowerUsed { get; set; }
            public string Target { get; set; }
            public IList<string[]> ControlPath { get; set; }

            public AntennaInfo(int status) => Status = status;
        }

        public static void StartKerbalismIntegration()
        {
            _kerbalismAssembly = AssemblyLoader.loadedAssemblies.FirstOrDefault(x => x.name.StartsWith("Kerbalism") && x.name[9] != 'B')?.assembly;
            if (_kerbalismAssembly?.GetType("KERBALISM.API") is Type api)
            {
                AddCallbacks();
                AddCommsHandler(api);
            }
        }

        private static void AddCallbacks()
        {
            GameEvents.onVesselChange.Add((v) => { cache.Remove(v); });
            GameEvents.onVesselStandardModification.Add(v => cache.Remove(v));
            GameEvents.onGameSceneSwitchRequested.Add(_ => cache.Clear());
            GameEvents.onVesselRecovered.Add((v, _) => cache.Remove(v));
            GameEvents.onVesselTerminated.Add(v => cache.Remove(v));
            GameEvents.onVesselWillDestroy.Add(v => cache.Remove(v));
            GameEvents.onPartCouple.Add(x => cache.Clear());
            GameEvents.onPartDie.Add(x => cache.Remove(x.vessel));
        }

        private static void AddCommsHandler(Type api)
        {
            var comm = api.GetField("Comm", BindingFlags.Static | BindingFlags.Public).GetValue(null);
            var addMethod = comm.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            var handlerMethod = typeof(KerbalismIntegration).GetMethod(nameof(HandleCommQuery), BindingFlags.Static | BindingFlags.Public);
            addMethod.Invoke(comm, new object[] { handlerMethod });
        }

        public static void HandleCommQuery(object kerbalismAntennaInfo, Vessel vessel)
        {
            var vesselSpecs = GetAntennaSpecs(vessel);
            var powered = kerbalismAntennaInfo.GetField<bool>("powered");
            var antennaInfo = GetAntennaInfo(vessel, vesselSpecs, powered);
            SetKerbalismFields(kerbalismAntennaInfo, vessel, antennaInfo);
        }

        private static void SetKerbalismFields(object kerbalismAntennaInfo, Vessel vessel, AntennaInfo antennaInfo)
        {
            kerbalismAntennaInfo.SetField("target_name", vessel.connection.ControlPath.First.end.displayName.Replace("Kerbin", "DSN"));
            kerbalismAntennaInfo.SetField("strength", antennaInfo.Strength);
            kerbalismAntennaInfo.SetField("rate", antennaInfo.Rate / 1024 / 1024 / 8);
            kerbalismAntennaInfo.SetField("status", antennaInfo.Status);
            kerbalismAntennaInfo.SetField("linked", antennaInfo.Linked);
            kerbalismAntennaInfo.SetField("ec", antennaInfo.PowerUsed / 1000);
            kerbalismAntennaInfo.SetField("control_path", antennaInfo.ControlPath);
        }

        private static AntennaInfo GetAntennaInfo(Vessel v, AntennaSpecs vesselSpecs, bool powered)
        {
            if (!powered || v.connection == null)
            {
                return new AntennaInfo(2);
            }

            return GetConnectedAntennaInfo(v, vesselSpecs);
        }

        private static AntennaInfo GetConnectedAntennaInfo(Vessel v, AntennaSpecs vesselSpecs)
        {
            var antennaInfo = new AntennaInfo(2);

            if (!v.loaded)
            {
                v.connection.SetField("unloadedDoOnce", true);
            }
            if (v.connection.IsConnected)
            {
                antennaInfo.Linked = true;
                var link = v.connection.ControlPath.First;
                antennaInfo.Status = link.hopType == HopType.Home ? 0 : 1;
                antennaInfo.Strength = link.signalStrength;
                antennaInfo.Rate = GetRate(v, vesselSpecs, link);
                antennaInfo.Target = v.connection.ControlPath.First.end.displayName.Replace("Kerbin", "DSN");
            }
            else if (v.connection.InPlasma)
            {
                antennaInfo.Status = 3;
            }

            antennaInfo.ControlPath = GetControlPath(v, antennaInfo);

            return antennaInfo;
        }

        private static float GetRate(Vessel vessel, AntennaSpecs vesselSpecs, CommLink link)
        {
            var rate = GetFirstHopRate(vesselSpecs, link);
            if (link.hopType != HopType.Home)
            {
                var prevHopRate = cache._dataRateCache.Get(vessel.Connection.ControlPath.First.end);
                rate = Mathf.Min(prevHopRate, rate);
            }
            cache._dataRateCache.Add(vessel.connection.Comm, rate);

            return rate;
        }

        private static float GetFirstHopRate(AntennaSpecs vesselSpecs, CommLink link)
        {
            var preciseRate = vesselSpecs.MinDataRate * Mathf.Pow(1 - (float) link.signalStrength, -2);
            var powerOf2Rate = Mathf.Floor(preciseRate.ToLog2()).FromLog2();
            return Mathf.Min(vesselSpecs.MaxDataRate, powerOf2Rate);
        }

        private static IList<string[]> GetControlPath(Vessel v, AntennaInfo antennaInfo)
        {
            var path = new List<string[]>();
            foreach (var link in v.connection.ControlPath)
            {
                var controlPathItem = GetControlPathItem(link);
                path.Add(controlPathItem);
            }
            return path;
        }

        private static string[] GetControlPathItem(CommLink link)
        {
            double antennaPower = link.end.isHome ? link.start.antennaTransmit.power + link.start.antennaRelay.power : link.start.antennaTransmit.power;
            double signalStrength = 1 - ((link.start.position - link.end.position).magnitude / Math.Sqrt(antennaPower * link.end.antennaRelay.power));
            signalStrength = (3 - (2 * signalStrength)) * Math.Pow(signalStrength, 2);

            string name = link.end.name.Replace("Kerbin", "DSN");
            string value = $"{(Math.Ceiling(signalStrength * 10000) / 10000):0.00}";
            string tooltip = "Distance: " + (link.start.position - link.end.position).magnitude.Format("m") +
                "\nMy Max Distance: " + Math.Sqrt((link.start.antennaTransmit.power + link.start.antennaRelay.power) * link.end.antennaRelay.power).Format("m");
            var controlPathItem = new string[] { name, value, tooltip };
            return controlPathItem;
        }

        private static AntennaSpecs GetAntennaSpecs(Vessel v)
        {
            var cachedAntennas = cache.AntennaCache.GetOrAdd(v, GetAntennas);
            var vesselSpec = new AntennaSpecs(0, 0, 0, float.MaxValue, float.MaxValue);
            foreach (var cachedAntenna in cachedAntennas)
            {
                DisableAntennaUI(cachedAntenna);
                var antennaSpecs = GetAntennaSpecs(cachedAntenna);
                var deployState = GetDeployState(cachedAntenna);
                if (deployState == null || deployState == ModuleDeployablePart.DeployState.EXTENDED)
                {
                    vesselSpec.MinDataRate = Mathf.Min(vesselSpec.MinDataRate, antennaSpecs.MinDataRate);
                    vesselSpec.MaxDataRate = Mathf.Min(vesselSpec.MaxDataRate, antennaSpecs.MaxDataRate);
                    vesselSpec.TotalPower += antennaSpecs.TotalPower;
                }
            }

            return vesselSpec;
        }

        private static ModuleDeployablePart.DeployState? GetDeployState(CachedAntenna antennaInfo)
        {
            return antennaInfo.Snapshot == null ? GetLoadedDeployState(antennaInfo) : GetUnloadedDeployState(antennaInfo);
        }

        private static ModuleDeployablePart.DeployState? GetLoadedDeployState(CachedAntenna antennaInfo) => antennaInfo.Antenna.part.FindModuleImplementing<ModuleDeployableAntenna>()?.deployState;

        private static ModuleDeployablePart.DeployState? GetUnloadedDeployState(CachedAntenna antennaInfo)
        {
            var moduleSnapShot = antennaInfo.Snapshot.FindModule("ModuleDeployableAntenna");
            return moduleSnapShot?.GetString("deployState").ToEnum<ModuleDeployablePart.DeployState>();
        }

        private static AntennaSpecs GetAntennaSpecs(CachedAntenna antennaInfo)
        {
            return antennaInfo.Snapshot == null ? antennaInfo.Antenna.GetAntennaSpecs() : antennaInfo.Antenna.GetAntennaSpecs(antennaInfo.Snapshot.FindModule(nameof(ModuleConfigurableAntenna)));
        }

        private static void DisableAntennaUI(CachedAntenna cachedAntenna)
        {
            if(cachedAntenna.Snapshot != null)
            {
                return;
            }
            var antenna = cachedAntenna.Antenna;
            antenna.Events["TransmitIncompleteToggle"].active = false;
            antenna.Events["StartTransmission"].active = false;
            antenna.Events["StopTransmission"].active = false;
            antenna.Actions["StartTransmissionAction"].active = false;
        }

        private static ICollection<CachedAntenna> GetAntennas(Vessel v)
        {
            return v.loaded ? GetLoadedAntennas(v) : GetUnloadedAntennas(v);
        }

        private static ICollection<CachedAntenna> GetUnloadedAntennas(Vessel v)
        {
            var antennaInfos = new List<CachedAntenna>();
            foreach (var partSnapshot in v.protoVessel.protoPartSnapshots)
            {
                var partPrefab = PartLoader.getPartInfoByName(partSnapshot.partName).partPrefab;

                foreach (var antenna in partPrefab.FindModulesImplementing<ModuleConfigurableAntenna>())
                {
                    antennaInfos.Add(new CachedAntenna(antenna, partSnapshot));
                }
            }

            return antennaInfos;
        }

        private static ICollection<CachedAntenna> GetLoadedAntennas(Vessel v)
        {
            var antennaInfos = new List<CachedAntenna>();
            foreach (var antenna in v.FindPartModulesImplementing<ModuleConfigurableAntenna>())
            {
                antennaInfos.Add(new CachedAntenna(antenna, null));
            }

            return antennaInfos;
        }
    }
}
