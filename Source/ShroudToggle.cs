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
        private List<ModuleJettison> jettisonModules;

        private Dictionary<string, Transform> jettisonTransforms;

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
                        continue;
                    }

                    if (this.bottomNode == null)
                    {
                        this.bottomNode = this.part.findAttachNode(jettisonModule.bottomNodeName);
                    }

                    this.jettisonModules.Add(jettisonModule);

                    this.jettisonTransforms[jettisonModule.jettisonName] = jettisonModule.jettisonTransform;
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

            bool shouldHaveFairing = !this.disableFairing && this.hasAttachedPart;

            // ...loop through the jettison modules and...
            ModuleJettison jettisonModule;
            for (int jIdx = 0; jIdx < this.jettisonModules.Count; jIdx++)
            {
                jettisonModule = this.jettisonModules[jIdx];

                // ...skip the module if something is wrong with it
                if (jettisonModule == null || jettisonModule.jettisonName == string.Empty)
                {
                    continue;
                }
                // ...otherwise...
                // ...fetch the transform...
                Transform jettisonTransform = this.jettisonTransforms[jettisonModule.jettisonName];

                // ...set the module as jettisoned (or not) as appropriate...
                jettisonModule.isJettisoned = !shouldHaveFairing;
                // ...set the module's jettison event as active (or not) as appropriate...
                jettisonModule.Events["Jettison"].guiActive = !shouldHaveFairing;

                // ...set the transform's gameObject as active (or not) as appropriate...
                jettisonTransform.gameObject.SetActive(shouldHaveFairing);

                // ...if we should have a fairing...
                if (shouldHaveFairing)
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
    }
}

