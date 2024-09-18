using System;
using UnityEngine;

// Token: 0x020003A3 RID: 931
public class BroadcastDisplayController : MonoBehaviour
{
	// Token: 0x0600152A RID: 5418 RVA: 0x0013479D File Offset: 0x0013299D
	private void Start()
	{
		if (this.rend != null)
		{
			this.rend.sharedMaterial = SessionData.Instance.televisionChannels[0].broadcastMaterialInstanced;
		}
	}

	// Token: 0x04001A27 RID: 6695
	public MeshRenderer rend;
}
