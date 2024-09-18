using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x02000907 RID: 2311
	[Serializable]
	public class ImportOptions
	{
		// Token: 0x04004C52 RID: 19538
		[Tooltip("load the OBJ file assuming its vertical axis is Z instead of Y")]
		public bool zUp = true;

		// Token: 0x04004C53 RID: 19539
		[Tooltip("Consider diffuse map as already lit (disable lighting) if no other texture is present")]
		public bool litDiffuse;

		// Token: 0x04004C54 RID: 19540
		[Tooltip("Consider to double-sided (duplicate and flip faces and normals")]
		public bool convertToDoubleSided;

		// Token: 0x04004C55 RID: 19541
		[Tooltip("Rescaling for the model (1 = no rescaling)")]
		public float modelScaling = 1f;

		// Token: 0x04004C56 RID: 19542
		[Tooltip("Reuse a model in memory if already loaded")]
		public bool reuseLoaded;

		// Token: 0x04004C57 RID: 19543
		[Tooltip("Inherit parent layer")]
		public bool inheritLayer;

		// Token: 0x04004C58 RID: 19544
		[Tooltip("Generate mesh colliders")]
		public bool buildColliders;

		// Token: 0x04004C59 RID: 19545
		[Tooltip("Generate convex mesh colliders (only active if buildColliders = true)\nNote: it could not work for meshes with too many smooth surface regions.")]
		public bool colliderConvex;

		// Token: 0x04004C5A RID: 19546
		[Tooltip("Mesh colliders as trigger (only active if colliderConvex = true)")]
		public bool colliderTrigger;

		// Token: 0x04004C5B RID: 19547
		[Tooltip("Use 32 bit indices when needed, if available")]
		public bool use32bitIndices = true;

		// Token: 0x04004C5C RID: 19548
		[Tooltip("Hide the loaded object during the loading process")]
		public bool hideWhileLoading;

		// Token: 0x04004C5D RID: 19549
		[Tooltip("Position of the object")]
		[Header("Local Transform for the imported game object")]
		public Vector3 localPosition = Vector3.zero;

		// Token: 0x04004C5E RID: 19550
		[Tooltip("Rotation of the object\n(Euler angles)")]
		public Vector3 localEulerAngles = Vector3.zero;

		// Token: 0x04004C5F RID: 19551
		[Tooltip("Scaling of the object\n([1,1,1] = no rescaling)")]
		public Vector3 localScale = Vector3.one;
	}
}
