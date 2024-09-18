using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200061C RID: 1564
public class ResizePanel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x060022BF RID: 8895 RVA: 0x001D3C50 File Offset: 0x001D1E50
	public void OnPointerDown(PointerEventData data)
	{
		if (this.controller == null)
		{
			this.controller = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.controller.rect.SetAsLastSibling();
		this.controller.SetPivot(this.pivot);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.controller.rect, data.position, data.pressEventCamera, ref this.previousPointerPosition);
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x001B23D5 File Offset: 0x001B05D5
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		InterfaceController.Instance.AddMouseOverElement(this);
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x001D3CC0 File Offset: 0x001D1EC0
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		if (!this.resizingActive)
		{
			InterfaceController.Instance.RemoveMouseOverElement(this);
		}
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x001D3CE9 File Offset: 0x001D1EE9
	private void OnDisable()
	{
		this.resizingActive = false;
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x001D3CE9 File Offset: 0x001D1EE9
	public void OnEndDrag(PointerEventData data)
	{
		this.resizingActive = false;
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x001D3D00 File Offset: 0x001D1F00
	public void OnDrag(PointerEventData data)
	{
		if (this.controller == null)
		{
			this.controller = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		if (this.controller.rect == null)
		{
			return;
		}
		this.resizingActive = true;
		InterfaceController.Instance.AddMouseOverElement(this);
		Vector2 vector = this.controller.rect.sizeDelta;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.controller.rect, data.position, data.pressEventCamera, ref this.currentPointerPosition);
		Vector2 vector2 = this.currentPointerPosition - this.previousPointerPosition;
		vector += new Vector2(vector2.x * (1f - this.pivot.x * 2f), vector2.y * (1f - this.pivot.y * 2f));
		vector..ctor(Mathf.Clamp(vector.x, this.controller.preset.minSize.x, this.controller.preset.maxSize.x), Mathf.Clamp(vector.y, this.controller.preset.minSize.y, this.controller.preset.maxSize.y));
		this.controller.rect.sizeDelta = vector;
		this.controller.OnResizeWindow();
		this.previousPointerPosition = this.currentPointerPosition;
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x0018E998 File Offset: 0x0018CB98
	private void OnDestroy()
	{
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x04002D08 RID: 11528
	public InfoWindow controller;

	// Token: 0x04002D09 RID: 11529
	public bool resizingActive;

	// Token: 0x04002D0A RID: 11530
	private Vector2 currentPointerPosition;

	// Token: 0x04002D0B RID: 11531
	private Vector2 previousPointerPosition;

	// Token: 0x04002D0C RID: 11532
	public Vector2 pivot;
}
