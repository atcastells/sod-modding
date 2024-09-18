using System;
using System.Collections.Generic;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200023E RID: 574
[Serializable]
public class Case
{
	// Token: 0x06000CF1 RID: 3313 RVA: 0x000B8240 File Offset: 0x000B6440
	public void AddNewStringColour(Evidence.FactLink link, InterfaceControls.EvidenceColours col)
	{
		bool flag = false;
		for (int i = 0; i < this.stringColours.Count; i++)
		{
			Case.StringColours stringColours = this.stringColours[i];
			if (stringColours.fromEv == link.thisEvidence.evID)
			{
				bool flag2 = true;
				foreach (Evidence evidence in link.destinationEvidence)
				{
					if (!stringColours.toEv.Contains(evidence.evID))
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					bool flag3 = false;
					foreach (Evidence.DataKey dataKey in link.thisKeys)
					{
						if (stringColours.fromDK.Contains(dataKey))
						{
							flag3 = true;
							break;
						}
					}
					if (flag3)
					{
						flag3 = false;
						foreach (Evidence.DataKey dataKey2 in link.destinationKeys)
						{
							if (stringColours.toDK.Contains(dataKey2))
							{
								flag3 = true;
								break;
							}
						}
						if (flag3)
						{
							Game.Log("Interface: Existing string colour found, changing to " + col.ToString(), 2);
							flag = true;
							stringColours.colIndex = (int)col;
							if (col == InterfaceControls.EvidenceColours.red)
							{
								this.stringColours.RemoveAt(i);
								i--;
							}
						}
					}
				}
			}
		}
		if (!flag)
		{
			Case.StringColours stringColours2 = new Case.StringColours();
			stringColours2.fromEv = link.thisEvidence.evID;
			stringColours2.toEv = new List<string>();
			foreach (Evidence evidence2 in link.destinationEvidence)
			{
				stringColours2.toEv.Add(evidence2.evID);
			}
			stringColours2.fromDK = link.thisKeys;
			stringColours2.toDK = link.destinationKeys;
			stringColours2.colIndex = (int)col;
			this.stringColours.Add(stringColours2);
			Game.Log("Interface: Created a new string colour: " + col.ToString(), 2);
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x000B84A8 File Offset: 0x000B66A8
	public void SetHidden(Fact fact, bool val)
	{
		string identifier = fact.GetIdentifier();
		if (this.hiddenConnections.Contains(identifier) && !val)
		{
			Game.Log("Interface: Removing hidden connection: " + identifier, 2);
			this.hiddenConnections.Remove(identifier);
		}
		else if (!this.hiddenConnections.Contains(identifier) && val)
		{
			Game.Log("Interface: Adding hidden connection: " + identifier, 2);
			this.hiddenConnections.Add(identifier);
		}
		foreach (StringController stringController in CasePanelController.Instance.spawnedStrings.FindAll((StringController item) => item.connection.facts.Contains(fact)))
		{
			stringController.UpdateHidden();
		}
		Predicate<FactButtonController> <>9__1;
		foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
		{
			if (infoWindow.item != null)
			{
				List<FactButtonController> spawnedFactButtons = infoWindow.item.spawnedFactButtons;
				Predicate<FactButtonController> predicate;
				if ((predicate = <>9__1) == null)
				{
					predicate = (<>9__1 = ((FactButtonController item) => item.fact == fact));
				}
				foreach (FactButtonController factButtonController in spawnedFactButtons.FindAll(predicate))
				{
					factButtonController.VisualUpdate();
				}
			}
		}
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x000B8648 File Offset: 0x000B6848
	public void ToggleHidden(Fact fact)
	{
		string identifier = fact.GetIdentifier();
		Game.Log("Interface: Toggle hidden connection: " + identifier, 2);
		if (this.hiddenConnections.Contains(identifier))
		{
			Game.Log("Interface: Remove hidden connection: " + identifier, 2);
			this.hiddenConnections.Remove(identifier);
		}
		else
		{
			Game.Log("Interface: Add hidden connection: " + identifier, 2);
			this.hiddenConnections.Add(identifier);
		}
		foreach (StringController stringController in CasePanelController.Instance.spawnedStrings.FindAll((StringController item) => item.connection.facts.Contains(fact)))
		{
			stringController.UpdateHidden();
		}
		Predicate<FactButtonController> <>9__1;
		foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
		{
			if (infoWindow.item != null)
			{
				List<FactButtonController> spawnedFactButtons = infoWindow.item.spawnedFactButtons;
				Predicate<FactButtonController> predicate;
				if ((predicate = <>9__1) == null)
				{
					predicate = (<>9__1 = ((FactButtonController item) => item.fact == fact));
				}
				foreach (FactButtonController factButtonController in spawnedFactButtons.FindAll(predicate))
				{
					factButtonController.VisualUpdate();
				}
			}
		}
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x000B87E4 File Offset: 0x000B69E4
	public void SetStatus(Case.CaseStatus newStatus, bool cancelObjectives = true)
	{
		this.caseStatus = newStatus;
		if (this.caseStatus == Case.CaseStatus.handInNotCollected)
		{
			if (this.caseType == Case.CaseType.mainStory || this.caseType == Case.CaseType.murder || this.caseType == Case.CaseType.retirement)
			{
				Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.collectHandIn, "Collect Hand In", false, 1f, null, null, null, null, null, null, null, "", false, default(Vector3));
				NewBuilding newBuilding = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Find((NewBuilding item) => item.preset == RoutineControls.Instance.cityHall);
				Interactable interactable = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.getHandIn, newBuilding.mainEntrance.node.room, Player.Instance, AIActionPreset.FindSetting.allAreas, false, null, null, newBuilding, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
				Vector3 pointerPosition = Vector3.zero;
				if (interactable != null)
				{
					pointerPosition = interactable.GetWorldPosition(true);
				}
				else
				{
					Game.LogError("Could not find case hand in tray object!", 2);
				}
				if (this.caseType == Case.CaseType.retirement)
				{
					this.AddObjective("Collect Retirement Form", trigger, true, pointerPosition, InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, null, true, false, true);
				}
				else
				{
					this.AddObjective("Collect Hand In", trigger, true, pointerPosition, InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, null, true, false, true);
				}
			}
		}
		else
		{
			if (this.caseStatus == Case.CaseStatus.handInCollected)
			{
				if (this.caseType == Case.CaseType.mainStory && (!(ChapterController.Instance != null) || ChapterController.Instance.currentPart <= 30))
				{
					goto IL_518;
				}
				if (this.job != null)
				{
					this.job.DisplayResolveObjectivesCheck();
				}
				else
				{
					foreach (Case.ResolveQuestion resolveQuestion in this.resolveQuestions)
					{
						if (resolveQuestion.displayObjective && resolveQuestion.inputType != Case.InputType.objective)
						{
							Game.Log("Jobs: Adding auto objective (resolve question): " + resolveQuestion.name + "...", 2);
							Objective.ObjectiveTrigger objectiveTrigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.onCompleteJob, "complete", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
							string entryRef = resolveQuestion.name;
							Objective.ObjectiveTrigger trigger2 = objectiveTrigger;
							bool usePointer = false;
							InterfaceControls.Icon icon = resolveQuestion.icon;
							SideJob jobRef = this.job;
							this.AddObjective(entryRef, trigger2, usePointer, default(Vector3), icon, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, jobRef, false, false, true);
							resolveQuestion.OnProgressChange += this.OnQuestionProgressChange;
						}
					}
				}
				if (this.caseType != Case.CaseType.murder || !(this.name == Strings.Get("ui.interface", "New Murder Case", Strings.Casing.asIs, false, false, false, null)))
				{
					goto IL_518;
				}
				using (List<MurderController.Murder>.Enumerator enumerator2 = MurderController.Instance.activeMurders.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						MurderController.Murder murder = enumerator2.Current;
						if (murder.murderer != null)
						{
							string monkier = murder.GetMonkier();
							if (monkier.Length > 0)
							{
								string newMessage = Strings.Get("ui.gamemessage", "The case of the ", Strings.Casing.asIs, false, false, false, null) + monkier;
								InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, newMessage, InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
								InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, string.Concat(new string[]
								{
									SessionData.Instance.CurrentTimeString(false, true),
									", ",
									SessionData.Instance.LongDateString(SessionData.Instance.gameTime, true, true, true, true, true, false, false, true),
									", ",
									Player.Instance.currentGameLocation.name
								}), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
								this.name = monkier;
								CasePanelController.Instance.UpdateCaseControls();
								break;
							}
						}
					}
					goto IL_518;
				}
			}
			if (this.caseStatus == Case.CaseStatus.submitted)
			{
				if (ResolveController.Instance != null)
				{
					ResolveController.Instance.wcc.window.CloseWindow(false);
				}
				if (this.caseType == Case.CaseType.murder || this.caseType == Case.CaseType.mainStory)
				{
					MurderController.Instance.UpdateCorrectResolveAnswers();
				}
				else if (this.caseType == Case.CaseType.sideJob)
				{
					this.job.UpdateResolveAnswers();
				}
				foreach (Case.ResolveQuestion resolveQuestion2 in this.resolveQuestions)
				{
					resolveQuestion2.UpdateValid(this);
					resolveQuestion2.UpdateCorrect(this, true);
				}
				if (!this.currentActiveObjectives.Exists((Objective item) => item.queueElement.entryRef == "case processing" && !item.isCancelled))
				{
					Objective.ObjectiveTrigger trigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.waitForCaseProcessing, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
					this.AddObjective("case processing", trigger3, false, default(Vector3), InterfaceControls.Icon.time, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, null, false, true, true);
				}
			}
			else if (this.caseStatus == Case.CaseStatus.closable && cancelObjectives)
			{
				this.ClearAllObjectives();
			}
		}
		IL_518:
		if (CasePanelController.Instance.activeCase == this)
		{
			CasePanelController.Instance.UpdateCloseCaseButton();
			if (ResolveController.Instance != null)
			{
				ResolveController.Instance.UpdateResolveFields();
			}
		}
		CasePanelController.Instance.UpdateResolveNotifications();
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x000B8D90 File Offset: 0x000B6F90
	public NewGameLocation GetGameLocationFromQuestionInput(Case.ResolveQuestion question)
	{
		NewGameLocation result = null;
		if (this.job != null)
		{
			result = this.job.GetGameLocationFromQuestionInput(question);
		}
		else
		{
			Case.ResolveQuestion addressQuestion = this.resolveQuestions.Find((Case.ResolveQuestion item) => item.inputType == Case.InputType.location);
			if (addressQuestion != null && addressQuestion.input != null && addressQuestion.input.Length > 0)
			{
				result = CityData.Instance.gameLocationDirectory.Find((NewGameLocation item) => item.name.ToLower() == addressQuestion.input.ToLower());
			}
			else
			{
				Game.Log("Jobs: Unable to get a valid address for address question input", 2);
			}
		}
		return result;
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x000B8E44 File Offset: 0x000B7044
	public Human GetCitizenFromQuestionInput(Case.ResolveQuestion question)
	{
		Human result = null;
		if (this.job != null)
		{
			result = this.job.GetCitizenFromQuestionInput(question);
		}
		else
		{
			Case.ResolveQuestion citizenQuestion = this.resolveQuestions.Find((Case.ResolveQuestion item) => item.inputType == Case.InputType.citizen);
			if (citizenQuestion != null && citizenQuestion.input != null && citizenQuestion.input.Length > 0)
			{
				result = CityData.Instance.citizenDirectory.Find((Citizen item) => item.GetCitizenName().ToLower() == citizenQuestion.input.ToLower());
			}
			else
			{
				Game.Log("Jobs: Unable to get a valid citizen for name question input", 2);
			}
		}
		return result;
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x000B8EF8 File Offset: 0x000B70F8
	public void OnQuestionProgressChange(Case.ResolveQuestion question)
	{
		Objective objective = this.currentActiveObjectives.Find((Objective item) => item.queueElement.entryRef == question.name);
		if (objective != null)
		{
			objective.SetProgress(question.progress);
		}
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x000B8F40 File Offset: 0x000B7140
	public void Resolve()
	{
		Game.Log("Resolving case " + this.name + "...", 2);
		int num = 0;
		int num2 = 0;
		this.isSolved = true;
		this.questionsRank = 0f;
		this.rank = Case.CaseRank.C;
		foreach (Case.ResolveQuestion resolveQuestion in this.resolveQuestions)
		{
			if (resolveQuestion.isCorrect)
			{
				num += resolveQuestion.reward;
				this.questionsRank += 1f;
			}
			else if (!resolveQuestion.isOptional)
			{
				num2 += resolveQuestion.penalty;
				this.isSolved = false;
				this.rank = Case.CaseRank.unSolved;
			}
		}
		this.questionsRank /= (float)this.resolveQuestions.Count;
		this.victimsRank = Mathf.InverseLerp((float)GameplayControls.Instance.worstCaseVictimCount, (float)GameplayControls.Instance.bestCaseVictimCount, (float)MurderController.Instance.activeMurders.Count);
		float num3 = (this.questionsRank + this.victimsRank) / 2f;
		if (num3 >= 2f)
		{
			this.rank = Case.CaseRank.super;
		}
		else
		{
			this.rank = Case.CaseRank.unSolved - Mathf.RoundToInt(num3 * 4f);
		}
		Game.Log(string.Concat(new string[]
		{
			"Case resolve score: Questions rank: ",
			this.questionsRank.ToString(),
			", victims Rank: ",
			this.victimsRank.ToString(),
			", overall rank: ",
			num3.ToString(),
			" = ",
			this.rank.ToString()
		}), 2);
		if (ResultsController.Instance != null)
		{
			ResultsController.Instance.wcc.window.CloseWindow(false);
		}
		InterfaceController.Instance.ExecuteResolveDisplay(this);
		if (this.isSolved)
		{
			InterfaceController.Instance.ExecuteMissionCompleteDisplay(this);
		}
		else
		{
			InterfaceController.Instance.ExecuteMissionUnsolvedDisplay(this);
		}
		using (List<int>.Enumerator enumerator2 = this.suspectsDetained.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				int i = enumerator2.Current;
				Human human = null;
				if (CityData.Instance.GetHuman(i, out human, true))
				{
					if (this.isSolved && (this.caseType == Case.CaseType.murder || this.caseType == Case.CaseType.mainStory) && MurderController.Instance.activeMurders.Exists((MurderController.Murder item) => item.murdererID == i))
					{
						human.ai.isConvicted = true;
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "suspect convicted", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.star, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					}
					else
					{
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "suspect released", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.empty, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
						human.RemoveFromWorld(false);
					}
				}
			}
		}
		if (!this.isSolved)
		{
			num = 0;
		}
		GameplayController.Instance.AddMoney(num, true, "case reward");
		GameplayController.Instance.AddMoney(-num2, true, "case penalty");
		if (this.isSolved)
		{
			this.SetStatus(Case.CaseStatus.closable, true);
			if (this.caseType == Case.CaseType.sideJob)
			{
				if (this.job != null)
				{
					this.job.Complete();
				}
			}
			else if (this.caseType == Case.CaseType.murder)
			{
				for (int k = 0; k < MurderController.Instance.activeMurders.Count; k++)
				{
					MurderController.Instance.activeMurders[k].murderer.ai.isConvicted = true;
					MurderController.Instance.activeMurders[k].murderer.RemoveFromWorld(true);
					MurderController.Instance.activeMurders[k].SetMurderState(MurderController.MurderState.solved, true);
					k--;
				}
				MurderController.Instance.currentMurderer = null;
				MurderController.Instance.currentVictim = null;
				MurderController.Instance.currentActiveCase = null;
				MurderController.Instance.maxDifficultyLevel++;
				if (AchievementsController.Instance != null)
				{
					if (AchievementsController.Instance.notTheAnswerFlag == this.id)
					{
						AchievementsController.Instance.UnlockAchievement("Not the Answer", "complete_murder_case_no_violence");
					}
					if (AchievementsController.Instance.privateSlyFlag == this.id)
					{
						AchievementsController.Instance.UnlockAchievement("Private Sly", "complete_murder_case_no_trespass");
					}
				}
			}
			else if (this.caseType == Case.CaseType.mainStory)
			{
				for (int j = 0; j < MurderController.Instance.activeMurders.Count; j++)
				{
					MurderController.Instance.activeMurders[j].murderer.ai.isConvicted = true;
					MurderController.Instance.activeMurders[j].murderer.RemoveFromWorld(true);
					MurderController.Instance.activeMurders[j].SetMurderState(MurderController.MurderState.solved, true);
					j--;
				}
				MurderController.Instance.currentMurderer = null;
				MurderController.Instance.currentVictim = null;
				MurderController.Instance.currentActiveCase = null;
			}
			using (List<Case.ResolveQuestion>.Enumerator enumerator = this.resolveQuestions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Case.ResolveQuestion resolveQuestion2 = enumerator.Current;
					resolveQuestion2.OnProgressChange -= this.OnQuestionProgressChange;
				}
				return;
			}
		}
		this.SetStatus(Case.CaseStatus.handInCollected, true);
		foreach (Case.ResolveQuestion resolveQuestion3 in this.resolveQuestions)
		{
			resolveQuestion3.input = "...";
		}
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x000B9564 File Offset: 0x000B7764
	public bool ValidationCheck()
	{
		this.handInValid = false;
		if (this.job != null && this.resolveQuestions != null && this.resolveQuestions.Exists((Case.ResolveQuestion item) => item.displayOnlyAtPhase && this.job.phase < item.displayAtPhase))
		{
			this.handInValid = false;
			return this.handInValid;
		}
		if (this.caseStatus == Case.CaseStatus.handInCollected)
		{
			this.handInValid = true;
			foreach (Case.ResolveQuestion resolveQuestion in this.resolveQuestions)
			{
				if (!resolveQuestion.isOptional && !resolveQuestion.UpdateValid(this))
				{
					this.handInValid = false;
					break;
				}
			}
			if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.currentActiveObjectives != null)
			{
				Objective objective = CasePanelController.Instance.activeCase.currentActiveObjectives.Find((Objective item) => item.queueElement.entryRef == "Submit case");
				if (this.handInValid)
				{
					Game.Log("Jobs: Case input is valid", 2);
					if (objective == null && (CasePanelController.Instance.activeCase.caseType != Case.CaseType.sideJob || (CasePanelController.Instance.activeCase.job != null && CasePanelController.Instance.activeCase.job.knowHandInLocation)))
					{
						Interactable closestHandIn = this.GetClosestHandIn();
						if (closestHandIn != null)
						{
							if (CasePanelController.Instance.activeCase.caseType != Case.CaseType.sideJob && CasePanelController.Instance.activeCase.caseType != Case.CaseType.retirement)
							{
								Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.submitCase, "Submit case", false, 1f, null, null, null, null, null, null, null, "", false, default(Vector3));
								CasePanelController.Instance.activeCase.AddObjective("Submit case", trigger, true, closestHandIn.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, null, false, true, true);
							}
						}
						else
						{
							Game.Log("Jobs: No hand in locations for case " + this.name, 2);
						}
					}
				}
				else
				{
					Game.Log("Jobs: Case input is invalid", 2);
					if (objective != null)
					{
						objective.Cancel();
					}
				}
			}
		}
		if (ResolveController.Instance != null)
		{
			ResolveController.Instance.ValidationUpdate();
		}
		CasePanelController.Instance.UpdateResolveNotifications();
		return this.handInValid;
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x000B97B8 File Offset: 0x000B79B8
	public virtual void AddObjective(string entryRef, Objective.ObjectiveTrigger trigger, bool usePointer = false, Vector3 pointerPosition = default(Vector3), InterfaceControls.Icon useIcon = InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction onCompleteAction = Objective.OnCompleteAction.nextChapterPart, float delay = 0f, bool removePrevious = false, string chapterString = "", bool isSilent = false, bool allowCrouchPromt = false, SideJob jobRef = null, bool forceBottomOfList = false, bool ignoreDuplicates = false, bool useParsing = true)
	{
		if (ignoreDuplicates || !this.currentActiveObjectives.Exists((Objective item) => item.queueElement.entryRef == entryRef))
		{
			if (ignoreDuplicates || !this.inactiveCurrentObjectives.Exists((Objective item) => item.queueElement.entryRef == entryRef))
			{
				if (ignoreDuplicates || !this.endedObjectives.Exists((Objective item) => item.queueElement.entryRef == entryRef && item.isCancelled))
				{
					Game.Log("Objective: Successfully added objective " + entryRef + " to case " + this.name, 2);
					List<Objective.ObjectiveTrigger> list = new List<Objective.ObjectiveTrigger>();
					list.Add(trigger);
					Player.Instance.speechController.speechQueue.Add(new SpeechController.QueueElement(this.id, entryRef, usePointer, pointerPosition, useIcon, list, onCompleteAction, delay, removePrevious, chapterString, isSilent, allowCrouchPromt, jobRef, forceBottomOfList, useParsing));
					Player.Instance.speechController.enabled = true;
					using (List<SpeechController.QueueElement>.Enumerator enumerator = Player.Instance.speechController.speechQueue.FindAll((SpeechController.QueueElement item) => item.forceBottom).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SpeechController.QueueElement queueElement = enumerator.Current;
							Player.Instance.speechController.speechQueue.Remove(queueElement);
							Player.Instance.speechController.speechQueue.Add(queueElement);
						}
						return;
					}
				}
				Game.Log(string.Concat(new string[]
				{
					"Objective: ",
					entryRef,
					" already exists in case ",
					this.name,
					" ended objectives"
				}), 2);
				return;
			}
			Game.Log(string.Concat(new string[]
			{
				"Objective: ",
				entryRef,
				" already exists in case ",
				this.name,
				" inactive objectives"
			}), 2);
			return;
		}
		else
		{
			Game.Log(string.Concat(new string[]
			{
				"Objective: ",
				entryRef,
				" already exists in case ",
				this.name,
				" active objectives"
			}), 2);
		}
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x000B99FC File Offset: 0x000B7BFC
	public Case(string newName, Case.CaseType newCaseType, Case.CaseStatus newCaseStatus)
	{
		this.name = newName;
		this.caseType = newCaseType;
		this.id = Case.assignCaseID;
		Case.assignCaseID++;
		this.SetStatus(newCaseStatus, true);
		if ((this.caseType == Case.CaseType.murder || this.caseType == Case.CaseType.mainStory) && AchievementsController.Instance != null)
		{
			if (AchievementsController.Instance.notTheAnswerFlag < 0)
			{
				AchievementsController.Instance.notTheAnswerFlag = this.id;
			}
			if (AchievementsController.Instance.privateSlyFlag < 0)
			{
				AchievementsController.Instance.privateSlyFlag = this.id;
			}
		}
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x000B9B2C File Offset: 0x000B7D2C
	public Interactable GetClosestHandIn()
	{
		Interactable interactable = null;
		float num = float.PositiveInfinity;
		foreach (int num2 in this.handIn)
		{
			Interactable interactable2 = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(num2, ref interactable2))
			{
				float num3 = Vector3.Distance(interactable2.wPos, Player.Instance.transform.position);
				if (num3 < num || interactable == null)
				{
					interactable = interactable2;
					num = num3;
				}
			}
			else
			{
				NewDoor newDoor = null;
				if (CityData.Instance.doorDictionary.TryGetValue(-num2, ref newDoor))
				{
					float num4 = Vector3.Distance(newDoor.peekInteractable.wPos, Player.Instance.transform.position);
					if (num4 < num || interactable == null)
					{
						interactable = newDoor.peekInteractable;
						num = num4;
					}
				}
			}
		}
		return interactable;
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x000B9C18 File Offset: 0x000B7E18
	public void ClearAllObjectives()
	{
		Game.Log("Jobs: Clearing all objectives for " + this.name + "...", 2);
		Player.Instance.speechController.speechQueue.Clear();
		if (Player.Instance.speechController.activeSpeechBubble != null)
		{
			Toolbox.Instance.DestroyObject(Player.Instance.speechController.activeSpeechBubble.gameObject);
			Player.Instance.speechController.activeSpeechBubble = null;
		}
		Player.Instance.speechController.speechDelay = 0f;
		for (int i = 0; i < this.currentActiveObjectives.Count; i++)
		{
			if (!this.currentActiveObjectives[i].isCancelled)
			{
				this.currentActiveObjectives[i].Cancel();
			}
		}
	}

	// Token: 0x04000EDF RID: 3807
	[Header("Serializable")]
	public string name = "New Case";

	// Token: 0x04000EE0 RID: 3808
	public int id;

	// Token: 0x04000EE1 RID: 3809
	public static int assignCaseID = 1;

	// Token: 0x04000EE2 RID: 3810
	public Case.CaseType caseType = Case.CaseType.custom;

	// Token: 0x04000EE3 RID: 3811
	public Case.CaseStatus caseStatus = Case.CaseStatus.closable;

	// Token: 0x04000EE4 RID: 3812
	public int jobReference = -1;

	// Token: 0x04000EE5 RID: 3813
	public string mainStoryChapter;

	// Token: 0x04000EE6 RID: 3814
	public List<Case.CaseElement> caseElements = new List<Case.CaseElement>();

	// Token: 0x04000EE7 RID: 3815
	public List<Case.StringColours> stringColours = new List<Case.StringColours>();

	// Token: 0x04000EE8 RID: 3816
	public List<string> hiddenConnections = new List<string>();

	// Token: 0x04000EE9 RID: 3817
	public bool isActive;

	// Token: 0x04000EEA RID: 3818
	public bool handInValid;

	// Token: 0x04000EEB RID: 3819
	public bool isSolved;

	// Token: 0x04000EEC RID: 3820
	public float questionsRank;

	// Token: 0x04000EED RID: 3821
	public float victimsRank;

	// Token: 0x04000EEE RID: 3822
	public Case.CaseRank rank = Case.CaseRank.unSolved;

	// Token: 0x04000EEF RID: 3823
	public List<Objective> currentActiveObjectives = new List<Objective>();

	// Token: 0x04000EF0 RID: 3824
	public List<Objective> inactiveCurrentObjectives = new List<Objective>();

	// Token: 0x04000EF1 RID: 3825
	public List<Objective> endedObjectives = new List<Objective>();

	// Token: 0x04000EF2 RID: 3826
	public List<Case.ResolveQuestion> resolveQuestions = new List<Case.ResolveQuestion>();

	// Token: 0x04000EF3 RID: 3827
	public List<int> suspectsDetained = new List<int>();

	// Token: 0x04000EF4 RID: 3828
	public List<int> handIn = new List<int>();

	// Token: 0x04000EF5 RID: 3829
	[NonSerialized]
	public List<Objective> waitForObjectives = new List<Objective>();

	// Token: 0x04000EF6 RID: 3830
	[NonSerialized]
	public SideJob job;

	// Token: 0x0200023F RID: 575
	public enum CaseStatus
	{
		// Token: 0x04000EF8 RID: 3832
		handInNotCollected,
		// Token: 0x04000EF9 RID: 3833
		handInCollected,
		// Token: 0x04000EFA RID: 3834
		submitted,
		// Token: 0x04000EFB RID: 3835
		closable,
		// Token: 0x04000EFC RID: 3836
		archived,
		// Token: 0x04000EFD RID: 3837
		forced
	}

	// Token: 0x02000240 RID: 576
	public enum CaseType
	{
		// Token: 0x04000EFF RID: 3839
		mainStory,
		// Token: 0x04000F00 RID: 3840
		murder,
		// Token: 0x04000F01 RID: 3841
		sideJob,
		// Token: 0x04000F02 RID: 3842
		custom,
		// Token: 0x04000F03 RID: 3843
		retirement
	}

	// Token: 0x02000241 RID: 577
	public enum CaseRank
	{
		// Token: 0x04000F05 RID: 3845
		super,
		// Token: 0x04000F06 RID: 3846
		A,
		// Token: 0x04000F07 RID: 3847
		B,
		// Token: 0x04000F08 RID: 3848
		C,
		// Token: 0x04000F09 RID: 3849
		D,
		// Token: 0x04000F0A RID: 3850
		unSolved
	}

	// Token: 0x02000242 RID: 578
	[Serializable]
	public class CaseElement
	{
		// Token: 0x06000D00 RID: 3328 RVA: 0x000B9D10 File Offset: 0x000B7F10
		public void SetColour(InterfaceControls.EvidenceColours newColour)
		{
			this.color = newColour;
			foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
			{
				if (infoWindow.currentPinnedCaseElement == this)
				{
					infoWindow.UpdatePinColour();
				}
			}
			foreach (PinnedItemController pinnedItemController in CasePanelController.Instance.spawnedPins)
			{
				if (pinnedItemController.caseElement == this)
				{
					pinnedItemController.pinButtonController.UpdatePinColour();
				}
			}
		}

		// Token: 0x04000F0B RID: 3851
		public int caseID;

		// Token: 0x04000F0C RID: 3852
		public string n;

		// Token: 0x04000F0D RID: 3853
		public string id;

		// Token: 0x04000F0E RID: 3854
		public List<Evidence.DataKey> dk;

		// Token: 0x04000F0F RID: 3855
		public Vector2 v;

		// Token: 0x04000F10 RID: 3856
		public List<Evidence.DataKey> sdk;

		// Token: 0x04000F11 RID: 3857
		public bool ap;

		// Token: 0x04000F12 RID: 3858
		public bool w;

		// Token: 0x04000F13 RID: 3859
		public Vector3 resPos;

		// Token: 0x04000F14 RID: 3860
		public Vector2 resPiv;

		// Token: 0x04000F15 RID: 3861
		public bool co;

		// Token: 0x04000F16 RID: 3862
		public bool m;

		// Token: 0x04000F17 RID: 3863
		public InterfaceControls.EvidenceColours color;

		// Token: 0x04000F18 RID: 3864
		[NonSerialized]
		public PinnedItemController pinnedController;
	}

	// Token: 0x02000243 RID: 579
	[Serializable]
	public class StringColours
	{
		// Token: 0x04000F19 RID: 3865
		public string fromEv;

		// Token: 0x04000F1A RID: 3866
		public List<string> toEv;

		// Token: 0x04000F1B RID: 3867
		public List<Evidence.DataKey> fromDK;

		// Token: 0x04000F1C RID: 3868
		public List<Evidence.DataKey> toDK;

		// Token: 0x04000F1D RID: 3869
		public int colIndex;
	}

	// Token: 0x02000244 RID: 580
	[Serializable]
	public class ResolveQuestion
	{
		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06000D03 RID: 3331 RVA: 0x000B9DCC File Offset: 0x000B7FCC
		// (remove) Token: 0x06000D04 RID: 3332 RVA: 0x000B9E04 File Offset: 0x000B8004
		public event Case.ResolveQuestion.ProgressChange OnProgressChange;

		// Token: 0x06000D05 RID: 3333 RVA: 0x000B9E3C File Offset: 0x000B803C
		public bool UpdateCorrect(Case forCase, bool isMainStory = true)
		{
			string text = "Jobs: ";
			if (isMainStory)
			{
				text = "Chapter: ";
			}
			if (!this.isValid)
			{
				Game.Log(string.Concat(new string[]
				{
					text,
					"Question ",
					this.name,
					" is incorrect: Invalid input (",
					this.input,
					")"
				}), 2);
				this.isCorrect = false;
				return this.isCorrect;
			}
			if (this.inputType != Case.InputType.citizen)
			{
				if (this.inputType == Case.InputType.location)
				{
					List<NewGameLocation> list = CityData.Instance.gameLocationDirectory.FindAll((NewGameLocation item) => item.name.ToLower() == this.input.ToLower());
					if (list.Count > 0)
					{
						using (List<NewGameLocation>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								NewGameLocation newGameLocation = enumerator.Current;
								if (newGameLocation.thisAsAddress != null && this.correctAnswers.Contains(newGameLocation.thisAsAddress.id.ToString()))
								{
									Game.Log(string.Concat(new string[]
									{
										text,
										"Question ",
										this.name,
										" is correct: ",
										this.input
									}), 2);
									this.isCorrect = true;
									return this.isCorrect;
								}
								if (newGameLocation.thisAsStreet != null && this.correctAnswers.Contains(newGameLocation.thisAsStreet.streetID.ToString()))
								{
									Game.Log(string.Concat(new string[]
									{
										text,
										"Question ",
										this.name,
										" is correct: ",
										this.input
									}), 2);
									this.isCorrect = true;
									return this.isCorrect;
								}
							}
							goto IL_A5F;
						}
					}
					Game.Log(string.Concat(new string[]
					{
						text,
						"Question ",
						this.name,
						" is incorrect. Input: ",
						this.input,
						", no matching locations..."
					}), 2);
					this.isCorrect = false;
					return this.isCorrect;
				}
				if (this.inputType == Case.InputType.item)
				{
					if (this.correctAnswers.Contains(this.inputtedEvidence))
					{
						Game.Log(string.Concat(new string[]
						{
							text,
							"Question ",
							this.name,
							" is correct: ",
							this.input,
							"(",
							this.inputtedEvidence,
							")"
						}), 2);
						this.isCorrect = true;
						return this.isCorrect;
					}
					string text2 = string.Empty;
					foreach (string text3 in this.correctAnswers)
					{
						text2 = text2 + text3 + ", ";
					}
					Game.Log(string.Concat(new string[]
					{
						text,
						"Question ",
						this.name,
						" is incorrect. Input: ",
						this.input,
						" (",
						this.inputtedEvidence,
						"), correct answer(s): ",
						text2
					}), 2);
					this.isCorrect = false;
					return this.isCorrect;
				}
				else if (this.inputType == Case.InputType.revengeObjective)
				{
					if (this.revengeObjective != null && this.revengeObjective.Length > 0)
					{
						if (this.completedRevenge)
						{
							Game.Log(text + " Previously completed revenge objective", 2);
							this.isCorrect = true;
							return this.isCorrect;
						}
						RevengeObjective revengeObjective = null;
						Toolbox.Instance.LoadDataFromResources<RevengeObjective>(this.revengeObjective, out revengeObjective);
						if (revengeObjective != null)
						{
							MethodInfo method = revengeObjective.GetType().GetMethod(revengeObjective.answerMethod);
							if (method != null)
							{
								object[] array = new object[]
								{
									this.revengeObjTarget,
									this.revengeObjLoc,
									this.revengeObjPassed
								};
								object obj = method.Invoke(revengeObjective, array);
								float num = 0f;
								if (float.TryParse(obj.ToString(), ref num))
								{
									Game.Log(text + " Managed to parse float from " + obj.ToString(), 2);
									if (num >= this.revengeObjPassed)
									{
										Game.Log(text + "Question " + this.name + " is correct", 2);
										this.completedRevenge = true;
										this.isCorrect = true;
										return this.isCorrect;
									}
									Game.Log(text + "Question " + this.name + " is incorrect.", 2);
									this.isCorrect = false;
									return this.isCorrect;
								}
								else
								{
									bool flag = false;
									if (bool.TryParse(obj.ToString(), ref flag))
									{
										Game.Log(text + " Managed to parse bool from " + obj.ToString(), 2);
										if (flag)
										{
											Game.Log(text + "Question " + this.name + " is correct", 2);
											this.completedRevenge = true;
											this.isCorrect = true;
											return this.isCorrect;
										}
										Game.Log(text + "Question " + this.name + " is incorrect.", 2);
										this.isCorrect = false;
										return this.isCorrect;
									}
									else
									{
										Game.Log("Misc Error: Unable to parse bool from invokation of method " + revengeObjective.answerMethod + " for resolve question " + this.name, 2);
									}
								}
							}
							else
							{
								Game.Log("Misc Error: Unable to parse answer method " + revengeObjective.answerMethod + " for resolve question " + this.name, 2);
							}
						}
						else
						{
							Game.Log("Misc Error: No revenge objective for resolve question " + this.name, 2);
						}
					}
				}
				else if (this.inputType == Case.InputType.objective)
				{
					if (this.revengeObjective == null || this.revengeObjective.Length <= 0)
					{
						Game.Log(text + " Objective reference is null!", 2);
						this.isCorrect = false;
						return this.isCorrect;
					}
					if (forCase == null || forCase.job == null)
					{
						Game.Log(text + " Unable to find case", 2);
						this.isCorrect = false;
						return this.isCorrect;
					}
					List<Objective> list2 = null;
					if (forCase.job.objectiveReference.TryGetValue(this.revengeObjective, ref list2))
					{
						using (List<Objective>.Enumerator enumerator3 = list2.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								if (enumerator3.Current.isComplete)
								{
									this.isCorrect = true;
									return this.isCorrect;
								}
							}
						}
						return this.isCorrect;
					}
					Game.Log(text + " Could not find objective " + this.revengeObjective + " in job objective's reference...", 2);
					this.isCorrect = false;
					return this.isCorrect;
				}
				else if (this.inputType == Case.InputType.arrestPurp)
				{
					if (this.revengeObjective != null && this.revengeObjective.Length > 0)
					{
						if (forCase != null)
						{
							Case.ResolveQuestion resolveQuestion = forCase.resolveQuestions.Find((Case.ResolveQuestion item) => item.name.ToLower() == this.revengeObjective.ToLower());
							if (resolveQuestion != null)
							{
								using (List<string>.Enumerator enumerator2 = resolveQuestion.correctAnswers.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										string text4 = enumerator2.Current;
										Human human = null;
										int id = -1;
										int.TryParse(text4, ref id);
										if (CityData.Instance.GetHuman(id, out human, true) && human != null && human.ai != null && human.ai.restrained)
										{
											Game.Log(text + " Found correct citizen is restrained (" + human.citizenName + ")", 2);
											this.isCorrect = true;
											return this.isCorrect;
										}
									}
									goto IL_931;
								}
							}
							Game.Log(text + " Could not find resolve question " + this.revengeObjective + " to know if suspect is arrested...", 2);
							this.isCorrect = false;
							return this.isCorrect;
						}
						Game.Log(text + " Unable to find case", 2);
						IL_931:
						this.isCorrect = false;
						return this.isCorrect;
					}
					if (forCase.job != null && forCase.job.purp != null && forCase.job.purp.ai != null && forCase.job.purp.ai.restrained)
					{
						this.isCorrect = true;
						return this.isCorrect;
					}
				}
				else if (this.inputType == Case.InputType.saveVictim)
				{
					if (forCase != null)
					{
						this.isCorrect = true;
						foreach (MurderController.Murder murder in MurderController.Instance.activeMurders)
						{
							if (murder.victim != null && murder.victim.isDead)
							{
								Game.Log(text + " " + murder.victim.GetCitizenName() + " is dead", 2);
								this.isCorrect = false;
								break;
							}
						}
						return this.isCorrect;
					}
					Game.Log(text + " Unable to find case", 2);
					this.isCorrect = false;
					return this.isCorrect;
				}
				IL_A5F:
				Game.Log(text + "Question " + this.name + " is incorrect.", 2);
				this.isCorrect = false;
				return this.isCorrect;
			}
			Citizen citizen = CityData.Instance.citizenDirectory.Find((Citizen item) => item.GetCitizenName().ToLower() == this.input.ToLower());
			if (citizen != null && this.correctAnswers.Contains(citizen.humanID.ToString()))
			{
				Game.Log(string.Concat(new string[]
				{
					text,
					"Question ",
					this.name,
					" is correct: ",
					this.input
				}), 2);
				this.isCorrect = true;
				return this.isCorrect;
			}
			string text5 = string.Empty;
			foreach (string text6 in this.correctAnswers)
			{
				Human human2 = null;
				int id2 = -1;
				int.TryParse(text6, ref id2);
				if (CityData.Instance.GetHuman(id2, out human2, true))
				{
					text5 = string.Concat(new string[]
					{
						text5,
						human2.GetCitizenName(),
						" (",
						id2.ToString(),
						"), "
					});
				}
			}
			Game.Log(string.Concat(new string[]
			{
				text,
				"Question ",
				this.name,
				" is incorrect. Input: ",
				this.input,
				", correct answer(s): ",
				text5
			}), 2);
			this.isCorrect = false;
			return this.isCorrect;
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x000BA968 File Offset: 0x000B8B68
		public bool UpdateValid(Case forCase)
		{
			if (this.displayOnlyAtPhase && forCase != null && forCase.job != null && forCase.job.phase < this.displayAtPhase)
			{
				this.isValid = false;
				this.SetProgress(0f, false);
				return this.isValid;
			}
			if (this.inputType == Case.InputType.revengeObjective)
			{
				if (this.revengeObjective != null && this.revengeObjective.Length > 0)
				{
					if (this.completedRevenge)
					{
						this.isValid = true;
					}
					else
					{
						RevengeObjective revengeObjective = null;
						Toolbox.Instance.LoadDataFromResources<RevengeObjective>(this.revengeObjective, out revengeObjective);
						if (revengeObjective != null)
						{
							MethodInfo method = revengeObjective.GetType().GetMethod(revengeObjective.answerMethod);
							if (method != null)
							{
								NewGameLocation gameLocationFromQuestionInput = forCase.GetGameLocationFromQuestionInput(this);
								int num = -1;
								Human citizenFromQuestionInput = forCase.GetCitizenFromQuestionInput(this);
								int num2 = -1;
								if (gameLocationFromQuestionInput != null && gameLocationFromQuestionInput.thisAsAddress != null)
								{
									num = gameLocationFromQuestionInput.thisAsAddress.id;
								}
								if (citizenFromQuestionInput != null)
								{
									num2 = citizenFromQuestionInput.humanID;
								}
								object[] array = new object[]
								{
									num2,
									num,
									this.revengeObjPassed
								};
								object obj = method.Invoke(revengeObjective, array);
								float num3 = 0f;
								if (float.TryParse(obj.ToString(), ref num3))
								{
									Game.Log("Jobs: Managed to parse float from " + obj.ToString(), 2);
									if (num3 >= this.revengeObjPassed)
									{
										Game.Log(string.Concat(new string[]
										{
											"Question ",
											this.name,
											" is valid (",
											num3.ToString(),
											"/",
											this.revengeObjPassed.ToString(),
											")"
										}), 2);
										this.isValid = true;
									}
									else
									{
										Game.Log(string.Concat(new string[]
										{
											"Question ",
											this.name,
											" is invalid (",
											num3.ToString(),
											"/",
											this.revengeObjPassed.ToString(),
											")"
										}), 2);
										this.isValid = false;
									}
								}
								else
								{
									bool flag = false;
									if (bool.TryParse(obj.ToString(), ref flag))
									{
										Game.Log("Managed to parse bool from " + obj.ToString(), 2);
										if (flag)
										{
											Game.Log("Question " + this.name + " is valid", 2);
											this.isValid = true;
										}
										else
										{
											Game.Log("Question " + this.name + " is invalid.", 2);
											this.isValid = false;
										}
									}
									else
									{
										Game.Log("Misc Error: Unable to parse bool from invokation of method " + revengeObjective.answerMethod + " for resolve question " + this.name, 2);
									}
								}
							}
							else
							{
								Game.Log("Misc Error: Unable to parse answer method " + revengeObjective.answerMethod + " for resolve question " + this.name, 2);
							}
						}
						else
						{
							Game.Log("Misc Error: No revenge objective for resolve question " + this.name, 2);
						}
					}
				}
			}
			else if (this.inputType == Case.InputType.objective)
			{
				this.isValid = false;
				if (this.revengeObjective != null && this.revengeObjective.Length > 0 && forCase != null)
				{
					List<Objective> list = null;
					if (forCase.job.objectiveReference.TryGetValue(this.revengeObjective, ref list))
					{
						using (List<Objective>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Objective objective = enumerator.Current;
								Game.Log(string.Concat(new string[]
								{
									"Jobs: objective ",
									objective.queueElement.entryRef,
									" is complete: ",
									objective.isComplete.ToString(),
									" is cancelled: ",
									objective.isCancelled.ToString()
								}), 2);
								foreach (Objective.ObjectiveTrigger objectiveTrigger in objective.queueElement.triggers)
								{
									Game.Log(string.Concat(new string[]
									{
										"Jobs: ... Trigger: ",
										objectiveTrigger.triggerType.ToString(),
										": ",
										objectiveTrigger.interactableID.ToString(),
										" = ",
										objectiveTrigger.progressAdd.ToString()
									}), 2);
								}
								if (objective.isComplete)
								{
									this.isValid = true;
								}
							}
							goto IL_7E4;
						}
					}
					Game.Log(string.Concat(new string[]
					{
						"Jobs: Could not find objective ",
						this.revengeObjective,
						" in job objective's reference (job ",
						forCase.job.jobID.ToString(),
						", ref count: ",
						forCase.job.objectiveReference.Count.ToString(),
						")"
					}), 2);
				}
			}
			else if (this.inputType == Case.InputType.arrestPurp)
			{
				this.isValid = false;
				if (forCase != null)
				{
					if (this.revengeObjective != null && this.revengeObjective.Length > 0)
					{
						this.isValid = false;
						Case.ResolveQuestion getQ = forCase.resolveQuestions.Find((Case.ResolveQuestion item) => item.name.ToLower() == this.revengeObjective.ToLower());
						if (getQ != null)
						{
							Citizen citizen = CityData.Instance.citizenDirectory.Find((Citizen item) => item.GetCitizenName().ToLower() == getQ.input.ToLower());
							if (citizen != null && citizen.ai != null && citizen.ai.restrained)
							{
								Game.Log("Purp arrest is valid: " + citizen.GetCitizenName(), 2);
								this.isValid = true;
							}
							else
							{
								Game.Log("Purp arrest is invalid: " + getQ.input, 2);
							}
						}
						else
						{
							Game.Log("Could not find resolve question " + this.revengeObjective + " to know if suspect is arrested...", 2);
						}
					}
					else if (forCase.job != null)
					{
						if (forCase.job.purp != null && forCase.job.purp.ai != null && forCase.job.purp.ai.restrained)
						{
							this.isValid = true;
						}
					}
					else
					{
						Game.Log("Could not find resolve question " + this.revengeObjective + " to know if suspect is arrested...", 2);
					}
				}
			}
			else if (this.inputType == Case.InputType.saveVictim)
			{
				this.isValid = true;
			}
			else
			{
				this.isValid = false;
				if (this.input != null && this.input.Length > 0)
				{
					if (this.inputType == Case.InputType.citizen)
					{
						if (this.input.Split(new char[]
						{
							' '
						}, 1).Length >= 2)
						{
							this.isValid = true;
						}
						else
						{
							this.isValid = false;
						}
					}
					else if (this.inputType == Case.InputType.location)
					{
						if (CityData.Instance.gameLocationDirectory.Exists((NewGameLocation item) => item.name.ToLower() == this.input.ToLower()))
						{
							this.isValid = true;
						}
					}
					else if (this.inputType == Case.InputType.item && this.inputtedEvidence != null && this.inputtedEvidence.Length > 0)
					{
						if (GameplayController.Instance.evidenceDictionary.ContainsKey(this.inputtedEvidence))
						{
							Game.Log("Jobs: Found valid item input for evidence; " + this.inputtedEvidence, 2);
							this.isValid = true;
						}
						else
						{
							int num4 = 0;
							Interactable interactable;
							if (int.TryParse(this.inputtedEvidence, ref num4) && CityData.Instance.savableInteractableDictionary.TryGetValue(num4, ref interactable))
							{
								Game.Log("Jobs: Found valid item input for interactable; " + this.inputtedEvidence, 2);
								this.isValid = true;
							}
						}
					}
				}
			}
			IL_7E4:
			if (this.inputType != Case.InputType.revengeObjective)
			{
				if (this.isValid)
				{
					this.SetProgress(1f, false);
				}
				else
				{
					this.SetProgress(0f, false);
				}
			}
			else if (!this.isValid)
			{
				this.SetProgress(0f, false);
			}
			if (this.inputField != null)
			{
				this.inputField.UpdateCheckbox();
			}
			return this.isValid;
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x000BB1F0 File Offset: 0x000B93F0
		public string GetText(Case belongsToCase, bool includeReward = true, bool includePenalty = true)
		{
			string text = string.Empty;
			if (belongsToCase != null && belongsToCase.job != null)
			{
				text = Strings.ComposeText(Strings.Get("missions.postings", this.name, Strings.Casing.asIs, false, false, false, null), belongsToCase.job, Strings.LinkSetting.forceLinks, null, null, false);
			}
			else
			{
				text = Strings.Get("missions.postings", this.name, Strings.Casing.asIs, false, false, false, null);
			}
			if (this.isOptional)
			{
				text = text + " (" + Strings.Get("missions.postings", "Optional", Strings.Casing.asIs, false, false, false, null) + ")";
			}
			if (includeReward)
			{
				text = string.Concat(new string[]
				{
					text,
					"\n<size=70%>",
					Strings.Get("missions.postings", "Reward", Strings.Casing.asIs, false, false, false, null),
					": ",
					CityControls.Instance.cityCurrency,
					this.reward.ToString()
				});
			}
			if (this.penalty != 0 && includePenalty)
			{
				text = string.Concat(new string[]
				{
					text,
					" <color=#d70a0a>",
					Strings.Get("missions.postings", "Penalty", Strings.Casing.asIs, false, false, false, null),
					": ",
					CityControls.Instance.cityCurrency,
					this.penalty.ToString()
				});
			}
			return text;
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x000BB330 File Offset: 0x000B9530
		public RevengeObjective GetRevengeObjective()
		{
			RevengeObjective result = null;
			Toolbox.Instance.LoadDataFromResources<RevengeObjective>(this.revengeObjective, out result);
			return result;
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x000BB354 File Offset: 0x000B9554
		public void SetProgress(float val, bool forceTrigger = false)
		{
			if (this.progress != val || forceTrigger)
			{
				this.progress = Mathf.Clamp01(val);
				Game.Log("Setting question " + this.name + " progress to " + val.ToString(), 2);
				if (this.OnProgressChange != null)
				{
					this.OnProgressChange(this);
				}
			}
		}

		// Token: 0x04000F1E RID: 3870
		[HorizontalLine(2f, 3)]
		[Header("Setup")]
		public string name;

		// Token: 0x04000F1F RID: 3871
		public bool displayObjective = true;

		// Token: 0x04000F20 RID: 3872
		[EnableIf("displayObjective")]
		public bool displayOnlyAtPhase;

		// Token: 0x04000F21 RID: 3873
		[EnableIf("displayOnlyAtPhase")]
		public int displayAtPhase;

		// Token: 0x04000F22 RID: 3874
		public float objectiveDelay;

		// Token: 0x04000F23 RID: 3875
		public List<SideMissionIntroPreset> onlyCompatibleWithIntros = new List<SideMissionIntroPreset>();

		// Token: 0x04000F24 RID: 3876
		public List<SideMissionHandInPreset> onlyCompatibleWithHandIns = new List<SideMissionHandInPreset>();

		// Token: 0x04000F25 RID: 3877
		public Case.InputType inputType;

		// Token: 0x04000F26 RID: 3878
		[Tooltip("A list of automatically set answers")]
		public List<Case.AutoCorrectAnswer> automaticAnswers = new List<Case.AutoCorrectAnswer>();

		// Token: 0x04000F27 RID: 3879
		public JobPreset.JobTag tag;

		// Token: 0x04000F28 RID: 3880
		public InterfaceControls.Icon icon = InterfaceControls.Icon.resolve;

		// Token: 0x04000F29 RID: 3881
		public Vector2 rewardRange;

		// Token: 0x04000F2A RID: 3882
		public Vector2 penaltyRange;

		// Token: 0x04000F2B RID: 3883
		public bool isOptional;

		// Token: 0x04000F2C RID: 3884
		[HorizontalLine(2f, 3)]
		[Header("Revenge Objective")]
		public bool useAlternateName;

		// Token: 0x04000F2D RID: 3885
		public Case.RevengeObjectiveName useName = Case.RevengeObjectiveName.IDTarget;

		// Token: 0x04000F2E RID: 3886
		public JobPreset.LeadCitizen target;

		// Token: 0x04000F2F RID: 3887
		public JobPreset.JobSpawnWhere location;

		// Token: 0x04000F30 RID: 3888
		[ReadOnly]
		public string revengeObjective;

		// Token: 0x04000F31 RID: 3889
		[ReadOnly]
		public int revengeObjTarget = -1;

		// Token: 0x04000F32 RID: 3890
		[ReadOnly]
		public int revengeObjLoc = -1;

		// Token: 0x04000F33 RID: 3891
		[ReadOnly]
		public float revengeObjPassed;

		// Token: 0x04000F34 RID: 3892
		[ReadOnly]
		public bool completedRevenge;

		// Token: 0x04000F35 RID: 3893
		[Header("Inputted")]
		[ReadOnly]
		public string input = "...";

		// Token: 0x04000F36 RID: 3894
		[ReadOnly]
		public string inputtedEvidence = string.Empty;

		// Token: 0x04000F37 RID: 3895
		[Header("State")]
		[ReadOnly]
		public List<string> correctAnswers = new List<string>();

		// Token: 0x04000F38 RID: 3896
		[ReadOnly]
		public float progress;

		// Token: 0x04000F39 RID: 3897
		[ReadOnly]
		public int reward;

		// Token: 0x04000F3A RID: 3898
		[ReadOnly]
		public int penalty;

		// Token: 0x04000F3B RID: 3899
		[ReadOnly]
		public bool isValid;

		// Token: 0x04000F3C RID: 3900
		[ReadOnly]
		public bool isCorrect;

		// Token: 0x04000F3D RID: 3901
		[NonSerialized]
		public InputFieldController inputField;

		// Token: 0x02000245 RID: 581
		// (Invoke) Token: 0x06000D11 RID: 3345
		public delegate void ProgressChange(Case.ResolveQuestion resolve);
	}

	// Token: 0x02000247 RID: 583
	public enum RevengeObjectiveName
	{
		// Token: 0x04000F41 RID: 3905
		D0,
		// Token: 0x04000F42 RID: 3906
		D1,
		// Token: 0x04000F43 RID: 3907
		IDTarget
	}

	// Token: 0x02000248 RID: 584
	public enum AutoCorrectAnswer
	{
		// Token: 0x04000F45 RID: 3909
		none,
		// Token: 0x04000F46 RID: 3910
		poster,
		// Token: 0x04000F47 RID: 3911
		purp,
		// Token: 0x04000F48 RID: 3912
		purpsParamour,
		// Token: 0x04000F49 RID: 3913
		posterHome,
		// Token: 0x04000F4A RID: 3914
		purpHome,
		// Token: 0x04000F4B RID: 3915
		purpsParamourHome,
		// Token: 0x04000F4C RID: 3916
		posterWork,
		// Token: 0x04000F4D RID: 3917
		purpWork,
		// Token: 0x04000F4E RID: 3918
		purpsParamourWork,
		// Token: 0x04000F4F RID: 3919
		posterPhoto,
		// Token: 0x04000F50 RID: 3920
		purpPhoto,
		// Token: 0x04000F51 RID: 3921
		purpsParamourPhoto,
		// Token: 0x04000F52 RID: 3922
		posterHomePhoto,
		// Token: 0x04000F53 RID: 3923
		purpHomePhoto,
		// Token: 0x04000F54 RID: 3924
		purpsParamourHomePhoto,
		// Token: 0x04000F55 RID: 3925
		posterWorkPhoto,
		// Token: 0x04000F56 RID: 3926
		purpWorkPhoto,
		// Token: 0x04000F57 RID: 3927
		purpsParamourWorkPhoto,
		// Token: 0x04000F58 RID: 3928
		spawnedItemA,
		// Token: 0x04000F59 RID: 3929
		spawnedItemB,
		// Token: 0x04000F5A RID: 3930
		spawnedItemC,
		// Token: 0x04000F5B RID: 3931
		spawnedItemD,
		// Token: 0x04000F5C RID: 3932
		spawnedItemE,
		// Token: 0x04000F5D RID: 3933
		spawnedItemF,
		// Token: 0x04000F5E RID: 3934
		spawnedItemTag
	}

	// Token: 0x02000249 RID: 585
	public enum InputType
	{
		// Token: 0x04000F60 RID: 3936
		citizen,
		// Token: 0x04000F61 RID: 3937
		location,
		// Token: 0x04000F62 RID: 3938
		item,
		// Token: 0x04000F63 RID: 3939
		revengeObjective,
		// Token: 0x04000F64 RID: 3940
		objective,
		// Token: 0x04000F65 RID: 3941
		arrestPurp,
		// Token: 0x04000F66 RID: 3942
		saveVictim
	}
}
