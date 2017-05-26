using System.Collections.Generic;
using UnityEngine;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// Base class for all controls
    /// </summary>
    public abstract class GUIControl
    {
        /// <summary>
        /// Control identifier, can be used to find control in the tree
        /// </summary>
        public string ControlId { get; set; }

        /// <summary>
        /// Control width
        /// </summary>
        public virtual float Width { get; set; }

        /// <summary>
        /// Control height
        /// </summary>
        public virtual float Height { get; set; }

        /// <summary>
        /// Determines if control is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Determines if control is to be rendered
        /// </summary>
        public bool Visible { get; set; }

        protected GUIControl()
        {
            Enabled = true;
            Visible = true;
        }

        /// <summary>
        /// Override in descendants and put rendering code there
        /// </summary>
        protected virtual void OnRender() {}

        /// <summary>
        /// Renders control
        /// </summary>
        public void Render()
        {
            if (!Visible)
                return;
            GUI.enabled = Enabled;
            if (!string.IsNullOrEmpty(ControlId))
            {
                GUI.SetNextControlName(ControlId);
            }
            OnRender();
        }

        /// <summary>
        /// Returns options
        /// </summary>
        /// <returns></returns>
        protected IList<GUILayoutOption> GetOptions()
        {
            var options = new List<GUILayoutOption>();
            if (System.Math.Abs (Width) > double.Epsilon)
            {
                options.Add(GUILayout.Width(Width));
            }
            if (System.Math.Abs (Height) > double.Epsilon)
            {
                options.Add(GUILayout.Height(Height));
            }
            return options;
        }
    }
}
