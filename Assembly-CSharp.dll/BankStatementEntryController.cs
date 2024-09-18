using System;
using TMPro;
using UnityEngine;

// Token: 0x0200054B RID: 1355
public class BankStatementEntryController : MonoBehaviour
{
	// Token: 0x06001D7F RID: 7551 RVA: 0x001A0DE0 File Offset: 0x0019EFE0
	public void Setup(string text, int amount, int balance)
	{
		this.descriptionText.text = text;
		this.amountText.text = amount.ToString();
		this.balanceText.text = balance.ToString();
		base.name = text;
		if (balance < 0)
		{
			this.balanceText.color = Color.red;
			return;
		}
		this.balanceText.color = Color.black;
	}

	// Token: 0x0400273B RID: 10043
	public RectTransform rect;

	// Token: 0x0400273C RID: 10044
	public TextMeshProUGUI descriptionText;

	// Token: 0x0400273D RID: 10045
	public TextMeshProUGUI amountText;

	// Token: 0x0400273E RID: 10046
	public TextMeshProUGUI balanceText;
}
