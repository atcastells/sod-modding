using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class RoomsLoader : Creator
{
	// Token: 0x17000054 RID: 84
	// (get) Token: 0x06000A03 RID: 2563 RVA: 0x00096B55 File Offset: 0x00094D55
	public static RoomsLoader Instance
	{
		get
		{
			return RoomsLoader._instance;
		}
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00096B5C File Offset: 0x00094D5C
	private void Awake()
	{
		if (RoomsLoader._instance != null && RoomsLoader._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		RoomsLoader._instance = this;
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x00096B8A File Offset: 0x00094D8A
	public override void StartLoading()
	{
		Game.Log("CityGen: Connect rooms...", 2);
		base.StartCoroutine("Load");
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x00096BA3 File Offset: 0x00094DA3
	private IEnumerator Load()
	{
		int cursor = 0;
		using (List<NewGameLocation>.Enumerator enumerator = CityData.Instance.gameLocationDirectory.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NewGameLocation newGameLocation = enumerator.Current;
				newGameLocation.entrances.Clear();
			}
			goto IL_177;
		}
		IL_6F:
		int num = 0;
		while (num < this.connectionChunk && cursor < CityData.Instance.roomDirectory.Count)
		{
			CityData.Instance.roomDirectory[cursor].ConnectNodes();
			foreach (NewNode.NodeAccess nodeAccess in CityData.Instance.roomDirectory[cursor].gameLocation.entrances)
			{
				nodeAccess.fromNode.room.AddEntrance(nodeAccess.fromNode, nodeAccess.toNode, false, NewNode.NodeAccess.AccessType.adjacent, false);
			}
			int num2 = cursor;
			cursor = num2 + 1;
			num++;
		}
		CityConstructor.Instance.loadingProgress = (float)cursor / (float)CityData.Instance.roomDirectory.Count * 0.1f;
		yield return null;
		IL_177:
		if (cursor >= CityData.Instance.roomDirectory.Count)
		{
			if (CityConstructor.Instance.generateNew)
			{
				foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
				{
					newBuilding.CalculateDirectionalCullingTrees();
				}
			}
			cursor = 0;
			int phase2Chunk = this.connectionChunk;
			if (CityConstructor.Instance.generateNew)
			{
				phase2Chunk = this.cullTreeChunk;
			}
			this.threads = new List<RoomsLoader.LoaderThread>();
			while (cursor < CityData.Instance.roomDirectory.Count || this.threads.Count > 0)
			{
				if (CityConstructor.Instance.generateNew)
				{
					if (!Game.Instance.generateCullingInGame)
					{
						if (cursor < CityData.Instance.roomDirectory.Count)
						{
							for (int i = this.threads.Count; i < Game.Instance.maxThreads; i++)
							{
								if (cursor >= CityData.Instance.roomDirectory.Count)
								{
									break;
								}
								RoomsLoader.LoaderThread loaderThread = new RoomsLoader.LoaderThread
								{
									room = CityData.Instance.roomDirectory[cursor]
								};
								loaderThread.thread = base.StartCoroutine(this.ThreadedRoomConnect(loaderThread));
								int num2 = cursor;
								cursor = num2 + 1;
							}
						}
					}
					else
					{
						cursor = CityData.Instance.roomDirectory.Count;
					}
				}
				else
				{
					int num3 = 0;
					while (num3 < phase2Chunk && cursor < CityData.Instance.roomDirectory.Count)
					{
						CityData.Instance.roomDirectory[cursor].LoadCullingTree();
						if (CityData.Instance.roomDirectory[cursor].gameLocation.thisAsAddress != null && !CityData.Instance.roomDirectory[cursor].gameLocation.thisAsAddress.generatedEntranceWeights)
						{
							foreach (NewNode.NodeAccess nodeAccess2 in CityData.Instance.roomDirectory[cursor].gameLocation.entrances)
							{
								nodeAccess2.PreComputeEntranceWeights();
							}
							CityData.Instance.roomDirectory[cursor].gameLocation.thisAsAddress.generatedEntranceWeights = true;
							if (Game.Instance.useJobSystem)
							{
								CityData.Instance.roomDirectory[cursor].gameLocation.thisAsAddress.GenerateJobPathingData();
							}
						}
						int num2 = cursor;
						cursor = num2 + 1;
						num3++;
					}
				}
				CityConstructor.Instance.loadingProgress = (float)cursor / (float)CityData.Instance.roomDirectory.Count * 0.9f + 0.1f;
				yield return null;
			}
			foreach (NewNode.NodeAccess nodeAccess3 in PathFinder.Instance.streetEntrances)
			{
				nodeAccess3.PreComputeEntranceWeights();
			}
			if (CityConstructor.Instance.generateNew)
			{
				foreach (NewBuilding newBuilding2 in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
				{
					newBuilding2.directionalCullingTrees = null;
				}
			}
			if (Game.Instance.useJobSystem)
			{
				PathFinder.Instance.GenerateJobPathingData();
			}
			base.SetComplete();
			yield break;
		}
		goto IL_6F;
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00096BB2 File Offset: 0x00094DB2
	private IEnumerator ThreadedRoomConnect(RoomsLoader.LoaderThread loaderReference)
	{
		loaderReference.isDone = false;
		this.threads.Add(loaderReference);
		Game.Log("CityGen: Room connect threads: " + this.threads.Count.ToString(), 2);
		NewRoom room = loaderReference.room;
		Thread thread = new Thread(delegate()
		{
			room.GenerateCullingTree(false);
			loaderReference.isDone = true;
		});
		thread.Start();
		while (!loaderReference.isDone || thread.IsAlive)
		{
			yield return null;
		}
		this.threads.Remove(loaderReference);
		Game.Log("CityGen: Room connect threads: " + this.threads.Count.ToString(), 2);
		yield break;
	}

	// Token: 0x04000A20 RID: 2592
	public int connectionChunk = 75;

	// Token: 0x04000A21 RID: 2593
	public int cullTreeChunk = 2;

	// Token: 0x04000A22 RID: 2594
	[NonSerialized]
	public List<RoomsLoader.LoaderThread> threads = new List<RoomsLoader.LoaderThread>();

	// Token: 0x04000A23 RID: 2595
	private static RoomsLoader _instance;

	// Token: 0x02000190 RID: 400
	public class LoaderThread
	{
		// Token: 0x04000A24 RID: 2596
		public Coroutine thread;

		// Token: 0x04000A25 RID: 2597
		public NewRoom room;

		// Token: 0x04000A26 RID: 2598
		public bool isDone;
	}
}
