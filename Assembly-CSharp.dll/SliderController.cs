using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200053C RID: 1340
public class SliderController : MonoBehaviour
{
	// Token: 0x06001D38 RID: 7480 RVA: 0x0019E890 File Offset: 0x0019CA90
	private void Start()
	{
		this.nextButton.button.onClick.AddListener(delegate()
		{
			this.OnNextButton();
		});
		this.prevButton.button.onClick.AddListener(delegate()
		{
			this.OnPreviousButton();
		});
		if (this.label != null)
		{
			MenuAutoTextController component = this.label.GetComponent<MenuAutoTextController>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		this.UpdateDisplayValue();
	}

	// Token: 0x06001D39 RID: 7481 RVA: 0x0019E910 File Offset: 0x0019CB10
	public void OnNextButton()
	{
		if (!this.clickThisFrame)
		{
			Slider slider = this.slider;
			float value = slider.value;
			slider.value = value + 1f;
			this.clickThisFrame = true;
			base.StartCoroutine(this.RunEnd());
		}
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x0019E954 File Offset: 0x0019CB54
	public void OnPreviousButton()
	{
		if (!this.clickThisFrame)
		{
			Slider slider = this.slider;
			float value = slider.value;
			slider.value = value - 1f;
			this.clickThisFrame = true;
			base.StartCoroutine(this.RunEnd());
		}
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x0019E996 File Offset: 0x0019CB96
	private IEnumerator RunEnd()
	{
		bool runOnce = true;
		while (runOnce)
		{
			runOnce = false;
			yield return new WaitForEndOfFrame();
		}
		this.clickThisFrame = false;
		yield break;
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x0019E9A5 File Offset: 0x0019CBA5
	public void SetValueWithoutNotify(int newVal)
	{
		this.slider.SetValueWithoutNotify((float)newVal);
		this.UpdateDisplayValue();
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x0019E9BA File Offset: 0x0019CBBA
	public void OnValueChange()
	{
		if (this.playerPrefsID != null && this.playerPrefsID.Length > 0)
		{
			PlayerPrefsController.Instance.OnToggleChanged(this.playerPrefsID, true, this);
		}
		this.UpdateDisplayValue();
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x0019E9EC File Offset: 0x0019CBEC
	public void UpdateDisplayValue()
	{
		if (this.label != null && this.labelDictRef != null && this.labelDictRef.Length > 0)
		{
			if (Strings.textFilesLoaded)
			{
				this.label.text = Strings.Get("ui.interface", this.labelDictRef, Strings.Casing.asIs, false, false, false, null);
			}
			if (this.displayValue)
			{
				string text = string.Empty;
				if (this.isPercentage)
				{
					text = "%";
				}
				TextMeshProUGUI textMeshProUGUI = this.label;
				textMeshProUGUI.text = textMeshProUGUI.text + ": " + this.slider.value.ToString() + text;
			}
		}
	}

	// Token: 0x040026F6 RID: 9974
	[Header("Components")]
	public Slider slider;

	// Token: 0x040026F7 RID: 9975
	public ButtonController prevButton;

	// Token: 0x040026F8 RID: 9976
	public ButtonController nextButton;

	// Token: 0x040026F9 RID: 9977
	public TextMeshProUGUI label;

	// Token: 0x040026FA RID: 9978
	[Header("Configuration")]
	public string labelDictRef;

	// Token: 0x040026FB RID: 9979
	public string playerPrefsID;

	// Token: 0x040026FC RID: 9980
	public bool displayValue = true;

	// Token: 0x040026FD RID: 9981
	public bool isPercentage = true;

	// Token: 0x040026FE RID: 9982
	private bool clickThisFrame;
}
