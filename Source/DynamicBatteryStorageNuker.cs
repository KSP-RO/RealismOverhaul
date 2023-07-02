using System.Linq;
using UnityEngine;

namespace RealismOverhaul
{
    /// <summary>
    /// DynamicBatteryStorage is supposed to disable itself in presence of Kerbalism but the implementation is flawed.
    /// In addition to Kerbalism, the mod is also known to cause issues with ROTanks and RealFuels.
    /// The code below will remove the problematic VesselModule.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class DynamicBatteryStorageNuker : MonoBehaviour
    {
        internal void Start()
        {
            var la = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.assembly.GetName().Name == "DynamicBatteryStorage");
            if (la != null)
            {
                var type = la.assembly.GetType("DynamicBatteryStorage.ModuleDynamicBatteryStorage");
                bool removed = VesselModuleManager.RemoveModuleOfType(type);
                if (!removed)
                {
                    Debug.Log("[RealismOverhaul] Detected DynamicBatteryStorage as installed but failed to remove its VesselModule");
                }
            }
        }
    }
}
