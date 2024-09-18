using System;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008BE RID: 2238
	[RequireComponent(typeof(CharacterController))]
	[AddComponentMenu("")]
	public class PressStartToJoinExample_GamePlayer : MonoBehaviour
	{
		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06002FF2 RID: 12274 RVA: 0x00212B97 File Offset: 0x00210D97
		private Player player
		{
			get
			{
				return PressStartToJoinExample_Assigner.GetRewiredPlayer(this.gamePlayerId);
			}
		}

		// Token: 0x06002FF3 RID: 12275 RVA: 0x00212BA4 File Offset: 0x00210DA4
		private void OnEnable()
		{
			this.cc = base.GetComponent<CharacterController>();
		}

		// Token: 0x06002FF4 RID: 12276 RVA: 0x00212BB2 File Offset: 0x00210DB2
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

		// Token: 0x06002FF5 RID: 12277 RVA: 0x00212BD4 File Offset: 0x00210DD4
		private void GetInput()
		{
			this.moveVector.x = this.player.GetAxis("Move Horizontal");
			this.moveVector.y = this.player.GetAxis("Move Vertical");
			this.fire = this.player.GetButtonDown("Fire");
		}

		// Token: 0x06002FF6 RID: 12278 RVA: 0x00212C30 File Offset: 0x00210E30
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

		// Token: 0x04004AA7 RID: 19111
		public int gamePlayerId;

		// Token: 0x04004AA8 RID: 19112
		public float moveSpeed = 3f;

		// Token: 0x04004AA9 RID: 19113
		public float bulletSpeed = 15f;

		// Token: 0x04004AAA RID: 19114
		public GameObject bulletPrefab;

		// Token: 0x04004AAB RID: 19115
		private CharacterController cc;

		// Token: 0x04004AAC RID: 19116
		private Vector3 moveVector;

		// Token: 0x04004AAD RID: 19117
		private bool fire;
	}
}
