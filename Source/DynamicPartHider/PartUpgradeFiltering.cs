using HarmonyLib;
using KSP.UI.Screens;

namespace RealismOverhaul.Harmony
{
	[HarmonyPatch(typeof(RDPartList))]
	internal class PatchRDPartList
	{
		[HarmonyPrefix]
		[HarmonyPatch("AddUpgradeListItem")]
		private void Prefix_AddUpgradeListItem(PartUpgradeHandler.Upgrade upgrade, bool purchased)
		{
			foreach (var filter in Filters.Instance)
			{
				if (!filter.IsUpgradeAvaliable(upgrade))
					return;
			}
		}
	}
}