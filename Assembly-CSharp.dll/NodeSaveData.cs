using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000220 RID: 544
[Serializable]
public class NodeSaveData
{
	// Token: 0x04000DDE RID: 3550
	public Vector2Int f_c;

	// Token: 0x04000DDF RID: 3551
	public int f_h;

	// Token: 0x04000DE0 RID: 3552
	public NewNode.FloorTileType f_t;

	// Token: 0x04000DE1 RID: 3553
	public string f_r;

	// Token: 0x04000DE2 RID: 3554
	public List<WallSaveData> w_d = new List<WallSaveData>();
}
