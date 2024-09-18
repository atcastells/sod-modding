using System;
using System.Collections.Generic;

// Token: 0x0200066F RID: 1647
public class EvidenceWound : Evidence
{
	// Token: 0x06002434 RID: 9268 RVA: 0x001DAA28 File Offset: 0x001D8C28
	public EvidenceWound(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x00002265 File Offset: 0x00000465
	public override void BuildDataSources()
	{
	}
}
