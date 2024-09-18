using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000564 RID: 1380
public class EvidenceButtonController : ButtonController, IPointerDownHandler, IEventSystemHandler
{
	// Token: 0x06001E01 RID: 7681 RVA: 0x001A531C File Offset: 0x001A351C
	public virtual void Setup(Evidence newEvidence, List<Evidence.DataKey> newKeys, PinnedItemController newController)
	{
		this.SetupReferences();
		this.pinnedController = newController;
		this.evidence = newEvidence;
		this.evidenceKeys = newKeys;
		this.UpdateTooltipText();
		this.VisualUpdate();
		CityConstructor.Instance.OnGameStarted += this.VisualUpdate;
		if (this.evidence != null)
		{
			this.evidence.OnDataKeyChange += this.VisualUpdate;
			this.evidence.OnNewName += this.UpdateTooltipText;
			if (this.evidence.preset.pinnedStyle == EvidencePreset.PinnedStyle.stickNote)
			{
				this.background.sprite = InterfaceControls.Instance.stickyNoteButtonSprite;
			}
		}
		this.ExtraSetup();
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x001A53D0 File Offset: 0x001A35D0
	private void OnDestroy()
	{
		CityConstructor.Instance.OnGameStarted -= this.VisualUpdate;
		if (this.evidence != null)
		{
			this.evidence.OnDataKeyChange -= this.VisualUpdate;
			this.evidence.OnNewName -= this.UpdateTooltipText;
		}
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void ExtraSetup()
	{
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x001A542C File Offset: 0x001A362C
	public override void OnLeftClick()
	{
		if (CasePanelController.Instance.pickModeActive && CasePanelController.Instance.pickForField != null)
		{
			EvidenceCitizen evidenceCitizen = this.pinnedController.evidence as EvidenceCitizen;
			if (evidenceCitizen != null && CasePanelController.Instance.pickForField.question.inputType == Case.InputType.citizen && evidenceCitizen.GetTiedKeys(this.pinnedController.caseElement.dk).Contains(Evidence.DataKey.name))
			{
				CasePanelController.Instance.pickForField.OnPick(this.pinnedController.evidence, this.pinnedController.caseElement.dk);
				this.pinnedController.ForcePointerUp();
				return;
			}
			if (this.pinnedController.evidence is EvidenceLocation && CasePanelController.Instance.pickForField.question.inputType == Case.InputType.location)
			{
				CasePanelController.Instance.pickForField.OnPick(this.pinnedController.evidence, this.pinnedController.caseElement.dk);
				this.pinnedController.ForcePointerUp();
				return;
			}
			if (CasePanelController.Instance.pickForField.question.inputType == Case.InputType.item)
			{
				CasePanelController.Instance.pickForField.OnPick(this.pinnedController.evidence, this.pinnedController.caseElement.dk);
				this.pinnedController.ForcePointerUp();
				return;
			}
		}
		else
		{
			Case.CaseElement caseElement = null;
			if (this.pinnedController != null)
			{
				caseElement = this.pinnedController.caseElement;
			}
			InterfaceController instance = InterfaceController.Instance;
			Evidence passedEvidence = this.evidence;
			Evidence.DataKey passedEvidenceKey = Evidence.DataKey.name;
			List<Evidence.DataKey> passedEvidenceKeys = this.evidenceKeys;
			string presetName = "";
			bool worldInteraction = false;
			bool autoPosition = true;
			Interactable interactable = this.evidence.interactable;
			Case.CaseElement forcedPinnedElement = caseElement;
			instance.SpawnWindow(passedEvidence, passedEvidenceKey, passedEvidenceKeys, presetName, worldInteraction, autoPosition, default(Vector2), interactable, null, forcedPinnedElement, true);
			this.pinnedController.ForcePointerUp();
		}
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x001A55EC File Offset: 0x001A37EC
	public override void OnHoverStart()
	{
		base.OnHoverStart();
		if (this.pinnedController != null && PinnedItemController.activeQuickMenu != null)
		{
			PinnedItemController.activeQuickMenu.Remove(true);
		}
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x001A561A File Offset: 0x001A381A
	public override void OnPointerDown(PointerEventData data)
	{
		this.pinnedController.dragController.OnPointerDown(data);
		this.pinnedController.OnPointerDown(data);
	}

	// Token: 0x06001E07 RID: 7687 RVA: 0x001A563C File Offset: 0x001A383C
	public override void VisualUpdate()
	{
		if (this.evidence != null)
		{
			this.evidenceKeys = this.evidence.GetTiedKeys(this.evidenceKeys);
			Texture photo = this.evidence.GetPhoto(this.evidenceKeys);
			if (this.icon != null)
			{
				this.icon.gameObject.SetActive(false);
			}
			if (this.evPhoto != null)
			{
				this.evPhoto.texture = photo;
				if (photo == null)
				{
					if (this.evPhoto != null)
					{
						this.evPhoto.color = Color.clear;
						this.evPhoto.gameObject.SetActive(false);
					}
				}
				else
				{
					this.evPhoto.color = Color.white;
					this.evPhoto.gameObject.SetActive(true);
				}
			}
		}
		else if (this.parentWindow != null)
		{
			if (this.icon != null)
			{
				this.icon.sprite = this.parentWindow.iconLarge;
			}
			if (this.icon != null)
			{
				this.icon.gameObject.SetActive(true);
			}
			if (this.evPhoto != null)
			{
				this.evPhoto.gameObject.SetActive(false);
			}
		}
		this.UpdateTooltipText();
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x001A5792 File Offset: 0x001A3992
	public override void UpdateTooltipText()
	{
		if (this.tooltip == null)
		{
			return;
		}
		if (this.evidence != null && this.evidenceKeys != null)
		{
			this.tooltip.mainText = this.evidence.GetNameForDataKey(this.evidenceKeys);
		}
	}

	// Token: 0x040027E0 RID: 10208
	public PinnedItemController pinnedController;

	// Token: 0x040027E1 RID: 10209
	public Evidence evidence;

	// Token: 0x040027E2 RID: 10210
	public List<Evidence.DataKey> evidenceKeys;

	// Token: 0x040027E3 RID: 10211
	public RawImage evPhoto;
}
