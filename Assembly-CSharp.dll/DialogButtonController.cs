using System;
using System.Text;
using UnityEngine;

// Token: 0x020004F7 RID: 1271
public class DialogButtonController : ButtonController
{
	// Token: 0x06001B5C RID: 7004 RVA: 0x0018CEDE File Offset: 0x0018B0DE
	public void Setup(EvidenceWitness.DialogOption newPreset)
	{
		base.SetupReferences();
		this.option = newPreset;
		this.UpdateButtonText();
	}

	// Token: 0x06001B5D RID: 7005 RVA: 0x0018CEF4 File Offset: 0x0018B0F4
	public override void UpdateButtonText()
	{
		if (this.option == null)
		{
			return;
		}
		if (this.option.preset == null)
		{
			return;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		Actor talkingTo = null;
		if (InteractionController.Instance.talkingTo != null)
		{
			talkingTo = InteractionController.Instance.talkingTo.isActor;
		}
		int num = this.option.preset.GetCost(talkingTo, Player.Instance);
		if (this.option.preset.specialCase == DialogPreset.SpecialCase.medicalCosts)
		{
			num = Mathf.RoundToInt((float)this.option.preset.GetCost(talkingTo, Player.Instance) * (1f - UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.reduceMedicalCosts)));
		}
		if (num > 0)
		{
			if (GameplayController.Instance.money < num)
			{
				text = "<s>";
				text2 = "</s>";
			}
			if (this.option.preset.usePercentageCost)
			{
				int num2 = this.option.preset.cost;
				if (this.option.preset.specialCase == DialogPreset.SpecialCase.medicalCosts)
				{
					num2 = Mathf.RoundToInt((float)this.option.preset.cost * (1f - UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.reduceMedicalCosts)));
				}
				text2 = text2 + "[" + num2.ToString() + "%]";
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.option.preset.preceedingSyntax);
		if (this.option.preset.displayAsIllegal)
		{
			stringBuilder.Append("<color=#" + InterfaceControls.Instance.interactionControlTextIllegalHex + ">");
		}
		stringBuilder.Append(text + Strings.GetTextForComponent(this.option.preset.msgID, Player.Instance, null, null, "\n", false, this.option.jobRef, Strings.LinkSetting.automatic, null) + text2);
		if (num > 0)
		{
			stringBuilder.Append(" [" + CityControls.Instance.cityCurrency + num.ToString() + "]");
		}
		this.text.text = stringBuilder.ToString();
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x0018D10C File Offset: 0x0018B30C
	public void SetSelectable(bool val)
	{
		this.selectable = val;
		this.button.interactable = this.selectable;
		if (this.selectable)
		{
			this.text.alpha = 1f;
			return;
		}
		this.text.alpha = 0.75f;
	}

	// Token: 0x04002421 RID: 9249
	public EvidenceWitness.DialogOption option;

	// Token: 0x04002422 RID: 9250
	public bool selectable = true;
}
