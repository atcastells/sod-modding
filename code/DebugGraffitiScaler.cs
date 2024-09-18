using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020001F9 RID: 505
public class DebugGraffitiScaler : MonoBehaviour
{
	// Token: 0x06000B9E RID: 2974 RVA: 0x000A9FE4 File Offset: 0x000A81E4
	[Button(null, 0)]
	public void LoadArt()
	{
		if (this.art != null && this.decal != null)
		{
			this.decal.material = this.art.material;
			this.pixelScaleMultiplier = this.art.pixelScaleMultiplier;
			this.SetScale();
		}
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x000AA03C File Offset: 0x000A823C
	[Button(null, 0)]
	public void SetScale()
	{
		if (this.decal != null)
		{
			Texture texture = this.decal.material.GetTexture("_BaseColorMap");
			this.decal.size = new Vector3((float)texture.width * this.pixelScaleMultiplier, (float)texture.height * this.pixelScaleMultiplier, 0.13f);
		}
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void SavePixelScale()
	{
	}

	// Token: 0x04000CDE RID: 3294
	public ArtPreset art;

	// Token: 0x04000CDF RID: 3295
	public DecalProjector decal;

	// Token: 0x04000CE0 RID: 3296
	public float pixelScaleMultiplier = 0.05f;
}
