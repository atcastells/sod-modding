using System;
using TMPro;
using UnityEngine;

// Token: 0x02000561 RID: 1377
public class CountController : MonoBehaviour
{
	// Token: 0x06001DF0 RID: 7664 RVA: 0x001A48BC File Offset: 0x001A2ABC
	private void Awake()
	{
		this.countText = base.gameObject.GetComponentInChildren<TextMeshProUGUI>();
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.VisibilityCheck();
	}

	// Token: 0x06001DF1 RID: 7665 RVA: 0x001A48E8 File Offset: 0x001A2AE8
	public void SetCount(int newVal)
	{
		if (this.countText == null)
		{
			this.countText = base.gameObject.GetComponentInChildren<TextMeshProUGUI>();
		}
		this.count = Mathf.Min(newVal, 99);
		this.countText.text = this.count.ToString();
		if (this.count >= 10)
		{
			this.countText.fontSize = 10f;
		}
		else
		{
			this.countText.fontSize = 15f;
		}
		this.VisibilityCheck();
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x001A496A File Offset: 0x001A2B6A
	public void VisibilityCheck()
	{
		if (this.invisibleIfZero)
		{
			if (this.count <= 0)
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x040027C1 RID: 10177
	public RectTransform rect;

	// Token: 0x040027C2 RID: 10178
	public TextMeshProUGUI countText;

	// Token: 0x040027C3 RID: 10179
	public int count;

	// Token: 0x040027C4 RID: 10180
	public bool invisibleIfZero = true;
}
