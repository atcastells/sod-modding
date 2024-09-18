using System;
using UnityEngine;

// Token: 0x020003AF RID: 943
public class ElevatorKillBox : MonoBehaviour
{
	// Token: 0x06001560 RID: 5472 RVA: 0x001372A4 File Offset: 0x001354A4
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && this.elevator != null && this.elevator.isMoving && !this.elevator.isGoingUp && Player.Instance.currentHealth > 0f)
		{
			Player.Instance.AddHealth(-999999f, false, false);
		}
	}

	// Token: 0x04001A77 RID: 6775
	public Elevator elevator;
}
