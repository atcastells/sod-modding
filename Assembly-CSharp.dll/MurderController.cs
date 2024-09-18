using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class MurderController : MonoBehaviour
{
	// Token: 0x1700002E RID: 46
	// (get) Token: 0x060007D2 RID: 2002 RVA: 0x00076E28 File Offset: 0x00075028
	public static MurderController Instance
	{
		get
		{
			return MurderController._instance;
		}
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x00076E2F File Offset: 0x0007502F
	private void Awake()
	{
		if (MurderController._instance != null && MurderController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		MurderController._instance = this;
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x00076E5D File Offset: 0x0007505D
	public void SetProcGenKillerLoop(bool val)
	{
		if (this.procGenLoopActive != val)
		{
			this.procGenLoopActive = val;
			if (this.procGenLoopActive)
			{
				Game.Log("Murder: Starting murder proc gen loop...", 2);
				this.SetUpdateEnabled(true);
				return;
			}
			Game.Log("Murder: Stopping murder proc gen loop...", 2);
		}
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x00076E98 File Offset: 0x00075098
	public MurderController.Murder GetCurrentMurder()
	{
		MurderController.Murder murder = null;
		if (this.currentVictim != null)
		{
			murder = this.activeMurders.Find((MurderController.Murder item) => item.victim == this.currentVictim && item.murderer == this.currentMurderer);
		}
		if (murder == null)
		{
			foreach (MurderController.Murder murder2 in this.activeMurders)
			{
				if (murder == null || murder2.creationTime > murder.creationTime)
				{
					murder = murder2;
				}
			}
		}
		return murder;
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00076F24 File Offset: 0x00075124
	public void Tick(float timePassed)
	{
		if (this.procGenLoopActive)
		{
			if (this.currentMurderer == null)
			{
				this.PickNewMurderer();
				return;
			}
			MurderController.Murder currentMurder = this.GetCurrentMurder();
			if (this.pauseBetweenMurders <= 0f && this.pauseBeforeKidnapperKill <= 0f && (currentMurder == null || currentMurder.preset.caseType != MurderPreset.CaseType.kidnap || currentMurder.ransomPhase == MurderController.KidnapRansomPhase.none || currentMurder.ransomPhase == MurderController.KidnapRansomPhase.travellingToRansom || currentMurder.ransomPhase == MurderController.KidnapRansomPhase.finishedFailed || currentMurder.ransomPhase == MurderController.KidnapRansomPhase.finishedSuccess))
			{
				if (this.murderRoutineActive)
				{
					for (int i = 0; i < this.activeMurders.Count; i++)
					{
						MurderController.Murder murder = this.activeMurders[i];
						if (murder.state <= MurderController.MurderState.executing)
						{
							murder.EuipmentCheck();
						}
						if (murder.murderGoal != null && !murder.murderGoal.isActive && (murder.state == MurderController.MurderState.acquireEuipment || murder.state == MurderController.MurderState.travellingTo) && murder.murderer != null)
						{
							murder.murderer.ai.AITick(true, false);
							return;
						}
					}
					return;
				}
				if (currentMurder != null && currentMurder.preset.caseType == MurderPreset.CaseType.kidnap && this.currentVictim != null && !this.currentVictim.isDead && this.activeMurders.Count > 0 && currentMurder.ransomPhase != MurderController.KidnapRansomPhase.finishedSuccess)
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("Murder: Waiting for kidnapper to kill their victim...", 2);
					}
					if (currentMurder.preset.caseType == MurderPreset.CaseType.kidnap)
					{
						if (!currentMurder.victim.isDead)
						{
							if (currentMurder.fakeNumberStr != null && currentMurder.fakeNumberStr.Length > 0)
							{
								TelephoneController.Instance.RemoveFakeNumber(currentMurder.fakeNumber);
							}
							currentMurder.kidnapKillPhase = true;
							currentMurder.victim.unreportable = true;
							currentMurder.CreateMurderGoal();
							if (currentMurder.murderer.currentGameLocation == currentMurder.victim.currentGameLocation)
							{
								if (currentMurder.victim.ai != null)
								{
									Game.Log("Murder: Setting victim unrestrained...", 2);
									currentMurder.victim.ai.SetRestrained(false, 0f);
								}
								currentMurder.SetMurderState(MurderController.MurderState.executing, false);
							}
							else if (currentMurder.state != MurderController.MurderState.executing)
							{
								Game.Log("Murder: It's time for the kidnapper to kill their victim!", 2);
								currentMurder.SetMurderState(MurderController.MurderState.travellingTo, false);
							}
							this.murderRoutineActive = true;
							this.SetUpdateEnabled(true);
							return;
						}
						if (currentMurder.ransomPhase != MurderController.KidnapRansomPhase.finishedFailed)
						{
							this.TriggerRansomFail();
							return;
						}
					}
				}
				else
				{
					if (this.currentActiveCase != null)
					{
						if (this.currentActiveCase.currentActiveObjectives.Exists((Objective item) => item.queueElement.entryRef == "Explore Reported Crime Scene"))
						{
							return;
						}
					}
					if (this.currentVictim == null)
					{
						this.PickNewVictim();
						return;
					}
					this.ExecuteNewMurder(this.currentMurderer, this.currentVictim, this.murderPreset, this.chosenMO, this.currentVictimSite);
					return;
				}
			}
			else
			{
				if (this.pauseBetweenMurders > 0f)
				{
					this.pauseBetweenMurders -= timePassed;
				}
				if (this.pauseBeforeKidnapperKill > 0f)
				{
					this.pauseBeforeKidnapperKill -= timePassed;
				}
				if (currentMurder != null && this.murderPreset != null && this.murderPreset.caseType == MurderPreset.CaseType.kidnap && this.currentVictim != null && !this.currentVictim.isDead && this.activeMurders.Count > 0)
				{
					if (currentMurder.ransomPhase == MurderController.KidnapRansomPhase.travellingToRansom)
					{
						if (currentMurder.murderer.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.kidnapperCollectRansom) == null)
						{
							if (Game.Instance.printDebug)
							{
								Game.Log("Murder: Making the kidnapper travel to the ransom...", 2);
							}
							if (currentMurder.ransomSite != null)
							{
								List<NewGameLocation> list = new List<NewGameLocation>();
								if (currentMurder.ransomSite.floors.ContainsKey(-1))
								{
									foreach (NewAddress newAddress in currentMurder.ransomSite.floors[-1].addresses)
									{
										if (newAddress.isLobby)
										{
											list.Add(newAddress);
										}
									}
								}
								NewGameLocation newGameLocation = null;
								Interactable interactable = null;
								if (list.Count > 0)
								{
									newGameLocation = list[Toolbox.Instance.Rand(0, list.Count, false)];
								}
								for (int j = -1; j >= -2; j--)
								{
									if (currentMurder.ransomSite.floors.ContainsKey(j))
									{
										foreach (NewAddress newAddress2 in currentMurder.ransomSite.floors[j].addresses)
										{
											foreach (NewRoom newRoom in newAddress2.rooms)
											{
												int num;
												string text;
												if (!Player.Instance.IsTrespassing(newRoom, out num, out text, true))
												{
													foreach (NewNode newNode in newRoom.nodes)
													{
														interactable = newNode.interactables.Find((Interactable item) => item.wo && item.preset.specialCaseFlag == InteractablePreset.SpecialCase.briefcase && item.inInventory == null);
														if (interactable != null)
														{
															interactable.GetWorldPosition(true);
															interactable.spWPos = interactable.wPos;
															interactable.UpdateWorldPositionAndNode(true);
															interactable.SetOriginalPosition(true, true);
															break;
														}
													}
													if (interactable != null)
													{
														break;
													}
												}
											}
											if (interactable != null)
											{
												break;
											}
										}
									}
									if (interactable != null)
									{
										break;
									}
								}
								NewAIController ai = currentMurder.murderer.ai;
								AIGoalPreset kidnapperCollectRansom = RoutineControls.Instance.kidnapperCollectRansom;
								float newTrigerTime = 0f;
								float newDuration = 0f;
								NewNode newPassedNode = null;
								NewGameLocation newPassedGameLocation = newGameLocation;
								ai.CreateNewGoal(kidnapperCollectRansom, newTrigerTime, newDuration, newPassedNode, interactable, newPassedGameLocation, null, null, -2);
								return;
							}
						}
					}
					else if (currentMurder.ransomPhase == MurderController.KidnapRansomPhase.freeingVictim)
					{
						if (currentMurder.murderer.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.kidnapperFreeVictim) == null)
						{
							if (Game.Instance.printDebug)
							{
								Game.Log("Murder: Making the kidnapper travel to free the victim...", 2);
							}
							currentMurder.murderer.ai.CreateNewGoal(RoutineControls.Instance.kidnapperFreeVictim, 0f, 0f, null, currentMurder.victim.interactable, null, null, null, -2);
						}
						if (currentMurder.murderer.currentGameLocation == currentMurder.victim.currentGameLocation)
						{
							Game.Log("Murder: Setting victim unrestrained...", 2);
							currentMurder.victim.ai.SetRestrained(false, 0f);
						}
					}
				}
			}
		}
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x00077630 File Offset: 0x00075830
	public void PickNewMurderer()
	{
		Game.Log("Murder: Picking new murderer...", 2);
		this.playerAcceptedCoverUp = false;
		this.triggerCoverUpCall = false;
		List<MurderPreset> list = new List<MurderPreset>();
		foreach (MurderPreset murderPreset in Toolbox.Instance.allMurderPresets)
		{
			if (!murderPreset.disabled)
			{
				for (int i = 0; i < murderPreset.frequency; i++)
				{
					list.Add(murderPreset);
				}
			}
		}
		this.murderPreset = list[Toolbox.Instance.Rand(0, list.Count, false)];
		Game.Log("Murder: ...Picked " + this.murderPreset.name + " as a preset...", 2);
		List<MurderController.MurderPick> list2 = new List<MurderController.MurderPick>();
		foreach (Citizen citizen in CityData.Instance.citizenDirectory)
		{
			if (!citizen.isPlayer && !citizen.isDead && !citizen.isHomeless && !this.previousMurderers.Contains(citizen) && !(this.currentVictim == citizen))
			{
				float num = Toolbox.Instance.Rand(this.murderPreset.murdererRandomScoreRange.x, this.murderPreset.murdererRandomScoreRange.y, false);
				float num2 = 0f;
				if (this.murderPreset.murdererTraitModifiers.Count > 0)
				{
					if (!this.TraitTest(citizen, ref this.murderPreset.murdererTraitModifiers, out num2))
					{
						continue;
					}
					num += num2;
				}
				if (this.murderPreset.useHexaco)
				{
					num += (float)citizen.GetHexacoScore(ref this.murderPreset.hexaco);
				}
				using (List<MurderMO>.Enumerator enumerator3 = Toolbox.Instance.allMurderMOs.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						MurderMO wp = enumerator3.Current;
						if (!wp.disabled && wp.compatibleWith.Contains(this.murderPreset))
						{
							float num3 = num + Toolbox.Instance.Rand(wp.pickRandomScoreRange.x, wp.pickRandomScoreRange.y, false);
							if (this.murderPreset.caseType == MurderPreset.CaseType.sniper && wp.requiresSniperVantageAtHome)
							{
								if (!(citizen.home != null) || !(citizen.home.floor != null) || citizen.home.floor.floor <= 0 || citizen.home.entrances.Count < 4)
								{
									continue;
								}
								int num4 = 0;
								using (List<NewNode.NodeAccess>.Enumerator enumerator4 = citizen.home.entrances.GetEnumerator())
								{
									while (enumerator4.MoveNext())
									{
										if (enumerator4.Current.accessType == NewNode.NodeAccess.AccessType.window)
										{
											num4++;
											if (num4 >= 4)
											{
												break;
											}
										}
									}
								}
								if (num4 < 4)
								{
									continue;
								}
								List<NewGameLocation> list3 = new List<NewGameLocation>();
								NewWall newWall;
								float num5;
								if (!Toolbox.Instance.TryGetSniperVantagePoint(citizen.home, out newWall, out num5, out list3, null))
								{
									Game.Log("Murder: Unable to find any valid sites for " + citizen.home.name, 2);
									continue;
								}
								List<NewGameLocation> list4 = list3.FindAll((NewGameLocation item) => item.thisAsAddress != null && item.thisAsAddress.inhabitants.Count > 0);
								if (list4.Count < 6)
								{
									Game.Log("Murder: Found " + list4.Count.ToString() + " for " + citizen.home.name, 2);
									continue;
								}
							}
							float num6 = 0f;
							if (wp.murdererTraitModifiers.Count > 0)
							{
								if (!this.TraitTest(citizen, ref wp.murdererTraitModifiers, out num6))
								{
									continue;
								}
								num3 += num6;
							}
							if (wp.useHexaco)
							{
								num3 += (float)citizen.GetHexacoScore(ref wp.hexaco);
							}
							foreach (MurderMO.JobModifier jobModifier in wp.murdererJobModifiers)
							{
								if (citizen.job != null && jobModifier.jobs.Contains(citizen.job.preset))
								{
									num3 += (float)jobModifier.jobBoost;
								}
							}
							foreach (MurderMO.CompanyModifier companyModifier in wp.murdererCompanyModifiers)
							{
								if (citizen.job != null && citizen.job.employer != null && companyModifier.companies.Contains(citizen.job.employer.preset))
								{
									List<Occupation> list5 = citizen.job.employer.companyRoster.FindAll((Occupation item) => item.employee != null);
									if (list5.Count >= companyModifier.mininumEmployees)
									{
										num3 += (float)companyModifier.companyBoost;
										num3 += (float)((list5.Count - companyModifier.mininumEmployees) * companyModifier.boostPerEmployeeOverMinimum);
									}
								}
							}
							if (wp.useMurdererSocialClassRange && citizen.societalClass >= wp.murdererClassRange.x && citizen.societalClass <= wp.murdererClassRange.y)
							{
								num3 += (float)wp.murdererClassRangeBoost;
							}
							if (wp.baseDifficulty <= MurderController.Instance.maxDifficultyLevel)
							{
								num3 += 12f;
							}
							if (MurderController.Instance.inactiveMurders.Exists((MurderController.Murder item) => item.mo == wp))
							{
								num3 -= 55f;
							}
							MurderController.MurderPick murderPick = new MurderController.MurderPick
							{
								person = citizen,
								score = num3,
								mo = wp
							};
							list2.Add(murderPick);
						}
					}
				}
			}
		}
		list2.Sort((MurderController.MurderPick p1, MurderController.MurderPick p2) => p2.score.CompareTo(p1.score));
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.debugLastMurderPicks = list2;
		}
		if (list2.Count <= 0)
		{
			Game.LogError("Cannot find new valid murderer!", 2);
			return;
		}
		this.currentMurderer = list2[0].person;
		this.chosenMO = list2[0].mo;
		if (this.murderPreset.pickDen && this.currentMurderer.den == null)
		{
			List<NewAddress> list6 = new List<NewAddress>();
			foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
			{
				if (newAddress.addressPreset != null && newAddress.addressPreset.presetName == "EmptyDen" && newAddress.inhabitants.Count <= 0 && newAddress.floor != null && newAddress.floor.floor >= -1)
				{
					list6.Add(newAddress);
				}
			}
			if (list6.Count > 0)
			{
				NewAddress newAddress2 = list6[Toolbox.Instance.Rand(0, list6.Count, false)];
				this.currentMurderer.SetDen(newAddress2);
				Game.Log("Murder: Chosen " + newAddress2.name + " as a den for the murderer", 2);
			}
		}
		Game.Log(string.Concat(new string[]
		{
			"Murder: Chosen ",
			this.currentMurderer.GetCitizenName(),
			" to be new murderer of type ",
			this.murderPreset.name,
			" with MO ",
			this.chosenMO.name
		}), 2);
		if (Game.Instance.debugMurdererOnStart && Game.Instance.collectDebugData)
		{
			this.currentMurderer.ai.ToggleHumanDebug();
		}
		this.UpdateResolveQuestions(true);
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00077F60 File Offset: 0x00076160
	public void OnVictimDiscovery()
	{
		if (Game.Instance.sandboxMode || (ChapterController.Instance != null && ChapterController.Instance.chapterScript as ChapterIntro != null && (ChapterController.Instance.chapterScript as ChapterIntro).completed))
		{
			if (!CasePanelController.Instance.activeCases.Exists((Case item) => item.caseType == Case.CaseType.murder && (item.caseStatus != Case.CaseStatus.closable && item.caseStatus != Case.CaseStatus.submitted) && item.caseStatus != Case.CaseStatus.archived))
			{
				Case newCase = CasePanelController.Instance.CreateNewCase(Case.CaseType.murder, Case.CaseStatus.handInNotCollected, true, Strings.Get("ui.interface", "New Murder Case", Strings.Casing.asIs, false, false, false, null));
				this.AssignActiveCase(newCase);
				SessionData.Instance.TutorialTrigger("murders", false);
			}
		}
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x00078020 File Offset: 0x00076220
	public void OnVictimKilled()
	{
		if (GameplayControls.Instance.enableCoverUps && this.murderPreset != null && (this.murderPreset.caseType == MurderPreset.CaseType.murder || this.murderPreset.caseType == MurderPreset.CaseType.sniper))
		{
			if (!this.triggerCoverUpCall && !this.playerAcceptedCoverUp)
			{
				if ((MurderController.Instance.inactiveMurders.Count >= GameplayControls.Instance.coverUpAvailableDuringCase || Game.Instance.forceCoverUpOffers) && Toolbox.Instance.Rand(0f, 1f, false) > GameplayControls.Instance.coverUpChance)
				{
					bool forceCoverUpOffers = Game.Instance.forceCoverUpOffers;
					return;
				}
			}
			else if (!this.playerAcceptedCoverUp)
			{
				bool forceCoverUpOffers2 = Game.Instance.forceCoverUpOffers;
			}
		}
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x000780E0 File Offset: 0x000762E0
	public void TriggerCoverUpTelephoneCall()
	{
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x000780ED File Offset: 0x000762ED
	public void OnCoverUpAccept()
	{
		this.triggerCoverUpCall = false;
		this.triggeredSeenWarning = false;
		this.coverUpCall = null;
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x000780ED File Offset: 0x000762ED
	public void OnCoverUpReject()
	{
		this.triggerCoverUpCall = false;
		this.triggeredSeenWarning = false;
		this.coverUpCall = null;
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x00078104 File Offset: 0x00076304
	public void TriggerCoverUpObjective()
	{
		this.triggerCoverUpCall = false;
		this.triggeredSeenWarning = false;
		this.coverUpCall = null;
		if (!CasePanelController.Instance.activeCases.Exists((Case item) => item.caseType == Case.CaseType.murder && (item.caseStatus != Case.CaseStatus.closable && item.caseStatus != Case.CaseStatus.submitted) && item.caseStatus != Case.CaseStatus.archived))
		{
			Case newCase = CasePanelController.Instance.CreateNewCase(Case.CaseType.murder, Case.CaseStatus.handInNotCollected, true, Strings.Get("ui.interface", "New Murder Case", Strings.Casing.asIs, false, false, false, null));
			this.AssignActiveCase(newCase);
		}
		if (this.currentActiveCase != null && this.currentVictim != null)
		{
			Game.Log("Murder: Creating cover up objective for this victim", 2);
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.disposeOfBody, "disposeOfBody", false, 0f, null, null, this.currentVictim.evidenceEntry, null, null, null, null, "", false, default(Vector3));
			string entryRef = Strings.Get("missions.postings", "Without anybody seeing, dispose of the body at: ", Strings.Casing.asIs, false, false, false, null) + this.currentVictim.currentGameLocation.name;
			this.currentActiveCase.AddObjective(entryRef, trigger, false, Vector3.zero, InterfaceControls.Icon.skull, Objective.OnCompleteAction.completeCoverUp, 0f, false, "", false, false, null, false, true, false);
			Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToRoom, "", false, 0f, this.currentVictim.currentRoom, null, null, null, null, null, null, "", false, default(Vector3));
			this.currentActiveCase.AddObjective("coverup_tips", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.coverUpTips, 0f, false, "", true, false, null, false, false, true);
		}
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x00078290 File Offset: 0x00076490
	public void TriggerKidnappingCase()
	{
		if (!CasePanelController.Instance.activeCases.Exists((Case item) => item.caseType == Case.CaseType.murder && (item.caseStatus != Case.CaseStatus.closable && item.caseStatus != Case.CaseStatus.submitted) && item.caseStatus != Case.CaseStatus.archived))
		{
			Game.Log("Triggered new kidnapping case", 2);
			if (this.currentVictim.home != null)
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "New Enforcer Call Kidnapping", Strings.Casing.asIs, false, false, false, null) + ": " + this.currentVictim.GetCitizenName(), InterfaceControls.Icon.skull, AudioControls.Instance.enforcerScannerMsg, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
			Case newCase = CasePanelController.Instance.CreateNewCase(Case.CaseType.murder, Case.CaseStatus.handInNotCollected, true, Strings.Get("ui.interface", "New Kidnap Case", Strings.Casing.asIs, false, false, false, null));
			this.AssignActiveCase(newCase);
		}
		if (this.currentActiveCase != null && MurderController.Instance.currentActiveCase != null && this.currentVictim != null)
		{
			MurderController.Murder currentMurder = this.GetCurrentMurder();
			if (currentMurder != null)
			{
				if (currentMurder.ransomSite == null)
				{
					currentMurder.GenerateRansomDetails();
				}
				currentMurder.killTime = SessionData.Instance.gameTime + currentMurder.preset.kidnapperTimeUntilKill;
				Interactable interactable = null;
				if (currentMurder.activeMurderItems.TryGetValue(JobPreset.JobTag.U, ref interactable))
				{
					Game.Log("Murder: Adding find ransom note objective...", 2);
					Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, interactable.evidence, null, null, null, null, "", false, default(Vector3));
					string entryRef = Strings.Get("missions.postings", "Examine the ransom note found at ", Strings.Casing.asIs, false, false, false, null) + this.currentVictim.home.name;
					MurderController.Instance.currentActiveCase.AddObjective(entryRef, trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.triggerRansomDelivery, 0f, false, "", false, false, null, false, true, false);
				}
				CasePanelController.Instance.PinToCasePanel(this.currentActiveCase, currentMurder.victim.evidenceEntry, Evidence.DataKey.name, true, default(Vector2), false);
			}
		}
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x000784AC File Offset: 0x000766AC
	public void TriggerRansomDelivery()
	{
		if (this.currentActiveCase != null && this.currentVictim != null)
		{
			MurderController.Murder currentMurder = this.GetCurrentMurder();
			if (currentMurder != null)
			{
				Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.victimIsFreed, "set_free", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
				this.currentActiveCase.AddObjective("set_victim_free", trigger, false, Vector3.zero, InterfaceControls.Icon.time, Objective.OnCompleteAction.kidnapperVictimFreed, 0f, false, "", false, false, null, false, false, true);
				Game.Log("Murder: Creating ransom objective...", 2);
				Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.kidnapperHasValidBriefcase, "ransom", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
				string entryRef = Strings.ComposeText(Strings.Get("missions.postings", "Ransom ", Strings.Casing.asIs, false, false, false, null) + currentMurder.ransomSite.name, null, Strings.LinkSetting.forceNoLinks, null, null, false);
				this.currentActiveCase.AddObjective(entryRef, trigger2, false, Vector3.zero, InterfaceControls.Icon.money, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, null, false, false, false);
				Game.Log("Murder: Setting objective to call number to trigger...", 2);
				if (GameplayControls.Instance.kidnapperCallTriggerDialog != null)
				{
					currentMurder.GenerateFakeNumber();
					TelephoneController.Instance.AddFakeNumber(currentMurder.fakeNumber, new TelephoneController.CallSource(TelephoneController.CallType.player, GameplayControls.Instance.kidnapperCallTriggerDialog, InteractionController.ConversationType.normal));
					TelephoneController.Instance.fakeTelephoneDictionary[currentMurder.fakeNumber].dialogGreeting = GameplayControls.Instance.kidnapperCallTriggerDialog;
					TelephoneController.Instance.fakeTelephoneDictionary[currentMurder.fakeNumber].dialog = GameplayControls.Instance.kidnapperCallTriggerDialog.name;
					Objective.ObjectiveTrigger trigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.makeCall, currentMurder.fakeNumber.ToString(), false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
					string entryRef2 = Strings.Get("missions.postings", "Call to trigger ransom ", Strings.Casing.asIs, false, false, false, null) + currentMurder.fakeNumberStr;
					this.currentActiveCase.AddObjective(entryRef2, trigger3, false, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.triggerKidnapperRansomCollection, 0f, false, "", false, false, null, false, false, false);
					return;
				}
				Game.LogError("Kidnapper call dialog reference is null!", 2);
			}
		}
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x000786E4 File Offset: 0x000768E4
	public void KidnapperCollectsRansom()
	{
		if (this.currentActiveCase != null && this.currentVictim != null)
		{
			MurderController.Murder currentMurder = this.GetCurrentMurder();
			if (currentMurder != null)
			{
				Game.Log("Murder: The kidnapper has been triggered to collect the ransom...", 2);
				currentMurder.SetRansomPhase(MurderController.KidnapRansomPhase.travellingToRansom);
				this.pauseBeforeKidnapperKill += 1f;
				Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.kidnapperHasValidBriefcase, "ransom", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
				this.currentActiveCase.AddObjective("ransom_wait", trigger, false, Vector3.zero, InterfaceControls.Icon.time, Objective.OnCompleteAction.kidnapperCollectedRansom, 0f, false, "", false, false, null, false, false, true);
			}
		}
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x00078790 File Offset: 0x00076990
	public void KidnapperCollectedRansom()
	{
		if (this.currentActiveCase != null && this.currentVictim != null)
		{
			MurderController.Murder currentMurder = this.GetCurrentMurder();
			if (currentMurder != null)
			{
				Game.Log("Murder: The kidnapper has collected a valid ransom briefcase", 2);
				for (int i = 0; i < this.currentActiveCase.currentActiveObjectives.Count; i++)
				{
					Objective objective = this.currentActiveCase.currentActiveObjectives[i];
					if (objective != null)
					{
						if (objective.queueElement.triggers.Exists((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.ifNoValidRansomBriefcase || item.triggerType == Objective.ObjectiveTriggerType.ifValidRansomBriefcase || item.triggerType == Objective.ObjectiveTriggerType.makeCall))
						{
							objective.Cancel();
						}
					}
				}
				currentMurder.SetRansomPhase(MurderController.KidnapRansomPhase.collectedRansom);
				for (int j = 0; j < currentMurder.murderer.inventory.Count; j++)
				{
					Interactable interactable = currentMurder.murderer.inventory[j];
					if (interactable != null && interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.briefcase)
					{
						if (interactable.preset.fpsItem != null && interactable.preset.fpsItem.presetName == "BriefcaseBomb")
						{
							Game.Log("Murder: This is a briefcase bomb, trigger it!", 2);
							Toolbox.Instance.TriggerBriefcaseBomb(interactable, currentMurder.murderer);
							interactable.SafeDelete(true);
							MurderController.Instance.pauseBeforeKidnapperKill = 0f;
							this.TriggerRansomFail();
						}
						else if (interactable.cs >= (float)currentMurder.ransomAmount)
						{
							Game.Log("Murder: The correct ransom amount is within the briefcase", 2);
							currentMurder.SetRansomPhase(MurderController.KidnapRansomPhase.freeingVictim);
							Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.victimIsFreed, "ransom_free", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
							this.currentActiveCase.AddObjective("ransom_wait_free", trigger, false, Vector3.zero, InterfaceControls.Icon.time, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, null, false, false, true);
						}
						else
						{
							Game.Log("Murder: The incorrect ransom amount is within the briefcase", 2);
							MurderController.Instance.pauseBeforeKidnapperKill = 0f;
							this.TriggerRansomFail();
						}
					}
				}
			}
		}
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x00078998 File Offset: 0x00076B98
	public void TriggerRansomFail()
	{
		if (this.currentActiveCase != null && this.currentVictim != null)
		{
			MurderController.Murder currentMurder = this.GetCurrentMurder();
			if (currentMurder != null)
			{
				Game.Log("Murder: The ransom has failed", 2);
				for (int i = 0; i < this.currentActiveCase.currentActiveObjectives.Count; i++)
				{
					Objective objective = this.currentActiveCase.currentActiveObjectives[i];
					if (objective != null)
					{
						if (objective.queueElement.triggers.Exists((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.ifNoValidRansomBriefcase || item.triggerType == Objective.ObjectiveTriggerType.ifValidRansomBriefcase || item.triggerType == Objective.ObjectiveTriggerType.makeCall))
						{
							objective.Cancel();
						}
					}
				}
				currentMurder.SetRansomPhase(MurderController.KidnapRansomPhase.finishedFailed);
				TelephoneController.Instance.RemoveFakeNumber(currentMurder.fakeNumber);
			}
		}
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x00078A58 File Offset: 0x00076C58
	public void VictimFreed()
	{
		if (this.currentActiveCase != null && this.currentVictim != null)
		{
			MurderController.Murder currentMurder = this.GetCurrentMurder();
			if (currentMurder != null)
			{
				for (int i = 0; i < this.currentActiveCase.currentActiveObjectives.Count; i++)
				{
					Objective objective = this.currentActiveCase.currentActiveObjectives[i];
					if (objective != null)
					{
						if (objective.queueElement.triggers.Exists((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.kidnapperHasValidBriefcase || item.triggerType == Objective.ObjectiveTriggerType.makeCall))
						{
							objective.Cancel();
						}
					}
				}
				Game.Log("Murder: The victim has been freed!", 2);
				currentMurder.SetRansomPhase(MurderController.KidnapRansomPhase.finishedSuccess);
				for (int j = 0; j < currentMurder.murderer.inventory.Count; j++)
				{
					Interactable interactable = currentMurder.murderer.inventory[j];
					if (interactable != null && interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.briefcase)
					{
						foreach (Human.WalletItem walletItem in currentMurder.murderer.walletItems)
						{
							if (walletItem.itemType == Human.WalletItemType.money)
							{
								walletItem.money += Mathf.RoundToInt(interactable.cs * 1000f);
								break;
							}
						}
						interactable.SafeDelete(true);
					}
				}
				if (currentMurder.fakeNumberStr != null && currentMurder.fakeNumberStr.Length > 0)
				{
					TelephoneController.Instance.RemoveFakeNumber(currentMurder.fakeNumber);
				}
				this.ResetKidnapper();
			}
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00078BF8 File Offset: 0x00076DF8
	public void ResetKidnapper()
	{
		Game.Log("Murder: Kidnapper starting afresh...", 2);
		this.currentVictim = null;
		this.pauseBeforeKidnapperKill = this.murderPreset.kidnapperTimeUntilKill;
		this.pauseBetweenMurders = this.murderPreset.minimumTimeBetweenMurders;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x00078C30 File Offset: 0x00076E30
	public void OnCaseSolved()
	{
		Game.Log("Murder: Case is solved!", 2);
		foreach (MurderController.Murder murder in this.activeMurders)
		{
			if (murder.victim != null)
			{
				murder.victim.unreportable = false;
				if (murder.victim.isDead && !murder.victim.removedFromWorld)
				{
					murder.victim.RemoveFromWorld(true);
				}
			}
			if (murder.murderer != null && !murder.murderer.removedFromWorld)
			{
				murder.murderer.RemoveFromWorld(true);
			}
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00078CF0 File Offset: 0x00076EF0
	public void CitizenHasSeenBody(Human seenBody, Human seenBy)
	{
		if (this.triggeredSeenWarning)
		{
			return;
		}
		if (seenBody == null)
		{
			if (!(InteractionController.Instance.currentlyDragging != null))
			{
				return;
			}
			using (List<Case>.Enumerator enumerator = CasePanelController.Instance.activeCases.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Case @case = enumerator.Current;
					for (int i = 0; i < @case.currentActiveObjectives.Count; i++)
					{
						Objective objective = @case.currentActiveObjectives[i];
						if (!objective.isCancelled)
						{
							Objective.ObjectiveTrigger objectiveTrigger = objective.queueElement.triggers.Find((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.disposeOfBody && item.evidenceID == InteractionController.Instance.currentlyDragging.ai.human.evidenceEntry.evID);
							if (objectiveTrigger != null && !objectiveTrigger.triggered)
							{
								InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "CoverUp_BodySeen", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.eye, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
								this.triggeredSeenWarning = true;
								break;
							}
						}
					}
				}
				return;
			}
		}
		Predicate<Objective.ObjectiveTrigger> <>9__1;
		foreach (Case case2 in CasePanelController.Instance.activeCases)
		{
			for (int j = 0; j < case2.currentActiveObjectives.Count; j++)
			{
				Objective objective2 = case2.currentActiveObjectives[j];
				if (!objective2.isCancelled)
				{
					List<Objective.ObjectiveTrigger> triggers = objective2.queueElement.triggers;
					Predicate<Objective.ObjectiveTrigger> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = ((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.disposeOfBody && item.evidenceID == seenBody.evidenceEntry.evID));
					}
					Objective.ObjectiveTrigger objectiveTrigger2 = triggers.Find(predicate);
					if (objectiveTrigger2 != null && !objectiveTrigger2.triggered)
					{
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "CoverUp_BodySeen", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.eye, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
						this.triggeredSeenWarning = true;
						break;
					}
				}
			}
		}
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x00078F44 File Offset: 0x00077144
	public void CoverUpFailCheck(Human seenBody)
	{
		bool flag = false;
		Game.Log("Murder: Checking for cover-up fail...", 2);
		if (seenBody == null)
		{
			if (!(InteractionController.Instance.currentlyDragging != null))
			{
				goto IL_1AE;
			}
			using (List<Case>.Enumerator enumerator = CasePanelController.Instance.activeCases.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Case @case = enumerator.Current;
					for (int i = 0; i < @case.currentActiveObjectives.Count; i++)
					{
						Objective objective = @case.currentActiveObjectives[i];
						if (!objective.isCancelled)
						{
							Objective.ObjectiveTrigger objectiveTrigger = objective.queueElement.triggers.Find((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.disposeOfBody && item.evidenceID == InteractionController.Instance.currentlyDragging.ai.human.evidenceEntry.evID);
							if (objectiveTrigger != null && !objectiveTrigger.triggered)
							{
								objective.Cancel();
								flag = true;
							}
						}
					}
				}
				goto IL_1AE;
			}
		}
		Predicate<Objective.ObjectiveTrigger> <>9__1;
		foreach (Case case2 in CasePanelController.Instance.activeCases)
		{
			for (int j = 0; j < case2.currentActiveObjectives.Count; j++)
			{
				Objective objective2 = case2.currentActiveObjectives[j];
				if (!objective2.isCancelled)
				{
					List<Objective.ObjectiveTrigger> triggers = objective2.queueElement.triggers;
					Predicate<Objective.ObjectiveTrigger> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = ((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.disposeOfBody && item.evidenceID == seenBody.evidenceEntry.evID));
					}
					Objective.ObjectiveTrigger objectiveTrigger2 = triggers.Find(predicate);
					if (objectiveTrigger2 != null && !objectiveTrigger2.triggered)
					{
						objective2.Cancel();
						flag = true;
					}
				}
			}
		}
		IL_1AE:
		if (flag)
		{
			InterfaceController.Instance.ExecuteCoverUpFailedDisplay();
		}
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00079128 File Offset: 0x00077328
	public void TriggerSuccessfulCoverUp(Evidence passedCitizen)
	{
		MurderController.Murder murder = this.activeMurders.Find((MurderController.Murder item) => item.victim.evidenceEntry == passedCitizen);
		if (murder != null)
		{
			murder.victim.RemoveFromWorld(true);
			this.triggerCoverUpSuccess = true;
			InterfaceController.Instance.ExecuteCoverUpSuccessDisplay();
			base.Invoke("TriggerCoverUpSuccessCall", 3f);
		}
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0007918C File Offset: 0x0007738C
	public void TriggerCoverUpSuccessCall()
	{
		if (!this.triggerCoverUpSuccess)
		{
			return;
		}
		if (this.successCall != null && (this.successCall.state == TelephoneController.CallState.dialing || this.successCall.state == TelephoneController.CallState.ringing || this.successCall.state == TelephoneController.CallState.started))
		{
			return;
		}
		if (Game.Instance.printDebug)
		{
			Game.Log("Murder: Attempt to trigger cover-up success call...", 2);
		}
		if (Player.Instance.currentRoom.gameLocation != null)
		{
			Telephone telephone = null;
			if (this.currentMurderer.home != null && this.currentMurderer.home.telephones.Count > 0)
			{
				telephone = this.currentMurderer.home.telephones[0];
			}
			if (telephone == null)
			{
				telephone = Toolbox.Instance.GetClosestTelephone(this.currentMurderer, 100f, true, false, true);
			}
			Telephone closestTelephone = Toolbox.Instance.GetClosestTelephone(Player.Instance, 35f, true, false, true);
			if (closestTelephone != null && telephone != null)
			{
				if (Game.Instance.printDebug)
				{
					string text = "Murder: Triggered cover up telephone call at ";
					Vector3 wPos = closestTelephone.interactable.wPos;
					Game.Log(text + wPos.ToString(), 2);
				}
				this.successCall = TelephoneController.Instance.CreateNewCall(telephone, closestTelephone, null, Player.Instance, new TelephoneController.CallSource(TelephoneController.CallType.player, CitizenControls.Instance.coverUpSuccess, InteractionController.ConversationType.normal), 0.25f, false);
				return;
			}
			if (closestTelephone == null && Game.Instance.printDebug)
			{
				Game.Log("Murder: Unable to locate a public phone close to player; cannot trigger cover-up success call", 2);
			}
			if (telephone == null && Game.Instance.printDebug)
			{
				Game.Log("Murder: Unable to locate a public phone close to killer; cannot trigger cover-up success call", 2);
			}
		}
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0007931C File Offset: 0x0007751C
	public void OnCoverUpSuccessEnd()
	{
		this.triggerCoverUpSuccess = false;
		this.successCall = null;
		GameplayController.Instance.AddMoney(GameplayControls.Instance.coverUpReward, true, "Cover up complete");
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x00079348 File Offset: 0x00077548
	public void DisplayCoverUpTips()
	{
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.disposeOfBody, "", false, 0f, null, null, this.currentVictim.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.currentActiveCase.AddObjective("Hint: You can dispose of a body by throwing it into the sea or a dumpster", trigger, false, Vector3.zero, InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, null, false, true, true);
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x000793B3 File Offset: 0x000775B3
	public void AssignActiveCase(Case newCase)
	{
		if (this.currentActiveCase != newCase)
		{
			this.currentActiveCase = newCase;
		}
		this.UpdateResolveQuestions(true);
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x000793CC File Offset: 0x000775CC
	public void UpdateCorrectResolveAnswers()
	{
		if (this.currentActiveCase != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Chapter: Updating resolve answers on ",
				this.currentActiveCase.name,
				" with ",
				this.activeMurders.Count.ToString(),
				" active murders..."
			}), 2);
			foreach (Case.ResolveQuestion resolveQuestion in this.currentActiveCase.resolveQuestions)
			{
				resolveQuestion.correctAnswers.Clear();
				Game.Log("Chapter: Getting new correct answer for " + resolveQuestion.name + "...", 2);
				if (resolveQuestion.tag == JobPreset.JobTag.A)
				{
					using (List<MurderController.Murder>.Enumerator enumerator2 = this.activeMurders.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							MurderController.Murder murder = enumerator2.Current;
							if (!resolveQuestion.correctAnswers.Contains(murder.murdererID.ToString()))
							{
								Game.Log("Chapter: Question A: Found murderer ID " + murder.murdererID.ToString() + "...", 2);
								resolveQuestion.correctAnswers.Add(murder.murdererID.ToString());
							}
						}
						goto IL_63A;
					}
					goto IL_12B;
				}
				goto IL_12B;
				IL_63A:
				resolveQuestion.UpdateValid(this.currentActiveCase);
				continue;
				IL_12B:
				if (resolveQuestion.tag == JobPreset.JobTag.C)
				{
					using (List<MurderController.Murder>.Enumerator enumerator2 = this.activeMurders.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							MurderController.Murder murder2 = enumerator2.Current;
							Game.Log("Chapter: Question C: Found murderer " + murder2.murderer.GetCitizenName() + "...", 2);
							if (murder2.murderer != null && murder2.murderer.home != null && !resolveQuestion.correctAnswers.Contains(murder2.murderer.home.id.ToString()))
							{
								resolveQuestion.correctAnswers.Add(murder2.murderer.home.id.ToString());
							}
						}
						goto IL_63A;
					}
				}
				if (resolveQuestion.tag == JobPreset.JobTag.E)
				{
					if (this.currentActiveCase.caseType == Case.CaseType.mainStory)
					{
						if (!(ChapterController.Instance.chapterScript != null))
						{
							goto IL_63A;
						}
						ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
						if (chapterIntro != null && chapterIntro.murderWeapon != null)
						{
							resolveQuestion.correctAnswers.Add(chapterIntro.murderWeapon.evidence.evID);
							Game.Log("Chapter: Question E: Found murder weapon...", 2);
							goto IL_63A;
						}
						goto IL_63A;
					}
					else
					{
						using (List<MurderController.Murder>.Enumerator enumerator2 = this.activeMurders.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								MurderController.Murder murder3 = enumerator2.Current;
								if (murder3.weapon != null)
								{
									Evidence evidence = murder3.weapon.evidence;
									if (evidence != null)
									{
										if (!resolveQuestion.correctAnswers.Contains(evidence.evID))
										{
											resolveQuestion.correctAnswers.Add(evidence.evID);
											Game.Log("Chapter: Question E: Found murder weapon...", 2);
										}
									}
									else if (!resolveQuestion.correctAnswers.Contains(murder3.weapon.id.ToString()))
									{
										resolveQuestion.correctAnswers.Add(murder3.weapon.id.ToString());
										Game.Log("Chapter: Question E: Found murder weapon...", 2);
									}
								}
							}
							goto IL_63A;
						}
					}
				}
				if (resolveQuestion.tag == JobPreset.JobTag.F)
				{
					if (resolveQuestion.inputtedEvidence == null || resolveQuestion.inputtedEvidence.Length <= 0)
					{
						goto IL_63A;
					}
					Evidence evidence2 = null;
					if (!GameplayController.Instance.evidenceDictionary.TryGetValue(resolveQuestion.inputtedEvidence, ref evidence2))
					{
						goto IL_63A;
					}
					EvidenceFingerprint evidenceFingerprint = evidence2 as EvidenceFingerprint;
					if (evidenceFingerprint != null)
					{
						if (this.currentActiveCase.caseType == Case.CaseType.mainStory)
						{
							if (ChapterController.Instance.chapterScript != null)
							{
								ChapterIntro chapterIntro2 = ChapterController.Instance.chapterScript as ChapterIntro;
								if (chapterIntro2 != null && chapterIntro2.killer != null && chapterIntro2.killer.evidenceEntry.GetTiedKeys(Evidence.DataKey.name).Contains(Evidence.DataKey.fingerprints) && chapterIntro2.killer == evidenceFingerprint.belongsTo)
								{
									resolveQuestion.correctAnswers.Add(resolveQuestion.inputtedEvidence);
									Game.Log("Chapter: Question F: Found valid print...", 2);
								}
							}
						}
						else
						{
							foreach (MurderController.Murder murder4 in this.activeMurders)
							{
								if (murder4.murderer != null && murder4.murderer.evidenceEntry.GetTiedKeys(Evidence.DataKey.name).Contains(Evidence.DataKey.fingerprints) && evidenceFingerprint.belongsTo == murder4.murderer)
								{
									resolveQuestion.correctAnswers.Add(resolveQuestion.inputtedEvidence);
									Game.Log("Chapter: Question F: Found valid print...", 2);
								}
							}
						}
					}
					if (evidence2.interactable == null)
					{
						goto IL_63A;
					}
					using (List<MurderController.Murder>.Enumerator enumerator2 = this.activeMurders.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							MurderController.Murder murder5 = enumerator2.Current;
							if (murder5.murderer != null && murder5.murderer.evidenceEntry.GetTiedKeys(Evidence.DataKey.name).Contains(Evidence.DataKey.fingerprints) && murder5.callingCardID == evidence2.interactable.id)
							{
								resolveQuestion.correctAnswers.Add(resolveQuestion.inputtedEvidence);
								Game.Log("Chapter: Question F: Found valid calling card...", 2);
							}
						}
						goto IL_63A;
					}
				}
				if (resolveQuestion.tag == JobPreset.JobTag.G)
				{
					foreach (MurderController.Murder murder6 in this.activeMurders)
					{
						Game.Log("Chapter: Question G: Found murderer " + murder6.murderer.GetCitizenName() + "...", 2);
						if (murder6.murderer != null && murder6.murderer.den != null && !resolveQuestion.correctAnswers.Contains(murder6.murderer.den.id.ToString()))
						{
							resolveQuestion.correctAnswers.Add(murder6.murderer.den.id.ToString());
						}
					}
					goto IL_63A;
				}
				goto IL_63A;
			}
		}
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x00079AE8 File Offset: 0x00077CE8
	public void UpdateResolveQuestions(bool clearExisting)
	{
		if (this.currentActiveCase == null)
		{
			Game.Log("Gameplay: Cannot update resolve questions; no active case", 2);
			return;
		}
		Game.Log("Gameplay: Updating resolve questions...", 2);
		if (clearExisting)
		{
			this.currentActiveCase.resolveQuestions.Clear();
		}
		MurderController.Murder currentMurder = this.GetCurrentMurder();
		if (currentMurder != null)
		{
			if (currentMurder.preset.useCustomResolveQuestions)
			{
				using (List<Case.ResolveQuestion>.Enumerator enumerator = currentMurder.preset.customResolveQuestions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Case.ResolveQuestion resolveQuestion = enumerator.Current;
						Case.ResolveQuestion resolveQuestion2 = new Case.ResolveQuestion();
						resolveQuestion2.name = resolveQuestion.name;
						resolveQuestion2.inputType = resolveQuestion.inputType;
						resolveQuestion2.tag = resolveQuestion.tag;
						resolveQuestion2.rewardRange = resolveQuestion.rewardRange;
						resolveQuestion2.isOptional = resolveQuestion.isOptional;
						resolveQuestion2.displayObjective = resolveQuestion.displayObjective;
						resolveQuestion2.displayAtPhase = resolveQuestion.displayAtPhase;
						resolveQuestion2.displayOnlyAtPhase = resolveQuestion.displayOnlyAtPhase;
						resolveQuestion2.objectiveDelay = resolveQuestion.objectiveDelay;
						resolveQuestion2.revengeObjective = resolveQuestion.revengeObjective;
						resolveQuestion2.reward = Mathf.RoundToInt(Toolbox.Instance.VectorToRandom(resolveQuestion.rewardRange) * Game.Instance.jobRewardMultiplier / 50f) * 50;
						resolveQuestion2.penalty = Mathf.RoundToInt(Toolbox.Instance.VectorToRandom(resolveQuestion.penaltyRange) * Game.Instance.jobPenaltyMultiplier / 50f) * 50;
						Game.Log("Adding resolve question: " + resolveQuestion2.name, 2);
						this.currentActiveCase.resolveQuestions.Add(resolveQuestion2);
					}
					goto IL_2F2;
				}
			}
			using (List<Case.ResolveQuestion>.Enumerator enumerator = GameplayControls.Instance.murderResolveQuestions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Case.ResolveQuestion resolveQuestion3 = enumerator.Current;
					Case.ResolveQuestion resolveQuestion4 = new Case.ResolveQuestion();
					resolveQuestion4.name = resolveQuestion3.name;
					resolveQuestion4.inputType = resolveQuestion3.inputType;
					resolveQuestion4.tag = resolveQuestion3.tag;
					resolveQuestion4.rewardRange = resolveQuestion3.rewardRange;
					resolveQuestion4.isOptional = resolveQuestion3.isOptional;
					resolveQuestion4.displayObjective = resolveQuestion3.displayObjective;
					resolveQuestion4.displayAtPhase = resolveQuestion3.displayAtPhase;
					resolveQuestion4.displayOnlyAtPhase = resolveQuestion3.displayOnlyAtPhase;
					resolveQuestion4.objectiveDelay = resolveQuestion3.objectiveDelay;
					resolveQuestion4.revengeObjective = resolveQuestion3.revengeObjective;
					resolveQuestion4.reward = Mathf.RoundToInt(Toolbox.Instance.VectorToRandom(resolveQuestion3.rewardRange) * Game.Instance.jobRewardMultiplier / 50f) * 50;
					resolveQuestion4.penalty = Mathf.RoundToInt(Toolbox.Instance.VectorToRandom(resolveQuestion3.penaltyRange) * Game.Instance.jobPenaltyMultiplier / 50f) * 50;
					Game.Log("Adding resolve question: " + resolveQuestion4.name, 2);
					this.currentActiveCase.resolveQuestions.Add(resolveQuestion4);
				}
				goto IL_2F2;
			}
		}
		Game.Log("Gameplay: Cannot update resolve questions; no current murder!", 2);
		IL_2F2:
		this.UpdateCorrectResolveAnswers();
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x00079E24 File Offset: 0x00078024
	public void PickNewVictim()
	{
		Game.Log("Murder: Picking new victim...", 2);
		float num = -99999f;
		this.currentVictim = null;
		this.sniperVictimSites.Clear();
		this.currentVictimSite = null;
		foreach (Citizen citizen in CityData.Instance.citizenDirectory)
		{
			if (!citizen.isPlayer && !citizen.isDead && !(this.currentMurderer == citizen) && !citizen.isHomeless && !this.previousMurderers.Contains(citizen))
			{
				NewGameLocation newGameLocation = null;
				float num2 = 0f;
				if (this.murderPreset.caseType == MurderPreset.CaseType.sniper)
				{
					if (this.chosenMO.requiresSniperVantageAtHome)
					{
						if (!(citizen.home != null))
						{
							continue;
						}
						if (this.sniperVictimSites == null || this.sniperVictimSites.Count <= 0)
						{
							NewWall newWall;
							float num3;
							Toolbox.Instance.TryGetSniperVantagePoint(this.currentMurderer.home, out newWall, out num3, out this.sniperVictimSites, null);
						}
						if (this.sniperVictimSites.Contains(citizen.home) && this.chosenMO.allowHome)
						{
							newGameLocation = citizen.home;
						}
						else
						{
							if (citizen.job == null || citizen.job.employer == null || !this.sniperVictimSites.Contains(citizen.job.employer.placeOfBusiness) || !this.chosenMO.allowWork)
							{
								continue;
							}
							newGameLocation = citizen.job.employer.placeOfBusiness;
						}
					}
					else
					{
						HashSet<NewGameLocation> hashSet = new HashSet<NewGameLocation>();
						if (citizen.job != null && citizen.job.employer != null && citizen.job.employer.placeOfBusiness != null && this.chosenMO.allowWork)
						{
							if (citizen.home != null)
							{
								PathFinder.PathData path = PathFinder.Instance.GetPath(citizen.FindSafeTeleport(citizen.home, false, true), citizen.FindSafeTeleport(citizen.job.employer.placeOfBusiness, false, true), citizen, null);
								if (path != null)
								{
									foreach (NewNode.NodeAccess nodeAccess in path.accessList)
									{
										if (!hashSet.Contains(nodeAccess.toNode.gameLocation))
										{
											NewWall newWall2 = null;
											float num4 = 0f;
											if (Toolbox.Instance.TryGetSniperVantagePoint(this.currentMurderer, nodeAccess.toNode.gameLocation, out newWall2, out num4, path.accessList) && newWall2 != null && num4 >= num2)
											{
												num2 = num4;
												newGameLocation = nodeAccess.toNode.gameLocation;
											}
											hashSet.Add(nodeAccess.toNode.gameLocation);
										}
									}
								}
							}
							if (!hashSet.Contains(citizen.job.employer.placeOfBusiness) && newGameLocation == null)
							{
								NewWall newWall3 = null;
								float num5 = 0f;
								if (Toolbox.Instance.TryGetSniperVantagePoint(this.currentMurderer, citizen.job.employer.placeOfBusiness, out newWall3, out num5, null) && newWall3 != null && num5 >= num2)
								{
									num2 = num5;
									newGameLocation = citizen.job.employer.placeOfBusiness;
								}
								hashSet.Add(citizen.job.employer.placeOfBusiness);
							}
						}
						if (this.chosenMO.allowStreets && newGameLocation == null)
						{
							foreach (StreetController streetController in CityData.Instance.streetDirectory)
							{
								if (!hashSet.Contains(streetController))
								{
									NewWall newWall4 = null;
									float num6 = 0f;
									if (Toolbox.Instance.TryGetSniperVantagePoint(this.currentMurderer, streetController, out newWall4, out num6, null) && newWall4 != null && num6 >= num2)
									{
										num2 = num6;
										newGameLocation = streetController;
									}
									hashSet.Add(streetController);
								}
							}
						}
						if (this.chosenMO.allowPublic && newGameLocation == null)
						{
							foreach (Company company in CityData.Instance.companyDirectory)
							{
								if (company.publicFacing && company.placeOfBusiness != null && !hashSet.Contains(company.placeOfBusiness))
								{
									NewWall newWall5 = null;
									float num7 = 0f;
									if (Toolbox.Instance.TryGetSniperVantagePoint(this.currentMurderer, company.placeOfBusiness, out newWall5, out num7, null) && newWall5 != null && num7 >= num2)
									{
										num2 = num7;
										newGameLocation = company.placeOfBusiness;
									}
									hashSet.Add(company.placeOfBusiness);
								}
							}
						}
					}
					if (newGameLocation == null)
					{
						continue;
					}
					Game.Log("Murder: Suggested sniper victim site of: " + newGameLocation.name + " " + num2.ToString(), 2);
				}
				float num8 = Toolbox.Instance.Rand(this.chosenMO.victimRandomScoreRange.x, this.chosenMO.victimRandomScoreRange.y, false) + num2;
				if (this.currentMurderer.attractedTo.Contains(citizen.gender))
				{
					num8 += (float)this.chosenMO.attractedToSuitabilityBoost;
				}
				Acquaintance acquaintance = null;
				if (this.currentMurderer.FindAcquaintanceExists(citizen, out acquaintance))
				{
					num8 += (float)this.chosenMO.acquaintedSuitabilityBoost;
					num8 += (float)this.chosenMO.likeSuitabilityBoost * acquaintance.like;
				}
				if (citizen.job != null && this.currentMurderer != null && this.currentMurderer.job != null && citizen.job.employer != null && citizen.job.employer == this.currentMurderer.job.employer)
				{
					num8 += (float)this.chosenMO.sameWorkplaceBoost;
				}
				if (this.chosenMO.murdererIsTenantBoost != 0 && citizen.GetLandlord() == this.currentMurderer)
				{
					num8 += (float)this.chosenMO.murdererIsTenantBoost;
				}
				float num9 = 0f;
				if (this.TraitTest(citizen, ref this.chosenMO.victimTraitModifiers, out num9))
				{
					num8 += num9;
					foreach (MurderMO.JobModifier jobModifier in this.chosenMO.victimJobModifiers)
					{
						if (citizen.job != null && jobModifier.jobs.Contains(citizen.job.preset))
						{
							num8 += (float)jobModifier.jobBoost;
						}
					}
					foreach (MurderMO.CompanyModifier companyModifier in this.chosenMO.victimCompanyModifiers)
					{
						if (citizen.job != null && citizen.job.employer != null && companyModifier.companies.Contains(citizen.job.employer.preset))
						{
							List<Occupation> list = citizen.job.employer.companyRoster.FindAll((Occupation item) => item.employee != null);
							if (list.Count >= companyModifier.mininumEmployees)
							{
								num8 += (float)companyModifier.companyBoost;
								num8 += (float)((list.Count - companyModifier.mininumEmployees) * companyModifier.boostPerEmployeeOverMinimum);
							}
						}
					}
					if (this.chosenMO.useVictimSocialClassRange && citizen.societalClass >= this.chosenMO.victimClassRange.x && citizen.societalClass <= this.chosenMO.victimClassRange.y)
					{
						num8 += (float)this.chosenMO.victimClassRangeBoost;
					}
					if (num8 > num)
					{
						this.currentVictim = citizen;
						num = num8;
						if (newGameLocation != null)
						{
							this.currentVictimSite = newGameLocation;
						}
					}
				}
			}
		}
		if (this.currentVictim == null)
		{
			Game.LogError("Cannot find new valid victim!", 2);
			return;
		}
		Game.Log("Murder: Chosen " + this.currentVictim.GetCitizenName() + " to be new victim...", 2);
		if (this.currentVictim.home != null)
		{
			Game.Log("Murder: Victim " + this.currentVictim.GetCitizenName() + " lives at " + this.currentVictim.home.name, 2);
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0007A74C File Offset: 0x0007894C
	public bool TraitTest(Citizen cit, ref List<MurderPreset.MurdererModifierRule> rules, out float output)
	{
		output = 1f;
		bool result = true;
		foreach (MurderPreset.MurdererModifierRule murdererModifierRule in rules)
		{
			bool flag = false;
			if (murdererModifierRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = murdererModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (!(searchTrait == null) && cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = true;
							break;
						}
					}
					goto IL_216;
				}
				goto IL_98;
			}
			goto IL_98;
			IL_216:
			if (flag)
			{
				output += murdererModifierRule.scoreModifier;
				continue;
			}
			if (murdererModifierRule.mustPassForApplication)
			{
				result = false;
				continue;
			}
			continue;
			IL_98:
			if (murdererModifierRule.rule == CharacterTrait.RuleType.ifAllOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = murdererModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (!(searchTrait == null) && !cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_216;
				}
			}
			if (murdererModifierRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = murdererModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (!(searchTrait == null) && cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_216;
				}
			}
			if (murdererModifierRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
			{
				if (cit.partner != null)
				{
					using (List<CharacterTrait>.Enumerator enumerator2 = murdererModifierRule.traitList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CharacterTrait searchTrait = enumerator2.Current;
							if (!(searchTrait == null) && cit.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
							{
								flag = true;
								break;
							}
						}
						goto IL_216;
					}
				}
				flag = false;
				goto IL_216;
			}
			goto IL_216;
		}
		return result;
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0007AA24 File Offset: 0x00078C24
	public MurderController.Murder ExecuteNewMurder(Human newMurderer, Human newVictim, MurderPreset preset, MurderMO motive, NewGameLocation victimSite = null)
	{
		if (Game.Instance.printDebug)
		{
			Game.Log(string.Concat(new string[]
			{
				"Murder: Creating new murder of type ",
				preset.name,
				" with murderer ",
				newMurderer.GetCitizenName(),
				" and victim ",
				newVictim.GetCitizenName(),
				" with victim site ",
				(victimSite != null) ? victimSite.ToString() : null
			}), 2);
		}
		MurderController.Murder result = new MurderController.Murder(newMurderer, newVictim, preset, motive, victimSite);
		this.murderRoutineActive = true;
		this.SetUpdateEnabled(true);
		return result;
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0007AAB4 File Offset: 0x00078CB4
	private void Update()
	{
		int num = 0;
		for (int i = 0; i < this.activeMurders.Count; i++)
		{
			MurderController.Murder m = this.activeMurders[i];
			if (m.state == MurderController.MurderState.acquireEuipment && !m.preset.requiresAcquirePhase)
			{
				m.SetMurderState(MurderController.MurderState.research, false);
			}
			if (m.state == MurderController.MurderState.research)
			{
				if (!m.preset.requiresResearchPhase)
				{
					m.SetMurderState(MurderController.MurderState.waitForLocation, false);
				}
				else if (m.murderer.inventory.Exists((Interactable item) => item.preset == m.weaponPreset))
				{
					if (m.ammoPreset != null)
					{
						if (m.murderer.inventory.Exists((Interactable item) => item.preset == m.ammoPreset))
						{
							m.SetMurderState(MurderController.MurderState.waitForLocation, false);
						}
					}
					else
					{
						m.SetMurderState(MurderController.MurderState.waitForLocation, false);
					}
				}
			}
			if (m.state == MurderController.MurderState.waitForLocation && SessionData.Instance.play)
			{
				if (m.IsValidLocation(m.victim.currentGameLocation))
				{
					m.SetMurderLocation(m.victim.currentGameLocation);
					m.SetMurderState(MurderController.MurderState.travellingTo, false);
				}
				else if (SessionData.Instance.gameTime - m.waitingTimestamp > 0.25f)
				{
					if (m.preset.caseType == MurderPreset.CaseType.murder || m.preset.caseType == MurderPreset.CaseType.kidnap)
					{
						if ((m.mo.allowAnywhere || m.mo.allowHome) && m.victim.home != null)
						{
							if (!m.victim.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.toGoGoal && item.passedNode != null && item.passedNode.gameLocation == m.victim.home))
							{
								Game.Log("Murder: Waiting for too long! Creating GoTo home routine for victim " + m.victim.GetCitizenName() + " to: " + m.victim.home.name, 2);
								NewAIController ai = m.victim.ai;
								AIGoalPreset toGoGoal = RoutineControls.Instance.toGoGoal;
								float newTrigerTime = 0f;
								float newDuration = 0f;
								NewGameLocation newPassedGameLocation = m.victim.home;
								ai.CreateNewGoal(toGoGoal, newTrigerTime, newDuration, m.victim.FindSafeTeleport(m.victim.home, false, true), null, newPassedGameLocation, null, null, -2);
							}
						}
						else if ((m.mo.allowAnywhere || m.mo.allowWork) && m.victim.job != null && m.victim.job.employer != null && m.victim.job.employer.placeOfBusiness != null && m.victim.job.employer.openForBusinessDesired && m.victim.job.employer.placeOfBusiness.building != null && m.victim.job.employer.placeOfBusiness.building.preset.presetName != "CityHall")
						{
							if (!m.victim.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.toGoGoal && item.passedNode != null && item.passedNode.gameLocation == m.victim.job.employer.placeOfBusiness))
							{
								Game.Log("Murder: Waiting for too long! Creating GoTo work routine for victim " + m.victim.GetCitizenName() + " to: " + m.victim.job.employer.placeOfBusiness.name, 2);
								NewAIController ai2 = m.victim.ai;
								AIGoalPreset toGoGoal2 = RoutineControls.Instance.toGoGoal;
								float newTrigerTime2 = 0f;
								float newDuration2 = 0f;
								NewGameLocation newPassedGameLocation = m.victim.job.employer.placeOfBusiness;
								ai2.CreateNewGoal(toGoGoal2, newTrigerTime2, newDuration2, m.victim.FindSafeTeleport(m.victim.job.employer.placeOfBusiness, false, true), null, newPassedGameLocation, null, null, -2);
							}
						}
						else if (m.mo.allowAnywhere || m.mo.allowStreets)
						{
							StreetController streetController = null;
							foreach (StreetController streetController2 in CityData.Instance.streetDirectory)
							{
								if (streetController == null || (streetController2.currentOccupants.Count <= m.preset.nonHomeMaximumOccupantsTrigger && streetController2.currentOccupants.Count < streetController.currentOccupants.Count))
								{
									streetController = streetController2;
								}
							}
							if (streetController != null)
							{
								if (!m.victim.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.toGoGoal))
								{
									Game.Log("Murder: Waiting for too long! Creating GoTo street routine for victim " + m.victim.GetCitizenName() + " to: " + streetController.name, 2);
									NewAIController ai3 = m.victim.ai;
									AIGoalPreset toGoGoal3 = RoutineControls.Instance.toGoGoal;
									float newTrigerTime3 = 0f;
									float newDuration3 = 0f;
									NewGameLocation newPassedGameLocation = streetController;
									ai3.CreateNewGoal(toGoGoal3, newTrigerTime3, newDuration3, m.victim.FindSafeTeleport(streetController, false, true), null, newPassedGameLocation, null, null, -2);
								}
							}
						}
						else if ((m.mo.allowAnywhere || m.mo.allowDen) && m.murderer.den != null && !m.victim.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.toGoGoal && item.passedNode != null && item.passedNode.gameLocation == m.murderer.den))
						{
							Game.Log("Murder: Waiting for too long! Creating GoTo den routine for victim " + m.victim.GetCitizenName() + " to: " + m.murderer.den.name, 2);
							NewAIController ai4 = m.victim.ai;
							AIGoalPreset toGoGoal4 = RoutineControls.Instance.toGoGoal;
							float newTrigerTime4 = 0f;
							float newDuration4 = 0f;
							NewGameLocation newPassedGameLocation = m.murderer.den;
							ai4.CreateNewGoal(toGoGoal4, newTrigerTime4, newDuration4, m.victim.FindSafeTeleport(m.murderer.den, false, true), null, newPassedGameLocation, null, null, -2);
						}
					}
					else if (m.preset.caseType == MurderPreset.CaseType.sniper && m.sniperVictimSite != null && !m.victim.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.toGoGoal && item.passedNode != null && item.passedNode.gameLocation == m.sniperVictimSite))
					{
						string text = "Murder: Waiting for too long! Creating GoTo VictimSite routine for victim ";
						string citizenName = m.victim.GetCitizenName();
						string text2 = " to: ";
						NewGameLocation sniperVictimSite = m.sniperVictimSite;
						Game.Log(text + citizenName + text2 + ((sniperVictimSite != null) ? sniperVictimSite.ToString() : null), 2);
						NewAIController ai5 = m.victim.ai;
						AIGoalPreset toGoGoal5 = RoutineControls.Instance.toGoGoal;
						float newTrigerTime5 = 0f;
						float newDuration5 = 0f;
						NewGameLocation newPassedGameLocation = m.sniperVictimSite;
						ai5.CreateNewGoal(toGoGoal5, newTrigerTime5, newDuration5, m.victim.FindSafeTeleport(m.sniperVictimSite, false, true), null, newPassedGameLocation, null, null, -2);
					}
				}
			}
			if (m.state == MurderController.MurderState.travellingTo && SessionData.Instance.play)
			{
				if (m.IsValidLocation(m.victim.currentGameLocation))
				{
					if (m.preset.travelSuccessTrigger == MurderPreset.SuccessfulTravelTrigger.whenMurdererIsAtTheSameLocation)
					{
						if (m.murderer.currentGameLocation == m.victim.currentGameLocation)
						{
							Game.Log("Murder: Murderer location: " + m.murderer.currentGameLocation.name + " victim location: " + m.victim.currentGameLocation.name, 2);
							m.SetMurderLocation(m.murderer.currentGameLocation);
							m.SetMurderState(MurderController.MurderState.executing, false);
						}
					}
					else if (m.preset.travelSuccessTrigger == MurderPreset.SuccessfulTravelTrigger.whenMurdererIsAtVantagePoint && m.murderer.ai.currentAction != null && m.murderer.ai.currentAction.isAtLocation && m.murderer.ai.currentAction.preset.allowSniperShot && m.victim.currentGameLocation == m.sniperVictimSite)
					{
						bool flag = false;
						foreach (NewRoom newRoom in m.sniperVictimSite.rooms)
						{
							if (!m.cullingActiveRooms.Contains(newRoom.roomID))
							{
								m.cullingActiveRooms.Add(newRoom.roomID);
								flag = true;
							}
						}
						if (flag)
						{
							Player.Instance.UpdateCulling();
						}
						if (m.victim.isAsleep)
						{
							m.victim.WakeUp(false);
							m.victim.AddEnergy(0.5f);
						}
						if (m.victim.ai.ko)
						{
							return;
						}
						if (this.sniperShotDelay > 0f && m.victim.currentRoom != null)
						{
							if (Game.Instance.printDebug)
							{
								Game.Log("Murder: Sniper shot delay: " + this.sniperShotDelay.ToString(), 2);
							}
							if (m.victim.currentRoom.mainLightStatus || m.victim.currentRoom.IsOutside())
							{
								this.sniperShotDelay -= Time.deltaTime;
								return;
							}
							this.sniperShotDelay -= Time.deltaTime * 0.1f;
							return;
						}
						else
						{
							Transform transform = m.victim.lookAtThisTransform;
							if (m.victim.outfitController != null)
							{
								if (this.limbTargetCycleCounter >= 1f)
								{
									this.limbTargetCycle++;
									if (this.limbTargetCycle > 5)
									{
										this.limbTargetCycle = 0;
									}
									this.limbTargetCycleCounter = 0f;
								}
								if (this.limbTargetCycle == 0)
								{
									transform = m.victim.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.Head);
								}
								else if (this.limbTargetCycle == 1)
								{
									transform = m.victim.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.upperTorso);
								}
								else if (this.limbTargetCycle == 2)
								{
									transform = m.victim.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.lowerTorso);
								}
								else if (this.limbTargetCycle == 3)
								{
									transform = m.victim.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.UpperLegLeft);
								}
								else if (this.limbTargetCycle == 4)
								{
									transform = m.victim.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.Head);
								}
								else if (this.limbTargetCycle == 5)
								{
									transform = m.victim.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.UpperLegRight);
								}
							}
							if (Game.Instance.printDebug)
							{
								Game.Log("Murder: Sniper attempting to line up shot for " + transform.name + " at " + transform.position.ToString(), 2);
							}
							Vector3 vector = m.murderer.lookAtThisTransform.position + (transform.position - m.murderer.lookAtThisTransform.position).normalized * 6f;
							Vector3 vector2 = transform.position - vector;
							float num2 = Vector3.Distance(transform.position, vector) + 2f;
							Ray ray;
							ray..ctor(vector, vector2);
							bool flag2 = false;
							List<RaycastHit> list = Enumerable.ToList<RaycastHit>(Physics.RaycastAll(ray, num2, Toolbox.Instance.sniperLOSMask, 1));
							list.Sort((RaycastHit p1, RaycastHit p2) => p1.distance.CompareTo(p2.distance));
							for (int j = 0; j < list.Count; j++)
							{
								RaycastHit confirmationHit = list[j];
								if (confirmationHit.transform.gameObject.layer == 27)
								{
									NewBuilding componentInParent = confirmationHit.transform.gameObject.GetComponentInParent<NewBuilding>();
									if (!(componentInParent != null) || (!(componentInParent == m.murderer.currentBuilding) && !(componentInParent == m.victim.currentBuilding)))
									{
										Debug.DrawLine(ray.origin, confirmationHit.point, Color.red, 4f);
										if (Game.Instance.printDebug)
										{
											Game.Log("Murder: Sniper has collider blocking LoS: " + confirmationHit.transform.name, 2);
											break;
										}
										break;
									}
								}
								else if (confirmationHit.transform.gameObject.layer == 24 || confirmationHit.transform.gameObject.layer == 7)
								{
									InteractableController component = confirmationHit.transform.gameObject.GetComponent<InteractableController>();
									if (component != null)
									{
										if (component.interactable == m.victim.interactable || component.interactable == m.victim.leftHandInteractable || component.interactable == m.victim.rightHandInteractable)
										{
											foreach (RaycastHit raycastHit in list)
											{
												if (Game.Instance.printDebug)
												{
													Game.Log(string.Concat(new string[]
													{
														"Murder: Sniper shot should hit: ",
														raycastHit.transform.name,
														" (",
														raycastHit.transform.gameObject.layer.ToString(),
														"): ",
														raycastHit.point.ToString(),
														"/",
														raycastHit.distance.ToString()
													}), 2);
												}
											}
											Debug.DrawLine(ray.origin, confirmationHit.point, Color.green, 4f);
											this.ExecuteSniperShot(m.victim, m.murderer, ray, confirmationHit, transform);
											flag2 = true;
											break;
										}
										if (Game.Instance.printDebug)
										{
											Game.Log("Murder: Sniper would hit wrong victim!", 2);
										}
									}
									else if (Game.Instance.printDebug)
									{
										Game.Log("Murder: Sniper would hit the citizen layer, but cannot find an interactablecontroller!", 2);
									}
								}
								else if (!confirmationHit.transform.CompareTag("RainWindowGlass"))
								{
									Debug.DrawLine(ray.origin, confirmationHit.point, Color.red, 4f);
									if (m.victim.ai.currentAction != null && Game.Instance.printDebug)
									{
										Game.Log(string.Concat(new string[]
										{
											"Murder: Sniper has collider blocking LoS: ",
											confirmationHit.transform.name,
											" ",
											confirmationHit.point.ToString(),
											" (Target: ",
											m.victim.ai.currentAction.preset.presetName,
											" at ",
											m.victim.currentRoom.GetName(),
											") + (",
											transform.name,
											") ",
											transform.position.ToString()
										}), 2);
										break;
									}
									break;
								}
							}
							if (flag2)
							{
								this.sniperShotDelay = Toolbox.Instance.Rand(2.5f, Mathf.Lerp(6f, 2.5f, m.murderer.combatSkill), true);
							}
							else
							{
								this.limbTargetCycleCounter += Time.deltaTime;
							}
							if (m.victim.isDead)
							{
								m.SetMurderState(MurderController.MurderState.executing, false);
							}
						}
					}
				}
				else if (m.preset.travelSuccessTrigger == MurderPreset.SuccessfulTravelTrigger.whenMurdererIsAtTheSameLocation)
				{
					if (m.location != null && m.location != m.victim.home && m.location.currentOccupants.Count > m.preset.nonHomeMaximumOccupantsCancel)
					{
						Game.Log(string.Concat(new string[]
						{
							"Murder: Cancelled murder because of too many occupants at ",
							m.location.name,
							" (",
							m.location.currentOccupants.Count.ToString(),
							"/",
							m.preset.nonHomeMaximumOccupantsTrigger.ToString(),
							")"
						}), 2);
						m.SetMurderLocation(null);
						m.SetMurderState(MurderController.MurderState.waitForLocation, false);
					}
					else if (m.murderer.currentGameLocation == m.location && m.victim.currentGameLocation != m.location)
					{
						Game.Log("Murder: Cancelled murder because victim is not at " + m.location.name + " when killer arrived.", 2);
						m.SetMurderLocation(null);
						m.SetMurderState(MurderController.MurderState.waitForLocation, false);
					}
				}
				else if (m.preset.travelSuccessTrigger == MurderPreset.SuccessfulTravelTrigger.whenMurdererIsAtVantagePoint && m.murderer.currentGameLocation == m.location && m.victim.currentGameLocation != m.location)
				{
					Game.Log("Murder: Cancelled murder because victim is not at " + m.location.name + " when killer arrived at the vantage point.", 2);
					m.SetMurderLocation(null);
					m.SetMurderState(MurderController.MurderState.waitForLocation, false);
				}
			}
			if (m.state == MurderController.MurderState.executing)
			{
				if (m.victim.isDead)
				{
					if (m.victim.isDead)
					{
						this.OnVictimKilled();
					}
					m.SetMurderState(MurderController.MurderState.post, false);
				}
				else if (SessionData.Instance.play)
				{
					if (m.murderer.currentNerve < m.murderer.maxNerve)
					{
						m.murderer.SetNerve(m.murderer.maxNerve);
					}
					if (m.preset.caseType == MurderPreset.CaseType.kidnap && !m.kidnapKillPhase && m.victim.ai.ko)
					{
						Game.Log("Murder: Victim is knocked out and restrained", 2);
						if (m.ransomPhase != MurderController.KidnapRansomPhase.freeingVictim)
						{
							m.victim.ai.SetRestrained(true, 100000000f);
						}
						m.SetMurderState(MurderController.MurderState.post, false);
					}
					else if (m.preset.caseType == MurderPreset.CaseType.kidnap && m.kidnapKillPhase && m.victim.ai.restrained)
					{
						Game.Log("Murder: Setting victim unrestrained...", 2);
						m.victim.ai.SetRestrained(false, 0f);
					}
					else if (m.weaponPreset.weapon.type != MurderWeaponPreset.WeaponType.poison || m.victim.poisoner == null || m.victim.poisoner != m.murderer)
					{
						if (!m.murderer.seesIllegal.ContainsKey(m.victim))
						{
							m.murderer.seesIllegal.Add(m.victim, 1f);
						}
						else
						{
							m.murderer.seesIllegal[m.victim] = 1f;
						}
						if (this.locationUpdateTimer <= 0f)
						{
							if (m.victim.currentNerve > 0f)
							{
								m.victim.SetNerve(0f);
								m.victim.OnZeroNerveReached();
							}
							m.murderer.ai.SetPersue(m.victim, false, 2, true, 10f);
							this.locationUpdateTimer = 1.5f;
						}
						else
						{
							this.locationUpdateTimer -= Time.deltaTime;
						}
					}
					else if (m.weaponPreset.weapon.type == MurderWeaponPreset.WeaponType.poison && m.victim.poisoner != null && m.victim.poisoner == m.murderer && m.murderer.ai.persuit)
					{
						Game.Log("Murder: Victim has been poisioned! Wait for them to die...", 2);
						m.murderer.ai.CancelPersue();
					}
				}
			}
			if (m.state == MurderController.MurderState.post)
			{
				if ((m.murderGoal.isActive && m.murderGoal.actions.Count <= 0) || m.preset.postActionSetup.Count <= 0)
				{
					m.SetMurderState(MurderController.MurderState.escaping, false);
				}
				else if (!m.murderer.ai.goals.Contains(m.murderGoal))
				{
					m.SetMurderState(MurderController.MurderState.escaping, false);
				}
			}
			if (m.state == MurderController.MurderState.escaping)
			{
				if (m.murderer.currentGameLocation != m.location)
				{
					bool flag3 = true;
					foreach (NewNode.NodeAccess nodeAccess in m.location.entrances)
					{
						if (m.murderer.currentGameLocation == nodeAccess.fromNode.gameLocation)
						{
							flag3 = false;
							break;
						}
						if (m.murderer.currentGameLocation == nodeAccess.toNode.gameLocation)
						{
							flag3 = false;
							break;
						}
					}
					if (flag3)
					{
						m.CancelCurrentMurder();
						i--;
					}
				}
			}
			else if (m.state == MurderController.MurderState.unsolved)
			{
				num++;
			}
		}
		if (this.activeMurders.Count <= 0 || (num > 0 && this.activeMurders.Count == num))
		{
			Game.Log("Murder: All murders completed or in unsolved state, deactivating murder controller update...", 2);
			if ((this.previousMurderers.Count > 0 || this.activeMurders.Count > 0) && this.murderPreset != null)
			{
				if (this.murderPreset.caseType == MurderPreset.CaseType.kidnap)
				{
					if (this.currentVictim != null && !this.currentVictim.isDead)
					{
						this.pauseBeforeKidnapperKill = this.murderPreset.kidnapperTimeUntilKill;
					}
					else
					{
						this.currentVictim = null;
						this.pauseBetweenMurders = this.murderPreset.minimumTimeBetweenMurders;
					}
				}
				else
				{
					this.currentVictim = null;
					this.pauseBetweenMurders = this.murderPreset.minimumTimeBetweenMurders;
				}
			}
			this.murderRoutineActive = false;
			this.SetUpdateEnabled(false);
		}
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0007C4A0 File Offset: 0x0007A6A0
	public void ExecuteSniperShot(Human victim, Human killer, Ray confirmationRay, RaycastHit confirmationHit, Transform victimTargetTransform)
	{
		Game.Log("Murder: Executing sniper killshot!", 2);
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.sniperKillShot, killer, killer.currentNode, confirmationRay.origin, null, null, 1f, null, false, null, false);
		if (Player.Instance.currentRoom != null && Player.Instance.currentRoom.IsOutside())
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.sniperStreetShot, killer, killer.currentNode, confirmationRay.origin, null, null, 1f, null, false, null, false);
		}
		Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(CriminalControls.Instance.sniperRifle.shellCasing, killer, killer, killer, killer.aimTransform.position, Vector3.zero, null, null, "");
		if (interactable != null)
		{
			interactable.MarkAsTrash(true, false, 0f);
			float num = 0.06f;
			if (victim.currentRoom != null && !victim.currentRoom.IsOutside() && !victim.currentRoom.mainLightStatus)
			{
				num = 0.12f;
			}
			Vector3 vector = killer.aimTransform.right * Toolbox.Instance.Rand(3.5f, 4.5f, false) + new Vector3(Toolbox.Instance.Rand(-num, num, false), Toolbox.Instance.Rand(-num, num, false), Toolbox.Instance.Rand(-num, num, false));
			if (interactable.controller != null)
			{
				interactable.controller.DropThis(false);
				interactable.controller.rb.AddForce(vector, 2);
			}
			else
			{
				interactable.ForcePhysicsActive(true, true, vector, 2, false);
			}
		}
		Vector3 vector2 = victimTargetTransform.position - killer.lookAtThisTransform.position;
		float num2 = Vector3.Distance(victimTargetTransform.position, killer.lookAtThisTransform.position) + 20f;
		Vector3 vector3 = killer.lookAtThisTransform.position + vector2.normalized * 6f;
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.streetSniperMuzzleFlash, PrefabControls.Instance.mapContainer);
		gameObject.transform.position = killer.lookAtThisTransform.position + vector2.normalized * 3f;
		gameObject.transform.rotation = Quaternion.LookRotation(vector2, gameObject.transform.up);
		Vector3 vector4 = victimTargetTransform.position - vector3 + new Vector3(Toolbox.Instance.Rand(-0.15f, 0.15f, true), Toolbox.Instance.Rand(-0.15f, 0.15f, true), Toolbox.Instance.Rand(-0.15f, 0.15f, true));
		Ray ray;
		ray..ctor(vector3, vector4);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, ref raycastHit, num2, Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			default(int),
			25
		}), 1))
		{
			Game.Log("Murder: Sniper hit: " + raycastHit.transform.name, 2);
			BreakableWindowController component = raycastHit.transform.gameObject.GetComponent<BreakableWindowController>();
			if (component != null)
			{
				component.AddBulletHole(CriminalControls.Instance.sniperRifle, raycastHit.point, ray.direction, killer, false, raycastHit.normal);
			}
		}
		List<RaycastHit> list = Enumerable.ToList<RaycastHit>(Physics.RaycastAll(ray, num2, Toolbox.Instance.sniperLOSMask, 1));
		list.Sort((RaycastHit p1, RaycastHit p2) => p1.distance.CompareTo(p2.distance));
		Predicate<MurderController.Murder> <>9__1;
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			if (hit.transform.gameObject.layer == 27)
			{
				NewBuilding componentInParent = hit.transform.gameObject.GetComponentInParent<NewBuilding>();
				if (!(componentInParent != null))
				{
					break;
				}
				if (!(componentInParent == killer.currentBuilding))
				{
					if (!(componentInParent == victim.currentBuilding))
					{
						break;
					}
				}
			}
			else if (hit.transform.gameObject.layer == 24 || hit.transform.gameObject.layer == 7)
			{
				InteractableController component2 = hit.transform.gameObject.GetComponent<InteractableController>();
				if (component2 != null && (component2.interactable == victim.interactable || component2.interactable == victim.leftHandInteractable || component2.interactable == victim.rightHandInteractable))
				{
					victim.RecieveDamage(victim.maximumHealth * 2f, killer, hit.point, ray.direction, CriminalControls.Instance.sniperRifle.forwardSpatter, CriminalControls.Instance.sniperRifle.backSpatter, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, true, true, 1f);
					if (CriminalControls.Instance.sniperRifle.impactEventBody != null)
					{
						AudioController.Instance.PlayWorldOneShot(CriminalControls.Instance.sniperRifle.impactEventBody, victim, victim.currentNode, hit.point, null, null, 1f, null, false, null, false);
					}
					if (CriminalControls.Instance.sniperRifle.entryWound != null)
					{
						(victim as Citizen).CreateWoundClosestToPoint(hit.point, hit.normal, CriminalControls.Instance.sniperRifle.entryWound, CriminalControls.Instance.sniperRifle);
					}
					List<MurderController.Murder> list2 = this.activeMurders;
					Predicate<MurderController.Murder> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = ((MurderController.Murder item) => item.victim == victim));
					}
					MurderController.Murder murder = list2.Find(predicate);
					if (murder != null && killer.currentNode != null)
					{
						murder.sniperKillShotNode = killer.currentNode.nodeCoord;
					}
				}
			}
			else if (!hit.transform.CompareTag("RainWindowGlass"))
			{
				Toolbox.Instance.CreateBulletSurfaceContactFX(CriminalControls.Instance.sniperRifle, hit);
				return;
			}
		}
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0007CADF File Offset: 0x0007ACDF
	public void SetUpdateEnabled(bool val)
	{
		Game.Log("Murder: Update Enabled: " + val.ToString(), 2);
		base.enabled = val;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0007CB00 File Offset: 0x0007AD00
	public void OnStartGame()
	{
		for (int i = 0; i < this.activeMurders.Count; i++)
		{
			MurderController.Murder murder = this.activeMurders[i];
			if (murder != null && murder.murderer != null)
			{
				Game.Log("Murder: Forcing update of priorities for " + murder.murderer.citizenName, 2);
				murder.murderer.ai.AITick(true, false);
			}
		}
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0007CB70 File Offset: 0x0007AD70
	[Button(null, 0)]
	public void TriggerNextMurder()
	{
		this.pauseBetweenMurders = 0f;
		if (this.currentActiveCase != null)
		{
			Objective objective = this.currentActiveCase.currentActiveObjectives.Find((Objective item) => item.queueElement.entryRef == "Explore Reported Crime Scene");
			if (objective != null)
			{
				objective.progress = 1f;
				objective.Complete();
			}
		}
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0007CBD4 File Offset: 0x0007ADD4
	public virtual void SpawnItemsCheck(MurderController.Murder murder)
	{
		if (this.murderPreset == null)
		{
			return;
		}
		bool firstMurder = true;
		if (this.activeMurders.Count >= 2)
		{
			firstMurder = false;
		}
		List<MurderPreset.MurderLeadItem> list = this.murderPreset.leads.FindAll((MurderPreset.MurderLeadItem item) => item.spawnOnPhase == murder.state && (firstMurder || item.tryToSpawnWithEachNewMurder));
		list.AddRange(murder.mo.MOleads.FindAll((MurderPreset.MurderLeadItem item) => item.spawnOnPhase == murder.state && (firstMurder || item.tryToSpawnWithEachNewMurder)));
		List<MurderPreset.MurderLeadItem> successsfullySpawned = new List<MurderPreset.MurderLeadItem>();
		int num = 999;
		while (list.Count > 0 && num > 0)
		{
			MurderPreset.MurderLeadItem spawn = list[0];
			if (!this.SpawnItemIsValid(murder, spawn, ref successsfullySpawned, !spawn.useOrGroup))
			{
				list.RemoveAt(0);
				num--;
			}
			else
			{
				List<MurderPreset.MurderLeadItem> list2 = null;
				if (spawn.useOrGroup)
				{
					list2 = new List<MurderPreset.MurderLeadItem>();
					List<MurderPreset.MurderLeadItem> list3 = list.FindAll((MurderPreset.MurderLeadItem item) => item.useOrGroup && item.orGroup == spawn.orGroup && this.SpawnItemIsValid(murder, item, ref successsfullySpawned, false));
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
						"Murder: ",
						this.murderPreset.name,
						": Spawn ",
						spawn.name,
						" item ",
						spawn.name,
						", tag: ",
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
					this.SpawnItem(murder, spawn.spawnItem, spawn.where, spawn.belongsTo, spawn.writer, spawn.receiver, spawn.security, spawn.ownershipRule, spawn.priority, spawn.itemTag);
					successsfullySpawned.Add(spawn);
				}
				if (spawn.vmailThread != null && spawn.vmailThread.Length > 0)
				{
					Human from = null;
					if (spawn.belongsTo == MurderPreset.LeadCitizen.victim)
					{
						from = murder.victim;
					}
					else if (spawn.belongsTo == MurderPreset.LeadCitizen.killer)
					{
						from = murder.murderer;
					}
					else
					{
						if (spawn.belongsTo == MurderPreset.LeadCitizen.victimsClosest)
						{
							float num2 = -9999f;
							using (List<Acquaintance>.Enumerator enumerator = murder.victim.acquaintances.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									Acquaintance acquaintance = enumerator.Current;
									if (acquaintance.like + acquaintance.known > num2)
									{
										from = acquaintance.with;
										num2 = acquaintance.like + acquaintance.known;
									}
								}
								goto IL_4F0;
							}
						}
						if (spawn.belongsTo == MurderPreset.LeadCitizen.killersClosest)
						{
							float num3 = -9999f;
							using (List<Acquaintance>.Enumerator enumerator = murder.murderer.acquaintances.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									Acquaintance acquaintance2 = enumerator.Current;
									if (acquaintance2.like + acquaintance2.known > num3)
									{
										from = acquaintance2.with;
										num3 = acquaintance2.like + acquaintance2.known;
									}
								}
								goto IL_4F0;
							}
						}
						if (spawn.belongsTo == MurderPreset.LeadCitizen.victimsDoctor)
						{
							from = murder.victim.GetDoctor();
						}
						else if (spawn.belongsTo == MurderPreset.LeadCitizen.killersDoctor)
						{
							from = murder.murderer.GetDoctor();
						}
					}
					IL_4F0:
					Game.Log(string.Concat(new string[]
					{
						"Murder: ",
						this.murderPreset.name,
						": Spawn ",
						spawn.name,
						" vmail thread ",
						spawn.vmailThread,
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
					Toolbox.Instance.NewVmailThread(from, new List<Human>(), spawn.vmailThread, SessionData.Instance.gameTime + Toolbox.Instance.Rand(-12f, -6f, false), Mathf.RoundToInt(Toolbox.Instance.Rand(spawn.vmailProgressThreshold.x, spawn.vmailProgressThreshold.y, false)), StateSaveData.CustomDataSource.sender, -1);
				}
				list.Remove(spawn);
				if (list2 != null)
				{
					foreach (MurderPreset.MurderLeadItem murderLeadItem in list2)
					{
						list.Remove(murderLeadItem);
					}
				}
				num--;
			}
		}
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0007D29C File Offset: 0x0007B49C
	private bool SpawnItemIsValid(MurderController.Murder murder, MurderPreset.MurderLeadItem spawn, ref List<MurderPreset.MurderLeadItem> successsfullySpawned, bool useChance)
	{
		if (useChance)
		{
			float num = spawn.chance;
			bool flag = true;
			if (spawn.useTraits)
			{
				foreach (MurderPreset.MurderModifierRule murderModifierRule in spawn.traitModifiers)
				{
					bool flag2 = false;
					Human human = null;
					if (murderModifierRule.who == MurderPreset.LeadCitizen.killer)
					{
						human = murder.murderer;
					}
					else if (murderModifierRule.who == MurderPreset.LeadCitizen.victim)
					{
						human = murder.victim;
					}
					if (human != null)
					{
						if (murderModifierRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
						{
							using (List<CharacterTrait>.Enumerator enumerator2 = murderModifierRule.traitList.GetEnumerator())
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
								goto IL_258;
							}
						}
						if (murderModifierRule.rule == CharacterTrait.RuleType.ifAllOfThese)
						{
							flag2 = true;
							using (List<CharacterTrait>.Enumerator enumerator2 = murderModifierRule.traitList.GetEnumerator())
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
								goto IL_258;
							}
						}
						if (murderModifierRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
						{
							flag2 = true;
							using (List<CharacterTrait>.Enumerator enumerator2 = murderModifierRule.traitList.GetEnumerator())
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
								goto IL_258;
							}
						}
						if (murderModifierRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
						{
							if (human.partner != null)
							{
								using (List<CharacterTrait>.Enumerator enumerator2 = murderModifierRule.traitList.GetEnumerator())
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
									goto IL_258;
								}
							}
							flag2 = false;
						}
					}
					IL_258:
					if (flag2)
					{
						num += murderModifierRule.chanceModifier;
					}
					else if (murderModifierRule.mustPassForApplication)
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
		return (spawn.compatibleWithAllMotives || spawn.compatibleWithMotives.Contains(this.chosenMO)) && (!spawn.useIf || successsfullySpawned.Exists((MurderPreset.MurderLeadItem item) => item.itemTag == spawn.itemTag));
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0007D620 File Offset: 0x0007B820
	public Interactable SpawnItem(MurderController.Murder murder, InteractablePreset spawnItem, MurderPreset.LeadSpawnWhere spawnWhere, MurderPreset.LeadCitizen spawnBelongsTo, MurderPreset.LeadCitizen spawnWriter, MurderPreset.LeadCitizen spawnReceiver, int security, InteractablePreset.OwnedPlacementRule ownedRule, int priority, JobPreset.JobTag itemTag)
	{
		Human human = null;
		Human human2 = null;
		Human human3 = null;
		if (spawnBelongsTo == MurderPreset.LeadCitizen.victim)
		{
			human = murder.victim;
		}
		else if (spawnBelongsTo == MurderPreset.LeadCitizen.killer)
		{
			human = murder.murderer;
		}
		else
		{
			if (spawnBelongsTo == MurderPreset.LeadCitizen.victimsClosest)
			{
				float num = -9999f;
				using (List<Acquaintance>.Enumerator enumerator = murder.victim.acquaintances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Acquaintance acquaintance = enumerator.Current;
						if (acquaintance.like + acquaintance.known > num)
						{
							human = acquaintance.with;
							num = acquaintance.like + acquaintance.known;
						}
					}
					goto IL_250;
				}
			}
			if (spawnBelongsTo == MurderPreset.LeadCitizen.killersClosest)
			{
				float num2 = -9999f;
				using (List<Acquaintance>.Enumerator enumerator = murder.murderer.acquaintances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Acquaintance acquaintance2 = enumerator.Current;
						if (acquaintance2.like + acquaintance2.known > num2)
						{
							human = acquaintance2.with;
							num2 = acquaintance2.like + acquaintance2.known;
						}
					}
					goto IL_250;
				}
			}
			if (spawnBelongsTo == MurderPreset.LeadCitizen.victimsDoctor)
			{
				human = murder.victim.GetDoctor();
			}
			else if (spawnBelongsTo == MurderPreset.LeadCitizen.killersDoctor)
			{
				human = murder.murderer.GetDoctor();
			}
			else if (spawnBelongsTo == MurderPreset.LeadCitizen.ransom)
			{
				human = murder.victim;
				if (murder.victim.home == null || !murder.victim.home.inhabitants.Exists((Human item) => item != murder.victim && !item.isDead))
				{
					float num3 = -99999f;
					foreach (Acquaintance acquaintance3 in murder.victim.acquaintances)
					{
						Human other = acquaintance3.GetOther(murder.victim);
						if (other != null && other.home != null && !other.isDead && (acquaintance3.like > num3 || human == null))
						{
							num3 = acquaintance3.like;
							human = other;
						}
					}
				}
			}
		}
		IL_250:
		if (spawnWriter == MurderPreset.LeadCitizen.victim)
		{
			human2 = murder.victim;
		}
		else if (spawnWriter == MurderPreset.LeadCitizen.killer)
		{
			human2 = murder.murderer;
		}
		else
		{
			if (spawnWriter == MurderPreset.LeadCitizen.victimsClosest)
			{
				float num4 = -9999f;
				using (List<Acquaintance>.Enumerator enumerator = murder.victim.acquaintances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Acquaintance acquaintance4 = enumerator.Current;
						if (acquaintance4.like + acquaintance4.known > num4)
						{
							human2 = acquaintance4.with;
							num4 = acquaintance4.like + acquaintance4.known;
						}
					}
					goto IL_48D;
				}
			}
			if (spawnWriter == MurderPreset.LeadCitizen.killersClosest)
			{
				float num5 = -9999f;
				using (List<Acquaintance>.Enumerator enumerator = murder.murderer.acquaintances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Acquaintance acquaintance5 = enumerator.Current;
						if (acquaintance5.like + acquaintance5.known > num5)
						{
							human2 = acquaintance5.with;
							num5 = acquaintance5.like + acquaintance5.known;
						}
					}
					goto IL_48D;
				}
			}
			if (spawnWriter == MurderPreset.LeadCitizen.victimsDoctor)
			{
				human2 = murder.victim.GetDoctor();
			}
			else if (spawnWriter == MurderPreset.LeadCitizen.killersDoctor)
			{
				human2 = murder.murderer.GetDoctor();
			}
			else if (spawnWriter == MurderPreset.LeadCitizen.ransom)
			{
				human2 = murder.victim;
				if (murder.victim.home == null || !murder.victim.home.inhabitants.Exists((Human item) => item != murder.victim && !item.isDead))
				{
					float num6 = -99999f;
					foreach (Acquaintance acquaintance6 in murder.victim.acquaintances)
					{
						Human other2 = acquaintance6.GetOther(murder.victim);
						if (other2 != null && other2.home != null && !other2.isDead && (acquaintance6.like > num6 || human2 == null))
						{
							num6 = acquaintance6.like;
							human2 = other2;
						}
					}
				}
			}
		}
		IL_48D:
		if (spawnReceiver == MurderPreset.LeadCitizen.victim)
		{
			human3 = murder.victim;
		}
		else if (spawnReceiver == MurderPreset.LeadCitizen.killer)
		{
			human3 = murder.murderer;
		}
		else
		{
			if (spawnReceiver == MurderPreset.LeadCitizen.victimsClosest)
			{
				float num7 = -9999f;
				using (List<Acquaintance>.Enumerator enumerator = murder.victim.acquaintances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Acquaintance acquaintance7 = enumerator.Current;
						if (acquaintance7.like + acquaintance7.known > num7)
						{
							human3 = acquaintance7.with;
							num7 = acquaintance7.like + acquaintance7.known;
						}
					}
					goto IL_6CA;
				}
			}
			if (spawnReceiver == MurderPreset.LeadCitizen.killersClosest)
			{
				float num8 = -9999f;
				using (List<Acquaintance>.Enumerator enumerator = murder.murderer.acquaintances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Acquaintance acquaintance8 = enumerator.Current;
						if (acquaintance8.like + acquaintance8.known > num8)
						{
							human3 = acquaintance8.with;
							num8 = acquaintance8.like + acquaintance8.known;
						}
					}
					goto IL_6CA;
				}
			}
			if (spawnReceiver == MurderPreset.LeadCitizen.victimsDoctor)
			{
				human3 = murder.victim.GetDoctor();
			}
			else if (spawnReceiver == MurderPreset.LeadCitizen.killersDoctor)
			{
				human3 = murder.murderer.GetDoctor();
			}
			else if (spawnReceiver == MurderPreset.LeadCitizen.ransom)
			{
				human3 = murder.victim;
				if (murder.victim.home == null || !murder.victim.home.inhabitants.Exists((Human item) => item != murder.victim && !item.isDead))
				{
					float num9 = -99999f;
					foreach (Acquaintance acquaintance9 in murder.victim.acquaintances)
					{
						Human other3 = acquaintance9.GetOther(murder.victim);
						if (other3 != null && other3.home != null && !other3.isDead && (acquaintance9.like > num9 || human3 == null))
						{
							num9 = acquaintance9.like;
							human3 = other3;
						}
					}
				}
			}
		}
		IL_6CA:
		Interactable interactable = null;
		List<Interactable.Passed> list = new List<Interactable.Passed>();
		list.Add(new Interactable.Passed(Interactable.PassedVarType.murderID, (float)murder.murderID, null));
		list.Add(new Interactable.Passed(Interactable.PassedVarType.jobTag, (float)itemTag, null));
		if ((spawnWhere == MurderPreset.LeadSpawnWhere.victimHome && murder.victim != null && murder.victim.home != null) || (spawnWhere == MurderPreset.LeadSpawnWhere.victimWork && murder.victim.home != null && murder.victim.job.employer == null))
		{
			if (interactable == null)
			{
				FurnitureLocation furnitureLocation;
				interactable = murder.victim.home.PlaceObject(spawnItem, human, human2, human3, out furnitureLocation, list, true, security, ownedRule, priority, null, false, null, null, null, "", false);
			}
		}
		else if (spawnWhere == MurderPreset.LeadSpawnWhere.victimWork && murder.victim != null && murder.victim.job.employer != null)
		{
			if (interactable == null)
			{
				FurnitureLocation furnitureLocation;
				interactable = murder.victim.job.employer.address.PlaceObject(spawnItem, human, human2, human3, out furnitureLocation, list, true, security, ownedRule, priority, null, false, null, null, null, "", false);
			}
		}
		else if ((spawnWhere == MurderPreset.LeadSpawnWhere.killerHome && murder.murderer != null && murder.murderer.home != null) || (spawnWhere == MurderPreset.LeadSpawnWhere.killerWork && murder.murderer.home != null && murder.murderer.job.employer == null))
		{
			if (interactable == null)
			{
				FurnitureLocation furnitureLocation;
				interactable = murder.murderer.home.PlaceObject(spawnItem, human, human2, human3, out furnitureLocation, list, true, security, ownedRule, priority, null, false, null, null, null, "", false);
			}
		}
		else if (spawnWhere == MurderPreset.LeadSpawnWhere.killerWork && murder.murderer != null && murder.murderer.job.employer != null)
		{
			if (interactable == null)
			{
				FurnitureLocation furnitureLocation;
				interactable = murder.murderer.job.employer.address.PlaceObject(spawnItem, human, human2, human3, out furnitureLocation, list, true, security, ownedRule, priority, null, false, null, null, null, "", false);
			}
		}
		else if (spawnWhere == MurderPreset.LeadSpawnWhere.ransom && murder.victim != null)
		{
			NewGameLocation home = murder.victim.home;
			if (murder.victim.home != null && murder.victim.home.inhabitants.Exists((Human item) => item != murder.victim && !item.isDead))
			{
				home = murder.victim.home;
			}
			if (home == null)
			{
				float num10 = -99999f;
				foreach (Acquaintance acquaintance10 in murder.victim.acquaintances)
				{
					Human other4 = acquaintance10.GetOther(murder.victim);
					if (other4 != null && other4.home != null && !other4.isDead && (acquaintance10.like > num10 || home == null))
					{
						num10 = acquaintance10.like;
						home = other4.home;
					}
				}
			}
			if (interactable == null)
			{
				FurnitureLocation furnitureLocation;
				interactable = home.PlaceObject(spawnItem, human, human2, human3, out furnitureLocation, list, true, security, ownedRule, priority, null, false, null, null, null, "", false);
			}
		}
		else if ((spawnWhere == MurderPreset.LeadSpawnWhere.killerDen && murder.murderer != null && murder.murderer.den != null) || (spawnWhere == MurderPreset.LeadSpawnWhere.killerDen && murder.murderer.home != null && murder.murderer.den == null))
		{
			if (interactable == null && murder.murderer != null && murder.murderer.den != null)
			{
				FurnitureLocation furnitureLocation;
				interactable = murder.murderer.den.PlaceObject(spawnItem, human, human2, human3, out furnitureLocation, list, true, security, ownedRule, priority, null, false, null, null, null, "", false);
			}
			else if (interactable == null && murder.murderer != null && murder.murderer.den == null && murder.murderer.home != null)
			{
				FurnitureLocation furnitureLocation;
				interactable = murder.murderer.home.PlaceObject(spawnItem, human, human2, human3, out furnitureLocation, list, true, security, ownedRule, priority, null, false, null, null, null, "", false);
			}
		}
		if (interactable != null)
		{
			string[] array = new string[7];
			array[0] = "Murder: Successfully spawned item ";
			array[1] = interactable.name;
			array[2] = " for at ";
			int num11 = 3;
			Vector3 wPos = interactable.wPos;
			array[num11] = wPos.ToString();
			array[4] = " (";
			array[5] = interactable.node.room.name;
			array[6] = ")";
			Game.Log(string.Concat(array), 2);
			if (murder.activeMurderItems == null)
			{
				murder.activeMurderItems = new Dictionary<JobPreset.JobTag, Interactable>();
			}
			if (!murder.activeMurderItems.ContainsKey(itemTag))
			{
				murder.activeMurderItems.Add(itemTag, interactable);
			}
		}
		else
		{
			Game.Log("Murder: Unable to spawn item " + spawnItem.name, 2);
		}
		return interactable;
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0007E344 File Offset: 0x0007C544
	[Button(null, 0)]
	public void LastMurderLocation()
	{
		Game.Log(Strings.ComposeText("Last murder location: |killer.lastmurder.victim.name| by |killer.lastmurder.killer.name| at |killer.lastmurder.location.name|", null, Strings.LinkSetting.automatic, null, null, false), 2);
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0007E35C File Offset: 0x0007C55C
	[Button(null, 0)]
	public void ListSpawnedMurderItems()
	{
		MurderController.Murder murder = this.activeMurders.Find((MurderController.Murder item) => item.murderID == this.debugMurderID);
		if (murder == null)
		{
			murder = this.inactiveMurders.Find((MurderController.Murder item) => item.murderID == this.debugMurderID);
		}
		if (murder != null && murder.activeMurderItems != null)
		{
			foreach (KeyValuePair<JobPreset.JobTag, Interactable> keyValuePair in murder.activeMurderItems)
			{
				if (keyValuePair.Value != null)
				{
					Game.Log(string.Concat(new string[]
					{
						"Spawned murder clue: ",
						keyValuePair.Key.ToString(),
						": ",
						keyValuePair.Value.GetName(),
						" at ",
						keyValuePair.Value.GetWorldPosition(true).ToString(),
						" (",
						keyValuePair.Value.node.room.GetName(),
						")"
					}), 2);
				}
			}
		}
	}

	// Token: 0x04000810 RID: 2064
	[Header("State")]
	public bool procGenLoopActive;

	// Token: 0x04000811 RID: 2065
	public bool murderRoutineActive;

	// Token: 0x04000812 RID: 2066
	public Human currentMurderer;

	// Token: 0x04000813 RID: 2067
	public Human currentVictim;

	// Token: 0x04000814 RID: 2068
	[NonSerialized]
	public Case currentActiveCase;

	// Token: 0x04000815 RID: 2069
	public MurderPreset murderPreset;

	// Token: 0x04000816 RID: 2070
	public MurderMO chosenMO;

	// Token: 0x04000817 RID: 2071
	public List<Human> previousMurderers = new List<Human>();

	// Token: 0x04000818 RID: 2072
	public float pauseBetweenMurders;

	// Token: 0x04000819 RID: 2073
	public float pauseBeforeKidnapperKill;

	// Token: 0x0400081A RID: 2074
	private float locationUpdateTimer;

	// Token: 0x0400081B RID: 2075
	public int maxDifficultyLevel = 1;

	// Token: 0x0400081C RID: 2076
	public NewGameLocation currentVictimSite;

	// Token: 0x0400081D RID: 2077
	public bool triggerCoverUpCall;

	// Token: 0x0400081E RID: 2078
	private TelephoneController.PhoneCall coverUpCall;

	// Token: 0x0400081F RID: 2079
	public bool playerAcceptedCoverUp;

	// Token: 0x04000820 RID: 2080
	public bool triggerCoverUpSuccess;

	// Token: 0x04000821 RID: 2081
	private TelephoneController.PhoneCall successCall;

	// Token: 0x04000822 RID: 2082
	private bool triggeredSeenWarning;

	// Token: 0x04000823 RID: 2083
	private List<NewGameLocation> sniperVictimSites = new List<NewGameLocation>();

	// Token: 0x04000824 RID: 2084
	[Header("DDS")]
	public List<MurderController.MurderMethod> methodTypes = new List<MurderController.MurderMethod>();

	// Token: 0x04000825 RID: 2085
	[Header("Murder")]
	public int assignMurderID = 1;

	// Token: 0x04000826 RID: 2086
	public List<MurderController.Murder> activeMurders = new List<MurderController.Murder>();

	// Token: 0x04000827 RID: 2087
	public List<MurderController.Murder> inactiveMurders = new List<MurderController.Murder>();

	// Token: 0x04000828 RID: 2088
	public float sniperShotDelay;

	// Token: 0x04000829 RID: 2089
	public float limbTargetCycleCounter;

	// Token: 0x0400082A RID: 2090
	public int limbTargetCycle;

	// Token: 0x0400082B RID: 2091
	[Header("References")]
	public AIGoalPreset murderGoalPreset;

	// Token: 0x0400082C RID: 2092
	[Header("Debug")]
	public List<MurderController.MurderPick> debugLastMurderPicks = new List<MurderController.MurderPick>();

	// Token: 0x0400082D RID: 2093
	public int debugMurderID;

	// Token: 0x0400082E RID: 2094
	private static MurderController _instance;

	// Token: 0x02000110 RID: 272
	[Serializable]
	public class MurderMethod
	{
		// Token: 0x0400082F RID: 2095
		public MurderWeaponPreset.WeaponType type;

		// Token: 0x04000830 RID: 2096
		public string blockDDS;
	}

	// Token: 0x02000111 RID: 273
	public enum MurderState
	{
		// Token: 0x04000832 RID: 2098
		none,
		// Token: 0x04000833 RID: 2099
		acquireEuipment,
		// Token: 0x04000834 RID: 2100
		research,
		// Token: 0x04000835 RID: 2101
		waitForLocation,
		// Token: 0x04000836 RID: 2102
		travellingTo,
		// Token: 0x04000837 RID: 2103
		executing,
		// Token: 0x04000838 RID: 2104
		post,
		// Token: 0x04000839 RID: 2105
		escaping,
		// Token: 0x0400083A RID: 2106
		unsolved,
		// Token: 0x0400083B RID: 2107
		solved
	}

	// Token: 0x02000112 RID: 274
	public enum KidnapRansomPhase
	{
		// Token: 0x0400083D RID: 2109
		none,
		// Token: 0x0400083E RID: 2110
		travellingToRansom,
		// Token: 0x0400083F RID: 2111
		collectedRansom,
		// Token: 0x04000840 RID: 2112
		freeingVictim,
		// Token: 0x04000841 RID: 2113
		finishedFailed,
		// Token: 0x04000842 RID: 2114
		finishedSuccess
	}

	// Token: 0x02000113 RID: 275
	[Serializable]
	public class MurderPick
	{
		// Token: 0x04000843 RID: 2115
		public Human person;

		// Token: 0x04000844 RID: 2116
		public MurderMO mo;

		// Token: 0x04000845 RID: 2117
		public float score;
	}

	// Token: 0x02000114 RID: 276
	[Serializable]
	public class Murder
	{
		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000802 RID: 2050 RVA: 0x0007E534 File Offset: 0x0007C734
		// (remove) Token: 0x06000803 RID: 2051 RVA: 0x0007E56C File Offset: 0x0007C76C
		public event MurderController.Murder.OnMurderStateChange OnStateChanged;

		// Token: 0x06000804 RID: 2052 RVA: 0x0007E5A4 File Offset: 0x0007C7A4
		public Murder(Human newMurderer, Human newVictim, MurderPreset newPreset, MurderMO newMotive, NewGameLocation newVictimSite = null)
		{
			this.murderID = MurderController.Instance.assignMurderID;
			MurderController.Instance.assignMurderID++;
			this.murderer = newMurderer;
			this.murdererID = this.murderer.humanID;
			this.victim = newVictim;
			this.victimID = this.victim.humanID;
			this.preset = newPreset;
			this.presetStr = this.preset.name;
			this.mo = newMotive;
			this.moStr = newMotive.name;
			this.creationTime = SessionData.Instance.gameTime;
			this.sniperVictimSite = newVictimSite;
			if (this.sniperVictimSite != null)
			{
				if (this.sniperVictimSite.thisAsAddress != null)
				{
					this.victimSiteID = this.sniperVictimSite.thisAsAddress.id;
					this.victimSiteIsStreet = false;
				}
				else if (this.sniperVictimSite.thisAsStreet != null)
				{
					this.victimSiteID = this.sniperVictimSite.thisAsStreet.streetID;
					this.victimSiteIsStreet = true;
				}
			}
			this.PickNewMurderWeapon();
			this.PickNewCallingCard();
			MurderController.Instance.activeMurders.Add(this);
			Game.Log("Murder: Creating new murder of " + this.victim.GetCitizenName() + " by " + this.murderer.GetCitizenName(), 2);
			this.EuipmentCheck();
			this.SetMurderState(MurderController.MurderState.acquireEuipment, false);
			if (this.preset.caseType == MurderPreset.CaseType.kidnap)
			{
				this.SetRansomPhase(MurderController.KidnapRansomPhase.none);
				this.GenerateRansomDetails();
			}
			this.murderer.ai.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, true);
			this.murderer.ai.disableTickRateUpdate = true;
			this.victim.ai.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, true);
			this.victim.ai.disableTickRateUpdate = true;
			this.CreateMurderGoal();
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x0007E804 File Offset: 0x0007CA04
		public void LoadSerializedData()
		{
			if (this.presetStr != null && this.presetStr.Length > 0)
			{
				Toolbox.Instance.LoadDataFromResources<MurderPreset>(this.presetStr, out this.preset);
			}
			if (this.moStr != null && this.moStr.Length > 0)
			{
				Toolbox.Instance.LoadDataFromResources<MurderMO>(this.moStr, out this.mo);
			}
			if (this.weaponStr != null && this.weaponStr.Length > 0)
			{
				Toolbox.Instance.LoadDataFromResources<InteractablePreset>(this.weaponStr, out this.weaponPreset);
			}
			if (this.callingCardStr != null && this.ammoStr.Length > 0)
			{
				Toolbox.Instance.LoadDataFromResources<InteractablePreset>(this.ammoStr, out this.ammoPreset);
			}
			if (this.callingCardStr != null && this.callingCardStr.Length > 0)
			{
				Toolbox.Instance.LoadDataFromResources<InteractablePreset>(this.callingCardStr, out this.callingCardPreset);
			}
			CityData.Instance.GetHuman(this.murdererID, out this.murderer, true);
			CityData.Instance.GetHuman(this.victimID, out this.victim, true);
			if (Game.Instance.debugMurdererOnStart && Game.Instance.collectDebugData && !Game.Instance.debugHuman.Contains(this.murderer))
			{
				this.murderer.ai.ToggleHumanDebug();
			}
			if (this.streetID > -1)
			{
				this.location = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == this.streetID);
			}
			else if (this.addressID > -1)
			{
				NewAddress newAddress = null;
				CityData.Instance.addressDictionary.TryGetValue(this.addressID, ref newAddress);
				this.location = newAddress;
			}
			if (this.weaponID > -1)
			{
				CityData.Instance.savableInteractableDictionary.TryGetValue(this.weaponID, ref this.weapon);
			}
			if (this.ammoID > -1)
			{
				CityData.Instance.savableInteractableDictionary.TryGetValue(this.ammoID, ref this.ammo);
			}
			if (this.callingCardID > -1)
			{
				CityData.Instance.savableInteractableDictionary.TryGetValue(this.callingCardID, ref this.callingCard);
			}
			if (this.weaponSourceID > -1)
			{
				this.weaponSource = CityData.Instance.companyDirectory.Find((Company item) => item.companyID == this.weaponSourceID);
			}
			if (this.victimSiteID > -1)
			{
				if (this.victimSiteIsStreet)
				{
					this.sniperVictimSite = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == this.victimSiteID);
				}
				else
				{
					NewAddress newAddress2 = null;
					if (CityData.Instance.addressDictionary.TryGetValue(this.victimSiteID, ref newAddress2))
					{
						this.sniperVictimSite = newAddress2;
					}
				}
			}
			if (this.ransomSiteID > -1)
			{
				this.ransomSite = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Find((NewBuilding item) => item.buildingID == this.ransomSiteID);
			}
			this.graffiti = new List<Interactable>();
			foreach (int num in this.graffitiIDs)
			{
				Interactable interactable = null;
				if (CityData.Instance.savableInteractableDictionary.TryGetValue(num, ref interactable))
				{
					this.graffiti.Add(interactable);
				}
			}
			if (this.location != null && this.location.thisAsAddress != null)
			{
				this.murderer.AddToKeyring(this.location.thisAsAddress, false);
			}
			if (this.murderGoal == null && this.state != MurderController.MurderState.solved && this.state != MurderController.MurderState.unsolved)
			{
				this.murderGoal = this.murderer.ai.goals.Find((NewAIGoal item) => item.preset == MurderController.Instance.murderGoalPreset);
				if (this.murderGoal == null)
				{
					this.CreateMurderGoal();
				}
			}
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0007EBDC File Offset: 0x0007CDDC
		public void CreateMurderGoal()
		{
			if (this.murderGoal == null)
			{
				string text = "Murder: Creating new murder goal with victim passed interactable: ";
				Interactable interactable = this.victim.interactable;
				Game.Log(text + ((interactable != null) ? interactable.ToString() : null), 2);
				this.murderGoal = this.murderer.ai.CreateNewGoal(MurderController.Instance.murderGoalPreset, SessionData.Instance.gameTime, 0f, null, this.victim.interactable, null, null, this, -2);
				this.murderer.ai.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, true);
				this.murderer.ai.disableTickRateUpdate = true;
				this.victim.ai.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, true);
				this.victim.ai.disableTickRateUpdate = true;
			}
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0007ECA4 File Offset: 0x0007CEA4
		public void SetMurderState(MurderController.MurderState newState, bool force = false)
		{
			if (this.state != newState || force)
			{
				this.state = newState;
				MurderController.Instance.SpawnItemsCheck(this);
				Game.Log("Murder: Set murder state: " + this.state.ToString(), 2);
				if (this.state == MurderController.MurderState.waitForLocation)
				{
					this.waitingTimestamp = SessionData.Instance.gameTime;
				}
				else if (this.state == MurderController.MurderState.travellingTo)
				{
					if (this.callingCardPreset != null && this.callingCard == null && this.callingCardOrigin == MurderMO.CallingCardOrigin.createOnGoToLocation)
					{
						string text = "Murder: Creating new calling card: ";
						InteractablePreset interactablePreset = this.callingCardPreset;
						Game.Log(text + ((interactablePreset != null) ? interactablePreset.ToString() : null), 2);
						this.callingCard = InteractableCreator.Instance.CreateWorldInteractable(this.callingCardPreset, this.murderer, this.murderer, this.victim, this.murderer.transform.position, Vector3.zero, null, null, "");
						if (this.callingCard != null)
						{
							this.callingCard.SetInInventory(this.murderer);
							this.callingCardID = this.callingCard.id;
						}
					}
					this.murderGoal.gameLocation = this.location;
					this.victim.ai.SetAsVictim(this);
					MurderController.Instance.sniperShotDelay = 8.5f;
					MurderController.Instance.limbTargetCycle = 0;
					this.murderer.ai.AITick(true, false);
					if (this.preset != null && this.preset.caseType == MurderPreset.CaseType.sniper)
					{
						this.murderer.ai.SetAsMurderer(this);
					}
				}
				else if (this.state == MurderController.MurderState.executing)
				{
					foreach (NewRoom newRoom in this.location.rooms)
					{
						if (!this.cullingActiveRooms.Contains(newRoom.roomID))
						{
							this.cullingActiveRooms.Add(newRoom.roomID);
						}
					}
					Player.Instance.UpdateCulling();
					this.murderer.ai.ResetInvestigate();
					this.victim.ai.SetAsVictim(this);
					this.murderer.ai.SetAsMurderer(this);
				}
				else if (this.state == MurderController.MurderState.post)
				{
					this.victim.ai.SetAsVictim(this);
					this.murderer.ai.SetAsMurderer(this);
					this.PlaceCallingCard();
					this.GenerateGraffiti();
					this.WeaponDisposal();
					for (int i = 0; i < this.murderGoal.actions.Count; i++)
					{
						Game.Log("Murder: Removing " + this.murderGoal.actions[i].preset.name, 2);
						this.murderGoal.actions[i].Remove(0f);
						i--;
					}
					this.murderer.ai.ResetInvestigate();
				}
				else if (this.state == MurderController.MurderState.escaping)
				{
					if (this.preset.caseType == MurderPreset.CaseType.kidnap)
					{
						MurderController.Instance.TriggerKidnappingCase();
					}
					this.cullingActiveRooms.Clear();
				}
				else if (this.state == MurderController.MurderState.solved)
				{
					MurderController.Instance.OnCaseSolved();
					if (MurderController.Instance.activeMurders.Contains(this))
					{
						MurderController.Instance.activeMurders.Remove(this);
					}
					if (!MurderController.Instance.inactiveMurders.Contains(this))
					{
						MurderController.Instance.inactiveMurders.Add(this);
					}
				}
				if (this.murderGoal != null)
				{
					if (this.murderGoal.isActive)
					{
						this.murderGoal.RefreshActions(true);
						this.murderGoal.AITick();
					}
					else
					{
						this.murderer.ai.AITick(true, true);
					}
					this.murderer.ai.UpdateCurrentWeapon();
				}
				MurderController.Instance.SetUpdateEnabled(true);
				if (this.OnStateChanged != null)
				{
					this.OnStateChanged(this.state);
				}
			}
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0007F0CC File Offset: 0x0007D2CC
		public void CancelCurrentMurder()
		{
			Game.Log("Murder: Cancelling current murder...", 2);
			if (this.murderGoal != null)
			{
				this.murderGoal.OnDeactivate(0f);
				this.murderGoal.murderRef = null;
				this.murderGoal.Remove();
			}
			this.murderer.ai.disableTickRateUpdate = false;
			this.victim.ai.disableTickRateUpdate = false;
			this.SetMurderState(MurderController.MurderState.unsolved, false);
			this.cullingActiveRooms.Clear();
			for (int i = 0; i < this.victim.ai.victimsForMurders.Count; i++)
			{
				if (this.victim.ai.victimsForMurders[i] == this)
				{
					this.victim.ai.victimsForMurders.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < this.murderer.ai.killerForMurders.Count; j++)
			{
				if (this.murderer.ai.killerForMurders[j] == this)
				{
					this.murderer.ai.killerForMurders.RemoveAt(j);
					j--;
				}
			}
			this.murderer.ai.AITick(true, false);
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0007F200 File Offset: 0x0007D400
		public bool IsValidLocation(NewGameLocation newLoc)
		{
			if (this.preset.caseType == MurderPreset.CaseType.sniper && this.sniperVictimSite != null)
			{
				return this.sniperVictimSite == newLoc;
			}
			if ((this.mo.allowHome || this.mo.allowAnywhere) && this.victim.home == newLoc)
			{
				return true;
			}
			bool flag = false;
			if (this.mo.allowAnywhere)
			{
				flag = true;
			}
			else if (this.mo.allowWork && this.victim.job != null && this.victim.job.employer != null && this.victim.job.employer.placeOfBusiness == newLoc)
			{
				flag = true;
			}
			else if (this.mo.allowPublic && newLoc.thisAsAddress != null && newLoc.thisAsAddress.preset.publicFacing)
			{
				flag = true;
			}
			else if (this.mo.allowStreets && newLoc.thisAsStreet != null)
			{
				flag = true;
			}
			else if (this.mo.allowDen && newLoc == this.murderer.den)
			{
				flag = true;
			}
			return (!flag || newLoc.currentOccupants.Count <= this.preset.nonHomeMaximumOccupantsTrigger) && flag;
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0007F360 File Offset: 0x0007D560
		public void PickNewMurderWeapon()
		{
			Game.Log("Murder: Picking new murder weapon...", 2);
			float num = -99999f;
			this.weaponPreset = null;
			this.ammoPreset = null;
			foreach (MurderWeaponsPool murderWeaponsPool in this.mo.weaponsPool)
			{
				using (List<MurderWeaponsPool.MurderWeaponPick>.Enumerator enumerator2 = murderWeaponsPool.murderWeaponPool.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						MurderWeaponsPool.MurderWeaponPick wp = enumerator2.Current;
						float num2 = Toolbox.Instance.GetPsuedoRandomNumber(wp.randomScoreRange.x, wp.randomScoreRange.y, CityData.Instance.seed + this.murdererID.ToString() + this.murderer.citizenName + wp.weapon.name, false);
						float num3 = 0f;
						if (wp.traitModifiers.Count > 0)
						{
							if (!MurderController.Instance.TraitTest(this.murderer as Citizen, ref wp.traitModifiers, out num3))
							{
								continue;
							}
							num2 += num3;
						}
						if (this.mo.baseDifficulty + wp.weapon.weapon.murderDifficultyModifier <= MurderController.Instance.maxDifficultyLevel)
						{
							num2 += 50f;
						}
						if (this.murderer.inventory.Exists((Interactable item) => item.preset == wp.weapon))
						{
							num2 += 100f;
						}
						else if (this.murderer.ai.putDownItems.Exists((Interactable item) => item.preset == wp.weapon))
						{
							num2 += 50f;
						}
						if (num2 > num)
						{
							bool flag = false;
							foreach (Company company in CityData.Instance.companyDirectory)
							{
								if (company != null && company.prices.ContainsKey(wp.weapon))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								this.weaponPreset = wp.weapon;
								num = num2;
								this.dropChance = wp.chanceOfDroppingAtScene;
							}
						}
					}
				}
			}
			if (this.weaponPreset != null)
			{
				Game.Log("Murder: Chosen murder weapon " + this.weaponPreset.weapon.name + ", choosing ammo...", 2);
				this.weaponStr = this.weaponPreset.name;
				if (this.weaponPreset.weapon.ammunition.Count > 0)
				{
					this.ammoPreset = this.weaponPreset.weapon.ammunition[Toolbox.Instance.GetPsuedoRandomNumber(0, this.weaponPreset.weapon.ammunition.Count, CityData.Instance.seed + this.murdererID.ToString() + this.murderer.citizenName + this.weaponPreset.name, false)];
					this.ammoStr = this.ammoPreset.name;
					Game.Log("Murder: Chosen ammo is " + this.ammoPreset.name + "...", 2);
					return;
				}
			}
			else
			{
				Game.LogError("Unable to choose weapon for murder!", 2);
			}
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0007F720 File Offset: 0x0007D920
		public void PickNewCallingCard()
		{
			Game.Log("Murder: Picking new calling card...", 2);
			float num = -99999f;
			this.callingCardPreset = null;
			foreach (MurderMO.CallingCardPick callingCardPick in this.mo.callingCardPool)
			{
				float num2 = Toolbox.Instance.Rand(callingCardPick.randomScoreRange.x, callingCardPick.randomScoreRange.y, false);
				float num3 = 0f;
				if (callingCardPick.traitModifiers.Count > 0)
				{
					if (!MurderController.Instance.TraitTest(this.murderer as Citizen, ref callingCardPick.traitModifiers, out num3))
					{
						continue;
					}
					num2 += num3;
				}
				if (num2 > num)
				{
					this.callingCardPreset = callingCardPick.item;
					this.callingCardStr = this.callingCardPreset.name;
					this.callingCardOrigin = callingCardPick.origin;
					num = num2;
				}
			}
			if (this.callingCardPreset != null)
			{
				Game.Log("Murder: Chosen calling card " + this.callingCardPreset.name + "...", 2);
			}
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0007F848 File Offset: 0x0007DA48
		public void SetMurderWeaponActual(Interactable newObj)
		{
			this.weapon = newObj;
			if (newObj != null)
			{
				this.weaponID = newObj.id;
			}
			this.GenerateMoniker();
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0007F868 File Offset: 0x0007DA68
		public void SetMurderLocation(NewGameLocation newLoc)
		{
			this.location = newLoc;
			if (this.location != null)
			{
				Game.Log("Murder: Set new murder location: " + newLoc.name, 2);
				if (this.location.thisAsAddress != null)
				{
					this.addressID = this.location.thisAsAddress.id;
					this.murderer.AddToKeyring(this.location.thisAsAddress, false);
				}
				else if (this.location.thisAsStreet != null)
				{
					this.streetID = this.location.thisAsStreet.streetID;
				}
				if (this.murderGoal != null)
				{
					this.murderGoal.gameLocation = newLoc;
				}
			}
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x0007F924 File Offset: 0x0007DB24
		public void EuipmentCheck()
		{
			if (!this.preset.requiresAcquirePhase)
			{
				return;
			}
			bool flag = this.murderer.inventory.Exists((Interactable item) => item.preset == this.weaponPreset);
			bool flag2 = this.ammoPreset == null || this.murderer.inventory.Exists((Interactable item) => item.preset == this.ammoPreset);
			if (flag && flag2)
			{
				this.acquiredEquipment = true;
			}
			else
			{
				this.acquiredEquipment = false;
			}
			if (this.acquiredEquipment)
			{
				if (this.state == MurderController.MurderState.acquireEuipment)
				{
					this.SetMurderState(MurderController.MurderState.research, false);
					return;
				}
			}
			else if (this.murderGoal != null && this.murderGoal.isActive && this.state != MurderController.MurderState.acquireEuipment)
			{
				this.SetMurderState(MurderController.MurderState.acquireEuipment, false);
			}
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0007F9E0 File Offset: 0x0007DBE0
		public string GetMonkier()
		{
			if (Strings.loadedLanguage != null && Strings.loadedLanguage.staticKillerMoniker && this.weapon != null)
			{
				return Strings.Get("misc", "static_killer_ref_" + Toolbox.Instance.GetPsuedoRandomNumber(1, 10, this.murderer.citizenName, false).ToString(), Strings.Casing.asIs, false, false, false, null);
			}
			if (this.monkierPre != null && this.monkierPre.Length > 0 && this.monkierPost != null && this.monkierPost.Length > 0)
			{
				return this.monkierPre + " " + this.monkierPost;
			}
			return Strings.Get("misc", "static_killer_ref_1", Strings.Casing.asIs, false, false, false, null);
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0007FA9C File Offset: 0x0007DC9C
		public void GenerateMoniker()
		{
			if (this.weapon != null)
			{
				DDSSaveClasses.DDSMessageSave ddsmessageSave = null;
				if (this.mo.monkierDDSMessageList != null && this.mo.monkierDDSMessageList.Length > 0 && Toolbox.Instance.allDDSMessages.TryGetValue(this.mo.monkierDDSMessageList, ref ddsmessageSave))
				{
					this.monkierPre = Strings.Get("dds.blocks", ddsmessageSave.blocks[Toolbox.Instance.Rand(0, ddsmessageSave.blocks.Count, false)].blockID, Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					this.monkierPre = CityData.Instance.cityName;
				}
				List<string> list = new List<string>();
				DDSSaveClasses.DDSMessageSave ddsmessageSave2 = null;
				if (Toolbox.Instance.allDDSMessages.TryGetValue("84c57fc6-4a6f-4ea8-b84b-b268ea4044ee", ref ddsmessageSave2))
				{
					foreach (DDSSaveClasses.DDSBlockCondition ddsblockCondition in ddsmessageSave2.blocks)
					{
						list.Add(ddsblockCondition.blockID);
					}
				}
				DDSSaveClasses.DDSMessageSave ddsmessageSave3 = null;
				string text = "84c57fc6-4a6f-4ea8-b84b-b268ea4044ee";
				if (this.weapon.preset.weapon.type == MurderWeaponPreset.WeaponType.blade)
				{
					text = "17669eb6-227e-4009-bded-2f2978dca87b";
				}
				else if (this.weapon.preset.weapon.type == MurderWeaponPreset.WeaponType.poison)
				{
					text = "5eeaeae7-4388-4879-907a-6a10ef911238";
				}
				else if (this.weapon.preset.weapon.type == MurderWeaponPreset.WeaponType.bluntObject)
				{
					text = "901e0290-7c47-4c40-b5e6-7957a4b8f97a";
				}
				else if (this.weapon.preset.weapon.type == MurderWeaponPreset.WeaponType.handgun)
				{
					text = "869d9e21-1385-4b04-a796-65e9c02724ce";
				}
				else if (this.weapon.preset.weapon.type == MurderWeaponPreset.WeaponType.rifle)
				{
					text = "2eb67819-2717-44ed-aa16-0abdd35ef77f";
				}
				else if (this.weapon.preset.weapon.type == MurderWeaponPreset.WeaponType.shotgun)
				{
					text = "b6cf2693-b187-40b9-b11f-0b1734cd0564";
				}
				else if (this.weapon.preset.weapon.type == MurderWeaponPreset.WeaponType.strangulation)
				{
					text = "999ffe45-9a78-43f3-8b8b-f9a049521ca0";
				}
				if (Toolbox.Instance.allDDSMessages.TryGetValue(text, ref ddsmessageSave3))
				{
					using (List<DDSSaveClasses.DDSBlockCondition>.Enumerator enumerator = ddsmessageSave3.blocks.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							DDSSaveClasses.DDSBlockCondition cond = enumerator.Current;
							if (!MurderController.Instance.inactiveMurders.Exists((MurderController.Murder item) => item.monkierPost == cond.blockID))
							{
								for (int i = 0; i < 2; i++)
								{
									list.Add(cond.blockID);
								}
							}
						}
					}
				}
				List<string> list2 = new List<string>();
				foreach (string key in list)
				{
					string text2 = Strings.Get("dds.blocks", key, Strings.Casing.asIs, false, false, false, null);
					if (text2.Length > 0 && this.monkierPre.Length > 0 && text2.Substring(0, 1).ToLower() == this.monkierPre.Substring(0, 1).ToLower())
					{
						list2.Add(text2);
					}
				}
				if (list2.Count > 0)
				{
					this.monkierPost = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
				}
				else if (list.Count > 0)
				{
					string key2 = list[Toolbox.Instance.Rand(0, list.Count, false)];
					this.monkierPost = Strings.Get("dds.blocks", key2, Strings.Casing.asIs, false, false, false, null);
				}
				Game.Log("Murder: Assigned monkier: " + this.GetMonkier(), 2);
			}
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0007FE5C File Offset: 0x0007E05C
		public void PlaceCallingCard()
		{
			if (this.callingCardPreset != null)
			{
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				if (!(this.victim != null))
				{
					return;
				}
				Transform bodyAnchor = this.victim.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.upperTorso);
				if (!(bodyAnchor != null))
				{
					return;
				}
				vector = bodyAnchor.position + new Vector3(0f, 1f, 0f);
				vector2 = bodyAnchor.eulerAngles;
				if (this.callingCard != null && this.callingCardOrigin != MurderMO.CallingCardOrigin.createAtScene)
				{
					string text = "Murder: Dropping existing calling card at ";
					Vector3 vector3 = vector;
					string text2 = vector3.ToString();
					string text3 = ", euler ";
					vector3 = vector2;
					Game.Log(text + text2 + text3 + vector3.ToString(), 2);
					this.callingCard.SetAsNotInventory(this.victim.currentNode);
					this.callingCard.MoveInteractable(vector, vector2, true);
					this.callingCard.ForcePhysicsActive(true, true, new Vector3(Toolbox.Instance.Rand(0.5f, 2f, false), 0f, Toolbox.Instance.Rand(0.5f, 2f, false)), 2, false);
					return;
				}
				if (this.callingCardOrigin == MurderMO.CallingCardOrigin.createAtScene)
				{
					string text4 = "Murder: Creating new calling card at ";
					Vector3 vector3 = vector;
					string text5 = vector3.ToString();
					string text6 = ", euler ";
					vector3 = vector2;
					Game.Log(text4 + text5 + text6 + vector3.ToString(), 2);
					List<Interactable.Passed> list = new List<Interactable.Passed>();
					list.Add(new Interactable.Passed(Interactable.PassedVarType.murderID, (float)this.murderID, null));
					this.callingCard = InteractableCreator.Instance.CreateWorldInteractable(this.callingCardPreset, this.murderer, this.murderer, this.victim, vector, vector2, list, null, "");
					if (this.callingCard != null)
					{
						this.callingCard.ForcePhysicsActive(true, true, new Vector3(Toolbox.Instance.Rand(0.5f, 2f, false), 0f, Toolbox.Instance.Rand(0.5f, 2f, false)), 2, false);
						this.callingCardID = this.callingCard.id;
					}
				}
			}
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x00080074 File Offset: 0x0007E274
		public void WeaponDisposal()
		{
			if (this.dropChance > 0f && this.weapon != null && !this.mo.blockDroppingWeapons && Toolbox.Instance.Rand(0f, 1f, false) <= this.dropChance)
			{
				Game.Log("Murder: Dropping weapon at scene...", 2);
				Vector3 position = this.murderer.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandRight).position;
				Vector3 eulerAngles = this.murderer.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandRight).eulerAngles;
				this.weapon.SetAsNotInventory(this.murderer.currentNode);
				this.weapon.MoveInteractable(position, eulerAngles, true);
				this.weapon.ForcePhysicsActive(true, true, new Vector3(Toolbox.Instance.Rand(0.5f, 2f, false), 0f, Toolbox.Instance.Rand(0.5f, 2f, false)), 2, false);
			}
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0008016C File Offset: 0x0007E36C
		public void GenerateGraffiti()
		{
			if (this.graffiti.Count <= 0)
			{
				foreach (MurderMO.Graffiti graffiti in this.mo.graffiti)
				{
					Vector3 vector = Vector3.zero;
					Vector3 vector2 = Vector3.zero;
					if (graffiti.pos == MurderMO.Graffiti.GraffitiPosition.victim)
					{
						if (this.victim != null)
						{
							Transform bodyAnchor = this.victim.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.upperTorso);
							if (bodyAnchor != null)
							{
								vector = bodyAnchor.position;
								vector.y = this.victim.currentNode.position.y + 0.001f;
								vector2..ctor(0f, bodyAnchor.eulerAngles.y, 0f);
								goto IL_29E;
							}
						}
						break;
					}
					if (graffiti.pos == MurderMO.Graffiti.GraffitiPosition.nearbyWall)
					{
						NewWall newWall = null;
						float num = -9999f;
						if (this.location != null)
						{
							foreach (NewRoom newRoom in this.location.rooms)
							{
								foreach (NewNode newNode in newRoom.nodes)
								{
									if (!newNode.individualFurniture.Exists((FurnitureLocation item) => item.furnitureClasses[0].tall))
									{
										foreach (NewWall newWall2 in newNode.walls)
										{
											if (newWall2.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance && newWall2.preset.sectionClass != DoorPairPreset.WallSectionClass.window)
											{
												float num2 = Toolbox.Instance.Rand(0f, 1f, false);
												num2 += 10f - Vector3.Distance(newWall2.position, this.victim.transform.position);
												if (!newNode.individualFurniture.Exists((FurnitureLocation item) => item.furnitureClasses[0].noBlocking))
												{
													num2 -= 20f;
												}
												if (newNode.individualFurniture.Count <= 0)
												{
													num2 += 5f;
												}
												if (num2 > num)
												{
													num = num2;
													newWall = newWall2;
												}
											}
										}
									}
								}
							}
							if (newWall != null)
							{
								vector = newWall.node.position;
								vector2 = newWall.localEulerAngles;
								goto IL_29E;
							}
						}
						break;
					}
					IL_29E:
					List<Interactable.Passed> list = new List<Interactable.Passed>();
					if (graffiti.artImage != null)
					{
						list.Add(new Interactable.Passed(Interactable.PassedVarType.decal, 0f, graffiti.artImage.name));
					}
					else
					{
						this.graffitiMsg = Strings.GetTextForComponent(graffiti.ddsMessageTextList, this, null, null, "\n", false, null, Strings.LinkSetting.automatic, null);
						string newStr = string.Concat(new string[]
						{
							this.graffitiMsg,
							"|",
							this.murderer.handwriting.fontAsset.name,
							"|",
							graffiti.size.ToString(),
							"|",
							ColorUtility.ToHtmlStringRGB(graffiti.color)
						});
						list.Add(new Interactable.Passed(Interactable.PassedVarType.decalDynamicText, 0f, newStr));
					}
					string text = "Murder: Creating new graffiti at ";
					Vector3 vector3 = vector;
					string text2 = vector3.ToString();
					string text3 = ", euler ";
					vector3 = vector2;
					Game.Log(text + text2 + text3 + vector3.ToString(), 2);
					Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(graffiti.preset, this.murderer, this.murderer, null, vector, vector2, list, null, "");
					if (interactable != null)
					{
						this.graffiti.Add(interactable);
						this.graffitiIDs.Add(interactable.id);
					}
				}
			}
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x000805E4 File Offset: 0x0007E7E4
		public void OnCleanCrimeScene()
		{
			for (int i = 0; i < this.graffiti.Count; i++)
			{
				this.graffiti[i].Delete();
			}
			if (this.callingCard != null)
			{
				this.callingCard.MarkAsTrash(true, false, 0f);
				if (!this.callingCard.inInventory && !FirstPersonItemController.Instance.slots.Exists((FirstPersonItemController.InventorySlot item) => item.GetInteractable() == this.callingCard))
				{
					this.callingCard.RemoveFromPlacement();
				}
			}
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x0008066C File Offset: 0x0007E86C
		public void GenerateRansomDetails()
		{
			this.ransomAmount = Toolbox.Instance.Rand(2, 6, false);
			this.GenerateFakeNumber();
			List<NewBuilding> list = new List<NewBuilding>();
			foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
			{
				if (newBuilding.floors.ContainsKey(-1))
				{
					list.Add(newBuilding);
				}
			}
			if (list.Count > 0)
			{
				this.ransomSite = list[Toolbox.Instance.Rand(0, list.Count, false)];
				this.ransomSiteID = this.ransomSite.buildingID;
				Game.Log("Murder: Chosen " + this.ransomSite.name + " as a ransom delivery location", 2);
			}
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x00080748 File Offset: 0x0007E948
		public virtual void GenerateFakeNumber()
		{
			if (this.fakeNumberStr == null || this.fakeNumberStr.Length <= 0)
			{
				string str = this.murdererID.ToString() + this.victimID.ToString();
				this.fakeNumber = Toolbox.Instance.GetPsuedoRandomNumber(8000000, 8999000, str, false);
				while (CityData.Instance.phoneDictionary.ContainsKey(this.fakeNumber) || TelephoneController.Instance.fakeTelephoneDictionary.ContainsKey(this.fakeNumber))
				{
					this.fakeNumber++;
				}
				this.fakeNumberStr = Toolbox.Instance.GetTelephoneNumberString(this.fakeNumber);
			}
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x000807FA File Offset: 0x0007E9FA
		public void SetRansomPhase(MurderController.KidnapRansomPhase newPhase)
		{
			if (this.ransomPhase != newPhase)
			{
				this.ransomPhase = newPhase;
			}
		}

		// Token: 0x04000846 RID: 2118
		[Header("Serializable Data")]
		public string presetStr;

		// Token: 0x04000847 RID: 2119
		public string moStr;

		// Token: 0x04000848 RID: 2120
		public float creationTime;

		// Token: 0x04000849 RID: 2121
		public int murderID = -1;

		// Token: 0x0400084A RID: 2122
		public int murdererID = -1;

		// Token: 0x0400084B RID: 2123
		public int victimID = -1;

		// Token: 0x0400084C RID: 2124
		public int streetID = -1;

		// Token: 0x0400084D RID: 2125
		public MurderController.MurderState state;

		// Token: 0x0400084E RID: 2126
		public int addressID = -1;

		// Token: 0x0400084F RID: 2127
		public float waitingTimestamp;

		// Token: 0x04000850 RID: 2128
		public float time;

		// Token: 0x04000851 RID: 2129
		public string monkierPre;

		// Token: 0x04000852 RID: 2130
		public string monkierPost;

		// Token: 0x04000853 RID: 2131
		public int victimSiteID = -1;

		// Token: 0x04000854 RID: 2132
		public bool victimSiteIsStreet;

		// Token: 0x04000855 RID: 2133
		public bool kidnapKillPhase;

		// Token: 0x04000856 RID: 2134
		public Vector3Int sniperKillShotNode;

		// Token: 0x04000857 RID: 2135
		[Space(7f)]
		public int ransomSiteID = -1;

		// Token: 0x04000858 RID: 2136
		public int ransomAmount = 1;

		// Token: 0x04000859 RID: 2137
		public int fakeNumber = -1;

		// Token: 0x0400085A RID: 2138
		public string fakeNumberStr;

		// Token: 0x0400085B RID: 2139
		public MurderController.KidnapRansomPhase ransomPhase;

		// Token: 0x0400085C RID: 2140
		public float killTime;

		// Token: 0x0400085D RID: 2141
		[Space(7f)]
		public string weaponStr;

		// Token: 0x0400085E RID: 2142
		public string ammoStr;

		// Token: 0x0400085F RID: 2143
		public int weaponID = -1;

		// Token: 0x04000860 RID: 2144
		public int ammoID = -1;

		// Token: 0x04000861 RID: 2145
		public int weaponSourceID = -1;

		// Token: 0x04000862 RID: 2146
		public bool acquiredEquipment;

		// Token: 0x04000863 RID: 2147
		public float dropChance;

		// Token: 0x04000864 RID: 2148
		[Space(7f)]
		public string callingCardStr;

		// Token: 0x04000865 RID: 2149
		public MurderMO.CallingCardOrigin callingCardOrigin;

		// Token: 0x04000866 RID: 2150
		public int callingCardID = -1;

		// Token: 0x04000867 RID: 2151
		public List<int> graffitiIDs = new List<int>();

		// Token: 0x04000868 RID: 2152
		public string graffitiMsg;

		// Token: 0x04000869 RID: 2153
		[Space(7f)]
		public List<int> cullingActiveRooms = new List<int>();

		// Token: 0x0400086A RID: 2154
		[Header("NonSerialzed Data")]
		[NonSerialized]
		public MurderPreset preset;

		// Token: 0x0400086B RID: 2155
		[NonSerialized]
		public MurderMO mo;

		// Token: 0x0400086C RID: 2156
		[NonSerialized]
		public Human murderer;

		// Token: 0x0400086D RID: 2157
		[NonSerialized]
		public Human victim;

		// Token: 0x0400086E RID: 2158
		[NonSerialized]
		public NewAIGoal murderGoal;

		// Token: 0x0400086F RID: 2159
		[NonSerialized]
		public NewGameLocation location;

		// Token: 0x04000870 RID: 2160
		[NonSerialized]
		public Human.Death death;

		// Token: 0x04000871 RID: 2161
		[NonSerialized]
		public Dictionary<JobPreset.JobTag, Interactable> activeMurderItems = new Dictionary<JobPreset.JobTag, Interactable>();

		// Token: 0x04000872 RID: 2162
		[NonSerialized]
		public InteractablePreset weaponPreset;

		// Token: 0x04000873 RID: 2163
		[NonSerialized]
		public InteractablePreset ammoPreset;

		// Token: 0x04000874 RID: 2164
		[NonSerialized]
		public Interactable weapon;

		// Token: 0x04000875 RID: 2165
		[NonSerialized]
		public Interactable ammo;

		// Token: 0x04000876 RID: 2166
		[NonSerialized]
		public InteractablePreset callingCardPreset;

		// Token: 0x04000877 RID: 2167
		[NonSerialized]
		public Interactable callingCard;

		// Token: 0x04000878 RID: 2168
		[NonSerialized]
		public Company weaponSource;

		// Token: 0x04000879 RID: 2169
		[NonSerialized]
		public List<Interactable> graffiti = new List<Interactable>();

		// Token: 0x0400087A RID: 2170
		[NonSerialized]
		public NewGameLocation sniperVictimSite;

		// Token: 0x0400087B RID: 2171
		[NonSerialized]
		public NewBuilding ransomSite;

		// Token: 0x02000115 RID: 277
		// (Invoke) Token: 0x06000820 RID: 2080
		public delegate void OnMurderStateChange(MurderController.MurderState newState);
	}
}
