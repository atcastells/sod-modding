using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class CityEditorController : HighlanderSingleton<CityEditorController>
{
	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06000B39 RID: 2873 RVA: 0x000A7DC4 File Offset: 0x000A5FC4
	// (remove) Token: 0x06000B3A RID: 2874 RVA: 0x000A7DFC File Offset: 0x000A5FFC
	public event CityEditorController.NewCityEditorData OnNewCityEditorData;

	// Token: 0x06000B3B RID: 2875 RVA: 0x000A7E31 File Offset: 0x000A6031
	protected override void Awake()
	{
		this.GetComponentReferences();
		this._buildingEditor.OnNewTileSelection += this.OnNewTileSelected;
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x000A7E50 File Offset: 0x000A6050
	private void Start()
	{
		this.DeactivateEditors();
		this.InitializeSelectedModeComponents();
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x000A7E5E File Offset: 0x000A605E
	private void Update()
	{
		this.canFinishLoadFromCurrentState = (this.dataGenerated && !this.needsUpdatedPathfinding);
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x000A7E7A File Offset: 0x000A607A
	private void GetComponentReferences()
	{
		this._buildingEditor = base.GetComponent<CityEditorBuildingEdit>();
		this._streetEditor = base.GetComponent<CityEditorStreetEdit>();
		this._editCam = base.GetComponent<CityEditorInputController>();
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x000A7EA0 File Offset: 0x000A60A0
	public void RerunPathfinder()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: Rerun Pathfinder", 2);
		}
		PathFinder.Instance.DestroySelf();
		foreach (KeyValuePair<Vector2Int, CityTile> keyValuePair in HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles)
		{
			keyValuePair.Value.outsideTiles.Clear();
		}
		CityData.Instance.streetDirectory.Clear();
		CityData.Instance.roomDirectory.Clear();
		CityData.Instance.roomDictionary.Clear();
		CityData.Instance.gameLocationDirectory.Clear();
		Object.Instantiate<GameObject>(PrefabControls.Instance.pathfinderPrefab, PrefabControls.Instance.CityConstructorTargetTransform);
		PathFinder.Instance.SetDimensions();
		PathFinder.Instance.CompilePathFindingMap(false);
		this.needsUpdatedPathfinding = false;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x000A7F94 File Offset: 0x000A6194
	public void GenerateNewCityEditorData()
	{
		this.dataGenerated = true;
		CityConstructor.Instance.StopCityConstructionAtEndOfLoadState(CityConstructor.LoadState.generatePathfinding);
		CityConstructor.Instance.GenerateNewCity();
		float num = (float)(RestartSafeController.Instance.cityX - 2) * CityControls.Instance.cityTileSize.x + CityControls.Instance.cityTileSize.x / (float)CityControls.Instance.tileMultiplier * 2f;
		float num2 = (float)(RestartSafeController.Instance.cityY - 2) * CityControls.Instance.cityTileSize.y + CityControls.Instance.cityTileSize.y / (float)CityControls.Instance.tileMultiplier * 2f;
		this.cityEditFloor.transform.localScale = new Vector3(num, num2, 1f);
		this.cityEditFloor.SetActive(true);
		this.SwitchEditorMode(CityEditorController.CityEditorMode.Default);
		this.isLoading = false;
		this.canvasController.tileEditModeButton.SetInteractable(true);
		this.canvasController.streetsEditModeButton.SetInteractable(true);
		this.canvasController.onContinueButton.SetInteractable(true);
		foreach (StreetController streetController in CityData.Instance.streetDirectory)
		{
			streetController.UpdateName(false);
			streetController.playerEditedStreetName = streetController.name;
			streetController.isPlayerEditedName = true;
			streetController.UpdateName(false);
		}
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x000A8108 File Offset: 0x000A6308
	public void OnHaltOnEndOfLoadState(CityConstructor.LoadState haltedOnState)
	{
		if (haltedOnState == CityConstructor.LoadState.generatePathfinding)
		{
			foreach (StreetController streetController in CityData.Instance.streetDirectory)
			{
				streetController.UpdateName(false);
				streetController.playerEditedStreetName = streetController.name;
				streetController.isPlayerEditedName = true;
				streetController.UpdateName(false);
			}
			this.canvasController.ShowStreetsHack(true);
			if (this.OnNewCityEditorData != null)
			{
				this.OnNewCityEditorData();
			}
		}
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x000A819C File Offset: 0x000A639C
	public void ClearCurrentCityEditorData()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: Clear current city editor data", 2);
		}
		this.dataGenerated = false;
		foreach (object obj in PrefabControls.Instance.cityContainer.transform)
		{
			Object.Destroy(((Transform)obj).gameObject);
		}
		CitizenBehaviour.Instance.smokestacks.Clear();
		HighlanderSingleton<CityBoundaryAndTiles>.Instance.DestroySelf();
		HighlanderSingleton<CityDistricts>.Instance.DestroySelf();
		HighlanderSingleton<CityBlocks>.Instance.DestroySelf();
		HighlanderSingleton<CityDensity>.Instance.DestroySelf();
		HighlanderSingleton<CityBuildings>.Instance.DestroySelf();
		PathFinder.Instance.DestroySelf();
		CityData.Instance.DestroySelf();
		CityConstructor.Instance.DestroySelf();
		GameplayController.Instance.DestroySelf();
		GameObject[] cityConstructorPrefabs = PrefabControls.Instance.CityConstructorPrefabs;
		for (int i = 0; i < cityConstructorPrefabs.Length; i++)
		{
			Object.Instantiate<GameObject>(cityConstructorPrefabs[i], PrefabControls.Instance.CityConstructorTargetTransform);
		}
		this.cityEditFloor.SetActive(false);
		this.SwitchEditorMode(CityEditorController.CityEditorMode.Default);
		this.canvasController.tileEditModeButton.SetInteractable(false);
		this.canvasController.streetsEditModeButton.SetInteractable(false);
		this.canvasController.onContinueButton.SetInteractable(false);
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x000A82FC File Offset: 0x000A64FC
	public void FinishLoading()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: Finish Loading", 2);
		}
		this.canvasController.ShrinkBuildingsHack(Vector3.one);
		this.isLoading = true;
		CityConstructor.Instance.ClearCityConstructionHoldStatus();
		MainMenuController.Instance.mainMenuContainer.gameObject.SetActive(true);
		PrefabControls.Instance.menuCanvas.gameObject.SetActive(true);
		this.canvasController.gameObject.SetActive(false);
		MainMenuController.MenuComponent menuComponent = MainMenuController.Instance.components.Find((MainMenuController.MenuComponent item) => item.component == MainMenuController.Component.loadingCity);
		if (menuComponent != null)
		{
			menuComponent.rect.anchoredPosition = new Vector2(0f, menuComponent.rect.anchoredPosition.y);
			menuComponent.rect.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x000A83E4 File Offset: 0x000A65E4
	public void SetCityEditorWarning(string warning)
	{
		Debug.LogError(warning);
		if (Game.Instance.printDebug)
		{
			Game.Log(warning, 2);
		}
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x000A8400 File Offset: 0x000A6600
	public void SetCityEditor(bool condition)
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: Setting city editor acitve: " + condition.ToString(), 2);
		}
		this.canvasController.gameObject.SetActive(condition);
		base.gameObject.SetActive(condition);
		SessionData.Instance.isCityEdit = condition;
		this.cityEditorPostProcessingVolume.gameObject.SetActive(condition);
		foreach (GameObject gameObject in this.disableWhileActive)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(!condition);
			}
		}
		if (condition)
		{
			MainMenuController.Instance.EnableMainMenu(false, true, true, MainMenuController.Component.mainMenuButtons);
			InputController.Instance.SetCursorLock(false);
			InputController.Instance.SetCursorVisible(true);
			SessionData.Instance.SetWeather(0f, 0f, 0f, 0f, 0f, 0.1f, true);
		}
		else
		{
			if (InterfaceController.Instance.selectedElement != null)
			{
				InterfaceController.Instance.selectedElement.OnDeselect();
			}
			this.cityEditFloor.SetActive(false);
			foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
			{
				newBuilding.RemoveGroundFloorBuildingModel();
			}
			this.canvasController.ShowStreetsHack(true);
			this.canvasController.ShrinkBuildingsHack(Vector3.one);
		}
		InterfaceController.Instance.UpdateDOF();
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x000A85A8 File Offset: 0x000A67A8
	public void SwitchEditorMode(CityEditorController.CityEditorMode mode)
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: Switch editor mode: " + mode.ToString(), 2);
		}
		this.DeactivateEditors();
		this.currentMode = mode;
		this.InitializeSelectedModeComponents();
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x000A85E6 File Offset: 0x000A67E6
	public void SwitchEditorSubMode(CityEditorController.CityEditorSubMode submode)
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: Switch editor sub mode: " + submode.ToString(), 2);
		}
		this.currentSubMode = submode;
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x000A8618 File Offset: 0x000A6818
	private void DeactivateEditors()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: Deactivate editors", 2);
		}
		this._buildingEditor.enabled = false;
		this._streetEditor.enabled = false;
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x000A8649 File Offset: 0x000A6849
	public ButtonController GetLastSelected()
	{
		string text = "City Edit: Get last selected: ";
		ButtonController buttonController = this.previouslySelected;
		Game.Log(text + ((buttonController != null) ? buttonController.ToString() : null), 2);
		return this.previouslySelected;
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x000A8674 File Offset: 0x000A6874
	private void InitializeSelectedModeComponents()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: InitializeSelectedModeComponents", 2);
		}
		CityEditorController.CityEditorMode cityEditorMode = this.currentMode;
		if (cityEditorMode == CityEditorController.CityEditorMode.EditBuildings)
		{
			this._buildingEditor.enabled = true;
			this._editCam.tgtRot = new Vector3(0f, 0f, 0f);
			return;
		}
		if (cityEditorMode != CityEditorController.CityEditorMode.EditStreets)
		{
			return;
		}
		this._streetEditor.enabled = true;
		this._editCam.tgtRot = new Vector3(40f, 0f, 0f);
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x000A86FF File Offset: 0x000A68FF
	public void OnNewTileSelected(CityTile newSelection)
	{
		this.canvasController.OnNewTileSelected(newSelection);
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x000A8710 File Offset: 0x000A6910
	public bool DoesCurrentMapMeetCityRequirements(bool displayPopups)
	{
		if (!HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Exists((NewBuilding item) => item.preset != null && item.preset.presetName == "CityHall"))
		{
			if (displayPopups)
			{
				PopupMessageController.Instance.PopupMessage("Edit_NoCityHall", true, false, "Continue", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			}
			return false;
		}
		if (!HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Exists((NewBuilding item) => item.preset != null && item.preset.presetName == "AmericanDiner"))
		{
			if (displayPopups)
			{
				PopupMessageController.Instance.PopupMessage("Edit_NoDiner", true, false, "Continue", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			}
			return false;
		}
		int num = 0;
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			num += newBuilding.preset.GetResidenceCount();
		}
		if (num < 20)
		{
			if (displayPopups)
			{
				PopupMessageController.Instance.PopupMessage("Edit_NotEnoughResidences", true, false, "Continue", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			}
			return false;
		}
		int num2 = 0;
		foreach (NewBuilding newBuilding2 in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			if (newBuilding2.preset != null && newBuilding2.preset.floorLayouts.Count > 0)
			{
				for (int i = 0; i < newBuilding2.preset.floorLayouts.Count; i++)
				{
					BuildingPreset.InteriorFloorSetting interiorFloorSetting = newBuilding2.preset.floorLayouts[i];
					if (interiorFloorSetting.floorsWithThisSetting > 0)
					{
						int num3 = 99999;
						foreach (TextAsset textAsset in interiorFloorSetting.blueprints)
						{
							if (textAsset == null)
							{
								Game.LogError("CityGen: Missing floor preset for " + base.name + " floor " + 0.ToString(), 2);
							}
							else
							{
								FloorSaveData floorSaveData = null;
								CityData.Instance.floorData.TryGetValue(textAsset.name, ref floorSaveData);
								if (floorSaveData == null)
								{
									Game.LogError("CityGen: No floor data for " + textAsset.name, 2);
								}
								else
								{
									num3 = Mathf.Min(floorSaveData.a_d.FindAll((AddressSaveData item) => item.p_n == "Office" || item.p_n == "OfficeHighrise").Count, num3);
								}
							}
						}
						if (num3 < 99999)
						{
							num2 += num3;
						}
						if (num2 >= 1)
						{
							break;
						}
					}
					if (num2 >= 1)
					{
						break;
					}
				}
			}
			if (num2 >= 1)
			{
				break;
			}
		}
		if (num2 < 1)
		{
			if (displayPopups)
			{
				PopupMessageController.Instance.PopupMessage("Edit_NotEnoughOffices", true, false, "Continue", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			}
			return false;
		}
		return true;
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x000A8AE0 File Offset: 0x000A6CE0
	private void OnDisable()
	{
		this.canvasController.ShowStreetsHack(true);
		this.canvasController.ShrinkBuildingsHack(Vector3.one);
		this._buildingEditor.OnNewTileSelection -= this.OnNewTileSelected;
	}

	// Token: 0x04000BCC RID: 3020
	[Header("References")]
	public NewBuilding activeBuilding;

	// Token: 0x04000BCD RID: 3021
	public Camera cityEditorCam;

	// Token: 0x04000BCE RID: 3022
	public CityEditorController.CityEditorMode currentMode;

	// Token: 0x04000BCF RID: 3023
	public CityEditorController.CityEditorSubMode currentSubMode;

	// Token: 0x04000BD0 RID: 3024
	public PrototypeDebugPanel canvasController;

	// Token: 0x04000BD1 RID: 3025
	[Tooltip("The editor has it's own overrides for post processing; enable/disable this along with the editor itself")]
	public GameObject cityEditorPostProcessingVolume;

	// Token: 0x04000BD2 RID: 3026
	[Tooltip("Disable/Reenable these objects when the city editor is active")]
	public List<GameObject> disableWhileActive = new List<GameObject>();

	// Token: 0x04000BD3 RID: 3027
	[Tooltip("A floor objects that acts as a collider for the mouse ray")]
	public GameObject cityEditFloor;

	// Token: 0x04000BD4 RID: 3028
	[Header("State")]
	public bool needsUpdatedPathfinding;

	// Token: 0x04000BD5 RID: 3029
	public bool dataGenerated;

	// Token: 0x04000BD6 RID: 3030
	[Tooltip("Is/should the city constructor loading something? This is mostly used to trigger the DoF effect but could be useful elsewhere")]
	public bool isLoading;

	// Token: 0x04000BD7 RID: 3031
	public ButtonController previouslySelected;

	// Token: 0x04000BD8 RID: 3032
	public bool canFinishLoadFromCurrentState;

	// Token: 0x04000BD9 RID: 3033
	private CityEditorBuildingEdit _buildingEditor;

	// Token: 0x04000BDA RID: 3034
	private CityEditorStreetEdit _streetEditor;

	// Token: 0x04000BDB RID: 3035
	private CityEditorInputController _editCam;

	// Token: 0x020001D8 RID: 472
	[Serializable]
	public enum CityEditorMode
	{
		// Token: 0x04000BDE RID: 3038
		EditBuildings = 1,
		// Token: 0x04000BDF RID: 3039
		EditStreets,
		// Token: 0x04000BE0 RID: 3040
		Default = 0
	}

	// Token: 0x020001D9 RID: 473
	[Serializable]
	public enum CityEditorSubMode
	{
		// Token: 0x04000BE2 RID: 3042
		MoveSelection = 1,
		// Token: 0x04000BE3 RID: 3043
		RenameSelection,
		// Token: 0x04000BE4 RID: 3044
		Default = 0
	}

	// Token: 0x020001DA RID: 474
	// (Invoke) Token: 0x06000B50 RID: 2896
	public delegate void NewCityEditorData();
}
