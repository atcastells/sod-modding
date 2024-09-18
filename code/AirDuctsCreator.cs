using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class AirDuctsCreator : Creator
{
	// Token: 0x17000031 RID: 49
	// (get) Token: 0x0600095E RID: 2398 RVA: 0x00092CCD File Offset: 0x00090ECD
	public static AirDuctsCreator Instance
	{
		get
		{
			return AirDuctsCreator._instance;
		}
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00092CD4 File Offset: 0x00090ED4
	private void Awake()
	{
		if (AirDuctsCreator._instance != null && AirDuctsCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		AirDuctsCreator._instance = this;
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00092D02 File Offset: 0x00090F02
	public override void StartLoading()
	{
		Game.Log("CityGen: Generating building air ducts...", 2);
		base.StartCoroutine("Load");
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00092D1B File Offset: 0x00090F1B
	private IEnumerator Load()
	{
		int cursor = 0;
		while (cursor < HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count)
		{
			int num = 0;
			while (num < this.loadChunk && cursor < HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count)
			{
				if (CityConstructor.Instance.generateNew)
				{
					HighlanderSingleton<CityBuildings>.Instance.buildingDirectory[cursor].GenerateAirDucts();
				}
				else
				{
					foreach (AirDuctGroup airDuctGroup in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory[cursor].airDucts)
					{
						airDuctGroup.LoadDucts();
					}
				}
				int num2 = cursor;
				cursor = num2 + 1;
				num++;
			}
			CityConstructor.Instance.loadingProgress = (float)cursor / (float)HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count;
			yield return null;
		}
		base.SetComplete();
		yield break;
	}

	// Token: 0x0400099D RID: 2461
	public int loadChunk = 1;

	// Token: 0x0400099E RID: 2462
	private static AirDuctsCreator _instance;
}
