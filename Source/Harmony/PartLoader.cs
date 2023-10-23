using HarmonyLib;
using KSP.Localization;
using System.Collections.Generic;

namespace RealismOverhaul.Harmony
{
    [HarmonyPatch(typeof(PartLoader))]
    internal class PatchPartLoader
    {
        [HarmonyPostfix]
        [HarmonyPatch("CompilePartInfo")]
        internal static void Postfix_CompilePartInfo(AvailablePart newPartInfo, Part part)
        {
            // better to fix in place, but we'll just recreate.
            newPartInfo.resourceInfos.Clear();

            string rInfoStr = string.Empty;
            foreach(var partResource in part.Resources)
            {
                int resID = partResource.info.id;
                AvailablePart.ResourceInfo resourceInfo = new AvailablePart.ResourceInfo();
                resourceInfo.resourceName = partResource.resourceName;
                resourceInfo.displayName = partResource.info.displayName.LocalizeRemoveGender();
                resourceInfo.info = Localizer.Format("#autoLOC_166269", ResourceUnits.PrintAmount(partResource.amount, resID, 6, "F1"));
                if (partResource.amount != partResource.maxAmount)
                    resourceInfo.info += " " + Localizer.Format("#autoLOC_6004042", ResourceUnits.PrintAmount(partResource.maxAmount, resID, 6, "F1"));
                double tons = partResource.amount * (double)partResource.info.density;
                if (tons > 0d)
                    resourceInfo.info += Localizer.Format("#autoLOC_166270", ResourceUnits.PrintMass(tons));
                if (partResource.info.unitCost > 0f)
                    resourceInfo.info += Localizer.Format("#autoLOC_166271", (partResource.amount * (double)partResource.info.unitCost).ToString("F2"));
                if (partResource.maxAmount > 0.0)
                    resourceInfo.primaryInfo = "<b>" + resourceInfo.displayName + ": </b>" + ResourceUnits.PrintAmount(partResource.maxAmount, resID, 6, "F1");

                if (!string.IsNullOrEmpty(resourceInfo.info))
                {
                    newPartInfo.resourceInfos.Add(resourceInfo);
                    if (rInfoStr != string.Empty)
                        rInfoStr += "\n";

                    rInfoStr += resourceInfo.info;
                }
            }
            if (part.Resources.Count > 0)
            {
                rInfoStr = rInfoStr + "\nDry Mass: " + part.mass;
            }
            newPartInfo.resourceInfo = rInfoStr;
            newPartInfo.resourceInfos.Sort((AvailablePart.ResourceInfo rp1, AvailablePart.ResourceInfo rp2) => rp1.resourceName.CompareTo(rp2.resourceName));
        }
    }
}
