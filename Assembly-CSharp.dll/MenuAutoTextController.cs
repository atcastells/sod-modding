using System;
using TMPro;
using UnityEngine;

// Token: 0x02000541 RID: 1345
public class MenuAutoTextController : MonoBehaviour
{
	// Token: 0x06001D55 RID: 7509 RVA: 0x0019EECC File Offset: 0x0019D0CC
	private void Start()
	{
		TextMeshProUGUI component = base.gameObject.GetComponent<TextMeshProUGUI>();
		if (component != null && component.text.Length > 0)
		{
			component.text = Strings.Get("ui.interface", component.text, Strings.Casing.asIs, false, false, false, null);
		}
	}
}
