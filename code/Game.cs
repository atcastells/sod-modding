using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

// Token: 0x020002CC RID: 716
public class Game : MonoBehaviour
{
	// Token: 0x0600100B RID: 4107 RVA: 0x000E5698 File Offset: 0x000E3898
	[Button(null, 0)]
	public void WordCount()
	{
		this.wordCountTotal = 0;
		char del = ' ';
		foreach (KeyValuePair<string, Dictionary<string, Strings.DisplayString>> keyValuePair in Strings.stringTable)
		{
			foreach (KeyValuePair<string, Strings.DisplayString> keyValuePair2 in keyValuePair.Value)
			{
				string[] array = Strings.CleanSplit(keyValuePair2.Value.displayStr, del, true, true);
				this.wordCountTotal += array.Length;
				array = Strings.CleanSplit(keyValuePair2.Value.alternateStr, del, true, true);
				this.wordCountTotal += array.Length;
			}
		}
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x000E5780 File Offset: 0x000E3980
	public void SetScreenshotMode(bool val, bool allowDialog = false)
	{
		this.screenshotMode = val;
		this.screenshotModeAllowDialog = allowDialog;
		foreach (RectTransform rectTransform in InterfaceControls.Instance.screenshotModeToggleObjects)
		{
			if (!(rectTransform == null) && (!this.screenshotMode || !this.screenshotModeAllowDialog || !InterfaceControls.Instance.screenShotModeAllowDialogObjects.Contains(rectTransform)))
			{
				rectTransform.gameObject.SetActive(!this.screenshotMode);
			}
		}
		if (!this.screenshotMode)
		{
			try
			{
				InteractionController.Instance.DisplayInteractionCursor(InteractionController.Instance.displayingInteraction, true);
			}
			catch
			{
			}
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x0600100D RID: 4109 RVA: 0x000E584C File Offset: 0x000E3A4C
	public static Game Instance
	{
		get
		{
			return Game._instance;
		}
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x000E5854 File Offset: 0x000E3A54
	private void Awake()
	{
		if (Game._instance != null && Game._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Game._instance = this;
		}
		this.mainThread = Thread.CurrentThread;
		Time.maximumDeltaTime = this.maxDeltaTime;
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x000E58A4 File Offset: 0x000E3AA4
	public void AddOnTimeEntry(Actor cc, float newOnTime)
	{
		this.guessCumulativeOnTime += newOnTime;
		this.guessDataEntries++;
		this.guessAverageOnTime = this.guessCumulativeOnTime / (float)this.guessDataEntries;
		if (newOnTime >= 0f)
		{
			this.guessEarlyEntries++;
		}
		else
		{
			this.guessLateEntries++;
		}
		this.guessEarlyPercent = (float)this.guessEarlyEntries / (float)this.guessDataEntries * 100f;
		this.guessLatePercent = (float)this.guessLateEntries / (float)this.guessDataEntries * 100f;
		this.boundaries = new Vector2(Mathf.Min(this.boundaries.x, newOnTime), Mathf.Max(this.boundaries.y, newOnTime));
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x000E596C File Offset: 0x000E3B6C
	public void AIInAddressFullyRested()
	{
		foreach (NewRoom newRoom in Player.Instance.currentGameLocation.rooms)
		{
			foreach (Actor actor in newRoom.currentOccupants)
			{
				((Human)actor).AddEnergy(1f);
			}
		}
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x000E5A08 File Offset: 0x000E3C08
	public void AIInAddressNeedShower()
	{
		foreach (NewRoom newRoom in Player.Instance.currentGameLocation.rooms)
		{
			foreach (Actor actor in newRoom.currentOccupants)
			{
				((Human)actor).AddHygiene(-1f);
			}
		}
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x000E5AA4 File Offset: 0x000E3CA4
	public void AIInAddressNeedFun()
	{
		foreach (NewRoom newRoom in Player.Instance.currentGameLocation.rooms)
		{
			foreach (Actor actor in newRoom.currentOccupants)
			{
				((Human)actor).AddExcitement(-1f);
			}
		}
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x000E5B40 File Offset: 0x000E3D40
	public void DebugButton()
	{
		foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
		{
			if (infoWindow.item != null)
			{
				foreach (Evidence.DataKey keyOne in Toolbox.Instance.allDataKeys)
				{
					infoWindow.passedEvidence.MergeDataKeys(keyOne, Evidence.DataKey.name);
				}
			}
		}
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x000E5BEC File Offset: 0x000E3DEC
	[Button(null, 0)]
	public void ResetRoutineCollectionData()
	{
		this.guessAverageOnTime = 0f;
		this.guessDataEntries = 0;
		this.guessEarlyPercent = 0f;
		this.guessLatePercent = 0f;
		this.guessCumulativeOnTime = 0f;
		this.guessEarlyEntries = 0;
		this.guessLateEntries = 0;
		this.boundaries = Vector2.zero;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x000E5C48 File Offset: 0x000E3E48
	[Button(null, 0)]
	public void AddRandomCitizenToAwareness()
	{
		Human human = CityData.Instance.citizenDirectory[Toolbox.Instance.Rand(0, CityData.Instance.citizenDirectory.Count, false)];
		if (human == null)
		{
			Game.Log("Random is null!", 2);
		}
		InterfaceController.Instance.AddAwarenessIcon(InterfaceController.AwarenessType.actor, InterfaceController.AwarenessBehaviour.alwaysVisible, human, null, Vector3.zero, InterfaceControls.Instance.spotted, 10, false, 20f);
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x000E5CBC File Offset: 0x000E3EBC
	public void ForceEnableMovement()
	{
		Player.Instance.pausedRememberPlayerMovement = true;
		Player.Instance.EnablePlayerMovement(true, true);
		Player.Instance.EnablePlayerMouseLook(true, false);
		Player.Instance.fps.m_StickToGroundForce = 7f;
		Player.Instance.fps.m_GravityMultiplier = 2f;
		Player.Instance.EnableCharacterController(true);
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x000E5D20 File Offset: 0x000E3F20
	public void SetRaindrops(bool val)
	{
		this.enableRaindrops = val;
		if (SessionData.Instance.startedGame)
		{
			foreach (StreetController streetController in CityData.Instance.streetDirectory)
			{
				foreach (KeyValuePair<MeshRenderer, StreetTilePreset.StreetSectionModel> keyValuePair in streetController.loadedModelReference)
				{
					if (this.enableRaindrops && keyValuePair.Value.rainMaterial != null)
					{
						keyValuePair.Key.sharedMaterial = SessionData.Instance.GetWeatherAffectedMaterial(keyValuePair.Value.rainMaterial, keyValuePair.Key);
					}
					else
					{
						keyValuePair.Key.sharedMaterial = keyValuePair.Value.normalMaterial;
					}
				}
			}
		}
		if (this.enableRaindrops)
		{
			SessionData.Instance.ExecuteWetnessChange();
		}
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x000E5E38 File Offset: 0x000E4038
	public void SetRainWindows(bool val)
	{
		if (val != this.enableRainyWindows)
		{
			this.enableRainyWindows = val;
			if (CityData.Instance != null)
			{
				foreach (NewRoom newRoom in CityData.Instance.roomDirectory)
				{
					foreach (NewNode newNode in newRoom.nodes)
					{
						foreach (NewWall newWall in newNode.walls)
						{
							foreach (NewWall.FrontageSetting frontageSetting in newWall.frontagePresets)
							{
								if (!(frontageSetting.mainTransform == null))
								{
									if (this.enableRainyWindows && (newWall.node.room.IsOutside() || newWall.otherWall.node.room.IsOutside()))
									{
										using (IEnumerator enumerator5 = frontageSetting.mainTransform.GetEnumerator())
										{
											while (enumerator5.MoveNext())
											{
												object obj = enumerator5.Current;
												Transform transform = (Transform)obj;
												if (transform.tag == "RainWindowGlass")
												{
													MeshRenderer component = transform.gameObject.GetComponent<MeshRenderer>();
													if (component != null)
													{
														component.sharedMaterial = frontageSetting.preset.rainyGlass;
													}
												}
											}
											continue;
										}
									}
									if (frontageSetting.preset.regularGlass != null)
									{
										foreach (object obj2 in frontageSetting.mainTransform)
										{
											Transform transform2 = (Transform)obj2;
											if (transform2.tag == "RainWindowGlass")
											{
												MeshRenderer component2 = transform2.gameObject.GetComponent<MeshRenderer>();
												if (component2 != null)
												{
													component2.sharedMaterial = frontageSetting.preset.regularGlass;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x000E6128 File Offset: 0x000E4328
	public void SetFOV(int val)
	{
		this.fov = val;
		if (CameraController.Instance != null && CameraController.Instance.cam != null)
		{
			CameraController.Instance.cam.fieldOfView = (float)this.fov;
		}
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x000E6168 File Offset: 0x000E4368
	public void SetObjectiveMarkers(bool val)
	{
		this.objectiveMarkers = val;
		foreach (Objective objective in InterfaceController.Instance.displayedObjectives)
		{
			if (objective.pointer != null)
			{
				objective.pointer.Remove();
			}
		}
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x000E61D8 File Offset: 0x000E43D8
	public void SetDirectionalArrow(bool val)
	{
		this.enableDirectionalArrow = val;
		if (this.enableDirectionalArrow)
		{
			MapController.Instance.directionalArrowContainer.SetActive(MapController.Instance.displayDirectionArrow);
			return;
		}
		MapController.Instance.directionalArrowContainer.SetActive(false);
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x000E6213 File Offset: 0x000E4413
	public void SetAwarenessIndicator(bool val)
	{
		InterfaceController.Instance.backgroundTransform.gameObject.SetActive(val);
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x000E622A File Offset: 0x000E442A
	public void SetDepthBlur(bool val)
	{
		this.depthBlur = val;
		InterfaceController.Instance.UpdateDOF();
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x000E623D File Offset: 0x000E443D
	public void SetSandboxStartTime(float val)
	{
		this.sandboxStartTime = val;
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x000E6246 File Offset: 0x000E4446
	public void SetGameDifficulty(int val)
	{
		this.gameDifficulty = val;
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x000E6250 File Offset: 0x000E4450
	public void SetGameLength(int val, bool updateSocialCredits, bool updateDropdown, bool updateSavedValue)
	{
		this.gameLength = val;
		Game.Log("CityGen: Set game length: " + val.ToString(), 2);
		if (updateSocialCredits)
		{
			BioScreenController.Instance.SetMaxSocialCreditLevels(this.gameLengthMaxLevels[this.gameLength]);
		}
		if (updateDropdown)
		{
			MainMenuController.Instance.gameLengthDropdown.dropdown.SetValueWithoutNotify(this.gameLength);
		}
		if (updateSavedValue)
		{
			MainMenuController.Instance.gameLengthDropdown.dropdown.value = this.gameLength;
		}
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x000E62D4 File Offset: 0x000E44D4
	public void SetEnableColdStatus(bool val)
	{
		this.coldStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x000E62F3 File Offset: 0x000E44F3
	public void SetEnableSmellyStatus(bool val)
	{
		this.smellyStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x000E6312 File Offset: 0x000E4512
	public void SetEnableHeadacheStatus(bool val)
	{
		this.headacheStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x000E6331 File Offset: 0x000E4531
	public void SetEnableBleedingStatus(bool val)
	{
		this.bleedingStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x000E6350 File Offset: 0x000E4550
	public void SetEnableInjuryStatus(bool val)
	{
		this.injuryStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x000E636F File Offset: 0x000E456F
	public void SetEnableHungerStatus(bool val)
	{
		this.hungerStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x000E638E File Offset: 0x000E458E
	public void SetEnableHydrationStatus(bool val)
	{
		this.hydrationStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x000E63AD File Offset: 0x000E45AD
	public void SetEnableWetStatus(bool val)
	{
		this.wetStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x000E63CC File Offset: 0x000E45CC
	public void SetEnableSickStatus(bool val)
	{
		this.sickStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x000E63EB File Offset: 0x000E45EB
	public void SetEnableNumbStatus(bool val)
	{
		this.numbStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x000E640A File Offset: 0x000E460A
	public void SetEnableTiredStatus(bool val)
	{
		this.tiredStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x000E6429 File Offset: 0x000E4629
	public void SetEnableDrunkStatus(bool val)
	{
		this.drunkStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x000E6448 File Offset: 0x000E4648
	public void SetEnableEnergizedStatus(bool val)
	{
		this.energizedStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x000E6467 File Offset: 0x000E4667
	public void SetEnableHydratedStatus(bool val)
	{
		this.hydratedStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x000E6486 File Offset: 0x000E4686
	public void SetEnableFocusedStatus(bool val)
	{
		this.focusedStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x000E64A5 File Offset: 0x000E46A5
	public void SetEnableWellRestedStatus(bool val)
	{
		this.wellRestedStatusEnabled = val;
		if (SessionData.Instance.startedGame)
		{
			StatusController.Instance.ForceStatusCheck();
		}
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x000E64C4 File Offset: 0x000E46C4
	public void SetSandboxStartingApartment(bool val)
	{
		this.sandboxStartingApartment = val;
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x000E64CD File Offset: 0x000E46CD
	public void SetSandboxStartingMoney(int val)
	{
		this.sandboxStartingMoney = val;
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x000E64D6 File Offset: 0x000E46D6
	public void SetSandboxStartingLockpicks(int val)
	{
		this.sandboxStartingLockpicks = val;
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000E64DF File Offset: 0x000E46DF
	public void SetForceSideJobDifficulty(bool val)
	{
		this.forceSideJobDifficulty = val;
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x000E64E8 File Offset: 0x000E46E8
	public void SetForcedSideJobDifficulty(int val)
	{
		this.forcedJobDifficulty = val;
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x000E64F1 File Offset: 0x000E46F1
	public void SetPauseAI(bool val)
	{
		this.pauseAI = val;
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x000E64FC File Offset: 0x000E46FC
	public void SetFreeCamMode(bool val)
	{
		this.freeCam = val;
		if (this.freeCam)
		{
			Player.Instance.EnableGhostMovement(true, false, 0f);
			Player.Instance.SetPlayerHeight(0.01f, false);
			Player.Instance.SetCameraHeight(0f);
			return;
		}
		Player.Instance.SetPlayerHeight(Player.Instance.GetPlayerHeightNormal(), true);
		Player.Instance.SetCameraHeight(GameplayControls.Instance.cameraHeightNormal);
		Player.Instance.EnableGhostMovement(false, true, 0f);
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x000E6583 File Offset: 0x000E4783
	public void SetFastForward(bool val)
	{
		this.fastForward = val;
		if (this.fastForward)
		{
			SessionData.Instance.SetTimeSpeed(SessionData.TimeSpeed.veryFast);
			return;
		}
		SessionData.Instance.SetTimeSpeed(SessionData.TimeSpeed.normal);
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x00002265 File Offset: 0x00000465
	public void SetDrawDistance(float val)
	{
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x000E65AC File Offset: 0x000E47AC
	public void SetLightDistance(float val)
	{
		this.lightFadeDistanceMultiplier = Mathf.Clamp(val, 0.4f, 2f);
		this.shadowFadeDistanceMultiplier = Mathf.Clamp(val, 0.4f, 2f);
		if (SessionData.Instance.startedGame)
		{
			LightController[] array = Object.FindObjectsOfType<LightController>(true);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateFadeDistances();
			}
		}
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x000E660E File Offset: 0x000E480E
	public void SetMurders(bool val)
	{
		this.enableMurdererInSandbox = val;
		if (MurderController.Instance != null && Game.Instance.sandboxMode && SessionData.Instance.startedGame)
		{
			MurderController.Instance.SetProcGenKillerLoop(this.enableMurdererInSandbox);
		}
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x00002265 File Offset: 0x00000465
	public void SetUIScale(int val)
	{
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x000E664C File Offset: 0x000E484C
	public void SetAAMode(int newMode)
	{
		HDAdditionalCameraData component = CameraController.Instance.cam.gameObject.GetComponent<HDAdditionalCameraData>();
		if (component != null)
		{
			string text = "Menu: Set AA Mode: ";
			HDAdditionalCameraData.AntialiasingMode antialiasingMode = newMode;
			Game.Log(text + antialiasingMode.ToString(), 2);
			component.antialiasing = newMode;
		}
		else
		{
			Game.LogError("Could not find HDAdditionalCameraData on camera!", 2);
		}
		this.aaMode = newMode;
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x000E66B4 File Offset: 0x000E48B4
	public void SetAAQuality(int newQuality)
	{
		HDAdditionalCameraData component = CameraController.Instance.cam.gameObject.GetComponent<HDAdditionalCameraData>();
		if (component != null)
		{
			string text = "Menu: Set AA Quality: ";
			HDAdditionalCameraData.SMAAQualityLevel smaaqualityLevel = newQuality;
			Game.Log(text + smaaqualityLevel.ToString(), 2);
			component.SMAAQuality = newQuality;
			component.TAAQuality = newQuality;
			return;
		}
		Game.LogError("Could not find HDAdditionalCameraData on camera!", 2);
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x000E6718 File Offset: 0x000E4918
	public void SetDithering(bool newVal)
	{
		HDAdditionalCameraData component = CameraController.Instance.cam.gameObject.GetComponent<HDAdditionalCameraData>();
		if (component != null)
		{
			Game.Log("Menu: Set Dithering: " + newVal.ToString(), 2);
			component.dithering = newVal;
			return;
		}
		Game.LogError("Could not find HDAdditionalCameraData on camera!", 2);
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x000E676D File Offset: 0x000E496D
	public void SetVsync(bool newVal)
	{
		this.vsync = newVal;
		if (this.vsync)
		{
			QualitySettings.vSyncCount = 1;
			return;
		}
		QualitySettings.vSyncCount = 0;
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x000E678B File Offset: 0x000E498B
	public void SetEnableFrameCap(bool newVal)
	{
		this.enableFrameCap = newVal;
		bool flag = this.enableFrameCap;
		this.SetFrameCap(this.frameCap);
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x000E67A7 File Offset: 0x000E49A7
	public void SetFrameCap(int newVal)
	{
		this.frameCap = newVal;
		if (this.enableFrameCap)
		{
			Application.targetFrameRate = this.frameCap;
			return;
		}
		Application.targetFrameRate = -1;
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x000E67CA File Offset: 0x000E49CA
	public void SetPasscodeOverrideToggle(bool newVal)
	{
		this.overridePasscodes = newVal;
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x000E67D3 File Offset: 0x000E49D3
	public void SetPasscodeOverride(int newPasscode)
	{
		this.overriddenPasscode = newPasscode;
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x000E67DC File Offset: 0x000E49DC
	public void SetFlickingLights(bool newVal)
	{
		this.flickeringLights = newVal;
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x000E67E5 File Offset: 0x000E49E5
	public static void Log(object print, int level = 2)
	{
		if (Game.Instance != null && Game.Instance.printDebug && Game.Instance.debugPrintLevel >= level)
		{
			Debug.Log(print);
		}
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x000E6813 File Offset: 0x000E4A13
	public static void LogError(object print, int level = 2)
	{
		if (Game.Instance.devMode && Game.Instance.printDebug && Game.Instance.debugPrintLevel >= level)
		{
			Debug.LogError(print);
			return;
		}
		if (Game.Instance.alwaysPrintErrors)
		{
			Debug.LogError(print);
		}
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x000E6854 File Offset: 0x000E4A54
	public void SetAllowLicensedMusic(bool val)
	{
		this.allowLicensedMusic = val;
		if (!this.allowLicensedMusic)
		{
			List<AudioController.LoopingSoundInfo> list = new List<AudioController.LoopingSoundInfo>();
			foreach (AudioController.LoopingSoundInfo loopingSoundInfo in AudioController.Instance.loopingSounds)
			{
				if (loopingSoundInfo != null && loopingSoundInfo.eventPreset != null && loopingSoundInfo.eventPreset.isLicensed)
				{
					list.Add(loopingSoundInfo);
				}
			}
			foreach (AudioController.LoopingSoundInfo loop in list)
			{
				AudioController.Instance.StopSound(loop, AudioController.StopType.immediate, "stop licensed music/sound");
			}
		}
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x000E6928 File Offset: 0x000E4B28
	[Button(null, 0)]
	public void Base26Test()
	{
		int i = this.base26Test + 1;
		LinkedList<int> linkedList = new LinkedList<int>();
		while (i > 26)
		{
			int num = i % 26;
			if (num == 0)
			{
				i = i / 26 - 1;
				linkedList.AddFirst(26);
			}
			else
			{
				i /= 26;
				linkedList.AddFirst(num);
			}
		}
		if (i >= 0)
		{
			linkedList.AddFirst(i);
		}
		Game.Log(new string(Enumerable.ToArray<char>(Enumerable.Select<int, char>(linkedList, (int s) => (char)(65 + s - 1)))), 2);
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x000E69B3 File Offset: 0x000E4BB3
	[Button(null, 0)]
	public void Give1000Crows()
	{
		GameplayController.Instance.AddMoney(1000, true, "cheat");
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x000E69CA File Offset: 0x000E4BCA
	[Button(null, 0)]
	public void Give100Lockpicks()
	{
		GameplayController.Instance.AddLockpicks(100, false);
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x000E69D9 File Offset: 0x000E4BD9
	[Button(null, 0)]
	public void ResetHealth()
	{
		Player.Instance.ResetHealthToMaximum();
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x000E69E5 File Offset: 0x000E4BE5
	[Button(null, 0)]
	public void KOPlayer()
	{
		Player.Instance.AddHealth(-99999f, false, false);
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x000E69F8 File Offset: 0x000E4BF8
	[Button(null, 0)]
	public void TestCurrentDetainedStatus()
	{
		StatusController.Instance.GetCurrentDetainedStatus();
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x000E6A08 File Offset: 0x000E4C08
	[Button(null, 0)]
	public void VictimsRankTest()
	{
		Game.Log(Mathf.InverseLerp((float)GameplayControls.Instance.worstCaseVictimCount, (float)GameplayControls.Instance.bestCaseVictimCount, 1f), 2);
		Game.Log(Mathf.InverseLerp((float)GameplayControls.Instance.worstCaseVictimCount, (float)GameplayControls.Instance.bestCaseVictimCount, 2f), 2);
		Game.Log(Mathf.InverseLerp((float)GameplayControls.Instance.worstCaseVictimCount, (float)GameplayControls.Instance.bestCaseVictimCount, 3f), 2);
		Game.Log(Mathf.InverseLerp((float)GameplayControls.Instance.worstCaseVictimCount, (float)GameplayControls.Instance.bestCaseVictimCount, 4f), 2);
		Game.Log(Mathf.InverseLerp((float)GameplayControls.Instance.worstCaseVictimCount, (float)GameplayControls.Instance.bestCaseVictimCount, 5f), 2);
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x000E6AEC File Offset: 0x000E4CEC
	[Button(null, 0)]
	public void GiveAllUpgrades()
	{
		foreach (SyncDiskPreset syncDiskPreset in Toolbox.Instance.allSyncDisks)
		{
			if (syncDiskPreset.interactable != null)
			{
				Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(syncDiskPreset.interactable, Player.Instance, null, null, Player.Instance.transform.position + new Vector3(Toolbox.Instance.Rand(-1f, 1f, false), 0f, Toolbox.Instance.Rand(-1f, 1f, false)), Vector3.zero, null, syncDiskPreset, "");
				if (interactable != null)
				{
					interactable.ForcePhysicsActive(false, false, default(Vector3), 2, false);
				}
			}
		}
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x000E6BD8 File Offset: 0x000E4DD8
	[Button(null, 0)]
	public void GiveSocialCredit()
	{
		GameplayController.Instance.AddSocialCredit(GameplayControls.Instance.socialCreditForSideJobs, true, "cheat");
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x000E6BF4 File Offset: 0x000E4DF4
	[Button(null, 0)]
	public void CompleteSideJob()
	{
		if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.job != null)
		{
			CasePanelController.Instance.activeCase.job.Complete();
		}
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x000E6C27 File Offset: 0x000E4E27
	[Button(null, 0)]
	public void DisplayAnswersToCurrentSideJob()
	{
		if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.job != null)
		{
			CasePanelController.Instance.activeCase.job.DebugDisplayAnswers();
		}
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x000E6C5C File Offset: 0x000E4E5C
	public void OnBuildValueChanged()
	{
		Debug.Log("Build value changed");
		this.buildDescription = this.buildID;
		if (this.customTags != null && this.customTags.Length > 0)
		{
			foreach (string text in this.customTags.Split(' ', 0))
			{
				this.buildDescription = this.buildDescription + " [" + text + "]";
			}
		}
		if (this.devMode)
		{
			this.buildDescription += " [Dev]";
		}
		if (this.printDebug)
		{
			this.buildDescription += " [Debug]";
		}
		if (this.collectDebugData)
		{
			this.buildDescription += " [CollData]";
		}
		if (this.enableBugReporting)
		{
			this.buildDescription += " [BugRep]";
		}
		if (this.enableFeedbackFormLink)
		{
			this.buildDescription += " [Feedback]";
		}
		if (this.timeLimited)
		{
			this.buildDescription = this.buildDescription + " [TimeLimit: " + this.timeLimit.ToString() + "]";
		}
		if (this.disableSaveLoadGames)
		{
			this.buildDescription += " [NoLoadSave]";
		}
		if (this.disableSandbox)
		{
			this.buildDescription += " [NoSandbox]";
		}
		if (this.disableCityGeneration)
		{
			this.buildDescription += " [NoGeneration]";
		}
		if (this.smallCitiesOnly)
		{
			this.buildDescription += " [SmallCities]";
		}
		if (this.displayBetaMessage)
		{
			this.buildDescription += " [BetaMsg]";
		}
		if (this.forceEnglish)
		{
			this.buildDescription += " [ForceENG]";
		}
		if (this.allowMods)
		{
			this.buildDescription += " [Mods]";
		}
		if (this.forceConsoleBuildFlag)
		{
			this.buildDescription += " [ForceConsole]";
		}
		if (this.steamScriptPath != null && this.steamScriptPath.Length > 0)
		{
			if (File.Exists(this.steamScriptPath))
			{
				string[] array2 = File.ReadAllLines(this.steamScriptPath);
				array2[3] = "      \"Desc\" \"" + this.buildDescription + "\" // internal description for this build";
				File.WriteAllLines(this.steamScriptPath, array2);
				return;
			}
			Debug.Log("No file exists at " + this.steamScriptPath);
		}
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x000E6EF0 File Offset: 0x000E50F0
	[Button(null, 0)]
	public void DebugFindWall()
	{
		foreach (NewGameLocation newGameLocation in CityData.Instance.gameLocationDirectory)
		{
			foreach (NewRoom newRoom in newGameLocation.rooms)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					foreach (NewWall newWall in newNode.walls)
					{
						if (newWall.id == this.debugFindWall)
						{
							string[] array = new string[8];
							array[0] = "Found wall ";
							array[1] = this.debugFindWall.ToString();
							array[2] = " at ";
							int num = 3;
							Vector3 vector = newWall.position;
							array[num] = vector.ToString();
							array[4] = " euler: ";
							int num2 = 5;
							vector = newWall.localEulerAngles;
							array[num2] = vector.ToString();
							array[6] = " door ";
							int num3 = 7;
							NewDoor door = newWall.door;
							array[num3] = ((door != null) ? door.ToString() : null);
							Game.Log(string.Concat(array), 2);
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x000E70D0 File Offset: 0x000E52D0
	[Button(null, 0)]
	public void DebugAddressID()
	{
		NewAddress newAddress = CityData.Instance.addressDirectory.Find((NewAddress item) => item.id == this.debugAddressID);
		if (newAddress != null)
		{
			Game.Log(string.Concat(new string[]
			{
				newAddress.name,
				" at ",
				newAddress.floor.floor.ToString(),
				" in ",
				newAddress.building.name
			}), 2);
		}
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x000E7150 File Offset: 0x000E5350
	[Button(null, 0)]
	public void DebugCitizenID()
	{
		Human human = CityData.Instance.citizenDirectory.Find((Citizen item) => item.humanID == this.debugCitizenID);
		if (human != null)
		{
			Game.Log(human.GetCitizenName(), 2);
		}
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x000E7190 File Offset: 0x000E5390
	[Button(null, 0)]
	public void ShotgunTest()
	{
		for (int i = 0; i < 12; i++)
		{
			Toolbox.Instance.Shoot(Player.Instance, CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.forward, 16f, 0.1f, 0.1f, GameplayControls.Instance.sentryGunWeapon, false, CameraController.Instance.cam.transform.position, false);
		}
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x000E7210 File Offset: 0x000E5410
	[Button(null, 0)]
	public void MissionPhotoTest()
	{
		Interactable interactable = null;
		if (this.debugPhotoTestID != 0)
		{
			interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.debugPhotoTestID);
		}
		if (interactable == null)
		{
			List<Interactable> list = CityData.Instance.interactableDirectory.FindAll((Interactable item) => item.inInventory == null && item.preset.spawnable && item.preset.prefab != null && item.node != null && item.node.gameLocation.thisAsStreet != null);
			interactable = list[Toolbox.Instance.Rand(0, list.Count, false)];
		}
		Game.Log(string.Concat(new string[]
		{
			"Player: Chosen to capture item ",
			interactable.GetName(),
			" ",
			interactable.id.ToString(),
			" at ",
			interactable.GetWorldPosition(true).ToString()
		}), 2);
		Interactable localizedSnapshot = Toolbox.Instance.GetLocalizedSnapshot(interactable);
		Player.Instance.Teleport(interactable.node, null, true, false);
		if (localizedSnapshot != null)
		{
			ActionController.Instance.Inspect(localizedSnapshot, Player.Instance.currentNode, Player.Instance);
			return;
		}
		Game.LogError("Unable to generate hidden item photo", 2);
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x000E7330 File Offset: 0x000E5530
	[Button(null, 0)]
	public void FindProsthetics()
	{
		foreach (Citizen citizen in CityData.Instance.citizenDirectory)
		{
			foreach (Human.Trait trait in citizen.characterTraits)
			{
				if (trait.trait.name == "Affliction-LostLeftArm" || trait.trait.name == "Affliction-LostLeftArmWhole" || trait.trait.name == "Affliction-LostRightArm" || trait.trait.name == "Affliction-LostRightArmWhole" || trait.trait.name == "Affliction-LostRightLeg" || trait.trait.name == "Affliction-LostLeftLeg")
				{
					Game.Log(string.Concat(new string[]
					{
						citizen.citizenName,
						" (homeless: ",
						citizen.isHomeless.ToString(),
						", job: ",
						citizen.job.preset.name,
						")"
					}), 2);
				}
			}
		}
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x000E74BC File Offset: 0x000E56BC
	[Button(null, 0)]
	public void TeleportPlayerStreetStart()
	{
		List<StreetController> list = CityData.Instance.streetDirectory.FindAll((StreetController item) => item.entrances.Count > 0 && item.rooms.Count > 0 && item.nodes.Count >= 8);
		SessionData.Instance.startingNode = Player.Instance.FindSafeTeleport(list[Toolbox.Instance.Rand(0, list.Count, false)], false, true);
		Player.Instance.Teleport(SessionData.Instance.startingNode, null, false, false);
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x000E7540 File Offset: 0x000E5740
	[Button(null, 0)]
	public void ToggleCitizenColliders()
	{
		foreach (Citizen citizen in CityData.Instance.citizenDirectory)
		{
			if (citizen.ai != null && citizen.ai.capCollider != null)
			{
				citizen.ai.capCollider.enabled = !citizen.ai.capCollider.enabled;
			}
		}
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x000E75D4 File Offset: 0x000E57D4
	[Button(null, 0)]
	public void GiveRandomJolt()
	{
		Player.Instance.fps.JoltCamera(new Vector3(Toolbox.Instance.Rand(-1f, 1f, false), Toolbox.Instance.Rand(-1f, 1f, false), Toolbox.Instance.Rand(-1f, 1f, false)), 45f, 1f);
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x000E763E File Offset: 0x000E583E
	[Button(null, 0)]
	public void TripPlayer()
	{
		Player.Instance.Trip(0f, false, true);
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x000E7654 File Offset: 0x000E5854
	[Button(null, 0)]
	public void TestTimeRange()
	{
		Vector2 vector = Toolbox.Instance.CreateTimeRange(SessionData.Instance.gameTime, GameplayControls.Instance.timeOfDeathAccuracy, false, true, 15);
		Game.Log(string.Concat(new string[]
		{
			SessionData.Instance.DecimalToClockString(vector.x, false),
			" - ",
			SessionData.Instance.DecimalToClockString(vector.y, false),
			" (",
			SessionData.Instance.DecimalToClockString(SessionData.Instance.gameTime, false),
			")"
		}), 2);
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x000E76F0 File Offset: 0x000E58F0
	[Button(null, 0)]
	public void DeletePlayerPrefs()
	{
		PopupMessageController.Instance.PopupMessage("DeletePrefs", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnRightButton += this.DeletePlayerPrefsConfirm;
		PopupMessageController.Instance.OnLeftButton += this.DeletePlayerPrefsCancel;
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x000E776C File Offset: 0x000E596C
	public void DeletePlayerPrefsConfirm()
	{
		PopupMessageController.Instance.OnRightButton -= this.DeletePlayerPrefsConfirm;
		PopupMessageController.Instance.OnLeftButton -= this.DeletePlayerPrefsCancel;
		PlayerPrefs.DeleteAll();
		AudioController.Instance.StopAllSounds();
		SceneManager.LoadScene("Main");
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x000E77BE File Offset: 0x000E59BE
	public void DeletePlayerPrefsCancel()
	{
		PopupMessageController.Instance.OnRightButton -= this.DeletePlayerPrefsConfirm;
		PopupMessageController.Instance.OnLeftButton -= this.DeletePlayerPrefsCancel;
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x000E77EC File Offset: 0x000E59EC
	[Button(null, 0)]
	public void ForcePlayerDirtyDeath()
	{
		Player.Instance.currentHealth = 0f;
		Player.Instance.TriggerPlayerKO(base.transform.forward, 0f, true);
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x000E7818 File Offset: 0x000E5A18
	[Button(null, 0)]
	public void TurnOffAllDynamicOcclusion()
	{
		MeshRenderer[] array = Object.FindObjectsOfType<MeshRenderer>(true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].allowOcclusionWhenDynamic = false;
		}
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x000E7844 File Offset: 0x000E5A44
	[Button(null, 0)]
	public void TurnOffAllLODS()
	{
		LODGroup[] array = Object.FindObjectsOfType<LODGroup>(true);
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x000E7870 File Offset: 0x000E5A70
	[Button(null, 0)]
	public void ShootFromPlayer()
	{
		Toolbox.Instance.Shoot(Player.Instance, CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.TransformPoint(Vector3.forward), 16f, 0.1f, 0f, Game.Instance.debugTestWeapon, true, CameraController.Instance.cam.transform.position, false);
	}

	// Token: 0x0400138E RID: 5006
	[Tooltip("Build version")]
	[Header("Build")]
	[OnValueChanged("OnBuildValueChanged")]
	public string buildID = "20";

	// Token: 0x0400138F RID: 5007
	[OnValueChanged("OnBuildValueChanged")]
	public bool displayBuildInMenu = true;

	// Token: 0x04001390 RID: 5008
	[Space(7f)]
	public string buildDescription = string.Empty;

	// Token: 0x04001391 RID: 5009
	public string customTags = string.Empty;

	// Token: 0x04001392 RID: 5010
	public string steamScriptPath;

	// Token: 0x04001393 RID: 5011
	[OnValueChanged("OnBuildValueChanged")]
	public bool updateAbove;

	// Token: 0x04001394 RID: 5012
	public string lastCompatibleCities = "30.00";

	// Token: 0x04001395 RID: 5013
	[OnValueChanged("OnBuildValueChanged")]
	[Tooltip("Enable dev options on main menu & collect debugging info on a range of gameobjects")]
	[Header("Build Settings")]
	public bool devMode;

	// Token: 0x04001396 RID: 5014
	[Tooltip("Print to console")]
	[OnValueChanged("OnBuildValueChanged")]
	public bool printDebug = true;

	// Token: 0x04001397 RID: 5015
	[DisableIf("printDebug")]
	[Tooltip("Allow errors to be printed even if the above is false")]
	public bool alwaysPrintErrors = true;

	// Token: 0x04001398 RID: 5016
	[Tooltip("Collect debug data")]
	[OnValueChanged("OnBuildValueChanged")]
	public bool collectDebugData;

	// Token: 0x04001399 RID: 5017
	[Range(0f, 2f)]
	[EnableIf("printDebug")]
	[Tooltip("Set the debug prints level: 0 is nothing, 2 is maximum")]
	public int debugPrintLevel = 2;

	// Token: 0x0400139A RID: 5018
	[Tooltip("Enable bug reporting")]
	[OnValueChanged("OnBuildValueChanged")]
	public bool enableBugReporting;

	// Token: 0x0400139B RID: 5019
	[Tooltip("Enable feedback form link button")]
	[OnValueChanged("OnBuildValueChanged")]
	public bool enableFeedbackFormLink;

	// Token: 0x0400139C RID: 5020
	[OnValueChanged("OnBuildValueChanged")]
	[Tooltip("Force the game to be english")]
	public bool forceEnglish;

	// Token: 0x0400139D RID: 5021
	[OnValueChanged("OnBuildValueChanged")]
	[Tooltip("Skip intro")]
	public bool skipIntro;

	// Token: 0x0400139E RID: 5022
	[Tooltip("Allows mod button in the main menu")]
	[OnValueChanged("OnBuildValueChanged")]
	public bool allowMods = true;

	// Token: 0x0400139F RID: 5023
	[Tooltip("Mimic/force this to behave like a console build")]
	[OnValueChanged("OnBuildValueChanged")]
	public bool forceConsoleBuildFlag;

	// Token: 0x040013A0 RID: 5024
	[Tooltip("If true and in dev mode and collect debug data, will ensure dictionaries have names for all items")]
	public bool ensureItemNamesInDictionaries;

	// Token: 0x040013A1 RID: 5025
	[Header("Console Limitations")]
	public List<ButtonController> removeButtonsOnConsoleVersion = new List<ButtonController>();

	// Token: 0x040013A2 RID: 5026
	[OnValueChanged("OnBuildValueChanged")]
	[Header("Demo Limitations")]
	[Tooltip("Is this a demo build? Limit time")]
	public bool timeLimited;

	// Token: 0x040013A3 RID: 5027
	[Tooltip("Limit time to this in minutes")]
	[OnValueChanged("OnBuildValueChanged")]
	public float timeLimit = 60f;

	// Token: 0x040013A4 RID: 5028
	[Tooltip("Start the timer after exit of apartment in story mode")]
	[OnValueChanged("OnBuildValueChanged")]
	public bool startTimerAfterApartmentExit = true;

	// Token: 0x040013A5 RID: 5029
	[OnValueChanged("OnBuildValueChanged")]
	[Tooltip("Pause the timer on caseboard or menu")]
	public bool pauseTimerOnGamePause;

	// Token: 0x040013A6 RID: 5030
	[Tooltip("Disable save games")]
	[OnValueChanged("OnBuildValueChanged")]
	public bool disableSaveLoadGames;

	// Token: 0x040013A7 RID: 5031
	[OnValueChanged("OnBuildValueChanged")]
	[Tooltip("Disable sandbox mode")]
	public bool disableSandbox;

	// Token: 0x040013A8 RID: 5032
	[OnValueChanged("OnBuildValueChanged")]
	[Tooltip("Disable city generation")]
	public bool disableCityGeneration;

	// Token: 0x040013A9 RID: 5033
	[OnValueChanged("OnBuildValueChanged")]
	public bool smallCitiesOnly;

	// Token: 0x040013AA RID: 5034
	[OnValueChanged("OnBuildValueChanged")]
	public bool displayBetaMessage;

	// Token: 0x040013AB RID: 5035
	[Tooltip("Use compression to store save games")]
	[Header("Game")]
	public bool useSaveGameCompression;

	// Token: 0x040013AC RID: 5036
	[Range(0f, 10f)]
	[EnableIf("useSaveGameCompression")]
	public int saveGameCompressionQuality = 9;

	// Token: 0x040013AD RID: 5037
	[Tooltip("Use compression to store generated city data")]
	public bool useCityDataCompression;

	// Token: 0x040013AE RID: 5038
	[EnableIf("useCityDataCompression")]
	[Range(0f, 10f)]
	public int cityDataCompressionQuality = 9;

	// Token: 0x040013AF RID: 5039
	[Tooltip("How many threads to spawn on certain loading/generation routines")]
	public int maxThreads = 16;

	// Token: 0x040013B0 RID: 5040
	[Tooltip("Automatically write unfound text entries to .csv files")]
	public bool writeUnfoundToTextFiles = true;

	// Token: 0x040013B1 RID: 5041
	[Tooltip("Start the game without loading a story chapter")]
	public bool sandboxMode;

	// Token: 0x040013B2 RID: 5042
	[DisableIf("sandboxMode")]
	[Tooltip("Load this chapter")]
	public int loadChapter;

	// Token: 0x040013B3 RID: 5043
	[Tooltip("If true, the player movement will update in the regular Update() loop, if false it will update in FixedUpdate()")]
	public bool updateMovementEveryFrame;

	// Token: 0x040013B4 RID: 5044
	[Tooltip("Default amount of saturation: Needed as we modify it with loosing health etc")]
	public float defaultSaturationAmount = 10f;

	// Token: 0x040013B5 RID: 5045
	[Tooltip("If true this will display control tips")]
	public bool displayExtraControlHints = true;

	// Token: 0x040013B6 RID: 5046
	[Tooltip("If true this will display objective markers")]
	public bool objectiveMarkers = true;

	// Token: 0x040013B7 RID: 5047
	[Tooltip("General game difficulty")]
	[Range(0f, 3f)]
	public int gameDifficulty = 1;

	// Token: 0x040013B8 RID: 5048
	public List<float> difficultyIncomingDamageMultipliers = new List<float>();

	// Token: 0x040013B9 RID: 5049
	[Tooltip("Game length")]
	[Range(0f, 4f)]
	public int gameLength = 2;

	// Token: 0x040013BA RID: 5050
	public List<int> gameLengthMaxLevels = new List<int>();

	// Token: 0x040013BB RID: 5051
	[Tooltip("Force side missions to be a certain difficulty")]
	public bool forceSideJobDifficulty;

	// Token: 0x040013BC RID: 5052
	[Range(0f, 6f)]
	public int forcedJobDifficulty = 2;

	// Token: 0x040013BD RID: 5053
	[Tooltip("Resume the game after pinning evidence item")]
	public bool resumeAfterPin;

	// Token: 0x040013BE RID: 5054
	[Tooltip("Close windows with world interaction status on resume game")]
	public bool closeInteractionsOnResume = true;

	// Token: 0x040013BF RID: 5055
	[Tooltip("Enables the directional arrow")]
	public bool enableDirectionalArrow = true;

	// Token: 0x040013C0 RID: 5056
	public float sandboxStartTime = 9f;

	// Token: 0x040013C1 RID: 5057
	[Tooltip("Enables the murderer in sandbox mode")]
	public bool enableMurdererInSandbox = true;

	// Token: 0x040013C2 RID: 5058
	public float weatherChangeFrequency = 0.5f;

	// Token: 0x040013C3 RID: 5059
	[Tooltip("Enable room loading thresholds: When a certain number of rooms are loaded, unload uneeded ones down to a certain cached number")]
	public bool roomCacheLimit = true;

	// Token: 0x040013C4 RID: 5060
	public bool disableSnow;

	// Token: 0x040013C5 RID: 5061
	public bool disableTrespass;

	// Token: 0x040013C6 RID: 5062
	public bool debugMurdererOnStart;

	// Token: 0x040013C7 RID: 5063
	[Tooltip("Starts the game outside the door of the apartment in story mode")]
	public bool demoChapterSkip;

	// Token: 0x040013C8 RID: 5064
	public bool demoMode;

	// Token: 0x040013C9 RID: 5065
	[Tooltip("Gives access to all hospital doors")]
	public bool allHospitalAccess;

	// Token: 0x040013CA RID: 5066
	[Tooltip("Auto pause the game after x seconds of no input")]
	public bool autoPause;

	// Token: 0x040013CB RID: 5067
	[EnableIf("autoPause")]
	public int autoPauseSeconds = 90;

	// Token: 0x040013CC RID: 5068
	[Tooltip("Ask to restart the game from this save game if left for x seconds of no input")]
	public bool demoAutoReset;

	// Token: 0x040013CD RID: 5069
	[EnableIf("demoAutoReset")]
	public int resetSeconds = 90;

	// Token: 0x040013CE RID: 5070
	[EnableIf("demoAutoReset")]
	public int resetChapterPart = 23;

	// Token: 0x040013CF RID: 5071
	[EnableIf("demoAutoReset")]
	public string resetSaveGameName;

	// Token: 0x040013D0 RID: 5072
	public float textSpeed = 1f;

	// Token: 0x040013D1 RID: 5073
	[Tooltip("Disables closing of case board: Used sometimes by the tutorial")]
	[ReadOnly]
	public bool disableCaseBoardClose;

	// Token: 0x040013D2 RID: 5074
	[Tooltip("Allows/disables licensed music")]
	public bool allowLicensedMusic = true;

	// Token: 0x040013D3 RID: 5075
	[Tooltip("Override all passcodes")]
	public bool overridePasscodes;

	// Token: 0x040013D4 RID: 5076
	public int overriddenPasscode = 1000;

	// Token: 0x040013D5 RID: 5077
	[Tooltip("Max delta time")]
	public float maxDeltaTime = 1.05f;

	// Token: 0x040013D6 RID: 5078
	public int aaMode;

	// Token: 0x040013D7 RID: 5079
	[NonSerialized]
	public Thread mainThread;

	// Token: 0x040013D8 RID: 5080
	[Header("Statuses")]
	public bool coldStatusEnabled = true;

	// Token: 0x040013D9 RID: 5081
	public bool smellyStatusEnabled = true;

	// Token: 0x040013DA RID: 5082
	public bool headacheStatusEnabled = true;

	// Token: 0x040013DB RID: 5083
	public bool injuryStatusEnabled = true;

	// Token: 0x040013DC RID: 5084
	public bool tiredStatusEnabled = true;

	// Token: 0x040013DD RID: 5085
	public bool hungerStatusEnabled = true;

	// Token: 0x040013DE RID: 5086
	public bool hydrationStatusEnabled = true;

	// Token: 0x040013DF RID: 5087
	public bool numbStatusEnabled = true;

	// Token: 0x040013E0 RID: 5088
	public bool bleedingStatusEnabled = true;

	// Token: 0x040013E1 RID: 5089
	public bool wetStatusEnabled = true;

	// Token: 0x040013E2 RID: 5090
	public bool sickStatusEnabled = true;

	// Token: 0x040013E3 RID: 5091
	public bool drunkStatusEnabled = true;

	// Token: 0x040013E4 RID: 5092
	public bool starchAddictionEnabled = true;

	// Token: 0x040013E5 RID: 5093
	public bool poisonStatusEnabled = true;

	// Token: 0x040013E6 RID: 5094
	public bool blindedStatusEnabled = true;

	// Token: 0x040013E7 RID: 5095
	[Space(7f)]
	public bool energizedStatusEnabled = true;

	// Token: 0x040013E8 RID: 5096
	public bool hydratedStatusEnabled = true;

	// Token: 0x040013E9 RID: 5097
	public bool focusedStatusEnabled = true;

	// Token: 0x040013EA RID: 5098
	public bool wellRestedStatusEnabled = true;

	// Token: 0x040013EB RID: 5099
	[Header("Controls")]
	[Tooltip("Enables mouse/keyboard control by default on startup; disable for console builds")]
	public Vector2 mouseSensitivity = new Vector2(20f, 20f);

	// Token: 0x040013EC RID: 5100
	public Vector2 controllerSensitivity = new Vector2(20f, 20f);

	// Token: 0x040013ED RID: 5101
	public Vector2 axisMP = new Vector2(1f, 1f);

	// Token: 0x040013EE RID: 5102
	public bool controlAutoSwitch = true;

	// Token: 0x040013EF RID: 5103
	public int mouseSmoothing = 10;

	// Token: 0x040013F0 RID: 5104
	public int controllerSmoothing = 40;

	// Token: 0x040013F1 RID: 5105
	public float movementSpeed = 1f;

	// Token: 0x040013F2 RID: 5106
	public int scrollSensitivity = 50;

	// Token: 0x040013F3 RID: 5107
	[Header("Player")]
	public string playerFirstName = "Fred";

	// Token: 0x040013F4 RID: 5108
	public string playerSurname = "Melrose";

	// Token: 0x040013F5 RID: 5109
	public Human.Gender playerGender;

	// Token: 0x040013F6 RID: 5110
	public Human.Gender partnerGender;

	// Token: 0x040013F7 RID: 5111
	public Color playerSkinColour;

	// Token: 0x040013F8 RID: 5112
	public int playerBirthDay = 19;

	// Token: 0x040013F9 RID: 5113
	public int playerBirthMonth = 12;

	// Token: 0x040013FA RID: 5114
	public int playerBirthYear = 1947;

	// Token: 0x040013FB RID: 5115
	[Tooltip("The game's language reference string")]
	[Header("Words")]
	public string language = "English";

	// Token: 0x040013FC RID: 5116
	public int wordCountTotal;

	// Token: 0x040013FD RID: 5117
	[Header("Streets")]
	public bool displayStreetChunks;

	// Token: 0x040013FE RID: 5118
	public bool displayStreetAndJunctionChunks;

	// Token: 0x040013FF RID: 5119
	[EnableIf("displayStreetAndJunctionChunks")]
	public bool displayTrafficSimulationResults;

	// Token: 0x04001400 RID: 5120
	public Material debugtrafficSimMaterial;

	// Token: 0x04001401 RID: 5121
	[DisableIf("displayStreetAndJunctionChunks")]
	public bool displayStreets;

	// Token: 0x04001402 RID: 5122
	public Material debugStreetMaterial;

	// Token: 0x04001403 RID: 5123
	public List<Color> streetDebugColours = new List<Color>();

	// Token: 0x04001404 RID: 5124
	[Header("City")]
	[Tooltip("Display roads")]
	public bool debugDisplayRoads;

	// Token: 0x04001405 RID: 5125
	[Tooltip("Player can open any door")]
	public bool keysToTheCity = true;

	// Token: 0x04001406 RID: 5126
	[Tooltip("Disable generation of furniture")]
	public bool disableFurniture;

	// Token: 0x04001407 RID: 5127
	[Tooltip("Destroy this when the game is started")]
	public Transform debugContainer;

	// Token: 0x04001408 RID: 5128
	[Tooltip("Enable culling debug for new cull system")]
	public bool enableCullingDebug;

	// Token: 0x04001409 RID: 5129
	[Tooltip("When generating a new city, use the city editor")]
	public bool enableCityEditor;

	// Token: 0x0400140A RID: 5130
	[Tooltip("Statistic for keeping track of how early/late citizens are on average compared with due time. Records guess accuracy.")]
	[Header("Citizens")]
	public bool collectRoutineTimingInfo = true;

	// Token: 0x0400140B RID: 5131
	public float guessAverageOnTime;

	// Token: 0x0400140C RID: 5132
	public int guessDataEntries;

	// Token: 0x0400140D RID: 5133
	public float guessEarlyPercent;

	// Token: 0x0400140E RID: 5134
	public float guessLatePercent;

	// Token: 0x0400140F RID: 5135
	[NonSerialized]
	public float guessCumulativeOnTime;

	// Token: 0x04001410 RID: 5136
	[NonSerialized]
	public int guessEarlyEntries;

	// Token: 0x04001411 RID: 5137
	[NonSerialized]
	public int guessLateEntries;

	// Token: 0x04001412 RID: 5138
	public Vector2 boundaries = Vector2.zero;

	// Token: 0x04001413 RID: 5139
	[Tooltip("If enabled, citizens won't react to getting attacked by the player")]
	public bool noReactOnAttack;

	// Token: 0x04001414 RID: 5140
	[Header("Pathfinding")]
	[Tooltip("If pathfinding fails, load objects to present the closed set for debugging")]
	public bool debugPathfinding;

	// Token: 0x04001415 RID: 5141
	[Tooltip("Use the job system for pathfinding")]
	public bool useJobSystem = true;

	// Token: 0x04001416 RID: 5142
	[Tooltip("Cache external routes for the pathfinding system")]
	public bool useExternalRouteCaching = true;

	// Token: 0x04001417 RID: 5143
	[Tooltip("Cache internal routes for the pathfinding system")]
	public bool useInternalRouteCaching = true;

	// Token: 0x04001418 RID: 5144
	[Tooltip("Force street pathing to be run on the main thread, this appears to be faster due to the data passed to the worker?")]
	public bool forceStreetPathsOnMainThread = true;

	// Token: 0x04001419 RID: 5145
	[Tooltip("If true the maximum cached path numbers will be ignored. The game could run out of memory eventually, so be careful!")]
	public bool unlimitedPathCaching;

	// Token: 0x0400141A RID: 5146
	[Tooltip("The maximum number of cached external paths")]
	public int maxExternalCachedPaths = 1000;

	// Token: 0x0400141B RID: 5147
	[Tooltip("The maximum number of cached paths in an internal address")]
	public int maxInternalCachedPaths = 12;

	// Token: 0x0400141C RID: 5148
	[Tooltip("The maximum number of cached street paths")]
	public int maxStreetCachedPaths = 1000;

	// Token: 0x0400141D RID: 5149
	[Tooltip("Enable dynamic rerouting")]
	public bool dynamicReRouting = true;

	// Token: 0x0400141E RID: 5150
	[Space(7f)]
	public List<string> pathfinderDebugLog = new List<string>();

	// Token: 0x0400141F RID: 5151
	[Header("Evidence")]
	[Tooltip("Discover all evidence as soon as it is created")]
	public bool discoverAllEvidence;

	// Token: 0x04001420 RID: 5152
	[Header("Map")]
	[Tooltip("Discover all rooms")]
	public bool discoverAllRooms;

	// Token: 0x04001421 RID: 5153
	[Header("Gameplay")]
	[Tooltip("Player is always in illegal area")]
	public bool everywhereIllegal;

	// Token: 0x04001422 RID: 5154
	[Tooltip("Player is always hidden for AI")]
	public bool invisiblePlayer;

	// Token: 0x04001423 RID: 5155
	[Tooltip("Player cannot be heard by AI")]
	public bool inaudiblePlayer;

	// Token: 0x04001424 RID: 5156
	[Tooltip("Player cannot be killed")]
	public bool invinciblePlayer;

	// Token: 0x04001425 RID: 5157
	[Tooltip("Plotting a route will teleport the player to that location isntead")]
	public bool routeTeleport;

	// Token: 0x04001426 RID: 5158
	[Tooltip("Give all upgrades available in the city")]
	public bool giveAllUpgrades;

	// Token: 0x04001427 RID: 5159
	[Tooltip("Disable fall damage")]
	public bool disableFallDamage;

	// Token: 0x04001428 RID: 5160
	[Tooltip("Pause the AI completely")]
	public bool pauseAI;

	// Token: 0x04001429 RID: 5161
	[Tooltip("Free camera mode")]
	public bool freeCam;

	// Token: 0x0400142A RID: 5162
	[Tooltip("Fast forward")]
	public bool fastForward;

	// Token: 0x0400142B RID: 5163
	[Tooltip("Disable negatives statuses while in story mode")]
	public bool disableSurvivalStatusesInStory = true;

	// Token: 0x0400142C RID: 5164
	public bool sandboxStartingApartment;

	// Token: 0x0400142D RID: 5165
	public int sandboxStartingMoney = 100;

	// Token: 0x0400142E RID: 5166
	public int sandboxStartingLockpicks = 5;

	// Token: 0x0400142F RID: 5167
	[Tooltip("Build types where the player apartment will be located")]
	public List<BuildingPreset> preferredStartingBuildings = new List<BuildingPreset>();

	// Token: 0x04001430 RID: 5168
	[Tooltip("Will always run and sprint control will walk")]
	public bool alwaysRun;

	// Token: 0x04001431 RID: 5169
	[Tooltip("Toggles run instead of having to hold it down")]
	public bool toggleRun;

	// Token: 0x04001432 RID: 5170
	[Tooltip("If you get KO'd in game, the game ends")]
	public bool permaDeath;

	// Token: 0x04001433 RID: 5171
	[Tooltip("If true, the game will pause while auto travelling")]
	public bool autoTravelPause;

	// Token: 0x04001434 RID: 5172
	[Tooltip("Allow echelon gated communities")]
	public bool allowEchelons = true;

	// Token: 0x04001435 RID: 5173
	[Tooltip("Allow loitering")]
	public bool allowLoitering = true;

	// Token: 0x04001436 RID: 5174
	[Tooltip("Allow auto travel")]
	public bool allowAutoTravel = true;

	// Token: 0x04001437 RID: 5175
	[Tooltip("Allow social credit perks")]
	public bool allowSocialCreditPerks = true;

	// Token: 0x04001438 RID: 5176
	[Tooltip("Allow draggable bodies")]
	public bool allowDraggableRagdolls = true;

	// Token: 0x04001439 RID: 5177
	[Tooltip("Enables a quick way of testing cover-up offers: DISABLE FOR RELEASE!")]
	public bool forceCoverUpOffers;

	// Token: 0x0400143A RID: 5178
	[Tooltip("Enabled Cole's code check for falling through a floor; possible conflict with Austin's attached to the player controller")]
	public bool enableColesFallingThroughFloorCheck = true;

	// Token: 0x0400143B RID: 5179
	[Header("Difficulty: Prices")]
	public float jobRewardMultiplier = 1f;

	// Token: 0x0400143C RID: 5180
	public float jobPenaltyMultiplier = 1f;

	// Token: 0x0400143D RID: 5181
	public float housePriceMultiplier = 1f;

	// Token: 0x0400143E RID: 5182
	[Tooltip("Switch indoor lights to non-shadow casting when player is in a different groundmap area")]
	[Header("Graphics")]
	public bool noShadowsWhenPlayerIsInDifferentGoundmapLocation = true;

	// Token: 0x0400143F RID: 5183
	[Tooltip("Spawn & setup reflection probes")]
	public bool useReflectionProbes = true;

	// Token: 0x04001440 RID: 5184
	[Tooltip("Use a special raindrop material/shader on the streets")]
	public bool enableRaindrops = true;

	// Token: 0x04001441 RID: 5185
	[Tooltip("Use a special raindrop material/shader on the windows")]
	public bool enableRainyWindows = true;

	// Token: 0x04001442 RID: 5186
	public int fov = 70;

	// Token: 0x04001443 RID: 5187
	public bool depthBlur = true;

	// Token: 0x04001444 RID: 5188
	public float motionBlurIntensity = 1f;

	// Token: 0x04001445 RID: 5189
	public float motionBlurGameSpeedModifier = 1f;

	// Token: 0x04001446 RID: 5190
	public float bloomIntensity = 0.151f;

	// Token: 0x04001447 RID: 5191
	public bool shadowsOnCitizenLOD = true;

	// Token: 0x04001448 RID: 5192
	public bool vsync;

	// Token: 0x04001449 RID: 5193
	public bool enableFrameCap;

	// Token: 0x0400144A RID: 5194
	public int frameCap = 60;

	// Token: 0x0400144B RID: 5195
	public bool flickeringLights;

	// Token: 0x0400144C RID: 5196
	public bool enableRuntimeStaticBatching = true;

	// Token: 0x0400144D RID: 5197
	[Tooltip("Use quads instead of decal projectors for footprints")]
	public bool useQuadsForFootprints;

	// Token: 0x0400144E RID: 5198
	[InfoBox("The below is an experimental light culling system involving jobified raycasts. It doesn't seem to increase performance much and has some slightly noticable glitches. I've chosen to keep it in the code base as a potential thing to return to but will probably keep it disabled for the forseeable future", 0)]
	public bool enableCustomLightCulling;

	// Token: 0x0400144F RID: 5199
	[InfoBox("The below is an experimental real time culling system that is being experimented with. If successful this would allow us to ditch any pre-baked culling trees; resulting in faster generation times and more memory (hopefully!)", 0)]
	public bool enableNewRealtimeTimeCullingSystem;

	// Token: 0x04001450 RID: 5200
	[DisableIf("enableNewRealtimeTimeCullingSystem")]
	[Tooltip("If true, the game will generate culling trees using the legacy system, previously used at generation-time only")]
	public bool generateCullingInGame;

	// Token: 0x04001451 RID: 5201
	[Range(0.4f, 2f)]
	[Space(7f)]
	public float lightFadeDistanceMultiplier = 1f;

	// Token: 0x04001452 RID: 5202
	[Range(0.4f, 2f)]
	public float shadowFadeDistanceMultiplier = 1f;

	// Token: 0x04001453 RID: 5203
	[Tooltip("Same as above but for the sun")]
	[Header("Shadows")]
	public int sunShadowUpdateFrequency = 1;

	// Token: 0x04001454 RID: 5204
	[ReadOnly]
	public int lastShadowsUpdatedCount;

	// Token: 0x04001455 RID: 5205
	[Tooltip("Override shadow mode on all light controllers...")]
	public bool overrideLightControllerShadowMode;

	// Token: 0x04001456 RID: 5206
	[EnableIf("overrideLightControllerShadowMode")]
	public LightingPreset.ShadowMode shadowModeOverride;

	// Token: 0x04001457 RID: 5207
	public int dynamicShadowUpdateFrames = 3;

	// Token: 0x04001458 RID: 5208
	public int maxUpdateDynamicShadowsPerFrame = 5;

	// Token: 0x04001459 RID: 5209
	[Header("Meshes")]
	[Tooltip("Combining meshes will take longer on load and increase memory, but also increase performance...")]
	public bool combineAirDuctMeshes = true;

	// Token: 0x0400145A RID: 5210
	[Tooltip("Combining meshes will take longer on load and increase memory, but also increase performance...")]
	public bool combineRoomMeshes = true;

	// Token: 0x0400145B RID: 5211
	public ShadowCastingMode roomWallShadowMode = 1;

	// Token: 0x0400145C RID: 5212
	public ShadowCastingMode roomFloorShadowMode = 1;

	// Token: 0x0400145D RID: 5213
	public ShadowCastingMode roomCeilingShadowMode = 2;

	// Token: 0x0400145E RID: 5214
	public ShadowCastingMode airDuctShadowMode = 1;

	// Token: 0x0400145F RID: 5215
	[Tooltip("Enables job based mesh creation")]
	public bool useJobSystemForMeshCombination = true;

	// Token: 0x04001460 RID: 5216
	public bool optimizeCombinedMeshes = true;

	// Token: 0x04001461 RID: 5217
	[Tooltip("Automatically weld vertices together if they share the same space")]
	public bool autoWeldVertices = true;

	// Token: 0x04001462 RID: 5218
	[Header("Interface")]
	public int uiScale = 100;

	// Token: 0x04001463 RID: 5219
	[Header("Editor")]
	public bool selectCitizenOnLookAt;

	// Token: 0x04001464 RID: 5220
	public int base26Test;

	// Token: 0x04001465 RID: 5221
	public bool screenshotMode;

	// Token: 0x04001466 RID: 5222
	public bool screenshotModeAllowDialog;

	// Token: 0x04001467 RID: 5223
	[Header("Debug")]
	public List<Actor> debugHuman = new List<Actor>();

	// Token: 0x04001468 RID: 5224
	public bool debugHumanMovement = true;

	// Token: 0x04001469 RID: 5225
	public bool debugHumanActions = true;

	// Token: 0x0400146A RID: 5226
	public bool debugHumanAttacks = true;

	// Token: 0x0400146B RID: 5227
	public bool debugHumanUpdates = true;

	// Token: 0x0400146C RID: 5228
	public bool debugHumanMisc = true;

	// Token: 0x0400146D RID: 5229
	public bool debugHumanSight = true;

	// Token: 0x0400146E RID: 5230
	public int debugFindWall;

	// Token: 0x0400146F RID: 5231
	public int debugAddressID;

	// Token: 0x04001470 RID: 5232
	public int debugCitizenID;

	// Token: 0x04001471 RID: 5233
	public int debugPhotoTestID;

	// Token: 0x04001472 RID: 5234
	public MurderWeaponPreset debugTestWeapon;

	// Token: 0x04001473 RID: 5235
	public List<Game.DebugCitizenWeapons> debugWeaponsSurvey = new List<Game.DebugCitizenWeapons>();

	// Token: 0x04001474 RID: 5236
	private static Game _instance;

	// Token: 0x020002CD RID: 717
	[Serializable]
	public class DebugCitizenWeapons
	{
		// Token: 0x04001475 RID: 5237
		public MurderWeaponPreset weapon;

		// Token: 0x04001476 RID: 5238
		public int count;

		// Token: 0x04001477 RID: 5239
		public float percentage;
	}
}
