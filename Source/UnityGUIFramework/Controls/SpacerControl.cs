using UnityEngine;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// This class represents spacer control. The spacer can be used to fill empty space between layout controls.
    /// </summary>
    public class SpacerControl : GUIControl
    {
        /// <summary>
        /// Defines a spacer width, if positive, or fills all available space if zero
        /// </summary>
        public int Size { get; set; }

        protected override void OnRender()
        {
            if (Size != 0)
            {
                GUILayout.Space(Size);
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
        }
    }
}
