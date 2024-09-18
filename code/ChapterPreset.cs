using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006D2 RID: 1746
[CreateAssetMenu(fileName = "chapter_data", menuName = "Database/Chapter Preset")]
public class ChapterPreset : SoCustomComparison
{
	// Token: 0x0600250E RID: 9486 RVA: 0x001E4B09 File Offset: 0x001E2D09
	[Button(null, 0)]
	public virtual void SkipToChapterPart()
	{
		ChapterController.Instance.SkipToChapterPart(this.startingPart, true, false);
	}

	// Token: 0x040031ED RID: 12781
	[Header("Settings")]
	[Tooltip("The number of the chapter. This must be exclusive.")]
	public int chapterNumber;

	// Token: 0x040031EE RID: 12782
	[Tooltip("The prefab that contains the logic for this chapter")]
	public GameObject scriptObject;

	// Token: 0x040031EF RID: 12783
	[Tooltip("The chapter script reference")]
	public string dictionary = "chapter.1";

	// Token: 0x040031F0 RID: 12784
	[Tooltip("Ask to enable the tutorial if this chapter is played.")]
	public bool askToEnableTutorial;

	// Token: 0x040031F1 RID: 12785
	[Header("Starting Time")]
	public float startingHour = 2.56f;

	// Token: 0x040031F2 RID: 12786
	public int startingDate = 2;

	// Token: 0x040031F3 RID: 12787
	public int startingMonth;

	// Token: 0x040031F4 RID: 12788
	public int startingYear = 1;

	// Token: 0x040031F5 RID: 12789
	public int yearZeroLeapYearCycle;

	// Token: 0x040031F6 RID: 12790
	public int dayZero;

	// Token: 0x040031F7 RID: 12791
	[Range(0f, 1f)]
	[Header("Starting Weather")]
	public float rainAmount;

	// Token: 0x040031F8 RID: 12792
	[Range(0f, 1f)]
	public float windAmount;

	// Token: 0x040031F9 RID: 12793
	[Range(0f, 1f)]
	public float snowAmount;

	// Token: 0x040031FA RID: 12794
	[Range(0f, 1f)]
	public float fogAmount = 0.75f;

	// Token: 0x040031FB RID: 12795
	[Range(0f, 1f)]
	public float lightningAmount;

	// Token: 0x040031FC RID: 12796
	public float transitionSpeed = 0.1f;

	// Token: 0x040031FD RID: 12797
	[Tooltip("Simulate at fast forward until a certain point (dictated manually)")]
	[Header("Pre-Simulation")]
	public bool usePreSimulation = true;

	// Token: 0x040031FE RID: 12798
	[Tooltip("The minimum amount of time to pre-simulate")]
	[EnableIf("usePreSimulation")]
	public float minimumPreSimLength = 23.5f;

	// Token: 0x040031FF RID: 12799
	[ReorderableList]
	[Header("Bespoke Audio Events")]
	public List<AudioEvent> audioEvents = new List<AudioEvent>();

	// Token: 0x04003200 RID: 12800
	[Header("Bespoke Dialog")]
	[ReorderableList]
	public List<DialogPreset> dialogEvents = new List<DialogPreset>();

	// Token: 0x04003201 RID: 12801
	[Header("Crimes")]
	public List<MurderPreset> crimePool = new List<MurderPreset>();

	// Token: 0x04003202 RID: 12802
	public List<MurderMO> MOPool = new List<MurderMO>();

	// Token: 0x04003203 RID: 12803
	[Header("Chapter Parts")]
	[ReorderableList]
	[Tooltip("Included mostly for reference: You can use the chapter controller to switch between these.")]
	public List<string> partNames = new List<string>();

	// Token: 0x04003204 RID: 12804
	[Tooltip("The part to load when the chapter is loaded. You can use this to skip parts for testing.")]
	public int startingPart;
}
