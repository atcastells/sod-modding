using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class NewGameLocation : Controller
{
	// Token: 0x06000870 RID: 2160 RVA: 0x00080DE4 File Offset: 0x0007EFE4
	public void CommonSetup(bool newIsOutside, DistrictController newDistrict, DesignStylePreset newDefaultStyle)
	{
		this.district = newDistrict;
		this.isOutside = newIsOutside;
		this.thisAsAddress = (this as NewAddress);
		this.thisAsStreet = (this as StreetController);
		if (newDefaultStyle != null)
		{
			this.SetDesignStyle(newDefaultStyle);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.room, base.transform);
		this.nullRoom = gameObject.GetComponent<NewRoom>();
		this.nullRoom.SetupAll(this, CityControls.Instance.nullDefaultRoom, -1);
		this.nullRoom.isBaseNullRoom = true;
		this.seed = Toolbox.Instance.SeedRand(0, 9999999).ToString();
		if (!CityData.Instance.gameLocationDirectory.Contains(this))
		{
			CityData.Instance.gameLocationDirectory.Add(this);
		}
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00080EAC File Offset: 0x0007F0AC
	public void AddNewNode(NewNode newNode)
	{
		if (!this.nodes.Contains(newNode))
		{
			if (newNode.gameLocation != null)
			{
				newNode.gameLocation.RemoveNode(newNode);
			}
			this.nodes.Add(newNode);
			newNode.gameLocation = this;
			this.nullRoom.AddNewNode(newNode);
			if (this.thisAsAddress != null)
			{
				newNode.floor = this.thisAsAddress.floor;
				newNode.building = this.thisAsAddress.building;
				if (SessionData.Instance.isFloorEdit)
				{
					GenerationController.Instance.UpdateGeometryFloor(this.thisAsAddress.floor, "NewGameLocation");
					if (FloorEditController.Instance.displayMode == FloorEditController.EditorDisplayMode.displayAddressDesignation && newNode.spawnedFloor != null)
					{
						newNode.spawnedFloor.GetComponent<MeshRenderer>().material = FloorEditController.Instance.adddressDesignationMaterial;
						newNode.spawnedFloor.GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", this.thisAsAddress.editorColour);
					}
				}
			}
		}
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00080FB6 File Offset: 0x0007F1B6
	public void RemoveNode(NewNode newNode)
	{
		this.nodes.Remove(newNode);
		newNode.gameLocation = null;
		newNode.floor = null;
		newNode.building = null;
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00080FDC File Offset: 0x0007F1DC
	public void AddNewRoom(NewRoom newRoom)
	{
		if (!this.rooms.Contains(newRoom))
		{
			if (newRoom.gameLocation != null)
			{
				newRoom.gameLocation.RemoveRoom(newRoom);
			}
			this.rooms.Add(newRoom);
			newRoom.gameLocation = this;
			if (this.thisAsAddress != null)
			{
				newRoom.floor = this.thisAsAddress.floor;
				newRoom.building = this.thisAsAddress.building;
			}
			foreach (NewNode newNode in newRoom.nodes)
			{
				newNode.gameLocation = this;
			}
		}
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0008109C File Offset: 0x0007F29C
	public void RemoveRoom(NewRoom newRoom)
	{
		if (this.rooms.Contains(newRoom))
		{
			this.rooms.Remove(newRoom);
			newRoom.gameLocation = null;
			newRoom.floor = null;
			newRoom.building = null;
			Object.Destroy(newRoom.gameObject);
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x000810DC File Offset: 0x0007F2DC
	public virtual void AddOccupant(Actor newOcc)
	{
		this.currentOccupants.Add(newOcc);
		if (this.thisAsAddress != null && this.thisAsAddress.company != null && !this.thisAsAddress.company.preset.isSelfEmployed)
		{
			this.thisAsAddress.company.OnAddressCitizenEnter(newOcc as Citizen);
		}
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0008113D File Offset: 0x0007F33D
	public virtual void RemoveOccupant(Actor remOcc)
	{
		this.currentOccupants.Remove(remOcc);
		if (this.thisAsAddress != null && this.thisAsAddress.company != null)
		{
			this.thisAsAddress.company.OnAddressCitizenExit(remOcc as Citizen);
		}
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00081180 File Offset: 0x0007F380
	public NewNode.NodeAccess GetMainEntrance()
	{
		NewNode.NodeAccess result = null;
		foreach (NewNode.NodeAccess nodeAccess in this.entrances)
		{
			if (nodeAccess.walkingAccess && (nodeAccess.accessType == NewNode.NodeAccess.AccessType.door || nodeAccess.accessType == NewNode.NodeAccess.AccessType.openDoorway) && !(nodeAccess.fromNode.gameLocation == nodeAccess.toNode.gameLocation))
			{
				if (nodeAccess.toNode.gameLocation.thisAsStreet != null || nodeAccess.fromNode.gameLocation.thisAsStreet != null)
				{
					return nodeAccess;
				}
				result = nodeAccess;
			}
		}
		return result;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x00081240 File Offset: 0x0007F440
	public void SetDesignStyle(DesignStylePreset newStyle)
	{
		this.designStyle = newStyle;
		if (this.thisAsAddress != null && (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew || SessionData.Instance.play))
		{
			this.thisAsAddress.wood = InteriorControls.Instance.woods[Toolbox.Instance.GetPsuedoRandomNumber(0, InteriorControls.Instance.woods.Count, this.thisAsAddress.id.ToString(), false)];
			foreach (NewRoom newRoom in this.rooms)
			{
				if (newRoom.preset != null && newRoom.preset.decorSetting == RoomConfiguration.DecorSetting.ownStyle)
				{
					newRoom.UpdateColourSchemeAndMaterials();
				}
			}
			foreach (NewRoom newRoom2 in this.rooms)
			{
				if (newRoom2.preset != null && newRoom2.preset.decorSetting != RoomConfiguration.DecorSetting.ownStyle)
				{
					newRoom2.UpdateColourSchemeAndMaterials();
				}
			}
		}
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x00081390 File Offset: 0x0007F590
	public void AddEntrance(NewNode fromNode, NewNode toNode, bool forceAccessType = false, NewNode.NodeAccess.AccessType forcedAccessType = NewNode.NodeAccess.AccessType.adjacent, bool forceWalkable = false)
	{
		if (!this.entrances.Exists((NewNode.NodeAccess item) => item.fromNode == fromNode && item.toNode == toNode))
		{
			NewDoor newDoorway = null;
			NewWall newWall = fromNode.walls.Find((NewWall item) => item.otherWall != null && item.otherWall.node == toNode && item.parentWall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall && item.parentWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventLower && item.parentWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventUpper && item.preset.sectionClass != DoorPairPreset.WallSectionClass.ventTop);
			if (newWall != null)
			{
				newDoorway = newWall.parentWall.door;
				newWall = newWall.parentWall;
			}
			NewNode.NodeAccess nodeAccess = new NewNode.NodeAccess(fromNode, toNode, newWall, newDoorway, forceAccessType, forcedAccessType, forceWalkable);
			this.entrances.Add(nodeAccess);
			fromNode.room.AddEntrance(fromNode, toNode, false, NewNode.NodeAccess.AccessType.adjacent, false);
			if (nodeAccess.fromNode.gameLocation.thisAsStreet != null || nodeAccess.toNode.gameLocation.thisAsStreet != null)
			{
				this.streetAccess = nodeAccess;
			}
			if (this.thisAsStreet != null && toNode.gameLocation.thisAsAddress != null && !PathFinder.Instance.streetEntrances.Exists((NewNode.NodeAccess item) => item.fromNode == fromNode && item.toNode == toNode))
			{
				PathFinder.Instance.streetEntrances.Add(nodeAccess);
			}
		}
		if (this.anchorNode == null)
		{
			if (fromNode.gameLocation == this)
			{
				this.anchorNode = fromNode;
				return;
			}
			if (toNode.gameLocation == this)
			{
				this.anchorNode = toNode;
			}
		}
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x00081518 File Offset: 0x0007F718
	public void RemoveEntrance(NewNode fromNode, NewNode toNode)
	{
		if (this.thisAsStreet && toNode.gameLocation.thisAsAddress != null)
		{
			NewNode.NodeAccess nodeAccess = PathFinder.Instance.streetEntrances.Find((NewNode.NodeAccess item) => item.fromNode == fromNode && item.toNode == toNode);
			PathFinder.Instance.streetEntrances.Remove(nodeAccess);
		}
		NewNode.NodeAccess nodeAccess2 = this.entrances.Find((NewNode.NodeAccess item) => item.fromNode == fromNode && item.toNode == toNode);
		this.entrances.Remove(nodeAccess2);
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x000815B0 File Offset: 0x0007F7B0
	public Interactable PlaceObject(InteractablePreset interactable, Human belongsTo, Human writer, Human reciever, out FurnitureLocation pickedFurn, bool passVariable = false, Interactable.PassedVarType passedVarType = Interactable.PassedVarType.jobID, int passedValue = -1, bool forceSecuritySettings = false, int forcedSecurity = 0, InteractablePreset.OwnedPlacementRule forcedOwnership = InteractablePreset.OwnedPlacementRule.nonOwnedOnly, int forcedPriority = 0, RetailItemPreset retailItem = null, bool printDebug = false, HashSet<NewRoom> dontPlaceInRooms = null, string loadGUID = null, NewNode placeClosestTo = null, string ddsOverride = "", bool ignoreLimits = false)
	{
		List<Interactable.Passed> list = null;
		pickedFurn = null;
		if (passVariable)
		{
			list = new List<Interactable.Passed>();
			list.Add(new Interactable.Passed(passedVarType, (float)passedValue, null));
		}
		return this.PlaceObject(interactable, belongsTo, writer, reciever, out pickedFurn, list, forceSecuritySettings, forcedSecurity, forcedOwnership, forcedPriority, retailItem, printDebug, dontPlaceInRooms, loadGUID, placeClosestTo, ddsOverride, ignoreLimits);
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00081604 File Offset: 0x0007F804
	public Interactable PlaceObject(InteractablePreset interactable, Human belongsTo, Human writer, Human receiver, out FurnitureLocation pickedFurn, List<Interactable.Passed> passedVars = null, bool forceSecuritySettings = false, int forcedSecurity = 0, InteractablePreset.OwnedPlacementRule forcedOwnership = InteractablePreset.OwnedPlacementRule.nonOwnedOnly, int forcedPriority = 0, object passedObject = null, bool printDebug = false, HashSet<NewRoom> dontPlaceInRooms = null, string loadGUID = null, NewNode placeClosestTo = null, string ddsOverride = "", bool ignoreLimits = false)
	{
		string seedInput = this.seed;
		if (ddsOverride.Length > 0)
		{
			if (passedVars == null)
			{
				passedVars = new List<Interactable.Passed>();
			}
			passedVars.Add(new Interactable.Passed(Interactable.PassedVarType.ddsOverride, 0f, ddsOverride));
		}
		pickedFurn = null;
		if (!forceSecuritySettings)
		{
			forcedSecurity = interactable.securityLevel;
			forcedOwnership = interactable.ownedRule;
			forcedPriority = Mathf.Max(interactable.perGameLocationObjectPriority, interactable.perOwnerObjectPriority);
		}
		if (!this.objectPoolPlaced && CityConstructor.Instance.generateNew)
		{
			Game.Log("Object: Object pool has not yet been placed, adding to that instead...", 2);
			this.AddToPlacementPool(interactable, belongsTo, writer, receiver, passedVars, forcedSecurity, forcedOwnership, forcedPriority, passedObject, dontPlaceInRooms);
			return null;
		}
		if (printDebug)
		{
			Game.Log(string.Concat(new string[]
			{
				"Object: Placing item ",
				interactable.name,
				" with security: ",
				forcedSecurity.ToString(),
				", ownership: ",
				forcedOwnership.ToString(),
				" and priority: ",
				forcedPriority.ToString(),
				" belonging to ",
				belongsTo.name
			}), 2);
		}
		bool warmItem = false;
		RetailItemPreset retailItemPreset = passedObject as RetailItemPreset;
		if (retailItemPreset != null && retailItemPreset.isHot)
		{
			float value = passedVars.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.time).value;
			if (SessionData.Instance.gameTime - value <= GameplayControls.Instance.foodHotTime)
			{
				warmItem = true;
			}
		}
		if (!ignoreLimits && (interactable.limitPerAddress || interactable.limitInCommercial || interactable.limitInResidential))
		{
			int num = 0;
			int num2 = 99999;
			if (interactable.limitPerAddress && this.thisAsAddress != null)
			{
				num2 = Mathf.Min(interactable.perAddressLimit, num2);
			}
			if (interactable.limitInCommercial && this.thisAsAddress != null && this.thisAsAddress.company != null)
			{
				num2 = Mathf.Min(interactable.perCommercialLimit, num2);
			}
			if (interactable.limitInResidential && this.thisAsAddress != null && this.thisAsAddress.residence != null)
			{
				num2 = Mathf.Min(interactable.perResidentialLimit, num2);
			}
			Predicate<Interactable> <>9__1;
			foreach (NewRoom newRoom in this.rooms)
			{
				foreach (FurnitureLocation furnitureLocation in newRoom.individualFurniture)
				{
					int num3 = num;
					List<Interactable> spawnedInteractables = furnitureLocation.spawnedInteractables;
					Predicate<Interactable> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = ((Interactable item) => item.preset == interactable));
					}
					num = num3 + spawnedInteractables.FindAll(predicate).Count;
					if (num >= num2)
					{
						break;
					}
				}
				if (num >= num2)
				{
					break;
				}
			}
			if (num >= num2)
			{
				if (printDebug)
				{
					Game.Log(string.Concat(new string[]
					{
						"Object: ",
						base.name,
						" has maximum number of ",
						interactable.name,
						" objects according to interactable limit setting."
					}), 2);
				}
				return null;
			}
		}
		if (interactable.attemptToStoreInFolder != null && Toolbox.Instance.RandContained(0f, 1f, seedInput, out seedInput) <= interactable.folderPlacementChance)
		{
			bool flag = false;
			Interactable interactable2 = null;
			Predicate<Interactable> <>9__2;
			Predicate<Interactable> <>9__3;
			foreach (NewRoom newRoom2 in this.rooms)
			{
				foreach (FurnitureLocation furnitureLocation2 in newRoom2.individualFurniture)
				{
					if (interactable.folderOwnershipMustMatch)
					{
						List<Interactable> spawnedInteractables2 = furnitureLocation2.spawnedInteractables;
						Predicate<Interactable> predicate2;
						if ((predicate2 = <>9__2) == null)
						{
							predicate2 = (<>9__2 = ((Interactable item) => item.evidence != null && item.evidence.preset == interactable.attemptToStoreInFolder && item.belongsTo == belongsTo));
						}
						interactable2 = spawnedInteractables2.Find(predicate2);
					}
					else
					{
						List<Interactable> spawnedInteractables3 = furnitureLocation2.spawnedInteractables;
						Predicate<Interactable> predicate3;
						if ((predicate3 = <>9__3) == null)
						{
							predicate3 = (<>9__3 = ((Interactable item) => item.evidence != null && item.evidence.preset == interactable.attemptToStoreInFolder));
						}
						interactable2 = spawnedInteractables3.Find(predicate3);
					}
					if (interactable2 != null)
					{
						EvidenceMultiPage evidenceMultiPage = interactable2.evidence as EvidenceMultiPage;
						if (evidenceMultiPage != null)
						{
							MetaObject containedMetaObject = new MetaObject(interactable, belongsTo, writer, receiver, passedVars);
							evidenceMultiPage.AddContainedMetaObjectToNewPage(containedMetaObject);
						}
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				return interactable2;
			}
			Game.Log(string.Concat(new string[]
			{
				"CityGen: Could not find ",
				interactable.attemptToStoreInFolder.name,
				" for placing ",
				interactable.name,
				" at ",
				base.name
			}), 2);
			if (!flag && interactable.dontPlaceIfNoFolder)
			{
				return null;
			}
		}
		NewGameLocation.ObjectPlacement bestSpawnLocation = this.GetBestSpawnLocation(interactable, warmItem, belongsTo, writer, receiver, out pickedFurn, passedVars, forceSecuritySettings, forcedSecurity, forcedOwnership, forcedPriority, passedObject, printDebug, dontPlaceInRooms, loadGUID, placeClosestTo, ddsOverride, ignoreLimits, false);
		if (bestSpawnLocation == null)
		{
			if (printDebug)
			{
				Game.Log("Object: No furniture allows " + interactable.name + " to spawn in " + base.name, 2);
			}
			return null;
		}
		if (printDebug)
		{
			Game.Log("Object: Chose placement on " + bestSpawnLocation.furnParent.furniture.name, 2);
		}
		if (bestSpawnLocation.existing != null)
		{
			bestSpawnLocation.existing.SafeDelete(true);
		}
		Interactable interactable3;
		if (bestSpawnLocation.subSpawn == null)
		{
			if (!bestSpawnLocation.furnParent.createdInteractables)
			{
				bestSpawnLocation.furnParent.CreateInteractables();
			}
			if (printDebug)
			{
				Game.Log("Object: Chose placement on " + bestSpawnLocation.furnParent.furniture.name, 2);
			}
			pickedFurn = bestSpawnLocation.furnParent;
			interactable3 = InteractableCreator.Instance.CreateFurnitureSpawnedInteractableThreadSafe(interactable, bestSpawnLocation.furnParent.anchorNode.room, bestSpawnLocation.furnParent, bestSpawnLocation.location, belongsTo, writer, receiver, passedVars, null, passedObject, "");
		}
		else
		{
			if (!bestSpawnLocation.subSpawn.furnitureParent.createdInteractables)
			{
				bestSpawnLocation.subSpawn.furnitureParent.CreateInteractables();
			}
			if (printDebug)
			{
				Game.Log("Object: Chose placement on subspawned " + bestSpawnLocation.subSpawn.preset.name, 2);
			}
			int num4 = Toolbox.Instance.RandContained(0, bestSpawnLocation.subSpawn.ssp.Count, seedInput, out seedInput);
			InteractablePreset.SubSpawnSlot subSpawnSlot = bestSpawnLocation.subSpawn.ssp[num4];
			Vector2 vector = Random.insideUnitCircle * 0.2f;
			Vector3 vector2 = bestSpawnLocation.subSpawn.wPos + subSpawnSlot.localPos;
			vector2 += new Vector3(vector.x, 0f, vector.y);
			Vector3 worldEuler = bestSpawnLocation.subSpawn.wEuler + subSpawnSlot.localEuler;
			interactable3 = InteractableCreator.Instance.CreateWorldInteractable(interactable, belongsTo, writer, receiver, vector2, worldEuler, passedVars, passedObject, "");
			bestSpawnLocation.subSpawn.ssp.RemoveAt(num4);
		}
		if (interactable3 != null)
		{
			interactable3.opp = forcedPriority;
		}
		return interactable3;
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x00081E38 File Offset: 0x00080038
	public NewGameLocation.ObjectPlacement GetBestSpawnLocation(InteractablePreset interactable, bool warmItem, Human belongsTo, Human writer, Human receiver, out FurnitureLocation pickedFurn, List<Interactable.Passed> passedVars = null, bool forceSecuritySettings = false, int forcedSecurity = 0, InteractablePreset.OwnedPlacementRule forcedOwnership = InteractablePreset.OwnedPlacementRule.nonOwnedOnly, int forcedPriority = 0, object passedObject = null, bool printDebug = false, HashSet<NewRoom> dontPlaceInRooms = null, string loadGUID = null, NewNode placeClosestTo = null, string ddsOverride = "", bool ignoreLimits = false, bool usePutDownPosition = false)
	{
		pickedFurn = null;
		string seedInput = this.seed + interactable.presetName;
		NewGameLocation.ObjectPlacement result = null;
		float num = -99999f;
		foreach (NewRoom newRoom in this.rooms)
		{
			if (!(newRoom.preset == null) && (ignoreLimits || dontPlaceInRooms == null || !dontPlaceInRooms.Contains(newRoom)) && (ignoreLimits || newRoom.preset.allowPersonalAffects || !(belongsTo != null) || forcedOwnership != InteractablePreset.OwnedPlacementRule.ownedOnly))
			{
				if (interactable.limitToCertainRooms && !ignoreLimits)
				{
					if (!interactable.onlyInRooms.Contains(newRoom.preset))
					{
						if (printDebug)
						{
							Game.Log("Object: Interactable banned from rooms of type " + newRoom.preset.name + "...", 2);
							continue;
						}
						continue;
					}
				}
				else if (interactable.banFromRooms.Contains(newRoom.preset) && !ignoreLimits)
				{
					if (printDebug)
					{
						Game.Log("Object: Interactable banned from rooms of type " + newRoom.preset.name + "...", 2);
						continue;
					}
					continue;
				}
				if (!ignoreLimits && interactable.limitPerRoom)
				{
					int num2 = 0;
					using (List<FurnitureLocation>.Enumerator enumerator2 = newRoom.individualFurniture.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							FurnitureLocation f = enumerator2.Current;
							num2 += f.spawnedInteractables.FindAll((Interactable item) => item.preset == interactable && item.furnitureParent == f).Count;
							if (num2 >= interactable.perRoomLimit)
							{
								break;
							}
						}
					}
					if (num2 >= interactable.perRoomLimit)
					{
						if (printDebug)
						{
							Game.Log("Object: Limit per room hit in " + newRoom.preset.name + "...", 2);
							continue;
						}
						continue;
					}
				}
				if (printDebug)
				{
					Game.Log("Object: Checking for placement in " + newRoom.preset.name + "...", 2);
				}
				foreach (FurnitureClusterLocation furnitureClusterLocation in newRoom.furniture)
				{
					using (List<FurnitureLocation>.Enumerator enumerator2 = furnitureClusterLocation.clusterList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							FurnitureLocation obj = enumerator2.Current;
							if (obj.furniture == null)
							{
								Game.Log(string.Concat(new string[]
								{
									"Misc Error: Object ",
									furnitureClusterLocation.cluster.name,
									" index ",
									furnitureClusterLocation.clusterList.IndexOf(obj).ToString(),
									" doesn't have a furniture preset associated!"
								}), 2);
							}
							else
							{
								if (printDebug)
								{
									Game.Log("Object: Checking for placement on furniture " + obj.furniture.name, 2);
								}
								if (ignoreLimits || !interactable.limitPerObject || obj.spawnedInteractables.FindAll((Interactable item) => item.preset == interactable && item.furnitureParent == obj).Count < interactable.perObjectLimit)
								{
									List<FurniturePreset.SubObject> list = new List<FurniturePreset.SubObject>();
									List<SubObjectClassPreset> list2 = interactable.subObjectClasses;
									if (usePutDownPosition)
									{
										list2 = interactable.putDownPositions;
									}
									using (List<SubObjectClassPreset>.Enumerator enumerator4 = list2.GetEnumerator())
									{
										while (enumerator4.MoveNext())
										{
											SubObjectClassPreset placeClass = enumerator4.Current;
											if (warmItem && placeClass == InteriorControls.Instance.fridge)
											{
												if (printDebug)
												{
													Game.Log("Object: Can't place warm item in fridge...", 2);
												}
											}
											else
											{
												List<FurniturePreset.SubObject> list3 = obj.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == placeClass);
												if (list3.Count > 0)
												{
													if (usePutDownPosition)
													{
														list.AddRange(list3);
													}
													else
													{
														if (printDebug)
														{
															Game.Log(string.Concat(new string[]
															{
																"Object: Found matching classes on ",
																obj.furniture.name,
																" (",
																list3.Count.ToString(),
																" potential points matching class ",
																placeClass.name,
																")"
															}), 2);
														}
														list.AddRange(list3);
													}
												}
												else if (printDebug)
												{
													Game.Log("Object: Cannot find any points of class " + placeClass.name + " on furniture " + obj.furniture.name, 2);
												}
											}
										}
									}
									if (!usePutDownPosition)
									{
										using (List<SubObjectClassPreset>.Enumerator enumerator4 = interactable.backupClasses.GetEnumerator())
										{
											while (enumerator4.MoveNext())
											{
												SubObjectClassPreset placeClass = enumerator4.Current;
												List<FurniturePreset.SubObject> list4 = obj.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == placeClass);
												if (list4.Count > 0)
												{
													if (printDebug)
													{
														Game.Log(string.Concat(new string[]
														{
															"Object: Found matching backup classes on ",
															obj.furniture.name,
															" (",
															list4.Count.ToString(),
															" potential points matching class ",
															placeClass.name,
															")"
														}), 2);
													}
													using (List<FurniturePreset.SubObject>.Enumerator enumerator5 = list4.GetEnumerator())
													{
														while (enumerator5.MoveNext())
														{
															FurniturePreset.SubObject s = enumerator5.Current;
															bool flag = obj.spawnedInteractables.Find((Interactable item) => item.subObject == s) != null;
															float num3 = (float)(-80 - Toolbox.Instance.RandContained(0, 10, seedInput, out seedInput));
															if (flag)
															{
																num3 -= 100f;
															}
															if (num3 > num)
															{
																result = new NewGameLocation.ObjectPlacement
																{
																	location = s,
																	furnParent = obj
																};
																num = num3;
															}
														}
														continue;
													}
												}
												if (printDebug)
												{
													Game.Log("Object: Cannot find any points of backup class " + placeClass.name + " on furniture " + obj.furniture.name, 2);
												}
											}
											goto IL_8BD;
										}
										goto IL_6F8;
									}
									goto IL_6F8;
									IL_8BD:
									if (list.Count > 0)
									{
										using (List<FurniturePreset.SubObject>.Enumerator enumerator5 = list.GetEnumerator())
										{
											while (enumerator5.MoveNext())
											{
												FurniturePreset.SubObject loc = enumerator5.Current;
												float num4 = 10f + Toolbox.Instance.RandContained(-0.1f, 0.1f, seedInput, out seedInput);
												int num5 = newRoom.preset.securityLevel + loc.security;
												if (num5 == forcedSecurity)
												{
													num4 += 10f;
												}
												else if (num5 > forcedSecurity)
												{
													num4 += (float)(10 - Mathf.Abs(num5 - forcedSecurity) * 2);
												}
												else
												{
													num4 += (float)(10 - Mathf.Abs(num5 - forcedSecurity) * 4);
												}
												Interactable interactable2 = obj.spawnedInteractables.Find((Interactable item) => item.subObject == loc);
												if (interactable2 != null)
												{
													if (interactable2.opp > forcedPriority)
													{
														if (printDebug)
														{
															string[] array = new string[7];
															array[0] = "Object: Existing item priority is higher (";
															array[1] = interactable2.name;
															array[2] = " ";
															int num6 = 3;
															int opp = interactable2.opp;
															array[num6] = opp.ToString();
															array[4] = ") than the item to be placed (";
															array[5] = forcedPriority.ToString();
															array[6] = ") skipping this option...";
															Game.Log(string.Concat(array), 2);
															continue;
														}
														continue;
													}
													else
													{
														num4 -= 100f;
													}
												}
												if (forcedOwnership == InteractablePreset.OwnedPlacementRule.both)
												{
													num4 += 10f;
												}
												else if (forcedOwnership == InteractablePreset.OwnedPlacementRule.ownedOnly)
												{
													if (belongsTo == null || loc.belongsTo == FurniturePreset.SubObjectOwnership.nobody)
													{
														continue;
													}
													if (loc.belongsTo == FurniturePreset.SubObjectOwnership.everybody)
													{
														if (!obj.ownerMap.ContainsKey(new FurnitureLocation.OwnerKey(belongsTo)))
														{
															if (belongsTo.home == null)
															{
																continue;
															}
															if (!obj.ownerMap.ContainsKey(new FurnitureLocation.OwnerKey(belongsTo.home)))
															{
																continue;
															}
														}
													}
													else
													{
														int num7 = loc.belongsTo - FurniturePreset.SubObjectOwnership.person0;
														Human human = null;
														NewAddress newAddress = null;
														foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in obj.ownerMap)
														{
															if (keyValuePair.Key.human != null)
															{
																if (keyValuePair.Value == num7)
																{
																	human = keyValuePair.Key.human;
																	break;
																}
															}
															else if (keyValuePair.Key.address != null && keyValuePair.Value == num7)
															{
																newAddress = keyValuePair.Key.address;
																break;
															}
														}
														if ((human != null && human != belongsTo) || (newAddress != null && newAddress != belongsTo.home))
														{
															continue;
														}
														if (human == null && newAddress == null)
														{
															continue;
														}
													}
												}
												else if (forcedOwnership == InteractablePreset.OwnedPlacementRule.prioritiseOwned)
												{
													if (belongsTo != null)
													{
														if (loc.belongsTo == FurniturePreset.SubObjectOwnership.everybody)
														{
															if (!obj.ownerMap.ContainsKey(new FurnitureLocation.OwnerKey(belongsTo)))
															{
																num4 += 10f;
															}
														}
														else
														{
															int num8 = loc.belongsTo - FurniturePreset.SubObjectOwnership.person0;
															Human human2 = null;
															NewAddress newAddress2 = null;
															foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair2 in obj.ownerMap)
															{
																if (keyValuePair2.Key.human != null)
																{
																	if (keyValuePair2.Value == num8)
																	{
																		human2 = keyValuePair2.Key.human;
																		break;
																	}
																}
																else if (keyValuePair2.Key.address != null && keyValuePair2.Value == num8)
																{
																	newAddress2 = keyValuePair2.Key.address;
																	break;
																}
															}
															if (human2 != null && human2 == belongsTo)
															{
																num4 += 10f;
															}
															else if (newAddress2 != null && newAddress2 == belongsTo.home)
															{
																num4 += 10f;
															}
														}
													}
												}
												else if (forcedOwnership == InteractablePreset.OwnedPlacementRule.nonOwnedOnly)
												{
													if (loc.belongsTo != FurniturePreset.SubObjectOwnership.nobody)
													{
														continue;
													}
												}
												else if (forcedOwnership == InteractablePreset.OwnedPlacementRule.prioritiseNonOwned && loc.belongsTo == FurniturePreset.SubObjectOwnership.nobody)
												{
													num4 += 10f;
												}
												if (placeClosestTo != null)
												{
													float num9 = Vector3.Distance(obj.anchorNode.nodeCoord, placeClosestTo.nodeCoord);
													num4 += (float)(10 - Mathf.RoundToInt(num9));
												}
												if (num4 > num)
												{
													result = new NewGameLocation.ObjectPlacement
													{
														location = loc,
														furnParent = obj
													};
													num = num4;
												}
											}
										}
									}
									if (interactable.useSubSpawning)
									{
										foreach (Interactable interactable3 in obj.spawnedInteractables)
										{
											if (interactable3.ssp != null && interactable3.ssp.Count > 0 && interactable3.preset.subSpawnClass != null && interactable.subObjectClasses.Contains(interactable3.preset.subSpawnClass))
											{
												float num10 = 22f + Toolbox.Instance.RandContained(-0.1f, 0.1f, seedInput, out seedInput);
												if (num10 > num)
												{
													result = new NewGameLocation.ObjectPlacement
													{
														subSpawn = interactable3
													};
													num = num10;
												}
											}
										}
										continue;
									}
									continue;
									IL_6F8:
									using (List<SubObjectClassPreset>.Enumerator enumerator4 = interactable.backupPutDownPositions.GetEnumerator())
									{
										while (enumerator4.MoveNext())
										{
											SubObjectClassPreset placeClass = enumerator4.Current;
											List<FurniturePreset.SubObject> list5 = obj.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == placeClass);
											if (list5.Count > 0)
											{
												if (printDebug)
												{
													Game.Log(string.Concat(new string[]
													{
														"Object: Found matching backup classes on ",
														obj.furniture.name,
														" (",
														list5.Count.ToString(),
														" potential points matching class ",
														placeClass.name,
														")"
													}), 2);
												}
												using (List<FurniturePreset.SubObject>.Enumerator enumerator5 = list5.GetEnumerator())
												{
													while (enumerator5.MoveNext())
													{
														FurniturePreset.SubObject s = enumerator5.Current;
														bool flag2 = obj.spawnedInteractables.Find((Interactable item) => item.subObject == s) != null;
														float num11 = (float)(-80 - Toolbox.Instance.RandContained(0, 10, seedInput, out seedInput));
														if (flag2)
														{
															num11 -= 100f;
														}
														if (num11 > num)
														{
															result = new NewGameLocation.ObjectPlacement
															{
																location = s,
																furnParent = obj
															};
															num = num11;
														}
													}
													continue;
												}
											}
											if (printDebug)
											{
												Game.Log("Object: Cannot find any points of backup class " + placeClass.name + " on furniture " + obj.furniture.name, 2);
											}
										}
									}
									goto IL_8BD;
								}
								if (printDebug)
								{
									Game.Log("Object: Reached limit for this type of object: " + interactable.perObjectLimit.ToString(), 2);
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x00082E44 File Offset: 0x00081044
	public void AddToPlacementPool(InteractablePreset interactable, Human belongsTo, Human writer, Human receiver, List<Interactable.Passed> passedVars = null, int security = 0, InteractablePreset.OwnedPlacementRule ownership = InteractablePreset.OwnedPlacementRule.nonOwnedOnly, int priority = 0, object passedObject = null, HashSet<NewRoom> dontPlaceInRooms = null)
	{
		if (interactable == null)
		{
			return;
		}
		if (this.thisAsAddress != null && (interactable.limitPerAddress || interactable.limitInCommercial || interactable.limitInResidential))
		{
			int num = 99999;
			if (interactable.limitPerAddress)
			{
				num = Mathf.Min(interactable.perAddressLimit, num);
			}
			if (interactable.limitInCommercial && this.thisAsAddress.company != null)
			{
				num = Mathf.Min(interactable.perCommercialLimit, num);
			}
			if (interactable.limitInResidential && this.thisAsAddress.residence != null)
			{
				num = Mathf.Min(interactable.perResidentialLimit, num);
			}
			if (this.objectsToPlace.FindAll((NewGameLocation.ObjectPlace item) => item.interactable == interactable).Count >= num)
			{
				return;
			}
		}
		NewGameLocation.ObjectPlace objectPlace = new NewGameLocation.ObjectPlace();
		objectPlace.interactable = interactable;
		objectPlace.belongsTo = belongsTo;
		objectPlace.writer = writer;
		objectPlace.receiver = receiver;
		objectPlace.passedVars = passedVars;
		objectPlace.security = security;
		objectPlace.ownership = ownership;
		objectPlace.priority = priority;
		objectPlace.passedObject = passedObject;
		objectPlace.dontPlaceInRooms = dontPlaceInRooms;
		this.objectsToPlace.Add(objectPlace);
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x00082FB0 File Offset: 0x000811B0
	public void PlaceObjects()
	{
		string text = this.seed;
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			foreach (NewRoom newRoom in this.rooms)
			{
				newRoom.palcementKey1 = text;
			}
		}
		foreach (InteractablePreset interactablePreset in Toolbox.Instance.placeAtGameLocationInteractables)
		{
			if (interactablePreset.autoPlacement == InteractablePreset.AutoPlacement.always || (interactablePreset.autoPlacement == InteractablePreset.AutoPlacement.onlyInCompany && this.thisAsAddress != null && this.thisAsAddress.company != null) || (interactablePreset.autoPlacement == InteractablePreset.AutoPlacement.onlyInHomes && this.thisAsAddress != null && this.thisAsAddress.residence != null) || (interactablePreset.autoPlacement == InteractablePreset.AutoPlacement.onlyOnStreet && this.thisAsStreet != null))
			{
				int num = Toolbox.Instance.RandContained(interactablePreset.frequencyPerGamelocationMin, interactablePreset.frequencyPerGameLocationMax + 1, text, out text);
				for (int i = 0; i < num; i++)
				{
					this.AddToPlacementPool(interactablePreset, null, null, null, null, interactablePreset.securityLevel, interactablePreset.ownedRule, interactablePreset.perGameLocationObjectPriority, null, null);
				}
			}
		}
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			foreach (NewRoom newRoom2 in this.rooms)
			{
				newRoom2.palcementKey2 = text;
			}
		}
		if (this.thisAsAddress != null)
		{
			foreach (Human human in this.thisAsAddress.inhabitants)
			{
				foreach (InteractablePreset interactablePreset2 in Toolbox.Instance.placePerOwnerInteractables)
				{
					if (interactablePreset2.autoPlacement == InteractablePreset.AutoPlacement.always || (interactablePreset2.autoPlacement == InteractablePreset.AutoPlacement.onlyInCompany && this.thisAsAddress != null && this.thisAsAddress.company != null) || (interactablePreset2.autoPlacement == InteractablePreset.AutoPlacement.onlyInHomes && this.thisAsAddress != null && this.thisAsAddress.residence != null) || (interactablePreset2.autoPlacement == InteractablePreset.AutoPlacement.onlyOnStreet && this.thisAsStreet != null))
					{
						bool flag = true;
						int num2 = 0;
						int num3 = 0;
						foreach (InteractablePreset.TraitPick traitPick in interactablePreset2.traitModifiers)
						{
							bool flag2 = false;
							if (traitPick.rule == CharacterTrait.RuleType.ifAnyOfThese)
							{
								using (List<CharacterTrait>.Enumerator enumerator5 = traitPick.traitList.GetEnumerator())
								{
									while (enumerator5.MoveNext())
									{
										CharacterTrait searchTrait = enumerator5.Current;
										if (human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
										{
											flag2 = true;
											break;
										}
									}
									goto IL_45A;
								}
								goto IL_2FC;
							}
							goto IL_2FC;
							IL_45A:
							if (!flag2 && traitPick.mustPassForApplication)
							{
								flag = false;
								break;
							}
							if (flag2)
							{
								num2 += traitPick.appliedFrequencyMin;
								num3 += traitPick.appliedFrequencyMax;
								continue;
							}
							continue;
							IL_2FC:
							if (traitPick.rule == CharacterTrait.RuleType.ifAllOfThese)
							{
								flag2 = true;
								using (List<CharacterTrait>.Enumerator enumerator5 = traitPick.traitList.GetEnumerator())
								{
									while (enumerator5.MoveNext())
									{
										CharacterTrait searchTrait = enumerator5.Current;
										if (!human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
										{
											flag2 = false;
											break;
										}
									}
									goto IL_45A;
								}
							}
							if (traitPick.rule == CharacterTrait.RuleType.ifNoneOfThese)
							{
								flag2 = true;
								using (List<CharacterTrait>.Enumerator enumerator5 = traitPick.traitList.GetEnumerator())
								{
									while (enumerator5.MoveNext())
									{
										CharacterTrait searchTrait = enumerator5.Current;
										if (human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
										{
											flag2 = false;
											break;
										}
									}
									goto IL_45A;
								}
							}
							if (traitPick.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese && human.partner != null)
							{
								using (List<CharacterTrait>.Enumerator enumerator5 = traitPick.traitList.GetEnumerator())
								{
									while (enumerator5.MoveNext())
									{
										CharacterTrait searchTrait = enumerator5.Current;
										if (human.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
										{
											flag2 = true;
											break;
										}
									}
								}
								goto IL_45A;
							}
							goto IL_45A;
						}
						if (flag)
						{
							int num4 = interactablePreset2.frequencyPerOwnerMin + num2;
							int num5 = interactablePreset2.frequencyPerOwnerMax + 1 + num3;
							if (interactablePreset2.multiplyByMessiness)
							{
								num4 = Mathf.RoundToInt((float)num4 * (1f - human.conscientiousness));
								num5 = Mathf.RoundToInt((float)num5 * (1f - human.conscientiousness));
							}
							int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(num4, num5, text, out text);
							Human human2 = human;
							if (interactablePreset2.writerIs == EvidencePreset.BelongsToSetting.partner && human2.partner != null)
							{
								human2 = human2.partner;
							}
							else if (interactablePreset2.writerIs == EvidencePreset.BelongsToSetting.boss && human2.job != null && human2.job.boss != null && human2.job.boss.employee != null)
							{
								human2 = human2.job.boss.employee;
							}
							else if (interactablePreset2.writerIs == EvidencePreset.BelongsToSetting.paramour && human2.paramour != null)
							{
								human2 = human2.paramour;
							}
							else if (interactablePreset2.writerIs == EvidencePreset.BelongsToSetting.doctor)
							{
								human2 = human2.GetDoctor();
							}
							else if (interactablePreset2.writerIs == EvidencePreset.BelongsToSetting.landlord)
							{
								human2 = human2.GetLandlord();
							}
							Human human3 = human;
							if (interactablePreset2.receiverIs == EvidencePreset.BelongsToSetting.partner && human3.partner != null)
							{
								human3 = human3.partner;
							}
							else if (interactablePreset2.receiverIs == EvidencePreset.BelongsToSetting.boss && human3.job != null && human3.job.boss != null && human3.job.boss.employee != null)
							{
								human3 = human3.job.boss.employee;
							}
							else if (interactablePreset2.receiverIs == EvidencePreset.BelongsToSetting.paramour && human3.paramour != null)
							{
								human3 = human3.paramour;
							}
							else if (interactablePreset2.receiverIs == EvidencePreset.BelongsToSetting.doctor)
							{
								human3 = human3.GetDoctor();
							}
							else if (interactablePreset2.receiverIs == EvidencePreset.BelongsToSetting.landlord)
							{
								human3 = human3.GetLandlord();
							}
							if (!(human2 == human3) || interactablePreset2.canBeFromSelf || (interactablePreset2.writerIs == EvidencePreset.BelongsToSetting.self && interactablePreset2.receiverIs == EvidencePreset.BelongsToSetting.self))
							{
								for (int j = 0; j < psuedoRandomNumberContained; j++)
								{
									this.AddToPlacementPool(interactablePreset2, human, human2, human3, null, interactablePreset2.securityLevel, interactablePreset2.ownedRule, interactablePreset2.perOwnerObjectPriority, null, null);
								}
							}
						}
					}
				}
			}
		}
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			foreach (NewRoom newRoom3 in this.rooms)
			{
				newRoom3.palcementKey3 = text;
			}
		}
		Dictionary<BookPreset, Human> dictionary = new Dictionary<BookPreset, Human>();
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			foreach (NewRoom newRoom4 in this.rooms)
			{
				newRoom4.palcementKey4 = text;
			}
		}
		if (this.thisAsAddress != null && this.thisAsAddress.addressPreset != null)
		{
			foreach (InteractablePreset interactablePreset3 in this.thisAsAddress.addressPreset.specialItems)
			{
				if (interactablePreset3 == null)
				{
					Game.LogError("Null interactable preset special item found in " + this.thisAsAddress.addressPreset.name, 2);
				}
				else
				{
					this.AddToPlacementPool(interactablePreset3, null, null, null, null, interactablePreset3.securityLevel, interactablePreset3.ownedRule, interactablePreset3.perGameLocationObjectPriority, null, null);
				}
			}
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				foreach (NewRoom newRoom5 in this.rooms)
				{
					newRoom5.palcementKey51 = text;
				}
			}
			if (this.thisAsAddress.residence != null)
			{
				foreach (Human human4 in this.thisAsAddress.inhabitants)
				{
					foreach (InteractablePreset interactablePreset4 in human4.personalAffects)
					{
						this.AddToPlacementPool(interactablePreset4, human4, human4, null, null, interactablePreset4.securityLevel, interactablePreset4.ownedRule, interactablePreset4.perOwnerObjectPriority, null, null);
					}
					int num6 = 0;
					while (num6 < human4.booksAwayFromShelf && human4.nonShelfBooks.Count > 0)
					{
						BookPreset bookPreset = human4.nonShelfBooks[Toolbox.Instance.GetPsuedoRandomNumberContained(0, human4.nonShelfBooks.Count, text, out text)];
						if (bookPreset.spawnRule != BookPreset.SpawnRules.onlyAtWork && (bookPreset.spawnRule != BookPreset.SpawnRules.homeOrWork || (bookPreset.spawnRule == BookPreset.SpawnRules.homeOrWork && Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, text, out text) > 0.5f)))
						{
							if (bookPreset.spawnRule == BookPreset.SpawnRules.secret)
							{
								this.AddToPlacementPool(InteriorControls.Instance.bookNonShelfSecret, human4, human4, null, null, InteriorControls.Instance.bookNonShelfSecret.securityLevel, InteriorControls.Instance.bookNonShelfSecret.ownedRule, InteriorControls.Instance.bookNonShelfSecret.perOwnerObjectPriority, bookPreset, null);
							}
							else
							{
								this.AddToPlacementPool(InteriorControls.Instance.bookNonShelf, human4, human4, null, null, InteriorControls.Instance.bookNonShelf.securityLevel, InteriorControls.Instance.bookNonShelf.ownedRule, InteriorControls.Instance.bookNonShelf.perOwnerObjectPriority, bookPreset, null);
							}
							human4.nonShelfBooks.Remove(bookPreset);
							human4.library.Remove(bookPreset);
						}
						num6++;
					}
					foreach (BookPreset bookPreset2 in human4.library)
					{
						if (!dictionary.ContainsKey(bookPreset2))
						{
							dictionary.Add(bookPreset2, human4);
						}
					}
				}
			}
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				foreach (NewRoom newRoom6 in this.rooms)
				{
					newRoom6.palcementKey52 = text;
				}
			}
			if (this.thisAsAddress.company != null)
			{
				foreach (Occupation occupation in this.thisAsAddress.company.companyRoster)
				{
					if (!(occupation.employee == null))
					{
						foreach (InteractablePreset interactablePreset5 in occupation.employee.workAffects)
						{
							if (interactablePreset5 == null)
							{
								Game.LogError("Null work affect found for " + occupation.employee.name, 2);
							}
							this.AddToPlacementPool(interactablePreset5, occupation.employee, occupation.employee, null, null, interactablePreset5.securityLevel, interactablePreset5.ownedRule, interactablePreset5.perOwnerObjectPriority, null, null);
						}
					}
				}
			}
		}
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			foreach (NewRoom newRoom7 in this.rooms)
			{
				newRoom7.palcementKey5 = text;
			}
		}
		this.objectsToPlace.Sort((NewGameLocation.ObjectPlace p1, NewGameLocation.ObjectPlace p2) => p2.priority.CompareTo(p1.priority));
		Dictionary<SubObjectClassPreset, List<NewGameLocation.Placement>> dictionary2 = new Dictionary<SubObjectClassPreset, List<NewGameLocation.Placement>>();
		Dictionary<InteractablePreset, List<Interactable>> dictionary3 = new Dictionary<InteractablePreset, List<Interactable>>();
		Dictionary<InteractablePreset, Dictionary<NewRoom, List<Interactable>>> dictionary4 = new Dictionary<InteractablePreset, Dictionary<NewRoom, List<Interactable>>>();
		Dictionary<InteractablePreset, Dictionary<FurnitureLocation, List<Interactable>>> dictionary5 = new Dictionary<InteractablePreset, Dictionary<FurnitureLocation, List<Interactable>>>();
		using (List<NewRoom>.Enumerator enumerator = this.rooms.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NewRoom newRoom8 = enumerator.Current;
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					newRoom8.poolSizeOnPlacement = this.objectsToPlace.Count;
					newRoom8.palcementKey6 = text;
				}
				foreach (FurnitureClusterLocation furnitureClusterLocation in newRoom8.furniture)
				{
					foreach (FurnitureLocation furnitureLocation in furnitureClusterLocation.clusterList)
					{
						if (furnitureLocation.furniture != null)
						{
							foreach (FurniturePreset.SubObject subObject in furnitureLocation.furniture.subObjects)
							{
								NewGameLocation.Placement placement = new NewGameLocation.Placement
								{
									room = newRoom8,
									furniture = furnitureLocation,
									placementClass = subObject.preset,
									subObject = subObject
								};
								if (!dictionary2.ContainsKey(placement.placementClass))
								{
									dictionary2.Add(placement.placementClass, new List<NewGameLocation.Placement>());
								}
								dictionary2[placement.placementClass].Add(placement);
							}
						}
					}
				}
			}
			goto IL_1EED;
		}
		IL_E5F:
		NewGameLocation.ObjectPlace pl = this.objectsToPlace[0];
		if (pl.interactable.attemptToStoreInFolder != null && Toolbox.Instance.RandContained(0f, 1f, text, out text) <= pl.interactable.folderPlacementChance)
		{
			bool flag3 = false;
			Predicate<Interactable> <>9__5;
			Predicate<Interactable> <>9__6;
			foreach (NewRoom newRoom9 in this.rooms)
			{
				foreach (FurnitureLocation furnitureLocation2 in newRoom9.individualFurniture)
				{
					Interactable interactable;
					if (pl.interactable.folderOwnershipMustMatch)
					{
						List<Interactable> spawnedInteractables = furnitureLocation2.spawnedInteractables;
						Predicate<Interactable> predicate;
						if ((predicate = <>9__5) == null)
						{
							predicate = (<>9__5 = ((Interactable item) => item.evidence != null && item.evidence.preset == pl.interactable.attemptToStoreInFolder && item.belongsTo == pl.belongsTo));
						}
						interactable = spawnedInteractables.Find(predicate);
					}
					else
					{
						List<Interactable> spawnedInteractables2 = furnitureLocation2.spawnedInteractables;
						Predicate<Interactable> predicate2;
						if ((predicate2 = <>9__6) == null)
						{
							predicate2 = (<>9__6 = ((Interactable item) => item.evidence != null && item.evidence.preset == pl.interactable.attemptToStoreInFolder));
						}
						interactable = spawnedInteractables2.Find(predicate2);
					}
					if (interactable != null)
					{
						EvidenceMultiPage evidenceMultiPage = interactable.evidence as EvidenceMultiPage;
						if (evidenceMultiPage != null)
						{
							MetaObject containedMetaObject = new MetaObject(pl.interactable, pl.belongsTo, pl.writer, pl.receiver, null);
							evidenceMultiPage.AddContainedMetaObjectToNewPage(containedMetaObject);
						}
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					break;
				}
			}
			if (!flag3)
			{
				Game.Log(string.Concat(new string[]
				{
					"CityGen: Could not find ",
					pl.interactable.attemptToStoreInFolder.name,
					" for placing ",
					pl.interactable.name,
					" at ",
					base.name
				}), 2);
			}
			if (flag3 || pl.interactable.dontPlaceIfNoFolder)
			{
				this.objectsToPlace.RemoveAt(0);
				goto IL_1EED;
			}
		}
		bool flag4 = false;
		RetailItemPreset retailItemPreset = pl.passedObject as RetailItemPreset;
		if (retailItemPreset != null && retailItemPreset.isHot)
		{
			float value = pl.passedVars.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.time).value;
			if (SessionData.Instance.gameTime - value <= GameplayControls.Instance.foodHotTime)
			{
				flag4 = true;
			}
		}
		if (pl.interactable.limitPerAddress && dictionary3.ContainsKey(pl.interactable) && dictionary3[pl.interactable].Count >= pl.interactable.perAddressLimit)
		{
			Game.Log(string.Concat(new string[]
			{
				"Object: ",
				base.name,
				" has maximum number of ",
				pl.interactable.name,
				" objects according to 'limit per gamelocation' setting."
			}), 2);
			this.objectsToPlace.RemoveAt(0);
		}
		else
		{
			List<NewGameLocation.Placement> list = new List<NewGameLocation.Placement>();
			int num7 = pl.interactable.subObjectClasses.Count + pl.interactable.backupClasses.Count;
			for (int k = 0; k < num7; k++)
			{
				SubObjectClassPreset subObjectClassPreset;
				if (k < pl.interactable.subObjectClasses.Count)
				{
					subObjectClassPreset = pl.interactable.subObjectClasses[k];
				}
				else
				{
					subObjectClassPreset = pl.interactable.backupClasses[k - pl.interactable.subObjectClasses.Count];
				}
				if (dictionary2.ContainsKey(subObjectClassPreset))
				{
					List<NewGameLocation.Placement> list2 = dictionary2[subObjectClassPreset];
					for (int l = 0; l < list2.Count; l++)
					{
						NewGameLocation.Placement placement2 = list2[l];
						if ((pl.dontPlaceInRooms == null || !pl.dontPlaceInRooms.Contains(placement2.room)) && (placement2.room.preset.allowPersonalAffects || !(pl.belongsTo != null)))
						{
							if (pl.interactable.limitToCertainRooms)
							{
								if (!pl.interactable.onlyInRooms.Contains(placement2.room.preset))
								{
									goto IL_19CB;
								}
							}
							else if (pl.interactable.banFromRooms.Contains(placement2.room.preset))
							{
								goto IL_19CB;
							}
							if ((!pl.interactable.limitToCertainBuildings || (!(placement2.room.building == null) && pl.interactable.onlyInBuildings.Contains(placement2.room.building.preset))) && (!pl.interactable.limitPerRoom || !dictionary4.ContainsKey(pl.interactable) || !dictionary4[pl.interactable].ContainsKey(placement2.room) || dictionary4[pl.interactable][placement2.room].Count < pl.interactable.perRoomLimit) && (!pl.interactable.limitPerObject || !dictionary5.ContainsKey(pl.interactable) || placement2.furniture == null || !dictionary5[pl.interactable].ContainsKey(placement2.furniture) || dictionary5[pl.interactable][placement2.furniture].Count < pl.interactable.perObjectLimit) && (!flag4 || !(placement2.placementClass == InteriorControls.Instance.fridge)))
							{
								placement2.rank = Toolbox.Instance.RandContained(0f, 0.1f, text, out text);
								if (pl.interactable.useSubSpawning && placement2.subSpawn != null)
								{
									placement2.rank += 1f;
								}
								InteractablePreset.OwnedPlacementRule ownedPlacementRule = pl.ownership;
								if (pl.interactable.overrideWithOnlyOwnedSpawnAtWork && pl.belongsTo != null && pl.belongsTo.job != null && pl.belongsTo.job.employer != null && pl.belongsTo.job.employer.address == placement2.room.gameLocation)
								{
									ownedPlacementRule = InteractablePreset.OwnedPlacementRule.ownedOnly;
								}
								if (ownedPlacementRule == InteractablePreset.OwnedPlacementRule.both)
								{
									placement2.rank += 1f;
								}
								else if (ownedPlacementRule == InteractablePreset.OwnedPlacementRule.ownedOnly)
								{
									if (pl.belongsTo == null || placement2.subObject.belongsTo == FurniturePreset.SubObjectOwnership.nobody)
									{
										goto IL_19CB;
									}
									if (placement2.subObject.belongsTo == FurniturePreset.SubObjectOwnership.everybody)
									{
										if (!placement2.furniture.ownerMap.ContainsKey(new FurnitureLocation.OwnerKey(pl.belongsTo)))
										{
											goto IL_19CB;
										}
									}
									else
									{
										int num8 = placement2.subObject.belongsTo - FurniturePreset.SubObjectOwnership.person0;
										Human human5 = null;
										NewAddress newAddress = null;
										foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in placement2.furniture.ownerMap)
										{
											if (keyValuePair.Key.human != null)
											{
												if (keyValuePair.Value == num8)
												{
													human5 = keyValuePair.Key.human;
													break;
												}
											}
											else if (keyValuePair.Key.address != null && keyValuePair.Value == num8)
											{
												newAddress = keyValuePair.Key.address;
												break;
											}
										}
										if ((human5 != null && human5 != pl.belongsTo) || (newAddress != null && newAddress != pl.belongsTo.home))
										{
											goto IL_19CB;
										}
										if (human5 == null && newAddress == null)
										{
											goto IL_19CB;
										}
									}
								}
								else if (ownedPlacementRule == InteractablePreset.OwnedPlacementRule.prioritiseOwned)
								{
									if (pl.belongsTo != null)
									{
										if (placement2.subObject.belongsTo == FurniturePreset.SubObjectOwnership.everybody)
										{
											if (!placement2.furniture.ownerMap.ContainsKey(new FurnitureLocation.OwnerKey(pl.belongsTo)))
											{
												placement2.rank += 1f;
											}
										}
										else
										{
											int num9 = placement2.subObject.belongsTo - FurniturePreset.SubObjectOwnership.person0;
											Human human6 = null;
											NewAddress newAddress2 = null;
											foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair2 in placement2.furniture.ownerMap)
											{
												if (keyValuePair2.Key.human != null)
												{
													if (keyValuePair2.Value == num9)
													{
														human6 = keyValuePair2.Key.human;
														break;
													}
												}
												else if (keyValuePair2.Key.address != null && keyValuePair2.Value == num9)
												{
													newAddress2 = keyValuePair2.Key.address;
													break;
												}
											}
											if (human6 != null && human6 == pl.belongsTo)
											{
												placement2.rank += 1f;
											}
											else if (newAddress2 != null && newAddress2 == pl.belongsTo.home)
											{
												placement2.rank += 1f;
											}
										}
									}
								}
								else if (ownedPlacementRule == InteractablePreset.OwnedPlacementRule.nonOwnedOnly)
								{
									if (placement2.subObject.belongsTo != FurniturePreset.SubObjectOwnership.nobody)
									{
										goto IL_19CB;
									}
								}
								else if (ownedPlacementRule == InteractablePreset.OwnedPlacementRule.prioritiseNonOwned && placement2.subObject.belongsTo == FurniturePreset.SubObjectOwnership.nobody)
								{
									placement2.rank += 1f;
								}
								int num10 = placement2.room.preset.securityLevel + placement2.subObject.security;
								if (num10 == pl.security)
								{
									placement2.rank += 1f;
								}
								else if (num10 > pl.security)
								{
									placement2.rank += 1f - (float)Mathf.Abs(num10 - pl.security) * 0.25f;
								}
								else
								{
									placement2.rank += 1f - (float)Mathf.Abs(num10 - pl.security) * 0.5f;
								}
								list.Add(placement2);
							}
						}
						IL_19CB:;
					}
					if (k >= pl.interactable.subObjectClasses.Count - 1 && list.Count > 0)
					{
						break;
					}
				}
			}
			if (list.Count > 0)
			{
				list.Sort((NewGameLocation.Placement p1, NewGameLocation.Placement p2) => p2.rank.CompareTo(p1.rank));
				NewGameLocation.Placement placement3 = list[0];
				Interactable interactable2;
				if (placement3.subSpawn == null)
				{
					if (!placement3.furniture.createdInteractables)
					{
						placement3.furniture.CreateInteractables();
					}
					interactable2 = InteractableCreator.Instance.CreateFurnitureSpawnedInteractableThreadSafe(pl.interactable, placement3.furniture.anchorNode.room, placement3.furniture, placement3.subObject, pl.belongsTo, pl.writer, pl.receiver, pl.passedVars, pl.interactable.isLight, pl.passedObject, "");
				}
				else
				{
					if (!placement3.subSpawn.furnitureParent.createdInteractables)
					{
						placement3.subSpawn.furnitureParent.CreateInteractables();
					}
					int num11 = Toolbox.Instance.RandContained(0, placement3.subSpawn.ssp.Count, text, out text);
					InteractablePreset.SubSpawnSlot subSpawnSlot = placement3.subSpawn.ssp[num11];
					Vector2 vector = Random.insideUnitCircle * 0.2f;
					Vector3 vector2 = placement3.subSpawn.wPos + subSpawnSlot.localPos;
					vector2 += new Vector3(vector.x, 0f, vector.y);
					Vector3 worldEuler = placement3.subSpawn.wEuler + subSpawnSlot.localEuler;
					interactable2 = InteractableCreator.Instance.CreateWorldInteractable(pl.interactable, pl.belongsTo, pl.writer, pl.receiver, vector2, worldEuler, pl.passedVars, pl.passedObject, "");
					placement3.subSpawn.ssp.RemoveAt(num11);
				}
				if (interactable2 != null)
				{
					interactable2.opp = pl.priority;
					if (interactable2.preset.limitPerAddress)
					{
						if (!dictionary3.ContainsKey(interactable2.preset))
						{
							dictionary3.Add(interactable2.preset, new List<Interactable>());
						}
						dictionary3[interactable2.preset].Add(interactable2);
					}
					if (interactable2.preset.limitPerRoom)
					{
						if (!dictionary4.ContainsKey(interactable2.preset))
						{
							dictionary4.Add(interactable2.preset, new Dictionary<NewRoom, List<Interactable>>());
						}
						if (!dictionary4[interactable2.preset].ContainsKey(placement3.room))
						{
							dictionary4[interactable2.preset].Add(placement3.room, new List<Interactable>());
						}
						dictionary4[interactable2.preset][placement3.room].Add(interactable2);
					}
					if (interactable2.preset.limitPerObject)
					{
						if (!dictionary5.ContainsKey(interactable2.preset))
						{
							dictionary5.Add(interactable2.preset, new Dictionary<FurnitureLocation, List<Interactable>>());
						}
						if (!dictionary5[interactable2.preset].ContainsKey(placement3.furniture))
						{
							dictionary5[interactable2.preset].Add(placement3.furniture, new List<Interactable>());
						}
						dictionary5[interactable2.preset][placement3.furniture].Add(interactable2);
					}
					if (interactable2.ssp != null && interactable2.ssp.Count > 0 && interactable2.preset.subSpawnClass != null)
					{
						NewGameLocation.Placement placement4 = new NewGameLocation.Placement
						{
							room = placement3.room,
							furniture = placement3.furniture,
							placementClass = interactable2.preset.subSpawnClass,
							subObject = placement3.subObject,
							subSpawn = placement3.subSpawn
						};
						if (!dictionary2.ContainsKey(placement4.placementClass))
						{
							dictionary2.Add(placement4.placementClass, new List<NewGameLocation.Placement>());
						}
						dictionary2[placement4.placementClass].Add(placement4);
					}
					if (placement3.subSpawn == null || placement3.subSpawn.ssp.Count <= 0)
					{
						dictionary2[placement3.placementClass].Remove(placement3);
						if (dictionary2[placement3.placementClass].Count <= 0)
						{
							dictionary2.Remove(placement3.placementClass);
						}
					}
					if (interactable2.preset.specialCaseFlag == InteractablePreset.SpecialCase.bookStack)
					{
						this.stacks.Add(interactable2);
					}
				}
			}
			if (dictionary2.Count <= 0)
			{
				goto IL_1EFE;
			}
			this.objectsToPlace.RemoveAt(0);
		}
		IL_1EED:
		if (this.objectsToPlace.Count > 0)
		{
			goto IL_E5F;
		}
		IL_1EFE:
		if (this.thisAsAddress != null)
		{
			if (this.thisAsAddress.addressPreset != null && Toolbox.Instance.RandContained(0f, 1f, text, out text) <= this.thisAsAddress.addressPreset.chanceOfExternalSpareKey)
			{
				Toolbox.Instance.SpawnSpareKey(this.thisAsAddress, null);
			}
			if (this.thisAsAddress.stacks.Count > 0)
			{
				InteractablePreset.SpecialCase stackType = InteractablePreset.SpecialCase.bookStack;
				List<Interactable> list3 = this.thisAsAddress.stacks.FindAll((Interactable item) => item.preset.specialCaseFlag == stackType && item.ssp.Count > 0);
				List<BookPreset> list4 = new List<BookPreset>(dictionary.Keys);
				while (list3.Count > 0 && dictionary.Count > 0)
				{
					int num12 = Toolbox.Instance.RandContained(0, list3.Count, text, out text);
					int num13 = Toolbox.Instance.RandContained(0, list4.Count, text, out text);
					Interactable interactable3 = list3[num12];
					BookPreset bookPreset3 = list4[num13];
					int num14 = Toolbox.Instance.RandContained(0, interactable3.ssp.Count, text, out text);
					Vector3 localPos = interactable3.lPos + interactable3.ssp[num14].localPos;
					Vector3 localEuler = interactable3.lEuler + interactable3.ssp[num14].localEuler;
					InteractableCreator.Instance.CreateBookInteractable(InteriorControls.Instance.bookShelf, interactable3.furnitureParent.anchorNode.room, interactable3.furnitureParent, dictionary[bookPreset3], localPos, localEuler, bookPreset3);
					interactable3.ssp.RemoveAt(num14);
					if (interactable3.ssp.Count <= 0)
					{
						list3.RemoveAt(num12);
					}
					list4.RemoveAt(num13);
					dictionary.Remove(bookPreset3);
				}
			}
		}
		this.objectsToPlace = null;
		this.objectPoolPlaced = true;
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x00085368 File Offset: 0x00083568
	public bool IsPublicallyOpen(bool forPlayer)
	{
		if (this.isCrimeScene)
		{
			return false;
		}
		if (this.thisAsAddress != null && this.thisAsAddress.access == AddressPreset.AccessType.allPublic)
		{
			if (forPlayer && this.thisAsAddress.addressPreset != null && this.thisAsAddress.addressPreset.needsPassword && !GameplayController.Instance.playerKnowsPasswords.Contains(this.thisAsAddress.id))
			{
				return false;
			}
			if (this.thisAsAddress.company != null)
			{
				return this.thisAsAddress.company.preset.publicFacing && (this.thisAsAddress.company.openForBusinessActual && this.thisAsAddress.company.openForBusinessDesired);
			}
			if (this.thisAsAddress.addressPreset != null && this.thisAsAddress.addressPreset.openHoursDicatedByAdjoiningCompany)
			{
				foreach (NewNode.NodeAccess nodeAccess in this.entrances)
				{
					if (nodeAccess.walkingAccess)
					{
						if (nodeAccess.toNode.gameLocation != this)
						{
							if (nodeAccess.toNode.gameLocation.thisAsAddress != null && nodeAccess.toNode.gameLocation.thisAsAddress.company != null && nodeAccess.toNode.gameLocation.thisAsAddress.company.publicFacing)
							{
								if (nodeAccess.toNode.gameLocation.thisAsAddress.company.openForBusinessActual && nodeAccess.toNode.gameLocation.thisAsAddress.company.openForBusinessDesired)
								{
									return true;
								}
								return false;
							}
						}
						else if (nodeAccess.fromNode.gameLocation != this && nodeAccess.fromNode.gameLocation.thisAsAddress != null && nodeAccess.fromNode.gameLocation.thisAsAddress.company != null && nodeAccess.toNode.gameLocation.thisAsAddress.company.publicFacing)
						{
							if (nodeAccess.fromNode.gameLocation.thisAsAddress.company.openForBusinessActual && nodeAccess.fromNode.gameLocation.thisAsAddress.company.openForBusinessDesired)
							{
								return true;
							}
							return false;
						}
					}
				}
				return true;
			}
		}
		else if (this.thisAsStreet != null)
		{
			return true;
		}
		return false;
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x00085624 File Offset: 0x00083824
	public void AddEscalation(Actor actor)
	{
		if (!this.escalation.ContainsKey(actor))
		{
			int actor2 = 0;
			Human human = actor as Human;
			if (human != null)
			{
				actor2 = human.humanID;
			}
			this.escalation.Add(actor, new NewGameLocation.TrespassEscalation
			{
				actor = actor2,
				isPlayer = actor.isPlayer,
				lastEscalationCheck = SessionData.Instance.gameTime,
				timeEscalation = 0f
			});
			if (!CitizenBehaviour.Instance.tempEscalationBoost.Contains(this))
			{
				CitizenBehaviour.Instance.tempEscalationBoost.Add(this);
			}
		}
		float num = SessionData.Instance.gameTime - this.escalation[actor].lastEscalationCheck;
		this.escalation[actor].timeEscalation += num;
		this.escalation[actor].timeEscalation = Mathf.Clamp(this.escalation[actor].timeEscalation, 0f, GameplayControls.Instance.additionalEscalationTime + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.trespassGraceModifier));
		this.escalation[actor].lastEscalationCheck = SessionData.Instance.gameTime;
		Game.Log("Setting escalation (+) for player by " + actor.name + ": " + this.escalation[actor].timeEscalation.ToString(), 2);
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x00085780 File Offset: 0x00083980
	public int GetAdditionalEscalation(Actor actor)
	{
		if (this.escalation.ContainsKey(actor))
		{
			int num = 0;
			if (this.escalation[actor].timeEscalation >= GameplayControls.Instance.additionalEscalationTime + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.trespassGraceModifier))
			{
				num++;
			}
			return num;
		}
		return 0;
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x000857D0 File Offset: 0x000839D0
	public void RemoveEscalation(Actor actor, bool removeAll = false)
	{
		if (this.escalation.ContainsKey(actor))
		{
			float num = SessionData.Instance.gameTime - this.escalation[actor].lastEscalationCheck;
			this.escalation[actor].timeEscalation -= num;
			this.escalation[actor].lastEscalationCheck = SessionData.Instance.gameTime;
			Game.Log("Setting escalation (-) for player by " + actor.name + ": " + this.escalation[actor].timeEscalation.ToString(), 2);
			if (this.escalation[actor].timeEscalation <= 0f || removeAll)
			{
				this.escalation.Remove(actor);
				if (this.escalation.Count <= 0)
				{
					CitizenBehaviour.Instance.tempEscalationBoost.Remove(this);
				}
			}
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x000858BB File Offset: 0x00083ABB
	public void AddSecurityCamera(Interactable newInteractable)
	{
		this.securityCameras.Add(newInteractable);
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x000858CC File Offset: 0x00083ACC
	public void SetAsCrimeScene(bool val)
	{
		if (val != this.isCrimeScene)
		{
			this.isCrimeScene = val;
			Game.Log(base.name + " is crime scene: " + this.isCrimeScene.ToString(), 2);
			if (this.isCrimeScene)
			{
				NewspaperController.Instance.GenerateNewNewspaper();
			}
		}
		if (this.isCrimeScene)
		{
			if (!GameplayController.Instance.crimeScenes.Contains(this))
			{
				GameplayController.Instance.crimeScenes.Add(this);
				return;
			}
		}
		else
		{
			GameplayController.Instance.crimeScenes.Remove(this);
			foreach (NewRoom newRoom in this.rooms)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					for (int i = 0; i < newNode.interactables.Count; i++)
					{
						Interactable interactable = newNode.interactables[i];
						if (interactable.preset == InteriorControls.Instance.streetCrimeScene)
						{
							interactable.SafeDelete(false);
							i--;
						}
						else if (interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.bloodPool)
						{
							interactable.SafeDelete(false);
							i--;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x00085A40 File Offset: 0x00083C40
	public virtual bool IsAlarmSystemTarget(Human human)
	{
		return false;
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x00085A43 File Offset: 0x00083C43
	public virtual bool IsAlarmActive(out float retAlarmTimer, out NewBuilding.AlarmTargetMode retTargetMode, out List<Human> retTargets)
	{
		retAlarmTimer = 0f;
		retTargetMode = NewBuilding.AlarmTargetMode.illegalActivities;
		retTargets = null;
		return false;
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00085A53 File Offset: 0x00083C53
	public virtual bool IsOutside()
	{
		return this.isOutside;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x00085A5C File Offset: 0x00083C5C
	public string GetReplicableSeed()
	{
		int num = 0;
		foreach (NewNode newNode in this.nodes)
		{
			num += newNode.nodeCoord.x + newNode.nodeCoord.y + newNode.nodeCoord.z;
		}
		return num.ToString();
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00085AD8 File Offset: 0x00083CD8
	public void ResetLoiteringTimer()
	{
		if (this.playerLoiteringTimer < 0f)
		{
			this.playerLoiteringTimer *= 0.6f;
			return;
		}
		this.playerLoiteringTimer = 0f;
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00085B05 File Offset: 0x00083D05
	public void LoiteringPurchase()
	{
		this.playerLoiteringTimer = GameplayControls.Instance.loiteringPurchaseResetValue;
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00085B17 File Offset: 0x00083D17
	[Button(null, 0)]
	public void RemoveEverything()
	{
		this.RemoveAllInhabitantFurniture(true, FurnitureClusterLocation.RemoveInteractablesOption.remove);
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00085B24 File Offset: 0x00083D24
	public void RemoveAllInhabitantFurniture(bool removeSkipAddressInhabitantsFurniture, FurnitureClusterLocation.RemoveInteractablesOption spawnedOnFurnitureRemovalOption)
	{
		foreach (NewRoom newRoom in this.rooms)
		{
			newRoom.RemoveAllInhabitantFurniture(removeSkipAddressInhabitantsFurniture, spawnedOnFurnitureRemovalOption);
		}
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00085B78 File Offset: 0x00083D78
	[Button(null, 0)]
	public void DisplayAccess()
	{
		foreach (NewRoom newRoom in this.rooms)
		{
			foreach (NewNode newNode in newRoom.nodes)
			{
				foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode.accessToOtherNodes)
				{
					if (!keyValuePair.Value.walkingAccess || keyValuePair.Value.employeeDoor || keyValuePair.Value.toNode.noPassThrough)
					{
						Object.Instantiate<GameObject>(PrefabControls.Instance.pathfindInternalDebug, PrefabControls.Instance.pathfindDebugParent).GetComponent<DebugPathfind>().Setup(keyValuePair.Value, keyValuePair.Value.fromNode.room, null);
					}
					else
					{
						Object.Instantiate<GameObject>(PrefabControls.Instance.pathfindNodeDebug, PrefabControls.Instance.pathfindDebugParent).GetComponent<DebugPathfind>().Setup(keyValuePair.Value, keyValuePair.Value.fromNode.room, null);
					}
				}
			}
		}
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00085D18 File Offset: 0x00083F18
	[Button(null, 0)]
	public int GetSQM(bool print = true)
	{
		float num = 0f;
		foreach (NewRoom newRoom in this.rooms)
		{
			num += (float)newRoom.nodes.Count;
		}
		num *= PathFinder.Instance.nodeSize.x;
		if (print)
		{
			Game.Log(Mathf.RoundToInt(num), 2);
		}
		return Mathf.RoundToInt(num);
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x00085DA8 File Offset: 0x00083FA8
	[Button(null, 0)]
	public int GetPrice(bool print = true)
	{
		float normalizedLandValue = Toolbox.Instance.GetNormalizedLandValue(this, print);
		float num = Mathf.Lerp(GameplayControls.Instance.propertyValueRange.x, GameplayControls.Instance.propertyValueRange.y, GameplayControls.Instance.propertyValueCurve.Evaluate(normalizedLandValue));
		float num2 = normalizedLandValue * (GameplayControls.Instance.propertyValueRange.y * 0.025f);
		num2 = (float)Mathf.RoundToInt(Toolbox.Instance.GetPsuedoRandomNumber(-num2, num2, this.rooms.Count.ToString() + this.nodes.Count.ToString() + CityData.Instance.seed, false));
		num += num2;
		num *= Game.Instance.housePriceMultiplier;
		num = (float)(Mathf.RoundToInt(num / 100f) * 100);
		if (print)
		{
			Game.Log(string.Concat(new string[]
			{
				Mathf.RoundToInt(num).ToString(),
				"= Land value: ",
				Mathf.Lerp(GameplayControls.Instance.propertyValueRange.x, GameplayControls.Instance.propertyValueRange.y, normalizedLandValue).ToString(),
				" + variance: ",
				num2.ToString()
			}), 2);
		}
		return Mathf.RoundToInt(num);
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x00085EFC File Offset: 0x000840FC
	[Button(null, 0)]
	public void GetAIActions()
	{
		foreach (KeyValuePair<AIActionPreset, List<Interactable>> keyValuePair in this.actionReference)
		{
			Game.Log(keyValuePair.Key.name + " (" + keyValuePair.Value.Count.ToString() + ")", 2);
		}
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x00085F80 File Offset: 0x00084180
	[Button(null, 0)]
	public void IsThisOutside()
	{
		Game.Log(this.IsOutside(), 2);
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x00085F94 File Offset: 0x00084194
	public bool AllowEmployeeDoors()
	{
		int num = 0;
		foreach (NewRoom newRoom in this.rooms)
		{
			foreach (NewNode newNode in newRoom.nodes)
			{
				foreach (NewWall newWall in newNode.walls)
				{
					if (newWall.preset != null && newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance && newWall.otherWall != null && newWall.node.gameLocation == this && newWall.otherWall.node.gameLocation != this)
					{
						num++;
						if (num >= 2)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x040008B1 RID: 2225
	[NonSerialized]
	public NewAddress thisAsAddress;

	// Token: 0x040008B2 RID: 2226
	[NonSerialized]
	public StreetController thisAsStreet;

	// Token: 0x040008B3 RID: 2227
	public string seed;

	// Token: 0x040008B4 RID: 2228
	[Header("Location")]
	public DistrictController district;

	// Token: 0x040008B5 RID: 2229
	public NewBuilding building;

	// Token: 0x040008B6 RID: 2230
	public NewFloor floor;

	// Token: 0x040008B7 RID: 2231
	public int residenceNumber;

	// Token: 0x040008B8 RID: 2232
	public MapAddressButtonController mapButton;

	// Token: 0x040008B9 RID: 2233
	[Space(7f)]
	public bool isLobby;

	// Token: 0x040008BA RID: 2234
	public bool isOutside;

	// Token: 0x040008BB RID: 2235
	public bool isCrimeScene;

	// Token: 0x040008BC RID: 2236
	public float loggedAsCrimeScene;

	// Token: 0x040008BD RID: 2237
	public AddressPreset.AccessType access;

	// Token: 0x040008BE RID: 2238
	[Header("Contents")]
	public NewRoom nullRoom;

	// Token: 0x040008BF RID: 2239
	public List<NewRoom> rooms = new List<NewRoom>();

	// Token: 0x040008C0 RID: 2240
	public List<NewNode> nodes = new List<NewNode>();

	// Token: 0x040008C1 RID: 2241
	public List<Actor> currentOccupants = new List<Actor>();

	// Token: 0x040008C2 RID: 2242
	public DesignStylePreset designStyle;

	// Token: 0x040008C3 RID: 2243
	public List<ArtPreset> artPieces = new List<ArtPreset>();

	// Token: 0x040008C4 RID: 2244
	public bool placedKey;

	// Token: 0x040008C5 RID: 2245
	public List<Interactable> securityCameras = new List<Interactable>();

	// Token: 0x040008C6 RID: 2246
	public List<Interactable> stacks = new List<Interactable>();

	// Token: 0x040008C7 RID: 2247
	public List<Telephone> telephones = new List<Telephone>();

	// Token: 0x040008C8 RID: 2248
	public List<Interactable> resetBehaviourObjects = new List<Interactable>();

	// Token: 0x040008C9 RID: 2249
	public Dictionary<FurnitureClass.OwnershipClass, Dictionary<FurnitureLocation, List<Human>>> furnitureBelongsTo = new Dictionary<FurnitureClass.OwnershipClass, Dictionary<FurnitureLocation, List<Human>>>();

	// Token: 0x040008CA RID: 2250
	[Header("AI Navigation")]
	public List<NewNode.NodeAccess> entrances = new List<NewNode.NodeAccess>();

	// Token: 0x040008CB RID: 2251
	[NonSerialized]
	public NewNode.NodeAccess streetAccess;

	// Token: 0x040008CC RID: 2252
	public NewNode anchorNode;

	// Token: 0x040008CD RID: 2253
	public Dictionary<AIActionPreset, List<Interactable>> actionReference = new Dictionary<AIActionPreset, List<Interactable>>();

	// Token: 0x040008CE RID: 2254
	public Dictionary<AIActionPreset, List<Interactable>> nearestPublicActionReference = new Dictionary<AIActionPreset, List<Interactable>>();

	// Token: 0x040008CF RID: 2255
	public Dictionary<Actor, NewGameLocation.TrespassEscalation> escalation = new Dictionary<Actor, NewGameLocation.TrespassEscalation>();

	// Token: 0x040008D0 RID: 2256
	public float playerLoiteringTimer;

	// Token: 0x040008D1 RID: 2257
	[Header("Evidence")]
	[NonSerialized]
	public EvidenceLocation evidenceEntry;

	// Token: 0x040008D2 RID: 2258
	public List<NewGameLocation.ObjectPlace> objectsToPlace = new List<NewGameLocation.ObjectPlace>();

	// Token: 0x040008D3 RID: 2259
	public bool objectPoolPlaced;

	// Token: 0x0200012D RID: 301
	[Serializable]
	public class TrespassEscalation
	{
		// Token: 0x040008D4 RID: 2260
		public int actor;

		// Token: 0x040008D5 RID: 2261
		public bool isPlayer;

		// Token: 0x040008D6 RID: 2262
		public float lastEscalationCheck;

		// Token: 0x040008D7 RID: 2263
		public float timeEscalation;
	}

	// Token: 0x0200012E RID: 302
	public class ObjectPlacement
	{
		// Token: 0x040008D8 RID: 2264
		public FurniturePreset.SubObject location;

		// Token: 0x040008D9 RID: 2265
		public FurnitureLocation furnParent;

		// Token: 0x040008DA RID: 2266
		public Interactable existing;

		// Token: 0x040008DB RID: 2267
		public Interactable subSpawn;
	}

	// Token: 0x0200012F RID: 303
	public class ObjectPlace
	{
		// Token: 0x040008DC RID: 2268
		public InteractablePreset interactable;

		// Token: 0x040008DD RID: 2269
		public Human belongsTo;

		// Token: 0x040008DE RID: 2270
		public Human writer;

		// Token: 0x040008DF RID: 2271
		public Human receiver;

		// Token: 0x040008E0 RID: 2272
		public List<Interactable.Passed> passedVars;

		// Token: 0x040008E1 RID: 2273
		public int security;

		// Token: 0x040008E2 RID: 2274
		public InteractablePreset.OwnedPlacementRule ownership;

		// Token: 0x040008E3 RID: 2275
		public int priority;

		// Token: 0x040008E4 RID: 2276
		public object passedObject;

		// Token: 0x040008E5 RID: 2277
		public HashSet<NewRoom> dontPlaceInRooms;
	}

	// Token: 0x02000130 RID: 304
	public class Placement
	{
		// Token: 0x040008E6 RID: 2278
		public NewRoom room;

		// Token: 0x040008E7 RID: 2279
		public FurnitureLocation furniture;

		// Token: 0x040008E8 RID: 2280
		public SubObjectClassPreset placementClass;

		// Token: 0x040008E9 RID: 2281
		public FurniturePreset.SubObject subObject;

		// Token: 0x040008EA RID: 2282
		public Interactable subSpawn;

		// Token: 0x040008EB RID: 2283
		public float rank;
	}
}
