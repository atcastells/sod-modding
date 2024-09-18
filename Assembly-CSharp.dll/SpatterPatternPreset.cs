using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007CF RID: 1999
[CreateAssetMenu(fileName = "spatter_data", menuName = "Database/Spatter Pattern")]
public class SpatterPatternPreset : SoCustomComparison
{
	// Token: 0x04003D5A RID: 15706
	[Header("Configuration")]
	public int spatterCount = 2000;

	// Token: 0x04003D5B RID: 15707
	public float maxAngleX = 1f;

	// Token: 0x04003D5C RID: 15708
	public float maxAngleY = 1f;

	// Token: 0x04003D5D RID: 15709
	[MinMaxSlider(0f, 10f)]
	public Vector2 rayLength;

	// Token: 0x04003D5E RID: 15710
	public AnimationCurve spreadCurve;

	// Token: 0x04003D5F RID: 15711
	public Material heavyMaterial;

	// Token: 0x04003D60 RID: 15712
	public Material mediumMaterial;

	// Token: 0x04003D61 RID: 15713
	public Material lightMaterial;
}
