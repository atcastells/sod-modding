using System;

// Token: 0x020001DF RID: 479
[Serializable]
public class DDSBundle
{
	// Token: 0x04000C09 RID: 3081
	public string displayName;

	// Token: 0x04000C0A RID: 3082
	public string description;

	// Token: 0x04000C0B RID: 3083
	public string languageCode = "English";

	// Token: 0x04000C0C RID: 3084
	[NonSerialized]
	public string path;
}
