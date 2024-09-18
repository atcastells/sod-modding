using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004D0 RID: 1232
public class CasePanelController : PanelController
{
	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06001A8E RID: 6798 RVA: 0x00185F30 File Offset: 0x00184130
	// (remove) Token: 0x06001A8F RID: 6799 RVA: 0x00185F68 File Offset: 0x00184168
	public event CasePanelController.PinnedChange OnPinnedChange;

	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06001A90 RID: 6800 RVA: 0x00185FA0 File Offset: 0x001841A0
	// (remove) Token: 0x06001A91 RID: 6801 RVA: 0x00185FD8 File Offset: 0x001841D8
	public event CasePanelController.PinEvidence OnPinEvidence;

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x06001A92 RID: 6802 RVA: 0x00186010 File Offset: 0x00184210
	// (remove) Token: 0x06001A93 RID: 6803 RVA: 0x00186048 File Offset: 0x00184248
	public event CasePanelController.UnpinEvidence OnUnpinEvidence;

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06001A94 RID: 6804 RVA: 0x0018607D File Offset: 0x0018427D
	public static CasePanelController Instance
	{
		get
		{
			return CasePanelController._instance;
		}
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x00186084 File Offset: 0x00184284
	private void Awake()
	{
		if (CasePanelController._instance != null && CasePanelController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CasePanelController._instance = this;
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x001860B2 File Offset: 0x001842B2
	private void Start()
	{
		this.UpdateCloseCaseButton();
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x001860BC File Offset: 0x001842BC
	public void UpdateCaseControls()
	{
		List<Case> list = new List<Case>(this.activeCases);
		for (int i = 0; i < this.spawnedCaseButtons.Count; i++)
		{
			CaseButtonController caseButtonController = this.spawnedCaseButtons[i];
			if (list.Contains(caseButtonController.thisCase))
			{
				list.Remove(caseButtonController.thisCase);
				caseButtonController.UpdateVisuals();
			}
			else
			{
				Object.Destroy(caseButtonController.gameObject);
				this.spawnedCaseButtons.RemoveAt(i);
				i--;
			}
		}
		foreach (Case newCase in list)
		{
			CaseButtonController component = Object.Instantiate<GameObject>(PrefabControls.Instance.caseButton, this.caseButtonParent).GetComponent<CaseButtonController>();
			component.Setup(newCase);
			this.spawnedCaseButtons.Add(component);
		}
		this.newCaseButton.transform.SetAsLastSibling();
		if (this.spawnedCaseButtons.Count <= 0)
		{
			this.stickNoteButton.RefreshAutomaticNavigation();
			this.newCaseButton.RefreshAutomaticNavigation();
		}
		else
		{
			this.stickNoteButton.RefreshAutomaticNavigation();
			this.newCaseButton.RefreshAutomaticNavigation();
		}
		for (int j = 0; j < this.spawnedCaseButtons.Count; j++)
		{
			this.spawnedCaseButtons[j].RefreshAutomaticNavigation();
		}
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x0018621C File Offset: 0x0018441C
	public void SelectNoCaseButton()
	{
		int num = 0;
		Chapter chapter;
		if (Toolbox.Instance.IsStoryMissionActive(out chapter, out num) && num < 31)
		{
			return;
		}
		this.SetActiveCase(null);
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x00186248 File Offset: 0x00184448
	public void NewCustomCaseButton()
	{
		int num = 0;
		Chapter chapter;
		if (Toolbox.Instance.IsStoryMissionActive(out chapter, out num) && num < 31)
		{
			return;
		}
		this.CreateNewCase(Case.CaseType.custom, Case.CaseStatus.closable, false, Strings.Get("ui.interface", "New Case", Strings.Casing.asIs, false, false, false, null));
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x0018628C File Offset: 0x0018448C
	public Case CreateNewCase(Case.CaseType newType, Case.CaseStatus newStatus, bool isSilent = false, string caseName = "New Case")
	{
		if (this.activeCases.Count >= GameplayControls.Instance.maxCases && newType != Case.CaseType.mainStory && newType != Case.CaseType.murder)
		{
			Game.Log("Player: Case limit reached!", 2);
			return null;
		}
		if (newType == Case.CaseType.murder)
		{
			InterfaceController.Instance.NewMurderCaseDisplay();
		}
		Case @case = null;
		if (!isSilent)
		{
			PopupMessageController.Instance.PopupMessage("CreateCase", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, caseName, false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.OnCancelNewCustomCase;
			PopupMessageController.Instance.OnRightButton += this.OnCreateNewCustomCase;
		}
		else
		{
			@case = new Case(caseName, newType, newStatus);
		}
		if (@case != null)
		{
			this.activeCases.Add(@case);
			this.UpdateCaseControls();
			this.SetActiveCase(@case);
			if (@case.caseType == Case.CaseType.mainStory || @case.caseType == Case.CaseType.murder || @case.caseType == Case.CaseType.retirement)
			{
				@case.handIn.Clear();
				foreach (Interactable interactable in CityData.Instance.caseTrays)
				{
					@case.handIn.Add(interactable.id);
				}
			}
		}
		return @case;
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x001863E8 File Offset: 0x001845E8
	public void OnCreateNewCustomCase()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnCancelNewCustomCase;
		PopupMessageController.Instance.OnRightButton -= this.OnCreateNewCustomCase;
		Case @case = new Case(PopupMessageController.Instance.inputField.text, Case.CaseType.custom, Case.CaseStatus.closable);
		if (@case != null)
		{
			this.activeCases.Add(@case);
			this.UpdateCaseControls();
			this.SetActiveCase(@case);
		}
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x00186454 File Offset: 0x00184654
	public void OnCancelNewCustomCase()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnCancelNewCustomCase;
		PopupMessageController.Instance.OnRightButton -= this.OnCreateNewCustomCase;
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x00186484 File Offset: 0x00184684
	public void UpdateCloseCaseButton()
	{
		try
		{
			if (this.pickModeActive)
			{
				this.closeCaseButton.icon.sprite = this.resolveSprite;
				this.closeCaseButton.textReference = "Cancel Pick";
				this.closeCaseButton.UpdateButtonText();
				this.closeCaseButton.tooltip.mainDictionaryKey = "cancelpick";
				base.enabled = true;
			}
			else
			{
				if (this.activeCase != null && (this.activeCase.caseStatus == Case.CaseStatus.handInCollected || this.activeCase.caseStatus == Case.CaseStatus.handInNotCollected || this.activeCase.caseStatus == Case.CaseStatus.closable))
				{
					if (this.activeCase.caseStatus == Case.CaseStatus.handInCollected)
					{
						this.closeCaseButton.icon.sprite = this.resolveSprite;
						if (this.activeCase.caseType == Case.CaseType.retirement)
						{
							this.closeCaseButton.textReference = "Retire";
						}
						else
						{
							this.closeCaseButton.textReference = "Resolve";
						}
						this.closeCaseButton.UpdateButtonText();
						this.closeCaseButton.tooltip.mainDictionaryKey = "resolvecase";
						this.closeCaseButton.tooltip.detailDictionaryKey = "resolvecase_detail";
					}
					else if (this.activeCase.caseStatus == Case.CaseStatus.closable)
					{
						this.closeCaseButton.icon.sprite = this.archiveSprite;
						this.closeCaseButton.textReference = "Archive/Close";
						this.closeCaseButton.UpdateButtonText();
						this.closeCaseButton.tooltip.mainDictionaryKey = "closecase";
						this.closeCaseButton.tooltip.detailDictionaryKey = "closecase_detail";
					}
					else if (this.activeCase.caseStatus == Case.CaseStatus.handInNotCollected)
					{
						this.closeCaseButton.icon.sprite = this.collectHandInSprite;
						if (this.activeCase.caseType == Case.CaseType.sideJob)
						{
							this.closeCaseButton.textReference = "Get Info";
							this.closeCaseButton.UpdateButtonText();
							this.closeCaseButton.tooltip.mainDictionaryKey = "getinfo";
							this.closeCaseButton.tooltip.detailDictionaryKey = "getinfo_detail";
						}
						else
						{
							this.closeCaseButton.textReference = "Get Form";
							this.closeCaseButton.UpdateButtonText();
							this.closeCaseButton.tooltip.mainDictionaryKey = "getform";
							this.closeCaseButton.tooltip.detailDictionaryKey = "getform_detail";
						}
						this.closeCaseButton.UpdateButtonText();
						this.closeCaseButton.tooltip.mainDictionaryKey = "collectHandIn";
						this.closeCaseButton.tooltip.detailDictionaryKey = "collectHandIn_detail";
					}
					if (this.closeCaseDisplayArea != null)
					{
						this.caseCloseTransition = this.closeCaseDisplayArea.sizeDelta.x / 354f;
					}
					this.closeCaseButton.SetInteractable(true);
					try
					{
						base.enabled = true;
						this.newCaseButton.RefreshAutomaticNavigation();
						goto IL_397;
					}
					catch
					{
						goto IL_397;
					}
				}
				if (this.closeCaseButton != null)
				{
					this.closeCaseButton.SetInteractable(false);
					this.closeCaseButton.text.gameObject.SetActive(false);
					this.closeCaseButton.icon.gameObject.SetActive(false);
				}
				if (this.closeCaseDisplayArea != null)
				{
					this.caseCloseTransition = this.closeCaseDisplayArea.sizeDelta.x / 354f;
				}
				base.enabled = true;
				if (InterfaceController.Instance.selectedElement == this.closeCaseButton && this.controllerMode && this.currentSelectMode == CasePanelController.ControllerSelectMode.topBar)
				{
					this.closeCaseButton.OnDeselect();
					this.notebookButton.OnSelect();
				}
				this.newCaseButton.RefreshAutomaticNavigation();
				IL_397:
				this.UpdateResolveNotifications();
			}
		}
		catch
		{
		}
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x00186868 File Offset: 0x00184A68
	private void Update()
	{
		if (this.closeCaseButton.interactable)
		{
			if (this.caseCloseTransition < 1f)
			{
				this.caseCloseTransition += Time.deltaTime / 0.1f;
				this.caseCloseTransition = Mathf.Clamp01(this.caseCloseTransition);
				this.closeCaseDisplayArea.sizeDelta = new Vector2(Mathf.Lerp(0f, 354f, this.caseCloseTransition), this.closeCaseDisplayArea.sizeDelta.y);
				if (this.caseCloseTransition >= 1f)
				{
					this.closeCaseButton.text.gameObject.SetActive(true);
					this.closeCaseButton.icon.gameObject.SetActive(true);
					this.closeCaseButton.SetInteractable(true);
					base.enabled = false;
					return;
				}
			}
		}
		else if (this.caseCloseTransition > 0f)
		{
			this.caseCloseTransition -= Time.deltaTime / 0.1f;
			this.caseCloseTransition = Mathf.Clamp01(this.caseCloseTransition);
			this.closeCaseDisplayArea.sizeDelta = new Vector2(Mathf.Lerp(0f, 354f, this.caseCloseTransition), this.closeCaseDisplayArea.sizeDelta.y);
			if (this.caseCloseTransition <= 0f)
			{
				base.enabled = false;
			}
		}
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x001869C4 File Offset: 0x00184BC4
	public void CloseCaseButton()
	{
		if (this.pickModeActive)
		{
			this.SetPickModeActive(false, null);
			return;
		}
		if (this.activeCase != null && ResolveController.Instance == null)
		{
			InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "CaseResolve", false, false, InterfaceControls.Instance.handbookWindowPosition, null, null, null, true);
		}
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x00186A1A File Offset: 0x00184C1A
	public void CloseCase(Case closeThisCase)
	{
		if (closeThisCase.caseElements.Count > 0)
		{
			closeThisCase.isActive = false;
		}
		this.activeCases.Remove(closeThisCase);
		if (closeThisCase == this.activeCase)
		{
			this.SetActiveCase(null);
		}
		this.UpdateCaseControls();
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x00186A54 File Offset: 0x00184C54
	public void SetActiveCase(Case newCase)
	{
		if (this.activeCase != newCase)
		{
			Case @case = this.activeCase;
			this.activeCase = newCase;
			if (this.activeCase != null)
			{
				Game.Log("Player: New active case: " + this.activeCase.name, 2);
			}
			foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
			{
				infoWindow.PinnedUpdateCheck();
			}
			this.UpdatePinned();
			foreach (StringController stringController in this.spawnedStrings)
			{
				stringController.UpdateStringColour();
				stringController.UpdateHidden();
			}
			if (ResolveController.Instance != null)
			{
				ResolveController.Instance.UpdateResolveFields();
			}
		}
		this.UpdateCaseButtonsActive();
		if (this.activeCase != null)
		{
			this.closeCaseButton.SetInteractable(true);
		}
		else
		{
			this.closeCaseButton.SetInteractable(false);
		}
		this.UpdateCloseCaseButton();
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x00186B78 File Offset: 0x00184D78
	public void UpdateCaseButtonsActive()
	{
		foreach (CaseButtonController caseButtonController in this.spawnedCaseButtons)
		{
			if (caseButtonController.thisCase == this.activeCase)
			{
				caseButtonController.thisCase.isActive = true;
				caseButtonController.icon.enabled = true;
			}
			else
			{
				caseButtonController.thisCase.isActive = false;
				caseButtonController.icon.enabled = false;
			}
		}
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x00186C04 File Offset: 0x00184E04
	public void NewStickyNoteButton()
	{
		this.NewStickyNote();
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x00186C10 File Offset: 0x00184E10
	public InfoWindow NewStickyNote()
	{
		if (CasePanelController.Instance.activeCase == null)
		{
			PopupMessageController.Instance.PopupMessage("NoActiveCase", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, Strings.Get("ui.interface", "New Case", Strings.Casing.asIs, false, false, false, null), false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.onCreateNewCasePopupCancel;
			PopupMessageController.Instance.OnRightButton += this.OnCreateNewCasePopup;
			return null;
		}
		EvidenceStickyNote passedEvidence = EvidenceCreator.Instance.CreateEvidence("PlayerStickyNote", "StickyNote" + InterfaceController.assignStickyNoteID.ToString(), null, null, null, null, null, true, null) as EvidenceStickyNote;
		InterfaceController.assignStickyNoteID++;
		return InterfaceController.Instance.SpawnWindow(passedEvidence, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x00186D05 File Offset: 0x00184F05
	public void OnCreateNewCasePopup()
	{
		PopupMessageController.Instance.OnLeftButton -= this.onCreateNewCasePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnCreateNewCasePopup;
		CasePanelController.Instance.OnCreateNewCustomCase();
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x00186D3D File Offset: 0x00184F3D
	public void onCreateNewCasePopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.onCreateNewCasePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnCreateNewCasePopup;
	}

	// Token: 0x06001AA7 RID: 6823 RVA: 0x00186D6C File Offset: 0x00184F6C
	public void PinToCasePanel(Case toCase, Evidence ev, Evidence.DataKey evKey, bool forceAutoPin = false, Vector2 localPostion = default(Vector2), bool debugFlag = false)
	{
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		list.Add(evKey);
		this.PinToCasePanel(toCase, ev, list, forceAutoPin, localPostion, debugFlag);
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x00186D98 File Offset: 0x00184F98
	public void PinToCasePanel(Case toCase, Evidence ev, List<Evidence.DataKey> evKeys, bool forceAutoPin = false, Vector2 localPostion = default(Vector2), bool debugFlag = false)
	{
		if (ev == null)
		{
			Game.Log("Evidence: Null evidence for pinning to case board.", 2);
			return;
		}
		Game.Log("Evidence: Pinning evidence " + ev.evID, 2);
		foreach (Case.CaseElement caseElement in toCase.caseElements)
		{
			if (caseElement.id == ev.evID)
			{
				List<Evidence.DataKey> tiedKeys = ev.GetTiedKeys(caseElement.dk);
				bool flag = false;
				foreach (Evidence.DataKey dataKey in evKeys)
				{
					if (ev.preset.IsKeyUnique(dataKey) && tiedKeys.Contains(dataKey))
					{
						Game.Log("Evidence: Found existing data key " + dataKey.ToString() + " matches previously spawned evidence tied data keys...", 2);
						flag = true;
					}
				}
				if (flag)
				{
					return;
				}
			}
		}
		InfoWindow window = InterfaceController.Instance.GetWindow(ev, evKeys);
		Case.CaseElement newElement = new Case.CaseElement();
		newElement.caseID = toCase.id;
		newElement.n = ev.GetNameForDataKey(Evidence.DataKey.name) + "(" + ev.evID + ")";
		newElement.id = ev.evID;
		newElement.dk = evKeys;
		newElement.v = localPostion;
		newElement.ap = forceAutoPin;
		toCase.caseElements.Add(newElement);
		if (AchievementsController.Instance != null && AchievementsController.Instance.allConnectedReference.Count < 100 && !AchievementsController.Instance.allConnectedReference.Contains(ev.evID))
		{
			AchievementsController.Instance.allConnectedReference.Add(ev.evID);
			if (AchievementsController.Instance.allConnectedReference.Count >= 100)
			{
				AchievementsController.Instance.UnlockAchievement("It’s all Connected!", "pin_100_things");
			}
		}
		ev.OnDataKeyChange += this.UpdatePinned;
		ev.OnDiscoverConnectedFact += this.UpdateStrings;
		ev.OnDiscoverChild += this.UpdateStrings;
		if (window != null)
		{
			newElement.w = true;
			if (window.pinButton.contextMenu.disabledItems.Contains("CreateCustomString"))
			{
				window.pinButton.contextMenu.disabledItems.Remove("CreateCustomString");
			}
			window.SetWorldInteraction(false);
			window.OnWindowPinnedChange(true, newElement);
			newElement.SetColour(window.evColour);
			if (this.OnPinnedChange != null)
			{
				this.OnPinnedChange();
				this.OnPinEvidence(ev);
			}
		}
		this.UpdatePinned();
		if (this.controllerMode)
		{
			PinnedItemController pinnedItemController = this.spawnedPins.Find((PinnedItemController item) => item.caseElement == newElement);
			if (pinnedItemController != null)
			{
				this.SetSelectedPinned(pinnedItemController, false);
			}
		}
		ev.OnPinnedChange();
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x001870D0 File Offset: 0x001852D0
	public void UnPinFromCasePanel(Case thisCase, Evidence ev, List<Evidence.DataKey> evKeys, bool uniqueKeysOnly = false, Case.CaseElement forceElement = null)
	{
		if (ev == null)
		{
			return;
		}
		List<Case.CaseElement> list = thisCase.caseElements.FindAll((Case.CaseElement item) => item.id == ev.evID);
		List<Case.CaseElement> list2 = new List<Case.CaseElement>();
		foreach (Case.CaseElement caseElement in list)
		{
			List<Evidence.DataKey> tiedKeys = ev.GetTiedKeys(caseElement.dk);
			foreach (Evidence.DataKey dataKey in evKeys)
			{
				if ((!uniqueKeysOnly || ev.preset.IsKeyUnique(dataKey)) && tiedKeys.Contains(dataKey))
				{
					list2.Add(caseElement);
				}
			}
		}
		if (forceElement != null && !list2.Contains(forceElement))
		{
			list2.Add(forceElement);
		}
		while (list2.Count > 0)
		{
			Game.Log("Interface: Removing case element " + ev.evID, 2);
			thisCase.caseElements.Remove(list2[0]);
			list2.RemoveAt(0);
		}
		ev.OnDataKeyChange -= this.UpdatePinned;
		ev.OnDiscoverConnectedFact -= this.UpdateStrings;
		ev.OnDiscoverChild -= this.UpdateStrings;
		InfoWindow window = InterfaceController.Instance.GetWindow(ev, evKeys);
		if (window != null)
		{
			if (!window.pinButton.contextMenu.disabledItems.Contains("CreateCustomString"))
			{
				window.pinButton.contextMenu.disabledItems.Add("CreateCustomString");
			}
			window.OnWindowPinnedChange(false, null);
		}
		this.UpdatePinned();
		if (this.OnPinnedChange != null)
		{
			this.OnPinnedChange();
			this.OnUnpinEvidence(ev);
		}
		ev.OnPinnedChange();
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x001872EC File Offset: 0x001854EC
	public void UpdatePinned()
	{
		if (MainMenuController.Instance.mainMenuActive)
		{
			return;
		}
		List<Case.CaseElement> list = new List<Case.CaseElement>();
		if (this.activeCase != null)
		{
			list.AddRange(this.activeCase.caseElements);
		}
		for (int i = 0; i < this.spawnedPins.Count; i++)
		{
			PinnedItemController pinnedItemController = this.spawnedPins[i];
			if (list.Contains(pinnedItemController.caseElement))
			{
				pinnedItemController.UpdateTooltipText();
				list.Remove(pinnedItemController.caseElement);
			}
			else
			{
				Object.Destroy(pinnedItemController.gameObject);
				this.spawnedPins.RemoveAt(i);
				i--;
			}
		}
		foreach (Case.CaseElement caseElement in list)
		{
			PinnedItemController component = Object.Instantiate<GameObject>(PrefabControls.Instance.casePanelObject, this.pinnedContainer).GetComponent<PinnedItemController>();
			component.Setup(caseElement);
			caseElement.pinnedController = component;
			this.spawnedPins.Add(component);
			component.rect.localPosition = caseElement.v;
			string text = "Set pinned local pos from case element (spawn): ";
			Vector2 v = caseElement.v;
			Game.Log(text + v.ToString(), 2);
		}
		this.UpdateStrings();
		if (this.pickModeActive && this.pickForField != null)
		{
			using (List<PinnedItemController>.Enumerator enumerator2 = this.spawnedPins.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					PinnedItemController pinnedItemController2 = enumerator2.Current;
					if (!(pinnedItemController2.evidenceButton == null))
					{
						if (pinnedItemController2.evidence is EvidenceCitizen && this.pickForField.question.inputType == Case.InputType.citizen && pinnedItemController2.caseElement.dk.Contains(Evidence.DataKey.name))
						{
							pinnedItemController2.evidenceButton.juice.Pulsate(true, false);
						}
						else if (pinnedItemController2.evidence is EvidenceLocation && this.pickForField.question.inputType == Case.InputType.location)
						{
							pinnedItemController2.evidenceButton.juice.Pulsate(true, false);
						}
						else if (pinnedItemController2.evidence != null && pinnedItemController2.evidence.interactable != null && this.pickForField.question.inputType == Case.InputType.item)
						{
							pinnedItemController2.evidenceButton.juice.Pulsate(true, false);
						}
						else
						{
							pinnedItemController2.evidenceButton.juice.Pulsate(false, false);
						}
					}
				}
				return;
			}
		}
		foreach (PinnedItemController pinnedItemController3 in this.spawnedPins)
		{
			if (pinnedItemController3.evidenceButton != null)
			{
				pinnedItemController3.evidenceButton.juice.Pulsate(false, false);
			}
		}
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x00187610 File Offset: 0x00185810
	public void UpdateStrings()
	{
		List<CasePanelController.StringConnection> list = new List<CasePanelController.StringConnection>();
		using (List<PinnedItemController>.Enumerator enumerator = this.spawnedPins.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PinnedItemController pic = enumerator.Current;
				if (!(pic == null) && pic.evidence != null && pic.caseElement != null)
				{
					foreach (Evidence.FactLink factLink in pic.evidence.GetFactsForDataKey(pic.caseElement.dk))
					{
						if (factLink != null && factLink.fact != null && factLink.fact.isFound && factLink.thisIsTheFromEvidence)
						{
							List<Evidence> toEvidence = factLink.fact.toEvidence;
							List<Evidence.DataKey> toDataKeys = factLink.fact.toDataKeys;
							foreach (Evidence evidence in toEvidence)
							{
								if (evidence != null && evidence.isFound)
								{
									PinnedItemController otherPinned = evidence.GetPinned(toDataKeys);
									if (otherPinned != null)
									{
										int num = list.FindIndex((CasePanelController.StringConnection item) => (item.from == pic && item.to == otherPinned) || (item.from == otherPinned && item.to == pic));
										if (num > -1)
										{
											list[num].links.Add(factLink);
											if (!list[num].facts.Contains(factLink.fact))
											{
												list[num].facts.Add(factLink.fact);
											}
										}
										else
										{
											CasePanelController.StringConnection stringConnection = new CasePanelController.StringConnection(pic, otherPinned);
											stringConnection.links.Add(factLink);
											if (!stringConnection.facts.Contains(factLink.fact))
											{
												stringConnection.facts.Add(factLink.fact);
											}
											list.Add(stringConnection);
										}
									}
								}
							}
						}
					}
				}
			}
		}
		for (int i = 0; i < this.spawnedStrings.Count; i++)
		{
			StringController spawned = this.spawnedStrings[i];
			if (spawned == null)
			{
				this.spawnedStrings.RemoveAt(i);
				i--;
			}
			else
			{
				int num2 = list.FindIndex((CasePanelController.StringConnection item) => item == spawned.connection);
				if (num2 > -1)
				{
					spawned.connection = list[num2];
					list.Remove(spawned.connection);
				}
				else
				{
					spawned.connection.from.OnMoved -= spawned.UpdatePosition;
					spawned.connection.to.OnMoved -= spawned.UpdatePosition;
					Object.Destroy(spawned.gameObject);
					this.spawnedStrings.RemoveAt(i);
					i--;
				}
			}
		}
		foreach (CasePanelController.StringConnection newConnection in list)
		{
			StringController component = Object.Instantiate<GameObject>(PrefabControls.Instance.stringLink, this.stringContainer).GetComponent<StringController>();
			component.Setup(newConnection);
			this.spawnedStrings.Add(component);
		}
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x00187A40 File Offset: 0x00185C40
	public void CustomStringLinkSelection(PinnedItemController pinnedItem, bool holdButtonMode = false)
	{
		if (this.customStringLinkSelection == pinnedItem)
		{
			return;
		}
		if (this.customLinkSelectionMode)
		{
			this.CancelCustomStringLinkSelection();
			return;
		}
		this.customStringLinkSelection = pinnedItem;
		base.StartCoroutine(this.CustomStringLink(holdButtonMode));
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x00187A75 File Offset: 0x00185C75
	private IEnumerator CustomStringLink(bool holdButtonMode = false)
	{
		Game.Log("Interface: Start custom string link selection...", 2);
		this.customLinkSelectionMode = true;
		RectTransform fromRect = null;
		if (this.customString == null)
		{
			Game.Log("Interface: Creating new selection linking string...", 2);
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.customStringLinkSelect, this.stringContainer);
			this.customString = gameObject.GetComponent<RectTransform>();
			InterfaceController.Instance.UpdateCursorSprite();
			fromRect = this.customStringLinkSelection.pinButtonController.rect;
		}
		int waitFrames = 0;
		while (this.customStringLinkSelection != null && this.customLinkSelectionMode && fromRect != null)
		{
			if (!InterfaceController.Instance.desktopMode)
			{
				this.CancelCustomStringLinkSelection();
			}
			if (!holdButtonMode)
			{
				if (Input.GetMouseButtonDown(1) && InputController.Instance.mouseInputMode)
				{
					this.CancelCustomStringLinkSelection();
				}
				if (InputController.Instance.player.GetButtonDown("CreateString"))
				{
					this.CancelCustomStringLinkSelection();
				}
			}
			if ((Input.GetMouseButtonDown(0) && InputController.Instance.mouseInputMode && !holdButtonMode) || (!InputController.Instance.player.GetButton("CreateString") && holdButtonMode) || (waitFrames > 2 && InputController.Instance.player.GetButtonDown("Select") && !InputController.Instance.mouseInputMode))
			{
				GameObject gameObject2 = null;
				if (InterfaceController.Instance.selectedPinned.Count > 0)
				{
					gameObject2 = InterfaceController.Instance.selectedPinned[InterfaceController.Instance.selectedPinned.Count - 1].gameObject;
				}
				string text = "Interface: String link target: ";
				GameObject gameObject3 = gameObject2;
				Game.Log(text + ((gameObject3 != null) ? gameObject3.ToString() : null), 2);
				if (gameObject2 == null)
				{
					this.CancelCustomStringLinkSelection();
				}
				else
				{
					PinnedItemController componentInParent = gameObject2.GetComponentInParent<PinnedItemController>();
					if (componentInParent != null && componentInParent != this.customStringLinkSelection)
					{
						string text2 = "Interface: Found pin folder: ";
						PinnedItemController pinnedItemController = componentInParent;
						Game.Log(text2 + ((pinnedItemController != null) ? pinnedItemController.ToString() : null), 2);
						this.FinishCustomStringLinkSelection(componentInParent);
					}
				}
			}
			Vector3 vector = InterfaceControls.Instance.caseBoardCursorRBContainer.position - fromRect.position;
			this.customString.sizeDelta = new Vector2(vector.magnitude, this.customString.sizeDelta.y);
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			this.customString.rotation = Quaternion.Euler(0f, 0f, num);
			this.customString.position = fromRect.position;
			int num2 = waitFrames;
			waitFrames = num2 + 1;
			yield return null;
		}
		if (this.customLinkSelectionMode)
		{
			this.CancelCustomStringLinkSelection();
		}
		yield break;
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x00187A8B File Offset: 0x00185C8B
	private void OnDisable()
	{
		this.CancelCustomStringLinkSelection();
		if (this.customString != null)
		{
			Object.Destroy(this.customString.gameObject);
		}
		this.SetPickModeActive(false, null);
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x00187ABC File Offset: 0x00185CBC
	public void CancelCustomStringLinkSelection()
	{
		Game.Log("Interface: End string link selection mode", 2);
		this.customStringLinkSelection = null;
		this.customLinkSelectionMode = false;
		if (this.customString != null)
		{
			Object.Destroy(this.customString.gameObject);
		}
		InterfaceController.Instance.UpdateCursorSprite();
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x00187B0C File Offset: 0x00185D0C
	public void FinishCustomStringLinkSelection(PinnedItemController target)
	{
		Game.Log("Interface: Finish string link: " + ((target != null) ? target.ToString() : null), 2);
		this.newestCreatedFact = (EvidenceCreator.Instance.CreateFact("CustomLink", this.customStringLinkSelection.evidence, target.evidence, null, null, false, null, this.customStringLinkSelection.caseElement.dk, target.caseElement.dk, true) as FactCustom);
		if (this.customString != null)
		{
			Object.Destroy(this.customString.gameObject);
		}
		this.customStringLinkSelection = null;
		this.customLinkSelectionMode = false;
		InterfaceController.Instance.UpdateCursorSprite();
		PopupMessageController.Instance.PopupMessage("CustomFactRename", true, true, "Cancel", "Continue", true, PopupMessageController.AffectPauseState.no, true, Strings.Get("evidence.generic", "customlink", Strings.Casing.asIs, false, false, false, null), false, true, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnLeftButton += this.OnCancelCustomFact;
		PopupMessageController.Instance.OnRightButton += this.OnContinueFactName;
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x00187C34 File Offset: 0x00185E34
	public void OnContinueFactName()
	{
		try
		{
			if (this.newestCreatedFact != null)
			{
				this.newestCreatedFact.SetCustomName(PopupMessageController.Instance.inputField.text);
				this.UpdateStrings();
				this.spawnedStrings.Find((StringController item) => item != null && item.connection != null && item.connection.facts.Exists((Fact item2) => item2 == this.newestCreatedFact)).SetColour(PopupMessageController.Instance.colourPicker.GetCurrentSelectedEvidenceColourValue());
			}
			PopupMessageController.Instance.OnLeftButton -= this.OnCancelCustomFact;
			PopupMessageController.Instance.OnRightButton -= this.OnContinueFactName;
		}
		catch
		{
		}
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x00187CD8 File Offset: 0x00185ED8
	public void OnCancelCustomFact()
	{
		if (this.newestCreatedFact != null)
		{
			if (this.newestCreatedFact.fromEvidence != null)
			{
				for (int i = 0; i < this.newestCreatedFact.fromEvidence.Count; i++)
				{
					this.newestCreatedFact.fromEvidence[i].RemoveFactLink(this.newestCreatedFact);
				}
			}
			if (this.newestCreatedFact.toEvidence != null)
			{
				for (int j = 0; j < this.newestCreatedFact.toEvidence.Count; j++)
				{
					this.newestCreatedFact.toEvidence[j].RemoveFactLink(this.newestCreatedFact);
				}
			}
			GameplayController.Instance.factList.Remove(this.newestCreatedFact);
		}
		PopupMessageController.Instance.OnLeftButton -= this.OnCancelCustomFact;
		PopupMessageController.Instance.OnRightButton -= this.OnContinueFactName;
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x00187DBC File Offset: 0x00185FBC
	public void UpdateResolveNotifications()
	{
		if (this.activeCase == null || !this.activeCase.isActive)
		{
			this.closeCaseButton.notifications.SetNotifications(0);
			return;
		}
		if (this.activeCase.caseStatus == Case.CaseStatus.handInCollected)
		{
			int num = 0;
			foreach (Case.ResolveQuestion resolveQuestion in this.activeCase.resolveQuestions)
			{
				if (!resolveQuestion.isValid || resolveQuestion.progress < 1f)
				{
					num++;
				}
			}
			this.closeCaseButton.notifications.SetNotifications(num);
			return;
		}
		this.closeCaseButton.notifications.SetNotifications(0);
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x00187E80 File Offset: 0x00186080
	public void SetPickModeActive(bool val, InputFieldController forField)
	{
		this.pickModeActive = val;
		this.pickForField = forField;
		InterfaceController.Instance.UpdateCursorSprite();
		if (ResolveController.Instance != null)
		{
			if (this.pickModeActive)
			{
				ResolveController.Instance.wcc.window.windowCanvasGroup.alpha = 0.02f;
				ResolveController.Instance.wcc.window.contentCanvasGroup.alpha = 0.02f;
				ResolveController.Instance.wcc.window.windowCanvasGroup.blocksRaycasts = false;
				ResolveController.Instance.wcc.window.contentCanvasGroup.blocksRaycasts = false;
				if (!InputController.Instance.mouseInputMode)
				{
					this.SetControllerMode(true, CasePanelController.ControllerSelectMode.caseBoard);
				}
			}
			else
			{
				ResolveController.Instance.wcc.window.windowCanvasGroup.alpha = 1f;
				ResolveController.Instance.wcc.window.contentCanvasGroup.alpha = 1f;
				ResolveController.Instance.wcc.window.windowCanvasGroup.blocksRaycasts = true;
				ResolveController.Instance.wcc.window.contentCanvasGroup.blocksRaycasts = true;
				if (!InputController.Instance.mouseInputMode)
				{
					this.SetControllerMode(true, CasePanelController.ControllerSelectMode.windows);
				}
			}
		}
		this.UpdatePinned();
		this.UpdateCloseCaseButton();
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x00187FDC File Offset: 0x001861DC
	public void OnShowCaseBoard()
	{
		this.SetControllerMode(!InputController.Instance.mouseInputMode, this.currentSelectMode);
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x00187FF7 File Offset: 0x001861F7
	public void OnHideCaseBoard()
	{
		this.SetControllerMode(false, this.currentSelectMode);
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x00188028 File Offset: 0x00186228
	public void SetControllerMode(bool isActive, CasePanelController.ControllerSelectMode newMode)
	{
		if (isActive != this.controllerMode)
		{
			Game.Log("Menu: Set caseboard controller section active: " + isActive.ToString(), 2);
			this.controllerMode = isActive;
		}
		if (isActive)
		{
			if (newMode == CasePanelController.ControllerSelectMode.windows)
			{
				if (InterfaceController.Instance.activeWindows.Count <= 0)
				{
					return;
				}
			}
			else
			{
				InterfaceController.Instance.RemoveWindowFocus();
			}
			if (newMode != CasePanelController.ControllerSelectMode.caseBoard && PinnedItemController.activeQuickMenu != null)
			{
				PinnedItemController.activeQuickMenu.Remove(false);
			}
			if ((UpgradesController.Instance.isOpen || BioScreenController.Instance.isOpen) && newMode != CasePanelController.ControllerSelectMode.topBar)
			{
				Game.Log("Menu: Cancelling caseboard mode as upgrades/inventory is open...", 2);
				return;
			}
			if (newMode != this.currentSelectMode)
			{
				if (ContextMenuController.activeMenu != null)
				{
					ContextMenuController.activeMenu.ForceClose();
				}
				if (InterfaceController.Instance.selectedElement != null)
				{
					InterfaceController.Instance.selectedElement.OnDeselect();
				}
			}
			this.currentSelectMode = newMode;
			Game.Log("Menu: Set caseboard controller section mode: " + this.currentSelectMode.ToString(), 2);
			if (this.currentSelectMode == CasePanelController.ControllerSelectMode.topBar)
			{
				this.topMenuSelect.gameObject.SetActive(true);
				this.upgradesSelect.gameObject.SetActive(true);
				this.boardSelect.gameObject.SetActive(false);
				this.mapSelect.gameObject.SetActive(true);
				if (InterfaceController.Instance.selectedElement == null || InterfaceController.Instance.selectedElementTag != "TopPanelButtons")
				{
					if (BioScreenController.Instance.isOpen)
					{
						BioScreenController.Instance.selectNothingButton.OnSelect();
					}
					else if (this.selectedTopBarButton != null)
					{
						this.selectedTopBarButton.OnSelect();
					}
					else
					{
						this.notebookButton.OnSelect();
					}
				}
				this.caseboardMO.ForceMouseOver(false);
				this.caseboardScroll.controlEnabled = false;
				this.mapMO.ForceMouseOver(false);
				this.mapScroll.controlEnabled = false;
				this.upgradesMO.ForceMouseOver(UpgradesController.Instance.isOpen);
				this.upgradesScroll.controlEnabled = UpgradesController.Instance.isOpen;
			}
			else if (this.currentSelectMode == CasePanelController.ControllerSelectMode.caseBoard)
			{
				this.topMenuSelect.gameObject.SetActive(false);
				this.upgradesSelect.gameObject.SetActive(false);
				this.boardSelect.gameObject.SetActive(true);
				this.mapSelect.gameObject.SetActive(false);
				this.caseboardMO.ForceMouseOver(true);
				this.caseboardScroll.controlEnabled = true;
				this.mapMO.ForceMouseOver(false);
				this.mapScroll.controlEnabled = false;
				this.upgradesMO.ForceMouseOver(false);
				this.upgradesScroll.controlEnabled = false;
				if (this.selectedPinned == null && this.spawnedPins.Count > 0)
				{
					this.SetSelectedPinned(this.GetClosestPinnedToCentre(), false);
				}
			}
			else if (this.currentSelectMode == CasePanelController.ControllerSelectMode.windows)
			{
				this.topMenuSelect.gameObject.SetActive(false);
				this.upgradesSelect.gameObject.SetActive(false);
				this.boardSelect.gameObject.SetActive(false);
				this.mapSelect.gameObject.SetActive(false);
				if (this.selectedWindow == null)
				{
					if (InterfaceController.Instance.activeWindows.Count > 0)
					{
						this.SetSelectedWindow(InterfaceController.Instance.activeWindows[0], false);
					}
					else
					{
						this.SetControllerMode(true, CasePanelController.ControllerSelectMode.caseBoard);
					}
				}
				this.mapMO.ForceMouseOver(false);
				this.mapScroll.controlEnabled = false;
				this.caseboardMO.ForceMouseOver(false);
				this.caseboardScroll.controlEnabled = false;
				this.upgradesMO.ForceMouseOver(false);
				this.upgradesScroll.controlEnabled = false;
			}
		}
		else
		{
			this.topMenuSelect.gameObject.SetActive(false);
			this.upgradesSelect.gameObject.SetActive(false);
			this.boardSelect.gameObject.SetActive(false);
			this.mapSelect.gameObject.SetActive(false);
			this.caseboardScroll.controlEnabled = false;
			this.mapScroll.controlEnabled = false;
			this.upgradesScroll.controlEnabled = false;
		}
		foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
		{
			infoWindow.UpdateControllerSelected();
		}
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x001884A8 File Offset: 0x001866A8
	public void SetSelectedWindow(InfoWindow newWindow, bool forceUpdate = false)
	{
		if (this.selectedWindow != newWindow || forceUpdate)
		{
			this.selectedWindow = newWindow;
			if (ContextMenuController.activeMenu != null)
			{
				ContextMenuController.activeMenu.ForceClose();
			}
			Game.Log("Menu: Set new controller selected window: " + newWindow.name, 2);
			foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
			{
				if (this.selectedWindow == infoWindow)
				{
					infoWindow.SetSelected(true);
				}
				else
				{
					infoWindow.SetSelected(false);
				}
			}
			if (this.selectedWindow != null && this.currentSelectMode == CasePanelController.ControllerSelectMode.windows)
			{
				if (InterfaceController.Instance.selectedElement != null)
				{
					InterfaceController.Instance.selectedElement.OnDeselect();
				}
				if (this.selectedWindow.pinButton != null)
				{
					this.selectedWindow.pinButton.OnSelect();
					return;
				}
				if (this.selectedWindow.activeContent != null)
				{
					List<ButtonController> list = Enumerable.ToList<ButtonController>(this.selectedWindow.activeContent.GetComponentsInChildren<ButtonController>(true));
					float num = -1E+11f;
					ButtonController buttonController = null;
					foreach (ButtonController buttonController2 in list)
					{
						if (buttonController == null || buttonController2.transform.position.y > num)
						{
							num = buttonController2.transform.position.y;
							buttonController = buttonController2;
						}
					}
					if (buttonController != null)
					{
						buttonController.OnSelect();
						return;
					}
					if (this.selectedWindow.closable && this.selectedWindow.closeButton != null)
					{
						this.selectedWindow.closeButton.OnSelect();
						return;
					}
					if (this.selectedWindow.tabs.Count > 0)
					{
						this.selectedWindow.tabs[0].tabButton.gameObject.GetComponent<ButtonController>().OnSelect();
					}
				}
			}
		}
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x001886D8 File Offset: 0x001868D8
	public void SetSelectedPinned(PinnedItemController newPinned, bool forceUpdate = false)
	{
		if (this.selectedPinned != newPinned || forceUpdate)
		{
			this.selectedPinned = newPinned;
			if (ContextMenuController.activeMenu != null)
			{
				ContextMenuController.activeMenu.ForceClose();
			}
			Game.Log("Menu: Set new controller selected pinned: " + newPinned.name, 2);
			if (this.selectedPinned != null && this.currentSelectMode == CasePanelController.ControllerSelectMode.caseBoard)
			{
				if (InterfaceController.Instance.selectedElement != null)
				{
					InterfaceController.Instance.selectedElement.OnDeselect();
				}
				if (PinnedItemController.activeQuickMenu != null && PinnedItemController.activeQuickMenu.parentPinned != this.selectedPinned)
				{
					PinnedItemController.activeQuickMenu.Remove(false);
				}
				foreach (PinnedItemController pinnedItemController in this.spawnedPins)
				{
					if (pinnedItemController == this.selectedPinned)
					{
						pinnedItemController.SetSelected(true, false);
					}
					else
					{
						pinnedItemController.SetSelected(false, false);
					}
				}
				this.selectedPinned.evidenceButton.OnSelect();
			}
			InteractionController.Instance.UpdateInteractionText();
		}
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x00188814 File Offset: 0x00186A14
	public void ControllerNavigate(Vector2 direction)
	{
		if (ContextMenuController.activeMenu == null && this.currentSelectMode == CasePanelController.ControllerSelectMode.caseBoard)
		{
			if (InterfaceController.Instance.selectedElement != null)
			{
				PinnedItemController pinnedItemController = null;
				ButtonController buttonController = null;
				float num = float.PositiveInfinity;
				foreach (PinnedItemController pinnedItemController2 in this.spawnedPins)
				{
					if (!(pinnedItemController2 == this.selectedPinned))
					{
						Vector2 vector = pinnedItemController2.rect.position - InterfaceController.Instance.selectedElement.transform.position;
						float num2 = Vector2.Distance(pinnedItemController2.rect.position, InterfaceController.Instance.selectedElement.transform.position);
						if (direction.x > 0f && vector.x > 0f)
						{
							if (num2 < num)
							{
								pinnedItemController = pinnedItemController2;
								num = num2;
							}
						}
						else if (direction.x < 0f && vector.x < 0f)
						{
							if (num2 < num)
							{
								pinnedItemController = pinnedItemController2;
								num = num2;
							}
						}
						else if (direction.y > 0f && vector.y > 0f)
						{
							if (num2 < num)
							{
								pinnedItemController = pinnedItemController2;
								num = num2;
							}
						}
						else if (direction.y < 0f && vector.y < 0f && num2 < num)
						{
							pinnedItemController = pinnedItemController2;
							num = num2;
						}
					}
				}
				if (PinnedItemController.activeQuickMenu != null)
				{
					foreach (ButtonController buttonController2 in PinnedItemController.activeQuickMenu.activeButtons)
					{
						if (!(InterfaceController.Instance.selectedElement == buttonController2.button) && !(buttonController2 == null))
						{
							Vector2 vector2 = buttonController2.rect.position - InterfaceController.Instance.selectedElement.transform.position;
							float num3 = Vector2.Distance(buttonController2.rect.position, InterfaceController.Instance.selectedElement.transform.position);
							if (direction.x > 0f && vector2.x > 0f)
							{
								if (num3 < num)
								{
									buttonController = buttonController2;
									num = num3;
								}
							}
							else if (direction.x < 0f && vector2.x < 0f)
							{
								if (num3 < num)
								{
									buttonController = buttonController2;
									num = num3;
								}
							}
							else if (direction.y > 0f && vector2.y > 0f)
							{
								if (num3 < num)
								{
									buttonController = buttonController2;
									num = num3;
								}
							}
							else if (direction.y < 0f && vector2.y < 0f && num3 < num)
							{
								buttonController = buttonController2;
								num = num3;
							}
						}
					}
				}
				if (buttonController != null)
				{
					InterfaceController.Instance.selectedElement.OnDeselect();
					buttonController.OnSelect();
					buttonController.AutoScroll();
					return;
				}
				if (pinnedItemController != null)
				{
					this.SetSelectedPinned(pinnedItemController, false);
					pinnedItemController.evidenceButton.AutoScroll();
					return;
				}
			}
			else if (this.spawnedPins.Count > 0)
			{
				this.SetSelectedPinned(this.GetClosestPinnedToCentre(), false);
			}
		}
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x00188BA0 File Offset: 0x00186DA0
	public void ShoulderNavigate(bool right)
	{
		if (ContextMenuController.activeMenu == null)
		{
			if (this.currentSelectMode == CasePanelController.ControllerSelectMode.topBar && !this.mapScroll.controlEnabled)
			{
				if (InterfaceController.Instance.selectedElement != null)
				{
					Selectable selectable = InterfaceController.Instance.selectedElement.button.FindSelectableOnLeft();
					if (right)
					{
						selectable = InterfaceController.Instance.selectedElement.button.FindSelectableOnRight();
					}
					if (selectable != null)
					{
						InterfaceController.Instance.selectedElement.OnDeselect();
						ButtonController component = selectable.gameObject.GetComponent<ButtonController>();
						if (component != null)
						{
							component.OnSelect();
							return;
						}
					}
				}
			}
			else if (this.currentSelectMode == CasePanelController.ControllerSelectMode.windows && this.selectedWindow != null)
			{
				InfoWindow infoWindow = null;
				float num = float.PositiveInfinity;
				foreach (InfoWindow infoWindow2 in InterfaceController.Instance.activeWindows)
				{
					if (!(infoWindow2 == this.selectedWindow))
					{
						float num2 = this.selectedWindow.transform.position.x - infoWindow2.transform.position.x;
						if (((right && num2 <= 0f) || (!right && num2 > 0f)) && Mathf.Abs(num2) < num)
						{
							infoWindow = infoWindow2;
							num = Mathf.Abs(num2);
						}
					}
				}
				if (infoWindow != null)
				{
					InterfaceController.Instance.RemoveWindowFocus();
					this.SetSelectedWindow(infoWindow, false);
				}
			}
		}
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x00188D3C File Offset: 0x00186F3C
	public PinnedItemController GetClosestPinnedToCentre()
	{
		PinnedItemController result = null;
		float num = float.PositiveInfinity;
		foreach (PinnedItemController pinnedItemController in this.spawnedPins)
		{
			float num2 = Vector3.Distance(pinnedItemController.transform.position, this.boardSelect.transform.position);
			if (num2 < num)
			{
				result = pinnedItemController;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x04002332 RID: 9010
	[Header("References")]
	public RectTransform corkBoard;

	// Token: 0x04002333 RID: 9011
	public RectTransform pinnedContainer;

	// Token: 0x04002334 RID: 9012
	public RectTransform stringContainer;

	// Token: 0x04002335 RID: 9013
	public RectTransform caseButtonParent;

	// Token: 0x04002336 RID: 9014
	public ButtonController newCaseButton;

	// Token: 0x04002337 RID: 9015
	public ButtonController closeCaseButton;

	// Token: 0x04002338 RID: 9016
	public RectTransform caseDisplayArea;

	// Token: 0x04002339 RID: 9017
	public RectTransform closeCaseDisplayArea;

	// Token: 0x0400233A RID: 9018
	public Sprite resolveSprite;

	// Token: 0x0400233B RID: 9019
	public Sprite archiveSprite;

	// Token: 0x0400233C RID: 9020
	public Sprite collectHandInSprite;

	// Token: 0x0400233D RID: 9021
	[Header("Cases")]
	[NonSerialized]
	public Case activeCase;

	// Token: 0x0400233E RID: 9022
	public List<Case> activeCases = new List<Case>();

	// Token: 0x0400233F RID: 9023
	public List<Case> archivedCases = new List<Case>();

	// Token: 0x04002340 RID: 9024
	public List<CaseButtonController> spawnedCaseButtons = new List<CaseButtonController>();

	// Token: 0x04002341 RID: 9025
	[Header("Pick Evidence Mode")]
	public bool pickModeActive;

	// Token: 0x04002342 RID: 9026
	public InputFieldController pickForField;

	// Token: 0x04002343 RID: 9027
	[Header("String Link")]
	public bool customLinkSelectionMode;

	// Token: 0x04002344 RID: 9028
	public PinnedItemController customStringLinkSelection;

	// Token: 0x04002345 RID: 9029
	public RectTransform customString;

	// Token: 0x04002346 RID: 9030
	private FactCustom newestCreatedFact;

	// Token: 0x04002347 RID: 9031
	[Header("Spawned")]
	public List<PinnedItemController> spawnedPins = new List<PinnedItemController>();

	// Token: 0x04002348 RID: 9032
	public List<StringController> spawnedStrings = new List<StringController>();

	// Token: 0x04002349 RID: 9033
	private float caseCloseTransition;

	// Token: 0x0400234A RID: 9034
	[Header("Controller Mode")]
	public bool controllerMode;

	// Token: 0x0400234B RID: 9035
	public InfoWindow selectedWindow;

	// Token: 0x0400234C RID: 9036
	public PinnedItemController selectedPinned;

	// Token: 0x0400234D RID: 9037
	public ButtonController selectedTopBarButton;

	// Token: 0x0400234E RID: 9038
	public CasePanelController.ControllerSelectMode currentSelectMode = CasePanelController.ControllerSelectMode.caseBoard;

	// Token: 0x0400234F RID: 9039
	public RectTransform topMenuSelect;

	// Token: 0x04002350 RID: 9040
	public JuiceController topMenuSelectJuice;

	// Token: 0x04002351 RID: 9041
	public RectTransform upgradesSelect;

	// Token: 0x04002352 RID: 9042
	public JuiceController upgradesSelectJuice;

	// Token: 0x04002353 RID: 9043
	public RectTransform boardSelect;

	// Token: 0x04002354 RID: 9044
	public JuiceController boardSelectJuice;

	// Token: 0x04002355 RID: 9045
	public RectTransform mapSelect;

	// Token: 0x04002356 RID: 9046
	public JuiceController mapSelectJuice;

	// Token: 0x04002357 RID: 9047
	public ButtonController notebookButton;

	// Token: 0x04002358 RID: 9048
	public ButtonController stickNoteButton;

	// Token: 0x04002359 RID: 9049
	public ButtonController selectNoCaseButton;

	// Token: 0x0400235A RID: 9050
	public ViewportMouseOver caseboardMO;

	// Token: 0x0400235B RID: 9051
	public ControllerViewRectScroll caseboardScroll;

	// Token: 0x0400235C RID: 9052
	public ViewportMouseOver mapMO;

	// Token: 0x0400235D RID: 9053
	public ControllerViewRectScroll mapScroll;

	// Token: 0x0400235E RID: 9054
	public ViewportMouseOver upgradesMO;

	// Token: 0x0400235F RID: 9055
	public ControllerViewRectScroll upgradesScroll;

	// Token: 0x04002363 RID: 9059
	private static CasePanelController _instance;

	// Token: 0x020004D1 RID: 1233
	public class StringConnection
	{
		// Token: 0x06001AC0 RID: 6848 RVA: 0x00188E4D File Offset: 0x0018704D
		public StringConnection(PinnedItemController fromPinned, PinnedItemController toPinned)
		{
			this.from = fromPinned;
			this.to = toPinned;
		}

		// Token: 0x04002364 RID: 9060
		public PinnedItemController from;

		// Token: 0x04002365 RID: 9061
		public PinnedItemController to;

		// Token: 0x04002366 RID: 9062
		public List<Evidence.FactLink> links = new List<Evidence.FactLink>();

		// Token: 0x04002367 RID: 9063
		public List<Fact> facts = new List<Fact>();
	}

	// Token: 0x020004D2 RID: 1234
	public enum ControllerSelectMode
	{
		// Token: 0x04002369 RID: 9065
		topBar,
		// Token: 0x0400236A RID: 9066
		caseBoard,
		// Token: 0x0400236B RID: 9067
		windows
	}

	// Token: 0x020004D3 RID: 1235
	// (Invoke) Token: 0x06001AC2 RID: 6850
	public delegate void PinnedChange();

	// Token: 0x020004D4 RID: 1236
	// (Invoke) Token: 0x06001AC6 RID: 6854
	public delegate void PinEvidence(Evidence evidence);

	// Token: 0x020004D5 RID: 1237
	// (Invoke) Token: 0x06001ACA RID: 6858
	public delegate void UnpinEvidence(Evidence evidence);
}
