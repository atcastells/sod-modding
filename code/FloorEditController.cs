using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000214 RID: 532
public class FloorEditController : MonoBehaviour
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000C08 RID: 3080 RVA: 0x000ACF30 File Offset: 0x000AB130
	public static FloorEditController Instance
	{
		get
		{
			return FloorEditController._instance;
		}
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x000ACF37 File Offset: 0x000AB137
	private void Awake()
	{
		if (FloorEditController._instance != null && FloorEditController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		FloorEditController._instance = this;
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x000ACF68 File Offset: 0x000AB168
	private void Start()
	{
		PathFinder.Instance.SetDimensions();
		CityData.Instance.seed = Toolbox.Instance.GenerateSeed(16, false, "");
		this.selectionLayerMask = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			29
		});
		this.wallsSelectionMask = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[1]);
		this.building.globalTileCoords = CityData.Instance.CityTileToTile(Vector2Int.zero);
		this.allLayouts = AssetLoader.Instance.GetAllLayoutConfigurations();
		this.allRoomConfigs = AssetLoader.Instance.GetAllRoomConfigurations();
		this.allLayoutTypes = AssetLoader.Instance.GetAllRoomTypePresets();
		this.addressTypeDropdown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = 0; i < this.allLayouts.Count; i++)
		{
			list.Add(this.allLayouts[i].name.ToString());
		}
		this.addressTypeDropdown.AddOptions(list);
		this.roomLayoutAssignDropdown.ClearOptions();
		this.allDoorPairs = AssetLoader.Instance.GetAllDoorPairPresets();
		this.wallPairsDropdown.ClearOptions();
		this.selectableDoorPairs = new List<DoorPairPreset>();
		foreach (DoorPairPreset doorPairPreset in this.allDoorPairs)
		{
			if (doorPairPreset.appearInEditor)
			{
				this.selectableDoorPairs.Add(doorPairPreset);
			}
		}
		list.Clear();
		for (int j = 0; j < this.selectableDoorPairs.Count; j++)
		{
			list.Add(this.selectableDoorPairs[j].name.ToString());
		}
		this.wallPairsDropdown.AddOptions(list);
		list.Clear();
		this.forceRoomDropdown.options.Clear();
		foreach (RoomConfiguration roomConfiguration in this.allRoomConfigs)
		{
			this.selectableRooms.Add(roomConfiguration);
			list.Add(roomConfiguration.name.ToString());
		}
		this.forceRoomDropdown.AddOptions(list);
		this.StartGame();
		this.SetTool(FloorEditController.FloorEditTool.none, false);
		this.SetDisplayMode(FloorEditController.EditorDisplayMode.normal);
		this.SetSelectionMode(FloorEditController.EditorSelectionMode.tile);
		if (RestartSafeController.Instance.newFloor)
		{
			RestartSafeController.Instance.newFloor = false;
			this.CreateNewFloor();
			return;
		}
		if (RestartSafeController.Instance.loadFloor)
		{
			if (CityData.Instance.floorData.Count <= 0)
			{
				CityData.Instance.ParseFloorData();
			}
			RestartSafeController.Instance.loadFloor = false;
			this.Load();
		}
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x000AD228 File Offset: 0x000AB428
	public void StartGame()
	{
		Player.Instance.fps.InitialiseController(true, true);
		Player.Instance.OnCityTileChange();
		InterfaceController.Instance.SetInterfaceActive(true);
		SessionData.Instance.startedGame = true;
		SessionData.Instance.ResumeGame();
		if (CitizenBehaviour.Instance != null)
		{
			CitizenBehaviour.Instance.StartGame();
		}
		InterfaceController.Instance.DisplayLocationText(4f, true);
		CameraController.Instance.SetupFPS();
		SessionData.Instance.OnPauseChange += this.OnPauseChange;
		SessionData.Instance.PauseGame(true, true, true);
		this.floorDesignationDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnNewFloorDesignationSetting();
		});
		this.addressDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnNewAddressDesignationSelection();
		});
		this.roomConfigAddressDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnNewAddressDesignationSelection2();
		});
		this.addressTypeDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnNewAddressTypeSelection();
		});
		this.wallPairsDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnNewWallDesignationSetting();
		});
		this.forceRoomDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnNewForceRoomSetting();
		});
		this.roomConfigsDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnNewRoomVariationSelection();
		});
		this.roomIDsDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnNewRoomSelection();
		});
		this.roomLayoutAssignDropdown.onValueChanged.AddListener(delegate(int <p0>)
		{
			this.OnAssignNewRoom();
		});
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x000AD3C0 File Offset: 0x000AB5C0
	private void Update()
	{
		if (this.isSaving)
		{
			return;
		}
		if (!RestartSafeController.Instance.recalculateAll)
		{
			if (this.selectionMode)
			{
				if (!EventSystem.current.IsPointerOverGameObject())
				{
					Ray ray = CameraController.Instance.cam.ScreenPointToRay(Input.mousePosition);
					int num = this.selectionLayerMask;
					if (this.selectionModeType == FloorEditController.EditorSelectionMode.wall)
					{
						num = this.wallsSelectionMask;
					}
					RaycastHit raycastHit;
					if (Physics.Raycast(ray, ref raycastHit, 200f, num))
					{
						if (this.selectionModeType == FloorEditController.EditorSelectionMode.tile)
						{
							EditorFloorTile component = raycastHit.transform.GetComponent<EditorFloorTile>();
							if (component != null && this.editFloor != null && this.editFloor.tileMap.ContainsKey(component.floorCoord))
							{
								NewTile newTile = this.editFloor.tileMap[component.floorCoord];
								if (newTile != this.tileSelection)
								{
									this.SelectNewTile(newTile);
								}
							}
						}
						else if (this.selectionModeType == FloorEditController.EditorSelectionMode.wall)
						{
							if (raycastHit.transform.parent != null && raycastHit.collider.isTrigger)
							{
								FloorEditWallDetector component2 = raycastHit.transform.parent.gameObject.GetComponent<FloorEditWallDetector>();
								if (component2 != null)
								{
									NewWall wall = component2.wall;
									if (wall != null && wall != this.wallSelection)
									{
										this.SelectNewWall(wall);
									}
								}
							}
						}
						else if (this.selectionModeType == FloorEditController.EditorSelectionMode.node)
						{
							Vector3 point = raycastHit.point;
							point.y = 0f;
							Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(point);
							NewNode newNode = null;
							if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && newNode != this.nodeSelection)
							{
								this.SelectNewNode(newNode);
							}
						}
					}
					if (Input.GetMouseButtonDown(0))
					{
						if (this.tool == FloorEditController.FloorEditTool.floorDesignation)
						{
							if (this.nodeSelection != null && !this.nodeSelection.tile.isEdge)
							{
								this.nodeSelection.SetFloorType(this.floorDesignationTypeSelection);
							}
						}
						else if (this.tool == FloorEditController.FloorEditTool.wallDesignation)
						{
							if (this.wallSelection != null)
							{
								if (this.addressSelection != null)
								{
									if (this.wallSelection.node.gameLocation == this.addressSelection || this.wallSelection.otherWall.node.gameLocation == this.addressSelection)
									{
										this.wallSelection.SetDoorPairPreset(this.wallPairPresetSelection, true, false, true);
										this.SaveCurrentVariation();
									}
									else
									{
										Game.Log("Cannot set wall: Invalid address selected!", 2);
									}
								}
								else
								{
									Game.Log("Cannot set wall: No address selected!", 2);
								}
							}
						}
						else if (this.tool == FloorEditController.FloorEditTool.rotateFloor)
						{
							if (this.tileSelection != null && !this.tileSelection.isEdge)
							{
								this.tileSelection.SetRotation(this.tileSelection.rotation + 90);
							}
						}
						else if (this.tool == FloorEditController.FloorEditTool.mainEntrance)
						{
							if (this.tileSelection != null && this.tileSelection.isEdge)
							{
								if (!this.tileSelection.isEntrance)
								{
									this.tileSelection.SetAsEntrance(true, true, true);
								}
								else
								{
									this.tileSelection.SetAsEntrance(false, false, true);
								}
							}
						}
						else if (this.tool == FloorEditController.FloorEditTool.secondaryEntrance)
						{
							if (this.tileSelection != null && this.tileSelection.isEdge)
							{
								if (!this.tileSelection.isEntrance)
								{
									this.tileSelection.SetAsEntrance(true, false, true);
								}
								else
								{
									this.tileSelection.SetAsEntrance(false, false, true);
								}
							}
						}
						else if (this.tool == FloorEditController.FloorEditTool.stairwell || this.tool == FloorEditController.FloorEditTool.elevator)
						{
							if (this.tileSelection != null && !this.tileSelection.isEdge)
							{
								if (!this.tileSelection.isStairwell)
								{
									bool isInverted = false;
									if (this.tool == FloorEditController.FloorEditTool.elevator)
									{
										isInverted = true;
									}
									this.tileSelection.SetAsStairwell(true, true, isInverted);
								}
								else if (this.tileSelection.stairwellRotation < 270)
								{
									this.tileSelection.SetStairwellRotation(this.tileSelection.stairwellRotation + 90);
								}
								else
								{
									this.tileSelection.SetAsStairwell(false, true, false);
									this.tileSelection.SetStairwellRotation(0);
								}
							}
						}
						else if (this.tool == FloorEditController.FloorEditTool.forceRoom && this.nodeSelection != null && !this.nodeSelection.tile.isEdge)
						{
							if (this.nodeSelection.forcedRoom == null)
							{
								this.nodeSelection.SetForcedRoom(this.forceRoomSelection);
								this.nodeSelection.forcedRoomRef = this.forceRoomDropdown.options[this.forceRoomDropdown.value].text;
							}
							else
							{
								this.nodeSelection.SetForcedRoom(null);
								this.nodeSelection.forcedRoomRef = null;
							}
						}
						this.UpdateStatusText();
						GenerationController.Instance.UpdateGeometryFloor(this.editFloor, "");
						GenerationController.Instance.LoadGeometryFloor(this.editFloor);
						this.SaveCurrentVariation();
					}
					if (Input.GetMouseButton(0))
					{
						if ((this.tool == FloorEditController.FloorEditTool.addressDesignation || this.tool == FloorEditController.FloorEditTool.roomDesignation) && !this.heldDown)
						{
							this.heldDown = true;
							this.heldDownOriginNode = this.nodeSelection;
							if (this.heldDownTransform == null)
							{
								GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.heldSelector, base.transform);
								this.heldDownTransform = gameObject.transform;
							}
						}
						if (this.heldDownTransform != null && this.heldDownOriginNode != null && this.nodeSelection != null)
						{
							this.heldDownTransform.transform.position = new Vector3((this.heldDownOriginNode.position.x + this.nodeSelection.position.x) / 2f, 0.1f, (this.heldDownOriginNode.position.z + this.nodeSelection.position.z) / 2f);
							this.heldDownTransform.localScale = new Vector3((float)(Mathf.Abs(this.heldDownOriginNode.floorCoord.x - this.nodeSelection.floorCoord.x) + 1) * 1.8f, (float)(Mathf.Abs(this.heldDownOriginNode.floorCoord.y - this.nodeSelection.floorCoord.y) + 1) * 1.8f, 1f);
						}
					}
					else if (this.heldDown)
					{
						if (this.tool == FloorEditController.FloorEditTool.addressDesignation || this.tool == FloorEditController.FloorEditTool.roomDesignation)
						{
							if (this.heldDownOriginNode != null && this.nodeSelection != null)
							{
								for (int i = Mathf.Min(this.heldDownOriginNode.floorCoord.x, this.nodeSelection.floorCoord.x); i <= Mathf.Max(this.heldDownOriginNode.floorCoord.x, this.nodeSelection.floorCoord.x); i++)
								{
									for (int j = Mathf.Min(this.heldDownOriginNode.floorCoord.y, this.nodeSelection.floorCoord.y); j <= Mathf.Max(this.heldDownOriginNode.floorCoord.y, this.nodeSelection.floorCoord.y); j++)
									{
										Vector2Int vector2Int;
										vector2Int..ctor(i, j);
										if (this.editFloor.nodeMap.ContainsKey(vector2Int))
										{
											if (this.tool == FloorEditController.FloorEditTool.addressDesignation)
											{
												NewNode newNode2 = this.editFloor.nodeMap[vector2Int];
												if (newNode2 != null && !newNode2.tile.isEdge)
												{
													this.addressSelection.AddNewNode(newNode2);
												}
											}
											else if (this.tool == FloorEditController.FloorEditTool.roomDesignation)
											{
												NewNode newNode3 = this.editFloor.nodeMap[vector2Int];
												if (newNode3 != null && this.addressSelection != null && this.roomSelection != null && newNode3.gameLocation == this.addressSelection && this.roomSelection != newNode3.room)
												{
													this.roomSelection.AddNewNode(newNode3);
												}
											}
										}
									}
								}
							}
							if (this.tool == FloorEditController.FloorEditTool.roomDesignation)
							{
								this.SetTool(FloorEditController.FloorEditTool.roomDesignation, false);
								this.SaveCurrentVariation();
							}
						}
						this.heldDown = false;
						this.heldDownOriginNode = null;
						if (this.heldDownTransform != null)
						{
							Object.Destroy(this.heldDownTransform.gameObject);
						}
						GenerationController.Instance.UpdateGeometryFloor(this.editFloor, "");
						GenerationController.Instance.LoadGeometryFloor(this.editFloor);
					}
				}
				else
				{
					if (this.tileSelection != null)
					{
						this.SelectNewTile(null);
					}
					if (this.wallSelection != null)
					{
						this.SelectNewWall(null);
					}
					if (this.nodeSelection != null)
					{
						this.SelectNewNode(null);
					}
				}
			}
			else if (this.heldDown)
			{
				this.heldDown = false;
				this.heldDownOriginNode = null;
				if (this.heldDownTransform != null)
				{
					Object.Destroy(this.heldDownTransform.gameObject);
				}
				GenerationController.Instance.UpdateGeometryFloor(this.editFloor, "");
				GenerationController.Instance.LoadGeometryFloor(this.editFloor);
			}
			if (!SessionData.Instance.play)
			{
				if (Input.GetMouseButton(1))
				{
					this.rightMouse = true;
					Player.Instance.EnablePlayerMouseLook(true, false);
					return;
				}
				if (this.rightMouse)
				{
					this.rightMouse = false;
					Player.Instance.EnablePlayerMouseLook(false, false);
				}
			}
			return;
		}
		if (RestartSafeController.Instance.floorList.Count <= 0)
		{
			this.recalculationDelay = 0;
			RestartSafeController.Instance.recalculateAll = false;
			Game.Log("Finished recalculating all floors.", 2);
			return;
		}
		if (this.recalculationDelay <= 0 && this.currentRecalculation.Length <= 0)
		{
			this.currentRecalculation = RestartSafeController.Instance.floorList[0];
			Game.Log("Recalulating " + this.currentRecalculation + "...", 2);
			this.LoadData(CityData.Instance.floorData[this.currentRecalculation]);
			GenerationController.Instance.LoadGeometryFloor(this.editFloor);
			this.recalculationDelay = 3;
			return;
		}
		if (this.recalculationDelay > 0 && !this.isSaving)
		{
			this.recalculationDelay--;
			if (this.recalculationDelay <= 0)
			{
				this.SaveCurrentData(this.editFloor);
				return;
			}
		}
		else if (this.recalculationDelay <= 0 && this.currentRecalculation.Length > 0)
		{
			RestartSafeController.Instance.floorList.RemoveAt(0);
			this.currentRecalculation = string.Empty;
			AudioController.Instance.StopAllSounds();
			SceneManager.LoadScene("FloorEditor");
		}
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x000ADE74 File Offset: 0x000AC074
	private void UpdateStatusText()
	{
		try
		{
			if (this.nodeSelection != null)
			{
				this.statusText.transform.parent.gameObject.SetActive(true);
				this.statusText.text = "Coord " + this.nodeSelection.floorCoord.ToString();
				TextMeshProUGUI textMeshProUGUI = this.statusText;
				textMeshProUGUI.text = textMeshProUGUI.text + "\nAddress: " + this.nodeSelection.gameLocation.name;
				TextMeshProUGUI textMeshProUGUI2 = this.statusText;
				textMeshProUGUI2.text = textMeshProUGUI2.text + "\nAddress Type: " + this.nodeSelection.gameLocation.thisAsAddress.preset.name;
				if (this.nodeSelection.room != null && this.nodeSelection.room.preset != null)
				{
					TextMeshProUGUI textMeshProUGUI3 = this.statusText;
					textMeshProUGUI3.text = textMeshProUGUI3.text + "\nRoom Type: " + this.nodeSelection.room.roomType.name;
				}
				TextMeshProUGUI textMeshProUGUI4 = this.statusText;
				textMeshProUGUI4.text = textMeshProUGUI4.text + "\nFloor Type: " + this.nodeSelection.floorType.ToString();
				TextMeshProUGUI textMeshProUGUI5 = this.statusText;
				textMeshProUGUI5.text = textMeshProUGUI5.text + "\nRoom ID: " + this.nodeSelection.room.roomID.ToString();
				if (this.nodeSelection.forcedRoom != null)
				{
					TextMeshProUGUI textMeshProUGUI6 = this.statusText;
					textMeshProUGUI6.text = textMeshProUGUI6.text + "\nForced Room: " + this.nodeSelection.forcedRoomRef;
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI7 = this.statusText;
					textMeshProUGUI7.text += "\nForced Room: None";
				}
				TextMeshProUGUI textMeshProUGUI8 = this.statusText;
				textMeshProUGUI8.text = textMeshProUGUI8.text + "\nAllow new furniture: " + this.nodeSelection.allowNewFurniture.ToString();
				TextMeshProUGUI textMeshProUGUI9 = this.statusText;
				textMeshProUGUI9.text = textMeshProUGUI9.text + "\nAccess Directions: " + this.nodeSelection.accessToOtherNodes.Count.ToString();
				TextMeshProUGUI textMeshProUGUI10 = this.statusText;
				textMeshProUGUI10.text = textMeshProUGUI10.text + "\nOptimized Floor: " + this.nodeSelection.tile.useOptimizedFloor.ToString();
				TextMeshProUGUI textMeshProUGUI11 = this.statusText;
				textMeshProUGUI11.text = textMeshProUGUI11.text + "\nOptimized Ceiling: " + this.nodeSelection.tile.useOptimizedCeiling.ToString();
			}
			else if (this.tileSelection != null)
			{
				this.statusText.transform.parent.gameObject.SetActive(true);
				this.statusText.text = "Coord " + this.tileSelection.globalTileCoord.ToString();
				if (this.tileSelection.isEntrance)
				{
					TextMeshProUGUI textMeshProUGUI12 = this.statusText;
					textMeshProUGUI12.text += "\nEntrance: Yes";
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI13 = this.statusText;
					textMeshProUGUI13.text += "\nEntrance: No";
				}
				if (this.tileSelection.isMainEntrance)
				{
					TextMeshProUGUI textMeshProUGUI14 = this.statusText;
					textMeshProUGUI14.text += "\nMain Entrance: Yes";
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI15 = this.statusText;
					textMeshProUGUI15.text += "\nMain Entrance: No";
				}
				if (this.tileSelection.isInvertedStairwell)
				{
					TextMeshProUGUI textMeshProUGUI16 = this.statusText;
					textMeshProUGUI16.text += "\nIs Elevator: Yes";
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI17 = this.statusText;
					textMeshProUGUI17.text += "\nIs Elevator: No";
				}
				if (this.tileSelection.isStairwell)
				{
					TextMeshProUGUI textMeshProUGUI18 = this.statusText;
					textMeshProUGUI18.text += "\nIs Stairwell: Yes";
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI19 = this.statusText;
					textMeshProUGUI19.text += "\nIs Stairwell: No";
				}
				if (this.tileSelection.useOptimizedCeiling)
				{
					TextMeshProUGUI textMeshProUGUI20 = this.statusText;
					textMeshProUGUI20.text += "\nOptimized Ceiling: Yes";
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI21 = this.statusText;
					textMeshProUGUI21.text += "\nOptimized Ceiling: No";
				}
				if (this.tileSelection.useOptimizedFloor)
				{
					TextMeshProUGUI textMeshProUGUI22 = this.statusText;
					textMeshProUGUI22.text += "\nOptimized Floor: Yes";
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI23 = this.statusText;
					textMeshProUGUI23.text += "\nOptimized Floor: No";
				}
			}
			else
			{
				this.statusText.transform.parent.gameObject.SetActive(false);
			}
		}
		catch
		{
		}
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x000AE334 File Offset: 0x000AC534
	public void SelectNewTile(NewTile newSelect)
	{
		this.tileSelection = newSelect;
		if (this.tileSelection != null)
		{
			this.selectionObject.gameObject.SetActive(true);
			this.selectionCoord = newSelect.floorCoord;
			this.selectionObject.transform.position = new Vector3(this.tileSelection.position.x, this.tileSelection.position.y, this.tileSelection.position.z);
		}
		else
		{
			this.selectionObject.gameObject.SetActive(false);
		}
		this.UpdateStatusText();
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x000AE3D0 File Offset: 0x000AC5D0
	public void SelectNewNode(NewNode newSelect)
	{
		this.nodeSelection = newSelect;
		if (this.nodeSelection != null)
		{
			this.selectionObject.gameObject.SetActive(true);
			this.selectionObject.transform.position = this.nodeSelection.position;
		}
		else
		{
			this.selectionObject.gameObject.SetActive(false);
		}
		this.UpdateStatusText();
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x000AE434 File Offset: 0x000AC634
	public void SelectNewWall(NewWall newSelect)
	{
		this.wallSelection = newSelect;
		if (this.wallSelection != null)
		{
			this.selectionObject.gameObject.SetActive(true);
			this.selectionObject.transform.position = this.wallSelection.position;
		}
		else
		{
			this.selectionObject.gameObject.SetActive(false);
		}
		this.UpdateStatusText();
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x000AE498 File Offset: 0x000AC698
	public void SetDisplayMode(FloorEditController.EditorDisplayMode newMode)
	{
		this.displayMode = newMode;
		if (this.editFloor == null)
		{
			return;
		}
		Game.Log("Update display mode: " + this.displayMode.ToString(), 2);
		if (this.displayMode == FloorEditController.EditorDisplayMode.normal)
		{
			using (List<NewAddress>.Enumerator enumerator = this.editFloor.addresses.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewAddress newAddress = enumerator.Current;
					foreach (NewNode newNode in newAddress.nodes)
					{
						if (newNode.spawnedFloor != null)
						{
							if (newNode.room.floorMat != null)
							{
								MaterialsController.Instance.SetMaterialGroup(newNode.spawnedFloor, newNode.room.floorMaterial, newNode.room.floorMatKey, false, null);
							}
							else
							{
								MaterialsController.Instance.SetMaterialGroup(newNode.spawnedFloor, this.defaultFloorMaterial, this.defaultMaterialKey, false, null);
							}
						}
					}
				}
				return;
			}
		}
		if (this.displayMode == FloorEditController.EditorDisplayMode.displayAddressDesignation)
		{
			using (List<NewAddress>.Enumerator enumerator = this.editFloor.addresses.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewAddress newAddress2 = enumerator.Current;
					foreach (NewNode newNode2 in newAddress2.nodes)
					{
						if (newNode2.spawnedFloor != null)
						{
							newNode2.spawnedFloor.GetComponent<MeshRenderer>().material = this.adddressDesignationMaterial;
							newNode2.spawnedFloor.GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", newAddress2.editorColour);
						}
					}
				}
				return;
			}
		}
		if (this.displayMode == FloorEditController.EditorDisplayMode.displayRoomSelection)
		{
			foreach (NewAddress newAddress3 in this.editFloor.addresses)
			{
				foreach (NewNode newNode3 in newAddress3.nodes)
				{
					if (newNode3.spawnedFloor != null)
					{
						if (this.addressSelection != null && this.roomSelection != null && this.roomSelection == newNode3.room)
						{
							newNode3.spawnedFloor.GetComponent<MeshRenderer>().material = this.adddressDesignationMaterial;
							newNode3.spawnedFloor.GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", newAddress3.editorColour);
						}
						else if (newNode3.room.floorMat != null)
						{
							MaterialsController.Instance.SetMaterialGroup(newNode3.spawnedFloor, newNode3.room.floorMaterial, newNode3.room.floorMatKey, false, null);
						}
						else
						{
							MaterialsController.Instance.SetMaterialGroup(newNode3.spawnedFloor, this.defaultFloorMaterial, this.defaultMaterialKey, false, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x000AE86C File Offset: 0x000ACA6C
	public void SetSelectionMode(FloorEditController.EditorSelectionMode newMode)
	{
		this.selectionModeType = newMode;
		if (this.selectionModeType == FloorEditController.EditorSelectionMode.tile)
		{
			this.floorSelectCursorObject.gameObject.SetActive(true);
			this.wallSelectCursorObject.gameObject.SetActive(false);
			return;
		}
		if (this.selectionModeType == FloorEditController.EditorSelectionMode.wall)
		{
			this.floorSelectCursorObject.gameObject.SetActive(false);
			this.wallSelectCursorObject.gameObject.SetActive(true);
			return;
		}
		if (this.selectionModeType == FloorEditController.EditorSelectionMode.node)
		{
			this.floorSelectCursorObject.gameObject.SetActive(false);
			this.wallSelectCursorObject.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x000AE904 File Offset: 0x000ACB04
	public void OnPauseChange(bool openDesktopMode)
	{
		Game.Log("OnPauseChange " + (!SessionData.Instance.play).ToString(), 2);
		if (SessionData.Instance.play)
		{
			Player.Instance.EnableGhostMovement(false, false, 0f);
			foreach (GameObject gameObject in this.wallTriggers)
			{
				gameObject.SetActive(false);
			}
			Player.Instance.transform.position = new Vector3(CameraController.Instance.transform.position.x, 2f, CameraController.Instance.transform.position.z);
			this.EnableSelectionMode(false);
			this.enabledInScrollView.SetActive(false);
			Player.Instance.EnablePlayerMouseLook(true, false);
			using (Dictionary<int, NewFloor>.Enumerator enumerator2 = this.building.floors.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<int, NewFloor> keyValuePair = enumerator2.Current;
					keyValuePair.Value.outsideAddress.nullRoom.SetVisible(false, true, "Floor Edit", false);
					foreach (NewRoom newRoom in keyValuePair.Value.outsideAddress.rooms)
					{
						newRoom.SetVisible(false, true, "Floor Edit", false);
					}
					foreach (NewAddress newAddress in keyValuePair.Value.addresses)
					{
						newAddress.nullRoom.SetVisible(false, true, "Floor Edit", false);
						foreach (NewRoom newRoom2 in newAddress.rooms)
						{
							newRoom2.SetVisible(false, true, "Floor Edit", false);
						}
						foreach (NewNode newNode in newAddress.nodes)
						{
							if (newNode.spawnedCeiling != null)
							{
								newNode.spawnedCeiling.GetComponent<MeshRenderer>().shadowCastingMode = 1;
							}
						}
					}
				}
				return;
			}
		}
		Player.Instance.EnableGhostMovement(true, false, 0f);
		foreach (KeyValuePair<int, NewFloor> keyValuePair2 in this.building.floors)
		{
			foreach (NewAddress newAddress2 in keyValuePair2.Value.addresses)
			{
				foreach (NewRoom newRoom3 in newAddress2.rooms)
				{
					newRoom3.SetVisible(true, true, "Floor Edit", false);
				}
			}
		}
		foreach (GameObject gameObject2 in this.wallTriggers)
		{
			gameObject2.SetActive(true);
		}
		this.EnableSelectionMode(true);
		this.enabledInScrollView.SetActive(true);
		Player.Instance.EnablePlayerMouseLook(false, false);
		foreach (KeyValuePair<int, NewFloor> keyValuePair3 in this.building.floors)
		{
			foreach (NewAddress newAddress3 in keyValuePair3.Value.addresses)
			{
				foreach (NewNode newNode2 in newAddress3.nodes)
				{
					if (newNode2.spawnedCeiling != null)
					{
						newNode2.spawnedCeiling.GetComponent<MeshRenderer>().shadowCastingMode = 3;
					}
				}
			}
		}
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x000AEE78 File Offset: 0x000AD078
	public void SetTool(int newTool)
	{
		this.SetTool((FloorEditController.FloorEditTool)newTool, false);
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x000AEE84 File Offset: 0x000AD084
	public void SetTool(FloorEditController.FloorEditTool newTool, bool forceRefresh = false)
	{
		if (newTool != this.tool || forceRefresh)
		{
			if (this.tool == FloorEditController.FloorEditTool.roomDesignation && this.tool != FloorEditController.FloorEditTool.roomDesignation)
			{
				this.SaveCurrentVariation();
			}
			this.tool = newTool;
			this.SetDisplayMode(FloorEditController.EditorDisplayMode.normal);
			this.SetSelectionMode(FloorEditController.EditorSelectionMode.tile);
			this.toolOptionsWindow.gameObject.SetActive(false);
			this.floorDesignationOptions.gameObject.SetActive(false);
			this.roomDesignationOptions.gameObject.SetActive(false);
			this.addressDesignationOptions.gameObject.SetActive(false);
			this.wallPairsOptions.gameObject.SetActive(false);
			this.forceRoomOptions.gameObject.SetActive(false);
			if (this.tool == FloorEditController.FloorEditTool.floorDesignation)
			{
				this.SetSelectionMode(FloorEditController.EditorSelectionMode.node);
				this.toolOptionsWindow.gameObject.SetActive(true);
				this.floorDesignationOptions.gameObject.SetActive(true);
				this.floorDesignationDropdown.ClearOptions();
				List<NewNode.FloorTileType> list = Enumerable.ToList<NewNode.FloorTileType>(Enumerable.Cast<NewNode.FloorTileType>(Enum.GetValues(typeof(NewNode.FloorTileType))));
				List<string> list2 = new List<string>();
				int value = 0;
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(list[i].ToString());
					if (this.floorDesignationTypeSelection == list[i])
					{
						value = i;
					}
				}
				this.floorDesignationDropdown.AddOptions(list2);
				this.floorDesignationDropdown.value = value;
				return;
			}
			if (this.tool == FloorEditController.FloorEditTool.roomDesignation)
			{
				this.SetSelectionMode(FloorEditController.EditorSelectionMode.node);
				this.SetDisplayMode(FloorEditController.EditorDisplayMode.displayRoomSelection);
				this.toolOptionsWindow.gameObject.SetActive(true);
				this.roomDesignationOptions.gameObject.SetActive(true);
				this.UpdateAddressDropdowns();
				this.UpdateRoomConfigsDropdown();
				this.UpdateRoomDesignationIDsDropdown();
				return;
			}
			if (this.tool == FloorEditController.FloorEditTool.addressDesignation)
			{
				this.SetSelectionMode(FloorEditController.EditorSelectionMode.node);
				this.SetDisplayMode(FloorEditController.EditorDisplayMode.displayAddressDesignation);
				this.toolOptionsWindow.gameObject.SetActive(true);
				this.addressDesignationOptions.gameObject.SetActive(true);
				this.UpdateAddressDropdowns();
				return;
			}
			if (this.tool == FloorEditController.FloorEditTool.wallDesignation)
			{
				this.toolOptionsWindow.gameObject.SetActive(true);
				this.wallPairsOptions.gameObject.SetActive(true);
				this.SetSelectionMode(FloorEditController.EditorSelectionMode.wall);
				this.wallPairsDropdown.value = this.selectableDoorPairs.IndexOf(this.wallPairPresetSelection);
				return;
			}
			if (this.tool != FloorEditController.FloorEditTool.rotateFloor && this.tool == FloorEditController.FloorEditTool.forceRoom)
			{
				this.SetSelectionMode(FloorEditController.EditorSelectionMode.node);
				this.SetDisplayMode(FloorEditController.EditorDisplayMode.displayAddressDesignation);
				this.toolOptionsWindow.gameObject.SetActive(true);
				this.forceRoomOptions.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x000AF111 File Offset: 0x000AD311
	public void NewFloorButton()
	{
		if (!this.newFloorWindow.gameObject.activeSelf)
		{
			this.newFloorWindow.gameObject.SetActive(true);
			return;
		}
		this.newFloorWindow.gameObject.SetActive(false);
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x000AF148 File Offset: 0x000AD348
	public void SaveFloorButton()
	{
		if (this.editFloor != null)
		{
			this.SaveCurrentData(this.editFloor);
		}
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x000AF164 File Offset: 0x000AD364
	public void SaveAsFloorButton()
	{
		if (!this.saveAsFloorWindow.gameObject.activeSelf)
		{
			this.saveAsFloorWindow.gameObject.SetActive(true);
			return;
		}
		this.saveAsFloorWindow.gameObject.SetActive(false);
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x000AF19C File Offset: 0x000AD39C
	public void EnableSelectionMode(bool val)
	{
		this.selectionObject.gameObject.SetActive(val);
		if (this.selectionMode != val)
		{
			this.selectionMode = val;
			this.selectionCoord = new Vector2(-1f, -1f);
			this.tileSelection = null;
			bool flag = this.selectionMode;
		}
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x000AF1F0 File Offset: 0x000AD3F0
	public void LoadFloorButton()
	{
		if (!this.loadFloorWindow.gameObject.activeSelf)
		{
			this.loadFloorWindow.gameObject.SetActive(true);
			this.loadDropdown.ClearOptions();
			this.loadFilePaths.Clear();
			foreach (KeyValuePair<string, FloorSaveData> keyValuePair in CityData.Instance.floorData)
			{
				this.loadFilePaths.Add(keyValuePair.Key);
			}
			this.loadDropdown.AddOptions(this.loadFilePaths);
			return;
		}
		this.loadFloorWindow.gameObject.SetActive(false);
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x000AF2B0 File Offset: 0x000AD4B0
	public void CreateNewFloorTrigger()
	{
		RestartSafeController.Instance.newFloor = true;
		RestartSafeController.Instance.newFloorName = this.newFloorName.text;
		RestartSafeController.Instance.newFloorSize = new Vector2((float)int.Parse(this.newFloorSizeX.text), (float)int.Parse(this.newFloorSizeY.text));
		RestartSafeController.Instance.newFloorFloorHeight = int.Parse(this.newFloorFloorHeight.text);
		RestartSafeController.Instance.newFloorCeilingHeight = int.Parse(this.newFloorCeilingHeight.text);
		AudioController.Instance.StopAllSounds();
		SceneManager.LoadScene("FloorEditor");
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x000AF358 File Offset: 0x000AD558
	public void CreateNewFloor()
	{
		Game.Log("CityGen: Creating a new floor following restart...", 2);
		NewAddress.assignEditorID = 1;
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.floor, base.transform);
		this.editFloor = gameObject.GetComponent<NewFloor>();
		this.editFloor.Setup(0, this.building, RestartSafeController.Instance.newFloorName, RestartSafeController.Instance.newFloorSize, RestartSafeController.Instance.newFloorFloorHeight, RestartSafeController.Instance.newFloorCeilingHeight);
		this.LoadEditorFloorToWorld();
		GenerationController.Instance.LoadGeometryFloor(this.editFloor);
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x000AF3E8 File Offset: 0x000AD5E8
	public void SaveAs()
	{
		if (this.editFloor == null)
		{
			return;
		}
		this.editFloor.floorName = this.newSaveAsFloorName.text;
		this.SaveCurrentData(this.editFloor);
		this.SaveAsFloorButton();
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x000AF424 File Offset: 0x000AD624
	public void LoadTrigger()
	{
		RestartSafeController.Instance.loadFloor = true;
		RestartSafeController.Instance.loadFloorString = this.loadDropdown.options[this.loadDropdown.value].text;
		AudioController.Instance.StopAllSounds();
		SceneManager.LoadScene("FloorEditor");
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x000AF47A File Offset: 0x000AD67A
	public void Load()
	{
		this.LoadData(CityData.Instance.floorData[RestartSafeController.Instance.loadFloorString]);
		GenerationController.Instance.LoadGeometryFloor(this.editFloor);
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x000AF4AB File Offset: 0x000AD6AB
	public void RecalculateAllTrigger()
	{
		RestartSafeController.Instance.recalculateAll = true;
		RestartSafeController.Instance.floorList.AddRange(CityData.Instance.floorData.Keys);
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x000AF4D8 File Offset: 0x000AD6D8
	public void SaveCurrentData(NewFloor data)
	{
		if (data == null)
		{
			return;
		}
		this.isSaving = true;
		Game.Log("Saving floor " + data.floorName + "...", 2);
		data.OnSaveDataComplete += this.OnCompleteSaveData;
		data.GetSaveData();
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x000AF52C File Offset: 0x000AD72C
	public void OnCompleteSaveData(NewFloor floor, FloorSaveData newSaveData)
	{
		floor.OnSaveDataComplete -= this.OnCompleteSaveData;
		string text = Path.Combine(Application.dataPath + "/AddressableAssets/FloorData/", floor.floorName + ".csv");
		string text2 = JsonUtility.ToJson(newSaveData);
		using (StreamWriter streamWriter = File.CreateText(text))
		{
			streamWriter.Write(text2);
		}
		if (!CityData.Instance.floorData.ContainsKey(floor.floorName))
		{
			CityData.Instance.floorData.Add(floor.floorName, null);
		}
		CityData.Instance.floorData[floor.floorName] = newSaveData;
		Game.Log("Completed saving " + floor.floorName, 2);
		CityData.Instance.ParseFloorData();
		this.isSaving = false;
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x000AF60C File Offset: 0x000AD80C
	public void LoadData(FloorSaveData savedData)
	{
		if (savedData == null)
		{
			return;
		}
		this.loaded = false;
		PathFinder.Instance.nodeMap.Clear();
		PathFinder.Instance.tileMap.Clear();
		NewAddress.assignEditorID = 1;
		if (this.editFloor != null && this.editFloor.building.floors.ContainsKey(this.editFloor.floor))
		{
			this.editFloor.building.floors.Remove(this.editFloor.floor);
			Object.Destroy(this.editFloor.gameObject);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.floor, base.transform);
		this.editFloor = gameObject.GetComponent<NewFloor>();
		this.editFloor.Setup(0, this.building, savedData.floorName, savedData.size, savedData.defaultFloorHeight, savedData.defaultCeilingHeight);
		this.LoadEditorFloorToWorld();
		this.editFloor.LoadDataToFloor(savedData);
		GenerationController.Instance.UpdateGeometryFloor(this.editFloor, "");
		GenerationController.Instance.LoadGeometryFloor(this.editFloor);
		if (SessionData.Instance.isFloorEdit)
		{
			this.editFloor.ConnectNodesOnFloor();
		}
		this.loaded = true;
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x000AF74C File Offset: 0x000AD94C
	public void LoadEditorFloorToWorld()
	{
		if (this.editFloor == null)
		{
			return;
		}
		int num = 0;
		while ((float)num < this.editFloor.size.x)
		{
			int num2 = 0;
			while ((float)num2 < this.editFloor.size.y)
			{
				for (int i = 0; i < CityControls.Instance.tileMultiplier; i++)
				{
					for (int j = 0; j < CityControls.Instance.tileMultiplier; j++)
					{
						Vector3Int coords;
						coords..ctor(num * CityControls.Instance.tileMultiplier + i, num2 * CityControls.Instance.tileMultiplier + j, 0);
						GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.editorTile, this.editorFloorParent);
						gameObject.transform.position = CityData.Instance.TileToRealpos(coords);
						gameObject.transform.position = new Vector3(gameObject.transform.position.x, -0.1f, gameObject.transform.position.z);
						gameObject.layer = 29;
						bool edgeTile = false;
						if (coords.x <= 0 || coords.y <= 0)
						{
							edgeTile = true;
							gameObject.GetComponent<MeshRenderer>().material = this.editorFloorEdgeMaterial;
						}
						else if ((float)coords.x >= this.editFloor.size.x * (float)CityControls.Instance.tileMultiplier - 1f || (float)coords.y >= this.editFloor.size.y * (float)CityControls.Instance.tileMultiplier - 1f)
						{
							edgeTile = true;
							gameObject.GetComponent<MeshRenderer>().material = this.editorFloorEdgeMaterial;
						}
						EditorFloorTile component = gameObject.GetComponent<EditorFloorTile>();
						component.floorCoord = new Vector2Int(coords.x, coords.y);
						component.edgeTile = edgeTile;
					}
				}
				num2++;
			}
			num++;
		}
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00002265 File Offset: 0x00000465
	public void OnPause()
	{
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x00002265 File Offset: 0x00000465
	public void OnPlay()
	{
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x000AF938 File Offset: 0x000ADB38
	public void OnNewFloorDesignationSetting()
	{
		this.floorDesignationTypeSelection = (NewNode.FloorTileType)this.floorDesignationDropdown.value;
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x000AF94C File Offset: 0x000ADB4C
	public void OnNewAddressDesignationSelection()
	{
		this.addressSelection = this.editFloor.addresses[this.addressDropdown.value];
		this.roomConfigAddressDropdown.SetValueWithoutNotify(this.addressDropdown.value);
		if (this.addressSelection != null)
		{
			Game.Log("New address selected:" + this.addressSelection.name, 2);
		}
		this.addressTypeSelection = this.addressSelection.preset;
		this.addressTypeDropdown.value = this.allLayouts.FindIndex((LayoutConfiguration item) => item == this.addressTypeSelection);
		this.addressDesignationColourImage.color = this.addressSelection.editorColour;
		this.addressDesignationColourImage2.color = this.addressSelection.editorColour;
		this.UpdateRoomConfigsDropdown();
		this.OnNewRoomVariationSelection();
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x000AFA24 File Offset: 0x000ADC24
	public void OnNewAddressDesignationSelection2()
	{
		this.addressSelection = this.editFloor.addresses[this.roomConfigAddressDropdown.value];
		this.addressDropdown.SetValueWithoutNotify(this.roomConfigAddressDropdown.value);
		if (this.addressSelection != null)
		{
			Game.Log("New address selected:" + this.addressSelection.name, 2);
		}
		this.addressTypeSelection = this.addressSelection.preset;
		this.addressTypeDropdown.value = this.allLayouts.FindIndex((LayoutConfiguration item) => item == this.addressTypeSelection);
		this.addressDesignationColourImage.color = this.addressSelection.editorColour;
		this.addressDesignationColourImage2.color = this.addressSelection.editorColour;
		this.UpdateRoomConfigsDropdown();
		this.OnNewRoomVariationSelection();
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x000AFAFC File Offset: 0x000ADCFC
	public void OnNewAddressTypeSelection()
	{
		this.addressSelection.SetAddressType(this.allLayouts[this.addressTypeDropdown.value]);
		if (this.tool == FloorEditController.FloorEditTool.addressDesignation)
		{
			this.SetTool(FloorEditController.FloorEditTool.addressDesignation, true);
			return;
		}
		if (this.tool == FloorEditController.FloorEditTool.roomDesignation)
		{
			this.SetTool(FloorEditController.FloorEditTool.roomDesignation, true);
		}
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x000AFB4F File Offset: 0x000ADD4F
	public void AddNewAddressButton()
	{
		this.editFloor.CreateNewAddress(this.addressTypeSelection, CityControls.Instance.defaultStyle);
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x000AFB70 File Offset: 0x000ADD70
	public void RemoveAddress()
	{
		for (int i = 0; i < this.editFloor.addresses[this.addressDropdown.value].nodes.Count; i++)
		{
			this.editFloor.lobbyAddress.AddNewNode(this.editFloor.addresses[this.addressDropdown.value].nodes[i]);
			i--;
		}
		Object.Destroy(this.editFloor.addresses[this.addressDropdown.value].gameObject);
		this.editFloor.addresses.RemoveAt(this.addressDropdown.value);
		this.SetTool(FloorEditController.FloorEditTool.addressDesignation, true);
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x000AFC2F File Offset: 0x000ADE2F
	public void OnNewWallDesignationSetting()
	{
		this.wallPairPresetSelection = this.selectableDoorPairs[this.wallPairsDropdown.value];
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x000AFC50 File Offset: 0x000ADE50
	public void OnNewForceRoomSetting()
	{
		string[] array = this.forceRoomDropdown.options[this.forceRoomDropdown.value].text.Split('.', 0);
		string lastElement = array[array.Length - 1];
		this.forceRoomSelection = this.allRoomConfigs.Find((RoomConfiguration item) => item.name == lastElement);
		Game.Log("Set force room selection to " + this.forceRoomDropdown.options[this.forceRoomDropdown.value].text, 2);
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x000AFCE8 File Offset: 0x000ADEE8
	public void GenerateAddressLayoutButton()
	{
		CityData.Instance.seed = Toolbox.Instance.GenerateSeed(16, false, "");
		GenerationController.Instance.GenerateAddressLayout(this.addressSelection);
		GenerationController.Instance.LoadGeometryFloor(this.editFloor);
		this.editFloor.ConnectNodesOnFloor();
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x000AFD3C File Offset: 0x000ADF3C
	public void GenerateAddressDecorButton()
	{
		this.addressSelection.CalculateLandValue();
		this.addressSelection.AssignPurpose();
		CityData.Instance.seed = Toolbox.Instance.GenerateSeed(16, false, "");
		GenerationController.Instance.GenerateGeometry(this.addressSelection);
		GenerationController.Instance.GenerateAddressDecor(this.addressSelection);
		GenerationController.Instance.LoadGeometryFloor(this.editFloor);
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x000AFDAC File Offset: 0x000ADFAC
	public void GenerateAddressLayoutAll()
	{
		foreach (NewAddress newAddress in this.editFloor.addresses)
		{
			if (newAddress.preset.roomLayout.Count > 0)
			{
				CityData.Instance.seed = Toolbox.Instance.GenerateSeed(16, false, "");
				GenerationController.Instance.GenerateAddressLayout(newAddress);
			}
		}
		GenerationController.Instance.LoadGeometryFloor(this.editFloor);
		this.editFloor.ConnectNodesOnFloor();
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x000AFE54 File Offset: 0x000AE054
	public void GenerateAddressDecorAll()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			foreach (NewAddress newAddress in this.editFloor.addresses)
			{
				foreach (NewRoom newRoom in newAddress.rooms)
				{
					newRoom.hasBeenDecorated = false;
				}
			}
		}
		foreach (NewAddress newAddress2 in this.editFloor.addresses)
		{
			if (newAddress2.preset.roomLayout.Count > 0)
			{
				newAddress2.CalculateLandValue();
				newAddress2.AssignPurpose();
				CityData.Instance.seed = Toolbox.Instance.GenerateSeed(16, false, "");
				GenerationController.Instance.GenerateGeometry(newAddress2);
				GenerationController.Instance.GenerateAddressDecor(newAddress2);
			}
		}
		GenerationController.Instance.LoadGeometryFloor(this.editFloor);
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x000AFF90 File Offset: 0x000AE190
	public void RemoveAllForcedRooms()
	{
		foreach (NewAddress newAddress in this.editFloor.addresses)
		{
			foreach (NewRoom newRoom in newAddress.rooms)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					newNode.SetForcedRoom(null);
				}
			}
		}
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x000B0058 File Offset: 0x000AE258
	[Button(null, 0)]
	public void ResetAllEntrances()
	{
		foreach (KeyValuePair<Vector2Int, NewTile> keyValuePair in this.editFloor.tileMap)
		{
			keyValuePair.Value.SetAsEntrance(false, false, true);
		}
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x000B00B8 File Offset: 0x000AE2B8
	public void UpdateAddressDropdowns()
	{
		this.addressDropdown.ClearOptions();
		this.roomConfigAddressDropdown.ClearOptions();
		List<string> list = new List<string>();
		int value = 0;
		if (this.editFloor != null)
		{
			for (int i = 0; i < this.editFloor.addresses.Count; i++)
			{
				list.Add(this.editFloor.addresses[i].name);
				if (this.addressSelection == this.editFloor.addresses[i])
				{
					value = i;
				}
			}
		}
		this.addressDropdown.AddOptions(list);
		this.roomConfigAddressDropdown.AddOptions(list);
		this.addressDropdown.value = value;
		this.roomConfigAddressDropdown.value = value;
		this.OnNewAddressDesignationSelection();
		this.UpdateRoomConfigsDropdown();
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x000B0184 File Offset: 0x000AE384
	public void UpdateRoomConfigsDropdown()
	{
		this.roomConfigsDropdown.ClearOptions();
		if (this.editFloor != null && this.addressSelection != null && this.addressSelection.saveData != null)
		{
			List<string> list = new List<string>();
			int value = 0;
			for (int i = 0; i < this.addressSelection.saveData.vs.Count; i++)
			{
				list.Add(i.ToString());
				if (i == this.addressSelection.loadedVarIndex)
				{
					value = i;
				}
			}
			this.roomConfigsDropdown.AddOptions(list);
			this.roomConfigsDropdown.value = value;
		}
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x000B0224 File Offset: 0x000AE424
	public void OnNewRoomVariationSelection()
	{
		if (this.editFloor != null && this.addressSelection != null && this.addressSelection.saveData != null && this.roomConfigsDropdown.value >= 0 && this.roomConfigsDropdown.value < this.addressSelection.saveData.vs.Count)
		{
			this.SaveCurrentVariation();
			Game.Log("Load variation #" + this.roomConfigsDropdown.value.ToString() + " for " + this.addressSelection.name, 2);
			GameObject gameObject;
			GenerationController.Instance.ResetLayout(this.addressSelection, out gameObject);
			this.editFloor.LoadVariation(this.addressSelection, this.addressSelection.saveData.vs[this.roomConfigsDropdown.value]);
			this.editFloor.FinalizeLoadingIn();
			GenerationController.Instance.LoadGeometryFloor(this.editFloor);
			this.editFloor.ConnectNodesOnFloor();
			this.UpdateRoomDesignationIDsDropdown();
		}
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x000B0344 File Offset: 0x000AE544
	public void UpdateRoomDesignationIDsDropdown()
	{
		this.roomIDsDropdown.ClearOptions();
		if (this.editFloor != null && this.addressSelection != null && this.addressSelection.saveData != null)
		{
			List<string> list = new List<string>();
			int value = 0;
			for (int i = 0; i < this.addressSelection.rooms.Count; i++)
			{
				list.Add(this.addressSelection.rooms[i].roomID.ToString() + ": " + this.addressSelection.rooms[i].roomType.name);
				if (this.roomSelection != null && this.roomSelection == this.addressSelection.rooms[i])
				{
					value = i;
				}
			}
			this.roomIDsDropdown.AddOptions(list);
			this.roomIDsDropdown.value = value;
		}
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x000B0440 File Offset: 0x000AE640
	public void OnNewRoomSelection()
	{
		if (this.editFloor != null && this.addressSelection != null && this.addressSelection.saveData != null)
		{
			int getID = -1;
			string[] array = this.roomIDsDropdown.options[this.roomIDsDropdown.value].text.Split(':', 0);
			if (array.Length != 0)
			{
				int.TryParse(array[0], ref getID);
			}
			this.roomSelection = this.addressSelection.rooms.Find((NewRoom item) => item.roomID == getID);
			if (this.roomSelection != null)
			{
				Game.Log(string.Concat(new string[]
				{
					"New room selected: ",
					this.roomSelection.name,
					" (",
					this.roomSelection.roomID.ToString(),
					")"
				}), 2);
			}
			this.SetDisplayMode(FloorEditController.EditorDisplayMode.displayRoomSelection);
			this.UpdateRoomLayoutAssignDropdown();
		}
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x000B0550 File Offset: 0x000AE750
	public void UpdateRoomLayoutAssignDropdown()
	{
		this.roomLayoutAssignDropdown.ClearOptions();
		if (this.editFloor != null && this.addressSelection != null && this.roomSelection != null)
		{
			List<string> list = new List<string>();
			list.Add(this.nullRoomType.name);
			int num = 1;
			int num2 = -1;
			if (this.roomSelection.roomType == this.nullRoomType)
			{
				num2 = 0;
			}
			for (int i = 0; i < this.addressSelection.preset.roomLayout.Count; i++)
			{
				if (!list.Contains(this.addressSelection.preset.roomLayout[i].name.ToString()))
				{
					list.Add(this.addressSelection.preset.roomLayout[i].name.ToString());
					if (this.roomSelection.roomType == this.addressSelection.preset.roomLayout[i])
					{
						Game.Log(string.Concat(new string[]
						{
							"Detected current room: ",
							this.roomSelection.roomType.ToString(),
							" (",
							num.ToString(),
							")"
						}), 2);
						num2 = num;
					}
					num++;
				}
			}
			this.roomLayoutAssignDropdown.AddOptions(list);
			if (num2 > -1)
			{
				this.roomLayoutAssignDropdown.value = num2;
			}
		}
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x000B06D8 File Offset: 0x000AE8D8
	public void OnAssignNewRoom()
	{
		if (this.editFloor != null && this.addressSelection != null && this.roomSelection != null)
		{
			RoomTypePreset roomTypePreset = this.allLayoutTypes.Find((RoomTypePreset item) => item.name == this.roomLayoutAssignDropdown.options[this.roomLayoutAssignDropdown.value].text);
			if (roomTypePreset != null && this.roomSelection.roomType != roomTypePreset)
			{
				Game.Log("Set new type " + roomTypePreset.name + " for room " + this.roomSelection.roomID.ToString(), 2);
				this.roomSelection.SetType(roomTypePreset);
				this.SaveCurrentVariation();
				this.UpdateRoomDesignationIDsDropdown();
			}
		}
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x000B078C File Offset: 0x000AE98C
	public void SaveCurrentVariation()
	{
		if (this.addressSelection != null && this.GetLoadedVariation(this.addressSelection) != null)
		{
			this.SaveLoadedAddressVariation(this.addressSelection);
		}
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x000B07B8 File Offset: 0x000AE9B8
	public void SaveLoadedAddressVariation(NewAddress add)
	{
		if (add != null && this.loaded && this.GetLoadedVariation(add) != null)
		{
			this.GetLoadedVariation(add).r_d = new List<RoomSaveData>();
			int num = 0;
			foreach (NewRoom newRoom in add.rooms)
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
						if (newWall.preset.id != "0")
						{
							num++;
						}
						nodeSaveData.w_d.Add(wallSaveData);
					}
					roomSaveData.n_d.Add(nodeSaveData);
				}
				this.GetLoadedVariation(add).r_d.Add(roomSaveData);
			}
			Game.Log(string.Concat(new string[]
			{
				"Saving the current variation of ",
				add.name,
				" (Index ",
				add.loadedVarIndex.ToString(),
				") Non default walls: ",
				num.ToString()
			}), 2);
		}
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x000B0A14 File Offset: 0x000AEC14
	public void AddVariationConfiguration()
	{
		if (this.addressSelection != null && this.addressSelection.saveData != null)
		{
			Game.Log("Creating new duplicated address variation...", 2);
			AddressLayoutVariation addressLayoutVariation = new AddressLayoutVariation();
			if (this.addressSelection.saveData.vs.Count <= 0)
			{
				CityData.Instance.seed = Toolbox.Instance.GenerateSeed(16, false, "");
				GenerationController.Instance.GenerateAddressLayout(this.addressSelection);
				GenerationController.Instance.LoadGeometryFloor(this.editFloor);
				this.editFloor.ConnectNodesOnFloor();
			}
			foreach (NewRoom newRoom in this.addressSelection.rooms)
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
			this.addressSelection.saveData.vs.Add(addressLayoutVariation);
			this.UpdateRoomConfigsDropdown();
			Game.Log("Selecting new duplicated address variation...", 2);
			this.roomConfigsDropdown.value = this.addressSelection.saveData.vs.Count - 1;
			this.SaveCurrentVariation();
		}
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x000B0CB0 File Offset: 0x000AEEB0
	public void RemoveVariationConfiguration()
	{
		if (this.addressSelection != null && this.addressSelection.saveData != null)
		{
			if (this.addressSelection.saveData.vs.Contains(this.GetLoadedVariation(this.addressSelection)))
			{
				Game.Log("Removing variation from address data...", 2);
				this.addressSelection.saveData.vs.Remove(this.GetLoadedVariation(this.addressSelection));
				this.UpdateRoomConfigsDropdown();
				this.SaveCurrentVariation();
				return;
			}
			Game.Log("Unable to find the selected variation in data!", 2);
		}
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x000B0D40 File Offset: 0x000AEF40
	public void AddRoom()
	{
		if (this.addressSelection != null)
		{
			NewRoom component = Object.Instantiate<GameObject>(PrefabControls.Instance.room, this.addressSelection.transform).GetComponent<NewRoom>();
			component.SetupLayoutOnly(this.addressSelection, this.nullRoomType, -1);
			this.addressSelection.AddNewRoom(component);
			this.UpdateRoomDesignationIDsDropdown();
			this.SaveCurrentVariation();
		}
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x000B0DA8 File Offset: 0x000AEFA8
	public void RemoveRoom()
	{
		if (this.roomSelection != null && this.addressSelection != null && this.addressSelection.nullRoom != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Removing room ",
				this.roomSelection.roomID.ToString(),
				" from ",
				this.addressSelection.name,
				"..."
			}), 2);
			foreach (NewNode newNode in new List<NewNode>(this.roomSelection.nodes))
			{
				this.addressSelection.nullRoom.AddNewNode(newNode);
			}
			this.addressSelection.RemoveRoom(this.roomSelection);
			this.UpdateRoomDesignationIDsDropdown();
			this.SaveCurrentVariation();
		}
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x000B0EB0 File Offset: 0x000AF0B0
	public AddressLayoutVariation GetLoadedVariation(NewAddress forAddress)
	{
		if (forAddress != null && forAddress.loadedVarIndex > -1 && forAddress.saveData != null)
		{
			try
			{
				return forAddress.saveData.vs[forAddress.loadedVarIndex];
			}
			catch
			{
				return null;
			}
		}
		return null;
	}

	// Token: 0x04000D5F RID: 3423
	[Header("General References")]
	public CityTile cityTile;

	// Token: 0x04000D60 RID: 3424
	public NewBuilding building;

	// Token: 0x04000D61 RID: 3425
	public Transform editorFloorParent;

	// Token: 0x04000D62 RID: 3426
	public GameObject enabledInScrollView;

	// Token: 0x04000D63 RID: 3427
	public RectTransform toolOptionsWindow;

	// Token: 0x04000D64 RID: 3428
	public FloorEditController.EditorDisplayMode displayMode;

	// Token: 0x04000D65 RID: 3429
	public FloorEditController.EditorSelectionMode selectionModeType;

	// Token: 0x04000D66 RID: 3430
	public InteractablePreset lightswitchPreset;

	// Token: 0x04000D67 RID: 3431
	public Transform fakeCitizenHolder;

	// Token: 0x04000D68 RID: 3432
	public bool heldDown;

	// Token: 0x04000D69 RID: 3433
	public NewNode heldDownOriginNode;

	// Token: 0x04000D6A RID: 3434
	public Transform heldDownTransform;

	// Token: 0x04000D6B RID: 3435
	private int recalculationDelay;

	// Token: 0x04000D6C RID: 3436
	private string currentRecalculation = string.Empty;

	// Token: 0x04000D6D RID: 3437
	public bool isSaving;

	// Token: 0x04000D6E RID: 3438
	public bool loaded;

	// Token: 0x04000D6F RID: 3439
	[Header("Movement")]
	public bool rightMouse;

	// Token: 0x04000D70 RID: 3440
	private int selectionLayerMask;

	// Token: 0x04000D71 RID: 3441
	private int wallsSelectionMask;

	// Token: 0x04000D72 RID: 3442
	[Header("New Floor")]
	public RectTransform newFloorWindow;

	// Token: 0x04000D73 RID: 3443
	public TMP_InputField newFloorName;

	// Token: 0x04000D74 RID: 3444
	public TMP_InputField newFloorSizeX;

	// Token: 0x04000D75 RID: 3445
	public TMP_InputField newFloorSizeY;

	// Token: 0x04000D76 RID: 3446
	public TMP_InputField newFloorFloorHeight;

	// Token: 0x04000D77 RID: 3447
	public TMP_InputField newFloorCeilingHeight;

	// Token: 0x04000D78 RID: 3448
	[Header("Save As")]
	public RectTransform saveAsFloorWindow;

	// Token: 0x04000D79 RID: 3449
	public InputField newSaveAsFloorName;

	// Token: 0x04000D7A RID: 3450
	[Header("Load")]
	public RectTransform loadFloorWindow;

	// Token: 0x04000D7B RID: 3451
	public TMP_Dropdown loadDropdown;

	// Token: 0x04000D7C RID: 3452
	private List<string> loadFilePaths = new List<string>();

	// Token: 0x04000D7D RID: 3453
	[Header("Map")]
	public GameObject mapParent;

	// Token: 0x04000D7E RID: 3454
	[Header("Current Data")]
	public NewFloor editFloor;

	// Token: 0x04000D7F RID: 3455
	[Header("Selection")]
	public bool selectionMode = true;

	// Token: 0x04000D80 RID: 3456
	public Transform selectionObject;

	// Token: 0x04000D81 RID: 3457
	public Transform floorSelectCursorObject;

	// Token: 0x04000D82 RID: 3458
	public Transform wallSelectCursorObject;

	// Token: 0x04000D83 RID: 3459
	public TextMeshProUGUI statusText;

	// Token: 0x04000D84 RID: 3460
	public NewTile tileSelection;

	// Token: 0x04000D85 RID: 3461
	public NewNode nodeSelection;

	// Token: 0x04000D86 RID: 3462
	public NewWall wallSelection;

	// Token: 0x04000D87 RID: 3463
	public Vector2 selectionCoord = new Vector2(-1f, -1f);

	// Token: 0x04000D88 RID: 3464
	public FloorEditController.FloorEditTool tool;

	// Token: 0x04000D89 RID: 3465
	public List<GameObject> wallTriggers = new List<GameObject>();

	// Token: 0x04000D8A RID: 3466
	[Header("Tools")]
	public RectTransform floorDesignationOptions;

	// Token: 0x04000D8B RID: 3467
	public TMP_Dropdown floorDesignationDropdown;

	// Token: 0x04000D8C RID: 3468
	public NewNode.FloorTileType floorDesignationTypeSelection = NewNode.FloorTileType.floorAndCeiling;

	// Token: 0x04000D8D RID: 3469
	[Space(5f)]
	public List<Color> editorAddressColours = new List<Color>();

	// Token: 0x04000D8E RID: 3470
	public Material adddressDesignationMaterial;

	// Token: 0x04000D8F RID: 3471
	public RectTransform addressDesignationOptions;

	// Token: 0x04000D90 RID: 3472
	public TMP_Dropdown addressDropdown;

	// Token: 0x04000D91 RID: 3473
	public TMP_Dropdown addressTypeDropdown;

	// Token: 0x04000D92 RID: 3474
	public NewAddress addressSelection;

	// Token: 0x04000D93 RID: 3475
	public LayoutConfiguration addressTypeSelection;

	// Token: 0x04000D94 RID: 3476
	public Image addressDesignationColourImage;

	// Token: 0x04000D95 RID: 3477
	public Image addressDesignationColourImage2;

	// Token: 0x04000D96 RID: 3478
	[Space(5f)]
	public RectTransform roomDesignationOptions;

	// Token: 0x04000D97 RID: 3479
	public TMP_Dropdown roomConfigAddressDropdown;

	// Token: 0x04000D98 RID: 3480
	public TMP_Dropdown roomConfigsDropdown;

	// Token: 0x04000D99 RID: 3481
	public TMP_Dropdown roomIDsDropdown;

	// Token: 0x04000D9A RID: 3482
	public TMP_Dropdown roomLayoutAssignDropdown;

	// Token: 0x04000D9B RID: 3483
	public NewRoom roomSelection;

	// Token: 0x04000D9C RID: 3484
	[Space(5f)]
	public RectTransform wallPairsOptions;

	// Token: 0x04000D9D RID: 3485
	public TMP_Dropdown wallPairsDropdown;

	// Token: 0x04000D9E RID: 3486
	public DoorPairPreset wallPairPresetSelection;

	// Token: 0x04000D9F RID: 3487
	[Space(5f)]
	public RectTransform forceRoomOptions;

	// Token: 0x04000DA0 RID: 3488
	public TMP_Dropdown forceRoomDropdown;

	// Token: 0x04000DA1 RID: 3489
	[NonSerialized]
	public RoomConfiguration forceRoomSelection;

	// Token: 0x04000DA2 RID: 3490
	[Space(5f)]
	public Toggle forceBasementToggle;

	// Token: 0x04000DA3 RID: 3491
	[Header("Materials")]
	public Material editorFloorMaterial;

	// Token: 0x04000DA4 RID: 3492
	public Material editorFloorEdgeMaterial;

	// Token: 0x04000DA5 RID: 3493
	public MaterialGroupPreset defaultFloorMaterial;

	// Token: 0x04000DA6 RID: 3494
	public Toolbox.MaterialKey defaultMaterialKey;

	// Token: 0x04000DA7 RID: 3495
	[Header("Scriptable Object Data")]
	public RoomTypePreset nullRoomType;

	// Token: 0x04000DA8 RID: 3496
	private List<RoomTypePreset> allLayoutTypes;

	// Token: 0x04000DA9 RID: 3497
	private List<LayoutConfiguration> allLayouts;

	// Token: 0x04000DAA RID: 3498
	private List<RoomConfiguration> allRoomConfigs;

	// Token: 0x04000DAB RID: 3499
	private List<DoorPairPreset> allDoorPairs;

	// Token: 0x04000DAC RID: 3500
	private List<DoorPairPreset> selectableDoorPairs;

	// Token: 0x04000DAD RID: 3501
	private List<RoomConfiguration> selectableRooms = new List<RoomConfiguration>();

	// Token: 0x04000DAE RID: 3502
	[Header("Debugging")]
	public Transform debugContainer;

	// Token: 0x04000DAF RID: 3503
	public GenerationDebugController currentlyDisplayingArea;

	// Token: 0x04000DB0 RID: 3504
	private static FloorEditController _instance;

	// Token: 0x02000215 RID: 533
	public enum EditorDisplayMode
	{
		// Token: 0x04000DB2 RID: 3506
		normal,
		// Token: 0x04000DB3 RID: 3507
		displayAddressDesignation,
		// Token: 0x04000DB4 RID: 3508
		displayRoomSelection
	}

	// Token: 0x02000216 RID: 534
	public enum EditorSelectionMode
	{
		// Token: 0x04000DB6 RID: 3510
		tile,
		// Token: 0x04000DB7 RID: 3511
		wall,
		// Token: 0x04000DB8 RID: 3512
		node
	}

	// Token: 0x02000217 RID: 535
	public enum FloorEditTool
	{
		// Token: 0x04000DBA RID: 3514
		none,
		// Token: 0x04000DBB RID: 3515
		floorDesignation,
		// Token: 0x04000DBC RID: 3516
		addressDesignation,
		// Token: 0x04000DBD RID: 3517
		wallDesignation,
		// Token: 0x04000DBE RID: 3518
		rotateFloor,
		// Token: 0x04000DBF RID: 3519
		mainEntrance,
		// Token: 0x04000DC0 RID: 3520
		secondaryEntrance,
		// Token: 0x04000DC1 RID: 3521
		stairwell,
		// Token: 0x04000DC2 RID: 3522
		elevator,
		// Token: 0x04000DC3 RID: 3523
		forceRoom,
		// Token: 0x04000DC4 RID: 3524
		roomDesignation
	}
}
