using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AD RID: 685
public class VMailApp : CruncherAppContent
{
	// Token: 0x06000F4A RID: 3914 RVA: 0x000DA0F4 File Offset: 0x000D82F4
	public override void OnSetup()
	{
		base.OnSetup();
		if (this.controller.loggedInAs != null)
		{
			Game.Log("Player: Retrieving " + this.controller.loggedInAs.messageThreadFeatures.Count.ToString() + " message threads...", 2);
			List<StateSaveData.MessageThreadSave> list = new List<StateSaveData.MessageThreadSave>();
			foreach (StateSaveData.MessageThreadSave messageThreadSave in this.controller.loggedInAs.messageThreadFeatures)
			{
				if (!list.Contains(messageThreadSave))
				{
					DDSSaveClasses.DDSTreeSave ddstreeSave = Toolbox.Instance.allDDSTrees[messageThreadSave.treeID];
					if ((messageThreadSave.participantA != this.controller.loggedInAs.humanID || ddstreeSave.participantA == null || !ddstreeSave.participantA.disableInbox) && (messageThreadSave.participantB != this.controller.loggedInAs.humanID || ddstreeSave.participantB == null || !ddstreeSave.participantB.disableInbox) && (messageThreadSave.participantC != this.controller.loggedInAs.humanID || ddstreeSave.participantC == null || !ddstreeSave.participantC.disableInbox) && (messageThreadSave.participantD != this.controller.loggedInAs.humanID || ddstreeSave.participantD == null || !ddstreeSave.participantD.disableInbox))
					{
						list.Add(messageThreadSave);
					}
				}
			}
			foreach (StateSaveData.MessageThreadSave messageThreadSave2 in this.controller.loggedInAs.messageThreadCCd)
			{
				if (!list.Contains(messageThreadSave2))
				{
					DDSSaveClasses.DDSTreeSave ddstreeSave2 = Toolbox.Instance.allDDSTrees[messageThreadSave2.treeID];
					if ((messageThreadSave2.participantA != this.controller.loggedInAs.humanID || ddstreeSave2.participantA == null || !ddstreeSave2.participantA.disableInbox) && (messageThreadSave2.participantB != this.controller.loggedInAs.humanID || ddstreeSave2.participantB == null || !ddstreeSave2.participantB.disableInbox) && (messageThreadSave2.participantC != this.controller.loggedInAs.humanID || ddstreeSave2.participantC == null || !ddstreeSave2.participantC.disableInbox) && (messageThreadSave2.participantD != this.controller.loggedInAs.humanID || ddstreeSave2.participantD == null || !ddstreeSave2.participantD.disableInbox))
					{
						list.Add(messageThreadSave2);
					}
				}
			}
			try
			{
				list.Sort((StateSaveData.MessageThreadSave p2, StateSaveData.MessageThreadSave p1) => p1.timestamps[p1.timestamps.Count - 1].CompareTo(p2.timestamps[p2.timestamps.Count - 1]));
			}
			catch
			{
			}
			List<ComputerOSMultiSelect.OSMultiOption> list2 = new List<ComputerOSMultiSelect.OSMultiOption>();
			foreach (StateSaveData.MessageThreadSave messageThreadSave3 in list)
			{
				DDSSaveClasses.DDSTreeSave ddstreeSave3 = Toolbox.Instance.allDDSTrees[messageThreadSave3.treeID];
				Acquaintance aq = null;
				Human findC = null;
				if (messageThreadSave3.recievers == null)
				{
					messageThreadSave3.recievers = new List<int>();
				}
				if (messageThreadSave3.recievers.Count > 0)
				{
					CityData.Instance.GetHuman(messageThreadSave3.recievers[0], out findC, true);
				}
				Human human = null;
				if (CityData.Instance.GetHuman(messageThreadSave3.participantA, out human, true))
				{
					human.FindAcquaintanceExists(findC, out aq);
				}
				if (messageThreadSave3.messages == null || messageThreadSave3.messages.Count <= 0)
				{
					Game.Log("Trying to retrieve vmail thread with no messages...", 2);
				}
				else
				{
					DDSSaveClasses.DDSMessageSettings ddsmessageSettings = ddstreeSave3.messageRef[messageThreadSave3.messages[0]];
					Game.Log("Player: Retrieving " + ddsmessageSettings.msgID + "...", 2);
					List<int> list4;
					List<string> list3 = this.controller.loggedInAs.ParseDDSMessage(ddsmessageSettings.msgID, aq, out list4, false, null, false);
					for (int i = messageThreadSave3.messages.Count - 1; i >= 0; i--)
					{
						string empty = string.Empty;
						Human vmailSender = Strings.GetVmailSender(messageThreadSave3, i, out empty);
						string text = Strings.Get("dds.blocks", list3[0], Strings.Casing.asIs, false, false, true, vmailSender);
						if (i > 0)
						{
							text = Strings.Get("computer", "RE", Strings.Casing.asIs, false, false, false, null) + ": " + text;
						}
						if (i != messageThreadSave3.messages.Count - 1)
						{
							text = ">" + text;
						}
						text = text + "\n<alpha=#AA>" + empty;
						ComputerOSMultiSelect.OSMultiOption osmultiOption = new ComputerOSMultiSelect.OSMultiOption
						{
							text = text,
							msgThread = messageThreadSave3,
							msgIndex = i
						};
						list2.Add(osmultiOption);
					}
				}
			}
			this.vmailList.UpdateElements(list2);
		}
		else
		{
			Game.Log("Player: Nobody is logged in on this machine.", 2);
		}
		this.SetSelectedVmail(null);
		this.vmailList.OnNewSelection += this.OnUpdatedSelection;
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x000DA684 File Offset: 0x000D8884
	private void OnDestroy()
	{
		this.vmailList.OnNewSelection -= this.OnUpdatedSelection;
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x000DA69D File Offset: 0x000D889D
	public void OnUpdatedSelection()
	{
		this.SetSelectedVmail(this.vmailList.selected);
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x000DA6B0 File Offset: 0x000D88B0
	public void SetSelectedVmail(ComputerOSMultiSelectElement newSelection)
	{
		this.selectedVmailElement = newSelection;
		this.vmailBodyText.pageToDisplay = 0;
		if (this.selectedVmailElement == null)
		{
			this.vmailHeaderText.text = string.Empty;
			this.vmailBodyText.text = string.Empty;
			this.selectedThread = null;
			return;
		}
		this.thread = newSelection.option.msgThread;
		this.msgIndex = newSelection.option.msgIndex;
		string empty = string.Empty;
		string empty2 = string.Empty;
		this.emailSender = Strings.GetVmailSender(this.thread, this.msgIndex, out empty2);
		this.emailReciever = Strings.GetVmailReciever(this.thread, this.msgIndex, out empty);
		this.vmailHeaderText.text = string.Concat(new string[]
		{
			Strings.Get("computer", "To", Strings.Casing.asIs, false, false, false, null),
			": ",
			empty,
			"\n",
			Strings.Get("computer", "From", Strings.Casing.asIs, false, false, false, null),
			": ",
			empty2,
			"\n",
			SessionData.Instance.GameTimeToClock12String(this.thread.timestamps[this.msgIndex], false),
			"\n",
			SessionData.Instance.LongDateString(this.thread.timestamps[this.msgIndex], true, true, true, true, true, true, false, false)
		});
		this.tree = Toolbox.Instance.allDDSTrees[this.selectedVmailElement.option.msgThread.treeID];
		Human human = null;
		Human human2 = null;
		Human human3 = null;
		Human human4 = null;
		CityData.Instance.GetHuman(this.thread.participantA, out human, true);
		CityData.Instance.GetHuman(this.thread.participantB, out human2, true);
		CityData.Instance.GetHuman(this.thread.participantC, out human3, true);
		CityData.Instance.GetHuman(this.thread.participantD, out human4, true);
		this.selectedThread = this.selectedVmailElement.option.msgThread;
		this.vmailBodyText.text = Strings.GetTextForComponent(this.tree.messageRef[this.selectedThread.messages[this.msgIndex]].msgID, new VMailApp.VmailParsingData
		{
			thread = this.thread,
			messageIndex = this.msgIndex
		}, this.emailSender, this.emailReciever, "\n\n", true, null, Strings.LinkSetting.automatic, null);
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x000DA948 File Offset: 0x000D8B48
	public void NextButton()
	{
		TextMeshProUGUI textMeshProUGUI = this.vmailBodyText;
		int pageToDisplay = textMeshProUGUI.pageToDisplay;
		textMeshProUGUI.pageToDisplay = pageToDisplay + 1;
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x000DA96C File Offset: 0x000D8B6C
	public void PrevButton()
	{
		TextMeshProUGUI textMeshProUGUI = this.vmailBodyText;
		int pageToDisplay = textMeshProUGUI.pageToDisplay;
		textMeshProUGUI.pageToDisplay = pageToDisplay - 1;
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x000D6564 File Offset: 0x000D4764
	public void ExitButton()
	{
		this.controller.OnAppExit();
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x000DA990 File Offset: 0x000D8B90
	public override void PrintButton()
	{
		if (this.controller.printedDocument == null && this.selectedThread != null && this.controller.printTimer <= 0f && this.tree != null)
		{
			this.controller.SetTimedLoading(Toolbox.Instance.Rand(0.5f, 1f, false), 0.33f);
			this.controller.printTimer = 1f;
			this.controller.printerParent.localPosition = new Vector3(this.controller.printerParent.localPosition.x, this.controller.printerParent.localPosition.y, -0.05f);
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerPrint, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			Human recevier = null;
			CityData.Instance.GetHuman(this.selectedThread.participantA, out recevier, true);
			Human writer = null;
			CityData.Instance.GetHuman(this.selectedThread.participantB, out writer, true);
			try
			{
				List<Interactable.Passed> list = new List<Interactable.Passed>();
				Interactable.Passed passed = new Interactable.Passed(Interactable.PassedVarType.vmailThreadID, (float)this.selectedThread.threadID, null);
				list.Add(passed);
				Interactable.Passed passed2 = new Interactable.Passed(Interactable.PassedVarType.vmailThreadMsgIndex, (float)this.msgIndex, null);
				list.Add(passed2);
				Game.Log("Print vmail: " + this.selectedThread.threadID.ToString(), 2);
				this.controller.printedDocument = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.vmailPrintout, Player.Instance, writer, recevier, this.controller.printerParent.position, this.controller.ic.transform.eulerAngles, list, null, this.selectedVmailElement.option.msgThread.treeID);
				if (this.controller.printedDocument != null)
				{
					this.controller.printedDocument.MarkAsTrash(true, false, 0f);
				}
				this.controller.printedDocument.OnRemovedFromWorld += base.OnPlayerTakePrint;
				return;
			}
			catch
			{
				Game.LogError("Unable to print message index", 2);
				return;
			}
		}
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerInvalidPasscode, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
	}

	// Token: 0x04001294 RID: 4756
	public ComputerOSMultiSelect vmailList;

	// Token: 0x04001295 RID: 4757
	public ComputerOSMultiSelectElement selectedVmailElement;

	// Token: 0x04001296 RID: 4758
	private StateSaveData.MessageThreadSave selectedThread;

	// Token: 0x04001297 RID: 4759
	public TextMeshProUGUI vmailHeaderText;

	// Token: 0x04001298 RID: 4760
	public TextMeshProUGUI vmailBodyText;

	// Token: 0x04001299 RID: 4761
	public Button nextPageButton;

	// Token: 0x0400129A RID: 4762
	public Button prevPageButton;

	// Token: 0x0400129B RID: 4763
	public Human emailSender;

	// Token: 0x0400129C RID: 4764
	public Human emailReciever;

	// Token: 0x0400129D RID: 4765
	public string emailTextContent;

	// Token: 0x0400129E RID: 4766
	private DDSSaveClasses.DDSTreeSave tree;

	// Token: 0x0400129F RID: 4767
	private StateSaveData.MessageThreadSave thread;

	// Token: 0x040012A0 RID: 4768
	private int msgIndex;

	// Token: 0x020002AE RID: 686
	public class VmailParsingData
	{
		// Token: 0x040012A1 RID: 4769
		public StateSaveData.MessageThreadSave thread;

		// Token: 0x040012A2 RID: 4770
		public int messageIndex;
	}
}
