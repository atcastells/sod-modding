using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000091 RID: 145
[Serializable]
public class NewAIGoal : IComparable<NewAIGoal>
{
	// Token: 0x060004C9 RID: 1225 RVA: 0x000474E4 File Offset: 0x000456E4
	public NewAIGoal(NewAIController newController, AIGoalPreset newPreset, float newTrigerTime, float newDuration, NewNode newPassedNode = null, Interactable newPassedInteractable = null, NewGameLocation newPassedGameLocation = null, GroupsController.SocialGroup newPassedGroup = null, MurderController.Murder newMurderRef = null, float newTraitMultiplier = 1f, int newPassedVar = -2)
	{
		this.aiController = newController;
		this.preset = newPreset;
		this.triggerTime = newTrigerTime;
		this.duration = newDuration;
		this.lastUpdatedAt = SessionData.Instance.gameTime;
		this.name = newPreset.name;
		this.basePriority = (float)this.preset.basePriority;
		this.traitMultiplier = newTraitMultiplier;
		this.passedNode = newPassedNode;
		this.passedInteractable = newPassedInteractable;
		this.passedGameLocation = newPassedGameLocation;
		this.passedGroup = newPassedGroup;
		this.passedVar = newPassedVar;
		this.murderRef = newMurderRef;
		if (this.preset.name == "Investigate")
		{
			this.aiController.investigationGoal = this;
		}
		if (this.preset.name == "PatrolGameLocation")
		{
			this.aiController.patrolGoal = this;
		}
		this.aiController.goals.Add(this);
		if (this.preset == RoutineControls.Instance.workGoal)
		{
			this.UpdateNextWorkingTimes();
		}
		if (this.passedGroup != null && this.passedGroup.preset != null && this.preset == Toolbox.Instance.groupsDictionary[this.passedGroup.preset].meetUpGoal)
		{
			this.UpdateNextGroupTimes();
		}
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0004767C File Offset: 0x0004587C
	public void UpdateNextWorkingTimes()
	{
		this.lastCheckedForWorkingDay = SessionData.Instance.day;
		this.triggerTime = SessionData.Instance.GetNextOrPreviousGameTimeForThisHour(this.aiController.human.job.workDaysList, this.aiController.human.job.startTimeDecimalHour, this.aiController.human.job.endTimeDecialHour);
		this.duration = this.aiController.human.job.workHours;
		this.debugWorkStartHour = this.aiController.human.job.startTimeDecimalHour;
		this.debugWorkEndHour = this.aiController.human.job.endTimeDecialHour;
		if (!this.startGameWorkCheck && SessionData.Instance.startedGame)
		{
			this.startGameWorkCheck = true;
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00047754 File Offset: 0x00045954
	public void UpdateNextGroupTimes()
	{
		this.lastCheckedForGroupDay = SessionData.Instance.day;
		this.triggerTime = this.passedGroup.GetNextMeetingTime();
		if (!this.startGameGroupCheck && SessionData.Instance.startedGame)
		{
			this.startGameGroupCheck = true;
		}
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00047794 File Offset: 0x00045994
	public void UpdatePriority(bool ignoreDelayTime = false)
	{
		if (!ignoreDelayTime && this.aiController.delayedGoalsForTime.ContainsKey(this.preset))
		{
			if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug(string.Concat(new string[]
				{
					"AI: ",
					this.aiController.human.name,
					" has delayed goal ",
					this.preset.name,
					" : ",
					this.aiController.delayedGoalsForTime[this.preset].ToString(),
					"/",
					SessionData.Instance.gameTime.ToString()
				}), Actor.HumanDebug.actions);
			}
			this.priority = 0f;
			return;
		}
		if (this.isActive)
		{
			this.activeTime += SessionData.Instance.gameTime - this.lastUpdatedAt;
			this.lastUpdatedAt = SessionData.Instance.gameTime;
			if (this.preset.cancelAfterTime && this.activeTime >= this.preset.cancelAfter)
			{
				this.Remove();
				return;
			}
		}
		if (this.preset == RoutineControls.Instance.workGoal)
		{
			if (this.lastCheckedForWorkingDay != SessionData.Instance.day || !this.startGameWorkCheck)
			{
				this.UpdateNextWorkingTimes();
			}
			if (SessionData.Instance.gameTime < this.triggerTime && this.lastEstimatedTravelTime != this.aiController.human.currentGameLocation && this.aiController.human.job.employer != null)
			{
				NewNode newNode = null;
				if (this.aiController.human.workPosition != null)
				{
					newNode = this.aiController.human.workPosition.node;
				}
				else if (this.aiController.human.job.employer.placeOfBusiness != null && this.aiController.human.job.employer.placeOfBusiness.nodes.Count > 0)
				{
					newNode = this.aiController.human.job.employer.placeOfBusiness.nodes[0];
				}
				else if (this.aiController.human.job.employer.address != null && this.aiController.human.job.employer.address.nodes.Count > 0)
				{
					newNode = this.aiController.human.job.employer.address.nodes[0];
				}
				if (newNode != null)
				{
					this.travelTime = Toolbox.Instance.TravelTimeEstimate(this.aiController.human, this.aiController.human.currentNode, newNode);
					this.lastEstimatedTravelTime = this.aiController.human.currentGameLocation;
				}
			}
		}
		if (this.passedGroup != null && this.passedGroup.preset != null && this.preset == Toolbox.Instance.groupsDictionary[this.passedGroup.preset].meetUpGoal)
		{
			if (this.lastCheckedForGroupDay != SessionData.Instance.day || !this.startGameGroupCheck)
			{
				this.UpdateNextGroupTimes();
			}
			if (SessionData.Instance.gameTime < this.triggerTime && this.lastEstimatedTravelTime != this.aiController.human.currentGameLocation)
			{
				NewAddress newAddress = CityData.Instance.addressDictionary[this.passedGroup.meetingPlace];
				this.travelTime = Toolbox.Instance.TravelTimeEstimate(this.aiController.human, this.aiController.human.currentNode, newAddress.anchorNode);
				this.lastEstimatedTravelTime = this.aiController.human.currentGameLocation;
			}
		}
		if (this.murderRef != null)
		{
			if (this.murderRef.state == MurderController.MurderState.none || this.murderRef.state == MurderController.MurderState.waitForLocation || this.murderRef.state == MurderController.MurderState.unsolved || this.murderRef.state == MurderController.MurderState.solved)
			{
				this.priority = 0f;
			}
			else
			{
				this.priority = 10.75f;
			}
			Game.Log(string.Concat(new string[]
			{
				"Murder: Setting murder goal priority: ",
				this.priority.ToString(),
				" (",
				this.murderRef.state.ToString(),
				")"
			}), 2);
			return;
		}
		if (this.preset == RoutineControls.Instance.fleeGoal)
		{
			if (this.aiController.killerForMurders.Count > 0)
			{
				if (this.aiController.killerForMurders.Exists((MurderController.Murder item) => item.state == MurderController.MurderState.executing || item.state == MurderController.MurderState.post))
				{
					this.priority = 0f;
					return;
				}
			}
			float num = Mathf.Clamp01(this.aiController.human.currentNerve / this.aiController.human.maxNerve);
			this.priority = this.preset.minMaxPriority.y - num * this.preset.minMaxPriority.y;
			int num2;
			string text;
			if (this.aiController.reactionState == NewAIController.ReactionState.persuing && this.aiController.investigateLocation != null && this.aiController.human.IsTrespassing(this.aiController.investigateLocation.room, out num2, out text, false))
			{
				this.priority = this.preset.minMaxPriority.y;
				return;
			}
		}
		else
		{
			if (this.preset == RoutineControls.Instance.stealItem)
			{
				if (this.passedInteractable == null)
				{
					Game.LogError("Steal goal with no passed interactable!", 2);
					return;
				}
				if (this.passedInteractable.node == null)
				{
					Game.LogError("Steal interactable with no node assigned!", 2);
					return;
				}
				NewGameLocation newGameLocation = this.passedInteractable.node.gameLocation;
				this.priority = 0f;
				if (!(newGameLocation.thisAsAddress != null))
				{
					return;
				}
				using (List<Human>.Enumerator enumerator = newGameLocation.thisAsAddress.inhabitants.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Human human = enumerator.Current;
						if (human.isHome && !human.isAsleep)
						{
							this.priority = 0f;
							break;
						}
						if (human.isHome && human.isAsleep)
						{
							this.priority += 7f / (float)newGameLocation.thisAsAddress.inhabitants.Count;
						}
						else if (!human.isHome)
						{
							float num3 = Mathf.Min(Vector3.Distance(human.transform.position, newGameLocation.thisAsAddress.entrances[0].worldAccessPoint), 100f);
							this.priority += num3 / 10f / (float)newGameLocation.thisAsAddress.inhabitants.Count;
						}
					}
					return;
				}
			}
			if (!this.preset.onlyImportantBetweenHours || (SessionData.Instance.decimalClock >= this.preset.validBetweenHours.x && SessionData.Instance.decimalClock <= this.preset.validBetweenHours.y))
			{
				this.priority = Toolbox.Instance.Rand(0f, (float)this.preset.randomVariance, false);
				if (this.preset.useLateDebtPriority && this.aiController.human.job != null && this.aiController.human.job.employer != null)
				{
					GameplayController.LoanDebt loanDebt = GameplayController.Instance.debt.Find((GameplayController.LoanDebt item) => item.companyID == this.aiController.human.job.employer.companyID);
					if (loanDebt != null && SessionData.Instance.gameTime > loanDebt.nextPaymentDueBy)
					{
						this.priority += this.preset.minMaxPriority.y;
					}
				}
				float num4 = 1f;
				if (this.preset.multiplyUsingTrashCarried)
				{
					num4 = (float)this.aiController.human.anywhereTrash;
				}
				float num5 = 1f;
				if (this.preset.rainFactor == AIGoalPreset.RainFactor.dontDoWhenRaining)
				{
					num5 = Mathf.Clamp01(1f - SessionData.Instance.currentRain);
				}
				else if (this.preset.rainFactor == AIGoalPreset.RainFactor.onlyDoWhenRaining)
				{
					num5 = Mathf.Clamp01(SessionData.Instance.currentRain);
				}
				float num6 = 1f;
				if (this.preset.affectPriorityOverTime)
				{
					num6 += this.activeTime * this.preset.multiplierModifierOverOneHour;
				}
				if (this.preset.useMusic)
				{
					if (!this.aiController.human.currentRoom.musicPlaying)
					{
						this.basePriority = -99f;
					}
					else if (SessionData.Instance.gameTime > this.aiController.human.currentRoom.musicStartedAt + 0.05f)
					{
						Interactable interactable = GameplayController.Instance.activeMusicPlayers.Find((Interactable item) => item.node != null && item.node.room == this.aiController.human.currentRoom);
						if (interactable != null)
						{
							if (interactable.loopingAudio.Exists((AudioController.LoopingSoundInfo item) => item.eventPreset.canDanceTo && item.state == 0))
							{
								this.basePriority = (float)this.preset.basePriority;
							}
							else
							{
								this.basePriority = -99f;
							}
						}
						else
						{
							this.basePriority = -99f;
						}
					}
					else
					{
						this.basePriority = -99f;
					}
				}
				if (this.preset.useTrespassing)
				{
					if (this.aiController.human.isTrespassing)
					{
						this.basePriority = 99f;
					}
					else
					{
						this.basePriority = -99f;
					}
				}
				this.priority += this.basePriority * num4 * num5 * num6;
				if (this.preset.useTiming)
				{
					float num7 = this.triggerTime - this.travelTime - SessionData.Instance.gameTime;
					if (num7 >= 0f && num7 <= this.preset.earlyTimingWindow)
					{
						if (this.preset.cancelIfLate && num7 < -this.preset.cancelIfThisLate)
						{
							this.Remove();
							return;
						}
						float num8 = 1f - Mathf.Max(num7, 0f) / this.preset.earlyTimingWindow;
						this.timingWeight = num8 * (float)this.preset.timingImportance;
						this.priority += this.timingWeight;
					}
					else if (num7 < 0f && SessionData.Instance.gameTime < this.triggerTime + this.duration)
					{
						this.timingWeight = (float)this.preset.timingImportance;
						if (this.preset == RoutineControls.Instance.workGoal && this.aiController.human.job.lunchBreak && SessionData.Instance.gameTime > this.triggerTime + this.aiController.human.job.lunchBreakHoursAfterStart && SessionData.Instance.gameTime < this.triggerTime + this.aiController.human.job.lunchBreakHoursAfterStart + 0.45f)
						{
							this.timingWeight = 0f;
						}
						this.priority += this.timingWeight;
					}
					else
					{
						this.timingWeight = 0f;
					}
				}
				this.nourishmentWeight = (1f - this.aiController.human.nourishment) * (float)this.preset.nourishmentImportance;
				this.priority += this.nourishmentWeight;
				this.hydrationWeight = (1f - this.aiController.human.hydration) * (float)this.preset.hydrationImportance;
				this.priority += this.hydrationWeight;
				this.altertnessWeight = (1f - this.aiController.human.alertness) * (float)this.preset.alertnessImportance;
				this.priority += this.altertnessWeight;
				this.energyWeight = (1f - this.aiController.human.energy) * (float)this.preset.energyImportance;
				this.priority += this.energyWeight;
				this.excitementWeight = (1f - this.aiController.human.excitement) * (float)this.preset.excitementImportance;
				this.priority += this.excitementWeight;
				this.choresWeight = (1f - this.aiController.human.chores) * (float)this.preset.choresImportance;
				this.priority += this.choresWeight;
				this.hygeieneWeight = (1f - this.aiController.human.hygiene) * (float)this.preset.hygieneImportance;
				this.priority += this.hygeieneWeight;
				this.bladderWeight = (1f - this.aiController.human.bladder) * (float)this.preset.bladderImportance;
				this.priority += this.bladderWeight;
				this.heatWeight = (1f - this.aiController.human.heat) * (float)this.preset.heatImportance;
				this.priority += this.heatWeight;
				this.drunkWeight = (1f - this.aiController.human.drunk) * (float)this.preset.drunkImportance;
				this.priority += this.drunkWeight;
				this.breathWeight = (1f - this.aiController.human.breath) * (float)this.preset.breathImportance;
				this.priority += this.breathWeight;
				this.poisonedWeight = this.aiController.human.poisoned * (float)this.preset.poisonImportance;
				this.priority += this.poisonedWeight;
				this.blindedWeight = this.aiController.human.blinded * (float)this.preset.blindedImportance;
				this.priority += this.blindedWeight;
				using (List<AIGoalPreset>.Enumerator enumerator2 = this.preset.ifGoalsPresent.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						AIGoalPreset p = enumerator2.Current;
						if (this.aiController.goals.Exists((NewAIGoal item) => item != this && item.preset == p))
						{
							this.priority += this.preset.otherGoalPriorityModifier;
							break;
						}
					}
				}
				if (this.preset.sniperVictimBoost && this.aiController.victimsForMurders.Count > 0)
				{
					if (this.aiController.victimsForMurders.Exists((MurderController.Murder item) => item.preset != null && item.preset.caseType == MurderPreset.CaseType.sniper && item.state == MurderController.MurderState.travellingTo))
					{
						this.priority = this.preset.minMaxPriority.y;
					}
				}
				this.priority = Mathf.Clamp(this.priority * this.traitMultiplier, this.preset.minMaxPriority.x, this.preset.minMaxPriority.y);
				return;
			}
			this.priority = 0f;
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x00048798 File Offset: 0x00046998
	public void OnActivate()
	{
		if (!this.isActive)
		{
			if (this.aiController == null || this.aiController.human == null || this.preset == null)
			{
				return;
			}
			if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug("Activate: " + this.name, Actor.HumanDebug.actions);
				this.aiController.AddDebugAction("Activate: " + this.name);
			}
			this.isActive = true;
			this.aiController.currentGoal = this;
			this.gameLocation = null;
			if (!this.aiController.human.isOnDuty && (this.preset == RoutineControls.Instance.workGoal || this.preset == RoutineControls.Instance.enforcerResponse || this.preset == RoutineControls.Instance.enforcerGuardDuty || this.aiController.human.currentGameLocation.isCrimeScene || (this.aiController.human.isEnforcer && (this.aiController.human.outfitController.currentOutfit == ClothesPreset.OutfitCategory.work || this.aiController.human.outfitController.currentOutfit == ClothesPreset.OutfitCategory.outdoorsWork))))
			{
				this.aiController.human.isOnDuty = true;
			}
			if (this.preset == RoutineControls.Instance.fleeGoal && !this.aiController.ko)
			{
				this.aiController.CancelCombat();
			}
			if (this.aiController.human.currentConsumables != null && this.preset.trashConsumablesOnActivate && this.aiController.human.currentConsumables.Count > 0)
			{
				int num = 99;
				while (this.aiController.human.currentConsumables.Count > 0 && num > 0)
				{
					this.aiController.human.RemoveCurrentConsumable(this.aiController.human.currentConsumables[0]);
					num--;
				}
			}
			if (this.preset.allowPottering)
			{
				this.SetNextPotterTime();
			}
			try
			{
				if (this.preset.locationOption == AIGoalPreset.LocationOption.useCurrent)
				{
					this.gameLocation = this.aiController.human.currentGameLocation;
				}
				else if (this.preset.locationOption == AIGoalPreset.LocationOption.murderLocation)
				{
					if (this.murderRef.location != null)
					{
						Game.Log("Murder: Setting murder goal location to " + this.murderRef.location.name, 2);
						this.gameLocation = this.murderRef.location;
					}
					else
					{
						this.gameLocation = this.murderRef.murderer.currentGameLocation;
					}
				}
				else if (this.preset.locationOption == AIGoalPreset.LocationOption.passedInteractable && this.passedInteractable != null)
				{
					if (this.passedInteractable.isActor != null)
					{
						this.gameLocation = this.passedInteractable.isActor.currentGameLocation;
					}
					else
					{
						this.gameLocation = this.passedInteractable.node.gameLocation;
					}
				}
				else if (this.preset.locationOption == AIGoalPreset.LocationOption.passedGamelocation)
				{
					this.gameLocation = this.passedGameLocation;
					if (this.passedGameLocation == null && this.passedNode != null)
					{
						this.gameLocation = this.passedNode.gameLocation;
					}
				}
				else if (this.preset.locationOption == AIGoalPreset.LocationOption.investigate && this.aiController.investigateLocation != null)
				{
					this.gameLocation = this.aiController.investigateLocation.gameLocation;
				}
				else if (this.preset.locationOption == AIGoalPreset.LocationOption.patrolLocation)
				{
					this.gameLocation = this.aiController.patrolLocation;
				}
				else if (this.preset.locationOption == AIGoalPreset.LocationOption.work)
				{
					if (this.aiController.human.job != null && this.aiController.human.job.employer != null)
					{
						if (this.aiController.human.job.preset.ownsWorkPosition)
						{
							if (this.aiController.human.workPosition != null && this.aiController.human.workPosition.node != null)
							{
								this.gameLocation = this.aiController.human.workPosition.node.gameLocation;
							}
							else
							{
								Game.LogError("AI: Citizen should own a job furniture/position but doesn't: " + this.aiController.human.name + ")", 2);
								this.gameLocation = this.aiController.human.job.employer.placeOfBusiness;
							}
						}
						else
						{
							this.gameLocation = this.aiController.human.job.employer.placeOfBusiness;
						}
					}
					else
					{
						this.Remove();
					}
				}
				else
				{
					if (this.preset.locationOption == AIGoalPreset.LocationOption.commercialDecision)
					{
						if (Game.Instance.collectDebugData)
						{
							this.aiController.human.SelectedDebug("Attempting commericial decision for " + this.preset.name, Actor.HumanDebug.actions);
						}
						if (this.aiController.human.energy >= 0.2f)
						{
							float num2 = Mathf.Clamp01(this.aiController.timeAtCurrentAddress / RoutineControls.Instance.commericalDecisionMPTimeSpent);
							if (this.aiController.human.currentBuilding == Player.Instance.currentBuilding)
							{
								if (this.aiController.human.currentGameLocation == Player.Instance.currentGameLocation)
								{
									num2 *= RoutineControls.Instance.commericalDecisionMPlayerSameLocation;
								}
								else
								{
									num2 *= RoutineControls.Instance.commericalDecisionMPlayerSameBuilding;
								}
							}
							else
							{
								num2 *= RoutineControls.Instance.commericalDecisionMPlayerElsewhere;
							}
							if (Game.Instance.collectDebugData)
							{
								this.aiController.human.SelectedDebug(string.Concat(new string[]
								{
									"... Commercial chance = ",
									num2.ToString(),
									" (I've been here for ",
									this.aiController.timeAtCurrentAddress.ToString(),
									")"
								}), Actor.HumanDebug.actions);
							}
							if (Toolbox.Instance.Rand(0f, 1f, false) > num2)
							{
								goto IL_90D;
							}
							if (Game.Instance.collectDebugData)
							{
								this.aiController.human.SelectedDebug("... Commercial chance succeded...", Actor.HumanDebug.actions);
							}
							if (this.gameLocation == null)
							{
								NewAddress newAddress = null;
								if (this.aiController.human.favouritePlaces.TryGetValue(this.preset.desireCategory, ref newAddress) && newAddress.company.openForBusinessDesired && newAddress.company.openForBusinessActual)
								{
									this.gameLocation = newAddress;
								}
							}
							if (this.gameLocation == null && this.aiController.human.currentGameLocation != null && this.aiController.human.currentGameLocation.thisAsAddress != null && this.aiController.human.currentGameLocation.thisAsAddress.company != null && this.aiController.human.currentGameLocation.thisAsAddress.company.preset.companyCategories.Contains(this.preset.desireCategory) && (this.aiController.human.job == null || this.aiController.human.job.employer == null || this.aiController.human.job.employer.placeOfBusiness != this.aiController.human.currentGameLocation) && this.aiController.human.currentGameLocation.thisAsAddress.IsPublicallyOpen(false))
							{
								this.gameLocation = this.aiController.human.currentGameLocation;
							}
							try
							{
								if (this.gameLocation == null)
								{
									AIActionPreset firstAction = this.GetFirstAction(null);
									this.passedInteractable = Toolbox.Instance.FindNearestWithAction(firstAction, this.aiController.human.currentRoom, this.aiController.human, AIActionPreset.FindSetting.nonTrespassing, true, null, null, null, false, InteractablePreset.SpecialCase.none, false, null, true, this.preset.allowEnforcersEverywhere, 0f, null, null, false, true, this.preset.desireCategory, false);
									if (this.passedInteractable != null)
									{
										this.gameLocation = this.passedInteractable.node.gameLocation;
										if (Game.Instance.collectDebugData)
										{
											this.aiController.human.SelectedDebug("Made commerical deicsion! " + this.gameLocation.name, Actor.HumanDebug.actions);
										}
									}
									else if (Game.Instance.collectDebugData)
									{
										this.aiController.human.SelectedDebug("...Unable for find public interactable for " + firstAction.name, Actor.HumanDebug.actions);
									}
								}
								goto IL_90D;
							}
							catch
							{
								goto IL_90D;
							}
						}
						if (Game.Instance.collectDebugData)
						{
							this.aiController.human.SelectedDebug("Energy is too low", Actor.HumanDebug.actions);
						}
						IL_90D:
						if (this.gameLocation == null)
						{
							this.gameLocation = this.aiController.human.home;
						}
					}
					if (this.preset.locationOption == AIGoalPreset.LocationOption.commercial)
					{
						if (this.aiController.human.currentGameLocation != null && this.aiController.human.currentGameLocation.thisAsAddress != null && this.aiController.human.currentGameLocation.thisAsAddress.company != null && (this.aiController.human.job == null || this.aiController.human.job.employer != this.aiController.human.currentGameLocation.thisAsAddress.company) && this.aiController.human.currentGameLocation.thisAsAddress.company.preset.companyCategories.Contains(this.preset.desireCategory) && this.aiController.human.currentGameLocation.thisAsAddress.IsPublicallyOpen(false))
						{
							this.gameLocation = this.aiController.human.currentGameLocation;
						}
						if (this.gameLocation == null)
						{
							NewAddress newAddress2 = null;
							if (this.aiController.human.favouritePlaces.TryGetValue(this.preset.desireCategory, ref newAddress2) && newAddress2.company.openForBusinessDesired && newAddress2.company.openForBusinessActual)
							{
								this.gameLocation = newAddress2;
							}
						}
						if (this.gameLocation == null)
						{
							this.passedInteractable = Toolbox.Instance.FindNearestWithAction(this.GetFirstAction(null), this.aiController.human.currentRoom, this.aiController.human, AIActionPreset.FindSetting.onlyPublic, false, null, null, null, false, InteractablePreset.SpecialCase.none, false, null, true, this.preset.allowEnforcersEverywhere, 0f, null, null, false, true, this.preset.desireCategory, false);
							if (this.passedInteractable != null && this.passedInteractable.node != null)
							{
								this.gameLocation = this.passedInteractable.node.gameLocation;
							}
						}
					}
					if (this.preset.locationOption == AIGoalPreset.LocationOption.nearestAvailable || ((this.preset.locationOption == AIGoalPreset.LocationOption.commercial || this.preset.locationOption == AIGoalPreset.LocationOption.commercialDecision) && this.gameLocation == null))
					{
						bool mustContainDesireCategory = false;
						if (this.preset.locationOption == AIGoalPreset.LocationOption.commercialDecision || this.preset.locationOption == AIGoalPreset.LocationOption.commercial)
						{
							mustContainDesireCategory = true;
						}
						this.passedInteractable = Toolbox.Instance.FindNearestWithAction(this.GetFirstAction(null), this.aiController.human.currentRoom, this.aiController.human, AIActionPreset.FindSetting.nonTrespassing, true, null, null, null, false, InteractablePreset.SpecialCase.none, false, null, true, this.preset.allowEnforcersEverywhere, 0f, null, null, false, mustContainDesireCategory, this.preset.desireCategory, false);
						if (this.passedInteractable != null && this.passedInteractable.node != null)
						{
							this.gameLocation = this.passedInteractable.node.gameLocation;
						}
					}
					else if (this.preset.locationOption == AIGoalPreset.LocationOption.home)
					{
						this.gameLocation = this.aiController.human.home;
					}
				}
			}
			catch
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Error: Unable to pick location: " + this.name, Actor.HumanDebug.actions);
				}
			}
			try
			{
				if (this.preset.roomOption == AIGoalPreset.RoomOption.none)
				{
					this.roomLocation = null;
				}
				else if (this.preset.roomOption == AIGoalPreset.RoomOption.bedroom)
				{
					List<Interactable> list = null;
					if (this.aiController.human.home != null && this.aiController.human.home.actionReference.TryGetValue(CitizenControls.Instance.sleep, ref list) && list.Count > 0)
					{
						this.roomLocation = list[0].furnitureParent.anchorNode.room;
					}
				}
				else if (this.preset.roomOption == AIGoalPreset.RoomOption.job && this.aiController.human.job != null && this.aiController && this.aiController.human.job.employer != null)
				{
					if (this.aiController.human.job.preset.preferredRooms != null && this.aiController.human.job.preset.preferredRooms.Count > 0)
					{
						List<NewRoom> list2 = new List<NewRoom>();
						if (this.aiController.human.job.employer.placeOfBusiness != null)
						{
							using (List<RoomConfiguration>.Enumerator enumerator = this.aiController.human.job.preset.preferredRooms.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									RoomConfiguration r = enumerator.Current;
									this.roomLocation = this.aiController.human.job.employer.placeOfBusiness.rooms.Find((NewRoom item) => item.preset == r);
									if (this.roomLocation != null)
									{
										list2.Add(this.roomLocation);
									}
								}
							}
						}
						if (list2.Count > 0)
						{
							this.roomLocation = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
						}
					}
					if (this.roomLocation == null)
					{
						List<NewRoom> list3 = new List<NewRoom>();
						if (this.aiController.human.job.preset.jobAIPosition == OccupationPreset.JobAI.randomBuilding)
						{
							using (Dictionary<int, NewFloor>.Enumerator enumerator2 = this.aiController.human.job.employer.address.building.floors.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									KeyValuePair<int, NewFloor> keyValuePair = enumerator2.Current;
									foreach (NewAddress newAddress3 in keyValuePair.Value.addresses)
									{
										if ((newAddress3.company == null || !(newAddress3 != this.aiController.human.job.employer.address)) && !(newAddress3.residence != null))
										{
											foreach (NewRoom newRoom in newAddress3.rooms)
											{
												if (!newRoom.isNullRoom && !this.aiController.human.job.preset.bannedRooms.Contains(newRoom.preset))
												{
													list3.Add(newRoom);
												}
											}
										}
									}
								}
								goto IL_116C;
							}
						}
						if (this.aiController.human.job.employer.placeOfBusiness != null)
						{
							foreach (NewRoom newRoom2 in this.aiController.human.job.employer.placeOfBusiness.rooms)
							{
								if (!newRoom2.isNullRoom && !this.aiController.human.job.preset.bannedRooms.Contains(newRoom2.preset))
								{
									list3.Add(newRoom2);
								}
							}
						}
						if (this.aiController.human.job.employer.address != null)
						{
							foreach (NewRoom newRoom3 in this.aiController.human.job.employer.address.rooms)
							{
								if (!newRoom3.isNullRoom && !this.aiController.human.job.preset.bannedRooms.Contains(newRoom3.preset))
								{
									list3.Add(newRoom3);
								}
							}
						}
						IL_116C:
						if (list3.Count > 0)
						{
							this.roomLocation = list3[Toolbox.Instance.Rand(0, list3.Count, false)];
						}
						else
						{
							Game.LogError("AI " + this.aiController.human.citizenName + " has no random rooms to choose from...", 2);
						}
					}
				}
				if (this.preset.furnitureOption == AIGoalPreset.FurnitureOption.bed)
				{
					if (this.aiController.human.sleepPosition != null && !this.aiController.human.sleepPosition.node.gameLocation.isCrimeScene)
					{
						this.passedInteractable = this.aiController.human.sleepPosition;
					}
					else if (!this.aiController.human.isHomeless)
					{
						Game.LogError(string.Concat(new string[]
						{
							"AI: Citizen has no sleep position or their home is a crime scene: ",
							this.aiController.human.name,
							" (isHomeless = ",
							this.aiController.human.isHomeless.ToString(),
							")"
						}), 2);
					}
				}
				else if (this.preset.furnitureOption == AIGoalPreset.FurnitureOption.job && this.aiController.human.job != null && this.aiController.human.job.employer != null)
				{
					if (this.aiController.human.job.preset.ownsWorkPosition)
					{
						if (this.aiController.human.workPosition != null)
						{
							this.passedInteractable = this.aiController.human.workPosition;
							this.gameLocation = this.aiController.human.workPosition.node.gameLocation;
							this.roomLocation = this.aiController.human.workPosition.node.room;
						}
						else
						{
							string[] array = new string[10];
							array[0] = "AI: Citizen should own a job furniture/position but doesn't: ";
							array[1] = this.aiController.human.name;
							array[2] = ", ";
							array[3] = this.aiController.human.job.preset.name;
							array[4] = " at ";
							array[5] = this.aiController.human.job.employer.name;
							array[6] = " (work ad: ";
							int num3 = 7;
							NewAddress address = this.aiController.human.job.employer.address;
							array[num3] = ((address != null) ? address.ToString() : null);
							array[8] = ", place of business: ";
							int num4 = 9;
							NewGameLocation placeOfBusiness = this.aiController.human.job.employer.placeOfBusiness;
							array[num4] = ((placeOfBusiness != null) ? placeOfBusiness.ToString() : null);
							Game.LogError(string.Concat(array), 2);
						}
					}
					else if (this.aiController.human.job.preset.jobAIPosition == OccupationPreset.JobAI.workPosition)
					{
						NewRoom newRoom4 = this.roomLocation;
						if (newRoom4 == null && this.aiController.human.job.employer.placeOfBusiness != null)
						{
							foreach (NewRoom newRoom5 in this.aiController.human.job.employer.placeOfBusiness.rooms)
							{
								if (newRoom5 != null)
								{
									newRoom4 = newRoom5;
									break;
								}
							}
						}
						this.passedInteractable = Toolbox.Instance.FindNearestWithAction(this.aiController.human.job.preset.actionSetup[0].actions[0], newRoom4, this.aiController.human, AIActionPreset.FindSetting.allAreas, false, null, this.aiController.human.job.employer.placeOfBusiness, null, true, this.aiController.human.job.preset.jobPostion, false, null, true, this.preset.allowEnforcersEverywhere, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
					}
					else if (this.aiController.human.job.preset.jobAIPosition == OccupationPreset.JobAI.passedCompanyPosition)
					{
						this.passedInteractable = this.aiController.human.job.employer.passedWorkPosition;
						this.gameLocation = this.passedInteractable.node.gameLocation;
						this.roomLocation = this.passedInteractable.node.room;
					}
				}
			}
			catch
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Error: Unable to pick room/interactable: " + this.name, Actor.HumanDebug.actions);
				}
			}
			try
			{
				if (this.preset.onTriggerBark.Count > 0 && Toolbox.Instance.Rand(0f, 1f, false) <= this.preset.chanceOfOnTrigger && this.aiController.human.speechController != null)
				{
					this.aiController.human.speechController.TriggerBark(this.preset.onTriggerBark[Toolbox.Instance.Rand(0, this.preset.onTriggerBark.Count, false)]);
				}
				if (this.gameLocation == null)
				{
					if (Game.Instance.collectDebugData)
					{
						this.aiController.human.SelectedDebug(string.Concat(new string[]
						{
							"AI: No GameLocation for goal ",
							this.name,
							" (",
							this.aiController.human.name,
							"), removing..."
						}), Actor.HumanDebug.actions);
					}
					this.Remove();
				}
				else
				{
					this.RefreshActions(false);
				}
			}
			catch
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Error: Unable to trigger speech: " + this.name, Actor.HumanDebug.actions);
				}
			}
			this.ResetBehaviourCheck(InteractablePreset.ObjectResetCondition.goalActivated);
		}
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0004A00C File Offset: 0x0004820C
	public void RefreshActions(bool refresh = false)
	{
		if (Game.Instance.collectDebugData)
		{
			this.aiController.human.SelectedDebug(string.Concat(new string[]
			{
				this.aiController.human.name,
				" Refreshing/creating actions for ",
				this.preset.name,
				" Refresh: ",
				refresh.ToString()
			}), Actor.HumanDebug.actions);
		}
		for (int i = 0; i < this.actions.Count; i++)
		{
			if (!refresh || !this.actions[i].preset.dontRemoveOnRefresh)
			{
				this.actions[i].Remove(0f);
				i--;
			}
		}
		string seedInput = this.aiController.human.citizenName + SessionData.Instance.gameTime.ToString();
		if (this.preset.actionSource == AIGoalPreset.GoalActionSource.thisConfiguration)
		{
			for (int j = 0; j < this.preset.actionsSetup.Count; j++)
			{
				if (this.preset.actionsSetup[j] != null)
				{
					AIActionPreset aiactionPreset = this.preset.actionsSetup[j].actions[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.actionsSetup[j].actions.Count, seedInput, out seedInput)];
					if ((!refresh || !aiactionPreset.nonRefreshable) && (!aiactionPreset.skipIfAIIsInState || this.aiController.reactionState != aiactionPreset.skipIfReaction))
					{
						NewGameLocation newGameLocation = this.gameLocation;
						if (newGameLocation == null && this.passedInteractable != null && this.passedInteractable.node != null)
						{
							newGameLocation = this.passedInteractable.node.gameLocation;
						}
						if (newGameLocation == null && this.roomLocation != null)
						{
							newGameLocation = this.roomLocation.gameLocation;
						}
						if (newGameLocation == null && this.passedNode != null)
						{
							newGameLocation = this.passedNode.gameLocation;
						}
						float actionChance = this.GetActionChance(this.preset.actionsSetup[j], newGameLocation);
						if (actionChance <= 0f && Game.Instance.collectDebugData)
						{
							this.aiController.human.SelectedDebug(this.aiController.human.name + " Chance for action " + aiactionPreset.name + " is 0, did you mean for this?", Actor.HumanDebug.actions);
						}
						if (actionChance == 1f || Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seedInput, out seedInput) <= actionChance)
						{
							NewAIController newAIController = this.aiController;
							AIActionPreset newPreset = aiactionPreset;
							bool newInsertedAction = false;
							Interactable newPassedInteractable = this.passedInteractable;
							newAIController.CreateNewAction(this, newPreset, newInsertedAction, this.roomLocation, newPassedInteractable, this.passedNode, this.passedGroup, null, false, 3, "");
						}
					}
				}
			}
			return;
		}
		if (this.preset.actionSource == AIGoalPreset.GoalActionSource.jobPreset)
		{
			if (this.aiController.human.job != null && this.aiController.human.job.preset != null)
			{
				for (int k = 0; k < this.aiController.human.job.preset.actionSetup.Count; k++)
				{
					if (this.aiController.human.job.preset.actionSetup[k] != null)
					{
						AIActionPreset aiactionPreset2 = this.aiController.human.job.preset.actionSetup[k].actions[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.aiController.human.job.preset.actionSetup[k].actions.Count, seedInput, out seedInput)];
						if ((!refresh || !aiactionPreset2.nonRefreshable) && (!aiactionPreset2.skipIfAIIsInState || this.aiController.reactionState != aiactionPreset2.skipIfReaction))
						{
							NewGameLocation newGameLocation2 = this.gameLocation;
							if (newGameLocation2 == null && this.passedInteractable != null && this.passedInteractable.node != null)
							{
								newGameLocation2 = this.passedInteractable.node.gameLocation;
							}
							if (newGameLocation2 == null && this.roomLocation != null)
							{
								newGameLocation2 = this.roomLocation.gameLocation;
							}
							if (newGameLocation2 == null && this.passedNode != null)
							{
								newGameLocation2 = this.passedNode.gameLocation;
							}
							float actionChance2 = this.GetActionChance(this.aiController.human.job.preset.actionSetup[k], newGameLocation2);
							if (actionChance2 == 1f || Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seedInput, out seedInput) <= actionChance2)
							{
								NewAIController newAIController2 = this.aiController;
								AIActionPreset newPreset2 = aiactionPreset2;
								bool newInsertedAction2 = false;
								Interactable newPassedInteractable = this.passedInteractable;
								newAIController2.CreateNewAction(this, newPreset2, newInsertedAction2, this.roomLocation, newPassedInteractable, this.passedNode, this.passedGroup, null, false, 3, "");
							}
						}
					}
				}
				return;
			}
		}
		else if (this.preset.actionSource == AIGoalPreset.GoalActionSource.murderPreset)
		{
			if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug(this.aiController.human.name + " Read actions from murder controller...", Actor.HumanDebug.actions);
			}
			List<AIGoalPreset.GoalActionSetup> list = null;
			Interactable interactable = this.passedInteractable;
			NewRoom newPassedRoom = this.roomLocation;
			List<InteractablePreset> list2 = null;
			if (this.murderRef == null)
			{
				Game.LogError("AI goal " + this.preset.name + " is expecting a passed murder ref to set up the action references, but it doesn't have one...", 2);
			}
			else if (this.murderRef.state == MurderController.MurderState.acquireEuipment)
			{
				if (!this.murderRef.preset.acquirePassInteractable)
				{
					interactable = null;
				}
				if (!this.murderRef.preset.acquirePassRoom)
				{
					newPassedRoom = null;
				}
				list2 = new List<InteractablePreset>();
				if (!this.murderRef.murderer.inventory.Exists((Interactable item) => item.preset == this.murderRef.weaponPreset))
				{
					float num;
					Interactable interactable2 = Toolbox.Instance.FindClosestObjectTo(this.murderRef.weaponPreset, this.murderRef.murderer.FindSafeTeleport(this.murderRef.murderer.home, false, true).position, this.murderRef.murderer.home.building, this.murderRef.murderer.home, null, out num, false);
					if (interactable2 != null)
					{
						Game.Log("Murder: Found weapon at home, inserting a pick up command...", 2);
						this.PickUpItem(interactable2);
					}
					else
					{
						Game.Log("Murder: Attempting to buy weapon...", 2);
						list2.Add(this.murderRef.weaponPreset);
					}
				}
				else
				{
					Game.Log("Murder: The killer already has this item in their inventory...", 2);
				}
				if (this.murderRef.ammoPreset != null)
				{
					if (!this.murderRef.murderer.inventory.Exists((Interactable item) => item.preset == this.murderRef.ammoPreset))
					{
						Game.Log("Murder: Ammo preset is not in killer's inventory...", 2);
						float num;
						Interactable interactable3 = Toolbox.Instance.FindClosestObjectTo(this.murderRef.ammoPreset, this.murderRef.murderer.FindSafeTeleport(this.murderRef.murderer.home, false, true).position, this.murderRef.murderer.home.building, this.murderRef.murderer.home, null, out num, false);
						if (interactable3 != null)
						{
							Game.Log("Murder: Found weapon at home, inserting a pick up command...", 2);
							this.PickUpItem(interactable3);
						}
						else
						{
							Game.Log("Murder: Attempting to buy ammo...", 2);
							list2.Add(this.murderRef.ammoPreset);
						}
					}
					else
					{
						Game.Log("Murder: The killer already has ammo in their inventory...", 2);
					}
				}
				else
				{
					Game.Log("Murder: The killer does not need ammunition for this weapon type...", 2);
				}
				if (list2.Count > 0)
				{
					if (Game.Instance.collectDebugData)
					{
						string text = "Murder: Killer acquiring items:";
						foreach (InteractablePreset interactablePreset in list2)
						{
							text = text + " " + interactablePreset.name;
						}
						Game.Log(text, 2);
					}
					this.murderRef.weaponSource = Toolbox.Instance.FindNearestThatSells(list2[0], this.murderRef.murderer.home);
					if (this.murderRef.weaponSource != null)
					{
						this.murderRef.weaponSourceID = this.murderRef.weaponSource.companyID;
						interactable = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.purchaseItem, this.murderRef.murderer.FindSafeTeleport(this.murderRef.weaponSource.placeOfBusiness, false, true).room, this.aiController.human, AIActionPreset.FindSetting.allAreas, false, null, this.murderRef.weaponSource.placeOfBusiness, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
						if (interactable != null)
						{
							newPassedRoom = interactable.node.room;
							Game.Log("Murder: Found passed interactable for murder weapon/ammo purchase: " + interactable.name, 2);
						}
						else
						{
							Game.Log("Murder: Unable to find purchase point", 2);
						}
						list = this.murderRef.preset.acquireActionSetup;
					}
					else
					{
						Game.Log("Murder: Unable to find weapon source that sells item, spawning a weapon instead...", 2);
						Interactable interactable4 = InteractableCreator.Instance.CreateWorldInteractable(list2[0], this.aiController.human, this.aiController.human, this.aiController.human, this.aiController.human.transform.position, Vector3.zero, null, null, "");
						if (interactable4 != null)
						{
							Game.Log("Murder: Spanwed and added to inventory...", 2);
							interactable4.SetInInventory(this.aiController.human);
						}
					}
				}
				else
				{
					Game.Log("Murder: The killer doesn't need to purchase anything new...", 2);
				}
			}
			else if (this.murderRef.state == MurderController.MurderState.research)
			{
				if (!this.murderRef.preset.researchPassInteractable)
				{
					interactable = null;
				}
				if (!this.murderRef.preset.researchPassRoom)
				{
					newPassedRoom = null;
				}
				list = this.murderRef.preset.researchActionSetup;
			}
			else if (this.murderRef.state == MurderController.MurderState.travellingTo)
			{
				if (!this.murderRef.preset.travelPassInteractable)
				{
					interactable = null;
				}
				if (!this.murderRef.preset.travelPassRoom)
				{
					newPassedRoom = null;
				}
				list = this.murderRef.preset.travelActionSetup;
			}
			else if (this.murderRef.state == MurderController.MurderState.executing)
			{
				if (!this.murderRef.preset.executePassInteractable)
				{
					interactable = null;
				}
				if (!this.murderRef.preset.executePassRoom)
				{
					newPassedRoom = null;
				}
				list = this.murderRef.preset.executionActionSetup;
			}
			else if (this.murderRef.state == MurderController.MurderState.post)
			{
				if (!this.murderRef.preset.postPassInteractable)
				{
					interactable = null;
				}
				if (!this.murderRef.preset.postPassRoom)
				{
					newPassedRoom = null;
				}
				list = this.murderRef.preset.postActionSetup;
			}
			else if (this.murderRef.state == MurderController.MurderState.escaping)
			{
				if (!this.murderRef.preset.escapePassInteractable)
				{
					interactable = null;
				}
				if (!this.murderRef.preset.escapePassRoom)
				{
					newPassedRoom = null;
				}
				list = this.murderRef.preset.escapeActionSetup;
			}
			if (list != null)
			{
				for (int l = 0; l < list.Count; l++)
				{
					if (list[l] != null)
					{
						AIActionPreset aiactionPreset3 = list[l].actions[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list[l].actions.Count, seedInput, out seedInput)];
						if ((!refresh || !aiactionPreset3.nonRefreshable) && (!aiactionPreset3.skipIfAIIsInState || this.aiController.reactionState != aiactionPreset3.skipIfReaction))
						{
							NewGameLocation newGameLocation3 = this.gameLocation;
							if (newGameLocation3 == null && this.passedInteractable != null && this.passedInteractable.node != null)
							{
								newGameLocation3 = this.passedInteractable.node.gameLocation;
							}
							if (newGameLocation3 == null && this.roomLocation != null)
							{
								newGameLocation3 = this.roomLocation.gameLocation;
							}
							if (newGameLocation3 == null && this.passedNode != null)
							{
								newGameLocation3 = this.passedNode.gameLocation;
							}
							float actionChance3 = this.GetActionChance(list[l], newGameLocation3);
							if (actionChance3 == 1f || Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seedInput, out seedInput) <= actionChance3)
							{
								if (Game.Instance.collectDebugData)
								{
									this.aiController.human.SelectedDebug(string.Concat(new string[]
									{
										this.aiController.human.name,
										" Creating new action ",
										aiactionPreset3.presetName,
										" with a chance of ",
										actionChance3.ToString()
									}), Actor.HumanDebug.actions);
								}
								NewAIController newAIController3 = this.aiController;
								AIActionPreset newPreset3 = aiactionPreset3;
								bool newInsertedAction3 = false;
								Interactable newPassedInteractable = interactable;
								newAIController3.CreateNewAction(this, newPreset3, newInsertedAction3, newPassedRoom, newPassedInteractable, this.passedNode, this.passedGroup, list2, false, 3, "");
							}
							else if (Game.Instance.collectDebugData)
							{
								this.aiController.human.SelectedDebug(string.Concat(new string[]
								{
									this.aiController.human.name,
									" Skipping creation of action ",
									aiactionPreset3.presetName,
									" with a chance of ",
									actionChance3.ToString()
								}), Actor.HumanDebug.actions);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0004AD94 File Offset: 0x00048F94
	public void OnDeactivate(float delayReactivationTime)
	{
		this.aiController.human.isOnDuty = false;
		if (this.isActive)
		{
			this.ResetBehaviourCheck(InteractablePreset.ObjectResetCondition.goalDeactivated);
			if (this.aiController.currentAction != null)
			{
				this.aiController.currentAction.OnDeactivate(true);
			}
			else if (this.actions.Count > 0 && this.actions[0].isActive)
			{
				this.actions[0].OnDeactivate(true);
			}
			if (delayReactivationTime > 0f && this.preset != RoutineControls.Instance.fleeGoal && this.preset != RoutineControls.Instance.investigateGoal)
			{
				if (!this.aiController.delayedGoalsForTime.ContainsKey(this.preset))
				{
					this.aiController.delayedGoalsForTime.Add(this.preset, 0f);
				}
				this.aiController.delayedGoalsForTime[this.preset] = SessionData.Instance.gameTime + delayReactivationTime;
			}
			if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug("Deactivate Goal: " + this.name, Actor.HumanDebug.actions);
				this.aiController.AddDebugAction("Deactivate Goal: " + this.name);
			}
			this.isActive = false;
			this.aiController.currentGoal = null;
			this.aiController.currentAction = null;
			this.gameLocation = null;
			this.searchProgress = 0;
			this.searchedNodes.Clear();
			this.actions.Clear();
		}
		this.aiController.UpdateHeldItems(AIActionPreset.ActionStateFlag.onGoalDeactivation);
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0004AF3C File Offset: 0x0004913C
	public void AITick()
	{
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		if (Game.Instance.collectDebugData)
		{
			this.aiController.human.SelectedDebug("Goal AITick: " + this.preset.name, Actor.HumanDebug.updates);
		}
		this.InsertActionsCheck();
		if (this.preset.allowPottering)
		{
			this.PotterCheck();
		}
		if (this.preset == RoutineControls.Instance.findDeadBody && this.passedInteractable != null && this.passedInteractable.isActor != null && !this.passedInteractable.isActor.isStunned)
		{
			this.OnDeactivate(this.preset.repeatDelayOnFinishActions);
		}
		if (this.actions.Count > 0)
		{
			if (this.aiController.currentAction != this.actions[0])
			{
				if (this.aiController.currentAction != null)
				{
					if (Game.Instance.collectDebugData)
					{
						this.aiController.human.SelectedDebug(string.Concat(new string[]
						{
							"Deactivating current action ",
							this.aiController.currentAction.name,
							" because priority of ",
							this.actions[0].name,
							" is higher..."
						}), Actor.HumanDebug.actions);
					}
					this.aiController.currentAction.OnDeactivate(true);
				}
				if (this.actions.Count > 0)
				{
					if (!this.actions[0].preset.sleepOnArrival && this.aiController.human.isAsleep)
					{
						this.aiController.human.WakeUp(false);
						return;
					}
					this.actions[0].OnActivate();
				}
			}
			if (this.aiController.currentAction != null)
			{
				this.aiController.currentAction.AITick();
				return;
			}
			if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug("Current action is null! There are " + this.actions.Count.ToString() + " queued actions...", Actor.HumanDebug.actions);
				return;
			}
		}
		else
		{
			if (this.preset.loopingActions)
			{
				this.RefreshActions(true);
				return;
			}
			if (Game.Instance.collectDebugData)
			{
				Actor human = this.aiController.human;
				string text = "Deactivating goal ";
				AIGoalPreset aigoalPreset = this.preset;
				human.SelectedDebug(text + ((aigoalPreset != null) ? aigoalPreset.ToString() : null) + " as there are no actions...", Actor.HumanDebug.actions);
			}
			this.OnDeactivate(this.preset.repeatDelayOnFinishActions);
			if (this.preset.completable)
			{
				this.Complete();
			}
		}
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0004B1DC File Offset: 0x000493DC
	public void InsertActionsCheck()
	{
		if (this.preset.raiseAlarm && this.aiController.human.lastScaredBy != null && this.aiController.human.lastScaredBy as Human != null && this.aiController.human.lastScaredAt == this.aiController.human.currentGameLocation)
		{
			bool flag = false;
			if (this.aiController.confineLocation != null)
			{
				flag = true;
			}
			if (this.aiController.avoidLocations != null && this.aiController.avoidLocations.Count > 0)
			{
				flag = true;
			}
			if (this.aiController.victimsForMurders.Count > 0)
			{
				foreach (MurderController.Murder murder in this.aiController.victimsForMurders)
				{
					if (!(murder.location == null) && murder.state >= MurderController.MurderState.travellingTo && murder.preset.blockVictimFromLeavingLocation)
					{
						flag = true;
					}
				}
			}
			if (!flag && this.aiController.inFleeState && this.aiController.human.lastScaredAt != null && this.aiController.human.lastScaredAt.building != null)
			{
				bool flag2 = true;
				if (this.aiController.human.lastScaredAt.thisAsAddress != null && this.aiController.human.lastScaredAt.thisAsAddress.addressPreset != null && this.aiController.human.lastScaredAt.thisAsAddress.addressPreset.useOwnSecuritySystem)
				{
					flag2 = false;
				}
				float num;
				NewBuilding.AlarmTargetMode alarmTargetMode;
				List<Human> list;
				if (!this.aiController.human.currentGameLocation.IsAlarmActive(out num, out alarmTargetMode, out list) || (flag2 && this.aiController.human.lastScaredAt.floor != null && !this.aiController.human.lastScaredAt.floor.alarmLockdown))
				{
					bool flag3 = true;
					if (this.aiController.inCombat)
					{
						flag3 = false;
						foreach (Actor actor in Player.Instance.witnessesToIllegalActivity)
						{
							if (!(actor == this.aiController.human) && !actor.isDead && actor.ai != null && actor.ai.currentAction != null)
							{
								if (actor.ai.currentAction.preset == RoutineControls.Instance.raiseAlarm)
								{
									flag3 = false;
									break;
								}
								flag3 = true;
							}
						}
					}
					if (flag3)
					{
						if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.raiseAlarm))
						{
							NewBuilding restrictToBuilding = null;
							NewGameLocation restrictTo = this.aiController.human.lastScaredAt;
							if (flag2)
							{
								restrictToBuilding = this.aiController.human.lastScaredAt.building;
								restrictTo = null;
							}
							Interactable interactable = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.raiseAlarm, this.aiController.human.currentRoom, this.aiController.human, AIActionPreset.FindSetting.nonTrespassing, true, null, restrictTo, restrictToBuilding, false, InteractablePreset.SpecialCase.none, false, null, true, this.preset.allowEnforcersEverywhere, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
							if (interactable != null)
							{
								this.TryInsertInteractableAction(interactable, RoutineControls.Instance.raiseAlarm, 11, null, false);
							}
							else
							{
								Game.Log("AI: Tried to find a raise alarm panel, but there is none found!", 2);
							}
						}
					}
				}
			}
		}
		if (!this.preset.disableActionInsertions && (!this.aiController.inCombat || (this.aiController.currentAction != null && (this.aiController.currentAction.preset == RoutineControls.Instance.searchArea || this.aiController.currentAction.preset == RoutineControls.Instance.searchAreaEnforcer))))
		{
			NewRoom newRoom = null;
			NewAIAction newAIAction = null;
			int i = 0;
			while (i < this.actions.Count)
			{
				if (this.actions[i].isActive && !this.actions[i].insertedAction && !(this.actions[i].preset == RoutineControls.Instance.AIPickUpItem) && !(this.actions[i].preset == RoutineControls.Instance.AIPutDownItem))
				{
					if (this.actions[i].node != null)
					{
						newAIAction = this.actions[i];
						newRoom = this.actions[i].node.room;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (newRoom == null || newAIAction == null)
			{
				return;
			}
			newAIAction.checkedForInsertions = true;
			if (this.aiController.human.currentGameLocation == this.aiController.human.home)
			{
				if (newAIAction.node.gameLocation != this.aiController.human.currentGameLocation)
				{
					if (this.aiController.putDownItems.Count > 0)
					{
						for (int j = 0; j < this.aiController.putDownItems.Count; j++)
						{
							Interactable interactable2 = this.aiController.putDownItems[j];
							if (interactable2.rem)
							{
								this.aiController.putDownItems.RemoveAt(j);
								j--;
							}
							else
							{
								try
								{
									if (this.aiController.human.currentGameLocation == interactable2.node.gameLocation && newAIAction.node.gameLocation != interactable2.node.gameLocation)
									{
										if (Game.Instance.collectDebugData)
										{
											this.aiController.human.SelectedDebug("Pick Up if going elsewhere: " + interactable2.GetName(), Actor.HumanDebug.actions);
										}
										this.PickUpItem(interactable2);
									}
								}
								catch
								{
								}
							}
						}
					}
				}
				else
				{
					foreach (Interactable interactable3 in this.aiController.human.inventory)
					{
						if (interactable3.preset.putDownAtHome)
						{
							if (this.murderRef != null && this.murderRef.murderer == this.aiController.human && this.murderRef.mo.requiresSniperVantageAtHome && this.murderRef.state == MurderController.MurderState.travellingTo)
							{
								break;
							}
							if (Game.Instance.collectDebugData)
							{
								this.aiController.human.SelectedDebug("PutDownAtHome: " + interactable3.GetName(), Actor.HumanDebug.actions);
							}
							this.PutDownItem(interactable3, this.aiController.human.home);
						}
					}
				}
			}
			if (newAIAction.preset.doorsAllowed)
			{
				if (this.aiController.human.isAtWork)
				{
					if (this.aiController.human.job.preset.selfEmployed || !(this.aiController.human.job.employer.placeOfBusiness.thisAsAddress != null))
					{
						goto IL_BBD;
					}
					using (List<NewNode.NodeAccess>.Enumerator enumerator4 = this.aiController.human.job.employer.placeOfBusiness.entrances.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							NewNode.NodeAccess nodeAccess = enumerator4.Current;
							if (nodeAccess.door != null)
							{
								if (this.aiController.human.job.employer.openForBusinessDesired && nodeAccess.door.isLocked)
								{
									NewAIGoal.DoorActionCheckResult doorActionCheckResult;
									this.TryInsertDoorAction(nodeAccess.door, RoutineControls.Instance.unlockDoor, NewAIGoal.DoorSide.mySide, 3, out doorActionCheckResult, null, "Unlock: AtWork - opening time", false);
								}
								else if (!this.aiController.human.job.employer.openForBusinessDesired && !nodeAccess.door.isLocked && (this.aiController.human.job.employer.placeOfBusiness.thisAsAddress.addressPreset == null || !this.aiController.human.job.employer.placeOfBusiness.thisAsAddress.addressPreset.disableLockingUp))
								{
									if (!this.aiController.human.job.employer.placeOfBusiness.currentOccupants.Exists((Actor item) => !item.isAtWork && !item.isTrespassing))
									{
										NewAIGoal.DoorActionCheckResult doorActionCheckResult;
										this.TryInsertDoorAction(nodeAccess.door, RoutineControls.Instance.lockDoor, NewAIGoal.DoorSide.forceCurrentOtherSide, 3, out doorActionCheckResult, null, "Lock: AtWork - closing time", false);
									}
								}
							}
						}
						goto IL_BBD;
					}
				}
				if (this.aiController.human.isHome && !GameplayController.Instance.guestPasses.ContainsKey(this.aiController.human.home) && this.aiController.human.isAsleep)
				{
					foreach (NewNode.NodeAccess nodeAccess2 in this.aiController.human.home.entrances)
					{
						if (nodeAccess2.wall != null && nodeAccess2.wall.door != null && !nodeAccess2.wall.door.isClosed && this.aiController.human.CanISee(nodeAccess2.wall.door.doorInteractable))
						{
							NewNode node = nodeAccess2.wall.node;
							if (nodeAccess2.wall.otherWall.node.gameLocation == this.aiController.human.home)
							{
								node = nodeAccess2.wall.otherWall.node;
							}
							bool flag4 = true;
							if (!this.aiController.human.currentRoom.nodes.Contains(node))
							{
								bool flag5 = false;
								foreach (NewNode.NodeAccess nodeAccess3 in this.aiController.human.currentRoom.entrances)
								{
									if (nodeAccess3.accessType == NewNode.NodeAccess.AccessType.openDoorway && nodeAccess3.toNode.room.nodes.Contains(node))
									{
										flag5 = true;
										break;
									}
								}
								if (!flag5)
								{
									flag4 = false;
								}
							}
							if (flag4)
							{
								foreach (Actor actor2 in this.aiController.human.home.owners)
								{
									if (actor2.currentNode == node && actor2.interactingWith != null)
									{
										flag4 = false;
										break;
									}
								}
							}
							if (flag4)
							{
								NewAIGoal.DoorActionCheckResult doorActionCheckResult;
								this.TryInsertDoorAction(nodeAccess2.wall.door, RoutineControls.Instance.lockDoor, NewAIGoal.DoorSide.mySide, 3, out doorActionCheckResult, node, "AnswerDoor", false);
							}
						}
					}
				}
			}
			IL_BBD:
			if (newAIAction.node.gameLocation.thisAsAddress != null && !this.aiController.seesOnPersuit && !this.aiController.human.isAsleep && newAIAction.preset.deactivateAllowed && this.aiController.human.locationsOfAuthority.Contains(this.aiController.human.currentGameLocation))
			{
				if (this.aiController.human.currentRoom != null && (this.preset.category == AIGoalPreset.GoalCategory.trivial || this.preset.category == AIGoalPreset.GoalCategory.important))
				{
					foreach (Interactable interactable4 in this.aiController.human.currentRoom.tamperedInteractables)
					{
						if (this.aiController.human.CanISee(interactable4))
						{
							this.DeactivateInteractable(interactable4);
						}
					}
				}
				this.doorCheckCycle++;
				if (this.doorCheckCycle >= 16)
				{
					if (this.aiController.human.currentRoom.roomType.doorSetting == NewDoor.DoorSetting.leaveClosed)
					{
						AIActionPreset.DoorRule doorRule = this.preset.doorRule;
						if (newAIAction.preset.overrideGoalDoorRule)
						{
							doorRule = newAIAction.preset.doorRule;
						}
						foreach (NewDoor newDoor in this.aiController.human.currentRoom.openDoors)
						{
							if (!(newDoor == this.aiController.openedDoor) && doorRule != AIActionPreset.DoorRule.dontClose && (doorRule != AIActionPreset.DoorRule.onlyCloseToLocation || !(newDoor.wall.node.gameLocation == newDoor.wall.otherWall.node.gameLocation)) && !newDoor.isClosed && !this.aiController.dontEverCloseDoors && this.aiController.human.CanISee(newDoor.doorInteractable))
							{
								NewAIGoal.DoorActionCheckResult doorActionCheckResult;
								this.TryInsertDoorAction(newDoor, RoutineControls.Instance.closeDoor, NewAIGoal.DoorSide.mySide, 0, out doorActionCheckResult, null, "CloseDoors", false);
							}
						}
					}
					this.doorCheckCycle = 0;
				}
			}
			if (this.aiController.human.currentGameLocation.thisAsAddress != null && this.aiController.human.currentRoom != null)
			{
				NewAIAction newAIAction2 = this.aiController.currentAction;
				if (newAIAction2.insertedAction)
				{
					newAIAction2 = newAIAction;
				}
				if (this.aiController.currentAction.preset.turnAllGamelocationLightsOff && newAIAction2.node.gameLocation == this.aiController.human.currentGameLocation)
				{
					using (List<NewRoom>.Enumerator enumerator9 = this.aiController.human.currentGameLocation.rooms.GetEnumerator())
					{
						while (enumerator9.MoveNext())
						{
							NewRoom r = enumerator9.Current;
							if (!(r == newAIAction2.node.room))
							{
								NewAIAction newAIAction3 = this.actions.Find((NewAIAction item) => (item.preset == RoutineControls.Instance.mainLightOff || item.preset == RoutineControls.Instance.mainLightOn || item.preset == RoutineControls.Instance.secondaryLightOn || item.preset == RoutineControls.Instance.secondaryLightOff) && item.passedRoom == r);
								if (newAIAction3 != null)
								{
									newAIAction3.Remove(newAIAction3.preset.repeatDelayOnActionFail);
								}
								if (this.IsLastOccupantOfRoom(r, true))
								{
									this.RoomLightingCheck(r, RoomConfiguration.AILightingBehaviour.LightingPreference.allOff);
								}
							}
						}
					}
				}
				List<RoomConfiguration.AILightingBehaviour> lightingBehaviour;
				if (this.aiController.currentAction != null && this.aiController.currentAction.preset.overrideGoalLightRule && (!this.aiController.currentAction.preset.onlyOverrideIfAtGamelocation || (this.aiController.currentAction.node != null && this.aiController.human.currentGameLocation == this.aiController.currentAction.node.gameLocation)))
				{
					lightingBehaviour = this.aiController.currentAction.preset.lightingBehaviour;
				}
				else if (this.aiController.currentGoal != null && this.aiController.currentGoal.preset.overrideLightingBehaviour && (!this.aiController.currentGoal.preset.onlyOverrideIfAtGamelocation || (this.aiController.currentAction.node != null && this.aiController.human.currentGameLocation == this.aiController.currentAction.node.gameLocation)))
				{
					lightingBehaviour = this.aiController.currentGoal.preset.lightingBehaviour;
				}
				else
				{
					lightingBehaviour = this.aiController.human.currentRoom.preset.lightingBehaviour;
				}
				if (lightingBehaviour != null && lightingBehaviour.Count > 0)
				{
					RoomConfiguration.AILightingBehaviour ailightingBehaviour = lightingBehaviour[0];
					if (lightingBehaviour.Count > 1)
					{
						using (List<RoomConfiguration.AILightingBehaviour>.Enumerator enumerator10 = lightingBehaviour.GetEnumerator())
						{
							if (enumerator10.MoveNext())
							{
								RoomConfiguration.AILightingBehaviour ailightingBehaviour2 = enumerator10.Current;
								if (ailightingBehaviour2.dayRule == RoomConfiguration.AILightingBehaviour.TimeOfDay.always)
								{
									ailightingBehaviour = ailightingBehaviour2;
								}
								else if (ailightingBehaviour2.dayRule == RoomConfiguration.AILightingBehaviour.TimeOfDay.daytime && SessionData.Instance.dayProgress > 0.25f)
								{
									float dayProgress = SessionData.Instance.dayProgress;
									ailightingBehaviour = ailightingBehaviour2;
								}
								else
								{
									ailightingBehaviour = ailightingBehaviour2;
								}
							}
						}
					}
					if (newAIAction2.node.room == this.aiController.human.currentRoom && !this.aiController.seesOnPersuit)
					{
						this.RoomLightingCheck(this.aiController.human.currentRoom, ailightingBehaviour.destinationBehaviour);
						this.arrivedRoom = this.aiController.human.currentRoom;
					}
					else
					{
						if (this.arrivedRoom != this.aiController.human.currentRoom && !this.aiController.seesOnPersuit && !this.aiController.currentAction.insertedAction)
						{
							this.RoomLightingCheck(this.aiController.human.currentRoom, ailightingBehaviour.passthroughBehaviour);
							this.arrivedRoom = this.aiController.human.currentRoom;
						}
						if (newAIAction2.node.room != this.aiController.human.currentRoom)
						{
							if (newAIAction2.node.gameLocation != this.aiController.human.currentGameLocation)
							{
								this.ResetBehaviourCheck(InteractablePreset.ObjectResetCondition.leavingLocation);
								if (!this.aiController.human.locationsOfAuthority.Contains(this.aiController.human.currentGameLocation) || ailightingBehaviour.exitGameLocationBehaviour == RoomConfiguration.AILightingBehaviour.LightingPreference.none || !this.IsLastOccupantOfGameLocation(this.aiController.human.currentGameLocation, true))
								{
									goto IL_13CB;
								}
								using (List<NewRoom>.Enumerator enumerator9 = this.aiController.human.currentGameLocation.rooms.GetEnumerator())
								{
									while (enumerator9.MoveNext())
									{
										NewRoom r = enumerator9.Current;
										NewAIAction newAIAction4 = this.actions.Find((NewAIAction item) => (item.preset == RoutineControls.Instance.mainLightOff || item.preset == RoutineControls.Instance.mainLightOn || item.preset == RoutineControls.Instance.secondaryLightOn || item.preset == RoutineControls.Instance.secondaryLightOff) && item.passedRoom == r);
										if (newAIAction4 != null)
										{
											newAIAction4.Remove(newAIAction4.preset.repeatDelayOnActionFail);
										}
										this.RoomLightingCheck(r, ailightingBehaviour.exitGameLocationBehaviour);
									}
									goto IL_13CB;
								}
							}
							if (ailightingBehaviour.exitRoomBehaviour != RoomConfiguration.AILightingBehaviour.LightingPreference.none)
							{
								NewAIAction newAIAction5 = this.actions.Find((NewAIAction item) => (item.preset == RoutineControls.Instance.mainLightOff || item.preset == RoutineControls.Instance.mainLightOn || item.preset == RoutineControls.Instance.secondaryLightOn || item.preset == RoutineControls.Instance.secondaryLightOff) && item.passedRoom == this.aiController.human.currentRoom);
								if (newAIAction5 != null)
								{
									newAIAction5.Remove(newAIAction5.preset.repeatDelayOnActionFail);
								}
								if (this.IsLastOccupantOfRoom(this.aiController.human.currentRoom, true))
								{
									this.RoomLightingCheck(this.aiController.human.currentRoom, ailightingBehaviour.exitRoomBehaviour);
								}
							}
						}
					}
				}
			}
			IL_13CB:
			if (this.aiController.spooked >= 0.95f && this.aiController.human.isMoving && this.aiController.reactionState == NewAIController.ReactionState.none)
			{
				if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.lookBehindSpooked))
				{
					Game.Log("LOOK BEHIND " + this.aiController.human.name, 2);
					this.aiController.spookCounter++;
					this.aiController.spookCounter = Mathf.Clamp(this.aiController.spookCounter, 0, 2);
					Game.Log("Spook counter: " + this.aiController.spookCounter.ToString(), 2);
					this.aiController.CreateNewAction(this, RoutineControls.Instance.lookBehindSpooked, true, null, null, null, null, null, false, 7, "");
				}
			}
			if ((this.aiController.reactionState == NewAIController.ReactionState.none || this.aiController.reactionState == NewAIController.ReactionState.searching) && (this.preset.category == AIGoalPreset.GoalCategory.trivial || this.preset.category == AIGoalPreset.GoalCategory.important) && (this.aiController.human.isOnStreet || this.aiController.human.currentRoom.mainLightStatus || this.aiController.human.currentRoom.GetSecondaryLightStatus()) && this.aiController.human.locationsOfAuthority.Contains(this.aiController.human.currentGameLocation) && this.aiController.currentGoal != null)
			{
				if (!this.aiController.currentGoal.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.pickupFromFloor))
				{
					foreach (Interactable interactable5 in this.aiController.human.currentRoom.worldObjects)
					{
						if (!interactable5.rPl && interactable5.inInventory == null && interactable5.PickUpTarget(this.aiController.human, false) && this.aiController.human.CanISee(interactable5))
						{
							Game.Log("AI PICKUP " + interactable5.name, 2);
							this.TryInsertInteractableAction(interactable5, RoutineControls.Instance.pickupFromFloor, 4, interactable5.node, true);
							break;
						}
					}
				}
			}
			if (!this.aiController.human.isAsleep)
			{
				if (this.aiController.human.isAtWork && this.aiController.human.job != null && this.aiController.human.job.preset != null)
				{
					if (this.aiController.human.job.preset.canPickUpLitter && (this.aiController.human.currentRoom.worldObjects.Count >= 20 || this.workCleanUpStarted) && Toolbox.Instance.Rand(0f, 1f, false) <= this.aiController.human.conscientiousness + 0.15f && this.aiController.currentGoal != null)
					{
						if (!this.aiController.currentGoal.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.cleanUp))
						{
							int num2 = 0;
							Interactable interactable6 = null;
							float num3 = 999f;
							foreach (Interactable interactable7 in this.aiController.human.currentRoom.worldObjects)
							{
								if (Toolbox.Instance.Rand(0f, 1f, false) <= 0.5f && interactable7.PickUpTarget(this.aiController.human, true))
								{
									num2++;
									this.workCleanUpStarted = true;
									float num4 = Vector3.Distance(this.aiController.human.transform.position, interactable7.wPos);
									if (num4 < num3)
									{
										interactable6 = interactable7;
										num3 = num4;
										if (num4 <= 1.8f)
										{
											break;
										}
									}
								}
							}
							if (interactable6 != null)
							{
								this.TryInsertInteractableAction(interactable6, RoutineControls.Instance.cleanUp, 0, interactable6.node, true);
							}
							if (this.workCleanUpStarted && num2 <= 0)
							{
								this.workCleanUpStarted = false;
							}
						}
					}
				}
				else if (this.aiController.human.isHome && (float)this.aiController.human.currentRoom.worldObjects.Count > 30f - this.aiController.human.conscientiousness * 30f && Toolbox.Instance.Rand(0f, 1f, false) <= this.aiController.human.conscientiousness && this.aiController.currentGoal != null)
				{
					if (!this.aiController.currentGoal.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.cleanUp))
					{
						foreach (Interactable interactable8 in this.aiController.human.currentRoom.worldObjects)
						{
							if (interactable8.PickUpTarget(this.aiController.human, true) && this.TryInsertInteractableAction(interactable8, RoutineControls.Instance.cleanUp, 0, interactable8.node, true))
							{
								break;
							}
						}
					}
				}
			}
			if (this.aiController.human.trash.Count > 0 && !this.aiController.human.isLitterBug)
			{
				for (int k = 0; k < this.aiController.human.trash.Count; k++)
				{
					MetaObject metaObject = CityData.Instance.FindMetaObject(this.aiController.human.trash[k]);
					if (metaObject != null)
					{
						InteractablePreset interactablePreset = null;
						if (Toolbox.Instance.objectPresetDictionary.TryGetValue(metaObject.preset, ref interactablePreset) && (interactablePreset.disposal != Human.DisposalType.homeOnly || this.aiController.human.isHome) && (interactablePreset.disposal != Human.DisposalType.workOnly || this.aiController.human.job.employer == null || this.aiController.human.isAtWork) && (interactablePreset.disposal != Human.DisposalType.homeOrWork || this.aiController.human.isHome || this.aiController.human.isAtWork))
						{
							if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.disposal))
							{
								Interactable interactable9 = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.disposal, this.aiController.human.currentRoom, this.aiController.human, AIActionPreset.FindSetting.nonTrespassing, true, null, this.aiController.human.currentGameLocation, null, false, InteractablePreset.SpecialCase.none, false, null, true, this.preset.allowEnforcersEverywhere, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
								if (interactable9 != null)
								{
									if (Game.Instance.collectDebugData)
									{
										this.aiController.human.SelectedDebug("Added dispose action: " + interactable9.name + ", " + interactable9.node.gameLocation.name, Actor.HumanDebug.actions);
									}
									if (this.TryInsertInteractableAction(interactable9, RoutineControls.Instance.disposal, 8, null, false))
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			if (this.aiController.human.isEnforcer && this.aiController.human.isOnDuty)
			{
				foreach (KeyValuePair<NewGameLocation, GameplayController.EnforcerCall> keyValuePair in GameplayController.Instance.enforcerCalls)
				{
					if (keyValuePair.Value != null && !(keyValuePair.Key == null) && keyValuePair.Value.state == GameplayController.EnforcerCallState.arrived)
					{
						if (keyValuePair.Key.thisAsAddress != null)
						{
							if (!keyValuePair.Value.response.Contains(this.aiController.human.humanID) || !(this.preset == RoutineControls.Instance.enforcerGuardDuty))
							{
								continue;
							}
							using (List<NewNode.NodeAccess>.Enumerator enumerator4 = keyValuePair.Key.entrances.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									NewNode.NodeAccess acc = enumerator4.Current;
									Predicate<NewAIAction> <>9__11;
									if (acc.walkingAccess && acc.door != null && !acc.door.forbiddenForPublic && !keyValuePair.Key.currentOccupants.Exists(delegate(Actor item)
									{
										if (item.ai != null && item.ai.currentGoal != null)
										{
											List<NewAIAction> list2 = item.ai.currentGoal.actions;
											Predicate<NewAIAction> predicate;
											if ((predicate = <>9__11) == null)
											{
												predicate = (<>9__11 = ((NewAIAction item2) => item2.preset == RoutineControls.Instance.putUpPoliceTape && item2.interactable == acc.door.doorInteractable));
											}
											return list2.Exists(predicate);
										}
										return false;
									}))
									{
										this.TryInsertInteractableAction(acc.door.doorInteractable, RoutineControls.Instance.putUpPoliceTape, 5, null, false);
									}
								}
								continue;
							}
						}
						if (keyValuePair.Key.thisAsStreet != null && keyValuePair.Value.response.Contains(this.aiController.human.humanID) && this.preset == RoutineControls.Instance.enforcerGuardDuty)
						{
							foreach (MurderController.Murder murder2 in MurderController.Instance.activeMurders)
							{
								if (murder2.location == keyValuePair.Key && murder2.location.isCrimeScene && murder2.victim != null && murder2.victim.isDead)
								{
									bool flag6 = true;
									foreach (NewRoom newRoom2 in murder2.location.rooms)
									{
										foreach (NewNode newNode in newRoom2.nodes)
										{
											if (newNode.interactables.Exists((Interactable item) => item.preset == InteriorControls.Instance.streetCrimeScene))
											{
												flag6 = false;
												break;
											}
										}
									}
									if (flag6)
									{
										this.TryInsertInteractableAction(murder2.victim.interactable, RoutineControls.Instance.putUpStreetCrimeScene, 5, null, false);
									}
								}
							}
						}
					}
				}
			}
			this.ResetBehaviourCheck(InteractablePreset.ObjectResetCondition.goalActive);
		}
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0004D324 File Offset: 0x0004B524
	public void ResetBehaviourCheck(InteractablePreset.ObjectResetCondition currentCondition)
	{
		for (int i = 0; i < this.aiController.human.currentGameLocation.resetBehaviourObjects.Count; i++)
		{
			Interactable interactable = this.aiController.human.currentGameLocation.resetBehaviourObjects[i];
			if (interactable == null)
			{
				this.aiController.human.currentGameLocation.resetBehaviourObjects.RemoveAt(i);
				i--;
			}
			else if (interactable.node != null)
			{
				foreach (InteractablePreset.ObjectResetBehaviour objectResetBehaviour in interactable.preset.resetBehaviour)
				{
					if ((objectResetBehaviour.scope != InteractablePreset.ObjectResetScope.ifInSameRoom || !(interactable.node.room != this.aiController.human.currentRoom)) && currentCondition == objectResetBehaviour.ifCondition)
					{
						if (objectResetBehaviour.ifCondition == InteractablePreset.ObjectResetCondition.leavingLocation)
						{
							if (this.aiController.currentDestinationNode == null || !(this.aiController.currentDestinationNode.gameLocation != interactable.node.gameLocation))
							{
								continue;
							}
						}
						else if ((objectResetBehaviour.ifCondition == InteractablePreset.ObjectResetCondition.goalActive || objectResetBehaviour.ifCondition == InteractablePreset.ObjectResetCondition.goalActivated || objectResetBehaviour.ifCondition == InteractablePreset.ObjectResetCondition.goalDeactivated) && this.preset != objectResetBehaviour.ifGoal)
						{
							continue;
						}
						if (interactable.GetSwitchQuery(objectResetBehaviour.ifSwitchState) == objectResetBehaviour.ifSwitchBool && (!objectResetBehaviour.onlyIfAuthority || this.aiController.human.locationsOfAuthority.Contains(interactable.node.gameLocation)) && (!objectResetBehaviour.onlyIfObjectBelongsTo || !(interactable.belongsTo != this.aiController.human)) && (!objectResetBehaviour.onlyIfLastOccupant || this.IsLastOccupantOfGameLocation(this.aiController.human.currentGameLocation, true)) && (!objectResetBehaviour.onlyIfHome || !(this.aiController.human.home != interactable.node.gameLocation)))
						{
							foreach (AIActionPreset aiactionPreset in objectResetBehaviour.insertActions)
							{
								if (!(aiactionPreset == null))
								{
									this.TryInsertInteractableAction(interactable, aiactionPreset, 5, null, true);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0004D5BC File Offset: 0x0004B7BC
	public void PutDownItem(Interactable inventoryItem, NewGameLocation location)
	{
		if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.AIPutDownItem && item.passedInteractable == inventoryItem))
		{
			this.aiController.CreateNewAction(this, RoutineControls.Instance.AIPutDownItem, true, null, inventoryItem, null, null, null, false, 3, "");
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0004D618 File Offset: 0x0004B818
	public void PickUpItem(Interactable inventoryItem)
	{
		if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.AIPickUpItem && item.passedInteractable == inventoryItem))
		{
			this.aiController.CreateNewAction(this, RoutineControls.Instance.AIPickUpItem, true, null, inventoryItem, null, null, null, false, 100, "");
		}
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x0004D678 File Offset: 0x0004B878
	public void RoomLightingCheck(NewRoom room, RoomConfiguration.AILightingBehaviour.LightingPreference pref)
	{
		if (pref == RoomConfiguration.AILightingBehaviour.LightingPreference.none)
		{
			return;
		}
		if (pref == RoomConfiguration.AILightingBehaviour.LightingPreference.mainOn)
		{
			this.TurnSecondaryLightsOff(room);
			this.TurnMainLightOn(room);
			return;
		}
		if (pref == RoomConfiguration.AILightingBehaviour.LightingPreference.mainOnSecondaryAny)
		{
			this.TurnMainLightOn(room);
			return;
		}
		if (pref != RoomConfiguration.AILightingBehaviour.LightingPreference.secondaryOn)
		{
			if (pref == RoomConfiguration.AILightingBehaviour.LightingPreference.eitherPriorityMain)
			{
				if (!room.actionReference.ContainsKey(RoutineControls.Instance.secondaryLightOn))
				{
					this.TurnMainLightOn(room);
					return;
				}
				if (!room.mainLightStatus && !room.GetSecondaryLightStatus())
				{
					this.TurnMainLightOn(room);
					return;
				}
			}
			else if (pref == RoomConfiguration.AILightingBehaviour.LightingPreference.eitherPrioritySecondary)
			{
				if (!room.actionReference.ContainsKey(RoutineControls.Instance.secondaryLightOn))
				{
					this.TurnMainLightOn(room);
					return;
				}
				if (!room.mainLightStatus && !room.GetSecondaryLightStatus())
				{
					this.TurnSecondaryLightOn(room);
					return;
				}
			}
			else if (pref == RoomConfiguration.AILightingBehaviour.LightingPreference.allOff)
			{
				if (room.GetSecondaryLightStatus() && room.actionReference.ContainsKey(RoutineControls.Instance.secondaryLightOn))
				{
					this.TurnSecondaryLightsOff(room);
				}
				if (room.mainLightStatus)
				{
					this.TurnMainLightOff(room);
					return;
				}
			}
			else if (pref == RoomConfiguration.AILightingBehaviour.LightingPreference.mainOff)
			{
				if (room.mainLightStatus)
				{
					this.TurnMainLightOff(room);
					return;
				}
			}
			else if (pref == RoomConfiguration.AILightingBehaviour.LightingPreference.secondaryOff && room.GetSecondaryLightStatus())
			{
				this.TurnSecondaryLightsOff(room);
			}
			return;
		}
		if (room.actionReference.ContainsKey(RoutineControls.Instance.secondaryLightOn))
		{
			this.TurnMainLightOff(room);
			this.TurnSecondaryLightOn(room);
			return;
		}
		this.TurnMainLightOn(room);
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0004D7B8 File Offset: 0x0004B9B8
	public bool IsLastOccupantOfRoom(NewRoom room, bool trueIfAsleep = false)
	{
		foreach (Actor actor in room.currentOccupants)
		{
			if (!actor.isPlayer && !(actor == this.aiController.human) && (trueIfAsleep || !actor.isAsleep))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0004D834 File Offset: 0x0004BA34
	public bool IsLastOccupantOfGameLocation(NewGameLocation gl, bool trueIfAsleep = false)
	{
		foreach (Actor actor in gl.currentOccupants)
		{
			if (!actor.isPlayer && !(actor == this.aiController.human) && (trueIfAsleep || !actor.isAsleep))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0004D8B0 File Offset: 0x0004BAB0
	public void PotterCheck()
	{
		if (SessionData.Instance.gameTime >= this.nextPotterAction)
		{
			List<AIActionPreset> list = null;
			if (this.preset.potterSource == AIGoalPreset.GoalActionSource.thisConfiguration)
			{
				list = this.preset.potterActions;
			}
			else if (this.preset.potterSource == AIGoalPreset.GoalActionSource.jobPreset && this.aiController.human.job != null && this.aiController.human.job.preset != null)
			{
				bool flag = false;
				if (this.aiController.human.job.preset.onlyPotterIfSomebodyElseWorking)
				{
					NewRoom newRoom = null;
					if (this.aiController.human.job.preset.preferredRooms.Count > 0)
					{
						List<NewRoom> list2 = new List<NewRoom>();
						using (List<RoomConfiguration>.Enumerator enumerator = this.aiController.human.job.preset.preferredRooms.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								RoomConfiguration r = enumerator.Current;
								newRoom = this.aiController.human.job.employer.placeOfBusiness.rooms.Find((NewRoom item) => item.preset == r);
								if (newRoom != null)
								{
									list2.Add(this.roomLocation);
								}
							}
						}
						if (list2.Count > 0)
						{
							newRoom = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
						}
					}
					if (!(newRoom != null) || !newRoom.specialCaseInteractables.ContainsKey(this.aiController.human.job.preset.jobPostion))
					{
						goto IL_26F;
					}
					using (List<Interactable>.Enumerator enumerator2 = newRoom.specialCaseInteractables[this.aiController.human.job.preset.jobPostion].GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Interactable interactable = enumerator2.Current;
							foreach (KeyValuePair<Interactable.UsePointSlot, Human> keyValuePair in interactable.usagePoint.users)
							{
								if (keyValuePair.Value != null && keyValuePair.Value != this.aiController.human)
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
						goto IL_26F;
					}
				}
				flag = true;
				IL_26F:
				if (flag)
				{
					list = this.aiController.human.job.preset.potterActions;
				}
			}
			if (list != null && list.Count > 0)
			{
				AIActionPreset aiactionPreset = list[Toolbox.Instance.Rand(0, list.Count, false)];
				if (this.roomLocation == null)
				{
					if (this.preset.roomOption == AIGoalPreset.RoomOption.none)
					{
						this.roomLocation = null;
					}
					else if (this.preset.roomOption == AIGoalPreset.RoomOption.bedroom)
					{
						List<Interactable> list3 = null;
						if (this.aiController.human.home != null && this.aiController.human.home.actionReference.TryGetValue(CitizenControls.Instance.sleep, ref list3) && list3.Count > 0)
						{
							this.roomLocation = list3[0].furnitureParent.anchorNode.room;
						}
					}
					else if (this.preset.roomOption == AIGoalPreset.RoomOption.job && this.aiController.human.job.employer != null)
					{
						if (this.aiController.human.job.preset.preferredRooms.Count > 0)
						{
							List<NewRoom> list4 = new List<NewRoom>();
							using (List<RoomConfiguration>.Enumerator enumerator = this.aiController.human.job.preset.preferredRooms.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									RoomConfiguration r = enumerator.Current;
									this.roomLocation = this.aiController.human.job.employer.placeOfBusiness.rooms.Find((NewRoom item) => item.preset == r);
									if (this.roomLocation != null)
									{
										list4.Add(this.roomLocation);
									}
								}
							}
							if (list4.Count > 0)
							{
								this.roomLocation = list4[Toolbox.Instance.Rand(0, list4.Count, false)];
							}
						}
						if (this.roomLocation == null)
						{
							List<NewRoom> list5 = new List<NewRoom>();
							if (this.aiController.human.job.preset.jobAIPosition == OccupationPreset.JobAI.randomBuilding)
							{
								using (Dictionary<int, NewFloor>.Enumerator enumerator4 = this.aiController.human.job.employer.address.building.floors.GetEnumerator())
								{
									while (enumerator4.MoveNext())
									{
										KeyValuePair<int, NewFloor> keyValuePair2 = enumerator4.Current;
										foreach (NewAddress newAddress in keyValuePair2.Value.addresses)
										{
											if ((newAddress.company == null || !(newAddress != this.aiController.human.job.employer.address)) && !(newAddress.residence != null))
											{
												foreach (NewRoom newRoom2 in newAddress.rooms)
												{
													if (!newRoom2.isNullRoom && !this.aiController.human.job.preset.bannedRooms.Contains(newRoom2.preset))
													{
														list5.Add(newRoom2);
													}
												}
											}
										}
									}
									goto IL_709;
								}
							}
							foreach (NewRoom newRoom3 in this.aiController.human.job.employer.placeOfBusiness.rooms)
							{
								if (!newRoom3.isNullRoom && !this.aiController.human.job.preset.bannedRooms.Contains(newRoom3.preset))
								{
									list5.Add(newRoom3);
								}
							}
							foreach (NewRoom newRoom4 in this.aiController.human.job.employer.address.rooms)
							{
								if (!newRoom4.isNullRoom && !this.aiController.human.job.preset.bannedRooms.Contains(newRoom4.preset))
								{
									list5.Add(newRoom4);
								}
							}
							IL_709:
							if (list5.Count > 0)
							{
								this.roomLocation = list5[Toolbox.Instance.Rand(0, list5.Count, false)];
							}
							else
							{
								Game.LogError("AI has no random rooms to choose from...", 2);
							}
						}
					}
					if (this.roomLocation == null)
					{
						Game.LogError("RoomLocation in goal " + this.preset.name + " is null for " + this.aiController.human.name, 2);
					}
				}
				else
				{
					Interactable interactable2 = Toolbox.Instance.FindNearestWithAction(aiactionPreset, this.roomLocation, this.aiController.human, AIActionPreset.FindSetting.allAreas, false, null, this.gameLocation, null, false, InteractablePreset.SpecialCase.none, false, null, true, this.preset.allowEnforcersEverywhere, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
					if (interactable2 != null)
					{
						this.TryInsertInteractableAction(interactable2, aiactionPreset, 1, null, false);
					}
				}
			}
			this.SetNextPotterTime();
		}
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0004E178 File Offset: 0x0004C378
	private void SetNextPotterTime()
	{
		this.nextPotterAction = SessionData.Instance.gameTime;
		if (this.preset.potterSource == AIGoalPreset.GoalActionSource.thisConfiguration)
		{
			this.nextPotterAction += Toolbox.Instance.Rand(this.preset.potterFrequency.x, this.preset.potterFrequency.y, false);
			return;
		}
		if (this.preset.potterSource == AIGoalPreset.GoalActionSource.jobPreset && this.aiController.human.job != null && this.aiController.human.job.preset != null)
		{
			this.nextPotterAction += Toolbox.Instance.Rand(this.aiController.human.job.preset.potterFrequency.x, this.aiController.human.job.preset.potterFrequency.y, false);
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0004E274 File Offset: 0x0004C474
	public bool TryInsertInteractableAction(Interactable with, AIActionPreset newPreset, int priority, NewNode forcedNode = null, bool duplicateActionCheck = true)
	{
		if (with != null && with.nextAIInteraction == null)
		{
			if (!duplicateActionCheck || !this.actions.Exists((NewAIAction item) => item.preset == newPreset && (with == null || item.passedInteractable == with)))
			{
				NewAIAction newAIAction = this.aiController.CreateNewAction(this, newPreset, true, null, with, forcedNode, this.passedGroup, null, false, priority, "");
				if (newAIAction != null)
				{
					with.SetNextAIInteraction(newAIAction, this.aiController);
					return true;
				}
				return false;
			}
			else if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug("Insert interactable action failed: Duplicate", Actor.HumanDebug.actions);
			}
		}
		else if (with == null && Game.Instance.collectDebugData)
		{
			this.aiController.human.SelectedDebug("Insert interactable action failed: interactable is Null", Actor.HumanDebug.actions);
		}
		else if (Game.Instance.collectDebugData)
		{
			this.aiController.human.SelectedDebug(string.Concat(new string[]
			{
				"Insert interactable action failed: interactable is busy with next action: ",
				with.nextAIInteraction.preset.name,
				"(",
				with.nextAIInteraction.goal.aiController.name,
				")"
			}), Actor.HumanDebug.actions);
		}
		return false;
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0004E3E0 File Offset: 0x0004C5E0
	public bool TryInsertDoorAction(NewDoor door, AIActionPreset preset, NewAIGoal.DoorSide doorSide, int priority, out NewAIGoal.DoorActionCheckResult result, NewNode forcedNode = null, string debug = "", bool immediateTick = false)
	{
		result = NewAIGoal.DoorActionCheckResult.success;
		if (forcedNode == null)
		{
			if (doorSide == NewAIGoal.DoorSide.mySide)
			{
				forcedNode = null;
			}
			else if (doorSide == NewAIGoal.DoorSide.forceCurrentSide)
			{
				if (door.wall.node.room == this.aiController.human.currentRoom)
				{
					forcedNode = door.wall.node;
				}
				else if (door.wall.otherWall.node.room == this.aiController.human.currentRoom)
				{
					forcedNode = door.wall.otherWall.node;
				}
				else if (door.wall.node.gameLocation == this.aiController.human.currentGameLocation)
				{
					forcedNode = door.wall.node;
				}
				else if (door.wall.otherWall.node.gameLocation == this.aiController.human.currentGameLocation)
				{
					forcedNode = door.wall.otherWall.node;
				}
				else
				{
					forcedNode = null;
				}
			}
			else if (doorSide == NewAIGoal.DoorSide.forceCurrentOtherSide)
			{
				if (door.wall.node.room == this.aiController.human.currentRoom)
				{
					forcedNode = door.wall.otherWall.node;
				}
				else if (door.wall.otherWall.node.room == this.aiController.human.currentRoom)
				{
					forcedNode = door.wall.node;
				}
				else if (door.wall.node.gameLocation == this.aiController.human.currentGameLocation)
				{
					forcedNode = door.wall.otherWall.node;
				}
				else if (door.wall.otherWall.node.gameLocation == this.aiController.human.currentGameLocation)
				{
					forcedNode = door.wall.node;
				}
				else
				{
					forcedNode = null;
				}
			}
		}
		if (door.handleInteractable != null)
		{
			if (door.handleInteractable.nextAIInteraction == null || door.handleInteractable.nextAIInteraction.goal.aiController == this.aiController)
			{
				if (!this.actions.Exists((NewAIAction item) => item.preset == preset && item.passedInteractable == door.handleInteractable))
				{
					NewAIAction newAIAction = this.aiController.CreateNewAction(this, preset, true, null, door.handleInteractable, forcedNode, this.passedGroup, null, false, priority, debug);
					if (newAIAction != null)
					{
						door.handleInteractable.SetNextAIInteraction(newAIAction, this.aiController);
						if (immediateTick)
						{
							this.aiController.AITick(false, false);
						}
						return true;
					}
				}
				else
				{
					result = NewAIGoal.DoorActionCheckResult.duplicate;
					if (Game.Instance.collectDebugData)
					{
						this.aiController.human.SelectedDebug(string.Concat(new string[]
						{
							"AI: Action ",
							preset.name,
							" already exists for this door in goal ",
							this.name,
							"..."
						}), Actor.HumanDebug.actions);
					}
				}
			}
			else
			{
				result = NewAIGoal.DoorActionCheckResult.beingUsed;
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("AI: Door is being used by somebody else: " + door.handleInteractable.nextAIInteraction.preset.name + ", " + door.handleInteractable.nextAIInteraction.goal.aiController.human.name, Actor.HumanDebug.actions);
				}
				if (door.handleInteractable.nextAIInteraction != null && SessionData.Instance.gameTime >= door.handleInteractable.nextAIInteraction.createdAt + 0.2f)
				{
					door.handleInteractable.SetNextAIInteraction(null, this.aiController);
				}
			}
		}
		else
		{
			result = NewAIGoal.DoorActionCheckResult.noHandle;
		}
		return false;
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0004E85C File Offset: 0x0004CA5C
	private void TurnMainLightOn(NewAIAction thisAction)
	{
		if (!thisAction.node.room.mainLightStatus && thisAction.node.room.mainLights.Count > 0)
		{
			string empty = string.Empty;
			int num;
			if (!this.preset.allowTrespass && this.aiController.human.IsTrespassing(thisAction.node.room, out num, out empty, this.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Lights in " + thisAction.node.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
				}
				return;
			}
			if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOn && item.passedRoom == thisAction.node.room))
			{
				Predicate<NewAIAction> <>9__1;
				foreach (Actor actor in thisAction.node.room.currentOccupants)
				{
					if (!(actor.ai == null) && !actor.isDead && !actor.isAsleep && !actor.isStunned && actor.ai.currentGoal != null)
					{
						List<NewAIAction> list = actor.ai.currentGoal.actions;
						Predicate<NewAIAction> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOn && item.passedRoom == thisAction.node.room));
						}
						if (list.Exists(predicate))
						{
							return;
						}
					}
				}
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Turn on main light in " + thisAction.node.room.name + ", " + thisAction.node.room.gameLocation.name, Actor.HumanDebug.actions);
				}
				this.aiController.CreateNewAction(this, RoutineControls.Instance.mainLightOn, true, thisAction.node.room, null, null, null, null, thisAction.forceRun, 5, this.aiController.human.currentNode.name);
				return;
			}
			if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug("...Action already exists.", Actor.HumanDebug.actions);
				return;
			}
		}
		else
		{
			if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug("...Main light status is already on", Actor.HumanDebug.actions);
			}
			NewAIAction newAIAction = this.actions.Find((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOn && item.passedRoom == thisAction.node.room);
			if (newAIAction != null)
			{
				newAIAction.Remove(newAIAction.preset.repeatDelayOnActionFail);
			}
		}
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0004EB34 File Offset: 0x0004CD34
	private void TurnMainLightOn(NewRoom where)
	{
		if (!where.mainLightStatus && where.mainLights.Count > 0)
		{
			string empty = string.Empty;
			int num;
			if (!this.preset.allowTrespass && this.aiController.human.IsTrespassing(where, out num, out empty, this.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Lights in " + where.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
				}
				return;
			}
			if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOn && item.passedRoom == where))
			{
				Predicate<NewAIAction> <>9__1;
				foreach (Actor actor in where.currentOccupants)
				{
					if (!(actor.ai == null) && !actor.isDead && !actor.isAsleep && !actor.isStunned && actor.ai.currentGoal != null)
					{
						List<NewAIAction> list = actor.ai.currentGoal.actions;
						Predicate<NewAIAction> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOn && item.passedRoom == where));
						}
						if (list.Exists(predicate))
						{
							return;
						}
					}
				}
				this.aiController.CreateNewAction(this, RoutineControls.Instance.mainLightOn, true, where, null, null, null, null, false, 5, this.aiController.human.currentNode.name);
				return;
			}
		}
		else
		{
			NewAIAction newAIAction = this.actions.Find((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOn && item.passedRoom == where);
			if (newAIAction != null)
			{
				newAIAction.Remove(newAIAction.preset.repeatDelayOnActionFail);
			}
		}
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0004ED2C File Offset: 0x0004CF2C
	private void TurnMainLightOff(NewAIAction thisAction)
	{
		if (thisAction.node.room.mainLightStatus && thisAction.node.room.mainLights.Count > 0)
		{
			string empty = string.Empty;
			int num;
			if (!this.preset.allowTrespass && this.aiController.human.IsTrespassing(thisAction.node.room, out num, out empty, this.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Lights in " + thisAction.node.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
				}
				return;
			}
			if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOff && item.passedRoom == thisAction.node.room))
			{
				Predicate<NewAIAction> <>9__1;
				foreach (Actor actor in thisAction.node.room.currentOccupants)
				{
					if (!(actor.ai == null) && !actor.isDead && !actor.isAsleep && !actor.isStunned && actor.ai.currentGoal != null)
					{
						List<NewAIAction> list = actor.ai.currentGoal.actions;
						Predicate<NewAIAction> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOff && item.passedRoom == thisAction.node.room));
						}
						if (list.Exists(predicate))
						{
							return;
						}
					}
				}
				this.aiController.CreateNewAction(this, RoutineControls.Instance.mainLightOff, true, thisAction.node.room, null, null, null, null, thisAction.forceRun, 6, this.aiController.human.currentNode.name);
				return;
			}
		}
		else
		{
			NewAIAction newAIAction = this.actions.Find((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOff && item.passedRoom == thisAction.node.room);
			if (newAIAction != null)
			{
				newAIAction.Remove(newAIAction.preset.repeatDelayOnActionFail);
			}
		}
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x0004EF64 File Offset: 0x0004D164
	private void TurnMainLightOff(NewRoom where)
	{
		if (where.mainLightStatus && where.mainLights.Count > 0)
		{
			string empty = string.Empty;
			int num;
			if (!this.preset.allowTrespass && this.aiController.human.IsTrespassing(where, out num, out empty, this.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Lights in " + where.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
				}
				return;
			}
			if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOff && item.passedRoom == where))
			{
				Predicate<NewAIAction> <>9__1;
				foreach (Actor actor in where.currentOccupants)
				{
					if (!(actor.ai == null) && !actor.isDead && !actor.isAsleep && !actor.isStunned && actor.ai.currentGoal != null)
					{
						List<NewAIAction> list = actor.ai.currentGoal.actions;
						Predicate<NewAIAction> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOff && item.passedRoom == where));
						}
						if (list.Exists(predicate))
						{
							return;
						}
					}
				}
				this.aiController.CreateNewAction(this, RoutineControls.Instance.mainLightOff, true, where, null, null, null, null, false, 6, this.aiController.human.currentNode.name);
				return;
			}
		}
		else
		{
			NewAIAction newAIAction = this.actions.Find((NewAIAction item) => item.preset == RoutineControls.Instance.mainLightOff && item.passedRoom == where);
			if (newAIAction != null)
			{
				newAIAction.Remove(newAIAction.preset.repeatDelayOnActionFail);
			}
		}
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0004F15C File Offset: 0x0004D35C
	private void TurnSecondaryLightOn(NewAIAction thisAction)
	{
		if (!thisAction.node.room.GetSecondaryLightStatus() && thisAction.node.room.secondaryLights.Count > 0)
		{
			string empty = string.Empty;
			int num;
			if (!this.preset.allowTrespass && this.aiController.human.IsTrespassing(thisAction.node.room, out num, out empty, this.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Lights in " + thisAction.node.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
				}
				return;
			}
			if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.secondaryLightOn && item.passedRoom == thisAction.node.room))
			{
				Predicate<NewAIAction> <>9__1;
				foreach (Actor actor in thisAction.node.room.currentOccupants)
				{
					if (!(actor.ai == null) && !actor.isDead && !actor.isAsleep && !actor.isStunned && actor.ai.currentGoal != null)
					{
						List<NewAIAction> list = actor.ai.currentGoal.actions;
						Predicate<NewAIAction> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((NewAIAction item) => item.preset == RoutineControls.Instance.secondaryLightOn && item.passedRoom == thisAction.node.room));
						}
						if (list.Exists(predicate))
						{
							return;
						}
					}
				}
				this.aiController.CreateNewAction(this, RoutineControls.Instance.secondaryLightOn, true, thisAction.node.room, null, null, null, null, thisAction.forceRun, 5, this.aiController.human.currentNode.name);
				return;
			}
		}
		else
		{
			NewAIAction newAIAction = this.actions.Find((NewAIAction item) => item.preset == RoutineControls.Instance.secondaryLightOn && item.passedRoom == thisAction.node.room);
			if (newAIAction != null)
			{
				newAIAction.Remove(newAIAction.preset.repeatDelayOnActionFail);
			}
		}
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0004F394 File Offset: 0x0004D594
	private void TurnSecondaryLightOn(NewRoom where)
	{
		if (!where.GetSecondaryLightStatus() && where.secondaryLights.Count > 0)
		{
			string empty = string.Empty;
			int num;
			if (!this.preset.allowTrespass && this.aiController.human.IsTrespassing(where, out num, out empty, this.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Lights in " + where.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
				}
				return;
			}
			if (!this.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.secondaryLightOn && item.passedRoom == where))
			{
				Predicate<NewAIAction> <>9__1;
				foreach (Actor actor in where.currentOccupants)
				{
					if (!(actor.ai == null) && !actor.isDead && !actor.isAsleep && !actor.isStunned && actor.ai.currentGoal != null)
					{
						List<NewAIAction> list = actor.ai.currentGoal.actions;
						Predicate<NewAIAction> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((NewAIAction item) => item.preset == RoutineControls.Instance.secondaryLightOn && item.passedRoom == where));
						}
						if (list.Exists(predicate))
						{
							return;
						}
					}
				}
				this.aiController.CreateNewAction(this, RoutineControls.Instance.secondaryLightOn, true, where, null, null, null, null, false, 5, this.aiController.human.currentNode.name);
				return;
			}
		}
		else
		{
			NewAIAction newAIAction = this.actions.Find((NewAIAction item) => item.preset == RoutineControls.Instance.secondaryLightOn && item.passedRoom == where);
			if (newAIAction != null)
			{
				newAIAction.Remove(newAIAction.preset.repeatDelayOnActionFail);
			}
		}
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0004F58C File Offset: 0x0004D78C
	private void TurnSecondaryLightsOff(NewAIAction thisAction)
	{
		if (thisAction.node.room.GetSecondaryLightStatus() && thisAction.node.room.secondaryLights.Count > 0)
		{
			string empty = string.Empty;
			int num;
			if (!this.preset.allowTrespass && this.aiController.human.IsTrespassing(thisAction.node.room, out num, out empty, this.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Lights in " + thisAction.node.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
				}
				return;
			}
			using (List<Interactable>.Enumerator enumerator = thisAction.node.room.secondaryLights.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Interactable inter = enumerator.Current;
					InteractablePreset.InteractionAction interactionAction = inter.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.effectSwitchStates.Exists((InteractablePreset.SwitchState item2) => item2.switchState == InteractablePreset.Switch.switchState && !item2.boolIs));
					if (interactionAction != null)
					{
						AIActionPreset releventAction = interactionAction.action;
						if (!inter.sw0)
						{
							NewAIAction newAIAction = this.actions.Find((NewAIAction item) => item.preset == releventAction && item.passedInteractable == inter);
							if (newAIAction != null)
							{
								newAIAction.Remove(newAIAction.preset.repeatDelayOnActionFail);
							}
						}
						else if (!this.actions.Exists((NewAIAction item) => item.preset == releventAction && item.passedInteractable == inter))
						{
							Predicate<NewAIAction> <>9__4;
							foreach (Actor actor in thisAction.node.room.currentOccupants)
							{
								if (!(actor.ai == null) && !actor.isDead && !actor.isAsleep && !actor.isStunned && actor.ai.currentGoal != null)
								{
									List<NewAIAction> list = actor.ai.currentGoal.actions;
									Predicate<NewAIAction> predicate;
									if ((predicate = <>9__4) == null)
									{
										predicate = (<>9__4 = ((NewAIAction item) => item.preset == releventAction && item.passedInteractable == inter));
									}
									if (list.Exists(predicate))
									{
										return;
									}
								}
							}
							NewAIController newAIController = this.aiController;
							AIActionPreset releventAction2 = releventAction;
							bool newInsertedAction = true;
							NewRoom newPassedRoom = null;
							bool forceRun = thisAction.forceRun;
							newAIController.CreateNewAction(this, releventAction2, newInsertedAction, newPassedRoom, inter, null, null, null, forceRun, 6, this.aiController.human.currentNode.name);
						}
					}
				}
			}
		}
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0004F85C File Offset: 0x0004DA5C
	private void TurnSecondaryLightsOff(NewRoom where)
	{
		if (where.GetSecondaryLightStatus() && where.secondaryLights.Count > 0)
		{
			string empty = string.Empty;
			int num;
			if (!this.preset.allowTrespass && this.aiController.human.IsTrespassing(where, out num, out empty, this.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Lights in " + where.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
				}
				return;
			}
			using (List<Interactable>.Enumerator enumerator = where.secondaryLights.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Interactable inter = enumerator.Current;
					InteractablePreset.InteractionAction interactionAction = inter.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.effectSwitchStates.Exists((InteractablePreset.SwitchState item2) => item2.switchState == InteractablePreset.Switch.switchState && !item2.boolIs));
					if (interactionAction != null)
					{
						AIActionPreset releventAction = interactionAction.action;
						if (!inter.sw0)
						{
							NewAIAction newAIAction = this.actions.Find((NewAIAction item) => item.preset == releventAction && item.passedInteractable == inter);
							if (newAIAction != null)
							{
								newAIAction.Remove(newAIAction.preset.repeatDelayOnActionFail);
							}
						}
						else if (!this.actions.Exists((NewAIAction item) => item.preset == releventAction && item.passedInteractable == inter))
						{
							Predicate<NewAIAction> <>9__4;
							foreach (Actor actor in where.currentOccupants)
							{
								if (!(actor.ai == null) && !actor.isDead && !actor.isAsleep && !actor.isStunned && actor.ai.currentGoal != null)
								{
									List<NewAIAction> list = actor.ai.currentGoal.actions;
									Predicate<NewAIAction> predicate;
									if ((predicate = <>9__4) == null)
									{
										predicate = (<>9__4 = ((NewAIAction item) => item.preset == releventAction && item.passedInteractable == inter));
									}
									if (list.Exists(predicate))
									{
										return;
									}
								}
							}
							this.aiController.CreateNewAction(this, releventAction, true, null, inter, null, null, null, false, 6, this.aiController.human.currentNode.name);
						}
					}
				}
			}
		}
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0004FAEC File Offset: 0x0004DCEC
	private void DeactivateInteractable(Interactable thisInteractable)
	{
		InteractablePreset.InteractionAction interactionAction = thisInteractable.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.action.tamperResetAction);
		if (interactionAction != null)
		{
			this.TryInsertInteractableAction(thisInteractable, interactionAction.action, 4, null, true);
			this.aiController.tamperedObject = thisInteractable;
			this.aiController.human.speechController.TriggerBark(SpeechController.Bark.discoverTamper);
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0004FB64 File Offset: 0x0004DD64
	public bool InsertUnlockAction(NewDoor door, bool lockBehind)
	{
		NewNode newNode = null;
		NewNode newNode2 = null;
		if (this.aiController.human.currentRoom == door.wall.node.room)
		{
			newNode = door.wall.node;
			newNode2 = door.wall.otherWall.node;
		}
		else if (this.aiController.human.currentRoom == door.wall.otherWall.node.room)
		{
			newNode2 = door.wall.node;
			newNode = door.wall.otherWall.node;
		}
		else if (this.aiController.human.currentGameLocation == door.wall.node.gameLocation)
		{
			newNode = door.wall.node;
			newNode2 = door.wall.otherWall.node;
		}
		else if (this.aiController.human.currentGameLocation == door.wall.otherWall.node.gameLocation)
		{
			newNode2 = door.wall.node;
			newNode = door.wall.otherWall.node;
		}
		if (newNode == null)
		{
			Game.Log("Unable to find unlock node", 2);
		}
		if (newNode2 == null)
		{
			Game.Log("Unable to find lock node", 2);
		}
		bool flag = true;
		if (door.isLocked)
		{
			NewAIGoal.DoorActionCheckResult doorActionCheckResult;
			flag = this.TryInsertDoorAction(door, RoutineControls.Instance.unlockDoor, NewAIGoal.DoorSide.mySide, 99, out doorActionCheckResult, newNode, "InsertUnlockAction", false);
		}
		if (flag && lockBehind && door.preset.lockType != DoorPreset.LockType.none && (this.aiController.human.currentGameLocation == null || this.aiController.human.currentGameLocation.thisAsAddress == null || !GameplayController.Instance.guestPasses.ContainsKey(this.aiController.human.currentGameLocation.thisAsAddress) || Player.Instance.currentGameLocation != this.aiController.human.currentGameLocation))
		{
			NewAIGoal.DoorActionCheckResult doorActionCheckResult;
			this.TryInsertDoorAction(door, RoutineControls.Instance.lockDoor, NewAIGoal.DoorSide.forceCurrentOtherSide, 98, out doorActionCheckResult, newNode2, "InsertLockAction", false);
		}
		return flag;
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0004FD94 File Offset: 0x0004DF94
	public bool InsertLockAction(NewDoor door)
	{
		bool result = true;
		if (!door.isLocked && door.preset.lockType != DoorPreset.LockType.none)
		{
			NewAIGoal.DoorActionCheckResult doorActionCheckResult;
			result = ((this.aiController.human.currentGameLocation == null || this.aiController.human.currentGameLocation.thisAsAddress == null || !GameplayController.Instance.guestPasses.ContainsKey(this.aiController.human.currentGameLocation.thisAsAddress) || Player.Instance.currentGameLocation != this.aiController.human.currentGameLocation) && this.TryInsertDoorAction(door, RoutineControls.Instance.lockDoor, NewAIGoal.DoorSide.mySide, 98, out doorActionCheckResult, null, "InsertLockAction2: " + this.aiController.human.currentNode.name, false));
		}
		return result;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0004FE78 File Offset: 0x0004E078
	public void InsertPlayerHidingPlaceRemoval()
	{
		if (InteractionController.Instance.lockedInInteraction != null)
		{
			Game.Log("Insert action for removing player from hiding... " + InteractionController.Instance.lockedInInteraction.name, 2);
			this.TryInsertInteractableAction(InteractionController.Instance.lockedInInteraction, RoutineControls.Instance.pullPlayerFromHiding, 99, null, true);
		}
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0004FED0 File Offset: 0x0004E0D0
	public void OnCompletedAction(NewAIAction completed)
	{
		if (completed.repeat)
		{
			bool flag = true;
			if (completed.preset.repeatWhileHavingConsumables && this.aiController.human.currentConsumables.Count <= 0)
			{
				flag = false;
			}
			if (flag)
			{
				this.aiController.CreateNewAction(this, completed.preset, false, null, null, null, this.passedGroup, null, false, 3, "");
			}
		}
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x0004FF38 File Offset: 0x0004E138
	public void Complete()
	{
		if (Game.Instance.collectDebugData)
		{
			this.aiController.human.SelectedDebug("Completed a goal: " + this.preset.name, Actor.HumanDebug.actions);
		}
		this.aiController.OnCompleteGoal(this);
		this.Remove();
		this.aiController.AITick(false, false);
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0004FF98 File Offset: 0x0004E198
	public void Remove()
	{
		if (Game.Instance.collectDebugData)
		{
			this.aiController.human.SelectedDebug("Remove goal " + this.preset.name, Actor.HumanDebug.actions);
		}
		if (this.isActive)
		{
			for (int i = 0; i < this.actions.Count; i++)
			{
				if (this.actions[i].isActive)
				{
					this.actions[i].OnDeactivate(true);
				}
			}
			if (Game.Instance.collectDebugData)
			{
				this.aiController.human.SelectedDebug("Set current goal to null", Actor.HumanDebug.actions);
			}
			this.aiController.currentGoal = null;
			this.isActive = false;
		}
		this.aiController.goals.Remove(this);
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00050064 File Offset: 0x0004E264
	public StateSaveData.CurrentGoalStateSave GetGoalStateSave()
	{
		if (this.preset.disableSave)
		{
			return null;
		}
		StateSaveData.CurrentGoalStateSave currentGoalStateSave = new StateSaveData.CurrentGoalStateSave();
		currentGoalStateSave.preset = this.preset.name;
		currentGoalStateSave.trigerTime = this.triggerTime;
		currentGoalStateSave.duration = this.duration;
		if (this.passedNode != null)
		{
			currentGoalStateSave.passedNode = this.passedNode.nodeCoord;
		}
		if (this.passedInteractable != null)
		{
			currentGoalStateSave.passedInteractable = this.passedInteractable.id;
		}
		currentGoalStateSave.priority = this.priority;
		currentGoalStateSave.var = this.passedVar;
		currentGoalStateSave.activeTime = this.activeTime;
		if (this.roomLocation != null)
		{
			currentGoalStateSave.room = this.roomLocation.roomID;
		}
		if (this.gameLocation != null)
		{
			if (this.gameLocation.thisAsAddress != null)
			{
				currentGoalStateSave.gameLocation = this.gameLocation.thisAsAddress.id;
				currentGoalStateSave.isAddress = true;
			}
			else
			{
				currentGoalStateSave.gameLocation = this.gameLocation.thisAsStreet.streetID;
				currentGoalStateSave.isAddress = false;
			}
		}
		if (this.passedGroup != null)
		{
			currentGoalStateSave.passedGroup = this.passedGroup.id;
		}
		currentGoalStateSave.jobID = this.jobID;
		foreach (NewAIAction newAIAction in this.actions)
		{
			StateSaveData.AIActionStateSave aiactionStateSave = new StateSaveData.AIActionStateSave();
			aiactionStateSave.preset = newAIAction.preset.name;
			if (newAIAction.node != null)
			{
				aiactionStateSave.node = newAIAction.node.nodeCoord;
			}
			if (newAIAction.interactable != null)
			{
				aiactionStateSave.interactable = newAIAction.interactable.id;
			}
			if (newAIAction.passedInteractable != null)
			{
				aiactionStateSave.passedInteractable = newAIAction.passedInteractable.id;
			}
			if (newAIAction.passedRoom != null)
			{
				aiactionStateSave.passedRoom = newAIAction.passedRoom.roomID;
			}
			if (newAIAction.forcedNode != null)
			{
				aiactionStateSave.forcedNode = newAIAction.forcedNode.nodeCoord;
			}
			aiactionStateSave.repeat = newAIAction.repeat;
			aiactionStateSave.inserted = newAIAction.insertedAction;
			aiactionStateSave.iap = newAIAction.insertedActionPriority;
			if (newAIAction.passedGroup != null)
			{
				aiactionStateSave.passedGroup = newAIAction.passedGroup.id;
			}
			currentGoalStateSave.actions.Add(aiactionStateSave);
		}
		return currentGoalStateSave;
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x000502E0 File Offset: 0x0004E4E0
	public int CompareTo(NewAIGoal otherObject)
	{
		return this.priority.CompareTo(otherObject.priority);
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x000502F4 File Offset: 0x0004E4F4
	public AIActionPreset GetFirstAction(NewGameLocation loc)
	{
		if (this.preset == null)
		{
			return null;
		}
		string seedInput = this.aiController.human.citizenName + SessionData.Instance.gameTime.ToString();
		for (int i = 0; i < this.preset.actionsSetup.Count; i++)
		{
			AIGoalPreset.GoalActionSetup goalActionSetup = this.preset.actionsSetup[i];
			if (goalActionSetup != null && goalActionSetup.actions.Count > 0)
			{
				float actionChance = this.GetActionChance(goalActionSetup, loc);
				if (actionChance == 1f || Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seedInput, out seedInput) <= actionChance)
				{
					return goalActionSetup.actions[Toolbox.Instance.GetPsuedoRandomNumberContained(0, goalActionSetup.actions.Count, seedInput, out seedInput)];
				}
			}
		}
		return null;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x000503C8 File Offset: 0x0004E5C8
	public float GetActionChance(AIGoalPreset.GoalActionSetup actionSetup, NewGameLocation loc)
	{
		float num = actionSetup.chance;
		if (loc != null)
		{
			if (actionSetup.condition == AIGoalPreset.ActionCondition.atHomeOnly && loc != this.aiController.human.home)
			{
				return 0f;
			}
			if (actionSetup.condition == AIGoalPreset.ActionCondition.atHomeNoGuestPass && (loc != this.aiController.human.home || GameplayController.Instance.guestPasses.ContainsKey(this.aiController.human.home)))
			{
				return 0f;
			}
			if (actionSetup.condition == AIGoalPreset.ActionCondition.noGuestPass && this.aiController.human.currentGameLocation != null && this.aiController.human.currentGameLocation.thisAsAddress != null && GameplayController.Instance.guestPasses.ContainsKey(this.aiController.human.currentGameLocation.thisAsAddress))
			{
				return 0f;
			}
			if (actionSetup.condition == AIGoalPreset.ActionCondition.inPublicOnly)
			{
				if (!(loc != this.aiController.human.home) || (this.aiController.human.job != null && this.aiController.human.job.employer != null && !(loc != this.aiController.human.job.employer.placeOfBusiness)))
				{
					return 0f;
				}
			}
			else if (actionSetup.condition == AIGoalPreset.ActionCondition.atWorkOnly)
			{
				if (this.aiController.human.job == null || this.aiController.human.job.employer == null || !(loc == this.aiController.human.job.employer.placeOfBusiness))
				{
					return 0f;
				}
			}
			else if (actionSetup.condition == AIGoalPreset.ActionCondition.onlyIfEscalated)
			{
				if (this.aiController.human.escalationLevel < 2)
				{
					if (Game.Instance.collectDebugData)
					{
						this.aiController.human.SelectedDebug("Skipping action " + actionSetup.actions[0].name + " beacuse escalation isn't high enough: " + this.aiController.human.escalationLevel.ToString(), Actor.HumanDebug.actions);
					}
					return 0f;
				}
			}
			else if (actionSetup.condition == AIGoalPreset.ActionCondition.onlyIfDead)
			{
				if (this.passedInteractable == null || !(this.passedInteractable.isActor != null) || !this.passedInteractable.isActor.isDead)
				{
					if (Game.Instance.collectDebugData)
					{
						this.aiController.human.SelectedDebug("Skipping action " + actionSetup.actions[0].name + " beacuse passed actor is null or alive", Actor.HumanDebug.actions);
					}
					return 0f;
				}
			}
			else if (actionSetup.condition == AIGoalPreset.ActionCondition.kidnapOnly)
			{
				if (this.murderRef != null && this.murderRef.kidnapKillPhase)
				{
					if (Game.Instance.collectDebugData)
					{
						this.aiController.human.SelectedDebug("Skipping action " + actionSetup.actions[0].name + " beacuse the killer is set to kill not kidnap", Actor.HumanDebug.actions);
					}
					return 0f;
				}
			}
			else if (actionSetup.condition == AIGoalPreset.ActionCondition.nonKidnapOnly && this.murderRef != null && !this.murderRef.kidnapKillPhase)
			{
				if (Game.Instance.collectDebugData)
				{
					this.aiController.human.SelectedDebug("Skipping action " + actionSetup.actions[0].name + " beacuse the killer is set to kidnap not kill", Actor.HumanDebug.actions);
				}
				return 0f;
			}
		}
		foreach (AIGoalPreset.StatusModifierRule statusModifierRule in actionSetup.statusModifiers)
		{
			if (statusModifierRule.status == AIGoalPreset.StatusType.alertness)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.alertness >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.alertness <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.bladder)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.bladder >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.bladder <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.chores)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.chores >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.chores <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.energy)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.energy >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.energy <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.excitement)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.excitement >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.excitement <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.health)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.currentHealthNormalized >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.currentHealthNormalized <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.heat)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.heat >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.heat <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.hydration)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.hydration >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.hydration <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.hygeine)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.hygiene >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.hygiene <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.nerve)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.currentNerve >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.currentNerve <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.nourishment)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.nourishment >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.nourishment <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.breath)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrAbove && this.aiController.human.breath >= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isEqualOrBelow && this.aiController.human.breath <= statusModifierRule.value)
				{
					num += statusModifierRule.chanceModifier;
				}
			}
			else if (statusModifierRule.status == AIGoalPreset.StatusType.onDutyEnforcer)
			{
				if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isTrue && this.aiController.human.isEnforcer && this.aiController.human.isOnDuty)
				{
					num += statusModifierRule.chanceModifier;
				}
				else if (statusModifierRule.condition == AIGoalPreset.StatusCondition.isFalse && (!this.aiController.human.isEnforcer || !this.aiController.human.isOnDuty))
				{
					num += statusModifierRule.chanceModifier;
				}
			}
		}
		foreach (AIGoalPreset.GoalModifierRule goalModifierRule in actionSetup.traitModifiers)
		{
			bool flag = false;
			if (goalModifierRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
			{
				using (List<CharacterTrait>.Enumerator enumerator3 = goalModifierRule.traitList.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						CharacterTrait searchTrait = enumerator3.Current;
						if (this.aiController.human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = true;
							break;
						}
					}
					goto IL_B57;
				}
				goto IL_9CF;
			}
			goto IL_9CF;
			IL_B57:
			if (flag)
			{
				num += goalModifierRule.priorityMultiplier;
				continue;
			}
			if (goalModifierRule.mustPassForApplication)
			{
				return 0f;
			}
			continue;
			IL_9CF:
			if (goalModifierRule.rule == CharacterTrait.RuleType.ifAllOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator3 = goalModifierRule.traitList.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						CharacterTrait searchTrait = enumerator3.Current;
						if (!this.aiController.human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_B57;
				}
			}
			if (goalModifierRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator3 = goalModifierRule.traitList.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						CharacterTrait searchTrait = enumerator3.Current;
						if (this.aiController.human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_B57;
				}
			}
			if (goalModifierRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
			{
				if (this.aiController.human.partner != null)
				{
					using (List<CharacterTrait>.Enumerator enumerator3 = goalModifierRule.traitList.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							CharacterTrait searchTrait = enumerator3.Current;
							if (this.aiController.human.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
							{
								flag = true;
								break;
							}
						}
						goto IL_B57;
					}
				}
				flag = false;
				goto IL_B57;
			}
			goto IL_B57;
		}
		return num;
	}

	// Token: 0x04000493 RID: 1171
	public string name = "NewGoal";

	// Token: 0x04000494 RID: 1172
	[Header("Parents")]
	[NonSerialized]
	public NewAIController aiController;

	// Token: 0x04000495 RID: 1173
	public AIGoalPreset preset;

	// Token: 0x04000496 RID: 1174
	[Header("Goal Variables")]
	public float basePriority;

	// Token: 0x04000497 RID: 1175
	private float traitMultiplier = 1f;

	// Token: 0x04000498 RID: 1176
	[Tooltip("Is this goal currently active?")]
	public bool isActive;

	// Token: 0x04000499 RID: 1177
	[Tooltip("The time this should happen at (ideally)")]
	public float triggerTime;

	// Token: 0x0400049A RID: 1178
	[NonSerialized]
	public float duration;

	// Token: 0x0400049B RID: 1179
	public float debugWorkStartHour;

	// Token: 0x0400049C RID: 1180
	public float debugWorkEndHour;

	// Token: 0x0400049D RID: 1181
	private NewGameLocation lastEstimatedTravelTime;

	// Token: 0x0400049E RID: 1182
	[NonSerialized]
	public float travelTime;

	// Token: 0x0400049F RID: 1183
	[Tooltip("The amount of time this has been active")]
	public float activeTime;

	// Token: 0x040004A0 RID: 1184
	private float lastUpdatedAt;

	// Token: 0x040004A1 RID: 1185
	public int jobID = -1;

	// Token: 0x040004A2 RID: 1186
	[Header("Priority")]
	[Tooltip("How important is this action compared to others")]
	[NonSerialized]
	public float timingWeight;

	// Token: 0x040004A3 RID: 1187
	[NonSerialized]
	public float nourishmentWeight;

	// Token: 0x040004A4 RID: 1188
	[NonSerialized]
	public float hydrationWeight;

	// Token: 0x040004A5 RID: 1189
	[NonSerialized]
	public float altertnessWeight;

	// Token: 0x040004A6 RID: 1190
	[NonSerialized]
	public float tirednessWeight;

	// Token: 0x040004A7 RID: 1191
	[NonSerialized]
	public float energyWeight;

	// Token: 0x040004A8 RID: 1192
	[NonSerialized]
	public float excitementWeight;

	// Token: 0x040004A9 RID: 1193
	[NonSerialized]
	public float choresWeight;

	// Token: 0x040004AA RID: 1194
	[NonSerialized]
	public float hygeieneWeight;

	// Token: 0x040004AB RID: 1195
	[NonSerialized]
	public float bladderWeight;

	// Token: 0x040004AC RID: 1196
	[NonSerialized]
	public float heatWeight;

	// Token: 0x040004AD RID: 1197
	[NonSerialized]
	public float drunkWeight;

	// Token: 0x040004AE RID: 1198
	[NonSerialized]
	public float breathWeight;

	// Token: 0x040004AF RID: 1199
	[NonSerialized]
	public float poisonedWeight;

	// Token: 0x040004B0 RID: 1200
	[NonSerialized]
	public float blindedWeight;

	// Token: 0x040004B1 RID: 1201
	[ReadOnly]
	[Space(7f)]
	public float priority;

	// Token: 0x040004B2 RID: 1202
	[Header("Actions")]
	public List<NewAIAction> actions = new List<NewAIAction>();

	// Token: 0x040004B3 RID: 1203
	public float nextPotterAction;

	// Token: 0x040004B4 RID: 1204
	private int doorCheckCycle;

	// Token: 0x040004B5 RID: 1205
	private bool workCleanUpStarted;

	// Token: 0x040004B6 RID: 1206
	public List<Interactable> chosenInteractablesThisGoal = new List<Interactable>();

	// Token: 0x040004B7 RID: 1207
	[Header("Location")]
	public NewGameLocation gameLocation;

	// Token: 0x040004B8 RID: 1208
	public NewRoom roomLocation;

	// Token: 0x040004B9 RID: 1209
	public NewNode passedNode;

	// Token: 0x040004BA RID: 1210
	[NonSerialized]
	public Interactable passedInteractable;

	// Token: 0x040004BB RID: 1211
	public NewGameLocation passedGameLocation;

	// Token: 0x040004BC RID: 1212
	public int passedVar = -2;

	// Token: 0x040004BD RID: 1213
	[NonSerialized]
	public GroupsController.SocialGroup passedGroup;

	// Token: 0x040004BE RID: 1214
	public int searchProgress;

	// Token: 0x040004BF RID: 1215
	public List<NewNode> searchedNodes = new List<NewNode>();

	// Token: 0x040004C0 RID: 1216
	[NonSerialized]
	public MurderController.Murder murderRef;

	// Token: 0x040004C1 RID: 1217
	public SessionData.WeekDay lastCheckedForWorkingDay;

	// Token: 0x040004C2 RID: 1218
	public SessionData.WeekDay lastCheckedForGroupDay;

	// Token: 0x040004C3 RID: 1219
	private bool startGameWorkCheck;

	// Token: 0x040004C4 RID: 1220
	private bool startGameGroupCheck;

	// Token: 0x040004C5 RID: 1221
	private NewRoom arrivedRoom;

	// Token: 0x02000092 RID: 146
	public enum DoorActionCheckResult
	{
		// Token: 0x040004C7 RID: 1223
		success,
		// Token: 0x040004C8 RID: 1224
		noHandle,
		// Token: 0x040004C9 RID: 1225
		beingUsed,
		// Token: 0x040004CA RID: 1226
		duplicate
	}

	// Token: 0x02000093 RID: 147
	public enum DoorSide
	{
		// Token: 0x040004CC RID: 1228
		mySide,
		// Token: 0x040004CD RID: 1229
		forceCurrentSide,
		// Token: 0x040004CE RID: 1230
		forceCurrentOtherSide
	}
}
