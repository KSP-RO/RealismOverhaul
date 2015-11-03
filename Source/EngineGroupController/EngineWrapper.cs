namespace EngineGroupController
{
    public class EngineWrapper
    {
        internal readonly ModuleEngines _engineMod;
        internal readonly ModuleEnginesFX _engineModFx;

        public EngineWrapper(ModuleEngines engineMod)
        {
            _engineMod = engineMod;
        }

        public EngineWrapper(ModuleEnginesFX engineModFx)
        {
            _engineModFx = engineModFx;
        }

        public float Throttle
        {
            get { return _engineMod != null ? _engineMod.thrustPercentage : _engineModFx.thrustPercentage; }
            set
            {
                if (_engineMod != null)
                {
                    _engineMod.thrustPercentage = value;
                }
                else
                {
                    _engineModFx.thrustPercentage = value;
                }
            }
        }
    }
}