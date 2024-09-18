using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000562 RID: 1378
public class DecorElementController : MonoBehaviour
{
	// Token: 0x06001DF4 RID: 7668 RVA: 0x001A49A8 File Offset: 0x001A2BA8
	public void Setup(MaterialGroupPreset newPreset, DecorController newDecorController, InfoWindow newThisWindow)
	{
		this.preset = newPreset;
		this.thisWindow = newThisWindow;
		this.decorController = newDecorController;
		if (this.preset != null)
		{
			if (newPreset.materialType == MaterialGroupPreset.MaterialType.walls)
			{
				this.price = this.preset.price * Player.Instance.currentRoom.GetWallCount();
			}
			else
			{
				this.price = this.preset.price * Player.Instance.currentRoom.nodes.Count;
			}
			this.mainImage.sprite = this.preset.decorSprite;
			this.mainImage.color = Color.white;
		}
		this.UpdateButtonText();
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x001A4A58 File Offset: 0x001A2C58
	private Texture2D GenerateDecorIcon(int size = 128)
	{
		Texture2D texture2D = new Texture2D(128, 128);
		Texture2D texture2D2 = this.preset.material.GetTexture("_BaseColorMap") as Texture2D;
		if (!texture2D2.isReadable)
		{
			Game.LogError(texture2D2.name + " is not readable!", 2);
			return texture2D;
		}
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				if (i < texture2D2.width && j < texture2D2.height)
				{
					texture2D.SetPixel(i, j, texture2D2.GetPixel(i, j));
				}
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x001A4AF4 File Offset: 0x001A2CF4
	public void SetupFurniture(FurniturePreset newFurniture, FurnishingsController newDecorController, InfoWindow newThisWindow, FurnitureLocation newWorldFurnReference)
	{
		this.furniture = newFurniture;
		this.thisWindow = newThisWindow;
		this.furnishingsController = newDecorController;
		this.worldFurnitureReference = newWorldFurnReference;
		if (this.storageButton != null)
		{
			this.storageButton.SetInteractable(false);
			this.storageButton.gameObject.SetActive(false);
		}
		if (this.furniture != null)
		{
			if (this.worldFurnitureReference == null)
			{
				this.price = this.furniture.cost;
			}
			else
			{
				this.price = 0;
				if (this.storageButton != null)
				{
					this.storageButton.SetInteractable(true);
					this.storageButton.gameObject.SetActive(true);
				}
			}
			this.mainImage.sprite = this.furniture.staticImage;
			this.mainImage.color = Color.white;
		}
		if (this.furniture.decorClass == FurniturePreset.DecorClass.chairs)
		{
			this.icon.sprite = this.chairIcon;
		}
		else if (this.furniture.decorClass == FurniturePreset.DecorClass.tables)
		{
			this.icon.sprite = this.tableIcon;
		}
		else if (this.furniture.decorClass == FurniturePreset.DecorClass.units)
		{
			this.icon.sprite = this.unitIcon;
		}
		else if (this.furniture.decorClass == FurniturePreset.DecorClass.electronics)
		{
			this.icon.sprite = this.electronicsIcon;
		}
		else if (this.furniture.decorClass == FurniturePreset.DecorClass.structural)
		{
			this.icon.sprite = this.structuralIcon;
		}
		else if (this.furniture.decorClass == FurniturePreset.DecorClass.decoration)
		{
			this.icon.sprite = this.decorationIcon;
		}
		else if (this.furniture.decorClass == FurniturePreset.DecorClass.misc)
		{
			this.icon.sprite = this.miscIcon;
		}
		this.VisualUpdate();
	}

	// Token: 0x06001DF7 RID: 7671 RVA: 0x001A4CBD File Offset: 0x001A2EBD
	public void VisualUpdate()
	{
		this.UpdateButtonText();
		this.UpdatePurchaseAbility();
	}

	// Token: 0x06001DF8 RID: 7672 RVA: 0x001A4CCC File Offset: 0x001A2ECC
	public void UpdateButtonText()
	{
		if (this.preset != null)
		{
			this.nameText.text = Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
			this.priceText.text = Strings.Get("ui.interface", "Select", Strings.Casing.asIs, false, false, false, null) + " " + CityControls.Instance.cityCurrency + this.price.ToString();
		}
		else if (this.furniture != null)
		{
			this.nameText.text = Strings.Get("evidence.names", this.furniture.name, Strings.Casing.asIs, false, false, false, null);
			if (this.worldFurnitureReference != null)
			{
				this.priceText.text = Strings.Get("ui.interface", "Place", Strings.Casing.asIs, false, false, false, null);
				if (this.sellButton != null)
				{
					this.sellText.text = Strings.Get("ui.interface", "Sell", Strings.Casing.asIs, false, false, false, null) + " " + CityControls.Instance.cityCurrency + Mathf.RoundToInt((float)this.furniture.cost * 0.5f).ToString();
					this.sellButton.gameObject.SetActive(true);
					this.sellButton.SetInteractable(true);
				}
			}
			else
			{
				this.priceText.text = Strings.Get("ui.interface", "Select", Strings.Casing.asIs, false, false, false, null) + " " + CityControls.Instance.cityCurrency + this.price.ToString();
				if (this.sellButton != null)
				{
					this.sellButton.gameObject.SetActive(false);
					this.sellButton.SetInteractable(false);
				}
				Toolbox.Instance.SetRectSize(this.placeButton.rect, 120f, 0f, 8f, 0f);
				this.placeButton.rect.sizeDelta = new Vector2(this.placeButton.rect.sizeDelta.x, 52f);
				this.placeButton.rect.anchoredPosition = new Vector2(this.placeButton.rect.anchoredPosition.x, 6f);
			}
		}
		this.UpdatePurchaseAbility();
	}

	// Token: 0x06001DF9 RID: 7673 RVA: 0x001A4F2C File Offset: 0x001A312C
	public void OnPlaceButton()
	{
		if (this.furnishingsController == null || (this.furnishingsController != null && this.furnishingsController.tabState == FurnishingsController.TabState.inShop))
		{
			if (this.price <= GameplayController.Instance.money)
			{
				if (this.decorController != null)
				{
					this.decorController.SetSelected(this.preset);
				}
				if (this.furnishingsController != null)
				{
					this.furnishingsController.SetSelected(this.furniture, this.worldFurnitureReference, false);
					return;
				}
			}
		}
		else if (this.furnishingsController != null)
		{
			bool flag = false;
			if (PlayerApartmentController.Instance.furnitureStorage.Contains(this.worldFurnitureReference))
			{
				flag = true;
			}
			this.furnishingsController.SetSelected(this.furniture, this.worldFurnitureReference, !flag);
		}
	}

	// Token: 0x06001DFA RID: 7674 RVA: 0x001A5002 File Offset: 0x001A3202
	public void OnStorageButton()
	{
		if (this.worldFurnitureReference != null)
		{
			PlayerApartmentController.Instance.MoveFurnitureToStorage(this.worldFurnitureReference);
		}
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x001A501C File Offset: 0x001A321C
	public void OnSellButton()
	{
		if (this.worldFurnitureReference != null)
		{
			PlayerApartmentController.Instance.SellFurniture(this.worldFurnitureReference);
		}
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x001A5038 File Offset: 0x001A3238
	public void UpdatePurchaseAbility()
	{
		if (this.furnishingsController == null || this.furnishingsController.tabState == FurnishingsController.TabState.inShop)
		{
			if (this.price > GameplayController.Instance.money)
			{
				this.placeButton.SetInteractable(false);
			}
			else
			{
				this.placeButton.SetInteractable(true);
			}
		}
		else
		{
			this.placeButton.SetInteractable(true);
		}
		if (this.storageButton != null && this.furnishingsController != null && (this.worldFurnitureReference == null || PlayerApartmentController.Instance.furnitureStorage.Contains(this.worldFurnitureReference)))
		{
			this.storageButton.gameObject.SetActive(false);
			this.storageButton.SetInteractable(false);
		}
	}

	// Token: 0x040027C5 RID: 10181
	[Header("Components")]
	public MaterialGroupPreset preset;

	// Token: 0x040027C6 RID: 10182
	public FurniturePreset furniture;

	// Token: 0x040027C7 RID: 10183
	public TextMeshProUGUI nameText;

	// Token: 0x040027C8 RID: 10184
	public TextMeshProUGUI priceText;

	// Token: 0x040027C9 RID: 10185
	public TextMeshProUGUI sellText;

	// Token: 0x040027CA RID: 10186
	public InfoWindow thisWindow;

	// Token: 0x040027CB RID: 10187
	public DecorController decorController;

	// Token: 0x040027CC RID: 10188
	public FurnishingsController furnishingsController;

	// Token: 0x040027CD RID: 10189
	public Image mainImage;

	// Token: 0x040027CE RID: 10190
	public FurnitureLocation worldFurnitureReference;

	// Token: 0x040027CF RID: 10191
	public ButtonController placeButton;

	// Token: 0x040027D0 RID: 10192
	public ButtonController storageButton;

	// Token: 0x040027D1 RID: 10193
	public ButtonController sellButton;

	// Token: 0x040027D2 RID: 10194
	public Image icon;

	// Token: 0x040027D3 RID: 10195
	[Space(7f)]
	public Sprite chairIcon;

	// Token: 0x040027D4 RID: 10196
	public Sprite tableIcon;

	// Token: 0x040027D5 RID: 10197
	public Sprite unitIcon;

	// Token: 0x040027D6 RID: 10198
	public Sprite electronicsIcon;

	// Token: 0x040027D7 RID: 10199
	public Sprite structuralIcon;

	// Token: 0x040027D8 RID: 10200
	public Sprite decorationIcon;

	// Token: 0x040027D9 RID: 10201
	public Sprite miscIcon;

	// Token: 0x040027DA RID: 10202
	[Header("State")]
	public int price;
}
