using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class DialogController : MonoBehaviour
{
	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000777 RID: 1911 RVA: 0x00071E21 File Offset: 0x00070021
	public static DialogController Instance
	{
		get
		{
			return DialogController._instance;
		}
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00071E28 File Offset: 0x00070028
	private void Start()
	{
		if (DialogController._instance != null && DialogController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			DialogController._instance = this;
		}
		foreach (DialogPreset dialogPreset in Toolbox.Instance.allDialog)
		{
			MethodInfo method = base.GetType().GetMethod(dialogPreset.name);
			if (method != null)
			{
				this.dialogRef.Add(dialogPreset, method);
			}
		}
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00071ED0 File Offset: 0x000700D0
	public bool ExecuteDialog(EvidenceWitness.DialogOption dialog, Interactable saysTo, NewNode where, Actor saidBy, DialogController.ForceSuccess forceSuccess = DialogController.ForceSuccess.none)
	{
		MethodInfo methodInfo = null;
		bool success = true;
		this.sideJobReference = dialog.jobRef;
		this.preset = dialog.preset;
		this.cit = null;
		if (saysTo.isActor != null)
		{
			this.cit = (saysTo.isActor as Citizen);
		}
		Predicate<SideJob> <>9__3;
		Predicate<SideJob> <>9__6;
		if (forceSuccess == DialogController.ForceSuccess.success)
		{
			success = true;
		}
		else if (forceSuccess == DialogController.ForceSuccess.fail)
		{
			success = false;
		}
		else if (SideJobController.Instance.jobTracking.Exists(delegate(SideJobController.JobTracking item)
		{
			if (item.preset.changePosterDialogCompliancy != JobPreset.ParticipantCompliancy.noChange)
			{
				List<SideJob> activeJobs = item.activeJobs;
				Predicate<SideJob> predicate;
				if ((predicate = <>9__3) == null)
				{
					predicate = (<>9__3 = ((SideJob item) => item.accepted && item.poster == this.cit));
				}
				return activeJobs.Exists(predicate);
			}
			return false;
		}))
		{
			Predicate<SideJob> <>9__5;
			SideJobController.JobTracking jobTracking = SideJobController.Instance.jobTracking.Find(delegate(SideJobController.JobTracking item)
			{
				if (item.preset.changePosterDialogCompliancy != JobPreset.ParticipantCompliancy.noChange)
				{
					List<SideJob> activeJobs = item.activeJobs;
					Predicate<SideJob> predicate;
					if ((predicate = <>9__5) == null)
					{
						predicate = (<>9__5 = ((SideJob item) => item.accepted && item.poster == this.cit));
					}
					return activeJobs.Exists(predicate);
				}
				return false;
			});
			if (jobTracking != null)
			{
				if (jobTracking.preset.changePosterDialogCompliancy == JobPreset.ParticipantCompliancy.alwaysSuccess)
				{
					success = true;
				}
				else if (jobTracking.preset.changePosterDialogCompliancy == JobPreset.ParticipantCompliancy.alwaysFail)
				{
					success = false;
				}
			}
		}
		else if (SideJobController.Instance.jobTracking.Exists(delegate(SideJobController.JobTracking item)
		{
			if (item.preset.changePerpDialogCompliancy != JobPreset.ParticipantCompliancy.noChange)
			{
				List<SideJob> activeJobs = item.activeJobs;
				Predicate<SideJob> predicate;
				if ((predicate = <>9__6) == null)
				{
					predicate = (<>9__6 = ((SideJob item) => item.accepted && item.purp == this.cit));
				}
				return activeJobs.Exists(predicate);
			}
			return false;
		}))
		{
			Predicate<SideJob> <>9__8;
			SideJobController.JobTracking jobTracking2 = SideJobController.Instance.jobTracking.Find(delegate(SideJobController.JobTracking item)
			{
				if (item.preset.changePerpDialogCompliancy != JobPreset.ParticipantCompliancy.noChange)
				{
					List<SideJob> activeJobs = item.activeJobs;
					Predicate<SideJob> predicate;
					if ((predicate = <>9__8) == null)
					{
						predicate = (<>9__8 = ((SideJob item) => item.accepted && item.purp == this.cit));
					}
					return activeJobs.Exists(predicate);
				}
				return false;
			});
			if (jobTracking2 != null)
			{
				if (jobTracking2.preset.changePerpDialogCompliancy == JobPreset.ParticipantCompliancy.alwaysSuccess)
				{
					success = true;
				}
				else if (jobTracking2.preset.changePerpDialogCompliancy == JobPreset.ParticipantCompliancy.alwaysFail)
				{
					success = false;
				}
			}
		}
		else if (dialog.preset.useSuccessTest && (this.cit == null || !this.cit.alwaysPassDialogSuccess))
		{
			float num = dialog.preset.baseChance;
			if (dialog.preset.modifySuccessChanceTraits.Count > 0 && this.cit != null)
			{
				num = this.cit.GetChance(ref dialog.preset.modifySuccessChanceTraits, num);
			}
			if (this.cit != null && this.cit.ai != null && this.cit.ai.restrained)
			{
				num += dialog.preset.affectChanceIfRestrained;
			}
			if (this.preset.specialCase == DialogPreset.SpecialCase.lookAroundHome)
			{
				num += UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.guestPassIssueModifier);
			}
			num = Mathf.Clamp01(num + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.dialogChanceModifier));
			if (this.preset.specialCase == DialogPreset.SpecialCase.mustBeMurdererForSuccess && MurderController.Instance.currentMurderer != this.cit)
			{
				num = 0f;
				success = false;
			}
			if (Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, saysTo.name, false) <= num)
			{
				success = true;
			}
			else
			{
				success = false;
			}
			if (dialog.preset.requiresPassword && this.cit != null && this.cit.currentGameLocation != null && this.cit.currentGameLocation.thisAsAddress != null && this.cit.currentGameLocation.thisAsAddress.addressPreset != null && this.cit.currentGameLocation.thisAsAddress.addressPreset.needsPassword && !GameplayController.Instance.playerKnowsPasswords.Contains(this.cit.currentGameLocation.thisAsAddress.id))
			{
				success = false;
				Game.Log("Player does not know password...", 2);
			}
		}
		if (dialog.preset.specialCase == DialogPreset.SpecialCase.talkingToJobPoster)
		{
			success = false;
			if (dialog.jobRef != null && dialog.jobRef.poster == this.cit)
			{
				success = true;
			}
		}
		else
		{
			if (dialog.preset.specialCase == DialogPreset.SpecialCase.lastCaller)
			{
				success = false;
				using (List<TelephoneController.PhoneCall>.Enumerator enumerator = where.building.callLog.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TelephoneController.PhoneCall phoneCall = enumerator.Current;
						if (phoneCall.toNS != null && phoneCall.toNS.location == where.gameLocation)
						{
							success = true;
							break;
						}
					}
					goto IL_721;
				}
			}
			if (dialog.preset.specialCase == DialogPreset.SpecialCase.returnJobItemA)
			{
				success = false;
				if (this.sideJobReference == null)
				{
					goto IL_721;
				}
				Interactable item2 = this.sideJobReference.GetItem(JobPreset.JobTag.A);
				if (item2 == null)
				{
					goto IL_721;
				}
				if (InteractionController.Instance.talkingTo != null && InteractionController.Instance.talkingTo == this.sideJobReference.poster.interactable)
				{
					int i = 0;
					while (i < FirstPersonItemController.Instance.slots.Count)
					{
						FirstPersonItemController.InventorySlot inventorySlot = FirstPersonItemController.Instance.slots[i];
						Interactable interactable = inventorySlot.GetInteractable();
						if (interactable != null && interactable == item2)
						{
							success = true;
							FirstPersonItemController.Instance.EmptySlot(inventorySlot, false, false, true, true);
							SideJobStolenItem sideJobStolenItem = this.sideJobReference as SideJobStolenItem;
							if (sideJobStolenItem != null)
							{
								sideJobStolenItem.ReturnItem();
								break;
							}
							break;
						}
						else
						{
							i++;
						}
					}
				}
				if (success)
				{
					goto IL_721;
				}
				Interactable mailbox = Toolbox.Instance.GetMailbox(this.sideJobReference.poster);
				if (mailbox == null)
				{
					goto IL_721;
				}
				using (List<Interactable>.Enumerator enumerator2 = mailbox.node.interactables.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Interactable interactable2 = enumerator2.Current;
						if (interactable2.preset == item2.preset && interactable2.inInventory == null && Vector3.Distance(interactable2.GetWorldPosition(true), mailbox.GetWorldPosition(true)) <= 0.5f)
						{
							success = true;
							break;
						}
					}
					goto IL_721;
				}
			}
			if (dialog.preset.specialCase == DialogPreset.SpecialCase.starchPitch)
			{
				if (success)
				{
					int addVal = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.starchAmbassador));
					GameplayController.Instance.AddMoney(addVal, true, "Starch pitch");
				}
			}
			else if (dialog.preset.specialCase == DialogPreset.SpecialCase.loanSharkAsk || dialog.preset.specialCase == DialogPreset.SpecialCase.loanSharkAccept)
			{
				if (this.cit.job != null && this.cit.job.employer != null && GameplayController.Instance.debt.Exists((GameplayController.LoanDebt item) => item.companyID == this.cit.job.employer.companyID))
				{
					Game.Log("Debug: Cannot find existing debt, returning dialog unsuccessful: " + this.cit.GetCitizenName(), 2);
					success = false;
				}
				else
				{
					Game.Log("Debug: Cannot find existing debt, returning dialog success: " + this.cit.GetCitizenName(), 2);
					success = true;
				}
			}
			else if (dialog.preset.specialCase == DialogPreset.SpecialCase.loanSharkPayment && this.cit.job != null && this.cit.job.employer != null)
			{
				GameplayController.LoanDebt loanDebt = GameplayController.Instance.debt.Find((GameplayController.LoanDebt item) => item.companyID == this.cit.job.employer.companyID);
				if (loanDebt != null)
				{
					if (loanDebt.debt > 0)
					{
						success = false;
					}
					else
					{
						success = true;
					}
				}
				else
				{
					success = true;
				}
			}
		}
		IL_721:
		if (this.dialogRef.TryGetValue(dialog.preset, ref methodInfo))
		{
			object[] array = new object[]
			{
				this.cit,
				saysTo,
				where,
				saidBy,
				success,
				dialog.roomRef,
				dialog.jobRef
			};
			methodInfo.Invoke(this, array);
		}
		SpeechController speechController = saysTo.speechController;
		if (InteractionController.Instance.isRemote && InteractionController.Instance.remoteOverride != null)
		{
			speechController = InteractionController.Instance.remoteOverride.speechController;
		}
		if (speechController != null)
		{
			List<AIActionPreset.AISpeechPreset> list = dialog.preset.responses.FindAll((AIActionPreset.AISpeechPreset item) => !this.preset.useSuccessTest || item.isSuccessful == success);
			speechController.Speak(ref list, null, dialog.jobRef, dialog.preset, saysTo);
		}
		else
		{
			string[] array2 = new string[6];
			array2[0] = "Speech controller for ";
			array2[1] = saysTo.id.ToString();
			array2[2] = " is null. IsRemote: ";
			array2[3] = InteractionController.Instance.isRemote.ToString();
			array2[4] = " = Override: ";
			int num2 = 5;
			Interactable remoteOverride = InteractionController.Instance.remoteOverride;
			array2[num2] = ((remoteOverride != null) ? remoteOverride.ToString() : null);
			Game.LogError(string.Concat(array2), 2);
		}
		if (dialog.preset.removeAfterSaying)
		{
			(saysTo.evidence as EvidenceWitness).RemoveDialogOption(dialog.preset.tiedToKey, dialog.preset, dialog.jobRef, null);
		}
		if ((dialog.jobRef != null & success) && dialog.jobRef.thisCase != null)
		{
			using (List<Objective>.Enumerator enumerator3 = dialog.jobRef.thisCase.currentActiveObjectives.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Objective objective = enumerator3.Current;
					foreach (Objective.ObjectiveTrigger objectiveTrigger in objective.queueElement.triggers)
					{
						if (objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.onDialogSuccess && objectiveTrigger.name.ToLower() == dialog.preset.name.ToLower())
						{
							objectiveTrigger.Trigger(false);
						}
					}
				}
				goto IL_A32;
			}
		}
		if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
		{
			ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
			if (chapterIntro != null)
			{
				foreach (Objective objective2 in chapterIntro.thisCase.currentActiveObjectives)
				{
					foreach (Objective.ObjectiveTrigger objectiveTrigger2 in objective2.queueElement.triggers)
					{
						if (objectiveTrigger2.triggerType == Objective.ObjectiveTriggerType.onDialogSuccess && objectiveTrigger2.name.ToLower() == dialog.preset.name.ToLower())
						{
							objectiveTrigger2.Trigger(false);
						}
					}
				}
			}
		}
		IL_A32:
		if ((dialog.preset.isJobDetails && dialog.jobRef != null) & success)
		{
			dialog.jobRef.OnAcquireJobInfo(dialog.preset);
		}
		return success;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00072994 File Offset: 0x00070B94
	public bool TestSpecialCaseAvailability(DialogPreset preset, Citizen saysTo, SideJob jobRef)
	{
		bool flag = false;
		if (preset.specialCase == DialogPreset.SpecialCase.neverDisplay)
		{
			return false;
		}
		if (preset.specialCase == DialogPreset.SpecialCase.mugging)
		{
			flag = (InteractionController.Instance.dialogType == InteractionController.ConversationType.mugging);
		}
		else if (InteractionController.Instance.dialogType == InteractionController.ConversationType.mugging)
		{
			return false;
		}
		if (preset.specialCase == DialogPreset.SpecialCase.killerCleanUp || preset.specialCase == DialogPreset.SpecialCase.killerCleanUpAccept || preset.specialCase == DialogPreset.SpecialCase.killerCleanUpReject)
		{
			flag = (InteractionController.Instance.dialogType == InteractionController.ConversationType.killerCleanUp);
		}
		else if (InteractionController.Instance.dialogType == InteractionController.ConversationType.killerCleanUp)
		{
			return false;
		}
		if (preset.specialCase == DialogPreset.SpecialCase.ransomInvestigate)
		{
			foreach (MurderController.Murder murder in MurderController.Instance.activeMurders)
			{
				if (murder.preset != null && murder.preset.caseType == MurderPreset.CaseType.kidnap)
				{
					Interactable interactable = null;
					if (murder.activeMurderItems.TryGetValue(JobPreset.JobTag.U, ref interactable) && saysTo != null && saysTo.currentGameLocation == interactable.node.gameLocation)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		if (preset.specialCase == DialogPreset.SpecialCase.kidnapperOnly)
		{
			foreach (MurderController.Murder murder2 in MurderController.Instance.activeMurders)
			{
				if (murder2.preset != null && murder2.preset.caseType == MurderPreset.CaseType.kidnap && murder2.murderer == saysTo)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		if (preset.specialCase == DialogPreset.SpecialCase.loanSharkPayment || preset.specialCase == DialogPreset.SpecialCase.loanSharkPaymentRefuse || preset.specialCase == DialogPreset.SpecialCase.loanSharkAsk || preset.specialCase == DialogPreset.SpecialCase.loanSharkAccept)
		{
			flag = (InteractionController.Instance.dialogType == InteractionController.ConversationType.loanSharkVisit);
		}
		else if (InteractionController.Instance.dialogType == InteractionController.ConversationType.loanSharkVisit)
		{
			return false;
		}
		if (preset.specialCase == DialogPreset.SpecialCase.none)
		{
			flag = true;
		}
		else if (preset.specialCase == DialogPreset.SpecialCase.medicalCosts)
		{
			flag = true;
		}
		else if (preset.specialCase == DialogPreset.SpecialCase.backroomBribe)
		{
			if (saysTo != null && saysTo.currentGameLocation.thisAsAddress != null && saysTo.job != null && saysTo.job.employer != null && saysTo.currentGameLocation.thisAsAddress.company != null && saysTo.currentGameLocation.thisAsAddress.company.openForBusinessDesired && saysTo.currentGameLocation.thisAsAddress.company.openForBusinessActual && saysTo.currentGameLocation == saysTo.job.employer.placeOfBusiness)
			{
				int num = 0;
				foreach (NewRoom newRoom in saysTo.currentGameLocation.rooms)
				{
					if (newRoom.passcode.used && !newRoom.belongsTo.Contains(saysTo))
					{
						num++;
					}
				}
				if (num > 0)
				{
					flag = true;
				}
			}
		}
		else if (preset.specialCase == DialogPreset.SpecialCase.publicFacingWorkplace)
		{
			if (saysTo != null && saysTo.currentGameLocation.thisAsAddress != null && saysTo.job != null && saysTo.job.employer != null && saysTo.job.employer.publicFacing && saysTo.job.employer.openForBusinessDesired && saysTo.currentGameLocation == saysTo.job.employer.placeOfBusiness)
			{
				flag = true;
			}
		}
		else if (preset.specialCase == DialogPreset.SpecialCase.working)
		{
			if (saysTo != null && saysTo.isAtWork && saysTo.currentGameLocation.thisAsAddress != null && saysTo.job != null && saysTo.job.employer != null)
			{
				flag = true;
			}
		}
		else if (preset.specialCase == DialogPreset.SpecialCase.workingGuestPass)
		{
			if (saysTo != null && saysTo.isAtWork && saysTo.currentGameLocation.thisAsAddress != null && saysTo.job != null && saysTo.job.employer != null && saysTo.job.employer.placeOfBusiness.thisAsAddress != null && !GameplayController.Instance.guestPasses.ContainsKey(saysTo.job.employer.placeOfBusiness.thisAsAddress))
			{
				flag = true;
			}
		}
		else if (preset.specialCase == DialogPreset.SpecialCase.lookAroundHome)
		{
			if (saysTo != null && saysTo.home != null && saysTo.currentGameLocation.thisAsAddress != null && saysTo.currentGameLocation == saysTo.home && !GameplayController.Instance.guestPasses.ContainsKey(saysTo.home))
			{
				flag = true;
			}
		}
		else if (preset.specialCase == DialogPreset.SpecialCase.starchPitch)
		{
			flag = (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.starchAmbassador) > 0f);
		}
		else
		{
			if (preset.specialCase == DialogPreset.SpecialCase.returnJobItemA)
			{
				if (!(saysTo != null) || jobRef == null)
				{
					goto IL_B93;
				}
				Interactable item2 = jobRef.GetItem(JobPreset.JobTag.A);
				if (item2 == null)
				{
					goto IL_B93;
				}
				if (InteractionController.Instance.talkingTo != null && InteractionController.Instance.talkingTo == this.sideJobReference.poster.interactable)
				{
					int i = 0;
					while (i < FirstPersonItemController.Instance.slots.Count)
					{
						FirstPersonItemController.InventorySlot inventorySlot = FirstPersonItemController.Instance.slots[i];
						Interactable interactable2 = inventorySlot.GetInteractable();
						if (interactable2 != null && interactable2 == item2)
						{
							flag = true;
							FirstPersonItemController.Instance.EmptySlot(inventorySlot, false, false, true, true);
							SideJobStolenItem sideJobStolenItem = this.sideJobReference as SideJobStolenItem;
							if (sideJobStolenItem != null)
							{
								sideJobStolenItem.ReturnItem();
								break;
							}
							break;
						}
						else
						{
							i++;
						}
					}
				}
				if (flag)
				{
					goto IL_B93;
				}
				Interactable mailbox = Toolbox.Instance.GetMailbox(this.sideJobReference.poster);
				if (mailbox == null)
				{
					goto IL_B93;
				}
				using (List<Interactable>.Enumerator enumerator3 = mailbox.node.interactables.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Interactable interactable3 = enumerator3.Current;
						if (interactable3.preset == item2.preset && interactable3.inInventory == null && Vector3.Distance(interactable3.GetWorldPosition(true), mailbox.GetWorldPosition(true)) <= 0.5f)
						{
							flag = true;
							break;
						}
					}
					goto IL_B93;
				}
			}
			if (preset.specialCase == DialogPreset.SpecialCase.callInSuspect)
			{
				if (CityData.Instance.citizenDirectory.Exists((Citizen item) => item.ai != null && item.ai.restrained))
				{
					flag = true;
				}
			}
			else
			{
				if (preset.specialCase == DialogPreset.SpecialCase.talkingToJobPoster)
				{
					using (Dictionary<int, SideJob>.Enumerator enumerator4 = SideJobController.Instance.allJobsDictionary.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							KeyValuePair<int, SideJob> keyValuePair = enumerator4.Current;
							if (keyValuePair.Value.accepted && keyValuePair.Value.state == SideJob.JobState.posted && keyValuePair.Value.poster.home == saysTo.home && saysTo.isHome)
							{
								flag = true;
								break;
							}
						}
						goto IL_B93;
					}
				}
				if (preset.specialCase == DialogPreset.SpecialCase.lastCaller)
				{
					flag = true;
				}
				else if (preset.specialCase == DialogPreset.SpecialCase.knowName)
				{
					if (saysTo != null && saysTo.evidenceEntry.GetTiedKeys(Evidence.DataKey.photo).Contains(Evidence.DataKey.voice))
					{
						flag = true;
					}
				}
				else if (preset.specialCase == DialogPreset.SpecialCase.loanSharkPayment || preset.specialCase == DialogPreset.SpecialCase.loanSharkPaymentRefuse)
				{
					if (saysTo != null && saysTo.job != null && saysTo.job.employer != null)
					{
						GameplayController.LoanDebt loanDebt = GameplayController.Instance.debt.Find((GameplayController.LoanDebt item) => item.companyID == saysTo.job.employer.companyID);
						flag = (loanDebt != null && SessionData.Instance.gameTime >= loanDebt.nextPaymentDueBy - 24f);
					}
					else
					{
						flag = false;
					}
				}
				else if (preset.specialCase == DialogPreset.SpecialCase.loanSharkAsk || preset.specialCase == DialogPreset.SpecialCase.loanSharkAccept)
				{
					if (saysTo != null && saysTo.job != null && saysTo.job.employer != null && saysTo.isAtWork)
					{
						flag = true;
					}
				}
				else if (preset.specialCase == DialogPreset.SpecialCase.revealHiddenitemPhoto)
				{
					flag = true;
				}
				else if (preset.specialCase == DialogPreset.SpecialCase.mustBeMurdererForSuccess)
				{
					flag = true;
				}
				else if (preset.specialCase == DialogPreset.SpecialCase.hotelBill || preset.specialCase == DialogPreset.SpecialCase.hotelCheckOut || preset.specialCase == DialogPreset.SpecialCase.mustHaveRoomAtHotel)
				{
					if (saysTo != null && saysTo.isAtWork && saysTo.currentGameLocation.thisAsAddress != null && saysTo.job != null && saysTo.job.employer != null && saysTo.job.employer.placeOfBusiness.thisAsAddress != null)
					{
						if (preset.specialCase == DialogPreset.SpecialCase.hotelBill)
						{
							if (GameplayController.Instance.hotelGuests.Exists((GameplayController.HotelGuest item) => item.humanID == Player.Instance.humanID && item.GetAddress() != null && item.bill > 0 && item.GetAddress().building == saysTo.job.employer.placeOfBusiness.thisAsAddress.building))
							{
								flag = true;
							}
						}
						else if (GameplayController.Instance.hotelGuests.Exists((GameplayController.HotelGuest item) => item.humanID == Player.Instance.humanID && item.GetAddress() != null && item.GetAddress().building == saysTo.job.employer.placeOfBusiness.thisAsAddress.building))
						{
							flag = true;
						}
					}
				}
				else if ((preset.specialCase == DialogPreset.SpecialCase.hotelRentRoom || preset.specialCase == DialogPreset.SpecialCase.rentHotelRoomCheap || preset.specialCase == DialogPreset.SpecialCase.rentHotelRoomExpensive) && saysTo != null && saysTo.isAtWork && saysTo.currentGameLocation.thisAsAddress != null && saysTo.job != null && saysTo.job.employer != null && saysTo.job.employer.placeOfBusiness.thisAsAddress != null && !GameplayController.Instance.hotelGuests.Exists((GameplayController.HotelGuest item) => item.humanID == Player.Instance.humanID && item.GetAddress() != null && item.GetAddress().building == saysTo.job.employer.placeOfBusiness.thisAsAddress.building))
				{
					flag = true;
				}
			}
		}
		IL_B93:
		if (preset.displayIfPasswordUnknown)
		{
			if (saysTo != null && saysTo.currentGameLocation != null && saysTo.currentGameLocation.thisAsAddress != null && saysTo.currentGameLocation.thisAsAddress.addressPreset != null && !saysTo.currentGameLocation.thisAsAddress.addressPreset.needsPassword)
			{
				flag = false;
			}
			else if (saysTo != null && saysTo.currentGameLocation != null && saysTo.currentGameLocation.thisAsAddress != null && saysTo.currentGameLocation.thisAsAddress.addressPreset != null && saysTo.currentGameLocation.thisAsAddress.addressPreset.needsPassword && GameplayController.Instance.playerKnowsPasswords.Contains(saysTo.currentGameLocation.thisAsAddress.id))
			{
				flag = false;
			}
		}
		return flag;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00073698 File Offset: 0x00071898
	public void OnDialogEnd(AIActionPreset.AISpeechPreset dialog, string dialogPresetStr, Interactable saysToInteractable, Actor saidBy, int jobRef)
	{
		DialogPreset dialogPreset = null;
		if (Toolbox.Instance.LoadDataFromResources<DialogPreset>(dialogPresetStr, out dialogPreset))
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("ON END DIALOG: " + dialogPreset.name, 2);
			}
			if (dialogPreset.specialCase == DialogPreset.SpecialCase.revealHiddenitemPhoto)
			{
				SideJob sideJob = null;
				if (!SideJobController.Instance.allJobsDictionary.TryGetValue(jobRef, ref sideJob))
				{
					Game.LogError("Unable to find sidejob " + jobRef.ToString(), 2);
					return;
				}
				if (Game.Instance.printDebug)
				{
					Game.Log("Jobs: Revealing hidden briefcase photograph...", 2);
				}
				if (sideJob.hiddenItemPhoto != null)
				{
					ActionController.Instance.Inspect(sideJob.hiddenItemPhoto, Player.Instance.currentNode, Player.Instance);
					CasePanelController.Instance.PinToCasePanel(sideJob.thisCase, sideJob.hiddenItemPhoto.evidence, Evidence.DataKey.photo, true, default(Vector2), false);
					return;
				}
				return;
			}
			else
			{
				if (dialogPreset.specialCase == DialogPreset.SpecialCase.killerCleanUpReject)
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("Murder: Player has rejected the cover up objective", 2);
					}
					MurderController.Instance.playerAcceptedCoverUp = false;
					MurderController.Instance.OnCoverUpReject();
					return;
				}
				if (dialogPreset.specialCase == DialogPreset.SpecialCase.killerCleanUpAccept)
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("Murder: Player has accepted the cover up objective", 2);
					}
					MurderController.Instance.playerAcceptedCoverUp = true;
					MurderController.Instance.OnCoverUpAccept();
					MurderController.Instance.TriggerCoverUpObjective();
					return;
				}
				if (dialogPreset.specialCase == DialogPreset.SpecialCase.killerCleanUpSuccess)
				{
					MurderController.Instance.OnCoverUpSuccessEnd();
					return;
				}
				if (dialogPreset.specialCase != DialogPreset.SpecialCase.ransomInvestigate)
				{
					return;
				}
				using (List<MurderController.Murder>.Enumerator enumerator = MurderController.Instance.activeMurders.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MurderController.Murder murder = enumerator.Current;
						if (murder.preset != null && murder.preset.caseType == MurderPreset.CaseType.kidnap)
						{
							Interactable interactable = null;
							if (murder.activeMurderItems.TryGetValue(JobPreset.JobTag.U, ref interactable) && saidBy != null && saidBy.currentGameLocation == interactable.node.gameLocation)
							{
								SessionData.Instance.PauseGame(true, false, true);
								InterfaceController.Instance.SpawnWindow(interactable.evidence, Evidence.DataKey.name, null, "", true, true, default(Vector2), null, null, null, true);
							}
						}
					}
					return;
				}
			}
		}
		Game.LogError("Unable to find dialog preset " + dialogPresetStr, 2);
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00073904 File Offset: 0x00071B04
	public void BribeForCode(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && saysTo.currentGameLocation.thisAsAddress != null && saysTo.job != null && saysTo.job.employer != null && saysTo.currentGameLocation == saysTo.job.employer.placeOfBusiness && saysTo.job.employer.placeOfBusiness.thisAsAddress != null)
		{
			foreach (NewRoom newRoom in saysTo.currentGameLocation.rooms)
			{
				if (newRoom.passcode.used)
				{
					newRoom.belongsTo.Contains(saysTo);
				}
			}
		}
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x000739E4 File Offset: 0x00071BE4
	public void Beg(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success)
		{
			GameplayController.Instance.AddMoney(Toolbox.Instance.Rand(1, 3, false), true, "Beg");
		}
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00073A08 File Offset: 0x00071C08
	public void PayForCode(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (saysTo != null && saysTo.currentGameLocation.thisAsAddress != null && saysTo.job != null && saysTo.job.employer != null && saysTo.currentGameLocation.thisAsAddress.company != null && saysTo.currentGameLocation.thisAsAddress.company.openForBusinessDesired && saysTo.currentGameLocation == saysTo.job.employer.placeOfBusiness)
		{
			foreach (NewRoom newRoom in saysTo.currentGameLocation.rooms)
			{
				if (newRoom.passcode.used)
				{
					foreach (NewNode.NodeAccess nodeAccess in newRoom.entrances)
					{
						if (nodeAccess.door != null && nodeAccess.door.passwordDoorsRoom != null && nodeAccess.door.passwordDoorsRoom.passcode != null)
						{
							GameplayController.Instance.AddPasscode(nodeAccess.door.passwordDoorsRoom.passcode, true);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00073B84 File Offset: 0x00071D84
	public void IssueGuestPass(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && saysTo.job != null && saysTo.job.employer != null && saysTo.currentGameLocation == saysTo.job.employer.placeOfBusiness && saysTo.job.employer.placeOfBusiness.thisAsAddress != null)
		{
			GameplayController.Instance.AddGuestPass(saysTo.job.employer.placeOfBusiness.thisAsAddress, 2f);
		}
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00073C08 File Offset: 0x00071E08
	public void DoYouKnowThisPerson(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success)
		{
			Game.Log("Interface: Spawn photo select...", 2);
			SessionData.Instance.PauseGame(true, false, true);
			this.askTarget = null;
			this.askTargetKeys.Clear();
			this.askWindow = InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "SelectPhoto", true, false, InterfaceControls.Instance.handbookWindowPosition, null, null, null, success);
		}
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00073C6C File Offset: 0x00071E6C
	public void DoYouKnowThisPersonBribe1(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success)
		{
			Game.Log("Interface: Spawn photo select...", 2);
			SessionData.Instance.PauseGame(true, false, true);
			this.askTarget = null;
			this.askTargetKeys.Clear();
			this.askWindow = InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "SelectPhoto", true, false, InterfaceControls.Instance.handbookWindowPosition, null, null, null, success);
		}
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00073CD0 File Offset: 0x00071ED0
	public void DoYouKnowThisPersonBribe2(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success)
		{
			Game.Log("Interface: Spawn photo select...", 2);
			SessionData.Instance.PauseGame(true, false, true);
			this.askTarget = null;
			this.askTargetKeys.Clear();
			this.askWindow = InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "SelectPhoto", true, false, InterfaceControls.Instance.handbookWindowPosition, null, null, null, success);
		}
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x00073D34 File Offset: 0x00071F34
	public void DoYouKnowThisPersonBribe3(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success)
		{
			Game.Log("Interface: Spawn photo select...", 2);
			SessionData.Instance.PauseGame(true, false, true);
			this.askTarget = null;
			this.askTargetKeys.Clear();
			this.askWindow = InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "SelectPhoto", true, false, InterfaceControls.Instance.handbookWindowPosition, null, null, null, success);
		}
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00073D98 File Offset: 0x00071F98
	public void BuySomething(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success)
		{
			Game.Log("Interface: Spawn buy select...", 2);
			SessionData.Instance.PauseGame(true, false, true);
			string presetName = "Buy";
			if (saysTo != null && saysTo.job != null && saysTo.job.employer != null && saysTo.job.employer.preset.enableSelling)
			{
				presetName = "BuySell";
			}
			InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, presetName, true, false, InterfaceControls.Instance.handbookWindowPosition, saysTo.interactable, null, null, true);
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00073E28 File Offset: 0x00072028
	public void PhoneKeypad(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success)
		{
			SessionData.Instance.PauseGame(true, false, true);
			InterfaceController.Instance.SpawnWindow(CityData.Instance.telephone, Evidence.DataKey.name, null, "", true, false, InterfaceControls.Instance.handbookWindowPosition, saysToInteractable, null, null, true);
		}
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00073E74 File Offset: 0x00072074
	public void IdentifyNumber(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && saysToInteractable.t != null)
		{
			where.gameLocation.evidenceEntry.AddDiscovery(Evidence.Discovery.phoneLocation);
			saysToInteractable.t.telephoneEntry.SetFound(true);
			Toolbox.Instance.SpawnWindowAfterSeconds(saysToInteractable.t.telephoneEntry, 3f);
		}
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x00073ECC File Offset: 0x000720CC
	public void LastCalled(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		Game.Log("Getting last called for " + saysToInteractable.id.ToString(), 2);
		TelephoneController.PhoneCall phoneCall = null;
		float num = -9999f;
		foreach (TelephoneController.PhoneCall phoneCall2 in where.building.callLog)
		{
			if (phoneCall2.toNS != null && phoneCall2.toNS.interactable == saysToInteractable && phoneCall2.time > num)
			{
				phoneCall = phoneCall2;
				num = phoneCall2.time;
			}
		}
		if (phoneCall != null)
		{
			SpeechController speechController = saysToInteractable.speechController;
			if (InteractionController.Instance.isRemote && InteractionController.Instance.remoteOverride != null)
			{
				SpeechController speechController2 = InteractionController.Instance.remoteOverride.speechController;
			}
			saysToInteractable.recentCallCheck = SessionData.Instance.gameTime;
			Evidence timeEvidence = EvidenceCreator.Instance.GetTimeEvidence(phoneCall.time, phoneCall.time, "TelephoneCall", phoneCall.fromNS.telephoneEntry.evID + ">" + phoneCall.toNS.telephoneEntry.evID, -1, -1);
			EvidenceCreator.Instance.CreateFact("CallFrom", phoneCall.fromNS.telephoneEntry, timeEvidence, null, null, true, null, null, null, false);
			EvidenceCreator.Instance.CreateFact("CallTo", timeEvidence, where.gameLocation.evidenceEntry, null, null, true, null, null, null, false);
			Toolbox.Instance.SpawnWindowAfterSeconds(timeEvidence, 5.5f);
			Toolbox.Instance.SpawnWindowAfterSeconds(phoneCall.fromNS.telephoneEntry, 5.5f);
			return;
		}
		saysToInteractable.recentCallCheck = -1f;
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x00074078 File Offset: 0x00072278
	public void Police(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success)
		{
			GameplayController.Instance.CallEnforcers(saidBy.currentGameLocation, false, false);
		}
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x00074094 File Offset: 0x00072294
	public void Escape(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		saysToInteractable.SetCustomState2(false, Player.Instance, false, false, false);
		Player.Instance.ClearAllDisabledActions();
		InteractionController.Instance.SetDialog(false, null, false, null, InteractionController.ConversationType.normal);
		AchievementsController.Instance.freeHealthCareFlag = true;
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, string.Concat(new string[]
		{
			SessionData.Instance.CurrentTimeString(false, true),
			", ",
			SessionData.Instance.LongDateString(SessionData.Instance.gameTime, true, true, true, true, true, false, false, true),
			", ",
			Player.Instance.currentGameLocation.name
		}), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00074154 File Offset: 0x00072354
	public void PayMedicalFees(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		saysToInteractable.SetCustomState2(false, Player.Instance, false, false, false);
		Player.Instance.ClearAllDisabledActions();
		InteractionController.Instance.SetDialog(false, null, false, null, InteractionController.ConversationType.normal);
		StatusController.Instance.SetDetainedInBuilding(Player.Instance.currentBuilding, false);
		AchievementsController.Instance.freeHealthCareFlag = false;
		Player.Instance.TriggerPlayerRecovery();
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x000741B4 File Offset: 0x000723B4
	public void WarnNotewriter(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (ChapterController.Instance.chapterScript != null)
		{
			ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
			if (chapterIntro != null)
			{
				chapterIntro.NotewriterLayLow();
			}
		}
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x00002265 File Offset: 0x00000465
	public void InstallNewSyncDiskSlot(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x000741F2 File Offset: 0x000723F2
	public void TakeALookAround(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && saysTo.currentGameLocation == saysTo.home)
		{
			GameplayController.Instance.AddGuestPass(saysTo.home, 2f);
		}
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x000741F2 File Offset: 0x000723F2
	public void TakeALookAroundBribe1(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && saysTo.currentGameLocation == saysTo.home)
		{
			GameplayController.Instance.AddGuestPass(saysTo.home, 2f);
		}
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x000741F2 File Offset: 0x000723F2
	public void TakeALookAroundBribe2(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && saysTo.currentGameLocation == saysTo.home)
		{
			GameplayController.Instance.AddGuestPass(saysTo.home, 2f);
		}
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x000741F2 File Offset: 0x000723F2
	public void TakeALookAroundBribe3(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && saysTo.currentGameLocation == saysTo.home)
		{
			GameplayController.Instance.AddGuestPass(saysTo.home, 2f);
		}
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x000741F2 File Offset: 0x000723F2
	public void TakeALookAroundBribe4(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && saysTo.currentGameLocation == saysTo.home)
		{
			GameplayController.Instance.AddGuestPass(saysTo.home, 2f);
		}
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x00074220 File Offset: 0x00072420
	public void Job_HouseMeet_StolenItem(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		GameplayController.Instance.AddGuestPass(saysTo.home, 6f);
		Player.Instance.AddToKeyring(saysTo.home, true);
		if (jobRef != null && jobRef.thisCase != null)
		{
			foreach (Human human in saysTo.home.inhabitants)
			{
				human.evidenceEntry.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.fingerprints);
				human.evidenceEntry.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.shoeSize);
				human.evidenceEntry.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.address);
				CasePanelController.Instance.PinToCasePanel(jobRef.thisCase, human.evidenceEntry, Evidence.DataKey.name, true, default(Vector2), false);
			}
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "You have acquired information about residents of: ", Strings.Casing.asIs, false, false, false, null) + saysTo.home.name, InterfaceControls.Icon.building, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x00074340 File Offset: 0x00072540
	public void SeenOrHeardUnusual(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		Game.Log("Seen or heard unusual...", 2);
		float num = -99999f;
		Human.Sighting sighting = null;
		Human human = null;
		foreach (KeyValuePair<Human, Human.Sighting> keyValuePair in saysTo.lastSightings)
		{
			if (keyValuePair.Value.poi && !keyValuePair.Value.phone && keyValuePair.Value.time > num)
			{
				num = keyValuePair.Value.time;
				human = keyValuePair.Key;
				sighting = keyValuePair.Value;
			}
		}
		if (sighting != null)
		{
			if (sighting.sound == 1)
			{
				saysTo.speechController.Speak("79b84ed9-5c40-4927-9b57-f096cea0930e", false, false, null, null);
			}
			else if (sighting.sound == 2)
			{
				saysTo.speechController.Speak("d2ac8d3d-1a62-410a-8a73-fd69965679e5", false, false, null, null);
			}
			else
			{
				saysTo.speechController.Speak("00b6772a-46d0-48cb-89fe-e212ce3b4cf8", false, false, null, null);
				List<string> list = new List<string>();
				if (human.descriptors.hairType != Descriptors.HairStyle.bald)
				{
					list.Add("3f0faad0-578d-401e-9fa4-5a92140cba6a");
				}
				list.Add("39cf63a9-132d-4369-81c7-59aed085f3a0");
				list.Add("05585825-104b-4ad2-91ab-4429daecbca1");
				if (human.characterTraits.Exists((Human.Trait item) => item.trait.name == "Affliction-ShortSighted" || item.trait.name == "Affliction-FarSighted"))
				{
					list.Add("72ddfd37-fa3b-4903-8671-e4c0d1403cee");
				}
				if (human.characterTraits.Exists((Human.Trait item) => item.trait.name == "Quirk-FacialHair"))
				{
					list.Add("40987a5c-c7cc-4cb0-b37e-aa28ac57d0cf");
				}
				saysTo.speechController.Speak(list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, saidBy.name, false)], false, false, human, null);
			}
			saysTo.RevealSighting(human, sighting);
			return;
		}
		saysTo.speechController.Speak("c266400b-fe2f-4363-a071-d37e7c58a09d", false, false, null, null);
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x00074540 File Offset: 0x00072740
	public void GivePassword(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (saysTo != null && saysTo.currentGameLocation != null && saysTo.currentGameLocation.thisAsAddress != null)
		{
			if (success)
			{
				GameplayController.Instance.SetPlayerKnowsPassword(saysTo.currentGameLocation.thisAsAddress);
				return;
			}
			saysTo.currentGameLocation.thisAsAddress.GetPassword();
		}
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x000745A2 File Offset: 0x000727A2
	public void MuggingAcquiesce(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (InteractionController.Instance.mugger == saysTo)
		{
			Game.Log("Resetting mug state", 2);
			InteractionController.Instance.mugger = null;
		}
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x000745CC File Offset: 0x000727CC
	public void LoanShark_AcceptLoan(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (saysTo != null && saysTo.job != null && saysTo.job.employer != null)
		{
			GameplayController.Instance.AddNewDebt(saysTo.job.employer, GameplayControls.Instance.defaultLoanAmount, GameplayControls.Instance.defaultLoanExtra, GameplayControls.Instance.defaultLoanRepayment);
		}
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0007462C File Offset: 0x0007282C
	public void LoanShark_Pay(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (saysTo != null && saysTo.job != null && saysTo.job.employer != null)
		{
			GameplayController.Instance.DebtPayment(saysTo.job.employer);
		}
		if (InteractionController.Instance.debtCollector == saysTo)
		{
			Game.Log("Resetting debt collector state", 2);
			InteractionController.Instance.debtCollector = null;
		}
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x00074694 File Offset: 0x00072894
	public void Give(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		Game.Log("Interface: Spawn item select...", 2);
		SessionData.Instance.PauseGame(true, false, true);
		InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "SelectItem", true, false, InterfaceControls.Instance.handbookWindowPosition, null, null, null, success);
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x000746E0 File Offset: 0x000728E0
	public void BuyBriefcase(Citizen saysTo, Interactable saysToInteractable, NewNode where, Actor saidBy, bool success, NewRoom roomRef, SideJob jobRef)
	{
		if (success && InteriorControls.Instance.briefcaseCustom != null)
		{
			Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.briefcaseCustom, Player.Instance, Player.Instance, Player.Instance, Player.Instance.transform.position, Vector3.zero, null, null, "");
			if (interactable != null)
			{
				FirstPersonItemController.Instance.PickUpItem(interactable, true, false, true, true, true);
			}
		}
	}

	// Token: 0x040007D0 RID: 2000
	public DialogPreset payForCode;

	// Token: 0x040007D1 RID: 2001
	public InfoWindow askWindow;

	// Token: 0x040007D2 RID: 2002
	public Human askTarget;

	// Token: 0x040007D3 RID: 2003
	public List<Evidence.DataKey> askTargetKeys = new List<Evidence.DataKey>();

	// Token: 0x040007D4 RID: 2004
	public Dictionary<DialogPreset, MethodInfo> dialogRef = new Dictionary<DialogPreset, MethodInfo>();

	// Token: 0x040007D5 RID: 2005
	[NonSerialized]
	public SideJob sideJobReference;

	// Token: 0x040007D6 RID: 2006
	[NonSerialized]
	public DialogPreset preset;

	// Token: 0x040007D7 RID: 2007
	[NonSerialized]
	public Citizen cit;

	// Token: 0x040007D8 RID: 2008
	private static DialogController _instance;

	// Token: 0x02000108 RID: 264
	public enum ForceSuccess
	{
		// Token: 0x040007DA RID: 2010
		none,
		// Token: 0x040007DB RID: 2011
		success,
		// Token: 0x040007DC RID: 2012
		fail
	}
}
