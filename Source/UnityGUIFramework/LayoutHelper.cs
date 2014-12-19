using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityGUIFramework
{
    internal class LayoutHelper : IDisposable
    {
        private readonly LayoutDirection _direction;

        public LayoutHelper(LayoutDirection direction)
        {
            _direction = direction;
            if (_direction == LayoutDirection.Vertical)
                GUILayout.BeginVertical();
            else
            {
                GUILayout.BeginHorizontal();
            }
        }

        void IDisposable.Dispose()
        {
            if (_direction == LayoutDirection.Vertical)
                GUILayout.EndVertical();
            else
            {
                GUILayout.EndHorizontal();
            }
        }
    }
}
