using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020003A5 RID: 933
public class ClockController : MonoBehaviour
{
	// Token: 0x06001531 RID: 5425 RVA: 0x001347D8 File Offset: 0x001329D8
	private void Start()
	{
		if (this.ic == null)
		{
			this.ic = base.gameObject.GetComponentInParent<InteractableController>();
		}
		if (this.hourlyAnimation != null)
		{
			SessionData.Instance.OnHourChange += this.OnHourChange;
		}
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x00134828 File Offset: 0x00132A28
	private void OnDestroy()
	{
		if (this.hourlyAnimation != null)
		{
			this.hourlyAnimation.SetBool("Animate", false);
			SessionData.Instance.OnHourChange -= this.OnHourChange;
		}
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x00134860 File Offset: 0x00132A60
	public void OnHourChange()
	{
		if (this.hourlyAnimation != null)
		{
			this.hourlyAnimation.SetBool("Animate", true);
			if (this.ic.interactable.preset.chimeEqualToHour)
			{
				int num = Mathf.FloorToInt(SessionData.Instance.decimalClock);
				this.animateTimer = this.ic.interactable.preset.chimeDelay * (float)num;
				return;
			}
			this.animateTimer = this.ic.interactable.preset.chimeDelay;
		}
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x001348F0 File Offset: 0x00132AF0
	private void Update()
	{
		float num = SessionData.Instance.decimalClock;
		if (num > 12f)
		{
			num = SessionData.Instance.decimalClock - 12f;
		}
		this.hourHand.localEulerAngles = new Vector3(0f, 0f, num / 12f * 360f);
		float num2 = SessionData.Instance.decimalClock - Mathf.Floor(SessionData.Instance.decimalClock);
		this.minuteHand.localEulerAngles = new Vector3(0f, 0f, num2 * 360f);
		if (this.animateTimer > 0f)
		{
			this.animateTimer -= Time.deltaTime;
			if (this.animateTimer <= 0f)
			{
				this.hourlyAnimation.SetBool("Animate", false);
			}
		}
	}

	// Token: 0x04001A29 RID: 6697
	public InteractableController ic;

	// Token: 0x04001A2A RID: 6698
	public Transform hourHand;

	// Token: 0x04001A2B RID: 6699
	public Transform minuteHand;

	// Token: 0x04001A2C RID: 6700
	public Animator hourlyAnimation;

	// Token: 0x04001A2D RID: 6701
	[ReadOnly]
	public float animateTimer;
}
