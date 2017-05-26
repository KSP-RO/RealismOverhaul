using System.Linq;
using UnityEngine;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// This class represents scroll area control
    /// </summary>
    public class ScrollAreaControl : GUICompositeControl
    {
        /// <summary>
        /// Scroll position
        /// </summary>
        public Vector2 ScrollPosition { get; set; }

        /// <summary>
        /// Force display horizontal scrollbar
        /// </summary>
        public bool AlwaysDisplayHorizontal { get; set; }

        /// <summary>
        /// Force display vertical scrollbar
        /// </summary>
        public bool AlwaysDisplayVertical { get; set; }

        protected override void OnRender()
        {
            var options = GetOptions().ToArray();
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, AlwaysDisplayHorizontal, AlwaysDisplayVertical,
                                                        options);
            base.OnRender();
            GUILayout.EndScrollView();
        }
    }
}
