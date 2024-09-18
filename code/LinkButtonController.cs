using System;

// Token: 0x02000580 RID: 1408
public class LinkButtonController : ButtonController
{
	// Token: 0x06001ED2 RID: 7890 RVA: 0x001AC660 File Offset: 0x001AA860
	public void Setup(string newLinkID, TMP_SelectionController newSelectionController)
	{
		this.linkID = newLinkID;
		this.selectionController = newSelectionController;
		this.SetupReferences();
		if (this.selectionController != null && this.selectionController.m_TextMeshPro != null)
		{
			if (this.selectionController.m_TextMeshPro.alignment == 513 || this.selectionController.m_TextMeshPro.alignment == 257 || this.selectionController.m_TextMeshPro.alignment == 2049 || this.selectionController.m_TextMeshPro.alignment == 8193 || this.selectionController.m_TextMeshPro.alignment == 4097 || this.selectionController.m_TextMeshPro.alignment == 1025)
			{
				this.thisNavRectPoint = ButtonController.NavRectPoint.min;
				this.otherNavRectPoint = ButtonController.NavRectPoint.min;
				return;
			}
			if (this.selectionController.m_TextMeshPro.alignment == 516 || this.selectionController.m_TextMeshPro.alignment == 260 || this.selectionController.m_TextMeshPro.alignment == 2052 || this.selectionController.m_TextMeshPro.alignment == 8196 || this.selectionController.m_TextMeshPro.alignment == 4100 || this.selectionController.m_TextMeshPro.alignment == 1028)
			{
				this.thisNavRectPoint = ButtonController.NavRectPoint.max;
				this.otherNavRectPoint = ButtonController.NavRectPoint.max;
			}
		}
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x001AC7DC File Offset: 0x001AA9DC
	public override void OnLeftClick()
	{
		int num = 0;
		int.TryParse(this.linkID, ref num);
		Strings.LinkData linkData = null;
		if (Strings.Instance.linkIDReference.TryGetValue(num, ref linkData))
		{
			linkData.OnLink();
		}
		base.OnLeftClick();
	}

	// Token: 0x040028A7 RID: 10407
	private TMP_SelectionController selectionController;

	// Token: 0x040028A8 RID: 10408
	private string linkID;
}
