using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityGUIFramework.Controls;

namespace UnityGUIFramework
{
    /// <summary>
    /// This class represents the foundation of the library. Its methods are used to initialize windows controls.
    /// </summary>
    public class GUIFramework
    {
        public const string StandardControlsNamespace = "http://www.asmitech.com/UnityGUIFramework/StandardControls";

        readonly IDictionary<string, string> _namespaces = new Dictionary<string, string>();
        readonly IDictionary<string, Func<GUIControl>> _controlFactories = new Dictionary<string, Func<GUIControl>>();
        readonly PropertyTypeInitializerContainer _propertyTypeInitContainer;
        public bool DisplayDebugMessages;

        /// <summary>
        /// Creates a new instance of object.
        /// </summary>
        public GUIFramework()
        {
            var configNode = GameDatabase.Instance.GetConfigNodes("UnityGUIFrameworkConfig").FirstOrDefault();
            if (configNode != null)
            {
                var debugStr = configNode.GetValue("DebugMode");
                DisplayDebugMessages = debugStr == "1";
            }

            _propertyTypeInitContainer = new PropertyTypeInitializerContainer(this);
            AddControlNamespace(StandardControlsNamespace, typeof(GUIControl).Namespace);

            AddDefaultInitializers();
        }

        void AddDefaultInitializers()
        {
            AddPropertyTypeInitializator(s => s);
            AddPropertyTypeInitializator(int.Parse);
            AddPropertyTypeInitializator(long.Parse);
            AddPropertyTypeInitializator(float.Parse);
            AddPropertyTypeInitializator(double.Parse);
            AddPropertyTypeInitializator(bool.Parse);
            AddPropertyTypeInitializator((s, type) => (Enum) Enum.Parse(type, s));
            AddPropertyTypeInitializator(ColorInitializer);

            AddPropertyTypeInitializator(TexturePropertyTypeInitializer);
            AddPropertyTypeInitializator(ListOfStringsInitializer);
        }

        internal void DebugPrint(string msg, params object[] args)
        {
            if (DisplayDebugMessages)
                DebugHelper.Debug(msg, args);
        }

        static readonly Regex ColorCodeRegex = new Regex("0x([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})",
            RegexOptions.IgnoreCase);

        static float ParseColor(string str)
        {
            var val = byte.Parse(str, NumberStyles.HexNumber);
            const byte maxValue = 0xff;
            return ((float) val) / maxValue;
        }

        static Color ColorInitializer(string text)
        {
            var match = ColorCodeRegex.Match(text);
            if (match.Success)
            {
                var a = ParseColor(match.Groups[1].Value);
                var r = ParseColor(match.Groups[2].Value);
                var g = ParseColor(match.Groups[3].Value);
                var b = ParseColor(match.Groups[4].Value);
                return new Color(r, g, b, a);
            }
            var propInfo = typeof (Color).GetProperty(text, BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (propInfo == null)
            {
                DebugHelper.Error("Invalid color: {0}", text);
                return new Color();
            }
            return (Color)propInfo.GetValue(null, null);
        }

        static IList<string> ListOfStringsInitializer(string input)
        {
            var parts = input.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            return parts.ToList();
        }

        Texture TexturePropertyTypeInitializer(string s)
        {
            //format is protocol://path/to/file.png
            var result = new Texture2D(32, 32);
            if (s.StartsWith ("resource://", StringComparison.Ordinal))
            {
                //resource://Namespace.Type,Assembly;Resourcename.xml
                var parts = s.Replace("resource://", string.Empty).Split(';');
                var resourceName = parts[1];
                var resourceType = parts[0];
                DebugPrint("Loading from resource: type {0}, name {1}", resourceType, resourceName);
                var type = Type.GetType(resourceType);
                DebugPrint("type = {0}, tt = {1}", resourceType, type);
                result.LoadImage(GetResourceData(type, resourceName));
                DebugPrint("image: {0}x{1}", result.width, result.height);
                return result;
            }
            if (s.StartsWith ("file:///", StringComparison.Ordinal))
            {
                s = "file://" + KSPUtil.ApplicationRootPath.Replace("\\", "/") + s.Replace("file:///", string.Empty);
            }
            var www = new WWW(s);
            while (!www.isDone)
                Thread.Sleep(50);
            www.LoadImageIntoTexture(result);
            DebugPrint("image: {0}x{1}", result.width, result.height);
            return result;
        }

        /// <summary>
        /// This methods registers custom controls namespace mapping between xml namespace used in markup and controls' namespace, or replaces existing one.
        /// </summary>
        /// <param name="xmlNamespace">Xml namespace</param>
        /// <param name="controlsNamespace">Controls namespace</param>
        public void AddControlNamespace(string xmlNamespace, string controlsNamespace)
        {
            if (!_namespaces.ContainsKey(xmlNamespace))
            {
                _namespaces.Add(xmlNamespace, controlsNamespace);
            }
        }

        #region Property Type Initializer Delegations

        /// <summary>
        /// Registers initializer for new property type, or replaces existing one.
        /// </summary>
        /// <param name="propertyType">Property type</param>
        /// <param name="initializer">Initializer delegate</param>
        public void AddPropertyTypeInitializator(Type propertyType, Func<string, object> initializer)
        {
            _propertyTypeInitContainer.AddPropertyTypeInitializator(propertyType, initializer);
        }

        /// <summary>
        /// Registers initializer for new property type.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="initializer">Initializer delegate</param>
        public void AddPropertyTypeInitializator<T>(Func<string, T> initializer)
        {
            _propertyTypeInitContainer.AddPropertyTypeInitializator(initializer);
        }

        /// <summary>
        /// Registers initializer for new property type, or replaces existing one.
        /// </summary>
        /// <param name="propertyType">Property type</param>
        /// <param name="initializer">Initializer delegate</param>
        public void AddPropertyTypeInitializator(Type propertyType, Func<string, Type, object> initializer)
        {
            _propertyTypeInitContainer.AddPropertyTypeInitializator(propertyType, initializer);
        }

        /// <summary>
        /// Registers initializer for new property type, or replaces existing one.
        /// </summary>
        /// <typeparam name="T">Ptoperty type</typeparam>
        /// <param name="initializer">Initializer delegate</param>
        public void AddPropertyTypeInitializator<T>(Func<string, Type, T> initializer)
        {
            _propertyTypeInitContainer.AddPropertyTypeInitializator(initializer);
        }

        #endregion


        string GetResource(Type type, string resourceName)
        {
            DebugPrint("Loading resource {0}, {1}", type, resourceName);
            using (var stream = type.Assembly.GetManifestResourceStream(type, resourceName))
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        return sr.ReadToEnd();
                    }
                }
                DebugHelper.Error("Loading resource {0}, {1}", type, resourceName);
                return string.Empty;
            }
        }


        byte[] GetResourceData(Type type, string resourceName)
        {
            DebugPrint("Loading resource data {0}, {1}", type, resourceName);
            using (var stream = type.Assembly.GetManifestResourceStream(type, resourceName))
            {
                if (stream == null)
                {
                    DebugHelper.Error("Unable to load resource data {0}, {1}", type, resourceName);
                    return null;
                }
                var result = new byte[stream.Length];
                stream.Read(result, 0, result.Length);
                return result;
            }
        }

        /// <summary>
        /// Creates new window object from resource.
        /// </summary>
        /// <param name="windowId">Unique window id</param>
        /// <param name="title">Window title</param>
        /// <param name="type">Type to use as a namespace for resource</param>
        /// <param name="resourceName">Resource Name</param>
        /// <returns></returns>
        public GUIWindow CreateWindowFromResource(int windowId, string title, Type type, string resourceName)
        {
            var text = GetResource(type, resourceName);
            return CreateWindow(windowId, title, text);
        }

        /// <summary>
        /// Creates new window object from passed-in markup.
        /// </summary>
        /// <param name="windowId">Unique window id</param>
        /// <param name="title">Window title</param>
        /// <param name="layoutMarkup">Markup text</param>
        /// <returns></returns>
        public GUIWindow CreateWindow(int windowId, string title, string layoutMarkup)
        {
            var xmlDoc = new XmlDocument();
            foreach (var key in _namespaces.Keys)
            {
                xmlDoc.NameTable.Add(key);
            }
            xmlDoc.LoadXml(layoutMarkup);
            var windowNodes = xmlDoc.GetElementsByTagName("Window");
            if (windowNodes.Count != 1)
            {
                DebugHelper.Error("Invalid markup, missing Window element.");
                return null;
            }
            var winNode = (XmlElement) windowNodes[0];
            var window = new GUIWindow(windowId, title);
            AssignProperties(winNode.Attributes, window);
            CreateChildControls(window.Children, winNode.ChildNodes);
            return window;
        }

        internal void InitWindow(GUIWindow window, Type type, string resourceName)
        {
            var text = GetResource(type, resourceName);
            InitWindow(window, text);
        }

        internal void InitWindow(GUIWindow window, string layoutMarkup)
        {
            var xmlDoc = new XmlDocument();
            foreach (var key in _namespaces.Keys)
            {
                xmlDoc.NameTable.Add(key);
            }
            xmlDoc.LoadXml(layoutMarkup);
            var windowNodes = xmlDoc.GetElementsByTagName("Window");
            var winNode = (XmlElement)windowNodes[0];
            AssignProperties(winNode.Attributes, window);
            CreateChildControls(window.Children, winNode.ChildNodes);
        }

        void CreateChildControls(ICollection<GUIControl> controlsCollection, XmlNodeList nodeList)
        {
            foreach (XmlElement childNode in nodeList)
            {
                var obj = CreateControl(childNode);
                if (obj == null)
                    continue;
                controlsCollection.Add(obj);
                var composite = obj as GUICompositeControl;
                if (composite != null)
                {
                    CreateChildControls(composite.Children, childNode.ChildNodes);
                }
            }
        }

        Func<GUIControl> GetFactory(string fullControlName)
        {
            if (!_controlFactories.ContainsKey(fullControlName))
            {
                DebugPrint("Building control factory for type {0}", fullControlName);
                var targetType = Type.GetType(fullControlName);
                if (targetType == null)
                {
                    DebugHelper.Error("Control type not found: {0}", fullControlName);
                    _controlFactories.Add(fullControlName, null);
                    return null;
                }
                var targetCtor = targetType.GetConstructor(Type.EmptyTypes);
                var method = new DynamicMethod("Init" + fullControlName.Replace(".", string.Empty), typeof(GUIControl), Type.EmptyTypes);
                var generator = method.GetILGenerator();
                generator.Emit(OpCodes.Newobj, targetCtor);
                generator.Emit(OpCodes.Ret);
                var factory = (Func<GUIControl>)method.CreateDelegate(typeof(Func<GUIControl>));
                _controlFactories.Add(fullControlName, factory);
                DebugPrint("Built control factory for type {0}", fullControlName);
            }
            return _controlFactories[fullControlName];
        }

        internal void AssignProperties(XmlAttributeCollection attributes, object targetObj)
        {
            var targetType = targetObj.GetType();
            foreach (XmlAttribute attribute in attributes)
            {
                var propInfo = targetType.GetProperty(attribute.LocalName);
                if (propInfo == null)
                {
                    if (!_namespaces.Keys.Contains(attribute.Value))
                    {
                        DebugHelper.Warning("Property {0} not found.", attribute.LocalName);
                    }
                    continue;
                }
                DebugPrint("Applying attribute {0}='{1}' to property {2} of type {3}", 
                    attribute.LocalName, attribute.Value,
                    propInfo.Name,
                    propInfo.PropertyType);
                _propertyTypeInitContainer.AssignProperty(propInfo, attribute.Value, targetObj);
            }
        }

        GUIControl CreateControl(XmlElement node)
        {
            DebugPrint("Attempting to init control {0}/{1}...", node.NamespaceURI, node.LocalName);
            if (!_namespaces.ContainsKey(node.NamespaceURI))
            {
                DebugHelper.Error("Unknown control namespace: {0}", node.NamespaceURI);
                return null;
            }
            var ctrlNs = _namespaces[node.NamespaceURI];
            var fullControlName = string.Format("{0}.{1}Control", ctrlNs, node.LocalName);
            DebugPrint("Control type name {0}", fullControlName);
            var factoryMethod = GetFactory(fullControlName);
            if (factoryMethod == null)
                return null;
            //var targetType = Type.GetType(fullControlName);
            var obj = factoryMethod();
            AssignProperties(node.Attributes, obj);
            return obj;
        }
    }
}
