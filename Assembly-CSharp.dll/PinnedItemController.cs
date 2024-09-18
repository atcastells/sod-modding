using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200060F RID: 1551
public class PinnedItemController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x1400004A RID: 74
	// (add) Token: 0x06002235 RID: 8757 RVA: 0x001D0ACC File Offset: 0x001CECCC
	// (remove) Token: 0x06002236 RID: 8758 RVA: 0x001D0B04 File Offset: 0x001CED04
	public event PinnedItemController.OnMove OnMoved;

	// Token: 0x06002237 RID: 8759 RVA: 0x001D0B3C File Offset: 0x001CED3C
	public void Setup(Case.CaseElement newElement)
	{
		this.caseElement = newElement;
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.rect.localScale = Vector3.one;
		if (PinnedItemController.angleSteps == null)
		{
			PinnedItemController.angleSteps = new List<float>();
			for (int i = 0; i < InterfaceControls.Instance.angleStepsCount; i++)
			{
				PinnedItemController.angleSteps.Add(360f / (float)InterfaceControls.Instance.angleStepsCount * (float)i);
			}
		}
		Toolbox.Instance.TryGetEvidence(this.caseElement.id, out this.evidence);
		if (this.evidence != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.evidenceButton, base.gameObject.transform, false);
			this.evidenceButton = gameObject.GetComponent<EvidenceButtonController>();
			this.evidenceButton.Setup(this.evidence, this.caseElement.dk, this);
			this.tooltip = this.evidenceButton.tooltip;
			if (this.evidenceButton.tooltip != null)
			{
				this.evidenceButton.tooltip.tooltipEnabled = true;
			}
			this.titleText.transform.SetAsLastSibling();
			this.newInfoIcon.SetAsLastSibling();
			this.pinnedRect.SetAsLastSibling();
			if (this.evidence.preset.pinnedStyle == EvidencePreset.PinnedStyle.stickNote)
			{
				this.rect.sizeDelta = new Vector2(150f, 150f);
				this.titleText.rectTransform.sizeDelta = new Vector2(this.titleText.rectTransform.sizeDelta.x, 130f);
				this.evidenceButton.rect.sizeDelta = new Vector2(140f, 140f);
				Object.Destroy(this.joint);
				Object.Destroy(this.rb);
				this.evidence.OnNoteAdded += this.VisualUpdate;
			}
			this.background.color = this.evidence.preset.pinnedBackgroundColour;
			gameObject.GetComponent<RectTransform>();
			this.evidence.OnDataKeyChange += this.VisualUpdate;
			base.name = this.evidenceButton.evidence.preset.name;
		}
		else
		{
			this.titleText.text = this.caseElement.n;
		}
		this.originalSize = this.rect.sizeDelta;
		this.dragController = base.gameObject.GetComponentInChildren<DragCasePanel>();
		this.dragController.Setup(this);
		this.VisualUpdate();
		if (this.caseElement.ap)
		{
			this.AutoPinPostion();
			this.caseElement.ap = false;
		}
		this.pinButtonController.Setup(this);
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x001D0DF4 File Offset: 0x001CEFF4
	private void OnDestroy()
	{
		this.SetSelected(false, false);
		if (InterfaceController.Instance.pinnedBeingDragged == this)
		{
			InterfaceController.Instance.pinnedBeingDragged = null;
		}
		if (PinnedItemController.activeQuickMenu != null)
		{
			PinnedItemController.activeQuickMenu.Remove(true);
		}
		if (this.evidence != null)
		{
			this.evidence.OnDataKeyChange -= this.VisualUpdate;
			if (this.evidence.preset.pinnedStyle == EvidencePreset.PinnedStyle.stickNote)
			{
				this.evidence.OnNoteAdded -= this.VisualUpdate;
			}
		}
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x001D0E87 File Offset: 0x001CF087
	public void SetPostion(Vector2 newPos)
	{
		this.rect.localPosition = newPos;
		this.OnMoveThis();
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x001D0EA0 File Offset: 0x001CF0A0
	public void AutoPinPostion()
	{
		Vector2 zero = Vector2.zero;
		int num = 0;
		float num2 = 0f;
		List<float> list = new List<float>();
		bool flag = false;
		int num3 = 9999;
		while (!flag && num3 > 0)
		{
			float num4 = (float)num * InterfaceControls.Instance.autoPinDistance;
			Vector2 vector;
			vector..ctor(zero.x + Mathf.Cos(num2) * num4, zero.y + Mathf.Sin(num2) * num4);
			flag = true;
			using (List<PinnedItemController>.Enumerator enumerator = CasePanelController.Instance.spawnedPins.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (Vector2.Distance(enumerator.Current.rect.localPosition, vector) <= InterfaceControls.Instance.pinnedEvidenceRadius)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.SetPostion(vector);
				return;
			}
			num3--;
			if (num <= 0)
			{
				num++;
				list.AddRange(PinnedItemController.angleSteps);
			}
			else if (list.Count <= 0)
			{
				num++;
				list.AddRange(PinnedItemController.angleSteps);
			}
			else
			{
				int num5 = Toolbox.Instance.Rand(0, list.Count, false);
				num2 = list[num5];
				list.RemoveAt(num5);
			}
		}
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x001D0FE8 File Offset: 0x001CF1E8
	public void OnMoveThis()
	{
		this.caseElement.v = this.rect.localPosition;
		if (this.juice != null)
		{
			this.juice.elements[0].originalLocalPos = this.rect.localPosition;
		}
		if (this.OnMoved != null)
		{
			this.OnMoved();
		}
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x001D1054 File Offset: 0x001CF254
	public void VisualUpdate()
	{
		if (this.evidence != null && this.evidence.preset.pinnedStyle == EvidencePreset.PinnedStyle.polaroid)
		{
			this.titleText.text = this.evidence.GetNameForDataKey(this.caseElement.dk);
		}
		this.UpdateNewInfoIcon();
		if (this.evidenceButton != null)
		{
			this.evidenceButton.VisualUpdate();
		}
		if (this.caseElement.co)
		{
			if (this.crossedOut == null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.crossOut, base.transform);
				this.crossedOut = gameObject.GetComponent<RectTransform>();
				this.titleText.rectTransform.SetAsLastSibling();
			}
		}
		else if (this.crossedOut != null)
		{
			Object.Destroy(this.crossedOut.gameObject);
		}
		if (this.caseElement.m)
		{
			this.Minimize();
		}
		else
		{
			this.Restore();
		}
		this.UpdateTooltipText();
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x001D114C File Offset: 0x001CF34C
	public void UpdateNewInfoIcon()
	{
		if (this.evidence != null && this.evidence.preset != null && this.evidence.preset.useDataKeys && this.caseElement != null)
		{
			bool active = false;
			if (this.caseElement.sdk != null)
			{
				if (this.evidence.GetTiedKeys(this.caseElement.dk).Count > this.caseElement.sdk.Count)
				{
					active = true;
				}
			}
			else
			{
				active = true;
			}
			if (this.newInfoIcon != null)
			{
				this.newInfoIcon.gameObject.SetActive(active);
				return;
			}
		}
		else if (this.newInfoIcon != null)
		{
			this.newInfoIcon.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x001D1212 File Offset: 0x001CF412
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			this.SetHovered(true);
		}
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x001D1227 File Offset: 0x001CF427
	public void OnPointerExit(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
			{
				return;
			}
			this.SetHovered(false);
		}
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x001D1250 File Offset: 0x001CF450
	public void SetHovered(bool val)
	{
		if (this.isOver != val)
		{
			this.isOver = val;
			if (this.isOver)
			{
				base.StartCoroutine(this.IsOver());
			}
		}
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x001D1278 File Offset: 0x001CF478
	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button != null)
		{
			return;
		}
		if (CasePanelController.Instance.customLinkSelectionMode)
		{
			return;
		}
		if (!InterfaceController.Instance.boxSelectActive && !this.permSelected)
		{
			for (int i = 0; i < InterfaceController.Instance.selectedPinned.Count; i++)
			{
				PinnedItemController pinnedItemController = InterfaceController.Instance.selectedPinned[i];
				if (pinnedItemController != this && pinnedItemController.permSelected)
				{
					pinnedItemController.SetSelected(false, false);
					i--;
				}
			}
		}
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x001D12F7 File Offset: 0x001CF4F7
	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button != null)
		{
			return;
		}
		this.ForcePointerUp();
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x001D1308 File Offset: 0x001CF508
	public void OnDrag(PointerEventData data)
	{
		if (data.button != null)
		{
			return;
		}
		this.ForceDrag();
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x001D131C File Offset: 0x001CF51C
	public void ForceDrag()
	{
		InterfaceController.Instance.AddMouseOverElement(this);
		if (!this.isDragging)
		{
			this.pinButtonController.mainColour.enabled = false;
			this.pinButtonController.mainOverlay.enabled = false;
			this.pinButtonController.icon.gameObject.SetActive(true);
			this.prevLocalPos = this.rect.localPosition;
			if (this.joint != null)
			{
				this.joint.connectedBody = InterfaceControls.Instance.caseBoardCursorRigidbody;
			}
			if (this.rb != null)
			{
				this.rb.drag = InterfaceControls.Instance.movingLinearDrag;
			}
			if (this.evidence != null && this.evidence is EvidenceStickyNote)
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.stickyNotePickUp, null, 1f);
			}
			else
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.folderPickUp, null, 1f);
			}
			this.isDragging = true;
			base.StartCoroutine(this.IsDragging());
		}
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x001D12F7 File Offset: 0x001CF4F7
	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button != null)
		{
			return;
		}
		this.ForcePointerUp();
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x001D142F File Offset: 0x001CF62F
	public void ForcePointerUp()
	{
		Game.Log("Interface: Folder put down", 2);
		if (CasePanelController.Instance.customLinkSelectionMode)
		{
			return;
		}
		this.isDragging = false;
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x001D1450 File Offset: 0x001CF650
	private IEnumerator IsOver()
	{
		if (this.rect == null)
		{
			this.rect = base.gameObject.GetComponent<RectTransform>();
		}
		this.rect.SetAsLastSibling();
		bool selected = false;
		float quickMenuTimer = 0f;
		while ((this.isOver && ContextMenuController.activeMenu == null) || (ContextMenuController.activeMenu != null && ContextMenuController.activeMenu.flag == ContextMenuController.MenuFlag.pinnedSelected) || (PinnedItemController.activeQuickMenu != null && PinnedItemController.activeQuickMenu.parentPinned == this && PinnedItemController.activeQuickMenu.isOver))
		{
			if (!selected && ContextMenuController.activeMenu == null && !InterfaceController.Instance.boxSelectActive && InterfaceController.Instance.pinnedBeingDragged == null)
			{
				this.SetSelected(true, false);
				selected = true;
			}
			if (InterfaceController.Instance.selectedPinned.Count == 1 && PinnedItemController.activeQuickMenu == null && !MainMenuController.Instance.mainMenuActive && InterfaceController.Instance.pinnedBeingDragged == null && ContextMenuController.activeMenu == null && (!InputController.Instance.mouseInputMode || this.evidenceButton == null || !this.evidenceButton.isOver))
			{
				if (InputController.Instance.mouseInputMode)
				{
					quickMenuTimer += Time.deltaTime;
					if (quickMenuTimer >= 0.75f)
					{
						PinnedItemController.activeQuickMenu = Object.Instantiate<GameObject>(PrefabControls.Instance.quickMenu, PrefabControls.Instance.contextMenuContainer).GetComponent<PinnedQuickMenuController>();
						PinnedItemController.activeQuickMenu.Setup(this);
					}
				}
				else if (InputController.Instance.player.GetButtonDown("Secondary"))
				{
					PinnedItemController.activeQuickMenu = Object.Instantiate<GameObject>(PrefabControls.Instance.quickMenu, PrefabControls.Instance.contextMenuContainer).GetComponent<PinnedQuickMenuController>();
					PinnedItemController.activeQuickMenu.Setup(this);
				}
			}
			else if (PinnedItemController.activeQuickMenu != null && !InputController.Instance.mouseInputMode && InputController.Instance.player.GetButtonDown("Secondary"))
			{
				PinnedItemController.activeQuickMenu.Remove(false);
			}
			if (InterfaceController.Instance.boxSelectActive || InterfaceController.Instance.pinnedBeingDragged != null || ContextMenuController.activeMenu != null || MainMenuController.Instance.mainMenuActive)
			{
				if (PinnedItemController.activeQuickMenu != null)
				{
					PinnedItemController.activeQuickMenu.Remove(false);
				}
				quickMenuTimer = 0f;
			}
			yield return null;
		}
		if (PinnedItemController.activeQuickMenu != null)
		{
			PinnedItemController.activeQuickMenu.Remove(false);
		}
		bool deselectTimer = false;
		while (!deselectTimer)
		{
			deselectTimer = true;
			yield return null;
		}
		if (selected && !this.permSelected)
		{
			this.SetSelected(false, false);
		}
		yield break;
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x001D145F File Offset: 0x001CF65F
	private IEnumerator IsDragging()
	{
		if (this.evidenceButton != null)
		{
			this.evidenceButton.SetInteractable(false);
		}
		while (this.isDragging)
		{
			InterfaceController.Instance.pinnedBeingDragged = this;
			if (PinnedItemController.activeQuickMenu != null)
			{
				PinnedItemController.activeQuickMenu.Remove(false);
			}
			foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
			{
				if (pinnedItemController.rb != null)
				{
					Vector3 vector = this.rect.localPosition - pinnedItemController.prevLocalPos;
					pinnedItemController.rb.AddForce(vector * InterfaceControls.Instance.pinnedMovementIntertiaMultiplier);
				}
				pinnedItemController.prevLocalPos = this.rect.localPosition;
			}
			if (this.tooltip != null)
			{
				this.tooltip.ForceClose();
				this.tooltip.enabled = false;
			}
			if (this.pinPlaceActive)
			{
				if (!InputController.Instance.mouseInputMode && !InputController.Instance.player.GetButton("Select"))
				{
					this.ForcePointerUp();
				}
				else if (InputController.Instance.mouseInputMode && !Input.GetMouseButton(0))
				{
					this.ForcePointerUp();
				}
			}
			else if (!this.pinPlaceActive && !InputController.Instance.mouseInputMode)
			{
				Vector2 vector2;
				vector2..ctor(InputController.Instance.GetAxisRelative("MoveEvidenceAxisX"), InputController.Instance.GetAxisRelative("MoveEvidenceAxisY"));
				if (vector2.magnitude <= 0.15f)
				{
					this.ForcePointerUp();
				}
			}
			yield return null;
		}
		if (InterfaceController.Instance.pinnedBeingDragged == this)
		{
			InterfaceController.Instance.pinnedBeingDragged = null;
		}
		if (this.tooltip != null && !this.tooltip.enabled)
		{
			this.tooltip.enabled = true;
		}
		if (this.evidenceButton != null)
		{
			this.evidenceButton.SetInteractable(true);
		}
		if (this.evidence != null && this.evidence is EvidenceStickyNote)
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.stickyNotePutDown, null, 1f);
		}
		else
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.folderPutDown, null, 1f);
		}
		if (this.joint != null)
		{
			this.joint.connectedBody = null;
		}
		if (this.rb != null)
		{
			this.rb.drag = InterfaceControls.Instance.pinnedLinearDrag;
		}
		InterfaceController.Instance.RemoveMouseOverElement(this);
		this.pinButtonController.mainColour.enabled = true;
		this.pinButtonController.mainOverlay.enabled = true;
		this.pinButtonController.icon.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x001D146E File Offset: 0x001CF66E
	private IEnumerator Rescale(Vector3 size)
	{
		float len = 0f;
		while (len < 1f)
		{
			len += this.scalingSpeed * Time.deltaTime;
			this.rect.localScale = Vector3.Lerp(this.rect.localScale, size, Mathf.Clamp(len, 0f, 1f));
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x001D1484 File Offset: 0x001CF684
	public void SetSelected(bool val, bool permenantSelected)
	{
		if (this.isSelected != val)
		{
			this.isSelected = val;
			this.permSelected = permenantSelected;
			this.SetHovered(this.isSelected);
			if (this.isSelected)
			{
				base.StartCoroutine(this.Rescale(this.mouseOverScale));
				if (!InterfaceController.Instance.selectedPinned.Contains(this))
				{
					InterfaceController.Instance.selectedPinned.Add(this);
					InteractionController.Instance.UpdateInteractionText();
				}
				this.pinButtonController.mainOverlay.sprite = this.pinButtonController.pinnedOverlayMO;
				this.pinButtonController.mainMOOverlay.gameObject.SetActive(true);
			}
			else
			{
				if (PinnedItemController.activeQuickMenu != null)
				{
					PinnedItemController.activeQuickMenu.Remove(false);
				}
				base.StartCoroutine(this.Rescale(new Vector3(1f, 1f, 1f)));
				this.permSelected = false;
				if (InterfaceController.Instance.selectedPinned.Contains(this))
				{
					Game.Log("Interface: Remove " + base.name + " from selected pinned (deselect)", 2);
					InterfaceController.Instance.selectedPinned.Remove(this);
				}
				InteractionController.Instance.UpdateInteractionText();
				if (!this.isOver)
				{
					this.pinButtonController.mainOverlay.sprite = this.pinButtonController.pinnedOverlay;
					this.pinButtonController.mainMOOverlay.gameObject.SetActive(false);
					if (this.joint != null)
					{
						this.joint.connectedBody = null;
					}
					if (this.rb != null)
					{
						this.rb.drag = InterfaceControls.Instance.pinnedLinearDrag;
					}
				}
			}
			this.UpdateContextMenuOptions();
			Game.Log(string.Concat(new string[]
			{
				"Interface: Pin set selected: ",
				base.name,
				": ",
				val.ToString(),
				" permenant: ",
				this.permSelected.ToString()
			}), 2);
		}
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x001D1682 File Offset: 0x001CF882
	public void ChangeBaseColour(Color newBaseColour)
	{
		base.gameObject.GetComponent<Image>().color = newBaseColour;
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x001D1698 File Offset: 0x001CF898
	public void UpdateTooltipText()
	{
		if (this.evidence != null && this.tooltip != null)
		{
			this.tooltip.mainText = this.evidence.GetNameForDataKey(this.caseElement.dk);
			string text = string.Empty;
			"<color=#" + ColorUtility.ToHtmlStringRGB(InterfaceControls.Instance.defaultTextColour) + ">";
			text += this.evidence.GetNoteComposed(this.caseElement.dk, false);
			this.tooltip.detailText = text;
			if (this.evidence.preset.pinnedStyle == EvidencePreset.PinnedStyle.stickNote)
			{
				this.titleText.text = text;
			}
		}
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x001D1750 File Offset: 0x001CF950
	public void ToggleHideChildren()
	{
		if (this.hideConnections)
		{
			this.ShowConnections();
			return;
		}
		this.HideConnections();
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x001D1768 File Offset: 0x001CF968
	public void HideConnections()
	{
		this.hideConnections = true;
		Game.Log("Interface: Set hide linked evidence connections for " + base.name + ": " + this.hideConnections.ToString(), 2);
		foreach (Evidence.FactLink factLink in this.evidence.GetFactsForDataKey(this.caseElement.dk))
		{
			if (factLink.fact.isFound)
			{
				if (factLink.fact.fromEvidence.Contains(this.evidence))
				{
					using (List<Evidence>.Enumerator enumerator2 = factLink.fact.toEvidence.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Evidence evidence = enumerator2.Current;
							if (evidence.isFound)
							{
								Case @case = CasePanelController.Instance.activeCases.Find((Case item) => item.id == this.caseElement.caseID);
								if (@case != null)
								{
									CasePanelController.Instance.UnPinFromCasePanel(@case, evidence, factLink.fact.toDataKeys, false, null);
								}
							}
						}
						continue;
					}
				}
				if (factLink.fact.toEvidence.Contains(this.evidence))
				{
					foreach (Evidence evidence2 in factLink.fact.fromEvidence)
					{
						if (evidence2.isFound)
						{
							Case case2 = CasePanelController.Instance.activeCases.Find((Case item) => item.id == this.caseElement.caseID);
							if (case2 != null)
							{
								CasePanelController.Instance.UnPinFromCasePanel(case2, evidence2, factLink.fact.fromDataKeys, false, null);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x001D1968 File Offset: 0x001CFB68
	public void ShowConnections()
	{
		this.hideConnections = false;
		Game.Log("Interface: Set show linked evidence connections for " + base.name + ": " + this.hideConnections.ToString(), 2);
		foreach (Evidence.FactLink factLink in this.evidence.GetFactsForDataKey(this.caseElement.dk))
		{
			if (factLink.fact.isFound)
			{
				if (factLink.fact.fromEvidence.Contains(this.evidence))
				{
					using (List<Evidence>.Enumerator enumerator2 = factLink.fact.toEvidence.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Evidence evidence = enumerator2.Current;
							if (evidence != null && evidence.isFound)
							{
								Case @case = CasePanelController.Instance.activeCases.Find((Case item) => item.id == this.caseElement.caseID);
								if (@case != null)
								{
									CasePanelController.Instance.PinToCasePanel(@case, evidence, factLink.fact.toDataKeys, true, default(Vector2), false);
								}
							}
						}
						continue;
					}
				}
				if (factLink.fact.toEvidence.Contains(this.evidence))
				{
					foreach (Evidence evidence2 in factLink.fact.fromEvidence)
					{
						if (evidence2 != null && evidence2.isFound)
						{
							Case case2 = CasePanelController.Instance.activeCases.Find((Case item) => item.id == this.caseElement.caseID);
							if (case2 != null)
							{
								CasePanelController.Instance.PinToCasePanel(case2, evidence2, factLink.fact.fromDataKeys, true, default(Vector2), false);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x001D1B88 File Offset: 0x001CFD88
	public void ToggleMinimize()
	{
		if (this.caseElement.m)
		{
			this.Restore();
			return;
		}
		this.Minimize();
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x001D1BA4 File Offset: 0x001CFDA4
	public void Minimize()
	{
		this.caseElement.m = true;
		if (this.evidenceButton != null)
		{
			this.evidenceButton.gameObject.SetActive(false);
		}
		this.titleText.gameObject.SetActive(false);
		this.rect.sizeDelta = Vector2.zero;
		if (this.crossedOut != null)
		{
			this.crossedOut.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
		}
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x001D1C28 File Offset: 0x001CFE28
	public void Restore()
	{
		if (this.caseElement != null)
		{
			this.caseElement.m = false;
		}
		if (this.evidenceButton != null)
		{
			this.evidenceButton.gameObject.SetActive(true);
		}
		if (this.titleText != null)
		{
			this.titleText.gameObject.SetActive(true);
		}
		if (this.rect != null)
		{
			this.rect.sizeDelta = this.originalSize;
		}
		if (this.crossedOut != null)
		{
			CanvasRenderer component = this.crossedOut.gameObject.GetComponent<CanvasRenderer>();
			if (component != null)
			{
				component.SetAlpha(1f);
			}
		}
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x001D1CD9 File Offset: 0x001CFED9
	public void OpenEvidence()
	{
		if (this.evidenceButton != null)
		{
			this.evidenceButton.OnLeftClick();
		}
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x001D1CF4 File Offset: 0x001CFEF4
	public void Unpin()
	{
		Case @case = CasePanelController.Instance.activeCases.Find((Case item) => item.id == this.caseElement.caseID);
		if (@case != null)
		{
			CasePanelController.Instance.UnPinFromCasePanel(@case, this.evidence, this.caseElement.dk, true, this.caseElement);
		}
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x001D1D43 File Offset: 0x001CFF43
	public void Cancel()
	{
		if (this.contextMenu != null)
		{
			this.contextMenu.ForceClose();
		}
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x001D1D5E File Offset: 0x001CFF5E
	private void LateUpdate()
	{
		this.pinnedRect.rotation = Quaternion.identity;
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x001D1D70 File Offset: 0x001CFF70
	public void UpdateContextMenuOptions()
	{
		this.contextMenu.disabledItems.Clear();
		if (InterfaceController.Instance.selectedPinned.Count > 1)
		{
			this.contextMenu.disabledItems.Add("LocateOnMap");
			this.contextMenu.disabledItems.Add("PlotRoute");
			this.contextMenu.disabledItems.Add("CreateCustomString");
		}
		if (this.evidence == null || !(this.evidence is EvidenceLocation))
		{
			this.contextMenu.disabledItems.Add("LocateOnMap");
			this.contextMenu.disabledItems.Add("PlotRoute");
		}
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x001D1E1D File Offset: 0x001D001D
	public void CreateCustomString()
	{
		CasePanelController.Instance.CustomStringLinkSelection(this, false);
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x001D1E2B File Offset: 0x001D002B
	public void ForceCancelDrag()
	{
		Game.Log("Cancel pinned drag", 2);
		this.ForcePointerUp();
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x001D1E40 File Offset: 0x001D0040
	public void ToggleCrossedOut()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.caseElement.co = !pinnedItemController.caseElement.co;
			pinnedItemController.VisualUpdate();
			pinnedItemController.pinButtonController.VisualUpdate();
		}
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x001D1EBC File Offset: 0x001D00BC
	public void PlotRoute()
	{
		MapController.Instance.PlotPlayerRoute(this.evidence);
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x001D1ECE File Offset: 0x001D00CE
	public void LocateOnMap()
	{
		MapController.Instance.LocateEvidenceOnMap(this.evidence);
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x001D1EE0 File Offset: 0x001D00E0
	public void ToggleCollapse()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.ToggleMinimize();
		}
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x001D1F34 File Offset: 0x001D0134
	public void NewStickyNote()
	{
		Case @case = CasePanelController.Instance.activeCases.Find((Case item) => item.id == this.caseElement.caseID);
		if (@case != null)
		{
			InfoWindow infoWindow = CasePanelController.Instance.NewStickyNote();
			if (infoWindow != null)
			{
				List<Evidence> list = new List<Evidence>();
				foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
				{
					list.Add(pinnedItemController.evidence);
				}
				EvidenceCreator.Instance.CreateFact("CustomLink", null, infoWindow.passedEvidence, list, null, false, null, this.caseElement.dk, this.caseElement.dk, true);
				CasePanelController.Instance.PinToCasePanel(@case, infoWindow.passedEvidence, this.caseElement.dk, true, default(Vector2), false);
			}
		}
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x001D202C File Offset: 0x001D022C
	public void MinimizeAll()
	{
		InterfaceController.Instance.MinimizeAll();
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x001D2038 File Offset: 0x001D0238
	public void PinAllLinks()
	{
		using (List<PinnedItemController>.Enumerator enumerator = InterfaceController.Instance.selectedPinned.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PinnedItemController pic = enumerator.Current;
				if (pic.evidence != null)
				{
					List<Evidence.FactLink> list = new List<Evidence.FactLink>();
					foreach (Evidence.FactLink factLink in pic.evidence.GetFactsForDataKey(pic.caseElement.dk))
					{
						if (factLink.fact.isFound && !list.Contains(factLink))
						{
							bool flag = false;
							foreach (Evidence.FactLink factLink2 in list)
							{
								if (factLink2.fact == factLink.fact && factLink2.thisEvidence == factLink.thisEvidence)
								{
									bool flag2 = true;
									foreach (Evidence evidence in factLink2.destinationEvidence)
									{
										if (!factLink.destinationEvidence.Contains(evidence))
										{
											flag2 = false;
											break;
										}
									}
									if (flag2)
									{
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								list.Add(factLink);
							}
						}
					}
					Case @case = CasePanelController.Instance.activeCases.Find((Case item) => item.id == pic.caseElement.caseID);
					if (@case != null)
					{
						foreach (Evidence.FactLink factLink3 in list)
						{
							if (factLink3.thisEvidence != null && factLink3.thisEvidence != this.evidence)
							{
								CasePanelController.Instance.PinToCasePanel(@case, factLink3.thisEvidence, factLink3.thisKeys, true, default(Vector2), false);
							}
							if (factLink3.destinationEvidence != null)
							{
								foreach (Evidence evidence2 in factLink3.destinationEvidence)
								{
									if (evidence2 != this.evidence)
									{
										CasePanelController.Instance.PinToCasePanel(@case, evidence2, factLink3.destinationKeys, true, default(Vector2), false);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x001D234C File Offset: 0x001D054C
	public void UnpinAllLinks()
	{
		using (List<PinnedItemController>.Enumerator enumerator = InterfaceController.Instance.selectedPinned.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PinnedItemController pic = enumerator.Current;
				if (pic.evidence != null)
				{
					List<Evidence.FactLink> list = new List<Evidence.FactLink>();
					foreach (Evidence.FactLink factLink in pic.evidence.GetFactsForDataKey(pic.caseElement.dk))
					{
						if (factLink.fact.isFound && !list.Contains(factLink))
						{
							bool flag = false;
							foreach (Evidence.FactLink factLink2 in list)
							{
								if (factLink2.fact == factLink.fact && factLink2.thisEvidence == factLink.thisEvidence)
								{
									bool flag2 = true;
									foreach (Evidence evidence in factLink2.destinationEvidence)
									{
										if (!factLink.destinationEvidence.Contains(evidence))
										{
											flag2 = false;
											break;
										}
									}
									if (flag2)
									{
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								list.Add(factLink);
							}
						}
					}
					Case @case = CasePanelController.Instance.activeCases.Find((Case item) => item.id == pic.caseElement.caseID);
					if (@case != null)
					{
						foreach (Evidence.FactLink factLink3 in list)
						{
							if (factLink3.thisEvidence != null && factLink3.thisEvidence != this.evidence)
							{
								CasePanelController.Instance.UnPinFromCasePanel(@case, factLink3.thisEvidence, factLink3.thisKeys, false, null);
							}
							if (factLink3.destinationEvidence != null)
							{
								foreach (Evidence evidence2 in factLink3.destinationEvidence)
								{
									if (evidence2 != this.evidence)
									{
										CasePanelController.Instance.UnPinFromCasePanel(@case, evidence2, factLink3.destinationKeys, false, null);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x001D264C File Offset: 0x001D084C
	public void SetColourRed()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.caseElement.SetColour(InterfaceControls.EvidenceColours.red);
		}
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x001D26A8 File Offset: 0x001D08A8
	public void SetColourBlue()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.caseElement.SetColour(InterfaceControls.EvidenceColours.blue);
		}
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x001D2704 File Offset: 0x001D0904
	public void SetColourYellow()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.caseElement.SetColour(InterfaceControls.EvidenceColours.yellow);
		}
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x001D2760 File Offset: 0x001D0960
	public void SetColourGreen()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.caseElement.SetColour(InterfaceControls.EvidenceColours.green);
		}
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x001D27BC File Offset: 0x001D09BC
	public void SetColourPurple()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.caseElement.SetColour(InterfaceControls.EvidenceColours.purple);
		}
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x001D2818 File Offset: 0x001D0A18
	public void SetColourWhite()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.caseElement.SetColour(InterfaceControls.EvidenceColours.white);
		}
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x001D2874 File Offset: 0x001D0A74
	public void SetColourBlack()
	{
		foreach (PinnedItemController pinnedItemController in InterfaceController.Instance.selectedPinned)
		{
			pinnedItemController.caseElement.SetColour(InterfaceControls.EvidenceColours.black);
		}
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x001D28D0 File Offset: 0x001D0AD0
	public void UpdatePulsate()
	{
		bool toggle = false;
		if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			using (List<InfoWindow>.Enumerator enumerator = InterfaceController.Instance.activeWindows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					InfoWindow w = enumerator.Current;
					if (w.currentPinnedCaseElement == this.caseElement && list.Exists((RaycastResult item) => item.gameObject == w.background.gameObject))
					{
						toggle = true;
						break;
					}
				}
			}
		}
		this.juice.Pulsate(toggle, true);
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x001D29C4 File Offset: 0x001D0BC4
	[Button(null, 0)]
	public void DisplayEvidenceIdentifier()
	{
		if (this.evidence != null)
		{
			Game.Log(this.evidence.GetIdentifier(), 2);
		}
	}

	// Token: 0x04002CBF RID: 11455
	public Case.CaseElement caseElement;

	// Token: 0x04002CC0 RID: 11456
	public Evidence evidence;

	// Token: 0x04002CC1 RID: 11457
	public EvidenceButtonController evidenceButton;

	// Token: 0x04002CC2 RID: 11458
	public RectTransform newInfoIcon;

	// Token: 0x04002CC3 RID: 11459
	public Image background;

	// Token: 0x04002CC4 RID: 11460
	public TextMeshProUGUI titleText;

	// Token: 0x04002CC5 RID: 11461
	public RectTransform rect;

	// Token: 0x04002CC6 RID: 11462
	public RectTransform pinnedRect;

	// Token: 0x04002CC7 RID: 11463
	public PinnedPinButtonController pinButtonController;

	// Token: 0x04002CC8 RID: 11464
	public DragCasePanel dragController;

	// Token: 0x04002CC9 RID: 11465
	public RectTransform crossedOut;

	// Token: 0x04002CCA RID: 11466
	public Rigidbody2D rb;

	// Token: 0x04002CCB RID: 11467
	public HingeJoint2D joint;

	// Token: 0x04002CCC RID: 11468
	public JuiceController juice;

	// Token: 0x04002CCD RID: 11469
	public ContextMenuController contextMenu;

	// Token: 0x04002CCE RID: 11470
	public TooltipController tooltip;

	// Token: 0x04002CCF RID: 11471
	public List<StringController> connectedStrings = new List<StringController>();

	// Token: 0x04002CD0 RID: 11472
	public static PinnedQuickMenuController activeQuickMenu;

	// Token: 0x04002CD1 RID: 11473
	public bool hideConnections;

	// Token: 0x04002CD2 RID: 11474
	public Vector2 originalSize = Vector2.zero;

	// Token: 0x04002CD3 RID: 11475
	public bool isOver;

	// Token: 0x04002CD4 RID: 11476
	public bool isDragging;

	// Token: 0x04002CD5 RID: 11477
	public bool isSelected;

	// Token: 0x04002CD6 RID: 11478
	public bool permSelected;

	// Token: 0x04002CD7 RID: 11479
	public bool pinPlaceActive;

	// Token: 0x04002CD8 RID: 11480
	public float scalingSpeed = 8f;

	// Token: 0x04002CD9 RID: 11481
	public Vector3 mouseOverScale = new Vector3(1.05f, 1.05f, 1f);

	// Token: 0x04002CDA RID: 11482
	public Vector3 prevLocalPos;

	// Token: 0x04002CDB RID: 11483
	public List<string> debug = new List<string>();

	// Token: 0x04002CDC RID: 11484
	public static List<float> angleSteps;

	// Token: 0x04002CDD RID: 11485
	public bool minimized;

	// Token: 0x02000610 RID: 1552
	// (Invoke) Token: 0x06002273 RID: 8819
	public delegate void OnMove();
}
