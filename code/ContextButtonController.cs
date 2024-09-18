using System;
using UnityEngine;

// Token: 0x020004E6 RID: 1254
public class ContextButtonController : ButtonController
{
	// Token: 0x06001B29 RID: 6953 RVA: 0x0018B4B0 File Offset: 0x001896B0
	public void Setup(ContextMenuController newCmc, ContextMenuPanelController newPanel, ContextMenuController.ContextMenuButtonSetup newSetup)
	{
		this.cmc = newCmc;
		this.panelController = newPanel;
		this.setup = newSetup;
		this.SetupReferences();
		this.UpdateButtonText();
		if (this.setup.useColour)
		{
			this.icon.gameObject.SetActive(true);
			this.icon.color = this.setup.colour;
		}
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x0018B514 File Offset: 0x00189714
	public override void UpdateButtonText()
	{
		if (!this.setup.useText || !(this.text != null))
		{
			if (this.text != null)
			{
				Object.Destroy(this.text);
			}
			return;
		}
		if (this.setup.overrideText != null && this.setup.overrideText.Length > 0)
		{
			this.text.text = this.setup.overrideText;
			return;
		}
		this.text.text = Strings.Get("ui.context", this.setup.commandString, Strings.Casing.asIs, false, false, false, null);
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x0018B5B2 File Offset: 0x001897B2
	public override void OnLeftClick()
	{
		this.cmc.OnCommand(this);
	}

	// Token: 0x040023C9 RID: 9161
	public ContextMenuController cmc;

	// Token: 0x040023CA RID: 9162
	public ContextMenuPanelController panelController;

	// Token: 0x040023CB RID: 9163
	public ContextMenuController.ContextMenuButtonSetup setup;
}
