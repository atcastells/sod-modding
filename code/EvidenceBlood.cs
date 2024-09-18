using System;
using System.Collections.Generic;

// Token: 0x02000643 RID: 1603
public class EvidenceBlood : Evidence
{
	// Token: 0x0600239A RID: 9114 RVA: 0x001D8C34 File Offset: 0x001D6E34
	public EvidenceBlood(EvidencePreset newPreset, string newID, Controller newController, List<object> newPassedObjects) : base(newPreset, newID, newController, newPassedObjects)
	{
		this.citizenController = (newController as Citizen);
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x001D8C4D File Offset: 0x001D6E4D
	public override string GenerateNameSuffix()
	{
		return this.citizenController.GetBloodTypeString();
	}

	// Token: 0x04002DB7 RID: 11703
	public Citizen citizenController;
}
