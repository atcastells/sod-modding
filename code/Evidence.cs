using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200062D RID: 1581
public class Evidence : CaseComponent
{
	// Token: 0x1400004F RID: 79
	// (add) Token: 0x0600230E RID: 8974 RVA: 0x001D5B60 File Offset: 0x001D3D60
	// (remove) Token: 0x0600230F RID: 8975 RVA: 0x001D5B98 File Offset: 0x001D3D98
	public event Evidence.OnDiscover OnDiscovered;

	// Token: 0x14000050 RID: 80
	// (add) Token: 0x06002310 RID: 8976 RVA: 0x001D5BD0 File Offset: 0x001D3DD0
	// (remove) Token: 0x06002311 RID: 8977 RVA: 0x001D5C08 File Offset: 0x001D3E08
	public event Evidence.NewParent OnNewParent;

	// Token: 0x14000051 RID: 81
	// (add) Token: 0x06002312 RID: 8978 RVA: 0x001D5C40 File Offset: 0x001D3E40
	// (remove) Token: 0x06002313 RID: 8979 RVA: 0x001D5C78 File Offset: 0x001D3E78
	public event Evidence.NewChild OnNewChild;

	// Token: 0x14000052 RID: 82
	// (add) Token: 0x06002314 RID: 8980 RVA: 0x001D5CB0 File Offset: 0x001D3EB0
	// (remove) Token: 0x06002315 RID: 8981 RVA: 0x001D5CE8 File Offset: 0x001D3EE8
	public event Evidence.RemChild OnRemoveChild;

	// Token: 0x14000053 RID: 83
	// (add) Token: 0x06002316 RID: 8982 RVA: 0x001D5D20 File Offset: 0x001D3F20
	// (remove) Token: 0x06002317 RID: 8983 RVA: 0x001D5D58 File Offset: 0x001D3F58
	public event Evidence.DiscoverChild OnDiscoverChild;

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x06002318 RID: 8984 RVA: 0x001D5D90 File Offset: 0x001D3F90
	// (remove) Token: 0x06002319 RID: 8985 RVA: 0x001D5DC8 File Offset: 0x001D3FC8
	public event Evidence.ConnectFact OnConnectFact;

	// Token: 0x14000055 RID: 85
	// (add) Token: 0x0600231A RID: 8986 RVA: 0x001D5E00 File Offset: 0x001D4000
	// (remove) Token: 0x0600231B RID: 8987 RVA: 0x001D5E38 File Offset: 0x001D4038
	public event Evidence.DiscoverConnectedFact OnDiscoverConnectedFact;

	// Token: 0x14000056 RID: 86
	// (add) Token: 0x0600231C RID: 8988 RVA: 0x001D5E70 File Offset: 0x001D4070
	// (remove) Token: 0x0600231D RID: 8989 RVA: 0x001D5EA8 File Offset: 0x001D40A8
	public event Evidence.DataKeyChange OnDataKeyChange;

	// Token: 0x14000057 RID: 87
	// (add) Token: 0x0600231E RID: 8990 RVA: 0x001D5EE0 File Offset: 0x001D40E0
	// (remove) Token: 0x0600231F RID: 8991 RVA: 0x001D5F18 File Offset: 0x001D4118
	public event Evidence.DiscoveryChanged OnDiscoveryChanged;

	// Token: 0x14000058 RID: 88
	// (add) Token: 0x06002320 RID: 8992 RVA: 0x001D5F50 File Offset: 0x001D4150
	// (remove) Token: 0x06002321 RID: 8993 RVA: 0x001D5F88 File Offset: 0x001D4188
	public event Evidence.MatchTypeAdded OnMatchTypeAdded;

	// Token: 0x14000059 RID: 89
	// (add) Token: 0x06002322 RID: 8994 RVA: 0x001D5FC0 File Offset: 0x001D41C0
	// (remove) Token: 0x06002323 RID: 8995 RVA: 0x001D5FF8 File Offset: 0x001D41F8
	public event Evidence.AnyPinnedChange OnAnyPinnedChange;

	// Token: 0x1400005A RID: 90
	// (add) Token: 0x06002324 RID: 8996 RVA: 0x001D6030 File Offset: 0x001D4230
	// (remove) Token: 0x06002325 RID: 8997 RVA: 0x001D6068 File Offset: 0x001D4268
	public event Evidence.NoteAdded OnNoteAdded;

	// Token: 0x06002326 RID: 8998 RVA: 0x001D60A0 File Offset: 0x001D42A0
	public Evidence(EvidencePreset newPreset, string newID, Controller newController, List<object> newPassedObjects)
	{
		this.preset = newPreset;
		this.evID = newID;
		if (newPassedObjects != null)
		{
			foreach (object obj in newPassedObjects)
			{
				Interactable interactable = obj as Interactable;
				if (interactable != null)
				{
					this.interactable = interactable;
					this.interactablePreset = interactable.preset;
				}
				InteractablePreset c = obj as InteractablePreset;
				if (c != null)
				{
					this.interactablePreset = c;
				}
				Interactable.Passed passed = obj as Interactable.Passed;
				if (passed != null)
				{
					if (passed.varType == Interactable.PassedVarType.ddsOverride)
					{
						this.SetOverrideDDS(passed.str);
					}
					else if (passed.varType == Interactable.PassedVarType.metaObjectID)
					{
						this.meta = CityData.Instance.FindMetaObject((int)passed.value);
						if (this.meta == null)
						{
							Game.Log(string.Concat(new string[]
							{
								"Unable to find meta object ",
								((int)passed.value).ToString(),
								" for evidence ",
								this.preset.name,
								" (",
								this.evID,
								")"
							}), 2);
						}
					}
				}
			}
		}
		if (!GameplayController.Instance.evidenceDictionary.ContainsKey(this.evID))
		{
			GameplayController.Instance.evidenceDictionary.Add(this.evID, this);
		}
		else
		{
			Game.LogError("Trying to create evidence with existing ID " + this.evID, 2);
		}
		if (this.preset.isSingleton)
		{
			GameplayController.Instance.singletonEvidence.Add(this);
		}
		this.controller = newController;
		base.SetNewIcon(this.preset.iconSpriteLarge);
		this.SetupKeyTies();
		foreach (MatchPreset newMatch in this.preset.matchTypes)
		{
			this.AddMatch(newMatch);
		}
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x001D630C File Offset: 0x001D450C
	public override string GenerateName()
	{
		if (this.writer != null && this.preset.useBelongsToInName && this.isFound)
		{
			return this.writer.GetCitizenName() + "'s " + Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
		}
		return base.GenerateName();
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x001D6374 File Offset: 0x001D4574
	public virtual void Compile()
	{
		this.AutoCreateFacts(false);
		if (!SessionData.Instance.isFloorEdit && !SessionData.Instance.isTestScene)
		{
			this.BuildDataSources();
		}
		foreach (EvidencePreset.FactLinkSetup factLinkSetup in this.preset.addFactLinks)
		{
			Human human = this.writer;
			if (factLinkSetup.subject == EvidencePreset.FactLinkSubject.receiver)
			{
				human = this.reciever;
			}
			if (human != null && human.factDictionary.ContainsKey(factLinkSetup.factDictionary))
			{
				this.AddFactLink(human.factDictionary[factLinkSetup.factDictionary], factLinkSetup.key, false);
			}
		}
		this.UpdateName();
		if (this.preset.discoverOnCreate || Game.Instance.discoverAllEvidence)
		{
			this.SetFound(true);
		}
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x001D6464 File Offset: 0x001D4664
	public override string GetIdentifier()
	{
		return this.evID;
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x001D646C File Offset: 0x001D466C
	public PinnedItemController GetPinned(List<Evidence.DataKey> inputKeys)
	{
		if (CasePanelController.Instance.activeCase == null)
		{
			return null;
		}
		inputKeys = this.GetTiedKeys(inputKeys);
		List<Case.CaseElement> list = CasePanelController.Instance.activeCase.caseElements.FindAll((Case.CaseElement item) => item.id == this.evID);
		HashSet<Evidence.DataKey> hashSet = new HashSet<Evidence.DataKey>();
		if (this.preset.useDataKeys)
		{
			foreach (Evidence.DataKey dataKey in inputKeys)
			{
				if (this.preset.IsKeyUnique(dataKey) && !hashSet.Contains(dataKey))
				{
					hashSet.Add(dataKey);
				}
			}
		}
		foreach (Evidence.DataKey dataKey2 in inputKeys)
		{
			foreach (Case.CaseElement caseElement in list)
			{
				if (!(caseElement.pinnedController == null) && caseElement.dk.Contains(dataKey2))
				{
					if (this.preset.useDataKeys && hashSet.Count > 0)
					{
						bool flag = false;
						foreach (Evidence.DataKey dataKey3 in hashSet)
						{
							if (caseElement.dk.Contains(dataKey3))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}
					}
					return caseElement.pinnedController;
				}
			}
		}
		return null;
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x001D5ADE File Offset: 0x001D3CDE
	public override string FoundAtName()
	{
		return string.Empty;
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x001D6630 File Offset: 0x001D4830
	private void SetupKeyTies()
	{
		List<Evidence.DataKey> validProfileKeys = this.preset.GetValidProfileKeys();
		if (this.preset.useDataKeys)
		{
			using (List<Evidence.DataKey>.Enumerator enumerator = validProfileKeys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Evidence.DataKey dataKey = enumerator.Current;
					this.keyTies.Add(dataKey, new List<Evidence.DataKey>());
					this.keyTies[dataKey].Add(dataKey);
				}
				return;
			}
		}
		this.keyTies.Add(Evidence.DataKey.name, new List<Evidence.DataKey>());
		this.keyTies[Evidence.DataKey.name].Add(Evidence.DataKey.name);
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x001D66D8 File Offset: 0x001D48D8
	public virtual void BuildDataSources()
	{
		this.writer != null;
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x001D66E8 File Offset: 0x001D48E8
	public void SetParent(Evidence newParent)
	{
		if (this.parent == newParent)
		{
			return;
		}
		if (this.parent != null)
		{
			this.parent.RemoveChild(this);
		}
		newParent.AddChild(this);
		this.parent = newParent;
		if (!this.isFound && this.parent != null && this.parent.isFound)
		{
			this.parent.UpdateDiscoveries();
		}
		if (this.OnNewParent != null)
		{
			this.OnNewParent();
		}
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x001D675C File Offset: 0x001D495C
	public void SetBelongsTo(Human newOwner)
	{
		this.belongsTo = newOwner;
		if (this.preset.itemOwner == EvidencePreset.BelongsToSetting.partner && this.belongsTo.partner != null)
		{
			this.belongsTo = this.belongsTo.partner;
			return;
		}
		if (this.preset.itemOwner == EvidencePreset.BelongsToSetting.boss && this.belongsTo.job != null && this.belongsTo.job.boss != null && this.belongsTo.job.boss.employee != null)
		{
			this.belongsTo = this.belongsTo.job.boss.employee;
			return;
		}
		if (this.preset.itemOwner == EvidencePreset.BelongsToSetting.paramour && this.belongsTo.paramour != null)
		{
			this.belongsTo = this.belongsTo.paramour;
			return;
		}
		if (this.preset.itemOwner == EvidencePreset.BelongsToSetting.doctor)
		{
			this.belongsTo = this.belongsTo.GetDoctor();
			return;
		}
		if (this.preset.itemOwner == EvidencePreset.BelongsToSetting.landlord)
		{
			this.belongsTo = this.belongsTo.GetLandlord();
		}
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x001D687C File Offset: 0x001D4A7C
	public void SetWriter(Human newWriter)
	{
		this.writer = newWriter;
		if (this.preset.itemWriter == EvidencePreset.BelongsToSetting.partner && this.writer.partner != null)
		{
			this.writer = this.writer.partner;
			return;
		}
		if (this.preset.itemWriter == EvidencePreset.BelongsToSetting.boss && this.writer.job != null && this.writer.job.boss != null && this.writer.job.boss.employee != null)
		{
			this.writer = this.writer.job.boss.employee;
			return;
		}
		if (this.preset.itemWriter == EvidencePreset.BelongsToSetting.paramour && this.writer.paramour != null)
		{
			this.writer = this.writer.paramour;
			return;
		}
		if (this.preset.itemWriter == EvidencePreset.BelongsToSetting.doctor)
		{
			this.writer = this.writer.GetDoctor();
			return;
		}
		if (this.preset.itemWriter == EvidencePreset.BelongsToSetting.landlord)
		{
			this.writer = this.writer.GetLandlord();
		}
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x001D699C File Offset: 0x001D4B9C
	public void SetReciever(Human newReciever)
	{
		this.reciever = newReciever;
		if (this.preset.itemReceiver == EvidencePreset.BelongsToSetting.partner && this.reciever.partner != null)
		{
			this.reciever = this.reciever.partner;
			return;
		}
		if (this.preset.itemReceiver == EvidencePreset.BelongsToSetting.boss && this.reciever.job != null && this.reciever.job.boss != null && this.reciever.job.boss.employee != null)
		{
			this.reciever = this.reciever.job.boss.employee;
			return;
		}
		if (this.preset.itemReceiver == EvidencePreset.BelongsToSetting.paramour && this.reciever.paramour != null)
		{
			this.reciever = this.reciever.paramour;
			return;
		}
		if (this.preset.itemReceiver == EvidencePreset.BelongsToSetting.doctor)
		{
			this.reciever = this.reciever.GetDoctor();
			return;
		}
		if (this.preset.itemReceiver == EvidencePreset.BelongsToSetting.landlord)
		{
			this.reciever = this.reciever.GetLandlord();
		}
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x001D6ABB File Offset: 0x001D4CBB
	public void SetOverrideDDS(string newTreeID)
	{
		if (newTreeID.Length > 0)
		{
			this.overrideDDS = newTreeID;
		}
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x001D6ACD File Offset: 0x001D4CCD
	private void AddChild(Evidence newEv)
	{
		if (!this.children.Contains(newEv))
		{
			this.children.Add(newEv);
			if (this.OnNewChild != null)
			{
				this.OnNewChild();
			}
			if (newEv.isFound)
			{
				this.OnChildEvidenceDiscovery();
			}
		}
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x001D6B0A File Offset: 0x001D4D0A
	private void RemoveChild(Evidence newEv)
	{
		if (this.children.Contains(newEv))
		{
			this.children.Remove(newEv);
			if (this.OnRemoveChild != null)
			{
				this.OnRemoveChild();
			}
		}
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x001D6B3A File Offset: 0x001D4D3A
	public void OnChildEvidenceDiscovery()
	{
		if (this.OnDiscoverChild != null)
		{
			this.OnDiscoverChild();
		}
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x001D6B50 File Offset: 0x001D4D50
	public void AddFactLink(Fact newFact, List<Evidence.DataKey> newKey, bool thisIsTheFromEvidence)
	{
		if (SessionData.Instance.startedGame)
		{
			Game.Log(string.Concat(new string[]
			{
				"Evidence: ",
				this.name,
				" Add fact link for ",
				newFact.name,
				" with ",
				newKey.Count.ToString(),
				" keys"
			}), 2);
		}
		foreach (Evidence.DataKey newKey2 in newKey)
		{
			this.AddFactLinkExe(newFact, newKey2, thisIsTheFromEvidence);
		}
		if (this.OnConnectFact != null)
		{
			this.OnConnectFact();
		}
		if (newFact.isFound)
		{
			if (SessionData.Instance.startedGame)
			{
				Game.Log("Evidence: " + newFact.preset.name + " is found: Trigger OnConnectedFactDiscovery", 2);
			}
			this.OnConnectedFactDiscovery(newFact);
			return;
		}
		if (SessionData.Instance.startedGame)
		{
			Game.Log("Evidence: " + newFact.preset.name + " is NOT found: Listen for fact's OnDiscoveredThis", 2);
		}
		newFact.OnDiscoveredThis += this.OnConnectedFactDiscovery;
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x001D6C90 File Offset: 0x001D4E90
	public void AddFactLink(Fact newFact, Evidence.DataKey newKey, bool thisIsTheFromEvidence)
	{
		if (SessionData.Instance.startedGame)
		{
			Game.Log("Match: AddFactLink " + newKey.ToString(), 2);
		}
		this.AddFactLinkExe(newFact, newKey, thisIsTheFromEvidence);
		if (this.OnConnectFact != null)
		{
			this.OnConnectFact();
		}
		if (newFact.isFound)
		{
			if (SessionData.Instance.startedGame)
			{
				Game.Log("Match: NewFact is found: Trigger OnConnectedFactDiscovery", 2);
			}
			this.OnConnectedFactDiscovery(newFact);
			return;
		}
		if (SessionData.Instance.startedGame)
		{
			Game.Log("Match: NewFact is NOT found: Listen for fact's OnDiscoveredThis", 2);
		}
		newFact.OnDiscoveredThis += this.OnConnectedFactDiscovery;
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x001D6D34 File Offset: 0x001D4F34
	private void AddFactLinkExe(Fact newFact, Evidence.DataKey newKey, bool thisIsTheFromEvidence)
	{
		if (!this.factDictionary.ContainsKey(newKey))
		{
			this.factDictionary.Add(newKey, new List<Evidence.FactLink>());
		}
		Evidence.FactLink factLink = new Evidence.FactLink();
		factLink.fact = newFact;
		factLink.thisEvidence = this;
		if (thisIsTheFromEvidence)
		{
			factLink.thisKeys = newFact.fromDataKeys;
			factLink.destinationEvidence = newFact.toEvidence;
			factLink.destinationKeys = newFact.toDataKeys;
		}
		else
		{
			factLink.thisKeys = newFact.toDataKeys;
			factLink.destinationEvidence = newFact.fromEvidence;
			factLink.destinationKeys = newFact.fromDataKeys;
		}
		factLink.thisIsTheFromEvidence = thisIsTheFromEvidence;
		this.factDictionary[newKey].Add(factLink);
		this.allFacts.Add(factLink);
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x001D6DE8 File Offset: 0x001D4FE8
	public void RemoveFactLink(Fact removeThis)
	{
		List<Evidence.FactLink> list = this.allFacts.FindAll((Evidence.FactLink item) => item.fact == removeThis);
		while (list.Count > 0)
		{
			this.allFacts.Remove(list[0]);
			list.RemoveAt(0);
		}
		foreach (KeyValuePair<Evidence.DataKey, List<Evidence.FactLink>> keyValuePair in this.factDictionary)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				if (keyValuePair.Value[i].fact == removeThis)
				{
					keyValuePair.Value.RemoveAt(i);
					i--;
				}
			}
		}
		if (this.OnConnectFact != null)
		{
			this.OnConnectFact();
		}
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x001D6EDC File Offset: 0x001D50DC
	public virtual void OnConnectedFactDiscovery(CaseComponent discovered)
	{
		if (this.OnDiscoverConnectedFact != null)
		{
			this.OnDiscoverConnectedFact();
		}
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x001D6EF1 File Offset: 0x001D50F1
	public void AddMatch(MatchPreset newMatch)
	{
		this.matches.Add(newMatch);
		GameplayController.Instance.AddNewMatch(newMatch, this);
		if (this.OnMatchTypeAdded != null)
		{
			this.OnMatchTypeAdded();
		}
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x001D6F20 File Offset: 0x001D5120
	public override void OnDiscovery()
	{
		foreach (EvidencePreset.MergeKeysSetup mergeKeysSetup in this.preset.keyMergeOnDiscovery)
		{
			Evidence linkForFact = this.GetLinkForFact(mergeKeysSetup.link);
			if (linkForFact != null)
			{
				foreach (Evidence.DataKey keyOne in mergeKeysSetup.mergeKeys)
				{
					foreach (Evidence.DataKey keyTwo in mergeKeysSetup.mergeKeys)
					{
						linkForFact.MergeDataKeys(keyOne, keyTwo);
					}
				}
			}
		}
		this.AutoCreateFacts(true);
		if (this.parent != null)
		{
			this.parent.OnChildEvidenceDiscovery();
		}
		foreach (EvidencePreset.DiscoveryApplication discoveryApplication in this.preset.applicationOnDiscover)
		{
			Evidence linkForFact2 = this.GetLinkForFact(discoveryApplication.link);
			if (linkForFact2 != null)
			{
				linkForFact2.AddDiscovery(discoveryApplication.applyDiscoveryTrigger);
			}
		}
		this.UpdateDiscoveries();
		foreach (EvidencePreset.FactLinkSetup factLinkSetup in this.preset.addFactLinks)
		{
			if (factLinkSetup.discovery)
			{
				Human human = this.writer;
				if (factLinkSetup.subject == EvidencePreset.FactLinkSubject.receiver)
				{
					human = this.reciever;
				}
				if (human != null && human.factDictionary.ContainsKey(factLinkSetup.factDictionary) && !human.factDictionary[factLinkSetup.factDictionary].isFound)
				{
					human.factDictionary[factLinkSetup.factDictionary].SetFound(true);
				}
			}
		}
		if (this.interactable != null)
		{
			this.interactable.UpdateName(true, Evidence.DataKey.photo);
		}
		if (this.OnDiscovered != null)
		{
			this.OnDiscovered(this);
		}
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x001D7170 File Offset: 0x001D5370
	public virtual void AutoCreateFacts(bool discovery)
	{
		foreach (EvidencePreset.EvidenceFactSetup evidenceFactSetup in this.preset.factSetup)
		{
			if ((!evidenceFactSetup.createOnDiscovery || discovery) && (evidenceFactSetup.createOnDiscovery || !discovery))
			{
				Evidence evidence = this;
				Evidence evidence2 = this.GetLinkForFact(evidenceFactSetup.link);
				if (evidenceFactSetup.switchFindingFactToFrom)
				{
					evidence = evidence2;
					evidence2 = this;
				}
				if (evidence != null && evidence2 != null && (!evidenceFactSetup.onlyIfInOwnedPosition || (this.interactable.subObject != null && this.interactable.subObject.belongsTo != FurniturePreset.SubObjectOwnership.nobody)))
				{
					EvidenceCreator.Instance.CreateFact(evidenceFactSetup.preset.name, evidence, evidence2, null, null, evidenceFactSetup.forceDiscoveryOnCreation, null, null, null, false);
				}
			}
		}
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x001D724C File Offset: 0x001D544C
	public Evidence GetLinkForFact(EvidencePreset.Subject subject)
	{
		Evidence result = null;
		if (subject == EvidencePreset.Subject.self)
		{
			result = this;
		}
		else if (subject == EvidencePreset.Subject.writer && this.writer != null)
		{
			result = this.writer.evidenceEntry;
		}
		else if (subject == EvidencePreset.Subject.receiver && this.reciever != null)
		{
			result = this.reciever.evidenceEntry;
		}
		else if (subject == EvidencePreset.Subject.parent && this.parent != null)
		{
			result = this.parent;
		}
		else if (subject == EvidencePreset.Subject.interactable && this.interactable != null && this.interactable.evidence != null)
		{
			result = this.interactable.evidence;
		}
		else if (subject == EvidencePreset.Subject.interactableLocation)
		{
			if (this.interactable != null)
			{
				if (this.interactable.node != null)
				{
					result = this.interactable.node.gameLocation.evidenceEntry;
				}
				else
				{
					Game.LogError(string.Concat(new string[]
					{
						"Unable to get current node for evidence ",
						this.evID,
						" interactable: ",
						this.interactable.id.ToString(),
						" ",
						this.interactable.GetName()
					}), 2);
				}
			}
			else
			{
				Game.LogError("Unable to get current location via interactable for evidence " + this.evID, 2);
			}
		}
		return result;
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x001D7390 File Offset: 0x001D5590
	public virtual void MergeDataKeys(Evidence.DataKey keyOne, Evidence.DataKey keyTwo)
	{
		if (keyOne == keyTwo)
		{
			return;
		}
		if (!this.preset.useDataKeys)
		{
			return;
		}
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		bool flag3 = false;
		bool flag4 = false;
		if (this.keyTies.ContainsKey(keyOne))
		{
			num = this.preset.GetProfileKeyCount(this.keyTies[keyOne]);
			foreach (Evidence.DataKey dataKey in this.keyTies[keyOne])
			{
				if (!list.Contains(dataKey))
				{
					list.Add(dataKey);
				}
				if (dataKey == Evidence.DataKey.photo)
				{
					flag = true;
				}
				else if (dataKey == Evidence.DataKey.name)
				{
					flag2 = true;
				}
			}
		}
		bool flag5 = false;
		bool flag6 = false;
		if (this.keyTies.ContainsKey(keyTwo))
		{
			num = Mathf.Max(num, this.preset.GetProfileKeyCount(this.keyTies[keyTwo]));
			foreach (Evidence.DataKey dataKey2 in this.keyTies[keyTwo])
			{
				if (!list.Contains(dataKey2))
				{
					list.Add(dataKey2);
					if (!flag4 && (this.preset.IsKeyUnique(dataKey2) || list.Exists((Evidence.DataKey item) => this.preset.IsKeyUnique(item))))
					{
						flag4 = true;
					}
				}
				if (dataKey2 == Evidence.DataKey.photo)
				{
					flag5 = true;
				}
				else if (dataKey2 == Evidence.DataKey.name)
				{
					flag6 = true;
				}
			}
		}
		foreach (Evidence.DataKey dataKey3 in list)
		{
			if (this.preset.IsKeyUnique(dataKey3))
			{
				this.keyTies[dataKey3] = list;
				flag3 = true;
			}
		}
		this.OnDataKeyMerge(keyOne, keyTwo);
		if (((flag && !flag2) || (!flag && flag2)) && ((flag5 && !flag6) || (!flag5 && flag6)) && ((flag && flag6) || (flag5 && flag2)))
		{
			this.NamePhotoMerge();
		}
		if (this.OnDataKeyChange != null)
		{
			this.OnDataKeyChange();
		}
		this.InstancingCheck();
		if (SessionData.Instance.startedGame && flag3)
		{
			List<Evidence.DataKey> list2 = this.GetTiedKeys(keyOne);
			int num2 = this.preset.GetProfileKeyCount(list2);
			List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(keyTwo);
			num2 = Mathf.Max(num2, this.preset.GetProfileKeyCount(tiedKeys));
			if (tiedKeys.Count > list2.Count)
			{
				list2 = tiedKeys;
			}
			if (this is EvidenceCitizen)
			{
				int count = this.preset.GetValidProfileKeys().Count;
				if (this.preset.notifyOfTies && flag4 && !InterfaceController.Instance.notificationQueue.Exists((InterfaceController.GameMessage item) => item.keyMerge && item.keyMergeEvidence == this) && num2 > num)
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("Citizen keys: " + num2.ToString() + "/" + count.ToString(), 2);
					}
					InterfaceController instance = InterfaceController.Instance;
					InterfaceController.GameMessageType newType = InterfaceController.GameMessageType.keyMerge;
					int newNumerical = num;
					string newMessage = string.Concat(new string[]
					{
						Strings.Get("ui.gamemessage", "Linked Unique Information", Strings.Casing.asIs, false, false, false, null),
						": ",
						this.GetNameForDataKey(list2),
						" (",
						num2.ToString(),
						"/",
						count.ToString(),
						")"
					});
					InterfaceControls.Icon newIcon = InterfaceControls.Icon.lookingGlass;
					AudioEvent additionalSFX = null;
					bool colourOverride = false;
					List<Evidence.DataKey> keyMergeKeys = list2;
					instance.NewGameMessage(newType, newNumerical, newMessage, newIcon, additionalSFX, colourOverride, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, this, keyMergeKeys, null);
					SessionData.Instance.TutorialTrigger("citizens", false);
				}
				if (num2 >= count && AchievementsController.Instance != null)
				{
					AchievementsController.Instance.UnlockAchievement("Nosy Parker", "complete_profile");
				}
			}
		}
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void NamePhotoMerge()
	{
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnDataKeyMerge(Evidence.DataKey keyOne, Evidence.DataKey keyTwo)
	{
	}

	// Token: 0x06002342 RID: 9026 RVA: 0x001D776C File Offset: 0x001D596C
	public List<Evidence.DataKey> GetTiedKeys(Evidence.DataKey inputKey)
	{
		List<Evidence.DataKey> inputKeys = Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[]
		{
			inputKey
		});
		return this.GetTiedKeys(inputKeys);
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x001D7790 File Offset: 0x001D5990
	public virtual List<Evidence.DataKey> GetTiedKeys(List<Evidence.DataKey> inputKeys)
	{
		if (!this.preset.useDataKeys)
		{
			return Toolbox.Instance.allDataKeys;
		}
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		if (this.keyTies == null)
		{
			Game.LogError(this.name + " key ties is null for " + this.name, 2);
			return list;
		}
		foreach (EvidencePreset.DataKeyAutomaticTies dataKeyAutomaticTies in this.preset.passiveTies)
		{
			if (inputKeys.Contains(dataKeyAutomaticTies.mainKey))
			{
				foreach (Evidence.DataKey dataKey in dataKeyAutomaticTies.mergeAtStart)
				{
					if (!list.Contains(dataKey))
					{
						list.Add(dataKey);
					}
				}
			}
		}
		for (int i = 0; i < inputKeys.Count; i++)
		{
			Evidence.DataKey dataKey2 = inputKeys[i];
			if (!list.Contains(dataKey2))
			{
				list.Add(dataKey2);
			}
			if (this.keyTies.ContainsKey(dataKey2))
			{
				foreach (Evidence.DataKey dataKey3 in this.keyTies[dataKey2])
				{
					if (!list.Contains(dataKey3))
					{
						list.Add(dataKey3);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x001D791C File Offset: 0x001D5B1C
	public List<Evidence.FactLink> GetFactsForDataKey(Evidence.DataKey inputKey)
	{
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		list.Add(inputKey);
		return this.GetFactsForDataKey(list);
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x001D7940 File Offset: 0x001D5B40
	public List<Evidence.FactLink> GetFactsForDataKey(List<Evidence.DataKey> inputKeys)
	{
		List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(inputKeys);
		List<Evidence.FactLink> list = new List<Evidence.FactLink>();
		if (inputKeys == null)
		{
			return list;
		}
		foreach (Evidence.DataKey dataKey in tiedKeys)
		{
			if (this.factDictionary.ContainsKey(dataKey))
			{
				foreach (Evidence.FactLink factLink in this.factDictionary[dataKey])
				{
					if (!list.Contains(factLink))
					{
						list.Add(factLink);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x001D7A00 File Offset: 0x001D5C00
	public string GetNameForDataKey(Evidence.DataKey inputKey)
	{
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		list.Add(inputKey);
		return this.GetNameForDataKey(list);
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x001D7A24 File Offset: 0x001D5C24
	public virtual string GetNameForDataKey(List<Evidence.DataKey> inputKeys)
	{
		string empty = string.Empty;
		string result;
		try
		{
			if (inputKeys == null)
			{
				result = Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(inputKeys);
				if (this.customNames.Count > 0)
				{
					foreach (Evidence.CustomName customName in this.customNames)
					{
						if (tiedKeys.Contains(customName.key))
						{
							return customName.name;
						}
					}
				}
				List<Evidence.DataKey> tiedKeys2 = this.GetTiedKeys(Evidence.DataKey.name);
				if (this.name != null && this.name.Length > 0 && (tiedKeys.Contains(Evidence.DataKey.name) || tiedKeys2.Exists((Evidence.DataKey item) => inputKeys.Contains(item))))
				{
					result = this.name;
				}
				else
				{
					result = Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
				}
			}
		}
		catch
		{
			result = empty;
		}
		return result;
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x001D7B60 File Offset: 0x001D5D60
	public void AddOrSetCustomName(Evidence.DataKey dk, string newCustomName)
	{
		Evidence.CustomName customName = this.customNames.Find((Evidence.CustomName item) => item.key == dk);
		if (customName == null)
		{
			customName = new Evidence.CustomName
			{
				key = dk
			};
			this.customNames.Add(customName);
		}
		customName.name = newCustomName;
		if (this.OnDataKeyChange != null)
		{
			this.OnDataKeyChange();
		}
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x001D7BD0 File Offset: 0x001D5DD0
	public void AddOrSetCustomName(List<Evidence.DataKey> dk, string newCustomName)
	{
		using (List<Evidence.DataKey>.Enumerator enumerator = dk.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Evidence.DataKey d = enumerator.Current;
				Evidence.CustomName customName = this.customNames.Find((Evidence.CustomName item) => item.key == d);
				if (customName == null)
				{
					customName = new Evidence.CustomName
					{
						key = d
					};
					this.customNames.Add(customName);
				}
				customName.name = newCustomName;
			}
		}
		if (this.OnDataKeyChange != null)
		{
			this.OnDataKeyChange();
		}
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x001D7C78 File Offset: 0x001D5E78
	public void AddDiscovery(Evidence.Discovery disc)
	{
		if (!this.discoveryProgress.Contains(disc))
		{
			this.discoveryProgress.Add(disc);
		}
		if (SessionData.Instance.startedGame)
		{
			Game.Log("Evidence: Add discovery " + disc.ToString() + " to " + this.name, 2);
		}
		this.UpdateDiscoveries();
		if (this.OnDiscoveryChanged != null)
		{
			this.OnDiscoveryChanged(disc);
		}
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x001D7CF0 File Offset: 0x001D5EF0
	public virtual void UpdateDiscoveries()
	{
		for (int i = 0; i < this.children.Count; i++)
		{
			Evidence evidence = this.children[i];
			if (!evidence.isFound)
			{
				foreach (Evidence.Discovery discovery in evidence.discoveryProgress)
				{
					if (this.preset.discoveryTriggers.Contains(discovery))
					{
						evidence.SetFound(true);
					}
				}
			}
		}
		for (int j = 0; j < this.allFacts.Count; j++)
		{
			Evidence.FactLink factLink = this.allFacts[j];
			if (!factLink.fact.isFound)
			{
				foreach (Evidence.Discovery discovery2 in this.discoveryProgress)
				{
					if (factLink.fact.preset.discoveryTriggers.Contains(discovery2))
					{
						factLink.fact.SetFound(true);
					}
				}
			}
		}
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x001D7E20 File Offset: 0x001D6020
	public virtual Sprite GetIcon()
	{
		if (this.interactablePreset != null)
		{
			return this.interactablePreset.iconOverride;
		}
		return this.preset.iconSpriteLarge;
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x001D7E48 File Offset: 0x001D6048
	public Texture2D GetPhoto(List<Evidence.DataKey> keys)
	{
		if (this.preset.useInGamePhoto)
		{
			if (!SessionData.Instance.startedGame)
			{
				goto IL_1E4;
			}
			try
			{
				if (this.preset.useWriter && this.writer != null)
				{
					return SceneCapture.Instance.CapturePhotoOfEvidence(this.writer.evidenceEntry, false);
				}
				if (keys.Contains(Evidence.DataKey.photo))
				{
					return SceneCapture.Instance.CapturePhotoOfEvidence(this, false);
				}
				if (this.interactablePreset != null && this.interactablePreset.staticImage != null)
				{
					return this.interactablePreset.staticImage.texture;
				}
				goto IL_1E4;
			}
			catch
			{
				Game.LogError("Unable to capture photo for evidence " + this.name, 2);
				goto IL_1E4;
			}
		}
		if (this.preset.useSurveillanceCapture)
		{
			if (SessionData.Instance.startedGame)
			{
				EvidenceSurveillance evidenceSurveillance = this as EvidenceSurveillance;
				if (evidenceSurveillance != null)
				{
					return SceneCapture.Instance.GetSurveillanceScene(evidenceSurveillance.savedCapture, true);
				}
			}
		}
		else
		{
			EvidenceFingerprint evidenceFingerprint = this as EvidenceFingerprint;
			if (evidenceFingerprint != null)
			{
				if (evidenceFingerprint.writer != null)
				{
					return CitizenControls.Instance.prints[Toolbox.Instance.GetPsuedoRandomNumber(0, CitizenControls.Instance.prints.Count, evidenceFingerprint.writer.citizenName + evidenceFingerprint.writer.humanID.ToString(), false)];
				}
				Game.LogError("Missing writer for fingerprint " + this.evID, 2);
				if (this.interactablePreset != null && this.interactablePreset.staticImage != null)
				{
					return this.interactablePreset.staticImage.texture;
				}
			}
			else if (this.interactablePreset != null && this.interactablePreset.staticImage != null)
			{
				return this.interactablePreset.staticImage.texture;
			}
		}
		IL_1E4:
		return this.preset.defaultNullImage;
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x001D8058 File Offset: 0x001D6258
	public virtual string GetSummary(List<Evidence.DataKey> keys)
	{
		if (this.interactablePreset != null && this.interactablePreset.summaryMessageSource != null && this.interactablePreset.summaryMessageSource.Length > 0)
		{
			return Strings.GetTextForComponent(this.interactablePreset.summaryMessageSource, null, null, null, "\n", false, null, Strings.LinkSetting.automatic, keys);
		}
		return "<Summary Missing>";
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x001D80B8 File Offset: 0x001D62B8
	public void SetNote(List<Evidence.DataKey> keys, string str)
	{
		if (!this.preset.useDataKeys)
		{
			if (!this.notes.ContainsKey(Evidence.DataKey.name))
			{
				this.notes.Add(Evidence.DataKey.name, string.Empty);
			}
			this.notes[Evidence.DataKey.name] = str;
		}
		else
		{
			foreach (Evidence.DataKey dataKey in this.GetTiedKeys(keys))
			{
				if (!this.notes.ContainsKey(dataKey))
				{
					this.notes.Add(dataKey, string.Empty);
				}
				this.notes[dataKey] = str;
			}
		}
		if (this.OnNoteAdded != null)
		{
			this.OnNoteAdded();
		}
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x001D8180 File Offset: 0x001D6380
	public virtual string GetNote(List<Evidence.DataKey> keys)
	{
		string text = string.Empty;
		List<string> list = new List<string>();
		bool flag = false;
		if (!this.preset.useDataKeys)
		{
			if (this.notes.ContainsKey(Evidence.DataKey.name))
			{
				string text2 = this.notes[Evidence.DataKey.name];
				if (!list.Contains(text2))
				{
					list.Add(text2);
					if (flag)
					{
						text += " ";
					}
					text += text2;
				}
			}
		}
		else
		{
			foreach (Evidence.DataKey dataKey in this.GetTiedKeys(keys))
			{
				if (this.notes.ContainsKey(dataKey))
				{
					string text3 = this.notes[dataKey];
					if (!list.Contains(text3))
					{
						list.Add(text3);
						if (flag)
						{
							text += " ";
						}
						text += text3;
						flag = true;
					}
				}
			}
		}
		return text;
	}

	// Token: 0x06002351 RID: 9041 RVA: 0x001D8284 File Offset: 0x001D6484
	public virtual string GetNoteComposed(List<Evidence.DataKey> keys, bool useLinks = true)
	{
		Strings.LinkSetting linkSetting = Strings.LinkSetting.forceNoLinks;
		if (useLinks)
		{
			linkSetting = Strings.LinkSetting.forceLinks;
		}
		return Strings.ComposeText(this.GetNote(keys), this.interactable, linkSetting, keys, null, false);
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x001D82AE File Offset: 0x001D64AE
	public void OnPinnedChange()
	{
		if (this.OnAnyPinnedChange != null)
		{
			this.OnAnyPinnedChange();
		}
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnPlayerLookedAtWithinReadingRange()
	{
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x001D82C3 File Offset: 0x001D64C3
	public void SetImageOverride(Sprite newSprite)
	{
		this.imageOverride = newSprite;
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x001D82CC File Offset: 0x001D64CC
	public void InstancingCheck()
	{
		List<InfoWindow> list = InterfaceController.Instance.activeWindows.FindAll((InfoWindow item) => item.passedEvidence == this);
		if (list.Count > 1)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i].UpdateEvidenceKeys();
			}
			Dictionary<Evidence.DataKey, List<InfoWindow>> dictionary = new Dictionary<Evidence.DataKey, List<InfoWindow>>();
			for (int j = 0; j < list.Count; j++)
			{
				InfoWindow infoWindow = list[j];
				for (int k = 0; k < list.Count; k++)
				{
					InfoWindow infoWindow2 = list[k];
					if (!(infoWindow == infoWindow2))
					{
						for (int l = 0; l < infoWindow.evidenceKeys.Count; l++)
						{
							Evidence.DataKey dataKey = infoWindow.evidenceKeys[l];
							if (this.preset.IsKeyUnique(dataKey) && infoWindow2.evidenceKeys.Contains(dataKey))
							{
								if (!dictionary.ContainsKey(dataKey))
								{
									dictionary.Add(dataKey, new List<InfoWindow>());
								}
								if (!dictionary[dataKey].Contains(infoWindow))
								{
									dictionary[dataKey].Add(infoWindow);
								}
								if (!dictionary[dataKey].Contains(infoWindow2))
								{
									dictionary[dataKey].Add(infoWindow2);
								}
							}
						}
					}
				}
			}
			if (dictionary.Count > 0)
			{
				Game.Log("Duplicate case windows found: " + dictionary.Count.ToString(), 2);
			}
			foreach (KeyValuePair<Evidence.DataKey, List<InfoWindow>> keyValuePair in dictionary)
			{
				Evidence.DataKey key = keyValuePair.Key;
				InfoWindow infoWindow3 = null;
				List<InfoWindow> list2 = new List<InfoWindow>();
				for (int m = 0; m < keyValuePair.Value.Count; m++)
				{
					InfoWindow infoWindow4 = keyValuePair.Value[m];
					if (!(infoWindow4 == null))
					{
						if (infoWindow3 == null)
						{
							infoWindow3 = infoWindow4;
						}
						else
						{
							list2.Add(infoWindow4);
						}
					}
				}
				for (int n = 0; n < list2.Count; n++)
				{
					list2[n].CloseWindow(false);
				}
				if (infoWindow3 != null)
				{
					infoWindow3.InstanceUpdateComplete();
				}
			}
		}
		if (!SessionData.Instance.isFloorEdit)
		{
			int num = CasePanelController.Instance.activeCases.Count + CasePanelController.Instance.archivedCases.Count;
			for (int num2 = 0; num2 < num; num2++)
			{
				Case @case = null;
				if (num2 < CasePanelController.Instance.activeCases.Count)
				{
					@case = CasePanelController.Instance.activeCases[num2];
				}
				else
				{
					@case = CasePanelController.Instance.archivedCases[num2 - CasePanelController.Instance.activeCases.Count];
				}
				List<Case.CaseElement> list3 = @case.caseElements.FindAll((Case.CaseElement item) => item.id == this.evID);
				if (list3.Count > 1)
				{
					foreach (Case.CaseElement caseElement in list3)
					{
						caseElement.dk = this.GetTiedKeys(caseElement.dk);
					}
					Dictionary<Evidence.DataKey, List<Case.CaseElement>> dictionary2 = new Dictionary<Evidence.DataKey, List<Case.CaseElement>>();
					for (int num3 = 0; num3 < list3.Count; num3++)
					{
						Case.CaseElement caseElement2 = list3[num3];
						for (int num4 = 0; num4 < list3.Count; num4++)
						{
							Case.CaseElement caseElement3 = list3[num4];
							if (caseElement2 != caseElement3)
							{
								for (int num5 = 0; num5 < caseElement2.dk.Count; num5++)
								{
									Evidence.DataKey dataKey2 = caseElement2.dk[num5];
									if (this.preset.IsKeyUnique(dataKey2) && caseElement3.dk.Contains(dataKey2))
									{
										if (!dictionary2.ContainsKey(dataKey2))
										{
											dictionary2.Add(dataKey2, new List<Case.CaseElement>());
										}
										if (!dictionary2[dataKey2].Contains(caseElement2))
										{
											dictionary2[dataKey2].Add(caseElement2);
										}
										if (!dictionary2[dataKey2].Contains(caseElement3))
										{
											dictionary2[dataKey2].Add(caseElement3);
										}
									}
								}
							}
						}
					}
					if (dictionary2.Count > 0)
					{
						Game.Log("Duplicate case elements found: " + dictionary2.Count.ToString(), 2);
					}
					foreach (KeyValuePair<Evidence.DataKey, List<Case.CaseElement>> keyValuePair2 in dictionary2)
					{
						Evidence.DataKey key2 = keyValuePair2.Key;
						Case.CaseElement caseElement4 = null;
						List<Case.CaseElement> list4 = new List<Case.CaseElement>();
						for (int num6 = 0; num6 < keyValuePair2.Value.Count; num6++)
						{
							Case.CaseElement caseElement5 = keyValuePair2.Value[num6];
							if (caseElement5 != null)
							{
								if (caseElement4 == null)
								{
									caseElement4 = caseElement5;
								}
								else
								{
									list4.Add(caseElement5);
								}
							}
						}
						for (int num7 = 0; num7 < list4.Count; num7++)
						{
							@case.caseElements.Remove(list4[num7]);
						}
					}
					if (@case == CasePanelController.Instance.activeCase)
					{
						CasePanelController.Instance.UpdatePinned();
						foreach (InfoWindow infoWindow5 in list)
						{
							infoWindow5.PinnedUpdateCheck();
						}
					}
				}
			}
		}
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x001D8870 File Offset: 0x001D6A70
	public void SetForceSave(bool val)
	{
		this.forceSave = val;
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x001D887C File Offset: 0x001D6A7C
	public List<Evidence.DataKey> GetMergedDiscoveryLinkKeysFor(Evidence linkEvidence, Evidence.DataKey mustFeature)
	{
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		if (linkEvidence == null)
		{
			return list;
		}
		foreach (EvidencePreset.MergeKeysSetup mergeKeysSetup in this.preset.keyMergeOnDiscovery)
		{
			Evidence linkForFact = this.GetLinkForFact(mergeKeysSetup.link);
			if (linkEvidence == linkForFact)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log(string.Concat(new string[]
					{
						"DDS: Link evidence ",
						linkEvidence.name,
						" matches (",
						mergeKeysSetup.link.ToString(),
						")"
					}), 2);
				}
				if (mergeKeysSetup.mergeKeys.Contains(mustFeature))
				{
					list.AddRange(mergeKeysSetup.mergeKeys);
				}
			}
		}
		if (!list.Contains(mustFeature))
		{
			list.Add(mustFeature);
		}
		return list;
	}

	// Token: 0x04002D47 RID: 11591
	public string evID;

	// Token: 0x04002D48 RID: 11592
	public bool forceSave;

	// Token: 0x04002D49 RID: 11593
	[NonSerialized]
	public Interactable interactable;

	// Token: 0x04002D4A RID: 11594
	public InteractablePreset interactablePreset;

	// Token: 0x04002D4B RID: 11595
	public EvidencePreset preset;

	// Token: 0x04002D4C RID: 11596
	public Sprite imageOverride;

	// Token: 0x04002D4D RID: 11597
	[NonSerialized]
	public Evidence parent;

	// Token: 0x04002D4E RID: 11598
	public Human writer;

	// Token: 0x04002D4F RID: 11599
	public Human reciever;

	// Token: 0x04002D50 RID: 11600
	public Human belongsTo;

	// Token: 0x04002D51 RID: 11601
	public string overrideDDS;

	// Token: 0x04002D52 RID: 11602
	public Controller controller;

	// Token: 0x04002D53 RID: 11603
	public MetaObject meta;

	// Token: 0x04002D54 RID: 11604
	[NonSerialized]
	public List<Evidence> children = new List<Evidence>();

	// Token: 0x04002D55 RID: 11605
	public Dictionary<Evidence.DataKey, List<Evidence.DataKey>> keyTies = new Dictionary<Evidence.DataKey, List<Evidence.DataKey>>();

	// Token: 0x04002D56 RID: 11606
	public List<Evidence.CustomName> customNames = new List<Evidence.CustomName>();

	// Token: 0x04002D57 RID: 11607
	public Dictionary<Evidence.DataKey, List<Evidence.FactLink>> factDictionary = new Dictionary<Evidence.DataKey, List<Evidence.FactLink>>();

	// Token: 0x04002D58 RID: 11608
	[NonSerialized]
	public List<Evidence.FactLink> allFacts = new List<Evidence.FactLink>();

	// Token: 0x04002D59 RID: 11609
	public List<Evidence.Discovery> discoveryProgress = new List<Evidence.Discovery>();

	// Token: 0x04002D5A RID: 11610
	public List<MatchPreset> matches = new List<MatchPreset>();

	// Token: 0x04002D5B RID: 11611
	public Dictionary<Evidence.DataKey, string> notes = new Dictionary<Evidence.DataKey, string>();

	// Token: 0x04002D68 RID: 11624
	private Action OrderCheck;

	// Token: 0x0200062E RID: 1582
	public enum Discovery
	{
		// Token: 0x04002D6A RID: 11626
		livesAt,
		// Token: 0x04002D6B RID: 11627
		partnerDiscovery,
		// Token: 0x04002D6C RID: 11628
		jobDiscovery,
		// Token: 0x04002D6D RID: 11629
		purchasedAt,
		// Token: 0x04002D6E RID: 11630
		phoneLocation,
		// Token: 0x04002D6F RID: 11631
		paramourDiscovery,
		// Token: 0x04002D70 RID: 11632
		phonePersonal,
		// Token: 0x04002D71 RID: 11633
		foundAt,
		// Token: 0x04002D72 RID: 11634
		foundOn,
		// Token: 0x04002D73 RID: 11635
		addressBookDiscovery,
		// Token: 0x04002D74 RID: 11636
		relationshipDiscovery,
		// Token: 0x04002D75 RID: 11637
		jobHours,
		// Token: 0x04002D76 RID: 11638
		diaryDiscovery,
		// Token: 0x04002D77 RID: 11639
		jobDiscoveryPhoto,
		// Token: 0x04002D78 RID: 11640
		dateOfBirth,
		// Token: 0x04002D79 RID: 11641
		timeOfDeath,
		// Token: 0x04002D7A RID: 11642
		referenceDiscovery,
		// Token: 0x04002D7B RID: 11643
		postedByDiscovery
	}

	// Token: 0x0200062F RID: 1583
	public enum DataKey
	{
		// Token: 0x04002D7D RID: 11645
		name,
		// Token: 0x04002D7E RID: 11646
		photo,
		// Token: 0x04002D7F RID: 11647
		fingerprints,
		// Token: 0x04002D80 RID: 11648
		code,
		// Token: 0x04002D81 RID: 11649
		voice,
		// Token: 0x04002D82 RID: 11650
		height,
		// Token: 0x04002D83 RID: 11651
		build,
		// Token: 0x04002D84 RID: 11652
		age,
		// Token: 0x04002D85 RID: 11653
		sex,
		// Token: 0x04002D86 RID: 11654
		hair,
		// Token: 0x04002D87 RID: 11655
		eyes,
		// Token: 0x04002D88 RID: 11656
		bloodType,
		// Token: 0x04002D89 RID: 11657
		shoeSize,
		// Token: 0x04002D8A RID: 11658
		facialHair,
		// Token: 0x04002D8B RID: 11659
		address,
		// Token: 0x04002D8C RID: 11660
		work,
		// Token: 0x04002D8D RID: 11661
		workHours,
		// Token: 0x04002D8E RID: 11662
		jobTitle,
		// Token: 0x04002D8F RID: 11663
		shoeSizeEstimate,
		// Token: 0x04002D90 RID: 11664
		glasses,
		// Token: 0x04002D91 RID: 11665
		dateOfBirth,
		// Token: 0x04002D92 RID: 11666
		salary,
		// Token: 0x04002D93 RID: 11667
		randomInterest,
		// Token: 0x04002D94 RID: 11668
		randomSocialClub,
		// Token: 0x04002D95 RID: 11669
		ageGroup,
		// Token: 0x04002D96 RID: 11670
		firstNameInitial,
		// Token: 0x04002D97 RID: 11671
		partnerFirstName,
		// Token: 0x04002D98 RID: 11672
		partnerJobTitle,
		// Token: 0x04002D99 RID: 11673
		partnerSocialClub,
		// Token: 0x04002D9A RID: 11674
		randomAffliction,
		// Token: 0x04002D9B RID: 11675
		heightEstimate,
		// Token: 0x04002D9C RID: 11676
		handwriting,
		// Token: 0x04002D9D RID: 11677
		livesOnFloor,
		// Token: 0x04002D9E RID: 11678
		telephoneNumber,
		// Token: 0x04002D9F RID: 11679
		livesInBuilding,
		// Token: 0x04002DA0 RID: 11680
		worksInBuilding,
		// Token: 0x04002DA1 RID: 11681
		location,
		// Token: 0x04002DA2 RID: 11682
		blueprints,
		// Token: 0x04002DA3 RID: 11683
		firstName,
		// Token: 0x04002DA4 RID: 11684
		surname,
		// Token: 0x04002DA5 RID: 11685
		initialedName,
		// Token: 0x04002DA6 RID: 11686
		initials,
		// Token: 0x04002DA7 RID: 11687
		purpose
	}

	// Token: 0x02000630 RID: 1584
	[Serializable]
	public class FactLink
	{
		// Token: 0x04002DA8 RID: 11688
		public Fact fact;

		// Token: 0x04002DA9 RID: 11689
		public Evidence thisEvidence;

		// Token: 0x04002DAA RID: 11690
		public List<Evidence.DataKey> thisKeys;

		// Token: 0x04002DAB RID: 11691
		public List<Evidence> destinationEvidence;

		// Token: 0x04002DAC RID: 11692
		public List<Evidence.DataKey> destinationKeys;

		// Token: 0x04002DAD RID: 11693
		public bool thisIsTheFromEvidence;
	}

	// Token: 0x02000631 RID: 1585
	[Serializable]
	public class CustomName
	{
		// Token: 0x04002DAE RID: 11694
		public Evidence.DataKey key;

		// Token: 0x04002DAF RID: 11695
		public string name;
	}

	// Token: 0x02000632 RID: 1586
	// (Invoke) Token: 0x06002360 RID: 9056
	public delegate void OnDiscover(Evidence disc);

	// Token: 0x02000633 RID: 1587
	// (Invoke) Token: 0x06002364 RID: 9060
	public delegate void NewParent();

	// Token: 0x02000634 RID: 1588
	// (Invoke) Token: 0x06002368 RID: 9064
	public delegate void NewChild();

	// Token: 0x02000635 RID: 1589
	// (Invoke) Token: 0x0600236C RID: 9068
	public delegate void RemChild();

	// Token: 0x02000636 RID: 1590
	// (Invoke) Token: 0x06002370 RID: 9072
	public delegate void DiscoverChild();

	// Token: 0x02000637 RID: 1591
	// (Invoke) Token: 0x06002374 RID: 9076
	public delegate void ConnectFact();

	// Token: 0x02000638 RID: 1592
	// (Invoke) Token: 0x06002378 RID: 9080
	public delegate void DiscoverConnectedFact();

	// Token: 0x02000639 RID: 1593
	// (Invoke) Token: 0x0600237C RID: 9084
	public delegate void DataKeyChange();

	// Token: 0x0200063A RID: 1594
	// (Invoke) Token: 0x06002380 RID: 9088
	public delegate void DiscoveryChanged(Evidence.Discovery newDisc);

	// Token: 0x0200063B RID: 1595
	// (Invoke) Token: 0x06002384 RID: 9092
	public delegate void MatchTypeAdded();

	// Token: 0x0200063C RID: 1596
	// (Invoke) Token: 0x06002388 RID: 9096
	public delegate void AnyPinnedChange();

	// Token: 0x0200063D RID: 1597
	// (Invoke) Token: 0x0600238C RID: 9100
	public delegate void NoteAdded();
}
