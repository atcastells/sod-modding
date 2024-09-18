using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200031F RID: 799
[Serializable]
public class SideJobStealBriefcase : SideJob
{
	// Token: 0x06001217 RID: 4631 RVA: 0x001018C6 File Offset: 0x000FFAC6
	public SideJobStealBriefcase(JobPreset newPreset, SideJobController.JobPickData newData, bool immediatePost) : base(newPreset, newData, immediatePost)
	{
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x001018D8 File Offset: 0x000FFAD8
	private void PickMeet()
	{
		List<SideJobStealBriefcase.NodeCompare> list = new List<SideJobStealBriefcase.NodeCompare>();
		foreach (NewGameLocation newGameLocation in CityData.Instance.gameLocationDirectory)
		{
			foreach (NewRoom newRoom in newGameLocation.rooms)
			{
				float num = 0f;
				if (Toolbox.Instance.RankRoomShadiness(newRoom, out num))
				{
					foreach (NewNode node in newRoom.nodes)
					{
						float num2 = 0f;
						if (Toolbox.Instance.RankNodeShadiness(node, out num2))
						{
							list.Add(new SideJobStealBriefcase.NodeCompare
							{
								node = node,
								score = num + num2
							});
						}
					}
				}
			}
		}
		if (list.Count > 0)
		{
			list.Sort((SideJobStealBriefcase.NodeCompare p1, SideJobStealBriefcase.NodeCompare p2) => p2.score.CompareTo(p1.score));
			SideJobStealBriefcase.NodeCompare nodeCompare = list[Toolbox.Instance.Rand(0, Mathf.CeilToInt((float)list.Count * 0.25f), false)];
			this.destination = nodeCompare.node;
			this.meetNodeLocation = nodeCompare.node.nodeCoord;
			string[] array = new string[5];
			array[0] = "Jobs: Chosen meeting point for briefcase job: ";
			int num3 = 1;
			Vector3 position = nodeCompare.node.position;
			array[num3] = position.ToString();
			array[2] = " out of ";
			array[3] = list.Count.ToString();
			array[4] = " options...";
			Game.Log(string.Concat(array), 2);
		}
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x00101AD4 File Offset: 0x000FFCD4
	public override void OnGooseChaseSuccess()
	{
		base.OnGooseChaseSuccess();
		this.SetupCarrier();
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x00101AE4 File Offset: 0x000FFCE4
	public void SetupCarrier()
	{
		this.caseCarrier = null;
		float num = -9999f;
		if (Player.Instance.currentRoom.currentOccupants.Count <= 1)
		{
			Human human = CityData.Instance.citizenDirectory.Find((Citizen item) => !item.isAsleep && !item.isStunned && item != this.purp && item != this.poster && item.humility < 0.7f);
			if (human != null)
			{
				human.Teleport(human.FindSafeTeleport(Player.Instance.currentRoom, false), null, true, false);
			}
		}
		foreach (Actor actor in Player.Instance.currentRoom.currentOccupants)
		{
			Human human2 = actor as Human;
			if (human2 != null && !human2.isPlayer && !(human2.ai == null))
			{
				if (this.caseCarrier == null)
				{
					this.caseCarrier = human2;
				}
				else
				{
					float num2 = (1f - human2.humility) * 5f;
					if (actor.animationController.idleAnimationState == CitizenAnimationController.IdleAnimationState.sitting)
					{
						num2 += 10f;
					}
					if (num2 > num)
					{
						this.caseCarrier = human2;
						num = num2;
					}
				}
			}
		}
		if (this.caseCarrier != null)
		{
			this.carrier = this.caseCarrier.humanID;
			this.meetTimer = Toolbox.Instance.Rand(3f, 5f, false) * 0.01667f;
			Game.Log("Jobs: Chosen briefcase carrier " + this.caseCarrier.name + "...", 2);
		}
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x00101C84 File Offset: 0x000FFE84
	public override void OnAcquireJobInfo(string infoDialogMessage)
	{
		base.OnAcquireJobInfo(infoDialogMessage);
		Interactable item = base.GetItem(JobPreset.JobTag.A);
		if (item != null)
		{
			Game.Log("Jobs: Spawned briefcase: " + item.preset.name, 2);
			item.SetInInventory(this.caseCarrier);
			this.PickMeet();
		}
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x00101CD0 File Offset: 0x000FFED0
	public NewNode GetLocationNode()
	{
		if (this.destination != null)
		{
			return this.destination;
		}
		NewNode result = null;
		PathFinder.Instance.nodeMap.TryGetValue(this.meetNodeLocation, ref result);
		return result;
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x00101D0C File Offset: 0x000FFF0C
	public override Human GetExtraPerson1()
	{
		if (this.caseCarrier != null)
		{
			return this.caseCarrier;
		}
		Human result = null;
		CityData.Instance.GetHuman(this.carrier, out result, true);
		return result;
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x00101D48 File Offset: 0x000FFF48
	public override void GameWorldLoop()
	{
		base.GameWorldLoop();
		if (this.knowHandInLocation && this.carrier > -1 && !this.triggeredMeet)
		{
			if (this.meetTimer > 0f)
			{
				this.meetTimer -= SessionData.Instance.gameTime - this.gwTime;
				Game.Log("Jobs: Briefcase job: Meet timer for exchange: " + this.meetTimer.ToString(), 2);
			}
			else
			{
				Game.Log("Jobs: Briefcase job: Meeting should happen soon...", 2);
				if (this.caseCarrier == null)
				{
					this.caseCarrier = this.GetExtraPerson1();
				}
				if (this.destination == null)
				{
					this.destination = this.GetLocationNode();
				}
				bool flag = false;
				if (this.caseCarrier != null && this.caseCarrier.ai != null && this.caseCarrier.ai.goals != null)
				{
					this.caseCarrier.ai.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, false);
					NewAIGoal newAIGoal = this.caseCarrier.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.toGoWalkGoal && item.passedNode == this.destination);
					if (newAIGoal == null)
					{
						Game.Log("Jobs: Briefcase job: Did not find case carrier goto goal...", 2);
						this.caseCarrier.AddBladder(1f);
						this.caseCarrier.AddHydration(1f);
						this.caseCarrier.AddNourishment(1f);
						this.caseCarrier.AddEnergy(1f);
						this.caseCarrier.AddHeat(1f);
						this.caseCarrier.AddAlertness(1f);
						this.caseCarrier.AddExcitement(1f);
						this.caseCarrier.AddHygiene(1f);
						this.caseCarrier.AddNerve(1f, null);
						if (this.destination != null)
						{
							string text = "Jobs: Briefcase job: Creating goal for ";
							string name = this.caseCarrier.name;
							string text2 = " to go to ";
							Vector3 position = this.destination.position;
							Game.Log(text + name + text2 + position.ToString(), 2);
							this.caseCarrier.ai.CreateNewGoal(RoutineControls.Instance.toGoWalkGoal, 0f, 0f, this.destination, null, null, null, null, -2);
						}
					}
					else
					{
						Game.Log("Jobs: Briefcase job: Found existing case carrier goto goal...", 2);
						if (!newAIGoal.isActive)
						{
							flag = false;
							this.caseCarrier.ai.AITick(true, false);
						}
						else
						{
							flag = (this.caseCarrier.ai.currentGoal == newAIGoal);
						}
					}
				}
				if (this.purp != null && this.purp.ai != null && this.purp.ai.goals != null)
				{
					NewAIGoal newAIGoal2 = this.purp.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.toGoGoal && item.passedNode == this.destination);
					if (newAIGoal2 == null)
					{
						this.purp.AddBladder(1f);
						this.purp.AddHydration(1f);
						this.purp.AddNourishment(1f);
						this.purp.AddEnergy(1f);
						this.purp.AddHeat(1f);
						this.purp.AddAlertness(1f);
						this.purp.AddExcitement(1f);
						this.purp.AddHygiene(1f);
						this.purp.AddNerve(1f, null);
						if (this.destination != null)
						{
							string text3 = "Jobs: Briefcase job: Creating goal for ";
							string name2 = this.purp.name;
							string text4 = " to go to ";
							Vector3 position = this.destination.position;
							Game.Log(text3 + name2 + text4 + position.ToString(), 2);
							this.purp.ai.CreateNewGoal(RoutineControls.Instance.toGoGoal, 0f, 0f, this.destination, null, null, null, null, -2);
						}
					}
					else
					{
						this.purp.ai.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, false);
						if (!newAIGoal2.isActive)
						{
							this.purp.ai.AITick(true, false);
						}
						else if (flag && this.purp.ai.currentAction != null && this.caseCarrier != null && this.caseCarrier.ai != null && this.caseCarrier.ai.currentAction != null && (this.purp.ai.currentAction.estimatedArrival > this.caseCarrier.ai.currentAction.estimatedArrival || this.caseCarrier.ai.currentAction.isAtLocation))
						{
							this.purp.SetDesiredSpeed(Human.MovementSpeed.running);
						}
					}
				}
				if (this.caseCarrier != null && this.purp != null && this.caseCarrier.ai != null && this.purp.ai != null)
				{
					if (Vector3.Distance(this.purp.transform.position, this.caseCarrier.transform.position) <= 3f)
					{
						if (this.caseCarrier.ai.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor == this.purp) || this.purp.ai.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor == this.caseCarrier))
						{
							Game.Log("Jobs: Briefcase job: Trigger meet...", 2);
							this.triggeredMeet = true;
							DDSSaveClasses.DDSTreeSave newTree = null;
							if (Toolbox.Instance.allDDSTrees.TryGetValue("18fea754-5675-4f73-aeb8-78c0a754465e", ref newTree))
							{
								Game.Log("Jobs: Briefcase job: Executing conversation", 2);
								this.caseCarrier.ExecuteConversationTree(newTree, Enumerable.ToList<Human>(new Human[]
								{
									this.purp
								}));
							}
						}
						if (this.waitObjective != null)
						{
							this.waitObjective.Complete();
						}
					}
					else if (this.waitObjective == null)
					{
						this.waitObjective = this.thisCase.currentActiveObjectives.Find((Objective item) => item.queueElement.entryRef == "Wait for the meeting to take place");
						if (this.waitObjective == null)
						{
							Game.Log("Jobs: Briefcase job: Setting up wait objective", 2);
							Objective.ObjectiveTrigger objectiveTrigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.sideMissionMeetTriggered, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
							string entryRef = "Wait for the meeting to take place";
							Objective.ObjectiveTrigger trigger = objectiveTrigger;
							bool usePointer = false;
							string chapterString = this.phase.ToString();
							this.AddObjective(entryRef, trigger, usePointer, default(Vector3), InterfaceControls.Icon.time, Objective.OnCompleteAction.nothing, 0f, false, chapterString, false, false);
						}
					}
				}
			}
		}
		else if (this.triggeredMeet && !this.triggeredSwitch && this.caseCarrier != null && !this.caseCarrier.inConversation && this.caseCarrier.ai != null)
		{
			Interactable item2 = base.GetItem(JobPreset.JobTag.A);
			if (item2 != null && this.purp != null && this.purp.ai != null)
			{
				item2.SetAsNotInventory(this.caseCarrier.currentNode);
				item2.SetInInventory(this.purp);
				Game.Log("Jobs: Briefcase job: Triggered briefcase switch...", 2);
				this.caseCarrier.ai.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
				this.purp.ai.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
				if (this.caseCarrier.animationController != null)
				{
					this.caseCarrier.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.armsOneShotUse);
				}
				if (this.purp.animationController != null)
				{
					this.purp.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.armsOneShotUse);
				}
				this.purp.AddHygiene(-1f);
				if (this.purp.home != null)
				{
					this.purp.ai.CreateNewGoal(RoutineControls.Instance.toGoWalkGoal, 0f, 0f, this.purp.FindSafeTeleport(this.purp.home, false, true), null, null, null, null, -2);
				}
				this.triggeredSwitch = true;
			}
		}
		this.gwTime = SessionData.Instance.gameTime;
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x00102590 File Offset: 0x00100790
	public override void UpdateResolveAnswers()
	{
		base.UpdateResolveAnswers();
		Case.ResolveQuestion resolveQuestion = this.thisCase.resolveQuestions.Find((Case.ResolveQuestion item) => item.tag == JobPreset.JobTag.B);
		if (resolveQuestion != null)
		{
			resolveQuestion.correctAnswers = new List<string>();
			foreach (Interactable interactable in CityData.Instance.interactableDirectory.FindAll((Interactable item) => item.preset == InteriorControls.Instance.surveillancePrintout))
			{
				if (interactable.evidence != null)
				{
					EvidenceSurveillance evidenceSurveillance = interactable.evidence as EvidenceSurveillance;
					if (evidenceSurveillance != null && evidenceSurveillance.savedCapture != null && this.purp != null && evidenceSurveillance.savedCapture.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.purp.humanID) && evidenceSurveillance.savedCapture.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.carrier) && !resolveQuestion.correctAnswers.Contains(evidenceSurveillance.evID))
					{
						resolveQuestion.correctAnswers.Add(evidenceSurveillance.evID);
					}
				}
			}
		}
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x001026E4 File Offset: 0x001008E4
	public override void OnDestroyMissionObject(Interactable destroyed)
	{
		base.OnDestroyMissionObject(destroyed);
	}

	// Token: 0x04001626 RID: 5670
	[Header("Serialized Data")]
	public int carrier = -1;

	// Token: 0x04001627 RID: 5671
	public Vector3Int meetNodeLocation;

	// Token: 0x04001628 RID: 5672
	public bool triggeredSwitch;

	// Token: 0x04001629 RID: 5673
	public bool triggeredMeet;

	// Token: 0x0400162A RID: 5674
	public float meetTimer;

	// Token: 0x0400162B RID: 5675
	[NonSerialized]
	public Human caseCarrier;

	// Token: 0x0400162C RID: 5676
	[NonSerialized]
	public NewNode destination;

	// Token: 0x0400162D RID: 5677
	private float gwTime;

	// Token: 0x0400162E RID: 5678
	private Objective waitObjective;

	// Token: 0x02000320 RID: 800
	public struct NodeCompare
	{
		// Token: 0x0400162F RID: 5679
		public NewNode node;

		// Token: 0x04001630 RID: 5680
		public float score;
	}
}
