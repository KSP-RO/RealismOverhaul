using System;
using System.Linq;
using SaveUpgradePipeline;
using UnityEngine;

namespace ProceduralParts
{
    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/FLIGHTSTATE/VESSEL/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade : UpgradeScript
    {
        public override string Name { get => "RO Part Upgrader - MEC->MPEC"; }
        public override string Description { get => "Updates Engines from MEC to MPEC"; }
        public override Version EarliestCompatibleVersion { get => new Version(1, 0, 0); }
        public override Version TargetVersion { get => new Version(1, 8, 1); }
        //protected override bool CheckMaxVersion(Version v) => true; // Upgrades are ProcParts-dependent, not KSP version.
        protected override TestResult VersionTest(Version v) => TestResult.Upgradeable;

        protected bool EngineConfigMatches(string configName)
        {
            if (string.IsNullOrEmpty(configName))
                return false;

            if (configName.StartsWith("LR87"))
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
            mecNode.SetValue("name", "ModulePatchableEngineConfigs");
            if (mecNode.GetValue("configuration") == "LR87-AJ-9-Kero-15AR")
            {
                mecNode.SetValue("configuration", "LR87-AJ-9-Kero");
                mecNode.AddValue("__mpecPatchName", "15AR");
            }

            Debug.Log($"[RealismOverhaul] UpgradePipeline context {loadContext} updated part {NodeUtil.GetPartNodeNameValue(node, loadContext)} from ModuleEngineConfigs to ModulePatchableEngineConfigs");
        }
    }

    public class MECtoMPECUpgrade_KCTBase : MECtoMPECUpgrade
    {
        public override string Name { get => "RO Part Upgrader Upgrader MEC->MPEC KCT-" + nodeUrlSFS; }
        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName) =>
            loadContext == LoadContext.Craft ? TestResult.Pass : base.OnTest(node, loadContext, ref nodeName);
    }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/VABList/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT1 : MECtoMPECUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHList/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT2 : MECtoMPECUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/VABWarehouse/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT3 : MECtoMPECUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHWarehouse/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT4 : MECtoMPECUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/VABPlans/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT5 : MECtoMPECUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHPlans/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT6 : MECtoMPECUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/LaunchComplex/BuildList/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT7 : MECtoMPECUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/LaunchComplex/Warehouse/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT8 : MECtoMPECUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/SCENARIO/KSC/LaunchComplex/Plans/KCTVessel/ShipNode/PART", craftNodeUrl = "PART")]
    public class MECtoMPECUpgrade_KCT9 : MECtoMPECUpgrade_KCTBase { }
}