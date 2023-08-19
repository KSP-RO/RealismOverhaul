using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RealismOverhaul
{
    public class ModuleROEjectionSeat : PartModule, IPartMassModifier, IPartCostModifier
    {
        public const string GroupName = "ROEjectionSeat";
        public const string GroupDisplayName = "Ejection Seat";

        private const string GameObjName = "ModuleROEjectionSeat";
        private const string JetpackPartName = "evaJetpack";

        [KSPField(isPersistant = true, guiActiveEditor = true, guiName = "Ejection seat(s)", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_Toggle(disabledText = "<color=red><b>Disabled</b></color>", enabledText = "<color=green>Enabled</color>", scene = UI_Scene.Editor)]
        public bool IsEnabled = true;

        [KSPField(isPersistant = true, guiActive = true, guiName = "Has fired", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public bool hasFired = false;

        [KSPField(guiName = "colliderSize", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public Vector3 colliderSize = new Vector3(0.2f, 0.2f, 0.2f);

        /// <summary>
        /// Offset of the collider on which the kerbal will spawn. Is calculated from the transform of the parent part.
        /// </summary>
        [KSPField(guiName = "colliderOffset", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public Vector3 colliderOffset = new Vector3(0, 0.4f, -1);

        [KSPField(guiName = "colliderRotAngles", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public Vector3 colliderRotAngles = new Vector3(-90, 0, 0);

        [KSPField(guiName = "forceDir", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public Vector3 forceDir = new Vector3(0, 0.15f, -1);

        [KSPField(guiFormat = "F2", guiName = "chuteMinPressureOverride", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float chuteMinPressureOverride = 0.01f;

        [KSPField(guiFormat = "F0", guiUnits = "m", guiName = "chuteDeployAltitudeOverride", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float chuteDeployAltitudeOverride = 1000f;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F0", guiUnits = "m/s", guiName = "Max ejection speed", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float maxEjectSpeed = 330f;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F0", guiUnits = "m", guiName = "Max ejection altitude", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float maxEjectAltitude = 15000f;

        [KSPField(guiActiveEditor = true, guiFormat = "F0", guiName = "Added cost", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float cost = 0f;

        [KSPField(guiActiveEditor = true, guiFormat = "F2", guiUnits = "t", guiName = "Added mass", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float mass = 0f;

#if !DEBUG
        // There are also debug copies of those fields to make them editable in PAW

        /// <summary>
        /// Ejection force measured in kN.
        /// </summary>
        [KSPField(guiFormat = "F1", guiUnits = "kN", guiName = "ejectionForce", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float ejectionForce = 27f;

        /// <summary>
        /// Time in seconds over which the ejection forces are applied.
        /// </summary>
        [KSPField(guiFormat = "F2", guiUnits = "s", guiName = "ejectionForceDuration", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float ejectionForceDuration = 0.125f;

        /// <summary>
        /// Delay between ejection and chute predeployment being armed.
        /// </summary>
        [KSPField(guiFormat = "F1", guiUnits = "s", guiName = "Chute deployment delay", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        public float chuteDelay = 0.8f;
#endif

        private List<SeatConfig> _seats;

        public override string GetModuleDisplayName() => "Ejection seat";

        public override string GetInfo()
        {
            return $"Max ejection speed: {maxEjectSpeed:N0} m/s\n" +
                   $"Max ejection altitude: {maxEjectAltitude:N0} m";
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            if (!PartLoader.Instance.IsReady())
            {
                _seats = new List<SeatConfig>();
                foreach (ConfigNode n in node.GetNodes("SEAT"))
                {
                    var seatConfig = new SeatConfig();
                    ConfigNode.LoadObjectFromConfig(seatConfig, n);
                    _seats.Add(seatConfig);
                }

                if (_seats.Count == 0)
                {
                    _seats.Add(new SeatConfig());    // fallback with default values
                }
            }
        }

        public override void OnStart(StartState state)
        {
            if (PartLoader.Instance.IsReady())
            {
                _seats = part.partInfo.partPrefab.FindModuleImplementing<ModuleROEjectionSeat>()._seats;
            }

            Fields[nameof(IsEnabled)].uiControlEditor.onFieldChanged = OnEnabledChanged;
            OnEnabledChanged(Fields[nameof(IsEnabled)], IsEnabled);

#if DEBUG
            SeatConfig seat = _seats[0];
            colliderOffsetX = seat.colliderOffset.x;
            colliderOffsetY = seat.colliderOffset.y;
            colliderOffsetZ = seat.colliderOffset.z;

            rotateAngleX = seat.colliderRotAngles.x;
            rotateAngleY = seat.colliderRotAngles.y;
            rotateAngleZ = seat.colliderRotAngles.z;

            forceDirX = seat.forceDir.x;
            forceDirY = seat.forceDir.y;
            forceDirZ = seat.forceDir.z;
#endif
        }

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit) => IsEnabled ? mass : 0;

        public ModifierChangeWhen GetModuleMassChangeWhen() => ModifierChangeWhen.FIXED;

        public float GetModuleCost(float defaultCost, ModifierStagingSituation sit) => IsEnabled ? cost : 0;

        public ModifierChangeWhen GetModuleCostChangeWhen() => ModifierChangeWhen.FIXED;

        [KSPAction("Eject!", KSPActionGroup.Abort)]
        public void Eject(KSPActionParam param) => Eject();

        [KSPEvent(guiName = "Eject!", guiActive = true, guiActiveEditor = false, groupName = GroupName)]
        public virtual void Eject()
        {
            if (!IsEnabled) return;

            if (hasFired)
            {
                Debug.Log("[ROEjectionSeat] Ejection seat has already fired");
                return;
            }

            if (part.protoModuleCrew.Count == 0)
            {
                Debug.Log("[ROEjectionSeat] No crew to eject");
                return;
            }

            if (!CheckSafeToEject()) return;

            ProcessInventory();

#if DEBUG
            if (_dbgCollider != null)
                _dbgCollider.gameObject.DestroyGameObjectImmediate();
#endif

            StartCoroutine(DoStaggeredEjectionRoutine());
        }

        private IEnumerator DoStaggeredEjectionRoutine()
        {
            int curCrewIdx = 0, failCount = 0;
            while (part.protoModuleCrew.Count > 0 && failCount < part.protoModuleCrew.Count)
            {
                ProtoCrewMember crew = part.protoModuleCrew[failCount];    // Do not attempt to re-process crewmembers whose ejection failed
                SeatConfig curSeat = _seats[curCrewIdx % _seats.Count];
                bool wasEjected = EjectCrewmember(crew, curSeat);
                if (!wasEjected)
                    failCount++;
                curCrewIdx++;

                if (part.protoModuleCrew.Count > 0)
                {
                    SeatConfig nextSeat = _seats[curCrewIdx % _seats.Count];
                    yield return new WaitForSeconds(nextSeat.ejectDelay);
                }
            }
        }

        private bool EjectCrewmember(ProtoCrewMember crew, SeatConfig seat)
        {
            var go = new GameObject(GameObjName);
            go.layer = 21;
            var collider = go.AddComponent<BoxCollider>();
            Transform partTransform = part.gameObject.transform;
            collider.transform.SetParent(partTransform, false);
            collider.transform.rotation = partTransform.rotation;
            collider.size = colliderSize;
            collider.transform.position += partTransform.rotation * seat.colliderOffset;
            collider.transform.Rotate(seat.colliderRotAngles);

            var landedBef = vessel.Landed;
            var splashedBef = vessel.Splashed;
            var sitBef = vessel.situation;
            vessel.Landed = true;    // Force the vessel to be landed to bypass some weird frame skips in KSP code

            KerbalEVA eva;
            try
            {
                eva = FlightEVA.fetch.spawnEVA(crew, part, collider.transform);
            }
            finally
            {
                vessel.situation = sitBef;
                vessel.Landed = landedBef;
                vessel.Splashed = splashedBef;
            }

            if (eva == null)
            {
                Debug.Log("[ROEjectionSeat] It appears that nobody was EVA'd");
                return false;
            }

            eva.autoGrabLadderOnStart = false;

            hasFired = true;
            var forceVector = partTransform.rotation * seat.forceDir.normalized;
            go.DestroyGameObjectImmediate();

            var handler = CrewEjectionHandler.CreateInstance(eva);
            handler.EjectionForce = ejectionForce;
            handler.EjectionForceDuration = ejectionForceDuration;
            handler.ChuteMinPressureOverride = chuteMinPressureOverride;
            handler.ChuteDeployAltitudeOverride = chuteDeployAltitudeOverride;
            handler.ChuteDelay = chuteDelay;
            handler.ForceDirection = forceVector;
            handler.StartProcessing();

            return true;
        }

        private bool CheckSafeToEject()
        {
            bool isAllowed = true;
            if (vessel.srfSpeed > maxEjectSpeed)
            {
                Debug.Log("[ROEjectionSeat] CheckSafeToEject: over speed limit");
                ScreenMessages.PostScreenMessage($"Ejection not possible, vessel speed exceeds the limit of {maxEjectSpeed:F0} m/s", 10f, ScreenMessageStyle.UPPER_CENTER, XKCDColors.Red);
                isAllowed = false;
            }

            if (vessel.altitude > maxEjectAltitude)
            {
                Debug.Log("[ROEjectionSeat] CheckSafeToEject: over altitude limit");
                ScreenMessages.PostScreenMessage($"Ejection not possible, vessel altitude exceeds the limit of {maxEjectAltitude:F0} m", 10f, ScreenMessageStyle.UPPER_CENTER, XKCDColors.Red);
                isAllowed = false;
            }

            return isAllowed;
        }

        private void ProcessInventory()
        {
            foreach (ProtoCrewMember pcm in part.protoModuleCrew)
            {
                ModuleInventoryPart nautInv = pcm.KerbalInventoryModule;
                int jetpackIdx = -1;
                StoredPart jetpackPart = null;
                for (int i = 0; i < nautInv.InventorySlots; i++)
                {
                    if (nautInv.storedParts.TryGetValue(i, out StoredPart val) && val.partName == JetpackPartName)
                    {
                        Debug.Log($"[ROEjectionSeat] Naut {pcm.name} has EVA jetpack in inventory which should get moved to parent part");
                        jetpackIdx = i;
                        jetpackPart = val;
                        break;
                    }
                }

                if (jetpackIdx > -1)
                {
                    Debug.Log($"[ROEjectionSeat] Removing EVA jetpack from naut slot {jetpackIdx}");
                    nautInv.ClearPartAtSlot(jetpackIdx);

                    TryAddJetpackToParentPartInventory(jetpackPart);
                }
            }
        }

        private void TryAddJetpackToParentPartInventory(StoredPart jetpackPart)
        {
            ModuleInventoryPart partInv = part.FindModuleImplementing<ModuleInventoryPart>();
            if (partInv == null)
            {
                Debug.Log($"[ROEjectionSeat] Part {part.name} does not have ModuleInventoryPart");
                return;
            }

            int slotIdx = partInv.FirstEmptySlot();
            if (slotIdx < 0)
            {
                Debug.Log($"[ROEjectionSeat] Part {part.name} does not have inventory space to add EVA jetpack");
                return;
            }

            Debug.Log($"[ROEjectionSeat] Adding EVA jetpack to part slot {slotIdx}");
            partInv.StoreCargoPartAtSlot(jetpackPart.snapshot, slotIdx);
        }

        private void OnEnabledChanged(BaseField field, object obj)
        {
            Fields[nameof(maxEjectSpeed)].guiActiveEditor = Fields[nameof(maxEjectSpeed)].guiActive = IsEnabled;
            Fields[nameof(maxEjectAltitude)].guiActiveEditor = Fields[nameof(maxEjectAltitude)].guiActive = IsEnabled;
            Fields[nameof(cost)].guiActiveEditor = IsEnabled;
            Fields[nameof(mass)].guiActiveEditor = IsEnabled;
            Fields[nameof(hasFired)].guiActive = IsEnabled;
            Events[nameof(Eject)].guiActive = IsEnabled;
        }

#if DEBUG
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiFormat = "F1", guiUnits = "kN", guiName = "ejectionForce", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = 1f, maxValue = 200, stepIncrement = 1f)]
        public float ejectionForce = 27f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiFormat = "F2", guiUnits = "s", guiName = "ejectionForceDuration", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = 0.01f, maxValue = 0.5f, stepIncrement = 0.01f)]
        public float ejectionForceDuration = 0.125f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiFormat = "F1", guiUnits = "s", guiName = "Chute deployment delay", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = 0.1f, maxValue = 5f, stepIncrement = 0.1f)]
        public float chuteDelay = 0.8f;

        [KSPField(guiActive = true, guiName = "IsHit", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_Label]
        public bool isHit = false;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Debug", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_Toggle]
        public bool isDebug = true;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F0", guiName = "rotateAngleX", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -180, maxValue = 180, stepIncrement = 1f)]
        public float rotateAngleX = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F0", guiName = "rotateAngleY", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -180, maxValue = 180, stepIncrement = 1f)]
        public float rotateAngleY = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F0", guiName = "rotateAngleZ", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -180, maxValue = 180, stepIncrement = 1f)]
        public float rotateAngleZ = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F2", guiName = "forceDirX", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -1, maxValue = 1, stepIncrement = 0.01f)]
        public float forceDirX = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F2", guiName = "forceDirY", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -1, maxValue = 1, stepIncrement = 0.01f)]
        public float forceDirY = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F2", guiName = "forceDirZ", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -1, maxValue = 1, stepIncrement = 0.01f)]
        public float forceDirZ = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F2", guiName = "colliderOffsetX", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -5, maxValue = 5, stepIncrement = 0.05f)]
        public float colliderOffsetX = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F2", guiName = "colliderOffsetY", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -5, maxValue = 5, stepIncrement = 0.05f)]
        public float colliderOffsetY = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F2", guiName = "colliderOffsetZ", groupName = GroupName, groupDisplayName = GroupDisplayName)]
        [UI_FloatRange(minValue = -5, maxValue = 5, stepIncrement = 0.05f)]
        public float colliderOffsetZ = 0;

        private BoxCollider _dbgCollider;

        public void Update()
        {
            if (!isDebug || !IsEnabled)
            {
                if (_dbgCollider != null) _dbgCollider.gameObject.DestroyGameObjectImmediate();
                return;
            }

            if (hasFired) return;

            Transform partTransform = part.gameObject.transform;
            if (_dbgCollider == null)
            {
                var go = new GameObject(GameObjName);
                go.layer = 21;
                _dbgCollider = go.AddComponent<BoxCollider>();
                _dbgCollider.transform.SetParent(partTransform, false);
            }
            _dbgCollider.transform.localPosition = Vector3.zero;
            _dbgCollider.size = colliderSize;

            colliderOffset = new Vector3(colliderOffsetX, colliderOffsetY, colliderOffsetZ);
            _dbgCollider.transform.position += partTransform.rotation * colliderOffset;

            _dbgCollider.transform.rotation = partTransform.rotation;
            colliderRotAngles = new Vector3(rotateAngleX, rotateAngleY, rotateAngleZ);
            _dbgCollider.transform.Rotate(colliderRotAngles);

            DebugDrawer.DebugTransforms(_dbgCollider.transform);
            DrawTools.DrawLocalCube(_dbgCollider.transform, _dbgCollider.size, Color.magenta, _dbgCollider.center);
            DebugDrawer.DebugTransforms(partTransform);

            forceDir = new Vector3(forceDirX, forceDirY, forceDirZ);
            Vector3 forceVector = partTransform.rotation * forceDir;
            DebugDrawer.DebugLine(_dbgCollider.transform.position, _dbgCollider.transform.position + forceVector, Color.magenta);

            Vector3 rayOrigin = _dbgCollider.transform.transform.position - 0.5f * (_dbgCollider.transform.position - part.transform.position).normalized;
            Vector3 rayDir = (_dbgCollider.transform.position - part.transform.position).normalized;
            DebugDrawer.DebugPoint(rayOrigin, Color.gray);
            DebugDrawer.DebugLine(rayOrigin, rayOrigin + rayDir, Color.cyan);
            int layer = LayerUtil.DefaultEquivalent | 0x8000 | 0x80000 | 0x4000000;
            isHit = Physics.Raycast(rayOrigin, rayDir,
                out var hitInfo, part.hatchObstructionCheckOutwardDistance + 0.5f,
                layer, QueryTriggerInteraction.Ignore);
            if (isHit)
            {
                DebugDrawer.DebugPoint(hitInfo.point, Color.black);
            }
            bool b = FlightEVA.hatchInsideFairing(part);
            isHit |= b;

            SeatConfig seat = _seats[0];
            seat.colliderOffset.x = colliderOffsetX;
            seat.colliderOffset.y = colliderOffsetY;
            seat.colliderOffset.z = colliderOffsetZ;

            seat.colliderRotAngles.x = rotateAngleX;
            seat.colliderRotAngles.y = rotateAngleY;
            seat.colliderRotAngles.z = rotateAngleZ;

            seat.forceDir.x = forceDirX;
            seat.forceDir.y = forceDirY;
            seat.forceDir.z = forceDirZ;
        }
#endif
    }
}
