using System;
using System.Collections.Generic;

// Token: 0x02000661 RID: 1633
public class EvidencePrintedVmail : Evidence
{
	// Token: 0x06002408 RID: 9224 RVA: 0x001DC2D4 File Offset: 0x001DA4D4
	public EvidencePrintedVmail(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		for (int i = 0; i < newPassedObjects.Count; i++)
		{
			object obj = newPassedObjects[i];
			Interactable.Passed passed = obj as Interactable.Passed;
			if (passed != null)
			{
				if (passed.varType == Interactable.PassedVarType.vmailThreadID)
				{
					this.threadID = (int)passed.value;
					if (!GameplayController.Instance.messageThreads.TryGetValue((int)passed.value, ref this.thread))
					{
						Game.LogError(string.Concat(new string[]
						{
							"Unable to find message thread ",
							((int)passed.value).ToString(),
							" (message threads: ",
							GameplayController.Instance.messageThreads.Count.ToString(),
							")"
						}), 2);
					}
				}
				else if (passed.varType == Interactable.PassedVarType.vmailThreadMsgIndex)
				{
					this.msgIndexID = (int)passed.value;
				}
			}
			Interactable interactable = obj as Interactable;
			if (interactable != null)
			{
				this.interactable = interactable;
				this.interactablePreset = this.interactable.preset;
			}
		}
	}

	// Token: 0x04002DE9 RID: 11753
	public int threadID;

	// Token: 0x04002DEA RID: 11754
	public int msgIndexID;

	// Token: 0x04002DEB RID: 11755
	public StateSaveData.MessageThreadSave thread;
}
