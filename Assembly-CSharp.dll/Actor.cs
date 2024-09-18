using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class Actor : Controller
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06000291 RID: 657 RVA: 0x00019914 File Offset: 0x00017B14
	// (remove) Token: 0x06000292 RID: 658 RVA: 0x0001994C File Offset: 0x00017B4C
	public event Actor.InteractionChanged OnInteractionChanged;

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x06000293 RID: 659 RVA: 0x00019984 File Offset: 0x00017B84
	// (remove) Token: 0x06000294 RID: 660 RVA: 0x000199BC File Offset: 0x00017BBC
	public event Actor.RoutineChanged OnRoutineChange;

	// Token: 0x06000295 RID: 661 RVA: 0x000199F4 File Offset: 0x00017BF4
	public void SetInteracting(Interactable other)
	{
		if (this.interactingWith != other)
		{
			this.interactingWith = other;
			if (other != null)
			{
				if (other.objectRef != null && other.objectRef as Actor != null && this as Citizen != null && (this as Citizen).ai != null)
				{
					(this as Citizen).ai.UpdateTrackedTargets();
				}
				this.OnNewInteraction();
			}
			else if (this.ai != null)
			{
				this.ai.faceTransform = null;
			}
			if (this.OnInteractionChanged != null)
			{
				this.OnInteractionChanged();
			}
		}
	}

	// Token: 0x06000296 RID: 662 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnNewInteraction()
	{
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00019A98 File Offset: 0x00017C98
	public virtual void Teleport(NewNode teleportLocation, Interactable.UsagePoint usagePoint, bool cancelVent = true, bool teleportYPostionOnly = false)
	{
		if (teleportLocation == null)
		{
			Game.LogError("Teleport location for " + base.name + " is null! Teleporting to random street instead...", 2);
			Human human = this as Human;
			if (human != null)
			{
				if (human.home != null)
				{
					this.Teleport(human.FindSafeTeleport(human.home, false, true), null, true, false);
					return;
				}
				this.Teleport(human.FindSafeTeleport(CityData.Instance.streetDirectory[Toolbox.Instance.Rand(0, CityData.Instance.streetDirectory.Count, false)], false, true), null, true, false);
			}
			return;
		}
		if (this.isDead)
		{
			Game.LogError("Teleporting a dead person: Are you sure you want to do this?", 2);
		}
		if (this.ai != null && this.ai.currentGoal != null && (this.currentGameLocation == null || teleportLocation.gameLocation != this.currentGameLocation))
		{
			int num = 50;
			while (this.ai.currentGoal.actions.Count > 0 && num > 0)
			{
				this.ai.currentGoal.actions[0].ImmediateComplete();
				num--;
				if (num <= 0)
				{
					Game.LogError("While trying to teleport AI, the loop that immediately completes actions is reaching the safety limit. This means there's something creating actions as we're trying to remove them!", 2);
				}
			}
			this.ai.currentGoal.OnDeactivate(0f);
		}
		if (usagePoint != null)
		{
			base.transform.position = usagePoint.GetUsageWorldPosition(teleportLocation.position, this);
		}
		else if (teleportLocation.defaultSpace != null)
		{
			base.transform.position = teleportLocation.defaultSpace.position;
		}
		else
		{
			base.transform.position = teleportLocation.position;
		}
		this.UpdateGameLocation(0f);
		if (Game.Instance.collectDebugData)
		{
			this.SelectedDebug("Teleported to " + teleportLocation.gameLocation.name, Actor.HumanDebug.movement);
			if (this.ai != null)
			{
				this.ai.debugDestinationPosition.Add("Teleport: " + teleportLocation.gameLocation.name);
			}
		}
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00019CA5 File Offset: 0x00017EA5
	public float GetCurrentMaxHealth()
	{
		return this.maximumHealth * StatusController.Instance.maxHealthMultiplier;
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00019CB8 File Offset: 0x00017EB8
	public virtual void UpdateGameLocation(float feetOffset = 0f)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		float num = 0f;
		if (this.isPlayer)
		{
			num = Player.Instance.fps.m_CharacterController.height * 0.5f + Player.Instance.fps.m_CharacterController.skinWidth;
		}
		Vector3 vector;
		vector..ctor(base.transform.position.x, base.transform.position.y + feetOffset, base.transform.position.z);
		Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(vector);
		NewNode newNode = null;
		if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
		{
			newNode = Toolbox.Instance.GetNearestGroundLevelOutside(vector);
			if (newNode != null)
			{
			}
		}
		else
		{
			float num2 = newNode.position.y - (base.transform.position.y - num);
			if (Mathf.Abs(num2) >= PathFinder.Instance.nodeSize.z - num + 0.1f && this.isPlayer)
			{
				num2 = Mathf.Clamp(num2, -1f, 1f);
				vector3Int -= new Vector3Int(0, 0, Mathf.RoundToInt(num2));
				if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
				{
					newNode = Toolbox.Instance.GetNearestGroundLevelOutside(vector);
					if (newNode == null && this.isPlayer && SessionData.Instance.startedGame && Game.Instance.printDebug)
					{
						string[] array = new string[5];
						array[0] = "Player: Unable to find player node for pos ";
						int num3 = 1;
						Vector3Int vector3Int2 = vector3Int;
						array[num3] = vector3Int2.ToString();
						array[2] = " (";
						array[3] = PathFinder.Instance.nodeMap.Count.ToString();
						array[4] = " nodes)";
						Game.Log(string.Concat(array), 2);
					}
				}
			}
		}
		if (newNode != null && newNode != this.currentNode)
		{
			this.debugPrevNode2 = this.debugPrevNode1;
			this.debugPrevNode1 = this.previousNode;
			this.previousNode = this.currentNode;
			this.currentNode = newNode;
			this.currentNodeCoord = this.currentNode.nodeCoord;
			this.OnNodeChange();
			if (this.currentNode.tile != this.currentTile)
			{
				this.previousTile = this.currentTile;
				this.currentTile = this.currentNode.tile;
				this.OnTileChange();
			}
			if (this.currentNode.room != this.currentRoom)
			{
				this.previousRoom = this.currentRoom;
				this.currentRoom = this.currentNode.room;
				if (this.previousRoom != null)
				{
					this.previousRoom.RemoveOccupant(this);
				}
				if (this.currentRoom != null)
				{
					this.currentRoom.AddOccupant(this);
				}
				this.OnRoomChange();
			}
			if (this.currentNode.gameLocation != this.currentGameLocation)
			{
				this.previousGameLocation = this.currentGameLocation;
				this.currentGameLocation = this.currentNode.gameLocation;
				if (this.previousGameLocation != null)
				{
					this.previousGameLocation.RemoveOccupant(this);
				}
				if (this.currentGameLocation != null)
				{
					this.currentGameLocation.AddOccupant(this);
				}
				if (this.currentGameLocation.isOutside)
				{
					this.SetOnStreet(true);
				}
				else
				{
					this.SetOnStreet(false);
				}
				this.OnGameLocationChange(true, false);
				if (this.currentNode.building != this.currentBuilding)
				{
					this.previousBuilding = this.currentBuilding;
					this.currentBuilding = this.currentNode.building;
					this.OnBuildingChange();
				}
			}
			if (this.currentNode.tile.cityTile != this.currentCityTile)
			{
				this.previousCityTile = this.currentCityTile;
				this.currentCityTile = this.currentNode.tile.cityTile;
				this.OnCityTileChange();
			}
		}
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnCityTileChange()
	{
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0001A0AC File Offset: 0x000182AC
	public virtual void OnBuildingChange()
	{
		for (int i = 0; i < this.meshes.Count; i++)
		{
			Toolbox.Instance.SetLightLayer(this.meshes[i], this.currentBuilding, false);
		}
		for (int j = 0; j < this.meshesLOD1.Count; j++)
		{
			Toolbox.Instance.SetLightLayer(this.meshesLOD1[j], this.currentBuilding, false);
		}
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnTileChange()
	{
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0001A11F File Offset: 0x0001831F
	public virtual void OnGameLocationChange(bool enableSocialSightings = true, bool forceDisableLocationMemory = false)
	{
		if (this.ai != null)
		{
			this.ai.timeAtCurrentAddress = 0f;
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0001A13F File Offset: 0x0001833F
	public virtual void OnNodeChange()
	{
		if (this.interactable != null)
		{
			this.interactable.UpdateWorldPositionAndNode(true);
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0001A158 File Offset: 0x00018358
	public virtual void OnRoomChange()
	{
		if (Game.Instance.collectDebugData && this.ai != null && this.ai.isRagdoll)
		{
			Game.Log(string.Concat(new string[]
			{
				"Nino: AI ragdoll room change: ",
				this.previousRoom.GetName(),
				" -> ",
				this.currentRoom.GetName(),
				" (",
				this.currentRoom.gameLocation.name,
				", ",
				this.currentRoom.roomID.ToString(),
				")"
			}), 2);
		}
		if (!this.visible && this.currentRoom.isVisible)
		{
			this.SetVisible(true, false);
			if (this.currentRoom.preset.forceStreetLightLayer != this.previousRoom.preset.forceStreetLightLayer)
			{
				for (int i = 0; i < this.meshes.Count; i++)
				{
					Toolbox.Instance.SetLightLayer(this.meshes[i], this.currentBuilding, false);
				}
				for (int j = 0; j < this.meshesLOD1.Count; j++)
				{
					Toolbox.Instance.SetLightLayer(this.meshesLOD1[j], this.currentBuilding, false);
				}
				return;
			}
		}
		else if (this.visible && !this.currentRoom.isVisible)
		{
			if (Game.Instance.collectDebugData)
			{
				this.SelectedDebug("Setting invisible as current room " + this.currentRoom.GetName() + " is not visible", Actor.HumanDebug.misc);
			}
			this.SetVisible(false, false);
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0001A307 File Offset: 0x00018507
	public virtual void SetOnStreet(bool val)
	{
		if (!this.isOnStreet && val)
		{
			this.isOnStreet = true;
			return;
		}
		if (this.isOnStreet && !val)
		{
			this.isOnStreet = false;
		}
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0001A330 File Offset: 0x00018530
	public virtual void AddToKeyring(NewAddress ad, bool gameMessage = true)
	{
		if (ad == null)
		{
			return;
		}
		if (Game.Instance.printDebug)
		{
			Game.Log("Gameplay: Add to keyring: " + ad.name, 2);
		}
		foreach (NewRoom newRoom in ad.rooms)
		{
			foreach (NewNode.NodeAccess nodeAccess in newRoom.entrances)
			{
				if (nodeAccess.wall != null && nodeAccess.wall.door != null)
				{
					if (nodeAccess.wall.door.preset.lockType != DoorPreset.LockType.none)
					{
						this.AddToKeyring(nodeAccess.wall.door, false);
					}
				}
				else if (nodeAccess.door != null && nodeAccess.door.preset.lockType != DoorPreset.LockType.none)
				{
					this.AddToKeyring(nodeAccess.door, false);
				}
			}
		}
		if (ad.residence != null && this.isPlayer && ad.residence.mailbox != null)
		{
			Predicate<Interactable.Passed> <>9__0;
			foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in ad.residence.mailbox.ownerMap)
			{
				if (keyValuePair.Key.address == ad)
				{
					foreach (Interactable interactable in ad.residence.mailbox.integratedInteractables)
					{
						if (interactable.pv != null)
						{
							List<Interactable.Passed> pv = interactable.pv;
							Predicate<Interactable.Passed> predicate;
							if ((predicate = <>9__0) == null)
							{
								predicate = (<>9__0 = ((Interactable.Passed item) => item.varType == Interactable.PassedVarType.ownedByAddress && item.value == (float)ad.id));
							}
							if (pv.Exists(predicate))
							{
								Player.Instance.AddToKeyring(interactable, false);
							}
						}
					}
				}
			}
		}
		if (ad.building.mainEntrance != null)
		{
			if (ad.building.mainEntrance.door != null)
			{
				this.AddToKeyring(ad.building.mainEntrance.door, false);
			}
		}
		else
		{
			Game.Log("CityGen: " + ad.building.name + " features no 'main entrance', listing all entrances...", 2);
			foreach (NewWall newWall in ad.building.additionalEntrances)
			{
				string[] array = new string[14];
				array[0] = "CityGen: ...";
				array[1] = newWall.id.ToString();
				array[2] = " ";
				int num = 3;
				Vector3 position = newWall.node.position;
				array[num] = position.ToString();
				array[4] = " (";
				array[5] = newWall.node.room.name;
				array[6] = ", ";
				array[7] = newWall.otherWall.node.room.name;
				array[8] = ") Tile ";
				int num2 = 9;
				Vector3Int globalTileCoord = newWall.node.tile.globalTileCoord;
				array[num2] = globalTileCoord.ToString();
				array[10] = " Entrance: ";
				array[11] = newWall.node.tile.isEntrance.ToString();
				array[12] = " main: ";
				array[13] = newWall.node.tile.isMainEntrance.ToString();
				Game.Log(string.Concat(array), 2);
			}
		}
		if (ad.building != null)
		{
			foreach (NewWall newWall2 in ad.building.additionalEntrances)
			{
				if (newWall2.door != null && newWall2.door.preset.lockType != DoorPreset.LockType.none)
				{
					this.AddToKeyring(newWall2.door, false);
				}
			}
		}
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0001A844 File Offset: 0x00018A44
	public virtual void AddToKeyring(NewDoor ac, bool gameMessage = true)
	{
		if (!this.keyring.Contains(ac))
		{
			this.keyring.Add(ac);
		}
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0001A860 File Offset: 0x00018A60
	public virtual void RemoveFromKeyring(NewAddress ad)
	{
		foreach (NewNode.NodeAccess nodeAccess in ad.entrances)
		{
			if (nodeAccess.wall != null && nodeAccess.wall.door != null)
			{
				this.RemoveFromKeyring(nodeAccess.wall.door);
			}
		}
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x0001A8D8 File Offset: 0x00018AD8
	public virtual void RemoveFromKeyring(NewDoor ac)
	{
		this.keyring.Remove(ac);
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0001A8E8 File Offset: 0x00018AE8
	public virtual void SetVisible(bool vis, bool force = false)
	{
		if (Player.Instance == this)
		{
			return;
		}
		if (vis == this.visible)
		{
			return;
		}
		if (!force && this.outline != null && this.outline.outlineActive && !vis)
		{
			return;
		}
		if (Game.Instance.collectDebugData)
		{
			this.SelectedDebug("Set visible official method: " + vis.ToString() + " force: " + force.ToString(), Actor.HumanDebug.misc);
		}
		if (vis)
		{
			this.visible = true;
			CitizenBehaviour.Instance.visibleHumans++;
			this.SetModelParentVisibility(true, "SetVisible");
			CityData.Instance.visibleActors.Add(this);
			if (this.animationController != null)
			{
				this.animationController.ForceUpdateAnimationSate(true);
			}
		}
		else
		{
			this.visible = false;
			CitizenBehaviour.Instance.visibleHumans--;
			CityData.Instance.visibleActors.Remove(this);
			this.SetModelParentVisibility(false, "SetVisible");
		}
		if (this.ai != null)
		{
			this.ai.OnVisibilityChanged();
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0001AA04 File Offset: 0x00018C04
	public void SetModelParentVisibility(bool val, string debugReason)
	{
		if (this.modelParent != null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.SelectedDebug(string.Concat(new string[]
				{
					"Set model parent visibility: ",
					val.ToString(),
					" (",
					debugReason,
					") Previously: ",
					this.modelParent.activeSelf.ToString()
				}), Actor.HumanDebug.misc);
			}
			this.modelParent.SetActive(val);
		}
		if (this.distantLOD != null)
		{
			this.distantLOD.SetActive(val);
		}
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0001AA9F File Offset: 0x00018C9F
	public virtual void GoToSleep()
	{
		if (this.isAsleep)
		{
			return;
		}
		this.isAsleep = true;
		this.sleepDepth = 0f;
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0001AABC File Offset: 0x00018CBC
	public virtual void WakeUp(bool forceImmediate = false)
	{
		if (this.isDead)
		{
			return;
		}
		if (!this.isAsleep)
		{
			return;
		}
		this.isAsleep = false;
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0001AAD7 File Offset: 0x00018CD7
	public void RoutineChange()
	{
		if (this.OnRoutineChange != null)
		{
			this.OnRoutineChange();
		}
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0001AAD7 File Offset: 0x00018CD7
	public void OnRoutineEnd()
	{
		if (this.OnRoutineChange != null)
		{
			this.OnRoutineChange();
		}
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0001AAEC File Offset: 0x00018CEC
	public void SetStealthMode(bool newVal)
	{
		if (newVal != this.stealthMode)
		{
			this.stealthMode = newVal;
			this.appliedStealth = 0f;
			if (this.isPlayer)
			{
				Player.Instance.nodesTraversedWhileWalking = 0;
			}
			if (this.stealthMode)
			{
				CitizenBehaviour.Instance.actorsInStealthMode.Add(this);
			}
			else
			{
				CitizenBehaviour.Instance.actorsInStealthMode.Remove(this);
				if (!this.isPlayer)
				{
					this.currentLightLevel = 1f;
					this.lightLevelTransition = 1f;
					this.currentVisibilityPotential = 1f;
				}
			}
			this.UpdateOverallVisibility();
			this.OnStealthModeChange();
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0001AB8C File Offset: 0x00018D8C
	public void SetCrouched(bool newVal, bool instant = false)
	{
		if (newVal != this.isCrouched)
		{
			this.isCrouched = newVal;
			Game.Log("Player: Set crouched: " + this.isCrouched.ToString(), 2);
			Player player = this as Player;
			if (!this.stealthMode && this.isCrouched)
			{
				this.SetStealthMode(true);
			}
			else if (!this.isCrouched && (player == null || (player != null && !player.illegalStatus)))
			{
				this.SetStealthMode(false);
			}
			if (instant && player != null)
			{
				if (this.isCrouched)
				{
					player.crouchedTransition = 0f;
					player.SetPlayerHeight(Player.Instance.GetPlayerHeightNormal(), true);
					player.SetCameraHeight(GameplayControls.Instance.cameraHeightNormal);
					player.crouchTransitionActive = false;
				}
				else
				{
					player.crouchedTransition = 1f;
					player.SetPlayerHeight(Player.Instance.GetPlayerHeightCrouched(), true);
					player.SetCameraHeight(GameplayControls.Instance.cameraHeightCrouched);
					player.crouchTransitionActive = false;
				}
			}
			this.UpdateOverallVisibility();
			this.OnCrouchedChange();
		}
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0001AC98 File Offset: 0x00018E98
	public void StealthModeLoop()
	{
		this.UpdateLightLevel();
		if (this.lightLevelTransition < this.currentLightLevel)
		{
			this.lightLevelTransition += GameplayControls.Instance.stealthModeLoopUpdateFrequency;
			this.lightLevelTransition = Mathf.Min(this.lightLevelTransition, this.currentLightLevel);
		}
		else if (this.lightLevelTransition > this.currentLightLevel)
		{
			this.lightLevelTransition -= GameplayControls.Instance.stealthModeLoopUpdateFrequency;
			this.lightLevelTransition = Mathf.Max(this.lightLevelTransition, this.currentLightLevel);
		}
		float num = this.currentLightLevel;
		if (this.isMoving)
		{
			if (this.appliedStealth < this.stealthSkill * 0.25f)
			{
				this.appliedStealth += CitizenControls.Instance.stealthSkillApplicationRate * GameplayControls.Instance.stealthModeLoopUpdateFrequency;
				this.appliedStealth = Mathf.Min(this.appliedStealth, this.stealthSkill * 0.25f);
			}
			else if (this.appliedStealth > this.stealthSkill * 0.25f)
			{
				this.appliedStealth -= CitizenControls.Instance.stealthSkillCancelRate * GameplayControls.Instance.stealthModeLoopUpdateFrequency;
				this.appliedStealth = Mathf.Max(this.appliedStealth, this.stealthSkill * 0.25f);
			}
			if (this.isRunning)
			{
				num = 1f;
			}
		}
		else if (this.appliedStealth < this.stealthSkill)
		{
			this.appliedStealth += CitizenControls.Instance.stealthSkillApplicationRate * GameplayControls.Instance.stealthModeLoopUpdateFrequency;
		}
		if (this.currentVisibilityPotential < num)
		{
			this.currentVisibilityPotential += GameplayControls.Instance.stealthModeLoopUpdateFrequency;
			this.currentVisibilityPotential = Mathf.Min(this.currentVisibilityPotential, num);
		}
		else if (this.currentVisibilityPotential > num)
		{
			this.currentVisibilityPotential -= GameplayControls.Instance.stealthModeLoopUpdateFrequency;
			this.currentVisibilityPotential = Mathf.Max(this.currentVisibilityPotential, num);
		}
		this.appliedStealth = Mathf.Clamp(this.appliedStealth, 0f, this.stealthSkill);
		this.UpdateOverallVisibility();
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0001AEA7 File Offset: 0x000190A7
	public virtual void UpdateLightLevel()
	{
		this.currentLightLevel = 1f;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnStealthModeChange()
	{
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnCrouchedChange()
	{
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0001AEB4 File Offset: 0x000190B4
	public void UpdateOverallVisibility()
	{
		this.overallVisibility = Mathf.Clamp01(this.currentVisibilityPotential - (1f - this.lightLevelTransition) * this.appliedStealth * 0.33f);
		if (this.isPlayer && FirstPersonItemController.Instance.flashlight)
		{
			this.overallVisibility = 1f;
		}
		float num = GameplayControls.Instance.citizenSightRange - GameplayControls.Instance.minimumStealthDetectionRange;
		this.stealthDistance = GameplayControls.Instance.minimumStealthDetectionRange + this.overallVisibility * this.overallVisibility * num;
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0001AF41 File Offset: 0x00019141
	public virtual void SetHiding(bool val, Interactable newHidingPlace)
	{
		if (val != this.isHiding)
		{
			this.isHiding = val;
		}
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0001AF54 File Offset: 0x00019154
	public virtual void RecieveDamage(float amount, Actor fromWho, Vector3 damagePosition, Vector3 damageDirection, SpatterPatternPreset forwardSpatter, SpatterPatternPreset backSpatter, SpatterSimulation.EraseMode spatterErase = SpatterSimulation.EraseMode.useDespawnTime, bool alertSurrounding = true, bool forceRagdoll = false, float forcedRagdollDuration = 0f, float shockMP = 1f, bool enableKill = false, bool allowRecoil = true, float ragdollForceMP = 1f)
	{
		if (this.currentHealth <= 0f || this.isStunned)
		{
			return;
		}
		damageDirection = damageDirection.normalized;
		if (this.isAsleep)
		{
			this.WakeUp(false);
		}
		this.currentHealth -= amount;
		this.currentHealth = Mathf.Max(this.currentHealth, 0f);
		this.currentHealthNormalized = this.currentHealth / this.maximumHealth;
		Game.Log(string.Concat(new string[]
		{
			base.name,
			" damage health of ",
			amount.ToString(),
			" remaining: ",
			this.currentHealth.ToString()
		}), 2);
		if (this.currentHealth <= 0f)
		{
			this.OnZeroHealthReached();
		}
		float num = amount / this.maximumHealth * shockMP;
		this.AddNerve(-num * CitizenControls.Instance.nerveDamageShockMultiplier, fromWho);
		Game.Log(string.Concat(new string[]
		{
			base.name,
			" damage nerve of ",
			num.ToString(),
			" remaining: ",
			this.currentNerve.ToString()
		}), 2);
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0001B07D File Offset: 0x0001927D
	public virtual void AddHealth(float amount, bool affectedByGameDifficulty = true, bool displayDamageIndicator = false)
	{
		this.currentHealth += amount;
		this.currentHealth = Mathf.Clamp(this.currentHealth, 0f, this.maximumHealth);
		this.currentHealthNormalized = this.currentHealth / this.maximumHealth;
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0001B0BC File Offset: 0x000192BC
	public virtual void SetHealth(float amount)
	{
		this.currentHealth = amount;
		this.currentHealth = Mathf.Clamp(this.currentHealth, 0f, this.maximumHealth);
		this.currentHealthNormalized = this.currentHealth / this.maximumHealth;
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0001B0F4 File Offset: 0x000192F4
	public virtual void AddNerve(float amount, Actor scaredBy = null)
	{
		float num = this.currentNerve;
		if (this.isEnforcer && (this.isOnDuty || (this.ai != null && this.ai.inCombat)) && amount < 0f)
		{
			amount *= 0.3f;
		}
		if (this.ai != null && this.ai.killerForMurders.Count > 0 && amount < 0f)
		{
			if (this.ai.killerForMurders.Exists((MurderController.Murder item) => item.state == MurderController.MurderState.executing || item.state == MurderController.MurderState.travellingTo || item.state == MurderController.MurderState.post || item.state == MurderController.MurderState.escaping))
			{
				return;
			}
		}
		this.currentNerve += amount;
		this.currentNerve = Mathf.Clamp(this.currentNerve, 0f, this.maxNerve);
		if (scaredBy != null && amount < 0f)
		{
			this.lastScaredBy = scaredBy;
			if (this.lastScaredBy != null)
			{
				this.lastScaredAt = this.lastScaredBy.currentGameLocation;
				if (this.lastScaredBy.currentGameLocation != null && this.lastScaredBy.currentGameLocation.thisAsAddress == null && this.currentGameLocation.thisAsAddress != null)
				{
					this.lastScaredAt = this.currentGameLocation.thisAsAddress;
				}
			}
		}
		if (this.currentNerve <= 0f && num != this.currentNerve)
		{
			this.OnZeroNerveReached();
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0001B26C File Offset: 0x0001946C
	public virtual void SetNerve(float amount)
	{
		this.currentNerve = amount;
		this.currentNerve = Mathf.Clamp(this.currentNerve, 0f, this.maxNerve);
		if (this.ai != null && this.ai.killerForMurders.Count > 0 && amount < 0f)
		{
			if (this.ai.killerForMurders.Exists((MurderController.Murder item) => item.state == MurderController.MurderState.executing || item.state == MurderController.MurderState.travellingTo || item.state == MurderController.MurderState.post || item.state == MurderController.MurderState.escaping))
			{
				this.currentNerve = this.maxNerve;
			}
		}
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0001B302 File Offset: 0x00019502
	public virtual void OnZeroHealthReached()
	{
		if (this.isAsleep)
		{
			this.WakeUp(false);
		}
		Game.Log("Zero health reached: " + base.name, 2);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0001B329 File Offset: 0x00019529
	public virtual void ResetHealthToMaximum()
	{
		this.AddHealth(1E+14f, true, false);
	}

	// Token: 0x060002BA RID: 698 RVA: 0x0001B338 File Offset: 0x00019538
	public virtual void ResetNerveToMaximum()
	{
		this.AddNerve(1E+14f, null);
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0001B348 File Offset: 0x00019548
	public virtual void OnZeroNerveReached()
	{
		if (this.isAsleep)
		{
			this.WakeUp(false);
		}
		Game.Log("Zero nerve reached: " + base.name, 2);
		if (this.ai != null)
		{
			this.ai.AITick(true, false);
		}
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0001B395 File Offset: 0x00019595
	public virtual void SetMaxHealth(float newMax, bool setToMax = false)
	{
		this.maximumHealth = Mathf.Max(newMax, 0.01f);
		if (setToMax)
		{
			this.ResetHealthToMaximum();
		}
	}

	// Token: 0x060002BD RID: 701 RVA: 0x0001B3B1 File Offset: 0x000195B1
	public virtual void SetMaxNerve(float newMax, bool setToMax = false)
	{
		this.maxNerve = Mathf.Max(newMax, 0.01f);
		if (setToMax)
		{
			this.ResetNerveToMaximum();
		}
	}

	// Token: 0x060002BE RID: 702 RVA: 0x0001B3CD File Offset: 0x000195CD
	public virtual void SetRecoveryRate(float newRate)
	{
		this.recoveryRate = Mathf.Clamp(newRate, 0.01f, 100f);
	}

	// Token: 0x060002BF RID: 703 RVA: 0x0001B3E5 File Offset: 0x000195E5
	public virtual void SetCombatSkill(float newSkill)
	{
		this.combatSkill = Mathf.Clamp(newSkill, 0.01f, 1f);
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0001B3FD File Offset: 0x000195FD
	public virtual void SetCombatHeft(float newHeft)
	{
		this.combatHeft = Mathf.Clamp(newHeft, 0f, 1f);
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0001B415 File Offset: 0x00019615
	public void SetInBed(bool newVal)
	{
		this.isInBed = newVal;
		if (this.animationController != null)
		{
			this.animationController.SetInBed(this.isInBed, false);
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0001B440 File Offset: 0x00019640
	public virtual void UpdateCurrentNodeSpace()
	{
		if (!this.visible)
		{
			return;
		}
		if (this.currentNode != null)
		{
			NewNode.NodeSpace nodeSpace = null;
			float num = float.PositiveInfinity;
			foreach (KeyValuePair<Vector3, NewNode.NodeSpace> keyValuePair in this.currentNode.walkableNodeSpace)
			{
				if (keyValuePair.Value.occ == NewNode.NodeSpaceOccupancy.empty || keyValuePair.Value.occupier == this)
				{
					float num2 = Vector3.Distance(keyValuePair.Value.position, base.transform.position);
					if (num2 < num)
					{
						nodeSpace = keyValuePair.Value;
						num = num2;
					}
				}
			}
			if (nodeSpace != this.currentNodeSpace && this.currentNodeSpace != null)
			{
				this.currentNodeSpace.SetEmpty();
			}
			this.currentNodeSpace = nodeSpace;
			if (this.currentNodeSpace != null)
			{
				nodeSpace.SetOccuppier(this, NewNode.NodeSpaceOccupancy.position);
			}
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0001B530 File Offset: 0x00019730
	public virtual void AddReservedNodeSpace(NewNode.NodeSpace newSpace)
	{
		if (!this.reservedNodeSpace.Contains(newSpace))
		{
			this.reservedNodeSpace.Add(newSpace);
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0001B550 File Offset: 0x00019750
	public virtual void RemoveReservedNodeSpace()
	{
		foreach (NewNode.NodeSpace nodeSpace in this.reservedNodeSpace)
		{
			nodeSpace.SetEmpty();
		}
		this.reservedNodeSpace.Clear();
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0001B5AC File Offset: 0x000197AC
	public virtual void UpdateTrespassing(bool allowEnforcersEverywhere)
	{
		bool enforcersAllowedEverywhere = false;
		if (this.ai != null && this.ai.currentGoal != null)
		{
			enforcersAllowedEverywhere = this.ai.currentGoal.preset.allowEnforcersEverywhere;
		}
		string text;
		this.isTrespassing = this.IsTrespassing(this.currentRoom, out this.trespassingEscalation, out text, enforcersAllowedEverywhere);
		this.illegalAreaActive = this.isTrespassing;
		this.UpdateIllegalStatus();
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0001B61C File Offset: 0x0001981C
	public virtual void SightingCheck(float fov, bool ignoreLightAndStealth = false)
	{
		this.seenIllegalThisCheck.Clear();
		if (!this.seesOthers || this.isDead || this.isAsleep)
		{
			this.ClearSeesIllegal();
			return;
		}
		float num = SessionData.Instance.gameTime - this.timeOfLastSightCheck;
		if (num <= 0f)
		{
			return;
		}
		Human human = this as Human;
		Dictionary<Actor, float> dictionary = new Dictionary<Actor, float>();
		float num2 = fov * 0.5f;
		List<Actor> list = new List<Actor>();
		for (int i = 0; i < CityData.Instance.visibleActors.Count; i++)
		{
			Actor actor = CityData.Instance.visibleActors[i];
			if (!(actor == this) && actor.isSeenByOthers && (!Game.Instance.invisiblePlayer || !actor.isPlayer) && !actor.inAirVent && (!actor.isPlayer || !Player.Instance.playerKOInProgress) && (!(actor.ai != null) || !actor.ai.restrained) && (!actor.isHiding || (actor.isPlayer && Player.Instance.spottedWhileHiding.Contains(this))))
			{
				float num3 = Vector3.Distance(this.lookAtThisTransform.position, actor.lookAtThisTransform.position);
				if (!ignoreLightAndStealth)
				{
					actor.UpdateOverallVisibility();
				}
				float num4 = Mathf.Min(GameplayControls.Instance.citizenSightRange, actor.stealthDistance);
				if (this.isMachine)
				{
					num4 = Mathf.Min(GameplayControls.Instance.securitySightRange, actor.stealthDistance);
				}
				if (num3 <= num4)
				{
					float num5 = Vector3.Angle(actor.transform.position - this.lookAtThisTransform.position, this.lookAtThisTransform.forward);
					if (num5 >= -num2 && num5 <= num2 && !dictionary.ContainsKey(actor))
					{
						dictionary.Add(actor, num3);
					}
				}
			}
		}
		foreach (KeyValuePair<Actor, float> keyValuePair in dictionary)
		{
			if (!(keyValuePair.Key == this))
			{
				if (human != null && !human.isDead && human.ai != null && (human.ai.currentAction == null || !human.ai.currentAction.preset.disableSightingUpdates))
				{
					human.UpdateLastSighting(keyValuePair.Key as Human, false, 0);
				}
				bool flag = false;
				RaycastHit raycastHit = default(RaycastHit);
				bool drawLine = false;
				if ((keyValuePair.Key.isDead || keyValuePair.Key.isStunned) && !keyValuePair.Key.unreportable && (this.currentRoom == keyValuePair.Key.currentRoom || keyValuePair.Value <= 6.5f) && (this.ai == null || this.ai.attackTarget != keyValuePair.Key) && keyValuePair.Key.currentGameLocation != null && !keyValuePair.Key.currentGameLocation.isCrimeScene)
				{
					flag = true;
				}
				if (!flag && this.ai != null && this.ai.currentAction != null && this.ai.currentAction.preset.spookAction && keyValuePair.Key.isPlayer)
				{
					flag = true;
				}
				if (!this.isMachine && (keyValuePair.Key.isDead || keyValuePair.Key.isStunned) && ((this.ai != null && this.ai.inCombat && this.ai.attackTarget == keyValuePair.Key) || this.ai.persuitTarget == keyValuePair.Key || this.seesIllegal.ContainsKey(keyValuePair.Key)) && this.speechController != null)
				{
					if (this.seesIllegal.ContainsKey(keyValuePair.Key))
					{
						if (this.speechController.speechQueue.Count <= 0)
						{
							this.speechController.TriggerBark(SpeechController.Bark.targetDown);
						}
						this.RemoveFromSeesIllegal(keyValuePair.Key, 0f);
					}
					if (keyValuePair.Key == this.ai.attackTarget || keyValuePair.Key == this.ai.persuitTarget)
					{
						this.ai.CancelPersue();
					}
					if (this.seesIllegal.Count <= 0)
					{
						this.ai.CancelCombat();
					}
				}
				if (keyValuePair.Value < GameplayControls.Instance.minimumStealthDetectionRange)
				{
					if (Game.Instance.collectDebugData)
					{
						string text = "Seen: ";
						Actor key = keyValuePair.Key;
						this.SelectedDebug(text + ((key != null) ? key.ToString() : null) + " distance: " + keyValuePair.Value.ToString(), Actor.HumanDebug.sight);
					}
					flag = true;
				}
				else if (this.ActorRaycastCheck(keyValuePair.Key, keyValuePair.Value + 3f, out raycastHit, drawLine, Color.green, Color.red, Color.white, 1f))
				{
					if (Game.Instance.collectDebugData)
					{
						string text2 = "Seen: ";
						Actor key2 = keyValuePair.Key;
						this.SelectedDebug(text2 + ((key2 != null) ? key2.ToString() : null) + " distance: " + keyValuePair.Value.ToString(), Actor.HumanDebug.sight);
					}
					flag = true;
				}
				if (flag)
				{
					if (!keyValuePair.Key.isMachine)
					{
						this.OnAddTrackedTarget(keyValuePair.Key);
					}
					bool allowEnforcersEverywhere = false;
					if (keyValuePair.Key.ai != null && keyValuePair.Key.ai.currentGoal != null)
					{
						allowEnforcersEverywhere = keyValuePair.Key.ai.currentGoal.preset.allowEnforcersEverywhere;
					}
					keyValuePair.Key.UpdateTrespassing(allowEnforcersEverywhere);
					bool flag2 = false;
					if (keyValuePair.Key.isEnforcer)
					{
						if (keyValuePair.Key.isOnDuty)
						{
							flag2 = true;
						}
						else
						{
							Human human2 = keyValuePair.Key as Human;
							if (human2 != null && human2.outfitController != null && (human2.outfitController.currentOutfit == ClothesPreset.OutfitCategory.work || human2.outfitController.currentOutfit == ClothesPreset.OutfitCategory.outdoorsWork))
							{
								flag2 = true;
							}
						}
					}
					bool flag3 = false;
					if (this.isMachine && this.interactable != null && this.interactable.node != null)
					{
						flag3 = this.interactable.node.gameLocation.IsAlarmSystemTarget(keyValuePair.Key as Human);
						if (flag3)
						{
							Game.Log(string.Concat(new string[]
							{
								"Machine illegal: ",
								keyValuePair.Key.name,
								": ",
								flag3.ToString(),
								" mode: ",
								this.interactable.node.building.targetMode.ToString()
							}), 2);
						}
					}
					if (this.ai != null && !this.isMachine && (keyValuePair.Key.isStunned || keyValuePair.Key.isDead) && !keyValuePair.Key.unreportable)
					{
						if (!this.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.findDeadBody))
						{
							Human other = keyValuePair.Key as Human;
							if (other != null && (other.death == null || !other.death.isDead || !other.death.reported) && (other.death == null || !other.death.isDead || other.death.GetKiller() != this) && (other.ai == null || (!other.ai.isTripping && other.drunk <= 0.2f)) && this.ai != null && !this.ai.killerForMurders.Exists((MurderController.Murder item) => item.victim == other))
							{
								Game.Log(string.Concat(new string[]
								{
									base.name,
									" Sees downed body of ",
									other.GetCitizenName(),
									" at ",
									other.currentRoom.GetName()
								}), 2);
								this.ai.CreateNewGoal(RoutineControls.Instance.findDeadBody, 0f, 0f, null, keyValuePair.Key.interactable, null, null, null, 0);
								if (MurderController.Instance.playerAcceptedCoverUp)
								{
									MurderController.Instance.CitizenHasSeenBody(other, this as Human);
								}
							}
							Human human3 = this as Human;
							if (!(human3 != null) || (other.death != null && other.death.isDead && !(other.death.GetKiller() != this)))
							{
								continue;
							}
							Acquaintance acquaintance = null;
							if (!human3.FindAcquaintanceExists(keyValuePair.Key as Human, out acquaintance) || (acquaintance.known <= SocialControls.Instance.knowMournThreshold && acquaintance.connections[0] != Acquaintance.ConnectionType.lover && acquaintance.connections[0] != Acquaintance.ConnectionType.paramour && acquaintance.connections[0] != Acquaintance.ConnectionType.housemate))
							{
								continue;
							}
							NewAIGoal newAIGoal = this.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.mourn);
							if (newAIGoal == null)
							{
								this.ai.CreateNewGoal(RoutineControls.Instance.mourn, 0f, 0f, null, null, null, null, null, -2);
								continue;
							}
							newAIGoal.activeTime = 0f;
							continue;
						}
					}
					if ((keyValuePair.Key.illegalStatus && !keyValuePair.Key.isStunned && !keyValuePair.Key.isDead && !flag2) || (!this.isMachine && this.ai != null && this.ai.persuitTarget == keyValuePair.Key) || ((this.isMachine && flag3) || (keyValuePair.Key.isPlayer && this.ai != null && (this.ai.spookCounter >= 2 || (this.ai.currentAction != null && this.ai.currentAction.preset.spookAction)))) || (keyValuePair.Key.isPlayer && this.currentBuilding != null && this.currentBuilding.wantedInBuilding > SessionData.Instance.gameTime && (this.locationsOfAuthority.Contains(keyValuePair.Key.currentGameLocation) || this.isMachine || (this.isEnforcer && this.isOnDuty))))
					{
						list.Add(keyValuePair.Key);
						float num6 = Player.Instance.stealthDistance;
						if (this.isMachine)
						{
							num6 = GameplayControls.Instance.securitySightRange;
						}
						else
						{
							num6 = GameplayControls.Instance.citizenSightRange;
						}
						float num7 = Mathf.Lerp(num6 * 0.75f, num6, (float)this.escalationLevel / 2f);
						if (raycastHit.distance <= num7)
						{
							if (Game.Instance.collectDebugData)
							{
								this.SelectedDebug(keyValuePair.Key.name + " seen distance: " + raycastHit.distance.ToString(), Actor.HumanDebug.sight);
							}
							if (this.ai != null)
							{
								this.ai.SetUpdateEnabled(true);
							}
							float num8 = keyValuePair.Value / GameplayControls.Instance.citizenSightRange;
							float num9 = Mathf.Lerp(CitizenControls.Instance.persuitTimerThreshold.x, CitizenControls.Instance.persuitTimerThreshold.y, num8);
							int num10 = 0;
							if ((!this.isMachine && keyValuePair.Key.illegalAreaActive) || (this.isMachine && flag3))
							{
								num10 = keyValuePair.Key.trespassingEscalation;
								if (num10 >= 2 || this.escalationLevel >= 2 || (this.isMachine && flag3))
								{
									if (Game.Instance.collectDebugData)
									{
										this.SelectedDebug(string.Concat(new string[]
										{
											"Add sight using: >Current Time MP: ",
											SessionData.Instance.currentTimeMultiplier.ToString(),
											" >Sight Addition (Ratio ",
											num8.ToString(),
											"): ",
											num9.ToString(),
											" >SpotFocusSpeed: ",
											this.spotFocusSpeedMultiplier.ToString()
										}), Actor.HumanDebug.sight);
									}
									this.AddToSeesIllegal(keyValuePair.Key, SessionData.Instance.currentTimeMultiplier * num9 * this.spotFocusSpeedMultiplier);
									this.seenIllegalThisCheck.Add(keyValuePair.Key);
									if (keyValuePair.Key.isPlayer && AchievementsController.Instance != null && AchievementsController.Instance.privateSlyFlag > -1)
									{
										AchievementsController.Instance.privateSlyFlag = -1;
									}
								}
								else if (this.ai != null)
								{
									this.currentRoom.gameLocation.AddEscalation(keyValuePair.Key);
									num10 += this.currentRoom.gameLocation.GetAdditionalEscalation(keyValuePair.Key);
									if (Game.Instance.collectDebugData)
									{
										this.SelectedDebug(string.Concat(new string[]
										{
											"AI: Get escalation for ",
											keyValuePair.Key.name,
											" in ",
											this.currentRoom.name,
											": ",
											num10.ToString()
										}), Actor.HumanDebug.sight);
									}
									if (keyValuePair.Key.isPlayer)
									{
										if (Player.Instance.currentGameLocation.thisAsAddress != null)
										{
											StatusController.Instance.ConfirmFine(Player.Instance.currentGameLocation.thisAsAddress, null, StatusController.CrimeType.trespassing);
										}
										if (AchievementsController.Instance != null && AchievementsController.Instance.privateSlyFlag > -1)
										{
											AchievementsController.Instance.privateSlyFlag = -1;
										}
									}
									if (this.speechController != null && this.speechController.speechQueue.Count <= 0)
									{
										if (keyValuePair.Key.isPlayer && Player.Instance.currentGameLocation.playerLoiteringTimer >= GameplayControls.Instance.loiteringTrespassThreshold)
										{
											this.speechController.TriggerBark(SpeechController.Bark.trespassLoiter);
										}
										else if (this.currentGameLocation != null && this.currentGameLocation.thisAsAddress != null && this.currentGameLocation.thisAsAddress.company != null && !this.currentGameLocation.thisAsAddress.company.openForBusinessDesired && this.locationsOfAuthority.Contains(this.currentGameLocation))
										{
											this.speechController.TriggerBark(SpeechController.Bark.trespassClosed);
										}
										else
										{
											this.speechController.TriggerBark(SpeechController.Bark.trespass);
										}
									}
								}
							}
							else if (this.isMachine)
							{
								if (keyValuePair.Key.illegalAreaActive && keyValuePair.Key.currentGameLocation != null && keyValuePair.Key.currentGameLocation.thisAsAddress != null && keyValuePair.Key.currentGameLocation.thisAsAddress.company != null && !keyValuePair.Key.currentGameLocation.thisAsAddress.company.openForBusinessDesired && keyValuePair.Key.currentGameLocation.thisAsAddress.company.openForBusinessActual)
								{
									continue;
								}
								num10 = 2;
								if (Game.Instance.collectDebugData)
								{
									this.SelectedDebug(string.Concat(new string[]
									{
										"Add sight using: >Current Time MP: ",
										SessionData.Instance.currentTimeMultiplier.ToString(),
										" >Sight Addition (Ratio ",
										num8.ToString(),
										"): ",
										num9.ToString(),
										" >SpotFocusSpeed: ",
										this.spotFocusSpeedMultiplier.ToString()
									}), Actor.HumanDebug.sight);
								}
								this.AddToSeesIllegal(keyValuePair.Key, SessionData.Instance.currentTimeMultiplier * num9 * this.spotFocusSpeedMultiplier);
								this.seenIllegalThisCheck.Add(keyValuePair.Key);
							}
							else if (keyValuePair.Key.illegalActionActive || (this.ai != null && this.ai.persuitTarget == keyValuePair.Key))
							{
								num10 = 2;
								if (Game.Instance.collectDebugData)
								{
									this.SelectedDebug(string.Concat(new string[]
									{
										"Add sight using: >Current Time MP: ",
										SessionData.Instance.currentTimeMultiplier.ToString(),
										" >Sight Addition (Ratio ",
										num8.ToString(),
										"): ",
										num9.ToString(),
										" >SpotFocusSpeed: ",
										this.spotFocusSpeedMultiplier.ToString()
									}), Actor.HumanDebug.sight);
								}
								this.AddToSeesIllegal(keyValuePair.Key, SessionData.Instance.currentTimeMultiplier * num9 * this.spotFocusSpeedMultiplier);
								this.seenIllegalThisCheck.Add(keyValuePair.Key);
								if (keyValuePair.Key.isPlayer && MurderController.Instance.playerAcceptedCoverUp)
								{
									MurderController.Instance.CitizenHasSeenBody(null, this as Human);
								}
								if (Game.Instance.printDebug && keyValuePair.Key.isPlayer)
								{
									Game.Log(base.name + " seen player illegal action!", 2);
								}
							}
							else if (keyValuePair.Key.isPlayer && this.ai != null && (this.ai.spookCounter >= 2 || (this.ai.currentAction != null && this.ai.currentAction.preset.spookAction)))
							{
								Game.Log("Seen: Spooked behaviour", 2);
								float num11 = SessionData.Instance.currentTimeMultiplier * num9 * this.spotFocusSpeedMultiplier;
								this.ai.AddSpooked(num11 * (0.05f * (float)this.ai.spookCounter));
								num10 = this.ai.spookCounter;
								if (!this.seesIllegal.ContainsKey(keyValuePair.Key))
								{
									this.speechController.TriggerBark(SpeechController.Bark.spookConfront);
								}
								this.AddToSeesIllegal(keyValuePair.Key, num11 * (0.05f * (float)this.ai.spookCounter));
								this.seenIllegalThisCheck.Add(keyValuePair.Key);
								this.AddNerve(num11 * -0.125f, null);
							}
							else if (num10 < 2)
							{
								continue;
							}
							if (!keyValuePair.Key.illegalAreaActive || !this.isMachine || !(keyValuePair.Key.currentGameLocation != null) || !(keyValuePair.Key.currentGameLocation.thisAsAddress != null) || keyValuePair.Key.currentGameLocation.thisAsAddress.company == null || keyValuePair.Key.currentGameLocation.thisAsAddress.company.openForBusinessDesired || !keyValuePair.Key.currentGameLocation.thisAsAddress.company.openForBusinessActual)
							{
								if (keyValuePair.Key.isPlayer)
								{
									InterfaceController.Instance.CrosshairReaction();
									if (Player.Instance.searchInteractable != null && Player.Instance.searchInteractable.isActor != null)
									{
										Human human4 = Player.Instance.searchInteractable.isActor as Human;
										if (human4 != null && human4 != this && human4.ai != null)
										{
											human4.ai.SetPersue(Player.Instance, true, 2, true, CitizenControls.Instance.punchedResponseRange);
										}
									}
								}
								this.OnInvestigate(keyValuePair.Key, num10);
							}
						}
					}
				}
			}
		}
		if (this.seesIllegal.Count > 0)
		{
			foreach (Actor actor2 in new List<Actor>(this.seesIllegal.Keys))
			{
				if (!list.Contains(actor2))
				{
					this.RemoveFromSeesIllegal(actor2, num * SessionData.Instance.currentTimeMultiplier * CitizenControls.Instance.persuitForgetThreshold * this.spotLoseFocusSpeedMultiplier * (3f - (float)this.escalationLevel));
				}
			}
			if (this.ai != null && this.ai.tickRate != NewAIController.AITickRate.veryHigh)
			{
				this.ai.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, false);
			}
		}
		if (this.ai != null)
		{
			this.ai.UpdateTrackedTargets();
		}
		this.timeOfLastSightCheck = SessionData.Instance.gameTime;
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0001CBFC File Offset: 0x0001ADFC
	public virtual bool CanISee(Interactable interactable)
	{
		float num = GameplayControls.Instance.citizenSightRange;
		if (this.isMachine)
		{
			num = GameplayControls.Instance.securitySightRange;
		}
		float num2 = GameplayControls.Instance.citizenFOV;
		if (this.isMachine)
		{
			num2 = GameplayControls.Instance.securityFOV;
		}
		if (interactable.node != null)
		{
			if (!interactable.node.room.mainLightStatus && !interactable.node.room.GetSecondaryLightStatus())
			{
				num *= 0.1f;
			}
			else if (!interactable.node.room.mainLightStatus && interactable.node.room.GetSecondaryLightStatus())
			{
				num *= 0.5f;
			}
		}
		if (Vector3.Distance(interactable.wPos, this.lookAtThisTransform.position) <= num)
		{
			float num3 = num2 * 0.5f;
			Vector3 vector = interactable.wPos - this.lookAtThisTransform.position;
			float num4 = Vector3.Angle(vector, this.lookAtThisTransform.forward);
			if (num4 >= -num3 && num4 <= num3)
			{
				if (!(interactable.controller != null))
				{
					return true;
				}
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(this.lookAtThisTransform.position, vector), ref raycastHit, num, Toolbox.Instance.aiSightingLayerMask))
				{
					InteractableController component = raycastHit.transform.gameObject.GetComponent<InteractableController>();
					if (component != null && component.interactable == interactable)
					{
						return true;
					}
					foreach (InteractableController interactableController in raycastHit.transform.gameObject.GetComponentsInChildren<InteractableController>())
					{
						if (interactableController != null && interactableController.interactable == interactable)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x0001CDB0 File Offset: 0x0001AFB0
	public bool ActorRaycastCheck(Actor other, float maxRange, out RaycastHit hit, bool drawLine = false, Color lineSuccess = default(Color), Color lineFail = default(Color), Color lineNothing = default(Color), float lineTime = 1f)
	{
		bool result = false;
		hit = default(RaycastHit);
		for (int i = 0; i < 2; i++)
		{
			Vector3 position = this.lookAtThisTransform.position;
			Vector3 vector = other.lookAtThisTransform.position;
			if (i == 1)
			{
				vector = other.transform.position + new Vector3(0f, 0.5f, 0f);
			}
			Vector3 vector2 = vector - position;
			if (Physics.Raycast(new Ray(position, vector2), ref hit, maxRange, Toolbox.Instance.aiSightingLayerMask, 2))
			{
				if (hit.transform.parent == other.transform || hit.transform == other.lookAtThisTransform || hit.transform == other.transform)
				{
					if (drawLine)
					{
						Debug.DrawLine(position, hit.point, lineSuccess, lineTime);
					}
					if (other.isDead || other.isStunned)
					{
						return true;
					}
					Vector3 vector3 = vector;
					Vector3 vector4 = position - vector3;
					RaycastHit raycastHit;
					if (Physics.Raycast(new Ray(vector3, vector4), ref raycastHit, maxRange, Toolbox.Instance.aiSightingLayerMask))
					{
						if (raycastHit.transform.parent == base.transform || raycastHit.transform == this.lookAtThisTransform || raycastHit.transform == base.transform)
						{
							if (drawLine)
							{
								Debug.DrawLine(vector3, raycastHit.point, lineSuccess, lineTime);
							}
							return true;
						}
						if (drawLine)
						{
							Debug.DrawLine(vector3, raycastHit.point, lineFail, lineTime);
						}
					}
					else if (drawLine)
					{
						Debug.DrawRay(vector3, vector4.normalized * maxRange, lineNothing, lineTime);
					}
				}
				else if (drawLine)
				{
					Debug.DrawLine(position, hit.point, lineFail, lineTime);
				}
			}
			else if (drawLine)
			{
				Debug.DrawRay(position, vector2.normalized * maxRange, lineNothing, lineTime);
			}
		}
		return result;
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0001CF94 File Offset: 0x0001B194
	public virtual void OnInvestigate(Actor newTarget, int escalation)
	{
		if (this.ai != null)
		{
			this.ai.Investigate(newTarget.currentNode, newTarget.transform.position, newTarget, NewAIController.ReactionState.investigatingSight, CitizenControls.Instance.sightingMinInvestigationTimeMP, escalation, false, 1f, null);
		}
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0001CFDF File Offset: 0x0001B1DF
	public virtual void OnAddTrackedTarget(Actor newTarget)
	{
		if (this.ai != null)
		{
			this.ai.AddTrackedTarget(newTarget);
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0001CFFC File Offset: 0x0001B1FC
	public virtual void AddToSeesIllegal(Actor newTarget, float focus)
	{
		if (!this.seesIllegal.ContainsKey(newTarget))
		{
			this.seesIllegal.Add(newTarget, 0f);
		}
		Dictionary<Actor, float> dictionary = this.seesIllegal;
		dictionary[newTarget] += focus;
		this.seesIllegal[newTarget] = Mathf.Clamp01(this.seesIllegal[newTarget]);
		if (Game.Instance.collectDebugData)
		{
			this.SelectedDebug(string.Concat(new string[]
			{
				"Add sight focus target ",
				newTarget.name,
				": ",
				focus.ToString(),
				", total: ",
				this.seesIllegal[newTarget].ToString()
			}), Actor.HumanDebug.sight);
		}
		if (newTarget.isPlayer)
		{
			this.debugSeesPlayer = Mathf.RoundToInt(this.seesIllegal[newTarget] * 100f);
			this.debugLastSeesPlayerChange = focus;
			if (this.ai != null)
			{
				this.ai.debugSeesPlayer = this.debugSeesPlayer;
				this.ai.debugLastSeesPlayerChange = this.debugLastSeesPlayerChange;
			}
		}
		if (!newTarget.witnessesToIllegalActivity.Contains(this))
		{
			newTarget.witnessesToIllegalActivity.Add(this);
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0001D138 File Offset: 0x0001B338
	public virtual void RemoveFromSeesIllegal(Actor newTarget, float focus)
	{
		if (this.seesIllegal.ContainsKey(newTarget))
		{
			Dictionary<Actor, float> dictionary = this.seesIllegal;
			dictionary[newTarget] -= focus;
			if (newTarget.isPlayer)
			{
				this.debugSeesPlayer = Mathf.RoundToInt(this.seesIllegal[newTarget] * 100f);
				this.debugLastSeesPlayerChange = -focus;
				if (this.ai != null)
				{
					this.ai.debugSeesPlayer = this.debugSeesPlayer;
					this.ai.debugLastSeesPlayerChange = this.debugLastSeesPlayerChange;
				}
			}
			try
			{
				if (Game.Instance.collectDebugData)
				{
					this.SelectedDebug(string.Concat(new string[]
					{
						"Remove sight focus target ",
						newTarget.name,
						": ",
						focus.ToString(),
						", total: ",
						this.seesIllegal[newTarget].ToString()
					}), Actor.HumanDebug.sight);
				}
			}
			catch
			{
			}
			if (this.seesIllegal[newTarget] <= 0f)
			{
				this.seesIllegal.Remove(newTarget);
				newTarget.witnessesToIllegalActivity.Remove(this);
			}
		}
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0001D270 File Offset: 0x0001B470
	public virtual void AddPersuedBy(Actor newTarget)
	{
		if (!this.persuedBy.Contains(newTarget))
		{
			this.persuedBy.Add(newTarget);
			if (this.isPlayer)
			{
				InterfaceController.Instance.movieBarJuice.Pulsate(true, false);
				if (Player.Instance.combatSnapshot == null)
				{
					Player.Instance.combatSnapshot = AudioController.Instance.Play2DLooping(AudioControls.Instance.combatSnapshot, null, 1f);
				}
			}
			if (this.isPlayer || newTarget.isPlayer)
			{
				Player.Instance.StatusCheckEndOfFrame();
			}
		}
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0001D2FC File Offset: 0x0001B4FC
	public virtual void RemovePersuedBy(Actor newTarget)
	{
		if (this.persuedBy.Contains(newTarget))
		{
			this.persuedBy.Remove(newTarget);
			if (this.persuedBy.Count <= 0 && this.isPlayer)
			{
				InterfaceController.Instance.movieBarJuice.Pulsate(false, false);
				if (Player.Instance.combatSnapshot != null)
				{
					AudioController.Instance.StopSound(Player.Instance.combatSnapshot, AudioController.StopType.triggerCue, "Exit combat");
					Player.Instance.combatSnapshot = null;
				}
				if (AchievementsController.Instance != null && Player.Instance.isHiding && Player.Instance.hidingInteractable == null)
				{
					AchievementsController.Instance.UnlockAchievement("Thin Disguise", "hide_with_newspaper");
				}
			}
			if (this.isPlayer || newTarget.isPlayer)
			{
				Player.Instance.StatusCheckEndOfFrame();
			}
		}
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0001D3D5 File Offset: 0x0001B5D5
	public void HearIllegal(AudioEvent audioEvent, NewNode newInvestigateNode, Vector3 newInvestigatePosition, Actor newTarget, int escLevel)
	{
		if (this.ai != null)
		{
			if (newTarget != null && newTarget.isPlayer)
			{
				InterfaceController.Instance.CrosshairReaction();
			}
			this.ai.HearIllegal(audioEvent, newInvestigateNode, newInvestigatePosition, newTarget, escLevel);
		}
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0001D414 File Offset: 0x0001B614
	public virtual void ClearSeesIllegal()
	{
		foreach (Actor newTarget in new List<Actor>(this.seesIllegal.Keys))
		{
			this.RemoveFromSeesIllegal(newTarget, 1f);
		}
		this.debugSeesPlayer = 0;
		this.debugLastSeesPlayerChange = 0f;
		if (this.ai != null)
		{
			this.ai.debugSeesPlayer = this.debugSeesPlayer;
			this.ai.debugLastSeesPlayerChange = this.debugLastSeesPlayerChange;
		}
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0001D4B8 File Offset: 0x0001B6B8
	public virtual void SetEscalation(int newEsc)
	{
		if (this.escalationLevel != newEsc)
		{
			this.escalationLevel = newEsc;
			if (this.escalationLevel >= 2)
			{
				Game.Log("AI: " + base.name + " is at escalation level 2!", 2);
			}
			if (this.ai != null)
			{
				if (this.ai.currentAction != null)
				{
					this.ai.currentAction.UpdateCombatPose();
				}
				this.ai.TriggerReactionIndicator();
			}
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00002265 File Offset: 0x00000465
	public void SelectedDebug(string str, Actor.HumanDebug debug)
	{
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0001D530 File Offset: 0x0001B730
	public void SpottedByPlayer(float graceTimeMultiplier = 1f)
	{
		this.spottedState = 1f;
		this.spottedGraceTime = Mathf.Max(GameplayControls.Instance.spottedGraceTime * graceTimeMultiplier * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.securityGraceTimeModifier)), this.spottedGraceTime);
		if (this.ai != null)
		{
			this.ai.TriggerReactionIndicator();
		}
		if (!Player.Instance.spottedByPlayer.Contains(this))
		{
			Player.Instance.spottedByPlayer.Add(this);
			InterfaceController.Instance.footstepAudioIndicator.UpdateCurrentEvent();
			foreach (KeyValuePair<InteractablePreset.InteractionKey, InteractionController.InteractionSetting> keyValuePair in InteractionController.Instance.currentInteractions)
			{
				if (keyValuePair.Value.audioEvent != null && keyValuePair.Value.newUIRef != null && keyValuePair.Value.newUIRef.soundIndicator != null)
				{
					keyValuePair.Value.newUIRef.soundIndicator.UpdateCurrentEvent();
				}
			}
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0001D660 File Offset: 0x0001B860
	public void HeardByPlayer()
	{
		if (Player.Instance.illegalStatus || Player.Instance.witnessesToIllegalActivity.Contains(this))
		{
			this.spottedState = 1f;
			this.spottedGraceTime = Mathf.Max(GameplayControls.Instance.spottedGraceTime * GameplayControls.Instance.audioOnlySpotGraceTimeMultiplier, this.spottedGraceTime);
			if (this.ai != null)
			{
				this.ai.TriggerReactionIndicator();
			}
			if (!Player.Instance.spottedByPlayer.Contains(this))
			{
				Player.Instance.spottedByPlayer.Add(this);
				InterfaceController.Instance.footstepAudioIndicator.UpdateCurrentEvent();
				foreach (KeyValuePair<InteractablePreset.InteractionKey, InteractionController.InteractionSetting> keyValuePair in InteractionController.Instance.currentInteractions)
				{
					if (keyValuePair.Value.audioEvent != null && keyValuePair.Value.newUIRef != null && keyValuePair.Value.newUIRef.soundIndicator != null)
					{
						keyValuePair.Value.newUIRef.soundIndicator.UpdateCurrentEvent();
					}
				}
			}
		}
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0001D7A8 File Offset: 0x0001B9A8
	public virtual bool IsTrespassing(NewRoom room, out int trespassEscalation, out string debugOutput, bool enforcersAllowedEverywhere = true)
	{
		trespassEscalation = 0;
		debugOutput = string.Empty;
		return false;
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0001D7B5 File Offset: 0x0001B9B5
	public void AddLocationOfAuthorty(NewGameLocation newLoc)
	{
		if (!this.locationsOfAuthority.Contains(newLoc))
		{
			this.locationsOfAuthority.Add(newLoc);
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0001D7D2 File Offset: 0x0001B9D2
	public void RemoveLocationOfAuthority(NewGameLocation newLoc)
	{
		this.locationsOfAuthority.Remove(newLoc);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x0001D7E4 File Offset: 0x0001B9E4
	public virtual void UpdateIllegalStatus()
	{
		bool flag = false;
		if (this.illegalActionActive)
		{
			flag = true;
		}
		if (this.illegalAreaActive)
		{
			flag = true;
		}
		if (this.forceTarget)
		{
			flag = true;
		}
		if (flag != this.illegalStatus)
		{
			this.illegalStatus = flag;
		}
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0001D824 File Offset: 0x0001BA24
	[Button(null, 0)]
	public void ListSeesIllegal()
	{
		foreach (KeyValuePair<Actor, float> keyValuePair in this.seesIllegal)
		{
			Game.Log(string.Concat(new string[]
			{
				keyValuePair.Key.name,
				" ",
				keyValuePair.Key.transform.name,
				": ",
				keyValuePair.Value.ToString()
			}), 2);
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0001D8C8 File Offset: 0x0001BAC8
	[Button(null, 0)]
	public void ListWitnessToIllegal()
	{
		foreach (Actor actor in this.witnessesToIllegalActivity)
		{
			Game.Log(actor.name + " " + actor.transform.name, 2);
			if (actor.seesIllegal.ContainsKey(this))
			{
				Game.Log("Sees illegal: " + actor.seesIllegal[this].ToString(), 2);
			}
			else
			{
				Game.Log("Sees illegal missing!", 2);
			}
		}
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0001D974 File Offset: 0x0001BB74
	public bool HasBeenThrowOutOfWindow()
	{
		return this.previousNode != null && this.currentNode != null && this.previousNode.nodeCoord.z > 0 && (this.currentNode.floorType == NewNode.FloorTileType.CeilingOnly || this.currentNode.floorType == NewNode.FloorTileType.noneButIndoors || this.currentNode.floorType == NewNode.FloorTileType.none) && this.previousNode.walls.Exists((NewWall item) => item.otherWall.node == this.currentNode || item.node == this.currentNode);
	}

	// Token: 0x040001D6 RID: 470
	[Header("Flags")]
	public bool isMoving;

	// Token: 0x040001D7 RID: 471
	public bool isRunning;

	// Token: 0x040001D8 RID: 472
	public bool isMachine;

	// Token: 0x040001D9 RID: 473
	public bool isPlayer;

	// Token: 0x040001DA RID: 474
	public bool isAsleep;

	// Token: 0x040001DB RID: 475
	public bool isDelayed;

	// Token: 0x040001DC RID: 476
	public bool isStunned;

	// Token: 0x040001DD RID: 477
	public bool isDead;

	// Token: 0x040001DE RID: 478
	public bool unreportable;

	// Token: 0x040001DF RID: 479
	public bool isTrespassing;

	// Token: 0x040001E0 RID: 480
	public bool isOnStreet;

	// Token: 0x040001E1 RID: 481
	public bool seesOthers = true;

	// Token: 0x040001E2 RID: 482
	public bool isSeenByOthers = true;

	// Token: 0x040001E3 RID: 483
	public bool canListen = true;

	// Token: 0x040001E4 RID: 484
	public bool visible = true;

	// Token: 0x040001E5 RID: 485
	public bool isHome;

	// Token: 0x040001E6 RID: 486
	public bool isAtWork;

	// Token: 0x040001E7 RID: 487
	public bool inAirVent;

	// Token: 0x040001E8 RID: 488
	public bool isHiding;

	// Token: 0x040001E9 RID: 489
	public bool isInBed;

	// Token: 0x040001EA RID: 490
	public bool inConversation;

	// Token: 0x040001EB RID: 491
	public bool isSpeaking;

	// Token: 0x040001EC RID: 492
	public bool isHomeless;

	// Token: 0x040001ED RID: 493
	public bool isLitterBug;

	// Token: 0x040001EE RID: 494
	public bool isOnDuty;

	// Token: 0x040001EF RID: 495
	public bool isEnforcer;

	// Token: 0x040001F0 RID: 496
	public bool ownsUmbrella = true;

	// Token: 0x040001F1 RID: 497
	public bool likesTheRain;

	// Token: 0x040001F2 RID: 498
	public bool forceTarget;

	// Token: 0x040001F3 RID: 499
	[Header("Illegal State")]
	[ProgressBar("Sees Player", 100f, 9)]
	public int debugSeesPlayer;

	// Token: 0x040001F4 RID: 500
	[ReadOnly]
	public float debugLastSeesPlayerChange;

	// Token: 0x040001F5 RID: 501
	[Space(5f)]
	public Dictionary<Actor, float> seesIllegal = new Dictionary<Actor, float>();

	// Token: 0x040001F6 RID: 502
	public HashSet<Actor> seenIllegalThisCheck = new HashSet<Actor>();

	// Token: 0x040001F7 RID: 503
	public HashSet<Actor> witnessesToIllegalActivity = new HashSet<Actor>();

	// Token: 0x040001F8 RID: 504
	public HashSet<Actor> persuedBy = new HashSet<Actor>();

	// Token: 0x040001F9 RID: 505
	public bool illegalActionActive;

	// Token: 0x040001FA RID: 506
	public bool illegalAreaActive;

	// Token: 0x040001FB RID: 507
	public int trespassingEscalation;

	// Token: 0x040001FC RID: 508
	public bool illegalStatus;

	// Token: 0x040001FD RID: 509
	[Space(7f)]
	[Tooltip("Transform that should be in the centre of this object, when others look at this, they will use this.")]
	public Transform lookAtThisTransform;

	// Token: 0x040001FE RID: 510
	public Transform aimTransform;

	// Token: 0x040001FF RID: 511
	[Header("Common Components")]
	public GameObject modelParent;

	// Token: 0x04000200 RID: 512
	public GameObject distantLOD;

	// Token: 0x04000201 RID: 513
	public List<MeshRenderer> meshes = new List<MeshRenderer>();

	// Token: 0x04000202 RID: 514
	public List<MeshRenderer> meshesLOD1 = new List<MeshRenderer>();

	// Token: 0x04000203 RID: 515
	public CitizenAnimationController animationController;

	// Token: 0x04000204 RID: 516
	public SpeechController speechController;

	// Token: 0x04000205 RID: 517
	public Transform neckTransform;

	// Token: 0x04000206 RID: 518
	public InteractablePreset citizenObjectPreset;

	// Token: 0x04000207 RID: 519
	[NonSerialized]
	public Interactable interactable;

	// Token: 0x04000208 RID: 520
	[NonSerialized]
	public Interactable leftHandInteractable;

	// Token: 0x04000209 RID: 521
	[NonSerialized]
	public Interactable rightHandInteractable;

	// Token: 0x0400020A RID: 522
	public InteractableController interactableController;

	// Token: 0x0400020B RID: 523
	public NewAIController ai;

	// Token: 0x0400020C RID: 524
	public Transform footstepSoundTransform;

	// Token: 0x0400020D RID: 525
	public OutlineController outline;

	// Token: 0x0400020E RID: 526
	[ReadOnly]
	[Header("Health")]
	public float currentHealth = 1f;

	// Token: 0x0400020F RID: 527
	[ReadOnly]
	public float maximumHealth = 1f;

	// Token: 0x04000210 RID: 528
	[ReadOnly]
	public float currentHealthNormalized = 1f;

	// Token: 0x04000211 RID: 529
	[ReadOnly]
	public float recoveryRate = 0.5f;

	// Token: 0x04000212 RID: 530
	[Header("Combat")]
	[ReadOnly]
	public float combatSkill = 1f;

	// Token: 0x04000213 RID: 531
	[ReadOnly]
	public float combatHeft = 0.1f;

	// Token: 0x04000214 RID: 532
	[ReadOnly]
	public float currentNerve = 0.1f;

	// Token: 0x04000215 RID: 533
	[ReadOnly]
	public Actor lastScaredBy;

	// Token: 0x04000216 RID: 534
	[ReadOnly]
	public NewGameLocation lastScaredAt;

	// Token: 0x04000217 RID: 535
	[ReadOnly]
	public float maxNerve = 0.5f;

	// Token: 0x04000218 RID: 536
	[Header("Location Data")]
	[NonSerialized]
	public CityTile previousCityTile;

	// Token: 0x04000219 RID: 537
	[NonSerialized]
	public CityTile currentCityTile;

	// Token: 0x0400021A RID: 538
	[NonSerialized]
	public NewTile previousTile;

	// Token: 0x0400021B RID: 539
	[NonSerialized]
	public NewTile currentTile;

	// Token: 0x0400021C RID: 540
	[NonSerialized]
	public NewBuilding previousBuilding;

	// Token: 0x0400021D RID: 541
	[NonSerialized]
	public NewBuilding currentBuilding;

	// Token: 0x0400021E RID: 542
	[NonSerialized]
	public NewGameLocation previousGameLocation;

	// Token: 0x0400021F RID: 543
	[NonSerialized]
	public NewGameLocation currentGameLocation;

	// Token: 0x04000220 RID: 544
	[Space(4f)]
	[NonSerialized]
	public NewRoom previousRoom;

	// Token: 0x04000221 RID: 545
	public NewRoom currentRoom;

	// Token: 0x04000222 RID: 546
	[Space(4f)]
	[NonSerialized]
	public AirDuctGroup currentDuct;

	// Token: 0x04000223 RID: 547
	[NonSerialized]
	public AirDuctGroup previousDuct;

	// Token: 0x04000224 RID: 548
	[Space(4f)]
	public NewNode.NodeSpace currentNodeSpace;

	// Token: 0x04000225 RID: 549
	public HashSet<NewNode.NodeSpace> reservedNodeSpace = new HashSet<NewNode.NodeSpace>();

	// Token: 0x04000226 RID: 550
	[Space(4f)]
	public NewNode debugPrevNode2;

	// Token: 0x04000227 RID: 551
	public NewNode debugPrevNode1;

	// Token: 0x04000228 RID: 552
	public NewNode previousNode;

	// Token: 0x04000229 RID: 553
	public NewNode currentNode;

	// Token: 0x0400022A RID: 554
	[NonSerialized]
	public Vector3Int currentNodeCoord;

	// Token: 0x0400022B RID: 555
	[Space(7f)]
	public List<NewDoor> keyring = new List<NewDoor>();

	// Token: 0x0400022C RID: 556
	[NonSerialized]
	public EvidenceWitness evidenceEntry;

	// Token: 0x0400022D RID: 557
	[Header("Vision")]
	[ReadOnly]
	public float currentLightLevel = 1f;

	// Token: 0x0400022E RID: 558
	[NonSerialized]
	private float lightLevelTransition = 1f;

	// Token: 0x0400022F RID: 559
	[ReadOnly]
	public bool stealthMode;

	// Token: 0x04000230 RID: 560
	[ReadOnly]
	public bool isCrouched;

	// Token: 0x04000231 RID: 561
	[ReadOnly]
	public float appliedStealth;

	// Token: 0x04000232 RID: 562
	[ReadOnly]
	private float currentVisibilityPotential = 1f;

	// Token: 0x04000233 RID: 563
	[ReadOnly]
	public float overallVisibility = 1f;

	// Token: 0x04000234 RID: 564
	[ReadOnly]
	public float stealthDistance = 20f;

	// Token: 0x04000235 RID: 565
	[ReadOnly]
	public int escalationLevel;

	// Token: 0x04000236 RID: 566
	public float timeOfLastSightCheck;

	// Token: 0x04000237 RID: 567
	[NonSerialized]
	public float spottedState;

	// Token: 0x04000238 RID: 568
	[NonSerialized]
	public float spottedGraceTime;

	// Token: 0x04000239 RID: 569
	[Space(5f)]
	public float spotFocusSpeedMultiplier = 1f;

	// Token: 0x0400023A RID: 570
	public float spotLoseFocusSpeedMultiplier = 1f;

	// Token: 0x0400023B RID: 571
	public float hearingMultiplier = 1f;

	// Token: 0x0400023C RID: 572
	public HashSet<NewGameLocation> locationsOfAuthority = new HashSet<NewGameLocation>();

	// Token: 0x0400023D RID: 573
	[Header("Interaction")]
	[NonSerialized]
	public Interactable interactingWith;

	// Token: 0x0400023E RID: 574
	[Header("Inventory")]
	public List<Interactable> inventory = new List<Interactable>();

	// Token: 0x0400023F RID: 575
	[Header("Skill Variables")]
	public float stealthSkill = 1f;

	// Token: 0x04000240 RID: 576
	[NonSerialized]
	public float sleepDepth;

	// Token: 0x04000241 RID: 577
	[NonSerialized]
	public int awakenPromt;

	// Token: 0x04000242 RID: 578
	[NonSerialized]
	public float awakenRegen;

	// Token: 0x02000041 RID: 65
	public enum HumanDebug
	{
		// Token: 0x04000246 RID: 582
		movement,
		// Token: 0x04000247 RID: 583
		actions,
		// Token: 0x04000248 RID: 584
		attacks,
		// Token: 0x04000249 RID: 585
		updates,
		// Token: 0x0400024A RID: 586
		misc,
		// Token: 0x0400024B RID: 587
		sight
	}

	// Token: 0x02000042 RID: 66
	// (Invoke) Token: 0x060002DF RID: 735
	public delegate void InteractionChanged();

	// Token: 0x02000043 RID: 67
	// (Invoke) Token: 0x060002E3 RID: 739
	public delegate void RoutineChanged();
}
