using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000410 RID: 1040
public class MeshToggleBehaviour : SwitchSyncBehaviour
{
	// Token: 0x060017BE RID: 6078 RVA: 0x00164360 File Offset: 0x00162560
	public override void SetOn(bool val)
	{
		base.SetOn(val);
		if (this.basicBehaviour == SwitchSyncBehaviour.BasicBehaviour.hideWhenOn)
		{
			using (List<MeshRenderer>.Enumerator enumerator = this.objectsToToggle.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MeshRenderer meshRenderer = enumerator.Current;
					meshRenderer.enabled = !this.isOn;
				}
				return;
			}
		}
		if (this.basicBehaviour == SwitchSyncBehaviour.BasicBehaviour.hideWhenOff)
		{
			foreach (MeshRenderer meshRenderer2 in this.objectsToToggle)
			{
				meshRenderer2.enabled = this.isOn;
			}
		}
	}

	// Token: 0x04001D24 RID: 7460
	public List<MeshRenderer> objectsToToggle = new List<MeshRenderer>();
}
