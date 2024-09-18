using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001D5 RID: 469
public class CityEditorBuildingEdit : MonoBehaviour
{
	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000B22 RID: 2850 RVA: 0x000A7188 File Offset: 0x000A5388
	// (remove) Token: 0x06000B23 RID: 2851 RVA: 0x000A71C0 File Offset: 0x000A53C0
	public event CityEditorBuildingEdit.NewTileSelection OnNewTileSelection;

	// Token: 0x06000B24 RID: 2852 RVA: 0x000A71F8 File Offset: 0x000A53F8
	private void Awake()
	{
		this.buildingPresets = AssetLoader.Instance.GetAllBuildingPresets();
		if (this.buildingTypeDropdown != null)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (BuildingPreset buildingPreset in this.buildingPresets)
			{
				if (buildingPreset != null && !buildingPreset.boundary && !buildingPreset.boundaryCorner)
				{
					list.Add(buildingPreset.presetName);
					list2.Add(Strings.Get("names.rooms", buildingPreset.presetName, Strings.Casing.asIs, false, false, false, null));
				}
			}
			this.buildingTypeDropdown.AddOptions(list, false, list2);
			this.buildingTypeDropdown.SetInteractalbe(false);
		}
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x000A72CC File Offset: 0x000A54CC
	private void Update()
	{
		for (int i = 0; i < this.animatingBuildingRotation.Count; i++)
		{
			NewBuilding newBuilding = this.animatingBuildingRotation[i];
			if (newBuilding != null)
			{
				Vector3 buildingEuler = newBuilding.GetBuildingEuler();
				if (newBuilding.buildingModelBase != null)
				{
					if (newBuilding.buildingModelBase.transform.eulerAngles != buildingEuler)
					{
						newBuilding.buildingModelBase.transform.rotation = Quaternion.Lerp(newBuilding.buildingModelBase.transform.rotation, Quaternion.Euler(buildingEuler), Time.deltaTime * 10f);
					}
					else
					{
						this.animatingBuildingRotation.RemoveAt(i);
						i--;
					}
				}
			}
		}
		if (InputController.Instance.mouseInputMode && EventSystem.current.IsPointerOverGameObject())
		{
			if (this.tileSelect2.activeSelf)
			{
				this.tileSelect2.SetActive(false);
			}
			return;
		}
		this.currentlyMousedOverTile = this.TryGetTile();
		if (this.currentlyMousedOverTile != null)
		{
			this.tileSelect2.transform.position = CityData.Instance.CityTileToRealpos(this.currentlyMousedOverTile.cityCoord) + new Vector3(0f, 0.24f, 0f);
			if (!this.tileSelect2.activeSelf)
			{
				this.tileSelect2.SetActive(true);
			}
		}
		else if (this.tileSelect2.activeSelf)
		{
			this.tileSelect2.SetActive(false);
		}
		if (InputController.Instance.mouseInputMode)
		{
			if (InputController.Instance.player.GetButtonDown("Primary"))
			{
				if (HighlanderSingleton<CityEditorController>.Instance.currentSubMode == CityEditorController.CityEditorSubMode.MoveSelection)
				{
					this.ProcessSwapBuildingInput();
					return;
				}
				this.SelectBuilding(this.currentlyMousedOverTile);
				return;
			}
		}
		else
		{
			this.SelectBuilding(this.currentlyMousedOverTile);
		}
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x000A7494 File Offset: 0x000A5694
	private void SelectBuilding(CityTile newTile)
	{
		this.currentlySelectedTile = newTile;
		if (this.currentlySelectedTile != null)
		{
			this.tileSelect1.SetActive(true);
			this.tileSelect1.transform.position = CityData.Instance.CityTileToRealpos(this.currentlySelectedTile.cityCoord) + new Vector3(0f, 0.25f, 0f);
			Vector3 buildingEuler = this.currentlySelectedTile.building.GetBuildingEuler();
			buildingEuler.x = 90f;
			this.tileSelect1.transform.localEulerAngles = buildingEuler;
			this.buildingTypeDropdown.SetInteractalbe(true);
			this.buildingTypeDropdown.SelectFromStaticOption(this.currentlySelectedTile.building.preset.presetName);
			if (!this.currentlySelectedTile.building.isPlayerEditedName)
			{
				this.currentlySelectedTile.building.UpdateName(false);
				this.currentlySelectedTile.building.isPlayerEditedName = true;
			}
			this.buildingNameButton.text.text = this.currentlySelectedTile.building.name;
			this.buildingNameButton.SetInteractable(true);
			this.rotateButton.SetInteractable(true);
			this.randomNameButton.SetInteractable(true);
		}
		else
		{
			this.buildingTypeDropdown.SetInteractalbe(false);
			this.tileSelect1.SetActive(false);
			this.buildingNameButton.text.text = string.Empty;
			this.buildingNameButton.SetInteractable(false);
			this.rotateButton.SetInteractable(false);
			this.randomNameButton.SetInteractable(false);
		}
		if (this.OnNewTileSelection != null)
		{
			this.OnNewTileSelection(this.currentlySelectedTile);
		}
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x000A7648 File Offset: 0x000A5848
	public void OnRandomBuildingNameButton()
	{
		if (this.currentlySelectedTile != null && this.currentlySelectedTile.building != null)
		{
			Game.Log("Giving building a random name...", 2);
			this.currentlySelectedTile.building.isPlayerEditedName = false;
			this.currentlySelectedTile.building.UpdateName(true);
			this.currentlySelectedTile.building.isPlayerEditedName = true;
			this.buildingNameButton.text.text = this.currentlySelectedTile.building.name;
		}
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x000A76D4 File Offset: 0x000A58D4
	public void OnChangeBuildingType()
	{
		if (this.currentlySelectedTile != null && this.currentlySelectedTile.building != null && this.buildingTypeDropdown.staticOptionReference[this.buildingTypeDropdown.dropdown.value] != this.currentlySelectedTile.building.preset.presetName)
		{
			BuildingPreset buildingPreset = AssetLoader.Instance.GetAllBuildingPresets().Find((BuildingPreset item) => item.presetName == this.buildingTypeDropdown.staticOptionReference[this.buildingTypeDropdown.dropdown.value]);
			if (buildingPreset != null)
			{
				int facing = (int)this.currentlySelectedTile.building.facing;
				this.currentlySelectedTile.building.RemoveBuilding();
				NewBuilding component = Object.Instantiate<GameObject>(HighlanderSingleton<CityBuildings>.Instance.buildingPrefab, this.currentlySelectedTile.transform).GetComponent<NewBuilding>();
				component.Setup(this.currentlySelectedTile, buildingPreset, true, this.currentlySelectedTile.building.buildingID);
				component.SetFacing((NewBuilding.Direction)facing, true);
				Vector3 buildingEuler = component.GetBuildingEuler();
				buildingEuler.x = 90f;
				this.tileSelect1.transform.localEulerAngles = buildingEuler;
				this.SelectBuilding(this.currentlySelectedTile);
				HighlanderSingleton<CityEditorController>.Instance.needsUpdatedPathfinding = true;
			}
		}
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x000A7810 File Offset: 0x000A5A10
	public void OnChangeBuildingNameButton()
	{
		if (this.currentlySelectedTile != null && this.currentlySelectedTile.building != null)
		{
			PopupMessageController.Instance.PopupMessage("buildingName", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.inputField.text = this.currentlySelectedTile.building.name;
			PopupMessageController.Instance.OnLeftButton += this.OnChangeBuildingNamePopupCancel;
			PopupMessageController.Instance.OnRightButton += this.OnChangeBuildingNamePopupConfirm;
		}
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x000A78D1 File Offset: 0x000A5AD1
	private void OnChangeBuildingNamePopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeBuildingNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeBuildingNamePopupConfirm;
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x000A7900 File Offset: 0x000A5B00
	private void OnChangeBuildingNamePopupConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeBuildingNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeBuildingNamePopupConfirm;
		PopupMessageController.Instance.inputField.text = Strings.FilterInputtedText(PopupMessageController.Instance.inputField.text, true, 100);
		this.RenameSelectedBuilding(PopupMessageController.Instance.inputField.text);
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x000A7974 File Offset: 0x000A5B74
	public void RenameSelectedBuilding(string newBuildingName)
	{
		if (string.IsNullOrEmpty(newBuildingName))
		{
			HighlanderSingleton<CityEditorController>.Instance.SetCityEditorWarning("Building Name contains no valid text characters. (Only alphabet characters allowed)");
			return;
		}
		NewBuilding building = this.currentlySelectedTile.building;
		if (building == null)
		{
			HighlanderSingleton<CityEditorController>.Instance.SetCityEditorWarning("No building selected to rename.");
			return;
		}
		building.isPlayerEditedName = true;
		building.playerEditedBuildingName = newBuildingName;
		building.UpdateName(false);
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x000A79D0 File Offset: 0x000A5BD0
	public void OnRotateButton()
	{
		if (this.currentlySelectedTile != null && this.currentlySelectedTile.building != null)
		{
			Game.Log("Rotate building " + this.currentlySelectedTile.building.facing.ToString() + " current facing = " + this.currentlySelectedTile.building.facing.ToString(), 2);
			int num = (int)this.currentlySelectedTile.building.facing;
			num++;
			if (num > 3)
			{
				num = 0;
			}
			this.currentlySelectedTile.building.SetFacing((NewBuilding.Direction)num, false);
			if (!this.animatingBuildingRotation.Contains(this.currentlySelectedTile.building))
			{
				this.animatingBuildingRotation.Add(this.currentlySelectedTile.building);
			}
			Vector3 buildingEuler = this.currentlySelectedTile.building.GetBuildingEuler();
			buildingEuler.x = 90f;
			this.tileSelect1.transform.localEulerAngles = buildingEuler;
		}
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x000A7AD8 File Offset: 0x000A5CD8
	private void ProcessSwapBuildingInput()
	{
		if (this.currentlySelectedTile == null)
		{
			this.currentlySelectedTile = this.TryGetTile();
			return;
		}
		CityTile cityTile = this.TryGetTile();
		if (cityTile == null || cityTile == this.currentlySelectedTile)
		{
			this.ResetSelection();
			return;
		}
		this.SwapTiles(this.currentlySelectedTile, cityTile);
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x000A7B28 File Offset: 0x000A5D28
	private CityTile TryGetTile()
	{
		Vector3 mousePosition = Input.mousePosition;
		if (!InputController.Instance.mouseInputMode)
		{
			mousePosition..ctor((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		}
		Ray ray = HighlanderSingleton<CityEditorController>.Instance.cityEditorCam.ScreenPointToRay(mousePosition);
		int num = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			3
		});
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, ref raycastHit, 1000f, num))
		{
			Vector3 point = raycastHit.point;
			point.y = 0f;
			Vector2Int vector2Int = CityData.Instance.RealPosToGroundmap(point);
			CityTile cityTile = null;
			if (HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles.TryGetValue(vector2Int, ref cityTile))
			{
				if (cityTile.cityCoord.x <= 0 || cityTile.cityCoord.y <= 0 || (float)cityTile.cityCoord.x >= CityData.Instance.citySize.x - 1f || (float)cityTile.cityCoord.y >= CityData.Instance.citySize.y - 1f)
				{
					return null;
				}
				return cityTile;
			}
		}
		return null;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x000A7C4C File Offset: 0x000A5E4C
	private void SwapTiles(CityTile originTile, CityTile targetTile)
	{
		HighlanderSingleton<CityEditorController>.Instance.needsUpdatedPathfinding = true;
		GameObject gameObject = originTile.transform.GetChild(0).gameObject;
		GameObject gameObject2 = targetTile.transform.GetChild(0).gameObject;
		NewBuilding component = gameObject.GetComponent<NewBuilding>();
		NewBuilding component2 = gameObject2.GetComponent<NewBuilding>();
		this.currentlySelectedTile.building = component2;
		targetTile.building = component;
		gameObject.transform.SetParent(targetTile.transform, false);
		gameObject2.transform.SetParent(this.currentlySelectedTile.transform, false);
		this.ResetSelection();
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x000A7CD6 File Offset: 0x000A5ED6
	private void ResetSelection()
	{
		this.SelectBuilding(null);
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x000A7CE0 File Offset: 0x000A5EE0
	private void OnDisable()
	{
		this.tileSelect1.SetActive(false);
		this.tileSelect2.SetActive(false);
		foreach (NewBuilding newBuilding in this.animatingBuildingRotation)
		{
			if (newBuilding != null && newBuilding.buildingModelBase != null)
			{
				newBuilding.buildingModelBase.transform.eulerAngles = newBuilding.GetBuildingEuler();
			}
		}
		this.ResetSelection();
	}

	// Token: 0x04000BC1 RID: 3009
	[Header("References")]
	public GameObject tileSelect1;

	// Token: 0x04000BC2 RID: 3010
	public GameObject tileSelect2;

	// Token: 0x04000BC3 RID: 3011
	public DropdownController buildingTypeDropdown;

	// Token: 0x04000BC4 RID: 3012
	public ButtonController buildingNameButton;

	// Token: 0x04000BC5 RID: 3013
	public ButtonController randomNameButton;

	// Token: 0x04000BC6 RID: 3014
	public ButtonController rotateButton;

	// Token: 0x04000BC7 RID: 3015
	[Header("State")]
	public CityTile currentlyMousedOverTile;

	// Token: 0x04000BC8 RID: 3016
	public CityTile currentlySelectedTile;

	// Token: 0x04000BC9 RID: 3017
	private List<BuildingPreset> buildingPresets = new List<BuildingPreset>();

	// Token: 0x04000BCA RID: 3018
	private List<NewBuilding> animatingBuildingRotation = new List<NewBuilding>();

	// Token: 0x020001D6 RID: 470
	// (Invoke) Token: 0x06000B36 RID: 2870
	public delegate void NewTileSelection(CityTile newSelected);
}
