using System;
using UnityEngine;

// Token: 0x0200023D RID: 573
public class BreakerController : DoorMovementController
{
	// Token: 0x06000CEF RID: 3311 RVA: 0x000B8218 File Offset: 0x000B6418
	public override void SetOpen(float newAjar, Actor interactor, bool skipAnimation = false)
	{
		newAjar = Mathf.Clamp01(this.interactable.val);
		base.SetOpen(newAjar, interactor, skipAnimation);
	}
}
