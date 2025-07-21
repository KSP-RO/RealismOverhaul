using ROUtils.DataTypes;

namespace RealismOverhaul
{
    public class HSClobbererSettings : ConfigNodePersistenceBase, IConfigNode
    {
        [Persistent]
        public float skinIntCondIncreaseMult = 200f;
        [Persistent]
        public float skinSkinCondIncreaseMult = 250f;
        [Persistent]
        public double ablatorThreshold = 0.01;
        [Persistent]
        public float effectTime = 5f;
        [Persistent]
        public PersistentHashSetValueType<string> excludedParts = new PersistentHashSetValueType<string>();

        public static HSClobbererSettings LoadHSClobbererSettings()
        {
            var settings = new HSClobbererSettings();
            foreach (ConfigNode n in GameDatabase.Instance.GetConfigNodes("HS_CLOBBERER_SETTINGS"))
            {
                settings.Load(n);
            }

            return settings;
        }
    }
}
