using System;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class BlocksCreator : Creator
{
	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000969 RID: 2409 RVA: 0x00092EA8 File Offset: 0x000910A8
	public static BlocksCreator Instance
	{
		get
		{
			return BlocksCreator._instance;
		}
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00092EAF File Offset: 0x000910AF
	private void Awake()
	{
		if (BlocksCreator._instance != null && BlocksCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		BlocksCreator._instance = this;
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x00092EDD File Offset: 0x000910DD
	public override void StartLoading()
	{
		Game.Log("CityGen: Setup city blocks...", 2);
		base.StartCoroutine("Blocks");
	}

	// Token: 0x040009A3 RID: 2467
	public int loadChunk = 10;

	// Token: 0x040009A4 RID: 2468
	private static BlocksCreator _instance;
}
