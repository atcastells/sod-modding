using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005BA RID: 1466
public class MapPinButtonController : ButtonController
{
	// Token: 0x06002022 RID: 8226 RVA: 0x001BA5BC File Offset: 0x001B87BC
	public void Setup(InfoWindow newWindow)
	{
		this.evWindow = newWindow;
		base.SetupReferences();
		this.pin = base.gameObject.GetComponent<RawImage>();
		this.canvasRend = base.gameObject.GetComponent<CanvasRenderer>();
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x001BA5F0 File Offset: 0x001B87F0
	public override void OnLeftDoubleClick()
	{
		InterfaceController.Instance.SpawnWindow(this.evWindow.passedEvidence, Evidence.DataKey.name, this.evWindow.passedKeys, "", false, true, default(Vector2), null, null, null, true);
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x001BA633 File Offset: 0x001B8833
	public override void OnHoverStart()
	{
		this.tooltip.mainText = this.evWindow.passedEvidence.GetNameForDataKey(this.evWindow.passedKeys);
	}

	// Token: 0x04002A26 RID: 10790
	public RawImage pin;

	// Token: 0x04002A27 RID: 10791
	public InfoWindow evWindow;

	// Token: 0x04002A28 RID: 10792
	public CanvasRenderer canvasRend;
}
