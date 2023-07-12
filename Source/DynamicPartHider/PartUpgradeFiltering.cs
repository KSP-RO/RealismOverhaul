using HarmonyLib;
using KSP.UI.Screens;
using UnityEngine;

namespace RealismOverhaul.Harmony
{
	[HarmonyPatch(typeof(RDPartList))]
	internal class PatchRDPartList
	{
		[HarmonyPrefix]
		[HarmonyPatch("AddUpgradeListItem")]
		internal static bool Prefix_AddUpgradeListItem(PartUpgradeHandler.Upgrade upgrade, bool purchased)
		{
			foreach (Filters.IFilter filter in Filters.Instance)
			{
				if (!filter.IsUpgradeAvaliable(upgrade))
					return false;
			}
			return true;
		}
	}
}