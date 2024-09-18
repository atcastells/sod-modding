using System;
using UnityEngine;

// Token: 0x020007F5 RID: 2037
public class CullingControls : MonoBehaviour
{
	// Token: 0x1700011C RID: 284
	// (get) Token: 0x060025FF RID: 9727 RVA: 0x001E9264 File Offset: 0x001E7464
	public static CullingControls Instance
	{
		get
		{
			return CullingControls._instance;
		}
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x001E926B File Offset: 0x001E746B
	private void Awake()
	{
		if (CullingControls._instance != null && CullingControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CullingControls._instance = this;
	}

	// Token: 0x04004078 RID: 16504
	[Header("FoVs")]
	[Tooltip("The angle which you can see other buildings")]
	public float visibleBuildingFoV = 91f;

	// Token: 0x04004079 RID: 16505
	[Tooltip("The angle which you can see other rooms from entrances")]
	public float visibleRoomFoV = 91f;

	// Token: 0x0400407A RID: 16506
	[Header("Distances")]
	public float fromOutsideToInsideDistanceMax = 25f;

	// Token: 0x0400407B RID: 16507
	public float fromInsideToInsideDistanceMax = 35f;

	// Token: 0x0400407C RID: 16508
	[Space(7f)]
	public float outsideDistanceMax = 75f;

	// Token: 0x0400407D RID: 16509
	[Tooltip("Boost the above by lerping the below by floor height (floor 16 max)")]
	public Vector2 outsideHeightDistanceBoost = new Vector2(0f, 30f);

	// Token: 0x0400407E RID: 16510
	[Space(7f)]
	[Tooltip("Distance within which rooms are drawn through windows")]
	public float windowCullingRange = 25f;

	// Token: 0x0400407F RID: 16511
	[Tooltip("Distance within which rooms are drawn through open doors")]
	public float doorCullingRange = 25f;

	// Token: 0x04004080 RID: 16512
	[Tooltip("Distance within which exterior air ducts are rendered")]
	public float exteriorDuctCullingRange = 35f;

	// Token: 0x04004081 RID: 16513
	[Tooltip("Distance within which connected rooms are drawn when inside a duct")]
	public float ductRoomCullingRange = 20f;

	// Token: 0x04004082 RID: 16514
	[Header("Air Ducts")]
	public float airDuctLODThreshold = 0.3f;

	// Token: 0x04004083 RID: 16515
	private static CullingControls _instance;
}
