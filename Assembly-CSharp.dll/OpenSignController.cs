using System;
using UnityEngine;

// Token: 0x02000417 RID: 1047
public class OpenSignController : SwitchSyncBehaviour
{
	// Token: 0x060017DB RID: 6107 RVA: 0x00165267 File Offset: 0x00163467
	public override void SetOn(bool val)
	{
		base.SetOn(val);
		if (!this.isOn)
		{
			this.signRend.sharedMaterial = this.onMat;
			return;
		}
		this.signRend.sharedMaterial = this.offMat;
	}

	// Token: 0x04001D69 RID: 7529
	public MeshRenderer signRend;

	// Token: 0x04001D6A RID: 7530
	public Material onMat;

	// Token: 0x04001D6B RID: 7531
	public Material offMat;
}
