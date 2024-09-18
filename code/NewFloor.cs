using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000364 RID: 868
public class NewFloor : Controller
{
	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06001387 RID: 4999 RVA: 0x00117E50 File Offset: 0x00116050
	// (remove) Token: 0x06001388 RID: 5000 RVA: 0x00117E88 File Offset: 0x00116088
	public event NewFloor.SaveDataComplete OnSaveDataComplete;

	// Token: 0x06001389 RID: 5001 RVA: 0x00117EC0 File Offset: 0x001160C0
	public void Setup(int newFloor, NewBuilding newBuilding, string newName, Vector2 newSize, int newFloorHeight, int newCeilingHeight)
	{
		this.floorID = NewFloor.assignID;
		NewFloor.assignID++;
		this.assignResidence = 1;
		this.floor = newFloor;
		newBuilding.AddNewFloor(this);
		base.transform.SetParent(this.building.gameObject.transform);
		base.transform.localPosition = new Vector3(0f, (float)this.floor * PathFinder.Instance.tileSize.z, 0f);
		if (!SessionData.Instance.isFloorEdit && Game.Instance.allowEchelons && newBuilding.preset.buildingFeaturesEchelonFloors && this.floor >= newBuilding.preset.echelonFloorStart)
		{
			this.isEchelons = true;
		}
		else
		{
			this.isEchelons = false;
		}
		this.size = newSize;
		this.defaultFloorHeight = newFloorHeight;
		this.defaultCeilingHeight = newCeilingHeight;
		CityData.Instance.floorRange = new Vector2(Mathf.Min(CityData.Instance.floorRange.x, (float)this.floor), Mathf.Max(CityData.Instance.floorRange.y, (float)this.floor));
		this.floorName = newName;
		if (SessionData.Instance.isFloorEdit)
		{
			base.transform.name = newName;
		}
		else
		{
			base.transform.name = this.floorName + " (Floor " + this.floor.ToString() + ")";
		}
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.address, base.transform);
		this.outsideAddress = gameObject.GetComponent<NewAddress>();
		this.outsideAddress.Setup(this, CityControls.Instance.outsideLayoutConfig, CityControls.Instance.defaultStyle);
		this.outsideAddress.isOutsideAddress = true;
		GameObject gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.address, base.transform);
		this.lobbyAddress = gameObject2.GetComponent<NewAddress>();
		this.lobbyAddress.Setup(this, CityControls.Instance.lobbyLayoutConfig, CityControls.Instance.defaultStyle);
		this.lobbyAddress.isLobbyAddress = true;
		if (SessionData.Instance.isFloorEdit)
		{
			FloorEditController.Instance.addressSelection = this.lobbyAddress;
		}
		int num = 0;
		while ((float)num < this.size.x)
		{
			int num2 = 0;
			while ((float)num2 < this.size.y)
			{
				for (int i = 0; i < CityControls.Instance.tileMultiplier; i++)
				{
					int j = 0;
					while (j < CityControls.Instance.tileMultiplier)
					{
						Vector2Int newCoord;
						newCoord..ctor(num * CityControls.Instance.tileMultiplier + i, num2 * CityControls.Instance.tileMultiplier + j);
						bool newIsEdge = false;
						if (newCoord.x <= 0 || newCoord.y <= 0)
						{
							newIsEdge = true;
						}
						else if ((float)newCoord.x >= this.size.x * (float)CityControls.Instance.tileMultiplier - 1f || (float)newCoord.y >= this.size.y * (float)CityControls.Instance.tileMultiplier - 1f)
						{
							newIsEdge = true;
						}
						NewTile newTile = null;
						Vector3Int vector3Int;
						vector3Int..ctor(this.building.globalTileCoords.x - Mathf.FloorToInt((float)CityControls.Instance.tileMultiplier * 0.5f) + newCoord.x, this.building.globalTileCoords.y - Mathf.FloorToInt((float)CityControls.Instance.tileMultiplier * 0.5f) + newCoord.y, this.floor);
						if (PathFinder.Instance.tileMap.TryGetValue(vector3Int, ref newTile))
						{
							using (List<NewNode>.Enumerator enumerator = newTile.nodes.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									NewNode newNode = enumerator.Current;
									Vector2Int vector2Int;
									vector2Int..ctor(Mathf.RoundToInt((float)(newCoord.x * CityControls.Instance.nodeMultiplier)) + newNode.localTileCoord.x, Mathf.RoundToInt((float)(newCoord.y * CityControls.Instance.nodeMultiplier)) + newNode.localTileCoord.y);
									this.nodeMap.Add(vector2Int, newNode);
								}
								goto IL_3FE;
							}
							goto IL_3EF;
						}
						goto IL_3EF;
						IL_3FE:
						j++;
						continue;
						IL_3EF:
						new NewTile().SetupInterior(this, newCoord, newIsEdge);
						goto IL_3FE;
					}
				}
				num2++;
			}
			num++;
		}
		if (!SessionData.Instance.isFloorEdit && !CityData.Instance.floorDirectory.Contains(this))
		{
			CityData.Instance.floorDirectory.Add(this);
		}
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x00118364 File Offset: 0x00116564
	public void Load(CitySaveData.FloorCitySave data, NewBuilding newBuilding)
	{
		this.floorID = data.floorID;
		this.floor = data.floor;
		this.building = newBuilding;
		newBuilding.AddNewFloor(this);
		base.transform.SetParent(this.building.gameObject.transform);
		base.transform.localPosition = new Vector3(0f, (float)this.floor * PathFinder.Instance.tileSize.z, 0f);
		this.size = data.size;
		this.defaultFloorHeight = data.defaultFloorHeight;
		this.defaultCeilingHeight = data.defaultCeilingHeight;
		this.isEchelons = data.echelons;
		CityData.Instance.floorRange = new Vector2(Mathf.Min(CityData.Instance.floorRange.x, (float)this.floor), Mathf.Max(CityData.Instance.floorRange.y, (float)this.floor));
		this.floorName = data.name;
		this.layoutIndex = data.layoutIndex;
		this.breakerLightsID = data.breakerLights;
		this.breakerSecurityID = data.breakerSec;
		this.breakerDoorsID = data.breakerDoors;
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.address, base.transform);
		this.outsideAddress = gameObject.GetComponent<NewAddress>();
		this.outsideAddress.Setup(this, CityControls.Instance.outsideLayoutConfig, CityControls.Instance.defaultStyle);
		this.outsideAddress.isOutsideAddress = true;
		GameObject gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.address, base.transform);
		this.lobbyAddress = gameObject2.GetComponent<NewAddress>();
		this.lobbyAddress.Setup(this, CityControls.Instance.lobbyLayoutConfig, CityControls.Instance.defaultStyle);
		this.lobbyAddress.isLobbyAddress = true;
		int num = 0;
		while ((float)num < this.size.x)
		{
			int num2 = 0;
			while ((float)num2 < this.size.y)
			{
				for (int i = 0; i < CityControls.Instance.tileMultiplier; i++)
				{
					int j = 0;
					while (j < CityControls.Instance.tileMultiplier)
					{
						Vector2Int newCoord;
						newCoord..ctor(num * CityControls.Instance.tileMultiplier + i, num2 * CityControls.Instance.tileMultiplier + j);
						bool newIsEdge = false;
						if (newCoord.x <= 0 || newCoord.y <= 0)
						{
							newIsEdge = true;
						}
						else if ((float)newCoord.x >= this.size.x * (float)CityControls.Instance.tileMultiplier - 1f || (float)newCoord.y >= this.size.y * (float)CityControls.Instance.tileMultiplier - 1f)
						{
							newIsEdge = true;
						}
						NewTile newTile = null;
						Vector3Int vector3Int;
						vector3Int..ctor(this.building.globalTileCoords.x - Mathf.FloorToInt((float)CityControls.Instance.tileMultiplier * 0.5f) + newCoord.x, this.building.globalTileCoords.y - Mathf.FloorToInt((float)CityControls.Instance.tileMultiplier * 0.5f) + newCoord.y, this.floor);
						if (PathFinder.Instance.tileMap.TryGetValue(vector3Int, ref newTile))
						{
							using (List<NewNode>.Enumerator enumerator = newTile.nodes.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									NewNode newNode = enumerator.Current;
									Vector2Int vector2Int;
									vector2Int..ctor(newCoord.x * CityControls.Instance.nodeMultiplier + newNode.localTileCoord.x, newCoord.y * CityControls.Instance.nodeMultiplier + newNode.localTileCoord.y);
									this.nodeMap.Add(vector2Int, newNode);
								}
								goto IL_390;
							}
							goto IL_381;
						}
						goto IL_381;
						IL_390:
						j++;
						continue;
						IL_381:
						new NewTile().SetupInterior(this, newCoord, newIsEdge);
						goto IL_390;
					}
				}
				num2++;
			}
			num++;
		}
		for (int k = 0; k < data.tiles.Count; k++)
		{
			CitySaveData.TileCitySave tileCitySave = data.tiles[k];
			NewTile newTile2 = null;
			if (PathFinder.Instance.tileMap.TryGetValue(tileCitySave.globalTileCoord, ref newTile2))
			{
				newTile2.LoadInterior(tileCitySave);
			}
		}
		for (int l = 0; l < data.addresses.Count; l++)
		{
			CitySaveData.AddressCitySave addressCitySave = data.addresses[l];
			NewAddress newAddress = null;
			if (!addressCitySave.isLobbyAddress && !addressCitySave.isOutsideAddress)
			{
				newAddress = Object.Instantiate<GameObject>(PrefabControls.Instance.address, base.transform).GetComponent<NewAddress>();
			}
			else if (addressCitySave.isOutsideAddress)
			{
				newAddress = this.outsideAddress;
			}
			else if (addressCitySave.isLobbyAddress)
			{
				newAddress = this.lobbyAddress;
			}
			newAddress.Load(addressCitySave, this);
		}
		base.transform.name = this.floorName + " (Floor " + this.floor.ToString() + ")";
		if (!SessionData.Instance.isFloorEdit && !CityData.Instance.floorDirectory.Contains(this))
		{
			CityData.Instance.floorDirectory.Add(this);
		}
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x001188A8 File Offset: 0x00116AA8
	public void AddNewAddress(NewAddress newAddress)
	{
		if (!this.addresses.Contains(newAddress))
		{
			if (newAddress.floor != null)
			{
				newAddress.floor.RemoveAddress(newAddress);
			}
			this.addresses.Add(newAddress);
			newAddress.floor = this;
			newAddress.building = this.building;
			foreach (NewRoom newRoom in newAddress.rooms)
			{
				newRoom.floor = this;
				foreach (NewNode newNode in newRoom.nodes)
				{
					newNode.floor = this;
				}
			}
		}
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x00118984 File Offset: 0x00116B84
	public void RemoveAddress(NewAddress newAddress)
	{
		if (this.addresses.Contains(newAddress))
		{
			this.addresses.Remove(newAddress);
			newAddress.floor = null;
			newAddress.building = null;
		}
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x001189AF File Offset: 0x00116BAF
	public void GetSaveData()
	{
		base.StartCoroutine(this.GenerateFloorSaveData());
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x001189BE File Offset: 0x00116BBE
	private IEnumerator GenerateFloorSaveData()
	{
		Game.Log("Gathering floor save data...", 2);
		this.saveData = new FloorSaveData();
		this.saveData.floorName = this.floorName;
		this.saveData.size = this.size;
		this.saveData.defaultCeilingHeight = this.defaultCeilingHeight;
		this.saveData.defaultFloorHeight = this.defaultFloorHeight;
		foreach (NewAddress ad in this.addresses)
		{
			ad.saveData.p_n = ad.preset.name;
			ad.saveData.e_c = ad.editorColour;
			while (ad.saveData.vs.Count < 0)
			{
				Game.Log("Generating new variation for " + ad.name + " as there is currently 0...", 2);
				GenerationController.Instance.GenerateAddressLayout(ad);
				this.ConnectNodesOnFloor();
				AddressLayoutVariation addressLayoutVariation = new AddressLayoutVariation();
				foreach (NewRoom newRoom in ad.rooms)
				{
					RoomSaveData roomSaveData = new RoomSaveData();
					roomSaveData.id = newRoom.roomFloorID;
					roomSaveData.l = newRoom.roomType.name;
					foreach (NewNode newNode in newRoom.nodes)
					{
						NodeSaveData nodeSaveData = new NodeSaveData();
						nodeSaveData.f_c = newNode.floorCoord;
						nodeSaveData.f_h = newNode.floorHeight;
						nodeSaveData.f_t = newNode.floorType;
						if (newNode.forcedRoom != null)
						{
							nodeSaveData.f_r = newNode.forcedRoomRef;
						}
						foreach (NewWall newWall in newNode.walls)
						{
							WallSaveData wallSaveData = new WallSaveData();
							wallSaveData.w_o = newWall.wallOffset;
							wallSaveData.p_n = newWall.preset.id;
							nodeSaveData.w_d.Add(wallSaveData);
						}
						roomSaveData.n_d.Add(nodeSaveData);
					}
					addressLayoutVariation.r_d.Add(roomSaveData);
				}
				ad.saveData.vs.Add(addressLayoutVariation);
				GenerationController.Instance.LoadGeometryFloor(this);
				yield return null;
			}
			for (int i = 0; i < ad.saveData.vs.Count; i++)
			{
				AddressLayoutVariation addressLayoutVariation2 = ad.saveData.vs[i];
				int num = 0;
				foreach (RoomSaveData roomSaveData2 in addressLayoutVariation2.r_d)
				{
					foreach (NodeSaveData nodeSaveData2 in roomSaveData2.n_d)
					{
						using (List<WallSaveData>.Enumerator enumerator7 = nodeSaveData2.w_d.GetEnumerator())
						{
							while (enumerator7.MoveNext())
							{
								if (enumerator7.Current.p_n != "0")
								{
									num++;
								}
							}
						}
					}
				}
				Game.Log(string.Concat(new string[]
				{
					num.ToString(),
					" non default walls saved in address ",
					ad.name,
					" variation ",
					i.ToString(),
					"..."
				}), 2);
			}
			this.saveData.a_d.Add(ad.saveData);
			ad = null;
		}
		List<NewAddress>.Enumerator enumerator = default(List<NewAddress>.Enumerator);
		foreach (KeyValuePair<Vector2Int, NewTile> keyValuePair in this.tileMap)
		{
			TileSaveData tileSaveData = new TileSaveData();
			tileSaveData.f_c = keyValuePair.Value.floorCoord;
			tileSaveData.e_r = keyValuePair.Value.elevatorRotation;
			tileSaveData.e_l = keyValuePair.Value.isInvertedStairwell;
			tileSaveData.i_e = keyValuePair.Value.isEntrance;
			tileSaveData.m_e = keyValuePair.Value.isMainEntrance;
			tileSaveData.s_t = keyValuePair.Value.isStairwell;
			tileSaveData.s_r = keyValuePair.Value.stairwellRotation;
			this.saveData.t_d.Add(tileSaveData);
		}
		if (this.OnSaveDataComplete != null)
		{
			this.OnSaveDataComplete(this, this.saveData);
		}
		yield break;
		yield break;
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x001189D0 File Offset: 0x00116BD0
	public void LoadDataToFloor(FloorSaveData savedData)
	{
		for (int i = 0; i < savedData.t_d.Count; i++)
		{
			TileSaveData tileSaveData = savedData.t_d[i];
			Vector2Int vector2Int = this.building.FaceLocalTileVector(tileSaveData.f_c);
			NewTile newTile = null;
			if (this.tileMap.TryGetValue(vector2Int, ref newTile))
			{
				newTile.SetAsEntrance(tileSaveData.i_e, tileSaveData.m_e, false);
				newTile.SetAsStairwell(tileSaveData.s_t, false, tileSaveData.e_l);
				newTile.SetStairwellRotation(tileSaveData.s_r + this.building.rotations * -90);
			}
		}
		for (int j = 0; j < savedData.a_d.Count; j++)
		{
			AddressSaveData addressSaveData = savedData.a_d[j];
			NewAddress newAddress = null;
			if (j == 0)
			{
				newAddress = this.outsideAddress;
			}
			else if (j == 1)
			{
				NewAddress newAddress2 = this.lobbyAddress;
				LayoutConfiguration newRoomConfig = null;
				if (Toolbox.Instance.LoadDataFromResources<LayoutConfiguration>(addressSaveData.p_n, out newRoomConfig))
				{
					this.lobbyAddress = this.CreateNewAddress(newRoomConfig, CityControls.Instance.defaultStyle);
				}
				newAddress = this.lobbyAddress;
				if (newAddress2 != null && SessionData.Instance.isFloorEdit)
				{
					int num = FloorEditController.Instance.editFloor.addresses.IndexOf(newAddress2);
					for (int k = 0; k < newAddress2.nodes.Count; k++)
					{
						this.lobbyAddress.AddNewNode(newAddress2.nodes[k]);
						k--;
					}
					Object.Destroy(newAddress2.gameObject);
					FloorEditController.Instance.editFloor.addresses.RemoveAt(num);
				}
			}
			else
			{
				LayoutConfiguration newRoomConfig2 = null;
				if (Toolbox.Instance.LoadDataFromResources<LayoutConfiguration>(addressSaveData.p_n, out newRoomConfig2))
				{
					newAddress = this.CreateNewAddress(newRoomConfig2, CityControls.Instance.defaultStyle);
				}
			}
			if (SessionData.Instance.isFloorEdit)
			{
				newAddress.editorColour = addressSaveData.e_c;
			}
			newAddress.saveData = addressSaveData;
			if (addressSaveData.vs.Count > 0)
			{
				int num2 = 0;
				if (!SessionData.Instance.isFloorEdit)
				{
					num2 = Toolbox.Instance.GetPsuedoRandomNumber(0, addressSaveData.vs.Count, newAddress.seed, false);
				}
				this.LoadVariation(newAddress, addressSaveData.vs[num2]);
			}
		}
		this.FinalizeLoadingIn();
		this.floorName = savedData.floorName;
		base.transform.name = this.floorName;
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x00118C4C File Offset: 0x00116E4C
	public void LoadVariation(NewAddress currentAdd, AddressLayoutVariation newVar)
	{
		currentAdd.loadedVarIndex = currentAdd.saveData.vs.IndexOf(newVar);
		if (currentAdd.loadedVarIndex > -1)
		{
			using (List<RoomSaveData>.Enumerator enumerator = newVar.r_d.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RoomSaveData roomSaveData = enumerator.Current;
					NewRoom newRoom = currentAdd.nullRoom;
					if (roomSaveData.l != null && roomSaveData.l.Length > 0 && currentAdd.preset != CityControls.Instance.outsideLayoutConfig && roomSaveData.n_d.Count > 0)
					{
						RoomTypePreset roomTypePreset = null;
						Toolbox.Instance.LoadDataFromResources<RoomTypePreset>(roomSaveData.l, out roomTypePreset);
						if (roomTypePreset != null)
						{
							newRoom = Object.Instantiate<GameObject>(PrefabControls.Instance.room, currentAdd.transform).GetComponent<NewRoom>();
							newRoom.SetupLayoutOnly(currentAdd, roomTypePreset, roomSaveData.id);
						}
					}
					foreach (NodeSaveData nodeSaveData in roomSaveData.n_d)
					{
						NewNode newNode = null;
						Vector2Int vector2Int = this.building.FaceLocalNodeVector(nodeSaveData.f_c);
						if (this.nodeMap.TryGetValue(vector2Int, ref newNode) && !newNode.tile.isEdge)
						{
							bool flag = false;
							if (nodeSaveData.f_r != null && nodeSaveData.f_r.Length > 0)
							{
								flag = true;
							}
							newNode.floorHeight = nodeSaveData.f_h;
							newNode.SetFloorType(nodeSaveData.f_t);
							currentAdd.AddNewNode(newNode);
							newRoom.AddNewNode(newNode);
							if (flag)
							{
								newNode.forcedRoomRef = nodeSaveData.f_r;
								string[] array = newNode.forcedRoomRef.Split('.', 0);
								string name = array[array.Length - 1];
								RoomConfiguration forcedRoom = null;
								if (Toolbox.Instance.LoadDataFromResources<RoomConfiguration>(name, out forcedRoom))
								{
									newNode.SetForcedRoom(forcedRoom);
								}
							}
						}
					}
				}
				return;
			}
		}
		Game.Log("Unable to find this variation in floor save data: " + currentAdd.loadedVarIndex.ToString() + "/" + currentAdd.saveData.vs.Count.ToString(), 2);
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x00118EAC File Offset: 0x001170AC
	public void FinalizeLoadingIn()
	{
		GenerationController.Instance.UpdateFloorCeilingFloor(this);
		GenerationController.Instance.UpdateWallsFloor(this);
		foreach (NewAddress newAddress in this.addresses)
		{
			if (newAddress.loadedVarIndex > -1)
			{
				AddressLayoutVariation addressLayoutVariation = null;
				try
				{
					addressLayoutVariation = newAddress.saveData.vs[newAddress.loadedVarIndex];
				}
				catch
				{
					Game.Log(newAddress.name + ": Unable to find this variation in floor save data: " + newAddress.loadedVarIndex.ToString(), 2);
				}
				if (addressLayoutVariation != null)
				{
					int num = 0;
					foreach (RoomSaveData roomSaveData in addressLayoutVariation.r_d)
					{
						foreach (NodeSaveData nodeSaveData in roomSaveData.n_d)
						{
							Vector2Int vector2Int = this.building.FaceLocalNodeVector(nodeSaveData.f_c);
							NewNode newNode = null;
							if (this.nodeMap.TryGetValue(vector2Int, ref newNode) && !newNode.tile.isEdge)
							{
								foreach (WallSaveData wallSaveData in nodeSaveData.w_d)
								{
									Vector2 facedWallOffset = this.building.FaceWallOffsetVector(wallSaveData.w_o);
									NewWall newWall = newNode.walls.Find((NewWall item) => item.wallOffset == facedWallOffset);
									if (newWall != null)
									{
										DoorPairPreset doorPairPreset = null;
										if (Toolbox.Instance.LoadDataFromResources<DoorPairPreset>(wallSaveData.p_n, out doorPairPreset))
										{
											newWall.SetDoorPairPreset(doorPairPreset, false, false, true);
											if (doorPairPreset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
											{
												if (newWall.parentWall != null && newWall.parentWall.node.gameLocation != null)
												{
													newWall.parentWall.node.gameLocation.AddEntrance(newWall.parentWall.node, newWall.childWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
													newWall.childWall.node.gameLocation.AddEntrance(newWall.childWall.node, newWall.parentWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
												}
												if (newWall.otherWall != null)
												{
													if (newWall.node.building != null && newWall.otherWall.node.building == null)
													{
														if (newWall.node.tile.isMainEntrance)
														{
															newWall.node.building.AddBuildingEntrance(newWall, true);
														}
														else if (newWall.node.tile.isEntrance)
														{
															newWall.node.building.AddBuildingEntrance(newWall, false);
														}
													}
													else if (newWall.node.building == null && newWall.otherWall.node.building != null)
													{
														if (newWall.otherWall.node.tile.isMainEntrance)
														{
															newWall.otherWall.node.building.AddBuildingEntrance(newWall.otherWall, true);
														}
														else if (newWall.otherWall.node.tile.isEntrance)
														{
															newWall.otherWall.node.building.AddBuildingEntrance(newWall.otherWall, false);
														}
													}
												}
											}
										}
										else
										{
											Game.Log("Cannot find door pair " + wallSaveData.p_n, 2);
										}
									}
									else
									{
										Game.Log("Cannot find existing wall for wall pair", 2);
									}
									if (SessionData.Instance.isFloorEdit && wallSaveData.p_n != "0")
									{
										num++;
									}
								}
							}
						}
					}
					if (!SessionData.Instance.isFloorEdit)
					{
						newAddress.saveData = null;
					}
				}
			}
			else if (newAddress.nodes.Count > 0)
			{
				Game.Log(string.Concat(new string[]
				{
					newAddress.name,
					" (",
					newAddress.id.ToString(),
					") Unable to find this variation in floor save data: ",
					newAddress.loadedVarIndex.ToString()
				}), 2);
			}
		}
		GenerationController.Instance.UpdateGeometryFloor(this, "NewFloor");
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x001193A4 File Offset: 0x001175A4
	public NewAddress CreateNewAddress(LayoutConfiguration newRoomConfig, DesignStylePreset newDesign)
	{
		NewAddress component = Object.Instantiate<GameObject>(PrefabControls.Instance.address, base.transform).GetComponent<NewAddress>();
		component.Setup(this, newRoomConfig, newDesign);
		if (SessionData.Instance.isFloorEdit)
		{
			FloorEditController.Instance.SetTool(FloorEditController.FloorEditTool.addressDesignation, true);
			FloorEditController.Instance.addressDropdown.value = FloorEditController.Instance.addressDropdown.options.Count - 1;
		}
		return component;
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x00119414 File Offset: 0x00117614
	public void ConnectNodesOnFloor()
	{
		if (!SessionData.Instance.isFloorEdit)
		{
			return;
		}
		foreach (NewAddress newAddress in this.addresses)
		{
			newAddress.entrances.Clear();
		}
		foreach (NewAddress newAddress2 in this.addresses)
		{
			if (!(newAddress2 == this.outsideAddress) || Game.Instance.enableNewRealtimeTimeCullingSystem)
			{
				foreach (NewRoom newRoom in newAddress2.rooms)
				{
					newRoom.ConnectNodes();
				}
			}
		}
		if (!SessionData.Instance.isFloorEdit)
		{
			foreach (NewAddress newAddress3 in this.addresses)
			{
				foreach (NewRoom newRoom2 in newAddress3.rooms)
				{
					newRoom2.GenerateCullingTree(false);
				}
			}
		}
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x00119590 File Offset: 0x00117790
	public void AssignWindowUVData(bool debug = false)
	{
		this.frontWindowDebug.Clear();
		this.rearWindowDebug.Clear();
		this.leftWindowDebug.Clear();
		this.rightWindowDebug.Clear();
		List<NewWall> list = new List<NewWall>();
		foreach (NewAddress newAddress in this.addresses)
		{
			if (!newAddress.isOutside)
			{
				if (!newAddress.generatedRoomConfigs)
				{
					newAddress.GenerateRoomConfigs();
				}
				foreach (NewRoom newRoom in newAddress.rooms)
				{
					if (newRoom.preset == null)
					{
						Game.LogError(string.Concat(new string[]
						{
							"Room ",
							newRoom.roomType.name,
							" in ",
							newRoom.building.preset.name,
							" has no preset..."
						}), 2);
					}
					else if (!newRoom.IsOutside())
					{
						foreach (NewNode newNode in newRoom.nodes)
						{
							if (newNode.floorType != NewNode.FloorTileType.none)
							{
								foreach (NewWall newWall in newNode.walls)
								{
									if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.window || newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge)
									{
										if (newWall.node.room.IsOutside() != newWall.otherWall.node.room.IsOutside())
										{
											newRoom.windows.Add(newWall);
										}
										else if (newWall.node.gameLocation.isOutside != newWall.otherWall.node.gameLocation.isOutside)
										{
											newRoom.windows.Add(newWall);
										}
										else if (newWall.otherWall.node.room.preset != null && newWall.otherWall.node.room.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)
										{
											newRoom.windows.Add(newWall);
										}
										if (this.floor > 0 && !newWall.node.room.IsOutside() && !newWall.node.room.isNullRoom && (newWall.otherWall.node.room.IsOutside() || (newWall.otherWall.node.room.preset != null && newWall.otherWall.node.room.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)) && !list.Contains(newWall))
										{
											list.Add(newWall);
										}
									}
								}
							}
						}
						if (this.floor > 0)
						{
							newRoom.UpdateEmissionEndOfFrame();
						}
					}
				}
			}
		}
		if (this.floor <= 0)
		{
			return;
		}
		List<NewWall> list2 = new List<NewWall>();
		List<NewWall> list3 = new List<NewWall>();
		List<NewWall> list4 = new List<NewWall>();
		List<NewWall> list5 = new List<NewWall>();
		foreach (NewWall newWall2 in list)
		{
			Vector2 originalWallOffset = newWall2.node.building.GetOriginalWallOffset(newWall2.wallOffset);
			if (debug)
			{
				string[] array = new string[6];
				int num = 0;
				Vector2 vector = newWall2.wallOffset;
				array[num] = vector.ToString();
				array[1] = " = ";
				int num2 = 2;
				vector = originalWallOffset;
				array[num2] = vector.ToString();
				array[3] = " (";
				array[4] = newWall2.node.building.rotations.ToString();
				array[5] = ")";
				Game.Log(string.Concat(array), 2);
			}
			if (originalWallOffset.x < -0.1f)
			{
				list5.Add(newWall2);
				this.rightWindowDebug.Add(newWall2.node.room);
			}
			else if (originalWallOffset.x > 0.1f)
			{
				list4.Add(newWall2);
				this.leftWindowDebug.Add(newWall2.node.room);
			}
			else if (originalWallOffset.y < -0.1f)
			{
				list2.Add(newWall2);
				this.frontWindowDebug.Add(newWall2.node.room);
			}
			else if (originalWallOffset.y > 0.1f)
			{
				list3.Add(newWall2);
				this.rearWindowDebug.Add(newWall2.node.room);
			}
		}
		list2.Sort((NewWall p1, NewWall p2) => this.building.GetOriginalWallOffset(p1.node.floorCoord).x.CompareTo(this.building.GetOriginalWallOffset(p2.node.floorCoord).x));
		list3.Sort((NewWall p1, NewWall p2) => this.building.GetOriginalWallOffset(p2.node.floorCoord).x.CompareTo(this.building.GetOriginalWallOffset(p1.node.floorCoord).x));
		list5.Sort((NewWall p1, NewWall p2) => this.building.GetOriginalWallOffset(p2.node.floorCoord).y.CompareTo(this.building.GetOriginalWallOffset(p1.node.floorCoord).y));
		list4.Sort((NewWall p1, NewWall p2) => this.building.GetOriginalWallOffset(p1.node.floorCoord).y.CompareTo(this.building.GetOriginalWallOffset(p2.node.floorCoord).y));
		if (debug)
		{
			foreach (NewWall newWall3 in list2)
			{
				Game.Log("Sort front X highest first: " + newWall3.node.building.GetOriginalWallOffset(newWall3.node.floorCoord).ToString(), 2);
			}
			foreach (NewWall newWall4 in list3)
			{
				Game.Log("Sort rear X lowest first: " + newWall4.node.building.GetOriginalWallOffset(newWall4.node.floorCoord).ToString(), 2);
			}
			foreach (NewWall newWall5 in list5)
			{
				Game.Log("Sort right Y lowest first: " + newWall5.node.building.GetOriginalWallOffset(newWall5.node.floorCoord).ToString(), 2);
			}
			foreach (NewWall newWall6 in list4)
			{
				Game.Log("Sort left Y highest first: " + newWall6.node.building.GetOriginalWallOffset(newWall6.node.floorCoord).ToString(), 2);
			}
		}
		BuildingPreset.WindowUVFloor windowUVFloor = null;
		try
		{
			windowUVFloor = this.building.preset.sortedWindows[this.floor - 1];
		}
		catch
		{
			return;
		}
		int k;
		int j;
		for (k = 0; k < list2.Count; k = j + 1)
		{
			list2[k].windowUVHorizonalPosition = k;
			list2[k].windowUV = windowUVFloor.front.Find((BuildingPreset.WindowUVBlock item) => item.horizonal == k);
			if (list2[k].windowUV != null)
			{
				list2[k].node.room.windowsWithUVData.Add(list2[k]);
			}
			j = k;
		}
		int l;
		for (l = 0; l < list3.Count; l = j + 1)
		{
			list3[l].windowUVHorizonalPosition = l;
			list3[l].windowUV = windowUVFloor.back.Find((BuildingPreset.WindowUVBlock item) => item.horizonal == l);
			if (list3[l].windowUV != null)
			{
				list3[l].node.room.windowsWithUVData.Add(list3[l]);
			}
			j = l;
		}
		int m;
		for (m = 0; m < list4.Count; m = j + 1)
		{
			list4[m].windowUVHorizonalPosition = m;
			list4[m].windowUV = windowUVFloor.left.Find((BuildingPreset.WindowUVBlock item) => item.horizonal == m);
			if (list4[m].windowUV != null)
			{
				list4[m].node.room.windowsWithUVData.Add(list4[m]);
			}
			j = m;
		}
		int i;
		for (i = 0; i < list5.Count; i = j + 1)
		{
			list5[i].windowUVHorizonalPosition = i;
			list5[i].windowUV = windowUVFloor.right.Find((BuildingPreset.WindowUVBlock item) => item.horizonal == i);
			if (list5[i].windowUV != null)
			{
				list5[i].node.room.windowsWithUVData.Add(list5[i]);
			}
			j = i;
		}
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x0011A094 File Offset: 0x00118294
	public void GenerateAirDucts()
	{
		foreach (NewAddress newAddress in this.addresses)
		{
			newAddress.SelectAirVentLocations();
		}
	}

	// Token: 0x06001396 RID: 5014 RVA: 0x0011A0E4 File Offset: 0x001182E4
	public void AddSecurityDoor(Interactable newInteractable)
	{
		this.securityDoors.Add(newInteractable);
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x0011A0F4 File Offset: 0x001182F4
	public NewAddress GetLobbyAddress()
	{
		List<NewAddress> list = this.addresses.FindAll((NewAddress item) => item.isLobby && item.nodes.Count > 0);
		return list[Toolbox.Instance.Rand(0, list.Count, false)];
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x0011A144 File Offset: 0x00118344
	public void SetAlarmLockdown(bool newVal, NewAddress addressOnly = null)
	{
		if (newVal != this.alarmLockdown)
		{
			this.alarmLockdown = newVal;
			Game.Log(string.Concat(new string[]
			{
				"Debug: Locking down floor: ",
				this.alarmLockdown.ToString(),
				" ",
				this.floor.ToString(),
				" by closing/opening ",
				this.securityDoors.Count.ToString(),
				" doors..."
			}), 2);
			foreach (Interactable interactable in this.securityDoors)
			{
				Game.Log(string.Concat(new string[]
				{
					"Debug: Door at ",
					interactable.node.gameLocation.name,
					" (address only: ",
					(addressOnly != null) ? addressOnly.ToString() : null,
					")"
				}), 2);
				if (addressOnly == null || addressOnly == interactable.node.gameLocation)
				{
					Game.Log("Debug: Door " + interactable.id.ToString() + ": " + (!this.alarmLockdown).ToString(), 2);
					interactable.SetSwitchState(!this.alarmLockdown, null, true, false, false);
					if (interactable.node.gameLocation.thisAsAddress != null)
					{
						Interactable interactable2 = interactable.node.gameLocation.thisAsAddress.GetBreakerDoors();
						if (interactable2 != null)
						{
							interactable2.SetSwitchState(!this.alarmLockdown, null, true, false, false);
							Game.Log("Debug: Set breaker: " + this.alarmLockdown.ToString(), 2);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x0011A32C File Offset: 0x0011852C
	public CitySaveData.FloorCitySave GenerateSaveData()
	{
		CitySaveData.FloorCitySave floorCitySave = new CitySaveData.FloorCitySave();
		floorCitySave.name = this.floorName;
		floorCitySave.floorID = this.floorID;
		floorCitySave.floor = this.floor;
		floorCitySave.size = this.size;
		floorCitySave.defaultFloorHeight = this.defaultFloorHeight;
		floorCitySave.defaultCeilingHeight = this.defaultCeilingHeight;
		floorCitySave.layoutIndex = this.layoutIndex;
		floorCitySave.echelons = this.isEchelons;
		floorCitySave.breakerSec = this.breakerSecurityID;
		floorCitySave.breakerLights = this.breakerLightsID;
		floorCitySave.breakerDoors = this.breakerDoorsID;
		foreach (NewAddress newAddress in this.addresses)
		{
			floorCitySave.addresses.Add(newAddress.GenerateSaveData());
		}
		foreach (KeyValuePair<Vector2Int, NewTile> keyValuePair in this.tileMap)
		{
			floorCitySave.tiles.Add(keyValuePair.Value.GenerateSaveData());
		}
		return floorCitySave;
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x0011A468 File Offset: 0x00118668
	[Button(null, 0)]
	public void DebugWindowUVAssign()
	{
		this.AssignWindowUVData(true);
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x0011A471 File Offset: 0x00118671
	public void SetBreakerSecurity(Interactable newObject)
	{
		this.breakerSecurity = newObject;
		if (this.breakerSecurity != null)
		{
			this.breakerSecurityID = this.breakerSecurity.id;
		}
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x0011A493 File Offset: 0x00118693
	public void SetBreakerLights(Interactable newObject)
	{
		this.breakerLights = newObject;
		if (this.breakerLights != null)
		{
			this.breakerLightsID = this.breakerLights.id;
		}
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x0011A4B5 File Offset: 0x001186B5
	public void SetBreakerDoors(Interactable newObject)
	{
		this.breakerDoors = newObject;
		if (this.breakerDoors != null)
		{
			this.breakerDoorsID = this.breakerDoors.id;
		}
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x0011A4D8 File Offset: 0x001186D8
	public Interactable GetBreakerSecurity()
	{
		if (this.breakerSecurity != null)
		{
			return this.breakerSecurity;
		}
		if (this.breakerSecurityID > -1)
		{
			this.breakerSecurity = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.breakerSecurityID);
		}
		if (this.breakerSecurity != null)
		{
			return this.breakerSecurity;
		}
		return null;
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x0011A530 File Offset: 0x00118730
	public Interactable GetBreakerLights()
	{
		if (this.breakerLights != null)
		{
			return this.breakerLights;
		}
		if (this.breakerLightsID > -1)
		{
			this.breakerLights = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.breakerLightsID);
		}
		if (this.breakerLights != null)
		{
			return this.breakerLights;
		}
		return null;
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x0011A588 File Offset: 0x00118788
	public Interactable GetBreakerDoors()
	{
		if (this.breakerDoors != null)
		{
			return this.breakerDoors;
		}
		if (this.breakerDoorsID > -1)
		{
			this.breakerDoors = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.breakerDoorsID);
		}
		if (this.breakerDoors != null)
		{
			return this.breakerDoors;
		}
		return null;
	}

	// Token: 0x040017E6 RID: 6118
	[Header("ID")]
	public int floorID;

	// Token: 0x040017E7 RID: 6119
	public static int assignID;

	// Token: 0x040017E8 RID: 6120
	[Header("Location")]
	public NewBuilding building;

	// Token: 0x040017E9 RID: 6121
	public int floor;

	// Token: 0x040017EA RID: 6122
	public int assignResidence = 1;

	// Token: 0x040017EB RID: 6123
	[Header("Floor Contents")]
	public List<NewAddress> addresses = new List<NewAddress>();

	// Token: 0x040017EC RID: 6124
	public NewAddress lobbyAddress;

	// Token: 0x040017ED RID: 6125
	public NewAddress outsideAddress;

	// Token: 0x040017EE RID: 6126
	public Dictionary<Vector2Int, NewTile> tileMap = new Dictionary<Vector2Int, NewTile>();

	// Token: 0x040017EF RID: 6127
	public Dictionary<Vector2Int, NewNode> nodeMap = new Dictionary<Vector2Int, NewNode>();

	// Token: 0x040017F0 RID: 6128
	public List<NewWall> buildingEntrances = new List<NewWall>();

	// Token: 0x040017F1 RID: 6129
	public List<Interactable> securityDoors = new List<Interactable>();

	// Token: 0x040017F2 RID: 6130
	public bool alarmLockdown;

	// Token: 0x040017F3 RID: 6131
	public int layoutIndex;

	// Token: 0x040017F4 RID: 6132
	public int breakerSecurityID = -1;

	// Token: 0x040017F5 RID: 6133
	public int breakerDoorsID = -1;

	// Token: 0x040017F6 RID: 6134
	public int breakerLightsID = -1;

	// Token: 0x040017F7 RID: 6135
	[NonSerialized]
	public Interactable breakerSecurity;

	// Token: 0x040017F8 RID: 6136
	[NonSerialized]
	public Interactable breakerDoors;

	// Token: 0x040017F9 RID: 6137
	[NonSerialized]
	public Interactable breakerLights;

	// Token: 0x040017FA RID: 6138
	public float breakerSecurityState = 1f;

	// Token: 0x040017FB RID: 6139
	public float breakerLightsState = 1f;

	// Token: 0x040017FC RID: 6140
	public float breakerDoorsState = 1f;

	// Token: 0x040017FD RID: 6141
	[Tooltip("The name of the floor data")]
	[Header("Details")]
	public string floorName = "newFloor";

	// Token: 0x040017FE RID: 6142
	[Tooltip("The size of this configuration in 7x7 cells")]
	public Vector2 size = new Vector2(1f, 1f);

	// Token: 0x040017FF RID: 6143
	[Tooltip("The default floor height (voxel units == 0.1)")]
	public int defaultFloorHeight;

	// Token: 0x04001800 RID: 6144
	[Tooltip("The default ceiling height (voxel units == 0.1)")]
	public int defaultCeilingHeight = 42;

	// Token: 0x04001801 RID: 6145
	public bool isEchelons;

	// Token: 0x04001802 RID: 6146
	[Header("Map")]
	public MapDuctsButtonController mapDucts;

	// Token: 0x04001803 RID: 6147
	[Header("Save Data")]
	public int maxDuctExtrusion;

	// Token: 0x04001804 RID: 6148
	private FloorSaveData saveData;

	// Token: 0x04001805 RID: 6149
	[Header("Debug")]
	public List<NewRoom> frontWindowDebug = new List<NewRoom>();

	// Token: 0x04001806 RID: 6150
	public List<NewRoom> rearWindowDebug = new List<NewRoom>();

	// Token: 0x04001807 RID: 6151
	public List<NewRoom> leftWindowDebug = new List<NewRoom>();

	// Token: 0x04001808 RID: 6152
	public List<NewRoom> rightWindowDebug = new List<NewRoom>();

	// Token: 0x02000365 RID: 869
	// (Invoke) Token: 0x060013AA RID: 5034
	public delegate void SaveDataComplete(NewFloor floor, FloorSaveData data);
}
