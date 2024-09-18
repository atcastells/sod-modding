using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000618 RID: 1560
public class DragPanel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IEndDragHandler, IDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	// Token: 0x1400004B RID: 75
	// (add) Token: 0x06002299 RID: 8857 RVA: 0x001D36AC File Offset: 0x001D18AC
	// (remove) Token: 0x0600229A RID: 8858 RVA: 0x001D36E4 File Offset: 0x001D18E4
	public event DragPanel.DragEnd OnDragEnd;

	// Token: 0x0600229B RID: 8859 RVA: 0x001D371C File Offset: 0x001D191C
	private void Start()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		foreach (Image image in base.gameObject.transform.parent.GetComponentsInChildren<Image>())
		{
			if (image.raycastTarget)
			{
				this.rayTargets.Add(image);
			}
		}
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x001B23D5 File Offset: 0x001B05D5
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		InterfaceController.Instance.AddMouseOverElement(this);
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x001B1782 File Offset: 0x001AF982
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x0018E998 File Offset: 0x0018CB98
	private void OnDestroy()
	{
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x001D3784 File Offset: 0x001D1984
	public virtual void OnPointerDown(PointerEventData data)
	{
		this.parentRect.SetAsLastSibling();
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.parentRect, data.position, data.pressEventCamera, ref this.pointerOffset);
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x001D37AF File Offset: 0x001D19AF
	public virtual void OnBeginDrag(PointerEventData data)
	{
		if (this.draggableComponent)
		{
			this.EndDrag();
			base.StartCoroutine("Drag", data);
		}
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x0018E998 File Offset: 0x0018CB98
	public virtual void OnEndDrag(PointerEventData data)
	{
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x001D37CC File Offset: 0x001D19CC
	public virtual void OnDrag(PointerEventData data)
	{
		if (InterfaceController.Instance.desktopMode)
		{
			if (this.parentRect == null)
			{
				return;
			}
			InterfaceController.Instance.AddMouseOverElement(this);
			Vector2 vector = this.ClampToWindow(data);
			Vector2 vector2;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceControls.Instance.hudCanvasRect, vector, data.pressEventCamera, ref vector2))
			{
				if (this.parentWindow != null)
				{
					Vector2 anchoredPosition;
					anchoredPosition..ctor(vector2.x - this.pointerOffset.x + InterfaceControls.Instance.hudCanvasRect.rect.width * 0.5f, vector2.y - this.pointerOffset.y - InterfaceControls.Instance.hudCanvasRect.rect.height * 0.5f);
					this.parentWindow.SetAnchoredPosition(anchoredPosition);
					return;
				}
				Vector2 anchoredPosition2;
				anchoredPosition2..ctor(vector2.x - this.pointerOffset.x, vector2.y - this.pointerOffset.y);
				this.parentRect.anchoredPosition = anchoredPosition2;
			}
		}
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x001D38E1 File Offset: 0x001D1AE1
	private IEnumerator Drag(PointerEventData data)
	{
		if (InterfaceController.Instance.desktopMode)
		{
			this.isDragging = true;
			InterfaceController.Instance.AddMouseOverElement(this);
			GameObject gameObject = base.gameObject;
			Vector2 zero = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.parentRect, Input.mousePosition, null, ref zero);
			InterfaceController.Instance.SetDragged(gameObject, this.dragTag, zero);
			using (List<Image>.Enumerator enumerator = this.rayTargets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Image image = enumerator.Current;
					try
					{
						image.raycastTarget = false;
					}
					catch
					{
					}
				}
				goto IL_CF;
			}
			IL_B8:
			yield return null;
			IL_CF:
			if (Input.GetMouseButton(0))
			{
				goto IL_B8;
			}
		}
		this.EndDrag();
		yield break;
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x001D38F0 File Offset: 0x001D1AF0
	public virtual void EndDrag()
	{
		if (this.OnDragEnd != null)
		{
			this.OnDragEnd(InterfaceController.Instance.dragged, InterfaceController.Instance.draggedTag);
		}
		InterfaceController.Instance.RemoveMouseOverElement(this);
		InterfaceController.Instance.SetDragged(null, "", Vector2.zero);
		base.StopCoroutine("Drag");
		this.isDragging = false;
		foreach (Image image in this.rayTargets)
		{
			if (!(image == null))
			{
				image.raycastTarget = true;
			}
		}
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x001D39A8 File Offset: 0x001D1BA8
	public Vector2 ClampToWindow(PointerEventData data)
	{
		Vector2 position = data.position;
		Vector3[] array = new Vector3[4];
		InterfaceControls.Instance.hudCanvasRect.GetWorldCorners(array);
		float num = Mathf.Clamp(position.x, array[0].x, array[2].x);
		float num2 = Mathf.Clamp(position.y, array[0].y, array[2].y);
		return new Vector2(num, num2);
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x001D3A20 File Offset: 0x001D1C20
	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == null)
		{
			if (Time.time - this.lastLeftClick <= InterfaceControls.Instance.doubleClickDelay)
			{
				this.OnLeftClick();
				this.OnLeftDoubleClick();
			}
			else
			{
				this.OnLeftClick();
			}
			this.lastLeftClick = Time.time;
			return;
		}
		if (eventData.button == 1)
		{
			if (Time.time - this.lastRightClick <= InterfaceControls.Instance.doubleClickDelay)
			{
				this.OnRightClick();
				this.OnRightDoubleClick();
			}
			else
			{
				this.OnRightClick();
			}
			this.lastRightClick = Time.time;
		}
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnLeftClick()
	{
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnRightClick()
	{
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x001D3AAD File Offset: 0x001D1CAD
	public virtual void OnLeftDoubleClick()
	{
		if (this.parentWindow != null)
		{
			this.parentWindow.Rename();
		}
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnRightDoubleClick()
	{
	}

	// Token: 0x04002CF7 RID: 11511
	public Vector2 pointerOffset;

	// Token: 0x04002CF8 RID: 11512
	public RectTransform parentRect;

	// Token: 0x04002CF9 RID: 11513
	public InfoWindow parentWindow;

	// Token: 0x04002CFA RID: 11514
	public bool draggableComponent;

	// Token: 0x04002CFB RID: 11515
	public string dragTag = string.Empty;

	// Token: 0x04002CFC RID: 11516
	public bool isDragging;

	// Token: 0x04002CFD RID: 11517
	private float lastLeftClick;

	// Token: 0x04002CFE RID: 11518
	private float lastRightClick;

	// Token: 0x04002D00 RID: 11520
	private List<Image> rayTargets = new List<Image>();

	// Token: 0x02000619 RID: 1561
	// (Invoke) Token: 0x060022AD RID: 8877
	public delegate void DragEnd(GameObject dragObj, string tag);
}
