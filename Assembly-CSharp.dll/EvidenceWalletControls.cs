using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x02000569 RID: 1385
public class EvidenceWalletControls : MonoBehaviour
{
	// Token: 0x06001E25 RID: 7717 RVA: 0x001A6694 File Offset: 0x001A4894
	private void OnEnable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.OnWindowRefresh += this.CheckEnabled;
		this.CheckEnabled();
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x001A66D2 File Offset: 0x001A48D2
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x001A670C File Offset: 0x001A490C
	public void CheckEnabled()
	{
		Game.Log("Interface: Check for location key in parent window...", 2);
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		EvidenceWalletControls.allItems = Enumerable.ToList<EvidenceWalletControls>(base.gameObject.transform.parent.parent.GetComponentsInChildren<EvidenceWalletControls>());
		Game.Log("Found " + EvidenceWalletControls.allItems.Count.ToString() + " items", 2);
		this.VisualUpdate(EvidenceWalletControls.allItems.IndexOf(this));
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x001A67A0 File Offset: 0x001A49A0
	public void VisualUpdate(int walletIndex)
	{
		if (this.parentWindow == null || this.parentWindow.passedInteractable == null || this.parentWindow.passedInteractable.belongsTo == null)
		{
			return;
		}
		Game.Log("Set wallet index: " + walletIndex.ToString(), 2);
		if (walletIndex > -1 && walletIndex < this.parentWindow.passedInteractable.belongsTo.walletItems.Count)
		{
			this.itemRef = this.parentWindow.passedInteractable.belongsTo.walletItems[walletIndex];
			if (this.itemRef != null)
			{
				if (this.itemRef.itemType == Human.WalletItemType.money && this.itemRef.money > 0)
				{
					this.buttonText.text = string.Concat(new string[]
					{
						Strings.Get("ui.interface", "Take Money", Strings.Casing.asIs, false, false, false, null),
						" [",
						CityControls.Instance.cityCurrency,
						Mathf.RoundToInt((float)this.itemRef.money).ToString(),
						"]"
					});
					this.button.background.sprite = this.moneySprite;
				}
				else if (this.itemRef.itemType == Human.WalletItemType.key)
				{
					this.buttonText.text = Strings.Get("ui.interface", "Take Key", Strings.Casing.asIs, false, false, false, null);
					this.button.background.sprite = this.keySprite;
				}
				else if (this.itemRef.itemType == Human.WalletItemType.evidence)
				{
					this.button.background.sprite = this.cardSprite;
					this.buttonText.text = string.Empty;
				}
				else
				{
					this.buttonText.text = string.Empty;
				}
			}
		}
		else
		{
			Game.Log("Unable to get wallet index of " + walletIndex.ToString(), 2);
			this.itemRef = null;
		}
		if (this.itemRef == null || this.itemRef.itemType == Human.WalletItemType.nothing || (this.itemRef.itemType == Human.WalletItemType.money && this.itemRef.money <= 0))
		{
			this.button.SetInteractable(false);
			base.gameObject.SetActive(false);
			this.buttonText.text = string.Empty;
			return;
		}
		this.button.SetInteractable(true);
		base.gameObject.SetActive(true);
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x001A6A0C File Offset: 0x001A4C0C
	public void OnButtonPress()
	{
		if (this.itemRef != null && this.parentWindow != null && this.parentWindow.passedInteractable != null && this.parentWindow.isWorldInteraction)
		{
			Game.Log("On wallet button: " + this.itemRef.itemType.ToString(), 2);
			if (this.itemRef.itemType != Human.WalletItemType.nothing && this.parentWindow.passedInteractable.inInventory != null && this.parentWindow.passedInteractable.inInventory.ai != null && Toolbox.Instance.Rand(0f, 1f, false) <= GameplayControls.Instance.stealTriggerChance)
			{
				this.parentWindow.passedInteractable.inInventory.ai.SetPersue(Player.Instance, true, 1, true, CitizenControls.Instance.punchedResponseRange);
			}
			if (this.itemRef.itemType == Human.WalletItemType.evidence)
			{
				MetaObject metaObject = CityData.Instance.FindMetaObject(this.itemRef.meta);
				if (metaObject != null)
				{
					InterfaceController.Instance.SpawnWindow(metaObject.GetEvidence(false, default(Vector3Int)), Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
				}
			}
			else if (this.itemRef.itemType == Human.WalletItemType.key)
			{
				foreach (NewRoom newRoom in this.parentWindow.passedInteractable.belongsTo.home.rooms)
				{
					foreach (NewNode.NodeAccess nodeAccess in newRoom.entrances)
					{
						if (nodeAccess.wall != null && nodeAccess.wall.door != null && nodeAccess.wall.door.preset.lockType == DoorPreset.LockType.key)
						{
							Player.Instance.AddToKeyring(nodeAccess.wall.door, true);
						}
					}
				}
				StatusController.Instance.AddFineRecord(Player.Instance.currentNode.gameLocation.thisAsAddress, null, StatusController.CrimeType.theft, true, 100, false);
				this.itemRef.itemType = Human.WalletItemType.nothing;
			}
			else if (this.itemRef.itemType == Human.WalletItemType.money)
			{
				GameplayController.Instance.AddMoney(Mathf.RoundToInt((float)this.itemRef.money), true, "stolen from wallet");
				StatusController.Instance.AddFineRecord(Player.Instance.currentNode.gameLocation.thisAsAddress, null, StatusController.CrimeType.theft, true, this.itemRef.money * 10, false);
				this.itemRef.money = 0;
				this.itemRef.itemType = Human.WalletItemType.nothing;
			}
			this.CheckEnabled();
		}
	}

	// Token: 0x04002800 RID: 10240
	public InfoWindow parentWindow;

	// Token: 0x04002801 RID: 10241
	public Sprite moneySprite;

	// Token: 0x04002802 RID: 10242
	public Sprite cardSprite;

	// Token: 0x04002803 RID: 10243
	public Sprite keySprite;

	// Token: 0x04002804 RID: 10244
	public static List<EvidenceWalletControls> allItems = new List<EvidenceWalletControls>();

	// Token: 0x04002805 RID: 10245
	[Header("Wallet")]
	public ButtonController button;

	// Token: 0x04002806 RID: 10246
	public TextMeshProUGUI buttonText;

	// Token: 0x04002807 RID: 10247
	[NonSerialized]
	public Human.WalletItem itemRef;
}
