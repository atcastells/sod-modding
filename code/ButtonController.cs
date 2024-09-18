using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200054C RID: 1356
public class ButtonController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	// Token: 0x14000031 RID: 49
	// (add) Token: 0x06001D81 RID: 7553 RVA: 0x001A0E4C File Offset: 0x0019F04C
	// (remove) Token: 0x06001D82 RID: 7554 RVA: 0x001A0E84 File Offset: 0x0019F084
	public event ButtonController.Press OnPress;

	// Token: 0x14000032 RID: 50
	// (add) Token: 0x06001D83 RID: 7555 RVA: 0x001A0EBC File Offset: 0x0019F0BC
	// (remove) Token: 0x06001D84 RID: 7556 RVA: 0x001A0EF4 File Offset: 0x0019F0F4
	public event ButtonController.HoverChange OnHoverChange;

	// Token: 0x06001D85 RID: 7557 RVA: 0x001A0F2C File Offset: 0x0019F12C
	[Button("Setup References", 0)]
	public virtual void SetupReferences()
	{
		if (this.setupReferences)
		{
			return;
		}
		if (this.rect == null)
		{
			this.rect = base.gameObject.GetComponent<RectTransform>();
		}
		if (this.button == null)
		{
			this.button = base.gameObject.GetComponent<Button>();
		}
		if (this.background == null)
		{
			this.background = base.gameObject.GetComponent<Image>();
		}
		if (this.rend == null)
		{
			this.rend = base.gameObject.GetComponent<CanvasRenderer>();
		}
		if (this.tooltip == null)
		{
			this.tooltip = base.gameObject.GetComponent<TooltipController>();
		}
		if (this.juice == null)
		{
			this.juice = base.gameObject.GetComponent<JuiceController>();
		}
		if (Application.isPlaying)
		{
			if (this.parentWindow == null)
			{
				this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
			}
			if (this.parentWindow != null)
			{
				this.windowOf = this.parentWindow.passedEvidence;
			}
			if (this.windowOf != null)
			{
				WindowContentController componentInParent = base.gameObject.GetComponentInParent<WindowContentController>();
				if (componentInParent != null)
				{
					this.tabOf = componentInParent.tabController;
				}
			}
			if (this.tooltip != null)
			{
				this.tooltip.OnBeforeTooltipSpawn += this.UpdateTooltipText;
			}
			if (this.button != null)
			{
				int persistentEventCount = this.button.onClick.GetPersistentEventCount();
				for (int i = 0; i < persistentEventCount; i++)
				{
					this.button.onClick.SetPersistentListenerState(i, 0);
				}
			}
			this.UpdateButtonText();
			if (this.refreshControllerNavigationOnSetup && base.isActiveAndEnabled && !InputController.Instance.mouseInputMode)
			{
				base.StartCoroutine(this.RefreshNavEndOfFrame());
			}
		}
		this.setupReferences = true;
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x001A1106 File Offset: 0x0019F306
	private void Start()
	{
		if (!this.setupReferences)
		{
			this.SetupReferences();
		}
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x001A1116 File Offset: 0x0019F316
	public virtual void VisualUpdate()
	{
		this.UpdateButtonText();
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x001A111E File Offset: 0x0019F31E
	public virtual void UpdateButtonText()
	{
		if (this.useAutomaticText && this.text != null)
		{
			this.text.text = Strings.Get(this.textDictionary, this.textReference, this.casing, false, false, false, null);
		}
	}

	// Token: 0x06001D89 RID: 7561 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void UpdateTooltipText()
	{
	}

	// Token: 0x06001D8A RID: 7562 RVA: 0x001A115C File Offset: 0x0019F35C
	public virtual void SetInteractable(bool val)
	{
		if (this.interactable != val)
		{
			this.interactable = val;
			if (this.button != null)
			{
				this.button.interactable = this.interactable;
			}
			if (!this.interactable)
			{
				if (this.glowOnHighlight && this.juice != null)
				{
					this.juice.Pulsate(false, false);
					this.background.color = this.baseColour;
				}
				if (InterfaceController.Instance.selectedElement == this && !InputController.Instance.mouseInputMode)
				{
					this.OnDeselect();
				}
			}
			if (this.text != null)
			{
				Color color = this.text.color;
				if (this.interactable)
				{
					color.a = this.interactableTextAlpha;
				}
				else
				{
					color.a = this.uninteractableTextAlpha;
				}
				this.text.color = color;
			}
			if (this.tooltip != null)
			{
				this.tooltip.ForceClose();
			}
			this.UpdateAdditionalHighlight();
		}
	}

	// Token: 0x06001D8B RID: 7563 RVA: 0x001A1264 File Offset: 0x0019F464
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.button == null)
		{
			this.button = base.gameObject.GetComponent<Button>();
		}
		if (this.button != null && !this.button.interactable)
		{
			return;
		}
		if (eventData.button == null)
		{
			Game.Log("Menu: Button OnPointerClick: " + base.gameObject.name, 2);
			if (this.button != null)
			{
				int persistentEventCount = this.button.onClick.GetPersistentEventCount();
				for (int i = 0; i < persistentEventCount; i++)
				{
					this.button.onClick.SetPersistentListenerState(i, 2);
				}
				this.button.onClick.Invoke();
				for (int j = 0; j < persistentEventCount; j++)
				{
					this.button.onClick.SetPersistentListenerState(j, 0);
				}
			}
			if (Time.time - this.lastLeftClick <= InterfaceControls.Instance.doubleClickDelay)
			{
				this.OnLeftClick();
				this.OnLeftDoubleClick();
			}
			else
			{
				this.OnLeftClick();
			}
			if (this.nudgeOnClick && this.juice != null)
			{
				this.juice.Nudge();
			}
			if (this.useGenericAudioSounds)
			{
				if (this.buttonType == ButtonController.ButtonAudioType.normal)
				{
					AudioController.Instance.Play2DSound(AudioControls.Instance.mainButton, null, 1f);
				}
				else if (this.buttonType == ButtonController.ButtonAudioType.forward)
				{
					AudioController.Instance.Play2DSound(AudioControls.Instance.mainButtonForward, null, 1f);
				}
				else if (this.buttonType == ButtonController.ButtonAudioType.back)
				{
					AudioController.Instance.Play2DSound(AudioControls.Instance.mainButtonBack, null, 1f);
				}
				else if (this.buttonType == ButtonController.ButtonAudioType.tickBox)
				{
					AudioController.Instance.Play2DSound(AudioControls.Instance.tickbox, null, 1f);
				}
			}
			else
			{
				if (this.clickPrimary != null)
				{
					AudioController.Instance.Play2DSound(this.clickPrimary, null, 1f);
				}
				if (this.clickSecondary != null)
				{
					AudioController.Instance.Play2DSound(this.clickSecondary, null, 1f);
				}
			}
			this.lastLeftClick = Time.time;
			return;
		}
		if (eventData.button == 1)
		{
			if (Time.time - this.lastRightClick <= InterfaceControls.Instance.doubleClickDelay)
			{
				this.OnRightClick();
				this.OnRightDoubleClick();
			}
			else
			{
				this.OnRightClick();
			}
			if (this.rightClick != null)
			{
				AudioController.Instance.Play2DSound(this.rightClick, null, 1f);
			}
			this.lastRightClick = Time.time;
		}
	}

	// Token: 0x06001D8C RID: 7564 RVA: 0x001A14ED File Offset: 0x0019F6ED
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (!this.useGenericAudioSounds && this.buttonDown != null)
		{
			AudioController.Instance.Play2DSound(this.buttonDown, null, 1f);
		}
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnPointerUp(PointerEventData eventData)
	{
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x001A151C File Offset: 0x0019F71C
	public virtual void OnLeftClick()
	{
		if (this.OnPress != null)
		{
			this.OnPress(this);
		}
		if (this.refreshControllerNavigationOnPress && !InputController.Instance.mouseInputMode)
		{
			base.StartCoroutine(this.RefreshNavEndOfFrame());
		}
	}

	// Token: 0x06001D8F RID: 7567 RVA: 0x001A1553 File Offset: 0x0019F753
	public virtual void OnRightClick()
	{
		if (this.refreshControllerNavigationOnPress && !InputController.Instance.mouseInputMode)
		{
			base.StartCoroutine(this.RefreshNavEndOfFrame());
		}
	}

	// Token: 0x06001D90 RID: 7568 RVA: 0x001A1553 File Offset: 0x0019F753
	public virtual void OnLeftDoubleClick()
	{
		if (this.refreshControllerNavigationOnPress && !InputController.Instance.mouseInputMode)
		{
			base.StartCoroutine(this.RefreshNavEndOfFrame());
		}
	}

	// Token: 0x06001D91 RID: 7569 RVA: 0x001A1553 File Offset: 0x0019F753
	public virtual void OnRightDoubleClick()
	{
		if (this.refreshControllerNavigationOnPress && !InputController.Instance.mouseInputMode)
		{
			base.StartCoroutine(this.RefreshNavEndOfFrame());
		}
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x001A1576 File Offset: 0x0019F776
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if (InterfaceController.Instance != null && InputController.Instance.mouseInputMode)
		{
			InterfaceController.Instance.AddMouseOverElement(this);
			this.OnSelect();
		}
	}

	// Token: 0x06001D93 RID: 7571 RVA: 0x001A15A4 File Offset: 0x0019F7A4
	public virtual void OnSelect()
	{
		if (InterfaceController.Instance.selectedElement == this)
		{
			return;
		}
		if (this == null || base.gameObject == null)
		{
			return;
		}
		Game.Log("Menu: OnSelect: " + base.gameObject.name, 2);
		if (this.refreshControllerNavigationOnSelect)
		{
			this.RefreshAutomaticNavigation();
		}
		if (InterfaceController.Instance != null)
		{
			if (this.button != null)
			{
				this.button.Select();
			}
			InterfaceController.Instance.selectedElement = this;
			InterfaceController.Instance.selectedElementTag = base.tag;
			if (MainMenuController.Instance != null)
			{
				MainMenuController.Instance.OnNewMouseOver();
			}
			if (CasePanelController.Instance.controllerMode && CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.topBar && base.transform.tag == "TopPanelButtons")
			{
				CasePanelController.Instance.selectedTopBarButton = this;
			}
		}
		if (this.OnHoverChange != null)
		{
			this.OnHoverChange(this, true);
		}
		this.isOver = true;
		this.OnHoverStart();
		if (this.useAdditionalHighlight)
		{
			this.UpdateAdditionalHighlight();
		}
		if (this.glowOnHighlight && this.juice != null && this.interactable)
		{
			this.juice.Pulsate(true, false);
		}
		if (this.scrollRectAutoScroll && !InputController.Instance.mouseInputMode)
		{
			this.AutoScroll();
		}
		if (this.tooltip != null)
		{
			this.tooltip.OnButtonHover();
		}
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x001A1728 File Offset: 0x0019F928
	public void AutoScroll()
	{
		CustomScrollRect componentInParent = base.gameObject.GetComponentInParent<CustomScrollRect>();
		if (componentInParent != null)
		{
			Toolbox.Instance.ScrollRectPosition(componentInParent, this.rect, this.scrollHorizontal, this.scrollVertical, 0.2f);
		}
	}

	// Token: 0x06001D95 RID: 7573 RVA: 0x001A176C File Offset: 0x0019F96C
	private void OnEnable()
	{
		if (this.button != null)
		{
			this.button.interactable = this.interactable;
		}
		if (InputController.Instance != null && !InputController.Instance.mouseInputMode)
		{
			base.StartCoroutine(this.RefreshNavEndOfFrame());
		}
	}

	// Token: 0x06001D96 RID: 7574 RVA: 0x001A17C0 File Offset: 0x0019F9C0
	private void OnDisable()
	{
		if (this.button != null)
		{
			this.button.interactable = false;
		}
		if (this.tooltip != null && this.tooltip.isOver)
		{
			this.tooltip.OnButtonExitHover();
		}
		if (InterfaceController.Instance != null && InputController.Instance.mouseInputMode)
		{
			InterfaceController.Instance.RemoveMouseOverElement(this);
		}
	}

	// Token: 0x06001D97 RID: 7575 RVA: 0x001A1831 File Offset: 0x0019FA31
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (InterfaceController.Instance != null && InputController.Instance.mouseInputMode)
		{
			if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
			{
				return;
			}
			InterfaceController.Instance.RemoveMouseOverElement(this);
			this.OnDeselect();
		}
	}

	// Token: 0x06001D98 RID: 7576 RVA: 0x001A1874 File Offset: 0x0019FA74
	public virtual void OnDeselect()
	{
		if (InterfaceController.Instance != null)
		{
			if (InterfaceController.Instance.selectedElement == this)
			{
				Game.Log("Menu: OnDeselect: " + base.gameObject.name, 2);
				InterfaceController.Instance.selectedElement = null;
				InterfaceController.Instance.selectedElementTag = string.Empty;
			}
			if (MainMenuController.Instance != null)
			{
				MainMenuController.Instance.OnNewMouseOver();
			}
		}
		if (this.OnHoverChange != null)
		{
			this.OnHoverChange(this, false);
		}
		this.isOver = false;
		this.OnHoverEnd();
		if (this.useAdditionalHighlight)
		{
			this.UpdateAdditionalHighlight();
		}
		if (this.glowOnHighlight && this.juice != null)
		{
			this.juice.Pulsate(false, false);
			this.background.color = this.baseColour;
		}
		if (this.tooltip != null)
		{
			this.tooltip.OnButtonExitHover();
		}
	}

	// Token: 0x06001D99 RID: 7577 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnHoverStart()
	{
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnHoverEnd()
	{
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x001A196C File Offset: 0x0019FB6C
	public virtual void SetButtonBaseColour(Color col)
	{
		this.baseColour = col;
		if (this.button == null)
		{
			return;
		}
		this.button.image.color = col;
		if (this.juice != null)
		{
			foreach (JuiceController.JuiceElement juiceElement in this.juice.elements)
			{
				if (juiceElement.imageElement != null)
				{
					juiceElement.originalColour = juiceElement.imageElement.color;
				}
				else if (juiceElement.rawImageElement != null)
				{
					juiceElement.originalColour = juiceElement.rawImageElement.color;
				}
			}
		}
	}

	// Token: 0x06001D9C RID: 7580 RVA: 0x001A1A34 File Offset: 0x0019FC34
	public void SetupAdditionalHighlight()
	{
		if (this.additionalHighlightPrefab == null)
		{
			this.additionalHighlightPrefab = PrefabControls.Instance.buttonAdditionalHighlight;
		}
		if (this.rect == null)
		{
			this.rect = base.gameObject.GetComponent<RectTransform>();
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.additionalHighlightPrefab, this.rect);
		this.additionalHighlightRect = gameObject.GetComponent<RectTransform>();
		this.additionalHImage = gameObject.GetComponent<Image>();
		this.additionalHImage.color = this.additionalHighlightColour;
		Toolbox.Instance.SetRectSize(this.additionalHighlightRect, this.additionalHighlightRectModifier.x, this.additionalHighlightRectModifier.y, this.additionalHighlightRectModifier.z, this.additionalHighlightRectModifier.w);
		if (!this.additionalHighlightAtFront)
		{
			this.additionalHighlightRect.SetAsFirstSibling();
		}
		else
		{
			this.additionalHighlightRect.SetAsLastSibling();
		}
		this.additionalHighlightRect.gameObject.SetActive(false);
		this.UpdateAdditionalHighlight();
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x001A1B2C File Offset: 0x0019FD2C
	public virtual void UpdateAdditionalHighlight()
	{
		if (this.button == null)
		{
			this.button = base.gameObject.GetComponent<Button>();
		}
		if (this.additionalHighlightRect == null)
		{
			this.SetupAdditionalHighlight();
		}
		bool flag = false;
		if (this.useAdditionalHighlight && this.isOver)
		{
			flag = true;
		}
		if (this.forceAdditionalHighlighted)
		{
			flag = true;
		}
		else if (!this.forceAdditionalHighlighted && !this.useAdditionalHighlight)
		{
			flag = false;
		}
		if (!this.additionalHighlighted && flag)
		{
			this.additionalHighlighted = true;
			this.additionalHighlightRect.gameObject.SetActive(true);
		}
		else if (this.additionalHighlighted && !flag)
		{
			this.additionalHighlighted = false;
			this.additionalHighlightRect.gameObject.SetActive(false);
		}
		if (this.interactable)
		{
			this.additionalHImage.color = this.additionalHighlightColour;
			return;
		}
		this.additionalHImage.color = this.additionalHighlightUninteractableColour;
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x001A1C13 File Offset: 0x0019FE13
	public void SetForceAdditionalHighlight(bool newVal)
	{
		this.forceAdditionalHighlighted = newVal;
		this.UpdateAdditionalHighlight();
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x001A1C22 File Offset: 0x0019FE22
	public void Flash(int repeat, Color flashColour)
	{
		base.StartCoroutine(this.FlashColour(repeat, flashColour));
	}

	// Token: 0x06001DA0 RID: 7584 RVA: 0x001A1C33 File Offset: 0x0019FE33
	public IEnumerator FlashColour(int repeat, Color flashColour)
	{
		int cycle = 0;
		float progress = 0f;
		float speed = 10f;
		while (cycle < repeat && progress < 2f)
		{
			progress += speed * Time.deltaTime;
			float num;
			if (progress <= 1f)
			{
				num = progress;
			}
			else
			{
				num = 2f - progress;
			}
			this.background.color = Color.Lerp(this.baseColour, flashColour, num);
			if (progress >= 2f)
			{
				int num2 = cycle;
				cycle = num2 + 1;
				progress = 0f;
			}
			yield return null;
		}
		this.background.color = this.baseColour;
		yield break;
	}

	// Token: 0x06001DA1 RID: 7585 RVA: 0x001A1C50 File Offset: 0x0019FE50
	public IEnumerator RefreshNavEndOfFrame()
	{
		bool waited = false;
		while (!waited)
		{
			waited = true;
			yield return null;
		}
		this.RefreshAutomaticNavigation();
		yield break;
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x001A1C60 File Offset: 0x0019FE60
	private void OnDestroy()
	{
		try
		{
			if (this.additionalHighlightRect != null)
			{
				Object.Destroy(this.additionalHighlightRect.gameObject);
			}
			InterfaceController.Instance.RemoveMouseOverElement(this);
		}
		catch
		{
		}
	}

	// Token: 0x06001DA3 RID: 7587 RVA: 0x001A1CAC File Offset: 0x0019FEAC
	public void RefreshAutomaticNavigation()
	{
		this.RefreshAutomaticNavigation(this.allowLeftNavigation, this.allowRightNavigation, this.allowUpNavigation, this.allowDownNavigation, this.includeInactiveSelectables);
	}

	// Token: 0x06001DA4 RID: 7588 RVA: 0x001A1CD4 File Offset: 0x0019FED4
	public void RefreshAutomaticNavigation(bool enableLeft, bool enableRight, bool enableUp, bool enableDown, bool includeInactive)
	{
		if (InputController.Instance.mouseInputMode)
		{
			return;
		}
		this.allowLeftNavigation = enableLeft;
		this.allowRightNavigation = enableRight;
		this.allowUpNavigation = enableUp;
		this.allowDownNavigation = enableDown;
		this.includeInactiveSelectables = includeInactive;
		if (this.button != null)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Menu: Refreshing auto navigation for " + base.name, 2);
			}
			Selectable selectable = this.button.navigation.selectOnRight;
			Selectable selectable2 = this.button.navigation.selectOnLeft;
			Selectable selectable3 = this.button.navigation.selectOnUp;
			Selectable selectable4 = this.button.navigation.selectOnDown;
			if (!this.includeInactiveSelectables)
			{
				if (selectable != null && !selectable.gameObject.activeInHierarchy)
				{
					selectable = null;
				}
				if (selectable2 != null && !selectable2.gameObject.activeInHierarchy)
				{
					selectable2 = null;
				}
				if (selectable3 != null && !selectable3.gameObject.activeInHierarchy)
				{
					selectable3 = null;
				}
				if (selectable4 != null && !selectable4.gameObject.activeInHierarchy)
				{
					selectable4 = null;
				}
			}
			Transform transform = base.transform;
			if (this.isEvidenceWindowButton)
			{
				InfoWindow componentInParent = base.gameObject.GetComponentInParent<InfoWindow>();
				if (componentInParent != null)
				{
					transform = componentInParent.transform;
				}
				else
				{
					for (int i = 0; i < this.selectableSearchParentHierarchyThreshold; i++)
					{
						if (transform.parent != null)
						{
							transform = transform.parent;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < this.selectableSearchParentHierarchyThreshold; j++)
				{
					if (transform.parent != null)
					{
						transform = transform.parent;
					}
				}
			}
			ButtonController[] componentsInChildren = transform.GetComponentsInChildren<ButtonController>(this.includeInactiveSelectables);
			Vector3 vector = this.rect.TransformPoint(this.rect.rect.center);
			if (this.thisNavRectPoint == ButtonController.NavRectPoint.min)
			{
				vector = this.rect.TransformPoint(this.rect.rect.min);
			}
			else if (this.thisNavRectPoint == ButtonController.NavRectPoint.max)
			{
				vector = this.rect.TransformPoint(this.rect.rect.max);
			}
			if (Game.Instance.printDebug)
			{
				string[] array = new string[7];
				array[0] = "Menu: Found ";
				int num = 1;
				int k = componentsInChildren.Length;
				array[num] = k.ToString();
				array[2] = " in ";
				array[3] = transform.name;
				array[4] = " (screen point: ";
				int num2 = 5;
				Vector3 vector2 = vector;
				array[num2] = vector2.ToString();
				array[6] = ")";
				Game.Log(string.Concat(array), 2);
			}
			Vector3 vector3;
			vector3..ctor(0.5f, 1f, 1f);
			Vector3 vector4;
			vector4..ctor(1f, 0.5f, 1f);
			List<ButtonController.NavRanking> list = new List<ButtonController.NavRanking>();
			foreach (ButtonController buttonController in componentsInChildren)
			{
				if (buttonController.findableForAutoNavigation && !(buttonController.button == null) && (this.includeInactiveSelectables || buttonController.gameObject.activeInHierarchy) && !(buttonController == this))
				{
					GraphicRaycaster componentInParent2 = buttonController.GetComponentInParent<GraphicRaycaster>();
					if (componentInParent2 != null && componentInParent2.enabled)
					{
						if (this.ignoreParentsNamed.Count > 0)
						{
							Transform transform2 = buttonController.transform;
							bool flag = false;
							int num3 = 0;
							while (num3 < 100 && transform2 != null)
							{
								foreach (string text in this.ignoreParentsNamed)
								{
									if (transform2.name.ToLower() == text.ToLower())
									{
										flag = true;
										break;
									}
								}
								if (!(transform2.parent != null))
								{
									break;
								}
								transform2 = transform2.parent;
								if (flag)
								{
									break;
								}
								num3++;
							}
							if (flag)
							{
								goto IL_77D;
							}
						}
						Vector3 vector5 = buttonController.rect.TransformPoint(buttonController.rect.rect.center);
						if (this.otherNavRectPoint == ButtonController.NavRectPoint.min)
						{
							vector5 = buttonController.rect.TransformPoint(buttonController.rect.rect.min);
						}
						else if (this.otherNavRectPoint == ButtonController.NavRectPoint.max)
						{
							vector5 = buttonController.rect.TransformPoint(buttonController.rect.rect.max);
						}
						if (this.allowLeftNavigation && vector5.x < vector.x - 6f)
						{
							float score = Vector3.Distance(new Vector3(vector5.x * vector3.x, vector5.y * vector3.y, vector5.z * vector3.z), new Vector3(vector.x * vector3.x, vector.y * vector3.y, vector.z * vector3.z));
							list.Add(new ButtonController.NavRanking
							{
								button = buttonController,
								score = score,
								dir = 2
							});
						}
						if (this.allowRightNavigation && vector5.x > vector.x + 6f)
						{
							float score2 = Vector3.Distance(new Vector3(vector5.x * vector3.x, vector5.y * vector3.y, vector5.z * vector3.z), new Vector3(vector.x * vector3.x, vector.y * vector3.y, vector.z * vector3.z));
							list.Add(new ButtonController.NavRanking
							{
								button = buttonController,
								score = score2,
								dir = 3
							});
						}
						if (this.allowUpNavigation && vector5.y > vector.y + 6f)
						{
							float score3 = Vector3.Distance(new Vector3(vector5.x * vector4.x, vector5.y * vector4.y, vector5.z * vector4.z), new Vector3(vector.x * vector4.x, vector.y * vector4.y, vector.z * vector4.z));
							list.Add(new ButtonController.NavRanking
							{
								button = buttonController,
								score = score3,
								dir = 0
							});
						}
						if (this.allowDownNavigation && vector5.y < vector.y - 6f)
						{
							float score4 = Vector3.Distance(new Vector3(vector5.x * vector4.x, vector5.y * vector4.y, vector5.z * vector4.z), new Vector3(vector.x * vector4.x, vector.y * vector4.y, vector.z * vector4.z));
							list.Add(new ButtonController.NavRanking
							{
								button = buttonController,
								score = score4,
								dir = 1
							});
						}
					}
				}
				IL_77D:;
			}
			list.Sort((ButtonController.NavRanking p1, ButtonController.NavRanking p2) => p1.score.CompareTo(p2.score));
			for (int l = 0; l < 4; l++)
			{
				for (int m = 0; m < list.Count; m++)
				{
					ButtonController.NavRanking navRanking = list[m];
					if (navRanking.dir == l)
					{
						if (l == 0)
						{
							selectable3 = navRanking.button.button;
						}
						else if (l == 1)
						{
							selectable4 = navRanking.button.button;
						}
						else if (l == 2)
						{
							selectable2 = navRanking.button.button;
						}
						else if (l == 3)
						{
							selectable = navRanking.button.button;
						}
						if (Game.Instance.printDebug)
						{
							Game.Log("Menu: Selected " + navRanking.button.name + " for " + l.ToString(), 2);
						}
						for (int n = 0; n < list.Count; n++)
						{
							if (list[n].dir == l || list[n].button == navRanking.button)
							{
								list.RemoveAt(n);
								n--;
							}
						}
						break;
					}
				}
			}
			Selectable selectable5 = this.button;
			Navigation navigation = default(Navigation);
			navigation.mode = 4;
			navigation.selectOnLeft = selectable2;
			navigation.selectOnRight = selectable;
			navigation.selectOnUp = selectable3;
			navigation.selectOnDown = selectable4;
			selectable5.navigation = navigation;
		}
	}

	// Token: 0x0400273F RID: 10047
	[Header("References")]
	[Tooltip("Reference to the rect transform component (assigned if not given)")]
	public RectTransform rect;

	// Token: 0x04002740 RID: 10048
	[Tooltip("Reference to the button component (assigned if not given)")]
	public Button button;

	// Token: 0x04002741 RID: 10049
	[Tooltip("Reference to the canvas renderer component (assigned if not given)")]
	public CanvasRenderer rend;

	// Token: 0x04002742 RID: 10050
	[Tooltip("The background image for this button")]
	public Image background;

	// Token: 0x04002743 RID: 10051
	[Tooltip("The icon image for this button")]
	public Image icon;

	// Token: 0x04002744 RID: 10052
	[Tooltip("Text for this button")]
	public TextMeshProUGUI text;

	// Token: 0x04002745 RID: 10053
	[Tooltip("Tooltip control component")]
	public TooltipController tooltip;

	// Token: 0x04002746 RID: 10054
	[Tooltip("Juice controller for effects")]
	public JuiceController juice;

	// Token: 0x04002747 RID: 10055
	[Tooltip("Used for notifications")]
	public NotificationController notifications;

	// Token: 0x04002748 RID: 10056
	[NonSerialized]
	public object genericReference;

	// Token: 0x04002749 RID: 10057
	[Space(5f)]
	[ReadOnly]
	public InfoWindow parentWindow;

	// Token: 0x0400274A RID: 10058
	[NonSerialized]
	public Evidence windowOf;

	// Token: 0x0400274B RID: 10059
	[ReadOnly]
	public WindowTabController tabOf;

	// Token: 0x0400274C RID: 10060
	[ReadOnly]
	public RectTransform additionalHighlightRect;

	// Token: 0x0400274D RID: 10061
	[Tooltip("Is this currently moused-over?")]
	[Header("State")]
	[Space(5f)]
	[ReadOnly]
	public bool isOver;

	// Token: 0x0400274E RID: 10062
	[ReadOnly]
	[Tooltip("Force the additional/manual highlight")]
	public bool forceAdditionalHighlighted;

	// Token: 0x0400274F RID: 10063
	[ReadOnly]
	[Tooltip("Addition/manual highlight active")]
	public bool additionalHighlighted;

	// Token: 0x04002750 RID: 10064
	[Tooltip("Interactable")]
	[ReadOnly]
	public bool interactable = true;

	// Token: 0x04002751 RID: 10065
	public bool setupReferences;

	// Token: 0x04002752 RID: 10066
	private float lastLeftClick;

	// Token: 0x04002753 RID: 10067
	private float lastRightClick;

	// Token: 0x04002754 RID: 10068
	[BoxGroup("Button Setup")]
	[Space(7f)]
	[Tooltip("Base colour used for built-in flash functionality")]
	public Color baseColour = Color.white;

	// Token: 0x04002755 RID: 10069
	[Tooltip("If enabled, this will detect if it's in a scroll rect, then adjust scroll accordingly when selected")]
	[BoxGroup("Button Setup")]
	public bool scrollRectAutoScroll = true;

	// Token: 0x04002756 RID: 10070
	[EnableIf("scrollRectAutoScroll")]
	[BoxGroup("Button Setup")]
	public bool scrollVertical = true;

	// Token: 0x04002757 RID: 10071
	[BoxGroup("Button Setup")]
	[EnableIf("scrollRectAutoScroll")]
	public bool scrollHorizontal;

	// Token: 0x04002758 RID: 10072
	[BoxGroup("Auto Navigation")]
	[Tooltip("Can this button be found using automatic navigation of other buttons?")]
	public bool findableForAutoNavigation = true;

	// Token: 0x04002759 RID: 10073
	[Tooltip("Automatically refresh the controller navigation when set up")]
	[BoxGroup("Auto Navigation")]
	public bool refreshControllerNavigationOnSetup;

	// Token: 0x0400275A RID: 10074
	[Tooltip("Automatically refresh the controller navigation when selected")]
	[BoxGroup("Auto Navigation")]
	public bool refreshControllerNavigationOnSelect;

	// Token: 0x0400275B RID: 10075
	[BoxGroup("Auto Navigation")]
	[Tooltip("Automatically refresh the controller navigation when pressed")]
	public bool refreshControllerNavigationOnPress;

	// Token: 0x0400275C RID: 10076
	[BoxGroup("Auto Navigation")]
	[Tooltip("When looking for navigation selectables, include inactive objects")]
	public bool includeInactiveSelectables;

	// Token: 0x0400275D RID: 10077
	[DisableIf("isEvidenceWindowButton")]
	[BoxGroup("Auto Navigation")]
	[Tooltip("When looking for navigation selectables, how many transform parents up should it search for other buttons?")]
	[Range(1f, 10f)]
	public int selectableSearchParentHierarchyThreshold = 1;

	// Token: 0x0400275E RID: 10078
	[Tooltip("A shortcut instead of above; search for components within this window...")]
	[BoxGroup("Auto Navigation")]
	public bool isEvidenceWindowButton;

	// Token: 0x0400275F RID: 10079
	[Tooltip("When auto navigation is setup, should we allow a search for selectables on the left?")]
	[BoxGroup("Auto Navigation")]
	public bool allowLeftNavigation = true;

	// Token: 0x04002760 RID: 10080
	[Tooltip("When auto navigation is setup, should we allow a search for selectables on the right?")]
	[BoxGroup("Auto Navigation")]
	public bool allowRightNavigation = true;

	// Token: 0x04002761 RID: 10081
	[Tooltip("When auto navigation is setup, should we allow a search for selectables up?")]
	[BoxGroup("Auto Navigation")]
	public bool allowUpNavigation = true;

	// Token: 0x04002762 RID: 10082
	[Tooltip("When auto navigation is setup, should we allow a search for selectables down?")]
	[BoxGroup("Auto Navigation")]
	public bool allowDownNavigation = true;

	// Token: 0x04002763 RID: 10083
	[Tooltip("When measuring distances to to other nav buttons, use this point of the rect")]
	[BoxGroup("Auto Navigation")]
	public ButtonController.NavRectPoint thisNavRectPoint;

	// Token: 0x04002764 RID: 10084
	[BoxGroup("Auto Navigation")]
	[Tooltip("When measuring distances to to other nav buttons, use this point of the other rect")]
	public ButtonController.NavRectPoint otherNavRectPoint;

	// Token: 0x04002765 RID: 10085
	[BoxGroup("Auto Navigation")]
	[Tooltip("When selecting from auto navigation, ignore the following objects if they are parents")]
	public List<string> ignoreParentsNamed = new List<string>();

	// Token: 0x04002766 RID: 10086
	[BoxGroup("Auto Navigation")]
	[Tooltip("When using a controller, the secondary action button is classed as a right click")]
	public bool secondaryIsRightClick;

	// Token: 0x04002767 RID: 10087
	[Space(5f)]
	[BoxGroup("Text")]
	[Tooltip("Automatically set the button text")]
	public bool useAutomaticText;

	// Token: 0x04002768 RID: 10088
	[BoxGroup("Text")]
	[EnableIf("useAutomaticText")]
	[Tooltip("Use this dictionary reference to get the text")]
	public string textDictionary = "ui.interface";

	// Token: 0x04002769 RID: 10089
	[BoxGroup("Text")]
	[Tooltip("Use this reference to get the text")]
	[EnableIf("useAutomaticText")]
	public string textReference;

	// Token: 0x0400276A RID: 10090
	[BoxGroup("Text")]
	[EnableIf("useAutomaticText")]
	public Strings.Casing casing;

	// Token: 0x0400276B RID: 10091
	[BoxGroup("Text")]
	public string menuMouseoverReference;

	// Token: 0x0400276C RID: 10092
	[Space(5f)]
	[BoxGroup("Interactability")]
	[Range(0f, 1f)]
	public float uninteractableTextAlpha = 0.25f;

	// Token: 0x0400276D RID: 10093
	[BoxGroup("Interactability")]
	[Range(0f, 1f)]
	public float interactableTextAlpha = 1f;

	// Token: 0x0400276E RID: 10094
	[Range(-2f, 2f)]
	[Tooltip("If nothing is selected, the priority of this button being the defauly selection vs others")]
	[BoxGroup("Interactability")]
	public int defaultSelectionPriority;

	// Token: 0x0400276F RID: 10095
	[Tooltip("Use the additional/manual highlight functionality")]
	[Space(5f)]
	[BoxGroup("Additional Highlighter")]
	public bool useAdditionalHighlight = true;

	// Token: 0x04002770 RID: 10096
	[BoxGroup("Additional Highlighter")]
	[Tooltip("Prefab that acts as the additional/manual highlighter. If empty a default highlighter will be used.")]
	public GameObject additionalHighlightPrefab;

	// Token: 0x04002771 RID: 10097
	[Tooltip("Colour applied to the additional/manual highlighter")]
	[BoxGroup("Additional Highlighter")]
	public Color additionalHighlightColour = Color.white;

	// Token: 0x04002772 RID: 10098
	[BoxGroup("Additional Highlighter")]
	[Tooltip("Colour applied to the additional/manual highlighter when uninteractable")]
	public Color additionalHighlightUninteractableColour = Color.gray;

	// Token: 0x04002773 RID: 10099
	[BoxGroup("Additional Highlighter")]
	[Tooltip("Set the additional/manual highlight to the front")]
	public bool additionalHighlightAtFront;

	// Token: 0x04002774 RID: 10100
	[BoxGroup("Additional Highlighter")]
	[Tooltip("Add to the rect size of the highlighter")]
	public Vector4 additionalHighlightRectModifier = Vector4.zero;

	// Token: 0x04002775 RID: 10101
	private Image additionalHImage;

	// Token: 0x04002776 RID: 10102
	[Space(5f)]
	[BoxGroup("Juice")]
	public bool nudgeOnClick = true;

	// Token: 0x04002777 RID: 10103
	[BoxGroup("Juice")]
	public bool glowOnHighlight = true;

	// Token: 0x04002778 RID: 10104
	[Tooltip("Use generic button sounds")]
	[Space(5f)]
	[BoxGroup("Audio")]
	public bool useGenericAudioSounds = true;

	// Token: 0x04002779 RID: 10105
	[BoxGroup("Audio")]
	[EnableIf("useGenericAudioSounds")]
	public ButtonController.ButtonAudioType buttonType;

	// Token: 0x0400277A RID: 10106
	[BoxGroup("Audio")]
	[DisableIf("useGenericAudioSounds")]
	public AudioEvent buttonDown;

	// Token: 0x0400277B RID: 10107
	[DisableIf("useGenericAudioSounds")]
	[BoxGroup("Audio")]
	public AudioEvent clickPrimary;

	// Token: 0x0400277C RID: 10108
	[DisableIf("useGenericAudioSounds")]
	[BoxGroup("Audio")]
	public AudioEvent clickSecondary;

	// Token: 0x0400277D RID: 10109
	[DisableIf("useGenericAudioSounds")]
	[BoxGroup("Audio")]
	public AudioEvent rightClick;

	// Token: 0x0200054D RID: 1357
	public enum NavRectPoint
	{
		// Token: 0x04002781 RID: 10113
		center,
		// Token: 0x04002782 RID: 10114
		min,
		// Token: 0x04002783 RID: 10115
		max
	}

	// Token: 0x0200054E RID: 1358
	public enum ButtonAudioType
	{
		// Token: 0x04002785 RID: 10117
		normal,
		// Token: 0x04002786 RID: 10118
		forward,
		// Token: 0x04002787 RID: 10119
		back,
		// Token: 0x04002788 RID: 10120
		tickBox
	}

	// Token: 0x0200054F RID: 1359
	public struct NavRanking
	{
		// Token: 0x04002789 RID: 10121
		public ButtonController button;

		// Token: 0x0400278A RID: 10122
		public float score;

		// Token: 0x0400278B RID: 10123
		public int dir;
	}

	// Token: 0x02000550 RID: 1360
	// (Invoke) Token: 0x06001DA7 RID: 7591
	public delegate void Press(ButtonController thisButton);

	// Token: 0x02000551 RID: 1361
	// (Invoke) Token: 0x06001DAB RID: 7595
	public delegate void HoverChange(ButtonController thisButton, bool mouseOver);
}
