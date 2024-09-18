using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class StartingEvidenceCreator : Creator
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000A18 RID: 2584 RVA: 0x00097329 File Offset: 0x00095529
	public static StartingEvidenceCreator Instance
	{
		get
		{
			return StartingEvidenceCreator._instance;
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00097330 File Offset: 0x00095530
	private void Awake()
	{
		if (StartingEvidenceCreator._instance != null && StartingEvidenceCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		StartingEvidenceCreator._instance = this;
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0009735E File Offset: 0x0009555E
	public override void StartLoading()
	{
		this.called = true;
		Game.Log("CityGen: Generating starting evidence...", 2);
		base.StartCoroutine("GenChunk");
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0009737E File Offset: 0x0009557E
	private IEnumerator GenChunk()
	{
		List<Controller> evToCreate = new List<Controller>();
		foreach (StreetController streetController in CityData.Instance.streetDirectory)
		{
			evToCreate.Add(streetController);
		}
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			evToCreate.Add(newBuilding);
		}
		foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
		{
			evToCreate.Add(newAddress);
		}
		foreach (Citizen citizen in CityData.Instance.citizenDirectory)
		{
			evToCreate.Add(citizen);
		}
		int evProgress = 0;
		while (evProgress < evToCreate.Count)
		{
			int num = 0;
			while (num < this.loadChunk && evProgress < evToCreate.Count)
			{
				evToCreate[evProgress].SetupEvidence();
				int num2 = evProgress;
				evProgress = num2 + 1;
				num++;
			}
			CityConstructor.Instance.loadingProgress = (float)evProgress / (float)evToCreate.Count * 0.5f;
			yield return null;
		}
		foreach (Company company in CityData.Instance.companyDirectory)
		{
			company.SetupEvidence();
		}
		foreach (Citizen citizen2 in CityData.Instance.citizenDirectory)
		{
			foreach (Acquaintance acquaintance in citizen2.acquaintances)
			{
				acquaintance.OthersKnowledgeUpdate();
			}
		}
		this.CompileEvidence();
		yield break;
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0009738D File Offset: 0x0009558D
	public void CompileEvidence()
	{
		base.StartCoroutine("Compile");
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0009739B File Offset: 0x0009559B
	private IEnumerator Compile()
	{
		int evProgress = 0;
		while (evProgress < CityConstructor.Instance.evidenceToCompile.Count)
		{
			int num = 0;
			while (num < this.loadChunkCompile && evProgress < CityConstructor.Instance.evidenceToCompile.Count)
			{
				CityConstructor.Instance.evidenceToCompile[evProgress].Compile();
				int num2 = evProgress;
				evProgress = num2 + 1;
				num++;
			}
			CityConstructor.Instance.loadingProgress = 0.5f + (float)evProgress / (float)CityConstructor.Instance.evidenceToCompile.Count * 0.5f;
			yield return null;
		}
		base.SetComplete();
		yield break;
	}

	// Token: 0x04000A34 RID: 2612
	public int loadChunk = 10;

	// Token: 0x04000A35 RID: 2613
	public int loadChunkCompile = 20;

	// Token: 0x04000A36 RID: 2614
	public bool called;

	// Token: 0x04000A37 RID: 2615
	private static StartingEvidenceCreator _instance;
}
