using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200074A RID: 1866
[CreateAssetMenu(fileName = "object_data", menuName = "Database/Interactable Preset")]
public class InteractablePreset : SoCustomComparison
{
	// Token: 0x0600256D RID: 9581 RVA: 0x001E674C File Offset: 0x001E494C
	public List<InteractablePreset.InteractionAction> GetActions(int lockedInPhase = 0)
	{
		List<InteractablePreset.InteractionAction> list = new List<InteractablePreset.InteractionAction>();
		foreach (InteractableActionsPreset interactableActionsPreset in this.actionsPreset)
		{
			if (interactableActionsPreset == null)
			{
				Game.Log(this.presetName + " has missing actions!", 2);
			}
			list.AddRange(interactableActionsPreset.actions);
			if (lockedInPhase == 3)
			{
				list.AddRange(interactableActionsPreset.physicsActions);
			}
			else
			{
				if (lockedInPhase >= 1)
				{
					list.AddRange(interactableActionsPreset.lockedInActions1);
				}
				if (lockedInPhase >= 2)
				{
					list.AddRange(interactableActionsPreset.lockedInActions2);
				}
			}
		}
		return list;
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x001E67FC File Offset: 0x001E49FC
	public PhysicsProfile GetPhysicsProfile()
	{
		if (this.physicsProfile != null)
		{
			return this.physicsProfile;
		}
		return GameplayControls.Instance.defaultObjectPhysicsProfile;
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CopyFPSHeldPostionFromTransform()
	{
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CalculateDroppedAngleHeightBoost()
	{
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x001E6820 File Offset: 0x001E4A20
	[Button(null, 0)]
	public void SpawnIntoInventory()
	{
		if (this.isInventoryItem)
		{
			if (!FirstPersonItemController.Instance.IsSlotAvailable())
			{
				PopupMessageController.Instance.PopupMessage("dropitem", true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				return;
			}
			Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(this, Player.Instance, Player.Instance, null, Player.Instance.transform.position, Player.Instance.transform.eulerAngles, null, null, "");
			if (interactable != null)
			{
				interactable.SetSpawnPositionRelevent(false);
				if (FirstPersonItemController.Instance.PickUpItem(interactable, false, false, true, true, true))
				{
					interactable.MarkAsTrash(true, false, 0f);
					return;
				}
				interactable.Delete();
			}
		}
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void SetToZeroValue()
	{
	}

	// Token: 0x040036B8 RID: 14008
	[Tooltip("If true this object can be spawned through the object creator")]
	[Header("Spawning")]
	public bool spawnable = true;

	// Token: 0x040036B9 RID: 14009
	[ShowIf("spawnable")]
	[Tooltip("You only need to set this if the item is spawnable.")]
	public GameObject prefab;

	// Token: 0x040036BA RID: 14010
	[Tooltip("This value is held as a workaround for not being able to access the prefab in multithreading")]
	[ShowIf("spawnable")]
	[ReadOnly]
	public Vector3 prefabLocalEuler = Vector3.zero;

	// Token: 0x040036BB RID: 14011
	[Tooltip("This value is held as a workaround for not being able to access the prefab in multithreading")]
	[ShowIf("spawnable")]
	[ReadOnly]
	public Vector3 prefabLocalScale = Vector3.one;

	// Token: 0x040036BC RID: 14012
	[Tooltip("Don't save with state data")]
	public bool dontSaveWithSaveGames;

	// Token: 0x040036BD RID: 14013
	[Tooltip("Override the above behaviour if this is classed a world object; useful for lightswitches if they have been placed by the player")]
	[ShowIf("dontSaveWithSaveGames")]
	public bool onlySaveWithSaveGamesIfWorldObject;

	// Token: 0x040036BE RID: 14014
	[Tooltip("Object pooling will not be used for this")]
	[ShowIf("spawnable")]
	public bool excludeFromObjectPooling;

	// Token: 0x040036BF RID: 14015
	[Tooltip("If true, the mesh renderers on this object won't get turned on and off with range or room visibility.")]
	[ShowIf("excludeFromObjectPooling")]
	public bool excludeFromVisibilityRangeChecks;

	// Token: 0x040036C0 RID: 14016
	[Tooltip("Load in at this range")]
	[ShowIf("spawnable")]
	[DisableIf("excludeFromVisibilityRangeChecks")]
	public ObjectPoolingController.ObjectLoadRange spawnRange = ObjectPoolingController.ObjectLoadRange.medium;

	// Token: 0x040036C1 RID: 14017
	[Header("Scene Capture")]
	[Tooltip("If true include in any scene capturing. If false the object will be hidden. Toggle for any static or integrated objects.")]
	public bool showWorldObjectInSceneCapture;

	// Token: 0x040036C2 RID: 14018
	[Tooltip("If true, capture and set the state of this object for captures")]
	[EnableIf("showWorldObjectInSceneCapture")]
	public bool captureStateInSceneCapture;

	// Token: 0x040036C3 RID: 14019
	[DisableIf("showWorldObjectInSceneCapture")]
	public bool createProxy;

	// Token: 0x040036C4 RID: 14020
	[ShowIf("createProxy")]
	public bool onlyCreateProxyInDetailedCapture = true;

	// Token: 0x040036C5 RID: 14021
	[ShowIf("createProxy")]
	public ObjectPoolingController.ObjectLoadRange createProxyAtRange;

	// Token: 0x040036C6 RID: 14022
	[Tooltip("If true the same material colours will be shared over all instances of this furniture for the room. Does not apply to integrated interactables which will be coloured by their parent furniture.")]
	[Header("Colour")]
	public bool inheritColouringFromDecor;

	// Token: 0x040036C7 RID: 14023
	[Tooltip("If true the same material colours will be shared over all instances of this furniture for the room. Difference from furniture: This cannot 'create' a material key, so furniture with it must already exist in the room.")]
	[ShowIf("inheritColouringFromDecor")]
	public FurniturePreset.ShareColours shareColoursWithFurniture;

	// Token: 0x040036C8 RID: 14024
	[HideIf("inheritColouringFromDecor")]
	[Tooltip("If this object needs custom colours...")]
	public bool useOwnColourSettings;

	// Token: 0x040036C9 RID: 14025
	[ShowIf("useOwnColourSettings")]
	public InteractablePreset.InteractableColourSetting mainColour;

	// Token: 0x040036CA RID: 14026
	[ShowIf("useOwnColourSettings")]
	public InteractablePreset.InteractableColourSetting customColour1;

	// Token: 0x040036CB RID: 14027
	[ShowIf("useOwnColourSettings")]
	public InteractablePreset.InteractableColourSetting customColour2;

	// Token: 0x040036CC RID: 14028
	[ShowIf("useOwnColourSettings")]
	public InteractablePreset.InteractableColourSetting customColour3;

	// Token: 0x040036CD RID: 14029
	[ShowIf("useOwnColourSettings")]
	public bool inheritGrubValue;

	// Token: 0x040036CE RID: 14030
	[Header("Setup")]
	[Tooltip("Attempt to name this using evidence entry or preset name, if false you must set this manually.")]
	public bool autoName = true;

	// Token: 0x040036CF RID: 14031
	[Tooltip("Include belongs to name in interactable name")]
	public bool includeBelongsTo;

	// Token: 0x040036D0 RID: 14032
	[ShowIf("includeBelongsTo")]
	[Tooltip("Use a shorthand version of the name (Initial + Surname)")]
	public bool useNameShorthand;

	// Token: 0x040036D1 RID: 14033
	[ShowIf("includeBelongsTo")]
	public bool useApartmentName;

	// Token: 0x040036D2 RID: 14034
	[Tooltip("Is this a light?")]
	public LightingPreset isLight;

	// Token: 0x040036D3 RID: 14035
	public InteractablePreset.Switch lightswitch;

	// Token: 0x040036D4 RID: 14036
	[Tooltip("If true, allows an unscrewed override state (cutsom switch 1)")]
	public bool allowUnscrewed = true;

	// Token: 0x040036D5 RID: 14037
	public bool isMainLight;

	// Token: 0x040036D6 RID: 14038
	[Tooltip("If true, this light is added to layer 1; the light layer for street lights")]
	public bool forceIncludeOnStreetLightLayer;

	// Token: 0x040036D7 RID: 14039
	[ShowAssetPreview(64, 64)]
	public Sprite staticImage;

	// Token: 0x040036D8 RID: 14040
	[ReadOnly]
	public Vector3 imagePos;

	// Token: 0x040036D9 RID: 14041
	[ReadOnly]
	public Vector3 imageRot;

	// Token: 0x040036DA RID: 14042
	[ReadOnly]
	public float imageScale = 1f;

	// Token: 0x040036DB RID: 14043
	[ReadOnly]
	public GameObject imagePrefabOverride;

	// Token: 0x040036DC RID: 14044
	[Tooltip("Weapon selection icon override")]
	[ShowAssetPreview(64, 64)]
	public Sprite iconOverride;

	// Token: 0x040036DD RID: 14045
	public InteractablePreset.ItemClass itemClass = InteractablePreset.ItemClass.misc;

	// Token: 0x040036DE RID: 14046
	public bool allowInApartmentStorage = true;

	// Token: 0x040036DF RID: 14047
	[EnableIf("allowInApartmentStorage")]
	public bool allowInApartmentShop;

	// Token: 0x040036E0 RID: 14048
	[Tooltip("If enabled, this item cannot be 'moved to storage'. Only needed for spawnable items")]
	[EnableIf("spawnable")]
	public bool disableMoveToStorage;

	// Token: 0x040036E1 RID: 14049
	[EnableIf("allowInApartmentStorage")]
	[Tooltip("The method of placement used when the player uses the apartment editor to place this")]
	public InteractablePreset.ApartmentPlacementMode apartmentPlacementMode;

	// Token: 0x040036E2 RID: 14050
	public List<FurniturePreset> mustTouchFurniture = new List<FurniturePreset>();

	// Token: 0x040036E3 RID: 14051
	[Space(7f)]
	public bool useMaterialOverride;

	// Token: 0x040036E4 RID: 14052
	[EnableIf("useMaterialOverride")]
	public AudioController.SoundMaterialOverride materialOverride;

	// Token: 0x040036E5 RID: 14053
	[Header("Interaction")]
	[Tooltip("Setup of actions able to be performed")]
	public List<InteractableActionsPreset> actionsPreset;

	// Token: 0x040036E6 RID: 14054
	[Tooltip("Illegal actions are only classed as illegal if the item is in a non-public space")]
	public bool onlyIllegalIfInNonPublic = true;

	// Token: 0x040036E7 RID: 14055
	[Tooltip("This modifier will be added to the interactable distance")]
	public float rangeModifier;

	// Token: 0x040036E8 RID: 14056
	[Header("Physics")]
	public PhysicsProfile physicsProfile;

	// Token: 0x040036E9 RID: 14057
	public bool overrideMass;

	// Token: 0x040036EA RID: 14058
	public bool forcePhysicsAlwaysOn;

	// Token: 0x040036EB RID: 14059
	[Tooltip("If true this object will react with doors, damage impacts etc")]
	public bool reactWithExternalStimuli;

	// Token: 0x040036EC RID: 14060
	[ShowIf("overrideMass")]
	public float mass = 1f;

	// Token: 0x040036ED RID: 14061
	public bool breakable;

	// Token: 0x040036EE RID: 14062
	[EnableIf("breakable")]
	public ParticleEffect particleProfile;

	// Token: 0x040036EF RID: 14063
	[EnableIf("breakable")]
	public bool overrideShatterSettings;

	// Token: 0x040036F0 RID: 14064
	[EnableIf("overrideShatterSettings")]
	[Tooltip("The size of the shards created")]
	public Vector3 shardSize = new Vector3(0.025f, 0.025f, 0.025f);

	// Token: 0x040036F1 RID: 14065
	[Tooltip("Create a shard every this amount of pixels on the texture")]
	[EnableIf("overrideShatterSettings")]
	public int shardEveryXPixels = 64;

	// Token: 0x040036F2 RID: 14066
	[EnableIf("breakable")]
	public bool overrideSpatterSettings;

	// Token: 0x040036F3 RID: 14067
	[EnableIf("overrideSpatterSettings")]
	public SpatterPatternPreset spatterSimulation;

	// Token: 0x040036F4 RID: 14068
	[EnableIf("overrideSpatterSettings")]
	public float spatterCountMultiplier = 1f;

	// Token: 0x040036F5 RID: 14069
	[Tooltip("Use this to override the hiding place camera settings in the furniture preset")]
	[Header("Hiding Place Override")]
	public bool overrideFurnitureSetting;

	// Token: 0x040036F6 RID: 14070
	[ShowIf("overrideFurnitureSetting")]
	public PlayerTransitionPreset enterTransition;

	// Token: 0x040036F7 RID: 14071
	[ShowIf("overrideFurnitureSetting")]
	public PlayerTransitionPreset exitTransition;

	// Token: 0x040036F8 RID: 14072
	[ShowIf("overrideFurnitureSetting")]
	public PlayerTransitionPreset enterTransition2;

	// Token: 0x040036F9 RID: 14073
	[ShowIf("overrideFurnitureSetting")]
	public PlayerTransitionPreset exitTransition2;

	// Token: 0x040036FA RID: 14074
	[Header("Trigger Sounds")]
	[Tooltip("Trigger audio on these switch events")]
	public List<InteractablePreset.IfSwitchStateSFX> switchSFX = new List<InteractablePreset.IfSwitchStateSFX>();

	// Token: 0x040036FB RID: 14075
	[Tooltip("Set the switch state to this on start")]
	[Header("Starting States")]
	public bool startingSwitchState;

	// Token: 0x040036FC RID: 14076
	[Tooltip("Set the switch state to this on start")]
	public bool startingCustomState1;

	// Token: 0x040036FD RID: 14077
	[Tooltip("Set the switch state to this on start")]
	public bool startingCustomState2;

	// Token: 0x040036FE RID: 14078
	[Tooltip("Set the switch state to this on start")]
	public bool startingCustomState3;

	// Token: 0x040036FF RID: 14079
	[Tooltip("Set the lock state to this on start")]
	public bool startingLockState;

	// Token: 0x04003700 RID: 14080
	[Header("Value")]
	[Tooltip("Monetary value of this object. Min/Max.")]
	[MinMaxSlider(0f, 10000f)]
	public Vector2 value = new Vector2(1f, 1f);

	// Token: 0x04003701 RID: 14081
	[Range(0f, 10f)]
	[Tooltip("AI will rank actions by this if there are multiple copies")]
	[Header("AI")]
	public int AIPriority = 5;

	// Token: 0x04003702 RID: 14082
	[Tooltip("Is this incompatible for social gatherings? Ie if I'm meeting someone here...")]
	public bool disableForSocialGroups;

	// Token: 0x04003703 RID: 14083
	[Tooltip("When chosing between interactables, how much to factor in the closest one?")]
	public float pickDistanceMultiplier = 1f;

	// Token: 0x04003704 RID: 14084
	[Tooltip("Use unique settings per action for each of the following")]
	public List<InteractablePreset.AIUsePriority> perActionPrioritySettings = new List<InteractablePreset.AIUsePriority>();

	// Token: 0x04003705 RID: 14085
	[Tooltip("Will the AI notice if this is moved?")]
	public bool tamperEnabled = true;

	// Token: 0x04003706 RID: 14086
	[Tooltip("Object reset behaviours based on activity and conditions")]
	[Space(7f)]
	public List<InteractablePreset.ObjectResetBehaviour> resetBehaviour = new List<InteractablePreset.ObjectResetBehaviour>();

	// Token: 0x04003707 RID: 14087
	[Space(7f)]
	[Tooltip("The AI will move to one of these postions to use this")]
	public InteractablePreset.AIUseSetting useSetting;

	// Token: 0x04003708 RID: 14088
	[Header("Reading")]
	[Tooltip("If within reading range then display text contained in this evidence")]
	public bool readingEnabled;

	// Token: 0x04003709 RID: 14089
	[ShowIf("readingEnabled")]
	[Tooltip("Reading mode is only active while switch status is true")]
	public bool readyingEnabledOnlyWithSwitchIsTue;

	// Token: 0x0400370A RID: 14090
	[ShowIf("readingEnabled")]
	[Tooltip("Reading mode is only active while switch status is true")]
	public bool readingEnabledOnlyWithKaizenSkill;

	// Token: 0x0400370B RID: 14091
	[Tooltip("Where to pull the text info from")]
	[ShowIf("readingEnabled")]
	public InteractablePreset.ReadingModeSource readingSource = InteractablePreset.ReadingModeSource.mainEvidenceText;

	// Token: 0x0400370C RID: 14092
	[Tooltip("Discover evidence upon read")]
	[ShowIf("readingEnabled")]
	public bool discoverOnRead;

	// Token: 0x0400370D RID: 14093
	[ShowIf("readingEnabled")]
	[Tooltip("A delay to reading when a page is turned")]
	public float pageTurnReadingDelay;

	// Token: 0x0400370E RID: 14094
	[Header("Distance Recognition")]
	[Tooltip("If within a certain range, then display a grey-ed out interaction icon with name text")]
	public bool distanceRecognitionEnabled;

	// Token: 0x0400370F RID: 14095
	public bool distanceRecognitionOnly;

	// Token: 0x04003710 RID: 14096
	public float recognitionRange = 5f;

	// Token: 0x04003711 RID: 14097
	[Header("Placement")]
	[Tooltip("Spawn this object using this sub object group")]
	public List<SubObjectClassPreset> subObjectClasses = new List<SubObjectClassPreset>();

	// Token: 0x04003712 RID: 14098
	[Tooltip("If the object fails to be placed in the above, use this class as a fall-back placement option. This is irrelevent for auto placement, as objects are spawned by the individual placements upon furniture, these places won't be considered.")]
	public List<SubObjectClassPreset> backupClasses = new List<SubObjectClassPreset>();

	// Token: 0x04003713 RID: 14099
	[Space(5f)]
	[Tooltip("Whether this will be automatically placed along with furniture...")]
	public InteractablePreset.AutoPlacement autoPlacement;

	// Token: 0x04003714 RID: 14100
	[Tooltip("If true, these objects will be placed with no owners at every gamelocation (based on other filters in this section).")]
	[Header("...Per Game Location")]
	public bool alwaysPlaceAtGameLocation;

	// Token: 0x04003715 RID: 14101
	[Range(0f, 20f)]
	[ShowIf("alwaysPlaceAtGameLocation")]
	[Tooltip("The minimum number of objects that will be auto-placed at every gamelocation")]
	public int frequencyPerGamelocationMin;

	// Token: 0x04003716 RID: 14102
	[Range(0f, 20f)]
	[ShowIf("alwaysPlaceAtGameLocation")]
	[Tooltip("The minimum number of objects that will be auto-placed at every gamelocation")]
	public int frequencyPerGameLocationMax;

	// Token: 0x04003717 RID: 14103
	[ShowIf("alwaysPlaceAtGameLocation")]
	[Tooltip("Dictates in what order objects should be placed in...")]
	[Range(0f, 10f)]
	public int perGameLocationObjectPriority;

	// Token: 0x04003718 RID: 14104
	[Tooltip("If true, owners/inhabitants/employees will be scanned for these traits and items will be placed accordingly...")]
	[Header("...Per Owner")]
	public bool placeIfFiltersPresentInOwner;

	// Token: 0x04003719 RID: 14105
	[Tooltip("Place if this is the citizen's home")]
	[ShowIf("placeIfFiltersPresentInOwner")]
	public bool placeAtHome = true;

	// Token: 0x0400371A RID: 14106
	[Tooltip("Place if this is the citizen's place of work")]
	[ShowIf("placeIfFiltersPresentInOwner")]
	public bool placeAtWork;

	// Token: 0x0400371B RID: 14107
	public List<InteractablePreset.TraitPick> traitModifiers = new List<InteractablePreset.TraitPick>();

	// Token: 0x0400371C RID: 14108
	[Tooltip("The minimum number of objects that will be auto-placed for each owner")]
	[Range(0f, 20f)]
	[ShowIf("placeIfFiltersPresentInOwner")]
	public int frequencyPerOwnerMin;

	// Token: 0x0400371D RID: 14109
	[Tooltip("The minimum number of objects that will be auto-placed for each owner")]
	[Range(0f, 20f)]
	[ShowIf("placeIfFiltersPresentInOwner")]
	public int frequencyPerOwnerMax;

	// Token: 0x0400371E RID: 14110
	[ShowIf("placeIfFiltersPresentInOwner")]
	[Tooltip("If true, the overall frequency range will be multiplied by the inverse of conscientiousness (untidy = more)")]
	public bool multiplyByMessiness;

	// Token: 0x0400371F RID: 14111
	[Tooltip("Dictates in what order objects should be placed in...")]
	[Range(0f, 10f)]
	[ShowIf("placeIfFiltersPresentInOwner")]
	public int perOwnerObjectPriority;

	// Token: 0x04003720 RID: 14112
	[ShowIf("placeIfFiltersPresentInOwner")]
	public EvidencePreset.BelongsToSetting writerIs;

	// Token: 0x04003721 RID: 14113
	[ShowIf("placeIfFiltersPresentInOwner")]
	public EvidencePreset.BelongsToSetting receiverIs;

	// Token: 0x04003722 RID: 14114
	[ShowIf("placeIfFiltersPresentInOwner")]
	[Tooltip("If the above two options are different, is this allowed to be from the same person to the same person?")]
	public bool canBeFromSelf;

	// Token: 0x04003723 RID: 14115
	[Header("Placement Limits")]
	public bool limitPerObject;

	// Token: 0x04003724 RID: 14116
	[Tooltip("How many of these objects can be spawned per object?")]
	[ShowIf("limitPerObject")]
	public int perObjectLimit = 1;

	// Token: 0x04003725 RID: 14117
	public bool limitPerRoom;

	// Token: 0x04003726 RID: 14118
	[ShowIf("limitPerRoom")]
	[Tooltip("How many of these objects can be spawned per room?")]
	public int perRoomLimit = 99;

	// Token: 0x04003727 RID: 14119
	public bool limitPerAddress;

	// Token: 0x04003728 RID: 14120
	[Tooltip("How many of these objects can be spawned per address?")]
	[ShowIf("limitPerAddress")]
	public int perAddressLimit = 99;

	// Token: 0x04003729 RID: 14121
	public bool limitInResidential;

	// Token: 0x0400372A RID: 14122
	[ShowIf("limitInResidential")]
	[Tooltip("How many of these objects can be spawned if residential?")]
	public int perResidentialLimit = 1;

	// Token: 0x0400372B RID: 14123
	public bool limitInCommercial;

	// Token: 0x0400372C RID: 14124
	[Tooltip("How many of these objects can be spawned if residential?")]
	[ShowIf("limitInCommercial")]
	public int perCommercialLimit = 1;

	// Token: 0x0400372D RID: 14125
	[Tooltip("Ban this item from being placed in certain room types")]
	[HideIf("limitToCertainRooms")]
	public List<RoomConfiguration> banFromRooms = new List<RoomConfiguration>();

	// Token: 0x0400372E RID: 14126
	[Tooltip("Only feature this item in certain room types")]
	public bool limitToCertainRooms;

	// Token: 0x0400372F RID: 14127
	[ShowIf("limitToCertainRooms")]
	public List<RoomConfiguration> onlyInRooms = new List<RoomConfiguration>();

	// Token: 0x04003730 RID: 14128
	[Tooltip("Only feature this item in certain building types")]
	public bool limitToCertainBuildings;

	// Token: 0x04003731 RID: 14129
	[ShowIf("limitToCertainBuildings")]
	public List<BuildingPreset> onlyInBuildings = new List<BuildingPreset>();

	// Token: 0x04003732 RID: 14130
	[Space(7f)]
	[Tooltip("If this is not null, it will attempt to place this evidence inside a folder matching this evidence type.")]
	public EvidencePreset attemptToStoreInFolder;

	// Token: 0x04003733 RID: 14131
	[Tooltip("If the above is not null, the chance of being placed in the folder.")]
	[Range(0f, 1f)]
	public float folderPlacementChance = 1f;

	// Token: 0x04003734 RID: 14132
	[Tooltip("If unable to place in folder, then don't place at all")]
	public bool dontPlaceIfNoFolder;

	// Token: 0x04003735 RID: 14133
	[Tooltip("Folder's ownership must match")]
	public bool folderOwnershipMustMatch;

	// Token: 0x04003736 RID: 14134
	[Tooltip("If true this will also look to spawn upon on other objects (and prioritize them)")]
	public bool useSubSpawning;

	// Token: 0x04003737 RID: 14135
	[Range(0f, 3f)]
	[Tooltip("This will try to be placed in a place of security matching this, if not higher...")]
	public int securityLevel;

	// Token: 0x04003738 RID: 14136
	[Tooltip("Rules about being placed in owned vs non-owned locations. 'Prioritise' settings will favour owned locations but sill place in non-owned, while 'only' settings will only place in that location.")]
	public InteractablePreset.OwnedPlacementRule ownedRule = InteractablePreset.OwnedPlacementRule.both;

	// Token: 0x04003739 RID: 14137
	[Tooltip("Override with ownedOnly if at work")]
	public bool overrideWithOnlyOwnedSpawnAtWork;

	// Token: 0x0400373A RID: 14138
	[Tooltip("Can sub spawn objects with this class")]
	[Space(7f)]
	public SubObjectClassPreset subSpawnClass;

	// Token: 0x0400373B RID: 14139
	[Tooltip("Sub spawning slots within this")]
	public List<InteractablePreset.SubSpawnSlot> subSpawnPositions = new List<InteractablePreset.SubSpawnSlot>();

	// Token: 0x0400373C RID: 14140
	[Header("Relocation")]
	[Tooltip("If the object is moved by this person, also set the spawn point so it doesn't get reset.")]
	public InteractablePreset.RelocationAuthority relocationAuthority;

	// Token: 0x0400373D RID: 14141
	[Tooltip("Will not reset if placed in the player's home")]
	public bool relocateIfPlacedInPlayersHome = true;

	// Token: 0x0400373E RID: 14142
	[Tooltip("AI will attempt to put back this if it is out of place")]
	public bool AIWillCorrectPosition = true;

	// Token: 0x0400373F RID: 14143
	[Header("Evidence")]
	[Tooltip("Does this interactable need to reference a piece of evidence? If true will attempt to find the evidence as below (will be overriden by passed variabes in the constructor)")]
	public bool useEvidence;

	// Token: 0x04003740 RID: 14144
	[ShowIf("useEvidence")]
	[Tooltip("If not null, will attempt to find the singleton using this preset...")]
	public EvidencePreset useSingleton;

	// Token: 0x04003741 RID: 14145
	[Tooltip("Use a specific evidence from below")]
	[ShowIf("useEvidence")]
	public InteractablePreset.FindEvidence findEvidence;

	// Token: 0x04003742 RID: 14146
	[Tooltip("Create an evidence class of below")]
	public EvidencePreset spawnEvidence;

	// Token: 0x04003743 RID: 14147
	[Tooltip("On create evidence: Use the item's location as evidence parent")]
	[ShowIf("useEvidence")]
	public bool locationIsParent = true;

	// Token: 0x04003744 RID: 14148
	[Tooltip("Use this DDS message ID for the summary")]
	public string summaryMessageSource;

	// Token: 0x04003745 RID: 14149
	[Space(7f)]
	public bool overrideEvidencePhotoSettings;

	// Token: 0x04003746 RID: 14150
	[ShowIf("overrideEvidencePhotoSettings")]
	public Vector3 relativeCamPhotoPos = new Vector3(-0.075f, 0.25f, 0.2f);

	// Token: 0x04003747 RID: 14151
	[ShowIf("overrideEvidencePhotoSettings")]
	public Vector3 relativeCamPhotoEuler = new Vector3(45f, 155f, 0f);

	// Token: 0x04003748 RID: 14152
	[Header("Locks")]
	public InteractablePreset includeLock;

	// Token: 0x04003749 RID: 14153
	public Vector3 lockOffset = Vector3.zero;

	// Token: 0x0400374A RID: 14154
	[Tooltip("Preferred password source")]
	public RoomConfiguration.RoomPasswordPreference passwordSource;

	// Token: 0x0400374B RID: 14155
	[Tooltip("Play this when attempted to open while locked")]
	public AudioEvent attemptedOpenSound;

	// Token: 0x0400374C RID: 14156
	[Tooltip("The lock is armed when the door movement is closed")]
	public bool armLockOnClose = true;

	// Token: 0x0400374D RID: 14157
	[Tooltip("If this isn't an actual door, this is the lock strength range...")]
	public Vector2 lockStrength = new Vector2(0.1f, 0.4f);

	// Token: 0x0400374E RID: 14158
	[Tooltip("This object itself acts as the lock")]
	public bool isSelfLock;

	// Token: 0x0400374F RID: 14159
	[Header("Material Changes")]
	public bool useMaterialChanges;

	// Token: 0x04003750 RID: 14160
	[ShowIf("useMaterialChanges")]
	public Material lockOffMaterial;

	// Token: 0x04003751 RID: 14161
	[ShowIf("useMaterialChanges")]
	public Material lockOnMaterial;

	// Token: 0x04003752 RID: 14162
	[Header("Computer")]
	[Tooltip("Is this a computer (cruncher)?")]
	public bool isComputer;

	// Token: 0x04003753 RID: 14163
	[Tooltip("The boot application")]
	[ShowIf("isComputer")]
	public CruncherAppPreset bootApp;

	// Token: 0x04003754 RID: 14164
	[Tooltip("The booted app (what this boots to)")]
	[ShowIf("isComputer")]
	public CruncherAppPreset logInApp;

	// Token: 0x04003755 RID: 14165
	[ShowIf("isComputer")]
	[Tooltip("The desktop app")]
	public CruncherAppPreset desktopApp;

	// Token: 0x04003756 RID: 14166
	[Tooltip("Additional apps")]
	[ShowIf("isComputer")]
	public List<CruncherAppPreset> additionalApps = new List<CruncherAppPreset>();

	// Token: 0x04003757 RID: 14167
	[Header("Fingerprints")]
	[Tooltip("Should there be fingerprints here?")]
	public bool fingerprintsEnabled = true;

	// Token: 0x04003758 RID: 14168
	[ShowIf("fingerprintsEnabled")]
	[Tooltip("The source of the prints")]
	public RoomConfiguration.PrintsSource printsSource = RoomConfiguration.PrintsSource.ownersWritersReceivers;

	// Token: 0x04003759 RID: 14169
	[ShowIf("fingerprintsEnabled")]
	[Tooltip("Fingerprint density")]
	[Range(0f, 5f)]
	public float fingerprintDensity = 3f;

	// Token: 0x0400375A RID: 14170
	[Tooltip("Dynamic fingerprints will be left when an AI uses this")]
	[ShowIf("fingerprintsEnabled")]
	public bool enableDynamicFingerprints = true;

	// Token: 0x0400375B RID: 14171
	[ShowIf("fingerprintsEnabled")]
	[Tooltip("Override the default fingerprint maximum")]
	public bool overrideMaxDynamicFingerprints;

	// Token: 0x0400375C RID: 14172
	[ShowIf("fingerprintsEnabled")]
	[EnableIf("overrideMaxDynamicFingerprints")]
	public int maxDynamicFingerprints = 8;

	// Token: 0x0400375D RID: 14173
	[Header("First Person Setup")]
	[Tooltip("If this is a first person item, the corresponding item ID")]
	public FirstPersonItem fpsItem;

	// Token: 0x0400375E RID: 14174
	public bool isInventoryItem;

	// Token: 0x0400375F RID: 14175
	[ShowIf("isInventoryItem")]
	[Tooltip("Offset of held item")]
	public Vector3 fpsItemOffset = Vector3.zero;

	// Token: 0x04003760 RID: 14176
	[ShowIf("isInventoryItem")]
	public Vector3 fpsItemRotation = Vector3.zero;

	// Token: 0x04003761 RID: 14177
	[Tooltip("The amount of consumable; consumed at 1 per second by the player")]
	public float consumableAmount;

	// Token: 0x04003762 RID: 14178
	[Tooltip("Destroy when this is all consumed")]
	[ShowIf("isInventoryItem")]
	public bool destroyWhenAllConsumed;

	// Token: 0x04003763 RID: 14179
	[Tooltip("Trash object")]
	[ShowIf("isInventoryItem")]
	public bool useSameModelAsTrash = true;

	// Token: 0x04003764 RID: 14180
	[DisableIf("useSameModelAsTrash")]
	[ShowIf("isInventoryItem")]
	public InteractablePreset trashItem;

	// Token: 0x04003765 RID: 14181
	[ShowIf("isInventoryItem")]
	public AudioEvent playerConsumeLoop;

	// Token: 0x04003766 RID: 14182
	[ShowIf("isInventoryItem")]
	public AudioEvent takeOneEvent;

	// Token: 0x04003767 RID: 14183
	[ShowIf("isInventoryItem")]
	[DisableIf("destroyWhenAllConsumed")]
	[Space(7f)]
	public Human.DisposalType disposal;

	// Token: 0x04003768 RID: 14184
	[Range(0f, 1f)]
	[DisableIf("destroyWhenAllConsumed")]
	public float chanceOfDroppedAngle;

	// Token: 0x04003769 RID: 14185
	[DisableIf("destroyWhenAllConsumed")]
	public float droppedAngleHeightBoost = 0.1f;

	// Token: 0x0400376A RID: 14186
	[ShowIf("isInventoryItem")]
	public MurderWeaponPreset weapon;

	// Token: 0x0400376B RID: 14187
	[ShowIf("isInventoryItem")]
	[Tooltip("If in inventory, display object")]
	public bool inventoryCarryItem;

	// Token: 0x0400376C RID: 14188
	[ShowIf("isInventoryItem")]
	[Tooltip("This required a carrying animation")]
	public bool requiredCarryAnimation = true;

	// Token: 0x0400376D RID: 14189
	[ShowIf("isInventoryItem")]
	[Tooltip("If an AI can carry this, which carrying animation to play")]
	public int aiCarryAnimation = 1;

	// Token: 0x0400376E RID: 14190
	[ShowIf("isInventoryItem")]
	[Tooltip("position object by this when AI is holding")]
	public Vector3 aiHeldObjectPosition = new Vector3(-0.05f, -0.1f, -0.02f);

	// Token: 0x0400376F RID: 14191
	[Tooltip("Rotate object by this when AI is holding")]
	[ShowIf("isInventoryItem")]
	public Vector3 aiHeldObjectRotation = new Vector3(90f, 0f, 0f);

	// Token: 0x04003770 RID: 14192
	[Tooltip("The AI will put this down when at home")]
	[ShowIf("isInventoryItem")]
	public bool putDownAtHome;

	// Token: 0x04003771 RID: 14193
	[Tooltip("The AI will take this when they leave home")]
	public bool takeWith;

	// Token: 0x04003772 RID: 14194
	[ShowIf("isInventoryItem")]
	public List<SubObjectClassPreset> putDownPositions = new List<SubObjectClassPreset>();

	// Token: 0x04003773 RID: 14195
	[ShowIf("isInventoryItem")]
	public List<SubObjectClassPreset> backupPutDownPositions = new List<SubObjectClassPreset>();

	// Token: 0x04003774 RID: 14196
	[Header("Special cases")]
	public InteractablePreset.SpecialCase specialCaseFlag;

	// Token: 0x04003775 RID: 14197
	[Tooltip("Affect room steam amount with switch state 1")]
	public bool affectRoomSteamLevel;

	// Token: 0x04003776 RID: 14198
	[Tooltip("This is a payphone")]
	public bool isPayphone;

	// Token: 0x04003777 RID: 14199
	[Tooltip("This is a clock; use hourly chimes")]
	public bool isClock;

	// Token: 0x04003778 RID: 14200
	[Tooltip("If true this will be a naming special case.")]
	public bool isMoney;

	// Token: 0x04003779 RID: 14201
	[Tooltip("According to AI, only 1 entertainment source should be active in a room")]
	public bool entertainmentSource;

	// Token: 0x0400377A RID: 14202
	[Tooltip("Is this a heat source? Only active when switch 0 is on")]
	public bool isHeatSource;

	// Token: 0x0400377B RID: 14203
	[Tooltip("Mark this as trash as soon as it is created, for removal as soon as possible")]
	public bool markAsTrashOnCreate;

	// Token: 0x0400377C RID: 14204
	[Tooltip("If picked up, the AI will seek to put this in a bin/gets added to their carrying trash")]
	public bool isLitter;

	// Token: 0x0400377D RID: 14205
	[Tooltip("Will require an art asset sent to a decal projector")]
	public bool isDecal;

	// Token: 0x0400377E RID: 14206
	[Tooltip("Resets switch states to starting configuration after x amount of time")]
	public bool resetSwitchStates;

	// Token: 0x0400377F RID: 14207
	[EnableIf("resetSwitchStates")]
	public float resetTimer = 0.01f;

	// Token: 0x04003780 RID: 14208
	[Tooltip("Don't save switch states")]
	public bool dontSaveSwitchStates;

	// Token: 0x04003781 RID: 14209
	[Tooltip("Don't load switch states")]
	public bool dontLoadSwitchStates;

	// Token: 0x04003782 RID: 14210
	[Tooltip("If true, the game will record the creation time of this in passed variables")]
	public bool recordCreationTime;

	// Token: 0x04003783 RID: 14211
	[Tooltip("If this is a music player: Track list")]
	public List<AudioEvent> musicTracks = new List<AudioEvent>();

	// Token: 0x04003784 RID: 14212
	[Tooltip("Is this a retailItem? If so here's the reference. This is set by having a RetailItem Preset that points to this.")]
	public RetailItemPreset retailItem;

	// Token: 0x04003785 RID: 14213
	[Tooltip("If this is associated with a shop interface, override the location's menu with this one (useful for vending machines)")]
	public MenuPreset menuOverride;

	// Token: 0x04003786 RID: 14214
	[ShowIf("isClock")]
	public AudioEvent hourlyChime;

	// Token: 0x04003787 RID: 14215
	[ShowIf("isClock")]
	[Tooltip("Do as many chimes as the hour dictates")]
	public bool chimeEqualToHour;

	// Token: 0x04003788 RID: 14216
	[ShowIf("isClock")]
	[Tooltip("Delay between chimes if above is true")]
	public float chimeDelay = 1.5f;

	// Token: 0x04003789 RID: 14217
	[Tooltip("Audio loop played on search")]
	public AudioEvent searchLoop;

	// Token: 0x0400378A RID: 14218
	[Header("Debug")]
	public InteractablePreset copyFrom;

	// Token: 0x0200074B RID: 1867
	[Serializable]
	public class AIUseSetting
	{
		// Token: 0x0400378B RID: 14219
		[Tooltip("Usage point relative to the interactable. Calculated on spawn/move position.")]
		public Vector3 usageOffset = new Vector3(0f, 0f, 0.35f);

		// Token: 0x0400378C RID: 14220
		[Tooltip("Look at point relative to the interactable. Calculated on spawn/move position.")]
		public Vector3 facingOffset;

		// Token: 0x0400378D RID: 14221
		[Tooltip("If true, this will use the parent node's floor Y value for the position")]
		public bool useNodeFloorPosition = true;

		// Token: 0x0400378E RID: 14222
		[Tooltip("If true, flip the Z axis usage offset depending on actor relative position to the door")]
		public bool useDoorBehaviour;

		// Token: 0x0400378F RID: 14223
		[Tooltip("If true use the citizen's sitting offset position")]
		public bool useSittingOffset;

		// Token: 0x04003790 RID: 14224
		[Tooltip("If true use the citizen's standing offset position")]
		public bool useArmsStandingOffset;
	}

	// Token: 0x0200074C RID: 1868
	public enum InteractionKey
	{
		// Token: 0x04003792 RID: 14226
		none,
		// Token: 0x04003793 RID: 14227
		primary,
		// Token: 0x04003794 RID: 14228
		secondary,
		// Token: 0x04003795 RID: 14229
		alternative,
		// Token: 0x04003796 RID: 14230
		scrollAxisUp,
		// Token: 0x04003797 RID: 14231
		scrollAxisDown,
		// Token: 0x04003798 RID: 14232
		jump,
		// Token: 0x04003799 RID: 14233
		crouch,
		// Token: 0x0400379A RID: 14234
		sprint,
		// Token: 0x0400379B RID: 14235
		flashlight,
		// Token: 0x0400379C RID: 14236
		caseBoard,
		// Token: 0x0400379D RID: 14237
		map,
		// Token: 0x0400379E RID: 14238
		notebook,
		// Token: 0x0400379F RID: 14239
		moveHorizontal,
		// Token: 0x040037A0 RID: 14240
		moveVertical,
		// Token: 0x040037A1 RID: 14241
		lookHorizontal,
		// Token: 0x040037A2 RID: 14242
		lookVertical,
		// Token: 0x040037A3 RID: 14243
		WeaponSelect,
		// Token: 0x040037A4 RID: 14244
		nearestInteractable,
		// Token: 0x040037A5 RID: 14245
		CaseBoardZoomAxis,
		// Token: 0x040037A6 RID: 14246
		MoveEvidenceAxisX,
		// Token: 0x040037A7 RID: 14247
		MoveEvidenceAxisY,
		// Token: 0x040037A8 RID: 14248
		ContentMoveAxisX,
		// Token: 0x040037A9 RID: 14249
		ContentMoveAxisY,
		// Token: 0x040037AA RID: 14250
		SelectLeft,
		// Token: 0x040037AB RID: 14251
		SelectRight,
		// Token: 0x040037AC RID: 14252
		SelectUp,
		// Token: 0x040037AD RID: 14253
		SelectDown,
		// Token: 0x040037AE RID: 14254
		CreateString,
		// Token: 0x040037AF RID: 14255
		LeanLeft,
		// Token: 0x040037B0 RID: 14256
		LeanRight
	}

	// Token: 0x0200074D RID: 1869
	public enum Switch
	{
		// Token: 0x040037B2 RID: 14258
		switchState,
		// Token: 0x040037B3 RID: 14259
		custom1,
		// Token: 0x040037B4 RID: 14260
		custom2,
		// Token: 0x040037B5 RID: 14261
		custom3,
		// Token: 0x040037B6 RID: 14262
		lockState,
		// Token: 0x040037B7 RID: 14263
		lockedIn,
		// Token: 0x040037B8 RID: 14264
		sprinting,
		// Token: 0x040037B9 RID: 14265
		enforcersInside,
		// Token: 0x040037BA RID: 14266
		ko,
		// Token: 0x040037BB RID: 14267
		securityGrid,
		// Token: 0x040037BC RID: 14268
		carryPhysicsObject
	}

	// Token: 0x0200074E RID: 1870
	[Serializable]
	public class SwitchState
	{
		// Token: 0x040037BD RID: 14269
		public InteractablePreset.Switch switchState;

		// Token: 0x040037BE RID: 14270
		public bool boolIs;
	}

	// Token: 0x0200074F RID: 1871
	[Serializable]
	public class IfSwitchState
	{
		// Token: 0x040037BF RID: 14271
		public InteractablePreset.Switch switchState;

		// Token: 0x040037C0 RID: 14272
		public bool boolIs;
	}

	// Token: 0x02000750 RID: 1872
	[Serializable]
	public class IfSwitchStateSFX
	{
		// Token: 0x040037C1 RID: 14273
		public InteractablePreset.Switch switchState;

		// Token: 0x040037C2 RID: 14274
		public bool boolIs;

		// Token: 0x040037C3 RID: 14275
		public AudioEvent triggerAudio;

		// Token: 0x040037C4 RID: 14276
		public bool isLoop;

		// Token: 0x040037C5 RID: 14277
		public bool isBroadcast;

		// Token: 0x040037C6 RID: 14278
		public bool isMusicPlayer;

		// Token: 0x040037C7 RID: 14279
		public AudioController.StopType stop = AudioController.StopType.fade;

		// Token: 0x040037C8 RID: 14280
		[Tooltip("Passes an open parameter to FMOD based on switch state")]
		public bool passOpenParam;

		// Token: 0x040037C9 RID: 14281
		[Tooltip("Passes the consumable state parameter to FMOD based on switch state")]
		public bool passCSParam;

		// Token: 0x040037CA RID: 14282
		[Tooltip("Pass door opening or closing direction")]
		public bool passDoorDirParam;

		// Token: 0x040037CB RID: 14283
		[Tooltip("Only if player is inside sync bed/chamber")]
		public bool onlyIfInSyncBed;

		// Token: 0x040037CC RID: 14284
		[Tooltip("Only if player is not inside sync bed/chamber")]
		public bool onlyIfNotInSyncBed;

		// Token: 0x040037CD RID: 14285
		[Tooltip("Only if this door features a neon sign")]
		public bool onlyIfNeonSign;
	}

	// Token: 0x02000751 RID: 1873
	[Serializable]
	public class InteractionAction
	{
		// Token: 0x06002578 RID: 9592 RVA: 0x001E6C0C File Offset: 0x001E4E0C
		public InteractablePreset.InteractionKey GetInteractionKey()
		{
			InteractablePreset.InteractionKey defaultKey;
			if (this.useDefaultKeySetting && this.action != null)
			{
				defaultKey = this.action.defaultKey;
			}
			else
			{
				defaultKey = this.keyOverride;
			}
			return defaultKey;
		}

		// Token: 0x040037CE RID: 14286
		[Tooltip("The dictionary reference to this action's name")]
		public string interactionName;

		// Token: 0x040037CF RID: 14287
		[Tooltip("The action preset")]
		public AIActionPreset action;

		// Token: 0x040037D0 RID: 14288
		[Tooltip("Use the default key as found on the action preset...")]
		public bool useDefaultKeySetting = true;

		// Token: 0x040037D1 RID: 14289
		[Tooltip("Which key will activate this?")]
		public InteractablePreset.InteractionKey keyOverride;

		// Token: 0x040037D2 RID: 14290
		[Tooltip("Alter the interaction name based on special cases")]
		public InteractablePreset.InteractionAction.SpecialCase specialCase;

		// Token: 0x040037D3 RID: 14291
		[Tooltip("Is this usable by the AI")]
		[Space(7f)]
		public bool usableByAI = true;

		// Token: 0x040037D4 RID: 14292
		[Tooltip("When AI is performing this, use a delay (seconds)")]
		[ShowIf("usableByAI")]
		public float aiUsageDelay;

		// Token: 0x040037D5 RID: 14293
		[Tooltip("This action effects these states")]
		[Space(7f)]
		public List<InteractablePreset.SwitchState> effectSwitchStates = new List<InteractablePreset.SwitchState>();

		// Token: 0x040037D6 RID: 14294
		[Tooltip("This action is only enabled if the following is true")]
		[Space(7f)]
		public List<InteractablePreset.IfSwitchState> onlyActiveIf = new List<InteractablePreset.IfSwitchState>();

		// Token: 0x040037D7 RID: 14295
		[Tooltip("Is this action illegal?")]
		[Space(7f)]
		public bool actionIsIllegal;

		// Token: 0x040037D8 RID: 14296
		[Tooltip("Is this action available while illegal?")]
		public bool availableWhileIllegal = true;

		// Token: 0x040037D9 RID: 14297
		[EnableIf("availableWhileIllegal")]
		[Tooltip("If above is true, is this allowed when others have witnessed illegal activity?")]
		public bool availableWhileWitnessesToIllegal;

		// Token: 0x040037DA RID: 14298
		[Tooltip("Force availability on restrained while illegal status is active")]
		public bool onlyAvailableToRestrainedWhileIllegal;

		// Token: 0x040037DB RID: 14299
		[Tooltip("Is this action available while using a locked in action?")]
		public bool availableWhileLockedIn;

		// Token: 0x040037DC RID: 14300
		[Tooltip("Is this action available while jumping?")]
		public bool availableWhileJumping = true;

		// Token: 0x040037DD RID: 14301
		[Tooltip("Cost of performing this action")]
		public int actionCost;

		// Token: 0x040037DE RID: 14302
		[Tooltip("If true when this action is unavailable, it will be striked through instead of invisible")]
		[Space(5f)]
		public bool useStrikethrough;

		// Token: 0x040037DF RID: 14303
		[Tooltip("Is this a hiding place?")]
		public bool isHidingPlace;

		// Token: 0x040037E0 RID: 14304
		[EnableIf("isHidingPlace")]
		[Tooltip("Only a hiding place in areas classed as public")]
		public bool onlyHidingPlaceIfPublic;

		// Token: 0x040037E1 RID: 14305
		[Tooltip("Sound event reference for this action")]
		[Space(7f)]
		public AudioEvent soundEvent;

		// Token: 0x040037E2 RID: 14306
		[Tooltip("If true this sound event will automatically be played on trigger. If false then this is just a reference for the sound indicator to known the sound level.")]
		public bool playOnTrigger = true;

		// Token: 0x02000752 RID: 1874
		public enum SpecialCase
		{
			// Token: 0x040037E4 RID: 14308
			none,
			// Token: 0x040037E5 RID: 14309
			takeSwap,
			// Token: 0x040037E6 RID: 14310
			onlyIfDeadAsleepOrUncon,
			// Token: 0x040037E7 RID: 14311
			availableInFastForward,
			// Token: 0x040037E8 RID: 14312
			onlyAvailableInFastForward,
			// Token: 0x040037E9 RID: 14313
			caseFormsNeeded,
			// Token: 0x040037EA RID: 14314
			activeCaseHandInReady,
			// Token: 0x040037EB RID: 14315
			search,
			// Token: 0x040037EC RID: 14316
			knockOnDoor,
			// Token: 0x040037ED RID: 14317
			putBack,
			// Token: 0x040037EE RID: 14318
			originalPlace,
			// Token: 0x040037EF RID: 14319
			onlyIfRestrained,
			// Token: 0x040037F0 RID: 14320
			onlyIfNotRestrained,
			// Token: 0x040037F1 RID: 14321
			ifInventoryItemDrawn,
			// Token: 0x040037F2 RID: 14322
			onlyIfSick,
			// Token: 0x040037F3 RID: 14323
			nonCombat,
			// Token: 0x040037F4 RID: 14324
			onlyIfMultiPageHasPages,
			// Token: 0x040037F5 RID: 14325
			onlyInNormalTimeAndAwakeNonDialog,
			// Token: 0x040037F6 RID: 14326
			nonDialog,
			// Token: 0x040037F7 RID: 14327
			decorPlacementPurchase,
			// Token: 0x040037F8 RID: 14328
			furniturePlacement,
			// Token: 0x040037F9 RID: 14329
			decorItemPlacement,
			// Token: 0x040037FA RID: 14330
			citizenReturn,
			// Token: 0x040037FB RID: 14331
			nonCombatOrRestrained
		}
	}

	// Token: 0x02000753 RID: 1875
	public enum InteractableColourSetting
	{
		// Token: 0x040037FD RID: 14333
		none,
		// Token: 0x040037FE RID: 14334
		ownersFavColour,
		// Token: 0x040037FF RID: 14335
		randomColour,
		// Token: 0x04003800 RID: 14336
		randomDecorColour,
		// Token: 0x04003801 RID: 14337
		syncDisk
	}

	// Token: 0x02000754 RID: 1876
	public enum ItemClass
	{
		// Token: 0x04003803 RID: 14339
		consumable,
		// Token: 0x04003804 RID: 14340
		medical,
		// Token: 0x04003805 RID: 14341
		equipment,
		// Token: 0x04003806 RID: 14342
		document,
		// Token: 0x04003807 RID: 14343
		misc,
		// Token: 0x04003808 RID: 14344
		electronics
	}

	// Token: 0x02000755 RID: 1877
	public enum ApartmentPlacementMode
	{
		// Token: 0x0400380A RID: 14346
		physics,
		// Token: 0x0400380B RID: 14347
		vertical,
		// Token: 0x0400380C RID: 14348
		ceiling
	}

	// Token: 0x02000756 RID: 1878
	[Serializable]
	public class AIUsePriority
	{
		// Token: 0x0400380D RID: 14349
		public List<AIActionPreset> actions;

		// Token: 0x0400380E RID: 14350
		[Tooltip("AI will rank actions by this if there are multiple copies")]
		[Range(0f, 10f)]
		public float AIPriority = 5f;

		// Token: 0x0400380F RID: 14351
		[Tooltip("When chosing between interactables, how much to factor in the closest one?")]
		public float pickDistanceMultiplier = 1f;
	}

	// Token: 0x02000757 RID: 1879
	[Serializable]
	public class ObjectResetBehaviour
	{
		// Token: 0x04003810 RID: 14352
		public InteractablePreset.Switch ifSwitchState;

		// Token: 0x04003811 RID: 14353
		public bool ifSwitchBool;

		// Token: 0x04003812 RID: 14354
		public InteractablePreset.ObjectResetCondition ifCondition;

		// Token: 0x04003813 RID: 14355
		public AIGoalPreset ifGoal;

		// Token: 0x04003814 RID: 14356
		public InteractablePreset.ObjectResetScope scope;

		// Token: 0x04003815 RID: 14357
		public bool onlyIfObjectBelongsTo;

		// Token: 0x04003816 RID: 14358
		public bool onlyIfAuthority = true;

		// Token: 0x04003817 RID: 14359
		public bool onlyIfLastOccupant;

		// Token: 0x04003818 RID: 14360
		public bool onlyIfHome;

		// Token: 0x04003819 RID: 14361
		public List<AIActionPreset> insertActions = new List<AIActionPreset>();
	}

	// Token: 0x02000758 RID: 1880
	public enum ObjectResetCondition
	{
		// Token: 0x0400381B RID: 14363
		leavingLocation,
		// Token: 0x0400381C RID: 14364
		goalActive,
		// Token: 0x0400381D RID: 14365
		goalActivated,
		// Token: 0x0400381E RID: 14366
		goalDeactivated
	}

	// Token: 0x02000759 RID: 1881
	public enum ObjectResetScope
	{
		// Token: 0x04003820 RID: 14368
		ifInSameRoom,
		// Token: 0x04003821 RID: 14369
		ifInSameLocation
	}

	// Token: 0x0200075A RID: 1882
	public enum ReadingModeSource
	{
		// Token: 0x04003823 RID: 14371
		evidenceNote,
		// Token: 0x04003824 RID: 14372
		multipageEvidence,
		// Token: 0x04003825 RID: 14373
		time,
		// Token: 0x04003826 RID: 14374
		bookPreset,
		// Token: 0x04003827 RID: 14375
		recordPreset,
		// Token: 0x04003828 RID: 14376
		syncDiskPreset,
		// Token: 0x04003829 RID: 14377
		mainEvidenceText,
		// Token: 0x0400382A RID: 14378
		kaizenSkillDisplay
	}

	// Token: 0x0200075B RID: 1883
	public enum AutoPlacement
	{
		// Token: 0x0400382C RID: 14380
		always,
		// Token: 0x0400382D RID: 14381
		onlyInCompany,
		// Token: 0x0400382E RID: 14382
		onlyInHomes,
		// Token: 0x0400382F RID: 14383
		onlyOnStreet,
		// Token: 0x04003830 RID: 14384
		never
	}

	// Token: 0x0200075C RID: 1884
	[Serializable]
	public class TraitPick
	{
		// Token: 0x04003831 RID: 14385
		public CharacterTrait.RuleType rule;

		// Token: 0x04003832 RID: 14386
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x04003833 RID: 14387
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		public bool mustPassForApplication = true;

		// Token: 0x04003834 RID: 14388
		[Range(0f, 20f)]
		[Tooltip("If the rules match, then apply this frequency")]
		public int appliedFrequencyMin;

		// Token: 0x04003835 RID: 14389
		[Tooltip("If the rules match, then apply this frequency")]
		[Range(0f, 20f)]
		public int appliedFrequencyMax;
	}

	// Token: 0x0200075D RID: 1885
	public enum OwnedPlacementRule
	{
		// Token: 0x04003837 RID: 14391
		nonOwnedOnly,
		// Token: 0x04003838 RID: 14392
		ownedOnly,
		// Token: 0x04003839 RID: 14393
		prioritiseNonOwned,
		// Token: 0x0400383A RID: 14394
		prioritiseOwned,
		// Token: 0x0400383B RID: 14395
		both
	}

	// Token: 0x0200075E RID: 1886
	public enum RelocationAuthority
	{
		// Token: 0x0400383D RID: 14397
		AIAndOwnersCanRelocate,
		// Token: 0x0400383E RID: 14398
		ownerCanRelocate,
		// Token: 0x0400383F RID: 14399
		anyoneCanRelocate,
		// Token: 0x04003840 RID: 14400
		nooneCanRelocate
	}

	// Token: 0x0200075F RID: 1887
	public enum FindEvidence
	{
		// Token: 0x04003842 RID: 14402
		none,
		// Token: 0x04003843 RID: 14403
		residentsContract,
		// Token: 0x04003844 RID: 14404
		sideJob,
		// Token: 0x04003845 RID: 14405
		companyRoster,
		// Token: 0x04003846 RID: 14406
		addressKey,
		// Token: 0x04003847 RID: 14407
		businessCard,
		// Token: 0x04003848 RID: 14408
		namePlacard,
		// Token: 0x04003849 RID: 14409
		photo,
		// Token: 0x0400384A RID: 14410
		calendar,
		// Token: 0x0400384B RID: 14411
		retailItem,
		// Token: 0x0400384C RID: 14412
		workID,
		// Token: 0x0400384D RID: 14413
		salesRecords,
		// Token: 0x0400384E RID: 14414
		diary,
		// Token: 0x0400384F RID: 14415
		menu,
		// Token: 0x04003850 RID: 14416
		homeFile,
		// Token: 0x04003851 RID: 14417
		birthCertificate,
		// Token: 0x04003852 RID: 14418
		bankStatement,
		// Token: 0x04003853 RID: 14419
		medicalDetails,
		// Token: 0x04003854 RID: 14420
		IDCard,
		// Token: 0x04003855 RID: 14421
		addressBook,
		// Token: 0x04003856 RID: 14422
		residentRoster,
		// Token: 0x04003857 RID: 14423
		telephone,
		// Token: 0x04003858 RID: 14424
		callLogs,
		// Token: 0x04003859 RID: 14425
		hospitalBed
	}

	// Token: 0x02000760 RID: 1888
	public enum SpecialCase
	{
		// Token: 0x0400385B RID: 14427
		none,
		// Token: 0x0400385C RID: 14428
		sleepPosition,
		// Token: 0x0400385D RID: 14429
		workDesk,
		// Token: 0x0400385E RID: 14430
		workCounter,
		// Token: 0x0400385F RID: 14431
		workKitchen,
		// Token: 0x04003860 RID: 14432
		securityDoor,
		// Token: 0x04003861 RID: 14433
		alarmSystem,
		// Token: 0x04003862 RID: 14434
		sentryGun,
		// Token: 0x04003863 RID: 14435
		securityCamera,
		// Token: 0x04003864 RID: 14436
		interestBook,
		// Token: 0x04003865 RID: 14437
		bookStack,
		// Token: 0x04003866 RID: 14438
		thrownItem,
		// Token: 0x04003867 RID: 14439
		fingerprint,
		// Token: 0x04003868 RID: 14440
		shower,
		// Token: 0x04003869 RID: 14441
		syncDisk,
		// Token: 0x0400386A RID: 14442
		unused1,
		// Token: 0x0400386B RID: 14443
		unused2,
		// Token: 0x0400386C RID: 14444
		codebreaker,
		// Token: 0x0400386D RID: 14445
		doorWedge,
		// Token: 0x0400386E RID: 14446
		telephone,
		// Token: 0x0400386F RID: 14447
		hospitalBed,
		// Token: 0x04003870 RID: 14448
		syncBed,
		// Token: 0x04003871 RID: 14449
		padlock,
		// Token: 0x04003872 RID: 14450
		salesLedger,
		// Token: 0x04003873 RID: 14451
		caseTray,
		// Token: 0x04003874 RID: 14452
		footprint,
		// Token: 0x04003875 RID: 14453
		breakerSecurity,
		// Token: 0x04003876 RID: 14454
		breakerLights,
		// Token: 0x04003877 RID: 14455
		breakerDoors,
		// Token: 0x04003878 RID: 14456
		fridge,
		// Token: 0x04003879 RID: 14457
		stovetopKettle,
		// Token: 0x0400387A RID: 14458
		syncDiskUpgrade,
		// Token: 0x0400387B RID: 14459
		otherSecuritySystem,
		// Token: 0x0400387C RID: 14460
		gasReleaseSystem,
		// Token: 0x0400387D RID: 14461
		tracker,
		// Token: 0x0400387E RID: 14462
		grenade,
		// Token: 0x0400387F RID: 14463
		ballisticArmour,
		// Token: 0x04003880 RID: 14464
		forceStanding,
		// Token: 0x04003881 RID: 14465
		lightswitch,
		// Token: 0x04003882 RID: 14466
		airVent,
		// Token: 0x04003883 RID: 14467
		burningBarrel,
		// Token: 0x04003884 RID: 14468
		addressBook,
		// Token: 0x04003885 RID: 14469
		garbageDisposal,
		// Token: 0x04003886 RID: 14470
		glassBulletHole,
		// Token: 0x04003887 RID: 14471
		bloodPool,
		// Token: 0x04003888 RID: 14472
		briefcase
	}

	// Token: 0x02000761 RID: 1889
	[Serializable]
	public class SubSpawnSlot
	{
		// Token: 0x04003889 RID: 14473
		public Vector3 localPos;

		// Token: 0x0400388A RID: 14474
		public Vector3 localEuler;
	}
}
