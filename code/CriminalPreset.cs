using System;
using UnityEngine;

// Token: 0x020006F4 RID: 1780
[CreateAssetMenu(fileName = "criminal_data", menuName = "Database/Criminal")]
public class CriminalPreset : SoCustomComparison
{
	// Token: 0x04003310 RID: 13072
	public CriminalPreset.CriminalType type;

	// Token: 0x04003311 RID: 13073
	public bool canBeAgent;

	// Token: 0x04003312 RID: 13074
	public bool canHaveJob = true;

	// Token: 0x04003313 RID: 13075
	public int suggestedRank;

	// Token: 0x04003314 RID: 13076
	public CriminalPreset boss;

	// Token: 0x04003315 RID: 13077
	public int positionsMin = 1;

	// Token: 0x04003316 RID: 13078
	public int positionsMax = 1;

	// Token: 0x04003317 RID: 13079
	public float desiredCrimePerDay = 1.5f;

	// Token: 0x020006F5 RID: 1781
	public enum CriminalType
	{
		// Token: 0x04003319 RID: 13081
		serialKiller
	}
}
