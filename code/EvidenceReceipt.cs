using System;
using System.Collections.Generic;

// Token: 0x02000662 RID: 1634
public class EvidenceReceipt : Evidence
{
	// Token: 0x06002409 RID: 9225 RVA: 0x001DC3E4 File Offset: 0x001DA5E4
	public EvidenceReceipt(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		for (int i = 0; i < newPassedObjects.Count; i++)
		{
			object obj = newPassedObjects[i];
			Interactable.Passed passed = obj as Interactable.Passed;
			if (passed != null)
			{
				if (passed.varType == Interactable.PassedVarType.companyID)
				{
					this.soldHere = CityData.Instance.companyDirectory.Find((Company item) => item.companyID == (int)passed.value);
				}
				else if (passed.varType == Interactable.PassedVarType.time)
				{
					this.purchasedTime = passed.value;
				}
				else if (passed.varType == Interactable.PassedVarType.stringInteractablePreset)
				{
					InteractablePreset interactablePreset = null;
					if (Toolbox.Instance.objectPresetDictionary.TryGetValue(passed.str, ref interactablePreset))
					{
						this.purchased.Add(interactablePreset);
					}
				}
			}
			Interactable interactable = obj as Interactable;
			if (interactable != null)
			{
				this.interactable = interactable;
				this.interactablePreset = this.interactable.preset;
			}
			Company company = obj as Company;
			if (company != null)
			{
				this.soldHere = company;
			}
		}
	}

	// Token: 0x0600240A RID: 9226 RVA: 0x001DC50C File Offset: 0x001DA70C
	public override void BuildDataSources()
	{
		if (this.fromFact == null && this.soldHere != null && this.soldHere.address != null)
		{
			this.fromFact = EvidenceCreator.Instance.CreateFact("From", this, this.soldHere.address.evidenceEntry, null, null, false, null, null, null, false);
		}
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x001DC56C File Offset: 0x001DA76C
	public override string GenerateName()
	{
		string text = Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
		if (this.isFound && this.soldHere != null)
		{
			text = string.Concat(new string[]
			{
				text,
				" ",
				Strings.Get("evidence.names", "from", Strings.Casing.asIs, false, false, false, null),
				" ",
				this.soldHere.name
			});
		}
		return text;
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x001DC5EC File Offset: 0x001DA7EC
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		this.UpdateName();
		if (this.purchaseTimeEvidence == null)
		{
			this.purchaseTimeEvidence = EvidenceCreator.Instance.GetTimeEvidence(this.purchasedTime, this.purchasedTime, "PurchasedTime", "", -1, -1);
		}
		if (this.interactable != null)
		{
			this.interactable.UpdateName(true, Evidence.DataKey.name);
		}
	}

	// Token: 0x04002DEC RID: 11756
	public Company soldHere;

	// Token: 0x04002DED RID: 11757
	public float purchasedTime;

	// Token: 0x04002DEE RID: 11758
	public EvidenceTime purchaseTimeEvidence;

	// Token: 0x04002DEF RID: 11759
	public Fact fromFact;

	// Token: 0x04002DF0 RID: 11760
	public List<InteractablePreset> purchased = new List<InteractablePreset>();
}
