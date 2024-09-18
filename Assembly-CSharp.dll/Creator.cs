using System;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class Creator : MonoBehaviour
{
	// Token: 0x060009A4 RID: 2468 RVA: 0x0009499C File Offset: 0x00092B9C
	public virtual void StartLoading()
	{
		this.SetComplete();
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x000949A4 File Offset: 0x00092BA4
	public void SetComplete()
	{
		CityConstructor.Instance.loadingProgress = 1f;
		CityConstructor.Instance.stateComplete = true;
		base.StopAllCoroutines();
		base.enabled = false;
	}
}
