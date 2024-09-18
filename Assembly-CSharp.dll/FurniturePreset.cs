using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000737 RID: 1847
[CreateAssetMenu(fileName = "furniture_data", menuName = "Database/Decor/Furniture Preset")]
public class FurniturePreset : SoCustomComparison
{
	// Token: 0x040035E7 RID: 13799
	[Header("Rules")]
	[Space(7f)]
	[Tooltip("Classes that this furniture belongs to")]
	public List<FurnitureClass> classes = new List<FurnitureClass>();

	// Token: 0x040035E8 RID: 13800
	[Header("Visuals")]
	public GameObject prefab;

	// Token: 0x040035E9 RID: 13801
	[Tooltip("If true this will seach for identical furniture in a room to batch with")]
	public bool allowStaticBatching;

	// Token: 0x040035EA RID: 13802
	public ObjectPoolingController.ObjectLoadRange spawnRange = ObjectPoolingController.ObjectLoadRange.far;

	// Token: 0x040035EB RID: 13803
	[DisableIf("inheritColouringFromDecor")]
	[Tooltip("Allows this furniture to check for weather affected material (only works without custom colour keys or material changes")]
	public bool allowWeatherAffectedMaterials;

	// Token: 0x040035EC RID: 13804
	[Header("AI Interaction")]
	[Tooltip("What interatables will be instanced on this? These won't be spawned but created and searched for within the furniture prefab")]
	public List<FurniturePreset.IntegratedInteractable> integratedInteractables = new List<FurniturePreset.IntegratedInteractable>();

	// Token: 0x040035ED RID: 13805
	[Tooltip("If true use across all design styles")]
	[Header("Decor Settings")]
	public bool universalDesignStyle;

	// Token: 0x040035EE RID: 13806
	public List<DesignStylePreset> designStyles = new List<DesignStylePreset>();

	// Token: 0x040035EF RID: 13807
	[Space(7f)]
	public bool inheritColouringFromDecor;

	// Token: 0x040035F0 RID: 13808
	[Tooltip("If true the same material colours will be shared over all instances of this furniture for the room")]
	public FurniturePreset.ShareColours shareColours;

	// Token: 0x040035F1 RID: 13809
	[Tooltip("If true this furniture will inherit a grub value from the decor/room")]
	public bool inheritGrubFromDecor = true;

	// Token: 0x040035F2 RID: 13810
	public List<MaterialGroupPreset.MaterialVariation> variations = new List<MaterialGroupPreset.MaterialVariation>();

	// Token: 0x040035F3 RID: 13811
	[Space(7f)]
	[Tooltip("If this is a part of a group, furntiure of the same group will be chosen in this room.")]
	public FurniturePreset.FurnitureGroup furnitureGroup;

	// Token: 0x040035F4 RID: 13812
	public int groupID;

	// Token: 0x040035F5 RID: 13813
	[Header("Material Composition")]
	[Range(0f, 1f)]
	public float concrete = 1f;

	// Token: 0x040035F6 RID: 13814
	[Range(0f, 1f)]
	public float plaster;

	// Token: 0x040035F7 RID: 13815
	[Range(0f, 1f)]
	public float wood;

	// Token: 0x040035F8 RID: 13816
	[Range(0f, 1f)]
	public float carpet;

	// Token: 0x040035F9 RID: 13817
	[Range(0f, 1f)]
	public float tile;

	// Token: 0x040035FA RID: 13818
	[Range(0f, 1f)]
	public float metal;

	// Token: 0x040035FB RID: 13819
	[Range(0f, 1f)]
	public float glass;

	// Token: 0x040035FC RID: 13820
	[Range(0f, 1f)]
	public float fabric;

	// Token: 0x040035FD RID: 13821
	[Tooltip("This is secondary to the same property in the class preset.")]
	[Header("Suitability")]
	public int minimumRoomSize = 99;

	// Token: 0x040035FE RID: 13822
	[Tooltip("Is this allowed in open plan rooms?")]
	public FurnitureCluster.AllowedOpenPlan allowedInOpenPlan;

	// Token: 0x040035FF RID: 13823
	[Space(7f)]
	[Tooltip("Only allow this in certain inhabitant presets")]
	public bool onlyAllowInFollowing;

	// Token: 0x04003600 RID: 13824
	public List<AddressPreset> allowedInAddressesOfType = new List<AddressPreset>();

	// Token: 0x04003601 RID: 13825
	[Tooltip("Ban this in certain inhabitant presets")]
	public bool banInFollowing;

	// Token: 0x04003602 RID: 13826
	public List<AddressPreset> bannedInAddressesOfType = new List<AddressPreset>();

	// Token: 0x04003603 RID: 13827
	[Space(7f)]
	[Tooltip("Only allow this in certain buildings")]
	public bool OnlyAllowInBuildings;

	// Token: 0x04003604 RID: 13828
	[EnableIf("OnlyAllowInBuildings")]
	public List<BuildingPreset> allowedInBuildings = new List<BuildingPreset>();

	// Token: 0x04003605 RID: 13829
	public bool banFromBuildings;

	// Token: 0x04003606 RID: 13830
	[EnableIf("banFromBuildings")]
	public List<BuildingPreset> notAllowedInBuildings = new List<BuildingPreset>();

	// Token: 0x04003607 RID: 13831
	[Space(7f)]
	[Tooltip("Only allow this in certain districts")]
	public bool OnlyAllowInDistricts;

	// Token: 0x04003608 RID: 13832
	[EnableIf("OnlyAllowInDistricts")]
	public List<DistrictPreset> allowedInDistricts = new List<DistrictPreset>();

	// Token: 0x04003609 RID: 13833
	public bool banFromDistricts;

	// Token: 0x0400360A RID: 13834
	[EnableIf("banFromDistricts")]
	public List<DistrictPreset> notAllowedInDistricts = new List<DistrictPreset>();

	// Token: 0x0400360B RID: 13835
	[Space(7f)]
	public bool requiresGenderedInhabitants;

	// Token: 0x0400360C RID: 13836
	public List<Human.Gender> enableIfGenderPresent = new List<Human.Gender>();

	// Token: 0x0400360D RID: 13837
	[Tooltip("The furniture is only allowed in these room types")]
	[Space(7f)]
	public List<RoomTypeFilter> allowedRoomFilters = new List<RoomTypeFilter>();

	// Token: 0x0400360E RID: 13838
	[Space(7f)]
	[Range(0f, 1f)]
	public float minimumWealth;

	// Token: 0x0400360F RID: 13839
	[Space(5f)]
	public bool usePersonalityWeighting;

	// Token: 0x04003610 RID: 13840
	[Tooltip("0 = old fashioned/conservative, 1 = modern/liberal: Driven by the design style")]
	[EnableIf("usePersonalityWeighting")]
	[Range(0f, 10f)]
	public int modernity = 5;

	// Token: 0x04003611 RID: 13841
	[Range(0f, 10f)]
	[Tooltip("0 = informal/cosy, 1 = clean/souless: Driven by the room type.")]
	[EnableIf("usePersonalityWeighting")]
	public int cleanness = 5;

	// Token: 0x04003612 RID: 13842
	[Tooltip("0 = understated/quiet, 1 = loud/bold: Driven by the owner's personality")]
	[Range(0f, 10f)]
	[EnableIf("usePersonalityWeighting")]
	public int loudness = 5;

	// Token: 0x04003613 RID: 13843
	[Tooltip("0 = cold/hard, 1 = warm/sensitive: Driven by the owner's personality")]
	[EnableIf("usePersonalityWeighting")]
	[Range(0f, 10f)]
	public int emotive = 5;

	// Token: 0x04003614 RID: 13844
	[Header("Sub Objects")]
	public List<FurniturePreset.SubObject> subObjects = new List<FurniturePreset.SubObject>();

	// Token: 0x04003615 RID: 13845
	[Tooltip("Use this setting to test for subobject spawn modifiers (see 'SubObjectClassPreset')")]
	public FurniturePreset.ModifierTest testForModifiers;

	// Token: 0x04003616 RID: 13846
	[Tooltip("Objects with illegal actions on this will override the public area allowance set in the interactable setup")]
	public bool forcePublicIllegal;

	// Token: 0x04003617 RID: 13847
	[Header("Hiding")]
	public PlayerTransitionPreset hidingEnterTransition;

	// Token: 0x04003618 RID: 13848
	public PlayerTransitionPreset hidingExitTransition;

	// Token: 0x04003619 RID: 13849
	public PlayerTransitionPreset hidingEnterTransition2;

	// Token: 0x0400361A RID: 13850
	public PlayerTransitionPreset hidingExitTransition2;

	// Token: 0x0400361B RID: 13851
	[Header("Map")]
	public Texture2D map;

	// Token: 0x0400361C RID: 13852
	public bool drawUnderWalls;

	// Token: 0x0400361D RID: 13853
	public bool ignoreDirection;

	// Token: 0x0400361E RID: 13854
	[Tooltip("Should there be fingerprints here?")]
	[Header("Fingerprints")]
	public bool fingerprintsEnabled = true;

	// Token: 0x0400361F RID: 13855
	[Tooltip("The source of the prints")]
	public RoomConfiguration.PrintsSource printsSource = RoomConfiguration.PrintsSource.inhabitants;

	// Token: 0x04003620 RID: 13856
	[Tooltip("Fingerprint density")]
	[Range(0f, 5f)]
	public float fingerprintDensity = 1f;

	// Token: 0x04003621 RID: 13857
	[Header("Environment")]
	[Tooltip("Change area colours")]
	public bool alterAreaLighting;

	// Token: 0x04003622 RID: 13858
	[EnableIf("alterAreaLighting")]
	public List<Color> possibleColours = new List<Color>();

	// Token: 0x04003623 RID: 13859
	[EnableIf("alterAreaLighting")]
	[Tooltip("This is used in combination with the following to adjust street area lighting")]
	public DistrictPreset.AffectStreetAreaLights lightOperation;

	// Token: 0x04003624 RID: 13860
	[EnableIf("alterAreaLighting")]
	public float lightAmount = 0.12f;

	// Token: 0x04003625 RID: 13861
	[Tooltip("This is added to brightness")]
	[EnableIf("alterAreaLighting")]
	public float brightnessModifier = 10f;

	// Token: 0x04003626 RID: 13862
	[Header("Decor Edit")]
	public bool purchasable = true;

	// Token: 0x04003627 RID: 13863
	public int cost = 100;

	// Token: 0x04003628 RID: 13864
	public FurniturePreset.DecorClass decorClass = FurniturePreset.DecorClass.misc;

	// Token: 0x04003629 RID: 13865
	[Space(7f)]
	[ShowAssetPreview(64, 64)]
	public Sprite staticImage;

	// Token: 0x0400362A RID: 13866
	[ReadOnly]
	public Vector3 imagePos;

	// Token: 0x0400362B RID: 13867
	[ReadOnly]
	public Vector3 imageRot;

	// Token: 0x0400362C RID: 13868
	[ReadOnly]
	public float imageScale = 1f;

	// Token: 0x0400362D RID: 13869
	[ReadOnly]
	public GameObject imagePrefabOverride;

	// Token: 0x0400362E RID: 13870
	[Header("Special")]
	[Tooltip("Is this a board where jobs can be posted?")]
	public bool isJobBoard;

	// Token: 0x0400362F RID: 13871
	[Tooltip("Is this a desk (for work)? If true furniture ownership will be assigned based on jobs.")]
	public bool isWorkPosition;

	// Token: 0x04003630 RID: 13872
	[Tooltip("Can spawn a variety of plants")]
	public bool isPlant;

	// Token: 0x04003631 RID: 13873
	[Tooltip("Does this require the game to pick a piece of art to fit this?")]
	public bool isArt;

	// Token: 0x04003632 RID: 13874
	[Tooltip("Is this a security camera? (Special limitations)")]
	public bool isSecurityCamera;

	// Token: 0x04003633 RID: 13875
	[Tooltip("If true, if the player is here, upon load the game will teleport the player to an available adjacent space")]
	public bool onLoadAdjacentPlayerTeleport;

	// Token: 0x04003634 RID: 13876
	[EnableIf("isArt")]
	public ArtPreset.ArtOrientation artOrientation;

	// Token: 0x04003635 RID: 13877
	[Tooltip("Does this require a special self employed job?")]
	public CompanyPreset createSelfEmployed;

	// Token: 0x04003636 RID: 13878
	[Tooltip("If above is true: Which slot contains the work position?")]
	public InteractableController.InteractableID workPositionID;

	// Token: 0x04003637 RID: 13879
	[Tooltip("Chance to spawn the below objects; works on a item-by-item basis")]
	public float spawnObjectOnChance = 1f;

	// Token: 0x04003638 RID: 13880
	[Tooltip("Spawns these objects once placed")]
	public List<InteractablePreset> spawnObjectsOnPlacement = new List<InteractablePreset>();

	// Token: 0x02000738 RID: 1848
	public enum SubObjectOwnership
	{
		// Token: 0x0400363A RID: 13882
		nobody,
		// Token: 0x0400363B RID: 13883
		everybody,
		// Token: 0x0400363C RID: 13884
		person0,
		// Token: 0x0400363D RID: 13885
		person1,
		// Token: 0x0400363E RID: 13886
		person2,
		// Token: 0x0400363F RID: 13887
		person3,
		// Token: 0x04003640 RID: 13888
		person4,
		// Token: 0x04003641 RID: 13889
		person5,
		// Token: 0x04003642 RID: 13890
		person6,
		// Token: 0x04003643 RID: 13891
		person7,
		// Token: 0x04003644 RID: 13892
		person8,
		// Token: 0x04003645 RID: 13893
		person9,
		// Token: 0x04003646 RID: 13894
		person10,
		// Token: 0x04003647 RID: 13895
		person11,
		// Token: 0x04003648 RID: 13896
		person12,
		// Token: 0x04003649 RID: 13897
		person13,
		// Token: 0x0400364A RID: 13898
		person14,
		// Token: 0x0400364B RID: 13899
		person15,
		// Token: 0x0400364C RID: 13900
		person16,
		// Token: 0x0400364D RID: 13901
		person17,
		// Token: 0x0400364E RID: 13902
		person18,
		// Token: 0x0400364F RID: 13903
		person19,
		// Token: 0x04003650 RID: 13904
		person20,
		// Token: 0x04003651 RID: 13905
		person21,
		// Token: 0x04003652 RID: 13906
		person22,
		// Token: 0x04003653 RID: 13907
		person23,
		// Token: 0x04003654 RID: 13908
		person24,
		// Token: 0x04003655 RID: 13909
		person25,
		// Token: 0x04003656 RID: 13910
		person26,
		// Token: 0x04003657 RID: 13911
		person27,
		// Token: 0x04003658 RID: 13912
		person28,
		// Token: 0x04003659 RID: 13913
		person29
	}

	// Token: 0x02000739 RID: 1849
	[Serializable]
	public class SubObject
	{
		// Token: 0x0400365A RID: 13914
		public SubObjectClassPreset preset;

		// Token: 0x0400365B RID: 13915
		public string parent;

		// Token: 0x0400365C RID: 13916
		public Vector3 localPos;

		// Token: 0x0400365D RID: 13917
		public Vector3 localRot;

		// Token: 0x0400365E RID: 13918
		public FurniturePreset.SubObjectOwnership belongsTo;

		// Token: 0x0400365F RID: 13919
		public int security;
	}

	// Token: 0x0200073A RID: 1850
	[Serializable]
	public class IntegratedInteractable
	{
		// Token: 0x04003660 RID: 13920
		public InteractablePreset preset;

		// Token: 0x04003661 RID: 13921
		public InteractableController.InteractableID pairToController;

		// Token: 0x04003662 RID: 13922
		public FurniturePreset.SubObjectOwnership belongsTo;
	}

	// Token: 0x0200073B RID: 1851
	public enum ShareColours
	{
		// Token: 0x04003664 RID: 13924
		none,
		// Token: 0x04003665 RID: 13925
		seating,
		// Token: 0x04003666 RID: 13926
		wallFrontage,
		// Token: 0x04003667 RID: 13927
		cabinets,
		// Token: 0x04003668 RID: 13928
		cubicles,
		// Token: 0x04003669 RID: 13929
		curtains,
		// Token: 0x0400366A RID: 13930
		telephone,
		// Token: 0x0400366B RID: 13931
		wood,
		// Token: 0x0400366C RID: 13932
		doors,
		// Token: 0x0400366D RID: 13933
		shelving,
		// Token: 0x0400366E RID: 13934
		bins,
		// Token: 0x0400366F RID: 13935
		blinds
	}

	// Token: 0x0200073C RID: 1852
	public enum FurnitureGroup
	{
		// Token: 0x04003671 RID: 13937
		none,
		// Token: 0x04003672 RID: 13938
		seating,
		// Token: 0x04003673 RID: 13939
		windowDecor
	}

	// Token: 0x0200073D RID: 1853
	public enum ModifierTest
	{
		// Token: 0x04003675 RID: 13941
		none,
		// Token: 0x04003676 RID: 13942
		testOwner,
		// Token: 0x04003677 RID: 13943
		testInhbitants
	}

	// Token: 0x0200073E RID: 1854
	public enum DecorClass
	{
		// Token: 0x04003679 RID: 13945
		chairs,
		// Token: 0x0400367A RID: 13946
		tables,
		// Token: 0x0400367B RID: 13947
		units,
		// Token: 0x0400367C RID: 13948
		electronics,
		// Token: 0x0400367D RID: 13949
		structural,
		// Token: 0x0400367E RID: 13950
		decoration,
		// Token: 0x0400367F RID: 13951
		misc
	}
}
