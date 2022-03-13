using UnityEngine;

namespace RealismOverhaul
{
    class RDTechFixer : MonoBehaviour
    {
        public void Update()
        {
            // Hides unwanted parts from the RnD techtree
            RDTech tech = GetComponent<RDTech>();
            RDTechFilters.Instance.FilterRDNode(tech);
            Destroy(this);
        }
    }
}