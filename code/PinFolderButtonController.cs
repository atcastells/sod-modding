using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200058B RID: 1419
public class PinFolderButtonController : ButtonController, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	// Token: 0x06001EFB RID: 7931 RVA: 0x00189A17 File Offset: 0x00187C17
	private void Start()
	{
		this.SetupReferences();
		this.VisualUpdate();
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x001ADDDC File Offset: 0x001ABFDC
	public override void OnPointerDown(PointerEventData eventData)
	{
		if (this.parentWindow != null && this.parentWindow.forceDisablePin)
		{
			return;
		}
		if (eventData.button != null)
		{
			Game.Log("Pointer button: " + eventData.button.ToString(), 2);
			return;
		}
		if (CasePanelController.Instance.activeCase == null)
		{
			PopupMessageController.Instance.PopupMessage("NoActiveCase", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, Strings.Get("ui.interface", "New Case", Strings.Casing.asIs, false, false, false, null), false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.onCreateNewCasePopupCancel;
			PopupMessageController.Instance.OnRightButton += this.OnCreateNewCasePopup;
			Game.Log("No active case...", 2);
			return;
		}
		if (CasePanelController.Instance.customLinkSelectionMode)
		{
			Game.Log("Custom string active...", 2);
			return;
		}
		if (this.parentWindow != null && !this.parentWindow.pinned && this.parentWindow.currentPinnedCaseElement == null)
		{
			Game.Log("Toggling pinned...", 2);
			this.pointerDown = true;
			this.parentWindow.pinColour.enabled = false;
			this.parentWindow.pinOverlay.enabled = false;
			this.parentWindow.TogglePinned();
			try
			{
				this.parentWindow.currentPinnedCaseElement.pinnedController.dragController.OnPointerDown(eventData);
				this.parentWindow.currentPinnedCaseElement.pinnedController.OnPointerDown(eventData);
				this.parentWindow.currentPinnedCaseElement.pinnedController.juice.Nudge(new Vector2(2.5f, 2.5f), Vector2.zero, true, true, false);
				if (!this.placementActive)
				{
					base.StartCoroutine(this.PlacementFade());
				}
				return;
			}
			catch
			{
				return;
			}
		}
		this.icon.gameObject.SetActive(true);
		if (this.parentWindow != null)
		{
			this.parentWindow.pinColour.enabled = false;
			this.parentWindow.pinOverlay.enabled = false;
		}
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x001AE020 File Offset: 0x001AC220
	public void OnCreateNewCasePopup()
	{
		PopupMessageController.Instance.OnLeftButton -= this.onCreateNewCasePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnCreateNewCasePopup;
		CasePanelController.Instance.OnCreateNewCustomCase();
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x001AE058 File Offset: 0x001AC258
	public void onCreateNewCasePopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.onCreateNewCasePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnCreateNewCasePopup;
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x001AE086 File Offset: 0x001AC286
	public override void OnPointerUp(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.desktopMode)
		{
			return;
		}
		if (eventData.button != null)
		{
			return;
		}
		this.ForcePointerUp();
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x001AE0A4 File Offset: 0x001AC2A4
	public void ForcePointerUp()
	{
		Game.Log("Force pointer up", 2);
		this.pointerDown = false;
		if (CasePanelController.Instance.customLinkSelectionMode)
		{
			return;
		}
		if (this.parentWindow.currentPinnedCaseElement != null && this.parentWindow.currentPinnedCaseElement.pinnedController != null)
		{
			this.parentWindow.currentPinnedCaseElement.pinnedController.ForcePointerUp();
		}
		if (this.placementActive)
		{
			this.placementActive = false;
			if (InterfaceControls.Instance.minimizeEvidenceOnPinned)
			{
				this.parentWindow.CloseWindow(true);
			}
		}
		else if (this.parentWindow.pinned && this.parentWindow.currentPinnedCaseElement != null && this.parentWindow.currentPinnedCaseElement.pinnedController != null)
		{
			this.parentWindow.TogglePinned();
		}
		this.parentWindow.pinColour.enabled = true;
		this.parentWindow.pinOverlay.enabled = true;
		this.icon.gameObject.SetActive(false);
	}

	// Token: 0x06001F01 RID: 7937 RVA: 0x001AE1A3 File Offset: 0x001AC3A3
	private IEnumerator PlacementFade()
	{
		this.placementActive = true;
		if (!InputController.Instance.mouseInputMode)
		{
			this.parentWindow.currentPinnedCaseElement.pinnedController.rect.transform.position = this.rect.transform.position;
		}
		while (this.placementActive)
		{
			if (this.parentWindow.currentPinnedCaseElement != null && this.parentWindow.currentPinnedCaseElement.pinnedController != null)
			{
				this.parentWindow.currentPinnedCaseElement.pinnedController.pinPlaceActive = true;
				if (InputController.Instance.mouseInputMode)
				{
					this.parentWindow.currentPinnedCaseElement.pinnedController.ForceDrag();
					this.parentWindow.currentPinnedCaseElement.pinnedController.dragController.ForceDrag(Input.mousePosition);
				}
				else
				{
					Vector2 vector;
					vector..ctor(InputController.Instance.GetAxisRelative("MoveEvidenceAxisX"), InputController.Instance.GetAxisRelative("MoveEvidenceAxisY"));
					if (vector.magnitude > 0.15f)
					{
						vector *= 12f;
						this.parentWindow.currentPinnedCaseElement.pinnedController.ForceDrag();
						this.parentWindow.currentPinnedCaseElement.pinnedController.dragController.ForceDragController(new Vector2(this.parentWindow.currentPinnedCaseElement.pinnedController.rect.localPosition.x, this.parentWindow.currentPinnedCaseElement.pinnedController.rect.localPosition.y) + vector);
					}
				}
			}
			yield return null;
		}
		this.parentWindow.currentPinnedCaseElement.pinnedController.pinPlaceActive = false;
		yield break;
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x001AE1B4 File Offset: 0x001AC3B4
	public override void OnLeftClick()
	{
		if (CasePanelController.Instance.customLinkSelectionMode && this.parentWindow.currentPinnedCaseElement != null && this.parentWindow.currentPinnedCaseElement.pinnedController != null)
		{
			CasePanelController.Instance.FinishCustomStringLinkSelection(this.parentWindow.currentPinnedCaseElement.pinnedController);
			return;
		}
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x001AE20D File Offset: 0x001AC40D
	public override void OnHoverStart()
	{
		this.VisualUpdate();
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x001AE20D File Offset: 0x001AC40D
	public override void OnHoverEnd()
	{
		this.VisualUpdate();
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x001AE218 File Offset: 0x001AC418
	public override void VisualUpdate()
	{
		if (this.parentWindow != null && this.parentWindow.pinnable)
		{
			if (this.parentWindow.pinned)
			{
				this.parentWindow.pinColour.sprite = this.pinnedColour;
				if (this.isOver)
				{
					this.parentWindow.pinOverlay.sprite = this.pinnedMO;
				}
				else
				{
					this.parentWindow.pinOverlay.sprite = this.pinnedImage;
				}
				this.rect.sizeDelta = new Vector2(64f, 64f);
				RectTransform component = this.parentWindow.pinColour.GetComponent<RectTransform>();
				if (component != null)
				{
					component.sizeDelta = this.rect.sizeDelta;
					return;
				}
			}
			else
			{
				this.parentWindow.pinColour.sprite = this.unpinnedColour;
				if (this.isOver)
				{
					this.parentWindow.pinOverlay.sprite = this.unpinnedMO;
				}
				else
				{
					this.parentWindow.pinOverlay.sprite = this.unpinnedImage;
				}
				if (this.rect != null)
				{
					this.rect.sizeDelta = new Vector2(128f, 64f);
				}
				RectTransform component2 = this.parentWindow.pinColour.GetComponent<RectTransform>();
				if (component2 != null)
				{
					component2.sizeDelta = this.rect.sizeDelta;
				}
			}
		}
	}

	// Token: 0x040028C8 RID: 10440
	public Sprite pinnedImage;

	// Token: 0x040028C9 RID: 10441
	public Sprite pinnedMO;

	// Token: 0x040028CA RID: 10442
	public Sprite unpinnedImage;

	// Token: 0x040028CB RID: 10443
	public Sprite unpinnedMO;

	// Token: 0x040028CC RID: 10444
	public Sprite pinnedColour;

	// Token: 0x040028CD RID: 10445
	public Sprite unpinnedColour;

	// Token: 0x040028CE RID: 10446
	public ContextMenuController contextMenu;

	// Token: 0x040028CF RID: 10447
	public bool placementActive;

	// Token: 0x040028D0 RID: 10448
	public bool pointerDown;
}
