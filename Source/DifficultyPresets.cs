using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RealismOverhaul
{
    public class RealismOverhaulSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "General Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "RealismOverhaul"; } }
        public override string DisplaySection { get { return "RealismOverhaul"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Speculative Level", toolTip = "What parts are available.\nReal = Only real hardware is available.\nProposal = Real proposals that were never built are available.\nScifi = Scifi parts are available.")]
        public RealismOverhaulspeculative speculativeLevel = RealismOverhaulspeculative.real;
        

    }

    public enum RealismOverhaulspeculative
    {
        real,
        proposal,
        scifi
    }
}
