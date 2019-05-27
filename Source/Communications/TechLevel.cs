using RealismOverhaul.Utils;
using System.Collections.Generic;

namespace RealismOverhaul.Communications
{
    public class TechLevel
    {
        public float Efficiency { get; private set; }
        public float Gain { get; private set; }
        public float MinDataRate { get; private set; }
        public float MaxDataRate { get; private set; }
        public float MaxPower { get; private set; }
        public float MassPerWatt { get; private set; }
        public float BaseMass { get; private set; }
        public float BasePower { get; private set; }
        public float Frequency { get; private set; }
        public float BaseCost { get; private set; }
        public float CostPerWatt { get; private set; }

        public TechLevel(float efficiency, float gain, float minDataRate, float maxDataRate, float maxPower, float massPerWatt, float baseMass, float basePower, float frequency, float baseCost, float costPerWatt)
        {
            Efficiency = efficiency;
            Gain = gain;
            MinDataRate = minDataRate;
            MaxDataRate = maxDataRate;
            MaxPower = maxPower;
            MassPerWatt = massPerWatt;
            BaseMass = baseMass;
            BasePower = basePower;
            Frequency = frequency;
            BaseCost = baseCost;
            CostPerWatt = costPerWatt;
        }

        public static TechLevel GetTechLevel(int i)
        {
            if (i < 0)
            {
                i = 0;
            }
            return _techLevels[i];
        }

        private static IList<TechLevel> _techLevels = new List<TechLevel>()
        {
            new TechLevel(1/18f, -13f, 4, 4, 0.1f, 1.6f, 1, 20.5f, 8415f, 2, 5),
            new TechLevel(1/13f, -10f, 4, 4, 1, 1.34f, 0.26f, 20.5f, 8415f, 4, 4),
            new TechLevel(1/10f, -4.5f, 1, 64, 5, 1.16f, 6.9f, 20.5f, 8415f, 30, 3.5f),
            new TechLevel(3/23f, -2.5f, 8, 64, 5, 1, 20.2f, 20.5f, 8415f, 50, 3),
            new TechLevel(1/6f, 0f, 8, 4096, 10, 0.86f, 17.2f, 20.5f, 8415f, 80, 2.5f),
            new TechLevel(1/4.5f, 3.2f, 16, 16384, 20, 0.75f, 21, 21, 8415f, 120, 2),
            new TechLevel(1/4f, 6f, 16, 131072, 20, 11.6f/18, 30.7f, 21.4f, 8415f, 175, 1.7f),
            new TechLevel(20/53.7f, 7f, 16, 262144, 40, 10.8f/20, 21.3f, 18.3f, 8415f, 75, 0.5f),
            new TechLevel(41.6f/94.6f, 7f, 16, 134217728, 100, 5.9f/41.6f, 7.5f, 11.7f, 32050f, 50, 0.4f)
        };
    }
}
