using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020002CF RID: 719
public class GeometryCullingController : MonoBehaviour
{
	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06001071 RID: 4209 RVA: 0x000E7DF7 File Offset: 0x000E5FF7
	public static GeometryCullingController Instance
	{
		get
		{
			return GeometryCullingController._instance;
		}
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x000E7DFE File Offset: 0x000E5FFE
	private void Awake()
	{
		if (GeometryCullingController._instance != null && GeometryCullingController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GeometryCullingController._instance = this;
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x000E7E2C File Offset: 0x000E602C
	public void UpdateCullingForRoom(NewRoom currentRoom, bool updateSound, bool inAirVent, AirDuctGroup currentDuct, bool immediateLoad = false)
	{
		if (updateSound)
		{
			AudioController.Instance.UpdateAllLoopingSoundOcclusion();
		}
		this.currentRoomsCullingTree.Clear();
		this.currentDuctsCullingTree.Clear();
		if (!SessionData.Instance.isFloorEdit && !SessionData.Instance.isTestScene)
		{
			foreach (MurderController.Murder murder in MurderController.Instance.activeMurders)
			{
				foreach (int num in murder.cullingActiveRooms)
				{
					NewRoom newRoom = null;
					if (CityData.Instance.roomDictionary.TryGetValue(num, ref newRoom))
					{
						this.currentRoomsCullingTree.Add(newRoom);
					}
				}
			}
			foreach (Human human in GameplayController.Instance.activeRagdolls)
			{
				if (!this.currentRoomsCullingTree.Contains(human.currentRoom))
				{
					this.currentRoomsCullingTree.Add(human.currentRoom);
				}
				foreach (NewRoom newRoom2 in human.currentRoom.adjacentRooms)
				{
					if (!this.currentRoomsCullingTree.Contains(newRoom2))
					{
						this.currentRoomsCullingTree.Add(newRoom2);
					}
				}
			}
			foreach (Interactable interactable in GameplayController.Instance.activePhysics)
			{
				if (interactable.node == null)
				{
					interactable.UpdateWorldPositionAndNode(false);
					if (interactable.node == null)
					{
						continue;
					}
				}
				if (!this.currentRoomsCullingTree.Contains(interactable.node.room))
				{
					this.currentRoomsCullingTree.Add(interactable.node.room);
				}
				foreach (NewRoom newRoom3 in interactable.node.room.adjacentRooms)
				{
					if (!this.currentRoomsCullingTree.Contains(newRoom3))
					{
						this.currentRoomsCullingTree.Add(newRoom3);
					}
				}
			}
			if (inAirVent)
			{
				if (currentDuct == null)
				{
					Game.Log("Player: Duct culling failed: No current duct", 2);
					return;
				}
				Game.Log(string.Concat(new string[]
				{
					"Player: Update culling for duct ",
					currentDuct.ductID.ToString(),
					" with ",
					currentDuct.ventRooms.Count.ToString(),
					" vent rooms..."
				}), 2);
				this.currentDuctsCullingTree.Add(currentDuct);
				if (Player.Instance.currentRoom != null && !this.currentRoomsCullingTree.Contains(Player.Instance.currentRoom))
				{
					this.currentRoomsCullingTree.Add(Player.Instance.currentRoom);
				}
				foreach (NewRoom newRoom4 in currentDuct.ventRooms)
				{
					if (!this.currentRoomsCullingTree.Contains(newRoom4))
					{
						Vector3 middleRoomPosition = newRoom4.middleRoomPosition;
						if (Vector3.Distance(Player.Instance.transform.position, middleRoomPosition) < CullingControls.Instance.ductRoomCullingRange || newRoom4.IsOutside())
						{
							this.currentRoomsCullingTree.Add(newRoom4);
							foreach (KeyValuePair<NewRoom, List<NewRoom.CullTreeEntry>> keyValuePair in newRoom4.cullingTree)
							{
								if (!(keyValuePair.Key == null) && (!(keyValuePair.Key.building == Player.Instance.currentBuilding) || Vector3.Distance(Player.Instance.transform.position, middleRoomPosition) <= PathFinder.Instance.tileSize.z + 1f))
								{
									if (!this.currentRoomsCullingTree.Contains(keyValuePair.Key))
									{
										foreach (NewRoom.CullTreeEntry cullTreeEntry in keyValuePair.Value)
										{
											if (cullTreeEntry.requiredOpenDoors == null || cullTreeEntry.requiredOpenDoors.Count <= 0)
											{
												this.currentRoomsCullingTree.Add(keyValuePair.Key);
											}
											else
											{
												bool flag = true;
												foreach (int num2 in cullTreeEntry.requiredOpenDoors)
												{
													NewDoor newDoor = null;
													if (CityData.Instance.doorDictionary.TryGetValue(num2, ref newDoor))
													{
														if (newDoor.isClosed && !newDoor.peekedUnder)
														{
															flag = false;
															break;
														}
													}
													else
													{
														Game.LogError("Cannot find door at wall " + num2.ToString(), 2);
													}
												}
												if (flag)
												{
													this.currentRoomsCullingTree.Add(keyValuePair.Key);
												}
											}
										}
									}
									foreach (AirDuctGroup airDuctGroup in keyValuePair.Key.ductGroups)
									{
										if (!this.currentDuctsCullingTree.Contains(airDuctGroup))
										{
											this.currentDuctsCullingTree.Add(airDuctGroup);
										}
									}
								}
							}
						}
					}
				}
				using (List<AirDuctGroup>.Enumerator enumerator9 = currentDuct.adjoiningGroups.GetEnumerator())
				{
					while (enumerator9.MoveNext())
					{
						AirDuctGroup airDuctGroup2 = enumerator9.Current;
						this.currentDuctsCullingTree.Add(airDuctGroup2);
					}
					goto IL_B47;
				}
			}
			if (currentRoom == null)
			{
				Game.Log("Player: room culling failed: No current room", 2);
				return;
			}
			if (Game.Instance.enableNewRealtimeTimeCullingSystem)
			{
				this.GenerateDynamicCulling(currentRoom, 0);
			}
			else
			{
				if (!currentRoom.completedTreeCull & !currentRoom.loadedCullTreeFromSave)
				{
					currentRoom.GenerateCullingTree(false);
				}
				if (currentRoom.isNullRoom && currentRoom.cullingTree.Count <= 1 && !SessionData.Instance.isTestScene)
				{
					if (!currentRoom.IsOutside())
					{
						Game.Log("Player: Player is in null room; " + currentRoom.name + " don't update culling tree for " + currentRoom.name, 2);
						return;
					}
					Game.Log("Player: Player is in outside window room, attempt to use nearest street room instead...", 2);
					NewNode nearestGroundLevelOutside = Toolbox.Instance.GetNearestGroundLevelOutside(Player.Instance.transform.position);
					if (nearestGroundLevelOutside != null)
					{
						currentRoom = nearestGroundLevelOutside.room;
						Game.Log("Player: Found replacement closest street room: " + currentRoom.gameLocation.name, 2);
					}
					else
					{
						Game.Log("Player: Failed to find closest street room...", 2);
					}
				}
				if (currentRoom.cullingTree.Count <= 0 && !SessionData.Instance.isTestScene)
				{
					Game.Log("Player: Culling tree of " + currentRoom.name + " is zero! There is probably something wrong here...", 2);
				}
				foreach (KeyValuePair<NewRoom, List<NewRoom.CullTreeEntry>> keyValuePair2 in currentRoom.cullingTree)
				{
					if (!(keyValuePair2.Key == null))
					{
						if (!this.currentRoomsCullingTree.Contains(keyValuePair2.Key))
						{
							foreach (NewRoom.CullTreeEntry cullTreeEntry2 in keyValuePair2.Value)
							{
								if (cullTreeEntry2.requiredOpenDoors == null || cullTreeEntry2.requiredOpenDoors.Count <= 0)
								{
									this.currentRoomsCullingTree.Add(keyValuePair2.Key);
									if (keyValuePair2.Key.atriumTop != null && !this.currentRoomsCullingTree.Contains(keyValuePair2.Key.atriumTop))
									{
										this.currentRoomsCullingTree.Add(keyValuePair2.Key.atriumTop);
									}
									foreach (NewRoom newRoom5 in keyValuePair2.Key.atriumRooms)
									{
										if (!this.currentRoomsCullingTree.Contains(newRoom5))
										{
											this.currentRoomsCullingTree.Add(newRoom5);
										}
									}
									if (!(keyValuePair2.Key.gameLocation.thisAsStreet != null))
									{
										continue;
									}
									using (List<StreetController>.Enumerator enumerator10 = keyValuePair2.Key.gameLocation.thisAsStreet.sharedGroundElements.GetEnumerator())
									{
										while (enumerator10.MoveNext())
										{
											StreetController streetController = enumerator10.Current;
											foreach (NewRoom newRoom6 in streetController.rooms)
											{
												if (!this.currentRoomsCullingTree.Contains(newRoom6))
												{
													this.currentRoomsCullingTree.Add(newRoom6);
												}
											}
										}
										continue;
									}
								}
								bool flag2 = true;
								foreach (int num3 in cullTreeEntry2.requiredOpenDoors)
								{
									NewDoor newDoor2 = null;
									if (CityData.Instance.doorDictionary.TryGetValue(num3, ref newDoor2))
									{
										if (newDoor2.isClosed && !newDoor2.peekedUnder)
										{
											flag2 = false;
											break;
										}
									}
									else
									{
										Game.LogError("Cannot find door at wall " + num3.ToString(), 2);
									}
								}
								if (flag2)
								{
									this.currentRoomsCullingTree.Add(keyValuePair2.Key);
									if (keyValuePair2.Key.atriumTop != null && !this.currentRoomsCullingTree.Contains(keyValuePair2.Key.atriumTop))
									{
										this.currentRoomsCullingTree.Add(keyValuePair2.Key.atriumTop);
									}
									foreach (NewRoom newRoom7 in keyValuePair2.Key.atriumRooms)
									{
										if (!this.currentRoomsCullingTree.Contains(newRoom7))
										{
											this.currentRoomsCullingTree.Add(newRoom7);
										}
									}
									if (keyValuePair2.Key.gameLocation.thisAsStreet != null)
									{
										foreach (StreetController streetController2 in keyValuePair2.Key.gameLocation.thisAsStreet.sharedGroundElements)
										{
											foreach (NewRoom newRoom8 in streetController2.rooms)
											{
												if (!this.currentRoomsCullingTree.Contains(newRoom8))
												{
													this.currentRoomsCullingTree.Add(newRoom8);
												}
											}
										}
									}
								}
							}
						}
						foreach (AirDuctGroup airDuctGroup3 in keyValuePair2.Key.ductGroups)
						{
							if (!this.currentDuctsCullingTree.Contains(airDuctGroup3))
							{
								this.currentDuctsCullingTree.Add(airDuctGroup3);
							}
						}
					}
				}
			}
			IL_B47:
			using (List<NewBuilding>.Enumerator enumerator11 = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.GetEnumerator())
			{
				while (enumerator11.MoveNext())
				{
					NewBuilding newBuilding = enumerator11.Current;
					if (newBuilding.displayBuildingModel && Vector3.Distance(currentRoom.middleRoomPosition, newBuilding.transform.position) < CullingControls.Instance.exteriorDuctCullingRange)
					{
						foreach (AirDuctGroup airDuctGroup4 in newBuilding.airDucts)
						{
							if (airDuctGroup4.isExterior && !this.currentDuctsCullingTree.Contains(airDuctGroup4))
							{
								this.currentDuctsCullingTree.Add(airDuctGroup4);
							}
						}
					}
				}
				goto IL_CA1;
			}
		}
		if (FloorEditController.Instance != null && FloorEditController.Instance.editFloor != null)
		{
			foreach (NewAddress newAddress in FloorEditController.Instance.editFloor.addresses)
			{
				foreach (NewRoom newRoom9 in newAddress.rooms)
				{
					this.currentRoomsCullingTree.Add(newRoom9);
				}
			}
		}
		IL_CA1:
		this.ExecuteCurrentCullingTree(immediateLoad);
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x000E8D58 File Offset: 0x000E6F58
	public void ExecuteCurrentCullingTree(bool immediateLoad)
	{
		this.transformSyncRequired = false;
		for (int i = 0; i < GameplayController.Instance.roomsVicinity.Count; i++)
		{
			NewRoom newRoom = GameplayController.Instance.roomsVicinity[i];
			if (newRoom == null)
			{
				GameplayController.Instance.roomsVicinity.RemoveAt(i);
				i--;
			}
			else if (this.currentRoomsCullingTree.Contains(newRoom))
			{
				if (!newRoom.isVisible)
				{
					this.transformSyncRequired = true;
				}
				newRoom.SetVisible(true, false, "Tree by ", immediateLoad);
				this.currentRoomsCullingTree.Remove(newRoom);
			}
			else
			{
				newRoom.SetVisible(false, false, "Lacking in tree by ", immediateLoad);
				GameplayController.Instance.roomsVicinity.RemoveAt(i);
				i--;
			}
		}
		try
		{
			foreach (NewRoom newRoom2 in this.currentRoomsCullingTree)
			{
				if (!newRoom2.isVisible)
				{
					this.transformSyncRequired = true;
				}
				newRoom2.SetVisible(true, false, "Tree", immediateLoad);
				GameplayController.Instance.roomsVicinity.Add(newRoom2);
			}
		}
		catch
		{
		}
		if (Game.Instance.collectDebugData)
		{
			this.debugCurrentRoomsVisible = new List<NewRoom>(GameplayController.Instance.roomsVicinity);
		}
		for (int j = 0; j < GameplayController.Instance.ductsVicinity.Count; j++)
		{
			AirDuctGroup airDuctGroup = GameplayController.Instance.ductsVicinity[j];
			if (airDuctGroup == null)
			{
				GameplayController.Instance.ductsVicinity.RemoveAt(j);
				j--;
			}
			else if (this.currentDuctsCullingTree.Contains(airDuctGroup))
			{
				airDuctGroup.SetVisible(true);
				this.currentDuctsCullingTree.Remove(airDuctGroup);
			}
			else
			{
				airDuctGroup.SetVisible(false);
				GameplayController.Instance.ductsVicinity.RemoveAt(j);
				j--;
			}
		}
		foreach (AirDuctGroup airDuctGroup2 in this.currentDuctsCullingTree)
		{
			airDuctGroup2.SetVisible(true);
			GameplayController.Instance.ductsVicinity.Add(airDuctGroup2);
		}
		if (this.transformSyncRequired)
		{
			Physics.SyncTransforms();
		}
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x000E8FBC File Offset: 0x000E71BC
	public void GenerateDynamicCulling(NewRoom forRoom, int displayDebugLevel = 0)
	{
		if (forRoom == null)
		{
			return;
		}
		if (displayDebugLevel > 0)
		{
			this.debugRayCommands.Clear();
		}
		List<GeometryCullingController.CullingTreeData> list = new List<GeometryCullingController.CullingTreeData>();
		HashSet<NewRoom> hashSet = new HashSet<NewRoom>();
		HashSet<NewNode.NodeAccess> hashSet2 = new HashSet<NewNode.NodeAccess>();
		if (!forRoom.calculatedWorldPos)
		{
			forRoom.UpdateWorldPositionAndBoundsSize();
		}
		Vector3 worldPos = forRoom.worldPos;
		list.Add(new GeometryCullingController.CullingTreeData
		{
			room = forRoom,
			pathPoint = worldPos,
			pathDistance = 0f
		});
		bool flag = true;
		int num = this.maximumLoopCount;
		Color colour = Color.white;
		int num2 = 0;
		while (list.Count > 0 && num > 0)
		{
			list.Sort((GeometryCullingController.CullingTreeData p1, GeometryCullingController.CullingTreeData p2) => p1.pathDistance.CompareTo(p2.pathDistance));
			GeometryCullingController.CullingTreeData cullingTreeData = list[0];
			NewRoom room = cullingTreeData.room;
			if (displayDebugLevel >= 1)
			{
				Game.Log(string.Concat(new string[]
				{
					"Culling: --- Open set reaches ",
					room.GetName(),
					" with ",
					room.entrances.Count.ToString(),
					" entrances ---"
				}), 2);
			}
			if (!this.currentRoomsCullingTree.Contains(room))
			{
				this.currentRoomsCullingTree.Add(room);
			}
			foreach (NewNode.NodeAccess nodeAccess in room.entrances)
			{
				if (cullingTreeData.accessCount <= 0 || !hashSet2.Contains(nodeAccess))
				{
					NewRoom otherRoom = nodeAccess.GetOtherRoom(room);
					if (!this.currentRoomsCullingTree.Contains(otherRoom) && !hashSet.Contains(otherRoom) && this.IsAccessValid(room, otherRoom, nodeAccess, displayDebugLevel))
					{
						if (this.IsRoomRenderableFromOrigin(forRoom, otherRoom, displayDebugLevel))
						{
							if (this.IsRoomRenderableFromThisRoom(room, forRoom, otherRoom, nodeAccess, displayDebugLevel))
							{
								float num3 = Vector3.Distance(worldPos, nodeAccess.worldAccessPoint);
								if (num3 <= this.maxDistance)
								{
									float num4 = Vector3.Angle(nodeAccess.worldAccessPoint - cullingTreeData.initialAccessPoint, cullingTreeData.initialAccessFoward);
									Vector2 vector;
									vector..ctor(cullingTreeData.angleThresholds.x, cullingTreeData.angleThresholds.y);
									if (cullingTreeData.accessCount == 1)
									{
										Vector3 worldAccessPoint = nodeAccess.worldAccessPoint;
										Vector3 worldAccessPoint2 = nodeAccess.worldAccessPoint;
										nodeAccess.GetEntranceSidePoints(out worldAccessPoint, out worldAccessPoint2);
										Vector3 vector2 = worldAccessPoint - cullingTreeData.initialAccessPoint;
										Vector3 vector3 = worldAccessPoint2 - cullingTreeData.initialAccessPoint;
										float num5 = Vector3.Angle(vector2, cullingTreeData.initialAccessFoward);
										float num6 = Vector3.Angle(vector3, cullingTreeData.initialAccessFoward);
										vector..ctor(Mathf.Max(num5, cullingTreeData.angleThresholds.x), Mathf.Min(num6, cullingTreeData.angleThresholds.y));
										if (displayDebugLevel >= 2)
										{
											Vector3 direction = worldAccessPoint + new Vector3(0.01f, 2f, 0f) - (worldAccessPoint + new Vector3(0.01f, 1f, 0f));
											this.QueueDrawRay(worldAccessPoint + new Vector3(0.01f, 1f, 0f), direction, Color.blue, this.rayTime, 0f);
											direction = worldAccessPoint2 + new Vector3(0f, 2f, 0f) - (worldAccessPoint2 + new Vector3(0f, 1f, 0f));
											this.QueueDrawRay(worldAccessPoint2 + new Vector3(0f, 1f, 0f), direction, Color.blue, this.rayTime, 0f);
											this.QueueDrawRay(cullingTreeData.initialAccessPoint + new Vector3(0f, 1f, 0f), vector2, new Color(1f, 0.92f, 0.016f, 0.4f), this.rayTime * 0.5f, 0f);
											this.QueueDrawRay(cullingTreeData.initialAccessPoint + new Vector3(0f, 1f, 0f), vector3, new Color(1f, 0.92f, 0.016f, 0.4f), this.rayTime * 0.5f, 0f);
										}
									}
									if (displayDebugLevel > 0)
									{
										colour = Color.Lerp(Color.white, Color.green, (float)cullingTreeData.accessCount * 0.25f);
									}
									if (flag || (num4 >= vector.x && num4 <= vector.y))
									{
										GeometryCullingController.CullingTreeData cullingTreeData2 = new GeometryCullingController.CullingTreeData
										{
											room = otherRoom,
											pathPoint = nodeAccess.worldAccessPoint,
											pathDistance = num3,
											initialAccessPoint = cullingTreeData.initialAccessPoint,
											initialAccessFoward = cullingTreeData.initialAccessFoward,
											angleThresholds = vector,
											accessCount = cullingTreeData.accessCount + 1
										};
										if (flag)
										{
											cullingTreeData2.initialAccessPoint = nodeAccess.worldAccessPoint;
											cullingTreeData2.accessCount = 1;
											cullingTreeData2.angleThresholds = new Vector2(-this.maxAngleAtMinDistance, this.maxAngleAtMinDistance);
											NewNode newNode = nodeAccess.toNode;
											if (newNode.room == room)
											{
												newNode = nodeAccess.fromNode;
											}
											cullingTreeData2.initialAccessFoward = (newNode.position - nodeAccess.worldAccessPoint).normalized;
											if (displayDebugLevel > 0)
											{
												Vector3 direction2 = nodeAccess.worldAccessPoint + new Vector3(0f, 1f, 0f) - (forRoom.worldPos + new Vector3(0f, 1f, 0f));
												this.QueueDrawRay(forRoom.worldPos + new Vector3(0f, 1f, 0f), direction2, Color.white, this.rayTime, 0f);
											}
										}
										list.Add(cullingTreeData2);
										if (displayDebugLevel > 0)
										{
											Vector3 direction3 = nodeAccess.worldAccessPoint + new Vector3(0f, 1f, 0f) - (cullingTreeData.pathPoint + new Vector3(0f, 1f, 0f));
											this.QueueDrawRay(cullingTreeData.pathPoint + new Vector3(0f, 1f, 0f), direction3, colour, this.rayTime, this.rayDelay);
										}
										if (displayDebugLevel >= 2)
										{
										}
									}
									else if (displayDebugLevel > 0)
									{
										Vector3 direction4 = nodeAccess.worldAccessPoint + new Vector3(0f, 1f, 0f) - (cullingTreeData.pathPoint + new Vector3(0f, 1f, 0f));
										this.QueueDrawRay(cullingTreeData.pathPoint + new Vector3(0f, 1f, 0f), direction4, Color.red, this.rayTime, this.rayDelay);
									}
								}
							}
						}
						else if (!hashSet.Contains(otherRoom))
						{
							if (displayDebugLevel >= 2)
							{
								Game.Log("Culling: Room " + otherRoom.GetName() + " is unrenderable from " + forRoom.GetName(), 2);
							}
							hashSet.Add(otherRoom);
						}
					}
					else if (displayDebugLevel >= 2)
					{
						Game.Log(string.Concat(new string[]
						{
							"Culling: Entrance in ",
							forRoom.GetName(),
							": ",
							nodeAccess.fromNode.room.GetName(),
							" > ",
							nodeAccess.toNode.room.GetName(),
							" is invalid"
						}), 2);
					}
					if (!hashSet2.Contains(nodeAccess))
					{
						hashSet2.Add(nodeAccess);
					}
				}
			}
			flag = false;
			list.RemoveAt(0);
			num--;
			num2++;
			if (num <= 0)
			{
				Game.Log("Culling: Realtime culling failsafe is reached!", 2);
			}
		}
		if (displayDebugLevel > 0)
		{
			Game.Log("Culling: Culling complete! Realtime culling used " + num2.ToString() + " loops out of " + this.maximumLoopCount.ToString(), 2);
		}
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x0008FFAC File Offset: 0x0008E1AC
	private bool IsRoomRenderableFromOrigin(NewRoom startingRoom, NewRoom destinationRoom, int displayDebugLevel)
	{
		return true;
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x000E9800 File Offset: 0x000E7A00
	private bool IsRoomRenderableFromThisRoom(NewRoom adjacentRoom, NewRoom originRoom, NewRoom destinationRoom, NewNode.NodeAccess access, int displayDebugLevel)
	{
		if (adjacentRoom == null || originRoom == null || destinationRoom == null)
		{
			return true;
		}
		bool flag = Enumerable.FirstOrDefault<NewNode>(adjacentRoom.nodes) != null;
		NewNode newNode = Enumerable.FirstOrDefault<NewNode>(originRoom.nodes);
		NewNode newNode2 = Enumerable.FirstOrDefault<NewNode>(destinationRoom.nodes);
		if (!flag || newNode2 == null)
		{
			return true;
		}
		bool flag2 = adjacentRoom.IsOutside();
		originRoom.IsOutside();
		bool flag3 = destinationRoom.IsOutside();
		if (flag2 && !flag3 && (access.accessType == NewNode.NodeAccess.AccessType.window || access.accessType == NewNode.NodeAccess.AccessType.door) && newNode.nodeCoord.z != newNode2.nodeCoord.z)
		{
			if (displayDebugLevel > 0)
			{
				Game.Log(string.Concat(new string[]
				{
					"Trying to reach indoor room ",
					destinationRoom.GetName(),
					" from outdoor room ",
					adjacentRoom.GetName(),
					" through ",
					access.accessType.ToString(),
					": Return false"
				}), 2);
			}
			return false;
		}
		return true;
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x000E9900 File Offset: 0x000E7B00
	private bool IsAccessValid(NewRoom currentRoom, NewRoom destinationRoom, NewNode.NodeAccess access, int displayDebugLevel)
	{
		if (currentRoom.isOutsideWindow && !destinationRoom.IsOutside())
		{
			if (displayDebugLevel > 0)
			{
				Game.Log(string.Concat(new string[]
				{
					"Trying to reach indoor room ",
					destinationRoom.GetName(),
					" from outside window ",
					currentRoom.GetName(),
					": Return false"
				}), 2);
			}
			return false;
		}
		if (this.doorsBlockSight && access.door != null && access.door.isClosed && (access.door.preset == null || !access.door.preset.isTransparent))
		{
			if (displayDebugLevel > 0)
			{
				Game.Log(string.Concat(new string[]
				{
					"Trying to reach ray through door from ",
					currentRoom.GetName(),
					" to ",
					destinationRoom.GetName(),
					": Return false"
				}), 2);
			}
			return false;
		}
		return true;
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x000E99ED File Offset: 0x000E7BED
	[Button(null, 0)]
	public void DebugDynamicCulling()
	{
		this.GenerateDynamicCulling(Player.Instance.currentRoom, this.debugLevel);
		this.ExecuteCurrentCullingTree(true);
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x000E9A0C File Offset: 0x000E7C0C
	private void QueueDrawRay(Vector3 origin, Vector3 direction, Color colour, float duration, float delay)
	{
		this.debugRayCommands.Add(new GeometryCullingController.DebugCullingRayCommands
		{
			start = origin,
			dir = direction,
			color = colour,
			duration = duration,
			delay = delay
		});
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x000E9A44 File Offset: 0x000E7C44
	private void Update()
	{
		if (CutSceneController.Instance.cutSceneActive)
		{
			return;
		}
		if (this.toCalculateList.Count > 0 && !this.asyncCullingTreeActive)
		{
			NewRoom newRoom = this.toCalculateList[0];
			if (newRoom != null)
			{
				if (!newRoom.completedTreeCull)
				{
					this.ProcessCullingTreeForRoom(newRoom);
				}
				else
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("Culling: Culling tree already generated for " + newRoom.GetName(), 2);
					}
					this.toCalculateList.RemoveAt(0);
				}
			}
			else
			{
				this.toCalculateList.RemoveAt(0);
			}
		}
		if (!Game.Instance.collectDebugData && this.toCalculateList.Count <= 0 && !this.asyncCullingTreeActive)
		{
			base.enabled = false;
		}
		while (this.debugRayCommands.Count > 0)
		{
			GeometryCullingController.DebugCullingRayCommands debugCullingRayCommands = this.debugRayCommands[0];
			if (debugCullingRayCommands.delay > 0f && this.animateDrawDebugRays)
			{
				debugCullingRayCommands.delay -= Time.deltaTime;
				return;
			}
			Debug.DrawRay(debugCullingRayCommands.start, debugCullingRayCommands.dir, debugCullingRayCommands.color, debugCullingRayCommands.duration);
			this.debugRayCommands.RemoveAt(0);
		}
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x000E9B70 File Offset: 0x000E7D70
	public void OnStartGame()
	{
		if (Game.Instance.generateCullingInGame)
		{
			this.toCalculateList.AddRange(CityData.Instance.roomDirectory);
			if (Game.Instance.printDebug)
			{
				Game.Log("Culling: Starting in-game culling tree generation for " + this.toCalculateList.Count.ToString() + " rooms...", 2);
			}
			base.enabled = true;
		}
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x000E9BDC File Offset: 0x000E7DDC
	public void ProcessCullingTreeForRoom(NewRoom room)
	{
		GeometryCullingController.<ProcessCullingTreeForRoom>d__32 <ProcessCullingTreeForRoom>d__;
		<ProcessCullingTreeForRoom>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ProcessCullingTreeForRoom>d__.<>4__this = this;
		<ProcessCullingTreeForRoom>d__.room = room;
		<ProcessCullingTreeForRoom>d__.<>1__state = -1;
		<ProcessCullingTreeForRoom>d__.<>t__builder.Start<GeometryCullingController.<ProcessCullingTreeForRoom>d__32>(ref <ProcessCullingTreeForRoom>d__);
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x000E9C1B File Offset: 0x000E7E1B
	public Task TaskedCullTreeGeneration(NewRoom room, bool debugMode)
	{
		return Task.Run(delegate()
		{
			room.GenerateCullingTree(debugMode);
		});
	}

	// Token: 0x0400147C RID: 5244
	[Header("Settings")]
	public float maxDistance = 100f;

	// Token: 0x0400147D RID: 5245
	[Tooltip("The max FoV angle calculated from the first entrance in the room loop")]
	[Range(0f, 100f)]
	public float maxAngleAtMinDistance = 90f;

	// Token: 0x0400147E RID: 5246
	[Range(0f, 100f)]
	public float maxAngleAtMaxDistance = 1f;

	// Token: 0x0400147F RID: 5247
	public int maximumLoopCount = 3000;

	// Token: 0x04001480 RID: 5248
	[Tooltip("If true, with realtime culling doors will blocks sight but the culling will have to be updated when doors open")]
	public bool doorsBlockSight = true;

	// Token: 0x04001481 RID: 5249
	[Header("State")]
	public HashSet<NewRoom> currentRoomsCullingTree = new HashSet<NewRoom>();

	// Token: 0x04001482 RID: 5250
	private HashSet<AirDuctGroup> currentDuctsCullingTree = new HashSet<AirDuctGroup>();

	// Token: 0x04001483 RID: 5251
	public bool transformSyncRequired;

	// Token: 0x04001484 RID: 5252
	[Range(0f, 2f)]
	[Header("Debugging")]
	public int debugLevel = 2;

	// Token: 0x04001485 RID: 5253
	public bool animateDrawDebugRays = true;

	// Token: 0x04001486 RID: 5254
	[EnableIf("animateDrawDebugRays")]
	public float rayDelay = 0.01f;

	// Token: 0x04001487 RID: 5255
	public float rayTime = 6f;

	// Token: 0x04001488 RID: 5256
	public List<NewRoom> debugCurrentRoomsVisible = new List<NewRoom>();

	// Token: 0x04001489 RID: 5257
	private List<GeometryCullingController.DebugCullingRayCommands> debugRayCommands = new List<GeometryCullingController.DebugCullingRayCommands>();

	// Token: 0x0400148A RID: 5258
	[InfoBox("The above is mostly deprecated, but the below relates to calculating the room culling trees while running the game using asyncronous methods", 0)]
	[Header("Realtime Culling")]
	[Tooltip("List of rooms set to be calculated using async methods")]
	public List<NewRoom> toCalculateList = new List<NewRoom>();

	// Token: 0x0400148B RID: 5259
	public bool asyncCullingTreeActive;

	// Token: 0x0400148C RID: 5260
	private static GeometryCullingController _instance;

	// Token: 0x020002D0 RID: 720
	public class DebugCullingRayCommands
	{
		// Token: 0x0400148D RID: 5261
		public Vector3 start;

		// Token: 0x0400148E RID: 5262
		public Vector3 dir;

		// Token: 0x0400148F RID: 5263
		public Color color;

		// Token: 0x04001490 RID: 5264
		public float duration;

		// Token: 0x04001491 RID: 5265
		public float delay;
	}

	// Token: 0x020002D1 RID: 721
	public class CullingTreeData
	{
		// Token: 0x04001492 RID: 5266
		public NewRoom room;

		// Token: 0x04001493 RID: 5267
		public Vector3 initialAccessPoint;

		// Token: 0x04001494 RID: 5268
		public Vector3 initialAccessFoward;

		// Token: 0x04001495 RID: 5269
		public Vector3 pathPoint;

		// Token: 0x04001496 RID: 5270
		public float pathDistance;

		// Token: 0x04001497 RID: 5271
		public int accessCount;

		// Token: 0x04001498 RID: 5272
		public Vector2 angleThresholds;
	}
}
