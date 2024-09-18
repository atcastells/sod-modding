using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200038B RID: 907
[Serializable]
public class NewTile
{
	// Token: 0x06001498 RID: 5272 RVA: 0x0012BF38 File Offset: 0x0012A138
	public void SetupInterior(NewFloor newFloor, Vector2Int newCoord, bool newIsEdge)
	{
		this.floor = newFloor;
		this.building = this.floor.building;
		this.parent = newFloor.gameObject.transform;
		this.floorCoord = newCoord;
		this.isEdge = newIsEdge;
		this.SetAsOutside(this.isEdge);
		this.globalTileCoord = new Vector3Int(this.building.globalTileCoords.x - Mathf.FloorToInt((float)CityControls.Instance.tileMultiplier * 0.5f) + this.floorCoord.x, this.building.globalTileCoords.y - Mathf.FloorToInt((float)CityControls.Instance.tileMultiplier * 0.5f) + this.floorCoord.y, this.floor.floor);
		this.position = CityData.Instance.TileToRealpos(this.globalTileCoord);
		this.floor.tileMap.Add(this.floorCoord, this);
		this.cityTile = this.floor.building.cityTile;
		this.CommonSetup();
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x0012C050 File Offset: 0x0012A250
	public void SetupExterior(CityTile newCityTile, Vector3Int newCityCoord)
	{
		newCityTile.AddOutsideTile(this);
		this.cityTile = newCityTile;
		this.isEdge = true;
		this.globalTileCoord = newCityCoord;
		this.position = CityData.Instance.TileToRealpos(this.globalTileCoord);
		this.parent = newCityTile.gameObject.transform;
		this.SetAsOutside(true);
		this.CheckOffMap();
		this.CommonSetup();
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x0012C0B4 File Offset: 0x0012A2B4
	private void CommonSetup()
	{
		if (this.isSetup)
		{
			return;
		}
		if (this.tileID <= -1)
		{
			this.tileID = NewTile.assignID;
			NewTile.assignID++;
			this.name = string.Concat(new string[]
			{
				"Tile ",
				Mathf.RoundToInt((float)this.globalTileCoord.x).ToString(),
				",",
				Mathf.RoundToInt((float)this.globalTileCoord.y).ToString(),
				",",
				Mathf.RoundToInt((float)this.globalTileCoord.z).ToString(),
				" edge: ",
				this.isEdge.ToString()
			});
		}
		if (!PathFinder.Instance.tileMap.ContainsKey(this.globalTileCoord))
		{
			PathFinder.Instance.tileMap.Add(this.globalTileCoord, this);
		}
		for (int i = 0; i < CityControls.Instance.nodeMultiplier; i++)
		{
			for (int j = 0; j < CityControls.Instance.nodeMultiplier; j++)
			{
				Vector2Int newLocalCoord;
				newLocalCoord..ctor(i, j);
				if (!this.isEdge || SessionData.Instance.isTestScene || this.globalTileCoord.z == 0 || ((this.floorCoord.x > 0 || newLocalCoord.x >= CityControls.Instance.nodeMultiplier - 1) && (this.floorCoord.x < CityControls.Instance.tileMultiplier - 1 || newLocalCoord.x <= 0) && (this.floorCoord.y > 0 || newLocalCoord.y >= CityControls.Instance.nodeMultiplier - 1) && (this.floorCoord.y < CityControls.Instance.tileMultiplier - 1 || newLocalCoord.y <= 0)))
				{
					NewNode newNode = new NewNode();
					if (this.floor == null || this.floor.lobbyAddress == null)
					{
						newNode.Setup(this, this.streetController, newLocalCoord);
					}
					else if (this.isEdge)
					{
						newNode.Setup(this, this.floor.outsideAddress, newLocalCoord);
					}
					else
					{
						newNode.Setup(this, this.floor.lobbyAddress, newLocalCoord);
					}
					if (i == Mathf.FloorToInt((float)CityControls.Instance.nodeMultiplier * 0.5f) && j == Mathf.FloorToInt((float)CityControls.Instance.nodeMultiplier * 0.5f))
					{
						this.anchorNode = newNode;
					}
				}
			}
		}
		this.isSetup = true;
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x0012C364 File Offset: 0x0012A564
	public void LoadPathfindTileData(CitySaveData.TileCitySave data)
	{
		this.SetAsOutside(data.isOutside);
		this.globalTileCoord = data.globalTileCoord;
		this.tileID = data.tileID;
		this.isOutside = data.isOutside;
		this.isObstacle = data.isObstacle;
		this.isEdge = data.isEdge;
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x0012C3BC File Offset: 0x0012A5BC
	public void LoadExterior(CitySaveData.TileCitySave data)
	{
		this.tileID = data.tileID;
		this.floorCoord = data.floorCoord;
		this.globalTileCoord = data.globalTileCoord;
		this.SetAsOutside(data.isOutside);
		this.isObstacle = data.isObstacle;
		this.isEdge = data.isEdge;
		this.rotation = data.rotation;
		this.isEntrance = data.isEntrance;
		this.isMainEntrance = data.isMainEntrance;
		this.isStairwell = data.isStairwell;
		this.stairwellRotation = data.stairwellRotation;
		this.isInvertedStairwell = data.isElevator;
		this.elevatorRotation = data.elevatorRotation;
		this.isTop = data.isTop;
		this.isBottom = data.isBottom;
		this.cityTile.AddOutsideTile(this);
		this.isEdge = true;
		this.globalTileCoord = data.globalTileCoord;
		this.position = CityData.Instance.TileToRealpos(this.globalTileCoord);
		this.parent = this.cityTile.gameObject.transform;
		this.SetAsOutside(true);
		this.CheckOffMap();
		this.CommonSetup();
		this.isLoaded = true;
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x0012C4E4 File Offset: 0x0012A6E4
	public void LoadInterior(CitySaveData.TileCitySave data)
	{
		this.tileID = data.tileID;
		this.floorCoord = data.floorCoord;
		this.globalTileCoord = data.globalTileCoord;
		this.SetAsOutside(data.isOutside);
		this.isObstacle = data.isObstacle;
		this.isMainEntrance = data.isMainEntrance;
		this.isEdge = data.isEdge;
		this.rotation = data.rotation;
		this.isTop = data.isTop;
		this.isBottom = data.isBottom;
		this.position = CityData.Instance.TileToRealpos(this.globalTileCoord);
		this.parent = this.cityTile.gameObject.transform;
		this.SetAsEntrance(data.isEntrance, data.isMainEntrance, false);
		this.SetAsStairwell(data.isStairwell, false, data.isElevator);
		this.SetStairwellRotation(data.stairwellRotation);
		this.CommonSetup();
		this.isLoaded = true;
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x0012C5D4 File Offset: 0x0012A7D4
	public void AddNewNode(NewNode newNode)
	{
		if (!this.nodes.Contains(newNode))
		{
			if (newNode.tile != null)
			{
				newNode.tile.RemoveNode(newNode);
			}
			this.nodes.Add(newNode);
			newNode.tile = this;
			newNode.floor = this.floor;
			newNode.building = this.building;
			newNode.SetAsObstacle(this.isObstacle);
			newNode.SetAsOutside(this.isOutside);
		}
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x0012C646 File Offset: 0x0012A846
	public void RemoveNode(NewNode newNode)
	{
		if (this.nodes.Contains(newNode))
		{
			this.nodes.Remove(newNode);
			newNode.tile = null;
			newNode.room = null;
			newNode.gameLocation = null;
			newNode.floor = null;
			newNode.building = null;
		}
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x0012C688 File Offset: 0x0012A888
	public void SetRotation(int newRot)
	{
		this.rotation = newRot;
		foreach (NewNode newNode in this.nodes)
		{
			if (newNode.spawnedFloor != null)
			{
				newNode.spawnedFloor.transform.localEulerAngles = new Vector3(0f, (float)this.rotation, 0f);
			}
		}
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x0012C710 File Offset: 0x0012A910
	public void SetAsEntrance(bool val, bool mainEntrance, bool set = false)
	{
		this.isEntrance = val;
		if (val)
		{
			this.isMainEntrance = mainEntrance;
			if (SessionData.Instance.isFloorEdit && this.isEdge)
			{
				Toolbox.Instance.DestroyObject(this.entranceArrow);
				this.entranceArrow = Toolbox.Instance.SpawnObject(PrefabControls.Instance.entranceArrow, this.parent);
				this.entranceArrow.transform.position = this.position;
				if (this.isMainEntrance)
				{
					this.entranceArrow.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
				}
				else
				{
					this.entranceArrow.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.blue);
				}
			}
		}
		else
		{
			this.isMainEntrance = false;
			if (this.entranceArrow != null)
			{
				Toolbox.Instance.DestroyObject(this.entranceArrow);
			}
		}
		if (SessionData.Instance.isFloorEdit && set && (this.isEntrance || (!this.isEntrance && this.entrancePair != null)))
		{
			Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
			int i = 0;
			while (i < offsetArrayX.Length)
			{
				Vector2Int vector2Int = offsetArrayX[i] + this.floorCoord;
				if (this.floor != null && this.floor.tileMap.ContainsKey(vector2Int) && !this.floor.tileMap[vector2Int].isEdge)
				{
					if (this.isEntrance)
					{
						this.entrancePair = this.floor.tileMap[vector2Int];
						this.entrancePair.entrancePair = this;
						this.entrancePair.isEntrance = this.isEntrance;
						this.entrancePair.isMainEntrance = this.isMainEntrance;
						if (this.entranceArrow != null)
						{
							this.entranceArrow.transform.LookAt(this.entrancePair.position);
						}
					}
					else
					{
						if (this.entrancePair != null)
						{
							this.entrancePair.isEntrance = false;
							this.entrancePair.isMainEntrance = false;
							this.entrancePair.entrancePair = null;
							if (this.entranceArrow != null)
							{
								Toolbox.Instance.DestroyObject(this.entranceArrow);
							}
						}
						this.entrancePair = null;
					}
					if (this.entrancePair != null)
					{
						string[] array = new string[6];
						array[0] = "Set found paring tile of ";
						int num = 1;
						Vector3Int vector3Int = this.entrancePair.globalTileCoord;
						array[num] = vector3Int.ToString();
						array[2] = " to entrance: ";
						array[3] = this.isEntrance.ToString();
						array[4] = " ";
						array[5] = this.isMainEntrance.ToString();
						Game.Log(string.Concat(array), 2);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x0012C9D8 File Offset: 0x0012ABD8
	public void SetAsStairwell(bool val, bool spawnPrefabs, bool isInverted)
	{
		this.isStairwell = val;
		if (this.isStairwell)
		{
			this.isInvertedStairwell = isInverted;
			StairwellPreset stairwellPreset = InteriorControls.Instance.defaultStairwell;
			if (this.isInvertedStairwell)
			{
				stairwellPreset = InteriorControls.Instance.defaultStairwellInverted;
				if (this.building != null && this.building.preset != null && this.building.preset.stairwellLarge != null)
				{
					stairwellPreset = this.building.preset.stairwellLarge;
				}
			}
			else if (this.building != null && this.building.preset != null && this.building.preset.stairwellRegular != null)
			{
				stairwellPreset = this.building.preset.stairwellRegular;
			}
			foreach (NewNode newNode in this.nodes)
			{
				if (!newNode.room.isNullRoom && !newNode.room.isOutsideWindow)
				{
					newNode.room.featuresStairwell = true;
				}
				if (!this.isTop && !this.isBottom)
				{
					newNode.SetFloorType(NewNode.FloorTileType.noneButIndoors);
				}
				else if (this.isTop)
				{
					newNode.SetFloorType(NewNode.FloorTileType.CeilingOnly);
				}
				else if (this.isBottom)
				{
					newNode.SetFloorType(NewNode.FloorTileType.floorOnly);
				}
			}
			this.SetStairwellRotation(this.stairwellRotation);
			this.stairwellAssign = this.building.AddStairwellSystem(this, stairwellPreset);
			if (spawnPrefabs && this.stairwell == null)
			{
				if (!this.isTop)
				{
					this.stairwell = Toolbox.Instance.SpawnObject(stairwellPreset.spawnObject, this.parent);
				}
				else
				{
					this.stairwell = Toolbox.Instance.SpawnObject(stairwellPreset.objectTop, this.parent);
				}
				this.stairwell.transform.position = this.position;
				Toolbox.Instance.SetLightLayer(this.stairwell, this.building, false);
				if (this.nodes.Count > 0 && this.nodes[0].room != null)
				{
					this.stairwell.transform.SetParent(this.nodes[0].room.transform, true);
				}
				this.stairwell.transform.localEulerAngles = new Vector3(0f, (float)this.stairwellRotation, 0f);
				this.stairwellAssign.OnSpawnStairwell(this);
				return;
			}
		}
		else
		{
			foreach (NewNode newNode2 in this.nodes)
			{
				newNode2.room.featuresStairwell = false;
			}
			if (this.stairwell != null)
			{
				Toolbox.Instance.DestroyObject(this.stairwell);
			}
		}
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x0012CCDC File Offset: 0x0012AEDC
	public void SetStairwellRotation(int newRot)
	{
		while (newRot >= 360)
		{
			newRot -= 360;
		}
		while (newRot <= -360)
		{
			newRot += 360;
		}
		this.stairwellRotation = newRot;
		if (this.stairwell != null)
		{
			this.stairwell.transform.localEulerAngles = new Vector3(0f, (float)this.stairwellRotation, 0f);
		}
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x0012CD4C File Offset: 0x0012AF4C
	public void SetAsTop(bool newIsTop)
	{
		this.isTop = newIsTop;
		foreach (NewNode newNode in this.nodes)
		{
			newNode.SetFloorType(NewNode.FloorTileType.CeilingOnly);
		}
		if (this.elevator != null)
		{
			Toolbox.Instance.DestroyObject(this.elevator);
		}
		if (this.stairwell != null)
		{
			Toolbox.Instance.DestroyObject(this.stairwell);
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0012CDE0 File Offset: 0x0012AFE0
	public void SetAsBottom(bool newIsBottom)
	{
		this.isBottom = newIsBottom;
		foreach (NewNode newNode in this.nodes)
		{
			newNode.SetFloorType(NewNode.FloorTileType.floorOnly);
		}
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x0012CE38 File Offset: 0x0012B038
	public bool CanBeOptimized()
	{
		bool result = true;
		NewRoom newRoom = null;
		NewNode.FloorTileType floorTileType = NewNode.FloorTileType.floorAndCeiling;
		foreach (NewNode newNode in this.nodes)
		{
			if (newRoom == null)
			{
				newRoom = newNode.room;
				floorTileType = newNode.floorType;
			}
			else
			{
				if (newRoom != newNode.room)
				{
					result = false;
					break;
				}
				if (floorTileType != newNode.floorType)
				{
					result = false;
					break;
				}
				if (newNode.ceilingAirVent || newNode.floorAirVent)
				{
					result = false;
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x0012CEE0 File Offset: 0x0012B0E0
	public void SetFloorCeilingOptimization(bool val, bool spawnPrefabs)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (NewNode newNode in this.nodes)
		{
			if (newNode.ceilingAirVent)
			{
				flag = true;
			}
			if (newNode.floorAirVent)
			{
				flag2 = true;
			}
		}
		this.useOptimizedFloor = val;
		this.useOptimizedCeiling = val;
		if (flag)
		{
			this.useOptimizedCeiling = false;
		}
		if (flag2)
		{
			this.useOptimizedFloor = false;
		}
		if (spawnPrefabs)
		{
			foreach (NewNode newNode2 in this.nodes)
			{
				newNode2.SpawnCeiling(false);
				newNode2.SpawnFloor(false);
			}
		}
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x0012CFB0 File Offset: 0x0012B1B0
	public void SetAsObstacle(bool val)
	{
		this.isObstacle = val;
		foreach (NewNode newNode in this.nodes)
		{
			newNode.SetAsObstacle(val);
		}
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x0012D008 File Offset: 0x0012B208
	public void SetAsOutside(bool val)
	{
		this.isOutside = val;
		foreach (NewNode newNode in this.nodes)
		{
			newNode.SetAsOutside(val);
		}
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x0012D060 File Offset: 0x0012B260
	public void CheckOffMap()
	{
		bool flag = true;
		if (this.globalTileCoord.x < 0)
		{
			flag = false;
		}
		else if ((float)this.globalTileCoord.x > PathFinder.Instance.tileCitySize.x)
		{
			flag = false;
		}
		if (this.globalTileCoord.y < 0)
		{
			flag = false;
		}
		else if ((float)this.globalTileCoord.y > PathFinder.Instance.tileCitySize.y)
		{
			flag = false;
		}
		if (!flag)
		{
			this.SetAsObstacle(true);
		}
		this.isMapCorner = false;
		if ((this.globalTileCoord.x == 0 || (float)this.globalTileCoord.x == PathFinder.Instance.tileCitySize.x - 1f) && (this.globalTileCoord.y == 0 || (float)this.globalTileCoord.y == PathFinder.Instance.tileCitySize.y - 1f))
		{
			this.isMapCorner = true;
		}
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x0012D14C File Offset: 0x0012B34C
	public void ConnectStairwell()
	{
		Dictionary<Vector2, NewNode> dictionary = new Dictionary<Vector2, NewNode>();
		foreach (NewNode newNode in this.nodes)
		{
			Vector2 vector;
			vector..ctor((float)newNode.localTileCoord.x - (float)(CityControls.Instance.nodeMultiplier - 1) * 0.5f, (float)newNode.localTileCoord.y - (float)(CityControls.Instance.nodeMultiplier - 1) * 0.5f);
			vector = Toolbox.Instance.RotateVector2ACW(vector, (float)this.stairwellRotation);
			Vector2 vector2;
			vector2..ctor((float)Mathf.RoundToInt(vector.x + (float)(CityControls.Instance.nodeMultiplier - 1) * 0.5f), (float)Mathf.RoundToInt(vector.y + (float)(CityControls.Instance.nodeMultiplier - 1) * 0.5f));
			try
			{
				dictionary.Add(vector2, newNode);
			}
			catch
			{
				if (Game.Instance.devMode)
				{
					Game.Log(this.name, 2);
					foreach (NewNode newNode2 in this.nodes)
					{
						Game.Log(newNode2.localTileCoord, 2);
					}
					string[] array = new string[6];
					array[0] = "Existing rotation error: ";
					int num = 1;
					Vector2 vector3 = vector;
					array[num] = vector3.ToString();
					array[2] = " rotated is ";
					int num2 = 3;
					vector3 = vector2;
					array[num2] = vector3.ToString();
					array[4] = " ";
					array[5] = newNode.name;
					Game.LogError(string.Concat(array), 2);
				}
				return;
			}
		}
		foreach (KeyValuePair<Vector2, NewNode> keyValuePair in dictionary)
		{
			if (keyValuePair.Key.x == 0f || keyValuePair.Key.x == (float)(CityControls.Instance.nodeMultiplier - 1))
			{
				NewNode newNode3 = null;
				if (!this.isTop && dictionary.TryGetValue(new Vector2(keyValuePair.Key.x + 0f, keyValuePair.Key.y + 1f), ref newNode3) && (!keyValuePair.Value.accessToOtherNodes.ContainsKey(newNode3) || !keyValuePair.Value.accessToOtherNodes[newNode3].walkingAccess))
				{
					keyValuePair.Value.AddAccessToOtherNode(newNode3, true, false, NewNode.NodeAccess.AccessType.adjacent, true);
				}
				if (!this.isTop && dictionary.TryGetValue(new Vector2(keyValuePair.Key.x + 0f, keyValuePair.Key.y - 1f), ref newNode3) && (!keyValuePair.Value.accessToOtherNodes.ContainsKey(newNode3) || !keyValuePair.Value.accessToOtherNodes[newNode3].walkingAccess))
				{
					keyValuePair.Value.AddAccessToOtherNode(newNode3, true, false, NewNode.NodeAccess.AccessType.adjacent, true);
				}
			}
			if (keyValuePair.Key.y == 0f)
			{
				NewNode newNode4 = null;
				if (!this.isTop && dictionary.TryGetValue(new Vector2(keyValuePair.Key.x + 1f, keyValuePair.Key.y), ref newNode4) && (!keyValuePair.Value.accessToOtherNodes.ContainsKey(newNode4) || !keyValuePair.Value.accessToOtherNodes[newNode4].walkingAccess))
				{
					keyValuePair.Value.AddAccessToOtherNode(newNode4, true, false, NewNode.NodeAccess.AccessType.adjacent, true);
				}
				if (!this.isTop && dictionary.TryGetValue(new Vector2(keyValuePair.Key.x - 1f, keyValuePair.Key.y), ref newNode4) && (!keyValuePair.Value.accessToOtherNodes.ContainsKey(newNode4) || !keyValuePair.Value.accessToOtherNodes[newNode4].walkingAccess))
				{
					keyValuePair.Value.AddAccessToOtherNode(newNode4, true, false, NewNode.NodeAccess.AccessType.adjacent, true);
				}
			}
			if (keyValuePair.Key.x == 1f && keyValuePair.Key.y < 2f)
			{
				NewNode newNode5 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(keyValuePair.Value.nodeCoord + new Vector3Int(0, 0, 1), ref newNode5) && !keyValuePair.Value.accessToOtherNodes.ContainsKey(newNode5))
				{
					keyValuePair.Value.AddAccessToOtherNode(newNode5, true, false, NewNode.NodeAccess.AccessType.adjacent, false);
				}
				if (PathFinder.Instance.nodeMap.TryGetValue(keyValuePair.Value.nodeCoord + new Vector3Int(0, 0, -1), ref newNode5) && !keyValuePair.Value.accessToOtherNodes.ContainsKey(newNode5))
				{
					keyValuePair.Value.AddAccessToOtherNode(newNode5, true, false, NewNode.NodeAccess.AccessType.adjacent, false);
				}
			}
			if (this.isInvertedStairwell)
			{
				if (keyValuePair.Key.x == 0f && keyValuePair.Key.y == 2f)
				{
					keyValuePair.Value.stairwellLowerLink = true;
					if (!this.isTop)
					{
						keyValuePair.Value.SetFloorHeight(7);
					}
				}
				else if (keyValuePair.Key.x == 0f && keyValuePair.Key.y == 1f)
				{
					if (!this.isTop)
					{
						keyValuePair.Value.SetFloorHeight(20);
					}
				}
				else if (keyValuePair.Key.x == 0f && keyValuePair.Key.y == 0f)
				{
					if (!this.isTop)
					{
						keyValuePair.Value.SetFloorHeight(24);
					}
				}
				else if (keyValuePair.Key.x == 1f && keyValuePair.Key.y == 0f)
				{
					if (!this.isTop)
					{
						keyValuePair.Value.SetFloorHeight(28);
					}
				}
				else if (keyValuePair.Key.x == 2f && keyValuePair.Key.y == 0f)
				{
					if (!this.isTop)
					{
						keyValuePair.Value.SetFloorHeight(33);
					}
				}
				else if (keyValuePair.Key.x == 2f && keyValuePair.Key.y == 1f)
				{
					if (!this.isTop)
					{
						keyValuePair.Value.SetFloorHeight(39);
					}
				}
				else if (keyValuePair.Key.x == 2f && keyValuePair.Key.y == 2f)
				{
					keyValuePair.Value.stairwellUpperLink = true;
					if (!this.isTop)
					{
						keyValuePair.Value.SetFloorHeight(51);
					}
				}
			}
			else if (keyValuePair.Key.x == 2f && keyValuePair.Key.y == 2f)
			{
				keyValuePair.Value.stairwellLowerLink = true;
				if (!this.isTop)
				{
					keyValuePair.Value.SetFloorHeight(7);
				}
			}
			else if (keyValuePair.Key.x == 2f && keyValuePair.Key.y == 1f)
			{
				if (!this.isTop)
				{
					keyValuePair.Value.SetFloorHeight(20);
				}
			}
			else if (keyValuePair.Key.x == 2f && keyValuePair.Key.y == 0f)
			{
				if (!this.isTop)
				{
					keyValuePair.Value.SetFloorHeight(24);
				}
			}
			else if (keyValuePair.Key.x == 1f && keyValuePair.Key.y == 0f)
			{
				if (!this.isTop)
				{
					keyValuePair.Value.SetFloorHeight(28);
				}
			}
			else if (keyValuePair.Key.x == 0f && keyValuePair.Key.y == 0f)
			{
				if (!this.isTop)
				{
					keyValuePair.Value.SetFloorHeight(33);
				}
			}
			else if (keyValuePair.Key.x == 0f && keyValuePair.Key.y == 1f)
			{
				if (!this.isTop)
				{
					keyValuePair.Value.SetFloorHeight(39);
				}
			}
			else if (keyValuePair.Key.x == 0f && keyValuePair.Key.y == 2f)
			{
				keyValuePair.Value.stairwellUpperLink = true;
				if (!this.isTop)
				{
					keyValuePair.Value.SetFloorHeight(51);
				}
			}
			keyValuePair.Value.isConnected = true;
		}
	}

	// Token: 0x060014AC RID: 5292 RVA: 0x0012DA9C File Offset: 0x0012BC9C
	public CitySaveData.TileCitySave GenerateSaveData()
	{
		return new CitySaveData.TileCitySave
		{
			tileID = this.tileID,
			floorCoord = this.floorCoord,
			globalTileCoord = this.globalTileCoord,
			isOutside = this.isOutside,
			isObstacle = this.isObstacle,
			isEdge = this.isEdge,
			rotation = this.rotation,
			isEntrance = this.isEntrance,
			isMainEntrance = this.isMainEntrance,
			isStairwell = this.isStairwell,
			stairwellRotation = this.stairwellRotation,
			isElevator = this.isInvertedStairwell,
			elevatorRotation = this.elevatorRotation,
			isTop = this.isTop,
			isBottom = this.isBottom
		};
	}

	// Token: 0x0400193D RID: 6461
	[Header("ID")]
	public int tileID = -1;

	// Token: 0x0400193E RID: 6462
	public static int assignID;

	// Token: 0x0400193F RID: 6463
	[Header("Transform")]
	public string name;

	// Token: 0x04001940 RID: 6464
	public Vector3 position;

	// Token: 0x04001941 RID: 6465
	public Transform parent;

	// Token: 0x04001942 RID: 6466
	[Header("Location")]
	public NewBuilding building;

	// Token: 0x04001943 RID: 6467
	public NewFloor floor;

	// Token: 0x04001944 RID: 6468
	public CityTile cityTile;

	// Token: 0x04001945 RID: 6469
	public Vector2Int floorCoord;

	// Token: 0x04001946 RID: 6470
	public Vector3Int globalTileCoord;

	// Token: 0x04001947 RID: 6471
	public PathFinder.StreetChunk streetChunk;

	// Token: 0x04001948 RID: 6472
	[Header("Tile Contents")]
	public List<NewNode> nodes = new List<NewNode>();

	// Token: 0x04001949 RID: 6473
	public NewNode anchorNode;

	// Token: 0x0400194A RID: 6474
	[Header("Tile")]
	public bool isSetup;

	// Token: 0x0400194B RID: 6475
	public bool isLoaded;

	// Token: 0x0400194C RID: 6476
	public bool isOutside;

	// Token: 0x0400194D RID: 6477
	public bool isObstacle;

	// Token: 0x0400194E RID: 6478
	public bool isMapCorner;

	// Token: 0x0400194F RID: 6479
	public bool isEdge;

	// Token: 0x04001950 RID: 6480
	public int rotation;

	// Token: 0x04001951 RID: 6481
	public bool isEntrance;

	// Token: 0x04001952 RID: 6482
	public bool isMainEntrance;

	// Token: 0x04001953 RID: 6483
	public NewTile entrancePair;

	// Token: 0x04001954 RID: 6484
	public bool isStairwell;

	// Token: 0x04001955 RID: 6485
	public int stairwellRotation;

	// Token: 0x04001956 RID: 6486
	public bool isInvertedStairwell;

	// Token: 0x04001957 RID: 6487
	public int elevatorRotation;

	// Token: 0x04001958 RID: 6488
	public bool isTop;

	// Token: 0x04001959 RID: 6489
	public bool isBottom;

	// Token: 0x0400195A RID: 6490
	[Header("Roads")]
	public StreetController streetController;

	// Token: 0x0400195B RID: 6491
	[Header("Optimization")]
	public bool useOptimizedFloor;

	// Token: 0x0400195C RID: 6492
	public bool useOptimizedCeiling;

	// Token: 0x0400195D RID: 6493
	[Header("Spawned Objects")]
	public GameObject entranceArrow;

	// Token: 0x0400195E RID: 6494
	public GameObject stairwell;

	// Token: 0x0400195F RID: 6495
	public GameObject elevator;

	// Token: 0x04001960 RID: 6496
	public Elevator stairwellAssign;
}
