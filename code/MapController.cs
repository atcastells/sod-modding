using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005AA RID: 1450
public class MapController : MonoBehaviour
{
	// Token: 0x1400003E RID: 62
	// (add) Token: 0x06001FB7 RID: 8119 RVA: 0x001B4A64 File Offset: 0x001B2C64
	// (remove) Token: 0x06001FB8 RID: 8120 RVA: 0x001B4A9C File Offset: 0x001B2C9C
	public event MapController.RoutePlot OnPlotRoute;

	// Token: 0x1400003F RID: 63
	// (add) Token: 0x06001FB9 RID: 8121 RVA: 0x001B4AD4 File Offset: 0x001B2CD4
	// (remove) Token: 0x06001FBA RID: 8122 RVA: 0x001B4B0C File Offset: 0x001B2D0C
	public event MapController.RemoveRoute OnRemoveRoute;

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06001FBB RID: 8123 RVA: 0x001B4B41 File Offset: 0x001B2D41
	public static MapController Instance
	{
		get
		{
			return MapController._instance;
		}
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x001B4B48 File Offset: 0x001B2D48
	private void Awake()
	{
		if (MapController._instance != null && MapController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		MapController._instance = this;
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x001B4B78 File Offset: 0x001B2D78
	public void Setup()
	{
		this.realPositionMultiplier = this.nodePositionMultiplier / PathFinder.Instance.nodeSize.x;
		this.UpdateSize();
		this.BuildMap();
		this.districtMapName.text = CityData.Instance.cityName;
		this.keyUnexplored.text = Strings.Get("ui.interface", "MapKeyUnexplored", Strings.Casing.asIs, false, false, false, null);
		this.keyExploredSafe.text = Strings.Get("ui.interface", "MapKeyExploredSafe", Strings.Casing.asIs, false, false, false, null);
		this.keyExploredPrivate.text = Strings.Get("ui.interface", "MapKeyExploredPrivate", Strings.Casing.asIs, false, false, false, null);
		this.keyVent.text = Strings.Get("ui.interface", "MapKeyVent", Strings.Casing.asIs, false, false, false, null);
		this.keyDuct.text = Strings.Get("ui.interface", "MapKeyDuct", Strings.Casing.asIs, false, false, false, null);
		this.keyOpenHoursOnly.text = Strings.Get("ui.interface", "MapKeyOpenHoursOnly", Strings.Casing.asIs, false, false, false, null);
		this.SetFloorLayer(this.load, true);
		this.fzc.floorSlider.slider.minValue = CityData.Instance.floorRange.x;
		this.fzc.floorSlider.slider.maxValue = CityData.Instance.floorRange.y;
		this.fzc.floorSlider.slider.value = 0f;
		if (this.displayPlayerCharacter)
		{
			this.AddNewTrackedObject(Player.Instance.transform, PrefabControls.Instance.mapCharacterMarker, new Vector2(22f, 22f), PrefabControls.Instance.characterMarkerColor, true, Player.Instance);
		}
		this.plotRouteButton.SetInteractable(false);
		if (MapController.Instance.autoTravelButton != null)
		{
			this.autoTravelButton.SetInteractable(false);
		}
		this.drag.parentRect.sizeDelta = new Vector2(0f * this.savedSize, this.drag.parentRect.sizeDelta.y);
		if (this.displayFirstPerson)
		{
			this.displayFirstPerson = false;
		}
		this.drag.gameObject.SetActive(false);
		this.fzc.gameObject.SetActive(false);
		this.DisplayDirectionArrow(false);
		base.enabled = false;
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x001B4DCB File Offset: 0x001B2FCB
	public void ControllerMapHoverChange(ButtonController hoveredButton, bool hovered)
	{
		CasePanelController.Instance.mapMO.ForceMouseOver(hovered);
		CasePanelController.Instance.mapScroll.controlEnabled = hovered;
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x001B4DF8 File Offset: 0x001B2FF8
	public void BuildMap()
	{
		this.paperRect.sizeDelta = new Vector2(PathFinder.Instance.nodeCitySize.x * this.nodePositionMultiplier + this.positionBuffer, PathFinder.Instance.nodeCitySize.y * this.nodePositionMultiplier + this.positionBuffer);
		this.contentRect.sizeDelta = new Vector2(this.paperRect.sizeDelta.x + this.edgeBuffer * 2f, this.paperRect.sizeDelta.y + this.edgeBuffer * 2f);
		this.pinsRect.anchoredPosition = new Vector2(this.edgeBuffer, this.edgeBuffer);
		this.overlayAll.anchoredPosition = new Vector2(this.edgeBuffer, this.edgeBuffer);
		this.routesRect.anchoredPosition = new Vector2(this.edgeBuffer, this.edgeBuffer);
		int num = (int)CityData.Instance.floorRange.x;
		while ((float)num <= CityData.Instance.floorRange.y)
		{
			MapController.MapLayer mapLayer = default(MapController.MapLayer);
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.mapLayerCanvas, this.contentRect);
			mapLayer.canvas = gameObject.GetComponent<Canvas>();
			mapLayer.canvasGroup = gameObject.GetComponent<CanvasGroup>();
			GameObject gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.mapRectContainer, gameObject.transform);
			mapLayer.backgroundContainer = gameObject2.GetComponent<RectTransform>();
			mapLayer.backgroundContainer.anchoredPosition = new Vector2(this.edgeBuffer, this.edgeBuffer);
			mapLayer.backgroundContainer.transform.name = "Background";
			GameObject gameObject3 = Object.Instantiate<GameObject>(PrefabControls.Instance.mapRectContainer, gameObject.transform);
			mapLayer.baseContainer = gameObject3.GetComponent<RectTransform>();
			mapLayer.baseContainer.transform.name = "Base";
			mapLayer.baseContainer.anchoredPosition = new Vector2(this.edgeBuffer, this.edgeBuffer);
			GameObject gameObject4 = Object.Instantiate<GameObject>(PrefabControls.Instance.mapRectContainer, gameObject.transform);
			mapLayer.ductsContainer = gameObject4.GetComponent<RectTransform>();
			mapLayer.ductsContainer.transform.name = "Ducts";
			mapLayer.ductsContainer.anchoredPosition = new Vector2(this.edgeBuffer, this.edgeBuffer);
			mapLayer.baseBackgroundImages = new Dictionary<Vector2, RawImage>();
			mapLayer.wallImages = new Dictionary<Vector2, Image>();
			List<NewFloor> list = new List<NewFloor>();
			List<NewGameLocation> list2 = new List<NewGameLocation>();
			List<NewDoor> list3 = new List<NewDoor>();
			List<NewTile> list4 = new List<NewTile>();
			int num2 = 0;
			while ((float)num2 < PathFinder.Instance.nodeCitySize.x)
			{
				int num3 = 0;
				while ((float)num3 < PathFinder.Instance.nodeCitySize.y)
				{
					new Vector2((float)num2, (float)num3);
					Vector3Int vector3Int;
					vector3Int..ctor(num2, num3, num);
					NewNode newNode = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
					{
						foreach (NewWall newWall in newNode.walls)
						{
							if (newWall.door != null && !list3.Contains(newWall.door))
							{
								NewNode node = newWall.node;
								if (newWall.node == newNode)
								{
									node = newWall.otherWall.node;
								}
								if (newNode.room.nodes.Count >= node.room.nodes.Count)
								{
									GameObject gameObject5 = Object.Instantiate<GameObject>(PrefabControls.Instance.doorMapComponent, mapLayer.baseContainer);
									RectTransform component = gameObject5.GetComponent<RectTransform>();
									component.gameObject.SetActive(true);
									component.anchoredPosition = this.RealPosToMap(newWall.parentWall.position);
									component.localEulerAngles = new Vector3(0f, 0f, newWall.parentWall.localEulerAngles.y);
									list3.Add(newWall.door);
									newWall.door.mapDoorObject = (component.GetChild(0) as RectTransform);
									newWall.node.room.mapDoors.Add(component);
									newWall.otherWall.node.room.mapDoors.Add(component);
									if (newWall.node.room.explorationLevel < 1 && newWall.otherWall.node.room.explorationLevel < 1)
									{
										gameObject5.SetActive(false);
									}
								}
							}
						}
						if (!list2.Contains(newNode.gameLocation))
						{
							list2.Add(newNode.gameLocation);
						}
						if (newNode.gameLocation.floor != null && !list.Contains(newNode.gameLocation.floor))
						{
							list.Add(newNode.gameLocation.floor);
						}
						if (newNode.gameLocation.thisAsStreet != null && newNode.isObstacle && !list4.Contains(newNode.tile))
						{
							Object.Instantiate<GameObject>(PrefabControls.Instance.floorPlanBlockWall, mapLayer.backgroundContainer).GetComponent<RectTransform>().anchoredPosition = this.NodeCoordToMap(CityData.Instance.RealPosToNode(CityData.Instance.TileToRealpos(newNode.tile.globalTileCoord)));
							list4.Add(newNode.tile);
						}
					}
					num3++;
				}
				num2++;
			}
			foreach (NewGameLocation newGameLocation in list2)
			{
				MapAddressButtonController component2 = Object.Instantiate<GameObject>(PrefabControls.Instance.mapButtonComponent, mapLayer.backgroundContainer).GetComponent<MapAddressButtonController>();
				component2.gameObject.SetActive(true);
				Vector2 vector;
				vector..ctor(999999f, 999999f);
				Vector2 vector2;
				vector2..ctor(999999f, -999999f);
				Vector2 vector3;
				vector3..ctor(999999f, -999999f);
				foreach (NewNode newNode2 in newGameLocation.nodes)
				{
					Vector2 vector4 = this.NodeCoordToMap(newNode2.nodeCoord);
					vector.x = Mathf.Min(vector4.x - this.nodePositionMultiplier * 0.5f, vector.x);
					vector.y = Mathf.Min(vector4.y - this.nodePositionMultiplier * 0.5f, vector.y);
					vector2.x = Mathf.Min(vector4.x - this.nodePositionMultiplier * 0.5f, vector2.x);
					vector2.y = Mathf.Max(vector4.x + this.nodePositionMultiplier * 0.5f, vector2.y);
					vector3.x = Mathf.Min(vector4.y - this.nodePositionMultiplier * 0.5f, vector3.x);
					vector3.y = Mathf.Max(vector4.y + this.nodePositionMultiplier * 0.5f, vector3.y);
				}
				component2.range = new Vector2(vector2.y - vector2.x, vector3.y - vector3.x);
				component2.rect.anchoredPosition = vector;
				component2.rect.sizeDelta = component2.range;
				component2.rect.localScale = Vector3.one;
				component2.Setup(newGameLocation);
				component2.transform.SetAsFirstSibling();
				this.buttons.Add(component2);
			}
			foreach (NewFloor newFloor in list)
			{
				MapDuctsButtonController component3 = Object.Instantiate<GameObject>(PrefabControls.Instance.mapDuctComponent, mapLayer.ductsContainer).GetComponent<MapDuctsButtonController>();
				component3.gameObject.SetActive(true);
				Vector2 vector5;
				vector5..ctor(999999f, 999999f);
				Vector2 vector6;
				vector6..ctor(999999f, -999999f);
				Vector2 vector7;
				vector7..ctor(999999f, -999999f);
				foreach (NewAddress newAddress in newFloor.addresses)
				{
					foreach (NewNode newNode3 in newAddress.nodes)
					{
						Vector2 vector8 = this.NodeCoordToMap(newNode3.nodeCoord);
						vector5.x = Mathf.Min(vector8.x - this.nodePositionMultiplier * 0.5f, vector5.x);
						vector5.y = Mathf.Min(vector8.y - this.nodePositionMultiplier * 0.5f, vector5.y);
						vector6.x = Mathf.Min(vector8.x - this.nodePositionMultiplier * 0.5f, vector6.x);
						vector6.y = Mathf.Max(vector8.x + this.nodePositionMultiplier * 0.5f, vector6.y);
						vector7.x = Mathf.Min(vector8.y - this.nodePositionMultiplier * 0.5f, vector7.x);
						vector7.y = Mathf.Max(vector8.y + this.nodePositionMultiplier * 0.5f, vector7.y);
					}
				}
				component3.range = new Vector2(vector6.y - vector6.x, vector7.y - vector7.x);
				component3.rect.anchoredPosition = vector5;
				component3.rect.sizeDelta = component3.range;
				component3.rect.localScale = Vector3.one;
				component3.Setup(newFloor);
				component3.transform.SetAsFirstSibling();
			}
			GameObject gameObject6 = Object.Instantiate<GameObject>(PrefabControls.Instance.mapRectContainer, gameObject.transform);
			mapLayer.drawingController = gameObject6.AddComponent<DrawingController>();
			mapLayer.drawingController.container = gameObject6.GetComponent<RectTransform>();
			mapLayer.drawingController.container.sizeDelta = this.contentRect.sizeDelta;
			mapLayer.drawingController.container.anchoredPosition = Vector2.zero;
			mapLayer.drawingController.container.transform.name = "Drawing";
			mapLayer.drawingController.img = gameObject6.AddComponent<RawImage>();
			mapLayer.drawingController.img.texture = null;
			mapLayer.drawingController.img.color = Color.clear;
			mapLayer.drawingController.brushImage = this.drawBrushRect.GetComponent<RawImage>();
			mapLayer.drawingController.brush = PrefabControls.Instance.drawingBrush;
			mapLayer.drawingController.eraserButton = this.eraserButton;
			mapLayer.drawingController.toggleDrawingButton = this.toggleDrawingButton;
			mapLayer.drawingController.colourButton = this.colourButton;
			mapLayer.drawingController.clearButton = this.clearButton;
			mapLayer.drawingController.drawBrushRect = this.drawBrushRect;
			this.mapLayers.Add(num, mapLayer);
			num++;
		}
		this.mapCursor.SetAsLastSibling();
		this.routesRect.transform.SetAsLastSibling();
		this.pinsRect.transform.SetAsLastSibling();
		this.overlayAll.transform.SetAsLastSibling();
		this.drawBrushRect.SetAsLastSibling();
		this.tooltipOverride.SetAsLastSibling();
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			if (newBuilding.preset.customDrawOnMap)
			{
				GameObject gameObject7 = Object.Instantiate<GameObject>(PrefabControls.Instance.mapBuildingGraphic, this.overlayAll);
				RectTransform component4 = gameObject7.GetComponent<RectTransform>();
				gameObject7.GetComponent<RawImage>().texture = newBuilding.preset.tex;
				component4.eulerAngles = new Vector3(0f, 0f, -newBuilding.buildingModelBase.transform.eulerAngles.y);
				component4.anchoredPosition = this.NodeCoordToMap(CityData.Instance.RealPosToNode(CityData.Instance.CityTileToRealpos(newBuilding.cityTile.cityCoord)));
			}
		}
		foreach (StreetController streetController in CityData.Instance.streetDirectory)
		{
			List<NewTile> list5 = new List<NewTile>();
			List<NewTile> list6 = new List<NewTile>();
			bool flag = true;
			foreach (NewTile newTile in streetController.tiles)
			{
				for (int i = 0; i < 2; i++)
				{
					List<NewTile> list7 = new List<NewTile>();
					List<NewTile> list8 = new List<NewTile>();
					list7.Add(newTile);
					int num4 = 99;
					if (i == 0)
					{
						while (list7.Count > 0)
						{
							if (num4 <= 0)
							{
								break;
							}
							NewTile newTile2 = list7[0];
							foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
							{
								if (Mathf.Abs(vector2Int.y) <= 0)
								{
									Vector3Int vector3Int2 = newTile2.globalTileCoord + new Vector3Int(vector2Int.x, 0, 0);
									NewTile newTile3;
									if (PathFinder.Instance.tileMap.TryGetValue(vector3Int2, ref newTile3) && !newTile3.isObstacle && streetController.tiles.Contains(newTile3) && !list7.Contains(newTile3) && !list8.Contains(newTile3))
									{
										list7.Add(newTile3);
									}
								}
							}
							list7.RemoveAt(0);
							list8.Add(newTile2);
							num4--;
						}
					}
					else if (i == 1)
					{
						while (list7.Count > 0 && num4 > 0)
						{
							NewTile newTile4 = list7[0];
							foreach (Vector2Int vector2Int2 in CityData.Instance.offsetArrayX4)
							{
								if (Mathf.Abs(vector2Int2.x) <= 0)
								{
									Vector3Int vector3Int3 = newTile4.globalTileCoord + new Vector3Int(0, vector2Int2.y, 0);
									NewTile newTile5;
									if (PathFinder.Instance.tileMap.TryGetValue(vector3Int3, ref newTile5) && !newTile5.isObstacle && streetController.tiles.Contains(newTile5) && !list7.Contains(newTile5) && !list8.Contains(newTile5))
									{
										list7.Add(newTile5);
									}
								}
							}
							list7.RemoveAt(0);
							list8.Add(newTile4);
							num4--;
						}
					}
					if (list8.Count > list5.Count)
					{
						list5 = list8;
						list6.Clear();
						if (i == 0)
						{
							flag = true;
						}
						else if (i == 1)
						{
							flag = false;
						}
					}
					else if (list8.Count == list5.Count)
					{
						bool flag2 = true;
						int num5 = 0;
						int num6 = Mathf.RoundToInt((float)list5.Count * 0.25f);
						if (flag)
						{
							using (List<NewTile>.Enumerator enumerator9 = list8.GetEnumerator())
							{
								while (enumerator9.MoveNext())
								{
									NewTile searchTile = enumerator9.Current;
									if (!list5.Exists((NewTile item) => Mathf.Abs(item.globalTileCoord.y - searchTile.globalTileCoord.y) == 1))
									{
										num5++;
									}
									if (num5 >= num6)
									{
										flag2 = false;
										break;
									}
								}
								goto IL_101F;
							}
							goto IL_FBF;
						}
						goto IL_FBF;
						IL_101F:
						if (flag2)
						{
							list6 = list8;
							goto IL_1027;
						}
						goto IL_1027;
						IL_FBF:
						using (List<NewTile>.Enumerator enumerator9 = list8.GetEnumerator())
						{
							while (enumerator9.MoveNext())
							{
								NewTile searchTile = enumerator9.Current;
								if (!list5.Exists((NewTile item) => Mathf.Abs(item.globalTileCoord.x - searchTile.globalTileCoord.x) == 1))
								{
									num5++;
								}
								if (num5 >= num6)
								{
									flag2 = false;
									break;
								}
							}
						}
						goto IL_101F;
					}
					IL_1027:;
				}
			}
			if (list5.Count >= 3)
			{
				GameObject gameObject8 = Object.Instantiate<GameObject>(PrefabControls.Instance.streetName, this.mapLayers[streetController.nodes[0].nodeCoord.z].backgroundContainer);
				RectTransform component5 = gameObject8.GetComponent<RectTransform>();
				gameObject8.GetComponent<TextMeshProUGUI>().text = streetController.name;
				Vector3 vector9 = Vector3.zero;
				foreach (NewTile newTile6 in list5)
				{
					foreach (NewNode newNode4 in newTile6.nodes)
					{
						vector9 += newNode4.nodeCoord;
					}
				}
				foreach (NewTile newTile7 in list6)
				{
					foreach (NewNode newNode5 in newTile7.nodes)
					{
						vector9 += newNode5.nodeCoord;
					}
				}
				vector9 /= (float)((list5.Count + list6.Count) * 9);
				component5.anchoredPosition = this.NodeCoordToMap(vector9);
				if (!flag)
				{
					component5.localEulerAngles = new Vector3(0f, 0f, 90f);
				}
			}
		}
		foreach (DistrictController districtController in HighlanderSingleton<CityDistricts>.Instance.districtDirectory)
		{
			if (districtController.cityTiles.Count > 0)
			{
				Vector2 vector10 = Vector2.zero;
				foreach (CityTile cityTile in districtController.cityTiles)
				{
					vector10 += cityTile.cityCoord;
				}
				vector10 /= (float)districtController.cityTiles.Count;
				float num7 = float.PositiveInfinity;
				CityTile cityTile2 = districtController.cityTiles[0];
				foreach (CityTile cityTile3 in districtController.cityTiles)
				{
					float num8 = Vector2.Distance(cityTile3.cityCoord, vector10);
					if (cityTile2 == null || num8 < num7)
					{
						cityTile2 = cityTile3;
						num7 = num8;
					}
				}
				GameObject gameObject9 = Object.Instantiate<GameObject>(PrefabControls.Instance.districtName, this.mapLayers[0].backgroundContainer);
				RectTransform component6 = gameObject9.GetComponent<RectTransform>();
				gameObject9.GetComponent<TextMeshProUGUI>().text = districtController.name;
				Vector3 coords = CityData.Instance.CityTileToRealpos(cityTile2.cityCoord + new Vector2(0f, 0.5f));
				component6.anchoredPosition = this.NodeCoordToMap(CityData.Instance.RealPosToNode(coords));
			}
		}
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x001B6358 File Offset: 0x001B4558
	public void AddUpdateCall(MapAddressButtonController loc)
	{
		if (loc == null)
		{
			return;
		}
		if (!this.mapUpdateList.Contains(loc))
		{
			this.mapUpdateList.Add(loc);
		}
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x001B637E File Offset: 0x001B457E
	public void AddDuctUpdateCall(MapDuctsButtonController loc)
	{
		if (loc == null)
		{
			return;
		}
		if (!this.ductsUpdateList.Contains(loc))
		{
			this.ductsUpdateList.Add(loc);
		}
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x00002265 File Offset: 0x00000465
	public void OnPinNewEvidence(Evidence ev)
	{
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x00002265 File Offset: 0x00000465
	public void OnUnpinEvidence(Evidence ev)
	{
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x001B63A4 File Offset: 0x001B45A4
	public void PinnedDataKeyChange()
	{
		Game.Log("Pinned data key change", 2);
		foreach (KeyValuePair<InfoWindow, MapPinButtonController> keyValuePair in this.pinnedObjects)
		{
			if (keyValuePair.Key.passedEvidence.preset.useDataKeys)
			{
				if (keyValuePair.Key.evidenceKeys.Contains(Evidence.DataKey.name))
				{
					this.invisiblePins.Remove(keyValuePair.Key);
					keyValuePair.Value.gameObject.SetActive(true);
				}
				else
				{
					this.invisiblePins.Add(keyValuePair.Key);
					keyValuePair.Value.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x001B6478 File Offset: 0x001B4678
	public void AddNewTrackedObject(Transform gameObj, Sprite mapIcon, Vector2 size, Color colour, bool isDynamic, object buttonReference)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.playerMarker, this.overlayAll);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.sizeDelta = size;
		gameObject.GetComponent<Image>().sprite = mapIcon;
		ButtonController component2 = gameObject.GetComponent<ButtonController>();
		component2.SetButtonBaseColour(colour);
		component2.genericReference = buttonReference;
		component2.OnPress += this.PressTracked;
		component2.OnHoverChange += this.HoverTracked;
		if (isDynamic)
		{
			if (!this.dynamicTrackedObjects.ContainsKey(gameObj))
			{
				this.dynamicTrackedObjects.Add(gameObj, new List<RectTransform>());
			}
			this.dynamicTrackedObjects[gameObj].Add(component);
		}
		else
		{
			if (!this.staticTrackedObjects.ContainsKey(gameObj))
			{
				this.staticTrackedObjects.Add(gameObj, new List<RectTransform>());
			}
			this.staticTrackedObjects[gameObj].Add(component);
		}
		this.UpdateTrackedObject(gameObj, component);
	}

	// Token: 0x06001FC6 RID: 8134 RVA: 0x001B655C File Offset: 0x001B475C
	public void PressTracked(ButtonController pressedButton)
	{
		if (pressedButton.genericReference != null)
		{
			Evidence evidence = pressedButton.genericReference as Evidence;
			if (evidence != null)
			{
				if (!Player.Instance.autoTravelActive || Game.Instance.autoTravelPause)
				{
					SessionData.Instance.PauseGame(true, false, true);
				}
				InterfaceController.Instance.SpawnWindow(evidence, Evidence.DataKey.photo, null, "", false, true, default(Vector2), null, null, null, true);
			}
		}
	}

	// Token: 0x06001FC7 RID: 8135 RVA: 0x001B65C8 File Offset: 0x001B47C8
	public void HoverTracked(ButtonController hoveredButton, bool hovered)
	{
		if (hovered && hoveredButton.genericReference != null)
		{
			Evidence evidence = hoveredButton.genericReference as Evidence;
			if (evidence != null && hoveredButton.tooltip != null)
			{
				hoveredButton.tooltip.mainText = evidence.GetNameForDataKey(Evidence.DataKey.photo);
			}
			if (hoveredButton.genericReference as Player != null && hoveredButton.tooltip != null)
			{
				hoveredButton.tooltip.mainText = Player.Instance.GetCitizenName();
			}
		}
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x001B6648 File Offset: 0x001B4848
	public void RemoveTrackedObject(Transform gameObj)
	{
		if (this.dynamicTrackedObjects.ContainsKey(gameObj))
		{
			for (int i = 0; i < this.dynamicTrackedObjects[gameObj].Count; i++)
			{
				ButtonController component = this.dynamicTrackedObjects[gameObj][i].GetComponent<ButtonController>();
				component.OnPress -= this.PressTracked;
				component.OnHoverChange -= this.HoverTracked;
				Object.Destroy(this.dynamicTrackedObjects[gameObj][i]);
			}
			this.dynamicTrackedObjects.Remove(gameObj);
		}
		if (this.staticTrackedObjects.ContainsKey(gameObj))
		{
			for (int j = 0; j < this.staticTrackedObjects[gameObj].Count; j++)
			{
				ButtonController component2 = this.staticTrackedObjects[gameObj][j].GetComponent<ButtonController>();
				component2.OnPress -= this.PressTracked;
				component2.OnHoverChange -= this.HoverTracked;
				Object.Destroy(this.staticTrackedObjects[gameObj][j]);
			}
			this.staticTrackedObjects.Remove(gameObj);
		}
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x001B6768 File Offset: 0x001B4968
	public void UpdateTrackedObject(Transform gameObj, RectTransform mapObj)
	{
		int num = Mathf.FloorToInt(gameObj.position.y / PathFinder.Instance.nodeSize.z);
		mapObj.localEulerAngles = new Vector3(0f, 0f, -gameObj.localRotation.eulerAngles.y);
		mapObj.anchoredPosition = this.RealPosToMap(gameObj.transform.position);
		int num2 = Mathf.Abs(this.load - num);
		if (num2 > 0)
		{
			float num3 = 1f - (float)Mathf.Clamp(num2, 0, 11) / 10f;
			float alpha = Mathf.Lerp(0.1f, 0.5f, num3);
			mapObj.GetComponent<CanvasRenderer>().SetAlpha(alpha);
			return;
		}
		mapObj.GetComponent<CanvasRenderer>().SetAlpha(1f);
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x001B6830 File Offset: 0x001B4A30
	public void CentreOnTrackedObject(Transform gameObj, bool instant = false)
	{
		this.scrollRect.velocity = Vector2.zero;
		RectTransform mapObj;
		if (this.dynamicTrackedObjects.ContainsKey(gameObj))
		{
			mapObj = this.dynamicTrackedObjects[gameObj][0];
		}
		else
		{
			if (!this.staticTrackedObjects.ContainsKey(gameObj))
			{
				return;
			}
			mapObj = this.staticTrackedObjects[gameObj][0];
		}
		int num = Mathf.FloorToInt(gameObj.position.y / PathFinder.Instance.nodeSize.z);
		if (this.load != num)
		{
			this.SetFloorLayer(num, false);
		}
		this.CentreOnObject(mapObj, instant, false);
	}

	// Token: 0x06001FCB RID: 8139 RVA: 0x001B68D4 File Offset: 0x001B4AD4
	public void CentreOnObject(RectTransform mapObj, bool instant = false, bool showPointer = false)
	{
		this.scrollRect.velocity = Vector2.zero;
		this.focusRect = mapObj;
		if (!instant)
		{
			this.forceFocusProgress = 0f;
			this.forceFocusActive = true;
		}
		else
		{
			Vector3 vector = this.contentRect.InverseTransformPoint(this.focusRect.position) + new Vector3(this.contentRect.sizeDelta.x * 0.5f, this.contentRect.sizeDelta.y * 0.5f, 0f);
			Vector2 normalizedPosition;
			normalizedPosition..ctor(Mathf.Clamp01(vector.x / this.contentRect.sizeDelta.x), Mathf.Clamp01(vector.y / this.contentRect.sizeDelta.y));
			this.scrollRect.content.pivot = new Vector2(0.5f, 0.5f);
			this.scrollRect.normalizedPosition = normalizedPosition;
		}
		if (showPointer)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.mapPointer, this.pinsRect);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			gameObject.GetComponent<FlashController>().Flash(3);
			MapController.PointerData pointerData = new MapController.PointerData();
			pointerData.pointerObject = component;
			pointerData.followRect = mapObj;
			pointerData.pointerShow = 1.5f;
			this.pointers.Add(pointerData);
			component.sizeDelta = mapObj.sizeDelta + new Vector2(8f, 8f);
			component.position = mapObj.position - new Vector3(4f, 4f, 0f);
		}
	}

	// Token: 0x06001FCC RID: 8140 RVA: 0x001B6A6C File Offset: 0x001B4C6C
	public void CentreOnNodeCoordinate(Vector3 pathCoord, bool instant = false, bool showPointer = false)
	{
		this.scrollRect.velocity = Vector2.zero;
		Vector2 vector = this.NodeCoordToMap(pathCoord);
		this.focusRect = null;
		this.focusPos = vector;
		int num = Mathf.FloorToInt(pathCoord.z);
		if (this.load != num)
		{
			this.SetFloorLayer(num, false);
		}
		if (!instant)
		{
			this.forceFocusProgress = 0f;
			this.forceFocusActive = true;
		}
		else
		{
			this.contentRect.anchoredPosition = this.ClampMapScrollPosition(this.focusPos);
		}
		if (showPointer)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.mapPointer, this.pinsRect);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			gameObject.GetComponent<FlashController>().Flash(3);
			MapController.PointerData pointerData = new MapController.PointerData();
			pointerData.pointerObject = component;
			pointerData.followPos = vector;
			pointerData.pointerShow = 1.5f;
			this.pointers.Add(pointerData);
			component.sizeDelta = new Vector2(32f, 32f);
			component.position = vector;
		}
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x001B6B60 File Offset: 0x001B4D60
	public Vector2 ClampMapScrollPosition(Vector2 focusPos)
	{
		float num = this.contentRect.sizeDelta.x * 0.5f - this.viewport.rect.width * 0.5f;
		float num2 = this.contentRect.sizeDelta.y * 0.5f - this.viewport.rect.height * 0.5f;
		string text = "Focus pos: ";
		Vector2 vector = focusPos;
		Game.Log(text + vector.ToString(), 2);
		focusPos..ctor(Mathf.Clamp(-focusPos.x, -num, num), Mathf.Clamp(-focusPos.y, -num2, num2));
		string text2 = "Clamped: ";
		vector = focusPos;
		Game.Log(text2 + vector.ToString(), 2);
		return focusPos;
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x001B6C34 File Offset: 0x001B4E34
	public void SetFloorLayer(int newFloor, bool forceLoad = false)
	{
		newFloor = Mathf.Clamp(newFloor, (int)CityData.Instance.floorRange.x, (int)CityData.Instance.floorRange.y);
		if (newFloor == this.load && !forceLoad)
		{
			return;
		}
		this.load = newFloor;
		int num = (int)CityData.Instance.floorRange.x;
		while ((float)num <= CityData.Instance.floorRange.y)
		{
			if (num == this.load)
			{
				if (!this.mapLayers[num].canvas.gameObject.activeSelf)
				{
					this.mapLayers[num].canvas.gameObject.SetActive(true);
				}
			}
			else
			{
				if (this.mapLayers[num].canvas.gameObject.activeSelf)
				{
					this.mapLayers[num].canvas.gameObject.SetActive(false);
				}
				this.mapLayers[num].drawingController.SetDrawingActive(false);
			}
			num++;
		}
		if (this.drawingMode)
		{
			this.mapLayers[this.load].drawingController.SetDrawingActive(true);
		}
		if (!this.mapLayers.ContainsKey(this.load))
		{
			Game.LogError(string.Concat(new string[]
			{
				"Map layer has no entry for ",
				this.load.ToString(),
				" (it contains ",
				this.mapLayers.Count.ToString(),
				" entries)"
			}), 2);
		}
		this.drawBrushRect.SetParent(this.mapLayers[this.load].drawingController.container);
		this.drawBrushRect.SetAsLastSibling();
		if (this.fzc != null)
		{
			if (this.load == 0)
			{
				this.fzc.floorText.text = "G";
			}
			else
			{
				this.fzc.floorText.text = this.load.ToString();
			}
			this.fzc.floorSlider.SetValueWithoutNotify(this.load);
			foreach (KeyValuePair<Transform, List<RectTransform>> keyValuePair in this.dynamicTrackedObjects)
			{
				foreach (RectTransform mapObj in keyValuePair.Value)
				{
					this.UpdateTrackedObject(keyValuePair.Key, mapObj);
				}
			}
			foreach (KeyValuePair<Transform, List<RectTransform>> keyValuePair2 in this.staticTrackedObjects)
			{
				foreach (RectTransform mapObj2 in keyValuePair2.Value)
				{
					this.UpdateTrackedObject(keyValuePair2.Key, mapObj2);
				}
			}
			if (this.playerRoute != null)
			{
				this.playerRoute.UpdateDrawnRoute();
			}
		}
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x001B6F80 File Offset: 0x001B5180
	public Vector2 NodeCoordToMap(Vector3 pos)
	{
		return new Vector2((pos.x + 0.5f) * this.nodePositionMultiplier + this.positionBuffer, (pos.y + 0.5f) * this.nodePositionMultiplier + this.positionBuffer);
	}

	// Token: 0x06001FD0 RID: 8144 RVA: 0x001B6FBC File Offset: 0x001B51BC
	public Vector2 RealPosToMap(Vector3 coords)
	{
		return new Vector2((coords.x + CityData.Instance.citySize.x * 0.5f * CityControls.Instance.cityTileSize.x) * this.realPositionMultiplier + this.positionBuffer, (coords.z + CityData.Instance.citySize.y * 0.5f * CityControls.Instance.cityTileSize.y) * this.realPositionMultiplier + this.positionBuffer);
	}

	// Token: 0x06001FD1 RID: 8145 RVA: 0x001B7044 File Offset: 0x001B5244
	public Vector2 MapToNode(Vector2 coords)
	{
		return new Vector2(Mathf.Clamp((float)Mathf.FloorToInt(coords.x / this.nodePositionMultiplier), 0f, PathFinder.Instance.nodeCitySize.x - 1f), Mathf.Clamp((float)Mathf.FloorToInt(coords.y / this.nodePositionMultiplier), 0f, PathFinder.Instance.nodeCitySize.y - 1f));
	}

	// Token: 0x06001FD2 RID: 8146 RVA: 0x001B70BC File Offset: 0x001B52BC
	private void Update()
	{
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		if (!MainMenuController.Instance.mainMenuActive)
		{
			if (this.mapUpdateList.Count > 0)
			{
				this.mapUpdateList[0].UpdateMapImageEndOfFrame();
				this.mapUpdateList.RemoveAt(0);
			}
			if (this.ductsUpdateList.Count > 0)
			{
				if (this.ductsUpdateList[0] != null)
				{
					this.ductsUpdateList[0].UpdateMapImageEndOfFrame();
				}
				this.ductsUpdateList.RemoveAt(0);
			}
			foreach (KeyValuePair<Transform, List<RectTransform>> keyValuePair in this.dynamicTrackedObjects)
			{
				foreach (RectTransform mapObj in keyValuePair.Value)
				{
					this.UpdateTrackedObject(keyValuePair.Key, mapObj);
				}
			}
			if (this.displayFirstPerson)
			{
				if (this.mapCloseButton != null)
				{
					this.mapCloseButton.gameObject.SetActive(false);
				}
				if (!InputController.Instance.player.GetButton("Map"))
				{
					this.CloseMap(true);
				}
				this.CentreOnTrackedObject(Player.Instance.transform, true);
			}
			else if (this.mapCloseButton != null && !this.mapCloseButton.gameObject.activeSelf)
			{
				this.mapCloseButton.gameObject.SetActive(true);
			}
			for (int i = 0; i < this.pointers.Count; i++)
			{
				MapController.PointerData pointerData = this.pointers[i];
				if (pointerData.pointerShow <= 0f)
				{
					Object.Destroy(pointerData.pointerObject.gameObject);
					this.pointers.RemoveAt(i);
					i--;
				}
				else
				{
					if (pointerData.followRect != null)
					{
						pointerData.pointerObject.sizeDelta = new Vector2(Mathf.Max(pointerData.followRect.sizeDelta.x + 8f, 32f), Mathf.Max(pointerData.followRect.sizeDelta.y + 8f, 32f));
						pointerData.pointerObject.position = pointerData.followRect.position;
					}
					else
					{
						pointerData.pointerObject.sizeDelta = new Vector2(32f, 32f);
						pointerData.pointerObject.localPosition = pointerData.followPos;
					}
					pointerData.pointerShow -= Time.fixedDeltaTime;
				}
			}
			if (this.forceFocusActive)
			{
				Vector3 vector = Vector3.zero;
				if (this.focusRect != null)
				{
					vector = this.contentRect.InverseTransformPoint(this.focusRect.position) + new Vector3(this.contentRect.sizeDelta.x * 0.5f, this.contentRect.sizeDelta.y * 0.5f, 0f);
				}
				else
				{
					vector = this.focusPos;
				}
				Vector2 vector2;
				vector2..ctor(Mathf.Clamp01(vector.x / this.contentRect.sizeDelta.x), Mathf.Clamp01(vector.y / this.contentRect.sizeDelta.y));
				this.forceFocusProgress += Time.deltaTime * this.focusSpeed;
				this.scrollRect.content.pivot = new Vector2(0.5f, 0.5f);
				this.scrollRect.normalizedPosition = Vector2.Lerp(this.scrollRect.normalizedPosition, vector2, this.forceFocusProgress);
				if (this.forceFocusProgress >= 1f)
				{
					this.forceFocusProgress = 0f;
					this.forceFocusActive = false;
				}
			}
			if (!InputController.Instance.mouseInputMode && CasePanelController.Instance.mapScroll.controlEnabled)
			{
				Vector3[] array = new Vector3[4];
				(this.scrollRect.transform as RectTransform).GetWorldCorners(array);
				Vector3 zero = Vector3.zero;
				foreach (Vector3 vector3 in array)
				{
					zero.x += vector3.x;
					zero.y += vector3.y;
				}
				zero.x /= 4f;
				zero.y /= 4f;
				MapAddressButtonController mapAddressButtonController = null;
				float num = float.PositiveInfinity;
				foreach (MapAddressButtonController mapAddressButtonController2 in this.buttons)
				{
					if (mapAddressButtonController2.gameObject.activeInHierarchy)
					{
						float num2 = Vector3.Distance(mapAddressButtonController2.gameObject.transform.position, zero);
						if (num2 < num)
						{
							mapAddressButtonController = mapAddressButtonController2;
							num = num2;
						}
					}
				}
				foreach (MapAddressButtonController mapAddressButtonController3 in this.buttons)
				{
					if (mapAddressButtonController3.gameObject.activeInHierarchy && mapAddressButtonController3 == mapAddressButtonController)
					{
						mapAddressButtonController3.OnHoverStart();
					}
					else
					{
						mapAddressButtonController3.OnHoverEnd();
					}
				}
				if (mapAddressButtonController != null && InputController.Instance.player.GetButtonDown("Select"))
				{
					mapAddressButtonController.OnLeftDoubleClick();
				}
			}
			if (!this.drawingMode && this.mapContextMenu != null && this.mapContextMenu.spawnedMenu == null)
			{
				Vector2 zero2 = Vector2.zero;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.overlayAll, Input.mousePosition, null, ref zero2);
				this.mapCursor.localPosition = zero2;
				Vector2 vector4 = this.MapToNode(this.mapCursor.anchoredPosition);
				Vector3Int vector3Int;
				vector3Int..ctor(Mathf.RoundToInt(vector4.x), Mathf.RoundToInt(vector4.y), this.load);
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref this.mapCursorNode))
				{
					if (this.mapCursorNode != this.cursorNodeChange)
					{
						this.cursorPos = this.NodeCoordToMap(vector3Int);
						if (this.mapCursorNode.gameLocation == null || this.mapCursorNode.gameLocation.evidenceEntry == null)
						{
							if (!this.mapContextMenu.disabledItems.Contains("OpenEvidence"))
							{
								this.mapContextMenu.disabledItems.Add("OpenEvidence");
							}
							if (!this.mapContextMenu.disabledItems.Contains("PlotRoute"))
							{
								this.mapContextMenu.disabledItems.Add("PlotRoute");
							}
						}
						else
						{
							this.mapContextMenu.disabledItems.Remove("OpenEvidence");
							if (this.mapCursorNode.isObstacle)
							{
								if (!this.mapContextMenu.disabledItems.Contains("PlotRoute"))
								{
									this.mapContextMenu.disabledItems.Add("PlotRoute");
								}
							}
							else
							{
								this.mapContextMenu.disabledItems.Remove("PlotRoute");
							}
						}
						if (!this.drawingMode)
						{
							this.mapCursor.gameObject.SetActive(true);
						}
						this.cursorNodeChange = this.mapCursorNode;
					}
					this.mapCursor.anchoredPosition = this.cursorPos;
					return;
				}
				this.mapCursorNode = null;
				this.cursorNodeChange = null;
				this.mapCursor.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001FD3 RID: 8147 RVA: 0x001B789C File Offset: 0x001B5A9C
	private void OnEnable()
	{
		this.colourButton.OnChangeColour += this.OnChangeDrawingColour;
		this.controllerSelectMapButton.OnHoverChange += this.ControllerMapHoverChange;
		CasePanelController.Instance.OnPinEvidence += this.OnPinNewEvidence;
		CasePanelController.Instance.OnUnpinEvidence += this.OnUnpinEvidence;
		this.ControllerMapHoverChange(null, false);
		if (this.drag != null)
		{
			this.drag.OnDragged += this.UpdateSize;
		}
		if (this.playerRoute != null)
		{
			this.playerRoute.UpdateDrawnRoute();
		}
	}

	// Token: 0x06001FD4 RID: 8148 RVA: 0x001B7944 File Offset: 0x001B5B44
	private void OnDisable()
	{
		this.colourButton.OnChangeColour -= this.OnChangeDrawingColour;
		this.controllerSelectMapButton.OnHoverChange -= this.ControllerMapHoverChange;
		CasePanelController.Instance.OnPinEvidence -= this.OnPinNewEvidence;
		CasePanelController.Instance.OnUnpinEvidence -= this.OnUnpinEvidence;
		if (this.drag != null)
		{
			this.drag.OnDragged -= this.UpdateSize;
		}
	}

	// Token: 0x06001FD5 RID: 8149 RVA: 0x001B79D0 File Offset: 0x001B5BD0
	private void UpdateSize()
	{
		this.savedSize = this.drag.parentRect.sizeDelta.x;
	}

	// Token: 0x06001FD6 RID: 8150 RVA: 0x001B79F0 File Offset: 0x001B5BF0
	public void OpenMap(bool firstPerson, bool playSound = true)
	{
		this.displayFirstPerson = firstPerson;
		InterfaceController.Instance.minimapCanvas.gameObject.SetActive(true);
		base.enabled = true;
		base.StopCoroutine("Open");
		base.StopCoroutine("Close");
		base.StartCoroutine("Open");
		SessionData.Instance.TutorialTrigger("map", false);
		if (playSound)
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.mapSlideIn, null, 1f);
		}
		if (InterfaceController.Instance.minimapCanvas.gameObject.activeSelf && !this.displayFirstPerson)
		{
			InterfaceController.Instance.notebookButton.RefreshAutomaticNavigation();
			InterfaceController.Instance.upgradesButton.RefreshAutomaticNavigation();
			InterfaceController.Instance.mapButton.RefreshAutomaticNavigation();
			CasePanelController.Instance.selectNoCaseButton.RefreshAutomaticNavigation();
			CasePanelController.Instance.stickNoteButton.RefreshAutomaticNavigation();
		}
	}

	// Token: 0x06001FD7 RID: 8151 RVA: 0x001B7AD9 File Offset: 0x001B5CD9
	private IEnumerator Open()
	{
		if (!this.displayFirstPerson)
		{
			this.fzc.gameObject.SetActive(true);
		}
		else
		{
			this.fzc.gameObject.SetActive(false);
		}
		this.CentreOnTrackedObject(Player.Instance.transform, true);
		while (this.openProgress < 1f)
		{
			this.openProgress += Time.deltaTime * 20f;
			this.openProgress = Mathf.Clamp01(this.openProgress);
			this.drag.SetSize(this.openProgress * this.savedSize);
			InterfaceController.Instance.minimapCanvasGroup.alpha = this.openProgress;
			yield return null;
		}
		this.drag.gameObject.SetActive(true);
		this.scrollRect.enabled = true;
		yield break;
	}

	// Token: 0x06001FD8 RID: 8152 RVA: 0x001B7AE8 File Offset: 0x001B5CE8
	public void CloseMap(bool playSound = true)
	{
		base.StopCoroutine("Open");
		base.StopCoroutine("Close");
		base.StartCoroutine("Close");
		if (playSound)
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.mapSlideOut, null, 1f);
		}
		Navigation navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation2 = navigation;
		navigation2.selectOnLeft = InterfaceController.Instance.notebookButton.button.FindSelectableOnLeft();
		navigation2.selectOnRight = InterfaceController.Instance.notebookButton.button.FindSelectableOnRight();
		InterfaceController.Instance.notebookButton.button.navigation = navigation2;
		navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation3 = navigation;
		navigation3.selectOnLeft = InterfaceController.Instance.upgradesButton.button.FindSelectableOnLeft();
		navigation3.selectOnRight = InterfaceController.Instance.upgradesButton.button.FindSelectableOnRight();
		InterfaceController.Instance.upgradesButton.button.navigation = navigation3;
		navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation4 = navigation;
		navigation4.selectOnLeft = InterfaceController.Instance.mapButton.button.FindSelectableOnLeft();
		navigation4.selectOnRight = InterfaceController.Instance.mapButton.button.FindSelectableOnRight();
		InterfaceController.Instance.mapButton.button.navigation = navigation4;
		navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation5 = navigation;
		navigation5.selectOnLeft = CasePanelController.Instance.selectNoCaseButton.button.FindSelectableOnLeft();
		navigation5.selectOnRight = CasePanelController.Instance.selectNoCaseButton.button.FindSelectableOnRight();
		CasePanelController.Instance.selectNoCaseButton.button.navigation = navigation5;
		navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation6 = navigation;
		navigation6.selectOnLeft = CasePanelController.Instance.stickNoteButton.button.FindSelectableOnLeft();
		navigation6.selectOnRight = CasePanelController.Instance.stickNoteButton.button.FindSelectableOnRight();
		CasePanelController.Instance.stickNoteButton.button.navigation = navigation6;
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x001B7D0D File Offset: 0x001B5F0D
	private IEnumerator Close()
	{
		this.scrollRect.enabled = false;
		if (this.displayFirstPerson)
		{
			this.displayFirstPerson = false;
		}
		this.drag.gameObject.SetActive(false);
		while (this.openProgress > 0f)
		{
			this.openProgress -= Time.deltaTime * 20f;
			this.openProgress = Mathf.Clamp01(this.openProgress);
			this.drag.SetSize(this.openProgress * this.savedSize);
			InterfaceController.Instance.minimapCanvasGroup.alpha = this.openProgress;
			yield return null;
		}
		base.enabled = false;
		InterfaceController.Instance.minimapCanvas.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x001B7D1C File Offset: 0x001B5F1C
	public void LocateEvidenceOnMap(Evidence ev)
	{
		EvidenceLocation loc = ev as EvidenceLocation;
		EvidenceBuilding evidenceBuilding = ev as EvidenceBuilding;
		if (loc != null)
		{
			if (!InterfaceController.Instance.showDesktopMap)
			{
				InterfaceController.Instance.SetShowDesktopMap(true, true);
			}
			if (loc.locationController.nodes.Count > 0)
			{
				NewNode newNode = loc.locationController.nodes[0];
				this.SetFloorLayer(newNode.nodeCoord.z, false);
				MapAddressButtonController mapAddressButtonController = this.buttons.Find((MapAddressButtonController item) => item.gameLocation.evidenceEntry == loc);
				if (mapAddressButtonController != null)
				{
					this.CentreOnObject(mapAddressButtonController.rect, false, true);
					return;
				}
			}
		}
		else if (evidenceBuilding != null)
		{
			if (!InterfaceController.Instance.showDesktopMap)
			{
				InterfaceController.Instance.SetShowDesktopMap(true, true);
			}
			using (List<NewAddress>.Enumerator enumerator = evidenceBuilding.building.lobbies.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewAddress lob = enumerator.Current;
					if (lob.entrances.Count > 0)
					{
						MapAddressButtonController mapAddressButtonController2 = this.buttons.Find((MapAddressButtonController item) => item.gameLocation == lob);
						if (mapAddressButtonController2 != null)
						{
							this.CentreOnObject(mapAddressButtonController2.rect, false, true);
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x001B7E98 File Offset: 0x001B6098
	public void LocateRoomOnMap(NewRoom room)
	{
		if (room != null)
		{
			if (!InterfaceController.Instance.showDesktopMap)
			{
				InterfaceController.Instance.SetShowDesktopMap(true, true);
			}
			NewNode newNode = Enumerable.FirstOrDefault<NewNode>(room.nodes);
			this.SetFloorLayer(newNode.nodeCoord.z, false);
			Vector3 vector = Vector3.zero;
			foreach (NewNode newNode2 in room.nodes)
			{
				vector += newNode2.nodeCoord;
			}
			vector /= (float)room.nodes.Count;
			this.CentreOnNodeCoordinate(vector, false, true);
		}
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x001B7F5C File Offset: 0x001B615C
	public void PlotPlayerRoute(Evidence ev)
	{
		Game.Log("Interface: Plot route from evidence...", 2);
		EvidenceLocation evidenceLocation = ev as EvidenceLocation;
		EvidenceBuilding evidenceBuilding = ev as EvidenceBuilding;
		if (evidenceLocation != null)
		{
			Game.Log("Interface: Plotting route to location...", 2);
			this.PlotPlayerRoute(evidenceLocation.locationController);
			return;
		}
		if (evidenceBuilding != null)
		{
			bool flag = false;
			foreach (NewAddress newAddress in evidenceBuilding.building.lobbies)
			{
				if (newAddress.entrances.Count > 0)
				{
					Game.Log("Interface: Plotting route to building...", 2);
					this.PlotPlayerRoute(newAddress);
					flag = true;
					break;
				}
			}
			if (!flag && evidenceBuilding.building.mainEntrance != null)
			{
				this.PlotPlayerRoute(evidenceBuilding.building.mainEntrance.node, false, null);
				flag = true;
			}
		}
	}

	// Token: 0x06001FDD RID: 8157 RVA: 0x001B803C File Offset: 0x001B623C
	public void PlotPlayerRoute(NewGameLocation loc)
	{
		if (loc != null)
		{
			if (loc.thisAsAddress != null)
			{
				this.PlotPlayerRoute(loc.thisAsAddress);
				return;
			}
			if (loc.thisAsStreet != null)
			{
				this.PlotPlayerRoute(loc.thisAsStreet);
			}
		}
	}

	// Token: 0x06001FDE RID: 8158 RVA: 0x001B807C File Offset: 0x001B627C
	public void PlotPlayerRoute(NewAddress loc)
	{
		if (loc != null)
		{
			Game.Log("Interface: Plotting route to address: " + loc.name, 2);
			this.PlotPlayerRoute(loc.GetDestinationNode(), true, loc);
		}
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x001B80AB File Offset: 0x001B62AB
	public void PlotPlayerRoute(StreetController loc)
	{
		if (loc != null)
		{
			Game.Log("Interface: Plotting route to street: " + loc.name, 2);
			this.PlotPlayerRoute(loc.GetDestinationNode(), false, null);
		}
	}

	// Token: 0x06001FE0 RID: 8160 RVA: 0x001B80DC File Offset: 0x001B62DC
	public void PlotPlayerRoute(NewNode loc, bool nodeSpecific, NewGameLocation destinationTextOverride = null)
	{
		if (loc != null)
		{
			if (this.playerRoute != null)
			{
				if (this.playerRoute.end != loc)
				{
					this.playerRoute.Remove();
				}
				else
				{
					if (this.playerRoute.UpdatePathData(this.playerRoute.start))
					{
						return;
					}
					Game.Log("Player: Unable to calculate a valid route; removing it...", 2);
					this.playerRoute.Remove();
				}
			}
			this.SetFloorLayer(loc.nodeCoord.z, false);
			if (Game.Instance.routeTeleport)
			{
				Player.Instance.Teleport(loc, null, true, false);
			}
			else
			{
				string[] array = new string[7];
				array[0] = "Interface: Adding new player route: ";
				int num = 1;
				Vector3 position = Player.Instance.currentNode.position;
				array[num] = position.ToString();
				array[2] = " -> ";
				int num2 = 3;
				position = loc.position;
				array[num2] = position.ToString();
				array[4] = " (node specific: ";
				array[5] = nodeSpecific.ToString();
				array[6] = ")";
				Game.Log(string.Concat(array), 2);
				MapController.MapRoute mapRoute = new MapController.MapRoute(Player.Instance.currentNode, loc, Player.Instance, nodeSpecific, destinationTextOverride);
				this.plotRouteActiveJuice.gameObject.SetActive(true);
				this.plotRouteActiveJuice.Pulsate(true, false);
				this.plotRouteButton.SetInteractable(true);
				this.plotRouteButton.tooltip.detailText = mapRoute.GetDestinationText();
				if (MapController.Instance.autoTravelButton != null)
				{
					this.autoTravelButton.SetInteractable(true);
				}
			}
			if (this.mapContextMenu.disabledItems.Contains("CancelRoute"))
			{
				this.mapContextMenu.disabledItems.Remove("CancelRoute");
			}
			InterfaceControls.Instance.plottedRouteText.gameObject.SetActive(true);
			string text = loc.gameLocation.name;
			if (MapController.Instance.playerRoute != null)
			{
				text = MapController.Instance.playerRoute.GetDestinationText();
			}
			if (loc.gameLocation.building != null && loc.gameLocation.floor != null && loc.gameLocation.floor.floor != 0)
			{
				if (loc.gameLocation.floor.floor < 0)
				{
					text = string.Concat(new string[]
					{
						" (",
						Strings.Get("names.rooms", "basement", Strings.Casing.firstLetterCaptial, false, false, false, null),
						" ",
						loc.gameLocation.floor.floor.ToString(),
						")"
					});
				}
				else if (loc.gameLocation.floor.floor > 0)
				{
					text = string.Concat(new string[]
					{
						text,
						" (",
						Toolbox.Instance.GetNumbericalStringReference(Mathf.Abs(loc.gameLocation.floor.floor)),
						" ",
						Strings.Get("evidence.generic", "floor", Strings.Casing.asIs, false, false, false, null),
						")"
					});
				}
			}
			InterfaceControls.Instance.plottedRouteText.text = text;
			if (this.OnPlotRoute != null)
			{
				this.OnPlotRoute();
			}
		}
	}

	// Token: 0x06001FE1 RID: 8161 RVA: 0x001B8406 File Offset: 0x001B6606
	public void RemovePlayerRoute()
	{
		if (this.playerRoute != null)
		{
			this.playerRoute.Remove();
		}
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x001B841C File Offset: 0x001B661C
	private void SetTimelineCitizenTransparency(int citizenFloor, RectTransform objectRect)
	{
		int num = Mathf.Abs(this.load - citizenFloor);
		if (num > 0)
		{
			float num2 = 1f - (float)Mathf.Clamp(num, 0, 11) / 10f;
			float alpha = Mathf.Lerp(0.05f, 0.2f, num2);
			objectRect.gameObject.GetComponent<CanvasRenderer>().SetAlpha(alpha);
			return;
		}
		objectRect.gameObject.GetComponent<CanvasRenderer>().SetAlpha(1f);
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x001B848A File Offset: 0x001B668A
	private Vector3 FindWorldPoint(PathFinder.PathData pathData, float percentAlong, out int lastPointIndex, out float distanceSinceLastPoint, out int nextPointIndex)
	{
		lastPointIndex = 0;
		distanceSinceLastPoint = 0f;
		nextPointIndex = 0;
		return Vector3.zero;
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x001B84A0 File Offset: 0x001B66A0
	public void DisplayDirectionArrow(bool val)
	{
		this.displayDirectionArrow = val;
		if (Game.Instance.enableDirectionalArrow)
		{
			this.directionalArrowContainer.SetActive(this.displayDirectionArrow);
			return;
		}
		this.directionalArrowContainer.SetActive(false);
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x001B84D4 File Offset: 0x001B66D4
	public void ResetThis()
	{
		this.load = -1;
		this.displayPlayerCharacter = true;
		this.displayFirstPerson = false;
		while (this.pointers.Count > 0)
		{
			Object.Destroy(this.pointers[0].pointerObject.gameObject);
			Object.Destroy(this.pointers[0].followRect.gameObject);
			this.pointers.RemoveAt(0);
		}
		this.pointers = new List<MapController.PointerData>();
		foreach (KeyValuePair<Transform, List<RectTransform>> keyValuePair in this.dynamicTrackedObjects)
		{
			while (keyValuePair.Value.Count > 0)
			{
				keyValuePair.Value.RemoveAt(0);
			}
		}
		foreach (KeyValuePair<Transform, List<RectTransform>> keyValuePair2 in this.staticTrackedObjects)
		{
			while (keyValuePair2.Value.Count > 0)
			{
				keyValuePair2.Value.RemoveAt(0);
			}
		}
		this.dynamicTrackedObjects = new Dictionary<Transform, List<RectTransform>>();
		this.staticTrackedObjects = new Dictionary<Transform, List<RectTransform>>();
		foreach (KeyValuePair<InfoWindow, MapPinButtonController> keyValuePair3 in this.pinnedObjects)
		{
			Object.Destroy(keyValuePair3.Value.gameObject);
		}
		this.pinnedObjects = new Dictionary<InfoWindow, MapPinButtonController>();
		this.invisiblePins = new List<InfoWindow>();
		this.forceFocusActive = false;
		this.forceFocusProgress = 0f;
		this.focusPos = Vector2.zero;
		if (this.playerRoute != null)
		{
			this.playerRoute.Remove();
		}
		foreach (KeyValuePair<int, MapController.MapLayer> keyValuePair4 in this.mapLayers)
		{
			Object.Destroy(keyValuePair4.Value.canvas.gameObject);
		}
		this.mapLayers = new Dictionary<int, MapController.MapLayer>();
		while (this.buttons.Count > 0)
		{
			Object.Destroy(this.buttons[0]);
		}
		this.buttons = new List<MapAddressButtonController>();
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x001B873C File Offset: 0x001B693C
	public void ToggleDrawingMode()
	{
		this.drawingMode = !this.drawingMode;
		if (this.drawingMode)
		{
			this.toggleDrawingButton.background.color = InterfaceControls.Instance.selectionColour;
			if (this.load > -1)
			{
				this.mapLayers[this.load].drawingController.SetDrawingActive(true);
				this.mapCursor.gameObject.SetActive(false);
				return;
			}
		}
		else
		{
			this.toggleDrawingButton.background.color = InterfaceControls.Instance.nonSelectionColour;
			foreach (KeyValuePair<int, MapController.MapLayer> keyValuePair in this.mapLayers)
			{
				keyValuePair.Value.drawingController.SetDrawingActive(false);
			}
			this.mapCursor.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x001B8830 File Offset: 0x001B6A30
	public void OnChangeDrawingColour()
	{
		foreach (KeyValuePair<int, MapController.MapLayer> keyValuePair in this.mapLayers)
		{
			keyValuePair.Value.drawingController.SetBrushColour(this.colourButton.selectedColour);
		}
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x001B8898 File Offset: 0x001B6A98
	public void ToggleEraser()
	{
		this.eraseMode = !this.eraseMode;
		foreach (KeyValuePair<int, MapController.MapLayer> keyValuePair in this.mapLayers)
		{
			keyValuePair.Value.drawingController.SetEraserMode(this.eraseMode);
		}
		if (this.eraseMode)
		{
			this.eraserButton.background.color = InterfaceControls.Instance.selectionColour;
			return;
		}
		this.eraserButton.background.color = InterfaceControls.Instance.nonSelectionColour;
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x001B8948 File Offset: 0x001B6B48
	public void ClearDrawing()
	{
		if (this.load > -1)
		{
			this.mapLayers[this.load].drawingController.ResetDrawingTexture();
		}
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x001B8970 File Offset: 0x001B6B70
	public void OpenEvidence()
	{
		if (this.mapCursorNode != null && this.mapCursorNode.gameLocation != null)
		{
			InterfaceController.Instance.SpawnWindow(this.mapCursorNode.gameLocation.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
		}
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x001B89C9 File Offset: 0x001B6BC9
	public void PlotRoute()
	{
		if (this.mapCursorNode != null && this.mapCursorNode.gameLocation != null)
		{
			this.PlotPlayerRoute(this.mapCursorNode, true, null);
		}
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x001B89F4 File Offset: 0x001B6BF4
	public void AutoTravel()
	{
		if (this.mapCursorNode != null && this.mapCursorNode.gameLocation != null)
		{
			Player.Instance.ExecuteAutoTravel(this.mapCursorNode, false);
		}
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x001B8A22 File Offset: 0x001B6C22
	public void CancelRoute()
	{
		this.RemovePlayerRoute();
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x001B8A2C File Offset: 0x001B6C2C
	public void DebugAccess()
	{
		while (this.spawnedDebugComponents.Count > 0)
		{
			Object.Destroy(this.spawnedDebugComponents[0]);
			this.spawnedDebugComponents.RemoveAt(0);
		}
		if (this.mapCursorNode == null || this.mapCursorNode.accessToOtherNodes == null)
		{
			return;
		}
		foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in this.mapCursorNode.accessToOtherNodes)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.debugAccess, MapController.Instance.mapLayers[keyValuePair.Key.nodeCoord.z].baseContainer);
			this.spawnedDebugComponents.Add(gameObject);
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = keyValuePair.Value.accessType.ToString();
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchoredPosition = this.NodeCoordToMap(keyValuePair.Key.nodeCoord);
			component.anchoredPosition = new Vector2(component.anchoredPosition.x - MapController.Instance.nodePositionMultiplier * 1.5f, component.anchoredPosition.y - MapController.Instance.nodePositionMultiplier * 1.5f);
			Image component2 = gameObject.GetComponent<Image>();
			if (keyValuePair.Value.walkingAccess)
			{
				component2.color = Color.green;
			}
			else
			{
				component2.color = Color.red;
			}
		}
	}

	// Token: 0x04002995 RID: 10645
	[Header("Components")]
	public RectTransform contentRect;

	// Token: 0x04002996 RID: 10646
	public RectTransform paperRect;

	// Token: 0x04002997 RID: 10647
	public RectTransform viewport;

	// Token: 0x04002998 RID: 10648
	public ZoomContent zoomController;

	// Token: 0x04002999 RID: 10649
	public DragCoverage drag;

	// Token: 0x0400299A RID: 10650
	public CustomScrollRect scrollRect;

	// Token: 0x0400299B RID: 10651
	public RectTransform controlsRect;

	// Token: 0x0400299C RID: 10652
	public Canvas contentCanvas;

	// Token: 0x0400299D RID: 10653
	public ButtonController mapCloseButton;

	// Token: 0x0400299E RID: 10654
	public RectTransform mapCursor;

	// Token: 0x0400299F RID: 10655
	public ContextMenuController mapContextMenu;

	// Token: 0x040029A0 RID: 10656
	public TextMeshProUGUI districtMapName;

	// Token: 0x040029A1 RID: 10657
	public ButtonController centreOnPlayerButton;

	// Token: 0x040029A2 RID: 10658
	public ButtonController controllerSelectMapButton;

	// Token: 0x040029A3 RID: 10659
	public ButtonController plotRouteButton;

	// Token: 0x040029A4 RID: 10660
	public ButtonController autoTravelButton;

	// Token: 0x040029A5 RID: 10661
	public JuiceController plotRouteActiveJuice;

	// Token: 0x040029A6 RID: 10662
	public JuiceController autoTravelActiveJuice;

	// Token: 0x040029A7 RID: 10663
	public Sprite autoTravelIcon;

	// Token: 0x040029A8 RID: 10664
	public Sprite fastTravelIcon;

	// Token: 0x040029A9 RID: 10665
	[Header("Drawing")]
	public bool drawingMode;

	// Token: 0x040029AA RID: 10666
	public bool eraseMode;

	// Token: 0x040029AB RID: 10667
	public Color drawingColour = Color.white;

	// Token: 0x040029AC RID: 10668
	public RectTransform drawBrushRect;

	// Token: 0x040029AD RID: 10669
	public ButtonController toggleDrawingButton;

	// Token: 0x040029AE RID: 10670
	public ColourSelectorButtonController colourButton;

	// Token: 0x040029AF RID: 10671
	public ButtonController eraserButton;

	// Token: 0x040029B0 RID: 10672
	public ButtonController clearButton;

	// Token: 0x040029B1 RID: 10673
	[Header("State")]
	public int load = -1;

	// Token: 0x040029B2 RID: 10674
	public bool displayPlayerCharacter = true;

	// Token: 0x040029B3 RID: 10675
	public bool displayFirstPerson;

	// Token: 0x040029B4 RID: 10676
	public RectTransform playerCharacterRect;

	// Token: 0x040029B5 RID: 10677
	public NewNode mapCursorNode;

	// Token: 0x040029B6 RID: 10678
	private NewNode cursorNodeChange;

	// Token: 0x040029B7 RID: 10679
	public Vector2 cursorPos;

	// Token: 0x040029B8 RID: 10680
	public List<MapAddressButtonController> mapUpdateList = new List<MapAddressButtonController>();

	// Token: 0x040029B9 RID: 10681
	public List<MapDuctsButtonController> ductsUpdateList = new List<MapDuctsButtonController>();

	// Token: 0x040029BA RID: 10682
	[Header("Map Overlays")]
	public RectTransform routesRect;

	// Token: 0x040029BB RID: 10683
	public RectTransform linesRouteRect;

	// Token: 0x040029BC RID: 10684
	public RectTransform citizensRouteRect;

	// Token: 0x040029BD RID: 10685
	public RectTransform sightingsRoutRect;

	// Token: 0x040029BE RID: 10686
	public RectTransform overlayAll;

	// Token: 0x040029BF RID: 10687
	public RectTransform pinsRect;

	// Token: 0x040029C0 RID: 10688
	public RectTransform tooltipOverride;

	// Token: 0x040029C1 RID: 10689
	private List<MapController.PointerData> pointers = new List<MapController.PointerData>();

	// Token: 0x040029C2 RID: 10690
	public Dictionary<Transform, List<RectTransform>> dynamicTrackedObjects = new Dictionary<Transform, List<RectTransform>>();

	// Token: 0x040029C3 RID: 10691
	public Dictionary<Transform, List<RectTransform>> staticTrackedObjects = new Dictionary<Transform, List<RectTransform>>();

	// Token: 0x040029C4 RID: 10692
	public Dictionary<InfoWindow, MapPinButtonController> pinnedObjects = new Dictionary<InfoWindow, MapPinButtonController>();

	// Token: 0x040029C5 RID: 10693
	public List<InfoWindow> invisiblePins = new List<InfoWindow>();

	// Token: 0x040029C6 RID: 10694
	[Header("Key")]
	public TextMeshProUGUI keyUnexplored;

	// Token: 0x040029C7 RID: 10695
	public TextMeshProUGUI keyExploredSafe;

	// Token: 0x040029C8 RID: 10696
	public TextMeshProUGUI keyExploredPrivate;

	// Token: 0x040029C9 RID: 10697
	public TextMeshProUGUI keyVent;

	// Token: 0x040029CA RID: 10698
	public TextMeshProUGUI keyDuct;

	// Token: 0x040029CB RID: 10699
	public TextMeshProUGUI keyOpenHoursOnly;

	// Token: 0x040029CC RID: 10700
	[Header("Setup")]
	public float nodePositionMultiplier = 32f;

	// Token: 0x040029CD RID: 10701
	private float realPositionMultiplier = 1f;

	// Token: 0x040029CE RID: 10702
	public float positionBuffer;

	// Token: 0x040029CF RID: 10703
	public float edgeBuffer = 128f;

	// Token: 0x040029D0 RID: 10704
	public float focusSpeed = 8f;

	// Token: 0x040029D1 RID: 10705
	public float openProgress;

	// Token: 0x040029D2 RID: 10706
	public float savedSize = 952f;

	// Token: 0x040029D3 RID: 10707
	public RectTransform baseLayer;

	// Token: 0x040029D4 RID: 10708
	public FloorZoomController fzc;

	// Token: 0x040029D5 RID: 10709
	private bool forceFocusActive;

	// Token: 0x040029D6 RID: 10710
	private float forceFocusProgress;

	// Token: 0x040029D7 RID: 10711
	private RectTransform focusRect;

	// Token: 0x040029D8 RID: 10712
	private Vector2 focusPos;

	// Token: 0x040029D9 RID: 10713
	public MapController.MapRoute playerRoute;

	// Token: 0x040029DA RID: 10714
	[Header("Graphics")]
	public float mapResolutionDivision = 1f;

	// Token: 0x040029DB RID: 10715
	public int wallWidth = 2;

	// Token: 0x040029DC RID: 10716
	public Color roomBaseColor = Color.grey;

	// Token: 0x040029DD RID: 10717
	[Tooltip("Add this amount to the above once highlighted")]
	public Color highlightedColourAdditive = Color.white;

	// Token: 0x040029DE RID: 10718
	[Tooltip("This will be drawn to all floor textures")]
	public Texture2D publicFloorTexture;

	// Token: 0x040029DF RID: 10719
	public Texture2D privateFloorTexture;

	// Token: 0x040029E0 RID: 10720
	public Texture2D nullRoomTexture;

	// Token: 0x040029E1 RID: 10721
	public Texture2D undiscoveredTexture;

	// Token: 0x040029E2 RID: 10722
	[Tooltip("This will be used for drawing walls")]
	public Texture2D wallTexture;

	// Token: 0x040029E3 RID: 10723
	public Texture2D wallTexCorners;

	// Token: 0x040029E4 RID: 10724
	public List<Texture2D> wallEdge = new List<Texture2D>();

	// Token: 0x040029E5 RID: 10725
	public List<Texture2D> wallDoorway = new List<Texture2D>();

	// Token: 0x040029E6 RID: 10726
	public List<Texture2D> wallWindow = new List<Texture2D>();

	// Token: 0x040029E7 RID: 10727
	public List<Texture2D> outsideWindow = new List<Texture2D>();

	// Token: 0x040029E8 RID: 10728
	public List<Texture2D> dividerLeft = new List<Texture2D>();

	// Token: 0x040029E9 RID: 10729
	public List<Texture2D> dividerRight = new List<Texture2D>();

	// Token: 0x040029EA RID: 10730
	public List<Texture2D> stairwell = new List<Texture2D>();

	// Token: 0x040029EB RID: 10731
	public Texture2D vent;

	// Token: 0x040029EC RID: 10732
	public Texture2D ventUpwardsConnection;

	// Token: 0x040029ED RID: 10733
	public Texture2D ventDownwardsConnection;

	// Token: 0x040029EE RID: 10734
	[Header("Direction Arrow")]
	public GameObject directionalArrowContainer;

	// Token: 0x040029EF RID: 10735
	public bool displayDirectionArrow;

	// Token: 0x040029F0 RID: 10736
	public Transform directionalArrow;

	// Token: 0x040029F1 RID: 10737
	public float directionalArrowDesiredFade;

	// Token: 0x040029F2 RID: 10738
	public float directionalArrowAlpha;

	// Token: 0x040029F3 RID: 10739
	public Material arrowMaterial;

	// Token: 0x040029F4 RID: 10740
	[Header("Canvas Components")]
	public Dictionary<int, MapController.MapLayer> mapLayers = new Dictionary<int, MapController.MapLayer>();

	// Token: 0x040029F5 RID: 10741
	public List<MapAddressButtonController> buttons = new List<MapAddressButtonController>();

	// Token: 0x040029F6 RID: 10742
	private List<GameObject> spawnedDebugComponents = new List<GameObject>();

	// Token: 0x040029F9 RID: 10745
	private static MapController _instance;

	// Token: 0x020005AB RID: 1451
	public class PointerData
	{
		// Token: 0x040029FA RID: 10746
		public RectTransform pointerObject;

		// Token: 0x040029FB RID: 10747
		public RectTransform followRect;

		// Token: 0x040029FC RID: 10748
		public Vector2 followPos;

		// Token: 0x040029FD RID: 10749
		public float pointerShow;
	}

	// Token: 0x020005AC RID: 1452
	public class MapRoute
	{
		// Token: 0x06001FF1 RID: 8177 RVA: 0x001B8D14 File Offset: 0x001B6F14
		public MapRoute(NewNode newStart, NewNode newEnd, Human newHuman, bool newNodeSpecific, NewGameLocation newDestinationTextOverride)
		{
			this.start = newStart;
			this.end = newEnd;
			this.human = newHuman;
			this.nodeSpecific = newNodeSpecific;
			this.destinationTextOverride = newDestinationTextOverride;
			if (this.human == Player.Instance)
			{
				MapController.Instance.playerRoute = this;
				MapController.Instance.DisplayDirectionArrow(true);
			}
			if (!this.UpdatePathData(this.start))
			{
				Game.Log("Player: Unable to calculate a valid route; removing it...", 2);
				this.Remove();
			}
		}

		// Token: 0x06001FF2 RID: 8178 RVA: 0x001B8DA0 File Offset: 0x001B6FA0
		public bool TryUpdateRouteCursor(out int newCursor, int offcourseTolerance = 1)
		{
			newCursor = 0;
			if (this.pathData == null || this.pathData.accessList == null)
			{
				return false;
			}
			if (Player.Instance.currentNode == null)
			{
				return false;
			}
			for (int i = this.pathData.accessList.Count - 1; i >= 0; i--)
			{
				NewNode nodeBehind = this.pathData.GetNodeBehind(i);
				if (nodeBehind != null)
				{
					if (nodeBehind == Player.Instance.currentNode)
					{
						if (Game.Instance.printDebug)
						{
							Game.Log("Player: Updated route cursor: " + this.routeCursor.ToString(), 2);
						}
						newCursor = i;
						return true;
					}
					if (offcourseTolerance > 0)
					{
						for (int j = 0; j < offcourseTolerance; j++)
						{
							Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX8;
							for (int k = 0; k < offsetArrayX.Length; k++)
							{
								Vector2 vector = offsetArrayX[k];
								Vector3 searchNodeCoord = nodeBehind.nodeCoord + new Vector3Int((int)vector.x * (j + 1), (int)vector.y * (j + 1), 0);
								if (!this.pathData.accessList.Exists((NewNode.NodeAccess item) => item.fromNode.nodeCoord == searchNodeCoord || item.toNode.nodeCoord == searchNodeCoord) && searchNodeCoord == Player.Instance.currentNode.nodeCoord)
								{
									if (Game.Instance.printDebug)
									{
										Game.Log(string.Concat(new string[]
										{
											"Player: Updated route cursor: ",
											this.routeCursor.ToString(),
											" (tolerance ",
											(j + 1).ToString(),
											")"
										}), 2);
									}
									newCursor = i;
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06001FF3 RID: 8179 RVA: 0x001B8F64 File Offset: 0x001B7164
		public void UpdateRouteBasedOnPlayerPosition()
		{
			if (this.TryUpdateRouteCursor(out this.routeCursor, 1))
			{
				Game.Log("Player: Player is " + this.routeCursor.ToString() + " along the currently set route...", 2);
				if (MapController.Instance.isActiveAndEnabled)
				{
					this.UpdateDrawnRoute();
				}
				this.routeUpdateDue = false;
				return;
			}
			string text = "Player: Player is off-course (";
			Vector3 position = Player.Instance.currentNode.position;
			Game.Log(text + position.ToString() + "), calculating new route...", 2);
			if (this.UpdatePathData(Player.Instance.currentNode))
			{
				this.routeUpdateDue = false;
				return;
			}
			if (Player.Instance.currentNode == null || Player.Instance.currentNode.floorType == NewNode.FloorTileType.CeilingOnly || Player.Instance.currentNode.floorType == NewNode.FloorTileType.noneButIndoors || Player.Instance.currentNode.floorType == NewNode.FloorTileType.none)
			{
				this.routeUpdateDue = true;
				return;
			}
			Game.Log("Player: Unable to calculate a valid route; removing it...", 2);
			this.routeUpdateDue = false;
			this.Remove();
		}

		// Token: 0x06001FF4 RID: 8180 RVA: 0x001B9068 File Offset: 0x001B7268
		public bool UpdatePathData(NewNode fromNode)
		{
			MapController.Instance.playerRoute.start = fromNode;
			string text = "Player: Updating player route path starting at ";
			Vector3 position = this.start.position;
			Game.Log(text + position.ToString(), 2);
			this.pathData = PathFinder.Instance.GetPath(this.start, this.end, this.human, null);
			if (this.pathData == null || this.pathData.accessList == null || this.pathData.accessList.Count <= 0)
			{
				NewNode.NodeAccess nodeAccess = this.end.gameLocation.entrances.Find((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.door && !item.employeeDoor);
				try
				{
					if (nodeAccess.toNode.gameLocation == this.end.gameLocation)
					{
						this.end = nodeAccess.fromNode;
					}
					else
					{
						this.end = nodeAccess.toNode;
					}
					this.pathData = PathFinder.Instance.GetPath(this.start, this.end, this.human, null);
				}
				catch
				{
					return false;
				}
			}
			if (this.pathData != null && this.pathData.accessList != null && this.pathData.accessList.Count > 0)
			{
				this.TryUpdateRouteCursor(out this.routeCursor, 1);
				if (MapController.Instance.isActiveAndEnabled)
				{
					this.UpdateDrawnRoute();
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001FF5 RID: 8181 RVA: 0x001B91F0 File Offset: 0x001B73F0
		public void UpdateDrawnRoute()
		{
			if (Player.Instance.currentNode != this.drawnFrom || this.end != this.drawnTo)
			{
				List<GameObject> list = new List<GameObject>();
				foreach (KeyValuePair<GameObject, NewNode> keyValuePair in this.spawnedObjects)
				{
					list.Add(keyValuePair.Key);
				}
				foreach (GameObject gameObject in list)
				{
					this.spawnedObjects.Remove(gameObject);
					Object.Destroy(gameObject);
				}
				if (this.pathData == null || this.pathData.accessList == null || this.pathData.accessList.Count <= 0)
				{
					return;
				}
				NewNode nodeBehind = this.pathData.GetNodeBehind(this.routeCursor);
				if (nodeBehind != null)
				{
					Vector2 vector = MapController.Instance.NodeCoordToMap(nodeBehind.nodeCoord);
					for (int i = this.routeCursor; i < this.pathData.accessList.Count; i++)
					{
						NewNode nodeBehind2 = this.pathData.GetNodeBehind(i);
						NewNode nodeAhead = this.pathData.GetNodeAhead(i);
						if (nodeBehind2 != null && nodeAhead != null)
						{
							if (!MapController.Instance.mapLayers.ContainsKey(nodeBehind2.nodeCoord.z))
							{
								Game.LogError("Map does not contain layer " + nodeBehind2.nodeCoord.z.ToString(), 2);
							}
							GameObject gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.routeLine, MapController.Instance.mapLayers[nodeBehind2.nodeCoord.z].baseContainer);
							this.spawnedObjects.Add(gameObject2, nodeBehind2);
							RectTransform component = gameObject2.GetComponent<RectTransform>();
							component.anchoredPosition = vector;
							vector = MapController.Instance.NodeCoordToMap(nodeAhead.nodeCoord);
							Vector3 vector2 = vector - component.anchoredPosition;
							component.sizeDelta = new Vector2(vector2.magnitude + 8f, component.sizeDelta.y);
							float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
							component.rotation = Quaternion.Euler(0f, 0f, num);
							component.anchoredPosition = new Vector2(component.anchoredPosition.x, component.anchoredPosition.y - MapController.Instance.nodePositionMultiplier * 1.5f);
						}
					}
				}
				this.drawnFrom = this.start;
				this.drawnTo = this.end;
			}
		}

		// Token: 0x06001FF6 RID: 8182 RVA: 0x001B94D4 File Offset: 0x001B76D4
		public void Remove()
		{
			Game.Log("Interface: Remove player route", 2);
			List<GameObject> list = new List<GameObject>();
			foreach (KeyValuePair<GameObject, NewNode> keyValuePair in this.spawnedObjects)
			{
				list.Add(keyValuePair.Key);
			}
			foreach (GameObject gameObject in list)
			{
				this.spawnedObjects.Remove(gameObject);
				Object.Destroy(gameObject);
			}
			if (this.human == Player.Instance)
			{
				MapController.Instance.playerRoute = null;
				MapController.Instance.DisplayDirectionArrow(false);
				MapController.Instance.plotRouteActiveJuice.Pulsate(false, false);
				MapController.Instance.plotRouteActiveJuice.gameObject.SetActive(false);
				MapController.Instance.plotRouteButton.SetInteractable(false);
				MapController.Instance.plotRouteButton.tooltip.detailText = string.Empty;
				if (!Player.Instance.autoTravelActive && MapController.Instance.autoTravelButton != null)
				{
					MapController.Instance.autoTravelButton.SetInteractable(false);
				}
				if (!MapController.Instance.mapContextMenu.disabledItems.Contains("CancelRoute"))
				{
					MapController.Instance.mapContextMenu.disabledItems.Add("CancelRoute");
				}
				InterfaceControls.Instance.plottedRouteText.gameObject.SetActive(false);
				if (MapController.Instance.OnRemoveRoute != null)
				{
					MapController.Instance.OnRemoveRoute();
				}
			}
		}

		// Token: 0x06001FF7 RID: 8183 RVA: 0x001B9698 File Offset: 0x001B7898
		public string GetDestinationText()
		{
			if (this.destinationTextOverride != null)
			{
				return this.destinationTextOverride.name;
			}
			if (this.nodeSpecific)
			{
				return this.end.room.GetName();
			}
			return this.end.gameLocation.name;
		}

		// Token: 0x06001FF8 RID: 8184 RVA: 0x001B96E8 File Offset: 0x001B78E8
		public NewGameLocation GetDestinationLocation()
		{
			if (this.destinationTextOverride != null)
			{
				return this.destinationTextOverride;
			}
			return this.end.gameLocation;
		}

		// Token: 0x040029FE RID: 10750
		public NewNode start;

		// Token: 0x040029FF RID: 10751
		public NewNode end;

		// Token: 0x04002A00 RID: 10752
		public NewGameLocation destinationTextOverride;

		// Token: 0x04002A01 RID: 10753
		public Human human;

		// Token: 0x04002A02 RID: 10754
		public int routeCursor;

		// Token: 0x04002A03 RID: 10755
		public PathFinder.PathData pathData;

		// Token: 0x04002A04 RID: 10756
		public bool nodeSpecific;

		// Token: 0x04002A05 RID: 10757
		public bool routeUpdateDue;

		// Token: 0x04002A06 RID: 10758
		private NewNode drawnFrom;

		// Token: 0x04002A07 RID: 10759
		private NewNode drawnTo;

		// Token: 0x04002A08 RID: 10760
		public Dictionary<GameObject, NewNode> spawnedObjects = new Dictionary<GameObject, NewNode>();
	}

	// Token: 0x020005AF RID: 1455
	public struct MapLayer
	{
		// Token: 0x04002A0C RID: 10764
		public Canvas canvas;

		// Token: 0x04002A0D RID: 10765
		public CanvasGroup canvasGroup;

		// Token: 0x04002A0E RID: 10766
		public RectTransform backgroundContainer;

		// Token: 0x04002A0F RID: 10767
		public RectTransform baseContainer;

		// Token: 0x04002A10 RID: 10768
		public RectTransform ductsContainer;

		// Token: 0x04002A11 RID: 10769
		public DrawingController drawingController;

		// Token: 0x04002A12 RID: 10770
		public Dictionary<Vector2, RawImage> baseBackgroundImages;

		// Token: 0x04002A13 RID: 10771
		public Dictionary<Vector2, Image> wallImages;
	}

	// Token: 0x020005B0 RID: 1456
	// (Invoke) Token: 0x06001FFF RID: 8191
	public delegate void RoutePlot();

	// Token: 0x020005B1 RID: 1457
	// (Invoke) Token: 0x06002003 RID: 8195
	public delegate void RemoveRoute();
}
