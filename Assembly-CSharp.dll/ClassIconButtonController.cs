using System;

// Token: 0x0200055D RID: 1373
public class ClassIconButtonController : ButtonController
{
	// Token: 0x06001DDB RID: 7643 RVA: 0x001A4478 File Offset: 0x001A2678
	public void Setup(Evidence newEvidenceEntry, InfoWindow newParentWindow, Fact newFact)
	{
		this.evidenceEntry = newEvidenceEntry;
		this.parentWindow = newParentWindow;
		this.SetupReferences();
		this.connectionFact = newFact;
		if (this.connectionFact != null)
		{
			this.connectionFact.OnConnectingEvidenceChangeDataKey += this.VisualUpdate;
			this.connectionFact.OnNewName += this.UpdateTooltipText;
		}
		this.evidenceEntry.OnDataKeyChange += this.VisualUpdate;
		this.VisualUpdate();
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x001A44F6 File Offset: 0x001A26F6
	public override void VisualUpdate()
	{
		this.icon.sprite = this.evidenceEntry.GetIcon();
		this.UpdateTooltipText();
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x001A4514 File Offset: 0x001A2714
	private void OnDestroy()
	{
		if (this.connectionFact != null)
		{
			this.connectionFact.OnConnectingEvidenceChangeDataKey -= this.VisualUpdate;
			this.connectionFact.OnNewName -= this.UpdateTooltipText;
		}
		this.evidenceEntry.OnDataKeyChange -= this.VisualUpdate;
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x00002265 File Offset: 0x00000465
	public override void OnLeftDoubleClick()
	{
	}

	// Token: 0x06001DDF RID: 7647 RVA: 0x001A4574 File Offset: 0x001A2774
	public override void UpdateTooltipText()
	{
		if (this.connectionFact != null)
		{
			this.tooltip.mainText = this.evidenceEntry.GetNameForDataKey(this.connectionFact.fromDataKeys);
			this.tooltip.detailText = this.connectionFact.GetName(null);
			return;
		}
		this.tooltip.mainText = string.Empty;
		this.tooltip.detailText = string.Empty;
	}

	// Token: 0x040027B8 RID: 10168
	[NonSerialized]
	public Fact connectionFact;

	// Token: 0x040027B9 RID: 10169
	[NonSerialized]
	public Evidence evidenceEntry;
}
