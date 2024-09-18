using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000437 RID: 1079
public class ParticleLauncher : MonoBehaviour
{
	// Token: 0x0600183D RID: 6205 RVA: 0x00002265 File Offset: 0x00000465
	private void Awake()
	{
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x0016A054 File Offset: 0x00168254
	private void OnParticleCollision(GameObject other)
	{
		ParticlePhysicsExtensions.GetCollisionEvents(this.particleLauncher, other, this.collisionEvents);
		for (int i = 0; i < this.collisionEvents.Count; i++)
		{
			this.splatDecalPool.ParticleHit(this.collisionEvents[i], this.particleColorGradient);
			this.EmitAtLocation(this.collisionEvents[i]);
		}
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x0016A0BC File Offset: 0x001682BC
	private void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
	{
		this.splatterParticles.transform.position = particleCollisionEvent.intersection;
		this.splatterParticles.transform.rotation = Quaternion.LookRotation(particleCollisionEvent.normal);
		this.splatterParticles.main.startColor = this.particleColorGradient.Evaluate(Toolbox.Instance.Rand(0f, 1f, false));
		this.splatterParticles.Emit(1);
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x0016A140 File Offset: 0x00168340
	private void Update()
	{
		if (this.particles > 0)
		{
			this.particleLauncher.main.startColor = this.particleColorGradient.Evaluate(Toolbox.Instance.Rand(0f, 1f, false));
			this.particleLauncher.Emit(1);
			this.particles--;
			return;
		}
		Object.Destroy(base.transform.parent.gameObject);
	}

	// Token: 0x04001E32 RID: 7730
	public int particles = 50;

	// Token: 0x04001E33 RID: 7731
	public ParticleSystem particleLauncher;

	// Token: 0x04001E34 RID: 7732
	public ParticleSystem splatterParticles;

	// Token: 0x04001E35 RID: 7733
	public Gradient particleColorGradient;

	// Token: 0x04001E36 RID: 7734
	public ParticleDecalPool splatDecalPool;

	// Token: 0x04001E37 RID: 7735
	private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
}
