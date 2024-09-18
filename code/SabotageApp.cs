using System;
using TMPro;
using UnityEngine;

// Token: 0x020002A8 RID: 680
public class SabotageApp : CruncherAppContent
{
	// Token: 0x06000F18 RID: 3864 RVA: 0x000D7430 File Offset: 0x000D5630
	private void Update()
	{
		this.progress += Time.deltaTime * Toolbox.Instance.Rand(0.5f, 1f, false);
		this.progress = Mathf.Clamp01(this.progress);
		this.progressBarFill.sizeDelta = new Vector2(this.progressBar.sizeDelta.x * this.progress, this.progressBarFill.sizeDelta.y);
		this.percentageText.text = Mathf.CeilToInt(this.progress * 100f).ToString() + "%";
		if (this.progress >= 1f)
		{
			if (this.controller.ic.interactable.node.gameLocation.thisAsAddress != null && this.controller.ic.interactable.node.gameLocation.thisAsAddress.company != null)
			{
				if (!GameplayController.Instance.companiesSabotaged.Contains(this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.name))
				{
					int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.installMalware));
					if (this.controller.ic.interactable.node.gameLocation.thisAsAddress.owners.Contains(this.controller.loggedInAs))
					{
						num = Mathf.RoundToInt((float)num * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.malwareOwnerBonus)));
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "corpsabotage_success", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageGreen, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					}
					else
					{
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "corpsabotage_success_bonus", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageGreen, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					}
					GameplayController.Instance.AddMoney(Mathf.Max(num, 1), true, "corpsabotage");
					GameplayController.Instance.companiesSabotaged.Add(this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.name);
				}
				else
				{
					InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "corpsabotage_fail", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
				}
			}
			this.controller.OnAppExit();
		}
	}

	// Token: 0x0400123C RID: 4668
	public TextMeshProUGUI titleText;

	// Token: 0x0400123D RID: 4669
	public TextMeshProUGUI percentageText;

	// Token: 0x0400123E RID: 4670
	public RectTransform progressBar;

	// Token: 0x0400123F RID: 4671
	public RectTransform progressBarFill;

	// Token: 0x04001240 RID: 4672
	public float progress;
}
