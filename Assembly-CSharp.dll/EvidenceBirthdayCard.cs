using System;
using System.Collections.Generic;

// Token: 0x02000642 RID: 1602
public class EvidenceBirthdayCard : Evidence
{
	// Token: 0x06002397 RID: 9111 RVA: 0x001D89EC File Offset: 0x001D6BEC
	public EvidenceBirthdayCard(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		this.birthdayCitizen = (newController as Citizen);
		int id = (int)(newPassedObjects[0] as Interactable.Passed).value;
		CityData.Instance.GetHuman(id, out this.from, true);
		this.birthdayCitizen.FindAcquaintanceExists(this.from, out this.relationship);
		base.AddFactLink(this.birthdayCitizen.factDictionary["Birthday"], Evidence.DataKey.name, false);
		foreach (Fact newFact in this.relationship.connectionFacts)
		{
			base.AddFactLink(newFact, Evidence.DataKey.name, false);
		}
		if (this.relationship.connectionFacts.Count <= 0)
		{
			Game.LogError(this.relationship.from.GetCitizenName() + " to " + this.relationship.with.GetCitizenName() + " features no connection facts!", 2);
		}
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x001D8B04 File Offset: 0x001D6D04
	public override void BuildDataSources()
	{
		base.BuildDataSources();
		try
		{
			if (this.relationship.connections.Contains(Acquaintance.ConnectionType.boss) || this.relationship.connections.Contains(Acquaintance.ConnectionType.familiarWork) || this.relationship.connections.Contains(Acquaintance.ConnectionType.workOther) || this.relationship.connections.Contains(Acquaintance.ConnectionType.workTeam) || this.relationship.secretConnection == Acquaintance.ConnectionType.paramour)
			{
				base.AddFactLink(this.birthdayCitizen.factDictionary["WorksAt"], Evidence.DataKey.name, false);
			}
			else
			{
				base.AddFactLink(this.birthdayCitizen.factDictionary["LivesAt"], Evidence.DataKey.name, false);
			}
			base.AddFactLink(EvidenceCreator.Instance.CreateFact("BelongsTo", this, this.birthdayCitizen.evidenceEntry, null, null, false, null, null, null, false), Evidence.DataKey.name, true);
			base.AddFactLink(EvidenceCreator.Instance.CreateFact("SentBy", this, this.from.evidenceEntry, null, null, false, null, null, null, false), Evidence.DataKey.name, true);
		}
		catch
		{
		}
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x001D8C18 File Offset: 0x001D6E18
	public override string GenerateName()
	{
		return Strings.Get("evidence.generic", this.preset.name, Strings.Casing.asIs, false, false, false, null);
	}

	// Token: 0x04002DB4 RID: 11700
	public Citizen birthdayCitizen;

	// Token: 0x04002DB5 RID: 11701
	public Human from;

	// Token: 0x04002DB6 RID: 11702
	public Acquaintance relationship;
}
