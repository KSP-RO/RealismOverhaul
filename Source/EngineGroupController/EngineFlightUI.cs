using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGUIFramework;
using UnityGUIFramework.Controls;

namespace EngineGroupController
{
    public class EngineFlightUI : GUIWindow
    {
#pragma warning disable 649
        private ScrollAreaControl _saTable;
#pragma warning restore 649

        private readonly IDictionary<EngineGroup, LayoutControl> _groupControls = new Dictionary<EngineGroup, LayoutControl>();
        private readonly LayoutControl _root;

        public EngineFlightUI(string title, GUIFramework framework)
            : base(title, framework, typeof (EngineFlightUI), "EngineFlightUI.xml")
        {
            _root = new LayoutControl {Direction = LayoutDirection.Vertical};

            _saTable.Children.Add(_root);
        }

        public void AddGroup(EngineGroup group)
        {
            if (_groupControls.Count == 0)
            {
                _root.Children.Clear();
            }
            var ctrl = BuildEngineGroupControls(group);
            _groupControls.Add(group, ctrl);
            _root.Children.Add(ctrl);

            this.Height = 30.0f + 30.0f*_groupControls.Count;
        }

        public void ClearGroups()
        {
            _groupControls.Clear();
            _root.Children.Clear();
            this.Height = 30.0f + 30.0f * _groupControls.Count;
        }

        public void RemoveGroup(EngineGroup group)
        {
            _root.Children.Remove(_groupControls[group]);
            _groupControls.Remove(group);
            this.Height = 30.0f + 30.0f * _groupControls.Count;
        }

        public void DisplayNoGroupsMessage()
        {
            _root.Children.Add(new LabelControl { Text = "No engine groups defined."});
        }

        private LayoutControl BuildEngineGroupControls(EngineGroup group)
        {
            var root = new LayoutControl {Direction = LayoutDirection.Horizontal};
            //root.Children.Add(new LabelControl { Text = group.GroupId, Width = 100.0f});
            var cbx = new ToggleControl {Text = group.GroupId, Width = 100.0f, IsChecked = group.IsEnabled};
            root.Children.Add(cbx);

            var valTextBox = new TextBoxControl { Enabled = false, Width = 40.0f, Text = group.Throttle.ToString("f0")};

            var slider = new SliderControl
            {
                Direction = LayoutDirection.Horizontal, 
                LeftValue = 1.0f, 
                RightValue = 100.0f, 
                Width = 140.0f, 
                Height = 10.0f, 
                Value = group.Throttle,
                Enabled = group.IsEnabled,
            };
            slider.ValueChanged += x => 
            { 
                group.Throttle = x;
                group.ThrottleChanged = true;
                valTextBox.Text = x.ToString("f0");
            };
            root.Children.Add(slider);
            root.Children.Add(valTextBox);

            cbx.CheckChanged += x =>
            {
                if (!x)
                {
                    group.Throttle = 100.0f;
                    group.ThrottleChanged = true;
                    /*slider.Value = 100.0f;
                    valTextBox.Text = "100";*/
                }
                else
                {
                    //slider.Enabled = true;
                    group.Throttle = slider.Value;
                    group.ThrottleChanged = true;
                }
                slider.Enabled = x;
                group.IsEnabled = x;
            };

            return root;
        }
    }
}
