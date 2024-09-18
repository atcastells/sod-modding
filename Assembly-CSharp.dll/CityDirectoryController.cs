using System;
using TMPro;
using UnityEngine;

// Token: 0x0200055C RID: 1372
public class CityDirectoryController : MonoBehaviour
{
	// Token: 0x06001DD7 RID: 7639 RVA: 0x001A42B4 File Offset: 0x001A24B4
	private void OnEnable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		if (this.windowContent == null)
		{
			this.windowContent = base.gameObject.GetComponentInParent<WindowContentController>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange += this.CheckEnabled;
		this.parentWindow.OnWindowRefresh += this.CheckEnabled;
		this.CheckEnabled();
	}

	// Token: 0x06001DD8 RID: 7640 RVA: 0x001A4338 File Offset: 0x001A2538
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001DD9 RID: 7641 RVA: 0x001A4398 File Offset: 0x001A2598
	public void CheckEnabled()
	{
		this.descriptionText.text = string.Empty;
		for (int i = 0; i < Toolbox.Instance.alphabet.Length; i++)
		{
			char c = Toolbox.Instance.alphabet[i];
			if (this.windowContent.tabController.preset.displayContentWithTag.StartsWith(c.ToString()))
			{
				this.descriptionText.text = string.Concat(new string[]
				{
					CityData.Instance.cityDirText[i].Trim(),
					"\n",
					CityData.Instance.cityDirText[i + 1].Trim(),
					"\n",
					CityData.Instance.cityDirText[i + 2].Trim()
				});
				return;
			}
		}
	}

	// Token: 0x040027B5 RID: 10165
	public WindowContentController windowContent;

	// Token: 0x040027B6 RID: 10166
	public InfoWindow parentWindow;

	// Token: 0x040027B7 RID: 10167
	public TextMeshProUGUI descriptionText;
}
