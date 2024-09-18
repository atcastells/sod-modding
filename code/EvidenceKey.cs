using System;
using System.Collections.Generic;

// Token: 0x0200064E RID: 1614
public class EvidenceKey : Evidence
{
	// Token: 0x060023CA RID: 9162 RVA: 0x001DB2B8 File Offset: 0x001D94B8
	public EvidenceKey(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		int num = (int)(newPassedObjects[0] as Interactable.Passed).value;
		CityData.Instance.roomDictionary.TryGetValue(num, ref this.keyTo);
	}

	// Token: 0x04002DC5 RID: 11717
	public NewRoom keyTo;
}
