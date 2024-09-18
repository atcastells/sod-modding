using System;
using ModIO;

// Token: 0x02000336 RID: 822
[Serializable]
public class ModSettingsData
{
	// Token: 0x040016B5 RID: 5813
	public string name;

	// Token: 0x040016B6 RID: 5814
	public int loadOrderValue;

	// Token: 0x040016B7 RID: 5815
	[NonSerialized]
	public UserInstalledMod modData;
}
