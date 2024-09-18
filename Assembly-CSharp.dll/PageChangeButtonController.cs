using System;

// Token: 0x02000585 RID: 1413
public class PageChangeButtonController : ButtonController
{
	// Token: 0x06001EE2 RID: 7906 RVA: 0x001AD1F4 File Offset: 0x001AB3F4
	public override void OnLeftClick()
	{
		base.OnLeftClick();
		WindowContentController componentInParent = base.gameObject.GetComponentInParent<WindowContentController>();
		if (componentInParent != null)
		{
			if (this.nextPage)
			{
				componentInParent.NextPage();
				return;
			}
			componentInParent.PrevPage();
		}
	}

	// Token: 0x040028BA RID: 10426
	public bool nextPage;
}
