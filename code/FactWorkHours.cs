using System;
using System.Collections.Generic;

// Token: 0x02000679 RID: 1657
public class FactWorkHours : Fact
{
	// Token: 0x0600245F RID: 9311 RVA: 0x001DE6B9 File Offset: 0x001DC8B9
	public FactWorkHours(FactPreset newPreset, List<Evidence> newFromEvidence, List<Evidence> newToEvidence, List<object> newPassedObjects, List<Evidence.DataKey> overrideFromKeys, List<Evidence.DataKey> overrideToKeys, bool isCustomFact) : base(newPreset, newFromEvidence, newToEvidence, newPassedObjects, overrideFromKeys, overrideToKeys, isCustomFact)
	{
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x001DEDD4 File Offset: 0x001DCFD4
	public override string GetName(Evidence.FactLink specificLink = null)
	{
		string text = string.Empty;
		if (specificLink == null || !specificLink.thisIsTheFromEvidence)
		{
			for (int i = 0; i < this.fromEvidence.Count; i++)
			{
				Evidence evidence = this.fromEvidence[i];
				if (i > 0)
				{
					text += " & ";
				}
				text += evidence.GetNameForDataKey(this.fromDataKeys);
			}
		}
		else
		{
			text += specificLink.thisEvidence.GetNameForDataKey(this.fromDataKeys);
		}
		text = text + " " + Strings.Get("evidence.generic", this.preset.name, Strings.Casing.asIs, false, false, false, null) + " ";
		text = text + (this.fromEvidence[0] as EvidenceCitizen).witnessController.job.GetWorkingHoursString() + this.FoundAtName() + this.GenerateNameSuffix();
		return text.Trim();
	}
}
