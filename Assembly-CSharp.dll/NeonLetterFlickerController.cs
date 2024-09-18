using System;
using UnityEngine;

// Token: 0x02000398 RID: 920
public class NeonLetterFlickerController : MonoBehaviour
{
	// Token: 0x060014F4 RID: 5364 RVA: 0x00132B40 File Offset: 0x00130D40
	private void OnEnable()
	{
		if (this.closestStreetNode == null)
		{
			this.closestStreetNode = Toolbox.Instance.FindClosestValidNodeToWorldPosition(base.transform.TransformPoint(this.soundOffset), false, true, true, new Vector3Int(0, 0, -1), false, 0, false, 200);
			this.nodeWorldPos = this.closestStreetNode.position;
		}
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x00132B9C File Offset: 0x00130D9C
	private void Update()
	{
		if (this.closestStreetNode == null)
		{
			return;
		}
		if (this.neonMat.flickerInterval && !this.state)
		{
			if (this.loop != null)
			{
				AudioController.Instance.StopSound(this.loop, AudioController.StopType.triggerCue, "Neon switch off");
			}
			this.state = true;
			this.loop = AudioController.Instance.PlayWorldLoopingStatic(this.neonMat.flickerAudio, null, this.closestStreetNode, base.transform.TransformPoint(this.soundOffset), null, 1f, false, false, null, null);
			return;
		}
		if (this.neonMat.flickerInterval && this.state)
		{
			this.loop.audioEvent.setParameterByName("Brightness", this.neonMat.brightness, false);
			return;
		}
		if (!this.neonMat.flickerInterval && this.state)
		{
			if (this.loop != null)
			{
				AudioController.Instance.StopSound(this.loop, AudioController.StopType.triggerCue, "Neon switch off");
			}
			this.loop = null;
			this.state = false;
		}
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x00132CA3 File Offset: 0x00130EA3
	private void OnDisable()
	{
		if (this.loop != null)
		{
			AudioController.Instance.StopSound(this.loop, AudioController.StopType.fade, "Neon disabled");
		}
		this.loop = null;
		this.state = false;
	}

	// Token: 0x040019C3 RID: 6595
	public CityControls.NeonMaterial neonMat;

	// Token: 0x040019C4 RID: 6596
	public bool state;

	// Token: 0x040019C5 RID: 6597
	public AudioController.LoopingSoundInfo loop;

	// Token: 0x040019C6 RID: 6598
	public Vector3 soundOffset = new Vector3(0f, 0f, 0.2f);

	// Token: 0x040019C7 RID: 6599
	public NewNode closestStreetNode;

	// Token: 0x040019C8 RID: 6600
	public Vector3 nodeWorldPos;
}
