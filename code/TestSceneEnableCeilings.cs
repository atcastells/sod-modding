using System;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class TestSceneEnableCeilings : MonoBehaviour
{
	// Token: 0x06000C61 RID: 3169 RVA: 0x000B1337 File Offset: 0x000AF537
	private void Start()
	{
		this.ceilingParent.gameObject.SetActive(true);
	}

	// Token: 0x04000DEE RID: 3566
	public Transform ceilingParent;
}
