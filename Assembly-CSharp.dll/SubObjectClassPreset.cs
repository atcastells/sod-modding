using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007DA RID: 2010
[CreateAssetMenu(fileName = "subobject_data", menuName = "Database/Sub Object Class")]
public class SubObjectClassPreset : SoCustomComparison
{
	// Token: 0x04003DCE RID: 15822
	[Header("Spawning")]
	public bool limitCountPerObject;

	// Token: 0x04003DCF RID: 15823
	[EnableIf("limitCountPerObject")]
	[Tooltip("If true only one of these types will be spawned per object")]
	public int maxPerObject = 1;

	// Token: 0x04003DD0 RID: 15824
	[Tooltip("The chance of spawning here on a per-object basis")]
	[Range(0f, 1f)]
	public float perObjectSpawnChance = 1f;

	// Token: 0x04003DD1 RID: 15825
	[Tooltip("The chance of spawning here on a per-instance basis")]
	[Range(0f, 1f)]
	public float perInstanceSpawnChance = 1f;

	// Token: 0x04003DD2 RID: 15826
	[Tooltip("Added to the perInstanceSpawnChance as modifiers, uses test type found on the furniture preset")]
	[ReorderableList]
	public List<CharacterTrait.TraitPickRule> perInstanceModifiers = new List<CharacterTrait.TraitPickRule>();

	// Token: 0x04003DD3 RID: 15827
	public SubObjectClassPreset.PlacementTypeLimit typeLimit;

	// Token: 0x020007DB RID: 2011
	public enum PlacementTypeLimit
	{
		// Token: 0x04003DD5 RID: 15829
		all,
		// Token: 0x04003DD6 RID: 15830
		companyOnly,
		// Token: 0x04003DD7 RID: 15831
		homeOnly,
		// Token: 0x04003DD8 RID: 15832
		indoorsOnly,
		// Token: 0x04003DD9 RID: 15833
		outdoorsOnly
	}
}
