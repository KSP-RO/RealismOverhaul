using UnityEngine;

namespace RealismOverhaul
{
    class RDTechFixer : MonoBehaviour
    {
        public bool fixParts = false;
        public void Start() { fixParts = true; }
        public void Update()
        {
            if (fixParts == true)
            {
                // Hides unwanted parts from the RnD techtree
                RDTech tech = GetComponent<RDTech>();
                RDTechFilters.Instance.FilterRDNode(tech);
                Destroy(this);
            }
        }
    }
}