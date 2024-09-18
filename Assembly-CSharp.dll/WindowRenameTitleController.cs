using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200059A RID: 1434
public class WindowRenameTitleController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x06001F50 RID: 8016 RVA: 0x001B1768 File Offset: 0x001AF968
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if (this.inputField.isFocused)
		{
			InterfaceController.Instance.AddMouseOverElement(this);
		}
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x001B1782 File Offset: 0x001AF982
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x0018E998 File Offset: 0x0018CB98
	private void OnDestroy()
	{
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x001B17A4 File Offset: 0x001AF9A4
	public void OnPointerClick(PointerEventData eventData)
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

	// Token: 0x06001F54 RID: 8020 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnLeftClick()
	{
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnRightClick()
	{
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x001B1831 File Offset: 0x001AFA31
	public virtual void OnLeftDoubleClick()
	{
		this.window.Rename();
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnRightDoubleClick()
	{
	}

	// Token: 0x0400292B RID: 10539
	public InfoWindow window;

	// Token: 0x0400292C RID: 10540
	public TMP_InputField inputField;

	// Token: 0x0400292D RID: 10541
	private float lastLeftClick;

	// Token: 0x0400292E RID: 10542
	private float lastRightClick;
}
