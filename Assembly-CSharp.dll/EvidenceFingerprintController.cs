using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000565 RID: 1381
public class EvidenceFingerprintController : MonoBehaviour
{
	// Token: 0x06001E0A RID: 7690 RVA: 0x001A57D0 File Offset: 0x001A39D0
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

	// Token: 0x06001E0B RID: 7691 RVA: 0x001A5838 File Offset: 0x001A3A38
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x001A5898 File Offset: 0x001A3A98
	public void CheckEnabled()
	{
		Game.Log("Interface: Check for fingerprint key in parent window...", 2);
		Game.Log("Player: Fingerprints belong to " + this.parentWindow.passedEvidence.writer.GetCitizenName(), 2);
		if (this.parentWindow.evidenceKeys.Contains(Evidence.DataKey.fingerprints))
		{
			if (this.parentWindow.passedEvidence.writer.fingerprintLoop <= -1)
			{
				this.parentWindow.passedEvidence.writer.fingerprintLoop = GameplayController.Instance.printsLetterLoop;
				GameplayController.Instance.printsLetterLoop++;
			}
			this.photoRaw.texture = CitizenControls.Instance.prints[Toolbox.Instance.GetPsuedoRandomNumber(0, CitizenControls.Instance.prints.Count, this.parentWindow.passedEvidence.writer.citizenName + this.parentWindow.passedEvidence.writer.humanID.ToString(), false)];
			this.identifierText.text = Strings.Get("evidence.generic", "Type", Strings.Casing.firstLetterCaptial, false, false, false, null) + " " + Toolbox.Instance.ToBase26(this.parentWindow.passedEvidence.writer.fingerprintLoop);
			this.identifierText.gameObject.SetActive(true);
		}
		else
		{
			this.photoRaw.texture = CitizenControls.Instance.unknownPrint;
			this.identifierText.gameObject.SetActive(false);
		}
		this.photoRaw.color = Color.white;
	}

	// Token: 0x040027E4 RID: 10212
	public InfoWindow parentWindow;

	// Token: 0x040027E5 RID: 10213
	public RawImage photoRaw;

	// Token: 0x040027E6 RID: 10214
	public TextMeshProUGUI identifierText;
}
