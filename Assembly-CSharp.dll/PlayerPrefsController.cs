using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x0200044B RID: 1099
public class PlayerPrefsController : MonoBehaviour
{
	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06001898 RID: 6296 RVA: 0x0016E935 File Offset: 0x0016CB35
	public static PlayerPrefsController Instance
	{
		get
		{
			return PlayerPrefsController._instance;
		}
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x0016E93C File Offset: 0x0016CB3C
	private void Awake()
	{
		if (PlayerPrefsController._instance != null && PlayerPrefsController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			PlayerPrefsController._instance = this;
		}
		if (!this.loadedPlayerPrefs)
		{
			this.playedBefore = Convert.ToBoolean(PlayerPrefs.GetInt("playedBefore", 0));
			this.acceptedEULA = Convert.ToBoolean(PlayerPrefs.GetInt("EULA", 0));
			Game.Log("Menu: Played before: " + this.playedBefore.ToString(), 2);
			if (!this.playedBefore)
			{
				PlayerPrefs.SetInt("playedBefore", 1);
			}
			this.LoadPlayerPrefs(false);
		}
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x0016E9E0 File Offset: 0x0016CBE0
	public void LoadPlayerPrefs(bool lateLoad = false)
	{
		Game.Log("Loading player prefs...", 2);
		foreach (PlayerPrefsController.GameSetting gameSetting in this.gameSettingControls)
		{
			if (lateLoad == gameSetting.lateLoad)
			{
				if (gameSetting.toggle != null)
				{
					gameSetting.intValue = PlayerPrefs.GetInt(gameSetting.identifier, gameSetting.intDefault);
					gameSetting.toggle.SetIsOnWithoutNotify(Convert.ToBoolean(gameSetting.intValue));
				}
				else if (gameSetting.slider != null)
				{
					gameSetting.intValue = PlayerPrefs.GetInt(gameSetting.identifier, gameSetting.intDefault);
					gameSetting.slider.SetValueWithoutNotify(gameSetting.intValue);
				}
				else if (gameSetting.dropdown != null)
				{
					if (gameSetting.useDropdownInt)
					{
						gameSetting.intValue = PlayerPrefs.GetInt(gameSetting.identifier, gameSetting.intDefault);
						gameSetting.dropdown.dropdown.SetValueWithoutNotify(gameSetting.intValue);
						if (gameSetting.secondaryDropdown != null)
						{
							gameSetting.secondaryDropdown.dropdown.SetValueWithoutNotify(gameSetting.intValue);
						}
						Game.Log("Menu: Set " + gameSetting.identifier + " to " + gameSetting.intValue.ToString(), 2);
					}
					else
					{
						gameSetting.strValue = PlayerPrefs.GetString(gameSetting.identifier, gameSetting.strDefault);
						if (gameSetting.strValue == null || gameSetting.strValue.Length <= 0)
						{
							gameSetting.strValue = gameSetting.strDefault;
						}
						gameSetting.dropdown.SelectFromStaticOption(gameSetting.strValue);
						if (gameSetting.secondaryDropdown != null)
						{
							gameSetting.secondaryDropdown.SelectFromStaticOption(gameSetting.strValue);
						}
						Game.Log("Menu: Set " + gameSetting.identifier + " to " + gameSetting.strValue, 2);
					}
				}
				else if (gameSetting.multiselect != null)
				{
					gameSetting.intValue = PlayerPrefs.GetInt(gameSetting.identifier, gameSetting.intDefault);
					gameSetting.multiselect.SetChosen(gameSetting.intValue);
					Game.Log("Menu: Multiselect " + gameSetting.identifier + " to " + gameSetting.intValue.ToString(), 2);
				}
				this.OnToggleChanged(gameSetting.identifier, false, null);
				if (gameSetting.valueDisplayText != null)
				{
					gameSetting.valueDisplayText.text = gameSetting.intValue.ToString();
				}
			}
		}
		this.loadedPlayerPrefs = true;
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x0016EC88 File Offset: 0x0016CE88
	public void ResetPlayerPrefsToDefaults()
	{
		foreach (PlayerPrefsController.GameSetting gameSetting in this.gameSettingControls)
		{
			gameSetting.intValue = gameSetting.intDefault;
			gameSetting.strValue = gameSetting.strDefault;
			if (gameSetting.toggle != null)
			{
				gameSetting.toggle.SetIsOnWithoutNotify(Convert.ToBoolean(gameSetting.intValue));
			}
			else if (gameSetting.slider != null)
			{
				gameSetting.slider.SetValueWithoutNotify(gameSetting.intValue);
			}
			else if (gameSetting.dropdown != null)
			{
				if (gameSetting.useDropdownInt)
				{
					gameSetting.dropdown.dropdown.SetValueWithoutNotify(gameSetting.intValue);
					if (gameSetting.secondaryDropdown != null)
					{
						gameSetting.secondaryDropdown.dropdown.SetValueWithoutNotify(gameSetting.intValue);
					}
				}
				else
				{
					gameSetting.dropdown.SelectFromStaticOption(gameSetting.strValue);
					if (gameSetting.secondaryDropdown != null)
					{
						gameSetting.secondaryDropdown.SelectFromStaticOption(gameSetting.strValue);
					}
				}
			}
			this.OnToggleChanged(gameSetting.identifier, false, null);
			if (gameSetting.valueDisplayText != null)
			{
				gameSetting.valueDisplayText.text = gameSetting.intValue.ToString();
			}
		}
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x0016EE00 File Offset: 0x0016D000
	public int GetSettingInt(string id)
	{
		int result = 0;
		PlayerPrefsController.GameSetting gameSetting = this.gameSettingControls.Find((PlayerPrefsController.GameSetting item) => item.identifier.ToLower() == id.ToLower());
		if (gameSetting != null)
		{
			result = gameSetting.intValue;
		}
		return result;
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x0016EE40 File Offset: 0x0016D040
	public string GetSettingStr(string id)
	{
		string result = string.Empty;
		PlayerPrefsController.GameSetting gameSetting = this.gameSettingControls.Find((PlayerPrefsController.GameSetting item) => item.identifier.ToLower() == id.ToLower());
		if (gameSetting != null)
		{
			result = gameSetting.strValue;
		}
		return result;
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x0016EE84 File Offset: 0x0016D084
	public void OnToggleChanged(string id, bool fetchValueFromControls, MonoBehaviour elementScript = null)
	{
		PlayerPrefsController.GameSetting gameSetting = this.gameSettingControls.Find((PlayerPrefsController.GameSetting item) => item.identifier.ToLower() == id.ToLower());
		if (gameSetting.toggle != null)
		{
			if (fetchValueFromControls)
			{
				gameSetting.intValue = Convert.ToInt32(gameSetting.toggle.isOn);
			}
			PlayerPrefs.SetInt(gameSetting.identifier, gameSetting.intValue);
		}
		else if (gameSetting.slider != null)
		{
			if (fetchValueFromControls)
			{
				gameSetting.intValue = Mathf.RoundToInt(gameSetting.slider.slider.value);
			}
			PlayerPrefs.SetInt(gameSetting.identifier, gameSetting.intValue);
		}
		else if (gameSetting.dropdown != null)
		{
			if (gameSetting.secondaryDropdown == null || gameSetting.dropdown == elementScript)
			{
				if (gameSetting.useDropdownInt)
				{
					if (fetchValueFromControls)
					{
						gameSetting.intValue = gameSetting.dropdown.dropdown.value;
					}
					PlayerPrefs.SetInt(gameSetting.identifier, gameSetting.intValue);
				}
				else
				{
					if (fetchValueFromControls)
					{
						gameSetting.strValue = gameSetting.dropdown.GetCurrentSelectedStaticOption();
					}
					PlayerPrefs.SetString(gameSetting.identifier, gameSetting.strValue);
				}
				if (gameSetting.secondaryDropdown != null)
				{
					gameSetting.secondaryDropdown.dropdown.SetValueWithoutNotify(gameSetting.dropdown.dropdown.value);
				}
			}
			else if (gameSetting.secondaryDropdown != null && gameSetting.secondaryDropdown == elementScript)
			{
				if (gameSetting.useDropdownInt)
				{
					if (fetchValueFromControls)
					{
						gameSetting.intValue = gameSetting.secondaryDropdown.dropdown.value;
					}
					PlayerPrefs.SetInt(gameSetting.identifier, gameSetting.intValue);
				}
				else
				{
					if (fetchValueFromControls)
					{
						gameSetting.strValue = gameSetting.secondaryDropdown.GetCurrentSelectedStaticOption();
					}
					PlayerPrefs.SetString(gameSetting.identifier, gameSetting.strValue);
				}
				if (gameSetting.dropdown != null)
				{
					gameSetting.dropdown.dropdown.SetValueWithoutNotify(gameSetting.secondaryDropdown.dropdown.value);
				}
			}
		}
		else if (gameSetting.multiselect != null)
		{
			if (fetchValueFromControls)
			{
				gameSetting.intValue = Convert.ToInt32(gameSetting.multiselect.chosenIndex);
			}
			PlayerPrefs.SetInt(gameSetting.identifier, gameSetting.intValue);
		}
		if (gameSetting.valueDisplayText != null)
		{
			gameSetting.valueDisplayText.text = gameSetting.intValue.ToString();
		}
		if (gameSetting.identifier == "motionBlur")
		{
			if (!SessionData.Instance.isFloorEdit)
			{
				SessionData.Instance.motionBlur.active = Convert.ToBoolean(gameSetting.intValue);
				SessionData.Instance.motionBlur.intensity.value = Game.Instance.motionBlurIntensity + SessionData.Instance.GetGameSpeedMotionBlurModifier();
				return;
			}
		}
		else if (gameSetting.identifier == "depthBlur")
		{
			if (!SessionData.Instance.isFloorEdit)
			{
				Game.Instance.SetDepthBlur(Convert.ToBoolean(gameSetting.intValue));
				return;
			}
		}
		else if (gameSetting.identifier == "filmGrain")
		{
			if (!SessionData.Instance.isFloorEdit)
			{
				SessionData.Instance.grain.active = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
		}
		else if (gameSetting.identifier == "bloom")
		{
			if (!SessionData.Instance.isFloorEdit)
			{
				SessionData.Instance.bloom.active = Convert.ToBoolean(gameSetting.intValue);
				SessionData.Instance.bloom.intensity.value = Game.Instance.bloomIntensity;
				return;
			}
		}
		else if (gameSetting.identifier == "colourGrading")
		{
			if (!SessionData.Instance.isFloorEdit)
			{
				SessionData.Instance.toneMapping.active = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
		}
		else
		{
			if (gameSetting.identifier == "everywhereIllegal")
			{
				Game.Instance.everywhereIllegal = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
			if (gameSetting.identifier == "keysToTheCity")
			{
				Game.Instance.keysToTheCity = Convert.ToBoolean(gameSetting.intValue);
				if (!SessionData.Instance.startedGame || !Game.Instance.keysToTheCity)
				{
					return;
				}
				foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
				{
					Player.Instance.AddToKeyring(newAddress, false);
					if (newAddress.passcode.used)
					{
						GameplayController.Instance.AddPasscode(newAddress.passcode, false);
					}
					foreach (NewRoom newRoom in newAddress.rooms)
					{
						if (newRoom.passcode.used)
						{
							GameplayController.Instance.AddPasscode(newRoom.passcode, false);
						}
					}
				}
				using (List<Citizen>.Enumerator enumerator3 = CityData.Instance.citizenDirectory.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Human human = enumerator3.Current;
						GameplayController.Instance.AddPasscode(human.passcode, false);
					}
					return;
				}
			}
			if (gameSetting.identifier == "invisiblePlayer")
			{
				Game.Instance.invisiblePlayer = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
			if (gameSetting.identifier == "inaudiblePlayer")
			{
				Game.Instance.inaudiblePlayer = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
			if (gameSetting.identifier == "invinciblePlayer")
			{
				Game.Instance.invinciblePlayer = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
			if (gameSetting.identifier == "disableFallDamage")
			{
				Game.Instance.disableFallDamage = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
			if (gameSetting.identifier == "routeTeleport")
			{
				Game.Instance.routeTeleport = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
			if (gameSetting.identifier == "allUpgrades")
			{
				Game.Instance.giveAllUpgrades = Convert.ToBoolean(gameSetting.intValue);
				return;
			}
			if (gameSetting.identifier == "allRooms")
			{
				Game.Instance.discoverAllRooms = Convert.ToBoolean(gameSetting.intValue);
				if (!Game.Instance.discoverAllRooms || !SessionData.Instance.startedGame)
				{
					return;
				}
				using (List<NewRoom>.Enumerator enumerator2 = CityData.Instance.roomDirectory.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NewRoom newRoom2 = enumerator2.Current;
						newRoom2.SetExplorationLevel(2);
						foreach (AirDuctGroup airDuctGroup in newRoom2.ductGroups)
						{
							foreach (AirDuctGroup.AirDuctSection airDuctSection in airDuctGroup.airDucts)
							{
								airDuctSection.SetDiscovered(true);
							}
						}
					}
					return;
				}
			}
			if (gameSetting.identifier == "music")
			{
				bool flag = Convert.ToBoolean(gameSetting.intValue);
				if (MusicController.Instance != null && MusicController.Instance.enableMusic != flag)
				{
					MusicController.Instance.enableMusic = Convert.ToBoolean(gameSetting.intValue);
					if (MusicController.Instance.enableMusic)
					{
						MusicController.Instance.ForceNextTrack();
						return;
					}
					MusicController.Instance.StopCurrentTrack();
					return;
				}
			}
			else
			{
				if (gameSetting.identifier == "headBob")
				{
					GameplayControls.Instance.headBobMultiplier = (float)gameSetting.intValue / 100f;
					return;
				}
				if (gameSetting.identifier == "weatherChange")
				{
					Game.Instance.weatherChangeFrequency = (float)gameSetting.intValue / 100f;
					return;
				}
				if (gameSetting.identifier == "rainDetail")
				{
					if (gameSetting.intValue <= 0)
					{
						Game.Instance.SetRaindrops(false);
						Game.Instance.SetRainWindows(false);
						return;
					}
					if (gameSetting.intValue == 1)
					{
						Game.Instance.SetRaindrops(true);
						Game.Instance.SetRainWindows(false);
						return;
					}
					if (gameSetting.intValue >= 2)
					{
						Game.Instance.SetRaindrops(true);
						Game.Instance.SetRainWindows(true);
						return;
					}
				}
				else
				{
					if (gameSetting.identifier == "uiScale")
					{
						Game.Instance.SetUIScale(gameSetting.intValue);
						return;
					}
					if (gameSetting.identifier == "language")
					{
						Game.Instance.language = gameSetting.strValue;
						return;
					}
					if (gameSetting.identifier == "fpsfov")
					{
						Game.Instance.SetFOV(gameSetting.intValue);
						return;
					}
					if (gameSetting.identifier == "objectiveMarkers")
					{
						Game.Instance.SetObjectiveMarkers(Convert.ToBoolean(gameSetting.intValue));
						return;
					}
					if (gameSetting.identifier == "gameDifficulty")
					{
						if (gameSetting.strValue == "Easy")
						{
							Game.Instance.SetGameDifficulty(0);
							return;
						}
						if (gameSetting.strValue == "Hard")
						{
							Game.Instance.SetGameDifficulty(2);
							return;
						}
						if (gameSetting.strValue == "Extreme")
						{
							Game.Instance.SetGameDifficulty(3);
							return;
						}
						Game.Instance.SetGameDifficulty(1);
						return;
					}
					else if (gameSetting.identifier == "gameLength")
					{
						if (gameSetting.strValue == "Very Short")
						{
							Game.Instance.SetGameLength(0, SessionData.Instance.startedGame, false, false);
							return;
						}
						if (gameSetting.strValue == "Short")
						{
							Game.Instance.SetGameLength(1, SessionData.Instance.startedGame, false, false);
							return;
						}
						if (gameSetting.strValue == "Long")
						{
							Game.Instance.SetGameLength(3, SessionData.Instance.startedGame, false, false);
							return;
						}
						if (gameSetting.strValue == "Very Long")
						{
							Game.Instance.SetGameLength(4, SessionData.Instance.startedGame, false, false);
							return;
						}
						Game.Instance.SetGameLength(2, SessionData.Instance.startedGame, false, false);
						return;
					}
					else if (gameSetting.identifier == "startTime")
					{
						if (gameSetting.strValue == "Morning")
						{
							Game.Instance.SetSandboxStartTime(9f);
							return;
						}
						if (gameSetting.strValue == "Midday")
						{
							Game.Instance.SetSandboxStartTime(12f);
							return;
						}
						if (gameSetting.strValue == "Evening")
						{
							Game.Instance.SetSandboxStartTime(18f);
							return;
						}
						Game.Instance.SetSandboxStartTime(0f);
						return;
					}
					else
					{
						if (gameSetting.identifier == "resumeAfterPin")
						{
							Game.Instance.resumeAfterPin = Convert.ToBoolean(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "closeInteractionsOnResume")
						{
							Game.Instance.closeInteractionsOnResume = Convert.ToBoolean(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "directionalArrow")
						{
							Game.Instance.SetDirectionalArrow(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "awarenessIndicator")
						{
							Game.Instance.SetAwarenessIndicator(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "mouseSensitivityX")
						{
							Game.Instance.mouseSensitivity.x = (float)gameSetting.intValue;
							return;
						}
						if (gameSetting.identifier == "mouseSensitivityY")
						{
							Game.Instance.mouseSensitivity.y = (float)gameSetting.intValue;
							return;
						}
						if (gameSetting.identifier == "controllerSensitivityX")
						{
							Game.Instance.controllerSensitivity.x = (float)gameSetting.intValue;
							return;
						}
						if (gameSetting.identifier == "controllerSensitivityY")
						{
							Game.Instance.controllerSensitivity.y = (float)gameSetting.intValue;
							return;
						}
						if (gameSetting.identifier == "controlAutoSwitch")
						{
							Game.Instance.controlAutoSwitch = Convert.ToBoolean(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "controlMethod")
						{
							InputController.Instance.SetMouseInputMode(Convert.ToBoolean(gameSetting.intValue), false);
							return;
						}
						if (gameSetting.identifier == "invertX")
						{
							Game.Instance.axisMP.x = (Mathf.Clamp01((float)gameSetting.intValue) * 2f - 1f) * -1f;
							return;
						}
						if (gameSetting.identifier == "invertY")
						{
							Game.Instance.axisMP.y = (Mathf.Clamp01((float)gameSetting.intValue) * 2f - 1f) * -1f;
							return;
						}
						if (gameSetting.identifier == "mouseSmoothing")
						{
							Game.Instance.mouseSmoothing = gameSetting.intValue;
							return;
						}
						if (gameSetting.identifier == "controllerSmoothing")
						{
							Game.Instance.controllerSmoothing = gameSetting.intValue;
							return;
						}
						if (gameSetting.identifier == "enableMurders")
						{
							Game.Instance.SetMurders(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "startingApartment")
						{
							Game.Instance.SetSandboxStartingApartment(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "startingMoney")
						{
							Game.Instance.SetSandboxStartingMoney(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "startingLockpicks")
						{
							Game.Instance.SetSandboxStartingLockpicks(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "coldStatus")
						{
							Game.Instance.SetEnableColdStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "smellyStatus")
						{
							Game.Instance.SetEnableSmellyStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "injuryStatus")
						{
							Game.Instance.SetEnableInjuryStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "bleedingStatus")
						{
							Game.Instance.SetEnableBleedingStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "hungerStatus")
						{
							Game.Instance.SetEnableHungerStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "hydrationStatus")
						{
							Game.Instance.SetEnableHydrationStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "tiredStatus")
						{
							Game.Instance.SetEnableTiredStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "numbStatus")
						{
							Game.Instance.SetEnableNumbStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "sickStatus")
						{
							Game.Instance.SetEnableSickStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "wetStatus")
						{
							Game.Instance.SetEnableWetStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "drunkStatus")
						{
							Game.Instance.SetEnableDrunkStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "headacheStatus")
						{
							Game.Instance.SetEnableHeadacheStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "energizedStatus")
						{
							Game.Instance.SetEnableEnergizedStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "hydratedStatus")
						{
							Game.Instance.SetEnableHydratedStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "focusedStatus")
						{
							Game.Instance.SetEnableFocusedStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "wellRestedStatus")
						{
							Game.Instance.SetEnableWellRestedStatus(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "controlHints")
						{
							Game.Instance.displayExtraControlHints = Convert.ToBoolean(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "popupTips")
						{
							SessionData.Instance.SetDisplayTutorialText(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "screenshotMode")
						{
							Game.Instance.SetScreenshotMode(Convert.ToBoolean(gameSetting.intValue), false);
							return;
						}
						if (gameSetting.identifier == "forceSideMissionDifficulty")
						{
							Game.Instance.SetForceSideJobDifficulty(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "sideMissionDifficulty")
						{
							Game.Instance.SetForcedSideJobDifficulty(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "demoMode")
						{
							Game.Instance.demoMode = Convert.ToBoolean(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "pauseAI")
						{
							Game.Instance.SetPauseAI(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "freeCam")
						{
							Game.Instance.SetFreeCamMode(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "fastForward")
						{
							Game.Instance.SetFastForward(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "skipIntro")
						{
							Game.Instance.skipIntro = Convert.ToBoolean(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "movementSpeed")
						{
							Game.Instance.movementSpeed = (float)gameSetting.intValue / 100f;
							return;
						}
						if (gameSetting.identifier == "drawDist")
						{
							Game.Instance.SetDrawDistance((float)gameSetting.intValue / 100f);
							return;
						}
						if (gameSetting.identifier == "vsync")
						{
							Game.Instance.SetVsync(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "enableFrameCap")
						{
							Game.Instance.SetEnableFrameCap(Convert.ToBoolean(gameSetting.intValue));
							return;
						}
						if (gameSetting.identifier == "frameCap")
						{
							Game.Instance.SetFrameCap(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "textspeed")
						{
							Game.Instance.textSpeed = (float)gameSetting.intValue / 100f;
							return;
						}
						if (gameSetting.identifier == "disableTrespass")
						{
							Game.Instance.disableTrespass = Convert.ToBoolean(gameSetting.intValue);
							return;
						}
						if (gameSetting.identifier == "timeScale")
						{
							Time.timeScale = Mathf.Max(0.1f, (float)gameSetting.intValue / 100f);
							return;
						}
						if (gameSetting.identifier == "masterVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("SFX", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else if (gameSetting.identifier == "musicVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("Soundtrack", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else if (gameSetting.identifier == "ambienceVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("Ambience", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else if (gameSetting.identifier == "weatherVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("Weather", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else if (gameSetting.identifier == "footstepsVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("Footsteps", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else if (gameSetting.identifier == "interfaceVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("UI", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else if (gameSetting.identifier == "notificationsVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("Notifications", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else if (gameSetting.identifier == "paVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("PA system", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else if (gameSetting.identifier == "otherVolume")
						{
							if (AudioController.Instance != null)
							{
								AudioController.Instance.SetVCALevel("Other SFX", (float)gameSetting.intValue / 100f);
								return;
							}
						}
						else
						{
							if (gameSetting.identifier == "dithering")
							{
								Game.Instance.SetDithering(Convert.ToBoolean(gameSetting.intValue));
								return;
							}
							if (gameSetting.identifier == "aaMode")
							{
								Game.Instance.SetAAMode(gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "aaQuality")
							{
								Game.Instance.SetAAQuality(gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "licensedMusic")
							{
								Game.Instance.SetAllowLicensedMusic(Convert.ToBoolean(gameSetting.intValue));
								return;
							}
							if (gameSetting.identifier == "twitchAudienceCitizens")
							{
								StreamingOptionsController.Instance.SetEnableTwitchAudienceCitizens(Convert.ToBoolean(gameSetting.intValue));
								return;
							}
							if (gameSetting.identifier == "passcodeOverrideToggle")
							{
								Game.Instance.SetPasscodeOverrideToggle(Convert.ToBoolean(gameSetting.intValue));
								return;
							}
							if (gameSetting.identifier == "passcodeOverride")
							{
								Game.Instance.SetPasscodeOverride(gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "flickeringLights")
							{
								Game.Instance.SetFlickingLights(Convert.ToBoolean(gameSetting.intValue));
								return;
							}
							if (gameSetting.identifier == "lightDistance")
							{
								Game.Instance.SetLightDistance((float)gameSetting.intValue / 100f);
								return;
							}
							if (gameSetting.identifier == "saveGameCompression")
							{
								Game.Instance.useSaveGameCompression = Convert.ToBoolean(gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "cityDataCompression")
							{
								Game.Instance.useCityDataCompression = Convert.ToBoolean(gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "dynamicResolution")
							{
								bool flag2 = Convert.ToBoolean(gameSetting.intValue);
								DynamicResolutionController.Instance.DynamicResolutionEnabled = flag2;
								DynamicResolutionController.Instance.DLSSEnabled = flag2;
								return;
							}
							if (gameSetting.identifier == "dlssMode")
							{
								DynamicResolutionController.Instance.SetDLSSQualityMode((DynamicResolutionController.DLSSQuality)gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "alwaysRun")
							{
								Game.Instance.alwaysRun = Convert.ToBoolean(gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "toggleRun")
							{
								Game.Instance.toggleRun = Convert.ToBoolean(gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "permaDeath")
							{
								Game.Instance.permaDeath = Convert.ToBoolean(gameSetting.intValue);
								return;
							}
							if (gameSetting.identifier == "cityEditor")
							{
								Game.Instance.enableCityEditor = Convert.ToBoolean(gameSetting.intValue);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x001706C4 File Offset: 0x0016E8C4
	[Button(null, 0)]
	public void ResetFirstPlay()
	{
		PlayerPrefs.SetInt("playedBefore", 0);
	}

	// Token: 0x04001E8C RID: 7820
	[ReorderableList]
	public List<PlayerPrefsController.GameSetting> gameSettingControls = new List<PlayerPrefsController.GameSetting>();

	// Token: 0x04001E8D RID: 7821
	public bool playedBefore;

	// Token: 0x04001E8E RID: 7822
	public bool acceptedEULA;

	// Token: 0x04001E8F RID: 7823
	public bool loadedPlayerPrefs;

	// Token: 0x04001E90 RID: 7824
	private static PlayerPrefsController _instance;

	// Token: 0x0200044C RID: 1100
	[Serializable]
	public class GameSetting
	{
		// Token: 0x04001E91 RID: 7825
		public string identifier;

		// Token: 0x04001E92 RID: 7826
		public int intDefault;

		// Token: 0x04001E93 RID: 7827
		public int intValue;

		// Token: 0x04001E94 RID: 7828
		public string strDefault;

		// Token: 0x04001E95 RID: 7829
		public string strValue;

		// Token: 0x04001E96 RID: 7830
		public ToggleController toggle;

		// Token: 0x04001E97 RID: 7831
		public SliderController slider;

		// Token: 0x04001E98 RID: 7832
		public DropdownController dropdown;

		// Token: 0x04001E99 RID: 7833
		public DropdownController secondaryDropdown;

		// Token: 0x04001E9A RID: 7834
		public MultiSelectController multiselect;

		// Token: 0x04001E9B RID: 7835
		public TextMeshProUGUI valueDisplayText;

		// Token: 0x04001E9C RID: 7836
		public bool lateLoad;

		// Token: 0x04001E9D RID: 7837
		public bool useDropdownInt;
	}
}
