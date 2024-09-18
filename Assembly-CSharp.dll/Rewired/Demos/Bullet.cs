using System;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008BF RID: 2239
	[AddComponentMenu("")]
	public class Bullet : MonoBehaviour
	{
		// Token: 0x06002FF8 RID: 12280 RVA: 0x00212D00 File Offset: 0x00210F00
		private void Start()
		{
			if (this.lifeTime > 0f)
			{
				this.deathTime = Time.time + this.lifeTime;
				this.die = true;
			}
		}

		// Token: 0x06002FF9 RID: 12281 RVA: 0x00212D28 File Offset: 0x00210F28
		private void Update()
		{
			if (this.die && Time.time >= this.deathTime)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04004AAE RID: 19118
		public float lifeTime = 3f;

		// Token: 0x04004AAF RID: 19119
		private bool die;

		// Token: 0x04004AB0 RID: 19120
		private float deathTime;
	}
}
