using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealismOverhaul.DataTransmitterRO
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
            return _techLevels[i];
        }

        private static IList<TechLevel> _techLevels = new List<TechLevel>()
        {
            new TechLevel(1/10f, -13f, 4, 0.1f, 21.7f/18, 30.7f, 10.3f, 8415f),
            new TechLevel(1/8f, -10f, 4, 1, 21.7f/18, 30.7f, 10.3f, 8415f),
            new TechLevel(1/7f, -4.5f, 1, 5, 21.7f/18, 30.7f, 10.3f, 8415f),
            new TechLevel(1/6f, -2.5f, 8, 5, 21.7f/18, 30.7f, 10.3f, 8415f),
            new TechLevel(1/5.5f, 0f, 8, 10, 21.7f/18, 30.7f, 10.3f, 8415f),
            new TechLevel(1/4.5f, 3.2f, 16, 20, 21.7f/18, 30.7f, 10.3f, 8415f),
            new TechLevel(1/4f, 6f, 128, 40, 21.7f/18, 30.7f, 10.3f, 8415f),
            new TechLevel(1/3f, 7f, 128, 80, 21.7f/18, 30.7f, 10.3f, 8415f)
        };
    }
}
