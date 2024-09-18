using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x02000644 RID: 1604
public class EvidenceBuilding : Evidence
{
	// Token: 0x0600239C RID: 9116 RVA: 0x001D8C5A File Offset: 0x001D6E5A
	public EvidenceBuilding(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		this.building = (newController as NewBuilding);
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x001D8C74 File Offset: 0x001D6E74
	public override string GetNoteComposed(List<Evidence.DataKey> keys, bool useLinks = true)
	{
		Strings.LinkSetting linkSetting = Strings.LinkSetting.forceNoLinks;
		if (useLinks)
		{
			linkSetting = Strings.LinkSetting.forceLinks;
		}
		return Strings.ComposeText(this.GetNote(keys), this.building, linkSetting, keys, null, false);
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x001D8CA0 File Offset: 0x001D6EA0
	public override string GetSummary(List<Evidence.DataKey> keys)
	{
		if (this.building != null && this.building.floors.ContainsKey(0) && this.building.floors[0].addresses.Count > 0)
		{
			return Strings.GetTextForComponent("b4195f2d-aea2-438d-a57f-84a71c4540de", this.building.floors[0].addresses[0], null, null, "\n", false, null, Strings.LinkSetting.automatic, keys);
		}
		return base.GetSummary(keys);
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x001D8D25 File Offset: 0x001D6F25
	public override string GenerateName()
	{
		return this.building.name;
	}

	// Token: 0x060023A0 RID: 9120 RVA: 0x001D8D34 File Offset: 0x001D6F34
	public override string GetNote(List<Evidence.DataKey> keys)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string text = "<sprite=\"icons\" name=\"Checkbox Checked\">";
		string text2 = "<font=\"PapaManAOE SDF\">";
		string text3 = "</font>";
		string text4 = string.Empty;
		text4 = text;
		stringBuilder.Append(text4 + Strings.Get("descriptors", "Type", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (this.building != null && this.building.preset != null)
		{
			stringBuilder.Append(text2 + Strings.Get("names.rooms", this.building.preset.name, Strings.Casing.asIs, false, false, false, null) + text3);
		}
		text4 = text;
		stringBuilder.Append("\n" + text4 + Strings.Get("descriptors", "District", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (this.building != null && this.building.cityTile != null && this.building.cityTile.district != null)
		{
			stringBuilder.Append(text2 + this.building.cityTile.district.name + text3);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002DB8 RID: 11704
	public NewBuilding building;
}
