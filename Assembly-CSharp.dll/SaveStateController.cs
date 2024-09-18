using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class SaveStateController : MonoBehaviour
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060008E5 RID: 2277 RVA: 0x00087174 File Offset: 0x00085374
	public static SaveStateController Instance
	{
		get
		{
			return SaveStateController._instance;
		}
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0008717B File Offset: 0x0008537B
	private void Awake()
	{
		if (SaveStateController._instance != null && SaveStateController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SaveStateController._instance = this;
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x000871AC File Offset: 0x000853AC
	public Task CaptureSaveStateAsync(string path)
	{
		SaveStateController.<CaptureSaveStateAsync>d__4 <CaptureSaveStateAsync>d__;
		<CaptureSaveStateAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CaptureSaveStateAsync>d__.path = path;
		<CaptureSaveStateAsync>d__.<>1__state = -1;
		<CaptureSaveStateAsync>d__.<>t__builder.Start<SaveStateController.<CaptureSaveStateAsync>d__4>(ref <CaptureSaveStateAsync>d__);
		return <CaptureSaveStateAsync>d__.<>t__builder.Task;
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x000871F0 File Offset: 0x000853F0
	public void PreLoadCases(ref StateSaveData load)
	{
		foreach (SideJobAffair sideJobAffair in load.affairJobs)
		{
			if (!SideJobController.Instance.allJobsDictionary.ContainsKey(sideJobAffair.jobID))
			{
				SideJobController.Instance.allJobsDictionary.Add(sideJobAffair.jobID, sideJobAffair);
			}
			else
			{
				SideJobController.Instance.allJobsDictionary[sideJobAffair.jobID] = sideJobAffair;
			}
			SideJob.assignJobID = Mathf.Max(sideJobAffair.jobID + 1, SideJob.assignJobID);
		}
		foreach (SideJobSabotage sideJobSabotage in load.sabotageJobs)
		{
			if (!SideJobController.Instance.allJobsDictionary.ContainsKey(sideJobSabotage.jobID))
			{
				SideJobController.Instance.allJobsDictionary.Add(sideJobSabotage.jobID, sideJobSabotage);
			}
			else
			{
				SideJobController.Instance.allJobsDictionary[sideJobSabotage.jobID] = sideJobSabotage;
			}
			SideJob.assignJobID = Mathf.Max(sideJobSabotage.jobID + 1, SideJob.assignJobID);
		}
		foreach (SideJobStolenItem sideJobStolenItem in load.stolenItemJobs)
		{
			if (!SideJobController.Instance.allJobsDictionary.ContainsKey(sideJobStolenItem.jobID))
			{
				SideJobController.Instance.allJobsDictionary.Add(sideJobStolenItem.jobID, sideJobStolenItem);
			}
			else
			{
				SideJobController.Instance.allJobsDictionary[sideJobStolenItem.jobID] = sideJobStolenItem;
			}
			SideJob.assignJobID = Mathf.Max(sideJobStolenItem.jobID + 1, SideJob.assignJobID);
		}
		foreach (SideJobMissingPerson sideJobMissingPerson in load.missingPersonJobs)
		{
			if (!SideJobController.Instance.allJobsDictionary.ContainsKey(sideJobMissingPerson.jobID))
			{
				SideJobController.Instance.allJobsDictionary.Add(sideJobMissingPerson.jobID, sideJobMissingPerson);
			}
			else
			{
				SideJobController.Instance.allJobsDictionary[sideJobMissingPerson.jobID] = sideJobMissingPerson;
			}
			SideJob.assignJobID = Mathf.Max(sideJobMissingPerson.jobID + 1, SideJob.assignJobID);
		}
		foreach (SideJobRevenge sideJobRevenge in load.revengeJobs)
		{
			if (!SideJobController.Instance.allJobsDictionary.ContainsKey(sideJobRevenge.jobID))
			{
				SideJobController.Instance.allJobsDictionary.Add(sideJobRevenge.jobID, sideJobRevenge);
			}
			else
			{
				SideJobController.Instance.allJobsDictionary[sideJobRevenge.jobID] = sideJobRevenge;
			}
			SideJob.assignJobID = Mathf.Max(sideJobRevenge.jobID + 1, SideJob.assignJobID);
		}
		foreach (SideJobStealBriefcase sideJobStealBriefcase in load.briefcaseJobs)
		{
			if (!SideJobController.Instance.allJobsDictionary.ContainsKey(sideJobStealBriefcase.jobID))
			{
				SideJobController.Instance.allJobsDictionary.Add(sideJobStealBriefcase.jobID, sideJobStealBriefcase);
			}
			else
			{
				SideJobController.Instance.allJobsDictionary[sideJobStealBriefcase.jobID] = sideJobStealBriefcase;
			}
			SideJob.assignJobID = Mathf.Max(sideJobStealBriefcase.jobID + 1, SideJob.assignJobID);
		}
		foreach (SideJob sideJob in load.basicJobs)
		{
			if (!SideJobController.Instance.allJobsDictionary.ContainsKey(sideJob.jobID))
			{
				SideJobController.Instance.allJobsDictionary.Add(sideJob.jobID, sideJob);
			}
			else
			{
				SideJobController.Instance.allJobsDictionary[sideJob.jobID] = sideJob;
			}
			SideJob.assignJobID = Mathf.Max(sideJob.jobID + 1, SideJob.assignJobID);
		}
		foreach (MurderController.Murder murder in load.murders)
		{
			if (!MurderController.Instance.activeMurders.Contains(murder))
			{
				MurderController.Instance.activeMurders.Add(murder);
			}
		}
		foreach (MurderController.Murder murder2 in load.iaMurders)
		{
			if (!MurderController.Instance.inactiveMurders.Contains(murder2))
			{
				MurderController.Instance.inactiveMurders.Add(murder2);
			}
		}
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00087718 File Offset: 0x00085918
	public void LoadSaveState(StateSaveData load)
	{
		SessionData.Instance.SetGameTime(load.gameTime, load.leapCycle);
		SessionData.Instance.gameTimeLimit = load.timeLimit;
		SessionData.Instance.weatherChangeTimer = load.weatherChange;
		SessionData.Instance.SetWeather(load.desiredRain, load.desiredWind, load.desiredSnow, load.desiredLightning, load.desiredFog, 1f, false);
		SessionData.Instance.currentRain = load.currentRain;
		SessionData.Instance.currentWind = load.currentWind;
		SessionData.Instance.currentSnow = load.currentSnow;
		SessionData.Instance.currentFog = load.currentFog;
		SessionData.Instance.currentLightning = load.currentLightning;
		SessionData.Instance.cityWetness = load.cityWetness;
		SessionData.Instance.citySnow = load.citySnow;
		SessionData.Instance.ExecuteWeatherChange();
		SessionData.Instance.ExecuteWetnessChange();
		SessionData.Instance.ExecuteWindChange();
		GameplayController.Instance.printsLetterLoop = load.fingerprintLoop;
		SceneRecorder.assignCapID = load.assignCaptureID;
		GameplayController.Instance.assignMessageThreadID = load.assignMessageThreadID;
		GroupsController.assignID = load.assignGroupID;
		InterfaceController.assignStickyNoteID = load.assignStickNote;
		Interactable.worldAssignID = load.assignInteractableID;
		Case.assignCaseID = load.assignCaseID;
		MurderController.Instance.assignMurderID = load.assignMurderID;
		Game.Instance.SetGameLength(load.gameLength, true, true, false);
		SessionData.Instance.SetDisplayTutorialText(load.tutorial);
		if (load.tutorial)
		{
			SessionData.Instance.tutorialTextTriggered = new HashSet<string>(load.tutTextTriggered);
		}
		SessionData.Instance.UpdateTutorialNotifications();
		foreach (string evidenceID in load.timeEvidence)
		{
			EvidenceCreator.Instance.GetTimeEvidence(evidenceID);
		}
		foreach (string date in load.dateEvidence)
		{
			EvidenceCreator.Instance.GetDateEvidence(date, "date", "", -1, -1, -1);
		}
		GameplayController.Instance.SetJobDifficultyLevel(load.jobDiffLevel);
		if (load.residence > -1)
		{
			NewAddress newAddress = null;
			if (CityData.Instance.addressDictionary.TryGetValue(load.residence, ref newAddress))
			{
				if (newAddress != null)
				{
					if (newAddress.residence != null)
					{
						Game.Log("CityGen: Loading player's residence from save data...", 2);
						Player.Instance.SetResidence(newAddress.residence, true);
					}
					if (!Player.Instance.apartmentsOwned.Contains(newAddress))
					{
						Player.Instance.apartmentsOwned.Add(newAddress);
					}
				}
			}
			else
			{
				Game.Log("CityGen: Setting player as having no apartment...", 2);
				Player.Instance.SetResidence(null, true);
			}
		}
		else
		{
			Game.Log("CityGen: Setting player as having no apartment...", 2);
			Player.Instance.SetResidence(null, true);
		}
		if (load.apartmentsOwned != null)
		{
			foreach (int num in load.apartmentsOwned)
			{
				NewAddress newAddress2 = null;
				if (CityData.Instance.addressDictionary.TryGetValue(num, ref newAddress2))
				{
					if (newAddress2 != null && !Player.Instance.apartmentsOwned.Contains(newAddress2))
					{
						Player.Instance.apartmentsOwned.Add(newAddress2);
					}
					Player.Instance.AddLocationOfAuthorty(newAddress2);
				}
			}
		}
		Player.Instance.claimedAccidentCover = load.accidentCover;
		Player.Instance.foodHygeinePhotos = new List<int>(load.foodH);
		Player.Instance.sanitaryHygeinePhotos = new List<int>(load.sanitary);
		Player.Instance.illegalOpsPhotos = new List<int>(load.ops);
		GameplayController.Instance.playerKnowsPasswords = new List<int>(load.knowsPasswords);
		GameplayController.Instance.debt = new List<GameplayController.LoanDebt>(load.debt);
		if (load.playerSavedCaptures.Count > 0)
		{
			Player.Instance.sceneRecorder.interactable.sCap = new List<SceneRecorder.SceneCapture>(load.playerSavedCaptures);
			foreach (SceneRecorder.SceneCapture sceneCapture in load.playerSavedCaptures)
			{
				sceneCapture.recorder = Player.Instance.sceneRecorder;
			}
			Game.Log("Loaded " + load.playerSavedCaptures.Count.ToString() + " player captures...", 2);
		}
		Game.Log("CityGen: Loading " + load.messageThreads.Count.ToString() + " vmail threads...", 2);
		foreach (StateSaveData.MessageThreadSave messageThreadSave in load.messageThreads)
		{
			Human human = null;
			if (CityData.Instance.GetHuman(messageThreadSave.participantA, out human, true))
			{
				if (!GameplayController.Instance.messageThreads.ContainsKey(messageThreadSave.threadID))
				{
					GameplayController.Instance.messageThreads.Add(messageThreadSave.threadID, messageThreadSave);
				}
				else
				{
					GameplayController.Instance.messageThreads[messageThreadSave.threadID] = messageThreadSave;
				}
				if (!human.messageThreadsStarted.Contains(messageThreadSave))
				{
					human.messageThreadsStarted.Add(messageThreadSave);
				}
				if (!human.messageThreadFeatures.Contains(messageThreadSave))
				{
					human.messageThreadFeatures.Add(messageThreadSave);
				}
				if (messageThreadSave.participantB > -1)
				{
					Human human2 = null;
					if (CityData.Instance.GetHuman(messageThreadSave.participantB, out human2, true))
					{
						if (!human2.messageThreadFeatures.Contains(messageThreadSave))
						{
							human2.messageThreadFeatures.Add(messageThreadSave);
						}
					}
					else
					{
						Game.LogError("CityGen: Cannot find participant B " + messageThreadSave.participantA.ToString() + " for vmail", 2);
					}
				}
				if (messageThreadSave.participantC > -1)
				{
					Human human3 = null;
					if (CityData.Instance.GetHuman(messageThreadSave.participantC, out human3, true))
					{
						if (!human3.messageThreadFeatures.Contains(messageThreadSave))
						{
							human3.messageThreadFeatures.Add(messageThreadSave);
						}
					}
					else
					{
						Game.LogError("CityGen: Cannot find participant C " + messageThreadSave.participantA.ToString() + " for vmail", 2);
					}
				}
				if (messageThreadSave.participantD > -1)
				{
					Human human4 = null;
					if (CityData.Instance.GetHuman(messageThreadSave.participantD, out human4, true))
					{
						if (!human4.messageThreadFeatures.Contains(messageThreadSave))
						{
							human4.messageThreadFeatures.Add(messageThreadSave);
						}
					}
					else
					{
						Game.LogError("CityGen: Cannot find participant D " + messageThreadSave.participantA.ToString() + " for vmail", 2);
					}
				}
				using (List<int>.Enumerator enumerator2 = messageThreadSave.cc.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						int id = enumerator2.Current;
						Human human5 = null;
						if (CityData.Instance.GetHuman(id, out human5, true) && !human5.messageThreadCCd.Contains(messageThreadSave))
						{
							human5.messageThreadCCd.Add(messageThreadSave);
						}
					}
					continue;
				}
			}
			Game.LogError("CityGen: Cannot find participant A " + messageThreadSave.participantA.ToString() + " for vmail", 2);
		}
		if (load.currentMurderer > -1)
		{
			CityData.Instance.GetHuman(load.currentMurderer, out MurderController.Instance.currentMurderer, true);
		}
		if (load.currentVictim > -1)
		{
			CityData.Instance.GetHuman(load.currentVictim, out MurderController.Instance.currentVictim, true);
		}
		if (load.murderPreset != null && load.murderPreset.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MurderPreset>(load.murderPreset, out MurderController.Instance.murderPreset);
		}
		if (load.chosenMO != null && load.chosenMO.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<MurderMO>(load.chosenMO, out MurderController.Instance.chosenMO);
		}
		MurderController.Instance.triggerCoverUpCall = load.triggerCoverUpCall;
		MurderController.Instance.playerAcceptedCoverUp = load.playerAcceptedCoverUp;
		MurderController.Instance.triggerCoverUpSuccess = load.triggerCoverUpSuccess;
		if (load.currentVictimSite > -1)
		{
			if (load.victimSiteIsStreet)
			{
				MurderController.Instance.currentVictimSite = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == load.currentVictimSite);
			}
			else
			{
				NewAddress currentVictimSite = null;
				if (CityData.Instance.addressDictionary.TryGetValue(load.currentVictimSite, ref currentVictimSite))
				{
					MurderController.Instance.currentVictimSite = currentVictimSite;
				}
			}
		}
		MurderController.Instance.pauseBetweenMurders = load.pauseBetweenMurders;
		MurderController.Instance.pauseBeforeKidnapperKill = load.pauseForKidnapperKill;
		MurderController.Instance.murderRoutineActive = load.murderRoutineActive;
		MurderController.Instance.maxDifficultyLevel = load.maxMurderDiffLevel;
		MurderController.Instance.previousMurderers = new List<Human>();
		foreach (int id2 in load.previousMurderers)
		{
			Human human6 = null;
			if (CityData.Instance.GetHuman(id2, out human6, true))
			{
				MurderController.Instance.previousMurderers.Add(human6);
			}
		}
		foreach (MurderController.Murder murder in MurderController.Instance.activeMurders)
		{
			murder.LoadSerializedData();
		}
		foreach (MurderController.Murder murder2 in MurderController.Instance.inactiveMurders)
		{
			murder2.LoadSerializedData();
		}
		foreach (MetaObject metaObject in load.metas)
		{
			if (!CityData.Instance.metaObjectDictionary.ContainsKey(metaObject.id))
			{
				CityData.Instance.metaObjectDictionary.Add(metaObject.id, metaObject);
			}
		}
		foreach (Interactable interactable in load.interactables)
		{
			interactable.wasLoadedFromSave = true;
			interactable.MainSetupStart();
			interactable.OnLoad();
		}
		Game.Log("Jobs: Loading " + load.affairJobs.Count.ToString() + " affair jobs...", 2);
		foreach (SideJobAffair job in load.affairJobs)
		{
			this.LoadJob(job);
		}
		Game.Log("Jobs: Loading " + load.sabotageJobs.Count.ToString() + " sabotage jobs...", 2);
		foreach (SideJobSabotage job2 in load.sabotageJobs)
		{
			this.LoadJob(job2);
		}
		Game.Log("Jobs: Loading " + load.stolenItemJobs.Count.ToString() + " stolen item jobs...", 2);
		foreach (SideJobStolenItem job3 in load.stolenItemJobs)
		{
			this.LoadJob(job3);
		}
		Game.Log("Jobs: Loading " + load.missingPersonJobs.Count.ToString() + " missing person jobs...", 2);
		foreach (SideJobMissingPerson job4 in load.missingPersonJobs)
		{
			this.LoadJob(job4);
		}
		Game.Log("Jobs: Loading " + load.revengeJobs.Count.ToString() + " revenge jobs...", 2);
		foreach (SideJobRevenge job5 in load.revengeJobs)
		{
			this.LoadJob(job5);
		}
		Game.Log("Jobs: Loading " + load.briefcaseJobs.Count.ToString() + " briefcase jobs...", 2);
		foreach (SideJobStealBriefcase job6 in load.briefcaseJobs)
		{
			this.LoadJob(job6);
		}
		Game.Log("Jobs: Loading " + load.basicJobs.Count.ToString() + " basic jobs...", 2);
		foreach (SideJob job7 in load.basicJobs)
		{
			this.LoadJob(job7);
		}
		SideJobController.Instance.JobCreationCheck();
		CasePanelController.Instance.activeCases = new List<Case>(load.activeCases);
		CasePanelController.Instance.archivedCases = new List<Case>(load.archivedCases);
		foreach (Case @case in CasePanelController.Instance.activeCases)
		{
			if (@case.caseType == Case.CaseType.sideJob && @case.jobReference > -1 && SideJobController.Instance.allJobsDictionary.TryGetValue(@case.jobReference, ref @case.job))
			{
				@case.job.thisCase = @case;
				Game.Log("Jobs: Loading objectives for job " + @case.job.jobID.ToString(), 2);
			}
			foreach (Objective objective in new List<Objective>(@case.endedObjectives))
			{
				objective.Setup(@case);
			}
			foreach (Objective objective2 in @case.inactiveCurrentObjectives)
			{
				objective2.Setup(@case);
			}
			foreach (Objective objective3 in @case.currentActiveObjectives)
			{
				objective3.Setup(@case);
			}
			if (@case.id == load.currentActiveCase)
			{
				MurderController.Instance.currentActiveCase = @case;
			}
		}
		foreach (Case case2 in CasePanelController.Instance.archivedCases)
		{
			if (case2.caseType == Case.CaseType.sideJob && case2.jobReference > -1 && SideJobController.Instance.allJobsDictionary.TryGetValue(case2.jobReference, ref case2.job))
			{
				case2.job.thisCase = case2;
			}
			if (case2.id == load.currentActiveCase)
			{
				MurderController.Instance.currentActiveCase = case2;
			}
		}
		MurderController.Instance.SetProcGenKillerLoop(load.pgLoop);
		foreach (GameplayController.Footprint footprint in load.footprints)
		{
			NewRoom newRoom = null;
			if (CityData.Instance.roomDictionary.TryGetValue(footprint.rID, ref newRoom))
			{
				if (!GameplayController.Instance.activeFootprints.ContainsKey(newRoom))
				{
					GameplayController.Instance.activeFootprints.Add(newRoom, new List<GameplayController.Footprint>());
				}
				GameplayController.Instance.activeFootprints[newRoom].Add(footprint);
				GameplayController.Instance.footprintsList.Add(footprint);
			}
		}
		GameplayController.Instance.history = new List<GameplayController.History>(load.history);
		GameplayController.Instance.acquiredPasscodes = new List<GameplayController.Passcode>();
		using (List<GameplayController.Passcode>.Enumerator enumerator18 = load.passcodes.GetEnumerator())
		{
			while (enumerator18.MoveNext())
			{
				GameplayController.Passcode code = enumerator18.Current;
				if (code.type == GameplayController.PasscodeType.citizen)
				{
					Human human7 = null;
					if (CityData.Instance.GetHuman(code.id, out human7, true))
					{
						GameplayController.Instance.acquiredPasscodes.Add(human7.passcode);
					}
				}
				else if (code.type == GameplayController.PasscodeType.address)
				{
					NewAddress newAddress3 = null;
					if (CityData.Instance.addressDictionary.TryGetValue(code.id, ref newAddress3))
					{
						GameplayController.Instance.acquiredPasscodes.Add(newAddress3.passcode);
					}
				}
				else if (code.type == GameplayController.PasscodeType.room)
				{
					NewRoom newRoom2 = null;
					if (CityData.Instance.roomDictionary.TryGetValue(code.id, ref newRoom2))
					{
						GameplayController.Instance.acquiredPasscodes.Add(newRoom2.passcode);
					}
				}
				else if (code.type == GameplayController.PasscodeType.interactable)
				{
					Interactable interactable2 = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == code.id);
					if (interactable2 != null)
					{
						GameplayController.Instance.acquiredPasscodes.Add(interactable2.passcode);
					}
				}
			}
		}
		GameplayController.Instance.acquiredNumbers = new List<GameplayController.PhoneNumber>(load.numbers);
		for (int i = 0; i < GameplayController.Instance.history.Count; i++)
		{
			GameplayController.History history = GameplayController.Instance.history[i];
			Evidence evidence = null;
			if (GameplayController.Instance.evidenceDictionary.TryGetValue(history.evID, ref evidence) && evidence.interactable != null && evidence.interactable.preset.spawnable)
			{
				GameplayController.Instance.itemOnlyHistory.Add(history);
			}
		}
		using (List<GameplayController.EnforcerCall>.Enumerator enumerator19 = load.enforcerCalls.GetEnumerator())
		{
			while (enumerator19.MoveNext())
			{
				GameplayController.EnforcerCall call = enumerator19.Current;
				NewGameLocation newGameLocation;
				if (call.isStreet)
				{
					newGameLocation = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == call.id);
				}
				else
				{
					newGameLocation = CityData.Instance.addressDictionary[call.id];
				}
				GameplayController.Instance.enforcerCalls.Add(newGameLocation, call);
				newGameLocation.SetAsCrimeScene(call.isCrimeScene);
				if (call.isCrimeScene)
				{
					newGameLocation.loggedAsCrimeScene = call.logTime;
				}
			}
		}
		Player.Instance.isCrouched = load.crouched;
		Player.Instance.OnCrouchedChange();
		Player.Instance.SetPosition(load.playerPos, load.playerRot);
		GameplayController.Instance.SetMoney(load.money);
		GameplayController.Instance.SetLockpicks(load.lockpicks);
		GameplayController.Instance.socialCreditPerks = new List<SocialControls.SocialCreditBuff>();
		for (int j = 0; j < load.socCreditPerks.Count; j++)
		{
			string soc = load.socCreditPerks[j];
			SocialControls.SocialCreditBuff socialCreditBuff = SocialControls.Instance.socialCreditBuffs.Find((SocialControls.SocialCreditBuff item) => item.name == soc);
			if (socialCreditBuff != null)
			{
				GameplayController.Instance.socialCreditPerks.Add(socialCreditBuff);
			}
			else
			{
				Game.LogError("Cannot find social credit perk: " + soc, 2);
			}
		}
		GameplayController.Instance.SetSocialCredit(load.socCredit);
		Player.Instance.SetHealth(load.health);
		Player.Instance.nourishment = load.nourishment;
		Player.Instance.hydration = load.hydration;
		Player.Instance.alertness = load.alertness;
		Player.Instance.energy = load.energy;
		Player.Instance.hygiene = load.hygiene;
		Player.Instance.heat = load.heat;
		Player.Instance.drunk = load.drunk;
		Player.Instance.sick = load.sick;
		Player.Instance.headache = load.headache;
		Player.Instance.wet = load.wet;
		Player.Instance.brokenLeg = load.brokenLeg;
		Player.Instance.bruised = load.bruised;
		Player.Instance.blackEye = load.blackEye;
		Player.Instance.blackedOut = load.blackedOut;
		Player.Instance.numb = load.numb;
		Player.Instance.poisoned = load.poisoned;
		Player.Instance.bleeding = load.bleeding;
		Player.Instance.wellRested = load.wellRested;
		Player.Instance.starchAddiction = load.starchAddiction;
		Player.Instance.syncDiskInstall = load.syncDiskInstall;
		Player.Instance.blinded = load.blinded;
		if (load.speech.Count > 0)
		{
			Player.Instance.speechController.speechQueue.AddRange(load.speech);
			foreach (SpeechController.QueueElement queueElement in Player.Instance.speechController.speechQueue)
			{
				foreach (Objective.ObjectiveTrigger objectiveTrigger in queueElement.triggers)
				{
					objectiveTrigger.SetupNonSerialized();
				}
			}
			Player.Instance.speechController.enabled = true;
		}
		UpgradesController.Instance.upgrades.Clear();
		GameplayController.Instance.companiesSabotaged = new List<string>(load.sabotaged);
		GameplayController.Instance.booksRead = new List<string>(load.booksRead);
		foreach (UpgradesController.Upgrades upgrades in load.upgrades)
		{
			UpgradesController.Instance.upgrades.Add(upgrades);
		}
		foreach (int num2 in load.keyring)
		{
			if (CityData.Instance.doorDictionary.ContainsKey(num2))
			{
				Player.Instance.AddToKeyring(CityData.Instance.doorDictionary[num2], false);
			}
		}
		using (List<int>.Enumerator enumerator2 = load.keyringInt.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				int d = enumerator2.Current;
				Interactable interactable3 = null;
				if (CityData.Instance.savableInteractableDictionary.TryGetValue(d, ref interactable3))
				{
					Player.Instance.AddToKeyring(interactable3, false);
				}
				else
				{
					interactable3 = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == d);
					if (interactable3 != null)
					{
						Player.Instance.AddToKeyring(interactable3, false);
					}
				}
			}
		}
		foreach (StateSaveData.FakeTelephone fakeTelephone in load.fakeTelephone)
		{
			TelephoneController.Instance.AddFakeNumber(fakeTelephone.number, fakeTelephone.source);
		}
		using (List<StateSaveData.BuildingStateSav>.Enumerator enumerator24 = load.buildings.GetEnumerator())
		{
			while (enumerator24.MoveNext())
			{
				StateSaveData.BuildingStateSav b = enumerator24.Current;
				NewBuilding newBuilding = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Find((NewBuilding item) => item.buildingID == b.id);
				if (newBuilding != null)
				{
					newBuilding.SetAlarm(b.alarmActive, null, null);
					newBuilding.alarmTimer = b.alarmTimer;
					newBuilding.targetMode = b.targetMode;
					newBuilding.wantedInBuilding = b.wanted;
					foreach (StateSaveData.ElevatorStateSave elevatorStateSave in b.elevators)
					{
						foreach (KeyValuePair<NewTile, Elevator> keyValuePair in newBuilding.stairwells)
						{
							if (keyValuePair.Key.tileID == elevatorStateSave.tileID)
							{
								keyValuePair.Value.LoadElevatorSaveData(elevatorStateSave);
							}
						}
					}
					foreach (int id3 in b.targets)
					{
						Human human8 = null;
						if (CityData.Instance.GetHuman(id3, out human8, true))
						{
							newBuilding.alarmTargets.Add(human8);
						}
					}
					newBuilding.callLog = new List<TelephoneController.PhoneCall>(b.callLog);
					foreach (TelephoneController.PhoneCall phoneCall in newBuilding.callLog)
					{
						phoneCall.SetupNonSerializedData();
					}
					newBuilding.lostAndFound = new List<GameplayController.LostAndFound>(b.lostAndFound);
				}
				foreach (StateSaveData.FloorStateSave floorStateSave in load.floors)
				{
					foreach (KeyValuePair<int, NewFloor> keyValuePair2 in newBuilding.floors)
					{
						if (keyValuePair2.Value.floorID == floorStateSave.id)
						{
							keyValuePair2.Value.SetAlarmLockdown(floorStateSave.alarmLockdown, null);
						}
					}
				}
			}
		}
		foreach (StateSaveData.AddressStateSave addressStateSave in load.addresses)
		{
			NewAddress newAddress4 = null;
			if (CityData.Instance.addressDictionary.TryGetValue(addressStateSave.id, ref newAddress4))
			{
				newAddress4.SetAlarm(addressStateSave.alarmActive, null);
				newAddress4.alarmTimer = addressStateSave.alarmTimer;
				newAddress4.targetMode = addressStateSave.targetMode;
				newAddress4.targetModeSetAt = addressStateSave.targetModeSetAt;
				newAddress4.playerLoiteringTimer = addressStateSave.loiter;
				if (addressStateSave.targetMode != NewBuilding.AlarmTargetMode.illegalActivities && !GameplayController.Instance.alteredSecurityTargetsLocations.Contains(newAddress4))
				{
					GameplayController.Instance.alteredSecurityTargetsLocations.Add(newAddress4);
				}
				if (addressStateSave.sale > -1)
				{
					CityData.Instance.savableInteractableDictionary.TryGetValue(addressStateSave.sale, ref newAddress4.saleNote);
				}
				newAddress4.vandalism = new List<NewAddress.Vandalism>(addressStateSave.vandalism);
				foreach (int id4 in addressStateSave.targets)
				{
					Human human9 = null;
					if (CityData.Instance.GetHuman(id4, out human9, true))
					{
						newAddress4.alarmTargets.Add(human9);
					}
				}
				foreach (NewGameLocation.TrespassEscalation trespassEscalation in addressStateSave.escalation)
				{
					if (trespassEscalation.isPlayer)
					{
						newAddress4.escalation.Add(Player.Instance, trespassEscalation);
						if (!CitizenBehaviour.Instance.tempEscalationBoost.Contains(newAddress4))
						{
							CitizenBehaviour.Instance.tempEscalationBoost.Add(newAddress4);
						}
					}
					else
					{
						Human human10 = null;
						if (CityData.Instance.GetHuman(trespassEscalation.actor, out human10, true))
						{
							newAddress4.escalation.Add(human10, trespassEscalation);
							if (!CitizenBehaviour.Instance.tempEscalationBoost.Contains(newAddress4))
							{
								CitizenBehaviour.Instance.tempEscalationBoost.Add(newAddress4);
							}
						}
					}
				}
			}
		}
		foreach (StateSaveData.RoomStateSave roomStateSave in load.rooms)
		{
			NewRoom newRoom3 = null;
			if (CityData.Instance.roomDictionary.TryGetValue(roomStateSave.id, ref newRoom3))
			{
				newRoom3.SetExplorationLevel(roomStateSave.ex);
				newRoom3.SetMainLights(roomStateSave.ml, "load game", null, true, true);
				newRoom3.AddGas(roomStateSave.gl);
				newRoom3.furnitureAssignID = roomStateSave.fID;
				newRoom3.interactableAssignID = roomStateSave.iID;
				if (roomStateSave.decorOverride != null && roomStateSave.decorOverride.Count > 0)
				{
					newRoom3.decorEdit = true;
				}
			}
		}
		using (List<StateSaveData.CompanyStateSave>.Enumerator enumerator33 = load.companies.GetEnumerator())
		{
			while (enumerator33.MoveNext())
			{
				StateSaveData.CompanyStateSave c = enumerator33.Current;
				Company company = CityData.Instance.companyDirectory.Find((Company item) => item.companyID == c.id);
				if (company != null)
				{
					company.sales = new List<Company.SalesRecord>(c.sales);
					foreach (Company.SalesRecord salesRecord in company.sales)
					{
						salesRecord.SpawnFact();
					}
				}
			}
		}
		using (List<StateSaveData.CitizenStateSave>.Enumerator enumerator35 = load.citizens.GetEnumerator())
		{
			while (enumerator35.MoveNext())
			{
				StateSaveData.CitizenStateSave h = enumerator35.Current;
				Human human11 = null;
				if (CityData.Instance.GetHuman(h.id, out human11, false))
				{
					human11.transform.position = h.pos;
					human11.transform.rotation = h.rot;
					human11.UpdateGameLocation(0f);
					if (Game.Instance.devMode && human11.ai != null)
					{
						human11.ai.debugDestinationPosition.Add("Load in position " + human11.transform.position.ToString());
					}
					human11.trespassingEscalation = h.trespassingEscalation;
					human11.outfitController.SetCurrentOutfit(h.currentOutfit, false, false, true);
					human11.nourishment = h.nourishment;
					human11.hydration = h.hydration;
					human11.alertness = h.alertness;
					human11.energy = h.energy;
					human11.excitement = h.excitement;
					human11.chores = h.chores;
					human11.hygiene = h.hygiene;
					human11.bladder = h.bladder;
					human11.heat = h.heat;
					human11.drunk = h.drunk;
					human11.breath = h.breath;
					human11.poisoned = h.poisoned;
					human11.blinded = h.blinded;
					if (h.poisoner > -1)
					{
						CityData.Instance.GetHuman(h.poisoner, out human11.poisoner, true);
					}
					if (h.den > -1 && CityData.Instance.addressDictionary.TryGetValue(h.den, ref human11.den))
					{
						human11.SetDen(human11.den);
					}
					human11.currentWounds = new List<Human.Wound>(h.wounds);
					foreach (Human.Wound wound in human11.currentWounds)
					{
						wound.Load();
					}
					human11.fingerprintLoop = h.fingerprintLoop;
					human11.SetHealth(h.currentHealth);
					human11.SetNerve(h.currentNerve);
					human11.footstepBlood = h.fsBlood;
					human11.footstepDirt = h.fsDirt;
					if (h.currentConsumable != null && h.currentConsumable.Count > 0)
					{
						foreach (string name in h.currentConsumable)
						{
							InteractablePreset newPreset = null;
							Toolbox.Instance.LoadDataFromResources<InteractablePreset>(name, out newPreset);
							human11.AddCurrentConsumable(newPreset);
						}
					}
					human11.trash = new List<int>(h.trash);
					human11.unreportable = h.unreportable;
					human11.death = h.death;
					if (h.death != null && h.death.isDead)
					{
						Human killer = null;
						CityData.Instance.GetHuman(h.death.killer, out killer, true);
						human11.death.isDead = true;
						human11.ai.SetKO(true, default(Vector3), default(Vector3), false, 0f, true, 1f);
						human11.Murder(killer, false, null, null, 1f);
						if (human11.death != null)
						{
							MurderController.Murder murder3 = human11.death.GetMurder();
							if (murder3 != null)
							{
								murder3.death = human11.death;
							}
						}
						human11.ai.deadRagdollTimer = 0f;
						if (human11.animationController.newBoxCollider != null)
						{
							Object.Destroy(human11.animationController.newBoxCollider);
						}
						if (h.ragdollSnapshotWorld != null)
						{
							human11.animationController.LoadLimbSnapshot(h.ragdollSnapshotWorld);
						}
						else if (h.ragdollSnapshot != null)
						{
							human11.animationController.LoadLimbSnapshot(h.ragdollSnapshot);
						}
						if (!CityData.Instance.deadCitizensDirectory.Contains(human11))
						{
							CityData.Instance.deadCitizensDirectory.Add(human11);
						}
					}
					if (human11.ai != null)
					{
						human11.ai.lastUpdated = SessionData.Instance.gameTime;
						human11.ai.isConvicted = h.convicted;
						if (h.putDown != null)
						{
							foreach (int num3 in h.putDown)
							{
								Interactable interactable4 = null;
								if (CityData.Instance.savableInteractableDictionary.TryGetValue(num3, ref interactable4))
								{
									human11.ai.putDownItems.Add(interactable4);
								}
							}
						}
						if (h.kidnapper > -1)
						{
							CityData.Instance.citizenDictionary.TryGetValue(h.kidnapper, ref human11.ai.kidnapper);
						}
						if (h.reactionState != NewAIController.ReactionState.none)
						{
							NewNode newInvestigateNode = null;
							Human newTarget = null;
							if (h.persuit)
							{
								if (h.persuitPlayer)
								{
									newTarget = Player.Instance;
								}
								else
								{
									Human human12 = null;
									CityData.Instance.GetHuman(h.persuitTarget, out human12, true);
									if (human12 != null)
									{
										newTarget = human12;
									}
								}
								human11.ai.SetPersue(newTarget, false, h.escalationLevel, true, 10f);
								human11.ai.seesOnPersuit = h.seesPlayerOnPersuit;
								human11.ai.persuitChaseLogicUses = h.persuitChaseLogicUses;
							}
							if (PathFinder.Instance.nodeMap.TryGetValue(h.investigateLocation, ref newInvestigateNode))
							{
								human11.ai.Investigate(newInvestigateNode, h.investigatePosition, newTarget, h.reactionState, h.minimumInvestigationTimeMultiplier, h.escalationLevel, false, 1f, null);
							}
							human11.ai.investigatePositionProjection = h.investigatePositionProjection;
							human11.ai.lastInvestigate = h.lastInvestigate;
						}
						if (h.currentGoal != null)
						{
							NewAIGoal newAIGoal = human11.ai.goals.Find((NewAIGoal item) => item.preset.name == h.currentGoal.preset && !item.preset.disableSave);
							if (newAIGoal == null)
							{
								AIGoalPreset aigoalPreset = Toolbox.Instance.allGoals.Find((AIGoalPreset item) => item.name == h.currentGoal.preset);
								if (aigoalPreset != null && !aigoalPreset.disableSave)
								{
									NewNode newPassedNode = null;
									PathFinder.Instance.nodeMap.TryGetValue(h.currentGoal.passedNode, ref newPassedNode);
									Interactable newPassedInteractable = null;
									if (h.currentGoal.passedInteractable > -1 && !CityData.Instance.savableInteractableDictionary.TryGetValue(h.currentGoal.passedInteractable, ref newPassedInteractable))
									{
										newPassedInteractable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == h.currentGoal.passedInteractable);
									}
									NewGameLocation newPassedGameLocation = null;
									if (h.currentGoal.gameLocation > -1)
									{
										if (h.currentGoal.isAddress)
										{
											newPassedGameLocation = CityData.Instance.addressDirectory.Find((NewAddress item) => item.id == h.currentGoal.gameLocation);
										}
										else
										{
											newPassedGameLocation = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == h.currentGoal.gameLocation);
										}
									}
									GroupsController.SocialGroup newPassedGroup = null;
									if (h.currentGoal.passedGroup > -1)
									{
										newPassedGroup = GroupsController.Instance.groups.Find((GroupsController.SocialGroup item) => item.id == h.currentGoal.passedGroup);
									}
									newAIGoal = human11.ai.CreateNewGoal(aigoalPreset, h.currentGoal.trigerTime, h.currentGoal.duration, newPassedNode, newPassedInteractable, newPassedGameLocation, newPassedGroup, null, h.currentGoal.var);
									if (h.currentGoal.room > -1)
									{
										CityData.Instance.roomDictionary.TryGetValue(h.currentGoal.room, ref newAIGoal.roomLocation);
									}
								}
							}
							if (newAIGoal != null)
							{
								human11.ai.currentGoal = newAIGoal;
								newAIGoal.triggerTime = h.currentGoal.trigerTime;
								newAIGoal.duration = h.currentGoal.duration;
								PathFinder.Instance.nodeMap.TryGetValue(h.currentGoal.passedNode, ref newAIGoal.passedNode);
								CityData.Instance.savableInteractableDictionary.TryGetValue(h.currentGoal.passedInteractable, ref newAIGoal.passedInteractable);
								newAIGoal.priority = h.currentGoal.priority;
								newAIGoal.passedVar = h.currentGoal.var;
								newAIGoal.isActive = true;
								newAIGoal.activeTime = h.currentGoal.activeTime;
								if (h.currentGoal.gameLocation > -1)
								{
									if (h.currentGoal.isAddress)
									{
										newAIGoal.gameLocation = CityData.Instance.addressDirectory.Find((NewAddress item) => item.id == h.currentGoal.gameLocation);
									}
									else
									{
										newAIGoal.gameLocation = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == h.currentGoal.gameLocation);
									}
								}
								if (h.currentGoal.passedGroup > -1)
								{
									newAIGoal.passedGroup = GroupsController.Instance.groups.Find((GroupsController.SocialGroup item) => item.id == h.currentGoal.passedGroup);
								}
								newAIGoal.jobID = h.currentGoal.jobID;
								using (List<StateSaveData.AIActionStateSave>.Enumerator enumerator37 = h.currentGoal.actions.GetEnumerator())
								{
									while (enumerator37.MoveNext())
									{
										StateSaveData.AIActionStateSave act = enumerator37.Current;
										Interactable interactable5 = null;
										CityData.Instance.savableInteractableDictionary.TryGetValue(act.passedInteractable, ref interactable5);
										NewRoom newRoom4 = null;
										if (act.passedRoom > -1)
										{
											CityData.Instance.roomDictionary.TryGetValue(act.passedRoom, ref newRoom4);
										}
										NewNode newNode = null;
										Vector3Int forcedNode = act.forcedNode;
										PathFinder.Instance.nodeMap.TryGetValue(act.forcedNode, ref newNode);
										AIActionPreset aiactionPreset = null;
										Toolbox.Instance.LoadDataFromResources<AIActionPreset>(act.preset, out aiactionPreset);
										GroupsController.SocialGroup socialGroup = null;
										if (act.passedGroup > -1)
										{
											socialGroup = GroupsController.Instance.groups.Find((GroupsController.SocialGroup item) => item.id == act.passedGroup);
										}
										NewAIGoal newGoal = newAIGoal;
										AIActionPreset newPreset2 = aiactionPreset;
										Interactable newPassedInteractable2 = interactable5;
										NewRoom newPassedRoom = newRoom4;
										NewNode newForcedNode = newNode;
										GroupsController.SocialGroup newPassedGroup2 = socialGroup;
										NewAIAction newAIAction = new NewAIAction(newGoal, newPreset2, act.inserted, newPassedRoom, newPassedInteractable2, newForcedNode, newPassedGroup2, null, false, act.iap, "");
										newAIAction.repeat = act.repeat;
										Interactable interactable6 = null;
										CityData.Instance.savableInteractableDictionary.TryGetValue(act.interactable, ref interactable6);
										newAIAction.interactable = interactable6;
									}
								}
							}
						}
						human11.walletItems = new List<Human.WalletItem>(h.wallet);
						if (h.ko)
						{
							NewAIController ai = human11.ai;
							bool ko = h.ko;
							bool ko2 = h.ko;
							float forcedDuration = h.koTime - SessionData.Instance.gameTime;
							ai.SetKO(ko, default(Vector3), default(Vector3), ko2, forcedDuration, true, 1f);
						}
						human11.ai.spooked = h.spooked;
						human11.ai.spookCounter = h.spookCount;
						if (h.res)
						{
							human11.ai.SetRestrained(h.res, h.resTime - SessionData.Instance.gameTime);
						}
						if (h.remFromWorld)
						{
							human11.RemoveFromWorld(true);
						}
						if (h.sightingCit != null)
						{
							for (int k = 0; k < h.sightingCit.Count; k++)
							{
								Human human13 = null;
								if (CityData.Instance.GetHuman(h.sightingCit[k], out human13, true))
								{
									if (!human11.lastSightings.ContainsKey(human13))
									{
										human11.lastSightings.Add(human13, h.sightings[k]);
									}
									else
									{
										human11.lastSightings[human13] = h.sightings[k];
									}
								}
							}
						}
						if (h.confine != null)
						{
							if (h.confine.st)
							{
								human11.ai.confineLocation = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == h.confine.id);
							}
							else
							{
								human11.ai.confineLocation = CityData.Instance.addressDirectory.Find((NewAddress item) => item.id == h.confine.id);
							}
						}
						if (h.avoid != null)
						{
							using (List<StateSaveData.AvoidConfineStateSave>.Enumerator enumerator38 = h.avoid.GetEnumerator())
							{
								while (enumerator38.MoveNext())
								{
									StateSaveData.AvoidConfineStateSave d = enumerator38.Current;
									if (d.st)
									{
										human11.ai.avoidLocations.Add(CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == d.id));
									}
									else
									{
										human11.ai.avoidLocations.Add(CityData.Instance.addressDirectory.Find((NewAddress item) => item.id == d.id));
									}
								}
							}
						}
						human11.ai.UpdateCurrentWeapon();
						if (human11.ai.isConvicted)
						{
							human11.RemoveFromWorld(true);
						}
					}
				}
			}
		}
		foreach (StateSaveData.DoorStateSave doorStateSave in load.doors)
		{
			NewDoor newDoor = null;
			if (CityData.Instance.doorDictionary.TryGetValue(doorStateSave.id, ref newDoor))
			{
				if (Game.Instance.collectDebugData)
				{
					newDoor.isLockedDebug.Add("Loaded save state for door ID " + doorStateSave.id.ToString() + ", locked: " + doorStateSave.l.ToString());
				}
				newDoor.wall.SetCurrentLockStrength(doorStateSave.ls);
				newDoor.wall.SetDoorStrength(doorStateSave.ds);
				newDoor.SetLocked(doorStateSave.l, null, false);
				newDoor.SetOpen(doorStateSave.ajar, null, true, 1f);
				if (doorStateSave.cs)
				{
					newDoor.SetForbidden(doorStateSave.cs);
				}
			}
		}
		foreach (StateSaveData.EvidenceStateSave evidenceStateSave in load.evidence)
		{
			Evidence evidence2 = null;
			if (Toolbox.Instance.TryGetEvidence(evidenceStateSave.id, out evidence2) && evidence2 != null)
			{
				if (evidenceStateSave.dds != null)
				{
					evidence2.SetOverrideDDS(evidenceStateSave.dds);
				}
				evidence2.SetFound(evidenceStateSave.found);
				evidence2.SetForceSave(evidenceStateSave.fs);
				if (evidenceStateSave.mpContent != null && evidenceStateSave.mpContent.Count > 0)
				{
					EvidenceMultiPage evidenceMultiPage = evidence2 as EvidenceMultiPage;
					if (evidenceMultiPage != null)
					{
						evidenceMultiPage.pageContent = new List<EvidenceMultiPage.MultiPageContent>(evidenceStateSave.mpContent);
					}
				}
			}
		}
		foreach (StateSaveData.EvidenceStateSave evidenceStateSave2 in load.evidence)
		{
			Evidence evidence3 = null;
			if (Toolbox.Instance.TryGetEvidence(evidenceStateSave2.id, out evidence3))
			{
				if (evidence3 != null)
				{
					if (evidenceStateSave2.dds != null)
					{
						evidence3.SetOverrideDDS(evidenceStateSave2.dds);
					}
					evidence3.SetFound(evidenceStateSave2.found);
					evidence3.SetForceSave(evidenceStateSave2.fs);
					evidence3.customNames = new List<Evidence.CustomName>(evidenceStateSave2.customName);
					if (evidence3.preset.useDataKeys)
					{
						foreach (StateSaveData.EvidenceDataKeyTie evidenceDataKeyTie in evidenceStateSave2.keyTies)
						{
							foreach (Evidence.DataKey keyTwo in evidenceDataKeyTie.tied)
							{
								evidence3.MergeDataKeys(evidenceDataKeyTie.key, keyTwo);
							}
						}
					}
					foreach (Evidence.Discovery disc in evidenceStateSave2.discovery)
					{
						evidence3.AddDiscovery(disc);
					}
					Evidence evidence4 = evidence3 as EvidenceStickyNote;
					if (evidence4 != null)
					{
						evidence4.SetNote(Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[1]), evidenceStateSave2.n);
					}
				}
			}
			else
			{
				Game.LogError("Cannot find evidence ID " + evidenceStateSave2.id, 2);
			}
		}
		if (load.activeCase > -1)
		{
			CasePanelController.Instance.SetActiveCase(CasePanelController.Instance.activeCases.Find((Case item) => item.id == load.activeCase));
		}
		using (List<StateSaveData.GuestPassStateSave>.Enumerator enumerator44 = load.guestPasses.GetEnumerator())
		{
			while (enumerator44.MoveNext())
			{
				StateSaveData.GuestPassStateSave saveState = enumerator44.Current;
				GameplayController.Instance.AddGuestPass(CityData.Instance.addressDirectory.Find((NewAddress item) => item.id == saveState.id), saveState.guestPassUntil);
			}
		}
		foreach (StateSaveData.BrokenWindowSave brokenWindowSave in load.brokenWindows)
		{
			GameplayController.Instance.brokenWindows.Add(brokenWindowSave.pos, brokenWindowSave.brokenAt);
		}
		using (List<StateSaveData.CrimeSceneCleanup>.Enumerator enumerator46 = load.crimeSceneCleanup.GetEnumerator())
		{
			while (enumerator46.MoveNext())
			{
				StateSaveData.CrimeSceneCleanup cs = enumerator46.Current;
				if (cs.isStreet)
				{
					StreetController streetController = CityData.Instance.streetDirectory.Find((StreetController item) => item.streetID == cs.id);
					if (streetController != null)
					{
						NewGameLocation newGameLocation2 = streetController;
						if (!GameplayController.Instance.crimeSceneCleanups.Contains(newGameLocation2))
						{
							GameplayController.Instance.crimeSceneCleanups.Add(newGameLocation2);
						}
					}
				}
				else
				{
					NewAddress newAddress5 = null;
					if (CityData.Instance.addressDictionary.TryGetValue(cs.id, ref newAddress5))
					{
						NewGameLocation newGameLocation3 = newAddress5;
						if (!GameplayController.Instance.crimeSceneCleanups.Contains(newGameLocation3))
						{
							GameplayController.Instance.crimeSceneCleanups.Add(newGameLocation3);
						}
					}
				}
			}
		}
		GameplayController.Instance.hotelGuests = new List<GameplayController.HotelGuest>(load.hotelGuests);
		foreach (GameplayController.HotelGuest hotelGuest in GameplayController.Instance.hotelGuests)
		{
			hotelGuest.FromLoadGame();
		}
		if (!Game.Instance.freeCam)
		{
			Player.Instance.SetPlayerHeight(Player.Instance.GetPlayerHeightNormal(), true);
			Player.Instance.SetCameraHeight(GameplayControls.Instance.cameraHeightNormal);
		}
		if (load.duct > -1)
		{
			Game.Log(string.Concat(new string[]
			{
				"Player: Loading player in vent: ",
				load.duct.ToString(),
				" (",
				CityData.Instance.airDuctGroupDirectory.Count.ToString(),
				")"
			}), 2);
			if (CityData.Instance.airDuctGroupDirectory.Find((AirDuctGroup item) => item.ductID == load.duct) != null)
			{
				Interactable interactable7 = CityData.Instance.interactableDirectory.Find((Interactable item) => item.preset.specialCaseFlag == InteractablePreset.SpecialCase.airVent);
				if (interactable7 != null)
				{
					Player.Instance.OnCrawlIntoVent(interactable7, true);
				}
				Player.Instance.SetPosition(load.playerPos, load.playerRot);
			}
		}
		if (load.mapPathActive)
		{
			NewNode loc = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(load.mapPathNode, ref loc))
			{
				MapController.Instance.PlotPlayerRoute(loc, load.mapPathNodeSpecific, null);
			}
		}
		if (load.chapter < 0)
		{
			Game.Instance.sandboxMode = true;
		}
		else
		{
			Game.Instance.sandboxMode = false;
			ChapterController.Instance.LoadChapter(ChapterController.Instance.allChapters[load.chapter], false);
			ChapterController.Instance.SkipToChapterPart(load.chapterPart, false, false);
		}
		foreach (Case case3 in CasePanelController.Instance.activeCases)
		{
			foreach (Case.CaseElement caseElement in case3.caseElements)
			{
				if (caseElement.id != null && caseElement.id.Length > 0)
				{
					Evidence evidence5 = null;
					DDSSaveClasses.DDSTreeSave ddstreeSave;
					if (Toolbox.Instance.TryGetEvidence(caseElement.id, out evidence5) && evidence5 != null && evidence5.overrideDDS != null && evidence5.overrideDDS.Length > 0 && Toolbox.Instance.allDDSTrees.TryGetValue(evidence5.overrideDDS, ref ddstreeSave))
					{
						foreach (DDSSaveClasses.DDSMessageSettings ddsmessageSettings in ddstreeSave.messages)
						{
							if (ddsmessageSettings.msgID != null && ddsmessageSettings.msgID.Length > 0)
							{
								Strings.GetTextForComponent(ddsmessageSettings.msgID, evidence5.interactable, evidence5.writer, evidence5.reciever, "\n", false, null, Strings.LinkSetting.automatic, null);
							}
						}
					}
				}
			}
		}
		foreach (string str in load.customStrings)
		{
			EvidenceCreator.Instance.CreateFactFromSerializedString(str);
		}
		foreach (Case case4 in CasePanelController.Instance.archivedCases)
		{
			foreach (Case.CaseElement caseElement2 in case4.caseElements)
			{
				if (caseElement2.id != null && caseElement2.id.Length > 0)
				{
					Evidence evidence6 = null;
					DDSSaveClasses.DDSTreeSave ddstreeSave2;
					if (Toolbox.Instance.TryGetEvidence(caseElement2.id, out evidence6) && evidence6 != null && evidence6.overrideDDS != null && evidence6.overrideDDS.Length > 0 && Toolbox.Instance.allDDSTrees.TryGetValue(evidence6.overrideDDS, ref ddstreeSave2))
					{
						foreach (DDSSaveClasses.DDSMessageSettings ddsmessageSettings2 in ddstreeSave2.messages)
						{
							Strings.GetTextForComponent(ddsmessageSettings2.msgID, evidence6.interactable, evidence6.writer, evidence6.reciever, "\n", false, null, Strings.LinkSetting.automatic, null);
						}
					}
				}
			}
		}
		if (load.hideInteractable > -1)
		{
			Interactable newHideInteractable = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(load.hideInteractable, ref newHideInteractable))
			{
				Player.Instance.OnHide(newHideInteractable, load.hideRef, true, true);
			}
		}
		else if (load.phoneInteractable > -1)
		{
			Interactable newPhone = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(load.phoneInteractable, ref newPhone))
			{
				Player.Instance.OnAnswerPhone(newPhone);
			}
		}
		else if (load.computerInteractable > -1)
		{
			Interactable newComp = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(load.computerInteractable, ref newComp))
			{
				Player.Instance.OnUseComputer(newComp, true);
			}
		}
		Player.Instance.storedTransitionPosition = load.storedTransPos;
		GameplayController.Instance.spatter = new List<SpatterSimulation>(load.spatter);
		foreach (CitySaveData.FurnitureClusterObjectCitySave furnitureClusterObjectCitySave in load.furnitureStorage)
		{
			List<FurnitureClass> list = new List<FurnitureClass>();
			foreach (string name2 in furnitureClusterObjectCitySave.furnitureClasses)
			{
				FurnitureClass furnitureClass = null;
				Toolbox.Instance.LoadDataFromResources<FurnitureClass>(name2, out furnitureClass);
				if (furnitureClass != null)
				{
					list.Add(furnitureClass);
				}
			}
			int id5 = furnitureClusterObjectCitySave.id;
			FurnitureClusterLocation newCluster = null;
			List<FurnitureClass> newClasses = list;
			int newAngle = 0;
			NewNode newAnchor = null;
			List<NewNode> newCoversNodes = null;
			bool newUseFOVBlock = false;
			Vector3 offset = furnitureClusterObjectCitySave.offset;
			FurnitureLocation furnitureLocation = new FurnitureLocation(id5, newCluster, newClasses, newAngle, newAnchor, newCoversNodes, newUseFOVBlock, default(Vector2), 5, default(Vector3), true, offset);
			Toolbox.Instance.LoadDataFromResources<FurniturePreset>(furnitureClusterObjectCitySave.furniture, out furnitureLocation.furniture);
			if (furnitureClusterObjectCitySave.art.Length > 0)
			{
				Toolbox.Instance.LoadDataFromResources<ArtPreset>(furnitureClusterObjectCitySave.art, out furnitureLocation.art);
				furnitureLocation.pickedArt = true;
			}
			furnitureLocation.matKey = furnitureClusterObjectCitySave.matKey;
			furnitureLocation.artMatKey = furnitureClusterObjectCitySave.artMatKet;
			furnitureLocation.pickedMaterials = true;
			PlayerApartmentController.Instance.furnitureStorage.Add(furnitureLocation);
		}
		if (AchievementsController.Instance != null)
		{
			AchievementsController.Instance.LoadTrackingDataFromSave(ref load);
		}
		foreach (SpatterSimulation spatterSimulation in GameplayController.Instance.spatter)
		{
			spatterSimulation.LoadFromSerializedData();
		}
		UpgradesController.Instance.UpdateActivation();
		UpgradeEffectController.Instance.OnSyncDiskChange(true);
		FirstPersonItemController.Instance.SetSlotSize(GameplayControls.Instance.defaultInventorySlots + Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.increaseInventory)));
		foreach (FirstPersonItemController.InventorySlot inventorySlot in load.firstPersonItems)
		{
			if (inventorySlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
			{
				FirstPersonItemController.Instance.slots.Add(inventorySlot);
			}
		}
		FirstPersonItemController.Instance.SetSlotSize(GameplayControls.Instance.defaultInventorySlots + Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.increaseInventory)));
		foreach (StateSaveData.ScannedObjPrint scannedObjPrint in load.scannedPrints)
		{
			Interactable interactable8 = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(scannedObjPrint.objID, ref interactable8))
			{
				List<Interactable> list2 = new List<Interactable>();
				foreach (int num4 in scannedObjPrint.prints)
				{
					Interactable interactable9 = null;
					if (CityData.Instance.savableInteractableDictionary.TryGetValue(num4, ref interactable9) && !BioScreenController.Instance.scannedObjectsPrintsCache.ContainsKey(interactable8))
					{
						BioScreenController.Instance.scannedObjectsPrintsCache.Add(interactable8, list2);
					}
				}
			}
		}
		if (load.carried > -1)
		{
			Interactable newObj = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(load.carried, ref newObj))
			{
				InteractionController.Instance.PickUp(newObj);
			}
		}
		UpgradesController.Instance.UpdateUpgrades();
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0008BCF4 File Offset: 0x00089EF4
	private void LoadJob(SideJob job)
	{
		job.SetupNonSerializedData();
		job.SetJobState(job.state, true);
		if (job.state == SideJob.JobState.generated)
		{
			job.PostJob();
		}
		job.CreateAcqusitionFacts();
		Game.Log("Jobs: Loaded job " + job.preset.name, 2);
	}

	// Token: 0x04000932 RID: 2354
	private static SaveStateController _instance;
}
