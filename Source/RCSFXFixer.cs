using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealismOverhaul
{
    public class RCSFXFixer : PartModule
    {
        public List<ModuleRCS> rcsModules;
        int lastIdx = -1;

        [KSPField]
        public float fxScalar = 1f;

        public override void OnStart(StartState state)
        {
            rcsModules = new List<ModuleRCS>();

            ModuleRCS m;
            for (int i = part.Modules.Count - 1; i >= 0; --i)
            {
                m = part.Modules[i] as ModuleRCS;
                if (m != null)
                    rcsModules.Add(m);
            }
            lastIdx = rcsModules.Count - 1;
        }
        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight
                || (TimeWarp.CurrentRate > 1f && TimeWarp.WarpMode == TimeWarp.Modes.HIGH)
                || !vessel.ActionGroups[KSPActionGroup.RCS])
                return;

            ModuleRCS m;
            for (int i = lastIdx; i >= 0; --i)
            {
                m = rcsModules[i];
                if (!m.rcsEnabled)
                    continue;

                int lastT = m.thrustForces.Length - 1;
                float force;
                float forceRecip = 1f / m.thrusterPower;
                for (int j = lastT; j >= 0; --j)
                {
                    force = m.thrustForces[j];
                    if (force > 0f)
                    {
                        m.thrusterFX[j].SetPower(Mathf.Clamp(force * forceRecip * fxScalar, 0.1f, 1f));
                    }
                }
            }
        }
    }
}
