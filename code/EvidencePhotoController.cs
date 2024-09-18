using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000567 RID: 1383
public class EvidencePhotoController : MonoBehaviour
{
	// Token: 0x06001E1E RID: 7710 RVA: 0x001A64BC File Offset: 0x001A46BC
	private void OnEnable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange += this.CheckEnabled;
		this.CheckEnabled();
	}

	// Token: 0x06001E1F RID: 7711 RVA: 0x001A650A File Offset: 0x001A470A
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
	}

	// Token: 0x06001E20 RID: 7712 RVA: 0x001A6548 File Offset: 0x001A4748
	public void CheckEnabled()
	{
		Game.Log("Interface: Check for photo key in parent window...", 2);
		this.photoRaw.texture = this.parentWindow.passedEvidence.GetPhoto(this.parentWindow.evidenceKeys);
		this.photoRaw.color = Color.white;
	}

	// Token: 0x040027F2 RID: 10226
	public InfoWindow parentWindow;

	// Token: 0x040027F3 RID: 10227
	public RawImage photoRaw;
}
