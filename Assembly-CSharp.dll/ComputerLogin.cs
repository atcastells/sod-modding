using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000291 RID: 657
public class ComputerLogin : CruncherAppContent
{
	// Token: 0x06000EB5 RID: 3765 RVA: 0x000D489C File Offset: 0x000D2A9C
	public override void OnSetup()
	{
		base.OnSetup();
		this.loginSelection.Setup(this.controller);
		List<ComputerOSMultiSelect.OSMultiOption> list = new List<ComputerOSMultiSelect.OSMultiOption>();
		if (this.controller.ic.interactable.node.gameLocation.thisAsAddress != null)
		{
			if (this.controller.ic.interactable.node.gameLocation.thisAsAddress.residence != null)
			{
				using (List<Human>.Enumerator enumerator = this.controller.ic.interactable.node.gameLocation.thisAsAddress.inhabitants.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Human human = enumerator.Current;
						ComputerOSMultiSelect.OSMultiOption osmultiOption = new ComputerOSMultiSelect.OSMultiOption
						{
							text = human.GetInitialledName(),
							human = human
						};
						list.Add(osmultiOption);
					}
					goto IL_289;
				}
			}
			foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in this.controller.ic.interactable.furnitureParent.ownerMap)
			{
				if (keyValuePair.Key.human != null)
				{
					ComputerOSMultiSelect.OSMultiOption osmultiOption2 = new ComputerOSMultiSelect.OSMultiOption
					{
						text = keyValuePair.Key.human.GetInitialledName(),
						human = keyValuePair.Key.human
					};
					list.Add(osmultiOption2);
				}
			}
			if (this.controller.ic.interactable.node != null && this.controller.ic.interactable.node.gameLocation.thisAsAddress != null && this.controller.ic.interactable.node.gameLocation.thisAsAddress.company != null && this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.director != null)
			{
				ComputerOSMultiSelect.OSMultiOption osmultiOption3 = new ComputerOSMultiSelect.OSMultiOption
				{
					text = this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.shortName + "_Admin",
					human = this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.director
				};
				list.Add(osmultiOption3);
			}
		}
		IL_289:
		this.loginSelection.UpdateElements(list);
		this.loginSelection.OnNewSelection += this.OnNewUserSelected;
		this.OnNewUserSelected();
		this.ClearCode(false);
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x000D4B80 File Offset: 0x000D2D80
	private void OnDestroy()
	{
		this.loginSelection.OnNewSelection -= this.OnNewUserSelected;
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x000D4B9C File Offset: 0x000D2D9C
	public void OnNewUserSelected()
	{
		if (this.loginSelection.selected != null)
		{
			this.numPadParent.SetActive(true);
			this.instructionText.text = Strings.Get("computer", "Enter Passcode", Strings.Casing.asIs, false, false, false, null);
			List<int> digits = this.loginSelection.selected.option.human.passcode.GetDigits();
			Game.Log(string.Concat(new string[]
			{
				"Player: Correct code is ",
				digits[0].ToString(),
				digits[1].ToString(),
				digits[2].ToString(),
				digits[3].ToString()
			}), 2);
			if (GameplayController.Instance.acquiredPasscodes.Contains(this.loginSelection.selected.option.human.passcode))
			{
				this.OnInputCode(this.loginSelection.selected.option.human.passcode.GetDigits(), 0.15f);
				return;
			}
		}
		else
		{
			this.numPadParent.SetActive(false);
			this.instructionText.text = Strings.Get("computer", "Select User", Strings.Casing.asIs, false, false, false, null);
		}
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x000D4CEC File Offset: 0x000D2EEC
	public void PressNumberButton(int newInt)
	{
		if (this.checking)
		{
			return;
		}
		this.input.Add(newInt);
		string text = string.Empty;
		for (int i = 0; i < this.input.Count; i++)
		{
			text += this.input[i].ToString();
		}
		while (text.Length < 4)
		{
			text += "_";
		}
		this.inputText.text = text;
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerKeyboardKey, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
		if (this.input.Count >= 4)
		{
			this.SubmitCode();
		}
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x000D4DB8 File Offset: 0x000D2FB8
	public void ClearCode(bool press = true)
	{
		this.checkCounter = 0f;
		this.checking = false;
		this.correct = false;
		this.inputText.color = this.defaultTextColour;
		this.input.Clear();
		this.inputText.text = "____";
		if (press)
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerKeyboardKey, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
		}
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x000D4E48 File Offset: 0x000D3048
	public void SubmitCode()
	{
		if (this.loginSelection.selected == null)
		{
			this.ClearCode(true);
			return;
		}
		List<int> digits = this.loginSelection.selected.option.human.passcode.GetDigits();
		if (digits == null || digits.Count <= 0)
		{
			Game.Log("Wrong code: Unable to get password from human.", 2);
			this.inputText.color = Color.red;
			this.correct = false;
			this.checking = true;
			return;
		}
		for (int i = 0; i < this.input.Count; i++)
		{
			if (this.input[i] != digits[i])
			{
				Game.Log(string.Concat(new string[]
				{
					"Player: Correct code is ",
					digits[0].ToString(),
					digits[1].ToString(),
					digits[2].ToString(),
					digits[3].ToString()
				}), 2);
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerInvalidPasscode, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
				this.inputText.color = Color.red;
				this.correct = false;
				this.checking = true;
				return;
			}
		}
		this.inputText.color = Color.green;
		this.correct = true;
		this.checking = true;
		GameplayController.Instance.AddPasscode(this.loginSelection.selected.option.human.passcode, true);
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerValidPasscode, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x000D5034 File Offset: 0x000D3234
	private void Update()
	{
		if (Player.Instance.computerInteractable == this.controller.ic.interactable && !InterfaceController.Instance.playerTextInputActive)
		{
			if (InputController.Instance.player.GetButtonDown("0"))
			{
				this.PressNumberButton(0);
			}
			else if (InputController.Instance.player.GetButtonDown("1"))
			{
				this.PressNumberButton(1);
			}
			else if (InputController.Instance.player.GetButtonDown("2"))
			{
				this.PressNumberButton(2);
			}
			else if (InputController.Instance.player.GetButtonDown("3"))
			{
				this.PressNumberButton(3);
			}
			else if (InputController.Instance.player.GetButtonDown("4"))
			{
				this.PressNumberButton(4);
			}
			else if (InputController.Instance.player.GetButtonDown("5"))
			{
				this.PressNumberButton(5);
			}
			else if (InputController.Instance.player.GetButtonDown("6"))
			{
				this.PressNumberButton(6);
			}
			else if (InputController.Instance.player.GetButtonDown("7"))
			{
				this.PressNumberButton(7);
			}
			else if (InputController.Instance.player.GetButtonDown("8"))
			{
				this.PressNumberButton(8);
			}
			else if (InputController.Instance.player.GetButtonDown("9"))
			{
				this.PressNumberButton(9);
			}
		}
		if (this.checking)
		{
			this.checkCounter += Time.deltaTime;
			if (this.checkCounter >= 0.3f)
			{
				if (this.correct)
				{
					this.controller.SetLoggedIn(this.loginSelection.selected.option.human);
					this.controller.OnAppExit();
				}
				else
				{
					this.ClearCode(false);
				}
				this.checkCounter = 0f;
				this.checking = false;
				this.correct = false;
			}
		}
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x000D522F File Offset: 0x000D342F
	public void OnInputCode(List<int> code, float keyDelay = 0.15f)
	{
		if (!this.inputCodeActive)
		{
			this.ClearCode(false);
			base.StartCoroutine(this.InputCode(code, keyDelay));
		}
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x000D524F File Offset: 0x000D344F
	private IEnumerator InputCode(List<int> code, float keyDelay = 0.15f)
	{
		this.inputCodeActive = true;
		int codeCursor = 0;
		while (codeCursor < 4)
		{
			if (!this.checking)
			{
				this.PressNumberButton(code[codeCursor]);
				int num = codeCursor;
				codeCursor = num + 1;
			}
			yield return new WaitForSeconds(keyDelay);
		}
		this.inputCodeActive = false;
		yield break;
	}

	// Token: 0x040011DC RID: 4572
	public ComputerOSMultiSelect loginSelection;

	// Token: 0x040011DD RID: 4573
	public TextMeshProUGUI inputText;

	// Token: 0x040011DE RID: 4574
	public TextMeshProUGUI instructionText;

	// Token: 0x040011DF RID: 4575
	public List<int> input = new List<int>();

	// Token: 0x040011E0 RID: 4576
	public Color defaultTextColour;

	// Token: 0x040011E1 RID: 4577
	public GameObject numPadParent;

	// Token: 0x040011E2 RID: 4578
	public bool checking;

	// Token: 0x040011E3 RID: 4579
	public bool correct;

	// Token: 0x040011E4 RID: 4580
	public float checkCounter;

	// Token: 0x040011E5 RID: 4581
	public bool inputCodeActive;
}
