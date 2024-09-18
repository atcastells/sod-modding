using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200035E RID: 862
public class NewDoor : MonoBehaviour
{
	// Token: 0x06001354 RID: 4948 RVA: 0x00113A48 File Offset: 0x00111C48
	public void Setup(NewWall newParent)
	{
		this.wall = newParent;
		this.doorPairPreset = newParent.preset;
		this.GetPreset();
		if (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew)
		{
			string seedInput = base.transform.position.ToString();
			this.wall.SetDoorStrengthBase(Toolbox.Instance.GetPsuedoRandomNumberContained(this.preset.doorStrengthRange.x, this.preset.doorStrengthRange.y, seedInput, out seedInput));
			this.wall.SetLockStrengthBase(Toolbox.Instance.GetPsuedoRandomNumberContained(this.preset.lockStrengthRange.x, this.preset.lockStrengthRange.y, seedInput, out seedInput));
		}
		this.doorOpenSpeed = this.preset.doorOpenSpeed;
		this.openAngle = this.preset.openAngle;
		this.doorSetting = (NewDoor.DoorSetting)Mathf.Max((int)this.wall.node.room.roomType.doorSetting, (int)this.wall.otherWall.node.room.roomType.doorSetting);
		this.SetOpen(this.ajar, null, true, 1f);
		this.SetKnowLockedStatus(false);
		if (!CityData.Instance.doorDictionary.ContainsKey(this.wall.id))
		{
			CityData.Instance.doorDictionary.Add(this.wall.id, this);
		}
		else
		{
			CityData.Instance.doorDictionary[this.wall.id] = this;
		}
		if (!CityData.Instance.doorDictionary.ContainsKey(this.wall.otherWall.id))
		{
			CityData.Instance.doorDictionary.Add(this.wall.otherWall.id, this);
		}
		else
		{
			CityData.Instance.doorDictionary[this.wall.otherWall.id] = this;
		}
		this.ParentToRoom(newParent.node.room);
		this.doorInteractable = InteractableCreator.Instance.CreateTransformInteractable(this.preset.objectPreset, base.transform, null, newParent.node.room.gameLocation.evidenceEntry, Vector3.zero, Vector3.zero, null);
		this.doorInteractable.SetPolymorphicReference(this);
		this.wall.otherWall.node.AddInteractable(this.doorInteractable);
		if (this.preset.handlePreset != null)
		{
			this.handleInteractable = InteractableCreator.Instance.CreateTransformInteractable(this.preset.handlePreset, base.transform, null, newParent.node.room.gameLocation.evidenceEntry, Vector3.zero, Vector3.zero, null);
			this.handleInteractable.SetPolymorphicReference(this);
			this.wall.otherWall.node.AddInteractable(this.handleInteractable);
		}
		if (this.preset.canPeakUnderneath)
		{
			this.peekInteractable = InteractableCreator.Instance.CreateTransformInteractable(PrefabControls.Instance.peekInteractable, base.transform, null, newParent.node.room.gameLocation.evidenceEntry, Vector3.zero, Vector3.zero, null);
			this.peekInteractable.SetPolymorphicReference(this);
			this.wall.otherWall.node.AddInteractable(this.peekInteractable);
			this.peekInteractable.SetSwitchState(!this.isClosed, null, true, false, false);
			if (this.preset.lockType == DoorPreset.LockType.key)
			{
				this.peekInteractable.SetCustomState1(this.knowLockStatus, null, true, false, false);
			}
			else
			{
				this.peekInteractable.SetCustomState1(false, null, true, false, false);
			}
			if (this.preset.lockType != DoorPreset.LockType.none)
			{
				this.peekInteractable.SetLockedState(this.isLocked, null, true, false);
			}
			else
			{
				this.peekInteractable.SetLockedState(false, null, true, false);
			}
		}
		this.doorInteractable.SetSwitchState(!this.isClosed, null, false, false, false);
		if (this.handleInteractable != null)
		{
			this.handleInteractable.SetSwitchState(!this.isClosed, null, false, false, false);
		}
		if (this.preset.lockType == DoorPreset.LockType.key)
		{
			this.doorInteractable.SetCustomState1(this.knowLockStatus, null, false, false, false);
			if (this.handleInteractable != null)
			{
				this.handleInteractable.SetCustomState1(this.knowLockStatus, null, false, false, false);
			}
		}
		else
		{
			this.doorInteractable.SetCustomState1(false, null, false, false, false);
			if (this.handleInteractable != null)
			{
				this.handleInteractable.SetCustomState1(false, null, false, false, false);
			}
		}
		if (this.preset.lockType != DoorPreset.LockType.none)
		{
			this.doorInteractable.SetLockedState(this.isLocked, null, false, false);
			if (this.handleInteractable != null)
			{
				this.handleInteractable.SetLockedState(this.isLocked, null, false, false);
			}
		}
		else
		{
			this.doorInteractable.SetLockedState(false, null, false, false);
			if (this.handleInteractable != null)
			{
				this.handleInteractable.SetLockedState(false, null, false, false);
			}
		}
		if (Player.Instance.keyring.Contains(this))
		{
			this.doorInteractable.SetCustomState3(true, null, false, false, false);
			if (this.handleInteractable != null)
			{
				this.handleInteractable.SetCustomState3(true, null, false, false, false);
			}
			if (this.peekInteractable != null)
			{
				this.peekInteractable.SetCustomState3(true, null, false, false, false);
			}
		}
		else
		{
			this.doorInteractable.SetCustomState3(false, null, false, false, false);
			if (this.handleInteractable != null)
			{
				this.handleInteractable.SetCustomState3(false, null, false, false, false);
			}
			if (this.peekInteractable != null)
			{
				this.peekInteractable.SetCustomState3(false, null, false, false, false);
			}
		}
		if (this.GetDefaultLockState())
		{
			this.lockSetting = NewDoor.LockSetting.keepLocked;
			this.SetLocked(true, null, false);
		}
		else
		{
			this.lockSetting = NewDoor.LockSetting.keepUnlocked;
			this.SetLocked(false, null, false);
		}
		this.bothNodesForAudioSource.Add(this.wall.node);
		this.bothNodesForAudioSource.Add(this.wall.otherWall.node);
		this.doorInteractable.actionAudioEventOverrides.Add(RoutineControls.Instance.openDoor, this.preset.audioOpen);
		this.doorInteractable.actionAudioEventOverrides.Add(RoutineControls.Instance.closeDoor, this.preset.audioClose);
		this.doorInteractable.actionAudioEventOverrides.Add(RoutineControls.Instance.knockOnDoor, this.preset.audioKnockLight);
		if (this.handleInteractable != null)
		{
			this.handleInteractable.actionAudioEventOverrides.Add(RoutineControls.Instance.openDoor, this.preset.audioOpen);
			this.handleInteractable.actionAudioEventOverrides.Add(RoutineControls.Instance.closeDoor, this.preset.audioClose);
		}
		if (this.preset.lockType == DoorPreset.LockType.keypad && this.preset.lockInteractable != null)
		{
			this.lockInteractableFront = InteractableCreator.Instance.CreateTransformInteractable(this.preset.lockInteractable, base.transform, null, null, Vector3.zero, Vector3.zero, null);
			this.lockInteractableFront.thisDoor = this.doorInteractable;
			this.lockInteractableFront.SetLockedState(this.isLocked, null, true, false);
			this.lockInteractableRear = InteractableCreator.Instance.CreateTransformInteractable(this.preset.lockInteractable, base.transform, null, null, Vector3.zero, Vector3.zero, null);
			this.lockInteractableRear.thisDoor = this.doorInteractable;
			this.lockInteractableRear.SetLockedState(this.isLocked, null, true, false);
			if (this.passwordDoorsRoom.preset.preferredPassword == RoomConfiguration.RoomPasswordPreference.interactableBelongsTo && this.passwordDoorsRoom.belongsTo != null && this.passwordDoorsRoom.belongsTo.Count > 0)
			{
				this.lockInteractableFront.SetPasswordSource(this.passwordDoorsRoom.belongsTo[0]);
				this.lockInteractableRear.SetPasswordSource(this.passwordDoorsRoom.belongsTo[0]);
				return;
			}
			if (this.passwordDoorsRoom.preset.preferredPassword == RoomConfiguration.RoomPasswordPreference.thisRoom)
			{
				this.lockInteractableFront.SetPasswordSource(this.passwordDoorsRoom);
				this.lockInteractableRear.SetPasswordSource(this.passwordDoorsRoom);
				return;
			}
			if (this.passwordDoorsRoom.preset.preferredPassword == RoomConfiguration.RoomPasswordPreference.thisAddress)
			{
				this.lockInteractableFront.SetPasswordSource(this.passwordDoorsRoom.gameLocation);
				this.lockInteractableRear.SetPasswordSource(this.passwordDoorsRoom.gameLocation);
			}
		}
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x0011429C File Offset: 0x0011249C
	public void PlaceKeys()
	{
		if (this.preset.lockType == DoorPreset.LockType.key && CityConstructor.Instance != null && CityConstructor.Instance.generateNew)
		{
			if (this.passwordDoorsRoom == null)
			{
				Game.Log("Misc Error: No room for door " + this.wall.id.ToString() + ". This could mean a null room has a door with a lock. Cannot place door key...", 2);
				return;
			}
			if (this.passwordDoorsRoom.preset.placeKey.Count > 0)
			{
				List<NewGameLocation> list = new List<NewGameLocation>();
				List<Human> list2 = new List<Human>();
				foreach (RoomConfiguration.KeyPlacement keyPlacement in this.passwordDoorsRoom.preset.placeKey)
				{
					if (keyPlacement == RoomConfiguration.KeyPlacement.thisAddress)
					{
						if (this.passwordDoorsRoom.gameLocation.thisAsAddress.owners.Count > 0 && !this.passwordDoorsRoom.gameLocation.placedKey)
						{
							Human human = this.passwordDoorsRoom.gameLocation.thisAsAddress.owners[Toolbox.Instance.SeedRand(0, this.passwordDoorsRoom.gameLocation.thisAsAddress.owners.Count)];
							list.Add(this.passwordDoorsRoom.gameLocation);
							list2.Add(human);
						}
					}
					else if (keyPlacement == RoomConfiguration.KeyPlacement.belongsToHome || keyPlacement == RoomConfiguration.KeyPlacement.belongsToWork)
					{
						foreach (Human human2 in this.passwordDoorsRoom.belongsTo)
						{
							if (keyPlacement == RoomConfiguration.KeyPlacement.belongsToHome && human2.home != null)
							{
								list.Add(human2.home);
								list2.Add(human2);
							}
							else if (keyPlacement == RoomConfiguration.KeyPlacement.belongsToWork && human2.job != null && human2.job.employer != null)
							{
								list.Add(human2.job.employer.address);
								list2.Add(human2);
							}
						}
					}
				}
				if (list.Count > 0)
				{
					int num = Toolbox.Instance.SeedRand(0, list.Count);
					NewGameLocation newGameLocation = list[num];
					FurnitureLocation furnitureLocation;
					newGameLocation.PlaceObject(InteriorControls.Instance.key, list2[num], list2[num], null, out furnitureLocation, true, Interactable.PassedVarType.roomID, this.passwordDoorsRoom.roomID, true, 1, this.passwordDoorsRoom.preset.keyOwnershipPlacement, 1, null, false, null, null, null, "", false);
					newGameLocation.placedKey = true;
				}
			}
		}
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x00114560 File Offset: 0x00112760
	private void GetPreset()
	{
		if (this.wall.node.room.preset == null || this.wall.otherWall.node.room.preset == null)
		{
			this.preset = InteriorControls.Instance.defaultDoor;
			return;
		}
		if (this.wall.node.room.preset.passwordPriority > this.wall.otherWall.node.room.preset.passwordPriority)
		{
			this.passwordDoorsRoom = this.wall.node.room;
		}
		else
		{
			this.passwordDoorsRoom = this.wall.otherWall.node.room;
		}
		if (this.wall.node.gameLocation.thisAsAddress == null || this.wall.otherWall.node.gameLocation.thisAsAddress == null)
		{
			if (this.wall.node.room.preset.exteriorDoor != null && this.wall.otherWall.node.room.preset.exteriorDoor != null)
			{
				if (this.wall.node.room.roomType.doorPriority >= this.wall.otherWall.node.room.roomType.doorPriority)
				{
					this.preset = this.wall.node.room.preset.exteriorDoor;
					return;
				}
				this.preset = this.wall.otherWall.node.room.preset.exteriorDoor;
				return;
			}
			else
			{
				if (this.wall.node.room.preset.exteriorDoor != null)
				{
					this.preset = this.wall.node.room.preset.exteriorDoor;
					return;
				}
				if (this.wall.otherWall.node.room.preset.exteriorDoor != null)
				{
					this.preset = this.wall.otherWall.node.room.preset.exteriorDoor;
					return;
				}
			}
		}
		if (this.wall.node.gameLocation != this.wall.otherWall.node.gameLocation)
		{
			if (this.wall.node.room.preset.addressDoor != null && this.wall.otherWall.node.room.preset.addressDoor != null)
			{
				if (this.wall.node.room.roomType.doorPriority >= this.wall.otherWall.node.room.roomType.doorPriority)
				{
					this.preset = this.wall.node.room.preset.addressDoor;
					return;
				}
				this.preset = this.wall.otherWall.node.room.preset.addressDoor;
				return;
			}
			else
			{
				if (this.wall.node.room.preset.addressDoor != null)
				{
					this.preset = this.wall.node.room.preset.addressDoor;
					return;
				}
				if (this.wall.otherWall.node.room.preset.addressDoor != null)
				{
					this.preset = this.wall.otherWall.node.room.preset.addressDoor;
					return;
				}
			}
		}
		if (this.wall.node.room.preset.internalDoor != null && this.wall.otherWall.node.room.preset.internalDoor != null)
		{
			if (this.wall.node.room.roomType.doorPriority >= this.wall.otherWall.node.room.roomType.doorPriority)
			{
				this.preset = this.wall.node.room.preset.internalDoor;
				return;
			}
			this.preset = this.wall.otherWall.node.room.preset.internalDoor;
			return;
		}
		else
		{
			if (this.wall.node.room.preset.internalDoor != null)
			{
				this.preset = this.wall.node.room.preset.internalDoor;
				return;
			}
			if (this.wall.otherWall.node.room.preset.internalDoor != null)
			{
				this.preset = this.wall.otherWall.node.room.preset.internalDoor;
				return;
			}
			if (this.preset == null)
			{
				this.preset = InteriorControls.Instance.defaultDoor;
			}
			return;
		}
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x00114AE0 File Offset: 0x00112CE0
	public void SelectColouring(bool overrideWithKey = false, Toolbox.MaterialKey keyOverride = null)
	{
		if (this.preset.inheritColouringFromDecor && this.preset.variations.Count > 0 && !this.wall.foundDoorMaterialKey)
		{
			this.wall.foundDoorMaterialKey = false;
			if (this.preset.shareColours != FurniturePreset.ShareColours.none)
			{
				NewDoor newDoor = null;
				foreach (KeyValuePair<int, NewFloor> keyValuePair in this.wall.node.room.building.floors)
				{
					foreach (NewAddress newAddress in keyValuePair.Value.addresses)
					{
						foreach (NewRoom newRoom in newAddress.rooms)
						{
							foreach (NewNode newNode in newRoom.nodes)
							{
								foreach (NewWall newWall in newNode.walls)
								{
									if (newWall.door != null && newWall != newWall.otherWall && newWall.foundDoorMaterialKey && newWall.door.preset.shareColours == this.preset.shareColours)
									{
										newDoor = newWall.door;
										this.wall.doorMatKey = newDoor.wall.doorMatKey;
										this.wall.otherWall.doorMatKey = newDoor.wall.doorMatKey;
										this.wall.foundDoorMaterialKey = true;
										this.wall.otherWall.foundDoorMaterialKey = true;
										break;
									}
								}
								if (newDoor != null)
								{
									break;
								}
							}
							if (newDoor != null)
							{
								break;
							}
						}
						if (newDoor != null)
						{
							break;
						}
					}
					if (newDoor != null)
					{
						break;
					}
				}
			}
			if (!this.wall.foundDoorMaterialKey)
			{
				ColourSchemePreset colourScheme = this.wall.node.room.colourScheme;
				if (colourScheme == null && this.wall.node.room.building != null)
				{
					colourScheme = this.wall.node.room.building.colourScheme;
				}
				this.wall.doorMatKey = MaterialsController.Instance.GenerateMaterialKey(this.preset.variations[Toolbox.Instance.GetPsuedoRandomNumber(0, this.preset.variations.Count, this.wall.node.nodeCoord.ToString(), false)], colourScheme, this.wall.node.room, true, null);
				this.wall.otherWall.doorMatKey = this.wall.doorMatKey;
				this.wall.foundDoorMaterialKey = true;
				this.wall.otherWall.foundDoorMaterialKey = true;
			}
		}
		Toolbox.MaterialKey materialKey = this.wall.doorMatKey;
		if (overrideWithKey && keyOverride != null)
		{
			materialKey = keyOverride;
		}
		if (this.spawnedDoor == null && this.wall.foundDoorMaterialKey && materialKey != null)
		{
			MaterialsController.Instance.ApplyMaterialKey(this.spawnedDoor, materialKey);
		}
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x00114F04 File Offset: 0x00113104
	public void SpawnDoor()
	{
		if (this.spawnedDoor != null)
		{
			return;
		}
		this.spawnedDoor = Object.Instantiate<GameObject>(this.preset.doorModel, base.transform);
		this.spawnedDoor.transform.localPosition = this.parentedWall.preset.doorOffset;
		this.spawnedDoorColliders = Enumerable.ToList<Collider>(this.spawnedDoor.GetComponentsInChildren<Collider>(true));
		this.doorInteractableController = this.spawnedDoor.GetComponent<InteractableController>();
		this.doorInteractableController.isDoor = this;
		this.doorInteractableController.Setup(this.doorInteractable);
		this.spawnedDoor.transform.localRotation = Quaternion.Euler(0f, this.desiredAngle, 0f);
		if (this.mapDoorObject != null)
		{
			this.mapDoorObject.localEulerAngles = new Vector3(0f, 0f, -this.desiredAngle);
		}
		if (this.wall.foundDoorMaterialKey)
		{
			MaterialsController.Instance.ApplyMaterialKey(this.spawnedDoor, this.wall.doorMatKey);
		}
		if (this.handleInteractable != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.preset.handleModel, this.spawnedDoor.transform);
			this.doorHandleInteractableController = gameObject.GetComponent<InteractableController>();
			this.doorHandleInteractableController.isDoor = this;
			this.doorHandleInteractableController.Setup(this.handleInteractable);
		}
		if (this.doorSignFront == null && this.preset.doorSigns.Count > 0)
		{
			List<GameObject> list = new List<GameObject>();
			NewNode behindNode = this.GetBehindNode();
			NewNode infontNode = this.GetInfontNode();
			if (behindNode != null)
			{
				foreach (DoorPreset.DoorSign doorSign in this.preset.doorSigns)
				{
					if (doorSign != null)
					{
						try
						{
							if (doorSign.ifEntranceToRoom.Contains(behindNode.room.preset) && (!doorSign.placeIfFromOutside || infontNode.isOutside) && (!doorSign.placeIfFromInside || !infontNode.isOutside) && (!doorSign.placeIfFromPublicArea || infontNode.room.preset.forbidden == RoomConfiguration.Forbidden.alwaysAllowed || infontNode.room.preset.forbidden == RoomConfiguration.Forbidden.allowedDuringOpenHours) && (!doorSign.onlyPlaceIfInhabited || (infontNode.room.gameLocation.thisAsAddress != null && infontNode.room.gameLocation.thisAsAddress.inhabitants.Count > 0)))
							{
								list.AddRange(doorSign.signagePool);
							}
						}
						catch
						{
						}
					}
				}
			}
			if (list.Count > 0)
			{
				GameObject gameObject2 = list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, CityData.Instance.seed + this.preset.name, false)];
				this.doorSignFront = Object.Instantiate<GameObject>(gameObject2, this.spawnedDoor.transform);
				this.doorSignFront.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
				this.doorSignFront.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				this.doorSignFront.transform.localPosition = this.preset.doorSignOffset;
				OpenSignController component = this.doorSignFront.GetComponent<OpenSignController>();
				if (component != null)
				{
					if (this.doorInteractableController.switchSyncObjects != null)
					{
						this.doorInteractableController.switchSyncObjects.Add(component);
					}
					this.doorInteractableController.enableSwitchSync = true;
					this.doorInteractableController.UpdateSwitchSync();
					this.featuresNeonSign = true;
				}
				ApartmentNumberController component2 = this.doorSignFront.GetComponent<ApartmentNumberController>();
				if (component2 != null)
				{
					component2.Setup(behindNode, this);
				}
			}
		}
		if (this.doorSignRear == null && this.preset.doorSigns.Count > 0)
		{
			List<GameObject> list2 = new List<GameObject>();
			NewNode infontNode2 = this.GetInfontNode();
			NewNode behindNode2 = this.GetBehindNode();
			if (infontNode2 != null)
			{
				foreach (DoorPreset.DoorSign doorSign2 in this.preset.doorSigns)
				{
					if (doorSign2 != null)
					{
						try
						{
							if (doorSign2.ifEntranceToRoom.Contains(infontNode2.room.preset) && (!doorSign2.placeIfFromOutside || behindNode2.isOutside) && (!doorSign2.placeIfFromInside || !behindNode2.isOutside) && (!doorSign2.placeIfFromPublicArea || behindNode2.room.preset.forbidden == RoomConfiguration.Forbidden.alwaysAllowed || behindNode2.room.preset.forbidden == RoomConfiguration.Forbidden.allowedDuringOpenHours))
							{
								list2.AddRange(doorSign2.signagePool);
							}
						}
						catch
						{
						}
					}
				}
			}
			if (list2.Count > 0)
			{
				GameObject gameObject3 = list2[Toolbox.Instance.GetPsuedoRandomNumber(0, list2.Count, CityData.Instance.seed + this.preset.name, false)];
				this.doorSignRear = Object.Instantiate<GameObject>(gameObject3, this.spawnedDoor.transform);
				this.doorSignRear.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				this.doorSignRear.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
				this.doorSignRear.transform.localPosition = new Vector3(this.preset.doorSignOffset.x, this.preset.doorSignOffset.y, -this.preset.doorSignOffset.z - 0.1f);
				OpenSignController component3 = this.doorSignRear.GetComponent<OpenSignController>();
				if (component3 != null)
				{
					if (this.doorInteractableController.switchSyncObjects != null)
					{
						this.doorInteractableController.switchSyncObjects.Add(component3);
					}
					this.doorInteractableController.enableSwitchSync = true;
					this.doorInteractableController.UpdateSwitchSync();
					this.featuresNeonSign = true;
				}
				ApartmentNumberController component4 = this.doorSignRear.GetComponent<ApartmentNumberController>();
				if (component4 != null)
				{
					component4.Setup(infontNode2, this);
				}
			}
		}
		if (this.preset.canPeakUnderneath)
		{
			GameObject gameObject4 = Object.Instantiate<GameObject>(PrefabControls.Instance.peekUnderDoor, this.spawnedDoor.transform);
			this.peekInteractableController = gameObject4.GetComponent<InteractableController>();
			this.peekInteractableController.isDoor = this;
			this.peekInteractableController.Setup(this.peekInteractable);
		}
		if (this.lockInteractableFront != null)
		{
			this.lockInteractableFront.spawnedObject = Toolbox.Instance.SpawnObject(this.lockInteractableFront.preset.prefab, this.spawnedDoor.transform);
			this.lockInteractableFront.spawnedObject.transform.localPosition = this.preset.lockOffsetFront;
			this.lockInteractableFront.spawnedObject.GetComponent<InteractableController>().Setup(this.lockInteractableFront);
			if (this.lockInteractableFront.preset.useMaterialChanges)
			{
				if (this.lockInteractableFront.locked && this.lockInteractableFront.preset.lockOnMaterial != null)
				{
					this.lockInteractableFront.spawnedObject.GetComponent<MeshRenderer>().material = this.lockInteractableFront.preset.lockOnMaterial;
				}
				else if (!this.lockInteractableFront.locked && this.lockInteractableFront.preset.lockOffMaterial != null)
				{
					this.lockInteractableFront.spawnedObject.GetComponent<MeshRenderer>().material = this.lockInteractableFront.preset.lockOffMaterial;
				}
			}
		}
		if (this.lockInteractableRear != null)
		{
			this.lockInteractableRear.spawnedObject = Toolbox.Instance.SpawnObject(this.lockInteractableRear.preset.prefab, this.spawnedDoor.transform);
			this.lockInteractableRear.spawnedObject.transform.localPosition = this.preset.lockOffsetRear;
			this.lockInteractableRear.spawnedObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			this.lockInteractableRear.spawnedObject.GetComponent<InteractableController>().Setup(this.lockInteractableRear);
			if (this.lockInteractableRear.preset.useMaterialChanges)
			{
				if (this.lockInteractableRear.locked && this.lockInteractableRear.preset.lockOnMaterial != null)
				{
					this.lockInteractableRear.spawnedObject.GetComponent<MeshRenderer>().material = this.lockInteractableRear.preset.lockOnMaterial;
				}
				else if (!this.lockInteractableRear.locked && this.lockInteractableRear.preset.lockOffMaterial != null)
				{
					this.lockInteractableRear.spawnedObject.GetComponent<MeshRenderer>().material = this.lockInteractableRear.preset.lockOffMaterial;
				}
			}
		}
		uint num = 0U;
		if (this.wall.node.gameLocation.thisAsStreet != null || this.wall.otherWall.node.gameLocation.thisAsStreet != null)
		{
			num = 2U;
		}
		foreach (MeshRenderer meshRenderer in this.spawnedDoor.GetComponentsInChildren<MeshRenderer>())
		{
			if (this.wall.node.building != null)
			{
				if (this.wall.node.building.interiorLightCullingLayer == 0)
				{
					meshRenderer.renderingLayerMask = 5U + num;
				}
				else if (this.wall.node.building.interiorLightCullingLayer == 1)
				{
					meshRenderer.renderingLayerMask = 9U + num;
				}
				else if (this.wall.node.building.interiorLightCullingLayer == 2)
				{
					meshRenderer.renderingLayerMask = 17U + num;
				}
				else if (this.wall.node.building.interiorLightCullingLayer == 3)
				{
					meshRenderer.renderingLayerMask = 33U + num;
				}
			}
		}
		this.SetKnowLockedStatus(this.knowLockStatus);
		this.doorInteractable.name = base.name;
		if (this.forbiddenForPublic)
		{
			this.SpawnPoliceTape();
		}
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x0011598C File Offset: 0x00113B8C
	public void UpdateNameBasedOnPlayerPosition()
	{
		this.playerOtherSideRoom = this.wall.node.room;
		if (this.playerOtherSideRoom == Player.Instance.currentRoom)
		{
			this.playerOtherSideRoom = this.wall.otherWall.node.room;
		}
		string text;
		this.otherSideIsTrespassing = Player.Instance.IsTrespassing(this.playerOtherSideRoom, out this.otherSideTrespassingEscalation, out text, false);
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x00115A00 File Offset: 0x00113C00
	private NewNode GetBehindNode()
	{
		Vector3 coords = base.transform.TransformPoint(-0.1f, 0f, 1f);
		NewNode result = null;
		PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(coords), ref result);
		return result;
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x00115A50 File Offset: 0x00113C50
	private NewNode GetInfontNode()
	{
		Vector3 coords = base.transform.TransformPoint(-0.1f, 0f, -1f);
		NewNode result = null;
		PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(coords), ref result);
		return result;
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x00115A9D File Offset: 0x00113C9D
	public string GetNameForParent()
	{
		return this.wall.otherWall.node.gameLocation.name;
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x00115ABC File Offset: 0x00113CBC
	public string GetName()
	{
		string empty = string.Empty;
		if (this.wall.node.gameLocation.thisAsStreet != null || this.wall.node.gameLocation.isLobby || this.wall.otherWall.node.gameLocation.thisAsStreet != null || this.wall.otherWall.node.gameLocation.isLobby)
		{
			return this.passwordDoorsRoom.gameLocation.name;
		}
		return this.passwordDoorsRoom.name;
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x00115B60 File Offset: 0x00113D60
	public void ParentToRoom(NewRoom newRoom)
	{
		if (this.playerRoom != newRoom)
		{
			this.playerRoom = newRoom;
			if (this.wall.parentWall.node.room == newRoom)
			{
				base.transform.SetParent(this.wall.parentWall.node.room.transform, true);
				this.parentedWall = this.wall.parentWall;
				return;
			}
			if (this.wall.childWall.node.room == newRoom)
			{
				base.transform.SetParent(this.wall.childWall.node.room.transform, true);
				this.parentedWall = this.wall.childWall;
			}
		}
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x00115C30 File Offset: 0x00113E30
	public void SetKnowLockedStatus(bool val)
	{
		this.knowLockStatus = val;
		if (this.doorInteractable != null)
		{
			this.doorInteractable.SetCustomState1(this.knowLockStatus, null, true, false, false);
		}
		if (this.handleInteractable != null)
		{
			this.handleInteractable.SetCustomState1(this.knowLockStatus, null, true, false, false);
		}
		if (this.peekInteractable != null)
		{
			this.peekInteractable.SetCustomState1(this.knowLockStatus, null, true, false, false);
		}
		if ((this.doorInteractable != null && InteractionController.Instance.currentLookingAtInteractable == this.doorInteractable.controller) || (this.handleInteractable != null && InteractionController.Instance.currentLookingAtInteractable == this.handleInteractable.controller))
		{
			InteractionController.Instance.DisplayInteractionCursor(InteractionController.Instance.displayingInteraction, true);
		}
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x00115CF8 File Offset: 0x00113EF8
	public void SetPlayerHasKey(bool val)
	{
		if (this.preset.lockType == DoorPreset.LockType.none)
		{
			val = false;
		}
		if (this.doorInteractable != null)
		{
			this.doorInteractable.SetCustomState3(val, null, true, false, false);
		}
		if (this.handleInteractable != null)
		{
			this.handleInteractable.SetCustomState3(val, null, true, false, false);
		}
		if (this.peekInteractable != null)
		{
			this.peekInteractable.SetCustomState3(val, null, true, false, false);
		}
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x00115D60 File Offset: 0x00113F60
	public void OpenByActor(Actor actor, bool forceInverseOpenDirection = false, float speedMultiplier = 1f)
	{
		if ((!this.isClosed && !this.isClosing) || (this.animating && !this.isClosing))
		{
			return;
		}
		if (!base.isActiveAndEnabled)
		{
			this.SetOpen(1f, actor, false, speedMultiplier);
			return;
		}
		if (base.transform.InverseTransformPoint(actor.transform.position).z > 0f)
		{
			if (!forceInverseOpenDirection)
			{
				this.SetOpen(-1f, actor, false, speedMultiplier);
				return;
			}
			this.SetOpen(1f, actor, false, speedMultiplier);
			return;
		}
		else
		{
			if (!forceInverseOpenDirection)
			{
				this.SetOpen(1f, actor, false, speedMultiplier);
				return;
			}
			this.SetOpen(-1f, actor, false, speedMultiplier);
			return;
		}
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x00115E08 File Offset: 0x00114008
	public void SetOpen(float newAjar, Actor actor, bool skipAnimation = false, float speedMultiplier = 1f)
	{
		newAjar = (float)Mathf.RoundToInt(newAjar * 100f) / 100f;
		newAjar = Mathf.Clamp(newAjar, -1f, 1f);
		if (this.ajar == newAjar)
		{
			return;
		}
		this.ajar = newAjar;
		this.desiredAngle = Mathf.Lerp(-this.openAngle, this.openAngle, (this.ajar + 1f) / 2f);
		if (this.ajar != (float)Mathf.Abs(0))
		{
			this.OnOpen();
		}
		if (skipAnimation || !base.gameObject.activeInHierarchy || this.spawnedDoor == null)
		{
			this.ajarProgress = this.ajar;
			if (this.spawnedDoor != null)
			{
				this.spawnedDoor.transform.localRotation = Quaternion.Euler(0f, this.desiredAngle, 0f);
			}
			if (base.gameObject.activeInHierarchy && this.mapDoorObject != null)
			{
				this.mapDoorObject.localEulerAngles = new Vector3(0f, 0f, -this.desiredAngle);
			}
			if (this.ajar == (float)Mathf.Abs(0))
			{
				if (this.parentedWall != null && actor != null)
				{
					AudioController.Instance.PlayWorldOneShot(this.preset.audioClose, actor as Human, this.parentedWall.node, base.transform.position, null, null, 1f, this.bothNodesForAudioSource, false, null, false);
				}
				this.OnClose(actor);
			}
		}
		else
		{
			if (actor != null && actor.ai != null && actor.animationController != null)
			{
				actor.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.armsOneShotUse);
			}
			base.StopAllCoroutines();
			base.StartCoroutine(this.OpenDoor(actor, speedMultiplier));
			if (base.gameObject.activeInHierarchy && (this.wall.node.gameLocation.isOutside || this.wall.otherWall.node.gameLocation.isOutside))
			{
				AudioController.Instance.UpdateClosestWindowAndDoor(false);
			}
		}
		if (this.parentedWall != null && actor != null)
		{
			if (this.ajar == 0f)
			{
				AudioController.Instance.PlayWorldOneShot(this.preset.audioCloseAction, actor, this.parentedWall.node, base.transform.position, null, null, 1f, this.bothNodesForAudioSource, false, null, false);
			}
			else
			{
				AudioController.Instance.PlayWorldOneShot(this.preset.audioOpen, actor, this.parentedWall.node, base.transform.position, null, null, 1f, this.bothNodesForAudioSource, false, null, false);
			}
		}
		if (this.doorInteractable != null)
		{
			this.doorInteractable.SetSwitchState(!this.isClosed, actor, !skipAnimation, false, false);
		}
		if (this.handleInteractable != null)
		{
			this.handleInteractable.SetSwitchState(!this.isClosed, actor, !skipAnimation, false, false);
		}
		if (this.peekInteractable != null)
		{
			this.peekInteractable.SetSwitchState(!this.isClosed, actor, !skipAnimation, false, false);
		}
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x00116134 File Offset: 0x00114334
	private void OnEnable()
	{
		if (this.mapDoorObject != null && this.spawnedDoor != null)
		{
			this.mapDoorObject.localEulerAngles = new Vector3(0f, 0f, -this.spawnedDoor.transform.localEulerAngles.y);
		}
	}

	// Token: 0x06001364 RID: 4964 RVA: 0x00116190 File Offset: 0x00114390
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.animating = false;
		if (this.spawnedDoor != null)
		{
			if (this.spawnedDoor.transform.localEulerAngles.y != this.desiredAngle && this.parentedWall != null)
			{
				AudioController.Instance.PlayWorldOneShot(this.preset.audioClose, null, this.parentedWall.node, base.transform.position, null, null, 1f, this.bothNodesForAudioSource, false, null, false);
			}
			this.spawnedDoor.transform.localRotation = Quaternion.Euler(0f, this.desiredAngle, 0f);
			if (this.mapDoorObject != null)
			{
				this.mapDoorObject.localEulerAngles = new Vector3(0f, 0f, -this.desiredAngle);
			}
			if (!SessionData.Instance.isFloorEdit && this.ajar == (float)Mathf.Abs(0))
			{
				this.OnClose(null);
			}
		}
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x00116293 File Offset: 0x00114493
	private IEnumerator OpenDoor(Actor actor, float speedMultiplier)
	{
		float angle = Mathf.Round(this.spawnedDoor.transform.localEulerAngles.y * 100f) / 100f;
		angle = ((angle > 180f) ? (angle - 360f) : angle);
		float amountToRotate = 0f;
		float doorSpeedMultiplier = speedMultiplier;
		if (actor != null && actor.stealthMode && this.ajar == 0f)
		{
			doorSpeedMultiplier = 0.75f;
		}
		this.animating = true;
		if (this.ajar == (float)Mathf.Abs(0))
		{
			this.isClosing = true;
		}
		else
		{
			this.isClosing = false;
		}
		int audioUpdateTicker = 0;
		this.SetCollisionsWithPlayerActive(false);
		bool closeSFXPlayed = false;
		while (this.desiredAngle != angle)
		{
			float num = Mathf.Abs(this.desiredAngle - angle);
			this.ajarProgress = Mathf.InverseLerp(-this.openAngle, this.openAngle, angle) * 2f - 1f;
			if (angle < this.desiredAngle)
			{
				amountToRotate = Mathf.Min((this.doorOpenSpeed * 20f + num * this.doorOpenSpeed * doorSpeedMultiplier) * Time.deltaTime * SessionData.Instance.currentTimeMultiplier, num);
			}
			else if (angle > this.desiredAngle)
			{
				amountToRotate = -Mathf.Min((this.doorOpenSpeed * 20f + num * this.doorOpenSpeed * doorSpeedMultiplier) * Time.deltaTime * SessionData.Instance.currentTimeMultiplier, num);
			}
			this.spawnedDoor.transform.localEulerAngles = new Vector3(0f, Mathf.Clamp(angle + amountToRotate, -this.openAngle, this.openAngle), 0f);
			angle = Mathf.Round(this.spawnedDoor.transform.localEulerAngles.y * 100f) / 100f;
			angle = ((angle > 180f) ? (angle - 360f) : angle);
			if (this.mapDoorObject != null && this.mapDoorObject.gameObject.activeInHierarchy)
			{
				this.mapDoorObject.localEulerAngles = new Vector3(0f, 0f, -this.spawnedDoor.transform.localEulerAngles.y);
			}
			int num2 = audioUpdateTicker;
			audioUpdateTicker = num2 + 1;
			if (audioUpdateTicker >= 2)
			{
				AudioController.Instance.UpdateClosestWindowAndDoor(true);
				audioUpdateTicker = 0;
			}
			if (!closeSFXPlayed && this.desiredAngle == 0f && Mathf.Abs(this.ajarProgress) <= AudioDebugging.Instance.doorCloseTriggerPoint)
			{
				if (this.parentedWall != null)
				{
					if (actor != null)
					{
						AudioController.Instance.PlayWorldOneShot(this.preset.audioClose, actor as Human, this.parentedWall.node, base.transform.position, null, null, 1f, this.bothNodesForAudioSource, false, null, false);
					}
					else
					{
						AudioController.Instance.PlayWorldOneShot(this.preset.audioClose, null, this.parentedWall.node, base.transform.position, null, null, 1f, this.bothNodesForAudioSource, false, null, false);
					}
				}
				closeSFXPlayed = true;
			}
			if (num <= 0.001f)
			{
				this.spawnedDoor.transform.localEulerAngles = new Vector3(0f, this.desiredAngle, 0f);
				break;
			}
			yield return null;
		}
		this.animating = false;
		this.isClosing = false;
		if (this.ajar == (float)Mathf.Abs(0))
		{
			this.OnClose(actor);
		}
		yield break;
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x001162B0 File Offset: 0x001144B0
	public void OnClose(Actor actor)
	{
		this.isClosed = true;
		this.SetCollisionsWithPlayerActive(true);
		if (this.doorInteractable != null)
		{
			this.doorInteractable.SetSwitchState(!this.isClosed, actor, true, false, false);
		}
		if (this.handleInteractable != null)
		{
			this.handleInteractable.SetSwitchState(!this.isClosed, actor, true, false, false);
		}
		if (this.peekInteractable != null)
		{
			this.peekInteractable.SetSwitchState(!this.isClosed, actor, true, false, false);
		}
		if (!this.isLocked && this.preset.armLockOnClose)
		{
			if (this.lockInteractableFront != null)
			{
				InteractablePreset.InteractionAction action = this.lockInteractableFront.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.effectSwitchStates.Exists((InteractablePreset.SwitchState item2) => item2.switchState == InteractablePreset.Switch.lockState && item2.boolIs));
				this.lockInteractableFront.OnInteraction(action, null, true, 0f);
			}
			if (this.lockInteractableRear != null)
			{
				InteractablePreset.InteractionAction action2 = this.lockInteractableRear.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.effectSwitchStates.Exists((InteractablePreset.SwitchState item2) => item2.switchState == InteractablePreset.Switch.lockState && item2.boolIs));
				this.lockInteractableRear.OnInteraction(action2, null, true, 0f);
			}
		}
		this.wall.node.room.openDoors.Remove(this);
		this.wall.otherWall.node.room.openDoors.Remove(this);
		this.wall.node.room.closedDoors.Add(this);
		this.wall.otherWall.node.room.closedDoors.Add(this);
		if (Player.Instance.currentRoom != null && (Player.Instance.currentRoom.doorCheckSet.Contains(this.wall.id) || Player.Instance.currentRoom.doorCheckSet.Contains(this.wall.otherWall.id)) && (!Game.Instance.enableNewRealtimeTimeCullingSystem || GeometryCullingController.Instance.doorsBlockSight))
		{
			Player.Instance.UpdateCullingOnEndOfFrame();
		}
		if (Player.Instance.currentCityTile == this.wall.node.tile.cityTile)
		{
			AudioController.Instance.UpdateAmbientZonesOnEndOfFrame();
		}
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x00116510 File Offset: 0x00114710
	public void SetCollisionsWithPlayerActive(bool val)
	{
		if (val)
		{
			using (List<Collider>.Enumerator enumerator = this.spawnedDoorColliders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Collider collider = enumerator.Current;
					collider.gameObject.layer = 0;
				}
				return;
			}
		}
		foreach (Collider collider2 in this.spawnedDoorColliders)
		{
			collider2.gameObject.layer = 6;
		}
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x001165B0 File Offset: 0x001147B0
	public void OnOpen()
	{
		this.isClosed = false;
		this.doorInteractable.SetSwitchState(!this.isClosed, null, true, false, false);
		if (this.handleInteractable != null)
		{
			this.handleInteractable.SetSwitchState(!this.isClosed, null, true, false, false);
		}
		if (this.peekInteractable != null)
		{
			this.peekInteractable.SetSwitchState(!this.isClosed, null, true, false, false);
		}
		this.wall.node.room.openDoors.Add(this);
		this.wall.otherWall.node.room.openDoors.Add(this);
		this.wall.node.room.closedDoors.Remove(this);
		this.wall.otherWall.node.room.closedDoors.Remove(this);
		this.SetCollisionsWithPlayerActive(true);
		if (Player.Instance.currentRoom != null && (Player.Instance.currentRoom.doorCheckSet.Contains(this.wall.id) || Player.Instance.currentRoom.doorCheckSet.Contains(this.wall.otherWall.id)) && (!Game.Instance.enableNewRealtimeTimeCullingSystem || GeometryCullingController.Instance.doorsBlockSight))
		{
			Player.Instance.UpdateCullingOnEndOfFrame();
		}
		if (Player.Instance.currentCityTile == this.wall.node.tile.cityTile)
		{
			AudioController.Instance.UpdateAmbientZonesOnEndOfFrame();
		}
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x00116748 File Offset: 0x00114948
	public void SetLocked(bool val, Actor actor, bool playSound = true)
	{
		if (this.preset.lockType == DoorPreset.LockType.none || this.wall.currentDoorStrength <= 0f || this.forbiddenForPublic)
		{
			if (!this.isLocked)
			{
				return;
			}
			val = false;
		}
		if (this.isLocked != val)
		{
			this.isLocked = val;
			if (!this.isLocked)
			{
				if (actor != null && actor.isPlayer)
				{
					Game.Log("Doors: Set door unlocked by player", 2);
				}
			}
			else
			{
				this.wall.ResetLockStrength();
			}
			if (this.isLocked)
			{
				if (playSound)
				{
					AudioController.Instance.PlayWorldOneShot(this.preset.audioLock, actor, this.parentedWall.node, base.transform.position, null, null, 1f, this.bothNodesForAudioSource, false, null, false);
				}
			}
			else if (playSound)
			{
				AudioController.Instance.PlayWorldOneShot(this.preset.audioUnlock, actor, this.parentedWall.node, base.transform.position, null, null, 1f, this.bothNodesForAudioSource, false, null, false);
			}
			if (this.isLocked && this.ajar != 0f)
			{
				this.SetOpen(0f, actor, false, 1f);
			}
		}
		if (this.doorInteractable != null)
		{
			this.doorInteractable.SetLockedState(this.isLocked, actor, true, false);
		}
		if (this.handleInteractable != null)
		{
			this.handleInteractable.SetLockedState(this.isLocked, actor, true, false);
		}
		if (this.peekInteractable != null)
		{
			this.peekInteractable.SetLockedState(this.isLocked, actor, true, false);
		}
		if (this.lockInteractableFront != null)
		{
			this.lockInteractableFront.SetLockedState(this.isLocked, actor, true, false);
		}
		if (this.lockInteractableRear != null)
		{
			this.lockInteractableRear.SetLockedState(this.isLocked, actor, true, false);
		}
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x0011690C File Offset: 0x00114B0C
	public void SetJammed(bool val, Interactable doorWedgeUsed = null, bool createUsedWedge = true)
	{
		if (this.isJammed != val)
		{
			this.isJammed = val;
			if (this.isJammed && doorWedgeUsed != null)
			{
				if (this.doorWedge != null)
				{
					this.doorWedge.Delete();
				}
				this.doorWedge = doorWedgeUsed;
				return;
			}
			if (!this.isJammed && this.doorWedge != null)
			{
				Game.Log("Removing door wedge...", 2);
				if (this.doorWedge.controller != null && createUsedWedge)
				{
					InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.doorWedgeUsed, Player.Instance, null, null, this.doorWedge.wPos, this.doorWedge.wEuler, null, null, "").ForcePhysicsActive(false, true, this.doorWedge.controller.transform.up * 1.25f, 2, false);
				}
				this.doorWedge.Delete();
			}
		}
	}

	// Token: 0x0600136B RID: 4971 RVA: 0x001169F4 File Offset: 0x00114BF4
	public void SetForbidden(bool val)
	{
		this.forbiddenForPublic = val;
		if (this.forbiddenForPublic)
		{
			this.SetLocked(false, null, false);
			this.SetOpen(1f, null, false, 1f);
		}
		if (this.forbiddenForPublic)
		{
			if (this.spawnedDoor != null)
			{
				this.SpawnPoliceTape();
				Game.Log("Spawning police tape at " + this.spawnedDoor.name, 2);
			}
			if (!GameplayController.Instance.policeTapeDoors.Contains(this))
			{
				GameplayController.Instance.policeTapeDoors.Add(this);
				return;
			}
		}
		else
		{
			if (this.policeTape != null)
			{
				Object.Destroy(this.policeTape);
				this.SetOpen(0f, null, false, 1f);
				Game.Log("Removing police tape at " + this.spawnedDoor.name, 2);
			}
			GameplayController.Instance.policeTapeDoors.Remove(this);
		}
	}

	// Token: 0x0600136C RID: 4972 RVA: 0x00116ADC File Offset: 0x00114CDC
	private void SpawnPoliceTape()
	{
		if (this.policeTape == null)
		{
			this.policeTape = Object.Instantiate<GameObject>(PrefabControls.Instance.policeTape, base.transform);
			this.policeTape.transform.localPosition = new Vector3(0f, 0f, -0.25f);
			Toolbox.Instance.SetLightLayer(this.policeTape, this.wall.node.building, false);
		}
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x00116B58 File Offset: 0x00114D58
	public bool CitizenPassCheck(Human cc, out NewDoor.CitizenPassResult reason)
	{
		reason = NewDoor.CitizenPassResult.success;
		if (cc.ai != null && cc.ai.currentAction != null && cc.ai.currentAction.preset.ignoreLockedDoors)
		{
			return true;
		}
		bool result = false;
		bool flag = false;
		if (cc.currentRoom == this.wall.node.room)
		{
			flag = true;
		}
		if (this.forbiddenForPublic && !cc.currentGameLocation.entrances.Exists((NewNode.NodeAccess item) => item.door != this && item.walkingAccess))
		{
			flag = true;
		}
		if (this.isJammed)
		{
			reason = NewDoor.CitizenPassResult.isJammed;
		}
		else if (!this.isLocked && (!this.forbiddenForPublic || flag))
		{
			result = true;
		}
		else if (this.isLocked && (!this.forbiddenForPublic || flag))
		{
			if (cc.keyring.Contains(this) || (cc.ai != null && cc.ai.currentAction != null && cc.ai.currentAction.preset.ignoreLockedDoors))
			{
				result = true;
			}
			else
			{
				reason = NewDoor.CitizenPassResult.isLocked;
			}
		}
		else if (this.forbiddenForPublic && !flag)
		{
			reason = NewDoor.CitizenPassResult.isForbidden;
		}
		return result;
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x00116C80 File Offset: 0x00114E80
	public void Barge(Actor barger)
	{
		Game.Log("Gameplay: Execute barge door attempt...", 2);
		AudioController.Instance.PlayWorldOneShot(this.preset.doorBargeContact, barger, barger.currentNode, barger.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
		if (!this.isLocked && !this.isJammed)
		{
			this.doorInteractable.controller.isDoor.OpenByActor(barger, false, 4f);
			if (barger != null && barger.isPlayer)
			{
				Player.Instance.TransformPlayerController(GameplayControls.Instance.bargeDoorSuccess, null, this.doorInteractable, null, false, false, 0f, false, default(Vector3), 1f, true);
				StatusController.Instance.AddFineRecord(barger.currentGameLocation.thisAsAddress, this.doorInteractable, StatusController.CrimeType.breakingAndEntering, true, -1, false);
			}
			if (!(barger != null))
			{
				return;
			}
			NewNode node = this.wall.node;
			if (barger.currentNode == this.wall.node)
			{
				node = this.wall.otherWall.node;
			}
			using (HashSet<Actor>.Enumerator enumerator = node.room.currentOccupants.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Actor actor = enumerator.Current;
					if (actor.ai != null && actor.currentNode == node && !actor.isStunned && !actor.isDead)
					{
						actor.ai.SetKO(true, this.spawnedDoor.transform.position + new Vector3(0f, 1f, 0f), barger.transform.forward, false, 0f, true, CitizenControls.Instance.doorBargeKOForceMultiplier);
						if (barger != null && barger.isPlayer && AchievementsController.Instance != null)
						{
							AchievementsController.Instance.pacifistFlag = true;
							AchievementsController.Instance.UnlockAchievement("Coming Through!", "ko_with_barge");
						}
					}
				}
				return;
			}
		}
		if (barger != null && barger.isPlayer)
		{
			this.SetKnowLockedStatus(true);
			this.wall.SetDoorStrength(this.wall.currentDoorStrength - (Toolbox.Instance.Rand(GameplayControls.Instance.bargeDamageRange.x, GameplayControls.Instance.bargeDamageRange.y, false) + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.doorBargeModifier)));
		}
		else if (barger != null)
		{
			this.wall.SetDoorStrength(this.wall.currentDoorStrength - barger.combatHeft * 0.1f);
			Game.Log("Gameplay: AI set door strength: " + this.wall.currentDoorStrength.ToString(), 2);
		}
		else
		{
			this.wall.SetDoorStrength(this.wall.currentDoorStrength - 0.1f);
			Game.Log("AI Error: Barger is null!", 2);
		}
		if (this.wall.currentDoorStrength <= 0f && barger != null)
		{
			this.OpenByActor(barger, false, 4f);
			if (barger.isPlayer)
			{
				Player.Instance.TransformPlayerController(GameplayControls.Instance.bargeDoorSuccess, null, this.doorInteractable, null, false, false, 0f, false, default(Vector3), 1f, true);
			}
			AudioController.Instance.PlayWorldOneShot(this.preset.doorBargeBreak, barger, barger.currentNode, barger.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
			return;
		}
		if (barger != null && barger.isPlayer)
		{
			Player.Instance.TransformPlayerController(GameplayControls.Instance.bargeDoorFail, null, this.doorInteractable, null, false, false, 0f, false, default(Vector3), 1f, true);
		}
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x00117064 File Offset: 0x00115264
	public void OnKnock(Actor actor, int knockCount = 2, float forceAdditionalUrgency = 0f)
	{
		if (this.knockingInProgress)
		{
			return;
		}
		if (!this.isClosed)
		{
			return;
		}
		GameplayController.Instance.KnockOnDoor(this, actor, knockCount, forceAdditionalUrgency);
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x00117088 File Offset: 0x00115288
	public void OnDoorPeek()
	{
		InteractionController.Instance.SetLockedInInteractionMode(this.peekInteractable, 0, true);
		Vector3 vector = base.transform.InverseTransformPoint(Player.Instance.transform.position);
		Vector3 vector2 = Vector3.zero;
		if (vector.z > 0f)
		{
			vector2 = base.transform.TransformPoint(new Vector3(0f, 0f, 0.3f));
		}
		else
		{
			vector2 = base.transform.TransformPoint(new Vector3(0f, 0f, -0.3f));
		}
		vector2.y = Player.Instance.transform.position.y;
		Player.Instance.TransformPlayerController(GameplayControls.Instance.doorPeekEnter, GameplayControls.Instance.doorPeekExit, this.peekInteractable, null, false, false, 0f, false, default(Vector3), 1f, true);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromPeek;
		this.spawnedDoor.transform.localPosition = this.parentedWall.preset.doorOffset + new Vector3(0f, 0.03f, 0f);
		this.peekedUnder = true;
		if (Player.Instance.currentRoom != null && (Player.Instance.currentRoom.doorCheckSet.Contains(this.wall.id) || Player.Instance.currentRoom.doorCheckSet.Contains(this.wall.otherWall.id)) && (!Game.Instance.enableNewRealtimeTimeCullingSystem || GeometryCullingController.Instance.doorsBlockSight))
		{
			Player.Instance.UpdateCullingOnEndOfFrame();
		}
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x00117240 File Offset: 0x00115440
	public void OnReturnFromPeek()
	{
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromPeek;
		Player.Instance.ReturnFromTransform(false, true);
		this.spawnedDoor.transform.localPosition = this.parentedWall.preset.doorOffset;
		this.peekedUnder = false;
		if (Player.Instance.currentRoom != null && (Player.Instance.currentRoom.doorCheckSet.Contains(this.wall.id) || Player.Instance.currentRoom.doorCheckSet.Contains(this.wall.otherWall.id)) && (!Game.Instance.enableNewRealtimeTimeCullingSystem || GeometryCullingController.Instance.doorsBlockSight))
		{
			Player.Instance.UpdateCullingOnEndOfFrame();
		}
	}

	// Token: 0x06001372 RID: 4978 RVA: 0x00117314 File Offset: 0x00115514
	public void OnLockpick()
	{
		this.SetKnowLockedStatus(true);
		if (!this.isLocked)
		{
			return;
		}
		if (!this.isClosed)
		{
			return;
		}
		InteractionController.Instance.SetLockedInInteractionMode(this.handleInteractable, 0, true);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.lockpickEnter, GameplayControls.Instance.lockpickExit, this.handleInteractable, null, false, false, 0f, false, default(Vector3), 1f, true);
		InteractionController.Instance.SetInteractionAction(0f, this.wall.currentLockStrength, Mathf.LerpUnclamped(GameplayControls.Instance.lockpickEffectivenessRange.x, GameplayControls.Instance.lockpickEffectivenessRange.y, UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.lockpickingEfficiencyModifier)), "lockpicking", true, true, this.doorHandleInteractableController.transform, true);
		Player.Instance.isLockpicking = true;
		InteractionController.Instance.OnInteractionActionProgressChange += this.OnLockpickProgressChange;
		InteractionController.Instance.OnInteractionActionCompleted += this.OnCompleteLockpick;
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromLockpick;
		InteractionController.Instance.OnInteractionActionLookedAway += this.OnLockpickLookedAway;
		AudioController.Instance.StopSound(this.lockpickLoop, AudioController.StopType.immediate, "Stop lockpick");
		if (this.doorInteractable != null)
		{
			StatusController.Instance.AddFineRecord(Player.Instance.currentGameLocation.thisAsAddress, this.doorInteractable, StatusController.CrimeType.breakingAndEntering, false, -1, false);
		}
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x00117488 File Offset: 0x00115688
	public void OnLockpickLookedAway()
	{
		AudioController.Instance.StopSound(this.lockpickLoop, AudioController.StopType.immediate, "Stop lockpick");
		this.audioLoopStarted = false;
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x001174A8 File Offset: 0x001156A8
	public void OnLockpickProgressChange(float amountChangeThisFrame, float amountToal)
	{
		if (!this.audioLoopStarted)
		{
			this.audioLoopStarted = true;
			this.lockpickLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.lockpick, Player.Instance, this.handleInteractable, null, 1f, false, false, null, null);
		}
		this.wall.SetCurrentLockStrength(this.wall.currentLockStrength - amountChangeThisFrame);
		GameplayController.Instance.UseLockpick(amountChangeThisFrame);
		if (GameplayController.Instance.lockPicks <= 0)
		{
			InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_lockpicks", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.lockpick, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.lockpicks, null, null, null);
		}
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x0011756C File Offset: 0x0011576C
	public void OnCompleteLockpick()
	{
		AudioController.Instance.StopSound(this.lockpickLoop, AudioController.StopType.fade, "Complete lockpick");
		this.lockpickLoop = null;
		InteractionController.Instance.OnInteractionActionProgressChange -= this.OnLockpickProgressChange;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteLockpick;
		InteractionController.Instance.OnInteractionActionLookedAway -= this.OnLockpickLookedAway;
		Player.Instance.isLockpicking = false;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		StatusController.Instance.RemoveFineRecord(null, this.doorInteractable, StatusController.CrimeType.breakingAndEntering, true, true);
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x00117604 File Offset: 0x00115804
	public void OnReturnFromLockpick()
	{
		AudioController.Instance.StopSound(this.lockpickLoop, AudioController.StopType.fade, "Return from lockpick");
		this.lockpickLoop = null;
		InteractionController.Instance.OnInteractionActionProgressChange -= this.OnLockpickProgressChange;
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromLockpick;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteLockpick;
		Player.Instance.isLockpicking = false;
		Player.Instance.ReturnFromTransform(false, true);
		InterfaceController instance = InterfaceController.Instance;
		InterfaceController.GameMessageType newType = InterfaceController.GameMessageType.notification;
		int newNumerical = 0;
		string newMessage = GameplayController.Instance.lockPicks.ToString() + " " + Strings.Get("ui.gamemessage", "lockpick_deplete", Strings.Casing.asIs, false, false, false, null);
		InterfaceControls.Icon newIcon = InterfaceControls.Icon.lockpick;
		AudioEvent additionalSFX = null;
		bool colourOverride = false;
		RectTransform moneyNotificationIcon = InterfaceController.Instance.moneyNotificationIcon;
		instance.NewGameMessage(newType, newNumerical, newMessage, newIcon, additionalSFX, colourOverride, default(Color), -1, 0f, moneyNotificationIcon, GameMessageController.PingOnComplete.lockpicks, null, null, null);
		StatusController.Instance.RemoveFineRecord(null, this.doorInteractable, StatusController.CrimeType.breakingAndEntering, true, true);
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x001176F8 File Offset: 0x001158F8
	public bool GetDefaultLockState()
	{
		if (((this.wall.node.gameLocation.thisAsAddress != null && this.wall.node.gameLocation.thisAsAddress.residence != null) || (this.wall.otherWall.node.gameLocation.thisAsAddress != null && this.wall.otherWall.node.gameLocation.thisAsAddress.residence != null)) && this.wall.node.gameLocation != this.wall.otherWall.node.gameLocation)
		{
			return true;
		}
		if (this.wall.node.gameLocation.thisAsAddress != null && this.wall.node.gameLocation.thisAsAddress.company != null)
		{
			return this.wall.node.gameLocation.thisAsAddress.addressPreset == null || this.wall.node.gameLocation.thisAsAddress.addressPreset.entrancesLockedByDefault;
		}
		return this.wall.otherWall.node.gameLocation.thisAsAddress != null && this.wall.otherWall.node.gameLocation.thisAsAddress.company != null && (this.wall.otherWall.node.gameLocation.thisAsAddress.addressPreset == null || this.wall.otherWall.node.gameLocation.thisAsAddress.addressPreset.entrancesLockedByDefault);
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x001178D0 File Offset: 0x00115AD0
	[Button(null, 0)]
	public void DebugTestPlayersRelativePosition()
	{
		Game.Log(base.transform.InverseTransformPoint(Player.Instance.transform.position), 2);
	}

	// Token: 0x0400179D RID: 6045
	[Tooltip("This will remain static: Always the parent wall")]
	[Header("Location")]
	public NewWall wall;

	// Token: 0x0400179E RID: 6046
	[Tooltip("The door will always be parented to the room the player is in (or can see).")]
	public NewRoom playerRoom;

	// Token: 0x0400179F RID: 6047
	[Tooltip("This will change with the above, the wall this is currently parented to")]
	public NewWall parentedWall;

	// Token: 0x040017A0 RID: 6048
	[Header("Details")]
	public DoorPairPreset doorPairPreset;

	// Token: 0x040017A1 RID: 6049
	public DoorPreset preset;

	// Token: 0x040017A2 RID: 6050
	[Header("Spawned Objects")]
	public GameObject spawnedDoor;

	// Token: 0x040017A3 RID: 6051
	public List<Collider> spawnedDoorColliders = new List<Collider>();

	// Token: 0x040017A4 RID: 6052
	[NonSerialized]
	public Interactable doorInteractable;

	// Token: 0x040017A5 RID: 6053
	[NonSerialized]
	public Interactable handleInteractable;

	// Token: 0x040017A6 RID: 6054
	[NonSerialized]
	public Interactable peekInteractable;

	// Token: 0x040017A7 RID: 6055
	public RectTransform mapDoorObject;

	// Token: 0x040017A8 RID: 6056
	public GameObject doorSignFront;

	// Token: 0x040017A9 RID: 6057
	public GameObject doorSignRear;

	// Token: 0x040017AA RID: 6058
	public InteractableController doorInteractableController;

	// Token: 0x040017AB RID: 6059
	public InteractableController doorHandleInteractableController;

	// Token: 0x040017AC RID: 6060
	public InteractableController peekInteractableController;

	// Token: 0x040017AD RID: 6061
	public GameObject policeTape;

	// Token: 0x040017AE RID: 6062
	[Tooltip("-1 = Open, 0 = Closed, 1 = Open")]
	[Header("Door State")]
	public float ajar;

	// Token: 0x040017AF RID: 6063
	[Tooltip("True if closed")]
	public bool isClosed = true;

	// Token: 0x040017B0 RID: 6064
	[Tooltip("True if closing (animation)")]
	public bool isClosing;

	// Token: 0x040017B1 RID: 6065
	[Tooltip("Updated actual ajar value that is consistent with animation")]
	public float ajarProgress;

	// Token: 0x040017B2 RID: 6066
	[Tooltip("How fast the door opens and closes")]
	public float doorOpenSpeed = 1.46f;

	// Token: 0x040017B3 RID: 6067
	[Tooltip("True if animating")]
	public bool animating;

	// Token: 0x040017B4 RID: 6068
	[Tooltip("What the AI will do when passing through the door")]
	public NewDoor.DoorSetting doorSetting = NewDoor.DoorSetting.leaveClosed;

	// Token: 0x040017B5 RID: 6069
	[Tooltip("What the AI will do when passing through the door")]
	public NewDoor.LockSetting lockSetting;

	// Token: 0x040017B6 RID: 6070
	[Tooltip("If there are others on this list when it comes to the AI closing the door behind them, keep it open.")]
	public HashSet<Actor> usingDoorList = new HashSet<Actor>();

	// Token: 0x040017B7 RID: 6071
	[Tooltip("The door is being peeked under")]
	public bool peekedUnder;

	// Token: 0x040017B8 RID: 6072
	[Tooltip("True if the other side of this door would be trespassing")]
	public bool otherSideIsTrespassing;

	// Token: 0x040017B9 RID: 6073
	public int otherSideTrespassingEscalation;

	// Token: 0x040017BA RID: 6074
	private NewRoom playerOtherSideRoom;

	// Token: 0x040017BB RID: 6075
	public float desiredAngle;

	// Token: 0x040017BC RID: 6076
	[Tooltip("The maximum angle for an open door. This should be less than 90 if you don't want it to clip close walls")]
	public float openAngle = 89.9f;

	// Token: 0x040017BD RID: 6077
	[Tooltip("Is this door locked?")]
	public bool isLocked;

	// Token: 0x040017BE RID: 6078
	[Tooltip("Is this door jammed with a door wedge?")]
	public bool isJammed;

	// Token: 0x040017BF RID: 6079
	[NonSerialized]
	public Interactable doorWedge;

	// Token: 0x040017C0 RID: 6080
	[Tooltip("Is this door marked as forbidden for public?")]
	public bool forbiddenForPublic;

	// Token: 0x040017C1 RID: 6081
	[Tooltip("Does the player know the status of the lock?")]
	public bool knowLockStatus;

	// Token: 0x040017C2 RID: 6082
	[Tooltip("Knock attempt count for the player")]
	public bool knockingInProgress;

	// Token: 0x040017C3 RID: 6083
	[Tooltip("True if this features a neon sign")]
	public bool featuresNeonSign;

	// Token: 0x040017C4 RID: 6084
	[Tooltip("Lock interactable")]
	[NonSerialized]
	public Interactable lockInteractableFront;

	// Token: 0x040017C5 RID: 6085
	[NonSerialized]
	public Interactable lockInteractableRear;

	// Token: 0x040017C6 RID: 6086
	public NewRoom passwordDoorsRoom;

	// Token: 0x040017C7 RID: 6087
	private AudioController.LoopingSoundInfo lockpickLoop;

	// Token: 0x040017C8 RID: 6088
	public List<NewNode> bothNodesForAudioSource = new List<NewNode>();

	// Token: 0x040017C9 RID: 6089
	private bool audioLoopStarted;

	// Token: 0x040017CA RID: 6090
	[Header("Debug")]
	public List<string> passwordPlacementDebug = new List<string>();

	// Token: 0x040017CB RID: 6091
	public List<string> isLockedDebug = new List<string>();

	// Token: 0x0200035F RID: 863
	public enum DoorSetting
	{
		// Token: 0x040017CD RID: 6093
		leaveOpen,
		// Token: 0x040017CE RID: 6094
		leaveClosed
	}

	// Token: 0x02000360 RID: 864
	public enum LockSetting
	{
		// Token: 0x040017D0 RID: 6096
		keepUnlocked,
		// Token: 0x040017D1 RID: 6097
		keepLocked
	}

	// Token: 0x02000361 RID: 865
	public enum CitizenPassResult
	{
		// Token: 0x040017D3 RID: 6099
		success,
		// Token: 0x040017D4 RID: 6100
		isLocked,
		// Token: 0x040017D5 RID: 6101
		isJammed,
		// Token: 0x040017D6 RID: 6102
		isForbidden
	}
}
