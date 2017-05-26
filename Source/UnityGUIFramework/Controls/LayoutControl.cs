namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// This class represents layout control
    /// </summary>
    public class LayoutControl : GUICompositeControl
    {
        /// <summary>
        /// Layout direction
        /// </summary>
        public LayoutDirection Direction { get; set; }

        /// <summary>
        /// Initializes a new instance of a class
        /// </summary>
        public LayoutControl()
        {
            Direction = LayoutDirection.Vertical;
        }

        protected override void OnRender()
        {
            using (new LayoutHelper(Direction))
            {
               base.OnRender();
            }
        }
    }
}
