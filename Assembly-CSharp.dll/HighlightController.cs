using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200061B RID: 1563
public class HighlightController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x060022B6 RID: 8886 RVA: 0x00002265 File Offset: 0x00000465
	private void Start()
	{
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x001D3C08 File Offset: 0x001D1E08
	public void SetSelectable(bool tf)
	{
		this.selectable = tf;
		if (this.selectable)
		{
			base.enabled = true;
			return;
		}
		base.enabled = false;
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x00002265 File Offset: 0x00000465
	private void OnDestroy()
	{
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x00002265 File Offset: 0x00000465
	public void OnPointerEnter(PointerEventData data)
	{
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x001D3C28 File Offset: 0x001D1E28
	public void OnPointerExit(PointerEventData data)
	{
		InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(data, base.transform);
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x00002265 File Offset: 0x00000465
	private void Update()
	{
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x00002265 File Offset: 0x00000465
	public void Hightlight()
	{
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x00002265 File Offset: 0x00000465
	public void Restore()
	{
	}

	// Token: 0x04002D04 RID: 11524
	[NonSerialized]
	public InfoWindow window;

	// Token: 0x04002D05 RID: 11525
	public string selectableType = string.Empty;

	// Token: 0x04002D06 RID: 11526
	public bool selectable;

	// Token: 0x04002D07 RID: 11527
	public bool highlighted;
}
