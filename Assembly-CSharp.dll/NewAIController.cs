using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class NewAIController : MonoBehaviour
{
	// Token: 0x06000434 RID: 1076 RVA: 0x000396C4 File Offset: 0x000378C4
	public void Setup(Human newParent)
	{
		this.human = newParent;
		this.human.ai = this;
		this.lastUpdated = SessionData.Instance.gameTime;
		this.SetDesiredTickRate(NewAIController.AITickRate.veryLow, true);
		this.original = this.human.lookAtThisTransform.rotation;
		this.lookingQuatPrevious = this.human.neckTransform.rotation;
		this.lookingQuatCurrent = this.human.neckTransform.rotation;
		this.SetExpression(CitizenOutfitController.Expression.neutral);
		this.UpdateCurrentWeapon();
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x0003974C File Offset: 0x0003794C
	public void AITick(bool forceUpdatePriorities = false, bool ignoreRepeatDelays = false)
	{
		if (this.human.isDead)
		{
			return;
		}
		if (Game.Instance.pauseAI)
		{
			return;
		}
		if (this.tickActive)
		{
			return;
		}
		this.tickActive = true;
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("AITick. Force update priorities: " + forceUpdatePriorities.ToString(), Actor.HumanDebug.updates);
		}
		this.timeSinceLastUpdate = SessionData.Instance.gameTime - this.lastUpdated;
		this.timeAtCurrentAddress += this.timeSinceLastUpdate;
		this.dueUpdate = false;
		if (this.spookCounter > 0)
		{
			this.spookForgetCounter += this.timeSinceLastUpdate;
			if (this.spookForgetCounter >= 1f)
			{
				this.spookForgetCounter = 0f;
				this.spookCounter--;
			}
		}
		if (this.delayedActionsForTime.Count > 0)
		{
			this.rem.Clear();
			foreach (KeyValuePair<AIActionPreset, float> keyValuePair in this.delayedActionsForTime)
			{
				if (SessionData.Instance.gameTime > keyValuePair.Value)
				{
					this.rem.Add(keyValuePair.Key);
				}
			}
			foreach (AIActionPreset aiactionPreset in this.rem)
			{
				this.delayedActionsForTime.Remove(aiactionPreset);
			}
		}
		if (this.human.isOnStreet)
		{
			this.human.AddHeat(this.timeSinceLastUpdate * SessionData.Instance.temperature);
		}
		else
		{
			this.human.AddHeat(this.timeSinceLastUpdate * GameplayControls.Instance.indoorTemperature);
		}
		if (this.human.isLitterBug && this.human.isMoving && this.human.currentNode != null && this.human.trash.Count > 0 && Toolbox.Instance.Rand(0f, 1f, false) > 0.6f)
		{
			int i = 0;
			while (i < this.human.trash.Count)
			{
				MetaObject metaObject = CityData.Instance.FindMetaObject(this.human.trash[i]);
				if (metaObject == null)
				{
					goto IL_4BA;
				}
				InteractablePreset interactablePreset = null;
				if (!Toolbox.Instance.objectPresetDictionary.TryGetValue(metaObject.preset, ref interactablePreset))
				{
					Game.LogError("Could not find preset for " + metaObject.preset, 2);
					goto IL_4BA;
				}
				if ((interactablePreset.disposal != Human.DisposalType.homeOnly || this.human.isHome) && (interactablePreset.disposal != Human.DisposalType.workOnly || this.human.job.employer == null || this.human.isAtWork) && (interactablePreset.disposal != Human.DisposalType.homeOrWork || this.human.isHome || this.human.isAtWork))
				{
					metaObject.passed.Add(new Interactable.Passed(Interactable.PassedVarType.isTrash, SessionData.Instance.gameTime, null));
					Vector2 vector = Random.insideUnitCircle * 0.33f;
					Vector3 vector2 = this.human.transform.position;
					if (this.human.currentNode.walkableNodeSpace.Count > 0)
					{
						List<NewNode.NodeSpace> list = new List<NewNode.NodeSpace>(this.human.currentNode.walkableNodeSpace.Values);
						vector2 = list[Toolbox.Instance.Rand(0, list.Count, false)].position + new Vector3(vector.x, 0f, vector.y);
					}
					else
					{
						vector2 += new Vector3(vector.x, 0f, vector.y);
					}
					Vector3 vector3;
					vector3..ctor(0f, Toolbox.Instance.Rand(0f, 360f, false));
					if (Toolbox.Instance.Rand(0f, 1f, false) <= interactablePreset.chanceOfDroppedAngle)
					{
						vector3 += new Vector3(-90f, 0f, 0f);
						vector2.y += interactablePreset.droppedAngleHeightBoost;
					}
					Interactable interactable = InteractableCreator.Instance.CreateWorldInteractableFromMetaObject(metaObject, interactablePreset, vector2, vector3);
					interactable.MarkAsTrash(true, false, 0f);
					if (interactablePreset.consumableAmount > 0f)
					{
						interactable.cs = Toolbox.Instance.Rand(0f, 0.09f, false);
					}
					interactable.ft = true;
					if (interactablePreset.disposal == Human.DisposalType.anywhere)
					{
						this.human.anywhereTrash--;
					}
					this.human.trash.RemoveAt(i);
					break;
				}
				IL_4D2:
				i++;
				continue;
				IL_4BA:
				this.human.trash.RemoveAt(i);
				i--;
				goto IL_4D2;
			}
			this.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
		}
		if (this.human.visible)
		{
			this.human.UpdateCurrentNodeSpace();
		}
		this.StatusStatUpdate();
		if (this.human.poisoned > 0f || (this.human.currentRoom != null && this.human.currentRoom.gasLevel > 0.25f))
		{
			forceUpdatePriorities = true;
		}
		else if (this.human.blinded > 0f)
		{
			forceUpdatePriorities = true;
		}
		if (!forceUpdatePriorities)
		{
			if (this.human.interactingWith != null)
			{
				if (Game.Instance.collectDebugData)
				{
					Actor actor = this.human;
					string text = "Is interacting with ";
					Interactable interactingWith = this.human.interactingWith;
					actor.SelectedDebug(text + ((interactingWith != null) ? interactingWith.ToString() : null), Actor.HumanDebug.misc);
				}
				this.tickActive = false;
				return;
			}
			if (this.human.inConversation)
			{
				if (Game.Instance.collectDebugData)
				{
					Actor actor2 = this.human;
					string text2 = "Is in conversation ";
					Interactable interactingWith2 = this.human.interactingWith;
					actor2.SelectedDebug(text2 + ((interactingWith2 != null) ? interactingWith2.ToString() : null), Actor.HumanDebug.actions);
				}
				this.tickActive = false;
				return;
			}
		}
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.nourishment = this.human.nourishment;
			this.hydration = this.human.hydration;
			this.alertness = this.human.alertness;
			this.excitement = this.human.excitement;
			this.energy = this.human.energy;
			this.chores = this.human.chores;
			this.hygiene = this.human.hygiene;
			this.bladder = this.human.bladder;
			this.heat = this.human.heat;
			this.drunk = this.human.drunk;
			this.breath = this.human.breath;
		}
		if (this.human.isDelayed && SessionData.Instance.gameTime > this.delayedUntil)
		{
			this.human.isDelayed = false;
			this.delayedUntil = 0f;
		}
		if (!this.human.isStunned)
		{
			bool flag = false;
			NewGameLocation newGameLocation = null;
			if (this.confineLocation != null)
			{
				newGameLocation = this.confineLocation;
				flag = true;
			}
			if (this.victimsForMurders.Count > 0)
			{
				foreach (MurderController.Murder murder in this.victimsForMurders)
				{
					if (!(murder.location == null) && murder.state >= MurderController.MurderState.travellingTo && murder.preset.blockVictimFromLeavingLocation)
					{
						flag = true;
						newGameLocation = murder.location;
						break;
					}
				}
			}
			if (flag && this.currentAction != null && this.currentAction.node != null && this.currentAction.node.gameLocation != newGameLocation)
			{
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Current desitnation for " + this.currentAction.name + " is not at confined location: " + newGameLocation.name, Actor.HumanDebug.updates);
				}
				this.currentAction.Remove(this.currentAction.preset.repeatDelayOnActionFail);
			}
			if (!forceUpdatePriorities)
			{
				forceUpdatePriorities = true;
				if (this.currentAction != null && this.currentAction.isActive)
				{
					if (this.currentAction.dontUpdateGoalPriorityForExtraTime > 0f)
					{
						if (Game.Instance.collectDebugData)
						{
							this.human.SelectedDebug("Block update of priorities: Current action extra time: " + this.currentAction.dontUpdateGoalPriorityForExtraTime.ToString(), Actor.HumanDebug.updates);
						}
						forceUpdatePriorities = false;
					}
					else if (this.currentAction.preset.dontUpdateGoalPriorityWhileActive)
					{
						if (Game.Instance.collectDebugData)
						{
							this.human.SelectedDebug("Block update of priorities: While current action active", Actor.HumanDebug.updates);
						}
						forceUpdatePriorities = false;
					}
				}
				if (this.currentGoal != null && this.currentGoal.isActive && this.currentGoal.preset.dontUpdateGoalPriorityWhileActive)
				{
					if (Game.Instance.collectDebugData)
					{
						this.human.SelectedDebug("Block update of priorities: Goal blocking update", Actor.HumanDebug.updates);
					}
					forceUpdatePriorities = false;
				}
			}
			if (forceUpdatePriorities)
			{
				if (this.delayedGoalsForTime.Count > 0)
				{
					List<AIGoalPreset> list2 = new List<AIGoalPreset>();
					foreach (KeyValuePair<AIGoalPreset, float> keyValuePair2 in this.delayedGoalsForTime)
					{
						if (SessionData.Instance.gameTime > keyValuePair2.Value)
						{
							list2.Add(keyValuePair2.Key);
						}
					}
					foreach (AIGoalPreset aigoalPreset in list2)
					{
						this.delayedGoalsForTime.Remove(aigoalPreset);
					}
				}
				for (int j = 0; j < this.goals.Count; j++)
				{
					this.goals[j].UpdatePriority(ignoreRepeatDelays);
				}
			}
			if (this.human.seesOthers)
			{
				this.human.SightingCheck(GameplayControls.Instance.citizenFOV, false);
			}
			if (this.investigateLocation != null)
			{
				float num = SessionData.Instance.gameTime - this.lastInvestigate;
				if (this.tickRate != NewAIController.AITickRate.veryHigh)
				{
					this.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, false);
				}
				if (num <= CitizenControls.Instance.minimumInvestigateTime * this.minimumInvestigationTimeMultiplier)
				{
					this.investigationGoal.priority = 11f;
				}
				else
				{
					this.investigationGoal.priority = 0f;
					if (!this.killerForMurders.Exists((MurderController.Murder item) => item.state == MurderController.MurderState.executing))
					{
						this.ResetInvestigate();
					}
				}
				this.TriggerReactionIndicator();
			}
			else
			{
				this.investigationGoal.priority = 0f;
			}
			this.goals.Sort((NewAIGoal p2, NewAIGoal p1) => p1.priority.CompareTo(p2.priority));
			if (this.goals.Count > 0)
			{
				if (this.goals[0] != this.currentGoal)
				{
					bool flag2 = false;
					NewAIGoal newAIGoal = this.goals[0];
					if (Game.Instance.collectDebugData && this.currentGoal != null && this.currentGoal.preset != null && this.goals.Count > 0 && this.goals[0].preset != null)
					{
						this.human.SelectedDebug("...The current goal " + this.currentGoal.preset.presetName + " is different to the top one " + this.goals[0].preset.presetName, Actor.HumanDebug.actions);
					}
					if (this.currentGoal != null)
					{
						if (this.currentGoal.preset.interuptable)
						{
							int num2 = 0;
							if (this.currentGoal.preset.useInteruptionThreshold)
							{
								num2 = this.currentGoal.preset.interuptionThreshold;
							}
							if (this.currentGoal.preset.unteruptableByFollowingCategories)
							{
								newAIGoal = null;
								using (List<NewAIGoal>.Enumerator enumerator6 = this.goals.GetEnumerator())
								{
									while (enumerator6.MoveNext())
									{
										NewAIGoal newAIGoal2 = enumerator6.Current;
										if (newAIGoal2 != this.currentGoal && !this.currentGoal.preset.uninteruptableByCategories.Contains(newAIGoal2.preset.category) && newAIGoal2.priority >= this.currentGoal.priority + (float)num2 && (newAIGoal == null || newAIGoal2.priority > newAIGoal.priority))
										{
											flag2 = true;
											newAIGoal = newAIGoal2;
										}
									}
									goto IL_E36;
								}
							}
							if (this.goals[0].priority >= this.currentGoal.priority + (float)num2)
							{
								flag2 = true;
								newAIGoal = this.goals[0];
								if (Game.Instance.collectDebugData)
								{
									this.human.SelectedDebug(string.Concat(new string[]
									{
										"...The higher ranking goal (",
										this.goals[0].priority.ToString(),
										") is enough to interupt the current goal (",
										(this.currentGoal.priority + (float)this.currentGoal.preset.interuptionThreshold).ToString(),
										")"
									}), Actor.HumanDebug.actions);
								}
							}
							else if (Game.Instance.collectDebugData)
							{
								this.human.SelectedDebug(string.Concat(new string[]
								{
									"...The top ranked goal ",
									this.goals[0].preset.name,
									" priority ",
									this.goals[0].priority.ToString(),
									" is lower than the current goal rank + interuption threshold ",
									this.currentGoal.priority.ToString(),
									" + ",
									this.currentGoal.preset.interuptionThreshold.ToString()
								}), Actor.HumanDebug.actions);
							}
						}
						else if (Game.Instance.collectDebugData)
						{
							this.human.SelectedDebug("...The current goal is uninteruptable", Actor.HumanDebug.actions);
						}
						IL_E36:
						if (this.currentAction != null && this.currentAction.preset.uninteruptableWhileAtLocation && this.currentAction.isAtLocation)
						{
							if (Game.Instance.collectDebugData)
							{
								this.human.SelectedDebug("...The current action is uninteruptable while at location", Actor.HumanDebug.actions);
							}
							flag2 = false;
						}
					}
					else
					{
						flag2 = true;
						foreach (NewAIGoal newAIGoal3 in this.goals)
						{
							if (newAIGoal3.isActive)
							{
								newAIGoal3.OnDeactivate(0.1f);
							}
						}
					}
					if (flag2 && newAIGoal != null)
					{
						if (this.currentGoal != null)
						{
							this.currentGoal.OnDeactivate(this.currentGoal.preset.repeatDelayOnInterupt);
						}
						if (Game.Instance.collectDebugData)
						{
							this.human.SelectedDebug("Activate a new goal: " + newAIGoal.preset.name, Actor.HumanDebug.actions);
						}
						newAIGoal.OnActivate();
						if (this.currentGoal != null && this.currentGoal.preset == RoutineControls.Instance.fleeGoal)
						{
							this.inFleeState = true;
						}
						else
						{
							this.inFleeState = false;
						}
					}
				}
				if (this.currentGoal != null)
				{
					this.currentGoal.AITick();
				}
				else if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Current goal is null!", Actor.HumanDebug.actions);
				}
			}
			else
			{
				this.inFleeState = false;
				this.currentGoal = null;
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Set current goal to null", Actor.HumanDebug.actions);
				}
			}
			if (this.investigateLocation != null && !this.investigationGoal.isActive)
			{
				if (this.killerForMurders.Count > 0)
				{
					if (this.killerForMurders.Exists((MurderController.Murder item) => item.state == MurderController.MurderState.executing))
					{
						goto IL_101A;
					}
				}
				this.SetReactionState(NewAIController.ReactionState.none);
			}
			IL_101A:
			if (this.human.isAsleep)
			{
				if (this.human.energy >= 0.99f)
				{
					this.human.WakeUp(false);
				}
				if (this.human.awakenPromt <= 0)
				{
					this.human.sleepDepth += (SessionData.Instance.gameTime - this.lastUpdated) * 0.75f;
					this.human.sleepDepth = Mathf.Clamp01(this.human.sleepDepth);
					if (SessionData.Instance.gameTime >= this.lastSnore + this.human.snoreDelay * 0.0167f)
					{
						if (Toolbox.Instance.Rand(0f, 1f, false) < 0.075f)
						{
							this.AwakenPrompt();
						}
						else if (this.human.snoring >= 0.6f)
						{
							if (this.human.genderScale >= 0.5f)
							{
								AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.maleSnoreHeavy, this.human, this.human.currentNode, this.human.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
							}
							else
							{
								AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.femaleSnoreHeavy, this.human, this.human.currentNode, this.human.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
							}
						}
						else if (this.human.genderScale >= 0.5f)
						{
							AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.maleSnoreLight, this.human, this.human.currentNode, this.human.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
						}
						else
						{
							AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.femaleSnoreLight, this.human, this.human.currentNode, this.human.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
						}
						this.lastSnore = SessionData.Instance.gameTime;
						this.human.speechController.TriggerBark(SpeechController.Bark.sleeping);
					}
				}
				else
				{
					this.human.awakenRegen += (SessionData.Instance.gameTime - this.lastUpdated) * 11f;
					if (this.human.awakenRegen >= 1f)
					{
						this.human.awakenRegen = 0f;
						this.human.awakenPromt--;
					}
				}
			}
			else
			{
				if (this.human.sleepDepth > 0f)
				{
					this.human.sleepDepth -= SessionData.Instance.gameTime - this.lastUpdated;
					this.human.sleepDepth = Mathf.Clamp01(this.human.sleepDepth);
				}
				if (this.idleSound <= 0f)
				{
					this.human.speechController.TriggerBark(SpeechController.Bark.idleSounds);
					this.idleSound = Toolbox.Instance.Rand(0.9f, 1f, false);
				}
				if (this.currentTrackTarget != null && this.currentTrackTarget.distance < 8f)
				{
					Actor trackedTarget = null;
					if (this.currentTrackTarget != null)
					{
						trackedTarget = this.currentTrackTarget.actor;
					}
					this.human.SpeechTriggerPoint(DDSSaveClasses.TriggerPoint.whileTickOnTrackTarget, trackedTarget, null);
				}
			}
			if (this.human.currentRoom != null && this.human.currentRoom.isOutsideWindow && !this.human.isDead && !this.isRagdoll)
			{
				if (this.human.previousNode != null && !this.human.previousNode.room.isOutsideWindow && !this.human.previousNode.room.isNullRoom)
				{
					this.human.Teleport(this.human.previousNode, null, true, false);
				}
				else if (this.human.previousRoom != null && !this.human.previousRoom.isOutsideWindow && !this.human.previousRoom.isNullRoom)
				{
					this.human.Teleport(this.human.FindSafeTeleport(this.human.previousRoom.gameLocation, false, true), null, true, false);
				}
				else if (this.human.home != null)
				{
					this.human.Teleport(this.human.FindSafeTeleport(this.human.home, false, true), null, true, false);
				}
				else
				{
					this.human.Teleport(this.human.FindSafeTeleport(CityData.Instance.streetDirectory[Toolbox.Instance.Rand(0, CityData.Instance.streetDirectory.Count, false)], false, true), null, true, false);
				}
			}
		}
		this.lastUpdated = SessionData.Instance.gameTime;
		List<RetailItemPreset> list3 = new List<RetailItemPreset>();
		foreach (RetailItemPreset retailItemPreset in Enumerable.ToList<RetailItemPreset>(this.human.recentPurchases.Keys))
		{
			Dictionary<RetailItemPreset, float> recentPurchases = this.human.recentPurchases;
			RetailItemPreset retailItemPreset2 = retailItemPreset;
			recentPurchases[retailItemPreset2] += this.timeSinceLastUpdate * 2.08f;
			if (this.human.recentPurchases[retailItemPreset] >= 0f)
			{
				list3.Add(retailItemPreset);
			}
		}
		foreach (RetailItemPreset retailItemPreset3 in list3)
		{
			this.human.recentPurchases.Remove(retailItemPreset3);
		}
		this.tickActive = false;
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x0003ADC8 File Offset: 0x00038FC8
	public NewAIGoal CreateNewGoal(AIGoalPreset newPreset, float newTrigerTime, float newDuration, NewNode newPassedNode = null, Interactable newPassedInteractable = null, NewGameLocation newPassedGameLocation = null, GroupsController.SocialGroup newPassedGroup = null, MurderController.Murder newMurderRef = null, int newPassedVar = -2)
	{
		if (newPreset == null)
		{
			Game.LogError(this.human.GetCitizenName() + ": Trying to create an action without a preset!", 2);
			return null;
		}
		float newTraitMultiplier = 1f;
		if (newPreset.goalModifiers.Count > 0 && !this.human.TraitGoalTest(newPreset, out newTraitMultiplier))
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Creation of goal: " + newPreset.presetName + " was rejected due to trait tests", Actor.HumanDebug.actions);
			}
			return null;
		}
		NewAIGoal newAIGoal = new NewAIGoal(this, newPreset, newTrigerTime, newDuration, newPassedNode, newPassedInteractable, newPassedGameLocation, newPassedGroup, newMurderRef, newTraitMultiplier, newPassedVar);
		if (newAIGoal != null && newPreset.forcePriorityUpdateOnCreation && SessionData.Instance.startedGame)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Created new goal: " + newAIGoal.preset.presetName + ", attempting to force priority update...", Actor.HumanDebug.actions);
			}
			this.tickActive = false;
			this.AITick(true, false);
		}
		else if (newAIGoal == null && Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Creation of goal: " + newPreset.presetName + " was unsuccessful...", Actor.HumanDebug.actions);
		}
		return newAIGoal;
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x0003AEF0 File Offset: 0x000390F0
	public NewAIAction CreateNewAction(NewAIGoal newGoal, AIActionPreset newPreset, bool newInsertedAction = false, NewRoom newPassedRoom = null, Interactable newPassedInteractable = null, NewNode newForcedNode = null, GroupsController.SocialGroup newPassedGroup = null, List<InteractablePreset> newPassedAcquireItems = null, bool newForceRun = false, int newInsertedActionPriority = 3, string newDebug = "")
	{
		if (newPreset == null)
		{
			Game.LogError(this.human.GetCitizenName() + ": Trying to create an action without a preset!", 2);
			return null;
		}
		if (this.delayedActionsForTime.ContainsKey(newPreset))
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Rejecting new action " + newPreset.name + " as there is an action delay until: " + this.delayedActionsForTime[newPreset].ToString(), Actor.HumanDebug.actions);
			}
			return null;
		}
		return new NewAIAction(newGoal, newPreset, newInsertedAction, newPassedRoom, newPassedInteractable, newForcedNode, newPassedGroup, newPassedAcquireItems, newForceRun, newInsertedActionPriority, newDebug);
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x0003AF8C File Offset: 0x0003918C
	public void StatusStatUpdate()
	{
		this.human.AddNourishment(RoutineControls.Instance.hungerRate * -this.timeSinceLastUpdate);
		this.human.AddHydration(RoutineControls.Instance.thirstRate * -this.timeSinceLastUpdate);
		this.human.AddAlertness(RoutineControls.Instance.tirednessRate * -this.timeSinceLastUpdate);
		this.human.AddEnergy(RoutineControls.Instance.energyRate * -this.timeSinceLastUpdate);
		this.human.AddExcitement(RoutineControls.Instance.boredemRate * -this.timeSinceLastUpdate);
		this.human.AddChores(RoutineControls.Instance.choresRate * -this.timeSinceLastUpdate);
		this.human.AddHygiene(RoutineControls.Instance.hygeieneRate * -this.timeSinceLastUpdate);
		this.human.AddBladder(RoutineControls.Instance.bladderRate * -this.timeSinceLastUpdate);
		float num = 1f;
		if (this.ko || this.human.isAsleep)
		{
			num = 2.5f;
		}
		this.human.AddDrunk(RoutineControls.Instance.drunkRate * -this.timeSinceLastUpdate * num);
		this.human.AddBreath(RoutineControls.Instance.breathRate * -this.timeSinceLastUpdate * Math.Max(0.2f, this.human.breathRecoveryRate));
		if (this.human.poisoned > 0f)
		{
			bool enableKill = false;
			if (this.human.poisoner != null && this.human.poisoner.ai != null && this.human.ai != null && this.human.poisoner.ai.GetCurrentKillTarget() == this.human)
			{
				enableKill = true;
			}
			Transform bodyAnchor = this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.Head);
			this.human.RecieveDamage(this.human.poisoned * 13f * this.timeSinceLastUpdate, this.human.poisoner, bodyAnchor.position, bodyAnchor.forward, CitizenControls.Instance.vomitSpatter, null, SpatterSimulation.EraseMode.useDespawnTime, false, false, 0f, 0f, enableKill, false, 1f);
			this.human.AddPoisoned(RoutineControls.Instance.poisonRate * -this.timeSinceLastUpdate, null);
		}
		if (this.human.blinded > 0f)
		{
			this.human.AddBlinded(RoutineControls.Instance.blindedRate * -this.timeSinceLastUpdate);
		}
		if (this.human.currentRoom != null && this.human.currentRoom.gasLevel >= 0.2f)
		{
			this.human.AddNerve(-10f * this.timeSinceLastUpdate, null);
			Transform bodyAnchor2 = this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.Head);
			this.human.RecieveDamage(this.human.currentRoom.gasLevel * 10f * this.timeSinceLastUpdate, null, bodyAnchor2.position, bodyAnchor2.forward, CitizenControls.Instance.vomitSpatter, null, SpatterSimulation.EraseMode.useDespawnTime, false, false, 0f, 0f, false, false, 1f);
		}
		this.idleSound += RoutineControls.Instance.idleSoundRate * -this.timeSinceLastUpdate;
		if (this.human.drunk > 0.2f)
		{
			this.drunkIdleTimer += this.timeSinceLastUpdate * this.human.drunk;
			if (this.drunkIdleTimer >= 0.16f)
			{
				this.human.speechController.TriggerBark(SpeechController.Bark.drunkIdle);
				this.drunkIdleTimer = 0f;
			}
		}
		if (this.restrained)
		{
			this.restrainedIdleTimer += this.timeSinceLastUpdate;
			if (this.restrainedIdleTimer >= 0.18f)
			{
				this.human.speechController.TriggerBark(SpeechController.Bark.restrainedIdle);
				this.restrainedIdleTimer = 0f;
			}
		}
		if (this.human.currentHealth < this.human.maximumHealth)
		{
			float num2 = this.human.recoveryRate * this.timeSinceLastUpdate;
			if (this.human.isAsleep)
			{
				num2 *= 2f;
			}
			if (this.human.currentNerve > 0.15f && this.human.heat > 0.5f)
			{
				this.human.AddHealth(num2, true, false);
			}
		}
		if (this.human.currentNerve < this.human.maxNerve)
		{
			float num3 = this.human.recoveryRate * this.timeSinceLastUpdate;
			if (this.human.isAsleep)
			{
				num3 *= 2f;
			}
			if (this.human.lastScaredBy == null || (this.human.ai != null && !this.human.ai.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor == this.human.lastScaredBy)))
			{
				this.human.AddNerve(num3 * CitizenControls.Instance.nerveRecoveryRateMultiplier, null);
			}
		}
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x0003B4AD File Offset: 0x000396AD
	public void OnCompleteGoal(NewAIGoal completed)
	{
		this.AddDebugAction("Completed goal " + completed.name);
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x0003B4C8 File Offset: 0x000396C8
	public void SetDesiredTickRate(NewAIController.AITickRate newRate, bool forceUpdate = false)
	{
		if (this.disableTickRateUpdate)
		{
			return;
		}
		if (this.desiredTickRate != newRate || forceUpdate)
		{
			if (this.investigateLocation != null || this.human.seesIllegal.Count > 0)
			{
				this.desiredTickRate = NewAIController.AITickRate.veryHigh;
			}
			else
			{
				this.desiredTickRate = newRate;
			}
			this.UpdateTickRate(forceUpdate);
		}
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x0003B520 File Offset: 0x00039720
	public void UpdateTickRate(bool forceUpdate = false)
	{
		this.previousTickRate = this.tickRate;
		this.tickRate = this.desiredTickRate;
		if (this.currentAction != null && this.currentAction.preset.limitTickRate)
		{
			if (this.tickRate < this.currentAction.preset.minimumTickRate)
			{
				this.tickRate = this.currentAction.preset.minimumTickRate;
			}
			if (this.tickRate > this.currentAction.preset.maximumTickRate)
			{
				this.tickRate = this.currentAction.preset.maximumTickRate;
			}
		}
		if (this.previousTickRate != this.tickRate || forceUpdate)
		{
			if (this.previousTickRate == NewAIController.AITickRate.veryLow)
			{
				CitizenBehaviour.Instance.veryLowTickRate.Remove(this);
			}
			else if (this.previousTickRate == NewAIController.AITickRate.low)
			{
				CitizenBehaviour.Instance.lowTickRate.Remove(this);
			}
			else if (this.previousTickRate == NewAIController.AITickRate.medium)
			{
				CitizenBehaviour.Instance.mediumTickRate.Remove(this);
			}
			else if (this.previousTickRate == NewAIController.AITickRate.high)
			{
				CitizenBehaviour.Instance.highTickRate.Remove(this);
			}
			else if (this.previousTickRate == NewAIController.AITickRate.veryHigh)
			{
				CitizenBehaviour.Instance.veryHighTickRate.Remove(this);
			}
			if (this.tickRate == NewAIController.AITickRate.veryLow)
			{
				CitizenBehaviour.Instance.veryLowTickRate.Add(this);
				return;
			}
			if (this.tickRate == NewAIController.AITickRate.low)
			{
				CitizenBehaviour.Instance.lowTickRate.Add(this);
				return;
			}
			if (this.tickRate == NewAIController.AITickRate.medium)
			{
				CitizenBehaviour.Instance.mediumTickRate.Add(this);
				return;
			}
			if (this.tickRate == NewAIController.AITickRate.high)
			{
				CitizenBehaviour.Instance.highTickRate.Add(this);
				return;
			}
			if (this.tickRate == NewAIController.AITickRate.veryHigh)
			{
				CitizenBehaviour.Instance.veryHighTickRate.Add(this);
			}
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x0003B6DC File Offset: 0x000398DC
	public void FrequentUpdate()
	{
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		this.delta = Time.timeSinceLevelLoad - this.prevDelta;
		this.prevDelta = Time.timeSinceLevelLoad;
		if (SessionData.Instance.play && !Game.Instance.pauseAI)
		{
			this.visibleMovementAnimationLerpRequired = false;
			this.doIMove = false;
			this.facingActive = false;
			if (this.human.animationController.armsBoolAnimationState == CitizenAnimationController.ArmsBoolSate.armsOneShotUse)
			{
				this.human.animationController.oneShotUseReset -= this.delta;
				if (this.human.animationController.oneShotUseReset <= 0f)
				{
					this.human.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
				}
			}
			if (this.staticFromAnimation)
			{
				this.staticAnimationSafetyTimer -= this.delta;
				if (this.staticAnimationSafetyTimer <= 0f)
				{
					this.SetStaticFromAnimation(false);
				}
			}
			if (this.spooked > 0f)
			{
				this.AddSpooked(GameplayControls.Instance.loseSpookedRate * this.delta);
			}
			this.MovementSpeedUpdate();
			this.HearingUpdate();
			this.StatesUpdate();
			this.AttackUpdate();
			this.KOUpdate();
			if (this.restrained && SessionData.Instance.gameTime > this.restrainTime && MurderController.Instance.currentMurderer != this.human)
			{
				this.SetRestrained(false, 0f);
			}
			if (this.human.inConversation)
			{
				this.human.UpdateConversation();
			}
			for (int i = 0; i < this.queuedActions.Count; i++)
			{
				NewAIController.QueuedAction queuedAction = this.queuedActions[i];
				queuedAction.delay -= this.delta;
				if (queuedAction.delay <= 0f)
				{
					queuedAction.interactable.OnInteraction(queuedAction.actionSetting, this.human, false, 0f);
					this.queuedActions.RemoveAt(i);
					i--;
				}
			}
			if (!this.visibleMovementAnimationLerpRequired && !this.attackActive && !this.ko && !this.isRagdoll && !this.human.isDelayed && !this.human.isStunned && !this.restrained && !this.outOfBreath && !this.doIMove && !this.persuit && this.human.seesIllegal.Count <= 0 && !this.human.inConversation && !this.restrained && !this.facingActive && (this.currentAction == null || (this.currentAction.isAtLocation && !this.currentAction.preset.requiresForcedUpdate)) && this.queuedActions.Count <= 0 && !this.human.visible)
			{
				this.SetUpdateEnabled(false);
			}
		}
		else if (this.human != null)
		{
			this.human.neckTransform.rotation = this.lookingQuatLastFrame;
			this.ClampNeckRotation();
		}
		if (this.human.updateMeshList && this.human.visible)
		{
			this.human.UpdateMeshList();
		}
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x0003BA1C File Offset: 0x00039C1C
	private void MovementSpeedUpdate()
	{
		if (this.human.visible && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
		{
			if (this.human.currentNormalizedSpeed < this.human.desiredNormalizedSpeed)
			{
				this.human.currentNormalizedSpeed += CitizenControls.Instance.acceleration.Evaluate(this.human.currentNormalizedSpeed) * SessionData.Instance.currentTimeMultiplier * this.delta;
				this.human.currentNormalizedSpeed = Mathf.Min(this.human.currentNormalizedSpeed, this.human.desiredNormalizedSpeed);
				this.human.UpdateMovementSpeed();
				this.visibleMovementAnimationLerpRequired = true;
			}
			else if (this.human.currentNormalizedSpeed > this.human.desiredNormalizedSpeed)
			{
				this.human.currentNormalizedSpeed -= CitizenControls.Instance.decceleration.Evaluate(this.human.currentNormalizedSpeed) * SessionData.Instance.currentTimeMultiplier * this.delta;
				this.human.currentNormalizedSpeed = Mathf.Max(this.human.currentNormalizedSpeed, this.human.desiredNormalizedSpeed);
				this.human.UpdateMovementSpeed();
				this.visibleMovementAnimationLerpRequired = true;
			}
			if (this.capCollider != null)
			{
				if (this.restrained)
				{
					this.capCollider.radius = CitizenControls.Instance.capsuleMovementThickness.y;
				}
				else
				{
					this.capCollider.radius = Mathf.Lerp(CitizenControls.Instance.capsuleMovementThickness.x, CitizenControls.Instance.capsuleMovementThickness.y, this.human.currentNormalizedSpeed * 2f);
				}
			}
			if (this.human.animationController.mainAnimator != null)
			{
				float num = this.human.animationController.mainAnimator.GetLayerWeight(1);
				if (num < this.human.animationController.armsLayerDesiredWeight)
				{
					num = Mathf.Clamp01(num + this.delta / 0.15f);
					this.human.animationController.mainAnimator.SetLayerWeight(1, num);
					this.visibleMovementAnimationLerpRequired = true;
				}
				else if (num > this.human.animationController.armsLayerDesiredWeight)
				{
					num = Mathf.Clamp01(num - this.delta / 0.15f);
					this.human.animationController.mainAnimator.SetLayerWeight(1, num);
					this.visibleMovementAnimationLerpRequired = true;
				}
				float num2 = this.human.animationController.mainAnimator.GetLayerWeight(2);
				if (num2 < this.human.animationController.umbreallLayerDesiredWeight)
				{
					num2 = Mathf.Clamp01(num2 + this.delta / 0.5f);
					this.human.animationController.mainAnimator.SetLayerWeight(2, num2);
					this.visibleMovementAnimationLerpRequired = true;
					if (this.human.animationController.umbrellaCanopy != null)
					{
						this.human.animationController.umbrellaCanopy.localScale = Vector3.Lerp(new Vector3(0.15f, 0.15f, 3f), Vector3.one, num2);
					}
				}
				else if (num2 > this.human.animationController.umbreallLayerDesiredWeight)
				{
					num2 = Mathf.Clamp01(num2 - this.delta / 0.5f);
					this.human.animationController.mainAnimator.SetLayerWeight(2, num2);
					this.visibleMovementAnimationLerpRequired = true;
					if (this.human.animationController.umbrellaCanopy != null)
					{
						this.human.animationController.umbrellaCanopy.localScale = Vector3.Lerp(new Vector3(0.15f, 0.15f, 3f), Vector3.one, num2);
						if (num2 <= 0f)
						{
							Object.Destroy(this.human.animationController.spawnedUmbrella);
						}
					}
				}
			}
			if (this.human.currentRoom == Player.Instance.currentRoom)
			{
				if (this.blink >= 1f && this.currentExpression != null && this.currentExpression.allowBlinking)
				{
					this.blinkInProgress = true;
					this.blinkTimer = 0f;
					this.blink = 0f;
				}
				else if (this.eyesOpen > 0f && this.currentExpression != null && this.currentExpression.allowBlinking)
				{
					this.blink += this.delta * Toolbox.Instance.Rand(0.1f, 0.4f, false);
				}
				if ((this.expressionProgress < 1f || this.blinkInProgress) && this.currentExpression != null)
				{
					this.expressionProgress += this.delta;
					this.human.outfitController.rightEyebrow.localEulerAngles = Vector3.Lerp(this.human.outfitController.rightEyebrow.localEulerAngles, this.currentExpression.eyebrowsEuler, this.expressionProgress);
					this.human.outfitController.leftEyebrow.localEulerAngles = Vector3.Lerp(this.human.outfitController.rightEyebrow.localEulerAngles, -this.currentExpression.eyebrowsEuler, this.expressionProgress);
					float num3 = Mathf.Lerp(this.human.outfitController.rightEyebrow.localPosition.y, this.currentExpression.eyebrowsRaise, this.expressionProgress);
					this.human.outfitController.rightEyebrow.localPosition = new Vector3(0.045f, num3, 0f);
					this.human.outfitController.leftEyebrow.localPosition = new Vector3(-0.045f, num3, 0f);
					if (!this.blinkInProgress)
					{
						float num4 = Mathf.Lerp(this.human.outfitController.rightPupil.localScale.y, 0.02f * this.currentExpression.eyeHeightMultiplier * this.eyesOpen, this.expressionProgress);
						this.human.outfitController.rightPupil.localScale = new Vector3(0.02f, num4, 0.02f);
						this.human.outfitController.leftPupil.localScale = new Vector3(0.02f, num4, 0.02f);
						return;
					}
					this.blinkTimer += this.delta * 10f;
					if (this.blinkTimer <= 1f)
					{
						float num5 = Mathf.Lerp(this.human.outfitController.rightPupil.localScale.y, 0f, this.blinkTimer);
						this.human.outfitController.rightPupil.localScale = new Vector3(0.02f, num5, 0.02f);
						this.human.outfitController.leftPupil.localScale = new Vector3(0.02f, num5, 0.02f);
						this.human.outfitController.rightPupil.localPosition = new Vector3(0.045f, -(0.02f - this.human.outfitController.leftPupil.localScale.y), 0f);
						this.human.outfitController.leftPupil.localPosition = new Vector3(-0.045f, -(0.02f - this.human.outfitController.leftPupil.localScale.y), 0f);
						return;
					}
					if (this.blinkTimer <= 2f)
					{
						float num6 = Mathf.Lerp(this.human.outfitController.rightPupil.localScale.y, 0.02f * this.currentExpression.eyeHeightMultiplier * this.eyesOpen, this.blinkTimer - 1f);
						this.human.outfitController.rightPupil.localScale = new Vector3(0.02f, num6, 0.02f);
						this.human.outfitController.leftPupil.localScale = new Vector3(0.02f, num6, 0.02f);
						this.human.outfitController.rightPupil.localPosition = new Vector3(0.045f, -(0.02f - this.human.outfitController.leftPupil.localScale.y), 0f);
						this.human.outfitController.leftPupil.localPosition = new Vector3(-0.045f, -(0.02f - this.human.outfitController.leftPupil.localScale.y), 0f);
						return;
					}
					this.human.outfitController.rightPupil.localPosition = new Vector3(0.045f, 0f, 0f);
					this.human.outfitController.leftPupil.localPosition = new Vector3(-0.045f, 0f, 0f);
					this.blinkInProgress = false;
					this.blinkTimer = 0f;
					return;
				}
			}
		}
		else
		{
			if (this.human.currentNormalizedSpeed != this.human.desiredNormalizedSpeed)
			{
				this.human.currentNormalizedSpeed = this.human.desiredNormalizedSpeed;
				this.human.currentMovementSpeed = this.human.movementRunSpeed * this.human.currentNormalizedSpeed;
				this.human.UpdateMovementSpeed();
				this.capCollider.radius = Mathf.Lerp(CitizenControls.Instance.capsuleMovementThickness.x, CitizenControls.Instance.capsuleMovementThickness.y, this.human.currentNormalizedSpeed * 2f);
			}
			if (this.human.animationController.mainAnimator != null && this.human.animationController.mainAnimator.GetLayerWeight(1) != this.human.animationController.armsLayerDesiredWeight)
			{
				this.human.animationController.mainAnimator.SetLayerWeight(1, this.human.animationController.armsLayerDesiredWeight);
			}
		}
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x0003C440 File Offset: 0x0003A640
	private void HearingUpdate()
	{
		if (this.hearsIllegal > 0f)
		{
			this.hearsIllegal -= this.delta * CitizenControls.Instance.hearingForgetThreshold;
			if (this.human.inConversation)
			{
				this.human.currentConversation.EndConversation();
			}
			if (this.hearsIllegal <= 0f)
			{
				this.hearsIllegal = 0f;
				this.hearTarget = null;
				if (this.audioFocusAction != null)
				{
					this.audioFocusAction.Complete();
					if (this.reactionState == NewAIController.ReactionState.investigatingSound)
					{
						this.SetReactionState(NewAIController.ReactionState.none);
					}
				}
			}
		}
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x0003C4D8 File Offset: 0x0003A6D8
	private void StatesUpdate()
	{
		if (this.human.isDead || this.human.isStunned)
		{
			if (this.attackActive)
			{
				Game.Log(this.human.name + " Abort attack: Is dead/stunned", 2);
				this.OnAbortAttack();
			}
			if (this.reactionState != NewAIController.ReactionState.none)
			{
				this.SetReactionState(NewAIController.ReactionState.none);
			}
			if (this.human.inConversation)
			{
				this.human.currentConversation.EndConversation();
			}
			this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
			return;
		}
		this.PersuitUpdate();
		this.MovementUpdate();
		this.FacingUpdate();
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x0003C574 File Offset: 0x0003A774
	private void PersuitUpdate()
	{
		if (this.human.seesIllegal.Count > 0 || this.persuit)
		{
			if (this.persuitUpdateTimer > 0f)
			{
				this.persuitUpdateTimer -= this.delta;
				return;
			}
			if (Game.Instance.collectDebugData)
			{
				Actor actor = this.human;
				string[] array = new string[8];
				array[0] = "Persuit update: ";
				array[1] = this.human.seesIllegal.Count.ToString();
				array[2] = " target(s) seen with persuit active: ";
				array[3] = this.persuit.ToString();
				array[4] = " for target ";
				int num = 5;
				Actor actor2 = this.persuitTarget;
				array[num] = ((actor2 != null) ? actor2.ToString() : null);
				array[6] = " at escalation level ";
				array[7] = this.human.escalationLevel.ToString();
				actor.SelectedDebug(string.Concat(array), Actor.HumanDebug.updates);
			}
			this.persuitUpdateTimer = 0.33f;
			this.human.SightingCheck(GameplayControls.Instance.citizenFOV, false);
			if (this.human.escalationLevel >= 2)
			{
				foreach (Actor actor3 in new List<Actor>(this.human.seesIllegal.Keys))
				{
					if (this.human.seesIllegal.ContainsKey(actor3))
					{
						float num2 = this.human.seesIllegal[actor3];
						if (Game.Instance.collectDebugData)
						{
							Actor actor4 = this.human;
							string text = "Sees illegal value for ";
							Actor actor5 = actor3;
							actor4.SelectedDebug(text + ((actor5 != null) ? actor5.ToString() : null) + ": " + num2.ToString(), Actor.HumanDebug.sight);
						}
						if (num2 >= 1f)
						{
							if (this.currentAction != null && this.currentAction.preset.completeOnSeeIllegal)
							{
								this.currentAction.progress = 1f;
								this.currentAction.ImmediateComplete();
							}
							if (Game.Instance.collectDebugData)
							{
								Actor actor6 = this.human;
								string text2 = "Found a valid persuit target: ";
								Actor actor7 = actor3;
								actor6.SelectedDebug(text2 + ((actor7 != null) ? actor7.ToString() : null), Actor.HumanDebug.sight);
							}
							this.SetPersuit(true);
							this.SetPersueTarget(actor3);
							this.persuitChaseLogicUses = 0.5f;
							this.SetReactionState(NewAIController.ReactionState.persuing);
							if (!actor3.isPlayer)
							{
								break;
							}
							InterfaceControls.Instance.seenJuice.Nudge(new Vector2(1.3f, 1.3f), new Vector2(2f, 2f), true, true, true);
							if (Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.illegalActionActive)
							{
								StatusController.Instance.ConfirmFine(Player.Instance.currentGameLocation.thisAsAddress, null, StatusController.CrimeType.trespassing);
								break;
							}
							break;
						}
						else if (num2 >= 0.75f)
						{
							if (this.currentAction != null && this.currentAction.preset.completeOnSeeIllegal)
							{
								this.currentAction.progress = 1f;
								this.currentAction.ImmediateComplete();
							}
							if (actor3.isPlayer)
							{
								InterfaceControls.Instance.seenJuice.Nudge(new Vector2(1.3f, 1.3f), new Vector2(2f, 2f), true, true, true);
							}
							if (Game.Instance.collectDebugData)
							{
								this.human.SelectedDebug(num2.ToString() + " Investigate target...", Actor.HumanDebug.sight);
							}
							this.Investigate(actor3.currentNode, actor3.transform.position, actor3, NewAIController.ReactionState.investigatingSight, CitizenControls.Instance.sightingMinInvestigationTimeMP * 0.8f, 0, false, 1f, null);
						}
					}
				}
			}
			if (this.persuit && this.persuitTarget != null)
			{
				if (this.human.seenIllegalThisCheck.Contains(this.persuitTarget) && !this.persuitTarget.isStunned && !this.persuitTarget.isDead)
				{
					if (Game.Instance.collectDebugData)
					{
						Actor actor8 = this.human;
						string text3 = "Seen persuit target ";
						Actor actor9 = this.persuitTarget;
						actor8.SelectedDebug(text3 + ((actor9 != null) ? actor9.ToString() : null) + " this frame...", Actor.HumanDebug.sight);
					}
					this.SetSeesOnPersuit(true);
				}
				else
				{
					if (Game.Instance.collectDebugData)
					{
						Actor actor10 = this.human;
						string text4 = "Not seen persuit target ";
						Actor actor11 = this.persuitTarget;
						actor10.SelectedDebug(text4 + ((actor11 != null) ? actor11.ToString() : null) + " this frame...", Actor.HumanDebug.sight);
					}
					this.SetSeesOnPersuit(false);
				}
				if (this.seesOnPersuit)
				{
					this.chaseLogic.UpdateLastSeen();
					NewAIController.ReactionState newReactionState = NewAIController.ReactionState.investigatingSight;
					float minimumInvestiationTimeMP = CitizenControls.Instance.sightingMinInvestigationTimeMP;
					if (this.persuit)
					{
						newReactionState = NewAIController.ReactionState.persuing;
						minimumInvestiationTimeMP = CitizenControls.Instance.persuitMinInvestigationTimeMP;
					}
					this.Investigate(this.persuitTarget.currentNode, this.persuitTarget.transform.position, this.persuitTarget, newReactionState, minimumInvestiationTimeMP, 0, true, 1f, null);
					this.AddTrackedTarget(this.persuitTarget);
					if (this.human.escalationLevel >= 2)
					{
						this.persuitChaseLogicUses += CitizenControls.Instance.persuitChaseLogicAdditionPerSecond * this.delta;
						this.persuitChaseLogicUses = Mathf.Clamp(this.persuitChaseLogicUses, 0f, (float)CitizenControls.Instance.maxChaseLogic);
						return;
					}
				}
				else if (this.currentAction != null && !this.currentAction.isAtLocation)
				{
					if (this.persuitChaseLogicUses > 0f)
					{
						this.persuitChaseLogicUses += 0.1f;
						return;
					}
				}
				else if (this.persuitChaseLogicUses >= 1f)
				{
					this.chaseLogic.GenerateProjectedNode();
					this.Investigate(this.chaseLogic.projectedNode, this.chaseLogic.projectedPosition, this.persuitTarget, NewAIController.ReactionState.persuing, CitizenControls.Instance.persuitMinInvestigationTimeMP, 0, true, 1f, null);
					this.persuitChaseLogicUses = 0f;
					if (Game.Instance.collectDebugData)
					{
						Actor actor12 = this.human;
						string text5 = "Use a persuit logic (";
						string text6 = this.persuitChaseLogicUses.ToString();
						string text7 = " left), investigate: ";
						Vector3 projectedPosition = this.chaseLogic.projectedPosition;
						actor12.SelectedDebug(text5 + text6 + text7 + projectedPosition.ToString(), Actor.HumanDebug.sight);
						return;
					}
				}
				else if (!this.human.seesIllegal.ContainsKey(this.persuitTarget))
				{
					this.CancelPersue();
					if (this.human.escalationLevel >= 2)
					{
						this.SetReactionState(NewAIController.ReactionState.searching);
						return;
					}
					this.SetReactionState(NewAIController.ReactionState.none);
					return;
				}
			}
			else if (this.persuit)
			{
				this.CancelPersue();
			}
		}
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x0003CBFC File Offset: 0x0003ADFC
	private void MovementUpdate()
	{
		this.doIMove = false;
		if (this.currentAction != null && this.currentAction.isActive)
		{
			if (!this.ko && !this.restrained && !this.outOfBreath && !this.staticFromAnimation)
			{
				if (!this.currentAction.isAtLocation)
				{
					if (this.human.interactingWith == null && (!this.human.inConversation || !this.human.currentConversation.tree.stopMovement))
					{
						if (this.doorCheck)
						{
							if (this.currentAction.path != null && this.pathCursor < this.currentAction.path.accessList.Count)
							{
								this.doIMove = true;
								if (this.debugMovement)
								{
									Game.Log("AI: " + this.human.name + " Moving.", 2);
								}
							}
							else if (base.transform.position != this.currentDestinationPositon)
							{
								this.doIMove = true;
								if (this.debugMovement)
								{
									Game.Log("AI: " + this.human.name + " Moving.", 2);
								}
							}
							else if (this.debugMovement)
							{
								string[] array = new string[10];
								array[0] = "AI: ";
								array[1] = this.human.name;
								array[2] = " Not moving: Path cursor ";
								array[3] = this.pathCursor.ToString();
								array[4] = "/";
								array[5] = this.currentAction.path.accessList.Count.ToString();
								array[6] = " dest pos: ";
								array[7] = base.transform.position.ToString();
								array[8] = "/";
								int num = 9;
								Vector3 position = this.currentDestinationPositon;
								array[num] = position.ToString();
								Game.Log(string.Concat(array), 2);
							}
						}
						else if (this.currentDestinationNode == this.human.currentNode)
						{
							this.doIMove = true;
							if (this.debugMovement)
							{
								string text = "AI: ";
								string name = this.human.name;
								string text2 = " Moving. Door check fail but destination is the current node ";
								Vector3 position = this.currentDestinationNode.position;
								Game.Log(text + name + text2 + position.ToString(), 2);
							}
						}
						else if (this.debugMovement)
						{
							Game.Log("AI: " + this.human.name + " Not moving: Door check is false", 2);
						}
					}
					else if (this.debugMovement)
					{
						Game.Log("AI: Not moving: Interacting", 2);
					}
				}
				else if (this.debugMovement)
				{
					Game.Log("AI: " + this.human.name + " Not moving: Is at location: " + this.currentAction.isAtLocation.ToString(), 2);
				}
			}
			else if (this.debugMovement)
			{
				Game.Log(string.Concat(new string[]
				{
					"AI: ",
					this.human.name,
					" Not moving: KO: ",
					this.ko.ToString(),
					" restrained: ",
					this.restrained.ToString(),
					" staticAnim: ",
					this.staticFromAnimation.ToString(),
					" out of breath: ",
					this.outOfBreath.ToString()
				}), 2);
			}
		}
		else if (this.debugMovement)
		{
			string[] array2 = new string[5];
			array2[0] = "AI: ";
			array2[1] = this.human.name;
			array2[2] = " Not moving: Current action is ";
			int num2 = 3;
			NewAIAction newAIAction = this.currentAction;
			array2[num2] = ((newAIAction != null) ? newAIAction.ToString() : null);
			array2[4] = " or inactive...";
			Game.Log(string.Concat(array2), 2);
		}
		if (this.doIMove)
		{
			if ((this.currentAction == null || (!this.currentAction.preset.forceRun && !this.currentAction.forceRun)) && (this.inCombat || this.reactionState != NewAIController.ReactionState.none || this.spookCounter < 1 || this.human.maxNerve >= 0.7f || this.spooked < 0.5f))
			{
				if (this.currentAction.preset.runIfSeesPlayer)
				{
					if (this.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor != null && item.actor.isPlayer))
					{
						goto IL_485;
					}
				}
				if (this.human.currentGameLocation != null && this.human.currentGameLocation.isOutside && SessionData.Instance.currentRain > 0.1f && !this.human.ownsUmbrella && !this.human.likesTheRain && !this.human.isHomeless)
				{
					this.human.SetDesiredSpeed(Human.MovementSpeed.running);
					goto IL_605;
				}
				if (this.currentGoal != null && this.currentGoal.preset.useTiming && this.currentGoal.preset.runIfLate && this.human.currentGameLocation != this.currentGoal.gameLocation && !this.currentAction.preset.useInvestigationUrgency)
				{
					float num3 = this.currentGoal.triggerTime - this.currentGoal.travelTime - SessionData.Instance.gameTime;
					if (num3 >= 0f && num3 <= this.currentGoal.preset.earlyTimingWindow)
					{
						this.human.SetDesiredSpeed(Human.MovementSpeed.running);
						goto IL_605;
					}
					this.human.SetDesiredSpeed(Human.MovementSpeed.walking);
					goto IL_605;
				}
				else
				{
					if (!this.currentAction.preset.useInvestigationUrgency)
					{
						this.human.SetDesiredSpeed(Human.MovementSpeed.walking);
						goto IL_605;
					}
					if (this.investigationUrgency == NewAIController.InvestigationUrgency.run)
					{
						this.human.SetDesiredSpeed(Human.MovementSpeed.running);
						goto IL_605;
					}
					this.human.SetDesiredSpeed(Human.MovementSpeed.walking);
					goto IL_605;
				}
			}
			IL_485:
			this.human.SetDesiredSpeed(Human.MovementSpeed.running);
			IL_605:
			if (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive)
			{
				this.movementAmount = this.human.currentMovementSpeed * SessionData.Instance.currentTimeMultiplier * GameplayControls.Instance.preSimSpeedMultiplier * this.delta;
			}
			else
			{
				this.movementAmount = this.human.currentMovementSpeed * SessionData.Instance.currentTimeMultiplier * this.delta;
				if (!this.human.visible)
				{
					if (this.human.currentTile != null && this.human.currentTile.isStairwell)
					{
						this.movementAmount *= CitizenControls.Instance.offscreenStairwellMovementSpeedMultiplier;
					}
					else
					{
						this.movementAmount *= CitizenControls.Instance.offscreenMovementSpeedMultiplier;
					}
				}
			}
			this.distanceToNext = Vector3.Distance(base.transform.position, this.currentDestinationPositon);
			if (this.distanceToNext <= 0f)
			{
				if (this.doorCheck)
				{
					this.ReachNewPathNode("Node Reached", true);
				}
				else if (!this.staticFromAnimation)
				{
					this.SetStaticFromAnimation(true);
				}
				if (this.currentAction == null)
				{
					this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
				}
				else if (this.currentAction.path == null)
				{
					this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
				}
				else if (this.currentAction.path.accessList == null)
				{
					this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
				}
			}
			while (this.movementAmount > 0f && this.distanceToNext > 0f)
			{
				if (this.currentAction != null && this.currentAction.path != null && this.currentDestinationNode == null && this.pathCursor < this.currentAction.path.accessList.Count)
				{
					NewNode nodeAhead = this.currentAction.path.GetNodeAhead(this.pathCursor);
					this.SetDestinationNode(nodeAhead, true);
				}
				this.distanceToNext = Vector3.Distance(base.transform.position, this.currentDestinationPositon);
				if (!this.human.animationController.mainAnimator.enabled)
				{
					this.footStepDistanceCounter += this.movementAmount;
					if (this.footStepDistanceCounter >= CitizenControls.Instance.citizenFootstepDistance * AudioDebugging.Instance.citizenFootstepDistanceMultiplier)
					{
						this.footStepDistanceCounter -= CitizenControls.Instance.citizenFootstepDistance * AudioDebugging.Instance.citizenFootstepDistanceMultiplier;
						this.rightFootNext = !this.rightFootNext;
						this.human.OnFootstep(this.rightFootNext);
					}
				}
				if (this.distanceToNext > this.movementAmount)
				{
					if (this.human.visible && SessionData.Instance.currentTimeSpeed == SessionData.TimeSpeed.normal)
					{
						Quaternion quaternion = this.lastMovementRotation;
						Vector3 normalized = (this.currentDestinationPositon - base.transform.position).normalized;
						if (normalized != Vector3.zero)
						{
							quaternion = Quaternion.LookRotation(normalized, Vector3.up);
						}
						if (quaternion != this.lastMovementRotation)
						{
							float num4 = 0f;
							float num5 = this.GetRotationalLerpValue(this.lastMovementRotation, quaternion, CitizenControls.Instance.citizenRotationalMovementSpeed + this.human.currentMovementSpeed * 2f, out num4) * SessionData.Instance.currentTimeMultiplier * this.delta;
							if (Mathf.Abs(num4) < 0.5f)
							{
								this.lastMovementRotation = quaternion;
								base.transform.position = Vector3.MoveTowards(base.transform.position, this.currentDestinationPositon, this.movementAmount);
							}
							else
							{
								this.lastMovementRotation = Quaternion.Slerp(this.lastMovementRotation, quaternion, num5);
								Vector3 vector = this.lastMovementRotation * Vector3.forward * this.movementAmount;
								vector = base.transform.position + vector;
								if (Vector3.Distance(vector, this.currentDestinationPositon) < this.distanceToNext)
								{
									Vector3 vector2 = Vector3.MoveTowards(base.transform.position, this.currentDestinationPositon, this.movementAmount);
									vector.y = vector2.y;
									base.transform.position = vector;
								}
								else
								{
									base.transform.position = Vector3.MoveTowards(base.transform.position, this.currentDestinationPositon, this.movementAmount);
								}
							}
						}
						else
						{
							base.transform.position = Vector3.MoveTowards(base.transform.position, this.currentDestinationPositon, this.movementAmount);
						}
					}
					else
					{
						base.transform.position = Vector3.MoveTowards(base.transform.position, this.currentDestinationPositon, this.movementAmount);
					}
					this.movementAmount = 0f;
				}
				else
				{
					base.transform.position = this.currentDestinationPositon;
					this.movementAmount -= this.distanceToNext;
					if (this.doorCheck)
					{
						this.ReachNewPathNode("Node Reached", true);
					}
					else if (!this.staticFromAnimation)
					{
						this.SetStaticFromAnimation(true);
					}
					if (this.currentAction == null)
					{
						this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
						break;
					}
					if (this.currentAction.path == null)
					{
						this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
						break;
					}
					if (this.currentAction.path.accessList == null)
					{
						this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
						break;
					}
					break;
				}
			}
			if (this.human.drunk > 0f && this.human.visible && !this.isTripping && !this.human.isDead && !this.human.isStunned && !this.human.isAsleep)
			{
				if (this.drunkTripCheckTimer <= 0f)
				{
					float num6 = this.human.currentNormalizedSpeed * this.human.drunk * CitizenControls.Instance.drunkFallChance;
					if (Toolbox.Instance.Rand(0f, 1f, false) < num6)
					{
						Game.Log("Tripped up with chance of " + num6.ToString(), 2);
						this.human.speechController.TriggerBark(SpeechController.Bark.fallOffChair);
						this.Trip();
					}
					this.drunkTripCheckTimer = 1f;
				}
				else
				{
					this.drunkTripCheckTimer -= this.delta;
				}
			}
			if (this.human.isRunning)
			{
				this.human.AddBreath(-0.1f * this.delta);
				return;
			}
		}
		else
		{
			this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
			if (this.human.interactingWith != null && this.human.interactingWith.parentTransform != null && (this.currentAction == null || !this.currentAction.isAtLocation || this.currentAction.preset.facePlayerWhileTalkingTo))
			{
				this.faceTransform = this.human.interactingWith.parentTransform;
			}
			if (this.human.inConversation && this.human.currentConversation.tree.stopMovement && (this.currentAction == null || !this.currentAction.isAtLocation))
			{
				if (this.human.currentConversation.currentlyTalking != null && this.human != this.human.currentConversation.currentlyTalking)
				{
					this.faceTransform = this.human.currentConversation.currentlyTalking.transform;
				}
				else if (this.human.currentConversation.currentlyTalking == this.human && this.human.currentConversation.currentlyTalkingTo != null)
				{
					this.faceTransform = this.human.currentConversation.currentlyTalkingTo.transform;
				}
				else if (this.human.currentConversation.previouslyTalking != null && this.human != this.human.currentConversation.previouslyTalking)
				{
					this.faceTransform = this.human.currentConversation.previouslyTalking.transform;
				}
				else if (this.human.currentConversation.participantA != null && this.human.currentConversation.participantA != this.human)
				{
					this.faceTransform = this.human.currentConversation.participantA.transform;
				}
				else if (this.human.currentConversation.participantB != null && this.human.currentConversation.participantB != this.human)
				{
					this.faceTransform = this.human.currentConversation.participantB.transform;
				}
				else if (this.human.currentConversation.participantC != null && this.human.currentConversation.participantC != this.human)
				{
					this.faceTransform = this.human.currentConversation.participantC.transform;
				}
				else if (this.human.currentConversation.participantD != null && this.human.currentConversation.participantD != this.human)
				{
					this.faceTransform = this.human.currentConversation.participantD.transform;
				}
			}
			if (!this.doorCheck)
			{
				if (this.doorCheckProcessTimer <= 0)
				{
					this.DoorCheckProcess();
					this.AITick(false, false);
				}
				else
				{
					this.doorCheckProcessTimer--;
				}
			}
			if (this.currentAction != null && this.currentAction.preset == RoutineControls.Instance.bargeDoor)
			{
				Game.Log("Gameplay: AI " + this.human.name + " is barging door...", 2);
				this.bargeTimer += this.delta;
				if (this.bargeTimer >= 1.5f && this.currentAction.passedInteractable != null)
				{
					NewDoor newDoor = this.currentAction.passedInteractable.objectRef as NewDoor;
					newDoor.Barge(this.human);
					this.bargeTimer = 0f;
					if (newDoor.wall.currentDoorStrength <= 0f)
					{
						Game.Log("Gameplay: AI " + this.human.name + " barge door complete!", 2);
						this.bargeTimer = 0f;
						this.currentAction.repeat = false;
						this.currentAction.Complete();
					}
				}
				if (this.currentAction != null && this.currentAction.passedInteractable == null)
				{
					this.bargeTimer = 0f;
					this.currentAction.repeat = false;
					this.currentAction.Complete();
				}
			}
		}
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x0003DCE4 File Offset: 0x0003BEE4
	private float GetRotationalLerpValue(Quaternion originalRotation, Quaternion targetRotation, float multiplier, out float angleBetween)
	{
		angleBetween = Quaternion.Angle(originalRotation, targetRotation);
		float num = angleBetween / 180f;
		float num2 = 1f - Mathf.Clamp01(this.distanceToNext / 1.8f);
		return (num + num2) * 0.5f * multiplier;
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x0003DD28 File Offset: 0x0003BF28
	private void FacingUpdate()
	{
		if (this.restrained)
		{
			return;
		}
		if (this.faceTransform != null)
		{
			Vector3 normalized = (this.faceTransform.position + this.faceTransformOffset - base.transform.position).normalized;
			normalized.y = 0f;
			if (normalized != Vector3.zero)
			{
				this.facingQuat = Quaternion.LookRotation(normalized, Vector3.up);
			}
		}
		if (this.human.visible)
		{
			if (this.facingQuat != base.transform.rotation)
			{
				this.facingActive = true;
				float num = 0f;
				float num2 = this.GetRotationalLerpValue(base.transform.rotation, this.facingQuat, CitizenControls.Instance.citizenFaceSpeed, out num) * SessionData.Instance.currentTimeMultiplier * this.delta;
				if (Mathf.Abs(num) <= 0.33f)
				{
					base.transform.rotation = this.facingQuat;
				}
				else
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.facingQuat, num2);
				}
			}
			Vector3 vector = this.human.transform.forward;
			if (this.human.interactingWith != null)
			{
				vector = this.lookAtTransform.position - this.human.interactingWith.wPos;
				if (vector != Vector3.zero)
				{
					this.lookingQuatCurrent = Quaternion.LookRotation(vector, Vector3.up);
				}
				if ((this.currentTrackTarget == null || !this.currentTrackTarget.priorityTarget) && Mathf.Abs(Vector3.Angle(vector, base.transform.forward)) >= GameplayControls.Instance.citizenFOV * 0.6f)
				{
					this.SetTrackTarget(null);
					this.lookingQuatCurrent = Quaternion.LookRotation(this.human.transform.forward, Vector3.up);
					this.UpdateTrackedTargets();
				}
			}
			else if (this.lookAtTransform != null)
			{
				vector = this.lookAtTransform.position - this.human.lookAtThisTransform.position;
				if (vector != Vector3.zero)
				{
					this.lookingQuatCurrent = Quaternion.LookRotation(vector, Vector3.up);
				}
				if ((this.currentTrackTarget == null || !this.currentTrackTarget.priorityTarget) && Mathf.Abs(Vector3.Angle(vector, base.transform.forward)) >= GameplayControls.Instance.citizenFOV * 0.6f)
				{
					this.SetTrackTarget(null);
					this.lookingQuatCurrent = Quaternion.LookRotation(this.human.transform.forward, Vector3.up);
					this.UpdateTrackedTargets();
				}
			}
			else if (this.currentAction != null && this.currentAction.preset.lookAround)
			{
				this.lookAroundTimer += Time.deltaTime * Toolbox.Instance.Rand(0.75f, 1.25f, false);
				if (this.lookAroundTimer > 1.6f)
				{
					this.lookAroundPosition = new Vector3(Toolbox.Instance.Rand(-1.2f, 1.2f, false), Toolbox.Instance.Rand(1.45f, 1.6f, false), 1.5f);
					this.lookAroundTimer = 0f;
				}
				vector = this.human.transform.TransformPoint(this.lookAroundPosition) - this.human.lookAtThisTransform.position;
				this.lookingQuatCurrent = Quaternion.LookRotation(vector, Vector3.up);
			}
			else
			{
				this.lookingQuatCurrent = this.human.neckTransform.rotation;
			}
			float num3 = this.delta * Mathf.Lerp(CitizenControls.Instance.citizenLookAtSpeed.x, CitizenControls.Instance.citizenLookAtSpeed.y, this.lookAtTransformRank);
			this.human.neckTransform.rotation = Quaternion.Slerp(this.lookingQuatLastFrame, this.lookingQuatCurrent, num3);
			this.ClampNeckRotation();
			this.lookingQuatLastFrame = this.human.neckTransform.rotation;
			Vector3 vector2 = vector / 100f;
			vector2 = this.human.outfitController.pupilParentOffset + new Vector3(Mathf.Clamp(vector2.x, -0.0135f, 0.0135f), Mathf.Clamp(vector2.y, -0.008f, 0.008f), 0f);
			if (this.human.outfitController.pupilParent != null)
			{
				this.human.outfitController.pupilParent.localPosition = vector2;
				return;
			}
		}
		else if (this.facingQuat != base.transform.rotation)
		{
			base.transform.rotation = this.facingQuat;
		}
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x0003E204 File Offset: 0x0003C404
	private void AttackUpdate()
	{
		if (this.attackActive && this.attackTarget != null)
		{
			this.attackActiveLength += this.delta;
			this.human.animationController.umbreallLayerDesiredWeight = 0f;
			this.SetFacingPosition(this.attackTarget.transform.position);
			float num = Vector3.Distance(this.attackTarget.transform.position, base.transform.position);
			if (!this.throwActive && (num > this.weaponRangeMax || num < this.currentWeaponPreset.minimumRange))
			{
				Game.Log(string.Concat(new string[]
				{
					this.human.name,
					" Abort attack: Out of range ",
					num.ToString(),
					" (",
					this.weaponRangeMax.ToString(),
					"/",
					this.currentWeaponPreset.minimumRange.ToString(),
					")"
				}), 2);
				this.OnAbortAttack();
			}
			else if (this.throwActive)
			{
				this.attackProgress = Mathf.Clamp01(this.attackActiveLength / 1.25f);
				this.human.animationController.mainAnimator.SetLayerWeight(1, 1f);
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Set current arms layer weight: 1 (" + this.human.animationController.armsLayerDesiredWeight.ToString() + ")", Actor.HumanDebug.misc);
				}
				if (!this.damageColliderCreated && this.attackProgress >= 0.33f)
				{
					if (this.throwItem != null)
					{
						if (this.human.currentConsumables.Count > 0)
						{
							this.human.currentConsumables.RemoveAt(0);
						}
						else if (this.human.trash.Count > 0)
						{
							this.human.trash.RemoveAt(0);
						}
						Transform bodyAnchor = this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandRight);
						Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(this.throwItem, this.human, this.human, null, bodyAnchor.transform.position, Vector2.zero, null, null, "");
						float num2 = 30f;
						if (this.human.descriptors.build == Descriptors.BuildType.skinny)
						{
							num2 = 20f;
						}
						else if (this.human.descriptors.build == Descriptors.BuildType.overweight)
						{
							num2 = 40f;
						}
						else if (this.human.descriptors.build == Descriptors.BuildType.muscular)
						{
							num2 = 60f;
						}
						interactable.ForcePhysicsActive(false, true, base.transform.forward * num2, 2, false);
						this.throwItem = null;
						this.DespawnRightItem();
						this.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
					}
					this.damageColliderCreated = true;
				}
				if (this.attackProgress >= 1f)
				{
					this.attackActive = false;
					this.attackProgress = 0f;
					this.damageColliderCreated = false;
					this.ejectBrassCreated = false;
					this.throwActive = false;
					this.throwDelay = Toolbox.Instance.Rand(4f, 7f, false);
					this.OnAttackComplete();
				}
			}
			else if (this.combatMode == 0)
			{
				AnimatorStateInfo currentAnimatorStateInfo = this.human.animationController.mainAnimator.GetCurrentAnimatorStateInfo(1);
				if (currentAnimatorStateInfo.IsTag("Punch") || currentAnimatorStateInfo.IsTag("Kick"))
				{
					this.human.animationController.mainAnimator.SetLayerWeight(1, 1f);
					this.attackProgress = currentAnimatorStateInfo.normalizedTime % 1f;
					if (this.attackTarget.isPlayer && FirstPersonItemController.Instance.currentItem != null && FirstPersonItemController.Instance.currentItem.name.ToLower() == "fists" && this.activeAttackBar == null)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.attackBar, InterfaceControls.Instance.speechBubbleParent);
						this.activeAttackBar = gameObject.GetComponent<AttackBarController>();
						this.activeAttackBar.Setup(this);
					}
					if (this.attackProgress >= this.currentWeaponPreset.attackTriggerPoint && !this.damageColliderCreated)
					{
						GameObject gameObject2;
						if (currentAnimatorStateInfo.IsTag("Kick"))
						{
							gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.damageCollider, this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.LowerLegRight));
						}
						else
						{
							gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.damageCollider, this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.LowerArmRight));
						}
						this.damageCollider = gameObject2.GetComponent<DamageColliderController>();
						this.damageCollider.Setup(this.human, this.attackTarget, this.weaponDamage, this.GetCurrentKillTarget(), this.currentWeaponPreset);
						if (this.activeAttackBar != null)
						{
							this.activeAttackBar.removeHit = true;
						}
						if (this.currentWeaponPreset.fireEvent != null)
						{
							AudioController.Instance.PlayWorldOneShot(this.currentWeaponPreset.fireEvent, this.human, this.human.currentNode, this.damageCollider.transform.position, null, null, 1f, null, false, null, false);
						}
						this.damageColliderCreated = true;
					}
					else if (this.attackProgress > this.currentWeaponPreset.attackRemovePoint && this.damageColliderCreated)
					{
						this.OnAttackComplete();
						if (this.damageCollider != null)
						{
							Object.Destroy(this.damageCollider.gameObject);
						}
					}
				}
				else if (this.damageColliderCreated && this.attackActiveLength > this.weaponRefire)
				{
					this.attackActive = false;
					this.throwActive = false;
					this.attackProgress = 0f;
					this.damageColliderCreated = false;
					this.ejectBrassCreated = false;
					if (this.damageCollider != null)
					{
						Object.Destroy(this.damageCollider.gameObject);
					}
					this.OnAttackComplete();
				}
			}
			else if (this.combatMode >= 1 && this.combatMode <= 3)
			{
				this.attackProgress = Mathf.Clamp01(this.attackActiveLength / this.weaponRefire);
				this.human.animationController.mainAnimator.SetLayerWeight(1, 1f);
				if (!this.damageColliderCreated && this.attackProgress >= this.currentWeaponPreset.attackTriggerPoint && this.spawnedRightItem != null)
				{
					Vector3 muzzlePoint = this.spawnedRightItem.transform.TransformPoint(this.currentWeaponPreset.muzzleOffset);
					Vector3 ejectBrassPoint = Vector3.zero;
					bool ejectBrass = false;
					if (this.currentWeaponPreset.ejectBrassSetting == MurderWeaponPreset.EjectBrass.onFire)
					{
						ejectBrassPoint = this.spawnedRightItem.transform.TransformPoint(this.currentWeaponPreset.brassEjectOffset);
						ejectBrass = true;
					}
					else if (this.currentWeaponPreset.ejectBrassSetting == MurderWeaponPreset.EjectBrass.revolver)
					{
						this.revolverShots++;
					}
					for (int i = 0; i < this.currentWeaponPreset.shots; i++)
					{
						Vector3 position = this.attackTarget.transform.position;
						if (this.attackTarget.aimTransform != null)
						{
							position = this.attackTarget.aimTransform.position;
						}
						else if (this.attackTarget.lookAtThisTransform != null)
						{
							position = this.attackTarget.lookAtThisTransform.position;
						}
						else if (this.attackTarget.isPlayer)
						{
							position = CameraController.Instance.cam.transform.position;
						}
						Toolbox.Instance.Shoot(this.human, muzzlePoint, position, this.weaponRangeMax, this.weaponAccuracy, this.weaponDamage, this.currentWeaponPreset, ejectBrass, ejectBrassPoint, false);
						ejectBrass = false;
					}
					this.damageColliderCreated = true;
				}
				if (!this.ejectBrassCreated && this.currentWeaponPreset.ejectBrassSetting == MurderWeaponPreset.EjectBrass.onPumpAction && this.currentWeaponPreset.shellCasing != null && this.attackProgress >= this.currentWeaponPreset.attackTriggerPoint + 0.1f && this.currentWeaponPreset != null && this.spawnedRightItem != null)
				{
					Vector3 worldPos = this.spawnedRightItem.transform.TransformPoint(this.currentWeaponPreset.brassEjectOffset);
					Interactable interactable2 = InteractableCreator.Instance.CreateWorldInteractable(this.currentWeaponPreset.shellCasing, this.human, this.human, null, worldPos, Vector3.zero, null, null, "");
					if (interactable2 != null)
					{
						interactable2.MarkAsTrash(true, false, 0f);
						Vector3 vector = base.transform.right * Toolbox.Instance.Rand(3.5f, 4.5f, false) + new Vector3(Toolbox.Instance.Rand(-0.1f, 0.1f, false), Toolbox.Instance.Rand(-0.1f, 0.1f, false), Toolbox.Instance.Rand(-0.1f, 0.1f, false));
						if (interactable2.controller != null)
						{
							interactable2.controller.DropThis(false);
							interactable2.controller.rb.AddForce(vector, 2);
						}
						else
						{
							interactable2.ForcePhysicsActive(false, true, vector, 2, false);
						}
					}
					this.ejectBrassCreated = true;
				}
				if (this.attackProgress >= 1f)
				{
					if (!this.ejectBrassCreated && this.currentWeaponPreset.ejectBrassSetting == MurderWeaponPreset.EjectBrass.revolver && this.currentWeaponPreset.shellCasing != null && this.revolverShots >= 7 && this.spawnedRightItem != null && this.currentWeaponPreset != null)
					{
						Vector3 worldPos2 = this.spawnedRightItem.transform.TransformPoint(this.currentWeaponPreset.brassEjectOffset);
						for (int j = 0; j < 7; j++)
						{
							Interactable interactable3 = InteractableCreator.Instance.CreateWorldInteractable(this.currentWeaponPreset.shellCasing, this.human, this.human, null, worldPos2, Vector3.zero, null, null, "");
							if (interactable3 != null)
							{
								interactable3.MarkAsTrash(true, false, 0f);
								Vector3 vector2 = base.transform.right * Toolbox.Instance.Rand(3.5f, 4.5f, false) + new Vector3(Toolbox.Instance.Rand(-0.1f, 0.1f, false), Toolbox.Instance.Rand(-0.1f, 0.1f, false), Toolbox.Instance.Rand(-0.1f, 0.1f, false));
								if (interactable3.controller != null)
								{
									interactable3.controller.DropThis(false);
									interactable3.controller.rb.AddForce(vector2, 2);
								}
								else
								{
									interactable3.ForcePhysicsActive(false, true, vector2, 2, false);
								}
							}
						}
						this.ejectBrassCreated = true;
						this.revolverShots = 0;
					}
					this.attackActive = false;
					this.throwActive = false;
					this.attackProgress = 0f;
					this.damageColliderCreated = false;
					this.ejectBrassCreated = false;
					if (this.damageCollider != null)
					{
						Object.Destroy(this.damageCollider.gameObject);
					}
					this.OnAttackComplete();
				}
			}
			else if (this.combatMode == 4)
			{
				this.human.animationController.mainAnimator.SetLayerWeight(1, 1f);
				AnimatorStateInfo currentAnimatorStateInfo2 = this.human.animationController.mainAnimator.GetCurrentAnimatorStateInfo(1);
				if (currentAnimatorStateInfo2.IsTag("Swing"))
				{
					this.attackProgress = currentAnimatorStateInfo2.normalizedTime % 1f;
					Game.Log("Attack progress: " + this.attackProgress.ToString(), 2);
					if (this.attackProgress >= this.currentWeaponPreset.attackTriggerPoint && !this.damageColliderCreated)
					{
						GameObject gameObject3 = Object.Instantiate<GameObject>(PrefabControls.Instance.damageCollider, this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.LowerArmRight));
						this.damageCollider = gameObject3.GetComponent<DamageColliderController>();
						this.damageCollider.Setup(this.human, this.attackTarget, this.weaponDamage, this.GetCurrentKillTarget(), this.currentWeaponPreset);
						if (this.currentWeaponPreset.fireEvent != null)
						{
							AudioController.Instance.PlayWorldOneShot(this.currentWeaponPreset.fireEvent, this.human, this.human.currentNode, this.damageCollider.transform.position, null, null, 1f, null, false, null, false);
						}
						this.damageColliderCreated = true;
					}
					else if (this.attackProgress > this.currentWeaponPreset.attackRemovePoint && this.damageColliderCreated)
					{
						this.OnAttackComplete();
						if (this.damageCollider != null)
						{
							Object.Destroy(this.damageCollider.gameObject);
						}
					}
				}
				else if (this.damageColliderCreated && this.attackActiveLength > this.weaponRefire)
				{
					this.attackActive = false;
					this.throwActive = false;
					this.attackProgress = 0f;
					this.damageColliderCreated = false;
					this.ejectBrassCreated = false;
					if (this.damageCollider != null)
					{
						Object.Destroy(this.damageCollider.gameObject);
					}
					this.OnAttackComplete();
				}
			}
		}
		else if (this.attackDelay > 0f)
		{
			this.attackDelay -= this.delta;
			this.attackDelay = Mathf.Max(this.attackDelay, 0f);
		}
		if (this.throwDelay > 0f)
		{
			this.throwDelay -= this.delta;
			this.throwDelay = Mathf.Max(this.throwDelay, 0f);
		}
		if (this.attackActive)
		{
			if (this.attackTimeout < 3f)
			{
				this.attackTimeout += this.delta;
				return;
			}
			this.EndAttack();
		}
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x0003F010 File Offset: 0x0003D210
	public Human GetCurrentKillTarget()
	{
		if (this.killerForMurders.Count > 0)
		{
			foreach (MurderController.Murder murder in this.killerForMurders)
			{
				if (murder.state == MurderController.MurderState.executing && (murder.preset.caseType != MurderPreset.CaseType.kidnap || murder.kidnapKillPhase))
				{
					return murder.victim;
				}
			}
		}
		return null;
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x0003F098 File Offset: 0x0003D298
	private void KOUpdate()
	{
		if (this.ko)
		{
			if (SessionData.Instance.gameTime < this.koTime || this.human.isDead)
			{
				if (this.human.animationController.newBoxCollider != null)
				{
					this.human.animationController.newBoxCollider.center = this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.lowerTorso).localPosition;
				}
				if (this.human.isDead)
				{
					if (this.deadRagdollTimer < 6f)
					{
						if (this.human.animationController.upperTorsoRB != null && Mathf.Abs(this.human.animationController.upperTorsoRB.velocity.x) + Mathf.Abs(this.human.animationController.upperTorsoRB.velocity.y) + Mathf.Abs(this.human.animationController.upperTorsoRB.velocity.z) < 0.1f)
						{
							this.deadRagdollTimer += this.delta;
							return;
						}
						this.deadRagdollTimer += this.delta * 0.5f;
						return;
					}
				}
				else if (this.restrained)
				{
					this.koTime = SessionData.Instance.gameTime;
					Game.Log(this.human.name + " Set KO time: " + this.koTime.ToString(), 2);
					return;
				}
			}
			else
			{
				if (this.koTransitionTimer > 0f)
				{
					if (this.isRagdoll)
					{
						this.SetParentPositionToRagdollLimbPosition();
						this.human.animationController.SetRagdoll(false, false);
					}
					this.koTransitionTimer -= this.delta;
					float num = 1f - this.koTransitionTimer / CitizenControls.Instance.ragdollTransitionTime;
					if (CitizenControls.Instance.getUpManualAnimation.Count <= 0)
					{
						return;
					}
					using (List<CitizenAnimationController.RagdollSnapshot>.Enumerator enumerator = this.human.animationController.ragdollSnapshot.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CitizenAnimationController.RagdollSnapshot ragdollSnapshot = enumerator.Current;
							CitizenControls.ManualAnimation manualAnimation = CitizenControls.Instance.getUpManualAnimation[0];
							for (int i = 0; i < manualAnimation.limbData.Count; i++)
							{
								CitizenControls.LimbPos limbPos = manualAnimation.limbData[i];
								if (limbPos.anchor == ragdollSnapshot.anchorConfig.anchor)
								{
									Transform transform = null;
									if (this.human.outfitController.anchorReference.TryGetValue(limbPos.anchor, ref transform))
									{
										transform.localPosition = Vector3.Lerp(ragdollSnapshot.localPos, limbPos.localPosition, num);
										transform.localRotation = Quaternion.Slerp(ragdollSnapshot.localRot, limbPos.localRotation, num);
									}
								}
							}
						}
						return;
					}
				}
				if (this.getUpDelayTimer > 0f)
				{
					float num2 = 1f - Mathf.InverseLerp(0f, CitizenControls.Instance.getUpTimer, this.getUpDelayTimer);
					CitizenControls.ManualAnimation manualAnimation2 = null;
					CitizenControls.ManualAnimation manualAnimation3 = null;
					for (int j = 0; j < CitizenControls.Instance.getUpManualAnimation.Count - 1; j++)
					{
						if (CitizenControls.Instance.getUpManualAnimation[j].timeline <= num2 || manualAnimation2 == null)
						{
							manualAnimation2 = CitizenControls.Instance.getUpManualAnimation[j];
							manualAnimation3 = CitizenControls.Instance.getUpManualAnimation[j + 1];
						}
					}
					float num3 = Mathf.InverseLerp(manualAnimation2.timeline, manualAnimation3.timeline, num2);
					for (int k = 0; k < manualAnimation2.limbData.Count; k++)
					{
						CitizenControls.LimbPos limbPos2 = manualAnimation2.limbData[k];
						CitizenControls.LimbPos limbPos3 = manualAnimation3.limbData[k];
						Transform transform2 = null;
						if (this.human.outfitController.anchorReference.TryGetValue(limbPos2.anchor, ref transform2))
						{
							transform2.localPosition = Vector3.Lerp(limbPos2.localPosition, limbPos3.localPosition, num3);
							transform2.localRotation = Quaternion.Slerp(limbPos2.localRotation, limbPos3.localRotation, num3);
						}
					}
					this.getUpDelayTimer -= this.delta;
					return;
				}
				if (this.human.animationController.paused)
				{
					this.human.interactableController.coll.enabled = true;
					this.human.animationController.SetPauseAnimation(false);
					this.human.animationController.mainAnimator.Rebind();
					if (this.restrained)
					{
						this.human.animationController.SetRestrained(true);
						this.human.animationController.mainAnimator.SetTrigger("restrain");
						this.human.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.armsCuffed);
					}
				}
				this.SetKO(false, default(Vector3), default(Vector3), false, 0f, true, 1f);
			}
		}
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x0003F5B4 File Offset: 0x0003D7B4
	public void SetParentPositionToRagdollLimbPosition()
	{
		Transform bodyAnchor = this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.lowerTorso);
		if ((bodyAnchor.position + new Vector3(0f, 0.15f, 0f)).y < -20f)
		{
			if (this.human.isDead)
			{
				foreach (Case @case in CasePanelController.Instance.activeCases)
				{
					foreach (Objective objective in @case.currentActiveObjectives)
					{
						Objective.ObjectiveTrigger objectiveTrigger = objective.queueElement.triggers.Find((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.disposeOfBody && item.evidenceID == this.human.evidenceEntry.evID);
						if (objectiveTrigger != null && !objectiveTrigger.triggered && !objective.isCancelled)
						{
							Game.Log("Murder: Successful disposal of body in the sea", 2);
							objectiveTrigger.Trigger(false);
						}
					}
				}
				if (!this.human.removedFromWorld)
				{
					this.human.RemoveFromWorld(true);
				}
			}
			else
			{
				NewNode newNode = this.human.FindSafeTeleport(CityData.Instance.streetDirectory[Toolbox.Instance.Rand(0, CityData.Instance.streetDirectory.Count, false)], false, true);
				if (newNode != null)
				{
					bodyAnchor.position = new Vector3(newNode.position.x, newNode.position.y + 1.2f, newNode.position.z);
				}
				else
				{
					bodyAnchor.position = new Vector3(base.transform.position.x, base.transform.position.y + 1.2f, base.transform.position.z);
				}
			}
		}
		Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(bodyAnchor.position);
		NewNode newNode2 = null;
		Vector3 position = bodyAnchor.position;
		if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2))
		{
			position.y = newNode2.position.y;
		}
		else
		{
			newNode2 = Toolbox.Instance.GetNearestGroundLevelOutside(bodyAnchor.position);
			if (newNode2 != null)
			{
				if (Game.Instance.printDebug)
				{
					string text = "Ragdoll: Could not find a proper node position for ";
					string text2 = bodyAnchor.position.ToString();
					string text3 = ", nearest valid node is at";
					Vector3 position2 = newNode2.position;
					Game.Log(text + text2 + text3 + position2.ToString(), 2);
				}
				position = newNode2.position;
			}
		}
		bodyAnchor.SetParent(null, true);
		base.transform.position = position;
		Transform bodyAnchor2 = this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.Head);
		Vector3 vector = base.transform.position - bodyAnchor2.position;
		base.transform.LookAt(bodyAnchor2.position + vector * 3f);
		base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
		bodyAnchor.transform.SetParent(this.human.modelParent.transform, true);
		this.human.UpdateGameLocation(0f);
		Player.Instance.UpdateCulling();
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x0003F92C File Offset: 0x0003DB2C
	public void SetUpdateEnabled(bool val)
	{
		if (base.enabled != val)
		{
			if (this.human != null && Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Set Update Enabled: " + val.ToString(), Actor.HumanDebug.updates);
			}
			base.enabled = val;
		}
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x0003F980 File Offset: 0x0003DB80
	public void ClampNeckRotation()
	{
		Vector3 localEulerAngles = this.human.neckTransform.localEulerAngles;
		this.human.neckTransform.localEulerAngles = new Vector3(Toolbox.Instance.ClampAngle(localEulerAngles.x, CitizenControls.Instance.upExtent, CitizenControls.Instance.downExtent), Toolbox.Instance.ClampAngle(localEulerAngles.y, CitizenControls.Instance.leftExtent, CitizenControls.Instance.rightExtent), 0f);
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x0003FA00 File Offset: 0x0003DC00
	public void ReachNewPathNode(string debug, bool scanForNextNodeFurniture = true)
	{
		this.human.UpdateGameLocation(0f);
		this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
		if (this.doorInteractions.Count > 0)
		{
			foreach (NewDoor newDoor in this.doorInteractions)
			{
				newDoor.usingDoorList.Remove(this.human);
			}
			this.doorInteractions.Clear();
		}
		AIActionPreset.DoorRule doorRule = AIActionPreset.DoorRule.normal;
		if (this.currentAction != null)
		{
			doorRule = this.currentAction.goal.preset.doorRule;
			if (this.currentAction.preset.overrideGoalDoorRule)
			{
				doorRule = this.currentAction.preset.doorRule;
			}
		}
		if (this.openedDoor != null)
		{
			if (this.openedDoor.doorSetting == NewDoor.DoorSetting.leaveClosed && !this.dontEverCloseDoors && !this.openedDoor.isClosed && !this.openedDoor.isClosing && this.openedDoor.usingDoorList.Count <= 0)
			{
				bool flag = true;
				if (this.human.ai.currentAction != null)
				{
					if (doorRule == AIActionPreset.DoorRule.dontClose)
					{
						flag = false;
					}
					else if ((doorRule == AIActionPreset.DoorRule.onlyCloseToLocation || doorRule == AIActionPreset.DoorRule.onlyLockToLocation) && this.openedDoor.wall.node.gameLocation == this.openedDoor.wall.otherWall.node.gameLocation)
					{
						flag = false;
					}
				}
				if (InteractionController.Instance.talkingTo != null && InteractionController.Instance.talkingTo.isActor != null && (InteractionController.Instance.talkingTo.isActor.currentNode == this.openedDoor.wall.node || InteractionController.Instance.talkingTo.isActor.currentNode == this.openedDoor.wall.otherWall.node))
				{
					flag = false;
				}
				if (flag)
				{
					this.openedDoor.SetOpen(0f, this.human, false, 1f);
				}
				bool flag2 = false;
				if (this.openedDoor.lockSetting == NewDoor.LockSetting.keepLocked)
				{
					flag2 = true;
					if (doorRule == AIActionPreset.DoorRule.dontClose || doorRule == AIActionPreset.DoorRule.dontLock || doorRule == AIActionPreset.DoorRule.onlyCloseToLocation)
					{
						flag2 = false;
					}
					else if (doorRule == AIActionPreset.DoorRule.onlyLockToLocation && this.openedDoor.wall.node.gameLocation == this.openedDoor.wall.otherWall.node.gameLocation)
					{
						flag2 = false;
					}
					if (!this.openedDoor.wall.node.gameLocation.isCrimeScene)
					{
						if (!this.openedDoor.wall.node.gameLocation.currentOccupants.Exists((Actor item) => item.isEnforcer && item.isOnDuty))
						{
							if (!this.openedDoor.wall.otherWall.node.gameLocation.isCrimeScene)
							{
								if (!this.openedDoor.wall.otherWall.node.gameLocation.currentOccupants.Exists((Actor item) => item.isEnforcer && item.isOnDuty))
								{
									goto IL_334;
								}
							}
							flag2 = false;
							goto IL_334;
						}
					}
					flag2 = false;
				}
				IL_334:
				if (InteractionController.Instance.talkingTo != null && InteractionController.Instance.talkingTo.isActor != null && (InteractionController.Instance.talkingTo.isActor.currentNode == this.openedDoor.wall.node || InteractionController.Instance.talkingTo.isActor.currentNode == this.openedDoor.wall.otherWall.node))
				{
					flag2 = false;
				}
				if (this.currentGoal != null && flag2)
				{
					this.currentGoal.InsertLockAction(this.openedDoor);
				}
			}
			this.openedDoor = null;
		}
		this.pathCursor++;
		if (this.currentAction != null && this.currentAction.path != null)
		{
			if (Game.Instance.collectDebugData)
			{
				if (this.currentAction.path.accessList.Count > this.pathCursor)
				{
					string[] array = new string[8];
					array[0] = "ReachNewPathNode: ";
					array[1] = this.pathCursor.ToString();
					array[2] = "/";
					array[3] = this.currentAction.path.accessList.Count.ToString();
					array[4] = " world pos: ";
					int num = 5;
					Vector3 worldAccessPoint = this.currentAction.path.accessList[this.pathCursor].worldAccessPoint;
					array[num] = worldAccessPoint.ToString();
					array[6] = ", actor pos: ";
					array[7] = this.human.transform.position.ToString();
					this.DebugDestinationPosition(string.Concat(array));
					Actor actor = this.human;
					string[] array2 = new string[8];
					array2[0] = "ReachNewPathNode: ";
					array2[1] = this.pathCursor.ToString();
					array2[2] = "/";
					array2[3] = this.currentAction.path.accessList.Count.ToString();
					array2[4] = " world pos: ";
					int num2 = 5;
					worldAccessPoint = this.currentAction.path.accessList[this.pathCursor].worldAccessPoint;
					array2[num2] = worldAccessPoint.ToString();
					array2[6] = ", actor pos: ";
					array2[7] = this.human.transform.position.ToString();
					actor.SelectedDebug(string.Concat(array2), Actor.HumanDebug.movement);
				}
				else
				{
					this.DebugDestinationPosition(string.Concat(new string[]
					{
						"ReachNewPathNode: ",
						this.pathCursor.ToString(),
						"/",
						this.currentAction.path.accessList.Count.ToString(),
						", actor pos: ",
						this.human.transform.position.ToString()
					}));
					this.human.SelectedDebug(string.Concat(new string[]
					{
						"ReachNewPathNode: ",
						this.pathCursor.ToString(),
						"/",
						this.currentAction.path.accessList.Count.ToString(),
						", actor pos: ",
						this.human.transform.position.ToString()
					}), Actor.HumanDebug.movement);
				}
			}
			if (this.pathCursor >= this.currentAction.path.accessList.Count)
			{
				this.currentAction.DestinationCheck("", false);
			}
			else
			{
				this.doorCheck = true;
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Reset door check", Actor.HumanDebug.movement);
				}
				if (this.currentAction.path.accessList[this.pathCursor].door == null && Game.Instance.collectDebugData)
				{
					Actor actor2 = this.human;
					string text = "No door detected at path cursor ";
					string text2 = this.pathCursor.ToString();
					string text3 = " position ";
					Vector3 worldAccessPoint = this.currentAction.path.accessList[this.pathCursor].worldAccessPoint;
					actor2.SelectedDebug(text + text2 + text3 + worldAccessPoint.ToString(), Actor.HumanDebug.movement);
				}
				if (this.currentAction.path.accessList[this.pathCursor].door != null)
				{
					this.DoorCheckProcess();
				}
				else
				{
					if (this.human.currentBuilding != null)
					{
						if (!(this.human.currentNode.floor != null) || !this.human.currentNode.floor.alarmLockdown || this.pathCursor > this.currentAction.path.accessList.Count - 1)
						{
							goto IL_96E;
						}
						using (List<Interactable>.Enumerator enumerator2 = this.human.currentNode.floor.securityDoors.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Interactable interactable = enumerator2.Current;
								if (!interactable.sw0 && interactable.furnitureParent.coversNodes.Contains(this.currentAction.path.GetNodeAhead(this.pathCursor)))
								{
									this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
									this.SetStaticFromAnimation(true);
									if (this.human.isEnforcer && this.human.isOnDuty)
									{
										this.human.currentNode.floor.SetAlarmLockdown(false, null);
										this.doorCheck = false;
										break;
									}
									this.currentGoal.OnDeactivate(this.currentGoal.preset.repeatDelayOnBusy);
									return;
								}
							}
							goto IL_96E;
						}
					}
					if (this.currentAction.usagePoint != null && !this.currentAction.InteractableUsePointCheck())
					{
						this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
						this.SetStaticFromAnimation(true);
						if (!this.usePointBusyRecursion)
						{
							this.usePointBusyRecursion = true;
							this.currentAction.OnUsePointBusy();
							this.usePointBusyRecursion = false;
						}
						else
						{
							this.currentGoal.OnDeactivate(this.currentGoal.preset.repeatDelayOnBusy);
						}
					}
				}
				IL_96E:
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Door check: " + this.doorCheck.ToString(), Actor.HumanDebug.movement);
				}
				if (this.currentAction != null && this.currentAction.path != null && this.currentAction.path.accessList != null && this.doorCheck)
				{
					NewNode newLocation = null;
					if (this.pathCursor <= this.currentAction.path.accessList.Count - 1)
					{
						newLocation = this.currentAction.path.GetNodeAhead(this.pathCursor);
					}
					this.SetDestinationNode(newLocation, scanForNextNodeFurniture);
				}
			}
		}
		this.usePointBusyRecursion = false;
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00040440 File Offset: 0x0003E640
	public void DoorCheckProcess()
	{
		if (this.currentAction != null && this.currentAction.path != null && this.pathCursor > -1 && this.pathCursor < this.currentAction.path.accessList.Count && this.currentAction.path.accessList[this.pathCursor].door != null)
		{
			NewDoor door = this.currentAction.path.accessList[this.pathCursor].door;
			NewDoor.CitizenPassResult citizenPassResult;
			this.doorCheck = door.CitizenPassCheck(this.human, out citizenPassResult);
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug(string.Concat(new string[]
				{
					"Door check ",
					this.currentAction.path.accessList[this.pathCursor].door.name,
					" result: ",
					this.doorCheck.ToString(),
					" reason: ",
					citizenPassResult.ToString(),
					" actor position",
					base.transform.position.ToString()
				}), Actor.HumanDebug.movement);
			}
			AIActionPreset.DoorRule doorRule = AIActionPreset.DoorRule.normal;
			if (this.currentAction != null)
			{
				doorRule = this.currentAction.goal.preset.doorRule;
				if (this.currentAction.preset.overrideGoalDoorRule)
				{
					doorRule = this.currentAction.preset.doorRule;
				}
			}
			if (this.doorCheck)
			{
				this.openedDoor = this.currentAction.path.accessList[this.pathCursor].door;
				if (this.openedDoor.isLocked)
				{
					bool lockBehind = false;
					if (this.openedDoor.lockSetting == NewDoor.LockSetting.keepLocked)
					{
						lockBehind = true;
						if (doorRule == AIActionPreset.DoorRule.dontClose || doorRule == AIActionPreset.DoorRule.dontLock || doorRule == AIActionPreset.DoorRule.onlyCloseToLocation)
						{
							lockBehind = false;
						}
						else if (doorRule == AIActionPreset.DoorRule.onlyLockToLocation && this.openedDoor.wall.node.gameLocation == this.openedDoor.wall.otherWall.node.gameLocation)
						{
							lockBehind = false;
						}
					}
					if (this.currentGoal != null)
					{
						this.doorCheck = this.currentGoal.InsertUnlockAction(this.openedDoor, lockBehind);
						if (Game.Instance.collectDebugData)
						{
							this.human.SelectedDebug("Door check; attempt to insert unlock action: " + this.doorCheck.ToString(), Actor.HumanDebug.actions);
						}
					}
					if (this.doorCheck)
					{
						this.AITick(false, false);
						return;
					}
				}
				if (this.doorCheck)
				{
					if (!this.openedDoor.usingDoorList.Contains(this.human))
					{
						this.openedDoor.usingDoorList.Add(this.human);
						this.doorInteractions.Add(this.openedDoor);
					}
					this.openedDoor.OpenByActor(this.human, false, 1f);
				}
			}
			if (!this.doorCheck && door != null)
			{
				this.human.SetDesiredSpeed(Human.MovementSpeed.stopped);
				this.SetStaticFromAnimation(true);
				float doorKnockAttemptValue = GameplayController.Instance.GetDoorKnockAttemptValue(door, this.human);
				if (doorKnockAttemptValue <= 0f)
				{
					AudioController.Instance.PlayWorldOneShot(door.preset.audioLockedEntryAttempt, this.human, door.parentedWall.node, door.transform.position, null, null, 1f, null, false, null, false);
				}
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Knock on door value: " + doorKnockAttemptValue.ToString(), Actor.HumanDebug.movement);
				}
				if (doorKnockAttemptValue > 1.5f && citizenPassResult == NewDoor.CitizenPassResult.isJammed)
				{
					bool flag = true;
					foreach (NewNode.NodeAccess nodeAccess in this.human.currentRoom.entrances)
					{
						if (!(nodeAccess.door == door) && (nodeAccess.door != null || nodeAccess.walkingAccess))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						this.human.speechController.TriggerBark(SpeechController.Bark.doorBlocked);
					}
				}
				if (doorKnockAttemptValue < 3f)
				{
					try
					{
						Game.Log(string.Concat(new string[]
						{
							"Debug: ",
							this.human.name,
							" Knock on door at ",
							this.human.currentRoom.name,
							", ",
							this.human.currentGameLocation.name,
							": Knock value: ",
							doorKnockAttemptValue.ToString(),
							" action: ",
							this.currentAction.preset.name,
							" reason: ",
							citizenPassResult.ToString()
						}), 2);
					}
					catch
					{
					}
					NewAIGoal.DoorActionCheckResult doorActionCheckResult;
					if (this.currentAction != null && !this.currentGoal.TryInsertDoorAction(door, RoutineControls.Instance.knockOnDoor, NewAIGoal.DoorSide.mySide, 99, out doorActionCheckResult, null, "Knock on door", false))
					{
						if (!this.usePointBusyRecursion)
						{
							this.usePointBusyRecursion = true;
							this.currentAction.OnUsePointBusy();
							this.usePointBusyRecursion = false;
							return;
						}
						this.currentGoal.OnDeactivate(this.currentGoal.preset.repeatDelayOnBusy);
						return;
					}
				}
				else if (this.currentGoal != null)
				{
					if (citizenPassResult == NewDoor.CitizenPassResult.isJammed || (this.currentAction != null && this.currentAction.preset.breakDownDoors))
					{
						if (Game.Instance.collectDebugData)
						{
							this.human.SelectedDebug("Break down door!", Actor.HumanDebug.actions);
						}
						NewAIGoal.DoorActionCheckResult doorActionCheckResult2 = NewAIGoal.DoorActionCheckResult.success;
						if (!this.currentGoal.TryInsertDoorAction(door, RoutineControls.Instance.bargeDoor, NewAIGoal.DoorSide.mySide, 99, out doorActionCheckResult2, null, "Barge door!", true))
						{
							if (!this.usePointBusyRecursion)
							{
								this.usePointBusyRecursion = true;
								this.currentAction.OnUsePointBusy();
								this.usePointBusyRecursion = false;
							}
							else
							{
								this.currentGoal.OnDeactivate(this.currentGoal.preset.repeatDelayOnBusy);
							}
						}
					}
					else if (!this.usePointBusyRecursion)
					{
						this.usePointBusyRecursion = true;
						this.currentAction.OnUsePointBusy();
						this.usePointBusyRecursion = false;
					}
					else
					{
						this.currentGoal.OnDeactivate(this.currentGoal.preset.repeatDelayOnBusy);
					}
				}
			}
		}
		else
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Door check: Invalid current action, setting check to true", Actor.HumanDebug.movement);
			}
			this.doorCheck = true;
		}
		this.doorCheckProcessTimer = 5;
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00040AE8 File Offset: 0x0003ECE8
	public void SetDestinationNode(NewNode newLocation, bool scanForNextNodeFurniture = true)
	{
		if (newLocation != null && Game.Instance.collectDebugData)
		{
			Actor actor = this.human;
			string[] array = new string[5];
			array[0] = "Setting destination node location as ";
			int num = 1;
			Vector3 position = newLocation.position;
			array[num] = position.ToString();
			array[2] = " (current: ";
			array[3] = base.transform.position.ToString();
			array[4] = ")";
			actor.SelectedDebug(string.Concat(array), Actor.HumanDebug.actions);
		}
		if (this.currentDestinationNode != newLocation && this.currentDestinationNode != null)
		{
			this.human.RemoveReservedNodeSpace();
		}
		bool flag = false;
		if (this.currentAction != null && scanForNextNodeFurniture && this.currentAction.node == newLocation && this.currentAction.interactable != null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug(string.Concat(new string[]
				{
					"Occupied space override as current action (",
					this.currentAction.name,
					") is on the new location node (",
					(newLocation != null) ? newLocation.ToString() : null,
					")"
				}), Actor.HumanDebug.actions);
			}
			flag = true;
			if (!this.currentAction.InteractableUsePointCheck())
			{
				this.currentAction.OnUsePointBusy();
			}
		}
		this.currentDestinationNode = newLocation;
		if (this.currentDestinationNode != null)
		{
			this.currentDesitnationNodeCoord = this.currentDestinationNode.nodeCoord;
		}
		else
		{
			this.currentDesitnationNodeCoord = Vector3.zero;
		}
		if (this.currentDestinationNode != null && this.currentDestinationNode.room.gameObject.activeInHierarchy && Game.Instance.dynamicReRouting && this.currentDestinationNode.occupiedSpace.Count > 0 && !flag && this.currentAction != null && this.currentAction.path != null && this.currentAction.path.accessList != null && this.pathCursor + 1 < this.currentAction.path.accessList.Count)
		{
			NewNode newNode = null;
			NewNode nodeAhead = this.currentAction.path.GetNodeAhead(this.pathCursor + 1);
			if (this.DynamicReRoute(this.human.currentNode, this.currentDestinationNode, nodeAhead, out newNode))
			{
				this.currentDestinationNode = newNode;
				if (this.currentDestinationNode != null)
				{
					this.currentDesitnationNodeCoord = this.currentDestinationNode.nodeCoord;
				}
				else
				{
					this.currentDesitnationNodeCoord = Vector3.zero;
				}
			}
		}
		bool flag2;
		if (this.currentDestinationNode != null && this.currentAction != null)
		{
			flag2 = this.currentDestinationNode.AddHumanTraveller(this.human, this.currentAction.usagePoint, out this.currentDestinationPositon, this.currentAction.preset.useRandomNodeSublocation);
			if (this.currentAction.usagePoint != null && Game.Instance.collectDebugData)
			{
				Actor actor2 = this.human;
				string[] array2 = new string[9];
				array2[0] = "Updated destination position: ";
				int num2 = 1;
				Vector3 position = this.currentDestinationPositon;
				array2[num2] = position.ToString();
				array2[2] = " (node: ";
				int num3 = 3;
				position = this.currentDesitnationNodeCoord;
				array2[num3] = position.ToString();
				array2[4] = ", usage point: ";
				array2[5] = this.currentAction.usagePoint.interactable.name;
				array2[6] = " ";
				array2[7] = this.currentAction.usagePoint.interactable.id.ToString();
				array2[8] = ")";
				actor2.SelectedDebug(string.Concat(array2), Actor.HumanDebug.movement);
				string[] array3 = new string[9];
				array3[0] = "Updated destination position: ";
				int num4 = 1;
				position = this.currentDestinationPositon;
				array3[num4] = position.ToString();
				array3[2] = " (node: ";
				int num5 = 3;
				position = this.currentDesitnationNodeCoord;
				array3[num5] = position.ToString();
				array3[4] = ", usage point: ";
				array3[5] = this.currentAction.usagePoint.interactable.name;
				array3[6] = " ";
				array3[7] = this.currentAction.usagePoint.interactable.id.ToString();
				array3[8] = ")";
				this.DebugDestinationPosition(string.Concat(array3));
			}
		}
		else
		{
			flag2 = false;
			if (Game.Instance.collectDebugData)
			{
				Actor actor3 = this.human;
				string[] array4 = new string[5];
				array4[0] = "The destination was not set because of invalid input. It remains: ";
				int num6 = 1;
				Vector3 position = this.currentDestinationPositon;
				array4[num6] = position.ToString();
				array4[2] = " (node: ";
				int num7 = 3;
				position = this.currentDesitnationNodeCoord;
				array4[num7] = position.ToString();
				array4[4] = ")";
				actor3.SelectedDebug(string.Concat(array4), Actor.HumanDebug.movement);
				string[] array5 = new string[5];
				array5[0] = "The destination was not set because of invalid input. It remains: ";
				int num8 = 1;
				position = this.currentDestinationPositon;
				array5[num8] = position.ToString();
				array5[2] = " (node: ";
				int num9 = 3;
				position = this.currentDesitnationNodeCoord;
				array5[num9] = position.ToString();
				array5[4] = ")";
				this.DebugDestinationPosition(string.Concat(array5));
			}
		}
		if (!flag2 && this.delayFlag < 3)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Invalid movement, creating short delay...", Actor.HumanDebug.movement);
			}
			this.currentDestinationNode = null;
			this.currentDesitnationNodeCoord = Vector3.zero;
			this.SetDelayed(Toolbox.Instance.Rand(1f, 2f, false));
			if (this.currentAction != null)
			{
				this.currentAction.OnInvalidMovement(this.delayFlag);
			}
			this.delayFlag++;
		}
		else
		{
			this.delayFlag = 0;
		}
		if (this.currentAction == null || !this.currentAction.isAtLocation)
		{
			this.SetFaceTravelDirection();
		}
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00041078 File Offset: 0x0003F278
	private bool DynamicReRoute(NewNode current, NewNode avoidThis, NewNode beyond, out NewNode bestAvoidanceTile)
	{
		bestAvoidanceTile = null;
		if (current == null || avoidThis == null || beyond == null)
		{
			return false;
		}
		int num = -1;
		foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in current.accessToOtherNodes)
		{
			if (keyValuePair.Key == avoidThis && keyValuePair.Value.door != null)
			{
				return false;
			}
			if (keyValuePair.Key != avoidThis && keyValuePair.Value.walkingAccess && !(keyValuePair.Value.door != null) && (keyValuePair.Value.accessType == NewNode.NodeAccess.AccessType.adjacent || keyValuePair.Value.accessType == NewNode.NodeAccess.AccessType.streetToStreet) && !keyValuePair.Key.isObstacle && !keyValuePair.Key.noPassThrough && keyValuePair.Key.nodeCoord.z == current.nodeCoord.z)
			{
				NewNode.NodeAccess nodeAccess = null;
				if (keyValuePair.Key.accessToOtherNodes.TryGetValue(beyond, ref nodeAccess) && nodeAccess.walkingAccess && !(nodeAccess.door != null) && (nodeAccess.accessType == NewNode.NodeAccess.AccessType.adjacent || keyValuePair.Value.accessType == NewNode.NodeAccess.AccessType.streetToStreet) && keyValuePair.Key.nodeCoord.z == current.nodeCoord.z && keyValuePair.Key.occupiedSpace.Count < keyValuePair.Key.walkableNodeSpace.Count && (avoidThis.occupiedSpace.Count >= avoidThis.walkableNodeSpace.Count || keyValuePair.Key.occupiedSpace.Count < avoidThis.occupiedSpace.Count) && (bestAvoidanceTile == null || keyValuePair.Key.occupiedSpace.Count < num))
				{
					bestAvoidanceTile = keyValuePair.Key;
					num = keyValuePair.Key.occupiedSpace.Count;
				}
			}
		}
		if (bestAvoidanceTile == null)
		{
			if (Game.Instance.collectDebugData)
			{
				Actor actor = this.human;
				string text = "Dynamic re-route failed: From ";
				Vector3 position = current.position;
				string text2 = position.ToString();
				string text3 = ", avoiding ";
				position = avoidThis.position;
				actor.SelectedDebug(text + text2 + text3 + position.ToString(), Actor.HumanDebug.movement);
			}
			return false;
		}
		if (Game.Instance.collectDebugData)
		{
			Actor actor2 = this.human;
			string[] array = new string[6];
			array[0] = "Dynamic re-route success: From ";
			int num2 = 1;
			Vector3 position = current.position;
			array[num2] = position.ToString();
			array[2] = ", avoiding ";
			int num3 = 3;
			position = avoidThis.position;
			array[num3] = position.ToString();
			array[4] = ", new: ";
			int num4 = 5;
			position = bestAvoidanceTile.position;
			array[num4] = position.ToString();
			actor2.SelectedDebug(string.Concat(array), Actor.HumanDebug.movement);
		}
		return true;
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00041388 File Offset: 0x0003F588
	public void SetFaceTravelDirection()
	{
		this.SetFacingPosition(this.currentDestinationPositon);
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00041398 File Offset: 0x0003F598
	public void SetFacingPosition(Vector3 newLookPoint)
	{
		if (newLookPoint == this.human.transform.position)
		{
			if (Game.Instance.collectDebugData)
			{
				this.DebugDestinationPosition("New look point is same as position!");
			}
			return;
		}
		this.faceTransform = null;
		this.facingDirection = newLookPoint - this.human.transform.position;
		if (Game.Instance.collectDebugData)
		{
			Actor actor = this.human;
			string text = "Set facing position ";
			Vector3 vector = newLookPoint;
			actor.SelectedDebug(text + vector.ToString(), Actor.HumanDebug.movement);
		}
		this.facingDirection.y = 0f;
		if (this.facingDirection != Vector3.zero)
		{
			this.facingQuat = Quaternion.LookRotation(this.facingDirection);
		}
		this.DebugDestinationPosition("Set facing position: " + this.facingQuat.eulerAngles.ToString() + " current: " + base.transform.eulerAngles.ToString());
		if (this.human.visible)
		{
			this.SetUpdateEnabled(true);
			return;
		}
		base.transform.rotation = this.facingQuat;
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x000414CC File Offset: 0x0003F6CC
	public void SetFacingDirection(Vector3 newLookDirection)
	{
		this.faceTransform = null;
		this.facingDirection = newLookDirection;
		if (Game.Instance.collectDebugData)
		{
			Actor actor = this.human;
			string text = "Set facing direction ";
			Vector3 vector = newLookDirection;
			actor.SelectedDebug(text + vector.ToString(), Actor.HumanDebug.movement);
		}
		this.facingDirection.y = 0f;
		if (this.facingDirection != Vector3.zero)
		{
			this.facingQuat = Quaternion.LookRotation(this.facingDirection);
		}
		if (this.human.visible)
		{
			this.SetUpdateEnabled(true);
			return;
		}
		base.transform.rotation = this.facingQuat;
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00041574 File Offset: 0x0003F774
	public void SetFacingTransform(Transform newLookAt, Vector3 offset)
	{
		this.faceTransform = newLookAt;
		this.faceTransformOffset = offset;
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Set facing transform " + ((newLookAt != null) ? newLookAt.ToString() : null), Actor.HumanDebug.movement);
		}
		if (this.human.visible)
		{
			this.SetUpdateEnabled(true);
			return;
		}
		Vector3 vector = this.faceTransform.position + this.faceTransform.TransformPoint(this.faceTransformOffset) - base.transform.position;
		vector.y = 0f;
		this.facingQuat = Quaternion.LookRotation(vector, Vector3.up);
		base.transform.rotation = this.facingQuat;
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00041633 File Offset: 0x0003F833
	public void SetLookAtTransform(Transform newTarget, float newRank)
	{
		if (this.lookAtTransform != newTarget)
		{
			this.lookAtTransform = newTarget;
		}
		this.lookAtTransformRank = newRank;
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00041654 File Offset: 0x0003F854
	public void AddTrackedTarget(Actor newTracked)
	{
		int num = this.trackedTargets.FindIndex((NewAIController.TrackingTarget item) => item.actor == newTracked);
		NewAIController.TrackingTarget trackingTarget;
		if (num <= -1)
		{
			trackingTarget = new NewAIController.TrackingTarget();
			trackingTarget.actor = newTracked;
			this.trackedTargets.Add(trackingTarget);
			if (Game.Instance.collectDebugData)
			{
				Actor actor = this.human;
				string text = "Added new tracking target: ";
				Actor actor2 = trackingTarget.actor;
				actor.SelectedDebug(text + ((actor2 != null) ? actor2.ToString() : null), Actor.HumanDebug.sight);
			}
			trackingTarget.attractionRank = 0.5f;
			Human human = newTracked as Human;
			if (human != null && this.human.attractedTo.Contains(human.gender))
			{
				trackingTarget.attractionRank = 1f;
			}
		}
		else
		{
			trackingTarget = this.trackedTargets[num];
		}
		trackingTarget.lastValidSighting = SessionData.Instance.gameTime;
		trackingTarget.priorityTarget = newTracked.illegalStatus;
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00041754 File Offset: 0x0003F954
	private void TrackingSpookCheck(NewAIController.TrackingTarget newTarget, bool seen)
	{
		if (seen)
		{
			if (!newTarget.spookedByItem && newTarget.active)
			{
				if (newTarget.actor.ai != null)
				{
					if (newTarget.actor.ai.inCombat && (newTarget.actor.ai.attackTarget == this.human || newTarget.actor.ai.persuitTarget == this.human) && newTarget.actor.ai.currentWeaponPreset != null && this.human.speechController != null && !this.human.ai.inCombat && Toolbox.Instance.Rand(0f, 1f, false) <= newTarget.actor.ai.currentWeaponPreset.barkTriggerChance && this.human.speechController.speechQueue.Count <= 0)
					{
						this.human.speechController.TriggerBark(newTarget.actor.ai.currentWeaponPreset.bark);
						newTarget.spookedByItem = true;
					}
				}
				else if (newTarget.actor.isPlayer && FirstPersonItemController.Instance.currentItem != null && FirstPersonItemController.Instance.currentItem.barkTriggerChance > 0f && this.human.speechController != null && !this.human.ai.inCombat && Toolbox.Instance.Rand(0f, 1f, false) <= FirstPersonItemController.Instance.currentItem.barkTriggerChance && this.human.speechController.speechQueue.Count <= 0)
				{
					this.human.speechController.TriggerBark(FirstPersonItemController.Instance.currentItem.bark);
					newTarget.spookedByItem = true;
				}
			}
			else if (newTarget.spookedByItem)
			{
				if (newTarget.actor.ai != null && !newTarget.actor.ai.inCombat)
				{
					newTarget.spookedByItem = false;
					newTarget.spookTimer = 0;
				}
				else if (newTarget.actor.isPlayer && (FirstPersonItemController.Instance.currentItem == null || FirstPersonItemController.Instance.currentItem.barkTriggerChance <= 0f))
				{
					newTarget.spookedByItem = false;
					newTarget.spookTimer = 0;
				}
				newTarget.spookTimer++;
				if (newTarget.spookTimer > 35)
				{
					newTarget.spookedByItem = false;
					newTarget.spookTimer = 0;
				}
			}
			else if (!newTarget.active)
			{
				newTarget.spookedByItem = false;
				newTarget.spookTimer = 0;
			}
		}
		else
		{
			newTarget.spookedByItem = false;
		}
		this.UpdateHumanDrawnWeapon(newTarget.actor as Human, seen);
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00041A5C File Offset: 0x0003FC5C
	public void UpdateHumanDrawnWeapon(Human who, bool seen)
	{
		if (who != null)
		{
			float num = 0f;
			if (!seen)
			{
				if (this.appliedNerveEffect.ContainsKey(who))
				{
					num = -this.appliedNerveEffect[who];
					this.appliedNerveEffect.Remove(who);
				}
			}
			else if (who.ai != null)
			{
				if (!who.ai.inCombat)
				{
					if (this.appliedNerveEffect.ContainsKey(who))
					{
						num = -this.appliedNerveEffect[who];
						this.appliedNerveEffect.Remove(who);
					}
				}
				else if (who.ai.currentWeaponPreset != null && !this.appliedNerveEffect.ContainsKey(who))
				{
					this.appliedNerveEffect.Add(who, who.ai.currentWeaponPreset.drawnNerveModifier);
					num = this.appliedNerveEffect[who];
				}
			}
			else if (who.isPlayer)
			{
				if (FirstPersonItemController.Instance.currentItem == null)
				{
					if (this.appliedNerveEffect.ContainsKey(who))
					{
						num = -this.appliedNerveEffect[who];
						this.appliedNerveEffect.Remove(who);
					}
				}
				else
				{
					float num2;
					if (FirstPersonItemController.Instance.currentItem.name.ToLower() == "fists")
					{
						num2 = FirstPersonItemController.Instance.currentItem.drawnNerveModifier * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.fistsThreatModifier));
					}
					else
					{
						num2 = FirstPersonItemController.Instance.currentItem.drawnNerveModifier;
					}
					if (this.appliedNerveEffect.ContainsKey(who))
					{
						float num3 = this.appliedNerveEffect[who];
						num = num2 - num3;
						if (num != 0f)
						{
							this.appliedNerveEffect[who] = num2;
						}
					}
					else
					{
						this.appliedNerveEffect.Add(who, num2);
						num = this.appliedNerveEffect[who];
					}
				}
			}
			if (num != 0f)
			{
				this.human.AddNerve(num * CitizenControls.Instance.nerveWeaponDrawMultiplier, null);
			}
		}
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00041C6C File Offset: 0x0003FE6C
	public void UpdateTrackedTargets()
	{
		float num = GameplayControls.Instance.citizenFOV * 0.5f;
		for (int i = 0; i < this.trackedTargets.Count; i++)
		{
			NewAIController.TrackingTarget trackingTarget = this.trackedTargets[i];
			trackingTarget.active = false;
			if (SessionData.Instance.gameTime > trackingTarget.lastValidSighting + CitizenControls.Instance.lookAtGracePeriod)
			{
				if (Game.Instance.collectDebugData)
				{
					Actor actor = this.human;
					string text = "Removed tracking target due to sighting grace period: ";
					Actor actor2 = trackingTarget.actor;
					actor.SelectedDebug(text + ((actor2 != null) ? actor2.ToString() : null), Actor.HumanDebug.sight);
				}
				this.TrackingSpookCheck(trackingTarget, false);
				this.RemoveLookAtTargetAt(i);
				i--;
			}
			else
			{
				trackingTarget.lookAtRank = 0f;
				trackingTarget.attractionRank = 0f;
				trackingTarget.fovRank = 0f;
				trackingTarget.distanceRank = 0f;
				trackingTarget.itemRank = 0f;
				if (trackingTarget.priorityTarget || trackingTarget.spookedByItem)
				{
					trackingTarget.lookAtRank = 99f;
				}
				else
				{
					trackingTarget.distance = Vector3.Distance(this.human.lookAtThisTransform.position, trackingTarget.actor.lookAtThisTransform.position);
					if (trackingTarget.distance <= GameplayControls.Instance.citizenSightRange)
					{
						trackingTarget.distanceRank += (1f - Mathf.Clamp01(trackingTarget.distance / GameplayControls.Instance.citizenSightRange)) * 0.33f;
					}
					else
					{
						trackingTarget.distanceRank = 0f;
					}
					float num2 = Vector3.Angle(trackingTarget.actor.lookAtThisTransform.position - this.human.lookAtThisTransform.position, base.transform.forward);
					if (num2 >= -num && num2 <= num)
					{
						trackingTarget.fovRank += (1f - Mathf.Abs(num2) / num) * 0.33f;
					}
					if (this.appliedNerveEffect.ContainsKey(trackingTarget.actor as Human))
					{
						trackingTarget.itemRank = this.appliedNerveEffect[trackingTarget.actor as Human] * -1f;
					}
					trackingTarget.lookAtRank = trackingTarget.attractionRank + trackingTarget.distanceRank + trackingTarget.fovRank + trackingTarget.itemRank;
				}
			}
		}
		if (this.human.interactingWith != null)
		{
			this.SetLookAtTransform(this.human.lookAtThisTransform, 2f);
		}
		else if (this.human.inConversation && this.human.currentConversation.currentlyTalking != null && this.human.currentConversation.currentlyTalking != this.human)
		{
			this.SetLookAtTransform(this.human.currentConversation.currentlyTalking.lookAtThisTransform, 2f);
		}
		else if (this.human.inConversation && this.human.currentConversation.currentlyTalkingTo != null && this.human.currentConversation.currentlyTalkingTo != this.human)
		{
			this.SetLookAtTransform(this.human.currentConversation.currentlyTalkingTo.lookAtThisTransform, 2f);
		}
		else if (this.seesOnPersuit)
		{
			this.SetLookAtTransform(Player.Instance.lookAtThisTransform, 2f);
		}
		else if (this.audioFocusAction != null)
		{
			this.SetTrackTarget(null);
		}
		else if (this.trackedTargets.Count > 0)
		{
			this.trackedTargets.Remove(null);
			try
			{
				this.trackedTargets.Sort((NewAIController.TrackingTarget a1, NewAIController.TrackingTarget a2) => a2.lookAtRank.CompareTo(a1.lookAtRank));
			}
			catch
			{
			}
			if (this.trackedTargets[0] != this.currentTrackTarget)
			{
				if (this.currentTrackTarget != null)
				{
					this.TrackingSpookCheck(this.currentTrackTarget, false);
				}
				this.SetTrackTarget(this.trackedTargets[0]);
			}
		}
		else if (this.lookAtTransform != null)
		{
			this.SetTrackTarget(null);
		}
		if (this.currentTrackTarget != null)
		{
			this.currentTrackTarget.active = true;
			this.TrackingSpookCheck(this.currentTrackTarget, true);
		}
		if (this.human.visible)
		{
			this.SetUpdateEnabled(true);
		}
		else
		{
			if (this.lookAtTransform != null)
			{
				Vector3 vector = this.lookAtTransform.position - this.human.lookAtThisTransform.position;
				this.lookingQuatCurrent = Quaternion.LookRotation(vector, Vector3.up);
			}
			else
			{
				this.lookingQuatCurrent = Quaternion.LookRotation(this.human.transform.forward, Vector3.up);
			}
			this.human.neckTransform.rotation = this.lookingQuatCurrent;
			this.ClampNeckRotation();
		}
		this.lastLookAtUpdateTime = SessionData.Instance.gameTime;
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x00042150 File Offset: 0x00040350
	public void SetTrackTarget(NewAIController.TrackingTarget newTrackingTarget)
	{
		if (newTrackingTarget == null)
		{
			this.SetLookAtTransform(null, 0f);
			this.currentTrackTarget = null;
			return;
		}
		if (newTrackingTarget != this.currentTrackTarget)
		{
			if (this.currentTrackTarget != null && Game.Instance.collectDebugData)
			{
				Actor actor = this.human;
				string[] array = new string[6];
				array[0] = "Set new tracking target: ";
				int num = 1;
				Actor actor2 = newTrackingTarget.actor;
				array[num] = ((actor2 != null) ? actor2.ToString() : null);
				array[2] = " with rank of ";
				array[3] = newTrackingTarget.lookAtRank.ToString();
				array[4] = " vs old target ";
				array[5] = this.currentTrackTarget.lookAtRank.ToString();
				actor.SelectedDebug(string.Concat(array), Actor.HumanDebug.sight);
			}
			this.currentTrackTarget = newTrackingTarget;
			this.SetLookAtTransform(newTrackingTarget.actor.lookAtThisTransform, newTrackingTarget.lookAtRank);
			this.OnNewTrackTarget();
		}
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x00042220 File Offset: 0x00040420
	public void OnNewTrackTarget()
	{
		if (this.currentTrackTarget != null && this.currentTrackTarget.actor != null)
		{
			this.human.SpeechTriggerPoint(DDSSaveClasses.TriggerPoint.onNewTrackTarget, this.currentTrackTarget.actor, null);
			try
			{
				Human human = this.currentTrackTarget.actor as Human;
				string text;
				if (human != null && Toolbox.Instance.Rand(0f, 1f, false) <= GameplayControls.Instance.muggingChance && this.IsMuggingValid(human, out text) && this.currentGoal != null)
				{
					if (!this.currentGoal.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.mugging) && !this.human.inConversation && this.currentGoal.TryInsertInteractableAction(human.interactable, RoutineControls.Instance.mugging, 99, null, true))
					{
						Game.Log(string.Concat(new string[]
						{
							"Triggered mugging of ",
							human.name,
							" by ",
							this.human.name,
							"..."
						}), 2);
					}
				}
			}
			catch
			{
			}
			if (Game.Instance.allowLoitering && !this.inCombat && !this.human.isStunned && !this.human.isDead && !this.human.isAsleep && this.currentTrackTarget.actor.isPlayer && Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.company != null && Player.Instance.currentGameLocation.thisAsAddress.company.preset.enableLoiteringBehaviour && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.disableLoitering) < 1f && Player.Instance.currentGameLocation.thisAsAddress.company.openForBusinessActual && Player.Instance.currentGameLocation == this.human.currentGameLocation && this.human.isAtWork)
			{
				if (this.human.currentGameLocation.playerLoiteringTimer >= GameplayControls.Instance.loiteringCommentThreshold && this.human.currentGameLocation.playerLoiteringTimer < GameplayControls.Instance.loiteringTrespassThreshold)
				{
					if (this.human.speechController != null && this.human.speechController.speechQueue.Count <= 0)
					{
						this.human.speechController.Speak("d11eaedc-aa24-4c8a-8b38-bfb2362861ca", false, false, null, null);
						return;
					}
				}
				else if (this.human.currentGameLocation.playerLoiteringTimer >= GameplayControls.Instance.loiteringTrespassThreshold && this.human.speechController != null && this.human.speechController.speechQueue.Count <= 0)
				{
					this.human.speechController.Speak("76bd43d0-627d-4f86-ac6f-cabea3ec2fe8", false, true, null, null);
				}
			}
		}
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00042584 File Offset: 0x00040784
	public bool IsMuggingValid(Human target, out string debugReason)
	{
		bool result = false;
		debugReason = string.Empty;
		if (this.human.currentRoom.preset.allowMuggings && target.currentRoom != null && target.currentRoom.preset.allowMuggings)
		{
			if (!target.isPlayer || GameplayController.Instance.GetCurrentSocialCreditLevel() >= 2)
			{
				int num;
				string text;
				if (!this.human.IsTrespassing(target.currentRoom, out num, out text, false))
				{
					if (this.currentGoal == null || !this.currentGoal.preset.diabledMugging)
					{
						if (!this.persuit && !this.human.isDead && !this.human.isStunned && !target.isAsleep && !target.isStunned && !target.isDead && !target.isAsleep && !target.inConversation)
						{
							if (!target.isPlayer || InteractionController.Instance.talkingTo != this.human.interactable)
							{
								if (!target.isPlayer || GameplayController.Instance.money > 0)
								{
									if (this.human.agreeableness + this.human.humility <= 0.7f)
									{
										if (this.human.societalClass <= 0.25f)
										{
											if (target.currentRoom != null)
											{
												if (target.isPlayer || target.societalClass > this.human.societalClass + 0.35f)
												{
													if (target.isPlayer || target.ai.currentGoal == null || !target.ai.currentGoal.preset.diabledMugging)
													{
														if (target.isPlayer || !this.human.acquaintances.Exists((Acquaintance item) => item.with == target))
														{
															int num2 = 0;
															using (HashSet<Actor>.Enumerator enumerator = target.currentRoom.currentOccupants.GetEnumerator())
															{
																while (enumerator.MoveNext())
																{
																	if (Vector3.Distance(enumerator.Current.transform.position, target.transform.position) < 10f)
																	{
																		num2++;
																	}
																}
															}
															foreach (NewRoom newRoom in target.currentRoom.adjacentRooms)
															{
																if (!(newRoom == target.currentRoom))
																{
																	using (HashSet<Actor>.Enumerator enumerator = newRoom.currentOccupants.GetEnumerator())
																	{
																		while (enumerator.MoveNext())
																		{
																			if (Vector3.Distance(enumerator.Current.transform.position, target.transform.position) < 10f)
																			{
																				num2++;
																			}
																		}
																	}
																}
															}
															if (num2 < 3)
															{
																if (this.currentGoal != null)
																{
																	result = true;
																}
															}
															else
															{
																debugReason = "Too many people at target's location (" + num2.ToString() + ")";
															}
														}
														else
														{
															debugReason = "Mugger knows target";
														}
													}
													else
													{
														debugReason = "Target's goal doe not allow mugging";
													}
												}
												else
												{
													debugReason = "Mismatch of target's societal class";
												}
											}
											else
											{
												debugReason = "Target is not in a room";
											}
										}
										else
										{
											debugReason = "Mugger's societal class is not compatible";
										}
									}
									else
									{
										debugReason = "Mugger's traits are not compatible";
									}
								}
							}
							else
							{
								debugReason = "Player is talking to someboy else";
							}
						}
						else
						{
							debugReason = "Mugger is inPersuit/Dead/Stunned or target is Asleep/Stunned/Dead/Asleep";
						}
					}
					else
					{
						debugReason = "Mugger's current goal does not allow mugging (" + this.currentGoal.preset.name + ")";
					}
				}
				else
				{
					debugReason = "Mugger would be trespassing at target's location (" + target.currentRoom.GetName() + ")";
				}
			}
		}
		else
		{
			debugReason = "Location " + this.human.currentRoom.GetName() + " does not allow muggings";
		}
		return result;
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00042A24 File Offset: 0x00040C24
	private void RemoveLookAtTargetAt(int index)
	{
		this.trackedTargets.RemoveAt(index);
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00002265 File Offset: 0x00000465
	public void OnVisibilityChanged()
	{
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00042A32 File Offset: 0x00040C32
	public void SetExpression(CitizenOutfitController.Expression newExpression)
	{
		if (this.currentExpression == null || this.currentExpression.expression != newExpression)
		{
			this.expressionProgress = 0f;
			this.human.outfitController.expressionReference.TryGetValue(newExpression, ref this.currentExpression);
		}
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00042A74 File Offset: 0x00040C74
	public void AddDebugAction(string msg)
	{
		if (!Game.Instance.devMode || !Game.Instance.collectDebugData)
		{
			return;
		}
		if (this.lastActions.Count >= 20)
		{
			this.lastActions.RemoveAt(0);
		}
		this.lastActions.Add(SessionData.Instance.gameTime.ToString() + ": " + msg);
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00042ADA File Offset: 0x00040CDA
	[Button("Teleport Player", 0)]
	public void DebugTeleportPlayerToLocation()
	{
		Player.Instance.Teleport(this.human.currentNode, null, true, false);
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x00042AF4 File Offset: 0x00040CF4
	[Button("Give Sleep!", 0)]
	public void GiveSleep()
	{
		this.human.AddEnergy(1f);
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00042B06 File Offset: 0x00040D06
	[Button("Remove Sleep!", 0)]
	public void RemoveSleep()
	{
		this.human.AddEnergy(-1f);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00042B18 File Offset: 0x00040D18
	[Button("Give Food!", 0)]
	public void GiveFood()
	{
		this.human.AddNourishment(0.2f);
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00042B2A File Offset: 0x00040D2A
	[Button("Remove Food!", 0)]
	public void RemoveFood()
	{
		this.human.AddNourishment(-0.2f);
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00042B3C File Offset: 0x00040D3C
	[Button("Give Drink!", 0)]
	public void GiveDrink()
	{
		this.human.AddHydration(0.2f);
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x00042B4E File Offset: 0x00040D4E
	[Button("Remove Drink!", 0)]
	public void RemoveDrink()
	{
		this.human.AddHydration(-0.2f);
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00042B60 File Offset: 0x00040D60
	[Button("Give Caffeine!", 0)]
	public void GiveCaffeine()
	{
		this.human.AddAlertness(0.2f);
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00042B72 File Offset: 0x00040D72
	[Button("Remove Caffeine!", 0)]
	public void RemoveCaffeine()
	{
		this.human.AddAlertness(-0.2f);
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00042B84 File Offset: 0x00040D84
	[Button("Give Fun!", 0)]
	public void GiveFun()
	{
		this.human.AddExcitement(0.2f);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00042B96 File Offset: 0x00040D96
	[Button("Remove Fun!", 0)]
	public void RemoveFun()
	{
		this.human.AddExcitement(-0.2f);
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x00042BA8 File Offset: 0x00040DA8
	[Button("Give Bladder!", 0)]
	public void GiveBladder()
	{
		this.human.AddBladder(0.2f);
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00042BBA File Offset: 0x00040DBA
	[Button("Remove Bladder!", 0)]
	public void RemoveBladder()
	{
		this.human.AddBladder(-0.2f);
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00042BCC File Offset: 0x00040DCC
	[Button("Give Hygeine!", 0)]
	public void GiveHygiene()
	{
		this.human.AddHygiene(0.2f);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00042BDE File Offset: 0x00040DDE
	[Button("Remove Hygeine!", 0)]
	public void RemoveHygiene()
	{
		this.human.AddHygiene(-0.2f);
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00042BF0 File Offset: 0x00040DF0
	[Button("Give Drunk!", 0)]
	public void GiveDrunk()
	{
		this.human.AddDrunk(0.2f);
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00042C02 File Offset: 0x00040E02
	[Button("Remove Drunk!", 0)]
	public void RemoveDrunk()
	{
		this.human.AddDrunk(-0.2f);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00042C14 File Offset: 0x00040E14
	[Button("Murder", 0)]
	public void MurderButton()
	{
		this.human.SetHealth(0f);
		this.SetKO(true, default(Vector3), default(Vector3), false, 0f, true, 1f);
		this.human.Murder(Player.Instance, true, null, null, 1f);
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00042C6E File Offset: 0x00040E6E
	[Button("Debug: Why Aren't I moving?", 0)]
	public void DebugMovement()
	{
		this.debugMovement = !this.debugMovement;
		Game.Log("Debug movement for " + this.human.name + ": " + this.debugMovement.ToString(), 2);
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x00042CAC File Offset: 0x00040EAC
	[Button("Trip", 0)]
	public void Trip()
	{
		if (!this.human.isDead && !this.human.isStunned && !this.isTripping && this.human.animationController != null)
		{
			this.isTripping = true;
			this.human.animationController.TriggerTrip();
		}
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00042D05 File Offset: 0x00040F05
	[Button(null, 0)]
	public void UpdateProjectedChasePosition()
	{
		this.chaseLogic.GenerateProjectedNode();
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00042D14 File Offset: 0x00040F14
	public void HearIllegal(AudioEvent audioEvent, NewNode newInvestigateNode, Vector3 newInvestigatePosition, Actor newTarget, int escLevel)
	{
		float num = audioEvent.audioFocus;
		if (this.seesOnPersuit)
		{
			return;
		}
		if ((this.persuit || (newTarget != null && this.human.seesIllegal.ContainsKey(newTarget))) && !this.seesOnPersuit)
		{
			num = 1f;
		}
		if (audioEvent != null)
		{
			this.human.SelectedDebug(string.Concat(new string[]
			{
				"Hears illegal sound: ",
				audioEvent.name,
				" by ",
				(newTarget != null) ? newTarget.ToString() : null,
				" audio focus: ",
				num.ToString()
			}), Actor.HumanDebug.sight);
		}
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.debugLastHeardIllegalAudio = audioEvent;
		}
		if (this.human.inConversation)
		{
			this.human.currentConversation.EndConversation();
		}
		if (audioEvent.spookValue != 0f && (!audioEvent.noSpookIfEnforcer || !this.human.isEnforcer))
		{
			this.human.AddNerve(-audioEvent.spookValue, null);
		}
		this.hearTarget = newTarget;
		this.hearsIllegal += num;
		this.hearsIllegal = Mathf.Clamp01(this.hearsIllegal);
		if (this.hearsIllegal >= 1f)
		{
			this.Investigate(newInvestigateNode, newInvestigatePosition, null, NewAIController.ReactionState.investigatingSound, CitizenControls.Instance.soundMinInvestigationTimeMP, escLevel, audioEvent.urgentResponse, 1f, null);
			if (newTarget != null && audioEvent.citizenMemoryTag > AudioEvent.MemoryTag.none)
			{
				this.human.UpdateLastSighting(newTarget as Human, false, (int)audioEvent.citizenMemoryTag);
			}
		}
		else if (this.hearsIllegal >= 0f && this.audioFocusAction == null && this.currentGoal != null)
		{
			if (this.human.speechController != null)
			{
				this.human.speechController.TriggerBark(SpeechController.Bark.hearsSuspicious);
			}
			this.audioFocusAction = this.CreateNewAction(this.currentGoal, RoutineControls.Instance.audioFocus, true, null, null, newInvestigateNode, null, null, false, 99, "");
			this.AITick(false, false);
			this.SetReactionState(NewAIController.ReactionState.investigatingSound);
		}
		this.SetUpdateEnabled(true);
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00042F40 File Offset: 0x00041140
	public void Investigate(NewNode newInvestigateNode, Vector3 newInvestigatePosition, Actor newTarget, NewAIController.ReactionState newReactionState, float minimumInvestiationTimeMP, int escalation, bool setHighUrgency = false, float focusTimeMultiplier = 1f, Interactable newInvesigationObj = null)
	{
		if (this.human.isDead)
		{
			return;
		}
		if (newInvestigateNode != null)
		{
			if (this.confineLocation != null && this.confineLocation != newInvestigateNode.gameLocation)
			{
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("AI is confined to location " + this.confineLocation.name, Actor.HumanDebug.actions);
					this.DebugDestinationPosition("AI is confined to location " + this.confineLocation.name);
				}
				return;
			}
			if (this.avoidLocations != null && this.avoidLocations.Contains(newInvestigateNode.gameLocation))
			{
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("AI is avoiding location " + newInvestigateNode.gameLocation.name, Actor.HumanDebug.actions);
					this.DebugDestinationPosition("AI is avoiding location " + newInvestigateNode.gameLocation.name);
				}
				return;
			}
		}
		if (this.victimsForMurders.Count > 0)
		{
			foreach (MurderController.Murder murder in this.victimsForMurders)
			{
				if (!(murder.location == null) && murder.state >= MurderController.MurderState.travellingTo && murder.preset.blockVictimFromLeavingLocation && murder.location != newInvestigateNode.gameLocation)
				{
					Game.Log("Murder: Removing investigation exterior to murder location of " + murder.location.name, 2);
					return;
				}
			}
		}
		if (this.audioFocusAction != null)
		{
			this.audioFocusAction.Complete();
		}
		if (setHighUrgency && this.investigationUrgency == NewAIController.InvestigationUrgency.walk)
		{
			this.SetInvestigationUrgency(NewAIController.InvestigationUrgency.run);
		}
		if (this.human.inConversation)
		{
			this.human.currentConversation.EndConversation();
		}
		if (newTarget != null && newTarget.isPlayer && newTarget.isHiding && newTarget.currentNode == newInvestigateNode && !Player.Instance.spottedWhileHiding.Contains(this.human))
		{
			newInvestigateNode = Toolbox.Instance.PickNearbyNode(newTarget.currentNode);
			newInvestigatePosition = newInvestigateNode.position;
		}
		this.investigatePosition = newInvestigatePosition;
		if (this.currentGoal != null && this.currentGoal.preset == RoutineControls.Instance.fleeGoal)
		{
			return;
		}
		bool flag = false;
		if (newInvestigateNode != null)
		{
			int num;
			string text;
			flag = this.human.IsTrespassing(newInvestigateNode.room, out num, out text, true);
		}
		if (flag && newReactionState != NewAIController.ReactionState.persuing)
		{
			string[] array = new string[9];
			array[0] = "AI: ";
			array[1] = this.human.name;
			array[2] = " (";
			array[3] = this.human.job.preset.name;
			array[4] = ") would be trespassing by investigating ";
			int num2 = 5;
			Vector3 position = newInvestigateNode.position;
			array[num2] = position.ToString();
			array[6] = " (";
			array[7] = newInvestigateNode.room.gameLocation.name;
			array[8] = ") so doing nothing...";
			Game.Log(string.Concat(array), 2);
			return;
		}
		this.SetUpdateEnabled(true);
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Set escalation: MAX " + this.human.escalationLevel.ToString() + "/" + escalation.ToString(), Actor.HumanDebug.sight);
		}
		this.human.SetEscalation(Mathf.Max(this.human.escalationLevel, escalation));
		if (this.persuitTarget != null)
		{
			if (!this.persuitTarget.isPlayer)
			{
				this.investigatePositionProjection = this.persuitTarget.transform.TransformPoint(Vector3.forward * 2f);
			}
			else
			{
				this.investigatePositionProjection = this.persuitTarget.transform.position + Player.Instance.fps.movementThisUpdate.normalized * 2f;
			}
		}
		else
		{
			this.investigatePositionProjection = newInvestigatePosition;
		}
		if (this.investigateLocation != newInvestigateNode)
		{
			this.investigateLocation = newInvestigateNode;
			if (this.investigationGoal.isActive)
			{
				if (Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Investigation goal is active and a new different node is given: Refresh actions", Actor.HumanDebug.actions);
				}
				this.investigationGoal.RefreshActions(true);
				this.investigationGoal.AITick();
			}
		}
		else if (this.investigationGoal.isActive && this.currentAction != null && (this.currentAction.preset == RoutineControls.Instance.searchArea || this.currentAction.preset == RoutineControls.Instance.searchAreaEnforcer))
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Investigation goal is active and AI was previously searching: Refresh actions", Actor.HumanDebug.actions);
			}
			this.investigationGoal.RefreshActions(true);
			this.investigationGoal.AITick();
		}
		this.lastInvestigate = SessionData.Instance.gameTime;
		this.investigateObject = newInvesigationObj;
		this.minimumInvestigationTimeMultiplier = Mathf.Max(this.minimumInvestigationTimeMultiplier, minimumInvestiationTimeMP);
		if ((this.persuit || this.human.seesIllegal.Count > 0) && newReactionState == NewAIController.ReactionState.investigatingSound)
		{
			return;
		}
		if (this.persuit && newReactionState == NewAIController.ReactionState.investigatingSight)
		{
			return;
		}
		bool flag2 = false;
		if (newInvesigationObj != null)
		{
			if (!this.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor != null && item.actor.isPlayer) && newReactionState != this.reactionState && newReactionState == NewAIController.ReactionState.investigatingSound && Toolbox.Instance.Rand(0f, 1f, false) > 0.75f)
			{
				flag2 = true;
				this.human.speechController.TriggerBark(SpeechController.Bark.hearsObject);
			}
		}
		if (!flag2 && newReactionState != this.reactionState)
		{
			if (newTarget != null && newTarget.isPlayer && this.spookCounter > 0)
			{
				this.human.speechController.TriggerBark(SpeechController.Bark.spookConfront);
			}
			else if (newReactionState == NewAIController.ReactionState.investigatingSound && this.reactionState != NewAIController.ReactionState.investigatingSight && this.persuitTarget == null && !this.persuit)
			{
				this.human.speechController.TriggerBark(SpeechController.Bark.hearsSuspicious);
			}
			else if (newReactionState == NewAIController.ReactionState.investigatingSight)
			{
				this.human.speechController.TriggerBark(SpeechController.Bark.seesSuspicious);
			}
		}
		this.SetReactionState(newReactionState);
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00043574 File Offset: 0x00041774
	public void SetInvestigationUrgency(NewAIController.InvestigationUrgency newUrgency)
	{
		this.investigationUrgency = newUrgency;
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x00043580 File Offset: 0x00041780
	public void SetPersue(Actor newTarget, bool publicFauxPas, int escalation, bool setHighUrgency, float responseRange = 10f)
	{
		if (!this.human.isStunned && !this.human.isDead && !this.human.isAsleep)
		{
			this.SetPersuit(true);
			this.SetPersueTarget(newTarget);
			this.human.AddToSeesIllegal(newTarget, 1f);
			this.persuitChaseLogicUses = 1f;
			this.Investigate(newTarget.currentNode, newTarget.transform.position, newTarget, NewAIController.ReactionState.persuing, CitizenControls.Instance.persuitMinInvestigationTimeMP, escalation, setHighUrgency, 1f, null);
		}
		if (publicFauxPas)
		{
			foreach (Actor actor in new List<Actor>(this.human.currentRoom.currentOccupants))
			{
				if (!(actor == this.human) && !actor.isStunned && !actor.isDead && !actor.isAsleep && Vector3.Distance(actor.transform.position, this.human.transform.position) <= responseRange && actor.ai != null)
				{
					actor.ai.SetPersuit(true);
					actor.ai.SetPersueTarget(newTarget);
					actor.AddToSeesIllegal(newTarget, 1f);
					actor.ai.persuitChaseLogicUses = 0.5f;
					actor.ai.Investigate(newTarget.currentNode, newTarget.transform.position, newTarget, NewAIController.ReactionState.persuing, CitizenControls.Instance.persuitMinInvestigationTimeMP, escalation, setHighUrgency, 1f, null);
				}
			}
		}
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x00043730 File Offset: 0x00041930
	public void SetPersueTarget(Actor newTarget)
	{
		if (this.persuitTarget != null)
		{
			this.persuitTarget.RemovePersuedBy(this.human);
		}
		this.persuitTarget = newTarget;
		if (this.persuitTarget != null)
		{
			this.persuitTarget.AddPersuedBy(this.human);
			this.chaseLogic.UpdateLastSeen();
		}
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00043790 File Offset: 0x00041990
	public void CancelPersue()
	{
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Cancelled persuit", Actor.HumanDebug.sight);
		}
		this.SetPersueTarget(null);
		this.SetPersuit(false);
		this.SetSeesOnPersuit(false);
		this.persuitChaseLogicUses = 0f;
		this.SetPersueTarget(null);
		this.persuitUpdateTimer = 0f;
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x000437EC File Offset: 0x000419EC
	public void SetPersuit(bool val)
	{
		if (this.persuit != val)
		{
			this.persuit = val;
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Set persuit: " + this.persuit.ToString(), Actor.HumanDebug.sight);
			}
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0004382B File Offset: 0x00041A2B
	public void SetSeesOnPersuit(bool val)
	{
		if (this.seesOnPersuit != val)
		{
			this.seesOnPersuit = val;
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Set seesOnPersuit: " + this.seesOnPersuit.ToString(), Actor.HumanDebug.sight);
			}
		}
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0004386C File Offset: 0x00041A6C
	public void ResetInvestigate()
	{
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("AI: Reset Investigate", Actor.HumanDebug.actions);
		}
		this.human.ClearSeesIllegal();
		this.investigateLocation = null;
		this.lastInvestigate = 0f;
		this.CancelPersue();
		this.minimumInvestigationTimeMultiplier = 1f;
		this.human.SetEscalation(0);
		this.SetInvestigationUrgency(NewAIController.InvestigationUrgency.walk);
		if (InteractionController.Instance.mugger == this.human)
		{
			Game.Log("Resetting mugger state", 2);
			InteractionController.Instance.mugger = null;
		}
		if (InteractionController.Instance.debtCollector == this.human)
		{
			Game.Log("Resetting debt collector state", 2);
			InteractionController.Instance.debtCollector = null;
		}
		this.SetReactionState(NewAIController.ReactionState.none);
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00043938 File Offset: 0x00041B38
	public void Patrol(NewGameLocation newPatLoc)
	{
		this.patrolLocation = newPatLoc;
		if (this.patrolGoal == null)
		{
			this.patrolGoal = this.CreateNewGoal(RoutineControls.Instance.patrolGoal, 0f, 0f, null, null, null, null, null, -2);
		}
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0004397C File Offset: 0x00041B7C
	public void StartAttack(Actor newAttackTarget)
	{
		if (newAttackTarget != null && Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug(this.human.name + " Start attack: " + newAttackTarget.name, Actor.HumanDebug.attacks);
		}
		if (this.attackActive || this.throwActive)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("attack already active", Actor.HumanDebug.attacks);
			}
			return;
		}
		if (this.restrained)
		{
			return;
		}
		if (this.attackDelay > 0f)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("attack delay: " + this.attackDelay.ToString(), Actor.HumanDebug.attacks);
			}
			return;
		}
		if (this.currentWeaponPreset.type != MurderWeaponPreset.WeaponType.handgun && this.currentWeaponPreset.type != MurderWeaponPreset.WeaponType.shotgun && this.currentWeaponPreset.type != MurderWeaponPreset.WeaponType.rifle && this.outOfBreath)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("out of breath", Actor.HumanDebug.movement);
			}
			return;
		}
		if (this.activeAttackBar != null && ((this.attackTarget != null && this.attackTarget.isPlayer) || (newAttackTarget != null && newAttackTarget.isPlayer)))
		{
			Object.Destroy(this.activeAttackBar.gameObject);
		}
		this.human.speechController.TriggerBark(SpeechController.Bark.attack);
		this.attackActive = true;
		this.damageColliderCreated = false;
		this.ejectBrassCreated = false;
		this.attackProgress = 0f;
		this.attackActiveLength = 0f;
		this.attackTarget = newAttackTarget;
		Game.Log(this.human.name + ": TRIGGER ATTACK", 2);
		this.human.animationController.AttackTrigger();
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00043B40 File Offset: 0x00041D40
	public void ThrowObject(Actor newAttackTarget)
	{
		if (this.currentGoal != null && this.currentGoal.preset.disableThrowing)
		{
			return;
		}
		if (newAttackTarget != null && Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug(this.human.name + " Start throw: " + newAttackTarget.name, Actor.HumanDebug.attacks);
		}
		if (this.outOfBreath)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("out of breath", Actor.HumanDebug.movement);
			}
			return;
		}
		if (this.restrained)
		{
			return;
		}
		if (this.attackActive || this.throwActive)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("attack already active", Actor.HumanDebug.attacks);
			}
			return;
		}
		if (this.attackDelay > 0f || this.throwDelay > 0f)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("attack delay: " + this.attackDelay.ToString(), Actor.HumanDebug.attacks);
			}
			return;
		}
		this.throwItem = null;
		if (this.human.currentConsumables.Count > 0)
		{
			this.throwItem = this.human.currentConsumables[0];
		}
		else if (this.human.trash.Count > 0)
		{
			MetaObject metaObject = CityData.Instance.FindMetaObject(this.human.trash[0]);
			if (metaObject != null)
			{
				this.throwItem = metaObject.GetPreset();
			}
		}
		if (this.throwItem != null)
		{
			Game.Log("THROW OBJECT " + this.throwItem.name, 2);
			if (this.spawnedRightItem == null)
			{
				Vector3 aiHeldObjectPosition = this.throwItem.aiHeldObjectPosition;
				Vector3 aiHeldObjectRotation = this.throwItem.aiHeldObjectRotation;
				this.usingCarryAnimation = this.throwItem.requiredCarryAnimation;
				int aiCarryAnimation = this.throwItem.aiCarryAnimation;
				this.spawnedRightItem = Toolbox.Instance.SpawnObject(this.throwItem.prefab, this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandRight));
				this.spawnedRightItem.transform.localPosition = aiHeldObjectPosition;
				this.spawnedRightItem.transform.localEulerAngles = aiHeldObjectRotation;
				Collider[] componentsInChildren = this.spawnedRightItem.GetComponentsInChildren<Collider>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Object.Destroy(componentsInChildren[i]);
				}
				this.human.AddMesh(this.spawnedRightItem, true, false);
			}
			this.attackActive = true;
			this.damageColliderCreated = false;
			this.ejectBrassCreated = false;
			this.attackProgress = 0f;
			this.attackActiveLength = 0f;
			this.attackTarget = newAttackTarget;
			this.throwActive = true;
			this.human.animationController.ThrowTrigger();
		}
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00043E00 File Offset: 0x00042000
	public void OnAttackComplete()
	{
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Complete attack", Actor.HumanDebug.attacks);
		}
		if (this.activeAttackBar != null)
		{
			this.activeAttackBar.removeHit = true;
		}
		this.SetAttackDelay(false, false);
		this.EndAttack();
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00043E54 File Offset: 0x00042054
	public void OnAttackBlock(bool perfect = false)
	{
		if (this.activeAttackBar != null)
		{
			this.activeAttackBar.removeBlocked = true;
			this.activeAttackBar.abortProgress = this.attackProgress;
		}
		Game.Log("Player: " + this.human.name + " blocked successfully! Perfect: " + perfect.ToString(), 2);
		this.SetAttackDelay(true, perfect);
		this.human.animationController.BlockTrigger(this.attackDelay, perfect);
		this.EndAttack();
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00043ED8 File Offset: 0x000420D8
	public void OnAbortAttack()
	{
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Abort attack", Actor.HumanDebug.attacks);
		}
		if (this.activeAttackBar != null)
		{
			this.activeAttackBar.removeAbort = true;
			this.activeAttackBar.abortProgress = this.attackProgress;
		}
		this.SetAttackDelay(false, false);
		this.human.animationController.AbortAttackTrigger();
		this.EndAttack();
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00043F4C File Offset: 0x0004214C
	private void SetAttackDelay(bool blocked = false, bool blockedPerfect = false)
	{
		if (this.combatMode <= 0 && blocked)
		{
			if (blockedPerfect)
			{
				this.attackDelay = Mathf.Lerp(GameplayControls.Instance.perfectBlockAttackDelay.x, GameplayControls.Instance.perfectBlockAttackDelay.y, this.human.combatSkill);
			}
			else
			{
				this.attackDelay = Mathf.Lerp(GameplayControls.Instance.blockedAttackDelay.x, GameplayControls.Instance.blockedAttackDelay.y, this.human.combatSkill);
			}
			this.SetDelayed(this.attackDelay);
			return;
		}
		this.attackDelay = this.weaponRefire;
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00043FF0 File Offset: 0x000421F0
	public void EndAttack()
	{
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Ended attack", Actor.HumanDebug.attacks);
		}
		if (this.damageCollider != null)
		{
			Object.Destroy(this.damageCollider.gameObject);
		}
		this.attackActive = false;
		this.throwActive = false;
		this.damageColliderCreated = false;
		this.ejectBrassCreated = false;
		this.attackProgress = 0f;
		this.attackActiveLength = 0f;
		this.attackTimeout = 0f;
		if (!this.inCombat && this.human.animationController != null && this.human.animationController.mainAnimator != null)
		{
			this.human.animationController.mainAnimator.cullingMode = 2;
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x000440C0 File Offset: 0x000422C0
	public void TalkTo(InteractionController.ConversationType convoType = InteractionController.ConversationType.normal)
	{
		if (Player.Instance.playerKOInProgress)
		{
			Game.Log("TalkTo player KO is in progress so igoring...", 2);
			return;
		}
		if (this.human.isDead || this.human.isAsleep || this.human.isStunned)
		{
			Game.Log("TalkTo candidate is dead/asleep/stunned...", 2);
			return;
		}
		if (!this.restrained)
		{
			if (this.inFleeState)
			{
				Game.Log("TalkTo candidate is in flee state...", 2);
				return;
			}
			if (this.reactionState != NewAIController.ReactionState.none)
			{
				Game.Log("TalkTo candidate is in reaction state " + this.reactionState.ToString(), 2);
				return;
			}
		}
		if (Player.Instance.spendingTimeMode)
		{
			Player.Instance.SetSpendingTimeMode(false);
		}
		if (InteractionController.Instance.carryingObject != null)
		{
			InteractionController.Instance.carryingObject.DropThis(false);
		}
		Game.Log("Setting TalkTo", 2);
		this.human.SetInteracting(Player.Instance.interactable);
		Player.Instance.SetInteracting(this.human.interactable);
		if (convoType == InteractionController.ConversationType.mugging)
		{
			InteractionController.Instance.mugger = this.human;
		}
		else if (convoType == InteractionController.ConversationType.loanSharkVisit)
		{
			InteractionController.Instance.debtCollector = this.human;
		}
		InteractionController.Instance.SetLockedInInteractionMode(this.human.interactable, 1, true);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.citizenTalkToTransition, null, this.human.interactable, this.human.lookAtThisTransform, false, false, 0f, false, default(Vector3), 1f, true);
		InteractionController.Instance.SetDialog(true, this.human.interactable, false, null, convoType);
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromTalkTo;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00044280 File Offset: 0x00042480
	public void OnReturnFromTalkTo()
	{
		if (InteractionController.Instance.dialogType == InteractionController.ConversationType.mugging)
		{
			if (InteractionController.Instance.mugger != null)
			{
				Game.Log("Setting mugging combat persuit...", 2);
				InteractionController.Instance.mugger.ai.SetPersue(Player.Instance, false, 2, true, 10f);
			}
		}
		else if (InteractionController.Instance.dialogType == InteractionController.ConversationType.loanSharkVisit && InteractionController.Instance.debtCollector != null)
		{
			Game.Log("Setting debt collector combat persuit...", 2);
			InteractionController.Instance.debtCollector.ai.SetPersue(Player.Instance, false, 2, true, 10f);
		}
		this.human.SetInteracting(null);
		InteractionController.Instance.SetDialog(false, null, false, null, InteractionController.ConversationType.normal);
		this.AITick(false, false);
		this.SetUpdateEnabled(true);
		if (this.currentAction != null)
		{
			this.currentAction.OnDeactivate(false);
		}
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromTalkTo;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		Player.Instance.ReturnFromTransform(false, true);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00044392 File Offset: 0x00042592
	public void SetStunned(bool val)
	{
		Game.Log("AI: Set Stunned " + val.ToString(), 2);
		this.human.isStunned = val;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x000443B7 File Offset: 0x000425B7
	public void SetDelayed(float seconds)
	{
		this.human.isDelayed = true;
		this.delayedUntil = SessionData.Instance.gameTime + seconds * 0.0167f;
		if (this.currentAction != null)
		{
			this.currentAction.estimatedArrival = -1f;
		}
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x000443F8 File Offset: 0x000425F8
	public void AnswerDoor(NewDoor dc, NewGameLocation where, Actor byWho)
	{
		if (this.human.isDead || dc == null)
		{
			return;
		}
		NewNode node = dc.wall.node;
		if (dc.wall.otherWall.node.gameLocation == where)
		{
			node = dc.wall.otherWall.node;
		}
		if (!this.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.answerDoorGoal && item.passedInteractable == dc.handleInteractable))
		{
			Game.Log("Debug: " + this.human.name + " add answer door event", 2);
			this.CreateNewGoal(RoutineControls.Instance.answerDoorGoal, 0f, 0f, node, dc.handleInteractable, null, null, null, -2);
			this.AITick(true, false);
			if (byWho != null && byWho.isPlayer)
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Somebody is coming to answer the door...", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
				return;
			}
		}
		else if (SessionData.Instance.gameTime - this.human.speechController.lastSpeech > 0.0167f)
		{
			this.human.speechController.TriggerBark(SpeechController.Bark.answeringDoor);
			this.human.ai.AITick(true, false);
		}
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00044578 File Offset: 0x00042778
	public void AnswerPhone(Telephone where)
	{
		if (this.human.isDead || this.human.isStunned || this.inCombat)
		{
			return;
		}
		if (this.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.mourn))
		{
			return;
		}
		if (this.currentGoal != null && (this.investigationGoal == null || !this.investigationGoal.isActive))
		{
			if (this.currentAction != null)
			{
				if (this.currentGoal.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.answerTelephone))
				{
					return;
				}
			}
			Interactable interactable = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.answerTelephone, this.human.currentRoom, this.human, AIActionPreset.FindSetting.nonTrespassing, true, null, where.location, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
			if (interactable != null)
			{
				this.human.SetInConversation(null, true);
				Game.Log("Phone: Sending " + this.human.name + " to answer phone at " + where.location.name, 2);
				this.CreateNewAction(this.currentGoal, RoutineControls.Instance.answerTelephone, true, null, interactable, null, null, null, false, 9, "");
				this.AITick(false, false);
			}
		}
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x000446E0 File Offset: 0x000428E0
	public void AwakenPrompt()
	{
		if (this.human.awakenPromt < 2)
		{
			this.human.awakenPromt++;
		}
		this.human.sleepDepth -= 0.12f;
		this.human.sleepDepth = Mathf.Clamp01(this.human.sleepDepth);
		if (this.human.awakenPromt >= 2)
		{
			this.human.WakeUp(false);
			return;
		}
		if (this.human.isAsleep)
		{
			if (this.human.genderScale >= 0.5f)
			{
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.maleSnort, this.human, this.human.currentNode, this.human.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
				return;
			}
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.femaleSnort, this.human, this.human.currentNode, this.human.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
		}
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00044800 File Offset: 0x00042A00
	[Button(null, 0)]
	public void DisplayCurrentRoute()
	{
		if (this.currentAction != null && this.currentAction.path != null)
		{
			for (int i = 0; i < this.currentAction.path.accessList.Count; i++)
			{
				NewNode.NodeAccess nodeAccess = this.currentAction.path.accessList[i];
				Debug.DrawRay(nodeAccess.fromNode.position, nodeAccess.toNode.position - nodeAccess.fromNode.position, Color.magenta, 5f);
			}
		}
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00044890 File Offset: 0x00042A90
	public void SetInCombat(bool val, bool forceUpdate = false)
	{
		if (this.inCombat != val || forceUpdate)
		{
			this.inCombat = val;
			if (this.inCombat)
			{
				this.human.SetEscalation(2);
				this.RecalculateWeaponStats();
				this.throwDelay = Toolbox.Instance.Rand(2f, 4f, false);
				this.human.AddNerve(-0.1f, null);
			}
			else
			{
				this.human.AddNerve(0.1f, null);
			}
			this.human.animationController.SetInCombat(this.inCombat);
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug(this.human.name + " in combat: " + val.ToString(), Actor.HumanDebug.actions);
			}
			this.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00044964 File Offset: 0x00042B64
	public void RecalculateWeaponStats()
	{
		if (this.currentWeaponPreset != null)
		{
			this.weaponRangeMax = this.currentWeaponPreset.GetAttackValue(MurderWeaponPreset.AttackValue.range, this.human);
			this.weaponRefire = this.currentWeaponPreset.GetAttackValue(MurderWeaponPreset.AttackValue.fireDelay, this.human);
			this.weaponAccuracy = this.currentWeaponPreset.GetAttackValue(MurderWeaponPreset.AttackValue.accuracy, this.human);
			this.weaponDamage = this.currentWeaponPreset.GetAttackValue(MurderWeaponPreset.AttackValue.damage, this.human);
		}
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x000449E0 File Offset: 0x00042BE0
	public void SetKO(bool val, Vector3 impactPoint = default(Vector3), Vector3 impactDirection = default(Vector3), bool forced = false, float forcedDuration = 0f, bool resetInvesigate = true, float forceMultiplier = 1f)
	{
		Game.Log("AI: Set KO " + val.ToString(), 2);
		this.ko = val;
		this.SetStunned(this.ko);
		this.SetUpdateEnabled(true);
		this.TriggerReactionIndicator();
		if (this.ko)
		{
			if (resetInvesigate)
			{
				this.ResetInvestigate();
			}
			if (this.human.inConversation)
			{
				this.human.currentConversation.EndConversation();
			}
			if (this.attackActive)
			{
				Game.Log(this.human.name + " Abort attack: KO", 2);
				this.OnAbortAttack();
			}
			this.queuedActions.Clear();
			if (!forced)
			{
				this.human.ResetHealthToMaximum();
				this.koTime = SessionData.Instance.gameTime + Toolbox.Instance.Rand(GameplayControls.Instance.koTimeRange.x, GameplayControls.Instance.koTimeRange.y, false) * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.KOTimeModifier));
				Game.Log(this.human.name + " Set KO time: " + this.koTime.ToString(), 2);
			}
			else
			{
				this.koTime = SessionData.Instance.gameTime + forcedDuration;
				Game.Log(this.human.name + " Set KO time: " + this.koTime.ToString(), 2);
			}
			this.koTransitionTimer = CitizenControls.Instance.ragdollTransitionTime;
			this.getUpDelayTimer = CitizenControls.Instance.getUpTimer;
			this.human.animationController.SetPauseAnimation(true);
			this.human.animationController.SetRagdoll(true, this.human.isDead);
			if (!this.human.isDead)
			{
				Rigidbody rigidbody = this.human.animationController.upperTorsoRB;
				float num = float.PositiveInfinity;
				foreach (Rigidbody rigidbody2 in this.human.animationController.createdRBs)
				{
					float num2 = Vector3.Distance(rigidbody2.transform.position, impactPoint);
					if (num2 < num)
					{
						rigidbody = rigidbody2;
						num = num2;
					}
				}
				rigidbody.AddForceAtPosition(impactDirection.normalized * GameplayControls.Instance.playerKOPunchForce * forceMultiplier, impactPoint);
			}
		}
		else
		{
			if (resetInvesigate)
			{
				this.human.speechController.TriggerBark(SpeechController.Bark.dazed);
			}
			foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
			{
				newBuilding.alarmTargets.Remove(this.human);
			}
		}
		if (this.human.interactable != null)
		{
			this.human.interactable.UpdateCurrentActions();
			if (InteractionController.Instance.currentLookingAtInteractable != null && InteractionController.Instance.currentLookingAtInteractable.interactable == this.human.interactable)
			{
				InteractionController.Instance.UpdateInteractionText();
			}
		}
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00044D08 File Offset: 0x00042F08
	public void SetOutOfBreath(bool val)
	{
		if (this.outOfBreath != val)
		{
			this.outOfBreath = val;
			this.human.animationController.mainAnimator.SetBool("outOfBreath", this.outOfBreath);
			if (this.outOfBreath && Toolbox.Instance.Rand(0f, 1f, false) > 0.5f)
			{
				this.human.speechController.TriggerBark(SpeechController.Bark.outOfBreath);
			}
		}
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00044D7C File Offset: 0x00042F7C
	public void SetRestrained(bool val, float duration)
	{
		if (val != this.restrained)
		{
			this.restrained = val;
			Game.Log("AI: Set restrained " + this.human.name + ": " + this.restrained.ToString(), 2);
			if (this.restrained)
			{
				if (this.ko && !this.human.isDead)
				{
					this.koTime = 0f;
				}
				this.human.speechController.TriggerBark(SpeechController.Bark.restrained);
				this.human.animationController.SetInBed(false, false);
				this.human.animationController.SetUmbrella(false);
				Game.Log(this.human.name + " Abort attack: Restrained", 2);
				this.OnAbortAttack();
				this.SetFacingDirection(Player.Instance.transform.forward);
				this.SetExpression(CitizenOutfitController.Expression.angry);
				this.human.animationController.mainAnimator.SetTrigger("restrain");
				this.restrainTime = SessionData.Instance.gameTime + duration;
				this.MovementSpeedUpdate();
			}
			else
			{
				this.restrainTime = 0f;
			}
			this.human.animationController.SetRestrained(this.restrained);
			foreach (Case @case in CasePanelController.Instance.activeCases)
			{
				foreach (Case.ResolveQuestion resolveQuestion in @case.resolveQuestions)
				{
					if (resolveQuestion.inputType == Case.InputType.arrestPurp || resolveQuestion.inputType == Case.InputType.saveVictim)
					{
						if (@case.caseType == Case.CaseType.murder || @case.caseType == Case.CaseType.mainStory)
						{
							MurderController.Instance.UpdateCorrectResolveAnswers();
						}
						else if (@case.caseType == Case.CaseType.sideJob && @case.job != null)
						{
							@case.job.UpdateResolveAnswers();
						}
					}
				}
			}
		}
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00044F88 File Offset: 0x00043188
	public void SetReactionState(NewAIController.ReactionState newState)
	{
		if (this.human.isDead)
		{
			newState = NewAIController.ReactionState.none;
		}
		if (newState != this.reactionState)
		{
			this.reactionState = newState;
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("New reaction state: " + newState.ToString(), Actor.HumanDebug.updates);
			}
		}
		this.TriggerReactionIndicator();
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00044FEC File Offset: 0x000431EC
	public void TriggerReactionIndicator()
	{
		if (this.reactionIndicator != null)
		{
			this.reactionIndicator.removeFade = false;
		}
		if (this.human.spottedState <= 0f)
		{
			if (this.reactionState == NewAIController.ReactionState.persuing && (this.persuitTarget == null || this.persuitTarget != Player.Instance))
			{
				if (this.reactionIndicator != null)
				{
					this.reactionIndicator.removeFade = true;
				}
				return;
			}
			if (this.reactionState == NewAIController.ReactionState.investigatingSound && (this.hearTarget == null || !this.hearTarget.isPlayer))
			{
				if (this.reactionIndicator != null)
				{
					this.reactionIndicator.removeFade = true;
				}
				return;
			}
			if (this.reactionState == NewAIController.ReactionState.investigatingSight && (this.persuitTarget == null || !this.persuitTarget.isPlayer))
			{
				if (this.reactionIndicator != null)
				{
					this.reactionIndicator.removeFade = true;
				}
				return;
			}
			if ((this.reactionState == NewAIController.ReactionState.none && this.investigationGoal.priority <= 0f) || this.ko)
			{
				if (this.reactionIndicator != null)
				{
					this.reactionIndicator.removeFade = true;
				}
				return;
			}
			if ((this.reactionState == NewAIController.ReactionState.none || this.reactionState == NewAIController.ReactionState.searching) && this.human.escalationLevel <= 1)
			{
				if (this.reactionIndicator != null)
				{
					this.reactionIndicator.removeFade = true;
				}
				return;
			}
			if (this.human.escalationLevel >= 2 && this.persuitTarget != Player.Instance && this.hearTarget != Player.Instance)
			{
				if (this.reactionIndicator != null)
				{
					this.reactionIndicator.removeFade = true;
				}
				return;
			}
			if (this.reactionIndicator != null)
			{
				this.reactionIndicator.UpdateReactionType();
				return;
			}
		}
		if (this.reactionIndicator != null)
		{
			this.reactionIndicator.UpdateReactionType();
		}
		else
		{
			Game.Log("Interface: Create reaction icon for " + this.human.name, 2);
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.aiReactionIndicator, InterfaceControls.Instance.speechBubbleParent);
			this.reactionIndicator = gameObject.GetComponent<ReactionIndicatorController>();
			this.reactionIndicator.Setup(this.human);
			if (this.human.outline != null)
			{
				this.human.outline.SetOutlineActive(true);
			}
		}
		if (this.reactionIndicator.awarenessIcon == null || !this.reactionIndicator.awarenessIcon.setup)
		{
			Game.Log("Interface: Create awareness icon for " + this.human.name, 2);
			this.reactionIndicator.awarenessIcon = InterfaceController.Instance.AddAwarenessIcon(InterfaceController.AwarenessType.actor, InterfaceController.AwarenessBehaviour.invisibleInfront, this.human, null, Vector3.zero, InterfaceControls.Instance.spotted, 10, true, InterfaceControls.Instance.maxIndicatorDistance);
			this.reactionIndicator.UpdateReactionType();
		}
		else if (this.reactionIndicator.awarenessIcon.removalFlag)
		{
			this.reactionIndicator.awarenessIcon.removalFlag = false;
			this.reactionIndicator.awarenessIcon.removalProgress = 0f;
		}
		if (this.reactionIndicator.awarenessIcon != null && (this.reactionState == NewAIController.ReactionState.investigatingSight || this.reactionState == NewAIController.ReactionState.investigatingSound || this.reactionState == NewAIController.ReactionState.persuing))
		{
			this.reactionIndicator.awarenessIcon.TriggerAlert();
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0004534C File Offset: 0x0004354C
	public void DebugDestinationPosition(string input)
	{
		if (!Game.Instance.devMode || !Game.Instance.collectDebugData)
		{
			return;
		}
		if (this.debugDestinationPosition.Count > 50)
		{
			this.debugDestinationPosition.RemoveAt(0);
		}
		this.debugDestinationPosition.Add(input);
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x00045399 File Offset: 0x00043599
	public void CancelCombat()
	{
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Cancel combat", Actor.HumanDebug.actions);
		}
		this.human.ClearSeesIllegal();
		this.ResetInvestigate();
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x000453CC File Offset: 0x000435CC
	public void SetAsVictim(MurderController.Murder newMurder)
	{
		if (!this.victimsForMurders.Contains(newMurder))
		{
			if (Game.Instance.collectDebugData)
			{
				Game.Log("Murder: Add victim allowance: " + this.human.name, 2);
			}
			this.victimsForMurders.Add(newMurder);
		}
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0004541A File Offset: 0x0004361A
	public void SetAsMurderer(MurderController.Murder newMurderer)
	{
		if (!this.killerForMurders.Contains(newMurderer))
		{
			Game.Log("Murder: Add killer allowance: " + this.human.name, 2);
			this.killerForMurders.Add(newMurderer);
		}
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00045454 File Offset: 0x00043654
	public void SetStaticFromAnimation(bool val)
	{
		if (val != this.staticFromAnimation)
		{
			this.staticFromAnimation = val;
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Set static from animation: " + this.staticFromAnimation.ToString(), Actor.HumanDebug.movement);
			}
		}
		if (this.staticFromAnimation)
		{
			this.staticAnimationSafetyTimer = 2.5f;
		}
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x000454B4 File Offset: 0x000436B4
	[Button(null, 0)]
	public void GetRotationState()
	{
		Game.Log(string.Concat(new string[]
		{
			"Character rotation is ",
			base.transform.rotation.ToString(),
			" (",
			base.transform.eulerAngles.ToString(),
			")"
		}), 2);
		string[] array = new string[5];
		array[0] = "Desired rotation is ";
		int num = 1;
		Quaternion quaternion = this.facingQuat;
		array[num] = quaternion.ToString();
		array[2] = " (";
		array[3] = this.facingQuat.eulerAngles.ToString();
		array[4] = ")";
		Game.Log(string.Concat(array), 2);
		Game.Log("Rotation status is: " + this.facingActive.ToString(), 2);
		Game.Log("This enabled: " + base.enabled.ToString(), 2);
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x000455B7 File Offset: 0x000437B7
	public void CloseDoorsNormallyAfterLeavingGamelocation(NewGameLocation afterLeaving)
	{
		this.closeDoorsNormallyAfterLeaving = afterLeaving;
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x000455C0 File Offset: 0x000437C0
	public void UpdateCurrentWeapon()
	{
		Interactable interactable = null;
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("Updating current weapon, killer for murders: " + this.killerForMurders.Count.ToString(), Actor.HumanDebug.actions);
		}
		foreach (Interactable interactable2 in this.human.inventory)
		{
			if (interactable2 != null && !(interactable2.preset == null) && interactable2.preset.weapon != null)
			{
				if (interactable2.preset.weapon.usedInPersonalDefence && (interactable == null || interactable.preset == null || (interactable != null && interactable.preset != null && interactable.preset.weapon != null && interactable2.preset.weapon.basePriority > interactable.preset.weapon.basePriority)))
				{
					interactable = interactable2;
				}
				bool flag = false;
				foreach (MurderController.Murder murder in this.killerForMurders)
				{
					if (murder != null && (murder.state == MurderController.MurderState.travellingTo || murder.state == MurderController.MurderState.executing || murder.state == MurderController.MurderState.post || murder.state == MurderController.MurderState.escaping) && murder.murdererID == this.human.humanID && murder.weaponPreset == interactable2.preset)
					{
						interactable = interactable2;
						flag = true;
						if (Game.Instance.collectDebugData)
						{
							Actor actor = this.human;
							string text = "Detected killer should be using weapon: ";
							Interactable interactable3 = interactable;
							actor.SelectedDebug(text + ((interactable3 != null) ? interactable3.ToString() : null), Actor.HumanDebug.actions);
							break;
						}
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		this.SetCurrentWeapon(interactable);
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x000457E0 File Offset: 0x000439E0
	public void SetCurrentWeapon(Interactable obj)
	{
		if (obj != this.currentWeapon)
		{
			if (obj != null && obj.preset.weapon != null)
			{
				this.currentWeapon = obj;
				this.currentWeaponPreset = obj.preset.weapon;
			}
			else
			{
				this.currentWeapon = null;
				this.currentWeaponPreset = InteriorControls.Instance.fistsWeapon;
			}
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("Set current weapon to " + this.currentWeaponPreset.name, Actor.HumanDebug.attacks);
			}
			this.RecalculateWeaponStats();
			if (this.currentWeaponPreset.type == MurderWeaponPreset.WeaponType.fists)
			{
				this.combatMode = 0;
			}
			else if (this.currentWeaponPreset.type == MurderWeaponPreset.WeaponType.handgun)
			{
				this.combatMode = 1;
			}
			else if (this.currentWeaponPreset.type == MurderWeaponPreset.WeaponType.rifle)
			{
				this.combatMode = 2;
			}
			else if (this.currentWeaponPreset.type == MurderWeaponPreset.WeaponType.shotgun)
			{
				this.combatMode = 3;
			}
			else if (this.currentWeaponPreset.type == MurderWeaponPreset.WeaponType.blade || this.currentWeaponPreset.type == MurderWeaponPreset.WeaponType.poison || this.currentWeaponPreset.type == MurderWeaponPreset.WeaponType.bluntObject || this.currentWeaponPreset.type == MurderWeaponPreset.WeaponType.strangulation)
			{
				this.combatMode = 4;
			}
			else
			{
				this.combatMode = 0;
			}
			this.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
		}
		this.human.animationController.SetCombatArmsOverride(this.combatMode);
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00045938 File Offset: 0x00043B38
	public void UpdateHeldItems(AIActionPreset.ActionStateFlag state)
	{
		if (Game.Instance.collectDebugData)
		{
			this.human.SelectedDebug("UpdateHeldItems: " + state.ToString(), Actor.HumanDebug.actions);
		}
		GameObject gameObject = null;
		Vector3 localPosition = Vector3.zero;
		Vector3 localEulerAngles = Vector3.zero;
		GameObject gameObject2 = null;
		Vector3 localPosition2 = Vector3.zero;
		Vector3 localEulerAngles2 = Vector3.zero;
		this.usingCarryAnimation = false;
		int carryItemType = 0;
		if (this.inCombat)
		{
			if (this.currentWeaponPreset.itemRightOverride != null)
			{
				gameObject = this.currentWeaponPreset.itemRightOverride;
				localPosition = this.currentWeaponPreset.itemRightLocalPos;
				localEulerAngles = this.currentWeaponPreset.itemRightLocalEuler;
				carryItemType = this.currentWeaponPreset.overrideCarryAnimation;
				this.usingCarryAnimation = this.currentWeaponPreset.overideUsesCarryAnimation;
			}
			else
			{
				gameObject = null;
			}
			if (this.currentWeaponPreset.itemLeftOverride != null)
			{
				gameObject2 = this.currentWeaponPreset.itemLeftOverride;
				localPosition2 = this.currentWeaponPreset.itemLeftLocalPos;
				localEulerAngles2 = this.currentWeaponPreset.itemLeftLocalEuler;
				carryItemType = this.currentWeaponPreset.overrideCarryAnimation;
				this.usingCarryAnimation = this.currentWeaponPreset.overideUsesCarryAnimation;
			}
			else
			{
				gameObject2 = null;
			}
		}
		else if (this.currentAction != null && !this.currentAction.preset.allowItems)
		{
			gameObject = null;
			gameObject2 = null;
			this.customItemSource = null;
		}
		else
		{
			if (this.customItemSource != null)
			{
				if (this.customItemSource.preset.destroyCustomItemOn == state)
				{
					if (state == AIActionPreset.ActionStateFlag.onDeactivation && (!this.customItemSource.isActive || this.currentAction != this.customItemSource))
					{
						if (Game.Instance.collectDebugData)
						{
							this.human.SelectedDebug("Set custom item source to null: " + state.ToString(), Actor.HumanDebug.actions);
						}
						this.customItemSource = null;
					}
					else if (state == AIActionPreset.ActionStateFlag.onGoalDeactivation && (!this.customItemSource.goal.isActive || this.currentGoal != this.customItemSource.goal))
					{
						if (Game.Instance.collectDebugData)
						{
							this.human.SelectedDebug("Set custom item source to null: " + state.ToString(), Actor.HumanDebug.actions);
						}
						this.customItemSource = null;
					}
				}
				if (this.customItemSource != null && Game.Instance.collectDebugData)
				{
					this.human.SelectedDebug("Has custom item source: " + this.customItemSource.name, Actor.HumanDebug.actions);
				}
			}
			if (this.customItemSource == null)
			{
				if (this.currentAction != null && this.currentAction.preset.enableCustomItem && this.currentAction.preset.spawnCustomItemOn == state)
				{
					if (Game.Instance.collectDebugData)
					{
						this.human.SelectedDebug("Spawning new custom item from within action: " + state.ToString() + " and setting new custom item source from this action", Actor.HumanDebug.actions);
					}
					gameObject2 = this.currentAction.preset.itemLeft;
					localPosition2 = this.currentAction.preset.itemLeftLocalPos;
					localEulerAngles2 = this.currentAction.preset.itemLeftLocalEuler;
					this.usingCarryAnimation = this.currentAction.preset.requiresCarryAnimation;
					carryItemType = this.currentAction.preset.overrideCarryAnimation;
					gameObject = this.currentAction.preset.itemRight;
					localPosition = this.currentAction.preset.itemRightLocalPos;
					localEulerAngles = this.currentAction.preset.itemRightLocalEuler;
					this.customItemSource = this.currentAction;
				}
				else
				{
					bool flag = false;
					if (this.human.inventory.Count > 0)
					{
						foreach (Interactable interactable in this.human.inventory)
						{
							if (interactable.preset.inventoryCarryItem)
							{
								gameObject2 = null;
								gameObject = interactable.preset.prefab;
								localPosition = interactable.preset.aiHeldObjectPosition;
								localEulerAngles = interactable.preset.aiHeldObjectRotation;
								this.usingCarryAnimation = interactable.preset.requiredCarryAnimation;
								carryItemType = interactable.preset.aiCarryAnimation;
								if (Game.Instance.collectDebugData)
								{
									Actor actor = this.human;
									string text = "Carrying item from inventory: ";
									GameObject gameObject3 = gameObject;
									actor.SelectedDebug(text + ((gameObject3 != null) ? gameObject3.ToString() : null), Actor.HumanDebug.actions);
								}
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						if (this.human.currentConsumables.Count > 0)
						{
							gameObject2 = null;
							gameObject = this.human.currentConsumables[0].prefab;
							localPosition = this.human.currentConsumables[0].aiHeldObjectPosition;
							localEulerAngles = this.human.currentConsumables[0].aiHeldObjectRotation;
							this.usingCarryAnimation = this.human.currentConsumables[0].requiredCarryAnimation;
							carryItemType = this.human.currentConsumables[0].aiCarryAnimation;
							if (Game.Instance.collectDebugData)
							{
								Actor actor2 = this.human;
								string text2 = "Carrying item from consumables: ";
								GameObject gameObject4 = gameObject;
								actor2.SelectedDebug(text2 + ((gameObject4 != null) ? gameObject4.ToString() : null), Actor.HumanDebug.actions);
							}
						}
						else if (this.human.trash.Count > 0)
						{
							gameObject2 = null;
							MetaObject metaObject = CityData.Instance.FindMetaObject(this.human.trash[0]);
							if (metaObject != null)
							{
								InteractablePreset preset = metaObject.GetPreset();
								if (preset != null && preset.prefab != null)
								{
									gameObject = preset.prefab;
									localPosition = preset.aiHeldObjectPosition;
									localEulerAngles = preset.aiHeldObjectRotation;
									this.usingCarryAnimation = preset.requiredCarryAnimation;
									carryItemType = preset.aiCarryAnimation;
									if (Game.Instance.collectDebugData)
									{
										Actor actor3 = this.human;
										string text3 = "Carrying item from consumables: ";
										GameObject gameObject5 = gameObject;
										actor3.SelectedDebug(text3 + ((gameObject5 != null) ? gameObject5.ToString() : null), Actor.HumanDebug.actions);
									}
								}
								else
								{
									gameObject2 = null;
									gameObject = null;
								}
							}
							else
							{
								gameObject2 = null;
								gameObject = null;
							}
						}
						else
						{
							gameObject2 = null;
							gameObject = null;
						}
					}
				}
			}
		}
		bool flag2 = false;
		if (this.customItemSource == null)
		{
			if (this.spawnedRightItem != null && (gameObject == null || gameObject.name != this.spawnedRightItem.name))
			{
				this.DespawnRightItem();
				flag2 = true;
			}
			if (this.spawnedLeftItem != null && (gameObject2 == null || gameObject2.name != this.spawnedLeftItem.name))
			{
				this.DespawnLeftItem();
				flag2 = true;
			}
		}
		if (gameObject != null)
		{
			this.human.SelectedDebug("Spawning desired item right: " + gameObject.name, Actor.HumanDebug.actions);
			this.DespawnRightItem();
			this.spawnedRightItem = Toolbox.Instance.SpawnObject(gameObject, this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandRight));
			this.spawnedRightItem.transform.localPosition = localPosition;
			this.spawnedRightItem.transform.localEulerAngles = localEulerAngles;
			Collider[] componentsInChildren = this.spawnedRightItem.GetComponentsInChildren<Collider>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i]);
			}
			LODGroup componentInChildren = this.spawnedRightItem.GetComponentInChildren<LODGroup>(true);
			if (componentInChildren != null)
			{
				componentInChildren.SetLODs(new LOD[0]);
				Object.Destroy(componentInChildren);
			}
			InteractableController componentInChildren2 = this.spawnedRightItem.GetComponentInChildren<InteractableController>(true);
			if (componentInChildren2 != null)
			{
				Object.Destroy(componentInChildren2);
			}
			this.human.AddMesh(this.spawnedRightItem, true, false);
			flag2 = true;
		}
		else if (this.spawnedRightItem != null)
		{
			this.DespawnRightItem();
		}
		if (gameObject2 != null)
		{
			this.human.SelectedDebug("Spawning desired item right: " + gameObject2.name, Actor.HumanDebug.actions);
			this.DespawnLeftItem();
			this.spawnedLeftItem = Toolbox.Instance.SpawnObject(gameObject2, this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandLeft));
			this.spawnedLeftItem.transform.localPosition = localPosition2;
			this.spawnedLeftItem.transform.localEulerAngles = localEulerAngles2;
			Collider[] componentsInChildren2 = this.spawnedLeftItem.GetComponentsInChildren<Collider>(true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Object.Destroy(componentsInChildren2[j]);
			}
			LODGroup componentInChildren3 = this.spawnedLeftItem.GetComponentInChildren<LODGroup>(true);
			if (componentInChildren3 != null)
			{
				componentInChildren3.SetLODs(new LOD[0]);
				Object.Destroy(componentInChildren3);
			}
			InteractableController componentInChildren4 = this.spawnedLeftItem.GetComponentInChildren<InteractableController>(true);
			if (componentInChildren4 != null)
			{
				Object.Destroy(componentInChildren4);
			}
			this.human.AddMesh(this.spawnedLeftItem, true, false);
			flag2 = true;
		}
		else if (this.spawnedLeftItem != null)
		{
			this.DespawnLeftItem();
		}
		if (flag2)
		{
			if (this.usingCarryAnimation)
			{
				this.human.animationController.SetCarryItemType(carryItemType);
				this.human.animationController.SetCarryingItem(true);
				if (this.human.animationController.armsBoolAnimationState == CitizenAnimationController.ArmsBoolSate.none)
				{
					this.human.animationController.armsLayerDesiredWeight = 1f;
					if (Game.Instance.collectDebugData)
					{
						this.human.SelectedDebug("Set animator arms weight: 1 (" + this.human.animationController.armsLayerDesiredWeight.ToString() + ")", Actor.HumanDebug.misc);
						return;
					}
				}
			}
			else
			{
				this.human.animationController.SetCarryingItem(false);
				this.human.animationController.SetCarryItemType(0);
				this.human.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
			}
		}
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x000462B0 File Offset: 0x000444B0
	public void DespawnRightItem()
	{
		if (this.spawnedRightItem != null)
		{
			this.human.SelectedDebug("Removing item right: " + this.spawnedRightItem.name, Actor.HumanDebug.actions);
			Toolbox.Instance.DestroyObject(this.spawnedRightItem);
			this.human.UpdateMeshList();
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x00046308 File Offset: 0x00044508
	public void DespawnLeftItem()
	{
		if (this.spawnedLeftItem != null)
		{
			this.human.SelectedDebug("Removing item right: " + this.spawnedLeftItem.name, Actor.HumanDebug.actions);
			Toolbox.Instance.DestroyObject(this.spawnedLeftItem);
			this.human.UpdateMeshList();
		}
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00046360 File Offset: 0x00044560
	public void InstantPersuitCheck(Actor target)
	{
		if (target.isPlayer && Game.Instance.invisiblePlayer)
		{
			return;
		}
		if (!this.human.isStunned && !this.human.isDead && !target.isStunned && !target.isDead && (target.illegalStatus || this.persuitTarget == target || (this.human.currentBuilding != null && target.isPlayer && this.human.currentBuilding.wantedInBuilding > SessionData.Instance.gameTime && this.human.locationsOfAuthority.Contains(Player.Instance.currentGameLocation))))
		{
			target.UpdateTrespassing(false);
			RaycastHit raycastHit;
			if ((!target.isTrespassing || target.trespassingEscalation >= 2) && this.human.ActorRaycastCheck(target, 3f, out raycastHit, false, Color.green, Color.red, Color.white, 1f))
			{
				Game.Log(this.human.name + " set " + ((target != null) ? target.ToString() : null) + " instant persuit!", 2);
				this.SetPersue(target, false, 1, true, 10f);
			}
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x000464A8 File Offset: 0x000446A8
	public void EnableAI(bool val)
	{
		if (val)
		{
			if (!CitizenBehaviour.Instance.updateList.Contains(this))
			{
				CitizenBehaviour.Instance.updateList.Add(this);
			}
			this.SetDesiredTickRate(NewAIController.AITickRate.high, true);
			this.SetUpdateEnabled(true);
			return;
		}
		this.SetPersuit(false);
		this.ResetInvestigate();
		this.CancelCombat();
		if (this.currentAction != null)
		{
			this.currentAction.Remove(0f);
		}
		if (this.currentGoal != null)
		{
			this.currentGoal.Remove();
		}
		CitizenBehaviour.Instance.updateList.Remove(this);
		CitizenBehaviour.Instance.veryHighTickRate.Remove(this);
		CitizenBehaviour.Instance.highTickRate.Remove(this);
		CitizenBehaviour.Instance.mediumTickRate.Remove(this);
		CitizenBehaviour.Instance.lowTickRate.Remove(this);
		CitizenBehaviour.Instance.veryLowTickRate.Remove(this);
		this.queuedActions.Clear();
		this.SetReactionState(NewAIController.ReactionState.none);
		if (!this.isRagdoll)
		{
			this.SetUpdateEnabled(false);
		}
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x000465AF File Offset: 0x000447AF
	public void SetConfineLocation(NewGameLocation newConfine)
	{
		this.confineLocation = newConfine;
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x000465B8 File Offset: 0x000447B8
	public void AddAvoidLocation(NewGameLocation newAvoid)
	{
		if (this.avoidLocations.Contains(newAvoid))
		{
			this.avoidLocations.Add(newAvoid);
		}
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x000465D4 File Offset: 0x000447D4
	public void RemoveAvoidLocation(NewGameLocation remAvoid)
	{
		this.avoidLocations.Remove(remAvoid);
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x000465E4 File Offset: 0x000447E4
	public NewGameLocation CheckConfinedLocation(NewGameLocation desired)
	{
		if (this.confineLocation != null && this.confineLocation != desired)
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug(string.Concat(new string[]
				{
					"AI ",
					this.human.name,
					" is confined to location ",
					this.confineLocation.name,
					", removing action for ",
					desired.name,
					"..."
				}), Actor.HumanDebug.actions);
				this.DebugDestinationPosition(string.Concat(new string[]
				{
					"AI ",
					this.human.name,
					" is confined to location ",
					this.confineLocation.name,
					", removing action for ",
					desired.name,
					"..."
				}));
			}
			return this.confineLocation;
		}
		if (this.avoidLocations != null && this.avoidLocations.Contains(desired))
		{
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug(string.Concat(new string[]
				{
					"AI ",
					this.human.name,
					" is avoiding location ",
					desired.name,
					", removing action for ",
					desired.name,
					"..."
				}), Actor.HumanDebug.actions);
				this.DebugDestinationPosition(string.Concat(new string[]
				{
					"AI ",
					this.human.name,
					" is avoiding location ",
					desired.name,
					", removing action for ",
					desired.name,
					"..."
				}));
			}
			return null;
		}
		if (this.victimsForMurders.Count > 0)
		{
			foreach (MurderController.Murder murder in this.victimsForMurders)
			{
				if (!(murder.location == null) && murder.state >= MurderController.MurderState.travellingTo && murder.preset.blockVictimFromLeavingLocation && murder.location != desired)
				{
					Game.Log(string.Concat(new string[]
					{
						"Murder: AI ",
						this.human.name,
						" is confined to location ",
						murder.location.name,
						", removing action for ",
						desired.name,
						"..."
					}), 2);
					this.human.SelectedDebug(string.Concat(new string[]
					{
						"AI ",
						this.human.name,
						" is confined to location ",
						murder.location.name,
						", removing action for ",
						desired.name,
						"..."
					}), Actor.HumanDebug.actions);
					this.DebugDestinationPosition(string.Concat(new string[]
					{
						"AI ",
						this.human.name,
						" is confined to location ",
						murder.location.name,
						", removing action for ",
						desired.name,
						"..."
					}));
					return murder.location;
				}
			}
			return desired;
		}
		return desired;
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x00046964 File Offset: 0x00044B64
	public void AddSpooked(float val)
	{
		this.spooked += val;
		this.spooked = Mathf.Clamp01(this.spooked);
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x00046988 File Offset: 0x00044B88
	[Button(null, 0)]
	public void IsTrespassingAtActionDestination()
	{
		if (this.currentAction != null)
		{
			string empty = string.Empty;
			int num;
			bool flag = this.human.IsTrespassing(this.currentAction.goal.passedInteractable.node.room, out num, out empty, true);
			Game.Log(string.Concat(new string[]
			{
				"Trespassing at (goal) ",
				this.currentAction.goal.passedInteractable.node.room.name,
				": ",
				flag.ToString(),
				" ",
				empty
			}), 2);
			flag = this.human.IsTrespassing(this.currentAction.node.room, out num, out empty, true);
			Game.Log(string.Concat(new string[]
			{
				"Trespassing at (action) ",
				this.currentAction.node.room.name,
				": ",
				flag.ToString(),
				" ",
				empty
			}), 2);
		}
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x00046A9C File Offset: 0x00044C9C
	[Button(null, 0)]
	public void CurrentGoalTriggerTime()
	{
		if (this.currentGoal != null)
		{
			Game.Log(string.Concat(new string[]
			{
				this.currentGoal.triggerTime.ToString(),
				" = ",
				SessionData.Instance.GameTimeToClock24String(this.currentGoal.triggerTime, true),
				", ",
				SessionData.Instance.ShortDateString(this.currentGoal.triggerTime, true)
			}), 2);
		}
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00046B17 File Offset: 0x00044D17
	[Button(null, 0)]
	public void ForceNodeReached()
	{
		this.ReachNewPathNode("Force Node Reached", true);
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x00046B25 File Offset: 0x00044D25
	[Button(null, 0)]
	public void DestinationCheck()
	{
		if (this.currentAction != null && !this.currentAction.isAtLocation)
		{
			this.currentAction.DestinationCheck("Force check", false);
		}
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00046B50 File Offset: 0x00044D50
	[Button(null, 0)]
	public void OpenEvidenceFirstName()
	{
		InterfaceController.Instance.SpawnWindow(this.human.evidenceEntry, Evidence.DataKey.firstName, null, "", false, true, default(Vector2), null, null, null, true);
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00046B8C File Offset: 0x00044D8C
	[Button(null, 0)]
	public void OpenEvidenceName()
	{
		InterfaceController.Instance.SpawnWindow(this.human.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00046BC8 File Offset: 0x00044DC8
	[Button(null, 0)]
	public void OpenEvidencePhoto()
	{
		InterfaceController.Instance.SpawnWindow(this.human.evidenceEntry, Evidence.DataKey.photo, null, "", false, true, default(Vector2), null, null, null, true);
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00046C04 File Offset: 0x00044E04
	[Button(null, 0)]
	public void ToggleHumanDebug()
	{
		if (Game.Instance.debugHuman.Contains(this.human))
		{
			Game.Log("Toggle AI debug for " + base.name + ": false", 2);
			Game.Instance.debugHuman.Remove(this.human);
			return;
		}
		Game.Log("Toggle AI debug for " + base.name + ": true", 2);
		Game.Instance.debugHuman.Add(this.human);
	}

	// Token: 0x040003B9 RID: 953
	[NonSerialized]
	public Human human;

	// Token: 0x040003BA RID: 954
	public CapsuleCollider capCollider;

	// Token: 0x040003BB RID: 955
	[NonSerialized]
	public float delta;

	// Token: 0x040003BC RID: 956
	private float prevDelta;

	// Token: 0x040003BD RID: 957
	[Header("Debug: Status Stats")]
	[ProgressBar("Nourishment", 1f, 7)]
	public float nourishment;

	// Token: 0x040003BE RID: 958
	[ProgressBar("Hydration", 1f, 7)]
	public float hydration;

	// Token: 0x040003BF RID: 959
	[ProgressBar("Alertness", 1f, 7)]
	public float alertness;

	// Token: 0x040003C0 RID: 960
	[ProgressBar("Energy", 1f, 7)]
	public float energy;

	// Token: 0x040003C1 RID: 961
	[ProgressBar("Excitement", 1f, 7)]
	public float excitement;

	// Token: 0x040003C2 RID: 962
	[ProgressBar("Chores", 1f, 7)]
	public float chores;

	// Token: 0x040003C3 RID: 963
	[ProgressBar("Hygiene", 1f, 7)]
	public float hygiene;

	// Token: 0x040003C4 RID: 964
	[ProgressBar("Bladder", 1f, 7)]
	public float bladder;

	// Token: 0x040003C5 RID: 965
	[ProgressBar("Heat", 1f, 7)]
	public float heat;

	// Token: 0x040003C6 RID: 966
	[ProgressBar("Drunk", 1f, 7)]
	public float drunk;

	// Token: 0x040003C7 RID: 967
	[ProgressBar("Breath", 1f, 7)]
	public float breath;

	// Token: 0x040003C8 RID: 968
	[ProgressBar("IdleSound", 1f, 7)]
	public float idleSound = 1f;

	// Token: 0x040003C9 RID: 969
	[ProgressBar("Blink", 1f, 7)]
	public float blink = 1f;

	// Token: 0x040003CA RID: 970
	[Space(5f)]
	[ProgressBar("Sees Player", 100f, 9)]
	public int debugSeesPlayer;

	// Token: 0x040003CB RID: 971
	[ReadOnly]
	public float debugLastSeesPlayerChange;

	// Token: 0x040003CC RID: 972
	[ProgressBar("Hears Player", 1f, 9)]
	public float hearsIllegal;

	// Token: 0x040003CD RID: 973
	public Actor hearTarget;

	// Token: 0x040003CE RID: 974
	[Tooltip("Goals are a list of things this AI wants to achieve")]
	[Header("Goals")]
	public List<NewAIGoal> goals = new List<NewAIGoal>();

	// Token: 0x040003CF RID: 975
	[Tooltip("The currently active goal")]
	[NonSerialized]
	public NewAIGoal currentGoal;

	// Token: 0x040003D0 RID: 976
	[Tooltip("The currently active action")]
	[NonSerialized]
	public NewAIAction currentAction;

	// Token: 0x040003D1 RID: 977
	[Tooltip("Investigation goal")]
	[NonSerialized]
	public NewAIGoal investigationGoal;

	// Token: 0x040003D2 RID: 978
	[Tooltip("Patrol goal")]
	[NonSerialized]
	public NewAIGoal patrolGoal;

	// Token: 0x040003D3 RID: 979
	[Tooltip("The current ineractable this is using")]
	[NonSerialized]
	public FurnitureLocation currentFurnitureUser;

	// Token: 0x040003D4 RID: 980
	[NonSerialized]
	public NewNode currentFurnitureNode;

	// Token: 0x040003D5 RID: 981
	public Interactable nextAIAction;

	// Token: 0x040003D6 RID: 982
	public Human kidnapper;

	// Token: 0x040003D7 RID: 983
	public NewGameLocation confineLocation;

	// Token: 0x040003D8 RID: 984
	public List<NewGameLocation> avoidLocations = new List<NewGameLocation>();

	// Token: 0x040003D9 RID: 985
	[Header("Movement")]
	[NonSerialized]
	public int pathCursor;

	// Token: 0x040003DA RID: 986
	[NonSerialized]
	public NewNode currentDestinationNode;

	// Token: 0x040003DB RID: 987
	[NonSerialized]
	public Vector3 currentDesitnationNodeCoord;

	// Token: 0x040003DC RID: 988
	public Vector3 currentDestinationPositon;

	// Token: 0x040003DD RID: 989
	public float movementAmount;

	// Token: 0x040003DE RID: 990
	public float distanceToNext;

	// Token: 0x040003DF RID: 991
	private Quaternion lastMovementRotation = Quaternion.identity;

	// Token: 0x040003E0 RID: 992
	private bool doIMove;

	// Token: 0x040003E1 RID: 993
	private float footStepDistanceCounter;

	// Token: 0x040003E2 RID: 994
	private bool rightFootNext;

	// Token: 0x040003E3 RID: 995
	public bool isTripping;

	// Token: 0x040003E4 RID: 996
	public bool doorCheck = true;

	// Token: 0x040003E5 RID: 997
	[Tooltip("If I've just opened a door, this is a reference to it so I can close it later")]
	[NonSerialized]
	public NewDoor openedDoor;

	// Token: 0x040003E6 RID: 998
	[NonSerialized]
	private int delayFlag;

	// Token: 0x040003E7 RID: 999
	private List<NewDoor> doorInteractions = new List<NewDoor>();

	// Token: 0x040003E8 RID: 1000
	[Header("Turning")]
	public bool facingActive;

	// Token: 0x040003E9 RID: 1001
	[Header("Facing")]
	public Vector3 facingDirection = Vector3.zero;

	// Token: 0x040003EA RID: 1002
	[NonSerialized]
	public Transform faceTransform;

	// Token: 0x040003EB RID: 1003
	[NonSerialized]
	public Vector3 faceTransformOffset = Vector3.zero;

	// Token: 0x040003EC RID: 1004
	public Quaternion facingQuat;

	// Token: 0x040003ED RID: 1005
	private Quaternion lookingQuatPrevious;

	// Token: 0x040003EE RID: 1006
	private Quaternion lookingQuatLastFrame;

	// Token: 0x040003EF RID: 1007
	private Quaternion lookingQuatCurrent;

	// Token: 0x040003F0 RID: 1008
	private float lookAroundTimer;

	// Token: 0x040003F1 RID: 1009
	private Vector3 lookAroundPosition;

	// Token: 0x040003F2 RID: 1010
	[Header("Vision")]
	public List<NewAIController.TrackingTarget> trackedTargets = new List<NewAIController.TrackingTarget>();

	// Token: 0x040003F3 RID: 1011
	[NonSerialized]
	public NewAIController.TrackingTarget currentTrackTarget;

	// Token: 0x040003F4 RID: 1012
	public Transform lookAtTransform;

	// Token: 0x040003F5 RID: 1013
	public float lookAtTransformRank;

	// Token: 0x040003F6 RID: 1014
	private float lastLookAtUpdateTime;

	// Token: 0x040003F7 RID: 1015
	[HideInInspector]
	[SerializeField]
	private Quaternion original;

	// Token: 0x040003F8 RID: 1016
	private Vector3 dirXZ;

	// Token: 0x040003F9 RID: 1017
	private Vector3 forwardXZ;

	// Token: 0x040003FA RID: 1018
	private Vector3 dirYZ;

	// Token: 0x040003FB RID: 1019
	private Vector3 forwardYZ;

	// Token: 0x040003FC RID: 1020
	[Header("Expression")]
	[NonSerialized]
	public CitizenOutfitController.ExpressionSetup currentExpression;

	// Token: 0x040003FD RID: 1021
	public float expressionProgress;

	// Token: 0x040003FE RID: 1022
	public bool blinkInProgress;

	// Token: 0x040003FF RID: 1023
	private float blinkTimer;

	// Token: 0x04000400 RID: 1024
	public float eyesOpen = 1f;

	// Token: 0x04000401 RID: 1025
	public float bargeTimer;

	// Token: 0x04000402 RID: 1026
	[Header("Investigate AI")]
	public Actor persuitTarget;

	// Token: 0x04000403 RID: 1027
	public NewNode investigateLocation;

	// Token: 0x04000404 RID: 1028
	public Vector3 investigatePosition;

	// Token: 0x04000405 RID: 1029
	public Vector3 investigatePositionProjection;

	// Token: 0x04000406 RID: 1030
	public Interactable investigateObject;

	// Token: 0x04000407 RID: 1031
	public Interactable tamperedObject;

	// Token: 0x04000408 RID: 1032
	public NewAIController.InvestigationUrgency investigationUrgency;

	// Token: 0x04000409 RID: 1033
	[NonSerialized]
	public NewAIAction audioFocusAction;

	// Token: 0x0400040A RID: 1034
	public float lastInvestigate;

	// Token: 0x0400040B RID: 1035
	private float persuitUpdateTimer;

	// Token: 0x0400040C RID: 1036
	public bool persuit;

	// Token: 0x0400040D RID: 1037
	public bool seesOnPersuit;

	// Token: 0x0400040E RID: 1038
	public float persuitChaseLogicUses;

	// Token: 0x0400040F RID: 1039
	public float minimumInvestigationTimeMultiplier = 1f;

	// Token: 0x04000410 RID: 1040
	public NewAIController.ChaseLogic chaseLogic;

	// Token: 0x04000411 RID: 1041
	public ReactionIndicatorController reactionIndicator;

	// Token: 0x04000412 RID: 1042
	public NewAIController.ReactionState reactionState;

	// Token: 0x04000413 RID: 1043
	[Header("Patrol AI")]
	public NewGameLocation patrolLocation;

	// Token: 0x04000414 RID: 1044
	[Header("Attack")]
	public bool inCombat;

	// Token: 0x04000415 RID: 1045
	public bool inFleeState;

	// Token: 0x04000416 RID: 1046
	public bool staticFromAnimation;

	// Token: 0x04000417 RID: 1047
	public float staticAnimationSafetyTimer;

	// Token: 0x04000418 RID: 1048
	public bool attackActive;

	// Token: 0x04000419 RID: 1049
	public Actor attackTarget;

	// Token: 0x0400041A RID: 1050
	public AttackBarController activeAttackBar;

	// Token: 0x0400041B RID: 1051
	public float attackTimeout;

	// Token: 0x0400041C RID: 1052
	public float attackProgress;

	// Token: 0x0400041D RID: 1053
	private int revolverShots;

	// Token: 0x0400041E RID: 1054
	public bool damageColliderCreated;

	// Token: 0x0400041F RID: 1055
	private bool ejectBrassCreated;

	// Token: 0x04000420 RID: 1056
	public DamageColliderController damageCollider;

	// Token: 0x04000421 RID: 1057
	public float attackDelay;

	// Token: 0x04000422 RID: 1058
	private float attackActiveLength;

	// Token: 0x04000423 RID: 1059
	public bool ko;

	// Token: 0x04000424 RID: 1060
	public bool isRagdoll;

	// Token: 0x04000425 RID: 1061
	public RigidbodyDragObject dragController;

	// Token: 0x04000426 RID: 1062
	public RagdollPositionUpdater ragdollPositionUpdate;

	// Token: 0x04000427 RID: 1063
	public float koTime;

	// Token: 0x04000428 RID: 1064
	public float koTransitionTimer;

	// Token: 0x04000429 RID: 1065
	private float getUpDelayTimer;

	// Token: 0x0400042A RID: 1066
	public float deadRagdollTimer;

	// Token: 0x0400042B RID: 1067
	public bool restrained;

	// Token: 0x0400042C RID: 1068
	public bool outOfBreath;

	// Token: 0x0400042D RID: 1069
	public float restrainTime;

	// Token: 0x0400042E RID: 1070
	[NonSerialized]
	public Interactable currentWeapon;

	// Token: 0x0400042F RID: 1071
	public MurderWeaponPreset currentWeaponPreset;

	// Token: 0x04000430 RID: 1072
	public float weaponRangeMax = 1.75f;

	// Token: 0x04000431 RID: 1073
	public float weaponRefire = 1f;

	// Token: 0x04000432 RID: 1074
	public float weaponAccuracy = 1f;

	// Token: 0x04000433 RID: 1075
	public float weaponDamage = 0.1f;

	// Token: 0x04000434 RID: 1076
	[Header("Update")]
	public NewAIController.AITickRate desiredTickRate = NewAIController.AITickRate.medium;

	// Token: 0x04000435 RID: 1077
	public NewAIController.AITickRate previousTickRate = NewAIController.AITickRate.medium;

	// Token: 0x04000436 RID: 1078
	public NewAIController.AITickRate tickRate = NewAIController.AITickRate.medium;

	// Token: 0x04000437 RID: 1079
	public bool dueUpdate;

	// Token: 0x04000438 RID: 1080
	public float delayedUntil;

	// Token: 0x04000439 RID: 1081
	public float lastUpdated;

	// Token: 0x0400043A RID: 1082
	private float lastSnore;

	// Token: 0x0400043B RID: 1083
	public float timeSinceLastUpdate;

	// Token: 0x0400043C RID: 1084
	public float timeAtCurrentAddress;

	// Token: 0x0400043D RID: 1085
	private float drunkTripCheckTimer;

	// Token: 0x0400043E RID: 1086
	private int doorCheckProcessTimer;

	// Token: 0x0400043F RID: 1087
	private bool visibleMovementAnimationLerpRequired;

	// Token: 0x04000440 RID: 1088
	public bool disableTickRateUpdate;

	// Token: 0x04000441 RID: 1089
	public Dictionary<AIGoalPreset, float> delayedGoalsForTime = new Dictionary<AIGoalPreset, float>();

	// Token: 0x04000442 RID: 1090
	public Dictionary<AIActionPreset, float> delayedActionsForTime = new Dictionary<AIActionPreset, float>();

	// Token: 0x04000443 RID: 1091
	public List<NewAIController.QueuedAction> queuedActions = new List<NewAIController.QueuedAction>();

	// Token: 0x04000444 RID: 1092
	[Header("Held Items")]
	public GameObject spawnedRightItem;

	// Token: 0x04000445 RID: 1093
	public GameObject spawnedLeftItem;

	// Token: 0x04000446 RID: 1094
	[NonSerialized]
	public NewAIAction customItemSource;

	// Token: 0x04000447 RID: 1095
	public bool usingCarryAnimation;

	// Token: 0x04000448 RID: 1096
	public int combatMode;

	// Token: 0x04000449 RID: 1097
	[NonSerialized]
	public InteractablePreset throwItem;

	// Token: 0x0400044A RID: 1098
	public bool throwActive;

	// Token: 0x0400044B RID: 1099
	public float throwDelay;

	// Token: 0x0400044C RID: 1100
	[Header("Special Cases")]
	public bool dontEverCloseDoors;

	// Token: 0x0400044D RID: 1101
	public List<MurderController.Murder> victimsForMurders = new List<MurderController.Murder>();

	// Token: 0x0400044E RID: 1102
	public List<MurderController.Murder> killerForMurders = new List<MurderController.Murder>();

	// Token: 0x0400044F RID: 1103
	public bool isConvicted;

	// Token: 0x04000450 RID: 1104
	private bool usePointBusyRecursion;

	// Token: 0x04000451 RID: 1105
	[NonSerialized]
	public NewGameLocation closeDoorsNormallyAfterLeaving;

	// Token: 0x04000452 RID: 1106
	public List<Interactable> putDownItems = new List<Interactable>();

	// Token: 0x04000453 RID: 1107
	private float drunkIdleTimer;

	// Token: 0x04000454 RID: 1108
	private float restrainedIdleTimer;

	// Token: 0x04000455 RID: 1109
	public Dictionary<Human, float> appliedNerveEffect = new Dictionary<Human, float>();

	// Token: 0x04000456 RID: 1110
	private bool tickActive;

	// Token: 0x04000457 RID: 1111
	public float spooked;

	// Token: 0x04000458 RID: 1112
	public int spookCounter;

	// Token: 0x04000459 RID: 1113
	public float spookForgetCounter;

	// Token: 0x0400045A RID: 1114
	[Header("Debug")]
	public List<string> lastActions = new List<string>();

	// Token: 0x0400045B RID: 1115
	public List<string> debugDestinationPosition = new List<string>();

	// Token: 0x0400045C RID: 1116
	public string jobDebug;

	// Token: 0x0400045D RID: 1117
	public bool debugMovement;

	// Token: 0x0400045E RID: 1118
	public AudioEvent debugLastHeardIllegalAudio;

	// Token: 0x0400045F RID: 1119
	protected List<AIActionPreset> rem = new List<AIActionPreset>();

	// Token: 0x02000087 RID: 135
	[Serializable]
	public class TrackingTarget
	{
		// Token: 0x04000460 RID: 1120
		public Actor actor;

		// Token: 0x04000461 RID: 1121
		public float lastValidSighting;

		// Token: 0x04000462 RID: 1122
		public bool priorityTarget;

		// Token: 0x04000463 RID: 1123
		public float attractionRank;

		// Token: 0x04000464 RID: 1124
		public float distance;

		// Token: 0x04000465 RID: 1125
		public float distanceRank;

		// Token: 0x04000466 RID: 1126
		public float fovRank;

		// Token: 0x04000467 RID: 1127
		public float itemRank;

		// Token: 0x04000468 RID: 1128
		public float lookAtRank;

		// Token: 0x04000469 RID: 1129
		public bool active;

		// Token: 0x0400046A RID: 1130
		public bool spookedByItem;

		// Token: 0x0400046B RID: 1131
		public int spookTimer;
	}

	// Token: 0x02000088 RID: 136
	[Serializable]
	public class ChaseLogic
	{
		// Token: 0x060004B2 RID: 1202 RVA: 0x00046E10 File Offset: 0x00045010
		public void UpdateLastSeen()
		{
			if (this.ai.persuitTarget == null)
			{
				return;
			}
			this.lastSeenNode = this.ai.persuitTarget.currentNode;
			this.lastSeenPosition = this.ai.persuitTarget.transform.position;
			this.lastSeenDirection = this.ai.persuitTarget.transform.forward;
			if (this.ai.persuitTarget.isPlayer)
			{
				string[] array = new string[6];
				array[0] = "AI: ";
				array[1] = this.ai.name;
				array[2] = " updated persuit last seen of player: ";
				int num = 3;
				Vector3 vector = this.lastSeenPosition;
				array[num] = vector.ToString();
				array[4] = " facing dir ";
				int num2 = 5;
				vector = this.lastSeenDirection;
				array[num2] = vector.ToString();
				Game.Log(string.Concat(array), 2);
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00046EF4 File Offset: 0x000450F4
		public void GenerateProjectedNode()
		{
			if (this.lastSeenNode == null)
			{
				if (this.ai.persuitTarget != null)
				{
					this.lastSeenNode = this.ai.persuitTarget.currentNode;
				}
				else
				{
					this.lastSeenNode = this.ai.human.currentNode;
				}
			}
			if (this.lastSeenNode == null)
			{
				return;
			}
			this.lastSeenNode.nodeCoord;
			NewNode newNode = this.lastSeenNode;
			Vector3 normalized = this.lastSeenDirection.normalized;
			if (Mathf.Abs(normalized.x) > Mathf.Abs(normalized.z))
			{
				normalized..ctor((float)Mathf.RoundToInt(normalized.x), 0f, 0f);
			}
			else
			{
				normalized..ctor(0f, (float)Mathf.RoundToInt(normalized.z), 0f);
			}
			this.projectedNode = this.lastSeenNode;
			int num = 0;
			int i = 0;
			while (i < 12)
			{
				Vector3Int vector3Int = this.projectedNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(normalized.x), Mathf.RoundToInt(normalized.y), Mathf.RoundToInt(normalized.z));
				string[] array = new string[8];
				array[0] = "AI: ";
				array[1] = this.ai.name;
				array[2] = " Distance ";
				array[3] = num.ToString();
				array[4] = " attempting to find a node at ";
				int num2 = 5;
				Vector3Int vector3Int2 = vector3Int;
				array[num2] = vector3Int2.ToString();
				array[6] = " using normalized direction ";
				int num3 = 7;
				Vector3 vector = normalized;
				array[num3] = vector.ToString();
				Game.Log(string.Concat(array), 2);
				if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
				{
					goto IL_27F;
				}
				string text = "AI: ";
				string name = this.ai.name;
				string text2 = " found a valid node at ";
				NewNode newNode2 = newNode;
				Game.Log(text + name + text2 + ((newNode2 != null) ? newNode2.ToString() : null), 2);
				int num4;
				string text3;
				List<DataRaycastController.NodeRaycastHit> list;
				if (this.ai.human.IsTrespassing(newNode.room, out num4, out text3, true) || !DataRaycastController.Instance.NodeRaycast(this.lastSeenNode, newNode, out list, null, false))
				{
					goto IL_27F;
				}
				string[] array2 = new string[6];
				array2[0] = "AI: ";
				array2[1] = this.ai.name;
				array2[2] = " data raycast between ";
				int num5 = 3;
				vector = this.lastSeenPosition;
				array2[num5] = vector.ToString();
				array2[4] = " and node at ";
				int num6 = 5;
				vector = newNode.position;
				array2[num6] = vector.ToString();
				Game.Log(string.Concat(array2), 2);
				this.projectedNode = newNode;
				num++;
				IL_488:
				i++;
				continue;
				IL_27F:
				if (num >= 4)
				{
					goto IL_488;
				}
				NewRoom newRoom = null;
				float num7 = -99999f;
				if (this.projectedNode.room != this.ai.human.currentRoom)
				{
					newRoom = this.projectedNode.room;
					num7 = Toolbox.Instance.Rand(0f, 2f, false);
					if (this.ai.persuitTarget != null && this.ai.persuitTarget.currentRoom == newRoom)
					{
						num7 += 1f;
					}
				}
				foreach (NewNode.NodeAccess nodeAccess in this.projectedNode.room.entrances)
				{
					if (nodeAccess.walkingAccess)
					{
						NewRoom room = nodeAccess.toNode.room;
						if (room == this.projectedNode.room)
						{
							room = nodeAccess.fromNode.room;
						}
						if (!this.ai.human.IsTrespassing(room, out num4, out text3, true))
						{
							float num8 = Toolbox.Instance.Rand(0f, 2f, false);
							float num9 = Vector3.Distance(this.projectedNode.position, nodeAccess.worldAccessPoint);
							if (this.ai.persuitTarget != null && this.ai.persuitTarget.currentRoom == newRoom)
							{
								num8 += 1f;
							}
							if (num9 < 10f && num9 > 1.2f && num8 > num7)
							{
								newRoom = room;
								num7 = num8;
							}
						}
					}
				}
				if (!(newRoom != null))
				{
					break;
				}
				NewNode newNode3 = this.ai.human.FindSafeTeleport(newRoom, false);
				if (newNode3 != null)
				{
					this.projectedNode = newNode3;
					Game.Log("AI: " + this.ai.name + " found room using an entrance for " + this.projectedNode.room.name, 2);
					break;
				}
				break;
			}
			this.projectedPosition = this.projectedNode.position;
			if (this.ai.persuitTarget != null && this.ai.persuitTarget.isPlayer)
			{
				string text4 = "AI: ";
				string name2 = this.ai.name;
				string text5 = " generated new projection of player at ";
				Vector3 vector = this.projectedPosition;
				Game.Log(text4 + name2 + text5 + vector.ToString(), 2);
			}
		}

		// Token: 0x0400046C RID: 1132
		public NewAIController ai;

		// Token: 0x0400046D RID: 1133
		public Vector3 lastSeenPosition;

		// Token: 0x0400046E RID: 1134
		public NewNode lastSeenNode;

		// Token: 0x0400046F RID: 1135
		public Vector3 lastSeenDirection;

		// Token: 0x04000470 RID: 1136
		public NewNode projectedNode;

		// Token: 0x04000471 RID: 1137
		public Vector3 projectedPosition;
	}

	// Token: 0x02000089 RID: 137
	public enum InvestigationUrgency
	{
		// Token: 0x04000473 RID: 1139
		walk,
		// Token: 0x04000474 RID: 1140
		run
	}

	// Token: 0x0200008A RID: 138
	public enum ReactionState
	{
		// Token: 0x04000476 RID: 1142
		none,
		// Token: 0x04000477 RID: 1143
		investigatingSight,
		// Token: 0x04000478 RID: 1144
		investigatingSound,
		// Token: 0x04000479 RID: 1145
		persuing,
		// Token: 0x0400047A RID: 1146
		searching
	}

	// Token: 0x0200008B RID: 139
	public enum AITickRate
	{
		// Token: 0x0400047C RID: 1148
		veryLow,
		// Token: 0x0400047D RID: 1149
		low,
		// Token: 0x0400047E RID: 1150
		medium,
		// Token: 0x0400047F RID: 1151
		high,
		// Token: 0x04000480 RID: 1152
		veryHigh
	}

	// Token: 0x0200008C RID: 140
	public class QueuedAction
	{
		// Token: 0x04000481 RID: 1153
		public Interactable interactable;

		// Token: 0x04000482 RID: 1154
		public InteractablePreset.InteractionAction actionSetting;

		// Token: 0x04000483 RID: 1155
		public float delay;
	}
}
