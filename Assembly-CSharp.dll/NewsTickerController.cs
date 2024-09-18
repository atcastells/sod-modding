using System;
using UnityEngine;

// Token: 0x020004BD RID: 1213
public class NewsTickerController : MonoBehaviour
{
	// Token: 0x06001A45 RID: 6725 RVA: 0x001829B4 File Offset: 0x00180BB4
	private void Start()
	{
		if (this.meshRender == null)
		{
			this.meshRender = base.gameObject.GetComponent<MeshRenderer>();
		}
		if (this.meshRender != null)
		{
			this.meshRender.sharedMaterial = TextToImageController.Instance.newsTickerMaterial;
			Object.Destroy(this);
		}
	}

	// Token: 0x040022D6 RID: 8918
	public MeshRenderer meshRender;
}
