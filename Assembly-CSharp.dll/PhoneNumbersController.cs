using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005F4 RID: 1524
public class PhoneNumbersController : MonoBehaviour
{
	// Token: 0x0600218B RID: 8587 RVA: 0x001C8A4C File Offset: 0x001C6C4C
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
			this.contentsText.text = Strings.Get("ui.handbook", "phonenumbersheader", Strings.Casing.asIs, false, false, false, null);
		}
		this.isSetup = true;
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x001C8AD9 File Offset: 0x001C6CD9
	public void SetPageSize(Vector2 newSize)
	{
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x001C8AFD File Offset: 0x001C6CFD
	private void OnEnable()
	{
		if (this.isSetup)
		{
			GameplayController.Instance.OnNewEvidenceHistory += this.UpdateListDisplay;
			this.UpdateListDisplay();
		}
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x001C8B23 File Offset: 0x001C6D23
	private void OnDisable()
	{
		GameplayController.Instance.OnNewEvidenceHistory -= this.UpdateListDisplay;
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x001C8B23 File Offset: 0x001C6D23
	private void OnDestroy()
	{
		GameplayController.Instance.OnNewEvidenceHistory -= this.UpdateListDisplay;
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x001C8B3C File Offset: 0x001C6D3C
	public void UpdateListDisplay()
	{
		Game.Log("Interface: Updating phone numbers list...", 2);
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		List<GameplayController.PhoneNumber> list = new List<GameplayController.PhoneNumber>();
		foreach (GameplayController.PhoneNumber phoneNumber in GameplayController.Instance.acquiredNumbers)
		{
			if (list.Count >= 40)
			{
				break;
			}
			if (!list.Contains(phoneNumber))
			{
				if (this.searchInputField.text.Length <= 0)
				{
					list.Add(phoneNumber);
				}
				else if (Toolbox.Instance.GetTelephoneNumberString(phoneNumber.number).ToLower().Contains(this.searchInputField.text.ToLower()))
				{
					list.Add(phoneNumber);
				}
			}
		}
		for (int i = 0; i < this.spawnedEntries.Count; i++)
		{
			PhoneNumberEntryController phoneNumberEntryController = this.spawnedEntries[i];
			if (phoneNumberEntryController == null)
			{
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
			else if (list.Contains(phoneNumberEntryController.number))
			{
				list.Remove(phoneNumberEntryController.number);
				phoneNumberEntryController.VisualUpdate();
			}
			else
			{
				Object.Destroy(phoneNumberEntryController.gameObject);
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
		}
		foreach (GameplayController.PhoneNumber newNumber in list)
		{
			PhoneNumberEntryController component = Object.Instantiate<GameObject>(PrefabControls.Instance.phoneNumberEntry, this.entryParent).GetComponent<PhoneNumberEntryController>();
			if (this.isMini)
			{
				component.rect.sizeDelta = new Vector2(338f, component.rect.sizeDelta.y);
			}
			component.Setup(newNumber);
			this.spawnedEntries.Add(component);
		}
		for (int j = 0; j < this.spawnedEntries.Count; j++)
		{
			this.spawnedEntries[j].transform.SetAsLastSibling();
		}
		this.entryParent.sizeDelta = new Vector2(this.entryParent.sizeDelta.x, Mathf.Max((float)this.spawnedEntries.Count * 130f + 24f, 466f));
		this.SetPageSize(new Vector2(this.rect.sizeDelta.x, this.entryParent.sizeDelta.y + 500f));
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x001C8E08 File Offset: 0x001C7008
	public void ClearSearchButton()
	{
		this.searchInputField.text = string.Empty;
	}

	// Token: 0x04002BE1 RID: 11233
	public RectTransform rect;

	// Token: 0x04002BE2 RID: 11234
	public WindowContentController wcc;

	// Token: 0x04002BE3 RID: 11235
	public bool isSetup;

	// Token: 0x04002BE4 RID: 11236
	public bool isMini;

	// Token: 0x04002BE5 RID: 11237
	public TextMeshProUGUI contentsText;

	// Token: 0x04002BE6 RID: 11238
	public RectTransform entryParent;

	// Token: 0x04002BE7 RID: 11239
	public TMP_InputField searchInputField;

	// Token: 0x04002BE8 RID: 11240
	public List<PhoneNumberEntryController> spawnedEntries = new List<PhoneNumberEntryController>();
}
