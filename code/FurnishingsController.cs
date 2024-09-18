using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005E4 RID: 1508
public class FurnishingsController : MonoBehaviour
{
	// Token: 0x0600212C RID: 8492 RVA: 0x001C4FF0 File Offset: 0x001C31F0
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.wcc = newContentController;
		this.room = Player.Instance.currentRoom;
		this.SetPageSize(new Vector2(640f, 748f));
		this.displayClasses = new List<FurniturePreset.DecorClass>(PlayerApartmentController.Instance.rememberDisplayClasses);
		this.SetTabState(PlayerApartmentController.Instance.rememberRoomStorageShop, true);
		this.isSetup = true;
		PlayerApartmentController.Instance.OnFurnitureChange += this.OnFurnitureChange;
	}

	// Token: 0x0600212D RID: 8493 RVA: 0x001C507D File Offset: 0x001C327D
	public void SetPageSize(Vector2 newSize)
	{
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x001C50A1 File Offset: 0x001C32A1
	public void SetTabState(int newState)
	{
		this.SetTabState((FurnishingsController.TabState)newState, false);
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x001C50AC File Offset: 0x001C32AC
	public void SetTabState(FurnishingsController.TabState newState, bool forceUpdate = false)
	{
		if (newState != this.tabState || forceUpdate)
		{
			this.tabState = newState;
			while (this.spawnedEntries.Count > 0)
			{
				Object.Destroy(this.spawnedEntries[0].gameObject);
				this.spawnedEntries.RemoveAt(0);
			}
			this.UpdateListDisplay();
		}
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x001C5108 File Offset: 0x001C3308
	public void UpdateListDisplay()
	{
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		if (this.tabState == FurnishingsController.TabState.inRoom)
		{
			this.inRoomButton.SetInteractable(false);
			this.inShopButton.SetInteractable(true);
			this.inStorageButton.SetInteractable(true);
		}
		else if (this.tabState == FurnishingsController.TabState.inShop)
		{
			this.inRoomButton.SetInteractable(true);
			this.inShopButton.SetInteractable(false);
			this.inStorageButton.SetInteractable(true);
		}
		else if (this.tabState == FurnishingsController.TabState.inStorage)
		{
			this.inRoomButton.SetInteractable(true);
			this.inShopButton.SetInteractable(true);
			this.inStorageButton.SetInteractable(false);
		}
		while (this.spawnedEntries.Count > 0)
		{
			Object.Destroy(this.spawnedEntries[0].gameObject);
			this.spawnedEntries.RemoveAt(0);
		}
		if (this.displayClasses.Contains(FurniturePreset.DecorClass.chairs))
		{
			this.chairsButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.chairsButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(FurniturePreset.DecorClass.tables))
		{
			this.tablesButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.tablesButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(FurniturePreset.DecorClass.units))
		{
			this.unitsButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.unitsButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(FurniturePreset.DecorClass.electronics))
		{
			this.electronicsButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.electronicsButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(FurniturePreset.DecorClass.structural))
		{
			this.structuralButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.structuralButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(FurniturePreset.DecorClass.decoration))
		{
			this.decorationButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.decorationButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(FurniturePreset.DecorClass.misc))
		{
			this.miscButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.miscButton.icon.sprite = this.uncheckedSprite;
		}
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		new List<FurniturePreset>();
		List<FurnitureLocation> list = new List<FurnitureLocation>();
		List<FurniturePreset> list2 = new List<FurniturePreset>();
		List<FurnitureLocation> list3 = new List<FurnitureLocation>();
		if (this.tabState == FurnishingsController.TabState.inRoom)
		{
			list = new List<FurnitureLocation>(this.room.individualFurniture);
		}
		else if (this.tabState == FurnishingsController.TabState.inStorage)
		{
			list = new List<FurnitureLocation>(PlayerApartmentController.Instance.furnitureStorage);
		}
		else if (this.tabState == FurnishingsController.TabState.inShop)
		{
			foreach (FurniturePreset furniturePreset in Toolbox.Instance.allFurniture.FindAll((FurniturePreset item) => item.purchasable && this.displayClasses.Contains(item.decorClass)))
			{
				if (!list2.Contains(furniturePreset))
				{
					if (this.searchInputField.text.Length <= 0)
					{
						list2.Add(furniturePreset);
					}
					else if (Strings.Get("evidence.names", furniturePreset.name, Strings.Casing.asIs, false, false, false, null).ToLower().Contains(this.searchInputField.text.ToLower()))
					{
						list2.Add(furniturePreset);
					}
				}
			}
		}
		if (this.tabState == FurnishingsController.TabState.inRoom || this.tabState == FurnishingsController.TabState.inStorage)
		{
			foreach (FurnitureLocation furnitureLocation in list)
			{
				if (!list3.Contains(furnitureLocation) && this.displayClasses.Contains(furnitureLocation.furniture.decorClass))
				{
					if (this.searchInputField.text.Length <= 0)
					{
						list3.Add(furnitureLocation);
					}
					else if (Strings.Get("evidence.names", furnitureLocation.furniture.name, Strings.Casing.asIs, false, false, false, null).ToLower().Contains(this.searchInputField.text.ToLower()))
					{
						list3.Add(furnitureLocation);
					}
				}
			}
		}
		list2.Sort((FurniturePreset p1, FurniturePreset p2) => p1.cost.CompareTo(p2.cost));
		list3.Sort((FurnitureLocation p1, FurnitureLocation p2) => p1.furniture.cost.CompareTo(p2.furniture.cost));
		for (int i = 0; i < this.spawnedEntries.Count; i++)
		{
			DecorElementController decorElementController = this.spawnedEntries[i];
			if (decorElementController == null)
			{
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
			else if (list2.Contains(decorElementController.furniture))
			{
				list2.Remove(decorElementController.furniture);
				decorElementController.VisualUpdate();
			}
			else if (list3.Contains(decorElementController.worldFurnitureReference))
			{
				list2.Remove(decorElementController.furniture);
				decorElementController.VisualUpdate();
			}
			else
			{
				Object.Destroy(decorElementController.gameObject);
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
		}
		foreach (FurniturePreset newFurniture in list2)
		{
			DecorElementController component = Object.Instantiate<GameObject>(this.furnitureElementPrefab, this.entryParent).GetComponent<DecorElementController>();
			component.SetupFurniture(newFurniture, this, this.wcc.window, null);
			this.spawnedEntries.Add(component);
		}
		foreach (FurnitureLocation furnitureLocation2 in list3)
		{
			DecorElementController component2 = Object.Instantiate<GameObject>(this.furnitureElementPrefab, this.entryParent).GetComponent<DecorElementController>();
			component2.SetupFurniture(furnitureLocation2.furniture, this, this.wcc.window, furnitureLocation2);
			this.spawnedEntries.Add(component2);
		}
		for (int j = 0; j < this.spawnedEntries.Count; j++)
		{
			this.spawnedEntries[j].transform.SetAsLastSibling();
		}
		this.entryParent.sizeDelta = new Vector2(this.entryParent.sizeDelta.x, Mathf.Max((float)this.spawnedEntries.Count * 130f + 24f, 466f));
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x001C5808 File Offset: 0x001C3A08
	public void ToggleDisplayClass(int classInt)
	{
		if (this.displayClasses.Contains((FurniturePreset.DecorClass)classInt))
		{
			this.displayClasses.Remove((FurniturePreset.DecorClass)classInt);
		}
		else
		{
			this.displayClasses.Add((FurniturePreset.DecorClass)classInt);
		}
		this.UpdateListDisplay();
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x001C5848 File Offset: 0x001C3A48
	public void SetSelected(FurniturePreset newSelection, FurnitureLocation existingLocation, bool newPlaceExistingRoomObject)
	{
		Game.Log("Decor: Set selected furniture: " + ((newSelection != null) ? newSelection.ToString() : null), 2);
		if (PlayerApartmentController.Instance.furniturePlacementMode)
		{
			Game.Log("Decor: Already in furniture placement mode, removing the furniture currently being placed...", 2);
			PlayerApartmentController.Instance.RemoveBeingPlaced();
		}
		PlayerApartmentController.Instance.ResetExisting();
		if (newSelection != null)
		{
			Game.Log("Decor: Set selected furniture: " + ((newSelection != null) ? newSelection.ToString() : null), 2);
			PlayerApartmentController.FurniturePlacement furniturePlacement = new PlayerApartmentController.FurniturePlacement();
			furniturePlacement.preset = newSelection;
			furniturePlacement.existing = existingLocation;
			if (furniturePlacement.existing != null && furniturePlacement.existing.id != 0)
			{
				furniturePlacement.materialKey = existingLocation.matKey;
				furniturePlacement.art = existingLocation.art;
				PlayerApartmentController.Instance.MoveFurnitureToStorage(existingLocation);
			}
			PlayerApartmentController.Instance.SetFurniturePlacementMode(true, furniturePlacement, this.room, newPlaceExistingRoomObject, true);
			if (furniturePlacement.existing == null || furniturePlacement.existing.id <= 0)
			{
				PlayerApartmentController.Instance.OpenOrUpdateMaterialWindow(newSelection, new Toolbox.MaterialKey(), null);
			}
			else
			{
				SessionData.Instance.ResumeGame();
			}
			this.wcc.window.CloseWindow(false);
			return;
		}
		if (PlayerApartmentController.Instance.furniturePlacementMode)
		{
			Game.Log("Decor: Cancelling furniture placement mode as selection is null", 2);
			PlayerApartmentController.Instance.SetFurniturePlacementMode(false, null, null, false, false);
		}
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x001C5992 File Offset: 0x001C3B92
	public void ClearSearchButton()
	{
		this.searchInputField.text = string.Empty;
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x001C59A4 File Offset: 0x001C3BA4
	public void OnFurnitureChange()
	{
		this.UpdateListDisplay();
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x001C59AC File Offset: 0x001C3BAC
	private void OnDestroy()
	{
		PlayerApartmentController.Instance.OnFurnitureChange -= this.OnFurnitureChange;
		PlayerApartmentController.Instance.rememberRoomStorageShop = this.tabState;
		PlayerApartmentController.Instance.rememberDisplayClasses = new List<FurniturePreset.DecorClass>(this.displayClasses);
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x001C59EC File Offset: 0x001C3BEC
	public void MoveAllToStorageButton()
	{
		PopupMessageController.Instance.PopupMessage("Move to Storage", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnRightButton += this.ConfirmMoveToStorage;
		PopupMessageController.Instance.OnLeftButton += this.CancelMoveToStorage;
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x001C5A68 File Offset: 0x001C3C68
	public void ConfirmMoveToStorage()
	{
		PopupMessageController.Instance.OnRightButton -= this.ConfirmMoveToStorage;
		PopupMessageController.Instance.OnLeftButton -= this.CancelMoveToStorage;
		Game.Log("Decor: Move all furniture to storage...", 2);
		foreach (FurnitureLocation newStorage in new List<FurnitureLocation>(this.room.individualFurniture))
		{
			PlayerApartmentController.Instance.MoveFurnitureToStorage(newStorage);
		}
		this.UpdateListDisplay();
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x001C5B08 File Offset: 0x001C3D08
	public void CancelMoveToStorage()
	{
		PopupMessageController.Instance.OnRightButton -= this.ConfirmMoveToStorage;
		PopupMessageController.Instance.OnLeftButton -= this.CancelMoveToStorage;
	}

	// Token: 0x04002B62 RID: 11106
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002B63 RID: 11107
	public WindowContentController wcc;

	// Token: 0x04002B64 RID: 11108
	public RectTransform entryParent;

	// Token: 0x04002B65 RID: 11109
	public ButtonController inRoomButton;

	// Token: 0x04002B66 RID: 11110
	public ButtonController inStorageButton;

	// Token: 0x04002B67 RID: 11111
	public ButtonController inShopButton;

	// Token: 0x04002B68 RID: 11112
	public GameObject furnitureElementPrefab;

	// Token: 0x04002B69 RID: 11113
	public ButtonController chairsButton;

	// Token: 0x04002B6A RID: 11114
	public ButtonController tablesButton;

	// Token: 0x04002B6B RID: 11115
	public ButtonController unitsButton;

	// Token: 0x04002B6C RID: 11116
	public ButtonController electronicsButton;

	// Token: 0x04002B6D RID: 11117
	public ButtonController structuralButton;

	// Token: 0x04002B6E RID: 11118
	public ButtonController decorationButton;

	// Token: 0x04002B6F RID: 11119
	public ButtonController miscButton;

	// Token: 0x04002B70 RID: 11120
	[Header("Settings")]
	public Sprite uncheckedSprite;

	// Token: 0x04002B71 RID: 11121
	public Sprite checkedSprite;

	// Token: 0x04002B72 RID: 11122
	[Header("State")]
	public bool isSetup;

	// Token: 0x04002B73 RID: 11123
	public FurnishingsController.TabState tabState;

	// Token: 0x04002B74 RID: 11124
	public List<FurniturePreset.DecorClass> displayClasses = new List<FurniturePreset.DecorClass>();

	// Token: 0x04002B75 RID: 11125
	public NewRoom room;

	// Token: 0x04002B76 RID: 11126
	public MaterialKeyController keyController;

	// Token: 0x04002B77 RID: 11127
	public TMP_InputField searchInputField;

	// Token: 0x04002B78 RID: 11128
	public List<DecorElementController> spawnedEntries = new List<DecorElementController>();

	// Token: 0x020005E5 RID: 1509
	public enum TabState
	{
		// Token: 0x04002B7A RID: 11130
		inRoom,
		// Token: 0x04002B7B RID: 11131
		inStorage,
		// Token: 0x04002B7C RID: 11132
		inShop
	}
}
