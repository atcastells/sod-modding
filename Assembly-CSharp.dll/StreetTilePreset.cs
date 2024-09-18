using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007D7 RID: 2007
[CreateAssetMenu(fileName = "streettile_data", menuName = "Database/Street Tile Preset")]
public class StreetTilePreset : SoCustomComparison
{
	// Token: 0x04003DC1 RID: 15809
	[Header("Setup")]
	public StreetTilePreset.StreetSection sectionType;

	// Token: 0x04003DC2 RID: 15810
	[ReorderableList]
	public List<StreetTilePreset.StreetSectionModel> prefabList = new List<StreetTilePreset.StreetSectionModel>();

	// Token: 0x020007D8 RID: 2008
	[Serializable]
	public class StreetSectionModel
	{
		// Token: 0x04003DC3 RID: 15811
		public GameObject prefab;

		// Token: 0x04003DC4 RID: 15812
		[Tooltip("When raindrops are disabled, use this material...")]
		public Material normalMaterial;

		// Token: 0x04003DC5 RID: 15813
		[Tooltip("When raindrops are enabled, use this material...")]
		public Material rainMaterial;
	}

	// Token: 0x020007D9 RID: 2009
	public enum StreetSection
	{
		// Token: 0x04003DC7 RID: 15815
		streetLong,
		// Token: 0x04003DC8 RID: 15816
		streetShort,
		// Token: 0x04003DC9 RID: 15817
		streetInsideCorner,
		// Token: 0x04003DCA RID: 15818
		streetJunctionCorner,
		// Token: 0x04003DCB RID: 15819
		streetOutsideCorner,
		// Token: 0x04003DCC RID: 15820
		joinerLong,
		// Token: 0x04003DCD RID: 15821
		joinerShort
	}
}
