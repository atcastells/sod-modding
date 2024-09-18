using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007B1 RID: 1969
[CreateAssetMenu(fileName = "retailitem_data", menuName = "Database/Retail Item")]
public class RetailItemPreset : SoCustomComparison
{
	// Token: 0x04003BFD RID: 15357
	[Header("Item")]
	public InteractablePreset itemPreset;

	// Token: 0x04003BFE RID: 15358
	[Tooltip("Can this be ranked by the citizen as a favourite item? If true this will be used to calculate favourite places, as well as appear on shopping lists etc.")]
	public bool canBeFavourite = true;

	// Token: 0x04003BFF RID: 15359
	[Tooltip("If true this item stays warm for 1 hour after it was purchased.")]
	public bool isHot;

	// Token: 0x04003C00 RID: 15360
	[Tooltip("A citizen can pick this to buy at a shop")]
	public bool isConsumable = true;

	// Token: 0x04003C01 RID: 15361
	[Tooltip("If this is entered, upon singleton creation the evidence will be named using this entry.")]
	public string brandName = string.Empty;

	// Token: 0x04003C02 RID: 15362
	public List<RetailItemPreset.Tags> tags = new List<RetailItemPreset.Tags>();

	// Token: 0x04003C03 RID: 15363
	[Header("Menu")]
	public CompanyPreset.CompanyCategory desireCategory = CompanyPreset.CompanyCategory.snack;

	// Token: 0x04003C04 RID: 15364
	public RetailItemPreset.MenuCategory menuCategory;

	// Token: 0x04003C05 RID: 15365
	[Header("Ethnicity")]
	[Tooltip("Which ethnicity is this food (if any)")]
	[ReorderableList]
	public List<Descriptors.EthnicGroup> ethnicity = new List<Descriptors.EthnicGroup>();

	// Token: 0x04003C06 RID: 15366
	[Range(0f, 1f)]
	[Header("Citizen Suitability")]
	[Tooltip("Citizen's money must be higher than this to list this item in favourites")]
	public float minimumWealth;

	// Token: 0x04003C07 RID: 15367
	[ReorderableList]
	public List<CharacterTrait> mustFeatureTraits = new List<CharacterTrait>();

	// Token: 0x04003C08 RID: 15368
	[ReorderableList]
	public List<CharacterTrait> cantFeatureTrait = new List<CharacterTrait>();

	// Token: 0x04003C09 RID: 15369
	[ReorderableList]
	public List<CharacterTrait> preferredTraits = new List<CharacterTrait>();

	// Token: 0x04003C0A RID: 15370
	[Header("Stat Modifiers")]
	[Tooltip("This is applied as progress increases")]
	public float nourishment;

	// Token: 0x04003C0B RID: 15371
	[Tooltip("This is applied as progress increases")]
	public float hydration;

	// Token: 0x04003C0C RID: 15372
	[Tooltip("This is applied as progress increases")]
	public float alertness;

	// Token: 0x04003C0D RID: 15373
	[Tooltip("This is applied as progress increases")]
	public float energy;

	// Token: 0x04003C0E RID: 15374
	[Tooltip("This is applied as progress increases")]
	public float excitement;

	// Token: 0x04003C0F RID: 15375
	[Tooltip("This is applied as progress increases")]
	public float chores;

	// Token: 0x04003C10 RID: 15376
	[Tooltip("This is applied as progress increases")]
	public float hygiene;

	// Token: 0x04003C11 RID: 15377
	[Tooltip("This is applied as progress increases")]
	public float bladder;

	// Token: 0x04003C12 RID: 15378
	[Tooltip("This is applied as progress increases")]
	public float heat;

	// Token: 0x04003C13 RID: 15379
	[Tooltip("This is applied as progress increases")]
	public float drunk;

	// Token: 0x04003C14 RID: 15380
	[Tooltip("This is applied as progress increases")]
	public float sick;

	// Token: 0x04003C15 RID: 15381
	[Tooltip("This is applied as progress increases")]
	public float headache;

	// Token: 0x04003C16 RID: 15382
	[Tooltip("This is applied as progress increases")]
	public float wet;

	// Token: 0x04003C17 RID: 15383
	[Tooltip("This is applied as progress increases")]
	public float brokenLeg;

	// Token: 0x04003C18 RID: 15384
	[Tooltip("This is applied as progress increases")]
	public float bruised;

	// Token: 0x04003C19 RID: 15385
	[Tooltip("This is applied as progress increases")]
	public float blackEye;

	// Token: 0x04003C1A RID: 15386
	[Tooltip("This is applied as progress increases")]
	public float blackedOut;

	// Token: 0x04003C1B RID: 15387
	[Tooltip("This is applied as progress increases")]
	public float numb;

	// Token: 0x04003C1C RID: 15388
	[Tooltip("This is applied as progress increases")]
	public float bleeding;

	// Token: 0x04003C1D RID: 15389
	[Tooltip("This is applied as progress increases")]
	public float wellRested;

	// Token: 0x04003C1E RID: 15390
	[Tooltip("This is applied as progress increases")]
	public float breath;

	// Token: 0x04003C1F RID: 15391
	[Tooltip("This is applied as progress increases")]
	public float starchAddiction;

	// Token: 0x04003C20 RID: 15392
	[Tooltip("This is applied as progress increases")]
	public float poisoned;

	// Token: 0x04003C21 RID: 15393
	[Tooltip("This is applied as progress increases")]
	public float health;

	// Token: 0x020007B2 RID: 1970
	public enum Tags
	{
		// Token: 0x04003C23 RID: 15395
		starchProduct
	}

	// Token: 0x020007B3 RID: 1971
	public enum MenuCategory
	{
		// Token: 0x04003C25 RID: 15397
		food,
		// Token: 0x04003C26 RID: 15398
		drinks,
		// Token: 0x04003C27 RID: 15399
		snacks,
		// Token: 0x04003C28 RID: 15400
		none
	}
}
