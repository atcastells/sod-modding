using System;
using Rewired.Demos;
using TMPro;
using UnityEngine;

// Token: 0x0200053A RID: 1338
public class RemapController : MonoBehaviour
{
	// Token: 0x06001D32 RID: 7474 RVA: 0x0019E790 File Offset: 0x0019C990
	public void OnSetAlternateButton()
	{
		Game.Log("Menu: OnSetAlternateButton...", 2);
		SimpleControlRemappingSOD.Instance.OnInputFieldClicked(this);
	}

	// Token: 0x040026EF RID: 9967
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x040026F0 RID: 9968
	public ButtonController primaryControlButton;

	// Token: 0x040026F1 RID: 9969
	public TextMeshProUGUI controlDescriptionText;

	// Token: 0x040026F2 RID: 9970
	public TextMeshProUGUI primaryText;
}
