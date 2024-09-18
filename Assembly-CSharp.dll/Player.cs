using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000036 RID: 54
public class Player : Human
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060001E1 RID: 481 RVA: 0x0000F8F0 File Offset: 0x0000DAF0
	// (remove) Token: 0x060001E2 RID: 482 RVA: 0x0000F928 File Offset: 0x0000DB28
	public event Player.TransitionCompleted OnTransitionCompleted;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060001E3 RID: 483 RVA: 0x0000F960 File Offset: 0x0000DB60
	// (remove) Token: 0x060001E4 RID: 484 RVA: 0x0000F998 File Offset: 0x0000DB98
	public event Player.StartAutoTravel OnExecuteAutoTravel;

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x060001E5 RID: 485 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
	// (remove) Token: 0x060001E6 RID: 486 RVA: 0x0000FA08 File Offset: 0x0000DC08
	public event Player.AutoTravelEnd OnEndAutoTravel;

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060001E7 RID: 487 RVA: 0x0000FA40 File Offset: 0x0000DC40
	// (remove) Token: 0x060001E8 RID: 488 RVA: 0x0000FA78 File Offset: 0x0000DC78
	public event Player.GameLocationChange OnNewGameLocation;

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060001E9 RID: 489 RVA: 0x0000FAAD File Offset: 0x0000DCAD
	public static Player Instance
	{
		get
		{
			return Player._instance;
		}
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000FAB4 File Offset: 0x0000DCB4
	private void Awake()
	{
		if (Player._instance != null && Player._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Player._instance = this;
		}
		this.normalStepOffset = this.charController.stepOffset;
		this.updateCullingAction = (Action)Delegate.Combine(this.updateCullingAction, new Action(this.UpdateCulling));
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000FB24 File Offset: 0x0000DD24
	private void Start()
	{
		if (!SessionData.Instance.isFloorEdit)
		{
			this.updateStatusAction = (Action)Delegate.Combine(this.updateStatusAction, new Action(StatusController.Instance.ForceStatusCheck));
		}
		if (!Game.Instance.freeCam)
		{
			this.SetPlayerHeight(Player.Instance.GetPlayerHeightNormal(), true);
		}
		this.SetCombatSkill(GameplayControls.Instance.playerCombatSkill);
		this.SetCombatHeft(GameplayControls.Instance.playerCombatHeft);
		UpgradeEffectController.Instance.OnSyncDiskChange(false);
		SessionData.Instance.OnPauseChange += this.OnPauseChange;
		this.isPlayer = true;
		this.humanID = 1;
		this.humility = 1f;
		this.emotionality = 0.5f;
		this.extraversion = 0.4f;
		this.agreeableness = 0.7f;
		this.conscientiousness = 1f;
		this.creativity = 0.5f;
		this.RestorePlayerMovementSpeed();
		this.UpdateSkinWidth();
		this.visible = true;
		CityData.Instance.visibleActors.Add(this);
		this.isCrunchingDatabase = false;
		this.SetFootwear(Human.ShoeType.normal);
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000FC42 File Offset: 0x0000DE42
	public void EnablePlayerMovement(bool val, bool updateCulling = true)
	{
		if (CutSceneController.Instance.cutSceneActive && val)
		{
			val = false;
		}
		Game.Log("Player: Movement: " + val.ToString(), 2);
		this.fps.enableMovement = val;
		if (updateCulling)
		{
			this.UpdateCullingOnEndOfFrame();
		}
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0000FC84 File Offset: 0x0000DE84
	public void EnablePlayerMouseLook(bool val, bool forceHideMouseOnDisable = false)
	{
		if (CutSceneController.Instance.cutSceneActive && val)
		{
			val = false;
		}
		bool cursorVisible = !val;
		if (!val && forceHideMouseOnDisable)
		{
			cursorVisible = false;
		}
		if (!SessionData.Instance.isFloorEdit)
		{
			Game.Log("Player: MouseLook: " + val.ToString() + " show mouse: " + cursorVisible.ToString(), 2);
		}
		this.fps.enableLook = val;
		InputController.Instance.SetCursorLock(val);
		InputController.Instance.SetCursorVisible(cursorVisible);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000FD04 File Offset: 0x0000DF04
	private void OnPauseChange(bool openDesktopMode)
	{
		if (InteractionController.Instance.interactionMode)
		{
			return;
		}
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (SessionData.Instance.play)
		{
			this.EnablePlayerMovement(this.pausedRememberPlayerMovement, true);
			InterfaceController.Instance.SetDesktopMode(false, true);
			return;
		}
		this.pausedRememberPlayerMovement = this.fps.enableMovement;
		this.EnablePlayerMovement(false, true);
		if (openDesktopMode)
		{
			bool showPanels = true;
			if (SessionData.Instance.isDecorEdit)
			{
				showPanels = false;
			}
			if (PlayerApartmentController.Instance.decoratingMode)
			{
				showPanels = false;
			}
			if (PlayerApartmentController.Instance.furniturePlacementMode)
			{
				showPanels = false;
			}
			InterfaceController.Instance.SetDesktopMode(true, showPanels);
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000FDA8 File Offset: 0x0000DFA8
	public override void UpdateGameLocation(float feetOffset = 0f)
	{
		base.UpdateGameLocation(feetOffset);
		if (this.inAirVent)
		{
			AirDuctGroup.AirDuctSection airDuctSection = null;
			float num = 9999f;
			if (this.currentNode != null)
			{
				foreach (AirDuctGroup.AirDuctSection airDuctSection2 in this.currentNode.airDucts)
				{
					Vector3 vector = CityData.Instance.NodeToRealpos(airDuctSection2.node.nodeCoord + new Vector3(0f, 0f, (float)airDuctSection2.level * 2f + 1f) / 6f) + new Vector3(0f, InteriorControls.Instance.airDuctYOffset, 0f);
					float num2 = Vector3.Distance(base.transform.position, vector);
					if (num2 < num)
					{
						airDuctSection = airDuctSection2;
						num = num2;
					}
				}
			}
			if (airDuctSection != null)
			{
				if (airDuctSection.group != this.currentDuct)
				{
					this.previousDuct = this.currentDuct;
					this.currentDuct = airDuctSection.group;
					this.OnDuctGroupChange();
				}
				if (airDuctSection != this.currentDuctSection)
				{
					this.previousDuctSection = this.currentDuctSection;
					this.currentDuctSection = airDuctSection;
					string[] array = new string[5];
					array[0] = "Player: Found new closest duct section at node position ";
					int num3 = 1;
					Vector3 position = airDuctSection.node.position;
					array[num3] = position.ToString();
					array[2] = " (current node: ";
					int num4 = 3;
					position = this.currentNode.position;
					array[num4] = position.ToString();
					array[4] = ")";
					Game.Log(string.Concat(array), 2);
					this.OnDuctSectionChange();
				}
			}
		}
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000FF6C File Offset: 0x0000E16C
	public virtual void OnDuctGroupChange()
	{
		this.UpdateCullingOnEndOfFrame();
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000FF74 File Offset: 0x0000E174
	public void OnDuctSectionChange()
	{
		if (this.inAirVent)
		{
			Game.Log("Player: Duct section change map update...", 2);
			List<AirDuctGroup.AirDuctSection> list = new List<AirDuctGroup.AirDuctSection>();
			List<Vector3> list2 = new List<Vector3>();
			list.Add(this.currentDuctSection);
			list2.Add(Vector3.zero);
			List<AirDuctGroup.AirDuctSection> list3 = new List<AirDuctGroup.AirDuctSection>();
			int num = 200;
			while (list.Count > 0 && num > 0)
			{
				AirDuctGroup.AirDuctSection airDuctSection = list[0];
				Vector3 vector = list2[0];
				List<Vector3Int> list4;
				List<AirDuctGroup.AirVent> list5;
				List<Vector3Int> list6;
				List<AirDuctGroup.AirDuctSection> neighborSections = airDuctSection.GetNeighborSections(out list4, out list5, out list6);
				foreach (AirDuctGroup.AirVent airVent in list5)
				{
					airVent.SetDiscovered(true);
					if (airVent.room.explorationLevel < 2 && this.currentDuctSection == airDuctSection)
					{
						airVent.room.SetExplorationLevel(2);
					}
				}
				for (int i = 0; i < neighborSections.Count; i++)
				{
					AirDuctGroup.AirDuctSection airDuctSection2 = neighborSections[i];
					Vector3 vector2 = list4[i];
					if ((airDuctSection == this.currentDuctSection || !(vector2 != vector)) && !list.Contains(airDuctSection2) && !list3.Contains(airDuctSection2))
					{
						list.Add(airDuctSection2);
						list2.Add(vector2);
					}
				}
				airDuctSection.SetDiscovered(true);
				list.RemoveAt(0);
				list2.RemoveAt(0);
				num--;
			}
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x000100EC File Offset: 0x0000E2EC
	public override void OnCityTileChange()
	{
		base.OnCityTileChange();
		if (this.previousCityTile != null)
		{
			this.previousCityTile.SetPlayerPresentOnGroundmap(false);
		}
		if (this.currentCityTile != null)
		{
			this.currentCityTile.SetPlayerPresentOnGroundmap(true);
		}
		this.requiredVicinity.Clear();
		this.requiredVicinity.Add(this.currentCityTile);
		if (this.currentCityTile != null)
		{
			for (int i = 0; i < CityData.Instance.offsetArrayX8.Length; i++)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(this.currentCityTile.cityCoord.x + CityData.Instance.offsetArrayX8[i].x, this.currentCityTile.cityCoord.y + CityData.Instance.offsetArrayX8[i].y);
				CityTile cityTile = null;
				if (HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles.TryGetValue(vector2Int, ref cityTile))
				{
					this.requiredVicinity.Add(cityTile);
				}
			}
		}
		for (int j = 0; j < this.cityTilesInVicinity.Count; j++)
		{
			CityTile cityTile2 = this.cityTilesInVicinity[j];
			if (this.requiredVicinity.Contains(cityTile2))
			{
				this.requiredVicinity.Remove(cityTile2);
			}
			else
			{
				cityTile2.SetPlayerInVicinity(false);
				this.cityTilesInVicinity.RemoveAt(j);
				j--;
			}
		}
		foreach (CityTile cityTile3 in this.requiredVicinity)
		{
			if (!(cityTile3 == null))
			{
				cityTile3.SetPlayerInVicinity(true);
				this.cityTilesInVicinity.Add(cityTile3);
			}
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x000102AC File Offset: 0x0000E4AC
	public override void OnGameLocationChange(bool enableSocialSightings = true, bool forceDisableLocationMemory = false)
	{
		base.OnGameLocationChange(true, false);
		if (this.currentGameLocation != null && InterfaceController.Instance.locationText != null)
		{
			if (this.currentGameLocation.isCrimeScene)
			{
				MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.arriveAtCrimeScene);
			}
			if (this.currentGameLocation.evidenceEntry != null)
			{
				this.currentGameLocation.evidenceEntry.OnPlayerArrival();
			}
			this.currentGameLocation.ResetLoiteringTimer();
			if (this.currentGameLocation.thisAsAddress == null || (this.currentGameLocation.thisAsAddress.addressPreset != null && !this.currentGameLocation.thisAsAddress.addressPreset.disableLocationInformationDisplay))
			{
				InterfaceController.Instance.locationText.text = this.currentGameLocation.name;
				InterfaceController.Instance.DisplayLocationText(2.2f, false);
				if (MapController.Instance.playerRoute != null && !MapController.Instance.playerRoute.nodeSpecific && MapController.Instance.playerRoute.end.gameLocation == this.currentGameLocation)
				{
					if (InterfaceController.Instance.gameHeaderQueue.Count <= 0)
					{
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, SessionData.Instance.CurrentTimeString(false, true) + ", " + MapController.Instance.playerRoute.GetDestinationText(), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					}
					MapController.Instance.playerRoute.Remove();
				}
			}
		}
		if (SessionData.Instance.isDecorEdit)
		{
			InfoWindow infoWindow = InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.preset.name == "ApartmentDecor");
			if (infoWindow != null)
			{
				infoWindow.CloseWindow(false);
			}
		}
		if (this.currentGameLocation == this.home || Enumerable.Contains<NewGameLocation>(this.apartmentsOwned, this.currentGameLocation))
		{
			float upgradeEffect = UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.apartmentStatusReset);
			if (upgradeEffect >= 1f)
			{
				this.ResetNegativeStatuses(upgradeEffect);
			}
		}
		BioScreenController.Instance.UpdateDecorEditButton();
		if (MurderController.Instance.triggerCoverUpCall)
		{
			MurderController.Instance.TriggerCoverUpTelephoneCall();
		}
		else if (MurderController.Instance.triggerCoverUpSuccess)
		{
			MurderController.Instance.TriggerCoverUpSuccessCall();
		}
		if (this.OnNewGameLocation != null)
		{
			this.OnNewGameLocation();
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00010520 File Offset: 0x0000E720
	private void ResetNegativeStatuses(float resetLevel)
	{
		if (this.hygiene < 1f)
		{
			this.hygiene = 1f;
		}
		if (this.heat < 1f)
		{
			this.heat = 1f;
		}
		if (this.nourishment < 0.5f)
		{
			this.nourishment = 0.5f;
		}
		if (this.hydration < 0.5f)
		{
			this.hydration = 0.5f;
		}
		if (this.energy < 0.5f)
		{
			this.energy = 0.5f;
		}
		if (resetLevel >= 2f)
		{
			if (this.drunk > 0f)
			{
				this.drunk = 0f;
			}
			if (this.sick > 0f)
			{
				this.sick = 0f;
			}
			if (this.starchAddiction > 0f)
			{
				this.starchAddiction = 0f;
			}
			if (this.headache > 0f)
			{
				this.headache = 0f;
			}
			if (this.wet > 0f)
			{
				this.wet = 0f;
			}
			if (this.brokenLeg > 0f)
			{
				this.brokenLeg = 0f;
			}
			if (this.bruised > 0f)
			{
				this.bruised = 0f;
			}
			if (this.blackEye > 0f)
			{
				this.blackEye = 0f;
			}
			if (this.bleeding > 0f)
			{
				this.bleeding = 0f;
			}
			if (this.poisoned > 0f)
			{
				this.poisoned = 0f;
			}
		}
		StatusController.Instance.ForceStatusCheck();
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x000106AC File Offset: 0x0000E8AC
	public override void OnBuildingChange()
	{
		base.OnBuildingChange();
		if (this.previousBuilding != null)
		{
			this.previousBuilding.SetDisplayBuildingModel(true, true, null);
			if (AchievementsController.Instance != null && AchievementsController.Instance.freeHealthCareFlag && this.previousBuilding.preset.presetName == "CityHall")
			{
				AchievementsController.Instance.UnlockAchievement("Free Healthcare", "escape_from_hospital");
			}
		}
		this.UpdateCurrentBuildingModelVisibility();
		if (this.seenProgressLag <= 0f)
		{
			StatusController.Instance.FineEscapeCheck();
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x00010744 File Offset: 0x0000E944
	[Button(null, 0)]
	public void UpdateCurrentBuildingModelVisibility()
	{
		if (this.currentRoom != null && this.currentRoom.building != null)
		{
			if (this.inAirVent)
			{
				this.currentRoom.building.SetDisplayBuildingModel(false, false, null);
				return;
			}
			BuildingPreset.InteriorFloorSetting floorSetting = this.currentRoom.building.preset.GetFloorSetting(this.currentRoom.floor.floor, this.currentRoom.floor.layoutIndex);
			bool flag = this.currentRoom.IsOutside();
			if (floorSetting != null && floorSetting.forceShowModel)
			{
				this.currentRoom.building.SetDisplayBuildingModel(true, flag, floorSetting.forceHideModels);
			}
			else if (this.currentRoom.preset.drawBuildingModel || flag)
			{
				if (flag && floorSetting.forceHideModelsOutside.Count > 0)
				{
					this.currentRoom.building.SetDisplayBuildingModel(true, true, floorSetting.forceHideModelsOutside);
				}
				else
				{
					this.currentRoom.building.SetDisplayBuildingModel(true, true, null);
				}
			}
			else if (this.currentRoom.windows.Count > 0 && floorSetting != null)
			{
				this.currentRoom.building.SetDisplayBuildingModel(true, false, floorSetting.forceHideModels);
			}
			else
			{
				this.currentRoom.building.SetDisplayBuildingModel(false, false, null);
			}
			if (floorSetting == null || floorSetting.forceHideModelsInRooms.Count <= 0)
			{
				return;
			}
			using (List<BuildingPreset.ForceHideModelsForRoom>.Enumerator enumerator = floorSetting.forceHideModelsInRooms.FindAll((BuildingPreset.ForceHideModelsForRoom item) => item.roomConfig == this.currentRoom.preset).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BuildingPreset.ForceHideModelsForRoom forceHideModelsForRoom = enumerator.Current;
					this.currentRoom.building.SelectivelyHideModels(forceHideModelsForRoom.forceHideModels);
				}
				return;
			}
		}
		if (this.currentBuilding != null)
		{
			if (this.inAirVent)
			{
				this.currentBuilding.SetDisplayBuildingModel(false, false, null);
				return;
			}
			if (this.currentRoom.preset.drawBuildingModel || this.currentRoom.IsOutside())
			{
				this.currentBuilding.SetDisplayBuildingModel(true, true, null);
				return;
			}
			if (this.currentRoom.windows.Count > 0)
			{
				this.currentBuilding.SetDisplayBuildingModel(true, false, this.currentBuilding.preset.GetFloorSetting(this.currentRoom.floor.floor, this.currentRoom.floor.layoutIndex).forceHideModels);
				return;
			}
			this.currentBuilding.SetDisplayBuildingModel(false, false, null);
		}
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x000109D0 File Offset: 0x0000EBD0
	public override void OnNodeChange()
	{
		base.OnNodeChange();
		this.DoFallThroughFloorCheck();
		AudioController.Instance.UpdateAmbientZonesOnEndOfFrame();
		if (!this.isRunning && !this.isCrouched && !this.stealthMode && this.brokenLeg <= 0f && this.drunk <= 0f && this.currentGameLocation != this.home && !CutSceneController.Instance.cutSceneActive && base.transform.position.y > CityControls.Instance.basementWaterLevel)
		{
			this.nodesTraversedWhileWalking++;
			if (this.nodesTraversedWhileWalking > 30)
			{
				if (Game.Instance.displayExtraControlHints)
				{
					ControlsDisplayController.Instance.DisplayControlIcon(InteractablePreset.InteractionKey.sprint, "Sprint", InterfaceControls.Instance.controlIconDisplayTime, false);
				}
				this.nodesTraversedWhileWalking = 0;
			}
		}
		if (MapController.Instance.playerRoute != null && MapController.Instance.playerRoute.nodeSpecific && MapController.Instance.playerRoute.end == this.currentNode)
		{
			Game.Log("Interface: Reached route destination...", 2);
			if (InterfaceController.Instance.gameHeaderQueue.Count <= 0)
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, SessionData.Instance.CurrentTimeString(false, true) + ", " + MapController.Instance.playerRoute.GetDestinationText(), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
			MapController.Instance.playerRoute.Remove();
		}
		if (!SessionData.Instance.isFloorEdit && MapController.Instance.playerRoute != null)
		{
			MapController.Instance.playerRoute.UpdateRouteBasedOnPlayerPosition();
		}
		for (int i = 0; i < SessionData.Instance.particleSystems.Count; i++)
		{
			InteractableController interactableController = SessionData.Instance.particleSystems[i];
			if (interactableController == null)
			{
				SessionData.Instance.particleSystems.RemoveAt(i);
				i--;
			}
			else
			{
				interactableController.UpdateParticleSystemDistance();
			}
		}
		foreach (KeyValuePair<InteractablePreset.InteractionKey, InteractionController.InteractionSetting> keyValuePair in InteractionController.Instance.currentInteractions)
		{
			if (keyValuePair.Value.audioEvent != null && keyValuePair.Value.newUIRef != null && keyValuePair.Value.newUIRef.soundIndicator != null)
			{
				keyValuePair.Value.newUIRef.soundIndicator.UpdateCurrentEvent();
			}
		}
		ObjectPoolingController.Instance.UpdateObjectRanges();
		if (this.isMoving && this.currentNode != null && this.currentRoom != null)
		{
			try
			{
				foreach (Actor actor in this.currentRoom.currentOccupants)
				{
					if (actor.ai != null)
					{
						try
						{
							actor.ai.InstantPersuitCheck(Player.Instance);
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x00010D24 File Offset: 0x0000EF24
	public void DoFallThroughFloorCheck()
	{
		if (!Game.Instance.enableColesFallingThroughFloorCheck)
		{
			return;
		}
		if (this.currentNode != null && this.previousNode != null && !this.fps.ghostMovement && this.fps.clipping && !this.inAirVent && this.previousNode.room != null && !this.previousNode.room.isNullRoom && this.currentNode.nodeCoord == new Vector3Int(this.previousNode.nodeCoord.x, this.previousNode.nodeCoord.y, this.previousNode.nodeCoord.z - 1) && (this.previousNode.floorType == NewNode.FloorTileType.floorAndCeiling || this.previousNode.floorType == NewNode.FloorTileType.floorOnly))
		{
			Game.Log("Player: Illegal falling through floor detected! Teleporting back to previous position!", 2);
			this.Teleport(this.previousNode, null, true, true);
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x00010E26 File Offset: 0x0000F026
	private void OnDisable()
	{
		this.previousNode = null;
	}

	// Token: 0x060001FA RID: 506 RVA: 0x00010E30 File Offset: 0x0000F030
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			if (this.currentRoom != null)
			{
				if (this.gasLevel < this.currentRoom.gasLevel)
				{
					this.gasLevel += Time.deltaTime / 2f;
					this.gasLevel = Mathf.Clamp01(this.gasLevel);
					StatusController.Instance.ForceStatusCheck();
				}
				else if (this.gasLevel > this.currentRoom.gasLevel)
				{
					this.gasLevel -= Time.deltaTime / 2f;
					this.gasLevel = Mathf.Clamp01(this.gasLevel);
					StatusController.Instance.ForceStatusCheck();
				}
			}
			if (!CutSceneController.Instance.cutSceneActive)
			{
				if (this.spawnProtection > 0f)
				{
					this.spawnProtection -= Time.deltaTime;
				}
				if (Player.Instance.syncDiskInstall > 0f)
				{
					Player.Instance.AddSyncDiskInstall(-Time.deltaTime / 7.6f);
				}
				if (Player.Instance.blinded > 0f)
				{
					Player.Instance.AddBlinded(-Time.deltaTime / 7f);
				}
				if (this.takeDamageIndicatorTimer > 0f)
				{
					if (this.takeDamageIndicatorTimer > 0.9f)
					{
						InterfaceController.Instance.takeDamageIndicatorImg.canvasRenderer.SetAlpha(Mathf.Clamp01(1f - this.takeDamageIndicatorTimer * 10f));
					}
					else
					{
						InterfaceController.Instance.takeDamageIndicatorImg.canvasRenderer.SetAlpha(Mathf.Clamp01(this.takeDamageIndicatorTimer / 0.9f));
					}
					this.takeDamageIndicatorTimer -= Time.deltaTime * this.takeDamageDisplaySpeed;
					if (this.takeDamageIndicatorTimer <= 0f)
					{
						this.takeDamageIndicatorTimer = 0f;
						InterfaceController.Instance.takeDamageIndicatorImg.gameObject.SetActive(false);
					}
				}
				if (this.currentHealth < base.GetCurrentMaxHealth() && !StatusController.Instance.disabledRecovery)
				{
					float amount = this.recoveryRate * StatusController.Instance.recoveryRateMultiplier * Time.deltaTime * SessionData.Instance.currentTimeMultiplier * 0.01f;
					this.AddHealth(amount, true, false);
				}
				if (this.fps.m_WalkSpeed < this.desiredWalkSpeed)
				{
					this.fps.m_WalkSpeed += Time.deltaTime * 2f;
					this.fps.m_WalkSpeed = Mathf.Min(this.desiredWalkSpeed, this.fps.m_WalkSpeed);
					this.movementWalkSpeed = this.fps.m_WalkSpeed;
				}
				else if (this.fps.m_WalkSpeed > this.desiredWalkSpeed)
				{
					this.fps.m_WalkSpeed -= Time.deltaTime * 2f;
					this.fps.m_WalkSpeed = Mathf.Max(this.desiredWalkSpeed, this.fps.m_WalkSpeed);
					this.movementWalkSpeed = this.fps.m_WalkSpeed;
				}
				if (this.fps.m_RunSpeed < this.desiredRunSpeed)
				{
					this.fps.m_RunSpeed += Time.deltaTime * 2f;
					this.fps.m_RunSpeed = Mathf.Min(this.desiredRunSpeed, this.fps.m_RunSpeed);
					this.movementRunSpeed = this.fps.m_RunSpeed;
				}
				else if (this.fps.m_RunSpeed > this.desiredRunSpeed)
				{
					this.fps.m_RunSpeed -= Time.deltaTime * 2f;
					this.fps.m_RunSpeed = Mathf.Max(this.desiredRunSpeed, this.fps.m_RunSpeed);
					this.movementRunSpeed = this.fps.m_RunSpeed;
				}
				AudioController.Instance.updateClosestWindowTicker++;
				if (AudioController.Instance.updateClosestWindowTicker > AudioController.Instance.updateClosestWindow)
				{
					AudioController.Instance.UpdateClosestWindowAndDoor(false);
					AudioController.Instance.UpdateClosestExteriorWall();
					AudioController.Instance.updateClosestWindowTicker = 0;
				}
				AudioController.Instance.updateMixingTicker++;
				if (AudioController.Instance.updateMixingTicker > AudioController.Instance.updateMixing)
				{
					AudioController.Instance.UpdateMixing();
					AudioController.Instance.updateMixingTicker = 0;
				}
				AudioController.Instance.updateAmbientZonesTimer += Time.deltaTime;
				if (AudioController.Instance.updateAmbientZonesTimer > 1f)
				{
					AudioController.Instance.UpdateAmbientZonesOnEndOfFrame();
					AudioController.Instance.updateAmbientZonesTimer = 0f;
				}
				ObjectPoolingController.Instance.updateObjectRangesTimer += Time.deltaTime;
				if (ObjectPoolingController.Instance.updateObjectRangesTimer > 5f)
				{
					ObjectPoolingController.Instance.UpdateObjectRanges();
					ObjectPoolingController.Instance.updateObjectRangesTimer = 0f;
				}
				this.updateNodeSpace--;
				if (this.updateNodeSpace <= 0)
				{
					this.updateNodeSpace = 10;
					this.UpdateCurrentNodeSpace();
				}
				if (this.transitionActive)
				{
					this.ExecuteTransition();
				}
				else if (this.forceLookAtActive)
				{
					this.ExecuteForceLookAt();
				}
				else if (InteractionController.Instance.currentInteractable != null)
				{
					if (InteractionController.Instance.interactionLookProgress < 1f)
					{
						InteractionController.Instance.interactionLookProgress += Time.deltaTime;
					}
					Vector3 vector = Vector3.zero;
					if (InteractionController.Instance.currentInteractable.lookAtTarget != null)
					{
						vector = InteractionController.Instance.currentInteractable.lookAtTarget.position - CameraController.Instance.cam.transform.position;
					}
					else
					{
						vector = InteractionController.Instance.currentInteractable.transform.position - CameraController.Instance.cam.transform.position;
					}
					Quaternion quaternion = Quaternion.LookRotation(vector, Vector3.up);
					if (quaternion != CameraController.Instance.cam.transform.rotation)
					{
						CameraController.Instance.cam.transform.rotation = Quaternion.Lerp(CameraController.Instance.cam.transform.rotation, quaternion, InteractionController.Instance.interactionLookProgress);
					}
				}
				if ((InteractionController.Instance.lockedInInteraction == null || InteractionController.Instance.carryingObject != null) && !this.inAirVent)
				{
					if (this.isCrouched && this.crouchedTransition < 1f)
					{
						this.crouchedTransition += Time.deltaTime * 2.6f;
						this.crouchedTransition = Mathf.Clamp01(this.crouchedTransition);
						this.crouchTransitionActive = true;
						this.SetPlayerHeight(Mathf.Lerp(Player.Instance.GetPlayerHeightNormal(), Player.Instance.GetPlayerHeightCrouched(), GameplayControls.Instance.crouchHeightCurve.Evaluate(this.crouchedTransition)), true);
						float cameraHeight = Mathf.SmoothStep(GameplayControls.Instance.cameraHeightNormal, GameplayControls.Instance.cameraHeightCrouched, this.crouchedTransition);
						this.SetCameraHeight(cameraHeight);
					}
					else if (!this.isCrouched && this.crouchedTransition > 0f)
					{
						this.crouchedTransition -= Time.deltaTime * 2.2f;
						this.crouchedTransition = Mathf.Clamp01(this.crouchedTransition);
						this.crouchTransitionActive = true;
						this.SetPlayerHeight(Mathf.Lerp(Player.Instance.GetPlayerHeightNormal(), Player.Instance.GetPlayerHeightCrouched(), GameplayControls.Instance.crouchHeightCurve.Evaluate(this.crouchedTransition)), true);
						float cameraHeight2 = Mathf.SmoothStep(GameplayControls.Instance.cameraHeightNormal, GameplayControls.Instance.cameraHeightCrouched, this.crouchedTransition);
						this.SetCameraHeight(cameraHeight2);
					}
					else if (this.crouchTransitionActive)
					{
						this.crouchTransitionActive = false;
					}
				}
				this.spotCheckTimer++;
				if (this.spotCheckTimer >= GameplayControls.Instance.playerSpotUpdateEveryXFrame)
				{
					this.spotCheckTimer = 0;
					this.SightingCheck(0f, false);
					foreach (KeyValuePair<InteractablePreset.InteractionKey, InteractionController.InteractionSetting> keyValuePair in InteractionController.Instance.currentInteractions)
					{
						if (keyValuePair.Value.audioEvent != null && keyValuePair.Value.newUIRef != null && keyValuePair.Value.newUIRef.soundIndicator != null)
						{
							keyValuePair.Value.newUIRef.soundIndicator.UpdateCurrentEvent();
						}
					}
				}
				if (this.spottedByPlayer.Count > 0)
				{
					new List<Actor>();
					new List<Actor>();
					for (int i = 0; i < this.spottedByPlayer.Count; i++)
					{
						Actor actor = this.spottedByPlayer[i];
						if (actor.spottedGraceTime > 0f)
						{
							actor.spottedGraceTime -= Time.deltaTime;
							if (actor.currentNodeCoord.z != Player.Instance.currentNodeCoord.z)
							{
								actor.spottedGraceTime -= Time.deltaTime;
							}
						}
						else
						{
							actor.spottedState -= Time.deltaTime / GameplayControls.Instance.spottedFadeSpeed;
							if (actor.spottedState <= 0f)
							{
								actor.spottedGraceTime = 0f;
								actor.spottedState = 0f;
								if (actor.ai != null)
								{
									actor.ai.TriggerReactionIndicator();
								}
								this.spottedByPlayer.RemoveAt(i);
								i--;
								InterfaceController.Instance.footstepAudioIndicator.UpdateCurrentEvent();
								foreach (KeyValuePair<InteractablePreset.InteractionKey, InteractionController.InteractionSetting> keyValuePair2 in InteractionController.Instance.currentInteractions)
								{
									if (keyValuePair2.Value.audioEvent != null && keyValuePair2.Value.newUIRef != null && keyValuePair2.Value.newUIRef.soundIndicator != null)
									{
										keyValuePair2.Value.newUIRef.soundIndicator.UpdateCurrentEvent();
									}
								}
							}
						}
					}
				}
				this.seenProgress = 0f;
				if (this.witnessesToIllegalActivity.Count > 0)
				{
					if (this.seenIconLag < 1f)
					{
						this.seenIconLag += Time.deltaTime * 4f;
						this.seenIconLag = Mathf.Clamp01(this.seenIconLag);
						InterfaceControls.Instance.seenRenderer.SetAlpha(this.seenIconLag);
					}
					using (HashSet<Actor>.Enumerator enumerator2 = this.witnessesToIllegalActivity.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Actor actor2 = enumerator2.Current;
							if (actor2.seesIllegal.ContainsKey(this))
							{
								this.seenProgress = Mathf.Max(this.seenProgress, actor2.seesIllegal[this]);
							}
						}
						goto IL_B6C;
					}
				}
				if (this.seenProgress > 0f)
				{
					this.seenProgress = 0f;
				}
				if (this.seenIconLag > 0f || this.firstFrame)
				{
					this.seenIconLag -= Time.deltaTime * 3f;
					this.seenIconLag = Mathf.Clamp01(this.seenIconLag);
					if (InterfaceControls.Instance.seenRenderer != null)
					{
						InterfaceControls.Instance.seenRenderer.SetAlpha(this.seenIconLag);
					}
				}
				IL_B6C:
				if (this.seenProgressLag != this.seenProgress || this.firstFrame)
				{
					if (this.seenProgressLag < this.seenProgress)
					{
						this.seenProgressLag += Time.deltaTime * 0.5f;
						this.seenProgressLag = Mathf.Min(this.seenProgressLag, this.seenProgress);
					}
					else if (this.seenProgressLag > this.seenProgress)
					{
						this.seenProgressLag -= Time.deltaTime * 0.25f;
						this.seenProgressLag = Mathf.Max(this.seenProgressLag, this.seenProgress);
					}
					this.seenProgressLag = Mathf.Clamp(this.seenProgressLag, 0f, 1f);
					if (!SessionData.Instance.isFloorEdit)
					{
						if (this.seenProgressLag <= 0f)
						{
							StatusController.Instance.FineEscapeCheck();
						}
						InterfaceController.Instance.movieBarJuice.pulsateSpeed = Mathf.Lerp(0.5f, 2.5f, this.seenProgressLag);
						InterfaceControls.Instance.lightOrbFillImg.color = Color.Lerp(Color.white, InterfaceControls.Instance.interactionControlTextColourIllegal, this.seenProgressLag * 1.25f);
						InterfaceControls.Instance.lightOrbOutline.color = InterfaceControls.Instance.lightOrbFillImg.color;
						InterfaceControls.Instance.seenImg.color = InterfaceControls.Instance.lightOrbFillImg.color;
						InterfaceControls.Instance.seenJuice.elements[0].originalColour = InterfaceControls.Instance.seenImg.color;
					}
				}
				if (this.persuedBy.Count > 0)
				{
					this.persuedProgress = 1f;
					if (AudioController.Instance.threatLoop != null)
					{
						Game.Log("Debug: Stopping threat loop", 2);
						AudioController.Instance.StopSound(AudioController.Instance.threatLoop, AudioController.StopType.fade, "Persued");
						AudioController.Instance.threatLoop = null;
					}
				}
				else
				{
					this.persuedProgress = this.seenProgress;
				}
				if (this.persuedProgressLag != this.persuedProgress || this.firstFrame)
				{
					if (this.persuedProgressLag < this.persuedProgress && this.persuedBy.Count <= 0)
					{
						this.persuedProgressLag += Time.deltaTime * 1f;
						this.persuedProgressLag = Mathf.Clamp01(this.persuedProgressLag);
					}
					else if (this.persuedProgressLag > this.persuedProgress)
					{
						this.persuedProgressLag -= Time.deltaTime * 0.5f;
						this.persuedProgressLag = Mathf.Clamp01(this.persuedProgressLag);
					}
					if (this.persuedProgressLag > 0f)
					{
						if (AudioController.Instance.threatLoop != null)
						{
							AudioController.Instance.threatLoop.audioEvent.setParameterByName("Threat", this.persuedProgressLag, false);
						}
					}
					else if (AudioController.Instance.threatLoop != null)
					{
						Game.Log("Debug: Stopping threat loop", 2);
						AudioController.Instance.StopSound(AudioController.Instance.threatLoop, AudioController.StopType.fade, "Persued");
						AudioController.Instance.threatLoop = null;
					}
					if (InterfaceController.Instance.movieBarTop != null)
					{
						InterfaceController.Instance.movieBarTop.sizeDelta = new Vector2(InterfaceController.Instance.movieBarTop.sizeDelta.x, this.persuedProgressLag * InterfaceControls.Instance.movieBarHeight);
					}
					if (InterfaceController.Instance.movieBarBottom != null)
					{
						InterfaceController.Instance.movieBarBottom.sizeDelta = new Vector2(InterfaceController.Instance.movieBarTop.sizeDelta.x, this.persuedProgressLag * InterfaceControls.Instance.movieBarHeight);
					}
				}
				if (this.visibilityLag != this.overallVisibility || this.firstFrame)
				{
					if (this.visibilityLag < this.overallVisibility)
					{
						this.visibilityLag += Time.deltaTime;
						this.visibilityLag = Mathf.Min(this.visibilityLag, this.overallVisibility);
					}
					else if (this.visibilityLag > this.overallVisibility)
					{
						this.visibilityLag -= Time.deltaTime;
						this.visibilityLag = Mathf.Max(this.visibilityLag, this.overallVisibility);
					}
					this.visibilityLag = Mathf.Clamp(this.visibilityLag, 0f, 1f);
					if (InterfaceControls.Instance.lightOrbFillImg != null)
					{
						InterfaceControls.Instance.lightOrbFillImg.canvasRenderer.SetAlpha(this.visibilityLag * 0.9f);
					}
				}
				if ((this.stealthLag < 1f && (this.stealthMode || this.witnessesToIllegalActivity.Count > 0)) || this.firstFrame)
				{
					this.stealthLag += Time.deltaTime * 2f;
					this.stealthLag = Mathf.Clamp01(this.stealthLag);
					float num = Mathf.LerpUnclamped(InterfaceControls.Instance.lightOrbSize.x, InterfaceControls.Instance.lightOrbSize.y, InterfaceControls.Instance.stealthModeOrbSizeTransitionIn.Evaluate(this.stealthLag));
					if (InterfaceControls.Instance.lightOrbRect != null)
					{
						InterfaceControls.Instance.lightOrbRect.sizeDelta = new Vector2(num, num);
						InterfaceControls.Instance.lightOrbOutline.rectTransform.sizeDelta = InterfaceControls.Instance.lightOrbRect.sizeDelta;
					}
				}
				else if ((this.stealthLag > 0f && !this.stealthMode && this.witnessesToIllegalActivity.Count <= 0) || this.firstFrame)
				{
					this.stealthLag -= Time.deltaTime * 2f;
					this.stealthLag = Mathf.Clamp01(this.stealthLag);
					float num2 = Mathf.LerpUnclamped(InterfaceControls.Instance.lightOrbSize.x, InterfaceControls.Instance.lightOrbSize.y, InterfaceControls.Instance.stealthModeOrbSizeTransitionOut.Evaluate(this.stealthLag));
					if (InterfaceControls.Instance.lightOrbRect != null)
					{
						InterfaceControls.Instance.lightOrbRect.sizeDelta = new Vector2(num2, num2);
						InterfaceControls.Instance.lightOrbOutline.rectTransform.sizeDelta = InterfaceControls.Instance.lightOrbRect.sizeDelta;
					}
				}
				if (this.setAlarmModeAfterDelay > 0f)
				{
					this.setAlarmModeAfterDelay -= Time.deltaTime;
					if (this.setAlarmModeAfterDelay <= 0f)
					{
						this.SetSettingAlarmMode(true);
					}
				}
				if (this.spendingTimeDelay > 0f)
				{
					this.spendingTimeDelay -= Time.deltaTime;
					if (this.spendingTimeDelay <= 0f)
					{
						this.SetSpendingTimeMode(true);
					}
				}
				if (this.playerKOInProgress)
				{
					if (InterfaceController.Instance.fade >= 1f && !this.playerKOFadeOut)
					{
						foreach (Actor actor3 in new List<Actor>(this.witnessesToIllegalActivity))
						{
							if (actor3.ai != null)
							{
								actor3.ai.CancelCombat();
							}
						}
						this.citizensArrestActive = false;
						if (!this.KORecovery)
						{
							this.ReturnFromTransform(true, true);
							if (!this.paidFines)
							{
								if (!this.dirtyDeath)
								{
									StatusController.Instance.PayActiveFines();
								}
								else if (this.debtPayment != null)
								{
									if (GameplayController.Instance.money >= this.debtPayment.GetRepaymentAmount())
									{
										GameplayController.Instance.DebtPayment(CityData.Instance.companyDirectory.Find((Company item) => item.companyID == this.debtPayment.companyID));
									}
									else
									{
										GameplayController.Instance.ShortDebtPayment(CityData.Instance.companyDirectory.Find((Company item) => item.companyID == this.debtPayment.companyID), GameplayController.Instance.money);
									}
								}
								else
								{
									GameplayController.Instance.AddMoney(-Mathf.Max(Mathf.RoundToInt((float)GameplayController.Instance.money * 0.5f), 200), true, "Mugged");
								}
								foreach (Actor actor4 in new List<Actor>(this.persuedBy))
								{
									actor4.RemoveFromSeesIllegal(this, 0f);
								}
								this.paidFines = true;
							}
							if (this.dirtyDeath)
							{
								List<NewRoom> list = CityData.Instance.roomDirectory.FindAll((NewRoom item) => item.preset.muggingAwakenRoom);
								if (list.Count > 0)
								{
									this.bed = null;
									this.Teleport(base.FindSafeTeleport(list[Toolbox.Instance.Rand(0, list.Count, false)], false), null, true, false);
								}
								else
								{
									this.bed = GameplayController.Instance.hospitalBeds[Toolbox.Instance.Rand(0, GameplayController.Instance.hospitalBeds.Count, false)];
									this.Teleport(this.bed.node, null, true, false);
								}
							}
							else if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.awakenAtHome) > 0f && this.home != null)
							{
								NewRoom newRoom;
								FurnitureLocation furnitureLocation = Toolbox.Instance.FindFurnitureWithinGameLocation(this.home, InteriorControls.Instance.bed, out newRoom);
								if (furnitureLocation != null)
								{
									this.bed = furnitureLocation.integratedInteractables[0];
									this.Teleport(this.bed.node, null, true, false);
								}
								else
								{
									this.bed = GameplayController.Instance.hospitalBeds[Toolbox.Instance.Rand(0, GameplayController.Instance.hospitalBeds.Count, false)];
									this.Teleport(this.bed.node, null, true, false);
								}
							}
							else
							{
								this.bed = GameplayController.Instance.hospitalBeds[Toolbox.Instance.Rand(0, GameplayController.Instance.hospitalBeds.Count, false)];
								this.Teleport(this.bed.node, null, true, false);
							}
							if (this.bed != null)
							{
								InteractablePreset.InteractionAction action = this.bed.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.action == RoutineControls.Instance.sleep);
								InteractionController.Instance.currentLookingAtInteractable = this.bed.controller;
								this.bed.OnInteraction(action, Player.Instance, true, 0f);
							}
							if (this.dirtyDeath)
							{
								this.SetHealth(base.GetCurrentMaxHealth() * 0.25f);
							}
							else
							{
								this.SetHealth(base.GetCurrentMaxHealth() * 0.5f);
								this.AddBlackedOut(-1f);
								this.AddBrokenLeg(-1f);
							}
							this.AddBleeding(-1f);
							this.AddPoisoned(-1f, null);
							this.AddBlinded(-1f);
						}
						else
						{
							this.ResetHealthToMaximum();
							this.AddBlackedOut(-1f);
							this.AddBrokenLeg(-1f);
							this.AddBleeding(-1f);
							this.AddBruised(-1f);
							this.AddHeadache(-1f);
							this.AddBlackEye(-1f);
							this.AddSick(-1f);
							this.AddPoisoned(-1f, null);
							this.AddBlinded(-1f);
							this.isCrunchingDatabase = false;
						}
						this.ClearSeesIllegal();
						InteractionController.Instance.UpdateInteractionText();
						this.playerKOFadeOut = true;
						Game.Log("Player: Teleport to bed after KO...", 1);
					}
					else if (this.playerKOFadeOut && this.KOTimePassed < this.KOTime)
					{
						if (SessionData.Instance.currentTimeSpeed != SessionData.TimeSpeed.veryFast)
						{
							SessionData.Instance.SetTimeSpeed(SessionData.TimeSpeed.veryFast);
							InterfaceControls.Instance.fastForwardArrow.gameObject.SetActive(true);
							if (!Game.Instance.permaDeath)
							{
								Game.Log("Player: Display KO progress bar...", 2);
								InteractionController.Instance.SetInteractionAction(0f, this.KOTime, 1f, "ko", false, false, null, false);
							}
							if (!this.KORecovery)
							{
								if (Game.Instance.permaDeath)
								{
									InterfaceController.Instance.ExecuteGameOverDisplay();
								}
								else
								{
									InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "ko", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.passedOut, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
									SessionData.Instance.TutorialTrigger("ko", false);
								}
							}
						}
						if (!SessionData.Instance.play)
						{
							SessionData.Instance.ResumeGame();
						}
						this.KOTimePassed += (float)SessionData.Instance.gameTimePassedThisFrame;
						for (int j = 0; j < InteractionController.Instance.spawnedProgressControllers.Count; j++)
						{
							InteractionController.Instance.spawnedProgressControllers[j].SetAmount(this.KOTimePassed);
						}
						if (Game.Instance.permaDeath && this.KOTimePassed >= this.KOTime * 0.5f)
						{
							RestartSafeController.Instance.loadFromDirty = false;
							AudioController.Instance.StopAllSounds();
							InputController.Instance.SetCursorLock(false);
							InputController.Instance.SetCursorVisible(true);
							SceneManager.LoadScene("Main");
							return;
						}
						if (this.KOTimePassed >= this.KOTime)
						{
							SessionData.Instance.SetTimeSpeed(SessionData.TimeSpeed.normal);
							InterfaceControls.Instance.fastForwardArrow.gameObject.SetActive(false);
							InterfaceController.Instance.Fade(0f, 6f, false);
							while (InteractionController.Instance.spawnedProgressControllers.Count > 0)
							{
								Object.Destroy(InteractionController.Instance.spawnedProgressControllers[0].gameObject);
								InteractionController.Instance.spawnedProgressControllers.RemoveAt(0);
							}
							InterfaceControls.Instance.actionInteractionDisplay.gameObject.SetActive(false);
							InteractionController.Instance.CompleteInteractionAction();
							if (this.KORecovery || UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.reduceMedicalCosts) >= 1f || (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.accidentCover) > 0f && !Player.Instance.claimedAccidentCover))
							{
								this.ResetHealthToMaximum();
								this.AddNourishment(1f);
								this.AddHydration(1f);
								this.AddHygiene(1f);
								this.AddEnergy(1f);
								this.AddBlackedOut(-1f);
								this.AddBrokenLeg(-1f);
								this.AddBleeding(-1f);
								this.AddBruised(-1f);
								this.AddHeadache(-1f);
								this.AddBlackEye(-1f);
								this.AddSick(-1f);
								this.AddStarchAddiction(-1f);
								this.AddDrunk(-1f);
								this.AddPoisoned(-1f, null);
								this.AddBlinded(-1f);
							}
							else
							{
								if (!this.dirtyDeath)
								{
									StatusController.Instance.SetDetainedInBuilding(Player.Instance.currentBuilding, true);
								}
								this.AddNumb(0.75f);
								if (Toolbox.Instance.Rand(0f, 1f, false) <= 0.25f)
								{
									this.AddHeadache(0.5f);
								}
								if (Toolbox.Instance.Rand(0f, 1f, false) <= 0.25f)
								{
									this.AddBruised(0.5f);
								}
							}
						}
					}
					else if (this.playerKOFadeOut && InterfaceController.Instance.fade <= 0f)
					{
						Game.Log("Player: Wake up from KO", 1);
						this.playerKOInProgress = false;
						InteractionController.Instance.UpdateNearbyInteractables();
						SessionData.Instance.SetEnablePause(true);
						Chapter chapter;
						int num3;
						if (Toolbox.Instance.IsStoryMissionActive(out chapter, out num3) && ChapterController.Instance.currentPart < 31)
						{
							this.KORecovery = true;
						}
						else if (this.dirtyDeath)
						{
							this.KORecovery = false;
						}
						else if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.accidentCover) > 0f && !Player.Instance.claimedAccidentCover)
						{
							this.KORecovery = true;
							Player.Instance.claimedAccidentCover = true;
							GameplayController.Instance.AddMoney(Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.accidentCover)), true, "Accident cover");
						}
						else if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.reduceMedicalCosts) >= 1f)
						{
							this.KORecovery = true;
						}
						if (this.KORecovery)
						{
							this.ClearAllDisabledActions();
							this.KORecovery = false;
						}
						else if (this.bed != null)
						{
							this.bed.SetCustomState2(true, Player.Instance, false, false, false);
							InteractionController.Instance.SetDialog(true, this.bed, false, null, InteractionController.ConversationType.normal);
						}
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, string.Concat(new string[]
						{
							SessionData.Instance.CurrentTimeString(false, true),
							", ",
							SessionData.Instance.LongDateString(SessionData.Instance.gameTime, true, true, true, true, true, false, false, true),
							", ",
							Player.Instance.currentGameLocation.name
						}), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
						this.SetActionDisable("Get In", false);
						this.SetActionDisable("Get Up", false);
						this.SetActionDisable("Enter", false);
						this.SetActionDisable("Sleep", false);
						this.SetActionDisable("Sleep...", false);
						InteractionController.Instance.UpdateInteractionText();
					}
				}
				if (this.inConversation)
				{
					this.UpdateConversation();
				}
				this.firstFrame = false;
			}
			if (this.hurt > 0f)
			{
				this.hurt -= Time.deltaTime * 0.35f;
				this.hurt = Mathf.Clamp01(this.hurt);
				SessionData.Instance.exposure.active = true;
				SessionData.Instance.exposure.fixedExposure.value = Mathf.Min(-this.hurt * 0.6f, StatusController.Instance.exposureAmount);
			}
			else if (SessionData.Instance.exposure.IsActive() && StatusController.Instance.exposureAmount == 0f)
			{
				SessionData.Instance.exposure.active = false;
			}
			if (this.autoTravelActive)
			{
				if (MapController.Instance.playerRoute == null)
				{
					Player.Instance.EndAutoTravel();
				}
				else if (MapController.Instance.playerRoute.pathData != null && MapController.Instance.playerRoute.pathData.accessList.Count > 0)
				{
					if (this.currentAutoTravelDest == null)
					{
						this.currentAutoTravelDest = MapController.Instance.playerRoute.pathData.accessList[MapController.Instance.playerRoute.routeCursor];
					}
					if (this.currentAutoTravelDest != null)
					{
						if (this.currentAutoTravelDest.door != this.autoTravelDoor && this.autoTravelDoor != null)
						{
							ActionController.Instance.OpenDoor(this.autoTravelDoor.doorInteractable, this.currentNode, this);
							foreach (Collider collider in this.autoTravelDoor.spawnedDoorColliders)
							{
								if (collider != null)
								{
									Physics.IgnoreCollision(collider, this.fps.m_CharacterController, false);
								}
							}
							this.autoTravelDoor = null;
						}
						if (this.currentAutoTravelDest.door != null && this.currentAutoTravelDest.door.isClosed)
						{
							ActionController.Instance.OpenDoor(this.currentAutoTravelDest.door.doorInteractable, this.currentNode, this);
							if (this.currentAutoTravelDest.door.isClosed)
							{
								this.EndAutoTravel();
							}
							else
							{
								this.autoTravelDoor = this.currentAutoTravelDest.door;
								foreach (Collider collider2 in this.autoTravelDoor.spawnedDoorColliders)
								{
									if (collider2 != null)
									{
										Physics.IgnoreCollision(collider2, this.fps.m_CharacterController, true);
									}
								}
							}
						}
						NewNode nodeAhead = MapController.Instance.playerRoute.pathData.GetNodeAhead(MapController.Instance.playerRoute.routeCursor);
						NewNode.NodeSpace nodeSpace = null;
						float num4 = -99999f;
						foreach (KeyValuePair<Vector3, NewNode.NodeSpace> keyValuePair3 in nodeAhead.walkableNodeSpace)
						{
							float num5 = 5f - Vector3.Distance(keyValuePair3.Value.position, base.transform.position);
							RaycastHit raycastHit;
							if (Physics.SphereCast(keyValuePair3.Value.position + new Vector3(0f, 0.3f, 0f), this.fps.m_CharacterController.radius * 0.75f, Vector3.up, ref raycastHit, this.fps.m_CharacterController.height, Toolbox.Instance.playerMovementLayerMask, 1))
							{
								num5 -= 10f;
							}
							if (Vector3.Distance(keyValuePair3.Value.position, nodeAhead.position) < 0.2f)
							{
								num5 += 5f;
							}
							if (this.currentNodeSpaceDest == keyValuePair3.Value && this.currentNodeSpaceDestTimer > 1.5f)
							{
								num5 -= this.currentNodeSpaceDestTimer - 1.5f;
							}
							if (num5 > num4)
							{
								nodeSpace = keyValuePair3.Value;
								num4 = num5;
							}
						}
						if (this.currentNodeSpaceDest != nodeSpace)
						{
							this.currentNodeSpaceDest = nodeSpace;
							this.currentNodeSpaceDestTimer = 0f;
						}
						else
						{
							this.currentNodeSpaceDestTimer += Time.deltaTime;
						}
						Vector3 vector2 = nodeAhead.position + new Vector3(0f, this.fps.m_CharacterController.height * 0.5f + this.fps.m_CharacterController.skinWidth, 0f);
						if (nodeSpace != null)
						{
							vector2 = nodeSpace.position + new Vector3(0f, this.fps.m_CharacterController.height * 0.5f + this.fps.m_CharacterController.skinWidth, 0f);
						}
						this.autoTravelForward = vector2 - base.transform.position;
						this.autoTravelForward.y = 0f;
						this.autoTravelForward = this.autoTravelForward.normalized;
						this.autoTravelDistanceToNext = Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(vector2.x, vector2.z));
						this.currentAutoTravelDest = null;
					}
					else
					{
						this.EndAutoTravel();
					}
				}
				else
				{
					this.EndAutoTravel();
				}
			}
		}
		else if (this.playerKOInProgress && !MainMenuController.Instance.mainMenuActive)
		{
			SessionData.Instance.ResumeGame();
		}
		if (MapController.Instance != null && MapController.Instance.playerRoute != null)
		{
			if (MapController.Instance.playerRoute.pathData == null || MapController.Instance.playerRoute.pathData.accessList == null || MapController.Instance.playerRoute.pathData.accessList.Count <= 0)
			{
				Game.LogError("Player route is invalid!", 2);
			}
			else
			{
				Vector3 vector3 = MapController.Instance.playerRoute.pathData.accessList[Mathf.Min(MapController.Instance.playerRoute.routeCursor + 3, MapController.Instance.playerRoute.pathData.accessList.Count - 1)].worldAccessPoint - Player.Instance.transform.position;
				vector3.y = 0f;
				Quaternion quaternion2 = Quaternion.LookRotation(vector3, Vector3.up);
				MapController.Instance.directionalArrow.rotation = Quaternion.Slerp(MapController.Instance.directionalArrow.rotation, quaternion2, 6f * SessionData.Instance.currentTimeMultiplier * Time.deltaTime);
				Vector3 vector4 = base.transform.eulerAngles - MapController.Instance.directionalArrow.eulerAngles;
				if (Mathf.Abs(vector4.y) > 45f)
				{
					MapController.Instance.directionalArrowDesiredFade = Mathf.Clamp01((Mathf.Abs(vector4.y) - 45f) / 20f);
				}
				else
				{
					MapController.Instance.directionalArrowDesiredFade = 0f;
				}
				if (MapController.Instance.directionalArrowAlpha < MapController.Instance.directionalArrowDesiredFade)
				{
					MapController.Instance.directionalArrowAlpha += 1.33f * Time.deltaTime;
					MapController.Instance.directionalArrowAlpha = Mathf.Clamp01(MapController.Instance.directionalArrowAlpha);
					MapController.Instance.arrowMaterial.SetFloat("_Alpha", MapController.Instance.directionalArrowAlpha);
				}
				else if (MapController.Instance.directionalArrowAlpha > MapController.Instance.directionalArrowDesiredFade)
				{
					MapController.Instance.directionalArrowAlpha -= 1.33f * Time.deltaTime;
					MapController.Instance.directionalArrowAlpha = Mathf.Clamp01(MapController.Instance.directionalArrowAlpha);
					MapController.Instance.arrowMaterial.SetFloat("_Alpha", MapController.Instance.directionalArrowAlpha);
				}
			}
		}
		if (this.setAlarmMode)
		{
			if (this.alarmFlash)
			{
				this.setAlarmFlashCounter -= Time.deltaTime;
				if (this.setAlarmFlashCounter <= 0f)
				{
					if (SessionData.Instance.newWatchTimeText != null)
					{
						SessionData.Instance.newWatchTimeText.gameObject.SetActive(false);
					}
					this.alarmFlash = false;
				}
			}
			else if (!this.alarmFlash)
			{
				this.setAlarmFlashCounter += Time.deltaTime;
				if (this.setAlarmFlashCounter >= 0.5f)
				{
					if (SessionData.Instance.newWatchTimeText != null)
					{
						SessionData.Instance.newWatchTimeText.gameObject.SetActive(true);
					}
					this.alarmFlash = true;
				}
			}
		}
		if (this.spendingTimeMode && !this.playerKOInProgress && !this.autoTravelActive && SessionData.Instance.gameTime >= this.alarm)
		{
			this.SetSpendingTimeMode(false);
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.watchAlarm, this, this.currentNode, this.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
			BioScreenController.Instance.SelectSlot(FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.watch), false, false);
		}
	}

	// Token: 0x060001FB RID: 507 RVA: 0x000134EC File Offset: 0x000116EC
	private void FixedUpdate()
	{
		this.lateFixedUpdate = true;
	}

	// Token: 0x060001FC RID: 508 RVA: 0x000134F5 File Offset: 0x000116F5
	private void LateUpdate()
	{
		if (this.lateFixedUpdate)
		{
			this.lateFixedUpdate = false;
			this.fps.PlayerOutOfWorldCheck();
		}
	}

	// Token: 0x060001FD RID: 509 RVA: 0x00013514 File Offset: 0x00011714
	private float GetRotationalLerpValue(Quaternion originalRotation, Quaternion targetRotation, float multiplier, out float angleBetween, float distanceToNext)
	{
		angleBetween = Quaternion.Angle(originalRotation, targetRotation);
		float num = angleBetween / 180f;
		float num2 = 1f - Mathf.Clamp01(distanceToNext / 1.8f);
		return (num + num2) * 0.5f * multiplier;
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00013554 File Offset: 0x00011754
	public void UpdateMovementPhysics(bool forceUpdateBeforeGameStart = false)
	{
		if (!forceUpdateBeforeGameStart && !SessionData.Instance.startedGame)
		{
			return;
		}
		this.UpdateGameLocation(0f);
		this.isMoving = this.fps.isMoving;
		if (this.isMoving && !this.fps.m_IsWalking)
		{
			if (!this.isRunning)
			{
				this.isRunning = true;
				this.nodesTraversedWhileWalking = 0;
				if (InterfaceController.Instance.footstepAudioIndicator != null)
				{
					InterfaceController.Instance.footstepAudioIndicator.UpdateCurrentEvent();
				}
				if (InteractionController.Instance.currentLookingAtInteractable != null)
				{
					InteractionController.Instance.currentLookingAtInteractable.interactable.UpdateCurrentActions();
				}
			}
		}
		else if (this.isRunning)
		{
			this.isRunning = false;
			if (InterfaceController.Instance.footstepAudioIndicator != null)
			{
				InterfaceController.Instance.footstepAudioIndicator.UpdateCurrentEvent();
			}
			if (InteractionController.Instance.currentLookingAtInteractable != null)
			{
				InteractionController.Instance.currentLookingAtInteractable.interactable.UpdateCurrentActions();
			}
		}
		this.isGrounded = this.fps.m_CharacterController.isGrounded;
		if (this.isGrounded != this.wasGrounded)
		{
			this.wasGrounded = this.isGrounded;
			if (InteractionController.Instance.currentLookingAtInteractable != null)
			{
				InteractionController.Instance.currentLookingAtInteractable.interactable.UpdateCurrentActions();
			}
		}
		if (this.isMoving == this.wasMoving)
		{
			if (!this.isMoving)
			{
				this.nearbyInteractableUpdate++;
				if (this.nearbyInteractableUpdate >= 10)
				{
					this.nearbyInteractableUpdate = 0;
					InteractionController.Instance.UpdateNearbyInteractables();
				}
			}
			return;
		}
		this.wasMoving = this.isMoving;
		if (!this.isMoving)
		{
			InteractionController.Instance.UpdateNearbyInteractables();
			return;
		}
		InteractionController.Instance.ClearNearbyInteractables();
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0001371C File Offset: 0x0001191C
	public void ExecuteTransition()
	{
		float num = this.currentTransition.transitionTime;
		if (this.transitionForceTime)
		{
			num = this.transtionForcedTime;
			Game.Log("Player: Force transition time: " + num.ToString(), 2);
		}
		if (this.transitionTime <= num)
		{
			this.transitionTime += Time.deltaTime;
			if (num <= 0f)
			{
				this.transitionProgress = 1f;
			}
			else
			{
				this.transitionProgress = Mathf.Clamp01(this.transitionTime / num);
			}
			float num2 = this.GetPlayerHeightNormal() * this.currentTransition.playerHeightMP;
			if (this.isCrouched && this.currentTransition.factorInCrouching)
			{
				num2 = this.GetPlayerHeightCrouched() * this.currentTransition.playerHeightMP;
			}
			float newHeight = Mathf.LerpUnclamped(this.originalPlayerHeight, num2, this.currentTransition.heightCurve.Evaluate(this.transitionProgress));
			this.SetPlayerHeight(newHeight, false);
			float num3 = GameplayControls.Instance.cameraHeightNormal * this.currentTransition.CamHeightMP;
			if (this.isCrouched && this.currentTransition.factorInCrouching)
			{
				num3 = GameplayControls.Instance.cameraHeightCrouched * this.currentTransition.CamHeightMP;
			}
			float cameraHeight = Mathf.LerpUnclamped(this.originalCamHeight, num3, this.currentTransition.camHeightCurve.Evaluate(this.transitionProgress));
			this.SetCameraHeight(cameraHeight);
			Vector3 vector = base.transform.position;
			if (this.currentTransition.useXMovement || this.currentTransition.useYMovement || this.currentTransition.useZMovement)
			{
				Vector3 zero = Vector3.zero;
				if (this.currentTransition.useXMovement)
				{
					zero.x = this.currentTransition.playerXCurve.Evaluate(this.transitionProgress);
				}
				if (this.currentTransition.useYMovement)
				{
					zero.y = this.currentTransition.playerYCurve.Evaluate(this.transitionProgress);
				}
				if (this.currentTransition.useZMovement)
				{
					zero.z = this.currentTransition.playerZCurve.Evaluate(this.transitionProgress);
				}
				if (this.currentTransition.invertZPositionBasedOnRelativePlayerZ || this.currentTransition.invertYPositionBasedOnRelativePlayerY || this.currentTransition.invertXPositionBasedOnRelativePlayerX)
				{
					Vector3 vector2 = Vector3.zero;
					if (this.transitionInteractable != null && this.transitionInteractable.controller != null)
					{
						vector2 = this.transitionInteractable.controller.transform.InverseTransformPoint(base.transform.position);
					}
					if (this.currentTransition.invertZPositionBasedOnRelativePlayerZ && vector2.z < 0f)
					{
						zero.z *= -1f;
					}
					if (this.currentTransition.invertYPositionBasedOnRelativePlayerY && vector2.y < 0f)
					{
						zero.y *= -1f;
					}
					if (this.currentTransition.invertXPositionBasedOnRelativePlayerX && vector2.x < 0f)
					{
						zero.x *= -1f;
					}
				}
				if (this.currentTransition.transitionRelativity == PlayerTransitionPreset.TransitionPosition.relativeToInteractable && this.transitionInteractable != null && this.transitionInteractable.controller != null)
				{
					vector = this.transitionInteractable.controller.transform.TransformPoint(zero);
				}
				else if (this.currentTransition.transitionRelativity == PlayerTransitionPreset.TransitionPosition.relativeToPlayer)
				{
					vector = Matrix4x4.TRS(this.originalModPosition, this.originalModRotationGlobal, Vector3.one).MultiplyPoint3x4(zero) - this.originalPlayerPosition;
					vector += this.originalPlayerPosition;
				}
				if (this.currentTransition.transitionFromExistingPosition)
				{
					vector = Vector3.Lerp(this.originalPlayerPosition, vector, this.currentTransition.positionTransitionCurve.Evaluate(this.transitionProgress));
				}
				if (this.currentTransition.transitionToSavedReturnPosition)
				{
					string text = "Player: Transitioning to the stored transition postion: ";
					Vector3 vector3 = this.storedTransitionPosition;
					Game.Log(text + vector3.ToString(), 2);
					vector = Vector3.Lerp(vector, this.storedTransitionPosition, this.currentTransition.positionTransitionCurve.Evaluate(this.transitionProgress));
				}
				base.transform.position = vector;
			}
			if (this.currentTransition.useXLook || this.currentTransition.useYLook || this.currentTransition.useZLook)
			{
				Vector3 vector4 = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				Quaternion quaternion = Quaternion.identity;
				if (this.currentTransition.useXLook)
				{
					zero2.x = this.currentTransition.playerXLookCurve.Evaluate(this.transitionProgress) * this.currentTransition.lookMovementMultiplier * this.additionalLookMultiplier.x;
				}
				if (this.currentTransition.useYLook)
				{
					zero2.y = this.currentTransition.playerYLookCurve.Evaluate(this.transitionProgress) * this.currentTransition.lookMovementMultiplier * this.additionalLookMultiplier.y;
				}
				if (this.currentTransition.useZLook)
				{
					zero2.z = this.currentTransition.playerZLookCurve.Evaluate(this.transitionProgress) * this.currentTransition.lookMovementMultiplier * this.additionalLookMultiplier.z;
				}
				if (this.transitionRecoilState)
				{
					zero2.x += this.currentTransition.playerXRecoilLookCurve.Evaluate(this.transitionProgress) * this.currentTransition.lookMovementMultiplier * this.additionalLookMultiplier.x;
					zero2.y += this.currentTransition.playerYRecoilLookCurve.Evaluate(this.transitionProgress) * this.currentTransition.lookMovementMultiplier * this.additionalLookMultiplier.y;
					zero2.z += this.currentTransition.playerZRecoilLookCurve.Evaluate(this.transitionProgress) * this.currentTransition.lookMovementMultiplier * this.additionalLookMultiplier.z;
				}
				if (this.currentTransition.lookRelativity == PlayerTransitionPreset.TransitionPosition.relativeToInteractable && this.transitionInteractable != null && this.transitionInteractable.controller != null)
				{
					Vector3 vector5 = Vector3.zero;
					if (this.transitionInteractable.controller != null)
					{
						if (this.transitionInteractable.isActor == null || (!this.transitionInteractable.isActor.isStunned && !this.transitionInteractable.isActor.isDead))
						{
							vector5 = this.transitionInteractable.controller.lockedInTransformOffset;
						}
						if (this.transitionInteractable.controller.coll != null)
						{
							vector5 += this.transitionInteractable.controller.coll.bounds.center - this.transitionInteractable.controller.transform.position;
						}
					}
					vector4 = this.transitionInteractable.controller.transform.TransformPoint(zero2 + vector5);
					vector4 -= this.camHeightParent.position;
					quaternion = Quaternion.LookRotation(vector4, Vector3.up);
				}
				else
				{
					vector4 = this.startingLookPointWorldPosition;
					Matrix4x4 matrix4x = Matrix4x4.TRS(this.originalModPosition, this.originalModRotationGlobal, Vector3.one);
					vector4 += matrix4x.MultiplyPoint3x4(zero2) - this.camHeightParent.position;
					vector4 = matrix4x.inverse.MultiplyPoint3x4(vector4);
					quaternion = Quaternion.LookRotation(vector4, Vector3.up);
				}
				if (this.currentTransition.applyCameraRoll)
				{
					quaternion *= Quaternion.Euler(new Vector3(0f, 0f, this.currentTransition.cameraRoll.Evaluate(this.transitionProgress) * this.currentTransition.rollMultiplier * this.rollMultiplier));
				}
				if (this.currentTransition.lookRelativity == PlayerTransitionPreset.TransitionPosition.relativeToInteractable)
				{
					if (this.currentTransition.transitionFromExistingMouse)
					{
						quaternion = Quaternion.Slerp(this.originalModRotationGlobal, quaternion, this.currentTransition.mouseTransitionCurve.Evaluate(this.transitionProgress));
					}
					this.camHeightParent.transform.rotation = quaternion;
				}
				else if (this.currentTransition.transitionFromExistingMouse)
				{
					quaternion = Quaternion.Slerp(this.originalModRotationLocal, quaternion, this.currentTransition.mouseTransitionCurve.Evaluate(this.transitionProgress));
				}
			}
			if (this.currentTransition.retainMovementControl)
			{
				this.fps.m_MouseLook.LookRotation(this.fps.transform, this.fps.m_Camera.transform, false);
			}
			this.fps.m_Camera.transform.localPosition = Vector3.Lerp(this.fps.m_Camera.transform.localPosition, Vector3.zero, this.transitionProgress);
			if (this.currentTransition.useChromaticAberration)
			{
				if (!SessionData.Instance.chromaticAberration.IsActive())
				{
					SessionData.Instance.chromaticAberration.active = true;
				}
				SessionData.Instance.chromaticAberration.intensity.value = this.currentTransition.chromaticAberrationCurve.Evaluate(this.transitionProgress) + StatusController.Instance.chromaticAbberationAmount;
			}
			if (this.currentTransition.useGain)
			{
				if (!SessionData.Instance.lgg.gain.overrideState)
				{
					SessionData.Instance.lgg.gain.overrideState = true;
				}
				SessionData.Instance.lgg.gain.value = new Vector4(1f, 1f, 1f, this.currentTransition.gainCurve.Evaluate(this.transitionProgress));
			}
			if (this.currentTransition.sfx.Count > 0)
			{
				foreach (PlayerTransitionPreset.SFXSetting sfxsetting in this.currentTransition.sfx)
				{
					if (!this.soundsPlayed.Contains(sfxsetting) && this.transitionProgress >= sfxsetting.atProgress)
					{
						AudioController.Instance.PlayWorldOneShot(sfxsetting.soundEvent, this, this.currentNode, this.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
						this.soundsPlayed.Add(sfxsetting);
					}
				}
			}
			if (this.currentTransition.resetCameraRoll)
			{
				Quaternion rotation = base.transform.rotation;
				rotation.x = 0f;
				rotation.z = 0f;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation, Mathf.Clamp01((this.transitionProgress - 0.9f) * 10f));
			}
		}
		if (this.transitionTime >= num)
		{
			if (this.currentTransition.resetCameraRoll)
			{
				Quaternion rotation2 = base.transform.rotation;
				rotation2.x = 0f;
				rotation2.z = 0f;
				base.transform.rotation = rotation2;
			}
			this.ConvertModifierMovementToPlayerMovement(this.currentTransition.resetCameraRoll);
			if (StatusController.Instance.chromaticAbberationAmount <= 0f)
			{
				SessionData.Instance.chromaticAberration.active = false;
			}
			SessionData.Instance.lgg.gain.overrideState = false;
			this.transitionActive = false;
			this.OnTransitionComplete();
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00014278 File Offset: 0x00012478
	public void ConvertModifierMovementToPlayerMovement(bool resetCamRoll = true)
	{
		Vector3 localEulerAngles = this.camHeightParent.transform.localEulerAngles;
		float num = 0f;
		if (!resetCamRoll)
		{
			num = localEulerAngles.z;
		}
		this.fps.m_Camera.transform.localRotation *= Quaternion.Euler(localEulerAngles.x, 0f, num);
		base.transform.localRotation *= Quaternion.Euler(0f, localEulerAngles.y, 0f);
		this.camHeightParent.transform.localRotation = Quaternion.identity;
		this.fps.m_MouseLook.Init(this.fps.transform, this.fps.m_Camera.transform);
	}

	// Token: 0x06000201 RID: 513 RVA: 0x00014344 File Offset: 0x00012544
	public void ConvertPlayerMovementToModifierMovement()
	{
		Vector3 localEulerAngles = this.fps.m_Camera.transform.localEulerAngles;
		this.camHeightParent.transform.localRotation *= Quaternion.Euler(localEulerAngles.x, 0f, 0f);
		this.fps.m_Camera.transform.localRotation = Quaternion.identity;
		this.fps.m_MouseLook.Init(this.fps.transform, this.fps.m_Camera.transform);
	}

	// Token: 0x06000202 RID: 514 RVA: 0x000143DC File Offset: 0x000125DC
	public void ForceLookAt(Interactable interactable, float time)
	{
		if (this.transitionActive)
		{
			return;
		}
		this.forceLookAtInteractable = interactable;
		this.forceLookAtTime = time;
		this.forceLookAtActive = true;
		this.originalLookAtModRotationGlobal = this.camHeightParent.transform.rotation;
		this.lookAtTime = 0f;
		this.lookAtProgress = 0f;
		this.ConvertPlayerMovementToModifierMovement();
		this.ExecuteForceLookAt();
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00014440 File Offset: 0x00012640
	public void ExecuteForceLookAt()
	{
		if (this.lookAtTime <= this.forceLookAtTime)
		{
			this.lookAtTime += Time.deltaTime;
			if (this.forceLookAtTime <= 0f)
			{
				this.lookAtProgress = 1f;
			}
			else
			{
				this.lookAtProgress = Mathf.Clamp01(this.lookAtTime / this.forceLookAtTime);
			}
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			Vector3 zero = Vector3.zero;
			if (this.forceLookAtInteractable.isActor != null && this.forceLookAtInteractable.isActor.lookAtThisTransform != null)
			{
				vector = this.forceLookAtInteractable.isActor.lookAtThisTransform.position;
			}
			else if (this.forceLookAtInteractable.controller != null)
			{
				if (this.forceLookAtInteractable.controller.coll != null)
				{
					vector = this.forceLookAtInteractable.controller.coll.bounds.center;
				}
				else if (this.forceLookAtInteractable.controller.altColl != null)
				{
					vector = this.forceLookAtInteractable.controller.altColl.bounds.center;
				}
				else if (this.forceLookAtInteractable.controller.meshes.Count > 0)
				{
					vector = this.forceLookAtInteractable.controller.meshes[0].bounds.center;
				}
				else
				{
					vector = this.forceLookAtInteractable.GetWorldPosition(true);
				}
			}
			else
			{
				vector = this.forceLookAtInteractable.controller.transform.position;
			}
			quaternion = Quaternion.LookRotation(vector - this.camHeightParent.position, Vector3.up);
			quaternion = Quaternion.Slerp(this.camHeightParent.transform.rotation, quaternion, InputController.Instance.nearestLookAtCurve.Evaluate(this.lookAtProgress));
			this.camHeightParent.transform.rotation = quaternion;
			this.fps.m_MouseLook.LookRotation(this.fps.transform, this.fps.m_Camera.transform, false);
			this.fps.m_Camera.transform.localPosition = Vector3.Lerp(this.fps.m_Camera.transform.localPosition, Vector3.zero, this.lookAtProgress);
		}
		if (this.lookAtTime >= this.forceLookAtTime)
		{
			Quaternion rotation = base.transform.rotation;
			rotation.x = 0f;
			rotation.z = 0f;
			base.transform.rotation = rotation;
			this.ConvertModifierMovementToPlayerMovement(true);
			this.forceLookAtActive = false;
			this.fps.InitialiseController(true, true);
		}
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00014700 File Offset: 0x00012900
	public void TransformPlayerController(PlayerTransitionPreset newEnterTransition, PlayerTransitionPreset newExitTransition, Interactable newInteractable, Transform newLookAt, bool newForceMovementOnEnd = false, bool forceTime = false, float forcedTime = 0f, bool useAdditionalLookMultiplier = false, Vector3 newAdditionalLookMultiplier = default(Vector3), float newRollMultiplier = 1f, bool writeReturnPosition = true)
	{
		Game.Log("Player: Enter transition: " + newEnterTransition.name + " at pos " + base.transform.position.ToString(), 1);
		if (this.spendingTimeMode)
		{
			this.SetSpendingTimeMode(false);
		}
		if (this.autoTravelActive)
		{
			this.EndAutoTravel();
		}
		this.transitionInteractable = newInteractable;
		this.currentTransition = newEnterTransition;
		this.exitTransition = newExitTransition;
		if (this.currentTransition.transitionRelativity == PlayerTransitionPreset.TransitionPosition.relativeToPlayer)
		{
			Vector3 vector = base.transform.TransformPoint(this.currentTransition.playerXCurve.Evaluate(1f), this.currentTransition.playerYCurve.Evaluate(1f), this.currentTransition.playerZCurve.Evaluate(1f));
			if (this.currentTransition.raycastCheck)
			{
				Vector3 vector2 = vector - base.transform.position;
				float num = Vector3.Distance(vector, base.transform.position);
				RaycastHit[] array = Physics.RaycastAll(new Ray(base.transform.position, vector2), num);
				bool flag = true;
				foreach (RaycastHit raycastHit in array)
				{
					InteractableController componentInParent = raycastHit.transform.GetComponentInParent<InteractableController>();
					if (!(componentInParent != null) || componentInParent.interactable != newInteractable)
					{
						Game.Log(raycastHit.transform.name + " FAIL", 2);
						flag = false;
						break;
					}
					Game.Log(raycastHit.transform.name + " OK", 2);
				}
				if (!flag)
				{
					if (this.currentTransition.onFailUse != null)
					{
						this.TransformPlayerController(this.currentTransition.onFailUse, newExitTransition, newInteractable, newLookAt, newForceMovementOnEnd, forceTime, forcedTime, false, default(Vector3), 1f, true);
					}
					return;
				}
			}
		}
		if (this.currentTransition.transitionFromExistingMouse)
		{
			this.ConvertPlayerMovementToModifierMovement();
		}
		this.originalPlayerPosition = base.transform.position;
		this.originalPlayerHeight = this.charController.height;
		this.originalCamHeight = this.camHeightParent.localPosition.y;
		this.originalModRotationGlobal = this.camHeightParent.transform.rotation;
		this.originalModRotationLocal = this.camHeightParent.transform.localRotation;
		this.originalModPosition = this.camHeightParent.transform.position;
		this.transitionRecoilState = false;
		this.soundsPlayed.Clear();
		this.additionalLookMultiplier = Vector3.one;
		this.rollMultiplier = 1f;
		if (useAdditionalLookMultiplier)
		{
			this.additionalLookMultiplier = newAdditionalLookMultiplier;
			this.rollMultiplier = newRollMultiplier;
		}
		if (this.transitionInteractable != null && this.transitionInteractable.controller != null)
		{
			if (this.currentTransition.lookRelativity == PlayerTransitionPreset.TransitionPosition.relativeToInteractable)
			{
				this.startingLookPointWorldPosition = this.transitionInteractable.controller.transform.position;
			}
			else if (this.currentTransition.lookRelativity == PlayerTransitionPreset.TransitionPosition.relativeToPlayer)
			{
				this.startingLookPointWorldPosition = this.camHeightParent.TransformPoint(Vector3.forward * this.currentTransition.forwardPositionModifier);
			}
		}
		if (forceTime)
		{
			this.transitionForceTime = true;
			this.transtionForcedTime = forcedTime;
		}
		else
		{
			this.transitionForceTime = false;
		}
		if (!this.currentTransition.transitionToSavedReturnPosition && !this.currentTransition.disableWriteReturnPosition && writeReturnPosition)
		{
			if (this.currentTransition.useCustomReturnPosition && this.transitionInteractable.controller != null)
			{
				this.storedTransitionPosition = this.transitionInteractable.controller.transform.TransformPoint(this.currentTransition.returnPostion);
				string[] array3 = new string[5];
				array3[0] = "Player: Writing stored return position: ";
				int num2 = 1;
				Vector3 vector3 = this.storedTransitionPosition;
				array3[num2] = vector3.ToString();
				array3[2] = " (";
				int num3 = 3;
				PlayerTransitionPreset playerTransitionPreset = this.currentTransition;
				array3[num3] = ((playerTransitionPreset != null) ? playerTransitionPreset.ToString() : null);
				array3[4] = ")";
				Game.Log(string.Concat(array3), 2);
			}
			else
			{
				this.storedTransitionPosition = this.originalPlayerPosition;
				string[] array4 = new string[5];
				array4[0] = "Player: Writing stored return position: ";
				int num4 = 1;
				Vector3 vector3 = this.storedTransitionPosition;
				array4[num4] = vector3.ToString();
				array4[2] = " (";
				int num5 = 3;
				PlayerTransitionPreset playerTransitionPreset2 = this.currentTransition;
				array4[num5] = ((playerTransitionPreset2 != null) ? playerTransitionPreset2.ToString() : null);
				array4[4] = ")";
				Game.Log(string.Concat(array4), 2);
			}
		}
		if (!this.currentTransition.retainMovementControl)
		{
			this.EnablePlayerMovement(false, false);
			Game.Log("Player: Disable gravity", 2);
			this.fps.m_StickToGroundForce = 0f;
			this.fps.m_GravityMultiplier = 0f;
			this.EnableCharacterController(false);
		}
		if (newLookAt != null)
		{
			this.EnablePlayerMouseLook(false, true);
		}
		this.transitionLookAt = newLookAt;
		this.transitionActive = true;
		this.transitionTime = 0f;
		this.movementOnTransitionComplete = this.currentTransition.allowMovementOnEnd;
		this.restoreHolsterOnTransitionComplete = this.currentTransition.restoreHolsterOnTransitionEnd;
		if (newForceMovementOnEnd)
		{
			this.movementOnTransitionComplete = true;
		}
		if (this.movementOnTransitionComplete)
		{
			if (this.currentTransition.restoreNormalMovementSpeed)
			{
				this.RestorePlayerMovementSpeed();
			}
			else
			{
				string text = "Player: Setting custom movement speed at end of transition: ";
				PlayerTransitionPreset playerTransitionPreset3 = this.currentTransition;
				Game.Log(text + ((playerTransitionPreset3 != null) ? playerTransitionPreset3.ToString() : null), 2);
				this.SetMaxSpeed(this.currentTransition.customMovementSpeed, this.currentTransition.customMovementSpeed);
			}
		}
		this.fps.enableHeadBob = false;
		this.fps.m_UseJumpBob = false;
		if (this.currentTransition.forceHolsterOnTransition)
		{
			FirstPersonItemController.Instance.ForceHolster();
		}
		Game.Log("Player: Disable physics", 1);
		this.ExecuteTransition();
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00014CA4 File Offset: 0x00012EA4
	public void RestorePlayerMovementSpeed()
	{
		if (this.isCrouched && !this.inAirVent)
		{
			Game.Log(string.Concat(new string[]
			{
				"Player: Restoring player movement speed: ",
				GameplayControls.Instance.playerWalkSpeed.ToString(),
				" * ",
				GameplayControls.Instance.playerStealthWalkMuliplier.ToString(),
				" (isCrouched: ",
				this.isCrouched.ToString(),
				", in air vent: ",
				this.inAirVent.ToString(),
				")..."
			}), 2);
			this.SetMaxSpeed(GameplayControls.Instance.playerWalkSpeed * GameplayControls.Instance.playerStealthWalkMuliplier, GameplayControls.Instance.playerRunSpeed * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.maxSpeedModifier)) * GameplayControls.Instance.playerStealthRunMultiplier);
			return;
		}
		Game.Log(string.Concat(new string[]
		{
			"Player: Restoring player movement speed: ",
			GameplayControls.Instance.playerWalkSpeed.ToString(),
			" (isCrouched: ",
			this.isCrouched.ToString(),
			", in air vent: ",
			this.inAirVent.ToString(),
			")..."
		}), 2);
		this.SetMaxSpeed(GameplayControls.Instance.playerWalkSpeed, GameplayControls.Instance.playerRunSpeed * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.maxSpeedModifier)));
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00014E14 File Offset: 0x00013014
	public void UpdateSkinWidth()
	{
		if (this.fps.ghostMovement)
		{
			this.charController.skinWidth = GameplayControls.Instance.ductSkinWidth;
			Game.Log("Player: Update skin width (Ghost): " + this.charController.skinWidth.ToString(), 2);
			return;
		}
		if (InteractionController.Instance.carryingObject != null)
		{
			this.charController.skinWidth = GameplayControls.Instance.carryingSkinWidth;
			Game.Log("Player: Update skin width (Carrying): " + this.charController.skinWidth.ToString(), 2);
			return;
		}
		this.charController.skinWidth = GameplayControls.Instance.normalSkinWidth;
		Game.Log("Player: Update skin width (Normal): " + this.charController.skinWidth.ToString(), 2);
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00014EEC File Offset: 0x000130EC
	public void ReturnFromTransform(bool immediate = false, bool restorePlayerTransform = true)
	{
		InteractionController.Instance.CancelInteractionAction();
		if (immediate || !restorePlayerTransform)
		{
			if (restorePlayerTransform)
			{
				if (this.isCrouched)
				{
					this.SetPlayerHeight(this.GetPlayerHeightCrouched(), true);
					this.SetCameraHeight(GameplayControls.Instance.cameraHeightCrouched);
				}
				else
				{
					this.SetPlayerHeight(this.GetPlayerHeightNormal(), true);
					this.SetCameraHeight(GameplayControls.Instance.cameraHeightNormal);
				}
			}
			this.transitionActive = false;
			this.movementOnTransitionComplete = true;
			base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y + CameraController.Instance.transform.localRotation.eulerAngles.y, base.transform.rotation.eulerAngles.z);
			if (this.exitTransition != null)
			{
				CameraController.Instance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			else
			{
				CameraController.Instance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			this.OnTransitionComplete();
			return;
		}
		bool forceTime = false;
		float forcedTime = 0f;
		if (this.transitionActive)
		{
			forceTime = true;
			forcedTime = this.transitionTime;
		}
		if (this.exitTransition != null)
		{
			this.TransformPlayerController(this.exitTransition, null, this.transitionInteractable, null, true, forceTime, forcedTime, false, default(Vector3), 1f, true);
			return;
		}
		this.TransformPlayerController(GameplayControls.Instance.defaultReturnTransition, null, this.transitionInteractable, null, true, forceTime, forcedTime, false, default(Vector3), 1f, true);
	}

	// Token: 0x06000208 RID: 520 RVA: 0x000150AC File Offset: 0x000132AC
	public void OnTransitionComplete()
	{
		if (this.currentTransition == null)
		{
			return;
		}
		Game.Log("Player: OnTransitionComplete : " + InteractionController.Instance.interactionMode.ToString(), 1);
		this.fps.InitialiseController(true, true);
		if (this.spendingTimeMode)
		{
			this.SetSpendingTimeMode(false);
		}
		if (this.autoTravelActive)
		{
			this.EndAutoTravel();
		}
		if (!InteractionController.Instance.interactionMode)
		{
			this.EnablePlayerMovement(this.movementOnTransitionComplete, true);
			this.EnablePlayerMouseLook(true, false);
		}
		if (this.restoreHolsterOnTransitionComplete)
		{
			FirstPersonItemController.Instance.RestoreItemSelection();
		}
		FirstPersonItemController.Instance.SetEnableFirstPersonItemSelection(this.currentTransition.allowWeaponSwitchingAfterTransition);
		this.SetSettingAlarmMode(false);
		if (InteractionController.Instance.lockedInInteraction == null || this.movementOnTransitionComplete)
		{
			if (!this.currentTransition.disableGravity)
			{
				Game.Log("Player: Enable gravity", 1);
				this.fps.m_StickToGroundForce = 7f;
				this.fps.m_GravityMultiplier = 2f;
			}
			if (!this.currentTransition.disableHeadBob)
			{
				this.fps.enableHeadBob = true;
				this.fps.m_UseJumpBob = true;
			}
			Game.Log("Player: Enable physics", 1);
			this.EnableCharacterController(true);
		}
		if (this.OnTransitionCompleted != null)
		{
			this.OnTransitionCompleted(false);
		}
	}

	// Token: 0x06000209 RID: 521 RVA: 0x000151F7 File Offset: 0x000133F7
	public void EnableCharacterController(bool val)
	{
		this.charController.enabled = val;
		this.transitionDamageTrigger.enabled = !this.charController.enabled;
	}

	// Token: 0x0600020A RID: 522 RVA: 0x00015220 File Offset: 0x00013420
	public override void UpdateIllegalStatus()
	{
		bool flag = false;
		string text;
		this.isTrespassing = this.IsTrespassing(this.currentRoom, out this.trespassingEscalation, out text, true);
		this.illegalAreaActive = this.isTrespassing;
		if (this.illegalAreaActive || Game.Instance.everywhereIllegal)
		{
			flag = true;
			if (this.isHiding && this.hidingInteractable == null)
			{
				this.SetHiding(false, null);
			}
		}
		if (this.illegalActionActive)
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
			Game.Log("Player: Update illegal status: " + this.illegalStatus.ToString(), 2);
			if (this.illegalStatus)
			{
				base.SetStealthMode(true);
				if (this.trespassingSnapshot == null)
				{
					this.trespassingSnapshot = AudioController.Instance.Play2DLooping(AudioControls.Instance.trespassingSnapshot, null, 1f);
				}
			}
			else
			{
				if (!this.isCrouched)
				{
					base.SetStealthMode(false);
				}
				if (this.trespassingSnapshot != null)
				{
					AudioController.Instance.StopSound(this.trespassingSnapshot, AudioController.StopType.triggerCue, "Illegal activity ended");
					this.trespassingSnapshot = null;
				}
			}
			if (InteractionController.Instance.currentLookingAtInteractable != null)
			{
				InteractionController.Instance.currentLookingAtInteractable.interactable.UpdateCurrentActions();
			}
		}
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x0600020B RID: 523 RVA: 0x00015360 File Offset: 0x00013560
	public override bool IsTrespassing(NewRoom room, out int trespassEscalation, out string debugOutput, bool enforcersAllowedEverywhere = true)
	{
		if (Game.Instance.disableTrespass)
		{
			trespassEscalation = 0;
			debugOutput = "Disable trespass";
			return false;
		}
		if (StatusController.Instance.GetCurrentDetainedStatus())
		{
			trespassEscalation = 2;
			debugOutput = "Detained";
			return true;
		}
		if (room != null && room.gameLocation != null && Enumerable.Contains<NewGameLocation>(this.apartmentsOwned, room.gameLocation))
		{
			trespassEscalation = 0;
			debugOutput = "Player owns apartment";
			return false;
		}
		bool result = base.IsTrespassing(room, out trespassEscalation, out debugOutput, enforcersAllowedEverywhere);
		if (Game.Instance.allowEchelons && trespassEscalation <= 0 && room != null && !room.featuresStairwell && room.gameLocation != null && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.floor != null && room.gameLocation.thisAsAddress.floor.isEchelons && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.allowedInEchelons) <= 0f)
		{
			trespassEscalation = Mathf.Max(trespassEscalation, 1);
			debugOutput = "Echelons Trespass";
			return true;
		}
		if (Game.Instance.allowLoitering && this.currentGameLocation != null && this.currentGameLocation.thisAsAddress != null && this.currentGameLocation.thisAsAddress.company != null && this.currentGameLocation.thisAsAddress.company.preset.enableLoiteringBehaviour && !GameplayController.Instance.guestPasses.ContainsKey(this.currentGameLocation.thisAsAddress) && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.disableLoitering) < 1f && this.currentGameLocation.playerLoiteringTimer >= GameplayControls.Instance.loiteringTrespassThreshold)
		{
			trespassEscalation = Mathf.Max(trespassEscalation, 1);
			debugOutput = "Loitering Trespass";
			return true;
		}
		return result;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0001553B File Offset: 0x0001373B
	public override void OnStealthModeChange()
	{
		InterfaceController.Instance.footstepAudioIndicator.UpdateCurrentEvent();
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0001554C File Offset: 0x0001374C
	public override void OnCrouchedChange()
	{
		this.nodesTraversedWhileWalking = 0;
		if (!Player.Instance.transitionActive || this.currentTransition == null)
		{
			this.RestorePlayerMovementSpeed();
			return;
		}
		string text = "Player: Unable to affect movement speed as the player is part of a transition (";
		PlayerTransitionPreset playerTransitionPreset = this.currentTransition;
		Game.Log(text + ((playerTransitionPreset != null) ? playerTransitionPreset.ToString() : null) + ")", 2);
	}

	// Token: 0x0600020E RID: 526 RVA: 0x000155A8 File Offset: 0x000137A8
	public void SetMaxSpeed(float newWalkSpeed, float newRunSpeed)
	{
		this.desiredWalkSpeed = newWalkSpeed;
		this.desiredRunSpeed = newRunSpeed;
		Game.Log(string.Concat(new string[]
		{
			"Player: Set player max speed: Walk = ",
			this.desiredWalkSpeed.ToString(),
			", Run = ",
			this.desiredRunSpeed.ToString(),
			" (isCrouched: ",
			this.isCrouched.ToString(),
			")"
		}), 2);
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0001561E File Offset: 0x0001381E
	public void SetCameraHeight(float newHeight)
	{
		this.camHeightParent.localPosition = new Vector3(0f, newHeight, 0f);
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0001563C File Offset: 0x0001383C
	public void SetPlayerHeight(float newHeight, bool stayOnFloorPlane = true)
	{
		if (stayOnFloorPlane)
		{
			this.charController.center = new Vector3(0f, (Player.Instance.GetPlayerHeightNormal() - this.charController.height) * -0.5f, 0f);
		}
		else
		{
			this.charController.center = Vector3.zero;
		}
		this.charController.height = newHeight;
		if (stayOnFloorPlane)
		{
			this.charController.center = new Vector3(0f, (Player.Instance.GetPlayerHeightNormal() - this.charController.height) * -0.5f, 0f);
			return;
		}
		this.charController.center = Vector3.zero;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x000156EC File Offset: 0x000138EC
	public override void UpdateLightLevel()
	{
		this.currentLightLevel = CameraController.Instance.GetPlayerLightLevel();
		if (Game.Instance.displayExtraControlHints && !this.illegalStatus && this.currentLightLevel <= 0.16f && !FirstPersonItemController.Instance.flashlight && InteractionController.Instance.lockedInInteraction == null && !Player.Instance.transitionActive && !CutSceneController.Instance.cutSceneActive)
		{
			ControlsDisplayController.Instance.DisplayControlIcon(InteractablePreset.InteractionKey.flashlight, "Flashlight", InterfaceControls.Instance.controlIconDisplayTime, false);
		}
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00015778 File Offset: 0x00013978
	public override void OnRoomChange()
	{
		base.OnRoomChange();
		SessionData.Instance.SetSceneProfile(this.currentRoom.GetEnvironment(), false);
		if (!this.inAirVent)
		{
			this.currentRoom.SetExplorationLevel(2);
		}
		this.UpdateCurrentBuildingModelVisibility();
		this.UpdateCulling();
		if (this.previousRoom != null)
		{
			foreach (Actor actor in this.previousRoom.currentOccupants)
			{
				Citizen citizen = actor as Citizen;
				if (citizen != null)
				{
					citizen.UpdateTickRateOnProx();
				}
			}
		}
		if (this.currentRoom != null)
		{
			foreach (Actor actor2 in this.currentRoom.currentOccupants)
			{
				Citizen citizen2 = actor2 as Citizen;
				if (citizen2 != null)
				{
					citizen2.UpdateTickRateOnProx();
				}
			}
			if (UpgradesController.Instance.notInstalled > 0 && this.currentRoom.preset.name == "SyncClinic")
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.interface", "Use a bed or chamber to install or uninstall Sync Disks", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.questionMark, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
		}
	}

	// Token: 0x06000213 RID: 531 RVA: 0x000158F0 File Offset: 0x00013AF0
	public override void OnTileChange()
	{
		base.OnTileChange();
	}

	// Token: 0x06000214 RID: 532 RVA: 0x000158F8 File Offset: 0x00013AF8
	public void UpdateCullingOnEndOfFrame()
	{
		if (Toolbox.Instance != null)
		{
			Toolbox.Instance.InvokeEndOfFrame(this.updateCullingAction, "Update culling");
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0001591C File Offset: 0x00013B1C
	public void UpdateCulling()
	{
		SessionData.Instance.ExecuteSyncPhysics(SessionData.PhysicsSyncType.onPlayerMovement);
		if (GeometryCullingController.Instance != null)
		{
			GeometryCullingController.Instance.UpdateCullingForRoom(this.currentRoom, true, this.inAirVent, this.currentDuct, false);
		}
		ObjectPoolingController.Instance.UpdateObjectRanges();
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0001596C File Offset: 0x00013B6C
	public override void SetResidence(ResidenceController newHome, bool removePreviousResidence = true)
	{
		Game.Log("Player: Set player residence: " + ((newHome != null) ? newHome.ToString() : null), 2);
		base.SetResidence(newHome, removePreviousResidence);
		if (newHome != null && newHome.address != null && newHome.address.mapButton != null)
		{
			MapController.Instance.AddUpdateCall(newHome.address.mapButton);
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x000159E0 File Offset: 0x00013BE0
	public override void AddToKeyring(NewAddress ad, bool gameMessage = true)
	{
		base.AddToKeyring(ad, false);
		if (gameMessage)
		{
			string msg = Strings.Get("ui.gamemessage", "key", Strings.Casing.asIs, false, false, false, null) + " " + ad.name;
			if (!InterfaceController.Instance.notificationQueue.Exists((InterfaceController.GameMessage item) => item.message == msg))
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, msg, InterfaceControls.Icon.key, null, true, InterfaceControls.Instance.messageGrey, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x00015A70 File Offset: 0x00013C70
	public override void AddToKeyring(NewDoor ac, bool gameMessage = true)
	{
		base.AddToKeyring(ac, false);
		ac.SetPlayerHasKey(true);
		if (ac.preset.lockType == DoorPreset.LockType.key && gameMessage)
		{
			string msg = Strings.Get("ui.gamemessage", "key", Strings.Casing.asIs, false, false, false, null) + " " + ac.GetName();
			if (!InterfaceController.Instance.notificationQueue.Exists((InterfaceController.GameMessage item) => item.message == msg))
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, msg, InterfaceControls.Icon.key, null, true, InterfaceControls.Instance.messageGrey, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
		}
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00015B15 File Offset: 0x00013D15
	public void AddToKeyring(Interactable inter, bool gameMessage = true)
	{
		if (inter == null)
		{
			return;
		}
		if (!this.playerKeyringInt.Contains(inter))
		{
			this.playerKeyringInt.Add(inter);
		}
		inter.SetCustomState3(true, null, true, false, false);
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00015B40 File Offset: 0x00013D40
	public override void RemoveFromKeyring(NewDoor ac)
	{
		base.RemoveFromKeyring(ac);
		ac.SetPlayerHasKey(false);
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00015B50 File Offset: 0x00013D50
	public void RemoveFromKeyring(Interactable inter)
	{
		if (inter == null)
		{
			return;
		}
		this.playerKeyringInt.Remove(inter);
		inter.SetCustomState3(false, null, true, false, false);
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00015B70 File Offset: 0x00013D70
	public void TriggerPlayerKO(Vector3 KODirection, float RollMP, bool forceDirtyDeath = false)
	{
		if (this.playerKOInProgress)
		{
			return;
		}
		if (InteractionController.Instance.dialogMode)
		{
			return;
		}
		if (BioScreenController.Instance.isOpen)
		{
			BioScreenController.Instance.SetInventoryOpen(false, false, true);
		}
		Game.Log("Player: Player KO!", 1);
		if (this.spendingTimeMode)
		{
			this.SetSpendingTimeMode(false);
		}
		if (this.autoTravelActive)
		{
			this.EndAutoTravel();
		}
		if (InteractionController.Instance.carryingObject != null)
		{
			InteractionController.Instance.carryingObject.DropThis(false);
		}
		if (Player.Instance.currentBuilding != null)
		{
			StatusController.Instance.SetDetainedInBuilding(Player.Instance.currentBuilding, false);
		}
		if (AchievementsController.Instance != null)
		{
			AchievementsController.Instance.notAScratchFlag = true;
		}
		this.playerKOInProgress = true;
		this.KORecovery = false;
		this.paidFines = false;
		this.dirtyDeath = false;
		this.debtPayment = null;
		InteractionController.Instance.UpdateNearbyInteractables();
		if (InteractionController.Instance.mugger != null && InteractionController.Instance.mugger == this.lastDmgFrom)
		{
			this.dirtyDeath = true;
		}
		if (InteractionController.Instance.debtCollector != null && InteractionController.Instance.debtCollector == this.lastDmgFrom)
		{
			this.dirtyDeath = true;
			if (InteractionController.Instance.debtCollector.job != null && InteractionController.Instance.debtCollector.job.employer != null)
			{
				this.debtPayment = GameplayController.Instance.debt.Find((GameplayController.LoanDebt item) => item.companyID == InteractionController.Instance.debtCollector.job.employer.companyID);
			}
		}
		if (forceDirtyDeath)
		{
			this.dirtyDeath = true;
		}
		this.lastDmgFrom = null;
		SessionData.Instance.SetEnablePause(false);
		base.SetInteracting(null);
		this.ReturnFromTransform(true, true);
		FirstPersonItemController.Instance.ForceHolster();
		this.TransformPlayerController(GameplayControls.Instance.playerKO, null, null, null, false, false, 0f, true, KODirection, RollMP, true);
		this.playerKOFadeOut = false;
		this.KOTime = GameplayControls.Instance.koTimePass;
		this.KOTimePassed = 0f;
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			newBuilding.wantedInBuilding = 0f;
		}
		InterfaceController.Instance.Fade(1f, 2f, false);
		this.SetActionDisable("Get In", true);
		this.SetActionDisable("Get Up", true);
		this.SetActionDisable("Enter", true);
		this.SetActionDisable("Sleep...", true);
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00015E30 File Offset: 0x00014030
	public void TriggerPlayerRecovery()
	{
		if (this.playerKOInProgress)
		{
			return;
		}
		Game.Log("Player: Player Recovery!", 1);
		this.playerKOInProgress = true;
		InteractionController.Instance.UpdateNearbyInteractables();
		this.KORecovery = true;
		SessionData.Instance.SetEnablePause(false);
		base.SetInteracting(null);
		FirstPersonItemController.Instance.ForceHolster();
		this.playerKOFadeOut = false;
		this.KOTime = 0.5f;
		this.KOTimePassed = 0f;
		InterfaceController.Instance.Fade(1f, 2f, false);
		this.SetActionDisable("Get In", true);
		this.SetActionDisable("Get Up", true);
		this.SetActionDisable("Enter", true);
		this.SetActionDisable("Sleep...", true);
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00015EF4 File Offset: 0x000140F4
	public override void Teleport(NewNode teleportLocation, Interactable.UsagePoint usagePoint, bool cancelVent = true, bool teleportYPostionOnly = false)
	{
		if (teleportLocation == null)
		{
			Game.Log("Player: Teleport cancelled as location is null", 2);
			return;
		}
		base.SetInteracting(null);
		InteractionController.Instance.CancelInteractionAction();
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		if (this.fps.m_CharacterController != null)
		{
			this.fps.m_CharacterController.SimpleMove(Vector3.zero);
		}
		if (cancelVent && this.inAirVent)
		{
			Game.Log("Player: Cancelling vent...", 2);
			this.ExitVent(true);
		}
		if (teleportLocation.defaultSpace != null)
		{
			if (teleportYPostionOnly)
			{
				base.transform.position = new Vector3(base.transform.position.x, teleportLocation.defaultSpace.position.y + 2.5f, base.transform.position.z);
			}
			else
			{
				base.transform.position = teleportLocation.defaultSpace.position + new Vector3(0f, 2.5f, 0f);
			}
		}
		else if (teleportYPostionOnly)
		{
			base.transform.position = new Vector3(base.transform.position.x, teleportLocation.position.y + 2.5f, base.transform.position.z);
		}
		else
		{
			base.transform.position = teleportLocation.position + new Vector3(0f, 2.5f, 0f);
		}
		this.fps.lastY = base.transform.position.y;
		this.fps.fallCount = 0f;
		string[] array = new string[6];
		array[0] = "Player: Teleported player to position ";
		array[1] = base.transform.position.ToString();
		array[2] = " node ";
		int num = 3;
		Vector3Int nodeCoord = teleportLocation.nodeCoord;
		array[num] = nodeCoord.ToString();
		array[4] = " room ";
		array[5] = teleportLocation.room.GetName();
		Game.Log(string.Concat(array), 1);
		this.UpdateGameLocation(0f);
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00016110 File Offset: 0x00014310
	public void SetPosition(Vector3 newWorldPos, Quaternion newRot)
	{
		base.SetInteracting(null);
		InteractionController.Instance.CancelInteractionAction();
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		base.transform.position = newWorldPos + new Vector3(0f, 0f, 0f);
		base.transform.rotation = newRot;
		this.fps.InitialiseController(false, true);
		this.UpdateGameLocation(0f);
		string text = "Player: Set player to position ";
		Vector3 vector = newWorldPos;
		string text2 = vector.ToString();
		string text3 = ", new game location: ";
		NewGameLocation currentGameLocation = this.currentGameLocation;
		Game.Log(text + text2 + text3 + ((currentGameLocation != null) ? currentGameLocation.ToString() : null), 1);
	}

	// Token: 0x06000220 RID: 544 RVA: 0x000161BC File Offset: 0x000143BC
	public void UpdatePlayerAmbientState()
	{
		if (SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive) && !SessionData.Instance.isFloorEdit)
		{
			if (this.persuedBy.Count > 0)
			{
				MusicController.Instance.SetPlayerState(MusicCue.MusicTriggerPlayerState.combat);
			}
			else if (this.isTrespassing && this.trespassingEscalation >= 2)
			{
				MusicController.Instance.SetPlayerState(MusicCue.MusicTriggerPlayerState.trespass);
			}
			else if (this.spendingTimeMode)
			{
				MusicController.Instance.SetPlayerState(MusicCue.MusicTriggerPlayerState.passingTime);
			}
			else
			{
				MusicController.Instance.SetPlayerState(MusicCue.MusicTriggerPlayerState.safe);
			}
			if (this.isOnStreet)
			{
				MusicController.Instance.SetPlayerLocation(MusicCue.MusicTriggerPlayerLocation.outdoors);
				return;
			}
			if (this.currentGameLocation != null)
			{
				if (this.currentGameLocation == this.home || Enumerable.Contains<NewGameLocation>(this.apartmentsOwned, this.currentGameLocation))
				{
					MusicController.Instance.SetPlayerLocation(MusicCue.MusicTriggerPlayerLocation.playersApartment);
					return;
				}
				if (this.currentGameLocation.IsOutside())
				{
					MusicController.Instance.SetPlayerLocation(MusicCue.MusicTriggerPlayerLocation.outdoors);
					return;
				}
				MusicController.Instance.SetPlayerLocation(MusicCue.MusicTriggerPlayerLocation.indoors);
			}
		}
	}

	// Token: 0x06000221 RID: 545 RVA: 0x000162D4 File Offset: 0x000144D4
	public void OnHide(Interactable newHideInteractable, int reference = 0, bool instant = false, bool allowReturnPositionWrite = true)
	{
		if (InteractionController.Instance.hideInteractable != null)
		{
			InteractionController.Instance.hideInteractable.SetSwitchState(false, this, true, false, false);
		}
		InteractionController.Instance.hideInteractable = newHideInteractable;
		this.hideInteractable = newHideInteractable;
		this.hideReference = reference;
		InteractionController.Instance.SetLockedInInteractionMode(InteractionController.Instance.hideInteractable, reference, true);
		PlayerTransitionPreset playerTransitionPreset = null;
		PlayerTransitionPreset playerTransitionPreset2 = null;
		bool flag = true;
		if (reference == 0)
		{
			if (InteractionController.Instance.hideInteractable.furnitureParent != null)
			{
				playerTransitionPreset = InteractionController.Instance.hideInteractable.furnitureParent.furniture.hidingEnterTransition;
				playerTransitionPreset2 = InteractionController.Instance.hideInteractable.furnitureParent.furniture.hidingExitTransition;
			}
			if (playerTransitionPreset == null || InteractionController.Instance.hideInteractable.preset.overrideFurnitureSetting)
			{
				playerTransitionPreset = InteractionController.Instance.hideInteractable.preset.enterTransition;
				playerTransitionPreset2 = InteractionController.Instance.hideInteractable.preset.exitTransition;
			}
		}
		else if (reference == 1)
		{
			if (InteractionController.Instance.hideInteractable.furnitureParent != null)
			{
				playerTransitionPreset = InteractionController.Instance.hideInteractable.furnitureParent.furniture.hidingEnterTransition2;
				playerTransitionPreset2 = InteractionController.Instance.hideInteractable.furnitureParent.furniture.hidingExitTransition2;
			}
			if (playerTransitionPreset == null || InteractionController.Instance.hideInteractable.preset.overrideFurnitureSetting)
			{
				playerTransitionPreset = InteractionController.Instance.hideInteractable.preset.enterTransition2;
				playerTransitionPreset2 = InteractionController.Instance.hideInteractable.preset.exitTransition2;
			}
			flag = false;
		}
		if (!allowReturnPositionWrite)
		{
			flag = false;
		}
		PlayerTransitionPreset newEnterTransition = playerTransitionPreset;
		PlayerTransitionPreset newExitTransition = playerTransitionPreset2;
		Interactable newInteractable = InteractionController.Instance.hideInteractable;
		Transform newLookAt = null;
		bool newForceMovementOnEnd = false;
		float forcedTime = 0f;
		bool useAdditionalLookMultiplier = false;
		bool writeReturnPosition = flag;
		this.TransformPlayerController(newEnterTransition, newExitTransition, newInteractable, newLookAt, newForceMovementOnEnd, instant, forcedTime, useAdditionalLookMultiplier, default(Vector3), 1f, writeReturnPosition);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromHide;
		if (newHideInteractable.preset.specialCaseFlag == InteractablePreset.SpecialCase.syncBed)
		{
			UpgradesController.Instance.UpdateInstallButton(true);
			if (newHideInteractable.preset.presetName == "SyncChamber")
			{
				if (this.syncMachineSnapshot == null)
				{
					this.syncMachineSnapshot = AudioController.Instance.Play2DLooping(AudioControls.Instance.syncMachineSnapshot, null, 1f);
				}
			}
			else if (this.syncMachineSnapshot != null)
			{
				AudioController.Instance.StopSound(this.syncMachineSnapshot, AudioController.StopType.triggerCue, "Sync machine ended");
				this.syncMachineSnapshot = null;
			}
		}
		if (this.hideInteractable != null)
		{
			this.hideInteractable.UpdateCurrentActions();
			InteractionController.Instance.UpdateInteractionText();
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00016550 File Offset: 0x00014750
	public void OnReturnFromHide()
	{
		this.hideInteractable = null;
		this.SetHiding(false, null);
		if (InteractionController.Instance.hideInteractable.furnitureParent != null)
		{
			foreach (Interactable interactable in InteractionController.Instance.hideInteractable.furnitureParent.integratedInteractables)
			{
				if (interactable.aiActionReference.ContainsKey(RoutineControls.Instance.openLocker))
				{
					interactable.OnInteraction(interactable.aiActionReference[RoutineControls.Instance.openLocker], this, true, 0f);
					interactable.controller.transform.localPosition = new Vector3(interactable.controller.transform.localPosition.x + 0.01f * interactable.controller.transform.localScale.x, interactable.controller.transform.localPosition.y, interactable.controller.transform.localPosition.z);
				}
			}
		}
		if (this.syncMachineSnapshot != null)
		{
			AudioController.Instance.StopSound(this.syncMachineSnapshot, AudioController.StopType.triggerCue, "Sync machine ended");
			this.syncMachineSnapshot = null;
		}
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromHide;
		this.ReturnFromTransform(false, true);
		UpgradesController.Instance.UpdateInstallButton(false);
	}

	// Token: 0x06000223 RID: 547 RVA: 0x000166D0 File Offset: 0x000148D0
	public void OnAnswerPhone(Interactable newPhone)
	{
		this.answeringPhone = newPhone.t;
		this.phoneInteractable = newPhone;
		InteractionController.Instance.SetLockedInInteractionMode(newPhone, 0, true);
		this.EnablePlayerMovement(false, true);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromAnswerPhone;
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00016710 File Offset: 0x00014910
	public void OnReturnFromAnswerPhone()
	{
		if (this.answeringPhone != null)
		{
			this.answeringPhone.SetTelephoneAnswered(null);
		}
		this.answeringPhone = null;
		this.phoneInteractable = null;
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromAnswerPhone;
		InteractionController.Instance.SetDialog(false, null, false, null, InteractionController.ConversationType.normal);
		this.EnablePlayerMovement(true, true);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0001676C File Offset: 0x0001496C
	public void OnCrawlIntoVent(Interactable vent, bool instant = false)
	{
		base.SetCrouched(false, true);
		this.TransformPlayerController(GameplayControls.Instance.enterVentTransition, null, vent, null, true, instant, 0f, false, default(Vector3), 1f, true);
		this.inAirVent = true;
		this.OnTransitionCompleted += this.EnterVent;
		if (instant)
		{
			this.EnterVent(false);
		}
	}

	// Token: 0x06000226 RID: 550 RVA: 0x000167D0 File Offset: 0x000149D0
	public void OnCrawlOutOfVent(Interactable vent, bool instant = false)
	{
		this.TransformPlayerController(GameplayControls.Instance.exitVentTransition, null, vent, null, true, instant, 0f, false, default(Vector3), 1f, true);
		this.inAirVent = false;
		this.OnTransitionCompleted += this.ExitVent;
	}

	// Token: 0x06000227 RID: 551 RVA: 0x00016820 File Offset: 0x00014A20
	public void EnterVent(bool restoreTransform = false)
	{
		this.inAirVent = true;
		this.OnTransitionCompleted -= this.EnterVent;
		Player.Instance.ReturnFromTransform(false, false);
		this.EnableGhostMovement(true, true, 0f);
		AudioController.Instance.UpdateMixing();
		AudioController.Instance.UpdateVentIndoorOutdoor();
		AudioController.Instance.UpdateDistanceToVent();
		Game.Log("Player: EnterVent", 2);
		this.RestorePlayerMovementSpeed();
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00016890 File Offset: 0x00014A90
	public void ExitVent(bool restoreTransform = false)
	{
		this.inAirVent = false;
		this.OnTransitionCompleted -= this.ExitVent;
		Player.Instance.ReturnFromTransform(true, restoreTransform);
		this.EnableGhostMovement(false, false, 0f);
		AudioController.Instance.UpdateMixing();
		AudioController.Instance.UpdateVentIndoorOutdoor();
		AudioController.Instance.UpdateDistanceToVent();
		this.RestorePlayerMovementSpeed();
	}

	// Token: 0x06000229 RID: 553 RVA: 0x000168F4 File Offset: 0x00014AF4
	public void OnUseComputer(Interactable newComp, bool instant = false)
	{
		this.computerInteractable = newComp;
		SessionData.Instance.TutorialTrigger("computers", false);
		SessionData.Instance.TutorialTrigger("surveillance", false);
		SessionData.Instance.TutorialTrigger("passwords", false);
		Game.Log("Player: Use computer", 2);
		InteractionController.Instance.SetLockedInInteractionMode(newComp, 0, true);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.playerUseComputer, GameplayControls.Instance.playerComputerExit, newComp, null, false, instant, 0f, false, default(Vector3), 1f, true);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromUseComputer;
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0001699D File Offset: 0x00014B9D
	public void OnReturnFromUseComputer()
	{
		this.computerInteractable = null;
		Game.Log("Player: Return from computer", 2);
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromUseComputer;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		this.ReturnFromTransform(false, true);
	}

	// Token: 0x0600022B RID: 555 RVA: 0x000169DC File Offset: 0x00014BDC
	public void OnTakePrint(Interactable newHand)
	{
		InteractionController.Instance.SetLockedInInteractionMode(newHand, 0, true);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.playerSearch, GameplayControls.Instance.playerSearchExit, newHand, null, false, false, 0f, false, default(Vector3), 1f, true);
		InteractionController.Instance.SetInteractionAction(0f, 1f, 1f, "takeprint", false, false, newHand.controller.transform, true);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromTakePrint;
		InteractionController.Instance.OnInteractionActionCompleted += this.OnCompleteTakePrint;
		if (newHand.isActor != null && newHand.isActor.ai != null && !newHand.isActor.isDead && !newHand.isActor.isStunned && !newHand.isActor.isAsleep && !newHand.isActor.ai.restrained)
		{
			Game.Log(newHand.isActor.name + " sees player taking print and isn't happy about it!", 2);
			newHand.isActor.ai.SetPersue(Player.Instance, false, 1, true, 0f);
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00016B1C File Offset: 0x00014D1C
	public void OnCompleteTakePrint()
	{
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		list.Add(Evidence.DataKey.photo);
		if (!InteractionController.Instance.lockedInInteraction.isActor.evidenceEntry.GetTiedKeys(list).Contains(Evidence.DataKey.fingerprints))
		{
			InteractionController.Instance.lockedInInteraction.isActor.evidenceEntry.MergeDataKeys(Evidence.DataKey.photo, Evidence.DataKey.fingerprints);
		}
		this.OnReturnFromTakePrint();
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00016B7C File Offset: 0x00014D7C
	public void OnReturnFromTakePrint()
	{
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromTakePrint;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteTakePrint;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		this.ReturnFromTransform(false, true);
	}

	// Token: 0x0600022E RID: 558 RVA: 0x00016BCC File Offset: 0x00014DCC
	public void OnSearch(Interactable newSearchItem)
	{
		this.searchInteractable = newSearchItem;
		AudioController.Instance.StopSound(this.searchInteractable.actionLoop, AudioController.StopType.immediate, "Stop search");
		this.searchInteractable.actionLoop = AudioController.Instance.PlayWorldLooping(this.searchInteractable.preset.searchLoop, Player.Instance, this.searchInteractable, null, 1f, false, false, null, null);
		InteractionController.Instance.SetLockedInInteractionMode(newSearchItem, 0, true);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.playerSearch, GameplayControls.Instance.playerSearchExit, newSearchItem, null, false, false, 0f, false, default(Vector3), 1f, true);
		bool cancelIfTooFar = true;
		if (this.searchInteractable.isActor != null && this.searchInteractable.isActor.isDead)
		{
			cancelIfTooFar = false;
		}
		InteractionController.Instance.SetInteractionAction(0f, 1f, 1f, "search", false, false, newSearchItem.controller.transform, cancelIfTooFar);
		if (newSearchItem.objectRef != null)
		{
			Human human = newSearchItem.objectRef as Human;
			if (human != null)
			{
				foreach (Interactable interactable in human.inventory)
				{
					Game.Log("Player: Add search discovery over time (interactable): " + interactable.name + "...", 2);
					if (!InteractionController.Instance.discoveryOverTime.ContainsKey(interactable))
					{
						InteractionController.Instance.discoveryOverTime.Add(interactable, Toolbox.Instance.Rand(0.1f, 0.85f, false));
					}
				}
				if (human.isDead)
				{
					foreach (Human.Wound wound in human.currentWounds)
					{
						if (wound.interactable != null)
						{
							InteractionController.Instance.discoveryOverTime.Add(wound.interactable, Toolbox.Instance.Rand(0.1f, 0.7f, false));
							break;
						}
					}
					InteractionController.Instance.discoveryOverTimeDiscovery.Add(new EvidenceMultiPage.MultiPageContent
					{
						discEvID = human.evidenceEntry.evID,
						disc = Evidence.Discovery.timeOfDeath
					}, 0.9f);
					Evidence timeOfDeathEvidence = human.death.GetTimeOfDeathEvidence();
					if (!InteractionController.Instance.discoveryOverTimeEvidence.ContainsKey(timeOfDeathEvidence))
					{
						InteractionController.Instance.discoveryOverTimeEvidence.Add(timeOfDeathEvidence, 0.9f);
					}
				}
			}
		}
		if (newSearchItem.evidence != null)
		{
			EvidenceMultiPage evidenceMultiPage = newSearchItem.evidence as EvidenceMultiPage;
			if (evidenceMultiPage != null)
			{
				foreach (EvidenceMultiPage.MultiPageContent multiPageContent in evidenceMultiPage.pageContent)
				{
					if (multiPageContent.evID != null && multiPageContent.evID.Length > 0)
					{
						Evidence evidence = null;
						if (GameplayController.Instance.evidenceDictionary.TryGetValue(multiPageContent.evID, ref evidence))
						{
							Game.Log("Player: Add search discovery over time (contained evidence): " + evidence.preset.name + "...", 2);
							if (!InteractionController.Instance.discoveryOverTimeEvidence.ContainsKey(evidence))
							{
								InteractionController.Instance.discoveryOverTimeEvidence.Add(evidence, Toolbox.Instance.Rand(0.1f, 0.99f, false));
							}
						}
						else
						{
							Game.LogError("Unable to find evidence " + multiPageContent.evID, 2);
						}
					}
					else if (multiPageContent.meta > 0)
					{
						MetaObject metaObject = CityData.Instance.FindMetaObject(multiPageContent.meta);
						if (metaObject != null)
						{
							Game.Log("Player: Add search discovery over time (meta object): " + metaObject.preset + "...", 2);
							if (!InteractionController.Instance.discoveryOverTimeMeta.ContainsKey(metaObject))
							{
								InteractionController.Instance.discoveryOverTimeMeta.Add(metaObject, Toolbox.Instance.Rand(0.1f, 0.99f, false));
							}
						}
					}
					else if (multiPageContent.discEvID != null && multiPageContent.discEvID.Length > 0 && !InteractionController.Instance.discoveryOverTimeDiscovery.ContainsKey(multiPageContent))
					{
						InteractionController.Instance.discoveryOverTimeDiscovery.Add(multiPageContent, Toolbox.Instance.Rand(0.1f, 0.99f, false));
					}
				}
			}
		}
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromSearch;
		InteractionController.Instance.OnInteractionActionCompleted += this.OnCompleteSearch;
	}

	// Token: 0x0600022F RID: 559 RVA: 0x000170A4 File Offset: 0x000152A4
	public void OnCompleteSearch()
	{
		if (InteractionController.Instance.lockedInInteraction != null)
		{
			InteractionController.Instance.lockedInInteraction.ins = true;
		}
		if (AchievementsController.Instance != null && this.searchInteractable != null)
		{
			if (this.searchInteractable.preset.actionsPreset.Exists((InteractableActionsPreset item) => item.presetName == "Bin"))
			{
				AchievementsController.Instance.AddToStat("Dumpster Diver", "trash_searched", 1);
			}
		}
		this.OnReturnFromSearch();
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00017134 File Offset: 0x00015334
	public void OnReturnFromSearch()
	{
		if (this.searchInteractable != null)
		{
			AudioController.Instance.StopSound(this.searchInteractable.actionLoop, AudioController.StopType.fade, "Return from search");
			this.searchInteractable.actionLoop = null;
		}
		this.searchInteractable = null;
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromSearch;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteSearch;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		this.ReturnFromTransform(false, true);
	}

	// Token: 0x06000231 RID: 561 RVA: 0x000171B8 File Offset: 0x000153B8
	public void OnDrink(Interactable newSearchItem)
	{
		this.searchInteractable = newSearchItem;
		InteractionController.Instance.SetLockedInInteractionMode(newSearchItem, 0, true);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.playerSearch, GameplayControls.Instance.playerSearchExit, newSearchItem, null, false, false, 0f, false, default(Vector3), 1f, true);
		bool cancelIfTooFar = true;
		InteractionController.Instance.SetInteractionAction(0f, 1f, 1f, "drinking", false, false, newSearchItem.controller.transform, cancelIfTooFar);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromDrink;
		InteractionController.Instance.OnInteractionActionCompleted += this.OnCompleteDrink;
		InteractionController.Instance.OnInteractionActionLookedAway += this.OnLookAwayFromFountain;
		InteractionController.Instance.OnInteractionActionProgressChange += this.DrinkProgress;
	}

	// Token: 0x06000232 RID: 562 RVA: 0x00017298 File Offset: 0x00015498
	public void DrinkProgress(float amountChangeThisFrame, float amountToal)
	{
		if (!this.drinkLoopStarted)
		{
			this.drinkLoopStarted = true;
			this.drinkLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.drinkLoop, Player.Instance, this.searchInteractable, null, 1f, false, false, null, null);
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x000172E3 File Offset: 0x000154E3
	public void OnLookAwayFromFountain()
	{
		this.drinkLoopStarted = false;
		AudioController.Instance.StopSound(this.drinkLoop, AudioController.StopType.immediate, "Stopping drink");
	}

	// Token: 0x06000234 RID: 564 RVA: 0x00017302 File Offset: 0x00015502
	public void OnCompleteDrink()
	{
		AudioController.Instance.StopSound(this.drinkLoop, AudioController.StopType.immediate, "Stopping drink");
		this.OnReturnFromDrink();
		this.AddHydration(0.5f);
	}

	// Token: 0x06000235 RID: 565 RVA: 0x0001732C File Offset: 0x0001552C
	public void OnReturnFromDrink()
	{
		if (InteractionController.Instance.interactionActionAmount >= 0.5f)
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.waterCoolerRefill, this, this.interactable.node, this.interactable.node.position, null, null, 1f, null, false, null, false);
		}
		if (InteractionController.Instance.interactionActionAmount >= 0.1f && InteractionController.Instance.interactionActionAmount < 0.5f)
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.waterCoolerRefillShort, this, this.interactable.node, this.interactable.node.position, null, null, 1f, null, false, null, false);
		}
		if (this.drinkLoop != null)
		{
			AudioController.Instance.StopSound(this.drinkLoop, AudioController.StopType.fade, "Return from drink");
		}
		if (this.searchInteractable != null)
		{
			AudioController.Instance.StopSound(this.searchInteractable.actionLoop, AudioController.StopType.fade, "Return from drink");
			this.searchInteractable.actionLoop = null;
		}
		this.searchInteractable = null;
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromDrink;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteDrink;
		InteractionController.Instance.OnInteractionActionLookedAway -= this.OnLookAwayFromFountain;
		InteractionController.Instance.OnInteractionActionProgressChange -= this.DrinkProgress;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		this.ReturnFromTransform(false, true);
	}

	// Token: 0x06000236 RID: 566 RVA: 0x000174A5 File Offset: 0x000156A5
	public void OnInteractionActionProgress(float amountThisFrame, float interactionActionAmount)
	{
		if (InteractionController.Instance.activeInteractionAction && InteractionController.Instance.interactionActionName == "Vomiting")
		{
			this.AddSick(-1f * amountThisFrame);
			this.AddHygiene(-0.5f * amountThisFrame);
		}
	}

	// Token: 0x06000237 RID: 567 RVA: 0x000174E4 File Offset: 0x000156E4
	public void OnGenericTimedAction(string actionName, float threshold, float increaseRate, Interactable newItem, bool playObjectsSearchLoop = false)
	{
		this.genericActionInteractable = newItem;
		if (playObjectsSearchLoop && newItem != null)
		{
			AudioController.Instance.StopSound(newItem.actionLoop, AudioController.StopType.immediate, "Stop generic action");
			newItem.actionLoop = AudioController.Instance.PlayWorldLooping(newItem.preset.searchLoop, Player.Instance, newItem, null, 1f, false, false, null, null);
		}
		InteractionController.Instance.SetLockedInInteractionMode(newItem, 0, true);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.playerSearch, GameplayControls.Instance.playerSearchExit, newItem, null, false, false, 0f, false, default(Vector3), 1f, true);
		InteractionController.Instance.SetInteractionAction(0f, threshold, increaseRate, actionName, false, false, newItem.controller.transform, true);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromGenericAction;
		InteractionController.Instance.OnInteractionActionCompleted += this.OnReturnFromGenericAction;
	}

	// Token: 0x06000238 RID: 568 RVA: 0x000175DC File Offset: 0x000157DC
	public void OnReturnFromGenericAction()
	{
		AudioController.Instance.StopSound(this.genericActionInteractable.actionLoop, AudioController.StopType.fade, "Return from action");
		this.genericActionInteractable.actionLoop = null;
		this.genericActionInteractable = null;
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromGenericAction;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnReturnFromGenericAction;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		this.ReturnFromTransform(false, true);
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00017658 File Offset: 0x00015858
	public void OnHandcuff(Interactable newBody)
	{
		Game.Log("Player: handcuff " + newBody.name, 2);
		this.restrainedHandcuffsSlot = BioScreenController.Instance.selectedSlot;
		InteractionController.Instance.SetLockedInInteractionMode(newBody, 0, true);
		this.restrainedInteractable = newBody;
		AudioController.Instance.StopSound(this.restrainedInteractable.actionLoop, AudioController.StopType.immediate, "Stop search");
		this.restrainedInteractable.actionLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.handcuff, Player.Instance, this.restrainedInteractable, null, 1f, false, false, null, null);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.playerSearch, GameplayControls.Instance.playerSearchExit, newBody, null, false, false, 0f, false, default(Vector3), 1f, true);
		InteractionController.Instance.SetInteractionAction(0f, 1f, 1f, "handcuff", false, false, null, false);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromHandcuff;
		InteractionController.Instance.OnInteractionActionCompleted += this.OnCompleteHandcuff;
		this.restrainedInteractable.isActor.ai.SetRestrained(true, GameplayControls.Instance.restrainedTimer);
		bool publicFauxPas = true;
		if (this.restrainedInteractable.isActor.persuedBy.Count > 0 || this.restrainedInteractable.isActor.illegalStatus)
		{
			publicFauxPas = false;
		}
		this.restrainedInteractable.isActor.ai.SetPersue(Player.Instance, publicFauxPas, 1, true, CitizenControls.Instance.punchedResponseRange);
	}

	// Token: 0x0600023A RID: 570 RVA: 0x000177E8 File Offset: 0x000159E8
	public void OnCompleteHandcuff()
	{
		Game.Log("Player: Complete handcuff", 2);
		AudioController.Instance.StopSound(this.restrainedInteractable.actionLoop, AudioController.StopType.immediate, "Stop search");
		AudioController.Instance.Play2DSound(AudioControls.Instance.handcuffArrestEnd, null, 1f);
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromHandcuff;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteHandcuff;
		Interactable interactable = this.restrainedHandcuffsSlot.GetInteractable();
		if (interactable != null)
		{
			FirstPersonItemController.Instance.EmptySlot(this.restrainedHandcuffsSlot, false, false, true, false);
			interactable.SetAsNotInventory(Player.Instance.currentNode);
			interactable.SetInInventory(this.restrainedInteractable.isActor as Human);
		}
		else
		{
			Game.LogError("Unable to get handcuff interactable from inventory", 2);
		}
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		this.ReturnFromTransform(false, true);
	}

	// Token: 0x0600023B RID: 571 RVA: 0x000178D0 File Offset: 0x00015AD0
	public void OnReturnFromHandcuff()
	{
		AudioController.Instance.StopSound(this.restrainedInteractable.actionLoop, AudioController.StopType.immediate, "Stop search");
		Game.Log("Player: Return from handcuff", 2);
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromHandcuff;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteHandcuff;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		this.ReturnFromTransform(false, true);
		this.restrainedInteractable.isActor.ai.SetRestrained(false, 0f);
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00017960 File Offset: 0x00015B60
	public void EnableGhostMovement(bool ghost, bool clipping = false, float stickToGround = 0f)
	{
		Game.Log("Player: Ghost movement: " + ghost.ToString() + " clipping: " + clipping.ToString(), 2);
		if (!ghost)
		{
			this.fps.enableHeadBob = true;
			this.fps.m_GravityMultiplier = 2f;
			this.fps.m_StickToGroundForce = 7f;
			this.fps.ghostMovement = false;
			this.fps.fallCount = 0f;
			this.fps.clipping = true;
			this.fps.m_CharacterController.detectCollisions = true;
			this.fps.lastY = base.transform.position.y;
		}
		else
		{
			this.fps.enableHeadBob = false;
			this.fps.m_GravityMultiplier = 0f;
			this.fps.m_StickToGroundForce = stickToGround;
			this.fps.fallCount = 0f;
			this.fps.ghostMovement = true;
			this.fps.clipping = clipping;
			this.fps.m_CharacterController.detectCollisions = clipping;
			this.fps.lastY = base.transform.position.y;
		}
		this.UpdateSkinWidth();
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00017A9C File Offset: 0x00015C9C
	public void SetActionDisable(string newString, bool val)
	{
		newString = newString.ToLower();
		Game.Log("Player: SetActionDisable " + newString + " = " + val.ToString(), 2);
		if (val)
		{
			if (!this.disabledActions.Contains(newString))
			{
				this.disabledActions.Add(newString);
			}
		}
		else
		{
			this.disabledActions.Remove(newString);
		}
		if (InteractionController.Instance.currentLookingAtInteractable != null)
		{
			InteractionController.Instance.currentLookingAtInteractable.interactable.UpdateCurrentActions();
		}
		if (InteractionController.Instance.lockedInInteraction != null)
		{
			InteractionController.Instance.lockedInInteraction.UpdateCurrentActions();
		}
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00017B48 File Offset: 0x00015D48
	public void ClearAllDisabledActions()
	{
		this.disabledActions.Clear();
		if (InteractionController.Instance.currentLookingAtInteractable != null)
		{
			InteractionController.Instance.currentLookingAtInteractable.interactable.UpdateCurrentActions();
		}
		if (InteractionController.Instance.lockedInInteraction != null)
		{
			InteractionController.Instance.lockedInInteraction.UpdateCurrentActions();
		}
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00017BAC File Offset: 0x00015DAC
	public void SetVehicle(Transform newVehicle)
	{
		if (newVehicle != null && !SessionData.Instance.startedGame)
		{
			return;
		}
		Game.Log("Player: Set vehicle " + ((newVehicle != null) ? newVehicle.ToString() : null), 2);
		if (newVehicle != this.currentVehicle)
		{
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			if (newVehicle != null)
			{
				base.gameObject.transform.SetParent(newVehicle, true);
				this.currentVehicle = newVehicle;
			}
			else
			{
				base.gameObject.transform.SetParent(this.playerContainer, true);
				this.currentVehicle = null;
			}
			base.transform.position = position;
			base.transform.rotation = rotation;
			this.fps.InitialiseController(true, true);
		}
	}

	// Token: 0x06000240 RID: 576 RVA: 0x00017C7C File Offset: 0x00015E7C
	public void SetSettingAlarmMode(bool val)
	{
		this.setAlarmMode = val;
		Game.Log("Player: Set alarm mode: " + this.setAlarmMode.ToString(), 2);
		if (this.setAlarmMode)
		{
			this.alarm = SessionData.Instance.gameTime;
			SessionData.Instance.UpdateUIClock();
			SessionData.Instance.UpdateUIDay();
			this.alarmFlash = true;
			this.setAlarmFlashCounter = 0.5f;
			ControlsDisplayController.Instance.SetControlDisplayArea(450f, 0f, 300f, 300f);
			return;
		}
		ControlsDisplayController.Instance.RestoreDefaultDisplayArea();
		if (SessionData.Instance.newWatchTimeText != null)
		{
			SessionData.Instance.newWatchTimeText.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x00017D3C File Offset: 0x00015F3C
	public void AddToAlarmTime(float plusTime)
	{
		if (this.setAlarmMode)
		{
			Game.Log("Player: Add " + plusTime.ToString() + " to alarm...", 2);
			this.alarm += plusTime;
			SessionData.Instance.UpdateUIClock();
			SessionData.Instance.UpdateUIDay();
			if (plusTime > 0f)
			{
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.timeForward, this, this.currentNode, this.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
				return;
			}
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.timeBackward, this, this.currentNode, this.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
		}
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00017E00 File Offset: 0x00016000
	public void SetSpendingTimeMode(bool val)
	{
		if (this.playerKOInProgress)
		{
			return;
		}
		this.spendingTimeMode = val;
		Game.Log("Player: Spending time mode: " + this.spendingTimeMode.ToString(), 2);
		if (this.spendingTimeMode)
		{
			if (SessionData.Instance.currentTimeSpeed != SessionData.TimeSpeed.veryFast)
			{
				SessionData.Instance.SetTimeSpeed(SessionData.TimeSpeed.veryFast);
				MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.passingTime);
				this.wristwatchLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.wristwatchTickTimeLoop, Player.Instance, this.currentNode, base.transform.position, null, null, 1f, false, false, null, null);
			}
			if (SessionData.Instance.play)
			{
				InterfaceControls.Instance.fastForwardArrow.gameObject.SetActive(true);
			}
			else
			{
				InterfaceControls.Instance.fastForwardArrow.gameObject.SetActive(false);
			}
			if (InteractionController.Instance.lockedInInteraction != null && InteractionController.Instance.lockedInInteraction.preset.specialCaseFlag == InteractablePreset.SpecialCase.sleepPosition)
			{
				InterfaceController.Instance.Fade(1f, 2f, true);
			}
		}
		else
		{
			AudioController.Instance.StopSound(this.wristwatchLoop, AudioController.StopType.fade, "Wristwatch sound complete");
			if (InterfaceController.Instance.fade != 0f || InterfaceController.Instance.desiredFade != 0f)
			{
				InterfaceController.Instance.Fade(0f, 2f, true);
			}
			if (SessionData.Instance.currentTimeSpeed != GameplayControls.Instance.startingTimeSpeed)
			{
				SessionData.Instance.SetTimeSpeed(GameplayControls.Instance.startingTimeSpeed);
			}
			InterfaceControls.Instance.fastForwardArrow.gameObject.SetActive(false);
		}
		if (InteractionController.Instance.lockedInInteraction != null)
		{
			InteractionController.Instance.lockedInInteraction.UpdateCurrentActions();
		}
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00017FD0 File Offset: 0x000161D0
	public override void RecieveDamage(float amount, Actor fromWho, Vector3 damagePosition, Vector3 damageDirection, SpatterPatternPreset forwardSpatter, SpatterPatternPreset backSpatter, SpatterSimulation.EraseMode eraseMode = SpatterSimulation.EraseMode.useDespawnTime, bool alertSurrounding = true, bool forceRagdoll = false, float forcedRagdollDuration = 0f, float shockMP = 1f, bool enableKill = false, bool allowRecoil = true, float ragdollForceMP = 1f)
	{
		if (this.playerKOInProgress)
		{
			Game.Log("Player: Negated damage; player KO in progress", 2);
			return;
		}
		this.lastDmgFrom = fromWho;
		if (SessionData.Instance.gameTime - this.lastDamageAt > 0.1f)
		{
			float upgradeEffect = UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.blockIncoming);
			this.lastDamageAt = SessionData.Instance.gameTime;
			if (upgradeEffect >= 1f)
			{
				amount = 0f;
			}
		}
		else
		{
			this.lastDamageAt = SessionData.Instance.gameTime;
		}
		if (FirstPersonItemController.Instance.slots.Exists((FirstPersonItemController.InventorySlot item) => item.GetInteractable() != null && item.GetInteractable().preset.specialCaseFlag == InteractablePreset.SpecialCase.ballisticArmour))
		{
			amount *= 0.7f;
		}
		float currentMaxHealth = base.GetCurrentMaxHealth();
		float newSpatterCountMultiplier = Mathf.Clamp01(amount / currentMaxHealth * 2f);
		amount *= Game.Instance.difficultyIncomingDamageMultipliers[Game.Instance.gameDifficulty];
		amount *= StatusController.Instance.damageIncomingMultiplier * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.incomingDamageModifier));
		this.hurt += amount;
		this.hurt = Mathf.Clamp01(this.hurt);
		base.RecieveDamage(amount, fromWho, damagePosition, damageDirection, forwardSpatter, backSpatter, eraseMode, true, false, 0f, shockMP, enableKill, true, 1f);
		if (this.autoTravelActive && amount > 0.01f)
		{
			this.EndAutoTravel();
		}
		this.takeDamageIndicatorTimer = 1f;
		float num = Mathf.Lerp(80f, 140f, amount / currentMaxHealth);
		InterfaceController.Instance.takeDamageIndicatorImg.rectTransform.sizeDelta = new Vector2(num, num);
		this.takeDamageDisplaySpeed = Mathf.Lerp(0.5f, 2f, amount / currentMaxHealth);
		InterfaceController.Instance.takeDamageIndicatorImg.gameObject.SetActive(true);
		Vector3 vector = base.transform.InverseTransformDirection(damageDirection);
		Vector3 newLocalPosition = base.transform.InverseTransformPoint(damagePosition);
		if (amount > 0.01f)
		{
			this.fps.JoltCamera(-vector, amount * 50f, Mathf.Max(1.1f - amount, 0.5f));
		}
		if (forwardSpatter != null)
		{
			new SpatterSimulation(this, newLocalPosition, vector, forwardSpatter, eraseMode, newSpatterCountMultiplier, false);
		}
		if (backSpatter != null)
		{
			new SpatterSimulation(this, newLocalPosition, -vector, backSpatter, eraseMode, newSpatterCountMultiplier, false);
		}
		Vector3 zero = Vector3.zero;
		if (Toolbox.Instance.Rand(0f, 1f, false) <= GameplayControls.Instance.combatHitChanceOfBleeding)
		{
			this.AddBleeding(Toolbox.Instance.Rand(0.2f, 0.8f, false));
		}
		else if (Toolbox.Instance.Rand(0f, 1f, false) <= GameplayControls.Instance.combatHitChanceOfBruised)
		{
			this.AddBruised(Toolbox.Instance.Rand(0.2f, 0.8f, false));
		}
		else if (Toolbox.Instance.Rand(0f, 1f, false) <= GameplayControls.Instance.combatHitChanceOfBlackEye)
		{
			this.AddBrokenLeg(Toolbox.Instance.Rand(0.2f, 0.8f, false));
		}
		else if (Toolbox.Instance.Rand(0f, 1f, false) <= GameplayControls.Instance.combatHitChanceOfBrokenLeg)
		{
			this.AddBrokenLeg(Toolbox.Instance.Rand(0.7f, 1f, false));
		}
		if (amount > 0f)
		{
			float upgradeEffect2 = UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.focusFromDamage);
			if (upgradeEffect2 > 0f)
			{
				this.AddAlertness(upgradeEffect2);
			}
		}
		Vector3 vector2 = this.aimTransform.InverseTransformPoint(damagePosition).normalized * -1f;
		string text = "Player: Incoming damage ";
		string text2 = amount.ToString();
		string text3 = " relative to me: ";
		Vector3 vector3 = vector2;
		Game.Log(text + text2 + text3 + vector3.ToString(), 2);
		zero.y = vector2.z + vector2.y;
		zero.x = vector2.x;
		if (this.currentHealthNormalized <= InterfaceControls.Instance.lowHealthIndicatorThreshold)
		{
			InterfaceController.Instance.lowHealthIndicatorImg.gameObject.SetActive(true);
		}
		SessionData.Instance.colour.saturation.overrideState = true;
		SessionData.Instance.colour.saturation.Override(Mathf.Lerp(-100f, Game.Instance.defaultSaturationAmount, this.currentHealthNormalized));
		if (this.spendingTimeMode)
		{
			this.SetSpendingTimeMode(false);
		}
	}

	// Token: 0x06000244 RID: 580 RVA: 0x00018430 File Offset: 0x00016630
	public override void SetFootwear(Human.ShoeType newType)
	{
		this.footwear = newType;
		if (this.footwear == Human.ShoeType.normal)
		{
			this.footstepEvent = AudioControls.Instance.playerFootstepShoe;
			return;
		}
		if (this.footwear == Human.ShoeType.boots)
		{
			this.footstepEvent = AudioControls.Instance.playerFootstepBoot;
			return;
		}
		if (this.footwear == Human.ShoeType.heel)
		{
			this.footstepEvent = AudioControls.Instance.playerFootstepHeel;
		}
	}

	// Token: 0x06000245 RID: 581 RVA: 0x00018490 File Offset: 0x00016690
	public override void AddHealth(float amount, bool affectedByGameDifficulty = true, bool displayDamageIndicator = false)
	{
		if (amount < 0f)
		{
			if (this.playerKOInProgress)
			{
				return;
			}
			if (InteractionController.Instance.dialogMode)
			{
				return;
			}
			if (affectedByGameDifficulty)
			{
				amount *= Game.Instance.difficultyIncomingDamageMultipliers[Game.Instance.gameDifficulty];
			}
		}
		this.currentHealth += amount;
		float currentMaxHealth = base.GetCurrentMaxHealth();
		this.currentHealth = Mathf.Clamp(this.currentHealth, 0f, currentMaxHealth);
		this.currentHealthNormalized = Mathf.Clamp01(this.currentHealth / this.maximumHealth);
		if (displayDamageIndicator && amount < 0f)
		{
			this.takeDamageIndicatorTimer = 1f;
			float num = Mathf.Lerp(80f, 140f, -amount / currentMaxHealth);
			InterfaceController.Instance.takeDamageIndicatorImg.rectTransform.sizeDelta = new Vector2(num, num);
			this.takeDamageDisplaySpeed = Mathf.Lerp(0.5f, 2f, -amount / currentMaxHealth);
			InterfaceController.Instance.takeDamageIndicatorImg.gameObject.SetActive(true);
		}
		if (this.currentHealthNormalized > InterfaceControls.Instance.lowHealthIndicatorThreshold)
		{
			if (InterfaceController.Instance.lowHealthIndicatorImg != null)
			{
				InterfaceController.Instance.lowHealthIndicatorImg.gameObject.SetActive(false);
			}
		}
		else if (this.currentHealthNormalized <= InterfaceControls.Instance.lowHealthIndicatorThreshold)
		{
			InterfaceController.Instance.lowHealthIndicatorImg.gameObject.SetActive(true);
		}
		SessionData.Instance.colour.saturation.overrideState = true;
		SessionData.Instance.colour.saturation.Override(Mathf.Lerp(-100f, Game.Instance.defaultSaturationAmount, this.currentHealthNormalized));
		if (this.currentHealth <= 0f && !Game.Instance.invinciblePlayer)
		{
			this.TriggerPlayerKO(base.transform.forward, 0f, false);
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00018668 File Offset: 0x00016868
	public override void SetHealth(float amount)
	{
		base.SetHealth(amount);
		if (this.currentHealthNormalized > InterfaceControls.Instance.lowHealthIndicatorThreshold && InterfaceController.Instance.lowHealthIndicatorImg != null)
		{
			InterfaceController.Instance.lowHealthIndicatorImg.gameObject.SetActive(false);
		}
		SessionData.Instance.colour.saturation.overrideState = true;
		SessionData.Instance.colour.saturation.Override(Mathf.Lerp(-100f, Game.Instance.defaultSaturationAmount, this.currentHealthNormalized));
		if (this.currentHealth <= 0f && !Game.Instance.invinciblePlayer)
		{
			this.TriggerPlayerKO(base.transform.forward, 0f, false);
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00018728 File Offset: 0x00016928
	public override void SightingCheck(float fov, bool ignoreLightAndStealth = false)
	{
		if (this.illegalStatus || this.witnessesToIllegalActivity.Count > 0)
		{
			List<Actor> list = new List<Actor>();
			if (this.illegalStatus)
			{
				using (List<Actor>.Enumerator enumerator = CityData.Instance.visibleActors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Actor actor = enumerator.Current;
						if (Vector3.Distance(actor.transform.position, base.transform.position) < GameplayControls.Instance.playerMaxSpotDistance)
						{
							list.Add(actor);
						}
					}
					goto IL_F1;
				}
			}
			foreach (Actor actor2 in this.witnessesToIllegalActivity)
			{
				if (actor2.visible && Vector3.Distance(actor2.transform.position, base.transform.position) < GameplayControls.Instance.playerMaxSpotDistance)
				{
					list.Add(actor2);
				}
			}
			IL_F1:
			foreach (Actor actor3 in list)
			{
				for (int i = 0; i < 2; i++)
				{
					Ray ray;
					if (i == 0)
					{
						ray..ctor(CameraController.Instance.cam.transform.position, actor3.lookAtThisTransform.position - CameraController.Instance.cam.transform.position);
					}
					else
					{
						ray..ctor(CameraController.Instance.cam.transform.position, actor3.transform.position - CameraController.Instance.cam.transform.position);
					}
					RaycastHit raycastHit;
					if (Physics.Raycast(ray, ref raycastHit, GameplayControls.Instance.playerMaxSpotDistance, Toolbox.Instance.aiSightingLayerMask) && raycastHit.transform.parent != null && raycastHit.transform.parent.gameObject == actor3.gameObject)
					{
						actor3.SpottedByPlayer(1f);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000248 RID: 584 RVA: 0x000189A0 File Offset: 0x00016BA0
	public override void PrepForStart()
	{
		this.nourishment = 1f;
		this.hydration = 1f;
		this.alertness = 1f;
		this.energy = 1f;
		this.excitement = 1f;
		this.chores = 1f;
		this.hygiene = 1f;
		this.bladder = 1f;
		this.heat = 1f;
		this.drunk = 0f;
		this.sick = 0f;
		this.starchAddiction = 0f;
		this.headache = 0f;
		this.wet = 0f;
		this.brokenLeg = 0f;
		this.bruised = 0f;
		this.blackEye = 0f;
		this.blackedOut = 0f;
		this.numb = 0f;
		this.breath = 1f;
		this.poisoned = 0f;
		this.blinded = 0f;
		this.descriptors.skinColour = Game.Instance.playerSkinColour;
		this.ResetHealthToMaximum();
		this.ResetNerveToMaximum();
		if (this.home != null)
		{
			base.AddLocationOfAuthorty(this.home);
		}
		foreach (NewAddress newLoc in this.apartmentsOwned)
		{
			base.AddLocationOfAuthorty(newLoc);
		}
		this.citizenName = Game.Instance.playerFirstName + " " + Game.Instance.playerSurname;
		this.firstName = Game.Instance.playerFirstName;
		this.surName = Game.Instance.playerSurname;
		this.casualName = string.Empty;
		base.gameObject.name = this.citizenName;
		base.name = this.citizenName;
		this.gender = Game.Instance.playerGender;
		this.birthGender = Game.Instance.playerGender;
		this.passcode.digits.Clear();
		this.passcode.digits.Add(1);
		this.passcode.digits.Add(2);
		this.passcode.digits.Add(3);
		this.passcode.digits.Add(4);
		this.evidenceEntry = (EvidenceCreator.Instance.CreateEvidence("citizen", "Player", this, this, this, null, null, false, null) as EvidenceWitness);
		this.interactable = InteractableCreator.Instance.CreateCitizenInteractable(CitizenControls.Instance.citizenInteractable, this, base.transform, this.evidenceEntry);
		this.sceneRecorder = new SceneRecorder(this.interactable);
	}

	// Token: 0x06000249 RID: 585 RVA: 0x00018C60 File Offset: 0x00016E60
	public override void AddNourishment(float addVal)
	{
		base.AddNourishment(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x0600024A RID: 586 RVA: 0x00018C6F File Offset: 0x00016E6F
	public override void AddHydration(float addVal)
	{
		base.AddHydration(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00018C7E File Offset: 0x00016E7E
	public override void AddEnergy(float addVal)
	{
		base.AddEnergy(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x0600024C RID: 588 RVA: 0x00018C8D File Offset: 0x00016E8D
	public override void AddAlertness(float addVal)
	{
		base.AddAlertness(addVal);
		if (this.alertness > 0.5f)
		{
			this.energy = Mathf.Max(this.energy, this.alertness - 0.5f);
		}
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00018CC6 File Offset: 0x00016EC6
	public override void AddHygiene(float addVal)
	{
		base.AddHygiene(addVal);
		if (this.hygiene <= 0f)
		{
			this.StatusCheckEndOfFrame();
			return;
		}
		if (this.hygiene >= 1f)
		{
			this.StatusCheckEndOfFrame();
		}
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00018CF6 File Offset: 0x00016EF6
	public override void AddHeat(float addVal)
	{
		if (addVal < 0f)
		{
			addVal *= StatusController.Instance.temperatureGainMultiplier;
		}
		base.AddHeat(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x0600024F RID: 591 RVA: 0x00018D1B File Offset: 0x00016F1B
	public override void AddDrunk(float addVal)
	{
		base.AddDrunk(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00018D2A File Offset: 0x00016F2A
	public override void AddSick(float addVal)
	{
		base.AddSick(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00018D39 File Offset: 0x00016F39
	public override void AddHeadache(float addVal)
	{
		base.AddHeadache(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00018D48 File Offset: 0x00016F48
	public override void AddWet(float addVal)
	{
		base.AddWet(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00018D57 File Offset: 0x00016F57
	public override void AddBrokenLeg(float addVal)
	{
		if (addVal > 0f && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.noBrokenBones) > 0f)
		{
			addVal = 0f;
		}
		base.AddBrokenLeg(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00018D88 File Offset: 0x00016F88
	public override void AddBruised(float addVal)
	{
		if (addVal > 0f && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.noBrokenBones) > 0f)
		{
			addVal = 0f;
		}
		base.AddBruised(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00018DB9 File Offset: 0x00016FB9
	public override void AddBlackEye(float addVal)
	{
		if (addVal > 0f && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.noBrokenBones) > 0f)
		{
			addVal = 0f;
		}
		base.AddBlackEye(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00018DEA File Offset: 0x00016FEA
	public override void AddBlackedOut(float addVal)
	{
		base.AddBlackedOut(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00018DF9 File Offset: 0x00016FF9
	public override void AddNumb(float addVal)
	{
		base.AddNumb(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000258 RID: 600 RVA: 0x00018E08 File Offset: 0x00017008
	public override void AddBleeding(float addVal)
	{
		if (addVal > 0f && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.noBleeding) > 0f)
		{
			addVal = 0f;
		}
		base.AddBleeding(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00018E39 File Offset: 0x00017039
	public override void AddStarchAddiction(float addVal)
	{
		base.AddStarchAddiction(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00018E48 File Offset: 0x00017048
	public override void AddSyncDiskInstall(float addVal)
	{
		base.AddSyncDiskInstall(addVal);
		this.StatusCheckEndOfFrame();
	}

	// Token: 0x0600025B RID: 603 RVA: 0x00018E57 File Offset: 0x00017057
	public void StatusCheckEndOfFrame()
	{
		Toolbox.Instance.InvokeEndOfFrame(this.updateStatusAction, "Update status");
	}

	// Token: 0x0600025C RID: 604 RVA: 0x00018E6E File Offset: 0x0001706E
	public override void SetOnStreet(bool val)
	{
		if (this.inAirVent)
		{
			val = false;
		}
		base.SetOnStreet(val);
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00018E84 File Offset: 0x00017084
	public void Trip(float damage, bool forwards = false, bool playSound = true)
	{
		if (!this.transitionActive && !this.isAsleep && !this.inAirVent && this.spawnProtection <= 0f && !Player.Instance.isCrouched)
		{
			base.SetInteracting(null);
			this.AddHealth(-damage, false, true);
			Vector3 direction = Vector3.left;
			if (forwards)
			{
				direction = Vector3.right;
			}
			direction.y = Toolbox.Instance.Rand(-0.1f, 0.1f, false);
			direction.z = Toolbox.Instance.Rand(-0.1f, 0.1f, false);
			this.fps.JoltCamera(direction, 60f, 0.25f);
			if (this.currentHealth > 0f)
			{
				this.TransformPlayerController(GameplayControls.Instance.tripTransition, null, null, null, false, false, 0f, false, default(Vector3), 1f, true);
				if (playSound)
				{
					AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.playerTripSound, Player.Instance, Player.Instance.currentNode, Player.Instance.transform.position, null, null, 1f, null, false, null, false);
				}
			}
		}
	}

	// Token: 0x0600025E RID: 606 RVA: 0x00018FB8 File Offset: 0x000171B8
	public override void SetHiding(bool val, Interactable newHidingPlace)
	{
		if (this.hygiene < 1f)
		{
			val = false;
		}
		if (this.isHiding != val && val)
		{
			this.spottedWhileHiding.Clear();
			if (val)
			{
				this.hidingInteractable = newHidingPlace;
				using (HashSet<Actor>.Enumerator enumerator = this.witnessesToIllegalActivity.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Actor actor = enumerator.Current;
						if (actor.seesIllegal.ContainsKey(this))
						{
							if (actor.seesIllegal[this] >= 1f)
							{
								Game.Log("Player: Spotted while hiding: " + actor.name, 2);
								this.spottedWhileHiding.Add(actor);
							}
							else if (actor.ai != null && actor.ai.investigateLocation == this.currentNode)
							{
								actor.ai.investigateLocation = Toolbox.Instance.PickNearbyNode(this.currentNode);
								actor.ai.investigatePosition = actor.ai.investigateLocation.position;
							}
						}
					}
					goto IL_116;
				}
			}
			this.hidingInteractable = null;
		}
		IL_116:
		base.SetHiding(val, newHidingPlace);
		Game.Log("Player: Set hiding: " + val.ToString(), 2);
		StatusController.Instance.ForceStatusCheck();
	}

	// Token: 0x0600025F RID: 607 RVA: 0x00019114 File Offset: 0x00017314
	public float GetPlayerHeightNormal()
	{
		return GameplayControls.Instance.playerHeightNormal * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.playerHeightModifier));
	}

	// Token: 0x06000260 RID: 608 RVA: 0x00019133 File Offset: 0x00017333
	public float GetPlayerHeightCrouched()
	{
		return GameplayControls.Instance.playerHeightCrouched * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.playerHeightModifier));
	}

	// Token: 0x06000261 RID: 609 RVA: 0x00019154 File Offset: 0x00017354
	public void ExecuteAutoTravel(Evidence toLocation, bool fastTravel = false)
	{
		EvidenceLocation evidenceLocation = toLocation as EvidenceLocation;
		if (evidenceLocation != null && evidenceLocation.locationController != null)
		{
			this.ExecuteAutoTravel(evidenceLocation.locationController, fastTravel);
			return;
		}
		EvidenceBuilding evidenceBuilding = toLocation as EvidenceBuilding;
		if (evidenceBuilding != null && evidenceBuilding.building != null)
		{
			this.ExecuteAutoTravel(evidenceBuilding.building, fastTravel);
		}
	}

	// Token: 0x06000262 RID: 610 RVA: 0x000191AC File Offset: 0x000173AC
	public void ExecuteAutoTravel(NewGameLocation toLocation, bool fastTravel = false)
	{
		NewNode newNode = null;
		if (toLocation != null)
		{
			if (toLocation.thisAsAddress != null)
			{
				newNode = toLocation.thisAsAddress.GetDestinationNode();
			}
			else if (toLocation.thisAsStreet != null)
			{
				newNode = toLocation.thisAsStreet.GetDestinationNode();
			}
		}
		if (newNode != null)
		{
			this.ExecuteAutoTravel(newNode, fastTravel);
		}
	}

	// Token: 0x06000263 RID: 611 RVA: 0x00019208 File Offset: 0x00017408
	public void ExecuteAutoTravel(NewBuilding toBuilding, bool fastTravel = false)
	{
		bool flag = false;
		foreach (NewAddress newAddress in toBuilding.lobbies)
		{
			if (newAddress.entrances.Count > 0)
			{
				Game.Log("Interface: Plotting route to building...", 2);
				this.ExecuteAutoTravel(newAddress, false);
				flag = true;
				break;
			}
		}
		if (!flag && toBuilding.mainEntrance != null)
		{
			this.ExecuteAutoTravel(toBuilding.mainEntrance.node, fastTravel);
		}
	}

	// Token: 0x06000264 RID: 612 RVA: 0x00019298 File Offset: 0x00017498
	public void ExecuteAutoTravel(NewNode toNode, bool fastTravel = false)
	{
		if (this.inAirVent)
		{
			return;
		}
		if (this.transitionActive)
		{
			return;
		}
		if (toNode == null)
		{
			return;
		}
		if (MapController.Instance.playerRoute != null)
		{
			MapController.Instance.playerRoute.Remove();
		}
		this.currentAutoTravelDest = null;
		MapController.Instance.PlotPlayerRoute(toNode, true, null);
		if (MapController.Instance.playerRoute != null)
		{
			string text = "Player: Starting fast travel to ";
			Vector3 position = toNode.position;
			Game.Log(text + position.ToString() + " path length: " + MapController.Instance.playerRoute.pathData.accessList.Count.ToString(), 2);
			if (fastTravel)
			{
				this.SetSpendingTimeMode(true);
			}
			else if (this.spendingTimeMode)
			{
				this.SetSpendingTimeMode(false);
			}
			this.autoTravelActive = true;
			InteractionController.Instance.UpdateInteractionText();
			if (MapController.Instance.autoTravelActiveJuice != null)
			{
				MapController.Instance.autoTravelActiveJuice.gameObject.SetActive(true);
				MapController.Instance.autoTravelActiveJuice.Pulsate(true, false);
			}
			if (this.OnExecuteAutoTravel != null)
			{
				this.OnExecuteAutoTravel();
			}
		}
	}

	// Token: 0x06000265 RID: 613 RVA: 0x000193BC File Offset: 0x000175BC
	public void EndAutoTravel()
	{
		Game.Log("Player: Ending auto travel...", 2);
		this.SetSpendingTimeMode(false);
		if (MapController.Instance.autoTravelActiveJuice != null)
		{
			MapController.Instance.autoTravelActiveJuice.Pulsate(false, false);
			MapController.Instance.autoTravelActiveJuice.gameObject.SetActive(false);
		}
		this.currentAutoTravelDest = null;
		if (MapController.Instance.playerRoute == null && MapController.Instance.autoTravelButton != null)
		{
			MapController.Instance.autoTravelButton.SetInteractable(false);
		}
		if (this.autoTravelDoor != null)
		{
			foreach (Collider collider in this.autoTravelDoor.spawnedDoorColliders)
			{
				if (collider != null)
				{
					Physics.IgnoreCollision(collider, this.fps.m_CharacterController, false);
				}
			}
		}
		this.autoTravelActive = false;
		this.OnTransitionComplete();
		InteractionController.Instance.UpdateInteractionText();
		if (this.OnEndAutoTravel != null)
		{
			this.OnEndAutoTravel();
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x000194E0 File Offset: 0x000176E0
	[Button(null, 0)]
	public void KillPlayer()
	{
		this.AddHealth(-100000000f, false, true);
	}

	// Token: 0x06000267 RID: 615 RVA: 0x000194EF File Offset: 0x000176EF
	[Button(null, 0)]
	public void GetCurrentNodeCoord()
	{
		if (this.currentNode != null)
		{
			Game.Log(this.currentNode.nodeCoord, 2);
		}
	}

	// Token: 0x0400013C RID: 316
	[Header("Player Attributes")]
	public bool fpsMode;

	// Token: 0x0400013D RID: 317
	public FirstPersonController fps;

	// Token: 0x0400013E RID: 318
	public CharacterController charController;

	// Token: 0x0400013F RID: 319
	public CapsuleCollider transitionDamageTrigger;

	// Token: 0x04000140 RID: 320
	public CameraController cam;

	// Token: 0x04000141 RID: 321
	public Transform camHeightParent;

	// Token: 0x04000142 RID: 322
	public Transform playerContainer;

	// Token: 0x04000143 RID: 323
	public Transform currentVehicle;

	// Token: 0x04000144 RID: 324
	public AirDuctGroup.AirDuctSection previousDuctSection;

	// Token: 0x04000145 RID: 325
	public AirDuctGroup.AirDuctSection currentDuctSection;

	// Token: 0x04000146 RID: 326
	[Header("Player specific flags")]
	public bool isCrunchingDatabase;

	// Token: 0x04000147 RID: 327
	public SceneRecorder sceneRecorder;

	// Token: 0x04000148 RID: 328
	[Header("Watch Alarm")]
	public bool setAlarmMode;

	// Token: 0x04000149 RID: 329
	public bool editingHours = true;

	// Token: 0x0400014A RID: 330
	private float setAlarmFlashCounter = 0.5f;

	// Token: 0x0400014B RID: 331
	public float alarm;

	// Token: 0x0400014C RID: 332
	private bool alarmFlash = true;

	// Token: 0x0400014D RID: 333
	public float setAlarmModeAfterDelay;

	// Token: 0x0400014E RID: 334
	public float spendingTimeDelay;

	// Token: 0x0400014F RID: 335
	public bool spendingTimeMode;

	// Token: 0x04000150 RID: 336
	[Header("Auto Travel")]
	public bool autoTravelActive;

	// Token: 0x04000151 RID: 337
	private NewDoor autoTravelDoor;

	// Token: 0x04000152 RID: 338
	private NewNode.NodeAccess currentAutoTravelDest;

	// Token: 0x04000153 RID: 339
	private NewNode.NodeSpace currentNodeSpaceDest;

	// Token: 0x04000154 RID: 340
	private float currentNodeSpaceDestTimer;

	// Token: 0x04000155 RID: 341
	public float autoTravelDistanceToNext;

	// Token: 0x04000156 RID: 342
	public Vector3 autoTravelForward = Vector3.zero;

	// Token: 0x04000157 RID: 343
	[Header("Telephone")]
	[NonSerialized]
	public Telephone answeringPhone;

	// Token: 0x04000158 RID: 344
	[NonSerialized]
	public TelephoneController.PhoneCall activeCall;

	// Token: 0x04000159 RID: 345
	[Header("Audio")]
	public List<CanvasRenderer> footstepSoundObjects = new List<CanvasRenderer>();

	// Token: 0x0400015A RID: 346
	[Header("Player State")]
	public float crouchedTransition;

	// Token: 0x0400015B RID: 347
	public bool crouchTransitionActive;

	// Token: 0x0400015C RID: 348
	private int updateNodeSpace;

	// Token: 0x0400015D RID: 349
	private float takeDamageIndicatorTimer;

	// Token: 0x0400015E RID: 350
	private float takeDamageDisplaySpeed = 2f;

	// Token: 0x0400015F RID: 351
	private float spawnProtection = 2f;

	// Token: 0x04000160 RID: 352
	private bool wasMoving;

	// Token: 0x04000161 RID: 353
	private int nearbyInteractableUpdate;

	// Token: 0x04000162 RID: 354
	public float gasLevel;

	// Token: 0x04000163 RID: 355
	public float hurt;

	// Token: 0x04000164 RID: 356
	private Interactable bed;

	// Token: 0x04000165 RID: 357
	public List<CityTile> cityTilesInVicinity = new List<CityTile>();

	// Token: 0x04000166 RID: 358
	public List<Interactable> playerKeyringInt = new List<Interactable>();

	// Token: 0x04000167 RID: 359
	public bool forceLookAtActive;

	// Token: 0x04000168 RID: 360
	public Interactable forceLookAtInteractable;

	// Token: 0x04000169 RID: 361
	public float forceLookAtTime;

	// Token: 0x0400016A RID: 362
	private float lookAtTime;

	// Token: 0x0400016B RID: 363
	private float lookAtProgress;

	// Token: 0x0400016C RID: 364
	private Quaternion originalLookAtModRotationGlobal;

	// Token: 0x0400016D RID: 365
	public bool transitionActive;

	// Token: 0x0400016E RID: 366
	private float transitionTime;

	// Token: 0x0400016F RID: 367
	public float transitionProgress;

	// Token: 0x04000170 RID: 368
	[NonSerialized]
	public Interactable transitionInteractable;

	// Token: 0x04000171 RID: 369
	public PlayerTransitionPreset currentTransition;

	// Token: 0x04000172 RID: 370
	public PlayerTransitionPreset exitTransition;

	// Token: 0x04000173 RID: 371
	public Vector3 originalPlayerPosition;

	// Token: 0x04000174 RID: 372
	public Vector3 originalModPosition;

	// Token: 0x04000175 RID: 373
	public float originalPlayerHeight;

	// Token: 0x04000176 RID: 374
	public float originalCamHeight;

	// Token: 0x04000177 RID: 375
	public Vector3 startingLookPointWorldPosition;

	// Token: 0x04000178 RID: 376
	public bool transitionRecoilState;

	// Token: 0x04000179 RID: 377
	private List<PlayerTransitionPreset.SFXSetting> soundsPlayed = new List<PlayerTransitionPreset.SFXSetting>();

	// Token: 0x0400017A RID: 378
	public Quaternion originalModRotationGlobal;

	// Token: 0x0400017B RID: 379
	public Quaternion originalModRotationLocal;

	// Token: 0x0400017C RID: 380
	public Vector3 additionalLookMultiplier = Vector3.one;

	// Token: 0x0400017D RID: 381
	public float rollMultiplier = 1f;

	// Token: 0x0400017E RID: 382
	public bool transitionForceTime;

	// Token: 0x0400017F RID: 383
	public float transtionForcedTime;

	// Token: 0x04000180 RID: 384
	public Transform transitionLookAt;

	// Token: 0x04000181 RID: 385
	private bool movementOnTransitionComplete = true;

	// Token: 0x04000182 RID: 386
	private bool restoreHolsterOnTransitionComplete = true;

	// Token: 0x04000183 RID: 387
	public bool citizensArrestActive;

	// Token: 0x04000184 RID: 388
	public List<string> disabledActions = new List<string>();

	// Token: 0x04000185 RID: 389
	public int forcedLeanState;

	// Token: 0x04000186 RID: 390
	public float extraLeanSpeed;

	// Token: 0x04000187 RID: 391
	public float normalStepOffset = 0.25f;

	// Token: 0x04000188 RID: 392
	public float airVentStepOffset = 0.25f;

	// Token: 0x04000189 RID: 393
	public Vector3 storedTransitionPosition;

	// Token: 0x0400018A RID: 394
	public float desiredWalkSpeed = 1f;

	// Token: 0x0400018B RID: 395
	public float desiredRunSpeed = 1f;

	// Token: 0x0400018C RID: 396
	private bool playerKOFadeOut;

	// Token: 0x0400018D RID: 397
	private bool paidFines;

	// Token: 0x0400018E RID: 398
	private float KOTime;

	// Token: 0x0400018F RID: 399
	private float KOTimePassed;

	// Token: 0x04000190 RID: 400
	private bool KORecovery;

	// Token: 0x04000191 RID: 401
	private bool dirtyDeath;

	// Token: 0x04000192 RID: 402
	private GameplayController.LoanDebt debtPayment;

	// Token: 0x04000193 RID: 403
	public bool pausedRememberPlayerMovement = true;

	// Token: 0x04000194 RID: 404
	[NonSerialized]
	public Interactable hideInteractable;

	// Token: 0x04000195 RID: 405
	[NonSerialized]
	public int hideReference;

	// Token: 0x04000196 RID: 406
	[NonSerialized]
	public Interactable phoneInteractable;

	// Token: 0x04000197 RID: 407
	[NonSerialized]
	public Interactable computerInteractable;

	// Token: 0x04000198 RID: 408
	[NonSerialized]
	public Interactable restrainedInteractable;

	// Token: 0x04000199 RID: 409
	[NonSerialized]
	public FirstPersonItemController.InventorySlot restrainedHandcuffsSlot;

	// Token: 0x0400019A RID: 410
	[NonSerialized]
	public Interactable searchInteractable;

	// Token: 0x0400019B RID: 411
	[NonSerialized]
	public Interactable genericActionInteractable;

	// Token: 0x0400019C RID: 412
	[NonSerialized]
	public int nodesTraversedWhileWalking;

	// Token: 0x0400019D RID: 413
	[Header("Damage Block")]
	public float lastDamageAt;

	// Token: 0x0400019E RID: 414
	public Actor lastDmgFrom;

	// Token: 0x0400019F RID: 415
	[Header("Illegal State")]
	[NonSerialized]
	public float illegalActionTimer = 1f;

	// Token: 0x040001A0 RID: 416
	public float seenProgress;

	// Token: 0x040001A1 RID: 417
	public float seenProgressLag;

	// Token: 0x040001A2 RID: 418
	public float persuedProgress;

	// Token: 0x040001A3 RID: 419
	public float persuedProgressLag;

	// Token: 0x040001A4 RID: 420
	[NonSerialized]
	public AudioController.LoopingSoundInfo trespassingSnapshot;

	// Token: 0x040001A5 RID: 421
	[NonSerialized]
	public AudioController.LoopingSoundInfo combatSnapshot;

	// Token: 0x040001A6 RID: 422
	[NonSerialized]
	public AudioController.LoopingSoundInfo syncMachineSnapshot;

	// Token: 0x040001A7 RID: 423
	[NonSerialized]
	public AudioController.LoopingSoundInfo onlyMusicSnapshot;

	// Token: 0x040001A8 RID: 424
	[NonSerialized]
	public AudioController.LoopingSoundInfo wristwatchLoop;

	// Token: 0x040001A9 RID: 425
	[NonSerialized]
	public float visibilityLag;

	// Token: 0x040001AA RID: 426
	private float stealthLag;

	// Token: 0x040001AB RID: 427
	public float seenIconLag;

	// Token: 0x040001AC RID: 428
	private int spotCheckTimer;

	// Token: 0x040001AD RID: 429
	public bool playerKOInProgress;

	// Token: 0x040001AE RID: 430
	public bool isLockpicking;

	// Token: 0x040001AF RID: 431
	public bool isGrounded = true;

	// Token: 0x040001B0 RID: 432
	private bool wasGrounded;

	// Token: 0x040001B1 RID: 433
	public bool claimedAccidentCover;

	// Token: 0x040001B2 RID: 434
	public List<int> foodHygeinePhotos = new List<int>();

	// Token: 0x040001B3 RID: 435
	public List<int> sanitaryHygeinePhotos = new List<int>();

	// Token: 0x040001B4 RID: 436
	public List<int> illegalOpsPhotos = new List<int>();

	// Token: 0x040001B5 RID: 437
	public bool firstFrame = true;

	// Token: 0x040001B6 RID: 438
	private bool lateFixedUpdate;

	// Token: 0x040001B7 RID: 439
	private bool drinkLoopStarted;

	// Token: 0x040001B8 RID: 440
	private AudioController.LoopingSoundInfo drinkLoop;

	// Token: 0x040001B9 RID: 441
	[Header("Apartments")]
	public List<NewAddress> apartmentsOwned = new List<NewAddress>();

	// Token: 0x040001BA RID: 442
	private Action updateCullingAction;

	// Token: 0x040001BB RID: 443
	private Action updateStatusAction;

	// Token: 0x040001BC RID: 444
	public List<Actor> spottedByPlayer = new List<Actor>();

	// Token: 0x040001BD RID: 445
	public List<Actor> spottedWhileHiding = new List<Actor>();

	// Token: 0x040001BE RID: 446
	[NonSerialized]
	public Interactable hidingInteractable;

	// Token: 0x040001C3 RID: 451
	private static Player _instance;

	// Token: 0x040001C4 RID: 452
	private List<CityTile> requiredVicinity = new List<CityTile>();

	// Token: 0x02000037 RID: 55
	// (Invoke) Token: 0x0600026D RID: 621
	public delegate void TransitionCompleted(bool restoreTransform);

	// Token: 0x02000038 RID: 56
	// (Invoke) Token: 0x06000271 RID: 625
	public delegate void StartAutoTravel();

	// Token: 0x02000039 RID: 57
	// (Invoke) Token: 0x06000275 RID: 629
	public delegate void AutoTravelEnd();

	// Token: 0x0200003A RID: 58
	// (Invoke) Token: 0x06000279 RID: 633
	public delegate void GameLocationChange();
}
