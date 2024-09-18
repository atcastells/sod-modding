using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005F1 RID: 1521
public class PasscodesController : MonoBehaviour
{
	// Token: 0x06002172 RID: 8562 RVA: 0x001C7A24 File Offset: 0x001C5C24
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.wcc = newContentController;
		if (this.isMini)
		{
			this.SetPageSize(new Vector2(338f, 738f));
		}
		else
		{
			this.SetPageSize(new Vector2(740f, 738f));
		}
		if (this.contentsText != null)
		{
			this.contentsText.text = Strings.Get("ui.handbook", "passcodesheader", Strings.Casing.asIs, false, false, false, null);
		}
		this.isSetup = true;
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x001C7AB4 File Offset: 0x001C5CB4
	public void SetPageSize(Vector2 newSize)
	{
		string text = "Set history size: ";
		Vector2 vector = newSize;
		Game.Log(text + vector.ToString(), 2);
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x001C7B02 File Offset: 0x001C5D02
	private void OnEnable()
	{
		if (this.isSetup)
		{
			GameplayController.Instance.OnNewEvidenceHistory += this.UpdateListDisplay;
			this.UpdateListDisplay();
		}
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x001C7B28 File Offset: 0x001C5D28
	private void OnDisable()
	{
		GameplayController.Instance.OnNewEvidenceHistory -= this.UpdateListDisplay;
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x001C7B28 File Offset: 0x001C5D28
	private void OnDestroy()
	{
		GameplayController.Instance.OnNewEvidenceHistory -= this.UpdateListDisplay;
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x001C7B40 File Offset: 0x001C5D40
	public void UpdateListDisplay()
	{
		Game.Log("Interface: Updating passcodes list...", 2);
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		List<GameplayController.Passcode> list = new List<GameplayController.Passcode>();
		foreach (GameplayController.Passcode passcode in GameplayController.Instance.acquiredPasscodes)
		{
			if (list.Count >= 40)
			{
				break;
			}
			if (!list.Contains(passcode) && passcode.digits.Count >= 4)
			{
				if (this.searchInputField.text.Length <= 0)
				{
					list.Add(passcode);
				}
				else
				{
					string text = string.Empty;
					if (passcode.type == GameplayController.PasscodeType.room)
					{
						NewRoom newRoom = null;
						if (CityData.Instance.roomDictionary.TryGetValue(passcode.id, ref newRoom))
						{
							text = text + newRoom.GetName() + ", " + newRoom.gameLocation.evidenceEntry.GetNameForDataKey(Evidence.DataKey.name);
						}
					}
					else
					{
						NewAddress newAddress = null;
						if (CityData.Instance.addressDictionary.TryGetValue(passcode.id, ref newAddress) && newAddress.evidenceEntry != null)
						{
							text += newAddress.evidenceEntry.GetNameForDataKey(Evidence.DataKey.name);
						}
					}
					text = string.Concat(new string[]
					{
						text,
						" ",
						passcode.GetDigit(0).ToString(),
						passcode.GetDigit(1).ToString(),
						passcode.GetDigit(2).ToString(),
						passcode.GetDigit(3).ToString()
					});
					if (text.ToLower().Contains(this.searchInputField.text.ToLower()))
					{
						list.Add(passcode);
					}
				}
			}
		}
		for (int i = 0; i < this.spawnedEntries.Count; i++)
		{
			PasscodesEntryController passcodesEntryController = this.spawnedEntries[i];
			if (passcodesEntryController == null)
			{
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
			else if (list.Contains(passcodesEntryController.passcode))
			{
				list.Remove(passcodesEntryController.passcode);
				passcodesEntryController.VisualUpdate();
			}
			else
			{
				Object.Destroy(passcodesEntryController.gameObject);
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
		}
		foreach (GameplayController.Passcode newPasscode in list)
		{
			GameObject gameObject;
			if (this.isMini)
			{
				gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.passcodesEntryMini, this.entryParent);
			}
			else
			{
				gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.passcodesEntry, this.entryParent);
			}
			PasscodesEntryController component = gameObject.GetComponent<PasscodesEntryController>();
			component.Setup(newPasscode);
			this.spawnedEntries.Add(component);
		}
		for (int j = 0; j < this.spawnedEntries.Count; j++)
		{
			this.spawnedEntries[j].transform.SetAsLastSibling();
		}
		this.entryParent.sizeDelta = new Vector2(this.entryParent.sizeDelta.x, Mathf.Max((float)this.spawnedEntries.Count * 130f + 24f, 466f));
		this.SetPageSize(new Vector2(this.rect.sizeDelta.x, this.entryParent.sizeDelta.y + 400f));
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x001C7F28 File Offset: 0x001C6128
	public void ClearSearchButton()
	{
		this.searchInputField.text = string.Empty;
	}

	// Token: 0x04002BC1 RID: 11201
	public RectTransform rect;

	// Token: 0x04002BC2 RID: 11202
	public WindowContentController wcc;

	// Token: 0x04002BC3 RID: 11203
	public bool isSetup;

	// Token: 0x04002BC4 RID: 11204
	public bool isMini;

	// Token: 0x04002BC5 RID: 11205
	public TextMeshProUGUI contentsText;

	// Token: 0x04002BC6 RID: 11206
	public RectTransform entryParent;

	// Token: 0x04002BC7 RID: 11207
	public TMP_InputField searchInputField;

	// Token: 0x04002BC8 RID: 11208
	public List<PasscodesEntryController> spawnedEntries = new List<PasscodesEntryController>();
}
