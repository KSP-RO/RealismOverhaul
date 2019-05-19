using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealismOverhaul.Utils
{
    public static class Extensions
    {
        public static Transform GetModelTransform(this Part p) => p.partTransform.Find("model");
        public static Part GetPrefab(this Part p) => p.partInfo.partPrefab;

        public static float ToLog2(this float value) => Mathf.Log(value) / Mathf.Log(2);
        public static float FromLog2(this float value) => Mathf.Pow(2, value);
        public static float ToLog2(this int value) => Mathf.Log(value) / Mathf.Log(2);
        public static float FromLog2(this int value) => Mathf.Pow(2, value);

        public static float ToDB(this float value) => Mathf.Log10(value) * 10;
        public static float FromDB(this float value) => Mathf.Pow(10, value / 10f);

        public static string Format(this float value, string unit, int logBase = 1000)
        {
            var prefixes = new[] { "m", "", "k", "M", "G", "T", "P", "E" };
            var prefixNumber = (int)Mathf.Floor(Mathf.Log(value) / Mathf.Log(logBase));
            value /= Mathf.Pow(logBase, prefixNumber);
            var digits = (int)Mathf.Log10(value);
            return $"{value:G3}\u2009{prefixes[prefixNumber + 1]}{unit}";
        }

        public static float GetFloat(this ConfigNode node, string name, float defaultValue = 0)
        {
            node.TryGetValue(name, ref defaultValue);
            return defaultValue;
        }

        public static int GetInt(this ConfigNode node, string name, int defaultValue = 0)
        {
            node.TryGetValue(name, ref defaultValue);
            return defaultValue;
        }
    }
}
