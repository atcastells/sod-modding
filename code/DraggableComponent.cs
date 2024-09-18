using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020004F8 RID: 1272
public class DraggableComponent : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x1400002C RID: 44
	// (add) Token: 0x06001B60 RID: 7008 RVA: 0x0018D16C File Offset: 0x0018B36C
	// (remove) Token: 0x06001B61 RID: 7009 RVA: 0x0018D1A4 File Offset: 0x0018B3A4
	public event DraggableComponent.DragEnd OnDragEnd;

	// Token: 0x06001B62 RID: 7010 RVA: 0x0018D1D9 File Offset: 0x0018B3D9
	private void Start()
	{
		this.thisRect = base.gameObject.GetComponent<RectTransform>();
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x0018D1EC File Offset: 0x0018B3EC
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.isOver = true;
		base.StartCoroutine("MouseOver");
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x0018D201 File Offset: 0x0018B401
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.isOver = false;
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x0018D21E File Offset: 0x0018B41E
	private IEnumerator MouseOver()
	{
		while (this.isOver)
		{
			if (Input.GetMouseButton(0))
			{
				if (!this.recClick)
				{
					this.originalClickPoint = Input.mousePosition;
					this.recClick = true;
					this.dragThresholdCheck = 0f;
				}
				else
				{
					this.dragThresholdCheck = Mathf.Abs(Input.mousePosition.x - this.originalClickPoint.x) + Mathf.Abs(Input.mousePosition.y - this.originalClickPoint.y);
				}
				if (!this.isDragging && this.dragThresholdCheck > 10f && InterfaceController.Instance.dragged == null)
				{
					this.EndDrag();
					base.StartCoroutine("Drag");
				}
			}
			else
			{
				this.recClick = false;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x0018D22D File Offset: 0x0018B42D
	private IEnumerator Drag()
	{
		this.isDragging = true;
		GameObject gameObject = base.gameObject;
		if (this.objectOverride != null)
		{
			gameObject = this.objectOverride;
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.thisRect, Input.mousePosition, null, ref this.pointerOffset);
		InterfaceController.Instance.SetDragged(gameObject, this.dragTag, this.pointerOffset);
		this.spawnedObject = Object.Instantiate<GameObject>(this.dragObject, this.thisRect.parent, false);
		this.rect = this.spawnedObject.GetComponent<RectTransform>();
		this.rect.localScale = this.thisRect.localScale;
		this.rect.sizeDelta = this.thisRect.sizeDelta;
		this.rect.offsetMax = this.thisRect.offsetMax;
		this.rect.offsetMin = this.thisRect.offsetMin;
		this.rect.anchorMax = this.thisRect.anchorMax;
		this.rect.anchorMin = this.thisRect.anchorMin;
		this.rect.pivot = this.thisRect.pivot;
		this.rect.position = this.thisRect.position;
		this.rect.SetParent(InterfaceControls.Instance.hudCanvasRect, true);
		this.rect.SetAsLastSibling();
		while (Input.GetMouseButton(0))
		{
			this.SetPosition(Input.mousePosition, this.pointerOffset);
			yield return null;
		}
		this.EndDrag();
		yield break;
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x0018D23C File Offset: 0x0018B43C
	private void OnDestory()
	{
		this.EndDrag();
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x0018D23C File Offset: 0x0018B43C
	private void OnDisable()
	{
		this.EndDrag();
	}

	// Token: 0x06001B69 RID: 7017 RVA: 0x0018D244 File Offset: 0x0018B444
	public void EndDrag()
	{
		if (this.OnDragEnd != null)
		{
			this.OnDragEnd(InterfaceController.Instance.dragged, InterfaceController.Instance.draggedTag);
		}
		InterfaceController.Instance.SetDragged(null, "", Vector2.zero);
		Object.Destroy(this.spawnedObject);
		base.StopCoroutine("Drag");
		this.isDragging = false;
		this.recClick = false;
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x0018D2B4 File Offset: 0x0018B4B4
	public void SetPosition(Vector2 pointerPosition, Vector2 offset)
	{
		Vector2 vector;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceControls.Instance.hudCanvasRect, pointerPosition, null, ref vector))
		{
			this.rect.localPosition = vector - offset;
		}
	}

	// Token: 0x04002423 RID: 9251
	private RectTransform thisRect;

	// Token: 0x04002424 RID: 9252
	public bool isDragging;

	// Token: 0x04002425 RID: 9253
	public bool isOver;

	// Token: 0x04002426 RID: 9254
	public GameObject dragObject;

	// Token: 0x04002427 RID: 9255
	private GameObject spawnedObject;

	// Token: 0x04002428 RID: 9256
	private RectTransform rect;

	// Token: 0x04002429 RID: 9257
	public Vector2 pointerOffset;

	// Token: 0x0400242A RID: 9258
	private Vector2 originalClickPoint;

	// Token: 0x0400242B RID: 9259
	private bool recClick;

	// Token: 0x0400242C RID: 9260
	private float dragThresholdCheck;

	// Token: 0x0400242D RID: 9261
	public GameObject objectOverride;

	// Token: 0x0400242E RID: 9262
	public string dragTag = string.Empty;

	// Token: 0x020004F9 RID: 1273
	// (Invoke) Token: 0x06001B6D RID: 7021
	public delegate void DragEnd(GameObject dragObj, string tag);
}
