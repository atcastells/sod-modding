using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000438 RID: 1080
public class SplatOnCollision : MonoBehaviour
{
	// Token: 0x06001842 RID: 6210 RVA: 0x0016A1D9 File Offset: 0x001683D9
	private void Start()
	{
		this.collisionEvents = new List<ParticleCollisionEvent>();
	}

	// Token: 0x06001843 RID: 6211 RVA: 0x0016A1E8 File Offset: 0x001683E8
	private void OnParticleCollision(GameObject other)
	{
		int num = ParticlePhysicsExtensions.GetCollisionEvents(this.particleLauncher, other, this.collisionEvents);
		for (int i = 0; i < num; i++)
		{
			this.dropletDecalPool.ParticleHit(this.collisionEvents[i], this.particleColorGradient);
		}
	}

	// Token: 0x04001E38 RID: 7736
	public ParticleSystem particleLauncher;

	// Token: 0x04001E39 RID: 7737
	public Gradient particleColorGradient;

	// Token: 0x04001E3A RID: 7738
	public ParticleDecalPool dropletDecalPool;

	// Token: 0x04001E3B RID: 7739
	private List<ParticleCollisionEvent> collisionEvents;
}
