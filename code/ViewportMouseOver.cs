using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000595 RID: 1429
public class ViewportMouseOver : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06001F33 RID: 7987 RVA: 0x001B019E File Offset: 0x001AE39E
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			this.isOver = true;
		}
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x001B01B3 File Offset: 0x001AE3B3
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		if (InputController.Instance.mouseInputMode)
		{
			this.isOver = false;
		}
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x001B01DC File Offset: 0x001AE3DC
	public void ForceMouseOver(bool val)
	{
		this.isOver = val;
	}

	// Token: 0x0400290E RID: 10510
	public bool isOver;
}
