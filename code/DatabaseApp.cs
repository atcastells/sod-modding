using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200029D RID: 669
public class DatabaseApp : CruncherAppContent
{
	// Token: 0x06000EF3 RID: 3827 RVA: 0x000D6014 File Offset: 0x000D4214
	public override void OnSetup()
	{
		this.searchString = string.Empty;
		this.list.OnNewSelection += this.UpdateSelected;
		this.UpdateSearch();
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x000D6040 File Offset: 0x000D4240
	private void Update()
	{
		if (!SessionData.Instance.play)
		{
			return;
		}
		if (Player.Instance.computerInteractable == this.controller.ic.interactable && !InterfaceController.Instance.playerTextInputActive && InputController.Instance.mouseInputMode && InteractionController.Instance.inputCooldown <= 0.01f)
		{
			if (Input.GetKeyDown(97))
			{
				this.KeyboardButton("A");
			}
			if (Input.GetKeyDown(98))
			{
				this.KeyboardButton("B");
			}
			if (Input.GetKeyDown(99))
			{
				this.KeyboardButton("C");
			}
			if (Input.GetKeyDown(100))
			{
				this.KeyboardButton("D");
			}
			if (Input.GetKeyDown(101))
			{
				this.KeyboardButton("E");
			}
			if (Input.GetKeyDown(102))
			{
				this.KeyboardButton("F");
			}
			if (Input.GetKeyDown(103))
			{
				this.KeyboardButton("G");
			}
			if (Input.GetKeyDown(104))
			{
				this.KeyboardButton("H");
			}
			if (Input.GetKeyDown(105))
			{
				this.KeyboardButton("I");
			}
			if (Input.GetKeyDown(106))
			{
				this.KeyboardButton("J");
			}
			if (Input.GetKeyDown(107))
			{
				this.KeyboardButton("K");
			}
			if (Input.GetKeyDown(108))
			{
				this.KeyboardButton("L");
			}
			if (Input.GetKeyDown(109))
			{
				this.KeyboardButton("M");
			}
			if (Input.GetKeyDown(110))
			{
				this.KeyboardButton("N");
			}
			if (Input.GetKeyDown(111))
			{
				this.KeyboardButton("O");
			}
			if (Input.GetKeyDown(112))
			{
				this.KeyboardButton("P");
			}
			if (Input.GetKeyDown(113))
			{
				this.KeyboardButton("Q");
			}
			if (Input.GetKeyDown(114))
			{
				this.KeyboardButton("R");
			}
			if (Input.GetKeyDown(115))
			{
				this.KeyboardButton("S");
			}
			if (Input.GetKeyDown(116))
			{
				this.KeyboardButton("T");
			}
			if (Input.GetKeyDown(117))
			{
				this.KeyboardButton("U");
			}
			if (Input.GetKeyDown(118))
			{
				this.KeyboardButton("V");
			}
			if (Input.GetKeyDown(119))
			{
				this.KeyboardButton("W");
			}
			if (Input.GetKeyDown(120))
			{
				this.KeyboardButton("X");
			}
			if (Input.GetKeyDown(121))
			{
				this.KeyboardButton("Y");
			}
			if (Input.GetKeyDown(122))
			{
				this.KeyboardButton("Z");
			}
			if (Input.GetKeyDown(32))
			{
				this.KeyboardButton(" ");
			}
			if (Input.GetKeyDown(8))
			{
				this.BackspaceButton();
			}
		}
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x000D62D5 File Offset: 0x000D44D5
	private void OnDestroy()
	{
		this.list.OnNewSelection -= this.UpdateSelected;
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x000D62F0 File Offset: 0x000D44F0
	public void UpdateSelected()
	{
		if (this.list.selected != null)
		{
			this.printButton.gameObject.SetActive(true);
			if (this.list.selected.option != null)
			{
				this.selectedHuman = this.list.selected.option.human;
				return;
			}
		}
		else
		{
			this.printButton.gameObject.SetActive(false);
			this.selectedHuman = null;
		}
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x000D6368 File Offset: 0x000D4568
	public void KeyboardButton(string charStr)
	{
		this.searchString += charStr;
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerKeyboardKey, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
		this.UpdateSearch();
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x000D63C7 File Offset: 0x000D45C7
	public void BackspaceButton()
	{
		if (this.searchString.Length > 0)
		{
			this.searchString = this.searchString.Substring(0, this.searchString.Length - 1);
		}
		this.UpdateSearch();
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x000D63FC File Offset: 0x000D45FC
	public void UpdateSearch()
	{
		List<ComputerOSMultiSelect.OSMultiOption> list = new List<ComputerOSMultiSelect.OSMultiOption>();
		string text = string.Empty;
		if (this.searchString.Length >= 2)
		{
			List<Citizen> list2 = new List<Citizen>();
			if (this.citizenPool == DatabaseApp.CitizenPool.allCitizens)
			{
				list2 = CityData.Instance.citizenDirectory.FindAll((Citizen item) => item.GetCitizenName().ToLower().Contains(this.searchString.ToLower()));
			}
			else if (this.citizenPool == DatabaseApp.CitizenPool.companyOnly)
			{
				list2 = CityData.Instance.citizenDirectory.FindAll((Citizen item) => item.job != null && item.job.employer != null && item.job.employer.address == this.controller.ic.interactable.node.gameLocation && item.GetCitizenName().ToLower().Contains(this.searchString.ToLower()));
			}
			else if (this.citizenPool == DatabaseApp.CitizenPool.buildingOnly)
			{
				list2 = CityData.Instance.citizenDirectory.FindAll((Citizen item) => item.home != null && item.home.building == this.controller.ic.interactable.node.gameLocation.building && item.GetCitizenName().ToLower().Contains(this.searchString.ToLower()));
			}
			using (List<Citizen>.Enumerator enumerator = list2.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Citizen citizen = enumerator.Current;
					list.Add(new ComputerOSMultiSelect.OSMultiOption
					{
						text = citizen.GetCitizenName(),
						human = citizen
					});
				}
				goto IL_EC;
			}
		}
		text = "<color=\"red\">";
		IL_EC:
		this.searchText.text = string.Concat(new string[]
		{
			Strings.Get("computer", "Database Search", Strings.Casing.asIs, false, false, false, null),
			": ",
			text,
			this.searchString,
			"_"
		});
		this.list.UpdateElements(list);
		this.UpdateSelected();
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x000D6564 File Offset: 0x000D4764
	public void ExitButton()
	{
		this.controller.OnAppExit();
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x000D6574 File Offset: 0x000D4774
	public void OnPrintEntry()
	{
		if (!(this.selectedHuman != null))
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerInvalidPasscode, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			return;
		}
		string text = "Print ";
		Human human = this.selectedHuman;
		Game.Log(text + ((human != null) ? human.ToString() : null), 2);
		this.controller.SetTimedLoading(Toolbox.Instance.Rand(0.5f, 1f, false), 0.33f);
		if (this.controller.printedDocument == null && this.controller.printTimer <= 0f)
		{
			this.controller.printTimer = 1f;
			this.controller.printerParent.localPosition = new Vector3(this.controller.printerParent.localPosition.x, this.controller.printerParent.localPosition.y, -0.05f);
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerPrint, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			this.controller.printedDocument = InteractableCreator.Instance.CreateWorldInteractable(this.ddsPrintout, Player.Instance, this.selectedHuman, this.selectedHuman, this.controller.printerParent.position, this.controller.ic.transform.eulerAngles, null, null, "");
			if (this.controller.printedDocument != null)
			{
				this.controller.printedDocument.MarkAsTrash(true, false, 0f);
			}
			this.controller.printedDocument.OnRemovedFromWorld += base.OnPlayerTakePrint;
			return;
		}
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerInvalidPasscode, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
	}

	// Token: 0x04001220 RID: 4640
	[Header("Components")]
	public TextMeshProUGUI titleText;

	// Token: 0x04001221 RID: 4641
	public TextMeshProUGUI searchText;

	// Token: 0x04001222 RID: 4642
	public ComputerOSMultiSelect list;

	// Token: 0x04001223 RID: 4643
	public RectTransform printButton;

	// Token: 0x04001224 RID: 4644
	private Human selectedHuman;

	// Token: 0x04001225 RID: 4645
	[Header("State")]
	public string searchString;

	// Token: 0x04001226 RID: 4646
	public InteractablePreset ddsPrintout;

	// Token: 0x04001227 RID: 4647
	public DatabaseApp.CitizenPool citizenPool;

	// Token: 0x0200029E RID: 670
	public enum CitizenPool
	{
		// Token: 0x04001229 RID: 4649
		allCitizens,
		// Token: 0x0400122A RID: 4650
		companyOnly,
		// Token: 0x0400122B RID: 4651
		buildingOnly
	}
}
