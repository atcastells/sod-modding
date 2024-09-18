using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000698 RID: 1688
[CreateAssetMenu(fileName = "address_data", menuName = "Database/Address Preset")]
public class AddressPreset : SoCustomComparison
{
	// Token: 0x04002EC7 RID: 11975
	[Header("Zoning")]
	public bool debug;

	// Token: 0x04002EC8 RID: 11976
	[Tooltip("Fits in units of this size (in tiles)")]
	[Range(0f, 225f)]
	public int fitsUnitSizeMin;

	// Token: 0x04002EC9 RID: 11977
	[Range(0f, 225f)]
	public int fitsUnitSizeMax = 225;

	// Token: 0x04002ECA RID: 11978
	[Tooltip("If true, units incompatible with size will be completely discounted instead of just ranked lower...")]
	public bool hardSizeLimits;

	// Token: 0x04002ECB RID: 11979
	[Space(7f)]
	public Vector2 minMaxFloors = new Vector2(-1f, 999f);

	// Token: 0x04002ECC RID: 11980
	[Space(7f)]
	[Tooltip("If true, the game will place at least one of these if at all possible")]
	public bool important;

	// Token: 0x04002ECD RID: 11981
	[Tooltip("Maximum number of instances")]
	public int maxInstances = 9999;

	// Token: 0x04002ECE RID: 11982
	[Space(7f)]
	public int baseScore = 3;

	// Token: 0x04002ECF RID: 11983
	[Tooltip("Minus to base score with every instance")]
	public int baseScoreFrequencyPenalty = 1;

	// Token: 0x04002ED0 RID: 11984
	[Space(7f)]
	[Range(0f, 1f)]
	public float idealFootfall;

	// Token: 0x04002ED1 RID: 11985
	[Tooltip("How important is the correct footfall?")]
	public float footfallMultiplier = 3f;

	// Token: 0x04002ED2 RID: 11986
	public List<AddressPreset.AddressRule> addressRules = new List<AddressPreset.AddressRule>();

	// Token: 0x04002ED3 RID: 11987
	public List<BuildingPreset> limitToBuildings = new List<BuildingPreset>();

	// Token: 0x04002ED4 RID: 11988
	[Tooltip("Always pick this if it is compatible")]
	public bool forcePick;

	// Token: 0x04002ED5 RID: 11989
	[Header("Ownership")]
	[Tooltip("Does the ethnicity of this address factor in the ownership?")]
	public bool ethnicityMatters;

	// Token: 0x04002ED6 RID: 11990
	[Tooltip("Ethnicity of this address")]
	[EnableIf("ethnicityMatters")]
	public Descriptors.EthnicGroup ethnicity = Descriptors.EthnicGroup.northAmerican;

	// Token: 0x04002ED7 RID: 11991
	[Header("Compatible Layouts")]
	public List<LayoutConfiguration> compatible = new List<LayoutConfiguration>();

	// Token: 0x04002ED8 RID: 11992
	[Header("Room Config")]
	public List<RoomConfiguration> roomConfig = new List<RoomConfiguration>();

	// Token: 0x04002ED9 RID: 11993
	[Header("Access")]
	public AddressPreset.AccessType access;

	// Token: 0x04002EDA RID: 11994
	[Tooltip("If true an AI can pass through this on the way to another place (origins, destinations unaffected)")]
	public bool canPassThrough;

	// Token: 0x04002EDB RID: 11995
	[Tooltip("Are open hours dictated by a company that ajoins this?")]
	public bool openHoursDicatedByAdjoiningCompany;

	// Token: 0x04002EDC RID: 11996
	[Tooltip("The player needs a password to enter this location")]
	public bool needsPassword;

	// Token: 0x04002EDD RID: 11997
	[Tooltip("Sources for a password")]
	public List<string> dictionaryPasswordSources = new List<string>();

	// Token: 0x04002EDE RID: 11998
	[Header("Purpose")]
	[Tooltip("If a company operates this address, this is the preset")]
	public CompanyPreset company;

	// Token: 0x04002EDF RID: 11999
	[Tooltip("If a residence is at this address, this is the preset")]
	public ResidencePreset residence;

	// Token: 0x04002EE0 RID: 12000
	[Tooltip("Purpose/icon is known to the player at the start")]
	public bool playerKnowsPurpose = true;

	// Token: 0x04002EE1 RID: 12001
	[Header("Interface")]
	public Sprite evidenceIconLarge;

	// Token: 0x04002EE2 RID: 12002
	[Header("Signage")]
	[Range(0f, 1f)]
	public float chanceOfNameSignHorizontal = 1f;

	// Token: 0x04002EE3 RID: 12003
	[Tooltip("Make a sign using this character set")]
	public Vector3 horizontalSignOffset = Vector3.zero;

	// Token: 0x04002EE4 RID: 12004
	public List<NeonSignCharacters> signCharacterSet = new List<NeonSignCharacters>();

	// Token: 0x04002EE5 RID: 12005
	[Range(0f, 1f)]
	public float chanceOfNameSignVertical = 1f;

	// Token: 0x04002EE6 RID: 12006
	[Tooltip("Make a sign using one of these")]
	public List<GameObject> possibleSigns = new List<GameObject>();

	// Token: 0x04002EE7 RID: 12007
	[Header("Special Items")]
	public List<InteractablePreset> specialItems = new List<InteractablePreset>();

	// Token: 0x04002EE8 RID: 12008
	[Tooltip("Chance of a spare key being left in an adjoining lobby (will be hidden under mat, or in a plant or radiator)")]
	public float chanceOfExternalSpareKey;

	// Token: 0x04002EE9 RID: 12009
	[Header("Air Vents")]
	public Vector2 airVentRange = Vector2.one;

	// Token: 0x04002EEA RID: 12010
	[Header("Security")]
	[Tooltip("If false, this uses the building's security system")]
	public bool useOwnSecuritySystem;

	// Token: 0x04002EEB RID: 12011
	[Tooltip("If false, this uses the breaker box contained on the floor")]
	public bool useOwnBreakerBox;

	// Token: 0x04002EEC RID: 12012
	[Tooltip("If triggered, does the alarm lock down the building floor?")]
	[EnableIf("useOwnSecuritySystem")]
	public bool alarmLocksDownFloor;

	// Token: 0x04002EED RID: 12013
	[Header("Environment")]
	public bool overrideBuildingEnvironment;

	// Token: 0x04002EEE RID: 12014
	[EnableIf("overrideBuildingEnvironment")]
	public SessionData.SceneProfile sceneProfile = SessionData.SceneProfile.indoors;

	// Token: 0x04002EEF RID: 12015
	[Header("Misc")]
	[Tooltip("Are entrance doors locked by default?")]
	public bool entrancesLockedByDefault = true;

	// Token: 0x04002EF0 RID: 12016
	[Tooltip("AI leaves lights on, even when empty")]
	public bool leaveLightsOn;

	// Token: 0x04002EF1 RID: 12017
	[Tooltip("If enabled, AI leaves doesn't lock doors out of hours or when empty")]
	public bool disableLockingUp;

	// Token: 0x04002EF2 RID: 12018
	[Tooltip("Stop this from appearing in the bottom left when the player enters")]
	public bool disableLocationInformationDisplay;

	// Token: 0x04002EF3 RID: 12019
	[Tooltip("This will be included in the city directory")]
	public bool forceCityDirectoryInclusion;

	// Token: 0x04002EF4 RID: 12020
	[Tooltip("The name of this also contains the building name")]
	public bool nameFeaturesBuildingReference;

	// Token: 0x04002EF5 RID: 12021
	[Tooltip("The name of this will become the name of the building")]
	public bool overrideBuildingName;

	// Token: 0x04002EF6 RID: 12022
	[Tooltip("Employees in the same building will have this as a location of authority")]
	public bool sameBuildingEmployeesAuthority;

	// Token: 0x04002EF7 RID: 12023
	[Tooltip("Residents in the same building will have this as a location of authority")]
	public bool sameBuildingResidentsAuthority;

	// Token: 0x04002EF8 RID: 12024
	[Tooltip("This address can feature lost & found notes")]
	public bool canFeatureLostAndFound;

	// Token: 0x04002EF9 RID: 12025
	[Range(0f, 1f)]
	[Tooltip("The minimum land value for this address type")]
	public float minimumLandValue;

	// Token: 0x04002EFA RID: 12026
	[Tooltip("The maximum land value for this address type")]
	[Range(0f, 1f)]
	public float maximumLandValue = 1f;

	// Token: 0x04002EFB RID: 12027
	[Tooltip("If a sniper type killer is searching for a vantage point, allow this location")]
	public bool allowSniperVantagePoint;

	// Token: 0x04002EFC RID: 12028
	[EnableIf("allowSniperVantagePoint")]
	[Tooltip("Add weight to this being chosen as a sniper vantage point")]
	public float vantagePointBoost;

	// Token: 0x04002EFD RID: 12029
	[Header("Debug")]
	[Tooltip("If true this won't be chosen in-game")]
	public bool disableThis;

	// Token: 0x02000699 RID: 1689
	[Serializable]
	public class AddressRule
	{
		// Token: 0x04002EFE RID: 12030
		public DistrictPreset districtPreset;

		// Token: 0x04002EFF RID: 12031
		public int scoreModifier;
	}

	// Token: 0x0200069A RID: 1690
	public enum AccessType
	{
		// Token: 0x04002F01 RID: 12033
		allPublic,
		// Token: 0x04002F02 RID: 12034
		residents,
		// Token: 0x04002F03 RID: 12035
		buildingInhabitants,
		// Token: 0x04002F04 RID: 12036
		employees,
		// Token: 0x04002F05 RID: 12037
		none
	}
}
