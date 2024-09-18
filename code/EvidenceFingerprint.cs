using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x0200064B RID: 1611
public class EvidenceFingerprint : Evidence
{
	// Token: 0x060023B9 RID: 9145 RVA: 0x001DAA28 File Offset: 0x001D8C28
	public EvidenceFingerprint(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x001DAA38 File Offset: 0x001D8C38
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		if (this.writer != null && this.writer.evidenceEntry != null)
		{
			this.OnCitizensDataKeyChange();
			this.writer.evidenceEntry.OnDataKeyChange += this.OnCitizensDataKeyChange;
		}
		if (this.interactable == null || this.interactable.print == null || this.interactable.print.Count <= 0)
		{
			Game.Log("Evidence: Fingerprint creating 'found at' evidence...", 2);
			EvidenceCreator.Instance.CreateFact("FoundAt", this, this.parent, null, null, true, null, null, null, false);
			return;
		}
		Interactable interactable = null;
		if (!CityData.Instance.savableInteractableDictionary.TryGetValue(this.interactable.print[0].interactableID, ref interactable))
		{
			Game.Log("Evidence: Fingerprint creating 'found at' evidence...", 2);
			EvidenceCreator.Instance.CreateFact("FoundAt", this, this.parent, null, null, true, null, null, null, false);
			return;
		}
		if (interactable.evidence != null)
		{
			Game.Log("Evidence: Fingerprint creating 'found on' evidence...", 2);
			EvidenceCreator.Instance.CreateFact("FoundOn", this, interactable.evidence, null, null, true, null, null, null, false);
			return;
		}
		Game.Log("Evidence: Fingerprint creating 'found at' evidence...", 2);
		EvidenceCreator.Instance.CreateFact("FoundAt", this, this.parent, null, null, true, null, null, null, false);
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x001DAB93 File Offset: 0x001D8D93
	public override void BuildDataSources()
	{
		this.UpdateSummary();
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x001DAB9C File Offset: 0x001D8D9C
	public void UpdateSummary()
	{
		foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows.FindAll((InfoWindow item) => item.passedEvidence == this))
		{
			foreach (WindowTabController windowTabController in infoWindow.tabs)
			{
				if (infoWindow.passedEvidence != null && windowTabController.preset.contentType == WindowTabPreset.TabContentType.generated)
				{
					windowTabController.content.LoadContent();
				}
			}
		}
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x001DAC58 File Offset: 0x001D8E58
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
				text = Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
				List<Evidence.DataKey> tiedKeys2 = base.GetTiedKeys(Evidence.DataKey.name);
				if (tiedKeys.Contains(Evidence.DataKey.name) || (tiedKeys2.Exists((Evidence.DataKey item) => inputKeys.Contains(item)) && this.writer != null))
				{
					text = text + " (" + this.writer.GetCitizenName() + ")";
				}
				else if (this.writer != null)
				{
					text = string.Concat(new string[]
					{
						text,
						" (",
						Strings.Get("evidence.generic", "Type", Strings.Casing.firstLetterCaptial, false, false, false, null),
						" ",
						Toolbox.Instance.ToBase26(this.writer.fingerprintLoop),
						")"
					});
				}
				else
				{
					Game.LogError("Missing writer for fingerprint " + this.evID, 2);
				}
				result = text;
			}
		}
		catch
		{
			result = text;
		}
		return result;
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x001DAE34 File Offset: 0x001D9034
	public void OnCitizensDataKeyChange()
	{
		if (this.writer.evidenceEntry.GetTiedKeys(Evidence.DataKey.name).Contains(Evidence.DataKey.fingerprints))
		{
			this.MergeDataKeys(Evidence.DataKey.fingerprints, Evidence.DataKey.name);
			this.UpdateSummary();
			if (this.interactable != null)
			{
				this.interactable.UpdateName(false, Evidence.DataKey.name);
			}
		}
		if (this.interactable != null)
		{
			this.interactable.UpdateName(true, Evidence.DataKey.fingerprints);
			return;
		}
		Game.Log("No print interactable", 2);
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x001DAEA0 File Offset: 0x001D90A0
	public override string GetNoteComposed(List<Evidence.DataKey> keys, bool useLinks = true)
	{
		Strings.LinkSetting linkSetting = Strings.LinkSetting.forceNoLinks;
		if (useLinks)
		{
			linkSetting = Strings.LinkSetting.forceLinks;
		}
		return Strings.ComposeText(this.GetNote(keys), this.writer, linkSetting, keys, null, false);
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x001DAECC File Offset: 0x001D90CC
	public override string GetNote(List<Evidence.DataKey> keys)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(keys);
		string text = "<sprite=\"icons\" name=\"Checkbox Empty\">";
		string text2 = "<sprite=\"icons\" name=\"Checkbox Checked\">";
		string text3 = "<font=\"PapaManAOE SDF\">";
		string text4 = "</font>";
		string text5 = string.Empty;
		if (tiedKeys.Contains(Evidence.DataKey.fingerprints))
		{
			text5 = text2;
		}
		else
		{
			text5 = text;
		}
		stringBuilder.Append(text5 + Strings.Get("descriptors", "Type", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (tiedKeys.Contains(Evidence.DataKey.fingerprints) && this.writer != null)
		{
			stringBuilder.Append(text3 + Toolbox.Instance.ToBase26(this.writer.fingerprintLoop) + text4);
		}
		else
		{
			stringBuilder.Append(text3 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text4);
		}
		if (tiedKeys.Contains(Evidence.DataKey.name))
		{
			text5 = text2;
		}
		else
		{
			text5 = text;
		}
		stringBuilder.Append("\n" + text5 + Strings.Get("descriptors", "Belongs To", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (tiedKeys.Contains(Evidence.DataKey.name))
		{
			stringBuilder.Append(text3 + "|name|" + text4);
		}
		else
		{
			stringBuilder.Append(text3 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text4);
		}
		return stringBuilder.ToString();
	}
}
