using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000670 RID: 1648
public class Fact : CaseComponent
{
	// Token: 0x1400005C RID: 92
	// (add) Token: 0x06002436 RID: 9270 RVA: 0x001DD9E4 File Offset: 0x001DBBE4
	// (remove) Token: 0x06002437 RID: 9271 RVA: 0x001DDA1C File Offset: 0x001DBC1C
	public event Fact.ConnectingEvidenceChangeDataKey OnConnectingEvidenceChangeDataKey;

	// Token: 0x1400005D RID: 93
	// (add) Token: 0x06002438 RID: 9272 RVA: 0x001DDA54 File Offset: 0x001DBC54
	// (remove) Token: 0x06002439 RID: 9273 RVA: 0x001DDA8C File Offset: 0x001DBC8C
	public event Fact.IsSeen OnSeen;

	// Token: 0x0600243A RID: 9274 RVA: 0x001DDAC4 File Offset: 0x001DBCC4
	public Fact(FactPreset newPreset, List<Evidence> newFromEvidence, List<Evidence> newToEvidence, List<object> newPassedObjects, List<Evidence.DataKey> overrideFromKeys, List<Evidence.DataKey> overrideToKeys, bool isCustomFact)
	{
		GameplayController.Instance.factList.Add(this);
		this.preset = newPreset;
		this.fromEvidence = newFromEvidence;
		this.toEvidence = newToEvidence;
		this.isCustom = isCustomFact;
		if (overrideFromKeys == null)
		{
			if (this.preset.fromDataKeys.Count > 0)
			{
				this.fromDataKeys = new List<Evidence.DataKey>(this.preset.fromDataKeys);
			}
			else
			{
				this.fromDataKeys = Toolbox.Instance.allDataKeys;
			}
		}
		else
		{
			this.fromDataKeys = overrideFromKeys;
		}
		if (overrideToKeys == null)
		{
			if (this.preset.toDataKeys.Count > 0)
			{
				this.toDataKeys = new List<Evidence.DataKey>(this.preset.toDataKeys);
			}
			else
			{
				this.toDataKeys = Toolbox.Instance.allDataKeys;
			}
		}
		else
		{
			this.toDataKeys = overrideToKeys;
		}
		base.SetNewIcon(this.preset.iconSpriteLarge);
		this.ConnectFact();
		if (this.preset.discoverOnCreate || Game.Instance.discoverAllEvidence)
		{
			this.SetFound(true);
		}
		foreach (Evidence evidence in this.fromEvidence)
		{
			if (evidence != null)
			{
				evidence.OnDataKeyChange += this.OnConnectionsChangedDataKeys;
			}
		}
		foreach (Evidence evidence2 in this.toEvidence)
		{
			if (evidence2 != null)
			{
				evidence2.OnDataKeyChange += this.OnConnectionsChangedDataKeys;
			}
		}
		if (this.preset.iconSpriteLarge != null)
		{
			base.SetNewIcon(this.preset.iconSpriteLarge);
		}
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x001DDC98 File Offset: 0x001DBE98
	public override string GetIdentifier()
	{
		string text = string.Empty;
		foreach (Evidence evidence in this.fromEvidence)
		{
			text += evidence.GetIdentifier();
		}
		string text2 = string.Empty;
		foreach (Evidence evidence2 in this.toEvidence)
		{
			text2 += evidence2.GetIdentifier();
		}
		return string.Concat(new string[]
		{
			this.preset.name,
			"-",
			text,
			"-",
			text2
		});
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x001DDD78 File Offset: 0x001DBF78
	public override string GenerateName()
	{
		string text = string.Empty;
		for (int i = 0; i < this.fromEvidence.Count; i++)
		{
			Evidence evidence = this.fromEvidence[i];
			if (i > 0)
			{
				text += " & ";
			}
			text += evidence.GetNameForDataKey(Evidence.DataKey.name);
		}
		text = text + " " + Strings.Get("evidence.generic", this.preset.name, Strings.Casing.asIs, false, false, false, null) + " ";
		for (int j = 0; j < this.toEvidence.Count; j++)
		{
			Evidence evidence2 = this.toEvidence[j];
			if (j > 0)
			{
				text += " & ";
			}
			text += evidence2.GetNameForDataKey(Evidence.DataKey.name);
		}
		return text.Trim();
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x001DDE44 File Offset: 0x001DC044
	public virtual void ConnectFact()
	{
		foreach (Evidence evidence in this.fromEvidence)
		{
			if (evidence != null)
			{
				evidence.AddFactLink(this, this.fromDataKeys, true);
				if (evidence.isFound)
				{
					this.OnConnectedEvidenceDiscovery(evidence);
				}
				else
				{
					evidence.OnDiscoveredThis += this.OnConnectedEvidenceDiscovery;
				}
			}
		}
		foreach (Evidence evidence2 in this.toEvidence)
		{
			if (evidence2 != null)
			{
				evidence2.AddFactLink(this, this.toDataKeys, false);
				if (evidence2.isFound)
				{
					this.OnConnectedEvidenceDiscovery(evidence2);
				}
				else
				{
					evidence2.OnDiscoveredThis += this.OnConnectedEvidenceDiscovery;
				}
			}
		}
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x001DDF38 File Offset: 0x001DC138
	public override void OnDiscovery()
	{
		if (!this.preset.countsAsNewInformationOnDiscovery)
		{
			this.SetSeen();
		}
		foreach (Evidence.DataKey keyTwo in this.fromDataKeys)
		{
			foreach (Evidence evidence in this.fromEvidence)
			{
				foreach (Evidence.DataKey keyOne in this.preset.applyFromKeysOnDiscovery)
				{
					evidence.MergeDataKeys(keyOne, keyTwo);
				}
			}
		}
		foreach (Evidence.DataKey keyTwo2 in this.toDataKeys)
		{
			foreach (Evidence evidence2 in this.toEvidence)
			{
				foreach (Evidence.DataKey keyOne2 in this.preset.applyToKeysOnDiscovery)
				{
					evidence2.MergeDataKeys(keyOne2, keyTwo2);
				}
			}
		}
		int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.moneyForAddresses));
		if (num > 0 && this.preset.name == "LivesAt")
		{
			GameplayController.Instance.AddMoney(num, false, "moneyforaddresses");
		}
	}

	// Token: 0x0600243F RID: 9279 RVA: 0x001DE128 File Offset: 0x001DC328
	public void SetSeen()
	{
		if (!this.isSeen)
		{
			this.isSeen = true;
			if (this.OnSeen != null)
			{
				this.OnSeen();
			}
		}
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x001DE14C File Offset: 0x001DC34C
	public virtual void OnConnectedEvidenceDiscovery(CaseComponent discovered)
	{
		if (!this.isFound)
		{
			Evidence evidence = discovered as Evidence;
			if (evidence != null)
			{
				evidence.UpdateDiscoveries();
			}
		}
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x001DE174 File Offset: 0x001DC374
	public virtual string GetName(Evidence.FactLink specificLink = null)
	{
		string text = string.Empty;
		if (specificLink == null || !specificLink.thisIsTheFromEvidence)
		{
			for (int i = 0; i < this.fromEvidence.Count; i++)
			{
				Evidence evidence = this.fromEvidence[i];
				if (evidence == null)
				{
					Game.LogError("Null from evidence in fact " + this.preset.name, 2);
				}
				else
				{
					if (i > 0)
					{
						text += " & ";
					}
					text += evidence.GetNameForDataKey(this.fromDataKeys);
				}
			}
		}
		else
		{
			text += specificLink.thisEvidence.GetNameForDataKey(this.fromDataKeys);
		}
		text = text + " " + Strings.Get("evidence.generic", this.preset.name, Strings.Casing.asIs, false, false, false, null) + " ";
		if (specificLink == null || specificLink.thisIsTheFromEvidence)
		{
			for (int j = 0; j < this.toEvidence.Count; j++)
			{
				Evidence evidence2 = this.toEvidence[j];
				if (evidence2 == null)
				{
					Game.LogError("Null to evidence in fact " + this.preset.name, 2);
				}
				else
				{
					if (j > 0)
					{
						text += " & ";
					}
					text += evidence2.GetNameForDataKey(this.toDataKeys);
				}
			}
		}
		else
		{
			text += specificLink.thisEvidence.GetNameForDataKey(this.fromDataKeys);
		}
		text = text + this.FoundAtName() + this.GenerateNameSuffix();
		return text.Trim();
	}

	// Token: 0x06002442 RID: 9282 RVA: 0x001DE2E8 File Offset: 0x001DC4E8
	public Evidence GetOther(Evidence ev)
	{
		if (this.fromEvidence.Count <= 0)
		{
			Game.LogError("Evidence for fact " + this.name + " features no 'from' evidence!", 2);
			return null;
		}
		if (this.toEvidence.Count <= 0)
		{
			Game.LogError("Evidence for fact " + this.name + " features no 'to' evidence!", 2);
			return null;
		}
		if (this.fromEvidence.Contains(ev))
		{
			return this.toEvidence[0];
		}
		if (this.toEvidence.Contains(ev))
		{
			return this.fromEvidence[0];
		}
		return null;
	}

	// Token: 0x06002443 RID: 9283 RVA: 0x001DE384 File Offset: 0x001DC584
	public List<Evidence> GetOther(List<Evidence> ev)
	{
		foreach (Evidence evidence in ev)
		{
			if (this.fromEvidence.Contains(evidence))
			{
				return this.toEvidence;
			}
			if (this.toEvidence.Contains(evidence))
			{
				return this.fromEvidence;
			}
		}
		return null;
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x001DE3FC File Offset: 0x001DC5FC
	public void OnConnectionsChangedDataKeys()
	{
		if (this.OnConnectingEvidenceChangeDataKey != null)
		{
			this.OnConnectingEvidenceChangeDataKey();
		}
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x001DE414 File Offset: 0x001DC614
	public string GetSerializedString()
	{
		string text = string.Empty;
		string result;
		try
		{
			if (this.preset != null)
			{
				text = text + this.preset.name + "|";
			}
			if (this.fromEvidence != null)
			{
				foreach (Evidence evidence in this.fromEvidence)
				{
					if (evidence != null)
					{
						text = text + evidence.evID + ",";
					}
				}
			}
			text += "|";
			if (this.toEvidence != null)
			{
				foreach (Evidence evidence2 in this.toEvidence)
				{
					if (evidence2 != null)
					{
						text = text + evidence2.evID + ",";
					}
				}
			}
			text += "|";
			if (this.fromDataKeys != null)
			{
				foreach (Evidence.DataKey dataKey in this.fromDataKeys)
				{
					string text2 = text;
					int num = (int)dataKey;
					text = text2 + num.ToString() + ",";
				}
			}
			text += "|";
			if (this.toDataKeys != null)
			{
				foreach (Evidence.DataKey dataKey2 in this.toDataKeys)
				{
					string text3 = text;
					int num = (int)dataKey2;
					text = text3 + num.ToString() + ",";
				}
			}
			text = string.Concat(new string[]
			{
				text,
				"|",
				Convert.ToInt32(this.isFound).ToString(),
				"|",
				Convert.ToInt32(this.isSeen).ToString(),
				"|",
				Convert.ToInt32(this.isCustom).ToString(),
				"|",
				this.customName
			});
			result = text;
		}
		catch
		{
			result = text;
		}
		return result;
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x001DE6B0 File Offset: 0x001DC8B0
	public virtual void SetCustomName(string str)
	{
		this.customName = str;
	}

	// Token: 0x04002E0A RID: 11786
	[NonSerialized]
	public FactPreset preset;

	// Token: 0x04002E0B RID: 11787
	public List<Evidence> fromEvidence;

	// Token: 0x04002E0C RID: 11788
	public List<Evidence> toEvidence;

	// Token: 0x04002E0D RID: 11789
	public List<Evidence.DataKey> fromDataKeys;

	// Token: 0x04002E0E RID: 11790
	public List<Evidence.DataKey> toDataKeys;

	// Token: 0x04002E0F RID: 11791
	public bool isSeen;

	// Token: 0x04002E10 RID: 11792
	public bool isCustom;

	// Token: 0x04002E11 RID: 11793
	public string customName;

	// Token: 0x02000671 RID: 1649
	// (Invoke) Token: 0x06002448 RID: 9288
	public delegate void ConnectingEvidenceChangeDataKey();

	// Token: 0x02000672 RID: 1650
	// (Invoke) Token: 0x0600244C RID: 9292
	public delegate void IsSeen();
}
