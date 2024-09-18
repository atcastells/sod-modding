using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class RoomsCreator : Creator
{
	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060009F8 RID: 2552 RVA: 0x000969DC File Offset: 0x00094BDC
	public static RoomsCreator Instance
	{
		get
		{
			return RoomsCreator._instance;
		}
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x000969E3 File Offset: 0x00094BE3
	private void Awake()
	{
		if (RoomsCreator._instance != null && RoomsCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		RoomsCreator._instance = this;
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00096A11 File Offset: 0x00094C11
	public override void StartLoading()
	{
		Game.Log("CityGen: Generating interior layouts...", 2);
		base.StartCoroutine("Load");
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x00096A2A File Offset: 0x00094C2A
	private IEnumerator Load()
	{
		int cursor = 0;
		while (cursor < CityData.Instance.addressDirectory.Count)
		{
			int num = 0;
			while (num < this.loadChunk && cursor < CityData.Instance.addressDirectory.Count)
			{
				GenerationController.Instance.GenerateAddressLayout(CityData.Instance.addressDirectory[cursor]);
				int num2 = cursor;
				cursor = num2 + 1;
				num++;
			}
			CityConstructor.Instance.loadingProgress = (float)cursor / (float)CityData.Instance.addressDirectory.Count;
			yield return null;
		}
		GenerationController.Instance.ClearCache();
		base.SetComplete();
		yield break;
	}

	// Token: 0x04000A1A RID: 2586
	public int loadChunk = 1;

	// Token: 0x04000A1B RID: 2587
	private static RoomsCreator _instance;
}
