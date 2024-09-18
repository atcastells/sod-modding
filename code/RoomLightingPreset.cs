using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007C5 RID: 1989
[CreateAssetMenu(fileName = "roomlighting_data", menuName = "Database/Room Lighting Preset")]
public class RoomLightingPreset : SoCustomComparison
{
	// Token: 0x04003CF8 RID: 15608
	public bool disable;

	// Token: 0x04003CF9 RID: 15609
	[Header("Light Objects")]
	[ReorderableList]
	public List<InteractablePreset> lightObjects = new List<InteractablePreset>();

	// Token: 0x04003CFA RID: 15610
	public LightingPreset lightingPreset;

	// Token: 0x04003CFB RID: 15611
	[ReorderableList]
	[Header("Room Compatibility")]
	public List<RoomConfiguration> roomCompatibility = new List<RoomConfiguration>();

	// Token: 0x04003CFC RID: 15612
	public int minimumRoomSize = 1;

	// Token: 0x04003CFD RID: 15613
	public int maximumRoomSize = 9999;

	// Token: 0x04003CFE RID: 15614
	[Header("Building Compatibility")]
	public List<BuildingPreset> onlyAllowInBuildings = new List<BuildingPreset>();

	// Token: 0x04003CFF RID: 15615
	public List<BuildingPreset> banFromBuildings = new List<BuildingPreset>();

	// Token: 0x04003D00 RID: 15616
	public RoomLightingPreset.StairwellLightRule stairwellRule;

	// Token: 0x04003D01 RID: 15617
	[ReorderableList]
	[Header("Design Style Compatibility")]
	public List<DesignStylePreset> designStyleCompatibility = new List<DesignStylePreset>();

	// Token: 0x04003D02 RID: 15618
	[Header("Ceiling Fan Compatibility")]
	[ReorderableList]
	public List<GameObject> ceilingFans = new List<GameObject>();

	// Token: 0x04003D03 RID: 15619
	[Header("Misc.")]
	[Tooltip("How often these appear compared to others")]
	public int frequency = 1;

	// Token: 0x020007C6 RID: 1990
	public enum StairwellLightRule
	{
		// Token: 0x04003D05 RID: 15621
		noStairwells,
		// Token: 0x04003D06 RID: 15622
		onlyStairwells,
		// Token: 0x04003D07 RID: 15623
		either
	}
}
