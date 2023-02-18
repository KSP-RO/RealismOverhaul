using UnityEngine;

namespace RealismOverhaul
{
    public class SeatConfig
    {
        [Persistent]
        public Vector3 colliderOffset = new Vector3(0, 0.4f, -1);

        [Persistent]
        public Vector3 colliderRotAngles = new Vector3(-90, 0, 0);

        [Persistent]
        public Vector3 forceDir = new Vector3(0, 0.15f, -1);

        /// <summary>
        /// Delay between the previous and current ejection.
        /// No effect on the first ejection since that will always happen instantly.
        /// </summary>
        [Persistent]
        public float ejectDelay = 0.5f;
    }
}