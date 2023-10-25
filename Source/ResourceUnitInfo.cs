using System.Collections.Generic;
using System;
using KSP.Localization;

namespace RealismOverhaul
{
    public class ResourceUnits
    {
        public static void ModuleManagerPostLoad()
        {
            LoadResourceUnitInfo();
        }

        public static readonly int ECResID = "ElectricCharge".GetHashCode();

        public sealed class ResourceUnitInfo : IConfigNode
        {
            // We have to disable the "never assigned" warning
            // because ConfigNode deserialization is what actually
            // assigns some of these fields
#pragma warning disable CS0649
            [Persistent]
            private string name;
            public string Name => name;
            [Persistent]
            private string rateUnit;
            public string RateUnit => rateUnit;
            [Persistent]
            private bool useRatePostfix = true;
            public bool UseRatePostfix => useRatePostfix;
            [Persistent]
            private string amountUnit;
            public string AmountUnit => amountUnit;
            [Persistent]
            private double multiplierToUnit = 1d;
            public double MultiplierToUnit => multiplierToUnit;
            [Persistent]
            private bool useHuman = false;
            public bool UseHuman => useHuman;
#pragma warning restore CS0649

            private bool isValid;
            public bool IsValid => isValid;

            public void Load(ConfigNode node)
            {
                ConfigNode.LoadObjectFromConfig(this, node);
                if (string.IsNullOrEmpty(rateUnit))
                    rateUnit = amountUnit;
                if (!useHuman && useRatePostfix && rateUnit != null)
                    rateUnit = Localizer.Format("#autoLOC_6001048", rateUnit);

                isValid = !string.IsNullOrEmpty(name) && rateUnit != null && amountUnit != null;
            }

            public void Save(ConfigNode node)
            {
                ConfigNode.CreateConfigFromObject(this, node);
            }
        }

        private static readonly Dictionary<int, ResourceUnitInfo> resourceUnitInfos = new Dictionary<int, ResourceUnitInfo>();

        public static void LoadResourceUnitInfo()
        {
            resourceUnitInfos.Clear();

            ConfigNode[] defs = GameDatabase.Instance.GetConfigNodes("RESOURCE_DEFINITION");
            foreach (var node in defs)
            {
                var info = new ResourceUnitInfo();
                info.Load(node);
                if (info.IsValid)
                    resourceUnitInfos[info.Name.GetHashCode()] = info;
            }
            //Lib.Log("ResourceUnitInfo: Loaded " + resourceUnitInfos.Count + " infos from " + defs.Length + " nodes.");
        }

        public static ResourceUnitInfo GetResourceUnitInfo(PartResourceDefinition res)
        {
            return GetResourceUnitInfo(res.id);
        }

        public static ResourceUnitInfo GetResourceUnitInfo(string resName)
        {
            return GetResourceUnitInfo(resName.GetHashCode());
        }

        public static ResourceUnitInfo GetResourceUnitInfo(int id)
        {
            resourceUnitInfos.TryGetValue(id, out var info);
            return info;
        }

        public static string PrintMass(double mass, int sigFigs = 3, bool longPrefix = false)
        {
            return mass < 1d ? KSPUtil.PrintSI(mass * 1000d * 1000d, longPrefix ? "grams" : "g", sigFigs, longPrefix) : KSPUtil.PrintSI(mass, longPrefix ? "tons" : "t", sigFigs, longPrefix);
        }

        public static string PrintRatePerSecBare(double rate, int resID, int sigFigs = 3, string precision = "G2", bool longPrefix = false)
        {
            string unitRate = string.Empty;
            bool useSI = false;
            if (GetResourceUnitInfo(resID) is ResourceUnitInfo rui)
            {
                unitRate = rui.RateUnit;
                rate  *= rui.MultiplierToUnit;
                if (rui.UseHuman)
                {
                    useSI = false;
                }
                else
                {
                    useSI = true;
                }
            }
            if (useSI)
                return KSPUtil.PrintSI(rate, unitRate, sigFigs, longPrefix);
            else
                return rate.ToString(precision) + unitRate;
        }
        
        public static string PrintRate(double rate, int resID, bool showFlowMode, ModuleResource res = null, Propellant p = null, bool showMass = false, int sigFigs = 3, bool longPrefix = false)
        {
            bool varyTime = true;
            string unitRate = string.Empty;
            double rMult = 1d;
            bool useSI = false;
            if (res != null)
            {
                varyTime = res.varyTime;
                unitRate = res.unitName;
                rMult = res.displayUnitMult;
                useSI = res.useSI;
            }
            if (GetResourceUnitInfo(resID) is ResourceUnitInfo rui)
            {
                unitRate = rui.RateUnit;
                rMult = rui.MultiplierToUnit;
                if (rui.UseHuman)
                {
                    useSI = false;
                    varyTime = true;
                }
                else
                {
                    useSI = true;
                    varyTime = false;
                }
            }

            rate *= rMult;

            string output = string.Empty;
            PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition(resID);
            double density = showMass ? (definition?.density ?? 0d) : 0d;
            string title = definition == null ? (res?.title ?? string.Empty) : definition.displayName;
            if (varyTime)
            {
                if (Math.Abs(rate) > 0.1d)
                {
                    string massRate = density * rate > 0d ? " - " + PrintMass(rate * density) : string.Empty;
                    output += Localizer.Format("#autoLOC_244197", title, rate.ToString("0.0") + " " + unitRate + massRate);
                }
                else if (Math.Abs(rate) > (0.1d / 60d))
                {
                    string massRate = density * rate > 0d ? " - " + PrintMass(rate * density * 60d) : string.Empty;
                    output += Localizer.Format("#autoLOC_244201", title, (rate * 60d).ToString("0.0") + " " + unitRate + massRate);
                }
                else
                {
                    string massRate = density * rate > 0d ? " - " + PrintMass(rate * density * 3600d) : string.Empty;
                    output += Localizer.Format("#autoLOC_6002411", title, (rate * 3600d).ToString("0.0") + " " + unitRate + massRate);
                }
            }
            else
            {
                string massRate = density * rate > 0d ? " - " + Localizer.Format("#autoLOC_6001048", PrintMass(rate * density)) : string.Empty;
                if (useSI)
                    output += "- <b>" + title + ": </b>" + KSPUtil.PrintSI(rate, unitRate, sigFigs, longPrefix) + massRate + "\n";
                else
                    output += Localizer.Format("#autoLOC_6002412", title, rate.ToString("0.000"), " "  + unitRate + massRate);
            }
            if (showFlowMode && (p != null || definition != null))
            {
                if (p != null)
                    output += p.GetFlowModeDescription();
                else
                    output += definition.GetFlowModeDescription();
            }
            return output;
        }

        public static string PrintAmount(double amount, int resID, int sigFigs = 3, string precision = "G2", bool preventSI = false, bool longPrefix = false)
        {
            string unit = string.Empty;
            if (GetResourceUnitInfo(resID) is ResourceUnitInfo rui)
            {
                if (rui.UseHuman || preventSI)
                    unit = rui.AmountUnit;
                else
                    return PrintSIAmount(amount, rui, sigFigs, longPrefix);
            }

            return amount.ToString(precision) + " " + unit;
        }

        public static string PrintSIRate(double rate, string unit, int sigFigs = 3, bool longPrefix = false)
        {
            return KSPUtil.PrintSI(Math.Abs(rate), unit, sigFigs, longPrefix);
        }

        public static string PrintSIRate(double rate, int resID, int sigFigs = 3, bool longPrefix = false)
        {
            return PrintSIRate(rate, GetResourceUnitInfo(resID), sigFigs, longPrefix);
        }

        public static string PrintSIRate(double rate, ResourceUnitInfo rui, int sigFigs = 3, bool longPrefix = false)
        {
            return PrintSIRate(rate * rui.MultiplierToUnit, rui.RateUnit, sigFigs, longPrefix);
        }

        public static string PrintSIAmount(double amount, string unit, int sigFigs = 3, bool longPrefix = false)
        {
            return KSPUtil.PrintSI(amount, unit, sigFigs, longPrefix);
        }

        public static string PrintSIAmount(double rate, int resID, int sigFigs = 3, bool longPrefix = false)
        {
            return PrintSIAmount(rate, GetResourceUnitInfo(resID), sigFigs, longPrefix);
        }

        public static string PrintSIAmount(double rate, ResourceUnitInfo rui, int sigFigs = 3, bool longPrefix = false)
        {
            return PrintSIAmount(rate * rui.MultiplierToUnit, rui.AmountUnit, sigFigs, longPrefix);
        }

        public static string PrintSI(double amount, string unitName, int sigFigs = 3, bool longPrefix = false)
        {
            if (amount == 0d || double.IsInfinity(amount) || double.IsNaN(amount))
                return Localizer.Format("<<1>><<2>>", amount.ToString(), unitName);

            int exp = (int)Math.Floor(Math.Log10(Math.Abs(amount)));
            int prefix = ((exp >= 0) ? (exp / 3) : ((exp - 2) / 3));
            int digits = exp - prefix * 3 + 1;
            int decimals = sigFigs - digits;
            int index = Math.Max(0, Math.Min(prefix + KSPUtil.unitIndex, KSPUtil.prefixMults.Length - 1));
            string prefixString = (longPrefix ? KSPUtil.longSIprefixes[index] : KSPUtil.shortSIprefixes[index]);
            amount /= KSPUtil.prefixMults[index];
            if (decimals < 0)
            {
                double mult = KSPUtil.digitsScale[-decimals];
                amount = Math.Round(amount / mult) * mult;
                decimals = 0;
            }
            return Localizer.Format("<<1>> <<2>><<3>>", amount.ToString(decimals == 0 ? "F0" : "0." + new string('#', decimals)), prefixString, unitName);
        }
    }
}
