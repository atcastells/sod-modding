using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005DE RID: 1502
public class ApartmentItemsController : MonoBehaviour
{
	// Token: 0x06002101 RID: 8449 RVA: 0x001C3A14 File Offset: 0x001C1C14
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.wcc = newContentController;
		this.room = Player.Instance.currentRoom;
		this.SetPageSize(new Vector2(640f, 748f));
		this.displayClasses = new List<InteractablePreset.ItemClass>(PlayerApartmentController.Instance.rememberItemDisplayClasses);
		this.SetTabState(PlayerApartmentController.Instance.rememberRoomStorageShop, true);
		this.isSetup = true;
		PlayerApartmentController.Instance.OnFurnitureChange += this.OnFurnitureChange;
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x001C3AA1 File Offset: 0x001C1CA1
	public void SetPageSize(Vector2 newSize)
	{
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x001C3AC5 File Offset: 0x001C1CC5
	public void SetTabState(int newState)
	{
		this.SetTabState((FurnishingsController.TabState)newState, false);
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x001C3AD0 File Offset: 0x001C1CD0
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

	// Token: 0x06002105 RID: 8453 RVA: 0x001C3B2C File Offset: 0x001C1D2C
	public void UpdateListDisplay()
	{
		Game.Log("Decor: Update item list display...", 2);
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
		if (this.displayClasses.Contains(InteractablePreset.ItemClass.consumable))
		{
			this.consumableButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.consumableButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(InteractablePreset.ItemClass.medical))
		{
			this.medicalButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.medicalButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(InteractablePreset.ItemClass.equipment))
		{
			this.equipmentButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.equipmentButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(InteractablePreset.ItemClass.document))
		{
			this.documentsButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.documentsButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(InteractablePreset.ItemClass.electronics))
		{
			this.electronicsButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.electronicsButton.icon.sprite = this.uncheckedSprite;
		}
		if (this.displayClasses.Contains(InteractablePreset.ItemClass.misc))
		{
			this.miscButton.icon.sprite = this.checkedSprite;
		}
		else
		{
			this.miscButton.icon.sprite = this.uncheckedSprite;
		}
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		List<InteractablePreset> list = new List<InteractablePreset>();
		List<Interactable> list2 = new List<Interactable>();
		List<InteractablePreset> list3 = new List<InteractablePreset>();
		List<Interactable> list4 = new List<Interactable>();
		if (this.tabState == FurnishingsController.TabState.inRoom)
		{
			list2 = new List<Interactable>();
			using (HashSet<NewNode>.Enumerator enumerator = this.room.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewNode newNode = enumerator.Current;
					foreach (Interactable interactable in newNode.interactables)
					{
						if (interactable.preset.spawnable && !(interactable.preset.prefab == null) && !interactable.rem && !interactable.rPl && interactable.preset.allowInApartmentStorage && !PlayerApartmentController.Instance.itemStorage.Contains(interactable) && !list2.Contains(interactable))
						{
							list2.Add(interactable);
						}
					}
				}
				goto IL_4A4;
			}
		}
		if (this.tabState == FurnishingsController.TabState.inStorage)
		{
			list2 = new List<Interactable>(PlayerApartmentController.Instance.itemStorage);
		}
		else if (this.tabState == FurnishingsController.TabState.inShop)
		{
			list = new List<InteractablePreset>();
			foreach (KeyValuePair<string, InteractablePreset> keyValuePair in Toolbox.Instance.objectPresetDictionary)
			{
				if (keyValuePair.Value.spawnable && keyValuePair.Value.prefab != null && keyValuePair.Value.allowInApartmentShop)
				{
					list.Add(keyValuePair.Value);
				}
			}
			foreach (InteractablePreset interactablePreset in list)
			{
				if (!list3.Contains(interactablePreset))
				{
					if (this.searchInputField.text.Length <= 0)
					{
						list3.Add(interactablePreset);
					}
					else if (Strings.Get("evidence.names", interactablePreset.presetName, Strings.Casing.asIs, false, false, false, null).ToLower().Contains(this.searchInputField.text.ToLower()))
					{
						list3.Add(interactablePreset);
					}
				}
			}
		}
		IL_4A4:
		if (this.tabState == FurnishingsController.TabState.inRoom || this.tabState == FurnishingsController.TabState.inStorage)
		{
			foreach (Interactable interactable2 in list2)
			{
				if (!list4.Contains(interactable2) && this.displayClasses.Contains(interactable2.preset.itemClass))
				{
					if (this.searchInputField.text.Length <= 0)
					{
						list4.Add(interactable2);
					}
					else if (interactable2.GetName().ToLower().Contains(this.searchInputField.text.ToLower()))
					{
						list4.Add(interactable2);
					}
				}
			}
		}
		list3.Sort((InteractablePreset p1, InteractablePreset p2) => p1.value.x.CompareTo(p2.value.x));
		list4.Sort((Interactable p1, Interactable p2) => p1.preset.value.x.CompareTo(p2.preset.value.x));
		for (int i = 0; i < this.spawnedEntries.Count; i++)
		{
			ApartmentItemElementController apartmentItemElementController = this.spawnedEntries[i];
			if (apartmentItemElementController == null)
			{
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
			else if (list3.Contains(apartmentItemElementController.itemPreset))
			{
				list3.Remove(apartmentItemElementController.itemPreset);
				apartmentItemElementController.VisualUpdate();
			}
			else if (list4.Contains(apartmentItemElementController.worldItemReference))
			{
				list3.Remove(apartmentItemElementController.itemPreset);
				apartmentItemElementController.VisualUpdate();
			}
			else
			{
				Object.Destroy(apartmentItemElementController.gameObject);
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
		}
		foreach (InteractablePreset newItemPreset in list3)
		{
			ApartmentItemElementController component = Object.Instantiate<GameObject>(this.itemElementPrefab, this.entryParent).GetComponent<ApartmentItemElementController>();
			component.SetupItem(newItemPreset, this, this.wcc.window, null);
			this.spawnedEntries.Add(component);
		}
		foreach (Interactable interactable3 in list4)
		{
			ApartmentItemElementController component2 = Object.Instantiate<GameObject>(this.itemElementPrefab, this.entryParent).GetComponent<ApartmentItemElementController>();
			component2.SetupItem(interactable3.preset, this, this.wcc.window, interactable3);
			this.spawnedEntries.Add(component2);
		}
		for (int j = 0; j < this.spawnedEntries.Count; j++)
		{
			this.spawnedEntries[j].transform.SetAsLastSibling();
		}
		this.entryParent.sizeDelta = new Vector2(this.entryParent.sizeDelta.x, Mathf.Max((float)this.spawnedEntries.Count * 130f + 24f, 466f));
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x001C432C File Offset: 0x001C252C
	public void ToggleDisplayClass(int classInt)
	{
		if (this.displayClasses.Contains((InteractablePreset.ItemClass)classInt))
		{
			this.displayClasses.Remove((InteractablePreset.ItemClass)classInt);
		}
		else
		{
			this.displayClasses.Add((InteractablePreset.ItemClass)classInt);
		}
		this.UpdateListDisplay();
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x001C436C File Offset: 0x001C256C
	public void PlaceObject(Interactable existingObject)
	{
		if (existingObject.preset.actionsPreset.Exists((InteractableActionsPreset item) => item.actions.Exists((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == "take (lockpick)")))
		{
			if (existingObject.preset.presetName == "LockpickKit")
			{
				ActionController.Instance.TakeLockpickKit(existingObject, Player.Instance.currentNode, Player.Instance);
			}
			else
			{
				ActionController.Instance.TakeLockpick(existingObject, Player.Instance.currentNode, Player.Instance);
			}
			this.UpdateListDisplay();
			return;
		}
		Game.Log("Decor: Set selected item: " + existingObject.GetName(), 2);
		existingObject.rPl = false;
		ActionController.Instance.PickUp(existingObject, Player.Instance.currentNode, Player.Instance);
		if (existingObject.spawnedObject == null)
		{
			string text = "Decor: Unable to spawn object to world! ";
			Human inInventory = existingObject.inInventory;
			Game.Log(text + ((inInventory != null) ? inInventory.ToString() : null), 2);
		}
		existingObject.SetPhysicsPickupState(true, Player.Instance, true, false);
		SessionData.Instance.ResumeGame();
	}

	// Token: 0x06002108 RID: 8456 RVA: 0x001C447D File Offset: 0x001C267D
	public void ClearSearchButton()
	{
		this.searchInputField.text = string.Empty;
	}

	// Token: 0x06002109 RID: 8457 RVA: 0x001C448F File Offset: 0x001C268F
	public void OnFurnitureChange()
	{
		this.UpdateListDisplay();
	}

	// Token: 0x0600210A RID: 8458 RVA: 0x001C4497 File Offset: 0x001C2697
	private void OnDestroy()
	{
		PlayerApartmentController.Instance.OnFurnitureChange -= this.OnFurnitureChange;
		PlayerApartmentController.Instance.rememberRoomStorageShop = this.tabState;
		PlayerApartmentController.Instance.rememberItemDisplayClasses = new List<InteractablePreset.ItemClass>(this.displayClasses);
	}

	// Token: 0x0600210B RID: 8459 RVA: 0x001C44D4 File Offset: 0x001C26D4
	public void MoveAllToStorageButton()
	{
		PopupMessageController.Instance.PopupMessage("Move to Storage", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnRightButton += this.ConfirmMoveToStorage;
		PopupMessageController.Instance.OnLeftButton += this.CancelMoveToStorage;
	}

	// Token: 0x0600210C RID: 8460 RVA: 0x001C4550 File Offset: 0x001C2750
	public void ConfirmMoveToStorage()
	{
		PopupMessageController.Instance.OnRightButton -= this.ConfirmMoveToStorage;
		PopupMessageController.Instance.OnLeftButton -= this.CancelMoveToStorage;
		Game.Log("Decor: Move all items to storage...", 2);
		List<Interactable> list = new List<Interactable>();
		foreach (NewNode newNode in this.room.nodes)
		{
			foreach (Interactable interactable in newNode.interactables)
			{
				if ((interactable.furnitureParent == null || !interactable.furnitureParent.integratedInteractables.Contains(interactable)) && interactable.preset.spawnable && !interactable.preset.disableMoveToStorage && !list.Contains(interactable))
				{
					list.Add(interactable);
				}
			}
		}
		foreach (Interactable newStorage in list)
		{
			PlayerApartmentController.Instance.MoveItemToStorage(newStorage);
		}
		this.UpdateListDisplay();
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x001C46A8 File Offset: 0x001C28A8
	public void CancelMoveToStorage()
	{
		PopupMessageController.Instance.OnRightButton -= this.ConfirmMoveToStorage;
		PopupMessageController.Instance.OnLeftButton -= this.CancelMoveToStorage;
	}

	// Token: 0x04002B30 RID: 11056
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002B31 RID: 11057
	public WindowContentController wcc;

	// Token: 0x04002B32 RID: 11058
	public RectTransform entryParent;

	// Token: 0x04002B33 RID: 11059
	public ButtonController inRoomButton;

	// Token: 0x04002B34 RID: 11060
	public ButtonController inStorageButton;

	// Token: 0x04002B35 RID: 11061
	public ButtonController inShopButton;

	// Token: 0x04002B36 RID: 11062
	public GameObject itemElementPrefab;

	// Token: 0x04002B37 RID: 11063
	public ButtonController consumableButton;

	// Token: 0x04002B38 RID: 11064
	public ButtonController medicalButton;

	// Token: 0x04002B39 RID: 11065
	public ButtonController equipmentButton;

	// Token: 0x04002B3A RID: 11066
	public ButtonController electronicsButton;

	// Token: 0x04002B3B RID: 11067
	public ButtonController documentsButton;

	// Token: 0x04002B3C RID: 11068
	public ButtonController miscButton;

	// Token: 0x04002B3D RID: 11069
	[Header("Settings")]
	public Sprite uncheckedSprite;

	// Token: 0x04002B3E RID: 11070
	public Sprite checkedSprite;

	// Token: 0x04002B3F RID: 11071
	[Header("State")]
	public bool isSetup;

	// Token: 0x04002B40 RID: 11072
	public FurnishingsController.TabState tabState;

	// Token: 0x04002B41 RID: 11073
	public List<InteractablePreset.ItemClass> displayClasses = new List<InteractablePreset.ItemClass>();

	// Token: 0x04002B42 RID: 11074
	public NewRoom room;

	// Token: 0x04002B43 RID: 11075
	public TMP_InputField searchInputField;

	// Token: 0x04002B44 RID: 11076
	public List<ApartmentItemElementController> spawnedEntries = new List<ApartmentItemElementController>();
}
