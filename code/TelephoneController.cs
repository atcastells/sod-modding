using System;
using System.Collections.Generic;
using FMOD.Studio;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class TelephoneController : MonoBehaviour
{
	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000940 RID: 2368 RVA: 0x00091440 File Offset: 0x0008F640
	// (remove) Token: 0x06000941 RID: 2369 RVA: 0x00091478 File Offset: 0x0008F678
	public event TelephoneController.PlayerCall OnPlayerCall;

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000942 RID: 2370 RVA: 0x000914AD File Offset: 0x0008F6AD
	public static TelephoneController Instance
	{
		get
		{
			return TelephoneController._instance;
		}
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x000914B4 File Offset: 0x0008F6B4
	private void Awake()
	{
		if (TelephoneController._instance != null && TelephoneController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		TelephoneController._instance = this;
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x000914E4 File Offset: 0x0008F6E4
	public TelephoneController.PhoneCall CreateNewCall(int from, int to, Human caller, Human intendedReceiver, TelephoneController.CallSource callSource, float maxRingTime = 0.1f, bool specificRecevier = false)
	{
		Telephone from2 = null;
		if (CityData.Instance.phoneDictionary.TryGetValue(from, ref from2))
		{
			Telephone to2 = null;
			if (CityData.Instance.phoneDictionary.TryGetValue(to, ref to2))
			{
				return this.CreateNewCall(from2, to2, caller, intendedReceiver, callSource, maxRingTime, specificRecevier);
			}
		}
		return null;
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x00091530 File Offset: 0x0008F730
	public TelephoneController.PhoneCall CreateNewCall(Telephone from, Telephone to, Human caller, Human intendedReceiver, TelephoneController.CallSource callSource, float maxRingTime = 0.1f, bool specificRecevier = false)
	{
		if (from == to)
		{
			Game.LogError("Cannot make call to the same phone!", 2);
			return null;
		}
		if (caller == intendedReceiver && caller != null)
		{
			Game.LogError("Cannot make call to the same person!", 2);
			return null;
		}
		TelephoneController.PhoneCall phoneCall = new TelephoneController.PhoneCall(from, to, SessionData.Instance.gameTime, caller, intendedReceiver, callSource, maxRingTime, specificRecevier);
		if (from != null)
		{
			if (from.location.building != null)
			{
				if (from.location.building.callLog.Count >= GameplayControls.Instance.buildingCallLogMax)
				{
					from.location.building.callLog.RemoveAt(0);
				}
				from.location.building.callLog.Add(phoneCall);
			}
			if (from.dialTone != null)
			{
				Game.Log("Stop dial tone", 2);
				AudioController.Instance.StopSound(from.dialTone, AudioController.StopType.immediate, "call while picked up");
				from.dialTone = null;
			}
		}
		if (to != null && to.location.building != null)
		{
			if (to.location.building.callLog.Count >= GameplayControls.Instance.buildingCallLogMax)
			{
				to.location.building.callLog.RemoveAt(0);
			}
			to.location.building.callLog.Add(phoneCall);
		}
		this.AddActiveCall(phoneCall);
		if (phoneCall.callerNS != null && phoneCall.callerNS.isPlayer)
		{
			phoneCall.connecting = AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.phoneConnect, phoneCall.callerNS, phoneCall.callerNS.currentNode, phoneCall.callerNS.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
		}
		return phoneCall;
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x000916EB File Offset: 0x0008F8EB
	public void OnPlayerCalls()
	{
		if (this.OnPlayerCall != null)
		{
			this.OnPlayerCall();
		}
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x00091700 File Offset: 0x0008F900
	public void AddFakeNumber(int number, TelephoneController.CallSource source)
	{
		if (!this.fakeTelephoneDictionary.ContainsKey(number))
		{
			this.fakeTelephoneDictionary.Add(number, source);
		}
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x0009171D File Offset: 0x0008F91D
	public void RemoveFakeNumber(int number)
	{
		if (this.fakeTelephoneDictionary.ContainsKey(number))
		{
			this.fakeTelephoneDictionary.Remove(number);
		}
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x0009173C File Offset: 0x0008F93C
	public void AddActiveCall(TelephoneController.PhoneCall newCall)
	{
		if (!this.activeCalls.Contains(newCall))
		{
			this.activeCalls.Add(newCall);
			this.gameTimeLastLoop = SessionData.Instance.gameTime;
			base.enabled = true;
			if (newCall.callerNS != null && newCall.callerNS.isPlayer)
			{
				Game.Log("Set player active call", 2);
				Player.Instance.activeCall = newCall;
			}
		}
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x000917AC File Offset: 0x0008F9AC
	public void RemoveActiveCall(TelephoneController.PhoneCall newCall)
	{
		this.activeCalls.Remove(newCall);
		if (newCall.callerNS != null && newCall.callerNS.isPlayer)
		{
			Game.Log("Phone: Remove player active call", 2);
			Player.Instance.activeCall = null;
			return;
		}
		if (newCall.recevierNS != null && newCall.recevierNS.isPlayer)
		{
			Game.Log("Phone: Remove player active call", 2);
			Player.Instance.activeCall = null;
		}
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x0009182C File Offset: 0x0008FA2C
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			float num = SessionData.Instance.gameTime - this.gameTimeLastLoop;
			if (this.activeCalls.Count > 0)
			{
				for (int i = 0; i < this.activeCalls.Count; i++)
				{
					TelephoneController.PhoneCall phoneCall = this.activeCalls[i];
					if (phoneCall.state == TelephoneController.CallState.dialing)
					{
						phoneCall.dialingTimer += num;
						if (phoneCall.dialingTimer > 0.0037f)
						{
							if (phoneCall.toNS != null && phoneCall.toNS.activeCall.Count > 0)
							{
								if (phoneCall.callerNS != null && phoneCall.callerNS.isPlayer)
								{
									InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "telephone engaged", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.citizen, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
								}
								Game.Log(string.Concat(new string[]
								{
									"Gameplay: Call ",
									phoneCall.fromNS.numberString,
									" to ",
									phoneCall.toNS.numberString,
									" is engaged..."
								}), 2);
								phoneCall.EndCall();
								if (phoneCall.fromNS != null && phoneCall.callerNS != null && phoneCall.callerNS.isPlayer)
								{
									phoneCall.fromNS.engaged = AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.phoneLineEngaged, phoneCall.callerNS, phoneCall.callerNS.currentNode, phoneCall.callerNS.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
								}
							}
							else
							{
								if (phoneCall.toNS != null)
								{
									phoneCall.toNS.SetActiveCall(phoneCall);
								}
								if (phoneCall.fromNS != null)
								{
									phoneCall.fromNS.SetActiveCall(phoneCall);
								}
							}
						}
					}
					else if (phoneCall.state == TelephoneController.CallState.ringing)
					{
						if (phoneCall.ringDelay > 1f)
						{
							if (phoneCall.toNS != null)
							{
								bool flag = false;
								List<Human> list = new List<Human>();
								foreach (Actor actor in phoneCall.toNS.location.currentOccupants)
								{
									Human human = (Human)actor;
									if (!human.isDead && !(human.ai == null) && human.canListen && (!(phoneCall.toNS.location.thisAsAddress != null) || phoneCall.toNS.location.thisAsAddress.company == null || !phoneCall.toNS.location.thisAsAddress.company.publicFacing) && (human.locationsOfAuthority.Contains(phoneCall.toNS.location) || (human.isEnforcer && human.isOnDuty)))
									{
										if (human == phoneCall.intendedReceiverNS)
										{
											human.ai.AnswerPhone(phoneCall.toNS);
											flag = true;
											break;
										}
										if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.job != null && CasePanelController.Instance.activeCase.job.poster == human)
										{
											human.ai.AnswerPhone(phoneCall.toNS);
											flag = true;
											break;
										}
										list.Add(human);
									}
								}
								if (flag)
								{
									goto IL_3D3;
								}
								using (List<Human>.Enumerator enumerator2 = list.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										Human human2 = enumerator2.Current;
										human2.ai.AnswerPhone(phoneCall.toNS);
									}
									goto IL_3D3;
								}
							}
							phoneCall.SetCallState(TelephoneController.CallState.started);
						}
						else
						{
							phoneCall.ringDelay += Time.deltaTime;
						}
						IL_3D3:
						if (phoneCall.source.callType != TelephoneController.CallType.fakeOutbound)
						{
							phoneCall.ringTime -= num;
							if (phoneCall.ringTime <= 0f)
							{
								phoneCall.EndCall();
							}
						}
					}
					else if (phoneCall.state == TelephoneController.CallState.started && phoneCall.source.callType == TelephoneController.CallType.audioEvent)
					{
						PLAYBACK_STATE playback_STATE = 2;
						phoneCall.callAudioInstance.getPlaybackState(ref playback_STATE);
						if (playback_STATE == 2)
						{
							phoneCall.EndCall();
						}
					}
				}
			}
			else
			{
				base.enabled = false;
			}
			this.gameTimeLastLoop = SessionData.Instance.gameTime;
		}
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00091CD4 File Offset: 0x0008FED4
	[Button(null, 0)]
	public void FindTelephoneByNumber()
	{
		Telephone telephone = null;
		if (CityData.Instance.phoneDictionary.TryGetValue(this.debugNumber, ref telephone))
		{
			Game.Log(string.Concat(new string[]
			{
				"Found telephone at ",
				telephone.interactable.GetWorldPosition(true).ToString(),
				" (",
				telephone.interactable.node.room.GetName(),
				", ",
				telephone.interactable.node.gameLocation.name,
				")"
			}), 2);
			return;
		}
		Game.Log("Unable to find telephone of " + this.debugNumber.ToString(), 2);
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00091D98 File Offset: 0x0008FF98
	[Button(null, 0)]
	public void FindTelephonesAtPlayerLocation()
	{
		if (Player.Instance.currentGameLocation != null)
		{
			foreach (KeyValuePair<int, Telephone> keyValuePair in CityData.Instance.phoneDictionary)
			{
				if (keyValuePair.Value.interactable != null && keyValuePair.Value.interactable.node.gameLocation == Player.Instance.currentGameLocation)
				{
					Game.Log("Found telephone " + keyValuePair.Key.ToString() + " at " + keyValuePair.Value.interactable.GetWorldPosition(true).ToString(), 2);
				}
			}
		}
	}

	// Token: 0x04000967 RID: 2407
	[Header("Telecoms")]
	public List<TelephoneController.PhoneCall> activeCalls = new List<TelephoneController.PhoneCall>();

	// Token: 0x04000968 RID: 2408
	private float gameTimeLastLoop;

	// Token: 0x04000969 RID: 2409
	public Dictionary<Interactable, EventInstance> engagedEvents = new Dictionary<Interactable, EventInstance>();

	// Token: 0x0400096A RID: 2410
	public Dictionary<int, TelephoneController.CallSource> fakeTelephoneDictionary = new Dictionary<int, TelephoneController.CallSource>();

	// Token: 0x0400096B RID: 2411
	[Header("Debug")]
	public int debugNumber;

	// Token: 0x0400096D RID: 2413
	private static TelephoneController _instance;

	// Token: 0x02000162 RID: 354
	public enum CallState
	{
		// Token: 0x0400096F RID: 2415
		dialing,
		// Token: 0x04000970 RID: 2416
		denied,
		// Token: 0x04000971 RID: 2417
		ringing,
		// Token: 0x04000972 RID: 2418
		started,
		// Token: 0x04000973 RID: 2419
		ended
	}

	// Token: 0x02000163 RID: 355
	public enum CallType
	{
		// Token: 0x04000975 RID: 2421
		dds,
		// Token: 0x04000976 RID: 2422
		audioEvent,
		// Token: 0x04000977 RID: 2423
		player,
		// Token: 0x04000978 RID: 2424
		fakeOutbound
	}

	// Token: 0x02000164 RID: 356
	[Serializable]
	public class CallSource
	{
		// Token: 0x0600094F RID: 2383 RVA: 0x00091EA5 File Offset: 0x000900A5
		public CallSource(TelephoneController.CallType newType, string newDDS)
		{
			this.callType = newType;
			this.dds = newDDS;
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x00091EC2 File Offset: 0x000900C2
		public CallSource(TelephoneController.CallType newType, AudioEvent newAudioEvent)
		{
			this.callType = newType;
			this.audio = newAudioEvent.name;
			this.audioEvent = newAudioEvent;
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x00091EEC File Offset: 0x000900EC
		public CallSource(TelephoneController.CallType newType, DialogPreset newGreeting, InteractionController.ConversationType newConvoType = InteractionController.ConversationType.normal)
		{
			this.callType = newType;
			this.dialogGreeting = newGreeting;
			if (this.dialogGreeting == null)
			{
				this.dialogGreeting = CitizenControls.Instance.telephoneGreeting;
			}
			this.dialog = this.dialogGreeting.name;
			this.convoType = newConvoType;
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x00091F4C File Offset: 0x0009014C
		public CallSource(TelephoneController.CallType newType, DialogPreset newGreeting, SideJob newJob, InteractionController.ConversationType newConvoType = InteractionController.ConversationType.normal)
		{
			this.callType = newType;
			this.dialogGreeting = newGreeting;
			if (this.dialogGreeting == null)
			{
				this.dialogGreeting = CitizenControls.Instance.telephoneGreeting;
			}
			this.dialog = this.dialogGreeting.name;
			this.job = newJob.jobID;
			this.convoType = newConvoType;
		}

		// Token: 0x04000979 RID: 2425
		public TelephoneController.CallType callType;

		// Token: 0x0400097A RID: 2426
		public string dds;

		// Token: 0x0400097B RID: 2427
		public string audio;

		// Token: 0x0400097C RID: 2428
		public string dialog;

		// Token: 0x0400097D RID: 2429
		public int job = -1;

		// Token: 0x0400097E RID: 2430
		public InteractionController.ConversationType convoType;

		// Token: 0x0400097F RID: 2431
		[NonSerialized]
		public AudioEvent audioEvent;

		// Token: 0x04000980 RID: 2432
		[NonSerialized]
		public DialogPreset dialogGreeting;
	}

	// Token: 0x02000165 RID: 357
	[Serializable]
	public class PhoneCall
	{
		// Token: 0x06000953 RID: 2387 RVA: 0x00091FB8 File Offset: 0x000901B8
		public PhoneCall(Telephone newFrom, Telephone newTo, float newTime, Human newCaller, Human newIntendedReceiver, TelephoneController.CallSource newCallSource, float newMaxRingTime = 0.1f, bool newSpecificRecevier = false)
		{
			if (newFrom != null)
			{
				this.from = newFrom.number;
			}
			if (newTo != null)
			{
				this.to = newTo.number;
			}
			this.time = newTime;
			if (newCaller != null)
			{
				this.caller = newCaller.humanID;
			}
			if (newIntendedReceiver != null)
			{
				this.intendedReceiver = newIntendedReceiver.humanID;
			}
			this.source = newCallSource;
			this.ringTime = newMaxRingTime;
			this.specRecevier = newSpecificRecevier;
			this.fromNS = newFrom;
			this.toNS = newTo;
			this.callerNS = newCaller;
			this.intendedReceiverNS = newIntendedReceiver;
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00092088 File Offset: 0x00090288
		public void SetCallState(TelephoneController.CallState newState)
		{
			if (newState != this.state)
			{
				this.previousSate = this.state;
				this.state = newState;
				if (this.state == TelephoneController.CallState.ringing)
				{
					if (this.toNS != null)
					{
						this.toNS.interactable.SetSwitchState(true, null, true, false, false);
					}
					if (this.lineRingingLoop == null && this.callerNS != null && this.callerNS.isPlayer)
					{
						this.lineRingingLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.phoneLineRing, this.callerNS, this.callerNS.interactable, null, 1f, false, false, null, null);
						return;
					}
				}
				else
				{
					if (this.lineRingingLoop != null)
					{
						AudioController.Instance.StopSound(this.lineRingingLoop, AudioController.StopType.fade, "Phone stop ringing");
						this.lineRingingLoop = null;
					}
					if (this.toNS != null)
					{
						this.toNS.interactable.SetSwitchState(false, null, true, false, false);
					}
					if (this.state == TelephoneController.CallState.started)
					{
						if (this.recevierNS != null && this.recevierNS.isPlayer)
						{
							Game.Log("Phone: Set player active call: " + this.source.callType.ToString(), 2);
							Player.Instance.activeCall = this;
							if (TelephoneController.Instance.OnPlayerCall != null)
							{
								TelephoneController.Instance.OnPlayerCall();
							}
						}
						if (this.lineActiveLoopCaller == null && this.callerNS != null && this.callerNS.isPlayer)
						{
							this.lineActiveLoopCaller = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.phoneLineActive, this.callerNS, this.callerNS.interactable, null, 1f, false, false, null, null);
						}
						if (this.lineActiveLoopReceiver == null && this.recevierNS != null && this.recevierNS.isPlayer)
						{
							this.lineActiveLoopReceiver = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.phoneLineActive, this.recevierNS, this.recevierNS.interactable, null, 1f, false, false, null, null);
						}
						if (this.callerNS != null && this.recevierNS != null)
						{
							this.callerNS.UpdateLastSighting(this.recevierNS, true, 0);
							this.recevierNS.UpdateLastSighting(this.callerNS, true, 0);
						}
						if (this.specRecevier && this.intendedReceiverNS != null && this.intendedReceiverNS != this.recevierNS)
						{
							Game.Log("Not the right intended reciever, ending call...", 2);
							this.EndCall();
							return;
						}
						if (this.source.callType == TelephoneController.CallType.player || this.source.callType == TelephoneController.CallType.fakeOutbound)
						{
							if (this.source.dialogGreeting == null)
							{
								Toolbox.Instance.LoadDataFromResources<DialogPreset>(this.source.dialog, out this.source.dialogGreeting);
							}
							if (Player.Instance.phoneInteractable == null)
							{
								Game.Log("Player phone interactable is null!", 2);
							}
							if (this.recevierNS != null)
							{
								InteractionController.Instance.SetDialog(true, this.recevierNS.interactable, true, Player.Instance.phoneInteractable, this.source.convoType);
							}
							else if (Player.Instance.phoneInteractable != null)
							{
								InteractionController.Instance.SetDialog(true, Player.Instance.interactable, true, Player.Instance.phoneInteractable, this.source.convoType);
							}
							if (this.source.job < 0)
							{
								Interactable interactable = Player.Instance.phoneInteractable;
								if (this.recevierNS != null)
								{
									interactable = this.recevierNS.interactable;
								}
								try
								{
									if (interactable != null)
									{
										string text = "Player phone executing source greeting: ";
										DialogPreset dialogGreeting = this.source.dialogGreeting;
										Game.Log(text + ((dialogGreeting != null) ? dialogGreeting.ToString() : null) + " with receiver: " + interactable.GetName(), 2);
										DialogController.Instance.ExecuteDialog(new EvidenceWitness.DialogOption
										{
											preset = this.source.dialogGreeting
										}, interactable, Player.Instance.currentNode, Player.Instance, DialogController.ForceSuccess.none);
									}
									else
									{
										Game.LogError("Unable to execute telephone greeting", 2);
										this.EndCall();
									}
									return;
								}
								catch
								{
									Game.LogError("Unable to execute telephone greeting", 2);
									this.EndCall();
									return;
								}
							}
							SideJob sideJob = null;
							Interactable interactable2 = Player.Instance.interactable;
							if (this.recevierNS != null)
							{
								interactable2 = this.recevierNS.interactable;
							}
							if (this.source == null)
							{
								Game.LogError("Call source is null!", 2);
								this.EndCall();
								return;
							}
							if (this.source.dialogGreeting == null)
							{
								Game.LogError("Call source dialog greeting is null!", 2);
								this.EndCall();
								return;
							}
							if (interactable2 == null)
							{
								Game.LogError("Unable to execute telephone greeting", 2);
								this.EndCall();
								return;
							}
							if (!SideJobController.Instance.allJobsDictionary.TryGetValue(this.source.job, ref sideJob))
							{
								DialogController.Instance.ExecuteDialog(new EvidenceWitness.DialogOption
								{
									preset = this.source.dialogGreeting
								}, interactable2, Player.Instance.currentNode, Player.Instance, DialogController.ForceSuccess.none);
								return;
							}
							string text2 = "Jobs: Executing dialog from job ";
							JobPreset preset = sideJob.preset;
							string text3 = (preset != null) ? preset.ToString() : null;
							string text4 = ": ";
							DialogPreset dialogGreeting2 = this.source.dialogGreeting;
							Game.Log(text2 + text3 + text4 + ((dialogGreeting2 != null) ? dialogGreeting2.ToString() : null), 2);
							DialogController.Instance.ExecuteDialog(new EvidenceWitness.DialogOption
							{
								preset = this.source.dialogGreeting,
								jobRef = sideJob
							}, interactable2, Player.Instance.currentNode, Player.Instance, DialogController.ForceSuccess.none);
							return;
						}
						else
						{
							if (this.source.callType == TelephoneController.CallType.dds)
							{
								List<Human> list = new List<Human>();
								list.Add(this.recevierNS);
								this.callerNS.ExecuteConversationTree(Toolbox.Instance.allDDSTrees[this.source.dds], list);
								return;
							}
							if (this.source.callType == TelephoneController.CallType.audioEvent)
							{
								if (this.source.audioEvent == null)
								{
									Toolbox.Instance.LoadDataFromResources<AudioEvent>(this.source.audio, out this.source.audioEvent);
								}
								if (!(this.source.audioEvent != null))
								{
									Game.LogError("Cannot find audio event for telephone call: " + this.source.audio, 2);
									return;
								}
								if (this.recevierNS != null)
								{
									float volumeOverride = 1f;
									if (!this.recevierNS.isPlayer)
									{
										volumeOverride = 0.4f;
									}
									this.callAudioInstance = AudioController.Instance.PlayWorldOneShot(this.source.audioEvent, this.recevierNS, this.recevierNS.currentNode, this.recevierNS.lookAtThisTransform.position, null, null, volumeOverride, null, false, null, false);
									return;
								}
							}
						}
					}
					else if (this.state == TelephoneController.CallState.ended)
					{
						if (this.lineActiveLoopCaller != null)
						{
							AudioController.Instance.StopSound(this.lineActiveLoopCaller, AudioController.StopType.fade, "Phone call ended");
							this.lineActiveLoopCaller = null;
						}
						if (this.lineActiveLoopReceiver != null)
						{
							AudioController.Instance.StopSound(this.lineActiveLoopReceiver, AudioController.StopType.fade, "Phone call ended");
							this.lineActiveLoopReceiver = null;
						}
						if (this.previousSate == TelephoneController.CallState.started)
						{
							if (this.callerNS != null && this.callerNS == Player.Instance && this.fromNS.activeReceiver == Player.Instance)
							{
								this.hangUpCaller = AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.hangUp, this.callerNS, this.callerNS.currentNode, this.callerNS.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
								if (Player.Instance.phoneInteractable.sw1 && !Player.Instance.phoneInteractable.sw2)
								{
									this.fromNS.dialTone = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.dialTone, Player.Instance, Player.Instance.interactable, null, 1f, false, false, null, null);
								}
							}
							if (this.recevierNS != null && this.recevierNS == Player.Instance && this.toNS.activeReceiver == Player.Instance)
							{
								this.hangUpReciever = AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.hangUp, this.recevierNS, this.recevierNS.currentNode, this.recevierNS.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
								if (Player.Instance.phoneInteractable.sw1 && !Player.Instance.phoneInteractable.sw2)
								{
									this.toNS.dialTone = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.dialTone, Player.Instance, Player.Instance.interactable, null, 1f, false, false, null, null);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x0009296C File Offset: 0x00090B6C
		public void EndCall()
		{
			if (this.fromNS != null && this.toNS != null)
			{
				Game.Log("Phone: End telephone call from " + this.fromNS.numberString + " to " + this.toNS.numberString, 2);
			}
			AudioController.Instance.StopSound(this.connecting, AudioController.StopType.immediate);
			if (this.source.callType == TelephoneController.CallType.audioEvent)
			{
				this.callAudioInstance.stop(1);
				this.callAudioInstance.release();
			}
			if (this.toNS != null)
			{
				foreach (Actor actor in this.toNS.location.currentOccupants)
				{
					Human human = (Human)actor;
					if (!(human.ai == null) && human.ai.currentGoal != null)
					{
						foreach (NewAIAction newAIAction in human.ai.currentGoal.actions.FindAll((NewAIAction item) => item.preset == RoutineControls.Instance.answerTelephone))
						{
							newAIAction.Complete();
						}
					}
				}
			}
			if (this.callerNS != null && this.callerNS.ai != null)
			{
				NewAIAction currentAction = this.callerNS.ai.currentAction;
				if (currentAction != null && currentAction.preset == RoutineControls.Instance.makeCall)
				{
					currentAction.Complete();
				}
			}
			if (this.callerNS == Player.Instance || this.recevierNS == Player.Instance)
			{
				if (Player.Instance.interactingWith != null)
				{
					Actor actor2 = Player.Instance.interactingWith.objectRef as Actor;
					if (actor2 != null)
					{
						actor2.SetInteracting(null);
					}
				}
				Player.Instance.SetInteracting(null);
				if (Player.Instance.phoneInteractable != null)
				{
					Player.Instance.phoneInteractable.SetCustomState1(true, Player.Instance, true, false, false);
					Player.Instance.phoneInteractable.SetCustomState2(false, Player.Instance, true, false, false);
					Player.Instance.phoneInteractable.SetCustomState3(false, null, true, false, false);
				}
				InteractionController.Instance.SetDialog(false, null, false, null, InteractionController.ConversationType.normal);
			}
			TelephoneController.Instance.RemoveActiveCall(this);
			this.SetCallState(TelephoneController.CallState.ended);
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00092BFC File Offset: 0x00090DFC
		public void SetupNonSerializedData()
		{
			if (!CityData.Instance.phoneDictionary.TryGetValue(this.from, ref this.fromNS))
			{
				Game.LogError("Unable to get 'from' telephone: " + this.from.ToString(), 2);
			}
			if (!CityData.Instance.phoneDictionary.TryGetValue(this.to, ref this.toNS))
			{
				Game.LogError("Unable to get 'to' telephone: " + this.to.ToString(), 2);
			}
			CityData.Instance.GetHuman(this.caller, out this.callerNS, true);
			CityData.Instance.GetHuman(this.receiver, out this.recevierNS, true);
			CityData.Instance.GetHuman(this.intendedReceiver, out this.intendedReceiverNS, true);
		}

		// Token: 0x04000981 RID: 2433
		public int from;

		// Token: 0x04000982 RID: 2434
		public int to = -1;

		// Token: 0x04000983 RID: 2435
		public float time;

		// Token: 0x04000984 RID: 2436
		public float duration;

		// Token: 0x04000985 RID: 2437
		public int caller = -2;

		// Token: 0x04000986 RID: 2438
		public int receiver = -2;

		// Token: 0x04000987 RID: 2439
		public int intendedReceiver = -2;

		// Token: 0x04000988 RID: 2440
		public TelephoneController.CallSource source;

		// Token: 0x04000989 RID: 2441
		public TelephoneController.CallState previousSate = TelephoneController.CallState.denied;

		// Token: 0x0400098A RID: 2442
		public TelephoneController.CallState state;

		// Token: 0x0400098B RID: 2443
		public float ringTime = 0.1f;

		// Token: 0x0400098C RID: 2444
		public bool specRecevier;

		// Token: 0x0400098D RID: 2445
		public float dialingTimer;

		// Token: 0x0400098E RID: 2446
		public float ringDelay;

		// Token: 0x0400098F RID: 2447
		[NonSerialized]
		public Telephone fromNS;

		// Token: 0x04000990 RID: 2448
		[NonSerialized]
		public Telephone toNS;

		// Token: 0x04000991 RID: 2449
		[NonSerialized]
		public Human callerNS;

		// Token: 0x04000992 RID: 2450
		[NonSerialized]
		public Human recevierNS;

		// Token: 0x04000993 RID: 2451
		[NonSerialized]
		public Human intendedReceiverNS;

		// Token: 0x04000994 RID: 2452
		[NonSerialized]
		public AudioController.LoopingSoundInfo lineRingingLoop;

		// Token: 0x04000995 RID: 2453
		[NonSerialized]
		public AudioController.LoopingSoundInfo lineActiveLoopCaller;

		// Token: 0x04000996 RID: 2454
		[NonSerialized]
		public AudioController.LoopingSoundInfo lineActiveLoopReceiver;

		// Token: 0x04000997 RID: 2455
		[NonSerialized]
		public EventInstance callAudioInstance;

		// Token: 0x04000998 RID: 2456
		[NonSerialized]
		public EventInstance connecting;

		// Token: 0x04000999 RID: 2457
		[NonSerialized]
		public EventInstance hangUpCaller;

		// Token: 0x0400099A RID: 2458
		[NonSerialized]
		public EventInstance hangUpReciever;
	}

	// Token: 0x02000167 RID: 359
	// (Invoke) Token: 0x0600095B RID: 2395
	public delegate void PlayerCall();
}
