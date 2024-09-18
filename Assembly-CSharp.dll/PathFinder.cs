using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class PathFinder : MonoBehaviour
{
	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000A4D RID: 2637 RVA: 0x00098913 File Offset: 0x00096B13
	public static PathFinder Instance
	{
		get
		{
			return PathFinder._instance;
		}
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0009891A File Offset: 0x00096B1A
	private void Awake()
	{
		if (PathFinder._instance != null && PathFinder._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		PathFinder._instance = this;
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x00098948 File Offset: 0x00096B48
	private void Start()
	{
		this.SetDimensions();
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00098950 File Offset: 0x00096B50
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
		PathFinder._instance = null;
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00098964 File Offset: 0x00096B64
	public void SetDimensions()
	{
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		if (SessionData.Instance.isFloorEdit)
		{
			CityData.Instance.citySize = new Vector2(1f, 1f);
		}
		this.tileSize = new Vector3(CityControls.Instance.cityTileSize.x / (float)CityControls.Instance.tileMultiplier, CityControls.Instance.cityTileSize.y / (float)CityControls.Instance.tileMultiplier, CityControls.Instance.cityTileSize.z / (float)CityControls.Instance.tileMultiplier);
		this.nodeSize = new Vector3(PathFinder.Instance.tileSize.x / (float)CityControls.Instance.nodeMultiplier, PathFinder.Instance.tileSize.y / (float)CityControls.Instance.nodeMultiplier, PathFinder.Instance.tileSize.z);
		this.citySizeReal = new Vector2(CityData.Instance.citySize.x * CityControls.Instance.cityTileSize.x, CityData.Instance.citySize.y * CityControls.Instance.cityTileSize.y);
		this.halfCitySizeReal = this.citySizeReal * 0.5f;
		this.tileCitySize = CityData.Instance.citySize * (float)CityControls.Instance.tileMultiplier;
		this.nodeCitySize = PathFinder.Instance.tileCitySize * (float)CityControls.Instance.nodeMultiplier;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x00098AF0 File Offset: 0x00096CF0
	public void CompilePathFindingMap(bool calculateNewBuildingFacing = true)
	{
		Game.Log("CityGen: Compiling pathfinding map...", 2);
		Game.Log("Pathfinder: Compiling pathfinding map...", 2);
		int num = 0;
		while ((float)num < CityData.Instance.citySize.x)
		{
			int num2 = 0;
			while ((float)num2 < CityData.Instance.citySize.y)
			{
				CityTile currentCityTile = HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles[new Vector2Int(num, num2)];
				CitySaveData.CityTileCitySave cityTileCitySave = null;
				if (!CityConstructor.Instance.generateNew)
				{
					cityTileCitySave = CityConstructor.Instance.currentData.cityTiles.Find((CitySaveData.CityTileCitySave item) => item.cityCoord == currentCityTile.cityCoord);
				}
				Vector2Int vector2Int;
				vector2Int..ctor(Mathf.RoundToInt((float)(num * CityControls.Instance.tileMultiplier)), Mathf.RoundToInt((float)(num2 * CityControls.Instance.tileMultiplier)));
				for (int i = 0; i < CityControls.Instance.tileMultiplier; i++)
				{
					for (int j = 0; j < CityControls.Instance.tileMultiplier; j++)
					{
						if (i == 0 || i == CityControls.Instance.tileMultiplier - 1 || j == 0 || j == CityControls.Instance.tileMultiplier - 1)
						{
							Vector3Int currentTileCoord = new Vector3Int(vector2Int.x + i, vector2Int.y + j, 0);
							if (currentTileCoord.x > 5 && (float)currentTileCoord.x < CityData.Instance.citySize.x * (float)CityControls.Instance.tileMultiplier - 6f && currentTileCoord.y > 5 && (float)currentTileCoord.y < CityData.Instance.citySize.y * (float)CityControls.Instance.tileMultiplier - 6f)
							{
								NewTile newTile = new NewTile();
								NewTile newTile2 = newTile;
								string text = "Pathfinder Created Tile ";
								Vector3Int vector3Int = currentTileCoord;
								newTile2.name = text + vector3Int.ToString();
								if (!CityConstructor.Instance.generateNew)
								{
									CitySaveData.TileCitySave data = cityTileCitySave.outsideTiles.Find((CitySaveData.TileCitySave item) => item.globalTileCoord == currentTileCoord);
									newTile.LoadPathfindTileData(data);
								}
								else
								{
									newTile.globalTileCoord = currentTileCoord;
								}
								newTile.SetAsOutside(true);
								newTile.cityTile = currentCityTile;
								newTile.isEdge = true;
								this.tileMap.Add(currentTileCoord, newTile);
							}
						}
					}
				}
				num2++;
			}
			num++;
		}
		if (CityConstructor.Instance.generateNew)
		{
			this.CreateStreetChunks();
			if (calculateNewBuildingFacing)
			{
				foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
				{
					newBuilding.CalculateFacing();
				}
			}
			using (Dictionary<Vector3, NewTile>.Enumerator enumerator2 = this.tileMap.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<Vector3, NewTile> keyValuePair = enumerator2.Current;
					if (!keyValuePair.Value.isSetup)
					{
						if (keyValuePair.Value.streetController == null)
						{
							string[] array = new string[6];
							array[0] = "CityGen: Tile ";
							int num3 = 1;
							Vector3Int vector3Int = keyValuePair.Value.globalTileCoord;
							array[num3] = vector3Int.ToString();
							array[2] = " has no street controller assigned! Street count: ";
							array[3] = CityData.Instance.streetDirectory.Count.ToString();
							array[4] = " street chunk";
							int num4 = 5;
							PathFinder.StreetChunk streetChunk = keyValuePair.Value.streetChunk;
							array[num4] = ((streetChunk != null) ? streetChunk.ToString() : null);
							Game.LogError(string.Concat(array), 2);
						}
						else
						{
							keyValuePair.Value.SetupExterior(keyValuePair.Value.cityTile, keyValuePair.Value.globalTileCoord);
							keyValuePair.Value.parent = keyValuePair.Value.streetController.transform;
						}
					}
				}
				goto IL_490;
			}
		}
		foreach (CitySaveData.StreetCitySave data2 in CityConstructor.Instance.currentData.streets)
		{
			StreetController component = Object.Instantiate<GameObject>(PrefabControls.Instance.street, PrefabControls.Instance.cityContainer.transform).GetComponent<StreetController>();
			component.Load(data2);
			if (!CityData.Instance.streetDirectory.Contains(component))
			{
				CityData.Instance.streetDirectory.Add(component);
			}
		}
		IL_490:
		foreach (StreetController streetController in CityData.Instance.streetDirectory)
		{
			streetController.LoadSections();
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0009902C File Offset: 0x0009722C
	public void CreateStreetChunks()
	{
		List<Vector3> list = new List<Vector3>(96);
		List<Vector3> list2 = new List<Vector3>(96);
		int num = Mathf.RoundToInt(CityData.Instance.citySize.x * (float)CityControls.Instance.tileMultiplier);
		int num2 = Mathf.RoundToInt(CityData.Instance.citySize.y * (float)CityControls.Instance.tileMultiplier);
		int num3 = 0;
		int num4 = 0;
		HashSet<Vector3> hashSet = new HashSet<Vector3>(96);
		HashSet<Vector3> hashSet2 = new HashSet<Vector3>(96);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				Vector3 vector;
				vector..ctor((float)i, (float)j, 0f);
				NewTile newTile = null;
				if (this.tileMap.TryGetValue(vector, ref newTile))
				{
					if ((num3 == 0 || num3 == 6) && (num4 == 0 || num4 == 6))
					{
						list.Add(newTile.globalTileCoord);
					}
					else
					{
						list2.Add(newTile.globalTileCoord);
					}
					hashSet.Add(newTile.globalTileCoord);
					if (Game.Instance.displayStreetChunks)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.streetChunkDebug, PrefabControls.Instance.mapContainer);
						gameObject.transform.position = CityData.Instance.TileToRealpos(newTile.globalTileCoord);
						string[] array = new string[7];
						array[0] = "Street Area ";
						array[1] = num3.ToString();
						array[2] = ",";
						array[3] = num4.ToString();
						array[4] = "(";
						int num5 = 5;
						Vector3Int globalTileCoord = newTile.globalTileCoord;
						array[num5] = globalTileCoord.ToString();
						array[6] = ")";
						gameObject.name = string.Concat(array);
					}
				}
				num4++;
				if (num4 >= CityControls.Instance.tileMultiplier)
				{
					num4 = 0;
				}
			}
			num3++;
			if (num3 >= CityControls.Instance.tileMultiplier)
			{
				num3 = 0;
			}
		}
		while (hashSet.Count > 0)
		{
			Vector3 vector2 = Enumerable.First<Vector3>(hashSet);
			hashSet2.Add(vector2);
			HashSet<Vector3> hashSet3 = new HashSet<Vector3>(96);
			HashSet<Vector3> hashSet4 = new HashSet<Vector3>(96);
			HashSet<Vector3> hashSet5 = new HashSet<Vector3>(96);
			hashSet5.Add(vector2);
			hashSet3.Add(vector2);
			bool flag = false;
			if (list.Contains(vector2))
			{
				flag = true;
			}
			while (hashSet3.Count > 0)
			{
				Vector3 vector3 = Enumerable.First<Vector3>(hashSet3);
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
				for (int k = 0; k < offsetArrayX.Length; k++)
				{
					Vector2 vector4 = offsetArrayX[k];
					Vector3 vector5 = vector3 + new Vector3(vector4.x, vector4.y, 0f);
					if (!hashSet2.Contains(vector5) && !hashSet4.Contains(vector5) && !hashSet3.Contains(vector5))
					{
						if (flag && list.Contains(vector5))
						{
							hashSet3.Add(vector5);
							hashSet5.Add(vector5);
							if (!hashSet2.Contains(vector5))
							{
								hashSet2.Add(vector5);
							}
							if (hashSet.Contains(vector5))
							{
								hashSet.Remove(vector5);
							}
						}
						else if (!flag && list2.Contains(vector5))
						{
							hashSet3.Add(vector5);
							hashSet5.Add(vector5);
							if (!hashSet2.Contains(vector5))
							{
								hashSet2.Add(vector5);
							}
							if (hashSet.Contains(vector5))
							{
								hashSet.Remove(vector5);
							}
						}
					}
					if (!hashSet4.Contains(vector3))
					{
						hashSet4.Add(vector3);
					}
				}
				if (!hashSet4.Contains(vector3))
				{
					hashSet4.Add(vector3);
				}
				hashSet3.Remove(vector3);
				if (!hashSet2.Contains(vector3))
				{
					hashSet2.Add(vector3);
				}
				if (hashSet.Contains(vector3))
				{
					hashSet.Remove(vector3);
				}
			}
			if (!hashSet2.Contains(vector2))
			{
				hashSet2.Add(vector2);
			}
			hashSet.Remove(vector2);
			this.streetChunks.Add(new PathFinder.StreetChunk(vector2, Enumerable.ToList<Vector3>(hashSet5), flag));
		}
		this.FootTrafficSimulation();
		if (Game.Instance.displayStreetAndJunctionChunks)
		{
			foreach (PathFinder.StreetChunk streetChunk in this.streetChunks)
			{
				if (streetChunk.isJunction)
				{
					GameObject gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.junctionChunkDebug, PrefabControls.Instance.mapContainer);
					gameObject2.transform.position = CityData.Instance.TileToRealpos(streetChunk.anchorTile);
					Object @object = gameObject2;
					string text = "Junction ";
					Vector3 anchorTile = streetChunk.anchorTile;
					@object.name = text + anchorTile.ToString();
					if (Game.Instance.displayTrafficSimulationResults)
					{
						MeshRenderer componentInChildren = gameObject2.GetComponentInChildren<MeshRenderer>();
						componentInChildren.material = Object.Instantiate<Material>(Game.Instance.debugtrafficSimMaterial);
						componentInChildren.material.SetColor("_BaseColor", Color.Lerp(Color.white, Color.red, streetChunk.footfallNormalized));
					}
				}
				else
				{
					foreach (Vector3 coords in streetChunk.allCoords)
					{
						GameObject gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.streetAreaChunkDebug, PrefabControls.Instance.mapContainer);
						gameObject2.transform.position = CityData.Instance.TileToRealpos(coords);
						Object object2 = gameObject2;
						string text2 = "Street Area ";
						Vector3 anchorTile = streetChunk.anchorTile;
						object2.name = text2 + anchorTile.ToString();
						if (Game.Instance.displayTrafficSimulationResults)
						{
							MeshRenderer componentInChildren2 = gameObject2.GetComponentInChildren<MeshRenderer>();
							componentInChildren2.material = Object.Instantiate<Material>(Game.Instance.debugtrafficSimMaterial);
							componentInChildren2.material.SetColor("_BaseColor", Color.Lerp(Color.white, Color.red, streetChunk.footfallNormalized));
						}
					}
				}
			}
		}
		this.CreateStreets();
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x00099670 File Offset: 0x00097870
	private void FootTrafficSimulation()
	{
		int num = Mathf.CeilToInt(CityData.Instance.citySize.x * CityData.Instance.citySize.y);
		List<Vector3> list = new List<Vector3>(96);
		List<Vector3> list2 = new List<Vector3>(96);
		Vector3 vector;
		vector..ctor(CityData.Instance.citySize.x * (float)CityControls.Instance.tileMultiplier * 0.5f, CityData.Instance.citySize.y * (float)CityControls.Instance.tileMultiplier * 0.5f, 0f);
		foreach (KeyValuePair<Vector3, NewTile> keyValuePair in this.tileMap)
		{
			list2.Add(keyValuePair.Key);
			float num2 = Vector3.Distance(keyValuePair.Key, vector);
			float num3 = Mathf.InverseLerp(0f, Mathf.Max(CityData.Instance.citySize.x * 0.5f, CityData.Instance.citySize.y * 0.5f), num2);
			int num4 = Mathf.Max(Mathf.CeilToInt((1f - num3) * 10f), 1);
			for (int i = 0; i < num4; i++)
			{
				list.Add(keyValuePair.Key);
			}
		}
		Dictionary<Vector3, int> dictionary = new Dictionary<Vector3, int>(96);
		string seed = CityData.Instance.seed;
		for (int j = 0; j < num; j++)
		{
			Vector3 vector2 = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, seed, out seed)];
			Vector3 vector3 = list2[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list2.Count, seed, out seed)];
			foreach (NewTile newTile in this.GetTileRoute(this.tileMap[vector2], this.tileMap[vector3], null))
			{
				if (!dictionary.ContainsKey(newTile.globalTileCoord))
				{
					dictionary.Add(newTile.globalTileCoord, 0);
				}
				Dictionary<Vector3, int> dictionary2 = dictionary;
				Vector3 vector4 = newTile.globalTileCoord;
				int num5 = dictionary2[vector4];
				dictionary2[vector4] = num5 + 1;
			}
		}
		int num6 = 0;
		foreach (PathFinder.StreetChunk streetChunk in this.streetChunks)
		{
			streetChunk.footfall = 0;
			foreach (Vector3 vector5 in streetChunk.allCoords)
			{
				if (dictionary.ContainsKey(vector5))
				{
					streetChunk.footfall += dictionary[vector5];
				}
			}
			num6 = Mathf.Max(num6, streetChunk.footfall);
		}
		foreach (PathFinder.StreetChunk streetChunk2 in this.streetChunks)
		{
			streetChunk2.footfallNormalized = (float)streetChunk2.footfall / (float)num6;
		}
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x000999E8 File Offset: 0x00097BE8
	private void CreateStreets()
	{
		int num = 4;
		string seed = CityData.Instance.seed;
		List<PathFinder.StreetChunk> list = new List<PathFinder.StreetChunk>(this.streetChunks);
		list.Sort((PathFinder.StreetChunk p1, PathFinder.StreetChunk p2) => p2.footfall.CompareTo(p1.footfall));
		while (list.Count > 0)
		{
			PathFinder.StreetChunk streetChunk = list[0];
			List<PathFinder.StreetChunk> list2 = new List<PathFinder.StreetChunk>(96);
			list2.Add(streetChunk);
			int num2 = 1;
			StreetController streetController = this.NewRoad(this.tileMap[streetChunk.anchorTile].cityTile.district);
			while (list2.Count > 0)
			{
				PathFinder.StreetChunk streetChunk2 = list2[0];
				foreach (NewTile newTile in streetChunk2.allTiles)
				{
					streetController.AddTile(newTile);
				}
				streetController.AddChunk(streetChunk2);
				streetController.normalizedFootfall = Mathf.Max(streetController.normalizedFootfall, streetChunk2.footfallNormalized);
				streetController.chunkSize++;
				list.Remove(streetChunk2);
				if (num2 < num)
				{
					Vector2 vector;
					vector..ctor(9999f, -9999f);
					Vector2 vector2;
					vector2..ctor(9999f, -9999f);
					foreach (NewTile newTile2 in streetController.tiles)
					{
						vector = new Vector2(Mathf.Min(vector.x, (float)newTile2.globalTileCoord.x), Mathf.Max(vector.x, (float)newTile2.globalTileCoord.x));
						vector2 = new Vector2(Mathf.Min(vector2.y, (float)newTile2.globalTileCoord.y), Mathf.Max(vector2.y, (float)newTile2.globalTileCoord.y));
					}
					bool horizontal = Mathf.Abs(vector2.y - vector2.x) <= Mathf.Abs(vector.y - vector.x);
					Dictionary<PathFinder.StreetChunk, bool> adjacentChunks = streetChunk2.GetAdjacentChunks(horizontal);
					PathFinder.StreetChunk streetChunk3 = null;
					float num3 = -1f;
					foreach (KeyValuePair<PathFinder.StreetChunk, bool> keyValuePair in adjacentChunks)
					{
						if (keyValuePair.Key != streetChunk2)
						{
							float num4 = 0f;
							if (keyValuePair.Value)
							{
								num4 = 0.2f;
							}
							else if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seed, out seed) > 0.5f)
							{
								continue;
							}
							if (list.Contains(keyValuePair.Key) && !list2.Contains(keyValuePair.Key) && keyValuePair.Key.footfallNormalized + num4 > num3)
							{
								streetChunk3 = keyValuePair.Key;
								num3 = (float)keyValuePair.Key.footfall;
							}
						}
					}
					if (streetChunk3 != null)
					{
						list2.Add(streetChunk3);
						num2++;
					}
				}
				list2.RemoveAt(0);
			}
		}
		Dictionary<StreetController, List<NewTile>> dictionary = new Dictionary<StreetController, List<NewTile>>(96);
		foreach (StreetController streetController2 in CityData.Instance.streetDirectory)
		{
			if (streetController2.chunkSize <= 1)
			{
				if (streetController2.tiles.Count > 6 && (streetController2.normalizedFootfall < 0.6f || Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seed, out seed) > 0.66f))
				{
					List<NewTile> list3 = new List<NewTile>(96);
					List<NewTile> list4 = new List<NewTile>(96);
					List<NewTile> list5 = new List<NewTile>(96);
					bool flag = true;
					using (List<NewTile>.Enumerator enumerator = streetController2.tiles.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							NewTile t = enumerator.Current;
							if (!t.streetChunk.isJunction)
							{
								if (!list4.Exists((NewTile item) => item.globalTileCoord.x == t.globalTileCoord.x))
								{
									list4.Add(t);
								}
								else if (!list5.Exists((NewTile item) => item.globalTileCoord.y == t.globalTileCoord.y))
								{
									list5.Add(t);
								}
							}
							if (t.cityTile.building != null && !t.cityTile.building.preset.enableAlleywayWalls)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						if (list4.Count > list5.Count && list4.Count > 2)
						{
							NewTile newTile3 = list4[Toolbox.Instance.GetPsuedoRandomNumberContained(1, list4.Count - 1, seed, out seed)];
							using (List<NewTile>.Enumerator enumerator = streetController2.tiles.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									NewTile newTile4 = enumerator.Current;
									if (newTile4.globalTileCoord.x >= newTile3.globalTileCoord.x)
									{
										list3.Add(newTile4);
									}
								}
								goto IL_5A1;
							}
							goto IL_522;
						}
						goto IL_522;
						IL_5A1:
						if (list3.Count > 0)
						{
							dictionary.Add(streetController2, list3);
						}
						streetController2.SetAsAlley();
						continue;
						IL_522:
						if (list5.Count > 2)
						{
							NewTile newTile5 = list5[Toolbox.Instance.GetPsuedoRandomNumberContained(1, list5.Count - 1, seed, out seed)];
							foreach (NewTile newTile6 in streetController2.tiles)
							{
								if (newTile6.globalTileCoord.y >= newTile5.globalTileCoord.y)
								{
									list3.Add(newTile6);
								}
							}
							goto IL_5A1;
						}
						goto IL_5A1;
					}
					else
					{
						streetController2.SetAsBackstreet();
					}
				}
				else
				{
					streetController2.SetAsBackstreet();
				}
			}
			else if (streetController2.normalizedFootfall >= 0.5f)
			{
				streetController2.SetAsStreet();
			}
			else
			{
				streetController2.SetAsBackstreet();
			}
		}
		foreach (StreetController streetController3 in CityData.Instance.streetDirectory)
		{
			streetController3.LoadStreetTiles();
		}
		using (Dictionary<StreetController, List<NewTile>>.Enumerator enumerator4 = dictionary.GetEnumerator())
		{
			while (enumerator4.MoveNext())
			{
				KeyValuePair<StreetController, List<NewTile>> pair = enumerator4.Current;
				if (pair.Value.Count > 0 && pair.Key.tiles.Count != pair.Value.Count && pair.Key.tiles.Count > 6)
				{
					bool flag2 = true;
					NewTile origin = pair.Value[0];
					List<NewTile> list6 = pair.Key.tiles.FindAll((NewTile item) => !pair.Value.Contains(item));
					foreach (StreetController streetController4 in CityData.Instance.streetDirectory)
					{
						if (!(streetController4 == pair.Key) && streetController4.tiles.Count > 0)
						{
							NewTile destination = streetController4.tiles[0];
							if (list6 != null && list6.Count > 0)
							{
								List<NewTile> tileRoute = this.GetTileRoute(origin, destination, list6);
								if (tileRoute == null || tileRoute.Count <= 0)
								{
									flag2 = false;
									break;
								}
								tileRoute = this.GetTileRoute(list6[0], destination, pair.Value);
								if (tileRoute == null || tileRoute.Count <= 0)
								{
									flag2 = false;
									break;
								}
							}
						}
					}
					if (flag2)
					{
						StreetController streetController5 = this.NewRoad(pair.Value[0].cityTile.district);
						streetController5.sharedGroundElements.Add(pair.Key);
						pair.Key.sharedGroundElements.Add(streetController5);
						foreach (NewTile newTile7 in pair.Value)
						{
							pair.Key.RemoveTile(newTile7);
							streetController5.AddTile(newTile7);
						}
						streetController5.SetAsAlley();
					}
				}
			}
		}
		if (Game.Instance.displayStreets)
		{
			foreach (StreetController streetController6 in CityData.Instance.streetDirectory)
			{
				foreach (NewTile newTile8 in streetController6.tiles)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.streetNodeDebug, PrefabControls.Instance.mapContainer);
					gameObject.transform.position = CityData.Instance.TileToRealpos(newTile8.globalTileCoord);
					gameObject.name = string.Concat(new string[]
					{
						"Street ",
						streetController6.streetID.ToString(),
						" (",
						streetController6.normalizedFootfall.ToString(),
						")"
					});
					MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
					componentInChildren.material = Object.Instantiate<Material>(Game.Instance.debugStreetMaterial);
					componentInChildren.material.SetColor("_BaseColor", Game.Instance.streetDebugColours[streetController6.streetID % Game.Instance.streetDebugColours.Count]);
				}
			}
		}
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0009A524 File Offset: 0x00098724
	private StreetController NewRoad(DistrictController dis)
	{
		StreetController component = Object.Instantiate<GameObject>(PrefabControls.Instance.street, PrefabControls.Instance.cityContainer.transform).GetComponent<StreetController>();
		component.Setup(dis);
		if (!CityData.Instance.streetDirectory.Contains(component))
		{
			CityData.Instance.streetDirectory.Add(component);
		}
		return component;
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0009A580 File Offset: 0x00098780
	public PathFinder.PathData GetPath(NewNode origin, NewNode destination, Human human, NewNode[] avoidNodes = null)
	{
		this.totalPathCalls++;
		if (origin == null)
		{
			Game.LogError("Pathfinder: Origin is null!", 2);
			return null;
		}
		if (destination == null)
		{
			Game.LogError("Pathfinder: destination is null", 2);
			return null;
		}
		if (origin.isInaccessable || origin.noAccess)
		{
			List<NewNode> list = new List<NewNode>(96);
			List<NewNode> list2 = new List<NewNode>(96);
			int num = 99;
			bool flag = false;
			list.Add(origin);
			while (list.Count > 0 && num > 0)
			{
				NewNode newNode = list[0];
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
				for (int n = 0; n < offsetArrayX.Length; n++)
				{
					Vector2 vector = offsetArrayX[n];
					Vector3 vector2;
					vector2..ctor((float)newNode.nodeCoord.x + vector.x, (float)newNode.nodeCoord.y + vector.y, (float)newNode.nodeCoord.z);
					NewNode newNode2 = null;
					if (this.nodeMap.TryGetValue(vector2, ref newNode2))
					{
						if (!newNode2.isInaccessable)
						{
							origin = newNode2;
							flag = true;
							break;
						}
						if (!list.Contains(newNode2) && !list2.Contains(newNode2))
						{
							list.Add(newNode2);
						}
					}
				}
				if (flag)
				{
					break;
				}
				list2.Add(newNode);
				list.RemoveAt(0);
				num--;
			}
		}
		if (destination.isInaccessable || destination.noAccess)
		{
			List<NewNode> list3 = new List<NewNode>(96);
			List<NewNode> list4 = new List<NewNode>(96);
			int num2 = 99;
			bool flag2 = false;
			list3.Add(origin);
			while (list3.Count > 0 && num2 > 0)
			{
				NewNode newNode3 = list3[0];
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
				for (int n = 0; n < offsetArrayX.Length; n++)
				{
					Vector2 vector3 = offsetArrayX[n];
					Vector3 vector4;
					vector4..ctor((float)newNode3.nodeCoord.x + vector3.x, (float)newNode3.nodeCoord.y + vector3.y, (float)newNode3.nodeCoord.z);
					NewNode newNode4 = null;
					if (this.nodeMap.TryGetValue(vector4, ref newNode4))
					{
						if (!newNode4.isInaccessable)
						{
							destination = newNode4;
							flag2 = true;
							break;
						}
						if (!list3.Contains(newNode4) && !list4.Contains(newNode4))
						{
							list3.Add(newNode4);
						}
					}
				}
				if (flag2)
				{
					break;
				}
				list4.Add(newNode3);
				list3.RemoveAt(0);
				num2--;
			}
		}
		List<NewNode.NodeAccess> list5 = new List<NewNode.NodeAccess>(96);
		if (origin.gameLocation != destination.gameLocation)
		{
			list5 = this.GetGameLocationRoute(origin, destination);
		}
		if (list5 == null)
		{
			if (human.ai != null)
			{
				try
				{
					string[] array = new string[13];
					array[0] = "Location route is null for ";
					array[1] = human.name;
					array[2] = " on action ";
					array[3] = human.ai.currentAction.preset.name;
					array[4] = ": ";
					array[5] = human.ai.currentAction.node.gameLocation.name;
					array[6] = ", ";
					array[7] = human.ai.currentAction.node.room.name;
					array[8] = ", ";
					int num3 = 9;
					Vector3Int nodeCoord = human.ai.currentAction.node.nodeCoord;
					array[num3] = nodeCoord.ToString();
					array[10] = " (";
					int num4 = 11;
					Vector3 vector5 = human.ai.currentAction.node.position;
					array[num4] = vector5.ToString();
					array[12] = ")";
					Game.LogError(string.Concat(array), 2);
				}
				catch
				{
				}
			}
			return null;
		}
		List<NewNode.NodeAccess> list6 = new List<NewNode.NodeAccess>(96);
		NewNode newNode5 = origin;
		if (Game.Instance.useJobSystem)
		{
			NativeList<JobHandle> nativeList = new NativeList<JobHandle>(2);
			List<PathFinder.GetInternalRouteJob> list7 = new List<PathFinder.GetInternalRouteJob>(96);
			Dictionary<int, List<NewNode.NodeAccess>> dictionary = new Dictionary<int, List<NewNode.NodeAccess>>(96);
			for (int j = 0; j < list5.Count + 1; j++)
			{
				NewNode newNode6 = null;
				if (j >= 0 && j < list5.Count)
				{
					newNode6 = list5[j].GetOtherGameLocation(newNode5);
				}
				if (j == list5.Count)
				{
					if (newNode5 == destination)
					{
						break;
					}
					newNode6 = destination;
				}
				if (newNode5 != newNode6)
				{
					NewAddress.PathKey pathKey = new NewAddress.PathKey(newNode5, newNode6);
					bool flag3 = false;
					if (Game.Instance.useInternalRouteCaching && SessionData.Instance.startedGame)
					{
						List<NewNode.NodeAccess> list8 = null;
						if (newNode5.gameLocation.thisAsAddress != null)
						{
							if (newNode5.gameLocation.thisAsAddress.internalRoutes.TryGetValue(pathKey, ref list8))
							{
								dictionary.Add(j, list8);
								flag3 = true;
							}
						}
						else if (PathFinder.Instance.internalRoutes.TryGetValue(pathKey, ref list8))
						{
							dictionary.Add(j, list8);
							flag3 = true;
						}
					}
					if (!flag3)
					{
						if (newNode5.gameLocation.thisAsAddress != null)
						{
							PathFinder.GetInternalRouteJob getInternalRouteJob = new PathFinder.GetInternalRouteJob
							{
								origin = Toolbox.Instance.ToFloat3(pathKey.origin.nodeCoord),
								destination = Toolbox.Instance.ToFloat3(pathKey.destination.nodeCoord),
								output = new NativeList<int>(3),
								listIndex = j,
								accessRef = newNode5.gameLocation.thisAsAddress.accessRef,
								accessPositions = newNode5.gameLocation.thisAsAddress.accessPositions,
								toNodeReference = newNode5.gameLocation.thisAsAddress.toNodeReference,
								noPassRef = newNode5.gameLocation.thisAsAddress.noPassRef
							};
							PathFinder.GetInternalRouteJob getInternalRouteJob2 = getInternalRouteJob;
							list7.Add(getInternalRouteJob2);
							JobHandle jobHandle = IJobExtensions.Schedule<PathFinder.GetInternalRouteJob>(getInternalRouteJob2, default(JobHandle));
							nativeList.Add(ref jobHandle);
						}
						else if (!Game.Instance.forceStreetPathsOnMainThread)
						{
							PathFinder.GetInternalRouteJob getInternalRouteJob = new PathFinder.GetInternalRouteJob
							{
								origin = Toolbox.Instance.ToFloat3(pathKey.origin.nodeCoord),
								destination = Toolbox.Instance.ToFloat3(pathKey.destination.nodeCoord),
								output = new NativeList<int>(3),
								listIndex = j,
								accessRef = this.streetAccessRef,
								accessPositions = this.streetAccessPositions,
								toNodeReference = this.streetToNodeReference,
								noPassRef = this.streetNoPassRef
							};
							PathFinder.GetInternalRouteJob getInternalRouteJob3 = getInternalRouteJob;
							list7.Add(getInternalRouteJob3);
							JobHandle jobHandle2 = IJobExtensions.Schedule<PathFinder.GetInternalRouteJob>(getInternalRouteJob3, default(JobHandle));
							nativeList.Add(ref jobHandle2);
						}
					}
					newNode5 = newNode6;
				}
			}
			if (newNode5 != destination)
			{
				Game.LogError("PathFinder: Current node != desitnation. This shouldn't be happeneing.", 2);
			}
			JobHandle.CompleteAll(nativeList);
			newNode5 = origin;
			int n;
			int i;
			for (i = 0; i < list5.Count + 1; i = n + 1)
			{
				NewNode newNode7 = null;
				if (i >= 0 && i < list5.Count)
				{
					newNode7 = list5[i].GetOtherGameLocation(newNode5);
				}
				if (i == list5.Count)
				{
					if (newNode5 == destination)
					{
						break;
					}
					newNode7 = destination;
				}
				if (newNode5 != newNode7)
				{
					int num5 = list7.FindIndex((PathFinder.GetInternalRouteJob item) => item.listIndex == i);
					if (num5 > -1)
					{
						PathFinder.GetInternalRouteJob getInternalRouteJob = list7[num5];
						if (getInternalRouteJob.output.Length <= 0)
						{
							Vector3 vector5;
							for (int k = 0; k < list5.Count; k++)
							{
								string text = string.Empty;
								if (k == i)
								{
									text = "*";
								}
								string text2 = "Pathfinder: ";
								string text3 = text;
								vector5 = list5[k].worldAccessPoint;
								Game.Log(text2 + text3 + vector5.ToString(), 2);
							}
							string[] array2 = new string[8];
							array2[0] = "Pathfinder: Internal pathfinding ";
							int num6 = 1;
							getInternalRouteJob = list7[num5];
							array2[num6] = getInternalRouteJob.listIndex.ToString();
							array2[2] = ": ";
							int num7 = 3;
							vector5 = this.nodeMap[list7[num5].origin].position;
							array2[num7] = vector5.ToString();
							array2[4] = "->";
							int num8 = 5;
							vector5 = this.nodeMap[list7[num5].destination].position;
							array2[num8] = vector5.ToString();
							array2[6] = " @ ";
							array2[7] = newNode5.gameLocation.name;
							Game.LogError(string.Concat(array2), 2);
						}
						List<NewNode.NodeAccess> list9 = new List<NewNode.NodeAccess>(96);
						int num9 = 0;
						for (;;)
						{
							int num10 = num9;
							getInternalRouteJob = list7[num5];
							if (num10 >= getInternalRouteJob.output.Length)
							{
								break;
							}
							getInternalRouteJob = list7[num5];
							int num11 = getInternalRouteJob.output[num9];
							list9.Add(this.nodeAccessReference[num11]);
							num9++;
						}
						list6.AddRange(list9);
						if (Game.Instance.useInternalRouteCaching && SessionData.Instance.startedGame)
						{
							NewAddress.PathKey pathKey2 = new NewAddress.PathKey(newNode5, newNode7);
							if (newNode5.gameLocation.thisAsAddress != null)
							{
								if (Game.Instance.unlimitedPathCaching || newNode5.gameLocation.thisAsAddress.internalRoutes.Count < Game.Instance.maxInternalCachedPaths)
								{
									newNode5.gameLocation.thisAsAddress.internalRoutes.Add(pathKey2, list9);
								}
							}
							else if (newNode5.isIndoorsEntrance && newNode7.isIndoorsEntrance && (Game.Instance.unlimitedPathCaching || this.internalRoutes.Count < Game.Instance.maxStreetCachedPaths))
							{
								this.internalRoutes.Add(pathKey2, list9);
							}
						}
						PathFinder.Instance.calculatedInternalRoutes++;
						getInternalRouteJob = list7[num5];
						getInternalRouteJob.output.Dispose();
					}
					else if (Game.Instance.forceStreetPathsOnMainThread && newNode5.gameLocation.thisAsStreet != null)
					{
						List<NewNode.NodeAccess> internalRoute = this.GetInternalRoute(new NewAddress.PathKey(newNode5, newNode7), newNode5.gameLocation);
						if (internalRoute != null)
						{
							list6.AddRange(internalRoute);
						}
					}
					else
					{
						List<NewNode.NodeAccess> list10 = null;
						if (dictionary.TryGetValue(i, ref list10))
						{
							list6.AddRange(list10);
						}
					}
					newNode5 = newNode7;
				}
				n = i;
			}
			nativeList.Dispose();
		}
		else
		{
			for (int l = 0; l <= list5.Count; l++)
			{
				NewNode newNode8 = null;
				if (l >= 0 && l < list5.Count)
				{
					newNode8 = list5[l].GetOtherGameLocation(newNode5);
				}
				if (l == list5.Count)
				{
					if (newNode5 == destination)
					{
						break;
					}
					newNode8 = destination;
				}
				if (newNode5 != newNode8)
				{
					if (Game.Instance.debugPathfinding)
					{
						bool flag4 = false;
						if (newNode5.noAccess)
						{
							Game.LogError("Pathfinder: Current node is not accessable at index " + l.ToString() + "/" + list5.Count.ToString(), 2);
							flag4 = true;
						}
						if (newNode8.noAccess)
						{
							Game.LogError("Pathfinder: Next node is not accessable at index " + l.ToString() + "/" + list5.Count.ToString(), 2);
							flag4 = true;
						}
						if (flag4)
						{
							for (int m = 0; m < list5.Count; m++)
							{
								GameObject gameObject = GameObject.CreatePrimitive(3);
								gameObject.name = "Location route " + l.ToString() + "/" + list5.Count.ToString();
								gameObject.transform.SetParent(PrefabControls.Instance.pathfindDebugParent);
								gameObject.transform.position = list5[m].worldAccessPoint;
							}
						}
					}
					List<NewNode.NodeAccess> internalRoute2 = this.GetInternalRoute(new NewAddress.PathKey(newNode5, newNode8), newNode5.gameLocation);
					if (internalRoute2 != null)
					{
						list6.AddRange(internalRoute2);
						newNode5 = newNode8;
					}
				}
			}
			if (newNode5 != destination)
			{
				Game.Log("Path Error: Current node != desitnation. This shouldn't be happeneing.", 2);
			}
		}
		return new PathFinder.PathData
		{
			accessList = list6
		};
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0009B194 File Offset: 0x00099394
	private List<NewNode.NodeAccess> GetGameLocationRoute(NewNode origin, NewNode destination)
	{
		PathFinder.GameLocationPathKey gameLocationPathKey = new PathFinder.GameLocationPathKey(origin.gameLocation, destination.gameLocation);
		List<NewNode.NodeAccess> list = null;
		if (Game.Instance.useExternalRouteCaching)
		{
			if (this.gameLocationRoutes.TryGetValue(gameLocationPathKey, ref list))
			{
				this.returnedCachedRoomRoutes++;
				return list;
			}
			PathFinder.GameLocationPathKey gameLocationPathKey2 = new PathFinder.GameLocationPathKey(destination.gameLocation, origin.gameLocation);
			if (this.gameLocationRoutes.TryGetValue(gameLocationPathKey2, ref list))
			{
				list = new List<NewNode.NodeAccess>(list);
				list.Reverse();
				this.returnedCachedRoomRoutes++;
				return list;
			}
		}
		Dictionary<NewNode.NodeAccess, bool> dictionary = new Dictionary<NewNode.NodeAccess, bool>(96);
		Dictionary<NewNode.NodeAccess, bool> dictionary2 = new Dictionary<NewNode.NodeAccess, bool>(96);
		Dictionary<NewNode.NodeAccess, NewNode.NodeAccess> dictionary3 = new Dictionary<NewNode.NodeAccess, NewNode.NodeAccess>(96);
		Dictionary<NewNode.NodeAccess, float> dictionary4 = new Dictionary<NewNode.NodeAccess, float>(96);
		Dictionary<NewNode.NodeAccess, float> dictionary5 = new Dictionary<NewNode.NodeAccess, float>(96);
		List<NewNode.NodeAccess> entrances;
		if (gameLocationPathKey.originLocation.thisAsStreet != null)
		{
			if (gameLocationPathKey.destinationLocation.thisAsStreet != null)
			{
				entrances = gameLocationPathKey.originLocation.entrances;
			}
			else
			{
				entrances = this.streetEntrances;
			}
		}
		else
		{
			entrances = gameLocationPathKey.originLocation.entrances;
		}
		Dictionary<NewGameLocation, List<DebugPathfind.DebugLocationLink>> dictionary6 = null;
		if (Game.Instance.debugPathfinding)
		{
			dictionary6 = new Dictionary<NewGameLocation, List<DebugPathfind.DebugLocationLink>>(96);
		}
		foreach (NewNode.NodeAccess nodeAccess in entrances)
		{
			if (nodeAccess.toNode.noAccess)
			{
				if (Game.Instance.debugPathfinding)
				{
					if (!dictionary6.ContainsKey(gameLocationPathKey.originLocation))
					{
						dictionary6.Add(gameLocationPathKey.originLocation, new List<DebugPathfind.DebugLocationLink>(96));
					}
					dictionary6[gameLocationPathKey.originLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess, "Fail: No access"));
				}
			}
			else if (!nodeAccess.walkingAccess)
			{
				if (Game.Instance.debugPathfinding)
				{
					if (!dictionary6.ContainsKey(gameLocationPathKey.originLocation))
					{
						dictionary6.Add(gameLocationPathKey.originLocation, new List<DebugPathfind.DebugLocationLink>(96));
					}
					dictionary6[gameLocationPathKey.originLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess, "Fail: No walking access"));
				}
			}
			else if (nodeAccess.employeeDoor)
			{
				if (Game.Instance.debugPathfinding)
				{
					if (!dictionary6.ContainsKey(gameLocationPathKey.originLocation))
					{
						dictionary6.Add(gameLocationPathKey.originLocation, new List<DebugPathfind.DebugLocationLink>(96));
					}
					dictionary6[gameLocationPathKey.originLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess, "Fail: Employee door"));
				}
			}
			else if (nodeAccess.toNode.noPassThrough && nodeAccess.toNode != destination)
			{
				if (Game.Instance.debugPathfinding)
				{
					if (!dictionary6.ContainsKey(gameLocationPathKey.originLocation))
					{
						dictionary6.Add(gameLocationPathKey.originLocation, new List<DebugPathfind.DebugLocationLink>(96));
					}
					dictionary6[gameLocationPathKey.originLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess, "Fail: No Pass-Through"));
				}
			}
			else if (nodeAccess.toNode.gameLocation.thisAsAddress != null && nodeAccess.toNode.gameLocation != gameLocationPathKey.originLocation && nodeAccess.toNode.gameLocation != gameLocationPathKey.destinationLocation && nodeAccess.toNode.gameLocation.thisAsAddress.addressPreset != null && !nodeAccess.toNode.gameLocation.thisAsAddress.addressPreset.canPassThrough)
			{
				if (Game.Instance.debugPathfinding)
				{
					if (!dictionary6.ContainsKey(gameLocationPathKey.originLocation))
					{
						dictionary6.Add(gameLocationPathKey.originLocation, new List<DebugPathfind.DebugLocationLink>(96));
					}
					dictionary6[gameLocationPathKey.originLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess, "Fail: No Pass-Through"));
				}
			}
			else
			{
				if (Game.Instance.debugPathfinding)
				{
					if (!dictionary6.ContainsKey(gameLocationPathKey.originLocation))
					{
						dictionary6.Add(gameLocationPathKey.originLocation, new List<DebugPathfind.DebugLocationLink>(96));
					}
					dictionary6[gameLocationPathKey.originLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess, "Success"));
				}
				dictionary2.Add(nodeAccess, true);
				float num = Vector3.Distance(origin.position, nodeAccess.worldAccessPoint);
				if (!dictionary4.ContainsKey(nodeAccess))
				{
					dictionary4.Add(nodeAccess, num);
				}
				float num2 = Vector3.Distance(nodeAccess.worldAccessPoint, destination.position);
				if (!dictionary5.ContainsKey(nodeAccess))
				{
					dictionary5.Add(nodeAccess, num2);
				}
			}
		}
		int num3 = 0;
		bool flag = false;
		bool flag2 = false;
		while (dictionary2.Count > 0)
		{
			NewNode.NodeAccess nodeAccess2 = null;
			float num4 = float.PositiveInfinity;
			foreach (KeyValuePair<NewNode.NodeAccess, bool> keyValuePair in dictionary2)
			{
				if (dictionary5.ContainsKey(keyValuePair.Key) && dictionary5[keyValuePair.Key] < num4)
				{
					num4 = dictionary5[keyValuePair.Key];
					nodeAccess2 = keyValuePair.Key;
				}
			}
			if (nodeAccess2.toNode.gameLocation == gameLocationPathKey.destinationLocation && nodeAccess2.walkingAccess && !nodeAccess2.employeeDoor && !nodeAccess2.toNode.noAccess && (!nodeAccess2.toNode.noPassThrough || nodeAccess2.toNode == destination))
			{
				List<NewNode.NodeAccess> list2 = new List<NewNode.NodeAccess>();
				list2.Add(nodeAccess2);
				while (dictionary3.ContainsKey(nodeAccess2))
				{
					if (!nodeAccess2.walkingAccess)
					{
						string text = "Pathfinder: Returned route contains unwalkable access! ";
						Vector3 vector = nodeAccess2.worldAccessPoint;
						Game.LogError(text + vector.ToString() + " path index " + list2.Count.ToString(), 2);
					}
					else if (nodeAccess2.employeeDoor)
					{
						string text2 = "Pathfinder: Returned route contains employee door! ";
						Vector3 vector = nodeAccess2.worldAccessPoint;
						Game.LogError(text2 + vector.ToString() + " path index " + list2.Count.ToString(), 2);
					}
					NewNode.NodeAccess nodeAccess3 = nodeAccess2;
					nodeAccess2 = dictionary3[nodeAccess2];
					if (nodeAccess2 == null)
					{
						Game.Log("Pathfinder: Room pathfinder path construction error: current is null, invalid path", 2);
						break;
					}
					if (nodeAccess2 != nodeAccess3)
					{
						list2.Add(nodeAccess2);
					}
				}
				list2.Reverse();
				if (Game.Instance.useExternalRouteCaching && SessionData.Instance.startedGame && (Game.Instance.unlimitedPathCaching || this.gameLocationRoutes.Count < Game.Instance.maxExternalCachedPaths))
				{
					this.gameLocationRoutes.Add(gameLocationPathKey, list2);
				}
				this.calculatedRoomRoutes++;
				if (Game.Instance.debugPathfinding)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						NewNode.NodeAccess nodeAccess4 = list2[i];
						if (!nodeAccess4.walkingAccess || nodeAccess4.employeeDoor)
						{
							string text3 = "Pathfinder: Faulty route pathfinder! No walking access/employee door ";
							Vector3 vector = nodeAccess4.worldAccessPoint;
							Game.LogError(text3 + vector.ToString() + " path index " + i.ToString(), 2);
						}
						else if (nodeAccess4.toNode.noAccess)
						{
							string text4 = "Pathfinder: Faulty route pathfinder! To node no access ";
							Vector3 vector = nodeAccess4.worldAccessPoint;
							Game.LogError(text4 + vector.ToString() + " path index " + i.ToString(), 2);
						}
						else if (nodeAccess4.toNode.noPassThrough && nodeAccess4.toNode != destination)
						{
							string text5 = "Pathfinder: Faulty route pathfinder! To node no pass through ";
							Vector3 vector = nodeAccess4.worldAccessPoint;
							Game.LogError(text5 + vector.ToString() + " path index " + i.ToString(), 2);
						}
					}
				}
				flag = true;
				return list2;
			}
			if (!flag2 && destination.building != null && nodeAccess2.fromNode.gameLocation.thisAsStreet != null)
			{
				flag2 = true;
			}
			dictionary2.Remove(nodeAccess2);
			if (!dictionary.ContainsKey(nodeAccess2))
			{
				dictionary.Add(nodeAccess2, true);
			}
			if (nodeAccess2.oppositeAccess != null && !dictionary.ContainsKey(nodeAccess2.oppositeAccess))
			{
				dictionary.Add(nodeAccess2.oppositeAccess, true);
			}
			List<NewNode.NodeAccess> entrances2;
			if (nodeAccess2.toNode.gameLocation.thisAsStreet != null)
			{
				if (gameLocationPathKey.destinationLocation.thisAsStreet != null)
				{
					entrances2 = nodeAccess2.toNode.gameLocation.entrances;
				}
				else
				{
					entrances2 = this.streetEntrances;
				}
			}
			else
			{
				entrances2 = nodeAccess2.toNode.gameLocation.entrances;
			}
			foreach (NewNode.NodeAccess nodeAccess5 in entrances2)
			{
				if (!nodeAccess5.walkingAccess)
				{
					if (Game.Instance.debugPathfinding)
					{
						if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
						{
							dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
						}
						dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: No Walking Access"));
					}
				}
				else if (nodeAccess5.toNode.noAccess)
				{
					if (Game.Instance.debugPathfinding)
					{
						if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
						{
							dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
						}
						dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: No Access"));
					}
				}
				else if (nodeAccess5.employeeDoor)
				{
					if (Game.Instance.debugPathfinding)
					{
						if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
						{
							dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
						}
						dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: Employee Door"));
					}
				}
				else if (nodeAccess5.toNode.noPassThrough && nodeAccess5.toNode != destination)
				{
					if (Game.Instance.debugPathfinding)
					{
						if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
						{
							dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
						}
						dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: No Pass-Through"));
					}
				}
				else
				{
					bool flag3 = false;
					if (dictionary.TryGetValue(nodeAccess5, ref flag3))
					{
						if (Game.Instance.debugPathfinding)
						{
							if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
							{
								dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
							}
							dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: Already Scanned"));
						}
					}
					else if (dictionary2.TryGetValue(nodeAccess5, ref flag3))
					{
						if (Game.Instance.debugPathfinding)
						{
							if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
							{
								dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
							}
							dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: Already In Line For Scanning"));
						}
					}
					else
					{
						if (flag2 && destination.building != null)
						{
							if (nodeAccess5.toNode.gameLocation.thisAsAddress != null && nodeAccess5.toNode.building != destination.building)
							{
								if (Game.Instance.debugPathfinding)
								{
									if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
									{
										dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
									}
									dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: Skip Other Buildings"));
									continue;
								}
								continue;
							}
						}
						else if (nodeAccess2.toNode.building != null && (destination.building == null || destination.building != nodeAccess2.toNode.building) && nodeAccess5.toNode.floor != null && nodeAccess5.toNode.floor.buildingEntrances.Count <= 0 && nodeAccess5.toNode.floor != gameLocationPathKey.originLocation.floor && !nodeAccess5.toNode.tile.isStairwell && !nodeAccess5.toNode.tile.isInvertedStairwell)
						{
							if (Game.Instance.debugPathfinding)
							{
								if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
								{
									dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
								}
								dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: This Floor Has No Building Exit"));
								continue;
							}
							continue;
						}
						if (nodeAccess5.toNode.gameLocation.thisAsAddress != null && nodeAccess5.toNode.gameLocation != gameLocationPathKey.originLocation && nodeAccess5.toNode.gameLocation != gameLocationPathKey.destinationLocation && nodeAccess5.toNode.gameLocation.thisAsAddress.addressPreset != null && !nodeAccess5.toNode.gameLocation.thisAsAddress.addressPreset.canPassThrough)
						{
							if (Game.Instance.debugPathfinding)
							{
								if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
								{
									dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>(96));
								}
								dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Fail: No Pass-Through Allowed @ Neighbor Address"));
							}
						}
						else
						{
							if (Game.Instance.debugPathfinding)
							{
								if (!dictionary6.ContainsKey(nodeAccess2.toNode.gameLocation))
								{
									dictionary6.Add(nodeAccess2.toNode.gameLocation, new List<DebugPathfind.DebugLocationLink>());
								}
								dictionary6[nodeAccess2.toNode.gameLocation].Add(new DebugPathfind.DebugLocationLink(nodeAccess5, "Success"));
							}
							dictionary2.Add(nodeAccess5, true);
							float num5 = 0f;
							float num6;
							if (nodeAccess2.entranceWeights.TryGetValue(nodeAccess5, ref num5))
							{
								num6 = dictionary4[nodeAccess2] + num5;
							}
							else
							{
								float num7 = Vector3.Distance(nodeAccess2.worldAccessPoint, nodeAccess5.worldAccessPoint);
								nodeAccess2.entranceWeights[nodeAccess2] = num7;
								num6 = dictionary4[nodeAccess2] + num7;
							}
							if (!dictionary4.ContainsKey(nodeAccess5) || num6 < dictionary4[nodeAccess5])
							{
								if (!dictionary3.ContainsKey(nodeAccess5))
								{
									dictionary3.Add(nodeAccess5, nodeAccess2);
								}
								else
								{
									dictionary3[nodeAccess5] = nodeAccess2;
								}
								if (!dictionary4.ContainsKey(nodeAccess5))
								{
									dictionary4.Add(nodeAccess5, num6);
								}
								else
								{
									dictionary4[nodeAccess5] = num6;
								}
								float num8 = dictionary4[nodeAccess5] + Vector3.Distance(nodeAccess5.worldAccessPoint, destination.position);
								if (!dictionary5.ContainsKey(nodeAccess5))
								{
									dictionary5.Add(nodeAccess5, num8);
								}
								else
								{
									dictionary5[nodeAccess5] = num8;
								}
							}
						}
					}
				}
			}
			num3++;
			if (num3 > 9999)
			{
				Game.LogError("Pathfinder: GameLocation pathfinding failsafe reached, check for cascades! Dumping open set...", 2);
				foreach (KeyValuePair<NewNode.NodeAccess, bool> keyValuePair2 in dictionary2)
				{
					Game.Log(string.Concat(new string[]
					{
						"Pathfinder: ",
						keyValuePair2.Key.fromNode.name,
						"(",
						keyValuePair2.Key.fromNode.room.roomID.ToString(),
						") -> ",
						keyValuePair2.Key.toNode.name,
						"(",
						keyValuePair2.Key.toNode.room.roomID.ToString(),
						")"
					}), 2);
				}
				return null;
			}
		}
		if (!flag)
		{
			Game.LogError("Pathfinder: GameLocation pathfinding unsucessful- there is an unreachable route with " + dictionary.Count.ToString() + " closed set count, information below...", 2);
			if (Game.Instance.debugPathfinding)
			{
				Game.Log("Pathfinder: Outputting debug information for this error only...", 2);
				foreach (KeyValuePair<NewNode.NodeAccess, bool> keyValuePair3 in dictionary)
				{
					DebugPathfind component = Object.Instantiate<GameObject>(PrefabControls.Instance.pathfindRoomDebug, PrefabControls.Instance.pathfindDebugParent).GetComponent<DebugPathfind>();
					List<DebugPathfind.DebugLocationLink> linkList = null;
					dictionary6.TryGetValue(keyValuePair3.Key.toNode.gameLocation, ref linkList);
					component.Setup(keyValuePair3.Key, keyValuePair3.Key.toNode.room, linkList);
				}
				Game.Instance.debugPathfinding = false;
			}
			string[] array = new string[11];
			array[0] = "Pathfinder: from ";
			array[1] = gameLocationPathKey.originLocation.name;
			array[2] = ", ";
			array[3] = origin.room.name;
			array[4] = ", ";
			array[5] = origin.name;
			array[6] = ", ";
			int num9 = 7;
			Vector3Int nodeCoord = origin.nodeCoord;
			array[num9] = nodeCoord.ToString();
			array[8] = " (";
			int num10 = 9;
			Vector3 vector = origin.position;
			array[num10] = vector.ToString();
			array[10] = ")";
			Game.Log(string.Concat(array), 2);
			string[] array2 = new string[11];
			array2[0] = "Pathfinder: to ";
			array2[1] = gameLocationPathKey.destinationLocation.name;
			array2[2] = ", ";
			array2[3] = destination.room.name;
			array2[4] = ", ";
			array2[5] = destination.name;
			array2[6] = ", ";
			int num11 = 7;
			nodeCoord = destination.nodeCoord;
			array2[num11] = nodeCoord.ToString();
			array2[8] = " (";
			int num12 = 9;
			vector = destination.position;
			array2[num12] = vector.ToString();
			array2[10] = ")";
			Game.Log(string.Concat(array2), 2);
		}
		return null;
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0009C4B0 File Offset: 0x0009A6B0
	public List<NewNode.NodeAccess> GetInternalRoute(NewAddress.PathKey pathKey, NewGameLocation gameLocation)
	{
		List<NewNode.NodeAccess> result = null;
		if (Game.Instance.useInternalRouteCaching)
		{
			if (gameLocation.thisAsAddress != null)
			{
				if (gameLocation.thisAsAddress.internalRoutes.TryGetValue(pathKey, ref result))
				{
					return result;
				}
			}
			else if (this.internalRoutes.TryGetValue(pathKey, ref result))
			{
				return result;
			}
		}
		HashSet<NewNode.NodeAccess> hashSet = new HashSet<NewNode.NodeAccess>(96);
		HashSet<NewNode.NodeAccess> hashSet2 = new HashSet<NewNode.NodeAccess>(96);
		HashSet<NewNode> hashSet3 = new HashSet<NewNode>(96);
		hashSet3.Add(pathKey.origin);
		Dictionary<NewNode.NodeAccess, NewNode.NodeAccess> dictionary = new Dictionary<NewNode.NodeAccess, NewNode.NodeAccess>(96);
		Dictionary<NewNode.NodeAccess, float> dictionary2 = new Dictionary<NewNode.NodeAccess, float>(96);
		Dictionary<NewNode.NodeAccess, float> dictionary3 = new Dictionary<NewNode.NodeAccess, float>(96);
		foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in pathKey.origin.accessToOtherNodes)
		{
			if (keyValuePair.Value.walkingAccess && !keyValuePair.Value.employeeDoor && (!keyValuePair.Value.toNode.noPassThrough || keyValuePair.Value.toNode == pathKey.destination) && (((!(gameLocation.thisAsAddress == null) || !(keyValuePair.Key.gameLocation.thisAsAddress != null)) && (!(gameLocation.thisAsAddress != null) || !(keyValuePair.Key.gameLocation != gameLocation))) || !(keyValuePair.Key.gameLocation != pathKey.origin.gameLocation) || !(keyValuePair.Key.gameLocation != pathKey.destination.gameLocation)))
			{
				hashSet2.Add(keyValuePair.Value);
				if (!dictionary2.ContainsKey(keyValuePair.Value))
				{
					dictionary2.Add(keyValuePair.Value, 0f);
				}
				if (!dictionary3.ContainsKey(keyValuePair.Value))
				{
					dictionary3.Add(keyValuePair.Value, Vector3.Distance(keyValuePair.Value.worldAccessPoint, pathKey.destination.position));
				}
			}
		}
		int num = 0;
		bool flag = false;
		while (hashSet2.Count > 0)
		{
			NewNode.NodeAccess nodeAccess = null;
			float num2 = float.PositiveInfinity;
			foreach (NewNode.NodeAccess nodeAccess2 in hashSet2)
			{
				if (dictionary3.ContainsKey(nodeAccess2) && dictionary3[nodeAccess2] < num2)
				{
					num2 = dictionary3[nodeAccess2];
					nodeAccess = nodeAccess2;
				}
			}
			if (nodeAccess.toNode == pathKey.destination && nodeAccess.walkingAccess && !nodeAccess.toNode.noAccess && !nodeAccess.employeeDoor)
			{
				List<NewNode.NodeAccess> list = new List<NewNode.NodeAccess>(96);
				list.Add(nodeAccess);
				while (dictionary.ContainsKey(nodeAccess))
				{
					NewNode.NodeAccess nodeAccess3 = nodeAccess;
					nodeAccess = dictionary[nodeAccess];
					if (nodeAccess == null)
					{
						Game.Log("PathFinder: Room pathfinder path construction error: current is null, invalid path", 2);
						break;
					}
					if (nodeAccess != nodeAccess3)
					{
						list.Add(nodeAccess);
					}
				}
				list.Reverse();
				if (Game.Instance.useInternalRouteCaching && SessionData.Instance.startedGame)
				{
					if (gameLocation.thisAsAddress != null)
					{
						if (Game.Instance.unlimitedPathCaching || gameLocation.thisAsAddress.internalRoutes.Count < Game.Instance.maxInternalCachedPaths)
						{
							gameLocation.thisAsAddress.internalRoutes.Add(pathKey, list);
						}
					}
					else if (pathKey.origin.isIndoorsEntrance && pathKey.destination.isIndoorsEntrance && (Game.Instance.unlimitedPathCaching || this.internalRoutes.Count < Game.Instance.maxStreetCachedPaths))
					{
						this.internalRoutes.Add(pathKey, list);
					}
				}
				PathFinder.Instance.calculatedInternalRoutes++;
				flag = true;
				return list;
			}
			if (Game.Instance.debugPathfinding && nodeAccess.toNode == pathKey.destination)
			{
				Game.LogError(string.Concat(new string[]
				{
					"Pathfinder: Final destination rejected! ",
					nodeAccess.walkingAccess.ToString(),
					" ",
					nodeAccess.toNode.noAccess.ToString(),
					" ",
					nodeAccess.employeeDoor.ToString()
				}), 2);
			}
			hashSet2.Remove(nodeAccess);
			hashSet.Add(nodeAccess);
			foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair2 in nodeAccess.toNode.accessToOtherNodes)
			{
				if (keyValuePair2.Value.walkingAccess && !keyValuePair2.Value.employeeDoor && !keyValuePair2.Value.toNode.noAccess && (!keyValuePair2.Value.toNode.noPassThrough || keyValuePair2.Value.toNode == pathKey.destination) && (((!(gameLocation.thisAsAddress == null) || !(keyValuePair2.Key.gameLocation.thisAsAddress != null)) && (!(gameLocation.thisAsAddress != null) || !(keyValuePair2.Key.gameLocation != gameLocation))) || !(keyValuePair2.Key.gameLocation != pathKey.origin.gameLocation) || !(keyValuePair2.Key.gameLocation != pathKey.destination.gameLocation)))
				{
					NewNode.NodeAccess value = keyValuePair2.Value;
					if (!hashSet.Contains(value) && !hashSet2.Contains(value) && !hashSet3.Contains(value.toNode))
					{
						float num3 = dictionary2[nodeAccess] + keyValuePair2.Value.weight;
						hashSet2.Add(value);
						hashSet3.Add(value.toNode);
						if (!dictionary2.ContainsKey(value) || num3 < dictionary2[value])
						{
							if (!dictionary.ContainsKey(value))
							{
								dictionary.Add(value, nodeAccess);
							}
							else
							{
								dictionary[value] = nodeAccess;
							}
							if (!dictionary2.ContainsKey(value))
							{
								dictionary2.Add(value, num3);
							}
							else
							{
								dictionary2[value] = num3;
							}
							float num4 = dictionary2[value] + Vector3.Distance(value.worldAccessPoint, pathKey.destination.position);
							if (!dictionary3.ContainsKey(value))
							{
								dictionary3.Add(value, num4);
							}
							else
							{
								dictionary3[value] = num4;
							}
						}
					}
				}
			}
			num++;
			if (num > 9999)
			{
				Game.LogError("PathFinder: Internal pathfinding failsafe reached, check for cascades! Dumping open set...", 2);
				foreach (NewNode.NodeAccess nodeAccess4 in hashSet2)
				{
					Game.Log(string.Concat(new string[]
					{
						"Pathfinder: ",
						nodeAccess4.fromNode.name,
						"(",
						nodeAccess4.fromNode.room.roomID.ToString(),
						") -> ",
						nodeAccess4.toNode.name,
						"(",
						nodeAccess4.toNode.room.roomID.ToString(),
						")"
					}), 2);
				}
				return null;
			}
		}
		if (!flag)
		{
			if (Game.Instance.debugPathfinding)
			{
				string[] array = new string[7];
				array[0] = "PathFinder: ";
				array[1] = gameLocation.name;
				array[2] = " Internal pathfinding unsucessful- there is an unreachable route from: ";
				int num5 = 3;
				Vector3 position = pathKey.origin.position;
				array[num5] = position.ToString();
				array[4] = " to ";
				int num6 = 5;
				position = pathKey.destination.position;
				array[num6] = position.ToString();
				array[6] = ". Dumping closed set...";
				Game.LogError(string.Concat(array), 2);
				Game.Log("Pathfinder: Outputting debug information for this error only...", 2);
				GameObject gameObject = GameObject.CreatePrimitive(0);
				gameObject.name = "Origin, noAccess: " + pathKey.origin.noAccess.ToString() + " Access count: " + pathKey.origin.accessToOtherNodes.Count.ToString();
				gameObject.transform.SetParent(PrefabControls.Instance.pathfindDebugParent);
				gameObject.transform.position = pathKey.origin.position;
				foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair3 in pathKey.origin.accessToOtherNodes)
				{
					Object.Instantiate<GameObject>(PrefabControls.Instance.pathfindInternalDebug, gameObject.transform).GetComponent<DebugPathfind>().Setup(keyValuePair3.Value, keyValuePair3.Value.fromNode.room, null);
				}
				GameObject gameObject2 = GameObject.CreatePrimitive(0);
				gameObject2.name = "Destination, noAccess: " + pathKey.destination.noAccess.ToString() + " Access count: " + pathKey.destination.accessToOtherNodes.Count.ToString();
				gameObject2.transform.SetParent(PrefabControls.Instance.pathfindDebugParent);
				gameObject2.transform.position = pathKey.destination.position;
				foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair4 in pathKey.destination.accessToOtherNodes)
				{
					Object.Instantiate<GameObject>(PrefabControls.Instance.pathfindInternalDebug, gameObject2.transform).GetComponent<DebugPathfind>().Setup(keyValuePair4.Value, keyValuePair4.Value.fromNode.room, null);
				}
				foreach (NewNode.NodeAccess nodeAccess5 in hashSet)
				{
					GameObject gameObject3 = Object.Instantiate<GameObject>(PrefabControls.Instance.pathfindNodeDebug, PrefabControls.Instance.pathfindDebugParent);
					gameObject3.GetComponent<DebugPathfind>().Setup(nodeAccess5, nodeAccess5.fromNode.room, null);
					if (nodeAccess5.toNode == pathKey.destination)
					{
						GameObject gameObject4 = gameObject3;
						gameObject4.name += " DESTINATION CONNECTION";
					}
					if (nodeAccess5.fromNode == pathKey.origin)
					{
						GameObject gameObject5 = gameObject3;
						gameObject5.name += " ORIGIN CONNECTION";
					}
				}
				Game.Instance.debugPathfinding = false;
			}
			else
			{
				string[] array2 = new string[7];
				array2[0] = "Path Error: ";
				array2[1] = gameLocation.name;
				array2[2] = " Internal pathfinding unsucessful- there is an unreachable route from: ";
				int num7 = 3;
				Vector3 position = pathKey.origin.position;
				array2[num7] = position.ToString();
				array2[4] = " to ";
				int num8 = 5;
				position = pathKey.destination.position;
				array2[num8] = position.ToString();
				array2[6] = ".";
				Game.Log(string.Concat(array2), 2);
			}
		}
		return null;
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0009D064 File Offset: 0x0009B264
	public void GenerateJobPathingData()
	{
		this.streetAccessRef = new NativeMultiHashMap<float3, int>(0, 4);
		this.streetAccessPositions = new NativeHashMap<int, float3>(0, 4);
		this.streetToNodeReference = new NativeHashMap<int, float3>(0, 4);
		this.streetNoPassRef = new NativeList<float3>(0, 4);
		foreach (StreetController streetController in CityData.Instance.streetDirectory)
		{
			foreach (NewNode newNode in streetController.nodes)
			{
				if (newNode.noPassThrough)
				{
					float3 @float = Toolbox.Instance.ToFloat3(newNode.nodeCoord);
					this.streetNoPassRef.Add(ref @float);
				}
				foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode.accessToOtherNodes)
				{
					if (keyValuePair.Value.walkingAccess && !keyValuePair.Value.employeeDoor && !keyValuePair.Value.toNode.noAccess && !keyValuePair.Value.fromNode.noAccess)
					{
						this.streetAccessRef.Add(Toolbox.Instance.ToFloat3(newNode.nodeCoord), keyValuePair.Value.id);
						this.streetAccessPositions.TryAdd(keyValuePair.Value.id, keyValuePair.Value.worldAccessPoint);
						this.streetToNodeReference.TryAdd(keyValuePair.Value.id, Toolbox.Instance.ToFloat3(keyValuePair.Value.toNode.nodeCoord));
					}
				}
			}
		}
		foreach (NewNode.NodeAccess nodeAccess in this.streetEntrances)
		{
			if (nodeAccess.walkingAccess && !nodeAccess.employeeDoor && !nodeAccess.toNode.noAccess && !nodeAccess.fromNode.noAccess)
			{
				this.streetAccessRef.Add(Toolbox.Instance.ToFloat3(nodeAccess.fromNode.nodeCoord), nodeAccess.id);
				this.streetAccessPositions.TryAdd(nodeAccess.id, nodeAccess.worldAccessPoint);
				this.streetToNodeReference.TryAdd(nodeAccess.id, Toolbox.Instance.ToFloat3(nodeAccess.toNode.nodeCoord));
			}
		}
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0009D394 File Offset: 0x0009B594
	public List<NewTile> GetTileRoute(NewTile origin, NewTile destination, List<NewTile> avoidTiles = null)
	{
		Dictionary<NewTile, bool> dictionary = new Dictionary<NewTile, bool>(96);
		Dictionary<NewTile, bool> dictionary2 = new Dictionary<NewTile, bool>(96);
		Dictionary<NewTile, NewTile> dictionary3 = new Dictionary<NewTile, NewTile>(96);
		Dictionary<NewTile, float> dictionary4 = new Dictionary<NewTile, float>(96);
		Dictionary<NewTile, float> dictionary5 = new Dictionary<NewTile, float>(96);
		dictionary2.Add(origin, true);
		dictionary4.Add(origin, 0f);
		dictionary5.Add(origin, Vector3.Distance(origin.globalTileCoord, destination.globalTileCoord));
		int num = 0;
		while (dictionary2.Count > 0)
		{
			NewTile newTile = null;
			float num2 = float.PositiveInfinity;
			foreach (KeyValuePair<NewTile, bool> keyValuePair in dictionary2)
			{
				if (dictionary5.ContainsKey(keyValuePair.Key) && dictionary5[keyValuePair.Key] < num2)
				{
					num2 = dictionary5[keyValuePair.Key];
					newTile = keyValuePair.Key;
				}
			}
			if (newTile == destination)
			{
				List<NewTile> list = new List<NewTile>(96);
				list.Add(newTile);
				while (dictionary3.ContainsKey(newTile))
				{
					NewTile newTile2 = newTile;
					newTile = dictionary3[newTile];
					if (newTile == null)
					{
						Game.Log("Pathfinder: Tile pathfinder path construction error: current is null, invalid path", 2);
						break;
					}
					if (newTile != newTile2)
					{
						list.Add(newTile);
					}
				}
				list.Reverse();
				return list;
			}
			dictionary2.Remove(newTile);
			dictionary.Add(newTile, true);
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
			{
				Vector3 vector = newTile.globalTileCoord + new Vector3((float)vector2Int.x, (float)vector2Int.y, 0f);
				if (this.tileMap.ContainsKey(vector))
				{
					NewTile newTile3 = this.tileMap[vector];
					if (newTile3.isEdge && (avoidTiles == null || !avoidTiles.Contains(newTile3)))
					{
						bool flag = false;
						if (!dictionary.TryGetValue(newTile3, ref flag))
						{
							float num3 = dictionary4[newTile] + 1f;
							if (!dictionary2.TryGetValue(newTile3, ref flag))
							{
								dictionary2.Add(newTile3, true);
							}
							if (!dictionary4.ContainsKey(newTile3) || num3 < dictionary4[newTile3])
							{
								if (!dictionary3.ContainsKey(newTile3))
								{
									dictionary3.Add(newTile3, newTile);
								}
								else
								{
									dictionary3[newTile3] = newTile;
								}
								if (!dictionary4.ContainsKey(newTile3))
								{
									dictionary4.Add(newTile3, num3);
								}
								else
								{
									dictionary4[newTile3] = num3;
								}
								float num4 = dictionary4[newTile3] + Vector3.Distance(newTile3.globalTileCoord, destination.globalTileCoord);
								if (!dictionary5.ContainsKey(newTile3))
								{
									dictionary5.Add(newTile3, num4);
								}
								else
								{
									dictionary5[newTile3] = num4;
								}
							}
						}
					}
				}
			}
			num++;
			if (num > 9999)
			{
				return null;
			}
		}
		return null;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0009D690 File Offset: 0x0009B890
	private void OnDisable()
	{
		try
		{
			this.streetAccessRef.Dispose();
			this.streetAccessPositions.Dispose();
			this.streetToNodeReference.Dispose();
			this.streetNoPassRef.Dispose();
			this.streetAccessRef.Dispose();
		}
		catch
		{
		}
	}

	// Token: 0x04000A4F RID: 2639
	private const int INITIAL_COLLECTION_SIZE = 96;

	// Token: 0x04000A50 RID: 2640
	public Vector3 tileSize;

	// Token: 0x04000A51 RID: 2641
	public Vector3 nodeSize;

	// Token: 0x04000A52 RID: 2642
	public Vector2 citySizeReal;

	// Token: 0x04000A53 RID: 2643
	public Vector2 halfCitySizeReal;

	// Token: 0x04000A54 RID: 2644
	public Vector2 tileCitySize;

	// Token: 0x04000A55 RID: 2645
	public Vector2 nodeCitySize;

	// Token: 0x04000A56 RID: 2646
	public Vector2 nodeRangeX;

	// Token: 0x04000A57 RID: 2647
	public Vector2 nodeRangeY;

	// Token: 0x04000A58 RID: 2648
	public Vector2 nodeRangeZ;

	// Token: 0x04000A59 RID: 2649
	[Header("Debug Data")]
	public int totalPathCalls;

	// Token: 0x04000A5A RID: 2650
	public int calculatedRoomRoutes;

	// Token: 0x04000A5B RID: 2651
	public int returnedCachedRoomRoutes;

	// Token: 0x04000A5C RID: 2652
	public int calculatedInternalRoutes;

	// Token: 0x04000A5D RID: 2653
	public Dictionary<Vector3, NewNode> nodeMap = new Dictionary<Vector3, NewNode>(96, FastVector3Comparer.SharedFastVector3Comparer);

	// Token: 0x04000A5E RID: 2654
	public Dictionary<Vector3, NewTile> tileMap = new Dictionary<Vector3, NewTile>(96, FastVector3Comparer.SharedFastVector3Comparer);

	// Token: 0x04000A5F RID: 2655
	public Dictionary<PathFinder.GameLocationPathKey, List<NewNode.NodeAccess>> gameLocationRoutes = new Dictionary<PathFinder.GameLocationPathKey, List<NewNode.NodeAccess>>(96);

	// Token: 0x04000A60 RID: 2656
	[Header("AI Navigation")]
	public Dictionary<NewAddress.PathKey, List<NewNode.NodeAccess>> internalRoutes = new Dictionary<NewAddress.PathKey, List<NewNode.NodeAccess>>(96);

	// Token: 0x04000A61 RID: 2657
	public List<NewNode.NodeAccess> streetEntrances = new List<NewNode.NodeAccess>(96);

	// Token: 0x04000A62 RID: 2658
	public Dictionary<int, NewNode.NodeAccess> nodeAccessReference = new Dictionary<int, NewNode.NodeAccess>(96);

	// Token: 0x04000A63 RID: 2659
	public NativeMultiHashMap<float3, int> streetAccessRef;

	// Token: 0x04000A64 RID: 2660
	public NativeHashMap<int, float3> streetAccessPositions;

	// Token: 0x04000A65 RID: 2661
	public NativeHashMap<int, float3> streetToNodeReference;

	// Token: 0x04000A66 RID: 2662
	public NativeList<float3> streetNoPassRef;

	// Token: 0x04000A67 RID: 2663
	public List<PathFinder.StreetChunk> streetChunks = new List<PathFinder.StreetChunk>(96);

	// Token: 0x04000A68 RID: 2664
	private static PathFinder _instance;

	// Token: 0x020001A0 RID: 416
	public class PathData
	{
		// Token: 0x06000A5E RID: 2654 RVA: 0x0009D764 File Offset: 0x0009B964
		public NewNode GetNodeAhead(int routeCursor)
		{
			NewNode newNode = null;
			routeCursor = Mathf.Max(0, routeCursor);
			if (routeCursor < this.accessList.Count)
			{
				newNode = this.accessList[routeCursor].toNode;
				if (routeCursor + 1 < this.accessList.Count)
				{
					if (this.accessList[routeCursor + 1].toNode != newNode && this.accessList[routeCursor + 1].fromNode != newNode)
					{
						newNode = this.accessList[routeCursor].fromNode;
					}
				}
				else if (routeCursor - 1 >= 0 && (this.accessList[routeCursor - 1].toNode == newNode || this.accessList[routeCursor - 1].fromNode == newNode))
				{
					newNode = this.accessList[routeCursor].fromNode;
				}
			}
			return newNode;
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0009D834 File Offset: 0x0009BA34
		public NewNode GetNodeBehind(int routeCursor)
		{
			NewNode newNode = null;
			routeCursor = Mathf.Max(0, routeCursor);
			if (routeCursor < this.accessList.Count)
			{
				newNode = this.accessList[routeCursor].fromNode;
				if (routeCursor - 1 >= 0)
				{
					if (this.accessList[routeCursor - 1].toNode != newNode && this.accessList[routeCursor - 1].fromNode != newNode)
					{
						newNode = this.accessList[routeCursor].toNode;
					}
				}
				else if (routeCursor + 1 < this.accessList.Count && (this.accessList[routeCursor + 1].toNode == newNode || this.accessList[routeCursor + 1].fromNode == newNode))
				{
					newNode = this.accessList[routeCursor].toNode;
				}
			}
			return newNode;
		}

		// Token: 0x04000A69 RID: 2665
		public List<NewNode.NodeAccess> accessList = new List<NewNode.NodeAccess>(96);
	}

	// Token: 0x020001A1 RID: 417
	public struct RoomPathKey : IEquatable<PathFinder.RoomPathKey>
	{
		// Token: 0x06000A61 RID: 2657 RVA: 0x0009D919 File Offset: 0x0009BB19
		public RoomPathKey(NewRoom locOne, NewRoom locTwo)
		{
			this.originRoom = locOne;
			this.destinationRoom = locTwo;
			this.hasHash = false;
			this.hash = 0;
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x0009D937 File Offset: 0x0009BB37
		public override int GetHashCode()
		{
			if (!this.hasHash)
			{
				this.hash = HashCode.Combine<NewRoom, NewRoom>(this.originRoom, this.destinationRoom);
				this.hasHash = true;
			}
			return this.hash;
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x0009D965 File Offset: 0x0009BB65
		bool IEquatable<PathFinder.RoomPathKey>.Equals(PathFinder.RoomPathKey other)
		{
			return other.GetHashCode() == this.GetHashCode();
		}

		// Token: 0x04000A6A RID: 2666
		public NewRoom originRoom;

		// Token: 0x04000A6B RID: 2667
		public NewRoom destinationRoom;

		// Token: 0x04000A6C RID: 2668
		private bool hasHash;

		// Token: 0x04000A6D RID: 2669
		private int hash;
	}

	// Token: 0x020001A2 RID: 418
	public struct GameLocationPathKey : IEquatable<PathFinder.GameLocationPathKey>
	{
		// Token: 0x06000A64 RID: 2660 RVA: 0x0009D982 File Offset: 0x0009BB82
		public GameLocationPathKey(NewGameLocation locOne, NewGameLocation locTwo)
		{
			this.originLocation = locOne;
			this.destinationLocation = locTwo;
			this.hasHash = false;
			this.hash = 0;
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x0009D9A0 File Offset: 0x0009BBA0
		public override int GetHashCode()
		{
			if (!this.hasHash)
			{
				this.hash = HashCode.Combine<NewGameLocation, NewGameLocation>(this.originLocation, this.destinationLocation);
				this.hasHash = true;
			}
			return this.hash;
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x0009D9CE File Offset: 0x0009BBCE
		bool IEquatable<PathFinder.GameLocationPathKey>.Equals(PathFinder.GameLocationPathKey other)
		{
			return other.GetHashCode() == this.GetHashCode();
		}

		// Token: 0x04000A6E RID: 2670
		public NewGameLocation originLocation;

		// Token: 0x04000A6F RID: 2671
		public NewGameLocation destinationLocation;

		// Token: 0x04000A70 RID: 2672
		private bool hasHash;

		// Token: 0x04000A71 RID: 2673
		private int hash;
	}

	// Token: 0x020001A3 RID: 419
	public class StreetChunk
	{
		// Token: 0x06000A67 RID: 2663 RVA: 0x0009D9EC File Offset: 0x0009BBEC
		public StreetChunk(Vector3 newAnchor, List<Vector3> newList, bool newIsJunction)
		{
			this.id = PathFinder.StreetChunk.assignID;
			PathFinder.StreetChunk.assignID++;
			this.anchorTile = newAnchor;
			this.allCoords = newList;
			this.isJunction = newIsJunction;
			foreach (Vector3 vector in this.allCoords)
			{
				this.streetMaxSizeX = new Vector2(Mathf.Min(this.streetMaxSizeX.x, vector.x), Mathf.Max(this.streetMaxSizeX.y, vector.x));
				this.streetMaxSizeY = new Vector2(Mathf.Min(this.streetMaxSizeY.x, vector.y), Mathf.Max(this.streetMaxSizeY.y, vector.y));
				NewTile newTile;
				if (PathFinder.Instance.tileMap.TryGetValue(vector, ref newTile))
				{
					this.allTiles.Add(newTile);
					newTile.streetChunk = this;
				}
			}
			this.xMagnitude = Mathf.Abs(this.streetMaxSizeX.y - this.streetMaxSizeX.x);
			this.yMagnitude = Mathf.Abs(this.streetMaxSizeY.y - this.streetMaxSizeY.x);
			if (this.yMagnitude > this.xMagnitude)
			{
				this.isHorizontal = false;
			}
			else
			{
				this.isHorizontal = true;
			}
			if (this.isJunction)
			{
				this.name = "Junction " + this.id.ToString();
				return;
			}
			this.name = "Road " + this.id.ToString();
			if (this.isHorizontal)
			{
				this.name = "Horizontal " + this.name;
				return;
			}
			this.name = "Vertical " + this.name;
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x0009DC24 File Offset: 0x0009BE24
		public Dictionary<PathFinder.StreetChunk, bool> GetAdjacentChunks(bool horizontal)
		{
			Dictionary<PathFinder.StreetChunk, bool> dictionary = new Dictionary<PathFinder.StreetChunk, bool>(96);
			foreach (Vector3 vector in this.allCoords)
			{
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
				for (int i = 0; i < offsetArrayX.Length; i++)
				{
					Vector2 vector2 = offsetArrayX[i];
					Vector3 vector3 = vector + new Vector3(vector2.x, vector2.y, 0f);
					NewTile newTile;
					if (PathFinder.Instance.tileMap.TryGetValue(vector3, ref newTile) && newTile.streetChunk != null && !dictionary.ContainsKey(newTile.streetChunk))
					{
						bool flag = true;
						if (vector2.y != 0f)
						{
							flag = false;
						}
						bool flag2 = false;
						if (horizontal == flag)
						{
							flag2 = true;
						}
						dictionary.Add(newTile.streetChunk, flag2);
					}
				}
			}
			return dictionary;
		}

		// Token: 0x04000A72 RID: 2674
		public string name;

		// Token: 0x04000A73 RID: 2675
		public int id;

		// Token: 0x04000A74 RID: 2676
		public static int assignID;

		// Token: 0x04000A75 RID: 2677
		public Vector3 anchorTile;

		// Token: 0x04000A76 RID: 2678
		public List<Vector3> allCoords = new List<Vector3>();

		// Token: 0x04000A77 RID: 2679
		public List<NewTile> allTiles = new List<NewTile>();

		// Token: 0x04000A78 RID: 2680
		public bool isJunction;

		// Token: 0x04000A79 RID: 2681
		public bool isHorizontal = true;

		// Token: 0x04000A7A RID: 2682
		public float xMagnitude;

		// Token: 0x04000A7B RID: 2683
		public float yMagnitude;

		// Token: 0x04000A7C RID: 2684
		public Vector2 streetMaxSizeX = new Vector2(9999f, -9999f);

		// Token: 0x04000A7D RID: 2685
		public Vector2 streetMaxSizeY = new Vector2(9999f, -9999f);

		// Token: 0x04000A7E RID: 2686
		public int footfall;

		// Token: 0x04000A7F RID: 2687
		public float footfallNormalized;
	}

	// Token: 0x020001A4 RID: 420
	[BurstCompile]
	public struct GetInternalRouteJob : IJob
	{
		// Token: 0x06000A69 RID: 2665 RVA: 0x0009DD2C File Offset: 0x0009BF2C
		public void Execute()
		{
			int num = this.accessRef.Count();
			NativeHashMap<int, int> nativeHashMap = new NativeHashMap<int, int>(num, 2);
			NativeList<int> nativeList = new NativeList<int>(2);
			NativeHashMap<float3, int> nativeHashMap2 = new NativeHashMap<float3, int>(num, 2);
			nativeHashMap2.TryAdd(this.origin, 1);
			NativeHashMap<int, int> nativeHashMap3 = new NativeHashMap<int, int>(num, 2);
			NativeHashMap<int, float> nativeHashMap4 = new NativeHashMap<int, float>(num, 2);
			NativeHashMap<int, float> nativeHashMap5 = new NativeHashMap<int, float>(num, 2);
			int num2;
			NativeMultiHashMapIterator<float3> nativeMultiHashMapIterator;
			if (this.accessRef.TryGetFirstValue(this.origin, ref num2, ref nativeMultiHashMapIterator))
			{
				do
				{
					float3 @float = this.toNodeReference[num2];
					if (!NativeArrayExtensions.Contains<float3, float3>(this.noPassRef, @float) || (@float.x == this.destination.x && @float.y == this.destination.y && @float.z == this.destination.z))
					{
						nativeList.Add(ref num2);
						nativeHashMap4.TryAdd(num2, Vector3.Distance(this.origin, @float));
						nativeHashMap5.TryAdd(num2, Vector3.Distance(@float, this.destination));
					}
				}
				while (this.accessRef.TryGetNextValue(ref num2, ref nativeMultiHashMapIterator));
			}
			int num3 = 0;
			while (nativeList.Length > 0)
			{
				int num4 = -1;
				float num5 = float.PositiveInfinity;
				for (int i = 0; i < nativeList.Length; i++)
				{
					int num6 = nativeList[i];
					float num7;
					if (nativeHashMap5.TryGetValue(num6, ref num7) && num7 < num5)
					{
						num5 = num7;
						num4 = num6;
					}
				}
				float3 float2;
				this.toNodeReference.TryGetValue(num4, ref float2);
				int num10;
				if (float2.x == this.destination.x && float2.y == this.destination.y && float2.z == this.destination.z)
				{
					NativeList<int> nativeList2 = new NativeList<int>(2);
					nativeList2.Add(ref num4);
					int num8;
					while (nativeHashMap3.TryGetValue(num4, ref num8))
					{
						int num9 = num4;
						num4 = num8;
						if (num4 != num9)
						{
							nativeList2.Add(ref num4);
						}
					}
					for (int j = nativeList2.Length - 1; j >= 0; j--)
					{
						num10 = nativeList2[j];
						this.output.Add(ref num10);
					}
					nativeList2.Dispose();
					nativeHashMap.Dispose();
					nativeList.Dispose();
					nativeHashMap2.Dispose();
					nativeHashMap3.Dispose();
					nativeHashMap4.Dispose();
					nativeHashMap5.Dispose();
					return;
				}
				NativeList<int> nativeList3 = new NativeList<int>(2);
				for (int k = 0; k < nativeList.Length; k++)
				{
					int num11 = nativeList[k];
					if (num11 != num4)
					{
						nativeList3.Add(ref num11);
					}
				}
				nativeList.Clear();
				nativeList.AddRange(nativeList3);
				nativeList3.Dispose();
				nativeHashMap.TryAdd(num4, 1);
				int num12;
				NativeMultiHashMapIterator<float3> nativeMultiHashMapIterator2;
				if (this.accessRef.TryGetFirstValue(float2, ref num12, ref nativeMultiHashMapIterator2))
				{
					do
					{
						if (!nativeHashMap.TryGetValue(num12, ref num10))
						{
							float3 float3 = this.toNodeReference[num12];
							float num13 = Vector3.Distance(float2, float3);
							float num14 = nativeHashMap4[num4] + num13;
							if (!NativeArrayExtensions.Contains<int, int>(nativeList, num12) && !nativeHashMap2.TryGetValue(float3, ref num10) && (!NativeArrayExtensions.Contains<float3, float3>(this.noPassRef, float3) || (float3.x == this.destination.x && float3.y == this.destination.y && float3.z == this.destination.z)))
							{
								nativeList.Add(ref num12);
								nativeHashMap2.TryAdd(float3, 1);
							}
							float num15;
							if (!nativeHashMap4.TryGetValue(num12, ref num15) || num14 < nativeHashMap4[num12])
							{
								if (!nativeHashMap3.TryGetValue(num12, ref num10))
								{
									nativeHashMap3.TryAdd(num12, num4);
								}
								else
								{
									nativeHashMap3.Remove(num12);
									nativeHashMap3.TryAdd(num12, num4);
								}
								if (!nativeHashMap4.TryGetValue(num12, ref num15))
								{
									nativeHashMap4.TryAdd(num12, num14);
								}
								else
								{
									nativeHashMap4.Remove(num12);
									nativeHashMap4.TryAdd(num12, num14);
								}
								float num16 = num14 + Vector3.Distance(this.accessPositions[num12], this.destination);
								if (!nativeHashMap5.TryGetValue(num12, ref num15))
								{
									nativeHashMap5.TryAdd(num12, num16);
								}
								else
								{
									nativeHashMap5.Remove(num12);
									nativeHashMap5.TryAdd(num12, num16);
								}
							}
						}
					}
					while (this.accessRef.TryGetNextValue(ref num12, ref nativeMultiHashMapIterator2));
				}
				num3++;
				if (num3 > 99999)
				{
					nativeHashMap.Dispose();
					nativeList.Dispose();
					nativeHashMap2.Dispose();
					nativeHashMap3.Dispose();
					nativeHashMap4.Dispose();
					nativeHashMap5.Dispose();
					return;
				}
			}
			nativeHashMap.Dispose();
			nativeList.Dispose();
			nativeHashMap2.Dispose();
			nativeHashMap3.Dispose();
			nativeHashMap4.Dispose();
			nativeHashMap5.Dispose();
		}

		// Token: 0x04000A80 RID: 2688
		[ReadOnly]
		public float3 origin;

		// Token: 0x04000A81 RID: 2689
		[ReadOnly]
		public float3 destination;

		// Token: 0x04000A82 RID: 2690
		[ReadOnly]
		public int listIndex;

		// Token: 0x04000A83 RID: 2691
		[ReadOnly]
		public NativeMultiHashMap<float3, int> accessRef;

		// Token: 0x04000A84 RID: 2692
		[ReadOnly]
		public NativeHashMap<int, float3> accessPositions;

		// Token: 0x04000A85 RID: 2693
		[ReadOnly]
		public NativeHashMap<int, float3> toNodeReference;

		// Token: 0x04000A86 RID: 2694
		public NativeList<float3> noPassRef;

		// Token: 0x04000A87 RID: 2695
		[WriteOnly]
		public NativeList<int> output;
	}
}
