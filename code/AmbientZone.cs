using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006B9 RID: 1721
[CreateAssetMenu(fileName = "ambient_data", menuName = "Audio/Ambient Zone")]
public class AmbientZone : SoCustomComparison
{
	// Token: 0x040030CE RID: 12494
	[Header("Audio")]
	public AudioEvent mainEvent;

	// Token: 0x040030CF RID: 12495
	[Tooltip("If true, this zone can be active and heard outside of the assigned room.")]
	[Header("Occlusion")]
	public bool useOcclusion = true;

	// Token: 0x040030D0 RID: 12496
	[EnableIf("useOcclusion")]
	public float maxRange = 10f;

	// Token: 0x040030D1 RID: 12497
	[Tooltip("If true this sound can penetrate closed doors")]
	[EnableIf("useOcclusion")]
	public bool canPenetrateClosedDoors = true;

	// Token: 0x040030D2 RID: 12498
	[Tooltip("Overrides default occlusion value sound in the audio controller")]
	[Space(7f)]
	[EnableIf("useOcclusion")]
	public bool overrideOcclusionModifier;

	// Token: 0x040030D3 RID: 12499
	[Tooltip("Each occlusion unit will decrease volume by this amount...")]
	[EnableIf("overrideOcclusionModifier")]
	public float occlusionUnitVolumeModifier = -0.1f;

	// Token: 0x040030D4 RID: 12500
	[Header("Special Cases")]
	public bool isAirDuctAmbience;

	// Token: 0x040030D5 RID: 12501
	[Tooltip("Pass time of day")]
	[Header("Params")]
	public bool passTimeOfDay;

	// Token: 0x040030D6 RID: 12502
	[Tooltip("Pass walla amount")]
	public bool passWalla;

	// Token: 0x040030D7 RID: 12503
	[Tooltip("Pass player in vent")]
	public bool passPlayerInVent = true;

	// Token: 0x040030D8 RID: 12504
	[Tooltip("Pass player vent ext/int")]
	public bool passPlayerVentExtInt;

	// Token: 0x040030D9 RID: 12505
	[Tooltip("Pass the player's distance to the nearest vent")]
	public bool passDistanceToVent;

	// Token: 0x040030DA RID: 12506
	[Tooltip("Pass rain")]
	public bool passRain;

	// Token: 0x040030DB RID: 12507
	[Tooltip("Pass basement")]
	public bool passBasement;

	// Token: 0x040030DC RID: 12508
	[Tooltip("Pass combination of height and wind speed")]
	public bool passHeightWindSpeed;

	// Token: 0x040030DD RID: 12509
	[Tooltip("Pass city edge distance")]
	public bool passEdgeDistance;

	// Token: 0x040030DE RID: 12510
	[EnableIf("passWalla")]
	[Tooltip("The range to sample crowds")]
	public float maxWallaRange = 10f;

	// Token: 0x040030DF RID: 12511
	[Tooltip("The number of people present per node for maximum walla")]
	[EnableIf("passWalla")]
	public float maxWallaCrowd = 10f;
}
