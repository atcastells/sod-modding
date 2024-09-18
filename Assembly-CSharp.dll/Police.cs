using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class Police : Human
{
	// Token: 0x06000545 RID: 1349 RVA: 0x000516BA File Offset: 0x0004F8BA
	public void Remove()
	{
		Object.Destroy(base.gameObject);
	}
}
