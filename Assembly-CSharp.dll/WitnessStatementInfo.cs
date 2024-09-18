using System;

// Token: 0x02000622 RID: 1570
public class WitnessStatementInfo
{
	// Token: 0x060022DE RID: 8926 RVA: 0x001D4ED6 File Offset: 0x001D30D6
	public WitnessStatementInfo(Citizen newCit, WitnessStatementInfo.StatementType newStatementType)
	{
		this.citizen = newCit;
		this.statementType = newStatementType;
	}

	// Token: 0x04002D34 RID: 11572
	public Citizen citizen;

	// Token: 0x04002D35 RID: 11573
	public WitnessStatementInfo.StatementType statementType;

	// Token: 0x02000623 RID: 1571
	public enum StatementType
	{
		// Token: 0x04002D37 RID: 11575
		Alibi,
		// Token: 0x04002D38 RID: 11576
		knowVictim
	}
}
