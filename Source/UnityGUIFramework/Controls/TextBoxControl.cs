using System;
using System.Linq;
using UnityEngine;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// This class represents textbox control
    /// </summary>
    public class TextBoxControl : GUIControl
    {
        /// <summary>
        /// Text that is displayed inside textbox
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Maximum length of the text that can be entered into the textbox
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Determines if textbox is multiline
        /// </summary>
        public bool IsMultiLine { get; set; }

        /// <summary>
        /// Text color
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Text font style
        /// </summary>
        public FontStyle Style { get; set; }

        /// <summary>
        /// Fires when text changes
        /// </summary>
        public event Action<string> TextChanged;

        /// <summary>
        /// Initializes a new instance of a class
        /// </summary>
        public TextBoxControl()
        {
            MaxLength = -1;
            Text = string.Empty;
            Style = (FontStyle) (-1);
        }

        protected override void OnRender()
        {
            var options = GetOptions().ToArray();
            string newVal;
            var oldColor = GUI.skin.label.normal.textColor;
            var oldFontStyle = GUI.skin.label.fontStyle;
            if (Style != (FontStyle)(-1))
            {
                GUI.skin.textArea.fontStyle = Style;
                GUI.skin.textField.fontStyle = Style;
            }
            if (TextColor.a > 1e-4)
            {
                GUI.skin.textArea.normal.textColor = TextColor;
                GUI.skin.textField.normal.textColor = TextColor;
            }
            if (MaxLength != -1)
            {
                if (IsMultiLine)
                    newVal = GUILayout.TextArea(Text, MaxLength, options);
                else
                    newVal = GUILayout.TextField(Text, MaxLength, options);
            }
            else
            {
                if (IsMultiLine)
                    newVal = GUILayout.TextArea(Text, options);
                else
                    newVal = GUILayout.TextField(Text, options);
            }
            if (TextColor.a > 1e-4)
            {
                GUI.skin.textArea.normal.textColor = oldColor;
                GUI.skin.textField.normal.textColor = oldColor;
            }
            if (Style != (FontStyle)(-1))
            {
                GUI.skin.textArea.fontStyle = oldFontStyle;
                GUI.skin.textField.fontStyle = oldFontStyle;
            }

            if (newVal != Text)
            {
                Text = newVal;
                var changedEvent = TextChanged;
                if (changedEvent != null)
                {
                    changedEvent(newVal);
                }
            }
        }
    }
}
