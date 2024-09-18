using System;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public class MaterialOverrideController : MonoBehaviour
{
	// Token: 0x04001CF3 RID: 7411
	[Range(0f, 1f)]
	[Header("Override Properties")]
	public float concrete;

	// Token: 0x04001CF4 RID: 7412
	[Range(0f, 1f)]
	public float plaster;

	// Token: 0x04001CF5 RID: 7413
	[Range(0f, 1f)]
	public float wood = 1f;

	// Token: 0x04001CF6 RID: 7414
	[Range(0f, 1f)]
	public float carpet;

	// Token: 0x04001CF7 RID: 7415
	[Range(0f, 1f)]
	public float tile;

	// Token: 0x04001CF8 RID: 7416
	[Range(0f, 1f)]
	public float metal;

	// Token: 0x04001CF9 RID: 7417
	[Range(0f, 1f)]
	public float glass;

	// Token: 0x04001CFA RID: 7418
	[Range(0f, 1f)]
	public float fabric;
}
