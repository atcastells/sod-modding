using System;
using System.Collections.Generic;

// Token: 0x02000664 RID: 1636
public class EvidenceRetailItem : Evidence
{
	// Token: 0x0600240F RID: 9231 RVA: 0x001DC660 File Offset: 0x001DA860
	public EvidenceRetailItem(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		foreach (object obj in newPassedObjects)
		{
			Interactable.Passed passed = obj as Interactable.Passed;
			if (passed != null)
			{
				if (passed.varType == Interactable.PassedVarType.addressID)
				{
					this.soldHere = CityData.Instance.companyDirectory.Find((Company item) => item.address.id == (int)passed.value);
				}
				else if (passed.varType == Interactable.PassedVarType.time)
				{
					this.purchaseTime = passed.value;
				}
			}
			Interactable interactable = obj as Interactable;
			if (interactable != null)
			{
				this.interactable = interactable;
				this.interactablePreset = interactable.preset;
			}
			InteractablePreset interactablePreset = obj as InteractablePreset;
			if (interactablePreset != null)
			{
				this.interactablePreset = interactablePreset;
				this.isAbstract = true;
			}
			Company company = obj as Company;
			if (company != null)
			{
				this.soldHere = company;
				this.isAbstract = true;
			}
			RetailItemPreset c = obj as RetailItemPreset;
			if (c != null)
			{
				this.retailItem = c;
			}
		}
		if (this.interactablePreset == null && this.retailItem != null)
		{
			this.interactablePreset = this.retailItem.itemPreset;
		}
		if (this.interactablePreset == null && this.interactable != null)
		{
			this.interactablePreset = this.interactable.preset;
		}
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x001DC7EC File Offset: 0x001DA9EC
	public override void BuildDataSources()
	{
		if (this.interactablePreset == null && this.interactable != null)
		{
			this.interactablePreset = this.interactable.preset;
		}
		if (this.soldHere != null)
		{
			Fact newFact = EvidenceCreator.Instance.CreateFact("PurchasedAt", this, this.soldHere.address.evidenceEntry, null, null, false, null, null, null, false);
			base.AddFactLink(newFact, Evidence.DataKey.name, true);
		}
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x001DC85C File Offset: 0x001DAA5C
	public override string GetSummary(List<Evidence.DataKey> keys)
	{
		if (this.interactablePreset != null && this.interactablePreset.summaryMessageSource != null && this.interactablePreset.summaryMessageSource.Length > 0)
		{
			return Strings.GetTextForComponent(this.interactablePreset.summaryMessageSource, this.interactable, null, null, "\n", false, null, Strings.LinkSetting.automatic, null);
		}
		if (this.retailItem != null && this.retailItem.itemPreset != null && this.retailItem.itemPreset.summaryMessageSource != null && this.retailItem.itemPreset.summaryMessageSource.Length > 0)
		{
			return Strings.GetTextForComponent(this.retailItem.itemPreset.summaryMessageSource, this.interactable, null, null, "\n", false, null, Strings.LinkSetting.automatic, null);
		}
		Game.Log("Unable to get summary for EvidenceRetailItem: No interactable preset or retail item has been passed", 2);
		return base.GetSummary(keys);
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x001DC940 File Offset: 0x001DAB40
	public override string GenerateName()
	{
		if (this.interactablePreset == null && this.interactable != null)
		{
			this.interactablePreset = this.interactable.preset;
		}
		string text = string.Empty;
		if (this.interactablePreset != null)
		{
			if (this.interactablePreset.presetName == null || this.interactablePreset.presetName.Length <= 0)
			{
				string text2 = "Interactable preset name is null for ";
				Interactable interactable = this.interactable;
				Game.LogError(text2 + ((interactable != null) ? interactable.ToString() : null), 2);
			}
			else
			{
				text = Strings.Get("evidence.names", this.interactablePreset.presetName, Strings.Casing.asIs, false, false, false, null);
			}
		}
		if (this.isAbstract && this.retailItem != null && this.retailItem.brandName.Length > 0)
		{
			text = Strings.Get("evidence.names", this.retailItem.brandName, Strings.Casing.asIs, false, false, false, null);
		}
		if (this.isFound && this.soldHere != null && this.isAbstract)
		{
			text = string.Concat(new string[]
			{
				text,
				" ",
				Strings.Get("evidence.names", "from", Strings.Casing.asIs, false, false, false, null),
				" ",
				this.soldHere.name
			});
			if (this.soldAtFact == null)
			{
				this.soldAtFact = EvidenceCreator.Instance.CreateFact("SoldAt", this, this.soldHere.address.evidenceEntry, null, null, true, null, null, null, false);
			}
		}
		return text;
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x001DCAC4 File Offset: 0x001DACC4
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		this.UpdateName();
		if (!this.isAbstract)
		{
			if (this.retailItem != null && this.retailItem.isHot)
			{
				if (SessionData.Instance.gameTime - this.purchaseTime <= GameplayControls.Instance.foodHotTime && this.purchaseTimeEvidence == null)
				{
					this.purchaseTimeEvidence = EvidenceCreator.Instance.GetTimeEvidenceRange(this.purchaseTime, GameplayControls.Instance.foodHotTime, true, true, 15, "PurchasedTime", this.evID, -1, -1);
				}
			}
			else if (this.interactablePreset != null)
			{
			}
		}
		else
		{
			this.interactablePreset != null;
		}
		if (this.interactable != null)
		{
			this.interactable.UpdateName(true, Evidence.DataKey.name);
		}
	}

	// Token: 0x04002DF2 RID: 11762
	public Company soldHere;

	// Token: 0x04002DF3 RID: 11763
	public RetailItemPreset retailItem;

	// Token: 0x04002DF4 RID: 11764
	public EvidenceTime purchaseTimeEvidence;

	// Token: 0x04002DF5 RID: 11765
	public float purchaseTime;

	// Token: 0x04002DF6 RID: 11766
	public bool isAbstract;

	// Token: 0x04002DF7 RID: 11767
	public Fact soldAtFact;
}
