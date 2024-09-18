using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000677 RID: 1655
public class FactMatches : Fact
{
	// Token: 0x0600245A RID: 9306 RVA: 0x001DEA47 File Offset: 0x001DCC47
	public FactMatches(FactPreset newPreset, List<Evidence> newFromEvidence, List<Evidence> newToEvidence, List<object> newPassedObjects, List<Evidence.DataKey> overrideFromKeys, List<Evidence.DataKey> overrideToKeys, bool isCustomFact) : base(newPreset, newFromEvidence, newToEvidence, newPassedObjects, overrideFromKeys, overrideToKeys, isCustomFact)
	{
		this.matchPreset = (newPassedObjects[0] as MatchPreset);
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x001DEA70 File Offset: 0x001DCC70
	public static bool MatchCheck(MatchPreset match, Evidence matchFrom, Evidence matchTo)
	{
		bool result = true;
		foreach (MatchPreset.MatchCondition matchCondition in match.matchConditions)
		{
			if (matchCondition == MatchPreset.MatchCondition.fingerprint)
			{
				if (matchFrom.writer != matchTo.writer)
				{
					return false;
				}
			}
			else if (matchCondition == MatchPreset.MatchCondition.time)
			{
				float timeFrom = (matchFrom as EvidenceTime).timeFrom;
				float timeTo = (matchFrom as EvidenceTime).timeTo;
				float timeFrom2 = (matchTo as EvidenceTime).timeFrom;
				float timeTo2 = (matchTo as EvidenceTime).timeTo;
				if (!Toolbox.Instance.GameTimeRangeOverlap(new Vector2(timeFrom, timeTo), new Vector2(timeFrom2, timeTo2), true))
				{
					return false;
				}
			}
			else if (matchCondition == MatchPreset.MatchCondition.visualDescriptors)
			{
				EvidenceCitizen evidenceCitizen = matchFrom as EvidenceCitizen;
				EvidenceCitizen evidenceCitizen2 = matchTo as EvidenceCitizen;
				if (evidenceCitizen == null || evidenceCitizen2 == null)
				{
					return false;
				}
				Citizen citizen = evidenceCitizen.witnessController as Citizen;
				Citizen citizen2 = evidenceCitizen2.witnessController as Citizen;
				List<Evidence.DataKey> tiedKeys = evidenceCitizen.GetTiedKeys(match.linkFromKeys);
				List<Evidence.DataKey> tiedKeys2 = evidenceCitizen2.GetTiedKeys(match.linkToKeys);
				int num = 0;
				if (tiedKeys.Contains(Evidence.DataKey.sex) && tiedKeys2.Contains(Evidence.DataKey.sex))
				{
					if (citizen.gender != citizen2.gender)
					{
						return false;
					}
					num++;
				}
				if (tiedKeys.Contains(Evidence.DataKey.build) && tiedKeys2.Contains(Evidence.DataKey.build))
				{
					if (citizen.descriptors.build != citizen2.descriptors.build)
					{
						return false;
					}
					num++;
				}
				if (tiedKeys.Contains(Evidence.DataKey.eyes) && tiedKeys2.Contains(Evidence.DataKey.eyes))
				{
					if (citizen.descriptors.eyeColour != citizen2.descriptors.eyeColour)
					{
						return false;
					}
					num++;
				}
				if (tiedKeys.Contains(Evidence.DataKey.age) && tiedKeys2.Contains(Evidence.DataKey.age))
				{
					if (citizen.GetAge() != citizen2.GetAge())
					{
						return false;
					}
					num++;
				}
				if (num <= 0)
				{
					return false;
				}
			}
			else if (matchCondition == MatchPreset.MatchCondition.retailPresetMatch)
			{
				EvidenceRetailItem evidenceRetailItem = matchFrom as EvidenceRetailItem;
				EvidenceRetailItem evidenceRetailItem2 = matchTo as EvidenceRetailItem;
				if (evidenceRetailItem.interactablePreset != evidenceRetailItem2.interactablePreset)
				{
					return false;
				}
				if ((evidenceRetailItem.interactablePreset.spawnEvidence == GameplayControls.Instance.retailItemSoldDiscovery || evidenceRetailItem2.interactablePreset.spawnEvidence == GameplayControls.Instance.retailItemSoldDiscovery) && evidenceRetailItem.soldHere != evidenceRetailItem2.soldHere)
				{
					return false;
				}
			}
			else if (matchCondition == MatchPreset.MatchCondition.murderWeapon)
			{
				return true;
			}
		}
		return result;
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x001D5ADE File Offset: 0x001D3CDE
	public override string GenerateNameSuffix()
	{
		return string.Empty;
	}

	// Token: 0x04002E15 RID: 11797
	public MatchPreset matchPreset;

	// Token: 0x04002E16 RID: 11798
	public float timeRangeDifference;

	// Token: 0x04002E17 RID: 11799
	public float travelTime;

	// Token: 0x04002E18 RID: 11800
	private NewNode closest1;

	// Token: 0x04002E19 RID: 11801
	private NewNode closest2;
}
