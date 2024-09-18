using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005F3 RID: 1523
public class PhoneNumberEntryController : MonoBehaviour
{
	// Token: 0x06002183 RID: 8579 RVA: 0x001C8508 File Offset: 0x001C6708
	public void Setup(GameplayController.PhoneNumber newNumber)
	{
		this.number = newNumber;
		this.openLocationButton.SetupReferences();
		this.openEvidenceButton.SetupReferences();
		this.enterCodeButton.SetupReferences();
		this.openLocationButton.VisualUpdate();
		this.openEvidenceButton.VisualUpdate();
		this.enterCodeButton.VisualUpdate();
		CityData.Instance.phoneDictionary.TryGetValue(this.number.number, ref this.telephone);
		GameplayController.Instance.OnNewPhoneData += this.VisualUpdate;
		this.VisualUpdate();
		InterfaceController.Instance.OnNewActiveCodeInput += this.ActiveCodeInputCheck;
		this.ActiveCodeInputCheck(InterfaceController.Instance.activeCodeInput);
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x001C85C4 File Offset: 0x001C67C4
	public void VisualUpdate()
	{
		this.text.text = Toolbox.Instance.GetTelephoneNumberString(this.number.number) + "\n";
		string text = string.Empty;
		if (this.number.textOverride != null && this.number.textOverride.Length > 0)
		{
			TextMeshProUGUI textMeshProUGUI = this.text;
			textMeshProUGUI.text += Strings.Get("evidence.generic", this.number.textOverride, Strings.Casing.asIs, false, false, false, null);
		}
		if (this.number.loc && this.telephone != null)
		{
			TextMeshProUGUI textMeshProUGUI2 = this.text;
			textMeshProUGUI2.text += this.telephone.location.name;
			if (this.telephone.interactable != null && this.telephone.interactable.preset.isPayphone)
			{
				TextMeshProUGUI textMeshProUGUI3 = this.text;
				textMeshProUGUI3.text = textMeshProUGUI3.text + " " + Strings.Get("evidence.generic", "payphone", Strings.Casing.asIs, false, false, false, null);
			}
			this.openLocationButton.SetInteractable(true);
			text = ": ";
		}
		else
		{
			this.openLocationButton.SetInteractable(false);
		}
		if (this.telephone != null && this.telephone.telephoneEntry != null)
		{
			this.openLocationButton.SetInteractable(true);
		}
		else
		{
			this.openLocationButton.SetInteractable(false);
		}
		foreach (int id in this.number.p)
		{
			Human human = null;
			if (CityData.Instance.GetHuman(id, out human, true))
			{
				if (!this.citizenSubscriptions.Contains(human))
				{
					this.citizenSubscriptions.Add(human);
					human.evidenceEntry.OnNewName += this.VisualUpdate;
					human.evidenceEntry.OnDataKeyChange += this.VisualUpdate;
				}
				TextMeshProUGUI textMeshProUGUI4 = this.text;
				textMeshProUGUI4.text = textMeshProUGUI4.text + text + human.GetCitizenName();
			}
		}
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x001C87F4 File Offset: 0x001C69F4
	public void ActiveCodeInputCheck(KeypadController keypad)
	{
		if (InterfaceController.Instance.activeCodeInput != null && InterfaceController.Instance.activeCodeInput.isTelephone)
		{
			this.enterCodeButton.SetInteractable(true);
			return;
		}
		this.enterCodeButton.SetInteractable(false);
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x001C8834 File Offset: 0x001C6A34
	private void OnDestroy()
	{
		foreach (Human human in this.citizenSubscriptions)
		{
			human.evidenceEntry.OnNewName -= this.VisualUpdate;
			human.evidenceEntry.OnDataKeyChange -= this.VisualUpdate;
		}
		GameplayController.Instance.OnNewPhoneData -= this.VisualUpdate;
		InterfaceController.Instance.OnNewActiveCodeInput -= this.ActiveCodeInputCheck;
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x001C88D8 File Offset: 0x001C6AD8
	public void OpenLocation()
	{
		if (this.telephone != null && this.telephone.locationEntry != null)
		{
			InterfaceController.Instance.SpawnWindow(this.telephone.locationEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
		}
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x001C8928 File Offset: 0x001C6B28
	public void OpenEvidence()
	{
		if (this.telephone != null && this.telephone.telephoneEntry != null)
		{
			InterfaceController.Instance.SpawnWindow(this.telephone.telephoneEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
		}
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x001C8978 File Offset: 0x001C6B78
	public void EnterCode()
	{
		if (InterfaceController.Instance.activeCodeInput != null && InterfaceController.Instance.activeCodeInput.isTelephone)
		{
			InterfaceController.Instance.activeCodeInput.parentWindow.SetActiveContent(InterfaceController.Instance.activeCodeInput.windowContent);
			List<int> list = new List<int>();
			string text = this.number.number.ToString();
			for (int i = 0; i < text.Length; i++)
			{
				int num = 0;
				if (int.TryParse(text.get_Chars(i).ToString(), ref num))
				{
					list.Add(num);
				}
			}
			InterfaceController.Instance.activeCodeInput.OnInputCode(list);
			this.enterCodeButton.OnDeselect();
		}
	}

	// Token: 0x04002BD6 RID: 11222
	public RectTransform rect;

	// Token: 0x04002BD7 RID: 11223
	public Telephone telephone;

	// Token: 0x04002BD8 RID: 11224
	public GameplayController.PhoneNumber number;

	// Token: 0x04002BD9 RID: 11225
	public TextMeshProUGUI text;

	// Token: 0x04002BDA RID: 11226
	public ButtonController openLocationButton;

	// Token: 0x04002BDB RID: 11227
	public ButtonController openEvidenceButton;

	// Token: 0x04002BDC RID: 11228
	public ButtonController enterCodeButton;

	// Token: 0x04002BDD RID: 11229
	public Image icon;

	// Token: 0x04002BDE RID: 11230
	public string nameString;

	// Token: 0x04002BDF RID: 11231
	public string passcodeString;

	// Token: 0x04002BE0 RID: 11232
	private List<Human> citizenSubscriptions = new List<Human>();
}
