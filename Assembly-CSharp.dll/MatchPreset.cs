using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000778 RID: 1912
[CreateAssetMenu(fileName = "match_data", menuName = "Database/Match Type")]
public class MatchPreset : SoCustomComparison
{
	// Token: 0x04003989 RID: 14729
	[Tooltip("True if this match preset can only be matched with")]
	[Header("Matching Conditions")]
	public bool canOnlyBeMatchedWith;

	// Token: 0x0400398A RID: 14730
	[Tooltip("These conditions must return true for it to register as a match. No conditions will result in a match between evidence with this match preset.")]
	public List<MatchPreset.MatchCondition> matchConditions = new List<MatchPreset.MatchCondition>();

	// Token: 0x0400398B RID: 14731
	[Tooltip("Only match with a match parent, and not with non-parents")]
	public bool onlyMatchWithMatchParents;

	// Token: 0x0400398C RID: 14732
	[Tooltip("Can this match with evidence that is technically itself?")]
	public bool canMatchWithItself;

	// Token: 0x0400398D RID: 14733
	[Tooltip("Only match with evidence with this other match condition")]
	public MatchPreset onlyMatchWithThis;

	// Token: 0x0400398E RID: 14734
	[Tooltip("Link from data key")]
	public List<Evidence.DataKey> linkFromKeys = new List<Evidence.DataKey>();

	// Token: 0x0400398F RID: 14735
	[Tooltip("Link to data key")]
	public List<Evidence.DataKey> linkToKeys = new List<Evidence.DataKey>();

	// Token: 0x02000779 RID: 1913
	public enum MatchCondition
	{
		// Token: 0x04003991 RID: 14737
		bloodGroup,
		// Token: 0x04003992 RID: 14738
		fingerprint,
		// Token: 0x04003993 RID: 14739
		time,
		// Token: 0x04003994 RID: 14740
		visualDescriptors,
		// Token: 0x04003995 RID: 14741
		retailPresetMatch,
		// Token: 0x04003996 RID: 14742
		murderWeapon
	}
}
