using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000706 RID: 1798
[CreateAssetMenu(fileName = "district_data", menuName = "Database/District Preset")]
public class DistrictPreset : SoCustomComparison
{
	// Token: 0x06002535 RID: 9525 RVA: 0x001E53E0 File Offset: 0x001E35E0
	[Button(null, 0)]
	public void CopyFrom()
	{
		if (this.copyFrom != null)
		{
			this.ethnicityFrequencyModifiers = new List<SocialStatistics.EthnicityFrequency>(this.copyFrom.ethnicityFrequencyModifiers);
			this.aliterationWeight = this.copyFrom.aliterationWeight;
			this.prefixOrSuffixChance = this.copyFrom.prefixOrSuffixChance;
			this.prefixList = new List<string>(this.copyFrom.prefixList);
			this.mainChance = this.copyFrom.mainChance;
			this.mainNamingList = new List<string>(this.copyFrom.mainNamingList);
			this.suffixList = new List<string>(this.copyFrom.suffixList);
		}
	}

	// Token: 0x040033B6 RID: 13238
	[Tooltip("How important is it to generate this district first")]
	[Header("Generation")]
	public Vector2 generationPriority;

	// Token: 0x040033B7 RID: 13239
	[Tooltip("Can there be more than 1 of these districts?")]
	public bool limitToOne = true;

	// Token: 0x040033B8 RID: 13240
	[Range(0.1f, 0.5f)]
	[Tooltip("Distict size as ratio of the city")]
	public float cityRatio = 0.2f;

	// Token: 0x040033B9 RID: 13241
	[Tooltip("Hard minimum size")]
	public int minimumSize = 1;

	// Token: 0x040033BA RID: 13242
	[Tooltip("Hard maximum size")]
	public int maximumSize = 6;

	// Token: 0x040033BB RID: 13243
	[Tooltip("This district must be located on the coast")]
	public bool mustBeOnCoast;

	// Token: 0x040033BC RID: 13244
	[Tooltip("How important is it that this district is located near the centre of the city?")]
	[Range(-0.5f, 0.5f)]
	public float centreWeighting;

	// Token: 0x040033BD RID: 13245
	[Header("Naming")]
	[Tooltip("Chance of alliteration with prefix. This will add words with the same letter to the suffix to increase the chances of picking them by this amount")]
	[Range(0f, 15f)]
	public int aliterationWeight = 1;

	// Token: 0x040033BE RID: 13246
	[Range(0f, 1f)]
	[Space(5f)]
	public float prefixOrSuffixChance = 0.8f;

	// Token: 0x040033BF RID: 13247
	[Tooltip("Use this name list to pick a prefix")]
	[ReorderableList]
	public List<string> prefixList = new List<string>();

	// Token: 0x040033C0 RID: 13248
	[Range(0f, 1f)]
	public float mainChance = 1f;

	// Token: 0x040033C1 RID: 13249
	[Tooltip("Use this name list to pick a main name")]
	[ReorderableList]
	public List<string> mainNamingList = new List<string>();

	// Token: 0x040033C2 RID: 13250
	[ReorderableList]
	[Tooltip("Append a random selection of this suffix list to the name")]
	public List<string> suffixList = new List<string>();

	// Token: 0x040033C3 RID: 13251
	[Header("Composition")]
	public BuildingPreset.Density minimumDensity;

	// Token: 0x040033C4 RID: 13252
	public BuildingPreset.Density maximumDensity = BuildingPreset.Density.veryHigh;

	// Token: 0x040033C5 RID: 13253
	public BuildingPreset.LandValue minimumLandValue;

	// Token: 0x040033C6 RID: 13254
	public BuildingPreset.LandValue maximumLandValue = BuildingPreset.LandValue.veryHigh;

	// Token: 0x040033C7 RID: 13255
	[Space(7f)]
	[Tooltip("Affect the ethnicity of the citizens in this district...")]
	public bool affectEthnicity;

	// Token: 0x040033C8 RID: 13256
	[EnableIf("affectEthnicity")]
	public List<SocialStatistics.EthnicityFrequency> ethnicityFrequencyModifiers = new List<SocialStatistics.EthnicityFrequency>();

	// Token: 0x040033C9 RID: 13257
	[Header("Environment")]
	public SessionData.SceneProfile sceneProfile;

	// Token: 0x040033CA RID: 13258
	[Tooltip("Change street light area colours")]
	public bool alterStreetAreaLighting = true;

	// Token: 0x040033CB RID: 13259
	[EnableIf("alterStreetAreaLighting")]
	public List<Color> possibleColours = new List<Color>();

	// Token: 0x040033CC RID: 13260
	[EnableIf("alterStreetAreaLighting")]
	[Tooltip("This is used in combination with the following to adjust street area lighting")]
	public DistrictPreset.AffectStreetAreaLights lightOperation;

	// Token: 0x040033CD RID: 13261
	[EnableIf("alterStreetAreaLighting")]
	public float lightAmount = 1f;

	// Token: 0x040033CE RID: 13262
	[Tooltip("This is added to brightness")]
	[EnableIf("alterStreetAreaLighting")]
	public float brightnessModifier;

	// Token: 0x040033CF RID: 13263
	[Header("Debug")]
	public DistrictPreset copyFrom;

	// Token: 0x02000707 RID: 1799
	public enum AffectStreetAreaLights
	{
		// Token: 0x040033D1 RID: 13265
		lerp,
		// Token: 0x040033D2 RID: 13266
		multiply,
		// Token: 0x040033D3 RID: 13267
		add
	}
}
