using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityGUIFramework
{
    public class PropertyTypeInitializerContainer
    {
        readonly GUIFramework _framework;

        readonly IDictionary<Type, Func<string, Type, object>> _propertyTypeInitializators = new Dictionary<Type, Func<string, Type, object>>();

        public PropertyTypeInitializerContainer(GUIFramework framework)
        {
            _framework = framework;
        }

        public void AddPropertyTypeInitializator(Type propertyType, Func<string, object> initializer)
        {
            if (!_propertyTypeInitializators.ContainsKey(propertyType))
            {
                _propertyTypeInitializators.Add(propertyType, (s, type) => initializer(s));
            }
            else
            {
                _propertyTypeInitializators[propertyType] =(s, type) => initializer(s);
            }
        }

        public void AddPropertyTypeInitializator(Type propertyType, Func<string, Type, object> initializer)
        {
            if (!_propertyTypeInitializators.ContainsKey(propertyType))
            {
                _propertyTypeInitializators.Add(propertyType, initializer);
            }
            else
            {
                _propertyTypeInitializators[propertyType] = initializer;
            }
        }

        public void AddPropertyTypeInitializator<T>(Func<string, T> initializer)
        {
            AddPropertyTypeInitializator(typeof(T), s => initializer(s));
        }

        public void AddPropertyTypeInitializator<T>(Func<string, Type, T> initializer)
        {
            AddPropertyTypeInitializator(typeof(T), (s, type) => initializer(s, type));
        }

        public Func<string, Type, object> GetInitializer(Type propType, bool checkBaseTypes)
        {
            if (!_propertyTypeInitializators.ContainsKey(propType))
            {
                if (!checkBaseTypes)
                {
                    DebugHelper.Error("Unable to find property type initialier for type {0}", propType);
                    return null;
                }
                var baseType = propType.BaseType;
                while (baseType != null && baseType != typeof (object))
                {
                    _framework.DebugPrint("Checking type {0}", baseType);
                    if (_propertyTypeInitializators.ContainsKey(baseType))
                        return _propertyTypeInitializators[baseType];
                    baseType = baseType.BaseType;
                }
                DebugHelper.Error("Unable to find property type initialier for type {0} or any of it's base types", propType);
                _framework.DebugPrint("Initializers available: {0}",
                    string.Join(", ", _propertyTypeInitializators.Keys.Select(x => x.FullName).ToArray()));
                return null;
            }
            return _propertyTypeInitializators[propType];
        }

        public void AssignProperty(PropertyInfo propertyInfo, string value, object target)
        {
            var initializer = GetInitializer(propertyInfo.PropertyType, true);
            if (initializer == null)
                return;
            var propValue = initializer(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(target, propValue, null);
        }
    }
}