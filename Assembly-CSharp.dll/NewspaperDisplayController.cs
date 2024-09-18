using System;
using TMPro;
using UnityEngine;

// Token: 0x02000412 RID: 1042
public class NewspaperDisplayController : MonoBehaviour
{
	// Token: 0x060017C4 RID: 6084 RVA: 0x00164694 File Offset: 0x00162894
	private void Start()
	{
		NewspaperController.Instance.UpdateNewspaperReferences(this);
	}

	// Token: 0x04001D2E RID: 7470
	[Header("Components")]
	public TextMeshProUGUI newspaperTitleText;

	// Token: 0x04001D2F RID: 7471
	public TextMeshProUGUI newspaperDateText;

	// Token: 0x04001D30 RID: 7472
	[Space(7f)]
	public TextMeshProUGUI mainArticleHeadline;

	// Token: 0x04001D31 RID: 7473
	public TextMeshProUGUI mainArticleColumn1;

	// Token: 0x04001D32 RID: 7474
	public TextMeshProUGUI mainArticleColumn2;

	// Token: 0x04001D33 RID: 7475
	public TextMeshProUGUI mainArticleColumn3;

	// Token: 0x04001D34 RID: 7476
	[Space(7f)]
	public TextMeshProUGUI article2Headline;

	// Token: 0x04001D35 RID: 7477
	public TextMeshProUGUI article2Column1;

	// Token: 0x04001D36 RID: 7478
	public TextMeshProUGUI article2Column2;

	// Token: 0x04001D37 RID: 7479
	public TextMeshProUGUI article2Column3;

	// Token: 0x04001D38 RID: 7480
	[Space(7f)]
	public TextMeshProUGUI article3Headline;

	// Token: 0x04001D39 RID: 7481
	public TextMeshProUGUI article3Column1;

	// Token: 0x04001D3A RID: 7482
	public TextMeshProUGUI article3Column2;

	// Token: 0x04001D3B RID: 7483
	public TextMeshProUGUI article3Column3;

	// Token: 0x04001D3C RID: 7484
	[Space(7f)]
	public TextMeshProUGUI ad1Text;

	// Token: 0x04001D3D RID: 7485
	public TextMeshProUGUI ad2Text;

	// Token: 0x04001D3E RID: 7486
	public TextMeshProUGUI ad3Text;

	// Token: 0x04001D3F RID: 7487
	public TextMeshProUGUI ad4Text;
}
