using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007F9 RID: 2041
public class GameplayControls : MonoBehaviour
{
	// Token: 0x1700011F RID: 287
	// (get) Token: 0x0600260A RID: 9738 RVA: 0x001E93D4 File Offset: 0x001E75D4
	public static GameplayControls Instance
	{
		get
		{
			return GameplayControls._instance;
		}
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x001E93DC File Offset: 0x001E75DC
	private void Awake()
	{
		if (GameplayControls._instance != null && GameplayControls._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GameplayControls._instance = this;
		}
		this.startingDate = Mathf.Max(this.startingDate, 0);
		this.startingMonth = Mathf.Max(this.startingMonth, 0);
		this.startingYear = Mathf.Max(this.startingYear, 1);
		while (this.yearZeroLeapYearCycle >= 4)
		{
			this.yearZeroLeapYearCycle -= 4;
		}
		while (this.dayZero >= 7)
		{
			this.dayZero -= 7;
		}
	}

	// Token: 0x04004091 RID: 16529
	[Header("Cut Scenes")]
	public CutScenePreset intro;

	// Token: 0x04004092 RID: 16530
	public CutScenePreset outro;

	// Token: 0x04004093 RID: 16531
	[Header("Time")]
	public SessionData.TimeSpeed startingTimeSpeed = SessionData.TimeSpeed.normal;

	// Token: 0x04004094 RID: 16532
	[ReorderableList]
	[Tooltip("0 = slomo, 1 = normal, 2 = fast, 3 = ultrafast, 4 = sim")]
	public List<float> timeMultipliers = new List<float>();

	// Token: 0x04004095 RID: 16533
	public int startingDate;

	// Token: 0x04004096 RID: 16534
	public int startingMonth;

	// Token: 0x04004097 RID: 16535
	public int startingYear = 1;

	// Token: 0x04004098 RID: 16536
	[Tooltip("Only time cycles idependent of the actual time data are needed here")]
	public int yearZeroLeapYearCycle = 1;

	// Token: 0x04004099 RID: 16537
	public int dayZero = 6;

	// Token: 0x0400409A RID: 16538
	public int publicYearZero = 1978;

	// Token: 0x0400409B RID: 16539
	[Header("Routines")]
	public float routineUpdateFrequency = 0.5f;

	// Token: 0x0400409C RID: 16540
	public float gameWorldUpdateFrequency = 0.75f;

	// Token: 0x0400409D RID: 16541
	public float doorSequenceUpdateFrequency = 0.15f;

	// Token: 0x0400409E RID: 16542
	public float stealthModeLoopUpdateFrequency = 0.1f;

	// Token: 0x0400409F RID: 16543
	[Tooltip("Player height (normal)")]
	[Header("First Person")]
	public float playerHeightNormal = 1.64f;

	// Token: 0x040040A0 RID: 16544
	[Tooltip("Player height (crouched)")]
	public float playerHeightCrouched = 0.96f;

	// Token: 0x040040A1 RID: 16545
	public AnimationCurve crouchHeightCurve;

	// Token: 0x040040A2 RID: 16546
	public AnimationCurve leanCurve;

	// Token: 0x040040A3 RID: 16547
	public AnimationCurve joltCurve;

	// Token: 0x040040A4 RID: 16548
	[Tooltip("FPS Camera Y as offset of centre of player")]
	public float cameraHeightNormal = 0.78f;

	// Token: 0x040040A5 RID: 16549
	[Tooltip("FPS Camera Y as offset of centre of player")]
	public float cameraHeightCrouched = 0.68f;

	// Token: 0x040040A6 RID: 16550
	[Tooltip("Range from which things can be interacted-with by the player")]
	public float interactionRange = 1.85f;

	// Token: 0x040040A7 RID: 16551
	[Tooltip("Range from which things can be read by the player")]
	public float readingRange = 1.2f;

	// Token: 0x040040A8 RID: 16552
	[Tooltip("The distance from which things are carried by the player")]
	public float carryDistance = 1f;

	// Token: 0x040040A9 RID: 16553
	[Tooltip("The throw force of the player")]
	public float throwForce = 500f;

	// Token: 0x040040AA RID: 16554
	[Tooltip("Field of view (Normal)")]
	public float fovNormal = 70f;

	// Token: 0x040040AB RID: 16555
	[Tooltip("Field of view (interaction)")]
	public float fovInteraction = 40f;

	// Token: 0x040040AC RID: 16556
	[Tooltip("How much FPS model lag")]
	public float fpsModelLag = 60f;

	// Token: 0x040040AD RID: 16557
	[Tooltip("Player walk speed")]
	public float playerWalkSpeed = 4f;

	// Token: 0x040040AE RID: 16558
	[Tooltip("Player run speed")]
	public float playerRunSpeed = 8f;

	// Token: 0x040040AF RID: 16559
	[Tooltip("Jump height")]
	public float jumpHeight = 3.5f;

	// Token: 0x040040B0 RID: 16560
	[Tooltip("Player stealth mode walk multiplier")]
	public float playerStealthWalkMuliplier = 0.5f;

	// Token: 0x040040B1 RID: 16561
	[Tooltip("Player stealth mode run multiplier")]
	public float playerStealthRunMultiplier = 0.9f;

	// Token: 0x040040B2 RID: 16562
	[Tooltip("Head bob multiplier")]
	public float headBobMultiplier = 0.55f;

	// Token: 0x040040B3 RID: 16563
	[Tooltip("Player height multiplier in air ducts")]
	public float ductPlayerHeight = 0.25f;

	// Token: 0x040040B4 RID: 16564
	[Tooltip("Camera height multiplier in air ducts")]
	public float ductCamHeight = 2.4f;

	// Token: 0x040040B5 RID: 16565
	[Tooltip("Player height boost on enter air duct")]
	public float ductPlayerPosY = -0.5f;

	// Token: 0x040040B6 RID: 16566
	[Tooltip("Air duct entry point")]
	public Vector3 airDuctEntry = new Vector3(-0.35f, 0f, 0.05f);

	// Token: 0x040040B7 RID: 16567
	[Tooltip("Air duct exit point")]
	public Vector3 airDuctExit = new Vector3(0f, 0f, 0.5f);

	// Token: 0x040040B8 RID: 16568
	[Tooltip("Normal skin width")]
	public float normalSkinWidth = 0.08f;

	// Token: 0x040040B9 RID: 16569
	[Tooltip("Carrying skin width")]
	public float carryingSkinWidth = 0.2f;

	// Token: 0x040040BA RID: 16570
	[Tooltip("Normal skin width")]
	public float ductSkinWidth = 0.011f;

	// Token: 0x040040BB RID: 16571
	[Tooltip("Default return transition")]
	public PlayerTransitionPreset defaultReturnTransition;

	// Token: 0x040040BC RID: 16572
	public PlayerTransitionPreset enterVentTransition;

	// Token: 0x040040BD RID: 16573
	public PlayerTransitionPreset exitVentTransition;

	// Token: 0x040040BE RID: 16574
	public PlayerTransitionPreset citizensArrestTranstion;

	// Token: 0x040040BF RID: 16575
	public PlayerTransitionPreset citizenTalkToTransition;

	// Token: 0x040040C0 RID: 16576
	public PlayerTransitionPreset doorPeekEnter;

	// Token: 0x040040C1 RID: 16577
	public PlayerTransitionPreset doorPeekExit;

	// Token: 0x040040C2 RID: 16578
	public PlayerTransitionPreset lockpickEnter;

	// Token: 0x040040C3 RID: 16579
	public PlayerTransitionPreset lockpickExit;

	// Token: 0x040040C4 RID: 16580
	public PlayerTransitionPreset sabotageEnter;

	// Token: 0x040040C5 RID: 16581
	public PlayerTransitionPreset sabotageExit;

	// Token: 0x040040C6 RID: 16582
	public PlayerTransitionPreset bargeDoorEnter;

	// Token: 0x040040C7 RID: 16583
	public PlayerTransitionPreset bargeDoorFail;

	// Token: 0x040040C8 RID: 16584
	public PlayerTransitionPreset bargeDoorSuccess;

	// Token: 0x040040C9 RID: 16585
	public PlayerTransitionPreset punchedReaction;

	// Token: 0x040040CA RID: 16586
	public PlayerTransitionPreset playerKO;

	// Token: 0x040040CB RID: 16587
	public PlayerTransitionPreset playerUseComputer;

	// Token: 0x040040CC RID: 16588
	public PlayerTransitionPreset playerComputerExit;

	// Token: 0x040040CD RID: 16589
	public PlayerTransitionPreset playerTakePrint;

	// Token: 0x040040CE RID: 16590
	public PlayerTransitionPreset playerTakePrintExit;

	// Token: 0x040040CF RID: 16591
	public PlayerTransitionPreset playerSearch;

	// Token: 0x040040D0 RID: 16592
	public PlayerTransitionPreset playerSearchExit;

	// Token: 0x040040D1 RID: 16593
	public PlayerTransitionPreset focusOnInteractable;

	// Token: 0x040040D2 RID: 16594
	[Header("Ragdolls")]
	[Tooltip("Force applied when the player drags a body")]
	public float dragForceAmount = 1520f;

	// Token: 0x040040D3 RID: 16595
	[Tooltip("Amount of rotational camera movement allowed per second when dragging a body")]
	public float maxAngleMovementWhenDragging = 70f;

	// Token: 0x040040D4 RID: 16596
	[Tooltip("How far infront the player the ragdoll is held")]
	public float ragdollCarryMaxDistance = 0.5f;

	// Token: 0x040040D5 RID: 16597
	[Tooltip("Ragdoll preprocessing: Disabling preprocessing helps to stabilize impossible-to-fulfil configurations.")]
	public bool ragdollJointPreprocessing;

	// Token: 0x040040D6 RID: 16598
	public bool ragdollJointCollision;

	// Token: 0x040040D7 RID: 16599
	public bool ragdollJointProjection = true;

	// Token: 0x040040D8 RID: 16600
	public float ragdollJointContactDistance = 0.1f;

	// Token: 0x040040D9 RID: 16601
	public bool ragdollRigidbodyCollision = true;

	// Token: 0x040040DA RID: 16602
	public float ragdollJointBounce;

	// Token: 0x040040DB RID: 16603
	public float ragdollJointDampen = 50f;

	// Token: 0x040040DC RID: 16604
	public float ragdollJointSpring;

	// Token: 0x040040DD RID: 16605
	[Header("Depth of Field")]
	public float dofNormalNearStart;

	// Token: 0x040040DE RID: 16606
	public float dofNormalNearEnd;

	// Token: 0x040040DF RID: 16607
	public float dofNormalFarStart;

	// Token: 0x040040E0 RID: 16608
	public float dofNormalFarEnd;

	// Token: 0x040040E1 RID: 16609
	[Space(7f)]
	public float dofTalkingNearStart;

	// Token: 0x040040E2 RID: 16610
	public float dofTalkingNearEnd;

	// Token: 0x040040E3 RID: 16611
	public float dofTalkingFarStart;

	// Token: 0x040040E4 RID: 16612
	public float dofTalkingFarEnd;

	// Token: 0x040040E5 RID: 16613
	[Space(7f)]
	public float dofPausedNearStart;

	// Token: 0x040040E6 RID: 16614
	public float dofPausedNearEnd;

	// Token: 0x040040E7 RID: 16615
	public float dofPausedFarStart;

	// Token: 0x040040E8 RID: 16616
	public float dofPausedFarEnd;

	// Token: 0x040040E9 RID: 16617
	[Space(7f)]
	public float dofCityEditNearStart;

	// Token: 0x040040EA RID: 16618
	public float dofCityEditNearEnd;

	// Token: 0x040040EB RID: 16619
	public float dofCityEditFarStart = 275f;

	// Token: 0x040040EC RID: 16620
	public float dofCityEditFarEnd = 800f;

	// Token: 0x040040ED RID: 16621
	[Space(7f)]
	public float dofChangeTime = 0.25f;

	// Token: 0x040040EE RID: 16622
	[Tooltip("Start the game with these first person items")]
	[ReorderableList]
	public List<FirstPersonItem> startingItems = new List<FirstPersonItem>();

	// Token: 0x040040EF RID: 16623
	public FirstPersonItem nothingItem;

	// Token: 0x040040F0 RID: 16624
	public FirstPersonItem watchItem;

	// Token: 0x040040F1 RID: 16625
	public FirstPersonItem fistsItem;

	// Token: 0x040040F2 RID: 16626
	public FirstPersonItem coinItem;

	// Token: 0x040040F3 RID: 16627
	public FirstPersonItem printReader;

	// Token: 0x040040F4 RID: 16628
	[Tooltip("How long to display the item switch interface when it is activated.")]
	public float itemSwitchCounter = 2f;

	// Token: 0x040040F5 RID: 16629
	[Tooltip("Curve for ambient light levels throughout the day")]
	[Header("Stealth")]
	public AnimationCurve stealthAmbientLightLevel;

	// Token: 0x040040F6 RID: 16630
	[Tooltip("The above is multiplied by this when inside to give ambient level")]
	public float interiorAmbientLightMultiplier = 0.1f;

	// Token: 0x040040F7 RID: 16631
	[Tooltip("Transform for the floor light measuring point (the camera is used for the upper)")]
	public Transform floorLightMeasure;

	// Token: 0x040040F8 RID: 16632
	[Tooltip("Curve for direct sun light levels throughout the day")]
	public AnimationCurve stealthSunLightLevel;

	// Token: 0x040040F9 RID: 16633
	[Tooltip("How long in gametime a building alarm lasts once triggered: From high to low so we can lerp with skill multiplier")]
	public Vector2 buildingAlarmTime = new Vector2(0.4f, 0.1f);

	// Token: 0x040040FA RID: 16634
	[Tooltip("How fast a camera/turret tracks it's target once alert")]
	public float securityTrackSpeed = 12f;

	// Token: 0x040040FB RID: 16635
	[Tooltip("Citizen FoV")]
	public float citizenFOV = 160f;

	// Token: 0x040040FC RID: 16636
	[Tooltip("Security FoV")]
	public float securityFOV = 80f;

	// Token: 0x040040FD RID: 16637
	[Tooltip("Sabotage land value multiplier")]
	public float sabotageLandValueMP = 0.5f;

	// Token: 0x040040FE RID: 16638
	[Tooltip("Citizen max sight range")]
	public float citizenSightRange = 25f;

	// Token: 0x040040FF RID: 16639
	[Tooltip("Security max sight range")]
	public float securitySightRange = 16f;

	// Token: 0x04004100 RID: 16640
	[Tooltip("Minimum stealth detection threshold. If target is closer than this, even targets with 0 visibility are spotted")]
	public float minimumStealthDetectionRange = 0.75f;

	// Token: 0x04004101 RID: 16641
	[Tooltip("Sentry gun weapon config")]
	public MurderWeaponPreset sentryGunWeapon;

	// Token: 0x04004102 RID: 16642
	[Tooltip("Sentry gun rate of fire")]
	public float sentryGunROF = 5f;

	// Token: 0x04004103 RID: 16643
	[Tooltip("Sentry gun cone of fire")]
	public float sentryGunDamage = 0.25f;

	// Token: 0x04004104 RID: 16644
	[Tooltip("Sentry gun cone of fire")]
	public float sentryGunAccuracy = 0.1f;

	// Token: 0x04004105 RID: 16645
	[Tooltip("Maximum distance at which the player can officially 'spot' a citizen (used for triggering outlines etc)")]
	public float playerMaxSpotDistance = 20f;

	// Token: 0x04004106 RID: 16646
	[Tooltip("Perform a player spotting check every x frames")]
	public int playerSpotUpdateEveryXFrame = 5;

	// Token: 0x04004107 RID: 16647
	[Tooltip("The time before a spotted actor becomes invisible to the player (seconds)")]
	public float spottedGraceTime = 4f;

	// Token: 0x04004108 RID: 16648
	[Tooltip("The time it takes for a previously spotted actor to become invisible to the player again (seconds)")]
	public float spottedFadeSpeed = 1f;

	// Token: 0x04004109 RID: 16649
	[Tooltip("While sighted, the grace time multiplier for spotting a person is x1, how long is it for hearing a player?")]
	public float audioOnlySpotGraceTimeMultiplier = 0.5f;

	// Token: 0x0400410A RID: 16650
	[Tooltip("Maximum distance for surveillance capturing the image of the player")]
	public float playerImageCaptureMaxRange = 14f;

	// Token: 0x0400410B RID: 16651
	[Tooltip("If security catches player, any fines are now active for this long...")]
	public float buildingWantedTime = 24f;

	// Token: 0x0400410C RID: 16652
	[Tooltip("How long before breakers are reset")]
	public float breakerResetTime = 0.25f;

	// Token: 0x0400410D RID: 16653
	[Tooltip("How long before turned off security is reactivated")]
	public float securityResetTime = 2f;

	// Token: 0x0400410E RID: 16654
	[Tooltip("How long in gametime does it take for a room to fill up with toxic gas")]
	public float gasFillTime = 0.1f;

	// Token: 0x0400410F RID: 16655
	[Tooltip("How long in gametime does it take for a room to empty of toxic gas")]
	public float gasEmptyTime = 0.25f;

	// Token: 0x04004110 RID: 16656
	[Space(7f)]
	[Tooltip("How much time spent at a location trespassing before +1 is added to the escalation level")]
	public float additionalEscalationTime = 0.1f;

	// Token: 0x04004111 RID: 16657
	[Header("Skills")]
	[Tooltip("Start the game with this amount of money")]
	public int startingMoney = 100;

	// Token: 0x04004112 RID: 16658
	[Tooltip("Start the game with this presence level")]
	public int startingLockpicks = 3;

	// Token: 0x04004113 RID: 16659
	[Tooltip("How much lock strength can be used up by a single lockpick as a range dictated by skill")]
	public Vector2 lockpickEffectivenessRange = new Vector2(0.2f, 1f);

	// Token: 0x04004114 RID: 16660
	[Tooltip("Lockpicking speed multiplier")]
	public Vector2 lockpickSpeedRange = new Vector2(0.4f, 1f);

	// Token: 0x04004115 RID: 16661
	[Tooltip("How much door strength damage is done when barged")]
	public Vector2 bargeDamageRange = new Vector2(0.05f, 0.1f);

	// Token: 0x04004116 RID: 16662
	[Space(7f)]
	public float baseMaxPlayerHealth = 1f;

	// Token: 0x04004117 RID: 16663
	[Tooltip("The player recovers this amount of health (normalized) over time (game time 1 hour)")]
	public float playerRecoveryRate = 0.1f;

	// Token: 0x04004118 RID: 16664
	[Tooltip("The player's starting combat skill")]
	public float playerCombatSkill = 1f;

	// Token: 0x04004119 RID: 16665
	[Tooltip("The player's starting combat heft (damage per punch)")]
	public float playerCombatHeft = 0.25f;

	// Token: 0x0400411A RID: 16666
	[Tooltip("The default number of inventory slots")]
	public int defaultInventorySlots = 3;

	// Token: 0x0400411B RID: 16667
	[Tooltip("Damage multiplier for physics objects hitting the player")]
	public float incomingPlayerPhysicsDamageMultiplier = 0.5f;

	// Token: 0x0400411C RID: 16668
	[Space(7f)]
	public float commonSyncDisksPer200Citizens = 7f;

	// Token: 0x0400411D RID: 16669
	public float mediumSyncDisksPer200Citizens = 5f;

	// Token: 0x0400411E RID: 16670
	public float rareSyncDisksPer200Citizens = 3f;

	// Token: 0x0400411F RID: 16671
	public float veryRareSyncDisksPer200Citizens = 1f;

	// Token: 0x04004120 RID: 16672
	[Space(7f)]
	public int corpSabotageMoney = 200;

	// Token: 0x04004121 RID: 16673
	public int corpSabotageManagementBonus = 100;

	// Token: 0x04004122 RID: 16674
	public int moneyForAddresses = 20;

	// Token: 0x04004123 RID: 16675
	public int moneyForNewLocations = 20;

	// Token: 0x04004124 RID: 16676
	public int moneyForAirDucts = 20;

	// Token: 0x04004125 RID: 16677
	public int moneyForPasscodes = 100;

	// Token: 0x04004126 RID: 16678
	public int moneyForReading = 20;

	// Token: 0x04004127 RID: 16679
	public int moneyForStreetCleaning = 2;

	// Token: 0x04004128 RID: 16680
	public int passiveIncome = 10;

	// Token: 0x04004129 RID: 16681
	public float upgradeHeightModifier = 0.5f;

	// Token: 0x0400412A RID: 16682
	public float upgradeRunSpeed = 2f;

	// Token: 0x0400412B RID: 16683
	public float upgradeReach = 1.5f;

	// Token: 0x0400412C RID: 16684
	public float upgradeHealth = 1f;

	// Token: 0x0400412D RID: 16685
	public float upgradeRegen = 0.1f;

	// Token: 0x0400412E RID: 16686
	[Tooltip("Applied to all fines: From high to lower and lerped with the health insurance skill")]
	public Vector2 legalInsuranceMultiplier = new Vector2(1f, 0.2f);

	// Token: 0x0400412F RID: 16687
	[Header("Social Credit")]
	public int socialCreditForLostAndFound = 40;

	// Token: 0x04004130 RID: 16688
	public int socialCreditForSideJobs = 50;

	// Token: 0x04004131 RID: 16689
	public int socialCreditForMurders = 200;

	// Token: 0x04004132 RID: 16690
	public AnimationCurve socialCreditLevelCurve;

	// Token: 0x04004133 RID: 16691
	[Header("Evidence")]
	[Tooltip("Hot food is warm for this time after purchase")]
	public float foodHotTime = 1f;

	// Token: 0x04004134 RID: 16692
	public float timeOfDeathAccuracy = 1f;

	// Token: 0x04004135 RID: 16693
	public EvidencePreset retailItemSoldDiscovery;

	// Token: 0x04004136 RID: 16694
	public EvidencePreset retailItemNoSoldDiscovery;

	// Token: 0x04004137 RID: 16695
	[Header("First Person Skin Materials")]
	public Material fistMaterial;

	// Token: 0x04004138 RID: 16696
	public Material fingerUpperMaterial;

	// Token: 0x04004139 RID: 16697
	public Material fingerLowerMaterial;

	// Token: 0x0400413A RID: 16698
	public Material fingerTipMaterial;

	// Token: 0x0400413B RID: 16699
	public Material thumbJointMaterial;

	// Token: 0x0400413C RID: 16700
	[Header("Physics")]
	[Tooltip("Physics object interpolation: 'By default interpolation is turned off. Commonly rigidbody interpolation is used on the player's character. Physics is running at discrete timesteps, while graphics is renderered at variable frame rates. This can lead to jittery looking objects, because physics and graphics are not completely in sync. The effect is subtle but often visible on the player character, especially if a camera follows the main character. It is recommended to turn on interpolation for the main character but disable it for everything else.'")]
	public RigidbodyInterpolation interpolation = 1;

	// Token: 0x0400413D RID: 16701
	public float physicsOffTime = 3f;

	// Token: 0x0400413E RID: 16702
	public PhysicsProfile defaultObjectPhysicsProfile;

	// Token: 0x0400413F RID: 16703
	[Header("Trash")]
	[Tooltip("The world is allowed to have this much trash before it starts removing...")]
	public int globalCreatedTrashLimit = 500;

	// Token: 0x04004140 RID: 16704
	[Tooltip("Trash limit per bin")]
	public int binTrashLimit = 12;

	// Token: 0x04004141 RID: 16705
	[Header("Calls")]
	[Tooltip("Each building logs this many phone calls")]
	public int buildingCallLogMax = 100;

	// Token: 0x04004142 RID: 16706
	[Header("Citizens")]
	[Tooltip("Multiply citizen speed in presimulation by this amount")]
	public float preSimSpeedMultiplier = 1.5f;

	// Token: 0x04004143 RID: 16707
	[Tooltip("How much money a wallet contains based on citizien class")]
	public AnimationCurve walletCashAmountBasedOnWealth;

	// Token: 0x04004144 RID: 16708
	public CharacterTrait creditCardTrait;

	// Token: 0x04004145 RID: 16709
	public CharacterTrait donorCardTrait;

	// Token: 0x04004146 RID: 16710
	[Header("Combat")]
	[Tooltip("How close the block has to land for a successful block (total range = 1.0)")]
	public float successfulBlockThreshold = 0.1f;

	// Token: 0x04004147 RID: 16711
	[Tooltip("How close the block has to land for a perfect block (total range = 1.0)")]
	public float perfectBlockThreshold = 0.01f;

	// Token: 0x04004148 RID: 16712
	[Tooltip("The minimum base attack delay (AI time between attack) in seconds. Modified by combat skill using left from min to max.")]
	public Vector2 baseAttackDelay = new Vector2(0.5f, 0.2f);

	// Token: 0x04004149 RID: 16713
	[Tooltip("Blocking will use this base attack delay instead of the above. Modified by combat skill using left from min to max")]
	public Vector2 blockedAttackDelay = new Vector2(0.8f, 0.6f);

	// Token: 0x0400414A RID: 16714
	[Tooltip("Blocking perfectly use this base attack delay instead of the above. Modified by combat skill using left from min to max")]
	public Vector2 perfectBlockAttackDelay = new Vector2(2f, 1.5f);

	// Token: 0x0400414B RID: 16715
	[Tooltip("How much time before an enemy gets up after being KO'd (game time)")]
	public Vector2 koTimeRange = new Vector2(1f, 1f);

	// Token: 0x0400414C RID: 16716
	[Tooltip("The force applied to an NPC ragdoll on KO")]
	public float playerKOPunchForce = 200f;

	// Token: 0x0400414D RID: 16717
	[Tooltip("How much time passes when the player is KO'd (in-game hours)")]
	public float koTimePass = 5f;

	// Token: 0x0400414E RID: 16718
	[Tooltip("How long (game time) will a citizen be restrained by handcuffs?")]
	public float restrainedTimer = 1f;

	// Token: 0x0400414F RID: 16719
	[Tooltip("When using a takedown, how long a citizen will stay down for")]
	public float takedownTimer = 0.5f;

	// Token: 0x04004150 RID: 16720
	[Tooltip("The fuse for a thrown grenade")]
	public float thrownGrenadeFuse = 3f;

	// Token: 0x04004151 RID: 16721
	[Tooltip("The fuse for a proxy grenade")]
	public float proxyGrenadeFuse = 2.5f;

	// Token: 0x04004152 RID: 16722
	[Tooltip("Blood amount multiplier")]
	public float bloodAmountMultiplier = 1f;

	// Token: 0x04004153 RID: 16723
	[Space(5f)]
	public PlayerTransitionPreset successfulBlockTransition;

	// Token: 0x04004154 RID: 16724
	public PlayerTransitionPreset unsuccessfulBlockTransition;

	// Token: 0x04004155 RID: 16725
	public PlayerTransitionPreset counterTransition;

	// Token: 0x04004156 RID: 16726
	[Header("Tailing")]
	public float maxPlayerLookAtTailingDistance = 12f;

	// Token: 0x04004157 RID: 16727
	[Tooltip("How fast the AI gains spooked while being looked at")]
	public float playerLookAtSpookRate = 0.2f;

	// Token: 0x04004158 RID: 16728
	public float loseSpookedRate = -0.015f;

	// Token: 0x04004159 RID: 16729
	public AnimationCurve screenCentreSpookCurve;

	// Token: 0x0400415A RID: 16730
	[Tooltip("Chance for player to be mugged if the conditions are right")]
	[Header("Mugging")]
	public float muggingChance = 1f;

	// Token: 0x0400415B RID: 16731
	[Header("Cleanup")]
	[Tooltip("Spatter removal time: Only applys when the spatter simulation is set to use this erase mode. In-game hours.")]
	public float spatterRemovalTime = 3f;

	// Token: 0x0400415C RID: 16732
	[Tooltip("Moved objects reset time in-game hours.")]
	public float objectPositionResetTime = 4f;

	// Token: 0x0400415D RID: 16733
	[Tooltip("Broken windows will become boarded up after this time")]
	public float brokenWindowBoardTime = 2.5f;

	// Token: 0x0400415E RID: 16734
	[Tooltip("Broken windows will reset after this time")]
	public float brokenWindowResetTime = 24f;

	// Token: 0x0400415F RID: 16735
	[Header("Crime/Cases")]
	[Tooltip("Fine amount for breaking windows")]
	public int breakingWindowsFine = 500;

	// Token: 0x04004160 RID: 16736
	[Tooltip("Vandalism fine multiplier")]
	public int vandalismFineMultiplier = 4;

	// Token: 0x04004161 RID: 16737
	[Tooltip("How long it takes to cancel out address vandalism")]
	public float vandalismTimeout = 24f;

	// Token: 0x04004162 RID: 16738
	[Tooltip("Minimum amount of time for illegal actions to be present (seconds)")]
	public float illegalActionMinimumTime = 1f;

	// Token: 0x04004163 RID: 16739
	[Tooltip("How many items can be tampered with before it is considered a crime")]
	public int tamperGrace = 3;

	// Token: 0x04004164 RID: 16740
	[Tooltip("How far a physics object can be moved before it is considered a crime")]
	public float physicsTamperDistance = 1f;

	// Token: 0x04004165 RID: 16741
	[Tooltip("Dynamic fingerprints stay in the world for this many in-game hours.")]
	public float fingerprintLife = 24f;

	// Token: 0x04004166 RID: 16742
	[Tooltip("Maximum number of dynamic prints on an object at any one time.")]
	public int maxDynamicPrintsPerObject = 8;

	// Token: 0x04004167 RID: 16743
	public InteractablePreset fignerprintPreset;

	// Token: 0x04004168 RID: 16744
	[Tooltip("Time until suspects are detained after a call-in (gametime)")]
	public float detainDelay = 0.25f;

	// Token: 0x04004169 RID: 16745
	[Tooltip("Time until results of the case are processed after detaining (gametime)")]
	public float caseResultProcessTime = 1f;

	// Token: 0x0400416A RID: 16746
	[Tooltip("The number of murder victims needed to get the top rank")]
	public int bestCaseVictimCount = 1;

	// Token: 0x0400416B RID: 16747
	[Tooltip("The number of murder victims needed to get the worst rank")]
	public int worstCaseVictimCount = 4;

	// Token: 0x0400416C RID: 16748
	[Tooltip("Multiplier for job difficulty")]
	public AnimationCurve sideJobDifficultyRewardMultiplier;

	// Token: 0x0400416D RID: 16749
	[Tooltip("Used for side jobs; leave item at this secret location")]
	public List<FurniturePreset> secretLocationFurniture = new List<FurniturePreset>();

	// Token: 0x0400416E RID: 16750
	[Tooltip("Chance of triggering combat for stealing items from citizen's inventory")]
	public float stealTriggerChance = 0.5f;

	// Token: 0x0400416F RID: 16751
	[Tooltip("The max number of side jobs/custom cases")]
	public int maxCases = 5;

	// Token: 0x04004170 RID: 16752
	[Tooltip("The between a crime scene being sweeped, and the cleanup time")]
	public float crimeSceneCleanupDelay = 5f;

	// Token: 0x04004171 RID: 16753
	[Tooltip("The minimum/maxinum distance a mission photo can be from the object")]
	public Vector2 missionPhotoMinMaxDistance = new Vector2(1f, 10f);

	// Token: 0x04004172 RID: 16754
	[Tooltip("Scoring curve between min/max distances for a mission photo")]
	public AnimationCurve missionPhotoDistanceScoreCurve;

	// Token: 0x04004173 RID: 16755
	[Tooltip("Enable player crime cover up mission opporunities")]
	public bool enableCoverUps = true;

	// Token: 0x04004174 RID: 16756
	[EnableIf("enableCoverUps")]
	[Tooltip("Cover ups will be available during and after case #")]
	public int coverUpAvailableDuringCase = 2;

	// Token: 0x04004175 RID: 16757
	[Tooltip("Once cover ups are available, what are the chances that you'll get one?")]
	[EnableIf("enableCoverUps")]
	public float coverUpChance = 0.5f;

	// Token: 0x04004176 RID: 16758
	[EnableIf("enableCoverUps")]
	public int coverUpReward = 1000;

	// Token: 0x04004177 RID: 16759
	[Tooltip("The maximum number of footprints per room")]
	[Header("Footprints")]
	public int maximumFootprintsPerRoom = 20;

	// Token: 0x04004178 RID: 16760
	[Tooltip("The maximum time a footprint remains active (with a dirt value of 1)")]
	public float maximumFootprintTime = 24f;

	// Token: 0x04004179 RID: 16761
	[Tooltip("Min/max size of footprints lerped shoe size")]
	public Vector2 footprintScaleRange = Vector2.one;

	// Token: 0x0400417A RID: 16762
	[Tooltip("Each step removes this level of dirt from the citizen")]
	public float stepDirtRemoval = -0.01f;

	// Token: 0x0400417B RID: 16763
	[Tooltip("Each step removes this level of blood from the citizen")]
	public float stepBloodRemoval = -0.01f;

	// Token: 0x0400417C RID: 16764
	[Tooltip("Each step outside adds this level of dirt + material specific values")]
	public float outdoorStepDirtAccumulation = 0.011f;

	// Token: 0x0400417D RID: 16765
	public InteractablePreset footprintPreset;

	// Token: 0x0400417E RID: 16766
	[Tooltip("How long do enforcers search a crime scene?")]
	[Header("Murders")]
	public float crimeSceneSearchLength = 0.18f;

	// Token: 0x0400417F RID: 16767
	[Tooltip("How long does a crime scene stay active after enforcers arrive?")]
	public float crimeSceneLength = 2f;

	// Token: 0x04004180 RID: 16768
	[Tooltip("Time for a dead body smell to extend one additional room (starts with 0)")]
	public float smellTime = 1f;

	// Token: 0x04004181 RID: 16769
	[Tooltip("Murder turn-in questions")]
	public List<Case.ResolveQuestion> murderResolveQuestions = new List<Case.ResolveQuestion>();

	// Token: 0x04004182 RID: 16770
	[Tooltip("Retirement turn-in questions")]
	public List<Case.ResolveQuestion> retirementResolveQuestions = new List<Case.ResolveQuestion>();

	// Token: 0x04004183 RID: 16771
	[Header("Kidnappings")]
	public DialogPreset kidnapperCallTriggerDialog;

	// Token: 0x04004184 RID: 16772
	[Header("Computers")]
	[Tooltip("The cursor object to load")]
	public GameObject OScursor;

	// Token: 0x04004185 RID: 16773
	[Tooltip("Cursor load sprite")]
	public Sprite loadCursor;

	// Token: 0x04004186 RID: 16774
	[Header("Surveillance")]
	[InfoBox("Don't lower this past 85 or it will screw with the introduction set up process", 0)]
	[Tooltip("Surveillance camera capture FoV")]
	[Range(85f, 180f)]
	public float captureFoV = 180f;

	// Token: 0x04004187 RID: 16775
	[Tooltip("Surveillance camera capture range (nodes/visuals)")]
	public float captureRange = 18f;

	// Token: 0x04004188 RID: 16776
	[Tooltip("Surveillance camera capture range (humans)")]
	public float humanCaptureRange = 12.5f;

	// Token: 0x04004189 RID: 16777
	[Tooltip("Gap between capturing for cameras (gametime 0.1667 = 10 mins)")]
	public float captureInterval = 0.1667f;

	// Token: 0x0400418A RID: 16778
	[Tooltip("How many captures a camera can hold before overwriting (288 = 48 hours @ 10 mins)")]
	public int cameraCaptureMemory = 288;

	// Token: 0x0400418B RID: 16779
	[Tooltip("How old a camera capture can be before overwriting")]
	public float cameraCaptureMaxTime = 48f;

	// Token: 0x0400418C RID: 16780
	[Tooltip("Maximum camera captures per gameworld cycle")]
	public int maxCapturesPerFrame = 24;

	// Token: 0x0400418D RID: 16781
	[ReorderableList]
	[Header("Upgrades")]
	public List<GameplayControls.SyncDiskColour> syncDiskColours = new List<GameplayControls.SyncDiskColour>();

	// Token: 0x0400418E RID: 16782
	public int defaultDiskSlots = 3;

	// Token: 0x0400418F RID: 16783
	[Tooltip("Scrolling sensitivity of the mouse wheel when not zooming")]
	[Header("General")]
	public int mouseWheelEvidenceScrollSensitivity = 22;

	// Token: 0x04004190 RID: 16784
	[Tooltip("Indoor temperature (-2.5 - 2.5)")]
	public float indoorTemperature = 1.5f;

	// Token: 0x04004191 RID: 16785
	[Tooltip("Air duct temperature")]
	public float airDuctTemperature = -1.5f;

	// Token: 0x04004192 RID: 16786
	[Tooltip("Heat source temperature")]
	public float heatSourceTemperature = 10f;

	// Token: 0x04004193 RID: 16787
	[Tooltip("Oscillators")]
	public AnimationCurve oscillatorX;

	// Token: 0x04004194 RID: 16788
	public AnimationCurve oscillatorY;

	// Token: 0x04004195 RID: 16789
	public Vector2 drunkOscillationSpeed = new Vector2(0.2f, 0.5f);

	// Token: 0x04004196 RID: 16790
	[Tooltip("Shiver fluctuation")]
	public AnimationCurve shiverFluctuation;

	// Token: 0x04004197 RID: 16791
	public Vector2 shiverOscillationSpeed = new Vector2(1f, 1.2f);

	// Token: 0x04004198 RID: 16792
	[Tooltip("Drunk Lens Distort")]
	public AnimationCurve drunkLensDistortOscillator;

	// Token: 0x04004199 RID: 16793
	public Vector2 drunkLensDistortSpeed = new Vector2(1f, 1.2f);

	// Token: 0x0400419A RID: 16794
	public PlayerTransitionPreset tripTransition;

	// Token: 0x0400419B RID: 16795
	[Tooltip("Headache fluctuation")]
	public AnimationCurve headacheFluctuation;

	// Token: 0x0400419C RID: 16796
	public SpatterPatternPreset bleedingSpatter;

	// Token: 0x0400419D RID: 16797
	public float fallDamageMultiplier = 0.1f;

	// Token: 0x0400419E RID: 16798
	public StatusPreset detainedStatus;

	// Token: 0x0400419F RID: 16799
	public StatusPreset wantedInBuildingStatus;

	// Token: 0x040041A0 RID: 16800
	[Space(7f)]
	public float playerHungerRate = 0.125f;

	// Token: 0x040041A1 RID: 16801
	public float playerThirstRate = 0.2f;

	// Token: 0x040041A2 RID: 16802
	public float playerTirednessRate = 0.12f;

	// Token: 0x040041A3 RID: 16803
	public float playerEnergyRate = 0.058f;

	// Token: 0x040041A4 RID: 16804
	[Space(7f)]
	public float combatHitChanceOfBruised = 0.1f;

	// Token: 0x040041A5 RID: 16805
	public float combatHitChanceOfBlackEye = 0.06f;

	// Token: 0x040041A6 RID: 16806
	public float combatHitChanceOfBrokenLeg = 0.01f;

	// Token: 0x040041A7 RID: 16807
	public float combatHitChanceOfBleeding = 0.1f;

	// Token: 0x040041A8 RID: 16808
	[Header("Pricing")]
	public Vector2 propertyValueRange = new Vector2(800f, 100000f);

	// Token: 0x040041A9 RID: 16809
	public AnimationCurve propertyValueCurve;

	// Token: 0x040041AA RID: 16810
	[Tooltip("How much the player gets now")]
	[Header("Loan Sharks")]
	public int defaultLoanAmount = 2000;

	// Token: 0x040041AB RID: 16811
	[Tooltip("How much extra the player pays in full")]
	public int defaultLoanExtra = 250;

	// Token: 0x040041AC RID: 16812
	[Tooltip("The daily repayment per day")]
	public int defaultLoanRepayment = 250;

	// Token: 0x040041AD RID: 16813
	[Header("Loitering")]
	[Tooltip("How long in secords before the AI starts commenting on your loitering behaviour")]
	public float loiteringCommentThreshold = 10f;

	// Token: 0x040041AE RID: 16814
	[Tooltip("How long in secords before the player is approached by staff")]
	public float loiteringConfrontThreshold = 30f;

	// Token: 0x040041AF RID: 16815
	[Tooltip("How long in secords before the player is classed as trespassing")]
	public float loiteringTrespassThreshold = 120f;

	// Token: 0x040041B0 RID: 16816
	[Tooltip("The timer resets to this after a purchase")]
	public float loiteringPurchaseResetValue = -0.02f;

	// Token: 0x040041B1 RID: 16817
	[Header("Scope Bases")]
	public DDSScope humanScope;

	// Token: 0x040041B2 RID: 16818
	public DDSScope itemScope;

	// Token: 0x040041B3 RID: 16819
	public DDSScope murderScope;

	// Token: 0x040041B4 RID: 16820
	public DDSScope locationScope;

	// Token: 0x040041B5 RID: 16821
	public DDSScope evidenceScope;

	// Token: 0x040041B6 RID: 16822
	public DDSScope sideJobScope;

	// Token: 0x040041B7 RID: 16823
	public DDSScope syncDiskScope;

	// Token: 0x040041B8 RID: 16824
	public DDSScope groupScope;

	// Token: 0x040041B9 RID: 16825
	private static GameplayControls _instance;

	// Token: 0x020007FA RID: 2042
	[Serializable]
	public class SyncDiskColour
	{
		// Token: 0x040041BA RID: 16826
		public SyncDiskPreset.Manufacturer category;

		// Token: 0x040041BB RID: 16827
		public Color mainColour = Color.white;

		// Token: 0x040041BC RID: 16828
		public Color colour1 = Color.white;

		// Token: 0x040041BD RID: 16829
		public Color colour2 = Color.white;

		// Token: 0x040041BE RID: 16830
		public Color colour3 = Color.white;
	}
}
