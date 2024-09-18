using System;
using UnityEngine;

// Token: 0x020005FB RID: 1531
public class SwatchController : ButtonController
{
	// Token: 0x060021B8 RID: 8632 RVA: 0x001CA3C5 File Offset: 0x001C85C5
	public void Setup(Color newColor, ColourPickerController newController)
	{
		this.controller = newController;
		this.SetButtonBaseColour(newColor);
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x001CA3D5 File Offset: 0x001C85D5
	public override void OnLeftClick()
	{
		this.controller.OnPickNewColour(this);
		base.OnLeftClick();
	}

	// Token: 0x04002C29 RID: 11305
	private ColourPickerController controller;
}
