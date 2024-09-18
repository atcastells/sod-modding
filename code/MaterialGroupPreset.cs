using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200077A RID: 1914
[CreateAssetMenu(fileName = "materialgroup_data", menuName = "Database/Decor/Material Group Preset")]
public class MaterialGroupPreset : SoCustomComparison
{
	// Token: 0x04003997 RID: 14743
	[Header("Material")]
	public Material material;

	// Token: 0x04003998 RID: 14744
	[Header("Material Variations")]
	[ReorderableList]
	public List<MaterialGroupPreset.MaterialVariation> variations = new List<MaterialGroupPreset.MaterialVariation>();

	// Token: 0x04003999 RID: 14745
	[Header("Material Properties")]
	[Range(0f, 1f)]
	public float concrete = 1f;

	// Token: 0x0400399A RID: 14746
	[Range(0f, 1f)]
	public float plaster;

	// Token: 0x0400399B RID: 14747
	[Range(0f, 1f)]
	public float wood;

	// Token: 0x0400399C RID: 14748
	[Range(0f, 1f)]
	public float carpet;

	// Token: 0x0400399D RID: 14749
	[Range(0f, 1f)]
	public float tile;

	// Token: 0x0400399E RID: 14750
	[Range(0f, 1f)]
	public float metal;

	// Token: 0x0400399F RID: 14751
	[Range(0f, 1f)]
	public float glass;

	// Token: 0x040039A0 RID: 14752
	[Range(0f, 1f)]
	public float fabric;

	// Token: 0x040039A1 RID: 14753
	[Tooltip("If this is assigned and the node has no floor, instead use this material.")]
	public MaterialGroupPreset noFloorReplacement;

	// Token: 0x040039A2 RID: 14754
	public bool allowFootprints;

	// Token: 0x040039A3 RID: 14755
	[Range(-1f, 1f)]
	public float affectFootprintDirt;

	// Token: 0x040039A4 RID: 14756
	[Tooltip("If this material is grubby, use this multiplier to add to footprint dirt")]
	public float grubFootprintDirtMultiplier = 0.05f;

	// Token: 0x040039A5 RID: 14757
	[Header("Suitability")]
	public MaterialGroupPreset.MaterialType materialType = MaterialGroupPreset.MaterialType.floor;

	// Token: 0x040039A6 RID: 14758
	[Range(0f, 1f)]
	public float minimumWealth;

	// Token: 0x040039A7 RID: 14759
	[ReorderableList]
	public List<MaterialGroupPreset.MaterialSettings> designStyles = new List<MaterialGroupPreset.MaterialSettings>();

	// Token: 0x040039A8 RID: 14760
	[ReorderableList]
	[Tooltip("The furniture is only allowed in these room types")]
	public List<RoomTypeFilter> allowedRoomFilters = new List<RoomTypeFilter>();

	// Token: 0x040039A9 RID: 14761
	[Header("In-Game")]
	public bool purchasable = true;

	// Token: 0x040039AA RID: 14762
	public int price = 10;

	// Token: 0x040039AB RID: 14763
	public Sprite decorSprite;

	// Token: 0x0200077B RID: 1915
	[Serializable]
	public class MaterialSettings
	{
		// Token: 0x040039AC RID: 14764
		public DesignStylePreset designStyle;

		// Token: 0x040039AD RID: 14765
		[Range(1f, 5f)]
		public int weighting = 3;
	}

	// Token: 0x0200077C RID: 1916
	public enum MaterialType
	{
		// Token: 0x040039AF RID: 14767
		walls,
		// Token: 0x040039B0 RID: 14768
		floor,
		// Token: 0x040039B1 RID: 14769
		ceiling,
		// Token: 0x040039B2 RID: 14770
		other
	}

	// Token: 0x0200077D RID: 1917
	public enum MaterialColour
	{
		// Token: 0x040039B4 RID: 14772
		anyPrimary,
		// Token: 0x040039B5 RID: 14773
		anySecondary,
		// Token: 0x040039B6 RID: 14774
		anyPrimaryOrNeutral,
		// Token: 0x040039B7 RID: 14775
		anySecondaryOrNeutral,
		// Token: 0x040039B8 RID: 14776
		any1,
		// Token: 0x040039B9 RID: 14777
		any2,
		// Token: 0x040039BA RID: 14778
		any1OrNeutral,
		// Token: 0x040039BB RID: 14779
		any2OrNeutral,
		// Token: 0x040039BC RID: 14780
		any,
		// Token: 0x040039BD RID: 14781
		primary1,
		// Token: 0x040039BE RID: 14782
		primary2,
		// Token: 0x040039BF RID: 14783
		secondary1,
		// Token: 0x040039C0 RID: 14784
		secondary2,
		// Token: 0x040039C1 RID: 14785
		neutral,
		// Token: 0x040039C2 RID: 14786
		wood,
		// Token: 0x040039C3 RID: 14787
		none,
		// Token: 0x040039C4 RID: 14788
		anyPrimaryOrSecondary
	}

	// Token: 0x0200077E RID: 1918
	[Serializable]
	public class MaterialVariation
	{
		// Token: 0x040039C5 RID: 14789
		public string name;

		// Token: 0x040039C6 RID: 14790
		public MaterialGroupPreset.MaterialColour main = MaterialGroupPreset.MaterialColour.none;

		// Token: 0x040039C7 RID: 14791
		public MaterialGroupPreset.MaterialColour colour1 = MaterialGroupPreset.MaterialColour.any;

		// Token: 0x040039C8 RID: 14792
		public MaterialGroupPreset.MaterialColour colour2 = MaterialGroupPreset.MaterialColour.any;

		// Token: 0x040039C9 RID: 14793
		public MaterialGroupPreset.MaterialColour colour3 = MaterialGroupPreset.MaterialColour.any;
	}
}
