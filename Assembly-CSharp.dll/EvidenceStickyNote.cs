using System;
using System.Collections.Generic;

// Token: 0x02000667 RID: 1639
public class EvidenceStickyNote : Evidence
{
	// Token: 0x06002417 RID: 9239 RVA: 0x001DAA28 File Offset: 0x001D8C28
	public EvidenceStickyNote(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x001DCC64 File Offset: 0x001DAE64
	public override string GenerateName()
	{
		return Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
	}
}
