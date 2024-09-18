using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000591 RID: 1425
public class ShopSelectButtonController : ButtonController
{
	// Token: 0x06001F17 RID: 7959 RVA: 0x001AEB68 File Offset: 0x001ACD68
	public void Setup(InteractablePreset newPreset, int newPrice, BuyInterfaceController newBuyController, InfoWindow newThisWindow, SyncDiskPreset newSyncDisk = null, bool newTemp = false, Interactable newSellInteractable = null, bool newSellMode = false)
	{
		this.preset = newPreset;
		this.price = newPrice;
		this.thisWindow = newThisWindow;
		this.buyController = newBuyController;
		this.syncDisk = newSyncDisk;
		this.todayOnly = newTemp;
		this.sellInteractable = newSellInteractable;
		this.sellMode = newSellMode;
		base.SetupReferences();
		this.UpdateButtonText();
		this.UpdateTooltip();
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x001AEBC4 File Offset: 0x001ACDC4
	public override void UpdateButtonText()
	{
		if (this.syncDisk != null && !this.sellMode)
		{
			if (this.todayOnly)
			{
				this.specialText.gameObject.SetActive(true);
				this.specialText.text = Strings.Get("ui.interface", "Special Offer: Today Only", Strings.Casing.asIs, false, false, false, null) + "\n" + Strings.Get("ui.interface", "Install at Sync Clinic", Strings.Casing.asIs, false, false, false, null);
			}
			this.text.text = "#" + this.syncDisk.syncDiskNumber.ToString() + " " + Strings.Get("evidence.syncdisks", this.syncDisk.name, Strings.Casing.asIs, false, false, false, null);
		}
		else if (this.sellMode && this.sellInteractable != null && (this.buyController.company == null || !this.buyController.company.preset.enableSellingOfIllegalItems) && StatusController.Instance.activeFineRecords.Exists((StatusController.FineRecord item) => item.crime == StatusController.CrimeType.theft && item.objectID == this.sellInteractable.id))
		{
			this.specialText.gameObject.SetActive(true);
			this.specialText.text = Strings.Get("ui.interface", "You cannot sell stolen items here", Strings.Casing.asIs, false, false, false, null);
		}
		else
		{
			this.specialText.gameObject.SetActive(false);
			if (this.sellInteractable != null)
			{
				this.text.text = this.sellInteractable.GetName();
			}
			else if (this.preset != null)
			{
				this.text.text = Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
			}
		}
		this.priceText.text = CityControls.Instance.cityCurrency + this.price.ToString();
		if (this.preset != null)
		{
			this.icon.sprite = this.preset.iconOverride;
			this.mainImage.sprite = this.preset.staticImage;
		}
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x001AEDD4 File Offset: 0x001ACFD4
	public void UpdateTooltip()
	{
		if (this.syncDisk != null && !this.sellMode)
		{
			this.tooltip.mainText = "#" + this.syncDisk.syncDiskNumber.ToString() + " " + Strings.Get("evidence.syncdisks", this.syncDisk.name, Strings.Casing.asIs, false, false, false, null);
			string text = string.Empty;
			if (this.syncDisk.mainEffect1 != SyncDiskPreset.Effect.none)
			{
				text += Strings.ComposeText(Strings.Get("evidence.syncdisks", this.syncDisk.mainEffect1Description, Strings.Casing.asIs, false, false, false, null), this.syncDisk, Strings.LinkSetting.automatic, null, new int[2], false);
			}
			if (this.syncDisk.mainEffect2 != SyncDiskPreset.Effect.none)
			{
				string[] array = new string[5];
				array[0] = text;
				array[1] = "\n";
				array[2] = Strings.Get("evidence.syncdisks", "OR", Strings.Casing.asIs, false, false, false, null);
				array[3] = "\n";
				int num = 4;
				string input = Strings.Get("evidence.syncdisks", this.syncDisk.mainEffect2Description, Strings.Casing.asIs, false, false, false, null);
				object baseObject = this.syncDisk;
				Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic;
				List<Evidence.DataKey> evidenceKeys = null;
				int[] array2 = new int[2];
				array2[0] = 1;
				array[num] = Strings.ComposeText(input, baseObject, linkSetting, evidenceKeys, array2, false);
				text = string.Concat(array);
			}
			if (this.syncDisk.mainEffect3 != SyncDiskPreset.Effect.none)
			{
				string[] array3 = new string[5];
				array3[0] = text;
				array3[1] = "\n";
				array3[2] = Strings.Get("evidence.syncdisks", "OR", Strings.Casing.asIs, false, false, false, null);
				array3[3] = "\n";
				int num2 = 4;
				string input2 = Strings.Get("evidence.syncdisks", this.syncDisk.mainEffect3Description, Strings.Casing.asIs, false, false, false, null);
				object baseObject2 = this.syncDisk;
				Strings.LinkSetting linkSetting2 = Strings.LinkSetting.automatic;
				List<Evidence.DataKey> evidenceKeys2 = null;
				int[] array4 = new int[2];
				array4[0] = 2;
				array3[num2] = Strings.ComposeText(input2, baseObject2, linkSetting2, evidenceKeys2, array4, false);
				text = string.Concat(array3);
			}
			this.tooltip.detailText = text;
			return;
		}
		this.tooltip.mainText = Strings.GetTextForComponent(this.preset.summaryMessageSource, null, null, null, "\n", false, null, Strings.LinkSetting.automatic, null);
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x001AEFB0 File Offset: 0x001AD1B0
	public override void OnLeftClick()
	{
		Game.Log("Player: Purchase/sell " + this.preset.name, 2);
		if (!this.sellMode)
		{
			if (this.price <= GameplayController.Instance.money)
			{
				if (!FirstPersonItemController.Instance.IsSlotAvailable())
				{
					PopupMessageController.Instance.PopupMessage("dropitem", true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				}
				else
				{
					PopupMessageController.Instance.PopupMessage("purchaseask", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
					PopupMessageController.Instance.OnRightButton += this.PurchaseExecute;
					PopupMessageController.Instance.OnLeftButton += this.Cancel;
				}
			}
		}
		else if (this.sellInteractable != null)
		{
			PopupMessageController.Instance.PopupMessage("sellask", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnRightButton += this.SellExecute;
			PopupMessageController.Instance.OnLeftButton += this.Cancel;
		}
		base.OnLeftClick();
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x001AF138 File Offset: 0x001AD338
	public void PurchaseExecute()
	{
		PopupMessageController.Instance.OnRightButton -= this.PurchaseExecute;
		PopupMessageController.Instance.OnLeftButton -= this.Cancel;
		Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(this.preset, Player.Instance, Player.Instance, null, this.thisWindow.passedInteractable.wPos + new Vector3(0f, 3.5f, 0f), this.thisWindow.passedInteractable.wEuler, null, this.syncDisk, "");
		if (interactable != null)
		{
			Game.Log("Successfully created newObject " + interactable.name + " id " + interactable.id.ToString(), 2);
			interactable.SetSpawnPositionRelevent(false);
			if (Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.company != null && Player.Instance.currentGameLocation.thisAsAddress.company.preset.enableLoiteringBehaviour)
			{
				Player.Instance.currentGameLocation.LoiteringPurchase();
			}
			if (!FirstPersonItemController.Instance.PickUpItem(interactable, true, false, true, true, false))
			{
				Game.LogError("Unable to pickup item " + interactable.name, 2);
				interactable.Delete();
				return;
			}
			AudioController.Instance.Play2DSound(AudioControls.Instance.purchaseItem, null, 1f);
			interactable.MarkAsTrash(true, false, 0f);
			GameplayController.Instance.AddMoney(-this.price, true, "purchase");
			this.buyController.UpdatePurchaseAbility();
			if (this.thisWindow != null && this.thisWindow.passedInteractable != null && this.thisWindow.passedInteractable.preset.menuOverride != null && this.thisWindow.passedInteractable.preset.menuOverride.purchaseAudio != null)
			{
				AudioController.Instance.PlayWorldOneShot(this.thisWindow.passedInteractable.preset.menuOverride.purchaseAudio, Player.Instance, this.thisWindow.passedInteractable.node, this.thisWindow.passedInteractable.wPos, this.thisWindow.passedInteractable, null, 1f, null, false, null, false);
				return;
			}
		}
		else
		{
			Game.LogError("Unable to create world item " + this.preset.name, 2);
		}
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x001AF3D0 File Offset: 0x001AD5D0
	public void SellExecute()
	{
		PopupMessageController.Instance.OnRightButton -= this.SellExecute;
		PopupMessageController.Instance.OnLeftButton -= this.Cancel;
		if (this.sellInteractable != null)
		{
			if (FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && item.interactableID == this.sellInteractable.id) != null)
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.purchaseItem, null, 1f);
				GameplayController.Instance.AddMoney(this.price, true, "sell");
				this.buyController.UpdatePurchaseAbility();
				this.sellInteractable.SafeDelete(true);
				this.buyController.UpdateElements();
				if (Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.company != null && Player.Instance.currentGameLocation.thisAsAddress.company.preset.enableLoiteringBehaviour)
				{
					Player.Instance.currentGameLocation.LoiteringPurchase();
					return;
				}
			}
		}
		else
		{
			Game.LogError("Unable to find sell interactable", 2);
		}
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x001AF504 File Offset: 0x001AD704
	public void Cancel()
	{
		PopupMessageController.Instance.OnRightButton -= this.PurchaseExecute;
		PopupMessageController.Instance.OnRightButton -= this.SellExecute;
		PopupMessageController.Instance.OnLeftButton -= this.Cancel;
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x001AF554 File Offset: 0x001AD754
	public void UpdatePurchaseAbility()
	{
		if (!this.sellMode && this.price > GameplayController.Instance.money)
		{
			this.button.interactable = false;
			this.priceText.text = "<color=#FD313F>" + CityControls.Instance.cityCurrency + this.price.ToString();
			return;
		}
		if (!this.sellMode || this.sellInteractable == null || (this.buyController.company != null && this.buyController.company.preset.enableSellingOfIllegalItems))
		{
			this.button.interactable = true;
			this.priceText.text = CityControls.Instance.cityCurrency + this.price.ToString();
			return;
		}
		if (StatusController.Instance.activeFineRecords.Exists((StatusController.FineRecord item) => item.crime == StatusController.CrimeType.theft && item.objectID == this.sellInteractable.id))
		{
			this.button.interactable = false;
			this.priceText.text = CityControls.Instance.cityCurrency + this.price.ToString();
			return;
		}
		this.button.interactable = true;
		this.priceText.text = CityControls.Instance.cityCurrency + this.price.ToString();
	}

	// Token: 0x040028E4 RID: 10468
	public InteractablePreset preset;

	// Token: 0x040028E5 RID: 10469
	public Interactable sellInteractable;

	// Token: 0x040028E6 RID: 10470
	public SyncDiskPreset syncDisk;

	// Token: 0x040028E7 RID: 10471
	public int price;

	// Token: 0x040028E8 RID: 10472
	public TextMeshProUGUI priceText;

	// Token: 0x040028E9 RID: 10473
	public TextMeshProUGUI specialText;

	// Token: 0x040028EA RID: 10474
	public InfoWindow thisWindow;

	// Token: 0x040028EB RID: 10475
	public BuyInterfaceController buyController;

	// Token: 0x040028EC RID: 10476
	public Image mainImage;

	// Token: 0x040028ED RID: 10477
	public bool todayOnly;

	// Token: 0x040028EE RID: 10478
	public bool sellMode;
}
