using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealismOverhaul
{
    class AdjustableCoMShifter : CoMShifter
    {

        [KSPField(isPersistant = true, guiActive = true, guiName = "CoM Offset Limit")]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = 0.01f)]
        private float offsetPercentSlider = 1;

        protected float offsetPercent = 1;
 
        public override void ToggleMode()
        {
            IsDescentMode = !IsDescentMode;         
            updateCoM();
            Events["ToggleMode"].guiName="Turn Descent Mode "+(IsDescentMode?"Off":"On");
        }
        protected void updateCoM()
        {
            if(IsDescentMode)        
               part.CoMOffset=DescentModeCoM*offsetPercent+_defaultCoM;
            else
                part.CoMOffset=_defaultCoM;
            Fields["comString"].guiActive = PhysicsGlobals.ThermalDataDisplay;
            comString = part.CoMOffset.x.ToString("N3") + "," + part.CoMOffset.y.ToString("N3") + "," + part.CoMOffset.z.ToString("N3");
        }

        public void FixedUpdate()
        {
            if(IsDescentMode)                          //Not sure if necessary but could this check improve performance in case descent mode is off?
                if(offsetPercent!=offsetPercentSlider)
                {
                    offsetPercent = offsetPercentSlider;
                    updateCoM();
                }
        }
    }
}
