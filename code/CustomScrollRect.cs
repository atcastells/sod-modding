using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020004F6 RID: 1270
public class CustomScrollRect : ScrollRect
{
	// Token: 0x06001B56 RID: 6998 RVA: 0x0018CD68 File Offset: 0x0018AF68
	public void ScrollZoom(Vector2 deltaPos)
	{
		this.m_ContentStartPosition -= deltaPos;
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x0018CD7C File Offset: 0x0018AF7C
	public override void OnBeginDrag(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			if (this.rightMouseScroll && eventData.button == 1)
			{
				eventData.button = 0;
				base.OnBeginDrag(eventData);
				this.isScrolling = true;
				if (InterfaceController.Instance != null)
				{
					InterfaceController.Instance.AddMouseOverElement(this);
					return;
				}
			}
			else if (this.leftMouseScroll)
			{
				base.OnBeginDrag(eventData);
				this.isScrolling = true;
				if (InterfaceController.Instance != null)
				{
					InterfaceController.Instance.AddMouseOverElement(this);
				}
			}
		}
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x0018CE04 File Offset: 0x0018B004
	public override void OnEndDrag(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			if (this.rightMouseScroll && eventData.button == 1)
			{
				eventData.button = 0;
				base.OnEndDrag(eventData);
				this.isScrolling = false;
				InterfaceController.Instance.RemoveMouseOverElement(this);
				return;
			}
			if (this.leftMouseScroll)
			{
				base.OnEndDrag(eventData);
				this.isScrolling = false;
				InterfaceController.Instance.RemoveMouseOverElement(this);
			}
		}
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x0018CE70 File Offset: 0x0018B070
	public override void OnDrag(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			if (this.rightMouseScroll && eventData.button == 1)
			{
				eventData.button = 0;
				base.OnDrag(eventData);
				this.isScrolling = true;
				return;
			}
			if (this.leftMouseScroll)
			{
				base.OnDrag(eventData);
				this.isScrolling = true;
			}
		}
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x0018CEC6 File Offset: 0x0018B0C6
	public void SetAnchorPos(Vector2 position)
	{
		base.SetContentAnchoredPosition(position);
	}

	// Token: 0x0400241E RID: 9246
	public bool rightMouseScroll = true;

	// Token: 0x0400241F RID: 9247
	public bool leftMouseScroll;

	// Token: 0x04002420 RID: 9248
	public bool isScrolling;
}
