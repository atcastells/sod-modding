using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006ED RID: 1773
[CreateAssetMenu(fileName = "company_structure_data", menuName = "Database/Company/Structure Preset")]
public class CompanyStructurePreset : SoCustomComparison
{
	// Token: 0x04003307 RID: 13063
	[Header("Company Structure")]
	public CompanyStructurePreset.BossConfig companyStructure;

	// Token: 0x020006EE RID: 1774
	[Serializable]
	public class OccupationSettings
	{
		// Token: 0x04003308 RID: 13064
		public OccupationPreset occupation;

		// Token: 0x04003309 RID: 13065
		public int positionsMinimum = 1;

		// Token: 0x0400330A RID: 13066
		public int positionsMaximum = 1;

		// Token: 0x0400330B RID: 13067
		[Range(0f, 1f)]
		public float payGrade = 0.5f;
	}

	// Token: 0x020006EF RID: 1775
	[Serializable]
	public class BossConfig : CompanyStructurePreset.OccupationSettings
	{
		// Token: 0x0400330C RID: 13068
		[Header("Is Boss Of...")]
		public List<CompanyStructurePreset.Hierarchy1Config> subordinates = new List<CompanyStructurePreset.Hierarchy1Config>();
	}

	// Token: 0x020006F0 RID: 1776
	[Serializable]
	public class Hierarchy1Config : CompanyStructurePreset.OccupationSettings
	{
		// Token: 0x0400330D RID: 13069
		[Header("Is Boss Of...")]
		public List<CompanyStructurePreset.Hierarchy2Config> subordinates = new List<CompanyStructurePreset.Hierarchy2Config>();
	}

	// Token: 0x020006F1 RID: 1777
	[Serializable]
	public class Hierarchy2Config : CompanyStructurePreset.OccupationSettings
	{
		// Token: 0x0400330E RID: 13070
		[Header("Is Boss Of...")]
		public List<CompanyStructurePreset.Hierarchy3Config> subordinates = new List<CompanyStructurePreset.Hierarchy3Config>();
	}

	// Token: 0x020006F2 RID: 1778
	[Serializable]
	public class Hierarchy3Config : CompanyStructurePreset.OccupationSettings
	{
		// Token: 0x0400330F RID: 13071
		[Header("Is Boss Of...")]
		public List<CompanyStructurePreset.OccupationSettings> subordinates = new List<CompanyStructurePreset.OccupationSettings>();
	}

	// Token: 0x020006F3 RID: 1779
	[Serializable]
	public class Hierarchy4Config : CompanyStructurePreset.OccupationSettings
	{
	}
}
