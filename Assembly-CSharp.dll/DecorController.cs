using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005E2 RID: 1506
public class DecorController : MonoBehaviour
{
	// Token: 0x06002120 RID: 8480 RVA: 0x001C4A90 File Offset: 0x001C2C90
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.wcc = newContentController;
		this.room = Player.Instance.currentRoom;
		Game.Log("Decor: Starting decor controller for : " + this.decorType.ToString(), 2);
		this.SetPageSize(new Vector2(640f, 748f));
		this.SetDecorType((int)PlayerApartmentController.Instance.rememberDecorType);
		this.SetSelected(null);
		this.isSetup = true;
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x001C4B19 File Offset: 0x001C2D19
	public void SetDecorType(int newType)
	{
		this.decorType = (MaterialGroupPreset.MaterialType)newType;
		this.UpdateListDisplay();
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x001C4B28 File Offset: 0x001C2D28
	public void SetPageSize(Vector2 newSize)
	{
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x001C4B4C File Offset: 0x001C2D4C
	public void UpdateListDisplay()
	{
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		if (this.decorType == MaterialGroupPreset.MaterialType.walls)
		{
			this.wallsButton.SetInteractable(false);
			this.floorButton.SetInteractable(true);
			this.ceilingButton.SetInteractable(true);
		}
		else if (this.decorType == MaterialGroupPreset.MaterialType.floor)
		{
			this.wallsButton.SetInteractable(true);
			this.floorButton.SetInteractable(false);
			this.ceilingButton.SetInteractable(true);
		}
		else if (this.decorType == MaterialGroupPreset.MaterialType.ceiling)
		{
			this.wallsButton.SetInteractable(true);
			this.floorButton.SetInteractable(true);
			this.ceilingButton.SetInteractable(false);
		}
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		while (this.spawnedEntries.Count > 0)
		{
			Object.Destroy(this.spawnedEntries[0].gameObject);
			this.spawnedEntries.RemoveAt(0);
		}
		List<MaterialGroupPreset> list = Toolbox.Instance.allMaterialGroups.FindAll((MaterialGroupPreset item) => item.purchasable && item.materialType == this.decorType);
		List<MaterialGroupPreset> list2 = new List<MaterialGroupPreset>();
		foreach (MaterialGroupPreset materialGroupPreset in list)
		{
			if (!list2.Contains(materialGroupPreset))
			{
				if (this.searchInputField.text.Length <= 0)
				{
					list2.Add(materialGroupPreset);
				}
				else if (Strings.Get("evidence.names", materialGroupPreset.name, Strings.Casing.asIs, false, false, false, null).ToLower().Contains(this.searchInputField.text.ToLower()))
				{
					list2.Add(materialGroupPreset);
				}
			}
		}
		list2.Sort((MaterialGroupPreset p1, MaterialGroupPreset p2) => p1.price.CompareTo(p2.price));
		for (int i = 0; i < this.spawnedEntries.Count; i++)
		{
			DecorElementController decorElementController = this.spawnedEntries[i];
			if (decorElementController == null)
			{
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
			else if (list2.Contains(decorElementController.preset))
			{
				list2.Remove(decorElementController.preset);
				decorElementController.VisualUpdate();
			}
			else
			{
				Object.Destroy(decorElementController);
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
		}
		foreach (MaterialGroupPreset newPreset in list2)
		{
			DecorElementController component = Object.Instantiate<GameObject>(this.decorElementPrefab, this.entryParent).GetComponent<DecorElementController>();
			component.Setup(newPreset, this, this.wcc.window);
			this.spawnedEntries.Add(component);
		}
		for (int j = 0; j < this.spawnedEntries.Count; j++)
		{
			this.spawnedEntries[j].transform.SetAsLastSibling();
		}
		this.entryParent.sizeDelta = new Vector2(this.entryParent.sizeDelta.x, Mathf.Max((float)this.spawnedEntries.Count * 130f + 24f, 466f));
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x001C4E98 File Offset: 0x001C3098
	public void ClearSearchButton()
	{
		this.searchInputField.text = string.Empty;
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x001C4EAC File Offset: 0x001C30AC
	public void SetSelected(MaterialGroupPreset newSelection)
	{
		if (newSelection != null)
		{
			Game.Log("Decor: Set selected material: " + ((newSelection != null) ? newSelection.ToString() : null), 2);
			if (PlayerApartmentController.Instance.decoratingMode)
			{
				PlayerApartmentController.Instance.SetDecoratingMode(false, null, MaterialGroupPreset.MaterialType.walls, null, null);
			}
			Toolbox.MaterialKey materialKey = this.room.defaultWallKey;
			if (this.decorType == MaterialGroupPreset.MaterialType.floor)
			{
				materialKey = this.room.floorMatKey;
			}
			else if (this.decorType == MaterialGroupPreset.MaterialType.ceiling)
			{
				materialKey = this.room.ceilingMatKey;
			}
			PlayerApartmentController.Instance.SetDecoratingMode(true, newSelection, this.decorType, materialKey, this.room);
			PlayerApartmentController.Instance.OpenOrUpdateMaterialWindow(null, materialKey, newSelection);
			this.wcc.window.CloseWindow(false);
		}
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x001C4F6D File Offset: 0x001C316D
	private void OnDestroy()
	{
		PlayerApartmentController.Instance.rememberDecorType = this.decorType;
		if (!PlayerApartmentController.Instance.furniturePlacementMode && !PlayerApartmentController.Instance.decoratingMode)
		{
			SessionData.Instance.ResumeGame();
		}
	}

	// Token: 0x04002B53 RID: 11091
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002B54 RID: 11092
	public WindowContentController wcc;

	// Token: 0x04002B55 RID: 11093
	public RectTransform entryParent;

	// Token: 0x04002B56 RID: 11094
	public ButtonController wallsButton;

	// Token: 0x04002B57 RID: 11095
	public ButtonController ceilingButton;

	// Token: 0x04002B58 RID: 11096
	public ButtonController floorButton;

	// Token: 0x04002B59 RID: 11097
	public GameObject decorElementPrefab;

	// Token: 0x04002B5A RID: 11098
	[Header("State")]
	public MaterialGroupPreset.MaterialType decorType;

	// Token: 0x04002B5B RID: 11099
	public bool isSetup;

	// Token: 0x04002B5C RID: 11100
	public NewRoom room;

	// Token: 0x04002B5D RID: 11101
	public MaterialKeyController keyController;

	// Token: 0x04002B5E RID: 11102
	public TMP_InputField searchInputField;

	// Token: 0x04002B5F RID: 11103
	public List<DecorElementController> spawnedEntries = new List<DecorElementController>();
}
