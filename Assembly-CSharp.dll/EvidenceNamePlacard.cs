using System;
using System.Collections.Generic;

// Token: 0x02000660 RID: 1632
public class EvidenceNamePlacard : Evidence
{
	// Token: 0x06002405 RID: 9221 RVA: 0x001DAA28 File Offset: 0x001D8C28
	public EvidenceNamePlacard(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x001DC214 File Offset: 0x001DA414
	public override void Compile()
	{
		if (this.belongsTo != null)
		{
			base.SetNote(Toolbox.Instance.allDataKeys, this.belongsTo.GetCitizenName() + "\n" + Strings.Get("jobs", this.belongsTo.job.preset.name, Strings.Casing.asIs, false, false, false, null));
		}
		else
		{
			Game.LogError("Name placard evidence has no owner", 2);
		}
		base.Compile();
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x001DC28C File Offset: 0x001DA48C
	public override string GenerateName()
	{
		return this.belongsTo.GetCitizenName() + "\n" + Strings.Get("jobs", this.belongsTo.job.preset.name, Strings.Casing.asIs, false, false, false, null);
	}
}
