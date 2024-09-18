using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200061D RID: 1565
public class WindowExtraControlsController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x060022C7 RID: 8903 RVA: 0x000B7FAD File Offset: 0x000B61AD
	private void Awake()
	{
		base.enabled = false;
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x001D3E79 File Offset: 0x001D2079
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		this.isOver = true;
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x001D3E82 File Offset: 0x001D2082
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.isOver = false;
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x001D3EA0 File Offset: 0x001D20A0
	public void SetEnableDrawingControls(bool val)
	{
		this.drawingControlsEnabled = val;
		if (val)
		{
			this.mouseOverDetector.raycastTarget = true;
			base.enabled = true;
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.drawingControls, base.transform);
			gameObject.SetActive(false);
			this.drawingControls = gameObject.GetComponent<RectTransform>();
			this.toggleDrawingButton = this.drawingControls.Find("ToggleDrawingMode").gameObject.GetComponent<ButtonController>();
			this.colourButton = this.drawingControls.Find("SetDrawingColour").gameObject.GetComponent<ColourSelectorButtonController>();
			this.eraserButton = this.drawingControls.Find("ToggleEraser").gameObject.GetComponent<ButtonController>();
			this.clearButton = this.drawingControls.Find("ClearDrawing").gameObject.GetComponent<ButtonController>();
			this.drawingRenderers.Add(gameObject.GetComponent<CanvasRenderer>());
			this.drawingRenderers.Add(this.toggleDrawingButton.gameObject.GetComponent<CanvasRenderer>());
			this.drawingRenderers.Add(this.toggleDrawingButton.icon.GetComponent<CanvasRenderer>());
			this.drawingRenderers.Add(this.colourButton.gameObject.GetComponent<CanvasRenderer>());
			this.drawingRenderers.Add(this.colourButton.icon.GetComponent<CanvasRenderer>());
			this.drawingRenderers.Add(this.eraserButton.gameObject.GetComponent<CanvasRenderer>());
			this.drawingRenderers.Add(this.eraserButton.icon.GetComponent<CanvasRenderer>());
			this.drawingRenderers.Add(this.clearButton.gameObject.GetComponent<CanvasRenderer>());
			this.drawingRenderers.Add(this.clearButton.icon.GetComponent<CanvasRenderer>());
			this.toggleDrawingButton.OnPress += this.ToggleDrawingMode;
			this.colourButton.OnChangeColour += this.OnChangeDrawingColour;
			this.eraserButton.OnPress += this.ToggleEraser;
			this.clearButton.OnPress += this.ClearDrawing;
			return;
		}
		Object.Destroy(this.drawingControls.gameObject);
		this.drawingRenderers.Clear();
		this.fade = 0f;
		this.mouseOverDetector.raycastTarget = false;
		base.enabled = false;
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x001D40F8 File Offset: 0x001D22F8
	private void OnDestroy()
	{
		if (this.toggleDrawingButton != null)
		{
			this.toggleDrawingButton.OnPress -= this.ToggleDrawingMode;
		}
		if (this.colourButton != null)
		{
			this.colourButton.OnChangeColour -= this.OnChangeDrawingColour;
		}
		if (this.eraserButton != null)
		{
			this.eraserButton.OnPress -= this.ToggleEraser;
		}
		if (this.clearButton != null)
		{
			this.clearButton.OnPress -= this.ClearDrawing;
		}
	}

	// Token: 0x060022CC RID: 8908 RVA: 0x001D419C File Offset: 0x001D239C
	private void Update()
	{
		if (this.isOver || (this.drawingController != null && this.drawingController.drawingActive))
		{
			if (!this.drawingControlsEnabled)
			{
				return;
			}
			if (this.drawingControls != null)
			{
				this.drawingControls.gameObject.SetActive(true);
			}
			if (this.fade >= 1f)
			{
				return;
			}
			this.fade += Time.deltaTime * 8f;
			this.fade = Mathf.Clamp01(this.fade);
			using (List<CanvasRenderer>.Enumerator enumerator = this.drawingRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CanvasRenderer canvasRenderer = enumerator.Current;
					canvasRenderer.SetAlpha(this.fade);
				}
				return;
			}
		}
		if (this.drawingControlsEnabled && this.fade > 0f)
		{
			this.fade -= Time.deltaTime * 8f;
			this.fade = Mathf.Clamp01(this.fade);
			foreach (CanvasRenderer canvasRenderer2 in this.drawingRenderers)
			{
				canvasRenderer2.SetAlpha(this.fade);
			}
			if (this.fade <= 0f && this.drawingControls != null)
			{
				this.drawingControls.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060022CD RID: 8909 RVA: 0x001D4338 File Offset: 0x001D2538
	public void ToggleDrawingMode(ButtonController pressedButton)
	{
		if (this.drawingControlsEnabled)
		{
			this.drawingController.SetDrawingActive(!this.drawingController.drawingActive);
			if (this.drawingController.drawingActive)
			{
				this.mouseOverDetector.raycastTarget = false;
				this.toggleDrawingButton.background.color = InterfaceControls.Instance.selectionColour;
				return;
			}
			this.mouseOverDetector.raycastTarget = true;
			this.toggleDrawingButton.background.color = InterfaceControls.Instance.nonSelectionColour;
		}
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x001D43C0 File Offset: 0x001D25C0
	public void OnChangeDrawingColour()
	{
		if (this.drawingControlsEnabled)
		{
			this.drawingController.SetBrushColour(this.colourButton.selectedColour);
		}
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x001D43E0 File Offset: 0x001D25E0
	public void ToggleEraser(ButtonController pressedButton)
	{
		if (this.drawingControlsEnabled)
		{
			this.drawingController.SetEraserMode(!this.drawingController.eraserMode);
			if (this.drawingController.eraserMode)
			{
				this.eraserButton.background.color = InterfaceControls.Instance.selectionColour;
				return;
			}
			this.eraserButton.background.color = InterfaceControls.Instance.nonSelectionColour;
		}
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x001D4450 File Offset: 0x001D2650
	public void ClearDrawing(ButtonController pressedButton)
	{
		if (this.drawingControlsEnabled)
		{
			this.drawingController.ResetDrawingTexture();
		}
	}

	// Token: 0x04002D0D RID: 11533
	public bool isOver;

	// Token: 0x04002D0E RID: 11534
	public float fade;

	// Token: 0x04002D0F RID: 11535
	public RawImage mouseOverDetector;

	// Token: 0x04002D10 RID: 11536
	[Header("Drawing Controls")]
	public bool drawingControlsEnabled;

	// Token: 0x04002D11 RID: 11537
	public RectTransform drawingControls;

	// Token: 0x04002D12 RID: 11538
	[ReorderableList]
	public List<CanvasRenderer> drawingRenderers = new List<CanvasRenderer>();

	// Token: 0x04002D13 RID: 11539
	public DrawingController drawingController;

	// Token: 0x04002D14 RID: 11540
	public ButtonController toggleDrawingButton;

	// Token: 0x04002D15 RID: 11541
	public ColourSelectorButtonController colourButton;

	// Token: 0x04002D16 RID: 11542
	public ButtonController eraserButton;

	// Token: 0x04002D17 RID: 11543
	public ButtonController clearButton;
}
