using System;

// Token: 0x020004CF RID: 1231
public class CaseButtonController : ButtonController
{
	// Token: 0x06001A87 RID: 6791 RVA: 0x00185DC8 File Offset: 0x00183FC8
	public void Setup(Case newCase)
	{
		this.SetupReferences();
		this.thisCase = newCase;
		this.UpdateVisuals();
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x00185DDD File Offset: 0x00183FDD
	public void UpdateVisuals()
	{
		this.text.text = this.thisCase.name;
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x00185DF5 File Offset: 0x00183FF5
	public override void OnLeftClick()
	{
		base.OnLeftClick();
		InterfaceController.Instance.ActivateObjectivesDisplay();
		CasePanelController.Instance.SetActiveCase(this.thisCase);
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x00185E18 File Offset: 0x00184018
	public override void OnLeftDoubleClick()
	{
		base.OnLeftDoubleClick();
		PopupMessageController.Instance.PopupMessage("RenameCase", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, this.thisCase.name, false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnLeftButton += this.OnRenameCancel;
		PopupMessageController.Instance.OnRightButton += this.OnRenameConfirm;
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x00185EA0 File Offset: 0x001840A0
	public void OnRenameConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnRenameCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnRenameConfirm;
		this.thisCase.name = PopupMessageController.Instance.inputField.text;
		this.UpdateVisuals();
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x00185EF9 File Offset: 0x001840F9
	public void OnRenameCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnRenameCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnRenameConfirm;
	}

	// Token: 0x04002331 RID: 9009
	public Case thisCase;
}
