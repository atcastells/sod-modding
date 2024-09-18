using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006E6 RID: 1766
[CreateAssetMenu(fileName = "company_open_hours_data", menuName = "Database/Company/Open Hours Preset")]
public class CompanyOpenHoursPreset : SoCustomComparison
{
	// Token: 0x040032B0 RID: 12976
	[Header("Opening Hours")]
	[Tooltip("Hours of retail opening hours")]
	public Vector2 retailOpenHours = new Vector2(8f, 17f);

	// Token: 0x040032B1 RID: 12977
	[Header("Days Open")]
	public bool monday = true;

	// Token: 0x040032B2 RID: 12978
	public bool tuesday = true;

	// Token: 0x040032B3 RID: 12979
	public bool wednesday = true;

	// Token: 0x040032B4 RID: 12980
	public bool thursday = true;

	// Token: 0x040032B5 RID: 12981
	public bool friday = true;

	// Token: 0x040032B6 RID: 12982
	public bool saturday = true;

	// Token: 0x040032B7 RID: 12983
	public bool sunday;

	// Token: 0x040032B8 RID: 12984
	[Header("Work Hours")]
	[ReorderableList]
	public List<CompanyOpenHoursPreset.CompanyShift> shifts = new List<CompanyOpenHoursPreset.CompanyShift>();

	// Token: 0x020006E7 RID: 1767
	[Serializable]
	public class CompanyShift
	{
		// Token: 0x040032B9 RID: 12985
		public string name = "shift";

		// Token: 0x040032BA RID: 12986
		public OccupationPreset.ShiftType shiftType = OccupationPreset.ShiftType.dayShift;

		// Token: 0x040032BB RID: 12987
		public Vector2 decimalHours = new Vector2(9f, 17f);

		// Token: 0x040032BC RID: 12988
		public bool monday = true;

		// Token: 0x040032BD RID: 12989
		public bool tuesday = true;

		// Token: 0x040032BE RID: 12990
		public bool wednesday = true;

		// Token: 0x040032BF RID: 12991
		public bool thursday = true;

		// Token: 0x040032C0 RID: 12992
		public bool friday = true;

		// Token: 0x040032C1 RID: 12993
		public bool saturday;

		// Token: 0x040032C2 RID: 12994
		public bool sunday;

		// Token: 0x040032C3 RID: 12995
		[NonSerialized]
		public List<Occupation> assigned = new List<Occupation>();

		// Token: 0x040032C4 RID: 12996
		[ReadOnly]
		public int debugAssigned;
	}
}
