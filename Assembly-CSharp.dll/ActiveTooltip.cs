using System;
using UnityEngine;

// Token: 0x020004CC RID: 1228
public class ActiveTooltip : MonoBehaviour
{
	// Token: 0x06001A7D RID: 6781 RVA: 0x00185CB0 File Offset: 0x00183EB0
	private void Update()
	{
		if (this.ttc != null && !this.ttc.isActiveAndEnabled)
		{
			this.ttc.ForceClose();
			return;
		}
		if (this.ttc == null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0400232C RID: 9004
	public bool setupComplete;

	// Token: 0x0400232D RID: 9005
	public TooltipController ttc;
}
