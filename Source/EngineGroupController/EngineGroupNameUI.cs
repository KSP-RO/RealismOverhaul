using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGUIFramework;
using UnityGUIFramework.Controls;

namespace EngineGroupController
{
    public class EngineGroupNameUI : GUIWindow
    {
#pragma warning disable 649
        private ButtonControl _btnAccept;
        private ButtonControl _btnCancel;
        private TextBoxControl _tbName;
#pragma warning restore 649
        private Action<string, bool> _resultFunc;

        public EngineGroupNameUI(string title, GUIFramework framework)
            : base(title, framework, typeof (EngineGroupNameUI), "EngineGroupNameUI.xml")
        {
            IsModal = true;
            _btnCancel.ButtonPressed += BtnCancelOnButtonPressed;
            _btnAccept.ButtonPressed += BtnAcceptOnButtonPressed;
            _btnAccept.Enabled = false;
            _tbName.TextChanged += TbNameOnTextChanged;
        }

        private void TbNameOnTextChanged(string s)
        {
            _btnAccept.Enabled = !string.IsNullOrEmpty(s);
        }

        private void BtnAcceptOnButtonPressed()
        {
            RenderEnabled = false;
            _resultFunc(_tbName.Text, true);
            InputLockManager.RemoveControlLock("EGN_LOCK");
        }

        private void BtnCancelOnButtonPressed()
        {
            RenderEnabled = false;
            _resultFunc(string.Empty, false);
            InputLockManager.RemoveControlLock("EGN_LOCK");
        }

        public void DoModal(Action<string, bool> resultFunc)
        {
            _resultFunc = resultFunc;
            _tbName.Text = string.Empty;
            RenderEnabled = true;
            InputLockManager.SetControlLock(
                ControlTypes.EDITOR_LOCK | ControlTypes.EDITOR_NEW | ControlTypes.EDITOR_LAUNCH |
                ControlTypes.EDITOR_EDIT_STAGES, "EGN_LOCK");
        }

        private static EngineGroupNameUI _gui;

        public static EngineGroupNameUI Show(Action<string, bool> resultFunc)
        {
            if (_gui == null)
            {
                _gui = new EngineGroupNameUI("Engine Group Id", new GUIFramework());
            }
            _gui.DoModal(resultFunc);
            return _gui;
        }
    }
}
