using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020003B8 RID: 952
[Serializable]
public class FurnitureLocation
{
	// Token: 0x0600158B RID: 5515 RVA: 0x0013A1D0 File Offset: 0x001383D0
	public FurnitureLocation(List<FurnitureClass> newClasses, int newAngle, NewNode newAnchor, List<NewNode> newCoversNodes, bool newUseFOVBlock = false, Vector2 newFovDirection = default(Vector2), int newFOVBlockMax = 5, Vector3 newScale = default(Vector3), bool newUserPlaced = false, Vector3 newOffset = default(Vector3))
	{
		this.userPlaced = newUserPlaced;
		this.offset = newOffset;
		this.furnitureClasses = newClasses;
		this.angle = newAngle;
		this.anchorNode = newAnchor;
		this.coversNodes = newCoversNodes;
		this.useFOVBLock = newUseFOVBlock;
		this.fovDirection = newFovDirection;
		this.fovMaxDistance = newFOVBlockMax;
		this.scaleMultiplier = newScale;
		if (!this.userPlaced)
		{
			this.DiagonalRotation();
		}
		this.CalculateWalkableSublocations();
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x0013A2D6 File Offset: 0x001384D6
	public void AssignID(NewRoom fromRoom)
	{
		if (this.id <= 0)
		{
			this.id = fromRoom.roomID * 10000 + fromRoom.furnitureAssignID;
			fromRoom.furnitureAssignID++;
		}
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x0013A308 File Offset: 0x00138508
	public FurnitureLocation(FurnitureClusterLocation newCluster, List<FurnitureClass> newClasses, int newAngle, NewNode newAnchor, List<NewNode> newCoversNodes, bool newUseFOVBlock = false, Vector2 newFovDirection = default(Vector2), int newFOVBlockMax = 5, Vector3 newScale = default(Vector3), bool newUserPlaced = false, Vector3 newOffset = default(Vector3))
	{
		if (newCluster != null)
		{
			this.id = newCluster.anchorNode.room.roomID * 10000 + newCluster.anchorNode.room.furnitureAssignID;
			newCluster.anchorNode.room.furnitureAssignID++;
		}
		this.userPlaced = newUserPlaced;
		this.offset = newOffset;
		this.furnitureClasses = newClasses;
		this.angle = newAngle;
		this.anchorNode = newAnchor;
		this.coversNodes = newCoversNodes;
		this.cluster = newCluster;
		this.useFOVBLock = newUseFOVBlock;
		this.fovDirection = newFovDirection;
		this.fovMaxDistance = newFOVBlockMax;
		this.scaleMultiplier = newScale;
		if (!this.userPlaced)
		{
			this.DiagonalRotation();
		}
		this.CalculateWalkableSublocations();
	}

	// Token: 0x0600158E RID: 5518 RVA: 0x0013A460 File Offset: 0x00138660
	public FurnitureLocation(int loadID, FurnitureClusterLocation newCluster, List<FurnitureClass> newClasses, int newAngle, NewNode newAnchor, List<NewNode> newCoversNodes, bool newUseFOVBlock = false, Vector2 newFovDirection = default(Vector2), int newFOVBlockMax = 5, Vector3 newScale = default(Vector3), bool newUserPlaced = false, Vector3 newOffset = default(Vector3))
	{
		this.id = loadID;
		this.userPlaced = newUserPlaced;
		this.offset = newOffset;
		this.furnitureClasses = newClasses;
		this.angle = newAngle;
		this.anchorNode = newAnchor;
		this.coversNodes = newCoversNodes;
		this.cluster = newCluster;
		this.useFOVBLock = newUseFOVBlock;
		this.fovDirection = newFovDirection;
		this.fovMaxDistance = newFOVBlockMax;
		this.scaleMultiplier = newScale;
		if (this.anchorNode != null)
		{
			if (!this.userPlaced)
			{
				this.DiagonalRotation();
			}
			this.CalculateWalkableSublocations();
		}
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x0013A580 File Offset: 0x00138780
	public void RaiseLightswitch()
	{
		float num = 1.375f;
		bool flag = false;
		foreach (FurnitureClass furnitureClass in this.furnitureClasses)
		{
			if (furnitureClass.raiseLightswitch)
			{
				flag = true;
				num = Mathf.Max(num, furnitureClass.lightswitchYOffset);
			}
		}
		if (flag)
		{
			foreach (NewWall newWall in this.anchorNode.walls)
			{
				if (newWall.lightswitchInteractable != null)
				{
					Vector3 worldPosition = newWall.lightswitchInteractable.GetWorldPosition(true);
					worldPosition.y = this.anchorNode.position.y + num;
					newWall.lightswitchInteractable.lPos.y = num;
					newWall.lightswitchInteractable.wPos = worldPosition;
					newWall.lightswitchInteractable.spWPos = worldPosition;
				}
			}
		}
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x0013A694 File Offset: 0x00138894
	private void DiagonalRotation()
	{
		using (List<FurnitureClass>.Enumerator enumerator = this.furnitureClasses.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.canFaceDiagonally && this.anchorNode.walls.Count == 2)
				{
					Vector2 vector = Vector2.zero;
					foreach (NewWall newWall in this.anchorNode.walls)
					{
						vector += newWall.wallOffset;
					}
					if (Mathf.Abs(vector.x) > 0f && Mathf.Abs(vector.x) < 1f && Mathf.Abs(vector.y) > 0f && Mathf.Abs(vector.y) < 1f)
					{
						Vector2 vector2 = Toolbox.Instance.RotateVector2CW(vector, (float)this.angle);
						if (vector2.x < 0f && vector2.y > 0f)
						{
							this.diagonalAngle -= 45;
						}
						else if (vector2.x > 0f && vector2.y < 0f)
						{
							this.diagonalAngle -= 45;
						}
						else if (vector2.x > 0f && vector2.y > 0f)
						{
							this.diagonalAngle += 45;
						}
						else if (vector2.x < 0f && vector2.y < 0f)
						{
							this.diagonalAngle += 45;
						}
					}
				}
			}
		}
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x0013A890 File Offset: 0x00138A90
	public void SpawnObject(bool forceSpawnImmediate = false)
	{
		if (!this.createdInteractables)
		{
			this.CreateInteractables();
		}
		if (this.spawnedObject != null)
		{
			return;
		}
		if (this.furniture == null)
		{
			Game.LogError("Furniture preset missing!", 2);
			return;
		}
		try
		{
			this.spawnedObject = Toolbox.Instance.SpawnObject(this.furniture.prefab, this.anchorNode.room.transform);
			this.spawnedObject.transform.position = this.anchorNode.position + this.offset;
			this.meshes = Enumerable.ToList<MeshRenderer>(this.spawnedObject.GetComponentsInChildren<MeshRenderer>());
			bool flag = false;
			if (this.furniture.allowWeatherAffectedMaterials && this.anchorNode != null && this.anchorNode.room != null && this.anchorNode.room.IsOutside())
			{
				flag = true;
			}
			foreach (MeshRenderer meshRenderer in this.meshes)
			{
				meshRenderer.staticShadowCaster = true;
				if (flag)
				{
					meshRenderer.sharedMaterial = SessionData.Instance.GetWeatherAffectedMaterial(meshRenderer.sharedMaterial, meshRenderer);
				}
				if (Game.Instance.freeCam || forceSpawnImmediate || this.furniture.allowStaticBatching)
				{
					meshRenderer.enabled = true;
				}
				else
				{
					meshRenderer.enabled = !ObjectPoolingController.Instance.useRange;
				}
			}
			if (ObjectPoolingController.Instance.useRange && !Game.Instance.freeCam && !ObjectPoolingController.Instance.furnitureRangeToEnableDisableList.Contains(this))
			{
				ObjectPoolingController.Instance.furnitureRangeToEnableDisableList.Add(this);
			}
		}
		catch
		{
			Game.LogError("Unable to spawn furniture!", 2);
			return;
		}
		if (this.furnitureClasses[0].ceilingPiece)
		{
			this.spawnedObject.transform.localPosition = new Vector3(this.spawnedObject.transform.localPosition.x, (float)this.anchorNode.floor.defaultCeilingHeight * 0.1f, this.spawnedObject.transform.localPosition.z) + this.offset;
		}
		this.spawnedObject.transform.localEulerAngles = new Vector3(0f, (float)(this.angle + this.diagonalAngle), 0f);
		Vector3 localScale = this.furniture.prefab.transform.localScale;
		this.spawnedObject.transform.localScale = new Vector3(localScale.x * this.scaleMultiplier.x, localScale.y * this.scaleMultiplier.y, localScale.z * this.scaleMultiplier.z);
		this.spawnedObject.isStatic = true;
		if (Game.Instance.collectDebugData)
		{
			Object transform = this.spawnedObject.transform;
			string[] array = new string[7];
			array[0] = this.furniture.prefab.name;
			array[1] = " ";
			array[2] = this.id.ToString();
			array[3] = ":  (Offset: ";
			int num = 4;
			Vector3 vector = this.offset;
			array[num] = vector.ToString();
			array[5] = ") CP: ";
			array[6] = this.furnitureClasses[0].ceilingPiece.ToString();
			transform.name = string.Concat(array);
			foreach (NewNode newNode in this.coversNodes)
			{
				Transform transform2 = this.spawnedObject.transform;
				string name = transform2.name;
				Vector3Int nodeCoord = newNode.nodeCoord;
				transform2.name = name + nodeCoord.ToString() + ", ";
			}
		}
		if (this.furniture.inheritColouringFromDecor && this.furniture.variations.Count > 0)
		{
			foreach (MeshRenderer renderer in this.meshes)
			{
				MaterialsController.Instance.ApplyMaterialKey(renderer, this.matKey);
			}
		}
		if (this.art != null)
		{
			int i = 0;
			while (i < this.spawnedObject.transform.childCount)
			{
				Transform child = this.spawnedObject.transform.GetChild(i);
				if (child.tag == "Art")
				{
					MeshRenderer component = child.GetComponent<MeshRenderer>();
					Material material = this.art.material;
					if (this.art.useDynamicText && !GameplayController.Instance.dynamicTextImages.TryGetValue(this.art, ref material))
					{
						material = this.art.material;
					}
					if (component != null)
					{
						component.sharedMaterial = material;
						break;
					}
					DecalProjector component2 = child.GetComponent<DecalProjector>();
					if (component2 != null)
					{
						component2.material = material;
						Texture texture = component2.material.GetTexture("_BaseColorMap");
						component2.size = new Vector3((float)texture.width * this.art.pixelScaleMultiplier, (float)texture.height * this.art.pixelScaleMultiplier, 0.13f);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
		foreach (MeshRenderer meshRenderer2 in this.meshes)
		{
			Toolbox.Instance.SetLightLayer(meshRenderer2, this.anchorNode.room.building, this.anchorNode.room.preset.forceStreetLightLayer);
			if (this.furniture.allowStaticBatching && meshRenderer2.enabled)
			{
				MeshFilter component3 = meshRenderer2.gameObject.GetComponent<MeshFilter>();
				if (component3 != null && meshRenderer2.gameObject.GetComponent<DoorMovementController>() == null && !component3.gameObject.CompareTag("IgnoreStaticBatch") && !component3.gameObject.CompareTag("RainWindowGlass"))
				{
					this.anchorNode.room.AddForStaticBatching(component3.gameObject, component3.sharedMesh, meshRenderer2.sharedMaterial);
				}
			}
		}
		InteractableController[] componentsInChildren = this.spawnedObject.GetComponentsInChildren<InteractableController>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			InteractableController ic = componentsInChildren[j];
			Interactable interactable = this.integratedInteractables.Find((Interactable item) => item.pt == (int)ic.id && item.pt != 11);
			if (interactable != null)
			{
				ic.Setup(interactable);
				interactable.loadedGeometry = true;
				interactable.spawnedObject = ic.gameObject;
				interactable.OnSpawn();
			}
			else
			{
				Game.LogError(string.Concat(new string[]
				{
					"Unable to match furntiure ",
					this.furniture.name,
					" with interactable ",
					ic.id.ToString(),
					" CreatedInteractables: ",
					this.createdInteractables.ToString()
				}), 2);
			}
		}
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x0013B044 File Offset: 0x00139244
	public void DespawnObject()
	{
		if (this.spawnedObject != null)
		{
			Toolbox.Instance.DestroyObject(this.spawnedObject);
			this.meshes.Clear();
		}
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x0013B06F File Offset: 0x0013926F
	public void Delete(bool removeIntegratedInteractables, FurnitureClusterLocation.RemoveInteractablesOption removeSpawnedInteractables)
	{
		if (this.cluster != null)
		{
			this.cluster.DeleteFurniture(this.id, removeIntegratedInteractables, removeSpawnedInteractables);
			return;
		}
		Game.LogError("Unable to delete furniture " + this.id.ToString() + " from cluster, as cluster is null", 2);
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x0013B0B0 File Offset: 0x001392B0
	public void RemoveSpawnedInteractables()
	{
		if (this.spawnedInteractables != null)
		{
			for (int i = 0; i < this.spawnedInteractables.Count; i++)
			{
				this.spawnedInteractables[i].SafeDelete(false);
				i--;
			}
			for (int j = 0; j < this.spawnedInteractables.Count; j++)
			{
				if (this.spawnedInteractables[j] == null)
				{
					this.spawnedInteractables.RemoveAt(j);
					j--;
				}
			}
		}
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x0013B128 File Offset: 0x00139328
	public void RemoveIntegratedInteractables()
	{
		if (this.integratedInteractables != null)
		{
			for (int i = 0; i < this.integratedInteractables.Count; i++)
			{
				this.integratedInteractables[i].Delete();
				i--;
			}
			this.integratedInteractables.Clear();
		}
		this.createdInteractables = false;
		this.integratedIDAssign = 1;
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x0013B184 File Offset: 0x00139384
	public void CreateInteractables()
	{
		if (this.createdInteractables)
		{
			return;
		}
		if (this.furniture == null)
		{
			return;
		}
		using (List<FurniturePreset.IntegratedInteractable>.Enumerator enumerator = this.furniture.integratedInteractables.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				FurniturePreset.IntegratedInteractable integrated = enumerator.Current;
				if (integrated.pairToController != InteractableController.InteractableID.none)
				{
					if (CityConstructor.Instance != null && !CityConstructor.Instance.generateNew)
					{
						Interactable interactable = null;
						if (CityConstructor.Instance.saveState != null)
						{
							int num = CityConstructor.Instance.saveState.interactables.FindIndex((Interactable item) => item.fp == this.id && item.fsoi <= -1 && item.pt == (int)integrated.pairToController);
							if (num > -1)
							{
								interactable = CityConstructor.Instance.saveState.interactables[num];
								CityConstructor.Instance.saveState.interactables.RemoveAt(num);
								interactable.wasLoadedFromSave = true;
								this.integratedIDAssign++;
							}
						}
						if (interactable != null)
						{
							interactable.MainSetupStart();
							interactable.OnLoad();
							if (!this.integratedInteractables.Contains(interactable))
							{
								this.integratedInteractables.Add(interactable);
								continue;
							}
							continue;
						}
					}
					InteractablePreset preset = integrated.preset;
					Human human = null;
					NewAddress newAddress = null;
					if (integrated.belongsTo != FurniturePreset.SubObjectOwnership.everybody && integrated.belongsTo != FurniturePreset.SubObjectOwnership.nobody)
					{
						if (!this.pickedOwners)
						{
							Game.LogError(string.Concat(new string[]
							{
								"Object: ",
								this.furniture.name,
								": Trying to assign ownership to interactables, but owners have not yet been picked: ",
								this.ownerMap.Count.ToString(),
								" ",
								this.pickedOwners.ToString()
							}), 2);
						}
						else
						{
							int num2 = integrated.belongsTo - FurniturePreset.SubObjectOwnership.person0;
							foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in this.ownerMap)
							{
								if (keyValuePair.Value == num2)
								{
									if (keyValuePair.Key.human != null)
									{
										human = keyValuePair.Key.human;
										break;
									}
									if (keyValuePair.Key.address != null)
									{
										newAddress = keyValuePair.Key.address;
										break;
									}
								}
							}
							if (human == null)
							{
								newAddress == null;
							}
						}
					}
					List<InteractableController> list = Enumerable.ToList<InteractableController>(this.furniture.prefab.GetComponentsInChildren<InteractableController>(true));
					InteractableController interactableController = list.Find((InteractableController item) => item.id == integrated.pairToController);
					if (interactableController == null && Game.Instance.collectDebugData)
					{
						Game.LogError(string.Concat(new string[]
						{
							this.furniture.name,
							" Could not find controller in prefab ",
							this.furniture.prefab.name,
							" for pairing id ",
							integrated.pairToController.ToString(),
							" (",
							integrated.preset.name,
							")"
						}), 2);
						foreach (InteractableController interactableController2 in list)
						{
							Game.Log("Listing controller for " + this.furniture.prefab.name + ": " + interactableController2.id.ToString(), 2);
						}
					}
					Vector3 vector = Vector3.zero;
					Vector3 vector2 = Vector3.zero;
					if (this.furniture.prefab == null)
					{
						Game.Log("Furniture prefab is null for " + this.furniture.name, 2);
					}
					else if (interactableController == null)
					{
						Game.Log("Unable for find corresponding controller for integrated interactable on " + this.furniture.name, 2);
					}
					else if (this.furniture.prefab.transform != interactableController.transform)
					{
						vector = interactableController.transform.localPosition;
						vector2 = interactableController.transform.localEulerAngles;
						Transform parent = interactableController.transform.parent;
						while (parent != null && parent != this.furniture.prefab.transform)
						{
							vector += parent.transform.localPosition;
							vector2 += parent.transform.localEulerAngles;
							parent = parent.parent;
						}
					}
					List<Interactable.Passed> list2 = null;
					if (newAddress != null)
					{
						list2 = new List<Interactable.Passed>();
						list2.Add(new Interactable.Passed(Interactable.PassedVarType.ownedByAddress, (float)newAddress.id, null));
					}
					Interactable interactable2 = InteractableCreator.Instance.CreateFurnitureIntegratedInteractable(preset, this.anchorNode.room, this, human, human, null, vector, vector2, integrated.pairToController, integrated.belongsTo, preset.isLight, list2);
					this.integratedInteractables.Add(interactable2);
				}
			}
		}
		foreach (NewNode newNode in this.coversNodes)
		{
			foreach (Interactable interactable3 in this.integratedInteractables)
			{
				if (!newNode.interactables.Contains(interactable3))
				{
					newNode.AddInteractable(interactable3);
				}
			}
		}
		this.createdInteractables = true;
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x0013B7C4 File Offset: 0x001399C4
	public void AssignOwner(Human newOwner, bool updateIntegratedObjectOwnership)
	{
		if (newOwner == null)
		{
			Game.LogError("CityGen: Trying to assign a null owner to furniture. This probably isn't something you want.", 2);
			return;
		}
		if (!this.anchorNode.room.gameLocation.furnitureBelongsTo.ContainsKey(this.furnitureClasses[0].ownershipClass))
		{
			this.anchorNode.room.gameLocation.furnitureBelongsTo.Add(this.furnitureClasses[0].ownershipClass, new Dictionary<FurnitureLocation, List<Human>>());
		}
		if (!this.anchorNode.room.gameLocation.furnitureBelongsTo[this.furnitureClasses[0].ownershipClass].ContainsKey(this))
		{
			this.anchorNode.room.gameLocation.furnitureBelongsTo[this.furnitureClasses[0].ownershipClass].Add(this, new List<Human>());
		}
		this.anchorNode.room.gameLocation.furnitureBelongsTo[this.furnitureClasses[0].ownershipClass][this].Add(newOwner);
		FurnitureLocation.OwnerKey ownerKey = new FurnitureLocation.OwnerKey(newOwner);
		if (!this.ownerMap.ContainsKey(ownerKey))
		{
			int num = -1;
			foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in this.ownerMap)
			{
				num = Mathf.Max(num, keyValuePair.Value);
			}
			this.ownerMap.Add(ownerKey, num + 1);
		}
		if (this.createdInteractables && updateIntegratedObjectOwnership)
		{
			this.UpdateIntegratedObjectOwnership();
		}
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x0013B968 File Offset: 0x00139B68
	public void AssignOwner(NewAddress newOwner, bool updateIntegratedObjectOwnership)
	{
		if (newOwner == null)
		{
			Game.LogError("CityGen: Trying to assign a null owner to furniture. This probably isn't something you want.", 2);
			return;
		}
		FurnitureLocation.OwnerKey ownerKey = new FurnitureLocation.OwnerKey(newOwner);
		if (!this.ownerMap.ContainsKey(ownerKey))
		{
			int num = -1;
			foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in this.ownerMap)
			{
				num = Mathf.Max(num, keyValuePair.Value);
			}
			this.ownerMap.Add(ownerKey, num + 1);
		}
		if (Game.Instance.collectDebugData && newOwner != null)
		{
			this.debugOwners.Add(newOwner);
		}
		if (this.createdInteractables && updateIntegratedObjectOwnership)
		{
			this.UpdateIntegratedObjectOwnership();
		}
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x0013BA30 File Offset: 0x00139C30
	public void UpdateIntegratedObjectOwnership()
	{
		foreach (Interactable interactable in this.integratedInteractables)
		{
			if (interactable != null)
			{
				InteractableController.InteractableID pairTo = (InteractableController.InteractableID)interactable.pt;
				InteractablePreset preset = interactable.preset;
				Human human = null;
				NewAddress newAddress = null;
				FurniturePreset.IntegratedInteractable integratedInteractable = this.furniture.integratedInteractables.Find((FurniturePreset.IntegratedInteractable item) => item.pairToController == pairTo);
				if (integratedInteractable != null)
				{
					if (integratedInteractable.belongsTo != FurniturePreset.SubObjectOwnership.everybody && integratedInteractable.belongsTo != FurniturePreset.SubObjectOwnership.nobody)
					{
						if (!this.pickedOwners)
						{
							string[] array = new string[8];
							array[0] = "Object: ";
							array[1] = this.furniture.name;
							array[2] = ": ";
							int num = 3;
							Vector3 position = this.anchorNode.position;
							array[num] = position.ToString();
							array[4] = " Trying to assign ownership to interactables, but owners have not yet been picked: ";
							array[5] = this.ownerMap.Count.ToString();
							array[6] = " ";
							array[7] = this.pickedOwners.ToString();
							Game.LogError(string.Concat(array), 2);
						}
						else
						{
							int num2 = integratedInteractable.belongsTo - FurniturePreset.SubObjectOwnership.person0;
							foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in this.ownerMap)
							{
								if (keyValuePair.Value == num2)
								{
									if (keyValuePair.Key.human != null)
									{
										human = keyValuePair.Key.human;
										break;
									}
									if (keyValuePair.Key.address != null)
									{
										newAddress = keyValuePair.Key.address;
										break;
									}
								}
							}
							if (human == null && newAddress == null)
							{
								string[] array2 = new string[8];
								array2[0] = "Object: ";
								array2[1] = this.furniture.name;
								array2[2] = ": ";
								int num3 = 3;
								Vector3 position = this.anchorNode.position;
								array2[num3] = position.ToString();
								array2[4] = "  Could not find interactable owner for index ";
								array2[5] = num2.ToString();
								array2[6] = ". Ownership list count of ";
								array2[7] = this.ownerMap.Count.ToString();
								Game.Log(string.Concat(array2), 2);
							}
						}
					}
					if (human != null)
					{
						interactable.SetOwner(human, true);
						if (interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.sleepPosition)
						{
							human.SetBed(interactable);
						}
						if (human.job != null && human.job.employer != null && human.job.preset.ownsWorkPosition && human.job.preset.jobPostion != InteractablePreset.SpecialCase.none && human.job.preset.jobPostion == interactable.preset.specialCaseFlag)
						{
							human.SetWorkFurniture(interactable);
						}
						if (Game.Instance.collectDebugData)
						{
							this.debugOwners.Add(human);
						}
					}
					if (newAddress != null)
					{
						if (interactable.pv == null)
						{
							interactable.pv = new List<Interactable.Passed>();
						}
						if (!interactable.pv.Exists((Interactable.Passed item) => item.varType == Interactable.PassedVarType.ownedByAddress))
						{
							interactable.pv.Add(new Interactable.Passed(Interactable.PassedVarType.ownedByAddress, (float)newAddress.id, null));
							interactable.objectRef = newAddress;
						}
						interactable.UpdateName(false, Evidence.DataKey.name);
					}
				}
			}
		}
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x0013BDCC File Offset: 0x00139FCC
	public Vector3 GetWorldAveragePosition()
	{
		Vector3 vector = Vector3.zero;
		foreach (NewNode newNode in this.coversNodes)
		{
			vector += newNode.position;
		}
		return vector / (float)this.coversNodes.Count;
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x0013BE40 File Offset: 0x0013A040
	public void CalculateWalkableSublocations()
	{
		foreach (FurnitureClass furnitureClass in this.furnitureClasses)
		{
			foreach (FurnitureClass.FurniureWalkSubLocations furniureWalkSubLocations in furnitureClass.sublocations)
			{
				Vector2 vector = Toolbox.Instance.RotateVector2CW(furniureWalkSubLocations.offset, (float)(this.angle - 180));
				Vector3 offsetCoord = this.anchorNode.nodeCoord + new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), 0f);
				NewNode newNode = this.coversNodes.Find((NewNode item) => item.nodeCoord == offsetCoord);
				if (newNode != null)
				{
					if (!this.sublocations.ContainsKey(newNode))
					{
						this.sublocations.Add(newNode, new List<Vector3>());
					}
					using (List<Vector3>.Enumerator enumerator3 = furniureWalkSubLocations.sublocations.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Vector3 vector2 = enumerator3.Current;
							Vector2 vector3;
							vector3..ctor(vector2.x, vector2.z);
							vector3 = Toolbox.Instance.RotateVector2CW(vector3, (float)this.angle);
							this.sublocations[newNode].Add(new Vector3(vector3.x, vector2.y, vector3.y));
						}
						continue;
					}
				}
				Game.Log(string.Concat(new string[]
				{
					"Unable to find walkable sublocation for ",
					furnitureClass.presetName,
					" + in 'covered nodes': ",
					offsetCoord.ToString(),
					" (there are ",
					this.coversNodes.Count.ToString(),
					" covered nodes) Rotated by angle ",
					this.angle.ToString()
				}), 2);
			}
		}
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x0013C0A8 File Offset: 0x0013A2A8
	public Vector3 GetSubObjectLocalPosition(FurniturePreset.SubObject subObj)
	{
		Vector3 vector = subObj.localPos;
		GameObject prefab = this.furniture.prefab;
		Transform transform = prefab.transform;
		if (subObj.parent != null && subObj.parent.Length > 0)
		{
			transform = Toolbox.Instance.SearchForTransform(prefab.transform, subObj.parent, false);
		}
		int num = 6;
		while (transform.transform.parent != null && transform.transform.parent != prefab.transform && num > 0)
		{
			vector += transform.transform.parent.localPosition;
			transform = transform.transform.parent;
			num--;
		}
		return vector;
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x0013C158 File Offset: 0x0013A358
	public Vector3 GetSubObjectLocalEuler(FurniturePreset.SubObject subObj)
	{
		Vector3 vector = subObj.localRot;
		GameObject prefab = this.furniture.prefab;
		Transform transform = prefab.transform;
		if (subObj.parent != null && subObj.parent.Length > 0)
		{
			transform = Toolbox.Instance.SearchForTransform(prefab.transform, subObj.parent, false);
		}
		int num = 6;
		while (transform.transform.parent != null && transform.transform.parent != prefab.transform && num > 0)
		{
			vector += transform.transform.parent.localEulerAngles;
			transform = transform.transform.parent;
			num--;
		}
		return vector;
	}

	// Token: 0x04001ABF RID: 6847
	public int id;

	// Token: 0x04001AC0 RID: 6848
	public List<FurnitureClass> furnitureClasses = new List<FurnitureClass>();

	// Token: 0x04001AC1 RID: 6849
	public int angle;

	// Token: 0x04001AC2 RID: 6850
	public Vector3 offset;

	// Token: 0x04001AC3 RID: 6851
	public NewNode anchorNode;

	// Token: 0x04001AC4 RID: 6852
	public List<NewNode> coversNodes = new List<NewNode>();

	// Token: 0x04001AC5 RID: 6853
	public FurnitureClusterLocation cluster;

	// Token: 0x04001AC6 RID: 6854
	public bool useFOVBLock;

	// Token: 0x04001AC7 RID: 6855
	public Vector2 fovDirection = Vector2.zero;

	// Token: 0x04001AC8 RID: 6856
	public int fovMaxDistance = 5;

	// Token: 0x04001AC9 RID: 6857
	public Vector3 scaleMultiplier = Vector3.one;

	// Token: 0x04001ACA RID: 6858
	public List<Interactable.UsagePoint> usage = new List<Interactable.UsagePoint>();

	// Token: 0x04001ACB RID: 6859
	public Dictionary<NewNode, List<Vector3>> sublocations = new Dictionary<NewNode, List<Vector3>>();

	// Token: 0x04001ACC RID: 6860
	public FurniturePreset furniture;

	// Token: 0x04001ACD RID: 6861
	public GameObject spawnedObject;

	// Token: 0x04001ACE RID: 6862
	public List<MeshRenderer> meshes = new List<MeshRenderer>();

	// Token: 0x04001ACF RID: 6863
	public bool pickedMaterials;

	// Token: 0x04001AD0 RID: 6864
	public bool createdInteractables;

	// Token: 0x04001AD1 RID: 6865
	public bool pickedOwners;

	// Token: 0x04001AD2 RID: 6866
	public bool pickedArt;

	// Token: 0x04001AD3 RID: 6867
	public bool userPlaced;

	// Token: 0x04001AD4 RID: 6868
	public int diagonalAngle;

	// Token: 0x04001AD5 RID: 6869
	public Toolbox.MaterialKey matKey;

	// Token: 0x04001AD6 RID: 6870
	public List<Interactable> integratedInteractables = new List<Interactable>();

	// Token: 0x04001AD7 RID: 6871
	public int integratedIDAssign = 1;

	// Token: 0x04001AD8 RID: 6872
	public List<Interactable> spawnedInteractables = new List<Interactable>();

	// Token: 0x04001AD9 RID: 6873
	public ArtPreset art;

	// Token: 0x04001ADA RID: 6874
	public Toolbox.MaterialKey artMatKey;

	// Token: 0x04001ADB RID: 6875
	public List<int> loadOwners = new List<int>();

	// Token: 0x04001ADC RID: 6876
	public Dictionary<FurnitureLocation.OwnerKey, int> ownerMap = new Dictionary<FurnitureLocation.OwnerKey, int>();

	// Token: 0x04001ADD RID: 6877
	public List<MonoBehaviour> debugOwners = new List<MonoBehaviour>();

	// Token: 0x020003B9 RID: 953
	public struct OwnerKey
	{
		// Token: 0x0600159E RID: 5534 RVA: 0x0013C208 File Offset: 0x0013A408
		public OwnerKey(Human newHuman)
		{
			this.human = newHuman;
			this.address = null;
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x0013C218 File Offset: 0x0013A418
		public OwnerKey(NewAddress newAddress)
		{
			this.address = newAddress;
			this.human = null;
		}

		// Token: 0x04001ADE RID: 6878
		public Human human;

		// Token: 0x04001ADF RID: 6879
		public NewAddress address;
	}
}
