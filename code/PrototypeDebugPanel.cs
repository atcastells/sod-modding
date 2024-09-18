using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020001BF RID: 447
public class PrototypeDebugPanel : MonoBehaviour
{
	// Token: 0x06000AE3 RID: 2787 RVA: 0x000A44FC File Offset: 0x000A26FC
	private void OnEnable()
	{
		this.cityNameInputButton.text.text = RestartSafeController.Instance.cityName;
		if (this.citySizeDropdownController != null)
		{
			List<string> list = new List<string>();
			list.Add("small");
			if (!Game.Instance.smallCitiesOnly)
			{
				list.Add("medium");
				list.Add("large");
				list.Add("veryLarge");
			}
			this.citySizeDropdownController.AddOptions(list, true, null);
			int num = CityControls.Instance.citySizes.FindIndex((CityControls.CitySize item) => item.v2.x == (float)RestartSafeController.Instance.cityX && item.v2.y == (float)RestartSafeController.Instance.cityY);
			if (num < 0)
			{
				this.citySizeDropdownController.SelectFromStaticOption(PlayerPrefsController.Instance.GetSettingStr("cityGenSize"));
			}
			else
			{
				this.citySizeDropdownController.dropdown.SetValueWithoutNotify(num);
			}
		}
		this.seedText.text = RestartSafeController.Instance.seed;
		this.loadingText.text = string.Empty;
		this.loadingSlider.value = 0f;
		this.OnNewTileSelected(null);
		this.tileEditModeButton.SetInteractable(false);
		this.streetsEditModeButton.SetInteractable(false);
		this.onContinueButton.SetInteractable(false);
		this.tileEditButtons.gameObject.SetActive(false);
		this.streetEditButtons.gameObject.SetActive(false);
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x000A4664 File Offset: 0x000A2864
	public void ResetControllerSelection(bool deselectCurrent = false)
	{
		if (deselectCurrent && InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		ButtonController lastSelected = this.cityNameInputButton;
		if (HighlanderSingleton<CityEditorController>.Instance != null)
		{
			lastSelected = HighlanderSingleton<CityEditorController>.Instance.GetLastSelected();
		}
		if (lastSelected != null)
		{
			lastSelected.OnSelect();
			return;
		}
		this.cityNameInputButton.OnSelect();
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x00002265 File Offset: 0x00000465
	private void OnDisable()
	{
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x000A46D0 File Offset: 0x000A28D0
	private void Update()
	{
		if (CityConstructor.Instance.loadingOperationActive)
		{
			this.loadingSlider.value = CityConstructor.Instance.loadingProgress;
			this.loadingText.text = MainMenuController.Instance.loadingText.text;
		}
		if (!InputController.Instance.mouseInputMode && InterfaceController.Instance.selectedElement == null)
		{
			this.ResetControllerSelection(false);
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x000A4740 File Offset: 0x000A2940
	public void ShowStreetsHack(bool condition)
	{
		foreach (NewRoom newRoom in CityData.Instance.roomDirectory)
		{
			newRoom.gameObject.SetActive(condition);
		}
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x000A479C File Offset: 0x000A299C
	public void ShrinkBuildingsHack(Vector3 vec)
	{
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			newBuilding.transform.localScale = vec;
		}
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x000A47F8 File Offset: 0x000A29F8
	public void OnChangeCityNameButton()
	{
		PopupMessageController.Instance.PopupMessage("cityName", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.inputField.text = RestartSafeController.Instance.cityName;
		PopupMessageController.Instance.OnLeftButton += this.OnChangeCityNamePopupCancel;
		PopupMessageController.Instance.OnRightButton += this.OnChangeCityNamePopupConfirm;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x000A488C File Offset: 0x000A2A8C
	private void OnChangeCityNamePopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeCityNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeCityNamePopupConfirm;
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x000A48BC File Offset: 0x000A2ABC
	private void OnChangeCityNamePopupConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeCityNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeCityNamePopupConfirm;
		RestartSafeController.Instance.cityName = Strings.FilterInputtedText(PopupMessageController.Instance.inputField.text, true, 100);
		this.OnChangeCityGenerationOption();
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x000A491C File Offset: 0x000A2B1C
	public void OnGenerateNewSeed()
	{
		RestartSafeController.Instance.seed = Toolbox.Instance.GenerateSeed(16, false, "");
		this.OnChangeCityGenerationOption();
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x000A4940 File Offset: 0x000A2B40
	public void OnGenerateBuildingsButton()
	{
		if (PrefabControls.Instance.cityContainer.transform.childCount <= 0)
		{
			HighlanderSingleton<CityEditorController>.Instance.GenerateNewCityEditorData();
			this.generateBuildingsButton.useAutomaticText = false;
			this.generateBuildingsButton.text.text = Strings.Get("ui.interface", "Clear Map", Strings.Casing.asIs, false, false, false, null);
			return;
		}
		HighlanderSingleton<CityEditorController>.Instance.ClearCurrentCityEditorData();
		this.generateBuildingsButton.useAutomaticText = true;
		this.generateBuildingsButton.UpdateButtonText();
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x000A49C0 File Offset: 0x000A2BC0
	public void OnChangeCityGenerationOption()
	{
		CityData.Instance.cityName = RestartSafeController.Instance.cityName;
		this.cityNameInputButton.text.text = RestartSafeController.Instance.cityName;
		RestartSafeController.Instance.cityX = Mathf.RoundToInt(CityControls.Instance.citySizes[this.citySizeDropdownController.dropdown.value].v2.x);
		RestartSafeController.Instance.cityY = Mathf.RoundToInt(CityControls.Instance.citySizes[this.citySizeDropdownController.dropdown.value].v2.y);
		if (Game.Instance.smallCitiesOnly)
		{
			RestartSafeController.Instance.cityX = Mathf.RoundToInt(CityControls.Instance.citySizes[0].v2.x);
			RestartSafeController.Instance.cityY = Mathf.RoundToInt(CityControls.Instance.citySizes[0].v2.y);
		}
		this.seedText.text = RestartSafeController.Instance.seed;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x000A4ADE File Offset: 0x000A2CDE
	public void OnChangeEditModeButton(int newEditMode)
	{
		this.OnChangeEditModeButton((CityEditorController.CityEditorMode)newEditMode);
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x000A4AE8 File Offset: 0x000A2CE8
	public void OnChangeEditModeButton(CityEditorController.CityEditorMode newEditMode)
	{
		HighlanderSingleton<CityEditorController>.Instance.SwitchEditorMode(newEditMode);
		if (newEditMode == CityEditorController.CityEditorMode.Default)
		{
			this.ShowStreetsHack(true);
			this.ShrinkBuildingsHack(Vector3.one);
			this.tileEditButtons.gameObject.SetActive(false);
			this.streetEditButtons.gameObject.SetActive(false);
			return;
		}
		if (newEditMode == CityEditorController.CityEditorMode.EditBuildings)
		{
			this.ShowStreetsHack(true);
			this.ShrinkBuildingsHack(Vector3.one);
			this.tileEditButtons.gameObject.SetActive(true);
			this.streetEditButtons.gameObject.SetActive(false);
			return;
		}
		if (newEditMode == CityEditorController.CityEditorMode.EditStreets)
		{
			this.ShowStreetsHack(true);
			this.ShrinkBuildingsHack(new Vector3(1f, 0.1f, 1f));
			this.tileEditButtons.gameObject.SetActive(false);
			this.streetEditButtons.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x000A4BB8 File Offset: 0x000A2DB8
	public void OnSwapBuildingButton()
	{
		HighlanderSingleton<CityEditorController>.Instance.SwitchEditorSubMode(CityEditorController.CityEditorSubMode.MoveSelection);
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x000A4BC8 File Offset: 0x000A2DC8
	public void OnContinueButton()
	{
		if (!HighlanderSingleton<CityEditorController>.Instance.DoesCurrentMapMeetCityRequirements(true))
		{
			return;
		}
		HighlanderSingleton<CityEditorController>.Instance.SwitchEditorMode(CityEditorController.CityEditorMode.Default);
		this.onContinueButton.SetInteractable(false);
		this.backButton.SetInteractable(false);
		this.generateBuildingsButton.SetInteractable(false);
		if (HighlanderSingleton<CityEditorController>.Instance.needsUpdatedPathfinding)
		{
			HighlanderSingleton<CityEditorController>.Instance.RerunPathfinder();
		}
		HighlanderSingleton<CityEditorController>.Instance.FinishLoading();
		InterfaceController.Instance.UpdateDOF();
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x000A4C3C File Offset: 0x000A2E3C
	public void OnBackButton()
	{
		RestartSafeController.Instance.loadFromDirty = false;
		AudioController.Instance.StopAllSounds();
		InputController.Instance.SetCursorLock(false);
		InputController.Instance.SetCursorVisible(true);
		SceneManager.LoadScene("Main");
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00002265 File Offset: 0x00000465
	public void OnNewTileSelected(CityTile newSelection)
	{
	}

	// Token: 0x04000B75 RID: 2933
	[Header("Components")]
	public ButtonController cityNameInputButton;

	// Token: 0x04000B76 RID: 2934
	public DropdownController citySizeDropdownController;

	// Token: 0x04000B77 RID: 2935
	public TextMeshProUGUI seedText;

	// Token: 0x04000B78 RID: 2936
	public ButtonController generateBuildingsButton;

	// Token: 0x04000B79 RID: 2937
	[Space(7f)]
	public ButtonController tileEditModeButton;

	// Token: 0x04000B7A RID: 2938
	public ButtonController streetsEditModeButton;

	// Token: 0x04000B7B RID: 2939
	public RectTransform tileEditButtons;

	// Token: 0x04000B7C RID: 2940
	public RectTransform streetEditButtons;

	// Token: 0x04000B7D RID: 2941
	[Space(7f)]
	public ButtonController buildingNameButton;

	// Token: 0x04000B7E RID: 2942
	public ButtonController buildingSwapButton;

	// Token: 0x04000B7F RID: 2943
	[Space(7f)]
	public Slider loadingSlider;

	// Token: 0x04000B80 RID: 2944
	public TextMeshProUGUI loadingText;

	// Token: 0x04000B81 RID: 2945
	public ButtonController onContinueButton;

	// Token: 0x04000B82 RID: 2946
	public ButtonController backButton;
}
