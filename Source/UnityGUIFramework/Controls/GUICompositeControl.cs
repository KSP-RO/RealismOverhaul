using System.Collections.Generic;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// Base class for composite controls
    /// </summary>
    public abstract class GUICompositeControl : GUIControl
    {
        protected GUICompositeControl()
        {
            Children = new List<GUIControl>();
        }

        /// <summary>
        /// Collection of child controls
        /// </summary>
        public IList<GUIControl> Children { get; private set; }

        /// <summary>
        /// Looks up control by its' identifier
        /// </summary>
        /// <typeparam name="T">Control type</typeparam>
        /// <param name="controlId">Control Id</param>
        /// <returns>Reference to the control, or null if control can't be found</returns>
        public T FindControl<T>(string controlId) where T: GUIControl
        {
            foreach (var child in Children)
            {
                if (child.ControlId == controlId)
                    return (T) child;
                var composite = child as GUICompositeControl;
                if (composite != null)
                {
                    var res = composite.FindControl<T>(controlId);
                    if (res != null)
                        return res;
                }
            }
            return null;
        }

        /// <summary>
        /// Looks up control by its' identifier
        /// </summary>
        /// <param name="controlId">Control Id</param>
        /// <returns>Reference to the control, or null if control can't be found</returns>
        public GUIControl FindControl(string controlId)
        {
            foreach (var child in Children)
            {
                if (child.ControlId == controlId)
                    return child;
                var composite = child as GUICompositeControl;
                if (composite != null)
                {
                    var res = composite.FindControl(controlId);
                    if (res != null)
                        return res;
                }
            }
            return null;
        }

        /// <summary>
        /// Renders child controls
        /// </summary>
        protected override void OnRender()
        {
            foreach (var child in Children)
            {
                child.Render();
            }
        }
    }
}
