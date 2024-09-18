using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000780 RID: 1920
[CreateAssetMenu(fileName = "motive_data", menuName = "Database/Motive Preset")]
public class MotivePreset : SoCustomComparison
{
	// Token: 0x040039D0 RID: 14800
	[Header("Purpetrator")]
	public bool allowHomelessPurps;

	// Token: 0x040039D1 RID: 14801
	public bool allowJoblessPurps = true;

	// Token: 0x040039D2 RID: 14802
	public bool purpMustLiveAtDifferentAddressToPoster = true;

	// Token: 0x040039D3 RID: 14803
	public bool allowEnforcers = true;

	// Token: 0x040039D4 RID: 14804
	[ReorderableList]
	[Tooltip("Purps must follow these trait rules")]
	public List<MotivePreset.ModifierRule> purpTraitModifiers = new List<MotivePreset.ModifierRule>();

	// Token: 0x040039D5 RID: 14805
	[Space(7f)]
	[Tooltip("Purp must have one of these jobs...")]
	public bool usePurpJobs;

	// Token: 0x040039D6 RID: 14806
	[ReorderableList]
	public List<OccupationPreset> purpJobs = new List<OccupationPreset>();

	// Token: 0x040039D7 RID: 14807
	[Header("Posters")]
	public bool allowHomelessPosters;

	// Token: 0x040039D8 RID: 14808
	public bool allowJoblessPosters = true;

	// Token: 0x040039D9 RID: 14809
	public bool usePosterConnections = true;

	// Token: 0x040039DA RID: 14810
	[Tooltip("Posters must be one of these connections (poster connection to purp)...")]
	public List<Acquaintance.ConnectionType> acceptableConnections = new List<Acquaintance.ConnectionType>();

	// Token: 0x040039DB RID: 14811
	public bool usePosterTraits;

	// Token: 0x040039DC RID: 14812
	[EnableIf("usePosterTraits")]
	public List<MotivePreset.ModifierRule> posterTraitModifiers = new List<MotivePreset.ModifierRule>();

	// Token: 0x040039DD RID: 14813
	[Tooltip("The chosen purp is exempt from further side jobs.")]
	[Header("Exempt")]
	public bool purpIsExemptFromPostingOtherJobs = true;

	// Token: 0x040039DE RID: 14814
	public bool purpIsExemptFromPurpingOtherJobs;

	// Token: 0x040039DF RID: 14815
	[Tooltip("The chosen poster is exempt from further side jobs.")]
	public bool posterIsExemptFromPostingOtherJobs = true;

	// Token: 0x040039E0 RID: 14816
	public bool posterIsExemptFromPurpingOtherJobs = true;

	// Token: 0x02000781 RID: 1921
	[Serializable]
	public class ModifierRule
	{
		// Token: 0x040039E1 RID: 14817
		public CharacterTrait.RuleType rule;

		// Token: 0x040039E2 RID: 14818
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x040039E3 RID: 14819
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		public bool mustPassForApplication = true;

		// Token: 0x040039E4 RID: 14820
		public int score;
	}
}
