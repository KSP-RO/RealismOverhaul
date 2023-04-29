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

        [GameParameters.CustomParameterUI("Speculative Level", toolTip =
        "What parts are available.\n"+
        "Operational = Historically flown hardware.\n"+
        "Prototype = Hardware that was tested in whole or in part, but has not flown.\n"+
        "Concept = Real projects that made it to a paper design study or mockup.\n"+
        "Speculative = Realistic extrapolations of historical designs.\n"+
        "AltHist = Designs from fictional timelines that nonetheless match the performance of real hardware.\n"+
        "SciFi = The sky's the limit!")]
        public SpeculativeLevel speculativeLevel = SpeculativeLevel.Speculative;

        [GameParameters.CustomParameterUI("Show Deprecated Parts", toolTip = "Deprecated parts are shown when this option is enabled.")]
        public bool showDeprecated = false;
    }
}
