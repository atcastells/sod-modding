using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class WindowViewpointCreator : Creator
{
	// Token: 0x1700005E RID: 94
	// (get) Token: 0x06000A2B RID: 2603 RVA: 0x000977CE File Offset: 0x000959CE
	public static WindowViewpointCreator Instance
	{
		get
		{
			return WindowViewpointCreator._instance;
		}
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x000977D5 File Offset: 0x000959D5
	private void Awake()
	{
		if (WindowViewpointCreator._instance != null && WindowViewpointCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		WindowViewpointCreator._instance = this;
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x00097803 File Offset: 0x00095A03
	public override void StartLoading()
	{
		Game.Log("CityGen: Setting up window viewpoints...", 2);
		base.StartCoroutine("Load");
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0009781C File Offset: 0x00095A1C
	private IEnumerator Load()
	{
		int roomCursor = 0;
		while (roomCursor < CityData.Instance.roomDirectory.Count)
		{
			for (int i = 0; i < this.loadChunk; i++)
			{
				foreach (NewNode newNode in CityData.Instance.roomDirectory[roomCursor].nodes)
				{
					foreach (NewWall newWall in newNode.walls.FindAll((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.window || item.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge))
					{
					}
				}
				int num = roomCursor;
				roomCursor = num + 1;
				if (roomCursor >= CityData.Instance.roomDirectory.Count)
				{
					break;
				}
			}
			CityConstructor.Instance.loadingProgress = Mathf.Min((float)roomCursor / (float)CityData.Instance.roomDirectory.Count, 0.99f) * 0.9f;
			yield return null;
		}
		roomCursor = 0;
		while (roomCursor < CityData.Instance.streetDirectory.Count)
		{
			for (int j = 0; j < this.loadChunk; j++)
			{
				int num = roomCursor;
				roomCursor = num + 1;
				if (roomCursor >= CityData.Instance.streetDirectory.Count)
				{
					break;
				}
			}
			CityConstructor.Instance.loadingProgress = 0.9f + Mathf.Min((float)roomCursor / (float)CityData.Instance.streetDirectory.Count, 0.99f) * 0.1f;
			yield return null;
		}
		base.SetComplete();
		yield break;
	}

	// Token: 0x04000A41 RID: 2625
	public int loadChunk = 2;

	// Token: 0x04000A42 RID: 2626
	private static WindowViewpointCreator _instance;
}
