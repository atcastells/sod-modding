using System;
using UnityEngine;

// Token: 0x020003A0 RID: 928
public class ActiveDoorWedgeController : MonoBehaviour
{
	// Token: 0x06001521 RID: 5409 RVA: 0x0013423C File Offset: 0x0013243C
	private void Update()
	{
		if (this.setupProgress < 1f)
		{
			this.setupProgress += Time.deltaTime / 0.4f;
			this.setupProgress = Mathf.Min(this.setupProgress, 1f);
			this.wedge1.localEulerAngles = new Vector3(Mathf.Lerp(0f, -45f, this.setupProgress), 0f, 0f);
			this.wedge2.localEulerAngles = new Vector3(Mathf.Lerp(0f, 45f, this.setupProgress), 0f, 0f);
		}
	}

	// Token: 0x04001A13 RID: 6675
	public InteractableController controller;

	// Token: 0x04001A14 RID: 6676
	public Transform wedge1;

	// Token: 0x04001A15 RID: 6677
	public Transform wedge2;

	// Token: 0x04001A16 RID: 6678
	public float setupProgress;
}
