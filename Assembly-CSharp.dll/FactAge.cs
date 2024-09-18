using System;
using System.Collections.Generic;

// Token: 0x02000673 RID: 1651
public class FactAge : Fact
{
	// Token: 0x0600244F RID: 9295 RVA: 0x001DE6B9 File Offset: 0x001DC8B9
	public FactAge(FactPreset newPreset, List<Evidence> newFromEvidence, List<Evidence> newToEvidence, List<object> newPassedObjects, List<Evidence.DataKey> overrideFromKeys, List<Evidence.DataKey> overrideToKeys, bool isCustomFact) : base(newPreset, newFromEvidence, newToEvidence, newPassedObjects, overrideFromKeys, overrideToKeys, isCustomFact)
	{
	}

	// Token: 0x06002450 RID: 9296 RVA: 0x001DE6CC File Offset: 0x001DC8CC
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
		for (int j = 0; j < this.toEvidence.Count; j++)
		{
			Evidence evidence2 = this.toEvidence[j];
			if (j > 0)
			{
				text += " & ";
			}
			text += evidence2.GetNameForDataKey(Evidence.DataKey.name);
		}
		int age = (this.fromEvidence[0] as EvidenceCitizen).witnessController.GetAge();
		text = string.Concat(new string[]
		{
			text,
			" (",
			Strings.Get("evidence.generic", "age", Strings.Casing.asIs, false, false, false, null),
			" ",
			age.ToString(),
			")"
		});
		return text.Trim();
	}
}
