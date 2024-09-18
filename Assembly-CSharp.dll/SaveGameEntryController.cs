using System;
using System.IO;

// Token: 0x020005C8 RID: 1480
public class SaveGameEntryController : ButtonController
{
	// Token: 0x0600206E RID: 8302 RVA: 0x001BC855 File Offset: 0x001BAA55
	public void Setup(FileInfo newInfo)
	{
		this.SetupReferences();
		this.info = newInfo;
		this.UpdateButtonText();
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x001BC86C File Offset: 0x001BAA6C
	public override void UpdateButtonText()
	{
		if (this.info != null)
		{
			this.text.text = this.info.Name.Substring(0, this.info.Name.Length - this.info.Extension.Length);
			return;
		}
		this.text.text = Strings.Get("ui.interface", "New Save", Strings.Casing.asIs, false, false, false, null);
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x001BC8DE File Offset: 0x001BAADE
	public override void OnLeftClick()
	{
		MainMenuController.Instance.SelectNewSave(this);
		base.OnLeftClick();
	}

	// Token: 0x04002A8D RID: 10893
	public FileInfo info;

	// Token: 0x04002A8E RID: 10894
	public bool selected;

	// Token: 0x04002A8F RID: 10895
	public bool isInternal;
}
