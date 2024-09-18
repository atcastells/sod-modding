using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000782 RID: 1922
[CreateAssetMenu(fileName = "murdererMO_data", menuName = "Database/Murderer MO")]
public class MurderMO : SoCustomComparison
{
	// Token: 0x0600259C RID: 9628 RVA: 0x001E72DC File Offset: 0x001E54DC
	private void OnGUIDValueChangedCallback()
	{
		this.maximumPotentialScore = 0f;
		this.maximumPotentialScore = this.pickRandomScoreRange.y;
		foreach (MurderPreset.MurdererModifierRule murdererModifierRule in this.murdererTraitModifiers)
		{
			this.maximumPotentialScore += murdererModifierRule.scoreModifier;
		}
		if (this.murdererJobModifiers.Count > 0)
		{
			int num = 0;
			foreach (MurderMO.JobModifier jobModifier in this.murdererJobModifiers)
			{
				if (num == 0)
				{
					num = jobModifier.jobBoost;
				}
				else
				{
					num = Mathf.Max(jobModifier.jobBoost, num);
				}
			}
			this.maximumPotentialScore += (float)num;
		}
		if (this.murdererCompanyModifiers.Count > 0)
		{
			int num2 = 0;
			foreach (MurderMO.CompanyModifier companyModifier in this.murdererCompanyModifiers)
			{
				if (num2 == 0)
				{
					num2 = companyModifier.companyBoost;
				}
				else
				{
					num2 = Mathf.Max(companyModifier.companyBoost, num2);
				}
			}
			this.maximumPotentialScore += (float)num2;
		}
		if (this.useMurdererSocialClassRange)
		{
			this.maximumPotentialScore += (float)this.murdererClassRangeBoost;
		}
		if (this.useHexaco)
		{
			this.maximumPotentialScore += (float)this.hexaco.outputMax;
		}
	}

	// Token: 0x040039E5 RID: 14821
	[Header("Notes")]
	[ResizableTextArea]
	public string notes;

	// Token: 0x040039E6 RID: 14822
	[Header("Compatibility")]
	public bool disabled;

	// Token: 0x040039E7 RID: 14823
	[Tooltip("Compatible with these killer types")]
	public List<MurderPreset> compatibleWith = new List<MurderPreset>();

	// Token: 0x040039E8 RID: 14824
	[Range(0f, 2f)]
	public int baseDifficulty;

	// Token: 0x040039E9 RID: 14825
	[ReadOnly]
	[Header("Murderer Suitability")]
	[InfoBox("The max score should equal roughly the same across all MOs if you want MOs to be balanced", 0)]
	public float maximumPotentialScore;

	// Token: 0x040039EA RID: 14826
	[OnValueChanged("OnGUIDValueChangedCallback")]
	public bool updateThis;

	// Token: 0x040039EB RID: 14827
	[OnValueChanged("OnGUIDValueChangedCallback")]
	[Space(10f)]
	public Vector2 pickRandomScoreRange = new Vector2(0f, 1f);

	// Token: 0x040039EC RID: 14828
	[OnValueChanged("OnGUIDValueChangedCallback")]
	public List<MurderPreset.MurdererModifierRule> murdererTraitModifiers = new List<MurderPreset.MurdererModifierRule>();

	// Token: 0x040039ED RID: 14829
	[OnValueChanged("OnGUIDValueChangedCallback")]
	public List<MurderMO.JobModifier> murdererJobModifiers = new List<MurderMO.JobModifier>();

	// Token: 0x040039EE RID: 14830
	[OnValueChanged("OnGUIDValueChangedCallback")]
	public List<MurderMO.CompanyModifier> murdererCompanyModifiers = new List<MurderMO.CompanyModifier>();

	// Token: 0x040039EF RID: 14831
	[OnValueChanged("OnGUIDValueChangedCallback")]
	public bool useMurdererSocialClassRange;

	// Token: 0x040039F0 RID: 14832
	[OnValueChanged("OnGUIDValueChangedCallback")]
	[EnableIf("useMurdererSocialClassRange")]
	public Vector2 murdererClassRange = new Vector2(0f, 1f);

	// Token: 0x040039F1 RID: 14833
	[OnValueChanged("OnGUIDValueChangedCallback")]
	[EnableIf("useMurdererSocialClassRange")]
	public int murdererClassRangeBoost;

	// Token: 0x040039F2 RID: 14834
	[Space(7f)]
	[OnValueChanged("OnGUIDValueChangedCallback")]
	public bool useHexaco;

	// Token: 0x040039F3 RID: 14835
	[ShowIf("useHexaco")]
	[OnValueChanged("OnGUIDValueChangedCallback")]
	public HEXACO hexaco;

	// Token: 0x040039F4 RID: 14836
	[Space(7f)]
	[Tooltip("Ensures the killer will have sniper vantage points at home. Victim's home has to be within line-of-sight of this.")]
	public bool requiresSniperVantageAtHome;

	// Token: 0x040039F5 RID: 14837
	[InfoBox("The killer will pick one of these to kill ALL their victims...", 0)]
	[Header("Weapons Picking")]
	public List<MurderWeaponsPool> weaponsPool = new List<MurderWeaponsPool>();

	// Token: 0x040039F6 RID: 14838
	[Tooltip("Block weapons from being dropped at scene")]
	[Space(7f)]
	public bool blockDroppingWeapons;

	// Token: 0x040039F7 RID: 14839
	[Header("Crime Scene")]
	[Tooltip("The murder can happen anywhere")]
	public bool allowAnywhere;

	// Token: 0x040039F8 RID: 14840
	[DisableIf("allowAnywhere")]
	[Tooltip("The murder can happen at home")]
	public bool allowHome = true;

	// Token: 0x040039F9 RID: 14841
	[DisableIf("allowAnywhere")]
	[Tooltip("The murder can happen at work")]
	public bool allowWork;

	// Token: 0x040039FA RID: 14842
	[DisableIf("allowAnywhere")]
	[Tooltip("The murder can happen in public")]
	public bool allowPublic;

	// Token: 0x040039FB RID: 14843
	[Tooltip("The murder can happen in public")]
	[DisableIf("allowAnywhere")]
	public bool allowStreets;

	// Token: 0x040039FC RID: 14844
	[DisableIf("allowAnywhere")]
	[Tooltip("The murder can happen at the killers den")]
	public bool allowDen;

	// Token: 0x040039FD RID: 14845
	[Header("Victim Suitability")]
	[Range(-20f, 20f)]
	[InfoBox("The below rule will give a big boost to the chances of this person being chosen.", 0)]
	public int acquaintedSuitabilityBoost;

	// Token: 0x040039FE RID: 14846
	[Range(-20f, 20f)]
	public int attractedToSuitabilityBoost;

	// Token: 0x040039FF RID: 14847
	[Tooltip("The following is multiplied by the like value in acquaintance class.")]
	[Range(-20f, 20f)]
	public int likeSuitabilityBoost;

	// Token: 0x04003A00 RID: 14848
	[Range(-20f, 20f)]
	public int sameWorkplaceBoost;

	// Token: 0x04003A01 RID: 14849
	[Range(-20f, 20f)]
	public int murdererIsTenantBoost;

	// Token: 0x04003A02 RID: 14850
	[InfoBox("The killer will rank using these settings to their victims...", 0)]
	public Vector2 victimRandomScoreRange = new Vector2(0f, 1f);

	// Token: 0x04003A03 RID: 14851
	public List<MurderPreset.MurdererModifierRule> victimTraitModifiers = new List<MurderPreset.MurdererModifierRule>();

	// Token: 0x04003A04 RID: 14852
	public List<MurderMO.JobModifier> victimJobModifiers = new List<MurderMO.JobModifier>();

	// Token: 0x04003A05 RID: 14853
	public List<MurderMO.CompanyModifier> victimCompanyModifiers = new List<MurderMO.CompanyModifier>();

	// Token: 0x04003A06 RID: 14854
	public bool useVictimSocialClassRange;

	// Token: 0x04003A07 RID: 14855
	[EnableIf("useVictimSocialClassRange")]
	public Vector2 victimClassRange = new Vector2(0f, 1f);

	// Token: 0x04003A08 RID: 14856
	[EnableIf("useVictimSocialClassRange")]
	public int victimClassRangeBoost;

	// Token: 0x04003A09 RID: 14857
	[Header("Monkier DDS Message List")]
	public string monkierDDSMessageList;

	// Token: 0x04003A0A RID: 14858
	[Header("Confessional Responses")]
	public List<string> confessionalDDSResponses = new List<string>();

	// Token: 0x04003A0B RID: 14859
	[Header("Leads")]
	public List<MurderPreset.MurderLeadItem> MOleads = new List<MurderPreset.MurderLeadItem>();

	// Token: 0x04003A0C RID: 14860
	[Header("Calling Cards")]
	public List<MurderMO.Graffiti> graffiti = new List<MurderMO.Graffiti>();

	// Token: 0x04003A0D RID: 14861
	[InfoBox("The killer will pick one of these to leave at ALL crime scenes...", 0)]
	public List<MurderMO.CallingCardPick> callingCardPool = new List<MurderMO.CallingCardPick>();

	// Token: 0x02000783 RID: 1923
	[Serializable]
	public class CallingCardPick
	{
		// Token: 0x04003A0E RID: 14862
		[Tooltip("The item itself")]
		public InteractablePreset item;

		// Token: 0x04003A0F RID: 14863
		public MurderMO.CallingCardOrigin origin;

		// Token: 0x04003A10 RID: 14864
		[Space(7f)]
		public Vector2 randomScoreRange = new Vector2(0f, 0f);

		// Token: 0x04003A11 RID: 14865
		public List<MurderPreset.MurdererModifierRule> traitModifiers = new List<MurderPreset.MurdererModifierRule>();
	}

	// Token: 0x02000784 RID: 1924
	public enum CallingCardOrigin
	{
		// Token: 0x04003A13 RID: 14867
		createAtScene,
		// Token: 0x04003A14 RID: 14868
		createOnGoToLocation
	}

	// Token: 0x02000785 RID: 1925
	[Serializable]
	public class Graffiti
	{
		// Token: 0x04003A15 RID: 14869
		public InteractablePreset preset;

		// Token: 0x04003A16 RID: 14870
		public MurderMO.Graffiti.GraffitiPosition pos;

		// Token: 0x04003A17 RID: 14871
		[Space(7f)]
		public ArtPreset artImage;

		// Token: 0x04003A18 RID: 14872
		[Space(7f)]
		public string ddsMessageTextList;

		// Token: 0x04003A19 RID: 14873
		public Color color = Color.white;

		// Token: 0x04003A1A RID: 14874
		public float size;

		// Token: 0x02000786 RID: 1926
		public enum GraffitiPosition
		{
			// Token: 0x04003A1C RID: 14876
			victim,
			// Token: 0x04003A1D RID: 14877
			nearbyWall
		}
	}

	// Token: 0x02000787 RID: 1927
	[Serializable]
	public class JobModifier
	{
		// Token: 0x04003A1E RID: 14878
		public List<OccupationPreset> jobs = new List<OccupationPreset>();

		// Token: 0x04003A1F RID: 14879
		[Range(-20f, 20f)]
		public int jobBoost;
	}

	// Token: 0x02000788 RID: 1928
	[Serializable]
	public class CompanyModifier
	{
		// Token: 0x04003A20 RID: 14880
		public List<CompanyPreset> companies = new List<CompanyPreset>();

		// Token: 0x04003A21 RID: 14881
		public int mininumEmployees = 3;

		// Token: 0x04003A22 RID: 14882
		[Range(-20f, 20f)]
		public int companyBoost;

		// Token: 0x04003A23 RID: 14883
		[Tooltip("Add even more for employee count over the minimum")]
		public int boostPerEmployeeOverMinimum = 1;
	}
}
