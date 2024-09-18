using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020003A2 RID: 930
public class BasicAnimationController : SwitchSyncBehaviour
{
	// Token: 0x06001525 RID: 5413 RVA: 0x00134497 File Offset: 0x00132697
	private void Start()
	{
		if (!this.isSetup)
		{
			this.Setup();
		}
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x001344A8 File Offset: 0x001326A8
	public void Setup()
	{
		this.closedLocalPos = this.animatedTransform.localPosition + this.preset.closedRelativePos;
		this.openLocalPos = this.animatedTransform.localPosition + this.preset.openRelativePos;
		this.closedLocalEuler = this.animatedTransform.localEulerAngles + this.preset.closedRelativeEuler;
		this.openLocalEuler = this.animatedTransform.localEulerAngles + this.preset.openRelativeEuler;
		this.closedLocalScale = this.animatedTransform.localScale + this.preset.closedRelativeScale;
		this.openLocalScale = this.animatedTransform.localScale + this.preset.openRelativeScale;
		this.isSetup = true;
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00134582 File Offset: 0x00132782
	public override void SetOn(bool val)
	{
		base.SetOn(val);
		if (val)
		{
			base.enabled = true;
		}
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00134598 File Offset: 0x00132798
	private void Update()
	{
		if (this.isOn && this.normalizedSpeed < 1f)
		{
			this.normalizedSpeed += Time.deltaTime;
		}
		else if (!this.isOn && this.normalizedSpeed > 0f)
		{
			this.normalizedSpeed -= Time.deltaTime;
			if (this.normalizedSpeed <= 0f)
			{
				base.enabled = false;
				return;
			}
		}
		if (this.oscillate)
		{
			if (!this.inOut)
			{
				this.progress += Time.deltaTime * this.preset.doorOpenSpeed * this.normalizedSpeed;
				if (this.progress >= 1f)
				{
					this.inOut = true;
				}
			}
			else
			{
				this.progress -= Time.deltaTime * this.preset.doorOpenSpeed * this.normalizedSpeed;
				if (this.progress <= 0f)
				{
					this.inOut = false;
				}
			}
		}
		else
		{
			this.progress += Time.deltaTime * this.preset.doorOpenSpeed * this.normalizedSpeed;
			if (this.progress >= 1f)
			{
				this.progress -= 1f;
			}
		}
		float num = this.preset.animationCurve.Evaluate(this.progress);
		this.animatedTransform.localEulerAngles = Vector3.Lerp(this.closedLocalEuler, this.openLocalEuler, num);
		this.animatedTransform.localPosition = Vector3.Lerp(this.closedLocalPos, this.openLocalPos, num);
		this.animatedTransform.localScale = Vector3.Lerp(this.closedLocalScale, this.openLocalScale, num);
	}

	// Token: 0x04001A19 RID: 6681
	public bool isSetup;

	// Token: 0x04001A1A RID: 6682
	public InteractableController controller;

	// Token: 0x04001A1B RID: 6683
	public DoorMovementPreset preset;

	// Token: 0x04001A1C RID: 6684
	public bool oscillate;

	// Token: 0x04001A1D RID: 6685
	public Transform animatedTransform;

	// Token: 0x04001A1E RID: 6686
	[Tooltip("This animator has a warm-up time")]
	[ReadOnly]
	public float normalizedSpeed;

	// Token: 0x04001A1F RID: 6687
	[ReadOnly]
	public float progress;

	// Token: 0x04001A20 RID: 6688
	[ReadOnly]
	public bool inOut;

	// Token: 0x04001A21 RID: 6689
	[Space(7f)]
	public Vector3 closedLocalPos = Vector3.zero;

	// Token: 0x04001A22 RID: 6690
	public Vector3 openLocalPos = Vector3.zero;

	// Token: 0x04001A23 RID: 6691
	public Vector3 closedLocalEuler = Vector3.zero;

	// Token: 0x04001A24 RID: 6692
	public Vector3 openLocalEuler = Vector3.zero;

	// Token: 0x04001A25 RID: 6693
	public Vector3 closedLocalScale = Vector3.one;

	// Token: 0x04001A26 RID: 6694
	public Vector3 openLocalScale = Vector3.one;
}
