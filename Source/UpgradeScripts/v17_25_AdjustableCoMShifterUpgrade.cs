using System;
using SaveUpgradePipeline;
using UnityEngine;

namespace RealismOverhaul.UpgradeScripts
{
    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/FLIGHTSTATE/VESSEL/PART", craftNodeUrl = "PART")]
    public class v17_25_AdjustableCoMShifterUpgrade : UpgradeScript
    {
        public override string Name => "RO AdjustableCoMShifter Unit Upgrade";
        public override string Description => "Converts AdjustableCoMShifter offset fields from centimeters to meters";
        public override Version EarliestCompatibleVersion => new Version(0, 0, 0);
        public override Version TargetVersion => new Version(17, 25, 0);

        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName)
        {
            nodeName = NodeUtil.GetPartNodeNameValue(node, loadContext);
            if (node.GetNode("MODULE", "name", "AdjustableCoMShifter") is ConfigNode comNode && HasNonZeroOffset(comNode))
                return TestResult.Upgradeable;
            return TestResult.Pass;
        }

        public override void OnUpgrade(ConfigNode node, LoadContext loadContext, ConfigNode parentNode)
        {
            ConfigNode comNode = node.GetNode("MODULE", "name", "AdjustableCoMShifter");
            if (comNode == null)
                return;

            ConvertOffset(comNode, "offsetX");
            ConvertOffset(comNode, "offsetY");
            ConvertOffset(comNode, "offsetZ");

            Debug.Log($"[RealismOverhaul] UpgradePipeline context {loadContext} converted AdjustableCoMShifter offsets from cm to m on part {NodeUtil.GetPartNodeNameValue(node, loadContext)}.");
        }

        private static bool HasNonZeroOffset(ConfigNode comNode) =>
            HasNonZeroValue(comNode, "offsetX") || HasNonZeroValue(comNode, "offsetY") || HasNonZeroValue(comNode, "offsetZ");

        private static bool HasNonZeroValue(ConfigNode node, string key) =>
            node.HasValue(key) && float.TryParse(node.GetValue(key), out float val) && val != 0f;

        private static void ConvertOffset(ConfigNode node, string key)
        {
            if (node.HasValue(key) && float.TryParse(node.GetValue(key), out float val))
                node.SetValue(key, val / 100f);
        }
    }

    public class v17_25_AdjustableCoMShifterUpgrade_KCTBase : v17_25_AdjustableCoMShifterUpgrade
    {
        public override string Name { get => $"{base.Name} KCT-{nodeUrlSFS}"; }
        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName) =>
            loadContext == LoadContext.Craft ? TestResult.Pass : base.OnTest(node, loadContext, ref nodeName);
    }

    [UpgradeModule(LoadContext.SFS, sfsNodeUrl = "GAME/SCENARIO/KSC/VABList/KCTVessel/ShipNode/PART")]
    public class v17_25_AdjustableCoMShifterUpgrade_KCT1 : v17_25_AdjustableCoMShifterUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHList/KCTVessel/ShipNode/PART")]
    public class v17_25_AdjustableCoMShifterUpgrade_KCT2 : v17_25_AdjustableCoMShifterUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS, sfsNodeUrl = "GAME/SCENARIO/KSC/VABWarehouse/KCTVessel/ShipNode/PART")]
    public class v17_25_AdjustableCoMShifterUpgrade_KCT3 : v17_25_AdjustableCoMShifterUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHWarehouse/KCTVessel/ShipNode/PART")]
    public class v17_25_AdjustableCoMShifterUpgrade_KCT4 : v17_25_AdjustableCoMShifterUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS, sfsNodeUrl = "GAME/SCENARIO/KSC/VABPlans/KCTVessel/ShipNode/PART")]
    public class v17_25_AdjustableCoMShifterUpgrade_KCT5 : v17_25_AdjustableCoMShifterUpgrade_KCTBase { }

    [UpgradeModule(LoadContext.SFS, sfsNodeUrl = "GAME/SCENARIO/KSC/SPHPlans/KCTVessel/ShipNode/PART")]
    public class v17_25_AdjustableCoMShifterUpgrade_KCT6 : v17_25_AdjustableCoMShifterUpgrade_KCTBase { }
}
