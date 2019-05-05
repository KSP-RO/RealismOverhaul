using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealismOverhaul.Communications
{
    public class TechLevel
    {
        public float Efficiency { get; private set; }
        public float Gain { get; private set; }
        public float MinDataRate { get; private set; }
        public float MaxPower { get; private set; }
        public float MassPerWatt { get; private set; }
        public float BaseMass { get; private set; }
        public float BasePower { get; private set; }
        public float Frequency { get; private set; }

        public TechLevel(float efficiency, float gain, float minDataRate, float maxPower, float massPerWatt, float baseMass, float basePower, float frequency)
        {
            Efficiency = efficiency;
            Gain = gain;
            MinDataRate = minDataRate;
            MaxPower = maxPower;
            MassPerWatt = massPerWatt;
            BaseMass = baseMass;
            BasePower = basePower;
            Frequency = frequency;
        }

        public static TechLevel GetTechLevel(int i)
        {
            if (i > _techLevels.Count)
            {
                i = 0;
            }
            return _techLevels[i];
        }

        private static IList<TechLevel> _techLevels = new List<TechLevel>()
        {
            new TechLevel(1/18f, -13f, 4, 0.1f, 1.6f, 1, 20.5f, 8415f),
            new TechLevel(1/13f, -10f, 4, 1, 1.34f, 0.26f, 20.5f, 8415f),
            new TechLevel(1/10f, -4.5f, 1, 5, 1.16f, 6.9f, 20.5f, 8415f),
            new TechLevel(3/21.5f, -2.5f, 8, 5, 1, 20.2f, 20.5f, 8415f),
            new TechLevel(1/6f, 0f, 8, 10, 0.86f, 17.2f, 20.5f, 8415f),
            new TechLevel(1/4.5f, 3.2f, 16, 20, 0.75f, 21, 21, 8415f),
            new TechLevel(1/4f, 6f, 16, 40, 11.6f/18, 30.7f, 21.4f, 8415f),
            new TechLevel(20/53.7f, 7f, 16, 80, 10.8f/20, 21.3f, 18.3f, 8415f)
        };
    }
}
