using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000666 RID: 1638
public class EvidenceSalesNote : Evidence
{
	// Token: 0x06002416 RID: 9238 RVA: 0x001DCBA8 File Offset: 0x001DADA8
	public EvidenceSalesNote(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		foreach (object obj in newPassedObjects)
		{
			Interactable.Passed passed = obj as Interactable.Passed;
			if (passed != null && passed.varType == Interactable.PassedVarType.addressID && CityData.Instance.addressDictionary.TryGetValue(Mathf.RoundToInt(passed.value), ref this.forSale) && this.interactable != null)
			{
				string textForComponent = Strings.GetTextForComponent("600d4a18-7306-4871-a68e-e7764ae62f81", this.interactable, null, null, "\n", false, null, Strings.LinkSetting.automatic, null);
				base.SetNote(Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[1]), textForComponent);
			}
		}
	}

	// Token: 0x04002DF9 RID: 11769
	public NewAddress forSale;
}
