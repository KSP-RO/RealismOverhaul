using System;
using System.Collections.Generic;
using SaveUpgradePipeline;
using UnityEngine;

namespace RealismOverhaul
{
    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/FLIGHTSTATE/VESSEL/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade : UpgradeScript
    {
        public override string Name { get => "RO MEC Config Upgrader"; }
        public override string Description { get => "Updates Engines configs from outdated to new"; }
        public override Version EarliestCompatibleVersion { get => new Version(1, 10, 0); }
        public override Version TargetVersion { get => new Version(1, 12, 3); }
        //protected override bool CheckMaxVersion(Version v) => true; // Upgrades are ProcParts-dependent, not KSP version.
        protected override TestResult VersionTest(Version v) => TestResult.Upgradeable;

        const string Separator = "$";

        protected static Dictionary<string, string> upgradeList = new Dictionary<string, string>();

        protected static bool hasLoadedData = false;

        protected static void LoadData()
        {
            if (hasLoadedData)
                return;

            foreach (var node in GameDatabase.Instance.GetConfigNodes("ROMECCONFIGUPGRADES"))
            {
                foreach (ConfigNode.Value kvp in node.values)
                    upgradeList[kvp.name] = kvp.value;
            }
        }

        protected string GetConfigString(ConfigNode mecNode)
        {
            string configName = mecNode.GetValue("configuration");

            if (string.IsNullOrEmpty(configName))
                return configName;

            string subConfigName = mecNode.GetValue("activePatchName");
            if (!string.IsNullOrEmpty(subConfigName))
                configName = configName + Separator + subConfigName;

            return configName;
        }

        protected bool TestEngineConfig(ConfigNode mecNode)
        {
            LoadData();

            string configName = GetConfigString(mecNode);

            if (string.IsNullOrEmpty(configName))
                return false;

            return upgradeList.ContainsKey(configName);
        }

        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName)
        {
            nodeName = NodeUtil.GetPartNodeNameValue(node, loadContext);
            TestResult res = TestResult.Pass;
            if (node.GetNode("MODULE", "name", "ModuleEngineConfigs") is ConfigNode mecNode)
                res = TestEngineConfig(mecNode) ? TestResult.Upgradeable : TestResult.Pass;
            return res;
        }

        public override void OnUpgrade(ConfigNode node, LoadContext loadContext, ConfigNode parentNode)
        {
            LoadData();
            var mecNode = node.GetNode("MODULE", "name", "ModuleEngineConfigs");
            string oldConfig = GetConfigString(mecNode);
            string newConfig;
            string newSubConfig = string.Empty;
            if (!upgradeList.TryGetValue(oldConfig, out newConfig))
            {
                // Should never hit, since we do OnTest first, but...
                Debug.LogError($"[RealismOverhaul] UpgradePipeline error: couldn't find oldconfig {oldConfig} in set of configs to upgrade! Context {loadContext} updated part {NodeUtil.GetPartNodeNameValue(node, loadContext)}");
                return;
            }
            int idx = newConfig.IndexOf(Separator);
            if (idx != -1)
            {
                newSubConfig = newConfig.Substring(idx + 1);
                newConfig = newConfig.Substring(0, idx);
            }
            mecNode.SetValue("configuration", newConfig);
            if (string.IsNullOrEmpty(newSubConfig))
                mecNode.RemoveValue("activePatchName");
            else
                mecNode.SetValue("activePatchName", newSubConfig, true);

            Debug.Log($"[RealismOverhaul] UpgradePipeline context {loadContext} updated part {NodeUtil.GetPartNodeNameValue(node, loadContext)} module ModuleEngineConfigs config {oldConfig} to {newConfig} with subconfig {newSubConfig}.");
        }
    }

    public class MECEngineConfigUpgrade_KCTBase : MECEngineConfigUpgrade
    {
        public override string Name { get => $"{base.Name} KCT-{nodeUrlSFS}"; }
        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName) =>
            loadContext == LoadContext.Craft ? TestResult.Pass : base.OnTest(node, loadContext, ref nodeName);
    }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/VABList/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT1 : MECEngineConfigUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHList/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT2 : MECEngineConfigUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/VABWarehouse/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT3 : MECEngineConfigUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHWarehouse/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT4 : MECEngineConfigUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/VABPlans/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT5 : MECEngineConfigUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHPlans/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT6 : MECEngineConfigUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/LaunchComplex/BuildList/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT7 : MECEngineConfigUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/LaunchComplex/Warehouse/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT8 : MECEngineConfigUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/LaunchComplex/Plans/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade_KCT9 : MECEngineConfigUpgrade_KCTBase { }
}