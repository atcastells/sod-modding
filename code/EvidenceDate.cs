using System;
using System.Collections.Generic;

// Token: 0x02000648 RID: 1608
public class EvidenceDate : Evidence
{
	// Token: 0x060023B1 RID: 9137 RVA: 0x001DA90C File Offset: 0x001D8B0C
	public EvidenceDate(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		string[] array = evID.Split('|', 0);
		if (array.Length > 1)
		{
			this.date = array[1];
		}
		if (array.Length > 3)
		{
			int id = -1;
			int.TryParse(array[3], ref id);
			CityData.Instance.GetHuman(id, out this.writer, true);
		}
		if (array.Length > 4)
		{
			int id2 = -1;
			int.TryParse(array[4], ref id2);
			CityData.Instance.GetHuman(id2, out this.reciever, true);
		}
		GameplayController.Instance.dateEvidence.Add(this);
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x00002265 File Offset: 0x00000465
	public override void BuildDataSources()
	{
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x001DA999 File Offset: 0x001D8B99
	public override string GenerateName()
	{
		return this.date;
	}

	// Token: 0x04002DBE RID: 11710
	public string date;
}
