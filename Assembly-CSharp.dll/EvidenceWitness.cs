using System;
using System.Collections.Generic;

// Token: 0x0200066C RID: 1644
public class EvidenceWitness : Evidence
{
	// Token: 0x0600242B RID: 9259 RVA: 0x001DD564 File Offset: 0x001DB764
	public EvidenceWitness(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		foreach (DialogPreset dialogPreset in Toolbox.Instance.defaultDialogOptions)
		{
			if (!dialogPreset.telephoneCallOption && !dialogPreset.hospitalDecisionOption)
			{
				this.AddDialogOption(dialogPreset.tiedToKey, dialogPreset, null, null, true);
			}
		}
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x001DD5EC File Offset: 0x001DB7EC
	public EvidenceWitness.DialogOption AddDialogOption(Evidence.DataKey key, DialogPreset newPreset, SideJob newSideJob = null, NewRoom roomRef = null, bool allowPresetDuplicates = true)
	{
		if (!allowPresetDuplicates && this.dialogOptions.ContainsKey(key))
		{
			foreach (EvidenceWitness.DialogOption dialogOption in this.dialogOptions[key])
			{
				if (dialogOption.preset == newPreset)
				{
					Game.Log(string.Concat(new string[]
					{
						"Found existing dialog ",
						(newPreset != null) ? newPreset.ToString() : null,
						" for key ",
						key.ToString(),
						" in ",
						this.evID
					}), 2);
					return dialogOption;
				}
			}
		}
		EvidenceWitness.DialogOption dialogOption2 = new EvidenceWitness.DialogOption();
		dialogOption2.preset = newPreset;
		dialogOption2.jobRef = newSideJob;
		dialogOption2.roomRef = roomRef;
		if (!this.dialogOptions.ContainsKey(key))
		{
			this.dialogOptions.Add(key, new List<EvidenceWitness.DialogOption>());
		}
		this.dialogOptions[key].Add(dialogOption2);
		return dialogOption2;
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x001DD70C File Offset: 0x001DB90C
	public void RemoveDialogOption(Evidence.DataKey key, DialogPreset newPreset, SideJob newSideJob = null, NewRoom roomRef = null)
	{
		if (this.dialogOptions.ContainsKey(key))
		{
			Game.Log("Remove dialog " + newPreset.name, 2);
			int num = this.dialogOptions[key].FindIndex((EvidenceWitness.DialogOption item) => item.preset == newPreset && item.jobRef == newSideJob && item.roomRef == roomRef);
			if (num > -1)
			{
				this.dialogOptions[key].RemoveAt(num);
			}
		}
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x001DD794 File Offset: 0x001DB994
	public List<EvidenceWitness.DialogOption> GetDialogOptions(Evidence.DataKey key)
	{
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		list.Add(key);
		return this.GetDialogOptions(list);
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x001DD7B8 File Offset: 0x001DB9B8
	public List<EvidenceWitness.DialogOption> GetDialogOptions(List<Evidence.DataKey> keys)
	{
		List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(keys);
		List<EvidenceWitness.DialogOption> list = new List<EvidenceWitness.DialogOption>();
		foreach (Evidence.DataKey dataKey in tiedKeys)
		{
			Game.Log("Debug: Getting dialog options for key: " + dataKey.ToString(), 2);
			if (this.dialogOptions.ContainsKey(dataKey))
			{
				foreach (EvidenceWitness.DialogOption dialogOption in this.dialogOptions[dataKey])
				{
					if (dialogOption.preset.ignoreActiveJobRequirement || dialogOption.jobRef == null || (CasePanelController.Instance.activeCase != null && dialogOption.jobRef == CasePanelController.Instance.activeCase.job))
					{
						if (!list.Contains(dialogOption))
						{
							list.Add(dialogOption);
						}
					}
					else
					{
						string text = "Skipped dialog option with job reference ";
						SideJob jobRef = dialogOption.jobRef;
						Game.Log(text + ((jobRef != null) ? jobRef.ToString() : null), 2);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x001DD8F8 File Offset: 0x001DBAF8
	public override List<Evidence.DataKey> GetTiedKeys(List<Evidence.DataKey> inputKeys)
	{
		inputKeys = base.GetTiedKeys(inputKeys);
		if (inputKeys.Contains(Evidence.DataKey.photo))
		{
			if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.agePerception) > 0f && !inputKeys.Contains(Evidence.DataKey.age))
			{
				inputKeys.Add(Evidence.DataKey.age);
			}
			if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.footSizePerception) > 0f && !inputKeys.Contains(Evidence.DataKey.shoeSize))
			{
				inputKeys.Add(Evidence.DataKey.shoeSize);
			}
			if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.heightPerception) > 0f && !inputKeys.Contains(Evidence.DataKey.height))
			{
				inputKeys.Add(Evidence.DataKey.height);
			}
			if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.salaryPerception) > 0f && !inputKeys.Contains(Evidence.DataKey.salary))
			{
				inputKeys.Add(Evidence.DataKey.salary);
			}
		}
		return inputKeys;
	}

	// Token: 0x04002E03 RID: 11779
	public Dictionary<Evidence.DataKey, List<EvidenceWitness.DialogOption>> dialogOptions = new Dictionary<Evidence.DataKey, List<EvidenceWitness.DialogOption>>();

	// Token: 0x0200066D RID: 1645
	[Serializable]
	public class DialogOption
	{
		// Token: 0x04002E04 RID: 11780
		public DialogPreset preset;

		// Token: 0x04002E05 RID: 11781
		public SideJob jobRef;

		// Token: 0x04002E06 RID: 11782
		public NewRoom roomRef;
	}
}
