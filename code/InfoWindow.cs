using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200056C RID: 1388
public class InfoWindow : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x14000034 RID: 52
	// (add) Token: 0x06001E3D RID: 7741 RVA: 0x001A78D4 File Offset: 0x001A5AD4
	// (remove) Token: 0x06001E3E RID: 7742 RVA: 0x001A790C File Offset: 0x001A5B0C
	public event InfoWindow.ResizedWindow OnResizedWindow;

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x06001E3F RID: 7743 RVA: 0x001A7944 File Offset: 0x001A5B44
	// (remove) Token: 0x06001E40 RID: 7744 RVA: 0x001A797C File Offset: 0x001A5B7C
	public event InfoWindow.WindowClosed OnWindowClosed;

	// Token: 0x14000036 RID: 54
	// (add) Token: 0x06001E41 RID: 7745 RVA: 0x001A79B4 File Offset: 0x001A5BB4
	// (remove) Token: 0x06001E42 RID: 7746 RVA: 0x001A79EC File Offset: 0x001A5BEC
	public event InfoWindow.WindowRefresh OnWindowRefresh;

	// Token: 0x14000037 RID: 55
	// (add) Token: 0x06001E43 RID: 7747 RVA: 0x001A7A24 File Offset: 0x001A5C24
	// (remove) Token: 0x06001E44 RID: 7748 RVA: 0x001A7A5C File Offset: 0x001A5C5C
	public event InfoWindow.WorldInteractionStateUpdate OnUpdateWorldInteractionState;

	// Token: 0x06001E45 RID: 7749 RVA: 0x001A7A94 File Offset: 0x001A5C94
	public void Setup(WindowStylePreset newPreset, Evidence newEv, List<Evidence.DataKey> newKeys, bool worldInteraction = false, Interactable newInteractable = null, Case newCase = null, Case.CaseElement newForcePinnedCaseElement = null, bool passedDialogSuccess = true)
	{
		if (newEv != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Interface: Spawn window for ",
				newEv.name,
				" (World interaction: ",
				worldInteraction.ToString(),
				", interactable: ",
				(newInteractable != null) ? newInteractable.ToString() : null,
				")"
			}), 2);
		}
		this.preset = newPreset;
		this.passedEvidence = newEv;
		this.passedKeys = newKeys;
		this.passedInteractable = newInteractable;
		this.passedCase = newCase;
		this.forcedPinnedCaseElement = newForcePinnedCaseElement;
		this.dialogSuccess = passedDialogSuccess;
		this.UpdateEvidenceKeys();
		if (this.rect == null)
		{
			this.rect = base.GetComponent<RectTransform>();
		}
		this.resizeZones = base.GetComponentsInChildren<ResizePanel>();
		this.horzScrollBar = this.scrollRect.horizontalScrollbar;
		this.vertScrollBar = this.scrollRect.verticalScrollbar;
		this.closable = this.preset.closable;
		this.pinnable = this.preset.pinnable;
		this.resizable = this.preset.resizable;
		this.defaultSize = this.preset.defaultSize;
		if (this.passedEvidence != null)
		{
			DDSSaveClasses.DDSTreeSave ddstreeSave = null;
			string text = this.passedEvidence.preset.ddsDocumentID;
			if (this.passedEvidence.overrideDDS != null && this.passedEvidence.overrideDDS.Length > 0)
			{
				text = this.passedEvidence.overrideDDS;
			}
			if (Toolbox.Instance.allDDSTrees.TryGetValue(text, ref ddstreeSave))
			{
				if (ddstreeSave.treeType == DDSSaveClasses.TreeType.vmail)
				{
					this.defaultSize = Toolbox.Instance.allDDSTrees["04f9b47f-e525-4aef-82b2-3cffe52cc061"].document.size + this.preset.DDSadditionalSize;
				}
				else
				{
					this.defaultSize = ddstreeSave.document.size + this.preset.DDSadditionalSize;
				}
			}
			this.passedEvidence.OnDataKeyChange += this.UpdateEvidenceKeys;
		}
		else
		{
			this.SetName(Strings.Get("ui.interface", this.preset.name, Strings.Casing.asIs, false, false, false, null));
		}
		this.iconLarge = this.preset.overrideIcon;
		this.SetWorldInteraction(worldInteraction);
		this.scrollRect.GetComponent<RectTransform>();
		if (this.passedEvidence != null)
		{
			if (this.item == null)
			{
				this.item = base.gameObject.AddComponent<ItemController>();
				DragPanel componentInChildren = base.gameObject.GetComponentInChildren<DragPanel>();
				componentInChildren.draggableComponent = true;
				componentInChildren.dragTag = this.passedEvidence.preset.name;
			}
			this.item.Setup(this);
			if (this.passedEvidence != null)
			{
				this.typeIcon.sprite = this.passedEvidence.GetIcon();
			}
		}
		foreach (WindowTabController windowTabController in this.tabs)
		{
			Object.Destroy(windowTabController.gameObject);
		}
		this.tabs.Clear();
		foreach (WindowTabPreset windowTabPreset in this.preset.tabs)
		{
			if (this.passedEvidence == null || ((this.passedEvidence.preset.enableFacts || windowTabPreset.contentType != WindowTabPreset.TabContentType.facts) && (this.passedEvidence.preset.enableSummary || windowTabPreset.contentType != WindowTabPreset.TabContentType.generated)))
			{
				this.LoadTab(windowTabPreset);
			}
		}
		if (this.preset.name == "ApartmentDecor")
		{
			WindowContentController windowContentController = this.contentPages.Find((WindowContentController item) => item.tabController.preset.contentType == PlayerApartmentController.Instance.rememberContent);
			if (windowContentController != null)
			{
				this.SetActiveContent(windowContentController);
			}
			else
			{
				this.SetActiveContent(this.contentPages[0]);
			}
		}
		else
		{
			this.SetActiveContent(this.contentPages[0]);
		}
		this.ResizeWindow(this.defaultSize);
		if (!this.resizable)
		{
			foreach (ResizePanel resizePanel in this.resizeZones)
			{
				resizePanel.gameObject.GetComponent<Button>().interactable = false;
				Object.Destroy(resizePanel);
			}
		}
		this.UpdateTabButtons();
		this.SetClosable(this.closable);
		if (!this.pinnable)
		{
			Object.Destroy(this.pinButton.gameObject);
		}
		if (worldInteraction)
		{
			this.rect.anchoredPosition = new Vector2(InterfaceControls.Instance.hudCanvasRect.rect.width * 0.7f - this.rect.sizeDelta.x * 0.5f, -InterfaceControls.Instance.hudCanvasRect.rect.height * 0.5f + this.rect.sizeDelta.y * 0.5f);
		}
		if (this.passedEvidence is EvidenceStickyNote)
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.stickyOpen, null, 1f);
		}
		else
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.folderOpen, null, 1f);
		}
		this.PinnedUpdateCheck();
		this.UpdateControllerNavigationEndOfFrame();
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x06001E46 RID: 7750 RVA: 0x001A7FFC File Offset: 0x001A61FC
	private void OnDestroy()
	{
		PopupMessageController.Instance.OnRightButton -= this.OnEditName;
		if (this.passedEvidence != null)
		{
			WindowTabController windowTabController = this.tabs.Find((WindowTabController item) => item.preset.contentType == WindowTabPreset.TabContentType.facts);
			if (windowTabController != null)
			{
				this.item.OnUpdateUnseenFacts -= windowTabController.SetNewItems;
			}
			this.passedEvidence.OnDataKeyChange -= this.UpdateEvidenceKeys;
		}
	}

	// Token: 0x06001E47 RID: 7751 RVA: 0x001A808C File Offset: 0x001A628C
	public void SetWorldInteraction(bool val)
	{
		if (this.passedEvidence != null && this.passedEvidence.preset.forceWorldInteraction)
		{
			val = true;
		}
		else if (this.preset.forceWorldInteraction)
		{
			val = true;
		}
		this.isWorldInteraction = val;
		this.interactionIconRect.gameObject.SetActive(this.isWorldInteraction);
		Game.Log("Interface: Set world interaction for " + base.name + ": " + this.isWorldInteraction.ToString(), 2);
		this.RefreshTakeButton();
		if (this.isWorldInteraction)
		{
			if (this.passedEvidence != null && this.passedEvidence.preset.useWindowFocusMode)
			{
				InterfaceController.Instance.ShowWindowFocus();
			}
			else if (this.preset.useWindowFocusMode)
			{
				InterfaceController.Instance.ShowWindowFocus();
			}
		}
		else
		{
			InterfaceController.Instance.RemoveWindowFocus();
		}
		if (this.OnUpdateWorldInteractionState != null)
		{
			this.OnUpdateWorldInteractionState();
		}
	}

	// Token: 0x06001E48 RID: 7752 RVA: 0x001A8178 File Offset: 0x001A6378
	public void RefreshTakeButton()
	{
		if (this.isWorldInteraction && this.passedInteractable != null && this.passedInteractable.inInventory != Player.Instance)
		{
			if (this.passedInteractable.preset.GetActions(0).Exists((InteractablePreset.InteractionAction item) => item.action == RoutineControls.Instance.takeFirstPersonItem))
			{
				this.takeItemButton.gameObject.SetActive(true);
				this.takeItemButton.icon.color = InterfaceControls.Instance.windowTakeItemIconDefaultColor;
				InteractablePreset.InteractionAction interactionAction = this.passedInteractable.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.action == RoutineControls.Instance.takeFirstPersonItem);
				if (interactionAction != null && interactionAction.actionIsIllegal && InteractionController.Instance.GetValidPlayerActionIllegal(this.passedInteractable, Player.Instance.currentNode, true, true))
				{
					this.takeItemButton.icon.color = InterfaceControls.Instance.interactionControlTextColourIllegal;
					return;
				}
				return;
			}
		}
		this.takeItemButton.gameObject.SetActive(false);
	}

	// Token: 0x06001E49 RID: 7753 RVA: 0x001A82A5 File Offset: 0x001A64A5
	public void CancelWorldInteractionButton()
	{
		if (this.isWorldInteraction)
		{
			this.SetWorldInteraction(false);
		}
	}

	// Token: 0x06001E4A RID: 7754 RVA: 0x001A82B8 File Offset: 0x001A64B8
	public void LoadTab(WindowTabPreset tabPreset)
	{
		Game.Log("Interface: Load tab " + tabPreset.name, 2);
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.tabButton, this.tabBar.transform);
		WindowTabController component = gameObject.GetComponent<WindowTabController>();
		component.tabButton = gameObject.GetComponent<Button>();
		component.preset = tabPreset;
		this.tabs.Add(component);
		if (tabPreset.contentPrefab == null)
		{
			Game.Log("Interface: Content does not exist for tab " + tabPreset.name, 2);
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(tabPreset.contentPrefab, this.scrollRect.viewport.transform);
		gameObject2.transform.SetAsFirstSibling();
		gameObject2.name = tabPreset.tabName;
		WindowContentController newWcc = gameObject2.GetComponent<WindowContentController>();
		newWcc.tabButton = gameObject.GetComponent<Button>();
		component.content = newWcc;
		newWcc.tabController = component;
		newWcc.window = this;
		component.content = newWcc;
		if (tabPreset.contentType == WindowTabPreset.TabContentType.facts)
		{
			this.item.factContent = newWcc;
			this.item.OnUpdateUnseenFacts += component.SetNewItems;
			this.item.UpdateFactsDisplay();
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.help)
		{
			gameObject2.gameObject.GetComponentInChildren<HelpController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.resolve)
		{
			gameObject2.gameObject.GetComponentInChildren<ResolveController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.caseOptions)
		{
			gameObject2.gameObject.GetComponentInChildren<ResolveOptionsController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.results)
		{
			gameObject2.gameObject.GetComponentInChildren<ResultsController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.decor)
		{
			gameObject2.gameObject.GetComponentInChildren<DecorController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.furnishings)
		{
			gameObject2.gameObject.GetComponentInChildren<FurnishingsController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.items)
		{
			gameObject2.gameObject.GetComponentInChildren<ApartmentItemsController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.colourPicker)
		{
			gameObject2.gameObject.GetComponentInChildren<ColourPickerController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.materialKey)
		{
			gameObject2.gameObject.GetComponentInChildren<MaterialKeyController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.history)
		{
			gameObject2.gameObject.GetComponentInChildren<HistoryController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.passcodes)
		{
			gameObject2.gameObject.GetComponentInChildren<PasscodesController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.phoneNumbers)
		{
			gameObject2.gameObject.GetComponentInChildren<PhoneNumbersController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.photoSelect)
		{
			gameObject2.gameObject.GetComponentInChildren<PhotoSelectController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.itemSelect)
		{
			gameObject2.gameObject.GetComponentInChildren<ItemSelectController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.shop)
		{
			gameObject2.gameObject.GetComponentInChildren<BuyInterfaceController>().Setup(newWcc);
		}
		else if (tabPreset.contentType == WindowTabPreset.TabContentType.objectives)
		{
			gameObject2.gameObject.GetComponentInChildren<ObjectivesContentController>().Setup(newWcc);
		}
		else if (tabPreset.contentType != WindowTabPreset.TabContentType.callLogsIncoming)
		{
			WindowTabPreset.TabContentType contentType = tabPreset.contentType;
		}
		if (tabPreset.scalableContent)
		{
			ZoomContent zoomContent = gameObject2.AddComponent<ZoomContent>();
			zoomContent.enableZoomWithMouseWheel = tabPreset.zoomWithMouseWheel;
			newWcc.zoomController = zoomContent;
			if (!tabPreset.zoomWithMouseWheel)
			{
				newWcc.SetAlwaysCentred(true);
			}
		}
		if (this.passedEvidence != null && tabPreset.contentType == WindowTabPreset.TabContentType.generated)
		{
			newWcc.LoadContent();
		}
		component.SetupButton();
		gameObject.GetComponent<Button>().onClick.AddListener(delegate()
		{
			this.SetActiveContent(newWcc);
		});
		this.contentPages.Add(newWcc);
		gameObject2.SetActive(false);
	}

	// Token: 0x06001E4B RID: 7755 RVA: 0x001A86F4 File Offset: 0x001A68F4
	public void OnResizeWindow()
	{
		this.UpdateTabButtons();
		foreach (WindowContentController windowContentController in this.contentPages)
		{
			windowContentController.centred = false;
			if (Mathf.Abs(windowContentController.rect.anchoredPosition.x) + Mathf.Abs(windowContentController.rect.anchoredPosition.y) < this.centringTollerance && windowContentController.zoomController != null && windowContentController.zoomController.zoom <= windowContentController.fitScale + 0.01f && windowContentController.zoomController.zoom >= windowContentController.fitScale - 0.01f)
			{
				windowContentController.centred = true;
			}
			windowContentController.UpdateFitScale();
			if (windowContentController.centred || windowContentController.alwaysCentred)
			{
				windowContentController.CentrePage();
			}
		}
		if (this.item != null)
		{
			Toolbox.Instance.UpdateButtonListPositions(this.item.spawnedChildEvButtons, 5f, 4f);
			this.item.PositionSpawnedFacts(10f, 6f);
		}
		ProgressBarController[] componentsInChildren = base.gameObject.GetComponentsInChildren<ProgressBarController>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].VisualUpdate();
		}
		if (this.OnResizedWindow != null)
		{
			this.OnResizedWindow();
		}
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x001A8860 File Offset: 0x001A6A60
	public void UpdateTabButtons()
	{
		float num = this.tabBar.GetComponent<RectTransform>().rect.height / (float)this.contentPages.Count;
		float num2 = 0f;
		float num3 = 16f;
		float num4 = 34f;
		for (int i = 0; i < this.contentPages.Count; i++)
		{
			RectTransform component = this.contentPages[i].tabButton.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(component.sizeDelta.x, num + num3);
			component.anchoredPosition = new Vector2(0f, num2);
			num2 -= num;
			RectTransform component2 = this.contentPages[i].tabController.text.gameObject.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(component.sizeDelta.y - num4, component2.sizeDelta.y);
			component2.anchoredPosition = new Vector2(component2.anchoredPosition.x, num4 * 0.5f);
		}
		foreach (WindowTabController windowTabController in this.tabs)
		{
			windowTabController.VisualUpdate();
		}
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x001A89BC File Offset: 0x001A6BBC
	public void SetActiveContent(WindowContentController wcc)
	{
		Game.Log("Interface: Set active content: " + wcc.name, 2);
		if (this.contentPages.Contains(wcc))
		{
			foreach (WindowContentController windowContentController in this.contentPages)
			{
				windowContentController.CentrePage();
				windowContentController.gameObject.SetActive(false);
				windowContentController.tabButton.transform.SetParent(this.tabBar.transform);
				if (wcc.tabController.pulsateController != null && wcc.tabController.newItems > 0)
				{
					wcc.tabController.pulsateController.enabled = true;
				}
			}
			if (wcc.tabController.preset.zoomWithMouseWheel)
			{
				this.scrollRect.scrollSensitivity = 0f;
			}
			else
			{
				this.scrollRect.scrollSensitivity = (float)GameplayControls.Instance.mouseWheelEvidenceScrollSensitivity;
			}
			wcc.gameObject.SetActive(true);
			wcc.tabButton.transform.SetParent(this.activeTabRect);
			if (wcc.tabController.pulsateController != null)
			{
				wcc.tabController.pulsateController.enabled = false;
			}
			wcc.tabButton.transform.SetAsLastSibling();
			this.activeContent = wcc;
			this.contentRect = wcc.gameObject.GetComponent<RectTransform>();
			this.scrollRect.content = this.contentRect;
			this.scrollRect.horizontal = wcc.tabController.preset.scrollBars;
			this.scrollRect.vertical = wcc.tabController.preset.scrollBars;
			if (!wcc.tabController.preset.scrollBars)
			{
				if (this.scrollRect.horizontalScrollbar != null)
				{
					this.scrollRect.horizontalScrollbar.gameObject.SetActive(false);
				}
				this.scrollRect.horizontalScrollbar = null;
			}
			else
			{
				this.scrollRect.horizontalScrollbar = this.horzScrollBar;
				this.scrollRect.horizontalScrollbar.gameObject.SetActive(true);
			}
			if (!wcc.tabController.preset.scrollBars)
			{
				if (this.scrollRect.verticalScrollbar != null)
				{
					this.scrollRect.verticalScrollbar.gameObject.SetActive(false);
				}
				this.scrollRect.verticalScrollbar = null;
			}
			else
			{
				this.scrollRect.verticalScrollbar = this.vertScrollBar;
				this.scrollRect.verticalScrollbar.gameObject.SetActive(true);
			}
			this.scrollRect.movementType = wcc.tabController.preset.scrollRestrcition;
			if (wcc.tabController.preset.contentType == WindowTabPreset.TabContentType.decor)
			{
				PlayerApartmentController.Instance.rememberContent = WindowTabPreset.TabContentType.decor;
			}
			else if (wcc.tabController.preset.contentType == WindowTabPreset.TabContentType.furnishings)
			{
				PlayerApartmentController.Instance.rememberContent = WindowTabPreset.TabContentType.furnishings;
			}
			else if (wcc.tabController.preset.contentType == WindowTabPreset.TabContentType.items)
			{
				PlayerApartmentController.Instance.rememberContent = WindowTabPreset.TabContentType.items;
			}
			this.OnResizeWindow();
			this.UpdateControllerNavigationEndOfFrame();
		}
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x001A8CE8 File Offset: 0x001A6EE8
	public void InstanceUpdateComplete()
	{
		if (this.item != null)
		{
			this.item.UpdateFactsDisplay();
			this.item.UpdateNameDisplay();
		}
		if (this.OnWindowRefresh != null)
		{
			this.OnWindowRefresh();
		}
	}

	// Token: 0x06001E4F RID: 7759 RVA: 0x001A8D24 File Offset: 0x001A6F24
	public void UpdateEvidenceKeys()
	{
		if (this.passedEvidence == null)
		{
			return;
		}
		this.evidenceKeys = this.passedEvidence.GetTiedKeys(this.passedKeys);
		if (this.evidenceKeys != null && this.passedKeys != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Interface: Update evidence keys for ",
				base.name,
				": ",
				this.evidenceKeys.Count.ToString(),
				" keys found using ",
				this.passedKeys.Count.ToString(),
				" passed keys."
			}), 2);
		}
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.sdk = new List<Evidence.DataKey>(this.evidenceKeys);
			if (this.currentPinnedCaseElement.pinnedController != null)
			{
				this.currentPinnedCaseElement.pinnedController.UpdateNewInfoIcon();
			}
		}
		this.iconLarge = this.passedEvidence.GetIcon();
	}

	// Token: 0x06001E50 RID: 7760 RVA: 0x001A8E1C File Offset: 0x001A701C
	public void SetName(string newName)
	{
		Game.Log("Interface: Set window name: " + newName, 2);
		base.name = newName.Trim();
		this.titleText.text = base.name;
		base.transform.name = base.name;
		RectTransform component = this.titleText.gameObject.GetComponent<RectTransform>();
		if (this.titleText.preferredWidth >= component.rect.width)
		{
			this.titleText.characterSpacing = -0.8f;
			this.titleText.wordSpacing = -0.8f;
			return;
		}
		this.titleText.characterSpacing = 0f;
		this.titleText.wordSpacing = 0f;
	}

	// Token: 0x06001E51 RID: 7761 RVA: 0x001A8ED8 File Offset: 0x001A70D8
	public void ResizeWindow(Vector2 sizeDelta)
	{
		if (this.rect == null)
		{
			this.rect = base.GetComponent<RectTransform>();
		}
		this.rect.sizeDelta = new Vector2(Mathf.Clamp(this.defaultSize.x, this.preset.minSize.x, this.preset.maxSize.x), Mathf.Clamp(this.defaultSize.y, this.preset.minSize.y, this.preset.maxSize.y));
		string[] array = new string[6];
		array[0] = "Interface: Resize window ";
		array[1] = base.name;
		array[2] = " default ";
		int num = 3;
		Vector2 vector = sizeDelta;
		array[num] = vector.ToString();
		array[4] = " = clamped: ";
		array[5] = this.rect.sizeDelta.ToString();
		Game.Log(string.Concat(array), 2);
		this.OnResizeWindow();
	}

	// Token: 0x06001E52 RID: 7762 RVA: 0x001A8FD8 File Offset: 0x001A71D8
	public void CloseWindow(bool animate = true)
	{
		if (this.forceDisableClose)
		{
			return;
		}
		string[] array = new string[8];
		array[0] = "Interface: CloseWindow: ";
		array[1] = base.name;
		array[2] = " animate: ";
		array[3] = animate.ToString();
		array[4] = " pinned: ";
		array[5] = this.pinned.ToString();
		array[6] = " caseElement: ";
		int num = 7;
		Case.CaseElement caseElement = this.currentPinnedCaseElement;
		array[num] = ((caseElement != null) ? caseElement.ToString() : null);
		Game.Log(string.Concat(array), 2);
		InterfaceController.Instance.activeWindows.Remove(this);
		bool flag = false;
		if (this.pinned && this.currentPinnedCaseElement != null)
		{
			flag = true;
			AudioController.Instance.Play2DSound(AudioControls.Instance.minimiseButton, null, 1f);
			if (this.passedEvidence is EvidenceStickyNote)
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.stickyClose, null, 1f);
				AudioController.Instance.Play2DSound(AudioControls.Instance.stickyNotePutDown, null, 1f);
			}
			else
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.folderClose, null, 1f);
				AudioController.Instance.Play2DSound(AudioControls.Instance.folderPutDown, null, 1f);
			}
			RectTransform component = this.windowCanvas.GetComponent<RectTransform>();
			this.currentPinnedCaseElement.resPos = component.position;
			this.currentPinnedCaseElement.resPiv = component.pivot;
			Game.Log("Interface: Close window " + base.name + " (minimize to corkboard)", 2);
			if (animate)
			{
				InterfaceController.Instance.MinimizeWindow(this);
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.folderClose, null, 1f);
		}
		if (this.isWorldInteraction && !PopupMessageController.Instance.active)
		{
			SessionData.Instance.ResumeGame();
		}
		if (this.OnWindowClosed != null)
		{
			this.OnWindowClosed();
		}
		if (CasePanelController.Instance.controllerMode && CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.windows && InterfaceController.Instance.activeWindows.Count <= 0)
		{
			CasePanelController.Instance.SetControllerMode(true, CasePanelController.ControllerSelectMode.caseBoard);
		}
		InteractionController.Instance.UpdateInteractionText();
		if (!flag && (this.currentPinnedCaseElement == null || !animate))
		{
			Game.Log("Interface: Close window " + base.name + " (destroy immediately)", 2);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001E53 RID: 7763 RVA: 0x001A9234 File Offset: 0x001A7434
	public void TogglePinned()
	{
		if (!this.pinnable)
		{
			return;
		}
		if (CasePanelController.Instance.activeCase == null)
		{
			return;
		}
		if (this.item == null)
		{
			return;
		}
		if (!this.pinned)
		{
			Vector2 zero = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(CasePanelController.Instance.pinnedContainer, Input.mousePosition, null, ref zero);
			zero.y -= 80f;
			CasePanelController.Instance.PinToCasePanel(CasePanelController.Instance.activeCase, this.passedEvidence, this.evidenceKeys, false, zero, false);
			return;
		}
		CasePanelController.Instance.UnPinFromCasePanel(CasePanelController.Instance.activeCase, this.passedEvidence, this.evidenceKeys, true, this.forcedPinnedCaseElement);
		AudioController.Instance.Play2DSound(AudioControls.Instance.unPin, null, 1f);
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x001A9308 File Offset: 0x001A7508
	public void PinnedUpdateCheck()
	{
		if (CasePanelController.Instance.activeCase != null && this.passedEvidence != null)
		{
			this.UpdateEvidenceKeys();
			List<Case.CaseElement> list = CasePanelController.Instance.activeCase.caseElements.FindAll((Case.CaseElement item) => this.passedEvidence != null && item.id == this.passedEvidence.evID);
			Case.CaseElement caseElement = null;
			foreach (Case.CaseElement caseElement2 in list)
			{
				if (!this.passedEvidence.preset.useDataKeys)
				{
					caseElement = caseElement2;
					break;
				}
				foreach (Evidence.DataKey dataKey in caseElement2.dk)
				{
					if (this.passedEvidence.preset.IsKeyUnique(dataKey) && this.evidenceKeys.Contains(dataKey))
					{
						caseElement = caseElement2;
						break;
					}
				}
				if (caseElement != null)
				{
					break;
				}
			}
			if (caseElement != null)
			{
				this.OnWindowPinnedChange(true, caseElement);
			}
			else if (this.forcedPinnedCaseElement != null && this.forcedPinnedCaseElement.pinnedController != null)
			{
				this.OnWindowPinnedChange(true, this.forcedPinnedCaseElement);
			}
			else
			{
				this.OnWindowPinnedChange(false, null);
			}
		}
		else if (this.pinned)
		{
			this.OnWindowPinnedChange(false, null);
		}
		this.UpdatePinColour();
	}

	// Token: 0x06001E55 RID: 7765 RVA: 0x001A9468 File Offset: 0x001A7668
	public void OnWindowPinnedChange(bool isPinned, Case.CaseElement newCaseElement)
	{
		this.pinned = isPinned;
		this.currentPinnedCaseElement = newCaseElement;
		Game.Log("Interface: Pinned change for " + base.name + ": " + this.pinned.ToString(), 2);
		if (!this.pinned)
		{
			this.closeButtonIcon.sprite = InterfaceControls.Instance.closeSprite;
		}
		else
		{
			this.closeButtonIcon.sprite = InterfaceControls.Instance.minimizeSprite;
		}
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.sdk = new List<Evidence.DataKey>(this.evidenceKeys);
			if (this.currentPinnedCaseElement.pinnedController != null)
			{
				this.currentPinnedCaseElement.pinnedController.UpdateNewInfoIcon();
			}
		}
		this.pinButton.VisualUpdate();
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x001A952C File Offset: 0x001A772C
	public void SetClosable(bool newClosble)
	{
		this.closable = newClosble;
		if (this.closable)
		{
			this.closeButton.SetInteractable(true);
			this.closeButton.gameObject.SetActive(true);
			return;
		}
		this.closeButton.SetInteractable(false);
		this.closeButton.gameObject.SetActive(false);
	}

	// Token: 0x06001E57 RID: 7767 RVA: 0x001A9584 File Offset: 0x001A7784
	public void SetAnchoredPosition(Vector2 newPos)
	{
		string text = "Interface: Set window anchored position: ";
		Vector2 vector = newPos;
		Game.Log(text + vector.ToString(), 2);
		if (this.rect == null)
		{
			this.rect = base.gameObject.GetComponent<RectTransform>();
		}
		this.debugSetAnchoredPosition = newPos;
		Vector2 size = this.rect.rect.size;
		Vector2 vector2 = Vector2.zero - this.rect.pivot;
		Vector3 vector3;
		vector3..ctor(vector2.x * size.x, vector2.y * size.y);
		Vector2 vector4 = Vector2.one - this.rect.pivot;
		Vector3 vector5;
		vector5..ctor(vector4.x * size.x, vector4.y * size.y);
		float num = Mathf.Clamp(newPos.x, -vector3.x, InterfaceControls.Instance.hudCanvasRect.rect.width - vector5.x);
		float num2 = Mathf.Clamp(newPos.y, -InterfaceControls.Instance.hudCanvasRect.rect.height - vector3.y, -vector5.y);
		this.rect.anchoredPosition = new Vector2(num, num2);
		num = Mathf.Clamp(this.rect.localPosition.x, InterfaceControls.Instance.hudCanvasRect.rect.width * -0.5f, InterfaceControls.Instance.hudCanvasRect.rect.width * 0.5f);
		num2 = Mathf.Clamp(this.rect.localPosition.y, InterfaceControls.Instance.hudCanvasRect.rect.height * -0.5f, InterfaceControls.Instance.hudCanvasRect.rect.height * 0.5f);
		this.rect.localPosition = new Vector3(num, num2, 0f);
		Game.Log("Interface: Set window local position: " + this.rect.localPosition.ToString(), 2);
	}

	// Token: 0x06001E58 RID: 7768 RVA: 0x001A97C0 File Offset: 0x001A79C0
	public void SetPivot(Vector2 p)
	{
		if (this.rect == null)
		{
			this.rect = base.gameObject.GetComponent<RectTransform>();
		}
		Vector2 size = this.rect.rect.size;
		Vector2 vector = this.rect.pivot - p;
		Vector3 vector2;
		vector2..ctor(vector.x * size.x, vector.y * size.y);
		this.rect.pivot = p;
		this.SetAnchoredPosition(new Vector2(this.rect.anchoredPosition.x - vector2.x, this.rect.anchoredPosition.y - vector2.y));
	}

	// Token: 0x06001E59 RID: 7769 RVA: 0x001A9878 File Offset: 0x001A7A78
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.isOver = true;
		if (this.currentPinnedCaseElement != null && this.currentPinnedCaseElement.pinnedController != null)
		{
			this.currentPinnedCaseElement.pinnedController.UpdatePulsate();
		}
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x001A98AC File Offset: 0x001A7AAC
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.isOver = false;
		if (this.currentPinnedCaseElement != null && this.currentPinnedCaseElement.pinnedController != null)
		{
			this.currentPinnedCaseElement.pinnedController.UpdatePulsate();
		}
	}

	// Token: 0x06001E5B RID: 7771 RVA: 0x001A9900 File Offset: 0x001A7B00
	public void UpdatePinColour()
	{
		if (this.passedEvidence == null)
		{
			return;
		}
		if (this.currentPinnedCaseElement != null)
		{
			this.evColour = this.currentPinnedCaseElement.color;
		}
		this.pinColourActual = InterfaceController.Instance.GetEvidenceColour(this.evColour);
		this.pinColour.color = this.pinColourActual;
		this.pinColourPressed.color = this.pinColourActual;
	}

	// Token: 0x06001E5C RID: 7772 RVA: 0x001A9968 File Offset: 0x001A7B68
	public void Rename()
	{
		if (this.passedEvidence != null && this.passedEvidence.preset.allowCustomNames)
		{
			PopupMessageController.Instance.PopupMessage("RenameEvidence", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, this.titleText.text, false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnRightButton += this.OnEditName;
			return;
		}
		PopupMessageController.Instance.PopupMessage("RenameDisabled", true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x001A9A30 File Offset: 0x001A7C30
	public void OnEditName()
	{
		PopupMessageController.Instance.OnRightButton -= this.OnEditName;
		if (this.passedEvidence != null && this.passedEvidence.preset.allowCustomNames)
		{
			this.passedEvidence.AddOrSetCustomName(this.passedKeys, PopupMessageController.Instance.inputField.text);
		}
		this.SetName(PopupMessageController.Instance.inputField.text);
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x001A9AA2 File Offset: 0x001A7CA2
	public void SetColourRed()
	{
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.SetColour(InterfaceControls.EvidenceColours.red);
			return;
		}
		this.evColour = InterfaceControls.EvidenceColours.red;
		this.UpdatePinColour();
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x001A9AC6 File Offset: 0x001A7CC6
	public void SetColourBlue()
	{
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.SetColour(InterfaceControls.EvidenceColours.blue);
			return;
		}
		this.evColour = InterfaceControls.EvidenceColours.blue;
		this.UpdatePinColour();
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x001A9AEA File Offset: 0x001A7CEA
	public void SetColourYellow()
	{
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.SetColour(InterfaceControls.EvidenceColours.yellow);
			return;
		}
		this.evColour = InterfaceControls.EvidenceColours.yellow;
		this.UpdatePinColour();
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x001A9B0E File Offset: 0x001A7D0E
	public void SetColourGreen()
	{
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.SetColour(InterfaceControls.EvidenceColours.green);
			return;
		}
		this.evColour = InterfaceControls.EvidenceColours.green;
		this.UpdatePinColour();
	}

	// Token: 0x06001E62 RID: 7778 RVA: 0x001A9B32 File Offset: 0x001A7D32
	public void SetColourPurple()
	{
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.SetColour(InterfaceControls.EvidenceColours.purple);
			return;
		}
		this.evColour = InterfaceControls.EvidenceColours.purple;
		this.UpdatePinColour();
	}

	// Token: 0x06001E63 RID: 7779 RVA: 0x001A9B56 File Offset: 0x001A7D56
	public void SetColourWhite()
	{
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.SetColour(InterfaceControls.EvidenceColours.white);
			return;
		}
		this.evColour = InterfaceControls.EvidenceColours.white;
		this.UpdatePinColour();
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x001A9B7A File Offset: 0x001A7D7A
	public void SetColourBlack()
	{
		if (this.currentPinnedCaseElement != null)
		{
			this.currentPinnedCaseElement.SetColour(InterfaceControls.EvidenceColours.black);
			return;
		}
		this.evColour = InterfaceControls.EvidenceColours.black;
		this.UpdatePinColour();
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x001A9BA0 File Offset: 0x001A7DA0
	public void SetSelected(bool val)
	{
		if (this.selected != val)
		{
			this.selected = val;
			this.controllerScrollView.controlEnabled = this.selected;
			if (this.selected && CasePanelController.Instance.controllerMode)
			{
				if (InterfaceController.Instance.selectedElement != null)
				{
					InterfaceController.Instance.selectedElement.OnDeselect();
				}
				this.pinButton.OnSelect();
			}
			this.UpdateControllerSelected();
		}
	}

	// Token: 0x06001E66 RID: 7782 RVA: 0x001A9C14 File Offset: 0x001A7E14
	public void UpdateControllerSelected()
	{
		if (CasePanelController.Instance.controllerMode && CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.windows)
		{
			this.controllerSelect.gameObject.SetActive(this.selected);
			return;
		}
		this.controllerSelect.gameObject.SetActive(false);
	}

	// Token: 0x06001E67 RID: 7783 RVA: 0x001A9C62 File Offset: 0x001A7E62
	public void UpdateControllerNavigationEndOfFrame()
	{
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		if (!this.updateNav)
		{
			this.updateNav = true;
			base.StartCoroutine(this.UpdateControllerNavigation());
		}
	}

	// Token: 0x06001E68 RID: 7784 RVA: 0x001A9C90 File Offset: 0x001A7E90
	public void OnClearTextButton()
	{
		if (this.activeContent != null)
		{
			this.activeContent.clearTextMode = !this.activeContent.clearTextMode;
			foreach (KeyValuePair<DDSSaveClasses.DDSMessageSettings, TextMeshProUGUI> keyValuePair in this.activeContent.spawnedText)
			{
				if (keyValuePair.Key.isHandwriting)
				{
					if (this.activeContent.clearTextMode)
					{
						keyValuePair.Value.font = DDSControls.Instance.clearModeFont;
					}
					else if (this.activeContent.window.passedEvidence.writer != null && this.activeContent.window.passedEvidence.writer.handwriting != null)
					{
						keyValuePair.Value.font = this.activeContent.window.passedEvidence.writer.handwriting.fontAsset;
					}
					else
					{
						keyValuePair.Value.font = DDSControls.Instance.defaultHandwritingFont;
					}
					keyValuePair.Value.ForceMeshUpdate(false, false);
					this.activeContent.TextOverflowCheck();
				}
			}
		}
	}

	// Token: 0x06001E69 RID: 7785 RVA: 0x001A9DE8 File Offset: 0x001A7FE8
	public void OnTakeItemButton()
	{
		if (this.isWorldInteraction && this.passedInteractable != null)
		{
			PopupMessageController.Instance.PopupMessage("Take Item", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnRightButton += this.OnTakeConfirm;
			PopupMessageController.Instance.OnLeftButton += this.OnTakeCancel;
		}
	}

	// Token: 0x06001E6A RID: 7786 RVA: 0x001A9E74 File Offset: 0x001A8074
	public void OnTakeConfirm()
	{
		PopupMessageController.Instance.OnRightButton -= this.OnTakeConfirm;
		PopupMessageController.Instance.OnLeftButton -= this.OnTakeCancel;
		if (this.passedInteractable != null && this.passedInteractable.inInventory != Player.Instance)
		{
			InteractablePreset.InteractionAction interactionAction = this.passedInteractable.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.action == RoutineControls.Instance.takeFirstPersonItem);
			if (interactionAction != null)
			{
				if (this.passedInteractable.inInventory != null && this.passedInteractable.inInventory != Player.Instance && Toolbox.Instance.Rand(0f, 1f, false) <= GameplayControls.Instance.stealTriggerChance)
				{
					this.passedInteractable.inInventory.ai.SetPersue(Player.Instance, true, 1, true, CitizenControls.Instance.punchedResponseRange);
				}
				this.passedInteractable.OnInteraction(interactionAction, Player.Instance, true, 0f);
			}
		}
		this.RefreshTakeButton();
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x001A9F9D File Offset: 0x001A819D
	public void OnTakeCancel()
	{
		PopupMessageController.Instance.OnRightButton -= this.OnTakeConfirm;
		PopupMessageController.Instance.OnLeftButton -= this.OnTakeCancel;
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x001A9FCB File Offset: 0x001A81CB
	private IEnumerator UpdateControllerNavigation()
	{
		int waitedFrame = 0;
		while (waitedFrame <= 1)
		{
			int num = waitedFrame;
			waitedFrame = num + 1;
			yield return null;
		}
		this.ExecuteUpdateControllerNavigation();
		this.updateNav = false;
		yield break;
	}

	// Token: 0x06001E6D RID: 7789 RVA: 0x001A9FDC File Offset: 0x001A81DC
	[Button(null, 0)]
	public void ExecuteUpdateControllerNavigation()
	{
		if (this.activeContent == null)
		{
			return;
		}
		Game.Log("Interface: Updating controller navigation for window " + base.name, 2);
		for (int i = 0; i < this.tabs.Count; i++)
		{
			this.tabs[i].tabButton.gameObject.GetComponent<ButtonController>().RefreshAutomaticNavigation();
		}
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x001AA044 File Offset: 0x001A8244
	[Button(null, 0)]
	public void ExecuteKeyMerge()
	{
		if (this.passedEvidence != null)
		{
			this.passedEvidence.MergeDataKeys(this.debugKeyOne, this.debugKeyTwo);
		}
	}

	// Token: 0x06001E6F RID: 7791 RVA: 0x001AA068 File Offset: 0x001A8268
	[Button(null, 0)]
	public void SpawnFingerprintOwner()
	{
		if (this.passedEvidence != null)
		{
			InterfaceController.Instance.SpawnWindow(this.passedEvidence.writer.evidenceEntry, Evidence.DataKey.fingerprints, null, "", false, true, default(Vector2), null, null, null, true);
		}
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x001AA0AE File Offset: 0x001A82AE
	[Button(null, 0)]
	public void RestoreAnchoredPosition()
	{
		this.SetAnchoredPosition(this.debugSetAnchoredPosition);
	}

	// Token: 0x0400281D RID: 10269
	[Header("Presets")]
	public WindowStylePreset preset;

	// Token: 0x0400281E RID: 10270
	[Header("References")]
	public Canvas windowCanvas;

	// Token: 0x0400281F RID: 10271
	public CanvasGroup windowCanvasGroup;

	// Token: 0x04002820 RID: 10272
	public Canvas contentConvas;

	// Token: 0x04002821 RID: 10273
	public CanvasGroup contentCanvasGroup;

	// Token: 0x04002822 RID: 10274
	public RectTransform background;

	// Token: 0x04002823 RID: 10275
	public TextMeshProUGUI titleText;

	// Token: 0x04002824 RID: 10276
	public RectTransform rect;

	// Token: 0x04002825 RID: 10277
	private ResizePanel[] resizeZones;

	// Token: 0x04002826 RID: 10278
	public CustomScrollRect scrollRect;

	// Token: 0x04002827 RID: 10279
	public GameObject tabBar;

	// Token: 0x04002828 RID: 10280
	public ButtonController closeButton;

	// Token: 0x04002829 RID: 10281
	public PinFolderButtonController pinButton;

	// Token: 0x0400282A RID: 10282
	public ItemController item;

	// Token: 0x0400282B RID: 10283
	public WindowContentController activeContent;

	// Token: 0x0400282C RID: 10284
	public RectTransform contentRect;

	// Token: 0x0400282D RID: 10285
	private Scrollbar horzScrollBar;

	// Token: 0x0400282E RID: 10286
	private Scrollbar vertScrollBar;

	// Token: 0x0400282F RID: 10287
	public RectTransform activeTabRect;

	// Token: 0x04002830 RID: 10288
	public RectTransform pageRect;

	// Token: 0x04002831 RID: 10289
	public Image typeIcon;

	// Token: 0x04002832 RID: 10290
	public Image closeButtonIcon;

	// Token: 0x04002833 RID: 10291
	public RectTransform dragZone;

	// Token: 0x04002834 RID: 10292
	public RectTransform controllerSelect;

	// Token: 0x04002835 RID: 10293
	public JuiceController controllerSelectJuice;

	// Token: 0x04002836 RID: 10294
	public ControllerViewRectScroll controllerScrollView;

	// Token: 0x04002837 RID: 10295
	public RectTransform interactionIconRect;

	// Token: 0x04002838 RID: 10296
	public ButtonController clearTextButton;

	// Token: 0x04002839 RID: 10297
	public ButtonController takeItemButton;

	// Token: 0x0400283A RID: 10298
	[Header("Parameters")]
	public bool closable = true;

	// Token: 0x0400283B RID: 10299
	public bool pinnable = true;

	// Token: 0x0400283C RID: 10300
	public bool pinned;

	// Token: 0x0400283D RID: 10301
	public bool selected;

	// Token: 0x0400283E RID: 10302
	[NonSerialized]
	public Case.CaseElement currentPinnedCaseElement;

	// Token: 0x0400283F RID: 10303
	[NonSerialized]
	public Case.CaseElement forcedPinnedCaseElement;

	// Token: 0x04002840 RID: 10304
	public bool isOver;

	// Token: 0x04002841 RID: 10305
	public bool isWorldInteraction;

	// Token: 0x04002842 RID: 10306
	private bool updateNav;

	// Token: 0x04002843 RID: 10307
	public bool forceDisablePin;

	// Token: 0x04002844 RID: 10308
	public bool forceDisableClose;

	// Token: 0x04002845 RID: 10309
	public bool dialogSuccess;

	// Token: 0x04002846 RID: 10310
	[Header("Graphics")]
	public Sprite iconLarge;

	// Token: 0x04002847 RID: 10311
	public InterfaceControls.EvidenceColours evColour;

	// Token: 0x04002848 RID: 10312
	public Image pinOverlay;

	// Token: 0x04002849 RID: 10313
	public Image pinColour;

	// Token: 0x0400284A RID: 10314
	public Image pinColourPressed;

	// Token: 0x0400284B RID: 10315
	public Color pinColourActual;

	// Token: 0x0400284C RID: 10316
	public Color baseColour = Color.white;

	// Token: 0x0400284D RID: 10317
	public Color flashColour = Color.cyan;

	// Token: 0x0400284E RID: 10318
	public Color borderColour = Color.white;

	// Token: 0x0400284F RID: 10319
	[Header("Resizing")]
	public bool resizable = true;

	// Token: 0x04002850 RID: 10320
	public Vector2 defaultSize = new Vector2(514f, 658f);

	// Token: 0x04002851 RID: 10321
	public float centringTollerance = 16f;

	// Token: 0x04002852 RID: 10322
	[Header("Content")]
	public Evidence passedEvidence;

	// Token: 0x04002853 RID: 10323
	public List<Evidence.DataKey> passedKeys = new List<Evidence.DataKey>();

	// Token: 0x04002854 RID: 10324
	public List<Evidence.DataKey> evidenceKeys = new List<Evidence.DataKey>();

	// Token: 0x04002855 RID: 10325
	public List<WindowContentController> contentPages = new List<WindowContentController>();

	// Token: 0x04002856 RID: 10326
	public List<WindowTabController> tabs = new List<WindowTabController>();

	// Token: 0x04002857 RID: 10327
	[NonSerialized]
	public Interactable passedInteractable;

	// Token: 0x04002858 RID: 10328
	[NonSerialized]
	public Case passedCase;

	// Token: 0x04002859 RID: 10329
	[Header("Debug")]
	public Evidence.DataKey debugKeyOne;

	// Token: 0x0400285A RID: 10330
	public Evidence.DataKey debugKeyTwo;

	// Token: 0x0400285B RID: 10331
	public Vector2 debugSetAnchoredPosition;

	// Token: 0x0200056D RID: 1389
	// (Invoke) Token: 0x06001E74 RID: 7796
	public delegate void ResizedWindow();

	// Token: 0x0200056E RID: 1390
	// (Invoke) Token: 0x06001E78 RID: 7800
	public delegate void WindowClosed();

	// Token: 0x0200056F RID: 1391
	// (Invoke) Token: 0x06001E7C RID: 7804
	public delegate void WindowRefresh();

	// Token: 0x02000570 RID: 1392
	// (Invoke) Token: 0x06001E80 RID: 7808
	public delegate void WorldInteractionStateUpdate();
}
