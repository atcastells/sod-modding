using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006C0 RID: 1728
[CreateAssetMenu(fileName = "book_data", menuName = "Database/Book")]
public class BookPreset : SoCustomComparison
{
	// Token: 0x04003144 RID: 12612
	[ReadOnly]
	public string bookName;

	// Token: 0x04003145 RID: 12613
	[Header("Settings")]
	public string author;

	// Token: 0x04003146 RID: 12614
	[ReorderableList]
	public List<BookPreset.BookGenre> genre;

	// Token: 0x04003147 RID: 12615
	[Tooltip("Is this part of a series?")]
	public bool isSeries;

	// Token: 0x04003148 RID: 12616
	[EnableIf("isSeries")]
	public BookPreset.BookSeries seriesTag;

	// Token: 0x04003149 RID: 12617
	[EnableIf("isSeries")]
	public int seriesNumber = 1;

	// Token: 0x0400314A RID: 12618
	[Header("Ownership rules")]
	[Tooltip("How common this book is")]
	[Range(0f, 1f)]
	public float common = 0.75f;

	// Token: 0x0400314B RID: 12619
	[Range(0f, 1f)]
	[Tooltip("How likely anyone is to own this...")]
	public float baseChance;

	// Token: 0x0400314C RID: 12620
	[ReorderableList]
	public List<CharacterTrait.TraitPickRule> pickRules = new List<CharacterTrait.TraitPickRule>();

	// Token: 0x0400314D RID: 12621
	[Tooltip("Rules for spawning this (when not on shelf).")]
	public BookPreset.SpawnRules spawnRule;

	// Token: 0x0400314E RID: 12622
	[Header("Visuals")]
	public Mesh bookMesh;

	// Token: 0x0400314F RID: 12623
	public Material bookMaterial;

	// Token: 0x04003150 RID: 12624
	[Header("Text")]
	[Tooltip("Where the text is located in the DDS editor")]
	public string ddsMessage;

	// Token: 0x020006C1 RID: 1729
	public enum BookGenre
	{
		// Token: 0x04003152 RID: 12626
		crime,
		// Token: 0x04003153 RID: 12627
		history,
		// Token: 0x04003154 RID: 12628
		esoteric,
		// Token: 0x04003155 RID: 12629
		romance,
		// Token: 0x04003156 RID: 12630
		medical,
		// Token: 0x04003157 RID: 12631
		science,
		// Token: 0x04003158 RID: 12632
		architecture,
		// Token: 0x04003159 RID: 12633
		sciFi,
		// Token: 0x0400315A RID: 12634
		memoir,
		// Token: 0x0400315B RID: 12635
		propaganda,
		// Token: 0x0400315C RID: 12636
		politics,
		// Token: 0x0400315D RID: 12637
		beauty,
		// Token: 0x0400315E RID: 12638
		food,
		// Token: 0x0400315F RID: 12639
		nature,
		// Token: 0x04003160 RID: 12640
		poetry
	}

	// Token: 0x020006C2 RID: 1730
	public enum BookSeries
	{
		// Token: 0x04003162 RID: 12642
		none,
		// Token: 0x04003163 RID: 12643
		detectiveGill,
		// Token: 0x04003164 RID: 12644
		talesOfTheHeart,
		// Token: 0x04003165 RID: 12645
		candorHistory
	}

	// Token: 0x020006C3 RID: 1731
	public enum SpawnRules
	{
		// Token: 0x04003167 RID: 12647
		onlyAtHome,
		// Token: 0x04003168 RID: 12648
		onlyAtWork,
		// Token: 0x04003169 RID: 12649
		homeOrWork,
		// Token: 0x0400316A RID: 12650
		secret
	}
}
