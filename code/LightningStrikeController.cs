using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200032B RID: 811
public class LightningStrikeController : MonoBehaviour
{
	// Token: 0x06001243 RID: 4675 RVA: 0x00103CE6 File Offset: 0x00101EE6
	private void Start()
	{
		this.timer = Toolbox.Instance.Rand(this.aliveTime.x, this.aliveTime.y, false);
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00103D0F File Offset: 0x00101F0F
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x04001674 RID: 5748
	public Vector2 aliveTime = Vector2.one;

	// Token: 0x04001675 RID: 5749
	[ReadOnly]
	public float timer;

	// Token: 0x04001676 RID: 5750
	public Transform startPoint;

	// Token: 0x04001677 RID: 5751
	public Transform endPoint;
}
