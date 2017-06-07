using System;
using UnityEngine;

namespace UnityGUIFramework
{
    class LayoutHelper : IDisposable
    {
        readonly LayoutDirection _direction;

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
