using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006C4 RID: 1732
[CreateAssetMenu(fileName = "broadcast_data", menuName = "Database/Broadcast Preset")]
public class BroadcastPreset : SoCustomComparison
{
	// Token: 0x0400316B RID: 12651
	[Header("Contents")]
	public AudioEvent audioEvent;

	// Token: 0x0400316C RID: 12652
	[Tooltip("Change image every x seconds (real time)")]
	public float changeImageEvery = 2f;

	// Token: 0x0400316D RID: 12653
	[Tooltip("What order these images display in")]
	public BroadcastPreset.ImageOrder order;

	// Token: 0x0400316E RID: 12654
	[Header("Atlas")]
	[ShowAssetPreview(64, 64)]
	public Texture2D spriteSheet;

	// Token: 0x0400316F RID: 12655
	public Vector2 spriteResolution = new Vector2(32f, 32f);

	// Token: 0x04003170 RID: 12656
	public int indexWidth = 5;

	// Token: 0x04003171 RID: 12657
	public int indexHeight = 1;

	// Token: 0x04003172 RID: 12658
	public int totalSpriteCount = 5;

	// Token: 0x020006C5 RID: 1733
	public enum ImageOrder
	{
		// Token: 0x04003174 RID: 12660
		random,
		// Token: 0x04003175 RID: 12661
		ordered
	}
}
