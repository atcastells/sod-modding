using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x020005C9 RID: 1481
public class BioScreenController : MonoBehaviour
{
	// Token: 0x14000046 RID: 70
	// (add) Token: 0x06002072 RID: 8306 RVA: 0x001BC8F4 File Offset: 0x001BAAF4
	// (remove) Token: 0x06002073 RID: 8307 RVA: 0x001BC92C File Offset: 0x001BAB2C
	public event BioScreenController.InventoryOpenChange OnInventoryOpenChange;

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06002074 RID: 8308 RVA: 0x001BC961 File Offset: 0x001BAB61
	public static BioScreenController Instance
	{
		get
		{
			return BioScreenController._instance;
		}
	}

	// Token: 0x06002075 RID: 8309 RVA: 0x001BC968 File Offset: 0x001BAB68
	private void Awake()
	{
		if (BioScreenController._instance != null && BioScreenController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			BioScreenController._instance = this;
		}
		foreach (CanvasRenderer canvasRenderer in this.socialCreditRenderers)
		{
			if (!(canvasRenderer == null))
			{
				canvasRenderer.SetAlpha(this.socialCreditDisplayProgress);
			}
		}
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x001BC9F8 File Offset: 0x001BABF8
	private void Start()
	{
		if (this.inventoryTitleText != null)
		{
			this.inventoryTitleText.text = Strings.Get("ui.interface", "Inventory", Strings.Casing.asIs, false, false, false, null);
		}
		if (this.socialLevelBarRect != null)
		{
			this.barHeight = this.socialLevelBarRect.rect.height;
		}
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x001BCA5C File Offset: 0x001BAC5C
	public void SetMaxSocialCreditLevels(int newMax)
	{
		this.maxLevels = newMax;
		for (int i = 0; i < this.levelBlips.Count; i++)
		{
			Object.Destroy(this.levelBlips[i].gameObject);
		}
		for (int j = 0; j < this.socialCreditRenderers.Count; j++)
		{
			if (this.socialCreditRenderers[j] == null)
			{
				this.socialCreditRenderers.RemoveAt(j);
				j--;
			}
		}
		this.levelBlips.Clear();
		this.maxPoints = GameplayController.Instance.GetSocialCreditThresholdForLevel(this.maxLevels);
		Game.Log(string.Concat(new string[]
		{
			"Gameplay: Set new max social credit levels: ",
			this.maxLevels.ToString(),
			", needing a total of ",
			this.maxPoints.ToString(),
			" points"
		}), 2);
		for (int k = 2; k <= this.maxLevels; k++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.levelBlipPrefab, this.levelBarBlipParent);
			ButtonController component = gameObject.GetComponent<ButtonController>();
			this.levelBlips.Add(component);
			int socialCreditThresholdForLevel = GameplayController.Instance.GetSocialCreditThresholdForLevel(k);
			component.rect.anchoredPosition = new Vector2(-26f, this.barHeight / (float)this.maxPoints * (float)socialCreditThresholdForLevel - 4f);
			gameObject.name = string.Concat(new string[]
			{
				k.ToString(),
				": ",
				socialCreditThresholdForLevel.ToString(),
				" (",
				(this.barHeight / (float)this.maxPoints * (float)socialCreditThresholdForLevel).ToString(),
				")"
			});
			component.text.text = k.ToString();
			component.tooltip.mainText = Strings.Get("ui.tooltips", "Social Credit Level", Strings.Casing.asIs, false, false, false, null) + " " + k.ToString();
		}
		foreach (ButtonController buttonController in this.levelBlips)
		{
			buttonController.juice.Pulsate(false, false);
			buttonController.notifications.gameObject.SetActive(false);
			buttonController.SetButtonBaseColour(this.futureLevel);
			CanvasRenderer[] componentsInChildren = buttonController.gameObject.GetComponentsInChildren<CanvasRenderer>(true);
			this.socialCreditRenderers.AddRange(componentsInChildren);
		}
		this.OnChangePoints(false);
		this.UpdateSocialCreditPerks();
	}

	// Token: 0x06002078 RID: 8312 RVA: 0x001BCCE0 File Offset: 0x001BAEE0
	public void SetInventoryOpen(bool val, bool forceUpdate, bool resumeGame = true)
	{
		if (CutSceneController.Instance.cutSceneActive)
		{
			val = false;
		}
		if (Player.Instance.playerKOInProgress)
		{
			val = false;
		}
		if (this.isOpen != val || forceUpdate)
		{
			this.isOpen = val;
			this.openedFromPause = !SessionData.Instance.play;
			Game.Log("Player: Inventory screen active: " + this.isOpen.ToString(), 2);
			InterfaceController.Instance.personButton.icon.enabled = BioScreenController.Instance.isOpen;
			InterfaceController.Instance.caseCanvasRaycaster.enabled = !this.isOpen;
			InterfaceController.Instance.windowCanvas.enabled = !this.isOpen;
			if (this.isOpen)
			{
				if (SessionData.Instance.play && (!Player.Instance.autoTravelActive || Game.Instance.autoTravelPause))
				{
					SessionData.Instance.PauseGame(true, false, false);
				}
				SessionData.Instance.TutorialTrigger("inventory", false);
				if (UpgradesController.Instance.isOpen)
				{
					UpgradesController.Instance.CloseUpgrades(true);
				}
				this.UpdateButtons();
				if (this.selectedSlot == null || (this.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && this.selectedSlot.GetInteractable() == null))
				{
					this.SelectSlot(FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, false);
				}
				this.dropButton.OnDeselect();
				this.inspectButton.OnDeselect();
				this.equipButton.OnDeselect();
				this.scanButton.OnDeselect();
				this.editDecorButton.OnDeselect();
				if (this.cashText != null)
				{
					this.cashText.text = InterfaceControls.Instance.cashText.text;
				}
				Player.Instance.EnablePlayerMouseLook(false, false);
				if (!InputController.Instance.mouseInputMode)
				{
					CasePanelController.Instance.SetControllerMode(true, CasePanelController.ControllerSelectMode.topBar);
				}
			}
			else
			{
				if (resumeGame && !InterfaceController.Instance.desktopMode && !PopupMessageController.Instance.active && !MainMenuController.Instance.mainMenuActive)
				{
					SessionData.Instance.ResumeGame();
					Player.Instance.EnablePlayerMouseLook(true, true);
				}
				this.openedFromPause = false;
				this.HoverSlot(null);
				if (!InterfaceController.Instance.desktopMode)
				{
					CasePanelController.Instance.SetControllerMode(false, CasePanelController.Instance.currentSelectMode);
				}
			}
			InteractionController.Instance.UpdateInteractionText();
			this.UpdateSummary();
			if (this.OnInventoryOpenChange != null)
			{
				this.OnInventoryOpenChange();
			}
		}
	}

	// Token: 0x06002079 RID: 8313 RVA: 0x001BCF78 File Offset: 0x001BB178
	public void HoverSlot(FirstPersonItemController.InventorySlot newSlot)
	{
		bool flag = false;
		if (this.hoveredSlot != newSlot)
		{
			flag = true;
		}
		this.hoveredSlot = newSlot;
		if (this.hoveredSlot != null)
		{
			this.hoverIndex = FirstPersonItemController.Instance.slots.IndexOf(this.hoveredSlot);
		}
		if (this.hoveredSlot != null)
		{
			foreach (FirstPersonItemController.InventorySlot inventorySlot in FirstPersonItemController.Instance.slots)
			{
				if (inventorySlot == this.hoveredSlot)
				{
					inventorySlot.spawnedSegment.text.fontStyle = 4;
				}
				else
				{
					inventorySlot.spawnedSegment.text.fontStyle = 0;
				}
			}
		}
		InteractionController.Instance.UpdateInteractionText();
		if (flag)
		{
			this.UpdateSummary();
		}
	}

	// Token: 0x0600207A RID: 8314 RVA: 0x001BD048 File Offset: 0x001BB248
	public void SelectSlot(FirstPersonItemController.InventorySlot newSlot, bool closeInventory = false, bool forceUpdate = false)
	{
		if (newSlot == null)
		{
			return;
		}
		Game.Log("Player: Select new inventory slot: " + newSlot.index.ToString(), 2);
		if (this.selectedSlot != newSlot || forceUpdate)
		{
			if (FirstPersonItemController.Instance.isConsuming)
			{
				FirstPersonItemController.Instance.SetConsuming(false);
			}
			if (FirstPersonItemController.Instance.isRaised)
			{
				FirstPersonItemController.Instance.SetRaised(false);
			}
			if (InteractionController.Instance.carryingObject != null && newSlot != null && newSlot.isStatic != FirstPersonItemController.InventorySlot.StaticSlot.holster)
			{
				InteractionController.Instance.carryingObject.DropThis(false);
			}
			FirstPersonItemController.Instance.forceHolstered = false;
			this.selectedSlot = newSlot;
			foreach (FirstPersonItemController.InventorySlot inventorySlot in FirstPersonItemController.Instance.slots)
			{
				if (inventorySlot.spawnedSegment != null)
				{
					if (this.selectedSlot == inventorySlot)
					{
						inventorySlot.spawnedSegment.selected.gameObject.SetActive(true);
					}
					else
					{
						inventorySlot.spawnedSegment.selected.gameObject.SetActive(false);
					}
				}
			}
			if (this.selectedSlot != null)
			{
				FirstPersonItemController.Instance.SetFirstPersonItem(this.selectedSlot.GetFirstPersonItem(), true);
			}
			this.UpdateButtons();
		}
		if (closeInventory)
		{
			this.SetInventoryOpen(false, true, true);
			return;
		}
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x0600207B RID: 8315 RVA: 0x001BD1B8 File Offset: 0x001BB3B8
	public void UpdateButtons()
	{
		if (this.selectedSlot != null)
		{
			if (this.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
			{
				Interactable interactable = this.selectedSlot.GetInteractable();
				if (interactable != null)
				{
					this.dropButton.SetInteractable(true);
					if (interactable.evidence != null)
					{
						this.inspectButton.SetInteractable(true);
						this.scanButton.SetInteractable(true);
					}
					else
					{
						this.inspectButton.SetInteractable(false);
						this.scanButton.SetInteractable(false);
					}
				}
				else
				{
					this.dropButton.SetInteractable(false);
					this.inspectButton.SetInteractable(false);
					this.scanButton.SetInteractable(false);
				}
			}
			else
			{
				this.dropButton.SetInteractable(false);
				this.inspectButton.SetInteractable(false);
				this.scanButton.SetInteractable(false);
			}
		}
		else
		{
			this.dropButton.SetInteractable(false);
			this.inspectButton.SetInteractable(false);
			this.scanButton.SetInteractable(false);
		}
		if (this.scanningItem == null)
		{
			if (FirstPersonItemController.Instance.slots.Exists((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.printReader))
			{
				return;
			}
		}
		this.scanButton.SetInteractable(false);
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x001BD2F0 File Offset: 0x001BB4F0
	public void UpdateDecorEditButton()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if ((Player.Instance.currentGameLocation == Player.Instance.home || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, Player.Instance.currentGameLocation)) && (Game.Instance.sandboxMode || ChapterController.Instance.currentPart >= 30))
		{
			this.editDecorButton.gameObject.SetActive(true);
			return;
		}
		this.editDecorButton.gameObject.SetActive(false);
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x001BD380 File Offset: 0x001BB580
	public void UpdateSummary()
	{
		this.summaryText.text = string.Empty;
		this.summaryTextToDisplay = string.Empty;
		this.summaryTextProgress = 0f;
		if (this.hoveredSlot != null && this.isOpen)
		{
			Interactable interactable = this.hoveredSlot.GetInteractable();
			if (interactable != null)
			{
				this.summaryTextToDisplay = Strings.GetTextForComponent(interactable.preset.summaryMessageSource, interactable, null, null, "\n", false, null, Strings.LinkSetting.automatic, null);
			}
			else
			{
				FirstPersonItem firstPersonItem = this.hoveredSlot.GetFirstPersonItem();
				if (firstPersonItem != null && firstPersonItem.summaryMsgID != null && firstPersonItem.summaryMsgID.Length > 0)
				{
					this.summaryTextToDisplay = Strings.GetTextForComponent(firstPersonItem.summaryMsgID, null, null, null, "\n", false, null, Strings.LinkSetting.automatic, null);
				}
			}
			this.summaryText.SetText(this.summaryTextToDisplay, true);
			this.summaryText.SetText(string.Empty, true);
		}
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x001BD468 File Offset: 0x001BB668
	public InventorySquareController SpawnSlotObject(FirstPersonItemController.InventorySlot slot)
	{
		RectTransform rectTransform = this.equipmentParentRect;
		if (slot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
		{
			rectTransform = this.itemsParentRect;
		}
		InventorySquareController component = Object.Instantiate<GameObject>(this.inventorySquarePrefab, rectTransform).GetComponent<InventorySquareController>();
		component.Setup(slot);
		return component;
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x001BD4A4 File Offset: 0x001BB6A4
	public void OnChangePoints(bool allowLevelChangeDisplay)
	{
		this.desiredBarFillLevel = Mathf.Clamp01((float)GameplayController.Instance.socialCredit / (float)this.maxPoints) * this.barHeight;
		int currentSocialCreditLevel = GameplayController.Instance.GetCurrentSocialCreditLevel();
		InterfaceControls.Instance.socialRankText.text = currentSocialCreditLevel.ToString();
		if (this.currentLevel != currentSocialCreditLevel)
		{
			if (currentSocialCreditLevel > this.currentLevel && allowLevelChangeDisplay)
			{
				InterfaceController.Instance.SocialCreditLevelUpDisplay();
			}
			this.currentLevel = currentSocialCreditLevel;
			this.UpdateSocialCreditPerks();
			int num = Mathf.Clamp(this.currentLevel - 2, -1, this.levelBlips.Count - 1);
			foreach (ButtonController buttonController in this.levelBlips)
			{
				buttonController.juice.Pulsate(false, false);
				buttonController.notifications.gameObject.SetActive(false);
				if (this.levelBlips.IndexOf(buttonController) <= num)
				{
					buttonController.SetButtonBaseColour(this.clearedLevel);
				}
				else
				{
					buttonController.SetButtonBaseColour(this.futureLevel);
				}
			}
			if (num >= 0)
			{
				this.currentLevelBlip = this.levelBlips[num];
			}
			else
			{
				this.currentLevelBlip = null;
			}
			if (this.currentLevelBlip != null)
			{
				this.currentLevelBlip.juice.Pulsate(true, false);
				this.currentLevelBlip.notifications.SetNotifications(1);
				this.currentLevelBlip.juice.Nudge();
				CanvasRenderer[] componentsInChildren = this.currentLevelBlip.gameObject.GetComponentsInChildren<CanvasRenderer>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].SetAlpha(this.socialCreditDisplayProgress);
				}
			}
			if (currentSocialCreditLevel >= Game.Instance.gameLengthMaxLevels[Game.Instance.gameLength])
			{
				if (!CasePanelController.Instance.activeCases.Exists((Case item) => item.caseType == Case.CaseType.retirement))
				{
					Game.Log("Gameplay: Trigger retirement...", 2);
					Case @case = CasePanelController.Instance.CreateNewCase(Case.CaseType.retirement, Case.CaseStatus.handInNotCollected, true, Strings.Get("ui.interface", "Retirement", Strings.Casing.asIs, false, false, false, null));
					foreach (Case.ResolveQuestion resolveQuestion in GameplayControls.Instance.retirementResolveQuestions)
					{
						Case.ResolveQuestion resolveQuestion2 = new Case.ResolveQuestion();
						resolveQuestion2.name = resolveQuestion.name;
						resolveQuestion2.inputType = resolveQuestion.inputType;
						resolveQuestion2.tag = resolveQuestion.tag;
						resolveQuestion2.rewardRange = resolveQuestion.rewardRange;
						resolveQuestion2.isOptional = resolveQuestion.isOptional;
						resolveQuestion2.displayObjective = resolveQuestion.displayObjective;
						resolveQuestion2.reward = Mathf.RoundToInt(Toolbox.Instance.VectorToRandom(resolveQuestion.rewardRange) * Game.Instance.jobRewardMultiplier / 50f) * 50;
						resolveQuestion2.penalty = Mathf.RoundToInt(Toolbox.Instance.VectorToRandom(resolveQuestion.penaltyRange) * Game.Instance.jobPenaltyMultiplier / 50f) * 50;
						Game.Log("Adding resolve question: " + resolveQuestion2.name, 2);
						@case.resolveQuestions.Add(resolveQuestion2);
					}
				}
			}
		}
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x001BD808 File Offset: 0x001BBA08
	public void UpdateSocialCreditPerks()
	{
		if (GameplayController.Instance.socialCreditPerks.Count < this.currentLevel - 1 && Game.Instance.allowSocialCreditPerks)
		{
			Dictionary<int, List<SocialControls.SocialCreditBuff>> dictionary = new Dictionary<int, List<SocialControls.SocialCreditBuff>>();
			foreach (SocialControls.SocialCreditBuff socialCreditBuff in SocialControls.Instance.socialCreditBuffs)
			{
				if (!dictionary.ContainsKey(socialCreditBuff.randomGrouping))
				{
					dictionary.Add(socialCreditBuff.randomGrouping, new List<SocialControls.SocialCreditBuff>());
				}
				dictionary[socialCreditBuff.randomGrouping].Add(socialCreditBuff);
			}
			int num = 999;
			while (GameplayController.Instance.socialCreditPerks.Count < this.currentLevel - 1 && num > 0 && GameplayController.Instance.socialCreditPerks.Count < SocialControls.Instance.socialCreditBuffs.Count)
			{
				int count = GameplayController.Instance.socialCreditPerks.Count;
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < count; i++)
				{
					if (!dictionary.ContainsKey(num2))
					{
						return;
					}
					if (num3 >= dictionary[num2].Count - 1)
					{
						num2++;
						num3 = 0;
					}
					else
					{
						num3++;
					}
				}
				if (dictionary.ContainsKey(num2))
				{
					List<SocialControls.SocialCreditBuff> list = new List<SocialControls.SocialCreditBuff>();
					using (List<SocialControls.SocialCreditBuff>.Enumerator enumerator = dictionary[num2].GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SocialControls.SocialCreditBuff buff = enumerator.Current;
							if (!GameplayController.Instance.socialCreditPerks.Exists((SocialControls.SocialCreditBuff item) => item.name == buff.name) && !list.Contains(buff))
							{
								list.Add(buff);
							}
						}
					}
					Game.Log("Player: Trying to grant new social credit perk for group " + num2.ToString() + ", choosing from a pool of " + list.Count.ToString(), 2);
					if (list.Count > 0)
					{
						SocialControls.SocialCreditBuff socialCreditBuff2 = list[Toolbox.Instance.Rand(0, list.Count, false)];
						if (socialCreditBuff2 != null)
						{
							Game.Log("Player: Granting perk " + socialCreditBuff2.name, 2);
							this.NewSocialCreditPerk(socialCreditBuff2, true);
						}
					}
				}
				num--;
				if (num <= 0)
				{
					Game.LogError("Safety reached for assigning new social credit perk! Something has gone wrong...", 2);
				}
			}
			UpgradesController.Instance.UpdateActivation();
		}
		this.UpdateLevelBlipsWithPerkTooltips();
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x001BDA94 File Offset: 0x001BBC94
	public void NewSocialCreditPerk(SocialControls.SocialCreditBuff newPerk, bool allowDisplay = true)
	{
		if (newPerk == null)
		{
			return;
		}
		if (!Game.Instance.allowSocialCreditPerks)
		{
			return;
		}
		if (!GameplayController.Instance.socialCreditPerks.Contains(newPerk))
		{
			GameplayController.Instance.socialCreditPerks.Add(newPerk);
			if (allowDisplay)
			{
				base.StartCoroutine(this.DisplayNewPerk(newPerk, 12f));
			}
			this.UpdateLevelBlipsWithPerkTooltips();
		}
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x001BDAF0 File Offset: 0x001BBCF0
	public void UpdateLevelBlipsWithPerkTooltips()
	{
		for (int i = 0; i < this.levelBlips.Count; i++)
		{
			ButtonController buttonController = this.levelBlips[i];
			if (buttonController != null)
			{
				if (GameplayController.Instance.socialCreditPerks.Count > i)
				{
					buttonController.tooltip.detailText = Strings.Get("ui.interface", GameplayController.Instance.socialCreditPerks[i].description, Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					buttonController.tooltip.detailText = string.Empty;
				}
			}
		}
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x001BDB7C File Offset: 0x001BBD7C
	private IEnumerator DisplayNewPerk(SocialControls.SocialCreditBuff newPerk, float delayTime)
	{
		while (delayTime > 0f)
		{
			if (SessionData.Instance.play)
			{
				delayTime -= Time.deltaTime;
			}
			yield return null;
		}
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.interface", "New Perk!", Strings.Casing.asIs, false, false, false, null) + " " + Strings.Get("ui.interface", newPerk.description, Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.star, SocialControls.Instance.perkNotificationAudioEvent, true, InterfaceControls.Instance.messageYellow, -1, 0f, InterfaceController.Instance.bioNotificationIcon, GameMessageController.PingOnComplete.none, null, null, null);
		yield break;
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x001BDB94 File Offset: 0x001BBD94
	private void Update()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (this.barFill.sizeDelta.y < this.desiredBarFillLevel)
		{
			float num = Mathf.Max(Mathf.Abs(this.barFill.sizeDelta.y - this.desiredBarFillLevel), 1f);
			this.barFill.sizeDelta = new Vector2(this.barFill.sizeDelta.x, Mathf.Min(this.desiredBarFillLevel, this.barFill.sizeDelta.y + num * Time.deltaTime));
			if (!this.barJuice.pulsateActive)
			{
				this.barJuice.Pulsate(true, true);
			}
			this.socialCreditBarDisplayTimer = 5f;
		}
		else if (this.barFill.sizeDelta.y > this.desiredBarFillLevel)
		{
			float num2 = Mathf.Max(Mathf.Abs(this.barFill.sizeDelta.y - this.desiredBarFillLevel), 1f);
			this.barFill.sizeDelta = new Vector2(this.barFill.sizeDelta.x, Mathf.Max(this.desiredBarFillLevel, this.barFill.sizeDelta.y - num2 * Time.deltaTime));
			if (!this.barJuice.pulsateActive)
			{
				this.barJuice.Pulsate(true, true);
			}
			this.socialCreditBarDisplayTimer = 5f;
		}
		else if (this.barJuice.pulsateActive)
		{
			this.barJuice.Pulsate(false, true);
		}
		if (this.socialCreditBarDisplayTimer > 0f && ((!InterfaceController.Instance.gameScreenQueued && !InterfaceController.Instance.gameSceenDisplayed) || (InterfaceController.Instance.gameSceenDisplayed && InterfaceController.Instance.currentGameScreen == InterfaceController.ScreenDisplayType.socialCreditLevelUp)))
		{
			if (!this.socialCreditDisplayParent.gameObject.activeSelf)
			{
				this.socialCreditDisplayParent.gameObject.SetActive(true);
			}
			if (this.socialCreditDisplayProgress >= 1f)
			{
				this.socialCreditBarDisplayTimer -= Time.deltaTime;
				this.socialCreditBarDisplayTimer = Mathf.Max(this.socialCreditBarDisplayTimer, 0f);
			}
			else
			{
				this.socialCreditDisplayProgress += Time.deltaTime * 3f;
				this.socialCreditDisplayProgress = Mathf.Clamp01(this.socialCreditDisplayProgress);
				foreach (CanvasRenderer canvasRenderer in this.socialCreditRenderers)
				{
					if (!(canvasRenderer == null))
					{
						canvasRenderer.SetAlpha(this.socialCreditDisplayProgress);
					}
				}
				InterfaceController.Instance.objectiveSideAnchor.anchoredPosition = new Vector2(Mathf.Lerp(0f, 600f, this.socialCreditDisplayProgress), 0f);
			}
		}
		else if (this.socialCreditDisplayProgress > 0f)
		{
			this.socialCreditDisplayProgress -= Time.deltaTime;
			this.socialCreditDisplayProgress = Mathf.Max(this.socialCreditDisplayProgress, 0f);
			foreach (CanvasRenderer canvasRenderer2 in this.socialCreditRenderers)
			{
				if (!(canvasRenderer2 == null))
				{
					canvasRenderer2.SetAlpha(this.socialCreditDisplayProgress);
				}
			}
			InterfaceController.Instance.objectiveSideAnchor.anchoredPosition = new Vector2(Mathf.Lerp(0f, 600f, this.socialCreditDisplayProgress), 0f);
		}
		else if (this.socialCreditDisplayParent.gameObject.activeSelf)
		{
			InterfaceController.Instance.objectiveSideAnchor.anchoredPosition = Vector2.zero;
			this.socialCreditDisplayParent.gameObject.SetActive(false);
		}
		if (this.currentLevelBlip != null && this.socialCreditDisplayProgress >= 1f)
		{
			this.currentLevelBlip.notifications.gameObject.SetActive(true);
		}
		if (this.isOpen)
		{
			if (!this.inventoryParentRect.gameObject.activeSelf)
			{
				this.inventoryParentRect.gameObject.SetActive(true);
			}
			if (this.solidBG.gameObject.activeSelf == SessionData.Instance.play)
			{
				this.solidBG.gameObject.SetActive(!SessionData.Instance.play);
			}
			this.socialCreditBarDisplayTimer = Mathf.Max(1f, this.socialCreditBarDisplayTimer);
			if (SessionData.Instance.play)
			{
				if (this.inventoryDisplayProgress < 1f)
				{
					this.inventoryDisplayProgress += Time.deltaTime / 0.2f;
					this.inventoryDisplayProgress = Mathf.Clamp01(this.inventoryDisplayProgress);
					foreach (CanvasRenderer canvasRenderer3 in this.inventoryRenderers)
					{
						canvasRenderer3.SetAlpha(this.inventoryDisplayProgress);
					}
					foreach (FirstPersonItemController.InventorySlot inventorySlot in FirstPersonItemController.Instance.slots)
					{
						if (inventorySlot.spawnedSegment != null)
						{
							foreach (CanvasRenderer canvasRenderer4 in inventorySlot.spawnedSegment.renderers)
							{
								canvasRenderer4.SetAlpha(this.inventoryDisplayProgress);
							}
						}
					}
					float num3 = InterfaceController.Instance.radialActivateScale.Evaluate(this.inventoryDisplayProgress);
					this.inventoryParentRect.localScale = new Vector3(num3, num3, num3);
					if (this.inventoryDisplayProgress >= 1f)
					{
						this.inventoryParentRect.localScale = Vector3.one;
					}
					InterfaceController.Instance.helpPointerRect.anchoredPosition = new Vector2(InterfaceController.Instance.helpPointerRect.anchoredPosition.x, Mathf.Lerp(-120f, -8f, this.inventoryDisplayProgress));
				}
			}
			else if (this.inventoryDisplayProgress < 1f)
			{
				this.inventoryDisplayProgress = 1f;
				foreach (CanvasRenderer canvasRenderer5 in this.inventoryRenderers)
				{
					canvasRenderer5.SetAlpha(this.inventoryDisplayProgress);
				}
				foreach (FirstPersonItemController.InventorySlot inventorySlot2 in FirstPersonItemController.Instance.slots)
				{
					if (inventorySlot2.spawnedSegment != null)
					{
						foreach (CanvasRenderer canvasRenderer6 in inventorySlot2.spawnedSegment.renderers)
						{
							canvasRenderer6.SetAlpha(this.inventoryDisplayProgress);
						}
					}
				}
				InterfaceController.Instance.helpPointerRect.gameObject.SetActive(false);
				InterfaceController.Instance.helpPointerRect.anchoredPosition = new Vector2(InterfaceController.Instance.helpPointerRect.anchoredPosition.x, Mathf.Lerp(-120f, -8f, this.inventoryDisplayProgress));
				this.inventoryParentRect.localScale = Vector3.one;
			}
			if (this.summaryTextToDisplay != null && this.summaryTextToDisplay.Length > 0 && this.summaryTextProgress < 1f)
			{
				this.summaryTextProgress += Time.deltaTime / 0.25f;
				this.summaryTextProgress = Mathf.Clamp01(this.summaryTextProgress);
				this.summaryText.text = this.summaryTextToDisplay.Substring(0, Mathf.CeilToInt(this.summaryTextProgress * (float)this.summaryTextToDisplay.Length));
			}
		}
		else if (SessionData.Instance.play)
		{
			if (this.inventoryDisplayProgress > 0f)
			{
				this.inventoryDisplayProgress -= Time.deltaTime / 0.25f;
				this.inventoryDisplayProgress = Mathf.Clamp01(this.inventoryDisplayProgress);
				foreach (CanvasRenderer canvasRenderer7 in this.inventoryRenderers)
				{
					canvasRenderer7.SetAlpha(this.inventoryDisplayProgress);
				}
				foreach (FirstPersonItemController.InventorySlot inventorySlot3 in FirstPersonItemController.Instance.slots)
				{
					if (inventorySlot3.spawnedSegment != null)
					{
						foreach (CanvasRenderer canvasRenderer8 in inventorySlot3.spawnedSegment.renderers)
						{
							canvasRenderer8.SetAlpha(this.inventoryDisplayProgress);
						}
					}
				}
				float num4 = InterfaceController.Instance.radialActivateScale.Evaluate(this.inventoryDisplayProgress);
				this.inventoryParentRect.localScale = new Vector3(num4, num4, num4);
				if (this.inventoryDisplayProgress <= 0f)
				{
					this.inventoryParentRect.localScale = Vector3.one;
					this.inventoryParentRect.gameObject.SetActive(false);
				}
				InterfaceController.Instance.helpPointerRect.anchoredPosition = new Vector2(InterfaceController.Instance.helpPointerRect.anchoredPosition.x, Mathf.Lerp(-120f, -8f, this.inventoryDisplayProgress));
			}
		}
		else if (this.inventoryDisplayProgress != 0f)
		{
			this.inventoryDisplayProgress = 0f;
			foreach (CanvasRenderer canvasRenderer9 in this.inventoryRenderers)
			{
				canvasRenderer9.SetAlpha(this.inventoryDisplayProgress);
			}
			foreach (FirstPersonItemController.InventorySlot inventorySlot4 in FirstPersonItemController.Instance.slots)
			{
				if (inventorySlot4.spawnedSegment != null)
				{
					foreach (CanvasRenderer canvasRenderer10 in inventorySlot4.spawnedSegment.renderers)
					{
						canvasRenderer10.SetAlpha(this.inventoryDisplayProgress);
					}
				}
			}
			this.inventoryParentRect.localScale = Vector3.one;
			this.inventoryParentRect.gameObject.SetActive(false);
			InterfaceController.Instance.helpPointerRect.anchoredPosition = new Vector2(InterfaceController.Instance.helpPointerRect.anchoredPosition.x, Mathf.Lerp(-120f, -8f, this.inventoryDisplayProgress));
		}
		if (this.scanningItem != null)
		{
			if (this.scanProgress < 1f)
			{
				this.scanProgress += Time.deltaTime * 0.5f;
				this.scanProgressBar.sizeDelta = new Vector2((this.scanButton.rect.sizeDelta.x - 8f) * this.scanProgress, this.scanProgressBar.sizeDelta.y);
				return;
			}
			this.OnScanComplete(this.scanningItem);
			this.scanningItem = null;
			this.scanProgress = 0f;
			this.scanProgressBar.sizeDelta = new Vector2(0f, this.scanProgressBar.sizeDelta.y);
		}
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x001BE788 File Offset: 0x001BC988
	[Button(null, 0)]
	public void AddSocialCredit()
	{
		GameplayController.Instance.AddSocialCredit(GameplayControls.Instance.socialCreditForSideJobs, true, "Debug button");
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x001BE7A4 File Offset: 0x001BC9A4
	public void DecorEditButton()
	{
		if ((Player.Instance.currentGameLocation == Player.Instance.home || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, Player.Instance.currentGameLocation)) && !SessionData.Instance.isDecorEdit)
		{
			InteractionController.Instance.StartDecorEdit();
		}
	}

	// Token: 0x06002087 RID: 8327 RVA: 0x001BE7FD File Offset: 0x001BC9FD
	public void DropButton()
	{
		if (this.selectedSlot != null && this.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && this.selectedSlot.GetInteractable() != null)
		{
			FirstPersonItemController.Instance.EmptySlot(this.selectedSlot, false, false, true, true);
		}
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x001BE838 File Offset: 0x001BCA38
	public void InspectButton()
	{
		if (this.selectedSlot != null && this.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && this.selectedSlot.GetInteractable() != null)
		{
			Interactable interactable = this.selectedSlot.GetInteractable();
			if (interactable != null && interactable.evidence != null)
			{
				this.SetInventoryOpen(false, true, false);
				InterfaceController.Instance.SetDesktopMode(true, true);
				InterfaceController instance = InterfaceController.Instance;
				Evidence evidence = interactable.evidence;
				Evidence.DataKey passedEvidenceKey = Evidence.DataKey.name;
				List<Evidence.DataKey> passedEvidenceKeys = null;
				string presetName = "";
				bool worldInteraction = false;
				bool autoPosition = true;
				Interactable passedInteractable = interactable;
				instance.SpawnWindow(evidence, passedEvidenceKey, passedEvidenceKeys, presetName, worldInteraction, autoPosition, default(Vector2), passedInteractable, null, null, true);
			}
		}
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x001BE8BC File Offset: 0x001BCABC
	public void ScanButton()
	{
		if (this.scanningItem == null)
		{
			this.scanningItem = this.selectedSlot.GetInteractable();
			this.scanProgress = 0f;
			AudioController.Instance.Play2DSound(AudioControls.Instance.printScannerSelect, null, 1f);
			this.UpdateButtons();
		}
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x001BE910 File Offset: 0x001BCB10
	public void OnScanComplete(Interactable scanCompleteOn)
	{
		AudioController.Instance.StopSound(this.scannerLoop, AudioController.StopType.fade, "");
		AudioController.Instance.Play2DSound(AudioControls.Instance.printScannerHolster, null, 1f);
		if (scanCompleteOn == null)
		{
			return;
		}
		if (this.scannedObjectsPrintsCache.ContainsKey(scanCompleteOn))
		{
			if (this.scannedObjectsPrintsCache[scanCompleteOn].Count <= 0)
			{
				goto IL_2D1;
			}
			this.SetInventoryOpen(false, true, false);
			InterfaceController.Instance.SetDesktopMode(true, true);
			using (List<Interactable>.Enumerator enumerator = this.scannedObjectsPrintsCache[scanCompleteOn].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Interactable interactable = enumerator.Current;
					InterfaceController instance = InterfaceController.Instance;
					Evidence evidence = interactable.evidence;
					Evidence.DataKey passedEvidenceKey = Evidence.DataKey.fingerprints;
					List<Evidence.DataKey> passedEvidenceKeys = null;
					string presetName = "";
					bool worldInteraction = false;
					bool autoPosition = true;
					Interactable passedInteractable = interactable;
					instance.SpawnWindow(evidence, passedEvidenceKey, passedEvidenceKeys, presetName, worldInteraction, autoPosition, default(Vector2), passedInteractable, null, null, true);
				}
				goto IL_2D1;
			}
		}
		if (scanCompleteOn.preset.fingerprintsEnabled)
		{
			List<Human> list = new List<Human>(Toolbox.Instance.GetFingerprintOwnerPool(null, null, scanCompleteOn, scanCompleteOn.preset.printsSource, Vector3.zero, false));
			if (scanCompleteOn.preset.enableDynamicFingerprints && scanCompleteOn.df.Count > 0)
			{
				foreach (Interactable.DynamicFingerprint dynamicFingerprint in scanCompleteOn.df)
				{
					Human human = null;
					if (CityData.Instance.GetHuman(dynamicFingerprint.id, out human, true))
					{
						if (!list.Contains(human))
						{
							list.Add(human);
						}
					}
					else
					{
						Game.LogError("Cannot find citizen " + dynamicFingerprint.id.ToString(), 2);
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				FingerprintScannerController.Print print = new FingerprintScannerController.Print();
				print.interactable = scanCompleteOn;
				print.type = FingerprintScannerController.Print.PrintType.fingerPrint;
				print.dynamicOwner = list[i];
				Game.Log("Player: Discovered print belonging to " + list[i].GetCitizenName(), 2);
				if (list[i].fingerprintLoop <= -1)
				{
					list[i].fingerprintLoop = GameplayController.Instance.printsLetterLoop;
					GameplayController.Instance.printsLetterLoop++;
				}
				Interactable interactable2 = InteractableCreator.Instance.CreateFingerprintInteractable(list[i], Vector3.zero, Vector3.zero, print);
				if (!this.scannedObjectsPrintsCache.ContainsKey(scanCompleteOn))
				{
					this.scannedObjectsPrintsCache.Add(scanCompleteOn, new List<Interactable>());
				}
				this.scannedObjectsPrintsCache[scanCompleteOn].Add(interactable2);
				this.SetInventoryOpen(false, true, false);
				InterfaceController.Instance.SetDesktopMode(true, true);
				InterfaceController instance2 = InterfaceController.Instance;
				Evidence evidence2 = interactable2.evidence;
				Evidence.DataKey passedEvidenceKey2 = Evidence.DataKey.fingerprints;
				List<Evidence.DataKey> passedEvidenceKeys2 = null;
				string presetName2 = "";
				bool worldInteraction2 = false;
				bool autoPosition2 = true;
				Interactable passedInteractable = interactable2;
				instance2.SpawnWindow(evidence2, passedEvidenceKey2, passedEvidenceKeys2, presetName2, worldInteraction2, autoPosition2, default(Vector2), passedInteractable, null, null, true);
			}
		}
		IL_2D1:
		this.UpdateButtons();
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x001BEC10 File Offset: 0x001BCE10
	public void EquipButton()
	{
		if (this.selectedSlot != null)
		{
			this.SelectSlot(this.selectedSlot, false, false);
		}
	}

	// Token: 0x0600208C RID: 8332 RVA: 0x00002265 File Offset: 0x00000465
	public void MoreOptionsButton()
	{
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x001BEC28 File Offset: 0x001BCE28
	public void CloseButton()
	{
		this.SetInventoryOpen(false, false, true);
	}

	// Token: 0x04002A90 RID: 10896
	[Header("Components")]
	public Canvas canvasParent;

	// Token: 0x04002A91 RID: 10897
	public RectTransform socialCreditDisplayParent;

	// Token: 0x04002A92 RID: 10898
	public RectTransform levelBarBlipParent;

	// Token: 0x04002A93 RID: 10899
	public RectTransform socialLevelBarRect;

	// Token: 0x04002A94 RID: 10900
	public RectTransform barFill;

	// Token: 0x04002A95 RID: 10901
	public JuiceController barJuice;

	// Token: 0x04002A96 RID: 10902
	public List<CanvasRenderer> socialCreditRenderers = new List<CanvasRenderer>();

	// Token: 0x04002A97 RID: 10903
	[Space(7f)]
	public RectTransform inventoryParentRect;

	// Token: 0x04002A98 RID: 10904
	public RectTransform solidBG;

	// Token: 0x04002A99 RID: 10905
	public ButtonController closeButton;

	// Token: 0x04002A9A RID: 10906
	public RectTransform equipmentParentRect;

	// Token: 0x04002A9B RID: 10907
	public RectTransform itemsParentRect;

	// Token: 0x04002A9C RID: 10908
	public TextMeshProUGUI inventoryTitleText;

	// Token: 0x04002A9D RID: 10909
	public TextMeshProUGUI cashText;

	// Token: 0x04002A9E RID: 10910
	public RectTransform summaryTextRect;

	// Token: 0x04002A9F RID: 10911
	public TextMeshProUGUI summaryText;

	// Token: 0x04002AA0 RID: 10912
	public RectTransform buttonAreaParent;

	// Token: 0x04002AA1 RID: 10913
	public ButtonController dropButton;

	// Token: 0x04002AA2 RID: 10914
	public ButtonController inspectButton;

	// Token: 0x04002AA3 RID: 10915
	public ButtonController equipButton;

	// Token: 0x04002AA4 RID: 10916
	public ButtonController scanButton;

	// Token: 0x04002AA5 RID: 10917
	public ButtonController moreOptionsButton;

	// Token: 0x04002AA6 RID: 10918
	public ButtonController editDecorButton;

	// Token: 0x04002AA7 RID: 10919
	public RectTransform scanProgressBar;

	// Token: 0x04002AA8 RID: 10920
	public ButtonController selectNothingButton;

	// Token: 0x04002AA9 RID: 10921
	public InventorySquareController nothingSquare;

	// Token: 0x04002AAA RID: 10922
	public List<CanvasRenderer> inventoryRenderers = new List<CanvasRenderer>();

	// Token: 0x04002AAB RID: 10923
	[Header("Settings")]
	public GameObject levelBlipPrefab;

	// Token: 0x04002AAC RID: 10924
	public Color clearedLevel = Color.white;

	// Token: 0x04002AAD RID: 10925
	public Color futureLevel = Color.grey;

	// Token: 0x04002AAE RID: 10926
	[Space(7f)]
	public GameObject inventorySquarePrefab;

	// Token: 0x04002AAF RID: 10927
	public Sprite equipmentBGIcon;

	// Token: 0x04002AB0 RID: 10928
	public Sprite itemBGIcon;

	// Token: 0x04002AB1 RID: 10929
	[Header("State")]
	public int maxLevels = 10;

	// Token: 0x04002AB2 RID: 10930
	public int maxPoints;

	// Token: 0x04002AB3 RID: 10931
	public float desiredBarFillLevel;

	// Token: 0x04002AB4 RID: 10932
	public float barHeight = -1f;

	// Token: 0x04002AB5 RID: 10933
	public int currentLevel;

	// Token: 0x04002AB6 RID: 10934
	private List<ButtonController> levelBlips = new List<ButtonController>();

	// Token: 0x04002AB7 RID: 10935
	public float socialCreditBarDisplayTimer;

	// Token: 0x04002AB8 RID: 10936
	public float socialCreditDisplayProgress;

	// Token: 0x04002AB9 RID: 10937
	private ButtonController currentLevelBlip;

	// Token: 0x04002ABA RID: 10938
	public bool openedFromPause;

	// Token: 0x04002ABB RID: 10939
	[Space(7f)]
	public bool isOpen;

	// Token: 0x04002ABC RID: 10940
	public float inventoryDisplayProgress;

	// Token: 0x04002ABD RID: 10941
	[NonSerialized]
	public FirstPersonItemController.InventorySlot hoveredSlot;

	// Token: 0x04002ABE RID: 10942
	[NonSerialized]
	public FirstPersonItemController.InventorySlot selectedSlot;

	// Token: 0x04002ABF RID: 10943
	public int hoverIndex;

	// Token: 0x04002AC0 RID: 10944
	private string summaryTextToDisplay;

	// Token: 0x04002AC1 RID: 10945
	private float summaryTextProgress;

	// Token: 0x04002AC2 RID: 10946
	[NonSerialized]
	public Interactable scanningItem;

	// Token: 0x04002AC3 RID: 10947
	public float scanProgress;

	// Token: 0x04002AC4 RID: 10948
	private AudioController.LoopingSoundInfo scannerLoop;

	// Token: 0x04002AC5 RID: 10949
	public Dictionary<Interactable, List<Interactable>> scannedObjectsPrintsCache = new Dictionary<Interactable, List<Interactable>>();

	// Token: 0x04002AC7 RID: 10951
	private static BioScreenController _instance;

	// Token: 0x020005CA RID: 1482
	// (Invoke) Token: 0x06002090 RID: 8336
	public delegate void InventoryOpenChange();
}
