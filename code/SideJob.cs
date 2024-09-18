using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Token: 0x020002F6 RID: 758
[Serializable]
public class SideJob
{
	// Token: 0x1400001F RID: 31
	// (add) Token: 0x0600114A RID: 4426 RVA: 0x000F6168 File Offset: 0x000F4368
	// (remove) Token: 0x0600114B RID: 4427 RVA: 0x000F61A0 File Offset: 0x000F43A0
	public event SideJob.ObjectivesChange OnObjectivesChanged;

	// Token: 0x14000020 RID: 32
	// (add) Token: 0x0600114C RID: 4428 RVA: 0x000F61D8 File Offset: 0x000F43D8
	// (remove) Token: 0x0600114D RID: 4429 RVA: 0x000F6210 File Offset: 0x000F4410
	public event SideJob.AcquireJobInfo AcquireInfo;

	// Token: 0x0600114E RID: 4430 RVA: 0x000F6248 File Offset: 0x000F4448
	public SideJob(JobPreset newPreset, SideJobController.JobPickData newData, bool immediatePost)
	{
		this.jobID = SideJob.assignJobID;
		SideJob.assignJobID++;
		if (!SideJobController.Instance.allJobsDictionary.ContainsKey(this.jobID))
		{
			SideJobController.Instance.allJobsDictionary.Add(this.jobID, this);
		}
		this.preset = newPreset;
		this.presetStr = newPreset.name;
		this.motive = newData.motive;
		this.motiveStr = this.motive.name;
		this.poster = newData.poster;
		this.posterID = this.poster.humanID;
		this.purp = newData.purp;
		this.purpID = this.purp.humanID;
		this.postImmediately = immediatePost;
		this.GenerateFakeNumber();
		this.startingScenario = Toolbox.Instance.Rand(0, this.preset.startingScenarios.Count, false);
		this.SetJobState(SideJob.JobState.generated, true);
		this.PickPoolLeadOptions();
		this.SpawnItems(ref this.preset.spawnItems);
		this.ChooseIntro();
		this.ChooseHandIn();
		if (this.preset.generateHidingLocation)
		{
			this.GenerateHidingLocation();
		}
		this.PostJob();
		this.GenerateResolveQuestions(true);
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x000F63FC File Offset: 0x000F45FC
	public virtual void GenerateFakeNumber()
	{
		if (this.fakeNumberStr == null || this.fakeNumberStr.Length <= 0)
		{
			string str = CityData.Instance.seed + this.motiveStr + (this.posterID * this.postID * this.purpID).ToString();
			this.fakeNumber = Toolbox.Instance.GetPsuedoRandomNumber(8000000, 8999000, str, false);
			while (CityData.Instance.phoneDictionary.ContainsKey(this.fakeNumber) || TelephoneController.Instance.fakeTelephoneDictionary.ContainsKey(this.fakeNumber))
			{
				this.fakeNumber++;
			}
			this.fakeNumberStr = Toolbox.Instance.GetTelephoneNumberString(this.fakeNumber);
		}
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x000F64C4 File Offset: 0x000F46C4
	public virtual void ChooseIntro()
	{
		List<JobPreset.IntroConfig> list = new List<JobPreset.IntroConfig>();
		foreach (JobPreset.IntroConfig introConfig in this.preset.compatibleIntros)
		{
			for (int i = 0; i < introConfig.frequency; i++)
			{
				list.Add(introConfig);
			}
		}
		if (list.Count > 0)
		{
			this.chosenIntro = list[Toolbox.Instance.Rand(0, list.Count, false)];
			this.intro = this.chosenIntro.preset.name;
		}
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x000F6570 File Offset: 0x000F4770
	public virtual void ChooseHandIn()
	{
		List<JobPreset.HandInConfig> list = new List<JobPreset.HandInConfig>();
		foreach (JobPreset.HandInConfig handInConfig in this.preset.compatibleHandIns)
		{
			for (int i = 0; i < handInConfig.frequency; i++)
			{
				list.Add(handInConfig);
			}
		}
		if (list.Count > 0)
		{
			this.chosenHandIn = list[Toolbox.Instance.Rand(0, list.Count, false)];
			this.handIn = this.chosenHandIn.preset.name;
		}
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x000F661C File Offset: 0x000F481C
	public virtual void SpawnItems(ref List<JobPreset.StartingSpawnItem> spawnThese)
	{
		List<JobPreset.StartingSpawnItem> list = new List<JobPreset.StartingSpawnItem>(spawnThese);
		List<JobPreset.StartingSpawnItem> successsfullySpawned = new List<JobPreset.StartingSpawnItem>();
		int num = 999;
		while (list.Count > 0 && num > 0)
		{
			JobPreset.StartingSpawnItem spawn = list[0];
			if (!this.SpawnItemIsValid(spawn, ref successsfullySpawned, !spawn.useOrGroup))
			{
				list.RemoveAt(0);
				num--;
			}
			else
			{
				List<JobPreset.StartingSpawnItem> list2 = null;
				if (spawn.useOrGroup)
				{
					list2 = new List<JobPreset.StartingSpawnItem>();
					List<JobPreset.StartingSpawnItem> list3 = list.FindAll((JobPreset.StartingSpawnItem item) => item.useOrGroup && item.orGroup == spawn.orGroup && this.SpawnItemIsValid(item, ref successsfullySpawned, false));
					for (int i = 0; i < list3.Count; i++)
					{
						for (int j = 0; j < Mathf.RoundToInt((float)list3[i].chanceRatio); j++)
						{
							list2.Add(list3[i]);
						}
					}
					spawn = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
				}
				if (spawn.spawnItem != null)
				{
					Game.Log(string.Concat(new string[]
					{
						"Jobs: ",
						this.presetStr,
						": Spawn item ",
						spawn.name,
						" for job ",
						this.preset.name,
						" tag: ",
						spawn.itemTag.ToString(),
						", use if ",
						spawn.useIf.ToString(),
						": ",
						spawn.ifTag.ToString(),
						", use or group ",
						spawn.useOrGroup.ToString(),
						": ",
						spawn.orGroup.ToString(),
						"..."
					}), 2);
					this.SpawnJobItem(spawn.spawnItem, spawn.where, spawn.belongsTo, spawn.writer, spawn.receiver, spawn.security, spawn.ownershipRule, spawn.priority, spawn.itemTag, spawn.findExisting);
					successsfullySpawned.Add(spawn);
				}
				if (spawn.vmailThread != null && spawn.vmailThread.Length > 0)
				{
					Human from = null;
					if (spawn.belongsTo == JobPreset.LeadCitizen.poster)
					{
						from = this.poster;
					}
					else if (spawn.belongsTo == JobPreset.LeadCitizen.purp)
					{
						from = this.purp;
					}
					else if (spawn.belongsTo == JobPreset.LeadCitizen.purpsParamour)
					{
						from = this.purp.paramour;
					}
					Toolbox.Instance.NewVmailThread(from, new List<Human>(), spawn.vmailThread, SessionData.Instance.gameTime + Toolbox.Instance.Rand(-48f, -12f, false), Mathf.RoundToInt(Toolbox.Instance.Rand(spawn.vmailProgressThreshold.x, spawn.vmailProgressThreshold.y, false)), StateSaveData.CustomDataSource.sender, -1);
				}
				list.Remove(spawn);
				if (list2 != null)
				{
					foreach (JobPreset.StartingSpawnItem startingSpawnItem in list2)
					{
						list.Remove(startingSpawnItem);
					}
				}
				num--;
			}
		}
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x000F6A24 File Offset: 0x000F4C24
	private bool SpawnItemIsValid(JobPreset.StartingSpawnItem spawn, ref List<JobPreset.StartingSpawnItem> successsfullySpawned, bool useChance)
	{
		if (useChance)
		{
			float num = spawn.chance;
			bool flag = true;
			if (spawn.useTraits)
			{
				foreach (JobPreset.JobModifierRule jobModifierRule in spawn.traitModifiers)
				{
					bool flag2 = false;
					Human human = null;
					if (jobModifierRule.who == JobPreset.LeadCitizen.poster)
					{
						human = this.poster;
					}
					else if (jobModifierRule.who == JobPreset.LeadCitizen.purp)
					{
						human = this.purp;
					}
					else if (jobModifierRule.who == JobPreset.LeadCitizen.purpsParamour)
					{
						human = this.purp.paramour;
					}
					if (human != null)
					{
						if (jobModifierRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
						{
							using (List<CharacterTrait>.Enumerator enumerator2 = jobModifierRule.traitList.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									CharacterTrait searchTrait = enumerator2.Current;
									if (human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag2 = true;
										break;
									}
								}
								goto IL_270;
							}
						}
						if (jobModifierRule.rule == CharacterTrait.RuleType.ifAllOfThese)
						{
							flag2 = true;
							using (List<CharacterTrait>.Enumerator enumerator2 = jobModifierRule.traitList.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									CharacterTrait searchTrait = enumerator2.Current;
									if (!human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag2 = false;
										break;
									}
								}
								goto IL_270;
							}
						}
						if (jobModifierRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
						{
							flag2 = true;
							using (List<CharacterTrait>.Enumerator enumerator2 = jobModifierRule.traitList.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									CharacterTrait searchTrait = enumerator2.Current;
									if (human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag2 = false;
										break;
									}
								}
								goto IL_270;
							}
						}
						if (jobModifierRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
						{
							if (human.partner != null)
							{
								using (List<CharacterTrait>.Enumerator enumerator2 = jobModifierRule.traitList.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										CharacterTrait searchTrait = enumerator2.Current;
										if (human.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
										{
											flag2 = true;
											break;
										}
									}
									goto IL_270;
								}
							}
							flag2 = false;
						}
					}
					IL_270:
					if (flag2)
					{
						num += jobModifierRule.chanceModifier;
					}
					else if (jobModifierRule.mustPassForApplication)
					{
						flag = false;
					}
				}
			}
			if (!flag || Toolbox.Instance.Rand(0f, 1f, false) > num)
			{
				return false;
			}
		}
		return !spawn.disableOnDifficulties.Contains(this.preset.difficultyTag) && (spawn.compatibleWithAllMotives || spawn.compatibleWithMotives.Contains(this.motive)) && (!spawn.useIf || successsfullySpawned.Exists((JobPreset.StartingSpawnItem item) => item.itemTag == spawn.itemTag));
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x000F6DDC File Offset: 0x000F4FDC
	public virtual void GameWorldLoop()
	{
		this.HandleObjectiveProgress();
		this.ObjectiveStateLoop();
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x000F6DEC File Offset: 0x000F4FEC
	public virtual void HandleObjectiveProgress()
	{
		if (this.thisCase != null)
		{
			bool flag = false;
			foreach (Case.ResolveQuestion resolveQuestion in this.thisCase.resolveQuestions)
			{
				if (resolveQuestion.inputType == Case.InputType.revengeObjective && resolveQuestion.revengeObjective != null && resolveQuestion.revengeObjective.Length > 0)
				{
					RevengeObjective revengeObjective = resolveQuestion.GetRevengeObjective();
					if (revengeObjective != null)
					{
						if (revengeObjective.specialConditions.Contains(RevengeObjective.SpecialConditions.trackProgressFromAddressQuestion))
						{
							NewGameLocation gameLocationFromQuestionInput = this.GetGameLocationFromQuestionInput(resolveQuestion);
							string text = "Jobs: Attempting to parse progress from location: ";
							NewGameLocation newGameLocation = gameLocationFromQuestionInput;
							Game.Log(text + ((newGameLocation != null) ? newGameLocation.ToString() : null), 2);
							if (gameLocationFromQuestionInput != null)
							{
								int num = 0;
								if (gameLocationFromQuestionInput.thisAsAddress != null)
								{
									num = gameLocationFromQuestionInput.thisAsAddress.id;
								}
								else if (gameLocationFromQuestionInput.thisAsStreet != null)
								{
									num = gameLocationFromQuestionInput.thisAsStreet.streetID;
								}
								MethodInfo method = revengeObjective.GetType().GetMethod(revengeObjective.answerMethod);
								if (method != null)
								{
									object[] array = new object[]
									{
										0,
										num,
										0
									};
									object obj = method.Invoke(revengeObjective, array);
									float num2 = 0f;
									if (float.TryParse(obj.ToString(), ref num2))
									{
										Game.Log(string.Concat(new string[]
										{
											"Jobs: Managed to parse float from ",
											obj.ToString(),
											" (",
											num2.ToString(),
											"/",
											resolveQuestion.revengeObjPassed.ToString(),
											")"
										}), 2);
									}
									else
									{
										Game.Log("Jobs: Unable to parse float from " + obj.ToString(), 2);
									}
									resolveQuestion.SetProgress(num2 / resolveQuestion.revengeObjPassed, false);
									Game.Log("Jobs: Progress for " + resolveQuestion.name + " is " + resolveQuestion.progress.ToString(), 2);
								}
								else
								{
									Game.Log("Jobs: Unable to return method from " + revengeObjective.answerMethod, 2);
									resolveQuestion.SetProgress(0f, false);
								}
							}
							else
							{
								resolveQuestion.SetProgress(0f, false);
							}
						}
					}
					else
					{
						Game.Log("Jobs: Unable to get revenge objective: " + resolveQuestion.revengeObjective, 2);
					}
					flag = true;
				}
			}
			if (flag)
			{
				this.thisCase.ValidationCheck();
			}
		}
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x000F7088 File Offset: 0x000F5288
	public NewGameLocation GetGameLocationFromQuestionInput(Case.ResolveQuestion question)
	{
		NewGameLocation result = null;
		if (this.preset.difficultyTag == JobPreset.DifficultyTag.D0)
		{
			result = this.GetGameLocation(question.location);
		}
		else
		{
			Case.ResolveQuestion addressQuestion = this.thisCase.resolveQuestions.Find((Case.ResolveQuestion item) => item.inputType == Case.InputType.location);
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

	// Token: 0x06001157 RID: 4439 RVA: 0x000F7144 File Offset: 0x000F5344
	public Human GetCitizenFromQuestionInput(Case.ResolveQuestion question)
	{
		Human result = null;
		if (this.preset.difficultyTag == JobPreset.DifficultyTag.D0)
		{
			result = this.GetTarget(question.target);
		}
		else
		{
			Case.ResolveQuestion citizenQuestion = this.thisCase.resolveQuestions.Find((Case.ResolveQuestion item) => item.inputType == Case.InputType.citizen);
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

	// Token: 0x06001158 RID: 4440 RVA: 0x000F7200 File Offset: 0x000F5400
	public virtual void ObjectiveStateLoop()
	{
		if (this.thisCase != null && this.thisCase.isActive && this.state != SideJob.JobState.ended && (InteractionController.Instance.talkingTo == null || (this.currentBlock != null && this.currentBlock.enableUpdateWhileTalking)))
		{
			bool flag = false;
			if (this.phase != this.phaseChange)
			{
				this.phaseChange = this.phase;
				flag = true;
				Game.Log("Jobs: Phase change detected: " + this.phase.ToString(), 2);
			}
			this.currentBlock = null;
			if (this.thisCase.caseStatus == Case.CaseStatus.handInNotCollected)
			{
				if (this.phase < this.chosenIntro.preset.blocks.Count)
				{
					this.currentBlock = this.chosenIntro.preset.blocks[this.phase];
				}
			}
			else if (this.thisCase.caseStatus == Case.CaseStatus.handInCollected)
			{
				try
				{
					if (this.phase - this.chosenIntro.preset.blocks.Count < this.preset.additional.Count)
					{
						this.currentBlock = this.preset.additional[this.phase - this.chosenIntro.preset.blocks.Count];
					}
					else if (this.phase - this.chosenIntro.preset.blocks.Count - this.preset.additional.Count < this.chosenHandIn.preset.blocks.Count)
					{
						this.currentBlock = this.chosenHandIn.preset.blocks[this.phase - this.chosenIntro.preset.blocks.Count - this.preset.additional.Count];
					}
				}
				catch
				{
					Game.LogError("Unable to get block for " + this.phase.ToString(), 2);
				}
			}
			if (this.currentBlock != null)
			{
				if (this.currentBlock.disableOnDifficulties.Contains(this.preset.difficultyTag))
				{
					this.phase++;
					return;
				}
				if (this.currentBlock.onlyCompatibleWithHandIns.Count > 0 && !this.currentBlock.onlyCompatibleWithHandIns.Exists((SideMissionHandInPreset item) => item.name.ToLower() == this.handIn.ToLower()))
				{
					this.phase++;
					return;
				}
				if (this.currentBlock.onlyCompativleWithIntros.Count > 0 && !this.currentBlock.onlyCompativleWithIntros.Exists((SideMissionIntroPreset item) => item.name.ToLower() == this.intro.ToLower()))
				{
					this.phase++;
					return;
				}
				if (flag)
				{
					Game.Log(string.Concat(new string[]
					{
						"Jobs: ",
						this.presetStr,
						" (",
						this.jobID.ToString(),
						") enters phase ",
						this.phase.ToString(),
						": ",
						this.currentBlock.elementType.ToString()
					}), 2);
					if (this.currentBlock.spawnItems != null && this.currentBlock.spawnItems.Count > 0)
					{
						this.SpawnItems(ref this.currentBlock.spawnItems);
					}
					if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.playerCallsNumber)
					{
						JobPreset.DialogReference dialogReference = this.preset.dialogReferences.Find((JobPreset.DialogReference item) => item.name.ToLower() == this.currentBlock.dialogReference.ToLower());
						if (dialogReference != null)
						{
							this.GenerateFakeNumber();
							TelephoneController.Instance.AddFakeNumber(this.fakeNumber, new TelephoneController.CallSource(TelephoneController.CallType.player, dialogReference.dialog, this, InteractionController.ConversationType.normal));
							TelephoneController.Instance.fakeTelephoneDictionary[this.fakeNumber].dialogGreeting = dialogReference.dialog;
							TelephoneController.Instance.fakeTelephoneDictionary[this.fakeNumber].dialog = dialogReference.dialog.name;
							Objective.ObjectiveTrigger objectiveTrigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.makeCall, this.fakeNumber.ToString(), false, 0f, null, null, null, null, null, null, this, "", false, default(Vector3));
							string name = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger = objectiveTrigger;
							bool usePointer = false;
							string chapterString = this.phase.ToString();
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name, trigger, usePointer, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
						}
						else
						{
							Game.LogError("Could not find job dialog index " + this.currentBlock.dialogReference, 2);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.acquireInformation)
					{
						JobPreset.DialogReference dialogReference2 = this.preset.dialogReferences.Find((JobPreset.DialogReference item) => item.name.ToLower() == this.currentBlock.dialogReference.ToLower());
						if (dialogReference2 != null)
						{
							this.OnAcquireJobInfo(dialogReference2.dialog);
						}
						else
						{
							Game.LogError("Could not find job dialog index " + this.currentBlock.dialogReference, 2);
						}
						this.phase++;
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.askStaff)
					{
						JobPreset.DialogReference dialogReference3 = this.preset.dialogReferences.Find((JobPreset.DialogReference item) => item.name.ToLower() == this.currentBlock.dialogReference.ToLower());
						if (dialogReference3 != null && this.post != null)
						{
							if (this.post.node != null)
							{
								if (this.post.node.gameLocation != null)
								{
									if (this.post.node.gameLocation.thisAsAddress != null)
									{
										if (this.post.node.gameLocation.thisAsAddress.company != null)
										{
											foreach (Occupation occupation in this.post.node.gameLocation.thisAsAddress.company.companyRoster.FindAll((Occupation item) => item.employee != null && item.preset.canAskAboutJob))
											{
												Game.Log("Jobs: Adding dialog " + dialogReference3.dialog.name + " to " + occupation.employee.name, 2);
												this.AddDialogOption(occupation.employee, Evidence.DataKey.photo, dialogReference3.dialog, null);
											}
										}
										Objective.ObjectiveTrigger objectiveTrigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.onDialogSuccess, dialogReference3.dialog.name, false, 0f, null, null, null, null, null, null, this, "", false, default(Vector3));
										string name2 = this.currentBlock.name;
										Objective.ObjectiveTrigger trigger2 = objectiveTrigger2;
										bool usePointer2 = false;
										string chapterString = this.phase.ToString();
										float objectiveDelay = this.currentBlock.objectiveDelay;
										this.AddObjective(name2, trigger2, usePointer2, default(Vector3), InterfaceControls.Icon.citizen, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
									}
									else
									{
										Game.LogError("Job post " + this.post.id.ToString() + " is not at an address! Location: " + this.post.GetWorldPosition(true).ToString(), 2);
									}
								}
								else
								{
									Game.LogError("Job post " + this.post.id.ToString() + " has not been assigned a GameLocation. Location: " + this.post.GetWorldPosition(true).ToString(), 2);
								}
							}
							else
							{
								Game.LogError(string.Concat(new string[]
								{
									"Job post ",
									this.post.id.ToString(),
									" has not been assigned a Node. Location: ",
									this.post.GetWorldPosition(true).ToString(),
									" Attempting to assign now..."
								}), 2);
								this.post.UpdateWorldPositionAndNode(false);
								if (this.post.node != null)
								{
									Game.Log("Job post " + this.post.id.ToString() + " node assign successful!", 2);
								}
							}
						}
						else
						{
							Game.LogError("Could not find job dialog index " + this.currentBlock.dialogReference, 2);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.spawnItems)
					{
						this.phase++;
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.photoOfItemLocation)
					{
						Interactable item9 = this.GetItem(this.currentBlock.tagReference);
						if (item9 != null)
						{
							if (this.hiddenItemPhoto == null)
							{
								this.hiddenItemPhoto = Toolbox.Instance.GetLocalizedSnapshot(item9);
							}
							if (this.hiddenItemPhoto != null)
							{
								ActionController.Instance.Inspect(this.hiddenItemPhoto, Player.Instance.currentNode, Player.Instance);
								CasePanelController.Instance.PinToCasePanel(this.thisCase, this.hiddenItemPhoto.evidence, Evidence.DataKey.photo, true, default(Vector2), false);
							}
							else
							{
								Game.LogError("Unable to generate hidden item photo", 2);
							}
							this.phase++;
						}
						else
						{
							Game.LogError("Could not find item with tag " + this.currentBlock.tagReference.ToString(), 2);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.openedBriefcase)
					{
						Interactable item2 = this.GetItem(this.currentBlock.tagReference);
						if (item2 != null)
						{
							Objective.ObjectiveTrigger objectiveTrigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.raiseFirstPersonItem, "", false, 0f, null, item2, null, null, null, null, null, "", false, default(Vector3));
							string name3 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger3 = objectiveTrigger3;
							bool usePointer3 = false;
							string chapterString = this.phase.ToString();
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name3, trigger3, usePointer3, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
							Game.Log("Jobs: Hidden item exists (" + item2.id.ToString() + ") at " + item2.GetWorldPosition(true).ToString(), 2);
						}
						else
						{
							Game.LogError("Could not find item with tag " + this.currentBlock.tagReference.ToString(), 2);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.playerHasCamera)
					{
						Objective.ObjectiveTrigger objectiveTrigger4 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.hasFPSInventory, "camera", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
						string name4 = this.currentBlock.name;
						Objective.ObjectiveTrigger trigger4 = objectiveTrigger4;
						bool usePointer4 = false;
						string chapterString = this.phase.ToString();
						float objectiveDelay = this.currentBlock.objectiveDelay;
						this.AddObjective(name4, trigger4, usePointer4, default(Vector3), InterfaceControls.Icon.camera, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.playerHasHandcuffs)
					{
						Objective.ObjectiveTrigger objectiveTrigger5 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.hasFPSInventory, "handcuffs", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
						string name5 = this.currentBlock.name;
						Objective.ObjectiveTrigger trigger5 = objectiveTrigger5;
						bool usePointer5 = false;
						string chapterString = this.phase.ToString();
						float objectiveDelay = this.currentBlock.objectiveDelay;
						this.AddObjective(name5, trigger5, usePointer5, default(Vector3), InterfaceControls.Icon.handcuffs, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.setGooseChaseCall)
					{
						if (this.gooseChasePhone == 0)
						{
							List<Telephone> list = new List<Telephone>(CityData.Instance.phoneDictionary.Values).FindAll((Telephone item) => item.interactable != null && item.interactable.preset.isPayphone && item.interactable.node.gameLocation.IsPublicallyOpen(true) && item.interactable.node.gameLocation != Player.Instance.currentGameLocation);
							if (list.Count > 0)
							{
								Telephone telephone = list[Toolbox.Instance.Rand(0, list.Count, false)];
								if (telephone != null)
								{
									this.chosenGooseChasePhone = telephone.interactable;
									this.gooseChasePhone = telephone.interactable.id;
									Game.Log("Jobs: Setting goose chase phone as " + this.gooseChasePhone.ToString(), 2);
									PathFinder.PathData path = PathFinder.Instance.GetPath(Player.Instance.currentNode, this.chosenGooseChasePhone.node, Player.Instance, null);
									if (path != null)
									{
										float num = 0.5f;
										this.gooseChaseCallTime = SessionData.Instance.gameTime + 0.085f + (float)path.accessList.Count / 60f / 60f * num;
									}
								}
								else
								{
									Game.Log("Jobs: Unable to pick goose chase phone!", 2);
								}
							}
							else
							{
								Game.Log("Jobs: Unable to pick goose chase phone!", 2);
							}
						}
						if (this.chosenGooseChasePhone != null)
						{
							this.gooseChaseCallTriggered = false;
							Objective.ObjectiveTrigger objectiveTrigger6 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, this.chosenGooseChasePhone.node, null, null, null, "", false, default(Vector3));
							string name6 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger6 = objectiveTrigger6;
							bool usePointer6 = false;
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name6, trigger6, usePointer6, default(Vector3), InterfaceControls.Icon.run, Objective.OnCompleteAction.nothing, objectiveDelay, false, "", false, false);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.setGooseChaseCallIndoorOnly)
					{
						if (this.gooseChasePhone == 0)
						{
							List<Telephone> list2 = new List<Telephone>(CityData.Instance.phoneDictionary.Values).FindAll((Telephone item) => item.interactable != null && item.interactable.preset.isPayphone && item.interactable.node.gameLocation.thisAsAddress != null && item.interactable.node.gameLocation.thisAsAddress.company != null && item.interactable.node.gameLocation.IsPublicallyOpen(true) && item.interactable.node.gameLocation != this.post.node.gameLocation);
							Telephone telephone2 = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
							if (telephone2 != null)
							{
								this.chosenGooseChasePhone = telephone2.interactable;
								this.gooseChasePhone = telephone2.interactable.id;
								PathFinder.PathData path2 = PathFinder.Instance.GetPath(Player.Instance.currentNode, this.chosenGooseChasePhone.node, Player.Instance, null);
								if (path2 != null)
								{
									float num2 = 0.5f;
									this.gooseChaseCallTime = SessionData.Instance.gameTime + 0.085f + (float)path2.accessList.Count / 60f / 60f * num2;
								}
							}
						}
						if (this.chosenGooseChasePhone != null)
						{
							this.gooseChaseCallTriggered = false;
							Objective.ObjectiveTrigger objectiveTrigger7 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, this.chosenGooseChasePhone.node, null, null, null, "", false, default(Vector3));
							string name7 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger7 = objectiveTrigger7;
							bool usePointer7 = false;
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name7, trigger7, usePointer7, default(Vector3), InterfaceControls.Icon.run, Objective.OnCompleteAction.nothing, objectiveDelay, false, "", false, false);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.setMeeting)
					{
						if (this.meetingPoint == 0)
						{
							List<Interactable> list3 = new List<Interactable>();
							foreach (NewGameLocation newGameLocation in CityData.Instance.gameLocationDirectory)
							{
								if ((!(newGameLocation.thisAsAddress != null) || newGameLocation.thisAsAddress.company == null || !(newGameLocation.thisAsAddress.company.preset.workHours.presetName != "247")) && newGameLocation.IsPublicallyOpen(true))
								{
									foreach (NewRoom newRoom in newGameLocation.rooms)
									{
										foreach (NewNode newNode in newRoom.nodes)
										{
											List<Interactable> list4 = newNode.interactables.FindAll(delegate(Interactable item)
											{
												Human human;
												return item.furnitureParent != null && item.node.gameLocation != Player.Instance.currentGameLocation && this.currentBlock.validItems.Contains(item.preset) && this.currentBlock.validFurniture.Contains(item.furnitureParent.furniture) && !item.usagePoint.TryGetUserAtSlot(Interactable.UsePointSlot.defaultSlot, out human);
											});
											if (list4.Count > 0)
											{
												list3.AddRange(list4);
											}
										}
									}
								}
							}
							if (list3.Count > 0)
							{
								this.chosenMeetingPoint = list3[Toolbox.Instance.Rand(0, list3.Count, false)];
							}
							if (this.chosenMeetingPoint != null)
							{
								Game.Log("Jobs: Choosing new meeting point at " + this.chosenMeetingPoint.GetName() + " " + this.chosenMeetingPoint.GetWorldPosition(true).ToString(), 2);
								this.meetingPoint = this.chosenMeetingPoint.id;
								this.gooseChaseCallTime = SessionData.Instance.gameTime;
								this.meetingConsumableIndex = Toolbox.Instance.Rand(0, InteriorControls.Instance.meetupConsumables.Count, false);
							}
							else
							{
								Game.LogError("Jobs: Unable to find meeting point for job " + this.jobID.ToString(), 2);
							}
						}
						if (this.chosenMeetingPoint != null)
						{
							JobPreset.DialogReference dialogReference4 = this.preset.dialogReferences.Find((JobPreset.DialogReference item) => item.name.ToLower() == this.currentBlock.dialogReference.ToLower());
							if (dialogReference4 != null)
							{
								this.AddDialogOption(this.poster, Evidence.DataKey.voice, dialogReference4.dialog, null);
								Game.Log("Jobs: Attempting to teleport poster " + this.poster.citizenName + "...", 2);
								this.poster.Teleport(this.poster.FindSafeTeleport(this.chosenMeetingPoint.node.gameLocation, false, true), this.chosenMeetingPoint.usagePoint, true, false);
								Game.Log("Jobs: ... Poster " + this.poster.citizenName + " teleported to " + this.poster.transform.position.ToString(), 2);
								this.poster.outfitController.SetCurrentOutfit(ClothesPreset.OutfitCategory.outdoorsCasual, false, false, true);
								this.poster.ai.CreateNewGoal(RoutineControls.Instance.missionMeetUpSpecific, SessionData.Instance.gameTime, 1f, null, this.chosenMeetingPoint, null, null, null, -2);
								if (this.poster.trash.Count > 0)
								{
									Interactable interactable = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.disposal, this.poster.currentRoom, this.poster, AIActionPreset.FindSetting.onlyPublic, true, null, null, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
									if (interactable != null)
									{
										ActionController.Instance.Dispose(interactable, interactable.node, this.poster);
									}
									for (int i = 0; i < this.poster.trash.Count; i++)
									{
										this.poster.trash.RemoveAt(i);
										i--;
									}
								}
								if (this.poster.ai != null)
								{
									this.poster.ai.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
								}
								this.poster.AddCurrentConsumable(InteriorControls.Instance.meetupConsumables[this.meetingConsumableIndex]);
								this.poster.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.armsConsuming);
								Objective.ObjectiveTrigger objectiveTrigger8 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.onDialogSuccess, dialogReference4.dialog.name, false, 0f, null, null, null, null, null, null, this, "", false, default(Vector3));
								string name8 = this.currentBlock.name;
								Objective.ObjectiveTrigger trigger8 = objectiveTrigger8;
								bool usePointer8 = false;
								Vector3 worldPosition = this.chosenMeetingPoint.GetWorldPosition(true);
								InterfaceControls.Icon useIcon = InterfaceControls.Icon.citizen;
								Objective.OnCompleteAction onCompleteAction = Objective.OnCompleteAction.nextSideJobPhase;
								string chapterString = this.phase.ToString();
								this.AddObjective(name8, trigger8, usePointer8, worldPosition, useIcon, onCompleteAction, this.currentBlock.objectiveDelay, false, chapterString, false, false);
							}
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.setHomeMeeting)
					{
						JobPreset.DialogReference dialogReference5 = this.preset.dialogReferences.Find((JobPreset.DialogReference item) => item.name.ToLower() == this.currentBlock.dialogReference.ToLower());
						if (dialogReference5 != null)
						{
							foreach (Human person in this.poster.home.inhabitants)
							{
								this.AddDialogOption(person, Evidence.DataKey.voice, dialogReference5.dialog, null);
							}
							this.poster.Teleport(this.poster.FindSafeTeleport(this.poster.home, false, true), null, true, false);
							Interactable newPassedInteractable = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.sit, this.poster.home.rooms[0], this.poster, AIActionPreset.FindSetting.homeOnly, true, null, null, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
							this.poster.ai.CreateNewGoal(RoutineControls.Instance.missionMeetUpSpecific, SessionData.Instance.gameTime, 3f, null, newPassedInteractable, null, null, null, -2);
							Objective.ObjectiveTrigger objectiveTrigger9 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.poster.home, this, "", false, default(Vector3));
							string name9 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger9 = objectiveTrigger9;
							bool usePointer9 = false;
							string chapterString = this.phase.ToString();
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name9, trigger9, usePointer9, default(Vector3), InterfaceControls.Icon.citizen, Objective.OnCompleteAction.nothing, objectiveDelay, false, chapterString, false, false);
							this.phase++;
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.setupHomeInvestigation)
					{
						Player.Instance.AddToKeyring(this.poster.home, true);
						GameplayController.Instance.AddGuestPass(this.poster.home, 12f);
						this.phase++;
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.tailBriefcase)
					{
						Objective.ObjectiveTrigger objectiveTrigger10 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.sideMissionMeetTriggered, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
						string name10 = this.currentBlock.name;
						Objective.ObjectiveTrigger trigger10 = objectiveTrigger10;
						bool usePointer10 = false;
						string chapterString = this.phase.ToString();
						float objectiveDelay = this.currentBlock.objectiveDelay;
						this.AddObjective(name10, trigger10, usePointer10, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.playerHasItemInPossession)
					{
						Interactable item3 = this.GetItem(this.currentBlock.tagReference);
						if (item3 != null)
						{
							Objective.ObjectiveTrigger objectiveTrigger11 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemInInventory, "", false, 0f, null, item3, null, null, null, null, null, "", false, default(Vector3));
							string name11 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger11 = objectiveTrigger11;
							bool usePointer11 = false;
							string chapterString = this.phase.ToString();
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name11, trigger11, usePointer11, default(Vector3), InterfaceControls.Icon.robbery, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
						}
						else
						{
							Game.Log("Jobs: Cannot find item with tag " + this.currentBlock.tagReference.ToString(), 2);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.leaveItemAtSecretLocation)
					{
						if (this.secretLocationFurniture == 0)
						{
							Game.LogError("Jobs: Secret location was not generated at start! Please enable this on the jobs preset...", 2);
							this.GenerateHidingLocation();
						}
						Interactable item4 = this.GetItem(this.currentBlock.tagReference);
						if (item4 != null)
						{
							Objective.ObjectiveTrigger objectiveTrigger12 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemIsPlacedAtSecretLocation, "", false, 0f, null, item4, null, null, null, null, this, "", false, default(Vector3));
							string name12 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger12 = objectiveTrigger12;
							bool usePointer12 = false;
							string chapterString = this.phase.ToString();
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name12, trigger12, usePointer12, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.destroyItem)
					{
						Interactable item5 = this.GetItem(this.currentBlock.tagReference);
						if (item5 != null)
						{
							Objective.ObjectiveTrigger objectiveTrigger13 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.destroyItem, "", false, 0f, null, item5, null, null, null, null, null, "", false, default(Vector3));
							string name13 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger13 = objectiveTrigger13;
							bool usePointer13 = false;
							string chapterString = this.phase.ToString();
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name13, trigger13, usePointer13, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.placeItemInPosterMailbox)
					{
						Interactable item6 = this.GetItem(this.currentBlock.tagReference);
						if (item6 != null)
						{
							Interactable mailbox = Toolbox.Instance.GetMailbox(this.poster);
							if (mailbox != null)
							{
								mailbox.SetLockedState(false, null, true, false);
								Objective.ObjectiveTrigger objectiveTrigger14 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemIsNear, "", false, 0f, null, item6, null, null, null, null, this, "", false, mailbox.GetWorldPosition(true));
								string name14 = this.currentBlock.name;
								Objective.ObjectiveTrigger trigger14 = objectiveTrigger14;
								bool usePointer14 = false;
								string chapterString = this.phase.ToString();
								float objectiveDelay = this.currentBlock.objectiveDelay;
								this.AddObjective(name14, trigger14, usePointer14, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
							}
							else
							{
								Game.Log("Jobs: Unable to get mailbox!", 2);
							}
						}
					}
					else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.placeItemOfTypeInPosterMailbox)
					{
						Interactable item7 = this.GetItem(this.currentBlock.tagReference);
						if (item7 != null)
						{
							Interactable mailbox2 = Toolbox.Instance.GetMailbox(this.poster);
							if (mailbox2 != null)
							{
								mailbox2.SetLockedState(false, null, true, false);
								Objective.ObjectiveTrigger objectiveTrigger15 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemOfTypeIsNear, "", false, 0f, null, item7, null, null, null, null, this, "", false, mailbox2.GetWorldPosition(true));
								string name15 = this.currentBlock.name;
								Objective.ObjectiveTrigger trigger15 = objectiveTrigger15;
								bool usePointer15 = false;
								string chapterString = this.phase.ToString();
								float objectiveDelay = this.currentBlock.objectiveDelay;
								this.AddObjective(name15, trigger15, usePointer15, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextSideJobPhase, objectiveDelay, false, chapterString, false, false);
							}
							else
							{
								Game.Log("Jobs: Unable to get mailbox!", 2);
							}
						}
					}
					if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.handDossier)
					{
						Interactable item8 = this.GetItem(this.currentBlock.tagReference);
						if (item8 != null)
						{
							item8.MoveInteractable(Player.Instance.transform.position, Vector3.zero, true);
							item8.SetSpawnPositionRelevent(false);
							FirstPersonItemController.Instance.PickUpItem(item8, true, false, true, true, true);
							this.phase++;
						}
					}
				}
				if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.setGooseChaseCall || this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.setGooseChaseCallIndoorOnly)
				{
					if (!this.gooseChaseCallTriggered)
					{
						if (SessionData.Instance.gameTime >= this.gooseChaseCallTime)
						{
							if (this.chosenGooseChasePhone != null)
							{
								List<Telephone> list5 = new List<Telephone>(CityData.Instance.phoneDictionary.Values).FindAll((Telephone item) => item.interactable != null && item.interactable.preset.isPayphone && item.interactable != this.chosenGooseChasePhone && !item.interactable.sw1);
								Telephone telephone3 = list5[Toolbox.Instance.Rand(0, list5.Count, false)];
								if (telephone3 != null && !telephone3.interactable.sw1)
								{
									JobPreset.DialogReference dialogReference6 = this.preset.dialogReferences.Find((JobPreset.DialogReference item) => item.name.ToLower() == this.currentBlock.dialogReference.ToLower());
									if (dialogReference6 != null)
									{
										this.gooseChaseCall = TelephoneController.Instance.CreateNewCall(telephone3, this.chosenGooseChasePhone.t, null, Player.Instance, new TelephoneController.CallSource(TelephoneController.CallType.player, dialogReference6.dialog, this, InteractionController.ConversationType.normal), 0.075f, true);
										this.gooseChaseFromPhone = telephone3.interactable.id;
										telephone3.interactable.SetCustomState1(true, null, true, false, false);
										this.gooseChaseCallTriggered = true;
										this.OnGooseChaseCallTriggered();
										TelephoneController.Instance.OnPlayerCall += this.OnPlayerCall;
										Objective.ObjectiveTrigger trigger16 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.answerPhone, "", false, 0f, null, this.chosenGooseChasePhone, null, null, null, null, null, "", false, default(Vector3));
										this.AddObjective("Answer", trigger16, true, this.chosenGooseChasePhone.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextSideJobPhase, 2f, false, this.phase.ToString(), false, false);
									}
								}
							}
							else
							{
								Game.Log("Error: Chosen goose chase phone is null!", 2);
							}
						}
					}
					else if (this.gooseChaseCall != null)
					{
						if (this.gooseChaseCall.recevierNS != null && !this.gooseChaseCall.recevierNS.isPlayer)
						{
							this.OnGooseChaseEnd();
							this.TriggerFail("Missed Call");
						}
						else if (this.gooseChaseCall.state == TelephoneController.CallState.ended)
						{
							this.OnGooseChaseEnd();
							this.TriggerFail("Missed Call");
						}
					}
				}
				if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.postSubmission)
				{
					if (!this.triggerHandIn && this.thisCase != null && this.thisCase.handInValid)
					{
						Interactable closestHandIn = this.thisCase.GetClosestHandIn();
						if (closestHandIn != null)
						{
							this.triggerHandIn = true;
							Objective.ObjectiveTrigger trigger17 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.submitCase, "Submit case sidejob", false, 1f, null, null, null, null, null, null, null, "", false, default(Vector3));
							this.AddObjective(this.currentBlock.name, trigger17, false, closestHandIn.GetWorldPosition(true), InterfaceControls.Icon.location, Objective.OnCompleteAction.submitSideJob, this.currentBlock.objectiveDelay, false, "", false, false);
						}
					}
				}
				else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.submitToPoster)
				{
					if (!this.triggerHandIn)
					{
						JobPreset.DialogReference dialogReference7 = this.preset.dialogReferences.Find((JobPreset.DialogReference item) => item.name.ToLower() == this.currentBlock.dialogReference.ToLower());
						if (dialogReference7 != null)
						{
							this.triggerHandIn = true;
							this.AddDialogOption(this.poster, Evidence.DataKey.photo, dialogReference7.dialog, null);
							Objective.ObjectiveTrigger objectiveTrigger16 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.onDialogSuccess, dialogReference7.dialog.name, false, 0f, null, null, null, null, null, null, this, "", false, default(Vector3));
							string name16 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger18 = objectiveTrigger16;
							bool usePointer16 = false;
							string chapterString = this.phase.ToString();
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name16, trigger18, usePointer16, default(Vector3), InterfaceControls.Icon.citizen, Objective.OnCompleteAction.submitSideJob, objectiveDelay, false, chapterString, false, false);
						}
						else
						{
							Game.LogError("Could not find job dialog index " + this.currentBlock.dialogReference, 2);
						}
					}
				}
				else if (this.currentBlock.elementType == SideMissionIntroPreset.SideMissionElementType.telephoneSubmission && !this.triggerHandIn)
				{
					JobPreset.DialogReference dialogReference8 = this.preset.dialogReferences.Find((JobPreset.DialogReference item) => item.name.ToLower() == this.currentBlock.dialogReference.ToLower());
					if (dialogReference8 != null)
					{
						this.thisCase.ValidationCheck();
						if (this.thisCase != null && this.thisCase.handInValid)
						{
							this.GenerateFakeNumber();
							TelephoneController.Instance.AddFakeNumber(this.fakeNumber, new TelephoneController.CallSource(TelephoneController.CallType.player, dialogReference8.dialog, this, InteractionController.ConversationType.normal));
							TelephoneController.Instance.fakeTelephoneDictionary[this.fakeNumber].dialogGreeting = dialogReference8.dialog;
							TelephoneController.Instance.fakeTelephoneDictionary[this.fakeNumber].dialog = dialogReference8.dialog.name;
							Objective.ObjectiveTrigger objectiveTrigger17 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.makeCall, this.fakeNumber.ToString(), false, 0f, null, null, null, null, null, null, this, "", false, default(Vector3));
							string name17 = this.currentBlock.name;
							Objective.ObjectiveTrigger trigger19 = objectiveTrigger17;
							bool usePointer17 = false;
							string chapterString = this.phase.ToString();
							float objectiveDelay = this.currentBlock.objectiveDelay;
							this.AddObjective(name17, trigger19, usePointer17, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.submitSideJob, objectiveDelay, false, chapterString, false, false);
						}
						else
						{
							Game.LogError("Cannot progress as case hand in is not valid", 2);
						}
					}
					else
					{
						Game.LogError("Could not find job dialog reference " + this.currentBlock.dialogReference + " in " + this.preset.name, 2);
					}
				}
			}
			if (flag)
			{
				this.DisplayResolveObjectivesCheck();
			}
		}
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x000F9108 File Offset: 0x000F7308
	public void GenerateHidingLocation()
	{
		List<FurnitureLocation> list = new List<FurnitureLocation>();
		foreach (NewRoom newRoom in CityData.Instance.roomDirectory)
		{
			if (newRoom.nodes.Count > 0 && newRoom.entrances.Count > 0 && newRoom.gameLocation.IsPublicallyOpen(true))
			{
				using (List<FurnitureLocation>.Enumerator enumerator2 = newRoom.individualFurniture.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						FurnitureLocation f = enumerator2.Current;
						if (GameplayControls.Instance.secretLocationFurniture.Contains(f.furniture) && !newRoom.individualFurniture.Exists((FurnitureLocation item) => item.furniture == f.furniture && item != f))
						{
							list.Add(f);
						}
					}
				}
			}
		}
		if (list.Count > 0)
		{
			FurnitureLocation furnitureLocation = list[Toolbox.Instance.Rand(0, list.Count, false)];
			this.secretLocationFurniture = furnitureLocation.id;
			this.secretLocationNode = furnitureLocation.anchorNode.nodeCoord;
		}
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x000F9268 File Offset: 0x000F7468
	public void OnPlayerCall()
	{
		Game.Log("Jobs: On player active call...", 2);
		if (Player.Instance.activeCall != null && this.gooseChaseCall != null)
		{
			if (Player.Instance.activeCall.from == this.gooseChaseCall.from && Player.Instance.activeCall.to == this.gooseChaseCall.to)
			{
				this.OnGooseChaseSuccess();
				this.OnGooseChaseEnd();
				return;
			}
			Game.Log("Jobs: Active call is not goose chase call", 2);
		}
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnGooseChaseCallTriggered()
	{
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x000F92E4 File Offset: 0x000F74E4
	public virtual void OnGooseChaseSuccess()
	{
		TelephoneController.Instance.OnPlayerCall -= this.OnPlayerCall;
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x000F92FC File Offset: 0x000F74FC
	public virtual void OnGooseChaseEnd()
	{
		TelephoneController.Instance.OnPlayerCall -= this.OnPlayerCall;
		if (this.gooseChaseFromPhone == 0)
		{
			Interactable interactable = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(this.gooseChaseFromPhone, ref interactable))
			{
				interactable.SetCustomState1(false, null, true, false, false);
			}
		}
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x00003EEE File Offset: 0x000020EE
	public virtual Human GetExtraPerson1()
	{
		return null;
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x000F9350 File Offset: 0x000F7550
	public virtual void SubmitCase()
	{
		if (this.thisCase != null)
		{
			if (!GameplayController.Instance.caseProcessing.ContainsKey(this.thisCase))
			{
				GameplayController.Instance.caseProcessing.Add(this.thisCase, SessionData.Instance.gameTime);
			}
			else
			{
				GameplayController.Instance.caseProcessing[this.thisCase] = SessionData.Instance.gameTime;
			}
			this.thisCase.SetStatus(Case.CaseStatus.submitted, true);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Case Submitted", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.resolve, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x000F9404 File Offset: 0x000F7604
	public Interactable SpawnJobItem(InteractablePreset spawnItem, JobPreset.JobSpawnWhere spawnWhere, JobPreset.LeadCitizen spawnBelongsTo, JobPreset.LeadCitizen spawnWriter, JobPreset.LeadCitizen spawnReceiver, int security, InteractablePreset.OwnedPlacementRule ownedRule, int priority, JobPreset.JobTag itemTag, bool tryFindExisting)
	{
		Human belongsTo = null;
		Human writer = null;
		Human receiver = null;
		if (spawnBelongsTo == JobPreset.LeadCitizen.poster)
		{
			belongsTo = this.poster;
		}
		else if (spawnBelongsTo == JobPreset.LeadCitizen.purp)
		{
			belongsTo = this.purp;
		}
		else if (spawnBelongsTo == JobPreset.LeadCitizen.purpsParamour)
		{
			belongsTo = this.purp.paramour;
		}
		if (spawnWriter == JobPreset.LeadCitizen.poster)
		{
			writer = this.poster;
		}
		else if (spawnWriter == JobPreset.LeadCitizen.purp)
		{
			writer = this.purp;
		}
		else if (spawnWriter == JobPreset.LeadCitizen.purpsParamour)
		{
			writer = this.purp.paramour;
		}
		if (spawnReceiver == JobPreset.LeadCitizen.poster)
		{
			receiver = this.poster;
		}
		else if (spawnReceiver == JobPreset.LeadCitizen.purp)
		{
			receiver = this.purp;
		}
		else if (spawnReceiver == JobPreset.LeadCitizen.purpsParamour)
		{
			receiver = this.purp.paramour;
		}
		Interactable interactable = null;
		List<Interactable.Passed> list = new List<Interactable.Passed>();
		list.Add(new Interactable.Passed(Interactable.PassedVarType.jobID, (float)this.jobID, null));
		list.Add(new Interactable.Passed(Interactable.PassedVarType.jobTag, (float)itemTag, null));
		NewGameLocation gameLocation = this.GetGameLocation(spawnWhere);
		NewNode placeClosestTo = null;
		if (spawnWhere == JobPreset.JobSpawnWhere.nearbyGooseChase && this.chosenGooseChasePhone != null)
		{
			placeClosestTo = this.chosenGooseChasePhone.node;
		}
		if (gameLocation != null)
		{
			if (tryFindExisting)
			{
				interactable = this.FindExisting(spawnItem, gameLocation, belongsTo, writer, receiver, itemTag);
			}
			if (interactable == null)
			{
				FurnitureLocation furnitureLocation;
				interactable = gameLocation.PlaceObject(spawnItem, belongsTo, writer, receiver, out furnitureLocation, list, true, security, ownedRule, priority, null, false, null, null, placeClosestTo, "", false);
			}
		}
		else
		{
			Game.Log(string.Concat(new string[]
			{
				"Jobs: Unable to spawn item ",
				spawnItem.name,
				" for job ",
				this.presetStr,
				": Missing location!"
			}), 2);
		}
		if (interactable != null)
		{
			string[] array = new string[13];
			array[0] = "Jobs: Successfully spawned item ";
			array[1] = interactable.name;
			array[2] = " ";
			array[3] = interactable.id.ToString();
			array[4] = " (";
			array[5] = itemTag.ToString();
			array[6] = ") for job ";
			array[7] = this.presetStr;
			array[8] = " at ";
			int num = 9;
			Vector3 wPos = interactable.wPos;
			array[num] = wPos.ToString();
			array[10] = " (";
			array[11] = interactable.node.room.name;
			array[12] = ")";
			Game.Log(string.Concat(array), 2);
			if (!this.activeJobItems.ContainsKey(itemTag))
			{
				this.activeJobItems.Add(itemTag, interactable);
			}
		}
		else
		{
			string[] array2 = new string[6];
			array2[0] = "Jobs: Unable to spawn item ";
			array2[1] = spawnItem.name;
			array2[2] = " for job ";
			array2[3] = this.presetStr;
			array2[4] = " at location ";
			int num2 = 5;
			NewGameLocation newGameLocation = gameLocation;
			array2[num2] = ((newGameLocation != null) ? newGameLocation.ToString() : null);
			Game.Log(string.Concat(array2), 2);
		}
		return interactable;
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x000F968C File Offset: 0x000F788C
	public Interactable FindExisting(InteractablePreset what, NewGameLocation location, Human belongsTo, Human writer, Human receiver, JobPreset.JobTag itemTag)
	{
		Interactable interactable = null;
		foreach (NewRoom newRoom in location.rooms)
		{
			foreach (NewNode newNode in newRoom.nodes)
			{
				foreach (Interactable interactable2 in newNode.interactables)
				{
					if (interactable2.preset == what && interactable2.belongsTo == belongsTo && interactable2.writer == writer && interactable2.reciever == receiver)
					{
						if (interactable.pv != null)
						{
							if (interactable.pv.Exists((Interactable.Passed item) => item.varType == Interactable.PassedVarType.jobID))
							{
								continue;
							}
							if (interactable.pv.Exists((Interactable.Passed item) => item.varType == Interactable.PassedVarType.jobTag))
							{
								continue;
							}
						}
						interactable = interactable2;
						interactable.pv.Add(new Interactable.Passed(Interactable.PassedVarType.jobID, (float)this.jobID, null));
						interactable.pv.Add(new Interactable.Passed(Interactable.PassedVarType.jobTag, (float)itemTag, null));
						interactable.jobParent = this;
						Game.Log(string.Concat(new string[]
						{
							"Jobs: Found item ",
							interactable.GetName(),
							" ",
							interactable.id.ToString(),
							" for job ",
							this.presetStr,
							" ",
							this.jobID.ToString()
						}), 2);
						break;
					}
				}
				if (interactable != null)
				{
					break;
				}
			}
			if (interactable != null)
			{
				break;
			}
		}
		return interactable;
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x000F98D8 File Offset: 0x000F7AD8
	public void SetJobState(SideJob.JobState newState, bool forceUpdate = false)
	{
		if (this.state != newState || forceUpdate)
		{
			this.state = newState;
			Game.Log(string.Concat(new string[]
			{
				"Jobs: Set job ",
				this.preset.name,
				" ",
				this.jobID.ToString(),
				" state to ",
				this.state.ToString()
			}), 2);
			if (this.state == SideJob.JobState.generated || this.state == SideJob.JobState.posted)
			{
				SideJobController.JobTracking jobTracking = SideJobController.Instance.jobTracking.Find((SideJobController.JobTracking item) => item.preset == this.preset);
				jobTracking.endedJobs.Remove(this);
				if (!jobTracking.activeJobs.Contains(this))
				{
					jobTracking.activeJobs.Add(this);
				}
				if (this.motive.posterIsExemptFromPostingOtherJobs)
				{
					SideJobController.Instance.AddExemptFromPostersJob(this.poster, this);
				}
				if (this.motive.posterIsExemptFromPurpingOtherJobs)
				{
					SideJobController.Instance.AddExemptFromPurpJob(this.poster, this);
				}
				if (this.motive.purpIsExemptFromPostingOtherJobs)
				{
					SideJobController.Instance.AddExemptFromPostersJob(this.purp, this);
				}
				if (this.motive.purpIsExemptFromPurpingOtherJobs)
				{
					SideJobController.Instance.AddExemptFromPurpJob(this.purp, this);
					return;
				}
			}
			else if (this.state == SideJob.JobState.ended)
			{
				SideJobController.JobTracking jobTracking2 = SideJobController.Instance.jobTracking.Find((SideJobController.JobTracking item) => item.preset == this.preset);
				jobTracking2.activeJobs.Remove(this);
				if (!jobTracking2.endedJobs.Contains(this))
				{
					jobTracking2.endedJobs.Add(this);
				}
				if (this.motive.posterIsExemptFromPostingOtherJobs)
				{
					SideJobController.Instance.RemoveExemptFromPosters(this.poster, this);
				}
				if (this.motive.posterIsExemptFromPurpingOtherJobs)
				{
					SideJobController.Instance.RemoveExemptFromPurps(this.poster, this);
				}
				if (this.motive.purpIsExemptFromPostingOtherJobs)
				{
					SideJobController.Instance.RemoveExemptFromPosters(this.purp, this);
				}
				if (this.motive.purpIsExemptFromPurpingOtherJobs)
				{
					SideJobController.Instance.RemoveExemptFromPurps(this.purp, this);
				}
			}
		}
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x000F9AF4 File Offset: 0x000F7CF4
	public void SetupNonSerializedData()
	{
		this.phaseChange = -1;
		if (this.activeJobItems == null)
		{
			this.activeJobItems = new Dictionary<JobPreset.JobTag, Interactable>();
		}
		if (this.objectiveReference == null)
		{
			this.objectiveReference = new Dictionary<string, List<Objective>>();
		}
		if (!CityData.Instance.savableInteractableDictionary.TryGetValue(this.postID, ref this.post))
		{
			Game.Log("Misc Error: Unable to find post for job: " + this.postID.ToString(), 2);
		}
		Toolbox.Instance.LoadDataFromResources<JobPreset>(this.presetStr, out this.preset);
		Toolbox.Instance.LoadDataFromResources<MotivePreset>(this.motiveStr, out this.motive);
		CityData.Instance.GetHuman(this.posterID, out this.poster, true);
		CityData.Instance.GetHuman(this.purpID, out this.purp, true);
		foreach (SideJob.AddedDialog addedDialog in this.dialog)
		{
			Human human = addedDialog.GetHuman();
			DialogPreset dialogPreset = addedDialog.GetDialog();
			NewRoom room = addedDialog.GetRoom();
			if (human != null && dialogPreset != null)
			{
				addedDialog.option = human.evidenceEntry.AddDialogOption(addedDialog.key, dialogPreset, this, room, true);
			}
		}
		if (this.thisCase == null && this.caseID > -1)
		{
			this.thisCase = CasePanelController.Instance.activeCases.Find((Case item) => item.id == this.caseID);
		}
		if (this.gooseChasePhone != 0 && !CityData.Instance.savableInteractableDictionary.TryGetValue(this.gooseChasePhone, ref this.chosenGooseChasePhone))
		{
			Game.LogError("Jobs: Unable to retrieve goose chance phone at " + this.gooseChasePhone.ToString() + ", attempting to find outside of dictionary...", 2);
			this.chosenGooseChasePhone = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.gooseChasePhone);
		}
		if (this.meetingPoint != 0 && !CityData.Instance.savableInteractableDictionary.TryGetValue(this.meetingPoint, ref this.chosenMeetingPoint))
		{
			Game.LogError("Jobs: Unable to retrieve meeting point at " + this.meetingPoint.ToString() + ", attempting to find outside of dictionary...", 2);
			this.chosenMeetingPoint = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.meetingPoint);
		}
		if (this.intro != null && this.intro.Length > 0)
		{
			foreach (JobPreset.IntroConfig introConfig in this.preset.compatibleIntros)
			{
				if (introConfig.preset.name.ToLower() == this.intro.ToLower())
				{
					this.chosenIntro = introConfig;
					break;
				}
			}
			if (this.chosenIntro == null)
			{
				Game.LogError("Unable to get intro " + this.intro, 2);
			}
		}
		if (this.handIn != null && this.handIn.Length > 0)
		{
			foreach (JobPreset.HandInConfig handInConfig in this.preset.compatibleHandIns)
			{
				if (handInConfig.preset.name.ToLower() == this.handIn.ToLower())
				{
					this.chosenHandIn = handInConfig;
					break;
				}
			}
			if (this.chosenHandIn == null)
			{
				Game.LogError("Unable to get hand in " + this.handIn, 2);
			}
		}
		this.GenerateFakeNumber();
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x000F9EA0 File Offset: 0x000F80A0
	public virtual void Complete()
	{
		this.OnRewarded();
		GameplayController.Instance.SetJobDifficultyLevel(Mathf.Max(GameplayController.Instance.jobDifficultyLevel, this.preset.GetDifficultyValue()));
		this.End();
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x000F9ED4 File Offset: 0x000F80D4
	public virtual void End()
	{
		TelephoneController.Instance.RemoveFakeNumber(this.fakeNumber);
		List<Objective> list = new List<Objective>();
		foreach (KeyValuePair<string, List<Objective>> keyValuePair in this.objectiveReference)
		{
			list.AddRange(keyValuePair.Value);
		}
		foreach (Objective objective in list)
		{
			if (!objective.isComplete)
			{
				foreach (Objective.ObjectiveTrigger objectiveTrigger in objective.queueElement.triggers)
				{
					if (!objectiveTrigger.triggered && objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.onCompleteJob)
					{
						objectiveTrigger.Trigger(true);
					}
				}
				objective.CheckingLoop();
			}
		}
		foreach (Objective objective2 in list)
		{
			if (!objective2.isComplete)
			{
				objective2.Cancel();
			}
		}
		for (int i = 0; i < this.dialog.Count; i++)
		{
			SideJob.AddedDialog addedDialog = this.dialog[i];
			Human human = addedDialog.GetHuman();
			if (human != null && addedDialog.option != null)
			{
				List<Evidence.DataKey> list2 = new List<Evidence.DataKey>();
				List<EvidenceWitness.DialogOption> list3 = new List<EvidenceWitness.DialogOption>();
				foreach (KeyValuePair<Evidence.DataKey, List<EvidenceWitness.DialogOption>> keyValuePair2 in human.evidenceEntry.dialogOptions)
				{
					foreach (EvidenceWitness.DialogOption dialogOption in keyValuePair2.Value)
					{
						if (addedDialog.option == dialogOption)
						{
							list2.Add(keyValuePair2.Key);
							list3.Add(dialogOption);
						}
					}
				}
				for (int j = 0; j < list2.Count; j++)
				{
					human.evidenceEntry.dialogOptions[list2[j]].Remove(list3[j]);
				}
			}
		}
		this.dialog.Clear();
		if (this.post != null)
		{
			this.post.RemoveFromPlacement();
		}
		foreach (KeyValuePair<JobPreset.JobTag, Interactable> keyValuePair3 in this.activeJobItems)
		{
			if (keyValuePair3.Value != null)
			{
				keyValuePair3.Value.MarkAsTrash(true, false, 0f);
			}
		}
		this.SetJobState(SideJob.JobState.ended, false);
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x000FA1F0 File Offset: 0x000F83F0
	public virtual void OnRewarded()
	{
		if (this.rewardSyncDisk != null && this.rewardSyncDisk.Length > 0)
		{
			SyncDiskPreset syncDiskPreset = Toolbox.Instance.allSyncDisks.Find((SyncDiskPreset item) => item.name == this.rewardSyncDisk);
			if (syncDiskPreset != null)
			{
				bool flag = false;
				Interactable interactable = null;
				Vector3 worldPos = Player.Instance.transform.position + new Vector3(Toolbox.Instance.Rand(-0.2f, 0.2f, false), 0f, Toolbox.Instance.Rand(-0.2f, 0.2f, false));
				if (this.preset.physicalRewardLocation == JobPreset.RewardLocation.postersMailbox)
				{
					Interactable mailbox = Toolbox.Instance.GetMailbox(this.poster);
					if (mailbox != null)
					{
						mailbox.SetLockedState(false, null, false, true);
						FurnitureLocation furnitureLocation;
						interactable = mailbox.node.gameLocation.PlaceObject(syncDiskPreset.interactable, this.poster, this.poster, Player.Instance, out furnitureLocation, null, true, 0, InteractablePreset.OwnedPlacementRule.ownedOnly, 3, syncDiskPreset, false, null, null, null, "", true);
						if (interactable != null)
						{
							flag = true;
							interactable.SetOwner(Player.Instance, true);
							Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemInInventory, "", false, 0f, null, interactable, null, null, null, null, null, "", false, default(Vector3));
							this.AddObjective("Pick up your reward " + this.preset.physicalRewardLocation.ToString(), trigger, true, interactable.GetWorldPosition(true), InterfaceControls.Icon.star, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
						}
					}
					else
					{
						Game.LogError("Jobs: Unable to locate poster's mailbox: " + this.poster.name, 2);
					}
				}
				else if (this.preset.physicalRewardLocation == JobPreset.RewardLocation.playersMailbox)
				{
					Interactable mailbox2 = Toolbox.Instance.GetMailbox(Player.Instance);
					if (mailbox2 != null)
					{
						mailbox2.SetLockedState(false, null, false, true);
						FurnitureLocation furnitureLocation;
						interactable = mailbox2.node.gameLocation.PlaceObject(syncDiskPreset.interactable, this.poster, this.poster, Player.Instance, out furnitureLocation, null, true, 0, InteractablePreset.OwnedPlacementRule.ownedOnly, 3, syncDiskPreset, false, null, null, null, "", true);
						if (interactable != null)
						{
							flag = true;
							interactable.SetOwner(Player.Instance, true);
							Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemInInventory, "", false, 0f, null, interactable, null, null, null, null, null, "", false, default(Vector3));
							this.AddObjective("Pick up your reward " + this.preset.physicalRewardLocation.ToString(), trigger2, true, interactable.GetWorldPosition(true), InterfaceControls.Icon.star, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
						}
					}
					else
					{
						Game.LogError("Jobs: Unable to locate poster's mailbox: " + this.poster.name, 2);
					}
				}
				if (this.preset.physicalRewardLocation == JobPreset.RewardLocation.cityHallDesk || interactable == null)
				{
					Interactable closestHandIn = this.thisCase.GetClosestHandIn();
					if (closestHandIn != null && closestHandIn.furnitureParent != null)
					{
						List<FurniturePreset.SubObject> list = closestHandIn.furnitureParent.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset.name == "MissionReward");
						if (list.Count > 0)
						{
							List<FurniturePreset.SubObject> list2 = new List<FurniturePreset.SubObject>();
							using (List<FurniturePreset.SubObject>.Enumerator enumerator = list.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									FurniturePreset.SubObject i = enumerator.Current;
									if (!closestHandIn.furnitureParent.spawnedInteractables.Exists((Interactable item) => item.subObject == i))
									{
										list2.Add(i);
									}
								}
							}
							FurniturePreset.SubObject subObject;
							if (list2.Count > 0)
							{
								subObject = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
							}
							else
							{
								subObject = list[Toolbox.Instance.Rand(0, list.Count, false)];
							}
							interactable = InteractableCreator.Instance.CreateFurnitureSpawnedInteractable(syncDiskPreset.interactable, closestHandIn.furnitureParent, subObject, Player.Instance, null, null, null, null, syncDiskPreset, "");
							if (interactable != null)
							{
								flag = true;
								interactable.SetOwner(Player.Instance, true);
								Objective.ObjectiveTrigger trigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemInInventory, "", false, 0f, null, interactable, null, null, null, null, null, "", false, default(Vector3));
								this.AddObjective("Pick up your reward " + JobPreset.RewardLocation.cityHallDesk.ToString(), trigger3, true, interactable.GetWorldPosition(true), InterfaceControls.Icon.star, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
							}
						}
					}
				}
				if (interactable == null)
				{
					interactable = InteractableCreator.Instance.CreateWorldInteractable(syncDiskPreset.interactable, Player.Instance, null, null, worldPos, Vector3.zero, null, syncDiskPreset, "");
					if (interactable != null && !flag)
					{
						interactable.SetOwner(Player.Instance, true);
						interactable.SetSpawnPositionRelevent(false);
						if (!FirstPersonItemController.Instance.PickUpItem(interactable, true, false, true, true, true))
						{
							interactable.ForcePhysicsActive(false, false, default(Vector3), 2, false);
						}
					}
				}
				if (interactable != null && interactable.inInventory == null)
				{
					Objective.ObjectiveTrigger trigger4 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemInInventory, "", false, 0f, null, interactable, null, null, null, null, null, "", false, default(Vector3));
					if (!flag)
					{
						this.AddObjective("Pick up your reward", trigger4, true, interactable.GetWorldPosition(true), InterfaceControls.Icon.star, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
					}
				}
			}
		}
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x000FA754 File Offset: 0x000F8954
	public virtual void PostJob()
	{
		if (this.poster == null)
		{
			Game.LogError("Cannot post job " + this.presetStr + "  without poster! Poster ID: " + this.posterID.ToString(), 2);
			return;
		}
		Game.Log(string.Concat(new string[]
		{
			"Jobs: Posting job ",
			this.presetStr,
			" (Boards: ",
			CityData.Instance.jobBoardsDirectory.Count.ToString(),
			") by poster ",
			this.poster.name
		}), 2);
		FurnitureLocation furnitureLocation = null;
		List<FurnitureLocation> list = new List<FurnitureLocation>();
		foreach (FurnitureLocation furnitureLocation2 in CityData.Instance.jobBoardsDirectory)
		{
			if (furnitureLocation2.spawnedInteractables.FindAll((Interactable item) => item.jobParent != null).Count <= 0)
			{
				list.Add(furnitureLocation2);
				float num = 0f;
				if (this.poster.home != null)
				{
					num = Vector2.Distance(furnitureLocation2.anchorNode.tile.cityTile.cityCoord, this.poster.home.building.cityTile.cityCoord);
				}
				int num2 = Mathf.Clamp(4 - Mathf.RoundToInt(num), 0, 4);
				for (int i = 0; i < num2; i++)
				{
					list.Add(furnitureLocation2);
				}
			}
		}
		if (list.Count > 0)
		{
			furnitureLocation = list[Toolbox.Instance.Rand(0, list.Count, false)];
		}
		if (furnitureLocation == null)
		{
			Game.Log("Jobs: No empty job boards found. Using a populated one...", 2);
			foreach (FurnitureLocation furnitureLocation3 in CityData.Instance.jobBoardsDirectory)
			{
				if (furnitureLocation3.spawnedInteractables.FindAll((Interactable item) => item.jobParent != null).Count < 5)
				{
					list.Add(furnitureLocation3);
					float num3 = 0f;
					if (this.poster.home != null)
					{
						num3 = Vector2.Distance(furnitureLocation3.anchorNode.tile.cityTile.cityCoord, this.poster.home.building.cityTile.cityCoord);
					}
					int num4 = Mathf.Clamp(4 - Mathf.RoundToInt(num3), 0, 4);
					for (int j = 0; j < num4; j++)
					{
						list.Add(furnitureLocation3);
					}
				}
			}
			if (list.Count > 0)
			{
				furnitureLocation = list[Toolbox.Instance.Rand(0, list.Count, false)];
			}
		}
		if (furnitureLocation == null)
		{
			Game.Log("Jobs: No empty job boards found. Picking a random city-wide one (" + CityData.Instance.jobBoardsDirectory.Count.ToString() + ")", 2);
			if (CityData.Instance.jobBoardsDirectory.Count > 0)
			{
				furnitureLocation = CityData.Instance.jobBoardsDirectory[Toolbox.Instance.Rand(0, CityData.Instance.jobBoardsDirectory.Count, false)];
			}
		}
		if (!this.postImmediately)
		{
			this.poster.ai.CreateNewGoal(RoutineControls.Instance.postJob, 0f, 0f, null, null, furnitureLocation.anchorNode.gameLocation, null, null, -2).jobID = this.jobID;
			this.poster.ai.AITick(false, false);
			Game.Log("Jobs: Citizen " + this.poster.name + " will manually post job at " + furnitureLocation.anchorNode.gameLocation.name, 2);
			return;
		}
		List<FurniturePreset.SubObject> list2 = new List<FurniturePreset.SubObject>();
		for (int k = 0; k < furnitureLocation.furniture.subObjects.Count; k++)
		{
			FurniturePreset.SubObject so = furnitureLocation.furniture.subObjects[k];
			if (this.preset.jobPosting.subObjectClasses.Contains(so.preset) && !furnitureLocation.spawnedInteractables.Exists((Interactable item) => item.subObject == so))
			{
				list2.Add(so);
			}
		}
		if (list2.Count > 0)
		{
			FurniturePreset.SubObject subObject = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
			List<Interactable.Passed> list3 = new List<Interactable.Passed>();
			list3.Add(new Interactable.Passed(Interactable.PassedVarType.jobID, (float)this.jobID, null));
			this.post = InteractableCreator.Instance.CreateFurnitureSpawnedInteractable(this.preset.jobPosting, furnitureLocation, subObject, this.poster, this.poster, null, list3, null, null, this.preset.startingScenarios[this.startingScenario].dds);
			this.postID = this.post.id;
			Game.Log("Jobs: Immediately posted job at " + furnitureLocation.anchorNode.gameLocation.name, 2);
			this.SetJobState(SideJob.JobState.posted, false);
			return;
		}
		Game.Log("Jobs: Placement pool empty!", 2);
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x000FACC8 File Offset: 0x000F8EC8
	public virtual void AcceptJob()
	{
		if (this.accepted || CasePanelController.Instance.activeCases.Count >= GameplayControls.Instance.maxCases)
		{
			Game.LogError("Too many active cases to accept side job " + this.preset.name, 2);
			return;
		}
		Game.Log("Jobs: Job " + this.preset.name + " accepted by player!", 2);
		List<JobPreset.StartingLead> list = new List<JobPreset.StartingLead>();
		if (this.startingScenario > -1 && this.preset.startingScenarios.Count > this.startingScenario)
		{
			list = this.preset.startingScenarios[this.startingScenario].leads;
		}
		else
		{
			Game.LogError("Unable to get starting lead at index " + this.startingScenario.ToString() + " for job " + this.preset.name, 2);
		}
		this.thisCase = CasePanelController.Instance.CreateNewCase(Case.CaseType.sideJob, Case.CaseStatus.handInNotCollected, true, Strings.Get("missions.postings", this.preset.caseName, Strings.Casing.asIs, false, false, false, null));
		if (this.thisCase != null)
		{
			this.accepted = true;
			this.caseID = this.thisCase.id;
			this.thisCase.job = this;
			this.thisCase.jobReference = this.jobID;
			this.ApplyLeads(ref list);
			CasePanelController.Instance.PinToCasePanel(CasePanelController.Instance.activeCase, this.post.evidence, Evidence.DataKey.name, true, default(Vector2), false);
			this.thisCase.resolveQuestions = new List<Case.ResolveQuestion>(this.resolveQuestions);
			this.resolveQuestions.Clear();
			this.UpdateResolveAnswers();
			this.SetHandIn();
			SessionData.Instance.TutorialTrigger("sidejobs", false);
			return;
		}
		Game.LogError("Unable to create new case for side job " + this.preset.name, 2);
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x000FAEA4 File Offset: 0x000F90A4
	public virtual void SetHandIn()
	{
		this.thisCase.handIn.Clear();
		if (this.chosenHandIn.preset.postersDoor)
		{
			foreach (NewNode.NodeAccess nodeAccess in this.poster.home.entrances)
			{
				if (nodeAccess.door != null && nodeAccess.door.peekInteractable != null)
				{
					this.thisCase.handIn.Add(-nodeAccess.door.wall.id);
				}
			}
		}
		if (this.chosenHandIn.preset.cityHall)
		{
			foreach (Interactable interactable in CityData.Instance.caseTrays)
			{
				this.thisCase.handIn.Add(interactable.id);
			}
		}
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x000FAFC0 File Offset: 0x000F91C0
	public virtual void AddObjective(string entryRef, Objective.ObjectiveTrigger trigger, bool usePointer = false, Vector3 pointerPosition = default(Vector3), InterfaceControls.Icon useIcon = InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction onCompleteAction = Objective.OnCompleteAction.nextChapterPart, float delay = 0f, bool removePrevious = false, string chapterString = "", bool isSilent = false, bool allowCrouchPromt = false)
	{
		if (this.thisCase != null)
		{
			Game.Log("Jobs: Job " + this.jobID.ToString() + " Adding objective: " + entryRef, 2);
			this.thisCase.AddObjective(entryRef, trigger, usePointer, pointerPosition, useIcon, onCompleteAction, delay, removePrevious, chapterString, isSilent, allowCrouchPromt, this, false, false, true);
			return;
		}
		Game.LogError(string.Concat(new string[]
		{
			"Jobs: Trying to create objective ",
			entryRef,
			" for job ",
			this.jobID.ToString(),
			" but no case is assigned to the job!"
		}), 2);
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x000FB054 File Offset: 0x000F9254
	public virtual void OnObjectiveChange()
	{
		if (this.OnObjectivesChanged != null)
		{
			this.OnObjectivesChanged();
		}
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x000FB06C File Offset: 0x000F926C
	public virtual void AddDialogOption(Human person, Evidence.DataKey key, DialogPreset newPreset, NewRoom roomRef = null)
	{
		if (person.evidenceEntry.AddDialogOption(key, newPreset, this, roomRef, true) != null)
		{
			SideJob.AddedDialog addedDialog = new SideJob.AddedDialog();
			addedDialog.humanID = person.humanID;
			addedDialog.dialogRef = newPreset.name;
			addedDialog.key = key;
			if (roomRef != null)
			{
				addedDialog.roomID = roomRef.roomID;
			}
			this.dialog.Add(addedDialog);
		}
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x000FB0D4 File Offset: 0x000F92D4
	public virtual void OnAcquireJobInfo(DialogPreset dialog)
	{
		Game.Log("Jobs: Acquire job info through dialog...", 2);
		string infoDialogMessage = string.Empty;
		if (dialog != null)
		{
			AIActionPreset.AISpeechPreset aispeechPreset = dialog.responses.Find((AIActionPreset.AISpeechPreset item) => item.isSuccessful);
			if (aispeechPreset != null)
			{
				infoDialogMessage = aispeechPreset.ddsMessageID;
			}
		}
		this.OnAcquireJobInfo(infoDialogMessage);
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x000FB138 File Offset: 0x000F9338
	public virtual void OnAcquireJobInfo(string infoDialogMessage)
	{
		if (!this.knowHandInLocation)
		{
			Game.Log("Jobs: OnAcquireJobInfo", 2);
			this.knowHandInLocation = true;
			this.jobInfoDialogMsg = infoDialogMessage;
			this.thisCase.SetStatus(Case.CaseStatus.handInCollected, true);
			this.CreateAcqusitionFacts();
			this.ApplyLeads(ref this.preset.informationAcquisitionLeads);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Job information updated!", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.resolve, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			if (this.AcquireInfo != null)
			{
				this.AcquireInfo();
			}
		}
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x000FB1D8 File Offset: 0x000F93D8
	public void CreateAcqusitionFacts()
	{
		if (this.knowHandInLocation)
		{
			foreach (JobPreset.FactCreation factCreation in this.preset.createFactsOnInformationAcquisition)
			{
				Evidence evidence = this.GetEvidence(factCreation.from);
				Evidence evidence2 = this.GetEvidence(factCreation.to);
				List<Evidence.DataKey> list = null;
				List<Evidence.DataKey> list2 = null;
				if (factCreation.overrideFromKeys)
				{
					list = new List<Evidence.DataKey>();
					if (factCreation.featureKeysFromLeadPool && this.leadKeys.Count > 0)
					{
						list.AddRange(this.leadKeys);
					}
					else
					{
						list.AddRange(factCreation.fromKeys);
					}
				}
				if (factCreation.overrideToKeys)
				{
					list2 = new List<Evidence.DataKey>();
					if (factCreation.featureKeysFromLeadPoolTo && this.leadKeys.Count > 0)
					{
						list2.AddRange(this.leadKeys);
					}
					else
					{
						list2.AddRange(factCreation.toKeys);
					}
				}
				Fact fact = EvidenceCreator.Instance.CreateFact(factCreation.factPreset.name, evidence, evidence2, null, null, true, null, list, list2, false);
				if (fact != null)
				{
					fact.SetFound(true);
				}
			}
		}
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x000FB30C File Offset: 0x000F950C
	private void PickPoolLeadOptions()
	{
		bool flag = false;
		for (int i = 0; i < this.preset.leadPoolData; i++)
		{
			if (!(this.purp == null))
			{
				List<JobPreset.BasicLeadPool> list = Enumerable.ToList<JobPreset.BasicLeadPool>(Enumerable.Cast<JobPreset.BasicLeadPool>(Enum.GetValues(typeof(JobPreset.BasicLeadPool))));
				foreach (JobPreset.BasicLeadPool basicLeadPool in this.appliedBasicLeads)
				{
					if (list.Contains(basicLeadPool))
					{
						list.Remove(basicLeadPool);
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					JobPreset.BasicLeadPool basicLeadPool2 = list[j];
					if (basicLeadPool2 == JobPreset.BasicLeadPool.jobTitle || basicLeadPool2 == JobPreset.BasicLeadPool.salary)
					{
						if (this.purp.job == null || this.purp.job.employer == null)
						{
							list.RemoveAt(j);
							j--;
						}
					}
					else if (basicLeadPool2 == JobPreset.BasicLeadPool.partnerFirstName)
					{
						if (this.purp.partner == null)
						{
							list.RemoveAt(j);
							j--;
						}
					}
					else if (basicLeadPool2 == JobPreset.BasicLeadPool.partnerJobTitle)
					{
						if (this.purp.partner == null || this.purp.partner.job == null || this.purp.partner.job.employer == null)
						{
							list.RemoveAt(j);
							j--;
						}
					}
					else if (basicLeadPool2 == JobPreset.BasicLeadPool.socialClub)
					{
						if (!this.purp.groups.Exists((GroupsController.SocialGroup item) => item.GetPreset().groupType == GroupPreset.GroupType.interestGroup))
						{
							list.RemoveAt(j);
							j--;
						}
					}
					else if (basicLeadPool2 == JobPreset.BasicLeadPool.partnerSocialClub)
					{
						if (!(this.purp.partner == null))
						{
							if (this.purp.partner.groups.Exists((GroupsController.SocialGroup item) => item.GetPreset().groupType == GroupPreset.GroupType.interestGroup))
							{
								goto IL_2DA;
							}
						}
						list.RemoveAt(j);
						j--;
					}
					else if (basicLeadPool2 == JobPreset.BasicLeadPool.randomInterest)
					{
						if (!this.purp.characterTraits.Exists((Human.Trait item) => item.trait.featureInInterestPool))
						{
							list.RemoveAt(j);
							j--;
						}
					}
					else if (basicLeadPool2 == JobPreset.BasicLeadPool.randomAffliction)
					{
						if (!this.purp.characterTraits.Exists((Human.Trait item) => item.trait.featureInAfflictionPool))
						{
							list.RemoveAt(j);
							j--;
						}
					}
					else if (basicLeadPool2 == JobPreset.BasicLeadPool.notableFeatures)
					{
						if (!this.purp.characterTraits.Exists((Human.Trait item) => item.trait.name == "Affliction-ShortSighted" || item.trait.name == "Affliction-FarSighted" || item.trait.name == "Quirk-FacialHair"))
						{
							list.RemoveAt(j);
							j--;
						}
					}
					IL_2DA:;
				}
				if (list.Count > 0)
				{
					JobPreset.BasicLeadPool basicLeadPool3 = list[Toolbox.Instance.Rand(0, list.Count, false)];
					if (!flag)
					{
						List<JobPreset.BasicLeadPool> list2 = list.FindAll((JobPreset.BasicLeadPool item) => item == JobPreset.BasicLeadPool.hair || item == JobPreset.BasicLeadPool.eyeColour || item == JobPreset.BasicLeadPool.build || item == JobPreset.BasicLeadPool.fingerprint || item == JobPreset.BasicLeadPool.age || item == JobPreset.BasicLeadPool.jobTitle || item == JobPreset.BasicLeadPool.firstNameInitial || item == JobPreset.BasicLeadPool.notableFeatures);
						if (list2.Count > 0)
						{
							basicLeadPool3 = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
							flag = true;
						}
					}
					this.appliedBasicLeads.Add(basicLeadPool3);
					if (basicLeadPool3 == JobPreset.BasicLeadPool.age)
					{
						this.leadKeys.Add(Evidence.DataKey.age);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.build)
					{
						this.leadKeys.Add(Evidence.DataKey.build);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.eyeColour)
					{
						this.leadKeys.Add(Evidence.DataKey.eyes);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.fingerprint)
					{
						this.leadKeys.Add(Evidence.DataKey.fingerprints);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.firstNameInitial)
					{
						this.leadKeys.Add(Evidence.DataKey.firstNameInitial);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.hair)
					{
						this.leadKeys.Add(Evidence.DataKey.hair);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.height)
					{
						this.leadKeys.Add(Evidence.DataKey.height);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.jobTitle)
					{
						this.leadKeys.Add(Evidence.DataKey.jobTitle);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.partnerFirstName)
					{
						this.leadKeys.Add(Evidence.DataKey.partnerFirstName);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.partnerJobTitle)
					{
						this.leadKeys.Add(Evidence.DataKey.partnerJobTitle);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.randomInterest)
					{
						this.leadKeys.Add(Evidence.DataKey.randomInterest);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.shoeSize)
					{
						this.leadKeys.Add(Evidence.DataKey.shoeSize);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.socialClub)
					{
						this.leadKeys.Add(Evidence.DataKey.randomSocialClub);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.partnerSocialClub)
					{
						this.leadKeys.Add(Evidence.DataKey.partnerSocialClub);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.notableFeatures)
					{
						this.leadKeys.Add(Evidence.DataKey.glasses);
						this.leadKeys.Add(Evidence.DataKey.facialHair);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.salary)
					{
						this.leadKeys.Add(Evidence.DataKey.salary);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.bloodType)
					{
						this.leadKeys.Add(Evidence.DataKey.bloodType);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.randomAffliction)
					{
						this.leadKeys.Add(Evidence.DataKey.randomAffliction);
					}
					else if (basicLeadPool3 == JobPreset.BasicLeadPool.handwriting)
					{
						this.leadKeys.Add(Evidence.DataKey.handwriting);
					}
				}
			}
		}
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x000FB864 File Offset: 0x000F9A64
	private void ApplyLeads(ref List<JobPreset.StartingLead> leads)
	{
		foreach (JobPreset.BasicLeadPool basicLeadPool in this.appliedBasicLeads)
		{
			if (basicLeadPool == JobPreset.BasicLeadPool.age)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.age, keyTwo);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.build)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo2 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.build, keyTwo2);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.eyeColour)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo3 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.eyes, keyTwo3);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.fingerprint)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo4 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.fingerprints, keyTwo4);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.firstNameInitial)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo5 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.firstNameInitial, keyTwo5);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.hair)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo6 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.hair, keyTwo6);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.height)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo7 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.height, keyTwo7);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.jobTitle)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo8 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.jobTitle, keyTwo8);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.partnerFirstName)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo9 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.partnerFirstName, keyTwo9);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.partnerJobTitle)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo10 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.partnerJobTitle, keyTwo10);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.randomInterest)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo11 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.randomInterest, keyTwo11);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.shoeSize)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo12 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.shoeSize, keyTwo12);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.socialClub)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo13 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.randomSocialClub, keyTwo13);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.partnerSocialClub)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo14 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.partnerSocialClub, keyTwo14);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.notableFeatures)
			{
				foreach (Evidence.DataKey keyTwo15 in this.leadKeys)
				{
					this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.glasses, keyTwo15);
				}
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo16 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.facialHair, keyTwo16);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.salary)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo17 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.salary, keyTwo17);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.bloodType)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo18 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.bloodType, keyTwo18);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.randomAffliction)
			{
				using (List<Evidence.DataKey>.Enumerator enumerator2 = this.leadKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Evidence.DataKey keyTwo19 = enumerator2.Current;
						this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.randomAffliction, keyTwo19);
					}
					continue;
				}
			}
			if (basicLeadPool == JobPreset.BasicLeadPool.handwriting)
			{
				foreach (Evidence.DataKey keyTwo20 in this.leadKeys)
				{
					this.purp.evidenceEntry.MergeDataKeys(Evidence.DataKey.handwriting, keyTwo20);
				}
			}
		}
		foreach (JobPreset.StartingLead startingLead in leads)
		{
			Evidence evidence = this.GetEvidence(startingLead.leadEvidence);
			if (evidence != null)
			{
				foreach (Evidence.DataKey dataKey in startingLead.mergeKeys)
				{
					foreach (Evidence.DataKey dataKey2 in startingLead.mergeKeys)
					{
						if (dataKey != dataKey2)
						{
							evidence.MergeDataKeys(dataKey, dataKey2);
						}
					}
				}
				List<Evidence.DataKey> list = new List<Evidence.DataKey>(startingLead.keys);
				if (startingLead.useKeyFromLeadPool && this.leadKeys.Count > 0)
				{
					list.AddRange(this.leadKeys);
				}
				foreach (string text in startingLead.factsReveal)
				{
					Fact fact = null;
					EvidenceCitizen evidenceCitizen = evidence as EvidenceCitizen;
					if (evidenceCitizen != null && evidenceCitizen.witnessController != null && evidenceCitizen.witnessController.factDictionary.TryGetValue(text, ref fact))
					{
						if (!fact.isFound)
						{
							fact.SetFound(true);
						}
						this.post.evidence.AddFactLink(fact, list, false);
					}
				}
				foreach (Evidence.Discovery disc in startingLead.discoveryApplication)
				{
					evidence.AddDiscovery(disc);
				}
				if (startingLead.autoPin && this.thisCase != null)
				{
					CasePanelController.Instance.PinToCasePanel(this.thisCase, evidence, list, true, default(Vector2), false);
				}
			}
		}
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x000FC2D8 File Offset: 0x000FA4D8
	private Evidence GetEvidence(JobPreset.LeadEvidence lead)
	{
		Evidence result = null;
		if (lead == JobPreset.LeadEvidence.poster && this.poster != null)
		{
			result = this.poster.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purp && this.purp != null)
		{
			result = this.purp.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsParamour && this.purp != null && this.purp.paramour != null)
		{
			result = this.purp.paramour.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.postersHome && this.poster != null && this.poster.home != null)
		{
			result = this.poster.home.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsHome && this.purp != null && this.purp.home != null)
		{
			result = this.purp.home.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsParamourHome && this.purp != null && this.purp.paramour != null && this.purp.paramour.home != null)
		{
			result = this.purp.paramour.home.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.postersWorkplace && this.poster != null && this.poster.job != null && this.poster.job.employer != null && this.poster.job.employer.address != null)
		{
			result = this.poster.job.employer.address.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsWorkplace && this.purp != null && this.purp.job != null && this.purp.job.employer != null && this.purp.job.employer.address != null)
		{
			result = this.purp.job.employer.address.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsParamourWorkplace && this.purp != null && this.purp.paramour != null && this.purp.paramour.job != null && this.purp.paramour.job.employer != null && this.purp.paramour.job.employer.address != null)
		{
			result = this.purp.paramour.job.employer.address.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.postersBuilding && this.poster != null && this.poster.home != null)
		{
			result = this.poster.home.building.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsBuilding && this.purp != null && this.purp.home != null)
		{
			result = this.purp.home.building.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsParamourBuilding && this.purp != null && this.purp.paramour != null && this.purp.paramour.home != null)
		{
			result = this.purp.paramour.home.building.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.post && this.post != null && this.post.evidence != null)
		{
			result = this.post.evidence;
		}
		else if (lead == JobPreset.LeadEvidence.posterTelephone && this.poster != null && this.poster.home != null && this.poster.home.telephones != null && this.poster.home.telephones.Count > 0)
		{
			result = this.poster.home.telephones[0].telephoneEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsTelephone && this.purp != null && this.purp.home != null && this.purp.home.telephones != null && this.purp.home.telephones.Count > 0)
		{
			result = this.purp.home.telephones[0].telephoneEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsParamourTelephone && this.purp != null && this.purp.paramour != null && this.purp.paramour.home != null && this.purp.paramour.home.telephones != null && this.purp.paramour.home.telephones.Count > 0)
		{
			result = this.purp.paramour.home.telephones[0].telephoneEntry;
		}
		else if (lead == JobPreset.LeadEvidence.postersWorkplaceBuilding && this.poster != null && this.poster.job != null && this.poster.job.employer != null && this.poster.job.employer.address != null)
		{
			result = this.poster.job.employer.address.building.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsWorkplaceBuilding && this.purp != null && this.purp.job != null && this.purp.job.employer != null && this.purp.job.employer.address != null)
		{
			result = this.purp.job.employer.address.building.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.purpsParamourWorkplaceBuilding && this.purp != null && this.purp.paramour != null && this.purp.paramour.job != null && this.purp.paramour.job.employer != null && this.purp.paramour.job.employer.address != null)
		{
			result = this.purp.paramour.job.employer.address.building.evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.extraPerson1 && this.GetExtraPerson1() != null)
		{
			result = this.GetExtraPerson1().evidenceEntry;
		}
		else if (lead == JobPreset.LeadEvidence.itemA && this.GetItem(JobPreset.JobTag.A) != null)
		{
			result = this.GetItem(JobPreset.JobTag.A).evidence;
		}
		else if (lead == JobPreset.LeadEvidence.itemB && this.GetItem(JobPreset.JobTag.B) != null)
		{
			result = this.GetItem(JobPreset.JobTag.B).evidence;
		}
		else if (lead == JobPreset.LeadEvidence.itemC && this.GetItem(JobPreset.JobTag.C) != null)
		{
			result = this.GetItem(JobPreset.JobTag.C).evidence;
		}
		else if (lead == JobPreset.LeadEvidence.itemD && this.GetItem(JobPreset.JobTag.D) != null)
		{
			result = this.GetItem(JobPreset.JobTag.D).evidence;
		}
		else if (lead == JobPreset.LeadEvidence.itemE && this.GetItem(JobPreset.JobTag.E) != null)
		{
			result = this.GetItem(JobPreset.JobTag.E).evidence;
		}
		return result;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x000FCA78 File Offset: 0x000FAC78
	public virtual void GenerateResolveQuestions(bool setRewardType)
	{
		this.reward = 0;
		int num = 0;
		this.resolveQuestions.Clear();
		foreach (Case.ResolveQuestion resolveQuestion in this.preset.resolveQuestions)
		{
			if ((resolveQuestion.onlyCompatibleWithHandIns.Count <= 0 || resolveQuestion.onlyCompatibleWithHandIns.Exists((SideMissionHandInPreset item) => item.name.ToLower() == this.handIn.ToLower())) && (resolveQuestion.onlyCompatibleWithIntros.Count <= 0 || resolveQuestion.onlyCompatibleWithIntros.Exists((SideMissionIntroPreset item) => item.name.ToLower() == this.intro.ToLower())))
			{
				Case.ResolveQuestion resolveQuestion2 = new Case.ResolveQuestion();
				resolveQuestion2.name = resolveQuestion.name;
				resolveQuestion2.inputType = resolveQuestion.inputType;
				resolveQuestion2.tag = resolveQuestion.tag;
				resolveQuestion2.rewardRange = resolveQuestion.rewardRange;
				resolveQuestion2.isOptional = resolveQuestion.isOptional;
				resolveQuestion2.automaticAnswers = new List<Case.AutoCorrectAnswer>(resolveQuestion.automaticAnswers);
				resolveQuestion2.icon = resolveQuestion.icon;
				resolveQuestion2.useAlternateName = resolveQuestion.useAlternateName;
				resolveQuestion2.useName = resolveQuestion.useName;
				resolveQuestion2.displayObjective = resolveQuestion.displayObjective;
				resolveQuestion2.displayOnlyAtPhase = resolveQuestion.displayOnlyAtPhase;
				resolveQuestion2.displayAtPhase = resolveQuestion.displayAtPhase;
				resolveQuestion2.objectiveDelay = resolveQuestion.objectiveDelay;
				resolveQuestion2.target = resolveQuestion.target;
				resolveQuestion2.location = resolveQuestion.location;
				float num2 = 1f;
				if (resolveQuestion.inputType == Case.InputType.revengeObjective)
				{
					RevengeObjective revengeObjective = this.GetRevengeObjective(resolveQuestion2);
					if (revengeObjective != null)
					{
						resolveQuestion2.revengeObjective = revengeObjective.name;
						resolveQuestion2.icon = revengeObjective.icon;
						float num3 = Toolbox.Instance.Rand(0f, 1f, false);
						resolveQuestion2.revengeObjPassed = (float)(Mathf.RoundToInt(Mathf.Lerp(revengeObjective.passedNumberRange.x, revengeObjective.passedNumberRange.y, num3) / 50f) * 50);
						num2 = Mathf.Lerp(revengeObjective.rewardMultiplier.x, revengeObjective.rewardMultiplier.y, num3);
						Human human = null;
						if (resolveQuestion2.target == JobPreset.LeadCitizen.poster)
						{
							human = this.poster;
						}
						else if (resolveQuestion2.target == JobPreset.LeadCitizen.purp)
						{
							human = this.purp;
						}
						else if (resolveQuestion2.target == JobPreset.LeadCitizen.purpsParamour)
						{
							human = this.purp.paramour;
						}
						if (human != null)
						{
							resolveQuestion2.revengeObjTarget = human.humanID;
							NewGameLocation gameLocation = this.GetGameLocation(resolveQuestion2.location);
							if (gameLocation != null && gameLocation.thisAsAddress != null)
							{
								resolveQuestion2.revengeObjLoc = gameLocation.thisAsAddress.id;
							}
						}
						if (resolveQuestion2.useName == Case.RevengeObjectiveName.D0)
						{
							resolveQuestion2.name = revengeObjective.d0Name;
							Game.Log(string.Concat(new string[]
							{
								"Objective: Using D0 name for ",
								revengeObjective.name,
								" on job ",
								this.jobID.ToString(),
								": ",
								resolveQuestion2.name
							}), 2);
						}
						else if (resolveQuestion2.useName == Case.RevengeObjectiveName.D1)
						{
							resolveQuestion2.name = revengeObjective.d1Name;
							Game.Log(string.Concat(new string[]
							{
								"Objective: Using D1 name for ",
								revengeObjective.name,
								" on job ",
								this.jobID.ToString(),
								": ",
								resolveQuestion2.name
							}), 2);
						}
						else if (resolveQuestion2.useName == Case.RevengeObjectiveName.IDTarget)
						{
							resolveQuestion2.name = revengeObjective.idTargetName;
							Game.Log(string.Concat(new string[]
							{
								"Objective: Using IDTarget name for ",
								revengeObjective.name,
								" on job ",
								this.jobID.ToString(),
								": ",
								resolveQuestion2.name
							}), 2);
						}
					}
				}
				else if (resolveQuestion.inputType == Case.InputType.objective || resolveQuestion.inputType == Case.InputType.arrestPurp || resolveQuestion.inputType == Case.InputType.saveVictim)
				{
					resolveQuestion2.revengeObjective = resolveQuestion.revengeObjective;
				}
				if (this.rewardSyncDisk == null || this.rewardSyncDisk.Length <= 0)
				{
					resolveQuestion2.reward = Mathf.RoundToInt(Toolbox.Instance.VectorToRandom(resolveQuestion.rewardRange) * Game.Instance.jobRewardMultiplier * num2 * GameplayControls.Instance.sideJobDifficultyRewardMultiplier.Evaluate(this.GetDifficulty()) / 50f) * 50;
					resolveQuestion2.reward = Mathf.RoundToInt((float)resolveQuestion2.reward * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.sideJobPayModifier)));
				}
				else
				{
					resolveQuestion2.reward = 0;
				}
				resolveQuestion2.penalty = Mathf.RoundToInt(Toolbox.Instance.VectorToRandom(resolveQuestion.penaltyRange) * Game.Instance.jobPenaltyMultiplier * GameplayControls.Instance.sideJobDifficultyRewardMultiplier.Evaluate(this.GetDifficulty()) / 50f) * 50;
				this.reward += resolveQuestion2.reward;
				if (!resolveQuestion2.isOptional)
				{
					num += resolveQuestion2.reward;
				}
				this.resolveQuestions.Add(resolveQuestion2);
			}
		}
		if (setRewardType && (this.rewardSyncDisk == null || this.rewardSyncDisk.Length <= 0) && Toolbox.Instance.Rand(0f, 1f, false) > 0.7f)
		{
			List<SyncDiskPreset> list = new List<SyncDiskPreset>();
			using (List<SyncDiskPreset>.Enumerator enumerator2 = Toolbox.Instance.allSyncDisks.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SyncDiskPreset d = enumerator2.Current;
					if (!d.disabled && d.canBeSideJobReward && (this.preset.allowBlackMarketSyncDiskRewards || d.manufacturer != SyncDiskPreset.Manufacturer.BlackMarket) && this.reward >= d.price && !UpgradesController.Instance.upgrades.Exists((UpgradesController.Upgrades item) => item.preset == d) && !FirstPersonItemController.Instance.slots.Exists((FirstPersonItemController.InventorySlot item) => item.GetInteractable() != null && item.GetInteractable().syncDisk == d))
					{
						list.Add(d);
					}
				}
			}
			if (list.Count > 0)
			{
				this.rewardSyncDisk = list[Toolbox.Instance.Rand(0, list.Count, false)].name;
				foreach (Case.ResolveQuestion resolveQuestion3 in this.resolveQuestions)
				{
					if (resolveQuestion3.isOptional)
					{
						resolveQuestion3.reward = 0;
					}
				}
			}
		}
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x000FD15C File Offset: 0x000FB35C
	private RevengeObjective GetRevengeObjective(Case.ResolveQuestion resolveQ)
	{
		if (this.preset.revengeObjectives.Count <= 0)
		{
			Game.Log("Misc Error: No revenge objectives set for job " + this.preset.name, 2);
			return null;
		}
		List<RevengeObjective> list = new List<RevengeObjective>();
		foreach (RevengeObjective revengeObjective in this.preset.revengeObjectives)
		{
			if (!revengeObjective.disabled)
			{
				NewGameLocation gameLocation = this.GetGameLocation(resolveQ.location);
				if (!(gameLocation == null))
				{
					int num = 0;
					int num2 = 0;
					using (List<RevengeObjective.SpecialConditions>.Enumerator enumerator2 = revengeObjective.specialConditions.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current == RevengeObjective.SpecialConditions.mustHaveWindows && gameLocation != null)
							{
								int count = gameLocation.entrances.FindAll((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.window).Count;
							}
						}
					}
					if (revengeObjective.useTraits)
					{
						if (this.poster != null && revengeObjective.characterTraitsPoster.Count > 0)
						{
							this.poster.outfitController.GetChance(this.poster, ref revengeObjective.characterTraitsPoster, out num);
						}
						else
						{
							Game.Log("Misc Error: No poster assigned to job " + this.preset.name + ", unable to calculate trait scores for poster. " + this.jobID.ToString(), 2);
						}
						if (this.purp != null && revengeObjective.characterTraitsPurp.Count > 0)
						{
							int num3 = 0;
							this.poster.outfitController.GetChance(this.purp, ref revengeObjective.characterTraitsPurp, out num3);
							num += num3;
						}
						else
						{
							Game.Log("Misc Error: No purp assigned to job " + this.preset.name + ", unable to calculate trait scores for purp. " + this.jobID.ToString(), 2);
						}
					}
					if (revengeObjective.useHEXACO)
					{
						if (this.poster != null)
						{
							int num4 = 10 - Mathf.RoundToInt(Mathf.Abs((float)revengeObjective.feminineMasculine - this.poster.genderScale * 10f));
							float num5 = (float)(10 - Mathf.RoundToInt(Mathf.Abs((float)revengeObjective.humility - this.poster.humility * 10f)));
							int num6 = 10 - Mathf.RoundToInt(Mathf.Abs((float)revengeObjective.emotionality - this.poster.emotionality * 10f));
							int num7 = 10 - Mathf.RoundToInt(Mathf.Abs((float)revengeObjective.extraversion - this.poster.extraversion * 10f));
							int num8 = 10 - Mathf.RoundToInt(Mathf.Abs((float)revengeObjective.agreeableness - this.poster.agreeableness * 10f));
							int num9 = 10 - Mathf.RoundToInt(Mathf.Abs((float)revengeObjective.conscientiousness - this.poster.conscientiousness * 10f));
							int num10 = 10 - Mathf.RoundToInt(Mathf.Abs((float)revengeObjective.creativity - this.poster.creativity * 10f));
							num2 = Mathf.Max(Mathf.FloorToInt((num5 + (float)num6 + (float)num7 + (float)num8 + (float)num9 + (float)num10 + (float)num4) / 7f), 1);
						}
						else
						{
							Game.Log("Misc Error: No poster assigned to job " + this.preset.name + ", unable to calculate HEXACO scores for poster. " + this.jobID.ToString(), 2);
						}
					}
					int num11 = revengeObjective.baseChance + num + num2;
					for (int i = 0; i < num11; i++)
					{
						list.Add(revengeObjective);
					}
				}
			}
		}
		if (list.Count > 0)
		{
			return list[Toolbox.Instance.Rand(0, list.Count, false)];
		}
		Game.Log("Misc Error: No revenge objective was returned for side job " + this.preset.name, 2);
		return null;
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x000FD574 File Offset: 0x000FB774
	private NewGameLocation GetGameLocation(JobPreset.JobSpawnWhere spawnWhere)
	{
		NewGameLocation result = null;
		if ((spawnWhere == JobPreset.JobSpawnWhere.posterHome && this.poster != null && this.poster.home != null) || (spawnWhere == JobPreset.JobSpawnWhere.posterWork && this.poster.home != null && this.poster.job.employer == null))
		{
			result = this.poster.home;
		}
		else if (spawnWhere == JobPreset.JobSpawnWhere.posterWork && this.poster != null && this.poster.job.employer != null)
		{
			result = this.poster.job.employer.address;
			if (this.poster.job.employer.address == null && this.poster.home != null)
			{
				result = this.poster.home;
			}
		}
		else if ((spawnWhere == JobPreset.JobSpawnWhere.purpHome && this.purp != null && this.purp.home != null) || (spawnWhere == JobPreset.JobSpawnWhere.purpWork && this.purp.home != null && this.purp.job.employer == null))
		{
			result = this.purp.home;
		}
		else if (spawnWhere == JobPreset.JobSpawnWhere.purpWork && this.purp != null && this.purp.job.employer != null)
		{
			result = this.purp.job.employer.address;
			if (this.purp.job.employer.address == null && this.purp.home != null)
			{
				result = this.purp.home;
			}
		}
		else if ((spawnWhere == JobPreset.JobSpawnWhere.purpsParamourHome && this.purp.paramour != null && this.purp.paramour.home != null) || (spawnWhere == JobPreset.JobSpawnWhere.purpsParamourWork && this.purp.paramour.home != null && this.purp.paramour.job.employer == null))
		{
			result = this.purp.paramour.home;
		}
		else if (spawnWhere == JobPreset.JobSpawnWhere.purpsParamourWork && this.purp.paramour != null && this.purp.paramour.job.employer != null)
		{
			result = this.purp.paramour.job.employer.address;
			if (this.purp.paramour.job.employer.address == null && this.purp.paramour.home != null)
			{
				result = this.purp.paramour.home;
			}
		}
		else if (spawnWhere == JobPreset.JobSpawnWhere.hiddenItemPlace)
		{
			List<NewGameLocation> list = new List<NewGameLocation>();
			foreach (StreetController streetController in CityData.Instance.streetDirectory)
			{
				foreach (NewRoom newRoom in streetController.rooms)
				{
					foreach (FurnitureClusterLocation furnitureClusterLocation in newRoom.furniture)
					{
						foreach (FurnitureLocation furnitureLocation in furnitureClusterLocation.clusterList)
						{
							List<FurniturePreset.SubObject> subObjs = furnitureLocation.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == InteriorControls.Instance.sideJobHiddenItemClass);
							List<Interactable> list2 = furnitureLocation.spawnedInteractables.FindAll((Interactable item) => subObjs.Contains(item.subObject));
							int num = subObjs.Count - list2.Count;
							if (num > 0)
							{
								for (int i = 0; i < num; i++)
								{
									list.Add(streetController);
								}
							}
						}
					}
				}
			}
			foreach (NewGameLocation newGameLocation in CityData.Instance.addressDirectory)
			{
				if (newGameLocation.IsPublicallyOpen(true))
				{
					foreach (NewRoom newRoom2 in newGameLocation.rooms)
					{
						foreach (FurnitureClusterLocation furnitureClusterLocation2 in newRoom2.furniture)
						{
							foreach (FurnitureLocation furnitureLocation2 in furnitureClusterLocation2.clusterList)
							{
								List<FurniturePreset.SubObject> subObjs = furnitureLocation2.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == InteriorControls.Instance.sideJobHiddenItemClass);
								List<Interactable> list3 = furnitureLocation2.spawnedInteractables.FindAll((Interactable item) => subObjs.Contains(item.subObject));
								int num2 = subObjs.Count - list3.Count;
								if (num2 > 0)
								{
									for (int j = 0; j < num2; j++)
									{
										list.Add(newGameLocation);
									}
								}
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				result = list[Toolbox.Instance.Rand(0, list.Count, false)];
			}
		}
		else if (spawnWhere == JobPreset.JobSpawnWhere.nearbyGooseChase)
		{
			if (this.chosenGooseChasePhone != null)
			{
				foreach (NewRoom newRoom3 in this.chosenGooseChasePhone.node.gameLocation.rooms)
				{
					foreach (FurnitureClusterLocation furnitureClusterLocation3 in newRoom3.furniture)
					{
						foreach (FurnitureLocation furnitureLocation3 in furnitureClusterLocation3.clusterList)
						{
							List<FurniturePreset.SubObject> subObjs = furnitureLocation3.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == InteriorControls.Instance.sideJobHiddenItemClass);
							List<Interactable> list4 = furnitureLocation3.spawnedInteractables.FindAll((Interactable item) => subObjs.Contains(item.subObject));
							if (subObjs.Count - list4.Count > 0)
							{
								return this.chosenGooseChasePhone.node.gameLocation;
							}
						}
					}
				}
				List<NewGameLocation> list5 = new List<NewGameLocation>();
				foreach (StreetController streetController2 in CityData.Instance.streetDirectory)
				{
					foreach (NewRoom newRoom4 in streetController2.rooms)
					{
						foreach (FurnitureClusterLocation furnitureClusterLocation4 in newRoom4.furniture)
						{
							foreach (FurnitureLocation furnitureLocation4 in furnitureClusterLocation4.clusterList)
							{
								List<FurniturePreset.SubObject> subObjs = furnitureLocation4.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == InteriorControls.Instance.sideJobHiddenItemClass);
								List<Interactable> list6 = furnitureLocation4.spawnedInteractables.FindAll((Interactable item) => subObjs.Contains(item.subObject));
								int num3 = subObjs.Count - list6.Count;
								if (num3 > 0)
								{
									for (int k = 0; k < num3; k++)
									{
										list5.Add(streetController2);
									}
								}
							}
						}
					}
				}
				foreach (NewGameLocation newGameLocation2 in CityData.Instance.addressDirectory)
				{
					if (newGameLocation2.IsPublicallyOpen(true))
					{
						foreach (NewRoom newRoom5 in newGameLocation2.rooms)
						{
							foreach (FurnitureClusterLocation furnitureClusterLocation5 in newRoom5.furniture)
							{
								foreach (FurnitureLocation furnitureLocation5 in furnitureClusterLocation5.clusterList)
								{
									List<FurniturePreset.SubObject> subObjs = furnitureLocation5.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == InteriorControls.Instance.sideJobHiddenItemClass);
									List<Interactable> list7 = furnitureLocation5.spawnedInteractables.FindAll((Interactable item) => subObjs.Contains(item.subObject));
									int num4 = subObjs.Count - list7.Count;
									if (num4 > 0)
									{
										for (int l = 0; l < num4; l++)
										{
											list5.Add(newGameLocation2);
										}
									}
								}
							}
						}
					}
				}
				if (list5.Count > 0)
				{
					float num5 = float.PositiveInfinity;
					NewGameLocation result2 = null;
					foreach (NewGameLocation newGameLocation3 in list5)
					{
						if (newGameLocation3.anchorNode != null)
						{
							float num6 = Vector3.Distance(this.chosenGooseChasePhone.node.position, newGameLocation3.anchorNode.position);
							if (num6 < num5)
							{
								num5 = num6;
								result2 = newGameLocation3;
							}
						}
					}
					return result2;
				}
				return result;
			}
			Game.Log("Jobs: Cannot get nearbygoose chase location as the phone reference is null!", 2);
		}
		return result;
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x000FE214 File Offset: 0x000FC414
	private Human GetTarget(JobPreset.LeadCitizen who)
	{
		Human result = null;
		if (who == JobPreset.LeadCitizen.poster)
		{
			result = this.poster;
		}
		else if (who == JobPreset.LeadCitizen.purp)
		{
			result = this.purp;
		}
		else if (who == JobPreset.LeadCitizen.purpsParamour)
		{
			result = this.purp.paramour;
		}
		return result;
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x000FE250 File Offset: 0x000FC450
	public Interactable GetItem(JobPreset.JobTag tag)
	{
		Interactable result = null;
		this.activeJobItems.TryGetValue(tag, ref result);
		return result;
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x000FE270 File Offset: 0x000FC470
	public virtual void UpdateResolveAnswers()
	{
		Game.Log("Jobs: " + this.presetStr + " updating resolve answers...", 2);
		foreach (Case.ResolveQuestion resolveQuestion in this.thisCase.resolveQuestions)
		{
			foreach (Case.AutoCorrectAnswer autoCorrectAnswer in resolveQuestion.automaticAnswers)
			{
				if (this.poster != null)
				{
					if (autoCorrectAnswer == Case.AutoCorrectAnswer.poster)
					{
						if (!resolveQuestion.correctAnswers.Contains(this.poster.humanID.ToString()))
						{
							resolveQuestion.correctAnswers.Add(this.poster.humanID.ToString());
						}
					}
					else if (autoCorrectAnswer == Case.AutoCorrectAnswer.posterHome && this.poster.home != null)
					{
						if (!resolveQuestion.correctAnswers.Contains(this.poster.home.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(this.poster.home.id.ToString());
						}
					}
					else if (autoCorrectAnswer == Case.AutoCorrectAnswer.posterWork && this.poster.job != null && this.poster.job.employer != null && this.poster.job.employer.address != null)
					{
						if (!resolveQuestion.correctAnswers.Contains(this.poster.job.employer.address.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(this.poster.job.employer.address.id.ToString());
						}
					}
					else if (autoCorrectAnswer == Case.AutoCorrectAnswer.posterPhoto || autoCorrectAnswer == Case.AutoCorrectAnswer.posterHomePhoto || autoCorrectAnswer == Case.AutoCorrectAnswer.posterWorkPhoto)
					{
						foreach (Interactable interactable in CityData.Instance.interactableDirectory.FindAll((Interactable item) => item.preset == InteriorControls.Instance.surveillancePrintout))
						{
							if (interactable.evidence != null)
							{
								EvidenceSurveillance evidenceSurveillance = interactable.evidence as EvidenceSurveillance;
								if (evidenceSurveillance != null && evidenceSurveillance.savedCapture != null)
								{
									if (autoCorrectAnswer == Case.AutoCorrectAnswer.posterPhoto)
									{
										if (evidenceSurveillance.savedCapture.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.poster.humanID) && !resolveQuestion.correctAnswers.Contains(evidenceSurveillance.evID))
										{
											resolveQuestion.correctAnswers.Add(evidenceSurveillance.evID);
										}
									}
									else
									{
										NewGameLocation captureGamelocation = evidenceSurveillance.savedCapture.GetCaptureGamelocation();
										if (autoCorrectAnswer == Case.AutoCorrectAnswer.posterHomePhoto && captureGamelocation == this.poster.home)
										{
											if (!resolveQuestion.correctAnswers.Contains(evidenceSurveillance.evID))
											{
												resolveQuestion.correctAnswers.Add(evidenceSurveillance.evID);
											}
										}
										else if (autoCorrectAnswer == Case.AutoCorrectAnswer.posterWorkPhoto && this.poster.job != null && this.poster.job.employer != null && captureGamelocation == this.poster.job.employer.address && !resolveQuestion.correctAnswers.Contains(evidenceSurveillance.evID))
										{
											resolveQuestion.correctAnswers.Add(evidenceSurveillance.evID);
										}
									}
								}
							}
						}
					}
				}
				if (this.purp != null)
				{
					if (autoCorrectAnswer == Case.AutoCorrectAnswer.purp)
					{
						if (!resolveQuestion.correctAnswers.Contains(this.purp.humanID.ToString()))
						{
							resolveQuestion.correctAnswers.Add(this.purp.humanID.ToString());
						}
					}
					else if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpHome && this.purp.home != null)
					{
						if (!resolveQuestion.correctAnswers.Contains(this.purp.home.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(this.purp.home.id.ToString());
						}
					}
					else if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpWork && this.purp.job != null && this.purp.job.employer != null && this.purp.job.employer.address != null)
					{
						if (!resolveQuestion.correctAnswers.Contains(this.purp.job.employer.address.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(this.purp.job.employer.address.id.ToString());
						}
					}
					else if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpPhoto || autoCorrectAnswer == Case.AutoCorrectAnswer.purpHomePhoto || autoCorrectAnswer == Case.AutoCorrectAnswer.purpWorkPhoto)
					{
						foreach (Interactable interactable2 in CityData.Instance.interactableDirectory.FindAll((Interactable item) => item.preset == InteriorControls.Instance.surveillancePrintout))
						{
							if (interactable2.evidence != null)
							{
								EvidenceSurveillance evidenceSurveillance2 = interactable2.evidence as EvidenceSurveillance;
								if (evidenceSurveillance2 != null && evidenceSurveillance2.savedCapture != null)
								{
									if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpPhoto)
									{
										if (evidenceSurveillance2.savedCapture.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.purp.humanID) && !resolveQuestion.correctAnswers.Contains(evidenceSurveillance2.evID))
										{
											resolveQuestion.correctAnswers.Add(evidenceSurveillance2.evID);
										}
									}
									else
									{
										NewGameLocation captureGamelocation2 = evidenceSurveillance2.savedCapture.GetCaptureGamelocation();
										if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpHomePhoto && captureGamelocation2 == this.purp.home)
										{
											if (!resolveQuestion.correctAnswers.Contains(evidenceSurveillance2.evID))
											{
												resolveQuestion.correctAnswers.Add(evidenceSurveillance2.evID);
											}
										}
										else if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpWorkPhoto && this.purp.job != null && this.purp.job.employer != null && captureGamelocation2 == this.purp.job.employer.address && !resolveQuestion.correctAnswers.Contains(evidenceSurveillance2.evID))
										{
											resolveQuestion.correctAnswers.Add(evidenceSurveillance2.evID);
										}
									}
								}
							}
						}
					}
					if (this.purp.paramour != null)
					{
						if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamour)
						{
							if (!resolveQuestion.correctAnswers.Contains(this.purp.paramour.humanID.ToString()))
							{
								resolveQuestion.correctAnswers.Add(this.purp.paramour.humanID.ToString());
							}
						}
						else if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamourHome && this.purp.paramour.home != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(this.purp.paramour.home.id.ToString()))
							{
								resolveQuestion.correctAnswers.Add(this.purp.paramour.home.id.ToString());
							}
						}
						else if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamourWork && this.purp.paramour.job != null && this.purp.paramour.job.employer != null && this.purp.paramour.job.employer.address != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(this.purp.paramour.job.employer.address.id.ToString()))
							{
								resolveQuestion.correctAnswers.Add(this.purp.paramour.job.employer.address.id.ToString());
							}
						}
						else if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamourPhoto || autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamourHomePhoto || autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamourWorkPhoto)
						{
							foreach (Interactable interactable3 in CityData.Instance.interactableDirectory.FindAll((Interactable item) => item.preset == InteriorControls.Instance.surveillancePrintout))
							{
								if (interactable3.evidence != null)
								{
									EvidenceSurveillance evidenceSurveillance3 = interactable3.evidence as EvidenceSurveillance;
									if (evidenceSurveillance3 != null && evidenceSurveillance3.savedCapture != null)
									{
										if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamourPhoto)
										{
											if (evidenceSurveillance3.savedCapture.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.purp.paramour.humanID) && !resolveQuestion.correctAnswers.Contains(evidenceSurveillance3.evID))
											{
												resolveQuestion.correctAnswers.Add(evidenceSurveillance3.evID);
											}
										}
										else
										{
											NewGameLocation captureGamelocation3 = evidenceSurveillance3.savedCapture.GetCaptureGamelocation();
											if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamourHomePhoto && captureGamelocation3 == this.purp.paramour.home)
											{
												if (!resolveQuestion.correctAnswers.Contains(evidenceSurveillance3.evID))
												{
													resolveQuestion.correctAnswers.Add(evidenceSurveillance3.evID);
												}
											}
											else if (autoCorrectAnswer == Case.AutoCorrectAnswer.purpsParamourWorkPhoto && this.purp.paramour.job != null && this.purp.paramour.job.employer != null && captureGamelocation3 == this.purp.paramour.job.employer.address && !resolveQuestion.correctAnswers.Contains(evidenceSurveillance3.evID))
											{
												resolveQuestion.correctAnswers.Add(evidenceSurveillance3.evID);
											}
										}
									}
								}
							}
						}
					}
				}
				if (autoCorrectAnswer == Case.AutoCorrectAnswer.spawnedItemTag)
				{
					Interactable item8 = this.GetItem(resolveQuestion.tag);
					if (item8 != null)
					{
						Evidence evidence = item8.evidence;
						if (evidence != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(evidence.evID))
							{
								resolveQuestion.correctAnswers.Add(evidence.evID);
							}
						}
						else if (!resolveQuestion.correctAnswers.Contains(item8.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(item8.id.ToString());
						}
					}
				}
				else if (autoCorrectAnswer == Case.AutoCorrectAnswer.spawnedItemA)
				{
					Interactable item2 = this.GetItem(JobPreset.JobTag.A);
					if (item2 != null)
					{
						Evidence evidence2 = item2.evidence;
						if (evidence2 != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(evidence2.evID))
							{
								resolveQuestion.correctAnswers.Add(evidence2.evID);
							}
						}
						else if (!resolveQuestion.correctAnswers.Contains(item2.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(item2.id.ToString());
						}
					}
				}
				else if (autoCorrectAnswer == Case.AutoCorrectAnswer.spawnedItemB)
				{
					Interactable item3 = this.GetItem(JobPreset.JobTag.B);
					if (item3 != null)
					{
						Evidence evidence3 = item3.evidence;
						if (evidence3 != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(evidence3.evID))
							{
								resolveQuestion.correctAnswers.Add(evidence3.evID);
							}
						}
						else if (!resolveQuestion.correctAnswers.Contains(item3.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(item3.id.ToString());
						}
					}
				}
				else if (autoCorrectAnswer == Case.AutoCorrectAnswer.spawnedItemC)
				{
					Interactable item4 = this.GetItem(JobPreset.JobTag.C);
					if (item4 != null)
					{
						Evidence evidence4 = item4.evidence;
						if (evidence4 != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(evidence4.evID))
							{
								resolveQuestion.correctAnswers.Add(evidence4.evID);
							}
						}
						else if (!resolveQuestion.correctAnswers.Contains(item4.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(item4.id.ToString());
						}
					}
				}
				else if (autoCorrectAnswer == Case.AutoCorrectAnswer.spawnedItemD)
				{
					Interactable item5 = this.GetItem(JobPreset.JobTag.D);
					if (item5 != null)
					{
						Evidence evidence5 = item5.evidence;
						if (evidence5 != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(evidence5.evID))
							{
								resolveQuestion.correctAnswers.Add(evidence5.evID);
							}
						}
						else if (!resolveQuestion.correctAnswers.Contains(item5.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(item5.id.ToString());
						}
					}
				}
				else if (autoCorrectAnswer == Case.AutoCorrectAnswer.spawnedItemE)
				{
					Interactable item6 = this.GetItem(JobPreset.JobTag.E);
					if (item6 != null)
					{
						Evidence evidence6 = item6.evidence;
						if (evidence6 != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(evidence6.evID))
							{
								resolveQuestion.correctAnswers.Add(evidence6.evID);
							}
						}
						else if (!resolveQuestion.correctAnswers.Contains(item6.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(item6.id.ToString());
						}
					}
				}
				else if (autoCorrectAnswer == Case.AutoCorrectAnswer.spawnedItemF)
				{
					Interactable item7 = this.GetItem(JobPreset.JobTag.F);
					if (item7 != null)
					{
						Evidence evidence7 = item7.evidence;
						if (evidence7 != null)
						{
							if (!resolveQuestion.correctAnswers.Contains(evidence7.evID))
							{
								resolveQuestion.correctAnswers.Add(evidence7.evID);
							}
						}
						else if (!resolveQuestion.correctAnswers.Contains(item7.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(item7.id.ToString());
						}
					}
				}
			}
			resolveQuestion.UpdateValid(this.thisCase);
			resolveQuestion.UpdateCorrect(this.thisCase, false);
		}
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x000FF0B8 File Offset: 0x000FD2B8
	public float GetDifficulty()
	{
		float num = 0f;
		if (this.preset.difficultyTag == JobPreset.DifficultyTag.D1)
		{
			num = 1f;
		}
		else if (this.preset.difficultyTag == JobPreset.DifficultyTag.D2A || this.preset.difficultyTag == JobPreset.DifficultyTag.D2B)
		{
			num = 2f;
		}
		else if (this.preset.difficultyTag == JobPreset.DifficultyTag.D3)
		{
			num = 3f;
		}
		else if (this.preset.difficultyTag == JobPreset.DifficultyTag.D4A || this.preset.difficultyTag == JobPreset.DifficultyTag.D4B || this.preset.difficultyTag == JobPreset.DifficultyTag.D4C)
		{
			num = 4f;
		}
		else if (this.preset.difficultyTag == JobPreset.DifficultyTag.D5)
		{
			num = 5f;
		}
		else if (this.preset.difficultyTag == JobPreset.DifficultyTag.D6)
		{
			num = 6f;
		}
		foreach (JobPreset.BasicLeadPool basicLeadPool in this.appliedBasicLeads)
		{
			if (basicLeadPool == JobPreset.BasicLeadPool.eyeColour)
			{
				num += 0.7f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.height)
			{
				num += 0.7f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.age)
			{
				num += 0.7f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.jobTitle)
			{
				num += -0.2f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.randomInterest)
			{
				num += 0.7f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.randomAffliction)
			{
				num += 0.6f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.socialClub)
			{
				num += -0.5f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.partnerSocialClub)
			{
				num += -0.1f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.salary)
			{
				num += 0.5f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.bloodType)
			{
				num += 0.5f;
			}
			else if (basicLeadPool == JobPreset.BasicLeadPool.handwriting)
			{
				num += 0.5f;
			}
		}
		return num;
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x000FF268 File Offset: 0x000FD468
	public void AddConfineLocation(Human who, NewAddress where)
	{
		if (who != null && who.ai != null && where != null)
		{
			who.ai.SetConfineLocation(where);
			this.confine.Add(new SideJob.ConfineLocation
			{
				id = who.humanID,
				address = where.id
			});
		}
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x000FF2CC File Offset: 0x000FD4CC
	public void RemoveConfineLocation(Human who, NewAddress where)
	{
		if (who != null && who.ai != null && where != null)
		{
			who.ai.SetConfineLocation(null);
			SideJob.ConfineLocation confineLocation = this.confine.Find((SideJob.ConfineLocation item) => item.id == who.humanID && item.address == where.id);
			if (confineLocation != null)
			{
				this.confine.Remove(confineLocation);
			}
		}
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x000FF358 File Offset: 0x000FD558
	public virtual void DisplayResolveObjectivesCheck()
	{
		if (this.thisCase != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Jobs: Checking for display ",
				this.thisCase.resolveQuestions.Count.ToString(),
				" resolve questions at phase ",
				this.phase.ToString(),
				"..."
			}), 2);
			if (this.knowHandInLocation)
			{
				using (List<Case.ResolveQuestion>.Enumerator enumerator = this.thisCase.resolveQuestions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Case.ResolveQuestion q = enumerator.Current;
						if (q.displayObjective)
						{
							if (q.displayOnlyAtPhase)
							{
								Game.Log("Display " + q.name + " at phase " + q.displayAtPhase.ToString(), 2);
								if (this.phase < q.displayAtPhase)
								{
									continue;
								}
							}
							if (q.inputType != Case.InputType.objective && !Player.Instance.speechController.speechQueue.Exists((SpeechController.QueueElement item) => item.entryRef == q.name))
							{
								Game.Log("Jobs: Adding auto objective (resolve question): " + q.name + "...", 2);
								Objective.ObjectiveTrigger objectiveTrigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.onCompleteJob, "complete", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
								Case @case = this.thisCase;
								string name = q.name;
								Objective.ObjectiveTrigger trigger = objectiveTrigger;
								bool usePointer = false;
								InterfaceControls.Icon icon = q.icon;
								float objectiveDelay = q.objectiveDelay;
								@case.AddObjective(name, trigger, usePointer, default(Vector3), icon, Objective.OnCompleteAction.nothing, objectiveDelay, false, "", false, false, this, false, false, true);
								q.OnProgressChange += this.thisCase.OnQuestionProgressChange;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x000FF580 File Offset: 0x000FD780
	public virtual void TriggerFail(string reason)
	{
		if (!this.failed)
		{
			this.failed = true;
			Game.Log("Jobs: " + this.preset.name + " job failed! " + reason, 2);
			InterfaceController.Instance.ExecuteMissionFailedDisplay(this.thisCase);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.ComposeText(Strings.Get("missions.postings", reason, Strings.Casing.asIs, false, false, false, null), this, Strings.LinkSetting.forceNoLinks, null, null, false), InterfaceControls.Icon.cross, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			this.End();
		}
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x000FF614 File Offset: 0x000FD814
	public virtual void OnDestroyMissionObject(Interactable destroyed)
	{
		if (this.currentBlock != null)
		{
			foreach (JobPreset.JobTag jobTag in this.currentBlock.triggerFailIfItemDestroyed)
			{
				Interactable interactable = null;
				if (this.activeJobItems.TryGetValue(jobTag, ref interactable) && destroyed == interactable)
				{
					this.TriggerFail("An important item was destroyed!");
				}
			}
		}
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x000FF690 File Offset: 0x000FD890
	public virtual void DebugDisplayAnswers()
	{
		if (this.poster != null)
		{
			Game.Log("Jobs: Poster is " + this.poster.name, 2);
		}
		if (this.purp != null)
		{
			Game.Log("Jobs: Purp is " + this.purp.name, 2);
		}
	}

	// Token: 0x0400158D RID: 5517
	[Header("Serialized Data")]
	public string presetStr;

	// Token: 0x0400158E RID: 5518
	public string motiveStr;

	// Token: 0x0400158F RID: 5519
	public int jobID;

	// Token: 0x04001590 RID: 5520
	public static int assignJobID = 1;

	// Token: 0x04001591 RID: 5521
	public SideJob.JobState state;

	// Token: 0x04001592 RID: 5522
	public bool postImmediately;

	// Token: 0x04001593 RID: 5523
	public int startingScenario;

	// Token: 0x04001594 RID: 5524
	public string intro;

	// Token: 0x04001595 RID: 5525
	public string handIn;

	// Token: 0x04001596 RID: 5526
	public bool accepted;

	// Token: 0x04001597 RID: 5527
	public int caseID = -1;

	// Token: 0x04001598 RID: 5528
	public int phase;

	// Token: 0x04001599 RID: 5529
	public int postID = -1;

	// Token: 0x0400159A RID: 5530
	public int gooseChasePhone;

	// Token: 0x0400159B RID: 5531
	public int gooseChaseFromPhone;

	// Token: 0x0400159C RID: 5532
	public bool knowHandInLocation;

	// Token: 0x0400159D RID: 5533
	public float gooseChaseCallTime;

	// Token: 0x0400159E RID: 5534
	public bool gooseChaseCallTriggered;

	// Token: 0x0400159F RID: 5535
	public int meetingPoint;

	// Token: 0x040015A0 RID: 5536
	public int meetingConsumableIndex;

	// Token: 0x040015A1 RID: 5537
	public int secretLocationFurniture;

	// Token: 0x040015A2 RID: 5538
	public Vector3Int secretLocationNode;

	// Token: 0x040015A3 RID: 5539
	public bool failed;

	// Token: 0x040015A4 RID: 5540
	public List<Case.ResolveQuestion> resolveQuestions = new List<Case.ResolveQuestion>();

	// Token: 0x040015A5 RID: 5541
	public int posterID = -1;

	// Token: 0x040015A6 RID: 5542
	public int purpID = -1;

	// Token: 0x040015A7 RID: 5543
	public int reward;

	// Token: 0x040015A8 RID: 5544
	public string rewardSyncDisk;

	// Token: 0x040015A9 RID: 5545
	public int fakeNumber = -1;

	// Token: 0x040015AA RID: 5546
	public string fakeNumberStr;

	// Token: 0x040015AB RID: 5547
	public string jobInfoDialogMsg;

	// Token: 0x040015AC RID: 5548
	public List<JobPreset.BasicLeadPool> appliedBasicLeads = new List<JobPreset.BasicLeadPool>();

	// Token: 0x040015AD RID: 5549
	public List<Evidence.DataKey> leadKeys = new List<Evidence.DataKey>();

	// Token: 0x040015AE RID: 5550
	public List<SideJob.ConfineLocation> confine = new List<SideJob.ConfineLocation>();

	// Token: 0x040015AF RID: 5551
	public List<SideJob.AddedDialog> dialog = new List<SideJob.AddedDialog>();

	// Token: 0x040015B0 RID: 5552
	[Header("Non Serialized")]
	[NonSerialized]
	public int phaseChange = -1;

	// Token: 0x040015B1 RID: 5553
	[NonSerialized]
	public JobPreset preset;

	// Token: 0x040015B2 RID: 5554
	[NonSerialized]
	public MotivePreset motive;

	// Token: 0x040015B3 RID: 5555
	[NonSerialized]
	private JobPreset.IntroConfig chosenIntro;

	// Token: 0x040015B4 RID: 5556
	[NonSerialized]
	private JobPreset.HandInConfig chosenHandIn;

	// Token: 0x040015B5 RID: 5557
	[NonSerialized]
	public Human poster;

	// Token: 0x040015B6 RID: 5558
	[NonSerialized]
	public Human purp;

	// Token: 0x040015B7 RID: 5559
	[NonSerialized]
	public Interactable post;

	// Token: 0x040015B8 RID: 5560
	[NonSerialized]
	public Dictionary<JobPreset.JobTag, Interactable> activeJobItems = new Dictionary<JobPreset.JobTag, Interactable>();

	// Token: 0x040015B9 RID: 5561
	[NonSerialized]
	public Case thisCase;

	// Token: 0x040015BA RID: 5562
	[NonSerialized]
	public Dictionary<string, List<Objective>> objectiveReference = new Dictionary<string, List<Objective>>();

	// Token: 0x040015BB RID: 5563
	[NonSerialized]
	public Interactable hiddenItemPhoto;

	// Token: 0x040015BC RID: 5564
	[NonSerialized]
	public Interactable chosenGooseChasePhone;

	// Token: 0x040015BD RID: 5565
	[NonSerialized]
	public Interactable chosenMeetingPoint;

	// Token: 0x040015BE RID: 5566
	[NonSerialized]
	public TelephoneController.PhoneCall gooseChaseCall;

	// Token: 0x040015BF RID: 5567
	[NonSerialized]
	private SideMissionIntroPreset.SideMissionObjectiveBlock currentBlock;

	// Token: 0x040015C0 RID: 5568
	[NonSerialized]
	private bool triggerHandIn;

	// Token: 0x020002F7 RID: 759
	public enum JobState
	{
		// Token: 0x040015C4 RID: 5572
		generated,
		// Token: 0x040015C5 RID: 5573
		posted,
		// Token: 0x040015C6 RID: 5574
		ended
	}

	// Token: 0x020002F8 RID: 760
	[Serializable]
	public class AddedDialog
	{
		// Token: 0x06001199 RID: 4505 RVA: 0x000FF948 File Offset: 0x000FDB48
		public Human GetHuman()
		{
			Human result = null;
			CityData.Instance.GetHuman(this.humanID, out result, true);
			return result;
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x000FF96C File Offset: 0x000FDB6C
		public DialogPreset GetDialog()
		{
			return Toolbox.Instance.allDialog.Find((DialogPreset item) => item.name.ToLower() == this.dialogRef.ToLower());
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x000FF98C File Offset: 0x000FDB8C
		public NewRoom GetRoom()
		{
			NewRoom result = null;
			CityData.Instance.roomDictionary.TryGetValue(this.roomID, ref result);
			return result;
		}

		// Token: 0x040015C7 RID: 5575
		public int humanID;

		// Token: 0x040015C8 RID: 5576
		public string dialogRef;

		// Token: 0x040015C9 RID: 5577
		public int roomID = -1;

		// Token: 0x040015CA RID: 5578
		public Evidence.DataKey key;

		// Token: 0x040015CB RID: 5579
		[NonSerialized]
		public EvidenceWitness.DialogOption option;
	}

	// Token: 0x020002F9 RID: 761
	[Serializable]
	public class ConfineLocation
	{
		// Token: 0x040015CC RID: 5580
		public int id;

		// Token: 0x040015CD RID: 5581
		public int address;
	}

	// Token: 0x020002FA RID: 762
	// (Invoke) Token: 0x060011A0 RID: 4512
	public delegate void ObjectivesChange();

	// Token: 0x020002FB RID: 763
	// (Invoke) Token: 0x060011A4 RID: 4516
	public delegate void AcquireJobInfo();
}
