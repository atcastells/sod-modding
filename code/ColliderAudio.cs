using System;
using UnityEngine;

// Token: 0x020003A6 RID: 934
public class ColliderAudio : MonoBehaviour
{
	// Token: 0x06001536 RID: 5430 RVA: 0x001349C4 File Offset: 0x00132BC4
	private void OnTriggerEnter(Collider coll)
	{
		string text = "Collider Audio: ";
		AudioEvent audioEvent = this.playSound;
		Game.Log(text + ((audioEvent != null) ? audioEvent.ToString() : null), 2);
		AudioController.Instance.PlayWorldOneShot(this.playSound, null, null, coll.transform.position, null, null, 1f, null, false, null, false);
	}

	// Token: 0x04001A2E RID: 6702
	public AudioEvent playSound;
}
