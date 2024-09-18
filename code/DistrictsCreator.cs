using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class DistrictsCreator : Creator
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060009AA RID: 2474 RVA: 0x00094A0A File Offset: 0x00092C0A
	public static DistrictsCreator Instance
	{
		get
		{
			return DistrictsCreator._instance;
		}
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00094A11 File Offset: 0x00092C11
	private void Awake()
	{
		if (DistrictsCreator._instance != null && DistrictsCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		DistrictsCreator._instance = this;
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00002265 File Offset: 0x00000465
	public override void StartLoading()
	{
	}

	// Token: 0x040009E5 RID: 2533
	private static DistrictsCreator _instance;

	// Token: 0x02000178 RID: 376
	public class DistrictPlacement
	{
		// Token: 0x040009E6 RID: 2534
		public float score;

		// Token: 0x040009E7 RID: 2535
		public List<CityTile> tiles = new List<CityTile>();

		// Token: 0x040009E8 RID: 2536
		public List<CityTile> innerTiles = new List<CityTile>();

		// Token: 0x040009E9 RID: 2537
		public List<CityTile> edgeTiles = new List<CityTile>();
	}
}
