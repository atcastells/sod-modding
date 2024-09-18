using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class CitizenAnimationEvents : MonoBehaviour
{
	// Token: 0x060001A3 RID: 419 RVA: 0x0000E19C File Offset: 0x0000C39C
	public void SetStaticAnimation()
	{
		this.ai.SetStaticFromAnimation(true);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000E1AA File Offset: 0x0000C3AA
	public void ResetStaticAnimation()
	{
		this.ai.SetStaticFromAnimation(false);
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000E1B8 File Offset: 0x0000C3B8
	public void ResetOneShotUse()
	{
		if (this.ai.human.animationController.armsBoolAnimationState == CitizenAnimationController.ArmsBoolSate.armsOneShotUse)
		{
			this.ai.human.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
		}
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000E1E8 File Offset: 0x0000C3E8
	public void FootstepLeft()
	{
		this.ai.human.OnFootstep(false);
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000E1FB File Offset: 0x0000C3FB
	public void FootstepRight()
	{
		this.ai.human.OnFootstep(true);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000E210 File Offset: 0x0000C410
	public void TripKO()
	{
		this.ai.isTripping = false;
		this.ai.human.animationController.CancelTrip();
		NewAIController newAIController = this.ai;
		bool val = true;
		Vector3 forward = this.ai.transform.forward;
		newAIController.SetKO(val, default(Vector3), forward, true, 0.002f, false, 1f);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x00002265 File Offset: 0x00000465
	public void MeleeAttackTrigger()
	{
	}

	// Token: 0x060001AA RID: 426 RVA: 0x00002265 File Offset: 0x00000465
	public void MeleeAttackRemove()
	{
	}

	// Token: 0x04000107 RID: 263
	public NewAIController ai;
}
