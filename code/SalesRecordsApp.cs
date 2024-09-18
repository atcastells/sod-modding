using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020002A9 RID: 681
public class SalesRecordsApp : CruncherAppContent
{
	// Token: 0x06000F1A RID: 3866 RVA: 0x000D76E8 File Offset: 0x000D58E8
	public override void OnSetup()
	{
		this.list.OnNewSelection += this.UpdateSelected;
		this.list.OnChangePage += this.OnChangePage;
		this.UpdateEntries();
		this.OnChangePage();
		this.UpdateSelected();
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x000D7735 File Offset: 0x000D5935
	private void OnDestroy()
	{
		this.list.OnNewSelection -= this.UpdateSelected;
		this.list.OnChangePage -= this.OnChangePage;
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x000D7768 File Offset: 0x000D5968
	public void UpdateEntries()
	{
		List<ComputerOSMultiSelect.OSMultiOption> list = new List<ComputerOSMultiSelect.OSMultiOption>();
		try
		{
			foreach (Company.SalesRecord salesRecord in this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.sales)
			{
				list.Add(new ComputerOSMultiSelect.OSMultiOption
				{
					salesRecord = salesRecord
				});
			}
		}
		catch
		{
			Game.LogError("Unable to update sales record entries!", 2);
		}
		this.list.UpdateElements(list);
		this.UpdateSelected();
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x000D7820 File Offset: 0x000D5A20
	public void OnChangePage()
	{
		this.displayText.text = string.Concat(new string[]
		{
			Strings.Get("computer", "Displaying page", Strings.Casing.asIs, false, false, false, null),
			" ",
			(this.list.page + 1).ToString(),
			"/",
			(Mathf.FloorToInt((float)(this.list.allOptions.Count / this.list.maxPerPage)) + 1).ToString()
		});
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x000D78B1 File Offset: 0x000D5AB1
	public void UpdateSelected()
	{
		if (this.list.selected != null)
		{
			this.printButton.gameObject.SetActive(true);
			return;
		}
		this.printButton.gameObject.SetActive(false);
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x000D6564 File Offset: 0x000D4764
	public void ExitButton()
	{
		this.controller.OnAppExit();
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x000D78EC File Offset: 0x000D5AEC
	public void OnPrintEntry()
	{
		if (!(this.list.selected != null))
		{
			Game.Log("No selected entry!", 2);
			return;
		}
		string text = "Print ";
		ComputerOSMultiSelectElement selected = this.list.selected;
		Game.Log(text + ((selected != null) ? selected.ToString() : null), 2);
		this.controller.SetTimedLoading(Toolbox.Instance.Rand(0.5f, 1f, false), 0.33f);
		if (this.controller.printedDocument == null && this.controller.printTimer <= 0f && this.list.selected != null && this.list.selected.option != null && this.list.selected.option.salesRecord != null)
		{
			this.controller.printTimer = 1f;
			this.controller.printerParent.localPosition = new Vector3(this.controller.printerParent.localPosition.x, this.controller.printerParent.localPosition.y, -0.05f);
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerPrint, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			List<Interactable.Passed> list = new List<Interactable.Passed>();
			list.Add(new Interactable.Passed(Interactable.PassedVarType.companyID, (float)this.list.selected.option.salesRecord.companyID, null));
			list.Add(new Interactable.Passed(Interactable.PassedVarType.time, this.list.selected.option.salesRecord.time, null));
			for (int i = 0; i < this.list.selected.option.salesRecord.items.Count; i++)
			{
				list.Add(new Interactable.Passed(Interactable.PassedVarType.stringInteractablePreset, -1f, this.list.selected.option.salesRecord.items[i]));
			}
			this.controller.printedDocument = InteractableCreator.Instance.CreateWorldInteractable(this.ddsPrintout, Player.Instance, this.list.selected.option.salesRecord.GetPunter(), this.list.selected.option.salesRecord.GetPunter(), this.controller.printerParent.position, this.controller.ic.transform.eulerAngles, list, null, "");
			if (this.controller.printedDocument != null)
			{
				this.controller.printedDocument.MarkAsTrash(true, false, 0f);
			}
			this.controller.printedDocument.OnRemovedFromWorld += base.OnPlayerTakePrint;
			return;
		}
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerInvalidPasscode, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
	}

	// Token: 0x04001241 RID: 4673
	[Header("Components")]
	public TextMeshProUGUI titleText;

	// Token: 0x04001242 RID: 4674
	public TextMeshProUGUI displayText;

	// Token: 0x04001243 RID: 4675
	public ComputerOSMultiSelect list;

	// Token: 0x04001244 RID: 4676
	public RectTransform printButton;

	// Token: 0x04001245 RID: 4677
	[Header("State")]
	public InteractablePreset ddsPrintout;

	// Token: 0x020002AA RID: 682
	public enum CitizenPool
	{
		// Token: 0x04001247 RID: 4679
		allCitizens,
		// Token: 0x04001248 RID: 4680
		companyOnly,
		// Token: 0x04001249 RID: 4681
		buildingOnly
	}
}
