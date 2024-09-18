using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021B RID: 539
[Serializable]
public class FloorSaveData
{
	// Token: 0x04000DCA RID: 3530
	public string floorName = "newFloor";

	// Token: 0x04000DCB RID: 3531
	public Vector2 size = new Vector2(1f, 1f);

	// Token: 0x04000DCC RID: 3532
	public int defaultFloorHeight;

	// Token: 0x04000DCD RID: 3533
	public int defaultCeilingHeight = 42;

	// Token: 0x04000DCE RID: 3534
	public List<AddressSaveData> a_d = new List<AddressSaveData>();

	// Token: 0x04000DCF RID: 3535
	public List<TileSaveData> t_d = new List<TileSaveData>();
}
