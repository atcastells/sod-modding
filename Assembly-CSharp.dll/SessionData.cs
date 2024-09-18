using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020001AC RID: 428
public class SessionData : MonoBehaviour
{
	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0009E30F File Offset: 0x0009C50F
	public static SessionData Instance
	{
		get
		{
			return SessionData._instance;
		}
	}

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06000A7B RID: 2683 RVA: 0x0009E318 File Offset: 0x0009C518
	// (remove) Token: 0x06000A7C RID: 2684 RVA: 0x0009E350 File Offset: 0x0009C550
	public event SessionData.OnPauseUnPause OnPauseChange;

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000A7D RID: 2685 RVA: 0x0009E388 File Offset: 0x0009C588
	// (remove) Token: 0x06000A7E RID: 2686 RVA: 0x0009E3C0 File Offset: 0x0009C5C0
	public event SessionData.WeatherChange OnWeatherChange;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x06000A7F RID: 2687 RVA: 0x0009E3F8 File Offset: 0x0009C5F8
	// (remove) Token: 0x06000A80 RID: 2688 RVA: 0x0009E430 File Offset: 0x0009C630
	public event SessionData.HourChange OnHourChange;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000A81 RID: 2689 RVA: 0x0009E468 File Offset: 0x0009C668
	// (remove) Token: 0x06000A82 RID: 2690 RVA: 0x0009E4A0 File Offset: 0x0009C6A0
	public event SessionData.TutorialNotificationChange OnTutorialNotificationChange;

	// Token: 0x06000A83 RID: 2691 RVA: 0x0009E4D8 File Offset: 0x0009C6D8
	private void Awake()
	{
		if (SessionData._instance != null && SessionData._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			SessionData._instance = this;
		}
		this.daysInMonths.Add(31);
		this.daysInMonths.Add(28);
		this.daysInMonths.Add(31);
		this.daysInMonths.Add(30);
		this.daysInMonths.Add(31);
		this.daysInMonths.Add(30);
		this.daysInMonths.Add(31);
		this.daysInMonths.Add(31);
		this.daysInMonths.Add(30);
		this.daysInMonths.Add(31);
		this.daysInMonths.Add(30);
		this.daysInMonths.Add(31);
		if (this.globalVolume != null)
		{
			for (int i = 0; i < this.globalVolume.profile.components.Count; i++)
			{
				if (this.globalVolume.profile.components[i] as DepthOfField != null)
				{
					this.dof = (this.globalVolume.profile.components[i] as DepthOfField);
				}
				else if (this.globalVolume.profile.components[i] as Vignette != null)
				{
					this.vignette = (this.globalVolume.profile.components[i] as Vignette);
				}
				else if (this.globalVolume.profile.components[i] as FilmGrain != null)
				{
					this.grain = (this.globalVolume.profile.components[i] as FilmGrain);
				}
				else if (this.globalVolume.profile.components[i] as MotionBlur != null)
				{
					this.motionBlur = (this.globalVolume.profile.components[i] as MotionBlur);
				}
				else if (this.globalVolume.profile.components[i] as Bloom != null)
				{
					this.bloom = (this.globalVolume.profile.components[i] as Bloom);
				}
				else if (this.globalVolume.profile.components[i] as Tonemapping != null)
				{
					this.toneMapping = (this.globalVolume.profile.components[i] as Tonemapping);
				}
				else if (this.globalVolume.profile.components[i] as GradientSky != null)
				{
					this.gradientSky = (this.globalVolume.profile.components[i] as GradientSky);
				}
				else if (this.globalVolume.profile.components[i] as Fog != null)
				{
					this.volFog = (this.globalVolume.profile.components[i] as Fog);
				}
				else if (this.globalVolume.profile.components[i] as ChromaticAberration != null)
				{
					this.chromaticAberration = (this.globalVolume.profile.components[i] as ChromaticAberration);
				}
				else if (this.globalVolume.profile.components[i] as LiftGammaGain != null)
				{
					this.lgg = (this.globalVolume.profile.components[i] as LiftGammaGain);
				}
				else if (this.globalVolume.profile.components[i] as ColorAdjustments != null)
				{
					this.colour = (this.globalVolume.profile.components[i] as ColorAdjustments);
				}
				else if (this.globalVolume.profile.components[i] as LensDistortion != null)
				{
					this.lensDistort = (this.globalVolume.profile.components[i] as LensDistortion);
				}
				else if (this.globalVolume.profile.components[i] as Exposure != null)
				{
					this.exposure = (this.globalVolume.profile.components[i] as Exposure);
				}
				else if (this.globalVolume.profile.components[i] as ChannelMixer != null)
				{
					this.channelMixer = (this.globalVolume.profile.components[i] as ChannelMixer);
				}
			}
		}
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x0009E9DC File Offset: 0x0009CBDC
	private void Start()
	{
		this.SetGameTime(GameplayControls.Instance.startingYear, GameplayControls.Instance.startingMonth, GameplayControls.Instance.startingDate, GameplayControls.Instance.dayZero, Game.Instance.sandboxStartTime, GameplayControls.Instance.yearZeroLeapYearCycle);
		Shader.SetGlobalFloat("_CoatMask", 0f);
		Shader.SetGlobalFloat("_Rain", 0f);
		Shader.SetGlobalFloat("_CityWetness", 0f);
		Shader.SetGlobalFloat("_Snow", 0f);
		Shader.SetGlobalFloat("_Rain", 0f);
		if (this.isTestScene)
		{
			this.StartTestScene();
		}
		CityControls.Instance.seaMaterial = Object.Instantiate<Material>(CityControls.Instance.seaMaterial);
		if (!SessionData.Instance.isFloorEdit)
		{
			CityControls.Instance.seaRenderer.sharedMaterial = CityControls.Instance.seaMaterial;
		}
		CityControls.Instance.skylineMaterial = Object.Instantiate<Material>(CityControls.Instance.skylineMaterial);
		foreach (MeshRenderer meshRenderer in CityControls.Instance.skylineRenderers)
		{
			if (meshRenderer != null)
			{
				meshRenderer.sharedMaterial = CityControls.Instance.skylineMaterial;
			}
		}
		CityControls.Instance.smokeMaterial = Object.Instantiate<Material>(CityControls.Instance.smokeMaterial);
		this.UnloadPipes = (Action)Delegate.Combine(this.UnloadPipes, new Action(this.ExecuteUnloadPipes));
		this.SetupTelevisionChannels();
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0009EB78 File Offset: 0x0009CD78
	public void SetupTelevisionChannels()
	{
		foreach (BroadcastSchedule currentBroadcastSchedule in Toolbox.Instance.allBroadcasts)
		{
			SessionData.TelevisionChannel televisionChannel = new SessionData.TelevisionChannel();
			televisionChannel.currentBroadcastSchedule = currentBroadcastSchedule;
			televisionChannel.broadcastMaterialInstanced = Object.Instantiate<Material>(this.broadcastMaterial);
			televisionChannel.broadcastMaterialInstanced.EnableKeyword("_TEX_ON");
			televisionChannel.broadcastMaterialInstanced.EnableKeyword("_EMISSION");
			televisionChannel.broadcastMaterialInstanced.EnableKeyword("_EMISSIVE_COLOR_MAP");
			this.televisionChannels.Add(televisionChannel);
		}
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0009EC24 File Offset: 0x0009CE24
	public void StartTestScene()
	{
		this.isTestScene = true;
		int num = 5;
		CityData.Instance.citySize = new Vector2((float)num, (float)num);
		PathFinder.Instance.tileSize = new Vector3(CityControls.Instance.cityTileSize.x / (float)CityControls.Instance.tileMultiplier, CityControls.Instance.cityTileSize.y / (float)CityControls.Instance.tileMultiplier, CityControls.Instance.cityTileSize.z / (float)CityControls.Instance.tileMultiplier);
		PathFinder.Instance.nodeSize = new Vector3(PathFinder.Instance.tileSize.x / (float)CityControls.Instance.nodeMultiplier, PathFinder.Instance.tileSize.y / (float)CityControls.Instance.nodeMultiplier, PathFinder.Instance.tileSize.z);
		PathFinder.Instance.citySizeReal = new Vector2(CityData.Instance.citySize.x * CityControls.Instance.cityTileSize.x, CityData.Instance.citySize.y * CityControls.Instance.cityTileSize.y);
		PathFinder.Instance.halfCitySizeReal = PathFinder.Instance.citySizeReal * 0.5f;
		PathFinder.Instance.tileCitySize = CityData.Instance.citySize * (float)CityControls.Instance.tileMultiplier;
		PathFinder.Instance.nodeCitySize = PathFinder.Instance.tileCitySize * (float)CityControls.Instance.nodeMultiplier;
		for (int i = -1; i < num + 1; i++)
		{
			for (int j = -1; j < num + 1; j++)
			{
				if (i >= 0 && j >= 0 && i < num && j < num)
				{
					CityTile component = Object.Instantiate<GameObject>(PrefabControls.Instance.cityTile, PrefabControls.Instance.cityContainer.transform).GetComponent<CityTile>();
					Vector2Int newCoord;
					newCoord..ctor(i, j);
					component.Setup(newCoord);
				}
			}
		}
		for (int k = 0; k < num; k++)
		{
			for (int l = 0; l < num; l++)
			{
				CityTile cityTile = HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles[new Vector2Int(k, l)];
				Vector2Int vector2Int;
				vector2Int..ctor(Mathf.RoundToInt((float)(k * CityControls.Instance.tileMultiplier)), Mathf.RoundToInt((float)(l * CityControls.Instance.tileMultiplier)));
				for (int m = 0; m < CityControls.Instance.tileMultiplier; m++)
				{
					for (int n = 0; n < CityControls.Instance.tileMultiplier; n++)
					{
						Vector3Int vector3Int;
						vector3Int..ctor(vector2Int.x + m, vector2Int.y + n, 0);
						NewTile newTile = new NewTile();
						NewTile newTile2 = newTile;
						string text = "Pathfinder Created Tile ";
						Vector3Int vector3Int2 = vector3Int;
						newTile2.name = text + vector3Int2.ToString();
						newTile.globalTileCoord = vector3Int;
						newTile.SetAsOutside(true);
						newTile.cityTile = cityTile;
						newTile.isEdge = true;
						PathFinder.Instance.tileMap.Add(vector3Int, newTile);
					}
				}
			}
		}
		DistrictController component2 = Object.Instantiate<GameObject>(PrefabControls.Instance.district, PrefabControls.Instance.cityContainer.transform).GetComponent<DistrictController>();
		component2.Setup(null);
		BlockController component3 = Object.Instantiate<GameObject>(PrefabControls.Instance.block, PrefabControls.Instance.cityContainer.transform).GetComponent<BlockController>();
		component3.Setup(component2);
		foreach (KeyValuePair<Vector2Int, CityTile> keyValuePair in HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles)
		{
			component2.AddCityTile(keyValuePair.Value);
		}
		component2.AddBlock(component3);
		StreetController component4 = Object.Instantiate<GameObject>(PrefabControls.Instance.street, PrefabControls.Instance.cityContainer.transform).GetComponent<StreetController>();
		component4.Setup(component2);
		for (int num2 = 0; num2 < num; num2++)
		{
			for (int num3 = 0; num3 < num; num3++)
			{
				CityTile newCityTile = HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles[new Vector2Int(num2, num3)];
				Vector2Int vector2Int2;
				vector2Int2..ctor(Mathf.RoundToInt((float)(num2 * CityControls.Instance.tileMultiplier)), Mathf.RoundToInt((float)(num3 * CityControls.Instance.tileMultiplier)));
				for (int num4 = 0; num4 < CityControls.Instance.tileMultiplier; num4++)
				{
					for (int num5 = 0; num5 < CityControls.Instance.tileMultiplier; num5++)
					{
						Vector3Int vector3Int3;
						vector3Int3..ctor(vector2Int2.x + num4, vector2Int2.y + num5, 0);
						if (PathFinder.Instance.tileMap.ContainsKey(vector3Int3))
						{
							NewTile newTile3 = PathFinder.Instance.tileMap[vector3Int3];
							component4.AddTile(newTile3);
							newTile3.SetupExterior(newCityTile, vector3Int3);
						}
					}
				}
			}
		}
		component4.rooms[0].SetVisible(true, false, "Test Scene", false);
		Player.Instance.currentRoom = component4.rooms[0];
		CameraController.Instance.SetupFPS();
		Player.Instance.OnCityTileChange();
		InterfaceController.Instance.SetInterfaceActive(true);
		SessionData.Instance.startedGame = true;
		CitizenBehaviour.Instance.StartGame();
		this.ResumeGame();
		AudioController.Instance.StartAmbienceTracks();
		MainMenuController.Instance.EnableMainMenu(false, true, true, MainMenuController.Component.mainMenuButtons);
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x0009F1B4 File Offset: 0x0009D3B4
	public void SetGameTime(int newYear, int newMonth, int newDate, int newDay, float newStartingTime, int newLeapYearCycle)
	{
		this.yearInt = newYear;
		this.monthInt = newMonth;
		this.month = this.MonthFromInt(this.monthInt);
		this.monthTempMultiplier = CityControls.Instance.weatherSettings.monthTempCurve.Evaluate((float)this.month / 11f);
		this.dateInt = newDate;
		this.dayInt = newDay;
		this.decimalClock = newStartingTime;
		this.leapYearCycle = newLeapYearCycle;
		this.formattedClock = this.FloatMinutes24H(newStartingTime);
		this.decimalClockDouble = (double)this.decimalClock;
		this.SetTimeSpeed(GameplayControls.Instance.startingTimeSpeed);
		while (this.leapYearCycle >= 4)
		{
			this.leapYearCycle -= 4;
		}
		this.gameTime = this.ParseGameTime(this.decimalClock, this.dateInt, this.monthInt, this.yearInt, out this.dayInt, out this.leapYearCycle);
		this.gameTimeDouble = (double)this.gameTime;
		this.day = this.WeekdayFromInt(this.dayInt);
		this.publicYear = this.yearInt + GameplayControls.Instance.publicYearZero;
		Game.Log(string.Concat(new string[]
		{
			"CityGen: Set game time to: ",
			this.gameTime.ToString(),
			" (",
			this.decimalClock.ToString(),
			")"
		}), 2);
		this.UpdateSkyboxGraidentTargets();
		this.UpdateUIClock();
		this.UpdateUIDay();
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x0009F328 File Offset: 0x0009D528
	public void SetGameTime(float newGameTime, int newLeapYearCycle)
	{
		this.gameTime = newGameTime;
		this.gameTimeDouble = (double)newGameTime;
		this.leapYearCycle = newLeapYearCycle;
		int num = 0;
		this.ParseTimeData(newGameTime, out this.decimalClock, out this.dayInt, out this.dateInt, out this.monthInt, out num);
		this.yearInt = num - GameplayControls.Instance.publicYearZero;
		this.decimalClockDouble = (double)this.decimalClock;
		this.month = this.MonthFromInt(this.monthInt);
		this.monthTempMultiplier = CityControls.Instance.weatherSettings.monthTempCurve.Evaluate((float)this.month / 11f);
		this.formattedClock = this.FloatMinutes24H(this.decimalClock);
		this.SetTimeSpeed(GameplayControls.Instance.startingTimeSpeed);
		while (this.leapYearCycle >= 4)
		{
			this.leapYearCycle -= 4;
		}
		this.day = this.WeekdayFromInt(this.dayInt);
		this.publicYear = this.yearInt + GameplayControls.Instance.publicYearZero;
		Game.Log("CityGen: Set game time to: " + this.gameTime.ToString(), 2);
		this.UpdateSkyboxGraidentTargets();
		this.UpdateUIClock();
		this.UpdateUIDay();
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0009F458 File Offset: 0x0009D658
	public void UpdateSkyboxGraidentTargets()
	{
		this.skyboxGradientIndex = 0;
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		for (int i = 0; i < CityControls.Instance.weatherSettings.skyboxGradientGrading.Count; i++)
		{
			if (this.decimalClock > CityControls.Instance.weatherSettings.skyboxGradientGrading[i].time)
			{
				this.skyboxGradientIndex++;
				if (this.skyboxGradientIndex >= CityControls.Instance.weatherSettings.skyboxGradientGrading.Count)
				{
					this.skyboxGradientIndex = 0;
				}
			}
		}
		this.fromSkyboxColours = CityControls.Instance.weatherSettings.skyboxGradientGrading[this.skyboxGradientIndex];
		if (this.skyboxGradientIndex + 1 >= CityControls.Instance.weatherSettings.skyboxGradientGrading.Count)
		{
			this.toSkyboxColours = CityControls.Instance.weatherSettings.skyboxGradientGrading[0];
			return;
		}
		this.toSkyboxColours = CityControls.Instance.weatherSettings.skyboxGradientGrading[this.skyboxGradientIndex + 1];
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x0009F568 File Offset: 0x0009D768
	public void SetTimeSpeed(SessionData.TimeSpeed newTimeSpeed)
	{
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		this.currentTimeSpeed = newTimeSpeed;
		this.currentTimeMultiplier = GameplayControls.Instance.timeMultipliers[(int)this.currentTimeSpeed];
		Game.Log("Player: Set new time speed: " + this.currentTimeSpeed.ToString(), 2);
		SessionData.Instance.motionBlur.intensity.value = Game.Instance.motionBlurIntensity + this.GetGameSpeedMotionBlurModifier();
		if (CitizenBehaviour.Instance != null)
		{
			CitizenBehaviour.Instance.GameSpeedChange();
		}
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x0009F601 File Offset: 0x0009D801
	public float GetGameSpeedMotionBlurModifier()
	{
		if (this.currentTimeSpeed == SessionData.TimeSpeed.veryFast)
		{
			return Game.Instance.motionBlurGameSpeedModifier;
		}
		return 0f;
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x0009F61C File Offset: 0x0009D81C
	public void SetSceneProfile(SessionData.SceneProfile newProfile, bool immediate = false)
	{
		if (newProfile != this.currentProfile)
		{
			this.currentProfile = newProfile;
			Game.Log("Player: Setting to volume profile: " + this.currentProfile.ToString(), 2);
			this.desiredSceneProfile = CityControls.Instance.sceneProfileSetup.Find((CityControls.PPProfile item) => item.profile == newProfile);
			if (immediate)
			{
				foreach (CityControls.PPProfile ppprofile in CityControls.Instance.sceneProfileSetup)
				{
					if (ppprofile == this.desiredSceneProfile)
					{
						this.currentSceneProfile = this.desiredSceneProfile;
						ppprofile.objectRef.SetActive(true);
						ppprofile.volume.priority = 1f;
						ppprofile.volume.weight = 1f;
					}
					else
					{
						ppprofile.volume.priority = -1f;
						ppprofile.volume.weight = 0f;
						ppprofile.objectRef.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0009F750 File Offset: 0x0009D950
	private void Update()
	{
		if (Game.Instance.demoAutoReset && Game.Instance.demoMode && ChapterController.Instance != null && ChapterController.Instance.currentPart >= Game.Instance.resetChapterPart)
		{
			if (this.autoResetTimer >= (float)Game.Instance.resetSeconds)
			{
				if (Game.Instance.resetSaveGameName.Length < 0)
				{
					goto IL_16A;
				}
				List<FileInfo> list = Enumerable.ToList<FileInfo>(new DirectoryInfo(Application.streamingAssetsPath + "/Save").GetFiles("*.sod", 1));
				FileInfo fileInfo = list.Find((FileInfo item) => item.Name == Game.Instance.resetSaveGameName);
				if (fileInfo != null)
				{
					RestartSafeController.Instance.saveStateFileInfo = fileInfo;
					RestartSafeController.Instance.newGameLoadCity = false;
					RestartSafeController.Instance.generateNew = false;
					RestartSafeController.Instance.loadSaveGame = true;
					RestartSafeController.Instance.loadFromDirty = true;
					Game.Log("CityGen: Scene is dirty, restarting to continue load process...", 2);
					AudioController.Instance.StopAllSounds();
					SceneManager.LoadScene("Main");
					goto IL_16A;
				}
				using (List<FileInfo>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FileInfo fileInfo2 = enumerator.Current;
						Game.LogError("Found save: " + fileInfo2.Name, 2);
					}
					goto IL_16A;
				}
			}
			this.autoResetTimer += Time.deltaTime;
		}
		IL_16A:
		if (this.play)
		{
			if (Game.Instance.autoPause)
			{
				if (this.autoPauseTimer >= (float)Game.Instance.autoPauseSeconds)
				{
					if (!Player.Instance.autoTravelActive || Game.Instance.autoTravelPause)
					{
						this.PauseGame(true, false, true);
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.ComposeText(Strings.Get("ui.gamemessage", "You can open your case board at any time by pressing F", Strings.Casing.asIs, false, false, false, null), null, Strings.LinkSetting.forceNoLinks, null, null, false), InterfaceControls.Icon.pin, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					}
				}
				else if (!CutSceneController.Instance.cutSceneActive && !BioScreenController.Instance.isOpen)
				{
					this.autoPauseTimer += Time.deltaTime;
				}
			}
			this.gameTimePassedThisFrame = (double)(Time.deltaTime * this.currentTimeMultiplier * 0.0025f);
			this.decimalClockDouble += this.gameTimePassedThisFrame;
			this.gameTimeDouble += this.gameTimePassedThisFrame;
			this.decimalClock = Toolbox.Instance.RoundToPlaces(this.decimalClockDouble, 4);
			this.gameTime = Toolbox.Instance.RoundToPlaces(this.gameTimeDouble, 4);
			int num = Mathf.FloorToInt(this.gameTime);
			if (num != this.prevHour)
			{
				this.prevHour = num;
				CitizenBehaviour.Instance.OnHourChange();
				if (this.OnHourChange != null)
				{
					this.OnHourChange();
				}
			}
			if (this.decimalClockDouble >= 24.0)
			{
				float num2 = Toolbox.Instance.RoundToPlaces(this.decimalClock - 24f, 2);
				this.decimalClockDouble = (double)num2;
				this.decimalClock = num2;
				this.gameTime = Mathf.Round(this.gameTime) + num2;
				this.gameTimeDouble = (double)(Mathf.Round((float)this.gameTimeDouble) + num2);
				this.dateInt++;
				this.dayInt++;
				if (this.dayInt >= 7)
				{
					this.dayInt -= 7;
				}
				this.day = this.WeekdayFromInt(this.dayInt);
				int num3 = this.daysInMonths[this.monthInt];
				if (this.leapYearCycle == 3 && this.monthInt == 1)
				{
					num3++;
				}
				if (this.dateInt >= num3)
				{
					this.dateInt -= num3;
					this.monthInt++;
					if (this.monthInt >= 12)
					{
						this.monthInt -= 12;
						this.yearInt++;
						this.leapYearCycle++;
						this.publicYear = this.yearInt + GameplayControls.Instance.publicYearZero;
						while (this.leapYearCycle >= 4)
						{
							this.leapYearCycle -= 4;
						}
					}
					this.month = this.MonthFromInt(this.monthInt);
					this.monthTempMultiplier = CityControls.Instance.weatherSettings.monthTempCurve.Evaluate((float)this.month / 11f);
				}
				foreach (Citizen citizen in CityData.Instance.citizenDirectory)
				{
					citizen.BirthdayCheck();
				}
				CitizenBehaviour.Instance.OnDayChange();
				this.UpdateUIDay();
				NewspaperController.Instance.GenerateNewNewspaper();
			}
			this.formattedClock = this.FloatMinutes24H(this.decimalClock);
			this.UpdateUIClock();
			if (this.decimalClock > 3f && this.decimalClock <= 12f && this.timeOfDay != SessionData.TimeOfDay.morning)
			{
				this.timeOfDay = SessionData.TimeOfDay.morning;
			}
			else if (this.decimalClock > 12f && this.decimalClock <= 5f && this.timeOfDay != SessionData.TimeOfDay.afternoon)
			{
				this.timeOfDay = SessionData.TimeOfDay.afternoon;
			}
			else if (this.decimalClock > 5f && this.decimalClock <= 3f && this.timeOfDay != SessionData.TimeOfDay.evening)
			{
				this.timeOfDay = SessionData.TimeOfDay.evening;
			}
			this.dayProgress = this.decimalClock / 24f;
			if (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive)
			{
				if (this.weatherChangeTimer >= 0.33f)
				{
					float newWind = CityControls.Instance.weatherSettings.weatherExtremityCurve.Evaluate(Toolbox.Instance.Rand(0f, 1f, false));
					float newRain = 0f;
					float newLightning = 0f;
					float num4 = 0f;
					float num5 = CityControls.Instance.weatherSettings.monthSnowChanceCurve.Evaluate((float)this.month / 11f);
					float newFog = CityControls.Instance.weatherSettings.weatherExtremityCurve.Evaluate(Toolbox.Instance.Rand(0f, 1f, false));
					if (Toolbox.Instance.Rand(0f, 1f, false) <= num5)
					{
						num4 = CityControls.Instance.weatherSettings.weatherExtremityCurve.Evaluate(Toolbox.Instance.Rand(0f, 1f, false));
						num4 = Toolbox.Instance.RandomRangeWeighted(0f, 1f, num4, 5);
					}
					if (num4 <= 0.1f)
					{
						newRain = CityControls.Instance.weatherSettings.weatherExtremityCurve.Evaluate(Toolbox.Instance.Rand(0f, 1f, false));
						newLightning = CityControls.Instance.weatherSettings.weatherExtremityCurve.Evaluate(Toolbox.Instance.Rand(0f, 1f, false));
					}
					this.SetWeather(newRain, newWind, num4, newLightning, newFog, Toolbox.Instance.Rand(0.1f, 0.4f, false), false);
					this.weatherChangeTimer = 0f;
				}
				else
				{
					this.weatherChangeTimer += (float)this.gameTimePassedThisFrame * (Game.Instance.weatherChangeFrequency * 0.7f);
				}
				bool flag = false;
				if (this.citySnow >= 0.25f)
				{
					this.desiredRain = 0f;
				}
				if (this.cityWetness >= 0.25f)
				{
					this.desiredSnow = 0f;
				}
				if (this.currentSnow < this.desiredSnow && this.currentRain <= 0.01f)
				{
					this.currentSnow += Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentSnow > this.desiredSnow)
					{
						this.currentSnow = this.desiredSnow;
					}
					flag = true;
				}
				else if (this.currentSnow > this.desiredSnow)
				{
					this.currentSnow -= Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentSnow < this.desiredSnow)
					{
						this.currentSnow = this.desiredSnow;
					}
					flag = true;
				}
				if (this.currentRain < this.desiredRain && this.currentSnow <= 0.01f)
				{
					this.currentRain += Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentRain > this.desiredRain)
					{
						this.currentRain = this.desiredRain;
					}
					flag = true;
				}
				else if (this.currentRain > this.desiredRain)
				{
					this.currentRain -= Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentRain < this.desiredRain)
					{
						this.currentRain = this.desiredRain;
					}
					flag = true;
				}
				if (this.currentWind < this.desiredWind)
				{
					this.currentWind += Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentWind > this.desiredWind)
					{
						this.currentWind = this.desiredWind;
					}
					flag = true;
					this.ExecuteWindChange();
				}
				else if (this.currentWind > this.desiredWind)
				{
					this.currentWind -= Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentWind < this.desiredWind)
					{
						this.currentWind = this.desiredWind;
					}
					flag = true;
					this.ExecuteWindChange();
				}
				if (this.currentLightning < this.desiredLightning)
				{
					this.currentLightning += Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentLightning > this.desiredLightning)
					{
						this.currentLightning = this.desiredLightning;
					}
					flag = true;
				}
				else if (this.currentLightning > this.desiredLightning)
				{
					this.currentLightning -= Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentLightning < this.desiredLightning)
					{
						this.currentLightning = this.desiredLightning;
					}
					flag = true;
				}
				if (this.currentFog < this.desiredFog)
				{
					this.currentFog += Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentFog > this.desiredFog)
					{
						this.currentFog = this.desiredFog;
					}
				}
				else if (this.currentFog > this.desiredFog)
				{
					this.currentFog -= Time.deltaTime * this.currentTimeMultiplier * this.transitionSpeed;
					if (this.currentFog < this.desiredFog)
					{
						this.currentFog = this.desiredFog;
					}
				}
				if (flag)
				{
					this.ExecuteWeatherChange();
				}
				bool flag2 = false;
				if (this.currentRain > 0f && this.cityWetness < 1f)
				{
					this.cityWetness += this.currentRain * ((float)this.gameTimePassedThisFrame / CityControls.Instance.timeForCityToGetWet);
					this.cityWetness = Mathf.Clamp01(this.cityWetness);
					flag2 = true;
				}
				else if (this.currentRain <= 0f && this.cityWetness > 0f)
				{
					this.cityWetness -= (float)this.gameTimePassedThisFrame / CityControls.Instance.timeForCityToGetDry;
					this.cityWetness = Mathf.Clamp01(this.cityWetness);
					flag2 = true;
				}
				if (this.currentSnow > 0f && this.citySnow < 1f)
				{
					this.citySnow += this.currentSnow * ((float)this.gameTimePassedThisFrame / CityControls.Instance.timeForCityToGetSnow);
					this.citySnow = Mathf.Clamp01(this.citySnow);
					flag2 = true;
				}
				else if (this.currentSnow <= 0f && this.citySnow > 0f)
				{
					this.citySnow -= (float)this.gameTimePassedThisFrame / CityControls.Instance.timeForCityToGetNotSnow;
					this.citySnow = Mathf.Clamp01(this.citySnow);
					flag2 = true;
				}
				if (flag2)
				{
					this.ExecuteWetnessChange();
				}
				if (this.currentLightning > 0f)
				{
					this.lightningTimer += Toolbox.Instance.Rand(0f, this.currentLightning, false) * Time.deltaTime * this.currentTimeMultiplier;
					if (AudioDebugging.Instance.overrideThunderDelay && this.lightningTimer >= AudioDebugging.Instance.thunderDelay)
					{
						this.ExecuteLightningStrike();
						this.lightningTimer = 0f;
					}
					else if (!AudioDebugging.Instance.overrideThunderDelay && this.lightningTimer >= CityControls.Instance.weatherSettings.thunderDelay)
					{
						this.ExecuteLightningStrike();
						this.lightningTimer = 0f;
					}
				}
				AudioController.Instance.PassTimeOfDay();
				this.SetSceneVisuals(this.decimalClock);
				for (int i = 0; i < CityControls.Instance.neonColours.Count; i++)
				{
					CityControls.NeonMaterial neonMaterial = CityControls.Instance.neonColours[i];
					if (neonMaterial.flickingMat != null)
					{
						neonMaterial.interval += Time.deltaTime * SessionData.Instance.currentTimeMultiplier;
						if (!neonMaterial.flickerInterval && neonMaterial.interval >= neonMaterial.intervalTime)
						{
							neonMaterial.interval = 0f;
							neonMaterial.flickerInterval = true;
							neonMaterial.intervalTime = Toolbox.Instance.Rand(0.25f, 2f, false);
						}
						else if (neonMaterial.flickerInterval && neonMaterial.interval >= neonMaterial.intervalTime)
						{
							neonMaterial.interval = 0f;
							neonMaterial.flickerInterval = false;
							neonMaterial.intervalTime = Toolbox.Instance.Rand(0.1f, 10f, false);
						}
						if (neonMaterial.flickerState < 1f && neonMaterial.flickerSwitch)
						{
							neonMaterial.flickerState += Time.deltaTime * neonMaterial.pulseSpeed;
						}
						else if (neonMaterial.flickerState >= 1f && neonMaterial.flickerSwitch)
						{
							neonMaterial.flickerSwitch = false;
							neonMaterial.flickerColourMultiplier = Toolbox.Instance.Rand(0.1f, 0.95f, false);
							neonMaterial.pulseSpeed = Toolbox.Instance.Rand(20f, 22f, false);
						}
						if (neonMaterial.flickerInterval && neonMaterial.flickerState > 0f && !neonMaterial.flickerSwitch)
						{
							neonMaterial.flickerState -= Time.deltaTime * neonMaterial.pulseSpeed;
						}
						else if (neonMaterial.flickerState <= 0f && !neonMaterial.flickerSwitch)
						{
							neonMaterial.flickerSwitch = true;
							neonMaterial.flickerColourMultiplier = Toolbox.Instance.Rand(0.1f, 0.95f, false);
						}
						neonMaterial.flickerState = Mathf.Clamp01(neonMaterial.flickerState);
						neonMaterial.brightness = neonMaterial.flickerColourMultiplier;
						neonMaterial.flickingMat.SetColor("_BaseColor", neonMaterial.neonColour * neonMaterial.flickerColourMultiplier);
					}
				}
				if (this.activeElevators.Count > 0)
				{
					for (int j = 0; j < this.activeElevators.Count; j++)
					{
						this.activeElevators[j].ElevatorUpdate();
					}
				}
				foreach (SessionData.TelevisionChannel televisionChannel in this.televisionChannels)
				{
					televisionChannel.ProcessTelevisionBroadcast();
				}
				if (InteriorControls.Instance.pulsingLightswitch != null && InteriorControls.Instance.pulsingLightswitchSwitch != null)
				{
					if (!this.lightswitchPulseMode && this.lightswitchPulse < 1f)
					{
						this.lightswitchPulse += Time.deltaTime;
						if (this.lightswitchPulse >= 1f)
						{
							this.lightswitchPulseMode = true;
						}
					}
					else if (this.lightswitchPulseMode && this.lightswitchPulse > 0f)
					{
						this.lightswitchPulse -= Time.deltaTime;
						if (this.lightswitchPulse <= 0f)
						{
							this.lightswitchPulseMode = false;
						}
					}
					Color color = Color.Lerp(Color.black, Color.white, this.lightswitchPulse);
					InteriorControls.Instance.pulsingLightswitch.SetColor("_EmissiveColor", color);
					InteriorControls.Instance.pulsingLightswitchSwitch.SetColor("_EmissiveColor", color);
				}
			}
			this.temperature = this.monthTempMultiplier * CityControls.Instance.weatherSettings.dayTempCurve.Evaluate(this.dayProgress) + CityControls.Instance.weatherSettings.NoRainModifier * (1f - this.currentRain) + CityControls.Instance.weatherSettings.NoWindModifier * (1f - this.currentWind) + CityControls.Instance.weatherSettings.NoSnowModifier * (1f - this.currentSnow) - 2.5f;
			if (AudioController.Instance.volumeChangingSounds.Count > 0)
			{
				AudioController.Instance.UpdateVolumeChanging();
			}
			if (AudioController.Instance.delayedSound.Count > 0)
			{
				for (int k = 0; k < AudioController.Instance.delayedSound.Count; k++)
				{
					AudioController.DelayedSoundInfo delayedSoundInfo = AudioController.Instance.delayedSound[k];
					delayedSoundInfo.delay -= Time.deltaTime * this.currentTimeMultiplier;
					if (delayedSoundInfo.delay <= 0f)
					{
						if (delayedSoundInfo.eventPreset.debug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Audio: Play delayed sound ",
								k.ToString(),
								"/",
								AudioController.Instance.delayedSound.Count.ToString(),
								": ",
								delayedSoundInfo.eventPreset.name
							}), 2);
						}
						if (delayedSoundInfo.is2D)
						{
							AudioController.Instance.Play2DSound(delayedSoundInfo.eventPreset, delayedSoundInfo.parameters, delayedSoundInfo.volumeOverride);
						}
						else
						{
							AudioController.Instance.PlayWorldOneShot(delayedSoundInfo.eventPreset, delayedSoundInfo.who, delayedSoundInfo.location, delayedSoundInfo.worldPosition, null, delayedSoundInfo.parameters, delayedSoundInfo.volumeOverride, delayedSoundInfo.additionalSources, delayedSoundInfo.forceIgnoreOcclusion, null, false);
						}
						AudioController.Instance.delayedSound.Remove(delayedSoundInfo);
						k--;
					}
				}
			}
			if (StatusController.Instance.drunkVision > 0f || StatusController.Instance.drunkControls > 0f)
			{
				this.drunkOscillatorX += Time.deltaTime * Toolbox.Instance.Rand(GameplayControls.Instance.drunkOscillationSpeed.x, GameplayControls.Instance.drunkOscillationSpeed.y, false);
				if (this.drunkOscillatorX >= 1f)
				{
					this.drunkOscillatorX -= 1f;
				}
				this.drunkOscillatorY += Time.deltaTime * Toolbox.Instance.Rand(GameplayControls.Instance.drunkOscillationSpeed.x, GameplayControls.Instance.drunkOscillationSpeed.y, false);
				if (this.drunkOscillatorY >= 1f)
				{
					this.drunkOscillatorY -= 1f;
				}
				this.drunkOscillation = new Vector2(GameplayControls.Instance.oscillatorX.Evaluate(this.drunkOscillatorX) * 12f, GameplayControls.Instance.oscillatorX.Evaluate(this.drunkOscillatorY) * 12f);
			}
			if (StatusController.Instance.shiverVision > 0f)
			{
				this.shiverOscillatorX += Time.deltaTime * Toolbox.Instance.Rand(GameplayControls.Instance.shiverOscillationSpeed.x, GameplayControls.Instance.shiverOscillationSpeed.y, false);
				if (this.shiverOscillatorX >= 1f)
				{
					this.shiverOscillatorX -= 1f;
				}
				this.shiverOscillatorY += Time.deltaTime * Toolbox.Instance.Rand(GameplayControls.Instance.shiverOscillationSpeed.x, GameplayControls.Instance.shiverOscillationSpeed.y, false);
				if (this.shiverOscillatorY >= 1f)
				{
					this.shiverOscillatorY -= 1f;
				}
				this.shiverProgress += Time.deltaTime * Toolbox.Instance.Rand(0.1f, 0.6f, false);
				if (this.shiverProgress >= 5f)
				{
					this.shiverProgress -= 5f;
				}
				float num6 = GameplayControls.Instance.shiverFluctuation.Evaluate(this.shiverProgress);
				this.shiverOscillation = new Vector2(GameplayControls.Instance.oscillatorX.Evaluate(this.shiverOscillatorX) * num6 * 0.5f, GameplayControls.Instance.oscillatorX.Evaluate(this.shiverOscillatorY) * num6 * 0.5f);
			}
			if (StatusController.Instance.drunkLensDistort != 0f || StatusController.Instance.headacheVision != 0f)
			{
				this.lensDistort.active = true;
				if (StatusController.Instance.drunkLensDistort != 0f)
				{
					this.drunkLensProgress += Time.deltaTime * Toolbox.Instance.Rand(GameplayControls.Instance.drunkLensDistortSpeed.x, GameplayControls.Instance.drunkLensDistortSpeed.x, false);
					if (this.drunkLensProgress >= 5f)
					{
						this.drunkLensProgress -= 5f;
					}
				}
				float num7 = 0f;
				if (StatusController.Instance.headacheVision != 0f)
				{
					this.headacheProgress += Time.deltaTime;
					if (this.headacheProgress >= 1.7f)
					{
						this.headacheProgress -= 1.7f;
					}
					num7 = GameplayControls.Instance.headacheFluctuation.Evaluate(this.headacheProgress) * StatusController.Instance.headacheVision;
				}
				this.lensDistort.intensity.value = GameplayControls.Instance.drunkLensDistortOscillator.Evaluate(this.drunkLensProgress) * StatusController.Instance.drunkLensDistort + num7;
			}
			else if (this.lensDistort.IsActive())
			{
				this.lensDistort.active = false;
			}
			for (int l = 0; l < GameplayController.Instance.closedBreakers.Count; l++)
			{
				Interactable interactable = GameplayController.Instance.closedBreakers[l];
				interactable.SetValue(Mathf.Clamp01(interactable.val + (float)this.gameTimePassedThisFrame / (GameplayControls.Instance.breakerResetTime * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.securityBreakerModifier)))));
			}
		}
		if (Game.Instance.timeLimited && !CutSceneController.Instance.cutSceneActive && !PopupMessageController.Instance.active && (!Game.Instance.startTimerAfterApartmentExit || Game.Instance.sandboxMode || ChapterController.Instance.currentPart >= 21 || Player.Instance.currentGameLocation != Player.Instance.home) && !MainMenuController.Instance.mainMenuActive)
		{
			if (this.gameTimeLimit > 0f)
			{
				if (Game.Instance.pauseTimerOnGamePause)
				{
					if (this.play)
					{
						this.gameTimeLimit -= Time.deltaTime;
					}
				}
				else
				{
					this.gameTimeLimit -= Time.deltaTime;
				}
			}
			this.gameTimeLimit = Mathf.Max(0f, this.gameTimeLimit);
			this.UpdateGameTimerText();
			if (this.gameTimeLimit <= 0f && !PopupMessageController.Instance.active)
			{
				PopupMessageController.Instance.OnLeftButton += this.EndDemo;
				PopupMessageController.Instance.PopupMessage("Time Limit Reached", true, false, "End Demo", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			}
		}
		if (this.pauseUnpauseDelay > 0)
		{
			this.pauseUnpauseDelay--;
		}
		if (this.desiredSceneProfile != this.currentSceneProfile)
		{
			foreach (CityControls.PPProfile ppprofile in CityControls.Instance.sceneProfileSetup)
			{
				if (ppprofile == this.desiredSceneProfile)
				{
					ppprofile.objectRef.SetActive(true);
					if (ppprofile.volume.priority != 1f)
					{
						ppprofile.volume.priority = 1f;
					}
					if (ppprofile.volume.weight < 1f)
					{
						ppprofile.volume.weight += Time.deltaTime * 0.4f;
						continue;
					}
					this.currentSceneProfile = this.desiredSceneProfile;
					using (List<CityControls.PPProfile>.Enumerator enumerator5 = CityControls.Instance.sceneProfileSetup.GetEnumerator())
					{
						while (enumerator5.MoveNext())
						{
							CityControls.PPProfile ppprofile2 = enumerator5.Current;
							if (ppprofile2 != this.desiredSceneProfile)
							{
								ppprofile2.volume.priority = -1f;
								ppprofile2.volume.weight = 0f;
								ppprofile2.objectRef.SetActive(false);
							}
						}
						break;
					}
				}
				if (ppprofile == this.currentSceneProfile)
				{
					ppprofile.objectRef.SetActive(true);
					if (ppprofile.volume.weight > 0f)
					{
						ppprofile.volume.weight -= Time.deltaTime;
					}
				}
				else
				{
					ppprofile.volume.priority = -1f;
				}
			}
		}
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x000A1158 File Offset: 0x0009F358
	public void UpdateGameTimerText()
	{
		float num = Mathf.Floor(this.gameTimeLimit / 60f);
		float num2 = Mathf.Floor((this.gameTimeLimit / 60f - num) * 60f) / 100f;
		InterfaceController.Instance.timerText.text = this.MinutesToClockString(num + num2, true);
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x000A11B0 File Offset: 0x0009F3B0
	public void EndDemo()
	{
		PopupMessageController.Instance.OnLeftButton -= this.EndDemo;
		RestartSafeController.Instance.newGameLoadCity = false;
		RestartSafeController.Instance.generateNew = false;
		RestartSafeController.Instance.loadSaveGame = false;
		Game.Log("CityGen: Restarting game due to time limit...", 2);
		AudioController.Instance.StopAllSounds();
		SceneManager.LoadScene("Main");
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x000A1213 File Offset: 0x0009F413
	public void ExecuteSyncPhysics(SessionData.PhysicsSyncType syncType)
	{
		if (syncType == SessionData.PhysicsSyncType.now)
		{
			Physics.SyncTransforms();
			return;
		}
		if (syncType == SessionData.PhysicsSyncType.onPlayerMovement)
		{
			Player.Instance.fps.syncTransforms = true;
			return;
		}
		if (syncType == SessionData.PhysicsSyncType.both)
		{
			Player.Instance.fps.syncTransforms = true;
			Physics.SyncTransforms();
		}
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x000A124C File Offset: 0x0009F44C
	public void ExecuteWeatherChange()
	{
		AudioController.Instance.PassWeatherParams();
		float num = this.currentRain;
		if (this.currentSnow >= 0.01f || this.currentRain >= 0.01f)
		{
			this.nearRainSheet.SetEnabled(true);
			this.farRainSheet.SetEnabled(true);
			PrecipitationParticleSystemController.Instance.SetEnabled(true);
		}
		else
		{
			this.nearRainSheet.SetEnabled(false);
			this.farRainSheet.SetEnabled(false);
			PrecipitationParticleSystemController.Instance.SetEnabled(false);
		}
		if (this.currentSnow >= 0.01f && this.currentRain <= 0.01f)
		{
			num = this.currentSnow;
			this.nearRainSheet.SetSnowMode(true, false);
			this.farRainSheet.SetSnowMode(true, false);
			PrecipitationParticleSystemController.Instance.SetSnowMode(true, false);
		}
		else
		{
			this.nearRainSheet.SetSnowMode(false, false);
			this.farRainSheet.SetSnowMode(false, false);
			PrecipitationParticleSystemController.Instance.SetSnowMode(false, false);
		}
		this.nearRainSheet.material.SetFloat("Vector1_9C48CC0E", Mathf.Lerp(this.nearRainAlpha1Threshold.x, this.nearRainAlpha1Threshold.y, num));
		this.nearRainSheet.material.SetFloat("Vector1_CAAA1509", Mathf.Lerp(this.nearRainAlpha2Threshold.x, this.nearRainAlpha2Threshold.y, num));
		this.nearRainSheet.material.SetFloat("Vector1_85EA96A5", Mathf.Lerp(this.nearRainSpeedThreshold.x, this.nearRainSpeedThreshold.y, num));
		this.nearRainSheet.material.SetVector("Vector2_E2361C8B", new Vector2(Mathf.Lerp(this.nearRainXTile1Threshold.x, this.nearRainXTile1Threshold.y, num), 27.2f));
		this.nearRainSheet.material.SetVector("Vector2_10A9AE04", new Vector2(Mathf.Lerp(this.nearRainXTile2Threshold.x, this.nearRainXTile2Threshold.y, num), 26.8f));
		this.farRainSheet.material.SetFloat("Vector1_9C48CC0E", Mathf.Lerp(this.farRainAlpha1Threshold.x, this.farRainAlpha1Threshold.y, num));
		this.farRainSheet.material.SetFloat("Vector1_CAAA1509", Mathf.Lerp(this.farRainAlpha2Threshold.x, this.farRainAlpha2Threshold.y, num));
		this.farRainSheet.material.SetFloat("Vector1_85EA96A5", Mathf.Lerp(this.farRainSpeedThreshold.x, this.nearRainSpeedThreshold.y, num));
		this.farRainSheet.material.SetVector("Vector2_E2361C8B", new Vector2(Mathf.Lerp(this.farRainXTile1Threshold.x, this.farRainXTile1Threshold.y, num), 27.2f));
		this.farRainSheet.material.SetVector("Vector2_10A9AE04", new Vector2(Mathf.Lerp(this.farRainXTile2Threshold.x, this.farRainXTile2Threshold.y, num), 26.8f));
		if (this.OnWeatherChange != null)
		{
			this.OnWeatherChange();
		}
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x000A1574 File Offset: 0x0009F774
	public void ExecuteWetnessChange()
	{
		Shader.SetGlobalFloat("_Rain", Mathf.Lerp(0f, 1f, Mathf.Clamp01(this.currentRain * 1f)));
		Shader.SetGlobalFloat("_CityWetness", Mathf.Lerp(0f, 1f, Mathf.Clamp01(Mathf.Pow(this.cityWetness, 1f))));
		Shader.SetGlobalFloat("_Snow", Mathf.Lerp(0f, 1f, Mathf.Clamp01(this.citySnow * 1f)));
		Shader.SetGlobalFloat("_CoatMask", Mathf.Lerp(0f, 1f, Mathf.Clamp01(this.cityWetness * 1f)));
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x000A162D File Offset: 0x0009F82D
	public void ExecuteWindChange()
	{
		Shader.SetGlobalFloat("_Wind", this.currentWind);
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x000A1640 File Offset: 0x0009F840
	public Material GetWeatherAffectedMaterial(Material inputMat, MeshRenderer inputRenderer)
	{
		SessionData.WetMaterial wetMaterial = null;
		if (!this.weatherMaterialsReference.TryGetValue(inputMat, ref wetMaterial))
		{
			return inputMat;
		}
		if (wetMaterial.instancedMat != null)
		{
			if (inputRenderer != null && !wetMaterial.affectedRenderers.Contains(inputRenderer))
			{
				wetMaterial.affectedRenderers.Add(inputRenderer);
			}
			return wetMaterial.instancedMat;
		}
		return inputMat;
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x000A169C File Offset: 0x0009F89C
	public void ExecuteLightningStrike()
	{
		if (!SessionData.Instance.play)
		{
			return;
		}
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		if (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive)
		{
			return;
		}
		if (SessionData.Instance.currentTimeSpeed != SessionData.TimeSpeed.normal)
		{
			return;
		}
		Vector2Int vector2Int;
		vector2Int..ctor(Toolbox.Instance.Rand(-2, Mathf.RoundToInt(CityData.Instance.citySize.x + 2f), false), Toolbox.Instance.Rand(-2, Mathf.RoundToInt(CityData.Instance.citySize.y + 2f), false));
		CityTile cityTile = null;
		NewBuilding newBuilding = null;
		float num = 0f;
		HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles.TryGetValue(vector2Int, ref cityTile);
		if (cityTile != null && cityTile.building != null)
		{
			newBuilding = cityTile.building;
			num = cityTile.building.preset.buildingHeight;
		}
		if (cityTile != null)
		{
			foreach (Vector2Int vector2Int2 in CityData.Instance.offsetArrayX8)
			{
				Vector2Int vector2Int3 = cityTile.cityCoord + vector2Int2;
				CityTile cityTile2 = null;
				if (HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles.TryGetValue(vector2Int3, ref cityTile2) && cityTile2.building != null && cityTile2.building.preset.buildingHeight > num)
				{
					newBuilding = cityTile2.building;
					num = cityTile2.building.preset.buildingHeight;
				}
			}
		}
		Vector3 vector = Vector3.zero;
		if (newBuilding != null)
		{
			vector = newBuilding.buildingModelBase.transform.TransformPoint(newBuilding.preset.lightningRodLocalPos);
		}
		else
		{
			vector = CityData.Instance.CityTileToRealpos(vector2Int + new Vector2(Toolbox.Instance.Rand(-0.5f, 0.5f, false), Toolbox.Instance.Rand(-0.5f, 0.5f, false))) + new Vector3(0f, -12.5f, 0f);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.lightningStrike, PrefabControls.Instance.cityContainer.transform);
		gameObject.transform.position = vector;
		LightningStrikeController component = gameObject.GetComponent<LightningStrikeController>();
		component.startPoint.transform.position = new Vector3(component.startPoint.transform.position.x + Toolbox.Instance.Rand(-20f, 20f, false), 250f, component.startPoint.transform.position.z + Toolbox.Instance.Rand(-20f, 20f, false));
		LightController componentInChildren = gameObject.GetComponentInChildren<LightController>();
		componentInChildren.Setup(null, null, null, componentInChildren.preset, -1, null);
		float num2 = Vector3.Distance(CameraController.Instance.transform.position, vector);
		float num3 = Vector2.Distance(new Vector2(CameraController.Instance.transform.position.x, CameraController.Instance.transform.position.z), new Vector2(vector.x, vector.z));
		List<AudioController.FMODParam> list = new List<AudioController.FMODParam>();
		AudioController.FMODParam fmodparam = new AudioController.FMODParam
		{
			name = "ThunderDistance"
		};
		if (num3 >= AudioDebugging.Instance.thunderDistanceThreshold)
		{
			fmodparam.value = 1f;
		}
		else
		{
			fmodparam.value = 0f;
		}
		list.Add(fmodparam);
		AudioController.FMODParam fmodparam2 = new AudioController.FMODParam
		{
			name = "Interior"
		};
		if (Player.Instance.currentRoom != null && Player.Instance.currentRoom.IsOutside())
		{
			fmodparam2.value = 0f;
		}
		else
		{
			fmodparam2.value = 1f;
		}
		list.Add(fmodparam2);
		Game.Log(string.Concat(new string[]
		{
			"Audio: Thunder: Distance = ",
			num3.ToString(),
			"/",
			AudioDebugging.Instance.thunderDistanceThreshold.ToString(),
			": ",
			fmodparam.value.ToString(),
			" Interior: ",
			fmodparam2.value.ToString()
		}), 2);
		float delay = num2 / AudioController.Instance.speedOfSound;
		AudioController.Instance.PlayOneShotDelayed(delay, AudioControls.Instance.thunder, null, null, vector, list, 1f, null, false);
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x000A1B20 File Offset: 0x0009FD20
	public void SetSceneVisuals(float newDecimalClock)
	{
		float num = newDecimalClock / 24f;
		if (this.skyboxGradientIndex + 1 < CityControls.Instance.weatherSettings.skyboxGradientGrading.Count)
		{
			if (newDecimalClock >= CityControls.Instance.weatherSettings.skyboxGradientGrading[this.skyboxGradientIndex + 1].time)
			{
				this.skyboxGradientIndex++;
				if (this.skyboxGradientIndex >= CityControls.Instance.weatherSettings.skyboxGradientGrading.Count)
				{
					this.skyboxGradientIndex = 0;
				}
				this.fromSkyboxColours = CityControls.Instance.weatherSettings.skyboxGradientGrading[this.skyboxGradientIndex];
				if (this.skyboxGradientIndex + 1 >= CityControls.Instance.weatherSettings.skyboxGradientGrading.Count)
				{
					this.toSkyboxColours = CityControls.Instance.weatherSettings.skyboxGradientGrading[0];
				}
				else
				{
					this.toSkyboxColours = CityControls.Instance.weatherSettings.skyboxGradientGrading[this.skyboxGradientIndex + 1];
				}
			}
		}
		else if (newDecimalClock < this.fromSkyboxColours.time && newDecimalClock >= CityControls.Instance.weatherSettings.skyboxGradientGrading[0].time)
		{
			this.skyboxGradientIndex = 0;
			this.fromSkyboxColours = CityControls.Instance.weatherSettings.skyboxGradientGrading[this.skyboxGradientIndex];
			this.toSkyboxColours = CityControls.Instance.weatherSettings.skyboxGradientGrading[this.skyboxGradientIndex + 1];
		}
		float num2 = this.toSkyboxColours.time - this.fromSkyboxColours.time;
		if (this.toSkyboxColours.time < this.fromSkyboxColours.time)
		{
			num2 = this.toSkyboxColours.time + 24f - this.fromSkyboxColours.time;
		}
		float num3 = (newDecimalClock - this.fromSkyboxColours.time) / num2;
		this.gradientSky.top.Override(Color.Lerp(this.fromSkyboxColours.ambientLightTop * CityControls.Instance.weatherSettings.ambientLightMultiplier, this.toSkyboxColours.ambientLightTop * CityControls.Instance.weatherSettings.ambientLightMultiplier, num3));
		this.gradientSky.middle.Override(Color.Lerp(this.fromSkyboxColours.ambientLightMiddle * CityControls.Instance.weatherSettings.ambientLightMultiplier, this.toSkyboxColours.ambientLightMiddle * CityControls.Instance.weatherSettings.ambientLightMultiplier, num3));
		this.gradientSky.bottom.Override(Color.Lerp(this.fromSkyboxColours.ambientLightBottom * CityControls.Instance.weatherSettings.ambientLightMultiplier, this.toSkyboxColours.ambientLightBottom * CityControls.Instance.weatherSettings.ambientLightMultiplier, num3));
		float num4 = CityControls.Instance.weatherSettings.fogAttenuationCurve.Evaluate(this.currentFog);
		this.volFog.meanFreePath.Override(num4);
		this.volFog.depthExtent.Override(CityControls.Instance.weatherSettings.volumetricFogDistanceCurve.Evaluate(this.currentFog));
		Color color = Color.Lerp(this.fromSkyboxColours.fogAlbedo * CityControls.Instance.weatherSettings.fogColourMultiplier, this.toSkyboxColours.fogAlbedo * CityControls.Instance.weatherSettings.fogColourMultiplier, num3);
		this.volFog.albedo.Override(color);
		this.volFog.color.Override(Color.Lerp(this.fromSkyboxColours.fogColour, this.toSkyboxColours.fogColour, num3));
		CityControls.Instance.seaMaterial.SetColor("Color_1A9C35BA", Color.Lerp(this.fromSkyboxColours.seaEmission, this.toSkyboxColours.seaEmission, num3));
		CityControls.Instance.smokeMaterial.SetColor("_EmissiveColor", Color.Lerp(this.fromSkyboxColours.smokeEmission, this.toSkyboxColours.smokeEmission, num3));
		CameraController.Instance.hdrpCam.backgroundColorHDR = Color.Lerp(this.fromSkyboxColours.skyColour * CityControls.Instance.weatherSettings.skyColourMultiplier, this.toSkyboxColours.skyColour * CityControls.Instance.weatherSettings.skyColourMultiplier, num3);
		CityControls.Instance.exteriorAmbientHDRP.color = Color.Lerp(this.fromSkyboxColours.ambientLightingColour * CityControls.Instance.weatherSettings.ambientLightMultiplier, this.toSkyboxColours.ambientLightingColour * CityControls.Instance.weatherSettings.ambientLightMultiplier, num3);
		CityControls.Instance.interiorAmbientHDRP.color = Color.Lerp(this.fromSkyboxColours.ambientLightingColour * CityControls.Instance.weatherSettings.ambientLightMultiplier, this.toSkyboxColours.ambientLightingColour * CityControls.Instance.weatherSettings.ambientLightMultiplier, num3);
		CityControls.Instance.exteriorAmbientHDRP.intensity = CityControls.Instance.weatherSettings.exteriorAmbientIntensityCurve.Evaluate(num) * CityControls.Instance.weatherSettings.ambientExteriorBooster;
		CityControls.Instance.interiorAmbientHDRP.intensity = CityControls.Instance.weatherSettings.interiorAmbientIntensityCurve.Evaluate(num) * CityControls.Instance.weatherSettings.ambientInteriorBooster;
		CityControls.Instance.hdrpLightSunData.intensity = CityControls.Instance.weatherSettings.daytimeSunIntensityCurve.Evaluate(num) * CityControls.Instance.weatherSettings.sunIntensityBooster;
		CityControls.Instance.hdrpLightSunData.volumetricDimmer = CityControls.Instance.weatherSettings.sunVolumetricDimmer.Evaluate(num);
		CityControls.Instance.hdrpLightSunData.volumetricShadowDimmer = CityControls.Instance.weatherSettings.sunVolumetricShadowDimmer.Evaluate(num);
		CityControls.Instance.sunLight.shadowStrength = Mathf.Clamp01(CityControls.Instance.weatherSettings.sunShadowStrengthCurve.Evaluate(num));
		if (CityControls.Instance.ships1 != null)
		{
			CityControls.Instance.ships1.eulerAngles = new Vector3(0f, CityControls.Instance.ships1.eulerAngles.y + Time.deltaTime * this.currentTimeMultiplier * 0.1f, 0f);
		}
		if (CityControls.Instance.skylineMaterial != null)
		{
			CityControls.Instance.skylineMaterial.SetColor("_EmissiveColor", CityControls.Instance.weatherSettings.skylineEmissionColor * CityControls.Instance.weatherSettings.skylineEmissionCurve.Evaluate(num));
		}
		float num5 = 0f;
		if (newDecimalClock >= CityControls.Instance.weatherSettings.sunRiseHour && newDecimalClock < CityControls.Instance.weatherSettings.sunSetHour)
		{
			float num6 = (newDecimalClock - CityControls.Instance.weatherSettings.sunRiseHour) / (CityControls.Instance.weatherSettings.sunSetHour - CityControls.Instance.weatherSettings.sunRiseHour);
			num5 = Mathf.Lerp(90f, 270f, num6);
			if (num6 <= 0.5f)
			{
				CityControls.Instance.sunLight.color = Color.Lerp(CityControls.Instance.weatherSettings.morningSunColour, CityControls.Instance.weatherSettings.middaySunColour, num6 * 2f);
			}
			else
			{
				CityControls.Instance.sunLight.color = Color.Lerp(CityControls.Instance.weatherSettings.middaySunColour, CityControls.Instance.weatherSettings.eveningSunColour, num6 * 0.5f);
			}
			this.sunShadowFrameCounter++;
			if (this.sunShadowFrameCounter >= Game.Instance.sunShadowUpdateFrequency)
			{
				CityControls.Instance.hdrpLightSunData.RequestShadowMapRendering();
				this.sunShadowFrameCounter = 0;
			}
		}
		else if (newDecimalClock < CityControls.Instance.weatherSettings.sunRiseHour)
		{
			float num7 = newDecimalClock / CityControls.Instance.weatherSettings.sunRiseHour;
			num5 = Mathf.Lerp(0f, 90f, num7);
		}
		else if (newDecimalClock >= CityControls.Instance.weatherSettings.sunSetHour)
		{
			num5 = Mathf.Lerp(270f, 360f, num);
		}
		num5 -= 90f;
		CityControls.Instance.sunLight.transform.eulerAngles = new Vector3(num5, CityControls.Instance.angleOfSun, 0f);
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x000A23A4 File Offset: 0x000A05A4
	public void SetEnablePause(bool val)
	{
		this.enableUserPause = val;
		if (!this.enableUserPause && !this.play)
		{
			this.ResumeGame();
		}
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x000A23C4 File Offset: 0x000A05C4
	public void ParseTimeData(float newTime, out float decimalHourOut, out int dayIntOut, out int dateIntOut, out int monthIntOut, out int yearIntOut, out SessionData.WeekDay dayEnumOut, out SessionData.Month monthEnumOut, out int leapCycleOut)
	{
		decimalHourOut = newTime;
		dayIntOut = GameplayControls.Instance.dayZero;
		dateIntOut = 0;
		monthIntOut = 0;
		yearIntOut = GameplayControls.Instance.publicYearZero + GameplayControls.Instance.startingYear;
		dayEnumOut = SessionData.WeekDay.monday;
		monthEnumOut = SessionData.Month.jan;
		leapCycleOut = GameplayControls.Instance.yearZeroLeapYearCycle;
		while (decimalHourOut >= 24f)
		{
			decimalHourOut -= 24f;
			dayIntOut++;
			dateIntOut++;
			if (dayIntOut >= 7)
			{
				dayIntOut -= 7;
			}
			dayEnumOut = this.WeekdayFromInt(dayIntOut);
			int num = this.daysInMonths[monthIntOut];
			if (leapCycleOut == 3 && monthIntOut == 1)
			{
				num++;
			}
			if (dateIntOut >= num)
			{
				dateIntOut -= num;
				monthIntOut++;
				if (monthIntOut >= 12)
				{
					monthIntOut -= 12;
					yearIntOut++;
					for (leapCycleOut++; leapCycleOut >= 4; leapCycleOut -= 4)
					{
					}
				}
				monthEnumOut = this.MonthFromInt(this.monthInt);
			}
		}
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x000A24C8 File Offset: 0x000A06C8
	public void ParseTimeData(float newTime, out float decimalHourOut, out int dayIntOut, out int dateIntOut, out int monthIntOut, out int yearIntOut)
	{
		decimalHourOut = newTime;
		dayIntOut = 0;
		dateIntOut = 0;
		monthIntOut = 0;
		yearIntOut = GameplayControls.Instance.publicYearZero + GameplayControls.Instance.startingYear;
		SessionData.WeekDay weekDay = SessionData.WeekDay.monday;
		SessionData.Month month = SessionData.Month.jan;
		int yearZeroLeapYearCycle = GameplayControls.Instance.yearZeroLeapYearCycle;
		this.ParseTimeData(newTime, out decimalHourOut, out dayIntOut, out dateIntOut, out monthIntOut, out yearIntOut, out weekDay, out month, out yearZeroLeapYearCycle);
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x000A2520 File Offset: 0x000A0720
	public void ParseTimeData(float newTime, out float decimalHourOut, out SessionData.WeekDay dayEnumOut, out int dateIntOut, out SessionData.Month monthEnumOut, out int yearIntOut)
	{
		decimalHourOut = newTime;
		int num = 0;
		dateIntOut = 0;
		int num2 = 0;
		yearIntOut = GameplayControls.Instance.publicYearZero + GameplayControls.Instance.startingYear;
		dayEnumOut = SessionData.WeekDay.monday;
		monthEnumOut = SessionData.Month.jan;
		int yearZeroLeapYearCycle = GameplayControls.Instance.yearZeroLeapYearCycle;
		this.ParseTimeData(newTime, out decimalHourOut, out num, out dateIntOut, out num2, out yearIntOut, out dayEnumOut, out monthEnumOut, out yearZeroLeapYearCycle);
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x000A2578 File Offset: 0x000A0778
	public float ParseGameTime(float decimalHourIn, int dateIntIn, int monthIntIn, int yearIntIn, out int dayCount, out int leapYear)
	{
		float num = decimalHourIn;
		int i = 0;
		int j = 0;
		int k = GameplayControls.Instance.publicYearZero + GameplayControls.Instance.startingYear;
		dayCount = GameplayControls.Instance.dayZero;
		leapYear = GameplayControls.Instance.yearZeroLeapYearCycle;
		while (k < yearIntIn)
		{
			int l = 0;
			while (l < 12)
			{
				int num2 = this.daysInMonths[l];
				if (leapYear == 3 && l == 1)
				{
					num2++;
				}
				while (i < num2)
				{
					i++;
					num += 24f;
					dayCount++;
					if (dayCount >= 7)
					{
						dayCount -= 7;
					}
				}
				l++;
				i = 0;
			}
			k++;
			leapYear++;
			if (leapYear >= 4)
			{
				leapYear -= 4;
			}
		}
		while (j < monthIntIn)
		{
			int num3 = 0;
			int num4 = this.daysInMonths[num3];
			if (leapYear == 3 && num3 == 1)
			{
				num4++;
			}
			while (i < num4)
			{
				i++;
				num += 24f;
				dayCount++;
				if (dayCount >= 7)
				{
					dayCount -= 7;
				}
			}
			num3++;
			i = 0;
		}
		while (i < dateIntIn)
		{
			num += 24f;
			i++;
			dayCount++;
			if (dayCount >= 7)
			{
				dayCount -= 7;
			}
		}
		return num;
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x000A26BB File Offset: 0x000A08BB
	public float FloatDecimal24H(float time)
	{
		while (time < 0f)
		{
			time += 24f;
		}
		time %= 24f;
		return Mathf.Clamp(time, 0f, 24f);
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x000A26EC File Offset: 0x000A08EC
	public float FloatMinutes24H(float newTime)
	{
		newTime = this.FloatDecimal24H(newTime);
		float num = Mathf.Floor(newTime);
		float num2 = Mathf.Floor((newTime - num) * 60f) / 100f;
		return num + num2;
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x000A2724 File Offset: 0x000A0924
	public float FloatMinutes12H(float newTime)
	{
		newTime = this.FloatDecimal24H(newTime);
		float num = Mathf.Floor(newTime);
		if (num > 12f)
		{
			num -= 12f;
		}
		float num2 = Mathf.Floor((newTime - Mathf.Floor(newTime)) * 60f) / 100f;
		return num + num2;
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x000A2770 File Offset: 0x000A0970
	public string DecimalToClockString(float newTime, bool useZeroHoursMethod)
	{
		float formatted = this.FloatMinutes24H(newTime);
		return this.MinutesToClockString(formatted, useZeroHoursMethod);
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x000A2790 File Offset: 0x000A0990
	public string DecimalToTimeLengthString(float newTime)
	{
		float num = this.FloatMinutes24H(newTime);
		int num2 = Mathf.FloorToInt(num);
		string text = Mathf.CeilToInt((num - (float)num2) * 100f).ToString();
		return num2.ToString() + ":" + text;
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x000A27D4 File Offset: 0x000A09D4
	public string GameTimeToClock24String(float newGameTime, bool useZeroHoursMethod)
	{
		float newTime = 0f;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		this.ParseTimeData(newGameTime, out newTime, out num, out num2, out num3, out num4);
		float formatted = this.FloatMinutes24H(newTime);
		return this.MinutesToClockString(formatted, useZeroHoursMethod);
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x000A2814 File Offset: 0x000A0A14
	public string GameTimeToClock12String(float newGameTime, bool useZeroHoursMethod)
	{
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		this.ParseTimeData(newGameTime, out num, out num2, out num3, out num4, out num5);
		float formatted = this.FloatMinutes12H(num);
		string text = Strings.Get("ui.interface", "AM", Strings.Casing.asIs, false, false, false, null);
		if (num > 12f)
		{
			text = Strings.Get("ui.interface", "PM", Strings.Casing.asIs, false, false, false, null);
		}
		return this.MinutesToClockString(formatted, useZeroHoursMethod) + text;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x000A2890 File Offset: 0x000A0A90
	public string MinutesToClockString(float formatted, bool useZeroHoursMethod)
	{
		int num = Mathf.FloorToInt(formatted);
		string text = num.ToString();
		if (useZeroHoursMethod && text.Length == 1)
		{
			text = "0" + text;
		}
		string text2 = Mathf.RoundToInt((formatted - (float)num) * 100f).ToString();
		if (text2.Length == 1)
		{
			text2 = "0" + text2;
		}
		return text + ":" + text2;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x000A28FD File Offset: 0x000A0AFD
	public string CurrentTimeString(bool useZeroHoursMethod, bool use12HourClock = false)
	{
		if (!use12HourClock)
		{
			return this.MinutesToClockString(this.formattedClock, useZeroHoursMethod);
		}
		return this.GameTimeToClock12String(this.gameTime, useZeroHoursMethod);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x000A2920 File Offset: 0x000A0B20
	public string ShortDateString(float newGameTime, bool shortenYear)
	{
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		this.ParseTimeData(newGameTime, out num, out num2, out num3, out num4, out num5);
		string text = (num4 + 1).ToString();
		if (text.Length == 1)
		{
			text = "0" + text;
		}
		string text2 = (num3 + 1).ToString();
		if (text2.Length == 1)
		{
			text2 = "0" + text2;
		}
		string text3 = num5.ToString();
		if (shortenYear && text3.Length > 2)
		{
			text3 = text3.Substring(text3.Length - 2);
		}
		return string.Concat(new string[]
		{
			text,
			"/",
			text2,
			"/",
			text3
		});
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x000A29EB File Offset: 0x000A0BEB
	public string CurrentShortDateString(bool shortenYear)
	{
		return this.ShortDateString(this.gameTime, shortenYear);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x000A29FC File Offset: 0x000A0BFC
	public string LongDateString(float newGameTime, bool includeDay, bool shortenDay, bool includeMonth, bool shortenMonth, bool includeDate, bool includeYear, bool shortenYear, bool useCommas)
	{
		float num = 0f;
		int weekInt = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		this.ParseTimeData(newGameTime, out num, out weekInt, out num2, out num3, out num4);
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		if (includeDay)
		{
			text = Strings.Get("ui.interface", this.WeekdayFromInt(weekInt).ToString(), Strings.Casing.asIs, false, false, false, null);
			if (shortenDay && Game.Instance.language == "English")
			{
				text = text.Substring(0, Mathf.Min(3, text.Length));
			}
			if (includeMonth || includeDate || includeYear)
			{
				if (useCommas)
				{
					text += ",";
				}
				text += " ";
			}
		}
		if (includeMonth)
		{
			text2 = Strings.Get("ui.interface", this.MonthFromInt(num3).ToString(), Strings.Casing.asIs, false, false, false, null);
			if (shortenMonth && Game.Instance.language == "English")
			{
				text2 = text2.Substring(0, Mathf.Min(3, text2.Length));
			}
			text2 += " ";
		}
		if (includeDate)
		{
			text3 = Toolbox.Instance.GetNumbericalStringReference(num2 + 1);
			if (includeYear)
			{
				if (useCommas)
				{
					text3 += ",";
				}
				text3 += " ";
			}
		}
		if (includeYear)
		{
			text4 = num4.ToString();
			if (shortenYear && text4.Length > 2)
			{
				text4 = text4.Substring(text4.Length - 2);
			}
		}
		string text5 = text + text2 + text3 + text4;
		if (text5.Length > 1 && text5.Substring(text5.Length - 1, 1) == " ")
		{
			text5 = text5.Substring(0, text5.Length - 1);
		}
		if (useCommas && text5.Length > 1 && text5.Substring(text5.Length - 1, 1) == ",")
		{
			text5 = text5.Substring(0, text5.Length - 1);
		}
		return text5;
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x000A2C24 File Offset: 0x000A0E24
	public string CurrentLongDateString(bool includeDay, bool shortenDay, bool includeMonth, bool shortenMonth, bool includeDate, bool includeYear, bool shortenYear, bool useCommas)
	{
		return this.LongDateString(this.gameTime, includeDay, shortenDay, includeMonth, shortenMonth, includeDate, includeYear, shortenYear, useCommas);
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x000A2C4C File Offset: 0x000A0E4C
	public string TimeString(float newGameTime, bool useZeroHoursMethod)
	{
		float newTime = 0f;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		this.ParseTimeData(newGameTime, out newTime, out num, out num2, out num3, out num4);
		return this.DecimalToClockString(newTime, useZeroHoursMethod);
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x000A2C84 File Offset: 0x000A0E84
	public string TimeStringOnDay(float newGameTime, bool useZeroHoursMethod, bool shortenDay)
	{
		float newTime = 0f;
		int weekInt = 0;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		this.ParseTimeData(newGameTime, out newTime, out weekInt, out num, out num2, out num3);
		string text = this.DecimalToClockString(newTime, useZeroHoursMethod);
		string text2 = Strings.Get("ui.interface", this.WeekdayFromInt(weekInt).ToString(), Strings.Casing.asIs, false, false, false, null);
		if (shortenDay && Game.Instance.language == "English" && text2.Length > 3)
		{
			text2 = text2.Substring(0, 3);
		}
		return text + " " + text2;
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x000A2D1A File Offset: 0x000A0F1A
	public string TimeAndDate(float newGameTime, bool useZeroHoursMethod, bool shortenDay, bool shortenYear)
	{
		return this.TimeStringOnDay(newGameTime, useZeroHoursMethod, shortenDay) + " " + this.ShortDateString(newGameTime, shortenYear);
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x000A2D38 File Offset: 0x000A0F38
	public string OnDay(int newDay, bool shortenDay)
	{
		string text = Strings.Get("ui.interface", this.WeekdayFromInt(newDay).ToString(), Strings.Casing.asIs, false, false, false, null);
		if (shortenDay && Game.Instance.language == "English" && text.Length > 3)
		{
			text = text.Substring(0, 3);
		}
		return Strings.Get("ui.interface", "on", Strings.Casing.asIs, false, false, false, null) + " " + text;
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x000A2DB4 File Offset: 0x000A0FB4
	public float GetNextOrPreviousGameTimeForThisHour(List<SessionData.WeekDay> days, float startHour, float endHour)
	{
		return this.GetNextOrPreviousGameTimeForThisHour(this.gameTime, this.decimalClock, this.day, days, startHour, endHour);
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x000A2DD4 File Offset: 0x000A0FD4
	public float GetNextOrPreviousGameTimeForThisHour(float newTime, float newDecimalClock, SessionData.WeekDay newDay, List<SessionData.WeekDay> days, float startHour, float endHour)
	{
		float num = newTime - newDecimalClock;
		if (endHour >= startHour)
		{
			if (days.Contains(newDay))
			{
				if (newDecimalClock >= startHour && newDecimalClock <= endHour)
				{
					float num2 = newDecimalClock - startHour;
					return newTime - num2;
				}
				if (newDecimalClock < startHour)
				{
					return num + startHour;
				}
			}
		}
		else if (endHour < startHour)
		{
			if (newDecimalClock >= endHour)
			{
				return num + startHour;
			}
			SessionData.WeekDay weekDay;
			if (this.day == SessionData.WeekDay.monday)
			{
				weekDay = SessionData.WeekDay.sunday;
			}
			else
			{
				weekDay = this.day - 1;
			}
			if (days.Contains(weekDay))
			{
				return num - (24f - startHour);
			}
		}
		int num3 = (int)(newDay + 1);
		if (num3 >= 7)
		{
			num3 = 0;
		}
		SessionData.WeekDay weekDay2 = (SessionData.WeekDay)num3;
		float num4 = num + 24f;
		int num5 = 8;
		while (!days.Contains(weekDay2) && num5 > 0)
		{
			if (weekDay2 == SessionData.WeekDay.sunday)
			{
				weekDay2 = SessionData.WeekDay.monday;
			}
			else
			{
				weekDay2 = this.day + 1;
			}
			num4 += 24f;
			num5--;
		}
		return num4 + startHour;
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x000A2E9F File Offset: 0x000A109F
	public float GetTimeDifference(float time1, float time2)
	{
		return time2 - time1;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x000A2EA4 File Offset: 0x000A10A4
	public bool CompareTimes(float time1, float time2)
	{
		float num = Toolbox.Instance.RoundToPlaces(time1, 2);
		float num2 = Toolbox.Instance.RoundToPlaces(time2, 2);
		return num == num2;
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x000A2ED0 File Offset: 0x000A10D0
	public SessionData.WeekDay WeekdayFromInt(int weekInt)
	{
		return (SessionData.WeekDay)weekInt;
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x000A2ED0 File Offset: 0x000A10D0
	public SessionData.Month MonthFromInt(int monthInt)
	{
		return (SessionData.Month)monthInt;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x000A2ED4 File Offset: 0x000A10D4
	public void SetWeather(float newRain, float newWind, float newSnow, float newLightning, float newFog, float newTransitionSpeed = 0.1f, bool updateInstantly = false)
	{
		if (Game.Instance.disableSnow)
		{
			this.desiredSnow = 0f;
		}
		else
		{
			this.desiredSnow = Mathf.Clamp01(newSnow);
		}
		if (this.desiredSnow <= 0.01f)
		{
			this.desiredRain = Mathf.Clamp01(newRain);
		}
		else
		{
			this.desiredRain = 0f;
		}
		this.desiredWind = Mathf.Clamp01(newWind);
		this.desiredLightning = Mathf.Clamp01(newLightning);
		this.desiredFog = Mathf.Clamp01(newFog);
		this.transitionSpeed = newTransitionSpeed;
		if (updateInstantly)
		{
			this.currentFog = this.desiredFog;
			this.currentWind = this.desiredWind;
			this.currentRain = this.desiredRain;
			this.currentSnow = this.desiredSnow;
			this.currentLightning = this.desiredLightning;
			this.ExecuteWeatherChange();
		}
		Game.Log(string.Concat(new string[]
		{
			"Gameplay: Set weather: Rain: ",
			this.desiredRain.ToString(),
			" Wind: ",
			this.desiredWind.ToString(),
			" Lightning: ",
			this.desiredLightning.ToString(),
			" Snow: ",
			this.desiredSnow.ToString()
		}), 2);
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x000A300C File Offset: 0x000A120C
	public void UpdateUIClock()
	{
		if (this.newWatchTimeText != null)
		{
			if (Player.Instance.setAlarmMode)
			{
				if (this.gameTime > Player.Instance.alarm)
				{
					Player.Instance.alarm = this.gameTime;
				}
				this.newWatchTimeText.text = this.DecimalToClockString(this.FloatMinutes24H(Player.Instance.alarm), true);
				return;
			}
			this.newWatchTimeText.text = this.DecimalToClockString(this.decimalClock, true);
		}
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x000A3090 File Offset: 0x000A1290
	public void UpdateUIDay()
	{
		string text = string.Empty;
		if (this.newWatchDateText != null)
		{
			float num = 0f;
			int weekInt = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (Player.Instance.setAlarmMode)
			{
				if (this.gameTime > Player.Instance.alarm)
				{
					Player.Instance.alarm = this.gameTime;
				}
				this.ParseTimeData(Player.Instance.alarm, out num, out weekInt, out num2, out num3, out num4);
			}
			else
			{
				this.ParseTimeData(this.gameTime, out num, out weekInt, out num2, out num3, out num4);
			}
			string empty = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string empty2 = string.Empty;
			string text4 = Strings.Get("ui.interface", this.WeekdayFromInt(weekInt).ToString(), Strings.Casing.asIs, false, false, false, null) + "\n";
			text2 = Strings.Get("ui.interface", this.MonthFromInt(num3).ToString() + "_short", Strings.Casing.asIs, false, false, false, null);
			text2.Substring(Mathf.Max(0, text2.Length - 3));
			text2 += " ";
			text3 = (num2 + 1).ToString();
			if (text3.Length > 0 && text3.Substring(text3.Length - 1, 1) == "1")
			{
				text3 += Strings.Get("ui.interface", "st", Strings.Casing.asIs, false, false, false, null);
			}
			else if (text3.Length > 0 && text3.Substring(text3.Length - 1, 1) == "2")
			{
				text3 += Strings.Get("ui.interface", "nd", Strings.Casing.asIs, false, false, false, null);
			}
			else if (text3.Length > 0 && text3.Substring(text3.Length - 1, 1) == "3")
			{
				text3 += Strings.Get("ui.interface", "rd", Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				text3 += Strings.Get("ui.interface", "th", Strings.Casing.asIs, false, false, false, null);
			}
			text3 += " ";
			text = text4 + text2 + text3;
			if (text3.Length > 1 && text.Substring(text3.Length - 1, 1) == " ")
			{
				text = text.Substring(0, text3.Length - 1);
			}
			this.newWatchDateText.text = text;
		}
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x000A331F File Offset: 0x000A151F
	public void TogglePause(bool openDesktopMode = true)
	{
		if (this.play)
		{
			this.PauseGame(true, false, openDesktopMode);
			return;
		}
		this.ResumeGame();
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x000A333C File Offset: 0x000A153C
	public void PauseGame(bool showPauseText, bool delayOverride = false, bool openDesktopMode = true)
	{
		if (this.play)
		{
			this.autoPauseTimer = 0f;
			if (!delayOverride && this.pauseUnpauseDelay > 0)
			{
				return;
			}
			this.pauseUnpauseDelay = 1;
			if (InterfaceController.Instance.timeText != null)
			{
				InterfaceController.Instance.timeText.text = SessionData.Instance.TimeAndDate(SessionData.Instance.gameTime, true, true, true);
			}
			Game.Log("Player: Pause Game", 2);
			if (this.pauseText != null)
			{
				this.pauseText.text = Strings.Get("ui.interface", "Paused", Strings.Casing.asIs, false, false, false, null);
			}
			if (InterfaceControls.Instance.fastForwardArrow != null)
			{
				InterfaceControls.Instance.fastForwardArrow.gameObject.SetActive(false);
			}
			this.play = false;
			InterfaceController.Instance.UpdateDOF();
			if (CitizenBehaviour.Instance != null)
			{
				CitizenBehaviour.Instance.GameSpeedChange();
			}
			if (this.pauseText != null)
			{
				if (showPauseText)
				{
					this.pauseText.enabled = true;
					this.pauseLensFlare.gameObject.SetActive(true);
				}
				else
				{
					this.pauseText.enabled = false;
					this.pauseLensFlare.gameObject.SetActive(false);
				}
			}
			if (this.OnPauseChange != null)
			{
				this.OnPauseChange(openDesktopMode);
			}
			InterfaceController.Instance.SetCrosshairVisible(false);
			InterfaceController.Instance.DisplayLocationText(4f, true);
		}
		if (this.interfaceActiveAudio == null)
		{
			this.interfaceActiveAudio = AudioController.Instance.Play2DLooping(AudioControls.Instance.interfaceEvent, null, 1f);
		}
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x000A34D8 File Offset: 0x000A16D8
	public void ResumeGame()
	{
		if (!this.play)
		{
			InteractionController.Instance.inputCooldown = 0.1f;
			if (Game.Instance.disableCaseBoardClose)
			{
				return;
			}
			if (this.pauseUnpauseDelay > 0)
			{
				return;
			}
			this.pauseUnpauseDelay = 1;
			if (this.isDecorEdit)
			{
				this.isDecorEdit = false;
				InterfaceController.Instance.UpdateDOF();
			}
			if (BioScreenController.Instance.isOpen && BioScreenController.Instance.openedFromPause)
			{
				BioScreenController.Instance.SetInventoryOpen(false, false, true);
			}
			Game.Log("Player: Resume Game", 2);
			this.play = true;
			if (UpgradesController.Instance.playSyncDiskInstallAudio)
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.syncDiskInstallStatus, null, 1f);
				UpgradesController.Instance.playSyncDiskInstallAudio = false;
			}
			InterfaceController.Instance.SetCrosshairVisible(true);
			if (CitizenBehaviour.Instance != null)
			{
				CitizenBehaviour.Instance.GameSpeedChange();
			}
			InterfaceController.Instance.UpdateDOF();
			if (!SessionData.Instance.isFloorEdit)
			{
				this.pauseText.enabled = false;
				this.pauseLensFlare.SetActive(false);
			}
			for (int i = 0; i < InterfaceController.Instance.activeWindows.Count; i++)
			{
				InfoWindow infoWindow = InterfaceController.Instance.activeWindows[i];
				if (infoWindow.isWorldInteraction)
				{
					infoWindow.SetWorldInteraction(false);
					if (Game.Instance.closeInteractionsOnResume)
					{
						infoWindow.CloseWindow(false);
						i--;
					}
				}
			}
			if (this.OnPauseChange != null)
			{
				this.OnPauseChange(true);
			}
			if (InterfaceControls.Instance.fastForwardArrow != null)
			{
				if (Player.Instance.spendingTimeMode)
				{
					InterfaceControls.Instance.fastForwardArrow.gameObject.SetActive(true);
				}
				else
				{
					InterfaceControls.Instance.fastForwardArrow.gameObject.SetActive(false);
				}
			}
		}
		if (this.interfaceActiveAudio != null)
		{
			AudioController.Instance.StopSound(this.interfaceActiveAudio, AudioController.StopType.triggerCue, "Game resumed");
			this.interfaceActiveAudio = null;
		}
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x000A36C4 File Offset: 0x000A18C4
	public void SetDisplayTutorialText(bool val)
	{
		if (this.enableTutorialText != val)
		{
			this.enableTutorialText = val;
			this.tutorialTextTriggered.Clear();
		}
		if (!this.enableTutorialText)
		{
			InterfaceController.Instance.notebookButton.notifications.SetNotifications(0);
		}
		PlayerPrefsController.GameSetting gameSetting = PlayerPrefsController.Instance.gameSettingControls.Find((PlayerPrefsController.GameSetting item) => item.identifier.ToLower() == "popuptips");
		if (gameSetting.toggle != null)
		{
			gameSetting.intValue = Convert.ToInt32(this.enableTutorialText);
			PlayerPrefs.SetInt(gameSetting.identifier, gameSetting.intValue);
		}
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x000A3768 File Offset: 0x000A1968
	public void TutorialTrigger(string str, bool isSilent = false)
	{
		if (!this.enableTutorialText)
		{
			return;
		}
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		if (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive)
		{
			return;
		}
		if (CityConstructor.Instance != null && CityConstructor.Instance.loadingOperationActive)
		{
			return;
		}
		str = str.ToLower();
		if (!this.tutorialTextTriggered.Contains(str))
		{
			this.tutorialTextTriggered.Add(str);
			if (!isSilent)
			{
				InterfaceController.Instance.NewHelpPointer(str);
			}
		}
		this.UpdateTutorialNotifications();
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x000A37F8 File Offset: 0x000A19F8
	public void UpdateTutorialNotifications()
	{
		int notifications = 0;
		bool flag = this.enableTutorialText;
		InterfaceController.Instance.notebookButton.notifications.SetNotifications(notifications);
		if (this.OnTutorialNotificationChange != null)
		{
			this.OnTutorialNotificationChange();
		}
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x000A3838 File Offset: 0x000A1A38
	public void ExecuteUnloadPipes()
	{
		foreach (PipeConstructor.PipeGroup pipeGroup in this.pipesToUnload)
		{
			bool flag = true;
			foreach (int num in pipeGroup.rooms)
			{
				NewRoom newRoom = null;
				if (CityData.Instance.roomDictionary.TryGetValue(num, ref newRoom) && newRoom.isVisible)
				{
					flag = false;
					break;
				}
			}
			pipeGroup.SetVisible(!flag);
		}
		this.pipesToUnload.Clear();
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x000A38FC File Offset: 0x000A1AFC
	[Button(null, 0)]
	public void DebugPreviousOrLastTime()
	{
		float nextOrPreviousGameTimeForThisHour = this.GetNextOrPreviousGameTimeForThisHour(this.debugDayList, this.debugDecimalRange.x, this.debugDecimalRange.y);
		Debug.Log(this.TimeStringOnDay(nextOrPreviousGameTimeForThisHour, true, true));
	}

	// Token: 0x04000A90 RID: 2704
	[Header("Flags")]
	public bool isFloorEdit;

	// Token: 0x04000A91 RID: 2705
	public bool isDialogEdit;

	// Token: 0x04000A92 RID: 2706
	public bool isCityEdit;

	// Token: 0x04000A93 RID: 2707
	public bool isTestScene;

	// Token: 0x04000A94 RID: 2708
	public bool dirtyScene;

	// Token: 0x04000A95 RID: 2709
	public bool isDecorEdit;

	// Token: 0x04000A96 RID: 2710
	public bool enableUserPause = true;

	// Token: 0x04000A97 RID: 2711
	public bool enableFirstPersonMap = true;

	// Token: 0x04000A98 RID: 2712
	public bool play;

	// Token: 0x04000A99 RID: 2713
	public bool enableTutorialText;

	// Token: 0x04000A9A RID: 2714
	public HashSet<string> tutorialTextTriggered = new HashSet<string>();

	// Token: 0x04000A9B RID: 2715
	public bool startedGame;

	// Token: 0x04000A9C RID: 2716
	private int pauseUnpauseDelay;

	// Token: 0x04000A9D RID: 2717
	private float drunkOscillatorX;

	// Token: 0x04000A9E RID: 2718
	private float drunkOscillatorY;

	// Token: 0x04000A9F RID: 2719
	public Vector2 drunkOscillation = Vector2.zero;

	// Token: 0x04000AA0 RID: 2720
	private float shiverOscillatorX;

	// Token: 0x04000AA1 RID: 2721
	private float shiverOscillatorY;

	// Token: 0x04000AA2 RID: 2722
	private float shiverProgress;

	// Token: 0x04000AA3 RID: 2723
	public Vector2 shiverOscillation = Vector2.zero;

	// Token: 0x04000AA4 RID: 2724
	private float drunkLensProgress;

	// Token: 0x04000AA5 RID: 2725
	private float headacheProgress;

	// Token: 0x04000AA6 RID: 2726
	private int sunShadowFrameCounter;

	// Token: 0x04000AA7 RID: 2727
	[Header("Time")]
	public float gameTime;

	// Token: 0x04000AA8 RID: 2728
	public double gameTimeDouble;

	// Token: 0x04000AA9 RID: 2729
	public double gameTimePassedThisFrame;

	// Token: 0x04000AAA RID: 2730
	private int prevHour;

	// Token: 0x04000AAB RID: 2731
	public float decimalClock;

	// Token: 0x04000AAC RID: 2732
	public double decimalClockDouble;

	// Token: 0x04000AAD RID: 2733
	public float formattedClock;

	// Token: 0x04000AAE RID: 2734
	public SessionData.TimeSpeed currentTimeSpeed;

	// Token: 0x04000AAF RID: 2735
	public float currentTimeMultiplier = 1f;

	// Token: 0x04000AB0 RID: 2736
	public float behaviourDelay = 0.5f;

	// Token: 0x04000AB1 RID: 2737
	public SessionData.TimeOfDay timeOfDay;

	// Token: 0x04000AB2 RID: 2738
	public int dayInt;

	// Token: 0x04000AB3 RID: 2739
	public SessionData.WeekDay day;

	// Token: 0x04000AB4 RID: 2740
	public int dateInt;

	// Token: 0x04000AB5 RID: 2741
	public SessionData.Month month;

	// Token: 0x04000AB6 RID: 2742
	public int monthInt;

	// Token: 0x04000AB7 RID: 2743
	public List<int> daysInMonths = new List<int>();

	// Token: 0x04000AB8 RID: 2744
	public int yearInt;

	// Token: 0x04000AB9 RID: 2745
	public int publicYear;

	// Token: 0x04000ABA RID: 2746
	public int leapYearCycle;

	// Token: 0x04000ABB RID: 2747
	public float gameTimeLimit;

	// Token: 0x04000ABC RID: 2748
	[Range(0f, 1f)]
	[Header("Weather")]
	public float currentRain;

	// Token: 0x04000ABD RID: 2749
	[Range(0f, 1f)]
	public float desiredRain;

	// Token: 0x04000ABE RID: 2750
	[Range(0f, 1f)]
	public float currentWind;

	// Token: 0x04000ABF RID: 2751
	[Range(0f, 1f)]
	public float desiredWind;

	// Token: 0x04000AC0 RID: 2752
	[Range(0f, 1f)]
	public float currentSnow;

	// Token: 0x04000AC1 RID: 2753
	[Range(0f, 1f)]
	public float desiredSnow;

	// Token: 0x04000AC2 RID: 2754
	[Range(0f, 1f)]
	public float currentLightning;

	// Token: 0x04000AC3 RID: 2755
	[Range(0f, 1f)]
	public float desiredLightning;

	// Token: 0x04000AC4 RID: 2756
	[Range(0f, 1f)]
	public float currentFog;

	// Token: 0x04000AC5 RID: 2757
	[Range(0f, 1f)]
	public float desiredFog;

	// Token: 0x04000AC6 RID: 2758
	public float transitionSpeed = 0.1f;

	// Token: 0x04000AC7 RID: 2759
	public float weatherChangeTimer;

	// Token: 0x04000AC8 RID: 2760
	private float monthTempMultiplier;

	// Token: 0x04000AC9 RID: 2761
	public float temperature;

	// Token: 0x04000ACA RID: 2762
	[Space(5f)]
	public float lightningTimer;

	// Token: 0x04000ACB RID: 2763
	[Space(5f)]
	public Vector3 windDirection = Vector3.zero;

	// Token: 0x04000ACC RID: 2764
	public float windForce = 20f;

	// Token: 0x04000ACD RID: 2765
	[Header("Scene")]
	public float dayProgress;

	// Token: 0x04000ACE RID: 2766
	public RainSheetController nearRainSheet;

	// Token: 0x04000ACF RID: 2767
	public RainSheetController farRainSheet;

	// Token: 0x04000AD0 RID: 2768
	public Vector2 nearRainAlpha1Threshold = new Vector2(0.9f, 0.9f);

	// Token: 0x04000AD1 RID: 2769
	public Vector2 nearRainAlpha2Threshold = new Vector2(0.5f, 0.5f);

	// Token: 0x04000AD2 RID: 2770
	public Vector2 nearRainSpeedThreshold = new Vector2(5f, 5f);

	// Token: 0x04000AD3 RID: 2771
	public Vector2 nearRainXTile1Threshold = new Vector2(10f, 10f);

	// Token: 0x04000AD4 RID: 2772
	public Vector2 nearRainXTile2Threshold = new Vector2(20f, 20f);

	// Token: 0x04000AD5 RID: 2773
	[Space(5f)]
	public Vector2 farRainAlpha1Threshold = new Vector2(1f, 1f);

	// Token: 0x04000AD6 RID: 2774
	public Vector2 farRainAlpha2Threshold = new Vector2(1f, 1f);

	// Token: 0x04000AD7 RID: 2775
	public Vector2 farRainSpeedThreshold = new Vector2(5f, 5f);

	// Token: 0x04000AD8 RID: 2776
	public Vector2 farRainXTile1Threshold = new Vector2(40f, 40f);

	// Token: 0x04000AD9 RID: 2777
	public Vector2 farRainXTile2Threshold = new Vector2(40f, 40f);

	// Token: 0x04000ADA RID: 2778
	[Space(5f)]
	public Vector2 particalRainCountThreshold = new Vector2(200f, 800f);

	// Token: 0x04000ADB RID: 2779
	public Vector2 particalSnowCountThreshold = new Vector2(200f, 800f);

	// Token: 0x04000ADC RID: 2780
	[Space(5f)]
	public float cityWetness;

	// Token: 0x04000ADD RID: 2781
	public float citySnow;

	// Token: 0x04000ADE RID: 2782
	[Tooltip("Configuration of materials where instancing will occur and weather params within them changed and updated")]
	public List<SessionData.WetMaterial> wetMaterials = new List<SessionData.WetMaterial>();

	// Token: 0x04000ADF RID: 2783
	public Dictionary<Material, SessionData.WetMaterial> weatherMaterialsReference = new Dictionary<Material, SessionData.WetMaterial>();

	// Token: 0x04000AE0 RID: 2784
	public List<CustomPassVolume> customPasses = new List<CustomPassVolume>();

	// Token: 0x04000AE1 RID: 2785
	public Dictionary<GameObject, WallFrontagePreset> rainyWindowFrontageObjects = new Dictionary<GameObject, WallFrontagePreset>();

	// Token: 0x04000AE2 RID: 2786
	public float autoPauseTimer;

	// Token: 0x04000AE3 RID: 2787
	public float autoResetTimer;

	// Token: 0x04000AE4 RID: 2788
	private float lightswitchPulse;

	// Token: 0x04000AE5 RID: 2789
	private bool lightswitchPulseMode;

	// Token: 0x04000AE6 RID: 2790
	[Header("PP Profiles")]
	public SessionData.SceneProfile currentProfile;

	// Token: 0x04000AE7 RID: 2791
	[NonSerialized]
	public CityControls.PPProfile currentSceneProfile;

	// Token: 0x04000AE8 RID: 2792
	[NonSerialized]
	public CityControls.PPProfile desiredSceneProfile;

	// Token: 0x04000AE9 RID: 2793
	[Header("HDRP")]
	[Tooltip("The global (outdoors) profile")]
	public Volume globalVolume;

	// Token: 0x04000AEA RID: 2794
	public GradientSky gradientSky;

	// Token: 0x04000AEB RID: 2795
	public Fog volFog;

	// Token: 0x04000AEC RID: 2796
	public DepthOfField dof;

	// Token: 0x04000AED RID: 2797
	public Vignette vignette;

	// Token: 0x04000AEE RID: 2798
	public MotionBlur motionBlur;

	// Token: 0x04000AEF RID: 2799
	public FilmGrain grain;

	// Token: 0x04000AF0 RID: 2800
	public Tonemapping toneMapping;

	// Token: 0x04000AF1 RID: 2801
	public Bloom bloom;

	// Token: 0x04000AF2 RID: 2802
	public ChromaticAberration chromaticAberration;

	// Token: 0x04000AF3 RID: 2803
	public LiftGammaGain lgg;

	// Token: 0x04000AF4 RID: 2804
	public ColorAdjustments colour;

	// Token: 0x04000AF5 RID: 2805
	public LensDistortion lensDistort;

	// Token: 0x04000AF6 RID: 2806
	public Exposure exposure;

	// Token: 0x04000AF7 RID: 2807
	public ChannelMixer channelMixer;

	// Token: 0x04000AF8 RID: 2808
	public int skyboxGradientIndex;

	// Token: 0x04000AF9 RID: 2809
	public SessionData.SkyboxGradient fromSkyboxColours;

	// Token: 0x04000AFA RID: 2810
	public SessionData.SkyboxGradient toSkyboxColours;

	// Token: 0x04000AFB RID: 2811
	[Header("Elevators")]
	public List<Elevator> activeElevators = new List<Elevator>();

	// Token: 0x04000AFC RID: 2812
	[Header("Particle Systems")]
	public List<InteractableController> particleSystems = new List<InteractableController>();

	// Token: 0x04000AFD RID: 2813
	[Header("Television")]
	public Material broadcastMaterial;

	// Token: 0x04000AFE RID: 2814
	public List<SessionData.TelevisionChannel> televisionChannels = new List<SessionData.TelevisionChannel>();

	// Token: 0x04000AFF RID: 2815
	[Header("References")]
	public TextMeshProUGUI pauseText;

	// Token: 0x04000B00 RID: 2816
	public GameObject pauseLensFlare;

	// Token: 0x04000B01 RID: 2817
	public Image pauseButtonImg;

	// Token: 0x04000B02 RID: 2818
	public Image normalSpeedButtonImg;

	// Token: 0x04000B03 RID: 2819
	public Image fastSpeedButtonImg;

	// Token: 0x04000B04 RID: 2820
	public Image veryFastSpeedButtonImg;

	// Token: 0x04000B05 RID: 2821
	public TextMeshPro newWatchTimeText;

	// Token: 0x04000B06 RID: 2822
	public TextMeshPro newWatchDateText;

	// Token: 0x04000B07 RID: 2823
	public TextMeshProUGUI clockText;

	// Token: 0x04000B08 RID: 2824
	public TextMeshProUGUI dayText;

	// Token: 0x04000B09 RID: 2825
	public Image pauseButtonIcon;

	// Token: 0x04000B0A RID: 2826
	public Sprite pauseIcon;

	// Token: 0x04000B0B RID: 2827
	public Sprite playIcon;

	// Token: 0x04000B0C RID: 2828
	public NewNode startingNode;

	// Token: 0x04000B0D RID: 2829
	[NonSerialized]
	private AudioController.LoopingSoundInfo interfaceActiveAudio;

	// Token: 0x04000B0E RID: 2830
	[Header("Debug")]
	public Vector2 debugDecimalRange = new Vector2(18f, 6f);

	// Token: 0x04000B0F RID: 2831
	public List<SessionData.WeekDay> debugDayList = new List<SessionData.WeekDay>();

	// Token: 0x04000B10 RID: 2832
	public Action UnloadPipes;

	// Token: 0x04000B11 RID: 2833
	public List<PipeConstructor.PipeGroup> pipesToUnload = new List<PipeConstructor.PipeGroup>();

	// Token: 0x04000B12 RID: 2834
	private static SessionData _instance;

	// Token: 0x020001AD RID: 429
	public enum TimeSpeed
	{
		// Token: 0x04000B18 RID: 2840
		slow,
		// Token: 0x04000B19 RID: 2841
		normal,
		// Token: 0x04000B1A RID: 2842
		fast,
		// Token: 0x04000B1B RID: 2843
		veryFast,
		// Token: 0x04000B1C RID: 2844
		simulation
	}

	// Token: 0x020001AE RID: 430
	public enum TimeOfDay
	{
		// Token: 0x04000B1E RID: 2846
		morning,
		// Token: 0x04000B1F RID: 2847
		afternoon,
		// Token: 0x04000B20 RID: 2848
		evening
	}

	// Token: 0x020001AF RID: 431
	public enum WeekDay
	{
		// Token: 0x04000B22 RID: 2850
		monday,
		// Token: 0x04000B23 RID: 2851
		tuesday,
		// Token: 0x04000B24 RID: 2852
		wednesday,
		// Token: 0x04000B25 RID: 2853
		thursday,
		// Token: 0x04000B26 RID: 2854
		friday,
		// Token: 0x04000B27 RID: 2855
		saturday,
		// Token: 0x04000B28 RID: 2856
		sunday
	}

	// Token: 0x020001B0 RID: 432
	public enum Month
	{
		// Token: 0x04000B2A RID: 2858
		jan,
		// Token: 0x04000B2B RID: 2859
		feb,
		// Token: 0x04000B2C RID: 2860
		mar,
		// Token: 0x04000B2D RID: 2861
		apr,
		// Token: 0x04000B2E RID: 2862
		may,
		// Token: 0x04000B2F RID: 2863
		jun,
		// Token: 0x04000B30 RID: 2864
		jul,
		// Token: 0x04000B31 RID: 2865
		aug,
		// Token: 0x04000B32 RID: 2866
		sep,
		// Token: 0x04000B33 RID: 2867
		oct,
		// Token: 0x04000B34 RID: 2868
		nov,
		// Token: 0x04000B35 RID: 2869
		dec
	}

	// Token: 0x020001B1 RID: 433
	[Serializable]
	public class WetMaterial
	{
		// Token: 0x04000B36 RID: 2870
		public Material mat;

		// Token: 0x04000B37 RID: 2871
		public Material instancedMat;

		// Token: 0x04000B38 RID: 2872
		public List<MeshRenderer> affectedRenderers = new List<MeshRenderer>();

		// Token: 0x04000B39 RID: 2873
		[Space(7f)]
		public bool affectRain = true;

		// Token: 0x04000B3A RID: 2874
		[MinMaxSlider(0f, 1f)]
		[ShowIf("affectRain")]
		public Vector2 rainMinMax = new Vector2(0f, 1f);

		// Token: 0x04000B3B RID: 2875
		[ShowIf("affectRain")]
		public float rainMultiplier = 1f;

		// Token: 0x04000B3C RID: 2876
		[Space(7f)]
		public bool affectCityWetness = true;

		// Token: 0x04000B3D RID: 2877
		[ShowIf("affectCityWetness")]
		[MinMaxSlider(0f, 1f)]
		public Vector2 cityWetnessMinMax = new Vector2(0f, 1f);

		// Token: 0x04000B3E RID: 2878
		[ShowIf("affectCityWetness")]
		public float cityWetnessMultiplier = 1f;

		// Token: 0x04000B3F RID: 2879
		[ShowIf("affectCityWetness")]
		public bool cityWetnessLogScale;

		// Token: 0x04000B40 RID: 2880
		[Space(7f)]
		public bool affectCitySnow = true;

		// Token: 0x04000B41 RID: 2881
		[MinMaxSlider(0f, 1f)]
		[ShowIf("affectCitySnow")]
		public Vector2 citySnowMinMax = new Vector2(0f, 1f);

		// Token: 0x04000B42 RID: 2882
		[ShowIf("affectCitySnow")]
		public float citySnowMultiplier = 1f;

		// Token: 0x04000B43 RID: 2883
		[Space(7f)]
		public bool affectCoatMask = true;

		// Token: 0x04000B44 RID: 2884
		[ShowIf("affectCoatMask")]
		[MinMaxSlider(0f, 1f)]
		public Vector2 coatMaskMinMax = new Vector2(0f, 1f);

		// Token: 0x04000B45 RID: 2885
		[ShowIf("affectCoatMask")]
		public float coatMaskMultiplier = 1f;

		// Token: 0x04000B46 RID: 2886
		[Space(7f)]
		public bool affectWind = true;

		// Token: 0x04000B47 RID: 2887
		[ShowIf("affectWind")]
		[MinMaxSlider(0f, 1f)]
		public Vector2 windMinMax = new Vector2(0f, 1f);

		// Token: 0x04000B48 RID: 2888
		[ShowIf("affectWind")]
		public float windMultiplier = 1f;
	}

	// Token: 0x020001B2 RID: 434
	public enum SceneProfile
	{
		// Token: 0x04000B4A RID: 2890
		outdoors,
		// Token: 0x04000B4B RID: 2891
		indoors,
		// Token: 0x04000B4C RID: 2892
		grimey,
		// Token: 0x04000B4D RID: 2893
		clean,
		// Token: 0x04000B4E RID: 2894
		corporate,
		// Token: 0x04000B4F RID: 2895
		cbd,
		// Token: 0x04000B50 RID: 2896
		chinatown,
		// Token: 0x04000B51 RID: 2897
		industrial,
		// Token: 0x04000B52 RID: 2898
		residential,
		// Token: 0x04000B53 RID: 2899
		warm
	}

	// Token: 0x020001B3 RID: 435
	[Serializable]
	public class SkyboxGradient : IComparable<SessionData.SkyboxGradient>
	{
		// Token: 0x06000AC0 RID: 2752 RVA: 0x000A3C15 File Offset: 0x000A1E15
		public int CompareTo(SessionData.SkyboxGradient otherObject)
		{
			return this.time.CompareTo(otherObject.time);
		}

		// Token: 0x04000B54 RID: 2900
		public float time;

		// Token: 0x04000B55 RID: 2901
		public Color skyColour = Color.cyan;

		// Token: 0x04000B56 RID: 2902
		public Color fogAlbedo = Color.cyan;

		// Token: 0x04000B57 RID: 2903
		[Space(3f)]
		public Color ambientLightTop = Color.cyan;

		// Token: 0x04000B58 RID: 2904
		public Color ambientLightMiddle = Color.cyan;

		// Token: 0x04000B59 RID: 2905
		public Color ambientLightBottom = Color.cyan;

		// Token: 0x04000B5A RID: 2906
		public Color ambientLightingColour = Color.cyan;

		// Token: 0x04000B5B RID: 2907
		public Color fogColour = Color.cyan;

		// Token: 0x04000B5C RID: 2908
		public Color seaEmission = Color.black;

		// Token: 0x04000B5D RID: 2909
		public Color smokeEmission = Color.black;
	}

	// Token: 0x020001B4 RID: 436
	public class TelevisionChannel
	{
		// Token: 0x06000AC2 RID: 2754 RVA: 0x000A3CA0 File Offset: 0x000A1EA0
		public void ProcessTelevisionBroadcast()
		{
			if (this.currentBroadcastSchedule != null)
			{
				if (this.currentShow == null)
				{
					this.currentScheduleIndex++;
					if (this.currentScheduleIndex >= this.currentBroadcastSchedule.broadcasts.Count)
					{
						this.currentScheduleIndex = 0;
					}
					this.currentShow = this.currentBroadcastSchedule.broadcasts[this.currentScheduleIndex];
					this.currentShowProgressSeconds = 0f;
					this.currentShowImageProgress = 9999999f;
					this.currentImageIndex = -1;
					if (this.currentShow.audioEvent != null)
					{
						RuntimeManager.StudioSystem.getEventByID(GUID.Parse(this.currentShow.audioEvent.guid), ref this.currentShowEventDescription);
						this.currentShowEventDescription.getLength(ref this.currentShowAudioLength);
					}
					this.currentShowImageLength = this.currentShow.totalSpriteCount;
					this.broadcastMaterialInstanced.SetTexture("_EmissiveColorMap", this.currentShow.spriteSheet);
					this.broadcastMaterialInstanced.SetTextureScale("_EmissiveColorMap", new Vector2(1f / (float)this.currentShow.indexWidth, 1f / (float)this.currentShow.indexHeight));
					foreach (AudioController.LoopingSoundInfo loopingSoundInfo in AudioController.Instance.loopingSounds)
					{
						if (loopingSoundInfo.isBroadcast == this)
						{
							loopingSoundInfo.audioEvent.stop(1);
							loopingSoundInfo.audioEvent.release();
							loopingSoundInfo.isValid = loopingSoundInfo.audioEvent.isValid();
							loopingSoundInfo.eventPreset = this.currentShow.audioEvent;
							loopingSoundInfo.UpdateOcclusion(false);
						}
					}
				}
				if (this.currentShow != null && this.currentShowImageProgress >= this.currentShow.changeImageEvery)
				{
					if (this.currentShow.order == BroadcastPreset.ImageOrder.ordered)
					{
						this.currentImageIndex++;
					}
					else if (this.currentShow.order == BroadcastPreset.ImageOrder.random)
					{
						this.currentImageIndex = Toolbox.Instance.Rand(0, this.currentShowImageLength, false);
					}
					if (this.currentImageIndex >= this.currentShowImageLength)
					{
						this.currentImageIndex = 0;
					}
					int num = 0;
					int i = this.currentImageIndex;
					while (i >= this.currentShow.indexWidth)
					{
						i -= this.currentShow.indexWidth;
						num++;
					}
					int num2 = i;
					Vector2 vector;
					vector..ctor((float)num2 / (float)this.currentShow.indexWidth, (float)(this.currentShow.indexHeight - 1 - num) / (float)this.currentShow.indexHeight);
					this.broadcastMaterialInstanced.SetTextureOffset("_EmissiveColorMap", vector);
					this.currentShowImageProgress = 0f;
				}
				this.currentShowImageProgress += Time.deltaTime * SessionData.Instance.currentTimeMultiplier;
				this.currentShowProgressSeconds += Time.deltaTime * SessionData.Instance.currentTimeMultiplier;
				if (this.currentShowProgressSeconds >= (float)this.currentShowAudioLength / 1000f)
				{
					this.currentShow = null;
				}
			}
		}

		// Token: 0x04000B5E RID: 2910
		public BroadcastSchedule currentBroadcastSchedule;

		// Token: 0x04000B5F RID: 2911
		public BroadcastPreset currentShow;

		// Token: 0x04000B60 RID: 2912
		public Material broadcastMaterialInstanced;

		// Token: 0x04000B61 RID: 2913
		public int currentScheduleIndex = -1;

		// Token: 0x04000B62 RID: 2914
		public float currentShowProgressSeconds;

		// Token: 0x04000B63 RID: 2915
		public float currentShowImageProgress;

		// Token: 0x04000B64 RID: 2916
		private EventDescription currentShowEventDescription;

		// Token: 0x04000B65 RID: 2917
		private int currentShowAudioLength;

		// Token: 0x04000B66 RID: 2918
		private int currentShowImageLength;

		// Token: 0x04000B67 RID: 2919
		private int currentImageIndex;
	}

	// Token: 0x020001B5 RID: 437
	public enum PhysicsSyncType
	{
		// Token: 0x04000B69 RID: 2921
		now,
		// Token: 0x04000B6A RID: 2922
		onPlayerMovement,
		// Token: 0x04000B6B RID: 2923
		both
	}

	// Token: 0x020001B6 RID: 438
	// (Invoke) Token: 0x06000AC5 RID: 2757
	public delegate void OnPauseUnPause(bool openDesktopMode);

	// Token: 0x020001B7 RID: 439
	// (Invoke) Token: 0x06000AC9 RID: 2761
	public delegate void WeatherChange();

	// Token: 0x020001B8 RID: 440
	// (Invoke) Token: 0x06000ACD RID: 2765
	public delegate void HourChange();

	// Token: 0x020001B9 RID: 441
	// (Invoke) Token: 0x06000AD1 RID: 2769
	public delegate void TutorialNotificationChange();
}
