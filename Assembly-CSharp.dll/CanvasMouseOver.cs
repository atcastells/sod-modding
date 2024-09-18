using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020004CE RID: 1230
public class CanvasMouseOver : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06001A82 RID: 6786 RVA: 0x00185D3D File Offset: 0x00183F3D
	public static CanvasMouseOver Instance
	{
		get
		{
			return CanvasMouseOver._instance;
		}
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x00185D44 File Offset: 0x00183F44
	private void Awake()
	{
		if (CanvasMouseOver._instance != null && CanvasMouseOver._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CanvasMouseOver._instance = this;
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x00185D74 File Offset: 0x00183F74
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject != null)
		{
			this.currentHover = eventData.pointerCurrentRaycast.gameObject;
		}
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x00185DAB File Offset: 0x00183FAB
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.currentHover = null;
	}

	// Token: 0x0400232F RID: 9007
	private static CanvasMouseOver _instance;

	// Token: 0x04002330 RID: 9008
	public GameObject currentHover;
}
