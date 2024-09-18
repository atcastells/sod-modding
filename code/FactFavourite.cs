using System;
using System.Collections.Generic;

// Token: 0x02000675 RID: 1653
public class FactFavourite : Fact
{
	// Token: 0x06002455 RID: 9301 RVA: 0x001DE876 File Offset: 0x001DCA76
	public FactFavourite(FactPreset newPreset, List<Evidence> newFromEvidence, List<Evidence> newToEvidence, List<object> newPassedObjects, List<Evidence.DataKey> overrideFromKeys, List<Evidence.DataKey> overrideToKeys, bool isCustomFact) : base(newPreset, newFromEvidence, newToEvidence, newPassedObjects, overrideFromKeys, overrideToKeys, isCustomFact)
	{
		this.company = (this.toEvidence[0] as EvidenceLocation).locationController.thisAsAddress.company;
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x001DE8AF File Offset: 0x001DCAAF
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		this.company = (this.toEvidence[0] as EvidenceLocation).locationController.thisAsAddress.company;
		this.UpdateName();
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x001DE8E4 File Offset: 0x001DCAE4
	public override string GetName(Evidence.FactLink specificLink = null)
	{
		this.company = (this.toEvidence[0] as EvidenceLocation).locationController.thisAsAddress.company;
		string text = string.Empty;
		for (int i = 0; i < this.fromEvidence.Count; i++)
		{
			Evidence evidence = this.fromEvidence[i];
			if (i > 0)
			{
				text += " & ";
			}
			text += evidence.GetNameForDataKey(Evidence.DataKey.name);
		}
		text = text + " " + Strings.Get("evidence.generic", this.preset.name, Strings.Casing.asIs, false, false, false, null);
		text = text + " " + Strings.Get("evidence.generic", "is", Strings.Casing.asIs, false, false, false, null);
		for (int j = 0; j < this.toEvidence.Count; j++)
		{
			Evidence evidence2 = this.toEvidence[j];
			if (j > 0)
			{
				text += " & ";
			}
			text += evidence2.GetNameForDataKey(Evidence.DataKey.name);
		}
		return text.Trim();
	}

	// Token: 0x04002E14 RID: 11796
	public Company company;
}
