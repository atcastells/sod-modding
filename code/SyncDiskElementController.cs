using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000602 RID: 1538
public class SyncDiskElementController : MonoBehaviour
{
	// Token: 0x060021E7 RID: 8679 RVA: 0x001CB970 File Offset: 0x001C9B70
	public void Setup(UpgradesController.Upgrades newUpgrade)
	{
		this.upgrade = newUpgrade;
		this.preset = this.upgrade.GetPreset();
		if (this.preset.mainEffect1 == SyncDiskPreset.Effect.none)
		{
			Object.Destroy(this.option1Button.gameObject);
		}
		else
		{
			this.option1Button.tooltip.mainDictionaryKey = this.preset.mainEffect1Name;
			this.option1Button.tooltip.detailText = Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.mainEffect1Description, Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[2], false);
			this.option1Icon.sprite = this.preset.mainEffect1Icon;
		}
		if (this.preset.mainEffect2 == SyncDiskPreset.Effect.none)
		{
			Object.Destroy(this.option2Button.gameObject);
		}
		else
		{
			this.option2Button.tooltip.mainDictionaryKey = this.preset.mainEffect2Name;
			TooltipController tooltip = this.option2Button.tooltip;
			string input = Strings.Get("evidence.syncdisks", this.preset.mainEffect2Description, Strings.Casing.asIs, false, false, false, null);
			object baseObject = this.preset;
			Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic;
			List<Evidence.DataKey> evidenceKeys = null;
			int[] array = new int[2];
			array[0] = 1;
			tooltip.detailText = Strings.ComposeText(input, baseObject, linkSetting, evidenceKeys, array, false);
			this.option2Icon.sprite = this.preset.mainEffect2Icon;
		}
		if (this.preset.mainEffect3 == SyncDiskPreset.Effect.none)
		{
			Object.Destroy(this.option3Button.gameObject);
		}
		else
		{
			this.option3Button.tooltip.mainDictionaryKey = this.preset.mainEffect3Name;
			TooltipController tooltip2 = this.option3Button.tooltip;
			string input2 = Strings.Get("evidence.syncdisks", this.preset.mainEffect3Description, Strings.Casing.asIs, false, false, false, null);
			object baseObject2 = this.preset;
			Strings.LinkSetting linkSetting2 = Strings.LinkSetting.automatic;
			List<Evidence.DataKey> evidenceKeys2 = null;
			int[] array2 = new int[2];
			array2[0] = 2;
			tooltip2.detailText = Strings.ComposeText(input2, baseObject2, linkSetting2, evidenceKeys2, array2, false);
			this.option3Icon.sprite = this.preset.mainEffect3Icon;
		}
		if (this.preset.sideEffect == SyncDiskPreset.Effect.none)
		{
			Object.Destroy(this.sideEffectButton.gameObject);
		}
		else
		{
			this.sideEffectButton.tooltip.detailText = Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.sideEffectDescription, Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, null, false);
		}
		int num = Mathf.Max(new int[]
		{
			this.preset.option1UpgradeEffects.Count,
			this.preset.option2UpgradeEffects.Count,
			this.preset.option3UpgradeEffects.Count
		});
		if (num <= 0)
		{
			Object.Destroy(this.upgradePip1.gameObject);
			Object.Destroy(this.upgradePip2.gameObject);
			Object.Destroy(this.upgradePip3.gameObject);
			Object.Destroy(this.upgradeButton.gameObject);
		}
		if (num <= 1)
		{
			if (this.upgradePip2 != null)
			{
				Object.Destroy(this.upgradePip2.gameObject);
			}
			if (this.upgradePip3 != null)
			{
				Object.Destroy(this.upgradePip3.gameObject);
			}
		}
		if (num <= 2 && this.upgradePip3 != null)
		{
			Object.Destroy(this.upgradePip3.gameObject);
		}
		this.numberText.text = "#" + this.preset.syncDiskNumber.ToString() + "/" + Toolbox.Instance.allSyncDisks.Count.ToString();
		this.titleText.text = Strings.Get("evidence.syncdisks", this.preset.name, Strings.Casing.asIs, false, false, false, null);
		if (this.preset.manufacturer == SyncDiskPreset.Manufacturer.StarchKola)
		{
			this.manufacturerLogo.sprite = InterfaceControls.Instance.starchLogo;
		}
		else if (this.preset.manufacturer == SyncDiskPreset.Manufacturer.CandorNews)
		{
			this.manufacturerLogo.sprite = InterfaceControls.Instance.candorLogo;
		}
		else if (this.preset.manufacturer == SyncDiskPreset.Manufacturer.ElGen)
		{
			this.manufacturerLogo.sprite = InterfaceControls.Instance.elGenLogo;
		}
		else if (this.preset.manufacturer == SyncDiskPreset.Manufacturer.Kaizen)
		{
			this.manufacturerLogo.sprite = InterfaceControls.Instance.KaizenLogo;
		}
		else if (this.preset.manufacturer == SyncDiskPreset.Manufacturer.KensingtonIndigo)
		{
			this.manufacturerLogo.sprite = InterfaceControls.Instance.kensingtonLogo;
		}
		else
		{
			this.manufacturerLogo.sprite = InterfaceControls.Instance.blackMarketLogo;
		}
		this.installAllowed = UpgradesController.Instance.installedAllowed;
		this.VisualUpdate();
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x001CBDD7 File Offset: 0x001C9FD7
	public void SetInstallAllowed(bool val)
	{
		this.installAllowed = val;
		this.VisualUpdate();
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x001CBDE8 File Offset: 0x001C9FE8
	public void VisualUpdate()
	{
		if (this.upgrade.state == UpgradesController.SyncDiskState.notInstalled)
		{
			string text = string.Empty;
			this.titleText.text = Strings.Get("evidence.syncdisks", this.preset.name, Strings.Casing.asIs, false, false, false, null) + "*";
			if (this.preset.mainEffect1 != SyncDiskPreset.Effect.none)
			{
				text += Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.mainEffect1Description, Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[2], false);
			}
			if (this.preset.mainEffect2 != SyncDiskPreset.Effect.none)
			{
				string[] array = new string[5];
				array[0] = text;
				array[1] = "\n";
				array[2] = Strings.Get("evidence.syncdisks", "OR", Strings.Casing.asIs, false, false, false, null);
				array[3] = "\n";
				int num = 4;
				string input = Strings.Get("evidence.syncdisks", this.preset.mainEffect2Description, Strings.Casing.asIs, false, false, false, null);
				object baseObject = this.preset;
				Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic;
				List<Evidence.DataKey> evidenceKeys = null;
				int[] array2 = new int[2];
				array2[0] = 1;
				array[num] = Strings.ComposeText(input, baseObject, linkSetting, evidenceKeys, array2, false);
				text = string.Concat(array);
			}
			if (this.preset.mainEffect3 != SyncDiskPreset.Effect.none)
			{
				string[] array3 = new string[5];
				array3[0] = text;
				array3[1] = "\n";
				array3[2] = Strings.Get("evidence.syncdisks", "OR", Strings.Casing.asIs, false, false, false, null);
				array3[3] = "\n";
				int num2 = 4;
				string input2 = Strings.Get("evidence.syncdisks", this.preset.mainEffect3Description, Strings.Casing.asIs, false, false, false, null);
				object baseObject2 = this.preset;
				Strings.LinkSetting linkSetting2 = Strings.LinkSetting.automatic;
				List<Evidence.DataKey> evidenceKeys2 = null;
				int[] array4 = new int[2];
				array4[0] = 2;
				array3[num2] = Strings.ComposeText(input2, baseObject2, linkSetting2, evidenceKeys2, array4, false);
				text = string.Concat(array3);
			}
			this.descriptionText.text = text;
			if (this.selectedOption <= 0)
			{
				if (this.option1Button != null)
				{
					this.option1Button.icon.gameObject.SetActive(false);
					this.option1Button.juice.Pulsate(this.installAllowed, false);
				}
				if (this.option2Button != null)
				{
					this.option2Button.icon.gameObject.SetActive(false);
					this.option2Button.juice.Pulsate(this.installAllowed, false);
				}
				if (this.option3Button != null)
				{
					this.option3Button.icon.gameObject.SetActive(false);
					this.option3Button.juice.Pulsate(this.installAllowed, false);
				}
			}
			else if (this.selectedOption == 1)
			{
				if (this.option1Button != null)
				{
					this.option1Button.icon.gameObject.SetActive(true);
				}
				if (this.option2Button != null)
				{
					this.option2Button.icon.gameObject.SetActive(false);
				}
				if (this.option3Button != null)
				{
					this.option3Button.icon.gameObject.SetActive(false);
				}
			}
			else if (this.selectedOption == 2)
			{
				if (this.option1Button != null)
				{
					this.option1Button.icon.gameObject.SetActive(false);
				}
				if (this.option2Button != null)
				{
					this.option2Button.icon.gameObject.SetActive(true);
				}
				if (this.option3Button != null)
				{
					this.option3Button.icon.gameObject.SetActive(false);
				}
			}
			else
			{
				if (this.option1Button != null)
				{
					this.option1Button.icon.gameObject.SetActive(false);
				}
				if (this.option2Button != null)
				{
					this.option2Button.icon.gameObject.SetActive(false);
				}
				if (this.option3Button != null)
				{
					this.option3Button.icon.gameObject.SetActive(true);
				}
			}
			if (this.option1Button != null)
			{
				this.option1Button.SetInteractable(this.installAllowed);
			}
			if (this.option2Button != null)
			{
				this.option2Button.SetInteractable(this.installAllowed);
			}
			if (this.option3Button != null)
			{
				this.option3Button.SetInteractable(this.installAllowed);
			}
			this.uninstallButton.text.text = Strings.Get("ui.interface", "Install", Strings.Casing.asIs, false, false, false, null);
			if (this.installAllowed && this.selectedOption >= 1)
			{
				this.uninstallButton.SetInteractable(true);
			}
			else
			{
				this.uninstallButton.SetInteractable(false);
			}
			if (this.upgradeButton != null)
			{
				this.upgradeButton.SetInteractable(false);
				this.upgradeButton.juice.Pulsate(false, false);
				this.upgradeButton.tooltip.detailText = string.Empty;
			}
			if (this.upgradePip1 != null)
			{
				this.upgradePip1.icon.sprite = this.upgradeEmptySprite;
				this.upgradePip1.tooltip.detailText = string.Empty;
				if (this.preset.option1UpgradeNameReferences.Count > 0)
				{
					TooltipController tooltip = this.upgradePip1.tooltip;
					tooltip.detailText = string.Concat(new string[]
					{
						tooltip.detailText,
						"<u>",
						Strings.Get("evidence.syncdisks", this.preset.mainEffect1Name, Strings.Casing.asIs, false, false, false, null),
						":</u>\n<i>",
						Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option1UpgradeNameReferences[0], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[2], false),
						"</i>"
					});
				}
				if (this.preset.option2UpgradeNameReferences.Count > 0)
				{
					TooltipController tooltip = this.upgradePip1.tooltip;
					TooltipController tooltipController = tooltip;
					string[] array5 = new string[6];
					array5[0] = tooltip.detailText;
					array5[1] = "\n\n<u>";
					array5[2] = Strings.Get("evidence.syncdisks", this.preset.mainEffect2Name, Strings.Casing.asIs, false, false, false, null);
					array5[3] = ":</u>\n<i>";
					int num3 = 4;
					string input3 = Strings.Get("evidence.syncdisks", this.preset.option2UpgradeNameReferences[0], Strings.Casing.asIs, false, false, false, null);
					object baseObject3 = this.preset;
					Strings.LinkSetting linkSetting3 = Strings.LinkSetting.automatic;
					List<Evidence.DataKey> evidenceKeys3 = null;
					int[] array6 = new int[2];
					array6[0] = 1;
					array5[num3] = Strings.ComposeText(input3, baseObject3, linkSetting3, evidenceKeys3, array6, false);
					array5[5] = "</i>";
					tooltipController.detailText = string.Concat(array5);
				}
				if (this.preset.option3UpgradeNameReferences.Count > 0)
				{
					TooltipController tooltip = this.upgradePip1.tooltip;
					TooltipController tooltipController2 = tooltip;
					string[] array7 = new string[6];
					array7[0] = tooltip.detailText;
					array7[1] = "\n\n<u>";
					array7[2] = Strings.Get("evidence.syncdisks", this.preset.mainEffect3Name, Strings.Casing.asIs, false, false, false, null);
					array7[3] = ":</u>\n<i>";
					int num4 = 4;
					string input4 = Strings.Get("evidence.syncdisks", this.preset.option3UpgradeNameReferences[0], Strings.Casing.asIs, false, false, false, null);
					object baseObject4 = this.preset;
					Strings.LinkSetting linkSetting4 = Strings.LinkSetting.automatic;
					List<Evidence.DataKey> evidenceKeys4 = null;
					int[] array8 = new int[2];
					array8[0] = 2;
					array7[num4] = Strings.ComposeText(input4, baseObject4, linkSetting4, evidenceKeys4, array8, false);
					array7[5] = "</i>";
					tooltipController2.detailText = string.Concat(array7);
				}
			}
			if (this.upgradePip2 != null)
			{
				this.upgradePip2.icon.sprite = this.upgradeEmptySprite;
				this.upgradePip2.tooltip.detailText = string.Empty;
				if (this.preset.option1UpgradeNameReferences.Count > 1)
				{
					TooltipController tooltip = this.upgradePip2.tooltip;
					tooltip.detailText = string.Concat(new string[]
					{
						tooltip.detailText,
						"<u>",
						Strings.Get("evidence.syncdisks", this.preset.mainEffect1Name, Strings.Casing.asIs, false, false, false, null),
						":</u>\n<i>",
						Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option1UpgradeNameReferences[1], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							default(int),
							1
						}, false),
						"</i>"
					});
				}
				if (this.preset.option2UpgradeNameReferences.Count > 1)
				{
					TooltipController tooltip = this.upgradePip2.tooltip;
					tooltip.detailText = string.Concat(new string[]
					{
						tooltip.detailText,
						"\n\n<u>",
						Strings.Get("evidence.syncdisks", this.preset.mainEffect2Name, Strings.Casing.asIs, false, false, false, null),
						":</u>\n<i>",
						Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option2UpgradeNameReferences[1], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							1,
							1
						}, false),
						"</i>"
					});
				}
				if (this.preset.option3UpgradeNameReferences.Count > 1)
				{
					TooltipController tooltip = this.upgradePip2.tooltip;
					tooltip.detailText = string.Concat(new string[]
					{
						tooltip.detailText,
						"\n\n<u>",
						Strings.Get("evidence.syncdisks", this.preset.mainEffect3Name, Strings.Casing.asIs, false, false, false, null),
						":</u>\n<i>",
						Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option3UpgradeNameReferences[1], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							2,
							1
						}, false),
						"</i>"
					});
				}
			}
			if (this.upgradePip3 != null)
			{
				this.upgradePip3.icon.sprite = this.upgradeEmptySprite;
				this.upgradePip3.tooltip.detailText = string.Empty;
				if (this.preset.option1UpgradeNameReferences.Count > 2)
				{
					TooltipController tooltip = this.upgradePip3.tooltip;
					tooltip.detailText = string.Concat(new string[]
					{
						tooltip.detailText,
						"<u>",
						Strings.Get("evidence.syncdisks", this.preset.mainEffect1Name, Strings.Casing.asIs, false, false, false, null),
						":</u>\n<i>",
						Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option1UpgradeNameReferences[2], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							default(int),
							2
						}, false),
						"</i>"
					});
				}
				if (this.preset.option2UpgradeNameReferences.Count > 2)
				{
					TooltipController tooltip = this.upgradePip3.tooltip;
					tooltip.detailText = string.Concat(new string[]
					{
						tooltip.detailText,
						"\n\n<u>",
						Strings.Get("evidence.syncdisks", this.preset.mainEffect2Name, Strings.Casing.asIs, false, false, false, null),
						":</u>\n<i>",
						Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option2UpgradeNameReferences[2], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							1,
							2
						}, false),
						"</i>"
					});
				}
				if (this.preset.option3UpgradeNameReferences.Count > 2)
				{
					TooltipController tooltip = this.upgradePip3.tooltip;
					tooltip.detailText = string.Concat(new string[]
					{
						tooltip.detailText,
						"\n\n<u>",
						Strings.Get("evidence.syncdisks", this.preset.mainEffect3Name, Strings.Casing.asIs, false, false, false, null),
						":</u>\n<i>",
						Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option3UpgradeNameReferences[2], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							2,
							2
						}, false),
						"</i>"
					});
					return;
				}
			}
		}
		else
		{
			this.titleText.text = Strings.Get("evidence.syncdisks", this.preset.name, Strings.Casing.asIs, false, false, false, null);
			if (this.option1Button != null)
			{
				this.option1Button.juice.Pulsate(false, false);
			}
			if (this.option2Button != null)
			{
				this.option2Button.juice.Pulsate(false, false);
			}
			if (this.option3Button != null)
			{
				this.option3Button.juice.Pulsate(false, false);
			}
			if (this.option1Button != null)
			{
				this.option1Button.icon.gameObject.SetActive(false);
			}
			if (this.option2Button != null)
			{
				this.option2Button.icon.gameObject.SetActive(false);
			}
			if (this.option3Button != null)
			{
				this.option3Button.icon.gameObject.SetActive(false);
			}
			int num5 = 0;
			if (this.upgrade.state == UpgradesController.SyncDiskState.option1)
			{
				this.descriptionText.text = Strings.Get("evidence.syncdisks", this.preset.mainEffect1Name, Strings.Casing.asIs, false, false, false, null) + ": " + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.mainEffect1Description, Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[2], false);
				if (this.option1Button != null)
				{
					this.option1Button.SetInteractable(true);
					this.option1Button.icon.gameObject.SetActive(true);
				}
				if (this.option2Button != null)
				{
					this.option2Button.SetInteractable(false);
					this.option2Icon.color = Color.gray;
				}
				if (this.option3Button != null)
				{
					this.option3Button.SetInteractable(false);
					this.option3Icon.color = Color.gray;
				}
				num5 = this.preset.option1UpgradeEffects.Count;
				try
				{
					if (this.upgrade.level < this.preset.option1UpgradeNameReferences.Count)
					{
						this.upgradeButton.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option1UpgradeNameReferences[this.upgrade.level], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							default(int),
							this.upgrade.level
						}, false);
					}
					else
					{
						this.upgradeButton.tooltip.detailText = string.Empty;
					}
					if (num5 >= 1 & this.upgradePip1 != null)
					{
						this.upgradePip1.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option1UpgradeNameReferences[0], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[2], false);
					}
					if (num5 >= 2 & this.upgradePip2 != null)
					{
						this.upgradePip2.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option1UpgradeNameReferences[1], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							default(int),
							1
						}, false);
					}
					if (num5 >= 3 & this.upgradePip3 != null)
					{
						this.upgradePip3.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option1UpgradeNameReferences[2], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							default(int),
							2
						}, false);
					}
					goto IL_1561;
				}
				catch
				{
					Game.LogError("Error setting upgrade levels for " + this.preset.name, 2);
					goto IL_1561;
				}
			}
			if (this.upgrade.state == UpgradesController.SyncDiskState.option2)
			{
				TMP_Text tmp_Text = this.descriptionText;
				string text2 = Strings.Get("evidence.syncdisks", this.preset.mainEffect2Name, Strings.Casing.asIs, false, false, false, null);
				string text3 = ": ";
				string input5 = Strings.Get("evidence.syncdisks", this.preset.mainEffect2Description, Strings.Casing.asIs, false, false, false, null);
				object baseObject5 = this.preset;
				Strings.LinkSetting linkSetting5 = Strings.LinkSetting.automatic;
				List<Evidence.DataKey> evidenceKeys5 = null;
				int[] array9 = new int[2];
				array9[0] = 1;
				tmp_Text.text = text2 + text3 + Strings.ComposeText(input5, baseObject5, linkSetting5, evidenceKeys5, array9, false);
				if (this.option1Button != null)
				{
					this.option1Button.SetInteractable(false);
					this.option1Icon.color = Color.gray;
				}
				if (this.option2Button != null)
				{
					this.option2Button.SetInteractable(true);
					this.option2Button.icon.gameObject.SetActive(true);
				}
				if (this.option3Button != null)
				{
					this.option3Button.SetInteractable(false);
					this.option3Icon.color = Color.gray;
				}
				num5 = this.preset.option2UpgradeEffects.Count;
				try
				{
					if (this.upgrade.level < this.preset.option2UpgradeNameReferences.Count)
					{
						this.upgradeButton.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option2UpgradeNameReferences[this.upgrade.level], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							1,
							this.upgrade.level
						}, false);
					}
					else
					{
						this.upgradeButton.tooltip.detailText = string.Empty;
					}
					if (num5 >= 1 & this.upgradePip1 != null)
					{
						TooltipController tooltip2 = this.upgradePip1.tooltip;
						string text4 = "<i>";
						string input6 = Strings.Get("evidence.syncdisks", this.preset.option2UpgradeNameReferences[0], Strings.Casing.asIs, false, false, false, null);
						object baseObject6 = this.preset;
						Strings.LinkSetting linkSetting6 = Strings.LinkSetting.automatic;
						List<Evidence.DataKey> evidenceKeys6 = null;
						int[] array10 = new int[2];
						array10[0] = 1;
						tooltip2.detailText = text4 + Strings.ComposeText(input6, baseObject6, linkSetting6, evidenceKeys6, array10, false);
					}
					if (num5 >= 2 & this.upgradePip2 != null)
					{
						this.upgradePip2.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option2UpgradeNameReferences[1], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							1,
							1
						}, false);
					}
					if (num5 >= 3 & this.upgradePip3 != null)
					{
						this.upgradePip3.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option2UpgradeNameReferences[2], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							1,
							2
						}, false);
					}
					goto IL_1561;
				}
				catch
				{
					Game.LogError("Error setting upgrade levels for " + this.preset.name, 2);
					goto IL_1561;
				}
			}
			if (this.upgrade.state == UpgradesController.SyncDiskState.option3)
			{
				TMP_Text tmp_Text2 = this.descriptionText;
				string text5 = Strings.Get("evidence.syncdisks", this.preset.mainEffect3Name, Strings.Casing.asIs, false, false, false, null);
				string text6 = ": ";
				string input7 = Strings.Get("evidence.syncdisks", this.preset.mainEffect3Description, Strings.Casing.asIs, false, false, false, null);
				object baseObject7 = this.preset;
				Strings.LinkSetting linkSetting7 = Strings.LinkSetting.automatic;
				List<Evidence.DataKey> evidenceKeys7 = null;
				int[] array11 = new int[2];
				array11[0] = 2;
				tmp_Text2.text = text5 + text6 + Strings.ComposeText(input7, baseObject7, linkSetting7, evidenceKeys7, array11, false);
				if (this.option1Button != null)
				{
					this.option1Button.SetInteractable(false);
					this.option1Icon.color = Color.gray;
				}
				if (this.option2Button != null)
				{
					this.option2Button.SetInteractable(false);
					this.option2Icon.color = Color.gray;
				}
				if (this.option3Button != null)
				{
					this.option3Button.SetInteractable(true);
					this.option3Button.icon.gameObject.SetActive(true);
				}
				num5 = this.preset.option3UpgradeEffects.Count;
				try
				{
					if (this.upgrade.level < this.preset.option3UpgradeNameReferences.Count)
					{
						this.upgradeButton.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option3UpgradeNameReferences[this.upgrade.level], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							2,
							this.upgrade.level
						}, false);
					}
					else
					{
						this.upgradeButton.tooltip.detailText = string.Empty;
					}
					if (num5 >= 1 & this.upgradePip1 != null)
					{
						TooltipController tooltip3 = this.upgradePip1.tooltip;
						string text7 = "<i>";
						string input8 = Strings.Get("evidence.syncdisks", this.preset.option3UpgradeNameReferences[0], Strings.Casing.asIs, false, false, false, null);
						object baseObject8 = this.preset;
						Strings.LinkSetting linkSetting8 = Strings.LinkSetting.automatic;
						List<Evidence.DataKey> evidenceKeys8 = null;
						int[] array12 = new int[2];
						array12[0] = 2;
						tooltip3.detailText = text7 + Strings.ComposeText(input8, baseObject8, linkSetting8, evidenceKeys8, array12, false);
					}
					if (num5 >= 2 & this.upgradePip2 != null)
					{
						this.upgradePip2.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option3UpgradeNameReferences[1], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							2,
							1
						}, false);
					}
					if (num5 >= 3 & this.upgradePip3 != null)
					{
						this.upgradePip3.tooltip.detailText = "<i>" + Strings.ComposeText(Strings.Get("evidence.syncdisks", this.preset.option3UpgradeNameReferences[2], Strings.Casing.asIs, false, false, false, null), this.preset, Strings.LinkSetting.automatic, null, new int[]
						{
							2,
							2
						}, false);
					}
				}
				catch
				{
					Game.LogError("Error setting upgrade levels for " + this.preset.name, 2);
				}
			}
			IL_1561:
			this.uninstallButton.text.text = Strings.Get("ui.interface", "Uninstall", Strings.Casing.asIs, false, false, false, null);
			if (this.upgrade.uninstallCost > 0)
			{
				TextMeshProUGUI text8 = this.uninstallButton.text;
				text8.text = string.Concat(new string[]
				{
					text8.text,
					" [",
					CityControls.Instance.cityCurrency,
					this.upgrade.uninstallCost.ToString(),
					"]"
				});
			}
			if (this.installAllowed && (this.upgrade.uninstallCost <= 0 || GameplayController.Instance.money >= this.upgrade.uninstallCost))
			{
				this.uninstallButton.SetInteractable(true);
			}
			else
			{
				this.uninstallButton.SetInteractable(false);
			}
			if (this.upgradeButton != null)
			{
				if (this.installAllowed && UpgradesController.Instance.upgradeVials.Count > 0 && this.upgrade.level < num5)
				{
					this.upgradeButton.SetInteractable(true);
					this.upgradeButton.juice.Pulsate(true, false);
				}
				else
				{
					this.upgradeButton.SetInteractable(false);
					this.upgradeButton.juice.Pulsate(false, false);
				}
			}
			if (this.upgrade.level <= 0)
			{
				if (this.upgradePip1 != null)
				{
					this.upgradePip1.icon.sprite = this.upgradeEmptySprite;
				}
				if (this.upgradePip2 != null)
				{
					this.upgradePip2.icon.sprite = this.upgradeEmptySprite;
				}
				if (this.upgradePip3 != null)
				{
					this.upgradePip3.icon.sprite = this.upgradeEmptySprite;
				}
			}
			else if (this.upgrade.level == 1)
			{
				if (this.upgradePip1 != null)
				{
					this.upgradePip1.icon.sprite = this.upgradeEnabledSprite;
				}
				if (this.upgradePip2 != null)
				{
					this.upgradePip2.icon.sprite = this.upgradeEmptySprite;
				}
				if (this.upgradePip3 != null)
				{
					this.upgradePip3.icon.sprite = this.upgradeEmptySprite;
				}
			}
			else if (this.upgrade.level == 2)
			{
				if (this.upgradePip1 != null)
				{
					this.upgradePip1.icon.sprite = this.upgradeEnabledSprite;
				}
				if (this.upgradePip2 != null)
				{
					this.upgradePip2.icon.sprite = this.upgradeEnabledSprite;
				}
				if (this.upgradePip3 != null)
				{
					this.upgradePip3.icon.sprite = this.upgradeEmptySprite;
				}
			}
			else
			{
				if (this.upgradePip1 != null)
				{
					this.upgradePip1.icon.sprite = this.upgradeEnabledSprite;
				}
				if (this.upgradePip2 != null)
				{
					this.upgradePip2.icon.sprite = this.upgradeEnabledSprite;
				}
				if (this.upgradePip3 != null)
				{
					this.upgradePip3.icon.sprite = this.upgradeEnabledSprite;
				}
			}
			if (this.sideEffectButton != null)
			{
				if (this.upgrade.GetUpgradeEffects().Exists((UpgradeEffectController.AppliedEffect item) => item.effect == SyncDiskPreset.Effect.removeSideEffect))
				{
					Object.Destroy(this.sideEffectButton.gameObject);
				}
			}
		}
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x001CD724 File Offset: 0x001CB924
	public void SelectOptionButton(int val)
	{
		this.selectedOption = val + 1;
		this.VisualUpdate();
	}

	// Token: 0x060021EB RID: 8683 RVA: 0x001CD738 File Offset: 0x001CB938
	public void InstallButton()
	{
		if (this.upgrade.state == UpgradesController.SyncDiskState.notInstalled)
		{
			if (UpgradesController.Instance.upgrades.Exists((UpgradesController.Upgrades item) => item.upgrade == this.upgrade.upgrade))
			{
				PopupMessageController.Instance.PopupMessage("OnlyOneSyncDisk", true, false, "Cancel", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				PopupMessageController.Instance.OnLeftButton += this.PopupCancel;
				return;
			}
			if (this.selectedOption > 0)
			{
				PopupMessageController.Instance.PopupMessage("InstallSyncDisk", true, true, "Cancel", "Install", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				PopupMessageController.Instance.OnRightButton += this.InstallPromptSuccess;
				PopupMessageController.Instance.OnLeftButton += this.PopupCancel;
				return;
			}
		}
		else if (this.upgrade.uninstallCost == 0 || (this.upgrade.uninstallCost > 0 && GameplayController.Instance.money >= this.upgrade.uninstallCost))
		{
			PopupMessageController.Instance.PopupMessage("UninstallSyncDisk", true, true, "Cancel", "Uninstall", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnRightButton += this.UninstallPromptSuccess;
			PopupMessageController.Instance.OnLeftButton += this.PopupCancel;
		}
	}

	// Token: 0x060021EC RID: 8684 RVA: 0x001CD8EC File Offset: 0x001CBAEC
	public void PopupCancel()
	{
		PopupMessageController.Instance.OnRightButton -= this.InstallPromptSuccess;
		PopupMessageController.Instance.OnRightButton -= this.UninstallPromptSuccess;
		PopupMessageController.Instance.OnRightButton -= this.UpgradePromptSuccess;
		PopupMessageController.Instance.OnLeftButton -= this.PopupCancel;
	}

	// Token: 0x060021ED RID: 8685 RVA: 0x001CD954 File Offset: 0x001CBB54
	public void InstallPromptSuccess()
	{
		PopupMessageController.Instance.OnRightButton -= this.InstallPromptSuccess;
		PopupMessageController.Instance.OnLeftButton -= this.PopupCancel;
		UpgradesController.Instance.InstallSyncDisk(this.upgrade, this.selectedOption);
		this.VisualUpdate();
	}

	// Token: 0x060021EE RID: 8686 RVA: 0x001CD9AC File Offset: 0x001CBBAC
	public void UninstallPromptSuccess()
	{
		PopupMessageController.Instance.OnRightButton -= this.UninstallPromptSuccess;
		PopupMessageController.Instance.OnLeftButton -= this.PopupCancel;
		if (this.upgrade.uninstallCost > 0 && GameplayController.Instance.money >= this.upgrade.uninstallCost)
		{
			GameplayController.Instance.AddMoney(-this.upgrade.uninstallCost, true, "Uninstall sync disk");
			UpgradesController.Instance.UninstallSyncDisk(this.upgrade);
			this.VisualUpdate();
			return;
		}
		UpgradesController.Instance.UninstallSyncDisk(this.upgrade);
		this.VisualUpdate();
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x001CDA54 File Offset: 0x001CBC54
	public void UpgradeButton()
	{
		PopupMessageController.Instance.PopupMessage("UpgradeSyncDisk", true, true, "Cancel", "Upgrade", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnRightButton += this.UpgradePromptSuccess;
		PopupMessageController.Instance.OnLeftButton += this.PopupCancel;
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x001CDAD0 File Offset: 0x001CBCD0
	public void UpgradePromptSuccess()
	{
		PopupMessageController.Instance.OnRightButton -= this.UpgradePromptSuccess;
		PopupMessageController.Instance.OnLeftButton -= this.PopupCancel;
		UpgradesController.Instance.UpgradeSyncDisk(this.upgrade);
		this.VisualUpdate();
	}

	// Token: 0x04002C6F RID: 11375
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002C70 RID: 11376
	[Space(7f)]
	public TextMeshProUGUI titleText;

	// Token: 0x04002C71 RID: 11377
	public TextMeshProUGUI numberText;

	// Token: 0x04002C72 RID: 11378
	public TextMeshProUGUI descriptionText;

	// Token: 0x04002C73 RID: 11379
	[Space(7f)]
	public ButtonController option1Button;

	// Token: 0x04002C74 RID: 11380
	public ButtonController option2Button;

	// Token: 0x04002C75 RID: 11381
	public ButtonController option3Button;

	// Token: 0x04002C76 RID: 11382
	public ButtonController upgradeButton;

	// Token: 0x04002C77 RID: 11383
	public ButtonController sideEffectButton;

	// Token: 0x04002C78 RID: 11384
	public ButtonController uninstallButton;

	// Token: 0x04002C79 RID: 11385
	public Image option1Icon;

	// Token: 0x04002C7A RID: 11386
	public Image option2Icon;

	// Token: 0x04002C7B RID: 11387
	public Image option3Icon;

	// Token: 0x04002C7C RID: 11388
	[Space(7f)]
	public ButtonController upgradePip1;

	// Token: 0x04002C7D RID: 11389
	public ButtonController upgradePip2;

	// Token: 0x04002C7E RID: 11390
	public ButtonController upgradePip3;

	// Token: 0x04002C7F RID: 11391
	[Space(7f)]
	public Image manufacturerLogo;

	// Token: 0x04002C80 RID: 11392
	[Header("Settings")]
	public Sprite upgradeEmptySprite;

	// Token: 0x04002C81 RID: 11393
	public Sprite upgradeEnabledSprite;

	// Token: 0x04002C82 RID: 11394
	[Header("State")]
	public UpgradesController.Upgrades upgrade;

	// Token: 0x04002C83 RID: 11395
	public SyncDiskPreset preset;

	// Token: 0x04002C84 RID: 11396
	public int selectedOption;

	// Token: 0x04002C85 RID: 11397
	public bool installAllowed;
}
