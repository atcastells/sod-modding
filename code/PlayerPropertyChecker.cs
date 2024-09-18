using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class PlayerPropertyChecker : MonoBehaviour
{
	// Token: 0x060001B7 RID: 439 RVA: 0x0000E31A File Offset: 0x0000C51A
	private void Start()
	{
		this.room = base.GetComponentInParent<NewRoom>();
		this.player = Player.Instance;
		this.player.apartmentsOwned.Contains(this.room.gameLocation.thisAsAddress);
	}

	// Token: 0x04000108 RID: 264
	public NewRoom room;

	// Token: 0x04000109 RID: 265
	public Player player;
}
