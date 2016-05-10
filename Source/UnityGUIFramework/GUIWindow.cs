using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityGUIFramework.Controls;

namespace UnityGUIFramework
{
    /// <summary>
    /// This class wrap window functionality.
    /// </summary>
    public class GUIWindow : GUICompositeControl
    {
        private readonly int _windowId;
        private Rect _windowRect;

        /// <summary>
        /// Window title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Window's left coordinate
        /// </summary>
        public float Left { get { return _windowRect.x; } set { _windowRect.x = value; } }

        /// <summary>
        /// Window's top coordinate
        /// </summary>
        public float Top { get { return _windowRect.y; } set { _windowRect.y = value; } }

        /// <summary>
        /// Window's width coordinate
        /// </summary>
        public override float Width { get { return _windowRect.width; } set { _windowRect.width = value; } }

        /// <summary>
        /// Window's height coordinate
        /// </summary>
        public override float Height { get { return _windowRect.height; } set { _windowRect.height = value; } } 

        /// <summary>
        /// Text color
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Used to control rendering of UI
        /// </summary>
        public bool RenderEnabled { get; set; }

        /// <summary>
        /// Delegate used to determine whether to display window or not (provided that <seealso cref="StartPostDisplay"/> is called).
        /// </summary>
        public Func<bool> IsDisplayDelegate { get; set; }

        public bool IsModal { get; set; }

        /// <summary>
        /// Initializes a new window and loads controls tree from resource.
        /// </summary>
        /// <param name="title">Window title</param>
        /// <param name="framework">Reference to <seealso cref="GUIFramework"/> object to be used for initializing the controls tree.</param>
        /// <param name="type">Type to use as a namespace for resource</param>
        /// <param name="resourceName">Resource name</param>
        public GUIWindow(string title, GUIFramework framework, Type type, string resourceName)
        {
            _windowId = GetHashCode();
            Title = title;
            IsDisplayDelegate = () => true;
            framework.InitWindow(this, type, resourceName);
            MapControlFields();
        }

        /// <summary>
        /// Initializes a new window, but DO NOT loads controls tree.
        /// </summary>
        /// <param name="windowId">Window unique id</param>
        /// <param name="title">Window title</param>
        public GUIWindow(int windowId, string title)
        {
            _windowId = windowId;
            Title = title;
            IsDisplayDelegate = () => true;
        }

        private void MapControlFields()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(GUIControl)))
                    continue;
                var fieldName = field.Name;
                if (fieldName.StartsWith("_"))
                    fieldName = fieldName.Substring(1);
                var ctrl = FindControl(fieldName);
                if (ctrl == null)
                    continue;
                if (field.FieldType.IsInstanceOfType(ctrl))
                {
                    field.SetValue(this, ctrl);
                }
            }
        }

        /// <summary>
        /// Renders window immediately.
        /// </summary>
        public void Show()
        {
            if (IsDisplayDelegate != null && !IsDisplayDelegate())
                return;

            var oldSkin = GUI.skin;
            GUI.skin = null;
            var oldColor = GUI.skin.window.normal.textColor;
            if (TextColor.a > 1e-4)
            {
                GUI.skin.window.normal.textColor = TextColor;
            }
            if (IsModal)
            {
                _windowRect = GUI.ModalWindow(_windowId, _windowRect, Render, Title);
            }
            else
            {
                _windowRect = GUILayout.Window(_windowId, _windowRect, Render, Title);
            }
            if (TextColor.a > 1e-4)
            {
                GUI.skin.window.normal.textColor = oldColor;
            }
            GUI.skin = oldSkin;
        }

        private void Render(int winId)
        {
            var oldEnabled = GUI.enabled;
            foreach (var guiControl in Children)
            {
                guiControl.Render();
            }
            GUI.enabled = oldEnabled;
            GUI.DragWindow();
        }

        ~GUIWindow()
        {
            RenderEnabled = false;
        }
    }
}
