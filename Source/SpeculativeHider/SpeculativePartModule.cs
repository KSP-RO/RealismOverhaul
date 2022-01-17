namespace RealismOverhaul
{
    /// <summary>
    /// Stores info about how speculative a part is.
    /// Has a Validate function that checks if the part is allowed to be used.
    /// </summary>
    public class SpeculativePartModule : PartModule
    {
        [KSPField]
        public RealismOverhaulSpeculative specLevel = RealismOverhaulSpeculative.real;

        public virtual bool Validate(out string validationError)
        {
            var setting = SpecFuncs.GetSpecLevelSetting();
            if (specLevel > setting)
            {
                validationError = $"Part {part.name} is not available";
                return false;
            }
            validationError = "";
            return true;
        }
    }
}