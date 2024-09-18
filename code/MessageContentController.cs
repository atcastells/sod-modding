using System;
using TMPro;
using UnityEngine;

// Token: 0x02000582 RID: 1410
public class MessageContentController : MonoBehaviour
{
	// Token: 0x06001ED9 RID: 7897 RVA: 0x001ACF5C File Offset: 0x001AB15C
	private void Awake()
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
	}

	// Token: 0x040028AC RID: 10412
	public TextMeshProUGUI messageText;

	// Token: 0x040028AD RID: 10413
	public ProgressBarController progressBar;

	// Token: 0x040028AE RID: 10414
	public RectTransform minigame;

	// Token: 0x040028AF RID: 10415
	public RectTransform rect;
}
