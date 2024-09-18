using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000725 RID: 1829
[CreateAssetMenu(fileName = "furnitureclass_data", menuName = "Database/Decor/Furniture Class")]
public class FurnitureClass : SoCustomComparison
{
	// Token: 0x06002551 RID: 9553 RVA: 0x001E5CE2 File Offset: 0x001E3EE2
	[Button(null, 0)]
	public void CopyBlockedAccess()
	{
		if (this.copyFrom != null)
		{
			this.blockedAccess.Clear();
			this.blockedAccess.AddRange(this.copyFrom.blockedAccess);
		}
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x001E5D13 File Offset: 0x001E3F13
	[Button(null, 0)]
	public void CopySublocations()
	{
		if (this.copyFrom != null)
		{
			this.sublocations.Clear();
			this.sublocations.AddRange(this.copyFrom.sublocations);
		}
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x001E5D44 File Offset: 0x001E3F44
	[Button(null, 0)]
	public void BlockSolid()
	{
		this.blockedAccess.Clear();
		int num = 0;
		while ((float)num < this.objectSize.x)
		{
			int num2 = 0;
			while ((float)num2 < this.objectSize.y)
			{
				Vector2 nodeOffset;
				nodeOffset..ctor((float)(-(float)num), (float)(-(float)num2));
				FurnitureClass.BlockedAccess blockedAccess = new FurnitureClass.BlockedAccess();
				blockedAccess.nodeOffset = nodeOffset;
				blockedAccess.blocked.Add(CityData.BlockingDirection.behind);
				blockedAccess.blocked.Add(CityData.BlockingDirection.behindLeft);
				blockedAccess.blocked.Add(CityData.BlockingDirection.behindRight);
				blockedAccess.blocked.Add(CityData.BlockingDirection.front);
				blockedAccess.blocked.Add(CityData.BlockingDirection.frontLeft);
				blockedAccess.blocked.Add(CityData.BlockingDirection.frontRight);
				blockedAccess.blocked.Add(CityData.BlockingDirection.left);
				blockedAccess.blocked.Add(CityData.BlockingDirection.right);
				this.blockedAccess.Add(blockedAccess);
				num2++;
			}
			num++;
		}
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x001E5E1C File Offset: 0x001E401C
	[Button(null, 0)]
	public void BlockAllButFront()
	{
		this.blockedAccess.Clear();
		int num = 0;
		while ((float)num < this.objectSize.x)
		{
			int num2 = 0;
			while ((float)num2 < this.objectSize.y)
			{
				Vector2 nodeOffset;
				nodeOffset..ctor((float)(-(float)num), (float)(-(float)num2));
				FurnitureClass.BlockedAccess blockedAccess = new FurnitureClass.BlockedAccess();
				blockedAccess.nodeOffset = nodeOffset;
				blockedAccess.blocked.Add(CityData.BlockingDirection.behind);
				blockedAccess.blocked.Add(CityData.BlockingDirection.behindLeft);
				blockedAccess.blocked.Add(CityData.BlockingDirection.behindRight);
				blockedAccess.blocked.Add(CityData.BlockingDirection.frontLeft);
				blockedAccess.blocked.Add(CityData.BlockingDirection.frontRight);
				blockedAccess.blocked.Add(CityData.BlockingDirection.left);
				blockedAccess.blocked.Add(CityData.BlockingDirection.right);
				this.blockedAccess.Add(blockedAccess);
				num2++;
			}
			num++;
		}
	}

	// Token: 0x06002555 RID: 9557 RVA: 0x001E5EE8 File Offset: 0x001E40E8
	[Button(null, 0)]
	public void UpdatePreCalculatedLimits()
	{
		this.minimumZeroNodeWallCount = 0;
		this.maximumZeroNodeWallCount = 4;
		HashSet<CityData.BlockingDirection> hashSet = new HashSet<CityData.BlockingDirection>();
		HashSet<CityData.BlockingDirection> hashSet2 = new HashSet<CityData.BlockingDirection>();
		foreach (FurnitureClass.FurnitureWallRule furnitureWallRule in this.wallRules)
		{
			if (Mathf.RoundToInt(furnitureWallRule.nodeOffset.x) == 0 && Mathf.RoundToInt(furnitureWallRule.nodeOffset.y) == 0)
			{
				if (furnitureWallRule.option == FurnitureClass.FurnitureRuleOption.mustFeature)
				{
					if (furnitureWallRule.tag != FurnitureClass.WallRule.nothing)
					{
						if (!hashSet.Contains(furnitureWallRule.wallDirection))
						{
							hashSet.Add(furnitureWallRule.wallDirection);
						}
					}
					else if (!hashSet2.Contains(furnitureWallRule.wallDirection))
					{
						hashSet2.Add(furnitureWallRule.wallDirection);
					}
				}
				else if (furnitureWallRule.option == FurnitureClass.FurnitureRuleOption.cantFeature && furnitureWallRule.tag == FurnitureClass.WallRule.nothing && !hashSet.Contains(furnitureWallRule.wallDirection))
				{
					hashSet.Add(furnitureWallRule.wallDirection);
				}
			}
		}
		this.minimumZeroNodeWallCount = Mathf.Max(this.minimumZeroNodeWallCount, hashSet.Count);
		this.maximumZeroNodeWallCount = Mathf.Min(this.maximumZeroNodeWallCount, 4 - hashSet2.Count);
		if (!this.noAccessNeeded)
		{
			this.maximumZeroNodeWallCount = Mathf.Min(this.maximumZeroNodeWallCount, 3);
		}
	}

	// Token: 0x0400351A RID: 13594
	[Header("Rules")]
	[Tooltip("List of rules this furniture must follow")]
	public List<FurnitureClass.FurnitureWallRule> wallRules = new List<FurnitureClass.FurnitureWallRule>();

	// Token: 0x0400351B RID: 13595
	[Tooltip("List of rules this furniture must follow")]
	[Space(7f)]
	public List<FurnitureClass.FurnitureNodeRule> nodeRules = new List<FurnitureClass.FurnitureNodeRule>();

	// Token: 0x0400351C RID: 13596
	[Tooltip("Which points between nodes are blocked (no walking access)")]
	[Space(7f)]
	public List<FurnitureClass.BlockedAccess> blockedAccess = new List<FurnitureClass.BlockedAccess>();

	// Token: 0x0400351D RID: 13597
	[Space(7f)]
	[Tooltip("Add a custom node weight to these nodes")]
	public List<FurnitureClass.CustomNodeWeighting> customNodeWeights = new List<FurnitureClass.CustomNodeWeighting>();

	// Token: 0x0400351E RID: 13598
	[Header("PreCalculated Limits")]
	[OnValueChanged("UpdatePreCalculatedLimits")]
	public bool updatePreCalculated;

	// Token: 0x0400351F RID: 13599
	[ReadOnly]
	public int minimumZeroNodeWallCount;

	// Token: 0x04003520 RID: 13600
	[ReadOnly]
	public int maximumZeroNodeWallCount = 4;

	// Token: 0x04003521 RID: 13601
	[Header("Behaviour")]
	[Tooltip("If true, face the furniture diagonally if in corner")]
	public bool canFaceDiagonally;

	// Token: 0x04003522 RID: 13602
	[Space(7f)]
	[Tooltip("Maximum number per room")]
	public bool limitPerRoom = true;

	// Token: 0x04003523 RID: 13603
	[EnableIf("limitPerRoom")]
	[Range(1f, 20f)]
	public int maximumNumberPerRoom = 8;

	// Token: 0x04003524 RID: 13604
	[Tooltip("Maximum number per address")]
	public bool limitPerAddress = true;

	// Token: 0x04003525 RID: 13605
	[EnableIf("limitPerAddress")]
	[Range(1f, 20f)]
	public int maximumNumberPerAddress = 8;

	// Token: 0x04003526 RID: 13606
	[Tooltip("Allow only on this floor")]
	public bool limitToFloor;

	// Token: 0x04003527 RID: 13607
	[EnableIf("limitToFloor")]
	public int allowedOnFloor;

	// Token: 0x04003528 RID: 13608
	[Tooltip("Allow only on this range")]
	[DisableIf("limitToFloor")]
	public bool limitToFloorRange;

	// Token: 0x04003529 RID: 13609
	[EnableIf("limitToFloorRange")]
	public Vector2 allowedOnFloorRange = Vector2.zero;

	// Token: 0x0400352A RID: 13610
	public bool limitPerBuildingResidence;

	// Token: 0x0400352B RID: 13611
	[Tooltip("Limit to 1 per below number of residences in the building")]
	[EnableIf("limitPerBuildingResidence")]
	public int perBuildingResidences = 30;

	// Token: 0x0400352C RID: 13612
	public bool limitPerJobs;

	// Token: 0x0400352D RID: 13613
	[Tooltip("Limit to 1 per below number of residences in the building")]
	[EnableIf("limitPerJobs")]
	public int perJobs = 6;

	// Token: 0x0400352E RID: 13614
	[Space(7f)]
	[Tooltip("Must be at least this distance (nodes) from these classes...")]
	public List<FurnitureClass> awayFromClasses = new List<FurnitureClass>();

	// Token: 0x0400352F RID: 13615
	[Tooltip("Minimum node distance from these classes. A diagonal is about 1.8")]
	public float minimumNodeDistance;

	// Token: 0x04003530 RID: 13616
	[Header("Visuals")]
	public Vector2 objectSize = new Vector2(1f, 1f);

	// Token: 0x04003531 RID: 13617
	[Tooltip("If true this would cover up items on the wall such as lightswitches or block windows")]
	public bool tall;

	// Token: 0x04003532 RID: 13618
	[Tooltip("Use the corresponding wall rules to place wall pieces")]
	public bool wallPiece;

	// Token: 0x04003533 RID: 13619
	[HideIf("wallPiece")]
	[Tooltip("If being placed in decor mode, snap to nearby walls")]
	public bool useWallSnappingInDecorMode;

	// Token: 0x04003534 RID: 13620
	[Tooltip("Allow one of these per window")]
	public bool windowPiece;

	// Token: 0x04003535 RID: 13621
	[Tooltip("Does this block the placement of other furniture (if this flag is true on them also)?")]
	public bool occupiesTile = true;

	// Token: 0x04003536 RID: 13622
	[Tooltip("If this furniture allowed on stairwell tiles?")]
	public bool allowedOnStairwell;

	// Token: 0x04003537 RID: 13623
	[ShowIf("allowedOnStairwell")]
	public bool onlyOnStairwell;

	// Token: 0x04003538 RID: 13624
	[Tooltip("Determins allowed if no floor")]
	public bool allowIfNoFloor;

	// Token: 0x04003539 RID: 13625
	[Tooltip("Is this a ceiling piece?")]
	public bool ceilingPiece;

	// Token: 0x0400353A RID: 13626
	[Tooltip("Does this require a ceiling above it?")]
	[DisableIf("ceilingPiece")]
	public bool requiresCeiling;

	// Token: 0x0400353B RID: 13627
	[Tooltip("Does this block ceiling pieces from being placed?")]
	public bool blocksCeiling;

	// Token: 0x0400353C RID: 13628
	[Tooltip("This is allowed to be placed if there is a lightswitch on this tile")]
	public bool allowLightswitch = true;

	// Token: 0x0400353D RID: 13629
	[Tooltip("If a lightswitch exists here, raise the height slightly")]
	[EnableIf("allowLightswitch")]
	public bool raiseLightswitch;

	// Token: 0x0400353E RID: 13630
	[EnableIf("raiseLightswitch")]
	public float lightswitchYOffset = 1.5f;

	// Token: 0x0400353F RID: 13631
	[Header("AI")]
	[Tooltip("If true this object doesn't need access and won't ever block anything; use on minor objects to optimize placement checks")]
	public bool noBlocking;

	// Token: 0x04003540 RID: 13632
	[Tooltip("If true the AI can access this node, but not pass through it on the way to something else. Can be used for 1 node items such as tables where you can't normally block access. IMPORTANT: Usually true if all but 1-3 directions are blocked.")]
	public bool noPassThrough;

	// Token: 0x04003541 RID: 13633
	[Tooltip("If true the AI can't access this node, this node is effectively exluded from access checks. IMPORTANT: Make sure this is enabled if all directions are blocked.")]
	public bool noAccessNeeded;

	// Token: 0x04003542 RID: 13634
	[Tooltip("If true then default sublocations will be blocked completely (usually they are added if there are no custom ones on a node). Default sublocations will be used if there are no custom ones, and there is no furniture class with a blocking flag.")]
	public bool blockDefaultSublocations;

	// Token: 0x04003543 RID: 13635
	[Tooltip("If true then the physics check will ignore colliders that are not citizens")]
	public bool ignoreGeometryInPhysicsCheck;

	// Token: 0x04003544 RID: 13636
	[Tooltip("These will be added to the tile's sublocations")]
	public List<FurnitureClass.FurniureWalkSubLocations> sublocations = new List<FurnitureClass.FurniureWalkSubLocations>();

	// Token: 0x04003545 RID: 13637
	[Tooltip("If the AI is robbing a location, use this to compare the likihood of valuable contents...")]
	[Range(0f, 10f)]
	public int aiRobberyPriority = 3;

	// Token: 0x04003546 RID: 13638
	public bool isSecurityCamera;

	// Token: 0x04003547 RID: 13639
	[Header("Ownership")]
	[Tooltip("Dictates what class of ownership (ie each person living here needs 1 bed)")]
	public FurnitureClass.OwnershipClass ownershipClass;

	// Token: 0x04003548 RID: 13640
	[Tooltip("From where does this derrive the owners pool?")]
	public FurnitureClass.OwnershipSource ownershipSource;

	// Token: 0x04003549 RID: 13641
	[Tooltip("Assign owners to this furniture")]
	public int assignBelongsToOwners;

	// Token: 0x0400354A RID: 13642
	[Tooltip("If this is checked the game will assign this object to a couple")]
	public bool preferCouples;

	// Token: 0x0400354B RID: 13643
	[Tooltip("If this is true, the object will try to copy ownership from previously placed items in the cluster")]
	public bool copyFromPreviouslyPlacedInCluster;

	// Token: 0x0400354C RID: 13644
	[Tooltip("If this is true, this will only pick from the owners of the room (if there are any)")]
	public bool onlyPickFromRoomOwners;

	// Token: 0x0400354D RID: 13645
	[Tooltip("Skip placement of this if there are no address owners")]
	public bool skipIfNoAddressInhabitants;

	// Token: 0x0400354E RID: 13646
	[Tooltip("Assign homeless owners")]
	public bool assignHomelessOwners;

	// Token: 0x0400354F RID: 13647
	[Tooltip("Make sure ownership is only assigned if mailbox not already assigned to an apartment")]
	public bool assignMailbox;

	// Token: 0x04003550 RID: 13648
	[Tooltip("Don't allow mission photographs on this furniture")]
	public bool discourageMissionPhotos;

	// Token: 0x04003551 RID: 13649
	[Header("Copy")]
	public FurnitureClass copyFrom;

	// Token: 0x02000726 RID: 1830
	public enum FurnitureRuleOption
	{
		// Token: 0x04003553 RID: 13651
		mustFeature,
		// Token: 0x04003554 RID: 13652
		cantFeature,
		// Token: 0x04003555 RID: 13653
		canFeature
	}

	// Token: 0x02000727 RID: 1831
	public enum WallRule
	{
		// Token: 0x04003557 RID: 13655
		nothing,
		// Token: 0x04003558 RID: 13656
		wall,
		// Token: 0x04003559 RID: 13657
		window,
		// Token: 0x0400355A RID: 13658
		windowLarge,
		// Token: 0x0400355B RID: 13659
		entrance,
		// Token: 0x0400355C RID: 13660
		ventUpper,
		// Token: 0x0400355D RID: 13661
		ventLower,
		// Token: 0x0400355E RID: 13662
		wallOrUpperVent,
		// Token: 0x0400355F RID: 13663
		ventTop,
		// Token: 0x04003560 RID: 13664
		entranceDoorOnly,
		// Token: 0x04003561 RID: 13665
		entranceToRoomOfType,
		// Token: 0x04003562 RID: 13666
		anyWindow,
		// Token: 0x04003563 RID: 13667
		entraceDivider,
		// Token: 0x04003564 RID: 13668
		securityDoorDivider,
		// Token: 0x04003565 RID: 13669
		fence,
		// Token: 0x04003566 RID: 13670
		addressEntrance,
		// Token: 0x04003567 RID: 13671
		lightswitch
	}

	// Token: 0x02000728 RID: 1832
	[Serializable]
	public class FurniureWalkSubLocations
	{
		// Token: 0x04003568 RID: 13672
		[Tooltip("This rule is applied at this offset")]
		public Vector2 offset = Vector2.zero;

		// Token: 0x04003569 RID: 13673
		public List<Vector3> sublocations = new List<Vector3>();
	}

	// Token: 0x02000729 RID: 1833
	[Serializable]
	public class FurnitureNodeRule
	{
		// Token: 0x0400356A RID: 13674
		[Tooltip("This rule is applied at this offset")]
		public Vector2 offset = Vector2.zero;

		// Token: 0x0400356B RID: 13675
		[Tooltip("Type of rule to apply")]
		public FurnitureClass.FurnitureRuleOption option;

		// Token: 0x0400356C RID: 13676
		public bool anyOccupiedTile;

		// Token: 0x0400356D RID: 13677
		[HideIf("anyOccupiedTile")]
		[Tooltip("What should be found at this node?")]
		public FurnitureClass furnitureClass;

		// Token: 0x0400356E RID: 13678
		[Range(-10f, 10f)]
		[Tooltip("If 'Can Feature' add this to the location score")]
		public int addScore;
	}

	// Token: 0x0200072A RID: 1834
	[Serializable]
	public class FurnitureWallRule
	{
		// Token: 0x0400356F RID: 13679
		[Tooltip("This rule is applied at this offset")]
		public Vector2 nodeOffset = Vector2.zero;

		// Token: 0x04003570 RID: 13680
		[Tooltip("This rule is applied at this offset")]
		public CityData.BlockingDirection wallDirection;

		// Token: 0x04003571 RID: 13681
		[Tooltip("Type of rule to apply")]
		public FurnitureClass.FurnitureRuleOption option;

		// Token: 0x04003572 RID: 13682
		[Tooltip("What should be found at this offset?")]
		public FurnitureClass.WallRule tag;

		// Token: 0x04003573 RID: 13683
		[Tooltip("If the tag is 'room to'")]
		public RoomConfiguration roomType;

		// Token: 0x04003574 RID: 13684
		[Tooltip("If 'Can Feature' add this to the location score")]
		[Range(-10f, 10f)]
		public int addScore;
	}

	// Token: 0x0200072B RID: 1835
	[Serializable]
	public class BlockedAccess
	{
		// Token: 0x04003575 RID: 13685
		public bool disabled;

		// Token: 0x04003576 RID: 13686
		public Vector2 nodeOffset = Vector2.zero;

		// Token: 0x04003577 RID: 13687
		[Tooltip("Block diagonals on adjacent tiles")]
		public bool blockExteriorDiagonals;

		// Token: 0x04003578 RID: 13688
		public List<CityData.BlockingDirection> blocked = new List<CityData.BlockingDirection>();
	}

	// Token: 0x0200072C RID: 1836
	[Serializable]
	public class CustomNodeWeighting
	{
		// Token: 0x04003579 RID: 13689
		public bool disabled;

		// Token: 0x0400357A RID: 13690
		public Vector2 nodeOffset = Vector2.zero;

		// Token: 0x0400357B RID: 13691
		public float nodeWeightModifier = 1f;
	}

	// Token: 0x0200072D RID: 1837
	[Serializable]
	public class SubObject
	{
		// Token: 0x0400357C RID: 13692
		public SubObjectClassPreset preset;

		// Token: 0x0400357D RID: 13693
		public string parent;

		// Token: 0x0400357E RID: 13694
		public Vector3 localPos;

		// Token: 0x0400357F RID: 13695
		public Vector3 localRot;
	}

	// Token: 0x0200072E RID: 1838
	public enum OwnershipClass
	{
		// Token: 0x04003581 RID: 13697
		none,
		// Token: 0x04003582 RID: 13698
		bed,
		// Token: 0x04003583 RID: 13699
		desk,
		// Token: 0x04003584 RID: 13700
		locker,
		// Token: 0x04003585 RID: 13701
		drawers,
		// Token: 0x04003586 RID: 13702
		noticeBoard,
		// Token: 0x04003587 RID: 13703
		safe,
		// Token: 0x04003588 RID: 13704
		mailboxes
	}

	// Token: 0x0200072F RID: 1839
	public enum OwnershipSource
	{
		// Token: 0x0400358A RID: 13706
		addressInhabitants,
		// Token: 0x0400358B RID: 13707
		buildingResidences
	}
}
