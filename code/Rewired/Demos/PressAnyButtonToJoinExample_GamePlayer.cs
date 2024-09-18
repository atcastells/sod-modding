using System;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008BB RID: 2235
	[AddComponentMenu("")]
	[RequireComponent(typeof(CharacterController))]
	public class PressAnyButtonToJoinExample_GamePlayer : MonoBehaviour
	{
		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06002FE5 RID: 12261 RVA: 0x00212865 File Offset: 0x00210A65
		private Player player
		{
			get
			{
				if (!ReInput.isReady)
				{
					return null;
				}
				return ReInput.players.GetPlayer(this.playerId);
			}
		}

		// Token: 0x06002FE6 RID: 12262 RVA: 0x00212880 File Offset: 0x00210A80
		private void OnEnable()
		{
			this.cc = base.GetComponent<CharacterController>();
		}

		// Token: 0x06002FE7 RID: 12263 RVA: 0x0021288E File Offset: 0x00210A8E
		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (this.player == null)
			{
				return;
			}
			this.GetInput();
			this.ProcessInput();
		}

		// Token: 0x06002FE8 RID: 12264 RVA: 0x002128B0 File Offset: 0x00210AB0
		private void GetInput()
		{
			this.moveVector.x = this.player.GetAxis("Move Horizontal");
			this.moveVector.y = this.player.GetAxis("Move Vertical");
			this.fire = this.player.GetButtonDown("Fire");
		}

		// Token: 0x06002FE9 RID: 12265 RVA: 0x0021290C File Offset: 0x00210B0C
		private void ProcessInput()
		{
			if (this.moveVector.x != 0f || this.moveVector.y != 0f)
			{
				this.cc.Move(this.moveVector * this.moveSpeed * Time.deltaTime);
			}
			if (this.fire)
			{
				Object.Instantiate<GameObject>(this.bulletPrefab, base.transform.position + base.transform.right, base.transform.rotation).GetComponent<Rigidbody>().AddForce(base.transform.right * this.bulletSpeed, 2);
			}
		}

		// Token: 0x04004A9A RID: 19098
		public int playerId;

		// Token: 0x04004A9B RID: 19099
		public float moveSpeed = 3f;

		// Token: 0x04004A9C RID: 19100
		public float bulletSpeed = 15f;

		// Token: 0x04004A9D RID: 19101
		public GameObject bulletPrefab;

		// Token: 0x04004A9E RID: 19102
		private CharacterController cc;

		// Token: 0x04004A9F RID: 19103
		private Vector3 moveVector;

		// Token: 0x04004AA0 RID: 19104
		private bool fire;
	}
}
