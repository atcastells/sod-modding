using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005B8 RID: 1464
public class MapDuctsButtonController : ButtonController
{
	// Token: 0x0600201A RID: 8218 RVA: 0x001B9A04 File Offset: 0x001B7C04
	public void Setup(NewFloor newAddress)
	{
		this.UpdateMapTex = (Action)Delegate.Combine(this.UpdateMapTex, new Action(this.GenerateMapImage));
		base.SetupReferences();
		this.floor = newAddress;
		this.floor.mapDucts = this;
		if (this.rect.sizeDelta.x > 0f && this.rect.sizeDelta.y > 0f)
		{
			this.UpdateMapImageEndOfFrame();
		}
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x001B9A80 File Offset: 0x001B7C80
	public void UpdateMapImageEndOfFrame()
	{
		Toolbox.Instance.InvokeEndOfFrame(this.UpdateMapTex, "Update map image");
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x001B9A98 File Offset: 0x001B7C98
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
		foreach (NewAddress newAddress in this.floor.addresses)
		{
			List<AirDuctGroup.AirDuctSection> list = new List<AirDuctGroup.AirDuctSection>();
			List<AirDuctGroup.AirDuctSection> list2 = new List<AirDuctGroup.AirDuctSection>();
			foreach (NewNode newNode in newAddress.nodes)
			{
				foreach (AirDuctGroup.AirDuctSection airDuctSection in newNode.airDucts)
				{
					if (airDuctSection.mapButton == null)
					{
						airDuctSection.mapButton = this;
					}
					if (airDuctSection.discovered)
					{
						Texture2D texture2D = null;
						List<Vector3Int> list3;
						List<AirDuctGroup.AirVent> list4;
						List<Vector3Int> list5;
						airDuctSection.GetNeighborSections(out list3, out list4, out list5);
						list3.AddRange(list5);
						if (airDuctSection.level == 0)
						{
							if (list3.Exists((Vector3Int item) => item.z < 0))
							{
								list2.Add(airDuctSection);
							}
						}
						else if (airDuctSection.level == 2)
						{
							if (list3.Exists((Vector3Int item) => item.z > 0))
							{
								list.Add(airDuctSection);
							}
						}
						foreach (InteriorControls.AirDuctOffset airDuctOffset in InteriorControls.Instance.airDuctModels)
						{
							if (airDuctOffset.offsets.Count == list3.Count)
							{
								bool flag = true;
								foreach (Vector3Int vector3Int in list3)
								{
									Vector3 vector2 = vector3Int;
									if (!airDuctOffset.offsets.Contains(vector2.normalized))
									{
										flag = false;
										break;
									}
								}
								if (flag && airDuctOffset.maps.Count > 0)
								{
									texture2D = airDuctOffset.maps[Mathf.Min(airDuctSection.index, airDuctOffset.maps.Count - 1)];
									break;
								}
							}
						}
						if (texture2D != null)
						{
							Vector2 vector3 = (MapController.Instance.NodeCoordToMap(newNode.nodeCoord) - this.rect.anchoredPosition - new Vector2(MapController.Instance.nodePositionMultiplier * 0.5f, MapController.Instance.nodePositionMultiplier * 0.5f)) / MapController.Instance.mapResolutionDivision;
							for (int j = 0; j < texture2D.width; j++)
							{
								for (int k = 0; k < texture2D.height; k++)
								{
									Color color2 = texture2D.GetPixel(j, k);
									if (color2.a > 0.001f)
									{
										Color pixel = this.tex.GetPixel((int)vector3.x + j, (int)vector3.y + k);
										color2 = color2 * color2.a + pixel * (1f - color2.a);
										this.tex.SetPixel((int)vector3.x + j, (int)vector3.y + k, color2);
									}
								}
							}
						}
					}
				}
			}
			foreach (NewRoom newRoom in newAddress.rooms)
			{
				foreach (AirDuctGroup.AirVent airVent in newRoom.airVents)
				{
					if (airVent.mapButton == null)
					{
						airVent.mapButton = this;
					}
					if (airVent.discovered)
					{
						if (airVent.wall != null)
						{
							Vector2 vector4 = (MapController.Instance.NodeCoordToMap(airVent.node.nodeCoord - new Vector3(airVent.wall.wallOffset.x, airVent.wall.wallOffset.y, 0f)) - this.rect.anchoredPosition - new Vector2(MapController.Instance.nodePositionMultiplier * 0.5f, MapController.Instance.nodePositionMultiplier * 0.5f)) / MapController.Instance.mapResolutionDivision;
							Color32[] pixels2 = MapController.Instance.vent.GetPixels32();
							this.tex.SetPixels32((int)vector4.x + 9, (int)vector4.y + 9, MapController.Instance.vent.width, MapController.Instance.vent.height, pixels2);
						}
						else if (airVent.node != null)
						{
							Vector2 vector5 = (MapController.Instance.NodeCoordToMap(airVent.node.nodeCoord) - this.rect.anchoredPosition - new Vector2(MapController.Instance.nodePositionMultiplier * 0.5f, MapController.Instance.nodePositionMultiplier * 0.5f)) / MapController.Instance.mapResolutionDivision;
							Color32[] pixels3 = MapController.Instance.vent.GetPixels32();
							this.tex.SetPixels32((int)vector5.x + 9, (int)vector5.y + 9, MapController.Instance.vent.width, MapController.Instance.vent.height, pixels3);
						}
					}
				}
			}
			foreach (AirDuctGroup.AirDuctSection airDuctSection2 in list)
			{
				if (airDuctSection2.discovered)
				{
					Vector2 vector6 = (MapController.Instance.NodeCoordToMap(airDuctSection2.node.nodeCoord) - this.rect.anchoredPosition - new Vector2(MapController.Instance.nodePositionMultiplier * 0.5f, MapController.Instance.nodePositionMultiplier * 0.5f)) / MapController.Instance.mapResolutionDivision;
					for (int l = 0; l < MapController.Instance.ventUpwardsConnection.width; l++)
					{
						for (int m = 0; m < MapController.Instance.ventUpwardsConnection.height; m++)
						{
							Color color3 = MapController.Instance.ventUpwardsConnection.GetPixel(l, m);
							if (color3.a > 0.001f)
							{
								Color pixel2 = this.tex.GetPixel((int)vector6.x + l, (int)vector6.y + m);
								color3 = color3 * color3.a + pixel2 * (1f - color3.a);
								this.tex.SetPixel((int)vector6.x + l, (int)vector6.y + m, color3);
							}
						}
					}
				}
			}
			foreach (AirDuctGroup.AirDuctSection airDuctSection3 in list2)
			{
				Vector2 vector7 = (MapController.Instance.NodeCoordToMap(airDuctSection3.node.nodeCoord) - this.rect.anchoredPosition - new Vector2(MapController.Instance.nodePositionMultiplier * 0.5f, MapController.Instance.nodePositionMultiplier * 0.5f)) / MapController.Instance.mapResolutionDivision;
				for (int n = 0; n < MapController.Instance.ventDownwardsConnection.width; n++)
				{
					for (int num = 0; num < MapController.Instance.ventDownwardsConnection.height; num++)
					{
						Color color4 = MapController.Instance.ventDownwardsConnection.GetPixel(n, num);
						if (color4.a > 0.001f)
						{
							Color pixel3 = this.tex.GetPixel((int)vector7.x + n, (int)vector7.y + num);
							color4 = color4 * color4.a + pixel3 * (1f - color4.a);
							this.tex.SetPixel((int)vector7.x + n, (int)vector7.y + num, color4);
						}
					}
				}
			}
		}
		this.tex.Apply();
	}

	// Token: 0x04002A1E RID: 10782
	public NewFloor floor;

	// Token: 0x04002A1F RID: 10783
	public Vector2 range;

	// Token: 0x04002A20 RID: 10784
	public Image generatedImage;

	// Token: 0x04002A21 RID: 10785
	public Texture2D tex;

	// Token: 0x04002A22 RID: 10786
	private Action UpdateMapTex;
}
