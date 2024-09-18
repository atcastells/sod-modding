using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006FE RID: 1790
[CreateAssetMenu(fileName = "scope_data", menuName = "Database/DDS Scope")]
public class DDSScope : SoCustomComparison
{
	// Token: 0x0400335C RID: 13148
	[Header("Setup")]
	public Color colour = Color.white;

	// Token: 0x0400335D RID: 13149
	[Tooltip("This can be accessed from any scope")]
	public bool isGlobal;

	// Token: 0x0400335E RID: 13150
	public DDSScope.SpecialCase specialCase;

	// Token: 0x0400335F RID: 13151
	[Header("Content")]
	public List<DDSScope.ContainedScope> containedScopes = new List<DDSScope.ContainedScope>();

	// Token: 0x04003360 RID: 13152
	public List<string> containedValues = new List<string>();

	// Token: 0x020006FF RID: 1791
	public enum SpecialCase
	{
		// Token: 0x04003362 RID: 13154
		none
	}

	// Token: 0x02000700 RID: 1792
	[Serializable]
	public class ContainedScope
	{
		// Token: 0x04003363 RID: 13155
		public string name;

		// Token: 0x04003364 RID: 13156
		public DDSScope type;
	}
}
