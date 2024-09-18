using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000555 RID: 1365
public class BuyInterfaceController : MonoBehaviour
{
	// Token: 0x06001DBD RID: 7613 RVA: 0x001A289D File Offset: 0x001A0A9D
	public void Setup(WindowContentController newWcc)
	{
		this.wcc = newWcc;
		this.UpdateElements();
	}

	// Token: 0x06001DBE RID: 7614 RVA: 0x001A28AC File Offset: 0x001A0AAC
	private void OnEnable()
	{
		if (this.sellMode)
		{
			this.UpdateElements();
		}
		this.UpdatePurchaseAbility();
	}

	// Token: 0x06001DBF RID: 7615 RVA: 0x001A28C4 File Offset: 0x001A0AC4
	public void UpdateElements()
	{
		if (this.wcc != null && this.wcc.window != null && this.wcc.window.passedInteractable != null)
		{
			while (this.spawned.Count > 0)
			{
				Object.Destroy(this.spawned[0].gameObject);
				this.spawned.RemoveAt(0);
			}
			if (!this.sellMode)
			{
				Dictionary<InteractablePreset, int> dictionary = new Dictionary<InteractablePreset, int>();
				if (this.wcc.window.passedInteractable.preset.menuOverride != null)
				{
					using (List<InteractablePreset>.Enumerator enumerator = this.wcc.window.passedInteractable.preset.menuOverride.itemsSold.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							InteractablePreset interactablePreset = enumerator.Current;
							int num = Mathf.RoundToInt(interactablePreset.value.y * Mathf.Max(1f - UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.priceModifier), 0.1f));
							dictionary.Add(interactablePreset, num);
						}
						goto IL_2BF;
					}
				}
				Human human = this.wcc.window.passedInteractable.isActor as Human;
				if (human != null)
				{
					this.company = human.job.employer;
					using (Dictionary<InteractablePreset, int>.Enumerator enumerator2 = this.company.prices.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<InteractablePreset, int> keyValuePair = enumerator2.Current;
							dictionary.Add(keyValuePair.Key, Mathf.RoundToInt((float)keyValuePair.Value * Mathf.Max(1f - UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.priceModifier), 0.1f)));
						}
						goto IL_2BF;
					}
				}
				if (this.wcc.window.passedInteractable.node.gameLocation.thisAsAddress != null && this.wcc.window.passedInteractable.node.gameLocation.thisAsAddress.company != null)
				{
					this.company = this.wcc.window.passedInteractable.node.gameLocation.thisAsAddress.company;
					foreach (KeyValuePair<InteractablePreset, int> keyValuePair2 in this.company.prices)
					{
						dictionary.Add(keyValuePair2.Key, Mathf.RoundToInt((float)keyValuePair2.Value * Mathf.Max(1f - UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.priceModifier), 0.1f)));
					}
				}
				IL_2BF:
				float num2 = 0f;
				List<MenuPreset> list = new List<MenuPreset>();
				if (this.wcc.window.passedInteractable.preset.menuOverride != null)
				{
					list.Add(this.wcc.window.passedInteractable.preset.menuOverride);
				}
				else if (this.company != null && this.company.preset != null)
				{
					list.AddRange(this.company.preset.menus);
				}
				foreach (MenuPreset menuPreset in list)
				{
					if (menuPreset.syncDiskSlots > 0)
					{
						List<SyncDiskPreset> list2 = new List<SyncDiskPreset>();
						List<SyncDiskPreset> list3 = new List<SyncDiskPreset>();
						using (List<SyncDiskPreset>.Enumerator enumerator4 = Toolbox.Instance.allSyncDisks.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								SyncDiskPreset syncDiskPreset = enumerator4.Current;
								if (!menuPreset.syncDisks.Contains(syncDiskPreset) && menuPreset.fromManufacturers.Contains(syncDiskPreset.manufacturer))
								{
									int num3 = (int)((SyncDiskPreset.Rarity)4 - syncDiskPreset.rarity);
									num3 *= num3;
									for (int i = 0; i < num3; i++)
									{
										list3.Add(syncDiskPreset);
									}
								}
							}
							goto IL_542;
						}
						goto IL_3FA;
						IL_542:
						if (list2.Count >= menuPreset.syncDiskSlots || list3.Count <= 0)
						{
							goto IL_55F;
						}
						IL_3FA:
						SyncDiskPreset newDisk = list3[Toolbox.Instance.GetPsuedoRandomNumber(0, list3.Count, CityData.Instance.seed + (SessionData.Instance.dateInt * SessionData.Instance.monthInt * SessionData.Instance.yearInt).ToString(), false)];
						list3.RemoveAll((SyncDiskPreset item) => item == newDisk);
						ShopSelectButtonController component = Object.Instantiate<GameObject>(this.elementPrefab, this.pageRect).GetComponent<ShopSelectButtonController>();
						this.spawned.Add(component);
						component.rect.anchoredPosition = new Vector2(0f, num2);
						component.Setup(newDisk.interactable, newDisk.price, this, this.wcc.window, newDisk, true, null, false);
						num2 -= component.rect.sizeDelta.y + 10f;
						this.pageRect.sizeDelta = new Vector2(this.pageRect.sizeDelta.x, -num2 + component.rect.sizeDelta.y + 60f);
						list2.Add(newDisk);
						goto IL_542;
					}
					IL_55F:
					foreach (SyncDiskPreset syncDiskPreset2 in menuPreset.syncDisks)
					{
						ShopSelectButtonController component2 = Object.Instantiate<GameObject>(this.elementPrefab, this.pageRect).GetComponent<ShopSelectButtonController>();
						this.spawned.Add(component2);
						component2.rect.anchoredPosition = new Vector2(0f, num2);
						component2.Setup(syncDiskPreset2.interactable, syncDiskPreset2.price, this, this.wcc.window, syncDiskPreset2, false, null, false);
						num2 -= component2.rect.sizeDelta.y + 10f;
						this.pageRect.sizeDelta = new Vector2(this.pageRect.sizeDelta.x, -num2 + component2.rect.sizeDelta.y + 60f);
					}
				}
				using (Dictionary<InteractablePreset, int>.Enumerator enumerator2 = dictionary.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<InteractablePreset, int> keyValuePair3 = enumerator2.Current;
						ShopSelectButtonController component3 = Object.Instantiate<GameObject>(this.elementPrefab, this.pageRect).GetComponent<ShopSelectButtonController>();
						this.spawned.Add(component3);
						component3.rect.anchoredPosition = new Vector2(0f, num2);
						component3.Setup(keyValuePair3.Key, keyValuePair3.Value, this, this.wcc.window, null, false, null, false);
						num2 -= component3.rect.sizeDelta.y + 10f;
						this.pageRect.sizeDelta = new Vector2(this.pageRect.sizeDelta.x, -num2 + component3.rect.sizeDelta.y + 60f);
					}
					goto IL_945;
				}
			}
			float num4 = 0f;
			Human human2 = this.wcc.window.passedInteractable.isActor as Human;
			if (human2 != null)
			{
				this.company = human2.job.employer;
			}
			else if (this.wcc.window.passedInteractable.node.gameLocation.thisAsAddress != null && this.wcc.window.passedInteractable.node.gameLocation.thisAsAddress.company != null)
			{
				this.company = this.wcc.window.passedInteractable.node.gameLocation.thisAsAddress.company;
			}
			float num5 = 0.5f;
			if (this.company != null)
			{
				num5 = this.company.preset.sellValueMultiplier;
			}
			foreach (FirstPersonItemController.InventorySlot inventorySlot in FirstPersonItemController.Instance.slots)
			{
				if (inventorySlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
				{
					Interactable interactable = inventorySlot.GetInteractable();
					if (interactable != null)
					{
						ShopSelectButtonController component4 = Object.Instantiate<GameObject>(this.elementPrefab, this.pageRect).GetComponent<ShopSelectButtonController>();
						this.spawned.Add(component4);
						component4.rect.anchoredPosition = new Vector2(0f, num4);
						component4.Setup(interactable.preset, Mathf.CeilToInt(interactable.val * num5), this, this.wcc.window, null, false, interactable, true);
						num4 -= component4.rect.sizeDelta.y + 10f;
						this.pageRect.sizeDelta = new Vector2(this.pageRect.sizeDelta.x, -num4 + component4.rect.sizeDelta.y + 60f);
					}
				}
			}
		}
		IL_945:
		this.UpdatePurchaseAbility();
	}

	// Token: 0x06001DC0 RID: 7616 RVA: 0x001A32E0 File Offset: 0x001A14E0
	public void UpdatePurchaseAbility()
	{
		foreach (ShopSelectButtonController shopSelectButtonController in this.spawned)
		{
			shopSelectButtonController.UpdatePurchaseAbility();
		}
	}

	// Token: 0x0400279A RID: 10138
	[Header("Settings")]
	public bool sellMode;

	// Token: 0x0400279B RID: 10139
	[Header("References")]
	public RectTransform pageRect;

	// Token: 0x0400279C RID: 10140
	public WindowContentController wcc;

	// Token: 0x0400279D RID: 10141
	public Company company;

	// Token: 0x0400279E RID: 10142
	[Header("Prefabs")]
	public GameObject elementPrefab;

	// Token: 0x0400279F RID: 10143
	private List<ShopSelectButtonController> spawned = new List<ShopSelectButtonController>();
}
