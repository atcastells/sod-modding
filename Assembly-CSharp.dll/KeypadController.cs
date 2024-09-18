using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200057D RID: 1405
public class KeypadController : MonoBehaviour
{
	// Token: 0x06001EBC RID: 7868 RVA: 0x001AB848 File Offset: 0x001A9A48
	private void OnEnable()
	{
		this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		this.windowContent = base.gameObject.GetComponentInParent<WindowContentController>();
		this.evidence = this.parentWindow.passedEvidence;
		this.defaultTextColour = this.inputText.color;
		if (this.evidence == CityData.Instance.telephone)
		{
			this.isTelephone = true;
			this.digits = 7;
		}
		else
		{
			this.digits = 4;
		}
		this.ClearCode(false);
		InterfaceController.Instance.OnInputCode += this.OnInputCode;
		if (!this.isTelephone)
		{
			InterfaceController.Instance.SetActiveCodeInput(this);
			GameplayController.Passcode passwordSource = InteractionController.Instance.currentLookingAtInteractable.interactable.GetPasswordSource();
			if (GameplayController.Instance.acquiredPasscodes.Contains(passwordSource))
			{
				this.OnInputCode(passwordSource.GetDigits());
			}
			SessionData.Instance.TutorialTrigger("passwords", false);
			return;
		}
		InterfaceController.Instance.SetActiveCodeInput(this);
		SessionData.Instance.TutorialTrigger("phones", false);
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x001AB951 File Offset: 0x001A9B51
	private void OnDisable()
	{
		InterfaceController.Instance.OnInputCode -= this.OnInputCode;
		this.inputCodeActive = false;
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x001AB970 File Offset: 0x001A9B70
	private void OnDestroy()
	{
		if (InterfaceController.Instance.activeCodeInput == this)
		{
			InterfaceController.Instance.SetActiveCodeInput(null);
		}
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x001AB990 File Offset: 0x001A9B90
	public void PressNumberButton(int newInt)
	{
		if (this.checking)
		{
			return;
		}
		if (this.isTelephone && this.parentWindow.passedInteractable.t != null && this.parentWindow.passedInteractable.t.dialTone != null)
		{
			AudioController.Instance.StopSound(this.parentWindow.passedInteractable.t.dialTone, AudioController.StopType.immediate, "phone hang up");
			this.parentWindow.passedInteractable.t.dialTone = null;
		}
		this.input.Add(newInt);
		string text = string.Empty;
		for (int i = 0; i < this.input.Count; i++)
		{
			text += this.input[i].ToString();
		}
		while (text.Length < this.digits)
		{
			text += "_";
		}
		this.inputText.text = text;
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.keypadButtons[newInt], Player.Instance, Player.Instance.currentNode, this.parentWindow.passedInteractable.controller.transform.position, null, null, 1f, null, false, null, false);
		if (this.input.Count >= this.digits)
		{
			this.SubmitCode();
		}
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x001ABAE8 File Offset: 0x001A9CE8
	public void OnKeypadButtonDown()
	{
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.keypadPress, Player.Instance, Player.Instance.currentNode, this.parentWindow.passedInteractable.controller.transform.position, null, null, 1f, null, false, null, false);
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x001ABB40 File Offset: 0x001A9D40
	public void ClearCode(bool press = true)
	{
		this.checkCounter = 0f;
		this.checking = false;
		this.correct = false;
		this.inputText.color = this.defaultTextColour;
		this.input.Clear();
		this.inputText.text = string.Empty;
		for (int i = 0; i < this.digits; i++)
		{
			TextMeshProUGUI textMeshProUGUI = this.inputText;
			textMeshProUGUI.text += "_";
		}
		if (press)
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.keypadClear, Player.Instance, Player.Instance.currentNode, InteractionController.Instance.currentLookingAtInteractable.interactable.spawnedObject.transform.position, null, null, 1f, null, false, null, false);
		}
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x001ABC10 File Offset: 0x001A9E10
	public void SubmitCode()
	{
		if (!this.isTelephone)
		{
			Interactable interactable = InteractionController.Instance.currentLookingAtInteractable.interactable;
			if (interactable != null)
			{
				List<string> list = null;
				List<int> passwordFromSource = interactable.GetPasswordFromSource(out list);
				Game.Log("Password contains " + passwordFromSource.Count.ToString() + " digits...", 2);
				for (int i = 0; i < this.input.Count; i++)
				{
					if (passwordFromSource.Count > i && this.input[i] != passwordFromSource[i])
					{
						if (passwordFromSource.Count >= 4)
						{
							Game.Log(string.Concat(new string[]
							{
								"Wrong code: Correct code is ",
								passwordFromSource[0].ToString(),
								passwordFromSource[1].ToString(),
								passwordFromSource[2].ToString(),
								passwordFromSource[3].ToString()
							}), 2);
						}
						else
						{
							Game.Log("Invalid passcode length: " + passwordFromSource.Count.ToString(), 2);
						}
						if (Game.Instance.devMode && Game.Instance.collectDebugData)
						{
							Game.Log("Displaying note placements...", 2);
							if (list != null)
							{
								foreach (string print in list)
								{
									Game.Log(print, 2);
								}
							}
						}
						AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.keypadDeny, Player.Instance, Player.Instance.currentNode, InteractionController.Instance.currentLookingAtInteractable.interactable.spawnedObject.transform.position, null, null, 1f, null, false, null, false);
						this.inputText.color = Color.red;
						this.correct = false;
						this.checking = true;
						return;
					}
				}
				if (interactable.thisDoor != null)
				{
					interactable.thisDoor.SetLockedState(false, Player.Instance, true, false);
				}
			}
		}
		else
		{
			if (this.parentWindow != null && this.parentWindow.passedInteractable != null && this.parentWindow.passedInteractable.preset.isPayphone)
			{
				int num = 1 + Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.payPhoneCostModifier));
				if (GameplayController.Instance.money < num)
				{
					InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_money", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.money, null, null, null);
					this.inputText.color = Color.red;
					this.correct = false;
					this.checking = true;
					return;
				}
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.payphoneMoneyIn, Player.Instance, Player.Instance.currentNode, this.parentWindow.passedInteractable.GetWorldPosition(true), null, null, 1f, null, false, null, false);
				GameplayController.Instance.AddMoney(-num, true, "payphone");
			}
			string text = string.Empty;
			for (int j = 0; j < this.input.Count; j++)
			{
				text += this.input[j].ToString();
			}
			int num2 = 0;
			int.TryParse(text, ref num2);
			if (num2 == 451)
			{
				NewAddress newAddress = CityData.Instance.addressDirectory.Find((NewAddress item) => item.currentOccupants.Count > 0 && item.telephones.Count > 0 && item.residence != null);
				if (newAddress != null)
				{
					this.input.Clear();
					string text2 = newAddress.telephones[0].number.ToString();
					for (int k = 0; k < text2.Length; k++)
					{
						int num3 = 0;
						int.TryParse(text2.get_Chars(k).ToString(), ref num3);
						this.input.Add(num3);
					}
				}
			}
			else if (!CityData.Instance.phoneDictionary.ContainsKey(num2) && !TelephoneController.Instance.fakeTelephoneDictionary.ContainsKey(num2))
			{
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.keypadDeny, Player.Instance, Player.Instance.currentNode, InteractionController.Instance.currentLookingAtInteractable.interactable.spawnedObject.transform.position, null, null, 1f, null, false, null, false);
				this.inputText.color = Color.red;
				this.correct = false;
				this.checking = true;
				return;
			}
		}
		this.inputText.color = Color.green;
		this.correct = true;
		this.checking = true;
		if (!this.isTelephone)
		{
			if (this.parentWindow != null)
			{
				Interactable passedInteractable = this.parentWindow.passedInteractable;
				if (passedInteractable != null)
				{
					passedInteractable.AddPasswordSourceToAcquired();
					return;
				}
			}
		}
		else
		{
			InteractionController.Instance.SetDialog(true, this.parentWindow.passedInteractable, false, null, InteractionController.ConversationType.normal);
		}
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x001AC134 File Offset: 0x001AA334
	private void Update()
	{
		if (!InterfaceController.Instance.playerTextInputActive)
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
					if (this.isTelephone)
					{
						string text = string.Empty;
						for (int i = 0; i < this.input.Count; i++)
						{
							text += this.input[i].ToString();
						}
						int num = 0;
						int.TryParse(text, ref num);
						if (CityData.Instance.phoneDictionary.ContainsKey(num))
						{
							TelephoneController.Instance.CreateNewCall(this.parentWindow.passedInteractable.t, CityData.Instance.phoneDictionary[num], Player.Instance, null, new TelephoneController.CallSource(TelephoneController.CallType.player, CitizenControls.Instance.telephoneGreeting, InteractionController.ConversationType.normal), 99f, false);
						}
						else if (TelephoneController.Instance.fakeTelephoneDictionary.ContainsKey(num))
						{
							TelephoneController.Instance.CreateNewCall(this.parentWindow.passedInteractable.t, null, Player.Instance, null, TelephoneController.Instance.fakeTelephoneDictionary[num], 99f, false);
						}
						InteractionController.Instance.RefreshDialogOptions();
					}
					else
					{
						InteractablePreset.InteractionAction action = InteractionController.Instance.currentLookingAtInteractable.interactable.thisDoor.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.effectSwitchStates.Exists((InteractablePreset.SwitchState item2) => item2.switchState == InteractablePreset.Switch.switchState && item2.boolIs));
						InteractionController.Instance.currentLookingAtInteractable.interactable.thisDoor.OnInteraction(action, Player.Instance, true, 0f);
					}
					this.parentWindow.CloseWindow(true);
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

	// Token: 0x06001EC4 RID: 7876 RVA: 0x001AC464 File Offset: 0x001AA664
	public void OnInputCode(List<int> code)
	{
		if (!this.inputCodeActive)
		{
			Game.Log("Interface: Inputting code with " + code.Count.ToString() + " digits...", 2);
			base.StartCoroutine(this.InputCode(code));
		}
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x001AC4AA File Offset: 0x001AA6AA
	private IEnumerator InputCode(List<int> code)
	{
		this.inputCodeActive = true;
		int codeCursor = 0;
		while (code != null && codeCursor < this.digits && codeCursor < code.Count)
		{
			if (!this.checking)
			{
				int num = code[codeCursor];
				Game.Log(num, 2);
				try
				{
					this.OnKeypadButtonDown();
					this.PressNumberButton(num);
				}
				catch
				{
					Game.LogError("Error inputting code!", 2);
				}
				int num2 = codeCursor;
				codeCursor = num2 + 1;
			}
			yield return new WaitForSeconds(0.15f);
		}
		this.inputCodeActive = false;
		yield break;
	}

	// Token: 0x04002892 RID: 10386
	public InfoWindow parentWindow;

	// Token: 0x04002893 RID: 10387
	public Evidence evidence;

	// Token: 0x04002894 RID: 10388
	public WindowContentController windowContent;

	// Token: 0x04002895 RID: 10389
	public TextMeshProUGUI inputText;

	// Token: 0x04002896 RID: 10390
	public List<int> input = new List<int>();

	// Token: 0x04002897 RID: 10391
	public Color defaultTextColour;

	// Token: 0x04002898 RID: 10392
	public bool checking;

	// Token: 0x04002899 RID: 10393
	public bool correct;

	// Token: 0x0400289A RID: 10394
	public float checkCounter;

	// Token: 0x0400289B RID: 10395
	public bool inputCodeActive;

	// Token: 0x0400289C RID: 10396
	public bool isTelephone;

	// Token: 0x0400289D RID: 10397
	public int digits = 4;
}
