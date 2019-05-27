using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealismOverhaul.Communications
{
    class AntennaSpecs
    {
        public float TxUsedPower { get; set; }
        public float TotalPower { get; set; }
        public double CommNetPower { get; set; }
        public float MinDataRate { get; set; }
        public float MaxDataRate { get; set; }

        public AntennaSpecs(float txUsedPower, float totalPower, double commNetPower, float minDataRate, float maxDataRate)
        {
            TxUsedPower = txUsedPower;
            TotalPower = totalPower;
            CommNetPower = commNetPower;
            MinDataRate = minDataRate;
            MaxDataRate = maxDataRate;
        }
    }
}
