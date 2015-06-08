// Ported from TweakableEverything by toadicus with permisison.
// Oriignal license follows.

// TweakableEngineFairings, a TweakableEverything module
//
// ShroudToggle.cs
//
// Copyright © 2014, toadicus
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation and/or other
//    materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its contributors may be used
//    to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using KSP;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealismOverhaul
{
    public class ShroudToggle : PartModule
    {
        private static readonly KSPActionParam actionParam = new KSPActionParam(
			KSPActionGroup.None, KSPActionType.Activate);

		private List<ModuleJettison> jettisonModules;

		private Dictionary<string, Transform> jettisonTransforms;
		private Dictionary<string, bool> hasJettisonedTable;

		private AttachNode bottomNode;

		[KSPField(isPersistant = true, guiName = "Fairing", guiActive = false, guiActiveEditor = true)]
		[UI_Toggle(enabledText = "Disabled", disabledText = "Enabled")]
		public bool disableFairing;

		private bool disableState;

		private bool hadAttachedPart;

		private bool hasAttachedPart
		{
			get
			{
				if (this.bottomNode == null)
				{
					return false;
				}
				else
				{
					return this.bottomNode.attachedPart != null;
				}
			}
		}

		public ShroudToggle()
		{
			// Seed disable flag to false to emulate stock behavior
			this.disableFairing = false;
			this.hadAttachedPart = false;

			this.jettisonModules = new List<ModuleJettison>();
			this.jettisonTransforms = new Dictionary<string, Transform>();
			this.hasJettisonedTable = new Dictionary<string, bool>();
		}

		public override void OnStart(StartState state)
		{
			// Start up the base PartModule, just in case.
			base.OnStart(state);

			// Fetch all of the ModuleJettisons from the part, filling a list of modules and a dict of transforms
			PartModule module;
			for (int mIdx = 0; mIdx < base.part.Modules.Count; mIdx++)
			{
				module = base.part.Modules[mIdx];
				if (module is ModuleJettison)
				{
					ModuleJettison jettisonModule = module as ModuleJettison;

					if (jettisonModule == null || jettisonModule.jettisonName == string.Empty)
					{
						Debug.Log("Skipping problematic jettisonModule");
						continue;
					}

					if (this.bottomNode == null)
					{
						this.bottomNode = this.part.findAttachNode(jettisonModule.bottomNodeName);
					}

					this.jettisonModules.Add(jettisonModule);

					this.jettisonTransforms[jettisonModule.jettisonName] = jettisonModule.jettisonTransform;

					// Seed the hasJettisoned table with the module's state at start up to avoid loading up a shroud
					// when we shouldn't have one.
					this.hasJettisonedTable[jettisonModule.jettisonName] = jettisonModule.isJettisoned;

					BaseEvent moduleJettisonEvent = new BaseEvent(
						this.Events,
						string.Format("{0}{1}", jettisonModule.jettisonName, "jettisonEvent"),
						(BaseEventDelegate)delegate
						{
							this.JettisonEvent(jettisonModule, actionParam);
						}
					);

					moduleJettisonEvent.active = true;
					moduleJettisonEvent.guiActive = jettisonModule.isJettisoned && !this.disableFairing;
					moduleJettisonEvent.guiActiveEditor = false;
					moduleJettisonEvent.guiName = "Jettison";

					this.Events.Add(moduleJettisonEvent);

					Debug.Log("Added new Jettison event wrapper " + moduleJettisonEvent);

					jettisonModule.Events["Jettison"].active = false;
					jettisonModule.Events["Jettison"].guiActive = false;
					jettisonModule.Events["Jettison"].guiActiveEditor = false;
					jettisonModule.Events["Jettison"].guiName += "(DEPRECATED)";

					BaseAction moduleJettisonAction = new BaseAction(
						this.Actions,
						string.Format("{0}{1}", jettisonModule.jettisonName, "jettisonAction"),
						(BaseActionDelegate)delegate(KSPActionParam param)
						{
							this.JettisonEvent(jettisonModule, param);
						},
						new KSPAction("Jettison")
					);

					this.Actions.Add(moduleJettisonAction);

					Debug.Log("Added new JettisonAction action wrapper " + moduleJettisonAction);

					jettisonModule.Actions["JettisonAction"].active = false;
					jettisonModule.Actions["JettisonAction"].guiName += "(DEPRECRATED)";
				}
			}

			Debug.Log("Found "+ jettisonModules.Count + " ModuleJettisons.");

			// Seed the disableState for first-run behavior.
			if (this.disableFairing || true)
			{
				this.disableState = !this.disableFairing;
			}
		}

		public void LateUpdate()
		{
			// If nothing has changed...
			if (this.hasAttachedPart == this.hadAttachedPart && this.disableState == this.disableFairing)
			{
				// ...move on with life
				return;
			}
			// Otherwise...

			// ...re-seed the states
			this.disableState = this.disableFairing;
			this.hadAttachedPart = this.hasAttachedPart;

			bool partMayHaveFairing = !this.disableFairing && this.hasAttachedPart;

			Debug.Log("partMayHaveFairing: " + partMayHaveFairing);

			// ...loop through the jettison modules and...
			ModuleJettison jettisonModule;
			for (int jIdx = 0; jIdx < this.jettisonModules.Count; jIdx++)
			{
				jettisonModule = this.jettisonModules[jIdx];

				// ...skip the module if something is wrong with it
				if (jettisonModule == null || jettisonModule.jettisonName == string.Empty)
				{
					Debug.Log("Skipping problematic jettisonModule");
					continue;
				}
				// ...otherwise...
				// ...fetch the transform...
				Transform jettisonTransform = this.jettisonTransforms[jettisonModule.jettisonName];

				// Disable fairings if we know to have loaded with them already jettisoned, but allow them to be 
				// re-enabled in the editor.
				bool moduleShouldHaveFairing =
					partMayHaveFairing &&
					(
						!this.hasJettisonedTable[jettisonModule.jettisonName] ||
						HighLogic.LoadedSceneIsEditor
					);

				Debug.Log("moduleShouldHaveFairing=" + moduleShouldHaveFairing);

				// ...set the module as jettisoned (or not) as appropriate...
				jettisonModule.isJettisoned = !moduleShouldHaveFairing;
				// ...set the module's jettison event as active (or not) as appropriate...
				string jettisonEventName = string.Format("{0}{1}", jettisonModule.jettisonName, "jettisonEvent");
				this.Events[jettisonEventName].guiActive = moduleShouldHaveFairing;

				// ...set the transform's gameObject as active (or not) as appropriate...
				jettisonTransform.gameObject.SetActive(moduleShouldHaveFairing);

				// ...if we should have a fairing...
				if (moduleShouldHaveFairing)
				{
					// ...Restore the transform
					jettisonModule.jettisonTransform = jettisonTransform;
				}
				else
				{
					// ...otherwise, null it
					jettisonModule.jettisonTransform = null;
				}
			}
		}

		public void JettisonEvent(ModuleJettison jettisonModule, KSPActionParam param)
		{
			Debug.Log("JettisonEvent called for " + jettisonModule.jettisonName + " with param=" + param);

			jettisonModule.JettisonAction(param);

			this.hasJettisonedTable[jettisonModule.jettisonName] = true;

			this.Events[string.Format("{0}{1}", jettisonModule.jettisonName, "jettisonEvent")].guiActive = false;
		}
	}
}
