using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003B4 RID: 948
public class FridgeDoorLightController : MonoBehaviour
{
	// Token: 0x06001579 RID: 5497 RVA: 0x001397DC File Offset: 0x001379DC
	private void Start()
	{
		if (this.ic != null && this.ic.interactable != null)
		{
			this.ic.interactable.OnSwitchChange += this.OnSwitchStateChange;
		}
		this.lightContainer.gameObject.SetActive(false);
		this.OnSwitchStateChange();
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x00139837 File Offset: 0x00137A37
	private void OnDestroy()
	{
		if (this.ic != null && this.ic.interactable != null)
		{
			this.ic.interactable.OnSwitchChange -= this.OnSwitchStateChange;
		}
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x00139870 File Offset: 0x00137A70
	public void OnSwitchStateChange()
	{
		base.StopAllCoroutines();
		if (this.ic.interactable != null && this.ic.interactable.sw0)
		{
			this.lightContainer.gameObject.SetActive(true);
			return;
		}
		if (base.isActiveAndEnabled)
		{
			base.StartCoroutine(this.LightOffDelay());
			return;
		}
		this.lightContainer.gameObject.SetActive(false);
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x001398DB File Offset: 0x00137ADB
	private IEnumerator LightOffDelay()
	{
		float timer = 0f;
		while (timer < 0.8f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		this.lightContainer.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x04001AAE RID: 6830
	public GameObject lightContainer;

	// Token: 0x04001AAF RID: 6831
	public InteractableController ic;
}
