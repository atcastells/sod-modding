using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
public class WindowCameraController : MonoBehaviour
{
	// Token: 0x0600028C RID: 652 RVA: 0x00019790 File Offset: 0x00017990
	private void Awake()
	{
		this.cam = this.cameraObj.GetComponent<Camera>();
	}

	// Token: 0x0600028D RID: 653 RVA: 0x000197A3 File Offset: 0x000179A3
	private void Start()
	{
		this.camPos = this.cameraObj.transform.position;
	}

	// Token: 0x0600028E RID: 654 RVA: 0x000197BC File Offset: 0x000179BC
	private void Update()
	{
		if (this.isActive)
		{
			Vector3 position = this.followObj.transform.position;
			this.camPos.x = position.x;
			this.camPos.z = position.z;
			this.camPos.y = 2f;
			this.UpdateCamPos();
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0001981C File Offset: 0x00017A1C
	private void UpdateCamPos()
	{
		this.cameraObj.transform.position = new Vector3(Mathf.Clamp(this.camPos.x, CityData.Instance.boundaryLeft, CityData.Instance.boundaryRight), Mathf.Clamp(this.camPos.y, this.camHeightLimit.x, this.camHeightLimit.y), Mathf.Clamp(this.camPos.z, CityData.Instance.boundaryDown, CityData.Instance.boundaryUp));
		this.cam.orthographicSize = Mathf.Clamp(this.camPos.y, this.camHeightLimit.x, this.camHeightLimit.y);
	}

	// Token: 0x040001D0 RID: 464
	public GameObject cameraObj;

	// Token: 0x040001D1 RID: 465
	private Camera cam;

	// Token: 0x040001D2 RID: 466
	public GameObject followObj;

	// Token: 0x040001D3 RID: 467
	public Vector2 camHeightLimit = new Vector2(1f, 20f);

	// Token: 0x040001D4 RID: 468
	public Vector3 camPos = new Vector3(0f, 2f, 0f);

	// Token: 0x040001D5 RID: 469
	public bool isActive;
}
