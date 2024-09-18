using System;
using UnityEngine.UI;

// Token: 0x0200029C RID: 668
public class CruncherTimelineEntry : ComputerOSUIComponent
{
	// Token: 0x06000EED RID: 3821 RVA: 0x000D5E92 File Offset: 0x000D4092
	public void Setup(SurveillanceApp newApp, SceneRecorder.SceneCapture newCap)
	{
		this.app = newApp;
		this.sceneReference = newCap;
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x000D5EA2 File Offset: 0x000D40A2
	public void SetMouseOver(bool val)
	{
		if (this.mousedOver != val)
		{
			this.mousedOver = val;
			this.VisualUpdate();
		}
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x000D5EBA File Offset: 0x000D40BA
	public void SetFlagged(bool val)
	{
		if (this.flagged != val)
		{
			this.flagged = val;
			this.VisualUpdate();
		}
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x000D5ED4 File Offset: 0x000D40D4
	public void VisualUpdate()
	{
		if (this.mousedOver)
		{
			if (this.flagged)
			{
				this.img.color = this.app.timelineFlagColor;
				this.juice.elements[0].originalColour = this.app.timelineFlagColor;
				this.juice.pulsateColour = this.app.timelineMOColor;
				this.juice.Pulsate(true, false);
				return;
			}
			this.img.color = this.app.timelineColor;
			this.juice.elements[0].originalColour = this.app.timelineColor;
			this.juice.pulsateColour = this.app.timelineMOColor;
			this.juice.Pulsate(true, false);
			return;
		}
		else
		{
			if (this.flagged)
			{
				this.juice.Pulsate(false, false);
				this.img.color = this.app.timelineFlagColor;
				return;
			}
			this.juice.Pulsate(false, false);
			this.img.color = this.app.timelineColor;
			return;
		}
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x000D5FF9 File Offset: 0x000D41F9
	public override void OnLeftClick()
	{
		this.app.SetScene(this.sceneReference);
	}

	// Token: 0x0400121A RID: 4634
	public SurveillanceApp app;

	// Token: 0x0400121B RID: 4635
	public Image img;

	// Token: 0x0400121C RID: 4636
	public JuiceController juice;

	// Token: 0x0400121D RID: 4637
	[NonSerialized]
	public SceneRecorder.SceneCapture sceneReference;

	// Token: 0x0400121E RID: 4638
	public bool mousedOver;

	// Token: 0x0400121F RID: 4639
	public bool flagged;
}
