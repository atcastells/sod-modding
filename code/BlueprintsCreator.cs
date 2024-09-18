using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class BlueprintsCreator : Creator
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600096D RID: 2413 RVA: 0x00092F06 File Offset: 0x00091106
	public static BlueprintsCreator Instance
	{
		get
		{
			return BlueprintsCreator._instance;
		}
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x00092F0D File Offset: 0x0009110D
	private void Awake()
	{
		if (BlueprintsCreator._instance != null && BlueprintsCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		BlueprintsCreator._instance = this;
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x00092F3C File Offset: 0x0009113C
	public override void StartLoading()
	{
		string newID = "Keypad";
		EvidenceCreator.Instance.CreateEvidence("Keypad", newID, null, null, null, null, null, false, null).Compile();
		Game.Log("CityGen: Generating interior blueprints...", 2);
		base.StartCoroutine("Load");
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x00092F82 File Offset: 0x00091182
	private IEnumerator Load()
	{
		int cursor = 0;
		while (cursor < HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count)
		{
			int num = 0;
			while (num < this.loadChunk && cursor < HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count)
			{
				HighlanderSingleton<CityBuildings>.Instance.buildingDirectory[cursor].LoadInterior();
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

	// Token: 0x040009A5 RID: 2469
	public int loadChunk = 1;

	// Token: 0x040009A6 RID: 2470
	private static BlueprintsCreator _instance;
}
