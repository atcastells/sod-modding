using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004EE RID: 1262
public class ControlGraphicController : MonoBehaviour
{
	// Token: 0x06001B48 RID: 6984 RVA: 0x00002265 File Offset: 0x00000465
	private void OnEnable()
	{
	}

	// Token: 0x040023F6 RID: 9206
	public Image img;

	// Token: 0x040023F7 RID: 9207
	public TextMeshProUGUI controlText;

	// Token: 0x040023F8 RID: 9208
	public ControlGraphicController.ControlGraphicType controlType;

	// Token: 0x040023F9 RID: 9209
	public string trackControl;

	// Token: 0x040023FA RID: 9210
	public string buttonStr;

	// Token: 0x020004EF RID: 1263
	public enum ControlGraphicType
	{
		// Token: 0x040023FC RID: 9212
		keyboard,
		// Token: 0x040023FD RID: 9213
		mouse,
		// Token: 0x040023FE RID: 9214
		controller
	}
}
