using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class Acquaintance : IComparable<Acquaintance>
{
	// Token: 0x060005F9 RID: 1529 RVA: 0x0005F290 File Offset: 0x0005D490
	public Acquaintance(Human newFrom, Human newWith, float newKnown, Acquaintance.ConnectionType newConnection, Acquaintance.ConnectionType newSecretConnection, GroupsController.SocialGroup newGroup)
	{
		this.secretConnection = newSecretConnection;
		this.group = newGroup;
		this.from = newFrom;
		this.with = newWith;
		this.AddConnection(newKnown, newConnection);
		this.from.acquaintances.Add(this);
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x0005F308 File Offset: 0x0005D508
	public void AddConnection(float newKnown, Acquaintance.ConnectionType newConnection)
	{
		if (!this.connections.Contains(newConnection))
		{
			if (newKnown > this.known)
			{
				this.connections.Insert(0, newConnection);
			}
			else
			{
				this.connections.Add(newConnection);
			}
		}
		this.known = Mathf.Max(newKnown, this.known);
		this.compatible = this.CalculateCompatible();
		if (newKnown > 0f)
		{
			this.CalculateLike();
		}
		this.SetupFacts();
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0005F37C File Offset: 0x0005D57C
	public Acquaintance(CitySaveData.AcquaintanceCitySave data)
	{
		this.from = CityData.Instance.citizenDirectory.Find((Citizen item) => item.humanID == data.from);
		this.with = CityData.Instance.citizenDirectory.Find((Citizen item) => item.humanID == data.with);
		if (this.from == null)
		{
			Game.LogError("Acquaintance from is missing! " + data.from.ToString(), 2);
		}
		if (this.with == null)
		{
			Game.LogError("Acquaintance with is missing! " + data.with.ToString(), 2);
		}
		this.connections = new List<Acquaintance.ConnectionType>(data.connections);
		this.secretConnection = data.secret;
		this.compatible = data.compatible;
		this.known = data.known;
		this.like = data.like;
		this.dataKeys = data.dataKeys;
		this.from.acquaintances.Add(this);
		this.SetupFacts();
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0005F4E8 File Offset: 0x0005D6E8
	public void SetupFacts()
	{
		using (List<Acquaintance.ConnectionType>.Enumerator enumerator = this.connections.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Acquaintance.ConnectionType conn = enumerator.Current;
				Acquaintance acquaintance = this.with.acquaintances.Find((Acquaintance item) => item.with == this.from && item.connections.Contains(conn));
				if (acquaintance != null)
				{
					using (List<Fact>.Enumerator enumerator2 = acquaintance.connectionFacts.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Fact fact = enumerator2.Current;
							if (conn == Acquaintance.ConnectionType.boss)
							{
								this.from.AddDetailToDict("IsBossOf_" + this.from.GetCitizenName(), fact);
							}
							else if (conn == Acquaintance.ConnectionType.friend)
							{
								this.from.AddDetailToDict("IsFriendsWith_" + this.with.GetCitizenName(), fact);
							}
							else if (conn == Acquaintance.ConnectionType.lover)
							{
								this.from.AddDetailToDict("IsInRelationshipWith_" + this.with.GetCitizenName(), fact);
							}
							else if (conn == Acquaintance.ConnectionType.housemate)
							{
								this.from.AddDetailToDict("IsHousemateOf" + this.with.GetCitizenName(), fact);
							}
							else if (conn == Acquaintance.ConnectionType.neighbor)
							{
								this.from.AddDetailToDict("IsNeighborOf_" + this.with.GetCitizenName(), fact);
							}
							else if (conn == Acquaintance.ConnectionType.workOther)
							{
								this.from.AddDetailToDict("WorksWith_" + this.with.GetCitizenName(), fact);
							}
							else if (conn == Acquaintance.ConnectionType.workTeam)
							{
								this.from.AddDetailToDict("WorksWith_" + this.with.GetCitizenName(), fact);
							}
							else if (conn == Acquaintance.ConnectionType.familiarResidence)
							{
								this.from.AddDetailToDict("FamiliarResidence" + this.with.GetCitizenName(), fact);
							}
							else if (conn == Acquaintance.ConnectionType.familiarWork)
							{
								this.from.AddDetailToDict("FamiliarWork" + this.with.GetCitizenName(), fact);
							}
							if (!this.connectionFacts.Contains(fact))
							{
								this.connectionFacts.Add(fact);
							}
						}
						continue;
					}
				}
				Fact fact2 = null;
				if (conn == Acquaintance.ConnectionType.boss)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("IsBossOf", this.with.evidenceEntry, this.from.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("IsBossOf_" + this.from.GetCitizenName(), fact2);
					}
				}
				else if (conn == Acquaintance.ConnectionType.friend)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("IsFriendsWith", this.from.evidenceEntry, this.with.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("IsFriendsWith_" + this.with.GetCitizenName(), fact2);
					}
				}
				else if (conn == Acquaintance.ConnectionType.lover)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("IsInRelationshipWith", this.from.evidenceEntry, this.with.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("IsInRelationshipWith_" + this.with.GetCitizenName(), fact2);
					}
				}
				else if (conn == Acquaintance.ConnectionType.housemate)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("IsHousemateOf", this.from.evidenceEntry, this.with.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("IsHousemateOf_" + this.with.GetCitizenName(), fact2);
					}
				}
				else if (conn == Acquaintance.ConnectionType.neighbor)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("IsNeighborOf", this.from.evidenceEntry, this.with.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("IsNeighborOf_" + this.with.GetCitizenName(), fact2);
					}
				}
				else if (conn == Acquaintance.ConnectionType.workOther)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("WorksWith", this.from.evidenceEntry, this.with.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("WorksWith_" + this.with.GetCitizenName(), fact2);
					}
				}
				else if (conn == Acquaintance.ConnectionType.workTeam)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("WorksWith", this.from.evidenceEntry, this.with.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("WorksWith_" + this.with.GetCitizenName(), fact2);
					}
				}
				else if (conn == Acquaintance.ConnectionType.familiarResidence)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("FamiliarResidence", this.from.evidenceEntry, this.with.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("FamiliarResidence_" + this.with.GetCitizenName(), fact2);
					}
				}
				else if (conn == Acquaintance.ConnectionType.familiarWork)
				{
					fact2 = EvidenceCreator.Instance.CreateFact("FamiliarWork", this.from.evidenceEntry, this.with.evidenceEntry, null, null, false, null, null, null, false);
					if (fact2 != null)
					{
						this.from.AddDetailToDict("FamiliarWork_" + this.with.GetCitizenName(), fact2);
					}
				}
				if (fact2 != null && !this.connectionFacts.Contains(fact2))
				{
					this.connectionFacts.Add(fact2);
				}
			}
		}
		Acquaintance acquaintance2 = this.with.acquaintances.Find((Acquaintance item) => item.with == this.from && item.secretConnection == this.secretConnection);
		if (acquaintance2 != null)
		{
			using (List<Fact>.Enumerator enumerator2 = acquaintance2.connectionFacts.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Fact fact3 = enumerator2.Current;
					if (this.secretConnection == Acquaintance.ConnectionType.paramour)
					{
						this.from.AddDetailToDict("IsParamourOf_" + this.from.GetCitizenName(), fact3);
					}
					if (!this.connectionFacts.Contains(fact3))
					{
						this.connectionFacts.Add(fact3);
					}
				}
				return;
			}
		}
		Fact fact4 = null;
		if (this.secretConnection == Acquaintance.ConnectionType.paramour)
		{
			fact4 = EvidenceCreator.Instance.CreateFact("IsParamourOf", this.with.evidenceEntry, this.from.evidenceEntry, null, null, false, null, null, null, false);
			if (fact4 != null)
			{
				this.from.AddDetailToDict("IsParamourOf_" + this.from.GetCitizenName(), fact4);
			}
		}
		if (fact4 != null && !this.connectionFacts.Contains(fact4))
		{
			this.connectionFacts.Add(fact4);
		}
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x0005FC6C File Offset: 0x0005DE6C
	public float CalculateCompatible()
	{
		return Mathf.Clamp01(0f + (1f - Mathf.Abs(this.from.humility - this.with.humility)) * 0.1f + this.with.humility * 0.2f + (1f - Mathf.Abs(this.from.emotionality - this.with.emotionality)) * 0.1f + (1f - Mathf.Abs(this.from.extraversion - this.with.extraversion)) * 0.1f + (1f - Mathf.Abs(this.from.agreeableness - this.with.agreeableness)) * 0.1f + this.with.agreeableness * 0.2f + (1f - Mathf.Abs(this.from.conscientiousness - this.with.conscientiousness)) * 0.1f + (1f - Mathf.Abs(this.from.creativity - this.with.creativity)) * 0.05f);
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x0005FD9D File Offset: 0x0005DF9D
	public Human GetOther(Human other)
	{
		if (other == this.from)
		{
			return this.with;
		}
		if (other = this.with)
		{
			return this.from;
		}
		return null;
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x0005FDCC File Offset: 0x0005DFCC
	public void AddKnow(float plusKnow)
	{
		this.known += plusKnow;
		this.known = Mathf.Clamp01(this.known);
		this.CalculateLike();
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x0005FDF4 File Offset: 0x0005DFF4
	public void CalculateLike()
	{
		this.compatible = Mathf.Clamp01(this.compatible);
		this.known = Mathf.Clamp01(this.known);
		this.like = Mathf.Lerp(Mathf.Clamp(this.from.agreeableness * 0.66f + 0.33f, 0.33f, 0.66f), this.compatible, this.known);
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0005FE60 File Offset: 0x0005E060
	public void OthersKnowledgeUpdate()
	{
		foreach (Fact fact in this.connectionFacts)
		{
			foreach (Evidence.DataKey dataKey in fact.preset.applyFromKeysOnDiscovery)
			{
				if (!this.dataKeys.Contains(dataKey))
				{
					this.dataKeys.Add(dataKey);
				}
			}
			foreach (Evidence.DataKey dataKey2 in fact.preset.applyToKeysOnDiscovery)
			{
				if (!this.dataKeys.Contains(dataKey2))
				{
					this.dataKeys.Add(dataKey2);
				}
			}
		}
		if (this.known > SocialControls.Instance.knowPlaceOfWorkThreshold || this.connections.Contains(Acquaintance.ConnectionType.familiarWork) || this.connections.Contains(Acquaintance.ConnectionType.workOther) || this.connections.Contains(Acquaintance.ConnectionType.workTeam) || this.connections.Contains(Acquaintance.ConnectionType.boss) || this.secretConnection == Acquaintance.ConnectionType.paramour)
		{
			Fact fact2 = null;
			if (this.with.factDictionary.TryGetValue("WorksAt", ref fact2))
			{
				foreach (Evidence.DataKey dataKey3 in fact2.preset.applyFromKeysOnDiscovery)
				{
					if (!this.dataKeys.Contains(dataKey3))
					{
						this.dataKeys.Add(dataKey3);
					}
				}
				foreach (Evidence.DataKey dataKey4 in fact2.preset.applyToKeysOnDiscovery)
				{
					if (!this.dataKeys.Contains(dataKey4))
					{
						this.dataKeys.Add(dataKey4);
					}
				}
				foreach (Fact fact3 in this.connectionFacts)
				{
				}
			}
		}
		if (this.known > SocialControls.Instance.knowAddressThreshold || this.connections.Contains(Acquaintance.ConnectionType.friend) || this.connections.Contains(Acquaintance.ConnectionType.housemate) || this.connections.Contains(Acquaintance.ConnectionType.lover) || this.connections.Contains(Acquaintance.ConnectionType.neighbor) || this.secretConnection == Acquaintance.ConnectionType.paramour)
		{
			Fact fact4 = null;
			if (this.with.factDictionary.TryGetValue("LivesAt", ref fact4))
			{
				foreach (Evidence.DataKey dataKey5 in fact4.preset.applyFromKeysOnDiscovery)
				{
					if (!this.dataKeys.Contains(dataKey5))
					{
						this.dataKeys.Add(dataKey5);
					}
				}
				foreach (Evidence.DataKey dataKey6 in fact4.preset.applyToKeysOnDiscovery)
				{
					if (!this.dataKeys.Contains(dataKey6))
					{
						this.dataKeys.Add(dataKey6);
					}
				}
				foreach (Fact fact5 in this.connectionFacts)
				{
				}
			}
		}
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00060244 File Offset: 0x0005E444
	public int CompareTo(Acquaintance comp)
	{
		return this.like.CompareTo(comp.like);
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00060258 File Offset: 0x0005E458
	public CitySaveData.AcquaintanceCitySave GenerateSaveData()
	{
		return new CitySaveData.AcquaintanceCitySave
		{
			from = this.from.humanID,
			with = this.with.humanID,
			connections = new List<Acquaintance.ConnectionType>(this.connections),
			secret = this.secretConnection,
			compatible = this.compatible,
			known = this.known,
			like = this.like,
			dataKeys = this.dataKeys
		};
	}

	// Token: 0x0400061C RID: 1564
	public Human from;

	// Token: 0x0400061D RID: 1565
	public Human with;

	// Token: 0x0400061E RID: 1566
	public Acquaintance.ConnectionType secretConnection;

	// Token: 0x0400061F RID: 1567
	public float compatible;

	// Token: 0x04000620 RID: 1568
	public float known = 0.1f;

	// Token: 0x04000621 RID: 1569
	public float like;

	// Token: 0x04000622 RID: 1570
	[NonSerialized]
	public GroupsController.SocialGroup group;

	// Token: 0x04000623 RID: 1571
	public List<Acquaintance.ConnectionType> connections = new List<Acquaintance.ConnectionType>();

	// Token: 0x04000624 RID: 1572
	public float customSort;

	// Token: 0x04000625 RID: 1573
	public List<Evidence.DataKey> dataKeys = new List<Evidence.DataKey>();

	// Token: 0x04000626 RID: 1574
	public List<Fact> connectionFacts = new List<Fact>();

	// Token: 0x04000627 RID: 1575
	public static Comparison<Acquaintance> customComparison = (Acquaintance object1, Acquaintance object2) => object1.customSort.CompareTo(object2.customSort);

	// Token: 0x020000D2 RID: 210
	public enum ConnectionType
	{
		// Token: 0x04000629 RID: 1577
		friend,
		// Token: 0x0400062A RID: 1578
		neighbor,
		// Token: 0x0400062B RID: 1579
		housemate,
		// Token: 0x0400062C RID: 1580
		lover,
		// Token: 0x0400062D RID: 1581
		boss,
		// Token: 0x0400062E RID: 1582
		workTeam,
		// Token: 0x0400062F RID: 1583
		workOther,
		// Token: 0x04000630 RID: 1584
		regularCustomer,
		// Token: 0x04000631 RID: 1585
		regularStaff,
		// Token: 0x04000632 RID: 1586
		familiarResidence,
		// Token: 0x04000633 RID: 1587
		familiarWork,
		// Token: 0x04000634 RID: 1588
		publicFigure,
		// Token: 0x04000635 RID: 1589
		stranger,
		// Token: 0x04000636 RID: 1590
		paramour,
		// Token: 0x04000637 RID: 1591
		player,
		// Token: 0x04000638 RID: 1592
		anyoneNotPlayer,
		// Token: 0x04000639 RID: 1593
		friendOrWork,
		// Token: 0x0400063A RID: 1594
		knowsName,
		// Token: 0x0400063B RID: 1595
		anyAcquaintance,
		// Token: 0x0400063C RID: 1596
		anyone,
		// Token: 0x0400063D RID: 1597
		workNotBoss,
		// Token: 0x0400063E RID: 1598
		relationshipMatch,
		// Token: 0x0400063F RID: 1599
		corpDove,
		// Token: 0x04000640 RID: 1600
		spamVmail,
		// Token: 0x04000641 RID: 1601
		corpStarch,
		// Token: 0x04000642 RID: 1602
		corpIndigo,
		// Token: 0x04000643 RID: 1603
		corpKaizen,
		// Token: 0x04000644 RID: 1604
		corpElgen,
		// Token: 0x04000645 RID: 1605
		corpCandor,
		// Token: 0x04000646 RID: 1606
		flairQuotes,
		// Token: 0x04000647 RID: 1607
		randomSpamVmail,
		// Token: 0x04000648 RID: 1608
		noReplyVmail,
		// Token: 0x04000649 RID: 1609
		bookGrubs,
		// Token: 0x0400064A RID: 1610
		pestControl,
		// Token: 0x0400064B RID: 1611
		landlord,
		// Token: 0x0400064C RID: 1612
		groupMember,
		// Token: 0x0400064D RID: 1613
		storyPartner
	}
}
