using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class CalculateNearest : Creator
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000984 RID: 2436 RVA: 0x00093164 File Offset: 0x00091364
	public static CalculateNearest Instance
	{
		get
		{
			return CalculateNearest._instance;
		}
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x0009316B File Offset: 0x0009136B
	private void Awake()
	{
		if (CalculateNearest._instance != null && CalculateNearest._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CalculateNearest._instance = this;
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00093199 File Offset: 0x00091399
	public override void StartLoading()
	{
		base.StartCoroutine("GenChunk");
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x000931A7 File Offset: 0x000913A7
	private IEnumerator GenChunk()
	{
		int cursor = 0;
		while (cursor < CityData.Instance.gameLocationDirectory.Count)
		{
			int num = 0;
			while (num < this.loadChunk && cursor < CityData.Instance.gameLocationDirectory.Count)
			{
				int num2 = cursor;
				cursor = num2 + 1;
				num++;
			}
			CityConstructor.Instance.loadingProgress = (float)cursor / (float)CityData.Instance.gameLocationDirectory.Count;
			yield return null;
		}
		base.SetComplete();
		yield break;
	}

	// Token: 0x040009B3 RID: 2483
	public int loadChunk = 20;

	// Token: 0x040009B4 RID: 2484
	private static CalculateNearest _instance;
}
