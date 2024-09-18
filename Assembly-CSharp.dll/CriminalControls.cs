using System;
using UnityEngine;

// Token: 0x020007F4 RID: 2036
public class CriminalControls : MonoBehaviour
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x060025FC RID: 9724 RVA: 0x001E922F File Offset: 0x001E742F
	public static CriminalControls Instance
	{
		get
		{
			return CriminalControls._instance;
		}
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x001E9236 File Offset: 0x001E7436
	private void Awake()
	{
		if (CriminalControls._instance != null && CriminalControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CriminalControls._instance = this;
	}

	// Token: 0x04004075 RID: 16501
	[Header("Blood Patterns")]
	public SpatterPatternPreset punchSpatter;

	// Token: 0x04004076 RID: 16502
	[Header("Weapon References")]
	public MurderWeaponPreset sniperRifle;

	// Token: 0x04004077 RID: 16503
	private static CriminalControls _instance;
}
