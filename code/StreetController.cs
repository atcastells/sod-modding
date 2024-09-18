using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class StreetController : NewGameLocation, IComparable<StreetController>
{
	// Token: 0x06000919 RID: 2329 RVA: 0x0008F43C File Offset: 0x0008D63C
	public void Setup(DistrictController newDistrict)
	{
		this.streetID = StreetController.assignID;
		StreetController.assignID++;
		this.district = newDistrict;
		base.name = "Street " + this.streetID.ToString();
		base.transform.name = base.name;
		base.transform.SetParent(newDistrict.transform, true);
		this.CreateEvidence();
		base.CommonSetup(true, newDistrict, CityControls.Instance.street);
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x0008F4C0 File Offset: 0x0008D6C0
	public void Load(CitySaveData.StreetCitySave data)
	{
		base.name = data.name;
		base.transform.name = base.name;
		this.residenceNumber = data.residenceNumber;
		this.isLobby = data.isLobby;
		this.isOutside = data.isOutside;
		this.access = data.access;
		this.streetID = data.streetID;
		StreetController.assignID = Mathf.Max(StreetController.assignID, this.streetID + 1);
		this.district = HighlanderSingleton<CityDistricts>.Instance.districtDirectory.Find((DistrictController item) => item.districtID == data.district);
		this.streetSuffix = data.streetSuffix;
		this.isAlley = data.isAlley;
		this.isBackstreet = data.isBackstreet;
		this.CreateEvidence();
		if (data.sharedGround != null)
		{
			this.sharedGroundElements.Clear();
			using (List<int>.Enumerator enumerator = data.sharedGround.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int stID = enumerator.Current;
					StreetController streetController = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == stID);
					if (streetController != null)
					{
						this.sharedGroundElements.Add(streetController);
					}
				}
			}
		}
		base.transform.SetParent(this.district.transform, true);
		base.CommonSetup(true, this.district, CityControls.Instance.street);
		foreach (Vector3Int vector3Int in data.tiles)
		{
			NewTile foundTile = PathFinder.Instance.tileMap[vector3Int];
			this.AddTile(foundTile);
			if (!foundTile.isSetup)
			{
				CitySaveData.TileCitySave data2 = CityConstructor.Instance.currentData.cityTiles.Find((CitySaveData.CityTileCitySave item) => item.cityCoord == foundTile.cityTile.cityCoord).outsideTiles.Find((CitySaveData.TileCitySave item) => item.tileID == foundTile.tileID);
				foundTile.LoadExterior(data2);
				foundTile.parent = foundTile.streetController.transform;
			}
		}
		using (List<CitySaveData.RoomCitySave>.Enumerator enumerator3 = data.rooms.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				CitySaveData.RoomCitySave room = enumerator3.Current;
				NewRoom newRoom;
				if (!room.isBaseNullRoom)
				{
					newRoom = Object.Instantiate<GameObject>(PrefabControls.Instance.room, base.transform).GetComponent<NewRoom>();
				}
				else
				{
					newRoom = this.nullRoom;
				}
				CitySaveData.RoomCitySave data3 = room;
				if (CityConstructor.Instance != null && CityConstructor.Instance.saveState != null)
				{
					StateSaveData.RoomStateSave roomStateSave = CityConstructor.Instance.saveState.rooms.Find((StateSaveData.RoomStateSave item) => item.id == room.id && item.decorOverride != null && item.decorOverride.Count > 0);
					if (roomStateSave != null)
					{
						Game.Log("CityGen: Found save state room decor override for: " + room.name, 2);
						data3 = roomStateSave.decorOverride[0];
						newRoom.decorEdit = true;
					}
				}
				newRoom.Load(data3, this);
			}
		}
		this.streetSections.AddRange(data.streetTiles);
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x0008F89C File Offset: 0x0008DA9C
	public void AddTile(NewTile newTile)
	{
		if (!this.tiles.Contains(newTile))
		{
			this.tiles.Add(newTile);
			newTile.streetController = this;
			newTile.parent = newTile.streetController.transform;
			foreach (NewNode newNode in newTile.nodes)
			{
				newTile.streetController.nullRoom.AddNewNode(newNode);
			}
		}
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0008F92C File Offset: 0x0008DB2C
	public void RemoveTile(NewTile newTile)
	{
		if (this.tiles.Contains(newTile))
		{
			newTile.streetController = null;
			this.tiles.Remove(newTile);
		}
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0008F950 File Offset: 0x0008DB50
	public void SetAsAlley()
	{
		this.isAlley = true;
		foreach (NewRoom newRoom in this.rooms)
		{
			newRoom.SetConfiguration(CityControls.Instance.alleyRoom);
		}
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0008F9B4 File Offset: 0x0008DBB4
	public void SetAsBackstreet()
	{
		this.isBackstreet = true;
		foreach (NewRoom newRoom in this.rooms)
		{
			newRoom.SetConfiguration(CityControls.Instance.backstreetRoom);
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x0008FA18 File Offset: 0x0008DC18
	public void SetAsStreet()
	{
		foreach (NewRoom newRoom in this.rooms)
		{
			newRoom.SetConfiguration(CityControls.Instance.streetRoom);
		}
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x0008FA74 File Offset: 0x0008DC74
	public void UpdateName(bool forceTrueRandom = false)
	{
		if (this.isPlayerEditedName)
		{
			base.name = this.playerEditedStreetName;
			base.transform.name = base.name;
			return;
		}
		Descriptors.EthnicGroup chosenGroup = this.district.EthnictiyBasedOnDominance();
		SocialStatistics.EthnicityStats ethnicityStats = SocialStatistics.Instance.ethnicityStats.Find((SocialStatistics.EthnicityStats item) => item.group == chosenGroup);
		string text = chosenGroup.ToString();
		if (ethnicityStats.overrideSur)
		{
			text = ethnicityStats.overrideNameSur.ToString();
		}
		string text2 = this.seed;
		if (forceTrueRandom)
		{
			text2 = Toolbox.Instance.GenerateUniqueID();
		}
		base.name = NameGenerator.Instance.GenerateName(null, 0f, "names." + text + ".sur", 1f, null, 0f, text2);
		if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 8, text2, out text2) >= this.tiles.Count)
		{
			this.streetSuffix = NameGenerator.Instance.GenerateName(null, 0f, "names.street.suffix.small", 1f, null, 0f, text2);
		}
		else
		{
			Vector2 vector;
			vector..ctor(float.PositiveInfinity, -1f);
			Vector2 vector2;
			vector2..ctor(float.PositiveInfinity, -1f);
			foreach (NewTile newTile in this.tiles)
			{
				vector = new Vector2(Mathf.Min(vector.x, (float)newTile.globalTileCoord.x), Mathf.Max(vector.x, (float)newTile.globalTileCoord.x));
				vector2 = new Vector2(Mathf.Min(vector2.y, (float)newTile.globalTileCoord.y), Mathf.Max(vector2.y, (float)newTile.globalTileCoord.y));
			}
			if (Mathf.Abs(vector.y - vector.x) < 1f)
			{
				this.streetSuffix = Strings.Get("names.street.suffix", "avenue", Strings.Casing.asIs, false, false, false, null);
			}
			else if (Mathf.Abs(vector2.y - vector2.x) < 1f)
			{
				this.streetSuffix = Strings.Get("names.street.suffix", "street", Strings.Casing.asIs, false, false, false, null);
			}
			else if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, text2, out text2) > 50)
			{
				this.streetSuffix = Strings.Get("names.street.suffix", "boulevard", Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				this.streetSuffix = Strings.Get("names.street.suffix", "parade", Strings.Casing.asIs, false, false, false, null);
			}
		}
		base.name = base.name + " " + this.streetSuffix;
		int num = 99;
		while (CityData.Instance.streetDirectory.Exists((StreetController item) => item != this && item.name == this.name) && num > 0)
		{
			base.name = NameGenerator.Instance.GenerateName(null, 0f, "names." + text + ".sur", 1f, null, 0f, this.seed);
			if (Toolbox.Instance.GetPsuedoRandomNumber(0, 8, this.seed, false) >= this.tiles.Count)
			{
				this.streetSuffix = NameGenerator.Instance.GenerateName(null, 0f, "names.street.suffix.small", 1f, null, 0f, this.seed);
			}
			else
			{
				Vector2 vector3;
				vector3..ctor(float.PositiveInfinity, -1f);
				Vector2 vector4;
				vector4..ctor(float.PositiveInfinity, -1f);
				foreach (NewTile newTile2 in this.tiles)
				{
					vector3 = new Vector2(Mathf.Min(vector3.x, (float)newTile2.globalTileCoord.x), Mathf.Max(vector3.x, (float)newTile2.globalTileCoord.x));
					vector4 = new Vector2(Mathf.Min(vector4.y, (float)newTile2.globalTileCoord.y), Mathf.Max(vector4.y, (float)newTile2.globalTileCoord.y));
				}
				if (Mathf.Abs(vector3.y - vector3.x) < 1f)
				{
					this.streetSuffix = Strings.Get("names.street.suffix", "avenue", Strings.Casing.asIs, false, false, false, null);
				}
				else if (Mathf.Abs(vector4.y - vector4.x) < 1f)
				{
					this.streetSuffix = Strings.Get("names.street.suffix", "street", Strings.Casing.asIs, false, false, false, null);
				}
				else if (Toolbox.Instance.GetPsuedoRandomNumber(0, 100, this.seed, false) > 50)
				{
					this.streetSuffix = Strings.Get("names.street.suffix", "boulevard", Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					this.streetSuffix = Strings.Get("names.street.suffix", "parade", Strings.Casing.asIs, false, false, false, null);
				}
			}
			num--;
		}
		base.transform.name = base.name;
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x0008FFAC File Offset: 0x0008E1AC
	public override bool IsOutside()
	{
		return true;
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0008FFAF File Offset: 0x0008E1AF
	public void AddChunk(PathFinder.StreetChunk newChunk)
	{
		if (!this.streetChunks.Contains(newChunk))
		{
			this.streetChunks.Add(newChunk);
		}
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x0008FFCC File Offset: 0x0008E1CC
	public List<StreetController> GetNeighboringStreets()
	{
		List<StreetController> list = new List<StreetController>();
		foreach (NewTile newTile in this.tiles)
		{
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
			{
				Vector3 vector = newTile.globalTileCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
				NewTile newTile2 = null;
				if (PathFinder.Instance.tileMap.TryGetValue(vector, ref newTile2) && newTile2.streetController != null && newTile2.streetController != this && !list.Contains(newTile2.streetController))
				{
					list.Add(newTile2.streetController);
				}
			}
		}
		return list;
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x000900CC File Offset: 0x0008E2CC
	public int CompareTo(StreetController otherObject)
	{
		return this.nullRoom.nodes.Count.CompareTo(otherObject.nullRoom.nodes.Count);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00090104 File Offset: 0x0008E304
	public override void CreateEvidence()
	{
		if (this.evidenceEntry == null)
		{
			this.evidenceEntry = (EvidenceCreator.Instance.CreateEvidence("street", "Street" + this.streetID.ToString(), this, null, null, null, null, false, null) as EvidenceLocation);
		}
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0009014F File Offset: 0x0008E34F
	public override void SetupEvidence()
	{
		base.SetupEvidence();
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00090158 File Offset: 0x0008E358
	public CitySaveData.StreetCitySave GenerateSaveData()
	{
		CitySaveData.StreetCitySave streetCitySave = new CitySaveData.StreetCitySave();
		streetCitySave.name = base.name;
		streetCitySave.district = this.district.districtID;
		streetCitySave.residenceNumber = this.residenceNumber;
		streetCitySave.isLobby = this.isLobby;
		streetCitySave.isOutside = this.isOutside;
		streetCitySave.access = this.access;
		streetCitySave.designStyle = this.designStyle.name;
		streetCitySave.streetID = this.streetID;
		streetCitySave.streetTiles = new List<StreetController.StreetTile>(this.streetSections);
		foreach (NewTile newTile in this.tiles)
		{
			streetCitySave.tiles.Add(Vector3Int.RoundToInt(newTile.globalTileCoord));
		}
		streetCitySave.streetSuffix = this.streetSuffix;
		streetCitySave.isAlley = this.isAlley;
		streetCitySave.isBackstreet = this.isBackstreet;
		foreach (NewRoom newRoom in this.rooms)
		{
			streetCitySave.rooms.Add(newRoom.GenerateSaveData());
		}
		if (this.sharedGroundElements.Count > 0)
		{
			streetCitySave.sharedGround = new List<int>();
			foreach (StreetController streetController in this.sharedGroundElements)
			{
				streetCitySave.sharedGround.Add(streetController.streetID);
			}
		}
		return streetCitySave;
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x0009031C File Offset: 0x0008E51C
	public void LoadStreetTiles()
	{
		foreach (PathFinder.StreetChunk streetChunk in this.streetChunks)
		{
			if (streetChunk.allTiles.Count > 0)
			{
				Vector3 vector = Vector3.zero;
				foreach (NewTile newTile in streetChunk.allTiles)
				{
					vector += CityData.Instance.TileToRealpos(newTile.globalTileCoord);
				}
				vector /= (float)streetChunk.allTiles.Count;
				if (streetChunk.isJunction)
				{
					Dictionary<Vector2, PathFinder.StreetChunk> dictionary = new Dictionary<Vector2, PathFinder.StreetChunk>();
					Dictionary<Vector2, bool> dictionary2 = new Dictionary<Vector2, bool>();
					int num = 0;
					foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
					{
						dictionary2.Add(vector2Int, false);
					}
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					foreach (Vector3 vector2 in streetChunk.allCoords)
					{
						foreach (Vector2Int vector2Int2 in CityData.Instance.offsetArrayX4)
						{
							if (!dictionary.ContainsKey(vector2Int2))
							{
								Vector3 vector3 = vector2 + new Vector3((float)vector2Int2.x, (float)vector2Int2.y, 0f);
								NewTile newTile2;
								if (PathFinder.Instance.tileMap.TryGetValue(vector3, ref newTile2) && newTile2.streetChunk != null && newTile2.streetChunk != streetChunk)
								{
									dictionary.Add(vector2Int2, newTile2.streetChunk);
									if (!newTile2.streetController.isAlley && !newTile2.streetController.isBackstreet)
									{
										dictionary2[vector2Int2] = true;
										num++;
										if (vector2Int2.x < 0)
										{
											flag = true;
										}
										else if (vector2Int2.x > 0)
										{
											flag3 = true;
										}
										else if (vector2Int2.y > 0)
										{
											flag2 = true;
										}
										else if (vector2Int2.y < 0)
										{
											flag4 = true;
										}
									}
								}
							}
						}
					}
					if (num <= 0)
					{
						for (int j = 0; j < CityData.Instance.offsetArrayX4StreetJunction.Length; j++)
						{
							Vector2 vector4 = CityData.Instance.offsetArrayX4StreetJunction[j];
							Vector3 newWorldPos = vector + new Vector3(PathFinder.Instance.tileSize.x * vector4.x, 0f, PathFinder.Instance.tileSize.y * vector4.y);
							this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos, StreetTilePreset.StreetSection.joinerShort, j * 90));
						}
					}
					else if (num >= 4)
					{
						for (int k = 0; k < CityData.Instance.offsetArrayX4StreetJunction.Length; k++)
						{
							Vector2 vector5 = CityData.Instance.offsetArrayX4StreetJunction[k];
							Vector3 newWorldPos2 = vector + new Vector3(PathFinder.Instance.tileSize.x * vector5.x, 0f, PathFinder.Instance.tileSize.y * vector5.y);
							this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos2, StreetTilePreset.StreetSection.streetJunctionCorner, k * 90));
						}
					}
					else if (num == 1)
					{
						for (int l = 0; l < CityData.Instance.offsetArrayX4StreetJunction.Length; l++)
						{
							StreetTilePreset.StreetSection newSection = StreetTilePreset.StreetSection.streetInsideCorner;
							int newAngle = 0;
							if (flag)
							{
								if (l == 0)
								{
									newSection = StreetTilePreset.StreetSection.streetShort;
									newAngle = 270;
								}
								else if (l == 1)
								{
									newSection = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle = 270;
								}
								else if (l == 2)
								{
									newSection = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle = 0;
								}
								else if (l == 3)
								{
									newSection = StreetTilePreset.StreetSection.streetShort;
									newAngle = 90;
								}
							}
							else if (flag3)
							{
								if (l == 0)
								{
									newSection = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle = 180;
								}
								else if (l == 1)
								{
									newSection = StreetTilePreset.StreetSection.streetShort;
									newAngle = 270;
								}
								else if (l == 2)
								{
									newSection = StreetTilePreset.StreetSection.streetShort;
									newAngle = 90;
								}
								else if (l == 3)
								{
									newSection = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle = 90;
								}
							}
							else if (flag2)
							{
								if (l == 0)
								{
									newSection = StreetTilePreset.StreetSection.streetShort;
									newAngle = 180;
								}
								else if (l == 1)
								{
									newSection = StreetTilePreset.StreetSection.streetShort;
									newAngle = 0;
								}
								else if (l == 2)
								{
									newSection = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle = 0;
								}
								else if (l == 3)
								{
									newSection = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle = 90;
								}
							}
							else if (flag4)
							{
								if (l == 0)
								{
									newSection = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle = 180;
								}
								else if (l == 1)
								{
									newSection = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle = 270;
								}
								else if (l == 2)
								{
									newSection = StreetTilePreset.StreetSection.streetShort;
									newAngle = 0;
								}
								else if (l == 3)
								{
									newSection = StreetTilePreset.StreetSection.streetShort;
									newAngle = 180;
								}
							}
							Vector2 vector6 = CityData.Instance.offsetArrayX4StreetJunction[l];
							Vector3 newWorldPos3 = vector + new Vector3(PathFinder.Instance.tileSize.x * vector6.x, 0f, PathFinder.Instance.tileSize.y * vector6.y);
							this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos3, newSection, newAngle));
						}
					}
					else if (num == 2)
					{
						for (int m = 0; m < CityData.Instance.offsetArrayX4StreetJunction.Length; m++)
						{
							StreetTilePreset.StreetSection newSection2 = StreetTilePreset.StreetSection.streetInsideCorner;
							int newAngle2 = 0;
							if (flag && flag3)
							{
								if (m == 0)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 270;
								}
								else if (m == 1)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 270;
								}
								else if (m == 2)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 90;
								}
								else if (m == 3)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 90;
								}
							}
							else if (flag2 && flag4)
							{
								if (m == 0)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 180;
								}
								else if (m == 1)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 0;
								}
								else if (m == 2)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 0;
								}
								else if (m == 3)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 180;
								}
							}
							else if (flag && flag4)
							{
								if (m == 0)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 270;
								}
								else if (m == 1)
								{
									newSection2 = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle2 = 270;
								}
								else if (m == 2)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 0;
								}
								else if (m == 3)
								{
									newSection2 = StreetTilePreset.StreetSection.streetInsideCorner;
									newAngle2 = 270;
								}
							}
							else if (flag && flag2)
							{
								if (m == 0)
								{
									newSection2 = StreetTilePreset.StreetSection.streetInsideCorner;
									newAngle2 = 0;
								}
								else if (m == 1)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 0;
								}
								else if (m == 2)
								{
									newSection2 = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle2 = 0;
								}
								else if (m == 3)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 90;
								}
							}
							else if (flag3 && flag4)
							{
								if (m == 0)
								{
									newSection2 = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle2 = 180;
								}
								else if (m == 1)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 270;
								}
								else if (m == 2)
								{
									newSection2 = StreetTilePreset.StreetSection.streetInsideCorner;
									newAngle2 = 180;
								}
								else if (m == 3)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 180;
								}
							}
							else if (flag3 && flag2)
							{
								if (m == 0)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 180;
								}
								else if (m == 1)
								{
									newSection2 = StreetTilePreset.StreetSection.streetInsideCorner;
									newAngle2 = 90;
								}
								else if (m == 2)
								{
									newSection2 = StreetTilePreset.StreetSection.streetShort;
									newAngle2 = 90;
								}
								else if (m == 3)
								{
									newSection2 = StreetTilePreset.StreetSection.streetOutsideCorner;
									newAngle2 = 90;
								}
							}
							Vector2 vector7 = CityData.Instance.offsetArrayX4StreetJunction[m];
							Vector3 newWorldPos4 = vector + new Vector3(PathFinder.Instance.tileSize.x * vector7.x, 0f, PathFinder.Instance.tileSize.y * vector7.y);
							this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos4, newSection2, newAngle2));
						}
					}
					else if (num == 3)
					{
						for (int n = 0; n < CityData.Instance.offsetArrayX4StreetJunction.Length; n++)
						{
							StreetTilePreset.StreetSection newSection3 = StreetTilePreset.StreetSection.streetInsideCorner;
							int newAngle3 = 0;
							if (!flag)
							{
								if (n == 0)
								{
									newSection3 = StreetTilePreset.StreetSection.streetShort;
									newAngle3 = 180;
								}
								else if (n == 1)
								{
									newSection3 = StreetTilePreset.StreetSection.streetJunctionCorner;
									newAngle3 = 90;
								}
								else if (n == 2)
								{
									newSection3 = StreetTilePreset.StreetSection.streetJunctionCorner;
									newAngle3 = 180;
								}
								else if (n == 3)
								{
									newSection3 = StreetTilePreset.StreetSection.streetShort;
									newAngle3 = 180;
								}
							}
							else if (!flag2)
							{
								if (n == 0)
								{
									newSection3 = StreetTilePreset.StreetSection.streetShort;
									newAngle3 = 270;
								}
								else if (n == 1)
								{
									newSection3 = StreetTilePreset.StreetSection.streetShort;
									newAngle3 = 270;
								}
								else if (n == 2)
								{
									newSection3 = StreetTilePreset.StreetSection.streetJunctionCorner;
									newAngle3 = 180;
								}
								else if (n == 3)
								{
									newSection3 = StreetTilePreset.StreetSection.streetJunctionCorner;
									newAngle3 = 270;
								}
							}
							else if (!flag3)
							{
								if (n == 0)
								{
									newSection3 = StreetTilePreset.StreetSection.streetJunctionCorner;
									newAngle3 = 0;
								}
								else if (n == 1)
								{
									newSection3 = StreetTilePreset.StreetSection.streetShort;
									newAngle3 = 0;
								}
								else if (n == 2)
								{
									newSection3 = StreetTilePreset.StreetSection.streetShort;
									newAngle3 = 0;
								}
								else if (n == 3)
								{
									newSection3 = StreetTilePreset.StreetSection.streetJunctionCorner;
									newAngle3 = 270;
								}
							}
							else if (!flag4)
							{
								if (n == 0)
								{
									newSection3 = StreetTilePreset.StreetSection.streetJunctionCorner;
									newAngle3 = 0;
								}
								else if (n == 1)
								{
									newSection3 = StreetTilePreset.StreetSection.streetJunctionCorner;
									newAngle3 = 90;
								}
								else if (n == 2)
								{
									newSection3 = StreetTilePreset.StreetSection.streetShort;
									newAngle3 = 90;
								}
								else if (n == 3)
								{
									newSection3 = StreetTilePreset.StreetSection.streetShort;
									newAngle3 = 90;
								}
							}
							Vector2 vector8 = CityData.Instance.offsetArrayX4StreetJunction[n];
							Vector3 newWorldPos5 = vector + new Vector3(PathFinder.Instance.tileSize.x * vector8.x, 0f, PathFinder.Instance.tileSize.y * vector8.y);
							this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos5, newSection3, newAngle3));
						}
					}
				}
				else
				{
					StreetTilePreset.StreetSection newSection4 = StreetTilePreset.StreetSection.streetLong;
					if (this.isBackstreet || this.isAlley)
					{
						newSection4 = StreetTilePreset.StreetSection.joinerLong;
					}
					if (streetChunk.isHorizontal)
					{
						Vector3 newWorldPos6 = vector + new Vector3(0f, 0f, PathFinder.Instance.tileSize.y * 0.5f);
						Vector3 newWorldPos7 = vector - new Vector3(0f, 0f, PathFinder.Instance.tileSize.y * 0.5f);
						this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos6, newSection4, 90));
						this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos7, newSection4, 270));
					}
					else
					{
						Vector3 newWorldPos8 = vector - new Vector3(PathFinder.Instance.tileSize.x * 0.5f, 0f, 0f);
						Vector3 newWorldPos9 = vector + new Vector3(PathFinder.Instance.tileSize.x * 0.5f, 0f, 0f);
						this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos8, newSection4, 0));
						this.streetSections.Add(new StreetController.StreetTile(streetChunk.name, newWorldPos9, newSection4, 180));
					}
				}
			}
		}
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00090E34 File Offset: 0x0008F034
	public void LoadSections()
	{
		foreach (StreetController.StreetTile streetTile in this.streetSections)
		{
			string seed = streetTile.worldPos.ToString() + streetTile.angle.ToString();
			StreetTilePreset.StreetSectionModel model = this.GetModel(streetTile.section, seed);
			GameObject gameObject = Object.Instantiate<GameObject>(model.prefab, this.nullRoom.transform);
			gameObject.transform.position = streetTile.worldPos;
			gameObject.transform.rotation = Quaternion.Euler(0f, (float)streetTile.angle, 0f);
			gameObject.transform.tag = "Street";
			gameObject.name = streetTile.name;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
			this.loadedModelReference.Add(component, model);
			if (Game.Instance.enableRaindrops && model.rainMaterial != null)
			{
				component.sharedMaterial = SessionData.Instance.GetWeatherAffectedMaterial(model.rainMaterial, component);
			}
			else
			{
				component.sharedMaterial = model.normalMaterial;
			}
			this.AddForStaticBatching(gameObject, component2.sharedMesh, component.sharedMaterial);
		}
		this.ExecuteStaticBatching();
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00090FAC File Offset: 0x0008F1AC
	public void AddForStaticBatching(GameObject obj, Mesh objectMesh, Material objectMat)
	{
		if (Game.Instance.enableRuntimeStaticBatching)
		{
			NewRoom.StaticBatchKey staticBatchKey = new NewRoom.StaticBatchKey
			{
				mesh = objectMesh,
				mat = objectMat
			};
			if (!this.staticBatchDictionary.ContainsKey(staticBatchKey))
			{
				this.staticBatchDictionary.Add(staticBatchKey, new List<GameObject>());
				this.staticBatchDictionary[staticBatchKey].Add(obj);
				return;
			}
			if (!this.staticBatchDictionary[staticBatchKey].Contains(obj))
			{
				this.staticBatchDictionary[staticBatchKey].Add(obj);
			}
		}
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x00091038 File Offset: 0x0008F238
	public void ExecuteStaticBatching()
	{
		if (Game.Instance.enableRuntimeStaticBatching)
		{
			foreach (KeyValuePair<NewRoom.StaticBatchKey, List<GameObject>> keyValuePair in this.staticBatchDictionary)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					if (keyValuePair.Value[i] == null)
					{
						keyValuePair.Value.RemoveAt(i);
						i--;
					}
				}
				if (keyValuePair.Value.Count > 1)
				{
					try
					{
						StaticBatchingUtility.Combine(keyValuePair.Value.ToArray(), keyValuePair.Value[0]);
					}
					catch
					{
						Game.LogError("Error trying to execute static batching...", 2);
					}
				}
			}
			this.staticBatchDictionary.Clear();
		}
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0009112C File Offset: 0x0008F32C
	private StreetTilePreset.StreetSectionModel GetModel(StreetTilePreset.StreetSection section, string seed)
	{
		List<StreetTilePreset> list = Toolbox.Instance.allStreetTiles.FindAll((StreetTilePreset item) => item.sectionType == section);
		if (list.Count > 0)
		{
			int psuedoRandomNumber = Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, seed, false);
			StreetTilePreset streetTilePreset = list[psuedoRandomNumber];
			int psuedoRandomNumber2 = Toolbox.Instance.GetPsuedoRandomNumber(0, streetTilePreset.prefabList.Count, seed, false);
			return streetTilePreset.prefabList[psuedoRandomNumber2];
		}
		Game.LogError("CityGen: Unable to find street tile preset " + section.ToString(), 2);
		return null;
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x000911D4 File Offset: 0x0008F3D4
	public NewNode GetDestinationNode()
	{
		if (Player.Instance.currentGameLocation == this)
		{
			return Player.Instance.FindSafeTeleport(this, false, true);
		}
		NewNode result = null;
		float num = float.PositiveInfinity;
		foreach (NewNode.NodeAccess nodeAccess in this.entrances)
		{
			if (nodeAccess.walkingAccess)
			{
				NewNode newNode;
				if (nodeAccess.fromNode.gameLocation == this)
				{
					newNode = nodeAccess.fromNode;
				}
				else
				{
					newNode = nodeAccess.toNode;
				}
				float num2 = Vector3.Distance(newNode.position, Player.Instance.transform.position);
				if (num2 < num)
				{
					result = newNode;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x000912A4 File Offset: 0x0008F4A4
	[Button(null, 0)]
	public void Redecorate()
	{
		this.rooms[0].RemoveAllInhabitantFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.remove);
		GenerationController.Instance.FurnishRoom(this.rooms[0]);
	}

	// Token: 0x0400094C RID: 2380
	[Header("ID")]
	public int streetID;

	// Token: 0x0400094D RID: 2381
	public static int assignID = 1;

	// Token: 0x0400094E RID: 2382
	[Header("Custom Editor Flags")]
	public bool isPlayerEditedName;

	// Token: 0x0400094F RID: 2383
	public string playerEditedStreetName = "";

	// Token: 0x04000950 RID: 2384
	[Header("Details")]
	public List<NewTile> tiles = new List<NewTile>();

	// Token: 0x04000951 RID: 2385
	public string streetSuffix = string.Empty;

	// Token: 0x04000952 RID: 2386
	public bool isAlley;

	// Token: 0x04000953 RID: 2387
	public bool isBackstreet;

	// Token: 0x04000954 RID: 2388
	public float normalizedFootfall;

	// Token: 0x04000955 RID: 2389
	public int chunkSize;

	// Token: 0x04000956 RID: 2390
	private Dictionary<NewRoom.StaticBatchKey, List<GameObject>> staticBatchDictionary = new Dictionary<NewRoom.StaticBatchKey, List<GameObject>>();

	// Token: 0x04000957 RID: 2391
	[Header("Road Tile Setup")]
	public List<PathFinder.StreetChunk> streetChunks = new List<PathFinder.StreetChunk>();

	// Token: 0x04000958 RID: 2392
	public List<StreetController.StreetTile> streetSections = new List<StreetController.StreetTile>();

	// Token: 0x04000959 RID: 2393
	public Dictionary<MeshRenderer, StreetTilePreset.StreetSectionModel> loadedModelReference = new Dictionary<MeshRenderer, StreetTilePreset.StreetSectionModel>();

	// Token: 0x0400095A RID: 2394
	public List<StreetController> sharedGroundElements = new List<StreetController>();

	// Token: 0x0400095B RID: 2395
	public List<string> debugAddressSet = new List<string>();

	// Token: 0x0200015A RID: 346
	[Serializable]
	public class StreetTile
	{
		// Token: 0x06000931 RID: 2353 RVA: 0x0009134E File Offset: 0x0008F54E
		public StreetTile(string chunkName, Vector3 newWorldPos, StreetTilePreset.StreetSection newSection, int newAngle)
		{
			this.name = chunkName;
			this.worldPos = newWorldPos;
			this.section = newSection;
			this.angle = newAngle;
		}

		// Token: 0x0400095C RID: 2396
		public string name;

		// Token: 0x0400095D RID: 2397
		public Vector3 worldPos;

		// Token: 0x0400095E RID: 2398
		public StreetTilePreset.StreetSection section;

		// Token: 0x0400095F RID: 2399
		public int angle;
	}
}
