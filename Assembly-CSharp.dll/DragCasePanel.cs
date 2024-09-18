using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000617 RID: 1559
public class DragCasePanel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
{
	// Token: 0x0600228E RID: 8846 RVA: 0x001D3202 File Offset: 0x001D1402
	public void Setup(PinnedItemController newController)
	{
		this.itemController = newController;
		this.pinnedContainer = CasePanelController.Instance.pinnedContainer;
		this.panelRect = this.itemController.gameObject.GetComponent<RectTransform>();
		base.transform.SetAsLastSibling();
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x001D323C File Offset: 0x001D143C
	public void OnPointerDown(PointerEventData data)
	{
		this.panelRect.SetAsLastSibling();
		if (InputController.Instance.mouseInputMode)
		{
			if (data.button != null)
			{
				return;
			}
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.panelRect, data.position, data.pressEventCamera, ref this.pointerOffset);
		}
		else
		{
			this.pointerOffset = Vector2.zero;
		}
		string text = "Interface: Drag Case Panel Pointer Down: ";
		string name = this.panelRect.name;
		string text2 = " offset = ";
		Vector2 vector = this.pointerOffset;
		Game.Log(text + name + text2 + vector.ToString(), 2);
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			if (!(pinnedItemController == this.itemController.pinButtonController))
			{
				pinnedItemController.dragController.panelRect.SetAsLastSibling();
				if (InputController.Instance.mouseInputMode)
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle(pinnedItemController.dragController.panelRect, data.position, data.pressEventCamera, ref pinnedItemController.dragController.pointerOffset);
				}
				else
				{
					this.pointerOffset = Vector2.zero;
				}
			}
		}
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x001D3370 File Offset: 0x001D1570
	public void OnDrag(PointerEventData data)
	{
		if (data.button != null)
		{
			return;
		}
		this.ForceDrag(data.position);
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			if (!(pinnedItemController == this.itemController))
			{
				pinnedItemController.dragController.ForceDrag(data.position);
			}
		}
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x001D33F4 File Offset: 0x001D15F4
	public void ForceDrag(Vector2 cursorPosition)
	{
		if (this.panelRect == null)
		{
			return;
		}
		this.SetPositionCursor(cursorPosition, this.pointerOffset);
		for (int i = 0; i < this.pinnedFiles.Count; i++)
		{
			DragCasePanel dragCasePanel = this.pinnedFiles[i];
			Vector2 vector = this.offsets[i];
			Vector2 positionDirect;
			positionDirect..ctor(this.panelRect.localPosition.x - vector.x, this.panelRect.localPosition.y - vector.y);
			dragCasePanel.SetPositionDirect(positionDirect);
		}
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x001D3488 File Offset: 0x001D1688
	public void ForceDragController(Vector2 newLocalPosition)
	{
		if (this.panelRect == null)
		{
			return;
		}
		this.panelRect.SetAsLastSibling();
		this.SetPositionDirect(newLocalPosition);
		for (int i = 0; i < this.pinnedFiles.Count; i++)
		{
			DragCasePanel dragCasePanel = this.pinnedFiles[i];
			Vector2 vector = this.offsets[i];
			Vector2 positionDirect;
			positionDirect..ctor(this.panelRect.localPosition.x - vector.x, this.panelRect.localPosition.y - vector.y);
			dragCasePanel.SetPositionDirect(positionDirect);
		}
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x001D3520 File Offset: 0x001D1720
	private Vector2 ClampCursor(Vector2 rawPointerPosition)
	{
		Vector3[] array = new Vector3[4];
		this.pinnedContainer.GetWorldCorners(array);
		float num = Mathf.Clamp(rawPointerPosition.x, array[0].x, array[2].x);
		float num2 = Mathf.Clamp(rawPointerPosition.y, array[0].y, array[2].y);
		return new Vector2(num, num2);
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x001D3590 File Offset: 0x001D1790
	public void SetPositionCursor(Vector2 pointerPosition, Vector2 offset)
	{
		pointerPosition = this.ClampCursor(pointerPosition);
		Vector2 vector;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.pinnedContainer, pointerPosition, null, ref vector))
		{
			this.SetPositionDirect(vector - this.pointerOffset);
		}
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x001D35C9 File Offset: 0x001D17C9
	public void SetPositionDirect(Vector2 localPosition)
	{
		this.panelRect.localPosition = this.ClampToCorkboard(localPosition);
		this.itemController.OnMoveThis();
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x001D35F0 File Offset: 0x001D17F0
	private Vector2 ClampToCorkboard(Vector2 original)
	{
		float num = Mathf.Clamp(original.x, this.pinnedContainer.rect.xMin, this.pinnedContainer.rect.xMax);
		float num2 = Mathf.Clamp(original.y, this.pinnedContainer.rect.yMin, this.pinnedContainer.rect.yMax);
		return new Vector2(num, num2);
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x001D3668 File Offset: 0x001D1868
	private Vector2 RadiusClamp(Vector2 original, Vector2 point, float radius)
	{
		Vector2 result = original;
		if (Vector2.Distance(original, point) > radius)
		{
			result = Vector2.MoveTowards(point, original, radius);
		}
		return result;
	}

	// Token: 0x04002CF0 RID: 11504
	private Vector2 pointerOffset;

	// Token: 0x04002CF1 RID: 11505
	public RectTransform pinnedContainer;

	// Token: 0x04002CF2 RID: 11506
	public RectTransform panelRect;

	// Token: 0x04002CF3 RID: 11507
	public PinnedItemController itemController;

	// Token: 0x04002CF4 RID: 11508
	public bool multipleParentInstances;

	// Token: 0x04002CF5 RID: 11509
	private List<DragCasePanel> pinnedFiles = new List<DragCasePanel>();

	// Token: 0x04002CF6 RID: 11510
	public List<Vector2> offsets = new List<Vector2>();
}
