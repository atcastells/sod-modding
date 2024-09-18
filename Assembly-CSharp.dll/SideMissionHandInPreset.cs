using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007CA RID: 1994
[CreateAssetMenu(fileName = "sidejobhandin_data", menuName = "Database/Side Job Hand-In Preset")]
public class SideMissionHandInPreset : SoCustomComparison
{
	// Token: 0x04003D2E RID: 15662
	[Header("Rewards")]
	public int rewardModifier;

	// Token: 0x04003D2F RID: 15663
	[Header("Location")]
	public bool postersDoor;

	// Token: 0x04003D30 RID: 15664
	public bool cityHall;

	// Token: 0x04003D31 RID: 15665
	[Header("Elements")]
	public List<SideMissionIntroPreset.SideMissionObjectiveBlock> blocks = new List<SideMissionIntroPreset.SideMissionObjectiveBlock>();
}
