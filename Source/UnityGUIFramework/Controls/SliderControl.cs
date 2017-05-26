using System;
using System.Linq;
using UnityEngine;

namespace UnityGUIFramework.Controls
{
    /// <summary>
    /// This class represents slider control
    /// </summary>
    public class SliderControl : GUIControl
    {
        /// <summary>
        /// Represents the value when slider is in left most or down most position
        /// </summary>
        public float LeftValue { get; set; }

        /// <summary>
        /// Represents the value when slider is in right most or up most position
        /// </summary>
        public float RightValue { get; set; }

        /// <summary>
        /// Current value
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Slider direction.
        /// </summary>
        public LayoutDirection Direction { get; set; }

        /// <summary>
        /// Fires when value is changed.
        /// </summary>
        public event Action<float> ValueChanged;

        /// <summary>
        /// Initializes a new instance of a class
        /// </summary>
        public SliderControl()
        {
            Direction = LayoutDirection.Horizontal;
        }

        protected virtual void OnValueChanged(float obj)
        {
            Action<float> handler = ValueChanged;
            if (handler != null) handler(obj);
        }

        protected override void OnRender()
        {
            var options = GetOptions().ToArray();
            float newVal;
            if (Direction == LayoutDirection.Horizontal)
            {
                newVal = GUILayout.HorizontalSlider(Value, LeftValue, RightValue, options);
            }
            else
            {
                newVal = GUILayout.VerticalSlider(Value, LeftValue, RightValue, options);
            }
            var range = Math.Abs(RightValue - LeftValue);
            var threshold = range / 1000.0f;
            if (Math.Abs(Value - newVal) > threshold)
            {
                Value = newVal;
                OnValueChanged(newVal);
            }
        }
    }
}
