using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006E8 RID: 1768
[CreateAssetMenu(fileName = "company_data", menuName = "Database/Company/Company Preset")]
public class CompanyPreset : SoCustomComparison
{
	// Token: 0x040032C5 RID: 12997
	[Header("Category")]
	[Tooltip("The category of this company")]
	[ReorderableList]
	public List<CompanyPreset.CompanyCategory> companyCategories;

	// Token: 0x040032C6 RID: 12998
	[Header("Legality")]
	public bool isIllegal;

	// Token: 0x040032C7 RID: 12999
	[Header("Naming")]
	[Tooltip("Use a building's name as main if there is one")]
	public bool useBuildingName;

	// Token: 0x040032C8 RID: 13000
	[DisableIf("useBuildingName")]
	[Tooltip("Use a building's overidden name as main if there is one")]
	public bool useBuildingOverrideName;

	// Token: 0x040032C9 RID: 13001
	[Tooltip("If the above is used, add this extra suffix")]
	public List<string> overrideSuffixList = new List<string>();

	// Token: 0x040032CA RID: 13002
	[Space(7f)]
	[Tooltip("Chances of using the street name as a main company name")]
	[Range(0f, 1f)]
	public float useStreetNameChance = 0.05f;

	// Token: 0x040032CB RID: 13003
	[Tooltip("Chances of using the district name as a main company name")]
	[Range(0f, 1f)]
	public float useDistrictNameChance = 0.05f;

	// Token: 0x040032CC RID: 13004
	[Range(0f, 1f)]
	[Tooltip("Chances of using the owner's first name as a main company name")]
	public float useOwnerFirstNameChance = 0.025f;

	// Token: 0x040032CD RID: 13005
	[Tooltip("Chances of using the owner's sur name as a main company name")]
	[Range(0f, 1f)]
	public float useOwnerSurNameChance = 0.025f;

	// Token: 0x040032CE RID: 13006
	[Tooltip("Chances of using the above name list as a main company name")]
	[Range(0f, 1f)]
	public float useCompanyNameListChance = 1f;

	// Token: 0x040032CF RID: 13007
	[Range(0f, 15f)]
	[Tooltip("Chance of alliteration with prefix. This will add words with the same letter to the suffix to increase the chances of picking them by this amount")]
	public int aliterationWeight = 1;

	// Token: 0x040032D0 RID: 13008
	[Range(0f, 1f)]
	[Space(5f)]
	public float prefixChance = 0.5f;

	// Token: 0x040032D1 RID: 13009
	[Tooltip("Use this name list to pick a prefix")]
	[ReorderableList]
	public List<string> prefixList = new List<string>();

	// Token: 0x040032D2 RID: 13010
	[Range(0f, 1f)]
	public float mainChance = 1f;

	// Token: 0x040032D3 RID: 13011
	[ReorderableList]
	[Tooltip("Use this name list to pick a main name")]
	public List<string> mainNamingList = new List<string>();

	// Token: 0x040032D4 RID: 13012
	[Tooltip("Append a random selection of this suffix list to the name")]
	[ReorderableList]
	public List<string> suffixList = new List<string>();

	// Token: 0x040032D5 RID: 13013
	[Tooltip("How likely is there to be 'the' appended to the start of this name")]
	public List<CompanyPreset.TheRule> theRules = new List<CompanyPreset.TheRule>();

	// Token: 0x040032D6 RID: 13014
	[Header("Wages")]
	[Tooltip("How much the lowest rank jobs earn")]
	public CompanyPreset.SalaryRange minimumSalary = CompanyPreset.SalaryRange.minimumWage;

	// Token: 0x040032D7 RID: 13015
	[Tooltip("How much the top rank jobs earn")]
	public CompanyPreset.SalaryRange topSalary = CompanyPreset.SalaryRange.veryHigh;

	// Token: 0x040032D8 RID: 13016
	[Tooltip("The pay grade curve from lowest rank to top rank")]
	public AnimationCurve payGradeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040032D9 RID: 13017
	[Tooltip("Does this company need a storefront?")]
	[Header("Retail")]
	public bool publicFacing = true;

	// Token: 0x040032DA RID: 13018
	[Tooltip("Is this company a self employed person?")]
	public bool isSelfEmployed;

	// Token: 0x040032DB RID: 13019
	[EnableIf("isSelfEmployed")]
	[Tooltip("Automatically create self employed companies")]
	public bool autoCreate;

	// Token: 0x040032DC RID: 13020
	[EnableIf("isSelfEmployed")]
	[Range(0f, 10f)]
	public int priority = 5;

	// Token: 0x040032DD RID: 13021
	[EnableIf("isSelfEmployed")]
	public float cityPopRatio = 0.03f;

	// Token: 0x040032DE RID: 13022
	[EnableIf("isSelfEmployed")]
	public int minimumNumber = 2;

	// Token: 0x040032DF RID: 13023
	[EnableIf("isSelfEmployed")]
	public int maximumNumber = 10;

	// Token: 0x040032E0 RID: 13024
	[Tooltip("If true, loitering behaviour is enabled")]
	public bool enableLoiteringBehaviour;

	// Token: 0x040032E1 RID: 13025
	[Tooltip("List of items that this shop stocks")]
	public List<MenuPreset> menus = new List<MenuPreset>();

	// Token: 0x040032E2 RID: 13026
	public bool recordSalesData;

	// Token: 0x040032E3 RID: 13027
	[EnableIf("recordSalesData")]
	public int previousFakeSalesRecords = 5;

	// Token: 0x040032E4 RID: 13028
	[EnableIf("recordSalesData")]
	[Tooltip("A citizen must have one of the following to log a sales record here...")]
	public List<CharacterTrait> requiredTraits = new List<CharacterTrait>();

	// Token: 0x040032E5 RID: 13029
	[Tooltip("Purchasing here also has a sell section")]
	public bool enableSelling;

	// Token: 0x040032E6 RID: 13030
	[EnableIf("enableSelling")]
	public bool enableSellingOfIllegalItems;

	// Token: 0x040032E7 RID: 13031
	[EnableIf("enableSelling")]
	public float sellValueMultiplier = 0.5f;

	// Token: 0x040032E8 RID: 13032
	[Header("Uniforms")]
	public List<Color> possibleUniformColours = new List<Color>();

	// Token: 0x040032E9 RID: 13033
	[Tooltip("Preset detailing work hours")]
	[Header("Work Hours")]
	public CompanyOpenHoursPreset workHours;

	// Token: 0x040032EA RID: 13034
	[Header("Hierarchy")]
	[Tooltip("This structure of this company detailing jobs")]
	public CompanyStructurePreset structure;

	// Token: 0x040032EB RID: 13035
	[Header("Special cases")]
	[Tooltip("Controls surveillance of building")]
	public bool controlsBuildingSurveillance;

	// Token: 0x040032EC RID: 13036
	[Tooltip("For easily identifying a hotel")]
	public bool isHotel;

	// Token: 0x020006E9 RID: 1769
	public enum CompanyCategory
	{
		// Token: 0x040032EE RID: 13038
		meal,
		// Token: 0x040032EF RID: 13039
		snack,
		// Token: 0x040032F0 RID: 13040
		caffeine,
		// Token: 0x040032F1 RID: 13041
		groceries,
		// Token: 0x040032F2 RID: 13042
		washing,
		// Token: 0x040032F3 RID: 13043
		medical,
		// Token: 0x040032F4 RID: 13044
		recreational,
		// Token: 0x040032F5 RID: 13045
		retail
	}

	// Token: 0x020006EA RID: 1770
	public enum SalaryRange
	{
		// Token: 0x040032F7 RID: 13047
		illegal,
		// Token: 0x040032F8 RID: 13048
		minimumWage,
		// Token: 0x040032F9 RID: 13049
		low,
		// Token: 0x040032FA RID: 13050
		average,
		// Token: 0x040032FB RID: 13051
		aboveAverage,
		// Token: 0x040032FC RID: 13052
		high,
		// Token: 0x040032FD RID: 13053
		veryHigh,
		// Token: 0x040032FE RID: 13054
		extreme,
		// Token: 0x040032FF RID: 13055
		millionaire
	}

	// Token: 0x020006EB RID: 1771
	public enum NameComponent
	{
		// Token: 0x04003301 RID: 13057
		prefix,
		// Token: 0x04003302 RID: 13058
		main,
		// Token: 0x04003303 RID: 13059
		suffix
	}

	// Token: 0x020006EC RID: 1772
	[Serializable]
	public class TheRule
	{
		// Token: 0x04003304 RID: 13060
		public CompanyPreset.NameComponent component;

		// Token: 0x04003305 RID: 13061
		public bool exists;

		// Token: 0x04003306 RID: 13062
		public float chanceModifier;
	}
}
