using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

// Token: 0x020003BF RID: 959
public class GenerationController : MonoBehaviour
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060015AC RID: 5548 RVA: 0x0013C2D4 File Offset: 0x0013A4D4
	public static GenerationController Instance
	{
		get
		{
			return GenerationController._instance;
		}
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x0013C2DB File Offset: 0x0013A4DB
	private void Awake()
	{
		if (GenerationController._instance != null && GenerationController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GenerationController._instance = this;
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x0013C30C File Offset: 0x0013A50C
	public void UpdateGeometryFloor(NewFloor editFloor, string debug = "")
	{
		if (!this.updateTheseFloors.Contains(editFloor))
		{
			this.updateTheseFloors.Add(editFloor);
			if (!this.updateGeometryActive)
			{
				try
				{
					base.StartCoroutine(this.ExeUpdateGeometryAtEndOfFrame());
				}
				catch
				{
				}
			}
		}
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x0013C360 File Offset: 0x0013A560
	private IEnumerator ExeUpdateGeometryAtEndOfFrame()
	{
		this.updateGeometryActive = true;
		bool wait = true;
		while (wait)
		{
			wait = false;
			yield return new WaitForEndOfFrame();
		}
		foreach (NewFloor newFloor in this.updateTheseFloors)
		{
			if (!(newFloor == null))
			{
				foreach (NewAddress newAddress in newFloor.addresses)
				{
					foreach (NewRoom room in newAddress.rooms)
					{
						this.UpdateFloorCeilingRoom(room);
						this.UpdateWallsRoom(room);
					}
				}
			}
		}
		this.updateTheseFloors.Clear();
		this.updateGeometryActive = false;
		yield break;
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x0013C370 File Offset: 0x0013A570
	public void LoadGeometryFloor(NewFloor editFloor)
	{
		if (!this.loadTheseFloors.Contains(editFloor))
		{
			this.loadTheseFloors.Add(editFloor);
			if (!this.loadGeometryActive)
			{
				try
				{
					base.StartCoroutine(this.LoadGeometryAtEndOfFrame());
				}
				catch
				{
				}
			}
		}
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x0013C3C4 File Offset: 0x0013A5C4
	private IEnumerator LoadGeometryAtEndOfFrame()
	{
		this.loadGeometryActive = true;
		bool wait = true;
		while (wait)
		{
			wait = false;
			yield return new WaitForEndOfFrame();
		}
		foreach (NewFloor newFloor in this.loadTheseFloors)
		{
			if (!(newFloor == null))
			{
				foreach (KeyValuePair<Vector2Int, NewTile> keyValuePair in newFloor.tileMap)
				{
					keyValuePair.Value.SetFloorCeilingOptimization(keyValuePair.Value.useOptimizedFloor, true);
					keyValuePair.Value.SetAsStairwell(keyValuePair.Value.isStairwell, true, keyValuePair.Value.isInvertedStairwell);
				}
				foreach (NewAddress newAddress in newFloor.addresses)
				{
					foreach (NewNode newNode in newAddress.nodes)
					{
						foreach (NewWall newWall in newNode.walls)
						{
							newWall.SpawnWall(false);
							newWall.SpawnFrontage(false, null);
						}
					}
				}
				foreach (NewAddress newAddress2 in newFloor.addresses)
				{
					foreach (NewRoom newRoom in newAddress2.rooms)
					{
						this.LoadCornersRoom(newRoom);
						newRoom.geometryLoaded = true;
					}
				}
			}
		}
		this.loadTheseFloors.Clear();
		this.loadGeometryActive = false;
		yield break;
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x0013C3D4 File Offset: 0x0013A5D4
	public void LoadGeometryRoom(NewRoom room)
	{
		if (!SessionData.Instance.isFloorEdit && !GenerationController.Instance.spawnedRooms.Contains(room))
		{
			GenerationController.Instance.spawnedRooms.Add(room);
			ObjectPoolingController.Instance.roomsLoaded = GenerationController.Instance.spawnedRooms.Count;
		}
		if (room.isOutsideWindow && room.gameLocation.thisAsStreet == null)
		{
			room.geometryLoaded = true;
			room.gameObject.SetActive(true);
			return;
		}
		if (!SessionData.Instance.isFloorEdit)
		{
			MeshPoolingController.Instance.SpawnMeshesForRoom(room);
		}
		room.geometryLoaded = true;
		room.gameObject.SetActive(true);
		if (room.preset != null)
		{
			foreach (NewRoom.LightZoneData lightZoneData in room.lightZones)
			{
				if (room.preset.useAdditionalAreaLights)
				{
					if (lightZoneData.CreateAreaLight() && InteriorControls.Instance.oneAreaLightPerRoom && room.gameLocation.thisAsStreet == null)
					{
						break;
					}
				}
				else if (lightZoneData.allowLight && room.mainLights.Count <= 0 && room.preset.useMainLights)
				{
					lightZoneData.areaLightBrightness = 150f;
					if (lightZoneData.CreateAreaLight() && InteriorControls.Instance.oneAreaLightPerRoom && room.gameLocation.thisAsStreet == null)
					{
						break;
					}
				}
			}
			if (room.preset.boostCeilingEmission && room.ceilingMat != null && room.uniqueCeilingMaterial)
			{
				if (room.mainLightStatus)
				{
					Color color = Color.white;
					if (room.lightZones.Count > 0)
					{
						color = room.lightZones[0].areaLightColour;
					}
					room.ceilingMat.SetColor("_EmissiveColor", room.preset.ceilingEmissionBoost * color);
				}
				else
				{
					room.ceilingMat.SetColor("_EmissiveColor", Color.black);
				}
			}
			if (room.preset.allowBugs)
			{
				int num = 0;
				foreach (NewNode newNode in room.nodes)
				{
					if (newNode.floorType == NewNode.FloorTileType.floorAndCeiling || newNode.floorType == NewNode.FloorTileType.floorOnly)
					{
						num++;
					}
				}
				int num2 = Mathf.FloorToInt(room.floorMatKey.grubiness * ((float)num * 0.4f) * room.preset.bugAmountMultiplier);
				for (int i = room.spawnedBugs.Count; i < num2; i++)
				{
					BugController component = Toolbox.Instance.SpawnObject(InteriorControls.Instance.bug, room.transform).GetComponent<BugController>();
					component.Setup(room);
					room.spawnedBugs.Add(component);
				}
			}
		}
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x0013C6CC File Offset: 0x0013A8CC
	public void UnloadOldestRooms()
	{
		if (this.roomUnloadCheckActive)
		{
			return;
		}
		if (this.oldestRoomUnloadTimer > 0)
		{
			this.oldestRoomUnloadTimer--;
			return;
		}
		this.oldestRoomUnloadTimer = 10;
		base.StartCoroutine(this.UnloadOldestRoomsAtEndOfFrame());
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x0013C704 File Offset: 0x0013A904
	private IEnumerator UnloadOldestRoomsAtEndOfFrame()
	{
		this.roomUnloadCheckActive = true;
		bool wait = true;
		while (wait)
		{
			wait = false;
			yield return new WaitForEndOfFrame();
		}
		Game.Log("Debug: Attempting to unload oldest room(s)...", 2);
		int num = 0;
		while (num < this.spawnedRooms.Count && this.spawnedRooms.Count >= ObjectPoolingController.Instance.maxRoomCache)
		{
			NewRoom newRoom = this.spawnedRooms[num];
			if (!newRoom.gameObject.activeSelf)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					for (int i = 0; i < newNode.interactables.Count; i++)
					{
						newNode.interactables[i].DespawnObject();
					}
				}
				foreach (FurnitureClusterLocation furnitureClusterLocation in newRoom.furniture)
				{
					furnitureClusterLocation.UnloadFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.keep);
				}
				Toolbox.Instance.DestroyObject(newRoom.combinedFloor);
				Toolbox.Instance.DestroyObject(newRoom.combinedCeiling);
				Toolbox.Instance.DestroyObject(newRoom.combinedWalls);
				List<NewBuilding> list = Enumerable.ToList<NewBuilding>(newRoom.additionalWalls.Keys);
				while (list.Count > 0)
				{
					Object.Destroy(newRoom.additionalWalls[list[0]]);
					list.RemoveAt(0);
				}
				newRoom.additionalWalls = new Dictionary<NewBuilding, GameObject>();
				while (newRoom.spawnedBugs.Count > 0)
				{
					Object.Destroy(newRoom.spawnedBugs[0].gameObject);
					newRoom.spawnedBugs.RemoveAt(0);
				}
				newRoom.spawnedBugs = new List<BugController>();
				foreach (NewRoom.LightZoneData lightZoneData in newRoom.lightZones)
				{
					lightZoneData.RemoveAreaLight();
				}
				newRoom.geometryLoaded = false;
				newRoom.gameObject.SetActive(false);
				GenerationController.Instance.spawnedRooms.RemoveAt(num);
				ObjectPoolingController.Instance.roomsLoaded = GenerationController.Instance.spawnedRooms.Count;
				num--;
			}
			num++;
		}
		this.roomUnloadCheckActive = false;
		yield break;
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x0013C714 File Offset: 0x0013A914
	public void UpdateFloorCeilingFloor(NewFloor editFloor)
	{
		if (editFloor == null)
		{
			return;
		}
		foreach (KeyValuePair<Vector2Int, NewTile> keyValuePair in editFloor.tileMap)
		{
			keyValuePair.Value.SetFloorCeilingOptimization(keyValuePair.Value.CanBeOptimized(), false);
		}
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x0013C784 File Offset: 0x0013A984
	public void UpdateFloorCeilingRoom(NewRoom room)
	{
		if (room == null)
		{
			return;
		}
		List<NewTile> list = new List<NewTile>();
		foreach (NewNode newNode in room.nodes)
		{
			if (!list.Contains(newNode.tile))
			{
				list.Add(newNode.tile);
			}
		}
		foreach (NewTile newTile in list)
		{
			newTile.SetFloorCeilingOptimization(newTile.CanBeOptimized(), false);
		}
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x0013C83C File Offset: 0x0013AA3C
	public void UpdateWallsFloor(NewFloor editFloor)
	{
		if (editFloor == null)
		{
			return;
		}
		List<NewWall> list = new List<NewWall>();
		List<NewTile> list2 = new List<NewTile>();
		foreach (KeyValuePair<Vector2Int, NewNode> keyValuePair in editFloor.nodeMap)
		{
			foreach (NewWall newWall in keyValuePair.Value.walls)
			{
				list.Add(newWall);
				newWall.optimizationAnchor = false;
				newWall.optimizationOverride = false;
			}
		}
		foreach (KeyValuePair<Vector2Int, NewNode> keyValuePair2 in editFloor.nodeMap)
		{
			NewNode value = keyValuePair2.Value;
			if (!list2.Contains(value.tile))
			{
				list2.Add(value.tile);
			}
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
			{
				Vector3Int vector3Int;
				vector3Int..ctor(value.nodeCoord.x + vector2Int.x, value.nodeCoord.y + vector2Int.y, value.nodeCoord.z);
				NewNode newNode = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && newNode.room != value.room)
				{
					if (!(newNode.gameLocation.thisAsStreet != null) || !(value.gameLocation.thisAsStreet != null))
					{
						if (!list2.Contains(newNode.tile))
						{
							list2.Add(newNode.tile);
						}
						Vector2 vector;
						vector..ctor((float)(newNode.nodeCoord.x - value.nodeCoord.x), (float)(newNode.nodeCoord.y - value.nodeCoord.y));
						Vector2 wallOffset = vector * 0.5f;
						NewWall newWall2 = null;
						if (!value.wallDict.TryGetValue(wallOffset, ref newWall2))
						{
							NewWall newWall3 = new NewWall();
							newWall3.otherWall = newNode.walls.Find((NewWall item) => item.wallOffset == wallOffset * -1f);
							if (newWall3.otherWall != null)
							{
								newWall3.Setup(newWall3.otherWall.preset, value, wallOffset, value.tile.isEdge);
							}
							else
							{
								newWall3.Setup(CityControls.Instance.defaultWalls, value, wallOffset, value.tile.isEdge);
							}
							if (newWall3.otherWall != null)
							{
								newWall3.otherWall.otherWall = newWall3;
								bool flag = false;
								if (newWall3.node.room.roomType != InteriorControls.Instance.nullRoomType && newWall3.otherWall.node.room.roomType == InteriorControls.Instance.nullRoomType)
								{
									flag = true;
								}
								else if (newWall3.node.room.roomType != InteriorControls.Instance.nullRoomType && newWall3.otherWall.node.room.roomType != InteriorControls.Instance.nullRoomType)
								{
									if (newWall3.node.room.roomType.cyclePriority > newWall3.otherWall.node.room.roomType.cyclePriority)
									{
										flag = true;
									}
									else if (newWall3.node.room.roomType.cyclePriority == newWall3.otherWall.node.room.roomType.cyclePriority && newWall3.node.room.roomFloorID > newWall3.otherWall.node.room.roomFloorID)
									{
										flag = true;
									}
								}
								if (flag)
								{
									newWall3.parentWall = newWall3;
									newWall3.childWall = newWall3.otherWall;
									newWall3.otherWall.parentWall = newWall3;
									newWall3.otherWall.childWall = newWall3.otherWall;
								}
								else
								{
									newWall3.parentWall = newWall3.otherWall;
									newWall3.childWall = newWall3;
									newWall3.otherWall.parentWall = newWall3.otherWall;
									newWall3.otherWall.childWall = newWall3;
								}
								if (newWall3.childWall.preset != newWall3.parentWall.preset)
								{
									Game.Log("Copy preset from parent wall...", 2);
									newWall3.childWall.SetDoorPairPreset(newWall3.parentWall.preset, true, false, true);
								}
								if (newWall3.node.building != null && newWall3.otherWall.node.building == null)
								{
									if (newWall3.node.building.preset.defaultExteriorWallMaterial.Count > 0)
									{
										newWall3.otherWall.separateWall = true;
									}
								}
								else if (newWall3.node.building == null && newWall3.otherWall.node.building != null && newWall3.otherWall.node.building.preset.defaultExteriorWallMaterial.Count > 0)
								{
									newWall3.separateWall = true;
								}
							}
						}
						else
						{
							list.Remove(newWall2);
						}
					}
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].RemoveWall();
			list.RemoveAt(j);
			j--;
		}
		foreach (NewTile newTile in list2)
		{
			for (int k = 0; k < CityControls.Instance.nodeMultiplier; k++)
			{
				List<NewWall> list3 = new List<NewWall>();
				List<NewWall> list4 = new List<NewWall>();
				List<NewWall> list5 = new List<NewWall>();
				List<NewWall> list6 = new List<NewWall>();
				for (int l = 0; l < CityControls.Instance.nodeMultiplier; l++)
				{
					Vector2 check = new Vector2((float)l, (float)k);
					NewNode newNode2 = newTile.nodes.Find((NewNode item) => item.localTileCoord == check);
					if (newNode2 != null)
					{
						NewWall newWall4 = newNode2.walls.Find((NewWall item) => item.wallOffset.x == 0f && item.wallOffset.y > 0f && item.preset.optimizeSections);
						if (newWall4 != null)
						{
							list3.Add(newWall4);
						}
						NewWall newWall5 = newNode2.walls.Find((NewWall item) => item.wallOffset.x == 0f && item.wallOffset.y < 0f && item.preset.optimizeSections);
						if (newWall5 != null)
						{
							list4.Add(newWall5);
						}
					}
					check = new Vector2((float)k, (float)l);
					newNode2 = newTile.nodes.Find((NewNode item) => item.localTileCoord == check);
					if (newNode2 != null)
					{
						NewWall newWall6 = newNode2.walls.Find((NewWall item) => item.wallOffset.x < 0f && item.wallOffset.y == 0f && item.preset.optimizeSections);
						if (newWall6 != null)
						{
							list5.Add(newWall6);
						}
						NewWall newWall7 = newNode2.walls.Find((NewWall item) => item.wallOffset.x > 0f && item.wallOffset.y == 0f && item.preset.optimizeSections);
						if (newWall7 != null)
						{
							list6.Add(newWall7);
						}
					}
				}
				List<List<NewWall>> list7 = new List<List<NewWall>>();
				list7.Add(list3);
				list7.Add(list4);
				list7.Add(list5);
				list7.Add(list6);
				foreach (List<NewWall> list8 in list7)
				{
					if (list8.Count >= CityControls.Instance.nodeMultiplier && list8[0].node.room == list8[1].node.room && list8[0].node.room == list8[2].node.room)
					{
						for (int m = 0; m < list8.Count; m++)
						{
							if (m == Mathf.FloorToInt((float)CityControls.Instance.nodeMultiplier * 0.5f))
							{
								list8[m].optimizationOverride = false;
								list8[m].optimizationAnchor = true;
							}
							else
							{
								list8[m].optimizationOverride = true;
								list8[m].optimizationAnchor = false;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x0013D19C File Offset: 0x0013B39C
	public void UpdateWallsRoom(NewRoom room)
	{
		if (room == null)
		{
			return;
		}
		List<NewWall> list = new List<NewWall>();
		List<NewTile> list2 = new List<NewTile>();
		foreach (NewNode newNode in room.nodes)
		{
			foreach (NewWall newWall in newNode.walls)
			{
				list.Add(newWall);
				newWall.optimizationAnchor = false;
				newWall.optimizationOverride = false;
			}
		}
		foreach (NewNode newNode2 in room.nodes)
		{
			if (!list2.Contains(newNode2.tile))
			{
				list2.Add(newNode2.tile);
			}
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
			{
				Vector3Int vector3Int;
				vector3Int..ctor(newNode2.nodeCoord.x + vector2Int.x, newNode2.nodeCoord.y + vector2Int.y, newNode2.nodeCoord.z);
				NewNode newNode3 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode3) && newNode3.room != newNode2.room)
				{
					bool flag = false;
					if (newNode3.gameLocation.thisAsStreet != null && newNode2.gameLocation.thisAsStreet != null)
					{
						if (!newNode3.gameLocation.thisAsStreet.isAlley || !newNode2.gameLocation.thisAsStreet.isAlley)
						{
							goto IL_5B3;
						}
						flag = true;
					}
					if (!list2.Contains(newNode3.tile))
					{
						list2.Add(newNode3.tile);
					}
					Vector2 vector;
					vector..ctor((float)(newNode3.nodeCoord.x - newNode2.nodeCoord.x), (float)(newNode3.nodeCoord.y - newNode2.nodeCoord.y));
					Vector2 wallOffset = vector * 0.5f;
					NewWall newWall2 = null;
					if (!newNode2.wallDict.TryGetValue(wallOffset, ref newWall2))
					{
						NewWall newWall3 = new NewWall();
						newWall3.otherWall = newNode3.walls.Find((NewWall item) => item.wallOffset == wallOffset * -1f);
						if (newWall3.otherWall != null)
						{
							newWall3.Setup(newWall3.otherWall.preset, newNode2, wallOffset, newNode2.tile.isEdge);
						}
						else if (flag)
						{
							newWall3.Setup(CityControls.Instance.alleyBlockWallPreset, newNode2, wallOffset, newNode2.tile.isEdge);
						}
						else
						{
							newWall3.Setup(CityControls.Instance.defaultWalls, newNode2, wallOffset, newNode2.tile.isEdge);
						}
						if (newWall3.otherWall != null)
						{
							newWall3.otherWall.otherWall = newWall3;
							bool flag2 = false;
							if (newWall3.node.room.roomType != InteriorControls.Instance.nullRoomType && newWall3.otherWall.node.room.roomType == InteriorControls.Instance.nullRoomType)
							{
								flag2 = true;
							}
							else if (newWall3.node.room.roomType != InteriorControls.Instance.nullRoomType && newWall3.otherWall.node.room.roomType != InteriorControls.Instance.nullRoomType)
							{
								if (newWall3.node.room.roomType.cyclePriority > newWall3.otherWall.node.room.roomType.cyclePriority)
								{
									flag2 = true;
								}
								else if (newWall3.node.room.roomType.cyclePriority == newWall3.otherWall.node.room.roomType.cyclePriority && newWall3.node.room.roomFloorID > newWall3.otherWall.node.room.roomFloorID)
								{
									flag2 = true;
								}
							}
							if (flag2)
							{
								newWall3.parentWall = newWall3;
								newWall3.childWall = newWall3.otherWall;
								newWall3.otherWall.parentWall = newWall3;
								newWall3.otherWall.childWall = newWall3.otherWall;
							}
							else
							{
								newWall3.parentWall = newWall3.otherWall;
								newWall3.childWall = newWall3;
								newWall3.otherWall.parentWall = newWall3.otherWall;
								newWall3.otherWall.childWall = newWall3;
							}
							if (newWall3.childWall.preset != newWall3.parentWall.preset)
							{
								Game.Log("Copy preset from parent wall...", 2);
								newWall3.childWall.SetDoorPairPreset(newWall3.parentWall.preset, true, false, true);
							}
							if (newWall3.node.building != null && newWall3.otherWall.node.building == null)
							{
								if (newWall3.node.building.preset.defaultExteriorWallMaterial.Count > 0)
								{
									newWall3.otherWall.separateWall = true;
								}
							}
							else if (newWall3.node.building == null && newWall3.otherWall.node.building != null && newWall3.otherWall.node.building.preset.defaultExteriorWallMaterial.Count > 0)
							{
								newWall3.separateWall = true;
							}
						}
					}
					else
					{
						list.Remove(newWall2);
					}
				}
				IL_5B3:;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].RemoveWall();
			list.RemoveAt(j);
			j--;
		}
		foreach (NewTile newTile in list2)
		{
			for (int k = 0; k < CityControls.Instance.nodeMultiplier; k++)
			{
				List<NewWall> list3 = new List<NewWall>();
				List<NewWall> list4 = new List<NewWall>();
				List<NewWall> list5 = new List<NewWall>();
				List<NewWall> list6 = new List<NewWall>();
				for (int l = 0; l < CityControls.Instance.nodeMultiplier; l++)
				{
					Vector2 check = new Vector2((float)l, (float)k);
					NewNode newNode4 = newTile.nodes.Find((NewNode item) => item.localTileCoord == check);
					if (newNode4 != null)
					{
						NewWall newWall4 = newNode4.walls.Find((NewWall item) => item.wallOffset.x == 0f && item.wallOffset.y > 0f && item.preset.optimizeSections);
						if (newWall4 != null)
						{
							list3.Add(newWall4);
						}
						NewWall newWall5 = newNode4.walls.Find((NewWall item) => item.wallOffset.x == 0f && item.wallOffset.y < 0f && item.preset.optimizeSections);
						if (newWall5 != null)
						{
							list4.Add(newWall5);
						}
					}
					check = new Vector2((float)k, (float)l);
					newNode4 = newTile.nodes.Find((NewNode item) => item.localTileCoord == check);
					if (newNode4 != null)
					{
						NewWall newWall6 = newNode4.walls.Find((NewWall item) => item.wallOffset.x < 0f && item.wallOffset.y == 0f && item.preset.optimizeSections);
						if (newWall6 != null)
						{
							list5.Add(newWall6);
						}
						NewWall newWall7 = newNode4.walls.Find((NewWall item) => item.wallOffset.x > 0f && item.wallOffset.y == 0f && item.preset.optimizeSections);
						if (newWall7 != null)
						{
							list6.Add(newWall7);
						}
					}
				}
				List<List<NewWall>> list7 = new List<List<NewWall>>();
				list7.Add(list3);
				list7.Add(list4);
				list7.Add(list5);
				list7.Add(list6);
				foreach (List<NewWall> list8 in list7)
				{
					if (list8.Count >= CityControls.Instance.nodeMultiplier && list8[0].node.room == list8[1].node.room && list8[0].node.room == list8[2].node.room)
					{
						for (int m = 0; m < list8.Count; m++)
						{
							if (m == Mathf.FloorToInt((float)CityControls.Instance.nodeMultiplier * 0.5f))
							{
								list8[m].optimizationOverride = false;
								list8[m].optimizationAnchor = true;
							}
							else
							{
								list8[m].optimizationOverride = true;
								list8[m].optimizationAnchor = false;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060015B9 RID: 5561 RVA: 0x0013DB48 File Offset: 0x0013BD48
	public void LoadCornersRoom(NewRoom room)
	{
		if (room == null)
		{
			Game.Log("room is missing", 2);
			return;
		}
		foreach (NewNode newNode in room.nodes)
		{
			foreach (NewWall newWall in newNode.walls)
			{
				newWall.SpawnCorner(false);
			}
		}
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x0013DBE8 File Offset: 0x0013BDE8
	public void GenerateAddressLayout(NewAddress ad)
	{
		string seedInput;
		if (SessionData.Instance.isFloorEdit)
		{
			seedInput = Toolbox.Instance.GenerateUniqueID();
		}
		else
		{
			string seed = CityData.Instance.seed;
			int i = ad.id * 2;
			seedInput = seed + i.ToString() + ad.building.cityTile.cityCoord.ToString() + ad.id.ToString();
		}
		GameObject gameObject = null;
		this.ResetLayout(ad, out gameObject);
		List<NewNode> list = new List<NewNode>();
		List<NewNode> list2 = new List<NewNode>();
		List<NewNode> list3 = new List<NewNode>();
		foreach (NewNode newNode in ad.nullRoom.nodes)
		{
			newNode.preventEntrances = new List<Vector2>();
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
			{
				Vector2Int vector2Int2 = newNode.floorCoord + vector2Int;
				if (ad.floor.nodeMap.ContainsKey(vector2Int2) && ad.floor.nodeMap[vector2Int2].gameLocation != newNode.gameLocation)
				{
					list.Add(newNode);
					foreach (NewWall newWall in newNode.walls)
					{
						if ((newWall.parentWall.node.gameLocation != ad || newWall.childWall.node.gameLocation != ad) && newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
						{
							list2.Add(newNode);
							if (newWall.parentWall.node.isOutside || newWall.childWall.node.isOutside)
							{
								if (list3.Count <= 0)
								{
									list3.Add(newNode);
								}
								else if (newWall.parentWall.node.tile.isMainEntrance || newWall.childWall.node.tile.isMainEntrance)
								{
									list3.Clear();
									list3.Add(newNode);
								}
								else
								{
									bool flag = false;
									if (newWall.parentWall.node.tile.isEntrance || newWall.childWall.node.tile.isEntrance)
									{
										flag = true;
									}
									bool flag2 = false;
									bool flag3 = false;
									foreach (NewNode newNode2 in list3)
									{
										foreach (NewWall newWall2 in newNode2.walls)
										{
											if (newWall2.parentWall.node.tile.isMainEntrance || newWall2.childWall.node.tile.isMainEntrance)
											{
												flag2 = true;
												break;
											}
											if (newWall2.parentWall.node.tile.isEntrance || newWall2.childWall.node.tile.isEntrance)
											{
												flag3 = true;
											}
										}
										if (flag2)
										{
											break;
										}
									}
									if (!flag2)
									{
										if (flag)
										{
											list3.Clear();
											list3.Add(newNode);
										}
										else if (!flag3)
										{
											list3.Add(newNode);
										}
									}
								}
							}
						}
					}
				}
			}
		}
		List<NewNode> list4 = new List<NewNode>(ad.nullRoom.nodes);
		this.CreateForcedRooms(ad);
		List<RoomTypePreset> list5 = new List<RoomTypePreset>(ad.preset.roomLayout);
		list5.Sort((RoomTypePreset p1, RoomTypePreset p2) => p2.cyclePriority.CompareTo(p1.cyclePriority));
		List<RoomTypePreset> list6 = new List<RoomTypePreset>();
		for (int j = 0; j < 5; j++)
		{
			foreach (RoomTypePreset roomTypePreset in list5)
			{
				if (roomTypePreset.maximumRoomTypesPerAddress > j)
				{
					list6.Add(roomTypePreset);
				}
			}
		}
		NewRoom newRoom = null;
		Vector2 vector;
		vector..ctor(0f, 0f);
		NewNode newNode3 = null;
		Vector2 vector2;
		vector2..ctor(0f, 0f);
		NewNode newNode4 = null;
		Vector2 vector3;
		vector3..ctor(0f, 0f);
		NewNode newNode5 = null;
		Vector2 vector4;
		vector4..ctor(0f, 0f);
		NewNode newNode6 = null;
		Dictionary<NewNode, NewNode> dictionary = new Dictionary<NewNode, NewNode>();
		int hallwayDistanceThreshold = ad.preset.hallwayDistanceThreshold;
		List<NewNode> list7 = new List<NewNode>(ad.nullRoom.nodes);
		while (list7.Count > 0)
		{
			NewNode newNode7 = list7[Toolbox.Instance.RandContained(0, list7.Count, seedInput, out seedInput)];
			float num = 0f;
			Vector2 vector5 = Vector2.zero;
			NewNode newNode8 = null;
			foreach (NewNode newNode9 in list2)
			{
				float num2 = Vector2.Distance(newNode7.floorCoord, newNode9.floorCoord);
				if (num2 > num)
				{
					num = num2;
					vector5 = new Vector2((float)(newNode7.localTileCoord.x - newNode9.localTileCoord.x), (float)(newNode7.localTileCoord.y - newNode9.localTileCoord.y));
					newNode8 = newNode9;
				}
			}
			if (num > (float)hallwayDistanceThreshold)
			{
				bool flag4 = false;
				if (vector5.x <= vector.x && vector5.y <= vector.y)
				{
					if (newNode3 != null)
					{
						dictionary.Remove(newNode3);
					}
					flag4 = true;
					vector = vector5;
					newNode3 = newNode7;
				}
				if (vector5.x <= vector2.x && vector5.y >= vector2.y)
				{
					if (newNode4 != null)
					{
						dictionary.Remove(newNode4);
					}
					flag4 = true;
					vector2 = vector5;
					newNode4 = newNode7;
				}
				if (vector5.x >= vector3.x && vector5.y <= vector3.y)
				{
					if (newNode5 != null)
					{
						dictionary.Remove(newNode5);
					}
					flag4 = true;
					vector3 = vector5;
					newNode5 = newNode7;
				}
				if (vector5.x >= vector4.x && vector5.y >= vector4.y)
				{
					if (newNode6 != null)
					{
						dictionary.Remove(newNode6);
					}
					flag4 = true;
					vector4 = vector5;
					newNode6 = newNode7;
				}
				if (flag4)
				{
					dictionary.Add(newNode7, newNode8);
				}
			}
			list7.Remove(newNode7);
		}
		if (ad.preset.requiresHallway && dictionary.Count > 0)
		{
			foreach (KeyValuePair<NewNode, NewNode> keyValuePair in dictionary)
			{
				if (newRoom != null)
				{
					bool flag5 = false;
					using (HashSet<NewNode>.Enumerator enumerator = newRoom.nodes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (Vector2.Distance(enumerator.Current.floorCoord, keyValuePair.Key.floorCoord) < (float)hallwayDistanceThreshold)
							{
								flag5 = true;
								break;
							}
						}
					}
					if (flag5)
					{
						continue;
					}
				}
				NewNode newNode10 = null;
				float num3 = -1f;
				foreach (NewNode newNode11 in ad.nullRoom.nodes)
				{
					float num4 = Vector2.Distance(newNode11.floorCoord, keyValuePair.Key.floorCoord);
					if (num4 <= (float)(hallwayDistanceThreshold - 1) && num4 > num3)
					{
						newNode10 = newNode11;
						num3 = num4;
					}
				}
				if (newNode10 != null)
				{
					List<NewNode> list8 = this.HallwayPathfind(keyValuePair.Value, newNode10, ad);
					if (list8 != null && list8.Count > 0)
					{
						if (newRoom == null)
						{
							newRoom = Object.Instantiate<GameObject>(PrefabControls.Instance.room, ad.transform).GetComponent<NewRoom>();
							newRoom.SetupAll(ad, ad.preset.hallway, -1);
							if (SessionData.Instance.isFloorEdit)
							{
								GameObject gameObject2 = new GameObject();
								gameObject2.transform.SetParent(gameObject.transform);
								gameObject2.transform.localPosition = Vector3.zero;
								gameObject2.name = "Hallway";
								GenerationDebugController component = Object.Instantiate<GameObject>(PrefabControls.Instance.debugAttemptObject, gameObject2.transform).GetComponent<GenerationDebugController>();
								component.Setup("Generated Hallway", ad.preset.hallway.roomType);
								component.attemptedValidNodes = list8;
								Game.Log("Hallways has been created using pathfinding from " + newNode10.name + " to " + keyValuePair.Value.name, 2);
							}
						}
						foreach (NewNode newNode12 in list8)
						{
							newRoom.AddNewNode(newNode12);
						}
					}
				}
			}
		}
		Predicate<NewNode> <>9__1;
		for (int k = 0; k < list6.Count; k++)
		{
			RoomTypePreset roomTypePreset2 = list6[k];
			List<NewNode> nodes = ad.nodes;
			Predicate<NewNode> predicate;
			if ((predicate = <>9__1) == null)
			{
				predicate = (<>9__1 = ((NewNode item) => (item.tile.isStairwell || item.tile.isInvertedStairwell) && item.room == ad.nullRoom));
			}
			NewNode newNode13 = nodes.Find(predicate);
			if (newNode13 != null)
			{
				NewTile tile = newNode13.tile;
				NewRoom component2 = Object.Instantiate<GameObject>(PrefabControls.Instance.room, ad.transform).GetComponent<NewRoom>();
				component2.SetupLayoutOnly(ad, roomTypePreset2, -1);
				int num5 = 0;
				if (tile.isInvertedStairwell)
				{
					num5 = tile.elevatorRotation;
				}
				else if (tile.isStairwell)
				{
					num5 = tile.stairwellRotation;
				}
				foreach (NewNode newNode14 in tile.nodes)
				{
					component2.AddNewNode(newNode14);
					ad.protectedNodes.Add(newNode14);
					foreach (Vector2Int vector2Int3 in CityData.Instance.offsetArrayX4)
					{
						NewNode newNode15 = null;
						if (this.GetAdjacentNode(newNode14, vector2Int3, out newNode15) && !tile.nodes.Contains(newNode15))
						{
							if (num5 == 0)
							{
								if (vector2Int3.y > 0)
								{
									component2.AddNewNode(newNode15);
									ad.protectedNodes.Add(newNode15);
									foreach (Vector2Int vector2Int4 in CityData.Instance.offsetArrayX4)
									{
										if (vector2Int4 != vector2Int3)
										{
											if (newNode15.preventEntrances == null)
											{
												newNode15.preventEntrances = new List<Vector2>();
											}
											newNode15.preventEntrances.Add(vector2Int4 / 2f);
										}
									}
								}
							}
							else if (num5 == 90)
							{
								if (vector2Int3.x > 0)
								{
									component2.AddNewNode(newNode15);
									ad.protectedNodes.Add(newNode15);
									foreach (Vector2Int vector2Int5 in CityData.Instance.offsetArrayX4)
									{
										if (vector2Int5 != vector2Int3)
										{
											if (newNode15.preventEntrances == null)
											{
												newNode15.preventEntrances = new List<Vector2>();
											}
											newNode15.preventEntrances.Add(vector2Int5 / 2f);
										}
									}
								}
							}
							else if (num5 == 180)
							{
								if (vector2Int3.y < 0)
								{
									component2.AddNewNode(newNode15);
									ad.protectedNodes.Add(newNode15);
									foreach (Vector2Int vector2Int6 in CityData.Instance.offsetArrayX4)
									{
										if (vector2Int6 != vector2Int3)
										{
											if (newNode15.preventEntrances == null)
											{
												newNode15.preventEntrances = new List<Vector2>();
											}
											newNode15.preventEntrances.Add(vector2Int6 / 2f);
										}
									}
								}
							}
							else if (num5 == 270 && vector2Int3.x < 0)
							{
								component2.AddNewNode(newNode15);
								ad.protectedNodes.Add(newNode15);
								foreach (Vector2Int vector2Int7 in CityData.Instance.offsetArrayX4)
								{
									if (vector2Int7 != vector2Int3)
									{
										if (newNode15.preventEntrances == null)
										{
											newNode15.preventEntrances = new List<Vector2>();
										}
										newNode15.preventEntrances.Add(vector2Int7 / 2f);
									}
								}
							}
						}
					}
				}
			}
			if (list4.Count >= roomTypePreset2.minimumAddressSize && Toolbox.Instance.RandContained(0f, 1f, seedInput, out seedInput) <= roomTypePreset2.chance)
			{
				List<GenerationController.PossibleRoomLocation> possibleRoomLocations = this.GetPossibleRoomLocations(ad, roomTypePreset2, list4, list2, list3, list, gameObject.transform);
				if (possibleRoomLocations.Count > 0)
				{
					possibleRoomLocations.Sort();
					possibleRoomLocations.Reverse();
					GenerationController.PossibleRoomLocation possibleRoomLocation = possibleRoomLocations[0];
					possibleRoomLocation.debugScript.name = "*" + possibleRoomLocation.debugScript.name;
					possibleRoomLocation.debugScript.executed = true;
					possibleRoomLocation.debugScript.gameObject.transform.SetAsFirstSibling();
					NewRoom component3 = Object.Instantiate<GameObject>(PrefabControls.Instance.room, ad.transform).GetComponent<NewRoom>();
					component3.SetupLayoutOnly(ad, roomTypePreset2, -1);
					component3.debugController = possibleRoomLocation.debugScript;
					component3.overrideData = possibleRoomLocation.overrideRankingData;
					new List<NewRoom>();
					foreach (NewNode newNode16 in possibleRoomLocation.nodes)
					{
						component3.AddNewNode(newNode16);
					}
					if (possibleRoomLocation.requiredHallway != null && possibleRoomLocation.requiredHallway.Count > 0)
					{
						foreach (NewNode newNode17 in possibleRoomLocation.requiredHallway)
						{
							if (newNode17.room != component3)
							{
								if (newRoom == null)
								{
									newRoom = Object.Instantiate<GameObject>(PrefabControls.Instance.room, ad.transform).GetComponent<NewRoom>();
									newRoom.SetupAll(ad, ad.preset.hallway, -1);
									if (SessionData.Instance.isFloorEdit)
									{
										GameObject gameObject3 = new GameObject();
										gameObject3.transform.SetParent(gameObject.transform);
										gameObject3.transform.localPosition = Vector3.zero;
										gameObject3.name = "Hallway (" + roomTypePreset2.ToString() + ")";
										GenerationDebugController component4 = Object.Instantiate<GameObject>(PrefabControls.Instance.debugAttemptObject, gameObject3.transform).GetComponent<GenerationDebugController>();
										component4.Setup("Generated Hallway", ad.preset.hallway.roomType);
										component4.attemptedValidNodes = possibleRoomLocation.requiredHallway;
										Game.Log("Hallways has been created using pathfinding", 2);
									}
								}
								if (newRoom != null)
								{
									newRoom.AddNewNode(newNode17);
									ad.protectedNodes.Add(newNode17);
								}
							}
						}
						if (newRoom.nodes.Count <= 2)
						{
							for (int m = 0; m < possibleRoomLocation.requiredHallway.Count; m++)
							{
								component3.AddNewNode(possibleRoomLocation.requiredHallway[m]);
							}
							ad.RemoveRoom(newRoom);
						}
					}
				}
			}
		}
		List<GenerationController.PossibleNullExpansion> list9 = new List<GenerationController.PossibleNullExpansion>();
		foreach (NewNode newNode18 in ad.nullRoom.nodes)
		{
			if (!ad.protectedNodes.Contains(newNode18))
			{
				foreach (Vector2Int vector2Int8 in CityData.Instance.offsetArrayX4)
				{
					Vector2Int vector2Int9 = newNode18.floorCoord + vector2Int8;
					NewNode newNode19 = null;
					if (ad.floor.nodeMap.TryGetValue(vector2Int9, ref newNode19) && !(newNode19.gameLocation == null) && !(newNode19.gameLocation != ad) && !(newNode19.room == ad.nullRoom) && newNode19.room.roomType.expandIntoNull && !newNode19.tile.isStairwell && !newNode19.tile.isInvertedStairwell)
					{
						List<NewNode> list10 = new List<NewNode>();
						List<NewNode> list11 = new List<NewNode>();
						list10.Add(newNode18);
						int num6 = 24;
						while (list10.Count > 0 && num6 > 0)
						{
							NewNode newNode20 = list10[0];
							foreach (Vector2Int vector2Int10 in CityData.Instance.offsetArrayX4)
							{
								Vector2Int vector2Int11 = newNode20.floorCoord + vector2Int10;
								NewNode newNode21 = null;
								if (ad.floor.nodeMap.TryGetValue(vector2Int11, ref newNode21) && newNode21.room == ad.nullRoom && newNode21.gameLocation == ad && !list11.Contains(newNode21))
								{
									foreach (Vector2Int vector2Int12 in CityData.Instance.offsetArrayX4)
									{
										Vector2Int vector2Int13 = newNode21.floorCoord + vector2Int12;
										NewNode newNode22 = null;
										if (ad.floor.nodeMap.TryGetValue(vector2Int13, ref newNode22) && newNode22.room == newNode19.room && !list10.Contains(newNode21) && !list11.Contains(newNode21))
										{
											list10.Add(newNode21);
										}
									}
								}
							}
							list11.Add(newNode20);
							list10.RemoveAt(0);
							num6--;
						}
						if (list11.Count >= newNode19.room.roomType.expandIntoNullAdjacencyMinimum)
						{
							list9.Add(new GenerationController.PossibleNullExpansion
							{
								nodesToExpand = list11,
								addToRoom = newNode19.room,
								ranking = (float)(list11.Count + newNode19.room.roomType.cyclePriority)
							});
						}
					}
				}
			}
		}
		list9.Sort();
		list9.Reverse();
		Predicate<NewNode> <>9__2;
		for (int num7 = 0; num7 < list9.Count; num7++)
		{
			List<NewNode> nodesToExpand = list9[num7].nodesToExpand;
			Predicate<NewNode> predicate2;
			if ((predicate2 = <>9__2) == null)
			{
				predicate2 = (<>9__2 = ((NewNode item) => item.room == ad.nullRoom));
			}
			List<NewNode> list12 = nodesToExpand.FindAll(predicate2);
			if (list12.Count >= list9[num7].addToRoom.roomType.expandIntoNullAdjacencyMinimum)
			{
				for (int num8 = 0; num8 < list12.Count; num8++)
				{
					list9[num7].addToRoom.AddNewNode(list12[num8]);
				}
			}
		}
		List<NewRoom> list13 = new List<NewRoom>();
		if (newRoom != null)
		{
			list13 = this.ConvertSplitRoom(ref newRoom.nodes, ad);
		}
		this.UpdateFloorCeilingFloor(ad.floor);
		this.UpdateWallsFloor(ad.floor);
		foreach (NewRoom newRoom2 in ad.rooms)
		{
			if (newRoom2.debugController != null)
			{
				newRoom2.debugController.Log("Begin primary entrances debug...");
			}
			if (newRoom2.roomType == InteriorControls.Instance.nullRoomType || newRoom2.preset == ad.preset.hallway)
			{
				if (newRoom2.debugController != null)
				{
					newRoom2.debugController.Log("Null space or hallway, this needs no entrances...");
				}
			}
			else if (newRoom2.roomType.mustConnectWithEntrance && list13.Count <= 0)
			{
				if (newRoom2.debugController != null)
				{
					newRoom2.debugController.Log("This must connect with entrance + there is no hallways: Doorway will open onto this...");
				}
			}
			else if (newRoom2.entrances.Count >= newRoom2.roomType.maxDoors || newRoom2.roomType.forceNoDoors)
			{
				if (newRoom2.debugController != null)
				{
					GenerationDebugController debugController = newRoom2.debugController;
					string text = "Maximum number of doors reached: ";
					int i = newRoom2.entrances.Count;
					debugController.Log(text + i.ToString() + "/" + newRoom2.roomType.maxDoors.ToString());
				}
			}
			else
			{
				List<GenerationController.PossibleDoorwayLocation> list14 = new List<GenerationController.PossibleDoorwayLocation>();
				foreach (NewNode newNode23 in newRoom2.nodes)
				{
					if (!newNode23.tile.isStairwell && !newNode23.tile.isInvertedStairwell)
					{
						using (List<NewWall>.Enumerator enumerator2 = newNode23.walls.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								NewWall wall = enumerator2.Current;
								if (wall.otherWall != null && wall.preset.sectionClass == DoorPairPreset.WallSectionClass.wall && !wall.preventEntrance && !wall.otherWall.preventEntrance && wall.otherWall.node.room.entrances.Count < wall.otherWall.node.room.roomType.maxDoors && !wall.otherWall.node.room.roomType.forceNoDoors && !wall.node.room.roomType.forceNoDoors && wall.otherWall.node.room.gameLocation == ad && wall.otherWall.node.room != newRoom2 && wall.otherWall.node.room.roomType != InteriorControls.Instance.nullRoomType && !wall.otherWall.node.tile.isStairwell && !wall.otherWall.node.tile.isInvertedStairwell && (newRoom2.roomType.mustAdjoinRooms.Count <= 0 || newRoom2.roomType.mustAdjoinRooms.Contains(wall.otherWall.node.room.roomType)))
								{
									GenerationController.PossibleDoorwayLocation possibleDoorwayLocation = new GenerationController.PossibleDoorwayLocation();
									possibleDoorwayLocation.wall = wall;
									possibleDoorwayLocation.ranking = (float)((5 - newRoom2.roomType.mustAdjoinRooms.FindIndex((RoomTypePreset item) => item == wall.otherWall.node.room.roomType)) * 2) + Toolbox.Instance.RandContained(0f, 0.1f, seedInput, out seedInput);
									List<NewWall> list15 = newNode23.walls.FindAll((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance || item.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge);
									list15.AddRange(wall.otherWall.node.walls.FindAll((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance || item.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge));
									possibleDoorwayLocation.ranking += (float)wall.otherWall.node.walls.Count * 0.2f;
									float num9 = 999f;
									foreach (NewNode newNode24 in list2)
									{
										float num10 = Vector2.Distance(newNode24.position, wall.position);
										num9 = Mathf.Min(num9, num10);
									}
									possibleDoorwayLocation.ranking -= num9;
									using (List<NewWall>.Enumerator enumerator4 = list15.GetEnumerator())
									{
										while (enumerator4.MoveNext())
										{
											if (enumerator4.Current.wallOffset == wall.wallOffset * -1f)
											{
												possibleDoorwayLocation.ranking -= 0.5f;
											}
											else
											{
												possibleDoorwayLocation.ranking -= 1f;
												possibleDoorwayLocation.requireFlatDoorway = true;
											}
										}
									}
									list14.Add(possibleDoorwayLocation);
								}
							}
						}
					}
				}
				if (newRoom2.debugController != null)
				{
					GenerationDebugController debugController2 = newRoom2.debugController;
					string text2 = "Possible doorways count: ";
					int i = list14.Count;
					debugController2.Log(text2 + i.ToString());
				}
				list14.Sort();
				list14.Reverse();
				if (list14.Count > 0)
				{
					if (newRoom2.debugController != null)
					{
						GenerationDebugController debugController3 = newRoom2.debugController;
						string[] array = new string[8];
						array[0] = "Picked doorway on node: ";
						int num11 = 1;
						Vector2Int localTileCoord = list14[0].wall.node.localTileCoord;
						array[num11] = localTileCoord.ToString();
						array[2] = " with offset of ";
						int num12 = 3;
						Vector2 wallOffset = list14[0].wall.wallOffset;
						array[num12] = wallOffset.ToString();
						array[4] = " (";
						array[5] = list14[0].wall.node.room.name;
						array[6] = " <-> ";
						array[7] = list14[0].wall.otherWall.node.room.name;
						debugController3.Log(string.Concat(array));
					}
					if (list14[0].requireFlatDoorway && newRoom2.gameLocation.thisAsAddress.preset.doorwaysFlat.Count > 0)
					{
						list14[0].wall.SetDoorPairPreset(newRoom2.gameLocation.thisAsAddress.preset.doorwaysFlat[Toolbox.Instance.RandContained(0, newRoom2.gameLocation.thisAsAddress.preset.doorwaysFlat.Count, seedInput, out seedInput)], false, false, true);
					}
					else
					{
						list14[0].wall.SetDoorPairPreset(newRoom2.gameLocation.thisAsAddress.preset.doorwaysNormal[Toolbox.Instance.RandContained(0, newRoom2.gameLocation.thisAsAddress.preset.doorwaysNormal.Count, seedInput, out seedInput)], false, false, true);
					}
					ad.generatedInteriorEntrances.Add(list14[0].wall);
					if (list14[0].wall.node.room.nodes.Count > 1 && list14[0].wall.otherWall.node.room.nodes.Count > 1)
					{
						ad.protectedNodes.Add(list14[0].wall.node);
						ad.protectedNodes.Add(list14[0].wall.otherWall.node);
					}
				}
			}
		}
		foreach (NewRoom newRoom3 in list13)
		{
			List<NewNode> list16 = new List<NewNode>();
			Dictionary<NewNode, NewRoom> dictionary2 = new Dictionary<NewNode, NewRoom>();
			if (newRoom3 != null)
			{
				foreach (NewNode newNode25 in newRoom3.nodes)
				{
					if (newNode25.walls.Count >= 3)
					{
						NewRoom newRoom4 = null;
						for (int num13 = 0; num13 < CityData.Instance.offsetArrayX4.Length; num13++)
						{
							NewNode newNode26 = null;
							Dictionary<NewRoom, int> dictionary3 = new Dictionary<NewRoom, int>();
							if (this.GetAdjacentNode(newNode25, CityData.Instance.offsetArrayX4[num13], out newNode26) && newNode26.gameLocation == ad && newNode26.room.roomType.allowCorridorReplacement)
							{
								if (dictionary3.ContainsKey(newNode26.room))
								{
									Dictionary<NewRoom, int> dictionary4 = dictionary3;
									NewRoom room = newNode26.room;
									int i = dictionary4[room];
									dictionary4[room] = i + 1;
								}
								else
								{
									dictionary3.Add(newNode26.room, 1);
								}
							}
							if (newRoom4 == null && dictionary3.Count > 0)
							{
								int num14 = -1;
								foreach (KeyValuePair<NewRoom, int> keyValuePair2 in dictionary3)
								{
									if (keyValuePair2.Value > num14)
									{
										newRoom4 = keyValuePair2.Key;
										num14 = keyValuePair2.Value;
									}
								}
							}
						}
						if (newRoom4 == null)
						{
							break;
						}
						List<NewWall> list17 = newNode25.walls.FindAll((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance);
						if (list17 == null || list17.Count <= 0)
						{
							dictionary2.Add(newNode25, newRoom4);
							list16.Add(newNode25);
						}
						else if (newRoom4 != null)
						{
							bool flag6 = true;
							foreach (NewWall newWall3 in list17)
							{
								if (!newWall3.otherWall.node.room.roomType.mustAdjoinRooms.Contains(newRoom4.roomType) && newWall3.otherWall.node.room != newRoom4)
								{
									flag6 = false;
									break;
								}
							}
							if (flag6)
							{
								dictionary2.Add(newNode25, newRoom4);
								list16.Add(newNode25);
							}
						}
					}
				}
			}
			int num15 = 999;
			while (dictionary2.Count > 0 && num15 > 0)
			{
				NewNode newNode27 = list16[0];
				NewRoom newRoom5 = dictionary2[list16[0]];
				List<NewNode> list18 = new List<NewNode>(newRoom3.nodes);
				list18.Remove(newNode27);
				if (!this.RoomSplitCheck(ref list18, null))
				{
					break;
				}
				for (int num16 = 0; num16 < CityData.Instance.offsetArrayX4.Length; num16++)
				{
					NewNode newNode28 = null;
					if (this.GetAdjacentNode(newNode27, CityData.Instance.offsetArrayX4[num16], out newNode28) && newNode28.gameLocation == ad && !newNode28.tile.isStairwell && !newNode28.tile.isInvertedStairwell && newNode28.room.gameLocation.thisAsAddress != null && newNode28.room.preset == newNode28.room.gameLocation.thisAsAddress.preset.hallway)
					{
						List<NewWall> list19 = newNode28.walls.FindAll((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance);
						if (list19 == null || list19.Count <= 0)
						{
							if (!dictionary2.ContainsKey(newNode28))
							{
								dictionary2.Add(newNode28, newRoom5);
								list16.Add(newNode28);
							}
						}
						else if (newRoom5 != null)
						{
							bool flag7 = true;
							foreach (NewWall newWall4 in list19)
							{
								if (!newWall4.otherWall.node.room.roomType.mustAdjoinRooms.Contains(newRoom5.roomType) && newWall4.otherWall.node.room != newRoom5)
								{
									flag7 = false;
									break;
								}
							}
							if (flag7 && !dictionary2.ContainsKey(newNode28))
							{
								dictionary2.Add(newNode28, newRoom5);
								list16.Add(newNode28);
							}
						}
					}
				}
				if (newRoom5 != null)
				{
					newRoom5.AddNewNode(newNode27);
				}
				dictionary2.Remove(newNode27);
				list16.RemoveAt(0);
				if (newRoom3.nodes.Count <= 2)
				{
					break;
				}
				num15--;
			}
		}
		this.UpdateFloorCeilingFloor(ad.floor);
		this.UpdateWallsFloor(ad.floor);
		HashSet<NewRoom> unreachableRooms = this.GetUnreachableRooms(list2, ad);
		foreach (NewRoom newRoom6 in ad.rooms)
		{
			if (!(newRoom6.roomType == InteriorControls.Instance.nullRoomType) && !newRoom6.roomType.forceNoDoors && (newRoom6.entrances.Count < newRoom6.roomType.maxDoors || newRoom6.roomType.allowRoomDividers || unreachableRooms.Contains(newRoom6)))
			{
				if (newRoom6.debugController != null)
				{
					newRoom6.debugController.Log("Begin secondary entrances debug...");
				}
				List<GenerationController.PossibleDoorwayLocation> list20 = new List<GenerationController.PossibleDoorwayLocation>();
				foreach (NewNode newNode29 in newRoom6.nodes)
				{
					if (!newNode29.tile.isStairwell && !newNode29.tile.isInvertedStairwell)
					{
						using (List<NewWall>.Enumerator enumerator2 = newNode29.walls.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								NewWall wall = enumerator2.Current;
								if (wall.otherWall != null && !wall.preset.divider && !wall.node.tile.isStairwell && !wall.node.tile.isInvertedStairwell && !wall.otherWall.node.tile.isStairwell && !wall.otherWall.node.tile.isInvertedStairwell && !wall.preventEntrance && !wall.otherWall.preventEntrance && (!newRoom6.reachableFromEntrance || wall.otherWall.node.room.roomType.allowRoomDividers || wall.otherWall.node.room.entrances.Count < wall.otherWall.node.room.roomType.maxDoors))
								{
									if (newRoom6.roomType.allowRoomDividers && wall.otherWall.node.room.roomType.allowRoomDividers)
									{
										if (newRoom6.roomDividers.Count >= newRoom6.roomType.maxDividers || wall.otherWall.node.room.roomDividers.Count >= wall.otherWall.node.room.roomType.maxDividers)
										{
											continue;
										}
										if (newRoom6.roomType.onlyAllowDividersAdjoining.Count <= 0 || newRoom6.roomType.onlyAllowDividersAdjoining.Contains(wall.otherWall.node.room.roomType))
										{
											if (wall.otherWall.node.room.roomType.onlyAllowDividersAdjoining.Count <= 0 || newRoom6.roomType.onlyAllowDividersAdjoining.Contains(newRoom6.roomType))
											{
												if (wall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall && wall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
												{
													continue;
												}
											}
											else if (wall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall)
											{
												continue;
											}
										}
										else if (wall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall)
										{
											continue;
										}
									}
									else if (wall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall)
									{
										continue;
									}
									if (wall.otherWall.node.room.gameLocation == ad && wall.otherWall.node.room != newRoom6 && wall.otherWall.node.room.roomType != InteriorControls.Instance.nullRoomType)
									{
										List<NewWall> list21 = new List<NewWall>();
										if (newRoom6.roomType.allowRoomDividers && wall.otherWall.node.room.roomType.allowRoomDividers && newRoom6.roomDividers.Count < newRoom6.roomType.maxDividers && wall.otherWall.node.room.roomDividers.Count < wall.otherWall.node.room.roomType.maxDividers && (newRoom6.roomType.onlyAllowDividersAdjoining.Count <= 0 || newRoom6.roomType.onlyAllowDividersAdjoining.Contains(wall.otherWall.node.room.roomType)) && (wall.otherWall.node.room.roomType.onlyAllowDividersAdjoining.Count <= 0 || newRoom6.roomType.onlyAllowDividersAdjoining.Contains(newRoom6.roomType)))
										{
											list21.Add(wall);
											Predicate<NewWall> <>9__8;
											foreach (Vector2Int offset in CityData.Instance.offsetArrayX4)
											{
												NewNode newNode30 = null;
												if (this.GetAdjacentNode(newNode29, offset, out newNode30) && newNode30.room == newNode29.room)
												{
													List<NewWall> walls = newNode30.walls;
													Predicate<NewWall> predicate3;
													if ((predicate3 = <>9__8) == null)
													{
														predicate3 = (<>9__8 = ((NewWall item) => item.wallOffset == wall.wallOffset && !item.preset.divider && !item.node.tile.isStairwell && !item.node.tile.isInvertedStairwell && !item.otherWall.node.tile.isStairwell && !item.otherWall.node.tile.isInvertedStairwell));
													}
													NewWall newWall5 = walls.Find(predicate3);
													if (newWall5 != null && (newWall5.otherWall.node.room == wall.otherWall.node.room || newWall5.node.room == wall.otherWall.node.room) && !newWall5.preventEntrance && !newWall5.otherWall.preventEntrance && !list21.Contains(newWall5))
													{
														list21.Add(newWall5);
													}
												}
											}
										}
										if ((!newRoom6.reachableFromEntrance || wall.otherWall.node.room.entrances.Count < wall.otherWall.node.room.roomType.maxDoors || list21.Count >= 2) && (newRoom6.roomType.mustAdjoinRooms.Count <= 0 || newRoom6.roomType.mustAdjoinRooms.Contains(wall.otherWall.node.room.roomType)))
										{
											List<NewNode.NodeAccess> list22 = newRoom6.entrances.FindAll((NewNode.NodeAccess item) => item.toNode.room == wall.node.room);
											List<NewNode.NodeAccess> list23 = newRoom6.entrances.FindAll((NewNode.NodeAccess item) => item.toNode.room == wall.otherWall.node.room);
											list22.AddRange(list23);
											if (list22.Count <= 0 || list21.Count >= 2)
											{
												GenerationController.PossibleDoorwayLocation possibleDoorwayLocation2 = null;
												if (list21.Count < 2 && wall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
												{
													possibleDoorwayLocation2 = new GenerationController.PossibleDoorwayLocation();
													possibleDoorwayLocation2.wall = wall;
													int num17 = newRoom6.roomType.mustAdjoinRooms.FindIndex((RoomTypePreset item) => item == wall.otherWall.node.room.roomType);
													if (num17 > -1)
													{
														possibleDoorwayLocation2.ranking = (float)((6 - num17) * 3) + Toolbox.Instance.RandContained(0f, 0.1f, seedInput, out seedInput);
													}
													else
													{
														possibleDoorwayLocation2.ranking = Toolbox.Instance.RandContained(0f, 0.1f, seedInput, out seedInput);
													}
													if (!wall.node.room.reachableFromEntrance && wall.otherWall.node.room.reachableFromEntrance)
													{
														possibleDoorwayLocation2.ranking += 100f;
													}
													else if (wall.node.room.reachableFromEntrance && !wall.otherWall.node.room.reachableFromEntrance)
													{
														possibleDoorwayLocation2.ranking += 100f;
													}
													List<NewWall> list24 = newNode29.walls.FindAll((NewWall item) => item.preset.sectionClass > DoorPairPreset.WallSectionClass.wall);
													list24.AddRange(wall.otherWall.node.walls.FindAll((NewWall item) => item.preset.sectionClass > DoorPairPreset.WallSectionClass.wall));
													possibleDoorwayLocation2.ranking += (float)wall.otherWall.node.walls.Count * 0.2f;
													float num18 = -1f;
													foreach (NewNode newNode31 in list2)
													{
														float num19 = Vector2.Distance(newNode31.position, wall.position);
														num18 = Mathf.Max(num18, num19);
													}
													possibleDoorwayLocation2.ranking += num18;
													using (List<NewWall>.Enumerator enumerator4 = list24.GetEnumerator())
													{
														while (enumerator4.MoveNext())
														{
															if (enumerator4.Current.wallOffset == wall.wallOffset * -1f)
															{
																possibleDoorwayLocation2.ranking -= 0.5f;
															}
															else
															{
																possibleDoorwayLocation2.ranking -= 1f;
																possibleDoorwayLocation2.requireFlatDoorway = true;
															}
														}
													}
													list20.Add(possibleDoorwayLocation2);
												}
												else
												{
													possibleDoorwayLocation2 = new GenerationController.PossibleDoorwayLocation();
													possibleDoorwayLocation2.wall = wall;
													possibleDoorwayLocation2.roomDivider = list21;
													possibleDoorwayLocation2.ranking = (float)(list21.Count * 10);
													list20.Add(possibleDoorwayLocation2);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (newRoom6.debugController != null)
				{
					GenerationDebugController debugController4 = newRoom6.debugController;
					string text3 = "Possible doorways count: ";
					int i = list20.Count;
					debugController4.Log(text3 + i.ToString());
				}
				list20.Sort();
				list20.Reverse();
				foreach (GenerationController.PossibleDoorwayLocation possibleDoorwayLocation3 in list20)
				{
					if (newRoom6.debugController != null)
					{
						newRoom6.debugController.Log(string.Concat(new string[]
						{
							possibleDoorwayLocation3.ranking.ToString(),
							" - ",
							possibleDoorwayLocation3.wall.node.room.name,
							" <-> ",
							possibleDoorwayLocation3.wall.otherWall.node.room.name
						}));
					}
				}
				bool flag8 = false;
				int num20 = 99;
				while (list20.Count > 0 && num20 > 0)
				{
					GenerationController.PossibleDoorwayLocation newDoorway = list20[0];
					if (!flag8 && (newDoorway.roomDivider == null || newDoorway.roomDivider.Count < 2))
					{
						if (newRoom6.debugController != null)
						{
							GenerationDebugController debugController5 = newRoom6.debugController;
							string[] array2 = new string[8];
							array2[0] = "Picked doorway on node: ";
							int num21 = 1;
							Vector2Int localTileCoord = newDoorway.wall.node.localTileCoord;
							array2[num21] = localTileCoord.ToString();
							array2[2] = " with offset of ";
							int num22 = 3;
							Vector2 wallOffset = newDoorway.wall.wallOffset;
							array2[num22] = wallOffset.ToString();
							array2[4] = " (";
							array2[5] = newDoorway.wall.node.room.name;
							array2[6] = " <-> ";
							array2[7] = newDoorway.wall.otherWall.node.room.name;
							debugController5.Log(string.Concat(array2));
						}
						if (newDoorway.requireFlatDoorway && newRoom6.gameLocation.thisAsAddress.preset.doorwaysFlat.Count > 0)
						{
							newDoorway.wall.SetDoorPairPreset(newRoom6.gameLocation.thisAsAddress.preset.doorwaysFlat[Toolbox.Instance.RandContained(0, newRoom6.gameLocation.thisAsAddress.preset.doorwaysFlat.Count, seedInput, out seedInput)], false, false, true);
						}
						else
						{
							newDoorway.wall.SetDoorPairPreset(newRoom6.gameLocation.thisAsAddress.preset.doorwaysNormal[Toolbox.Instance.RandContained(0, newRoom6.gameLocation.thisAsAddress.preset.doorwaysNormal.Count, seedInput, out seedInput)], false, false, true);
						}
						ad.generatedInteriorEntrances.Add(newDoorway.wall);
						if (!newDoorway.wall.node.room.reachableFromEntrance && newDoorway.wall.otherWall.node.room.reachableFromEntrance)
						{
							newDoorway.wall.node.room.reachableFromEntrance = true;
						}
						else if (newDoorway.wall.node.room.reachableFromEntrance && !newDoorway.wall.otherWall.node.room.reachableFromEntrance)
						{
							newDoorway.wall.otherWall.node.room.reachableFromEntrance = true;
						}
						flag8 = true;
					}
					else if (newDoorway.roomDivider != null && newDoorway.roomDivider.Count >= 2)
					{
						bool flag9 = true;
						if (newDoorway.wall.node.room.roomDividers.Exists((NewRoom.RoomDivider item) => item.fromRoom == newDoorway.wall.node.room && item.toRoom == newDoorway.wall.otherWall.node.room))
						{
							flag9 = false;
						}
						else if (newDoorway.wall.node.room.roomDividers.Exists((NewRoom.RoomDivider item) => item.fromRoom == newDoorway.wall.otherWall.node.room && item.toRoom == newDoorway.wall.node.room))
						{
							flag9 = false;
						}
						foreach (NewWall newWall6 in newDoorway.roomDivider)
						{
							if (newWall6.parentWall.preset.dividerLeft || newWall6.parentWall.preset.dividerRight)
							{
								flag9 = false;
								break;
							}
						}
						if (flag9)
						{
							if (newRoom6.debugController != null)
							{
								GenerationDebugController debugController6 = newRoom6.debugController;
								string text4 = "Picked divider on node: ";
								Vector2Int localTileCoord = newDoorway.wall.node.localTileCoord;
								debugController6.Log(text4 + localTileCoord.ToString());
							}
							List<NewWall> list25 = null;
							if (newDoorway.roomDivider[0].wallOffset.y > 0f)
							{
								list25 = Enumerable.ToList<NewWall>(Enumerable.OrderBy<NewWall, int>(newDoorway.roomDivider, (NewWall item1) => item1.node.floorCoord.x));
							}
							else if (newDoorway.roomDivider[0].wallOffset.y < 0f)
							{
								list25 = Enumerable.ToList<NewWall>(Enumerable.OrderBy<NewWall, int>(newDoorway.roomDivider, (NewWall item1) => item1.node.floorCoord.x));
								list25.Reverse();
							}
							else if (newDoorway.roomDivider[0].wallOffset.x > 0f)
							{
								list25 = Enumerable.ToList<NewWall>(Enumerable.OrderBy<NewWall, int>(newDoorway.roomDivider, (NewWall item1) => item1.node.floorCoord.y));
								list25.Reverse();
							}
							else if (newDoorway.roomDivider[0].wallOffset.x < 0f)
							{
								list25 = Enumerable.ToList<NewWall>(Enumerable.OrderBy<NewWall, int>(newDoorway.roomDivider, (NewWall item1) => item1.node.floorCoord.y));
							}
							for (int num23 = 0; num23 < list25.Count; num23++)
							{
								NewWall newWall7 = list25[num23];
								if (num23 == 0)
								{
									if (newWall7.parentWall == newWall7)
									{
										newWall7.SetDoorPairPreset(newRoom6.gameLocation.thisAsAddress.preset.roomDividersLeft[Toolbox.Instance.RandContained(0, newRoom6.gameLocation.thisAsAddress.preset.roomDividersLeft.Count, seedInput, out seedInput)], false, true, true);
									}
									else
									{
										newWall7.SetDoorPairPreset(newRoom6.gameLocation.thisAsAddress.preset.roomDividersRight[Toolbox.Instance.RandContained(0, newRoom6.gameLocation.thisAsAddress.preset.roomDividersRight.Count, seedInput, out seedInput)], false, true, true);
									}
								}
								else if (num23 >= list25.Count - 1)
								{
									if (newWall7.parentWall == newWall7)
									{
										newWall7.SetDoorPairPreset(newRoom6.gameLocation.thisAsAddress.preset.roomDividersRight[Toolbox.Instance.RandContained(0, newRoom6.gameLocation.thisAsAddress.preset.roomDividersRight.Count, seedInput, out seedInput)], false, true, true);
									}
									else
									{
										newWall7.SetDoorPairPreset(newRoom6.gameLocation.thisAsAddress.preset.roomDividersLeft[Toolbox.Instance.RandContained(0, newRoom6.gameLocation.thisAsAddress.preset.roomDividersLeft.Count, seedInput, out seedInput)], false, true, true);
									}
								}
								else
								{
									newWall7.SetDoorPairPreset(newRoom6.gameLocation.thisAsAddress.preset.roomDividersCentre[Toolbox.Instance.RandContained(0, newRoom6.gameLocation.thisAsAddress.preset.roomDividersCentre.Count, seedInput, out seedInput)], false, true, true);
								}
								ad.generatedInteriorEntrances.Add(newWall7);
							}
							NewRoom.RoomDivider roomDivider = new NewRoom.RoomDivider();
							roomDivider.fromRoom = newDoorway.wall.node.room;
							roomDivider.toRoom = newDoorway.wall.otherWall.node.room;
							roomDivider.dividerWalls = list25;
							newDoorway.wall.node.room.roomDividers.Add(roomDivider);
							NewRoom.RoomDivider roomDivider2 = new NewRoom.RoomDivider();
							roomDivider2.fromRoom = newDoorway.wall.otherWall.node.room;
							roomDivider2.toRoom = newDoorway.wall.node.room;
							roomDivider2.dividerWalls = list25;
							newDoorway.wall.otherWall.node.room.roomDividers.Add(roomDivider2);
							if (!newDoorway.wall.node.room.reachableFromEntrance && newDoorway.wall.otherWall.node.room.reachableFromEntrance)
							{
								newDoorway.wall.node.room.reachableFromEntrance = true;
							}
							else if (newDoorway.wall.node.room.reachableFromEntrance && !newDoorway.wall.otherWall.node.room.reachableFromEntrance)
							{
								newDoorway.wall.otherWall.node.room.reachableFromEntrance = true;
							}
						}
					}
					list20.RemoveAt(0);
					num20--;
				}
				for (int num24 = 0; num24 < newRoom6.roomDividers.Count; num24++)
				{
					NewRoom.RoomDivider div = newRoom6.roomDividers[num24];
					List<NewNode.NodeAccess> list26 = newRoom6.entrances.FindAll((NewNode.NodeAccess item) => ((item.fromNode.room == div.fromRoom && item.toNode.room == div.toRoom) || (item.toNode.room == div.fromRoom && item.fromNode.room == div.toRoom)) && !item.wall.preset.divider);
					while (list26.Count > 0)
					{
						if (newRoom6.debugController != null)
						{
							GenerationDebugController debugController7 = newRoom6.debugController;
							string text5 = "Removing regular entrance because of existing divider: ";
							Vector2Int localTileCoord = list26[0].wall.node.localTileCoord;
							debugController7.Log(text5 + localTileCoord.ToString());
						}
						list26[0].wall.parentWall.SetDoorPairPreset(CityControls.Instance.defaultWalls, true, false, true);
						list26[0].wall.node.room.RemoveEntrance(list26[0].fromNode, list26[0].toNode);
						list26[0].wall.node.room.RemoveEntrance(list26[0].toNode, list26[0].fromNode);
						list26[0].wall.otherWall.node.room.RemoveEntrance(list26[0].fromNode, list26[0].toNode);
						list26[0].wall.otherWall.node.room.RemoveEntrance(list26[0].toNode, list26[0].fromNode);
						list26.RemoveAt(0);
					}
				}
			}
		}
		unreachableRooms = this.GetUnreachableRooms(list2, ad);
		for (int num25 = 0; num25 < ad.rooms.Count; num25++)
		{
			NewRoom newRoom7 = ad.rooms[num25];
			if (!(newRoom7.roomType == InteriorControls.Instance.nullRoomType) && !newRoom7.roomType.forceNoDoors && !(newRoom7 == ad.nullRoom) && !newRoom7.gameLocation.isLobby && unreachableRooms.Contains(newRoom7))
			{
				Game.Log("Removing " + newRoom7.roomType.name + " as it is unreachable from any entrance...", 2);
				while (newRoom7.nodes.Count > 0)
				{
					ad.nullRoom.AddNewNode(Enumerable.First<NewNode>(newRoom7.nodes));
				}
				if (newRoom7.debugController != null)
				{
					newRoom7.debugController.Log("Removing room as it has no connection with entrance");
				}
				ad.rooms.RemoveAt(num25);
				Object.Destroy(newRoom7.gameObject);
				num25--;
			}
		}
		for (int num26 = 0; num26 < ad.rooms.Count; num26++)
		{
			NewRoom newRoom8 = ad.rooms[num26];
			if (!(newRoom8 == ad.nullRoom) && newRoom8.nodes.Count <= 0)
			{
				if (newRoom8.debugController != null)
				{
					newRoom8.debugController.Log("Removing room as it now has 0 nodes");
				}
				ad.rooms.RemoveAt(num26);
				Object.Destroy(newRoom8.gameObject);
				num26--;
			}
		}
		this.UpdateGeometryFloor(ad.floor, "");
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x00141A64 File Offset: 0x0013FC64
	public void ResetLayout(NewAddress ad, out GameObject newDebugParent)
	{
		bool isFloorEdit = SessionData.Instance.isFloorEdit;
		ad.generatedRoomConfigs = false;
		ad.RemoveAllInhabitantFurniture(true, FurnitureClusterLocation.RemoveInteractablesOption.remove);
		foreach (NewRoom newRoom in ad.rooms)
		{
			newRoom.blockedAccess = new Dictionary<NewNode, List<NewNode>>();
			newRoom.furniture = new List<FurnitureClusterLocation>();
			newRoom.individualFurniture = new List<FurnitureLocation>();
			newRoom.specialCaseInteractables = new Dictionary<InteractablePreset.SpecialCase, List<Interactable>>();
			newRoom.airVents = new List<AirDuctGroup.AirVent>();
			newRoom.noAccessNodes = new List<NewNode>();
			newRoom.pickFurnitureCache = null;
			newRoom.localizedRoomNodeMaps = null;
		}
		foreach (NewWall newWall in ad.generatedInteriorEntrances)
		{
			if (newWall != null)
			{
				newWall.SetDoorPairPreset(CityControls.Instance.defaultWalls, false, false, true);
			}
		}
		ad.generatedInteriorEntrances.Clear();
		ad.protectedNodes.Clear();
		for (int i = 0; i < ad.rooms.Count; i++)
		{
			NewRoom newRoom2 = ad.rooms[i];
			newRoom2.roomDividers.Clear();
			newRoom2.reachableFromEntrance = false;
			newRoom2.openPlanElements.Clear();
			if (!(newRoom2 == ad.nullRoom))
			{
				foreach (NewNode newNode in Enumerable.ToArray<NewNode>(newRoom2.nodes))
				{
					newNode.accessToOtherNodes.Clear();
					ad.nullRoom.AddNewNode(newNode);
				}
				ad.RemoveRoom(newRoom2);
				if (newRoom2.gameLocation != null)
				{
					newRoom2.gameLocation.RemoveRoom(newRoom2);
				}
				Object.Destroy(newRoom2.gameObject);
				i--;
			}
		}
		this.UpdateFloorCeilingFloor(ad.floor);
		this.UpdateWallsFloor(ad.floor);
		newDebugParent = null;
		if (SessionData.Instance.isFloorEdit)
		{
			if (ad.floorEditDebugParent != null)
			{
				Object.Destroy(ad.floorEditDebugParent);
			}
			newDebugParent = new GameObject();
			newDebugParent.transform.SetParent(FloorEditController.Instance.debugContainer.transform);
			newDebugParent.transform.localPosition = Vector3.zero;
			newDebugParent.name = ad.name;
			ad.floorEditDebugParent = newDebugParent;
		}
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x00141CD8 File Offset: 0x0013FED8
	private HashSet<NewRoom> GetUnreachableRooms(List<NewNode> entranceNodes, NewAddress ad)
	{
		HashSet<NewRoom> hashSet = new HashSet<NewRoom>();
		HashSet<NewRoom> hashSet2 = new HashSet<NewRoom>(ad.rooms);
		foreach (NewRoom newRoom in ad.rooms)
		{
			newRoom.reachableFromEntrance = false;
		}
		if (entranceNodes.Count <= 0 || entranceNodes == null)
		{
			return hashSet2;
		}
		foreach (NewNode newNode in entranceNodes)
		{
			if (!hashSet.Contains(newNode.room))
			{
				hashSet.Add(newNode.room);
			}
		}
		int num = 999;
		while (hashSet.Count > 0 && num > 0)
		{
			NewRoom newRoom2 = Enumerable.FirstOrDefault<NewRoom>(hashSet);
			newRoom2.reachableFromEntrance = true;
			hashSet2.Remove(newRoom2);
			foreach (NewNode newNode2 in newRoom2.nodes)
			{
				foreach (NewWall newWall in newNode2.walls)
				{
					if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
					{
						NewRoom room = newWall.otherWall.node.room;
						if (room == newRoom2)
						{
							room = newWall.node.room;
						}
						if (room.gameLocation == ad && !hashSet.Contains(room) && hashSet2.Contains(room))
						{
							hashSet.Add(room);
						}
					}
				}
			}
			hashSet.Remove(newRoom2);
			num--;
		}
		if (Game.Instance.devMode && hashSet2.Count > 0)
		{
			string text = string.Empty;
			foreach (NewRoom newRoom3 in hashSet2)
			{
				text = string.Concat(new string[]
				{
					text,
					newRoom3.roomType.name,
					"(",
					newRoom3.nodes.Count.ToString(),
					"), "
				});
			}
			Game.Log(string.Concat(new string[]
			{
				ad.name,
				": Unreachable rooms: ",
				hashSet2.Count.ToString(),
				"/",
				ad.rooms.Count.ToString(),
				" = ",
				text
			}), 2);
		}
		return hashSet2;
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x00141FC8 File Offset: 0x001401C8
	private List<GenerationController.PossibleRoomLocation> GetPossibleRoomLocations(NewAddress address, RoomTypePreset config, List<NewNode> possibleNodes, List<NewNode> entranceNodes, List<NewNode> mainEntranceNodes, List<NewNode> edgeNodes, Transform debugParent)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.SetParent(debugParent);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.name = config.name;
		List<GenerationController.PossibleRoomLocation> list = new List<GenerationController.PossibleRoomLocation>();
		List<Vector2> list2 = new List<Vector2>();
		for (int i = (int)config.minimumRoomAreaShape.x; i <= (int)config.maximumRoomAreaShape.x; i++)
		{
			for (int j = (int)config.minimumRoomAreaShape.y; j <= (int)config.maximumRoomAreaShape.y; j++)
			{
				Vector2 vector;
				vector..ctor((float)i, (float)j);
				if (!list2.Contains(vector))
				{
					list2.Add(vector);
				}
				if (vector.x != vector.y)
				{
					Vector2 vector2;
					vector2..ctor((float)j, (float)i);
					if (!list2.Contains(vector2))
					{
						list2.Add(vector2);
					}
				}
			}
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		List<NewNode> list3 = new List<NewNode>();
		int num4 = 999999;
		int num5 = -999999;
		int num6 = 999999;
		int num7 = -999999;
		foreach (NewNode newNode in possibleNodes)
		{
			num4 = Mathf.Min(newNode.floorCoord.x, num4);
			num5 = Mathf.Max(newNode.floorCoord.x, num5);
			num6 = Mathf.Min(newNode.floorCoord.y, num6);
			num7 = Mathf.Max(newNode.floorCoord.y, num7);
		}
		for (int k = num4; k <= num5; k++)
		{
			for (int l = num6; l <= num7; l++)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(k, l);
				NewNode newNode2 = null;
				if (address.floor.nodeMap.TryGetValue(vector2Int, ref newNode2) && !list3.Contains(newNode2))
				{
					list3.Add(newNode2);
				}
			}
		}
		for (int m = 0; m < list2.Count; m++)
		{
			Vector2 vector3 = list2[m];
			foreach (NewNode newNode3 in list3)
			{
				GenerationDebugController component = Object.Instantiate<GameObject>(PrefabControls.Instance.debugAttemptObject, gameObject.transform).GetComponent<GenerationDebugController>();
				GenerationDebugController generationDebugController = component;
				string[] array = new string[5];
				array[0] = config.name;
				array[1] = " ";
				int num8 = 2;
				Vector2 vector4 = vector3;
				array[num8] = vector4.ToString();
				array[3] = " @ ";
				int num9 = 4;
				Vector3Int nodeCoord = newNode3.nodeCoord;
				array[num9] = nodeCoord.ToString();
				generationDebugController.Setup(string.Concat(array), config);
				Dictionary<NewRoom, List<NewNode>> dictionary = new Dictionary<NewRoom, List<NewNode>>();
				List<NewNode> list4 = new List<NewNode>();
				Vector2 minimumRoomAreaShape = config.minimumRoomAreaShape;
				string seedInput = string.Empty;
				if (SessionData.Instance.isFloorEdit)
				{
					seedInput = Toolbox.Instance.GenerateUniqueID();
				}
				else
				{
					seedInput = newNode3.nodeCoord.ToString();
				}
				float psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, InteriorControls.Instance.roomRankingRandomThreshold, seedInput, out seedInput);
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				int num14 = 0;
				while ((float)num14 < vector3.x)
				{
					int num15 = 0;
					while ((float)num15 < vector3.y)
					{
						Vector2Int vector2Int2 = newNode3.floorCoord + new Vector2Int(num14, num15);
						NewNode newNode4 = null;
						if (address.floor.nodeMap.TryGetValue(vector2Int2, ref newNode4))
						{
							if (newNode4.gameLocation != address)
							{
								component.attemptedInvalidNodes.Add(newNode4, "Different address");
							}
							else if (!config.allowMainAddressEntrance && mainEntranceNodes.Contains(newNode4))
							{
								component.attemptedInvalidNodes.Add(newNode4, "Not allowed main address entrance");
							}
							else if (!config.allowSecondaryAddressEntrance && entranceNodes.Contains(newNode4) && !mainEntranceNodes.Contains(newNode4))
							{
								component.attemptedInvalidNodes.Add(newNode4, "Not allowed secondary address entrance");
							}
							else
							{
								bool flag;
								if (newNode4.room == address.nullRoom || newNode4.room.roomType == InteriorControls.Instance.nullRoomType)
								{
									if (newNode4.tile.isStairwell || newNode4.tile.isInvertedStairwell)
									{
										if (minimumRoomAreaShape.x <= 3f)
										{
											minimumRoomAreaShape..ctor(3f, Mathf.Max(minimumRoomAreaShape.x, 4f));
										}
										else if (minimumRoomAreaShape.y <= 4f)
										{
											minimumRoomAreaShape..ctor(Mathf.Max(minimumRoomAreaShape.x, 3f), 4f);
										}
									}
									flag = true;
								}
								else
								{
									if (!newNode4.room.roomType.overridable)
									{
										component.attemptedInvalidNodes.Add(newNode4, "OVR " + newNode4.room.roomType.name + " Non -overridable room: " + newNode4.room.roomType.name);
										goto IL_918;
									}
									if (address.protectedNodes.Contains(newNode4))
									{
										component.attemptedInvalidNodes.Add(newNode4, "OVR " + newNode4.room.roomType.name + " Protected node: " + newNode4.name);
										goto IL_918;
									}
									if (newNode4.room.roomType.cyclePriority > config.overwriteWithPriorityUpTo)
									{
										component.attemptedInvalidNodes.Add(newNode4, "OVR " + newNode4.room.roomType.name + " Lower overwrite priority than: " + newNode4.room.roomType.name);
										goto IL_918;
									}
									if (newNode4.room.roomType.blockOverridesFromType.Contains(config))
									{
										component.attemptedInvalidNodes.Add(newNode4, "OVR " + newNode4.room.roomType.name + " Room blocks overwrite from type " + newNode4.room.roomType.ToString());
										goto IL_918;
									}
									if (!dictionary.ContainsKey(newNode4.room))
									{
										dictionary.Add(newNode4.room, new List<NewNode>());
									}
									List<NewNode> list5 = new List<NewNode>(newNode4.room.nodes);
									list5.Remove(newNode4);
									foreach (NewNode newNode5 in dictionary[newNode4.room])
									{
										list5.Remove(newNode5);
									}
									if (!this.RoomMinimumShapeCheck(ref list5, newNode4.room.roomType.minimumRoomAreaShape, component))
									{
										component.attemptedInvalidNodes.Add(newNode4, "OVR " + newNode4.room.roomType.name + " Minimum room shape fail: " + newNode4.room.roomType.name);
										goto IL_918;
									}
									List<NewNode> list6;
									if (!this.MustAdjoinOneOfCheck(ref list5, newNode4.room.gameLocation, newNode4.room.roomType.mustAdjoinRooms, newNode4.room.roomType.mustConnectWithEntrance, out list6, component))
									{
										component.attemptedInvalidNodes.Add(newNode4, "OVR " + newNode4.room.roomType.name + " Adjoining fail: " + newNode4.room.roomType.name);
										goto IL_918;
									}
									if (newNode4.room.roomType.mustConnectWithEntrance && !this.CheckEntranceConnection(ref list5, newNode4.room.gameLocation, component))
									{
										component.attemptedInvalidNodes.Add(newNode4, "OVR " + newNode4.room.roomType.name + " entrance connect fail: " + newNode4.room.roomType.name);
										goto IL_918;
									}
									dictionary[newNode4.room].Add(newNode4);
									num13++;
									flag = true;
									component.overridenNodes.Add(newNode4, newNode4.room.roomType.name);
								}
								if (flag)
								{
									NewNode newNode6 = null;
									foreach (Vector2Int offset in CityData.Instance.offsetArrayX4)
									{
										if (this.GetAdjacentNode(newNode4, offset, out newNode6) && newNode6.gameLocation != newNode4.gameLocation)
										{
											num11++;
											if (newNode4.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.window || item.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge))
											{
												num10++;
											}
											else if (newNode4.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance && item.otherWall.node.gameLocation != item.node.gameLocation))
											{
												num12++;
												if (newNode4.room.roomType.preferMainAddressEntrance)
												{
													if (newNode4.walls.Exists((NewWall item) => (item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance && item.parentWall.node.isOutside) || item.childWall.node.isOutside))
													{
														num12++;
													}
												}
											}
										}
									}
									list4.Add(newNode4);
								}
							}
						}
						IL_918:
						num15++;
					}
					num14++;
				}
				component.attemptedValidNodes = list4;
				if (this.RoomMinimumShapeCheck(ref list4, minimumRoomAreaShape, component) && this.TesselationShapeCheck(ref list4, config.tesselationShape, component))
				{
					if (!this.RoomSplitCheck(ref list4, component))
					{
						num2++;
					}
					else
					{
						List<NewNode> requiredAdjoiningOptions = null;
						if (this.MustAdjoinOneOfCheck(ref list4, address, config.mustAdjoinRooms, config.mustConnectWithEntrance, out requiredAdjoiningOptions, component))
						{
							bool flag2 = true;
							List<GenerationController.OverrideData> list7 = new List<GenerationController.OverrideData>();
							foreach (KeyValuePair<NewRoom, List<NewNode>> keyValuePair in dictionary)
							{
								List<NewNode> list8 = new List<NewNode>(keyValuePair.Key.nodes);
								foreach (NewNode newNode7 in keyValuePair.Value)
								{
									list8.Remove(newNode7);
								}
								if (keyValuePair.Key.preset != address.preset.hallway && !this.RoomSplitCheck(ref list8, component))
								{
									flag2 = false;
									num++;
									break;
								}
								if (!this.TesselationShapeCheck(ref list8, keyValuePair.Key.roomType.tesselationShape, component))
								{
									flag2 = false;
									break;
								}
								int num16 = 0;
								int num17 = 0;
								foreach (NewNode newNode8 in keyValuePair.Value)
								{
									NewNode newNode9 = null;
									foreach (Vector2Int offset2 in CityData.Instance.offsetArrayX4)
									{
										if (this.GetAdjacentNode(newNode8, offset2, out newNode9) && newNode9.gameLocation != newNode8.gameLocation)
										{
											if (newNode8.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.window || item.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge))
											{
												num16++;
											}
											num17++;
										}
									}
								}
								GenerationController.OverrideData overrideData = default(GenerationController.OverrideData);
								overrideData.room = keyValuePair.Key;
								overrideData.floorSpacePenalty = (float)keyValuePair.Value.Count * 0.5f * (float)keyValuePair.Key.roomType.floorSpaceWeight * -1f;
								overrideData.exteriorWindowPenalty = (float)num16 * 0.5f * (float)keyValuePair.Key.roomType.exteriorWindowWeight * -1f;
								overrideData.exteriorWallPenalty = (float)num17 * 0.1f * (float)keyValuePair.Key.roomType.exteriorWallWeight * -1f;
								overrideData.overridingPenalty = overrideData.floorSpacePenalty + overrideData.exteriorWindowPenalty + overrideData.exteriorWallPenalty;
								list7.Add(overrideData);
							}
							if (flag2)
							{
								List<NewNode> list9 = null;
								if (config.mustConnectWithEntrance && !this.CheckEntranceConnection(ref list4, address, component))
								{
									bool flag3 = false;
									if (address.preset.requiresHallway && address.preset.hallway != null)
									{
										if (mainEntranceNodes.Count > 0)
										{
											using (List<NewNode>.Enumerator enumerator2 = mainEntranceNodes.GetEnumerator())
											{
												while (enumerator2.MoveNext())
												{
													NewNode newNode10 = enumerator2.Current;
													if (!(newNode10.room.roomType != InteriorControls.Instance.nullRoomType))
													{
														list9 = this.HallwayPathfind(newNode10, list4[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list4.Count, seedInput, out seedInput)], address);
														if (list9 != null && list9.Count > 0)
														{
															flag3 = true;
															break;
														}
													}
												}
												goto IL_D84;
											}
										}
										foreach (NewNode newNode11 in entranceNodes)
										{
											if (!(newNode11.room.roomType != InteriorControls.Instance.nullRoomType))
											{
												list9 = this.HallwayPathfind(newNode11, list4[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list4.Count, seedInput, out seedInput)], address);
												if (list9 != null && list9.Count > 0)
												{
													flag3 = true;
													break;
												}
											}
										}
									}
									IL_D84:
									if (entranceNodes.Count <= 0)
									{
										flag3 = true;
									}
									if (!flag3)
									{
										Game.Log("New room would not connect with the entrance.", 2);
										num3++;
										continue;
									}
								}
								float num18 = minimumRoomAreaShape.x * minimumRoomAreaShape.y;
								float num19 = ((float)list4.Count - num18) * 0.5f * (float)config.floorSpaceWeight;
								float num20 = (float)num10 * 0.5f * (float)config.exteriorWindowWeight;
								float num21 = (float)num11 * 0.1f * (float)config.exteriorWallWeight;
								float num22 = (float)num12 * 0.5f * (float)config.entranceWeight;
								GenerationController.PossibleRoomLocation possibleRoomLocation = new GenerationController.PossibleRoomLocation();
								possibleRoomLocation.nodes.AddRange(list4);
								possibleRoomLocation.exteriorWindowRanking = num20;
								possibleRoomLocation.exteriorWallsRanking = num21;
								possibleRoomLocation.floorSpaceRanking = num19;
								possibleRoomLocation.entrancesRanking = num22;
								possibleRoomLocation.randomRanking = psuedoRandomNumberContained;
								possibleRoomLocation.overrideRankingData = list7;
								possibleRoomLocation.debugScript = component;
								possibleRoomLocation.ranking = num20 + num19 + num22 + psuedoRandomNumberContained + num21;
								foreach (GenerationController.OverrideData overrideData2 in list7)
								{
									possibleRoomLocation.ranking += overrideData2.overridingPenalty;
								}
								possibleRoomLocation.requiredAdjoiningOptions = requiredAdjoiningOptions;
								possibleRoomLocation.requiredHallway = list9;
								list.Add(possibleRoomLocation);
								component.location = possibleRoomLocation;
								component.valid = true;
								component.name = "-" + component.name + " : " + possibleRoomLocation.ranking.ToString();
								component.gameObject.transform.SetAsFirstSibling();
							}
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x00143004 File Offset: 0x00141204
	private bool RoomMinimumShapeCheck(ref List<NewNode> nodes, Vector2 minimumShape, GenerationDebugController debug)
	{
		if (minimumShape.x + minimumShape.y <= 0f)
		{
			return true;
		}
		foreach (NewNode newNode in nodes)
		{
			bool flag = true;
			int num = 1;
			if (minimumShape.x != minimumShape.y)
			{
				num = 2;
			}
			for (int i = 0; i < num; i++)
			{
				Vector2 vector = minimumShape;
				if (i == 1)
				{
					vector..ctor(minimumShape.y, minimumShape.x);
				}
				flag = true;
				int num2 = 0;
				while ((float)num2 < vector.x)
				{
					int num3 = 0;
					while ((float)num3 < vector.y)
					{
						Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(num2, num3, 0);
						NewNode newNode2 = null;
						if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2))
						{
							flag = false;
							break;
						}
						if (!nodes.Contains(newNode2))
						{
							flag = false;
							break;
						}
						num3++;
					}
					if (!flag)
					{
						break;
					}
					num2++;
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				if (debug != null)
				{
					debug.Log("Passed minimum room shape check...");
				}
				return true;
			}
		}
		if (debug != null)
		{
			debug.Log("Failed minimum room shape check...");
		}
		return false;
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x00143160 File Offset: 0x00141360
	private bool RoomMinimumShapeCheck(ref HashSet<NewNode> nodes, Vector2 minimumShape, GenerationDebugController debug, bool nodesMustBeUnoccupied = false)
	{
		if (minimumShape.x + minimumShape.y <= 0f)
		{
			return true;
		}
		foreach (NewNode newNode in nodes)
		{
			bool flag = true;
			int num = 1;
			if (minimumShape.x != minimumShape.y)
			{
				num = 2;
			}
			for (int i = 0; i < num; i++)
			{
				Vector2 vector = minimumShape;
				if (i == 1)
				{
					vector..ctor(minimumShape.y, minimumShape.x);
				}
				flag = true;
				int num2 = 0;
				while ((float)num2 < vector.x)
				{
					int num3 = 0;
					while ((float)num3 < vector.y)
					{
						Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(num2, num3, 0);
						NewNode newNode2 = null;
						if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2))
						{
							flag = false;
							break;
						}
						if (nodesMustBeUnoccupied && !newNode2.allowNewFurniture)
						{
							flag = false;
							break;
						}
						if (!nodes.Contains(newNode2))
						{
							flag = false;
							break;
						}
						num3++;
					}
					if (!flag)
					{
						break;
					}
					num2++;
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				if (debug != null)
				{
					debug.Log("Passed minimum room shape check...");
				}
				return true;
			}
		}
		if (debug != null)
		{
			debug.Log("Failed minimum room shape check...");
		}
		return false;
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x001432DC File Offset: 0x001414DC
	private bool TesselationShapeCheck(ref List<NewNode> nodes, Vector2 tessShape, GenerationDebugController debugController)
	{
		if (tessShape.x + tessShape.y <= 0f)
		{
			return true;
		}
		List<NewNode> list = new List<NewNode>();
		foreach (NewNode newNode in nodes)
		{
			int num = 1;
			if (tessShape.x != tessShape.y)
			{
				num = 2;
			}
			for (int i = 0; i < num; i++)
			{
				Vector2 vector = tessShape;
				if (i == 1)
				{
					vector..ctor(tessShape.y, tessShape.x);
				}
				bool flag = true;
				List<NewNode> list2 = new List<NewNode>();
				int num2 = 0;
				while ((float)num2 < vector.x)
				{
					int num3 = 0;
					while ((float)num3 < vector.y)
					{
						Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(num2, num3, 0);
						NewNode newNode2 = null;
						if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2))
						{
							flag = false;
							break;
						}
						if (!nodes.Contains(newNode2))
						{
							flag = false;
							break;
						}
						if (!list.Contains(newNode2))
						{
							list2.Add(newNode2);
						}
						num3++;
					}
					if (!flag)
					{
						break;
					}
					num2++;
				}
				if (flag)
				{
					list.AddRange(list2);
				}
			}
		}
		if (debugController != null)
		{
			debugController.Log("...Tesselation results: " + list.Count.ToString() + "/" + nodes.Count.ToString());
		}
		return list.Count == nodes.Count;
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x00143490 File Offset: 0x00141690
	private bool MustAdjoinOneOfCheck(ref List<NewNode> nodes, NewGameLocation thisGameLocation, List<RoomTypePreset> roomTypes, bool includeEntrance, out List<NewNode> internalAdjoiningRoomNodes, GenerationDebugController debug)
	{
		bool flag = false;
		internalAdjoiningRoomNodes = new List<NewNode>();
		if (roomTypes.Count <= 0 && !includeEntrance)
		{
			debug.Log("Passed adjoin check (automatic)");
			return true;
		}
		foreach (NewNode newNode in nodes)
		{
			Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
			for (int i = 0; i < offsetArrayX.Length; i++)
			{
				Vector2Int vector2Int = offsetArrayX[i];
				Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
				NewNode scanNode = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref scanNode))
				{
					if (scanNode.gameLocation == thisGameLocation)
					{
						if (scanNode.room != null && roomTypes.Contains(scanNode.room.roomType))
						{
							flag = true;
							if (!internalAdjoiningRoomNodes.Contains(scanNode))
							{
								internalAdjoiningRoomNodes.Add(scanNode);
							}
						}
					}
					else if (includeEntrance)
					{
						NewWall newWall = newNode.walls.Find((NewWall item) => item.node == scanNode || item.otherWall.node == scanNode);
						if (newWall != null && newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
						{
							flag = true;
						}
					}
				}
			}
		}
		if (!flag)
		{
			debug.Log("Failed adjoin check...");
			return false;
		}
		debug.Log("Passed adjoin check...");
		return true;
	}

	// Token: 0x060015C2 RID: 5570 RVA: 0x00143644 File Offset: 0x00141844
	private bool CheckEntranceConnection(ref List<NewNode> nodes, NewGameLocation thisGameLocation, GenerationDebugController debug)
	{
		bool flag = false;
		foreach (NewNode newNode in nodes)
		{
			Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
			for (int i = 0; i < offsetArrayX.Length; i++)
			{
				Vector2Int vector2Int = offsetArrayX[i];
				Vector2Int vector2Int2 = newNode.floorCoord + vector2Int;
				NewNode scanNode = null;
				if (newNode.floor.nodeMap.TryGetValue(vector2Int2, ref scanNode))
				{
					if (scanNode.gameLocation == thisGameLocation)
					{
						if (scanNode.room.gameLocation.thisAsAddress != null && scanNode.room.gameLocation.thisAsAddress.preset != null && scanNode.room.gameLocation.thisAsAddress.preset.hallway != null && scanNode.room.roomType == scanNode.room.gameLocation.thisAsAddress.preset.hallway.roomType)
						{
							flag = true;
							break;
						}
					}
					else
					{
						NewWall newWall = newNode.walls.Find((NewWall item) => item.node == scanNode || item.otherWall.node == scanNode);
						if (newWall != null && newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
						{
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
		debug.Log("Check entrance connection pass: " + flag.ToString());
		return flag;
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x0014381C File Offset: 0x00141A1C
	private void CreateForcedRooms(NewAddress ad)
	{
		for (int i = 0; i < ad.nodes.Count; i++)
		{
			NewNode newNode = ad.nodes[i];
			if (newNode.forcedRoom != null && newNode.forcedRoomRef.Length > 0 && newNode.room == ad.nullRoom)
			{
				NewRoom component = Object.Instantiate<GameObject>(PrefabControls.Instance.room, ad.transform).GetComponent<NewRoom>();
				component.SetupAll(ad, newNode.forcedRoom, -1);
				List<NewNode> list = new List<NewNode>();
				list.Add(newNode);
				int num = 999;
				while (list.Count > 0 && num > 0)
				{
					NewNode newNode2 = list[0];
					component.AddNewNode(newNode2);
					ad.protectedNodes.Add(newNode2);
					foreach (Vector2Int offset in CityData.Instance.offsetArrayX4)
					{
						NewNode newNode3 = null;
						if (this.GetAdjacentNode(newNode2, offset, out newNode3) && newNode3.forcedRoom != null && newNode3.forcedRoomRef.Length > 0 && newNode3.forcedRoom == newNode.forcedRoom && newNode3.room == ad.nullRoom && !list.Contains(newNode3) && !component.nodes.Contains(newNode3))
						{
							list.Add(newNode3);
						}
					}
					list.RemoveAt(0);
					num--;
				}
			}
		}
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x001439B0 File Offset: 0x00141BB0
	private float GetRoomUniformity(List<NewNode> nodes, out int wallCount, out float shapeRatio)
	{
		int num = 0;
		Vector2 zero = Vector2.zero;
		wallCount = this.CalculateRoomEdges(nodes, out num, out zero);
		shapeRatio = Mathf.Min(zero.x, zero.y) / Mathf.Max(zero.x, zero.y);
		float num2 = (float)num / (float)wallCount;
		float num3 = (float)nodes.Count / (zero.x * zero.y);
		return num2 * num3;
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x00143A18 File Offset: 0x00141C18
	private int CalculateRoomEdges(List<NewNode> nodes, out int uniformWallCount, out Vector2 uniformBoundsSize)
	{
		int num = 0;
		uniformWallCount = 0;
		uniformBoundsSize = Vector2.zero;
		int num2 = 9999;
		int num3 = 9999;
		int num4 = -1;
		int num5 = -1;
		foreach (NewNode newNode in nodes)
		{
			num2 = Mathf.Min(num2, newNode.floorCoord.x);
			num3 = Mathf.Min(num3, newNode.floorCoord.y);
			num4 = Mathf.Max(num4, newNode.floorCoord.x);
			num5 = Mathf.Max(num5, newNode.floorCoord.y);
			for (int i = 0; i < CityData.Instance.offsetArrayX4.Length; i++)
			{
				Vector2Int vector2Int = newNode.floorCoord + CityData.Instance.offsetArrayX4[i];
				if (newNode.floor.nodeMap.ContainsKey(vector2Int))
				{
					NewNode newNode2 = newNode.floor.nodeMap[vector2Int];
					if (!nodes.Contains(newNode2))
					{
						num++;
					}
				}
				else
				{
					num++;
				}
			}
		}
		uniformBoundsSize = new Vector2((float)(num4 - num2 + 1), (float)(num5 - num3 + 1));
		uniformWallCount = Mathf.RoundToInt(uniformBoundsSize.x * 2f + uniformBoundsSize.y * 2f);
		return num;
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x00143B88 File Offset: 0x00141D88
	private bool RoomSplitCheck(ref List<NewNode> nodes, GenerationDebugController debug)
	{
		if (nodes.Count <= 0)
		{
			return false;
		}
		List<NewNode> list = new List<NewNode>();
		List<NewNode> list2 = new List<NewNode>();
		list.Add(nodes[0]);
		int num = 9999;
		while (list.Count > 0 && num > 0)
		{
			NewNode newNode = list[0];
			foreach (Vector2Int offset in CityData.Instance.offsetArrayX4)
			{
				NewNode newNode2 = null;
				if (this.GetAdjacentNode(newNode, offset, out newNode2) && nodes.Contains(newNode2) && !list.Contains(newNode2) && !list2.Contains(newNode2))
				{
					list.Add(newNode2);
				}
			}
			list2.Add(newNode);
			list.RemoveAt(0);
			num--;
		}
		if (list2.Count >= nodes.Count)
		{
			if (debug != null)
			{
				debug.Log("Passed room split check...");
			}
			return true;
		}
		if (debug != null)
		{
			debug.Log("Failed room split check...");
		}
		return false;
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x00143C88 File Offset: 0x00141E88
	private List<NewRoom> ConvertSplitRoom(ref HashSet<NewNode> nodes, NewAddress ad)
	{
		List<NewRoom> list = new List<NewRoom>();
		if (nodes.Count <= 0)
		{
			return list;
		}
		List<NewNode> list2 = new List<NewNode>(nodes);
		List<NewNode> list3 = new List<NewNode>();
		List<NewNode> list4 = new List<NewNode>();
		NewRoom newRoom = Enumerable.FirstOrDefault<NewNode>(nodes).room;
		list.Add(newRoom);
		int num = 9999;
		while (list2.Count > 0 && num > 0)
		{
			list3.Add(list2[0]);
			int num2 = 9999;
			while (list3.Count > 0 && num2 > 0)
			{
				NewNode newNode = list3[0];
				foreach (Vector2Int offset in CityData.Instance.offsetArrayX4)
				{
					NewNode newNode2 = null;
					if (this.GetAdjacentNode(newNode, offset, out newNode2) && nodes.Contains(newNode2) && !list3.Contains(newNode2) && !list4.Contains(newNode2))
					{
						list3.Add(newNode2);
					}
				}
				if (newNode.room != newRoom)
				{
					newRoom.AddNewNode(newNode);
				}
				list2.Remove(newNode);
				list4.Add(newNode);
				list3.RemoveAt(0);
				num2--;
			}
			if (list2.Count > 0)
			{
				NewRoom component = Object.Instantiate<GameObject>(PrefabControls.Instance.room, ad.transform).GetComponent<NewRoom>();
				component.SetupAll(ad, newRoom.preset, -1);
				newRoom = component;
				list.Add(component);
			}
			num--;
		}
		return list;
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x00143E04 File Offset: 0x00142004
	private List<NewNode> HallwayPathfind(NewNode origin, NewNode destination, NewAddress address)
	{
		Dictionary<NewNode, int> dictionary = new Dictionary<NewNode, int>();
		Dictionary<NewNode, int> dictionary2 = new Dictionary<NewNode, int>();
		NewWall newWall = origin.walls.Find((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance);
		Vector2Int invertedEntranceOffset = new Vector2Int(Mathf.RoundToInt(newWall.wallOffset.x * -2f), Mathf.RoundToInt(newWall.wallOffset.y * -2f));
		int num = Enumerable.ToList<Vector2Int>(CityData.Instance.offsetArrayX4).FindIndex((Vector2Int item) => item == invertedEntranceOffset);
		dictionary2.Add(origin, num);
		Dictionary<NewNode, NewNode> dictionary3 = new Dictionary<NewNode, NewNode>();
		Dictionary<NewNode, float> dictionary4 = new Dictionary<NewNode, float>();
		Dictionary<NewNode, float> dictionary5 = new Dictionary<NewNode, float>();
		if (!dictionary4.ContainsKey(origin))
		{
			dictionary4.Add(origin, 0f);
		}
		dictionary4[origin] = 0f;
		if (!dictionary5.ContainsKey(origin))
		{
			dictionary5.Add(origin, float.PositiveInfinity);
		}
		dictionary5[origin] = Toolbox.Instance.HeuristicCostEstimate(origin, destination);
		int num2 = 0;
		while (dictionary2.Count > 0)
		{
			NewNode newNode = null;
			float num3 = float.PositiveInfinity;
			int num4 = 0;
			foreach (KeyValuePair<NewNode, int> keyValuePair in dictionary2)
			{
				if (dictionary5.ContainsKey(keyValuePair.Key) && dictionary5[keyValuePair.Key] < num3)
				{
					num3 = dictionary5[keyValuePair.Key];
					newNode = keyValuePair.Key;
					num4 = keyValuePair.Value;
				}
			}
			if (newNode == destination)
			{
				return Toolbox.Instance.ConstructPathAccurate(dictionary3, newNode);
			}
			dictionary2.Remove(newNode);
			dictionary.Add(newNode, num4);
			for (int i = 0; i < CityData.Instance.offsetArrayX4.Length; i++)
			{
				NewNode newNode2 = null;
				if (this.GetAdjacentNode(newNode, CityData.Instance.offsetArrayX4[i], out newNode2) && newNode2.gameLocation == address && (newNode2.room == newNode.room || (newNode2.room.gameLocation.thisAsAddress != null && newNode2.room.preset == newNode2.room.gameLocation.thisAsAddress.preset.hallway) || newNode2.room.roomType == InteriorControls.Instance.nullRoomType))
				{
					int num5 = 0;
					if (!dictionary.TryGetValue(newNode2, ref num5))
					{
						float num6 = 1f;
						if (i == num4)
						{
							num6 = 0f;
						}
						if (newNode2.room.gameLocation.thisAsAddress != null && newNode2.room.preset == newNode2.room.gameLocation.thisAsAddress.preset.hallway)
						{
							num6 = 0f;
						}
						float num7 = dictionary4[newNode] + num6;
						if (!dictionary2.TryGetValue(newNode2, ref num5))
						{
							dictionary2.Add(newNode2, i);
						}
						if (!dictionary4.ContainsKey(newNode2) || num7 < dictionary4[newNode2])
						{
							if (!dictionary3.ContainsKey(newNode2))
							{
								dictionary3.Add(newNode2, newNode);
							}
							else
							{
								dictionary3[newNode2] = newNode;
							}
							if (!dictionary4.ContainsKey(newNode2))
							{
								dictionary4.Add(newNode2, num7);
							}
							else
							{
								dictionary4[newNode2] = num7;
							}
							if (!dictionary5.ContainsKey(newNode2))
							{
								dictionary5.Add(newNode2, dictionary4[newNode2] + Toolbox.Instance.HeuristicCostEstimate(newNode2, destination));
							}
							else
							{
								dictionary5[newNode2] = dictionary4[newNode2] + Toolbox.Instance.HeuristicCostEstimate(newNode2, destination);
							}
						}
					}
				}
			}
			num2++;
			if (num2 > 999)
			{
				return null;
			}
		}
		return null;
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x00144208 File Offset: 0x00142408
	public void GenerateGeometry(NewAddress ad)
	{
		string seedInput = string.Empty;
		if (SessionData.Instance.isFloorEdit)
		{
			seedInput = CityData.Instance.seed;
			foreach (NewRoom newRoom in ad.rooms)
			{
				foreach (FurnitureClusterLocation furnitureClusterLocation in newRoom.furniture)
				{
					furnitureClusterLocation.UnloadFurniture(true, FurnitureClusterLocation.RemoveInteractablesOption.remove);
				}
				foreach (NewNode newNode in newRoom.nodes)
				{
					foreach (Interactable interactable in newNode.interactables)
					{
						interactable.DespawnObject();
					}
					foreach (NewWall newWall in newNode.walls)
					{
						newWall.placedWallFurn = false;
					}
					newNode.ResetFurniture();
					newNode.interactables = new List<Interactable>();
					newNode.SetAllowNewFurniture(true);
					newNode.noPassThrough = false;
					foreach (NewWall newWall2 in newNode.walls)
					{
						newWall2.frontagePresets = new List<NewWall.FrontageSetting>();
					}
				}
				newRoom.blockedAccess = new Dictionary<NewNode, List<NewNode>>();
				newRoom.furniture = new List<FurnitureClusterLocation>();
				newRoom.individualFurniture = new List<FurnitureLocation>();
				newRoom.specialCaseInteractables = new Dictionary<InteractablePreset.SpecialCase, List<Interactable>>();
			}
			for (int i = 0; i < ad.owners.Count; i++)
			{
				Object.Destroy(ad.owners[i].gameObject);
				ad.owners.RemoveAt(i);
				i--;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.citizen, FloorEditController.Instance.fakeCitizenHolder);
			Citizen component = gameObject.GetComponent<Citizen>();
			component.outfitController.debugOverride = false;
			gameObject.GetComponent<NewAIController>().SetUpdateEnabled(false);
			component.SetSexualityAndGender();
			component.SetPersonality();
			ad.AddOwner(component);
			component.SetupGeneral();
			component.CalculateAge();
			ad.UpdateDesignStyle();
		}
		else
		{
			seedInput = CityData.Instance.seed + (ad.id * 2).ToString() + ad.building.cityTile.cityCoord.ToString() + ad.id.ToString();
			foreach (NewRoom newRoom2 in ad.rooms)
			{
				foreach (NewNode newNode2 in newRoom2.nodes)
				{
					foreach (NewWall newWall3 in newNode2.walls)
					{
						newWall3.placedWallFurn = false;
					}
					newNode2.ResetFurniture();
					newNode2.SetAllowNewFurniture(true);
					newNode2.noPassThrough = false;
					foreach (NewWall newWall4 in newNode2.walls)
					{
						newWall4.frontagePresets.Clear();
					}
				}
				newRoom2.blockedAccess = new Dictionary<NewNode, List<NewNode>>();
				newRoom2.furniture = new List<FurnitureClusterLocation>();
				newRoom2.individualFurniture = new List<FurnitureLocation>();
			}
			ad.UpdateDesignStyle();
		}
		using (List<NewRoom>.Enumerator enumerator = ad.rooms.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NewRoom room = enumerator.Current;
				if (!(room.preset != null) || room.preset.useMainLights || room.preset.useAdditionalAreaLights)
				{
					room.commonRooms.Clear();
					if (room.roomType.shareFeaturesWithCommonAdjacent)
					{
						room.commonRooms = ad.rooms.FindAll((NewRoom item) => item.gameLocation == room.gameLocation && item.roomType == room.roomType);
					}
					else
					{
						room.commonRooms.Add(room);
					}
					for (int j = 0; j < room.lightswitches.Count; j++)
					{
						room.lightswitches[j].SetAsLightswitch(null);
						j--;
					}
					room.mainLights.Clear();
					List<NewWall> list = new List<NewWall>();
					foreach (NewRoom newRoom3 in room.commonRooms)
					{
						foreach (NewNode newNode3 in newRoom3.nodes)
						{
							foreach (NewWall newWall5 in newNode3.walls)
							{
								if (newWall5.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
								{
									if (newWall5.otherWall.node.gameLocation != room.gameLocation || (newWall5.otherWall.node.room.gameLocation.thisAsAddress != null && newWall5.otherWall.node.room.preset == newWall5.otherWall.node.room.gameLocation.thisAsAddress.preset.hallway))
									{
										list.Insert(0, newWall5);
									}
									else
									{
										list.Add(newWall5);
									}
								}
							}
						}
					}
					if (room.preset != null && room.preset.useLightSwitches)
					{
						using (List<NewWall>.Enumerator enumerator5 = list.GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								NewWall doorway = enumerator5.Current;
								int num = 0;
								foreach (NewRoom newRoom4 in room.commonRooms)
								{
									num += newRoom4.lightswitches.Count;
								}
								if (num >= 2)
								{
									break;
								}
								bool flag = true;
								foreach (NewRoom newRoom5 in room.commonRooms)
								{
									using (List<NewWall>.Enumerator enumerator7 = newRoom5.lightswitches.GetEnumerator())
									{
										while (enumerator7.MoveNext())
										{
											if (Vector2.Distance(enumerator7.Current.node.floorCoord, doorway.node.floorCoord) < 3f)
											{
												flag = false;
												break;
											}
										}
									}
								}
								if (flag)
								{
									List<NewWall> list2 = new List<NewWall>();
									if (list2.Count <= 0)
									{
										Predicate<NewWall> <>9__1;
										for (int k = 0; k < CityData.Instance.offsetArrayX4.Length; k++)
										{
											Vector2Int vector2Int = doorway.node.floorCoord + CityData.Instance.offsetArrayX4[k];
											NewNode newNode4 = null;
											if (ad.floor.nodeMap.TryGetValue(vector2Int, ref newNode4) && (newNode4.floorType == NewNode.FloorTileType.floorAndCeiling || newNode4.floorType == NewNode.FloorTileType.floorOnly) && newNode4.gameLocation == doorway.node.gameLocation && (newNode4.room == doorway.node.room || (doorway.node.room.roomType.shareFeaturesWithCommonAdjacent && newNode4.room.roomType == doorway.node.room.roomType)))
											{
												List<NewWall> walls = newNode4.walls;
												Predicate<NewWall> predicate;
												if ((predicate = <>9__1) == null)
												{
													predicate = (<>9__1 = ((NewWall item) => item.wallOffset == doorway.wallOffset && item.preset.supportsWallProps));
												}
												NewWall newWall6 = walls.Find(predicate);
												if (newWall6 != null)
												{
													list2.Add(newWall6);
												}
											}
										}
									}
									if (list2.Count <= 0)
									{
										foreach (NewWall newWall7 in doorway.node.walls)
										{
											if (newWall7.wallOffset != doorway.wallOffset * -1f && newWall7.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance && newWall7.preset.supportsWallProps)
											{
												list2.Add(newWall7);
											}
										}
									}
									if (list2.Count <= 0)
									{
										for (int l = 0; l < CityData.Instance.offsetArrayX4.Length; l++)
										{
											Vector2Int vector2Int2 = doorway.node.floorCoord + CityData.Instance.offsetArrayX4[l];
											NewNode newNode5 = null;
											if (ad.floor.nodeMap.TryGetValue(vector2Int2, ref newNode5) && newNode5.gameLocation == doorway.node.gameLocation && (newNode5.room == doorway.node.room || (doorway.node.room.roomType.shareFeaturesWithCommonAdjacent && newNode5.room.roomType == doorway.node.room.roomType)))
											{
												NewWall newWall8 = newNode5.walls.Find((NewWall item) => item.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance && item.preset.supportsWallProps);
												if (newWall8 != null)
												{
													list2.Add(newWall8);
												}
											}
										}
									}
									if (list2.Count <= 0)
									{
										float num2 = float.PositiveInfinity;
										NewWall newWall9 = null;
										foreach (NewNode newNode6 in room.nodes)
										{
											foreach (NewWall newWall10 in newNode6.walls)
											{
												if (newWall10.preset.supportsWallProps && newWall10.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
												{
													float num3 = Vector3.Distance(doorway.node.nodeCoord, newWall10.node.nodeCoord);
													if (num3 < num2)
													{
														num2 = num3;
														newWall9 = newWall10;
													}
												}
											}
										}
										if (newWall9 != null)
										{
											list2.Add(newWall9);
										}
									}
									if (list2.Count > 0)
									{
										list2[Toolbox.Instance.RandContained(0, list2.Count, seedInput, out seedInput)].SetAsLightswitch(room);
									}
								}
							}
						}
					}
					this.GenerateLightZones(room);
				}
			}
		}
		this.UpdateGeometryFloor(ad.floor, "");
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x0014501C File Offset: 0x0014321C
	public void GenerateLightZones(NewRoom room)
	{
		room.lightZones.Clear();
		string seedInput = CityData.Instance.seed + (room.roomID * 2).ToString() + room.roomFloorID.ToString();
		if (room.preset == null || (!room.preset.useMainLights && !room.preset.useAdditionalAreaLights))
		{
			if (room.nodes.Count > 0)
			{
				NewNode[] array = Enumerable.ToArray<NewNode>(room.nodes);
				NewNode newNode = array[Toolbox.Instance.RandContained(0, array.Length, seedInput, out seedInput)];
				room.middleRoomPosition = newNode.position;
			}
			return;
		}
		List<NewNode> list = Enumerable.ToList<NewNode>(Enumerable.OrderBy<NewNode, int>(room.nodes, (NewNode item) => item.nodeCoord.x + item.nodeCoord.y));
		int num = 9999;
		while (list.Count > 0 && num > 0)
		{
			NewNode newNode2 = list[0];
			List<NewNode> list2 = new List<NewNode>();
			int num2 = (int)room.boundsSize.x;
			int num3 = (int)room.boundsSize.y;
			bool flag = true;
			if (Toolbox.Instance.RandContained(0f, 1f, seedInput, out seedInput) > 0.5f)
			{
				flag = false;
				num2 = (int)room.boundsSize.y;
				num3 = (int)room.boundsSize.x;
			}
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num3; j++)
				{
					Vector3 localCoord = newNode2.nodeCoord + new Vector3((float)i, (float)j, 0f);
					if (!flag)
					{
						localCoord = newNode2.nodeCoord + new Vector3((float)j, (float)i, 0f);
					}
					NewNode newNode3 = list.Find((NewNode item) => item.nodeCoord == localCoord);
					if (newNode3 == null)
					{
						num3 = j;
						num--;
						break;
					}
					list2.Add(newNode3);
					list.Remove(newNode3);
				}
				if (num3 <= 0)
				{
					break;
				}
			}
			if (list2.Count > 0)
			{
				room.lightZones.Add(new NewRoom.LightZoneData(room, list2));
			}
			num--;
		}
		room.lightZones = Enumerable.ToList<NewRoom.LightZoneData>(Enumerable.OrderBy<NewRoom.LightZoneData, int>(room.lightZones, (NewRoom.LightZoneData item) => item.nodeList.Count));
		room.lightZones.Reverse();
		if (room.lightZones.Count > 0)
		{
			int num4 = -1;
			room.middleRoomPosition = room.lightZones[0].centreWorldPosition;
			using (List<NewRoom.LightZoneData>.Enumerator enumerator = room.lightZones.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewRoom.LightZoneData lightZoneData = enumerator.Current;
					if (lightZoneData.nodeList.Count > num4)
					{
						room.middleRoomPosition = lightZoneData.centreWorldPosition;
						num4 = lightZoneData.nodeList.Count;
					}
				}
				return;
			}
		}
		if (room.nodes.Count > 0)
		{
			NewNode[] array2 = Enumerable.ToArray<NewNode>(room.nodes);
			NewNode newNode4 = array2[Toolbox.Instance.RandContained(0, array2.Length, seedInput, out seedInput)];
			room.middleRoomPosition = newNode4.position;
		}
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x00145378 File Offset: 0x00143578
	public void GenerateAddressDecor(NewAddress ad)
	{
		if (!SessionData.Instance.isFloorEdit && !ad.generatedRoomConfigs)
		{
			ad.GenerateRoomConfigs();
		}
		ad.RemoveAllInhabitantFurniture(true, FurnitureClusterLocation.RemoveInteractablesOption.remove);
		string seedInput = string.Empty;
		if (SessionData.Instance.isFloorEdit)
		{
			seedInput = Toolbox.Instance.GenerateUniqueID();
		}
		else
		{
			seedInput = CityData.Instance.seed + (ad.id * 2 + ad.id).ToString();
		}
		if (!Game.Instance.disableFurniture)
		{
			List<NewRoom> list = new List<NewRoom>(ad.rooms);
			try
			{
				list.Sort((NewRoom p1, NewRoom p2) => p2.preset.decorationPriority.CompareTo(p1.preset.decorationPriority));
			}
			catch
			{
			}
			foreach (NewRoom newRoom in list)
			{
				if (SessionData.Instance.isFloorEdit)
				{
					newRoom.seed = Toolbox.Instance.GenerateUniqueID();
				}
				this.FurnishRoom(newRoom);
				foreach (NewNode newNode in newRoom.nodes)
				{
					if (newNode.tile.isStairwell)
					{
						newRoom.featuresStairwell = true;
					}
					newNode.SetAllowNewFurniture(true);
					newNode.noPassThrough = false;
					foreach (NewWall newWall in newNode.walls)
					{
						newWall.SelectFrontage();
						if (newWall.door != null && !SessionData.Instance.isFloorEdit)
						{
							newWall.door.SelectColouring(false, null);
						}
					}
				}
				NewRoom newRoom2 = newRoom.commonRooms.Find((NewRoom item) => item.mainLightPreset != null);
				bool flag = false;
				if (newRoom2 != null)
				{
					newRoom.mainLightPreset = newRoom2.mainLightPreset;
					if (newRoom.mainLightPreset.stairwellRule != RoomLightingPreset.StairwellLightRule.either)
					{
						if (newRoom.featuresStairwell && newRoom.mainLightPreset.stairwellRule == RoomLightingPreset.StairwellLightRule.noStairwells)
						{
							newRoom.mainLightPreset = null;
						}
						else if (!newRoom.featuresStairwell && newRoom.mainLightPreset.stairwellRule == RoomLightingPreset.StairwellLightRule.onlyStairwells)
						{
							newRoom.mainLightPreset = null;
						}
					}
					if (newRoom.mainLightPreset != null)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					List<RoomLightingPreset> list2 = new List<RoomLightingPreset>();
					List<RoomLightingPreset> list3 = new List<RoomLightingPreset>();
					foreach (RoomLightingPreset roomLightingPreset in Toolbox.Instance.allRoomLighting)
					{
						if (!roomLightingPreset.disable && roomLightingPreset.roomCompatibility.Contains(newRoom.preset) && roomLightingPreset.designStyleCompatibility.Contains(newRoom.gameLocation.designStyle) && (!(newRoom.building.preset != null) || !roomLightingPreset.banFromBuildings.Contains(newRoom.building.preset)) && (!(newRoom.building.preset != null) || roomLightingPreset.onlyAllowInBuildings.Count <= 0 || roomLightingPreset.onlyAllowInBuildings.Contains(newRoom.building.preset)) && (!(newRoom.building.preset == null) || roomLightingPreset.onlyAllowInBuildings.Count <= 0) && newRoom.nodes.Count >= roomLightingPreset.minimumRoomSize && newRoom.nodes.Count <= roomLightingPreset.maximumRoomSize && (roomLightingPreset.stairwellRule == RoomLightingPreset.StairwellLightRule.either || ((!newRoom.featuresStairwell || roomLightingPreset.stairwellRule != RoomLightingPreset.StairwellLightRule.noStairwells) && (newRoom.featuresStairwell || roomLightingPreset.stairwellRule != RoomLightingPreset.StairwellLightRule.onlyStairwells))))
						{
							if (!roomLightingPreset.lightingPreset.isAtriumLight)
							{
								for (int i = 0; i < roomLightingPreset.frequency; i++)
								{
									list2.Add(roomLightingPreset);
								}
							}
							else
							{
								bool flag2 = false;
								foreach (NewNode newNode2 in newRoom.nodes)
								{
									if (newNode2.floorType == NewNode.FloorTileType.CeilingOnly)
									{
										Vector3Int vector3Int = Vector3Int.zero;
										bool flag3 = false;
										for (int j = 1; j < roomLightingPreset.lightingPreset.minimumFloors; j++)
										{
											vector3Int = newNode2.nodeCoord - new Vector3Int(0, 0, j);
											NewNode newNode3 = null;
											if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode3))
											{
												flag3 = false;
												break;
											}
											if (newNode3.floorType == NewNode.FloorTileType.floorOnly || newNode3.floorType == NewNode.FloorTileType.floorAndCeiling)
											{
												flag3 = false;
												break;
											}
											flag3 = true;
										}
										if (flag3)
										{
											flag2 = true;
											break;
										}
									}
								}
								if (flag2)
								{
									for (int k = 0; k < roomLightingPreset.frequency; k++)
									{
										list3.Add(roomLightingPreset);
									}
								}
							}
						}
					}
					if (true && list3.Count > 0)
					{
						newRoom.mainLightPreset = list3[Toolbox.Instance.RandContained(0, list3.Count, seedInput, out seedInput)];
					}
					else if (list2.Count <= 0)
					{
						if (!newRoom.isNullRoom)
						{
							Game.Log("CityGen: There are no valid room lights for " + newRoom.roomID.ToString(), 2);
						}
					}
					else
					{
						newRoom.mainLightPreset = list2[Toolbox.Instance.RandContained(0, list2.Count, seedInput, out seedInput)];
					}
				}
				if (newRoom.mainLightPreset != null && newRoom.mainLightPreset.ceilingFans.Count > 0 && Toolbox.Instance.RandContained(0f, 1f, seedInput, out seedInput) <= newRoom.preset.chanceOfCeilingFans)
				{
					newRoom.ceilingFans = Toolbox.Instance.RandContained(0, newRoom.mainLightPreset.ceilingFans.Count, seedInput, out seedInput);
				}
			}
		}
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x00145A28 File Offset: 0x00143C28
	public void FurnishRoom(NewRoom room)
	{
		Stopwatch stopwatch = null;
		if (Game.Instance.devMode && Game.Instance.collectDebugData && CityConstructor.Instance != null && CityConstructor.Instance.debugLoadTime != null)
		{
			stopwatch = new Stopwatch();
			stopwatch.Start();
		}
		room.furnitureGroups.Clear();
		if (room.gameLocation.thisAsAddress != null && !room.gameLocation.thisAsAddress.generatedRoomConfigs)
		{
			room.gameLocation.thisAsAddress.GenerateRoomConfigs();
		}
		List<GenerationController.ClusterRank> list = new List<GenerationController.ClusterRank>();
		List<RoomConfiguration> list2 = new List<RoomConfiguration>();
		if (!list2.Contains(room.preset))
		{
			list2.Add(room.preset);
		}
		foreach (RoomConfiguration roomConfiguration in room.openPlanElements)
		{
			if (!list2.Contains(roomConfiguration))
			{
				list2.Add(roomConfiguration);
			}
		}
		List<GenerationController.ClusterRank> list3 = new List<GenerationController.ClusterRank>();
		foreach (RoomConfiguration roomConfiguration2 in list2)
		{
			using (List<FurnitureCluster>.Enumerator enumerator2 = Toolbox.Instance.allFurnitureClusters.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					FurnitureCluster cluster = enumerator2.Current;
					if (!(cluster == null) && !cluster.disable && !list3.Exists((GenerationController.ClusterRank item) => item.cluster == cluster) && !list.Exists((GenerationController.ClusterRank item) => item.cluster == cluster))
					{
						if (cluster.enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Cluster check for ",
								cluster.presetName,
								" in ID ",
								room.roomID.ToString(),
								"..."
							}), 2);
						}
						if (room.nodes.Count < Mathf.Max(cluster.minimumRoomSize, cluster.calculatedMinRoomSize))
						{
							if (cluster.enableDebug)
							{
								Game.Log(string.Concat(new string[]
								{
									"Cluster ",
									cluster.presetName,
									" failed room size check for room  ID ",
									room.roomID.ToString(),
									" (too small)"
								}), 2);
							}
						}
						else if (cluster.useMaximumRoomSize && room.nodes.Count > cluster.maximumRoomSize)
						{
							if (cluster.enableDebug)
							{
								Game.Log(string.Concat(new string[]
								{
									"Cluster ",
									cluster.presetName,
									" failed room size check for room  ID ",
									room.roomID.ToString(),
									" (too big)"
								}), 2);
							}
						}
						else
						{
							if (cluster.limitToFloor)
							{
								if (room.floor != null && room.floor.floor != cluster.allowedOnFloor)
								{
									if (cluster.enableDebug)
									{
										Game.Log("Cluster " + cluster.presetName + " failed floor check for room ID " + room.roomID.ToString(), 2);
										continue;
									}
									continue;
								}
							}
							else if (cluster.limitToFloorRange && room.floor != null && (room.floor.floor < cluster.allowedOnFloorRange.x || room.floor.floor > cluster.allowedOnFloorRange.y))
							{
								if (cluster.enableDebug)
								{
									Game.Log("Cluster " + cluster.presetName + " failed floor check for room ID " + room.roomID.ToString(), 2);
									continue;
								}
								continue;
							}
							if (room.gameLocation.thisAsAddress != null)
							{
								if (!cluster.allowInResidential && room.gameLocation.thisAsAddress.residence != null)
								{
									if (cluster.enableDebug)
									{
										Game.Log("Cluster " + cluster.presetName + " failed residence check for room ID " + room.roomID.ToString(), 2);
										continue;
									}
									continue;
								}
								else if (!cluster.allowInCompanies && room.gameLocation.thisAsAddress.company != null && !room.gameLocation.thisAsAddress.company.preset.isSelfEmployed)
								{
									if (cluster.enableDebug)
									{
										Game.Log("Cluster " + cluster.presetName + " failed company check for room ID " + room.roomID.ToString(), 2);
										continue;
									}
									continue;
								}
								else if (!SessionData.Instance.isFloorEdit && cluster.skipIfNoAddressInhabitants && room.gameLocation.thisAsAddress.inhabitants.Count <= 0 && (!cluster.onlySkipNoInhabitantsIfResidenceOrCompany || room.gameLocation.thisAsAddress.company != null || room.gameLocation.thisAsAddress.residence != null) && !cluster.dontSkipNoInhabitantsIfIn.Contains(room.preset.roomClass) && (room.gameLocation.thisAsAddress.residence == null || room.gameLocation.thisAsAddress.residence.preset == null || !room.gameLocation.thisAsAddress.residence.preset.furnitureIfUnihabited))
								{
									if (cluster.enableDebug)
									{
										Game.Log("Cluster " + cluster.presetName + " failed inhabitants check for roomID " + room.roomID.ToString(), 2);
										continue;
									}
									continue;
								}
							}
							else if (!cluster.allowOnStreets && room.gameLocation.thisAsStreet != null)
							{
								if (cluster.enableDebug)
								{
									Game.Log("Cluster " + cluster.presetName + " failed street check for room ID " + room.roomID.ToString(), 2);
									continue;
								}
								continue;
							}
							if (!SessionData.Instance.isFloorEdit)
							{
								if (cluster.limitToDistricts && !cluster.allowedInDistricts.Contains(room.gameLocation.district.preset))
								{
									if (cluster.enableDebug)
									{
										Game.Log("Cluster " + cluster.presetName + " failed district limit check for room ID " + room.roomID.ToString(), 2);
										continue;
									}
									continue;
								}
								else if (cluster.banFromDistricts && cluster.notAllowedInDistricts.Contains(room.gameLocation.district.preset))
								{
									if (cluster.enableDebug)
									{
										Game.Log("Cluster " + cluster.presetName + " failed district ban check for room ID " + room.roomID.ToString(), 2);
										continue;
									}
									continue;
								}
							}
							if (!this.ClusterCountChecks(cluster, room, cluster.enableDebug))
							{
								if (cluster.enableDebug)
								{
									Game.Log("Cluster " + cluster.presetName + " failed cluster counts check for room ID " + room.roomID.ToString(), 2);
								}
							}
							else if (cluster.securityDoor && roomConfiguration2.securityDoors == RoomConfiguration.SecurityDoorRule.never)
							{
								if (cluster.enableDebug)
								{
									Game.Log("Cluster " + cluster.presetName + " failed security door check for room ID " + room.roomID.ToString(), 2);
								}
							}
							else
							{
								if (cluster.isBreakerBox)
								{
									if (!(room.gameLocation.thisAsAddress != null) || !(room.gameLocation.thisAsAddress.addressPreset != null))
									{
										continue;
									}
									if (room.gameLocation.thisAsAddress.addressPreset.useOwnBreakerBox)
									{
										if (room.gameLocation.thisAsAddress.breakerLightsID >= 0)
										{
											continue;
										}
									}
									else if (!(room.gameLocation.floor != null) || room.gameLocation.floor.breakerLightsID >= 0)
									{
										continue;
									}
								}
								bool flag = false;
								bool flag2 = false;
								if (cluster.allowedInOpenPlan == FurnitureCluster.AllowedOpenPlan.no && room.openPlanElements.Count > 0)
								{
									if (cluster.enableDebug)
									{
										Game.Log("Cluster " + cluster.presetName + " failed open plan check for room ID " + room.roomID.ToString(), 2);
									}
								}
								else
								{
									foreach (RoomTypeFilter roomTypeFilter in cluster.allowedRoomFilters)
									{
										if (roomTypeFilter == null || roomTypeFilter.roomClasses == null)
										{
											Game.LogError("Missing room filter in " + cluster.presetName, 2);
										}
										else
										{
											foreach (RoomConfiguration roomConfiguration3 in list2)
											{
												if (!(roomConfiguration3 == null) && roomTypeFilter.roomClasses.Contains(roomConfiguration3.roomClass))
												{
													if (!flag2)
													{
														if (cluster.allowedInOpenPlan == FurnitureCluster.AllowedOpenPlan.yes)
														{
															flag2 = true;
														}
														else if (room.openPlanElements.Contains(roomConfiguration3))
														{
															if (cluster.allowedInOpenPlan == FurnitureCluster.AllowedOpenPlan.openPlanOnly)
															{
																flag2 = true;
															}
														}
														else if (cluster.allowedInOpenPlan == FurnitureCluster.AllowedOpenPlan.no)
														{
															flag2 = true;
														}
													}
													flag = true;
													if (flag && flag2)
													{
														if (cluster.allowedInOpenPlan == FurnitureCluster.AllowedOpenPlan.yes)
														{
															break;
														}
														if (cluster.allowedInOpenPlan == FurnitureCluster.AllowedOpenPlan.openPlanOnly)
														{
															break;
														}
													}
												}
											}
										}
									}
									if (!flag2)
									{
										if (cluster.enableDebug)
										{
											Game.Log("Cluster " + cluster.presetName + " failed open plan check for room ID " + room.roomID.ToString(), 2);
										}
									}
									else if (!flag)
									{
										if (cluster.enableDebug)
										{
											Game.Log("Cluster " + cluster.presetName + " failed room suitability check for room ID " + room.roomID.ToString(), 2);
										}
									}
									else
									{
										float num = cluster.placementChance;
										if (cluster.modifyPlacementChanceTraits.Count > 0)
										{
											List<Human> list4 = new List<Human>(room.belongsTo);
											if (room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.residence != null)
											{
												foreach (Human human in room.gameLocation.thisAsAddress.inhabitants)
												{
													if (!list4.Contains(human))
													{
														list4.Add(human);
													}
												}
											}
											float num2 = 0f;
											foreach (Human human2 in list4)
											{
												num2 += human2.GetChance(ref cluster.modifyPlacementChanceTraits, 0f);
											}
											if (num2 > 0f)
											{
												num2 /= (float)list4.Count;
											}
											num += num2;
											num = Mathf.Clamp01(num);
										}
										string str = room.seed + cluster.presetName;
										if (Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, str, false) > num)
										{
											if (cluster.enableDebug)
											{
												Game.Log("Cluster " + cluster.presetName + " failed placement chance check for room ID " + room.roomID.ToString(), 2);
											}
										}
										else
										{
											float num3 = cluster.roomPriority + Toolbox.Instance.GetPsuedoRandomNumber(0f, 0.05f, str, false);
											if (cluster.modifyPriorityTraits.Count > 0)
											{
												List<Human> list5 = new List<Human>(room.belongsTo);
												if (room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.residence != null)
												{
													foreach (Human human3 in room.gameLocation.thisAsAddress.inhabitants)
													{
														if (!list5.Contains(human3))
														{
															list5.Add(human3);
														}
													}
												}
												float num4 = 0f;
												foreach (Human human4 in list5)
												{
													num4 += human4.GetChance(ref cluster.modifyPriorityTraits, 0f) * 10f;
												}
												num4 /= (float)list5.Count;
												num3 += num4;
											}
											if (cluster.essentialFurniture)
											{
												if (cluster.enableDebug)
												{
													Game.Log("Cluster " + cluster.presetName + " added to essential furniture for ID " + room.roomID.ToString(), 2);
												}
												list3.Add(new GenerationController.ClusterRank
												{
													cluster = cluster,
													rank = num3
												});
											}
											else
											{
												if (cluster.enableDebug)
												{
													Game.Log("Cluster " + cluster.presetName + " added to compatible furniture for ID " + room.roomID.ToString(), 2);
												}
												list.Add(new GenerationController.ClusterRank
												{
													cluster = cluster,
													rank = num3
												});
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		if (list.Count <= 0 && list3.Count <= 0)
		{
			return;
		}
		if (room.entrances.Count <= 0)
		{
			if (room.preset != CityControls.Instance.nullDefaultRoom)
			{
				if (room.floor != null)
				{
					Game.LogError(string.Concat(new string[]
					{
						"Room ",
						room.roomID.ToString(),
						", floor ",
						room.floor.floorName,
						" has no entrances! Cannot furnish..."
					}), 2);
				}
				else
				{
					Game.LogError("Room " + room.roomID.ToString() + ", has no entrances! Cannot furnish...", 2);
				}
			}
			Game.LogError("Room " + room.roomID.ToString() + ", has no entrances! Cannot furnish...", 2);
			return;
		}
		if (list.Count > 0)
		{
			list.Sort((GenerationController.ClusterRank p1, GenerationController.ClusterRank p2) => p2.rank.CompareTo(p1.rank));
		}
		if (list3.Count > 0)
		{
			list3.Sort((GenerationController.ClusterRank p1, GenerationController.ClusterRank p2) => p2.rank.CompareTo(p1.rank));
		}
		List<FurnitureClusterLocation> list6 = null;
		int num5 = 42;
		if (room.preset.overrideMaxFurnitureClusters)
		{
			num5 = room.preset.overridenMaxFurniture;
		}
		else
		{
			float num6 = InteriorControls.Instance.roomSizeClusterAttemptMultiplier;
			if (room.preset.overrideAttemptsPerNodeMultiplier)
			{
				num6 = room.preset.overridenAttemptsPerNode;
			}
			num5 = Mathf.CeilToInt(num6 * (float)room.nodes.Count);
		}
		Dictionary<FurnitureCluster, int> dictionary = new Dictionary<FurnitureCluster, int>();
		while (num5 > 0 && (list.Count > 0 || list3.Count > 0))
		{
			GenerationController.ClusterRank cluster;
			if (list3.Count > 0)
			{
				cluster = list3[0];
				list3.RemoveAt(0);
				if (cluster.cluster.enableDebug)
				{
					Game.Log(string.Concat(new string[]
					{
						"...Placing essential cluster ",
						cluster.cluster.presetName,
						" in ID ",
						room.roomID.ToString(),
						"..."
					}), 2);
				}
			}
			else
			{
				cluster = list[0];
				list.RemoveAt(0);
				if (cluster.cluster.enableDebug)
				{
					Game.Log(string.Concat(new string[]
					{
						"...Placing non-essential cluster ",
						cluster.cluster.presetName,
						" in ID ",
						room.roomID.ToString(),
						"..."
					}), 2);
				}
			}
			if (cluster.cluster == null)
			{
				Game.LogError("Null cluster detected, this shouldn't be happening...", 2);
				if (list3.Count > 0)
				{
					list3.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cluster.cluster);
				}
				if (list.Count > 0)
				{
					list.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cluster.cluster);
				}
			}
			else
			{
				if (cluster.cluster.limitPerRoom)
				{
					int num7 = 0;
					dictionary.TryGetValue(cluster.cluster, ref num7);
					if (num7 >= cluster.cluster.maximumPerRoom)
					{
						if (cluster.cluster.enableDebug)
						{
							Game.LogError("...Maximum furniture count reached for " + cluster.cluster.presetName + " in ID " + room.roomID.ToString(), 2);
						}
						if (list3.Count > 0)
						{
							list3.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cluster.cluster);
						}
						if (list.Count > 0)
						{
							list.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cluster.cluster);
							continue;
						}
						continue;
					}
				}
				if (!this.ClusterCountChecks(cluster.cluster, room, cluster.cluster.enableDebug))
				{
					if (cluster.cluster.enableDebug)
					{
						Game.LogError(string.Concat(new string[]
						{
							"...Cluster count check for ",
							cluster.cluster.presetName,
							" in ID ",
							room.roomID.ToString(),
							" has failed..."
						}), 2);
					}
					if (list3.Count > 0)
					{
						list3.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cluster.cluster);
					}
					if (list.Count > 0)
					{
						list.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cluster.cluster);
					}
				}
				else
				{
					FurnitureClusterLocation bestFurnitureClusterLocation = this.GetBestFurnitureClusterLocation(room, cluster.cluster, cluster.cluster.enableDebug);
					if (bestFurnitureClusterLocation == null)
					{
						if (list3.Count > 0)
						{
							list3.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cluster.cluster);
						}
						if (list.Count > 0)
						{
							list.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cluster.cluster);
						}
						using (List<FurnitureCluster>.Enumerator enumerator2 = cluster.cluster.removeClustersOnFail.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								FurnitureCluster cl = enumerator2.Current;
								if (list3.Count > 0)
								{
									list3.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cl);
								}
								if (list.Count > 0)
								{
									list.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cl);
								}
							}
						}
						if (cluster.cluster.enableDebug)
						{
							Game.LogError(string.Concat(new string[]
							{
								"Unable to place furniture cluster ",
								cluster.cluster.presetName,
								" in ID ",
								room.roomID.ToString(),
								" (0 valid locations found)"
							}), 2);
						}
					}
					else
					{
						if (cluster.cluster.enableDebug && list6 != null && cluster.cluster != null && room != null)
						{
							Game.Log(string.Concat(new string[]
							{
								list6.Count.ToString(),
								" locations for cluster ",
								cluster.cluster.presetName,
								" in room ID ",
								room.roomID.ToString()
							}), 2);
						}
						room.AddFurniture(bestFurnitureClusterLocation, true, true, false);
						num5--;
						if (Game.Instance.devMode && Game.Instance.collectDebugData)
						{
							room.clustersPlaced = string.Concat(new string[]
							{
								room.clustersPlaced,
								"(",
								cluster.rank.ToString(),
								" ",
								list3.Count.ToString(),
								"/",
								list.Count.ToString(),
								")"
							});
						}
						if (!dictionary.ContainsKey(bestFurnitureClusterLocation.cluster))
						{
							dictionary.Add(bestFurnitureClusterLocation.cluster, 1);
						}
						else
						{
							Dictionary<FurnitureCluster, int> dictionary2 = dictionary;
							FurnitureCluster cluster2 = bestFurnitureClusterLocation.cluster;
							int num8 = dictionary2[cluster2];
							dictionary2[cluster2] = num8 + 1;
						}
						if ((!cluster.cluster.limitPerRoom || dictionary[bestFurnitureClusterLocation.cluster] < cluster.cluster.maximumPerRoom) && !list.Contains(cluster))
						{
							list.Add(cluster);
						}
						using (List<FurnitureCluster>.Enumerator enumerator2 = cluster.cluster.addClustersOnSuccess.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								FurnitureCluster cl = enumerator2.Current;
								if (cl.enableDebug)
								{
									Game.Log(string.Concat(new string[]
									{
										cl.presetName,
										" is being added as an option for ID ",
										room.roomID.ToString(),
										" becuase of successful placement of ",
										cluster.cluster.presetName
									}), 2);
								}
								GenerationController.ClusterRank clusterRank = new GenerationController.ClusterRank
								{
									cluster = cl,
									rank = cl.roomPriority
								};
								if (cl.essentialFurniture && !list3.Exists((GenerationController.ClusterRank item) => item.cluster == cl))
								{
									list3.Add(clusterRank);
								}
								else if (!list.Exists((GenerationController.ClusterRank item) => item.cluster == cl))
								{
									list.Add(clusterRank);
								}
							}
						}
						using (List<FurnitureCluster>.Enumerator enumerator2 = cluster.cluster.removeClustersOnSuccess.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								FurnitureCluster cl = enumerator2.Current;
								if (cl.enableDebug)
								{
									Game.Log(string.Concat(new string[]
									{
										cl.presetName,
										" is being removed as an option for ID ",
										room.roomID.ToString(),
										" becuase of successful placement of ",
										cluster.cluster.presetName
									}), 2);
								}
								if (list3.Count > 0)
								{
									list3.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cl);
								}
								if (list.Count > 0)
								{
									list.RemoveAll((GenerationController.ClusterRank item) => item.cluster == cl);
								}
							}
						}
					}
				}
			}
		}
		if (SessionData.Instance.isFloorEdit)
		{
			room.SetMainLights(true, "FloorEdit", null, false, true);
			room.ConnectNodes();
		}
		if (room.pickFurnitureCache != null)
		{
			room.pickFurnitureCache = null;
		}
		if (Game.Instance.devMode && Game.Instance.collectDebugData && CityConstructor.Instance != null && CityConstructor.Instance.debugLoadTime != null)
		{
			stopwatch.Stop();
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
		}
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x00147460 File Offset: 0x00145660
	private bool ClusterCountChecks(FurnitureCluster cluster, NewRoom room, bool enableDebug = false)
	{
		if (cluster.limitPerAddress && this.GetClustersInGameLocation(room.gameLocation, cluster).Count >= cluster.maximumPerAddress)
		{
			if (enableDebug)
			{
				Game.Log("... Limit per address reached", 2);
			}
			return false;
		}
		if (cluster.wealthLimit)
		{
			float normalizedLandValue = Toolbox.Instance.GetNormalizedLandValue(room.gameLocation, false);
			if (normalizedLandValue < cluster.minimumWealth || normalizedLandValue > cluster.maximumWealth)
			{
				if (enableDebug)
				{
					Game.Log("... Wealth limit reached", 2);
				}
				return false;
			}
		}
		if (cluster.useRoomGrub)
		{
			float grubiness = room.defaultWallKey.grubiness;
			if (grubiness < cluster.minimumGrub || grubiness > cluster.maximumGrub)
			{
				if (enableDebug)
				{
					Game.Log("... Grub limit reached", 2);
				}
				return false;
			}
		}
		if (cluster.useBuildingResidences)
		{
			if (room.building == null)
			{
				if (enableDebug)
				{
					Game.Log("... Building residences: No building found", 2);
				}
				return false;
			}
			int num = 0;
			foreach (KeyValuePair<int, NewFloor> keyValuePair in room.building.floors)
			{
				num += keyValuePair.Value.addresses.FindAll((NewAddress item) => item.residence != null).Count;
			}
			if (num < cluster.minimumResidences || num > cluster.maximumResidences)
			{
				if (enableDebug)
				{
					Game.Log("... Building residence limit reached", 2);
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x001475DC File Offset: 0x001457DC
	public List<FurnitureClusterLocation> GetPossibleFurnitureClusterLocations(NewRoom room, FurnitureCluster cluster, bool enableDebug = false)
	{
		List<FurnitureClusterLocation> list = new List<FurnitureClusterLocation>();
		List<FurnitureCluster.FurnitureClusterRule> list2 = new List<FurnitureCluster.FurnitureClusterRule>();
		Dictionary<FurnitureClass, List<FurnitureLocation>> dictionary = null;
		Dictionary<FurnitureClass, List<FurnitureLocation>> dictionary2 = null;
		for (int i = 0; i < cluster.clusterElements.Count; i++)
		{
			FurnitureCluster.FurnitureClusterRule furnitureClusterRule = cluster.clusterElements[i];
			if (enableDebug)
			{
				string[] array = new string[5];
				array[0] = "Searching placements for element ";
				int num = 1;
				int j = i + 1;
				array[num] = j.ToString();
				array[2] = "/";
				int num2 = 3;
				j = cluster.clusterElements.Count;
				array[num2] = j.ToString();
				array[4] = "...";
				Game.Log(string.Concat(array), 2);
			}
			bool flag = true;
			if (furnitureClusterRule == null || furnitureClusterRule.furnitureClass == null)
			{
				flag = false;
				Game.LogError("Null furniture class found in cluster " + cluster.presetName + " index " + i.ToString(), 2);
			}
			else
			{
				if (furnitureClusterRule.furnitureClass.limitPerRoom)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<FurnitureClass, List<FurnitureLocation>>();
					}
					int num3 = 0;
					if (dictionary.ContainsKey(furnitureClusterRule.furnitureClass))
					{
						num3 = dictionary[furnitureClusterRule.furnitureClass].Count;
					}
					else
					{
						List<FurnitureLocation> furnitureInRoom = this.GetFurnitureInRoom(room, furnitureClusterRule.furnitureClass);
						num3 += furnitureInRoom.Count;
						dictionary.Add(furnitureClusterRule.furnitureClass, furnitureInRoom);
					}
					if (num3 >= furnitureClusterRule.furnitureClass.maximumNumberPerRoom)
					{
						if (enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Element Room limit reached (",
								num3.ToString(),
								"/",
								furnitureClusterRule.furnitureClass.maximumNumberPerRoom.ToString(),
								")"
							}), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return list;
						}
					}
				}
				int num4 = 9999;
				int num5 = 9999;
				if (furnitureClusterRule.furnitureClass.limitPerBuildingResidence)
				{
					if (room.building == null)
					{
						if (enableDebug)
						{
							Game.Log("Element per residence reached", 2);
						}
						flag = false;
					}
					else
					{
						int num6 = 0;
						foreach (KeyValuePair<int, NewFloor> keyValuePair in room.building.floors)
						{
							num6 += keyValuePair.Value.addresses.FindAll((NewAddress item) => item.residence != null).Count;
						}
						num4 = Mathf.CeilToInt((float)num6 / (float)furnitureClusterRule.furnitureClass.perBuildingResidences);
					}
				}
				if (furnitureClusterRule.furnitureClass.limitPerJobs)
				{
					if (room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.company != null)
					{
						num5 = Mathf.CeilToInt((float)room.gameLocation.thisAsAddress.company.companyRoster.Count / (float)furnitureClusterRule.furnitureClass.perJobs);
					}
					else
					{
						flag = false;
					}
				}
				if (furnitureClusterRule.furnitureClass.limitPerAddress || furnitureClusterRule.furnitureClass.limitPerBuildingResidence || furnitureClusterRule.furnitureClass.limitPerJobs)
				{
					if (dictionary2 == null)
					{
						dictionary2 = new Dictionary<FurnitureClass, List<FurnitureLocation>>();
					}
					int num7 = 0;
					if (dictionary2.ContainsKey(furnitureClusterRule.furnitureClass))
					{
						num7 = dictionary2[furnitureClusterRule.furnitureClass].Count;
					}
					else
					{
						List<FurnitureLocation> furnitureInGameLocation = this.GetFurnitureInGameLocation(room.gameLocation, furnitureClusterRule.furnitureClass);
						num7 += furnitureInGameLocation.Count;
						dictionary2.Add(furnitureClusterRule.furnitureClass, furnitureInGameLocation);
					}
					if (furnitureClusterRule.furnitureClass.limitPerAddress && num7 >= furnitureClusterRule.furnitureClass.maximumNumberPerAddress)
					{
						if (enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Element Room limit reached (",
								num7.ToString(),
								"/",
								furnitureClusterRule.furnitureClass.maximumNumberPerAddress.ToString(),
								")"
							}), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return list;
						}
					}
					else if (furnitureClusterRule.furnitureClass.limitPerBuildingResidence && num7 >= num4)
					{
						if (enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Element Room limit reached residences (",
								num7.ToString(),
								"/",
								num4.ToString(),
								")"
							}), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return list;
						}
					}
					else if (furnitureClusterRule.furnitureClass.limitPerJobs && num7 >= num5)
					{
						if (enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Element Room limit reached residences (",
								num7.ToString(),
								"/",
								num5.ToString(),
								")"
							}), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return list;
						}
					}
				}
				if (flag && furnitureClusterRule.furnitureClass.skipIfNoAddressInhabitants && room.gameLocation != null && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.inhabitants.Count <= 0)
				{
					if (enableDebug)
					{
						Game.Log("Element Skipping as no inhabitants...", 2);
					}
					flag = false;
					if (furnitureClusterRule.importantToCluster)
					{
						return list;
					}
				}
				if (flag && Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, room.seed, false) > furnitureClusterRule.chanceOfPlacementAttempt)
				{
					if (enableDebug)
					{
						Game.Log("Element Skipping because of element placement chance (" + furnitureClusterRule.chanceOfPlacementAttempt.ToString() + ")", 2);
					}
					flag = false;
				}
				if (flag && furnitureClusterRule.furnitureClass.limitToFloor && Mathf.RoundToInt((float)Enumerable.FirstOrDefault<NewNode>(room.nodes).nodeCoord.z) != furnitureClusterRule.furnitureClass.allowedOnFloor)
				{
					if (enableDebug)
					{
						Game.Log("Element Skipping as limited to floor " + furnitureClusterRule.furnitureClass.allowedOnFloor.ToString(), 2);
					}
					flag = false;
					if (furnitureClusterRule.importantToCluster)
					{
						return list;
					}
				}
				if (flag && furnitureClusterRule.furnitureClass.limitToFloorRange)
				{
					int num8 = Mathf.RoundToInt((float)Enumerable.FirstOrDefault<NewNode>(room.nodes).nodeCoord.z);
					if ((float)num8 < furnitureClusterRule.furnitureClass.allowedOnFloorRange.x || (float)num8 > furnitureClusterRule.furnitureClass.allowedOnFloorRange.y)
					{
						if (enableDebug)
						{
							Game.Log("Element Skipping as limited to floor range " + furnitureClusterRule.furnitureClass.limitToFloorRange.ToString(), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return list;
						}
					}
				}
				if (flag)
				{
					List<FurniturePreset> list3;
					flag = this.GetValidFurniture(furnitureClusterRule.furnitureClass, room, false, out list3, false);
					if (enableDebug)
					{
						Game.Log("Valid furniture exists for element: " + flag.ToString(), 2);
					}
				}
				if (flag)
				{
					if (enableDebug)
					{
						Game.Log("Element: Is a valid element...", 2);
					}
					list2.Add(furnitureClusterRule);
				}
				else
				{
					if (enableDebug)
					{
						Game.Log("Element: Unable to find valid furniture for this cluster/room combination", 2);
					}
					if (furnitureClusterRule.importantToCluster)
					{
						return list;
					}
				}
			}
		}
		foreach (NewNode newNode in room.nodes)
		{
			if (cluster.coastalOnly && !SessionData.Instance.isFloorEdit)
			{
				bool flag2 = true;
				foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
				{
					Vector3Int vector3Int = newNode.tile.globalTileCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
					NewTile newTile = null;
					if (PathFinder.Instance.tileMap.TryGetValue(vector3Int, ref newTile) && newTile.cityTile.building != null && !newTile.cityTile.building.preset.boundary)
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					continue;
				}
			}
			FurnitureClusterDebug furnitureClusterDebug = null;
			if (SessionData.Instance.isFloorEdit && enableDebug)
			{
				furnitureClusterDebug = Object.Instantiate<GameObject>(PrefabControls.Instance.furnitureDebug, FloorEditController.Instance.debugContainer).GetComponent<FurnitureClusterDebug>();
				furnitureClusterDebug.Setup(cluster, newNode);
			}
			int[] angleArrayX = CityData.Instance.angleArrayX4;
			int j = 0;
			while (j < angleArrayX.Length)
			{
				int num9 = angleArrayX[j];
				if (cluster.zeroNodeWallRules.Count <= 0)
				{
					goto IL_F83;
				}
				Toolbox.Instance.RotateVector2CW(Vector2.zero, (float)num9);
				bool flag3 = true;
				foreach (FurnitureClass.FurnitureWallRule furnitureWallRule in cluster.zeroNodeWallRules)
				{
					Vector2 v = CityData.Instance.GetOffsetFromDirection(furnitureWallRule.wallDirection);
					Vector2 vector = Toolbox.Instance.RotateVector2CW(furnitureWallRule.nodeOffset, (float)num9);
					Vector3Int vector3Int2 = newNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
					Vector2 vector2 = Toolbox.Instance.RotateVector2CW(v, (float)num9) * 0.5f;
					vector2..ctor(Mathf.Round(vector2.x * 2f) / 2f, Mathf.Round(vector2.y * 2f) / 2f);
					NewNode newNode2 = null;
					bool flag4 = true;
					bool flag5 = false;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode2))
					{
						flag5 = true;
						NewWall foundWall = null;
						if (newNode2.wallDict.TryGetValue(vector2, ref foundWall))
						{
							if (flag4)
							{
								if (furnitureWallRule.tag == FurnitureClass.WallRule.entrance)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.entranceDoorOnly)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
									{
										flag4 = false;
									}
									else if (foundWall.preset.divider)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.entraceDivider)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
									{
										flag4 = false;
									}
									else if (!foundWall.preset.divider)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.entranceToRoomOfType && furnitureWallRule.roomType != null)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance || foundWall.otherWall.node.room.preset != furnitureWallRule.roomType)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.addressEntrance)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance || foundWall.otherWall.node.room.gameLocation == foundWall.node.room.gameLocation)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.nothing)
								{
									flag4 = false;
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.ventLower)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventLower)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.ventUpper)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventUpper)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.ventTop)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventTop)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.wall)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.window)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.window)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.windowLarge)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.windowLarge)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.anyWindow)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.windowLarge && foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.window)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.wallOrUpperVent)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall && foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventUpper)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.fence)
								{
									if (!foundWall.preset.isFence)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.lightswitch)
								{
									if (foundWall.lightswitchInteractable == null)
									{
										flag4 = false;
									}
								}
								else if (furnitureWallRule.tag == FurnitureClass.WallRule.securityDoorDivider)
								{
									if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
									{
										flag4 = false;
									}
									else if (!foundWall.preset.divider)
									{
										flag4 = false;
									}
									else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.never)
									{
										flag4 = false;
									}
									else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.onlyToOtherAddress)
									{
										if (foundWall.node.gameLocation == foundWall.otherWall.node.gameLocation)
										{
											flag4 = false;
										}
									}
									else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.onlyToStairwell)
									{
										bool flag6 = false;
										using (HashSet<NewNode>.Enumerator enumerator4 = foundWall.otherWall.node.room.nodes.GetEnumerator())
										{
											while (enumerator4.MoveNext())
											{
												if (enumerator4.Current.tile.isStairwell)
												{
													flag6 = true;
													break;
												}
											}
										}
										if (!flag6)
										{
											flag4 = false;
										}
									}
									if (flag4)
									{
										Predicate<NewNode> <>9__1;
										foreach (Interactable interactable in foundWall.node.floor.securityDoors)
										{
											List<NewNode> coversNodes = interactable.furnitureParent.coversNodes;
											Predicate<NewNode> predicate;
											if ((predicate = <>9__1) == null)
											{
												predicate = (<>9__1 = ((NewNode item) => item == foundWall.node || item == foundWall.otherWall.node));
											}
											if (coversNodes.Exists(predicate))
											{
												flag4 = false;
												break;
											}
										}
									}
									if (flag4 && room.floor.floor != 0)
									{
										bool flag7 = false;
										foreach (NewGameLocation newGameLocation in room.floor.addresses)
										{
											new List<AirDuctGroup.AirDuctSection>();
											new List<AirDuctGroup.AirDuctSection>();
											foreach (NewNode newNode3 in newGameLocation.nodes)
											{
												foreach (AirDuctGroup.AirDuctSection airDuctSection in newNode3.airDucts)
												{
													List<Vector3Int> list4;
													List<AirDuctGroup.AirVent> list5;
													List<Vector3Int> list6;
													airDuctSection.GetNeighborSections(out list4, out list5, out list6);
													list4.AddRange(list6);
													if (airDuctSection.level == 0)
													{
														if (list4.Exists((Vector3Int item) => item.z < 0))
														{
															flag7 = true;
															break;
														}
													}
												}
											}
										}
										if (!flag7)
										{
											flag4 = false;
										}
									}
								}
							}
						}
						else if (furnitureWallRule.tag != FurnitureClass.WallRule.nothing)
						{
							flag4 = false;
						}
					}
					else
					{
						flag4 = false;
					}
					if (furnitureWallRule.option == FurnitureClass.FurnitureRuleOption.cantFeature)
					{
						if (flag4 || !flag5)
						{
							flag3 = false;
							break;
						}
					}
					else if (furnitureWallRule.option == FurnitureClass.FurnitureRuleOption.mustFeature && (!flag4 || !flag5))
					{
						flag3 = false;
						break;
					}
				}
				if (flag3)
				{
					goto IL_F83;
				}
				IL_3188:
				j++;
				continue;
				IL_F83:
				bool flag8 = true;
				int num10 = 0;
				Dictionary<NewNode, List<NewNode>> dictionary3 = new Dictionary<NewNode, List<NewNode>>();
				List<NewNode> list7 = new List<NewNode>();
				List<NewNode> list8 = new List<NewNode>();
				List<NewNode> list9 = new List<NewNode>();
				List<NewWall> list10 = new List<NewWall>();
				List<FurnitureLocation> list11 = new List<FurnitureLocation>();
				Dictionary<FurnitureClass, int> dictionary4 = new Dictionary<FurnitureClass, int>();
				bool flag9 = false;
				FurnitureClusterDebug.DebugFurnitureAnglePlacement debugFurnitureAnglePlacement = null;
				if (SessionData.Instance.isFloorEdit && enableDebug && furnitureClusterDebug != null)
				{
					debugFurnitureAnglePlacement = new FurnitureClusterDebug.DebugFurnitureAnglePlacement();
					debugFurnitureAnglePlacement.angle = num9;
					debugFurnitureAnglePlacement.log = new List<string>();
				}
				for (int k = 0; k < list2.Count; k++)
				{
					FurnitureCluster.FurnitureClusterRule furnitureClusterRule2 = list2[k];
					if (!furnitureClusterRule2.onlyValidIfPreviousObjectPlaced || flag9)
					{
						flag9 = false;
						bool flag10 = false;
						if (flag8 && furnitureClusterRule2.furnitureClass.limitPerRoom)
						{
							int num11 = dictionary[furnitureClusterRule2.furnitureClass].Count;
							int num12 = 0;
							if (dictionary4.TryGetValue(furnitureClusterRule2.furnitureClass, ref num12))
							{
								num11 += num12;
							}
							if (num11 >= furnitureClusterRule2.furnitureClass.maximumNumberPerRoom)
							{
								if (furnitureClusterRule2.importantToCluster)
								{
									flag8 = false;
									break;
								}
								goto IL_2FF8;
							}
						}
						if (flag8 && furnitureClusterRule2.furnitureClass.limitPerAddress)
						{
							int num13 = dictionary2[furnitureClusterRule2.furnitureClass].Count;
							int num14 = 0;
							if (dictionary4.TryGetValue(furnitureClusterRule2.furnitureClass, ref num14))
							{
								num13 += num14;
							}
							if (furnitureClusterRule2.furnitureClass.limitPerAddress && num13 >= furnitureClusterRule2.furnitureClass.maximumNumberPerAddress)
							{
								if (furnitureClusterRule2.importantToCluster)
								{
									flag8 = false;
									break;
								}
								goto IL_2FF8;
							}
						}
						if (enableDebug && debugFurnitureAnglePlacement != null)
						{
							debugFurnitureAnglePlacement.log.Add(string.Concat(new string[]
							{
								"Anylizing placements for element ",
								k.ToString(),
								"/",
								cluster.clusterElements.Count.ToString(),
								" ",
								furnitureClusterRule2.furnitureClass.name,
								"..."
							}));
						}
						foreach (Vector2 vector3 in furnitureClusterRule2.placements)
						{
							bool flag11 = true;
							if (enableDebug && debugFurnitureAnglePlacement != null)
							{
								List<string> log = debugFurnitureAnglePlacement.log;
								string text = "Anylizing placement ";
								Vector2 vector4 = vector3;
								log.Add(text + vector4.ToString());
							}
							Dictionary<NewNode, List<NewNode>> dictionary5 = new Dictionary<NewNode, List<NewNode>>();
							List<NewNode> list12 = new List<NewNode>();
							list10.Clear();
							NewNode newNode4 = null;
							Vector2 vector5 = Toolbox.Instance.RotateVector2CW(vector3, (float)num9);
							int num15 = num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing) - 180;
							int num16 = 0;
							while ((float)num16 < furnitureClusterRule2.furnitureClass.objectSize.x)
							{
								int num17 = 0;
								while ((float)num17 < furnitureClusterRule2.furnitureClass.objectSize.y)
								{
									Vector2 vector6 = Toolbox.Instance.RotateVector2CW(new Vector2((float)num16, (float)num17), (float)num15);
									Vector2 vector7 = vector5 + vector6;
									Vector3Int vector3Int3 = newNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector7.x), Mathf.RoundToInt(vector7.y), 0);
									NewNode newNode5 = null;
									if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int3, ref newNode5))
									{
										if (newNode4 == null)
										{
											newNode4 = newNode5;
										}
										if (newNode5.room != room)
										{
											if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
											{
												debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because the furniture would reach another room");
											}
											flag11 = false;
											break;
										}
										if (!furnitureClusterRule2.furnitureClass.allowedOnStairwell)
										{
											if (newNode5.tile.isStairwell)
											{
												if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because this is a stairwell");
												}
												flag11 = false;
												break;
											}
											foreach (Vector2Int vector2Int2 in CityData.Instance.offsetArrayX4)
											{
												Vector3Int vector3Int4 = newNode5.nodeCoord + new Vector3Int(vector2Int2.x, vector2Int2.y, 0);
												NewNode newNode6 = null;
												if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int4, ref newNode6) && newNode6.tile.isStairwell)
												{
													if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because this is a stairwell");
													}
													flag11 = false;
													break;
												}
											}
											if (!flag11)
											{
												break;
											}
										}
										else if (furnitureClusterRule2.furnitureClass.onlyOnStairwell && !newNode5.tile.isStairwell)
										{
											if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
											{
												debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because this isn't a stairwell");
											}
											flag11 = false;
											break;
										}
										if (newNode5.gameLocation.thisAsStreet == null && !furnitureClusterRule2.furnitureClass.allowIfNoFloor && !newNode5.HasValidFloor())
										{
											if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
											{
												debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because there is no floor");
											}
											flag11 = false;
											break;
										}
										if (furnitureClusterRule2.furnitureClass.ceilingPiece)
										{
											if (newNode5.floorType == NewNode.FloorTileType.floorOnly || newNode5.floorType == NewNode.FloorTileType.none || newNode5.floorType == NewNode.FloorTileType.noneButIndoors)
											{
												if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because there is no ceiling");
												}
												flag11 = false;
												break;
											}
											if (!newNode5.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level == 1))
											{
												if (newNode5.floor.defaultCeilingHeight > 42)
												{
													if (newNode5.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level == 2))
													{
														goto IL_1567;
													}
												}
												if (newNode5.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].ceilingPiece && item.furniture.classes[0].blocksCeiling))
												{
													if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because of other ceiling items");
													}
													flag11 = false;
													break;
												}
												goto IL_165C;
											}
											IL_1567:
											if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
											{
												debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because of nearby air ducts");
											}
											flag11 = false;
											break;
										}
										else if (furnitureClusterRule2.furnitureClass.requiresCeiling && !newNode5.HasValidCeiling())
										{
											if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
											{
												debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because there is no ceiling");
											}
											flag11 = false;
											break;
										}
										IL_165C:
										if (furnitureClusterRule2.furnitureClass.occupiesTile)
										{
											List<NewWall> list13 = newNode5.walls.FindAll((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance);
											if (list13.Count > 0)
											{
												if (list13.Exists((NewWall item) => !item.preset.divider))
												{
													if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because it blocks an entrance");
													}
													flag11 = false;
													break;
												}
											}
										}
										if (furnitureClusterRule2.furnitureClass.windowPiece)
										{
											if (newNode5.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].windowPiece))
											{
												if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because other window piece is here");
												}
												flag11 = false;
												break;
											}
										}
										if (furnitureClusterRule2.furnitureClass.tall || furnitureClusterRule2.furnitureClass.wallPiece)
										{
											if (newNode5.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.window || item.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge))
											{
												if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because it is tall and blocks a window");
												}
												flag11 = false;
												break;
											}
											if (newNode5.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].tall || item.furniture.classes[0].wallPiece))
											{
												if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because other tall/wall furniture is here");
												}
												flag11 = false;
												break;
											}
											if (furnitureClusterRule2.furnitureClass.tall)
											{
												if (newNode5.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level >= 0 && item.level <= 1))
												{
													if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because an air duct is here");
													}
													flag11 = false;
													break;
												}
											}
										}
										if (furnitureClusterRule2.furnitureClass.ceilingPiece)
										{
											if (newNode5.ceilingAirVent)
											{
												if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because it blocks a ceiling vent");
												}
												flag11 = false;
												break;
											}
											if (newNode5.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].blocksCeiling))
											{
												if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid becuase it blocks the ceiling");
												}
												flag11 = false;
												break;
											}
										}
										if (!furnitureClusterRule2.furnitureClass.allowLightswitch && furnitureClusterRule2.furnitureClass.occupiesTile)
										{
											if (newNode5.walls.Exists((NewWall item) => item.containsLightswitch != null))
											{
												flag11 = false;
												if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because a lightswitch is here");
													break;
												}
												break;
											}
										}
										if (newNode5.allowNewFurniture || !furnitureClusterRule2.furnitureClass.occupiesTile)
										{
											if (flag11)
											{
												list12.Add(newNode5);
											}
										}
										else
										{
											if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
											{
												debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because no new furniture is allowed here");
											}
											flag11 = false;
										}
									}
									else
									{
										flag11 = false;
									}
									if (!flag11)
									{
										break;
									}
									num17++;
								}
								if (!flag11)
								{
									break;
								}
								num16++;
							}
							if (enableDebug && debugFurnitureAnglePlacement != null && !flag11)
							{
								debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid after size check");
							}
							if (flag11)
							{
								if (enableDebug && debugFurnitureAnglePlacement != null)
								{
									debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is valid after size check");
								}
								int num18 = 0;
								if (furnitureClusterRule2.furnitureClass.minimumNodeDistance > 0f && flag11)
								{
									using (List<FurnitureClass>.Enumerator enumerator10 = furnitureClusterRule2.furnitureClass.awayFromClasses.GetEnumerator())
									{
										while (enumerator10.MoveNext())
										{
											FurnitureClass furnClass = enumerator10.Current;
											List<FurnitureLocation> list14 = room.individualFurniture.FindAll((FurnitureLocation item) => item.furnitureClasses.Contains(furnClass));
											if (room.floor != null)
											{
												for (int m = 0; m < room.entrances.Count; m++)
												{
													NewNode.NodeAccess nodeAccess = room.entrances[m];
													if (nodeAccess.door == null && nodeAccess.accessType != NewNode.NodeAccess.AccessType.window && nodeAccess.accessType != NewNode.NodeAccess.AccessType.bannister && nodeAccess.toNode.floor == room.floor && nodeAccess.fromNode.floor == room.floor)
													{
														for (int n = 0; n < nodeAccess.toNode.room.individualFurniture.Count; n++)
														{
															FurnitureLocation furnitureLocation = nodeAccess.toNode.room.individualFurniture[n];
															if (furnitureLocation != null && furnitureLocation.furnitureClasses != null && furnitureLocation.furnitureClasses.Contains(furnClass))
															{
																list14.Add(furnitureLocation);
															}
														}
													}
												}
											}
											foreach (FurnitureLocation furnitureLocation2 in list14)
											{
												foreach (NewNode newNode7 in furnitureLocation2.coversNodes)
												{
													foreach (NewNode newNode8 in list12)
													{
														if (Vector3.Distance(newNode7.nodeCoord, newNode8.nodeCoord) < furnitureClusterRule2.furnitureClass.minimumNodeDistance)
														{
															flag11 = false;
															break;
														}
													}
													if (!flag11)
													{
														break;
													}
												}
												if (!flag11)
												{
													break;
												}
											}
											if (!flag11)
											{
												break;
											}
										}
									}
									if (enableDebug && !flag11 && enableDebug && debugFurnitureAnglePlacement != null)
									{
										debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " fail on minimum distance");
									}
								}
								if (flag11)
								{
									foreach (FurnitureClass.FurnitureWallRule furnitureWallRule2 in furnitureClusterRule2.furnitureClass.wallRules)
									{
										Vector2 v2 = CityData.Instance.GetOffsetFromDirection(furnitureWallRule2.wallDirection);
										Vector2 vector8 = Toolbox.Instance.RotateVector2CW(furnitureWallRule2.nodeOffset, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
										Vector3Int vector3Int5 = newNode4.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector8.x), Mathf.RoundToInt(vector8.y), 0);
										Vector2 vector9 = Toolbox.Instance.RotateVector2CW(v2, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing))) * 0.5f;
										vector9..ctor(Mathf.Round(vector9.x * 2f) / 2f, Mathf.Round(vector9.y * 2f) / 2f);
										NewNode newNode9 = null;
										bool flag12 = true;
										bool flag13 = false;
										if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int5, ref newNode9))
										{
											flag13 = true;
											NewWall foundWall = null;
											if (newNode9.wallDict.TryGetValue(vector9, ref foundWall))
											{
												if (furnitureClusterRule2.furnitureClass.wallPiece && foundWall.placedWallFurn)
												{
													flag12 = false;
												}
												if (!furnitureClusterRule2.furnitureClass.allowLightswitch && foundWall.containsLightswitch != null)
												{
													flag12 = false;
												}
												if (flag12)
												{
													if (furnitureWallRule2.tag == FurnitureClass.WallRule.entrance)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.entranceDoorOnly)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
														{
															flag12 = false;
														}
														else if (foundWall.preset.divider)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.entraceDivider)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
														{
															flag12 = false;
														}
														else if (!foundWall.preset.divider)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.entranceToRoomOfType && furnitureWallRule2.roomType != null)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance || foundWall.otherWall.node.room.preset != furnitureWallRule2.roomType)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.addressEntrance)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance || foundWall.otherWall.node.room.gameLocation == foundWall.node.room.gameLocation)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.nothing)
													{
														flag12 = false;
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.ventLower)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventLower)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.ventUpper)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventUpper)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.ventTop)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventTop)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.wall)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.window)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.window)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.windowLarge)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.windowLarge)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.anyWindow)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.windowLarge && foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.window)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.wallOrUpperVent)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall && foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventUpper)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.fence)
													{
														if (!foundWall.preset.isFence)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.lightswitch)
													{
														if (foundWall.lightswitchInteractable == null)
														{
															flag12 = false;
														}
													}
													else if (furnitureWallRule2.tag == FurnitureClass.WallRule.securityDoorDivider)
													{
														if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
														{
															flag12 = false;
														}
														else if (!foundWall.preset.divider)
														{
															flag12 = false;
														}
														else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.never)
														{
															flag12 = false;
														}
														else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.onlyToOtherAddress)
														{
															if (foundWall.node.gameLocation == foundWall.otherWall.node.gameLocation)
															{
																flag12 = false;
															}
														}
														else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.onlyToStairwell)
														{
															bool flag14 = false;
															using (HashSet<NewNode>.Enumerator enumerator4 = foundWall.otherWall.node.room.nodes.GetEnumerator())
															{
																while (enumerator4.MoveNext())
																{
																	if (enumerator4.Current.tile.isStairwell)
																	{
																		flag14 = true;
																		break;
																	}
																}
															}
															if (!flag14)
															{
																flag12 = false;
															}
														}
														if (flag12)
														{
															Predicate<NewNode> <>9__15;
															foreach (Interactable interactable2 in foundWall.node.floor.securityDoors)
															{
																List<NewNode> coversNodes2 = interactable2.furnitureParent.coversNodes;
																Predicate<NewNode> predicate2;
																if ((predicate2 = <>9__15) == null)
																{
																	predicate2 = (<>9__15 = ((NewNode item) => item == foundWall.node || item == foundWall.otherWall.node));
																}
																if (coversNodes2.Exists(predicate2))
																{
																	flag12 = false;
																	break;
																}
															}
														}
														if (flag12 && room.floor.floor != 0)
														{
															bool flag15 = false;
															foreach (NewGameLocation newGameLocation2 in room.floor.addresses)
															{
																new List<AirDuctGroup.AirDuctSection>();
																new List<AirDuctGroup.AirDuctSection>();
																foreach (NewNode newNode10 in newGameLocation2.nodes)
																{
																	foreach (AirDuctGroup.AirDuctSection airDuctSection2 in newNode10.airDucts)
																	{
																		List<AirDuctGroup.AirVent> list5;
																		List<Vector3Int> list15;
																		List<Vector3Int> list16;
																		airDuctSection2.GetNeighborSections(out list15, out list5, out list16);
																		list15.AddRange(list16);
																		if (airDuctSection2.level == 0)
																		{
																			if (list15.Exists((Vector3Int item) => item.z < 0))
																			{
																				flag15 = true;
																				break;
																			}
																		}
																	}
																}
															}
															if (!flag15)
															{
																flag12 = false;
															}
														}
													}
												}
											}
											else if (furnitureWallRule2.tag != FurnitureClass.WallRule.nothing)
											{
												flag12 = false;
											}
										}
										else
										{
											flag12 = false;
										}
										if (furnitureWallRule2.option == FurnitureClass.FurnitureRuleOption.canFeature)
										{
											if (flag12)
											{
												num18 += furnitureWallRule2.addScore;
											}
										}
										else if (furnitureWallRule2.option == FurnitureClass.FurnitureRuleOption.cantFeature)
										{
											if (flag12 || !flag13)
											{
												flag11 = false;
												break;
											}
										}
										else if (furnitureWallRule2.option == FurnitureClass.FurnitureRuleOption.mustFeature && (!flag12 || !flag13))
										{
											flag11 = false;
											break;
										}
									}
									if (enableDebug && !flag11 && enableDebug && debugFurnitureAnglePlacement != null)
									{
										debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " fail on wall rules");
									}
								}
								if (flag11)
								{
									foreach (FurnitureClass.FurnitureNodeRule furnitureNodeRule in furnitureClusterRule2.furnitureClass.nodeRules)
									{
										Vector2 vector10 = Toolbox.Instance.RotateVector2CW(furnitureNodeRule.offset, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
										Vector3Int vector3Int6 = newNode4.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector10.x), Mathf.RoundToInt(vector10.y), 0);
										NewNode newNode11 = null;
										bool flag16 = false;
										if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int6, ref newNode11))
										{
											foreach (FurnitureClusterLocation furnitureClusterLocation in room.furniture)
											{
												List<FurnitureLocation> list17 = null;
												bool flag17 = false;
												if (furnitureClusterLocation.clusterObjectMap.TryGetValue(newNode11, ref list17))
												{
													foreach (FurnitureLocation furnitureLocation3 in list17)
													{
														if (furnitureNodeRule.anyOccupiedTile)
														{
															if (furnitureLocation3.furnitureClasses.Exists((FurnitureClass item) => item.occupiesTile))
															{
																flag17 = true;
																break;
															}
														}
														if (furnitureLocation3.furnitureClasses.Contains(furnitureNodeRule.furnitureClass))
														{
															flag17 = true;
															break;
														}
													}
												}
												if (flag17)
												{
													flag16 = true;
													break;
												}
											}
										}
										if (furnitureNodeRule.option == FurnitureClass.FurnitureRuleOption.canFeature)
										{
											if (flag16)
											{
												num18 += furnitureNodeRule.addScore;
											}
										}
										else if (furnitureNodeRule.option == FurnitureClass.FurnitureRuleOption.cantFeature)
										{
											if (flag16)
											{
												flag11 = false;
												break;
											}
										}
										else if (furnitureNodeRule.option == FurnitureClass.FurnitureRuleOption.mustFeature && !flag16)
										{
											flag11 = false;
											break;
										}
									}
									if (enableDebug && !flag11 && enableDebug && debugFurnitureAnglePlacement != null)
									{
										debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " fail on tile rules");
									}
								}
								if (flag11)
								{
									foreach (FurnitureClass.BlockedAccess blockedAccess in furnitureClusterRule2.furnitureClass.blockedAccess)
									{
										if (!blockedAccess.disabled)
										{
											Vector2 vector11 = Toolbox.Instance.RotateVector2CW(blockedAccess.nodeOffset, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
											Vector3Int vector3Int7 = newNode4.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector11.x), Mathf.RoundToInt(vector11.y), 0);
											NewNode newNode12 = null;
											if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int7, ref newNode12))
											{
												foreach (CityData.BlockingDirection dir in blockedAccess.blocked)
												{
													Vector2 v3 = CityData.Instance.GetOffsetFromDirection(dir);
													Vector2 vector12 = Toolbox.Instance.RotateVector2CW(v3, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
													Vector3Int vector3Int8 = newNode12.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector12.x), Mathf.RoundToInt(vector12.y), 0);
													NewNode newNode13 = null;
													if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int8, ref newNode13))
													{
														if (!dictionary5.ContainsKey(newNode12))
														{
															dictionary5.Add(newNode12, new List<NewNode>());
														}
														dictionary5[newNode12].Add(newNode13);
														if (!dictionary5.ContainsKey(newNode13))
														{
															dictionary5.Add(newNode13, new List<NewNode>());
														}
														dictionary5[newNode13].Add(newNode12);
													}
												}
											}
											if (blockedAccess.blockExteriorDiagonals && blockedAccess.blocked.Contains(CityData.BlockingDirection.behindLeft))
											{
												Vector2 vector13 = Toolbox.Instance.RotateVector2CW(new Vector2(-1f, 0f), (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
												Vector3Int vector3Int9 = vector3Int7 + new Vector3Int(Mathf.RoundToInt(vector13.x), Mathf.RoundToInt(vector13.y), 0);
												NewNode newNode14 = null;
												if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int9, ref newNode14))
												{
													Vector2 vector14 = Toolbox.Instance.RotateVector2CW(new Vector2(0f, -1f), (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
													Vector3Int vector3Int10 = vector3Int7 + new Vector3Int(Mathf.RoundToInt(vector14.x), Mathf.RoundToInt(vector14.y), 0);
													NewNode newNode15 = null;
													if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int10, ref newNode15))
													{
														try
														{
															if (!dictionary5.ContainsKey(newNode14))
															{
																dictionary5.Add(newNode14, new List<NewNode>());
															}
															dictionary5[newNode14].Add(newNode15);
														}
														catch
														{
														}
														try
														{
															if (!dictionary5.ContainsKey(newNode15))
															{
																dictionary5.Add(newNode15, new List<NewNode>());
															}
															dictionary5[newNode15].Add(newNode14);
														}
														catch
														{
														}
													}
												}
											}
										}
									}
									if (furnitureClusterRule2.furnitureClass.blockedAccess.Count > 0 || furnitureClusterRule2.furnitureClass.noPassThrough)
									{
										Dictionary<NewNode, List<NewNode>> dictionary6 = new Dictionary<NewNode, List<NewNode>>(dictionary3);
										foreach (KeyValuePair<NewNode, List<NewNode>> keyValuePair2 in dictionary5)
										{
											if (!dictionary6.ContainsKey(keyValuePair2.Key))
											{
												dictionary6.Add(keyValuePair2.Key, new List<NewNode>());
											}
											dictionary6[keyValuePair2.Key].AddRange(keyValuePair2.Value);
										}
										List<NewNode> list18 = new List<NewNode>(list7);
										if (furnitureClusterRule2.furnitureClass.noPassThrough)
										{
											list18.AddRange(list9);
										}
										List<NewNode> list19 = new List<NewNode>(list8);
										if (furnitureClusterRule2.furnitureClass.noAccessNeeded)
										{
											list19.AddRange(list9);
										}
										foreach (FurnitureClass.BlockedAccess blockedAccess2 in furnitureClusterRule2.furnitureClass.blockedAccess.FindAll((FurnitureClass.BlockedAccess item) => item.blocked.Count >= 8))
										{
											if (!blockedAccess2.disabled)
											{
												Vector2 vector15 = Toolbox.Instance.RotateVector2CW(blockedAccess2.nodeOffset, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
												Vector3Int vector3Int11 = newNode4.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector15.x), Mathf.RoundToInt(vector15.y), 0);
												NewNode newNode16 = null;
												if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int11, ref newNode16))
												{
													if (list19 == null)
													{
														list19 = new List<NewNode>();
													}
													if (!list19.Contains(newNode16))
													{
														list19.Add(newNode16);
													}
												}
											}
										}
										if (!furnitureClusterRule2.furnitureClass.noBlocking)
										{
											List<string> pathingLog = null;
											flag11 = this.IsFurniturePlacementValid(room, ref dictionary6, ref list18, ref list19, cluster.enableDebug, out pathingLog, false);
											if (enableDebug && !flag11 && enableDebug && debugFurnitureAnglePlacement != null)
											{
												debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " fail on pathing check");
												debugFurnitureAnglePlacement.pathingLog = pathingLog;
											}
										}
									}
									if (flag11)
									{
										List<FurnitureClass> list20 = new List<FurnitureClass>();
										list20.Add(furnitureClusterRule2.furnitureClass);
										int num19 = this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing);
										for (num19 += num9; num19 >= 360; num19 -= 360)
										{
										}
										foreach (KeyValuePair<NewNode, List<NewNode>> keyValuePair3 in dictionary5)
										{
											if (!dictionary3.ContainsKey(keyValuePair3.Key))
											{
												dictionary3.Add(keyValuePair3.Key, new List<NewNode>());
											}
											dictionary3[keyValuePair3.Key].AddRange(keyValuePair3.Value);
										}
										if (furnitureClusterRule2.furnitureClass.noPassThrough)
										{
											list7.AddRange(list12);
										}
										if (furnitureClusterRule2.furnitureClass.noAccessNeeded)
										{
											list8.AddRange(list12);
										}
										list9.AddRange(list12);
										FurnitureLocation furnitureLocation4 = new FurnitureLocation(list20, num19, newNode4, list12, furnitureClusterRule2.useFovBlock, furnitureClusterRule2.blockDirection, furnitureClusterRule2.maxFOVBlockDistance, furnitureClusterRule2.localScale, false, furnitureClusterRule2.positionOffset);
										list11.Add(furnitureLocation4);
										num10 += num18;
										num10 += furnitureClusterRule2.placementScoreBoost;
										if (!dictionary4.ContainsKey(furnitureClusterRule2.furnitureClass))
										{
											dictionary4.Add(furnitureClusterRule2.furnitureClass, 1);
										}
										else
										{
											Dictionary<FurnitureClass, int> dictionary7 = dictionary4;
											FurnitureClass furnitureClass = furnitureClusterRule2.furnitureClass;
											int l = dictionary7[furnitureClass];
											dictionary7[furnitureClass] = l + 1;
										}
										if (enableDebug && debugFurnitureAnglePlacement != null)
										{
											List<string> log2 = debugFurnitureAnglePlacement.log;
											string[] array2 = new string[6];
											array2[0] = "...Element is valid at ";
											int num20 = 1;
											Vector3Int nodeCoord = newNode4.nodeCoord;
											array2[num20] = nodeCoord.ToString();
											array2[2] = " ";
											int num21 = 3;
											Vector3 position = newNode4.position;
											array2[num21] = position.ToString();
											array2[4] = " angle ";
											array2[5] = num19.ToString();
											log2.Add(string.Concat(array2));
										}
									}
								}
							}
							if (flag11)
							{
								if (enableDebug && enableDebug && debugFurnitureAnglePlacement != null)
								{
									List<string> log3 = debugFurnitureAnglePlacement.log;
									string[] array3 = new string[8];
									array3[0] = "...Element ";
									array3[1] = furnitureClusterRule2.furnitureClass.name;
									array3[2] = " placement is VALID at ";
									array3[3] = num9.ToString();
									array3[4] = " at ";
									int num22 = 5;
									Vector3Int nodeCoord = newNode4.nodeCoord;
									array3[num22] = nodeCoord.ToString();
									array3[6] = " ";
									int num23 = 7;
									Vector3 position = newNode4.position;
									array3[num23] = position.ToString();
									log3.Add(string.Concat(array3));
								}
								flag9 = true;
								flag10 = true;
								break;
							}
						}
						if (!flag10 && furnitureClusterRule2.importantToCluster)
						{
							flag8 = false;
							break;
						}
					}
					IL_2FF8:;
				}
				if (flag8)
				{
					FurnitureClusterLocation furnitureClusterLocation2 = new FurnitureClusterLocation(newNode, cluster, num9, (float)num10 + Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, room.seed + cluster.presetName, false));
					foreach (FurnitureLocation furnitureLocation5 in list11)
					{
						furnitureLocation5.cluster = furnitureClusterLocation2;
						foreach (NewNode newNode17 in furnitureLocation5.coversNodes)
						{
							if (!furnitureClusterLocation2.clusterObjectMap.ContainsKey(newNode17))
							{
								furnitureClusterLocation2.clusterObjectMap.Add(newNode17, new List<FurnitureLocation>());
							}
							furnitureClusterLocation2.clusterObjectMap[newNode17].Add(furnitureLocation5);
							if (!furnitureClusterLocation2.clusterList.Contains(furnitureLocation5))
							{
								furnitureClusterLocation2.clusterList.Add(furnitureLocation5);
							}
						}
					}
					if (enableDebug && debugFurnitureAnglePlacement != null)
					{
						debugFurnitureAnglePlacement.isValid = true;
						debugFurnitureAnglePlacement.coversNodes = list9;
					}
					list.Add(furnitureClusterLocation2);
				}
				if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug && debugFurnitureAnglePlacement != null)
				{
					if (debugFurnitureAnglePlacement.isValid)
					{
						debugFurnitureAnglePlacement.name = "VALID";
					}
					else
					{
						debugFurnitureAnglePlacement.name = "INVALID";
					}
					furnitureClusterDebug.AddEntry(debugFurnitureAnglePlacement);
					goto IL_3188;
				}
				goto IL_3188;
			}
		}
		return list;
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x0014AA8C File Offset: 0x00148C8C
	public FurnitureClusterLocation GetBestFurnitureClusterLocation(NewRoom room, FurnitureCluster cluster, bool enableDebug = false)
	{
		Stopwatch stopwatch = null;
		CityConstructor.DecorClusterGenerationTimeInfo decorClusterGenerationTimeInfo = null;
		if (Game.Instance.devMode && Game.Instance.collectDebugData && CityConstructor.Instance != null && CityConstructor.Instance.debugLoadTime != null)
		{
			stopwatch = new Stopwatch();
			stopwatch.Start();
			decorClusterGenerationTimeInfo = new CityConstructor.DecorClusterGenerationTimeInfo();
			decorClusterGenerationTimeInfo.cluster = cluster;
		}
		FurnitureClusterLocation furnitureClusterLocation = null;
		List<FurnitureCluster.FurnitureClusterRule> list = new List<FurnitureCluster.FurnitureClusterRule>();
		Dictionary<FurnitureClass, List<FurnitureLocation>> dictionary = null;
		Dictionary<FurnitureClass, List<FurnitureLocation>> dictionary2 = null;
		for (int i = 0; i < cluster.clusterElements.Count; i++)
		{
			FurnitureCluster.FurnitureClusterRule furnitureClusterRule = cluster.clusterElements[i];
			if (enableDebug)
			{
				string[] array = new string[5];
				array[0] = "Searching placements for element ";
				int num = 1;
				int j = i + 1;
				array[num] = j.ToString();
				array[2] = "/";
				int num2 = 3;
				j = cluster.clusterElements.Count;
				array[num2] = j.ToString();
				array[4] = "...";
				Game.Log(string.Concat(array), 2);
			}
			bool flag = true;
			if (furnitureClusterRule == null || furnitureClusterRule.furnitureClass == null)
			{
				flag = false;
				Game.LogError("Null furniture class found in cluster " + cluster.presetName + " index " + i.ToString(), 2);
			}
			else
			{
				if (furnitureClusterRule.furnitureClass.limitPerRoom)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<FurnitureClass, List<FurnitureLocation>>();
					}
					int num3 = 0;
					if (dictionary.ContainsKey(furnitureClusterRule.furnitureClass))
					{
						num3 = dictionary[furnitureClusterRule.furnitureClass].Count;
					}
					else
					{
						List<FurnitureLocation> furnitureInRoom = this.GetFurnitureInRoom(room, furnitureClusterRule.furnitureClass);
						num3 += furnitureInRoom.Count;
						dictionary.Add(furnitureClusterRule.furnitureClass, furnitureInRoom);
					}
					if (num3 >= furnitureClusterRule.furnitureClass.maximumNumberPerRoom)
					{
						if (enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Element Room limit reached (",
								num3.ToString(),
								"/",
								furnitureClusterRule.furnitureClass.maximumNumberPerRoom.ToString(),
								")"
							}), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return furnitureClusterLocation;
						}
					}
				}
				int num4 = 9999;
				int num5 = 9999;
				if (furnitureClusterRule.furnitureClass.limitPerBuildingResidence)
				{
					if (room.building == null)
					{
						if (enableDebug)
						{
							Game.Log("Element per residence reached", 2);
						}
						flag = false;
					}
					else
					{
						int num6 = 0;
						foreach (KeyValuePair<int, NewFloor> keyValuePair in room.building.floors)
						{
							num6 += keyValuePair.Value.addresses.FindAll((NewAddress item) => item.residence != null).Count;
						}
						num4 = Mathf.CeilToInt((float)num6 / (float)furnitureClusterRule.furnitureClass.perBuildingResidences);
					}
				}
				if (furnitureClusterRule.furnitureClass.limitPerJobs)
				{
					if (room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.company != null)
					{
						num5 = Mathf.CeilToInt((float)room.gameLocation.thisAsAddress.company.companyRoster.Count / (float)furnitureClusterRule.furnitureClass.perJobs);
					}
					else
					{
						flag = false;
					}
				}
				if (furnitureClusterRule.furnitureClass.limitPerAddress || furnitureClusterRule.furnitureClass.limitPerBuildingResidence || furnitureClusterRule.furnitureClass.limitPerJobs)
				{
					if (dictionary2 == null)
					{
						dictionary2 = new Dictionary<FurnitureClass, List<FurnitureLocation>>();
					}
					int num7 = 0;
					if (dictionary2.ContainsKey(furnitureClusterRule.furnitureClass))
					{
						num7 = dictionary2[furnitureClusterRule.furnitureClass].Count;
					}
					else
					{
						List<FurnitureLocation> furnitureInGameLocation = this.GetFurnitureInGameLocation(room.gameLocation, furnitureClusterRule.furnitureClass);
						num7 += furnitureInGameLocation.Count;
						dictionary2.Add(furnitureClusterRule.furnitureClass, furnitureInGameLocation);
					}
					if (num7 >= furnitureClusterRule.furnitureClass.maximumNumberPerAddress && furnitureClusterRule.furnitureClass.limitPerAddress)
					{
						if (enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Element Room limit reached (",
								num7.ToString(),
								"/",
								furnitureClusterRule.furnitureClass.maximumNumberPerAddress.ToString(),
								")"
							}), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return furnitureClusterLocation;
						}
					}
					else if (num7 >= num4 && furnitureClusterRule.furnitureClass.limitPerBuildingResidence)
					{
						if (enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Element Room limit reached residences (",
								num7.ToString(),
								"/",
								num4.ToString(),
								")"
							}), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return furnitureClusterLocation;
						}
					}
					else if (num7 >= num5 && furnitureClusterRule.furnitureClass.limitPerJobs)
					{
						if (enableDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Element Room limit reached residences (",
								num7.ToString(),
								"/",
								num5.ToString(),
								")"
							}), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return furnitureClusterLocation;
						}
					}
				}
				if (flag && furnitureClusterRule.furnitureClass.skipIfNoAddressInhabitants && room.gameLocation != null && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.inhabitants.Count <= 0)
				{
					if (enableDebug)
					{
						Game.Log("Element Skipping as no inhabitants...", 2);
					}
					flag = false;
					if (furnitureClusterRule.importantToCluster)
					{
						return furnitureClusterLocation;
					}
				}
				if (flag && Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, room.seed, false) > furnitureClusterRule.chanceOfPlacementAttempt)
				{
					if (enableDebug)
					{
						Game.Log("Element Skipping because of element placement chance (" + furnitureClusterRule.chanceOfPlacementAttempt.ToString() + ")", 2);
					}
					flag = false;
				}
				if (flag && furnitureClusterRule.furnitureClass.limitToFloor && Mathf.RoundToInt((float)Enumerable.FirstOrDefault<NewNode>(room.nodes).nodeCoord.z) != furnitureClusterRule.furnitureClass.allowedOnFloor)
				{
					if (enableDebug)
					{
						Game.Log("Element Skipping as limited to floor " + furnitureClusterRule.furnitureClass.allowedOnFloor.ToString(), 2);
					}
					flag = false;
					if (furnitureClusterRule.importantToCluster)
					{
						return furnitureClusterLocation;
					}
				}
				if (flag && furnitureClusterRule.furnitureClass.limitToFloorRange)
				{
					int num8 = Mathf.RoundToInt((float)Enumerable.FirstOrDefault<NewNode>(room.nodes).nodeCoord.z);
					if ((float)num8 < furnitureClusterRule.furnitureClass.allowedOnFloorRange.x || (float)num8 > furnitureClusterRule.furnitureClass.allowedOnFloorRange.y)
					{
						if (enableDebug)
						{
							Game.Log("Element Skipping as limited to floor range " + furnitureClusterRule.furnitureClass.limitToFloorRange.ToString(), 2);
						}
						flag = false;
						if (furnitureClusterRule.importantToCluster)
						{
							return furnitureClusterLocation;
						}
					}
				}
				if (flag)
				{
					List<FurniturePreset> list2;
					flag = this.GetValidFurniture(furnitureClusterRule.furnitureClass, room, false, out list2, false);
					if (enableDebug)
					{
						Game.Log("Valid furniture exists for element: " + flag.ToString(), 2);
					}
				}
				if (flag)
				{
					if (enableDebug)
					{
						Game.Log("Element: Is a valid element...", 2);
					}
					list.Add(furnitureClusterRule);
				}
				else
				{
					if (enableDebug)
					{
						Game.Log("Element: Unable to find valid furniture for this cluster/room combination", 2);
					}
					if (furnitureClusterRule.importantToCluster)
					{
						return furnitureClusterLocation;
					}
				}
			}
		}
		foreach (NewNode newNode in room.nodes)
		{
			if (cluster.useCustomZeroNodeMinWallCount && newNode.walls.Count < cluster.customZeroNodeMinWallCount)
			{
				if (enableDebug)
				{
					Game.Log("Element: Custom zero node min wall count reached", 2);
				}
			}
			else if (cluster.useCustomZeroNodeMaxWallCount && newNode.walls.Count > cluster.customZeroNodeMaxWallCount)
			{
				if (enableDebug)
				{
					Game.Log("Element: Custom zero node max wall count reached", 2);
				}
			}
			else if (newNode.walls.Count < cluster.minimumZeroNodeWallCount)
			{
				if (enableDebug)
				{
					Game.Log("Element: Zero node min wall count reached", 2);
				}
			}
			else if (newNode.walls.Count > cluster.maximumZeroNodeWallCount)
			{
				if (enableDebug)
				{
					Game.Log("Element: Zero node max wall count reached", 2);
				}
			}
			else
			{
				if (cluster.coastalOnly && !SessionData.Instance.isFloorEdit)
				{
					bool flag2 = true;
					foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
					{
						Vector3Int vector3Int = newNode.tile.globalTileCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
						NewTile newTile = null;
						if (PathFinder.Instance.tileMap.TryGetValue(vector3Int, ref newTile) && newTile.cityTile.building != null && !newTile.cityTile.building.preset.boundary)
						{
							flag2 = false;
						}
					}
					if (!flag2)
					{
						if (enableDebug)
						{
							Game.Log("Element: Coastal only", 2);
							continue;
						}
						continue;
					}
				}
				FurnitureClusterDebug furnitureClusterDebug = null;
				if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug)
				{
					Transform transform;
					if (SessionData.Instance.isFloorEdit)
					{
						transform = FloorEditController.Instance.debugContainer;
					}
					else
					{
						transform = PrefabControls.Instance.debugDecorContainer;
					}
					furnitureClusterDebug = Object.Instantiate<GameObject>(PrefabControls.Instance.furnitureDebug, transform).GetComponent<FurnitureClusterDebug>();
					furnitureClusterDebug.Setup(cluster, newNode);
				}
				foreach (int num9 in CityData.Instance.angleArrayX4)
				{
					bool flag3 = true;
					int num10 = 0;
					Dictionary<NewNode, List<NewNode>> dictionary3 = new Dictionary<NewNode, List<NewNode>>();
					List<NewNode> list3 = new List<NewNode>();
					List<NewNode> list4 = new List<NewNode>();
					List<NewNode> list5 = new List<NewNode>();
					List<NewWall> list6 = new List<NewWall>();
					List<FurnitureLocation> list7 = new List<FurnitureLocation>();
					Dictionary<FurnitureClass, int> dictionary4 = new Dictionary<FurnitureClass, int>();
					bool flag4 = false;
					FurnitureClusterDebug.DebugFurnitureAnglePlacement debugFurnitureAnglePlacement = null;
					if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug)
					{
						debugFurnitureAnglePlacement = new FurnitureClusterDebug.DebugFurnitureAnglePlacement();
						debugFurnitureAnglePlacement.angle = num9;
						debugFurnitureAnglePlacement.log = new List<string>();
					}
					for (int k = 0; k < list.Count; k++)
					{
						FurnitureCluster.FurnitureClusterRule furnitureClusterRule2 = list[k];
						if (!furnitureClusterRule2.onlyValidIfPreviousObjectPlaced || flag4)
						{
							flag4 = false;
							bool flag5 = false;
							if (flag3 && furnitureClusterRule2.furnitureClass.limitPerRoom)
							{
								int num11 = dictionary[furnitureClusterRule2.furnitureClass].Count;
								int num12 = 0;
								if (dictionary4.TryGetValue(furnitureClusterRule2.furnitureClass, ref num12))
								{
									num11 += num12;
								}
								if (num11 >= furnitureClusterRule2.furnitureClass.maximumNumberPerRoom)
								{
									if (furnitureClusterRule2.importantToCluster)
									{
										flag3 = false;
										break;
									}
									goto IL_29DB;
								}
							}
							if (flag3 && furnitureClusterRule2.furnitureClass.limitPerAddress)
							{
								int num13 = dictionary2[furnitureClusterRule2.furnitureClass].Count;
								int num14 = 0;
								if (dictionary4.TryGetValue(furnitureClusterRule2.furnitureClass, ref num14))
								{
									num13 += num14;
								}
								if (num13 >= furnitureClusterRule2.furnitureClass.maximumNumberPerAddress)
								{
									if (furnitureClusterRule2.importantToCluster)
									{
										flag3 = false;
										break;
									}
									goto IL_29DB;
								}
							}
							if (flag3)
							{
								bool limitPerBuildingResidence = furnitureClusterRule2.furnitureClass.limitPerBuildingResidence;
							}
							if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug && debugFurnitureAnglePlacement != null)
							{
								debugFurnitureAnglePlacement.log.Add(string.Concat(new string[]
								{
									"Anylizing placements for element ",
									k.ToString(),
									"/",
									cluster.clusterElements.Count.ToString(),
									" ",
									furnitureClusterRule2.furnitureClass.name,
									"..."
								}));
							}
							foreach (Vector2 vector in furnitureClusterRule2.placements)
							{
								bool flag6 = true;
								if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug && debugFurnitureAnglePlacement != null)
								{
									List<string> log = debugFurnitureAnglePlacement.log;
									string text = "Anylizing placement ";
									Vector2 vector2 = vector;
									log.Add(text + vector2.ToString());
								}
								Dictionary<NewNode, List<NewNode>> dictionary5 = new Dictionary<NewNode, List<NewNode>>();
								List<NewNode> list8 = new List<NewNode>();
								list6.Clear();
								NewNode newNode2 = null;
								Vector2 vector3 = Toolbox.Instance.RotateVector2CW(vector, (float)num9);
								int num15 = num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing) - 180;
								int num16 = 0;
								while ((float)num16 < furnitureClusterRule2.furnitureClass.objectSize.x)
								{
									int num17 = 0;
									while ((float)num17 < furnitureClusterRule2.furnitureClass.objectSize.y)
									{
										Vector2 vector4 = Toolbox.Instance.RotateVector2CW(new Vector2((float)num16, (float)num17), (float)num15);
										Vector2 vector5 = vector3 + vector4;
										Vector3Int vector3Int2 = newNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector5.x), Mathf.RoundToInt(vector5.y), 0);
										NewNode newNode3 = null;
										if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode3))
										{
											if (newNode2 == null)
											{
												newNode2 = newNode3;
												if (newNode2.walls.Count < furnitureClusterRule2.furnitureClass.minimumZeroNodeWallCount)
												{
													flag6 = false;
													break;
												}
												if (newNode2.walls.Count > furnitureClusterRule2.furnitureClass.maximumZeroNodeWallCount)
												{
													flag6 = false;
													break;
												}
											}
											if (newNode3.room != room)
											{
												flag6 = false;
												break;
											}
											if (!furnitureClusterRule2.furnitureClass.allowedOnStairwell)
											{
												if (newNode3.tile.isStairwell)
												{
													flag6 = false;
													break;
												}
												foreach (Vector2Int vector2Int2 in CityData.Instance.offsetArrayX4)
												{
													Vector3Int vector3Int3 = newNode3.nodeCoord + new Vector3Int(vector2Int2.x, vector2Int2.y, 0);
													NewNode newNode4 = null;
													if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int3, ref newNode4) && newNode4.tile.isStairwell)
													{
														flag6 = false;
														break;
													}
												}
												if (!flag6)
												{
													break;
												}
											}
											else if (furnitureClusterRule2.furnitureClass.onlyOnStairwell && !newNode3.tile.isStairwell)
											{
												flag6 = false;
												break;
											}
											if (newNode3.gameLocation.thisAsStreet == null && !furnitureClusterRule2.furnitureClass.allowIfNoFloor && !newNode3.HasValidFloor())
											{
												flag6 = false;
												break;
											}
											if (furnitureClusterRule2.furnitureClass.ceilingPiece)
											{
												if (newNode3.floorType == NewNode.FloorTileType.floorOnly || newNode3.floorType == NewNode.FloorTileType.none || newNode3.floorType == NewNode.FloorTileType.noneButIndoors)
												{
													flag6 = false;
													break;
												}
												if (!newNode3.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level == 1))
												{
													if (newNode3.floor.defaultCeilingHeight > 42)
													{
														if (newNode3.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level == 2))
														{
															goto IL_EBB;
														}
													}
													if (newNode3.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].ceilingPiece && item.furniture.classes[0].blocksCeiling))
													{
														flag6 = false;
														break;
													}
													goto IL_F45;
												}
												IL_EBB:
												flag6 = false;
												break;
											}
											else if (furnitureClusterRule2.furnitureClass.requiresCeiling && !newNode3.HasValidCeiling())
											{
												if (enableDebug && debugFurnitureAnglePlacement != null)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " failed as this requires ceiling");
												}
												flag6 = false;
												break;
											}
											IL_F45:
											if (furnitureClusterRule2.furnitureClass.occupiesTile)
											{
												List<NewWall> list9 = newNode3.walls.FindAll((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance);
												if (list9.Count > 0)
												{
													if (list9.Exists((NewWall item) => !item.preset.divider))
													{
														flag6 = false;
														break;
													}
												}
											}
											if (furnitureClusterRule2.furnitureClass.windowPiece)
											{
												if (newNode3.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].windowPiece))
												{
													if (enableDebug && debugFurnitureAnglePlacement != null)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " failed as window already has a piece here");
													}
													flag6 = false;
													break;
												}
											}
											if (furnitureClusterRule2.furnitureClass.tall || furnitureClusterRule2.furnitureClass.wallPiece)
											{
												if (newNode3.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.window || item.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge))
												{
													if (enableDebug && debugFurnitureAnglePlacement != null)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " failed as window is blocked by tall furniture");
													}
													flag6 = false;
													break;
												}
												if (newNode3.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].tall || item.furniture.classes[0].wallPiece))
												{
													if (enableDebug && debugFurnitureAnglePlacement != null)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " failed as wall is blocked by tall furniture");
													}
													flag6 = false;
													break;
												}
												if (furnitureClusterRule2.furnitureClass.tall)
												{
													if (newNode3.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level >= 0 && item.level <= 1))
													{
														if (enableDebug && debugFurnitureAnglePlacement != null && !flag6)
														{
															debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid because an air duct is here");
														}
														flag6 = false;
														break;
													}
												}
											}
											if (furnitureClusterRule2.furnitureClass.ceilingPiece)
											{
												if (newNode3.ceilingAirVent)
												{
													if (enableDebug && debugFurnitureAnglePlacement != null)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " failed as ceiling vent would be blocked");
													}
													flag6 = false;
													break;
												}
												if (newNode3.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].blocksCeiling))
												{
													if (enableDebug && debugFurnitureAnglePlacement != null)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " failed as ceiling is blocked");
													}
													flag6 = false;
													break;
												}
											}
											if (newNode3.gameLocation.thisAsStreet == null && !furnitureClusterRule2.furnitureClass.allowLightswitch && furnitureClusterRule2.furnitureClass.occupiesTile)
											{
												if (newNode3.walls.Exists((NewWall item) => item.containsLightswitch != null))
												{
													if (enableDebug && debugFurnitureAnglePlacement != null)
													{
														debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " failed as lightswitch is not allowed");
													}
													flag6 = false;
													break;
												}
											}
											if (newNode3.allowNewFurniture || !furnitureClusterRule2.furnitureClass.occupiesTile)
											{
												if (flag6)
												{
													list8.Add(newNode3);
												}
											}
											else
											{
												if (enableDebug && debugFurnitureAnglePlacement != null)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " failed as no new furniture is allowed here");
												}
												flag6 = false;
											}
										}
										else
										{
											flag6 = false;
										}
										if (!flag6)
										{
											break;
										}
										num17++;
									}
									if (!flag6)
									{
										break;
									}
									num16++;
								}
								if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug && debugFurnitureAnglePlacement != null && !flag6)
								{
									debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is invalid after size check");
								}
								if (flag6)
								{
									if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug && debugFurnitureAnglePlacement != null)
									{
										debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " is valid after size check");
									}
									int num18 = 0;
									if (furnitureClusterRule2.furnitureClass.minimumNodeDistance > 0f && flag6)
									{
										using (List<FurnitureClass>.Enumerator enumerator4 = furnitureClusterRule2.furnitureClass.awayFromClasses.GetEnumerator())
										{
											while (enumerator4.MoveNext())
											{
												FurnitureClass furnClass = enumerator4.Current;
												List<FurnitureLocation> list10 = room.individualFurniture.FindAll((FurnitureLocation item) => item.furnitureClasses.Contains(furnClass));
												if (room.floor != null)
												{
													for (int m = 0; m < room.entrances.Count; m++)
													{
														NewNode.NodeAccess nodeAccess = room.entrances[m];
														if (nodeAccess.door == null && nodeAccess.accessType != NewNode.NodeAccess.AccessType.window && nodeAccess.accessType != NewNode.NodeAccess.AccessType.bannister && nodeAccess.toNode.floor == room.floor && nodeAccess.fromNode.floor == room.floor)
														{
															for (int n = 0; n < nodeAccess.toNode.room.individualFurniture.Count; n++)
															{
																FurnitureLocation furnitureLocation = nodeAccess.toNode.room.individualFurniture[n];
																if (furnitureLocation != null && furnitureLocation.furnitureClasses != null && furnitureLocation.furnitureClasses.Contains(furnClass))
																{
																	list10.Add(furnitureLocation);
																}
															}
														}
													}
												}
												foreach (FurnitureLocation furnitureLocation2 in list10)
												{
													foreach (NewNode newNode5 in furnitureLocation2.coversNodes)
													{
														foreach (NewNode newNode6 in list8)
														{
															if (Vector3.Distance(newNode5.nodeCoord, newNode6.nodeCoord) < furnitureClusterRule2.furnitureClass.minimumNodeDistance)
															{
																flag6 = false;
																break;
															}
														}
														if (!flag6)
														{
															break;
														}
													}
													if (!flag6)
													{
														break;
													}
												}
												if (!flag6)
												{
													break;
												}
											}
										}
										if (enableDebug && !flag6 && ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug) && debugFurnitureAnglePlacement != null)
										{
											debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " fail on minimum distance");
										}
									}
									if (flag6)
									{
										foreach (FurnitureClass.FurnitureWallRule furnitureWallRule in furnitureClusterRule2.furnitureClass.wallRules)
										{
											Vector2 v = CityData.Instance.GetOffsetFromDirection(furnitureWallRule.wallDirection);
											Vector2 vector6 = Toolbox.Instance.RotateVector2CW(furnitureWallRule.nodeOffset, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
											Vector3Int vector3Int4 = newNode2.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector6.x), Mathf.RoundToInt(vector6.y), 0);
											Vector2 vector7 = Toolbox.Instance.RotateVector2CW(v, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing))) * 0.5f;
											vector7..ctor(Mathf.Round(vector7.x * 2f) / 2f, Mathf.Round(vector7.y * 2f) / 2f);
											NewNode newNode7 = null;
											bool flag7 = true;
											bool flag8 = false;
											if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int4, ref newNode7))
											{
												flag8 = true;
												NewWall foundWall = null;
												if (newNode7.wallDict.TryGetValue(vector7, ref foundWall))
												{
													if (furnitureClusterRule2.furnitureClass.wallPiece && foundWall.placedWallFurn)
													{
														flag7 = false;
													}
													if (!furnitureClusterRule2.furnitureClass.allowLightswitch && foundWall.containsLightswitch != null)
													{
														flag7 = false;
													}
													if (flag7)
													{
														if (furnitureWallRule.tag == FurnitureClass.WallRule.entrance)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.entranceDoorOnly)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
															{
																flag7 = false;
															}
															else if (foundWall.preset.divider)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.entraceDivider)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
															{
																flag7 = false;
															}
															else if (!foundWall.preset.divider)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.entranceToRoomOfType && furnitureWallRule.roomType != null)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance || foundWall.otherWall.node.room.preset != furnitureWallRule.roomType)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.addressEntrance)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance || foundWall.otherWall.node.room.gameLocation == foundWall.node.room.gameLocation)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.nothing)
														{
															flag7 = false;
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.ventLower)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventLower)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.ventUpper)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventUpper)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.ventTop)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventTop)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.wall)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.window)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.window)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.windowLarge)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.windowLarge)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.anyWindow)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.windowLarge && foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.window)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.wallOrUpperVent)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall && foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventUpper)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.fence)
														{
															if (!foundWall.preset.isFence)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.lightswitch)
														{
															if (foundWall.lightswitchInteractable == null)
															{
																flag7 = false;
															}
														}
														else if (furnitureWallRule.tag == FurnitureClass.WallRule.securityDoorDivider)
														{
															if (foundWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
															{
																flag7 = false;
															}
															else if (!foundWall.preset.divider)
															{
																flag7 = false;
															}
															else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.never)
															{
																flag7 = false;
															}
															else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.onlyToOtherAddress)
															{
																if (foundWall.node.gameLocation == foundWall.otherWall.node.gameLocation)
																{
																	flag7 = false;
																}
															}
															else if (room.preset.securityDoors == RoomConfiguration.SecurityDoorRule.onlyToStairwell)
															{
																bool flag9 = false;
																using (HashSet<NewNode>.Enumerator enumerator9 = foundWall.otherWall.node.room.nodes.GetEnumerator())
																{
																	while (enumerator9.MoveNext())
																	{
																		if (enumerator9.Current.tile.isStairwell)
																		{
																			flag9 = true;
																			break;
																		}
																	}
																}
																if (!flag9)
																{
																	flag7 = false;
																}
															}
															if (flag7)
															{
																Predicate<NewNode> <>9__13;
																foreach (Interactable interactable in foundWall.node.floor.securityDoors)
																{
																	List<NewNode> coversNodes = interactable.furnitureParent.coversNodes;
																	Predicate<NewNode> predicate;
																	if ((predicate = <>9__13) == null)
																	{
																		predicate = (<>9__13 = ((NewNode item) => item == foundWall.node || item == foundWall.otherWall.node));
																	}
																	if (coversNodes.Exists(predicate))
																	{
																		flag7 = false;
																		break;
																	}
																}
															}
															if (flag7 && room.floor.floor != 0)
															{
																bool flag10 = false;
																foreach (NewGameLocation newGameLocation in room.floor.addresses)
																{
																	new List<AirDuctGroup.AirDuctSection>();
																	new List<AirDuctGroup.AirDuctSection>();
																	foreach (NewNode newNode8 in newGameLocation.nodes)
																	{
																		foreach (AirDuctGroup.AirDuctSection airDuctSection in newNode8.airDucts)
																		{
																			List<Vector3Int> list11;
																			List<AirDuctGroup.AirVent> list12;
																			List<Vector3Int> list13;
																			airDuctSection.GetNeighborSections(out list11, out list12, out list13);
																			list11.AddRange(list13);
																			if (airDuctSection.level == 0)
																			{
																				if (list11.Exists((Vector3Int item) => item.z < 0))
																				{
																					flag10 = true;
																					break;
																				}
																			}
																		}
																	}
																}
																if (!flag10)
																{
																	flag7 = false;
																}
															}
														}
													}
												}
												else if (furnitureWallRule.tag != FurnitureClass.WallRule.nothing)
												{
													flag7 = false;
												}
											}
											else
											{
												flag7 = false;
											}
											if (furnitureWallRule.option == FurnitureClass.FurnitureRuleOption.canFeature)
											{
												if (flag7)
												{
													num18 += furnitureWallRule.addScore;
												}
											}
											else if (furnitureWallRule.option == FurnitureClass.FurnitureRuleOption.cantFeature)
											{
												if (flag7 || !flag8)
												{
													flag6 = false;
													break;
												}
											}
											else if (furnitureWallRule.option == FurnitureClass.FurnitureRuleOption.mustFeature && (!flag7 || !flag8))
											{
												flag6 = false;
												break;
											}
										}
										if (enableDebug && !flag6 && ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug) && debugFurnitureAnglePlacement != null)
										{
											debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " fail on wall rules");
										}
									}
									if (flag6)
									{
										foreach (FurnitureClass.FurnitureNodeRule furnitureNodeRule in furnitureClusterRule2.furnitureClass.nodeRules)
										{
											Vector2 vector8 = Toolbox.Instance.RotateVector2CW(furnitureNodeRule.offset, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
											Vector3Int vector3Int5 = newNode2.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector8.x), Mathf.RoundToInt(vector8.y), 0);
											NewNode newNode9 = null;
											bool flag11 = false;
											if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int5, ref newNode9))
											{
												foreach (FurnitureClusterLocation furnitureClusterLocation2 in room.furniture)
												{
													List<FurnitureLocation> list14 = null;
													bool flag12 = false;
													if (furnitureClusterLocation2.clusterObjectMap.TryGetValue(newNode9, ref list14))
													{
														foreach (FurnitureLocation furnitureLocation3 in list14)
														{
															if (furnitureNodeRule.anyOccupiedTile)
															{
																if (furnitureLocation3.furnitureClasses.Exists((FurnitureClass item) => item.occupiesTile))
																{
																	flag12 = true;
																	break;
																}
															}
															if (furnitureLocation3.furnitureClasses.Contains(furnitureNodeRule.furnitureClass))
															{
																flag12 = true;
																break;
															}
														}
													}
													if (flag12)
													{
														flag11 = true;
														break;
													}
												}
											}
											if (furnitureNodeRule.option == FurnitureClass.FurnitureRuleOption.canFeature)
											{
												if (flag11)
												{
													num18 += furnitureNodeRule.addScore;
												}
											}
											else if (furnitureNodeRule.option == FurnitureClass.FurnitureRuleOption.cantFeature)
											{
												if (flag11)
												{
													flag6 = false;
													break;
												}
											}
											else if (furnitureNodeRule.option == FurnitureClass.FurnitureRuleOption.mustFeature && !flag11)
											{
												flag6 = false;
												break;
											}
										}
										if (enableDebug && !flag6 && ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug) && debugFurnitureAnglePlacement != null)
										{
											debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " fail on tile rules");
										}
									}
									if (flag6)
									{
										foreach (FurnitureClass.BlockedAccess blockedAccess in furnitureClusterRule2.furnitureClass.blockedAccess)
										{
											if (!blockedAccess.disabled)
											{
												Vector2 vector9 = Toolbox.Instance.RotateVector2CW(blockedAccess.nodeOffset, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
												Vector3Int vector3Int6 = newNode2.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector9.x), Mathf.RoundToInt(vector9.y), 0);
												NewNode newNode10 = null;
												if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int6, ref newNode10))
												{
													foreach (CityData.BlockingDirection dir in blockedAccess.blocked)
													{
														Vector2 v2 = CityData.Instance.GetOffsetFromDirection(dir);
														Vector2 vector10 = Toolbox.Instance.RotateVector2CW(v2, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
														Vector3Int vector3Int7 = newNode10.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector10.x), Mathf.RoundToInt(vector10.y), 0);
														NewNode newNode11 = null;
														if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int7, ref newNode11))
														{
															if (!dictionary5.ContainsKey(newNode10))
															{
																dictionary5.Add(newNode10, new List<NewNode>());
															}
															dictionary5[newNode10].Add(newNode11);
															if (!dictionary5.ContainsKey(newNode11))
															{
																dictionary5.Add(newNode11, new List<NewNode>());
															}
															dictionary5[newNode11].Add(newNode10);
														}
													}
												}
												if (blockedAccess.blockExteriorDiagonals && blockedAccess.blocked.Contains(CityData.BlockingDirection.behindLeft))
												{
													Vector2 vector11 = Toolbox.Instance.RotateVector2CW(new Vector2(-1f, 0f), (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
													Vector3Int vector3Int8 = vector3Int6 + new Vector3Int(Mathf.RoundToInt(vector11.x), Mathf.RoundToInt(vector11.y), 0);
													NewNode newNode12 = null;
													if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int8, ref newNode12))
													{
														Vector2 vector12 = Toolbox.Instance.RotateVector2CW(new Vector2(0f, -1f), (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
														Vector3Int vector3Int9 = vector3Int6 + new Vector3Int(Mathf.RoundToInt(vector12.x), Mathf.RoundToInt(vector12.y), 0);
														NewNode newNode13 = null;
														if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int9, ref newNode13))
														{
															try
															{
																if (!dictionary5.ContainsKey(newNode12))
																{
																	dictionary5.Add(newNode12, new List<NewNode>());
																}
																dictionary5[newNode12].Add(newNode13);
															}
															catch
															{
															}
															try
															{
																if (!dictionary5.ContainsKey(newNode13))
																{
																	dictionary5.Add(newNode13, new List<NewNode>());
																}
																dictionary5[newNode13].Add(newNode12);
															}
															catch
															{
															}
														}
													}
												}
											}
										}
										if (furnitureClusterRule2.furnitureClass.blockedAccess.Count > 0 || furnitureClusterRule2.furnitureClass.noPassThrough)
										{
											Dictionary<NewNode, List<NewNode>> dictionary6 = new Dictionary<NewNode, List<NewNode>>(dictionary3);
											foreach (KeyValuePair<NewNode, List<NewNode>> keyValuePair2 in dictionary5)
											{
												if (!dictionary6.ContainsKey(keyValuePair2.Key))
												{
													dictionary6.Add(keyValuePair2.Key, new List<NewNode>());
												}
												dictionary6[keyValuePair2.Key].AddRange(keyValuePair2.Value);
											}
											List<NewNode> list15 = new List<NewNode>(list3);
											if (furnitureClusterRule2.furnitureClass.noPassThrough)
											{
												list15.AddRange(list5);
											}
											List<NewNode> list16 = new List<NewNode>(list4);
											if (furnitureClusterRule2.furnitureClass.noAccessNeeded)
											{
												list16.AddRange(list5);
											}
											foreach (FurnitureClass.BlockedAccess blockedAccess2 in furnitureClusterRule2.furnitureClass.blockedAccess.FindAll((FurnitureClass.BlockedAccess item) => item.blocked.Count >= 8))
											{
												if (!blockedAccess2.disabled)
												{
													Vector2 vector13 = Toolbox.Instance.RotateVector2CW(blockedAccess2.nodeOffset, (float)(num9 + this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing)));
													Vector3Int vector3Int10 = newNode2.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector13.x), Mathf.RoundToInt(vector13.y), 0);
													NewNode newNode14 = null;
													if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int10, ref newNode14))
													{
														if (list16 == null)
														{
															list16 = new List<NewNode>();
														}
														if (!list16.Contains(newNode14))
														{
															list16.Add(newNode14);
														}
													}
												}
											}
											if (!furnitureClusterRule2.furnitureClass.noBlocking)
											{
												List<string> pathingLog = null;
												flag6 = this.IsFurniturePlacementValid(room, ref dictionary6, ref list15, ref list16, enableDebug, out pathingLog, false);
												if (enableDebug && !flag6 && ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug) && debugFurnitureAnglePlacement != null)
												{
													debugFurnitureAnglePlacement.log.Add("...Element " + furnitureClusterRule2.furnitureClass.name + " fail on pathing check");
													debugFurnitureAnglePlacement.pathingLog = pathingLog;
												}
											}
										}
										if (flag6)
										{
											List<FurnitureClass> list17 = new List<FurnitureClass>();
											list17.Add(furnitureClusterRule2.furnitureClass);
											int num19 = this.GetAngleForFurnitureFacing(furnitureClusterRule2.facing);
											for (num19 += num9; num19 >= 360; num19 -= 360)
											{
											}
											foreach (KeyValuePair<NewNode, List<NewNode>> keyValuePair3 in dictionary5)
											{
												if (!dictionary3.ContainsKey(keyValuePair3.Key))
												{
													dictionary3.Add(keyValuePair3.Key, new List<NewNode>());
												}
												dictionary3[keyValuePair3.Key].AddRange(keyValuePair3.Value);
											}
											if (furnitureClusterRule2.furnitureClass.noPassThrough)
											{
												list3.AddRange(list8);
											}
											if (furnitureClusterRule2.furnitureClass.noAccessNeeded)
											{
												list4.AddRange(list8);
											}
											list5.AddRange(list8);
											FurnitureLocation furnitureLocation4 = new FurnitureLocation(list17, num19, newNode2, list8, furnitureClusterRule2.useFovBlock, furnitureClusterRule2.blockDirection, furnitureClusterRule2.maxFOVBlockDistance, furnitureClusterRule2.localScale, false, furnitureClusterRule2.positionOffset);
											list7.Add(furnitureLocation4);
											num10 += num18;
											num10 += furnitureClusterRule2.placementScoreBoost;
											if (!dictionary4.ContainsKey(furnitureClusterRule2.furnitureClass))
											{
												dictionary4.Add(furnitureClusterRule2.furnitureClass, 1);
											}
											else
											{
												Dictionary<FurnitureClass, int> dictionary7 = dictionary4;
												FurnitureClass furnitureClass = furnitureClusterRule2.furnitureClass;
												int l = dictionary7[furnitureClass];
												dictionary7[furnitureClass] = l + 1;
											}
											if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug && debugFurnitureAnglePlacement != null)
											{
												List<string> log2 = debugFurnitureAnglePlacement.log;
												string[] array2 = new string[6];
												array2[0] = "...Element is valid at ";
												int num20 = 1;
												Vector3Int nodeCoord = newNode2.nodeCoord;
												array2[num20] = nodeCoord.ToString();
												array2[2] = " ";
												int num21 = 3;
												Vector3 position = newNode2.position;
												array2[num21] = position.ToString();
												array2[4] = " angle ";
												array2[5] = num19.ToString();
												log2.Add(string.Concat(array2));
											}
										}
									}
								}
								if (flag6)
								{
									if (enableDebug && ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug) && debugFurnitureAnglePlacement != null)
									{
										List<string> log3 = debugFurnitureAnglePlacement.log;
										string[] array3 = new string[8];
										array3[0] = "...Element ";
										array3[1] = furnitureClusterRule2.furnitureClass.name;
										array3[2] = " placement is VALID at ";
										array3[3] = num9.ToString();
										array3[4] = " at ";
										int num22 = 5;
										Vector3Int nodeCoord = newNode2.nodeCoord;
										array3[num22] = nodeCoord.ToString();
										array3[6] = " ";
										int num23 = 7;
										Vector3 position = newNode2.position;
										array3[num23] = position.ToString();
										log3.Add(string.Concat(array3));
									}
									flag4 = true;
									flag5 = true;
									break;
								}
							}
							if (!flag5 && furnitureClusterRule2.importantToCluster)
							{
								flag3 = false;
								break;
							}
						}
						IL_29DB:;
					}
					if (flag3)
					{
						FurnitureClusterLocation furnitureClusterLocation3 = new FurnitureClusterLocation(newNode, cluster, num9, (float)num10 + Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, room.seed + cluster.presetName, false));
						foreach (FurnitureLocation furnitureLocation5 in list7)
						{
							furnitureLocation5.cluster = furnitureClusterLocation3;
							foreach (NewNode newNode15 in furnitureLocation5.coversNodes)
							{
								if (!furnitureClusterLocation3.clusterObjectMap.ContainsKey(newNode15))
								{
									furnitureClusterLocation3.clusterObjectMap.Add(newNode15, new List<FurnitureLocation>());
								}
								furnitureClusterLocation3.clusterObjectMap[newNode15].Add(furnitureLocation5);
								if (!furnitureClusterLocation3.clusterList.Contains(furnitureLocation5))
								{
									furnitureClusterLocation3.clusterList.Add(furnitureLocation5);
								}
							}
						}
						if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug && debugFurnitureAnglePlacement != null)
						{
							debugFurnitureAnglePlacement.isValid = true;
							debugFurnitureAnglePlacement.coversNodes = list5;
						}
						if (furnitureClusterLocation == null || furnitureClusterLocation3.ranking > furnitureClusterLocation.ranking)
						{
							furnitureClusterLocation = furnitureClusterLocation3;
						}
					}
					if ((SessionData.Instance.isFloorEdit || (Game.Instance.devMode && Game.Instance.collectDebugData)) && enableDebug && debugFurnitureAnglePlacement != null)
					{
						if (debugFurnitureAnglePlacement.isValid)
						{
							debugFurnitureAnglePlacement.name = "VALID";
						}
						else
						{
							debugFurnitureAnglePlacement.name = "INVALID";
						}
						furnitureClusterDebug.AddEntry(debugFurnitureAnglePlacement);
					}
				}
			}
		}
		if (Game.Instance.devMode && Game.Instance.collectDebugData && CityConstructor.Instance != null && CityConstructor.Instance.debugLoadTime != null)
		{
			stopwatch.Stop();
			decorClusterGenerationTimeInfo.time = (float)stopwatch.Elapsed.TotalSeconds;
			if (furnitureClusterLocation != null)
			{
				decorClusterGenerationTimeInfo.found = true;
			}
			else
			{
				decorClusterGenerationTimeInfo.found = false;
			}
			try
			{
				if (!CityConstructor.Instance.debugLoadTime.decorTimes.ContainsKey(room))
				{
					CityConstructor.Instance.debugLoadTime.decorTimes.Add(room, new List<CityConstructor.DecorClusterGenerationTimeInfo>());
				}
				CityConstructor.Instance.debugLoadTime.decorTimes[room].Add(decorClusterGenerationTimeInfo);
			}
			catch
			{
			}
		}
		return furnitureClusterLocation;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x000AAD7C File Offset: 0x000A8F7C
	private int GetAngleForFurnitureFacing(FurnitureCluster.FurnitureFacing facing)
	{
		if (facing == FurnitureCluster.FurnitureFacing.up)
		{
			return 0;
		}
		if (facing == FurnitureCluster.FurnitureFacing.down)
		{
			return 180;
		}
		if (facing == FurnitureCluster.FurnitureFacing.left)
		{
			return 270;
		}
		if (facing == FurnitureCluster.FurnitureFacing.right)
		{
			return 90;
		}
		return 0;
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x0014D99C File Offset: 0x0014BB9C
	public bool IsFurniturePlacementValid(NewRoom room, ref Dictionary<NewNode, List<NewNode>> newBlockAccess, ref List<NewNode> newNoPassNodes, ref List<NewNode> newNoAccessNodes, bool printDebug, out List<string> debugOutput, bool ignoreNoPassThrough = false)
	{
		debugOutput = null;
		if (printDebug)
		{
			debugOutput = new List<string>();
		}
		if (room == null)
		{
			Game.LogError("Trying to place furniture in null room!", 2);
			return false;
		}
		HashSet<NewNode> hashSet = new HashSet<NewNode>();
		foreach (NewNode.NodeAccess nodeAccess in room.entrances)
		{
			if (nodeAccess.walkingAccess && (nodeAccess.accessType == NewNode.NodeAccess.AccessType.door || nodeAccess.accessType == NewNode.NodeAccess.AccessType.openDoorway || nodeAccess.accessType == NewNode.NodeAccess.AccessType.adjacent))
			{
				if (nodeAccess.fromNode.room == room && !hashSet.Contains(nodeAccess.fromNode))
				{
					hashSet.Add(nodeAccess.fromNode);
					if (printDebug)
					{
						List<string> list = debugOutput;
						string text = "Found entrance node at ";
						Vector3 position = nodeAccess.fromNode.position;
						list.Add(text + position.ToString());
					}
				}
				if (nodeAccess.toNode.room == room && !hashSet.Contains(nodeAccess.toNode))
				{
					hashSet.Add(nodeAccess.toNode);
					if (printDebug)
					{
						List<string> list2 = debugOutput;
						string text2 = "Found entrance node at ";
						Vector3 position = nodeAccess.fromNode.position;
						list2.Add(text2 + position.ToString());
					}
				}
			}
		}
		bool result = true;
		foreach (NewNode newNode in hashSet)
		{
			HashSet<NewNode> hashSet2 = new HashSet<NewNode>();
			hashSet2.Add(newNode);
			HashSet<NewNode> hashSet3 = new HashSet<NewNode>();
			foreach (NewNode newNode2 in room.noAccessNodes)
			{
				if (!hashSet3.Contains(newNode2))
				{
					hashSet3.Add(newNode2);
				}
			}
			if (newNoAccessNodes != null)
			{
				foreach (NewNode newNode3 in newNoAccessNodes)
				{
					if (!hashSet3.Contains(newNode3))
					{
						hashSet3.Add(newNode3);
					}
				}
			}
			int num = room.nodes.Count + 2;
			while (hashSet2.Count > 0 && num > 0 && hashSet3.Count < room.nodes.Count)
			{
				NewNode newNode4 = Enumerable.First<NewNode>(hashSet2);
				if ((newNode4.noPassThrough && !ignoreNoPassThrough) || (newNoPassNodes != null && newNoPassNodes.Contains(newNode4) && !ignoreNoPassThrough))
				{
					if (printDebug)
					{
						List<string> list3 = debugOutput;
						string text3 = "Node at ";
						Vector3 position = newNode4.position;
						list3.Add(text3 + position.ToString() + " is reached but does not allow pass-through...");
					}
				}
				else
				{
					foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX8)
					{
						Vector3Int vector3Int = newNode4.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
						NewNode newNode5 = null;
						if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode5) && !(newNode5.room != room) && !hashSet3.Contains(newNode5) && !hashSet2.Contains(newNode5))
						{
							if (Mathf.Abs(vector2Int.x) + Mathf.Abs(vector2Int.y) == 2)
							{
								Vector2 vector;
								vector..ctor((float)vector2Int.x * 0.5f, (float)vector2Int.y * 0.5f);
								Vector2 offset1 = new Vector2(0f, vector.y);
								Vector2 offset2 = new Vector2(vector.x, 0f);
								if (newNode4.walls.Exists((NewWall item) => item.wallOffset == offset1 || item.wallOffset == offset2))
								{
									goto IL_463;
								}
								offset1 = new Vector2(0f, -vector.y);
								offset2 = new Vector2(-vector.x, 0f);
								if (newNode5.walls.Exists((NewWall item) => item.wallOffset == offset1 || item.wallOffset == offset2))
								{
									goto IL_463;
								}
							}
							bool flag = false;
							if (room.blockedAccess == null)
							{
								room.blockedAccess = new Dictionary<NewNode, List<NewNode>>();
							}
							try
							{
								if (room.blockedAccess.ContainsKey(newNode4) && room.blockedAccess[newNode4].Contains(newNode5))
								{
									flag = true;
								}
								if (!flag && newBlockAccess.ContainsKey(newNode4) && newBlockAccess[newNode4].Contains(newNode5))
								{
									flag = true;
								}
							}
							catch
							{
							}
							if (!flag)
							{
								hashSet2.Add(newNode5);
							}
						}
						IL_463:;
					}
				}
				hashSet3.Add(newNode4);
				hashSet2.Remove(newNode4);
				num--;
			}
			if (hashSet3.Count < room.nodes.Count)
			{
				if (printDebug)
				{
					List<string> list4 = debugOutput;
					string[] array = new string[7];
					array[0] = "Fail: Closed Set = ";
					int num2 = 1;
					int i = hashSet3.Count;
					array[num2] = i.ToString();
					array[2] = ", Room Count = ";
					int num3 = 3;
					i = room.nodes.Count;
					array[num3] = i.ToString();
					array[4] = " reachable entrances remaining: ";
					int num4 = 5;
					i = hashSet.Count;
					array[num4] = i.ToString();
					array[6] = ". Unreachable nodes are as follows:";
					list4.Add(string.Concat(array));
					foreach (NewNode newNode6 in room.nodes)
					{
						if (!hashSet3.Contains(newNode6))
						{
							List<string> list5 = debugOutput;
							string text4 = "... ";
							Vector3 position = newNode6.position;
							list5.Add(text4 + position.ToString());
						}
					}
				}
				result = false;
				break;
			}
			result = true;
		}
		return result;
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x0014E008 File Offset: 0x0014C208
	public bool IsFurniturePlacementValidOLD(NewRoom room, ref Dictionary<NewNode, List<NewNode>> newBlockAccess, List<NewNode> newNoPassNodes = null, List<NewNode> newNoAccessNodes = null, bool printDebug = false)
	{
		if (printDebug)
		{
			Game.Log("IsFurniturePlacementValid?... noPassNodes: " + ((newNoPassNodes != null) ? newNoPassNodes.ToString() : null), 2);
		}
		List<NewNode> list = new List<NewNode>();
		List<NewNode> list2 = new List<NewNode>(room.noAccessNodes);
		if (newNoAccessNodes != null)
		{
			list2.AddRange(newNoAccessNodes);
		}
		list.Add(room.entrances[0].fromNode);
		int num = 1000;
		while (list.Count > 0 && num > 0)
		{
			NewNode newNode = list[0];
			if (!newNode.noPassThrough && (newNoPassNodes == null || !newNoPassNodes.Contains(newNode)))
			{
				foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX8)
				{
					Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
					NewNode newNode2 = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2) && !(newNode2.room != room))
					{
						if (Mathf.Abs(vector2Int.x) + Mathf.Abs(vector2Int.y) == 2)
						{
							Vector2 vector;
							vector..ctor((float)vector2Int.x * 0.5f, (float)vector2Int.y * 0.5f);
							Vector2 offset1 = new Vector2(0f, vector.y);
							Vector2 offset2 = new Vector2(vector.x, 0f);
							if (newNode.walls.Exists((NewWall item) => item.wallOffset == offset1 || item.wallOffset == offset2))
							{
								goto IL_243;
							}
							offset1 = new Vector2(0f, -vector.y);
							offset2 = new Vector2(-vector.x, 0f);
							if (newNode2.walls.Exists((NewWall item) => item.wallOffset == offset1 || item.wallOffset == offset2))
							{
								goto IL_243;
							}
						}
						if (!list2.Contains(newNode2))
						{
							bool flag = false;
							if (room.blockedAccess.ContainsKey(newNode) && room.blockedAccess[newNode].Contains(newNode2))
							{
								flag = true;
							}
							if (!flag && newBlockAccess.ContainsKey(newNode) && newBlockAccess[newNode].Contains(newNode2))
							{
								flag = true;
							}
							if (!flag && !list.Contains(newNode2))
							{
								list.Add(newNode2);
							}
						}
					}
					IL_243:;
				}
			}
			list2.Add(newNode);
			list.RemoveAt(0);
			num--;
		}
		if (list2.Count == room.nodes.Count)
		{
			return true;
		}
		if (printDebug)
		{
			Game.Log("Fail: Closed Set = " + list2.Count.ToString() + ", Room Count = " + room.nodes.Count.ToString(), 2);
		}
		return false;
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x0014E2E0 File Offset: 0x0014C4E0
	public FurniturePreset PickFurniture(FurnitureClass furnClass, NewRoom room, string randomSeed, bool debug = false)
	{
		List<FurniturePreset> list = null;
		List<FurniturePreset> list2 = new List<FurniturePreset>();
		if (room.pickFurnitureCache != null && room.pickFurnitureCache.ContainsKey(furnClass) && !furnClass.isSecurityCamera)
		{
			list2 = room.pickFurnitureCache[furnClass];
		}
		else
		{
			this.GetValidFurniture(furnClass, room, true, out list, false);
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			if (list != null && list.Count == 1)
			{
				return list[0];
			}
			foreach (FurniturePreset furniturePreset in list)
			{
				if (room.gameLocation.thisAsAddress != null)
				{
					int num = 2;
					if (furniturePreset.usePersonalityWeighting)
					{
						float num2 = (float)(10 - Mathf.RoundToInt(Mathf.Abs((float)furniturePreset.modernity - ((float)room.gameLocation.designStyle.modernity * 0.9f + room.gameLocation.thisAsAddress.averageCreativity))));
						int num3 = 10 - Mathf.RoundToInt((float)Mathf.Abs(furniturePreset.cleanness - room.preset.cleanness));
						int num4 = 10 - Mathf.RoundToInt(Mathf.Abs((float)furniturePreset.loudness - (room.gameLocation.thisAsAddress.averageExtraversion * 7.5f + (1f - room.gameLocation.thisAsAddress.averageAgreeableness) * 2.5f)));
						int num5 = 10 - Mathf.RoundToInt(Mathf.Abs((float)furniturePreset.emotive - (room.gameLocation.thisAsAddress.averageEmotionality * 7f + room.gameLocation.thisAsAddress.averageCreativity * 3f)));
						Mathf.Max(num = Mathf.FloorToInt((num2 + (float)num3 + (float)num4 + (float)num5) / 8f), 1);
					}
					for (int i = 0; i < num; i++)
					{
						list2.Add(furniturePreset);
					}
				}
				else
				{
					list2.Add(furniturePreset);
				}
			}
			if (room.pickFurnitureCache == null)
			{
				room.pickFurnitureCache = new Dictionary<FurnitureClass, List<FurniturePreset>>();
			}
			if (!room.pickFurnitureCache.ContainsKey(furnClass))
			{
				room.pickFurnitureCache.Add(furnClass, list2);
			}
			else
			{
				room.pickFurnitureCache[furnClass] = list2;
			}
		}
		if (list2.Count > 0)
		{
			return list2[Toolbox.Instance.GetPsuedoRandomNumber(0, list2.Count, randomSeed, false)];
		}
		if (list.Count > 0)
		{
			return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, randomSeed, false)];
		}
		if (!debug)
		{
			Game.LogError("Unable to pick furniture of class " + furnClass.name + " for room " + room.preset.name, 2);
		}
		return null;
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x0014E5A0 File Offset: 0x0014C7A0
	public bool GetValidFurniture(FurnitureClass furnClass, NewRoom room, bool returnList, out List<FurniturePreset> possibleFurniture, bool debug = false)
	{
		float normalizedLandValue = Toolbox.Instance.GetNormalizedLandValue(room.gameLocation, false);
		possibleFurniture = null;
		foreach (FurniturePreset furniturePreset in Toolbox.Instance.furnitureDesignStyleRef[room.gameLocation.designStyle])
		{
			if (!(furniturePreset == null))
			{
				if (debug)
				{
					Game.Log("---" + furniturePreset.name + "---", 2);
				}
				if (normalizedLandValue < furniturePreset.minimumWealth)
				{
					if (debug)
					{
						Game.Log("... Below minimum wealth of " + furniturePreset.minimumWealth.ToString(), 2);
					}
				}
				else if (room.nodes.Count < furniturePreset.minimumRoomSize)
				{
					if (debug)
					{
						Game.Log("... Below minimum room size of " + furniturePreset.minimumRoomSize.ToString(), 2);
					}
				}
				else if (furniturePreset.allowedInOpenPlan != FurnitureCluster.AllowedOpenPlan.no || room.openPlanElements.Count <= 0)
				{
					if (furniturePreset.furnitureGroup != FurniturePreset.FurnitureGroup.none && room.furnitureGroups.ContainsKey(furniturePreset.furnitureGroup) && furniturePreset.groupID != room.furnitureGroups[furniturePreset.furnitureGroup])
					{
						if (debug)
						{
							Game.Log("... Furniture group doesn't match: " + room.furnitureGroups[furniturePreset.furnitureGroup].ToString(), 2);
						}
					}
					else if (!furniturePreset.classes.Contains(furnClass))
					{
						if (debug)
						{
							Game.Log("... Not in class " + furnClass.name, 2);
						}
					}
					else
					{
						if (furniturePreset.isSecurityCamera && room.preset.limitSecurityCameras)
						{
							if (room.individualFurniture.FindAll((FurnitureLocation item) => item.furniture.isSecurityCamera).Count >= room.preset.securityCameraLimit)
							{
								if (debug)
								{
									Game.Log("... Security camera limit...", 2);
									continue;
								}
								continue;
							}
						}
						if (debug)
						{
							Game.Log("... Checking room compatibility...", 2);
						}
						if ((SessionData.Instance.isFloorEdit || ((!furniturePreset.OnlyAllowInBuildings || (!(room.gameLocation.building == null) && furniturePreset.allowedInBuildings.Contains(room.gameLocation.building.preset))) && (!furniturePreset.banFromBuildings || !(room.gameLocation.building != null) || !furniturePreset.notAllowedInBuildings.Contains(room.gameLocation.building.preset)))) && (SessionData.Instance.isFloorEdit || ((!furniturePreset.OnlyAllowInDistricts || furniturePreset.allowedInDistricts.Contains(room.gameLocation.district.preset)) && (!furniturePreset.banFromDistricts || !furniturePreset.notAllowedInDistricts.Contains(room.gameLocation.district.preset)))))
						{
							if (furniturePreset.requiresGenderedInhabitants)
							{
								if (!(room.gameLocation.thisAsAddress != null))
								{
									continue;
								}
								bool flag = true;
								foreach (Human human in room.gameLocation.thisAsAddress.inhabitants)
								{
									if (!furniturePreset.enableIfGenderPresent.Contains(human.gender))
									{
										flag = false;
									}
								}
								if (!flag)
								{
									continue;
								}
							}
							if (furniturePreset.onlyAllowInFollowing)
							{
								if (room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.addressPreset != null)
								{
									if (!furniturePreset.allowedInAddressesOfType.Contains(room.gameLocation.thisAsAddress.addressPreset))
									{
										if (debug)
										{
											Game.Log("... Not allowed in address " + room.gameLocation.thisAsAddress.addressPreset.name, 2);
											continue;
										}
										continue;
									}
								}
								else
								{
									if (debug)
									{
										Game.Log("... Not assigned an address preset", 2);
										continue;
									}
									continue;
								}
							}
							if (furniturePreset.banInFollowing && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.addressPreset != null && furniturePreset.bannedInAddressesOfType.Contains(room.gameLocation.thisAsAddress.addressPreset))
							{
								if (debug)
								{
									Game.Log("... Banned in address " + room.gameLocation.thisAsAddress.addressPreset.name, 2);
								}
							}
							else
							{
								bool flag2 = false;
								using (List<RoomTypeFilter>.Enumerator enumerator3 = furniturePreset.allowedRoomFilters.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										if (enumerator3.Current == null)
										{
											Game.LogError("Null filter found in " + furniturePreset.name, 2);
										}
										else
										{
											if (furniturePreset.allowedInOpenPlan != FurnitureCluster.AllowedOpenPlan.openPlanOnly)
											{
												HashSet<FurniturePreset> hashSet = null;
												if (Toolbox.Instance.furnitureRoomTypeRef.TryGetValue(room.preset.roomClass, ref hashSet) && hashSet.Contains(furniturePreset))
												{
													flag2 = true;
													break;
												}
											}
											foreach (RoomConfiguration roomConfiguration in room.openPlanElements)
											{
												if (furniturePreset.allowedInOpenPlan != FurnitureCluster.AllowedOpenPlan.no)
												{
													HashSet<FurniturePreset> hashSet2 = null;
													if (Toolbox.Instance.furnitureRoomTypeRef.TryGetValue(roomConfiguration.roomClass, ref hashSet2) && hashSet2.Contains(furniturePreset))
													{
														flag2 = true;
														break;
													}
												}
											}
										}
									}
								}
								if (!flag2)
								{
									if (debug)
									{
										Game.Log("... Open plan/filter check failed.", 2);
									}
								}
								else
								{
									if (!returnList)
									{
										return true;
									}
									if (possibleFurniture == null)
									{
										possibleFurniture = new List<FurniturePreset>();
									}
									possibleFurniture.Add(furniturePreset);
								}
							}
						}
					}
				}
			}
		}
		return possibleFurniture != null && possibleFurniture.Count > 0;
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x0014EBEC File Offset: 0x0014CDEC
	public ArtPreset PickArt(ArtPreset.ArtOrientation orientation, NewRoom room)
	{
		float wealthLevel = 0f;
		if (room.gameLocation.thisAsAddress != null)
		{
			wealthLevel = Toolbox.Instance.GetNormalizedLandValue(room.gameLocation.thisAsAddress, false);
		}
		List<ArtPreset> list = new List<ArtPreset>();
		list.AddRange(Toolbox.Instance.allArt.FindAll((ArtPreset item) => item.orientationCompatibility.Contains(orientation) && wealthLevel >= item.minimumWealth && wealthLevel <= item.maximumWealth && !room.gameLocation.artPieces.Contains(item) && !item.disable));
		if (list.Count == 1)
		{
			return list[0];
		}
		List<ArtPreset> list2 = new List<ArtPreset>();
		foreach (ArtPreset artPreset in list)
		{
			if ((artPreset.allowInCommerical || !(room.gameLocation.thisAsAddress != null) || room.gameLocation.thisAsAddress.company == null) && (artPreset.allowInResidential || !(room.gameLocation.thisAsAddress != null) || !(room.gameLocation.thisAsAddress.residence != null)) && (artPreset.allowInLobby || !(room.gameLocation.thisAsAddress != null) || !room.gameLocation.thisAsAddress.isLobby) && (artPreset.allowOnStreet || !(room.gameLocation.thisAsStreet != null)))
			{
				int num = artPreset.basePriority;
				if (room.gameLocation.thisAsAddress != null)
				{
					bool flag = false;
					using (List<ArtPreset.ArtPreference>.Enumerator enumerator2 = artPreset.traitModifiers.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ArtPreset.ArtPreference pref = enumerator2.Current;
							Predicate<Human.Trait> <>9__2;
							foreach (Human human in room.gameLocation.thisAsAddress.owners)
							{
								List<Human.Trait> characterTraits = human.characterTraits;
								Predicate<Human.Trait> predicate;
								if ((predicate = <>9__2) == null)
								{
									predicate = (<>9__2 = ((Human.Trait item) => item.trait == pref.trait));
								}
								if (characterTraits.Exists(predicate))
								{
									flag = true;
									num += pref.modifier;
								}
							}
						}
					}
					if (artPreset.mustRequireTraitFromBelow && !flag)
					{
						continue;
					}
				}
				int num2 = 2;
				int num3 = 2;
				int num4 = 2;
				int num5 = 2;
				int num6 = 0;
				if (room.gameLocation != null && room.gameLocation.thisAsAddress != null)
				{
					num2 = 10 - Mathf.RoundToInt(Mathf.Abs((float)artPreset.modernity - ((float)room.gameLocation.designStyle.modernity * 0.9f + room.gameLocation.thisAsAddress.averageCreativity)));
					num3 = 10 - Mathf.RoundToInt((float)Mathf.Abs(artPreset.cleanness - room.preset.cleanness));
					num4 = 10 - Mathf.RoundToInt(Mathf.Abs((float)artPreset.loudness - (room.gameLocation.thisAsAddress.averageExtraversion * 7.5f + (1f - room.gameLocation.thisAsAddress.averageAgreeableness) * 2.5f)));
					num5 = 10 - Mathf.RoundToInt(Mathf.Abs((float)artPreset.emotive - (room.gameLocation.thisAsAddress.averageEmotionality * 7f + room.gameLocation.thisAsAddress.averageCreativity * 3f)));
					if (room.colourScheme != null)
					{
						float num7 = 0f;
						foreach (Color color in artPreset.colourMatching)
						{
							float num8 = (Mathf.Abs(color.r - room.colourScheme.primary1.r) + Mathf.Abs(color.g - room.colourScheme.primary1.g) + Mathf.Abs(color.b - room.colourScheme.primary1.b)) / 3f;
							float num9 = (Mathf.Abs(color.r - room.colourScheme.primary2.r) + Mathf.Abs(color.g - room.colourScheme.primary2.g) + Mathf.Abs(color.b - room.colourScheme.primary2.b)) / 3f;
							float num10 = (Mathf.Abs(color.r - room.colourScheme.secondary1.r) + Mathf.Abs(color.g - room.colourScheme.secondary1.g) + Mathf.Abs(color.b - room.colourScheme.secondary1.b)) / 3f;
							float num11 = (Mathf.Abs(color.r - room.colourScheme.secondary2.r) + Mathf.Abs(color.g - room.colourScheme.secondary2.g) + Mathf.Abs(color.b - room.colourScheme.secondary2.b)) / 3f;
							num7 += Mathf.Max(new float[]
							{
								num8,
								num9,
								num10,
								num11
							});
						}
						num7 /= (float)artPreset.colourMatching.Count;
						num6 = Mathf.RoundToInt(num7 * (float)artPreset.colourMatchingScale);
					}
				}
				int num12 = Mathf.FloorToInt((float)(num2 + num3 + num4 + num5) / 40f * (float)artPreset.colourMatchingScale) + num6 + num;
				for (int i = 0; i < num12; i++)
				{
					list2.Add(artPreset);
				}
			}
		}
		if (list2.Count > 0)
		{
			return list2[Toolbox.Instance.GetPsuedoRandomNumber(0, list2.Count, room.seed, false)];
		}
		if (list.Count > 0)
		{
			return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, room.seed, false)];
		}
		Game.Log(string.Concat(new string[]
		{
			"CityGen: Unable to pick art of orentation ",
			orientation.ToString(),
			" for ",
			room.roomID.ToString(),
			" with soc level: ",
			wealthLevel.ToString(),
			", picking random..."
		}), 2);
		list.AddRange(Toolbox.Instance.allArt.FindAll((ArtPreset item) => item.orientationCompatibility.Contains(orientation) && !item.disable));
		return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, room.seed, false)];
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x0014F3F8 File Offset: 0x0014D5F8
	private bool GetAdjacentNode(NewNode original, Vector2Int offset, out NewNode output)
	{
		output = null;
		Vector2Int vector2Int = original.floorCoord + offset;
		if (original.floor.nodeMap.ContainsKey(vector2Int))
		{
			output = original.floor.nodeMap[vector2Int];
			return true;
		}
		return false;
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x0014F440 File Offset: 0x0014D640
	public List<FurnitureLocation> GetFurnitureInCity(FurnitureClass furnClass)
	{
		List<FurnitureLocation> list = new List<FurnitureLocation>();
		for (int i = 0; i < HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count; i++)
		{
			foreach (KeyValuePair<int, NewFloor> keyValuePair in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory[i].floors)
			{
				for (int j = 0; j < keyValuePair.Value.addresses.Count; j++)
				{
					NewAddress newAddress = keyValuePair.Value.addresses[j];
					for (int k = 0; k < newAddress.rooms.Count; k++)
					{
						NewRoom newRoom = newAddress.rooms[k];
						for (int l = 0; l < newRoom.individualFurniture.Count; l++)
						{
							FurnitureLocation furnitureLocation = newRoom.individualFurniture[l];
							try
							{
								if (furnitureLocation.furnitureClasses.Contains(furnClass))
								{
									list.Add(furnitureLocation);
								}
							}
							catch
							{
							}
						}
					}
				}
			}
		}
		for (int m = 0; m < CityData.Instance.streetDirectory.Count; m++)
		{
			StreetController streetController = CityData.Instance.streetDirectory[m];
			for (int n = 0; n < streetController.rooms.Count; n++)
			{
				NewRoom newRoom2 = streetController.rooms[n];
				for (int num = 0; num < newRoom2.individualFurniture.Count; num++)
				{
					FurnitureLocation furnitureLocation2 = newRoom2.individualFurniture[num];
					try
					{
						if (furnitureLocation2.furnitureClasses.Contains(furnClass))
						{
							list.Add(furnitureLocation2);
						}
					}
					catch
					{
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x0014F638 File Offset: 0x0014D838
	public List<FurnitureLocation> GetFurnitureInBuilding(NewBuilding building, FurnitureClass furnClass)
	{
		List<FurnitureLocation> list = new List<FurnitureLocation>();
		foreach (KeyValuePair<int, NewFloor> keyValuePair in building.floors)
		{
			for (int i = 0; i < keyValuePair.Value.addresses.Count; i++)
			{
				NewAddress newAddress = keyValuePair.Value.addresses[i];
				for (int j = 0; j < newAddress.rooms.Count; j++)
				{
					NewRoom newRoom = newAddress.rooms[j];
					for (int k = 0; k < newRoom.individualFurniture.Count; k++)
					{
						FurnitureLocation furnitureLocation = newRoom.individualFurniture[k];
						try
						{
							if (furnitureLocation.furnitureClasses.Contains(furnClass))
							{
								list.Add(furnitureLocation);
							}
						}
						catch
						{
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x0014F748 File Offset: 0x0014D948
	public List<FurnitureLocation> GetFurnitureInGameLocation(NewGameLocation address, FurnitureClass furnClass)
	{
		List<FurnitureLocation> list = new List<FurnitureLocation>();
		for (int i = 0; i < address.rooms.Count; i++)
		{
			NewRoom newRoom = address.rooms[i];
			for (int j = 0; j < newRoom.individualFurniture.Count; j++)
			{
				FurnitureLocation furnitureLocation = newRoom.individualFurniture[j];
				if (furnitureLocation.furnitureClasses.Contains(furnClass))
				{
					list.Add(furnitureLocation);
				}
			}
		}
		return list;
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x0014F7BC File Offset: 0x0014D9BC
	public List<FurnitureLocation> GetFurnitureInRoom(NewRoom room, FurnitureClass furnClass)
	{
		List<FurnitureLocation> list = new List<FurnitureLocation>();
		for (int i = 0; i < room.individualFurniture.Count; i++)
		{
			FurnitureLocation furnitureLocation = room.individualFurniture[i];
			if (furnitureLocation.furnitureClasses.Contains(furnClass))
			{
				list.Add(furnitureLocation);
			}
		}
		return list;
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x0014F808 File Offset: 0x0014DA08
	public List<FurnitureClusterLocation> GetClustersInCity(FurnitureCluster cluster)
	{
		List<FurnitureClusterLocation> list = new List<FurnitureClusterLocation>();
		for (int i = 0; i < HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count; i++)
		{
			foreach (KeyValuePair<int, NewFloor> keyValuePair in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory[i].floors)
			{
				for (int j = 0; j < keyValuePair.Value.addresses.Count; j++)
				{
					NewAddress newAddress = keyValuePair.Value.addresses[j];
					for (int k = 0; k < newAddress.rooms.Count; k++)
					{
						NewRoom newRoom = newAddress.rooms[k];
						for (int l = 0; l < newRoom.furniture.Count; l++)
						{
							FurnitureClusterLocation furnitureClusterLocation = newRoom.furniture[l];
							try
							{
								if (furnitureClusterLocation.cluster == cluster)
								{
									list.Add(furnitureClusterLocation);
								}
							}
							catch
							{
							}
						}
					}
				}
			}
		}
		for (int m = 0; m < CityData.Instance.streetDirectory.Count; m++)
		{
			StreetController streetController = CityData.Instance.streetDirectory[m];
			for (int n = 0; n < streetController.rooms.Count; n++)
			{
				NewRoom newRoom2 = streetController.rooms[n];
				for (int num = 0; num < newRoom2.furniture.Count; num++)
				{
					FurnitureClusterLocation furnitureClusterLocation2 = newRoom2.furniture[num];
					try
					{
						if (furnitureClusterLocation2.cluster == cluster)
						{
							list.Add(furnitureClusterLocation2);
						}
					}
					catch
					{
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x0014FA00 File Offset: 0x0014DC00
	public List<FurnitureClusterLocation> GetClustersInBuilding(NewBuilding building, FurnitureCluster cluster)
	{
		List<FurnitureClusterLocation> list = new List<FurnitureClusterLocation>();
		foreach (KeyValuePair<int, NewFloor> keyValuePair in building.floors)
		{
			for (int i = 0; i < keyValuePair.Value.addresses.Count; i++)
			{
				NewAddress newAddress = keyValuePair.Value.addresses[i];
				for (int j = 0; j < newAddress.rooms.Count; j++)
				{
					NewRoom newRoom = newAddress.rooms[j];
					for (int k = 0; k < newRoom.furniture.Count; k++)
					{
						FurnitureClusterLocation furnitureClusterLocation = newRoom.furniture[k];
						try
						{
							if (furnitureClusterLocation.cluster == cluster)
							{
								list.Add(furnitureClusterLocation);
							}
						}
						catch
						{
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x0014FB10 File Offset: 0x0014DD10
	public List<FurnitureClusterLocation> GetClustersInGameLocation(NewGameLocation address, FurnitureCluster cluster)
	{
		List<FurnitureClusterLocation> list = new List<FurnitureClusterLocation>();
		for (int i = 0; i < address.rooms.Count; i++)
		{
			NewRoom newRoom = address.rooms[i];
			for (int j = 0; j < newRoom.furniture.Count; j++)
			{
				FurnitureClusterLocation furnitureClusterLocation = newRoom.furniture[j];
				try
				{
					if (furnitureClusterLocation.cluster == cluster)
					{
						list.Add(furnitureClusterLocation);
					}
				}
				catch
				{
				}
			}
		}
		return list;
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x0014FB98 File Offset: 0x0014DD98
	public void ClearCache()
	{
		Game.Log("Clearing interior generation cache...", 2);
		Toolbox.Instance.furnitureDesignStyleRef = null;
		Toolbox.Instance.furnitureRoomTypeRef = null;
	}

	// Token: 0x04001AE7 RID: 6887
	private bool updateGeometryActive;

	// Token: 0x04001AE8 RID: 6888
	private List<NewFloor> updateTheseFloors = new List<NewFloor>();

	// Token: 0x04001AE9 RID: 6889
	private bool loadGeometryActive;

	// Token: 0x04001AEA RID: 6890
	private List<NewFloor> loadTheseFloors = new List<NewFloor>();

	// Token: 0x04001AEB RID: 6891
	private bool roomUnloadCheckActive;

	// Token: 0x04001AEC RID: 6892
	public int oldestRoomUnloadTimer;

	// Token: 0x04001AED RID: 6893
	public List<NewRoom> spawnedRooms = new List<NewRoom>();

	// Token: 0x04001AEE RID: 6894
	private static GenerationController _instance;

	// Token: 0x020003C0 RID: 960
	[Serializable]
	public class PossibleRoomLocation : IComparable<GenerationController.PossibleRoomLocation>
	{
		// Token: 0x060015E0 RID: 5600 RVA: 0x0014FBE4 File Offset: 0x0014DDE4
		public int CompareTo(GenerationController.PossibleRoomLocation otherObject)
		{
			return this.ranking.CompareTo(otherObject.ranking);
		}

		// Token: 0x04001AEF RID: 6895
		public List<NewNode> nodes = new List<NewNode>();

		// Token: 0x04001AF0 RID: 6896
		public float randomRanking;

		// Token: 0x04001AF1 RID: 6897
		public float exteriorWindowRanking;

		// Token: 0x04001AF2 RID: 6898
		public float exteriorWallsRanking;

		// Token: 0x04001AF3 RID: 6899
		public float floorSpaceRanking;

		// Token: 0x04001AF4 RID: 6900
		public float entrancesRanking;

		// Token: 0x04001AF5 RID: 6901
		public List<GenerationController.OverrideData> overrideRankingData = new List<GenerationController.OverrideData>();

		// Token: 0x04001AF6 RID: 6902
		public float ranking;

		// Token: 0x04001AF7 RID: 6903
		public List<NewNode> requiredAdjoiningOptions = new List<NewNode>();

		// Token: 0x04001AF8 RID: 6904
		public List<NewNode> requiredHallway = new List<NewNode>();

		// Token: 0x04001AF9 RID: 6905
		public GenerationDebugController debugScript;
	}

	// Token: 0x020003C1 RID: 961
	[Serializable]
	public struct OverrideData
	{
		// Token: 0x04001AFA RID: 6906
		public NewRoom room;

		// Token: 0x04001AFB RID: 6907
		public float floorSpacePenalty;

		// Token: 0x04001AFC RID: 6908
		public float exteriorWindowPenalty;

		// Token: 0x04001AFD RID: 6909
		public float exteriorWallPenalty;

		// Token: 0x04001AFE RID: 6910
		public float overridingPenalty;
	}

	// Token: 0x020003C2 RID: 962
	[Serializable]
	public class PossibleDoorwayLocation : IComparable<GenerationController.PossibleDoorwayLocation>
	{
		// Token: 0x060015E2 RID: 5602 RVA: 0x0014FC2B File Offset: 0x0014DE2B
		public int CompareTo(GenerationController.PossibleDoorwayLocation otherObject)
		{
			return this.ranking.CompareTo(otherObject.ranking);
		}

		// Token: 0x04001AFF RID: 6911
		public NewWall wall;

		// Token: 0x04001B00 RID: 6912
		public float ranking;

		// Token: 0x04001B01 RID: 6913
		public bool requireFlatDoorway;

		// Token: 0x04001B02 RID: 6914
		public List<NewWall> roomDivider;
	}

	// Token: 0x020003C3 RID: 963
	[Serializable]
	public class PossibleNullExpansion : IComparable<GenerationController.PossibleNullExpansion>
	{
		// Token: 0x060015E4 RID: 5604 RVA: 0x0014FC3E File Offset: 0x0014DE3E
		public int CompareTo(GenerationController.PossibleNullExpansion otherObject)
		{
			return this.ranking.CompareTo(otherObject.ranking);
		}

		// Token: 0x04001B03 RID: 6915
		public List<NewNode> nodesToExpand = new List<NewNode>();

		// Token: 0x04001B04 RID: 6916
		public NewRoom addToRoom;

		// Token: 0x04001B05 RID: 6917
		public float ranking;
	}

	// Token: 0x020003C4 RID: 964
	public struct ClusterRank
	{
		// Token: 0x04001B06 RID: 6918
		public FurnitureCluster cluster;

		// Token: 0x04001B07 RID: 6919
		public float rank;
	}
}
