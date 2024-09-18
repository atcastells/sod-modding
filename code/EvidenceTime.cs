using System;
using System.Collections.Generic;

// Token: 0x0200066B RID: 1643
public class EvidenceTime : Evidence
{
	// Token: 0x06002427 RID: 9255 RVA: 0x001DD414 File Offset: 0x001DB614
	public EvidenceTime(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		string[] array = evID.Split('|', 0);
		if (array.Length > 1)
		{
			float.TryParse(array[1], ref this.timeFrom);
		}
		if (array.Length > 2)
		{
			float.TryParse(array[2], ref this.timeTo);
		}
		if (array.Length > 4)
		{
			int id = -1;
			int.TryParse(array[4], ref id);
			CityData.Instance.GetHuman(id, out this.writer, true);
		}
		if (array.Length > 5)
		{
			int id2 = -1;
			int.TryParse(array[5], ref id2);
			CityData.Instance.GetHuman(id2, out this.reciever, true);
		}
		float newTime = this.timeTo - this.timeFrom;
		this.duration = SessionData.Instance.DecimalToTimeLengthString(newTime);
		GameplayController.Instance.timeEvidence.Add(this);
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x00002265 File Offset: 0x00000465
	public override void BuildDataSources()
	{
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x001DD4DC File Offset: 0x001DB6DC
	public override string GenerateName()
	{
		string text = string.Empty;
		if (this.timeFrom == this.timeTo)
		{
			text = SessionData.Instance.TimeAndDate(this.timeFrom, false, true, true);
		}
		else
		{
			text = SessionData.Instance.TimeAndDate(this.timeFrom, false, true, true) + " — " + SessionData.Instance.TimeAndDate(this.timeTo, false, true, true);
		}
		base.SetNote(Toolbox.Instance.allDataKeys, text);
		return text;
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x001DD556 File Offset: 0x001DB756
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		this.UpdateName();
	}

	// Token: 0x04002E00 RID: 11776
	public float timeFrom;

	// Token: 0x04002E01 RID: 11777
	public float timeTo;

	// Token: 0x04002E02 RID: 11778
	public string duration;
}
