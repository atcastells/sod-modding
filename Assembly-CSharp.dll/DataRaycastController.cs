using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class DataRaycastController : MonoBehaviour
{
	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000A39 RID: 2617 RVA: 0x00097AD0 File Offset: 0x00095CD0
	public static DataRaycastController Instance
	{
		get
		{
			return DataRaycastController._instance;
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00097AD7 File Offset: 0x00095CD7
	private void Awake()
	{
		if (DataRaycastController._instance != null && DataRaycastController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		DataRaycastController._instance = this;
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x00097B05 File Offset: 0x00095D05
	public bool EntranceRaycast(NewNode.NodeAccess fromEntrance, NewNode.NodeAccess toEntrance, out List<DataRaycastController.NodeRaycastHit> path, bool debugMode = false)
	{
		return this.NodeRaycast(fromEntrance.toNode, toEntrance.fromNode, out path, fromEntrance.door, debugMode);
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00097B24 File Offset: 0x00095D24
	public bool NodeRaycast(NewNode fromNode, NewNode toNode, out List<DataRaycastController.NodeRaycastHit> path, NewDoor startingDoor = null, bool debugMode = false)
	{
		path = new List<DataRaycastController.NodeRaycastHit>();
		toNode.nodeCoord - fromNode.nodeCoord;
		int num = fromNode.nodeCoord.x;
		int num2 = fromNode.nodeCoord.y;
		int num3 = fromNode.nodeCoord.z;
		int num4 = toNode.nodeCoord.x;
		int num5 = toNode.nodeCoord.y;
		int num6 = toNode.nodeCoord.z;
		int num7 = Mathf.Abs(num4 - num);
		int num8 = (num < num4) ? 1 : -1;
		int num9 = Mathf.Abs(num5 - num2);
		int num10 = (num2 < num5) ? 1 : -1;
		int num11 = Mathf.Abs(num6 - num3);
		int num12 = (num3 < num6) ? 1 : -1;
		int num13 = Mathf.Max(new int[]
		{
			num7,
			num9,
			num11
		});
		int num14 = num13;
		num5 = (num4 = (num6 = num13 / 2));
		Vector3Int vector3Int = fromNode.nodeCoord;
		List<int> list = new List<int>();
		if (startingDoor != null && !startingDoor.preset.isTransparent)
		{
			list.Add(startingDoor.wall.id);
		}
		float num15 = 0f;
		NewNode newNode = null;
		NewNode newNode2 = null;
		NewWall newWall = null;
		for (;;)
		{
			Vector3Int vector3Int2;
			vector3Int2..ctor(num, num2, num3);
			newNode = null;
			int num16 = vector3Int.z - vector3Int2.z;
			if (vector3Int2 != fromNode.nodeCoord && PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode))
			{
				num15 += PathFinder.Instance.nodeSize.x;
				if (num16 > 0)
				{
					if ((newNode.floorType == NewNode.FloorTileType.floorAndCeiling || newNode.floorType == NewNode.FloorTileType.CeilingOnly) && !this.TestAdjacentForNoCeilingAdjBannister(newNode))
					{
						break;
					}
				}
				else if (num16 < 0 && (newNode.floorType == NewNode.FloorTileType.floorAndCeiling || newNode.floorType == NewNode.FloorTileType.floorOnly) && !this.TestAdjacentForNoFloorAdjBannister(newNode))
				{
					return false;
				}
				newNode2 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2))
				{
					Vector2 vector = new Vector2((float)(vector3Int2.x - vector3Int.x), (float)(vector3Int2.y - vector3Int.y)) * 0.5f;
					if (debugMode)
					{
						string[] array = new string[5];
						array[0] = "Wall offset for ";
						int num17 = 1;
						Vector3 position = newNode2.position;
						array[num17] = position.ToString();
						array[2] = " is ";
						int num18 = 3;
						Vector2 vector2 = vector;
						array[num18] = vector2.ToString();
						array[4] = "...";
						Game.Log(string.Concat(array), 2);
					}
					if (vector.x != 0f)
					{
						newWall = null;
						if (newNode2.wallDict.TryGetValue(new Vector2(vector.x, 0f), ref newWall))
						{
							if (debugMode)
							{
								Game.Log("...Wall found: " + newWall.preset.sectionClass.ToString(), 2);
							}
							if (!newWall.preset.ignoreCullingRaycasts)
							{
								if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.window || newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge || (newWall.door != null && newWall.door.preset.isTransparent))
								{
									if (newNode2.room.IsOutside() && !newNode.room.IsOutside() && newNode.nodeCoord.z > 0)
									{
										return false;
									}
									if (num15 > CullingControls.Instance.windowCullingRange)
									{
										return false;
									}
								}
								else if (newWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
								{
									if (num15 > 12f)
									{
										return false;
									}
									if (!this.TestAdjacentForNoWall(newWall))
									{
										return false;
									}
								}
								else if (newWall.door != null && !newWall.door.preset.isTransparent)
								{
									if (num15 > CullingControls.Instance.doorCullingRange)
									{
										return false;
									}
									list.Add(newWall.door.wall.id);
								}
							}
						}
						else if (debugMode)
						{
							Game.Log("...Wall not found", 2);
						}
					}
					if (vector.y != 0f)
					{
						newWall = null;
						if (newNode2.wallDict.TryGetValue(new Vector2(0f, vector.y), ref newWall))
						{
							if (debugMode)
							{
								Game.Log("...Wall found: " + newWall.preset.sectionClass.ToString(), 2);
							}
							if (!newWall.preset.ignoreCullingRaycasts)
							{
								if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.window || newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge || (newWall.door != null && newWall.door.preset.isTransparent))
								{
									if (newNode2.room.IsOutside() && !newNode.room.IsOutside() && newNode.nodeCoord.z > 0)
									{
										return false;
									}
									if (num15 > CullingControls.Instance.windowCullingRange)
									{
										return false;
									}
								}
								else if (newWall.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
								{
									if (num15 >= 12f)
									{
										return false;
									}
									if (!this.TestAdjacentForNoWall(newWall))
									{
										return false;
									}
								}
								else if (newWall.door != null && !newWall.door.preset.isTransparent)
								{
									if (num15 > CullingControls.Instance.doorCullingRange)
									{
										return false;
									}
									list.Add(newWall.door.wall.id);
								}
							}
						}
						else if (debugMode)
						{
							Game.Log("...Wall not found", 2);
						}
					}
				}
			}
			path.Add(new DataRaycastController.NodeRaycastHit
			{
				coord = vector3Int2,
				conditionalDoors = list
			});
			if (num14-- == 0)
			{
				return true;
			}
			num4 -= num7;
			if (num4 < 0)
			{
				num4 += num13;
				num += num8;
			}
			num5 -= num9;
			if (num5 < 0)
			{
				num5 += num13;
				num2 += num10;
			}
			num6 -= num11;
			if (num6 < 0)
			{
				num6 += num13;
				num3 += num12;
			}
			vector3Int = vector3Int2;
		}
		return false;
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0009810C File Offset: 0x0009630C
	private bool TestAdjacentForNoCeilingAdjBannister(NewNode n)
	{
		NewNode newNode = null;
		foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
		{
			Vector3Int vector3Int = n.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && newNode.room != n.room)
			{
				Vector2 vector;
				vector..ctor((float)(newNode.nodeCoord.x - n.nodeCoord.x) * -0.5f, (float)(newNode.nodeCoord.y - n.nodeCoord.y) * -0.5f);
				NewWall newWall;
				if (newNode.wallDict.TryGetValue(vector, ref newWall) && newWall.preset.ignoreCullingRaycasts && (newNode.floorType == NewNode.FloorTileType.floorOnly || newNode.floorType == NewNode.FloorTileType.none || newNode.floorType == NewNode.FloorTileType.noneButIndoors))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x00098218 File Offset: 0x00096418
	private bool TestAdjacentForNoFloorAdjBannister(NewNode n)
	{
		NewNode newNode = null;
		NewWall newWall = null;
		foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
		{
			Vector3Int vector3Int = n.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && newNode.room != n.room)
			{
				Vector2 vector;
				vector..ctor((float)(newNode.nodeCoord.x - n.nodeCoord.x) * -0.5f, (float)(newNode.nodeCoord.y - n.nodeCoord.y) * -0.5f);
				if (newNode.wallDict.TryGetValue(vector, ref newWall) && newWall.preset.ignoreCullingRaycasts && (newNode.floorType == NewNode.FloorTileType.CeilingOnly || newNode.floorType == NewNode.FloorTileType.none || newNode.floorType == NewNode.FloorTileType.noneButIndoors))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x00098328 File Offset: 0x00096528
	private bool TestAdjacentForNoWall(NewWall w)
	{
		NewNode newNode = null;
		NewWall newWall = null;
		Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
		for (int i = 0; i < offsetArrayX.Length; i++)
		{
			Vector2Int v2 = offsetArrayX[i];
			if (!w.node.walls.Exists((NewWall item) => item.wallOffset * 2f == v2))
			{
				Vector3Int vector3Int = w.node.nodeCoord + new Vector3Int(v2.x, v2.y, 0);
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && newNode.wallDict.TryGetValue(w.wallOffset, ref newWall) && newWall.door == null && newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04000A49 RID: 2633
	private static DataRaycastController _instance;

	// Token: 0x0200019B RID: 411
	[Serializable]
	public struct NodeRaycastHit
	{
		// Token: 0x04000A4A RID: 2634
		public Vector3Int coord;

		// Token: 0x04000A4B RID: 2635
		public List<int> conditionalDoors;
	}
}
