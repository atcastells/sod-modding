using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class ObjectToggleBehaviour : SwitchSyncBehaviour
{
	// Token: 0x060017D9 RID: 6105 RVA: 0x001651FC File Offset: 0x001633FC
	public override void SetOn(bool val)
	{
		base.SetOn(val);
		foreach (GameObject gameObject in this.objectsToToggle)
		{
			gameObject.SetActive(val);
		}
	}

	// Token: 0x04001D68 RID: 7528
	[ReorderableList]
	public List<GameObject> objectsToToggle = new List<GameObject>();
}
