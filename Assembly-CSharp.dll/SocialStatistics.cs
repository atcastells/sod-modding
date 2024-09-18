using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000806 RID: 2054
public class SocialStatistics : MonoBehaviour
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06002622 RID: 9762 RVA: 0x001EA628 File Offset: 0x001E8828
	public static SocialStatistics Instance
	{
		get
		{
			return SocialStatistics._instance;
		}
	}

	// Token: 0x06002623 RID: 9763 RVA: 0x001EA62F File Offset: 0x001E882F
	private void Awake()
	{
		if (SocialStatistics._instance != null && SocialStatistics._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SocialStatistics._instance = this;
	}

	// Token: 0x04004455 RID: 17493
	[Header("Gender/Sexuality")]
	[Tooltip("The scale in the centre of Female:Male float that applies to citizens that identify as non-binary")]
	public float genderNonBinaryThreshold = 0.01f;

	// Token: 0x04004456 RID: 17494
	[Tooltip("How many citizens identify as something other than their birth gender?")]
	public float transThreshold = 0.01f;

	// Token: 0x04004457 RID: 17495
	[Tooltip("Sexuality threshold for being attracted to opposite sex (straight)")]
	public float sexualityStraightThreshold = 0.4f;

	// Token: 0x04004458 RID: 17496
	[Tooltip("Sexuality threshold for being attracted to same sex (gay)")]
	public float sexualityGayThreshold = 0.9f;

	// Token: 0x04004459 RID: 17497
	[Tooltip("Chance of being asexual if attracted to neither sex")]
	public float asexualChance = 0.7f;

	// Token: 0x0400445A RID: 17498
	[Space(7f)]
	public CharacterTrait maleTrait;

	// Token: 0x0400445B RID: 17499
	public CharacterTrait femaleTrait;

	// Token: 0x0400445C RID: 17500
	public CharacterTrait nbTrait;

	// Token: 0x0400445D RID: 17501
	public CharacterTrait AttractedToMaleTrait;

	// Token: 0x0400445E RID: 17502
	public CharacterTrait AttractedToFemaleTrait;

	// Token: 0x0400445F RID: 17503
	public CharacterTrait AttractedToNBTrait;

	// Token: 0x04004460 RID: 17504
	public CharacterTrait relationshipTrait;

	// Token: 0x04004461 RID: 17505
	[Space(7f)]
	public List<Color> lipstickColours = new List<Color>();

	// Token: 0x04004462 RID: 17506
	[Tooltip("The higher the rank, the more likely it is that the person is older: 19-21, 22-26, 27-31, 32-36, 37-41, 42-46, 47-51, 52-56, 57-61, 62-66, 67+")]
	[Header("Demographics")]
	public int[] ageRanges = new int[]
	{
		19,
		22,
		27,
		32,
		37,
		42,
		47,
		52,
		57,
		62
	};

	// Token: 0x04004463 RID: 17507
	[Header("Ethnicity")]
	public List<SocialStatistics.EthnicityFrequency> ethnicityFrequencies = new List<SocialStatistics.EthnicityFrequency>();

	// Token: 0x04004464 RID: 17508
	public int chanceOf2ndEthnicity = 10;

	// Token: 0x04004465 RID: 17509
	public float districtEthnictiyDominanceMultiplier = 0.82f;

	// Token: 0x04004466 RID: 17510
	[Header("Ethnicity Classes")]
	public List<SocialStatistics.EthnicityStats> ethnicityStats = new List<SocialStatistics.EthnicityStats>();

	// Token: 0x04004467 RID: 17511
	[Header("Physical Build")]
	[Tooltip("Real-world average height in cm")]
	public float averageHeight = 175.4f;

	// Token: 0x04004468 RID: 17512
	[Tooltip("Real-world average weight in kg")]
	public float averageWeight = 70.8f;

	// Token: 0x04004469 RID: 17513
	[Tooltip("Height min/max thresholds in cm")]
	public Vector2 heightMinMax = Vector2.one;

	// Token: 0x0400446A RID: 17514
	[Space(7f)]
	public int skinnyRatio = 15;

	// Token: 0x0400446B RID: 17515
	public int averageRatio = 35;

	// Token: 0x0400446C RID: 17516
	public int overweightRatio = 20;

	// Token: 0x0400446D RID: 17517
	public int muscleyRatio = 10;

	// Token: 0x0400446E RID: 17518
	[Header("Blood Group")]
	public float bloodOPosRatio = 38f;

	// Token: 0x0400446F RID: 17519
	public float bloodAPosRatio = 34f;

	// Token: 0x04004470 RID: 17520
	public float bloodBPosRatio = 9f;

	// Token: 0x04004471 RID: 17521
	public float bloodONegRatio = 7f;

	// Token: 0x04004472 RID: 17522
	public float bloodANegRatio = 6f;

	// Token: 0x04004473 RID: 17523
	public float bloodABPosRatio = 3f;

	// Token: 0x04004474 RID: 17524
	public float bloodBNegRatio = 2f;

	// Token: 0x04004475 RID: 17525
	public float bloodABNegRatio = 1f;

	// Token: 0x04004476 RID: 17526
	[Header("Hair")]
	[ReorderableList]
	public List<SocialStatistics.HairSetting> hairColourSettings = new List<SocialStatistics.HairSetting>();

	// Token: 0x04004477 RID: 17527
	[Space(7f)]
	public int RedHairRatio = 10;

	// Token: 0x04004478 RID: 17528
	public int blueHairRatio = 1;

	// Token: 0x04004479 RID: 17529
	public int greenHairRatio = 1;

	// Token: 0x0400447A RID: 17530
	public int purpleHairRatio = 1;

	// Token: 0x0400447B RID: 17531
	public int pinkHairRatio = 2;

	// Token: 0x0400447C RID: 17532
	[Header("Facial Features")]
	public int scaringRatio = 25;

	// Token: 0x0400447D RID: 17533
	public int menWithBeards = 36;

	// Token: 0x0400447E RID: 17534
	public int menWithMoustaches = 11;

	// Token: 0x0400447F RID: 17535
	public int piercingRatio = 25;

	// Token: 0x04004480 RID: 17536
	public int TattooRatio = 25;

	// Token: 0x04004481 RID: 17537
	public int glassesRatio = 50;

	// Token: 0x04004482 RID: 17538
	public int moleRatio = 2;

	// Token: 0x04004483 RID: 17539
	public int frecklesRatio = 5;

	// Token: 0x04004484 RID: 17540
	[Header("Society")]
	public float seriousRelationshipsRatio = 0.5f;

	// Token: 0x04004485 RID: 17541
	[Header("Slang Defaults")]
	[ReorderableList]
	[Tooltip("A default slang greeting to be used on anyone in a casual manor")]
	public List<string> slangGreetingDefault;

	// Token: 0x04004486 RID: 17542
	[ReorderableList]
	[Tooltip("Similar to above, but male specific (eg. 'bro')")]
	public List<string> slangGreetingMale;

	// Token: 0x04004487 RID: 17543
	[ReorderableList]
	[Tooltip("Similar to above, but female specific")]
	public List<string> slangGreetingFemale;

	// Token: 0x04004488 RID: 17544
	[ReorderableList]
	[Tooltip("Slang greeting for a lover")]
	public List<string> slangGreetingLover;

	// Token: 0x04004489 RID: 17545
	[ReorderableList]
	[Tooltip("Slang curse word")]
	public List<string> slangCurse;

	// Token: 0x0400448A RID: 17546
	[ReorderableList]
	[Tooltip("Slang curse noun word")]
	public List<string> slangCurseNoun;

	// Token: 0x0400448B RID: 17547
	[Tooltip("Slang praise noun word")]
	[ReorderableList]
	public List<string> slangPraiseNoun;

	// Token: 0x0400448C RID: 17548
	[ReorderableList]
	[Header("Fav Colours Pool")]
	public List<Color> favouriteColoursPool = new List<Color>();

	// Token: 0x0400448D RID: 17549
	private static SocialStatistics _instance;

	// Token: 0x02000807 RID: 2055
	[Serializable]
	public class EthnicityFrequency : IComparable<SocialStatistics.EthnicityFrequency>
	{
		// Token: 0x06002625 RID: 9765 RVA: 0x001EA812 File Offset: 0x001E8A12
		public int CompareTo(SocialStatistics.EthnicityFrequency otherObject)
		{
			return this.frequency.CompareTo(otherObject.frequency);
		}

		// Token: 0x0400448E RID: 17550
		public Descriptors.EthnicGroup ethnicity;

		// Token: 0x0400448F RID: 17551
		public int frequency;
	}

	// Token: 0x02000808 RID: 2056
	[Serializable]
	public class HairSetting
	{
		// Token: 0x04004490 RID: 17552
		public Descriptors.HairColour colour;

		// Token: 0x04004491 RID: 17553
		public Color hairColourRange1 = Color.white;

		// Token: 0x04004492 RID: 17554
		public Color hairColourRange2 = Color.black;
	}

	// Token: 0x02000809 RID: 2057
	[Serializable]
	public class EthnicityStats
	{
		// Token: 0x04004493 RID: 17555
		public Descriptors.EthnicGroup group;

		// Token: 0x04004494 RID: 17556
		[Header("Skin")]
		public Color skinColourRange1 = Color.white;

		// Token: 0x04004495 RID: 17557
		public Color skinColourRange2 = Color.black;

		// Token: 0x04004496 RID: 17558
		[Header("Hair Colour")]
		public int blackHairRatio = 22;

		// Token: 0x04004497 RID: 17559
		public int brownHairRatio = 50;

		// Token: 0x04004498 RID: 17560
		public int blondeHairRatio = 22;

		// Token: 0x04004499 RID: 17561
		public int gingerHairRatio = 1;

		// Token: 0x0400449A RID: 17562
		public int RedHairRatio;

		// Token: 0x0400449B RID: 17563
		public int blueHairRatio;

		// Token: 0x0400449C RID: 17564
		public int greenHairRatio;

		// Token: 0x0400449D RID: 17565
		public int purpleHairRatio;

		// Token: 0x0400449E RID: 17566
		public int pinkHairRatio;

		// Token: 0x0400449F RID: 17567
		public int greyHairRatio;

		// Token: 0x040044A0 RID: 17568
		public int whiteHairRatio;

		// Token: 0x040044A1 RID: 17569
		[Header("Hair Type")]
		public int baldHairRatioMale = 3;

		// Token: 0x040044A2 RID: 17570
		public int shortHairRatioMale = 60;

		// Token: 0x040044A3 RID: 17571
		public int longHairRatioMale = 5;

		// Token: 0x040044A4 RID: 17572
		public int baldHairRatioFemale;

		// Token: 0x040044A5 RID: 17573
		public int shortHairRatioFemale = 20;

		// Token: 0x040044A6 RID: 17574
		public int longHairRatioFemale = 35;

		// Token: 0x040044A7 RID: 17575
		[Header("Hair Type")]
		public int straightHairRatioMale = 40;

		// Token: 0x040044A8 RID: 17576
		public int curlyHairRatioMale = 10;

		// Token: 0x040044A9 RID: 17577
		public int balingHairRatioMale = 10;

		// Token: 0x040044AA RID: 17578
		public int messyHairRatioMale = 10;

		// Token: 0x040044AB RID: 17579
		public int styledHairRatioMale = 20;

		// Token: 0x040044AC RID: 17580
		public int mohawkHairRatioMale = 1;

		// Token: 0x040044AD RID: 17581
		public int afroHairRatioMale = 1;

		// Token: 0x040044AE RID: 17582
		public int straightHairRatioFemale = 50;

		// Token: 0x040044AF RID: 17583
		public int curlyHairRatioFemale = 20;

		// Token: 0x040044B0 RID: 17584
		public int balingHairRatioFemale;

		// Token: 0x040044B1 RID: 17585
		public int messyHairRatioFemale = 10;

		// Token: 0x040044B2 RID: 17586
		public int styledHairRatioFemale = 5;

		// Token: 0x040044B3 RID: 17587
		public int mohawkHairRatioFemale = 1;

		// Token: 0x040044B4 RID: 17588
		public int afroHairRatioFemale = 1;

		// Token: 0x040044B5 RID: 17589
		[Header("Eye Colour")]
		public int blueEyesRatio = 35;

		// Token: 0x040044B6 RID: 17590
		public int brownEyesRatio = 30;

		// Token: 0x040044B7 RID: 17591
		public int greenEyesRatio = 30;

		// Token: 0x040044B8 RID: 17592
		public int greyEyesRatio = 5;

		// Token: 0x040044B9 RID: 17593
		[Header("Naming")]
		public bool overrideFirst;

		// Token: 0x040044BA RID: 17594
		public Descriptors.EthnicGroup overrideNameFirst;

		// Token: 0x040044BB RID: 17595
		public bool overrideSur;

		// Token: 0x040044BC RID: 17596
		public Descriptors.EthnicGroup overrideNameSur;

		// Token: 0x040044BD RID: 17597
		[Header("Cultural Similiarities")]
		public List<Descriptors.EthnicGroup> culturalSimilarities = new List<Descriptors.EthnicGroup>();

		// Token: 0x040044BE RID: 17598
		[Header("Traits")]
		public List<CharacterTrait> ethTraits = new List<CharacterTrait>();
	}
}
