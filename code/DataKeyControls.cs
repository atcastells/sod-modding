using System;
using UnityEngine;

// Token: 0x020007F6 RID: 2038
public class DataKeyControls : MonoBehaviour
{
	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06002602 RID: 9730 RVA: 0x001E9332 File Offset: 0x001E7532
	public static DataKeyControls Instance
	{
		get
		{
			return DataKeyControls._instance;
		}
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x001E9339 File Offset: 0x001E7539
	private void Awake()
	{
		if (DataKeyControls._instance != null && DataKeyControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		DataKeyControls._instance = this;
	}

	// Token: 0x04004084 RID: 16516
	private static DataKeyControls _instance;

	// Token: 0x020007F7 RID: 2039
	[Serializable]
	public class DataKeySettings
	{
		// Token: 0x04004085 RID: 16517
		public Evidence.DataKey key;

		// Token: 0x04004086 RID: 16518
		[Tooltip("Is this a unique identifier?")]
		public bool uniqueKey;

		// Token: 0x04004087 RID: 16519
		public bool countTowardsProfile = true;
	}
}
