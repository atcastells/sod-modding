using System;
using System.Collections.Generic;

// Token: 0x02000674 RID: 1652
public class FactCustom : Fact
{
	// Token: 0x06002451 RID: 9297 RVA: 0x001DE82B File Offset: 0x001DCA2B
	public FactCustom(FactPreset newPreset, List<Evidence> newFromEvidence, List<Evidence> newToEvidence, List<object> newPassedObjects, List<Evidence.DataKey> overrideFromKeys, List<Evidence.DataKey> overrideToKeys, bool isCustomFact) : base(newPreset, newFromEvidence, newToEvidence, newPassedObjects, overrideFromKeys, overrideToKeys, isCustomFact)
	{
		this.SetCustomName(Strings.Get("evidence.generic", this.preset.name, Strings.Casing.asIs, false, false, false, null));
	}

	// Token: 0x06002452 RID: 9298 RVA: 0x001DE85E File Offset: 0x001DCA5E
	public override void SetCustomName(string str)
	{
		base.SetCustomName(str);
		this.GenerateName();
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x001DE86E File Offset: 0x001DCA6E
	public override string GenerateName()
	{
		return this.customName;
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x001DE86E File Offset: 0x001DCA6E
	public override string GetName(Evidence.FactLink specificLink = null)
	{
		return this.customName;
	}
}
