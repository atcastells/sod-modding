using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005C7 RID: 1479
public class PulseGlowController : MonoBehaviour
{
	// Token: 0x0600206A RID: 8298 RVA: 0x001BC616 File Offset: 0x001BA816
	private void Awake()
	{
		this.SetGlow(this.glowActiveOnStart);
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x001BC624 File Offset: 0x001BA824
	public void SetGlow(bool onOff)
	{
		if (!this.glowActive && onOff)
		{
			this.glowActive = true;
			base.enabled = true;
			return;
		}
		if (this.glowActive && !onOff)
		{
			this.glowActive = false;
			if (this.imageToGlow != null)
			{
				this.imageToGlow.color = this.originalColour;
			}
			if (this.rawImageToGlow != null)
			{
				this.rawImageToGlow.color = this.originalColour;
			}
			if (this.textToGlow != null)
			{
				this.textToGlow.color = this.originalColour;
			}
			base.enabled = false;
		}
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x001BC6C4 File Offset: 0x001BA8C4
	private void Update()
	{
		if (!this.glowActive)
		{
			base.enabled = false;
			return;
		}
		if (this.glowState < 1f && this.glowSwitch)
		{
			this.glowState += Time.deltaTime * this.pulseSpeed;
		}
		else if (this.glowState >= 1f && this.glowSwitch)
		{
			this.glowSwitch = false;
		}
		if (this.glowState > 0f && !this.glowSwitch)
		{
			this.glowState -= Time.deltaTime * this.pulseSpeed;
		}
		else if (this.glowState <= 0f && !this.glowSwitch)
		{
			this.glowSwitch = true;
		}
		this.glowState = Mathf.Clamp01(this.glowState);
		if (this.useLerpColour)
		{
			if (this.imageToGlow != null)
			{
				this.imageToGlow.color = Color.Lerp(this.originalColour, this.lerpColour, this.glowState);
			}
			if (this.textToGlow != null)
			{
				this.textToGlow.color = Color.Lerp(this.originalColour, this.lerpColour, this.glowState);
			}
			if (this.rawImageToGlow != null)
			{
				this.rawImageToGlow.color = Color.Lerp(this.originalColour, this.lerpColour, this.glowState);
			}
		}
	}

	// Token: 0x04002A82 RID: 10882
	public Image imageToGlow;

	// Token: 0x04002A83 RID: 10883
	public RawImage rawImageToGlow;

	// Token: 0x04002A84 RID: 10884
	public TextMeshProUGUI textToGlow;

	// Token: 0x04002A85 RID: 10885
	public bool glowActiveOnStart;

	// Token: 0x04002A86 RID: 10886
	public bool glowActive;

	// Token: 0x04002A87 RID: 10887
	public float pulseSpeed = 1f;

	// Token: 0x04002A88 RID: 10888
	private float glowState;

	// Token: 0x04002A89 RID: 10889
	private bool glowSwitch;

	// Token: 0x04002A8A RID: 10890
	public bool useLerpColour = true;

	// Token: 0x04002A8B RID: 10891
	public Color originalColour = Color.white;

	// Token: 0x04002A8C RID: 10892
	public Color lerpColour = Color.grey;
}
