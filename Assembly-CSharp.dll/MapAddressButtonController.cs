using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005A8 RID: 1448
public class MapAddressButtonController : ButtonController
{
	// Token: 0x06001F9A RID: 8090 RVA: 0x001B259C File Offset: 0x001B079C
	public void Setup(NewGameLocation newAddress)
	{
		this.UpdateMapTex = (Action)Delegate.Combine(this.UpdateMapTex, new Action(this.GenerateMapImage));
		base.SetupReferences();
		this.gameLocation = newAddress;
		this.gameLocation.mapButton = this;
		this.tooltip.mainText = string.Empty;
		this.tooltip.parentOverride = MapController.Instance.tooltipOverride;
		this.typeIcon.enabled = false;
		NewRoom newRoom = this.gameLocation.rooms[0];
		foreach (NewRoom newRoom2 in this.gameLocation.rooms)
		{
			if (newRoom2.nodes.Count > newRoom.nodes.Count)
			{
				newRoom = newRoom2;
			}
		}
		Vector2 vector = MapController.Instance.RealPosToMap(newRoom.middleRoomPosition);
		this.typeIcon.GetComponent<RectTransform>().anchoredPosition = vector - this.rect.anchoredPosition;
		if (this.rect.sizeDelta.x > 0f && this.rect.sizeDelta.y > 0f)
		{
			this.UpdateMapImageEndOfFrame();
		}
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x001B26EC File Offset: 0x001B08EC
	public void UpdateMapImageEndOfFrame()
	{
		Toolbox.Instance.InvokeEndOfFrame(this.UpdateMapTex, "Update map image");
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x001B2704 File Offset: 0x001B0904
	public void GenerateMapImage()
	{
		Vector2 vector;
		vector..ctor((float)Mathf.RoundToInt(this.rect.sizeDelta.x / MapController.Instance.mapResolutionDivision), (float)Mathf.RoundToInt(this.rect.sizeDelta.y / MapController.Instance.mapResolutionDivision));
		if (this.tex == null)
		{
			this.tex = new Texture2D((int)vector.x, (int)vector.y);
			this.tex.filterMode = 2;
			this.tex.name = "Generated Texture";
			this.generatedImage.sprite = Sprite.Create(this.tex, new Rect(0f, 0f, (float)this.tex.width, (float)this.tex.height), new Vector2(0.5f, 0.5f), 100f);
			this.generatedImage.GetComponent<RectTransform>().sizeDelta = this.rect.sizeDelta;
			this.generatedImage.color = MapController.Instance.roomBaseColor;
			this.generatedImage.alphaHitTestMinimumThreshold = 0.5f;
		}
		Color32 color;
		color..ctor(0, 0, 0, 0);
		Color32[] pixels = this.tex.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = color;
		}
		this.tex.SetPixels32(pixels);
		List<NewTile> list = new List<NewTile>();
		foreach (NewNode newNode in this.gameLocation.nodes)
		{
			Vector2 vector2 = (MapController.Instance.NodeCoordToMap(newNode.nodeCoord) - this.rect.anchoredPosition - new Vector2(MapController.Instance.nodePositionMultiplier * 0.5f, MapController.Instance.nodePositionMultiplier * 0.5f)) / MapController.Instance.mapResolutionDivision;
			if (!newNode.isOutside && !newNode.gameLocation.thisAsAddress.preset.isOutside && (newNode.floorType == NewNode.FloorTileType.floorOnly || newNode.floorType == NewNode.FloorTileType.floorAndCeiling || newNode.room.explorationLevel <= 0))
			{
				Texture2D texture2D = MapController.Instance.publicFloorTexture;
				if (newNode.room.preset.forbidden == RoomConfiguration.Forbidden.alwaysForbidden && newNode.room.gameLocation != Player.Instance.home && !Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, newNode.room.gameLocation))
				{
					if (newNode.room.preset.allowedIfGivenCorrectPassword && newNode.gameLocation != null && newNode.gameLocation.thisAsAddress != null && GameplayController.Instance.playerKnowsPasswords.Contains(newNode.gameLocation.thisAsAddress.id))
					{
						texture2D = MapController.Instance.publicFloorTexture;
					}
					else
					{
						texture2D = MapController.Instance.privateFloorTexture;
					}
				}
				if (newNode.room.isNullRoom)
				{
					texture2D = MapController.Instance.nullRoomTexture;
				}
				if (newNode.room.explorationLevel <= 0)
				{
					texture2D = MapController.Instance.undiscoveredTexture;
				}
				Color32[] pixels2 = texture2D.GetPixels32();
				this.tex.SetPixels32((int)vector2.x, (int)vector2.y, MapController.Instance.publicFloorTexture.width, MapController.Instance.publicFloorTexture.height, pixels2);
			}
			if (newNode.room.explorationLevel > 0 && newNode.tile.isStairwell && !list.Contains(newNode.tile))
			{
				list.Add(newNode.tile);
				NewNode newNode2 = newNode.tile.nodes.Find((NewNode item) => item.localTileCoord == Vector2.zero);
				Vector2 vector3 = (MapController.Instance.NodeCoordToMap(newNode2.nodeCoord) - this.rect.anchoredPosition - new Vector2(MapController.Instance.nodePositionMultiplier * 0.5f, MapController.Instance.nodePositionMultiplier * 0.5f)) / MapController.Instance.mapResolutionDivision;
				if (newNode.tile.stairwellRotation == 0)
				{
					Color[] pixels3 = MapController.Instance.stairwell[0].GetPixels(0, 0, MapController.Instance.stairwell[0].width, MapController.Instance.stairwell[0].height);
					this.tex.SetPixels((int)vector3.x + MapController.Instance.wallWidth, (int)vector3.y + MapController.Instance.wallWidth, MapController.Instance.stairwell[0].width, MapController.Instance.stairwell[0].height, pixels3);
				}
				else if (newNode.tile.stairwellRotation == 90 || newNode.tile.stairwellRotation == -270)
				{
					Color[] pixels4 = MapController.Instance.stairwell[1].GetPixels(0, 0, MapController.Instance.stairwell[1].width, MapController.Instance.stairwell[1].height);
					this.tex.SetPixels((int)vector3.x + MapController.Instance.wallWidth, (int)vector3.y + MapController.Instance.wallWidth, MapController.Instance.stairwell[1].width, MapController.Instance.stairwell[1].height, pixels4);
				}
				else if (newNode.tile.stairwellRotation == 180)
				{
					Color[] pixels5 = MapController.Instance.stairwell[2].GetPixels(0, 0, MapController.Instance.stairwell[2].width, MapController.Instance.stairwell[2].height);
					this.tex.SetPixels((int)vector3.x + MapController.Instance.wallWidth, (int)vector3.y, MapController.Instance.stairwell[2].width, MapController.Instance.stairwell[2].height, pixels5);
				}
				else if (newNode.tile.stairwellRotation == 270 || newNode.tile.stairwellRotation == -90)
				{
					Color[] pixels6 = MapController.Instance.stairwell[3].GetPixels(0, 0, MapController.Instance.stairwell[3].width, MapController.Instance.stairwell[3].height);
					this.tex.SetPixels((int)vector3.x, (int)vector3.y + MapController.Instance.wallWidth, MapController.Instance.stairwell[3].width, MapController.Instance.stairwell[3].height, pixels6);
				}
			}
		}
		foreach (NewRoom newRoom in this.gameLocation.rooms)
		{
			if (newRoom.explorationLevel >= 2)
			{
				foreach (FurnitureLocation furnitureLocation in newRoom.individualFurniture)
				{
					if (furnitureLocation.furniture.map != null && furnitureLocation.furniture.drawUnderWalls)
					{
						Texture2D texture2D2 = furnitureLocation.furniture.map;
						Vector3 vector4 = Vector3.zero;
						foreach (NewNode newNode3 in furnitureLocation.coversNodes)
						{
							vector4 += newNode3.nodeCoord;
						}
						if (!furnitureLocation.furniture.ignoreDirection)
						{
							int num = Mathf.RoundToInt(furnitureLocation.anchorNode.room.transform.TransformDirection(new Vector3(0f, (float)(furnitureLocation.angle + furnitureLocation.diagonalAngle), 0f)).y);
							texture2D2 = this.RotateTexture(texture2D2, (float)(-(float)num) * furnitureLocation.scaleMultiplier.x);
						}
						Vector2 vector5 = (MapController.Instance.NodeCoordToMap(vector4 / (float)furnitureLocation.coversNodes.Count) - this.rect.anchoredPosition - new Vector2((float)texture2D2.width * 0.5f, (float)texture2D2.height * 0.5f)) / MapController.Instance.mapResolutionDivision;
						for (int j = 0; j < texture2D2.width; j++)
						{
							for (int k = 0; k < texture2D2.height; k++)
							{
								Color color2 = texture2D2.GetPixel(j, k);
								if (color2.a > 0.001f)
								{
									Color pixel = this.tex.GetPixel((int)vector5.x + j, (int)vector5.y + k);
									color2 = color2 * color2.a + pixel * (1f - color2.a);
									this.tex.SetPixel((int)vector5.x + j, (int)vector5.y + k, color2);
								}
							}
						}
					}
				}
			}
		}
		foreach (NewNode newNode4 in this.gameLocation.nodes)
		{
			Vector2 vector6 = (MapController.Instance.NodeCoordToMap(newNode4.nodeCoord) - this.rect.anchoredPosition - new Vector2(MapController.Instance.nodePositionMultiplier * 0.5f, MapController.Instance.nodePositionMultiplier * 0.5f)) / MapController.Instance.mapResolutionDivision;
			int num2 = 0;
			if (!newNode4.isOutside && !newNode4.gameLocation.thisAsAddress.preset.isOutside && (newNode4.floorType == NewNode.FloorTileType.floorOnly || newNode4.floorType == NewNode.FloorTileType.floorAndCeiling) && newNode4.room.preset.forbidden == RoomConfiguration.Forbidden.alwaysForbidden && newNode4.room.gameLocation != Player.Instance.home && !Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, newNode4.room.gameLocation))
			{
				if (newNode4.room.preset.allowedIfGivenCorrectPassword && newNode4.gameLocation != null && newNode4.gameLocation.thisAsAddress != null && GameplayController.Instance.playerKnowsPasswords.Contains(newNode4.gameLocation.thisAsAddress.id))
				{
					num2 = 0;
				}
				else
				{
					num2 = 2;
				}
			}
			if (newNode4.room.explorationLevel <= 0)
			{
				num2 = 4;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			foreach (NewWall newWall in newNode4.walls)
			{
				if (newNode4.room.explorationLevel > 0 || newWall.otherWall.node.room.explorationLevel > 0 || !(newWall.otherWall.node.gameLocation == newWall.node.gameLocation))
				{
					int num3 = num2 + 1;
					if (newWall.wallOffset.y < 0f || newWall.wallOffset.y > 0f)
					{
						num3 = num2;
					}
					Texture2D texture2D3 = MapController.Instance.wallEdge[num3];
					int num4 = MapController.Instance.wallWidth;
					if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
					{
						texture2D3 = MapController.Instance.wallDoorway[num3];
						if (newWall.preset.dividerLeft)
						{
							if (newWall.parentWall == newWall)
							{
								texture2D3 = MapController.Instance.dividerLeft[num3];
							}
							else
							{
								texture2D3 = MapController.Instance.dividerRight[num3];
							}
						}
						else if (newWall.preset.dividerRight)
						{
							if (newWall.parentWall == newWall)
							{
								texture2D3 = MapController.Instance.dividerRight[num3];
							}
							else
							{
								texture2D3 = MapController.Instance.dividerLeft[num3];
							}
						}
						else if (newWall.preset.divider)
						{
							continue;
						}
					}
					else if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.window || newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge)
					{
						if (newNode4.isOutside || newNode4.room.IsOutside() || newNode4.room.building == null || (newNode4.floorType != NewNode.FloorTileType.floorOnly && newNode4.floorType != NewNode.FloorTileType.floorAndCeiling))
						{
							texture2D3 = MapController.Instance.outsideWindow[num3];
							num4 = MapController.Instance.wallWidth + 2;
						}
						else
						{
							texture2D3 = MapController.Instance.wallWindow[num3];
						}
					}
					if (newWall.preset.mapOverride.Count > 0)
					{
						texture2D3 = newWall.preset.mapOverride[num3];
					}
					if (newWall.wallOffset.x < 0f)
					{
						flag = true;
						Color[] pixels7 = texture2D3.GetPixels(0, 0, num4, MapController.Instance.wallTexture.height);
						this.tex.SetPixels((int)vector6.x, (int)vector6.y, num4, MapController.Instance.wallTexture.height, pixels7);
					}
					else if (newWall.wallOffset.x > 0f)
					{
						flag2 = true;
						Color[] pixels8 = texture2D3.GetPixels(MapController.Instance.wallTexture.width - num4, 0, num4, MapController.Instance.wallTexture.height);
						this.tex.SetPixels((int)vector6.x + MapController.Instance.wallTexture.width - num4, (int)vector6.y, num4, MapController.Instance.wallTexture.height, pixels8);
					}
					else if (newWall.wallOffset.y < 0f)
					{
						flag4 = true;
						Color[] pixels9 = texture2D3.GetPixels(0, 0, MapController.Instance.wallTexture.width, num4);
						this.tex.SetPixels((int)vector6.x, (int)vector6.y, MapController.Instance.wallTexture.width, num4, pixels9);
					}
					else if (newWall.wallOffset.y > 0f)
					{
						flag3 = true;
						Color[] pixels10 = texture2D3.GetPixels(0, MapController.Instance.wallTexture.height - num4, MapController.Instance.wallTexture.width, num4);
						this.tex.SetPixels((int)vector6.x, (int)vector6.y + MapController.Instance.wallTexture.height - num4, MapController.Instance.wallTexture.width, num4, pixels10);
					}
				}
			}
			if (flag && flag4)
			{
				Color[] pixels11 = MapController.Instance.wallTexture.GetPixels(0, 0, MapController.Instance.wallWidth, MapController.Instance.wallWidth);
				this.tex.SetPixels((int)vector6.x, (int)vector6.y, MapController.Instance.wallWidth, MapController.Instance.wallWidth, pixels11);
			}
			if (flag && flag3)
			{
				Color[] pixels12 = MapController.Instance.wallTexture.GetPixels(0, MapController.Instance.wallTexture.height - MapController.Instance.wallWidth, MapController.Instance.wallWidth, MapController.Instance.wallWidth);
				this.tex.SetPixels((int)vector6.x, (int)vector6.y + MapController.Instance.wallTexture.height - MapController.Instance.wallWidth, MapController.Instance.wallWidth, MapController.Instance.wallWidth, pixels12);
			}
			if (flag2 && flag4)
			{
				Color[] pixels13 = MapController.Instance.wallTexture.GetPixels(MapController.Instance.wallTexture.width - MapController.Instance.wallWidth, 0, MapController.Instance.wallWidth, MapController.Instance.wallWidth);
				this.tex.SetPixels((int)vector6.x + MapController.Instance.wallTexture.width - MapController.Instance.wallWidth, (int)vector6.y, MapController.Instance.wallWidth, MapController.Instance.wallWidth, pixels13);
			}
			if (flag2 && flag3)
			{
				Color[] pixels14 = MapController.Instance.wallTexture.GetPixels(MapController.Instance.wallTexture.width - MapController.Instance.wallWidth, MapController.Instance.wallTexture.height - MapController.Instance.wallWidth, MapController.Instance.wallWidth, MapController.Instance.wallWidth);
				this.tex.SetPixels((int)vector6.x + MapController.Instance.wallTexture.width - MapController.Instance.wallWidth, (int)vector6.y + MapController.Instance.wallTexture.height - MapController.Instance.wallWidth, MapController.Instance.wallWidth, MapController.Instance.wallWidth, pixels14);
			}
			NewNode newNode5;
			NewNode newNode6;
			if (!flag && !flag4 && PathFinder.Instance.nodeMap.TryGetValue(newNode4.nodeCoord + new Vector3Int(-1, 0, 0), ref newNode5) && PathFinder.Instance.nodeMap.TryGetValue(newNode4.nodeCoord + new Vector3Int(0, -1, 0), ref newNode6))
			{
				if (newNode5.walls.Exists((NewWall item) => item.wallOffset.y < 0f && (item.node.room.explorationLevel >= 1 || item.otherWall.node.room.explorationLevel >= 1 || item.node.gameLocation != item.otherWall.node.gameLocation)))
				{
					if (newNode6.walls.Exists((NewWall item) => item.wallOffset.x < 0f && (item.node.room.explorationLevel >= 1 || item.otherWall.node.room.explorationLevel >= 1 || item.node.gameLocation != item.otherWall.node.gameLocation)))
					{
						Color[] pixels15 = MapController.Instance.wallTexCorners.GetPixels(0, 0, MapController.Instance.wallWidth, MapController.Instance.wallWidth);
						this.tex.SetPixels((int)vector6.x, (int)vector6.y, MapController.Instance.wallWidth, MapController.Instance.wallWidth, pixels15);
					}
				}
			}
			NewNode newNode7;
			NewNode newNode8;
			if (!flag && !flag3 && PathFinder.Instance.nodeMap.TryGetValue(newNode4.nodeCoord + new Vector3Int(-1, 0, 0), ref newNode7) && PathFinder.Instance.nodeMap.TryGetValue(newNode4.nodeCoord + new Vector3Int(0, 1, 0), ref newNode8))
			{
				if (newNode7.walls.Exists((NewWall item) => item.wallOffset.y > 0f && (item.node.room.explorationLevel >= 1 || item.otherWall.node.room.explorationLevel >= 1 || item.node.gameLocation != item.otherWall.node.gameLocation)))
				{
					if (newNode8.walls.Exists((NewWall item) => item.wallOffset.x < 0f && (item.node.room.explorationLevel >= 1 || item.otherWall.node.room.explorationLevel >= 1 || item.node.gameLocation != item.otherWall.node.gameLocation)))
					{
						Color[] pixels16 = MapController.Instance.wallTexCorners.GetPixels(0, MapController.Instance.wallTexture.height - MapController.Instance.wallWidth, MapController.Instance.wallWidth, MapController.Instance.wallWidth);
						this.tex.SetPixels((int)vector6.x, (int)vector6.y + MapController.Instance.wallTexture.height - MapController.Instance.wallWidth, MapController.Instance.wallWidth, MapController.Instance.wallWidth, pixels16);
					}
				}
			}
			NewNode newNode9;
			NewNode newNode10;
			if (!flag2 && !flag4 && PathFinder.Instance.nodeMap.TryGetValue(newNode4.nodeCoord + new Vector3Int(1, 0, 0), ref newNode9) && PathFinder.Instance.nodeMap.TryGetValue(newNode4.nodeCoord + new Vector3Int(0, -1, 0), ref newNode10))
			{
				if (newNode9.walls.Exists((NewWall item) => item.wallOffset.y < 0f && (item.node.room.explorationLevel >= 1 || item.otherWall.node.room.explorationLevel >= 1 || item.node.gameLocation != item.otherWall.node.gameLocation)))
				{
					if (newNode10.walls.Exists((NewWall item) => item.wallOffset.x > 0f && (item.node.room.explorationLevel >= 1 || item.otherWall.node.room.explorationLevel >= 1 || item.node.gameLocation != item.otherWall.node.gameLocation)))
					{
						Color[] pixels17 = MapController.Instance.wallTexCorners.GetPixels(MapController.Instance.wallTexture.width - MapController.Instance.wallWidth, 0, MapController.Instance.wallWidth, MapController.Instance.wallWidth);
						this.tex.SetPixels((int)vector6.x + MapController.Instance.wallTexture.width - MapController.Instance.wallWidth, (int)vector6.y, MapController.Instance.wallWidth, MapController.Instance.wallWidth, pixels17);
					}
				}
			}
			NewNode newNode11;
			NewNode newNode12;
			if (!flag2 && !flag3 && PathFinder.Instance.nodeMap.TryGetValue(newNode4.nodeCoord + new Vector3Int(1, 0, 0), ref newNode11) && PathFinder.Instance.nodeMap.TryGetValue(newNode4.nodeCoord + new Vector3Int(0, 1, 0), ref newNode12))
			{
				if (newNode11.walls.Exists((NewWall item) => item.wallOffset.y > 0f && (item.node.room.explorationLevel >= 1 || item.otherWall.node.room.explorationLevel >= 1 || item.node.gameLocation != item.otherWall.node.gameLocation)))
				{
					if (newNode12.walls.Exists((NewWall item) => item.wallOffset.x > 0f && (item.node.room.explorationLevel >= 1 || item.otherWall.node.room.explorationLevel >= 1 || item.node.gameLocation != item.otherWall.node.gameLocation)))
					{
						Color[] pixels18 = MapController.Instance.wallTexCorners.GetPixels(MapController.Instance.wallTexture.width - MapController.Instance.wallWidth, MapController.Instance.wallTexture.height - MapController.Instance.wallWidth, MapController.Instance.wallWidth, MapController.Instance.wallWidth);
						this.tex.SetPixels((int)vector6.x + MapController.Instance.wallTexture.width - MapController.Instance.wallWidth, (int)vector6.y + MapController.Instance.wallTexture.height - MapController.Instance.wallWidth, MapController.Instance.wallWidth, MapController.Instance.wallWidth, pixels18);
					}
				}
			}
		}
		foreach (NewRoom newRoom2 in this.gameLocation.rooms)
		{
			if (newRoom2.explorationLevel >= 2)
			{
				foreach (FurnitureLocation furnitureLocation2 in newRoom2.individualFurniture)
				{
					if (furnitureLocation2.furniture.map != null && !furnitureLocation2.furniture.drawUnderWalls)
					{
						Texture2D texture2D4 = furnitureLocation2.furniture.map;
						Vector3 vector7 = Vector3.zero;
						foreach (NewNode newNode13 in furnitureLocation2.coversNodes)
						{
							vector7 += newNode13.nodeCoord;
						}
						if (!furnitureLocation2.furniture.ignoreDirection)
						{
							int num5 = Mathf.RoundToInt(furnitureLocation2.anchorNode.room.transform.TransformDirection(new Vector3(0f, (float)(furnitureLocation2.angle + furnitureLocation2.diagonalAngle), 0f)).y);
							if (num5 != 0)
							{
								texture2D4 = this.RotateTexture(texture2D4, (float)(-(float)num5) * furnitureLocation2.scaleMultiplier.x);
							}
						}
						Vector2 vector8 = (MapController.Instance.NodeCoordToMap(vector7 / (float)furnitureLocation2.coversNodes.Count) - this.rect.anchoredPosition - new Vector2((float)texture2D4.width * 0.5f, (float)texture2D4.height * 0.5f)) / MapController.Instance.mapResolutionDivision;
						for (int l = 0; l < texture2D4.width; l++)
						{
							for (int m = 0; m < texture2D4.height; m++)
							{
								Color color3 = texture2D4.GetPixel(l, m);
								if (color3.a > 0.001f)
								{
									Color pixel2 = this.tex.GetPixel((int)vector8.x + l, (int)vector8.y + m);
									color3 = color3 * color3.a + pixel2 * (1f - color3.a);
									this.tex.SetPixel((int)vector8.x + l, (int)vector8.y + m, color3);
								}
							}
						}
					}
				}
			}
		}
		this.tex.Apply();
		this.UpdateIcon();
		this.UpdateTooltip();
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x001B422C File Offset: 0x001B242C
	public void UpdateIcon()
	{
		if (this.gameLocation.thisAsAddress != null)
		{
			if (this.gameLocation.thisAsAddress.addressPreset != null && this.gameLocation.evidenceEntry != null && (this.gameLocation.thisAsAddress.addressPreset.company != null || this.gameLocation == Player.Instance.home || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, this.gameLocation)))
			{
				if (this.gameLocation.evidenceEntry.keyTies[Evidence.DataKey.location].Contains(Evidence.DataKey.purpose))
				{
					if (!this.typeIcon.enabled)
					{
						this.typeIcon.enabled = true;
					}
					this.typeIcon.sprite = this.gameLocation.evidenceEntry.iconSprite;
					this.typeIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(32f, 32f);
				}
				else
				{
					this.typeIcon.enabled = false;
				}
			}
			else
			{
				this.typeIcon.enabled = false;
			}
		}
		else
		{
			this.typeIcon.enabled = false;
		}
		this.typeIcon.transform.SetAsLastSibling();
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x001B4378 File Offset: 0x001B2578
	public void UpdateTooltip()
	{
		if (this.gameLocation.evidenceEntry != null)
		{
			this.tooltip.mainText = this.gameLocation.evidenceEntry.GetNameForDataKey(Evidence.DataKey.location);
			this.tooltip.detailText = this.gameLocation.evidenceEntry.GetNoteComposed(Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[]
			{
				Evidence.DataKey.location
			}), false);
		}
		this.tooltip.parentOverride = MapController.Instance.tooltipOverride;
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x001B43F0 File Offset: 0x001B25F0
	private Texture2D RotateTexture(Texture2D tex, float angle)
	{
		Texture2D texture2D = new Texture2D(tex.width, tex.height);
		int width = tex.width;
		int height = tex.height;
		float num = this.Rot_x(angle, (float)(-(float)width) / 2f, (float)(-(float)height) / 2f) + (float)width / 2f;
		float num2 = this.Rot_y(angle, (float)(-(float)width) / 2f, (float)(-(float)height) / 2f) + (float)height / 2f;
		float num3 = this.Rot_x(angle, 1f, 0f);
		float num4 = this.Rot_y(angle, 1f, 0f);
		float num5 = this.Rot_x(angle, 0f, 1f);
		float num6 = this.Rot_y(angle, 0f, 1f);
		float num7 = num;
		float num8 = num2;
		for (int i = 0; i < tex.width; i++)
		{
			float num9 = num7;
			float num10 = num8;
			for (int j = 0; j < tex.height; j++)
			{
				num9 += num3;
				num10 += num4;
				texture2D.SetPixel((int)Mathf.Floor((float)i), (int)Mathf.Floor((float)j), this.GetPixel(tex, num9, num10));
			}
			num7 += num5;
			num8 += num6;
		}
		return texture2D;
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x001B4524 File Offset: 0x001B2724
	private Color GetPixel(Texture2D tex, float x, float y)
	{
		int num = (int)Mathf.Floor(x);
		int num2 = (int)Mathf.Floor(y);
		Color result;
		if (num > tex.width || num < 0 || num2 > tex.height || num2 < 0)
		{
			result = Color.clear;
		}
		else
		{
			result = tex.GetPixel(num, num2);
		}
		return result;
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x001B4570 File Offset: 0x001B2770
	private float Rot_x(float angle, float x, float y)
	{
		float num = Mathf.Cos(angle / 180f * 3.1415927f);
		float num2 = Mathf.Sin(angle / 180f * 3.1415927f);
		return x * num + y * -num2;
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x001B45AC File Offset: 0x001B27AC
	private float Rot_y(float angle, float x, float y)
	{
		float num = Mathf.Cos(angle / 180f * 3.1415927f);
		float num2 = Mathf.Sin(angle / 180f * 3.1415927f);
		return x * num2 + y * num;
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x001B45E6 File Offset: 0x001B27E6
	public override void OnHoverStart()
	{
		this.generatedImage.color = MapController.Instance.roomBaseColor + MapController.Instance.highlightedColourAdditive;
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x001B460C File Offset: 0x001B280C
	public override void OnHoverEnd()
	{
		this.generatedImage.color = MapController.Instance.roomBaseColor;
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x001B4624 File Offset: 0x001B2824
	public override void OnLeftDoubleClick()
	{
		MapController.Instance.CentreOnObject(this.rect, false, false);
		InterfaceController.Instance.SpawnWindow(this.gameLocation.evidenceEntry, Evidence.DataKey.location, null, "", false, true, default(Vector2), null, null, null, true);
	}

	// Token: 0x06001FA6 RID: 8102 RVA: 0x001B4670 File Offset: 0x001B2870
	public override void OnRightClick()
	{
		MapController.Instance.mapContextMenu.OpenMenu();
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x001B4684 File Offset: 0x001B2884
	public void OpenFolder()
	{
		InterfaceController.Instance.SpawnWindow(this.gameLocation.evidenceEntry, Evidence.DataKey.location, null, "", false, true, default(Vector2), null, null, null, true);
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x00002265 File Offset: 0x00000465
	public void MapRoute()
	{
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x00002265 File Offset: 0x00000465
	public void DoFastTravel()
	{
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x00002265 File Offset: 0x00000465
	public void DoFastTravelBuilding()
	{
	}

	// Token: 0x04002985 RID: 10629
	public NewGameLocation gameLocation;

	// Token: 0x04002986 RID: 10630
	public Image typeIcon;

	// Token: 0x04002987 RID: 10631
	public Vector2 range;

	// Token: 0x04002988 RID: 10632
	public Image generatedImage;

	// Token: 0x04002989 RID: 10633
	public Texture2D tex;

	// Token: 0x0400298A RID: 10634
	private Action UpdateMapTex;
}
