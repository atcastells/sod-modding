using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020004DD RID: 1245
public class PinnedPinButtonController : ButtonController, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06001AE6 RID: 6886 RVA: 0x001899B2 File Offset: 0x00187BB2
	public void Setup(PinnedItemController newItem)
	{
		this.pinnedController = newItem;
		this.SetupReferences();
		this.VisualUpdate();
		this.UpdatePinColour();
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x001899D0 File Offset: 0x00187BD0
	public void UpdatePinColour()
	{
		Color evidenceColour = InterfaceController.Instance.GetEvidenceColour(this.pinnedController.caseElement.color);
		this.background.color = evidenceColour;
		this.icon.color = evidenceColour;
		this.SetButtonBaseColour(evidenceColour);
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x00189A17 File Offset: 0x00187C17
	private void OnEnable()
	{
		this.SetupReferences();
		this.VisualUpdate();
	}

	// Token: 0x06001AE9 RID: 6889 RVA: 0x00189A25 File Offset: 0x00187C25
	public override void OnLeftClick()
	{
		if (CasePanelController.Instance.customStringLinkSelection != null)
		{
			CasePanelController.Instance.FinishCustomStringLinkSelection(this.pinnedController);
			return;
		}
	}

	// Token: 0x06001AEA RID: 6890 RVA: 0x00002265 File Offset: 0x00000465
	public override void OnLeftDoubleClick()
	{
	}

	// Token: 0x0400238C RID: 9100
	public Image mainColour;

	// Token: 0x0400238D RID: 9101
	public Image mainOverlay;

	// Token: 0x0400238E RID: 9102
	public Image pressedColour;

	// Token: 0x0400238F RID: 9103
	public Image pressedOverlay;

	// Token: 0x04002390 RID: 9104
	public RectTransform mainMOOverlay;

	// Token: 0x04002391 RID: 9105
	public Sprite pinnedOverlay;

	// Token: 0x04002392 RID: 9106
	public Sprite pinnedOverlayMO;

	// Token: 0x04002393 RID: 9107
	public PinnedItemController pinnedController;
}
