using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class DebugBlockingSelector : MonoBehaviour
{
	// Token: 0x06000B99 RID: 2969 RVA: 0x000A9F62 File Offset: 0x000A8162
	public void Setup(WalkableRecorder.TileSetup newTile, CityData.BlockingDirection newDir, WalkableRecorder newRecorder, Vector2 newOffset)
	{
		this.tile = newTile;
		this.dir = newDir;
		this.offset = newOffset;
		this.recorder = newRecorder;
		this.rend = base.gameObject.GetComponent<MeshRenderer>();
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x000A9F92 File Offset: 0x000A8192
	[Button("Set Blocked", 0)]
	public void SetB()
	{
		this.SetBlocked(true);
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x000A9F9B File Offset: 0x000A819B
	[Button("Set Unblocked", 0)]
	public void SetUB()
	{
		this.SetBlocked(false);
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x000A9FA4 File Offset: 0x000A81A4
	private void SetBlocked(bool val)
	{
		this.blocked = val;
		if (this.blocked)
		{
			this.rend.sharedMaterial = this.recorder.blockedMaterial;
			return;
		}
		this.rend.sharedMaterial = this.recorder.nonBlockedMaterial;
	}

	// Token: 0x04000CD8 RID: 3288
	public WalkableRecorder.TileSetup tile;

	// Token: 0x04000CD9 RID: 3289
	public CityData.BlockingDirection dir;

	// Token: 0x04000CDA RID: 3290
	public MeshRenderer rend;

	// Token: 0x04000CDB RID: 3291
	public bool blocked;

	// Token: 0x04000CDC RID: 3292
	public WalkableRecorder recorder;

	// Token: 0x04000CDD RID: 3293
	public Vector2 offset;
}
