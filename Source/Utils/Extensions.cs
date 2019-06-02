using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static float ToDBm(this float value) => Mathf.Log10(value * 1000) * 10;
        public static float FromDBm(this float value) => Mathf.Pow(10, value / 10) / 1000;

        public static string Format(this float value, string unit, int logBase = 1000)
        {
            var prefixes = new[] { "m", "", "k", "M", "G", "T", "P", "E" };
            var prefixNumber = (int)Mathf.Floor(Mathf.Log(value) / Mathf.Log(logBase));
            value /= Mathf.Pow(logBase, prefixNumber);
            var digits = (int)Mathf.Log10(value);
            return $"{value:G3}\u2009{prefixes[prefixNumber + 1]}{unit}";
        }

        public static string Format(this double value, string unit, int logBase = 1000)
        {
            return Format((float) value, unit, logBase);
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

        public static string GetString(this ConfigNode node, string name, string defaultValue = "")
        {
            node.TryGetValue(name, ref defaultValue);
            return defaultValue;
        }

        public static float GetFloat(this ProtoPartModuleSnapshot snapshot, string name, float defaultValue = 0)
        {
            return snapshot.moduleValues.GetFloat(name, defaultValue);
        }

        public static int GetInt(this ProtoPartModuleSnapshot snapshot, string name, int defaultValue = 0)
        {
            return snapshot.moduleValues.GetInt(name, defaultValue);
        }

        public static string GetString(this ProtoPartModuleSnapshot snapshot, string name, string defaultValue = "")
        {
            return snapshot.moduleValues.GetString(name, defaultValue);
        }

        public static float Pow(this float @base, float exp)
        {
            return Mathf.Pow(@base, exp);
        }

        public static float Pow(this int @base, float exp)
        {
            return Mathf.Pow(@base, exp);
        }

        public static T GetField<T>(this object obj, string field)
        {
            return (T) GetField(obj, field).GetValue(obj);
        }

        public static void SetField<T>(this object obj, string field, T value)
        {
            GetField(obj, field).SetValue(obj, value);
        }

        private static FieldInfo GetField(object obj, string field) => obj.GetType().GetField(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
