using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001FE RID: 510
public class DebugWalkingZone : MonoBehaviour
{
	// Token: 0x06000BAD RID: 2989 RVA: 0x000AA5B4 File Offset: 0x000A87B4
	private void Awake()
	{
		this.textureList.Add(this.green);
		this.textureList.Add(this.yellow);
		this.textureList.Add(this.red);
		this.textureList.Add(this.orange);
		this.textureList.Add(this.blue);
		this.textureList.Add(this.violet);
		this.textureList.Add(this.turqoise);
	}

	// Token: 0x04000D02 RID: 3330
	public NewTile tile;

	// Token: 0x04000D03 RID: 3331
	private Renderer rend;

	// Token: 0x04000D04 RID: 3332
	public Texture green;

	// Token: 0x04000D05 RID: 3333
	public Texture yellow;

	// Token: 0x04000D06 RID: 3334
	public Texture red;

	// Token: 0x04000D07 RID: 3335
	public Texture orange;

	// Token: 0x04000D08 RID: 3336
	public Texture blue;

	// Token: 0x04000D09 RID: 3337
	public Texture violet;

	// Token: 0x04000D0A RID: 3338
	public Texture turqoise;

	// Token: 0x04000D0B RID: 3339
	private List<Texture> textureList = new List<Texture>();
}
