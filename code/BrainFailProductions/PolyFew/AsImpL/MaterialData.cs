using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x02000900 RID: 2304
	public class MaterialData
	{
		// Token: 0x04004C24 RID: 19492
		public string materialName;

		// Token: 0x04004C25 RID: 19493
		public Color ambientColor;

		// Token: 0x04004C26 RID: 19494
		public Color diffuseColor;

		// Token: 0x04004C27 RID: 19495
		public Color specularColor;

		// Token: 0x04004C28 RID: 19496
		public Color emissiveColor;

		// Token: 0x04004C29 RID: 19497
		public float shininess;

		// Token: 0x04004C2A RID: 19498
		public float overallAlpha = 1f;

		// Token: 0x04004C2B RID: 19499
		public int illumType;

		// Token: 0x04004C2C RID: 19500
		public bool hasReflectionTex;

		// Token: 0x04004C2D RID: 19501
		public string diffuseTexPath;

		// Token: 0x04004C2E RID: 19502
		public Texture2D diffuseTex;

		// Token: 0x04004C2F RID: 19503
		public string bumpTexPath;

		// Token: 0x04004C30 RID: 19504
		public Texture2D bumpTex;

		// Token: 0x04004C31 RID: 19505
		public string specularTexPath;

		// Token: 0x04004C32 RID: 19506
		public Texture2D specularTex;

		// Token: 0x04004C33 RID: 19507
		public string opacityTexPath;

		// Token: 0x04004C34 RID: 19508
		public Texture2D opacityTex;
	}
}
