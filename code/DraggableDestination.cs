using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020004FC RID: 1276
public class DraggableDestination : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x1400002D RID: 45
	// (add) Token: 0x06001B7C RID: 7036 RVA: 0x0018D600 File Offset: 0x0018B800
	// (remove) Token: 0x06001B7D RID: 7037 RVA: 0x0018D638 File Offset: 0x0018B838
	public event DraggableDestination.DragDestination OnDragged;

	// Token: 0x06001B7E RID: 7038 RVA: 0x0018D66D File Offset: 0x0018B86D
	private void Awake()
	{
		this.graphic = base.gameObject.GetComponent<Image>();
		this.but = base.gameObject.GetComponent<Button>();
		this.originalColour = this.graphic.color;
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x0018D6A4 File Offset: 0x0018B8A4
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.but != null && !this.but.interactable)
		{
			this.isOver = false;
			return;
		}
		if (InterfaceController.Instance.dragged != null)
		{
			bool flag = false;
			if (InterfaceController.Instance.draggedTag.Length > 0 && this.acceptedTags.Contains(InterfaceController.Instance.draggedTag))
			{
				flag = true;
			}
			if (flag)
			{
				this.isOver = true;
				if (this.useHoverColours)
				{
					this.graphic.color = this.hoverAcceptColour;
				}
			}
		}
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x0018D736 File Offset: 0x0018B936
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.isOver = false;
		if (this.useHoverColours)
		{
			this.graphic.color = this.originalColour;
		}
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x0018D76C File Offset: 0x0018B96C
	private void Update()
	{
		if (this.isOver && Input.GetMouseButtonUp(0))
		{
			if (this.OnDragged != null)
			{
				this.OnDragged(InterfaceController.Instance.dragged, InterfaceController.Instance.draggedTag);
			}
			if (this.useHoverColours)
			{
				this.graphic.color = this.originalColour;
			}
		}
	}

	// Token: 0x04002436 RID: 9270
	public bool isOver;

	// Token: 0x04002437 RID: 9271
	public Button but;

	// Token: 0x04002439 RID: 9273
	public List<string> acceptedTags = new List<string>();

	// Token: 0x0400243A RID: 9274
	private Image graphic;

	// Token: 0x0400243B RID: 9275
	public Color originalColour = Color.white;

	// Token: 0x0400243C RID: 9276
	public bool useHoverColours = true;

	// Token: 0x0400243D RID: 9277
	public Color hoverAcceptColour = Color.yellow;

	// Token: 0x020004FD RID: 1277
	// (Invoke) Token: 0x06001B84 RID: 7044
	public delegate void DragDestination(GameObject dragObj, string tag);
}
