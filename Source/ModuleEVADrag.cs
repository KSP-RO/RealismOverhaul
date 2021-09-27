using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealismOverhaul
{
    public class ModuleEVADrag : PartModule
    {
        [KSPField]
        public double csaStowed;

        [KSPField]
        public double csaSemiDeployed;

        [KSPField]
        public double csaDeployed;

        protected ModuleEvaChute chute;

        public override void OnAwake()
        {
            base.OnAwake();

            if (chute == null)
                chute = part.FindModuleImplementing<ModuleEvaChute>();
        }
        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight || part.Rigidbody == null || part.ShieldedFromAirstream)
                return;

            double dragArea = 0d;
            if (!chute || (chute.deploymentState != ModuleParachute.deploymentStates.DEPLOYED && chute.deploymentState != ModuleParachute.deploymentStates.SEMIDEPLOYED))
            {
                dragArea = csaStowed;
            }
            else if (chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED)
            {
                dragArea = csaSemiDeployed;
            }
            else // deployed
            {
                dragArea = csaDeployed;
            }

            if (part.dynamicPressurekPa > 0)
            {
                Vector3d nVel = part.Rigidbody.GetPointVelocity(part.Rigidbody.transform.position) + Krakensbane.GetFrameVelocity();
                double mag = nVel.magnitude;
                if (mag > 0) // it had better be, if dynamic pressure is > 0
                {
                    nVel = nVel / mag;

                    float mach = (float)part.machNumber;
                    double dragForce = dragArea * part.dynamicPressurekPa
                        * (PhysicsGlobals.DragCurveTip.Evaluate(mach) + PhysicsGlobals.DragCurveTail.Evaluate(mach))
                        * PhysicsGlobals.DragCurveMultiplier.Evaluate(mach);

                    part.AddForceAtPosition(-nVel * dragForce, part.partTransform.TransformPoint(part.CoPOffset));
                }
            }
        }
    }
}
