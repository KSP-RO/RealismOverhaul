using System.Linq;
using UnityEngine;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// This class represents label control
    /// </summary>
    public class LabelControl : GUIControl
    {
        /// <summary>
        /// Label text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Label tooltip
        /// </summary>
        public string Toolip { get; set; }

        /// <summary>
        /// Label image
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
        /// Label text align
        /// </summary>
        public TextAnchor Align { get; set; }

        /// <summary>
        /// Label text color
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Label text style
        /// </summary>
        public FontStyle Style { get; set; }

        /// <summary>
        /// Initializes a new instance of a class
        /// </summary>
        public LabelControl()
        {
            Align = TextAnchor.MiddleLeft;
            Text = string.Empty;
            Style = (FontStyle)(-1);
        }

        protected override void OnRender()
        {
            //GUI.skin.label.onNormal.
            var options = GetOptions().ToArray();
            var content = new GUIContent(Text, Image, Toolip);
            var oldFontStyle = GUI.skin.label.fontStyle;
            if (Style != (FontStyle)(-1))
            {
                GUI.skin.label.fontStyle = Style;
            }
            var oldAlign = GUI.skin.label.alignment;
            GUI.skin.label.alignment = Align;
            var oldColor = GUI.skin.label.normal.textColor;
            if (TextColor.a > 1e-4)
            {
                GUI.skin.label.normal.textColor = TextColor;
            }

            GUILayout.Label(content, options);

            if (TextColor.a > 1e-4)
            {
                GUI.skin.label.normal.textColor = oldColor;
            }
            GUI.skin.label.alignment = oldAlign;
            if (Style != (FontStyle)(-1))
            {
                GUI.skin.label.fontStyle = oldFontStyle;
            }
        }
    }
}