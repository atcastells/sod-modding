using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000546 RID: 1350
public class ApartmentSalesController : MonoBehaviour
{
	// Token: 0x06001D6F RID: 7535 RVA: 0x001A01F8 File Offset: 0x0019E3F8
	private void OnEnable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		if (this.windowContent == null)
		{
			this.windowContent = base.gameObject.GetComponentInParent<WindowContentController>();
		}
		this.parentWindow.OnWindowRefresh += this.UpdateDetails;
		this.UpdateDetails();
	}

	// Token: 0x06001D70 RID: 7536 RVA: 0x001A0260 File Offset: 0x0019E460
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.OnWindowRefresh -= this.UpdateDetails;
	}

	// Token: 0x06001D71 RID: 7537 RVA: 0x001A0298 File Offset: 0x0019E498
	public void UpdateDetails()
	{
		this.purchaseButton.SetInteractable(false);
		if (this.parentWindow.passedInteractable.forSale != null && this.parentWindow.passedInteractable.forSale.thisAsAddress != null)
		{
			this.img.texture = this.parentWindow.passedInteractable.forSale.evidenceEntry.GetPhoto(this.parentWindow.evidenceKeys);
			this.img.color = Color.white;
			this.dataText.text = Strings.GetTextForComponent("600d4a18-7306-4871-a68e-e7764ae62f81", this.parentWindow.passedInteractable, null, null, "\n", false, null, Strings.LinkSetting.automatic, this.parentWindow.evidenceKeys);
			int num = Mathf.RoundToInt(this.parentWindow.passedInteractable.forSale.thisAsAddress.normalizedLandValue * 2f);
			if (num <= 0)
			{
				this.descriptionText.text = Strings.GetTextForComponent("3651e904-22e5-4093-9660-e59140ea6176", this.parentWindow.passedInteractable, null, null, "\n", false, null, Strings.LinkSetting.automatic, this.parentWindow.evidenceKeys);
			}
			else if (num == 1)
			{
				this.descriptionText.text = Strings.GetTextForComponent("f1da9ff4-f0f8-42cb-b295-04d7ac7d353d", this.parentWindow.passedInteractable, null, null, "\n", false, null, Strings.LinkSetting.automatic, this.parentWindow.evidenceKeys);
			}
			else if (num >= 2)
			{
				this.descriptionText.text = Strings.GetTextForComponent("2a8c5e45-1b74-46f6-813d-6cae5e65a396", this.parentWindow.passedInteractable, null, null, "\n", false, null, Strings.LinkSetting.automatic, this.parentWindow.evidenceKeys);
			}
			int price = this.parentWindow.passedInteractable.forSale.GetPrice(false);
			this.purchaseButton.text.text = Strings.Get("evidence.generic", "Purchase", Strings.Casing.asIs, false, false, false, null) + " " + CityControls.Instance.cityCurrency + price.ToString();
			if (GameplayController.Instance.money >= price)
			{
				this.purchaseButton.SetInteractable(true);
			}
		}
	}

	// Token: 0x06001D72 RID: 7538 RVA: 0x001A04A8 File Offset: 0x0019E6A8
	public void OnPurchaseButton()
	{
		if (GameplayController.Instance.money >= this.parentWindow.passedInteractable.forSale.GetPrice(false))
		{
			int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.allowApartmentPurchases));
			if (!Game.Instance.allowSocialCreditPerks)
			{
				num = 1;
			}
			if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null && !Game.Instance.sandboxMode)
			{
				ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
				if (chapterIntro != null && !chapterIntro.completed)
				{
					num = -1;
				}
			}
			if (num <= -1)
			{
				PopupMessageController.Instance.PopupMessage("FinishCampaignBeforeApartment", true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				return;
			}
			if (num == 0)
			{
				PopupMessageController.Instance.PopupMessage("LevelUpBeforeApartment", true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				return;
			}
			if (num > 0)
			{
				PopupMessageController.Instance.PopupMessage("BuyApartment", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				PopupMessageController.Instance.OnRightButton += this.ConfirmApartmentPurchase;
				PopupMessageController.Instance.OnLeftButton += this.CancelApartmentPurchase;
			}
		}
	}

	// Token: 0x06001D73 RID: 7539 RVA: 0x001A0648 File Offset: 0x0019E848
	public void ConfirmApartmentPurchase()
	{
		PopupMessageController.Instance.OnRightButton -= this.ConfirmApartmentPurchase;
		PopupMessageController.Instance.OnLeftButton -= this.CancelApartmentPurchase;
		int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.allowApartmentPurchases));
		if (!Game.Instance.allowSocialCreditPerks)
		{
			num = 1;
		}
		if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null && !Game.Instance.sandboxMode)
		{
			ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
			if (chapterIntro != null && !chapterIntro.completed)
			{
				num = 0;
			}
		}
		if (num > 0)
		{
			GameplayController.Instance.AddMoney(-this.parentWindow.passedInteractable.forSale.GetPrice(false), true, "Property purchase");
			PlayerApartmentController.Instance.BuyNewResidence(this.parentWindow.passedInteractable.forSale.residence, false);
			SessionData.Instance.PauseGame(true, false, true);
			List<Evidence.DataKey> list = new List<Evidence.DataKey>();
			list.Add(Evidence.DataKey.location);
			list.Add(Evidence.DataKey.photo);
			list.Add(Evidence.DataKey.name);
			list.Add(Evidence.DataKey.purpose);
			list.Add(Evidence.DataKey.blueprints);
			InterfaceController.Instance.SpawnWindow(this.parentWindow.passedInteractable.forSale.evidenceEntry, Evidence.DataKey.name, list, "", false, true, default(Vector2), null, null, null, true);
			this.parentWindow.CloseWindow(false);
			InterfaceController.Instance.ApartmentPurchaseDisplay();
			if (AchievementsController.Instance != null)
			{
				AchievementsController.Instance.UnlockAchievement("A Place to Hang Your Hat", "new_apartment");
				return;
			}
		}
		else
		{
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.interface", "You need more social credit before purchasing an apartment...", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.star, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
	}

	// Token: 0x06001D74 RID: 7540 RVA: 0x001A081D File Offset: 0x0019EA1D
	public void CancelApartmentPurchase()
	{
		PopupMessageController.Instance.OnRightButton -= this.ConfirmApartmentPurchase;
		PopupMessageController.Instance.OnLeftButton -= this.CancelApartmentPurchase;
	}

	// Token: 0x0400272A RID: 10026
	public WindowContentController windowContent;

	// Token: 0x0400272B RID: 10027
	public InfoWindow parentWindow;

	// Token: 0x0400272C RID: 10028
	public TextMeshProUGUI dataText;

	// Token: 0x0400272D RID: 10029
	public TextMeshProUGUI descriptionText;

	// Token: 0x0400272E RID: 10030
	public ButtonController purchaseButton;

	// Token: 0x0400272F RID: 10031
	public RawImage img;
}
