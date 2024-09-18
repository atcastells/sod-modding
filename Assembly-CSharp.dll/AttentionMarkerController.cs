using System;
using TMPro;
using UnityEngine;

// Token: 0x020004CD RID: 1229
public class AttentionMarkerController : MonoBehaviour
{
	// Token: 0x06001A7F RID: 6783 RVA: 0x00185CFD File Offset: 0x00183EFD
	private void Start()
	{
		this.tmp = base.gameObject.GetComponentInChildren<TextMeshProUGUI>();
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x00185D10 File Offset: 0x00183F10
	public void SetText(string newText)
	{
		if (this.tmp == null)
		{
			this.tmp = base.gameObject.GetComponentInChildren<TextMeshProUGUI>();
		}
		this.tmp.text = newText;
	}

	// Token: 0x0400232E RID: 9006
	public TextMeshProUGUI tmp;
}
