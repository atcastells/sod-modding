using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class RagdollSFXController : MonoBehaviour
{
	// Token: 0x06000EA2 RID: 3746 RVA: 0x000D34A4 File Offset: 0x000D16A4
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.playedFloorImpact)
		{
			this.playedFloorImpact = true;
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.collapseOnFloor, this.actor, this.actor.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
			Object.Destroy(this);
		}
	}

	// Token: 0x040011B6 RID: 4534
	public Actor actor;

	// Token: 0x040011B7 RID: 4535
	public bool playedFloorImpact;
}
