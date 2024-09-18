using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class FirstPersonAnimationEvents : MonoBehaviour
{
	// Token: 0x060001AC RID: 428 RVA: 0x0000E271 File Offset: 0x0000C471
	public void Punch()
	{
		FirstPersonItemController.Instance.MeleeAttack();
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000E27D File Offset: 0x0000C47D
	public void CounterAttack()
	{
		FirstPersonItemController.Instance.CounterAttack();
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000E289 File Offset: 0x0000C489
	public void ThrowCoin()
	{
		FirstPersonItemController.Instance.ThrowCoin();
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000E295 File Offset: 0x0000C495
	public void ThrowFood()
	{
		FirstPersonItemController.Instance.ThrowFood();
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000E2A1 File Offset: 0x0000C4A1
	public void ThrowGrenade()
	{
		FirstPersonItemController.Instance.ThrowGrenade();
	}
}
