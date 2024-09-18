using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005F2 RID: 1522
public class PasscodesEntryController : MonoBehaviour
{
	// Token: 0x0600217A RID: 8570 RVA: 0x001C7F50 File Offset: 0x001C6150
	public void Setup(GameplayController.Passcode newPasscode)
	{
		this.passcode = newPasscode;
		this.locateOnMapButton.SetupReferences();
		this.enterCodeButton.SetupReferences();
		this.locateOnMapButton.VisualUpdate();
		this.enterCodeButton.VisualUpdate();
		if (this.passcode.type == GameplayController.PasscodeType.address)
		{
			if (CityData.Instance.addressDictionary.TryGetValue(this.passcode.id, ref this.address))
			{
				this.evidence = this.address.evidenceEntry;
				this.icon.sprite = this.evidence.GetIcon();
			}
		}
		else if (this.passcode.type == GameplayController.PasscodeType.citizen)
		{
			if (CityData.Instance.GetHuman(this.passcode.id, out this.human, true))
			{
				this.evidence = this.human.evidenceEntry;
				this.icon.sprite = this.evidence.GetIcon();
			}
			this.locateOnMapButton.SetInteractable(false);
		}
		else if (this.passcode.type == GameplayController.PasscodeType.room)
		{
			if (CityData.Instance.roomDictionary.TryGetValue(this.passcode.id, ref this.room))
			{
				this.evidence = this.room.gameLocation.evidenceEntry;
				this.icon.sprite = this.evidence.GetIcon();
			}
		}
		else if (this.passcode.type == GameplayController.PasscodeType.interactable)
		{
			this.interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.passcode.id);
			if (this.interactable != null)
			{
				this.evidence = this.interactable.evidence;
				if (this.evidence != null)
				{
					this.icon.sprite = this.evidence.GetIcon();
				}
				if (this.evidence == null)
				{
					this.evidence = this.interactable.node.gameLocation.evidenceEntry;
				}
			}
		}
		if (this.evidence != null)
		{
			this.evidence.OnNewName += this.VisualUpdate;
			this.evidence.OnDataKeyChange += this.VisualUpdate;
		}
		this.VisualUpdate();
		InterfaceController.Instance.OnNewActiveCodeInput += this.ActiveCodeInputCheck;
		this.ActiveCodeInputCheck(InterfaceController.Instance.activeCodeInput);
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x001C81A4 File Offset: 0x001C63A4
	public void VisualUpdate()
	{
		if (this.evidenceImage != null && this.evidence != null)
		{
			this.evidenceImage.texture = this.evidence.GetPhoto(this.evidence.GetTiedKeys(Evidence.DataKey.name));
		}
		if (this.passcode.type == GameplayController.PasscodeType.room)
		{
			this.nameString = this.room.GetName() + ", " + this.evidence.GetNameForDataKey(Evidence.DataKey.name);
		}
		else if (this.passcode.type == GameplayController.PasscodeType.interactable)
		{
			this.nameString = this.interactable.GetName() + ", " + this.evidence.GetNameForDataKey(Evidence.DataKey.name);
		}
		else if (this.evidence != null)
		{
			this.nameString = this.evidence.GetNameForDataKey(Evidence.DataKey.name);
		}
		this.passcodeString = this.passcode.GetDigit(0).ToString() + this.passcode.GetDigit(1).ToString() + this.passcode.GetDigit(2).ToString() + this.passcode.GetDigit(3).ToString();
		this.text.text = this.nameString + ": " + this.passcodeString;
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x001C82ED File Offset: 0x001C64ED
	public void ActiveCodeInputCheck(KeypadController keypad)
	{
		if (InterfaceController.Instance.activeCodeInput != null && !InterfaceController.Instance.activeCodeInput.isTelephone)
		{
			this.enterCodeButton.SetInteractable(true);
			return;
		}
		this.enterCodeButton.SetInteractable(false);
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x001C832C File Offset: 0x001C652C
	private void OnDestroy()
	{
		InterfaceController.Instance.OnNewActiveCodeInput -= this.ActiveCodeInputCheck;
		if (this.evidence != null)
		{
			this.evidence.OnNewName -= this.VisualUpdate;
			this.evidence.OnDataKeyChange -= this.VisualUpdate;
		}
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x001C8388 File Offset: 0x001C6588
	public void OpenEvidence()
	{
		if (this.evidence != null)
		{
			InterfaceController.Instance.SpawnWindow(this.evidence, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
		}
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x001C83C4 File Offset: 0x001C65C4
	public void LocateOnMap()
	{
		if (this.passcode.type == GameplayController.PasscodeType.address)
		{
			MapController.Instance.LocateEvidenceOnMap(this.evidence);
			return;
		}
		if (this.passcode.type == GameplayController.PasscodeType.room)
		{
			MapController.Instance.LocateRoomOnMap(this.room);
			return;
		}
		if (this.passcode.type == GameplayController.PasscodeType.interactable)
		{
			if (!InterfaceController.Instance.showDesktopMap)
			{
				InterfaceController.Instance.SetShowDesktopMap(true, true);
			}
			MapController.Instance.SetFloorLayer(this.interactable.node.nodeCoord.z, false);
			MapController.Instance.CentreOnNodeCoordinate(this.interactable.node.nodeCoord, false, false);
		}
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x001C8478 File Offset: 0x001C6678
	public void EnterCode()
	{
		if (InterfaceController.Instance.activeCodeInput != null && !InterfaceController.Instance.activeCodeInput.isTelephone)
		{
			InterfaceController.Instance.activeCodeInput.parentWindow.SetActiveContent(InterfaceController.Instance.activeCodeInput.windowContent);
			InterfaceController.Instance.activeCodeInput.OnInputCode(this.passcode.GetDigits());
			this.enterCodeButton.OnDeselect();
		}
	}

	// Token: 0x04002BC9 RID: 11209
	public NewAddress address;

	// Token: 0x04002BCA RID: 11210
	public NewRoom room;

	// Token: 0x04002BCB RID: 11211
	public Human human;

	// Token: 0x04002BCC RID: 11212
	public Interactable interactable;

	// Token: 0x04002BCD RID: 11213
	public Evidence evidence;

	// Token: 0x04002BCE RID: 11214
	public GameplayController.Passcode passcode;

	// Token: 0x04002BCF RID: 11215
	public TextMeshProUGUI text;

	// Token: 0x04002BD0 RID: 11216
	public ButtonController locateOnMapButton;

	// Token: 0x04002BD1 RID: 11217
	public ButtonController enterCodeButton;

	// Token: 0x04002BD2 RID: 11218
	public RawImage evidenceImage;

	// Token: 0x04002BD3 RID: 11219
	public Image icon;

	// Token: 0x04002BD4 RID: 11220
	public string nameString;

	// Token: 0x04002BD5 RID: 11221
	public string passcodeString;
}
