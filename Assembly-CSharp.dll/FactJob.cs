using System;
using System.Collections.Generic;

// Token: 0x02000676 RID: 1654
public class FactJob : Fact
{
	// Token: 0x06002458 RID: 9304 RVA: 0x001DE6B9 File Offset: 0x001DC8B9
	public FactJob(FactPreset newPreset, List<Evidence> newFromEvidence, List<Evidence> newToEvidence, List<object> newPassedObjects, List<Evidence.DataKey> overrideFromKeys, List<Evidence.DataKey> overrideToKeys, bool isCustomFact) : base(newPreset, newFromEvidence, newToEvidence, newPassedObjects, overrideFromKeys, overrideToKeys, isCustomFact)
	{
	}

	// Token: 0x06002459 RID: 9305 RVA: 0x001DE9F0 File Offset: 0x001DCBF0
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		Evidence evidence = this.fromEvidence[0];
		Strings.Get("evidence.generic", this.preset.name, Strings.Casing.asIs, false, false, false, null) + " " + this.toEvidence[0].GetNameForDataKey(Evidence.DataKey.name);
	}
}
