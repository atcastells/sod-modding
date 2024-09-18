using System;
using System.Collections.Generic;

// Token: 0x02000649 RID: 1609
public class EvidenceDebug : Evidence
{
	// Token: 0x060023B4 RID: 9140 RVA: 0x001DA9A1 File Offset: 0x001D8BA1
	public EvidenceDebug(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		this.id = EvidenceDebug.assignID;
		EvidenceDebug.assignID++;
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x001DA9C5 File Offset: 0x001D8BC5
	public override string GenerateName()
	{
		return "Debug Evidence " + this.id.ToString();
	}

	// Token: 0x04002DBF RID: 11711
	public static int assignID;

	// Token: 0x04002DC0 RID: 11712
	public int id;
}
