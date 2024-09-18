using System;
using UnityEngine;

// Token: 0x02000540 RID: 1344
public class ToggleController : MonoBehaviour
{
	// Token: 0x06001D4C RID: 7500 RVA: 0x0019ED5C File Offset: 0x0019CF5C
	private void Start()
	{
		this.onButton.button.onClick.AddListener(delegate()
		{
			this.SetOn();
		});
		this.offButton.button.onClick.AddListener(delegate()
		{
			this.SetOff();
		});
	}

	// Token: 0x06001D4D RID: 7501 RVA: 0x0019EDAB File Offset: 0x0019CFAB
	public void SetIsOnWithoutNotify(bool val)
	{
		this.isOn = val;
		this.ButtonsVisualUpdate();
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x0019EDBA File Offset: 0x0019CFBA
	public void SetOn()
	{
		this.isOn = true;
		this.OnValueChange();
		this.ButtonsVisualUpdate();
	}

	// Token: 0x06001D4F RID: 7503 RVA: 0x0019EDCF File Offset: 0x0019CFCF
	public void SetOff()
	{
		this.isOn = false;
		this.OnValueChange();
		this.ButtonsVisualUpdate();
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x0019EDE4 File Offset: 0x0019CFE4
	public void ButtonsVisualUpdate()
	{
		if (this.isOn)
		{
			this.onButton.icon.enabled = true;
			this.offButton.icon.enabled = false;
			this.onButton.text.fontStyle = 4;
			this.offButton.text.fontStyle = 0;
			return;
		}
		this.onButton.icon.enabled = false;
		this.offButton.icon.enabled = true;
		this.onButton.text.fontStyle = 0;
		this.offButton.text.fontStyle = 4;
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x0019EE82 File Offset: 0x0019D082
	public void OnValueChange()
	{
		if (this.playerPrefsID != null && this.playerPrefsID.Length > 0)
		{
			PlayerPrefsController.Instance.OnToggleChanged(this.playerPrefsID, true, this);
		}
	}

	// Token: 0x0400270D RID: 9997
	[Header("Components")]
	public ButtonController onButton;

	// Token: 0x0400270E RID: 9998
	public ButtonController offButton;

	// Token: 0x0400270F RID: 9999
	[Header("State")]
	public bool isOn = true;

	// Token: 0x04002710 RID: 10000
	[Header("Configuration")]
	public string playerPrefsID;
}
