using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007C7 RID: 1991
[CreateAssetMenu(fileName = "roomtypefilter_data", menuName = "Database/Room Type Filter")]
public class RoomTypeFilter : SoCustomComparison
{
	// Token: 0x04003D08 RID: 15624
	[Header("Room Types")]
	public List<RoomClassPreset> roomClasses = new List<RoomClassPreset>();
}
