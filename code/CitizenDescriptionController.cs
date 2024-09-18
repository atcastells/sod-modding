using System;
using TMPro;
using UnityEngine;

// Token: 0x0200055A RID: 1370
public class CitizenDescriptionController : MonoBehaviour
{
	// Token: 0x06001DCF RID: 7631 RVA: 0x001A3FA8 File Offset: 0x001A21A8
	private void OnEnable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange += this.CheckEnabled;
		this.parentWindow.OnWindowRefresh += this.CheckEnabled;
		this.CheckEnabled();
	}

	// Token: 0x06001DD0 RID: 7632 RVA: 0x001A4010 File Offset: 0x001A2210
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x001A406F File Offset: 0x001A226F
	public void CheckEnabled()
	{
		this.descriptionText.text = this.parentWindow.passedEvidence.GetNoteComposed(this.parentWindow.evidenceKeys, true);
	}

	// Token: 0x040027B1 RID: 10161
	public InfoWindow parentWindow;

	// Token: 0x040027B2 RID: 10162
	public TextMeshProUGUI descriptionText;
}
