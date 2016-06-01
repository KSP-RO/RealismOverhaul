using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGUIFramework;

namespace EngineGroupController
{
    public class EngineGroupModule : PartModule
    {
        EngineGroupNameUI _gui = null;

        [KSPField(isPersistant = true)] 
        public string EngineGroupId;

        [KSPField(guiName = "Engine Group ID is ", guiActive = true, guiActiveEditor = true)] 
        public string EngineGroupIdStr;

        public IList<EngineWrapper> Wrappers { get; set; }

        [KSPEvent(guiActiveEditor = true, guiName = "Assign Group ID")]
        public void AssignGroupId()
        {
            _gui = EngineGroupNameUI.Show((s, accepted) =>
            {
                if (!accepted)
                    return;
                EngineGroupId = s;
                EngineGroupIdStr = EngineGroupId;
            });
        }

        [KSPEvent(guiActiveEditor = true, guiName = "Clear Group ID")]
        public void ClearGroupId()
        {
            EngineGroupId = string.Empty;
            EngineGroupIdStr = "not set";
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            EngineGroupIdStr = string.IsNullOrEmpty(EngineGroupId) ? "not set" : EngineGroupId;
            if (part == null || part.Modules == null)
            {
                DebugHelper.Debug("part == null || part.Modules == null");
                return;
            }
            var modEnginesFx = part.FindModulesImplementing<ModuleEnginesFX>();
            var modEngines = part.FindModulesImplementing<ModuleEngines>();
            if (modEnginesFx.Any())
            {
                Wrappers = modEnginesFx.Select(x => new EngineWrapper(x)).ToList();
            }
            else if (modEngines.Any())
            {
                Wrappers = modEngines.Select(x => new EngineWrapper(x)).ToList();
            }
            else
            {
                DebugHelper.Warning("No engine module was found!");
            }
        }

        private void OnGUI()
        {
            if (_gui != null && _gui.RenderEnabled)
            {
                _gui.Show();
            }
        }
    }
}
