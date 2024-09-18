using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006D8 RID: 1752
[CreateAssetMenu(fileName = "clothes_data", menuName = "Database/Clothing Item")]
public class ClothesPreset : SoCustomComparison
{
	// Token: 0x04003251 RID: 12881
	[ReorderableList]
	[Tooltip("Covers these components: If an anchor here is NOT covered by this outfit, it will look for other elements in outfits.")]
	[Header("Setup")]
	public List<CitizenOutfitController.CharacterAnchor> covers = new List<CitizenOutfitController.CharacterAnchor>();

	// Token: 0x04003252 RID: 12882
	[Space(7f)]
	[ReorderableList]
	public List<ClothesPreset.OutfitCategory> outfitCategories = new List<ClothesPreset.OutfitCategory>();

	// Token: 0x04003253 RID: 12883
	[ReorderableList]
	public List<Human.Gender> suitableForGenders = new List<Human.Gender>();

	// Token: 0x04003254 RID: 12884
	[ReorderableList]
	public List<Descriptors.BuildType> suitableForBuilds = new List<Descriptors.BuildType>();

	// Token: 0x04003255 RID: 12885
	public List<ClothesPreset.ClothesTags> tags = new List<ClothesPreset.ClothesTags>();

	// Token: 0x04003256 RID: 12886
	[Tooltip("If true enable facial-feature specific setup")]
	public bool enableFacialFeatureSetup;

	// Token: 0x04003257 RID: 12887
	[ReorderableList]
	public List<Descriptors.HairStyle> suitableForHairstyle = new List<Descriptors.HairStyle>();

	// Token: 0x04003258 RID: 12888
	[Header("Head")]
	public bool isHead;

	// Token: 0x04003259 RID: 12889
	[EnableIf("isHead")]
	public Vector3 pupilsOffset = new Vector3(0f, 0.15f, 0.1205f);

	// Token: 0x0400325A RID: 12890
	[EnableIf("isHead")]
	public Vector3 eyebrowsOffset = new Vector3(0f, 0.1719f, 0.133f);

	// Token: 0x0400325B RID: 12891
	[EnableIf("isHead")]
	public Vector3 mouthOffset = new Vector3(0f, 0.045f, 0.151f);

	// Token: 0x0400325C RID: 12892
	[Header("Hair")]
	[Tooltip("This needs to be true for the game to render both the hair and hat")]
	public bool hatRenderCompatible;

	// Token: 0x0400325D RID: 12893
	[Tooltip("Exclude these types of hats from compatibility...")]
	[EnableIf("hatRenderCompatible")]
	public List<ClothesPreset> excludeHats = new List<ClothesPreset>();

	// Token: 0x0400325E RID: 12894
	[Header("Hat")]
	public ClothesPreset.HairRenderSetting hairRenderMode;

	// Token: 0x0400325F RID: 12895
	[Header("Feet")]
	public bool setFootwear;

	// Token: 0x04003260 RID: 12896
	[EnableIf("setFootwear")]
	public Human.ShoeType footwear = Human.ShoeType.barefoot;

	// Token: 0x04003261 RID: 12897
	[Header("Compatibility")]
	[Tooltip("Controls which clothing will be loaded first. If this is important to the outfit, eg wearing a coat outdoors, increase the priority.")]
	[Range(0f, 5f)]
	public int priority = 2;

	// Token: 0x04003262 RID: 12898
	[Tooltip("Only choose this preset if the model can display all elements in the 'models' section below. Only applies when using clothing elements not from the category (eg using casual for outdoors casual)")]
	public bool onlyChooseIfAllModelPartsAreAvailable;

	// Token: 0x04003263 RID: 12899
	[Tooltip("This cannot be chosen if these existing clothes are chosen")]
	[ReorderableList]
	public List<ClothesPreset.IncompatibilitySetting> incompatibility = new List<ClothesPreset.IncompatibilitySetting>();

	// Token: 0x04003264 RID: 12900
	[Tooltip("The citizen/company must have at least this much wealth to have this outfit")]
	public bool useWealthValues;

	// Token: 0x04003265 RID: 12901
	[EnableIf("useWealthValues")]
	[Range(0f, 1f)]
	public float minimumWealth;

	// Token: 0x04003266 RID: 12902
	[Range(0f, 1f)]
	[EnableIf("useWealthValues")]
	public float maximumWealth = 1f;

	// Token: 0x04003267 RID: 12903
	[Header("Colours")]
	[Space(7f)]
	public ClothesPreset.ClothingColourSource baseColourSource = ClothesPreset.ClothingColourSource.white;

	// Token: 0x04003268 RID: 12904
	[ReorderableList]
	public List<ColourPalettePreset> colourBase = new List<ColourPalettePreset>();

	// Token: 0x04003269 RID: 12905
	[Space(5f)]
	public ClothesPreset.ClothingColourSource colour1Source;

	// Token: 0x0400326A RID: 12906
	[ReorderableList]
	public List<ColourPalettePreset> colour1 = new List<ColourPalettePreset>();

	// Token: 0x0400326B RID: 12907
	[Space(5f)]
	public ClothesPreset.ClothingColourSource colour2Source;

	// Token: 0x0400326C RID: 12908
	[ReorderableList]
	public List<ColourPalettePreset> colour2 = new List<ColourPalettePreset>();

	// Token: 0x0400326D RID: 12909
	[Space(5f)]
	public ClothesPreset.ClothingColourSource colour3Source;

	// Token: 0x0400326E RID: 12910
	[ReorderableList]
	public List<ColourPalettePreset> colour3 = new List<ColourPalettePreset>();

	// Token: 0x0400326F RID: 12911
	[Header("Suited Personality")]
	[Tooltip("Include this when using citizen stats to pick a style")]
	public bool includeInPersonalityMatching = true;

	// Token: 0x04003270 RID: 12912
	[Range(0f, 10f)]
	[EnableIf("includeInPersonalityMatching")]
	[Tooltip("The base chance of selecting this item of clothing. This is added to by HEXACO and Traits below...")]
	public int baseChance = 1;

	// Token: 0x04003271 RID: 12913
	[Space(7f)]
	[InfoBox("If enabled: The below HEXACO values will combine for a score of 1 to 10: this will be used to calculate the likihood of this being chosen vs others.", 0)]
	[Tooltip("Use the below hexaco values to match to personality.")]
	public bool useHEXACO;

	// Token: 0x04003272 RID: 12914
	public HEXACO hexaco;

	// Token: 0x04003273 RID: 12915
	[Space(7f)]
	[InfoBox("If enabled: The below traits will be used to calculate the likihood of this being chosen vs others.", 0)]
	[Tooltip("Use character traits to match to personality.")]
	public bool useTraits;

	// Token: 0x04003274 RID: 12916
	[ReorderableList]
	public List<ClothesPreset.TraitPickRule> characterTraits = new List<ClothesPreset.TraitPickRule>();

	// Token: 0x04003275 RID: 12917
	[ReorderableList]
	[Header("Models")]
	[InfoBox("Note: The 'covers anchor' box will ensure only this model will be loaded to cover this anchor. If you want more than one model to be loaded for this anchor, make sure one is unchecked.", 0)]
	public List<ClothesPreset.ModelSettings> models = new List<ClothesPreset.ModelSettings>();

	// Token: 0x020006D9 RID: 1753
	[Serializable]
	public class MaterialSettings
	{
		// Token: 0x04003276 RID: 12918
		public Color colour;

		// Token: 0x04003277 RID: 12919
		[Range(1f, 5f)]
		public int weighting = 3;
	}

	// Token: 0x020006DA RID: 1754
	[Serializable]
	public class ModelSettings
	{
		// Token: 0x04003278 RID: 12920
		public GameObject prefab;

		// Token: 0x04003279 RID: 12921
		public CitizenOutfitController.CharacterAnchor anchor;

		// Token: 0x0400327A RID: 12922
		public Vector3 offsetPosition;

		// Token: 0x0400327B RID: 12923
		public Vector3 offsetEuler;

		// Token: 0x0400327C RID: 12924
		public bool exclusiveAnchorModel = true;
	}

	// Token: 0x020006DB RID: 1755
	public enum OutfitCategory
	{
		// Token: 0x0400327E RID: 12926
		casual,
		// Token: 0x0400327F RID: 12927
		work,
		// Token: 0x04003280 RID: 12928
		smart,
		// Token: 0x04003281 RID: 12929
		outdoorsCasual,
		// Token: 0x04003282 RID: 12930
		outdoorsWork,
		// Token: 0x04003283 RID: 12931
		outdoorsSmart,
		// Token: 0x04003284 RID: 12932
		undressed,
		// Token: 0x04003285 RID: 12933
		bed,
		// Token: 0x04003286 RID: 12934
		underwear
	}

	// Token: 0x020006DC RID: 1756
	public enum ClothingColourSource
	{
		// Token: 0x04003288 RID: 12936
		none,
		// Token: 0x04003289 RID: 12937
		garment,
		// Token: 0x0400328A RID: 12938
		skin,
		// Token: 0x0400328B RID: 12939
		white,
		// Token: 0x0400328C RID: 12940
		hair,
		// Token: 0x0400328D RID: 12941
		underneathColour1,
		// Token: 0x0400328E RID: 12942
		underneathColour2,
		// Token: 0x0400328F RID: 12943
		underneathColour3,
		// Token: 0x04003290 RID: 12944
		workUniformColour
	}

	// Token: 0x020006DD RID: 1757
	public enum ClothesTags
	{
		// Token: 0x04003292 RID: 12946
		longGarment,
		// Token: 0x04003293 RID: 12947
		noLongGarments
	}

	// Token: 0x020006DE RID: 1758
	public enum HairRenderSetting
	{
		// Token: 0x04003295 RID: 12949
		renderHatCompatibleHair,
		// Token: 0x04003296 RID: 12950
		renderAllHair,
		// Token: 0x04003297 RID: 12951
		dontRenderAnyHair
	}

	// Token: 0x020006DF RID: 1759
	public enum Incompatibility
	{
		// Token: 0x04003299 RID: 12953
		inAnyCategory,
		// Token: 0x0400329A RID: 12954
		inThisCategory
	}

	// Token: 0x020006E0 RID: 1760
	[Serializable]
	public class IncompatibilitySetting
	{
		// Token: 0x0400329B RID: 12955
		public ClothesPreset.Incompatibility incompatibleIf;

		// Token: 0x0400329C RID: 12956
		public List<ClothesPreset.ClothesTags> tags;

		// Token: 0x0400329D RID: 12957
		public ClothesPreset featured;
	}

	// Token: 0x020006E1 RID: 1761
	[Serializable]
	public class TraitPickRule
	{
		// Token: 0x0400329E RID: 12958
		public CharacterTrait.RuleType rule;

		// Token: 0x0400329F RID: 12959
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x040032A0 RID: 12960
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		[ShowIf("isTrait")]
		public bool mustPassForApplication = true;

		// Token: 0x040032A1 RID: 12961
		[Tooltip("If the rules match, then apply this to the base chance...")]
		[ShowIf("isTrait")]
		[Range(-10f, 10f)]
		public int addChance;
	}
}
