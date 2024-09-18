using System;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008AF RID: 2223
	[RequireComponent(typeof(CharacterController))]
	[AddComponentMenu("")]
	public class CustomControllerDemo_Player : MonoBehaviour
	{
		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06002F8E RID: 12174 RVA: 0x00210DC2 File Offset: 0x0020EFC2
		private Player player
		{
			get
			{
				if (this._player == null)
				{
					this._player = ReInput.players.GetPlayer(this.playerId);
				}
				return this._player;
			}
		}

		// Token: 0x06002F8F RID: 12175 RVA: 0x00210DE8 File Offset: 0x0020EFE8
		private void Awake()
		{
			this.cc = base.GetComponent<CharacterController>();
		}

		// Token: 0x06002F90 RID: 12176 RVA: 0x00210DF8 File Offset: 0x0020EFF8
		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			Vector2 vector;
			vector..ctor(this.player.GetAxis("Move Horizontal"), this.player.GetAxis("Move Vertical"));
			this.cc.Move(vector * this.speed * Time.deltaTime);
			if (this.player.GetButtonDown("Fire"))
			{
				Vector3 vector2 = Vector3.Scale(new Vector3(1f, 0f, 0f), base.transform.right);
				Object.Instantiate<GameObject>(this.bulletPrefab, base.transform.position + vector2, Quaternion.identity).GetComponent<Rigidbody>().velocity = new Vector3(this.bulletSpeed * base.transform.right.x, 0f, 0f);
			}
			if (this.player.GetButtonDown("Change Color"))
			{
				Renderer component = base.GetComponent<Renderer>();
				Material material = component.material;
				material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
				component.material = material;
			}
		}

		// Token: 0x04004A59 RID: 19033
		public int playerId;

		// Token: 0x04004A5A RID: 19034
		public float speed = 1f;

		// Token: 0x04004A5B RID: 19035
		public float bulletSpeed = 20f;

		// Token: 0x04004A5C RID: 19036
		public GameObject bulletPrefab;

		// Token: 0x04004A5D RID: 19037
		private Player _player;

		// Token: 0x04004A5E RID: 19038
		private CharacterController cc;
	}
}
