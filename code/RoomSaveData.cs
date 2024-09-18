using System;
using System.Collections.Generic;

// Token: 0x0200021E RID: 542
[Serializable]
public class RoomSaveData
{
	// Token: 0x04000DD4 RID: 3540
	public int id;

	// Token: 0x04000DD5 RID: 3541
	public List<NodeSaveData> n_d = new List<NodeSaveData>();

	// Token: 0x04000DD6 RID: 3542
	public string l;
}
