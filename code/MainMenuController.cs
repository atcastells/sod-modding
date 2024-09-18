using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AeLa.EasyFeedback;
using NaughtyAttributes;
using Rewired.Demos;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000523 RID: 1315
public class MainMenuController : MonoBehaviour
{
	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06001C7E RID: 7294 RVA: 0x00196CF1 File Offset: 0x00194EF1
	public static MainMenuController Instance
	{
		get
		{
			return MainMenuController._instance;
		}
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x00196CF8 File Offset: 0x00194EF8
	private void Awake()
	{
		if (MainMenuController._instance != null && MainMenuController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		MainMenuController._instance = this;
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x00196D28 File Offset: 0x00194F28
	private void Start()
	{
		if (this.buildText != null)
		{
			if (Game.Instance.displayBuildInMenu)
			{
				this.buildText.text = "v" + Game.Instance.buildID;
			}
			else
			{
				this.buildText.text = string.Empty;
			}
		}
		if (Toolbox.Instance.IsConsoleBuild())
		{
			foreach (ButtonController buttonController in Game.Instance.removeButtonsOnConsoleVersion)
			{
				if (buttonController != null)
				{
					Object.Destroy(buttonController.gameObject);
				}
			}
		}
		if (this.feedbackPlayerInfo != null)
		{
			this.feedbackPlayerInfo.version = Game.Instance.buildDescription;
		}
		if (Game.Instance.displayBetaMessage && this.betaMessageText != null)
		{
			this.betaMessageText.gameObject.SetActive(true);
		}
		else
		{
			Object.Destroy(this.betaMessageText.gameObject);
		}
		if (!Directory.Exists(Application.persistentDataPath + "/Save"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Save");
		}
		if (!Directory.Exists(Application.persistentDataPath + "/Cities"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Cities");
		}
		this.loadingSlider.value = 0f;
		this.tipText.text = string.Empty;
		this.tipImg.color = Color.clear;
		this.mouseOverText.text = string.Empty;
		foreach (MainMenuController.MenuComponent menuComponent in this.components)
		{
			if (menuComponent.rect != null)
			{
				menuComponent.onscreenAnchoredPosition = menuComponent.rect.anchoredPosition;
				if (menuComponent.xPhase > this.currentComponent.xPhase)
				{
					menuComponent.rect.anchoredPosition = new Vector2(PrefabControls.Instance.menuCanvas.sizeDelta.x, menuComponent.rect.anchoredPosition.y);
				}
				else if (menuComponent.xPhase < this.currentComponent.xPhase)
				{
					menuComponent.rect.anchoredPosition = new Vector2(-PrefabControls.Instance.menuCanvas.sizeDelta.x, menuComponent.rect.anchoredPosition.y);
				}
				ButtonController[] componentsInChildren = menuComponent.rect.gameObject.GetComponentsInChildren<ButtonController>();
				menuComponent.buttons = Enumerable.ToList<ButtonController>(componentsInChildren);
				foreach (ButtonController buttonController2 in menuComponent.buttons)
				{
					if (buttonController2.button != null)
					{
						buttonController2.button.interactable = false;
					}
				}
			}
		}
		this.LoadDropdownContent();
		PlayerPrefsController.Instance.LoadPlayerPrefs(true);
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (SessionData.Instance.isTestScene)
		{
			return;
		}
		if (this.bugReportButton != null && !Game.Instance.enableBugReporting)
		{
			Object.Destroy(this.bugReportButton.gameObject);
		}
		if (this.feedbackButton != null && !Game.Instance.enableFeedbackFormLink)
		{
			Object.Destroy(this.feedbackButton.gameObject);
		}
		if (this.modsButton != null && !Game.Instance.allowMods)
		{
			Object.Destroy(this.modsButton.gameObject);
		}
		if (RestartSafeController.Instance.loadFromDirty)
		{
			RestartSafeController.Instance.loadFromDirty = false;
			if (RestartSafeController.Instance.newGameLoadCity)
			{
				this.EnableMainMenu(true, true, false, MainMenuController.Component.mainMenuButtons);
				this.SetMenuComponent(MainMenuController.Component.loadingCity);
			}
			else if (RestartSafeController.Instance.loadSaveGame)
			{
				this.EnableMainMenu(true, true, false, MainMenuController.Component.mainMenuButtons);
				Game.Log("CityGen: Attempting to load save game after restart...", 2);
				this.SetMenuComponent(MainMenuController.Component.loadingCity);
			}
			else if (RestartSafeController.Instance.generateNew)
			{
				this.EnableMainMenu(true, true, false, MainMenuController.Component.mainMenuButtons);
				Game.Log("CityGen: Attempting to generate new city after restart...", 2);
				this.SetMenuComponent(MainMenuController.Component.loadingCity);
			}
		}
		else
		{
			MainMenuController.Component menuPhase = MainMenuController.Component.mainMenuButtons;
			this.EnableMainMenu(true, false, false, menuPhase);
			this.citySizeDropdown.SelectFromStaticOption("medium");
			this.cityPopDropdown.SelectFromStaticOption("maximum");
			this.OnGenerateNewSeed();
			this.RandomCityName();
		}
		if (this.setWeatherButton != null)
		{
			this.setWeatherButton.onClick.AddListener(delegate()
			{
				SessionData.Instance.SetWeather(this.rainSlider.normalizedValue, this.windSlider.normalizedValue, this.snowSlider.normalizedValue, this.lightningSlider.normalizedValue, this.fogSlider.normalizedValue, 0.1f, false);
			});
		}
		this.playerSkinToneSelect.OnSelect += this.OnSkinToneChange;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs != null)
		{
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i].Trim().ToLower() == "-uncleeden")
				{
					Game.Instance.devMode = true;
				}
				if (commandLineArgs[i].Trim().ToLower() == "-resetprefs")
				{
					PlayerPrefs.DeleteAll();
				}
			}
		}
		this.loadedLanguage = PlayerPrefsController.Instance.GetSettingStr("language");
		this.developerOptionsButton.SetInteractable(Game.Instance.devMode);
		this.developerOptionsButton.gameObject.SetActive(Game.Instance.devMode);
		this.generationWarningText.text = Strings.Get("ui.interface", "Depending on size, new cities may take a long time to generate...", Strings.Casing.asIs, false, false, false, null);
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x001972FC File Offset: 0x001954FC
	public void LoadDropdownContent()
	{
		this.languageDropdown.dropdown.ClearOptions();
		Application.streamingAssetsPath + "/Strings/";
		try
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < LanguageConfigLoader.Instance.fileInputConfig.Count; i++)
			{
				LanguageConfigLoader.LocInput locInput = LanguageConfigLoader.Instance.fileInputConfig[i];
				if (locInput != null && locInput.languageCode != null && locInput.languageCode.Length > 0)
				{
					list.Add(locInput.languageCode);
					if (locInput.displayName != null && locInput.displayName.Length > 0)
					{
						list2.Add(locInput.displayName);
					}
					else
					{
						list2.Add(locInput.languageCode);
					}
				}
			}
			this.languageDropdown.AddOptions(list, false, list2);
			this.languageDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("language"));
		}
		catch
		{
		}
		Resolution[] resolutions = Screen.resolutions;
		if (this.resolutionsDropdown != null)
		{
			List<string> list3 = new List<string>();
			foreach (Resolution resolution in resolutions)
			{
				list3.Add(resolution.ToString());
			}
			string text = string.Concat(new string[]
			{
				Screen.width.ToString(),
				" x ",
				Screen.height.ToString(),
				" @ ",
				Screen.currentResolution.refreshRate.ToString(),
				"Hz"
			});
			if (!list3.Contains(text))
			{
				list3.Add(text);
			}
			this.resolutionsDropdown.AddOptions(list3, false, null);
			this.resolutionsDropdown.dropdown.SetValueWithoutNotify(list3.IndexOf(text));
		}
		if (this.fullScreenModeDropdown != null)
		{
			List<FullScreenMode> list4 = Enumerable.ToList<FullScreenMode>(Enumerable.Cast<FullScreenMode>(Enum.GetValues(typeof(FullScreenMode))));
			List<string> list5 = new List<string>();
			for (int k = 0; k < list4.Count; k++)
			{
				list5.Add(list4[k].ToString());
			}
			this.fullScreenModeDropdown.AddOptions(list5, true, null);
			this.fullScreenModeDropdown.SelectFromStaticOption(Screen.fullScreenMode.ToString());
		}
		if (this.aaModeDropdown != null)
		{
			List<HDAdditionalCameraData.AntialiasingMode> list6 = Enumerable.ToList<HDAdditionalCameraData.AntialiasingMode>(Enumerable.Cast<HDAdditionalCameraData.AntialiasingMode>(Enum.GetValues(typeof(HDAdditionalCameraData.AntialiasingMode))));
			List<string> list7 = new List<string>();
			for (int l = 0; l < list6.Count; l++)
			{
				list7.Add(list6[l].ToString());
			}
			this.aaModeDropdown.AddOptions(list7, true, null);
			this.aaModeDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("aaMode"));
		}
		if (this.aaQualityDropdown != null)
		{
			List<HDAdditionalCameraData.SMAAQualityLevel> list8 = Enumerable.ToList<HDAdditionalCameraData.SMAAQualityLevel>(Enumerable.Cast<HDAdditionalCameraData.SMAAQualityLevel>(Enum.GetValues(typeof(HDAdditionalCameraData.SMAAQualityLevel))));
			List<string> list9 = new List<string>();
			for (int m = 0; m < list8.Count; m++)
			{
				list9.Add(list8[m].ToString());
			}
			this.aaQualityDropdown.AddOptions(list9, true, null);
			this.aaQualityDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("aaQuality"));
		}
		if (this.dlssModeDropdown != null)
		{
			List<DynamicResolutionController.DLSSQuality> list10 = Enumerable.ToList<DynamicResolutionController.DLSSQuality>(Enumerable.Cast<DynamicResolutionController.DLSSQuality>(Enum.GetValues(typeof(DynamicResolutionController.DLSSQuality))));
			List<string> list11 = new List<string>();
			for (int n = 0; n < list10.Count; n++)
			{
				list11.Add(list10[n].ToString());
			}
			this.dlssModeDropdown.AddOptions(list11, true, null);
			this.dlssModeDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("dlssMode"));
		}
		if (this.gameDifficultyDropdown != null)
		{
			List<string> list12 = new List<string>();
			list12.Add("Easy");
			list12.Add("Normal");
			list12.Add("Hard");
			list12.Add("Extreme");
			this.gameDifficultyDropdown.AddOptions(list12, true, null);
			this.gameDifficultyDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("gameDifficulty"));
			if (this.gameDifficultyDropdown2 != null)
			{
				this.gameDifficultyDropdown2.AddOptions(list12, true, null);
				this.gameDifficultyDropdown2.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("gameDifficulty"));
			}
		}
		if (this.gameLengthDropdown != null)
		{
			List<string> list13 = new List<string>();
			list13.Add("Very Short");
			list13.Add("Short");
			list13.Add("Normal");
			list13.Add("Long");
			list13.Add("Very Long");
			this.gameLengthDropdown.AddOptions(list13, true, null);
			this.gameLengthDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("gameLength"));
		}
		if (this.startTimeDropdown != null)
		{
			List<string> list14 = new List<string>();
			list14.Add("Morning");
			list14.Add("Midday");
			list14.Add("Evening");
			list14.Add("Midnight");
			this.startTimeDropdown.AddOptions(list14, true, null);
			this.startTimeDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("startTime"));
		}
		if (this.playerGenderDropdown != null && this.partnerGenderDropdown != null)
		{
			List<string> list15 = new List<string>();
			list15.Add("Male");
			list15.Add("Female");
			list15.Add("Non-Binary");
			this.playerGenderDropdown.AddOptions(list15, true, null);
			this.playerGenderDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("startingGender"));
			this.OnPlayerGenderChange();
			this.partnerGenderDropdown.AddOptions(list15, true, null);
			this.partnerGenderDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("startingPartnerGender"));
			this.OnPartnerGenderChange();
		}
		if (this.citySizeDropdown != null)
		{
			List<string> list16 = new List<string>();
			list16.Add("small");
			if (!Game.Instance.smallCitiesOnly)
			{
				list16.Add("medium");
				list16.Add("large");
				list16.Add("veryLarge");
			}
			this.citySizeDropdown.AddOptions(list16, true, null);
			this.citySizeDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("cityGenSize"));
		}
		if (this.cityPopDropdown != null)
		{
			List<string> list17 = new List<string>();
			list17.Add("ultraLow");
			list17.Add("veryLow");
			list17.Add("low");
			list17.Add("medium");
			list17.Add("high");
			list17.Add("veryHigh");
			list17.Add("maximum");
			this.cityPopDropdown.AddOptions(list17, true, null);
			this.cityPopDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("cityGenPop"));
		}
		if (this.statusEffectsDropdown != null)
		{
			List<string> list18 = new List<string>();
			list18.Add("All");
			list18.Add("Only Positive");
			list18.Add("Only Negative");
			list18.Add("None");
			list18.Add("Custom");
			this.statusEffectsDropdown.AddOptions(list18, true, null);
			this.SetDropdownAccordingToStatusEffects();
		}
		if (this.categoryDropdown != null)
		{
			List<string> list19 = new List<string>();
			list19.Add("General Bugs");
			list19.Add("AI Bugs");
			list19.Add("Geometry Bugs");
			list19.Add("Feedback");
			this.categoryDropdown.AddOptions(list19, true, null);
			this.categoryDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("bugCategory"));
		}
		if (this.priorityDropdown != null)
		{
			List<string> list20 = new List<string>();
			list20.Add("Low");
			list20.Add("Medium");
			list20.Add("High");
			this.priorityDropdown.AddOptions(list20, true, null);
			this.priorityDropdown.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("bugPriority"));
		}
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x00197BB8 File Offset: 0x00195DB8
	public void OnNewMouseOver()
	{
		if (!(InterfaceController.Instance.selectedElement != null))
		{
			this.mouseOverText.text = string.Empty;
			return;
		}
		if (InterfaceController.Instance.selectedElement.menuMouseoverReference.Length > 0)
		{
			this.mouseOverText.text = Strings.Get("ui.interface", InterfaceController.Instance.selectedElement.menuMouseoverReference, Strings.Casing.asIs, false, false, false, null);
			return;
		}
		this.mouseOverText.text = string.Empty;
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x00197C3C File Offset: 0x00195E3C
	public void EnableMainMenu(bool val, bool useFade = false, bool exitMain = false, MainMenuController.Component menuPhase = MainMenuController.Component.mainMenuButtons)
	{
		if (!this.mainMenuActive && val && useFade)
		{
			this.fadeOverlay.gameObject.SetActive(true);
			this.fadeOverlay.SetAlpha(1f);
			this.fade = 1f;
		}
		this.SelectNewSave(null);
		this.mainMenuActive = val;
		Game.Log("Menu: Main menu active: " + this.mainMenuActive.ToString(), 2);
		if (this.mainMenuActive)
		{
			this.mainMenuContainer.gameObject.SetActive(true);
			this.exitMainMenuAfterFade = false;
			this.time = 0f;
			this.loadingSlider.value = 0f;
			InterfaceController.Instance.SetInterfaceActive(false);
			PrefabControls.Instance.menuCanvas.gameObject.SetActive(true);
			MusicController.Instance.SetGameState(MusicCue.MusicTriggerGameState.menu);
			this.SetMenuComponent(menuPhase);
		}
		else
		{
			this.exitMainMenuAfterFade = exitMain;
			if (!useFade)
			{
				InterfaceController.Instance.SetInterfaceActive(true);
				PrefabControls.Instance.menuCanvas.gameObject.SetActive(false);
			}
			MusicController.Instance.SetGameState(MusicCue.MusicTriggerGameState.inGame);
			Player.Instance.UpdatePlayerAmbientState();
			this.SetMenuComponent(MainMenuController.Component.none);
		}
		if (this.mainMenuActive)
		{
			this.desiredFade = 0f;
		}
		else
		{
			this.desiredFade = 1f;
		}
		if (useFade)
		{
			base.StopCoroutine("FadeMenu");
			base.StartCoroutine("FadeMenu");
			return;
		}
		this.fade = this.desiredFade;
	}

	// Token: 0x06001C84 RID: 7300 RVA: 0x00197DAC File Offset: 0x00195FAC
	private void Update()
	{
		if (this.mainMenuActive)
		{
			this.time += Time.deltaTime;
			this.topBarRend.SetAlpha(Mathf.Clamp01(this.time * 0.1f));
			this.bottomBarRend.SetAlpha(Mathf.Clamp01(this.time * 0.1f));
			this.titleText.characterSpacing = this.titleTextKerningAnimation.Evaluate(this.time);
		}
		if (this.bugReportTimer > 0f)
		{
			this.bugReportTimer -= Time.deltaTime;
		}
		if (!PlayerPrefsController.Instance.acceptedEULA && !this.acceptedEULA && this.currentComponent.component != MainMenuController.Component.loadingCity && !PopupMessageController.Instance.active && !Toolbox.Instance.IsConsoleBuild())
		{
			PopupMessageController.Instance.PopupMessage("EULA", true, true, "Decline", "Accept", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", true, "", "");
			PopupMessageController.Instance.OnLeftButton += this.DeclineEULA;
			PopupMessageController.Instance.OnRightButton += this.AcceptEULA;
		}
		if (!PlayerPrefsController.Instance.playedBefore && !this.askedStreamerQuestion && this.currentComponent.component != MainMenuController.Component.loadingCity && !PopupMessageController.Instance.active)
		{
			PopupMessageController.Instance.PopupMessage("Streamer Mode", true, true, "Keep Enabled", "Disable", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.CancelStreamerMode;
			PopupMessageController.Instance.OnRightButton += this.SetToStreamerMode;
			this.askedStreamerQuestion = true;
		}
	}

	// Token: 0x06001C85 RID: 7301 RVA: 0x00197FA5 File Offset: 0x001961A5
	private IEnumerator FadeMenu()
	{
		this.desiredFade = Mathf.Clamp01(this.desiredFade);
		Game.Log("Menu: Execute menu fade: " + this.fade.ToString() + "/" + this.desiredFade.ToString(), 2);
		if (!this.mainMenuActive && !CameraController.Instance.cam.enabled)
		{
			CameraController.Instance.cam.enabled = true;
		}
		while (this.desiredFade != this.fade)
		{
			if (PopupMessageController.Instance != null && PopupMessageController.Instance.active)
			{
				yield return null;
			}
			else if (this.fade < this.desiredFade)
			{
				this.fade += 2f * Time.deltaTime;
			}
			else if (this.fade > this.desiredFade)
			{
				this.fade -= 2f * Time.deltaTime;
			}
			this.fade = Mathf.Clamp01(this.fade);
			this.fadeOverlay.SetAlpha(this.fade);
			if (this.exitMainMenuAfterFade)
			{
				if (this.fade >= 1f)
				{
					if (!SessionData.Instance.isCityEdit)
					{
						InterfaceController.Instance.SetInterfaceActive(true);
					}
					this.mainMenuContainer.gameObject.SetActive(false);
					this.desiredFade = 0f;
				}
				else if (this.fade <= 0f && this.desiredFade <= 0f)
				{
					PrefabControls.Instance.menuCanvas.gameObject.SetActive(false);
				}
			}
			yield return null;
		}
		if (this.mainMenuActive && CameraController.Instance.cam.enabled)
		{
			CameraController.Instance.cam.enabled = false;
		}
		yield break;
	}

	// Token: 0x06001C86 RID: 7302 RVA: 0x00197FB4 File Offset: 0x001961B4
	public void SetMenuComponent(int newComponent)
	{
		this.SetMenuComponent((MainMenuController.Component)newComponent);
	}

	// Token: 0x06001C87 RID: 7303 RVA: 0x00197FC0 File Offset: 0x001961C0
	public void SetMenuComponent(MainMenuController.Component newComponent)
	{
		if (newComponent == MainMenuController.Component.generateCity && Game.Instance.disableCityGeneration)
		{
			return;
		}
		if (newComponent == MainMenuController.Component.saveGame && Game.Instance.disableSaveLoadGames)
		{
			return;
		}
		if (newComponent == MainMenuController.Component.loadGame && Game.Instance.disableSaveLoadGames)
		{
			return;
		}
		if (InterfaceController.Instance.selectedElement != null)
		{
			Game.Log("Menu: Deselect button " + InterfaceController.Instance.selectedElement.name + " through selection", 2);
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		Game.Log("Menu: Set menu component: " + newComponent.ToString(), 2);
		if (newComponent != this.currentComponent.component)
		{
			if (this.currentComponent != null)
			{
				this.previousComponent = this.currentComponent.component;
			}
			if (newComponent == MainMenuController.Component.credits)
			{
				this.creditsText.text = CreditsController.Instance.GetFormattedCreditsText();
				this.creditsPageContent.sizeDelta = new Vector2(this.creditsPageContent.sizeDelta.x, this.creditsText.GetPreferredValues().y + 32f);
			}
			else if (newComponent == MainMenuController.Component.bugReport)
			{
				this.RefreshSaveGameDropdown();
				this.ResetBugReportDetails();
				this.OnOpenBugReport();
			}
			if (this.previousComponent == MainMenuController.Component.bugReport && newComponent != MainMenuController.Component.bugReport)
			{
				this.OnCloseBugReport();
			}
		}
		this.currentComponent = this.components.Find((MainMenuController.MenuComponent item) => item.component == newComponent);
		this.componentMotion = 0f;
		base.StartCoroutine(this.MenuMotion(this.currentComponent.skipMotion));
	}

	// Token: 0x06001C88 RID: 7304 RVA: 0x00198180 File Offset: 0x00196380
	public void SetToStreamerMode()
	{
		Game.Log("Menu: Setting to streamer safe mode", 2);
		PopupMessageController.Instance.OnLeftButton -= this.CancelStreamerMode;
		PopupMessageController.Instance.OnRightButton -= this.SetToStreamerMode;
		PlayerPrefs.SetInt("licensedMusic", 0);
		PlayerPrefsController.Instance.gameSettingControls.Find((PlayerPrefsController.GameSetting item) => item.identifier == "licensedMusic").intValue = 0;
		this.allowLicensedMusicToggle.SetOff();
		PlayerPrefsController.Instance.OnToggleChanged("licensedmusic", false, null);
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x0019821F File Offset: 0x0019641F
	public void CancelStreamerMode()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelStreamerMode;
		PopupMessageController.Instance.OnRightButton -= this.SetToStreamerMode;
	}

	// Token: 0x06001C8A RID: 7306 RVA: 0x0019824D File Offset: 0x0019644D
	public void AcceptEULA()
	{
		PopupMessageController.Instance.OnLeftButton -= this.DeclineEULA;
		PopupMessageController.Instance.OnRightButton -= this.AcceptEULA;
		PlayerPrefs.SetInt("EULA", 1);
		this.acceptedEULA = true;
	}

	// Token: 0x06001C8B RID: 7307 RVA: 0x0019828D File Offset: 0x0019648D
	public void DeclineEULA()
	{
		PopupMessageController.Instance.OnLeftButton -= this.DeclineEULA;
		PopupMessageController.Instance.OnRightButton -= this.AcceptEULA;
		Application.Quit();
	}

	// Token: 0x06001C8C RID: 7308 RVA: 0x001982C0 File Offset: 0x001964C0
	private IEnumerator MenuMotion(bool skipMotion)
	{
		while (this.componentMotion < 1f)
		{
			this.componentMotion += Time.deltaTime / 0.5f;
			if (skipMotion)
			{
				this.componentMotion = 1f;
			}
			this.componentMotion = Mathf.Clamp01(this.componentMotion);
			foreach (MainMenuController.MenuComponent menuComponent in this.components)
			{
				foreach (ButtonController buttonController in menuComponent.buttons)
				{
					if (buttonController.button != null)
					{
						buttonController.button.interactable = false;
					}
				}
				if (menuComponent == this.currentComponent && menuComponent.rect != null)
				{
					menuComponent.rect.gameObject.SetActive(true);
					menuComponent.rect.anchoredPosition = Vector2.Lerp(menuComponent.rect.anchoredPosition, menuComponent.onscreenAnchoredPosition, this.componentMotion);
				}
				else if (menuComponent.rect != null)
				{
					if (menuComponent.xPhase > this.currentComponent.xPhase)
					{
						menuComponent.rect.anchoredPosition = Vector2.Lerp(menuComponent.rect.anchoredPosition, new Vector2(PrefabControls.Instance.menuCanvas.sizeDelta.x, menuComponent.rect.anchoredPosition.y), this.componentMotion);
					}
					else if (menuComponent.xPhase < this.currentComponent.xPhase)
					{
						menuComponent.rect.anchoredPosition = Vector2.Lerp(menuComponent.rect.anchoredPosition, new Vector2(-PrefabControls.Instance.menuCanvas.sizeDelta.x, menuComponent.rect.anchoredPosition.y), this.componentMotion);
					}
				}
			}
			yield return null;
		}
		foreach (MainMenuController.MenuComponent menuComponent2 in this.components)
		{
			if (menuComponent2 != this.currentComponent && menuComponent2.rect != null)
			{
				menuComponent2.rect.gameObject.SetActive(false);
			}
			else
			{
				foreach (ButtonController buttonController2 in menuComponent2.buttons)
				{
					if (buttonController2.interactable && buttonController2.button != null)
					{
						buttonController2.button.interactable = true;
					}
				}
				if (this.currentComponent.component == MainMenuController.Component.mainMenuButtons)
				{
					this.saveGameButton.SetInteractable(this.IsSaveGameAllowed());
					this.loadGameButton.SetInteractable(!Game.Instance.disableSaveLoadGames);
					this.resumeGameButton.SetInteractable(SessionData.Instance.startedGame);
					this.helpButton.SetInteractable(SessionData.Instance.startedGame);
				}
				else if (this.currentComponent.component == MainMenuController.Component.citySelect)
				{
					this.cityGenButton.SetInteractable(!Game.Instance.disableCityGeneration);
				}
				else if (this.currentComponent.component == MainMenuController.Component.newGameSelect)
				{
					this.sandboxGameButton.SetInteractable(!Game.Instance.disableSandbox);
				}
			}
		}
		this.OnMenuComponentSwitchComplete();
		yield break;
	}

	// Token: 0x06001C8D RID: 7309 RVA: 0x001982D8 File Offset: 0x001964D8
	public bool IsSaveGameAllowed()
	{
		return !Game.Instance.disableSaveLoadGames && !Player.Instance.transitionActive && Player.Instance.answeringPhone == null && SessionData.Instance.startedGame && !CutSceneController.Instance.cutSceneActive;
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x00198324 File Offset: 0x00196524
	public void OnMenuComponentSwitchComplete()
	{
		if (this.currentComponent == null)
		{
			return;
		}
		if (this.currentComponent.component == this.previousComponent)
		{
			return;
		}
		Game.Log("Menu: Current menu component switch complete: " + this.currentComponent.component.ToString(), 2);
		if (this.currentComponent.component == MainMenuController.Component.loadingCity)
		{
			if (CityConstructor.Instance == null)
			{
				PrefabControls.Instance.mapContainer.gameObject.AddComponent<CityConstructor>();
			}
			if (CityConstructor.Instance.enabled)
			{
				CityConstructor.Instance.Cancel();
			}
			this.loadingSlider.value = 0f;
			if (SessionData.Instance.dirtyScene)
			{
				RestartSafeController.Instance.loadFromDirty = true;
				Game.Log("CityGen: Scene is dirty, restarting to continue load process...", 2);
				AudioController.Instance.StopAllSounds();
				SceneManager.LoadScene("Main");
				return;
			}
			Game.Log("CityGen: Scene is clean, continuing with load process...", 2);
			if (CityConstructor.Instance.IsUsingCityEditor())
			{
				HighlanderSingleton<CityEditorController>.Instance.SetCityEditor(true);
				return;
			}
			if (RestartSafeController.Instance.loadSaveGame)
			{
				CityConstructor.Instance.LoadSaveGame();
				return;
			}
			if (RestartSafeController.Instance.generateNew)
			{
				CityConstructor.Instance.GenerateNewCity();
				return;
			}
			if (RestartSafeController.Instance.newGameLoadCity)
			{
				CityConstructor.Instance.LoadCityStartNewGame();
			}
		}
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x0019846C File Offset: 0x0019666C
	public void SelectCityButton()
	{
		Game.Log("Menu: Select city", 2);
		RestartSafeController.Instance.newGameLoadCity = true;
		RestartSafeController.Instance.generateNew = false;
		RestartSafeController.Instance.loadSaveGame = false;
		this.RefreshMapDropdown();
		if (this.selectCityDropdown.dropdown.options.Count <= 0)
		{
			this.LoadCityInfo(null);
			return;
		}
		if (RestartSafeController.Instance.loadCityFileInfo != null)
		{
			this.LoadCityInfo(RestartSafeController.Instance.loadCityFileInfo);
			return;
		}
		string @string = PlayerPrefs.GetString("lastCity");
		FileInfo fileInfo = null;
		foreach (FileInfo fileInfo2 in this.cityMapFiles)
		{
			if (fileInfo2.Name == @string)
			{
				fileInfo = fileInfo2;
				break;
			}
		}
		if (fileInfo != null)
		{
			this.LoadCityInfo(fileInfo);
			return;
		}
		this.LoadCityInfo(this.cityMapFiles[this.selectCityDropdown.dropdown.value]);
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x00198578 File Offset: 0x00196778
	private void RefreshMapDropdown()
	{
		this.cityMapFiles.Clear();
		this.cityInfoFiles.Clear();
		this.cityInfoDict.Clear();
		foreach (FileInfo fileInfo in Enumerable.ToList<FileInfo>(new DirectoryInfo(Application.streamingAssetsPath + "/Cities").GetFiles()))
		{
			if (fileInfo.Extension.ToLower() == ".cit" || fileInfo.Extension.ToLower() == ".citb")
			{
				this.cityMapFiles.Add(fileInfo);
			}
			else if (fileInfo.Extension.ToLower() == ".txt")
			{
				this.cityInfoFiles.Add(fileInfo);
			}
		}
		if (Game.Instance.allowMods)
		{
			foreach (FileInfo fileInfo2 in ModLoader.Instance.GetActiveCities())
			{
				if (fileInfo2.Extension.ToLower() == ".cit" || fileInfo2.Extension.ToLower() == ".citb")
				{
					this.cityMapFiles.Add(fileInfo2);
				}
				else if (fileInfo2.Extension.ToLower() == ".txt")
				{
					this.cityInfoFiles.Add(fileInfo2);
				}
			}
		}
		if (!Directory.Exists(Application.persistentDataPath + "/Cities"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Cities");
		}
		foreach (FileInfo fileInfo3 in Enumerable.ToList<FileInfo>(new DirectoryInfo(Application.persistentDataPath + "/Cities").GetFiles()))
		{
			if (fileInfo3.Extension.ToLower() == ".cit" || fileInfo3.Extension.ToLower() == ".citb")
			{
				this.cityMapFiles.Add(fileInfo3);
			}
			else if (fileInfo3.Extension.ToLower() == ".txt")
			{
				this.cityInfoFiles.Add(fileInfo3);
			}
		}
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < this.cityMapFiles.Count; i++)
		{
			FileInfo fileInfo4 = this.cityMapFiles[i];
			string text = fileInfo4.Name.Split('.', 0)[0];
			if (!this.cityInfoDict.ContainsKey(fileInfo4.Name))
			{
				FileInfo fileInfo5 = null;
				CityInfoData cityInfoData = null;
				foreach (FileInfo fileInfo6 in this.cityInfoFiles)
				{
					if (fileInfo6.Name.Substring(0, fileInfo6.Name.Length - fileInfo6.Extension.Length) == fileInfo4.Name.Substring(0, fileInfo4.Name.Length - fileInfo4.Extension.Length))
					{
						fileInfo5 = fileInfo6;
						break;
					}
				}
				if (fileInfo5 != null)
				{
					using (StreamReader streamReader = File.OpenText(fileInfo5.FullName))
					{
						cityInfoData = JsonUtility.FromJson<CityInfoData>(streamReader.ReadToEnd());
						goto IL_35D;
					}
					goto IL_34F;
				}
				goto IL_34F;
				IL_35D:
				this.cityInfoDict.Add(fileInfo4.Name, cityInfoData);
				goto IL_371;
				IL_34F:
				cityInfoData = Toolbox.Instance.GenerateCityInfoFile(fileInfo4);
				goto IL_35D;
			}
			IL_371:
			list.Add(text);
			list2.Add(fileInfo4.Name);
		}
		this.selectCityDropdown.AddOptions(list2, false, list);
	}

	// Token: 0x06001C91 RID: 7313 RVA: 0x00198974 File Offset: 0x00196B74
	public void OnNewCitySelected()
	{
		if (this.selectCityDropdown.dropdown.options.Count > 0)
		{
			this.LoadCityInfo(this.cityMapFiles[this.selectCityDropdown.dropdown.value]);
			return;
		}
		this.LoadCityInfo(null);
	}

	// Token: 0x06001C92 RID: 7314 RVA: 0x001989C4 File Offset: 0x00196BC4
	public void LoadCityInfo(FileInfo fileInfo)
	{
		Game.Log("Menu: Load city info: " + fileInfo.FullName, 2);
		RestartSafeController.Instance.loadCityFileInfo = fileInfo;
		string text = string.Empty;
		if (fileInfo != null)
		{
			text = RestartSafeController.Instance.loadCityFileInfo.Name.Split('.', 0)[0];
		}
		if (text != null && text.Length > 0 && this.cityInfoDict.ContainsKey(fileInfo.Name))
		{
			this.selectedCityInfoData = this.cityInfoDict[fileInfo.Name];
			Game.Log("Menu: Loading city info " + fileInfo.Name + ": " + this.selectedCityInfoData.shareCode, 2);
			int num = CityControls.Instance.citySizes.FindIndex((CityControls.CitySize item) => Mathf.RoundToInt(item.v2.x) == Mathf.RoundToInt(this.selectedCityInfoData.citySize.x) && Mathf.RoundToInt(item.v2.y) == Mathf.RoundToInt(this.selectedCityInfoData.citySize.y));
			if (num < 0)
			{
				num = 0;
			}
			string text2 = this.citySizeDropdown.dropdown.options[num].text;
			this.selectedCityDetailsText.text = string.Concat(new string[]
			{
				text,
				"\n",
				Strings.Get("ui.interface", "City Size", Strings.Casing.asIs, false, false, false, null),
				": ",
				text2,
				"\n",
				Strings.Get("ui.interface", "Population", Strings.Casing.asIs, false, false, false, null),
				": ",
				this.selectedCityInfoData.population.ToString()
			});
			this.selectedCityShareCode.text = Strings.Get("ui.interface", "Share Code", Strings.Casing.asIs, false, false, false, null) + ": " + this.selectedCityInfoData.shareCode;
			this.selectedCityContinueButton.SetInteractable(true);
			this.selectedCityCopyShareCodeButton.SetInteractable(true);
			if (fileInfo.DirectoryName.Contains("StreamingAssets"))
			{
				Game.Log("Menu: " + fileInfo.DirectoryName + ": Disabling deletion button", 2);
				this.deleteCityButton.SetInteractable(false);
			}
			else
			{
				Game.Log("Menu: " + fileInfo.DirectoryName + ": Enabling deletion button", 2);
				this.deleteCityButton.SetInteractable(true);
			}
			PlayerPrefs.SetString("lastCity", fileInfo.Name);
			this.selectCityDropdown.SelectFromStaticOption(fileInfo.Name);
			RestartSafeController.Instance.generateNew = false;
			return;
		}
		this.selectedCityInfoData = null;
		RestartSafeController.Instance.loadCityFileInfo = null;
		this.selectedCityDetailsText.text = string.Empty;
		this.selectedCityShareCode.text = string.Empty;
		this.selectedCityContinueButton.SetInteractable(false);
		this.selectedCityCopyShareCodeButton.SetInteractable(false);
		this.deleteCityButton.SetInteractable(false);
		RestartSafeController.Instance.generateNew = true;
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x00198C74 File Offset: 0x00196E74
	public void SelectGenNewCity()
	{
		Game.Log("Menu: Generate city", 2);
		RestartSafeController.Instance.newGameLoadCity = false;
		RestartSafeController.Instance.generateNew = true;
		RestartSafeController.Instance.loadSaveGame = false;
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x00198CA4 File Offset: 0x00196EA4
	public void RandomCityName()
	{
		List<string> list = new List<string>();
		list.Add("names.district.western");
		list.Add("names.district.eastern");
		if (Toolbox.Instance.Rand(0f, 1f, false) < 0.3f)
		{
			if (Toolbox.Instance.Rand(0f, 1f, false) < 0.5f)
			{
				RestartSafeController.Instance.cityName = NameGenerator.Instance.GenerateName("names.district.prefix", 1f, list[Toolbox.Instance.Rand(0, list.Count, false)], 1f, "names.district.suffix", 0f, "");
			}
			else
			{
				RestartSafeController.Instance.cityName = NameGenerator.Instance.GenerateName("names.district.prefix", 0f, list[Toolbox.Instance.Rand(0, list.Count, false)], 1f, "names.district.suffix", 1f, "");
			}
		}
		else
		{
			RestartSafeController.Instance.cityName = NameGenerator.Instance.GenerateName("names.district.prefix", 0f, list[Toolbox.Instance.Rand(0, list.Count, false)], 1f, "names.district.suffix", 0f, "");
		}
		this.OnChangeCityGenerationOption();
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x00198DF3 File Offset: 0x00196FF3
	public void PasteShareCode()
	{
		if (GUIUtility.systemCopyBuffer != null && GUIUtility.systemCopyBuffer.Length > 0)
		{
			this.ParseShareCode(GUIUtility.systemCopyBuffer);
		}
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x00198E14 File Offset: 0x00197014
	private void ParseShareCode(string newCode)
	{
		string cityName = "New City";
		int parsedSizeX = 5;
		int parsedSizeY = 5;
		string empty = string.Empty;
		string seed = "daifbeufgaho";
		Toolbox.Instance.ParseShareCode(newCode, out cityName, out parsedSizeX, out parsedSizeY, out empty, out seed);
		RestartSafeController.Instance.cityName = cityName;
		RestartSafeController.Instance.cityX = parsedSizeX;
		RestartSafeController.Instance.cityY = parsedSizeY;
		int num = CityControls.Instance.citySizes.FindIndex((CityControls.CitySize item) => Mathf.RoundToInt(item.v2.x) == parsedSizeX && Mathf.RoundToInt(item.v2.y) == parsedSizeY);
		if (num < 0)
		{
			num = 0;
		}
		this.citySizeDropdown.dropdown.SetValueWithoutNotify(num);
		RestartSafeController.Instance.seed = seed;
		this.OnChangeCityGenerationOption();
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x00198ED7 File Offset: 0x001970D7
	public void CopyShareCodeGenerate()
	{
		GUIUtility.systemCopyBuffer = this.shareCodeText.text;
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x00198EEC File Offset: 0x001970EC
	public void CustomShareCodeButton()
	{
		PopupMessageController.Instance.PopupMessage("shareCode", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.inputField.text = this.shareCodeText.text;
		PopupMessageController.Instance.OnLeftButton += this.OnChangeShareCodePopupCancel;
		PopupMessageController.Instance.OnRightButton += this.OnChangeShareCodePopupConfirm;
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x00198F81 File Offset: 0x00197181
	public void OnChangeShareCodePopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeShareCodePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeShareCodePopupConfirm;
	}

	// Token: 0x06001C9A RID: 7322 RVA: 0x00198FB0 File Offset: 0x001971B0
	public void OnChangeShareCodePopupConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeShareCodePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeShareCodePopupConfirm;
		this.ParseShareCode(PopupMessageController.Instance.inputField.text);
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x00198FFE File Offset: 0x001971FE
	public void OnGenerateNewSeed()
	{
		RestartSafeController.Instance.seed = Toolbox.Instance.GenerateSeed(16, false, "");
		this.OnChangeCityGenerationOption();
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x00199024 File Offset: 0x00197224
	public void OnChangeCityNameButton()
	{
		PopupMessageController.Instance.PopupMessage("cityName", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.inputField.text = RestartSafeController.Instance.cityName;
		PopupMessageController.Instance.OnLeftButton += this.OnChangeCityNamePopupCancel;
		PopupMessageController.Instance.OnRightButton += this.OnChangeCityNamePopupConfirm;
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x001990B8 File Offset: 0x001972B8
	public void OnChangeCityNamePopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeCityNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeCityNamePopupConfirm;
	}

	// Token: 0x06001C9E RID: 7326 RVA: 0x001990E8 File Offset: 0x001972E8
	public void OnChangeCityNamePopupConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeCityNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeCityNamePopupConfirm;
		RestartSafeController.Instance.cityName = Strings.FilterInputtedText(PopupMessageController.Instance.inputField.text, true, 100);
		this.OnChangeCityGenerationOption();
	}

	// Token: 0x06001C9F RID: 7327 RVA: 0x00199148 File Offset: 0x00197348
	public void OnChangeCityGenerationOption()
	{
		this.changeCityNameButton.text.text = RestartSafeController.Instance.cityName;
		RestartSafeController.Instance.cityX = Mathf.RoundToInt(CityControls.Instance.citySizes[this.citySizeDropdown.dropdown.value].v2.x);
		RestartSafeController.Instance.cityY = Mathf.RoundToInt(CityControls.Instance.citySizes[this.citySizeDropdown.dropdown.value].v2.y);
		if (Game.Instance.smallCitiesOnly)
		{
			RestartSafeController.Instance.cityX = Mathf.RoundToInt(CityControls.Instance.citySizes[0].v2.x);
			RestartSafeController.Instance.cityY = Mathf.RoundToInt(CityControls.Instance.citySizes[0].v2.y);
		}
		this.shareCodeText.text = Toolbox.Instance.GetShareCode(RestartSafeController.Instance.cityName, RestartSafeController.Instance.cityX, RestartSafeController.Instance.cityY, Game.Instance.buildID, RestartSafeController.Instance.seed);
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x00199284 File Offset: 0x00197484
	public void NewCharacter()
	{
		string @string = PlayerPrefs.GetString("lastPlayerName");
		if (@string == null || @string.Length <= 0)
		{
			this.RandomPlayerGender();
			this.RandomPartnerGender();
			this.RandomPlayerName(false);
			this.RandomSkinTone();
			return;
		}
		this.SetPlayerName(@string);
	}

	// Token: 0x06001CA1 RID: 7329 RVA: 0x001992CC File Offset: 0x001974CC
	public void SetPlayerName(string newName)
	{
		newName = newName.Trim();
		if (newName == null || newName.Length <= 0)
		{
			this.RandomPlayerName(false);
		}
		string[] array = newName.Split(new char[]
		{
			' '
		}, 1);
		if (array.Length == 0)
		{
			this.RandomPlayerName(false);
			return;
		}
		if (array.Length == 1)
		{
			RestartSafeController.Instance.newGamePlayerFirstName = array[0];
			this.RandomPlayerName(true);
			return;
		}
		RestartSafeController.Instance.newGamePlayerFirstName = array[0];
		string text = string.Empty;
		for (int i = 1; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				text += array[i];
				if (i < array.Length - 1)
				{
					text += " ";
				}
			}
		}
		RestartSafeController.Instance.newGamePlayerSurname = text;
		this.OnPlayerNameChanged();
	}

	// Token: 0x06001CA2 RID: 7330 RVA: 0x0019938C File Offset: 0x0019758C
	public void RandomPlayerName(bool surnameOnly = false)
	{
		List<Descriptors.EthnicGroup> list = new List<Descriptors.EthnicGroup>();
		foreach (SocialStatistics.EthnicityFrequency ethnicityFrequency in SocialStatistics.Instance.ethnicityFrequencies)
		{
			for (int i = 0; i < ethnicityFrequency.frequency; i++)
			{
				list.Add(ethnicityFrequency.ethnicity);
			}
		}
		Descriptors.EthnicGroup ethnicGroup = list[Toolbox.Instance.Rand(0, list.Count, false)];
		string text = "male";
		if (RestartSafeController.Instance.newGamePlayerGender == Human.Gender.female)
		{
			text = "female";
		}
		string text2;
		string text3;
		string text4;
		bool flag;
		string text5;
		if (!surnameOnly)
		{
			RestartSafeController.Instance.newGamePlayerFirstName = NameGenerator.Instance.GenerateName(null, 0f, "names." + ethnicGroup.ToString() + ".first." + text, 1f, null, 0f, out text2, out text3, out text4, out flag, out text5, "");
		}
		RestartSafeController.Instance.newGamePlayerSurname = NameGenerator.Instance.GenerateName(null, 0f, "names." + ethnicGroup.ToString() + ".sur", 1f, null, 0f, out text2, out text3, out text4, out flag, out text5, "");
		this.OnPlayerNameChanged();
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x001994E4 File Offset: 0x001976E4
	public void OnChangeNameButton()
	{
		PopupMessageController.Instance.PopupMessage("characterName", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.inputField.text = RestartSafeController.Instance.newGamePlayerFirstName + " " + RestartSafeController.Instance.newGamePlayerSurname;
		PopupMessageController.Instance.OnLeftButton += this.OnChangeNamePopupCancel;
		PopupMessageController.Instance.OnRightButton += this.OnChangeNamePopupConfirm;
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x0019958C File Offset: 0x0019778C
	public void OnChangeNamePopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeNamePopupConfirm;
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x001995BC File Offset: 0x001977BC
	public void OnChangeNamePopupConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeNamePopupConfirm;
		this.SetPlayerName(Strings.FilterInputtedText(PopupMessageController.Instance.inputField.text, true, 100));
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x00199614 File Offset: 0x00197814
	public void OnPlayerNameChanged()
	{
		Game.Log("Menu: New player name: " + RestartSafeController.Instance.newGamePlayerFirstName + " " + RestartSafeController.Instance.newGamePlayerSurname, 2);
		PlayerPrefs.SetString("lastPlayerName", RestartSafeController.Instance.newGamePlayerFirstName + " " + RestartSafeController.Instance.newGamePlayerSurname);
		this.playerNameButton.text.text = RestartSafeController.Instance.newGamePlayerFirstName + " " + RestartSafeController.Instance.newGamePlayerSurname;
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x001996A0 File Offset: 0x001978A0
	public void OnPlayerGenderChange()
	{
		RestartSafeController.Instance.newGamePlayerGender = (Human.Gender)this.playerGenderDropdown.dropdown.value;
	}

	// Token: 0x06001CA8 RID: 7336 RVA: 0x001996BC File Offset: 0x001978BC
	public void OnPartnerGenderChange()
	{
		RestartSafeController.Instance.newGamePartnerGender = (Human.Gender)this.partnerGenderDropdown.dropdown.value;
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x001996D8 File Offset: 0x001978D8
	public void RandomPlayerGender()
	{
		this.playerGenderDropdown.dropdown.value = Toolbox.Instance.Rand(0, 3, false);
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x001996F7 File Offset: 0x001978F7
	public void RandomPartnerGender()
	{
		this.partnerGenderDropdown.dropdown.value = Toolbox.Instance.Rand(0, 3, false);
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x00199716 File Offset: 0x00197916
	public void RandomSkinTone()
	{
		this.playerSkinToneSelect.SetChosen(Toolbox.Instance.Rand(0, 12, false));
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x00199731 File Offset: 0x00197931
	public void OnSkinToneChange()
	{
		RestartSafeController.Instance.newGamePlayerSkinTone = this.playerSkinToneSelect.GetCurrentSelectedColourValue();
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x00199748 File Offset: 0x00197948
	public void SaveGame()
	{
		if (Game.Instance.disableSaveLoadGames)
		{
			return;
		}
		this.RefreshSaveEntries();
		this.SelectNewSave(this.newSaveGameEntry);
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x00199769 File Offset: 0x00197969
	public void LoadGame()
	{
		if (Game.Instance.disableSaveLoadGames)
		{
			return;
		}
		this.RefreshSaveEntries();
		this.SelectNewSave(null);
		RestartSafeController.Instance.newGameLoadCity = false;
		RestartSafeController.Instance.generateNew = false;
		RestartSafeController.Instance.loadSaveGame = true;
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x001997A8 File Offset: 0x001979A8
	public void OnSaveButton()
	{
		if (Game.Instance.disableSaveLoadGames)
		{
			return;
		}
		if (this.selectedSave != null && this.selectedSave.info != null)
		{
			PopupMessageController.Instance.PopupMessage("saveGameOverwrite", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, this.selectedSave.info.Name.Substring(0, this.selectedSave.info.Name.Length - this.selectedSave.info.Extension.Length), false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.CancelOverwriteSave;
			PopupMessageController.Instance.OnRightButton += this.OverwriteSave;
			return;
		}
		PopupMessageController.Instance.PopupMessage("saveGame", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, string.Concat(new string[]
		{
			Game.Instance.playerFirstName,
			" ",
			Game.Instance.playerSurname,
			" [",
			CityData.Instance.cityName,
			" ",
			DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss.ff"),
			"]"
		}), false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnLeftButton += this.CancelOverwriteSave;
		PopupMessageController.Instance.OnRightButton += this.OverwriteSave;
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x0019995E File Offset: 0x00197B5E
	public void CancelOverwriteSave()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelOverwriteSave;
		PopupMessageController.Instance.OnRightButton -= this.OverwriteSave;
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x0019998C File Offset: 0x00197B8C
	public void OverwriteSave()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelOverwriteSave;
		PopupMessageController.Instance.OnRightButton -= this.OverwriteSave;
		this.StartSaveAsync();
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x001999C0 File Offset: 0x00197BC0
	public void StartSaveAsync()
	{
		MainMenuController.<StartSaveAsync>d__171 <StartSaveAsync>d__;
		<StartSaveAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartSaveAsync>d__.<>4__this = this;
		<StartSaveAsync>d__.<>1__state = -1;
		<StartSaveAsync>d__.<>t__builder.Start<MainMenuController.<StartSaveAsync>d__171>(ref <StartSaveAsync>d__);
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x001999F8 File Offset: 0x00197BF8
	private void SaveCompleteMessage()
	{
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Game saved", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x00199A3C File Offset: 0x00197C3C
	public void OnDeleteSaveButton()
	{
		if (this.selectedSave != null && File.Exists(this.selectedSave.info.FullName))
		{
			PopupMessageController.Instance.PopupMessage("citySave", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.CancelDeleteSave;
			PopupMessageController.Instance.OnRightButton += this.DeleteSave;
		}
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x00199ADF File Offset: 0x00197CDF
	public void CancelDeleteSave()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelDeleteSave;
		PopupMessageController.Instance.OnRightButton -= this.DeleteSave;
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x00199B10 File Offset: 0x00197D10
	public void DeleteSave()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelDeleteSave;
		PopupMessageController.Instance.OnRightButton -= this.DeleteSave;
		if (File.Exists(this.selectedSave.info.FullName) && !this.selectedSave.isInternal)
		{
			File.Delete(this.selectedSave.info.FullName);
			this.RefreshSaveEntries();
		}
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x00199B88 File Offset: 0x00197D88
	public void RefreshSaveEntries()
	{
		Game.Log("Menu: Refresh save/load entries", 2);
		this.SelectNewSave(null);
		while (this.spawnedLoadGames.Count > 0)
		{
			Object.Destroy(this.spawnedLoadGames[0].gameObject);
			this.spawnedLoadGames.RemoveAt(0);
		}
		while (this.spawnedSaveGames.Count > 0)
		{
			Object.Destroy(this.spawnedSaveGames[0].gameObject);
			this.spawnedSaveGames.RemoveAt(0);
		}
		if (!Directory.Exists(Application.persistentDataPath + "/Save"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Save");
		}
		List<FileInfo> list = Enumerable.ToList<FileInfo>(new DirectoryInfo(Application.streamingAssetsPath + "/Save").GetFiles("*.sod", 1));
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/Save");
		List<FileInfo> list2 = Enumerable.ToList<FileInfo>(directoryInfo.GetFiles("*.sod", 1));
		list2.AddRange(Enumerable.ToList<FileInfo>(directoryInfo.GetFiles("*.sodb", 1)));
		if (Game.Instance.allowMods)
		{
			list2.AddRange(ModLoader.Instance.GetActiveSaves());
		}
		float num = -4f;
		float num2 = -4f;
		GameObject gameObject = Object.Instantiate<GameObject>(this.saveGameEntryPrefab, this.saveGameContentRect);
		this.newSaveGameEntry = gameObject.GetComponent<SaveGameEntryController>();
		this.newSaveGameEntry.Setup(null);
		this.newSaveGameEntry.icon.enabled = false;
		this.spawnedSaveGames.Add(this.newSaveGameEntry);
		num -= 62f;
		num2 -= 62f;
		foreach (FileInfo newInfo in list)
		{
			SaveGameEntryController component = Object.Instantiate<GameObject>(this.saveGameEntryPrefab, this.loadGameContentRect).GetComponent<SaveGameEntryController>();
			component.Setup(newInfo);
			component.isInternal = true;
			component.icon.enabled = false;
			this.spawnedLoadGames.Add(component);
			num2 -= 62f;
			this.loadGameContentRect.sizeDelta = new Vector2(this.loadGameContentRect.sizeDelta.x, -num2);
		}
		list2.Sort((FileInfo p2, FileInfo p1) => p1.LastWriteTime.CompareTo(p2.LastWriteTime));
		foreach (FileInfo newInfo2 in list2)
		{
			SaveGameEntryController component2 = Object.Instantiate<GameObject>(this.saveGameEntryPrefab, this.saveGameContentRect).GetComponent<SaveGameEntryController>();
			component2.Setup(newInfo2);
			component2.icon.enabled = false;
			this.spawnedSaveGames.Add(component2);
			SaveGameEntryController component3 = Object.Instantiate<GameObject>(this.saveGameEntryPrefab, this.loadGameContentRect).GetComponent<SaveGameEntryController>();
			component3.Setup(newInfo2);
			component3.icon.enabled = false;
			this.spawnedLoadGames.Add(component3);
			num -= 62f;
			num2 -= 62f;
			this.saveGameContentRect.sizeDelta = new Vector2(this.saveGameContentRect.sizeDelta.x, -num);
			this.loadGameContentRect.sizeDelta = new Vector2(this.loadGameContentRect.sizeDelta.x, -num);
		}
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x00199EF4 File Offset: 0x001980F4
	public void SelectNewSave(SaveGameEntryController sec)
	{
		this.loadButton.SetInteractable(false);
		this.deleteButton.SetInteractable(false);
		this.deleteButton2.SetInteractable(false);
		foreach (SaveGameEntryController saveGameEntryController in this.spawnedLoadGames)
		{
			saveGameEntryController.icon.enabled = false;
		}
		foreach (SaveGameEntryController saveGameEntryController2 in this.spawnedSaveGames)
		{
			saveGameEntryController2.icon.enabled = false;
		}
		this.selectedSave = sec;
		if (this.selectedSave != null)
		{
			if (!this.selectedSave.isInternal)
			{
				this.deleteButton.SetInteractable(true);
				this.deleteButton2.SetInteractable(true);
			}
			this.selectedSave.icon.enabled = true;
			this.loadButton.SetInteractable(true);
			RestartSafeController.Instance.saveStateFileInfo = this.selectedSave.info;
			return;
		}
		this.loadButton.SetInteractable(false);
		this.deleteButton.SetInteractable(false);
		this.deleteButton2.SetInteractable(false);
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x0019A044 File Offset: 0x00198244
	public void DeleteCityButton()
	{
		if (File.Exists(RestartSafeController.Instance.loadCityFileInfo.FullName))
		{
			PopupMessageController.Instance.PopupMessage("cityDelete", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.CancelDeleteCity;
			PopupMessageController.Instance.OnRightButton += this.DeleteCity;
		}
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x0019A0D5 File Offset: 0x001982D5
	public void CancelDeleteCity()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelDeleteCity;
		PopupMessageController.Instance.OnRightButton -= this.DeleteCity;
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x0019A104 File Offset: 0x00198304
	public void DeleteCity()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelDeleteCity;
		PopupMessageController.Instance.OnRightButton -= this.DeleteCity;
		if (File.Exists(RestartSafeController.Instance.loadCityFileInfo.FullName))
		{
			string text = RestartSafeController.Instance.loadCityFileInfo.FullName.Substring(0, RestartSafeController.Instance.loadCityFileInfo.FullName.Length - RestartSafeController.Instance.loadCityFileInfo.Extension.Length) + ".txt";
			File.Delete(RestartSafeController.Instance.loadCityFileInfo.FullName);
			if (text != null && text.Length > 0 && File.Exists(text))
			{
				File.Delete(text);
			}
			if (CityConstructor.Instance != null)
			{
				CityConstructor.Instance.currentData = null;
			}
			this.RefreshMapDropdown();
			this.OnNewCitySelected();
		}
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x0019A1F1 File Offset: 0x001983F1
	public void ExitGame()
	{
		Application.Quit();
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x0019A1F8 File Offset: 0x001983F8
	public void ResumeGame()
	{
		MainMenuController.Instance.EnableMainMenu(false, true, true, MainMenuController.Component.mainMenuButtons);
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x0019A208 File Offset: 0x00198408
	public void Help()
	{
		MainMenuController.Instance.EnableMainMenu(false, true, true, MainMenuController.Component.mainMenuButtons);
		InterfaceController.Instance.ToggleNotebook("", true);
	}

	// Token: 0x06001CBF RID: 7359 RVA: 0x0019A228 File Offset: 0x00198428
	public void BugReport()
	{
		if (Game.Instance.enableBugReporting)
		{
			try
			{
				Game.Log("Bug report", 2);
				if (SessionData.Instance.dof != null)
				{
					this.saveDof = SessionData.Instance.dof.active;
					SessionData.Instance.dof.active = false;
				}
				this.feedbackForm.Show();
				foreach (GraphicRaycaster graphicRaycaster in PopupMessageController.Instance.otherCanvasRaycasters)
				{
					graphicRaycaster.enabled = false;
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x0019A2EC File Offset: 0x001984EC
	public void FeedbackForm()
	{
		Application.OpenURL("https://fireshinegames.jotform.com/230333480267957");
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x0019A2F8 File Offset: 0x001984F8
	public void OnFeedbackFormClosed()
	{
		try
		{
			if (SessionData.Instance.dof != null)
			{
				SessionData.Instance.dof.active = this.saveDof;
			}
			foreach (GraphicRaycaster graphicRaycaster in PopupMessageController.Instance.otherCanvasRaycasters)
			{
				if (!(graphicRaycaster == null))
				{
					graphicRaycaster.enabled = true;
				}
			}
		}
		catch
		{
			Game.LogError("Error detected while closing feedback form", 2);
		}
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x0019A39C File Offset: 0x0019859C
	public void OnOpenBugReport()
	{
		Game.Log("Menu: Opening bug report (new)...", 2);
		if (Game.Instance.enableBugReporting)
		{
			try
			{
				if (SessionData.Instance.dof != null)
				{
					this.saveDof = SessionData.Instance.dof.active;
					SessionData.Instance.dof.active = false;
				}
				this.feedbackForm.Show();
			}
			catch
			{
				Game.LogError("Error detected while opening feedback form", 2);
			}
		}
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x0019A424 File Offset: 0x00198624
	public void OnCloseBugReport()
	{
		try
		{
			if (SessionData.Instance.dof != null)
			{
				SessionData.Instance.dof.active = this.saveDof;
			}
		}
		catch
		{
			Game.LogError("Error detected while closing feedback form", 2);
		}
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x0019A478 File Offset: 0x00198678
	public void SumbitBugReport()
	{
		Game.Log("Menu: Submitting bug report (new)...", 2);
		if (Game.Instance.enableBugReporting)
		{
			if (this.bugReportTimer <= 0f)
			{
				this.bugReportTimer = 15f;
				try
				{
					this.ffCategory.value = this.categoryDropdown.dropdown.value;
					this.ffPriority.value = this.priorityDropdown.dropdown.value;
					this.ffNameInput.text = this.bugNameInput.text.text;
					this.ffDescriptionInput.text = this.bugDetailsInput.text.text;
					if (this.sendScreenshotToggle.isOn)
					{
						try
						{
							RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
							CameraController.Instance.cam.targetTexture = renderTexture;
							RenderTexture.active = renderTexture;
							CameraController.Instance.cam.Render();
							Texture2D texture2D = new Texture2D(Screen.width, Screen.height);
							texture2D.ReadPixels(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), 0, 0);
							RenderTexture.active = null;
							if ((texture2D.width ^ 2 * texture2D.height ^ 2) > 4082)
							{
								float num = (float)(1920 / Mathf.Max(texture2D.width, texture2D.height));
								int num2 = Mathf.RoundToInt((float)texture2D.width * num);
								int num3 = Mathf.RoundToInt((float)texture2D.height * num);
								texture2D.filterMode = 2;
								texture2D.Apply(false);
								Graphics.SetRenderTarget(new RenderTexture(num2, num3, 32));
								GL.LoadPixelMatrix(0f, 1f, 1f, 0f);
								GL.Clear(true, true, default(Color));
								Graphics.DrawTexture(new Rect(0f, 0f, 1f, 1f), texture2D);
								texture2D.Reinitialize(num2, num3);
								texture2D.ReadPixels(new Rect(0f, 0f, (float)num2, (float)num3), 0, 0, false);
								texture2D.Apply(false);
							}
							this.feedbackForm.CurrentReport.AttachFile("screenshot.png", ImageConversion.EncodeToPNG(texture2D));
							renderTexture.Release();
							CameraController.Instance.cam.targetTexture = null;
						}
						catch
						{
							Game.LogError("Unable to capture screenshot for bug report", 2);
						}
					}
					this.ffPrevLogCollector.collectionEnabled = this.sendPrevLogToggle.isOn;
					this.ffSystemInfo.collectionEnabled = this.sendSystemSpecsToggle.isOn;
					if (this.bugSaveDropdown.dropdown.value > 0 && Directory.Exists(Application.persistentDataPath + "/Save"))
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/Save");
						List<FileInfo> list = Enumerable.ToList<FileInfo>(directoryInfo.GetFiles("*.sod", 1));
						list.AddRange(Enumerable.ToList<FileInfo>(directoryInfo.GetFiles("*.sodb", 1)));
						FileInfo fileInfo = list.Find((FileInfo item) => item.Name == this.bugSaveDropdown.staticOptionReference[this.bugSaveDropdown.dropdown.value]);
						if (fileInfo != null)
						{
							if (fileInfo.Length <= 200000000L)
							{
								if (fileInfo.Extension.ToLower() == ".sodb")
								{
									Game.Log(string.Concat(new string[]
									{
										"Menu: Save to attached is already in a compressed format: ",
										fileInfo.FullName,
										" the size is ",
										((float)fileInfo.Length / 1000000f).ToString(),
										"mb"
									}), 2);
									try
									{
										string text = Strings.FilterInputtedText(this.bugNameInput.text.text, true, 100);
										this.feedbackForm.CurrentReport.AttachFile(text + ".sodb", fileInfo.FullName);
										goto IL_4EA;
									}
									catch
									{
										Game.LogError("Unable to read compressed save at: " + fileInfo.FullName, 2);
										goto IL_4EA;
									}
								}
								Game.Log(string.Concat(new string[]
								{
									"Menu: Found save file to attach: ",
									fileInfo.FullName,
									" the size is ",
									((float)fileInfo.Length / 1000000f).ToString(),
									"mb"
								}), 2);
								string text2 = fileInfo.Directory.FullName + "\\BugReportCompressedSave.temp";
								ulong[] proc = new ulong[1];
								Game.Log("Menu: Compressing file...", 2);
								brotli.compressFile(fileInfo.FullName, text2, proc, 9, 19, 0, 0);
								if (File.Exists(text2))
								{
									try
									{
										string text3 = Strings.FilterInputtedText(this.bugNameInput.text.text, true, 100);
										this.feedbackForm.CurrentReport.AttachFile(text3 + ".sodb", text2);
										goto IL_4EA;
									}
									catch
									{
										Game.LogError("Unable to read compressed save at: " + text2, 2);
										goto IL_4EA;
									}
								}
								Game.LogError("Compressed save does not exist here: " + text2, 2);
							}
							else
							{
								Game.LogError("The save file is too large to submit!", 2);
							}
						}
						else
						{
							Game.LogError("Unable to find save file!", 2);
						}
					}
					IL_4EA:
					this.feedbackForm.Submit();
					PopupMessageController.Instance.PopupMessage("bugSubmit", true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
					this.ResetBugReportDetails();
					this.SetMenuComponent(this.previousComponent);
					return;
				}
				catch
				{
					Game.LogError("Unable to submit bug report!", 2);
					return;
				}
			}
			Game.Log("Menu: Please wait for bug report timer: " + this.bugReportTimer.ToString(), 2);
			PopupMessageController.Instance.PopupMessage("bugSubmitDelay", true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
		}
	}

	// Token: 0x06001CC5 RID: 7365 RVA: 0x0019AAA0 File Offset: 0x00198CA0
	public void RefreshSaveGameDropdown()
	{
		if (this.bugSaveDropdown != null)
		{
			List<string> list = new List<string>();
			list.Add("-");
			if (Directory.Exists(Application.persistentDataPath + "/Save"))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/Save");
				List<FileInfo> list2 = Enumerable.ToList<FileInfo>(directoryInfo.GetFiles("*.sod", 1));
				list2.AddRange(Enumerable.ToList<FileInfo>(directoryInfo.GetFiles("*.sodb", 1)));
				list2.Sort((FileInfo p2, FileInfo p1) => p1.LastWriteTime.CompareTo(p2.LastWriteTime));
				foreach (FileInfo fileInfo in list2)
				{
					list.Add(fileInfo.Name);
				}
			}
			this.bugSaveDropdown.AddOptions(list, false, null);
			this.bugSaveDropdown.SelectFromStaticOption("-");
		}
	}

	// Token: 0x06001CC6 RID: 7366 RVA: 0x0019ABB0 File Offset: 0x00198DB0
	public void ResetBugReportDetails()
	{
		this.bugNameInput.text.text = Strings.Get("ui.interface", "Bug Name", Strings.Casing.asIs, false, false, false, null);
		this.bugDetailsInput.text.text = Strings.Get("ui.interface", "Bug Details", Strings.Casing.asIs, false, false, false, null);
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x0019AC08 File Offset: 0x00198E08
	public void OnChangeBugNameButton()
	{
		PopupMessageController.Instance.PopupMessage("bugName", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.inputField.text = this.bugNameInput.text.text;
		PopupMessageController.Instance.OnLeftButton += this.OnChangeBugNameCancel;
		PopupMessageController.Instance.OnRightButton += this.OnChangeBugNameConfirm;
	}

	// Token: 0x06001CC8 RID: 7368 RVA: 0x0019ACA2 File Offset: 0x00198EA2
	public void OnChangeBugNameCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeBugNameCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeBugNameConfirm;
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x0019ACD0 File Offset: 0x00198ED0
	public void OnChangeBugNameConfirm()
	{
		this.bugNameInput.text.text = PopupMessageController.Instance.inputField.text;
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeBugNameCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeBugNameConfirm;
	}

	// Token: 0x06001CCA RID: 7370 RVA: 0x0019AD28 File Offset: 0x00198F28
	public void OnChangeBugDetailsButton()
	{
		PopupMessageController.Instance.PopupMessage("bugDetails", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.inputField.text = this.bugDetailsInput.text.text;
		PopupMessageController.Instance.OnLeftButton += this.OnChangeBugDetailsCancel;
		PopupMessageController.Instance.OnRightButton += this.OnChangeBugDetailsConfirm;
	}

	// Token: 0x06001CCB RID: 7371 RVA: 0x0019ADC2 File Offset: 0x00198FC2
	public void OnChangeBugDetailsCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeBugDetailsCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeBugDetailsConfirm;
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x0019ADF0 File Offset: 0x00198FF0
	public void OnChangeBugDetailsConfirm()
	{
		this.bugDetailsInput.text.text = PopupMessageController.Instance.inputField.text;
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeBugDetailsCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeBugDetailsConfirm;
	}

	// Token: 0x06001CCD RID: 7373 RVA: 0x0019AE48 File Offset: 0x00199048
	public void PlayButtonClick()
	{
		AudioController.Instance.Play2DSound(AudioControls.Instance.mainButton, null, 1f);
	}

	// Token: 0x06001CCE RID: 7374 RVA: 0x0019AE65 File Offset: 0x00199065
	public void PlayForwardButtonClick()
	{
		AudioController.Instance.Play2DSound(AudioControls.Instance.mainButtonForward, null, 1f);
	}

	// Token: 0x06001CCF RID: 7375 RVA: 0x0019AE82 File Offset: 0x00199082
	public void PlayBackButtonClick()
	{
		AudioController.Instance.Play2DSound(AudioControls.Instance.mainButtonBack, null, 1f);
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x0019AE9F File Offset: 0x0019909F
	public void PlayTickbox()
	{
		AudioController.Instance.Play2DSound(AudioControls.Instance.tickbox, null, 1f);
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x0019AEBC File Offset: 0x001990BC
	public void OnLanguageChange()
	{
		PopupMessageController.Instance.PopupMessage("lang_change", true, true, "Cancel", "Restart", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnLeftButton += this.LangCancelRestartGame;
		PopupMessageController.Instance.OnRightButton += this.LangRestartGame;
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x0019AF38 File Offset: 0x00199138
	private void LangRestartGame()
	{
		PopupMessageController.Instance.OnLeftButton -= this.LangCancelRestartGame;
		PopupMessageController.Instance.OnRightButton -= this.LangRestartGame;
		Game.Log("CityGen: Restarting game due to language change...", 2);
		AudioController.Instance.StopAllSounds();
		SceneManager.LoadScene("Main");
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x0019AF90 File Offset: 0x00199190
	private void LangCancelRestartGame()
	{
		PopupMessageController.Instance.OnLeftButton -= this.LangCancelRestartGame;
		PopupMessageController.Instance.OnRightButton -= this.LangRestartGame;
		Game.Log("CityGen: Reverting to loaded language: " + this.loadedLanguage, 2);
		this.languageDropdown.SelectFromStaticOption(this.loadedLanguage);
		PlayerPrefsController.Instance.OnToggleChanged("language", true, null);
	}

	// Token: 0x06001CD4 RID: 7380 RVA: 0x0019B004 File Offset: 0x00199204
	public void OnChangeResolution()
	{
		List<Resolution> list = Enumerable.ToList<Resolution>(Screen.resolutions);
		int width = Screen.width;
		int height = Screen.height;
		int refreshRate = Screen.currentResolution.refreshRate;
		int num = list.FindIndex((Resolution item) => item.ToString() == this.resolutionsDropdown.staticOptionReference[this.resolutionsDropdown.dropdown.value]);
		if (num > -1)
		{
			width = list[num].width;
			height = list[num].height;
			refreshRate = list[num].refreshRate;
		}
		FullScreenMode fullScreenMode = Screen.fullScreenMode;
		fullScreenMode = this.fullScreenModeDropdown.dropdown.value;
		Screen.SetResolution(width, height, fullScreenMode, refreshRate);
		PlayerPrefs.SetInt("width", width);
		PlayerPrefs.SetInt("height", height);
		PlayerPrefs.SetInt("refresh", refreshRate);
		PlayerPrefs.SetInt("fullscreen", fullScreenMode);
	}

	// Token: 0x06001CD5 RID: 7381 RVA: 0x0019B0D6 File Offset: 0x001992D6
	public void CopyShareCodeToClipboard()
	{
		if (this.selectedCityInfoData != null)
		{
			GUIUtility.systemCopyBuffer = this.selectedCityInfoData.shareCode;
			Game.Log("Menu: Coppied share code to clipboard: " + GUIUtility.systemCopyBuffer, 2);
		}
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x0019B105 File Offset: 0x00199305
	public void NewGameTypeButton(bool sandbox)
	{
		if (Game.Instance.disableSandbox && sandbox)
		{
			return;
		}
		RestartSafeController.Instance.sandbox = sandbox;
		this.SetMenuComponent(MainMenuController.Component.city);
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x0019B128 File Offset: 0x00199328
	public void PreviousMenu()
	{
		this.SetMenuComponent(this.previousComponent);
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x0019B138 File Offset: 0x00199338
	public void LoadTip()
	{
		if (Toolbox.Instance.allDDSTrees.ContainsKey(this.loadingTipsDDSTree))
		{
			DDSSaveClasses.DDSTreeSave ddstreeSave = Toolbox.Instance.allDDSTrees[this.loadingTipsDDSTree];
			DDSSaveClasses.DDSMessageSettings ddsmessageSettings = ddstreeSave.messages[Toolbox.Instance.Rand(0, ddstreeSave.messages.Count, true)];
			this.tipText.text = Strings.GetTextForComponent(ddsmessageSettings.msgID, null, null, null, "\n", false, null, Strings.LinkSetting.forceNoLinks, null);
			MainMenuController.LoadingTip loadingTip = this.loadingTips[Toolbox.Instance.Rand(0, MainMenuController.Instance.loadingTips.Count, true)];
			this.tipImg.sprite = loadingTip.image;
			this.tipImg.color = Color.white;
			this.nextTipTimer = 8f;
		}
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x0019B20D File Offset: 0x0019940D
	public void ShadowsWebsiteLink()
	{
		Application.OpenURL("http://shadows.game");
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x0019B219 File Offset: 0x00199419
	public void OnEffectStatusChange()
	{
		this.SetStatusEffectOptionsAccordingToDropdown();
	}

	// Token: 0x06001CDB RID: 7387 RVA: 0x0019B224 File Offset: 0x00199424
	public void SetStatusEffectOptionsAccordingToDropdown()
	{
		if (this.statusEffectsDropdown.dropdown.value == 0)
		{
			using (List<ToggleController>.Enumerator enumerator = this.statusEffectToggles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ToggleController toggleController = enumerator.Current;
					toggleController.SetOn();
				}
				return;
			}
		}
		if (this.statusEffectsDropdown.dropdown.value == 1)
		{
			using (List<ToggleController>.Enumerator enumerator = this.statusEffectToggles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ToggleController toggleController2 = enumerator.Current;
					if (toggleController2.playerPrefsID == "energizedStatus" || toggleController2.playerPrefsID == "hydratedStatus" || toggleController2.playerPrefsID == "focusedStatus" || toggleController2.playerPrefsID == "wellRestedStatus")
					{
						toggleController2.SetOn();
					}
					else
					{
						toggleController2.SetOff();
					}
				}
				return;
			}
		}
		if (this.statusEffectsDropdown.dropdown.value == 2)
		{
			using (List<ToggleController>.Enumerator enumerator = this.statusEffectToggles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ToggleController toggleController3 = enumerator.Current;
					if (toggleController3.playerPrefsID == "energizedStatus" || toggleController3.playerPrefsID == "hydratedStatus" || toggleController3.playerPrefsID == "focusedStatus" || toggleController3.playerPrefsID == "wellRestedStatus")
					{
						toggleController3.SetOff();
					}
					else
					{
						toggleController3.SetOn();
					}
				}
				return;
			}
		}
		if (this.statusEffectsDropdown.dropdown.value == 3)
		{
			foreach (ToggleController toggleController4 in this.statusEffectToggles)
			{
				toggleController4.SetOff();
			}
		}
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x0019B42C File Offset: 0x0019962C
	public void SetDropdownAccordingToStatusEffects()
	{
		bool flag = true;
		bool flag2 = true;
		using (List<ToggleController>.Enumerator enumerator = this.statusEffectToggles.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isOn)
				{
					if (flag2)
					{
						flag2 = false;
					}
				}
				else if (flag)
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			this.statusEffectsDropdown.dropdown.SetValueWithoutNotify(0);
			return;
		}
		if (flag2)
		{
			this.statusEffectsDropdown.dropdown.SetValueWithoutNotify(3);
			return;
		}
		this.statusEffectsDropdown.dropdown.SetValueWithoutNotify(4);
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x0019B4CC File Offset: 0x001996CC
	public void ResetControls()
	{
		SimpleControlRemappingSOD.Instance.ResetControls();
		this.SetMenuComponent(MainMenuController.Component.settings);
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x0019B4DF File Offset: 0x001996DF
	public void OnOpenModMenu()
	{
		ModController.Instance.UpdateModEntries();
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x0019B4EC File Offset: 0x001996EC
	public void OnContinueCityGeneration()
	{
		Game.Log("Menu: On continue city generation; checking for possible city file overwrite...", 2);
		FileInfo cityFile = Toolbox.Instance.GetCityFile(RestartSafeController.Instance.cityName, RestartSafeController.Instance.seed, RestartSafeController.Instance.cityX, RestartSafeController.Instance.cityY, Game.Instance.buildID);
		if (cityFile == null)
		{
			Game.Log("Menu: ... No overwrite detected, continuing...", 2);
			this.SetMenuComponent(7);
			this.NewCharacter();
			return;
		}
		Game.Log("Menu: ... Found an existing city at: " + cityFile.FullName, 2);
		PopupMessageController.Instance.PopupMessage("CityOverwriteWarning", true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
	}

	// Token: 0x040025E2 RID: 9698
	[Header("Background")]
	public RectTransform mainMenuContainer;

	// Token: 0x040025E3 RID: 9699
	public Image backgroundImage;

	// Token: 0x040025E4 RID: 9700
	public Image logoImage;

	// Token: 0x040025E5 RID: 9701
	public TextMeshProUGUI buildText;

	// Token: 0x040025E6 RID: 9702
	public GameObject buildNameObject;

	// Token: 0x040025E7 RID: 9703
	public float time;

	// Token: 0x040025E8 RID: 9704
	[Header("Components")]
	public MainMenuController.Component previousComponent;

	// Token: 0x040025E9 RID: 9705
	public MainMenuController.MenuComponent currentComponent;

	// Token: 0x040025EA RID: 9706
	public float componentMotion;

	// Token: 0x040025EB RID: 9707
	public FeedbackForm feedbackForm;

	// Token: 0x040025EC RID: 9708
	public FormField feedbackPlayerInfo;

	// Token: 0x040025ED RID: 9709
	public bool saveDof;

	// Token: 0x040025EE RID: 9710
	public TextMeshProUGUI betaMessageText;

	// Token: 0x040025EF RID: 9711
	public GraphicRaycaster raycaster;

	// Token: 0x040025F0 RID: 9712
	public bool askedStreamerQuestion;

	// Token: 0x040025F1 RID: 9713
	public bool acceptedEULA;

	// Token: 0x040025F2 RID: 9714
	[ReorderableList]
	public List<MainMenuController.MenuComponent> components = new List<MainMenuController.MenuComponent>();

	// Token: 0x040025F3 RID: 9715
	[Header("Tips")]
	public string loadingTipsDDSTree;

	// Token: 0x040025F4 RID: 9716
	[ReorderableList]
	public List<MainMenuController.LoadingTip> loadingTips = new List<MainMenuController.LoadingTip>();

	// Token: 0x040025F5 RID: 9717
	public float nextTipTimer;

	// Token: 0x040025F6 RID: 9718
	[Header("Dropdowns")]
	public DropdownController languageDropdown;

	// Token: 0x040025F7 RID: 9719
	public DropdownController resolutionsDropdown;

	// Token: 0x040025F8 RID: 9720
	public DropdownController fullScreenModeDropdown;

	// Token: 0x040025F9 RID: 9721
	public DropdownController startTimeDropdown;

	// Token: 0x040025FA RID: 9722
	public DropdownController gameDifficultyDropdown;

	// Token: 0x040025FB RID: 9723
	public DropdownController gameDifficultyDropdown2;

	// Token: 0x040025FC RID: 9724
	public DropdownController gameLengthDropdown;

	// Token: 0x040025FD RID: 9725
	public DropdownController selectCityDropdown;

	// Token: 0x040025FE RID: 9726
	public DropdownController playerGenderDropdown;

	// Token: 0x040025FF RID: 9727
	public DropdownController partnerGenderDropdown;

	// Token: 0x04002600 RID: 9728
	public DropdownController citySizeDropdown;

	// Token: 0x04002601 RID: 9729
	public DropdownController cityPopDropdown;

	// Token: 0x04002602 RID: 9730
	public DropdownController statusEffectsDropdown;

	// Token: 0x04002603 RID: 9731
	public DropdownController aaModeDropdown;

	// Token: 0x04002604 RID: 9732
	public DropdownController aaQualityDropdown;

	// Token: 0x04002605 RID: 9733
	public DropdownController dlssModeDropdown;

	// Token: 0x04002606 RID: 9734
	public List<ToggleController> statusEffectToggles = new List<ToggleController>();

	// Token: 0x04002607 RID: 9735
	[Header("Main Menu")]
	public ButtonController saveGameButton;

	// Token: 0x04002608 RID: 9736
	public ButtonController loadGameButton;

	// Token: 0x04002609 RID: 9737
	public ButtonController sandboxGameButton;

	// Token: 0x0400260A RID: 9738
	public ButtonController cityGenButton;

	// Token: 0x0400260B RID: 9739
	public ButtonController resumeGameButton;

	// Token: 0x0400260C RID: 9740
	public ButtonController helpButton;

	// Token: 0x0400260D RID: 9741
	public ButtonController bugReportButton;

	// Token: 0x0400260E RID: 9742
	public ButtonController feedbackButton;

	// Token: 0x0400260F RID: 9743
	public ButtonController modsButton;

	// Token: 0x04002610 RID: 9744
	[Header("City Setup Menu")]
	public TextMeshProUGUI selectedCityShareCode;

	// Token: 0x04002611 RID: 9745
	public TextMeshProUGUI selectedCityDetailsText;

	// Token: 0x04002612 RID: 9746
	public ButtonController selectedCityContinueButton;

	// Token: 0x04002613 RID: 9747
	[NonSerialized]
	public CityInfoData selectedCityInfoData;

	// Token: 0x04002614 RID: 9748
	public ButtonController selectedCityCopyShareCodeButton;

	// Token: 0x04002615 RID: 9749
	public ButtonController deleteCityButton;

	// Token: 0x04002616 RID: 9750
	private List<FileInfo> cityMapFiles = new List<FileInfo>();

	// Token: 0x04002617 RID: 9751
	private List<FileInfo> cityInfoFiles = new List<FileInfo>();

	// Token: 0x04002618 RID: 9752
	private Dictionary<string, CityInfoData> cityInfoDict = new Dictionary<string, CityInfoData>();

	// Token: 0x04002619 RID: 9753
	[Header("Dev Controls")]
	public ButtonController developerOptionsButton;

	// Token: 0x0400261A RID: 9754
	public Slider windSlider;

	// Token: 0x0400261B RID: 9755
	public Slider rainSlider;

	// Token: 0x0400261C RID: 9756
	public Slider lightningSlider;

	// Token: 0x0400261D RID: 9757
	public Slider snowSlider;

	// Token: 0x0400261E RID: 9758
	public Slider fogSlider;

	// Token: 0x0400261F RID: 9759
	public Button setWeatherButton;

	// Token: 0x04002620 RID: 9760
	public ToggleController allowLicensedMusicToggle;

	// Token: 0x04002621 RID: 9761
	[Header("New Character")]
	public ButtonController playerNameButton;

	// Token: 0x04002622 RID: 9762
	public MultiSelectController playerSkinToneSelect;

	// Token: 0x04002623 RID: 9763
	[Header("City Generation")]
	public TextMeshProUGUI shareCodeText;

	// Token: 0x04002624 RID: 9764
	public ButtonController pasteShareCodeButton;

	// Token: 0x04002625 RID: 9765
	public ButtonController changeCityNameButton;

	// Token: 0x04002626 RID: 9766
	public TextMeshProUGUI generationWarningText;

	// Token: 0x04002627 RID: 9767
	[Header("Credits")]
	public TextMeshProUGUI creditsText;

	// Token: 0x04002628 RID: 9768
	public RectTransform creditsPageContent;

	// Token: 0x04002629 RID: 9769
	[Header("Main Menu")]
	public TextMeshProUGUI mouseOverText;

	// Token: 0x0400262A RID: 9770
	public bool mainMenuActive;

	// Token: 0x0400262B RID: 9771
	[Header("Language")]
	public string loadedLanguage;

	// Token: 0x0400262C RID: 9772
	[Header("Loading Bar")]
	public TextMeshProUGUI loadingText;

	// Token: 0x0400262D RID: 9773
	public Slider loadingSlider;

	// Token: 0x0400262E RID: 9774
	public TextMeshProUGUI tipText;

	// Token: 0x0400262F RID: 9775
	public Image tipImg;

	// Token: 0x04002630 RID: 9776
	[Header("Menu Fading")]
	public CanvasRenderer fadeOverlay;

	// Token: 0x04002631 RID: 9777
	public float desiredFade;

	// Token: 0x04002632 RID: 9778
	public float fade = 1f;

	// Token: 0x04002633 RID: 9779
	private bool exitMainMenuAfterFade;

	// Token: 0x04002634 RID: 9780
	[Header("Save/Load Game")]
	public RectTransform loadGameContentRect;

	// Token: 0x04002635 RID: 9781
	public RectTransform saveGameContentRect;

	// Token: 0x04002636 RID: 9782
	public GameObject saveGameEntryPrefab;

	// Token: 0x04002637 RID: 9783
	private List<SaveGameEntryController> spawnedSaveGames = new List<SaveGameEntryController>();

	// Token: 0x04002638 RID: 9784
	private List<SaveGameEntryController> spawnedLoadGames = new List<SaveGameEntryController>();

	// Token: 0x04002639 RID: 9785
	public SaveGameEntryController selectedSave;

	// Token: 0x0400263A RID: 9786
	public ButtonController saveButton;

	// Token: 0x0400263B RID: 9787
	public ButtonController loadButton;

	// Token: 0x0400263C RID: 9788
	public ButtonController deleteButton;

	// Token: 0x0400263D RID: 9789
	public ButtonController deleteButton2;

	// Token: 0x0400263E RID: 9790
	public SaveGameEntryController newSaveGameEntry;

	// Token: 0x0400263F RID: 9791
	[Header("Bug Report (New)")]
	public DropdownController bugSaveDropdown;

	// Token: 0x04002640 RID: 9792
	public DropdownController priorityDropdown;

	// Token: 0x04002641 RID: 9793
	public DropdownController categoryDropdown;

	// Token: 0x04002642 RID: 9794
	public ButtonController bugNameInput;

	// Token: 0x04002643 RID: 9795
	public ButtonController bugDetailsInput;

	// Token: 0x04002644 RID: 9796
	public ToggleController sendScreenshotToggle;

	// Token: 0x04002645 RID: 9797
	public ToggleController sendSystemSpecsToggle;

	// Token: 0x04002646 RID: 9798
	public ToggleController sendPrevLogToggle;

	// Token: 0x04002647 RID: 9799
	public float bugReportTimer;

	// Token: 0x04002648 RID: 9800
	[Space(7f)]
	public TMP_Dropdown ffPriority;

	// Token: 0x04002649 RID: 9801
	public TMP_Dropdown ffCategory;

	// Token: 0x0400264A RID: 9802
	public TMP_InputField ffNameInput;

	// Token: 0x0400264B RID: 9803
	public TMP_InputField ffDescriptionInput;

	// Token: 0x0400264C RID: 9804
	public FormField ffSystemInfo;

	// Token: 0x0400264D RID: 9805
	public FormElement ffPrevLogCollector;

	// Token: 0x0400264E RID: 9806
	[Header("Special Cases")]
	public List<DropdownController> disableWithDynamicResolution = new List<DropdownController>();

	// Token: 0x0400264F RID: 9807
	public List<DropdownController> enableWithDynamicResolution = new List<DropdownController>();

	// Token: 0x04002650 RID: 9808
	[Header("Animation")]
	public CanvasRenderer topBarRend;

	// Token: 0x04002651 RID: 9809
	public CanvasRenderer bottomBarRend;

	// Token: 0x04002652 RID: 9810
	public TextMeshProUGUI titleText;

	// Token: 0x04002653 RID: 9811
	public AnimationCurve titleTextKerningAnimation;

	// Token: 0x04002654 RID: 9812
	private static MainMenuController _instance;

	// Token: 0x02000524 RID: 1316
	[Serializable]
	public class MenuComponent
	{
		// Token: 0x04002655 RID: 9813
		public MainMenuController.Component component;

		// Token: 0x04002656 RID: 9814
		public RectTransform rect;

		// Token: 0x04002657 RID: 9815
		public int xPhase;

		// Token: 0x04002658 RID: 9816
		public Vector2 onscreenAnchoredPosition;

		// Token: 0x04002659 RID: 9817
		public List<ButtonController> buttons = new List<ButtonController>();

		// Token: 0x0400265A RID: 9818
		public ButtonController previouslySelected;

		// Token: 0x0400265B RID: 9819
		public bool skipMotion;
	}

	// Token: 0x02000525 RID: 1317
	public enum Component
	{
		// Token: 0x0400265D RID: 9821
		none,
		// Token: 0x0400265E RID: 9822
		mainMenuButtons,
		// Token: 0x0400265F RID: 9823
		settings,
		// Token: 0x04002660 RID: 9824
		newGameSelect,
		// Token: 0x04002661 RID: 9825
		city,
		// Token: 0x04002662 RID: 9826
		citySelect,
		// Token: 0x04002663 RID: 9827
		generateCity,
		// Token: 0x04002664 RID: 9828
		charSetup,
		// Token: 0x04002665 RID: 9829
		interfaceSettings,
		// Token: 0x04002666 RID: 9830
		graphicsSettings,
		// Token: 0x04002667 RID: 9831
		audioSettings,
		// Token: 0x04002668 RID: 9832
		gameplaySettings,
		// Token: 0x04002669 RID: 9833
		controlSettings,
		// Token: 0x0400266A RID: 9834
		devSettings,
		// Token: 0x0400266B RID: 9835
		saveGame,
		// Token: 0x0400266C RID: 9836
		loadGame,
		// Token: 0x0400266D RID: 9837
		credits,
		// Token: 0x0400266E RID: 9838
		loadingCity,
		// Token: 0x0400266F RID: 9839
		splash,
		// Token: 0x04002670 RID: 9840
		controlDetect,
		// Token: 0x04002671 RID: 9841
		streamingSettings,
		// Token: 0x04002672 RID: 9842
		bugReport,
		// Token: 0x04002673 RID: 9843
		mods
	}

	// Token: 0x02000526 RID: 1318
	[Serializable]
	public class LoadingTip
	{
		// Token: 0x04002674 RID: 9844
		public string dictRef;

		// Token: 0x04002675 RID: 9845
		public Sprite image;
	}
}
