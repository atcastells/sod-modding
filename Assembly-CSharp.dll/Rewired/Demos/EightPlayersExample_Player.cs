using System;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008B5 RID: 2229
	[AddComponentMenu("")]
	[RequireComponent(typeof(CharacterController))]
	public class EightPlayersExample_Player : MonoBehaviour
	{
		// Token: 0x06002FB5 RID: 12213 RVA: 0x002118DE File Offset: 0x0020FADE
		private void Awake()
		{
			this.cc = base.GetComponent<CharacterController>();
		}

		// Token: 0x06002FB6 RID: 12214 RVA: 0x002118EC File Offset: 0x0020FAEC
		private void Initialize()
		{
			this.player = ReInput.players.GetPlayer(this.playerId);
			this.initialized = true;
		}

		// Token: 0x06002FB7 RID: 12215 RVA: 0x0021190B File Offset: 0x0020FB0B
		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (!this.initialized)
			{
				this.Initialize();
			}
			this.GetInput();
			this.ProcessInput();
		}

		// Token: 0x06002FB8 RID: 12216 RVA: 0x00211930 File Offset: 0x0020FB30
		private void GetInput()
		{
			this.moveVector.x = this.player.GetAxis("Move Horizontal");
			this.moveVector.y = this.player.GetAxis("Move Vertical");
			this.fire = this.player.GetButtonDown("Fire");
		}

		// Token: 0x06002FB9 RID: 12217 RVA: 0x0021198C File Offset: 0x0020FB8C
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

		// Token: 0x04004A76 RID: 19062
		public int playerId;

		// Token: 0x04004A77 RID: 19063
		public float moveSpeed = 3f;

		// Token: 0x04004A78 RID: 19064
		public float bulletSpeed = 15f;

		// Token: 0x04004A79 RID: 19065
		public GameObject bulletPrefab;

		// Token: 0x04004A7A RID: 19066
		private Player player;

		// Token: 0x04004A7B RID: 19067
		private CharacterController cc;

		// Token: 0x04004A7C RID: 19068
		private Vector3 moveVector;

		// Token: 0x04004A7D RID: 19069
		private bool fire;

		// Token: 0x04004A7E RID: 19070
		[NonSerialized]
		private bool initialized;
	}
}
