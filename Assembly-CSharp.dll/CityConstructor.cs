using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000B7 RID: 183
public class CityConstructor : MonoBehaviour
{
	// Token: 0x14000008 RID: 8
	// (add) Token: 0x0600057D RID: 1405 RVA: 0x00057C24 File Offset: 0x00055E24
	// (remove) Token: 0x0600057E RID: 1406 RVA: 0x00057C5C File Offset: 0x00055E5C
	public event CityConstructor.OnStartGame OnGameStarted;

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x0600057F RID: 1407 RVA: 0x00057C94 File Offset: 0x00055E94
	// (remove) Token: 0x06000580 RID: 1408 RVA: 0x00057CCC File Offset: 0x00055ECC
	public event CityConstructor.LoadFinalize OnLoadFinalize;

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000581 RID: 1409 RVA: 0x00057D01 File Offset: 0x00055F01
	public static CityConstructor Instance
	{
		get
		{
			return CityConstructor._instance;
		}
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x00057D08 File Offset: 0x00055F08
	private void Awake()
	{
		if (CityConstructor._instance != null && CityConstructor._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			CityConstructor._instance = this;
		}
		base.enabled = false;
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x00057D3E File Offset: 0x00055F3E
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
		CityConstructor._instance = null;
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00057D54 File Offset: 0x00055F54
	public void GenerateNewCity()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("Menu: Generating new city...", 2);
		}
		this.currentData = new CitySaveData();
		this.currentData.build = Game.Instance.buildID;
		this.currentData.cityName = RestartSafeController.Instance.cityName;
		this.currentData.seed = RestartSafeController.Instance.seed;
		this.currentData.citySize = new Vector2((float)RestartSafeController.Instance.cityX, (float)RestartSafeController.Instance.cityY);
		this.generateNew = true;
		this.StartLoading();
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x00057DF8 File Offset: 0x00055FF8
	public void LoadSaveGame()
	{
		CityConstructor.<LoadSaveGame>d__40 <LoadSaveGame>d__;
		<LoadSaveGame>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadSaveGame>d__.<>4__this = this;
		<LoadSaveGame>d__.<>1__state = -1;
		<LoadSaveGame>d__.<>t__builder.Start<CityConstructor.<LoadSaveGame>d__40>(ref <LoadSaveGame>d__);
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00057E2F File Offset: 0x0005602F
	public void IncompatibleVersionConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.IncompatibleVersionCancel;
		PopupMessageController.Instance.OnRightButton -= this.IncompatibleVersionConfirm;
		this.GenerateCityFromShareCode();
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00057E64 File Offset: 0x00056064
	public void IncompatibleVersionCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.IncompatibleVersionCancel;
		PopupMessageController.Instance.OnRightButton -= this.IncompatibleVersionConfirm;
		AudioController.Instance.StopAllSounds();
		SceneManager.LoadScene("Main");
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00057EB4 File Offset: 0x000560B4
	private void GenerateCityFromShareCode()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("Menu: ...Continuing to generate a city using this version using the saved share code...", 2);
		}
		this.currentData = new CitySaveData();
		string cityName = "New City";
		int num = 5;
		int num2 = 5;
		string seed = "anfw2ajfal2w54f3o";
		string empty = string.Empty;
		Toolbox.Instance.ParseShareCode(this.saveState.cityShare, out cityName, out num, out num2, out empty, out seed);
		this.currentData.cityName = cityName;
		this.currentData.citySize = new Vector2((float)num, (float)num2);
		this.currentData.seed = seed;
		this.currentData.build = empty;
		this.generateNew = true;
		this.StartLoading();
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x00057F5F File Offset: 0x0005615F
	public void LoadCityStartNewGame()
	{
		this.generateNew = false;
		this.StartLoading();
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x00057F70 File Offset: 0x00056170
	public void StartLoading()
	{
		NewRoom.assignRoomFloorID = 1;
		NewRoom.assignRoomID = 1;
		Interactable.worldAssignID = 10000000;
		NewWall.assignID = 1;
		NewBuilding.assignID = 1;
		NewAddress.assignID = 1;
		StreetController.assignID = 1;
		DistrictController.assignID = 1;
		GameplayController.Instance.printsLetterLoop = 0;
		SceneRecorder.assignCapID = 1;
		GameplayController.Instance.assignMessageThreadID = 1;
		GroupsController.assignID = 1;
		InterfaceController.assignStickyNoteID = 1;
		Case.assignCaseID = 1;
		Human.assignID = 2;
		Human.assignTraitID = 1;
		MeshPoolingController.Instance.roomMeshes = new Dictionary<NewRoom, MeshPoolingController.RoomMeshCache>();
		Game.Instance.sandboxMode = RestartSafeController.Instance.sandbox;
		MainMenuController.Instance.LoadTip();
		Toolbox.Instance.lastRandomNumberKey = string.Empty;
		this.loadingWallsReference.Clear();
		this.loadingFurnitureReference.Clear();
		this.preSimActive = false;
		this.preSimOccured = false;
		if (Game.Instance.debugContainer != null)
		{
			Object.Destroy(Game.Instance.debugContainer.gameObject);
		}
		MainMenuController.Instance.mouseOverText.text = string.Empty;
		this.allLoadStates = Enumerable.ToList<CityConstructor.LoadState>(Enumerable.Cast<CityConstructor.LoadState>(Enum.GetValues(typeof(CityConstructor.LoadState))));
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.debugLoadTime = new CityConstructor.CollectedLoadTimeInfo();
			this.debugLoadTime.build = Game.Instance.buildID;
			this.debugLoadTime.generateNew = this.generateNew;
			this.debugLoadTime.citySize = CityData.Instance.citySize.ToString();
		}
		MainMenuController.Instance.loadingSlider.value = 0f;
		MainMenuController.Instance.loadingText.text = Strings.Get("ui.loading", "Generating", Strings.Casing.asIs, false, false, false, null) + "...";
		this.loadingOperationActive = false;
		this.loadCursor = 0;
		this.loadState = (CityConstructor.LoadState)this.loadCursor;
		MainMenuController.Instance.loadingText.text = Strings.Get("ui.loading", this.loadState.ToString(), Strings.Casing.asIs, false, false, false, null);
		Game.Instance.playerFirstName = RestartSafeController.Instance.newGamePlayerFirstName;
		Game.Instance.playerSurname = RestartSafeController.Instance.newGamePlayerSurname;
		Game.Instance.playerGender = RestartSafeController.Instance.newGamePlayerGender;
		Game.Instance.partnerGender = RestartSafeController.Instance.newGamePartnerGender;
		Game.Instance.playerSkinColour = RestartSafeController.Instance.newGamePlayerSkinTone;
		Player.Instance.descriptors.skinColour = Game.Instance.playerSkinColour;
		if (Game.Instance.sandboxMode)
		{
			SessionData.Instance.SetGameTime(GameplayControls.Instance.startingYear, GameplayControls.Instance.startingMonth, GameplayControls.Instance.startingDate, GameplayControls.Instance.dayZero, Game.Instance.sandboxStartTime, GameplayControls.Instance.yearZeroLeapYearCycle);
		}
		else
		{
			ChapterPreset chapterPreset = ChapterController.Instance.allChapters[Game.Instance.loadChapter];
			SessionData.Instance.SetGameTime(chapterPreset.startingYear, chapterPreset.startingMonth, chapterPreset.startingDate, chapterPreset.dayZero, chapterPreset.startingHour, chapterPreset.yearZeroLeapYearCycle);
		}
		this.loadingProgress = 0f;
		this.displayedProgress = -1;
		this.stateComplete = false;
		base.enabled = true;
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x000582D0 File Offset: 0x000564D0
	private void Update()
	{
		if (PopupMessageController.Instance != null && PopupMessageController.Instance.active)
		{
			return;
		}
		if (MainMenuController.Instance.nextTipTimer <= 0f)
		{
			MainMenuController.Instance.LoadTip();
		}
		else
		{
			MainMenuController.Instance.nextTipTimer -= Time.deltaTime;
		}
		if (this.loadCursor < this.allLoadStates.Count)
		{
			MainMenuController.Instance.loadingSlider.value = this.loadingProgress;
			if (Mathf.Round(this.loadingProgress * 100f) != (float)this.displayedProgress)
			{
				this.SetLoadingText();
			}
			if (!this.loadingOperationActive || (this.loadFullCityDataTask != null && this.loadFullCityDataTask.IsCompleted))
			{
				if (Game.Instance.devMode && Game.Instance.collectDebugData && this.debugLoadTime != null)
				{
					if (Game.Instance.printDebug)
					{
						string text = "CityGen: Starting timer for load phase ";
						string text2 = this.loadState.ToString();
						string text3 = " ";
						int num = (int)this.loadState;
						Game.Log(text + text2 + text3 + num.ToString(), 2);
					}
					this.timeStamp = Time.realtimeSinceStartup;
				}
				if (this.loadState == CityConstructor.LoadState.parsingFile)
				{
					if (!this.generateNew && this.loadFullCityDataTask == null)
					{
						this.loadFullCityDataTask = this.LoadFullCityDataAsync();
					}
					if ((this.loadFullCityDataTask == null && this.generateNew) || (this.loadFullCityDataTask != null && this.loadFullCityDataTask.IsCompleted))
					{
						this.loadFullCityDataTask = null;
						CityData.Instance.cityName = this.currentData.cityName;
						CityData.Instance.citySize = this.currentData.citySize;
						CityData.Instance.cityBuiltWith = this.currentData.build;
						CityData.Instance.seed = this.currentData.seed;
						if (Game.Instance.printDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"CityGen: Loading city ",
								this.currentData.cityName,
								" ",
								this.currentData.build,
								"  with seed ",
								CityData.Instance.seed
							}), 2);
						}
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				else if (this.loadState == CityConstructor.LoadState.setupCityBoundary)
				{
					HighlanderSingleton<CityBoundaryAndTiles>.Instance.SetupCityBoundary();
					this.loadingProgress = 1f;
					this.stateComplete = true;
				}
				else if (this.loadState == CityConstructor.LoadState.generateDistricts)
				{
					HighlanderSingleton<CityDistricts>.Instance.GenerateDistricts();
					this.loadingProgress = 1f;
					this.stateComplete = true;
				}
				else if (this.loadState == CityConstructor.LoadState.generateBlocks)
				{
					if (this.generateNew)
					{
						HighlanderSingleton<CityBlocks>.Instance.GenerateBlocks();
					}
					else
					{
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				else if (this.loadState == CityConstructor.LoadState.generateDensity)
				{
					if (this.generateNew)
					{
						HighlanderSingleton<CityDensity>.Instance.GenerateDensity();
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
					else
					{
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				else if (this.loadState == CityConstructor.LoadState.generateBuildings)
				{
					HighlanderSingleton<CityBuildings>.Instance.GenerateBuildings();
					CityData.Instance.CreateSingletons();
				}
				else if (this.loadState == CityConstructor.LoadState.generatePathfinding)
				{
					PathFinder.Instance.CompilePathFindingMap(true);
					this.loadingProgress = 1f;
					this.stateComplete = true;
				}
				else if (this.loadState == CityConstructor.LoadState.generateBlueprints)
				{
					BlueprintsCreator.Instance.StartLoading();
				}
				else if (this.loadState == CityConstructor.LoadState.generateCompanies)
				{
					if (this.generateNew)
					{
						List<NewAddress> list = new List<NewAddress>(CityData.Instance.addressDirectory);
						string seed = CityData.Instance.seed;
						while (list.Count > 0)
						{
							int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, seed, out seed);
							NewAddress newAddress = list[psuedoRandomNumberContained];
							newAddress.CalculateLandValue();
							newAddress.AssignPurpose();
							list.RemoveAt(psuedoRandomNumberContained);
						}
					}
					this.loadingProgress = 1f;
					this.stateComplete = true;
				}
				else if (this.loadState == CityConstructor.LoadState.connectRooms)
				{
					if (!this.generateNew)
					{
						foreach (KeyValuePair<int, NewWall> keyValuePair in this.loadingWallsReference)
						{
							if (!this.loadingWallsReference.TryGetValue(keyValuePair.Value.otherWallID, ref keyValuePair.Value.otherWall))
							{
								Game.LogError("Cannot find wall " + keyValuePair.Value.otherWallID.ToString() + " in loading walls reference of " + this.loadingWallsReference.Count.ToString(), 2);
							}
							if (!this.loadingWallsReference.TryGetValue(keyValuePair.Value.parentWallID, ref keyValuePair.Value.parentWall))
							{
								Game.LogError("Cannot find wall " + keyValuePair.Value.parentWallID.ToString() + " in loading walls reference of " + this.loadingWallsReference.Count.ToString(), 2);
							}
							if (!this.loadingWallsReference.TryGetValue(keyValuePair.Value.childWallID, ref keyValuePair.Value.childWall))
							{
								Game.LogError("Cannot find wall " + keyValuePair.Value.childWallID.ToString() + " in loading walls reference of " + this.loadingWallsReference.Count.ToString(), 2);
							}
						}
						foreach (NewAddress newAddress2 in CityData.Instance.addressDirectory)
						{
							foreach (NewRoom newRoom in newAddress2.rooms)
							{
								foreach (NewNode newNode in newRoom.nodes)
								{
									foreach (NewWall newWall in newNode.walls)
									{
										newWall.SetDoorPairPreset(newWall.preset, false, newWall.preset.divider, true);
									}
								}
							}
						}
					}
					foreach (StreetController streetController in CityData.Instance.streetDirectory)
					{
						GenerationController.Instance.UpdateWallsRoom(streetController.nullRoom);
					}
					foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
					{
						foreach (KeyValuePair<int, NewFloor> keyValuePair2 in newBuilding.floors)
						{
							keyValuePair2.Value.AssignWindowUVData(false);
						}
					}
					RoomsLoader.Instance.StartLoading();
				}
				else if (this.loadState == CityConstructor.LoadState.generateCitizens)
				{
					if (Game.Instance.collectDebugData)
					{
						foreach (NewGameLocation newGameLocation in CityData.Instance.gameLocationDirectory)
						{
							using (List<NewNode.NodeAccess>.Enumerator enumerator10 = newGameLocation.entrances.GetEnumerator())
							{
								while (enumerator10.MoveNext())
								{
									NewNode.NodeAccess acc = enumerator10.Current;
									if (!acc.fromNode.room.entrances.Exists((NewNode.NodeAccess item) => item.fromNode == acc.fromNode && item.toNode == acc.toNode))
									{
										Game.LogError("An entrance in a gamelocation was not found in it's room!", 2);
									}
								}
							}
						}
					}
					CitizenCreator.Instance.StartLoading();
				}
				else if (this.loadState == CityConstructor.LoadState.generateRelationships)
				{
					if (this.generateNew)
					{
						RelationshipCreator.Instance.StartLoading();
						if (Game.Instance.printDebug)
						{
							Game.Log("CityGen: Load state key after relationships: " + Toolbox.Instance.lastRandomNumberKey, 2);
						}
						GroupsController.Instance.CreateGroups();
					}
					else
					{
						GroupsController.Instance.LoadGroups();
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				else if (this.loadState == CityConstructor.LoadState.gatherData)
				{
					this.GatherData();
					this.loadingProgress = 1f;
					this.stateComplete = true;
				}
				else if (this.loadState == CityConstructor.LoadState.generateAirDucts)
				{
					AirDuctsCreator.Instance.StartLoading();
				}
				else if (this.loadState == CityConstructor.LoadState.generateEvidence)
				{
					StartingEvidenceCreator.Instance.StartLoading();
				}
				else if (this.loadState == CityConstructor.LoadState.generateInteriors)
				{
					if (this.generateNew)
					{
						InteriorCreator.Instance.StartLoading();
					}
					else
					{
						foreach (NewGameLocation newGameLocation2 in CityData.Instance.gameLocationDirectory)
						{
							foreach (NewRoom newRoom2 in newGameLocation2.rooms)
							{
								newRoom2.LoadOwners();
							}
						}
						foreach (Company company in CityData.Instance.companyDirectory)
						{
							company.CreateItemSingletons();
							company.UpdatePassedWorkPosition();
						}
						foreach (CitySaveData.HumanCitySave humanCitySave in CityConstructor.Instance.currentData.citizens)
						{
							Human human = null;
							if (CityData.Instance.citizenDictionary.TryGetValue(humanCitySave.humanID, ref human))
							{
								human.LoadFavourites(humanCitySave);
							}
						}
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				else if (this.loadState == CityConstructor.LoadState.prepareCitizens)
				{
					foreach (Citizen citizen in CityData.Instance.citizenDirectory)
					{
						citizen.PrepForStart();
					}
					Player.Instance.PrepForStart();
					this.loadingProgress = 1f;
					this.stateComplete = true;
				}
				else if (this.loadState == CityConstructor.LoadState.loadObjects)
				{
					if (!this.generateNew)
					{
						if (this.saveState != null)
						{
							SaveStateController.Instance.PreLoadCases(ref this.saveState);
						}
						ObjectsCreator.Instance.StartLoading();
					}
					else
					{
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				else if (this.loadState == CityConstructor.LoadState.finalizing)
				{
					this.Finalized();
					this.loadingProgress = 1f;
					this.stateComplete = true;
				}
				else if (this.loadState == CityConstructor.LoadState.savingData)
				{
					if (this.generateNew)
					{
						base.StartCoroutine(this.SaveCityData());
					}
					else
					{
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				else if (this.loadState == CityConstructor.LoadState.loadState)
				{
					this.FinalizePostSave();
					if (this.saveState != null)
					{
						SaveStateController.Instance.LoadSaveState(this.saveState);
					}
					else
					{
						UpgradeEffectController.Instance.OnSyncDiskChange(true);
						FirstPersonItemController.Instance.SetSlotSize(GameplayControls.Instance.defaultInventorySlots + Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.increaseInventory)));
						if (!Game.Instance.sandboxMode)
						{
							ChapterController.Instance.LoadChapter(ChapterController.Instance.allChapters[Game.Instance.loadChapter], true);
						}
					}
					if (Game.Instance.sandboxMode && this.saveState == null && Player.Instance.home != null)
					{
						foreach (NewRoom newRoom3 in Player.Instance.home.rooms)
						{
							newRoom3.SetMainLights(true, "Set main lights on", null, false, false);
							foreach (NewNode.NodeAccess nodeAccess in Player.Instance.home.entrances)
							{
								if (nodeAccess.wall.door != null)
								{
									nodeAccess.wall.door.SetLocked(false, Player.Instance, false);
								}
							}
						}
					}
					this.loadingProgress = 1f;
					this.stateComplete = true;
				}
				else if (this.loadState == CityConstructor.LoadState.preSim)
				{
					if (!Game.Instance.sandboxMode && !this.preSimOccured && !this.preSimActive && this.saveState == null && ChapterController.Instance.loadedChapter != null && ChapterController.Instance.loadedChapter.usePreSimulation)
					{
						this.SetPreSim(true);
					}
					else
					{
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				else if (this.loadState == CityConstructor.LoadState.loadComplete)
				{
					if (this.saveState == null && ((ChapterController.Instance.loadedChapter != null && ChapterController.Instance.loadedChapter.askToEnableTutorial) || !PlayerPrefsController.Instance.playedBefore || SessionData.Instance.enableTutorialText))
					{
						PopupMessageController.Instance.PopupMessage("TutorialEnable", true, true, "Off", "On", true, PopupMessageController.AffectPauseState.no, false, "", false, false, false, false, "", "", false, "", false, "", "");
						PopupMessageController.Instance.OnLeftButton += this.DisableTutorial;
						PopupMessageController.Instance.OnRightButton += this.EnableTutorial;
					}
					else
					{
						this.loadingProgress = 1f;
						this.stateComplete = true;
					}
				}
				this.loadingOperationActive = true;
			}
			if (this.stateComplete && this.loadCursor < this.allLoadStates.Count)
			{
				if (this.useCityConstructionHold && this.loadState == this.cityConstructorHoldState)
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("City Edit: Halting city generation", 2);
					}
					base.enabled = false;
					if (HighlanderSingleton<CityEditorController>.Instance != null)
					{
						HighlanderSingleton<CityEditorController>.Instance.OnHaltOnEndOfLoadState(this.cityConstructorHoldState);
					}
				}
				if (Game.Instance.devMode && Game.Instance.collectDebugData && this.debugLoadTime != null)
				{
					int num2 = Mathf.CeilToInt(Time.realtimeSinceStartup - this.timeStamp);
					if (Game.Instance.printDebug)
					{
						string[] array = new string[7];
						array[0] = "CityGen: Completed load phase ";
						array[1] = this.loadState.ToString();
						array[2] = " ";
						int num3 = 3;
						int num = (int)this.loadState;
						array[num3] = num.ToString();
						array[4] = " in ";
						array[5] = num2.ToString();
						array[6] = " seconds...";
						Game.Log(string.Concat(array), 2);
					}
					if (!this.debugLoadTime.loadTimes.ContainsKey(this.loadState))
					{
						this.debugLoadTime.loadTimes.Add(this.loadState, num2);
					}
					else
					{
						this.debugLoadTime.loadTimes[this.loadState] = num2;
					}
				}
				this.loadCursor++;
				if (this.loadCursor < this.allLoadStates.Count)
				{
					this.loadState = (CityConstructor.LoadState)this.loadCursor;
					if (Game.Instance.printDebug)
					{
						Game.Log(string.Concat(new string[]
						{
							"CityGen: Load state ",
							this.loadCursor.ToString(),
							" (",
							this.loadState.ToString(),
							"). Random key: ",
							Toolbox.Instance.lastRandomNumberKey
						}), 2);
					}
				}
				this.loadingProgress = 0f;
				this.stateComplete = false;
				this.loadingOperationActive = false;
				this.SetLoadingText();
				return;
			}
		}
		else
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("CityGen: Loading cycle is complete!", 2);
			}
			if (Game.Instance.devMode && Game.Instance.collectDebugData && this.debugLoadTime != null)
			{
				this.WriteSavingTimings(ref this.debugLoadTime);
				this.WriteRoomDecorTimings(ref this.debugLoadTime);
				this.WriteGeneratedObjectDetails();
			}
			if (this.OnLoadFinalize != null)
			{
				this.OnLoadFinalize();
			}
			this.loadingOperationActive = false;
			this.isLoaded = true;
			MainMenuController.Instance.loadingText.text = Strings.Get("ui.loading", "Entering", Strings.Casing.asIs, false, false, false, null) + " " + this.currentData.cityName + "...";
			MainMenuController.Instance.loadingSlider.value = 1f;
			base.enabled = false;
			this.StartGame();
			this.saveState = null;
		}
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00059544 File Offset: 0x00057744
	public void StopCityConstructionAtEndOfLoadState(CityConstructor.LoadState stopHereState)
	{
		this.useCityConstructionHold = true;
		this.cityConstructorHoldState = stopHereState;
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x00059554 File Offset: 0x00057754
	public void ClearCityConstructionHoldStatus()
	{
		this.useCityConstructionHold = false;
		base.enabled = true;
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x00059564 File Offset: 0x00057764
	private void WriteSavingTimings(ref CityConstructor.CollectedLoadTimeInfo info)
	{
		if (!info.generateNew)
		{
			return;
		}
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/generation_timing_",
			info.build,
			info.citySize,
			".txt"
		});
		List<string> list = new List<string>();
		try
		{
			string[] array = File.ReadAllLines(text);
			if (Game.Instance.printDebug)
			{
				Game.Log("CityGen: Read existing load time contents: " + array.Length.ToString() + " lines...", 2);
			}
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					' ',
					'(',
					')'
				}, 1);
				float num = 0f;
				int num2 = 0;
				if (array2.Length > 2)
				{
					string loadState = array2[0].Substring(0, array2[0].Length - 1);
					if (Game.Instance.printDebug)
					{
						Game.Log("CityGen: Parsing load state " + loadState + "...", 2);
					}
					int num3 = this.allLoadStates.FindIndex((CityConstructor.LoadState item) => item.ToString() == loadState);
					if (num3 > -1)
					{
						CityConstructor.LoadState loadState2 = this.allLoadStates[num3];
						if (Game.Instance.printDebug)
						{
							Game.Log("CityGen: Parsed load state " + loadState2.ToString() + " from " + loadState, 2);
						}
						if (float.TryParse(array2[3], ref num))
						{
							num += (float)info.loadTimes[loadState2];
						}
						else if (Game.Instance.printDebug)
						{
							Game.Log("CityGen: Unable to parse float from " + array2[1], 2);
						}
						if (int.TryParse(array2[2], ref num2))
						{
							num2++;
						}
						else if (Game.Instance.printDebug)
						{
							Game.Log("CityGen: Unable to parse int from " + array2[2], 2);
						}
						float num4 = num / (float)num2;
						if (Game.Instance.printDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"CityGen: New average for ",
								loadState,
								" from ",
								num2.ToString(),
								" entries: ",
								num4.ToString(),
								" (this: ",
								info.loadTimes[loadState2].ToString(),
								", previous: ",
								array2[1],
								")"
							}), 2);
						}
						string text2 = string.Concat(new string[]
						{
							loadState2.ToString(),
							": ",
							num4.ToString(),
							" (",
							num2.ToString(),
							" (",
							num.ToString(),
							"))"
						});
						list.Add(text2);
					}
					else if (Game.Instance.printDebug)
					{
						Game.Log("CityGen: Unable to get index in existing for " + loadState, 2);
					}
				}
			}
		}
		catch
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("CityGen: No existing load time contents, creating new file at " + text, 2);
			}
			foreach (KeyValuePair<CityConstructor.LoadState, int> keyValuePair in info.loadTimes)
			{
				string text3 = string.Concat(new string[]
				{
					keyValuePair.Key.ToString(),
					": ",
					keyValuePair.Value.ToString(),
					" (1 (",
					keyValuePair.Value.ToString(),
					"))"
				});
				list.Add(text3);
			}
		}
		StreamWriter streamWriter = new StreamWriter(text, false);
		if (Game.Instance.printDebug)
		{
			Game.Log("CityGen: Writing " + list.Count.ToString() + " lines to " + text, 2);
		}
		for (int j = 0; j < list.Count; j++)
		{
			streamWriter.WriteLine(list[j]);
		}
		streamWriter.Close();
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x000599E0 File Offset: 0x00057BE0
	private void WriteRoomDecorTimings(ref CityConstructor.CollectedLoadTimeInfo info)
	{
		if (!this.generateNew)
		{
			return;
		}
		string text = Application.persistentDataPath + "/decortime_info.txt";
		List<string> list = new List<string>();
		List<CityConstructor.DecorTotalTime> list2 = new List<CityConstructor.DecorTotalTime>();
		Dictionary<NewRoom, List<string>> dictionary = new Dictionary<NewRoom, List<string>>();
		foreach (KeyValuePair<NewRoom, List<CityConstructor.DecorClusterGenerationTimeInfo>> keyValuePair in info.decorTimes)
		{
			try
			{
				CityConstructor.DecorTotalTime decorTotalTime = new CityConstructor.DecorTotalTime();
				decorTotalTime.room = keyValuePair.Key;
				dictionary.Add(keyValuePair.Key, new List<string>());
				dictionary[keyValuePair.Key].Add(string.Empty);
				dictionary[keyValuePair.Key].Add(string.Empty);
				dictionary[keyValuePair.Key].Add(string.Empty);
				dictionary[keyValuePair.Key].Add(string.Concat(new string[]
				{
					"-----Furniture cluster timings for room ",
					keyValuePair.Key.roomID.ToString(),
					" (",
					keyValuePair.Key.nodes.Count.ToString(),
					" nodes)-----"
				}));
				dictionary[keyValuePair.Key].Add(string.Empty);
				keyValuePair.Value.Sort((CityConstructor.DecorClusterGenerationTimeInfo p2, CityConstructor.DecorClusterGenerationTimeInfo p1) => p1.time.CompareTo(p2.time));
				foreach (CityConstructor.DecorClusterGenerationTimeInfo decorClusterGenerationTimeInfo in keyValuePair.Value)
				{
					dictionary[keyValuePair.Key].Add(string.Concat(new string[]
					{
						decorClusterGenerationTimeInfo.cluster.name,
						": ",
						decorClusterGenerationTimeInfo.time.ToString(),
						" Found: ",
						decorClusterGenerationTimeInfo.found.ToString()
					}));
					decorTotalTime.totalTime += decorClusterGenerationTimeInfo.time;
				}
				list2.Add(decorTotalTime);
			}
			catch
			{
			}
		}
		list2.Sort((CityConstructor.DecorTotalTime p2, CityConstructor.DecorTotalTime p1) => p1.totalTime.CompareTo(p2.totalTime));
		StreamWriter streamWriter = new StreamWriter(text, false);
		if (Game.Instance.printDebug)
		{
			Game.Log("CityGen: Writing " + list.Count.ToString() + " lines to " + text, 2);
		}
		foreach (CityConstructor.DecorTotalTime decorTotalTime2 in list2)
		{
			if (dictionary.ContainsKey(decorTotalTime2.room))
			{
				for (int i = 0; i < dictionary[decorTotalTime2.room].Count; i++)
				{
					streamWriter.WriteLine(dictionary[decorTotalTime2.room][i]);
				}
			}
		}
		streamWriter.Close();
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x00059D58 File Offset: 0x00057F58
	private void WriteGeneratedObjectDetails()
	{
		if (!this.generateNew)
		{
			return;
		}
		string text = Application.persistentDataPath + "/object_info.txt";
		List<string> list = new List<string>();
		list.Add(CityData.Instance.seed);
		list.Add(CityData.Instance.interactableDirectory.Count.ToString() + " objects...");
		list.Add("World id assign: " + Interactable.worldAssignID.ToString());
		list.Add(string.Empty);
		Dictionary<InteractablePreset, int> dictionary = new Dictionary<InteractablePreset, int>();
		CityData.Instance.interactableDirectory.Sort((Interactable p2, Interactable p1) => p1.preset.presetName.CompareTo(p2.preset.presetName));
		foreach (Interactable interactable in CityData.Instance.interactableDirectory)
		{
			if (!dictionary.ContainsKey(interactable.preset))
			{
				dictionary.Add(interactable.preset, 0);
			}
			Dictionary<InteractablePreset, int> dictionary2 = dictionary;
			InteractablePreset preset = interactable.preset;
			int num = dictionary2[preset];
			dictionary2[preset] = num + 1;
		}
		foreach (KeyValuePair<InteractablePreset, int> keyValuePair in dictionary)
		{
			list.Add(keyValuePair.Key.name + ": " + keyValuePair.Value.ToString());
		}
		list.Add(string.Empty);
		list.Add(string.Empty);
		int num2 = 0;
		Dictionary<FurnitureClass, int> dictionary3 = new Dictionary<FurnitureClass, int>();
		foreach (NewRoom newRoom in CityData.Instance.roomDirectory)
		{
			foreach (FurnitureLocation furnitureLocation in newRoom.individualFurniture)
			{
				if (!dictionary3.ContainsKey(furnitureLocation.furnitureClasses[0]))
				{
					dictionary3.Add(furnitureLocation.furnitureClasses[0], 0);
				}
				Dictionary<FurnitureClass, int> dictionary4 = dictionary3;
				FurnitureClass furnitureClass = furnitureLocation.furnitureClasses[0];
				int num = dictionary4[furnitureClass];
				dictionary4[furnitureClass] = num + 1;
				num2++;
			}
		}
		list.Add(num2.ToString() + " furniture...");
		list.Add(string.Empty);
		foreach (KeyValuePair<FurnitureClass, int> keyValuePair2 in dictionary3)
		{
			list.Add(keyValuePair2.Key.name + ": " + keyValuePair2.Value.ToString());
		}
		list.Add(string.Empty);
		list.Add(string.Empty);
		for (int i = 0; i < CityData.Instance.roomDirectory.Count; i++)
		{
			NewRoom newRoom2 = CityData.Instance.roomDirectory[i];
			list.Add(string.Concat(new string[]
			{
				"Room ",
				newRoom2.roomID.ToString(),
				" ",
				newRoom2.GetName(),
				" ended with ",
				newRoom2.seed,
				", furniture placed: ",
				newRoom2.furnitureAssignID.ToString(),
				" & ",
				newRoom2.interactableAssignID.ToString(),
				" objects placed. Clusters: ",
				newRoom2.clustersPlaced,
				", Objects (pool: ",
				newRoom2.poolSizeOnPlacement.ToString(),
				" with key ",
				newRoom2.palcementKey1,
				" -> ",
				newRoom2.palcementKey2,
				" -> ",
				newRoom2.palcementKey3,
				" -> ",
				newRoom2.palcementKey4,
				" -> (",
				newRoom2.palcementKey51,
				" -> ",
				newRoom2.palcementKey52,
				") -> ",
				newRoom2.palcementKey5,
				" -> ",
				newRoom2.palcementKey6,
				", original ",
				newRoom2.keyAtStart,
				"): ",
				newRoom2.itemsPlaced
			}));
		}
		list.Add(string.Empty);
		list.Add(string.Empty);
		for (int j = 0; j < CityData.Instance.addressDirectory.Count; j++)
		{
			NewAddress newAddress = CityData.Instance.addressDirectory[j];
			list.Add(string.Concat(new string[]
			{
				"Address ",
				newAddress.id.ToString(),
				" ",
				newAddress.name,
				" ended with seed ",
				newAddress.seed
			}));
		}
		list.Add(string.Empty);
		list.Add(string.Empty);
		for (int k = 0; k < CityData.Instance.streetDirectory.Count; k++)
		{
			StreetController streetController = CityData.Instance.streetDirectory[k];
			list.Add(string.Concat(new string[]
			{
				"Street ",
				streetController.streetID.ToString(),
				" ",
				streetController.name,
				" ended with seed ",
				streetController.seed
			}));
		}
		StreamWriter streamWriter = new StreamWriter(text, false);
		if (Game.Instance.printDebug)
		{
			Game.Log("CityGen: Writing " + list.Count.ToString() + " lines to " + text, 2);
		}
		for (int l = 0; l < list.Count; l++)
		{
			streamWriter.WriteLine(list[l]);
		}
		streamWriter.Close();
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0005A3C0 File Offset: 0x000585C0
	private void SetLoadingText()
	{
		MainMenuController.Instance.loadingText.text = Strings.Get("ui.loading", this.loadState.ToString(), Strings.Casing.asIs, false, false, false, null);
		this.displayedProgress = Mathf.RoundToInt(this.loadingProgress * 100f);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0005A414 File Offset: 0x00058614
	private void GatherData()
	{
		if (this.generateNew)
		{
			HighlanderSingleton<CityDistricts>.Instance.districtDirectory.Sort();
			HighlanderSingleton<CityDistricts>.Instance.districtDirectory.Reverse();
			foreach (DistrictController districtController in HighlanderSingleton<CityDistricts>.Instance.districtDirectory)
			{
				districtController.PopulateData();
			}
			foreach (StreetController streetController in CityData.Instance.streetDirectory)
			{
				streetController.UpdateName(false);
			}
			foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
			{
				newBuilding.UpdateName(false);
			}
			foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
			{
				newAddress.CalculateRoomOwnership();
			}
		}
		CityData.Instance.GenerateEchelonDecorData();
		foreach (NewAddress newAddress2 in CityData.Instance.addressDirectory)
		{
			if (newAddress2.featuresNeonSignageHorizontal)
			{
				newAddress2.CreateSignageHorizontal();
			}
			if (newAddress2.featuresNeonSignageVertical)
			{
				newAddress2.CreateSignageVertical();
			}
		}
		foreach (NewBuilding newBuilding2 in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			newBuilding2.SpawnStreetCables();
		}
		for (int i = 0; i < CityData.Instance.unemployedDirectory.Count; i++)
		{
			if (CityData.Instance.unemployedDirectory[i].employee == null)
			{
				CityData.Instance.unemployedDirectory.RemoveAt(i);
				i--;
			}
		}
		CityData.Instance.averageShoeSize = CityData.Instance.averageShoeSize / (float)CityData.Instance.citizenDirectory.Count;
		for (int j = 0; j < CityData.Instance.companyDirectory.Count; j++)
		{
			CityData.Instance.companyDirectory[j].OpenCloseCheck();
		}
		if (!this.generateNew && this.currentData.playersApartment > -1)
		{
			NewAddress newAddress3 = CityData.Instance.addressDirectory.Find((NewAddress item) => item.id == CityConstructor.Instance.currentData.playersApartment);
			if (newAddress3 != null)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log(string.Concat(new string[]
					{
						"CityGen: Loading player's apartment from city data (",
						this.currentData.playersApartment.ToString(),
						": ",
						newAddress3.name,
						")"
					}), 2);
				}
				Player.Instance.home = newAddress3;
				Player.Instance.residence = Player.Instance.home.residence;
				Player.Instance.SetResidence(Player.Instance.home.residence, true);
			}
		}
		FirstPersonItemController.Instance.GenerateSkinColourMaterials();
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0005A7AC File Offset: 0x000589AC
	private void Finalized()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("CityGen: Set scene dirty", 2);
		}
		SessionData.Instance.dirtyScene = true;
		foreach (NewRoom newRoom in CityData.Instance.roomDirectory)
		{
			foreach (NewNode newNode in newRoom.nodes)
			{
				if (newNode.floorType != NewNode.FloorTileType.none)
				{
					foreach (NewWall newWall in newNode.walls)
					{
						if ((newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.window || newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge) && newWall.node.room != null && newWall.otherWall.node.room != null)
						{
							newWall.node.room.AddEntrance(newWall.node, newWall.otherWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
							newWall.otherWall.node.room.AddEntrance(newWall.otherWall.node, newWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
						}
					}
				}
			}
		}
		CityData.Instance.CreateCityDirectory();
		if (this.generateNew)
		{
			PipeConstructor.Instance.GeneratePipes();
			foreach (Citizen citizen in CityData.Instance.citizenDirectory)
			{
				citizen.PickPassword();
			}
			foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
			{
				newAddress.PickPassword();
				foreach (NewRoom newRoom2 in newAddress.rooms)
				{
					newRoom2.PickPassword();
					newRoom2.pickFurnitureCache = null;
				}
			}
			if (Game.Instance.printDebug)
			{
				Game.Log("CityGen: Last random key: " + CityData.Instance.seed + " = " + Toolbox.Instance.lastRandomNumberKey, 2);
			}
			if (Game.Instance.printDebug)
			{
				Game.Log("CityGen: Generated " + CityData.Instance.interactableDirectory.Count.ToString() + " objects", 2);
			}
		}
		else
		{
			foreach (CitySaveData.EvidenceStateSave evidenceStateSave in this.currentData.multiPage)
			{
				Evidence evidence = null;
				if (Toolbox.Instance.TryGetEvidence(evidenceStateSave.id, out evidence))
				{
					if (evidenceStateSave.mpContent != null && evidenceStateSave.mpContent.Count > 0)
					{
						EvidenceMultiPage evidenceMultiPage = evidence as EvidenceMultiPage;
						if (evidenceMultiPage != null)
						{
							evidenceMultiPage.pageContent = new List<EvidenceMultiPage.MultiPageContent>(evidenceStateSave.mpContent);
							evidenceMultiPage.SetPage(evidenceStateSave.page, false);
						}
					}
				}
				else
				{
					Game.LogError("Unable to load multi page evidence " + evidenceStateSave.id, 2);
				}
			}
			PipeConstructor.Instance.generated = new List<PipeConstructor.PipeGroup>(this.currentData.pipes);
		}
		if (Game.Instance.keysToTheCity)
		{
			foreach (NewAddress newAddress2 in CityData.Instance.addressDirectory)
			{
				Player.Instance.AddToKeyring(newAddress2, false);
				if (newAddress2.passcode.used)
				{
					GameplayController.Instance.AddPasscode(newAddress2.passcode, false);
				}
				foreach (NewRoom newRoom3 in newAddress2.rooms)
				{
					if (newRoom3.passcode.used)
					{
						GameplayController.Instance.AddPasscode(newRoom3.passcode, false);
					}
				}
			}
			foreach (Human human in CityData.Instance.citizenDirectory)
			{
				GameplayController.Instance.AddPasscode(human.passcode, false);
			}
		}
		if (Player.Instance.home != null)
		{
			Player.Instance.AddToKeyring(Player.Instance.home, false);
		}
		foreach (PipeConstructor.PipeGroup pipeGroup in PipeConstructor.Instance.generated)
		{
			pipeGroup.AddToRoomsAsReferences();
			pipeGroup.Spawn();
		}
		foreach (SceneRecorder sceneRecorder in CitizenBehaviour.Instance.sceneRecorders)
		{
			sceneRecorder.RefreshCoveredArea();
		}
		if (Game.Instance.discoverAllRooms)
		{
			foreach (NewRoom newRoom4 in CityData.Instance.roomDirectory)
			{
				newRoom4.SetExplorationLevel(2);
				foreach (AirDuctGroup airDuctGroup in newRoom4.ductGroups)
				{
					foreach (AirDuctGroup.AirDuctSection airDuctSection in airDuctGroup.airDucts)
					{
						airDuctSection.SetDiscovered(true);
					}
				}
			}
		}
		UpgradesController.Instance.Setup();
		GameplayController.Instance.UpdateMatches();
		foreach (Citizen citizen2 in CityData.Instance.citizenDirectory)
		{
			citizen2.BirthdayCheck();
		}
		if (this.saveState == null)
		{
			MurderController.Instance.maxDifficultyLevel = 1;
			foreach (Company company in CityData.Instance.companyDirectory)
			{
				company.GenerateFakeSalesRecords();
			}
			GameplayController.Instance.money = 0;
			GameplayController.Instance.lockPicks = 0;
			if (Game.Instance.sandboxMode)
			{
				GameplayController.Instance.SetMoney(Game.Instance.sandboxStartingMoney);
				GameplayController.Instance.AddLockpicks(Game.Instance.sandboxStartingLockpicks, false);
			}
			else
			{
				GameplayController.Instance.SetMoney(GameplayControls.Instance.startingMoney);
				GameplayController.Instance.SetLockpicks(0);
			}
		}
		FirstPersonItemController.Instance.SetFirstPersonSkinColour();
		foreach (Interactable interactable in this.updateSwitchState)
		{
			interactable.SetSwitchState(interactable.sw0, null, true, true, false);
			interactable.SetCustomState1(interactable.sw1, null, true, true, false);
			interactable.SetCustomState2(interactable.sw2, null, true, true, false);
			interactable.SetCustomState3(interactable.sw3, null, true, true, false);
			interactable.SetLockedState(interactable.locked, null, true, true);
			interactable.SetPhysicsPickupState(interactable.phy, null, true, true);
		}
		this.updateSwitchState.Clear();
		GameplayController.Instance.ProcessDynamicTextImages();
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0005B0F4 File Offset: 0x000592F4
	private void FinalizePostSave()
	{
		if (Game.Instance.sandboxMode && !Game.Instance.sandboxStartingApartment)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("CityGen: Setting player as having no apartment...", 2);
			}
			Player.Instance.SetResidence(null, true);
			CityData.Instance.CreateCityDirectory();
		}
		if (Player.Instance.home != null)
		{
			SessionData.Instance.startingNode = Player.Instance.FindSafeTeleport(Player.Instance.home, false, true);
		}
		else
		{
			List<StreetController> list = CityData.Instance.streetDirectory.FindAll((StreetController item) => item.entrances.Count > 0 && item.rooms.Count > 0 && item.nodes.Count >= 8);
			SessionData.Instance.startingNode = Player.Instance.FindSafeTeleport(list[Toolbox.Instance.Rand(0, list.Count, false)], false, true);
		}
		Player.Instance.Teleport(SessionData.Instance.startingNode, null, false, false);
		Game.Instance.disableCaseBoardClose = false;
		if (CityControls.Instance.basementWaterTransform != null)
		{
			float num = (CityData.Instance.citySize.x - 2f) * CityControls.Instance.cityTileSize.x;
			float num2 = (CityData.Instance.citySize.y - 2f) * CityControls.Instance.cityTileSize.y;
			CityControls.Instance.basementWaterTransform.localScale = new Vector3(num, num2, CityControls.Instance.basementWaterTransform.localScale.z);
		}
		if (HighlanderSingleton<CityEditorController>.Instance != null && SessionData.Instance.isCityEdit)
		{
			HighlanderSingleton<CityEditorController>.Instance.SetCityEditor(false);
		}
		MapController.Instance.Setup();
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0005B2B0 File Offset: 0x000594B0
	public void SetPreSim(bool val)
	{
		if (val != this.preSimActive)
		{
			if (this.preSimActive && !val)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("CityGen: Pre Sim is complete...", 2);
				}
				this.preSimOccured = true;
				this.loadingProgress = 1f;
				this.stateComplete = true;
			}
			this.preSimActive = val;
			if (Game.Instance.printDebug)
			{
				Game.Log("CityGen: Pre Simulation active: " + this.preSimActive.ToString(), 2);
			}
			if (this.preSimActive)
			{
				int num = 0;
				foreach (Company company in CityData.Instance.companyDirectory)
				{
					if (company.placeOfBusiness.thisAsAddress != null && company.IsOpenAtThisTime(SessionData.Instance.gameTime))
					{
						if (company.placeOfBusiness.streetAccess != null && company.placeOfBusiness.streetAccess.door != null)
						{
							company.placeOfBusiness.streetAccess.door.SetLocked(false, null, false);
							num++;
						}
						foreach (NewRoom newRoom in company.address.rooms)
						{
							newRoom.SetMainLights(true, "Set open on presim", null, true, true);
						}
					}
				}
				if (Game.Instance.printDebug)
				{
					Game.Log("CityGen: Unlocked doors of " + num.ToString() + " businesses, ready for presim...", 2);
				}
				Player.Instance.OnCityTileChange();
				SessionData.Instance.startedGame = true;
				SessionData.Instance.ResumeGame();
				CitizenBehaviour.Instance.StartGame();
				SessionData.Instance.SetTimeSpeed(SessionData.TimeSpeed.veryFast);
				return;
			}
			SessionData.Instance.SetTimeSpeed(SessionData.TimeSpeed.normal);
			SessionData.Instance.PauseGame(false, false, true);
			SessionData.Instance.startedGame = false;
		}
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0005B4C4 File Offset: 0x000596C4
	public void StartGame()
	{
		if (MainMenuController.Instance.feedbackPlayerInfo != null)
		{
			MainMenuController.Instance.feedbackPlayerInfo.citySeed = Toolbox.Instance.GetShareCode(CityData.Instance.cityName, (int)CityData.Instance.citySize.x, (int)CityData.Instance.citySize.y, Game.Instance.buildID, CityData.Instance.seed);
		}
		InterfaceController.Instance.caseScrollingViewRect.OnInputModeChange();
		CameraController.Instance.SetupFPS();
		this.loadingWallsReference = null;
		this.loadingFurnitureReference = null;
		Player.Instance.OnCityTileChange();
		Player.Instance.UpdateSkinWidth();
		InterfaceController.Instance.SetInterfaceActive(true);
		GeometryCullingController.Instance.OnStartGame();
		SessionData.Instance.startedGame = true;
		SessionData.Instance.ResumeGame();
		CitizenBehaviour.Instance.StartGame();
		AudioController.Instance.StartAmbienceTracks();
		if (this.saveState == null)
		{
			NewspaperController.Instance.GenerateNewNewspaper();
			Game.Instance.SetGameLength(MainMenuController.Instance.gameLengthDropdown.dropdown.value, true, false, false);
			if (Game.Instance.timeLimited)
			{
				SessionData.Instance.gameTimeLimit = Game.Instance.timeLimit * 60f;
			}
		}
		else
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Murder: Updating states for " + MurderController.Instance.activeMurders.Count.ToString() + " active murders...", 2);
			}
			foreach (MurderController.Murder murder in MurderController.Instance.activeMurders)
			{
				murder.SetMurderState(murder.state, true);
			}
		}
		MurderController.Instance.OnStartGame();
		if (Game.Instance.timeLimited)
		{
			SessionData.Instance.gameTimeLimit = Mathf.Min(SessionData.Instance.gameTimeLimit, Game.Instance.timeLimit * 60f);
			SessionData.Instance.UpdateGameTimerText();
		}
		InterfaceController.Instance.fade = 1f;
		InterfaceController.Instance.Fade(0f, 2f, true);
		MainMenuController.Instance.EnableMainMenu(false, true, true, MainMenuController.Component.mainMenuButtons);
		if (Player.Instance.home != null)
		{
			Player.Instance.home.entrances.Find((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.door).door.SetKnowLockedStatus(true);
		}
		SessionData.Instance.skyboxGradientIndex = 0;
		SessionData.Instance.ExecuteWeatherChange();
		SessionData.Instance.ExecuteWetnessChange();
		SessionData.Instance.ExecuteWindChange();
		CasePanelController.Instance.UpdateCaseControls();
		CasePanelController.Instance.UpdateCaseButtonsActive();
		CitizenBehaviour.Instance.OnDayChange();
		CitizenBehaviour.Instance.OnHourChange();
		SessionData.Instance.UpdateTutorialNotifications();
		AudioController.Instance.UpdateAllLoopingSoundOcclusion();
		AudioController.Instance.UpdateAmbientZonesOnEndOfFrame();
		InterfaceController.Instance.UpdateDOF();
		if (Game.Instance.sandboxMode)
		{
			MurderController.Instance.SetProcGenKillerLoop(Game.Instance.enableMurdererInSandbox);
			if (this.saveState == null)
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, Player.Instance.GetCitizenName(), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, string.Concat(new string[]
			{
				SessionData.Instance.CurrentTimeString(false, true),
				", ",
				SessionData.Instance.LongDateString(SessionData.Instance.gameTime, true, true, true, true, true, false, false, true),
				", ",
				Player.Instance.currentGameLocation.name
			}), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
		if (Game.Instance.allHospitalAccess)
		{
			foreach (NewAddress newAddress in CityData.Instance.addressDirectory.FindAll((NewAddress item) => item.addressPreset != null && item.addressPreset.name == "HospitalWard"))
			{
				Player.Instance.AddToKeyring(newAddress, false);
				if (newAddress.passcode != null)
				{
					GameplayController.Instance.AddPasscode(newAddress.passcode, false);
				}
			}
		}
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			foreach (Company company in CityData.Instance.companyDirectory)
			{
				if (company.address != null)
				{
					company.address.debugCompanyShifts = company.shifts;
				}
			}
		}
		Player.Instance.UpdateCurrentBuildingModelVisibility();
		MeshPoolingController.Instance.StartCachingProcess();
		if (!Game.Instance.skipIntro && this.saveState == null)
		{
			CutSceneController.Instance.PlayCutScene(GameplayControls.Instance.intro);
			AudioController.LoopingSoundInfo onlyMusicSnapshot = AudioController.Instance.Play2DLooping(AudioControls.Instance.musicOnlySnapshot, null, 1f);
			Player.Instance.onlyMusicSnapshot = onlyMusicSnapshot;
		}
		else
		{
			this.TriggerStartEvent();
		}
		if (Player.Instance.currentNode != null && InteractionController.Instance.lockedInInteraction == null && !Player.Instance.inAirVent)
		{
			if (Player.Instance.currentNode.individualFurniture.Exists((FurnitureLocation item) => item.furniture != null && item.furniture.onLoadAdjacentPlayerTeleport))
			{
				Game.Log("Player: Attempting to teleport player to an adjacent node...", 2);
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
				for (int i = 0; i < offsetArrayX.Length; i++)
				{
					Vector2 vector = offsetArrayX[i];
					Vector3 vector2 = Player.Instance.currentNode.nodeCoord + new Vector3Int((int)vector.x, (int)vector.y, 0);
					NewNode newNode = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector2, ref newNode) && newNode.room == Player.Instance.currentNode.room && newNode.accessToOtherNodes.Count > 0 && !newNode.isInaccessable)
					{
						if (!newNode.individualFurniture.Exists((FurnitureLocation item) => item.furniture != null && item.furniture.classes.Count > 0 && item.furniture.classes[0].occupiesTile))
						{
							Player.Instance.Teleport(newNode, null, true, false);
							break;
						}
					}
				}
			}
		}
		if (Game.Instance.collectDebugData)
		{
			Benchmarking.Instance.StartBenchmarking();
		}
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0005BBA8 File Offset: 0x00059DA8
	public void TriggerStartEvent()
	{
		if (this.OnGameStarted != null)
		{
			this.OnGameStarted();
		}
		this.currentData = null;
		Object.Destroy(this);
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0005BBCC File Offset: 0x00059DCC
	private void EnableTutorial()
	{
		SessionData.Instance.SetDisplayTutorialText(true);
		PopupMessageController.Instance.OnLeftButton -= this.DisableTutorial;
		PopupMessageController.Instance.OnRightButton -= this.EnableTutorial;
		this.loadingProgress = 1f;
		this.stateComplete = true;
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x0005BC24 File Offset: 0x00059E24
	private void DisableTutorial()
	{
		SessionData.Instance.SetDisplayTutorialText(false);
		PopupMessageController.Instance.OnLeftButton -= this.DisableTutorial;
		PopupMessageController.Instance.OnRightButton -= this.EnableTutorial;
		this.loadingProgress = 1f;
		this.stateComplete = true;
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0005BC7A File Offset: 0x00059E7A
	private IEnumerator SaveCityData()
	{
		if (this.IsUsingCityEditor())
		{
			this.currentData.seed = "CityEditor";
			CityData.Instance.seed = "CityEditor";
		}
		foreach (Occupation occupation in CityData.Instance.criminalJobDirectory)
		{
			this.currentData.criminals.Add(occupation.GenerateSaveData());
		}
		foreach (DistrictController districtController in HighlanderSingleton<CityDistricts>.Instance.districtDirectory)
		{
			this.currentData.districts.Add(districtController.GenerateSaveData());
		}
		foreach (StreetController streetController in CityData.Instance.streetDirectory)
		{
			this.currentData.streets.Add(streetController.GenerateSaveData());
		}
		this.currentData.groups = new List<GroupsController.SocialGroup>(GroupsController.Instance.groups);
		this.currentData.pipes = new List<PipeConstructor.PipeGroup>(PipeConstructor.Instance.generated);
		int cursor = 0;
		List<CityTile> cityTiles = new List<CityTile>();
		using (Dictionary<Vector2Int, CityTile>.Enumerator enumerator4 = HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles.GetEnumerator())
		{
			while (enumerator4.MoveNext())
			{
				KeyValuePair<Vector2Int, CityTile> keyValuePair = enumerator4.Current;
				cityTiles.Add(keyValuePair.Value);
			}
			goto IL_26C;
		}
		IL_1DB:
		int num = 0;
		while (num < this.saveChunk && cursor < cityTiles.Count)
		{
			this.currentData.cityTiles.Add(cityTiles[cursor].GenerateSaveData());
			int num2 = cursor;
			cursor = num2 + 1;
			num++;
		}
		this.loadingProgress = (float)cursor / (float)cityTiles.Count;
		yield return null;
		IL_26C:
		if (cursor >= cityTiles.Count)
		{
			foreach (Human human in CityData.Instance.citizenDirectory)
			{
				this.currentData.citizens.Add(human.GenerateSaveData());
			}
			foreach (KeyValuePair<int, MetaObject> keyValuePair2 in CityData.Instance.metaObjectDictionary)
			{
				this.currentData.metas.Add(keyValuePair2.Value);
				keyValuePair2.Value.cd = true;
			}
			foreach (Interactable interactable in CityData.Instance.interactableDirectory)
			{
				if (interactable.save)
				{
					this.currentData.interactables.Add(interactable);
				}
			}
			this.currentData.population = CityData.Instance.citizenDirectory.Count;
			if (Player.Instance.home != null)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("CityGen: Saving player's apartment: " + Player.Instance.home.id.ToString(), 2);
				}
				this.currentData.playersApartment = Player.Instance.home.id;
			}
			else
			{
				this.currentData.playersApartment = -1;
			}
			string shareCode = Toolbox.Instance.GetShareCode(ref this.currentData);
			string cityDataPath = Application.persistentDataPath + "/Cities/" + shareCode + ".cit";
			string cityInfoPath = Application.persistentDataPath + "/Cities/" + shareCode + ".txt";
			if (Game.Instance.printDebug)
			{
				Game.Log("CityGen: Saving data: " + cityDataPath, 2);
			}
			foreach (EvidenceMultiPage evidenceMultiPage in GameplayController.Instance.multiPageEvidence)
			{
				if (evidenceMultiPage.pageContent != null && evidenceMultiPage.pageContent.Count > 0)
				{
					CitySaveData.EvidenceStateSave evidenceStateSave = new CitySaveData.EvidenceStateSave();
					evidenceStateSave.id = evidenceMultiPage.evID;
					evidenceStateSave.page = evidenceMultiPage.page;
					evidenceStateSave.mpContent = new List<EvidenceMultiPage.MultiPageContent>(evidenceMultiPage.pageContent);
					this.currentData.multiPage.Add(evidenceStateSave);
				}
			}
			CityInfoData cityInfoData = new CityInfoData();
			cityInfoData.cityName = this.currentData.cityName;
			cityInfoData.shareCode = shareCode;
			cityInfoData.citySize = this.currentData.citySize;
			cityInfoData.population = this.currentData.population;
			cityInfoData.build = Game.Instance.buildID;
			Stopwatch stopWatch = null;
			if (Game.Instance.devMode)
			{
				stopWatch = new Stopwatch();
				stopWatch.Start();
			}
			string writeString = JsonUtility.ToJson(cityInfoData, true);
			Task writeCityInfoTask = Task.Run(delegate()
			{
				using (StreamWriter streamWriter = File.CreateText(cityInfoPath))
				{
					streamWriter.Write(writeString);
				}
			});
			while (!writeCityInfoTask.IsCompleted)
			{
				yield return null;
			}
			if (Game.Instance.useCityDataCompression)
			{
				string compressedCityPath = cityDataPath + "b";
				Task<bool> tempCompressionTask = DataCompressionController.Instance.CompressAndSaveDataAsync<CitySaveData>(this.currentData, compressedCityPath, Game.Instance.cityDataCompressionQuality);
				while (!tempCompressionTask.IsCompleted)
				{
					yield return null;
				}
				if (tempCompressionTask.Result)
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("CityGen: Successfully compressed and saved city data: " + compressedCityPath, 2);
					}
				}
				else
				{
					Game.LogError("Unable to compress and save city data! Using non compressed instead...", 2);
					string jsonString = JsonUtility.ToJson(this.currentData);
					Task tempTask = Task.Run(delegate()
					{
						using (StreamWriter streamWriter = File.CreateText(cityDataPath))
						{
							streamWriter.Write(jsonString);
						}
					});
					while (!tempTask.IsCompleted)
					{
						yield return null;
					}
					tempTask = null;
				}
				compressedCityPath = null;
				tempCompressionTask = null;
			}
			else
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("CityGen: Using JSON to write city data...", 2);
				}
				string jsonString = JsonUtility.ToJson(this.currentData);
				Task tempTask = Task.Run(delegate()
				{
					using (StreamWriter streamWriter = File.CreateText(cityDataPath))
					{
						streamWriter.Write(jsonString);
					}
				});
				while (!tempTask.IsCompleted)
				{
					yield return null;
				}
				tempTask = null;
			}
			if (Game.Instance.devMode && stopWatch != null)
			{
				stopWatch.Stop();
				if (Game.Instance.printDebug)
				{
					Game.Log("CityGen: City save data written in " + stopWatch.Elapsed.TotalSeconds.ToString(), 2);
				}
			}
			CityConstructor.Instance.loadingProgress = 1f;
			CityConstructor.Instance.stateComplete = true;
			yield break;
		}
		goto IL_1DB;
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x0005BC89 File Offset: 0x00059E89
	public bool IsUsingCityEditor()
	{
		return HighlanderSingleton<CityEditorController>.Instance != null && Game.Instance.enableCityEditor && RestartSafeController.Instance.generateNew && !RestartSafeController.Instance.loadSaveGame;
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x0005BCBF File Offset: 0x00059EBF
	public void Cancel()
	{
		base.enabled = false;
		this.generateNew = false;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x0005BCD0 File Offset: 0x00059ED0
	public void CreateSelfEmployed(CompanyPreset company, Human employee, Interactable workLocation)
	{
		Company company2 = new Company();
		company2.Setup(company, employee.home);
		company2.UpdateName();
		employee.SetJob(company2.companyRoster[0]);
		if (workLocation != null)
		{
			company2.placeOfBusiness = workLocation.node.gameLocation;
			company2.passedWorkPosition = workLocation;
			company2.passedWorkLocationID = workLocation.id;
			if (workLocation.furnitureParent != null)
			{
				workLocation.furnitureParent.AssignOwner(employee, true);
				if (Game.Instance.printDebug)
				{
					Game.Log(string.Concat(new string[]
					{
						"CityGen: Creating self employed ",
						company.name,
						", Assigning owner ",
						employee.GetCitizenName(),
						" at work location: ",
						workLocation.id.ToString(),
						" ",
						workLocation.GetWorldPosition(true).ToString()
					}), 2);
				}
			}
			employee.SetWorkFurniture(workLocation);
			return;
		}
		company2.placeOfBusiness = employee.home;
		if (Game.Instance.printDebug)
		{
			Game.Log("CityGen: Creating self employed " + company.name + " with no work location " + employee.GetCitizenName(), 2);
		}
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0005BE00 File Offset: 0x0005A000
	public Task LoadFullCityDataAsync()
	{
		CityConstructor.<LoadFullCityDataAsync>d__66 <LoadFullCityDataAsync>d__;
		<LoadFullCityDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<LoadFullCityDataAsync>d__.<>4__this = this;
		<LoadFullCityDataAsync>d__.<>1__state = -1;
		<LoadFullCityDataAsync>d__.<>t__builder.Start<CityConstructor.<LoadFullCityDataAsync>d__66>(ref <LoadFullCityDataAsync>d__);
		return <LoadFullCityDataAsync>d__.<>t__builder.Task;
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x0005BE44 File Offset: 0x0005A044
	public Task LoadSaveStateFile()
	{
		CityConstructor.<LoadSaveStateFile>d__67 <LoadSaveStateFile>d__;
		<LoadSaveStateFile>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<LoadSaveStateFile>d__.<>4__this = this;
		<LoadSaveStateFile>d__.<>1__state = -1;
		<LoadSaveStateFile>d__.<>t__builder.Start<CityConstructor.<LoadSaveStateFile>d__67>(ref <LoadSaveStateFile>d__);
		return <LoadSaveStateFile>d__.<>t__builder.Task;
	}

	// Token: 0x04000540 RID: 1344
	[NonSerialized]
	public CitySaveData currentData;

	// Token: 0x04000541 RID: 1345
	[NonSerialized]
	public StateSaveData saveState;

	// Token: 0x04000542 RID: 1346
	public bool generateNew = true;

	// Token: 0x04000543 RID: 1347
	public bool isLoaded;

	// Token: 0x04000544 RID: 1348
	public bool useCityConstructionHold;

	// Token: 0x04000545 RID: 1349
	public int saveChunk = 1;

	// Token: 0x04000546 RID: 1350
	public List<Evidence> evidenceToCompile = new List<Evidence>();

	// Token: 0x04000547 RID: 1351
	public CityConstructor.LoadState loadState;

	// Token: 0x04000548 RID: 1352
	public CityConstructor.LoadState cityConstructorHoldState;

	// Token: 0x04000549 RID: 1353
	private List<CityConstructor.LoadState> allLoadStates = new List<CityConstructor.LoadState>();

	// Token: 0x0400054A RID: 1354
	public int loadCursor = 9999;

	// Token: 0x0400054B RID: 1355
	public float loadingProgress;

	// Token: 0x0400054C RID: 1356
	public int displayedProgress;

	// Token: 0x0400054D RID: 1357
	public bool stateComplete;

	// Token: 0x0400054E RID: 1358
	public bool loadingOperationActive;

	// Token: 0x0400054F RID: 1359
	public bool preSimActive;

	// Token: 0x04000550 RID: 1360
	public bool preSimOccured;

	// Token: 0x04000551 RID: 1361
	public Dictionary<int, NewWall> loadingWallsReference = new Dictionary<int, NewWall>();

	// Token: 0x04000552 RID: 1362
	public Dictionary<int, FurnitureLocation> loadingFurnitureReference = new Dictionary<int, FurnitureLocation>();

	// Token: 0x04000553 RID: 1363
	public List<Interactable> updateSwitchState = new List<Interactable>();

	// Token: 0x04000554 RID: 1364
	private float timeStamp;

	// Token: 0x04000555 RID: 1365
	[NonSerialized]
	public CityConstructor.CollectedLoadTimeInfo debugLoadTime;

	// Token: 0x04000558 RID: 1368
	private static CityConstructor _instance;

	// Token: 0x04000559 RID: 1369
	private Task loadFullCityDataTask;

	// Token: 0x020000B8 RID: 184
	public enum LoadState
	{
		// Token: 0x0400055B RID: 1371
		parsingFile,
		// Token: 0x0400055C RID: 1372
		setupCityBoundary,
		// Token: 0x0400055D RID: 1373
		generateDistricts,
		// Token: 0x0400055E RID: 1374
		generateBlocks,
		// Token: 0x0400055F RID: 1375
		generateDensity,
		// Token: 0x04000560 RID: 1376
		generateBuildings,
		// Token: 0x04000561 RID: 1377
		generatePathfinding,
		// Token: 0x04000562 RID: 1378
		generateBlueprints,
		// Token: 0x04000563 RID: 1379
		generateCompanies,
		// Token: 0x04000564 RID: 1380
		connectRooms,
		// Token: 0x04000565 RID: 1381
		generateCitizens,
		// Token: 0x04000566 RID: 1382
		generateRelationships,
		// Token: 0x04000567 RID: 1383
		gatherData,
		// Token: 0x04000568 RID: 1384
		generateAirDucts,
		// Token: 0x04000569 RID: 1385
		generateEvidence,
		// Token: 0x0400056A RID: 1386
		generateInteriors,
		// Token: 0x0400056B RID: 1387
		prepareCitizens,
		// Token: 0x0400056C RID: 1388
		loadObjects,
		// Token: 0x0400056D RID: 1389
		finalizing,
		// Token: 0x0400056E RID: 1390
		savingData,
		// Token: 0x0400056F RID: 1391
		loadState,
		// Token: 0x04000570 RID: 1392
		preSim,
		// Token: 0x04000571 RID: 1393
		loadComplete
	}

	// Token: 0x020000B9 RID: 185
	[Serializable]
	public class CollectedLoadTimeInfo
	{
		// Token: 0x04000572 RID: 1394
		public string build;

		// Token: 0x04000573 RID: 1395
		public string citySize;

		// Token: 0x04000574 RID: 1396
		public bool generateNew;

		// Token: 0x04000575 RID: 1397
		public Dictionary<CityConstructor.LoadState, int> loadTimes = new Dictionary<CityConstructor.LoadState, int>();

		// Token: 0x04000576 RID: 1398
		public Dictionary<NewRoom, List<CityConstructor.DecorClusterGenerationTimeInfo>> decorTimes = new Dictionary<NewRoom, List<CityConstructor.DecorClusterGenerationTimeInfo>>();
	}

	// Token: 0x020000BA RID: 186
	[Serializable]
	public class DecorClusterGenerationTimeInfo
	{
		// Token: 0x04000577 RID: 1399
		public FurnitureCluster cluster;

		// Token: 0x04000578 RID: 1400
		public bool found;

		// Token: 0x04000579 RID: 1401
		public float time;
	}

	// Token: 0x020000BB RID: 187
	[Serializable]
	public class DecorTotalTime
	{
		// Token: 0x0400057A RID: 1402
		public NewRoom room;

		// Token: 0x0400057B RID: 1403
		public float totalTime;
	}

	// Token: 0x020000BC RID: 188
	// (Invoke) Token: 0x060005A7 RID: 1447
	public delegate void OnStartGame();

	// Token: 0x020000BD RID: 189
	// (Invoke) Token: 0x060005AB RID: 1451
	public delegate void LoadFinalize();
}
