using System;
using System.Collections.Generic;

// Token: 0x0200066A RID: 1642
public class EvidenceTelephoneCall : EvidenceTime
{
	// Token: 0x06002425 RID: 9253 RVA: 0x001DD2A4 File Offset: 0x001DB4A4
	public EvidenceTelephoneCall(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		string[] array = evID.Split('|', 0);
		if (array.Length > 3)
		{
			string[] array2 = array[3].Split('>', 0);
			if (array2.Length != 0)
			{
				Game.Log("Parsing call from: " + array2[0], 2);
				if (GameplayController.Instance.evidenceDictionary.TryGetValue(array2[0], ref this.callFrom))
				{
					Game.Log("Creating call from fact", 2);
					EvidenceCreator.Instance.CreateFact("CallFrom", this.callFrom, this, null, null, true, null, null, null, false);
				}
			}
			if (array2.Length > 1)
			{
				Game.Log("Parsing call to: " + array2[1], 2);
				if (GameplayController.Instance.evidenceDictionary.TryGetValue(array2[1], ref this.callTo))
				{
					this.callTo = this.callTo.parent;
					Game.Log("Creating call to fact", 2);
					EvidenceCreator.Instance.CreateFact("CallTo", this, this.callTo, null, null, true, null, null, null, false);
				}
			}
		}
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x001DD3A4 File Offset: 0x001DB5A4
	public override void BuildDataSources()
	{
		string text = string.Empty;
		if (this.timeFrom == this.timeTo)
		{
			text += SessionData.Instance.TimeAndDate(this.timeFrom, false, true, true);
			return;
		}
		text = text + SessionData.Instance.TimeAndDate(this.timeFrom, false, true, true) + " — " + SessionData.Instance.TimeAndDate(this.timeTo, false, true, true);
	}

	// Token: 0x04002DFE RID: 11774
	public Evidence callFrom;

	// Token: 0x04002DFF RID: 11775
	public Evidence callTo;
}
