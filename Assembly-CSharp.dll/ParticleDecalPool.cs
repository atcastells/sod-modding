using System;
using UnityEngine;

// Token: 0x02000436 RID: 1078
public class ParticleDecalPool : MonoBehaviour
{
	// Token: 0x06001838 RID: 6200 RVA: 0x00169E24 File Offset: 0x00168024
	private void Start()
	{
		this.decalParticleSystem = base.GetComponent<ParticleSystem>();
		this.particles = new ParticleSystem.Particle[this.maxDecals];
		this.particleData = new ParticleDecalData[this.maxDecals];
		for (int i = 0; i < this.maxDecals; i++)
		{
			this.particleData[i] = new ParticleDecalData();
		}
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x00169E7D File Offset: 0x0016807D
	public void ParticleHit(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
	{
		this.SetParticleData(particleCollisionEvent, colorGradient);
		this.DisplayParticles();
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x00169E90 File Offset: 0x00168090
	private void SetParticleData(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
	{
		if (this.particleDecalDataIndex >= this.maxDecals)
		{
			this.particleDecalDataIndex = 0;
		}
		this.particleData[this.particleDecalDataIndex].position = particleCollisionEvent.intersection;
		Vector3 eulerAngles = Quaternion.LookRotation(particleCollisionEvent.normal).eulerAngles;
		eulerAngles.z = (float)Toolbox.Instance.Rand(0, 360, false);
		this.particleData[this.particleDecalDataIndex].rotation = eulerAngles;
		this.particleData[this.particleDecalDataIndex].size = Toolbox.Instance.Rand(this.decalSizeMin, this.decalSizeMax, false);
		this.particleData[this.particleDecalDataIndex].color = colorGradient.Evaluate(Toolbox.Instance.Rand(0f, 1f, false));
		this.particleDecalDataIndex++;
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x00169F70 File Offset: 0x00168170
	private void DisplayParticles()
	{
		for (int i = 0; i < this.particleData.Length; i++)
		{
			this.particles[i].position = this.particleData[i].position;
			this.particles[i].rotation3D = this.particleData[i].rotation;
			this.particles[i].startSize = this.particleData[i].size;
			this.particles[i].startColor = this.particleData[i].color;
		}
		this.decalParticleSystem.SetParticles(this.particles, this.particles.Length);
	}

	// Token: 0x04001E2B RID: 7723
	public int maxDecals = 100;

	// Token: 0x04001E2C RID: 7724
	public float decalSizeMin = 0.5f;

	// Token: 0x04001E2D RID: 7725
	public float decalSizeMax = 1.5f;

	// Token: 0x04001E2E RID: 7726
	private ParticleSystem decalParticleSystem;

	// Token: 0x04001E2F RID: 7727
	private int particleDecalDataIndex;

	// Token: 0x04001E30 RID: 7728
	private ParticleDecalData[] particleData;

	// Token: 0x04001E31 RID: 7729
	private ParticleSystem.Particle[] particles;
}
