using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200071F RID: 1823
[CreateAssetMenu(fileName = "fact_data", menuName = "Database/Evidence/Fact Preset")]
public class FactPreset : SoCustomComparison
{
	// Token: 0x04003496 RID: 13462
	[Header("Setup")]
	[Tooltip("Whenever this fact is displayed in an icon, use this sprite.")]
	public Sprite iconSpriteLarge;

	// Token: 0x04003497 RID: 13463
	[Tooltip("Spawn this subclass. If left empty it will use the base class.")]
	public string subClass = string.Empty;

	// Token: 0x04003498 RID: 13464
	[Tooltip("Allow to -> from duplicates of this evidence")]
	public bool allowDuplicates = true;

	// Token: 0x04003499 RID: 13465
	[Tooltip("Allow reverse duplicates of this evidence")]
	public bool allowReverseDuplicates = true;

	// Token: 0x0400349A RID: 13466
	[Tooltip("Link specifically to these data keys. These keys can be override manually by passing them in the constructor.")]
	[ReorderableList]
	[Header("Links")]
	public List<Evidence.DataKey> fromDataKeys = new List<Evidence.DataKey>();

	// Token: 0x0400349B RID: 13467
	[ReorderableList]
	[Tooltip("Link specifically to these data keys. These keys can be override manually by passing them in the constructor.")]
	public List<Evidence.DataKey> toDataKeys = new List<Evidence.DataKey>();

	// Token: 0x0400349C RID: 13468
	[Header("Discovery")]
	[Tooltip("Discover this evidence when it is created.")]
	public bool discoverOnCreate;

	// Token: 0x0400349D RID: 13469
	[Tooltip("When discovered, this is eligable to be tagged as 'new information'")]
	public bool countsAsNewInformationOnDiscovery = true;

	// Token: 0x0400349E RID: 13470
	[ReorderableList]
	[Tooltip("On discovery, apply these data keys to the 'from' evidence.")]
	public List<Evidence.DataKey> applyFromKeysOnDiscovery = new List<Evidence.DataKey>();

	// Token: 0x0400349F RID: 13471
	[ReorderableList]
	[Tooltip("On discovery, apply these data keys to the 'to' evidence.")]
	public List<Evidence.DataKey> applyToKeysOnDiscovery = new List<Evidence.DataKey>();

	// Token: 0x040034A0 RID: 13472
	[InfoBox("When either of the connecting evidence has this trigger applied, the fact will become 'discovered'", 0)]
	public List<Evidence.Discovery> discoveryTriggers = new List<Evidence.Discovery>();

	// Token: 0x040034A1 RID: 13473
	[Header("Misc.")]
	[Tooltip("Use this to rank facts within the facts list (lowest displayed first).")]
	[Range(0f, 10f)]
	public int factRank = 5;
}
