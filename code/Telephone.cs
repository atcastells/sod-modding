using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

// Token: 0x02000395 RID: 917
[Serializable]
public class Telephone
{
	// Token: 0x060014DB RID: 5339 RVA: 0x001312EC File Offset: 0x0012F4EC
	public Telephone(Interactable newTelephone)
	{
		this.interactable = newTelephone;
		this.location = this.interactable.node.gameLocation;
		if (!this.location.telephones.Contains(this))
		{
			this.location.telephones.Add(this);
		}
		this.locationEntry = this.interactable.node.gameLocation.evidenceEntry;
		this.GenerateTelephoneNumber();
		this.CreateEvidence();
		this.setup = true;
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x0013137C File Offset: 0x0012F57C
	public Telephone(Interactable newTelephone, int newNumber)
	{
		this.interactable = newTelephone;
		this.number = newNumber;
		this.location = this.interactable.node.gameLocation;
		if (!this.location.telephones.Contains(this))
		{
			this.location.telephones.Add(this);
		}
		this.locationEntry = this.interactable.node.gameLocation.evidenceEntry;
		this.LoadTelephoneNumber();
		this.CreateEvidence();
		this.setup = true;
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x00131410 File Offset: 0x0012F610
	public void LoadTelephoneNumber()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (!CityData.Instance.phoneDictionary.ContainsKey(this.number))
		{
			CityData.Instance.phoneDictionary.Add(this.number, this);
		}
		this.numberString = Toolbox.Instance.GetTelephoneNumberString(this.number);
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x00131470 File Offset: 0x0012F670
	public void GenerateTelephoneNumber()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		this.number = Toolbox.Instance.GetPsuedoRandomNumber(1000000, 6999000, this.location.GetReplicableSeed(), false);
		while (CityData.Instance.phoneDictionary.ContainsKey(this.number) || TelephoneController.Instance.fakeTelephoneDictionary.ContainsKey(this.number))
		{
			this.number++;
		}
		CityData.Instance.phoneDictionary.Add(this.number, this);
		this.numberString = Toolbox.Instance.GetTelephoneNumberString(this.number);
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x0013151C File Offset: 0x0012F71C
	public List<int> GetInputCode()
	{
		List<int> list = new List<int>();
		string text = this.number.ToString();
		for (int i = 0; i < text.Length; i++)
		{
			int num = 0;
			int.TryParse(text.get_Chars(i).ToString(), ref num);
			list.Add(num);
		}
		return list;
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x00131570 File Offset: 0x0012F770
	public void CreateEvidence()
	{
		List<object> list = new List<object>();
		list.Add(this);
		this.telephoneEntry = (EvidenceCreator.Instance.CreateEvidence("TelephoneNumber", "TelephoneNumber" + this.number.ToString(), this.location, null, null, null, this.location.evidenceEntry, false, list) as EvidenceTelephone);
		EvidenceCreator.Instance.CreateFact("IsTelephoneNumberFor", this.telephoneEntry, this.locationEntry, null, null, false, null, null, null, false);
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x001315F4 File Offset: 0x0012F7F4
	public void StopActiveCall()
	{
		if (this.activeCall.Count > 0)
		{
			TelephoneController.PhoneCall phoneCall = this.activeCall[0];
			this.activeCall[0].EndCall();
			if (phoneCall.fromNS != null)
			{
				phoneCall.fromNS.activeCall.Clear();
				if (phoneCall.fromNS.interactable != null)
				{
					phoneCall.fromNS.interactable.SetCustomState3(false, null, true, false, false);
				}
			}
			if (phoneCall.toNS != null)
			{
				phoneCall.toNS.activeCall.Clear();
				if (phoneCall.toNS.interactable != null)
				{
					phoneCall.toNS.interactable.SetCustomState3(false, null, true, false, false);
				}
			}
			this.interactable.SetCustomState3(false, null, true, false, false);
		}
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x001316B4 File Offset: 0x0012F8B4
	public void SetActiveCall(TelephoneController.PhoneCall newCall)
	{
		if (newCall == null || newCall.fromNS == null)
		{
			return;
		}
		if (this.activeCall.Count <= 0 || (this.activeCall.Count > 0 && this.activeCall[0] != newCall))
		{
			if (this.activeCall.Count > 0)
			{
				this.StopActiveCall();
			}
			Game.Log("Phone: Adding new active call: " + ((newCall != null) ? newCall.ToString() : null), 2);
			this.activeCall.Add(newCall);
			if (this.activeCall.Count > 0)
			{
				if (this.dialTone != null)
				{
					AudioController.Instance.StopSound(this.dialTone, AudioController.StopType.immediate, "call while picked up");
					this.dialTone = null;
				}
				this.interactable.SetCustomState3(true, null, true, false, false);
				if (this.activeReceiver != null && this.activeCall[0].toNS == this)
				{
					if (this.activeCall[0].toNS == this)
					{
						this.activeCall[0].recevierNS = this.activeReceiver;
						this.activeCall[0].receiver = this.activeReceiver.humanID;
					}
					this.activeCall[0].SetCallState(TelephoneController.CallState.started);
					return;
				}
				if (this.activeCall[0].source.callType == TelephoneController.CallType.fakeOutbound)
				{
					this.activeCall[0].SetCallState(TelephoneController.CallState.started);
					return;
				}
				this.activeCall[0].SetCallState(TelephoneController.CallState.ringing);
			}
		}
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x0013183C File Offset: 0x0012FA3C
	public void SetTelephoneAnswered(Human val)
	{
		if (this.activeReceiver != val)
		{
			if (this.activeReceiver != null && val != null)
			{
				return;
			}
			this.activeReceiver = val;
			this.interactable.usagePoint.TrySetUser(Interactable.UsePointSlot.defaultSlot, this.activeReceiver, "");
			if (this.activeReceiver == null)
			{
				Game.Log("Phone: Telephone " + this.numberString + " hung up.", 2);
				if (this.dialTone != null)
				{
					AudioController.Instance.StopSound(this.dialTone, AudioController.StopType.immediate, "phone hang up");
					this.dialTone = null;
				}
				this.StopActiveCall();
				AudioController.Instance.StopSound(this.engaged, AudioController.StopType.immediate);
				return;
			}
			string text = "Phone: Telephone ";
			string text2 = this.numberString;
			string text3 = " new active receiver: ";
			Human human = this.activeReceiver;
			Game.Log(text + text2 + text3 + ((human != null) ? human.ToString() : null), 2);
			if (this.activeCall.Count > 0)
			{
				if (this.activeCall[0].toNS == this)
				{
					this.activeCall[0].recevierNS = this.activeReceiver;
					this.activeCall[0].receiver = this.activeReceiver.humanID;
					this.activeCall[0].SetCallState(TelephoneController.CallState.started);
					return;
				}
				if (this.activeCall[0].fromNS == this && this.activeCall[0].toNS != null && this.activeCall[0].toNS.activeReceiver != null)
				{
					this.activeCall[0].SetCallState(TelephoneController.CallState.started);
					return;
				}
			}
			else
			{
				if (this.interactable != null && this.interactable.sw3)
				{
					this.interactable.SetCustomState3(false, null, false, false, false);
				}
				if (this.activeReceiver.isPlayer && this.dialTone == null)
				{
					Game.Log("Play dial tone...", 2);
					if (!Player.Instance.phoneInteractable.sw2)
					{
						this.dialTone = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.dialTone, this.activeReceiver, this.activeReceiver.interactable, null, 1f, false, false, null, null);
					}
				}
			}
		}
	}

	// Token: 0x040019A0 RID: 6560
	[Header("Serialized Components")]
	public int number;

	// Token: 0x040019A1 RID: 6561
	public string numberString;

	// Token: 0x040019A2 RID: 6562
	public List<TelephoneController.PhoneCall> activeCall = new List<TelephoneController.PhoneCall>();

	// Token: 0x040019A3 RID: 6563
	[Header("Non-Serialized Components")]
	[NonSerialized]
	public bool setup;

	// Token: 0x040019A4 RID: 6564
	[NonSerialized]
	public NewGameLocation location;

	// Token: 0x040019A5 RID: 6565
	[NonSerialized]
	public Interactable interactable;

	// Token: 0x040019A6 RID: 6566
	[NonSerialized]
	public SpeechController speechController;

	// Token: 0x040019A7 RID: 6567
	[NonSerialized]
	public Human activeReceiver;

	// Token: 0x040019A8 RID: 6568
	[NonSerialized]
	public AudioController.LoopingSoundInfo dialTone;

	// Token: 0x040019A9 RID: 6569
	[NonSerialized]
	public EventInstance engaged;

	// Token: 0x040019AA RID: 6570
	[NonSerialized]
	public EvidenceLocation locationEntry;

	// Token: 0x040019AB RID: 6571
	[NonSerialized]
	public EvidenceTelephone telephoneEntry;
}
