using System;
using UnityEngine.UI;

// Token: 0x0200057B RID: 1403
public class ItemSelectButtonController : ButtonController
{
	// Token: 0x06001EB5 RID: 7861 RVA: 0x001AB5D1 File Offset: 0x001A97D1
	public void Setup(Interactable newInteractable, InfoWindow newThisWindow)
	{
		base.SetupReferences();
		this.obj = newInteractable;
		this.thisWindow = newThisWindow;
		this.UpdateButtonText();
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x001AB5ED File Offset: 0x001A97ED
	public override void UpdateButtonText()
	{
		if (this.obj != null)
		{
			this.photo.sprite = this.obj.preset.staticImage;
			this.text.text = this.obj.GetName();
		}
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x001AB628 File Offset: 0x001A9828
	public override void OnLeftClick()
	{
		if (InteractionController.Instance.talkingTo != null && InteractionController.Instance.talkingTo.isActor != null)
		{
			Human human = InteractionController.Instance.talkingTo.isActor as Human;
			if (human != null)
			{
				human.TryGiveItem(this.obj, Player.Instance, this.thisWindow.dialogSuccess, true);
			}
		}
		this.End();
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x001AB69A File Offset: 0x001A989A
	private void End()
	{
		this.thisWindow.CloseWindow(true);
		base.OnLeftClick();
	}

	// Token: 0x0400288B RID: 10379
	[NonSerialized]
	public Interactable obj;

	// Token: 0x0400288C RID: 10380
	public Image photo;

	// Token: 0x0400288D RID: 10381
	public InfoWindow thisWindow;
}
