using System;
using System.Linq;
using UnityEngine;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// This class represents button control
    /// </summary>
    public class ButtonControl : GUIControl
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
        /// Button text color
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Button text style
        /// </summary>
        public FontStyle Style { get; set; }

        /// <summary>
        /// Event is fired when button is pressed
        /// </summary>
        public event Action ButtonPressed;

        /// <summary>
        /// Initializes a new instance of a class
        /// </summary>
        public ButtonControl()
        {
            Text = string.Empty;
            Style = (FontStyle)(-1);
        }

        protected override void OnRender()
        {
            var oldColor = GUI.skin.button.normal.textColor;
            if (TextColor.a > 1e-4)
            {
                GUI.skin.button.normal.textColor = TextColor;
            }
            var oldFontStyle = GUI.skin.button.fontStyle;
            if (Style != (FontStyle)(-1))
            {
                GUI.skin.button.fontStyle = Style;
            }
            var options = GetOptions().ToArray();

            var content = new GUIContent(Text, Image, Tooltip);
            if (GUILayout.Button(content, options))
            {
                
                Event.current.Use();
                var evt = ButtonPressed;
                if (evt != null)
                {
                    evt();
                }
            }
            if (Style != (FontStyle)(-1))
            {
                GUI.skin.button.fontStyle = oldFontStyle;
            }
            if (TextColor.a > 1e-4)
            {
                GUI.skin.button.normal.textColor = oldColor;
            }
        }
    }
}
