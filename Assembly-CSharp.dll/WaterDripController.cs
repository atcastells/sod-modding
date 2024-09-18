using System;
using UnityEngine;

// Token: 0x020004C9 RID: 1225
public class WaterDripController : MonoBehaviour
{
	// Token: 0x06001A71 RID: 6769 RVA: 0x0018595C File Offset: 0x00183B5C
	private void Start()
	{
		if (this.meshRenderer == null)
		{
			this.meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
		}
		if (this.meshRenderer != null)
		{
			this.meshRenderer.sharedMaterial = SessionData.Instance.GetWeatherAffectedMaterial(this.meshRenderer.sharedMaterial, this.meshRenderer);
		}
		Object.Destroy(this);
	}

	// Token: 0x0400231E RID: 8990
	public MeshRenderer meshRenderer;
}
