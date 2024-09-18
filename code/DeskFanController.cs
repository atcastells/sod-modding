using System;
using UnityEngine;

// Token: 0x020003A8 RID: 936
public class DeskFanController : SwitchSyncBehaviour
{
	// Token: 0x06001549 RID: 5449 RVA: 0x00135D20 File Offset: 0x00133F20
	public override void SetOn(bool val)
	{
		base.SetOn(val);
		if (this.speedProgress > 0f || this.isOn)
		{
			base.enabled = true;
		}
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x00135D48 File Offset: 0x00133F48
	private void Update()
	{
		if (this.isOn && this.speedProgress < 1f)
		{
			this.speedProgress += Time.deltaTime * 0.4f;
			this.speedProgress = Mathf.Clamp01(this.speedProgress);
		}
		else if (!this.isOn && this.speedProgress > 0f)
		{
			this.speedProgress -= Time.deltaTime * 0.15f;
			this.speedProgress = Mathf.Clamp01(this.speedProgress);
		}
		this.fanBlade.localEulerAngles = new Vector3(0f, 0f, this.fanBlade.localEulerAngles.z + this.fanSpeed * this.speedProgress);
		if (!this.isOn && this.speedProgress <= 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04001A4D RID: 6733
	public InteractableController ic;

	// Token: 0x04001A4E RID: 6734
	public Transform fanBlade;

	// Token: 0x04001A4F RID: 6735
	public float speedProgress;

	// Token: 0x04001A50 RID: 6736
	public float fanSpeed = 5f;
}
