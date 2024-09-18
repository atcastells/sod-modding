using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200036D RID: 877
public class NewNode
{
	// Token: 0x060013C1 RID: 5057 RVA: 0x0011AF60 File Offset: 0x00119160
	public void Setup(NewTile newTile, NewGameLocation newGameLoc, Vector2Int newLocalCoord)
	{
		newTile.AddNewNode(this);
		newGameLoc.AddNewNode(this);
		this.gameLocation.nullRoom.AddNewNode(this);
		this.localTileCoord = newLocalCoord;
		this.floorCoord = new Vector2Int(this.tile.floorCoord.x * CityControls.Instance.nodeMultiplier + this.localTileCoord.x, this.tile.floorCoord.y * CityControls.Instance.nodeMultiplier + this.localTileCoord.y);
		if (this.floor != null)
		{
			this.floorHeight = this.floor.defaultFloorHeight;
			if (this.room.roomType.overrideFloorHeight)
			{
				this.SetFloorHeight(this.room.roomType.floorHeight);
			}
			else
			{
				this.SetFloorHeight(this.floor.defaultFloorHeight);
			}
			if (!this.floor.nodeMap.ContainsKey(this.floorCoord))
			{
				this.floor.nodeMap.Add(this.floorCoord, this);
			}
		}
		this.nodeCoord = new Vector3Int(newTile.globalTileCoord.x * CityControls.Instance.nodeMultiplier + newLocalCoord.x, newTile.globalTileCoord.y * CityControls.Instance.nodeMultiplier + newLocalCoord.y, newTile.globalTileCoord.z);
		if (!PathFinder.Instance.nodeMap.ContainsKey(this.nodeCoord))
		{
			PathFinder.Instance.nodeMap.Add(this.nodeCoord, this);
			PathFinder.Instance.nodeRangeX.x = Mathf.Min((float)this.nodeCoord.x, PathFinder.Instance.nodeRangeX.x);
			PathFinder.Instance.nodeRangeX.y = Mathf.Max((float)this.nodeCoord.x, PathFinder.Instance.nodeRangeX.y);
			PathFinder.Instance.nodeRangeY.x = Mathf.Min((float)this.nodeCoord.y, PathFinder.Instance.nodeRangeY.x);
			PathFinder.Instance.nodeRangeY.y = Mathf.Max((float)this.nodeCoord.y, PathFinder.Instance.nodeRangeY.y);
			PathFinder.Instance.nodeRangeZ.x = Mathf.Min((float)this.nodeCoord.z, PathFinder.Instance.nodeRangeZ.x);
			PathFinder.Instance.nodeRangeZ.y = Mathf.Max((float)this.nodeCoord.z, PathFinder.Instance.nodeRangeZ.y);
		}
		this.position = CityData.Instance.NodeToRealpos(this.nodeCoord);
		this.position = new Vector3(this.position.x, this.position.y + (float)this.floorHeight * 0.1f, this.position.z);
		if (SessionData.Instance.isFloorEdit)
		{
			if (this.physicalObject == null)
			{
				this.physicalObject = Toolbox.Instance.SpawnObject(PrefabControls.Instance.node, this.room.transform);
				this.physicalObject.transform.position = this.position;
			}
			BoxCollider boxCollider = this.physicalObject.AddComponent<BoxCollider>();
			boxCollider.isTrigger = true;
			boxCollider.size = new Vector3(PathFinder.Instance.tileSize.x / (float)CityControls.Instance.nodeMultiplier, 0.1f, PathFinder.Instance.tileSize.x / (float)CityControls.Instance.nodeMultiplier);
		}
		if (this.tile.isEdge || this.tile.isOutside)
		{
			this.SetFloorType(NewNode.FloorTileType.none);
			return;
		}
		this.SetFloorType(NewNode.FloorTileType.floorAndCeiling);
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x0011B348 File Offset: 0x00119548
	public Vector3 TransformPoint(Vector3 localPos)
	{
		return this.position + localPos;
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x0011B356 File Offset: 0x00119556
	public Vector3 InverseTransformPoint(Vector3 worldPos)
	{
		return worldPos - this.position;
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x0011B364 File Offset: 0x00119564
	public void Load(CitySaveData.NodeCitySave data, NewRoom newRoom)
	{
		newRoom.AddNewNode(this);
		this.floorCoord = data.fc;
		this.localTileCoord = data.ltc;
		this.nodeCoord = data.nc;
		this.SetFloorHeight(data.fh);
		this.floorType = data.ft;
		this.isObstacle = data.io;
		this.isOutside = data.ios;
		this.stairwellLowerLink = data.sll;
		this.stairwellUpperLink = data.sul;
		this.forcedRoomRef = data.frr;
		this.SetCeilingVent(data.cav);
		this.SetFloorVent(data.fav);
		this.allowNewFurniture = data.anf;
		foreach (CitySaveData.WallCitySave data2 in data.w)
		{
			new NewWall().Load(data2, this);
		}
		if (data.fr != null && data.fr.Length > 0)
		{
			string[] array = this.forcedRoomRef.Split('.', 0);
			string text = array[array.Length - 1];
			RoomConfiguration roomConfiguration = null;
			if (Toolbox.Instance.LoadDataFromResources<RoomConfiguration>(text, out roomConfiguration))
			{
				this.SetForcedRoom(roomConfiguration);
			}
		}
		if (newRoom.preset.forceOutside == RoomConfiguration.OutsideSetting.forceInside)
		{
			this.SetAsOutside(false);
			return;
		}
		if (newRoom.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)
		{
			this.SetAsOutside(true);
		}
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x0011B4D0 File Offset: 0x001196D0
	public void AddNewWall(NewWall newWall)
	{
		if (!this.walls.Contains(newWall))
		{
			if (newWall.node != null)
			{
				newWall.node.RemoveWall(newWall);
			}
			this.walls.Add(newWall);
			this.wallDict.Add(newWall.wallOffset, newWall);
			newWall.node = this;
			if (this.preventEntrances != null && this.preventEntrances.Contains(newWall.wallOffset))
			{
				newWall.preventEntrance = true;
				return;
			}
			newWall.preventEntrance = false;
		}
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x0011B54E File Offset: 0x0011974E
	public void RemoveWall(NewWall newWall)
	{
		if (this.walls.Contains(newWall))
		{
			this.walls.Remove(newWall);
			this.wallDict.Remove(newWall.wallOffset);
			newWall.node = null;
		}
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x0011B584 File Offset: 0x00119784
	public void SpawnFloor(bool prepForCombinedMeshes)
	{
		Toolbox.Instance.DestroyObject(this.spawnedFloor);
		if (this.floorType == NewNode.FloorTileType.floorAndCeiling || this.floorType == NewNode.FloorTileType.floorOnly)
		{
			MeshRenderer meshRenderer = null;
			if (this.tile.useOptimizedFloor && this.tile.anchorNode == this)
			{
				this.floorPrefab = PrefabControls.Instance.floorTile;
				this.spawnedFloor = Toolbox.Instance.SpawnObject(this.floorPrefab, this.room.transform);
				this.spawnedFloor.transform.position = this.position;
				this.spawnedFloor.transform.localEulerAngles = new Vector3(0f, (float)this.tile.rotation, 0f);
				if (!prepForCombinedMeshes)
				{
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedFloor.GetComponent<MeshRenderer>();
					}
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedFloor.AddComponent<MeshRenderer>();
					}
					MaterialsController.Instance.SetMaterialGroup(this.spawnedFloor, this.room.floorMaterial, this.room.floorMatKey, false, meshRenderer);
				}
			}
			else if (!this.tile.useOptimizedFloor)
			{
				if (this.floorAirVent)
				{
					this.floorPrefab = PrefabControls.Instance.smallFloorTileVent;
				}
				else
				{
					this.floorPrefab = PrefabControls.Instance.smallFloorTile;
				}
				this.spawnedFloor = Toolbox.Instance.SpawnObject(this.floorPrefab, this.room.transform);
				this.spawnedFloor.transform.position = this.position;
				this.spawnedFloor.transform.localEulerAngles = new Vector3(0f, (float)this.tile.rotation, 0f);
				if (!prepForCombinedMeshes)
				{
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedFloor.GetComponent<MeshRenderer>();
					}
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedFloor.AddComponent<MeshRenderer>();
					}
					MaterialsController.Instance.SetMaterialGroup(this.spawnedFloor, this.room.floorMaterial, this.room.floorMatKey, false, meshRenderer);
				}
			}
			if (this.spawnedFloor != null)
			{
				if (SessionData.Instance.isFloorEdit)
				{
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedFloor.GetComponent<MeshRenderer>();
					}
					if (FloorEditController.Instance.displayMode == FloorEditController.EditorDisplayMode.displayAddressDesignation && meshRenderer != null)
					{
						meshRenderer.material = FloorEditController.Instance.adddressDesignationMaterial;
						meshRenderer.material.SetColor("_UnlitColor", this.gameLocation.thisAsAddress.editorColour);
					}
				}
				if (!prepForCombinedMeshes)
				{
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedFloor.GetComponent<MeshRenderer>();
					}
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedFloor.AddComponent<MeshRenderer>();
					}
					if (meshRenderer != null)
					{
						bool includeStreetLighting = this.room.IsOutside();
						if (this.room.preset != null && this.room.preset.forceStreetLightLayer)
						{
							includeStreetLighting = this.room.preset.forceStreetLightLayer;
						}
						Toolbox.Instance.SetLightLayer(meshRenderer, this.building, includeStreetLighting);
					}
				}
			}
		}
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x0011B8A0 File Offset: 0x00119AA0
	public void SpawnCeiling(bool prepForCombinedMeshes)
	{
		Toolbox.Instance.DestroyObject(this.spawnedCeiling);
		if (this.floorType == NewNode.FloorTileType.floorAndCeiling || this.floorType == NewNode.FloorTileType.CeilingOnly)
		{
			MeshRenderer meshRenderer = null;
			if (this.tile.useOptimizedCeiling && this.tile.anchorNode == this)
			{
				this.ceilingPrefab = PrefabControls.Instance.ceilingTile;
				this.spawnedCeiling = Toolbox.Instance.SpawnObject(this.ceilingPrefab, this.room.transform);
				this.spawnedCeiling.transform.position = this.position + new Vector3(this.spawnedCeiling.transform.localPosition.x, (float)this.floor.defaultCeilingHeight * 0.1f - (float)this.floorHeight * 0.1f, this.spawnedCeiling.transform.localPosition.z);
				if (!prepForCombinedMeshes)
				{
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedCeiling.GetComponent<MeshRenderer>();
					}
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedCeiling.AddComponent<MeshRenderer>();
					}
					bool flag = false;
					if (this.room.preset != null)
					{
						flag = this.room.preset.boostCeilingEmission;
					}
					this.room.ceilingMat = MaterialsController.Instance.SetMaterialGroup(this.spawnedCeiling, this.room.ceilingMaterial, this.room.ceilingMatKey, flag, meshRenderer);
					this.room.uniqueCeilingMaterial = flag;
					if (SessionData.Instance.isFloorEdit && !SessionData.Instance.play && meshRenderer != null)
					{
						meshRenderer.shadowCastingMode = 0;
					}
					bool includeStreetLighting = this.room.IsOutside();
					if (this.room.preset != null && this.room.preset.forceStreetLightLayer)
					{
						includeStreetLighting = this.room.preset.forceStreetLightLayer;
					}
					Toolbox.Instance.SetLightLayer(meshRenderer, this.building, includeStreetLighting);
					return;
				}
			}
			else if (!this.tile.useOptimizedCeiling)
			{
				if (this.ceilingAirVent)
				{
					this.ceilingPrefab = PrefabControls.Instance.smallCeilingTileVent;
				}
				else
				{
					this.ceilingPrefab = PrefabControls.Instance.smallCeilingTile;
				}
				this.spawnedCeiling = Toolbox.Instance.SpawnObject(this.ceilingPrefab, this.room.transform);
				this.spawnedCeiling.transform.position = this.position + new Vector3(this.spawnedCeiling.transform.localPosition.x, (float)this.floor.defaultCeilingHeight * 0.1f - (float)this.floorHeight * 0.1f, this.spawnedCeiling.transform.localPosition.z);
				if (!prepForCombinedMeshes)
				{
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedCeiling.GetComponent<MeshRenderer>();
					}
					if (meshRenderer == null)
					{
						meshRenderer = this.spawnedCeiling.AddComponent<MeshRenderer>();
					}
					bool flag2 = false;
					if (this.room.preset != null)
					{
						flag2 = this.room.preset.boostCeilingEmission;
					}
					this.room.ceilingMat = MaterialsController.Instance.SetMaterialGroup(this.spawnedCeiling, this.room.ceilingMaterial, this.room.ceilingMatKey, flag2, meshRenderer);
					this.room.uniqueCeilingMaterial = flag2;
					if (SessionData.Instance.isFloorEdit && !SessionData.Instance.play && meshRenderer != null)
					{
						meshRenderer.shadowCastingMode = 3;
					}
					bool includeStreetLighting2 = this.room.IsOutside();
					if (this.room.preset != null && this.room.preset.forceStreetLightLayer)
					{
						includeStreetLighting2 = this.room.preset.forceStreetLightLayer;
					}
					Toolbox.Instance.SetLightLayer(meshRenderer, this.building, includeStreetLighting2);
				}
			}
		}
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x0011BC7C File Offset: 0x00119E7C
	public void SetFloorType(NewNode.FloorTileType newType)
	{
		this.floorType = newType;
		if (this.floorType == NewNode.FloorTileType.none)
		{
			if (this.room.floor != null)
			{
				this.room.floor.outsideAddress.AddNewNode(this);
			}
		}
		else if (this.room.floor != null && (this.gameLocation == this.room.floor.outsideAddress || this.gameLocation.name.Length <= 0))
		{
			this.room.floor.lobbyAddress.AddNewNode(this);
		}
		if (SessionData.Instance.isFloorEdit || SessionData.Instance.isTestScene || CityConstructor.Instance.generateNew)
		{
			GenerationController.Instance.UpdateGeometryFloor(this.floor, "NewNode");
		}
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x0011BD55 File Offset: 0x00119F55
	public void SetAsObstacle(bool val)
	{
		this.isObstacle = val;
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x0011BD5E File Offset: 0x00119F5E
	public void SetAsOutside(bool val)
	{
		this.isOutside = val;
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x0011BD68 File Offset: 0x00119F68
	public void AddAccessToOtherNode(NewNode newNode, bool twoWay = true, bool forceAccessType = false, NewNode.NodeAccess.AccessType forcedAccessType = NewNode.NodeAccess.AccessType.adjacent, bool forceWalkable = false)
	{
		if (newNode == this)
		{
			return;
		}
		if (newNode.isObstacle && newNode.isObstacle)
		{
			return;
		}
		float num = Vector3.Distance(this.position, newNode.position) * newNode.nodeWeightMultiplier;
		NewDoor newDoorway = null;
		NewWall newWall = this.walls.Find((NewWall item) => item.otherWall != null && item.otherWall.node == newNode);
		if (newWall != null)
		{
			newDoorway = newWall.parentWall.door;
			newWall = newWall.parentWall;
		}
		if (!this.accessToOtherNodes.ContainsKey(newNode))
		{
			NewNode.NodeAccess nodeAccess = new NewNode.NodeAccess(this, newNode, newWall, newDoorway, forceAccessType, forcedAccessType, forceWalkable);
			this.accessToOtherNodes.Add(newNode, nodeAccess);
		}
		else if (num < this.accessToOtherNodes[newNode].weight)
		{
			NewNode.NodeAccess nodeAccess2 = new NewNode.NodeAccess(this, newNode, newWall, newDoorway, forceAccessType, forcedAccessType, forceWalkable);
			this.accessToOtherNodes[newNode] = nodeAccess2;
		}
		if (newNode.room != this.room)
		{
			this.room.AddEntrance(this, newNode, forceAccessType, forcedAccessType, forceWalkable);
		}
		if (twoWay)
		{
			newDoorway = null;
			if (newWall != null)
			{
				newWall = newWall.otherWall;
				newDoorway = newWall.parentWall.door;
				newWall = newWall.parentWall;
			}
			if (!newNode.accessToOtherNodes.ContainsKey(this))
			{
				NewNode.NodeAccess nodeAccess3 = new NewNode.NodeAccess(newNode, this, newWall, newDoorway, forceAccessType, forcedAccessType, forceWalkable);
				newNode.accessToOtherNodes.Add(this, nodeAccess3);
			}
			else if (num < newNode.accessToOtherNodes[this].weight)
			{
				NewNode.NodeAccess nodeAccess4 = new NewNode.NodeAccess(newNode, this, newWall, newDoorway, forceAccessType, forcedAccessType, forceWalkable);
				newNode.accessToOtherNodes[this] = nodeAccess4;
			}
			if (newNode.room != this.room)
			{
				newNode.room.AddEntrance(newNode, this, forceAccessType, forcedAccessType, forceWalkable);
			}
		}
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x0011BF80 File Offset: 0x0011A180
	public void RemoveAccessToOtherNode(NewNode newNode, bool twoWay = true)
	{
		if (this.accessToOtherNodes.ContainsKey(newNode))
		{
			this.accessToOtherNodes[newNode].walkingAccess = false;
			this.accessToOtherNodes.Remove(newNode);
		}
		if (newNode.room != this.room)
		{
			NewNode.NodeAccess nodeAccess = this.room.entrances.Find((NewNode.NodeAccess item) => item.fromNode == this && item.toNode == newNode);
			if (nodeAccess != null)
			{
				nodeAccess.walkingAccess = false;
			}
			NewNode.NodeAccess nodeAccess2 = newNode.room.entrances.Find((NewNode.NodeAccess item) => item.fromNode == newNode && item.toNode == this);
			if (nodeAccess2 != null)
			{
				nodeAccess2.walkingAccess = false;
			}
		}
		bool flag = true;
		foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in this.accessToOtherNodes)
		{
			if (keyValuePair.Value.walkingAccess)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			this.isInaccessable = true;
			this.noAccess = true;
		}
		if (twoWay)
		{
			if (newNode.accessToOtherNodes.ContainsKey(this))
			{
				newNode.accessToOtherNodes[this].walkingAccess = false;
				newNode.accessToOtherNodes.Remove(this);
			}
			flag = true;
			foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair2 in newNode.accessToOtherNodes)
			{
				if (keyValuePair2.Value.walkingAccess)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				newNode.isInaccessable = true;
				newNode.noAccess = true;
			}
		}
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x0011C164 File Offset: 0x0011A364
	public void SetForcedRoom(RoomConfiguration newRoom)
	{
		this.forcedRoom = newRoom;
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x0011C170 File Offset: 0x0011A370
	public void AddInteractable(Interactable newInteractable)
	{
		if (newInteractable == null)
		{
			return;
		}
		if (!this.interactables.Contains(newInteractable))
		{
			this.interactables.Add(newInteractable);
			if (newInteractable.preset.specialCaseFlag != InteractablePreset.SpecialCase.none)
			{
				if (!this.room.specialCaseInteractables.ContainsKey(newInteractable.preset.specialCaseFlag))
				{
					this.room.specialCaseInteractables.Add(newInteractable.preset.specialCaseFlag, new List<Interactable>());
				}
				if (!this.room.specialCaseInteractables[newInteractable.preset.specialCaseFlag].Contains(newInteractable))
				{
					this.room.specialCaseInteractables[newInteractable.preset.specialCaseFlag].Add(newInteractable);
				}
			}
			foreach (InteractablePreset.InteractionAction interactionAction in newInteractable.preset.GetActions(0))
			{
				if (interactionAction == null || interactionAction.action == null)
				{
					Game.Log("Misc Error: Null action detected in " + newInteractable.preset.name, 2);
				}
				else if (interactionAction.usableByAI)
				{
					if (!this.room.actionReference.ContainsKey(interactionAction.action))
					{
						this.room.actionReference.Add(interactionAction.action, new List<Interactable>());
					}
					this.room.actionReference[interactionAction.action].Add(newInteractable);
					this.room.actionReference[interactionAction.action].Sort((Interactable p1, Interactable p2) => p2.preset.AIPriority.CompareTo(p1.preset.AIPriority));
					if (Game.Instance.devMode && Game.Instance.collectDebugData)
					{
						this.room.debugAddActions.Add(string.Concat(new string[]
						{
							"Add: ",
							interactionAction.action.name,
							": ",
							newInteractable.name,
							" (",
							this.room.actionReference.Count.ToString(),
							")"
						}));
					}
					if (!this.gameLocation.actionReference.ContainsKey(interactionAction.action))
					{
						this.gameLocation.actionReference.Add(interactionAction.action, new List<Interactable>());
					}
					this.gameLocation.actionReference[interactionAction.action].Add(newInteractable);
					this.gameLocation.actionReference[interactionAction.action].Sort((Interactable p1, Interactable p2) => p2.preset.AIPriority.CompareTo(p1.preset.AIPriority));
				}
			}
			if (this.room.geometryLoaded)
			{
				if (newInteractable.furnitureParent != null)
				{
					if (newInteractable.furnitureParent.spawnedObject != null)
					{
						newInteractable.LoadInteractableToWorld(false, false);
					}
				}
				else
				{
					newInteractable.LoadInteractableToWorld(false, false);
				}
			}
			if (newInteractable.preset.isHeatSource)
			{
				this.room.heatSources.Add(newInteractable);
			}
			if (newInteractable.preset.resetBehaviour.Count > 0 && !this.room.gameLocation.resetBehaviourObjects.Contains(newInteractable))
			{
				this.room.gameLocation.resetBehaviourObjects.Add(newInteractable);
			}
		}
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x0011C4F4 File Offset: 0x0011A6F4
	public void RemoveInteractable(Interactable newInteractable)
	{
		if (newInteractable.preset.specialCaseFlag != InteractablePreset.SpecialCase.none && this.room.specialCaseInteractables.ContainsKey(newInteractable.preset.specialCaseFlag))
		{
			this.room.specialCaseInteractables[newInteractable.preset.specialCaseFlag].Remove(newInteractable);
		}
		foreach (InteractablePreset.InteractionAction interactionAction in newInteractable.preset.GetActions(0))
		{
			if (interactionAction.usableByAI)
			{
				if (this.room.actionReference.ContainsKey(interactionAction.action))
				{
					this.room.actionReference[interactionAction.action].Remove(newInteractable);
				}
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					this.room.debugAddActions.Add(string.Concat(new string[]
					{
						"Remove: ",
						interactionAction.action.name,
						": ",
						newInteractable.name,
						" (",
						this.room.actionReference.Count.ToString(),
						")"
					}));
				}
				if (this.gameLocation.actionReference.ContainsKey(interactionAction.action))
				{
					this.gameLocation.actionReference[interactionAction.action].Remove(newInteractable);
				}
			}
		}
		this.interactables.Remove(newInteractable);
		if (newInteractable.preset.isHeatSource)
		{
			this.room.heatSources.Remove(newInteractable);
		}
		if (newInteractable.preset.resetBehaviour.Count > 0)
		{
			this.room.gameLocation.resetBehaviourObjects.Remove(newInteractable);
		}
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x0011C6F4 File Offset: 0x0011A8F4
	[Button("Teleport Player", 0)]
	public void DebugTeleportPlayerToLocation()
	{
		Player.Instance.Teleport(this, null, true, false);
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x0011C704 File Offset: 0x0011A904
	public void SetFloorHeight(int val)
	{
		this.floorHeight = val;
		this.position = new Vector3(this.position.x, this.tile.position.y + (float)this.floorHeight * 0.1f, this.position.z);
		foreach (NewWall newWall in this.walls)
		{
			newWall.position = new Vector3(newWall.position.x, this.position.y + (float)this.floorHeight * -0.1f, newWall.position.z);
			if (newWall.physicalObject != null)
			{
				newWall.physicalObject.transform.position = newWall.position;
				FloorEditWallDetector component = newWall.physicalObject.GetComponent<FloorEditWallDetector>();
				component.debugNodePosition = this.position;
				component.debugFloorHeight = this.floorHeight;
			}
			if (newWall.spawnedWall != null)
			{
				newWall.spawnedWall.transform.position = newWall.position;
			}
		}
		if (this.spawnedCeiling != null && this.floor != null)
		{
			this.spawnedCeiling.transform.position = new Vector3(this.spawnedCeiling.transform.position.x, this.position.y + ((float)this.floor.defaultCeilingHeight * 0.1f - (float)this.floorHeight * 0.1f), this.spawnedCeiling.transform.position.z);
		}
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0011C8C8 File Offset: 0x0011AAC8
	public void AddFurniture(FurnitureLocation newFurn)
	{
		if (!this.individualFurniture.Contains(newFurn))
		{
			this.individualFurniture.Add(newFurn);
			if (newFurn.furniture.classes.Count > 0 && !newFurn.furniture.classes[0].noBlocking && newFurn.furniture.classes[0].occupiesTile)
			{
				this.nodeWeightMultiplier += 1f;
			}
		}
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x0011C944 File Offset: 0x0011AB44
	public void ResetFurniture()
	{
		this.individualFurniture = new List<FurnitureLocation>();
		this.nodeWeightMultiplier = 1f;
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x0011C95C File Offset: 0x0011AB5C
	public void SetAllowNewFurniture(bool val)
	{
		this.allowNewFurniture = val;
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x0011C965 File Offset: 0x0011AB65
	public void AddToNodeWeightMultiplier(float val)
	{
		this.nodeWeightMultiplier += val;
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x0011C978 File Offset: 0x0011AB78
	public CitySaveData.NodeCitySave GenerateSaveData()
	{
		CitySaveData.NodeCitySave nodeCitySave = new CitySaveData.NodeCitySave();
		nodeCitySave.fc = this.floorCoord;
		nodeCitySave.ltc = this.localTileCoord;
		nodeCitySave.nc = Vector3Int.RoundToInt(this.nodeCoord);
		foreach (NewWall newWall in this.walls)
		{
			nodeCitySave.w.Add(newWall.GenerateSaveData());
		}
		nodeCitySave.fh = this.floorHeight;
		nodeCitySave.ft = this.floorType;
		nodeCitySave.io = this.isObstacle;
		nodeCitySave.ios = this.isOutside;
		nodeCitySave.sll = this.stairwellLowerLink;
		nodeCitySave.sul = this.stairwellUpperLink;
		if (this.forcedRoom != null)
		{
			nodeCitySave.fr = this.forcedRoom.name;
		}
		nodeCitySave.frr = this.forcedRoomRef;
		nodeCitySave.anf = this.allowNewFurniture;
		nodeCitySave.cav = this.ceilingAirVent;
		nodeCitySave.fav = this.floorAirVent;
		return nodeCitySave;
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x0011CAA4 File Offset: 0x0011ACA4
	public bool AddHumanTraveller(Actor newActor, Interactable.UsagePoint usagePoint, out Vector3 usePosition, bool useRandomNodeSublocation = false)
	{
		if (newActor.ai != null)
		{
			NewAIController ai = newActor.ai;
			string text = "> Add ";
			string text2 = newActor.name;
			string text3 = " to node: ";
			Vector3 vector = this.position;
			ai.DebugDestinationPosition(text + text2 + text3 + vector.ToString());
		}
		if (Game.Instance.collectDebugData)
		{
			string text4 = "> Add ";
			string text5 = newActor.name;
			string text6 = " to node: ";
			Vector3 vector = this.position;
			newActor.SelectedDebug(text4 + text5 + text6 + vector.ToString(), Actor.HumanDebug.movement);
		}
		if (usagePoint != null && newActor.ai != null)
		{
			if (newActor.ai != null)
			{
				newActor.ai.DebugDestinationPosition("...Given usage point from " + usagePoint.interactable.name);
			}
			if (Game.Instance.collectDebugData)
			{
				newActor.SelectedDebug("...Given usage point from " + usagePoint.interactable.name, Actor.HumanDebug.movement);
			}
		}
		if (newActor.reservedNodeSpace.Count > 0)
		{
			if (newActor.ai != null)
			{
				newActor.ai.DebugDestinationPosition("Clearing actor reserved space");
			}
			if (Game.Instance.collectDebugData)
			{
				newActor.SelectedDebug("Clearing actor reserved space", Actor.HumanDebug.movement);
			}
			newActor.RemoveReservedNodeSpace();
		}
		if ((usagePoint == null || usagePoint.node != this) && newActor.ai != null && newActor.ai.currentAction != null && newActor.ai.currentAction.path != null)
		{
			if (Game.Instance.collectDebugData)
			{
				newActor.ai.DebugDestinationPosition("AI is on path node " + newActor.ai.pathCursor.ToString() + "/" + newActor.ai.currentAction.path.accessList.Count.ToString());
				newActor.SelectedDebug("AI is on path node " + newActor.ai.pathCursor.ToString() + "/" + newActor.ai.currentAction.path.accessList.Count.ToString(), Actor.HumanDebug.movement);
			}
			if (newActor.ai.pathCursor < newActor.ai.currentAction.path.accessList.Count - 1)
			{
				NewNode.NodeAccess nodeAccess = newActor.ai.currentAction.path.accessList[newActor.ai.pathCursor + 1];
				if (nodeAccess.door != null && nodeAccess.door.isClosed)
				{
					usagePoint = nodeAccess.door.handleInteractable.usagePoint;
					if (Game.Instance.collectDebugData)
					{
						newActor.SelectedDebug("The next usage point has been overridden by a door at path position " + (newActor.ai.pathCursor + 1).ToString() + "/" + newActor.ai.currentAction.path.accessList.Count.ToString(), Actor.HumanDebug.movement);
						newActor.ai.DebugDestinationPosition("The next usage point has been overridden by a door at path position " + (newActor.ai.pathCursor + 1).ToString() + "/" + newActor.ai.currentAction.path.accessList.Count.ToString());
					}
				}
			}
		}
		if (usagePoint != null && newActor.ai != null && newActor.ai.currentAction != null && newActor.ai.currentAction.path != null && newActor.ai.pathCursor >= newActor.ai.currentAction.path.accessList.Count)
		{
			usePosition = usagePoint.GetUsageWorldPosition(this.position, newActor);
			if (Game.Instance.collectDebugData)
			{
				NewAIController ai2 = newActor.ai;
				string text7 = "Reached end of path, get usage position of: ";
				Vector3 vector = usePosition;
				ai2.DebugDestinationPosition(text7 + vector.ToString());
				string text8 = "Reached end of path, get usage position of: ";
				vector = usePosition;
				newActor.SelectedDebug(text8 + vector.ToString(), Actor.HumanDebug.movement);
			}
			return true;
		}
		if (this.defaultSpace != null && !useRandomNodeSublocation && (this.defaultSpace.occ == NewNode.NodeSpaceOccupancy.empty || this.defaultSpace.occupier == newActor))
		{
			if (Game.Instance.collectDebugData)
			{
				if (newActor.ai != null)
				{
					newActor.ai.DebugDestinationPosition("There is nobody else on this node, so use the centre position");
				}
				newActor.SelectedDebug("There is nobody else on this node, so use the centre position", Actor.HumanDebug.movement);
			}
			usePosition = this.defaultSpace.position;
			this.defaultSpace.SetOccuppier(newActor, NewNode.NodeSpaceOccupancy.reserved);
			return true;
		}
		List<NewNode.NodeSpace> list = new List<NewNode.NodeSpace>();
		foreach (KeyValuePair<Vector3, NewNode.NodeSpace> keyValuePair in this.walkableNodeSpace)
		{
			if (keyValuePair.Value.occ == NewNode.NodeSpaceOccupancy.empty || !(keyValuePair.Value.occupier != newActor))
			{
				int num = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
				{
					default(int),
					7,
					29
				});
				RaycastHit raycastHit;
				if (!Physics.SphereCast(keyValuePair.Value.position + new Vector3(0f, -0.35f, 0f), 0.34f, new Vector3(0f, 1f, 0f), ref raycastHit, 2.5f, num, 1) || (!this.detectGeometry && !raycastHit.transform.parent.CompareTag("Citizen")))
				{
					list.Add(keyValuePair.Value);
				}
			}
		}
		if (list.Count <= 0)
		{
			if (Game.Instance.collectDebugData)
			{
				if (newActor.ai != null)
				{
					newActor.ai.DebugDestinationPosition("There is no room left on this node! Default to centre position but return false.");
				}
				newActor.SelectedDebug("There is no room left on this node! Default to centre position but return false.", Actor.HumanDebug.movement);
			}
			usePosition = this.position;
			if (this.defaultSpace != null)
			{
				usePosition = this.defaultSpace.position;
				this.defaultSpace.SetOccuppier(newActor, NewNode.NodeSpaceOccupancy.reserved);
			}
			return false;
		}
		if (list.Count > 1)
		{
			float num2 = 9999f;
			NewNode.NodeSpace nodeSpace = list[0];
			for (int i = 0; i < list.Count; i++)
			{
				NewNode.NodeSpace nodeSpace2 = list[i];
				float num3;
				if (useRandomNodeSublocation)
				{
					num3 = Toolbox.Instance.Rand(0f, 1f, false);
				}
				else
				{
					num3 = Vector3.Distance(newActor.transform.position, nodeSpace2.position);
				}
				if (num3 < num2)
				{
					num2 = num3;
					nodeSpace = nodeSpace2;
				}
			}
			usePosition = nodeSpace.position;
			nodeSpace.SetOccuppier(newActor, NewNode.NodeSpaceOccupancy.reserved);
			if (Game.Instance.collectDebugData)
			{
				Vector3 vector;
				if (newActor.ai != null)
				{
					NewAIController ai3 = newActor.ai;
					string text9 = "There are obstacles on this node, but space was found. New use position is: ";
					vector = usePosition;
					ai3.DebugDestinationPosition(text9 + vector.ToString());
				}
				string text10 = "There are obstacles on this node, but space was found. New use position is: ";
				vector = usePosition;
				newActor.SelectedDebug(text10 + vector.ToString(), Actor.HumanDebug.movement);
			}
			return true;
		}
		usePosition = list[0].position;
		list[0].SetOccuppier(newActor, NewNode.NodeSpaceOccupancy.reserved);
		if (Game.Instance.collectDebugData)
		{
			Vector3 vector;
			if (newActor.ai != null)
			{
				NewAIController ai4 = newActor.ai;
				string text11 = "There is 1 obstacle on this node, but space was found. New use position is: ";
				vector = usePosition;
				ai4.DebugDestinationPosition(text11 + vector.ToString());
			}
			string text12 = "There is 1 obstacle on this node, but space was found. New use position is: ";
			vector = usePosition;
			newActor.SelectedDebug(text12 + vector.ToString(), Actor.HumanDebug.movement);
		}
		return true;
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x0011D25C File Offset: 0x0011B45C
	public void UpdateWalkableSublocations()
	{
		List<Vector3> list = new List<Vector3>();
		this.detectGeometry = true;
		bool flag = false;
		foreach (FurnitureLocation furnitureLocation in this.individualFurniture)
		{
			foreach (FurnitureClass furnitureClass in furnitureLocation.furnitureClasses)
			{
				if (furnitureClass.ignoreGeometryInPhysicsCheck)
				{
					this.detectGeometry = false;
				}
				if (furnitureClass.blockDefaultSublocations)
				{
					flag = true;
				}
			}
			List<Vector3> list2 = null;
			if (furnitureLocation.sublocations.TryGetValue(this, ref list2))
			{
				list.AddRange(list2);
			}
		}
		if (list.Count <= 0 && !flag)
		{
			list.AddRange(CitizenControls.Instance.nodeLocalSubdivisions);
		}
		float num = float.PositiveInfinity;
		this.walkableNodeSpace.Clear();
		foreach (Vector3 vector in list)
		{
			if (!this.walkableNodeSpace.ContainsKey(vector))
			{
				this.walkableNodeSpace.Add(vector, new NewNode.NodeSpace
				{
					position = this.TransformPoint(vector),
					node = this
				});
			}
			else
			{
				this.walkableNodeSpace[vector].position = this.TransformPoint(vector);
			}
			float num2 = Vector3.Distance(this.position, this.walkableNodeSpace[vector].position);
			if (num2 < num)
			{
				num = num2;
				this.defaultSpace = this.walkableNodeSpace[vector];
			}
		}
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x0011D420 File Offset: 0x0011B620
	public void ClearTravellers()
	{
		foreach (KeyValuePair<Vector3, NewNode.NodeSpace> keyValuePair in this.walkableNodeSpace)
		{
			keyValuePair.Value.SetEmpty();
		}
		this.occupiedSpace.Clear();
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x0011D484 File Offset: 0x0011B684
	public void SetAsAudioSource(AudioEvent newEvent, Vector3 newOffset)
	{
		this.audioEvent = newEvent;
		this.audioOffset = newOffset;
		if (this.loop != null)
		{
			AudioController.Instance.StopSound(this.loop, AudioController.StopType.immediate, "Stop existing loop");
			this.loop = null;
		}
		if (this.audioEvent != null)
		{
			this.loop = AudioController.Instance.PlayWorldLoopingStatic(this.audioEvent, null, this, this.position + this.audioOffset, null, 1f, false, true, null, null);
		}
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x0011D508 File Offset: 0x0011B708
	public void SetCeilingVent(bool val)
	{
		this.ceilingAirVent = val;
		if (this.ceilingAirVent)
		{
			if (this.spawnedCeiling != null)
			{
				this.tile.SetFloorCeilingOptimization(false, true);
				return;
			}
			this.tile.SetFloorCeilingOptimization(false, false);
			return;
		}
		else
		{
			if (this.spawnedCeiling != null)
			{
				this.tile.SetFloorCeilingOptimization(this.tile.CanBeOptimized(), true);
				return;
			}
			this.tile.SetFloorCeilingOptimization(this.tile.CanBeOptimized(), false);
			return;
		}
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x0011D58C File Offset: 0x0011B78C
	public void SetFloorVent(bool val)
	{
		this.floorAirVent = val;
		if (this.floorAirVent)
		{
			if (this.spawnedFloor != null)
			{
				this.tile.SetFloorCeilingOptimization(false, true);
				return;
			}
			this.tile.SetFloorCeilingOptimization(false, false);
			return;
		}
		else
		{
			if (this.spawnedFloor != null)
			{
				this.tile.SetFloorCeilingOptimization(this.tile.CanBeOptimized(), true);
				return;
			}
			this.tile.SetFloorCeilingOptimization(this.tile.CanBeOptimized(), false);
			return;
		}
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x0011D60F File Offset: 0x0011B80F
	public bool HasValidFloor()
	{
		return this.floorType != NewNode.FloorTileType.CeilingOnly && this.floorType != NewNode.FloorTileType.none && this.floorType != NewNode.FloorTileType.noneButIndoors && !this.floorAirVent;
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x0011D636 File Offset: 0x0011B836
	public bool HasValidCeiling()
	{
		return this.floorType != NewNode.FloorTileType.floorOnly && this.floorType != NewNode.FloorTileType.none && this.floorType != NewNode.FloorTileType.noneButIndoors && !this.ceilingAirVent;
	}

	// Token: 0x04001816 RID: 6166
	[Header("Transform")]
	public string name;

	// Token: 0x04001817 RID: 6167
	public Vector3 position;

	// Token: 0x04001818 RID: 6168
	public GameObject physicalObject;

	// Token: 0x04001819 RID: 6169
	[Header("Location")]
	public NewBuilding building;

	// Token: 0x0400181A RID: 6170
	public NewFloor floor;

	// Token: 0x0400181B RID: 6171
	public NewGameLocation gameLocation;

	// Token: 0x0400181C RID: 6172
	public NewRoom room;

	// Token: 0x0400181D RID: 6173
	public NewTile tile;

	// Token: 0x0400181E RID: 6174
	[Space(5f)]
	public Vector2Int floorCoord;

	// Token: 0x0400181F RID: 6175
	public Vector2Int localTileCoord;

	// Token: 0x04001820 RID: 6176
	public Vector3Int nodeCoord;

	// Token: 0x04001821 RID: 6177
	[Header("Node Contents")]
	public List<NewWall> walls = new List<NewWall>();

	// Token: 0x04001822 RID: 6178
	public Dictionary<Vector2, NewWall> wallDict = new Dictionary<Vector2, NewWall>();

	// Token: 0x04001823 RID: 6179
	[Header("Details")]
	public int floorHeight;

	// Token: 0x04001824 RID: 6180
	public NewNode.FloorTileType floorType = NewNode.FloorTileType.floorAndCeiling;

	// Token: 0x04001825 RID: 6181
	public List<Vector2> preventEntrances;

	// Token: 0x04001826 RID: 6182
	[Header("Spawned Objects")]
	public GameObject floorPrefab;

	// Token: 0x04001827 RID: 6183
	public GameObject spawnedFloor;

	// Token: 0x04001828 RID: 6184
	public GameObject ceilingPrefab;

	// Token: 0x04001829 RID: 6185
	public GameObject spawnedCeiling;

	// Token: 0x0400182A RID: 6186
	[Header("AI Navigation")]
	public float nodeWeightMultiplier = 1f;

	// Token: 0x0400182B RID: 6187
	public bool isObstacle;

	// Token: 0x0400182C RID: 6188
	public bool isOutside;

	// Token: 0x0400182D RID: 6189
	public bool isConnected;

	// Token: 0x0400182E RID: 6190
	public bool stairwellLowerLink;

	// Token: 0x0400182F RID: 6191
	public bool stairwellUpperLink;

	// Token: 0x04001830 RID: 6192
	public bool isInaccessable;

	// Token: 0x04001831 RID: 6193
	public bool isIndoorsEntrance;

	// Token: 0x04001832 RID: 6194
	public bool ceilingAirVent;

	// Token: 0x04001833 RID: 6195
	public bool floorAirVent;

	// Token: 0x04001834 RID: 6196
	public bool noPassThrough;

	// Token: 0x04001835 RID: 6197
	public bool noAccess;

	// Token: 0x04001836 RID: 6198
	[NonSerialized]
	public RoomConfiguration forcedRoom;

	// Token: 0x04001837 RID: 6199
	public string forcedRoomRef;

	// Token: 0x04001838 RID: 6200
	public NewNode.NodeSpace defaultSpace;

	// Token: 0x04001839 RID: 6201
	public Dictionary<Vector3, NewNode.NodeSpace> walkableNodeSpace = new Dictionary<Vector3, NewNode.NodeSpace>();

	// Token: 0x0400183A RID: 6202
	public HashSet<NewNode.NodeSpace> occupiedSpace = new HashSet<NewNode.NodeSpace>();

	// Token: 0x0400183B RID: 6203
	public bool detectGeometry = true;

	// Token: 0x0400183C RID: 6204
	[Header("Furniture")]
	public bool allowNewFurniture = true;

	// Token: 0x0400183D RID: 6205
	public List<FurnitureLocation> individualFurniture = new List<FurnitureLocation>();

	// Token: 0x0400183E RID: 6206
	[Header("Interactables")]
	public List<Interactable> interactables = new List<Interactable>();

	// Token: 0x0400183F RID: 6207
	[Header("Air Ducts")]
	public List<AirDuctGroup.AirDuctSection> airDucts = new List<AirDuctGroup.AirDuctSection>();

	// Token: 0x04001840 RID: 6208
	[Header("Audio Source")]
	public AudioEvent audioEvent;

	// Token: 0x04001841 RID: 6209
	public AudioController.LoopingSoundInfo loop;

	// Token: 0x04001842 RID: 6210
	public Vector3 audioOffset;

	// Token: 0x04001843 RID: 6211
	public Dictionary<NewNode, NewNode.NodeAccess> accessToOtherNodes = new Dictionary<NewNode, NewNode.NodeAccess>();

	// Token: 0x0200036E RID: 878
	public enum FloorTileType
	{
		// Token: 0x04001845 RID: 6213
		none,
		// Token: 0x04001846 RID: 6214
		floorAndCeiling,
		// Token: 0x04001847 RID: 6215
		floorOnly,
		// Token: 0x04001848 RID: 6216
		CeilingOnly,
		// Token: 0x04001849 RID: 6217
		noneButIndoors
	}

	// Token: 0x0200036F RID: 879
	public class NodeSpace
	{
		// Token: 0x060013E1 RID: 5089 RVA: 0x0011D6EB File Offset: 0x0011B8EB
		public void SetEmpty()
		{
			this.occupier = null;
			this.occ = NewNode.NodeSpaceOccupancy.empty;
			if (this.node.occupiedSpace.Contains(this))
			{
				this.node.occupiedSpace.Remove(this);
			}
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0011D720 File Offset: 0x0011B920
		public void SetOccuppier(Actor newOcc, NewNode.NodeSpaceOccupancy occType)
		{
			if (this.occupier != newOcc && this.occupier != null)
			{
				this.SetEmpty();
			}
			this.occupier = newOcc;
			this.occ = occType;
			if (!this.node.occupiedSpace.Contains(this))
			{
				this.node.occupiedSpace.Add(this);
			}
			if (this.occ == NewNode.NodeSpaceOccupancy.reserved)
			{
				this.occupier.AddReservedNodeSpace(this);
			}
		}

		// Token: 0x0400184A RID: 6218
		public NewNode node;

		// Token: 0x0400184B RID: 6219
		public NewNode.NodeSpaceOccupancy occ;

		// Token: 0x0400184C RID: 6220
		public Actor occupier;

		// Token: 0x0400184D RID: 6221
		public Vector3 position;
	}

	// Token: 0x02000370 RID: 880
	public enum NodeSpaceOccupancy
	{
		// Token: 0x0400184F RID: 6223
		empty,
		// Token: 0x04001850 RID: 6224
		position,
		// Token: 0x04001851 RID: 6225
		reserved
	}

	// Token: 0x02000371 RID: 881
	[Serializable]
	public class NodeAccess : IEquatable<NewNode.NodeAccess>
	{
		// Token: 0x060013E4 RID: 5092 RVA: 0x0011D797 File Offset: 0x0011B997
		bool IEquatable<NewNode.NodeAccess>.Equals(NewNode.NodeAccess other)
		{
			return other.GetHashCode() == this.GetHashCode();
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x0011D7A7 File Offset: 0x0011B9A7
		public override int GetHashCode()
		{
			if (!this.hasHash)
			{
				this.hash = HashCode.Combine<string, int>(this.name, this.id);
				this.hasHash = true;
			}
			return this.hash;
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x0011D7D8 File Offset: 0x0011B9D8
		public NodeAccess(NewNode newFrom, NewNode newTo, NewWall newWall, NewDoor newDoorway, bool forceAccessType = false, NewNode.NodeAccess.AccessType forcedAccessType = NewNode.NodeAccess.AccessType.adjacent, bool forceWalkable = false)
		{
			this.id = NewNode.NodeAccess.assignId;
			NewNode.NodeAccess.assignId++;
			this.fromNode = newFrom;
			this.toNode = newTo;
			this.wall = newWall;
			this.door = newDoorway;
			if (forceAccessType)
			{
				this.accessType = forcedAccessType;
			}
			else
			{
				this.accessType = NewNode.NodeAccess.AccessType.adjacent;
				if (newFrom.room != newTo.room)
				{
					this.accessType = NewNode.NodeAccess.AccessType.openDoorway;
				}
				if (this.door != null)
				{
					this.accessType = NewNode.NodeAccess.AccessType.door;
					if (this.toNode.gameLocation != this.fromNode.gameLocation)
					{
						if (this.fromNode.gameLocation.thisAsAddress != null)
						{
							if (this.fromNode.gameLocation.thisAsAddress.company != null && this.fromNode.gameLocation.thisAsAddress.residence == null && this.fromNode.gameLocation.thisAsAddress.company.publicFacing && this.toNode.gameLocation.thisAsStreet == null && this.fromNode.room.preset != null && this.fromNode.room.preset.forbidden == RoomConfiguration.Forbidden.alwaysForbidden && (this.fromNode.floor == null || this.fromNode.floor.floor == 0) && this.fromNode.gameLocation.AllowEmployeeDoors())
							{
								this.employeeDoor = true;
							}
							if (this.toNode.gameLocation.thisAsStreet != null)
							{
								this.fromNode.isIndoorsEntrance = true;
								this.toNode.isIndoorsEntrance = true;
							}
						}
						if (this.toNode.gameLocation.thisAsAddress != null)
						{
							if (this.toNode.gameLocation.thisAsAddress.company != null && this.toNode.gameLocation.thisAsAddress.residence == null && this.toNode.gameLocation.thisAsAddress.company.publicFacing && this.fromNode.gameLocation.thisAsStreet == null && this.toNode.room.preset != null && this.toNode.room.preset.forbidden == RoomConfiguration.Forbidden.alwaysForbidden && (this.toNode.floor == null || this.toNode.floor.floor == 0) && this.toNode.gameLocation.AllowEmployeeDoors())
							{
								this.employeeDoor = true;
							}
							if (this.fromNode.gameLocation.thisAsStreet != null)
							{
								this.fromNode.isIndoorsEntrance = true;
								this.toNode.isIndoorsEntrance = true;
							}
						}
					}
					if (this.toNode.building != this.fromNode.building)
					{
						if (this.wall.node.building != null && this.wall.otherWall.node.building == null)
						{
							if (this.wall.node.tile.isMainEntrance && this.wall.node.floor.floor == 0)
							{
								this.wall.node.building.AddBuildingEntrance(this.wall, true);
							}
							else if (this.wall.node.tile.isEntrance && this.wall.node.floor.floor == 0)
							{
								this.wall.node.building.AddBuildingEntrance(this.wall, false);
							}
						}
						if (this.wall.node.building == null && this.wall.otherWall.node.building != null)
						{
							if (this.wall.otherWall.node.tile.isMainEntrance && this.wall.otherWall.node.floor.floor == 0)
							{
								this.wall.otherWall.node.building.AddBuildingEntrance(this.wall.otherWall, true);
							}
							else if (this.wall.otherWall.node.tile.isEntrance && this.wall.otherWall.node.floor.floor == 0)
							{
								this.wall.otherWall.node.building.AddBuildingEntrance(this.wall.otherWall, false);
							}
						}
					}
				}
				else if (this.wall != null && (this.wall.preset.sectionClass == DoorPairPreset.WallSectionClass.window || this.wall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge || this.wall.preset.sectionClass == DoorPairPreset.WallSectionClass.wall))
				{
					if (!this.wall.node.room.IsOutside() && !this.wall.otherWall.node.room.IsOutside() && this.wall.node.building == this.wall.otherWall.node.building)
					{
						this.accessType = NewNode.NodeAccess.AccessType.bannister;
					}
					else
					{
						this.accessType = NewNode.NodeAccess.AccessType.window;
						if (this.wall == null)
						{
							Game.LogError("No wall for window!", 2);
						}
					}
				}
				else if (newFrom.gameLocation.thisAsStreet != null && newTo.gameLocation.thisAsStreet != null && newFrom.gameLocation != newTo.gameLocation)
				{
					this.accessType = NewNode.NodeAccess.AccessType.streetToStreet;
				}
				else if (this.fromNode.nodeCoord.z != this.toNode.nodeCoord.z)
				{
					this.accessType = NewNode.NodeAccess.AccessType.verticalSpace;
				}
			}
			if (forceWalkable)
			{
				this.walkingAccess = true;
			}
			else if (this.accessType == NewNode.NodeAccess.AccessType.bannister || this.accessType == NewNode.NodeAccess.AccessType.window || this.accessType == NewNode.NodeAccess.AccessType.verticalSpace || this.fromNode.isObstacle || this.toNode.isObstacle)
			{
				this.walkingAccess = false;
			}
			this.weight = Vector3.Distance(this.fromNode.position, this.toNode.position) * this.toNode.nodeWeightMultiplier;
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				this.name = string.Concat(new string[]
				{
					this.fromNode.room.name,
					" -> ",
					this.toNode.room.name,
					"(",
					this.weight.ToString(),
					")"
				});
			}
			this.worldAccessPoint = (newFrom.position + newTo.position) * 0.5f;
			if (this.toNode.accessToOtherNodes.ContainsKey(this.fromNode))
			{
				this.oppositeAccess = this.toNode.accessToOtherNodes[this.fromNode];
			}
			PathFinder.Instance.nodeAccessReference.Add(this.id, this);
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x0011DF92 File Offset: 0x0011C192
		public NewNode GetOther(NewNode fromThis)
		{
			if (this.fromNode == fromThis)
			{
				return this.toNode;
			}
			return this.fromNode;
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x0011DFAA File Offset: 0x0011C1AA
		public NewRoom GetOtherRoom(NewRoom fromThis)
		{
			if (this.fromNode.room == fromThis)
			{
				return this.toNode.room;
			}
			return this.fromNode.room;
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x0011DFD6 File Offset: 0x0011C1D6
		public NewRoom GetOtherRoom(NewGameLocation fromThis)
		{
			if (this.fromNode.gameLocation == fromThis)
			{
				return this.toNode.room;
			}
			return this.fromNode.room;
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x0011E004 File Offset: 0x0011C204
		public NewNode GetOtherGameLocation(NewNode fromThis)
		{
			if (!(this.fromNode.gameLocation != fromThis.gameLocation))
			{
				return this.toNode;
			}
			if (this.fromNode.gameLocation.thisAsStreet != null && fromThis.gameLocation.thisAsStreet != null && this.toNode.gameLocation != fromThis.gameLocation && this.toNode.gameLocation.thisAsStreet == null)
			{
				return this.toNode;
			}
			return this.fromNode;
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x0011E098 File Offset: 0x0011C298
		public NewGameLocation GetOtherGameLocation(NewGameLocation fromThis)
		{
			if (!(this.fromNode.gameLocation != fromThis))
			{
				return this.toNode.gameLocation;
			}
			if (this.fromNode.gameLocation.thisAsStreet != null && fromThis.thisAsStreet != null && this.toNode.gameLocation != fromThis && this.toNode.gameLocation.thisAsStreet == null)
			{
				return this.toNode.gameLocation;
			}
			return this.fromNode.gameLocation;
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x0011E12C File Offset: 0x0011C32C
		public void PreComputeEntranceWeights()
		{
			if (this.employeeDoor)
			{
				if (!this.fromNode.gameLocation.entrances.Exists((NewNode.NodeAccess item) => item != this && item.accessType == NewNode.NodeAccess.AccessType.door && !item.employeeDoor))
				{
					this.employeeDoor = false;
				}
				if (!this.toNode.gameLocation.entrances.Exists((NewNode.NodeAccess item) => item != this && item.accessType == NewNode.NodeAccess.AccessType.door && !item.employeeDoor))
				{
					this.employeeDoor = false;
				}
			}
			if (this.fromNode.gameLocation.thisAsAddress != null)
			{
				using (List<NewNode.NodeAccess>.Enumerator enumerator = this.fromNode.gameLocation.entrances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NewNode.NodeAccess nodeAccess = enumerator.Current;
						if (nodeAccess != null && nodeAccess.walkingAccess && !nodeAccess.employeeDoor && !this.entranceWeights.ContainsKey(nodeAccess))
						{
							try
							{
								this.entranceWeights.Add(nodeAccess, Vector3.Distance(this.worldAccessPoint, nodeAccess.worldAccessPoint));
							}
							catch
							{
							}
						}
					}
					goto IL_169;
				}
			}
			foreach (NewNode.NodeAccess nodeAccess2 in PathFinder.Instance.streetEntrances)
			{
				if (nodeAccess2 != null && nodeAccess2.walkingAccess && !nodeAccess2.employeeDoor && !this.entranceWeights.ContainsKey(nodeAccess2))
				{
					try
					{
						this.entranceWeights.Add(nodeAccess2, Vector3.Distance(this.worldAccessPoint, nodeAccess2.worldAccessPoint));
					}
					catch
					{
					}
				}
			}
			IL_169:
			if (this.toNode.gameLocation.thisAsAddress != null)
			{
				using (List<NewNode.NodeAccess>.Enumerator enumerator = this.toNode.gameLocation.entrances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NewNode.NodeAccess nodeAccess3 = enumerator.Current;
						if (nodeAccess3 != null && nodeAccess3.walkingAccess && !this.entranceWeights.ContainsKey(nodeAccess3))
						{
							try
							{
								this.entranceWeights.Add(nodeAccess3, Vector3.Distance(this.worldAccessPoint, nodeAccess3.worldAccessPoint));
							}
							catch
							{
							}
						}
					}
					return;
				}
			}
			foreach (NewNode.NodeAccess nodeAccess4 in PathFinder.Instance.streetEntrances)
			{
				if (nodeAccess4 != null && nodeAccess4.walkingAccess && !this.entranceWeights.ContainsKey(nodeAccess4))
				{
					try
					{
						this.entranceWeights.Add(nodeAccess4, Vector3.Distance(this.worldAccessPoint, nodeAccess4.worldAccessPoint));
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x0011E40C File Offset: 0x0011C60C
		public void GetEntranceSidePoints(out Vector3 leftSide, out Vector3 rightSide)
		{
			Vector3 vector = this.toNode.position - this.fromNode.position;
			Vector3 vector2;
			vector2..ctor(vector.z * 0.5f, 0f, vector.x * 5f);
			leftSide = this.worldAccessPoint - vector2;
			rightSide = this.worldAccessPoint + vector2;
		}

		// Token: 0x04001852 RID: 6226
		public string name;

		// Token: 0x04001853 RID: 6227
		public int id;

		// Token: 0x04001854 RID: 6228
		public static int assignId;

		// Token: 0x04001855 RID: 6229
		public float weight = 1.8f;

		// Token: 0x04001856 RID: 6230
		public NewDoor door;

		// Token: 0x04001857 RID: 6231
		public NewWall wall;

		// Token: 0x04001858 RID: 6232
		public NewNode.NodeAccess.AccessType accessType = NewNode.NodeAccess.AccessType.adjacent;

		// Token: 0x04001859 RID: 6233
		public NewNode fromNode;

		// Token: 0x0400185A RID: 6234
		public NewNode toNode;

		// Token: 0x0400185B RID: 6235
		public bool walkingAccess = true;

		// Token: 0x0400185C RID: 6236
		public bool employeeDoor;

		// Token: 0x0400185D RID: 6237
		public Vector3 worldAccessPoint = Vector3.zero;

		// Token: 0x0400185E RID: 6238
		public NewNode.NodeAccess oppositeAccess;

		// Token: 0x0400185F RID: 6239
		public Dictionary<NewNode.NodeAccess, float> entranceWeights = new Dictionary<NewNode.NodeAccess, float>();

		// Token: 0x04001860 RID: 6240
		private bool hasHash;

		// Token: 0x04001861 RID: 6241
		private int hash;

		// Token: 0x02000372 RID: 882
		public enum AccessType
		{
			// Token: 0x04001863 RID: 6243
			streetToStreet,
			// Token: 0x04001864 RID: 6244
			door,
			// Token: 0x04001865 RID: 6245
			openDoorway,
			// Token: 0x04001866 RID: 6246
			verticalSpace,
			// Token: 0x04001867 RID: 6247
			adjacent,
			// Token: 0x04001868 RID: 6248
			window,
			// Token: 0x04001869 RID: 6249
			bannister
		}
	}
}
