using System;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class DensityCreator : Creator
{
	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060009A7 RID: 2471 RVA: 0x000949CD File Offset: 0x00092BCD
	public static DensityCreator Instance
	{
		get
		{
			return DensityCreator._instance;
		}
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x000949D4 File Offset: 0x00092BD4
	private void Awake()
	{
		if (DensityCreator._instance != null && DensityCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		DensityCreator._instance = this;
	}

	// Token: 0x040009E4 RID: 2532
	private static DensityCreator _instance;
}
