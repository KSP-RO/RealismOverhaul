using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI;

namespace RealismOverhaul
{
    public class EditorCrewMass : PartModule, IPartMassModifier
    {
        [KSPField(guiActiveEditor = true, guiName = "Crew Aboard")]
        public int totalCrew = 0;

        public void FixedUpdate()
        {
            totalCrew = 0;
            if (HighLogic.LoadedSceneIsEditor)
            {
                VesselCrewManifest man = CrewAssignmentDialog.Instance.GetManifest();
                if (man != null && part != null)
                {
                    foreach (PartCrewManifest pcm in man)
                    {
                        if (pcm.PartID == this.part.craftID)
                        {
                            ProtoCrewMember[] crew = pcm.GetPartCrew();
                            for (int i = crew.Length - 1; i >= 0; --i)
                            {
                                if (crew[i] != null)
                                    ++totalCrew;
                            }
                        }
                    }
                }
            }
        }

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit)
        {
            if(HighLogic.LoadedSceneIsEditor)
                return totalCrew * PhysicsGlobals.KerbalCrewMass;

            return 0f;
        }
        public ModifierChangeWhen GetModuleMassChangeWhen()
        {
            return ModifierChangeWhen.FIXED;
        }
    }
}
