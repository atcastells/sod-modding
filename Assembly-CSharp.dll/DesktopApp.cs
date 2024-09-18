using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029F RID: 671
public class DesktopApp : CruncherAppContent
{
	// Token: 0x06000F00 RID: 3840 RVA: 0x000D68D1 File Offset: 0x000D4AD1
	public override void OnSetup()
	{
		base.OnSetup();
		this.UpdateIcons();
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x000D68E0 File Offset: 0x000D4AE0
	public void UpdateIcons()
	{
		foreach (DesktopIconController desktopIconController in this.spawnedIcons)
		{
			Object.Destroy(desktopIconController.gameObject);
		}
		this.spawnedIcons.Clear();
		Vector2 vector;
		vector..ctor(0.034f, -0.034f);
		foreach (CruncherAppPreset cruncherAppPreset in this.controller.ic.interactable.preset.additionalApps)
		{
			if (!cruncherAppPreset.alwaysInstalled)
			{
				bool flag = false;
				if (this.controller.ic.interactable.node.gameLocation.thisAsAddress != null)
				{
					if (cruncherAppPreset.companyOnly)
					{
						flag = (this.controller.ic.interactable.node.gameLocation.thisAsAddress.company != null && (!cruncherAppPreset.salesRecordsOnly || this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.preset.recordSalesData));
						if (!flag)
						{
							continue;
						}
					}
					if (cruncherAppPreset.onlyIfOwner)
					{
						if (this.controller.ic.interactable.node.gameLocation.thisAsAddress != null)
						{
							flag = this.controller.ic.interactable.node.gameLocation.thisAsAddress.owners.Contains(this.controller.loggedInAs);
						}
						if (!flag)
						{
							continue;
						}
					}
					if (cruncherAppPreset.onlyInAddresses.Count > 0)
					{
						flag = (this.controller.ic.interactable.node.gameLocation.thisAsAddress != null && cruncherAppPreset.onlyInAddresses.Contains(this.controller.ic.interactable.node.gameLocation.thisAsAddress.addressPreset));
						if (!flag)
						{
							continue;
						}
					}
					if (cruncherAppPreset.onlyIfResidential)
					{
						flag = false;
						if (this.controller.ic.interactable.node.gameLocation.building != null)
						{
							foreach (KeyValuePair<int, NewFloor> keyValuePair in this.controller.ic.interactable.node.gameLocation.building.floors)
							{
								if (keyValuePair.Value.addresses.Exists((NewAddress item) => item.residence != null))
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							continue;
						}
					}
				}
				foreach (CruncherAppPreset.AppAccess appAccess in cruncherAppPreset.installationConditions)
				{
					if (appAccess.rule == CharacterTrait.RuleType.ifAnyOfThese)
					{
						using (List<CharacterTrait>.Enumerator enumerator5 = appAccess.traitList.GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								CharacterTrait searchTrait = enumerator5.Current;
								if (this.controller.loggedInAs.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
								{
									flag = true;
									break;
								}
							}
							continue;
						}
					}
					if (appAccess.rule == CharacterTrait.RuleType.ifAllOfThese)
					{
						flag = true;
						using (List<CharacterTrait>.Enumerator enumerator5 = appAccess.traitList.GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								CharacterTrait searchTrait = enumerator5.Current;
								if (!this.controller.loggedInAs.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
								{
									flag = false;
									break;
								}
							}
							continue;
						}
					}
					if (appAccess.rule == CharacterTrait.RuleType.ifNoneOfThese)
					{
						flag = true;
						using (List<CharacterTrait>.Enumerator enumerator5 = appAccess.traitList.GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								CharacterTrait searchTrait = enumerator5.Current;
								if (this.controller.loggedInAs.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
								{
									flag = false;
									break;
								}
							}
							continue;
						}
					}
					if (appAccess.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
					{
						if (this.controller.loggedInAs.partner != null)
						{
							using (List<CharacterTrait>.Enumerator enumerator5 = appAccess.traitList.GetEnumerator())
							{
								while (enumerator5.MoveNext())
								{
									CharacterTrait searchTrait = enumerator5.Current;
									if (this.controller.loggedInAs.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag = true;
										break;
									}
								}
								continue;
							}
						}
						flag = false;
					}
				}
				if (cruncherAppPreset.onlyIfCorporateSabotageSkill && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.installMalware) <= 0f)
				{
					flag = false;
				}
				if (!flag)
				{
					continue;
				}
			}
			DesktopIconController component = Object.Instantiate<GameObject>(this.desktopIconPrefab, base.transform).GetComponent<DesktopIconController>();
			component.Setup(this, cruncherAppPreset);
			this.spawnedIcons.Add(component);
			component.rect.anchoredPosition = vector;
			vector.x += component.rect.sizeDelta.x + 0.034f;
			if (vector.x > 0.966f - component.rect.sizeDelta.x)
			{
				vector.y -= 0.034f + component.rect.sizeDelta.y;
				vector.x = 0.034f;
			}
		}
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x000D6FB4 File Offset: 0x000D51B4
	public void OnDesktopAppSelect(CruncherAppPreset newApp)
	{
		this.controller.SetComputerApp(newApp, false);
	}

	// Token: 0x0400122C RID: 4652
	public GameObject desktopIconPrefab;

	// Token: 0x0400122D RID: 4653
	public List<DesktopIconController> spawnedIcons = new List<DesktopIconController>();
}
