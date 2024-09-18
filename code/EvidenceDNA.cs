using System;
using System.Collections.Generic;

// Token: 0x0200064A RID: 1610
public class EvidenceDNA : Evidence
{
	// Token: 0x060023B6 RID: 9142 RVA: 0x001DA9DC File Offset: 0x001D8BDC
	public EvidenceDNA(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		this.citizenController = (newController as Citizen);
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x001DA9F5 File Offset: 0x001D8BF5
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		this.citizenController.evidenceEntry.OnDataKeyChange += this.UpdateName;
	}

	// Token: 0x04002DC1 RID: 11713
	public Citizen citizenController;

	// Token: 0x04002DC2 RID: 11714
	public static int DNAAssign = -1;

	// Token: 0x04002DC3 RID: 11715
	public static int DNAAssignLoop = 0;
}
