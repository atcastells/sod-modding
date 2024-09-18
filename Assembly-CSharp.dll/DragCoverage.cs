using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020005A5 RID: 1445
public class DragCoverage : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x1400003D RID: 61
	// (add) Token: 0x06001F87 RID: 8071 RVA: 0x001B2348 File Offset: 0x001B0548
	// (remove) Token: 0x06001F88 RID: 8072 RVA: 0x001B2380 File Offset: 0x001B0580
	public event DragCoverage.OnDragCoverage OnDragged;

	// Token: 0x06001F89 RID: 8073 RVA: 0x001B23B5 File Offset: 0x001B05B5
	public void OnPointerDown(PointerEventData data)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.parentRect, data.position, data.pressEventCamera, ref this.previousPointerPosition);
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x001B23D5 File Offset: 0x001B05D5
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		InterfaceController.Instance.AddMouseOverElement(this);
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x001B1782 File Offset: 0x001AF982
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x001B23E4 File Offset: 0x001B05E4
	public void OnDrag(PointerEventData data)
	{
		Vector2 vector = this.parentRect.sizeDelta;
		InterfaceController.Instance.AddMouseOverElement(this);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.parentRect, data.position, data.pressEventCamera, ref this.currentPointerPosition);
		Vector2 vector2 = this.currentPointerPosition - this.previousPointerPosition;
		vector += new Vector2(vector2.x, 0f);
		vector..ctor(Mathf.Clamp(vector.x, this.sizeRange.x, this.sizeRange.y), vector.y);
		this.SetSize(vector.x);
		this.previousPointerPosition = this.currentPointerPosition;
		if (this.OnDragged != null)
		{
			this.OnDragged();
		}
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x0018E998 File Offset: 0x0018CB98
	private void OnDestroy()
	{
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x06001F8E RID: 8078 RVA: 0x001B24A8 File Offset: 0x001B06A8
	public void SetSize(float newSize)
	{
		this.parentRect.sizeDelta = new Vector2(newSize, this.parentRect.sizeDelta.y);
	}

	// Token: 0x0400297C RID: 10620
	public RectTransform parentRect;

	// Token: 0x0400297D RID: 10621
	private Vector2 currentPointerPosition;

	// Token: 0x0400297E RID: 10622
	private Vector2 previousPointerPosition;

	// Token: 0x0400297F RID: 10623
	public Vector2 pivot;

	// Token: 0x04002980 RID: 10624
	public Vector2 sizeRange = new Vector2(400f, 1730f);

	// Token: 0x04002981 RID: 10625
	public float edgeBuffer = 10f;

	// Token: 0x020005A6 RID: 1446
	// (Invoke) Token: 0x06001F91 RID: 8081
	public delegate void OnDragCoverage();
}
