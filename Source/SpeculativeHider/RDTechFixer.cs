using UnityEngine;

namespace RealismOverhaul
{
    class RDTechFixer : MonoBehaviour
    {
        public bool fixParts = false;
        private int specLevel = SpecFuncs.GetCompInt();
        public void Start() { fixParts = true; }
        public void Update()
        {
            if (fixParts == true)
            {
                // Hides unwanted parts from the RnD techtree
                RDTech tech = GetComponent<RDTech>();
                SpecFuncs.PruneRDNode(tech, specLevel);
                Destroy(this);
            }
        }
    }
}