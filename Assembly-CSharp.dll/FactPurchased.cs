using System;
using System.Collections.Generic;

// Token: 0x02000678 RID: 1656
public class FactPurchased : Fact
{
	// Token: 0x0600245D RID: 9309 RVA: 0x001DED24 File Offset: 0x001DCF24
	public FactPurchased(FactPreset newPreset, List<Evidence> newFromEvidence, List<Evidence> newToEvidence, List<object> newPassedObjects, List<Evidence.DataKey> overrideFromKeys, List<Evidence.DataKey> overrideToKeys, bool isCustomFact) : base(newPreset, newFromEvidence, newToEvidence, newPassedObjects, overrideFromKeys, overrideToKeys, isCustomFact)
	{
		this.sale = (newPassedObjects[0] as Company.SalesRecord);
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x001DED4C File Offset: 0x001DCF4C
	public override string GenerateNameSuffix()
	{
		if (this.sale == null)
		{
			return string.Empty;
		}
		return string.Concat(new string[]
		{
			" ",
			Strings.Get("evidence.generic", "at", Strings.Casing.asIs, false, false, false, null),
			" ",
			SessionData.Instance.ShortDateString(this.sale.time, false),
			" ",
			SessionData.Instance.GameTimeToClock24String(this.sale.time, false)
		});
	}

	// Token: 0x04002E1A RID: 11802
	public Company.SalesRecord sale;
}
