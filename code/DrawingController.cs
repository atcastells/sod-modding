using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020004DC RID: 1244
public class DrawingController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06001ADC RID: 6876 RVA: 0x00189244 File Offset: 0x00187444
	private void Awake()
	{
		this.SetDrawingActive(this.drawingActive);
		if (this.setupButtons)
		{
			this.windowButtonsController = base.gameObject.transform.parent.parent.parent.parent.GetComponentInChildren<WindowExtraControlsController>();
			this.windowButtonsController.drawingController = this;
			this.windowButtonsController.SetEnableDrawingControls(true);
		}
	}

	// Token: 0x06001ADD RID: 6877 RVA: 0x001892A7 File Offset: 0x001874A7
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		this.isOver = true;
	}

	// Token: 0x06001ADE RID: 6878 RVA: 0x001892B0 File Offset: 0x001874B0
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.isOver = false;
	}

	// Token: 0x06001ADF RID: 6879 RVA: 0x001892D0 File Offset: 0x001874D0
	public void SetDrawingActive(bool val)
	{
		this.drawingActive = val;
		if (!this.drawingActive)
		{
			this.img.raycastTarget = false;
			this.drawBrushRect.gameObject.SetActive(false);
			base.enabled = false;
			return;
		}
		this.img.raycastTarget = true;
		this.SetBrushColour(this.brushColour);
		this.SetBrushImage(this.brush);
		this.drawBrushRect.gameObject.SetActive(true);
		base.enabled = true;
		this.lastPosValid = false;
		this.isOver = false;
	}

	// Token: 0x06001AE0 RID: 6880 RVA: 0x0018935C File Offset: 0x0018755C
	public void ResetDrawingTexture()
	{
		if (this.drawingTex != null)
		{
			Object.Destroy(this.drawingTex);
		}
		this.drawingTex = new Texture2D(Mathf.RoundToInt(this.container.sizeDelta.x), Mathf.RoundToInt(this.container.sizeDelta.y));
		this.drawingTex.name = "Instanced Drawing Tex";
		for (int i = 0; i < Mathf.RoundToInt(this.container.sizeDelta.x); i++)
		{
			for (int j = 0; j < Mathf.RoundToInt(this.container.sizeDelta.y); j++)
			{
				this.drawingTex.SetPixel(i, j, Color.clear);
			}
		}
		this.drawingTex.Apply();
		this.img.texture = this.drawingTex;
		this.img.color = Color.white;
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x00189445 File Offset: 0x00187645
	public void SetEraserMode(bool val)
	{
		this.eraserMode = val;
		if (this.eraserMode)
		{
			this.SetBrushImage(PrefabControls.Instance.eraseBrush);
			return;
		}
		this.SetBrushImage(PrefabControls.Instance.drawingBrush);
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x00189477 File Offset: 0x00187677
	public void SetBrushColour(Color newCol)
	{
		this.brushColour = newCol;
		this.brushImage.color = newCol;
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x0018948C File Offset: 0x0018768C
	public void SetBrushImage(Texture2D newBrush)
	{
		this.brush = newBrush;
		this.brushImage.texture = newBrush;
		this.brushSize = new Vector2((float)newBrush.width, (float)newBrush.height);
		this.drawBrushRect.sizeDelta = this.brushSize;
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x001894CC File Offset: 0x001876CC
	private void Update()
	{
		if (this.drawingActive)
		{
			if (!this.isOver)
			{
				return;
			}
			if (this.drawingTex == null)
			{
				this.ResetDrawingTexture();
			}
			Vector2 zero = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.container, Input.mousePosition, null, ref zero);
			this.drawBrushRect.localPosition = zero;
			int width = this.drawingTex.width;
			int height = this.drawingTex.height;
			if (Input.GetMouseButtonDown(0) && !this.startedDraw && this.drawBrushRect.localPosition.x >= 0f && this.drawBrushRect.localPosition.x < this.container.sizeDelta.x && this.drawBrushRect.localPosition.y >= 0f && this.drawBrushRect.localPosition.y < this.container.sizeDelta.y)
			{
				this.startedDraw = true;
				this.lastPosValid = false;
			}
			if (this.startedDraw)
			{
				if (!Input.GetMouseButton(0))
				{
					this.lastPosValid = false;
					this.startedDraw = false;
					return;
				}
				if (this.drawBrushRect.localPosition.x < 0f || this.drawBrushRect.localPosition.x >= this.container.sizeDelta.x)
				{
					this.lastPosValid = false;
					return;
				}
				if (this.drawBrushRect.localPosition.y >= 0f && this.drawBrushRect.localPosition.y < this.container.sizeDelta.y)
				{
					if (this.lastPosValid)
					{
						Vector2 vector = this.lastValidLocalPos;
						float num = 0f;
						float num2 = Vector2.Distance(this.lastValidLocalPos, zero);
						while (vector != zero)
						{
							int num3 = 0;
							while ((float)num3 < this.brushSize.x)
							{
								int num4 = (int)vector.x - num3;
								if (num4 < width && num4 >= 0)
								{
									int num5 = 0;
									while ((float)num5 < this.brushSize.y)
									{
										int num6 = (int)vector.y + num5;
										if (num6 < height && num6 >= 0)
										{
											if (this.eraserMode)
											{
												this.drawingTex.SetPixel(num4, num6, Color.clear);
											}
											else
											{
												Color color = this.brushColour;
												color.a = this.brush.GetPixel(num3, num5).a;
												Color pixel = this.drawingTex.GetPixel(num4, num6);
												float a = color.a;
												float num7 = 1f - color.a;
												float num8 = a + num7 * pixel.a;
												Color color2 = (color * a + pixel * pixel.a * num7) / num8;
												color2.a = num8;
												this.drawingTex.SetPixel(num4, num6, color2);
											}
										}
										num5++;
									}
								}
								num3++;
							}
							num += 1f / num2;
							vector = Vector2.Lerp(this.lastValidLocalPos, zero, num);
						}
					}
					int num9 = 0;
					while ((float)num9 < this.brushSize.x)
					{
						int num10 = (int)zero.x - num9;
						if (num10 < width && num10 >= 0)
						{
							int num11 = 0;
							while ((float)num11 < this.brushSize.y)
							{
								int num12 = (int)zero.y + num11;
								if (num12 < height && num12 >= 0)
								{
									if (this.eraserMode)
									{
										this.drawingTex.SetPixel(num10, num12, Color.clear);
									}
									else
									{
										Color color3 = this.brushColour;
										color3.a = this.brush.GetPixel(num9, num11).a;
										Color pixel2 = this.drawingTex.GetPixel(num10, num12);
										float a2 = color3.a;
										float num13 = 1f - color3.a;
										float num14 = a2 + num13 * pixel2.a;
										Color color4 = (color3 * a2 + pixel2 * pixel2.a * num13) / num14;
										color4.a = num14;
										this.drawingTex.SetPixel(num10, num12, color4);
									}
								}
								num11++;
							}
						}
						num9++;
					}
					this.drawingTex.Apply();
					this.lastPosValid = true;
					this.lastValidLocalPos = zero;
					return;
				}
				this.lastPosValid = false;
				return;
			}
		}
		else
		{
			this.SetDrawingActive(false);
		}
	}

	// Token: 0x04002378 RID: 9080
	[Header("Setup Components")]
	public RectTransform container;

	// Token: 0x04002379 RID: 9081
	public RawImage img;

	// Token: 0x0400237A RID: 9082
	public RectTransform drawBrushRect;

	// Token: 0x0400237B RID: 9083
	public RawImage brushImage;

	// Token: 0x0400237C RID: 9084
	[Header("Generated Components")]
	public Texture2D drawingTex;

	// Token: 0x0400237D RID: 9085
	[Header("State")]
	public bool isOver;

	// Token: 0x0400237E RID: 9086
	public bool drawingActive;

	// Token: 0x0400237F RID: 9087
	public bool eraserMode;

	// Token: 0x04002380 RID: 9088
	private bool lastPosValid;

	// Token: 0x04002381 RID: 9089
	private Vector2 lastValidLocalPos = Vector2.zero;

	// Token: 0x04002382 RID: 9090
	[Header("Settings")]
	public Color brushColour = Color.white;

	// Token: 0x04002383 RID: 9091
	public Texture2D brush;

	// Token: 0x04002384 RID: 9092
	public Vector2 brushSize = new Vector2(4f, 4f);

	// Token: 0x04002385 RID: 9093
	public bool startedDraw;

	// Token: 0x04002386 RID: 9094
	[Header("Buttons")]
	public bool setupButtons;

	// Token: 0x04002387 RID: 9095
	public WindowExtraControlsController windowButtonsController;

	// Token: 0x04002388 RID: 9096
	public ButtonController toggleDrawingButton;

	// Token: 0x04002389 RID: 9097
	public ColourSelectorButtonController colourButton;

	// Token: 0x0400238A RID: 9098
	public ButtonController eraserButton;

	// Token: 0x0400238B RID: 9099
	public ButtonController clearButton;
}
