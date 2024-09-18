using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class PathCreator : Creator
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x060009E2 RID: 2530 RVA: 0x00096604 File Offset: 0x00094804
	public static PathCreator Instance
	{
		get
		{
			return PathCreator._instance;
		}
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x0009660B File Offset: 0x0009480B
	private void Awake()
	{
		if (PathCreator._instance != null && PathCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		PathCreator._instance = this;
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0009663C File Offset: 0x0009483C
	public override void StartLoading()
	{
		bool flag = false;
		foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
		{
			using (List<NewNode>.Enumerator enumerator2 = newAddress.nodes.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.accessToOtherNodes.Count <= 0)
					{
						Game.LogError("Pathfinder: Node with no access found!", 2);
						flag = true;
					}
				}
			}
		}
		if (flag)
		{
			return;
		}
		base.StartCoroutine("GenChunk");
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x000966F0 File Offset: 0x000948F0
	private IEnumerator GenChunk()
	{
		int pathsProgress = 0;
		while (pathsProgress < this.pathsNeededWalking.Count)
		{
			int num = 0;
			while (num < this.loadChunk && pathsProgress < this.pathsNeededWalking.Count)
			{
				if (pathsProgress < this.pathsNeededWalking.Count)
				{
					foreach (NewNode newNode in Enumerable.ElementAt<KeyValuePair<NewNode, List<NewNode>>>(this.pathsNeededWalking, pathsProgress).Value)
					{
					}
				}
				int num2 = pathsProgress;
				pathsProgress = num2 + 1;
				num++;
			}
			CityConstructor.Instance.loadingProgress = Mathf.Min((float)pathsProgress / (float)this.pathsNeededWalking.Count, 0.99f);
			yield return null;
		}
		base.SetComplete();
		yield break;
	}

	// Token: 0x04000A0D RID: 2573
	public int loadChunk = 10;

	// Token: 0x04000A0E RID: 2574
	public Dictionary<NewNode, List<NewNode>> pathsNeededWalking = new Dictionary<NewNode, List<NewNode>>();

	// Token: 0x04000A0F RID: 2575
	private static PathCreator _instance;
}
