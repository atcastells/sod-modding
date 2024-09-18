using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DE RID: 478
public class CityEditorStreetsEditListElement : MonoBehaviour
{
	// Token: 0x06000B6D RID: 2925 RVA: 0x000A943B File Offset: 0x000A763B
	public void Setup(StreetController newStreet, CityEditorStreetEdit controller)
	{
		this.streetEdit = controller;
		this.street = newStreet;
		this.selectButton.text.text = this.street.name;
		this.UpdateSelection();
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x000A946C File Offset: 0x000A766C
	public void UpdateSelection()
	{
		if (this.streetEdit.currentlySelectedStreet == this.street)
		{
			this.selectionImg.enabled = true;
			this.selectButton.AutoScroll();
			return;
		}
		this.selectionImg.enabled = false;
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x000A94AA File Offset: 0x000A76AA
	public void OnSelectButton()
	{
		this.streetEdit.SetSelectedStreet(this.street);
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x000A94C0 File Offset: 0x000A76C0
	public void OnRandomNameButton()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("Random name button: " + this.street.name, 2);
		}
		this.streetEdit.SetSelectedStreet(this.street);
		this.street.isPlayerEditedName = false;
		this.street.UpdateName(true);
		this.street.isPlayerEditedName = true;
		this.selectButton.text.text = this.street.name;
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x000A9544 File Offset: 0x000A7744
	public void OnEditNameButton()
	{
		this.streetEdit.SetSelectedStreet(this.street);
		this.OnChangeStreetNameButton();
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x000A9560 File Offset: 0x000A7760
	public void OnChangeStreetNameButton()
	{
		PopupMessageController.Instance.PopupMessage("streetName", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.inputField.text = this.street.name;
		PopupMessageController.Instance.OnLeftButton += this.OnChangeStreetNamePopupCancel;
		PopupMessageController.Instance.OnRightButton += this.OnChangeStreetNamePopupConfirm;
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x000A95F5 File Offset: 0x000A77F5
	private void OnChangeStreetNamePopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeStreetNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeStreetNamePopupConfirm;
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x000A9624 File Offset: 0x000A7824
	private void OnChangeStreetNamePopupConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnChangeStreetNamePopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnChangeStreetNamePopupConfirm;
		this.selectButton.text.text = Strings.FilterInputtedText(PopupMessageController.Instance.inputField.text, true, 100);
		this.streetEdit.RenameSelectedStreet(this.selectButton.text.text);
	}

	// Token: 0x04000C03 RID: 3075
	[Header("References")]
	public CityEditorStreetEdit streetEdit;

	// Token: 0x04000C04 RID: 3076
	public StreetController street;

	// Token: 0x04000C05 RID: 3077
	public ButtonController selectButton;

	// Token: 0x04000C06 RID: 3078
	public ButtonController editNameButton;

	// Token: 0x04000C07 RID: 3079
	public ButtonController randomNameButton;

	// Token: 0x04000C08 RID: 3080
	public Image selectionImg;
}
