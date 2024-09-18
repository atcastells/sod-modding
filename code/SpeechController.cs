using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200042B RID: 1067
public class SpeechController : MonoBehaviour
{
	// Token: 0x0600181E RID: 6174 RVA: 0x00168518 File Offset: 0x00166718
	public virtual void TriggerBark(SpeechController.Bark newBark)
	{
		if (this.actor.isDead || this.actor.isStunned)
		{
			return;
		}
		if (InterfaceController.Instance.activeSpeechBubbles.Count >= CitizenControls.Instance.maxSpeechBubbles)
		{
			return;
		}
		if (this.actor.inConversation)
		{
			return;
		}
		if (this.actor.interactable != null && InteractionController.Instance.talkingTo == this.actor.interactable)
		{
			return;
		}
		if (newBark == SpeechController.Bark.persuit)
		{
			this.Speak("f1d8d284-cee7-4243-b77e-342c698029a2", false, true, null, null);
			return;
		}
		if (newBark == SpeechController.Bark.lostTarget)
		{
			this.Speak("fbe69a11-2a9b-4c6b-b979-6637b66f7bb9", false, true, null, null);
			return;
		}
		if (newBark == SpeechController.Bark.answeringDoor)
		{
			this.Speak("1f677cec-70a1-4c97-9000-131135807b31", true, true, null, null);
			return;
		}
		if (newBark == SpeechController.Bark.answerDoor)
		{
			if (!(this.actor.currentGameLocation != null) || !(this.actor.currentGameLocation.thisAsAddress != null) || !(this.actor.currentGameLocation.thisAsAddress.addressPreset != null) || !this.actor.currentGameLocation.thisAsAddress.addressPreset.needsPassword)
			{
				this.Speak("d73fa4d4-6e57-46d0-8cd2-1f27dd84e541", false, true, null, null);
				return;
			}
			if (!GameplayController.Instance.playerKnowsPasswords.Contains(this.actor.currentGameLocation.thisAsAddress.id))
			{
				this.Speak("279cee3f-6799-4e7a-a93a-3e1c638e808c", false, true, null, null);
				return;
			}
			this.Speak("d73fa4d4-6e57-46d0-8cd2-1f27dd84e541", false, true, null, null);
			return;
		}
		else
		{
			if (newBark == SpeechController.Bark.giveUpSearch)
			{
				this.Speak("e0dd615f-6c72-45da-97ab-b7817bf11609", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.hearsSuspicious)
			{
				this.Speak("32b64e58-660f-4a9c-be30-961233a8bf03", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.seesSuspicious)
			{
				this.Speak("d29bbe40-437f-4ddf-adc3-84fd33aa7c30", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.enforcerRadio)
			{
				this.Speak("4249d7a9-3d34-4fe0-96ae-3ab8d348ab3b", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.idleSounds)
			{
				this.Speak("18b75486-cee4-4d52-ae06-dbdbacdd531b", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.discoverTamper)
			{
				this.Speak("a7e07cb7-4b4b-46d3-99fc-ff1adb044913", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.fallOffChair)
			{
				this.Speak("7df85447-902e-4f41-9691-76d0edd318a5", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.sleeping)
			{
				this.Speak("23719aae-b101-49f8-994f-ec7759d795c8", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.yawn)
			{
				this.Speak("1a5c991a-e4e0-4e05-a436-122395774555", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.hearsObject)
			{
				this.Speak("28fe0ce5-60db-4c17-8103-88fcb0a41919", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.stench)
			{
				this.Speak("31f09ad7-0274-4b95-be11-29f9c1680da9", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.seeBody)
			{
				this.Speak("4dcf2086-e959-4c70-9f32-7f4ba55f63c8", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.examineBody)
			{
				this.Speak("84b48e9d-2db5-4000-86c1-77100d9635b3", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.mourn)
			{
				this.Speak("fabeec40-1090-4a41-8600-cfab8a4a8bb2", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.enforcersKnock)
			{
				this.Speak("8d0dc67c-b8d4-466d-a827-585aa77a9455", true, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.scared)
			{
				this.Speak("a05f9f17-143d-411e-a16c-af7f4181f29f", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.attack)
			{
				this.Speak("88814142-7e3a-4452-9f36-06a2fa853c21", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.confrontMessingAround)
			{
				this.Speak("84b9a4a2-7ff9-4a5e-a624-94d8cf7e4ffb", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.pickUpMisplaced)
			{
				this.Speak("246df5c6-c562-424a-aa10-519b15ebd395", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.takeDamage)
			{
				this.Speak("70ee1e8a-a41f-4265-86ce-0bb24f561360", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.frustration)
			{
				this.Speak("6bc04ea6-e946-4f8e-aa8e-ec4cb99a69f6", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.outOfBreath)
			{
				this.Speak("8448cf9f-ad0c-4e71-aa05-3e878cc5d32a", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.cold)
			{
				this.Speak("d748cbac-ea16-46bf-9c46-153a0c73a01f", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.drunkIdle)
			{
				this.Speak("f17c6437-b71c-4745-a5d2-67ae78a5596e", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.targetDown)
			{
				this.Speak("989c6280-a58b-46cb-9392-5fa683489463", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.restrained)
			{
				this.Speak("7e54efc0-9a06-4ea5-9d52-6993cdd67a60", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.restrainedIdle)
			{
				this.Speak("d086041a-3048-4614-baff-a3d5550a4099", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.dazed)
			{
				this.Speak("42e04710-ce12-489c-afd9-a0954b909876", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.trespass)
			{
				this.Speak("ad2123f9-d1cb-479a-b6c4-fd3cca346f63", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.trespassClosed)
			{
				this.Speak("0f900ab4-f091-4b65-829f-1c0f3f8988ac", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.trespassLoiter)
			{
				this.Speak("76bd43d0-627d-4f86-ac6f-cabea3ec2fe8", false, false, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.threatenByCombat)
			{
				this.Speak("6e8d2443-ba0c-4f81-8075-31e4bdbd8d84", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.threatenByItem)
			{
				this.Speak("6e011e82-dfdc-4abe-b2fc-1f776bdc89da", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.soundAlarm)
			{
				this.Speak("c37dd664-29b2-40b4-a8bf-6748ee4e05f6", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.doorBlocked)
			{
				this.Speak("7aba83bc-9782-4d8d-87a6-b5d2dfa8b997", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.spooked)
			{
				this.Speak("5a75ed21-287a-4592-a419-a3cb279e292f", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.exposedConfront)
			{
				this.Speak("df8e6c66-5907-43cb-a9c5-f1f63d39e199", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.spookConfront)
			{
				this.Speak("75cd1949-c792-45b0-8eed-664d5a47f0cf", false, true, null, null);
				return;
			}
			if (newBark == SpeechController.Bark.loiteringConfront)
			{
				this.Speak("68df6d68-8831-4b5c-b9e9-ec6ebec5a133", false, true, null, null);
			}
			return;
		}
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x001689BC File Offset: 0x00166BBC
	public virtual void Speak(ref List<AIActionPreset.AISpeechPreset> speechOptions, Human speakAbout = null, SideJob sideJob = null, DialogPreset dialogPreset = null, Interactable saysTo = null)
	{
		if (speechOptions.Count <= 0)
		{
			Game.LogError("Speech has 0 options", 2);
		}
		if (CutSceneController.Instance.cutSceneActive && CutSceneController.Instance.preset.disableAISpeech && !this.actor.isPlayer)
		{
			return;
		}
		List<AIActionPreset.AISpeechPreset> list = new List<AIActionPreset.AISpeechPreset>();
		foreach (AIActionPreset.AISpeechPreset aispeechPreset in speechOptions)
		{
			if (aispeechPreset.mustFeatureTrait.Count > 0)
			{
				Human human = this.actor as Human;
				if (!(human != null))
				{
					continue;
				}
				bool flag = false;
				using (List<CharacterTrait>.Enumerator enumerator2 = aispeechPreset.mustFeatureTrait.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait t = enumerator2.Current;
						if (human.characterTraits.Exists((Human.Trait item) => item.trait == t))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					continue;
				}
			}
			if (aispeechPreset.cantFeatureTrait.Count > 0)
			{
				Human human2 = this.actor as Human;
				if (!(human2 != null))
				{
					continue;
				}
				bool flag2 = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = aispeechPreset.cantFeatureTrait.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait t = enumerator2.Current;
						if (human2.characterTraits.Exists((Human.Trait item) => item.trait == t))
						{
							flag2 = false;
							break;
						}
					}
				}
				if (!flag2)
				{
					continue;
				}
			}
			if ((!aispeechPreset.onlyIfEnfocerOnDuty || (this.actor.isEnforcer && this.actor.isOnDuty)) && (!aispeechPreset.onlyIfEnfocerOnDuty || !this.actor.isEnforcer || !this.actor.isOnDuty) && (aispeechPreset.mustBeKillerWithMotive.Count <= 0 || (MurderController.Instance.currentMurderer == this.actor && aispeechPreset.mustBeKillerWithMotive.Contains(MurderController.Instance.chosenMO))))
			{
				for (int i = 0; i < aispeechPreset.chance; i++)
				{
					list.Add(aispeechPreset);
				}
			}
		}
		if (list.Count > 0)
		{
			AIActionPreset.AISpeechPreset aispeechPreset2 = list[Toolbox.Instance.Rand(0, list.Count, false)];
			if (this.actor != null && this.actor.evidenceEntry != null)
			{
				if (aispeechPreset2.tieKeys.Count > 0)
				{
					foreach (Evidence.DataKey dataKey in aispeechPreset2.tieKeys)
					{
						Game.Log("Debug: Merging key " + dataKey.ToString() + "...", 2);
						foreach (Evidence.DataKey dataKey2 in aispeechPreset2.tieKeys)
						{
							if (dataKey != dataKey2)
							{
								this.actor.evidenceEntry.MergeDataKeys(dataKey, dataKey2);
							}
						}
					}
				}
				foreach (Evidence.Discovery disc in aispeechPreset2.applyDiscovery)
				{
					this.actor.evidenceEntry.AddDiscovery(disc);
				}
			}
			Human human3 = this.actor as Human;
			if (human3 == null)
			{
				human3 = Player.Instance;
			}
			string text = aispeechPreset2.ddsMessageID;
			if (aispeechPreset2.useMurderMOConfession && MurderController.Instance.currentMurderer == human3 && MurderController.Instance.chosenMO != null && MurderController.Instance.chosenMO.confessionalDDSResponses.Count > 0)
			{
				text = MurderController.Instance.chosenMO.confessionalDDSResponses[Toolbox.Instance.Rand(0, MurderController.Instance.chosenMO.confessionalDDSResponses.Count, false)];
				if (!Toolbox.Instance.allDDSMessages.ContainsKey(text))
				{
					text = aispeechPreset2.ddsMessageID;
				}
			}
			if (aispeechPreset2 != null && text != null && text.Length > 0)
			{
				if (Toolbox.Instance.allDDSMessages.ContainsKey(text))
				{
					Acquaintance aq = null;
					human3.FindAcquaintanceExists(Player.Instance, out aq);
					List<int> list3;
					List<string> list2 = human3.ParseDDSMessage(text, aq, out list3, true, sideJob, false);
					bool flag3 = false;
					bool flag4 = false;
					for (int j = 0; j < list2.Count; j++)
					{
						if (aispeechPreset2.endsDialog && j >= list2.Count - 1)
						{
							flag3 = true;
						}
						if (aispeechPreset2.jobHandIn && j >= list2.Count - 1)
						{
							flag4 = true;
						}
						string dictionary = "dds.blocks";
						string speechEntryRef = list2[j];
						bool useParsing = true;
						bool shout = aispeechPreset2.shout;
						bool interupt = aispeechPreset2.interupt;
						float delay = 0f;
						bool forceColour = false;
						bool endsDialog = flag3;
						bool jobHandIn = flag4;
						AIActionPreset.AISpeechPreset dialog = aispeechPreset2;
						this.Speak(dictionary, speechEntryRef, useParsing, shout, interupt, delay, forceColour, default(Color), speakAbout, endsDialog, jobHandIn, sideJob, dialogPreset, dialog, saysTo);
						if (aispeechPreset2.startCombat && j >= list2.Count - 1 && human3.ai != null)
						{
							Game.Log("Setting combat persuit from dialog option...", 2);
							human3.ai.SetPersue(Player.Instance, false, 2, true, 10f);
						}
						if (aispeechPreset2.flee && j >= list2.Count - 1 && human3.ai != null)
						{
							Game.Log("Setting flee from dialog option...", 2);
							human3.AddNerve(-9999999f, Player.Instance);
						}
						if (aispeechPreset2.giveUpSelf && j >= list2.Count - 1 && human3.ai != null)
						{
							Game.Log("Setting give up self from dialog option...", 2);
							human3.ai.CreateNewGoal(RoutineControls.Instance.giveSelfUp, 0f, 0f, null, null, null, null, null, -2);
						}
					}
					return;
				}
				Game.LogError("Missing DDS message: " + text, 2);
				return;
			}
			else
			{
				Game.LogError("No DDS message for speech... [" + aispeechPreset2.dictionaryString + "]", 2);
			}
		}
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x001690A8 File Offset: 0x001672A8
	public virtual void Speak(string ddsMessage, bool shout = false, bool interupt = false, Human speakAbout = null, SideJob sideJob = null)
	{
		if (CutSceneController.Instance.cutSceneActive && CutSceneController.Instance.preset.disableAISpeech && !this.actor.isPlayer)
		{
			return;
		}
		if (ddsMessage == null || ddsMessage.Length <= 0)
		{
			Game.LogError("No DDS message for speech...", 2);
			return;
		}
		if (Toolbox.Instance.allDDSMessages.ContainsKey(ddsMessage))
		{
			Acquaintance aq = null;
			Human human = this.actor as Human;
			List<string> list;
			if (human != null)
			{
				human.FindAcquaintanceExists(Player.Instance, out aq);
				List<int> list2;
				list = human.ParseDDSMessage(ddsMessage, aq, out list2, true, sideJob, false);
			}
			else
			{
				List<int> list2;
				list = Player.Instance.ParseDDSMessage(ddsMessage, null, out list2, true, sideJob, false);
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < list.Count; i++)
			{
				string dictionary = "dds.blocks";
				string speechEntryRef = list[i];
				bool useParsing = true;
				float delay = 0f;
				bool forceColour = false;
				bool endsDialog = flag;
				bool jobHandIn = flag2;
				this.Speak(dictionary, speechEntryRef, useParsing, shout, interupt, delay, forceColour, default(Color), speakAbout, endsDialog, jobHandIn, sideJob, null, null, null);
			}
			return;
		}
		Game.LogError("Missing DDS message: " + ddsMessage, 2);
	}

	// Token: 0x06001821 RID: 6177 RVA: 0x001691CC File Offset: 0x001673CC
	public virtual void Speak(string dictionary, string speechEntryRef, bool useParsing = false, bool shout = false, bool interupt = false, float delay = 0f, bool forceColour = false, Color color = default(Color), Human speakingAbout = null, bool endsDialog = false, bool jobHandIn = false, SideJob sideJob = null, DialogPreset dialogPreset = null, AIActionPreset.AISpeechPreset dialog = null, Interactable speakingTo = null)
	{
		if (this.actor != null)
		{
			if (!this.actor.isPlayer && (this.actor.currentCityTile == null || !this.actor.currentCityTile.isInPlayerVicinity))
			{
				return;
			}
			if (CutSceneController.Instance.cutSceneActive && CutSceneController.Instance.preset.disableAISpeech && !this.actor.isPlayer)
			{
				return;
			}
			if (Game.Instance.collectDebugData)
			{
				this.actor.SelectedDebug("Speech: " + this.actor.name + ": " + Strings.Get(dictionary, speechEntryRef, Strings.Casing.asIs, false, false, false, null), Actor.HumanDebug.actions);
			}
		}
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		this.speechQueue.Add(new SpeechController.QueueElement(dictionary, speechEntryRef, useParsing, delay, shout, interupt, forceColour, color, speakingAbout, endsDialog, jobHandIn, sideJob, dialogPreset, dialog, speakingTo));
		base.enabled = true;
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x001692C8 File Offset: 0x001674C8
	private void Update()
	{
		if (this.speechQueue.Count <= 0)
		{
			if (this.activeSpeechBubble == null)
			{
				this.SetSpeechActive(false);
				if (this.actor != null)
				{
					this.actor.isSpeaking = false;
				}
			}
			base.enabled = false;
			return;
		}
		try
		{
			this.SetSpeechActive(true);
			if (!(PopupMessageController.Instance != null) || !PopupMessageController.Instance.active)
			{
				if (this.activeSpeechBubble == null && this.speechDelay <= 0f)
				{
					SpeechController.QueueElement sp = this.speechQueue[0];
					if (this.actor != null)
					{
						if (this.actor.isPlayer && InterfaceController.Instance.helpPointerQueue.Count > 0 && !sp.interupt && !CutSceneController.Instance.cutSceneActive)
						{
							Game.Log("Objective: Speech queue is waiting...", 2);
							return;
						}
						if (!sp.isObjective && !this.actor.isPlayer && !this.actor.currentCityTile.isInPlayerVicinity)
						{
							this.speechQueue.RemoveAt(0);
							return;
						}
						if (this.actor.isDead || this.actor.isStunned)
						{
							this.speechQueue.RemoveAt(0);
							return;
						}
					}
					if (!sp.delayActivated)
					{
						this.speechDelay = sp.delay;
						sp.delayActivated = true;
					}
					if (this.speechDelay <= 0f)
					{
						if (sp.isObjective)
						{
							Case @case = CasePanelController.Instance.activeCases.Find((Case item) => item.id == sp.caseID);
							if (@case == null)
							{
								@case = CasePanelController.Instance.archivedCases.Find((Case item) => item.id == sp.caseID);
							}
							if (@case != null && @case.currentActiveObjectives.Exists((Objective item) => item.queueElement.entryRef == sp.entryRef))
							{
								Game.LogError("Objective: Duplicate objective " + sp.entryRef + " found: Not creating another one!", 2);
								if (this.speechQueue.Count > 0)
								{
									this.speechQueue.RemoveAt(0);
								}
							}
							else
							{
								if (sp.removePreviousObjectives)
								{
									if (@case != null)
									{
										Game.Log("Removing previous objectives for " + @case.name, 2);
										for (int i = 0; i < @case.currentActiveObjectives.Count; i++)
										{
											@case.currentActiveObjectives[i].Cancel();
										}
									}
									for (int j = 0; j < Player.Instance.speechController.speechQueue.Count; j++)
									{
										if (Player.Instance.speechController.speechQueue[j].isObjective)
										{
											if (Player.Instance.speechController.activeSpeechBubble != null && Player.Instance.speechController.activeSpeechBubble.speech == Player.Instance.speechController.speechQueue[j])
											{
												Object.Destroy(Player.Instance.speechController.activeSpeechBubble.gameObject);
												Player.Instance.speechController.activeSpeechBubble = null;
											}
											Player.Instance.speechController.speechQueue.RemoveAt(j);
											j--;
										}
									}
								}
								if (@case != null)
								{
									new Objective(sp);
								}
								if (this.speechQueue.Count > 0)
								{
									this.speechQueue.RemoveAt(0);
								}
							}
						}
						else if (SessionData.Instance.play)
						{
							if (SessionData.Instance.currentTimeSpeed == SessionData.TimeSpeed.normal)
							{
								AudioEvent audioEvent = null;
								Toolbox.Instance.voiceActedDictionary.TryGetValue(sp.entryRef, ref audioEvent);
								if (audioEvent == null)
								{
									audioEvent = AudioControls.Instance.speakEvent;
									if (sp.shouting)
									{
										audioEvent = AudioControls.Instance.shoutEvent;
									}
								}
								NewNode sourceLocation;
								if (this.actor != null)
								{
									sourceLocation = this.actor.currentNode;
								}
								else
								{
									sourceLocation = this.interactable.node;
								}
								int num = 0;
								float num2;
								if (this.actor != null && this.actor.isPlayer)
								{
									num2 = 1f;
								}
								else
								{
									List<AudioController.ActiveListener> list;
									bool flag;
									List<NewRoom> list2;
									float num3;
									num2 = AudioController.Instance.GetOcculusion(Player.Instance.currentNode, sourceLocation, audioEvent, 1f, this.actor, null, out num, out list, out flag, out list2, out num3, null, false);
								}
								if (num2 <= 0.01f)
								{
									this.speechQueue.RemoveAt(0);
									return;
								}
								if (this.actor != Player.Instance)
								{
									if (AudioController.Instance.GetPlayersSoundLevel(sourceLocation, audioEvent, num2, null) < 0.1f)
									{
										this.speechQueue.RemoveAt(0);
										return;
									}
									if (this.actor != null)
									{
										this.actor.HeardByPlayer();
									}
								}
								GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.speechBubble, InterfaceControls.Instance.speechBubbleParent);
								this.activeSpeechBubble = gameObject.GetComponent<SpeechBubbleController>();
								this.activeSpeechBubble.Setup(sp, this);
								this.lastSpeech = SessionData.Instance.gameTime;
								if (this.actor != null && !this.actor.isPlayer)
								{
									this.activeSpeechBubble.awarenessIcon = InterfaceController.Instance.AddAwarenessIcon(InterfaceController.AwarenessType.actor, InterfaceController.AwarenessBehaviour.invisibleInfront, this.actor, null, Vector3.zero, InterfaceControls.Instance.speech, 0, true, audioEvent.hearingRange);
								}
							}
							this.speechQueue.RemoveAt(0);
						}
					}
				}
				else if (this.speechDelay > 0f && SessionData.Instance.play)
				{
					this.speechDelay -= Time.deltaTime;
				}
			}
		}
		catch
		{
			Game.LogError("Objective: Error processing speech queue!", 2);
		}
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x001698CC File Offset: 0x00167ACC
	public void SetSpeechActive(bool val)
	{
		if (this.speechActive != val)
		{
			this.speechActive = val;
			if (InteractionController.Instance.dialogMode && (InteractionController.Instance.talkingTo == this.interactable || (InteractionController.Instance.isRemote && InteractionController.Instance.remoteOverride == this.interactable)))
			{
				InteractionController.Instance.RefreshDialogOptions();
			}
		}
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x0016992F File Offset: 0x00167B2F
	private void OnEnable()
	{
		if (this.speechQueue.Count > 0)
		{
			base.enabled = true;
		}
	}

	// Token: 0x04001DC4 RID: 7620
	public Actor actor;

	// Token: 0x04001DC5 RID: 7621
	[NonSerialized]
	public Interactable interactable;

	// Token: 0x04001DC6 RID: 7622
	public Telephone phoneLine;

	// Token: 0x04001DC7 RID: 7623
	[Header("Speech")]
	public SpeechBubbleController activeSpeechBubble;

	// Token: 0x04001DC8 RID: 7624
	public bool endAfterThisSpeech;

	// Token: 0x04001DC9 RID: 7625
	public float lastSpeech;

	// Token: 0x04001DCA RID: 7626
	public List<SpeechController.QueueElement> speechQueue = new List<SpeechController.QueueElement>();

	// Token: 0x04001DCB RID: 7627
	public float speechDelay;

	// Token: 0x04001DCC RID: 7628
	public bool speechActive;

	// Token: 0x0200042C RID: 1068
	[Serializable]
	public class QueueElement
	{
		// Token: 0x06001826 RID: 6182 RVA: 0x0016995C File Offset: 0x00167B5C
		public QueueElement(string newDictRef, string newEntryRef, bool newUseParsing, float newDelay, bool newIsShouting, bool newInterupt, bool newForceColour = false, Color newColor = default(Color), Human newSpeakingAbout = null, bool newEndsDialog = false, bool newJobHandIn = false, SideJob newJobRef = null, DialogPreset newDialogPreset = null, AIActionPreset.AISpeechPreset newDialog = null, Interactable newSpeakingTo = null)
		{
			this.dictRef = newDictRef;
			this.entryRef = newEntryRef;
			this.useParsing = newUseParsing;
			this.delay = newDelay;
			this.shouting = newIsShouting;
			this.isObjective = false;
			this.interupt = newInterupt;
			this.forceColour = newForceColour;
			this.color = newColor;
			this.endsDialog = newEndsDialog;
			this.jobHandIn = newJobHandIn;
			this.dialog = newDialog;
			if (newJobRef != null)
			{
				this.jobRef = newJobRef.jobID;
			}
			if (newSpeakingAbout != null)
			{
				this.speakingAbout = newSpeakingAbout.humanID;
			}
			if (newSpeakingTo != null)
			{
				this.speakingToRef = newSpeakingTo.id;
			}
			if (newDialogPreset != null)
			{
				this.dialogPreset = newDialogPreset.name;
			}
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x00169A40 File Offset: 0x00167C40
		public QueueElement(int newCaseID, string newName, bool newUseUIPointer, Vector3 newUseUIPosition, InterfaceControls.Icon newIcon, List<Objective.ObjectiveTrigger> newTriggers, Objective.OnCompleteAction newOnCompleteAction, float newDelay = 0f, bool newRemoveObjectives = false, string newChapterString = "", bool newIsSilent = false, bool newAllowCrouchPromt = false, SideJob newJobRef = null, bool newForceBottom = false, bool newUseParsing = true)
		{
			this.entryRef = newName;
			this.caseID = newCaseID;
			this.isObjective = true;
			this.useParsing = newUseParsing;
			this.forceBottom = newForceBottom;
			if (newJobRef != null)
			{
				this.jobRef = newJobRef.jobID;
			}
			this.usePointer = newUseUIPointer;
			this.pointerPosition = newUseUIPosition;
			this.delay = newDelay;
			this.triggers = newTriggers;
			this.onComplete = newOnCompleteAction;
			this.removePreviousObjectives = newRemoveObjectives;
			this.chapterString = newChapterString;
			this.isSilent = newIsSilent;
			this.allowCrouchPrompt = newAllowCrouchPromt;
			this.icon = newIcon;
		}

		// Token: 0x04001DCD RID: 7629
		public string dictRef;

		// Token: 0x04001DCE RID: 7630
		public string entryRef;

		// Token: 0x04001DCF RID: 7631
		public bool useParsing = true;

		// Token: 0x04001DD0 RID: 7632
		public float delay;

		// Token: 0x04001DD1 RID: 7633
		public bool delayActivated;

		// Token: 0x04001DD2 RID: 7634
		public bool shouting;

		// Token: 0x04001DD3 RID: 7635
		public bool interupt;

		// Token: 0x04001DD4 RID: 7636
		public bool forceColour;

		// Token: 0x04001DD5 RID: 7637
		public Color color;

		// Token: 0x04001DD6 RID: 7638
		public int speakingAbout = -1;

		// Token: 0x04001DD7 RID: 7639
		public int jobRef = -1;

		// Token: 0x04001DD8 RID: 7640
		public bool endsDialog;

		// Token: 0x04001DD9 RID: 7641
		public bool jobHandIn;

		// Token: 0x04001DDA RID: 7642
		public int speakingToRef = -1;

		// Token: 0x04001DDB RID: 7643
		public AIActionPreset.AISpeechPreset dialog;

		// Token: 0x04001DDC RID: 7644
		public string dialogPreset;

		// Token: 0x04001DDD RID: 7645
		public bool isObjective;

		// Token: 0x04001DDE RID: 7646
		public bool usePointer;

		// Token: 0x04001DDF RID: 7647
		public Vector3 pointerPosition;

		// Token: 0x04001DE0 RID: 7648
		public List<Objective.ObjectiveTrigger> triggers;

		// Token: 0x04001DE1 RID: 7649
		public Objective.OnCompleteAction onComplete;

		// Token: 0x04001DE2 RID: 7650
		public bool removePreviousObjectives;

		// Token: 0x04001DE3 RID: 7651
		public string chapterString;

		// Token: 0x04001DE4 RID: 7652
		public bool isSilent;

		// Token: 0x04001DE5 RID: 7653
		public bool allowCrouchPrompt;

		// Token: 0x04001DE6 RID: 7654
		public InterfaceControls.Icon icon;

		// Token: 0x04001DE7 RID: 7655
		public int caseID = -1;

		// Token: 0x04001DE8 RID: 7656
		public bool forceBottom;
	}

	// Token: 0x0200042D RID: 1069
	public enum Bark
	{
		// Token: 0x04001DEA RID: 7658
		persuit,
		// Token: 0x04001DEB RID: 7659
		lostTarget,
		// Token: 0x04001DEC RID: 7660
		answeringDoor,
		// Token: 0x04001DED RID: 7661
		answerDoor,
		// Token: 0x04001DEE RID: 7662
		giveUpSearch,
		// Token: 0x04001DEF RID: 7663
		hearsSuspicious,
		// Token: 0x04001DF0 RID: 7664
		seesSuspicious,
		// Token: 0x04001DF1 RID: 7665
		enforcerRadio,
		// Token: 0x04001DF2 RID: 7666
		idleSounds,
		// Token: 0x04001DF3 RID: 7667
		discoverTamper,
		// Token: 0x04001DF4 RID: 7668
		fallOffChair,
		// Token: 0x04001DF5 RID: 7669
		sleeping,
		// Token: 0x04001DF6 RID: 7670
		yawn,
		// Token: 0x04001DF7 RID: 7671
		hearsObject,
		// Token: 0x04001DF8 RID: 7672
		stench,
		// Token: 0x04001DF9 RID: 7673
		seeBody,
		// Token: 0x04001DFA RID: 7674
		examineBody,
		// Token: 0x04001DFB RID: 7675
		mourn,
		// Token: 0x04001DFC RID: 7676
		enforcersKnock,
		// Token: 0x04001DFD RID: 7677
		scared,
		// Token: 0x04001DFE RID: 7678
		cower,
		// Token: 0x04001DFF RID: 7679
		attack,
		// Token: 0x04001E00 RID: 7680
		confrontMessingAround,
		// Token: 0x04001E01 RID: 7681
		pickUpMisplaced,
		// Token: 0x04001E02 RID: 7682
		takeDamage,
		// Token: 0x04001E03 RID: 7683
		frustration,
		// Token: 0x04001E04 RID: 7684
		outOfBreath,
		// Token: 0x04001E05 RID: 7685
		cold,
		// Token: 0x04001E06 RID: 7686
		drunkIdle,
		// Token: 0x04001E07 RID: 7687
		targetDown,
		// Token: 0x04001E08 RID: 7688
		restrained,
		// Token: 0x04001E09 RID: 7689
		restrainedIdle,
		// Token: 0x04001E0A RID: 7690
		dazed,
		// Token: 0x04001E0B RID: 7691
		trespass,
		// Token: 0x04001E0C RID: 7692
		threatenByItem,
		// Token: 0x04001E0D RID: 7693
		threatenByCombat,
		// Token: 0x04001E0E RID: 7694
		soundAlarm,
		// Token: 0x04001E0F RID: 7695
		doorBlocked,
		// Token: 0x04001E10 RID: 7696
		spooked,
		// Token: 0x04001E11 RID: 7697
		exposedConfront,
		// Token: 0x04001E12 RID: 7698
		spookConfront,
		// Token: 0x04001E13 RID: 7699
		loiteringConfront,
		// Token: 0x04001E14 RID: 7700
		trespassClosed,
		// Token: 0x04001E15 RID: 7701
		trespassLoiter
	}
}
