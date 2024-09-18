using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x02000746 RID: 1862
[CreateAssetMenu(fileName = "helpcontent_data", menuName = "Database/Help Content Page")]
public class HelpContentPage : SoCustomComparison
{
	// Token: 0x040036A9 RID: 13993
	public bool disabled;

	// Token: 0x040036AA RID: 13994
	public string messageID;

	// Token: 0x040036AB RID: 13995
	public List<HelpContentPage.HelpContentDisplay> contentDisplay = new List<HelpContentPage.HelpContentDisplay>();

	// Token: 0x02000747 RID: 1863
	[Serializable]
	public class HelpContentDisplay
	{
		// Token: 0x040036AC RID: 13996
		public HelpContentPage.DisplaySetting helpDisplaySetting;

		// Token: 0x040036AD RID: 13997
		public VideoClip clip;

		// Token: 0x040036AE RID: 13998
		public Texture2D image;
	}

	// Token: 0x02000748 RID: 1864
	public enum DisplaySetting
	{
		// Token: 0x040036B0 RID: 14000
		dontDisplay,
		// Token: 0x040036B1 RID: 14001
		displayBeforeText,
		// Token: 0x040036B2 RID: 14002
		displayAfterText
	}
}
