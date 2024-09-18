using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200061E RID: 1566
public class WindowFocusController : ForceMouseOverInput, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x060022D2 RID: 8914 RVA: 0x001D4478 File Offset: 0x001D2678
	private void Update()
	{
		if (InputController.Instance.mouseInputMode && this.mouseOver && (InputController.Instance.player.GetButtonDown("Select") || Input.GetMouseButtonDown(0)))
		{
			InterfaceController.Instance.RemoveWindowFocus();
			InterfaceController.Instance.RemoveMouseOverElement(this);
		}
	}
}
