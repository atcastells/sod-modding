using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class BuildingCreator : Creator
{
	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000978 RID: 2424 RVA: 0x0009309E File Offset: 0x0009129E
	public static BuildingCreator Instance
	{
		get
		{
			return BuildingCreator._instance;
		}
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x000930A5 File Offset: 0x000912A5
	private void Awake()
	{
		if (BuildingCreator._instance != null && BuildingCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		BuildingCreator._instance = this;
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x000930D3 File Offset: 0x000912D3
	public override void StartLoading()
	{
		Game.Log("CityGen: Populating with buildings...", 2);
		base.StartCoroutine("Load");
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x000930EC File Offset: 0x000912EC
	private IEnumerator Load()
	{
		yield return null;
		yield break;
	}

	// Token: 0x040009AB RID: 2475
	public int loadChunk = 10;

	// Token: 0x040009AC RID: 2476
	private List<BuildingPreset> buildingPresets;

	// Token: 0x040009AD RID: 2477
	private List<BuildingCreator.PickBuilding> selectionList = new List<BuildingCreator.PickBuilding>();

	// Token: 0x040009AE RID: 2478
	private static BuildingCreator _instance;

	// Token: 0x0200016E RID: 366
	public class PickBuilding
	{
		// Token: 0x040009AF RID: 2479
		public BuildingPreset preset;

		// Token: 0x040009B0 RID: 2480
		public float rank;
	}
}
