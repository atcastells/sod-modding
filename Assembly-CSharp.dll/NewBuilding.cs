using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000349 RID: 841
public class NewBuilding : Controller
{
	// Token: 0x060012E6 RID: 4838 RVA: 0x0010BA54 File Offset: 0x00109C54
	public void AddNewFloor(NewFloor newFloor)
	{
		if (!this.floors.ContainsKey(newFloor.floor))
		{
			this.floors.Add(newFloor.floor, newFloor);
			newFloor.building = this;
			foreach (NewAddress newAddress in newFloor.addresses)
			{
				newAddress.building = this;
				foreach (NewRoom newRoom in newAddress.rooms)
				{
					newRoom.building = this;
					foreach (NewNode newNode in newRoom.nodes)
					{
						newNode.building = this;
					}
				}
			}
		}
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x0010BB54 File Offset: 0x00109D54
	public void Setup(CityTile newGroundmap, BuildingPreset newpreset, bool setupExistingBuilding = false, int replaceBuildingID = 0)
	{
		this.buildingID = NewBuilding.assignID;
		NewBuilding.assignID++;
		base.name = "Building " + this.buildingID.ToString();
		base.transform.name = base.name;
		this.seed = (Toolbox.Instance.SeedRand(0, 99999999) * this.buildingID).ToString();
		this.cityTile = newGroundmap;
		this.cityTile.building = this;
		this.globalTileCoords = CityData.Instance.CityTileToTile(this.cityTile.cityCoord);
		this.preset = newpreset;
		if (this.preset.defaultExteriorWallMaterial.Count > 0)
		{
			this.SetExteriorWallMaterialDefault(this.preset.defaultExteriorWallMaterial[Toolbox.Instance.GetPsuedoRandomNumber(0, this.preset.defaultExteriorWallMaterial.Count, this.buildingID.ToString(), false)]);
		}
		if (this.cityTile.cityCoord.x % 2 != 0)
		{
			this.interiorLightCullingLayer++;
		}
		if (this.cityTile.cityCoord.y % 2 != 0)
		{
			this.interiorLightCullingLayer += 2;
		}
		this.CreateEvidence();
		if (!setupExistingBuilding)
		{
			HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Add(this);
		}
		else
		{
			int num = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.FindIndex((NewBuilding x) => x.buildingID == replaceBuildingID);
			HighlanderSingleton<CityBuildings>.Instance.buildingDirectory[num] = this;
		}
		this.UpdateColourSchemeAndMaterials();
		this.SetupModel();
		this.SpawnNeonSideSigns();
		if (this.preset.featuresSmokestack)
		{
			CitizenBehaviour.Smokestack smokestack = new CitizenBehaviour.Smokestack
			{
				building = this,
				timer = Toolbox.Instance.Rand(this.preset.spawnInterval.x, this.preset.spawnInterval.y, false)
			};
			if (AudioDebugging.Instance.overrideSmokeStackEmissionFrequency && Game.Instance.devMode)
			{
				smokestack.timer = AudioDebugging.Instance.chemSmokeStackEmissionFrequency / 60f;
			}
			CitizenBehaviour.Instance.smokestacks.Add(smokestack);
		}
		this.worldPosition = base.transform.position;
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x0010BD94 File Offset: 0x00109F94
	public void RemoveBuilding()
	{
		if (this.evidenceEntry != null)
		{
			GameplayController.Instance.evidenceDictionary.Remove(this.evidenceEntry.evID);
			GameplayController.Instance.singletonEvidence.Remove(this.evidenceEntry);
			CityConstructor.Instance.evidenceToCompile.Remove(this.evidenceEntry);
		}
		if (this.residentRoster != null)
		{
			GameplayController.Instance.evidenceDictionary.Remove(this.residentRoster.evID);
			GameplayController.Instance.singletonEvidence.Remove(this.residentRoster);
			CityConstructor.Instance.evidenceToCompile.Remove(this.residentRoster);
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x0010BE4C File Offset: 0x0010A04C
	public void Load(CitySaveData.BuildingCitySave data, CityTile newCityTile)
	{
		this.buildingID = data.buildingID;
		NewBuilding.assignID = Mathf.Max(NewBuilding.assignID, this.buildingID + 1);
		this.CreateEvidence();
		this.cityTile = newCityTile;
		this.cityTile.building = this;
		this.globalTileCoords = CityData.Instance.CityTileToTile(this.cityTile.cityCoord);
		Toolbox.Instance.LoadDataFromResources<BuildingPreset>(data.preset, out this.preset);
		if (this.cityTile.cityCoord.x % 2 != 0)
		{
			this.interiorLightCullingLayer++;
		}
		if (this.cityTile.cityCoord.y % 2 != 0)
		{
			this.interiorLightCullingLayer += 2;
		}
		if (this.preset.defaultExteriorWallMaterial.Count > 0)
		{
			this.SetExteriorWallMaterialDefault(this.preset.defaultExteriorWallMaterial[Toolbox.Instance.GetPsuedoRandomNumber(0, this.preset.defaultExteriorWallMaterial.Count, this.buildingID.ToString(), false)]);
		}
		HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Add(this);
		if (data.isInaccessible)
		{
			this.SetInaccessible();
		}
		Toolbox.Instance.LoadDataFromResources<DesignStylePreset>(data.designStyle, out this.designStyle);
		this.wood = data.wood;
		Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.floorMaterial, out this.floorMaterial);
		this.floorMatKey = data.floorMatKey;
		Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.ceilingMaterial, out this.ceilingMaterial);
		this.ceilingMatKey = data.ceilingMatKey;
		Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.defaultWallMaterial, out this.defaultWallMaterial);
		this.defaultWallKey = data.defaultWallKey;
		if (data.extWallMaterial != null && data.extWallMaterial.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.extWallMaterial, out this.extWallMaterial);
		}
		if (data.wallMatOverride != null && data.wallMatOverride.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.wallMatOverride, out this.defaultWallMaterialOverride);
		}
		if (data.ceilingMatOverride != null && data.ceilingMatOverride.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.ceilingMatOverride, out this.ceilingMaterialOverride);
		}
		if (data.floorMatOverride != null && data.floorMatOverride.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.floorMatOverride, out this.floorMaterialOverride);
		}
		if (data.wallMatOverrideB != null && data.wallMatOverrideB.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.wallMatOverrideB, out this.basementDefaultWallMaterialOverride);
		}
		if (data.ceilingMatOverrideB != null && data.ceilingMatOverrideB.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.ceilingMatOverrideB, out this.basementCeilingMaterialOverride);
		}
		if (data.floorMatOverrideB != null && data.floorMatOverrideB.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.floorMatOverrideB, out this.basementFloorMaterialOverride);
		}
		this.SetupModel();
		this.sideSigns = new List<NewBuilding.SideSign>(data.sideSigns);
		if (data.name != null)
		{
			base.name = data.name;
		}
		else
		{
			base.name = this.preset.name;
		}
		base.transform.name = base.name;
		this.SetupEmissionTexture();
		this.SetFacing(data.facing, true);
		this.SpawnNeonSideSigns();
		if (this.preset.featuresSmokestack)
		{
			CitizenBehaviour.Smokestack smokestack = new CitizenBehaviour.Smokestack
			{
				building = this,
				timer = Toolbox.Instance.Rand(this.preset.spawnInterval.x, this.preset.spawnInterval.y, true)
			};
			if (AudioDebugging.Instance.overrideSmokeStackEmissionFrequency && Game.Instance.devMode)
			{
				smokestack.timer = AudioDebugging.Instance.chemSmokeStackEmissionFrequency / 60f;
			}
			CitizenBehaviour.Instance.smokestacks.Add(smokestack);
		}
		this.worldPosition = base.transform.position;
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x0010C244 File Offset: 0x0010A444
	private void SetupModel()
	{
		if (!SessionData.Instance.isFloorEdit)
		{
			if (this.preset.prefab != null)
			{
				this.buildingModelBase = Object.Instantiate<GameObject>(this.preset.prefab, base.transform);
				Transform[] allTransforms = Toolbox.Instance.GetAllTransforms(this.buildingModelBase.transform);
				int renderingLayerMask = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
				{
					default(int),
					1,
					8
				});
				foreach (Transform transform in allTransforms)
				{
					if (transform.CompareTag("BuildingModel") && !this.buildingModelsActual.Contains(transform.gameObject))
					{
						this.buildingModelsActual.Add(transform.gameObject);
						transform.gameObject.isStatic = true;
						foreach (Transform transform2 in allTransforms)
						{
							if (transform2.name.Substring(3) == "_static_" + transform.name)
							{
								transform2.tag = "BuildingModel";
								transform2.gameObject.layer = 27;
								MeshRenderer component = transform2.GetComponent<MeshRenderer>();
								if (component != null)
								{
									component.renderingLayerMask = (uint)renderingLayerMask;
									component.staticShadowCaster = true;
								}
								if (!this.buildingModelsActual.Contains(transform2.gameObject))
								{
									this.buildingModelsActual.Add(transform2.gameObject);
								}
							}
						}
					}
					else if (transform.CompareTag("BuildingModelLights"))
					{
						if (!this.buildingModelsActual.Contains(transform.gameObject))
						{
							this.buildingModelsActual.Add(transform.gameObject);
							transform.gameObject.isStatic = true;
							foreach (Transform transform3 in allTransforms)
							{
								if (transform3.name.Substring(3) == "_static_" + transform.name)
								{
									transform3.tag = "BuildingModelLights";
									transform3.gameObject.layer = 27;
									MeshRenderer component2 = transform3.GetComponent<MeshRenderer>();
									if (component2 != null)
									{
										component2.renderingLayerMask = (uint)renderingLayerMask;
									}
									if (!this.buildingModelsActual.Contains(transform3.gameObject))
									{
										this.buildingModelsActual.Add(transform3.gameObject);
									}
								}
							}
						}
						if (!this.buildingModelsLights.Contains(transform.gameObject))
						{
							this.buildingModelsLights.Add(transform.gameObject);
						}
					}
					else if (transform.CompareTag("BuildingSnowCollider"))
					{
						transform.gameObject.layer = 2;
						transform.gameObject.isStatic = true;
						Collider component3 = transform.GetComponent<Collider>();
						if (component3 != null)
						{
							component3.isTrigger = true;
							this.snowColliders.Add(component3);
							PrecipitationParticleSystemController.Instance.AddAreaTrigger(component3);
						}
					}
				}
				this.buildingModelBase.tag = "BuildingModel";
				foreach (GameObject gameObject in this.buildingModelsActual)
				{
					foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
					{
						if (!collider.gameObject.CompareTag("BuildingSnowCollider") && !this.colliders.Contains(collider))
						{
							this.colliders.Add(collider);
						}
					}
				}
			}
			base.name = this.preset.name;
			this.SetupEmissionTexture();
			if (this.buildingModelBase != null)
			{
				this.environmentalSettingsObject = this.buildingModelBase.transform.Find("EnvironmentVolume");
			}
			if (this.environmentalSettingsObject != null)
			{
				this.SetupEnvironment();
			}
			if (HighlanderSingleton<CityEditorController>.Instance != null && SessionData.Instance.isCityEdit)
			{
				this.DrawGroundFloorBuildingModel();
			}
		}
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x0010C650 File Offset: 0x0010A850
	public void DrawGroundFloorBuildingModel()
	{
		if (this.cityEditorGroundFloorRepresentation == null && this.buildingModelBase != null)
		{
			this.cityEditorGroundFloorRepresentation = new GameObject("City Editor Ground Floor");
			this.cityEditorGroundFloorRepresentation.transform.SetParent(this.buildingModelBase.transform, false);
			this.cityEditorGroundFloorRepresentation.transform.localPosition = Vector3.zero;
			this.cityEditorGroundFloorRepresentation.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			if (this.preset.floorLayouts.Count > 0)
			{
				int num = 0;
				while ((float)num < MathF.Min(2f, (float)this.preset.floorLayouts.Count))
				{
					BuildingPreset.InteriorFloorSetting interiorFloorSetting = this.preset.floorLayouts[num];
					if (interiorFloorSetting.blueprints.Count > 0)
					{
						TextAsset textAsset = interiorFloorSetting.blueprints[0];
						if (textAsset == null)
						{
							Game.LogError("CityGen: Missing floor preset for " + base.name + " floor " + 0.ToString(), 2);
							return;
						}
						FloorSaveData floorSaveData = null;
						CityData.Instance.floorData.TryGetValue(textAsset.name, ref floorSaveData);
						if (floorSaveData == null)
						{
							Game.LogError("CityGen: No floor data for " + textAsset.name, 2);
							return;
						}
						for (int i = 0; i < floorSaveData.a_d.Count; i++)
						{
							AddressSaveData addressSaveData = floorSaveData.a_d[i];
							if (addressSaveData.vs.Count > 0)
							{
								AddressLayoutVariation addressLayoutVariation = addressSaveData.vs[0];
								LayoutConfiguration layoutPreset = null;
								if (Toolbox.Instance.LoadDataFromResources<LayoutConfiguration>(addressSaveData.p_n, out layoutPreset))
								{
									AddressPreset addressPreset = Toolbox.Instance.allAddressPresets.Find((AddressPreset item) => item.compatible.Contains(layoutPreset));
									foreach (RoomSaveData roomSaveData in addressLayoutVariation.r_d)
									{
										RoomTypePreset config = null;
										Toolbox.Instance.LoadDataFromResources<RoomTypePreset>(roomSaveData.l, out config);
										if (config != null)
										{
											List<MeshFilter> list = new List<MeshFilter>();
											List<MeshFilter> list2 = new List<MeshFilter>();
											new List<MeshFilter>();
											RoomConfiguration roomConfiguration = null;
											if (addressPreset != null)
											{
												roomConfiguration = addressPreset.roomConfig.Find((RoomConfiguration item) => item.roomType == config);
											}
											foreach (NodeSaveData nodeSaveData in roomSaveData.n_d)
											{
												if ((num == 0 || config.presetName != "Null") && (nodeSaveData.f_t == NewNode.FloorTileType.floorAndCeiling || nodeSaveData.f_t == NewNode.FloorTileType.floorOnly || (nodeSaveData.f_t == NewNode.FloorTileType.noneButIndoors && num == 0)))
												{
													GameObject smallFloorTile = PrefabControls.Instance.smallFloorTile;
													GameObject gameObject = Toolbox.Instance.SpawnObject(smallFloorTile, this.cityEditorGroundFloorRepresentation.transform);
													gameObject.transform.localPosition = new Vector3((float)nodeSaveData.f_c.x * PathFinder.Instance.nodeSize.x, (float)num * 5.2f + 0.2f + (float)nodeSaveData.f_h * 0.1f, (float)nodeSaveData.f_c.y * PathFinder.Instance.nodeSize.y) - new Vector3(CityControls.Instance.cityTileSize.x * 0.5f, 0f, CityControls.Instance.cityTileSize.y * 0.5f) + new Vector3(PathFinder.Instance.nodeSize.x * 0.5f, 0f, PathFinder.Instance.nodeSize.y * 0.5f);
													gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
													MeshFilter component = gameObject.GetComponent<MeshFilter>();
													if (component != null)
													{
														list.Add(component);
													}
												}
												if (num == 0)
												{
													foreach (WallSaveData wallSaveData in nodeSaveData.w_d)
													{
														GameObject newObj = PrefabControls.Instance.shortWallTile;
														DoorPairPreset doorPairPreset = null;
														if (Toolbox.Instance.LoadDataFromResources<DoorPairPreset>(wallSaveData.p_n, out doorPairPreset) && doorPairPreset.parentWallsShort.Count > 0)
														{
															newObj = doorPairPreset.parentWallsShort[0];
															GameObject gameObject2 = Toolbox.Instance.SpawnObject(newObj, this.cityEditorGroundFloorRepresentation.transform);
															gameObject2.transform.localPosition = new Vector3((float)nodeSaveData.f_c.x * PathFinder.Instance.nodeSize.x, (float)nodeSaveData.f_h * 0.1f, (float)nodeSaveData.f_c.y * PathFinder.Instance.nodeSize.y) - new Vector3(CityControls.Instance.cityTileSize.x * 0.5f, 0f, CityControls.Instance.cityTileSize.y * 0.5f) + new Vector3(PathFinder.Instance.nodeSize.x * 0.5f, 0f, PathFinder.Instance.nodeSize.y * 0.5f);
															if (wallSaveData.w_o.x < 0f)
															{
																gameObject2.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
															}
															else if (wallSaveData.w_o.x > 0f)
															{
																gameObject2.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
															}
															else if (wallSaveData.w_o.y < 0f)
															{
																gameObject2.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
															}
															else if (wallSaveData.w_o.y > 0f)
															{
																gameObject2.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
															}
															gameObject2.transform.localPosition += new Vector3(wallSaveData.w_o.x * (PathFinder.Instance.tileSize.x / (float)CityControls.Instance.nodeMultiplier - 0.2f), (float)nodeSaveData.f_h * -0.1f, wallSaveData.w_o.y * (PathFinder.Instance.tileSize.y / (float)CityControls.Instance.nodeMultiplier - 0.2f));
															MeshFilter component2 = gameObject2.GetComponent<MeshFilter>();
															if (component2 != null)
															{
																list2.Add(component2);
															}
															if (roomConfiguration != null)
															{
																foreach (NewWall.FrontageSetting frontageSetting in this.SelectFrontage(roomConfiguration, doorPairPreset))
																{
																	GameObject gameObject3 = Toolbox.Instance.SpawnObject(frontageSetting.preset.gameObject, this.cityEditorGroundFloorRepresentation.transform);
																	gameObject3.transform.position = gameObject2.transform.position + frontageSetting.offset;
																	gameObject3.transform.localEulerAngles = -gameObject2.transform.localEulerAngles;
																	gameObject3.transform.position += gameObject3.transform.forward * -0.4f;
																	frontageSetting.mainTransform = gameObject3.transform;
																	Toolbox.MaterialKey matKey = frontageSetting.matKey;
																	foreach (MeshRenderer meshRenderer in gameObject3.GetComponentsInChildren<MeshRenderer>(true))
																	{
																		MaterialsController.Instance.ApplyMaterialKey(meshRenderer, matKey);
																		meshRenderer.staticShadowCaster = true;
																	}
																}
															}
														}
													}
												}
											}
											GameObject gameObject4 = this.CombineGroundFloorMeshes(ref list, "Floor Mesh");
											if (gameObject4 != null)
											{
												MeshRenderer component3 = gameObject4.GetComponent<MeshRenderer>();
												if (component3 != null)
												{
													component3.sharedMaterial = CityControls.Instance.defaultFloorMaterialGroup.material;
												}
											}
											if (num == 0)
											{
												GameObject gameObject5 = this.CombineGroundFloorMeshes(ref list2, "Wall Mesh");
												if (gameObject5 != null)
												{
													MeshRenderer component4 = gameObject5.GetComponent<MeshRenderer>();
													if (component4 != null)
													{
														component4.sharedMaterial = CityControls.Instance.defaultWallMaterialGroup.material;
														if (this.preset.defaultExteriorWallMaterial.Count > 0 && this.preset.defaultExteriorWallMaterial[0] != null)
														{
															component4.sharedMaterial = this.preset.defaultExteriorWallMaterial[0].material;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
					num++;
				}
			}
		}
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x0010CFF8 File Offset: 0x0010B1F8
	private GameObject CombineGroundFloorMeshes(ref List<MeshFilter> childMeshes, string objectName)
	{
		GameObject gameObject = null;
		if (childMeshes.Count > 0)
		{
			Mesh sharedMesh;
			if (Game.Instance.useJobSystemForMeshCombination)
			{
				sharedMesh = MeshPoolingController.Instance.CombineMeshesWithMeshAPI(ref childMeshes);
			}
			else
			{
				sharedMesh = MeshPoolingController.Instance.CombineMeshes(ref childMeshes);
			}
			gameObject = new GameObject(objectName);
			gameObject.transform.SetParent(this.cityEditorGroundFloorRepresentation.transform, true);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			meshFilter.sharedMesh = sharedMesh;
			while (childMeshes.Count > 0)
			{
				childMeshes[0].gameObject.SetActive(false);
				Object.Destroy(childMeshes[0].gameObject);
				childMeshes.RemoveAt(0);
			}
		}
		return gameObject;
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x0010D0AC File Offset: 0x0010B2AC
	private List<NewWall.FrontageSetting> SelectFrontage(RoomConfiguration roomConfig, DoorPairPreset wallPreset)
	{
		List<NewWall.FrontageSetting> list = new List<NewWall.FrontageSetting>();
		if (roomConfig.wallFrontage.Count > 0)
		{
			List<RoomConfiguration.WallFrontage> list2 = new List<RoomConfiguration.WallFrontage>();
			foreach (RoomConfiguration.WallFrontage wallFrontage in roomConfig.wallFrontage)
			{
				if ((!wallFrontage.limitToBuildingTypes || wallFrontage.limitedToBuildings.Contains(this.preset)) && wallFrontage.wallPreset == wallPreset && wallFrontage.onlyIfBorderingOutside)
				{
					list2.Add(wallFrontage);
				}
			}
			if (list2.Count <= 0)
			{
				return list;
			}
			foreach (RoomConfiguration.WallFrontage wallFrontage2 in list2)
			{
				foreach (WallFrontageClass frontageClass in wallFrontage2.outsideFrontage)
				{
					WallFrontagePreset wallFrontagePreset = Toolbox.Instance.SelectWallFrontage(CityControls.Instance.fallbackStyle, frontageClass, Toolbox.Instance.GenerateUniqueID());
					Toolbox.MaterialKey matKey = new Toolbox.MaterialKey();
					bool colors = false;
					list.Add(new NewWall.FrontageSetting
					{
						preset = wallFrontagePreset,
						matKey = matKey,
						offset = wallFrontage2.localOffset,
						colors = colors
					});
				}
			}
		}
		return list;
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x0010D230 File Offset: 0x0010B430
	public void RemoveGroundFloorBuildingModel()
	{
		if (this.cityEditorGroundFloorRepresentation != null)
		{
			Object.Destroy(this.cityEditorGroundFloorRepresentation);
		}
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x0010D24C File Offset: 0x0010B44C
	private void SetupEmissionTexture()
	{
		this.emissionTextureUnlit = this.preset.emissionMapUnlit;
		if (this.emissionTextureUnlit != null)
		{
			Transform[] allTransforms = Toolbox.Instance.GetAllTransforms(this.buildingModelBase.transform);
			foreach (GameObject gameObject in this.buildingModelsLights)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					if (this.emissionTextureInstanced == null)
					{
						this.emissionTextureInstanced = Object.Instantiate<Texture2D>(this.emissionTextureUnlit);
					}
					component.material.SetTexture("_EmissiveColorMap", this.emissionTextureInstanced);
					Material weatherAffectedMaterial = SessionData.Instance.GetWeatherAffectedMaterial(component.sharedMaterial, component);
					if (weatherAffectedMaterial != component.sharedMaterial)
					{
						component.sharedMaterial = weatherAffectedMaterial;
					}
					foreach (Transform transform in allTransforms)
					{
						if (transform.name.Substring(3) == "_static_" + gameObject.name)
						{
							MeshRenderer component2 = transform.gameObject.GetComponent<MeshRenderer>();
							if (component2 != null)
							{
								component2.sharedMaterial = component.sharedMaterial;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x0010D3B0 File Offset: 0x0010B5B0
	public void SetTargetMode(NewBuilding.AlarmTargetMode newMode, bool setResetTimer = true)
	{
		this.targetMode = newMode;
		Game.Log("Set target mode for " + base.name + ": " + this.targetMode.ToString(), 2);
		if (setResetTimer)
		{
			this.targetModeSetAt = SessionData.Instance.gameTime;
			if (this.targetMode != NewBuilding.AlarmTargetMode.illegalActivities && !GameplayController.Instance.alteredSecurityTargetsBuildings.Contains(this))
			{
				GameplayController.Instance.alteredSecurityTargetsBuildings.Add(this);
			}
		}
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x0010D42D File Offset: 0x0010B62D
	private void SetupEnvironment()
	{
		if (this.environmentalSettingsObject != null)
		{
			Object.Destroy(this.environmentalSettingsObject.gameObject);
		}
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x0010D450 File Offset: 0x0010B650
	public void UpdateColourSchemeAndMaterials()
	{
		this.wood = InteriorControls.Instance.woods[Toolbox.Instance.GetPsuedoRandomNumberContained(0, InteriorControls.Instance.woods.Count, this.seed, out this.seed)];
		List<DesignStylePreset> list = new List<DesignStylePreset>();
		foreach (DesignStylePreset designStylePreset in Toolbox.Instance.allDesignStyles)
		{
			if (this.preset.forceBuildingDesignStyles.Count > 0)
			{
				if (!this.preset.forceBuildingDesignStyles.Contains(designStylePreset))
				{
					continue;
				}
			}
			else if (!designStylePreset.includeInPersonalityMatching || designStylePreset.isBasement)
			{
				continue;
			}
			float num = (float)(10 - Mathf.RoundToInt(Mathf.Abs((float)designStylePreset.humility - (10f - (float)this.cityTile.landValue * 2.5f))));
			int num2 = 10 - Mathf.RoundToInt((float)Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, this.seed, out this.seed));
			int num3 = 10 - Mathf.RoundToInt((float)Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, this.seed, out this.seed));
			int num4 = 10 - Mathf.RoundToInt((float)Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, this.seed, out this.seed));
			int num5 = 10 - Mathf.RoundToInt((float)Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, this.seed, out this.seed));
			int num6 = Mathf.RoundToInt(Mathf.Abs((float)designStylePreset.creativity - (10f - (float)designStylePreset.modernity)));
			int num7 = Mathf.FloorToInt((num + (float)num2 + (float)num3 + (float)num4 + (float)num5 + (float)num6) / 6f);
			for (int i = 0; i < num7; i++)
			{
				list.Add(designStylePreset);
			}
		}
		if (list.Count <= 0)
		{
			list.Add(CityControls.Instance.fallbackStyle);
			Game.Log("CityGen: No compatible design styles found for " + base.name + ", using backup style...", 2);
		}
		this.designStyle = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.seed, out this.seed)];
		List<ColourSchemePreset> list2 = new List<ColourSchemePreset>();
		foreach (ColourSchemePreset colourSchemePreset in Toolbox.Instance.allColourSchemes)
		{
			float modernity = (float)this.preset.modernity;
			int num8 = Mathf.RoundToInt(Mathf.Clamp((float)this.cityTile.landValue * 2.5f, 0f, 10f));
			string input = this.buildingID.ToString();
			int num9 = Mathf.RoundToInt(Toolbox.Instance.RandomRangeWeightedSeedContained(0f, 10f, 5f, input, out input, 4));
			int num10 = Mathf.RoundToInt(Toolbox.Instance.RandomRangeWeightedSeedContained(0f, 10f, 5f, input, out input, 4));
			int num11 = Mathf.FloorToInt((modernity + (float)num8 + (float)num9 + (float)num10) / 8f);
			for (int j = 0; j < num11; j++)
			{
				list2.Add(colourSchemePreset);
			}
		}
		if (list2.Count <= 0)
		{
			list2.Add(CityControls.Instance.fallbackColourScheme);
		}
		this.colourScheme = list2[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list2.Count, this.seed, out this.seed)];
		float normalizedLandValue = Toolbox.Instance.GetNormalizedLandValue(this);
		string seedInput = this.buildingID.ToString();
		this.defaultWallMaterial = Toolbox.Instance.SelectMaterial(this.preset.lobbyPreset.roomConfig[0].roomClass, normalizedLandValue, this.designStyle, MaterialGroupPreset.MaterialType.walls, seedInput, out seedInput);
		MaterialGroupPreset.MaterialVariation variation = this.defaultWallMaterial.variations[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.defaultWallMaterial.variations.Count, this.seed, out this.seed)];
		this.defaultWallKey = MaterialsController.Instance.GenerateMaterialKey(variation, this.colourScheme, null, true, this);
		this.ceilingMaterial = Toolbox.Instance.SelectMaterial(this.preset.lobbyPreset.roomConfig[0].roomClass, normalizedLandValue, this.designStyle, MaterialGroupPreset.MaterialType.ceiling, seedInput, out seedInput);
		MaterialGroupPreset.MaterialVariation variation2 = this.ceilingMaterial.variations[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.ceilingMaterial.variations.Count, this.seed, out this.seed)];
		this.ceilingMatKey = MaterialsController.Instance.GenerateMaterialKey(variation2, this.colourScheme, null, true, this);
		this.floorMaterial = Toolbox.Instance.SelectMaterial(this.preset.lobbyPreset.roomConfig[0].roomClass, normalizedLandValue, this.designStyle, MaterialGroupPreset.MaterialType.floor, seedInput, out seedInput);
		MaterialGroupPreset.MaterialVariation variation3 = this.floorMaterial.variations[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.floorMaterial.variations.Count, this.seed, out this.seed)];
		this.floorMatKey = MaterialsController.Instance.GenerateMaterialKey(variation3, this.colourScheme, null, true, this);
		if (Game.Instance.devMode)
		{
			this.debugDecor.Add("New Decor: Chosen building decor style...");
		}
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x0010D9D8 File Offset: 0x0010BBD8
	public void LoadInterior()
	{
		if (this.isInaccessible)
		{
			return;
		}
		if (this.preset.floorLayouts.Count <= 0 && !this.preset.nonEnterable)
		{
			Game.Log("CityGen: Building " + base.name + " has no floor data.", 2);
		}
		if (!CityConstructor.Instance.generateNew)
		{
			CitySaveData.CityTileCitySave cityTileCitySave = CityConstructor.Instance.currentData.cityTiles.Find((CitySaveData.CityTileCitySave item) => item.cityCoord == this.cityTile.cityCoord);
			foreach (CitySaveData.FloorCitySave data in cityTileCitySave.building.floors)
			{
				Object.Instantiate<GameObject>(PrefabControls.Instance.floor, base.transform).GetComponent<NewFloor>().Load(data, this);
			}
			foreach (CitySaveData.FloorCitySave floorCitySave in cityTileCitySave.building.floors)
			{
				foreach (CitySaveData.AddressCitySave addressCitySave in floorCitySave.addresses)
				{
					foreach (CitySaveData.RoomCitySave roomCitySave in addressCitySave.rooms)
					{
						if (roomCitySave.commonRooms.Count > 0)
						{
							if (!CityData.Instance.roomDictionary.ContainsKey(roomCitySave.id))
							{
								Game.LogError("CityGen: Cannot find room " + roomCitySave.id.ToString() + ": This should have been loaded already and put into the dictionary...", 2);
							}
							else
							{
								NewRoom newRoom = CityData.Instance.roomDictionary[roomCitySave.id];
								foreach (int num in roomCitySave.commonRooms)
								{
									NewRoom newRoom2 = CityData.Instance.roomDictionary[num];
									newRoom.commonRooms.Add(newRoom2);
								}
							}
						}
						foreach (CitySaveData.NodeCitySave nodeCitySave in roomCitySave.nodes)
						{
							using (List<CitySaveData.WallCitySave>.Enumerator enumerator6 = nodeCitySave.w.GetEnumerator())
							{
								while (enumerator6.MoveNext())
								{
									CitySaveData.WallCitySave wallData = enumerator6.Current;
									if (wallData.cl > -1)
									{
										NewWall newWall = PathFinder.Instance.nodeMap[nodeCitySave.nc].walls.Find((NewWall item) => item.id == wallData.id);
										NewRoom newRoom3 = CityData.Instance.roomDictionary[wallData.cl];
										if (newRoom3 != null && newWall != null)
										{
											newWall.SetAsLightswitch(newRoom3);
										}
									}
								}
							}
						}
					}
				}
			}
			foreach (CitySaveData.AirDuctGroupCitySave load in cityTileCitySave.building.airDucts)
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.SetParent(base.transform, false);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.AddComponent<AirDuctGroup>().Load(load, this);
			}
			using (List<CitySaveData.AirDuctGroupCitySave>.Enumerator enumerator7 = cityTileCitySave.building.airDucts.GetEnumerator())
			{
				while (enumerator7.MoveNext())
				{
					CitySaveData.AirDuctGroupCitySave airDuct = enumerator7.Current;
					AirDuctGroup airDuctGroup = this.airDucts.Find((AirDuctGroup item) => item.ductID == airDuct.id);
					using (List<int>.Enumerator enumerator4 = airDuct.adjoining.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							int adj = enumerator4.Current;
							AirDuctGroup airDuctGroup2 = this.airDucts.Find((AirDuctGroup item) => item.ductID == adj);
							airDuctGroup.AddAdjoiningDuctGroup(airDuctGroup2);
							airDuctGroup2.AddAdjoiningDuctGroup(airDuctGroup);
						}
					}
				}
				return;
			}
		}
		List<NewTile> list = new List<NewTile>();
		int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained((int)this.preset.controlRoomRange.x, (int)this.preset.controlRoomRange.y, this.seed, out this.seed);
		List<int> list2 = new List<int>();
		int num2 = 0;
		if (psuedoRandomNumberContained > 0)
		{
			for (int i = 0; i < this.preset.floorLayouts.Count; i++)
			{
				BuildingPreset.InteriorFloorSetting interiorFloorSetting = this.preset.floorLayouts[i];
				for (int j = 0; j < interiorFloorSetting.floorsWithThisSetting; j++)
				{
					if (interiorFloorSetting.controlRoomVariants.Count > 0)
					{
						Game.Log("CityGen: Added control room possibility on floor " + num2.ToString() + " " + this.preset.name, 2);
						list2.Add(num2);
					}
					num2++;
				}
			}
			num2 = -1;
			for (int k = 0; k < this.preset.basementLayouts.Count; k++)
			{
				BuildingPreset.InteriorFloorSetting interiorFloorSetting2 = this.preset.basementLayouts[k];
				for (int l = 0; l < interiorFloorSetting2.floorsWithThisSetting; l++)
				{
					if (interiorFloorSetting2.controlRoomVariants.Count > 0)
					{
						Game.Log("Added control room possibility on floor " + num2.ToString() + " " + this.preset.name, 2);
						list2.Add(num2);
					}
					num2--;
				}
			}
		}
		List<int> list3 = new List<int>();
		for (int m = 0; m < psuedoRandomNumberContained; m++)
		{
			int psuedoRandomNumberContained2 = Toolbox.Instance.GetPsuedoRandomNumberContained(0, list2.Count, this.seed, out this.seed);
			list3.Add(list2[psuedoRandomNumberContained2]);
			Game.Log("CityGen: Chose control room floor " + list2[psuedoRandomNumberContained2].ToString(), 2);
			list2.RemoveAt(psuedoRandomNumberContained2);
			if (list2.Count <= 0)
			{
				break;
			}
		}
		num2 = 0;
		for (int n = 0; n < this.preset.floorLayouts.Count; n++)
		{
			BuildingPreset.InteriorFloorSetting interiorFloorSetting3 = this.preset.floorLayouts[n];
			for (int num3 = 0; num3 < interiorFloorSetting3.floorsWithThisSetting; num3++)
			{
				TextAsset textAsset;
				if (list3.Contains(num2))
				{
					Game.Log("CityGen: Picking control room for floor " + num2.ToString(), 2);
					textAsset = interiorFloorSetting3.controlRoomVariants[Toolbox.Instance.GetPsuedoRandomNumberContained(0, interiorFloorSetting3.controlRoomVariants.Count, this.seed, out this.seed)];
				}
				else
				{
					if (interiorFloorSetting3.blueprints.Count <= 0)
					{
						Game.LogError("No blueprints found on " + this.preset.name, 2);
					}
					textAsset = interiorFloorSetting3.blueprints[Toolbox.Instance.GetPsuedoRandomNumberContained(0, interiorFloorSetting3.blueprints.Count, this.seed, out this.seed)];
				}
				if (textAsset == null)
				{
					Game.LogError("CityGen: Missing floor preset for " + base.name + " floor " + num3.ToString(), 2);
					return;
				}
				FloorSaveData floorSaveData = null;
				CityData.Instance.floorData.TryGetValue(textAsset.name, ref floorSaveData);
				if (floorSaveData == null)
				{
					Game.LogError("CityGen: No floor data for " + textAsset.name, 2);
					return;
				}
				NewFloor component = Object.Instantiate<GameObject>(PrefabControls.Instance.floor, base.transform).GetComponent<NewFloor>();
				int newCeilingHeight = floorSaveData.defaultCeilingHeight;
				if (interiorFloorSetting3.overrideCeilingHeight)
				{
					newCeilingHeight = interiorFloorSetting3.newCeilingHeight;
				}
				component.Setup(num2, this, floorSaveData.floorName, floorSaveData.size, floorSaveData.defaultFloorHeight, newCeilingHeight);
				num2++;
				component.maxDuctExtrusion = interiorFloorSetting3.airVentMaximumExtrusion;
				component.LoadDataToFloor(floorSaveData);
				component.layoutIndex = n;
				foreach (KeyValuePair<Vector2Int, NewTile> keyValuePair in component.tileMap)
				{
					if (keyValuePair.Value.isStairwell || keyValuePair.Value.isInvertedStairwell)
					{
						list.Add(keyValuePair.Value);
					}
				}
			}
		}
		num2 = -1;
		for (int num4 = 0; num4 < this.preset.basementLayouts.Count; num4++)
		{
			BuildingPreset.InteriorFloorSetting interiorFloorSetting4 = this.preset.basementLayouts[num4];
			for (int num5 = 0; num5 < interiorFloorSetting4.floorsWithThisSetting; num5++)
			{
				TextAsset textAsset2;
				if (list3.Contains(num2))
				{
					Game.Log("Picking control room for floor " + num2.ToString(), 2);
					textAsset2 = interiorFloorSetting4.controlRoomVariants[Toolbox.Instance.GetPsuedoRandomNumberContained(0, interiorFloorSetting4.controlRoomVariants.Count, this.seed, out this.seed)];
				}
				else
				{
					if (interiorFloorSetting4.blueprints.Count <= 0)
					{
						Game.LogError("No blueprints found on " + this.preset.name, 2);
					}
					textAsset2 = interiorFloorSetting4.blueprints[Toolbox.Instance.GetPsuedoRandomNumberContained(0, interiorFloorSetting4.blueprints.Count, this.seed, out this.seed)];
				}
				if (textAsset2 == null)
				{
					Game.LogError("CityGen: Missing floor preset for " + base.name + " floor " + num5.ToString(), 2);
					return;
				}
				FloorSaveData floorSaveData2 = null;
				CityData.Instance.floorData.TryGetValue(textAsset2.name, ref floorSaveData2);
				if (floorSaveData2 == null)
				{
					Game.LogError("CityGen: No floor data for " + textAsset2.name, 2);
					return;
				}
				NewFloor component2 = Object.Instantiate<GameObject>(PrefabControls.Instance.floor, base.transform).GetComponent<NewFloor>();
				int newCeilingHeight2 = floorSaveData2.defaultCeilingHeight;
				if (interiorFloorSetting4.overrideCeilingHeight)
				{
					newCeilingHeight2 = interiorFloorSetting4.newCeilingHeight;
				}
				component2.Setup(num2, this, floorSaveData2.floorName, floorSaveData2.size, floorSaveData2.defaultFloorHeight, newCeilingHeight2);
				num2--;
				component2.maxDuctExtrusion = interiorFloorSetting4.airVentMaximumExtrusion;
				component2.LoadDataToFloor(floorSaveData2);
				component2.layoutIndex = num4;
				foreach (KeyValuePair<Vector2Int, NewTile> keyValuePair2 in component2.tileMap)
				{
					if (keyValuePair2.Value.isStairwell || keyValuePair2.Value.isInvertedStairwell)
					{
						list.Add(keyValuePair2.Value);
					}
				}
			}
		}
		using (List<NewTile>.Enumerator enumerator9 = list.GetEnumerator())
		{
			while (enumerator9.MoveNext())
			{
				NewTile stair = enumerator9.Current;
				if (list.Find((NewTile item) => item.floorCoord.x == stair.floorCoord.x && item.floorCoord.y == stair.floorCoord.y && item.floor.floor == stair.floor.floor + 1) == null)
				{
					stair.SetAsTop(true);
				}
				if (list.Find((NewTile item) => item.floorCoord.x == stair.floorCoord.x && item.floorCoord.y == stair.floorCoord.y && item.floor.floor == stair.floor.floor - 1) == null)
				{
					stair.SetAsBottom(true);
				}
			}
		}
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x0010E670 File Offset: 0x0010C870
	public void AddBuildingEntrance(NewWall wallTile, bool isMain = false)
	{
		if (wallTile == null)
		{
			return;
		}
		if (isMain)
		{
			if (wallTile.node.gameLocation.thisAsStreet == null)
			{
				StreetController thisAsStreet = wallTile.otherWall.node.gameLocation.thisAsStreet;
			}
			this.mainEntrance = wallTile;
			if (this.mainEntrance != null)
			{
				this.street = this.mainEntrance.otherWall.node.gameLocation.thisAsStreet;
			}
		}
		else if (!this.additionalEntrances.Contains(wallTile))
		{
			this.additionalEntrances.Add(wallTile);
		}
		if (wallTile.node.floor != null)
		{
			if (!wallTile.node.floor.buildingEntrances.Contains(wallTile))
			{
				wallTile.node.floor.buildingEntrances.Add(wallTile);
				return;
			}
		}
		else
		{
			string[] array = new string[8];
			array[0] = "CityGen: ";
			array[1] = wallTile.id.ToString();
			array[2] = " does not feature reference to a floor. This shouldn't be happening: ";
			int num = 3;
			Vector3 position = wallTile.position;
			array[num] = position.ToString();
			array[4] = " room: ";
			array[5] = wallTile.node.room.name;
			array[6] = " other: ";
			array[7] = wallTile.otherWall.node.room.name;
			Game.Log(string.Concat(array), 2);
		}
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x0010E7C4 File Offset: 0x0010C9C4
	public Vector2Int FaceLocalTileVector(Vector2Int r)
	{
		for (int i = 0; i < this.rotations; i++)
		{
			Vector2 vector;
			vector..ctor((float)r.x - (float)(CityControls.Instance.tileMultiplier - 1) * 0.5f, (float)r.y - (float)(CityControls.Instance.tileMultiplier - 1) * 0.5f);
			vector = Toolbox.Instance.RotateVector2ACW(vector, 90f);
			r..ctor(Mathf.RoundToInt(vector.x + (float)(CityControls.Instance.tileMultiplier - 1) * 0.5f), Mathf.RoundToInt(vector.y + (float)(CityControls.Instance.tileMultiplier - 1) * 0.5f));
		}
		return r;
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x0010E880 File Offset: 0x0010CA80
	public Vector2Int FaceLocalNodeVector(Vector2Int r)
	{
		for (int i = 0; i < this.rotations; i++)
		{
			Vector2 vector;
			vector..ctor((float)r.x - (float)(CityControls.Instance.tileMultiplier * CityControls.Instance.nodeMultiplier - 1) * 0.5f, (float)r.y - (float)(CityControls.Instance.tileMultiplier * CityControls.Instance.nodeMultiplier - 1) * 0.5f);
			vector = Toolbox.Instance.RotateVector2ACW(vector, 90f);
			r..ctor(Mathf.RoundToInt(vector.x + (float)(CityControls.Instance.tileMultiplier * CityControls.Instance.nodeMultiplier - 1) * 0.5f), Mathf.RoundToInt(vector.y + (float)(CityControls.Instance.tileMultiplier * CityControls.Instance.nodeMultiplier - 1) * 0.5f));
		}
		return r;
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x0010E968 File Offset: 0x0010CB68
	public Vector2 FaceWallOffsetVector(Vector2 r)
	{
		for (int i = 0; i < this.rotations; i++)
		{
			r = Toolbox.Instance.RotateVector2ACW(r, 90f);
		}
		return r;
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x0010E99C File Offset: 0x0010CB9C
	public Vector2 GetOriginalWallOffset(Vector2 r)
	{
		for (int i = 0; i < this.rotations; i++)
		{
			r = Toolbox.Instance.RotateVector2ACW(r, -90f);
		}
		return r;
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x0010E9D0 File Offset: 0x0010CBD0
	public Vector3 LocalToGlobalPathmap(Vector3 r)
	{
		Vector2 vector;
		vector..ctor((float)this.globalTileCoords.x - (float)(CityControls.Instance.tileMultiplier - 1) * 0.5f, (float)this.globalTileCoords.y - (float)(CityControls.Instance.tileMultiplier - 1) * 0.5f);
		return new Vector3((float)Mathf.RoundToInt(vector.x + r.x), (float)Mathf.RoundToInt(vector.y + r.y), r.z);
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x0010EA58 File Offset: 0x0010CC58
	public void CalculateFacing()
	{
		new Vector2((float)Mathf.FloorToInt(PathFinder.Instance.tileCitySize.x * 0.5f), (float)Mathf.FloorToInt(PathFinder.Instance.tileCitySize.y * 0.5f));
		float num = -1f;
		Vector2Int vector2Int;
		vector2Int..ctor(this.globalTileCoords.x, this.globalTileCoords.y + 1);
		foreach (Vector2Int vector2Int2 in CityData.Instance.offsetArrayX4)
		{
			Vector3Int vector3Int;
			vector3Int..ctor(Mathf.RoundToInt((float)this.globalTileCoords.x + (float)vector2Int2.x * Mathf.Floor((float)CityControls.Instance.tileMultiplier * 0.5f)), Mathf.RoundToInt((float)this.globalTileCoords.y + (float)vector2Int2.y * Mathf.Floor((float)CityControls.Instance.tileMultiplier * 0.5f)), 0);
			if (PathFinder.Instance.tileMap.ContainsKey(vector3Int))
			{
				NewTile newTile = PathFinder.Instance.tileMap[vector3Int];
				if (newTile.streetController != null && newTile.streetController.normalizedFootfall > num)
				{
					vector2Int..ctor(vector3Int.x, vector3Int.y);
					num = newTile.streetController.normalizedFootfall;
				}
			}
		}
		NewBuilding.Direction face = NewBuilding.Direction.South;
		if (this.preset.boundaryCorner)
		{
			if (this.cityTile.cityCoord.x == 0)
			{
				if (this.cityTile.cityCoord.y == 0)
				{
					face = NewBuilding.Direction.West;
				}
				else
				{
					face = NewBuilding.Direction.North;
				}
			}
			else if (this.cityTile.cityCoord.y == 0)
			{
				face = NewBuilding.Direction.South;
			}
			else
			{
				face = NewBuilding.Direction.East;
			}
		}
		else
		{
			if (vector2Int.x < this.globalTileCoords.x)
			{
				face = NewBuilding.Direction.East;
			}
			if (vector2Int.x > this.globalTileCoords.x)
			{
				face = NewBuilding.Direction.West;
			}
			if (vector2Int.y < this.globalTileCoords.y)
			{
				face = NewBuilding.Direction.North;
			}
		}
		this.SetFacing(face, true);
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x0010EC6C File Offset: 0x0010CE6C
	private void CalculateRotations()
	{
		if (this.facing == NewBuilding.Direction.North)
		{
			this.rotations = 0;
			return;
		}
		if (this.facing == NewBuilding.Direction.West)
		{
			this.rotations = 1;
			return;
		}
		if (this.facing == NewBuilding.Direction.South)
		{
			this.rotations = 2;
			return;
		}
		if (this.facing == NewBuilding.Direction.East)
		{
			this.rotations = 3;
		}
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x0010ECBB File Offset: 0x0010CEBB
	public void SetFacing(NewBuilding.Direction face, bool updateBuildingModel = true)
	{
		this.facing = face;
		this.CalculateRotations();
		if (updateBuildingModel && this.buildingModelBase != null)
		{
			this.buildingModelBase.transform.eulerAngles = this.GetBuildingEuler();
		}
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x0010ECF4 File Offset: 0x0010CEF4
	public Vector3 GetBuildingEuler()
	{
		Vector3 result;
		result..ctor(0f, 0f, 0f);
		if (this.facing == NewBuilding.Direction.North)
		{
			result.y = 180f;
		}
		else if (this.facing == NewBuilding.Direction.West)
		{
			result.y = 90f;
		}
		else if (this.facing == NewBuilding.Direction.South)
		{
			result.y = 0f;
		}
		else if (this.facing == NewBuilding.Direction.East)
		{
			result.y = 270f;
		}
		return result;
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x0010ED71 File Offset: 0x0010CF71
	public void SetInaccessible()
	{
		this.isInaccessible = true;
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x0009014F File Offset: 0x0008E34F
	public override void SetupEvidence()
	{
		base.SetupEvidence();
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x0010ED7A File Offset: 0x0010CF7A
	public void AddLobby(NewAddress newLob)
	{
		if (!this.lobbies.Contains(newLob))
		{
			this.lobbies.Add(newLob);
		}
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x0010ED98 File Offset: 0x0010CF98
	public void SetDisplayBuildingModel(bool vis, bool coll, List<string> hideModelChildOverride = null)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		this.ResetSelectivelyHidden();
		if (this.displayBuildingModel != vis)
		{
			if (this.buildingModelBase != null)
			{
				this.buildingModelBase.SetActive(vis);
			}
			this.displayBuildingModel = vis;
		}
		foreach (Collider collider in this.colliders)
		{
			if (collider != null)
			{
				collider.enabled = coll;
			}
		}
		if (vis)
		{
			this.SelectivelyHideModels(hideModelChildOverride);
		}
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x0010EE3C File Offset: 0x0010D03C
	public void SelectivelyHideModels(List<string> hideModelChildOverride)
	{
		if (hideModelChildOverride != null)
		{
			using (List<string>.Enumerator enumerator = hideModelChildOverride.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string i = enumerator.Current;
					GameObject gameObject = this.buildingModelsActual.Find((GameObject item) => item.name == i);
					if (gameObject != null)
					{
						gameObject.SetActive(false);
						if (!this.selectivelyHidden.Contains(gameObject))
						{
							this.selectivelyHidden.Add(gameObject);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x0010EED4 File Offset: 0x0010D0D4
	public void ResetSelectivelyHidden()
	{
		foreach (GameObject gameObject in this.selectivelyHidden)
		{
			gameObject.SetActive(true);
		}
		this.selectivelyHidden.Clear();
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x0010EF30 File Offset: 0x0010D130
	public void SpawnStreetCables()
	{
		int num = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			27
		});
		using (List<BuildingPreset.CableLinkPoint>.Enumerator enumerator = this.preset.cableLinkPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BuildingPreset.CableLinkPoint link = enumerator.Current;
				float num2 = Mathf.Clamp01(link.localPos.y / this.preset.meshHeight);
				float num3 = this.preset.cableSpawnChanceOverHeight.Evaluate(num2);
				Toolbox instance = Toolbox.Instance;
				float lowerRange = 0f;
				float upperRange = 1f;
				string text = this.buildingID.ToString();
				Vector3 vector = link.localPos;
				if (instance.GetPsuedoRandomNumber(lowerRange, upperRange, text + vector.ToString(), false) <= num3)
				{
					float num4 = 0f;
					float num5 = 0f;
					Toolbox instance2 = Toolbox.Instance;
					float lowerRange2 = -CityControls.Instance.maximumCableAngle;
					float maximumCableAngle = CityControls.Instance.maximumCableAngle;
					string text2 = this.buildingID.ToString();
					vector = link.localPos;
					Vector3 vector2;
					vector2..ctor(num4, num5, instance2.GetPsuedoRandomNumber(lowerRange2, maximumCableAngle, text2 + vector.ToString(), false));
					Vector3 vector3 = this.buildingModelBase.transform.TransformPoint(link.localPos);
					Vector3 normalized = this.buildingModelBase.transform.TransformDirection(Quaternion.Euler(link.localRot).eulerAngles + vector2).normalized;
					RaycastHit hit;
					if (Physics.Raycast(new Ray(vector3, normalized), ref hit, 35f, num))
					{
						float num6 = Vector3.Dot(normalized, hit.normal);
						if (num6 <= -0.85f)
						{
							NewBuilding componentInParent = hit.transform.gameObject.GetComponentInParent<NewBuilding>(true);
							if (!(componentInParent != null) || !(componentInParent == this))
							{
								List<CityControls.StreetCable> list = CityControls.Instance.cables.FindAll((CityControls.StreetCable item) => hit.distance <= item.maximumWidth && link.localPos.y <= item.maximumHeight && link.localPos.y >= item.minimumHeight && (!item.onlyFromZoneType || item.zone == this.preset.displayedZone));
								if (list.Count > 0)
								{
									List<CityControls.StreetCable> list2 = new List<CityControls.StreetCable>();
									foreach (CityControls.StreetCable streetCable in list)
									{
										int num7 = streetCable.frequency;
										if (streetCable.disitrctFrequencyModifier && streetCable.districts.Contains(this.cityTile.district.preset))
										{
											num7 += streetCable.frequencyModifier;
										}
										for (int i = 0; i < num7; i++)
										{
											list2.Add(streetCable);
										}
									}
									CityControls.StreetCable streetCable2 = list2[Toolbox.Instance.GetPsuedoRandomNumber(0, list2.Count, hit.point.ToString(), false)];
									GameObject gameObject = Object.Instantiate<GameObject>(streetCable2.prefab, this.buildingModelBase.transform);
									gameObject.transform.position = Vector3.Lerp(vector3, hit.point, 0.5f);
									this.spawnedCables.Add(gameObject);
									MeshFilter component = gameObject.transform.GetChild(0).GetComponent<MeshFilter>();
									foreach (MeshRenderer meshRenderer in gameObject.gameObject.GetComponentsInChildren<MeshRenderer>())
									{
										if (meshRenderer != null)
										{
											meshRenderer.sharedMaterial = SessionData.Instance.GetWeatherAffectedMaterial(meshRenderer.sharedMaterial, meshRenderer);
										}
									}
									Object @object = gameObject;
									string[] array = new string[6];
									array[0] = "dir: ";
									int num8 = 1;
									vector = normalized;
									array[num8] = vector.ToString();
									array[2] = " hit normal: ";
									array[3] = hit.normal.ToString();
									array[4] = " dot: ";
									array[5] = num6.ToString();
									@object.name = string.Concat(array);
									float num9 = (hit.distance - 0.55f) / component.sharedMesh.bounds.size.z;
									gameObject.transform.localScale = new Vector3(num9, num9, num9);
									gameObject.transform.LookAt(hit.point);
									Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(gameObject.transform.position);
									NewNode newNode = null;
									if (PathFinder.Instance.nodeMap.TryGetValue(new Vector3Int(vector3Int.x, vector3Int.y, 0), ref newNode) && newNode.gameLocation.thisAsStreet != null)
									{
										if (newNode.gameLocation.rooms[0].streetObjectContainer == null)
										{
											newNode.gameLocation.rooms[0].streetObjectContainer = new GameObject();
											newNode.gameLocation.rooms[0].streetObjectContainer.transform.SetParent(newNode.gameLocation.rooms[0].transform, false);
											newNode.gameLocation.rooms[0].streetObjectContainer.transform.localPosition = Vector3.zero;
											newNode.gameLocation.rooms[0].streetObjectContainer.transform.name = "StreetObjectContainer";
										}
										gameObject.transform.SetParent(newNode.gameLocation.rooms[0].streetObjectContainer.transform, true);
										if ((CityConstructor.Instance == null || CityConstructor.Instance.generateNew) && streetCable2.alterAreaLighting)
										{
											foreach (NewRoom.LightZoneData lightZoneData in newNode.gameLocation.rooms[0].lightZones)
											{
												if (streetCable2.possibleColours.Count > 0)
												{
													Color color = streetCable2.possibleColours[Toolbox.Instance.GetPsuedoRandomNumberContained(0, streetCable2.possibleColours.Count, this.seed, out this.seed)];
													if (streetCable2.lightOperation == DistrictPreset.AffectStreetAreaLights.lerp)
													{
														lightZoneData.areaLightColour = Color.Lerp(lightZoneData.areaLightColour, color, streetCable2.lightAmount);
													}
													else if (streetCable2.lightOperation == DistrictPreset.AffectStreetAreaLights.multiply)
													{
														lightZoneData.areaLightColour *= color * streetCable2.lightAmount;
													}
													else if (streetCable2.lightOperation == DistrictPreset.AffectStreetAreaLights.add)
													{
														lightZoneData.areaLightColour += color * streetCable2.lightAmount;
													}
													if (lightZoneData.spawnedAreaLight != null)
													{
														lightZoneData.spawnedAreaLight.color = lightZoneData.areaLightColour;
													}
												}
												lightZoneData.areaLightBrightness += streetCable2.brightnessModifier;
												lightZoneData.debug.Add("Area light street cable modifier: " + streetCable2.brightnessModifier.ToString());
												if (lightZoneData.spawnedAreaLight != null)
												{
													lightZoneData.aAdditional.intensity = lightZoneData.areaLightBrightness;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x0010F6D8 File Offset: 0x0010D8D8
	public void SpawnNeonSideSigns()
	{
		if (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew)
		{
			int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained((int)this.preset.signsPerBuildingRange.x, (int)this.preset.signsPerBuildingRange.y, this.seed, out this.seed);
			List<BuildingPreset.CableLinkPoint> list = new List<BuildingPreset.CableLinkPoint>(this.preset.sideSignPoints);
			List<GameObject> list2 = new List<GameObject>(this.preset.possibleNeonSigns);
			for (int i = 0; i < psuedoRandomNumberContained; i++)
			{
				if (list.Count > 0 && list2.Count > 0)
				{
					BuildingPreset.CableLinkPoint cableLinkPoint = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.seed, out this.seed)];
					GameObject gameObject = list2[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list2.Count, this.seed, out this.seed)];
					this.sideSigns.Add(new NewBuilding.SideSign
					{
						anchorPointIndex = this.preset.sideSignPoints.IndexOf(cableLinkPoint),
						signPrefabIndex = this.preset.possibleNeonSigns.IndexOf(gameObject)
					});
					list.Remove(cableLinkPoint);
					list2.Remove(gameObject);
				}
			}
		}
		foreach (NewBuilding.SideSign sideSign in this.sideSigns)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.preset.possibleNeonSigns[sideSign.signPrefabIndex], this.buildingModelBase.transform);
			gameObject2.transform.localPosition = this.preset.sideSignPoints[sideSign.anchorPointIndex].localPos;
			gameObject2.transform.localEulerAngles = this.preset.sideSignPoints[sideSign.anchorPointIndex].localRot;
		}
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x0010F8DC File Offset: 0x0010DADC
	public void GenerateAirDucts()
	{
		this.validVentSpace.Clear();
		foreach (KeyValuePair<int, NewFloor> keyValuePair in this.floors)
		{
			foreach (NewAddress newAddress in keyValuePair.Value.addresses)
			{
				foreach (NewRoom newRoom in newAddress.rooms)
				{
					int num = 0;
					List<NewNode> list = new List<NewNode>(newRoom.nodes);
					list.Sort((NewNode p1, NewNode p2) => p2.walls.Count.CompareTo(p1.walls.Count));
					foreach (NewNode newNode in list)
					{
						if (!newNode.tile.isStairwell && !newNode.tile.isInvertedStairwell && !newNode.tile.isObstacle)
						{
							if (newRoom.isNullRoom)
							{
								if (newRoom.IsOutside())
								{
									bool flag = false;
									foreach (Vector3Int vector3Int in CityData.Instance.offsetArrayX6)
									{
										for (int j = 0; j < keyValuePair.Value.maxDuctExtrusion; j++)
										{
											Vector3Int vector3Int2 = newNode.nodeCoord + vector3Int * (j + 1);
											NewNode newNode2 = null;
											if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode2) && !newNode2.room.IsOutside() && newNode2.gameLocation.building == this)
											{
												Vector3Int vector3Int3;
												vector3Int3..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3);
												this.validVentSpace.Add(vector3Int3, newNode);
												vector3Int3..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3 + 1);
												this.validVentSpace.Add(vector3Int3, newNode);
												vector3Int3..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3 + 2);
												this.validVentSpace.Add(vector3Int3, newNode);
												flag = true;
												break;
											}
										}
										if (flag)
										{
											break;
										}
									}
								}
								else
								{
									Vector3Int vector3Int4;
									vector3Int4..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3);
									this.validVentSpace.Add(vector3Int4, newNode);
									vector3Int4..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3 + 1);
									this.validVentSpace.Add(vector3Int4, newNode);
									vector3Int4..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3 + 2);
									this.validVentSpace.Add(vector3Int4, newNode);
								}
							}
							else
							{
								if (newNode.floor.defaultCeilingHeight <= 48 && (newNode.floorType == NewNode.FloorTileType.CeilingOnly || newNode.floorType == NewNode.FloorTileType.floorAndCeiling))
								{
									Vector3Int vector3Int5;
									vector3Int5..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3 + 2);
									this.validVentSpace.Add(vector3Int5, newNode);
								}
								if (newRoom.preset.allowUpperWallLevelDucts && (!newRoom.preset.onlyAllowUpperIfFloorLevelIsZero || newRoom.floor.defaultFloorHeight <= 1) && (newNode.floorType == NewNode.FloorTileType.CeilingOnly || newNode.floorType == NewNode.FloorTileType.floorAndCeiling) && num < newRoom.preset.limitUpperLevelDucts)
								{
									Vector3Int vector3Int6;
									vector3Int6..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3 + 1);
									this.validVentSpace.Add(vector3Int6, newNode);
									num++;
								}
								if (newRoom.preset.allowLowerWallLevelDucts)
								{
									Vector3Int vector3Int7;
									vector3Int7..ctor(newNode.nodeCoord.x, newNode.nodeCoord.y, newNode.nodeCoord.z * 3);
									this.validVentSpace.Add(vector3Int7, newNode);
								}
							}
						}
					}
				}
			}
		}
		foreach (KeyValuePair<int, NewFloor> keyValuePair2 in this.floors)
		{
			keyValuePair2.Value.GenerateAirDucts();
		}
		Dictionary<AirDuctGroup.AirVent, Vector3Int> dictionary = new Dictionary<AirDuctGroup.AirVent, Vector3Int>();
		List<AirDuctGroup.AirVent> list2 = new List<AirDuctGroup.AirVent>();
		foreach (KeyValuePair<int, NewFloor> keyValuePair3 in this.floors)
		{
			foreach (NewAddress newAddress2 in keyValuePair3.Value.addresses)
			{
				foreach (NewRoom newRoom2 in newAddress2.rooms)
				{
					foreach (AirDuctGroup.AirVent airVent in newRoom2.airVents)
					{
						Vector3Int zero = Vector3Int.zero;
						if (airVent.ventType == NewAddress.AirVent.ceiling)
						{
							zero..ctor(airVent.node.nodeCoord.x, airVent.node.nodeCoord.y, airVent.node.nodeCoord.z * 3 + 2);
							if (!this.validVentSpace.ContainsKey(zero))
							{
								this.validVentSpace.Add(zero, airVent.node);
							}
						}
						else if (airVent.ventType == NewAddress.AirVent.wallUpper)
						{
							zero..ctor(airVent.node.nodeCoord.x, airVent.node.nodeCoord.y, airVent.node.nodeCoord.z * 3 + 1);
							if (!this.validVentSpace.ContainsKey(zero))
							{
								this.validVentSpace.Add(zero, airVent.node);
							}
						}
						else if (airVent.ventType == NewAddress.AirVent.wallLower)
						{
							zero..ctor(airVent.node.nodeCoord.x, airVent.node.nodeCoord.y, airVent.node.nodeCoord.z * 3);
							if (!this.validVentSpace.ContainsKey(zero))
							{
								this.validVentSpace.Add(zero, airVent.node);
							}
						}
						dictionary.Add(airVent, zero);
						list2.Add(airVent);
					}
				}
			}
		}
		Dictionary<Vector3Int, NewBuilding.DuctPlacementData> dictionary2 = new Dictionary<Vector3Int, NewBuilding.DuctPlacementData>();
		while (dictionary.Count > 0)
		{
			List<AirDuctGroup.AirVent> list3 = Enumerable.ToList<AirDuctGroup.AirVent>(dictionary.Keys);
			AirDuctGroup.AirVent airVent2 = list3[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list3.Count, this.seed, out this.seed)];
			Vector3Int vector3Int8 = dictionary[airVent2];
			List<AirDuctGroup.AirVent> list4 = new List<AirDuctGroup.AirVent>();
			bool flag2 = false;
			int num2 = 99;
			while (!flag2 && dictionary.Count > 0 && num2 > 0)
			{
				AirDuctGroup.AirVent airVent3 = null;
				float num3 = float.PositiveInfinity;
				foreach (KeyValuePair<AirDuctGroup.AirVent, Vector3Int> keyValuePair4 in dictionary)
				{
					if (keyValuePair4.Key != airVent2 && !list4.Contains(keyValuePair4.Key))
					{
						float num4 = Vector3Int.Distance(keyValuePair4.Value, vector3Int8);
						if (num4 < num3)
						{
							airVent3 = keyValuePair4.Key;
							num3 = num4;
						}
					}
				}
				if (airVent3 == null)
				{
					break;
				}
				List<Vector3Int> ventRoute = this.GetVentRoute(vector3Int8, dictionary[airVent3], ref dictionary2);
				if (ventRoute != null && ventRoute.Count > 0)
				{
					for (int k = 0; k < ventRoute.Count; k++)
					{
						Vector3Int vector3Int9 = ventRoute[k];
						Vector3Int previous = vector3Int9;
						Vector3Int next = vector3Int9;
						if (k > 0)
						{
							previous = ventRoute[k - 1] - vector3Int9;
						}
						if (k < ventRoute.Count - 1)
						{
							next = ventRoute[k + 1] - vector3Int9;
						}
						if (!dictionary2.ContainsKey(vector3Int9))
						{
							dictionary2.Add(vector3Int9, new NewBuilding.DuctPlacementData
							{
								previous = previous,
								next = next,
								originVent = airVent2,
								destinationVent = airVent3
							});
						}
					}
					flag2 = true;
					break;
				}
				list4.Add(airVent3);
				num2--;
			}
			dictionary.Remove(airVent2);
		}
		List<Vector3Int> list5 = new List<Vector3Int>(Enumerable.ToList<Vector3Int>(dictionary2.Keys));
		List<Vector3Int> list6 = new List<Vector3Int>();
		List<Vector3Int> list7 = new List<Vector3Int>();
		int num5 = 99;
		while (list5.Count > 0 && num5 > 0)
		{
			if (list6.Count <= 0)
			{
				list6.Clear();
				list6.Add(list5[0]);
				GameObject gameObject = new GameObject();
				gameObject.transform.SetParent(base.transform, false);
				gameObject.transform.localPosition = Vector3.zero;
				AirDuctGroup airDuctGroup = gameObject.AddComponent<AirDuctGroup>();
				airDuctGroup.SetupNew(this);
				int num6 = 9999;
				while (list6.Count > 0 && num6 > 0)
				{
					Vector3Int vector3Int10 = list6[0];
					NewBuilding.DuctPlacementData ductPlacementData = dictionary2[vector3Int10];
					int num7 = Mathf.FloorToInt((float)vector3Int10.z / 3f);
					int level = Mathf.RoundToInt((float)vector3Int10.z - (float)num7 * 3f);
					Vector3Int vector3Int11;
					vector3Int11..ctor(vector3Int10.x, vector3Int10.y, num7);
					NewNode newNode3 = null;
					PathFinder.Instance.nodeMap.TryGetValue(vector3Int11, ref newNode3);
					if (newNode3.room.IsOutside())
					{
						airDuctGroup.isExterior = true;
					}
					airDuctGroup.AddAirDuctSection(level, vector3Int10, ductPlacementData.previous, ductPlacementData.next, newNode3, 0);
					airDuctGroup.AddAirVent(ductPlacementData.originVent);
					airDuctGroup.AddAirVent(ductPlacementData.destinationVent);
					foreach (Vector3Int vector3Int12 in CityData.Instance.offsetArrayX6)
					{
						Vector3Int vector3Int13 = vector3Int10 + vector3Int12;
						if (dictionary2.ContainsKey(vector3Int13) && !list6.Contains(vector3Int13) && !list7.Contains(vector3Int13))
						{
							int num8 = Mathf.FloorToInt((float)vector3Int13.z / 3f);
							Mathf.RoundToInt((float)vector3Int13.z - (float)num7 * 3f);
							Vector3Int vector3Int14;
							vector3Int14..ctor(vector3Int13.x, vector3Int13.y, num8);
							NewNode newNode4 = null;
							if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int14, ref newNode4))
							{
								if (newNode4.room.IsOutside() == airDuctGroup.isExterior)
								{
									list6.Add(vector3Int13);
								}
								else if (airDuctGroup.isExterior)
								{
									foreach (Vector3Int vector3Int15 in CityData.Instance.offsetArrayX6)
									{
										Vector3Int vector3Int16 = vector3Int14 + vector3Int15;
										NewNode newNode5 = null;
										if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int16, ref newNode5) && newNode5.room.IsOutside())
										{
											list6.Add(vector3Int13);
											break;
										}
									}
								}
							}
						}
					}
					list6.RemoveAt(0);
					list7.Add(vector3Int10);
					list5.Remove(vector3Int10);
					num6--;
					if (num6 <= 0)
					{
						Game.LogError("Error while assigning air duct groups: Safety level reached!", 2);
					}
				}
			}
			num5--;
			if (num5 <= 0)
			{
				Game.LogError("Error while assigning air duct groups: Safety level reached!", 2);
			}
		}
		foreach (AirDuctGroup airDuctGroup2 in this.airDucts)
		{
			foreach (AirDuctGroup.AirDuctSection airDuctSection in airDuctGroup2.airDucts)
			{
				Vector3Int[] offsetArrayX = CityData.Instance.offsetArrayX6;
				for (int i = 0; i < offsetArrayX.Length; i++)
				{
					Vector3Int vector3Int17 = offsetArrayX[i];
					Vector3Int searchCoord = airDuctSection.duct + vector3Int17;
					if (dictionary2.ContainsKey(searchCoord))
					{
						Predicate<AirDuctGroup.AirDuctSection> <>9__2;
						foreach (AirDuctGroup airDuctGroup3 in this.airDucts)
						{
							if (!(airDuctGroup3 == airDuctGroup2))
							{
								List<AirDuctGroup.AirDuctSection> list8 = airDuctGroup3.airDucts;
								Predicate<AirDuctGroup.AirDuctSection> predicate;
								if ((predicate = <>9__2) == null)
								{
									predicate = (<>9__2 = ((AirDuctGroup.AirDuctSection item) => item.duct == searchCoord));
								}
								if (list8.Exists(predicate))
								{
									airDuctGroup2.AddAdjoiningDuctGroup(airDuctGroup3);
									airDuctGroup3.AddAdjoiningDuctGroup(airDuctGroup2);
								}
							}
						}
					}
				}
			}
		}
		foreach (AirDuctGroup airDuctGroup4 in this.airDucts)
		{
			airDuctGroup4.LoadDucts();
		}
		this.validVentSpace.Clear();
		this.validVentSpace = null;
		List<AirDuctGroup.AirVent> list9 = list2.FindAll((AirDuctGroup.AirVent item) => item.group == null);
		if (list9.Count > 0)
		{
			string[] array = new string[5];
			array[0] = "CityGen: Detected ";
			int num9 = 1;
			int i = list9.Count;
			array[num9] = i.ToString();
			array[2] = " failed vents for building ";
			array[3] = this.preset.name;
			array[4] = ", removing them now...";
			Game.Log(string.Concat(array), 2);
			foreach (AirDuctGroup.AirVent airVent4 in list9)
			{
				airVent4.Remove();
			}
		}
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x001108E8 File Offset: 0x0010EAE8
	public Elevator AddStairwellSystem(NewTile newTile, StairwellPreset stairPreset)
	{
		for (int i = 0; i <= 50; i++)
		{
			Vector3Int vector3Int;
			vector3Int..ctor(newTile.globalTileCoord.x, newTile.globalTileCoord.y, newTile.globalTileCoord.z - i);
			NewTile newTile2 = null;
			if (PathFinder.Instance.tileMap.TryGetValue(vector3Int, ref newTile2) && this.stairwells.ContainsKey(newTile2))
			{
				this.stairwells[newTile2].AddFloor(newTile);
				return this.stairwells[newTile2];
			}
			vector3Int..ctor(newTile.globalTileCoord.x, newTile.globalTileCoord.y, newTile.globalTileCoord.z + i);
			newTile2 = null;
			if (PathFinder.Instance.tileMap.TryGetValue(vector3Int, ref newTile2) && this.stairwells.ContainsKey(newTile2))
			{
				this.stairwells[newTile2].AddFloor(newTile);
				return this.stairwells[newTile2];
			}
		}
		if (stairPreset == null)
		{
			stairPreset = InteriorControls.Instance.defaultStairwell;
		}
		return new Elevator(stairPreset, this, newTile);
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00110A09 File Offset: 0x0010EC09
	public int CompareTo(NewBuilding otherObject)
	{
		return this.cityTile.landValue.CompareTo(otherObject.cityTile.landValue);
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00110A34 File Offset: 0x0010EC34
	public CitySaveData.BuildingCitySave GenerateSaveData()
	{
		CitySaveData.BuildingCitySave buildingCitySave = new CitySaveData.BuildingCitySave();
		buildingCitySave.buildingID = this.buildingID;
		buildingCitySave.name = base.name;
		buildingCitySave.preset = this.preset.name;
		buildingCitySave.facing = this.facing;
		buildingCitySave.isInaccessible = this.isInaccessible;
		buildingCitySave.sideSigns = this.sideSigns;
		buildingCitySave.designStyle = this.designStyle.name;
		buildingCitySave.wood = this.wood;
		buildingCitySave.floorMaterial = this.floorMaterial.name;
		buildingCitySave.floorMatKey = this.floorMatKey;
		buildingCitySave.ceilingMaterial = this.ceilingMaterial.name;
		buildingCitySave.ceilingMatKey = this.ceilingMatKey;
		buildingCitySave.defaultWallMaterial = this.defaultWallMaterial.name;
		buildingCitySave.defaultWallKey = this.defaultWallKey;
		buildingCitySave.colourScheme = this.colourScheme.name;
		if (this.extWallMaterial != null)
		{
			buildingCitySave.extWallMaterial = this.extWallMaterial.name;
		}
		if (this.ceilingMaterialOverride != null)
		{
			buildingCitySave.ceilingMatOverride = this.ceilingMaterialOverride.name;
		}
		if (this.floorMaterialOverride != null)
		{
			buildingCitySave.floorMatOverride = this.floorMaterialOverride.name;
		}
		if (this.defaultWallMaterialOverride != null)
		{
			buildingCitySave.wallMatOverride = this.defaultWallMaterialOverride.name;
		}
		if (this.basementCeilingMaterialOverride != null)
		{
			buildingCitySave.ceilingMatOverrideB = this.basementCeilingMaterialOverride.name;
		}
		if (this.basementFloorMaterialOverride != null)
		{
			buildingCitySave.floorMatOverrideB = this.basementFloorMaterialOverride.name;
		}
		if (this.basementDefaultWallMaterialOverride != null)
		{
			buildingCitySave.wallMatOverrideB = this.basementDefaultWallMaterialOverride.name;
		}
		foreach (AirDuctGroup airDuctGroup in this.airDucts)
		{
			buildingCitySave.airDucts.Add(airDuctGroup.GenerateSaveData());
		}
		foreach (KeyValuePair<int, NewFloor> keyValuePair in this.floors)
		{
			buildingCitySave.floors.Add(keyValuePair.Value.GenerateSaveData());
		}
		return buildingCitySave;
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00110C98 File Offset: 0x0010EE98
	public void UpdateName(bool forceTrueRandom = false)
	{
		if (!this.isPlayerEditedName || this.playerEditedBuildingName.Length <= 0)
		{
			if (this.preset.overrideNaming && this.preset.possibleNames.Count > 0)
			{
				if (forceTrueRandom)
				{
					base.name = Strings.Get("names.building.bespoke", this.preset.possibleNames[Toolbox.Instance.Rand(0, this.preset.possibleNames.Count, true)], Strings.Casing.asIs, false, false, false, null).Trim();
				}
				else
				{
					base.name = Strings.Get("names.building.bespoke", this.preset.possibleNames[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.possibleNames.Count, this.seed, out this.seed)], Strings.Casing.asIs, false, false, false, null).Trim();
				}
			}
			else
			{
				string text = this.cityTile.district.EthnictiyBasedOnDominance().ToString();
				if (forceTrueRandom)
				{
					base.name = NameGenerator.Instance.GenerateName(null, 0f, "names." + text + ".sur", 1f, "names.building.suffix." + this.cityTile.landValue.ToString(), 1f, "").Trim();
				}
				else
				{
					base.name = NameGenerator.Instance.GenerateName(null, 0f, "names." + text + ".sur", 1f, "names.building.suffix." + this.cityTile.landValue.ToString(), 1f, this.seed).Trim();
				}
				int num = 99;
				while (HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Exists((NewBuilding item) => item != this && item.name == base.name))
				{
					if (num <= 0)
					{
						break;
					}
					Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
					base.name = NameGenerator.Instance.GenerateName(null, 0f, "names." + text + ".sur", 1f, "names.building.suffix." + this.cityTile.landValue.ToString(), 1f, this.seed).Trim();
					num--;
				}
			}
		}
		else
		{
			base.name = this.playerEditedBuildingName;
		}
		base.gameObject.name = base.name;
		this.nameOverride = null;
		foreach (KeyValuePair<int, NewFloor> keyValuePair in this.floors)
		{
			foreach (NewAddress newAddress in keyValuePair.Value.addresses)
			{
				if (newAddress.residence != null)
				{
					newAddress.SetName(newAddress.residence.GetResidenceString() + " " + base.name);
				}
				else if (newAddress.company != null)
				{
					newAddress.company.UpdateName();
				}
				else
				{
					string text2 = Strings.Get("names.rooms", newAddress.preset.name, Strings.Casing.firstLetterCaptial, false, false, false, null);
					if (newAddress.addressPreset != null && newAddress.addressPreset.nameFeaturesBuildingReference)
					{
						text2 = text2 + ", " + base.name;
					}
					newAddress.SetName(text2);
				}
				if (newAddress.addressPreset != null && newAddress.addressPreset.overrideBuildingName && this.nameOverride == null && (!this.isPlayerEditedName || this.playerEditedBuildingName.Length <= 0))
				{
					this.nameOverride = newAddress;
				}
				foreach (NewRoom newRoom in newAddress.rooms)
				{
					newRoom.SetRoomName();
				}
				newAddress.SetupNeonSigns();
			}
		}
		if (this.nameOverride != null && (!this.isPlayerEditedName || this.playerEditedBuildingName.Length <= 0))
		{
			base.name = this.nameOverride.name;
			base.gameObject.name = base.name;
			foreach (KeyValuePair<int, NewFloor> keyValuePair2 in this.floors)
			{
				foreach (NewAddress newAddress2 in keyValuePair2.Value.addresses)
				{
					if (!(newAddress2 == this.nameOverride))
					{
						if (newAddress2.residence != null)
						{
							newAddress2.SetName(newAddress2.residence.GetResidenceString() + " " + base.name);
						}
						else if (newAddress2.company != null)
						{
							newAddress2.company.UpdateName();
						}
						else
						{
							newAddress2.SetName(Strings.Get("names.rooms", newAddress2.preset.name, Strings.Casing.firstLetterCaptial, false, false, false, null));
						}
						foreach (NewRoom newRoom2 in newAddress2.rooms)
						{
							newRoom2.SetRoomName();
						}
					}
				}
			}
		}
		foreach (NewAddress newAddress3 in this.lobbies)
		{
			string text3 = string.Empty;
			if (newAddress3.floor.floor < 0)
			{
				text3 = Strings.Get("names.rooms", "basement", Strings.Casing.firstLetterCaptial, false, false, false, null) + " ";
			}
			if (newAddress3.floor.floor == 0)
			{
				text3 = Strings.Get("evidence.generic", "ground floor", Strings.Casing.firstLetterCaptial, false, false, false, null) + " " + Strings.Get("evidence.generic", "lobby", Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				text3 = text3 + Toolbox.Instance.GetNumbericalStringReference(Mathf.Abs(newAddress3.floor.floor)) + " " + Strings.Get("evidence.generic", "floor landing", Strings.Casing.asIs, false, false, false, null);
			}
			newAddress3.SetName(base.name + " " + text3);
		}
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x001113F4 File Offset: 0x0010F5F4
	public override void CreateEvidence()
	{
		if (this.evidenceEntry == null)
		{
			this.evidenceEntry = (EvidenceCreator.Instance.CreateEvidence("Building", "Building" + this.buildingID.ToString(), this, null, null, null, null, false, null) as EvidenceBuilding);
			this.residentRoster = (EvidenceCreator.Instance.CreateEvidence("ResidentRoster", "ResidentRoster" + this.buildingID.ToString(), this, null, null, null, null, false, null) as EvidenceMultiPage);
		}
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00111478 File Offset: 0x0010F678
	public void SetAlarm(bool newVal, Human target, NewFloor forFloor)
	{
		if (this.alarmActive != newVal)
		{
			this.alarmActive = newVal;
			if (this.alarmActive)
			{
				if (!GameplayController.Instance.activeAlarmsBuildings.Contains(this))
				{
					GameplayController.Instance.activeAlarmsBuildings.Add(this);
				}
				foreach (NewAddress newAddress in this.lobbies)
				{
					for (int i = 0; i < newAddress.currentOccupants.Count; i++)
					{
						Actor actor = newAddress.currentOccupants[i];
						if (!(actor == null) && !actor.isPlayer && !actor.isAsleep && !actor.isDead && !actor.isStunned && !(actor.ai == null))
						{
							actor.AddNerve(CitizenControls.Instance.nerveAlarm, null);
							actor.OnInvestigate(actor, 1);
						}
					}
				}
				this.TriggerAlarmPASounds();
			}
			else
			{
				this.alarmTimer = 0f;
				GameplayController.Instance.activeAlarmsBuildings.Remove(this);
				foreach (KeyValuePair<int, NewFloor> keyValuePair in this.floors)
				{
					keyValuePair.Value.SetAlarmLockdown(false, null);
				}
				this.alarmTargets.Clear();
				this.StopAlarmPASounds();
			}
			if (Player.Instance.currentBuilding == this)
			{
				StatusController.Instance.ForceStatusCheck();
			}
			for (int j = 0; j < this.alarms.Count; j++)
			{
				Interactable interactable = this.alarms[j];
				if (!(interactable.node.gameLocation.thisAsAddress != null) || !(interactable.node.gameLocation.thisAsAddress.addressPreset != null) || !interactable.node.gameLocation.thisAsAddress.addressPreset.useOwnSecuritySystem)
				{
					interactable.SetCustomState1(this.alarmActive, null, true, false, false);
				}
			}
			for (int k = 0; k < this.sentryGuns.Count; k++)
			{
				Interactable interactable2 = this.sentryGuns[k];
				if (!(interactable2.node.gameLocation.thisAsAddress != null) || !(interactable2.node.gameLocation.thisAsAddress.addressPreset != null) || !interactable2.node.gameLocation.thisAsAddress.addressPreset.useOwnSecuritySystem)
				{
					interactable2.SetCustomState1(this.alarmActive, null, true, false, false);
				}
			}
			for (int l = 0; l < this.otherSecurity.Count; l++)
			{
				Interactable interactable3 = this.otherSecurity[l];
				if (!(interactable3.node.gameLocation.thisAsAddress != null) || !(interactable3.node.gameLocation.thisAsAddress.addressPreset != null) || !interactable3.node.gameLocation.thisAsAddress.addressPreset.useOwnSecuritySystem)
				{
					interactable3.SetCustomState1(this.alarmActive, null, true, false, false);
				}
			}
			if (Player.Instance.currentBuilding == this)
			{
				Player.Instance.OnRoomChange();
			}
		}
		if (this.alarmActive)
		{
			if (target != null)
			{
				if (target.isPlayer)
				{
					SessionData.Instance.TutorialTrigger("alarms", false);
					StatusController.Instance.SetWantedInBuilding(this, GameplayControls.Instance.buildingWantedTime);
				}
				if (!this.alarmTargets.Contains(target))
				{
					this.alarmTargets.Add(target);
				}
			}
			this.alarmTimer = this.GetAlarmTime();
			if (forFloor != null)
			{
				forFloor.SetAlarmLockdown(true, null);
			}
		}
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x0010A92C File Offset: 0x00108B2C
	public float GetAlarmTime()
	{
		return Mathf.Lerp(GameplayControls.Instance.buildingAlarmTime.x, GameplayControls.Instance.buildingAlarmTime.y, 0f);
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x0011185C File Offset: 0x0010FA5C
	public bool IsAlarmSystemTarget(Human human)
	{
		if (human == null)
		{
			return false;
		}
		if (this.targetMode == NewBuilding.AlarmTargetMode.notPlayer)
		{
			if (!human.isPlayer)
			{
				return true;
			}
		}
		else
		{
			if (this.targetMode == NewBuilding.AlarmTargetMode.everybody)
			{
				return true;
			}
			if (this.targetMode == NewBuilding.AlarmTargetMode.illegalActivities)
			{
				if (this.alarmTargets.Contains(human) || (human.isPlayer && SessionData.Instance.gameTime < this.wantedInBuilding))
				{
					return true;
				}
			}
			else
			{
				if (this.targetMode == NewBuilding.AlarmTargetMode.nobody)
				{
					return false;
				}
				if (this.targetMode == NewBuilding.AlarmTargetMode.nonResidents)
				{
					return !(human.home != null) || !(human.home.building == this);
				}
			}
		}
		return false;
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x001118FE File Offset: 0x0010FAFE
	public void AddSecurityCamera(Interactable newInteractable)
	{
		this.securityCameras.Add(newInteractable);
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x0011190C File Offset: 0x0010FB0C
	public void AddSentryGun(Interactable newInteractable)
	{
		this.sentryGuns.Add(newInteractable);
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x0011191A File Offset: 0x0010FB1A
	public void AddOtherSecurity(Interactable newInteractable)
	{
		this.otherSecurity.Add(newInteractable);
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x00111928 File Offset: 0x0010FB28
	public void SetExteriorWallMaterialDefault(MaterialGroupPreset newMat)
	{
		this.extWallMaterial = newMat;
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00111934 File Offset: 0x0010FB34
	public List<Vector3Int> GetVentRoute(Vector3Int origin, Vector3Int destination, ref Dictionary<Vector3Int, NewBuilding.DuctPlacementData> placedDucts)
	{
		Dictionary<Vector3Int, bool> dictionary = new Dictionary<Vector3Int, bool>();
		Dictionary<Vector3Int, bool> dictionary2 = new Dictionary<Vector3Int, bool>();
		Dictionary<Vector3Int, Vector3Int> dictionary3 = new Dictionary<Vector3Int, Vector3Int>();
		Dictionary<Vector3Int, float> dictionary4 = new Dictionary<Vector3Int, float>();
		Dictionary<Vector3Int, float> dictionary5 = new Dictionary<Vector3Int, float>();
		dictionary2.Add(origin, true);
		dictionary4.Add(origin, 0f);
		dictionary5.Add(origin, Vector3.Distance(origin, destination));
		int num = 0;
		while (dictionary2.Count > 0)
		{
			Vector3Int vector3Int = Vector3Int.zero;
			float num2 = float.PositiveInfinity;
			foreach (KeyValuePair<Vector3Int, bool> keyValuePair in dictionary2)
			{
				if (dictionary5.ContainsKey(keyValuePair.Key) && dictionary5[keyValuePair.Key] < num2)
				{
					num2 = dictionary5[keyValuePair.Key];
					vector3Int = keyValuePair.Key;
				}
			}
			if (vector3Int == destination)
			{
				List<Vector3Int> list = new List<Vector3Int>();
				Vector3Int vector3Int2 = Vector3Int.zero;
				list.Add(vector3Int);
				while (dictionary3.ContainsKey(vector3Int))
				{
					vector3Int2 = vector3Int;
					vector3Int = dictionary3[vector3Int];
					if (vector3Int != vector3Int2)
					{
						list.Add(vector3Int);
					}
				}
				list.Reverse();
				return list;
			}
			dictionary2.Remove(vector3Int);
			dictionary.Add(vector3Int, true);
			Vector3Int vector3Int3 = vector3Int;
			dictionary3.TryGetValue(vector3Int, ref vector3Int3);
			Vector3Int vector3Int4 = vector3Int - vector3Int3;
			int num3 = Mathf.FloorToInt((float)vector3Int.z / 3f);
			Vector3Int vector3Int5;
			vector3Int5..ctor(vector3Int.x, vector3Int.y, num3);
			NewNode newNode = PathFinder.Instance.nodeMap[vector3Int5];
			foreach (Vector3Int vector3Int6 in CityData.Instance.offsetArrayX6)
			{
				Vector3Int vector3Int7 = vector3Int + vector3Int6;
				if (this.validVentSpace.ContainsKey(vector3Int7))
				{
					bool flag = false;
					if (!dictionary.TryGetValue(vector3Int7, ref flag))
					{
						int num4 = Mathf.FloorToInt((float)vector3Int7.z / 3f);
						Vector3Int vector3Int8;
						vector3Int8..ctor(vector3Int7.x, vector3Int7.y, num4);
						NewNode newNode2 = PathFinder.Instance.nodeMap[vector3Int8];
						bool flag2 = false;
						int num5 = Mathf.RoundToInt((float)(vector3Int7.z - num4));
						using (List<NewWall>.Enumerator enumerator2 = newNode.walls.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								NewWall wall = enumerator2.Current;
								if (wall.otherWall.node == newNode2)
								{
									if (wall.node.room.airVents.Exists((AirDuctGroup.AirVent item) => item.wall == wall))
									{
										flag2 = true;
										break;
									}
									if (wall.otherWall.node.room.airVents.Exists((AirDuctGroup.AirVent item) => item.wall == wall.otherWall))
									{
										flag2 = true;
										break;
									}
									if (wall.preset.sectionClass == DoorPairPreset.WallSectionClass.ventTop || wall.otherWall.preset.sectionClass == DoorPairPreset.WallSectionClass.ventTop)
									{
										flag2 = true;
										break;
									}
									if (wall.preset.sectionClass == DoorPairPreset.WallSectionClass.window || wall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge)
									{
										flag2 = true;
										break;
									}
									if (wall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance && (!wall.preset.divider || (wall.preset.divider && num5 >= 2)))
									{
										flag2 = true;
										break;
									}
								}
							}
						}
						if (!flag2)
						{
							float num6 = 2f;
							if (vector3Int4 == vector3Int6)
							{
								num6 = 0f;
							}
							if (placedDucts.ContainsKey(vector3Int7))
							{
								num6 = 0f;
							}
							float num7 = dictionary4[vector3Int] + 1f + num6;
							if (!dictionary2.TryGetValue(vector3Int7, ref flag))
							{
								dictionary2.Add(vector3Int7, true);
							}
							if (!dictionary4.ContainsKey(vector3Int7) || num7 < dictionary4[vector3Int7])
							{
								if (!dictionary3.ContainsKey(vector3Int7))
								{
									dictionary3.Add(vector3Int7, vector3Int);
								}
								else
								{
									dictionary3[vector3Int7] = vector3Int;
								}
								if (!dictionary4.ContainsKey(vector3Int7))
								{
									dictionary4.Add(vector3Int7, num7);
								}
								else
								{
									dictionary4[vector3Int7] = num7;
								}
								float num8 = dictionary4[vector3Int7] + Vector3.Distance(vector3Int7, destination);
								if (!dictionary5.ContainsKey(vector3Int7))
								{
									dictionary5.Add(vector3Int7, num8);
								}
								else
								{
									dictionary5[vector3Int7] = num8;
								}
							}
						}
					}
				}
			}
			num++;
			if (num > 9999)
			{
				return null;
			}
		}
		return null;
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00111E44 File Offset: 0x00110044
	public void CalculateDirectionalCullingTrees()
	{
		for (int i = this.floors.Count - 1; i > 0; i--)
		{
			if (this.floors.ContainsKey(i))
			{
				NewFloor newFloor = this.floors[i];
				if (this.directionalCullingTrees.Count >= 4)
				{
					break;
				}
				foreach (NewAddress newAddress in newFloor.addresses)
				{
					foreach (NewRoom newRoom in newAddress.rooms)
					{
						foreach (NewNode.NodeAccess nodeAccess in newRoom.entrances)
						{
							if (nodeAccess.toNode.room.IsOutside())
							{
								Vector2 vector = (nodeAccess.toNode.nodeCoord - nodeAccess.fromNode.nodeCoord).normalized;
								if (!this.directionalCullingTrees.ContainsKey(vector))
								{
									Dictionary<NewRoom, List<NewRoom.CullTreeEntry>> dictionary = new Dictionary<NewRoom, List<NewRoom.CullTreeEntry>>();
									foreach (NewRoom newRoom2 in CityData.Instance.roomDirectory)
									{
										if (!(newRoom2 == this) && newRoom2.nodes.Count > 0)
										{
											bool flag = false;
											if (newRoom2.gameLocation.thisAsStreet != null)
											{
												flag = true;
											}
											if ((!(newRoom2.preset != null) || !(newRoom2.preset.roomType == CityControls.Instance.nullDefaultRoom) || flag) && !newRoom2.IsOutside() && (flag || !(this != newRoom2.building) || !(newRoom2.preset != null) || newRoom2.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside || (newRoom.floor.floor <= 0 && newRoom2.floor.floor <= 0)) && (!dictionary.ContainsKey(newRoom2) || (dictionary[newRoom2][0].requiredOpenDoors != null && dictionary[newRoom2][0].requiredOpenDoors.Count > 0)))
											{
												bool flag2 = false;
												if ((!flag || !(newRoom.floor != null) || newRoom.floor.floor <= 0 || !(newRoom.preset != null) || newRoom.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside || nodeAccess.accessType == NewNode.NodeAccess.AccessType.window) && (flag || nodeAccess.accessType != NewNode.NodeAccess.AccessType.window || newRoom2.floor.floor <= 0 || !(newRoom2.preset != null) || newRoom2.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside))
												{
													float num = 91f;
													foreach (NewNode.NodeAccess nodeAccess2 in newRoom2.entrances)
													{
														if (nodeAccess2 != null && !(nodeAccess2.fromNode.room == nodeAccess2.toNode.room))
														{
															float num2 = Vector3.Angle(nodeAccess2.fromNode.nodeCoord - nodeAccess.toNode.nodeCoord, (nodeAccess.toNode.nodeCoord - nodeAccess.fromNode.nodeCoord).normalized);
															if (num2 >= -num && num2 <= num)
															{
																if (nodeAccess.toNode == null || nodeAccess2.fromNode == null)
																{
																	continue;
																}
																List<DataRaycastController.NodeRaycastHit> list;
																DataRaycastController.Instance.EntranceRaycast(nodeAccess, nodeAccess2, out list, false);
																new List<NewRoom>();
																for (int j = 1; j < list.Count; j++)
																{
																	DataRaycastController.NodeRaycastHit nodeRaycastHit = list[j];
																	NewNode newNode = null;
																	if (PathFinder.Instance.nodeMap.TryGetValue(nodeRaycastHit.coord, ref newNode) && newNode.room != null && !(newNode.room == this))
																	{
																		bool flag3;
																		if (!dictionary.ContainsKey(newNode.room))
																		{
																			dictionary.Add(newNode.room, new List<NewRoom.CullTreeEntry>());
																			flag3 = true;
																		}
																		else if (nodeRaycastHit.conditionalDoors == null || nodeRaycastHit.conditionalDoors.Count <= 0)
																		{
																			if (dictionary[newNode.room].Count > 0)
																			{
																				dictionary[newNode.room].Clear();
																			}
																			flag3 = true;
																		}
																		else
																		{
																			flag3 = true;
																			for (int k = 0; k < dictionary[newNode.room].Count; k++)
																			{
																				NewRoom.CullTreeEntry cullTreeEntry = dictionary[newNode.room][k];
																				if (cullTreeEntry.requiredOpenDoors == null || cullTreeEntry.requiredOpenDoors.Count <= 0)
																				{
																					flag3 = false;
																					break;
																				}
																				if (Enumerable.SequenceEqual<int>(cullTreeEntry.requiredOpenDoors, nodeRaycastHit.conditionalDoors))
																				{
																					flag3 = false;
																					break;
																				}
																			}
																		}
																		if (flag3)
																		{
																			dictionary[newNode.room].Add(new NewRoom.CullTreeEntry(nodeRaycastHit.conditionalDoors));
																			if (newNode.room.atriumTop != null)
																			{
																				if (!dictionary.ContainsKey(newNode.room.atriumTop))
																				{
																					dictionary.Add(newNode.room.atriumTop, new List<NewRoom.CullTreeEntry>());
																				}
																				dictionary[newNode.room.atriumTop].Clear();
																				dictionary[newNode.room.atriumTop].Add(new NewRoom.CullTreeEntry(nodeRaycastHit.conditionalDoors));
																			}
																			if (newNode.room == newRoom2 && (nodeRaycastHit.conditionalDoors == null || nodeRaycastHit.conditionalDoors.Count <= 0))
																			{
																				flag2 = true;
																			}
																		}
																	}
																}
																list = null;
															}
															if (flag2)
															{
																break;
															}
														}
													}
													if (flag2)
													{
														break;
													}
												}
											}
										}
									}
									this.directionalCullingTrees.Add(vector, dictionary);
									string[] array = new string[9];
									array[0] = "CityGen: Generated directional culling tree ";
									int num3 = 1;
									Vector2 vector2 = vector;
									array[num3] = vector2.ToString();
									array[2] = " for building ";
									array[3] = base.name;
									array[4] = " using room on floor ";
									array[5] = newRoom.floor.floor.ToString();
									array[6] = " with ";
									array[7] = dictionary.Count.ToString();
									array[8] = " entries...";
									Game.Log(string.Concat(array), 2);
									if (this.directionalCullingTrees.Count >= 4)
									{
										break;
									}
								}
							}
						}
						if (this.directionalCullingTrees.Count >= 4)
						{
							break;
						}
					}
					if (this.directionalCullingTrees.Count >= 4)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x001125E8 File Offset: 0x001107E8
	[Button(null, 0)]
	public void CountResidences()
	{
		int num = 0;
		foreach (KeyValuePair<int, NewFloor> keyValuePair in this.floors)
		{
			num += keyValuePair.Value.addresses.FindAll((NewAddress item) => item.residence != null).Count;
		}
		Game.Log("Found " + num.ToString() + " residences", 2);
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x0011268C File Offset: 0x0011088C
	public void TriggerAlarmPASounds()
	{
		this.StopAlarmPASounds();
		List<AudioController.FMODParam> alarmPAParams = this.GetAlarmPAParams();
		List<Vector3> list = new List<Vector3>();
		if (this.mainEntrance != null)
		{
			Vector3 vector = this.mainEntrance.otherWall.node.position + new Vector3(0f, 2.5f, 0f);
			string text = "Audio: Playing alarm pa at: ";
			Vector3 vector2 = vector;
			Game.Log(text + vector2.ToString(), 2);
			AudioController.LoopingSoundInfo loopingSoundInfo = AudioController.Instance.PlayWorldLoopingStatic(AudioControls.Instance.alarmPA, null, this.mainEntrance.otherWall.node, vector, alarmPAParams, 1f, false, false, null, null);
			if (loopingSoundInfo != null)
			{
				this.alarmPALoops.Add(loopingSoundInfo);
				list.Add(vector);
			}
		}
		foreach (NewWall newWall in this.additionalEntrances)
		{
			if (newWall != this.mainEntrance && !(newWall.door == null))
			{
				Vector3 vector3 = newWall.otherWall.node.position + new Vector3(0f, 2.5f, 0f);
				bool flag = false;
				using (List<Vector3>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (Vector3.Distance(enumerator2.Current, vector3) < 3f)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
				string text2 = "Audio: Playing alarm pa at: ";
				Vector3 vector2 = vector3;
				Game.Log(text2 + vector2.ToString(), 2);
				AudioController.LoopingSoundInfo loopingSoundInfo2 = AudioController.Instance.PlayWorldLoopingStatic(AudioControls.Instance.alarmPA, null, newWall.otherWall.node, vector3, alarmPAParams, 1f, false, false, null, null);
				if (loopingSoundInfo2 != null)
				{
					this.alarmPALoops.Add(loopingSoundInfo2);
					list.Add(vector3);
				}
			}
		}
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x001128B8 File Offset: 0x00110AB8
	public void UpdateAlarmPAWindowDistance(float val)
	{
		foreach (AudioController.LoopingSoundInfo loopingSoundInfo in this.alarmPALoops)
		{
			if (loopingSoundInfo != null)
			{
				if (loopingSoundInfo.parameters != null)
				{
					loopingSoundInfo.parameters.Find((AudioController.FMODParam item) => item.name == "WindowDistance").value = val;
				}
				loopingSoundInfo.audioEvent.setParameterByName("WindowDistance", val, false);
			}
		}
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00112958 File Offset: 0x00110B58
	public void UpdateAlarmPAExternalDoorDistance(float val)
	{
		foreach (AudioController.LoopingSoundInfo loopingSoundInfo in this.alarmPALoops)
		{
			if (loopingSoundInfo != null)
			{
				if (loopingSoundInfo.parameters != null)
				{
					loopingSoundInfo.parameters.Find((AudioController.FMODParam item) => item.name == "ExternalDoorDistance").value = val;
				}
				loopingSoundInfo.audioEvent.setParameterByName("ExternalDoorDistance", val, false);
			}
		}
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x001129F8 File Offset: 0x00110BF8
	public void UpdateAlarmPAIntExt(float val)
	{
		foreach (AudioController.LoopingSoundInfo loopingSoundInfo in this.alarmPALoops)
		{
			if (loopingSoundInfo != null)
			{
				if (loopingSoundInfo.parameters != null)
				{
					loopingSoundInfo.parameters.Find((AudioController.FMODParam item) => item.name == "Interior").value = val;
				}
				loopingSoundInfo.audioEvent.setParameterByName("Interior", val, false);
			}
		}
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x00112A98 File Offset: 0x00110C98
	private List<AudioController.FMODParam> GetAlarmPAParams()
	{
		List<AudioController.FMODParam> list = new List<AudioController.FMODParam>();
		AudioController.FMODParam fmodparam = new AudioController.FMODParam
		{
			name = "Interior"
		};
		if (Player.Instance.currentRoom != null && Player.Instance.currentRoom.IsOutside())
		{
			fmodparam.value = 0f;
		}
		else
		{
			fmodparam.value = 1f;
		}
		list.Add(fmodparam);
		list.Add(new AudioController.FMODParam
		{
			name = "ExternalDoorDistance",
			value = AudioController.Instance.closestDoorDistanceNormalized
		});
		list.Add(new AudioController.FMODParam
		{
			name = "WindowDistance",
			value = AudioController.Instance.closestWindowDistanceNormalized
		});
		return list;
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x00112B5C File Offset: 0x00110D5C
	public void StopAlarmPASounds()
	{
		Game.Log("Audio: Stopped PA alarm sounds for " + base.name, 2);
		for (int i = 0; i < this.alarmPALoops.Count; i++)
		{
			AudioController.Instance.StopSound(this.alarmPALoops[i], AudioController.StopType.triggerCue, "Stop building alarm");
		}
		this.alarmPALoops.Clear();
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x00112BBC File Offset: 0x00110DBC
	public void TriggerNewLostAndFound()
	{
		List<InteractablePreset> list = CityControls.Instance.lostAndFoundItems.FindAll((InteractablePreset item) => !this.lostAndFound.Exists((GameplayController.LostAndFound item2) => item2.preset == item.presetName));
		List<Citizen> list2 = CityData.Instance.citizenDirectory.FindAll((Citizen item) => item.home != null && item.home.building == this && !item.isDead && !this.lostAndFound.Exists((GameplayController.LostAndFound item2) => item2.ownerID == item.humanID));
		if (list2.Count > 0)
		{
			Citizen citizen = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
			Game.Log("Gameplay: Creating a new lost & found mission with owner " + citizen.GetCitizenName() + " for building " + base.name, 2);
			List<NewGameLocation.ObjectPlacement> list3 = new List<NewGameLocation.ObjectPlacement>();
			foreach (KeyValuePair<int, NewFloor> keyValuePair in this.floors)
			{
				foreach (NewAddress newAddress in keyValuePair.Value.addresses)
				{
					if ((newAddress.access == AddressPreset.AccessType.allPublic || newAddress.access == AddressPreset.AccessType.buildingInhabitants) && newAddress.addressPreset != null && newAddress.addressPreset.canFeatureLostAndFound && newAddress.nodes.Count > 9)
					{
						FurnitureLocation furnitureLocation;
						NewGameLocation.ObjectPlacement bestSpawnLocation = newAddress.GetBestSpawnLocation(CityControls.Instance.lostAndFoundNote, false, citizen, citizen, null, out furnitureLocation, null, true, 0, InteractablePreset.OwnedPlacementRule.prioritiseNonOwned, 1, null, false, null, null, null, "", true, false);
						if (bestSpawnLocation != null)
						{
							list3.Add(bestSpawnLocation);
						}
					}
				}
			}
			if (list3.Count > 0)
			{
				Interactable interactable = null;
				while (interactable == null && list.Count > 0)
				{
					InteractablePreset interactablePreset = list[Toolbox.Instance.Rand(0, list.Count, false)];
					if (interactablePreset != null)
					{
						List<NewGameLocation.ObjectPlacement> list4 = new List<NewGameLocation.ObjectPlacement>();
						foreach (KeyValuePair<int, NewFloor> keyValuePair2 in this.floors)
						{
							if (keyValuePair2.Key >= -1)
							{
								foreach (NewAddress newAddress2 in keyValuePair2.Value.addresses)
								{
									if (newAddress2.access == AddressPreset.AccessType.allPublic || newAddress2.access == AddressPreset.AccessType.buildingInhabitants)
									{
										FurnitureLocation furnitureLocation;
										NewGameLocation.ObjectPlacement bestSpawnLocation2 = newAddress2.GetBestSpawnLocation(interactablePreset, false, citizen, citizen, null, out furnitureLocation, null, true, 0, InteractablePreset.OwnedPlacementRule.prioritiseNonOwned, 2, null, false, null, null, null, "", true, false);
										if (bestSpawnLocation2 != null && !list3.Contains(bestSpawnLocation2))
										{
											list4.Add(bestSpawnLocation2);
										}
									}
								}
							}
						}
						Game.Log(string.Concat(new string[]
						{
							"Gameplay: ... Found ",
							list4.Count.ToString(),
							" valid locations for spawning a ",
							interactablePreset.presetName,
							" at ",
							base.name,
							"..."
						}), 2);
						if (list4.Count > 0)
						{
							NewGameLocation.ObjectPlacement objectPlacement = list4[Toolbox.Instance.Rand(0, list4.Count, false)];
							if (objectPlacement.existing != null)
							{
								objectPlacement.existing.SafeDelete(true);
							}
							if (objectPlacement.subSpawn == null)
							{
								interactable = InteractableCreator.Instance.CreateFurnitureSpawnedInteractableThreadSafe(interactablePreset, objectPlacement.furnParent.anchorNode.room, objectPlacement.furnParent, objectPlacement.location, citizen, citizen, null, null, null, null, "");
							}
							else
							{
								if (!objectPlacement.subSpawn.furnitureParent.createdInteractables)
								{
									objectPlacement.subSpawn.furnitureParent.CreateInteractables();
								}
								int num = Toolbox.Instance.Rand(0, objectPlacement.subSpawn.ssp.Count, false);
								InteractablePreset.SubSpawnSlot subSpawnSlot = objectPlacement.subSpawn.ssp[num];
								Vector2 vector = Random.insideUnitCircle * 0.2f;
								Vector3 vector2 = objectPlacement.subSpawn.wPos + subSpawnSlot.localPos;
								vector2 += new Vector3(vector.x, 0f, vector.y);
								Vector3 worldEuler = objectPlacement.subSpawn.wEuler + subSpawnSlot.localEuler;
								interactable = InteractableCreator.Instance.CreateWorldInteractable(interactablePreset, citizen, citizen, null, vector2, worldEuler, null, null, "");
								objectPlacement.subSpawn.ssp.RemoveAt(num);
							}
							if (interactable != null)
							{
								break;
							}
							list.Remove(interactablePreset);
						}
						else
						{
							list.Remove(interactablePreset);
						}
					}
				}
				if (interactable != null)
				{
					Game.Log("Gameplay: Successfully spawned " + interactable.GetName() + " at " + interactable.GetWorldPosition(true).ToString(), 2);
					NewGameLocation.ObjectPlacement objectPlacement2 = list3[Toolbox.Instance.Rand(0, list3.Count, false)];
					int num2 = Math.Clamp(Mathf.FloorToInt(interactable.val * 0.5f / 10f * 10f), 30, 300);
					List<Interactable.Passed> list5 = new List<Interactable.Passed>();
					list5.Add(new Interactable.Passed(Interactable.PassedVarType.lostItemPreset, 0f, interactable.preset.presetName));
					list5.Add(new Interactable.Passed(Interactable.PassedVarType.lostItemBuilding, (float)this.buildingID, null));
					list5.Add(new Interactable.Passed(Interactable.PassedVarType.lostItemReward, (float)num2, null));
					int floor = interactable.node.floor.floor;
					int max = Toolbox.Instance.Rand(3, 5, false);
					int num3 = Toolbox.Instance.Rand(0, max, false);
					int num4 = floor + num3;
					int num5 = floor + num3 - 4;
					int num6 = 100;
					while (!this.floors.ContainsKey(num5) && num6 > 0)
					{
						num4++;
						num5++;
						num6--;
					}
					num6 = 100;
					while (!this.floors.ContainsKey(num4) && num6 > 0)
					{
						num4--;
						num5--;
						num6--;
					}
					list5.Add(new Interactable.Passed(Interactable.PassedVarType.lostItemFloorX, (float)num5, null));
					list5.Add(new Interactable.Passed(Interactable.PassedVarType.lostItemFloorY, (float)num4, null));
					Interactable interactable2 = InteractableCreator.Instance.CreateFurnitureSpawnedInteractableThreadSafe(CityControls.Instance.lostAndFoundNote, objectPlacement2.furnParent.anchorNode.room, objectPlacement2.furnParent, objectPlacement2.location, citizen, citizen, null, list5, null, null, "");
					if (interactable2 != null)
					{
						Game.Log(string.Concat(new string[]
						{
							"Gameplay: Successfully created a new lost & found mission: Post located at ",
							interactable2.GetWorldPosition(true).ToString(),
							" (",
							interactable2.node.gameLocation.name,
							")"
						}), 2);
						GameplayController.LostAndFound lostAndFound = new GameplayController.LostAndFound();
						lostAndFound.preset = interactable.preset.presetName;
						lostAndFound.ownerID = citizen.humanID;
						lostAndFound.buildingID = this.buildingID;
						lostAndFound.spawnedItem = interactable.id;
						lostAndFound.spawnedNote = interactable2.id;
						lostAndFound.rewardMoney = num2;
						lostAndFound.rewardSC = GameplayControls.Instance.socialCreditForLostAndFound;
						this.lostAndFound.Add(lostAndFound);
						return;
					}
					Game.Log("Gameplay: Unable to spawn lost note within " + base.name, 2);
					return;
				}
			}
			else
			{
				Game.Log("Gameplay: Cannot find anywhere to post the lost & found note within " + base.name, 2);
			}
		}
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00113330 File Offset: 0x00111530
	public void CompleteLostAndFound(Citizen owner, InteractablePreset itemType, bool giveReward = true)
	{
		if (owner == null || itemType == null)
		{
			return;
		}
		foreach (GameplayController.LostAndFound lostAndFound in this.lostAndFound.FindAll((GameplayController.LostAndFound item) => item.ownerID == owner.humanID && item.preset == itemType.presetName && item.buildingID == this.buildingID))
		{
			Interactable interactable = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(lostAndFound.spawnedNote, ref interactable))
			{
				interactable.MarkAsTrash(true, false, 0f);
				interactable.SafeDelete(false);
			}
			Interactable interactable2 = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(lostAndFound.spawnedItem, ref interactable2))
			{
				interactable2.MarkAsTrash(true, false, 0f);
			}
			if (giveReward)
			{
				base.StartCoroutine(this.PayLostAndFoundReward(lostAndFound));
			}
			this.lostAndFound.Remove(lostAndFound);
		}
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x00113440 File Offset: 0x00111640
	private IEnumerator PayLostAndFoundReward(GameplayController.LostAndFound f)
	{
		float timer = 0f;
		while (timer < 4f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		GameplayController.Instance.AddMoney(f.rewardMoney, true, "Complete lost and found");
		GameplayController.Instance.AddSocialCredit(f.rewardSC, true, "Complete lost and found");
		if (AchievementsController.Instance != null)
		{
			AchievementsController.Instance.UnlockAchievement("Reunited", "return_lost_item");
		}
		yield break;
	}

	// Token: 0x0400171F RID: 5919
	[Header("ID")]
	public int buildingID;

	// Token: 0x04001720 RID: 5920
	public static int assignID = 1;

	// Token: 0x04001721 RID: 5921
	public string seed;

	// Token: 0x04001722 RID: 5922
	[Header("Custom Editor Flags")]
	public bool isPlayerEditedName;

	// Token: 0x04001723 RID: 5923
	public string playerEditedBuildingName = "";

	// Token: 0x04001724 RID: 5924
	[Header("Building Contents")]
	public Dictionary<int, NewFloor> floors = new Dictionary<int, NewFloor>();

	// Token: 0x04001725 RID: 5925
	public List<NewAddress> lobbies = new List<NewAddress>();

	// Token: 0x04001726 RID: 5926
	public List<GameObject> spawnedCables = new List<GameObject>();

	// Token: 0x04001727 RID: 5927
	public List<NewBuilding.SideSign> sideSigns = new List<NewBuilding.SideSign>();

	// Token: 0x04001728 RID: 5928
	public List<AirDuctGroup> airDucts = new List<AirDuctGroup>();

	// Token: 0x04001729 RID: 5929
	public Dictionary<Vector3Int, AirDuctGroup.AirDuctSection> ductMap = new Dictionary<Vector3Int, AirDuctGroup.AirDuctSection>();

	// Token: 0x0400172A RID: 5930
	[Header("Alarms")]
	public List<Interactable> alarms = new List<Interactable>();

	// Token: 0x0400172B RID: 5931
	public List<Interactable> sentryGuns = new List<Interactable>();

	// Token: 0x0400172C RID: 5932
	public List<Interactable> otherSecurity = new List<Interactable>();

	// Token: 0x0400172D RID: 5933
	public bool alarmActive;

	// Token: 0x0400172E RID: 5934
	public NewBuilding.AlarmTargetMode targetMode;

	// Token: 0x0400172F RID: 5935
	public float targetModeSetAt;

	// Token: 0x04001730 RID: 5936
	public List<Human> alarmTargets = new List<Human>();

	// Token: 0x04001731 RID: 5937
	public float alarmTimer;

	// Token: 0x04001732 RID: 5938
	public List<Interactable> securityCameras = new List<Interactable>();

	// Token: 0x04001733 RID: 5939
	public float wantedInBuilding;

	// Token: 0x04001734 RID: 5940
	public List<AudioController.LoopingSoundInfo> alarmPALoops = new List<AudioController.LoopingSoundInfo>();

	// Token: 0x04001735 RID: 5941
	public Dictionary<Vector2, Dictionary<NewRoom, List<NewRoom.CullTreeEntry>>> directionalCullingTrees = new Dictionary<Vector2, Dictionary<NewRoom, List<NewRoom.CullTreeEntry>>>();

	// Token: 0x04001736 RID: 5942
	[Header("Exterior Data")]
	public MaterialGroupPreset extWallMaterial;

	// Token: 0x04001737 RID: 5943
	public Material extMat;

	// Token: 0x04001738 RID: 5944
	public Dictionary<Vector3Int, NewNode> validVentSpace = new Dictionary<Vector3Int, NewNode>();

	// Token: 0x04001739 RID: 5945
	[Header("Culling: Mesh")]
	public GameObject buildingModelBase;

	// Token: 0x0400173A RID: 5946
	public List<GameObject> buildingModelsActual = new List<GameObject>();

	// Token: 0x0400173B RID: 5947
	public List<GameObject> buildingModelsLights = new List<GameObject>();

	// Token: 0x0400173C RID: 5948
	public List<Collider> colliders = new List<Collider>();

	// Token: 0x0400173D RID: 5949
	public Transform environmentalSettingsObject;

	// Token: 0x0400173E RID: 5950
	public bool displayBuildingModel = true;

	// Token: 0x0400173F RID: 5951
	public bool activeColliders = true;

	// Token: 0x04001740 RID: 5952
	private List<GameObject> selectivelyHidden = new List<GameObject>();

	// Token: 0x04001741 RID: 5953
	public List<Collider> snowColliders = new List<Collider>();

	// Token: 0x04001742 RID: 5954
	[Space(7f)]
	public GameObject cityEditorGroundFloorRepresentation;

	// Token: 0x04001743 RID: 5955
	[Header("Culling: Lighting")]
	public int interiorLightCullingLayer;

	// Token: 0x04001744 RID: 5956
	public List<LightController> allInteriorMainLights = new List<LightController>();

	// Token: 0x04001745 RID: 5957
	[Header("Location")]
	public BuildingPreset preset;

	// Token: 0x04001746 RID: 5958
	public int rotations;

	// Token: 0x04001747 RID: 5959
	public NewBuilding.Direction facing;

	// Token: 0x04001748 RID: 5960
	public CityTile cityTile;

	// Token: 0x04001749 RID: 5961
	public Vector3Int globalTileCoords;

	// Token: 0x0400174A RID: 5962
	public bool isInaccessible;

	// Token: 0x0400174B RID: 5963
	private float distance;

	// Token: 0x0400174C RID: 5964
	public Vector3 worldPosition;

	// Token: 0x0400174D RID: 5965
	[Header("Entrances")]
	public NewWall mainEntrance;

	// Token: 0x0400174E RID: 5966
	public StreetController street;

	// Token: 0x0400174F RID: 5967
	public List<NewWall> additionalEntrances = new List<NewWall>();

	// Token: 0x04001750 RID: 5968
	[Header("Stairwells")]
	public Dictionary<NewTile, Elevator> stairwells = new Dictionary<NewTile, Elevator>();

	// Token: 0x04001751 RID: 5969
	[Header("Emission")]
	public Texture2D emissionTextureInstanced;

	// Token: 0x04001752 RID: 5970
	public Texture2D emissionTextureUnlit;

	// Token: 0x04001753 RID: 5971
	[Header("Environmental")]
	public Volume volume;

	// Token: 0x04001754 RID: 5972
	[Header("Evidence")]
	[NonSerialized]
	public EvidenceBuilding evidenceEntry;

	// Token: 0x04001755 RID: 5973
	[NonSerialized]
	public EvidenceMultiPage residentRoster;

	// Token: 0x04001756 RID: 5974
	public List<TelephoneController.PhoneCall> callLog = new List<TelephoneController.PhoneCall>();

	// Token: 0x04001757 RID: 5975
	[Header("Decor")]
	public DesignStylePreset designStyle;

	// Token: 0x04001758 RID: 5976
	public Color wood;

	// Token: 0x04001759 RID: 5977
	public MaterialGroupPreset floorMaterial;

	// Token: 0x0400175A RID: 5978
	public Toolbox.MaterialKey floorMatKey;

	// Token: 0x0400175B RID: 5979
	public MaterialGroupPreset ceilingMaterial;

	// Token: 0x0400175C RID: 5980
	public Toolbox.MaterialKey ceilingMatKey;

	// Token: 0x0400175D RID: 5981
	public MaterialGroupPreset defaultWallMaterial;

	// Token: 0x0400175E RID: 5982
	public Toolbox.MaterialKey defaultWallKey;

	// Token: 0x0400175F RID: 5983
	public ColourSchemePreset colourScheme;

	// Token: 0x04001760 RID: 5984
	public bool triedGroundFloorRandom;

	// Token: 0x04001761 RID: 5985
	public bool groundFloorOverride;

	// Token: 0x04001762 RID: 5986
	public bool triedBasementRandom;

	// Token: 0x04001763 RID: 5987
	public bool basementFloorOverride;

	// Token: 0x04001764 RID: 5988
	public MaterialGroupPreset floorMaterialOverride;

	// Token: 0x04001765 RID: 5989
	public MaterialGroupPreset ceilingMaterialOverride;

	// Token: 0x04001766 RID: 5990
	public MaterialGroupPreset defaultWallMaterialOverride;

	// Token: 0x04001767 RID: 5991
	public MaterialGroupPreset basementFloorMaterialOverride;

	// Token: 0x04001768 RID: 5992
	public MaterialGroupPreset basementCeilingMaterialOverride;

	// Token: 0x04001769 RID: 5993
	public MaterialGroupPreset basementDefaultWallMaterialOverride;

	// Token: 0x0400176A RID: 5994
	public NewAddress nameOverride;

	// Token: 0x0400176B RID: 5995
	[Header("Lost & Found")]
	public List<GameplayController.LostAndFound> lostAndFound = new List<GameplayController.LostAndFound>();

	// Token: 0x0400176C RID: 5996
	[Header("Debug")]
	public List<string> debugDecor = new List<string>();

	// Token: 0x0400176D RID: 5997
	public static Comparison<NewBuilding> DistanceComparison = (NewBuilding object1, NewBuilding object2) => object1.distance.CompareTo(object2.distance);

	// Token: 0x0200034A RID: 842
	public enum AlarmTargetMode
	{
		// Token: 0x0400176F RID: 5999
		illegalActivities,
		// Token: 0x04001770 RID: 6000
		notPlayer,
		// Token: 0x04001771 RID: 6001
		nonResidents,
		// Token: 0x04001772 RID: 6002
		everybody,
		// Token: 0x04001773 RID: 6003
		nobody
	}

	// Token: 0x0200034B RID: 843
	public class DuctPlacementData
	{
		// Token: 0x04001774 RID: 6004
		public AirDuctGroup.AirVent originVent;

		// Token: 0x04001775 RID: 6005
		public AirDuctGroup.AirVent destinationVent;

		// Token: 0x04001776 RID: 6006
		public Vector3Int previous;

		// Token: 0x04001777 RID: 6007
		public Vector3Int next;
	}

	// Token: 0x0200034C RID: 844
	[Serializable]
	public class SideSign
	{
		// Token: 0x04001778 RID: 6008
		public int anchorPointIndex;

		// Token: 0x04001779 RID: 6009
		public int signPrefabIndex;
	}

	// Token: 0x0200034D RID: 845
	public enum Direction
	{
		// Token: 0x0400177B RID: 6011
		North,
		// Token: 0x0400177C RID: 6012
		East,
		// Token: 0x0400177D RID: 6013
		South,
		// Token: 0x0400177E RID: 6014
		West
	}
}
