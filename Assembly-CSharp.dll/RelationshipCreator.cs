using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class RelationshipCreator : Creator
{
	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060009ED RID: 2541 RVA: 0x0009686C File Offset: 0x00094A6C
	public static RelationshipCreator Instance
	{
		get
		{
			return RelationshipCreator._instance;
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00096873 File Offset: 0x00094A73
	private void Awake()
	{
		if (RelationshipCreator._instance != null && RelationshipCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		RelationshipCreator._instance = this;
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x000968A1 File Offset: 0x00094AA1
	public override void StartLoading()
	{
		Game.Log("CityGen: Generating relationships...", 2);
		base.StartCoroutine("Relationships");
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x000968BA File Offset: 0x00094ABA
	private IEnumerator Relationships()
	{
		int citizenCursor = 0;
		while (citizenCursor < CityData.Instance.citizenDirectory.Count)
		{
			int num = 0;
			while (num < this.loadChunk && citizenCursor < CityData.Instance.citizenDirectory.Count)
			{
				Citizen citizen = CityData.Instance.citizenDirectory[citizenCursor];
				citizen.GenerateItemFavs();
				citizen.CreateAcquaintances();
				int num2 = citizenCursor;
				citizenCursor = num2 + 1;
				num++;
			}
			CityConstructor.Instance.loadingProgress = (float)citizenCursor / (float)CityData.Instance.citizenDirectory.Count;
			yield return null;
		}
		base.SetComplete();
		yield break;
	}

	// Token: 0x04000A14 RID: 2580
	public int loadChunk = 10;

	// Token: 0x04000A15 RID: 2581
	private static RelationshipCreator _instance;
}
