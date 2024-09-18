using System;
using UnityEngine;

// Token: 0x020004BF RID: 1215
public class PrecipitationParticleSystemController : MonoBehaviour
{
	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06001A4C RID: 6732 RVA: 0x00182BCF File Offset: 0x00180DCF
	public static PrecipitationParticleSystemController Instance
	{
		get
		{
			return PrecipitationParticleSystemController._instance;
		}
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x00182BD6 File Offset: 0x00180DD6
	private void Awake()
	{
		if (PrecipitationParticleSystemController._instance != null && PrecipitationParticleSystemController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			PrecipitationParticleSystemController._instance = this;
		}
		this.SetSnowMode(false, true);
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x00182C10 File Offset: 0x00180E10
	public void SetSnowMode(bool val, bool forceUpdate = false)
	{
		if (val != this.snowMode || forceUpdate)
		{
			this.snowMode = val;
			if (base.enabled)
			{
				this.snowSystem.gameObject.SetActive(this.snowMode);
				this.rainSystem.gameObject.SetActive(!this.snowMode);
			}
		}
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x00182C6C File Offset: 0x00180E6C
	public void SetEnabled(bool val)
	{
		if (val)
		{
			this.snowSystem.gameObject.SetActive(this.snowMode);
			this.rainSystem.gameObject.SetActive(!this.snowMode);
		}
		else
		{
			this.snowSystem.gameObject.SetActive(false);
			this.rainSystem.gameObject.SetActive(false);
		}
		base.enabled = val;
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x00182CD8 File Offset: 0x00180ED8
	public void AddAreaTrigger(Collider coll)
	{
		this.snowSystem.trigger.AddCollider(coll);
		this.rainSystem.trigger.AddCollider(coll);
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x00182D10 File Offset: 0x00180F10
	public void RemoveAreaTrigger(Collider coll)
	{
		this.snowSystem.trigger.RemoveCollider(coll);
		this.rainSystem.trigger.RemoveCollider(coll);
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x00182D48 File Offset: 0x00180F48
	private void Update()
	{
		if (SessionData.Instance.currentRain > 0f || SessionData.Instance.currentSnow > 0f)
		{
			base.transform.position = new Vector3(Player.Instance.transform.position.x, 6f, Player.Instance.transform.position.z);
			if (this.snowMode)
			{
				this.snowSystem.emission.rateOverTime = SessionData.Instance.currentSnow * (float)this.snowMaxEmissionRate;
				if (Player.Instance.currentNode != null)
				{
					if (Player.Instance.currentNode.isOutside && this.snowSystem.isStopped)
					{
						this.snowSystem.Play();
						return;
					}
					if (!Player.Instance.currentNode.isOutside && this.snowSystem.isPlaying)
					{
						this.snowSystem.Stop();
						return;
					}
				}
			}
			else
			{
				this.rainSystem.emission.rateOverTime = SessionData.Instance.currentRain * (float)this.rainMaxEmissionRate;
				if (Player.Instance.currentNode != null)
				{
					if (Player.Instance.currentNode.isOutside && this.rainSystem.isStopped)
					{
						this.rainSystem.Play();
						return;
					}
					if (!Player.Instance.currentNode.isOutside && this.rainSystem.isPlaying)
					{
						this.rainSystem.Stop();
					}
				}
			}
		}
	}

	// Token: 0x040022DC RID: 8924
	[Header("Components")]
	public ParticleSystem snowSystem;

	// Token: 0x040022DD RID: 8925
	public ParticleSystem rainSystem;

	// Token: 0x040022DE RID: 8926
	[Header("Settings")]
	public int snowMaxEmissionRate = 2500;

	// Token: 0x040022DF RID: 8927
	public int rainMaxEmissionRate = 18000;

	// Token: 0x040022E0 RID: 8928
	[Header("State")]
	public bool snowMode;

	// Token: 0x040022E1 RID: 8929
	private static PrecipitationParticleSystemController _instance;
}
