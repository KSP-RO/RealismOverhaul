// Ported from TweakableEverything by toadicus with permisison.
// Oriignal license follows.

// TweakableEngineFairings, a TweakableEverything module
//
// ModuleTweakableJettison.cs
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
		protected List<ModuleJettison> jettisonModules;

		protected Dictionary<string, Transform> jettisonTransforms;

		protected Dictionary<string, bool> isJettisonedTable;

		[KSPField(isPersistant = true, guiName = "Fairing", guiActive = false, guiActiveEditor = true)]
		[UI_Toggle(enabledText = "Disabled", disabledText = "Enabled")]
		public bool disableFairing;

		protected bool disableState;

		public ShroudToggle()
		{
			// Seed disable flag to false to emulate stock behavior
			this.disableFairing = false;

			this.jettisonModules = new List<ModuleJettison>();
			this.jettisonTransforms = new Dictionary<string, Transform>();
			this.isJettisonedTable = new Dictionary<string, bool>();
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

					if (jettisonModule == null)
					{
						continue;
					}

					this.jettisonModules.Add(jettisonModule);

					if (jettisonModule.jettisonName == null)
					{
						continue;
					}

					this.jettisonTransforms[jettisonModule.jettisonName] = jettisonModule.jettisonTransform;
					this.isJettisonedTable[jettisonModule.jettisonName] = jettisonModule.isJettisoned;
				}
			}

			// Seed the disableState for first-run behavior.
			if (this.disableFairing || true)
			{
				this.disableState = !this.disableFairing;
			}
		}

		public void LateUpdate()
		{
			// If the disableFairing toggle has changed...
			if (this.disableState != this.disableFairing)
			{
				// ...re-seed the disableState
				this.disableState = this.disableFairing;

				// ...loop through the jettison modules and...
				ModuleJettison jettisonModule;
				for (int jIdx = 0; jIdx < this.jettisonModules.Count; jIdx++)
				{
					jettisonModule = this.jettisonModules[jIdx];

					if (jettisonModule.jettisonName == null)
					{
						continue;
					}

					// ...if the jettison module has not already been jettisoned...
					if (!jettisonModule.isJettisoned)
					{
						Transform jettisonTransform;

						// ...fetch the corresponding transform
						jettisonTransform = this.jettisonTransforms[jettisonModule.jettisonName];

						// ...set the transform's activity state
						jettisonTransform.gameObject.SetActive(!this.disableFairing);
						// ...set the module's event visibility
						jettisonModule.Events["Jettison"].guiActive = !this.disableFairing;

						// ...and if the fairing is disabled...
						if (this.disableFairing)
						{
							// ...null the jettison module's transform
							jettisonModule.jettisonTransform = null;
						}
					// ...otherwise, the fairing is enabled...
					else
						{
							// ...return the jettison module's transform
							jettisonModule.jettisonTransform = jettisonTransform;
						}

						jettisonModule.isJettisoned = this.disableFairing |
							this.isJettisonedTable[jettisonModule.jettisonName];
					}
				}
			}
		}
	}
}

