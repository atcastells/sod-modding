using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000ED RID: 237
public class AirDuctGroup : Controller
{
	// Token: 0x060006CA RID: 1738 RVA: 0x00067ED8 File Offset: 0x000660D8
	public void SetupNew(NewBuilding newBuilding)
	{
		this.ductID = AirDuctGroup.assignID;
		AirDuctGroup.assignID++;
		base.transform.name = "Air Duct Group " + this.ductID.ToString();
		this.building = newBuilding;
		this.building.airDucts.Add(this);
		CityData.Instance.airDuctGroupDirectory.Add(this);
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00067F44 File Offset: 0x00066144
	public void Load(CitySaveData.AirDuctGroupCitySave load, NewBuilding newBuilding)
	{
		this.ductID = load.id;
		base.transform.name = "Air Duct Group " + this.ductID.ToString();
		this.building = newBuilding;
		this.isExterior = load.ext;
		this.building.airDucts.Add(this);
		CityData.Instance.airDuctGroupDirectory.Add(this);
		using (List<int>.Enumerator enumerator = load.airVents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int ventID = enumerator.Current;
				AirDuctGroup.AirVent airVent = CityData.Instance.airVentDirectory.Find((AirDuctGroup.AirVent item) => item.ventID == ventID);
				if (airVent == null)
				{
					Game.LogError(string.Concat(new string[]
					{
						"Cannot find vent with ID of ",
						ventID.ToString(),
						" from ",
						CityData.Instance.airVentDirectory.Count.ToString(),
						" loaded vents"
					}), 2);
				}
				else
				{
					airVent.group = this;
					this.AddAirVent(airVent);
				}
			}
		}
		foreach (CitySaveData.AirDuctSegmentCitySave airDuctSegmentCitySave in load.airDucts)
		{
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(airDuctSegmentCitySave.node, ref newNode))
			{
				new AirDuctGroup.AirDuctSection(airDuctSegmentCitySave.level, airDuctSegmentCitySave.index, airDuctSegmentCitySave.duct, airDuctSegmentCitySave.previous, airDuctSegmentCitySave.next, newNode, this, airDuctSegmentCitySave.peek, airDuctSegmentCitySave.addRot);
			}
		}
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x0006811C File Offset: 0x0006631C
	public void AddAirDuctSection(int level, Vector3Int duct, Vector3Int previous, Vector3Int next, NewNode newNode, int index = 0)
	{
		int num = this.airDucts.FindIndex((AirDuctGroup.AirDuctSection item) => item.level == level && item.node == newNode);
		if (num > -1)
		{
			this.airDucts[num].index = index;
			this.airDucts[num].previous = previous;
			this.airDucts[num].next = next;
			return;
		}
		new AirDuctGroup.AirDuctSection(Mathf.RoundToInt((float)level), index, duct, previous, next, newNode, this, false, Vector3Int.zero);
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x000681B8 File Offset: 0x000663B8
	public void AddAirVent(AirDuctGroup.AirVent newVent)
	{
		if (!this.airVents.Contains(newVent))
		{
			newVent.group = this;
			this.airVents.Add(newVent);
			if (!this.ventRooms.Contains(newVent.room))
			{
				this.ventRooms.Add(newVent.room);
			}
		}
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x0006820A File Offset: 0x0006640A
	public void AddAdjoiningDuctGroup(AirDuctGroup ductGroup)
	{
		if (!this.adjoiningGroups.Contains(ductGroup))
		{
			this.adjoiningGroups.Add(ductGroup);
		}
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00068228 File Offset: 0x00066428
	public void LoadDucts()
	{
		List<MeshFilter> list = new List<MeshFilter>();
		using (List<AirDuctGroup.AirDuctSection>.Enumerator enumerator = this.airDucts.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				AirDuctGroup.AirDuctSection duct = enumerator.Current;
				List<Vector3Int> list2;
				List<AirDuctGroup.AirVent> list3;
				List<Vector3Int> list4;
				duct.GetNeighborSections(out list2, out list3, out list4);
				list2.AddRange(list4);
				bool flag = false;
				string text = string.Empty;
				if (Game.Instance.collectDebugData)
				{
					foreach (Vector3Int vector3Int in list2)
					{
						text = text + " " + vector3Int.ToString();
					}
				}
				Predicate<AirDuctGroup.AirVent> <>9__0;
				foreach (InteriorControls.AirDuctOffset airDuctOffset in InteriorControls.Instance.airDuctModels)
				{
					if (airDuctOffset.offsets.Count == list2.Count)
					{
						bool flag2 = true;
						foreach (Vector3Int vector3Int2 in list2)
						{
							Vector3 vector = vector3Int2;
							if (!airDuctOffset.offsets.Contains(vector.normalized))
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							GameObject gameObject = airDuctOffset.prefabs[duct.index];
							if (CityConstructor.Instance.generateNew && gameObject == InteriorControls.Instance.ductStraightModel && ((!duct.node.room.isNullRoom && !duct.node.room.isBaseNullRoom && duct.level < 2) || (duct.node.room.IsOutside() && !duct.node.room.isOutsideWindow)))
							{
								string seedInput = duct.node.nodeCoord.ToString() + duct.node.room.roomID.ToString();
								if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seedInput, out seedInput) <= 0.5f)
								{
									duct.peekSection = true;
									duct.additionalRot = new Vector3Int(0, 0, Toolbox.Instance.GetPsuedoRandomNumberContained(-1, 2, seedInput, out seedInput) * 90);
								}
							}
							if (duct.peekSection)
							{
								gameObject = InteriorControls.Instance.ductStraightWithPeekVent;
								if (!this.ventRooms.Contains(duct.node.room))
								{
									this.ventRooms.Add(duct.node.room);
								}
							}
							GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, base.transform);
							gameObject2.transform.position = CityData.Instance.NodeToRealpos(duct.node.nodeCoord + new Vector3(0f, 0f, (float)duct.level * 2f + 1f) / 6f) + new Vector3(0f, InteriorControls.Instance.airDuctYOffset, 0f);
							gameObject2.transform.localEulerAngles = airDuctOffset.rotation + duct.additionalRot;
							gameObject2.name = string.Concat(new string[]
							{
								"L",
								duct.level.ToString(),
								" Duct:",
								text,
								" outside: ",
								duct.node.room.IsOutside().ToString()
							});
							list.Add(gameObject2.GetComponent<MeshFilter>());
							foreach (Vector3Int vector3Int3 in list2)
							{
								if (duct.level == 2 && vector3Int3.z == -1)
								{
									duct.node.SetCeilingVent(true);
								}
								else if (duct.level == 2 && vector3Int3.z == 1)
								{
									Vector3Int vector3Int4 = duct.node.nodeCoord + new Vector3Int(0, 0, 1);
									NewNode newNode = null;
									if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int4, ref newNode))
									{
										newNode.SetFloorVent(true);
									}
								}
								else if (duct.level == 0 && vector3Int3.z == 1)
								{
									duct.node.SetFloorVent(true);
								}
								else if (vector3Int3.z == 0)
								{
									Vector3Int vector3Int5 = duct.node.nodeCoord + vector3Int3;
									NewNode newNode2 = null;
									if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int5, ref newNode2))
									{
										foreach (NewWall newWall in duct.node.walls)
										{
											if (newWall.otherWall.node == newNode2)
											{
												if (duct.level == 2)
												{
													bool flag3 = false;
													bool flag4 = false;
													for (int i = -1; i >= -2; i--)
													{
														Vector3Int vector3Int6 = duct.duct + new Vector3Int(0, 0, i);
														AirDuctGroup.AirDuctSection airDuctSection = null;
														if (duct.node.building.ductMap.TryGetValue(vector3Int6, ref airDuctSection))
														{
															List<Vector3Int> list5;
															List<Vector3Int> list6;
															airDuctSection.GetNeighborSections(out list5, out list3, out list6);
															if (list5.Contains(vector3Int3) || list6.Contains(vector3Int3))
															{
																if (i == -1)
																{
																	flag3 = true;
																}
																else if (i == -2)
																{
																	flag4 = true;
																}
															}
														}
													}
													if (flag3)
													{
														newWall.SetDoorPairPreset(InteriorControls.Instance.wallVentUpperWithTopSpace, true, false, true);
														break;
													}
													if (flag4)
													{
														newWall.SetDoorPairPreset(InteriorControls.Instance.wallVentLowerWithTopSpace, true, false, true);
														break;
													}
													if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.wall)
													{
														newWall.SetDoorPairPreset(InteriorControls.Instance.wallVentTop, true, false, true);
														break;
													}
													break;
												}
												else if (duct.level == 1)
												{
													if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.wall)
													{
														newWall.SetDoorPairPreset(InteriorControls.Instance.wallVentUpper, true, false, true);
														break;
													}
													break;
												}
												else if (duct.level == 0)
												{
													if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.wall)
													{
														newWall.SetDoorPairPreset(InteriorControls.Instance.wallVentLower, true, false, true);
														break;
													}
													break;
												}
											}
										}
									}
								}
							}
							if (duct.level == 2)
							{
								List<AirDuctGroup.AirVent> list7 = duct.node.room.airVents;
								Predicate<AirDuctGroup.AirVent> predicate;
								if ((predicate = <>9__0) == null)
								{
									predicate = (<>9__0 = ((AirDuctGroup.AirVent item) => item.node == duct.node && item.ventType == NewAddress.AirVent.ceiling));
								}
								AirDuctGroup.AirVent airVent = list7.Find(predicate);
								if (airVent != null)
								{
									Vector3 vector2;
									vector2..ctor(0f, -1f, 0f);
									Vector3 vector3 = gameObject2.transform.position + vector2 * (PathFinder.Instance.nodeSize.x * 0.5f) + new Vector3(0f, 0.1f, 0f);
									Quaternion targetRotation = Quaternion.LookRotation(base.transform.InverseTransformDirection(vector2));
									Vector3 localEulerAtRotation = Toolbox.Instance.GetLocalEulerAtRotation(base.transform, targetRotation);
									Interactable interactable = InteractableCreator.Instance.CreateTransformInteractable(PrefabControls.Instance.airVent, base.transform, null, null, base.transform.InverseTransformPoint(vector3), localEulerAtRotation, null);
									if (interactable != null)
									{
										interactable.SetPolymorphicReference(this);
										interactable.LoadInteractableToWorld(false, true);
										Toolbox.Instance.SetLightLayer(interactable.spawnedObject, this.building, this.isExterior);
										if (interactable.controller != null)
										{
											interactable.controller.debugVent = airVent;
										}
										airVent.spawned = interactable.controller;
									}
									duct.node.room.AddDuctGroup(this);
								}
							}
							else
							{
								Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
								for (int j = 0; j < offsetArrayX.Length; j++)
								{
									Vector2Int vector2Int = offsetArrayX[j];
									Vector3Int vector3Int7;
									vector3Int7..ctor(vector2Int.x, vector2Int.y, 0);
									Vector3 vector4;
									vector4..ctor((float)vector2Int.x, 0f, (float)vector2Int.y);
									Vector3 vector5 = gameObject2.transform.position + vector4 * (PathFinder.Instance.nodeSize.x * 0.5f + 0.0011f);
									Vector3Int vector3Int8 = duct.node.nodeCoord + vector3Int7;
									NewNode foundNode = null;
									Quaternion targetRotation2 = Quaternion.LookRotation(base.transform.InverseTransformDirection(vector4));
									Vector3 localEulerAtRotation2 = Toolbox.Instance.GetLocalEulerAtRotation(base.transform, targetRotation2);
									if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int8, ref foundNode))
									{
										if (duct.level == 1)
										{
											AirDuctGroup.AirVent airVent2 = foundNode.room.airVents.Find((AirDuctGroup.AirVent item) => item.node == duct.node && item.roomNode == foundNode && item.ventType == NewAddress.AirVent.wallUpper);
											if (airVent2 != null)
											{
												Interactable interactable2 = InteractableCreator.Instance.CreateTransformInteractable(PrefabControls.Instance.airVent, base.transform, null, null, base.transform.InverseTransformPoint(vector5), localEulerAtRotation2, null);
												if (interactable2 != null)
												{
													interactable2.SetPolymorphicReference(this);
													interactable2.LoadInteractableToWorld(false, true);
													Toolbox.Instance.SetLightLayer(interactable2.spawnedObject, this.building, this.isExterior);
													if (interactable2.controller != null)
													{
														interactable2.controller.debugVent = airVent2;
													}
													airVent2.spawned = interactable2.controller;
												}
												foundNode.room.AddDuctGroup(this);
											}
										}
										else if (duct.level == 0)
										{
											AirDuctGroup.AirVent airVent3 = foundNode.room.airVents.Find((AirDuctGroup.AirVent item) => item.node == duct.node && item.roomNode == foundNode && item.ventType == NewAddress.AirVent.wallLower);
											if (airVent3 != null)
											{
												Interactable interactable3 = InteractableCreator.Instance.CreateTransformInteractable(PrefabControls.Instance.airVent, base.transform, null, null, base.transform.InverseTransformPoint(vector5), localEulerAtRotation2, null);
												if (interactable3 != null)
												{
													interactable3.SetPolymorphicReference(this);
													interactable3.LoadInteractableToWorld(false, true);
													Toolbox.Instance.SetLightLayer(interactable3.spawnedObject, this.building, this.isExterior);
													if (interactable3.controller != null)
													{
														interactable3.controller.debugVent = airVent3;
													}
													airVent3.spawned = interactable3.controller;
												}
												foundNode.room.AddDuctGroup(this);
											}
										}
									}
								}
							}
							if (!Game.Instance.combineAirDuctMeshes)
							{
								Toolbox.Instance.SetLightLayer(gameObject2, this.building, false);
							}
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					Game.LogError("Unable to find air duct model for offsets:" + text, 2);
				}
			}
		}
		if (Game.Instance.combineAirDuctMeshes)
		{
			base.transform.position = Vector3.zero;
			base.transform.eulerAngles = Vector3.zero;
			CombineInstance[] array = new CombineInstance[list.Count];
			int k = 0;
			while (k < list.Count)
			{
				if (!(list[k].transform == base.transform))
				{
					array[k].mesh = list[k].sharedMesh;
					array[k].transform = list[k].transform.localToWorldMatrix;
					Object.Destroy(list[k].gameObject);
					k++;
				}
			}
			this.meshFilter = base.gameObject.AddComponent<MeshFilter>();
			this.combinedMesh = base.gameObject.AddComponent<MeshRenderer>();
			MeshCollider meshCollider = base.gameObject.AddComponent<MeshCollider>();
			this.meshFilter.mesh = new Mesh();
			this.meshFilter.mesh.CombineMeshes(array, true, true);
			base.transform.gameObject.SetActive(true);
			base.gameObject.layer = 28;
			this.combinedMesh.sharedMaterial = InteriorControls.Instance.ductMaterial;
			this.combinedMesh.shadowCastingMode = Game.Instance.airDuctShadowMode;
			meshCollider.sharedMesh = this.meshFilter.sharedMesh;
			base.transform.localPosition = Vector3.zero;
			base.transform.localEulerAngles = Vector3.zero;
			Toolbox.Instance.SetLightLayer(this.combinedMesh, this.building, this.isExterior);
			LODGroup lodgroup = base.gameObject.AddComponent<LODGroup>();
			float airDuctLODThreshold = CullingControls.Instance.airDuctLODThreshold;
			Renderer[] array2 = new MeshRenderer[]
			{
				this.combinedMesh
			};
			LOD lod;
			lod..ctor(airDuctLODThreshold, array2);
			lodgroup.SetLODs(new LOD[]
			{
				lod
			});
		}
		this.SetVisible(false);
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00069054 File Offset: 0x00067254
	public void SetVisible(bool newVis)
	{
		if (this.isVisible != newVis)
		{
			this.isVisible = newVis;
			base.gameObject.SetActive(this.isVisible);
		}
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00069078 File Offset: 0x00067278
	public List<Vector3Int> GetDuctOffsets(NewNode thisNode, AirDuctGroup.AirDuctSection duct)
	{
		List<Vector3Int> list = new List<Vector3Int>();
		List<AirDuctGroup.AirDuctSection> list2 = new List<AirDuctGroup.AirDuctSection>();
		Predicate<AirDuctGroup.AirDuctSection> <>9__0;
		foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
		{
			Vector3Int vector3Int;
			vector3Int..ctor(vector2Int.x, vector2Int.y, 0);
			Vector3Int vector3Int2 = thisNode.nodeCoord + vector3Int;
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode))
			{
				List<AirDuctGroup.AirDuctSection> list3 = newNode.airDucts;
				Predicate<AirDuctGroup.AirDuctSection> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((AirDuctGroup.AirDuctSection item) => item.level == duct.level));
				}
				AirDuctGroup.AirDuctSection airDuctSection = list3.Find(predicate);
				if (airDuctSection != null)
				{
					list.Add(vector3Int);
					list2.Add(airDuctSection);
				}
			}
		}
		if (duct.level < 2)
		{
			AirDuctGroup.AirDuctSection airDuctSection2 = thisNode.airDucts.Find((AirDuctGroup.AirDuctSection item) => item.level == duct.level + 1);
			if (airDuctSection2 != null)
			{
				list.Add(new Vector3Int(0, 0, 1));
				list2.Add(airDuctSection2);
			}
		}
		else
		{
			Vector3Int vector3Int3;
			vector3Int3..ctor(0, 0, 1);
			Vector3Int vector3Int4 = thisNode.nodeCoord + vector3Int3;
			NewNode newNode2 = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int4, ref newNode2))
			{
				AirDuctGroup.AirDuctSection airDuctSection3 = newNode2.airDucts.Find((AirDuctGroup.AirDuctSection item) => item.level == 0);
				if (airDuctSection3 != null)
				{
					list.Add(vector3Int3);
					list2.Add(airDuctSection3);
				}
			}
		}
		if (duct.level > 0)
		{
			AirDuctGroup.AirDuctSection airDuctSection4 = thisNode.airDucts.Find((AirDuctGroup.AirDuctSection item) => item.level == duct.level - 1);
			if (airDuctSection4 != null)
			{
				list.Add(new Vector3Int(0, 0, -1));
				list2.Add(airDuctSection4);
			}
		}
		else
		{
			Vector3Int vector3Int5;
			vector3Int5..ctor(0, 0, -1);
			Vector3Int vector3Int6 = thisNode.nodeCoord + vector3Int5;
			NewNode newNode3 = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int6, ref newNode3))
			{
				AirDuctGroup.AirDuctSection airDuctSection5 = newNode3.airDucts.Find((AirDuctGroup.AirDuctSection item) => item.level == 2);
				if (airDuctSection5 != null)
				{
					list.Add(vector3Int5);
					list2.Add(airDuctSection5);
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			Vector3Int vector3Int7 = list[j];
			AirDuctGroup.AirDuctSection airDuctSection6 = list2[j];
			if (vector3Int7 != duct.previous && vector3Int7 != duct.next && airDuctSection6.previous != -vector3Int7 && airDuctSection6.next != -vector3Int7)
			{
				list.RemoveAt(j);
				list2.RemoveAt(j);
				j--;
			}
		}
		if (duct.level == 2)
		{
			if (thisNode.room.airVents.Find((AirDuctGroup.AirVent item) => item.node == thisNode && item.ventType == NewAddress.AirVent.ceiling) != null)
			{
				Vector3Int vector3Int8;
				vector3Int8..ctor(0, 0, -1);
				if (!list.Contains(vector3Int8))
				{
					list.Add(vector3Int8);
				}
			}
		}
		else
		{
			Predicate<AirDuctGroup.AirVent> <>9__6;
			foreach (Vector2Int vector2Int2 in CityData.Instance.offsetArrayX4)
			{
				Vector3Int vector3Int9;
				vector3Int9..ctor(vector2Int2.x, vector2Int2.y, 0);
				Vector3Int vector3Int10 = thisNode.nodeCoord + vector3Int9;
				NewNode newNode4 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int10, ref newNode4))
				{
					List<AirDuctGroup.AirVent> list4 = newNode4.room.airVents;
					Predicate<AirDuctGroup.AirVent> predicate2;
					if ((predicate2 = <>9__6) == null)
					{
						predicate2 = (<>9__6 = ((AirDuctGroup.AirVent item) => item.node == thisNode));
					}
					AirDuctGroup.AirVent airVent = list4.Find(predicate2);
					if (airVent != null)
					{
						if (airVent.ventType == NewAddress.AirVent.wallUpper && duct.level == 1 && !list.Contains(vector3Int9))
						{
							list.Add(vector3Int9);
						}
						if (airVent.ventType == NewAddress.AirVent.wallLower && duct.level == 0 && !list.Contains(vector3Int9))
						{
							list.Add(vector3Int9);
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x000694DC File Offset: 0x000676DC
	public CitySaveData.AirDuctGroupCitySave GenerateSaveData()
	{
		CitySaveData.AirDuctGroupCitySave airDuctGroupCitySave = new CitySaveData.AirDuctGroupCitySave();
		airDuctGroupCitySave.id = this.ductID;
		airDuctGroupCitySave.ext = this.isExterior;
		foreach (AirDuctGroup.AirVent airVent in this.airVents)
		{
			if (!airVent.removed)
			{
				airDuctGroupCitySave.airVents.Add(airVent.ventID);
			}
		}
		foreach (AirDuctGroup.AirDuctSection airDuctSection in this.airDucts)
		{
			CitySaveData.AirDuctSegmentCitySave airDuctSegmentCitySave = new CitySaveData.AirDuctSegmentCitySave();
			airDuctSegmentCitySave.index = airDuctSection.index;
			airDuctSegmentCitySave.level = airDuctSection.level;
			airDuctSegmentCitySave.next = airDuctSection.next;
			airDuctSegmentCitySave.node = airDuctSection.node.nodeCoord;
			airDuctSegmentCitySave.previous = airDuctSection.previous;
			airDuctSegmentCitySave.duct = airDuctSection.duct;
			airDuctSegmentCitySave.peek = airDuctSection.peekSection;
			airDuctSegmentCitySave.addRot = airDuctSection.additionalRot;
			airDuctGroupCitySave.airDucts.Add(airDuctSegmentCitySave);
		}
		foreach (AirDuctGroup airDuctGroup in this.adjoiningGroups)
		{
			airDuctGroupCitySave.adjoining.Add(airDuctGroup.ductID);
		}
		return airDuctGroupCitySave;
	}

	// Token: 0x040006DA RID: 1754
	public int ductID;

	// Token: 0x040006DB RID: 1755
	public static int assignID;

	// Token: 0x040006DC RID: 1756
	public NewBuilding building;

	// Token: 0x040006DD RID: 1757
	public bool isExterior;

	// Token: 0x040006DE RID: 1758
	public bool isVisible = true;

	// Token: 0x040006DF RID: 1759
	[Header("Air Vents")]
	public List<AirDuctGroup.AirVent> airVents = new List<AirDuctGroup.AirVent>();

	// Token: 0x040006E0 RID: 1760
	[Header("Air Ducts")]
	public List<AirDuctGroup.AirDuctSection> airDucts = new List<AirDuctGroup.AirDuctSection>();

	// Token: 0x040006E1 RID: 1761
	[Header("Combined Mesh")]
	public MeshFilter meshFilter;

	// Token: 0x040006E2 RID: 1762
	public MeshRenderer combinedMesh;

	// Token: 0x040006E3 RID: 1763
	[Header("Culling")]
	public List<AirDuctGroup> adjoiningGroups = new List<AirDuctGroup>();

	// Token: 0x040006E4 RID: 1764
	public List<NewRoom> ventRooms = new List<NewRoom>();

	// Token: 0x020000EE RID: 238
	[Serializable]
	public class AirVent
	{
		// Token: 0x060006D4 RID: 1748 RVA: 0x000696B7 File Offset: 0x000678B7
		public AirVent(NewAddress.AirVent newType, NewRoom newRoom)
		{
			this.ventID = AirDuctGroup.AirVent.assignVentID;
			AirDuctGroup.AirVent.assignVentID++;
			this.ventType = newType;
			this.room = newRoom;
			CityData.Instance.airVentDirectory.Add(this);
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x000696F4 File Offset: 0x000678F4
		public void SetDiscovered(bool val)
		{
			if (val != this.discovered)
			{
				this.discovered = val;
				if (this.room.explorationLevel < 1)
				{
					this.room.SetExplorationLevel(1);
				}
				if (this.mapButton != null)
				{
					MapController.Instance.AddDuctUpdateCall(this.mapButton);
				}
			}
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x00069749 File Offset: 0x00067949
		public AirVent(CitySaveData.AirVentSave load)
		{
			this.ventID = load.id;
			this.ventType = load.ventType;
			CityData.Instance.airVentDirectory.Add(this);
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0006977C File Offset: 0x0006797C
		public void Remove()
		{
			this.removed = true;
			Game.Log(string.Concat(new string[]
			{
				"CityGen: Removed air vent ",
				this.ventID.ToString(),
				" at ",
				this.room.name,
				" (",
				this.room.gameLocation.name,
				")"
			}), 2);
			if (this.ventType == NewAddress.AirVent.ceiling)
			{
				this.node.SetCeilingVent(false);
			}
			else if (this.ventType == NewAddress.AirVent.wallLower || this.ventType == NewAddress.AirVent.wallUpper)
			{
				this.wall.SetDoorPairPreset(InteriorControls.Instance.wallNormal, true, false, true);
			}
			if (this.group != null)
			{
				this.group.airVents.Remove(this);
			}
			this.room.airVents.Remove(this);
			CityData.Instance.airVentDirectory.Remove(this);
		}

		// Token: 0x040006E5 RID: 1765
		public int ventID;

		// Token: 0x040006E6 RID: 1766
		public static int assignVentID;

		// Token: 0x040006E7 RID: 1767
		public NewAddress.AirVent ventType;

		// Token: 0x040006E8 RID: 1768
		public NewWall wall;

		// Token: 0x040006E9 RID: 1769
		public NewNode node;

		// Token: 0x040006EA RID: 1770
		public NewNode roomNode;

		// Token: 0x040006EB RID: 1771
		public NewRoom room;

		// Token: 0x040006EC RID: 1772
		public AirDuctGroup group;

		// Token: 0x040006ED RID: 1773
		public MapDuctsButtonController mapButton;

		// Token: 0x040006EE RID: 1774
		public bool discovered;

		// Token: 0x040006EF RID: 1775
		public bool removed;

		// Token: 0x040006F0 RID: 1776
		public InteractableController spawned;

		// Token: 0x040006F1 RID: 1777
		public Vector3 debugNode;

		// Token: 0x040006F2 RID: 1778
		public Vector3 debugRoomNode;
	}

	// Token: 0x020000EF RID: 239
	[Serializable]
	public class AirDuctSection
	{
		// Token: 0x060006D8 RID: 1752 RVA: 0x00069874 File Offset: 0x00067A74
		public AirDuctSection(int newLevel, int newIndex, Vector3Int newDuct, Vector3Int newPrevious, Vector3Int newNext, NewNode newNode, AirDuctGroup newGroup, bool newPeek, Vector3Int newAdditionalRot)
		{
			this.level = newLevel;
			this.index = newIndex;
			this.duct = newDuct;
			this.previous = newPrevious;
			this.next = newNext;
			this.node = newNode;
			this.group = newGroup;
			this.ext = this.node.room.IsOutside();
			this.peekSection = newPeek;
			this.additionalRot = newAdditionalRot;
			this.group.airDucts.Add(this);
			this.node.airDucts.Add(this);
			if (newGroup != null && !this.node.room.isNullRoom)
			{
				this.node.room.AddDuctGroup(newGroup);
			}
			this.node.building.ductMap.Add(this.duct, this);
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0006995C File Offset: 0x00067B5C
		public void SetDiscovered(bool val)
		{
			if (val != this.discovered)
			{
				this.discovered = val;
				if (this.mapButton != null)
				{
					MapController.Instance.AddDuctUpdateCall(this.mapButton);
				}
				int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.moneyForDucts));
				if (this.discovered && num > 0)
				{
					GameplayController.Instance.AddMoney(num, false, "moneyforducts");
				}
			}
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x000699C8 File Offset: 0x00067BC8
		public List<AirDuctGroup.AirDuctSection> GetNeighborSections(out List<Vector3Int> relativeOffsets, out List<AirDuctGroup.AirVent> vents, out List<Vector3Int> ventRelativeOffsets)
		{
			relativeOffsets = new List<Vector3Int>();
			vents = new List<AirDuctGroup.AirVent>();
			ventRelativeOffsets = new List<Vector3Int>();
			List<AirDuctGroup.AirDuctSection> list = new List<AirDuctGroup.AirDuctSection>();
			foreach (Vector3Int vector3Int in CityData.Instance.offsetArrayX6)
			{
				Vector3Int vector3Int2 = this.duct + vector3Int;
				AirDuctGroup.AirDuctSection airDuctSection;
				if (this.node.building.ductMap.TryGetValue(vector3Int2, ref airDuctSection) && (vector3Int == this.next || vector3Int == this.previous || airDuctSection.next == -vector3Int || airDuctSection.previous == -vector3Int) && !list.Contains(airDuctSection))
				{
					list.Add(airDuctSection);
					relativeOffsets.Add(vector3Int);
				}
			}
			if (this.level == 2)
			{
				AirDuctGroup.AirVent airVent = this.node.room.airVents.Find((AirDuctGroup.AirVent item) => item.node == this.node && item.ventType == NewAddress.AirVent.ceiling);
				if (airVent != null && !vents.Contains(airVent))
				{
					vents.Add(airVent);
					Vector3Int vector3Int3;
					vector3Int3..ctor(0, 0, -1);
					ventRelativeOffsets.Add(vector3Int3);
				}
			}
			else
			{
				Vector2Int[] offsetArrayX2 = CityData.Instance.offsetArrayX4;
				for (int i = 0; i < offsetArrayX2.Length; i++)
				{
					Vector2Int vector2Int = offsetArrayX2[i];
					Vector3Int vector3Int4;
					vector3Int4..ctor(vector2Int.x, vector2Int.y, 0);
					Vector3Int vector3Int5 = this.node.nodeCoord + vector3Int4;
					NewNode foundNode;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int5, ref foundNode))
					{
						AirDuctGroup.AirVent airVent2 = foundNode.room.airVents.Find((AirDuctGroup.AirVent item) => item.node == this.node && item.roomNode == foundNode);
						if (airVent2 != null)
						{
							if (airVent2.ventType == NewAddress.AirVent.wallUpper && this.level == 1)
							{
								if (!vents.Contains(airVent2))
								{
									vents.Add(airVent2);
									ventRelativeOffsets.Add(vector3Int4);
								}
							}
							else if (airVent2.ventType == NewAddress.AirVent.wallLower && this.level == 0 && !vents.Contains(airVent2))
							{
								vents.Add(airVent2);
								ventRelativeOffsets.Add(vector3Int4);
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x040006F3 RID: 1779
		public int level;

		// Token: 0x040006F4 RID: 1780
		public int index;

		// Token: 0x040006F5 RID: 1781
		public Vector3Int duct;

		// Token: 0x040006F6 RID: 1782
		public Vector3Int previous;

		// Token: 0x040006F7 RID: 1783
		public Vector3Int next;

		// Token: 0x040006F8 RID: 1784
		public bool ext;

		// Token: 0x040006F9 RID: 1785
		public bool peekSection;

		// Token: 0x040006FA RID: 1786
		public Vector3Int additionalRot = Vector3Int.zero;

		// Token: 0x040006FB RID: 1787
		public NewNode node;

		// Token: 0x040006FC RID: 1788
		public AirDuctGroup group;

		// Token: 0x040006FD RID: 1789
		public MapDuctsButtonController mapButton;

		// Token: 0x040006FE RID: 1790
		public bool discovered;
	}
}
