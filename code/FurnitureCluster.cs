using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000730 RID: 1840
[CreateAssetMenu(fileName = "furniturecluster_data", menuName = "Database/Decor/Furniture Cluster")]
public class FurnitureCluster : SoCustomComparison
{
	// Token: 0x0600255D RID: 9565 RVA: 0x001E6184 File Offset: 0x001E4384
	[Button(null, 0)]
	public void UpdatePreCalculatedLimits()
	{
		this.calculatedMinRoomSize = 0;
		this.minimumZeroNodeWallCount = 0;
		this.maximumZeroNodeWallCount = 4;
		List<FurnitureCluster.FurnitureClusterRule> list = new List<FurnitureCluster.FurnitureClusterRule>();
		this.zeroNodeClasses = new List<FurnitureClass>();
		foreach (FurnitureCluster.FurnitureClusterRule furnitureClusterRule in this.clusterElements)
		{
			if (furnitureClusterRule.importantToCluster && furnitureClusterRule.furnitureClass != null)
			{
				if (furnitureClusterRule.placements.Exists((Vector2 item) => Mathf.RoundToInt(item.x) == 0 && Mathf.RoundToInt(item.y) == 0))
				{
					list.Add(furnitureClusterRule);
					this.zeroNodeClasses.Add(furnitureClusterRule.furnitureClass);
				}
				int num = (int)furnitureClusterRule.furnitureClass.objectSize.x * (int)furnitureClusterRule.furnitureClass.objectSize.y;
				if (furnitureClusterRule.furnitureClass.occupiesTile)
				{
					this.calculatedMinRoomSize += num;
				}
			}
		}
		foreach (FurnitureCluster.FurnitureClusterRule furnitureClusterRule2 in list)
		{
			if (furnitureClusterRule2.furnitureClass != null)
			{
				furnitureClusterRule2.furnitureClass.UpdatePreCalculatedLimits();
				this.minimumZeroNodeWallCount = Mathf.Max(this.minimumZeroNodeWallCount, furnitureClusterRule2.furnitureClass.minimumZeroNodeWallCount);
				this.maximumZeroNodeWallCount = Mathf.Min(this.maximumZeroNodeWallCount, furnitureClusterRule2.furnitureClass.maximumZeroNodeWallCount);
			}
		}
		this.calculatedMinRoomSize = Mathf.Max(1, this.calculatedMinRoomSize);
		this.minimumRoomSize = Mathf.Max(this.minimumRoomSize, this.calculatedMinRoomSize);
	}

	// Token: 0x0400358C RID: 13708
	public bool disable;

	// Token: 0x0400358D RID: 13709
	[Header("Rules")]
	[Tooltip("List of rules this furniture must follow")]
	public List<FurnitureCluster.FurnitureClusterRule> clusterElements = new List<FurnitureCluster.FurnitureClusterRule>();

	// Token: 0x0400358E RID: 13710
	[Header("Suitability")]
	[Tooltip("Chance for skipping this cluster altogether")]
	[Range(0f, 1f)]
	public float placementChance = 1f;

	// Token: 0x0400358F RID: 13711
	[Tooltip("The ranking given to this item when choosing what to place.")]
	[Range(0f, 11f)]
	public float roomPriority = 5f;

	// Token: 0x04003590 RID: 13712
	[Tooltip("Modify priority with traits present. The base chance here is x10 and added to the above room priority.")]
	public List<CharacterTrait.TraitPickRule> modifyPriorityTraits = new List<CharacterTrait.TraitPickRule>();

	// Token: 0x04003591 RID: 13713
	[Tooltip("Modify placement chance with traits present. The base chance here is x10 and added to the above placement chance.")]
	public List<CharacterTrait.TraitPickRule> modifyPlacementChanceTraits = new List<CharacterTrait.TraitPickRule>();

	// Token: 0x04003592 RID: 13714
	[Space(7f)]
	[Tooltip("If true this will override priority with 10 unless one already exists")]
	public bool essentialFurniture;

	// Token: 0x04003593 RID: 13715
	[Header("PreCalculated Limits/Optimization")]
	[OnValueChanged("UpdatePreCalculatedLimits")]
	public bool updatePreCalculated;

	// Token: 0x04003594 RID: 13716
	[ReadOnly]
	public int calculatedMinRoomSize = 1;

	// Token: 0x04003595 RID: 13717
	[ReadOnly]
	public int minimumZeroNodeWallCount;

	// Token: 0x04003596 RID: 13718
	[ReadOnly]
	public int maximumZeroNodeWallCount = 4;

	// Token: 0x04003597 RID: 13719
	[ReadOnly]
	public List<FurnitureClass> zeroNodeClasses = new List<FurnitureClass>();

	// Token: 0x04003598 RID: 13720
	[Tooltip("Room must be at least this size")]
	[Header("Custom Limits/Optimization")]
	public int minimumRoomSize = 1;

	// Token: 0x04003599 RID: 13721
	public bool useMaximumRoomSize = true;

	// Token: 0x0400359A RID: 13722
	[EnableIf("useMaximumRoomSize")]
	public int maximumRoomSize = 99;

	// Token: 0x0400359B RID: 13723
	[Space(7f)]
	public bool useCustomZeroNodeMinWallCount;

	// Token: 0x0400359C RID: 13724
	[Range(0f, 4f)]
	[EnableIf("useCustomZeroNodeMinWallCount")]
	public int customZeroNodeMinWallCount;

	// Token: 0x0400359D RID: 13725
	public bool useCustomZeroNodeMaxWallCount;

	// Token: 0x0400359E RID: 13726
	[EnableIf("useCustomZeroNodeMaxWallCount")]
	[Range(0f, 4f)]
	public int customZeroNodeMaxWallCount;

	// Token: 0x0400359F RID: 13727
	public List<FurnitureClass.FurnitureWallRule> zeroNodeWallRules = new List<FurnitureClass.FurnitureWallRule>();

	// Token: 0x040035A0 RID: 13728
	[Tooltip("Is this allowed in open plan rooms?")]
	[Space(7f)]
	public FurnitureCluster.AllowedOpenPlan allowedInOpenPlan;

	// Token: 0x040035A1 RID: 13729
	[Space(7f)]
	public bool allowInResidential = true;

	// Token: 0x040035A2 RID: 13730
	public bool allowInCompanies = true;

	// Token: 0x040035A3 RID: 13731
	public bool allowOnStreets = true;

	// Token: 0x040035A4 RID: 13732
	[Tooltip("This is only to be placed on coastal streets")]
	public bool coastalOnly;

	// Token: 0x040035A5 RID: 13733
	[Space(7f)]
	[Tooltip("Only allow this in certain districts")]
	public bool limitToDistricts;

	// Token: 0x040035A6 RID: 13734
	[EnableIf("limitToDistricts")]
	public List<DistrictPreset> allowedInDistricts = new List<DistrictPreset>();

	// Token: 0x040035A7 RID: 13735
	public bool banFromDistricts;

	// Token: 0x040035A8 RID: 13736
	[EnableIf("banFromDistricts")]
	public List<DistrictPreset> notAllowedInDistricts = new List<DistrictPreset>();

	// Token: 0x040035A9 RID: 13737
	[Space(7f)]
	[Tooltip("Skip placement of this if there are no address owners")]
	public bool skipIfNoAddressInhabitants;

	// Token: 0x040035AA RID: 13738
	[EnableIf("skipIfNoAddressInhabitants")]
	[Tooltip("If the above is true, will only be skipped if this is a residence or company")]
	public bool onlySkipNoInhabitantsIfResidenceOrCompany = true;

	// Token: 0x040035AB RID: 13739
	[Tooltip("If the above is true, don't skip if within addresses of this type")]
	[EnableIf("skipIfNoAddressInhabitants")]
	public List<RoomClassPreset> dontSkipNoInhabitantsIfIn = new List<RoomClassPreset>();

	// Token: 0x040035AC RID: 13740
	[Space(7f)]
	public List<RoomTypeFilter> allowedRoomFilters = new List<RoomTypeFilter>();

	// Token: 0x040035AD RID: 13741
	[Space(7f)]
	[Tooltip("Maximum number per room")]
	public bool limitPerRoom = true;

	// Token: 0x040035AE RID: 13742
	[EnableIf("limitPerRoom")]
	[Range(1f, 20f)]
	public int maximumPerRoom = 2;

	// Token: 0x040035AF RID: 13743
	[Tooltip("Maximum number per address")]
	public bool limitPerAddress;

	// Token: 0x040035B0 RID: 13744
	[EnableIf("limitPerAddress")]
	[Range(1f, 20f)]
	public int maximumPerAddress = 1;

	// Token: 0x040035B1 RID: 13745
	[Tooltip("Allow only on this floor")]
	public bool limitToFloor;

	// Token: 0x040035B2 RID: 13746
	[EnableIf("limitToFloor")]
	public int allowedOnFloor;

	// Token: 0x040035B3 RID: 13747
	[DisableIf("limitToFloor")]
	[Tooltip("Allow only between these floors")]
	public bool limitToFloorRange;

	// Token: 0x040035B4 RID: 13748
	[EnableIf("limitToFloorRange")]
	public Vector2Int allowedOnFloorRange = new Vector2Int(-2, 99);

	// Token: 0x040035B5 RID: 13749
	public bool wealthLimit;

	// Token: 0x040035B6 RID: 13750
	[Range(0f, 1f)]
	[EnableIf("wealthLimit")]
	public float minimumWealth;

	// Token: 0x040035B7 RID: 13751
	[EnableIf("wealthLimit")]
	[Range(0f, 1f)]
	public float maximumWealth = 1f;

	// Token: 0x040035B8 RID: 13752
	public bool useRoomGrub;

	// Token: 0x040035B9 RID: 13753
	[EnableIf("useRoomGrub")]
	[Range(0f, 1f)]
	public float minimumGrub;

	// Token: 0x040035BA RID: 13754
	[EnableIf("useRoomGrub")]
	[Range(0f, 1f)]
	public float maximumGrub = 1f;

	// Token: 0x040035BB RID: 13755
	public bool useBuildingResidences;

	// Token: 0x040035BC RID: 13756
	[EnableIf("useBuildingResidences")]
	public int minimumResidences;

	// Token: 0x040035BD RID: 13757
	[EnableIf("useBuildingResidences")]
	public int maximumResidences = 40;

	// Token: 0x040035BE RID: 13758
	[Header("Optimizations")]
	[Tooltip("If this cluster is successfully placed, add the following cluster presets from trying to be placed")]
	public List<FurnitureCluster> addClustersOnSuccess = new List<FurnitureCluster>();

	// Token: 0x040035BF RID: 13759
	[Tooltip("If this cluster is successfully placed, remove the following cluster presets from trying to be placed")]
	public List<FurnitureCluster> removeClustersOnSuccess = new List<FurnitureCluster>();

	// Token: 0x040035C0 RID: 13760
	[Tooltip("If this cluster fails to be placed, remove the following cluster presets from trying to be placed")]
	public List<FurnitureCluster> removeClustersOnFail = new List<FurnitureCluster>();

	// Token: 0x040035C1 RID: 13761
	[Header("Misc.")]
	[Tooltip("Is this a security door?")]
	public bool securityDoor;

	// Token: 0x040035C2 RID: 13762
	[Tooltip("Is this a breaker box?")]
	public bool isBreakerBox;

	// Token: 0x040035C3 RID: 13763
	[Header("Debugging")]
	public bool enableDebug;

	// Token: 0x02000731 RID: 1841
	public enum FurnitureRuleOption
	{
		// Token: 0x040035C5 RID: 13765
		mustFeature,
		// Token: 0x040035C6 RID: 13766
		cantFeature,
		// Token: 0x040035C7 RID: 13767
		canFeature
	}

	// Token: 0x02000732 RID: 1842
	public enum WallRule
	{
		// Token: 0x040035C9 RID: 13769
		nothing,
		// Token: 0x040035CA RID: 13770
		wallNoDoor,
		// Token: 0x040035CB RID: 13771
		onlyWall,
		// Token: 0x040035CC RID: 13772
		doorway,
		// Token: 0x040035CD RID: 13773
		door,
		// Token: 0x040035CE RID: 13774
		bannister,
		// Token: 0x040035CF RID: 13775
		window
	}

	// Token: 0x02000733 RID: 1843
	public enum FurnitureFacing
	{
		// Token: 0x040035D1 RID: 13777
		down,
		// Token: 0x040035D2 RID: 13778
		up,
		// Token: 0x040035D3 RID: 13779
		left,
		// Token: 0x040035D4 RID: 13780
		right
	}

	// Token: 0x02000734 RID: 1844
	public enum AllowedOpenPlan
	{
		// Token: 0x040035D6 RID: 13782
		yes,
		// Token: 0x040035D7 RID: 13783
		no,
		// Token: 0x040035D8 RID: 13784
		openPlanOnly
	}

	// Token: 0x02000735 RID: 1845
	[Serializable]
	public class FurnitureClusterRule
	{
		// Token: 0x040035D9 RID: 13785
		[Tooltip("Only consider this if the last object got placed")]
		public bool onlyValidIfPreviousObjectPlaced;

		// Token: 0x040035DA RID: 13786
		[Tooltip("If not able to be placed at the above, scan list of alternates at random to find a valid position")]
		public List<Vector2> placements = new List<Vector2>();

		// Token: 0x040035DB RID: 13787
		[Tooltip("What should be found at this node?")]
		public FurnitureClass furnitureClass;

		// Token: 0x040035DC RID: 13788
		public FurnitureCluster.FurnitureFacing facing;

		// Token: 0x040035DD RID: 13789
		[Tooltip("If cannot be place here, this cluster placement is invalid")]
		public bool importantToCluster;

		// Token: 0x040035DE RID: 13790
		[DisableIf("importantToCluster")]
		[Tooltip("Chance this placement will be attempted: Done on a per-location basis, so often a lower number will result in a much lower placement count...")]
		[Range(0f, 1f)]
		public float chanceOfPlacementAttempt = 1f;

		// Token: 0x040035DF RID: 13791
		public int placementScoreBoost;

		// Token: 0x040035E0 RID: 13792
		[Tooltip("Block objects in this path")]
		public bool useFovBlock;

		// Token: 0x040035E1 RID: 13793
		[Tooltip("The FOV block will continue in this direction, this is before direction is applied, so 0,1 is infront for example.")]
		public Vector2 blockDirection = new Vector2(0f, 1f);

		// Token: 0x040035E2 RID: 13794
		public int maxFOVBlockDistance = 5;

		// Token: 0x040035E3 RID: 13795
		[Tooltip("Local scale")]
		public Vector3 localScale = Vector3.one;

		// Token: 0x040035E4 RID: 13796
		[Tooltip("Offset")]
		public Vector3 positionOffset = Vector3.zero;
	}
}
