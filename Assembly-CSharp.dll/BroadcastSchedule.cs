using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006C6 RID: 1734
[CreateAssetMenu(fileName = "broadcastschedule_data", menuName = "Database/Broadcast Schedule")]
public class BroadcastSchedule : SoCustomComparison
{
	// Token: 0x04003176 RID: 12662
	[Header("Contents")]
	public List<BroadcastPreset> broadcasts = new List<BroadcastPreset>();
}
