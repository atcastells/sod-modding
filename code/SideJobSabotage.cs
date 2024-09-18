using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031D RID: 797
[Serializable]
public class SideJobSabotage : SideJob
{
	// Token: 0x0600120E RID: 4622 RVA: 0x00101281 File Offset: 0x000FF481
	public SideJobSabotage(JobPreset newPreset, SideJobController.JobPickData newData, bool immediatePost) : base(newPreset, newData, immediatePost)
	{
		this.GenerateFakeNumber();
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x00101294 File Offset: 0x000FF494
	public override void GameWorldLoop()
	{
		base.GameWorldLoop();
		if (this.thisCase != null && this.thisCase.isActive)
		{
			if (this.phase == 0)
			{
				if (Player.Instance.phoneInteractable != null)
				{
					this.callTime = SessionData.Instance.gameTime + 0.1f;
					if (this.chosenAddress == null)
					{
						List<NewAddress> list = new List<NewAddress>();
						foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
						{
							if (!(newAddress == Player.Instance.currentGameLocation) && newAddress.residence == null && newAddress.telephones.Count > 0 && newAddress.company != null && newAddress.company.publicFacing && newAddress.company.IsOpenAtThisTime(this.callTime))
							{
								list.Add(newAddress);
							}
						}
						this.chosenAddress = list[Toolbox.Instance.Rand(0, list.Count, false)];
					}
					Game.Log("Job: Update data source details: " + this.chosenAddress.name + " and " + this.callTime.ToString(), 2);
				}
				if (this.phase != this.phaseChange)
				{
					this.phaseChange = this.phase;
					this.callTriggered = false;
					TelephoneController.Instance.fakeTelephoneDictionary.ContainsKey(this.fakeNumber);
					Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.makeCall, this.fakeNumber.ToString(), false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
					this.AddObjective("CallFake", trigger, false, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.triggerSideJobFunction, 0f, false, "", false, false);
					return;
				}
			}
			else if (this.phase == 1 && Player.Instance.answeringPhone == null)
			{
				if (this.phase != this.phaseChange)
				{
					this.phaseChange = this.phase;
					Game.Log("Jobs: Player called fake number...", 2);
					this.callTriggered = false;
					this.getToPhone = null;
				}
				if (!this.callTriggered && SessionData.Instance.gameTime >= this.callTime)
				{
					Game.Log("Jobs: Trigger the telephone call at " + this.chosenAddress.name, 2);
					if (this.chosenAddress.telephones.Find((Telephone item) => item.interactable.preset.isPayphone) == null)
					{
						Game.Log("Jobs: Cannot find payphone at " + this.chosenAddress.name + "...", 2);
					}
					List<Telephone> list2 = new List<Telephone>(CityData.Instance.phoneDictionary.Values);
					List<Telephone> list3 = list2.FindAll((Telephone item) => item.interactable != null && item.interactable.preset.isPayphone && item.interactable.node.gameLocation.thisAsStreet != null);
					Telephone telephone = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
					if (list3.Count > 0)
					{
						Telephone telephone2 = list3[Toolbox.Instance.Rand(0, list3.Count, false)];
					}
					else
					{
						list3 = list2.FindAll((Telephone item) => item.interactable != null && item.interactable.preset.isPayphone);
						if (list3.Count > 0)
						{
							Telephone telephone3 = list3[Toolbox.Instance.Rand(0, list3.Count, false)];
						}
					}
					this.callTriggered = true;
					return;
				}
				if (!this.callTriggered && Player.Instance.phoneInteractable == null && this.getToPhone == null)
				{
					this.getToPhone = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.chosenAddress, null, "", false, default(Vector3));
					this.AddObjective("GetToPhone", this.getToPhone, false, default(Vector3), InterfaceControls.Icon.run, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
					return;
				}
				if (this.callTriggered && this.call != null && this.call.state == TelephoneController.CallState.ended)
				{
					if (this.call.recevierNS == Player.Instance)
					{
						this.phase = 2;
						return;
					}
					this.phase = 3;
					return;
				}
			}
			else if (this.phase == 2)
			{
				if (this.phase != this.phaseChange)
				{
					this.phaseChange = this.phase;
					TelephoneController.Instance.RemoveFakeNumber(this.fakeNumber);
					Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.evidencePinned, "", false, 0f, null, null, this.activeJobItems[JobPreset.JobTag.A].evidence, null, null, null, null, "", false, default(Vector3));
					this.AddObjective("SabotageRecoverInfo", trigger2, false, default(Vector3), InterfaceControls.Icon.pin, Objective.OnCompleteAction.triggerSideJobFunction, 0f, false, "", false, false);
					Game.Log("Jobs: The player got the call. Setting up objectives...", 2);
					return;
				}
			}
			else if (this.phase == 3 && this.phase != this.phaseChange)
			{
				this.phaseChange = this.phase;
				this.getToPhone.Trigger(false);
				Game.Log("Jobs: The player missed the call, sending them back to phase 0...", 2);
				this.phase = 0;
			}
		}
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x001017E8 File Offset: 0x000FF9E8
	public override void UpdateResolveAnswers()
	{
		foreach (Case.ResolveQuestion resolveQuestion in this.thisCase.resolveQuestions)
		{
			resolveQuestion.correctAnswers.Clear();
		}
		Case.ResolveQuestion resolveQuestion2 = this.thisCase.resolveQuestions.Find((Case.ResolveQuestion item) => item.tag == JobPreset.JobTag.A);
		if (this.activeJobItems.ContainsKey(JobPreset.JobTag.A))
		{
			resolveQuestion2.correctAnswers.Add(this.activeJobItems[JobPreset.JobTag.A].id.ToString());
		}
		base.UpdateResolveAnswers();
	}

	// Token: 0x0400161C RID: 5660
	public NewAddress chosenAddress;

	// Token: 0x0400161D RID: 5661
	public float callTime;

	// Token: 0x0400161E RID: 5662
	private bool callTriggered;

	// Token: 0x0400161F RID: 5663
	private TelephoneController.PhoneCall call;

	// Token: 0x04001620 RID: 5664
	private Objective.ObjectiveTrigger getToPhone;
}
