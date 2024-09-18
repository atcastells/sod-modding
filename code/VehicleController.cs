using System;
using UnityEngine;

// Token: 0x02000434 RID: 1076
public class VehicleController : MonoBehaviour
{
	// Token: 0x06001834 RID: 6196 RVA: 0x00169D98 File Offset: 0x00167F98
	private void OnTriggerEnter(Collider other)
	{
		if (Player.Instance.currentVehicle == null && !Player.Instance.playerKOInProgress && other.tag == "Player")
		{
			Player.Instance.SetVehicle(this.vehicle);
		}
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x00169DE5 File Offset: 0x00167FE5
	private void OnTriggerExit(Collider other)
	{
		if (Player.Instance.currentVehicle != null && !Player.Instance.playerKOInProgress && other.tag == "Player")
		{
			Player.Instance.SetVehicle(null);
		}
	}

	// Token: 0x04001E26 RID: 7718
	public Transform vehicle;
}
