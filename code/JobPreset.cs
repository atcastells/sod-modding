using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000762 RID: 1890
[CreateAssetMenu(fileName = "sidejob_data", menuName = "Database/Job Preset")]
public class JobPreset : SoCustomComparison
{
	// Token: 0x0600257E RID: 9598 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyAcquisitionData()
	{
	}

	// Token: 0x0600257F RID: 9599 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyFrequencyData()
	{
	}

	// Token: 0x06002580 RID: 9600 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyStartingScenarios()
	{
	}

	// Token: 0x06002581 RID: 9601 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyItemSpawns()
	{
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyResolveQuestions()
	{
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyIntros()
	{
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyHandIns()
	{
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyAdditionalMainElements()
	{
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyDialogReferences()
	{
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x001E6CE8 File Offset: 0x001E4EE8
	public int GetDifficultyValue()
	{
		if (this.difficultyTag == JobPreset.DifficultyTag.D0)
		{
			return 0;
		}
		if (this.difficultyTag == JobPreset.DifficultyTag.D1)
		{
			return 1;
		}
		if (this.difficultyTag == JobPreset.DifficultyTag.D2A || this.difficultyTag == JobPreset.DifficultyTag.D2B)
		{
			return 2;
		}
		if (this.difficultyTag == JobPreset.DifficultyTag.D3)
		{
			return 3;
		}
		if (this.difficultyTag == JobPreset.DifficultyTag.D4A || this.difficultyTag == JobPreset.DifficultyTag.D4B || this.difficultyTag == JobPreset.DifficultyTag.D4C)
		{
			return 4;
		}
		if (this.difficultyTag == JobPreset.DifficultyTag.D5)
		{
			return 5;
		}
		if (this.difficultyTag == JobPreset.DifficultyTag.D6)
		{
			return 6;
		}
		return 0;
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x001E6D60 File Offset: 0x001E4F60
	public int GetFrequencyForSocialCreditLevel()
	{
		int currentSocialCreditLevel = GameplayController.Instance.GetCurrentSocialCreditLevel();
		return Mathf.Min(Mathf.RoundToInt(this.socialCreditLevelMinSpawnFrequency.Evaluate((float)currentSocialCreditLevel) * (float)Mathf.CeilToInt((float)CityData.Instance.citizenDirectory.Count * this.activePerCitizen)), this.maxJobs);
	}

	// Token: 0x0400388B RID: 14475
	[BoxGroup("Disable")]
	[Tooltip("Disable this in-game completely.")]
	public bool disabled;

	// Token: 0x0400388C RID: 14476
	[Header("Setup")]
	public string caseName;

	// Token: 0x0400388D RID: 14477
	public InteractablePreset jobPosting;

	// Token: 0x0400388E RID: 14478
	[Tooltip("Spawn this subclass. If left empty it will use the base class.")]
	public string subClass = string.Empty;

	// Token: 0x0400388F RID: 14479
	public bool allowSyncDiskRewards = true;

	// Token: 0x04003890 RID: 14480
	[EnableIf("allowSyncDiskRewards")]
	public bool allowBlackMarketSyncDiskRewards = true;

	// Token: 0x04003891 RID: 14481
	public JobPreset.RewardLocation physicalRewardLocation = JobPreset.RewardLocation.postersMailbox;

	// Token: 0x04003892 RID: 14482
	[Tooltip("Generates an item hiden location on acceptance")]
	public bool generateHidingLocation;

	// Token: 0x04003893 RID: 14483
	[Header("Frequency")]
	[InfoBox("The frequency uses the below graph multiplied by the active per citizen value to calculate how many jobs should be spawned...", 0)]
	[Tooltip("Spawn this job according to social credit level")]
	public AnimationCurve socialCreditLevelMinSpawnFrequency;

	// Token: 0x04003894 RID: 14484
	[Tooltip("The number of these jobs that should be active at one time, per citizen.")]
	public float activePerCitizen = 0.01f;

	// Token: 0x04003895 RID: 14485
	[Tooltip("Hard limit on maximum jobs spawned")]
	public int maxJobs = 8;

	// Token: 0x04003896 RID: 14486
	[Tooltip("If posted jobs count is below this, then spawn them immediately")]
	public int immediatePostCountThreshold = 2;

	// Token: 0x04003897 RID: 14487
	[Header("Difficulty")]
	public JobPreset.DifficultyTag difficultyTag;

	// Token: 0x04003898 RID: 14488
	[Header("Characters")]
	public JobPreset.ParticipantCompliancy changePosterDialogCompliancy;

	// Token: 0x04003899 RID: 14489
	public JobPreset.ParticipantCompliancy changePerpDialogCompliancy = JobPreset.ParticipantCompliancy.alwaysFail;

	// Token: 0x0400389A RID: 14490
	[Header("Motives")]
	public List<MotivePreset> purpetratorMotives = new List<MotivePreset>();

	// Token: 0x0400389B RID: 14491
	[Space(7f)]
	[Tooltip("Minus this from the score if the purp and poster live in the same building")]
	public int penaltyForPurpAndPosterSameBuilding = 5;

	// Token: 0x0400389C RID: 14492
	[Header("Starting Scenarios")]
	[Tooltip("Possible starting scenarios for this job")]
	public List<JobPreset.StartingScenario> startingScenarios = new List<JobPreset.StartingScenario>();

	// Token: 0x0400389D RID: 14493
	[Tooltip("Scenarios that will reveal the required information for the task")]
	[Header("Intros")]
	public List<JobPreset.IntroConfig> compatibleIntros = new List<JobPreset.IntroConfig>();

	// Token: 0x0400389E RID: 14494
	[Range(0f, 5f)]
	[Tooltip("How many entries from the general lead pool should we add?")]
	[Header("On Info Acquisition")]
	public int leadPoolData;

	// Token: 0x0400389F RID: 14495
	[InfoBox("Created facts here are automatically also discovered on creation", 0)]
	public List<JobPreset.FactCreation> createFactsOnInformationAcquisition = new List<JobPreset.FactCreation>();

	// Token: 0x040038A0 RID: 14496
	public List<JobPreset.StartingLead> informationAcquisitionLeads = new List<JobPreset.StartingLead>();

	// Token: 0x040038A1 RID: 14497
	[Header("Revenge Objectives")]
	public List<RevengeObjective> revengeObjectives = new List<RevengeObjective>();

	// Token: 0x040038A2 RID: 14498
	[Header("Spawn Items")]
	public List<JobPreset.StartingSpawnItem> spawnItems = new List<JobPreset.StartingSpawnItem>();

	// Token: 0x040038A3 RID: 14499
	[Header("Objectives")]
	public List<Case.ResolveQuestion> resolveQuestions = new List<Case.ResolveQuestion>();

	// Token: 0x040038A4 RID: 14500
	[Header("Additonal Main Elements")]
	public List<SideMissionIntroPreset.SideMissionObjectiveBlock> additional = new List<SideMissionIntroPreset.SideMissionObjectiveBlock>();

	// Token: 0x040038A5 RID: 14501
	[Tooltip("Scenarios that will reveal the required information for the task")]
	[Header("Hand-Ins")]
	public List<JobPreset.HandInConfig> compatibleHandIns = new List<JobPreset.HandInConfig>();

	// Token: 0x040038A6 RID: 14502
	[Header("Misc References")]
	public List<JobPreset.DialogReference> dialogReferences = new List<JobPreset.DialogReference>();

	// Token: 0x040038A7 RID: 14503
	[Header("Debug")]
	public JobPreset debugCopyFrom;

	// Token: 0x02000763 RID: 1891
	public enum JobTag
	{
		// Token: 0x040038A9 RID: 14505
		A,
		// Token: 0x040038AA RID: 14506
		B,
		// Token: 0x040038AB RID: 14507
		C,
		// Token: 0x040038AC RID: 14508
		D,
		// Token: 0x040038AD RID: 14509
		E,
		// Token: 0x040038AE RID: 14510
		F,
		// Token: 0x040038AF RID: 14511
		G,
		// Token: 0x040038B0 RID: 14512
		H,
		// Token: 0x040038B1 RID: 14513
		I,
		// Token: 0x040038B2 RID: 14514
		J,
		// Token: 0x040038B3 RID: 14515
		K,
		// Token: 0x040038B4 RID: 14516
		L,
		// Token: 0x040038B5 RID: 14517
		M,
		// Token: 0x040038B6 RID: 14518
		N,
		// Token: 0x040038B7 RID: 14519
		O,
		// Token: 0x040038B8 RID: 14520
		P,
		// Token: 0x040038B9 RID: 14521
		Q,
		// Token: 0x040038BA RID: 14522
		R,
		// Token: 0x040038BB RID: 14523
		S,
		// Token: 0x040038BC RID: 14524
		T,
		// Token: 0x040038BD RID: 14525
		U,
		// Token: 0x040038BE RID: 14526
		V,
		// Token: 0x040038BF RID: 14527
		W,
		// Token: 0x040038C0 RID: 14528
		X,
		// Token: 0x040038C1 RID: 14529
		Y,
		// Token: 0x040038C2 RID: 14530
		Z
	}

	// Token: 0x02000764 RID: 1892
	[Serializable]
	public class StartingScenario
	{
		// Token: 0x040038C3 RID: 14531
		public string name;

		// Token: 0x040038C4 RID: 14532
		public string dds;

		// Token: 0x040038C5 RID: 14533
		[Space(5f)]
		public List<JobPreset.StartingLead> leads = new List<JobPreset.StartingLead>();
	}

	// Token: 0x02000765 RID: 1893
	[Serializable]
	public class StartingLead
	{
		// Token: 0x040038C6 RID: 14534
		public JobPreset.LeadEvidence leadEvidence;

		// Token: 0x040038C7 RID: 14535
		[Space(5f)]
		[HideIf("useKeyFromLeadPool")]
		public List<Evidence.DataKey> keys = new List<Evidence.DataKey>();

		// Token: 0x040038C8 RID: 14536
		[Tooltip("Add to the above with keys from the lead pool (chosen first)")]
		public bool useKeyFromLeadPool;

		// Token: 0x040038C9 RID: 14537
		public bool autoPin;

		// Token: 0x040038CA RID: 14538
		[Space(5f)]
		[Tooltip("Add these dialog options to the above person")]
		public List<DialogPreset> addDialogOptions = new List<DialogPreset>();

		// Token: 0x040038CB RID: 14539
		[Tooltip("Add this fact link to the post facts section")]
		public List<string> factsReveal = new List<string>();

		// Token: 0x040038CC RID: 14540
		public List<Evidence.DataKey> mergeKeys = new List<Evidence.DataKey>();

		// Token: 0x040038CD RID: 14541
		public List<Evidence.Discovery> discoveryApplication = new List<Evidence.Discovery>();
	}

	// Token: 0x02000766 RID: 1894
	[Serializable]
	public class FactCreation
	{
		// Token: 0x040038CE RID: 14542
		public FactPreset factPreset;

		// Token: 0x040038CF RID: 14543
		public JobPreset.LeadEvidence from;

		// Token: 0x040038D0 RID: 14544
		public JobPreset.LeadEvidence to;

		// Token: 0x040038D1 RID: 14545
		[Space(5f)]
		public bool overrideFromKeys;

		// Token: 0x040038D2 RID: 14546
		public List<Evidence.DataKey> fromKeys = new List<Evidence.DataKey>();

		// Token: 0x040038D3 RID: 14547
		public bool featureKeysFromLeadPool = true;

		// Token: 0x040038D4 RID: 14548
		[Space(5f)]
		public bool overrideToKeys;

		// Token: 0x040038D5 RID: 14549
		public List<Evidence.DataKey> toKeys = new List<Evidence.DataKey>();

		// Token: 0x040038D6 RID: 14550
		public bool featureKeysFromLeadPoolTo;
	}

	// Token: 0x02000767 RID: 1895
	public enum LeadEvidence
	{
		// Token: 0x040038D8 RID: 14552
		none,
		// Token: 0x040038D9 RID: 14553
		poster,
		// Token: 0x040038DA RID: 14554
		purp,
		// Token: 0x040038DB RID: 14555
		purpsParamour,
		// Token: 0x040038DC RID: 14556
		postersHome,
		// Token: 0x040038DD RID: 14557
		purpsHome,
		// Token: 0x040038DE RID: 14558
		purpsParamourHome,
		// Token: 0x040038DF RID: 14559
		postersWorkplace,
		// Token: 0x040038E0 RID: 14560
		purpsWorkplace,
		// Token: 0x040038E1 RID: 14561
		purpsParamourWorkplace,
		// Token: 0x040038E2 RID: 14562
		postersBuilding,
		// Token: 0x040038E3 RID: 14563
		purpsBuilding,
		// Token: 0x040038E4 RID: 14564
		purpsParamourBuilding,
		// Token: 0x040038E5 RID: 14565
		post,
		// Token: 0x040038E6 RID: 14566
		posterTelephone,
		// Token: 0x040038E7 RID: 14567
		purpsTelephone,
		// Token: 0x040038E8 RID: 14568
		purpsParamourTelephone,
		// Token: 0x040038E9 RID: 14569
		postersWorkplaceBuilding,
		// Token: 0x040038EA RID: 14570
		purpsWorkplaceBuilding,
		// Token: 0x040038EB RID: 14571
		purpsParamourWorkplaceBuilding,
		// Token: 0x040038EC RID: 14572
		extraPerson1,
		// Token: 0x040038ED RID: 14573
		itemA,
		// Token: 0x040038EE RID: 14574
		itemB,
		// Token: 0x040038EF RID: 14575
		itemC,
		// Token: 0x040038F0 RID: 14576
		itemD,
		// Token: 0x040038F1 RID: 14577
		itemE
	}

	// Token: 0x02000768 RID: 1896
	public enum BasicLeadPool
	{
		// Token: 0x040038F3 RID: 14579
		hair,
		// Token: 0x040038F4 RID: 14580
		eyeColour,
		// Token: 0x040038F5 RID: 14581
		shoeSize,
		// Token: 0x040038F6 RID: 14582
		build,
		// Token: 0x040038F7 RID: 14583
		height,
		// Token: 0x040038F8 RID: 14584
		fingerprint,
		// Token: 0x040038F9 RID: 14585
		age,
		// Token: 0x040038FA RID: 14586
		jobTitle,
		// Token: 0x040038FB RID: 14587
		randomInterest,
		// Token: 0x040038FC RID: 14588
		partnerFirstName,
		// Token: 0x040038FD RID: 14589
		partnerJobTitle,
		// Token: 0x040038FE RID: 14590
		firstNameInitial,
		// Token: 0x040038FF RID: 14591
		socialClub,
		// Token: 0x04003900 RID: 14592
		partnerSocialClub,
		// Token: 0x04003901 RID: 14593
		notableFeatures,
		// Token: 0x04003902 RID: 14594
		salary,
		// Token: 0x04003903 RID: 14595
		bloodType,
		// Token: 0x04003904 RID: 14596
		randomAffliction,
		// Token: 0x04003905 RID: 14597
		handwriting
	}

	// Token: 0x02000769 RID: 1897
	public enum LeadCitizen
	{
		// Token: 0x04003907 RID: 14599
		nobody,
		// Token: 0x04003908 RID: 14600
		poster,
		// Token: 0x04003909 RID: 14601
		purp,
		// Token: 0x0400390A RID: 14602
		purpsParamour
	}

	// Token: 0x0200076A RID: 1898
	public enum JobSpawnWhere
	{
		// Token: 0x0400390C RID: 14604
		posterHome,
		// Token: 0x0400390D RID: 14605
		posterWork,
		// Token: 0x0400390E RID: 14606
		purpHome,
		// Token: 0x0400390F RID: 14607
		purpWork,
		// Token: 0x04003910 RID: 14608
		purpsParamourHome,
		// Token: 0x04003911 RID: 14609
		purpsParamourWork,
		// Token: 0x04003912 RID: 14610
		hiddenItemPlace,
		// Token: 0x04003913 RID: 14611
		nearbyGooseChase
	}

	// Token: 0x0200076B RID: 1899
	public enum DifficultyTag
	{
		// Token: 0x04003915 RID: 14613
		D0,
		// Token: 0x04003916 RID: 14614
		D1,
		// Token: 0x04003917 RID: 14615
		D2A,
		// Token: 0x04003918 RID: 14616
		D2B,
		// Token: 0x04003919 RID: 14617
		D3,
		// Token: 0x0400391A RID: 14618
		D4A,
		// Token: 0x0400391B RID: 14619
		D4B,
		// Token: 0x0400391C RID: 14620
		D4C,
		// Token: 0x0400391D RID: 14621
		D5,
		// Token: 0x0400391E RID: 14622
		D6
	}

	// Token: 0x0200076C RID: 1900
	[Serializable]
	public class JobModifierRule
	{
		// Token: 0x0400391F RID: 14623
		public JobPreset.LeadCitizen who;

		// Token: 0x04003920 RID: 14624
		public CharacterTrait.RuleType rule;

		// Token: 0x04003921 RID: 14625
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x04003922 RID: 14626
		[ShowIf("isTrait")]
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		public bool mustPassForApplication = true;

		// Token: 0x04003923 RID: 14627
		[Tooltip("Add this to a default priority multiplier of 1.")]
		public float chanceModifier;
	}

	// Token: 0x0200076D RID: 1901
	[Serializable]
	public class StartingSpawnItem
	{
		// Token: 0x04003924 RID: 14628
		public string name;

		// Token: 0x04003925 RID: 14629
		[Tooltip("Try and find an existing interactable that matches this criteria...")]
		public bool findExisting;

		// Token: 0x04003926 RID: 14630
		public List<MotivePreset> compatibleWithMotives = new List<MotivePreset>();

		// Token: 0x04003927 RID: 14631
		public bool compatibleWithAllMotives;

		// Token: 0x04003928 RID: 14632
		[Space(7f)]
		[DisableIf("useOrGroup")]
		[Range(0f, 1f)]
		public float chance = 1f;

		// Token: 0x04003929 RID: 14633
		[Space(7f)]
		public bool useTraits;

		// Token: 0x0400392A RID: 14634
		[EnableIf("useTraits")]
		public List<JobPreset.JobModifierRule> traitModifiers = new List<JobPreset.JobModifierRule>();

		// Token: 0x0400392B RID: 14635
		[Space(7f)]
		public bool useIf;

		// Token: 0x0400392C RID: 14636
		[EnableIf("useIf")]
		[Tooltip("Only spawn if a previous object of this letter is spawned...")]
		public JobPreset.JobTag ifTag;

		// Token: 0x0400392D RID: 14637
		[Space(7f)]
		public bool useOrGroup;

		// Token: 0x0400392E RID: 14638
		[Tooltip("If enabled, only one chosen item from this group will be spawned...")]
		[EnableIf("useOrGroup")]
		public JobPreset.JobTag orGroup;

		// Token: 0x0400392F RID: 14639
		[EnableIf("useOrGroup")]
		[Range(0f, 10f)]
		public int chanceRatio = 4;

		// Token: 0x04003930 RID: 14640
		[Space(7f)]
		public List<JobPreset.DifficultyTag> disableOnDifficulties = new List<JobPreset.DifficultyTag>();

		// Token: 0x04003931 RID: 14641
		[Space(7f)]
		public JobPreset.JobTag itemTag;

		// Token: 0x04003932 RID: 14642
		[Tooltip("What?")]
		public InteractablePreset spawnItem;

		// Token: 0x04003933 RID: 14643
		[Space(7f)]
		public string vmailThread;

		// Token: 0x04003934 RID: 14644
		public Vector2 vmailProgressThreshold;

		// Token: 0x04003935 RID: 14645
		[Tooltip("Where?")]
		public JobPreset.JobSpawnWhere where;

		// Token: 0x04003936 RID: 14646
		public JobPreset.LeadCitizen belongsTo = JobPreset.LeadCitizen.poster;

		// Token: 0x04003937 RID: 14647
		public JobPreset.LeadCitizen writer;

		// Token: 0x04003938 RID: 14648
		public JobPreset.LeadCitizen receiver;

		// Token: 0x04003939 RID: 14649
		public int security = 3;

		// Token: 0x0400393A RID: 14650
		public int priority = 1;

		// Token: 0x0400393B RID: 14651
		public InteractablePreset.OwnedPlacementRule ownershipRule;
	}

	// Token: 0x0200076E RID: 1902
	[Serializable]
	public class HandInLocation
	{
		// Token: 0x0400393C RID: 14652
		public JobPreset.LeadCitizen who;
	}

	// Token: 0x0200076F RID: 1903
	[Serializable]
	public class IntroConfig
	{
		// Token: 0x0400393D RID: 14653
		public SideMissionIntroPreset preset;

		// Token: 0x0400393E RID: 14654
		[Range(0f, 10f)]
		public int frequency = 5;
	}

	// Token: 0x02000770 RID: 1904
	[Serializable]
	public class HandInConfig
	{
		// Token: 0x0400393F RID: 14655
		public SideMissionHandInPreset preset;

		// Token: 0x04003940 RID: 14656
		[Range(0f, 10f)]
		public int frequency = 5;
	}

	// Token: 0x02000771 RID: 1905
	public enum RewardLocation
	{
		// Token: 0x04003942 RID: 14658
		none,
		// Token: 0x04003943 RID: 14659
		postersMailbox,
		// Token: 0x04003944 RID: 14660
		cityHallDesk,
		// Token: 0x04003945 RID: 14661
		playersMailbox
	}

	// Token: 0x02000772 RID: 1906
	public enum ParticipantCompliancy
	{
		// Token: 0x04003947 RID: 14663
		noChange,
		// Token: 0x04003948 RID: 14664
		alwaysSuccess,
		// Token: 0x04003949 RID: 14665
		alwaysFail
	}

	// Token: 0x02000773 RID: 1907
	[Serializable]
	public class DialogReference
	{
		// Token: 0x0400394A RID: 14666
		public string name;

		// Token: 0x0400394B RID: 14667
		public DialogPreset dialog;
	}
}
