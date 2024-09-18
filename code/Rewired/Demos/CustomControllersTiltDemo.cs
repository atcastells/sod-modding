using System;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008AD RID: 2221
	[AddComponentMenu("")]
	public class CustomControllersTiltDemo : MonoBehaviour
	{
		// Token: 0x06002F7F RID: 12159 RVA: 0x002109B4 File Offset: 0x0020EBB4
		private void Awake()
		{
			Screen.orientation = 3;
			this.player = ReInput.players.GetPlayer(0);
			ReInput.InputSourceUpdateEvent += new Action(this.OnInputUpdate);
			this.controller = (CustomController)this.player.controllers.GetControllerWithTag(20, "TiltController");
		}

		// Token: 0x06002F80 RID: 12160 RVA: 0x00210A0C File Offset: 0x0020EC0C
		private void Update()
		{
			if (this.target == null)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			vector.y = this.player.GetAxis("Tilt Vertical");
			vector.x = this.player.GetAxis("Tilt Horizontal");
			if (vector.sqrMagnitude > 1f)
			{
				vector.Normalize();
			}
			vector *= Time.deltaTime;
			this.target.Translate(vector * this.speed);
		}

		// Token: 0x06002F81 RID: 12161 RVA: 0x00210A94 File Offset: 0x0020EC94
		private void OnInputUpdate()
		{
			Vector3 acceleration = Input.acceleration;
			this.controller.SetAxisValue(0, acceleration.x);
			this.controller.SetAxisValue(1, acceleration.y);
			this.controller.SetAxisValue(2, acceleration.z);
		}

		// Token: 0x04004A4A RID: 19018
		public Transform target;

		// Token: 0x04004A4B RID: 19019
		public float speed = 10f;

		// Token: 0x04004A4C RID: 19020
		private CustomController controller;

		// Token: 0x04004A4D RID: 19021
		private Player player;
	}
}
