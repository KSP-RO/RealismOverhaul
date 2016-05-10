using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGUIFramework;
using KSP.UI.Screens;

namespace EngineGroupController
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    //[KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class EngineFlightControl : MonoBehaviour
    {
        internal IDictionary<string, EngineGroup> Groups = new Dictionary<string, EngineGroup>();

        private EngineFlightUI _gui;
        private GUIFramework _framework;
        private bool _displayGUI;

        public void Start()
        {
            DebugHelper.Debug("EngineFlightControl:Start");
            _framework = new GUIFramework();
            _gui = new EngineFlightUI("Engine Groups Controller", _framework);
            _gui.IsDisplayDelegate += () => _displayGUI;
            Instance = this;
            OnVesselWasModified(FlightGlobals.ActiveVessel);
            //GameEvents.onVesselChange.Add(OnVesselWasModified);
            GameEvents.onVesselWasModified.Add(OnVesselWasModified);
            _gui.RenderEnabled = true;
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            OnGUIAppLauncherReady();
        }

        private void OnGUIAppLauncherReady()
        {
            if (ApplicationLauncher.Ready && _appLauncherButton == null)
            {
                _appLauncherButton = ApplicationLauncher.Instance.AddModApplication(() => _displayGUI = true,
                    () => _displayGUI = false,
                    () => { },
                    () => { },
                    () => { },
                    () => { },
                    ApplicationLauncher.AppScenes.FLIGHT,
                    GameDatabase.Instance.GetTexture("EngineGroupController/ToolbarIcon4", false)
                    );
            }
        }

        private void OnVesselWasModified(Vessel data)
        {
            if (!ReferenceEquals(data, FlightGlobals.ActiveVessel))
            {
                DebugHelper.Debug("OnVesselWasModified called on non-active vessel, skipping");
                return;
            }
            DebugHelper.Debug("OnVesselWasModified, groups count {0}", Groups.Count);
            //_gui.ClearGroups();
            foreach (var engineGroup in Groups.Values)
            {
                DebugHelper.Debug("Cleaning group {0}: {1} engine(s)", engineGroup.GroupId, engineGroup.EngineRefList.Count);
                engineGroup.EngineRefList.Clear();
            }
            //Groups.Clear();
            RecurseParts(data.rootPart,
                x =>
                {
                    //DebugHelper.Debug("Part {0}", x.name);
                    //x.FindModuleImplementing<>()
                    if (!x.Modules.Contains("EngineGroupModule"))
                    {
                        //DebugHelper.Debug("!x.Modules.Contains('EngineGroupModule')");
                        return;
                    }
                    var moduleRef = x.Modules["EngineGroupModule"] as EngineGroupModule;
                    if (moduleRef == null)
                    {
                        //DebugHelper.Debug("moduleRef == null");
                        return;
                    }
                    var groupId = moduleRef.EngineGroupId;
                    if (string.IsNullOrEmpty(groupId))
                        return;
                    if (!Groups.ContainsKey(groupId))
                    {
                        DebugHelper.Debug("Creating a group {0}", groupId);
                        Groups.Add(groupId, new EngineGroup(groupId));
                        _gui.AddGroup(Groups[groupId]);
                    }
                    foreach (var wrapper in moduleRef.Wrappers)
                    {
                        Groups[groupId].EngineRefList.Add(wrapper);
                    }
                });

            foreach (var groupId in Groups.Keys.ToList())
            {
                if (Groups[groupId].EngineRefList.Count == 0)
                {
                    DebugHelper.Debug("Removing group {0} (no engines)", groupId);
                    _gui.RemoveGroup(Groups[groupId]);
                    Groups.Remove(groupId);
                }
            }
            if (Groups.Count == 0)
            {
                _gui.DisplayNoGroupsMessage();
            }
        }

        private void RecurseParts(Part root, Action<Part> action)
        {
            action(root);
            foreach (var child in root.children)
            {
                RecurseParts(child, action);
            }
        }


        public void OnDestroy()
        {
            DebugHelper.Debug("EngineFlightControl:OnDestroy");
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            ApplicationLauncher.Instance.RemoveModApplication(_appLauncherButton);
            //GameEvents.onVesselChange.Remove(OnVesselWasModified);
            GameEvents.onVesselWasModified.Remove(OnVesselWasModified);
            _gui.RenderEnabled = false;
            Instance = null;
            _gui = null;
            _framework = null;
        }

        public void Update()
        {

        }

        public void OnGUI()
        {
            if (_gui != null && _gui.RenderEnabled)
            {
                _gui.Show();
            }
        }

        public void FixedUpdate()
        {
            //DebugHelper.Debug("EngineFlightControl:FixedUpdate");^
            foreach (var engineGroup in Groups.Values)
            {
                if (!engineGroup.ThrottleChanged)
                    continue;
                foreach (var engine in engineGroup.EngineRefList)
                {
                    engine.Throttle = engineGroup.Throttle;
                }
                //engineGroup.ThrottleChanged = false;
            }
        }

        public static EngineFlightControl Instance;
        private ApplicationLauncherButton _appLauncherButton;
    }
}
