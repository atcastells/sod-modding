using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007B8 RID: 1976
[CreateAssetMenu(fileName = "roomconfig_data", menuName = "Database/Decor/Room Configuration")]
public class RoomConfiguration : SoCustomComparison
{
	// Token: 0x060025C4 RID: 9668 RVA: 0x001E8113 File Offset: 0x001E6313
	[Button(null, 0)]
	public void CopyWallFrontage()
	{
		if (this.debugRoom != null)
		{
			this.wallFrontage = new List<RoomConfiguration.WallFrontage>(this.debugRoom.wallFrontage);
		}
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x001E8139 File Offset: 0x001E6339
	[Button(null, 0)]
	public void AddWallFrontage()
	{
		if (this.debugRoom != null)
		{
			this.wallFrontage.AddRange(this.debugRoom.wallFrontage);
		}
	}

	// Token: 0x04003C47 RID: 15431
	[Header("Type")]
	[Tooltip("The room type: Dictates layout parameters")]
	public RoomTypePreset roomType;

	// Token: 0x04003C48 RID: 15432
	[Tooltip("The room class: Dictates what decor and furniture this has")]
	public RoomClassPreset roomClass;

	// Token: 0x04003C49 RID: 15433
	[Header("Integration Rules")]
	[Tooltip("If there's not enough room, section off this room to include its vital elements")]
	public bool canBeOpenPlan;

	// Token: 0x04003C4A RID: 15434
	[EnableIf("canBeOpenPlan")]
	public RoomTypePreset openPlanRoom;

	// Token: 0x04003C4B RID: 15435
	[Header("Doors")]
	[Tooltip("Allow security doors to spawn on exits for this room?")]
	public RoomConfiguration.SecurityDoorRule securityDoors;

	// Token: 0x04003C4C RID: 15436
	[Header("Special Rules")]
	[Tooltip("Limit security camera")]
	public bool limitSecurityCameras;

	// Token: 0x04003C4D RID: 15437
	[EnableIf("limitSecurityCameras")]
	[Range(0f, 5f)]
	public int securityCameraLimit = 1;

	// Token: 0x04003C4E RID: 15438
	[Header("Lighting")]
	[Tooltip("Use main lights")]
	public bool useMainLights = true;

	// Token: 0x04003C4F RID: 15439
	[Tooltip("If set to false then this room will use timer lights")]
	public bool useLightSwitches = true;

	// Token: 0x04003C50 RID: 15440
	[Tooltip("At the start of the game, are the main lights on or off?")]
	public bool lightsOnAtStart = true;

	// Token: 0x04003C51 RID: 15441
	[Tooltip("If true, boost the amount of light from main lights in this room")]
	public bool wellLit;

	// Token: 0x04003C52 RID: 15442
	[Tooltip("If true, the game will automatically disable lights on floors that are 2 or more floors away from the player, or out of their vicinity.")]
	public bool autoDisableLightsOutOfVicinity = true;

	// Token: 0x04003C53 RID: 15443
	[EnableIf("autoDisableLightsOutOfVicinity")]
	[Tooltip("If true, the game will automatically disable lights on floors that are 2 or more floors away from the player, or out of their vicinity (but not if a stairwell)")]
	public bool onlyAutoDisableInNonStairwell;

	// Token: 0x04003C54 RID: 15444
	[Tooltip("If true use an area light per zone in addition to normal lights")]
	public bool useAdditionalAreaLights;

	// Token: 0x04003C55 RID: 15445
	[Tooltip("If true, use district settings as a base for colour and brightness settings")]
	public bool useDistrictSettingsAsBase;

	// Token: 0x04003C56 RID: 15446
	[EnableIf("useAdditionalAreaLights")]
	public int minimumLightZoneSizeForAreaLights = 4;

	// Token: 0x04003C57 RID: 15447
	[EnableIf("useAdditionalAreaLights")]
	public Vector3 areaLightOffset;

	// Token: 0x04003C58 RID: 15448
	[EnableIf("useAdditionalAreaLights")]
	public float areaLightBrightness = 1000f;

	// Token: 0x04003C59 RID: 15449
	[EnableIf("useAdditionalAreaLights")]
	public Color areaLightColor = Color.white;

	// Token: 0x04003C5A RID: 15450
	[EnableIf("useAdditionalAreaLights")]
	public float areaLightRange = 10f;

	// Token: 0x04003C5B RID: 15451
	[Tooltip("Multiply the area light size by this")]
	[EnableIf("useAdditionalAreaLights")]
	public float areaLightCoverageMultiplier = 1f;

	// Token: 0x04003C5C RID: 15452
	[Tooltip("If true, boost the ceiling emission by this colour when main lights are on")]
	public bool boostCeilingEmission;

	// Token: 0x04003C5D RID: 15453
	[Tooltip("If true, boost the ceiling emission by this colour when main lights are on")]
	public Color ceilingEmissionBoost = Color.black;

	// Token: 0x04003C5E RID: 15454
	[Tooltip("Chance of having a ceiling fan on light fittings")]
	[Range(0f, 1f)]
	public float chanceOfCeilingFans;

	// Token: 0x04003C5F RID: 15455
	[Tooltip("Give the base lighting a shadow tint?")]
	public bool baseLightingShadowTint = true;

	// Token: 0x04003C60 RID: 15456
	[Tooltip("Fake caustics by lerping the shadow tint to decor and time of day colours...")]
	[Range(0f, 1f)]
	public float baseLightingShadowTintIntensity = 0.05f;

	// Token: 0x04003C61 RID: 15457
	[Tooltip("Give the base lighting a shadow tint?")]
	[EnableIf("useAdditionalAreaLights")]
	public bool areaLightingShadowTint = true;

	// Token: 0x04003C62 RID: 15458
	[Tooltip("Fake caustics by lerping the shadow tint to decor and time of day colours...")]
	[Range(0f, 1f)]
	[EnableIf("areaLightingShadowTint")]
	public float areaLightingShadowTintIntensity = 0.3f;

	// Token: 0x04003C63 RID: 15459
	[EnableIf("areaLightingShadowTint")]
	public bool overrideAreaLightShadowTint;

	// Token: 0x04003C64 RID: 15460
	[EnableIf("areaLightingShadowTint")]
	public Color areaLightShadowTintOverride;

	// Token: 0x04003C65 RID: 15461
	[EnableIf("useAdditionalAreaLights")]
	[Range(0f, 1f)]
	public float areaLightShadowDimmer = 1f;

	// Token: 0x04003C66 RID: 15462
	[InfoBox("These settings can be overriden by AI actions and goals", 0)]
	[Header("Lighting AI Behaviour")]
	public List<RoomConfiguration.AILightingBehaviour> lightingBehaviour = new List<RoomConfiguration.AILightingBehaviour>();

	// Token: 0x04003C67 RID: 15463
	[Header("Colour Scheme")]
	[Tooltip("Used when picking a colour scheme for this: How clean/corporate/soulless is this room?")]
	[Range(0f, 10f)]
	public int cleanness = 1;

	// Token: 0x04003C68 RID: 15464
	[Tooltip("Force a selection of these colour schemes...")]
	public List<ColourSchemePreset> forceColourSchemes = new List<ColourSchemePreset>();

	// Token: 0x04003C69 RID: 15465
	[Tooltip("Minimum level of grubiness this room can have....")]
	[Range(0f, 1f)]
	public float minimumGrubiness;

	// Token: 0x04003C6A RID: 15466
	[Tooltip("Maximum level of grubiness this room can have....")]
	[Range(0f, 1f)]
	public float maximumGrubiness = 1f;

	// Token: 0x04003C6B RID: 15467
	public RoomConfiguration.DecorSetting decorSetting;

	// Token: 0x04003C6C RID: 15468
	[Tooltip("If true other adjacent rooms with the 'borrow from adjacent' setting won't copy this style.")]
	public bool excludeFromOthersCopyingDecorStyle;

	// Token: 0x04003C6D RID: 15469
	[Tooltip("Use an override material if this is on the ground floor (picked from this list, saved in building class.)")]
	public float chanceOfOverrideMatIfGroundFloor;

	// Token: 0x04003C6E RID: 15470
	[Tooltip("Use an override material if this is in the basement (picked from this list, saved in building class.)")]
	public float chanceOfOverrideMatIfBasement;

	// Token: 0x04003C6F RID: 15471
	[Tooltip("Use an override material if this room contains stairs (picked from this list, saved in building class.)")]
	public float chanceOfOverrideMatIfStairwell;

	// Token: 0x04003C70 RID: 15472
	[ReorderableList]
	[Tooltip("List of override materials")]
	public List<MaterialGroupPreset> floorOverrides = new List<MaterialGroupPreset>();

	// Token: 0x04003C71 RID: 15473
	[ReorderableList]
	public List<MaterialGroupPreset> wallOverrides = new List<MaterialGroupPreset>();

	// Token: 0x04003C72 RID: 15474
	[ReorderableList]
	public List<MaterialGroupPreset> ceilingOverrides = new List<MaterialGroupPreset>();

	// Token: 0x04003C73 RID: 15475
	[Space(7f)]
	[Tooltip("The priority given to decorating: Higher priority rooms will override size variables of others.")]
	[Range(0f, 10f)]
	public int decorationPriority = 5;

	// Token: 0x04003C74 RID: 15476
	[Header("Ownership")]
	[Tooltip("Can this room be owned by anyone?")]
	public bool useOwnership;

	// Token: 0x04003C75 RID: 15477
	[Tooltip("Assign owners to this furniture")]
	[EnableIf("useOwnership")]
	public int assignBelongsToOwners;

	// Token: 0x04003C76 RID: 15478
	[Tooltip("If this is checked the game will assign this object to a couple")]
	[EnableIf("useOwnership")]
	public bool preferCouples;

	// Token: 0x04003C77 RID: 15479
	[ReorderableList]
	[EnableIf("useOwnership")]
	[Tooltip("If this isn't null, the game will use a job to assign ownership to this room")]
	public List<OccupationPreset> belongsToJob;

	// Token: 0x04003C78 RID: 15480
	[Header("Doors")]
	[Tooltip("If this features a door to the outside, use this preset")]
	public DoorPreset exteriorDoor;

	// Token: 0x04003C79 RID: 15481
	[Tooltip("If this features a door to outside this address, use this preset")]
	public DoorPreset addressDoor;

	// Token: 0x04003C7A RID: 15482
	[Tooltip("If this features a door to another room in this address, use this preset")]
	public DoorPreset internalDoor;

	// Token: 0x04003C7B RID: 15483
	[Tooltip("Which room should be the passworded room, ie the place to store the key in?")]
	[Range(0f, 10f)]
	public int passwordPriority = 1;

	// Token: 0x04003C7C RID: 15484
	[Tooltip("For doors belonging to this room, prefer the password from...")]
	public RoomConfiguration.RoomPasswordPreference preferredPassword = RoomConfiguration.RoomPasswordPreference.thisRoom;

	// Token: 0x04003C7D RID: 15485
	[Tooltip("If this spawns a door that requires a key, place it here...")]
	public List<RoomConfiguration.KeyPlacement> placeKey = new List<RoomConfiguration.KeyPlacement>();

	// Token: 0x04003C7E RID: 15486
	public InteractablePreset.OwnedPlacementRule keyOwnershipPlacement = InteractablePreset.OwnedPlacementRule.both;

	// Token: 0x04003C7F RID: 15487
	[Tooltip("Use these steps")]
	public GameObject steps;

	// Token: 0x04003C80 RID: 15488
	[Header("Custom Walls")]
	public DoorPairPreset replaceWindows;

	// Token: 0x04003C81 RID: 15489
	public DoorPairPreset replaceWalls;

	// Token: 0x04003C82 RID: 15490
	public DoorPairPreset replaceEntrance;

	// Token: 0x04003C83 RID: 15491
	[Tooltip("By default, only outside walls are replaced here. Check to replace inside walls...")]
	public bool replaceInsideAlso;

	// Token: 0x04003C84 RID: 15492
	[Tooltip("Only replace above if the other side is one of these rooms...")]
	public bool replaceOnlyIfOtherIs;

	// Token: 0x04003C85 RID: 15493
	[EnableIf("replaceOnlyIfOtherIs")]
	public List<RoomTypePreset> onlyReplaceIf = new List<RoomTypePreset>();

	// Token: 0x04003C86 RID: 15494
	[Tooltip("Force inclusion on the street light lighting layer.")]
	public bool forceStreetLightLayer;

	// Token: 0x04003C87 RID: 15495
	[Tooltip("Draw the current building model when in this room.")]
	public bool drawBuildingModel;

	// Token: 0x04003C88 RID: 15496
	[Header("Wall Frontage")]
	public List<RoomConfiguration.WallFrontage> wallFrontage = new List<RoomConfiguration.WallFrontage>();

	// Token: 0x04003C89 RID: 15497
	[Tooltip("Used for fake roofs for things like rooftop air vents. Only one wall frontage allowed per node")]
	public bool oneFrontagePerNode;

	// Token: 0x04003C8A RID: 15498
	[Header("Air Vents")]
	public int maximumVents = 1;

	// Token: 0x04003C8B RID: 15499
	[Range(0f, 10f)]
	public int chanceOfRoofVent;

	// Token: 0x04003C8C RID: 15500
	[Range(0f, 10f)]
	public int chanceOfWallVentUpper;

	// Token: 0x04003C8D RID: 15501
	[Range(0f, 10f)]
	public int chanceOfWallVentLower;

	// Token: 0x04003C8E RID: 15502
	[Tooltip("If true this room allows upper-wall level air ducts (below ceiling height)")]
	public bool allowUpperWallLevelDucts;

	// Token: 0x04003C8F RID: 15503
	[Tooltip("Only allow upper wall level ducts if floor height is 0")]
	[EnableIf("allowUpperWallLevelDucts")]
	public bool onlyAllowUpperIfFloorLevelIsZero = true;

	// Token: 0x04003C90 RID: 15504
	[Tooltip("Limit the number of upper level ducts")]
	public int limitUpperLevelDucts = 99;

	// Token: 0x04003C91 RID: 15505
	[Tooltip("If true this room allows lower-wall level air ducts (below standing height)")]
	public bool allowLowerWallLevelDucts;

	// Token: 0x04003C92 RID: 15506
	[Tooltip("Use a specific profile for this room")]
	[Header("Environment")]
	public bool overrideAddressEnvironment;

	// Token: 0x04003C93 RID: 15507
	[EnableIf("overrideAddressEnvironment")]
	public SessionData.SceneProfile sceneClean = SessionData.SceneProfile.indoors;

	// Token: 0x04003C94 RID: 15508
	[EnableIf("overrideAddressEnvironment")]
	public SessionData.SceneProfile sceneDirty = SessionData.SceneProfile.grimey;

	// Token: 0x04003C95 RID: 15509
	[Tooltip("Affects lighting volumetrics; creating a smokey atmosphere with a higher value.")]
	[Range(0f, 1f)]
	public float baseRoomAtmosphere = 0.5f;

	// Token: 0x04003C96 RID: 15510
	[Tooltip("Force the nodes in this room to register as outside or inside...")]
	public RoomConfiguration.OutsideSetting forceOutside;

	// Token: 0x04003C97 RID: 15511
	[Header("Audio")]
	public AmbientZone ambientZone;

	// Token: 0x04003C98 RID: 15512
	[Header("Fingerprints")]
	[Tooltip("Should there be fingerprints here?")]
	public bool fingerprintsEnabled = true;

	// Token: 0x04003C99 RID: 15513
	[Tooltip("Should there be footprints here?")]
	public bool footprintsEnabled = true;

	// Token: 0x04003C9A RID: 15514
	[Tooltip("The source of the prints")]
	public RoomConfiguration.PrintsSource printsSource = RoomConfiguration.PrintsSource.inhabitants;

	// Token: 0x04003C9B RID: 15515
	[Range(0f, 2f)]
	[Tooltip("Fingerprint density on walls")]
	public float fingerprintWallDensity = 1f;

	// Token: 0x04003C9C RID: 15516
	[Header("Other")]
	public bool allowCoving;

	// Token: 0x04003C9D RID: 15517
	[Tooltip("Allow bugs to be spawned in this room")]
	public bool allowBugs;

	// Token: 0x04003C9E RID: 15518
	[Tooltip("Number of bugs = number of nodes * grubiness * this")]
	[EnableIf("allowBugs")]
	public float bugAmountMultiplier = 0.75f;

	// Token: 0x04003C9F RID: 15519
	[Tooltip("If true the player will be tresspassing when here")]
	public RoomConfiguration.Forbidden forbidden;

	// Token: 0x04003CA0 RID: 15520
	[Tooltip("The player is allowed here after they have given a correct password (if set on the address preset)")]
	public bool allowedIfGivenCorrectPassword = true;

	// Token: 0x04003CA1 RID: 15521
	[Tooltip("Allow AI here if the password setting is on in the address preset")]
	public bool AIknowPassword = true;

	// Token: 0x04003CA2 RID: 15522
	[Tooltip("Severity for being caught when this is forbidden (0 = asked to leave, 2 = combat on sight)")]
	[Range(0f, 2f)]
	public int escalationLevelNormal;

	// Token: 0x04003CA3 RID: 15523
	[Tooltip("Severity for being caught when this is forbidden when after hours (0 = asked to leave, 2 = combat on sight)")]
	[Range(0f, 2f)]
	public int escalationLevelAfterHours;

	// Token: 0x04003CA4 RID: 15524
	[Range(0f, 4f)]
	[Tooltip("All object placements in this room have this increased security level")]
	public int securityLevel;

	// Token: 0x04003CA5 RID: 15525
	[Tooltip("If true, personal affects of citizens can be placed in this room")]
	public bool allowPersonalAffects = true;

	// Token: 0x04003CA6 RID: 15526
	public bool overrideMaxFurnitureClusters;

	// Token: 0x04003CA7 RID: 15527
	[EnableIf("overrideMaxFurnitureClusters")]
	public int overridenMaxFurniture = 42;

	// Token: 0x04003CA8 RID: 15528
	public bool overrideAttemptsPerNodeMultiplier;

	// Token: 0x04003CA9 RID: 15529
	[EnableIf("overrideAttemptsPerNodeMultiplier")]
	public float overridenAttemptsPerNode = 1.75f;

	// Token: 0x04003CAA RID: 15530
	[Tooltip("When ranking shadiness for certain jobs/systems the base value")]
	[Range(0f, 10f)]
	public int shadinessValue;

	// Token: 0x04003CAB RID: 15531
	[Tooltip("AI can mug here")]
	public bool allowMuggings;

	// Token: 0x04003CAC RID: 15532
	[Tooltip("Player can awaken here after mugging")]
	public bool muggingAwakenRoom;

	// Token: 0x04003CAD RID: 15533
	[Header("Debug")]
	public RoomConfiguration debugRoom;

	// Token: 0x020007B9 RID: 1977
	public enum DecorSetting
	{
		// Token: 0x04003CAF RID: 15535
		ownStyle,
		// Token: 0x04003CB0 RID: 15536
		borrowFromAdjoining,
		// Token: 0x04003CB1 RID: 15537
		borrowFromBuilding,
		// Token: 0x04003CB2 RID: 15538
		borrowFromBelow
	}

	// Token: 0x020007BA RID: 1978
	public enum RoomZoning
	{
		// Token: 0x04003CB4 RID: 15540
		lobby,
		// Token: 0x04003CB5 RID: 15541
		residential,
		// Token: 0x04003CB6 RID: 15542
		commerical,
		// Token: 0x04003CB7 RID: 15543
		industrial,
		// Token: 0x04003CB8 RID: 15544
		municpial,
		// Token: 0x04003CB9 RID: 15545
		park
	}

	// Token: 0x020007BB RID: 1979
	public enum Forbidden
	{
		// Token: 0x04003CBB RID: 15547
		alwaysAllowed,
		// Token: 0x04003CBC RID: 15548
		alwaysForbidden,
		// Token: 0x04003CBD RID: 15549
		allowedDuringOpenHours
	}

	// Token: 0x020007BC RID: 1980
	public enum SecurityDoorRule
	{
		// Token: 0x04003CBF RID: 15551
		never,
		// Token: 0x04003CC0 RID: 15552
		allAdjoining,
		// Token: 0x04003CC1 RID: 15553
		onlyToOtherAddress,
		// Token: 0x04003CC2 RID: 15554
		onlyToStairwell
	}

	// Token: 0x020007BD RID: 1981
	[Serializable]
	public class AILightingBehaviour
	{
		// Token: 0x04003CC3 RID: 15555
		public RoomConfiguration.AILightingBehaviour.TimeOfDay dayRule;

		// Token: 0x04003CC4 RID: 15556
		public RoomConfiguration.AILightingBehaviour.LightingPreference passthroughBehaviour = RoomConfiguration.AILightingBehaviour.LightingPreference.eitherPriorityMain;

		// Token: 0x04003CC5 RID: 15557
		public RoomConfiguration.AILightingBehaviour.LightingPreference destinationBehaviour;

		// Token: 0x04003CC6 RID: 15558
		public RoomConfiguration.AILightingBehaviour.LightingPreference exitRoomBehaviour = RoomConfiguration.AILightingBehaviour.LightingPreference.none;

		// Token: 0x04003CC7 RID: 15559
		public RoomConfiguration.AILightingBehaviour.LightingPreference exitGameLocationBehaviour = RoomConfiguration.AILightingBehaviour.LightingPreference.allOff;

		// Token: 0x020007BE RID: 1982
		public enum TimeOfDay
		{
			// Token: 0x04003CC9 RID: 15561
			always,
			// Token: 0x04003CCA RID: 15562
			daytime,
			// Token: 0x04003CCB RID: 15563
			evening
		}

		// Token: 0x020007BF RID: 1983
		public enum LightingPreference
		{
			// Token: 0x04003CCD RID: 15565
			mainOn,
			// Token: 0x04003CCE RID: 15566
			secondaryOn,
			// Token: 0x04003CCF RID: 15567
			eitherPriorityMain,
			// Token: 0x04003CD0 RID: 15568
			eitherPrioritySecondary,
			// Token: 0x04003CD1 RID: 15569
			allOff,
			// Token: 0x04003CD2 RID: 15570
			mainOff,
			// Token: 0x04003CD3 RID: 15571
			secondaryOff,
			// Token: 0x04003CD4 RID: 15572
			none,
			// Token: 0x04003CD5 RID: 15573
			mainOnSecondaryAny
		}
	}

	// Token: 0x020007C0 RID: 1984
	public enum RoomPasswordPreference
	{
		// Token: 0x04003CD7 RID: 15575
		interactableBelongsTo,
		// Token: 0x04003CD8 RID: 15576
		thisRoom,
		// Token: 0x04003CD9 RID: 15577
		thisAddress
	}

	// Token: 0x020007C1 RID: 1985
	public enum KeyPlacement
	{
		// Token: 0x04003CDB RID: 15579
		thisAddress,
		// Token: 0x04003CDC RID: 15580
		belongsToHome,
		// Token: 0x04003CDD RID: 15581
		belongsToWork
	}

	// Token: 0x020007C2 RID: 1986
	[Serializable]
	public class WallFrontage
	{
		// Token: 0x04003CDE RID: 15582
		public string name;

		// Token: 0x04003CDF RID: 15583
		public DoorPairPreset wallPreset;

		// Token: 0x04003CE0 RID: 15584
		public List<WallFrontageClass> insideFrontage;

		// Token: 0x04003CE1 RID: 15585
		public List<WallFrontageClass> outsideFrontage;

		// Token: 0x04003CE2 RID: 15586
		[Tooltip("This entry is only valid if the wall faces onto the outside")]
		public bool onlyIfBorderingOutside;

		// Token: 0x04003CE3 RID: 15587
		public Vector3 localOffset = Vector3.zero;

		// Token: 0x04003CE4 RID: 15588
		public bool limitToBuildingTypes;

		// Token: 0x04003CE5 RID: 15589
		public List<BuildingPreset> limitedToBuildings;
	}

	// Token: 0x020007C3 RID: 1987
	public enum OutsideSetting
	{
		// Token: 0x04003CE7 RID: 15591
		dontChange,
		// Token: 0x04003CE8 RID: 15592
		forceOutside,
		// Token: 0x04003CE9 RID: 15593
		forceInside
	}

	// Token: 0x020007C4 RID: 1988
	public enum PrintsSource
	{
		// Token: 0x04003CEB RID: 15595
		owners,
		// Token: 0x04003CEC RID: 15596
		inhabitants,
		// Token: 0x04003CED RID: 15597
		buildingResidents,
		// Token: 0x04003CEE RID: 15598
		customersAll,
		// Token: 0x04003CEF RID: 15599
		customersMale,
		// Token: 0x04003CF0 RID: 15600
		customersFemale,
		// Token: 0x04003CF1 RID: 15601
		publicAll,
		// Token: 0x04003CF2 RID: 15602
		inhabitantsAndCustomers,
		// Token: 0x04003CF3 RID: 15603
		writers,
		// Token: 0x04003CF4 RID: 15604
		receivers,
		// Token: 0x04003CF5 RID: 15605
		ownersAndWriters,
		// Token: 0x04003CF6 RID: 15606
		ownersWritersReceivers,
		// Token: 0x04003CF7 RID: 15607
		killer
	}
}
