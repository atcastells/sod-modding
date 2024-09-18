using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000502 RID: 1282
public class ForceMouseOverInput : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06001BAB RID: 7083 RVA: 0x0018E95C File Offset: 0x0018CB5C
	public void OnPointerEnter(PointerEventData eventData)
	{
		InterfaceController.Instance.AddMouseOverElement(this);
		this.mouseOver = true;
	}

	// Token: 0x06001BAC RID: 7084 RVA: 0x0018E970 File Offset: 0x0018CB70
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		InterfaceController.Instance.RemoveMouseOverElement(this);
		this.mouseOver = false;
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x0018E998 File Offset: 0x0018CB98
	private void OnDestroy()
	{
		InterfaceController.Instance.RemoveMouseOverElement(this);
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x0018E9A5 File Offset: 0x0018CBA5
	private void OnDisable()
	{
		if (InterfaceController.Instance != null)
		{
			InterfaceController.Instance.RemoveMouseOverElement(this);
			this.mouseOver = false;
		}
	}

	// Token: 0x0400245D RID: 9309
	public int cursorType;

	// Token: 0x0400245E RID: 9310
	public bool mouseOver;
}
