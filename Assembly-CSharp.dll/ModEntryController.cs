using System;
using System.IO;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000333 RID: 819
public class ModEntryController : MonoBehaviour
{
	// Token: 0x0600126D RID: 4717 RVA: 0x0010562B File Offset: 0x0010382B
	public void Setup(ModSettingsData newMod)
	{
		this.mod = newMod;
		this.StateRefresh();
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x0010563A File Offset: 0x0010383A
	private void Update()
	{
		this.updateTimer += Time.deltaTime;
		if (this.updateTimer >= 1f)
		{
			this.updateTimer = 0f;
			this.StateRefresh();
		}
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x0010566C File Offset: 0x0010386C
	public void StateRefresh()
	{
		this.nameText.text = string.Concat(new string[]
		{
			this.mod.modData.modProfile.name,
			"<size=80%><alpha=#CC> — ",
			this.mod.modData.modProfile.creator.username,
			"\n<size=60%><alpha=#AA>",
			this.mod.modData.modProfile.summary
		});
		if (this.mod.modData.enabled)
		{
			this.enableDisableButton.text.text = Strings.Get("ui.interface", "Disable", Strings.Casing.asIs, false, false, false, null);
			this.iconImg.sprite = this.enabledSprite;
		}
		else
		{
			this.enableDisableButton.text.text = Strings.Get("ui.interface", "Enable", Strings.Casing.asIs, false, false, false, null);
			this.iconImg.sprite = this.disabledSprite;
		}
		if (this.mod.modData.updatePending)
		{
			this.iconImg.sprite = this.updatePendingSprite;
		}
		this.enableDisableButton.SetInteractable(true);
		this.moveDownButton.SetInteractable(true);
		this.moveUpButton.SetInteractable(true);
		this.versionText.text = this.mod.modData.version;
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x001057CC File Offset: 0x001039CC
	public void OnEnableDisableButton()
	{
		if (this.mod.modData.enabled)
		{
			Game.Log("Menu: Disabling mod " + this.mod.modData.modProfile.name, 2);
			ModIOUnity.DisableMod(this.mod.modData.modProfile.id);
		}
		else
		{
			Game.Log("Menu: Enabling mod " + this.mod.modData.modProfile.name, 2);
			ModIOUnity.EnableMod(this.mod.modData.modProfile.id);
		}
		ModController.Instance.UpdateModEntries();
		ModController.Instance.SetModConfigChanged(true);
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x00105884 File Offset: 0x00103A84
	public void OnMoveDownButton()
	{
		int num = ModLoader.Instance.sortedModsList.IndexOf(this.mod);
		if (num > -1)
		{
			ModSettingsData modSettingsData = ModLoader.Instance.sortedModsList[num];
			int num2 = modSettingsData.loadOrderValue;
			int loadOrderValue = modSettingsData.loadOrderValue;
			if (num + 1 < ModLoader.Instance.sortedModsList.Count)
			{
				int i = num + 1;
				while (i < ModLoader.Instance.sortedModsList.Count)
				{
					ModSettingsData modSettingsData2 = ModLoader.Instance.sortedModsList[i];
					if (i != num + 1)
					{
						goto IL_105;
					}
					num2 = modSettingsData2.loadOrderValue + 1;
					if (modSettingsData.loadOrderValue != num2)
					{
						modSettingsData.loadOrderValue = num2;
						Game.Log("Menu: Writing new load order for " + modSettingsData.modData.modProfile.name + ": " + modSettingsData.loadOrderValue.ToString(), 2);
						string text = modSettingsData.modData.directory + "/ModSettings.txt";
						string text2 = JsonUtility.ToJson(modSettingsData, true);
						using (StreamWriter streamWriter = File.CreateText(text))
						{
							streamWriter.Write(text2);
							goto IL_189;
						}
						goto IL_105;
					}
					IL_189:
					loadOrderValue = modSettingsData2.loadOrderValue;
					i++;
					continue;
					IL_105:
					if (modSettingsData2.loadOrderValue <= loadOrderValue)
					{
						modSettingsData2.loadOrderValue = loadOrderValue + 1;
						Game.Log("Menu: Writing new load order for " + modSettingsData2.modData.modProfile.name + ": " + modSettingsData2.loadOrderValue.ToString(), 2);
						string text3 = modSettingsData2.modData.directory + "/ModSettings.txt";
						string text2 = JsonUtility.ToJson(modSettingsData2, true);
						using (StreamWriter streamWriter2 = File.CreateText(text3))
						{
							streamWriter2.Write(text2);
						}
						goto IL_189;
					}
					goto IL_189;
				}
			}
		}
		ModController.Instance.SetModConfigChanged(true);
		ModController.Instance.UpdateModEntries();
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x00105A70 File Offset: 0x00103C70
	public void OnMoveUpButton()
	{
		int num = ModLoader.Instance.sortedModsList.IndexOf(this.mod);
		if (num > -1)
		{
			ModSettingsData modSettingsData = ModLoader.Instance.sortedModsList[num];
			int num2 = modSettingsData.loadOrderValue;
			int loadOrderValue = modSettingsData.loadOrderValue;
			if (num - 1 >= 0)
			{
				int i = num - 1;
				while (i >= 0)
				{
					ModSettingsData modSettingsData2 = ModLoader.Instance.sortedModsList[i];
					if (i != num - 1)
					{
						goto IL_F7;
					}
					num2 = modSettingsData2.loadOrderValue - 1;
					if (modSettingsData.loadOrderValue != num2)
					{
						modSettingsData.loadOrderValue = num2;
						Game.Log("Mods: Writing new load order for " + modSettingsData.modData.modProfile.name + ": " + modSettingsData.loadOrderValue.ToString(), 2);
						string text = modSettingsData.modData.directory + "/ModSettings.txt";
						string text2 = JsonUtility.ToJson(modSettingsData, true);
						using (StreamWriter streamWriter = File.CreateText(text))
						{
							streamWriter.Write(text2);
							goto IL_17B;
						}
						goto IL_F7;
					}
					IL_17B:
					loadOrderValue = modSettingsData2.loadOrderValue;
					i--;
					continue;
					IL_F7:
					if (modSettingsData2.loadOrderValue >= loadOrderValue)
					{
						modSettingsData2.loadOrderValue = loadOrderValue - 1;
						Game.Log("Mods: Writing new load order for " + modSettingsData2.modData.modProfile.name + ": " + modSettingsData2.loadOrderValue.ToString(), 2);
						string text3 = modSettingsData2.modData.directory + "/ModSettings.txt";
						string text2 = JsonUtility.ToJson(modSettingsData2, true);
						using (StreamWriter streamWriter2 = File.CreateText(text3))
						{
							streamWriter2.Write(text2);
						}
						goto IL_17B;
					}
					goto IL_17B;
				}
			}
		}
		ModController.Instance.SetModConfigChanged(true);
		ModController.Instance.UpdateModEntries();
	}

	// Token: 0x040016A5 RID: 5797
	[Header("Setup")]
	public ModSettingsData mod;

	// Token: 0x040016A6 RID: 5798
	public Sprite enabledSprite;

	// Token: 0x040016A7 RID: 5799
	public Sprite disabledSprite;

	// Token: 0x040016A8 RID: 5800
	public Sprite updatePendingSprite;

	// Token: 0x040016A9 RID: 5801
	[Header("State")]
	public float updateTimer;

	// Token: 0x040016AA RID: 5802
	[Header("Componets")]
	public TextMeshProUGUI nameText;

	// Token: 0x040016AB RID: 5803
	public TextMeshProUGUI versionText;

	// Token: 0x040016AC RID: 5804
	public Image iconImg;

	// Token: 0x040016AD RID: 5805
	public ButtonController enableDisableButton;

	// Token: 0x040016AE RID: 5806
	public ButtonController moveUpButton;

	// Token: 0x040016AF RID: 5807
	public ButtonController moveDownButton;
}
