using System;

// Token: 0x020004A0 RID: 1184
[Serializable]
public class TwitchAudienceData
{
	// Token: 0x040021EE RID: 8686
	public string user_id;

	// Token: 0x040021EF RID: 8687
	public string login;

	// Token: 0x040021F0 RID: 8688
	public string _links;

	// Token: 0x040021F1 RID: 8689
	public int chatter_count;

	// Token: 0x040021F2 RID: 8690
	public Chatters chatters;

	// Token: 0x040021F3 RID: 8691
	public TwitchRootObject followers;

	// Token: 0x040021F4 RID: 8692
	public TwitchRootObject chattersNew;

	// Token: 0x040021F5 RID: 8693
	public TwitchRootObject vipsNew;

	// Token: 0x040021F6 RID: 8694
	public TwitchRootObject moderatorsNew;
}
