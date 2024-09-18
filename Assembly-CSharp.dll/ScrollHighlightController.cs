using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200003E RID: 62
public class ScrollHighlightController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06000289 RID: 649 RVA: 0x0001974B File Offset: 0x0001794B
	public void OnPointerEnter(PointerEventData data)
	{
		CameraController.Instance.NewHighlightScroll(this.scrollPositionPathmap);
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0001975D File Offset: 0x0001795D
	public void OnPointerExit(PointerEventData data)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(data, base.transform))
		{
			return;
		}
		CameraController.Instance.CancelHighlightScroll();
	}

	// Token: 0x040001CF RID: 463
	public Vector2 scrollPositionPathmap = Vector2.zero;
}
