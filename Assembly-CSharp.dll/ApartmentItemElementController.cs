using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000544 RID: 1348
public class ApartmentItemElementController : MonoBehaviour
{
	// Token: 0x06001D61 RID: 7521 RVA: 0x0019F8DC File Offset: 0x0019DADC
	public void SetupItem(InteractablePreset newItemPreset, ApartmentItemsController newDecorController, InfoWindow newThisWindow, Interactable newWorldItemReference)
	{
		this.itemPreset = newItemPreset;
		this.thisWindow = newThisWindow;
		this.itemsController = newDecorController;
		this.worldItemReference = newWorldItemReference;
		if (this.storageButton != null)
		{
			this.storageButton.SetInteractable(false);
			this.storageButton.gameObject.SetActive(false);
		}
		if (this.itemPreset != null)
		{
			if (this.worldItemReference == null)
			{
				this.price = (int)this.itemPreset.value.y;
			}
			else
			{
				this.price = Mathf.RoundToInt(this.worldItemReference.val);
				if (this.storageButton != null)
				{
					this.storageButton.SetInteractable(true);
					this.storageButton.gameObject.SetActive(true);
				}
			}
			this.mainImage.sprite = this.itemPreset.staticImage;
			this.mainImage.color = Color.white;
		}
		this.VisualUpdate();
	}

	// Token: 0x06001D62 RID: 7522 RVA: 0x0019F9D0 File Offset: 0x0019DBD0
	public void VisualUpdate()
	{
		this.UpdateButtonText();
		this.UpdatePurchaseAbility();
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x0019F9E0 File Offset: 0x0019DBE0
	public void UpdateButtonText()
	{
		if (this.itemPreset != null)
		{
			if (this.worldItemReference != null)
			{
				if (this.itemPreset.isInventoryItem || this.itemPreset.isMoney)
				{
					this.priceText.text = Strings.Get("ui.interface", "Take", Strings.Casing.asIs, false, false, false, null);
				}
				else if (this.itemPreset.actionsPreset.Exists((InteractableActionsPreset item) => item.actions.Exists((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == "take (lockpick)")))
				{
					this.priceText.text = Strings.Get("ui.interface", "Take", Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					this.priceText.text = Strings.Get("ui.interface", "Place", Strings.Casing.asIs, false, false, false, null);
				}
				this.nameText.text = this.worldItemReference.GetName();
				if (this.sellButton != null)
				{
					if (!this.itemPreset.isMoney)
					{
						this.sellText.text = Strings.Get("ui.interface", "Sell", Strings.Casing.asIs, false, false, false, null) + " " + CityControls.Instance.cityCurrency + Mathf.RoundToInt(this.worldItemReference.val * 0.5f).ToString();
						this.sellButton.gameObject.SetActive(true);
						this.sellButton.SetInteractable(true);
					}
					else
					{
						this.sellText.text = Strings.Get("ui.interface", "Sell", Strings.Casing.asIs, false, false, false, null);
						this.sellButton.gameObject.SetActive(true);
						this.sellButton.SetInteractable(false);
					}
				}
			}
			else
			{
				this.nameText.text = Strings.Get("evidence.names", this.itemPreset.name, Strings.Casing.asIs, false, false, false, null);
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

	// Token: 0x06001D64 RID: 7524 RVA: 0x0019FCB8 File Offset: 0x0019DEB8
	public void OnPlaceButton()
	{
		if (!(this.itemsController != null) || this.itemsController.tabState != FurnishingsController.TabState.inShop)
		{
			if (this.itemsController != null && this.worldItemReference != null)
			{
				if (PlayerApartmentController.Instance.furniturePlacementMode)
				{
					PlayerApartmentController.Instance.SetFurniturePlacementMode(false, null, null, false, false);
				}
				PlayerApartmentController.Instance.RemoveItemFromStorage(this.worldItemReference);
				if (this.worldItemReference.preset.isMoney)
				{
					ActionController.Instance.TakeMoney(this.worldItemReference, Player.Instance.currentNode, Player.Instance);
				}
				else if (this.worldItemReference.preset.isInventoryItem)
				{
					FirstPersonItemController.Instance.PickUpItem(this.worldItemReference, true, false, true, true, false);
				}
				else
				{
					this.itemsController.PlaceObject(this.worldItemReference);
				}
				this.itemsController.UpdateListDisplay();
				this.UpdatePurchaseAbility();
			}
			return;
		}
		if (this.price <= GameplayController.Instance.money)
		{
			PopupMessageController.Instance.PopupMessage("purchaseask", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.PurchaseCancel;
			PopupMessageController.Instance.OnRightButton += this.PurchaseConfirm;
			return;
		}
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_money", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.money, null, null, null);
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x0019FE68 File Offset: 0x0019E068
	public void PurchaseConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.PurchaseCancel;
		PopupMessageController.Instance.OnRightButton -= this.PurchaseConfirm;
		if (PlayerApartmentController.Instance.furniturePlacementMode)
		{
			PlayerApartmentController.Instance.SetFurniturePlacementMode(false, null, null, false, false);
		}
		this.worldItemReference = InteractableCreator.Instance.CreateWorldInteractable(this.itemPreset, Player.Instance, Player.Instance, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
		if (this.worldItemReference != null)
		{
			if (this.worldItemReference.preset.isInventoryItem)
			{
				if (FirstPersonItemController.Instance.PickUpItem(this.worldItemReference, true, false, true, true, false))
				{
					Game.Log("Successfully created newObject " + this.worldItemReference.name + " id " + this.worldItemReference.id.ToString(), 2);
					AudioController.Instance.Play2DSound(AudioControls.Instance.purchaseItem, null, 1f);
					this.worldItemReference.MarkAsTrash(true, false, 0f);
					GameplayController.Instance.AddMoney(-this.price, true, "purchase");
				}
				else
				{
					Game.LogError("Decor: Unable to create world item " + this.itemPreset.name, 2);
					this.worldItemReference.Delete();
					this.worldItemReference = null;
				}
			}
			else
			{
				Game.Log("Decor: Successfully created newObject " + this.worldItemReference.name + " id " + this.worldItemReference.id.ToString(), 2);
				AudioController.Instance.Play2DSound(AudioControls.Instance.purchaseItem, null, 1f);
				GameplayController.Instance.AddMoney(-this.price, true, "purchase");
				this.itemsController.PlaceObject(this.worldItemReference);
			}
		}
		else
		{
			Game.LogError("Decor: Unable to create world item " + this.itemPreset.name, 2);
		}
		this.UpdatePurchaseAbility();
	}

	// Token: 0x06001D66 RID: 7526 RVA: 0x001A0089 File Offset: 0x0019E289
	public void PurchaseCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.PurchaseCancel;
		PopupMessageController.Instance.OnRightButton -= this.PurchaseConfirm;
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x001A00B7 File Offset: 0x0019E2B7
	public void OnStorageButton()
	{
		if (this.worldItemReference != null)
		{
			PlayerApartmentController.Instance.MoveItemToStorage(this.worldItemReference);
		}
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x001A00D1 File Offset: 0x0019E2D1
	public void OnSellButton()
	{
		if (this.worldItemReference != null)
		{
			PlayerApartmentController.Instance.SellItem(this.worldItemReference);
		}
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x001A00EC File Offset: 0x0019E2EC
	public void UpdatePurchaseAbility()
	{
		if (this.itemsController == null || this.itemsController.tabState == FurnishingsController.TabState.inShop)
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
		if (this.storageButton != null && this.itemsController != null && (this.worldItemReference == null || PlayerApartmentController.Instance.itemStorage.Contains(this.worldItemReference)))
		{
			this.storageButton.gameObject.SetActive(false);
			this.storageButton.SetInteractable(false);
		}
	}

	// Token: 0x04002719 RID: 10009
	[Header("Components")]
	public InteractablePreset itemPreset;

	// Token: 0x0400271A RID: 10010
	public TextMeshProUGUI nameText;

	// Token: 0x0400271B RID: 10011
	public TextMeshProUGUI priceText;

	// Token: 0x0400271C RID: 10012
	public TextMeshProUGUI sellText;

	// Token: 0x0400271D RID: 10013
	public InfoWindow thisWindow;

	// Token: 0x0400271E RID: 10014
	public DecorController decorController;

	// Token: 0x0400271F RID: 10015
	public ApartmentItemsController itemsController;

	// Token: 0x04002720 RID: 10016
	public Image mainImage;

	// Token: 0x04002721 RID: 10017
	[NonSerialized]
	public Interactable worldItemReference;

	// Token: 0x04002722 RID: 10018
	public ButtonController placeButton;

	// Token: 0x04002723 RID: 10019
	public ButtonController storageButton;

	// Token: 0x04002724 RID: 10020
	public ButtonController sellButton;

	// Token: 0x04002725 RID: 10021
	public Image icon;

	// Token: 0x04002726 RID: 10022
	[Header("State")]
	public int price;
}
