using System;
using UnityEngine;

// Token: 0x020004C8 RID: 1224
public class ShardController : MonoBehaviour
{
	// Token: 0x06001A6C RID: 6764 RVA: 0x001858C6 File Offset: 0x00183AC6
	private void Awake()
	{
		ShardController.shardCounter++;
		if (ShardController.shardCounter > 150)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001A6D RID: 6765 RVA: 0x001858EC File Offset: 0x00183AEC
	private void Update()
	{
		this.timer += Time.deltaTime * Toolbox.Instance.Rand(0.2f, 1f, false);
		if (this.timer >= this.baseTime)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001A6E RID: 6766 RVA: 0x000516BA File Offset: 0x0004F8BA
	private void OnDisable()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001A6F RID: 6767 RVA: 0x0018593A File Offset: 0x00183B3A
	private void OnDestroy()
	{
		ShardController.shardCounter--;
	}

	// Token: 0x0400231B RID: 8987
	[Header("Setup")]
	public float baseTime = 3f;

	// Token: 0x0400231C RID: 8988
	[Header("State")]
	public float timer;

	// Token: 0x0400231D RID: 8989
	public static int shardCounter;
}
