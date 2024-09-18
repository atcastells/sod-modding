using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000263 RID: 611
[Serializable]
public class Objective
{
	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000E27 RID: 3623 RVA: 0x000CA414 File Offset: 0x000C8614
	// (remove) Token: 0x06000E28 RID: 3624 RVA: 0x000CA44C File Offset: 0x000C864C
	public event Objective.ProgressChange OnProgressChange;

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06000E29 RID: 3625 RVA: 0x000CA484 File Offset: 0x000C8684
	// (remove) Token: 0x06000E2A RID: 3626 RVA: 0x000CA4BC File Offset: 0x000C86BC
	public event Objective.Completed OnComplete;

	// Token: 0x06000E2B RID: 3627 RVA: 0x000CA4F4 File Offset: 0x000C86F4
	public Objective(SpeechController.QueueElement newQueueElement)
	{
		this.queueElement = newQueueElement;
		this.thisCase = CasePanelController.Instance.activeCases.Find((Case item) => item.id == this.queueElement.caseID);
		if (this.thisCase == null)
		{
			this.thisCase = CasePanelController.Instance.archivedCases.Find((Case item) => item.id == this.queueElement.caseID);
		}
		if (this.thisCase == null)
		{
			Game.LogError("Cannot find case with ID " + this.queueElement.caseID.ToString(), 2);
		}
		if (!this.queueElement.isSilent)
		{
			if (this.thisCase.caseType == Case.CaseType.mainStory)
			{
				this.name = Strings.Get(ChapterController.Instance.loadedChapter.dictionary, this.queueElement.entryRef, Strings.Casing.asIs, false, false, true, Player.Instance);
				this.name = Strings.ComposeText(this.name, null, Strings.LinkSetting.automatic, null, null, false);
			}
			else if (this.thisCase.caseType == Case.CaseType.sideJob || this.thisCase.caseType == Case.CaseType.murder || this.thisCase.caseType == Case.CaseType.retirement)
			{
				SideJob sideJob = null;
				if (this.queueElement.jobRef > -1 && !SideJobController.Instance.allJobsDictionary.TryGetValue(this.queueElement.jobRef, ref sideJob))
				{
					Game.LogError("Unable to get job from reference ID " + this.queueElement.jobRef.ToString(), 2);
				}
				if (this.queueElement.useParsing)
				{
					this.name = Strings.Get("missions.postings", this.queueElement.entryRef, Strings.Casing.asIs, false, false, true, Player.Instance);
					this.name = Strings.ComposeText(this.name, sideJob, Strings.LinkSetting.automatic, null, sideJob, false);
				}
				else
				{
					this.name = this.queueElement.entryRef;
				}
			}
		}
		else
		{
			this.name = this.queueElement.entryRef;
		}
		this.progress = 0f;
		this.Setup(this.thisCase);
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x000CA6F0 File Offset: 0x000C88F0
	public void Setup(Case newCase)
	{
		this.sprite = InterfaceControls.Instance.iconReference.Find((InterfaceControls.IconConfig item) => item.iconType == this.queueElement.icon).sprite;
		this.thisCase = newCase;
		if (this.thisCase.waitForObjectives == null)
		{
			this.thisCase.waitForObjectives = new List<Objective>();
		}
		Game.Log(string.Concat(new string[]
		{
			"Objective: Objective setup: ",
			this.queueElement.entryRef,
			" (Progress: ",
			this.progress.ToString(),
			" Complete: ",
			this.isComplete.ToString(),
			")"
		}), 2);
		foreach (Objective.ObjectiveTrigger objectiveTrigger in this.queueElement.triggers)
		{
			objectiveTrigger.SetupNonSerialized();
			if (objectiveTrigger.addedToObjectives == null)
			{
				objectiveTrigger.addedToObjectives = new List<Objective>();
			}
			if (!objectiveTrigger.addedToObjectives.Contains(this))
			{
				objectiveTrigger.addedToObjectives.Add(this);
			}
			if (objectiveTrigger.triggered)
			{
				objectiveTrigger.Trigger(true);
			}
		}
		if (this.queueElement.onComplete == Objective.OnCompleteAction.nextPartWhenAllCompleted && !this.thisCase.waitForObjectives.Contains(this))
		{
			this.thisCase.waitForObjectives.Add(this);
		}
		if (this.thisCase.caseType == Case.CaseType.sideJob && this.thisCase.job != null)
		{
			if (!this.thisCase.job.objectiveReference.ContainsKey(this.queueElement.entryRef))
			{
				this.thisCase.job.objectiveReference.Add(this.queueElement.entryRef, new List<Objective>());
			}
			this.thisCase.job.objectiveReference[this.queueElement.entryRef].Add(this);
			Game.Log(string.Concat(new string[]
			{
				"Objective: Adding objective ",
				this.queueElement.entryRef,
				" to job reference (job ",
				this.thisCase.job.jobID.ToString(),
				", ref count ",
				this.thisCase.job.objectiveReference.Count.ToString(),
				")"
			}), 2);
			this.thisCase.job.OnObjectiveChange();
		}
		this.isSetup = true;
		if (CasePanelController.Instance.activeCase == this.thisCase && !this.isCancelled && !this.isComplete)
		{
			this.Activate(false);
		}
		else if (!this.thisCase.endedObjectives.Contains(this))
		{
			this.thisCase.endedObjectives.Add(this);
		}
		if (this.thisCase.caseStatus == Case.CaseStatus.submitted && !this.isCancelled && !this.isComplete && this.queueElement.entryRef == "case processing")
		{
			if (!GameplayController.Instance.caseProcessing.ContainsKey(this.thisCase))
			{
				GameplayController.Instance.caseProcessing.Add(this.thisCase, SessionData.Instance.gameTime);
				return;
			}
			GameplayController.Instance.caseProcessing[this.thisCase] = SessionData.Instance.gameTime;
		}
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x000CAA50 File Offset: 0x000C8C50
	public void Activate(bool immediate = false)
	{
		Game.Log("Objective: Activate objective: " + this.queueElement.entryRef, 2);
		if (this.objectiveListItem != null)
		{
			Toolbox.Instance.DestroyObject(this.objectiveListItem);
		}
		this.thisCase.inactiveCurrentObjectives.Remove(this);
		if (!this.thisCase.currentActiveObjectives.Contains(this))
		{
			this.thisCase.currentActiveObjectives.Add(this);
		}
		if (this.thisCase.caseType == Case.CaseType.sideJob)
		{
			this.thisCase.job.OnObjectiveChange();
		}
		if (immediate || this.dispPhase >= Objective.DisplayPhase.removeText)
		{
			this.dispPhase = Objective.DisplayPhase.removeText;
			this.fadeInProgress = 0f;
			this.displayProgress = 0f;
			this.displayTime = 0f;
		}
		if (!InterfaceController.Instance.displayedObjectives.Contains(this))
		{
			InterfaceController.Instance.displayedObjectives.Add(this);
		}
		foreach (Objective.ObjectiveTrigger objectiveTrigger in this.queueElement.triggers)
		{
			if (objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.playerAction || objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.playerActionNobodyHome)
			{
				ActionController.Instance.OnPlayerAction += this.OnPlayerAction;
			}
			if (objectiveTrigger.interactable != null && objectiveTrigger.hightlightAction.Length > 0)
			{
				objectiveTrigger.interactable.SetActionHighlight(objectiveTrigger.hightlightAction, true);
				objectiveTrigger.interactable.SetActionDisable(objectiveTrigger.hightlightAction, false);
			}
		}
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x000CABE8 File Offset: 0x000C8DE8
	public void OnPlayerAction(AIActionPreset action, Interactable what, NewNode where, Actor who)
	{
		foreach (Objective.ObjectiveTrigger objectiveTrigger in this.queueElement.triggers)
		{
			Game.Log(string.Concat(new string[]
			{
				"OnPlayerAction: ",
				action.name,
				", ",
				objectiveTrigger.name,
				", triggered: ",
				objectiveTrigger.triggered.ToString()
			}), 2);
			if (!objectiveTrigger.triggered)
			{
				if (objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.playerAction)
				{
					if (objectiveTrigger.interactable == what && objectiveTrigger.name.ToLower() == action.name.ToLower())
					{
						objectiveTrigger.Trigger(false);
					}
				}
				else if (objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.playerActionNobodyHome && objectiveTrigger.gameLocation.currentOccupants.Count <= 0 && objectiveTrigger.name.ToLower() == action.name.ToLower())
				{
					foreach (NewNode.NodeAccess nodeAccess in objectiveTrigger.gameLocation.entrances)
					{
						if (nodeAccess.door != null && nodeAccess.door.doorInteractable == what)
						{
							objectiveTrigger.Trigger(false);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x000CAD8C File Offset: 0x000C8F8C
	public void Complete()
	{
		Game.Log("Chapter: Objective completed! " + this.queueElement.entryRef, 2);
		this.isComplete = true;
		this.progress = 1f;
		if (this.thisCase.job != null && ResultsController.Instance != null)
		{
			bool flag = false;
			foreach (InputFieldController inputFieldController in ResultsController.Instance.spawnedInputFields)
			{
				if (inputFieldController.question.inputType == Case.InputType.objective || inputFieldController.question.inputType == Case.InputType.revengeObjective || inputFieldController.question.inputType == Case.InputType.arrestPurp || inputFieldController.question.inputType == Case.InputType.saveVictim)
				{
					inputFieldController.OnInputEdited();
					flag = true;
				}
			}
			if (flag)
			{
				this.thisCase.ValidationCheck();
			}
		}
		if (this.queueElement.onComplete == Objective.OnCompleteAction.nextChapterPart)
		{
			ChapterController.Instance.LoadPart(ChapterController.Instance.currentPart + 1, false, true);
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.nextPartWhenAllCompleted)
		{
			if (this.thisCase.waitForObjectives.Count <= 0)
			{
				if (Player.Instance.speechController.speechQueue.FindAll((SpeechController.QueueElement item) => item.isObjective).Count <= 0)
				{
					ChapterController.Instance.LoadPart(ChapterController.Instance.currentPart + 1, false, true);
				}
			}
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.specificChapterByString)
		{
			ChapterController.Instance.LoadPart(this.queueElement.chapterString);
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.specificChapterWhenAllCompleted)
		{
			if (this.thisCase.waitForObjectives.Count <= 0)
			{
				if (Player.Instance.speechController.speechQueue.FindAll((SpeechController.QueueElement item) => item.isObjective).Count <= 0)
				{
					ChapterController.Instance.LoadPart(this.queueElement.chapterString);
				}
			}
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.invokeFunction)
		{
			ChapterController.Instance.chapterScript.Invoke(this.queueElement.chapterString, 1f);
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.triggerSideJobFunction)
		{
			SideJobController.Instance.SideJobObjectiveComplete(this.thisCase.job, this);
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.coverUpTips)
		{
			MurderController.Instance.DisplayCoverUpTips();
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.completeCoverUp)
		{
			if (this.queueElement.triggers.Count > 0)
			{
				MurderController.Instance.TriggerSuccessfulCoverUp(this.queueElement.triggers[0].evidence);
			}
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.triggerRansomDelivery)
		{
			MurderController.Instance.TriggerRansomDelivery();
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.triggerRansomCollection)
		{
			MurderController.Instance.KidnapperCollectsRansom();
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.triggerKidnapperRansomCollection)
		{
			MurderController.Instance.KidnapperCollectsRansom();
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.kidnapperCollectedRansom)
		{
			MurderController.Instance.KidnapperCollectedRansom();
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.kidnapperVictimFreed)
		{
			MurderController.Instance.VictimFreed();
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.triggerSideJobHandIn)
		{
			if (this.thisCase.job != null)
			{
				this.thisCase.job.Complete();
			}
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.nextSideJobPhase)
		{
			if (this.thisCase.job != null)
			{
				int num = -1;
				int.TryParse(this.queueElement.chapterString, ref num);
				if (num == this.thisCase.job.phase)
				{
					this.thisCase.job.phase = num + 1;
					Game.Log("Jobs: Side job " + this.thisCase.job.preset.name + " next side job phase trigger: " + this.thisCase.job.phase.ToString(), 2);
				}
				else
				{
					Game.LogError("Invalid current phase detected in objective! Make sure you are passing it in the chapter string. Ignoring...", 2);
				}
			}
		}
		else if (this.queueElement.onComplete == Objective.OnCompleteAction.submitSideJob && this.thisCase.job != null)
		{
			this.thisCase.job.SubmitCase();
		}
		if (!this.thisCase.endedObjectives.Contains(this))
		{
			this.thisCase.endedObjectives.Add(this);
		}
		if (this.dispPhase <= Objective.DisplayPhase.displayMainText)
		{
			this.displayProgress = 99999f;
		}
		if (this.pointer != null)
		{
			this.pointer.Remove();
		}
		if (this.awarenessIcon != null)
		{
			this.awarenessIcon.Remove(false);
		}
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x000CB290 File Offset: 0x000C9490
	public void Cancel()
	{
		Game.Log("Objective: Objective cancelled! " + this.queueElement.entryRef, 2);
		this.isCancelled = true;
		if (!this.thisCase.endedObjectives.Contains(this))
		{
			this.thisCase.endedObjectives.Add(this);
		}
		if (this.dispPhase <= Objective.DisplayPhase.displayMainText)
		{
			this.displayProgress = 99999f;
		}
		if (this.pointer != null)
		{
			this.pointer.Remove();
		}
		if (this.awarenessIcon != null)
		{
			this.awarenessIcon.Remove(false);
		}
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x000CB324 File Offset: 0x000C9524
	public void Remove()
	{
		foreach (Objective.ObjectiveTrigger objectiveTrigger in this.queueElement.triggers)
		{
			if (objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.playerAction || objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.playerActionNobodyHome)
			{
				ActionController.Instance.OnPlayerAction -= this.OnPlayerAction;
			}
			if (objectiveTrigger.interactable != null && objectiveTrigger.hightlightAction.Length > 0)
			{
				objectiveTrigger.interactable.SetActionHighlight(objectiveTrigger.hightlightAction, false);
			}
		}
		if (this.objectiveList != null)
		{
			if (this.isComplete)
			{
				this.objectiveList.OnComplete();
			}
			this.objectiveList.Remove();
		}
		if (this.thisCase.caseType == Case.CaseType.sideJob)
		{
			this.thisCase.job.OnObjectiveChange();
		}
		this.thisCase.waitForObjectives.Remove(this);
		if (this.dispPhase <= Objective.DisplayPhase.displayMainText)
		{
			this.displayProgress = 99999f;
		}
		if (this.pointer != null)
		{
			this.pointer.Remove();
		}
		if (this.awarenessIcon != null)
		{
			this.awarenessIcon.Remove(false);
		}
		if (InterfaceController.Instance.currentlyDisplaying == this)
		{
			InterfaceController.Instance.objectiveTextBackground.gameObject.SetActive(false);
			InterfaceController.Instance.objectiveTitleText.text = string.Empty;
			InterfaceController.Instance.objectiveTitleTextRenderer.SetAlpha(1f);
			InterfaceController.Instance.objectiveBackgroundRenderer.SetAlpha(1f);
			InterfaceController.Instance.objectiveTextBackground.anchoredPosition = new Vector2(0f, -180f);
			InterfaceController.Instance.currentlyDisplaying = null;
		}
		InterfaceController.Instance.displayedObjectives.Remove(this);
		if (!this.isComplete && !this.isCancelled && !this.thisCase.inactiveCurrentObjectives.Contains(this))
		{
			this.thisCase.inactiveCurrentObjectives.Add(this);
		}
		this.thisCase.currentActiveObjectives.Remove(this);
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x000CB540 File Offset: 0x000C9740
	public void CheckingLoop()
	{
		if ((this.isComplete || this.isCancelled || this.queueElement.isSilent) && this.dispPhase <= Objective.DisplayPhase.displayMainText)
		{
			this.displayProgress = 99999f;
		}
		if (this.dispPhase == Objective.DisplayPhase.preDisplay)
		{
			if (InterfaceController.Instance.currentlyDisplaying == null)
			{
				this.dispPhase = Objective.DisplayPhase.fadeInMainText;
				InterfaceController.Instance.currentlyDisplaying = this;
			}
		}
		else if (this.dispPhase == Objective.DisplayPhase.fadeInMainText)
		{
			if (!this.queueElement.isSilent)
			{
				InterfaceController.Instance.objectiveTextBackground.gameObject.SetActive(true);
				InterfaceController.Instance.objectiveTitleText.text = this.name;
			}
			this.displayTime = InterfaceControls.Instance.visualTalkDisplayDestroyDelay + (float)InterfaceController.Instance.objectiveTitleText.text.Length * InterfaceControls.Instance.visualTalkDisplayStringLengthModifier * 0.85f / Game.Instance.textSpeed;
			if (this.fadeInProgress < 1f)
			{
				this.fadeInProgress += Time.deltaTime * 2f;
				this.fadeInProgress = Mathf.Clamp01(this.fadeInProgress);
				InterfaceController.Instance.objectiveBackgroundRenderer.SetAlpha(this.fadeInProgress);
				InterfaceController.Instance.objectiveTitleTextRenderer.SetAlpha(this.fadeInProgress);
			}
			else
			{
				this.displayPointer = true;
				this.dispPhase = Objective.DisplayPhase.displayMainText;
			}
		}
		else if (this.dispPhase == Objective.DisplayPhase.displayMainText)
		{
			if (this.displayProgress < this.displayTime)
			{
				this.displayProgress += Time.deltaTime;
			}
			else
			{
				this.displayPointer = true;
				this.dispPhase = Objective.DisplayPhase.removeText;
			}
		}
		else if (this.dispPhase == Objective.DisplayPhase.removeText)
		{
			this.displayPointer = true;
			if (this.objectiveListItem == null && !this.queueElement.isSilent)
			{
				this.objectiveListItem = Toolbox.Instance.SpawnObject(PrefabControls.Instance.checklistButton, InterfaceController.Instance.objectiveSideAnchor);
				this.objectiveListRect = this.objectiveListItem.GetComponent<RectTransform>();
				this.objectiveList = this.objectiveListItem.GetComponent<ChecklistButtonController>();
				this.objectiveList.Setup(this);
				this.objectiveListRect.position = InterfaceController.Instance.objectiveTextBackground.position;
			}
			if (this.objectiveList != null)
			{
				if (this.objectiveList.fadeInProgress < 1f)
				{
					this.objectiveList.fadeInProgress += Time.deltaTime * 2f;
					this.objectiveList.fadeInProgress = Mathf.Clamp01(this.objectiveList.fadeInProgress);
					this.objectiveList.bgRend.SetAlpha(this.objectiveList.fadeInProgress * InterfaceController.Instance.objectivesAlpha);
					this.objectiveList.textRend.SetAlpha(this.objectiveList.fadeInProgress * InterfaceController.Instance.objectivesAlpha);
					this.objectiveList.iconRend.SetAlpha(this.objectiveList.fadeInProgress * InterfaceController.Instance.objectivesAlpha);
					this.objectiveList.barRend.SetAlpha(this.objectiveList.fadeInProgress * InterfaceController.Instance.objectivesAlpha);
					InterfaceController.Instance.objectiveTitleTextRenderer.SetAlpha(1f - this.objectiveList.fadeInProgress * 2.5f);
					InterfaceController.Instance.objectiveBackgroundRenderer.SetAlpha(1f - this.objectiveList.fadeInProgress * 2.5f);
					InterfaceController.Instance.objectiveTextBackground.anchoredPosition = Vector3.Lerp(InterfaceController.Instance.objectiveTextBackground.anchoredPosition, new Vector2(700f, InterfaceController.Instance.objectiveTextBackground.anchoredPosition.y), Time.deltaTime * 12f);
				}
				else
				{
					this.dispPhase = Objective.DisplayPhase.displayingList;
					InterfaceController.Instance.objectiveTextBackground.gameObject.SetActive(false);
					InterfaceController.Instance.objectiveTitleText.text = string.Empty;
					InterfaceController.Instance.objectiveTitleTextRenderer.SetAlpha(1f);
					InterfaceController.Instance.objectiveBackgroundRenderer.SetAlpha(1f);
					InterfaceController.Instance.objectiveTextBackground.anchoredPosition = new Vector2(0f, -180f);
					InterfaceController.Instance.currentlyDisplaying = null;
					this.dispPhase = Objective.DisplayPhase.waitForComplete;
				}
			}
			else
			{
				InterfaceController.Instance.objectiveTextBackground.gameObject.SetActive(false);
				InterfaceController.Instance.objectiveTitleText.text = string.Empty;
				InterfaceController.Instance.currentlyDisplaying = null;
				this.dispPhase = Objective.DisplayPhase.waitForComplete;
			}
		}
		else if (this.dispPhase == Objective.DisplayPhase.waitForComplete)
		{
			this.displayPointer = true;
			if (this.objectiveListItem == null && !this.queueElement.isSilent)
			{
				this.objectiveListItem = Toolbox.Instance.SpawnObject(PrefabControls.Instance.checklistButton, InterfaceController.Instance.objectiveSideAnchor);
				this.objectiveListRect = this.objectiveListItem.GetComponent<RectTransform>();
				this.objectiveList = this.objectiveListItem.GetComponent<ChecklistButtonController>();
				this.objectiveList.Setup(this);
				this.objectiveListRect.position = InterfaceController.Instance.objectiveTextBackground.position;
			}
			if (this.isComplete || this.isCancelled)
			{
				if (this.objectiveList != null)
				{
					if (this.isComplete)
					{
						this.objectiveList.OnComplete();
					}
					this.objectiveList.Remove();
				}
				this.thisCase.waitForObjectives.Remove(this);
				this.Remove();
			}
		}
		if (this.displayPointer && Game.Instance.objectiveMarkers && this.queueElement.usePointer && this.progress < 1f)
		{
			if (this.awarenessIcon == null)
			{
				Material material = Toolbox.Instance.SpawnMaterial(InterfaceControls.Instance.speech);
				material.SetTexture("_UnlitColorMap", this.sprite.texture);
				this.awarenessIcon = InterfaceController.Instance.AddAwarenessIcon(InterfaceController.AwarenessType.position, InterfaceController.AwarenessBehaviour.invisibleInfront, null, null, this.queueElement.pointerPosition, material, 0, false, 20f);
			}
			if (this.pointer == null && !this.isComplete && !this.isCancelled && !this.queueElement.isSilent)
			{
				this.pointer = InterfaceController.Instance.AddUIPointer(this);
			}
		}
		if (this.displayPointer && this.queueElement.allowCrouchPrompt)
		{
			Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(this.queueElement.pointerPosition);
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && this.queueElement.pointerPosition.y - newNode.position.y < 0.8f)
			{
				if (Vector3.Distance(Player.Instance.transform.position, this.queueElement.pointerPosition) < 1.8f)
				{
					if (this.crouchPromtTimer <= 0f)
					{
						this.crouchPromtTimer = 30f;
						ControlsDisplayController.Instance.DisplayControlIcon(InteractablePreset.InteractionKey.crouch, "Crouch", InterfaceControls.Instance.controlIconDisplayTime, true);
					}
				}
				else
				{
					this.crouchPromtTimer = 0f;
				}
			}
			if (this.crouchPromtTimer > 0f)
			{
				this.crouchPromtTimer -= Time.deltaTime;
			}
		}
		if (!this.isComplete && !this.isCancelled)
		{
			using (List<Objective.ObjectiveTrigger>.Enumerator enumerator = this.queueElement.triggers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Objective.ObjectiveTrigger trig = enumerator.Current;
					if (!trig.triggered)
					{
						if (trig.triggerType == Objective.ObjectiveTriggerType.switchStateTrue)
						{
							if (trig.interactable != null && trig.interactable.sw0)
							{
								trig.Trigger(false);
							}
						}
						else if (trig.triggerType == Objective.ObjectiveTriggerType.switchStateFalse)
						{
							if (trig.interactable != null && !trig.interactable.sw0)
							{
								trig.Trigger(false);
							}
						}
						else
						{
							if (trig.triggerType == Objective.ObjectiveTriggerType.switchStateTrueForType)
							{
								if (trig.interactable == null || !(trig.gameLocation != null))
								{
									continue;
								}
								using (List<NewRoom>.Enumerator enumerator2 = trig.gameLocation.rooms.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										NewRoom newRoom = enumerator2.Current;
										foreach (NewNode newNode2 in newRoom.nodes)
										{
											foreach (Interactable interactable in newNode2.interactables)
											{
												if (interactable.preset == trig.interactable.preset && interactable.sw0)
												{
													trig.Trigger(false);
												}
											}
										}
									}
									continue;
								}
							}
							if (trig.triggerType == Objective.ObjectiveTriggerType.roomLightOn)
							{
								if (trig.room.mainLightStatus)
								{
									trig.Trigger(false);
								}
							}
							else if (trig.triggerType == Objective.ObjectiveTriggerType.inspectInteractable)
							{
								if (trig.interactable != null)
								{
									if (trig.interactable.ins)
									{
										trig.Trigger(false);
									}
								}
								else
								{
									Game.Log(string.Concat(new string[]
									{
										"Objective: No interactable for ",
										this.queueElement.entryRef,
										" (",
										trig.interactableID.ToString(),
										")"
									}), 2);
								}
							}
							else if (trig.triggerType == Objective.ObjectiveTriggerType.itemInInventory)
							{
								if (trig.interactable != null)
								{
									if (FirstPersonItemController.Instance.slots.Exists((FirstPersonItemController.InventorySlot item) => item.GetInteractable() == trig.interactable))
									{
										Game.Log("Objective: Interactable " + trig.interactableID.ToString() + " is in inventory", 2);
										trig.Trigger(false);
									}
								}
								else
								{
									Game.Log(string.Concat(new string[]
									{
										"Objective: No interactable for ",
										this.queueElement.entryRef,
										" (",
										trig.interactableID.ToString(),
										")"
									}), 2);
								}
							}
							else
							{
								if (trig.triggerType == Objective.ObjectiveTriggerType.evidencePinned)
								{
									ControlsDisplayController.Instance.DisplayControlIcon(InteractablePreset.InteractionKey.caseBoard, "Case Board", 2f, false);
									using (List<Case>.Enumerator enumerator5 = CasePanelController.Instance.activeCases.GetEnumerator())
									{
										Predicate<Case.CaseElement> <>9__6;
										while (enumerator5.MoveNext())
										{
											Case @case = enumerator5.Current;
											if (trig.evidence == null)
											{
												Game.LogError("Trigger evidence is null: " + trig.name, 2);
											}
											else
											{
												List<Case.CaseElement> caseElements = @case.caseElements;
												Predicate<Case.CaseElement> predicate;
												if ((predicate = <>9__6) == null)
												{
													predicate = (<>9__6 = ((Case.CaseElement item) => item.id == trig.evidence.evID));
												}
												if (caseElements.Exists(predicate))
												{
													trig.Trigger(false);
												}
											}
										}
										continue;
									}
								}
								if (trig.triggerType == Objective.ObjectiveTriggerType.goToNode)
								{
									if (Player.Instance.currentNode == trig.node && !Player.Instance.inAirVent)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.keyInventory)
								{
									if (trig.door != null)
									{
										if (Player.Instance.keyring.Contains(trig.door))
										{
											trig.Trigger(false);
										}
									}
									else if (trig.interactable != null && Player.Instance.playerKeyringInt.Contains(trig.interactable))
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.interactableRemoved)
								{
									if (trig.interactable == null || trig.interactable.rem || trig.interactable.inInventory == Player.Instance)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.knowDoorLockedStatus)
								{
									if (trig.door.knowLockStatus)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.unlockDoor)
								{
									if (!trig.door.isLocked)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.unlockInteractable)
								{
									if (trig.interactable != null && trig.interactable.lockInteractable != null && !trig.interactable.lockInteractable.locked)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.goToAddress)
								{
									if (Player.Instance.currentGameLocation == trig.gameLocation && !Player.Instance.inAirVent)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.exploreCrimeScene)
								{
									if (Player.Instance.currentGameLocation == trig.gameLocation && !Player.Instance.inAirVent)
									{
										trig.Trigger(false);
									}
									if (MurderController.Instance.currentActiveCase == null || (!GameplayController.Instance.enforcerCalls.ContainsKey(trig.gameLocation) && !trig.gameLocation.isCrimeScene))
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.goToRoom)
								{
									if (Player.Instance.currentRoom == trig.room && !Player.Instance.inAirVent)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.playerHidden)
								{
									if (Player.Instance.isHiding || Player.Instance.inAirVent || (Player.Instance.currentRoom != null && Player.Instance.currentRoom.preset == InteriorControls.Instance.closet))
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.escapeGameLocation)
								{
									if (Player.Instance.currentGameLocation != trig.gameLocation && !Player.Instance.inAirVent)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.escapeBuilding)
								{
									if (Player.Instance.currentBuilding != trig.gameLocation.building)
									{
										string text = "Current building is: ";
										NewBuilding currentBuilding = Player.Instance.currentBuilding;
										Game.Log(text + ((currentBuilding != null) ? currentBuilding.ToString() : null), 2);
										string text2 = "Trigger building is: ";
										NewBuilding building = trig.gameLocation.building;
										Game.Log(text2 + ((building != null) ? building.ToString() : null), 2);
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.answerPhone)
								{
									if (Player.Instance.answeringPhone != null && Player.Instance.answeringPhone.interactable == trig.interactable)
									{
										trig.Trigger(false);
									}
								}
								else if (trig.triggerType == Objective.ObjectiveTriggerType.answerPhoneAndEndCall)
								{
									if (trig.interactable != null && trig.interactable.t.activeCall.Count <= 0)
									{
										trig.Trigger(false);
									}
								}
								else
								{
									if (trig.triggerType == Objective.ObjectiveTriggerType.openEvidence)
									{
										if (InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => item.passedEvidence != null && item.passedEvidence.evID == trig.evidence.evID))
										{
											trig.Trigger(false);
											continue;
										}
										using (List<Case>.Enumerator enumerator5 = CasePanelController.Instance.activeCases.GetEnumerator())
										{
											Predicate<Case.CaseElement> <>9__7;
											Predicate<InfoWindow> <>9__8;
											while (enumerator5.MoveNext())
											{
												Case case2 = enumerator5.Current;
												if (trig.evidence == null)
												{
													Game.LogError(string.Concat(new string[]
													{
														"Trigger evidence is null: ",
														trig.name,
														" for ",
														this.queueElement.entryRef,
														" (completing)"
													}), 2);
													trig.Trigger(false);
												}
												else
												{
													List<Case.CaseElement> caseElements2 = case2.caseElements;
													Predicate<Case.CaseElement> predicate2;
													if ((predicate2 = <>9__7) == null)
													{
														predicate2 = (<>9__7 = ((Case.CaseElement item) => item.id == trig.evidence.evID));
													}
													if (caseElements2.Exists(predicate2))
													{
														List<InfoWindow> activeWindows = InterfaceController.Instance.activeWindows;
														Predicate<InfoWindow> predicate3;
														if ((predicate3 = <>9__8) == null)
														{
															predicate3 = (<>9__8 = ((InfoWindow item) => item.passedEvidence != null && item.passedEvidence.evID == trig.evidence.evID));
														}
														activeWindows.FindAll(predicate3);
														trig.Trigger(false);
													}
												}
											}
											continue;
										}
									}
									if (trig.triggerType == Objective.ObjectiveTriggerType.discoverEvidence)
									{
										if (InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => item.passedEvidence != null && item.passedEvidence.evID == trig.evidence.evID))
										{
											trig.Trigger(false);
											continue;
										}
										if (trig.evidence.isFound || InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => item.passedEvidence != null && item.passedEvidence.evID == trig.evidence.evID))
										{
											trig.Trigger(false);
											continue;
										}
										using (List<Case>.Enumerator enumerator5 = CasePanelController.Instance.activeCases.GetEnumerator())
										{
											Predicate<Case.CaseElement> <>9__9;
											Predicate<InfoWindow> <>9__10;
											while (enumerator5.MoveNext())
											{
												Case case3 = enumerator5.Current;
												if (trig.evidence != null)
												{
													List<Case.CaseElement> caseElements3 = case3.caseElements;
													Predicate<Case.CaseElement> predicate4;
													if ((predicate4 = <>9__9) == null)
													{
														predicate4 = (<>9__9 = ((Case.CaseElement item) => item.id == trig.evidence.evID));
													}
													if (caseElements3.Exists(predicate4))
													{
														List<InfoWindow> activeWindows2 = InterfaceController.Instance.activeWindows;
														Predicate<InfoWindow> predicate5;
														if ((predicate5 = <>9__10) == null)
														{
															predicate5 = (<>9__10 = ((InfoWindow item) => item.passedEvidence != null && item.passedEvidence.evID == trig.evidence.evID));
														}
														activeWindows2.FindAll(predicate5);
														trig.Trigger(false);
													}
												}
											}
											continue;
										}
									}
									if (trig.triggerType == Objective.ObjectiveTriggerType.evidenceOpenAndDisplayed)
									{
										if (InterfaceController.Instance.desktopMode && InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => item.passedEvidence == trig.evidence))
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.collectHandIn)
									{
										if (this.thisCase != null && this.thisCase.caseStatus == Case.CaseStatus.handInCollected)
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.viewHandIn)
									{
										if (this.thisCase != null && this.thisCase.caseStatus == Case.CaseStatus.handInCollected && CasePanelController.Instance.activeCase == this.thisCase && ResolveController.Instance != null)
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.submitCase)
									{
										if (this.thisCase != null && (this.thisCase.caseStatus == Case.CaseStatus.submitted || this.thisCase.caseStatus == Case.CaseStatus.closable || this.thisCase.caseStatus == Case.CaseStatus.archived) && CasePanelController.Instance.activeCase == this.thisCase)
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.plotRoute)
									{
										if (MapController.Instance.playerRoute != null && (MapController.Instance.playerRoute.end.gameLocation == trig.gameLocation || MapController.Instance.playerRoute.destinationTextOverride == trig.gameLocation))
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.gameUnpaused)
									{
										if (SessionData.Instance.play)
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.gamePaused)
									{
										if (!SessionData.Instance.play)
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.goToPublicFacingAddress)
									{
										if (Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.company != null && Player.Instance.currentGameLocation.thisAsAddress.company.preset.publicFacing && Player.Instance.currentGameLocation.thisAsAddress.company.openForBusinessActual && Player.Instance.currentGameLocation.thisAsAddress.company.openForBusinessDesired && !Player.Instance.inAirVent && Player.Instance.currentGameLocation.telephones.Count > 0)
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.linkImageWithName)
									{
										List<Evidence.DataKey> list = new List<Evidence.DataKey>();
										list.Add(Evidence.DataKey.name);
										if (trig.evidence.GetTiedKeys(list).Contains(Evidence.DataKey.photo))
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.identifyFinerprints)
									{
										List<Evidence.DataKey> list2 = new List<Evidence.DataKey>();
										list2.Add(Evidence.DataKey.photo);
										if (trig.evidence.GetTiedKeys(list2).Contains(Evidence.DataKey.fingerprints))
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.viewInteractable)
									{
										if (InteractionController.Instance.currentLookingAtInteractable != null && InteractionController.Instance.currentLookingAtInteractable.interactable == trig.interactable)
										{
											trig.Trigger(false);
										}
									}
									else if (trig.triggerType == Objective.ObjectiveTriggerType.noMoreObjectives)
									{
										if (this.thisCase.currentActiveObjectives.Count == 1 && Player.Instance.speechController.speechQueue.Count <= 0 && this.thisCase.currentActiveObjectives[0].queueElement.triggers.Contains(trig))
										{
											trig.Trigger(false);
										}
									}
									else
									{
										if (trig.triggerType == Objective.ObjectiveTriggerType.findFingerprints)
										{
											Human h = trig.evidence.writer;
											if (!(h != null))
											{
												continue;
											}
											if (InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => item.passedEvidence is EvidenceFingerprint && item.passedEvidence.writer == h))
											{
												trig.Trigger(false);
												continue;
											}
											using (List<Case>.Enumerator enumerator5 = CasePanelController.Instance.activeCases.GetEnumerator())
											{
												while (enumerator5.MoveNext())
												{
													Case case4 = enumerator5.Current;
													foreach (Case.CaseElement caseElement in case4.caseElements)
													{
														Evidence evidence = null;
														if (Toolbox.Instance.TryGetEvidence(caseElement.id, out evidence))
														{
															EvidenceFingerprint evidenceFingerprint = evidence as EvidenceFingerprint;
															if (evidenceFingerprint != null && evidenceFingerprint.writer == h)
															{
																trig.Trigger(false);
																break;
															}
														}
													}
													if (trig.triggered)
													{
														break;
													}
												}
												continue;
											}
										}
										if (trig.triggerType == Objective.ObjectiveTriggerType.findFingerprintsOnObject)
										{
											Human h = trig.evidence.writer;
											List<InfoWindow> list3 = InterfaceController.Instance.activeWindows.FindAll((InfoWindow item) => item.passedEvidence is EvidenceFingerprint && item.passedEvidence.writer == h);
											if (!(h != null) || list3 == null)
											{
												continue;
											}
											using (List<InfoWindow>.Enumerator enumerator7 = list3.GetEnumerator())
											{
												while (enumerator7.MoveNext())
												{
													InfoWindow infoWindow = enumerator7.Current;
													if (infoWindow.passedEvidence != null)
													{
														EvidenceFingerprint evidenceFingerprint2 = infoWindow.passedEvidence as EvidenceFingerprint;
														if (evidenceFingerprint2 != null && evidenceFingerprint2.interactable != null && evidenceFingerprint2.interactable.print != null && evidenceFingerprint2.interactable.print.Count > 0 && trig.interactable != null && evidenceFingerprint2.interactable.print[0].interactableID == trig.interactableID)
														{
															trig.Trigger(false);
															break;
														}
													}
												}
												continue;
											}
										}
										if (trig.triggerType == Objective.ObjectiveTriggerType.findFingerprintsAtLocation)
										{
											Human h = trig.evidence.writer;
											List<InfoWindow> list4 = InterfaceController.Instance.activeWindows.FindAll((InfoWindow item) => item.passedEvidence is EvidenceFingerprint && item.passedEvidence.writer == h);
											if (!(h != null) || list4 == null)
											{
												continue;
											}
											using (List<InfoWindow>.Enumerator enumerator7 = list4.GetEnumerator())
											{
												while (enumerator7.MoveNext())
												{
													InfoWindow infoWindow2 = enumerator7.Current;
													if (infoWindow2.passedEvidence != null)
													{
														EvidenceFingerprint evidenceFingerprint3 = infoWindow2.passedEvidence as EvidenceFingerprint;
														if (evidenceFingerprint3 != null && evidenceFingerprint3.interactable != null && evidenceFingerprint3.interactable.print != null && evidenceFingerprint3.interactable.print.Count > 0 && trig.interactable != null && evidenceFingerprint3.interactable.node.gameLocation == trig.gameLocation)
														{
															trig.Trigger(false);
															break;
														}
													}
												}
												continue;
											}
										}
										if (trig.triggerType == Objective.ObjectiveTriggerType.findSurveillanceWith)
										{
											Human h = trig.evidence.writer;
											if (!(h != null))
											{
												continue;
											}
											using (List<Interactable>.Enumerator enumerator4 = trig.gameLocation.securityCameras.GetEnumerator())
											{
												Predicate<SceneRecorder.ActorCapture> <>9__14;
												while (enumerator4.MoveNext())
												{
													Interactable interactable2 = enumerator4.Current;
													if (SceneCapture.Instance.currrentlyViewing != null && SceneCapture.Instance.currrentlyViewing.recorder.interactable == interactable2)
													{
														List<SceneRecorder.ActorCapture> aCap = SceneCapture.Instance.currrentlyViewing.aCap;
														Predicate<SceneRecorder.ActorCapture> predicate6;
														if ((predicate6 = <>9__14) == null)
														{
															predicate6 = (<>9__14 = ((SceneRecorder.ActorCapture item) => item.id == h.humanID));
														}
														if (aCap.Exists(predicate6))
														{
															trig.Trigger(false);
															break;
														}
													}
												}
												continue;
											}
										}
										if (trig.triggerType == Objective.ObjectiveTriggerType.surveillanceFlagFootage)
										{
											if (Player.Instance.computerInteractable == null || trig.evidence == null || !(Player.Instance.computerInteractable.controller != null) || !(Player.Instance.computerInteractable.controller.computer != null))
											{
												continue;
											}
											using (List<GameObject>.Enumerator enumerator8 = Player.Instance.computerInteractable.controller.computer.spawnedContent.GetEnumerator())
											{
												while (enumerator8.MoveNext())
												{
													GameObject gameObject = enumerator8.Current;
													SurveillanceApp component = gameObject.GetComponent<SurveillanceApp>();
													if (component != null && component.flaggedActor == trig.evidence.controller)
													{
														trig.Trigger(false);
														break;
													}
												}
												continue;
											}
										}
										if (trig.triggerType == Objective.ObjectiveTriggerType.accessCruncher)
										{
											if (trig.interactable != null && InteractionController.Instance.lockedInInteraction == trig.interactable)
											{
												if (trig.interactable.controller != null && trig.interactable.controller.computer != null && trig.interactable.controller.computer.loggedInAs != null)
												{
													trig.Trigger(false);
												}
											}
											else if (InteractionController.Instance.lockedInInteraction != null && Player.Instance.computerInteractable != null && trig.evidence != null && InteractionController.Instance.lockedInInteraction.controller != null && InteractionController.Instance.lockedInInteraction.controller.computer != null && InteractionController.Instance.lockedInInteraction.controller.computer.loggedInAs == trig.evidence.controller)
											{
												trig.Trigger(false);
											}
										}
										else if (trig.triggerType == Objective.ObjectiveTriggerType.accessAnyCruncher)
										{
											if (trig.interactable != null && InteractionController.Instance.lockedInInteraction == trig.interactable)
											{
												if (trig.interactable.controller != null && trig.interactable.controller.computer != null)
												{
													trig.Trigger(false);
												}
											}
											else if (InteractionController.Instance.lockedInInteraction != null && Player.Instance.computerInteractable != null && trig.evidence != null && InteractionController.Instance.lockedInInteraction.controller != null && InteractionController.Instance.lockedInInteraction.controller.computer != null)
											{
												trig.Trigger(false);
											}
										}
										else if (trig.triggerType == Objective.ObjectiveTriggerType.accessApp)
										{
											if (trig.interactable != null && InteractionController.Instance.lockedInInteraction == trig.interactable)
											{
												if (trig.interactable.controller != null && trig.interactable.controller.computer != null && trig.interactable.controller.computer.loggedInAs != null && trig.interactable.controller.computer.currentApp != null && trig.interactable.controller.computer.currentApp.name == trig.name)
												{
													trig.Trigger(false);
												}
											}
											else if (InteractionController.Instance.lockedInInteraction != null && Player.Instance.computerInteractable != null && trig.evidence != null && InteractionController.Instance.lockedInInteraction.controller != null && InteractionController.Instance.lockedInInteraction.controller.computer != null && InteractionController.Instance.lockedInInteraction.controller.computer.loggedInAs == trig.evidence.controller && InteractionController.Instance.lockedInInteraction.controller.computer.currentApp != null && InteractionController.Instance.lockedInInteraction.controller.computer.currentApp.name == trig.name)
											{
												trig.Trigger(false);
											}
										}
										else if (trig.triggerType == Objective.ObjectiveTriggerType.syncDiskInstallTutorial)
										{
											if (UpgradesController.Instance.isOpen && Player.Instance.hideInteractable != null && Player.Instance.hideInteractable.preset.specialCaseFlag == InteractablePreset.SpecialCase.syncBed)
											{
												trig.Trigger(false);
											}
										}
										else if (trig.triggerType == Objective.ObjectiveTriggerType.printVmail)
										{
											if (InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.passedEvidence is EvidencePrintedVmail && (item.passedEvidence as EvidencePrintedVmail).thread.treeID == trig.name) != null)
											{
												trig.Trigger(false);
											}
										}
										else if (trig.triggerType == Objective.ObjectiveTriggerType.notewriterWarned)
										{
											if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
											{
												ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
												if (chapterIntro != null && (chapterIntro.layLowGoal != null || (chapterIntro.noteWriter.isDead && InteractionController.Instance.currentLookingAtInteractable != null && InteractionController.Instance.currentLookingAtInteractable.interactable.isActor == chapterIntro.noteWriter)))
												{
													trig.Trigger(false);
												}
											}
										}
										else if (trig.triggerType != Objective.ObjectiveTriggerType.answerLEMCall)
										{
											if (trig.triggerType == Objective.ObjectiveTriggerType.playerHasApartment)
											{
												if (Player.Instance.home != null)
												{
													trig.Trigger(false);
												}
											}
											else if (trig.triggerType == Objective.ObjectiveTriggerType.successsfulSolve)
											{
												if (trig.interactable != null && trig.interactable.isActor != null && trig.interactable.isActor.ai.isConvicted)
												{
													trig.Trigger(false);
												}
											}
											else if (trig.triggerType == Objective.ObjectiveTriggerType.acquireLockpicks)
											{
												if (trig.interactable != null && trig.interactable.lockInteractable != null)
												{
													int lockpicksNeeded = Toolbox.Instance.GetLockpicksNeeded(trig.interactable.val);
													float num = (float)GameplayController.Instance.lockPicks / (float)lockpicksNeeded;
													this.SetProgress(num);
													if (GameplayController.Instance.lockPicks >= lockpicksNeeded)
													{
														trig.Trigger(false);
													}
												}
											}
											else if (trig.triggerType == Objective.ObjectiveTriggerType.plotRouteToCallInvolving)
											{
												if (MapController.Instance.playerRoute != null && MapController.Instance.playerRoute.end.gameLocation != null && trig.gameLocation != null && trig.gameLocation.building != null && trig.gameLocation.building.callLog.Exists((TelephoneController.PhoneCall item) => (item.toNS != null && item.fromNS != null && item.toNS.interactable.node.gameLocation == trig.gameLocation && MapController.Instance.playerRoute.end.gameLocation.building == item.fromNS.interactable.node.gameLocation.building) || (item.fromNS != null && item.toNS != null && item.fromNS.interactable.node.gameLocation == trig.gameLocation && MapController.Instance.playerRoute.end.gameLocation.building == item.toNS.interactable.node.gameLocation.building)))
												{
													trig.Trigger(false);
												}
											}
											else if (trig.triggerType == Objective.ObjectiveTriggerType.makeCall)
											{
												if (Player.Instance.answeringPhone != null && Player.Instance.answeringPhone.activeCall.Count > 0)
												{
													int num2 = -1;
													if (int.TryParse(trig.name, ref num2))
													{
														TelephoneController.CallSource callSource = null;
														if (TelephoneController.Instance.fakeTelephoneDictionary.ContainsKey(num2))
														{
															callSource = TelephoneController.Instance.fakeTelephoneDictionary[num2];
														}
														if (((callSource != null && Player.Instance.answeringPhone.activeCall[0].source == callSource) || Player.Instance.answeringPhone.activeCall[0].to == num2) && Player.Instance.answeringPhone.activeCall[0].state == TelephoneController.CallState.started)
														{
															trig.Trigger(false);
														}
													}
												}
											}
											else if (trig.triggerType == Objective.ObjectiveTriggerType.discoverParamour)
											{
												if (trig.evidence != null && trig.evidence.discoveryProgress.Contains(Evidence.Discovery.paramourDiscovery))
												{
													trig.Trigger(false);
												}
											}
											else if (trig.triggerType == Objective.ObjectiveTriggerType.checkRecentCalls)
											{
												if (trig.interactable != null && trig.interactable.recentCallCheck != 0f)
												{
													trig.Trigger(false);
												}
											}
											else if (trig.triggerType == Objective.ObjectiveTriggerType.raiseFirstPersonItem)
											{
												if (trig.interactable != null && BioScreenController.Instance.selectedSlot != null && BioScreenController.Instance.selectedSlot.interactableID > -1 && BioScreenController.Instance.selectedSlot.GetInteractable() == trig.interactable && FirstPersonItemController.Instance.isRaised)
												{
													trig.Trigger(false);
												}
											}
											else
											{
												if (trig.triggerType == Objective.ObjectiveTriggerType.hasFPSInventory)
												{
													using (List<FirstPersonItemController.InventorySlot>.Enumerator enumerator9 = FirstPersonItemController.Instance.slots.GetEnumerator())
													{
														while (enumerator9.MoveNext())
														{
															FirstPersonItemController.InventorySlot inventorySlot = enumerator9.Current;
															FirstPersonItem firstPersonItem = inventorySlot.GetFirstPersonItem();
															if (firstPersonItem != null && firstPersonItem.name.ToLower() == trig.name.ToLower())
															{
																trig.Trigger(false);
																break;
															}
														}
														continue;
													}
												}
												if (trig.triggerType == Objective.ObjectiveTriggerType.sideMissionMeetTriggered)
												{
													if (this.thisCase != null && this.thisCase.job != null)
													{
														SideJobStealBriefcase sideJobStealBriefcase = this.thisCase.job as SideJobStealBriefcase;
														if (sideJobStealBriefcase != null && sideJobStealBriefcase.triggeredMeet)
														{
															trig.Trigger(false);
														}
													}
												}
												else if (trig.triggerType == Objective.ObjectiveTriggerType.itemIsPlacedAtSecretLocation)
												{
													if (trig.interactable != null && trig.job != null && trig.job.secretLocationFurniture != 0 && trig.interactable.inInventory == null)
													{
														Vector3 vector = CityData.Instance.NodeToRealpos(trig.job.secretLocationNode);
														float num3 = Vector3.Distance(trig.interactable.wPos, vector);
														if (num3 <= 1.8f)
														{
															trig.Trigger(false);
														}
														else
														{
															Game.Log("Jobs: Item distance to postion: " + num3.ToString(), 2);
														}
													}
												}
												else if (trig.triggerType == Objective.ObjectiveTriggerType.itemIsNear)
												{
													if (trig.interactable != null && trig.interactable.inInventory == null && Vector3.Distance(trig.interactable.GetWorldPosition(true), trig.position) <= 0.5f)
													{
														trig.Trigger(false);
													}
												}
												else if (trig.triggerType == Objective.ObjectiveTriggerType.itemOfTypeIsNear)
												{
													if (trig.interactable != null)
													{
														NewNode newNode3;
														if (PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(trig.position), ref newNode3))
														{
															using (List<Interactable>.Enumerator enumerator4 = newNode3.interactables.GetEnumerator())
															{
																while (enumerator4.MoveNext())
																{
																	Interactable interactable3 = enumerator4.Current;
																	if (interactable3.preset == trig.interactable.preset && interactable3.inInventory == null && Vector3.Distance(interactable3.GetWorldPosition(true), trig.position) <= 0.5f)
																	{
																		trig.Trigger(false);
																		break;
																	}
																}
																continue;
															}
														}
														string text3 = "Objectives: Unable to get node at ";
														Vector3 position = trig.position;
														Game.Log(text3 + position.ToString(), 2);
													}
												}
												else if (trig.triggerType == Objective.ObjectiveTriggerType.destroyItem)
												{
													if (trig.interactable != null && (trig.interactable.rem || (trig.interactable.rPl && trig.interactable.inInventory == null)))
													{
														trig.Trigger(false);
													}
												}
												else if (trig.triggerType == Objective.ObjectiveTriggerType.ifValidRansomBriefcase)
												{
													MurderController.Murder currentM = MurderController.Instance.GetCurrentMurder();
													if (currentM != null && currentM.preset.caseType == MurderPreset.CaseType.kidnap && currentM.ransomSite != null)
													{
														Predicate<Interactable> <>9__16;
														for (int i = -1; i >= -2; i--)
														{
															if (currentM.ransomSite.floors.ContainsKey(i))
															{
																foreach (NewAddress newAddress in currentM.ransomSite.floors[i].addresses)
																{
																	foreach (NewRoom newRoom2 in newAddress.rooms)
																	{
																		int num4;
																		string text4;
																		if (!Player.Instance.IsTrespassing(newRoom2, out num4, out text4, true))
																		{
																			foreach (NewNode newNode4 in newRoom2.nodes)
																			{
																				List<Interactable> interactables = newNode4.interactables;
																				Predicate<Interactable> predicate7;
																				if ((predicate7 = <>9__16) == null)
																				{
																					predicate7 = (<>9__16 = ((Interactable item) => item.wo && item.preset.specialCaseFlag == InteractablePreset.SpecialCase.briefcase && (item.inInventory == null || item.inInventory == currentM.murderer)));
																				}
																				Interactable interactable4 = interactables.Find(predicate7);
																				if (interactable4 != null)
																				{
																					Game.Log("Murder: A valid ransom briefcase is found at " + interactable4.GetWorldPosition(true).ToString(), 2);
																					trig.Trigger(false);
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
												else if (trig.triggerType == Objective.ObjectiveTriggerType.ifNoValidRansomBriefcase)
												{
													MurderController.Murder currentM = MurderController.Instance.GetCurrentMurder();
													bool flag = false;
													if (currentM != null && currentM.preset.caseType == MurderPreset.CaseType.kidnap && currentM.ransomSite != null)
													{
														Predicate<Interactable> <>9__17;
														for (int j = -1; j >= -2; j--)
														{
															if (currentM.ransomSite.floors.ContainsKey(j))
															{
																foreach (NewAddress newAddress2 in currentM.ransomSite.floors[j].addresses)
																{
																	foreach (NewRoom newRoom3 in newAddress2.rooms)
																	{
																		int num4;
																		string text4;
																		if (!Player.Instance.IsTrespassing(newRoom3, out num4, out text4, true))
																		{
																			foreach (NewNode newNode5 in newRoom3.nodes)
																			{
																				List<Interactable> interactables2 = newNode5.interactables;
																				Predicate<Interactable> predicate8;
																				if ((predicate8 = <>9__17) == null)
																				{
																					predicate8 = (<>9__17 = ((Interactable item) => item.wo && item.preset.specialCaseFlag == InteractablePreset.SpecialCase.briefcase && (item.inInventory == null || item.inInventory == currentM.murderer)));
																				}
																				if (interactables2.Find(predicate8) != null)
																				{
																					flag = true;
																					break;
																				}
																			}
																			if (flag)
																			{
																				break;
																			}
																		}
																	}
																	if (flag)
																	{
																		break;
																	}
																}
															}
															if (flag)
															{
																break;
															}
														}
													}
													if (!flag)
													{
														Game.Log("Murder: No valid briefcase is found at ransom site", 2);
														trig.Trigger(false);
													}
												}
												else if (trig.triggerType == Objective.ObjectiveTriggerType.kidnapperHasValidBriefcase)
												{
													MurderController.Murder currentMurder = MurderController.Instance.GetCurrentMurder();
													if (currentMurder != null && currentMurder.preset.caseType == MurderPreset.CaseType.kidnap && currentMurder.murderer != null)
													{
														if (currentMurder.murderer.inventory.Find((Interactable item) => item.wo && item.preset.specialCaseFlag == InteractablePreset.SpecialCase.briefcase) != null)
														{
															Game.Log("Murder: A valid ransom briefcase is found in the inventory of " + currentMurder.murderer.GetCitizenName(), 2);
															trig.Trigger(false);
														}
													}
												}
												else if (trig.triggerType == Objective.ObjectiveTriggerType.victimIsFreed)
												{
													MurderController.Murder currentMurder2 = MurderController.Instance.GetCurrentMurder();
													if (currentMurder2 != null && currentMurder2.preset.caseType == MurderPreset.CaseType.kidnap && currentMurder2.victim != null && !currentMurder2.victim.ai.restrained && !currentMurder2.victim.ai.ko && !currentMurder2.victim.isDead)
													{
														Game.Log("Murder: The current kidnap victim has been freed!", 2);
														trig.Trigger(false);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (this.progress >= 1f && !this.isComplete)
			{
				Game.Log("Objective: Completing objective...", 2);
				this.Complete();
			}
		}
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x000CE7E0 File Offset: 0x000CC9E0
	public void SetProgress(float newProgress)
	{
		if (newProgress != this.progress)
		{
			this.progress = newProgress;
			if (this.OnProgressChange != null)
			{
				this.OnProgressChange();
			}
		}
	}

	// Token: 0x04001083 RID: 4227
	public SpeechController.QueueElement queueElement;

	// Token: 0x04001084 RID: 4228
	public string name = "New Objective";

	// Token: 0x04001085 RID: 4229
	public float progress;

	// Token: 0x04001086 RID: 4230
	public bool isComplete;

	// Token: 0x04001087 RID: 4231
	public bool isCancelled;

	// Token: 0x04001088 RID: 4232
	public Objective.DisplayPhase dispPhase;

	// Token: 0x04001089 RID: 4233
	private float fadeInProgress;

	// Token: 0x0400108A RID: 4234
	private float displayProgress;

	// Token: 0x0400108B RID: 4235
	private float displayTime;

	// Token: 0x0400108C RID: 4236
	private float crouchPromtTimer;

	// Token: 0x0400108D RID: 4237
	[NonSerialized]
	public Case thisCase;

	// Token: 0x0400108E RID: 4238
	[NonSerialized]
	public GameObject objectiveListItem;

	// Token: 0x0400108F RID: 4239
	[NonSerialized]
	public RectTransform objectiveListRect;

	// Token: 0x04001090 RID: 4240
	[NonSerialized]
	public ChecklistButtonController objectiveList;

	// Token: 0x04001091 RID: 4241
	[NonSerialized]
	private bool displayPointer;

	// Token: 0x04001092 RID: 4242
	[NonSerialized]
	public RectTransform pointerUIObject;

	// Token: 0x04001093 RID: 4243
	[NonSerialized]
	public InterfaceController.AwarenessIcon awarenessIcon;

	// Token: 0x04001094 RID: 4244
	[NonSerialized]
	public UIPointerController pointer;

	// Token: 0x04001095 RID: 4245
	[NonSerialized]
	public Sprite sprite;

	// Token: 0x04001096 RID: 4246
	[NonSerialized]
	public bool isSetup;

	// Token: 0x04001097 RID: 4247
	[NonSerialized]
	public List<Objective.ObjectiveTrigger> appliedProgress;

	// Token: 0x04001098 RID: 4248
	[NonSerialized]
	public bool clearedForAnimation;

	// Token: 0x04001099 RID: 4249
	public Objective.ObjectiveTrigger objectiveAddOn;

	// Token: 0x0400109A RID: 4250
	public float progressAdd;

	// Token: 0x02000264 RID: 612
	public enum DisplayPhase
	{
		// Token: 0x0400109E RID: 4254
		preDisplay,
		// Token: 0x0400109F RID: 4255
		fadeInMainText,
		// Token: 0x040010A0 RID: 4256
		displayMainText,
		// Token: 0x040010A1 RID: 4257
		removeText,
		// Token: 0x040010A2 RID: 4258
		displayingList,
		// Token: 0x040010A3 RID: 4259
		waitForComplete
	}

	// Token: 0x02000265 RID: 613
	public enum OnCompleteAction
	{
		// Token: 0x040010A5 RID: 4261
		nextChapterPart,
		// Token: 0x040010A6 RID: 4262
		nextPartWhenAllCompleted,
		// Token: 0x040010A7 RID: 4263
		specificChapterByString,
		// Token: 0x040010A8 RID: 4264
		specificChapterWhenAllCompleted,
		// Token: 0x040010A9 RID: 4265
		nothing,
		// Token: 0x040010AA RID: 4266
		invokeFunction,
		// Token: 0x040010AB RID: 4267
		triggerSideJobFunction,
		// Token: 0x040010AC RID: 4268
		triggerSideJobHandIn,
		// Token: 0x040010AD RID: 4269
		nextSideJobPhase,
		// Token: 0x040010AE RID: 4270
		submitSideJob,
		// Token: 0x040010AF RID: 4271
		completeCoverUp,
		// Token: 0x040010B0 RID: 4272
		coverUpTips,
		// Token: 0x040010B1 RID: 4273
		triggerRansomDelivery,
		// Token: 0x040010B2 RID: 4274
		triggerRansomCollection,
		// Token: 0x040010B3 RID: 4275
		triggerKidnapperRansomCollection,
		// Token: 0x040010B4 RID: 4276
		kidnapperCollectedRansom,
		// Token: 0x040010B5 RID: 4277
		kidnapperVictimFreed
	}

	// Token: 0x02000266 RID: 614
	[Serializable]
	public class ObjectiveTrigger
	{
		// Token: 0x06000E37 RID: 3639 RVA: 0x000CE830 File Offset: 0x000CCA30
		public ObjectiveTrigger(Objective.ObjectiveTriggerType newType, string newName, bool newForceProgressAmount = false, float newProgressAdd = 0f, NewRoom newRoom = null, Interactable newInteractable = null, Evidence newEvidence = null, NewNode newNode = null, NewDoor newDoor = null, NewGameLocation newGameLocation = null, SideJob newJob = null, string newHighlightAction = "", bool newOrTrigger = false, Vector3 newPosition = default(Vector3))
		{
			this.triggerType = newType;
			this.name = newName;
			this.progressAdd = newProgressAdd;
			this.hightlightAction = newHighlightAction;
			this.forceProgressAmount = newForceProgressAmount;
			this.orTrigger = newOrTrigger;
			this.room = newRoom;
			if (this.room != null)
			{
				this.roomID = this.room.roomID;
			}
			this.interactable = newInteractable;
			if (this.interactable != null)
			{
				this.interactableID = this.interactable.id;
			}
			this.evidence = newEvidence;
			if (this.evidence != null)
			{
				this.evidenceID = this.evidence.evID;
			}
			this.node = newNode;
			if (this.node != null)
			{
				this.nodeCoord = this.node.nodeCoord;
			}
			this.door = newDoor;
			if (this.door != null)
			{
				this.doorID = this.door.wall.id;
			}
			this.job = newJob;
			if (this.job != null)
			{
				this.jobID = this.job.jobID;
			}
			this.position = newPosition;
			this.gameLocation = newGameLocation;
			if (this.gameLocation != null)
			{
				if (this.gameLocation.thisAsAddress != null)
				{
					this.addressID = this.gameLocation.thisAsAddress.id;
				}
				else if (this.gameLocation.thisAsStreet != null)
				{
					this.streetID = this.gameLocation.thisAsStreet.streetID;
				}
			}
			this.addedToObjectives = new List<Objective>();
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x000CE9F0 File Offset: 0x000CCBF0
		public void SetupNonSerialized()
		{
			if (this.roomID > -1 && this.room == null && !CityData.Instance.roomDictionary.TryGetValue(this.roomID, ref this.room))
			{
				Game.LogError("Unable to get room for objective: " + this.roomID.ToString(), 2);
			}
			if (this.interactableID > 0 && this.interactable == null && !CityData.Instance.savableInteractableDictionary.TryGetValue(this.interactableID, ref this.interactable))
			{
				Game.LogError("Unable to get interactable for objective: " + this.interactableID.ToString(), 2);
			}
			if (this.evidenceID != null && this.evidenceID.Length > 0 && this.evidence == null && !Toolbox.Instance.TryGetEvidence(this.evidenceID, out this.evidence))
			{
				Game.LogError("Unable to get evidence for objective: " + this.evidenceID, 2);
			}
			PathFinder.Instance.nodeMap.TryGetValue(this.nodeCoord, ref this.node);
			if (this.doorID > -1 && this.door == null && !CityData.Instance.doorDictionary.TryGetValue(this.doorID, ref this.door))
			{
				Game.LogError("Unable to get door for objective: " + this.doorID.ToString(), 2);
			}
			if (this.addressID > -1 && this.gameLocation == null)
			{
				NewAddress newAddress = null;
				if (!CityData.Instance.addressDictionary.TryGetValue(this.addressID, ref newAddress))
				{
					Game.LogError("Unable to get address for objective: " + this.addressID.ToString(), 2);
				}
				this.gameLocation = newAddress;
			}
			if (this.streetID > -1 && this.gameLocation == null)
			{
				StreetController streetController = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == this.streetID);
				this.gameLocation = streetController;
				if (streetController == null)
				{
					Game.LogError("Unable to get street for objective: " + this.streetID.ToString(), 2);
				}
			}
			if (this.jobID > -1 && this.job == null && !SideJobController.Instance.allJobsDictionary.TryGetValue(this.jobID, ref this.job))
			{
				Game.LogError("Unable to get side job for objective: " + this.jobID.ToString(), 2);
			}
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x000CEC4C File Offset: 0x000CCE4C
		public void Trigger(bool onSetup = false)
		{
			this.triggered = true;
			foreach (Objective objective in this.addedToObjectives)
			{
				if (objective.appliedProgress == null)
				{
					objective.appliedProgress = new List<Objective.ObjectiveTrigger>();
				}
				if (!objective.appliedProgress.Contains(this))
				{
					if (this.orTrigger)
					{
						if (objective.appliedProgress.Exists((Objective.ObjectiveTrigger item) => item.orTrigger))
						{
							continue;
						}
					}
					float num = this.progressAdd;
					if (!this.forceProgressAmount)
					{
						List<Objective.ObjectiveTrigger> list = new List<Objective.ObjectiveTrigger>();
						List<Objective.ObjectiveTrigger> list2 = new List<Objective.ObjectiveTrigger>();
						float num2 = 1f;
						int num3 = 0;
						foreach (Objective.ObjectiveTrigger objectiveTrigger in objective.queueElement.triggers)
						{
							if (objectiveTrigger.orTrigger)
							{
								list2.Add(objectiveTrigger);
								if (list2.Count == 1)
								{
									num3++;
									if (!objectiveTrigger.forceProgressAmount)
									{
										list.Add(objectiveTrigger);
									}
									else
									{
										num2 -= objectiveTrigger.progressAdd;
									}
								}
							}
							else
							{
								num3++;
								if (!objectiveTrigger.forceProgressAmount)
								{
									list.Add(objectiveTrigger);
								}
								else
								{
									num2 -= objectiveTrigger.progressAdd;
								}
							}
						}
						num = num2 / (float)num3;
					}
					objective.progress += (float)Mathf.CeilToInt(num * 100f) / 100f;
					objective.progress = Mathf.Clamp01(objective.progress);
					Game.Log(string.Concat(new string[]
					{
						"Objective: + ",
						num.ToString(),
						" Objective progress to ",
						objective.queueElement.entryRef,
						" with trigger count of ",
						objective.queueElement.triggers.Count.ToString(),
						", total is ",
						objective.progress.ToString(),
						"..."
					}), 2);
					objective.appliedProgress.Add(this);
					if (objective.OnProgressChange != null)
					{
						objective.OnProgressChange();
					}
					if (objective.progress >= 1f)
					{
						if (onSetup)
						{
							Game.Log("Objective: Setting objective " + objective.queueElement.entryRef + " to silent (objective triggered on setup - most likely already completed)", 2);
							objective.queueElement.isSilent = true;
						}
						else
						{
							try
							{
								objective.Complete();
								if (objective.OnComplete != null)
								{
									objective.OnComplete();
								}
							}
							catch
							{
								Game.Log("Completion trigger of an objective failed: This could be because it's triggering before loading the story configuration...", 2);
							}
						}
					}
				}
			}
		}

		// Token: 0x040010B6 RID: 4278
		public Objective.ObjectiveTriggerType triggerType;

		// Token: 0x040010B7 RID: 4279
		public bool forceProgressAmount;

		// Token: 0x040010B8 RID: 4280
		public float progressAdd;

		// Token: 0x040010B9 RID: 4281
		public bool triggered;

		// Token: 0x040010BA RID: 4282
		public string name;

		// Token: 0x040010BB RID: 4283
		public string hightlightAction;

		// Token: 0x040010BC RID: 4284
		public bool orTrigger;

		// Token: 0x040010BD RID: 4285
		public int roomID = -1;

		// Token: 0x040010BE RID: 4286
		public int interactableID = -1;

		// Token: 0x040010BF RID: 4287
		public string evidenceID;

		// Token: 0x040010C0 RID: 4288
		public Vector3Int nodeCoord;

		// Token: 0x040010C1 RID: 4289
		public int doorID = -1;

		// Token: 0x040010C2 RID: 4290
		public int addressID = -1;

		// Token: 0x040010C3 RID: 4291
		public int streetID = -1;

		// Token: 0x040010C4 RID: 4292
		public int jobID = -1;

		// Token: 0x040010C5 RID: 4293
		public Vector3 position;

		// Token: 0x040010C6 RID: 4294
		[NonSerialized]
		public NewRoom room;

		// Token: 0x040010C7 RID: 4295
		[NonSerialized]
		public Interactable interactable;

		// Token: 0x040010C8 RID: 4296
		[NonSerialized]
		public Evidence evidence;

		// Token: 0x040010C9 RID: 4297
		[NonSerialized]
		public NewNode node;

		// Token: 0x040010CA RID: 4298
		[NonSerialized]
		public NewDoor door;

		// Token: 0x040010CB RID: 4299
		[NonSerialized]
		public NewGameLocation gameLocation;

		// Token: 0x040010CC RID: 4300
		[NonSerialized]
		public SideJob job;

		// Token: 0x040010CD RID: 4301
		[NonSerialized]
		public List<Objective> addedToObjectives;
	}

	// Token: 0x02000268 RID: 616
	public enum ObjectiveTriggerType
	{
		// Token: 0x040010D1 RID: 4305
		playerAction,
		// Token: 0x040010D2 RID: 4306
		switchStateTrue,
		// Token: 0x040010D3 RID: 4307
		switchStateFalse,
		// Token: 0x040010D4 RID: 4308
		roomLightOn,
		// Token: 0x040010D5 RID: 4309
		inspectInteractable,
		// Token: 0x040010D6 RID: 4310
		evidencePinned,
		// Token: 0x040010D7 RID: 4311
		goToNode,
		// Token: 0x040010D8 RID: 4312
		keyInventory,
		// Token: 0x040010D9 RID: 4313
		knowDoorLockedStatus,
		// Token: 0x040010DA RID: 4314
		goToAddress,
		// Token: 0x040010DB RID: 4315
		goToRoom,
		// Token: 0x040010DC RID: 4316
		playerHidden,
		// Token: 0x040010DD RID: 4317
		escapeGameLocation,
		// Token: 0x040010DE RID: 4318
		escapeBuilding,
		// Token: 0x040010DF RID: 4319
		answerPhone,
		// Token: 0x040010E0 RID: 4320
		openEvidence,
		// Token: 0x040010E1 RID: 4321
		plotRoute,
		// Token: 0x040010E2 RID: 4322
		gameUnpaused,
		// Token: 0x040010E3 RID: 4323
		unlockDoor,
		// Token: 0x040010E4 RID: 4324
		goToPublicFacingAddress,
		// Token: 0x040010E5 RID: 4325
		answerPhoneAndEndCall,
		// Token: 0x040010E6 RID: 4326
		switchStateTrueForType,
		// Token: 0x040010E7 RID: 4327
		linkImageWithName,
		// Token: 0x040010E8 RID: 4328
		viewInteractable,
		// Token: 0x040010E9 RID: 4329
		noMoreObjectives,
		// Token: 0x040010EA RID: 4330
		findFingerprints,
		// Token: 0x040010EB RID: 4331
		findSurveillanceWith,
		// Token: 0x040010EC RID: 4332
		findFingerprintsOnObject,
		// Token: 0x040010ED RID: 4333
		accessCruncher,
		// Token: 0x040010EE RID: 4334
		printVmail,
		// Token: 0x040010EF RID: 4335
		successsfulSolve,
		// Token: 0x040010F0 RID: 4336
		makeCall,
		// Token: 0x040010F1 RID: 4337
		discoverParamour,
		// Token: 0x040010F2 RID: 4338
		onCompleteJob,
		// Token: 0x040010F3 RID: 4339
		identifyFinerprints,
		// Token: 0x040010F4 RID: 4340
		interactableRemoved,
		// Token: 0x040010F5 RID: 4341
		checkRecentCalls,
		// Token: 0x040010F6 RID: 4342
		acquireLockpicks,
		// Token: 0x040010F7 RID: 4343
		unlockInteractable,
		// Token: 0x040010F8 RID: 4344
		gamePaused,
		// Token: 0x040010F9 RID: 4345
		evidenceOpenAndDisplayed,
		// Token: 0x040010FA RID: 4346
		collectHandIn,
		// Token: 0x040010FB RID: 4347
		viewHandIn,
		// Token: 0x040010FC RID: 4348
		submitCase,
		// Token: 0x040010FD RID: 4349
		waitForCaseProcessing,
		// Token: 0x040010FE RID: 4350
		surveillanceFlagFootage,
		// Token: 0x040010FF RID: 4351
		findFingerprintsAtLocation,
		// Token: 0x04001100 RID: 4352
		plotRouteToCallInvolving,
		// Token: 0x04001101 RID: 4353
		notewriterWarned,
		// Token: 0x04001102 RID: 4354
		exploreCrimeScene,
		// Token: 0x04001103 RID: 4355
		nothing,
		// Token: 0x04001104 RID: 4356
		playerHasApartment,
		// Token: 0x04001105 RID: 4357
		answerLEMCall,
		// Token: 0x04001106 RID: 4358
		discoverEvidence,
		// Token: 0x04001107 RID: 4359
		accessApp,
		// Token: 0x04001108 RID: 4360
		syncDiskInstallTutorial,
		// Token: 0x04001109 RID: 4361
		onDialogSuccess,
		// Token: 0x0400110A RID: 4362
		raiseFirstPersonItem,
		// Token: 0x0400110B RID: 4363
		hasFPSInventory,
		// Token: 0x0400110C RID: 4364
		sideMissionMeetTriggered,
		// Token: 0x0400110D RID: 4365
		itemInInventory,
		// Token: 0x0400110E RID: 4366
		itemIsPlacedAtSecretLocation,
		// Token: 0x0400110F RID: 4367
		destroyItem,
		// Token: 0x04001110 RID: 4368
		itemIsNear,
		// Token: 0x04001111 RID: 4369
		playerActionNobodyHome,
		// Token: 0x04001112 RID: 4370
		accessAnyCruncher,
		// Token: 0x04001113 RID: 4371
		itemOfTypeIsNear,
		// Token: 0x04001114 RID: 4372
		disposeOfBody,
		// Token: 0x04001115 RID: 4373
		ifValidRansomBriefcase,
		// Token: 0x04001116 RID: 4374
		ifNoValidRansomBriefcase,
		// Token: 0x04001117 RID: 4375
		kidnapperHasValidBriefcase,
		// Token: 0x04001118 RID: 4376
		victimIsFreed
	}

	// Token: 0x02000269 RID: 617
	// (Invoke) Token: 0x06000E3F RID: 3647
	public delegate void ProgressChange();

	// Token: 0x0200026A RID: 618
	// (Invoke) Token: 0x06000E43 RID: 3651
	public delegate void Completed();
}
