using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200055B RID: 1371
public class CitizenProfileProgressController : MonoBehaviour
{
	// Token: 0x06001DD3 RID: 7635 RVA: 0x001A4098 File Offset: 0x001A2298
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

	// Token: 0x06001DD4 RID: 7636 RVA: 0x001A4100 File Offset: 0x001A2300
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x001A4160 File Offset: 0x001A2360
	public void CheckEnabled()
	{
		List<Evidence.DataKey> tiedKeys = this.parentWindow.passedEvidence.GetTiedKeys(this.parentWindow.evidenceKeys);
		int num = 0;
		int count = this.parentWindow.passedEvidence.preset.GetValidProfileKeys().Count;
		foreach (Evidence.DataKey key in tiedKeys)
		{
			bool flag = false;
			if (this.parentWindow.passedEvidence.preset.IsKeyValid(key, out flag) && flag)
			{
				num++;
			}
		}
		int num2 = Mathf.RoundToInt((float)num / (float)count * 100f);
		this.progressText.text = Strings.Get("descriptors", "Profile", Strings.Casing.asIs, false, false, false, null) + "\n" + num2.ToString() + "%";
		if (num >= count && this.parentWindow.passedEvidence is EvidenceCitizen)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Citizen profile progress: " + num.ToString() + "/" + count.ToString(), 2);
			}
			if (AchievementsController.Instance != null)
			{
				AchievementsController.Instance.UnlockAchievement("Nosy Parker", "complete_profile");
			}
		}
	}

	// Token: 0x040027B3 RID: 10163
	public InfoWindow parentWindow;

	// Token: 0x040027B4 RID: 10164
	public TextMeshProUGUI progressText;
}
