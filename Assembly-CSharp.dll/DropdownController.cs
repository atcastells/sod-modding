using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x02000533 RID: 1331
public class DropdownController : MonoBehaviour
{
	// Token: 0x06001D13 RID: 7443 RVA: 0x0019DC50 File Offset: 0x0019BE50
	private void Start()
	{
		this.normalWidth = this.dropdownRect.sizeDelta.x;
		this.OnControlModeChange();
		InputController.Instance.OnInputModeChange += this.OnControlModeChange;
		this.nextButton.button.onClick.AddListener(delegate()
		{
			this.OnNextButton();
		});
		this.prevButton.button.onClick.AddListener(delegate()
		{
			this.OnPreviousButton();
		});
	}

	// Token: 0x06001D14 RID: 7444 RVA: 0x0019DCD4 File Offset: 0x0019BED4
	public void AddOptions(List<string> newOptions, bool useDictionary, List<string> newListedOptions = null)
	{
		this.dropdown.ClearOptions();
		if (useDictionary)
		{
			List<string> list = new List<string>();
			foreach (string key in newOptions)
			{
				list.Add(Strings.Get("ui.interface", key, Strings.Casing.asIs, false, false, false, null));
			}
			this.dropdown.AddOptions(list);
		}
		else if (newListedOptions != null)
		{
			this.dropdown.AddOptions(newListedOptions);
		}
		else
		{
			this.dropdown.AddOptions(newOptions);
		}
		this.staticOptionReference.Clear();
		this.staticOptionReference.AddRange(newOptions);
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x0019DD88 File Offset: 0x0019BF88
	public void SelectFromStaticOption(string staticOption)
	{
		int num = this.staticOptionReference.FindIndex((string item) => item.ToLower() == staticOption.ToLower());
		if (num > -1)
		{
			Game.Log("Menu: Found static option reference for " + staticOption + ": " + this.staticOptionReference[this.dropdown.value], 2);
			this.dropdown.SetValueWithoutNotify(num);
			return;
		}
		Game.Log("Menu: Unable to find static option reference for " + staticOption, 2);
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x0019DE14 File Offset: 0x0019C014
	public string GetCurrentSelectedStaticOption()
	{
		if (this.staticOptionReference.Count > this.dropdown.value)
		{
			Game.Log("Menu: Found static option reference: " + this.staticOptionReference[this.dropdown.value], 2);
			return this.staticOptionReference[this.dropdown.value];
		}
		return string.Empty;
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x0019DE7C File Offset: 0x0019C07C
	public void OnControlModeChange()
	{
		if (InputController.Instance.mouseInputMode)
		{
			if (this.buttonsRect != null)
			{
				this.buttonsRect.gameObject.SetActive(false);
			}
			if (this.dropdownRect != null)
			{
				this.dropdownRect.sizeDelta = new Vector2(this.normalWidth, this.dropdownRect.sizeDelta.y);
			}
			if (this.prevButton != null)
			{
				this.prevButton.SetInteractable(false);
			}
			if (this.nextButton != null)
			{
				this.nextButton.SetInteractable(false);
			}
			if (this.dropdownArrow != null)
			{
				this.dropdownArrow.gameObject.SetActive(this.isInteractable);
			}
			if (this.dropdown != null)
			{
				this.dropdown.interactable = this.isInteractable;
				return;
			}
		}
		else
		{
			if (this.dropdown != null && this.dropdown.IsExpanded)
			{
				this.dropdown.Hide();
			}
			if (this.buttonsRect != null)
			{
				this.buttonsRect.gameObject.SetActive(true);
			}
			if (this.dropdownRect != null)
			{
				this.dropdownRect.sizeDelta = new Vector2(this.normalWidth - 140f, this.dropdownRect.sizeDelta.y);
			}
			if (this.prevButton != null)
			{
				this.prevButton.SetInteractable(this.isInteractable);
			}
			if (this.nextButton != null)
			{
				this.nextButton.SetInteractable(this.isInteractable);
			}
			if (this.dropdownArrow != null)
			{
				this.dropdownArrow.gameObject.SetActive(false);
			}
			if (this.dropdown != null)
			{
				this.dropdown.interactable = false;
			}
		}
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x0019E05C File Offset: 0x0019C25C
	public void OnNextButton()
	{
		TMP_Dropdown tmp_Dropdown = this.dropdown;
		int value = tmp_Dropdown.value;
		tmp_Dropdown.value = value + 1;
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x0019E080 File Offset: 0x0019C280
	public void OnPreviousButton()
	{
		TMP_Dropdown tmp_Dropdown = this.dropdown;
		int value = tmp_Dropdown.value;
		tmp_Dropdown.value = value - 1;
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x0019E0A2 File Offset: 0x0019C2A2
	public void OnValueChange()
	{
		if (this.playerPrefsID != null && this.playerPrefsID.Length > 0)
		{
			PlayerPrefsController.Instance.OnToggleChanged(this.playerPrefsID, true, this);
		}
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x0019E0CC File Offset: 0x0019C2CC
	public void SetInteractalbe(bool val)
	{
		if (val != this.isInteractable)
		{
			this.isInteractable = val;
			this.OnControlModeChange();
		}
	}

	// Token: 0x040026D9 RID: 9945
	[Header("Components")]
	public RectTransform dropdownRect;

	// Token: 0x040026DA RID: 9946
	public RectTransform dropdownArrow;

	// Token: 0x040026DB RID: 9947
	public TMP_Dropdown dropdown;

	// Token: 0x040026DC RID: 9948
	public RectTransform buttonsRect;

	// Token: 0x040026DD RID: 9949
	public ButtonController prevButton;

	// Token: 0x040026DE RID: 9950
	public ButtonController nextButton;

	// Token: 0x040026DF RID: 9951
	[Header("Configuration")]
	public string playerPrefsID;

	// Token: 0x040026E0 RID: 9952
	public List<string> staticOptionReference = new List<string>();

	// Token: 0x040026E1 RID: 9953
	[Header("State")]
	public bool isInteractable = true;

	// Token: 0x040026E2 RID: 9954
	[ReadOnly]
	public float normalWidth = 550f;
}
