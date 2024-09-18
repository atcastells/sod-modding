using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007A0 RID: 1952
[CreateAssetMenu(fileName = "newspaper_data", menuName = "Database/Newspaper Article")]
public class NewspaperArticle : SoCustomComparison
{
	// Token: 0x04003B17 RID: 15127
	[Header("Debug")]
	public bool disabled;

	// Token: 0x04003B18 RID: 15128
	[Header("Setup")]
	public string ddsReference;

	// Token: 0x04003B19 RID: 15129
	public NewspaperArticle.Category category;

	// Token: 0x04003B1A RID: 15130
	[Tooltip("The next generated newspaper will try to feature one of the following")]
	public List<NewspaperArticle> followupStories = new List<NewspaperArticle>();

	// Token: 0x04003B1B RID: 15131
	public NewspaperArticle.ContextSource context;

	// Token: 0x020007A1 RID: 1953
	public enum Category
	{
		// Token: 0x04003B1D RID: 15133
		general,
		// Token: 0x04003B1E RID: 15134
		murder,
		// Token: 0x04003B1F RID: 15135
		ad,
		// Token: 0x04003B20 RID: 15136
		foreignAffairs,
		// Token: 0x04003B21 RID: 15137
		murderSecond
	}

	// Token: 0x020007A2 RID: 1954
	public enum ContextSource
	{
		// Token: 0x04003B23 RID: 15139
		nothing,
		// Token: 0x04003B24 RID: 15140
		lastMurder,
		// Token: 0x04003B25 RID: 15141
		player,
		// Token: 0x04003B26 RID: 15142
		randomCitizen,
		// Token: 0x04003B27 RID: 15143
		randomCriminal,
		// Token: 0x04003B28 RID: 15144
		randomGroup
	}
}
