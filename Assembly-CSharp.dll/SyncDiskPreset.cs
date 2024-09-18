using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007DC RID: 2012
[CreateAssetMenu(fileName = "syncdisk_data", menuName = "Database/Sync Disk Preset")]
public class SyncDiskPreset : SoCustomComparison
{
	// Token: 0x060025E3 RID: 9699 RVA: 0x001E880C File Offset: 0x001E6A0C
	[Button(null, 0)]
	public void CopyOwnershipStats()
	{
		if (this.copyFrom != null)
		{
			this.minimumWealthLevel = this.copyFrom.minimumWealthLevel;
			this.traitWeight = this.copyFrom.traitWeight;
			this.traits.Clear();
			this.traits.AddRange(this.copyFrom.traits);
			this.occupationWeight = this.copyFrom.occupationWeight;
			this.occupation.Clear();
			this.occupation.AddRange(this.copyFrom.occupation);
		}
	}

	// Token: 0x04003DDA RID: 15834
	[BoxGroup("Disable")]
	[Tooltip("Disable this in-game completely.")]
	public bool disabled;

	// Token: 0x04003DDB RID: 15835
	[Header("Configuration")]
	public int syncDiskNumber = 1;

	// Token: 0x04003DDC RID: 15836
	public InteractablePreset interactable;

	// Token: 0x04003DDD RID: 15837
	public SyncDiskPreset.Rarity rarity = SyncDiskPreset.Rarity.medium;

	// Token: 0x04003DDE RID: 15838
	public SyncDiskPreset.Manufacturer manufacturer;

	// Token: 0x04003DDF RID: 15839
	public bool canBeSideJobReward = true;

	// Token: 0x04003DE0 RID: 15840
	[Header("Usage")]
	public string mainEffect1Name;

	// Token: 0x04003DE1 RID: 15841
	public string mainEffect1Description;

	// Token: 0x04003DE2 RID: 15842
	public SyncDiskPreset.Effect mainEffect1;

	// Token: 0x04003DE3 RID: 15843
	public float mainEffect1Value = 0.2f;

	// Token: 0x04003DE4 RID: 15844
	[ShowAssetPreview(64, 64)]
	public Sprite mainEffect1Icon;

	// Token: 0x04003DE5 RID: 15845
	[Space(7f)]
	public string mainEffect2Name;

	// Token: 0x04003DE6 RID: 15846
	public string mainEffect2Description;

	// Token: 0x04003DE7 RID: 15847
	public SyncDiskPreset.Effect mainEffect2;

	// Token: 0x04003DE8 RID: 15848
	public float mainEffect2Value = 0.2f;

	// Token: 0x04003DE9 RID: 15849
	[ShowAssetPreview(64, 64)]
	public Sprite mainEffect2Icon;

	// Token: 0x04003DEA RID: 15850
	[Space(7f)]
	public string mainEffect3Name;

	// Token: 0x04003DEB RID: 15851
	public string mainEffect3Description;

	// Token: 0x04003DEC RID: 15852
	public SyncDiskPreset.Effect mainEffect3;

	// Token: 0x04003DED RID: 15853
	public float mainEffect3Value = 0.2f;

	// Token: 0x04003DEE RID: 15854
	[ShowAssetPreview(64, 64)]
	public Sprite mainEffect3Icon;

	// Token: 0x04003DEF RID: 15855
	[Header("Upgrade Option 1")]
	public List<string> option1UpgradeNameReferences = new List<string>();

	// Token: 0x04003DF0 RID: 15856
	public List<SyncDiskPreset.UpgradeEffect> option1UpgradeEffects = new List<SyncDiskPreset.UpgradeEffect>();

	// Token: 0x04003DF1 RID: 15857
	public List<float> option1UpgradeValues = new List<float>();

	// Token: 0x04003DF2 RID: 15858
	[Header("Upgrade Option 2")]
	[Space(7f)]
	public List<string> option2UpgradeNameReferences = new List<string>();

	// Token: 0x04003DF3 RID: 15859
	public List<SyncDiskPreset.UpgradeEffect> option2UpgradeEffects = new List<SyncDiskPreset.UpgradeEffect>();

	// Token: 0x04003DF4 RID: 15860
	public List<float> option2UpgradeValues = new List<float>();

	// Token: 0x04003DF5 RID: 15861
	[Space(7f)]
	[Header("Upgrade Option 3")]
	public List<string> option3UpgradeNameReferences = new List<string>();

	// Token: 0x04003DF6 RID: 15862
	public List<SyncDiskPreset.UpgradeEffect> option3UpgradeEffects = new List<SyncDiskPreset.UpgradeEffect>();

	// Token: 0x04003DF7 RID: 15863
	public List<float> option3UpgradeValues = new List<float>();

	// Token: 0x04003DF8 RID: 15864
	[Header("Effects")]
	public string sideEffectDescription;

	// Token: 0x04003DF9 RID: 15865
	public SyncDiskPreset.Effect sideEffect;

	// Token: 0x04003DFA RID: 15866
	public float sideEffectValue;

	// Token: 0x04003DFB RID: 15867
	[Header("Costs")]
	public int price = 500;

	// Token: 0x04003DFC RID: 15868
	public int uninstallCost;

	// Token: 0x04003DFD RID: 15869
	[Header("Ownership")]
	[Range(0f, 1f)]
	public float minimumWealthLevel;

	// Token: 0x04003DFE RID: 15870
	[Range(0f, 5f)]
	public int traitWeight = 1;

	// Token: 0x04003DFF RID: 15871
	[ReorderableList]
	public List<SyncDiskPreset.TraitPick> traits = new List<SyncDiskPreset.TraitPick>();

	// Token: 0x04003E00 RID: 15872
	[Range(0f, 5f)]
	public int occupationWeight;

	// Token: 0x04003E01 RID: 15873
	[ReorderableList]
	public List<OccupationPreset> occupation = new List<OccupationPreset>();

	// Token: 0x04003E02 RID: 15874
	[Header("Debug")]
	public SyncDiskPreset copyFrom;

	// Token: 0x020007DD RID: 2013
	public enum Rarity
	{
		// Token: 0x04003E04 RID: 15876
		common,
		// Token: 0x04003E05 RID: 15877
		medium,
		// Token: 0x04003E06 RID: 15878
		rare,
		// Token: 0x04003E07 RID: 15879
		veryRare
	}

	// Token: 0x020007DE RID: 2014
	public enum Manufacturer
	{
		// Token: 0x04003E09 RID: 15881
		ElGen,
		// Token: 0x04003E0A RID: 15882
		Kaizen,
		// Token: 0x04003E0B RID: 15883
		KensingtonIndigo,
		// Token: 0x04003E0C RID: 15884
		StarchKola,
		// Token: 0x04003E0D RID: 15885
		CandorNews,
		// Token: 0x04003E0E RID: 15886
		BlackMarket
	}

	// Token: 0x020007DF RID: 2015
	public enum Effect
	{
		// Token: 0x04003E10 RID: 15888
		none,
		// Token: 0x04003E11 RID: 15889
		streetCleaningMoney,
		// Token: 0x04003E12 RID: 15890
		readingMoney,
		// Token: 0x04003E13 RID: 15891
		readingSeriesBonus,
		// Token: 0x04003E14 RID: 15892
		starchLoan,
		// Token: 0x04003E15 RID: 15893
		starchAddiction,
		// Token: 0x04003E16 RID: 15894
		reduceMedicalCosts,
		// Token: 0x04003E17 RID: 15895
		legalInsurance,
		// Token: 0x04003E18 RID: 15896
		accidentCover,
		// Token: 0x04003E19 RID: 15897
		awakenAtHome,
		// Token: 0x04003E1A RID: 15898
		increaseHealth,
		// Token: 0x04003E1B RID: 15899
		increaseInventory,
		// Token: 0x04003E1C RID: 15900
		increaseRegeneration,
		// Token: 0x04003E1D RID: 15901
		priceModifier,
		// Token: 0x04003E1E RID: 15902
		dialogChanceModifier,
		// Token: 0x04003E1F RID: 15903
		doorBargeModifier,
		// Token: 0x04003E20 RID: 15904
		fallDamageModifier,
		// Token: 0x04003E21 RID: 15905
		sideJobPayModifier,
		// Token: 0x04003E22 RID: 15906
		punchPowerModifier,
		// Token: 0x04003E23 RID: 15907
		throwPowerModifier,
		// Token: 0x04003E24 RID: 15908
		blockIncoming,
		// Token: 0x04003E25 RID: 15909
		focusFromDamage,
		// Token: 0x04003E26 RID: 15910
		noBrokenBones,
		// Token: 0x04003E27 RID: 15911
		reachModifier,
		// Token: 0x04003E28 RID: 15912
		holdingBlocksBullets,
		// Token: 0x04003E29 RID: 15913
		fistsThreatModifier,
		// Token: 0x04003E2A RID: 15914
		noBleeding,
		// Token: 0x04003E2B RID: 15915
		incomingDamageModifier,
		// Token: 0x04003E2C RID: 15916
		passiveIncome,
		// Token: 0x04003E2D RID: 15917
		installMalware,
		// Token: 0x04003E2E RID: 15918
		malwareOwnerBonus,
		// Token: 0x04003E2F RID: 15919
		footSizePerception,
		// Token: 0x04003E30 RID: 15920
		heightPerception,
		// Token: 0x04003E31 RID: 15921
		wealthPerception,
		// Token: 0x04003E32 RID: 15922
		salaryPerception,
		// Token: 0x04003E33 RID: 15923
		singlePerception,
		// Token: 0x04003E34 RID: 15924
		agePerception,
		// Token: 0x04003E35 RID: 15925
		starchAmbassador,
		// Token: 0x04003E36 RID: 15926
		starchGive,
		// Token: 0x04003E37 RID: 15927
		lockpickingSpeedModifier,
		// Token: 0x04003E38 RID: 15928
		lockpickingEfficiencyModifier,
		// Token: 0x04003E39 RID: 15929
		triggerIllegalOnPick,
		// Token: 0x04003E3A RID: 15930
		KOTimeModifier,
		// Token: 0x04003E3B RID: 15931
		securityBreakerModifier,
		// Token: 0x04003E3C RID: 15932
		securityGraceTimeModifier,
		// Token: 0x04003E3D RID: 15933
		noSmelly,
		// Token: 0x04003E3E RID: 15934
		noCold,
		// Token: 0x04003E3F RID: 15935
		noTired,
		// Token: 0x04003E40 RID: 15936
		kitchenPhotos,
		// Token: 0x04003E41 RID: 15937
		bathroomPhotos,
		// Token: 0x04003E42 RID: 15938
		illegalOpsPhotos,
		// Token: 0x04003E43 RID: 15939
		playerHeightModifier,
		// Token: 0x04003E44 RID: 15940
		removeSideEffect,
		// Token: 0x04003E45 RID: 15941
		moneyForLocations,
		// Token: 0x04003E46 RID: 15942
		moneyForDucts,
		// Token: 0x04003E47 RID: 15943
		moneyForAddresses,
		// Token: 0x04003E48 RID: 15944
		moneyForPasscodes,
		// Token: 0x04003E49 RID: 15945
		maxSpeedModifier,
		// Token: 0x04003E4A RID: 15946
		payPhoneCostModifier,
		// Token: 0x04003E4B RID: 15947
		allowApartmentPurchases,
		// Token: 0x04003E4C RID: 15948
		apartmentStatusReset,
		// Token: 0x04003E4D RID: 15949
		allowedAtCrimeScenes,
		// Token: 0x04003E4E RID: 15950
		spookedMultiplier,
		// Token: 0x04003E4F RID: 15951
		trespassGraceModifier,
		// Token: 0x04003E50 RID: 15952
		guestPassIssueModifier,
		// Token: 0x04003E51 RID: 15953
		fastTravelToApartment,
		// Token: 0x04003E52 RID: 15954
		fastTravelFromApartment,
		// Token: 0x04003E53 RID: 15955
		fastTravelUsingSignage,
		// Token: 0x04003E54 RID: 15956
		allowedInEchelons,
		// Token: 0x04003E55 RID: 15957
		disableLoitering
	}

	// Token: 0x020007E0 RID: 2016
	public enum UpgradeEffect
	{
		// Token: 0x04003E57 RID: 15959
		none,
		// Token: 0x04003E58 RID: 15960
		modifyEffect,
		// Token: 0x04003E59 RID: 15961
		bothConfigurations,
		// Token: 0x04003E5A RID: 15962
		readingSeriesBonus,
		// Token: 0x04003E5B RID: 15963
		reduceUninstallCost,
		// Token: 0x04003E5C RID: 15964
		reduceMedicalCosts,
		// Token: 0x04003E5D RID: 15965
		accidentCover,
		// Token: 0x04003E5E RID: 15966
		legalInsurance,
		// Token: 0x04003E5F RID: 15967
		awakenAtHome,
		// Token: 0x04003E60 RID: 15968
		increaseHealth,
		// Token: 0x04003E61 RID: 15969
		increaseInventory,
		// Token: 0x04003E62 RID: 15970
		increaseRegeneration,
		// Token: 0x04003E63 RID: 15971
		priceModifier,
		// Token: 0x04003E64 RID: 15972
		dialogChanceModifier,
		// Token: 0x04003E65 RID: 15973
		doorBargeModifier,
		// Token: 0x04003E66 RID: 15974
		fallDamageModifier,
		// Token: 0x04003E67 RID: 15975
		sideJobPayModifier,
		// Token: 0x04003E68 RID: 15976
		punchPowerModifier,
		// Token: 0x04003E69 RID: 15977
		throwPowerModifier,
		// Token: 0x04003E6A RID: 15978
		blockIncoming,
		// Token: 0x04003E6B RID: 15979
		focusFromDamage,
		// Token: 0x04003E6C RID: 15980
		noBrokenBones,
		// Token: 0x04003E6D RID: 15981
		reachModifier,
		// Token: 0x04003E6E RID: 15982
		holdingBlocksBullets,
		// Token: 0x04003E6F RID: 15983
		fistsThreatModifier,
		// Token: 0x04003E70 RID: 15984
		noBleeding,
		// Token: 0x04003E71 RID: 15985
		incomingDamageModifier,
		// Token: 0x04003E72 RID: 15986
		passiveIncome,
		// Token: 0x04003E73 RID: 15987
		installMalware,
		// Token: 0x04003E74 RID: 15988
		malwareOwnerBonus,
		// Token: 0x04003E75 RID: 15989
		footSizePerception,
		// Token: 0x04003E76 RID: 15990
		heightPerception,
		// Token: 0x04003E77 RID: 15991
		wealthPerception,
		// Token: 0x04003E78 RID: 15992
		removeSideEffect,
		// Token: 0x04003E79 RID: 15993
		salaryPerception,
		// Token: 0x04003E7A RID: 15994
		singlePerception,
		// Token: 0x04003E7B RID: 15995
		agePerception,
		// Token: 0x04003E7C RID: 15996
		starchAmbassador,
		// Token: 0x04003E7D RID: 15997
		starchGive,
		// Token: 0x04003E7E RID: 15998
		lockpickingSpeedModifier,
		// Token: 0x04003E7F RID: 15999
		lockpickingEfficiencyModifier,
		// Token: 0x04003E80 RID: 16000
		triggerIllegalOnPick,
		// Token: 0x04003E81 RID: 16001
		KOTimeModifier,
		// Token: 0x04003E82 RID: 16002
		securityBreakerModifier,
		// Token: 0x04003E83 RID: 16003
		securityGraceTimeModifier,
		// Token: 0x04003E84 RID: 16004
		noSmelly,
		// Token: 0x04003E85 RID: 16005
		noCold,
		// Token: 0x04003E86 RID: 16006
		noTired,
		// Token: 0x04003E87 RID: 16007
		kitchenPhotos,
		// Token: 0x04003E88 RID: 16008
		bathroomPhotos,
		// Token: 0x04003E89 RID: 16009
		illegalOpsPhotos,
		// Token: 0x04003E8A RID: 16010
		playerHeightModifier,
		// Token: 0x04003E8B RID: 16011
		moneyForLocations,
		// Token: 0x04003E8C RID: 16012
		moneyForDucts,
		// Token: 0x04003E8D RID: 16013
		moneyForAddresses,
		// Token: 0x04003E8E RID: 16014
		moneyForPasscodes,
		// Token: 0x04003E8F RID: 16015
		maxSpeedModifier
	}

	// Token: 0x020007E1 RID: 2017
	public enum SpecialCase
	{
		// Token: 0x04003E91 RID: 16017
		none,
		// Token: 0x04003E92 RID: 16018
		cancelSideEffect
	}

	// Token: 0x020007E2 RID: 2018
	[Serializable]
	public class TraitPick
	{
		// Token: 0x04003E93 RID: 16019
		public CharacterTrait.RuleType rule;

		// Token: 0x04003E94 RID: 16020
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x04003E95 RID: 16021
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		public bool mustPassForApplication;

		// Token: 0x04003E96 RID: 16022
		public int appliedFrequency = 1;
	}
}
