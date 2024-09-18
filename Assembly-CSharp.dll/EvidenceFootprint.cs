using System;
using System.Collections.Generic;

// Token: 0x0200064D RID: 1613
public class EvidenceFootprint : Evidence
{
	// Token: 0x060023C4 RID: 9156 RVA: 0x001DAA28 File Offset: 0x001D8C28
	public EvidenceFootprint(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x001DB030 File Offset: 0x001D9230
	public override void OnDiscovery()
	{
		base.OnDiscovery();
		Game.Log("Evidence: Footprint creating 'found at' evidence...", 2);
		EvidenceCreator.Instance.CreateFact("FoundAt", this, this.parent, null, null, true, null, null, null, false);
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x001DB06C File Offset: 0x001D926C
	public override void BuildDataSources()
	{
		this.UpdateSummary();
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x001DB074 File Offset: 0x001D9274
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

	// Token: 0x060023C8 RID: 9160 RVA: 0x001DB130 File Offset: 0x001D9330
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
				if (this.customNames.Count > 0)
				{
					List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(inputKeys);
					foreach (Evidence.CustomName customName in this.customNames)
					{
						if (tiedKeys.Contains(customName.key))
						{
							return customName.name;
						}
					}
				}
				text = string.Concat(new string[]
				{
					Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null),
					" (",
					Strings.Get("evidence.generic", this.interactable.belongsTo.descriptors.footwear.ToString(), Strings.Casing.firstLetterCaptial, false, false, false, null),
					", ",
					Strings.Get("evidence.generic", "size", Strings.Casing.asIs, false, false, false, null),
					" ",
					this.interactable.belongsTo.descriptors.shoeSize.ToString(),
					")"
				});
				result = text;
			}
		}
		catch
		{
			result = text;
		}
		return result;
	}
}
