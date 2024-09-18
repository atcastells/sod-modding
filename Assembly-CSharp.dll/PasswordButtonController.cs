using System;

// Token: 0x02000586 RID: 1414
public class PasswordButtonController : ButtonController
{
	// Token: 0x06001EE4 RID: 7908 RVA: 0x001AD231 File Offset: 0x001AB431
	public void Setup(InfoWindow newParentWindow)
	{
		this.parentWindow = newParentWindow;
		this.SetupReferences();
		this.UpdateTooltipText();
		this.VisualUpdate();
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x00002265 File Offset: 0x00000465
	private void OnEnable()
	{
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x00002265 File Offset: 0x00000465
	private void OnDisable()
	{
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x00002265 File Offset: 0x00000465
	private void OnDestroy()
	{
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x001AD24C File Offset: 0x001AB44C
	public override void VisualUpdate()
	{
		this.UpdateTooltipText();
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x00002265 File Offset: 0x00000465
	public override void OnLeftClick()
	{
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x00002265 File Offset: 0x00000465
	public override void OnRightClick()
	{
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x001AD254 File Offset: 0x001AB454
	public override void UpdateTooltipText()
	{
		if (this.tooltip == null)
		{
			return;
		}
		string empty = string.Empty;
		for (int i = 0; i < 4; i++)
		{
		}
		this.text.text = this.tooltip.mainText;
		this.tooltip.detailText = Strings.Get("evidence.generic", "passwordguess", Strings.Casing.asIs, false, false, false, null);
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x001AD2B7 File Offset: 0x001AB4B7
	public override void OnHoverStart()
	{
		this.text.fontStyle = 4;
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x001AD2C5 File Offset: 0x001AB4C5
	public override void OnHoverEnd()
	{
		this.text.fontStyle = 0;
	}
}
