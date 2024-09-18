using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class AnimatedTextureController : MonoBehaviour
{
	// Token: 0x06000C63 RID: 3171 RVA: 0x000B134C File Offset: 0x000AF54C
	[Button(null, 0)]
	public virtual void Play()
	{
		if (!this.isPlaying)
		{
			if (this.triggerAudio != null)
			{
				if (this.useSpeedOfSound)
				{
					float delay = Vector3.Distance(CameraController.Instance.transform.position, base.transform.position) / AudioController.Instance.speedOfSound;
					AudioController.Instance.PlayOneShotDelayed(delay, this.triggerAudio, null, null, base.transform.position, null, 1f, null, false);
				}
				else
				{
					AudioController.Instance.PlayWorldOneShot(this.triggerAudio, null, null, base.transform.position, null, null, 1f, null, false, null, false);
				}
			}
			this.nextFrameTimer = 0f;
			this.spriteCursorX = -1;
			this.spriteCursorY = (int)this.texTileCount.y;
			this.animtionTimer = 0f;
			this.animationProgress = 0f;
			if (this.billboardingOn && this.faceOnStartOnly)
			{
				this.Billboard();
			}
			this.isPlaying = true;
			base.enabled = true;
		}
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x000B1457 File Offset: 0x000AF657
	[Button(null, 0)]
	public virtual void Stop()
	{
		if (this.isPlaying)
		{
			this.isPlaying = false;
			base.enabled = false;
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x000B146F File Offset: 0x000AF66F
	private void OnDisable()
	{
		if (this.destroyIfInactive)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x000B1484 File Offset: 0x000AF684
	protected virtual void Awake()
	{
		if (this.animatedRenderer == null)
		{
			this.animatedRenderer = base.gameObject.GetComponent<Renderer>();
			if (this.animatedRenderer == null)
			{
				this.animatedRenderer = base.gameObject.GetComponentInChildren<Renderer>();
			}
		}
		this.ApplyScale(new Vector2(1f / this.texTileCount.x, 1f / this.texTileCount.y));
		this.isPlaying = false;
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x000B1503 File Offset: 0x000AF703
	protected virtual void Start()
	{
		if (this.playOnStart)
		{
			this.Play();
		}
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x000B1514 File Offset: 0x000AF714
	private void Update()
	{
		if (this.isPlaying)
		{
			this.animtionTimer += Time.deltaTime;
			this.animationProgress = Mathf.Clamp01(this.animtionTimer / this.animationCycleTime);
			if (this.nextFrameTimer > 0f)
			{
				this.nextFrameTimer -= Time.deltaTime;
			}
			if (this.nextFrameTimer <= 0f)
			{
				this.spriteCursorX++;
				if ((float)this.spriteCursorX >= this.texTileCount.x)
				{
					this.spriteCursorY--;
					this.spriteCursorX = 0;
					if (this.spriteCursorY <= -1)
					{
						if (this.destroyOnEnd)
						{
							Object.Destroy(base.gameObject);
						}
						if (this.loop)
						{
							this.spriteCursorX = -1;
							this.spriteCursorY = (int)this.texTileCount.y;
							this.nextFrameTimer = 0f;
							if (this.billboardingOn && this.faceOnStartOnly)
							{
								this.Billboard();
							}
							this.animtionTimer = 0f;
							this.animationProgress = 0f;
						}
						else
						{
							this.Stop();
						}
					}
				}
				if (this.spriteCursorX >= 0 && this.spriteCursorY >= 0)
				{
					this.ApplyOffset(new Vector2(1f / this.texTileCount.x * (float)this.spriteCursorX, 1f / this.texTileCount.y * (float)this.spriteCursorY));
				}
				this.nextFrameTimer += this.animationCycleTime / (this.texTileCount.x * this.texTileCount.y);
			}
			if (this.alterEmission)
			{
				Color color = this.startingEmission;
				if (this.animationProgress <= 0.5f)
				{
					color = Color.Lerp(this.startingEmission, this.midEmission, this.animationProgress * 2f);
				}
				else
				{
					color = Color.Lerp(this.midEmission, this.endEmission, (this.animationProgress - 0.5f) * 2f);
				}
				if (this.animatedRenderer.material.HasProperty("_EmissiveColor"))
				{
					this.animatedRenderer.material.SetColor("_EmissiveColor", color);
				}
			}
			if (this.alterScale)
			{
				this.parentScaleTransform.localScale = new Vector3(this.scaleX.Evaluate(this.animationProgress), this.scaleY.Evaluate(this.animationProgress), this.scaleZ.Evaluate(this.animationProgress));
			}
		}
		if (this.billboardingOn && !this.faceOnStartOnly)
		{
			this.Billboard();
		}
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x000B17A8 File Offset: 0x000AF9A8
	private void Billboard()
	{
		base.transform.rotation = CameraController.Instance.cam.transform.rotation;
		if (this.specialCase == AnimatedTextureController.SpecialCase.fireSmoke)
		{
			Vector3 vector;
			vector..ctor(0f, base.transform.localEulerAngles.y, 0f);
			base.transform.rotation = Quaternion.Slerp(Quaternion.identity, CameraController.Instance.cam.transform.rotation, 0.4f);
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, vector.y, 0f);
		}
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x000B1856 File Offset: 0x000AFA56
	protected virtual void ApplyOffset(Vector2 offset)
	{
		if (this.animatedRenderer != null && this.animatedRenderer.material.HasProperty("_BaseColorMap"))
		{
			this.animatedRenderer.material.SetTextureOffset("_BaseColorMap", offset);
		}
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x000B1893 File Offset: 0x000AFA93
	protected virtual void ApplyScale(Vector2 scale)
	{
		if (this.animatedRenderer != null && this.animatedRenderer.material.HasProperty("_BaseColorMap"))
		{
			this.animatedRenderer.material.SetTextureScale("_BaseColorMap", scale);
		}
	}

	// Token: 0x04000DEF RID: 3567
	[Header("Components")]
	public Renderer animatedRenderer;

	// Token: 0x04000DF0 RID: 3568
	[Header("Settings")]
	[Tooltip("How long it takes the animation to complete")]
	public float animationCycleTime;

	// Token: 0x04000DF1 RID: 3569
	[Tooltip("Count of X/Y frames within the base texture.")]
	public Vector2 texTileCount;

	// Token: 0x04000DF2 RID: 3570
	[Tooltip("Play this when active")]
	public bool playOnStart;

	// Token: 0x04000DF3 RID: 3571
	[Tooltip("Destroy self on end")]
	public bool destroyOnEnd = true;

	// Token: 0x04000DF4 RID: 3572
	[Tooltip("Destroy if inactive")]
	public bool destroyIfInactive = true;

	// Token: 0x04000DF5 RID: 3573
	[Tooltip("Always face the camera")]
	[Header("Billboarding")]
	public bool billboardingOn;

	// Token: 0x04000DF6 RID: 3574
	[EnableIf("billboardingOn")]
	[Tooltip("Only face towards camera when the animation starts playing...")]
	public bool faceOnStartOnly;

	// Token: 0x04000DF7 RID: 3575
	[EnableIf("billboardingOn")]
	public AnimatedTextureController.SpecialCase specialCase;

	// Token: 0x04000DF8 RID: 3576
	[Header("Extra VFX")]
	public bool alterEmission = true;

	// Token: 0x04000DF9 RID: 3577
	[ColorUsage(true, true)]
	public Color startingEmission;

	// Token: 0x04000DFA RID: 3578
	[ColorUsage(true, true)]
	public Color midEmission;

	// Token: 0x04000DFB RID: 3579
	[ColorUsage(true, true)]
	public Color endEmission;

	// Token: 0x04000DFC RID: 3580
	[Space(5f)]
	public bool alterScale = true;

	// Token: 0x04000DFD RID: 3581
	public Transform parentScaleTransform;

	// Token: 0x04000DFE RID: 3582
	public AnimationCurve scaleX;

	// Token: 0x04000DFF RID: 3583
	public AnimationCurve scaleY;

	// Token: 0x04000E00 RID: 3584
	public AnimationCurve scaleZ;

	// Token: 0x04000E01 RID: 3585
	[Header("Audio")]
	public AudioEvent triggerAudio;

	// Token: 0x04000E02 RID: 3586
	public bool useSpeedOfSound = true;

	// Token: 0x04000E03 RID: 3587
	[Header("State")]
	private float animtionTimer;

	// Token: 0x04000E04 RID: 3588
	public float animationProgress;

	// Token: 0x04000E05 RID: 3589
	public bool isPlaying;

	// Token: 0x04000E06 RID: 3590
	public bool loop;

	// Token: 0x04000E07 RID: 3591
	public float nextFrameTimer;

	// Token: 0x04000E08 RID: 3592
	public int spriteCursorX = -1;

	// Token: 0x04000E09 RID: 3593
	public int spriteCursorY = -1;

	// Token: 0x02000225 RID: 549
	public enum SpecialCase
	{
		// Token: 0x04000E0B RID: 3595
		fireSmoke
	}
}
