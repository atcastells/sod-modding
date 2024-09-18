using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000446 RID: 1094
public class PlayerApartmentController : MonoBehaviour
{
	// Token: 0x14000027 RID: 39
	// (add) Token: 0x0600186A RID: 6250 RVA: 0x0016C33C File Offset: 0x0016A53C
	// (remove) Token: 0x0600186B RID: 6251 RVA: 0x0016C374 File Offset: 0x0016A574
	public event PlayerApartmentController.FurnitureChange OnFurnitureChange;

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x0600186C RID: 6252 RVA: 0x0016C3A9 File Offset: 0x0016A5A9
	public static PlayerApartmentController Instance
	{
		get
		{
			return PlayerApartmentController._instance;
		}
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x0016C3B0 File Offset: 0x0016A5B0
	private void Awake()
	{
		if (PlayerApartmentController._instance != null && PlayerApartmentController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			PlayerApartmentController._instance = this;
		}
		base.enabled = false;
		this.SortSwatches();
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x0016C3EC File Offset: 0x0016A5EC
	[Button(null, 0)]
	public void SortSwatches()
	{
		this.swatches.Sort((Color p1, Color p2) => this.Step(p1).CompareTo(this.Step(p2)));
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x0016C408 File Offset: 0x0016A608
	public int Step(Color colour)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		Color.RGBToHSV(colour, ref num, ref num2, ref num3);
		return Mathf.RoundToInt(num * 1000f) + Mathf.RoundToInt(num2 * 10f) + Mathf.RoundToInt(num3 * 100f);
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x0016C45C File Offset: 0x0016A65C
	public void BuyNewResidence(ResidenceController newHome, bool removePreviousResidence = false)
	{
		if (Player.Instance.residence != null && removePreviousResidence)
		{
			ResidenceController residence = Player.Instance.residence;
			Player.Instance.residence.address.RemoveAllInhabitantFurniture(true, FurnitureClusterLocation.RemoveInteractablesOption.moveToStorage);
		}
		if (!Player.Instance.apartmentsOwned.Contains(newHome.address))
		{
			Player.Instance.apartmentsOwned.Add(newHome.address);
		}
		Player.Instance.SetResidence(newHome, removePreviousResidence);
		if (Player.Instance.residence != null)
		{
			Player.Instance.residence.address.RemoveAllInhabitantFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.remove);
			foreach (NewNode.NodeAccess nodeAccess in Player.Instance.residence.address.entrances)
			{
				if (nodeAccess.door != null)
				{
					if (nodeAccess.door.isJammed)
					{
						nodeAccess.door.SetJammed(false, null, true);
					}
					if (nodeAccess.door.policeTape != null)
					{
						Object.Destroy(nodeAccess.door.policeTape);
						Game.Log("Removing police tape at " + nodeAccess.door.name, 2);
					}
				}
			}
		}
		Player.Instance.AddToKeyring(newHome.address, true);
		CityData.Instance.CreateCityDirectory();
		CitizenBehaviour.Instance.UpdateForSale();
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x0016C5DC File Offset: 0x0016A7DC
	public void SetFurniturePlacementMode(bool val, PlayerApartmentController.FurniturePlacement newPlacement, NewRoom forRoom, bool newPlaceExistingRoomObject = false, bool forceUpdate = false)
	{
		if (this.furniturePlacementMode != val || forceUpdate)
		{
			this.furniturePlacementMode = val;
			Game.Log("Decor: Set furniture placement mode: " + this.furniturePlacementMode.ToString(), 2);
			if (this.furniturePlacementMode)
			{
				this.furnitureRotation = 0;
				this.placeExistingRoomObject = newPlaceExistingRoomObject;
				if (this.decoratingMode)
				{
					this.SetDecoratingMode(false, null, MaterialGroupPreset.MaterialType.walls, null, null);
				}
				if (this.GetExistingFurniture() != null)
				{
					string text = "Decor: Removing existing furniture ";
					FurnitureLocation existingFurniture = this.GetExistingFurniture();
					Game.Log(text + ((existingFurniture != null) ? existingFurniture.ToString() : null), 2);
					this.GetExistingFurniture().Delete(true, FurnitureClusterLocation.RemoveInteractablesOption.moveToStorage);
				}
				this.furnPlacement = newPlacement;
				this.furnPlacementRoom = forRoom;
				base.enabled = true;
				if (BioScreenController.Instance.isOpen)
				{
					BioScreenController.Instance.SetInventoryOpen(false, true, true);
				}
				FirstPersonItemController.Instance.SetFirstPersonItem(this.furnitureFPSItem, true);
			}
			else
			{
				this.placeExistingRoomObject = false;
				this.ResetExisting();
				if (this.materialKeyWindow != null)
				{
					Game.Log("Decor: Closing material key window", 2);
					this.materialKeyWindow.CloseWindow(false);
				}
				if (BioScreenController.Instance.isOpen)
				{
					BioScreenController.Instance.SetInventoryOpen(false, true, true);
				}
				FirstPersonItemController.InventorySlot inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster);
				if (inventorySlot != null)
				{
					BioScreenController.Instance.SelectSlot(inventorySlot, false, true);
				}
			}
			FirstPersonItemController.Instance.UpdateCurrentActions();
			InterfaceController.Instance.UpdateDOF();
		}
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x0016C768 File Offset: 0x0016A968
	public InfoWindow OpenOrUpdateMaterialWindow(FurniturePreset furn, Toolbox.MaterialKey useKey, MaterialGroupPreset newSelection)
	{
		if (this.materialKeyWindow != null)
		{
			Game.Log("Decor: Found an existing new material window...", 2);
		}
		else
		{
			Game.Log("Decor: Opening a new material window...", 2);
			this.materialKeyWindow = InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "MaterialKey", false, true, default(Vector2), null, null, null, true);
		}
		MaterialKeyController componentInChildren = this.materialKeyWindow.GetComponentInChildren<MaterialKeyController>();
		if (furn != null)
		{
			componentInChildren.UpdateButtonsBasedOnFurniture(furn);
		}
		else
		{
			componentInChildren.UpdateButtonsBasedOnMaterial(newSelection.material, false, MaterialKeyController.SliderPickerType.grub);
			componentInChildren.SetButtonsToKey(useKey);
		}
		componentInChildren.UpdatePlacementText();
		return this.materialKeyWindow;
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x0016C804 File Offset: 0x0016AA04
	public void SetDecoratingMode(bool val, MaterialGroupPreset materialPreset, MaterialGroupPreset.MaterialType editType = MaterialGroupPreset.MaterialType.walls, Toolbox.MaterialKey editKey = null, NewRoom forRoom = null)
	{
		if (this.decoratingMode != val)
		{
			this.decoratingMode = val;
			Game.Log("Decor: Set decorating mode: " + this.decoratingMode.ToString(), 2);
			if (this.decoratingMode)
			{
				if (this.furniturePlacementMode)
				{
					this.SetFurniturePlacementMode(false, null, null, false, false);
				}
				this.decoratingMaterial = materialPreset;
				this.decoratingType = editType;
				this.decoratingRoom = forRoom;
				base.enabled = true;
				if (BioScreenController.Instance.isOpen)
				{
					BioScreenController.Instance.SetInventoryOpen(false, true, true);
				}
				FirstPersonItemController.Instance.SetFirstPersonItem(this.furnitureFPSItem, true);
				this.ApplyDecor(this.decoratingType, this.decoratingMaterial, this.decoratingKey, false);
			}
			else
			{
				this.RevertDecor(this.decoratingType);
				if (this.materialKeyWindow != null)
				{
					Game.Log("Decor: Closing material key window", 2);
					this.materialKeyWindow.CloseWindow(false);
				}
				if (BioScreenController.Instance.isOpen)
				{
					BioScreenController.Instance.SetInventoryOpen(false, true, true);
				}
				BioScreenController.Instance.SelectSlot(FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, true);
			}
			InteractionController.Instance.UpdateInteractionText();
			InterfaceController.Instance.UpdateDOF();
		}
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x0016C958 File Offset: 0x0016AB58
	private void Update()
	{
		if (this.furniturePlacementMode)
		{
			bool flag = false;
			if (this.spawnedPlacementObj == null && this.furnPlacement.preset != null)
			{
				Game.Log("Decor: Creating furniture spawn object " + this.furnPlacement.preset.name, 2);
				this.spawnedPlacementObj = Object.Instantiate<GameObject>(this.furnPlacement.preset.prefab, Player.Instance.transform);
				this.spawnedPlacementObj.transform.localPosition = Vector3.zero;
				this.spawnedPlacementMesh = this.spawnedPlacementObj.GetComponent<MeshRenderer>();
				this.placementColliders = Enumerable.ToList<Collider>(this.spawnedPlacementObj.GetComponentsInChildren<Collider>());
				for (int i = 0; i < this.placementColliders.Count; i++)
				{
					if (this.placementColliders[i].isTrigger)
					{
						this.placementColliders.RemoveAt(i);
						i--;
					}
					else
					{
						this.placementColliders[i].isTrigger = true;
					}
				}
				this.UpdatePlacementColourKey();
				flag = true;
			}
			this.isPlacementValid = true;
			this.placementNode = this.UpdateFurnitureDesiredPosition();
			this.furnPlacement.angle = this.furnitureRotation;
			if (this.placementNode != null && this.spawnedPlacementObj != null)
			{
				this.furnPlacement.anchorNode = this.placementNode;
				this.furnPlacement.coversNodes = new List<NewNode>();
				float num = this.placementNode.position.y;
				if (this.furnPlacement.preset.classes[0].ceilingPiece)
				{
					num += (float)this.placementNode.floor.defaultCeilingHeight * 0.1f - (float)this.placementNode.floorHeight * 0.1f;
				}
				this.placementCursor.position = new Vector3(this.placementCursor.position.x, num, this.placementCursor.position.z);
				this.furnPlacement.offset = this.placementCursor.position - this.placementNode.position;
				this.furnPlacement.offset = new Vector3(this.furnPlacement.offset.x, 0f, this.furnPlacement.offset.z);
				Toolbox.Instance.SetLightLayer(this.spawnedPlacementObj, Player.Instance.currentBuilding, false);
				if (this.isPlacementValid)
				{
					int num2 = 0;
					while ((float)num2 < this.furnPlacement.preset.classes[0].objectSize.x)
					{
						int num3 = 0;
						while ((float)num3 < this.furnPlacement.preset.classes[0].objectSize.y)
						{
							Vector2 vector = Toolbox.Instance.RotateVector2CW(new Vector2((float)num2, (float)num3), (float)this.furnitureRotation);
							Vector3Int vector3Int = this.placementNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
							NewNode newNode = null;
							if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
							{
								goto IL_6DC;
							}
							this.furnPlacement.coversNodes.Add(newNode);
							if (newNode.gameLocation != this.furnPlacementRoom.gameLocation)
							{
								Game.Log("Decor: Different room", 2);
								this.isPlacementValid = false;
								break;
							}
							if (!this.furnPlacement.preset.classes[0].allowedOnStairwell && newNode.tile.isStairwell)
							{
								Game.Log("Decor: Is Stairwell", 2);
								this.isPlacementValid = false;
								break;
							}
							if (newNode.gameLocation.thisAsStreet == null && !this.furnPlacement.preset.classes[0].allowIfNoFloor && !newNode.HasValidFloor())
							{
								Game.Log("Decor: no floor", 2);
								this.isPlacementValid = false;
								break;
							}
							if (this.furnPlacement.preset.classes[0].ceilingPiece)
							{
								if (!newNode.HasValidCeiling())
								{
									Game.Log("Decor: Ceiling piece", 2);
									this.isPlacementValid = false;
									break;
								}
								if (!newNode.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level == 1))
								{
									if (newNode.floor.defaultCeilingHeight > 42)
									{
										if (newNode.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level == 2))
										{
											goto IL_475;
										}
									}
									if (newNode.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes[0].ceilingPiece && item.furniture.classes[0].blocksCeiling))
									{
										Game.Log("Decor: Ceiling piece", 2);
										this.isPlacementValid = false;
										break;
									}
									goto IL_50D;
								}
								IL_475:
								Game.Log("Decor: Ceiling piece", 2);
								this.isPlacementValid = false;
								break;
							}
							else if (this.furnPlacement.preset.classes[0].requiresCeiling && !newNode.HasValidCeiling())
							{
								Game.Log("Decor: Requires ceiling", 2);
								this.isPlacementValid = false;
								break;
							}
							IL_50D:
							int num4 = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
							{
								default(int),
								29
							});
							using (List<Collider>.Enumerator enumerator = this.placementColliders.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									Collider collider = enumerator.Current;
									foreach (Collider collider2 in Enumerable.ToList<Collider>(Physics.OverlapBox(collider.bounds.center, collider.bounds.extents * 0.9f, Quaternion.identity, num4, 1)))
									{
										bool flag2 = false;
										foreach (NewAddress newAddress in Player.Instance.apartmentsOwned)
										{
											foreach (NewRoom newRoom in newAddress.rooms)
											{
												if (newRoom.combinedFloor != null && collider2.gameObject == newRoom.combinedFloor)
												{
													flag2 = true;
													break;
												}
												if (newRoom.combinedCeiling != null && collider2.gameObject == newRoom.combinedCeiling)
												{
													flag2 = true;
													break;
												}
												if (flag2)
												{
													break;
												}
											}
											if (flag2)
											{
												break;
											}
										}
										if (!flag2 && !this.placementColliders.Contains(collider2))
										{
											Game.Log("Decor: Overlapping fail: " + collider.name + " overlaps " + collider2.name, 2);
											this.isPlacementValid = false;
											break;
										}
									}
									if (!this.isPlacementValid)
									{
										break;
									}
								}
								goto IL_6EE;
							}
							goto IL_6DC;
							IL_6EE:
							if (this.isPlacementValid)
							{
								num3++;
								continue;
							}
							break;
							IL_6DC:
							Game.Log("Decor: Invalid node", 2);
							this.isPlacementValid = false;
							goto IL_6EE;
						}
						if (!this.isPlacementValid)
						{
							break;
						}
						num2++;
					}
				}
				else
				{
					Game.Log("Decor: Invalid node", 2);
					this.isPlacementValid = false;
				}
			}
			else
			{
				Game.Log("Decor: Invalid node", 2);
				this.isPlacementValid = false;
			}
			if (flag)
			{
				this.spawnedPlacementObj.transform.position = this.placementCursor.transform.position;
				this.spawnedPlacementObj.transform.rotation = this.placementCursor.transform.rotation;
			}
			else
			{
				float num5 = Time.deltaTime * 20f;
				if (Vector3.Distance(this.spawnedPlacementObj.transform.position, this.placementCursor.transform.position) < 0.1f)
				{
					num5 = 1f;
				}
				this.spawnedPlacementObj.transform.position = Vector3.Lerp(this.spawnedPlacementObj.transform.position, this.placementCursor.transform.position, num5);
				this.spawnedPlacementObj.transform.rotation = Quaternion.Slerp(this.spawnedPlacementObj.transform.rotation, this.placementCursor.transform.rotation, num5);
			}
			Color color = this.placementValidLerpColour;
			if (!this.isPlacementValid)
			{
				color = this.placementInvalidLerpColour;
			}
			this.materialPulse += Time.deltaTime;
			if (this.materialPulse > 2f)
			{
				this.materialPulse = 0f;
			}
			if (this.materialPulse <= 1f)
			{
				this.lerpColour = Color.Lerp(Color.black, color, this.materialPulse);
				using (List<Material>.Enumerator enumerator5 = this.cloneMaterials.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						Material material = enumerator5.Current;
						material.SetColor("_EmissionColour", this.lerpColour);
						material.SetColor("_EmissiveColor", this.lerpColour);
					}
					goto IL_999;
				}
			}
			this.lerpColour = Color.Lerp(color, Color.black, this.materialPulse - 1f);
			foreach (Material material2 in this.cloneMaterials)
			{
				material2.SetColor("_EmissionColour", this.lerpColour);
				material2.SetColor("_EmissiveColor", this.lerpColour);
			}
			IL_999:
			if (this.furnPlacementRoom == null || Player.Instance.currentGameLocation != this.furnPlacementRoom.gameLocation)
			{
				this.SetFurniturePlacementMode(false, null, null, false, false);
				return;
			}
		}
		else if (this.decoratingMode)
		{
			if (this.decoratingRoom == null || Player.Instance.currentGameLocation != this.decoratingRoom.gameLocation)
			{
				this.SetDecoratingMode(false, null, MaterialGroupPreset.MaterialType.walls, null, null);
				return;
			}
		}
		else
		{
			this.RemoveBeingPlaced();
			base.enabled = false;
		}
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x0016D414 File Offset: 0x0016B614
	public void RemoveBeingPlaced()
	{
		if (this.spawnedPlacementObj != null)
		{
			Object.Destroy(this.spawnedPlacementObj);
		}
		foreach (Material material in this.cloneMaterials)
		{
			Object.Destroy(material);
		}
		this.cloneMaterials.Clear();
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x0016D488 File Offset: 0x0016B688
	public FurnitureLocation GetExistingFurniture()
	{
		if (this.furnPlacement != null && this.furnPlacement.existing != null && this.furnPlacement.existing.id != 0)
		{
			return this.furnPlacement.existing;
		}
		return null;
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x0016D4C0 File Offset: 0x0016B6C0
	public void UpdatePlacementColourKey()
	{
		if (this.spawnedPlacementObj != null && this.furniturePlacementMode)
		{
			Game.Log("Decor: Updating placement colour key...", 2);
			MeshRenderer[] componentsInChildren = this.spawnedPlacementObj.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer renderer in componentsInChildren)
			{
				MaterialsController.Instance.ApplyMaterialKey(renderer, this.furnPlacement.materialKey);
			}
			while (this.cloneMaterials.Count > 0)
			{
				Object.Destroy(this.cloneMaterials[0]);
				this.cloneMaterials.RemoveAt(0);
			}
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				Material sharedMaterial = meshRenderer.sharedMaterial;
				if (sharedMaterial != null)
				{
					Material material = Object.Instantiate<Material>(sharedMaterial);
					this.cloneMaterials.Add(material);
					meshRenderer.sharedMaterial = material;
					if (this.furnPlacement.art != null && meshRenderer.transform.tag == "Art")
					{
						Material material2 = this.furnPlacement.art.material;
						Game.Log("Decor: Found art material, applying " + material2.name, 2);
						if (this.furnPlacement.art.useDynamicText && !GameplayController.Instance.dynamicTextImages.TryGetValue(this.furnPlacement.art, ref sharedMaterial))
						{
							material2 = this.furnPlacement.art.material;
						}
						meshRenderer.sharedMaterial = material2;
					}
				}
			}
			if (this.furnPlacement.art != null)
			{
				DecalProjector componentInChildren = this.spawnedPlacementObj.GetComponentInChildren<DecalProjector>();
				if (componentInChildren != null)
				{
					componentInChildren.material = this.furnPlacement.art.material;
					Texture texture = componentInChildren.material.GetTexture("_BaseColorMap");
					componentInChildren.size = new Vector3((float)texture.width * this.furnPlacement.art.pixelScaleMultiplier, (float)texture.height * this.furnPlacement.art.pixelScaleMultiplier, 0.13f);
				}
			}
		}
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x0016D6E4 File Offset: 0x0016B8E4
	private NewNode UpdateFurnitureDesiredPosition()
	{
		NewNode newNode = null;
		if (this.spawnedPlacementMesh != null)
		{
			this.placementCursor.localPosition = new Vector3(0f, 0f, this.spawnedPlacementMesh.bounds.size.z * 0.5f + 1f);
			float num = 0f;
			if (this.furnPlacement.preset.classes[0].objectSize.x > 1f)
			{
				num = (this.spawnedPlacementMesh.bounds.size.x - 1f) * -0.5f;
			}
			this.placementCursor.localPosition = new Vector3(num, this.placementCursor.transform.localPosition.y, this.spawnedPlacementMesh.bounds.size.z * 0.5f + 1f);
		}
		else
		{
			this.placementCursor.localPosition = new Vector3(0f, 0f, 1f);
		}
		this.placementCursor.eulerAngles = new Vector3(0f, (float)this.furnitureRotation, 0f);
		if (this.furnPlacement.preset.classes[0].wallPiece || this.furnPlacement.preset.classes[0].useWallSnappingInDecorMode)
		{
			Ray ray;
			ray..ctor(CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.forward);
			int num2 = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
			{
				29
			});
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, ref raycastHit, 10f, num2))
			{
				NewWall newWall = null;
				float num3 = 9999f;
				foreach (NewRoom newRoom in Player.Instance.currentGameLocation.rooms)
				{
					foreach (NewNode newNode2 in newRoom.nodes)
					{
						foreach (NewWall newWall2 in newNode2.walls)
						{
							if (newWall2.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance)
							{
								float num4 = Vector3.Distance(newWall2.position, raycastHit.point);
								if (num4 < num3)
								{
									newWall = newWall2;
									num3 = num4;
								}
							}
						}
					}
				}
				if (newWall != null)
				{
					this.furnitureRotation = Mathf.RoundToInt(newWall.localEulerAngles.y);
					this.placementCursor.position = newWall.node.position;
					newNode = newWall.node;
				}
				else
				{
					this.isPlacementValid = false;
					Game.Log("Decor: No valid wall placement", 2);
				}
			}
			else
			{
				this.isPlacementValid = false;
				Game.Log("Decor: Raycast does not hit a wall", 2);
			}
		}
		if (newNode == null)
		{
			Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(this.placementCursor.position);
			PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode);
		}
		this.placementCursor.transform.position = new Vector3((float)Mathf.RoundToInt(this.placementCursor.transform.position.x / 0.3f) * 0.3f, this.placementCursor.transform.position.y, (float)Mathf.RoundToInt(this.placementCursor.transform.position.z / 0.3f) * 0.3f);
		return newNode;
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x0016DAC8 File Offset: 0x0016BCC8
	public void RotateFurn(bool right)
	{
		if (PlayerApartmentController.Instance.furniturePlacementMode && this.furnPlacement != null)
		{
			if (right)
			{
				if (this.furnPlacement.preset.classes[0].objectSize.x + this.furnPlacement.preset.classes[0].objectSize.y > 2f)
				{
					this.AddFurnitureRotation(90);
					return;
				}
				this.AddFurnitureRotation(45);
				return;
			}
			else
			{
				if (this.furnPlacement.preset.classes[0].objectSize.x + this.furnPlacement.preset.classes[0].objectSize.y > 2f)
				{
					this.AddFurnitureRotation(-90);
					return;
				}
				this.AddFurnitureRotation(-45);
			}
		}
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x0016DBA8 File Offset: 0x0016BDA8
	public void AddFurnitureRotation(int angle)
	{
		this.furnitureRotation += angle;
		while (this.furnitureRotation >= 360)
		{
			this.furnitureRotation -= 360;
		}
		while (this.furnitureRotation < 0)
		{
			this.furnitureRotation += 360;
		}
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x0016DC04 File Offset: 0x0016BE04
	public void ExecutePlacement()
	{
		if (this.furnPlacement != null && this.furniturePlacementMode && this.isPlacementValid)
		{
			string text = "Decor: Execute furniture placement: ";
			string text2 = this.isPlacementValid.ToString();
			string text3 = ": ";
			NewNode newNode = this.placementNode;
			Game.Log(text + text2 + text3 + ((newNode != null) ? newNode.ToString() : null), 2);
			FurnitureLocation existingFurniture = this.GetExistingFurniture();
			if (existingFurniture != null || (this.furnPlacement.preset.cost <= GameplayController.Instance.money && this.placementNode != null))
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.furniturePlacement, null, 1f);
				if (existingFurniture == null)
				{
					Game.Log("Decor: Creating new furniture...", 2);
					this.placementNode.room.AddFurnitureCustom(this.furnPlacement);
					AudioController.Instance.Play2DSound(AudioControls.Instance.purchaseItem, null, 1f);
					GameplayController.Instance.AddMoney(-this.furnPlacement.preset.cost, true, "furniture purchase");
				}
				else
				{
					if (!this.furnitureStorage.Contains(existingFurniture))
					{
						this.MoveFurnitureToStorage(existingFurniture);
					}
					Game.Log("Decor: Placing existing furniture...", 2);
					existingFurniture.anchorNode = this.furnPlacement.anchorNode;
					existingFurniture.coversNodes = new List<NewNode>();
					existingFurniture.coversNodes.Add(existingFurniture.anchorNode);
					existingFurniture.offset = this.furnPlacement.offset;
					existingFurniture.angle = this.furnPlacement.angle;
					existingFurniture.scaleMultiplier = Vector3.one;
					this.placementNode.room.AddFurnitureCustom(existingFurniture);
					this.RemoveFromStorage(existingFurniture);
					this.ResetExisting();
					this.SetFurniturePlacementMode(false, null, null, false, false);
				}
				if (this.OnFurnitureChange != null)
				{
					this.OnFurnitureChange();
				}
				ObjectPoolingController.Instance.UpdateObjectRanges();
				InteractionController.Instance.StartDecorEdit();
				return;
			}
		}
		else if (this.decoratingMode)
		{
			Game.Log("Decor: Execute decoration change", 2);
			int currentCost = this.GetCurrentCost();
			if (currentCost <= GameplayController.Instance.money)
			{
				this.ApplyDecor(this.decoratingType, this.decoratingMaterial, this.decoratingKey, true);
				AudioController.Instance.Play2DSound(AudioControls.Instance.furniturePlacement, null, 1f);
				AudioController.Instance.Play2DSound(AudioControls.Instance.purchaseItem, null, 1f);
				GameplayController.Instance.AddMoney(-currentCost, true, "decor purchase");
				this.SetDecoratingMode(false, null, MaterialGroupPreset.MaterialType.walls, null, null);
				InteractionController.Instance.StartDecorEdit();
			}
		}
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x0016DE85 File Offset: 0x0016C085
	public void ResetExisting()
	{
		Game.Log("Decor: Resetting existing furniture reference", 2);
		if (this.furnPlacement != null)
		{
			this.furnPlacement.existing = null;
		}
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x0016DEA8 File Offset: 0x0016C0A8
	public int GetCurrentCost()
	{
		int result = 0;
		if (this.furnPlacement != null && this.furniturePlacementMode && this.isPlacementValid && this.GetExistingFurniture() == null)
		{
			result = this.furnPlacement.preset.cost;
		}
		else if (this.decoratingMode)
		{
			if (this.decoratingType == MaterialGroupPreset.MaterialType.walls)
			{
				result = this.decoratingMaterial.price * this.decoratingRoom.GetWallCount();
			}
			else
			{
				result = this.decoratingMaterial.price * this.decoratingRoom.nodes.Count;
			}
		}
		return result;
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x0016DF34 File Offset: 0x0016C134
	public void CancelPlacement(bool restoreExistingPosition)
	{
		if (this.furniturePlacementMode)
		{
			Game.Log("Decor: Cancel furniture placement...", 2);
			FurnitureLocation existingFurniture = this.GetExistingFurniture();
			if (existingFurniture != null && restoreExistingPosition)
			{
				Game.Log("Decor: ... Restoring existing furniture...", 2);
				this.placementNode.room.AddFurnitureCustom(existingFurniture);
				this.RemoveFromStorage(existingFurniture);
				if (this.OnFurnitureChange != null)
				{
					this.OnFurnitureChange();
				}
				ObjectPoolingController.Instance.UpdateObjectRanges();
			}
			this.SetFurniturePlacementMode(false, null, null, false, false);
		}
		else if (this.decoratingMode)
		{
			Game.Log("Decor: Cancel decorating mode...", 2);
			this.SetDecoratingMode(false, null, MaterialGroupPreset.MaterialType.walls, null, null);
		}
		InteractionController.Instance.StartDecorEdit();
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x0016DFDC File Offset: 0x0016C1DC
	public void MoveFurnitureToStorage(FurnitureLocation newStorage)
	{
		if (newStorage == null)
		{
			return;
		}
		AudioController.Instance.Play2DSound(AudioControls.Instance.moveObjectsToStorage, null, 1f);
		if (!this.furnitureStorage.Contains(newStorage))
		{
			Game.Log("Decor: Moving " + newStorage.furniture.name + " to storage...", 2);
			if (this.furniturePlacementMode && this.GetExistingFurniture() == newStorage)
			{
				this.SetFurniturePlacementMode(false, null, null, false, false);
			}
			if (Player.Instance.currentRoom != null)
			{
				Player.Instance.currentRoom.decorEdit = true;
			}
			newStorage.Delete(true, FurnitureClusterLocation.RemoveInteractablesOption.moveToStorage);
			newStorage.fovDirection = Vector2.zero;
			newStorage.fovMaxDistance = 0;
			newStorage.useFOVBLock = false;
			newStorage.usage.Clear();
			newStorage.sublocations.Clear();
			newStorage.scaleMultiplier = Vector3.one;
			this.furnitureStorage.Add(newStorage);
			if (this.OnFurnitureChange != null)
			{
				this.OnFurnitureChange();
			}
		}
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x0016E0D8 File Offset: 0x0016C2D8
	public void RemoveFromStorage(FurnitureLocation newStorage)
	{
		if (newStorage == null)
		{
			return;
		}
		if (this.furnitureStorage.Contains(newStorage))
		{
			Game.Log("Decor: Removing " + newStorage.furniture.name + " from storage...", 2);
			if (Player.Instance.currentRoom != null)
			{
				Player.Instance.currentRoom.decorEdit = true;
			}
			this.furnitureStorage.Remove(newStorage);
			if (this.OnFurnitureChange != null)
			{
				this.OnFurnitureChange();
			}
		}
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x0016E15C File Offset: 0x0016C35C
	public void SellFurniture(FurnitureLocation newSell)
	{
		if (newSell == null)
		{
			return;
		}
		Game.Log("Decor: Selling " + newSell.furniture.name + "...", 2);
		if (Player.Instance.currentRoom != null)
		{
			Player.Instance.currentRoom.decorEdit = true;
		}
		this.RemoveFromStorage(newSell);
		if (this.furniturePlacementMode && this.GetExistingFurniture() == newSell)
		{
			this.SetFurniturePlacementMode(false, null, null, false, false);
		}
		AudioController.Instance.Play2DSound(AudioControls.Instance.pickUpMoney, null, 1f);
		GameplayController.Instance.AddMoney(Mathf.RoundToInt((float)newSell.furniture.cost * 0.5f), true, "Sell furniture");
		newSell.Delete(true, FurnitureClusterLocation.RemoveInteractablesOption.moveToStorage);
		if (this.OnFurnitureChange != null)
		{
			this.OnFurnitureChange();
		}
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x0016E230 File Offset: 0x0016C430
	public void MoveItemToStorage(Interactable newStorage)
	{
		if (newStorage == null)
		{
			return;
		}
		if (!this.itemStorage.Contains(newStorage))
		{
			if (!newStorage.preset.allowInApartmentStorage)
			{
				Game.Log("Decor: " + newStorage.GetName() + " is not allowed in apartment storage, deleting...", 2);
				newStorage.SafeDelete(false);
				return;
			}
			Game.Log("Decor: Moving " + newStorage.GetName() + " to storage...", 2);
			if (Player.Instance.currentRoom != null)
			{
				Player.Instance.currentRoom.decorEdit = true;
			}
			if (InteractionController.Instance.carryingObject != null && InteractionController.Instance.carryingObject.interactable == newStorage)
			{
				InteractionController.Instance.carryingObject.DropThis(false);
			}
			if (newStorage.phy)
			{
				newStorage.SetPhysicsPickupState(false, null, true, false);
			}
			if (newStorage.inInventory != null)
			{
				newStorage.SetAsNotInventory(Player.Instance.currentNode);
			}
			newStorage.MarkAsTrash(false, false, 0f);
			this.itemStorage.Add(newStorage);
			newStorage.RemoveFromPlacement();
			if (this.OnFurnitureChange != null)
			{
				this.OnFurnitureChange();
			}
		}
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x0016E358 File Offset: 0x0016C558
	public void RemoveItemFromStorage(Interactable newStorage)
	{
		if (newStorage == null)
		{
			return;
		}
		if (this.itemStorage.Contains(newStorage))
		{
			Game.Log("Decor: Removing " + newStorage.GetName() + " from storage...", 2);
			this.itemStorage.Remove(newStorage);
			newStorage.rPl = false;
			if (this.OnFurnitureChange != null)
			{
				this.OnFurnitureChange();
			}
		}
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x0016E3BC File Offset: 0x0016C5BC
	public void SellItem(Interactable newSell)
	{
		if (newSell == null)
		{
			return;
		}
		Game.Log("Decor: Selling " + newSell.GetName() + "...", 2);
		this.itemStorage.Remove(newSell);
		AudioController.Instance.Play2DSound(AudioControls.Instance.pickUpMoney, null, 1f);
		GameplayController.Instance.AddMoney(Mathf.RoundToInt(newSell.val * 0.5f), true, "Sell furniture");
		newSell.SafeDelete(true);
		if (this.OnFurnitureChange != null)
		{
			this.OnFurnitureChange();
		}
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x0016E44B File Offset: 0x0016C64B
	public void UpdateDecorColourKey()
	{
		if (this.decoratingMode)
		{
			this.ApplyDecor(this.decoratingType, this.decoratingMaterial, this.decoratingKey, false);
		}
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x0016E470 File Offset: 0x0016C670
	public void ApplyDecor(MaterialGroupPreset.MaterialType decorType, MaterialGroupPreset material, Toolbox.MaterialKey key, bool saveChanges)
	{
		if (material != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Decor: Apply decor ",
				decorType.ToString(),
				" using ",
				material.name,
				", save changes: ",
				saveChanges.ToString()
			}), 2);
			if (decorType == MaterialGroupPreset.MaterialType.walls)
			{
				if (saveChanges)
				{
					this.decoratingRoom.defaultWallKey = key;
					this.decoratingRoom.defaultWallMaterial = material;
					this.decoratingRoom.wallMat = MaterialsController.Instance.SetMaterialGroup(this.decoratingRoom.combinedWalls, material, key, true, null);
				}
				else
				{
					MaterialsController.Instance.SetMaterialGroup(this.decoratingRoom.combinedWalls, material, key, false, null);
				}
			}
			else if (decorType == MaterialGroupPreset.MaterialType.floor)
			{
				if (saveChanges)
				{
					this.decoratingRoom.floorMatKey = key;
					this.decoratingRoom.floorMaterial = material;
					this.decoratingRoom.floorMat = MaterialsController.Instance.SetMaterialGroup(this.decoratingRoom.combinedFloor, material, key, true, null);
				}
				else
				{
					MaterialsController.Instance.SetMaterialGroup(this.decoratingRoom.combinedFloor, material, key, false, null);
				}
			}
			else if (decorType == MaterialGroupPreset.MaterialType.ceiling)
			{
				if (saveChanges)
				{
					this.decoratingRoom.ceilingMatKey = key;
					this.decoratingRoom.ceilingMaterial = material;
					this.decoratingRoom.ceilingMat = MaterialsController.Instance.SetMaterialGroup(this.decoratingRoom.combinedCeiling, material, key, true, null);
				}
				else
				{
					MaterialsController.Instance.SetMaterialGroup(this.decoratingRoom.combinedCeiling, material, key, false, null);
				}
			}
			if (saveChanges)
			{
				this.decoratingRoom.decorEdit = true;
			}
		}
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x0016E610 File Offset: 0x0016C810
	public void RevertDecor(MaterialGroupPreset.MaterialType decorType)
	{
		Game.Log("Decor: Reverting decor for " + this.decoratingRoom.name + " " + decorType.ToString(), 2);
		if (decorType == MaterialGroupPreset.MaterialType.walls)
		{
			Material wallMat = this.decoratingRoom.wallMat;
			if (this.decoratingRoom.combinedWallRend != null)
			{
				this.decoratingRoom.combinedWallRend.material = this.decoratingRoom.wallMat;
				return;
			}
		}
		else if (decorType == MaterialGroupPreset.MaterialType.floor)
		{
			Material floorMat = this.decoratingRoom.floorMat;
			if (this.decoratingRoom.combinedFloorRend != null)
			{
				this.decoratingRoom.combinedFloorRend.material = this.decoratingRoom.floorMat;
				return;
			}
		}
		else if (decorType == MaterialGroupPreset.MaterialType.ceiling)
		{
			Material ceilingMat = this.decoratingRoom.ceilingMat;
			if (this.decoratingRoom.combinedCeilingRend != null)
			{
				this.decoratingRoom.combinedCeilingRend.material = this.decoratingRoom.ceilingMat;
			}
		}
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x0016E708 File Offset: 0x0016C908
	public void PlaceIndividualCluster(FurnitureCluster cluster, NewAddress address)
	{
		Dictionary<FurnitureClusterLocation, NewRoom> dictionary = new Dictionary<FurnitureClusterLocation, NewRoom>();
		foreach (NewRoom newRoom in address.rooms)
		{
			foreach (FurnitureClusterLocation furnitureClusterLocation in GenerationController.Instance.GetPossibleFurnitureClusterLocations(newRoom, cluster, cluster.enableDebug))
			{
				dictionary.Add(furnitureClusterLocation, newRoom);
			}
		}
		if (dictionary.Count > 0)
		{
			FurnitureClusterLocation furnitureClusterLocation2 = null;
			NewRoom newRoom2 = null;
			foreach (KeyValuePair<FurnitureClusterLocation, NewRoom> keyValuePair in dictionary)
			{
				if (furnitureClusterLocation2 == null || keyValuePair.Key.ranking > furnitureClusterLocation2.ranking)
				{
					furnitureClusterLocation2 = keyValuePair.Key;
					newRoom2 = keyValuePair.Value;
				}
			}
			if (furnitureClusterLocation2 != null)
			{
				newRoom2.decorEdit = true;
				newRoom2.AddFurniture(furnitureClusterLocation2, true, true, false);
				Game.Log("Added " + cluster.name + " to room " + newRoom2.name, 2);
			}
		}
	}

	// Token: 0x04001E58 RID: 7768
	[Header("Components")]
	public Transform placementCursor;

	// Token: 0x04001E59 RID: 7769
	[Header("Settings")]
	public Color placementValidLerpColour = Color.gray;

	// Token: 0x04001E5A RID: 7770
	public Color placementInvalidLerpColour = Color.red;

	// Token: 0x04001E5B RID: 7771
	public FirstPersonItem furnitureFPSItem;

	// Token: 0x04001E5C RID: 7772
	public FurnitureCluster nullCluster;

	// Token: 0x04001E5D RID: 7773
	[Header("State")]
	public List<Color> swatches = new List<Color>();

	// Token: 0x04001E5E RID: 7774
	public List<FurnitureLocation> furnitureStorage = new List<FurnitureLocation>();

	// Token: 0x04001E5F RID: 7775
	public List<Interactable> itemStorage = new List<Interactable>();

	// Token: 0x04001E60 RID: 7776
	[Space(7f)]
	public bool furniturePlacementMode;

	// Token: 0x04001E61 RID: 7777
	public bool placeExistingRoomObject;

	// Token: 0x04001E62 RID: 7778
	public PlayerApartmentController.FurniturePlacement furnPlacement;

	// Token: 0x04001E63 RID: 7779
	public NewRoom furnPlacementRoom;

	// Token: 0x04001E64 RID: 7780
	public GameObject spawnedPlacementObj;

	// Token: 0x04001E65 RID: 7781
	public MeshRenderer spawnedPlacementMesh;

	// Token: 0x04001E66 RID: 7782
	public int furnitureRotation;

	// Token: 0x04001E67 RID: 7783
	public List<Material> cloneMaterials = new List<Material>();

	// Token: 0x04001E68 RID: 7784
	public List<Collider> placementColliders = new List<Collider>();

	// Token: 0x04001E69 RID: 7785
	public float materialPulse;

	// Token: 0x04001E6A RID: 7786
	public Color lerpColour = Color.black;

	// Token: 0x04001E6B RID: 7787
	[Space(7f)]
	public NewNode placementNode;

	// Token: 0x04001E6C RID: 7788
	public bool isPlacementValid;

	// Token: 0x04001E6D RID: 7789
	[Space(7f)]
	public bool decoratingMode;

	// Token: 0x04001E6E RID: 7790
	public MaterialGroupPreset decoratingMaterial;

	// Token: 0x04001E6F RID: 7791
	public MaterialGroupPreset.MaterialType decoratingType;

	// Token: 0x04001E70 RID: 7792
	public Toolbox.MaterialKey decoratingKey;

	// Token: 0x04001E71 RID: 7793
	public NewRoom decoratingRoom;

	// Token: 0x04001E72 RID: 7794
	[Space(7f)]
	public InfoWindow materialKeyWindow;

	// Token: 0x04001E73 RID: 7795
	[Space(7f)]
	public WindowTabPreset.TabContentType rememberContent = WindowTabPreset.TabContentType.decor;

	// Token: 0x04001E74 RID: 7796
	public MaterialGroupPreset.MaterialType rememberDecorType;

	// Token: 0x04001E75 RID: 7797
	public FurnishingsController.TabState rememberRoomStorageShop;

	// Token: 0x04001E76 RID: 7798
	public List<FurniturePreset.DecorClass> rememberDisplayClasses = new List<FurniturePreset.DecorClass>();

	// Token: 0x04001E77 RID: 7799
	public List<InteractablePreset.ItemClass> rememberItemDisplayClasses = new List<InteractablePreset.ItemClass>();

	// Token: 0x04001E79 RID: 7801
	private static PlayerApartmentController _instance;

	// Token: 0x02000447 RID: 1095
	[Serializable]
	public class PlayerFurniture
	{
		// Token: 0x04001E7A RID: 7802
		public Toolbox.MaterialKey matKey;

		// Token: 0x04001E7B RID: 7803
		public string presetStr;

		// Token: 0x04001E7C RID: 7804
		[NonSerialized]
		public FurniturePreset preset;

		// Token: 0x04001E7D RID: 7805
		[NonSerialized]
		public FurnitureLocation placement;
	}

	// Token: 0x02000448 RID: 1096
	[Serializable]
	public class FurniturePlacement
	{
		// Token: 0x04001E7E RID: 7806
		public FurniturePreset preset;

		// Token: 0x04001E7F RID: 7807
		public FurnitureLocation existing;

		// Token: 0x04001E80 RID: 7808
		public Toolbox.MaterialKey materialKey;

		// Token: 0x04001E81 RID: 7809
		public ArtPreset art;

		// Token: 0x04001E82 RID: 7810
		public NewNode anchorNode;

		// Token: 0x04001E83 RID: 7811
		public List<NewNode> coversNodes = new List<NewNode>();

		// Token: 0x04001E84 RID: 7812
		public int angle;

		// Token: 0x04001E85 RID: 7813
		public Vector3 offset = Vector3.zero;
	}

	// Token: 0x02000449 RID: 1097
	// (Invoke) Token: 0x0600188E RID: 6286
	public delegate void FurnitureChange();
}
