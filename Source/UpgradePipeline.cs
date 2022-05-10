using System;
using System.Linq;
using SaveUpgradePipeline;
using UnityEngine;

namespace RealismOverhaul
{
    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/FLIGHTSTATE/VESSEL/PART", craftNodeUrl = "PART")]
    public class MECEngineConfigUpgrade : UpgradeScript
    {
        public override string Name { get => "RO MEC Config Upgrader"; }
        public override string Description { get => "Updates Engines configs from outdated to new"; }
        public override Version EarliestCompatibleVersion { get => new Version(1, 0, 0); }
        public override Version TargetVersion { get => new Version(1, 8, 1); }
        //protected override bool CheckMaxVersion(Version v) => true; // Upgrades are ProcParts-dependent, not KSP version.
        protected override TestResult VersionTest(Version v) => TestResult.Upgradeable;

        protected bool EngineConfigMatches(string configName)
        {
            if (string.IsNullOrEmpty(configName))
                return false;

            if (configName == "LR87-AJ-9-Kero-15AR")
                return true;

            return false;
        }

        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName)
        {
            nodeName = NodeUtil.GetPartNodeNameValue(node, loadContext);
            TestResult res = TestResult.Pass;
            if (node.GetNode("MODULE", "name", "ModuleEngineConfigs") is ConfigNode mecNode)
                res = EngineConfigMatches(mecNode.GetValue("configuration")) ? TestResult.Upgradeable : TestResult.Pass;
            return res;
        }

        public override void OnUpgrade(ConfigNode node, LoadContext loadContext, ConfigNode parentNode)
        {
            var mecNode = node.GetNode("MODULE", "name", "ModuleEngineConfigs");
            string oldConfig = mecNode.GetValue("configuration");
            string newConfig = "LR87-AJ-9-Kero";
            mecNode.SetValue("configuration", newConfig);
            mecNode.SetValue("__mpecPatchName", "15AR", true);

            Debug.Log($"[RealismOverhaul] UpgradePipeline context {loadContext} updated part {NodeUtil.GetPartNodeNameValue(node, loadContext)} module ModuleEngineConfigs config {oldConfig} to {newConfig} with subconfig 15AR");
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