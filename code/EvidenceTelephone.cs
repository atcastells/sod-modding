using System;
using System.Collections.Generic;

// Token: 0x02000669 RID: 1641
public class EvidenceTelephone : Evidence
{
	// Token: 0x0600241E RID: 9246 RVA: 0x001DCF00 File Offset: 0x001DB100
	public EvidenceTelephone(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		this.telephone = (newPassedObjects[0] as Telephone);
		this.interactable = this.telephone.interactable;
		if (this.telephone.locationEntry.locationController.thisAsAddress != null && this.telephone.locationEntry.locationController.thisAsAddress.residence != null)
		{
			foreach (Human human in this.telephone.locationEntry.locationController.thisAsAddress.inhabitants)
			{
				human.evidenceEntry.OnDiscoveryChanged += this.OnInhabitantDiscovery;
			}
		}
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x001DCFE4 File Offset: 0x001DB1E4
	public override string GenerateName()
	{
		string result = string.Empty;
		if (this.telephone != null)
		{
			result = this.telephone.numberString;
		}
		return result;
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x001DD00C File Offset: 0x001DB20C
	public override void BuildDataSources()
	{
		base.BuildDataSources();
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x001DD014 File Offset: 0x001DB214
	public override void OnConnectedFactDiscovery(CaseComponent discovered)
	{
		base.OnConnectedFactDiscovery(discovered);
		this.MergedDataCheck(false);
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x001DD024 File Offset: 0x001DB224
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		this.MergedDataCheck(true);
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x001DD033 File Offset: 0x001DB233
	public void OnInhabitantDiscovery(Evidence.Discovery disc)
	{
		if (disc == Evidence.Discovery.livesAt || disc == Evidence.Discovery.phonePersonal)
		{
			this.MergedDataCheck(false);
		}
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x001DD044 File Offset: 0x001DB244
	public void MergedDataCheck(bool displayMessage)
	{
		if (this.discoveredEverything)
		{
			return;
		}
		bool flag = false;
		List<Human> list = new List<Human>();
		bool flag2 = false;
		foreach (Evidence.FactLink factLink in this.allFacts)
		{
			if (factLink.fact.isFound && factLink.fact.preset.name == "IsTelephoneNumberFor")
			{
				flag = true;
				break;
			}
		}
		if (this.telephone.locationEntry.locationController.thisAsAddress != null && this.telephone.locationEntry.locationController.thisAsAddress.residence != null)
		{
			foreach (Human human in this.telephone.locationEntry.locationController.thisAsAddress.inhabitants)
			{
				if (human.evidenceEntry.discoveryProgress.Contains(Evidence.Discovery.phonePersonal) && !list.Contains(human))
				{
					list.Add(human);
				}
				if (flag && human.evidenceEntry.discoveryProgress.Contains(Evidence.Discovery.livesAt) && !list.Contains(human))
				{
					list.Add(human);
				}
			}
			if (list.Count < this.telephone.locationEntry.locationController.thisAsAddress.inhabitants.Count)
			{
				goto IL_1CF;
			}
			flag2 = true;
			using (List<Human>.Enumerator enumerator2 = this.telephone.locationEntry.locationController.thisAsAddress.inhabitants.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Human human2 = enumerator2.Current;
					human2.evidenceEntry.OnDiscoveryChanged -= this.OnInhabitantDiscovery;
				}
				goto IL_1CF;
			}
		}
		flag2 = true;
		IL_1CF:
		if (flag2 && flag)
		{
			this.discoveredEverything = true;
		}
		if (flag || list.Count > 0)
		{
			Game.Log("Update telephone no: " + flag.ToString() + " owners: " + list.Count.ToString(), 2);
			GameplayController.Instance.AddOrMergePhoneNumberData(this.telephone.number, flag, list, null, true);
		}
	}

	// Token: 0x04002DFC RID: 11772
	public Telephone telephone;

	// Token: 0x04002DFD RID: 11773
	public bool discoveredEverything;
}
