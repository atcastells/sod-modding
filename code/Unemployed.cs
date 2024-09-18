using System;

// Token: 0x020000E3 RID: 227
public class Unemployed
{
	// Token: 0x040006B0 RID: 1712
	public Unemployed.UnemployedType type;

	// Token: 0x040006B1 RID: 1713
	public float stateSalary = 6f;

	// Token: 0x020000E4 RID: 228
	public enum UnemployedType
	{
		// Token: 0x040006B3 RID: 1715
		Student,
		// Token: 0x040006B4 RID: 1716
		Retired,
		// Token: 0x040006B5 RID: 1717
		Unemployed,
		// Token: 0x040006B6 RID: 1718
		Prison
	}
}
