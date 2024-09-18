using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000399 RID: 921
public class NeonSignController : MonoBehaviour
{
	// Token: 0x060014F8 RID: 5368 RVA: 0x00132CF4 File Offset: 0x00130EF4
	private void OnEnable()
	{
		if (Toolbox.Instance != null)
		{
			this.frameCounter = Toolbox.Instance.Rand(0f, (float)this.frameDelay, false);
		}
		if (this.audioLoop != null && this.loop == null)
		{
			if (this.closestStreetNode == null)
			{
				this.closestStreetNode = Toolbox.Instance.FindClosestValidNodeToWorldPosition(base.transform.TransformPoint(this.localSoundOffset), false, true, true, new Vector3Int(0, 0, -1), false, 0, false, 200);
			}
			this.loop = AudioController.Instance.PlayWorldLoopingStatic(this.audioLoop, null, this.closestStreetNode, base.transform.TransformPoint(this.localSoundOffset), null, 1f, false, true, null, null);
		}
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x00132DB5 File Offset: 0x00130FB5
	private void OnDisable()
	{
		if (this.loop != null)
		{
			AudioController.Instance.StopSound(this.loop, AudioController.StopType.fade, "Neon disabled");
			this.loop = null;
		}
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x00132DDC File Offset: 0x00130FDC
	private void Update()
	{
		if (SessionData.Instance.play && this.materialAnimations.Count > 0)
		{
			this.frameCounter += Time.deltaTime;
			if (this.frameCounter >= (float)this.frameDelay)
			{
				this.frameCounter -= (float)this.frameDelay;
				this.frameCursor++;
				if (this.frameCursor >= this.materialAnimations.Count)
				{
					this.frameCursor = 0;
				}
				foreach (MeshRenderer meshRenderer in this.meshRenderers)
				{
					if (!(meshRenderer == null))
					{
						try
						{
							meshRenderer.sharedMaterial = this.materialAnimations[this.frameCursor];
						}
						catch
						{
						}
					}
				}
				try
				{
					if (this.lightBools[this.frameCursor])
					{
						if (this.lightComponent != null)
						{
							this.lightComponent.enabled = true;
						}
						if (this.loop != null)
						{
							this.loop.audioEvent.setPaused(false);
						}
					}
					else
					{
						if (this.lightComponent != null)
						{
							this.lightComponent.enabled = false;
						}
						if (this.loop != null)
						{
							this.loop.audioEvent.setPaused(true);
						}
					}
				}
				catch
				{
				}
			}
		}
	}

	// Token: 0x040019C9 RID: 6601
	public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

	// Token: 0x040019CA RID: 6602
	public Light lightComponent;

	// Token: 0x040019CB RID: 6603
	[ReorderableList]
	public List<Material> materialAnimations = new List<Material>();

	// Token: 0x040019CC RID: 6604
	[ReorderableList]
	public List<bool> lightBools = new List<bool>();

	// Token: 0x040019CD RID: 6605
	public int frameCursor;

	// Token: 0x040019CE RID: 6606
	public int frameDelay = 1;

	// Token: 0x040019CF RID: 6607
	private float frameCounter;

	// Token: 0x040019D0 RID: 6608
	[Header("Materials")]
	public bool useAddressColours = true;

	// Token: 0x040019D1 RID: 6609
	[EnableIf("useAddressColours")]
	public bool changeBaseColour = true;

	// Token: 0x040019D2 RID: 6610
	[EnableIf("useAddressColours")]
	public bool changeAltColour1;

	// Token: 0x040019D3 RID: 6611
	[EnableIf("useAddressColours")]
	public bool changeAltColour2;

	// Token: 0x040019D4 RID: 6612
	[EnableIf("useAddressColours")]
	public bool changeAltColour3;

	// Token: 0x040019D5 RID: 6613
	[Header("Audio")]
	public AudioEvent audioLoop;

	// Token: 0x040019D6 RID: 6614
	public Vector3 localSoundOffset;

	// Token: 0x040019D7 RID: 6615
	private AudioController.LoopingSoundInfo loop;

	// Token: 0x040019D8 RID: 6616
	private NewNode closestStreetNode;
}
