using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x0200064F RID: 1615
public class EvidenceLocation : Evidence
{
	// Token: 0x060023CB RID: 9163 RVA: 0x001DB2FB File Offset: 0x001D94FB
	public EvidenceLocation(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		this.locationController = (newController as NewGameLocation);
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x001DB314 File Offset: 0x001D9514
	public override void Compile()
	{
		base.Compile();
		if (this.locationController != null && (!(this.locationController.thisAsAddress != null) || !(this.locationController.thisAsAddress.addressPreset != null) || this.locationController.thisAsAddress.addressPreset.playerKnowsPurpose))
		{
			this.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.purpose);
			this.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.location);
			if (this.locationController.thisAsStreet != null)
			{
				this.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.photo);
				this.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.blueprints);
			}
		}
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x001DB3AD File Offset: 0x001D95AD
	public override void MergeDataKeys(Evidence.DataKey keyOne, Evidence.DataKey keyTwo)
	{
		base.MergeDataKeys(keyOne, keyTwo);
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x001DB3B8 File Offset: 0x001D95B8
	public void OnPlayerArrival()
	{
		if (!this.keyTies[Evidence.DataKey.name].Contains(Evidence.DataKey.purpose))
		{
			this.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.purpose);
		}
		if (!this.keyTies[Evidence.DataKey.name].Contains(Evidence.DataKey.location))
		{
			this.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.location);
		}
		if (!this.keyTies[Evidence.DataKey.name].Contains(Evidence.DataKey.blueprints))
		{
			this.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.blueprints);
		}
		if (!this.keyTies[Evidence.DataKey.name].Contains(Evidence.DataKey.photo))
		{
			this.MergeDataKeys(Evidence.DataKey.name, Evidence.DataKey.photo);
		}
		if (this.locationController.mapButton != null)
		{
			MapController.Instance.AddUpdateCall(this.locationController.mapButton);
		}
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x001DB464 File Offset: 0x001D9664
	public override string GetNoteComposed(List<Evidence.DataKey> keys, bool useLinks = true)
	{
		Strings.LinkSetting linkSetting = Strings.LinkSetting.forceNoLinks;
		if (useLinks)
		{
			linkSetting = Strings.LinkSetting.forceLinks;
		}
		return Strings.ComposeText(this.GetNote(keys), this.locationController, linkSetting, keys, null, false);
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x001DB490 File Offset: 0x001D9690
	public override string GetSummary(List<Evidence.DataKey> keys)
	{
		if (this.locationController.thisAsAddress != null)
		{
			return Strings.GetTextForComponent("ea090069-a4a6-45d4-b20b-362146677052", this.locationController, null, null, "\n", false, null, Strings.LinkSetting.automatic, keys);
		}
		if (this.locationController.thisAsStreet != null)
		{
			return Strings.GetTextForComponent("5dd6c421-d4e1-4c53-afab-097cc1c6df13", this.locationController, null, null, "\n", false, null, Strings.LinkSetting.automatic, keys);
		}
		return base.GetSummary(keys);
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x001DB502 File Offset: 0x001D9702
	public override string GenerateName()
	{
		return this.controller.name;
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x001DB510 File Offset: 0x001D9710
	public override string GetNameForDataKey(List<Evidence.DataKey> inputKeys)
	{
		string text = string.Empty;
		string result;
		try
		{
			if (inputKeys == null)
			{
				result = Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(inputKeys);
				if (this.customNames.Count > 0)
				{
					foreach (Evidence.CustomName customName in this.customNames)
					{
						if (tiedKeys.Contains(customName.key))
						{
							return customName.name;
						}
					}
				}
				text = string.Empty;
				List<Evidence.DataKey> tiedKeys2 = base.GetTiedKeys(Evidence.DataKey.name);
				if (tiedKeys.Contains(Evidence.DataKey.name) || tiedKeys2.Exists((Evidence.DataKey item) => inputKeys.Contains(item)))
				{
					result = this.name;
				}
				else
				{
					text = Strings.Get("evidence.generic", "Unknown", Strings.Casing.firstLetterCaptial, false, false, false, null) + " " + Strings.Get("evidence.generic", "Address", Strings.Casing.firstLetterCaptial, false, false, false, null);
					result = text;
				}
			}
		}
		catch
		{
			result = text;
		}
		return result;
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x001DB654 File Offset: 0x001D9854
	public override string GetNote(List<Evidence.DataKey> keys)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(keys);
		string text = "<sprite=\"icons\" name=\"Checkbox Empty\">";
		string text2 = "<sprite=\"icons\" name=\"Checkbox Checked\">";
		string text3 = "<font=\"PapaManAOE SDF\">";
		string text4 = "</font>";
		string text5 = string.Empty;
		if (tiedKeys.Contains(Evidence.DataKey.purpose))
		{
			text5 = text2;
		}
		else
		{
			text5 = text;
		}
		stringBuilder.Append(text5 + Strings.Get("descriptors", "Type", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (tiedKeys.Contains(Evidence.DataKey.purpose))
		{
			stringBuilder.Append(text3 + "|type|" + text4);
		}
		else
		{
			stringBuilder.Append(text3 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text4);
		}
		if (tiedKeys.Contains(Evidence.DataKey.location))
		{
			text5 = text2;
		}
		else
		{
			text5 = text;
		}
		stringBuilder.Append("\n" + text5 + Strings.Get("descriptors", "Location", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (tiedKeys.Contains(Evidence.DataKey.location))
		{
			stringBuilder.Append(text3 + "|building|" + text4);
		}
		else
		{
			stringBuilder.Append(text3 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text4);
		}
		if (this.locationController != null && this.locationController.thisAsAddress != null)
		{
			if (tiedKeys.Contains(Evidence.DataKey.blueprints))
			{
				text5 = text2;
			}
			else
			{
				text5 = text;
			}
			stringBuilder.Append("\n" + text5 + Strings.Get("descriptors", "Blueprints", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
			if (tiedKeys.Contains(Evidence.DataKey.blueprints))
			{
				stringBuilder.Append(text3 + Strings.Get("descriptors", "Yes", Strings.Casing.firstLetterCaptial, false, false, false, null) + text4);
			}
			else
			{
				stringBuilder.Append(text3 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text4);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002DC6 RID: 11718
	public NewGameLocation locationController;
}
