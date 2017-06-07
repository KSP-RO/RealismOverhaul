using System;
using System.Linq;
using UnityEngine;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// This class represents toggle button control
    /// </summary>
    public class ToggleControl : GUIControl
    {
        /// <summary>
        /// Button text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Button tooltip
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Button image
        /// </summary>
        /// <remarks>
        /// When defining in markup, a string can be used to either load image from resources, or from path.
        /// To load image from resource, use the following syntax: resource://Namespace.Type,Assembly;Resourcename.png, 
        /// where Namespace.Type is a full type name for type used to define the scope of a resource, 
        /// Assembly is assembly name for resource to be loaded from,
        /// Resourcename.png is resource name.
        /// To load image from file, use the following syntax: file:///path/to/file/File.png,
        /// where path/to/file/File.png is a path to the image relative to KSP's root folder
        /// </remarks>
        public Texture Image { get; set; }

        /// <summary>
        /// Determines if toggle is checked
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Fires when checked state changes
        /// </summary>
        public event Action<bool> CheckChanged;

        /// <summary>
        /// Initializes a new instance of a class
        /// </summary>
        public ToggleControl()
        {
            Text = string.Empty;
        }

        protected virtual void OnCheckChanged(bool obj)
        {
            Action<bool> handler = CheckChanged;
            if (handler != null) handler(obj);
        }

        protected override void OnRender()
        {
            var options = GetOptions().ToArray();
            var content = new GUIContent(Text, Image, Tooltip);
            var newVal = GUILayout.Toggle(IsChecked, content, options);
            if (newVal != IsChecked)
            {
                IsChecked = newVal;
                OnCheckChanged(newVal);
            }
        }
    }
}
