using System;
using System.Collections.Generic;

// Token: 0x0200049E RID: 1182
[Serializable]
public class TwitchRootObject
{
	// Token: 0x06001937 RID: 6455 RVA: 0x00174102 File Offset: 0x00172302
	public TwitchRootObject()
	{
		this.data = new List<TwitchUserData>();
	}

	// Token: 0x040021EC RID: 8684
	public List<TwitchUserData> data;
}
