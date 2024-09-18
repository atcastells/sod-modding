using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200061F RID: 1567
public class ZoomContent : MonoBehaviour
{
	// Token: 0x060022D4 RID: 8916 RVA: 0x001D44D4 File Offset: 0x001D26D4
	private void Awake()
	{
		this.window = base.gameObject.GetComponentInParent<InfoWindow>();
		if (this.scroll == null)
		{
			this.scroll = this.window.scrollRect;
		}
		this.containerRect = base.GetComponent<RectTransform>();
		this.scrollRectArea = this.scroll.gameObject.GetComponent<RectTransform>();
		this.viewportMouseOver = this.scroll.viewport.gameObject.GetComponent<ViewportMouseOver>();
		this.contentController = base.gameObject.GetComponent<WindowContentController>();
		this.normalSize = this.containerRect.sizeDelta;
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x00002265 File Offset: 0x00000465
	private void Start()
	{
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x001D4570 File Offset: 0x001D2770
	private void SetPivotPoint(float pivotBias, ZoomContent.ZoomPivot usePivot = ZoomContent.ZoomPivot.mousePosition)
	{
		if (this.scrollRectArea == null)
		{
			Game.LogError("Missing scrollRectArea for zoom! Assign this", 2);
			return;
		}
		if (this.containerRect == null)
		{
			Game.LogError("Missing containerRect for zoom! Assign this", 2);
			return;
		}
		if (this.scroll == null)
		{
			Game.LogError("Missing scroll for zoom! Assign this", 2);
			return;
		}
		Vector3[] array = new Vector3[4];
		this.scrollRectArea.GetWorldCorners(array);
		Vector2 vector = Vector2.zero;
		foreach (Vector3 vector2 in array)
		{
			vector.x += vector2.x;
			vector.y += vector2.y;
		}
		try
		{
			vector.x /= 4f;
			vector.y /= 4f;
			vector = RectTransformUtility.WorldToScreenPoint(null, vector);
			Vector2 vector3 = Input.mousePosition;
			if (usePivot == ZoomContent.ZoomPivot.playerMapPosition)
			{
				vector3 = RectTransformUtility.WorldToScreenPoint(null, MapController.Instance.playerCharacterRect.position);
			}
			Vector2 vector4 = Vector2.LerpUnclamped(vector, vector3, pivotBias);
			Vector2 zero = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.containerRect, vector4, null, ref zero);
			float num = Mathf.Clamp((zero.x + this.containerRect.sizeDelta.x * this.containerRect.pivot.x) / this.containerRect.sizeDelta.x, 0f, 1f);
			float num2 = Mathf.Clamp((zero.y + this.containerRect.sizeDelta.y * this.containerRect.pivot.y) / this.containerRect.sizeDelta.y, 0f, 1f);
			Vector2 vector5;
			vector5..ctor(num, num2);
			Vector2 vector6;
			vector6..ctor(this.containerRect.rect.size.x * this.containerRect.localScale.x, this.containerRect.rect.size.y * this.containerRect.localScale.y);
			Vector2 vector7 = this.containerRect.pivot - vector5;
			Vector3 vector8;
			vector8..ctor(vector7.x * vector6.x, vector7.y * vector6.y);
			this.containerRect.pivot = vector5;
			this.containerRect.localPosition -= vector8;
			this.scroll.SetAnchorPos(this.containerRect.anchoredPosition);
			this.scroll.ScrollZoom(vector8);
		}
		catch
		{
			Game.LogError("Unable to set pivot for zoom", 2);
		}
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x001D4848 File Offset: 0x001D2A48
	public void ResetPivot()
	{
		if (this.containerRect != null)
		{
			this.containerRect.pivot = new Vector2(0.5f, 0.5f);
		}
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x001D4874 File Offset: 0x001D2A74
	private void LateUpdate()
	{
		bool flag = false;
		try
		{
			if (this.viewportMouseOver.isOver || (this.enableInFirstPersonMap && MapController.Instance.displayFirstPerson && SessionData.Instance.play))
			{
				if (this.window == null)
				{
					if (!InterfaceController.Instance.desktopMode || !(InterfaceController.Instance.pinnedBeingDragged == null))
					{
						goto IL_E1;
					}
					if (InterfaceController.Instance.currentMouseOverElement.Count <= 0)
					{
						flag = true;
						goto IL_E1;
					}
					if (this.allowedMouseOverTags.Count <= 0)
					{
						goto IL_E1;
					}
					flag = true;
					using (List<MonoBehaviour>.Enumerator enumerator = InterfaceController.Instance.currentMouseOverElement.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MonoBehaviour monoBehaviour = enumerator.Current;
							if (!this.allowedMouseOverTags.Contains(monoBehaviour.transform.tag))
							{
								flag = false;
								break;
							}
						}
						goto IL_E1;
					}
				}
				flag = true;
			}
			IL_E1:
			if (flag && this.enableZoomWithMouseWheel && !InterfaceController.Instance.playerTextInputActive)
			{
				ZoomContent.ZoomPivot usePivot = ZoomContent.ZoomPivot.mousePosition;
				if (this.enableInFirstPersonMap && MapController.Instance.displayFirstPerson && SessionData.Instance.play)
				{
					usePivot = ZoomContent.ZoomPivot.playerMapPosition;
				}
				if (InputController.Instance.player.GetButtonDown("CaseBoard"))
				{
					if (!this.enableInFirstPersonMap && !InputController.Instance.mouseInputMode)
					{
						this.SetPivotPoint(0f, usePivot);
					}
					else
					{
						this.SetPivotPoint(1f, usePivot);
					}
				}
				if (Mathf.Abs(InputController.Instance.player.GetAxis(this.zoomAxis)) > 0.05f && this.axisInputDelay <= 0f)
				{
					float axis = InputController.Instance.player.GetAxis(this.zoomAxis);
					this.axisInputDelay = 0.1f;
					if (this.useZoomSteps)
					{
						int num;
						if (axis > 0f)
						{
							num = Mathf.Clamp(Mathf.CeilToInt(axis), -1, 1);
						}
						else
						{
							num = Mathf.Clamp(Mathf.FloorToInt(axis), -1, 1);
						}
						if (num == 0)
						{
							return;
						}
						float num2 = (this.zoomLimit.y - this.zoomLimit.x) / (float)this.numberOfSteps;
						this.desiredZoom += num2 * (float)num;
					}
					else
					{
						float num3 = 1f;
						if (!InputController.Instance.mouseInputMode)
						{
							num3 = this.controllerSensitivityMultiplier;
						}
						this.desiredZoom += axis * this.zoomSensitivity * num3;
					}
					this.desiredZoom = Mathf.Clamp(this.desiredZoom, this.zoomLimit.x, this.zoomLimit.y);
					int num4 = Mathf.RoundToInt(this.desiredZoom * 100f);
					this.desiredZoom = (float)num4 / 100f;
					if (this.desiredZoom != this.zoom)
					{
						float pivotBias = this.zoomToCursorPercentage;
						if (!InputController.Instance.mouseInputMode)
						{
							pivotBias = 0f;
						}
						if (axis < 0f)
						{
							pivotBias = 0f;
						}
						this.SetPivotPoint(pivotBias, usePivot);
						this.zoomProgress = 0f;
					}
				}
				if (this.axisInputDelay > 0f)
				{
					this.axisInputDelay -= Time.deltaTime;
				}
			}
			if (this.zoomProgress < 1f)
			{
				this.zoomProgress += this.smoothZoomSpeed * Time.smoothDeltaTime;
				this.zoomProgress = Mathf.Clamp01(this.zoomProgress);
				this.zoom = Mathf.Lerp(this.zoom, this.desiredZoom, this.zoomProgress);
				if (this.contentController != null)
				{
					this.zoom = Mathf.Clamp(this.zoom, Mathf.Max(this.zoomLimit.x, this.contentController.fitScale), this.zoomLimit.y);
				}
				this.normalizedZoom = this.GetNormalizedZoom(this.zoom);
				this.ApplyZoom(this.normalizedZoom);
			}
		}
		catch
		{
			Game.LogError("Viewport error!", 2);
		}
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x001D4C9C File Offset: 0x001D2E9C
	public float GetNormalizedZoom(float zoom)
	{
		return Mathf.Clamp01((zoom - this.zoomLimit.x) / (this.zoomLimit.y - this.zoomLimit.x));
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x001D4CC8 File Offset: 0x001D2EC8
	public void ApplyZoom(float normalizedZoom)
	{
		float num = Mathf.Clamp01(this.zoomCurve.Evaluate(normalizedZoom));
		num *= this.zoomLimit.y - this.zoomLimit.x;
		num += this.zoomLimit.x;
		num = (float)Mathf.RoundToInt(num * 100f) / 100f;
		if (this.containerRect != null)
		{
			this.containerRect.localScale = new Vector3(num, num, 1f);
		}
		foreach (RectTransform rectTransform in this.additionalRects)
		{
			if (rectTransform != null)
			{
				rectTransform.localScale = new Vector3(num, num, 1f);
			}
		}
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x001D4DA4 File Offset: 0x001D2FA4
	public void SetZoom(float newZoom)
	{
		this.zoom = Mathf.Clamp(newZoom, this.zoomLimit.x, this.zoomLimit.y);
		this.desiredZoom = this.zoom;
		this.normalizedZoom = this.GetNormalizedZoom(this.zoom);
		this.ApplyZoom(this.normalizedZoom);
	}

	// Token: 0x04002D18 RID: 11544
	[Tooltip("Which controller axis to poll")]
	[Header("Zoom settings")]
	public string zoomAxis = "EvidenceZoom";

	// Token: 0x04002D19 RID: 11545
	[Tooltip("Use controller input to affect zoom level")]
	public bool enableZoomWithMouseWheel = true;

	// Token: 0x04002D1A RID: 11546
	[Tooltip("Toggle true if this is the first person map")]
	public bool enableInFirstPersonMap;

	// Token: 0x04002D1B RID: 11547
	[Space(7f)]
	public bool useZoomSteps = true;

	// Token: 0x04002D1C RID: 11548
	[EnableIf("useZoomSteps")]
	public int numberOfSteps = 10;

	// Token: 0x04002D1D RID: 11549
	[DisableIf("useZoomSteps")]
	public float zoomSensitivity = 0.2f;

	// Token: 0x04002D1E RID: 11550
	[DisableIf("useZoomSteps")]
	public float controllerSensitivityMultiplier = 0.5f;

	// Token: 0x04002D1F RID: 11551
	[Space(7f)]
	[Tooltip("Scaling of the zoom level: Normalized values")]
	public AnimationCurve zoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002D20 RID: 11552
	[Tooltip("How fast this zooms in/out")]
	public float smoothZoomSpeed = 4f;

	// Token: 0x04002D21 RID: 11553
	[Tooltip("Min/max limits of the zoom")]
	public Vector2 zoomLimit = new Vector2(1f, 4f);

	// Token: 0x04002D22 RID: 11554
	[Tooltip("How much the centre of the viewpoint changes to the cursor position")]
	public float zoomToCursorPercentage = 1f;

	// Token: 0x04002D23 RID: 11555
	[Space(7f)]
	public float zoom = 1f;

	// Token: 0x04002D24 RID: 11556
	public float desiredZoom = 1f;

	// Token: 0x04002D25 RID: 11557
	public float normalizedZoom = 1f;

	// Token: 0x04002D26 RID: 11558
	[ReadOnly]
	public float zoomProgress = 1f;

	// Token: 0x04002D27 RID: 11559
	[ReadOnly]
	public Vector2 normalSize;

	// Token: 0x04002D28 RID: 11560
	[ReadOnly]
	public float axisInputDelay;

	// Token: 0x04002D29 RID: 11561
	[Space(7f)]
	[Tooltip("If the mouse is over one of these UI elements then allow zoom")]
	public List<string> allowedMouseOverTags = new List<string>();

	// Token: 0x04002D2A RID: 11562
	[Tooltip("Zoom these additional rectTransforms")]
	public List<RectTransform> additionalRects = new List<RectTransform>();

	// Token: 0x04002D2B RID: 11563
	[Header("References")]
	public InfoWindow window;

	// Token: 0x04002D2C RID: 11564
	public RectTransform containerRect;

	// Token: 0x04002D2D RID: 11565
	public CustomScrollRect scroll;

	// Token: 0x04002D2E RID: 11566
	public RectTransform scrollRectArea;

	// Token: 0x04002D2F RID: 11567
	public ViewportMouseOver viewportMouseOver;

	// Token: 0x04002D30 RID: 11568
	public WindowContentController contentController;

	// Token: 0x02000620 RID: 1568
	public enum ZoomPivot
	{
		// Token: 0x04002D32 RID: 11570
		mousePosition,
		// Token: 0x04002D33 RID: 11571
		playerMapPosition
	}
}
