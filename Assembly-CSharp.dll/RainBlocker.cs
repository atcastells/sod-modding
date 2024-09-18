using System;
using UnityEngine;

// Token: 0x0200041A RID: 1050
public class RainBlocker : MonoBehaviour
{
	// Token: 0x060017E5 RID: 6117 RVA: 0x00165718 File Offset: 0x00163918
	private void OnEnable()
	{
		if (this.rainCollider != null)
		{
			this.rainCollider.isTrigger = true;
			base.gameObject.layer = 2;
			if (PrecipitationParticleSystemController.Instance != null)
			{
				PrecipitationParticleSystemController.Instance.AddAreaTrigger(this.rainCollider);
			}
		}
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x00165768 File Offset: 0x00163968
	private void OnDestroy()
	{
		if (PrecipitationParticleSystemController.Instance != null)
		{
			PrecipitationParticleSystemController.Instance.RemoveAreaTrigger(this.rainCollider);
		}
	}

	// Token: 0x04001D77 RID: 7543
	public Collider rainCollider;
}
