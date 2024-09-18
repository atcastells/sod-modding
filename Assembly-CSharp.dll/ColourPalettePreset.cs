using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006E3 RID: 1763
[CreateAssetMenu(fileName = "colourpalette_data", menuName = "Database/Colour Palette")]
public class ColourPalettePreset : SoCustomComparison
{
	// Token: 0x040032A3 RID: 12963
	[Header("Colours")]
	[ReorderableList]
	public List<ColourPalettePreset.MaterialSettings> colours = new List<ColourPalettePreset.MaterialSettings>();

	// Token: 0x040032A4 RID: 12964
	[Header("Suited Personality")]
	public HEXACO hexaco;

	// Token: 0x020006E4 RID: 1764
	[Serializable]
	public class MaterialSettings
	{
		// Token: 0x040032A5 RID: 12965
		public Color colour;

		// Token: 0x040032A6 RID: 12966
		[Range(1f, 5f)]
		public int weighting = 3;
	}
}
