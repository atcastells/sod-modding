using System;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class MuzzleFlashController : MonoBehaviour
{
	// Token: 0x060014F2 RID: 5362 RVA: 0x00132A3C File Offset: 0x00130C3C
	private void Update()
	{
		this.timer += Time.deltaTime;
		this.progress = Mathf.Clamp01(this.timer / this.duration);
		if (this.light != null)
		{
			float num = this.curve.Evaluate(this.progress);
			this.light.intensity = num * this.maxIntensity;
			this.light.range = num * this.maxRange;
			this.light.color = Color.Lerp(this.startColour, this.endColour, this.progress);
			this.light.enabled = true;
		}
		if (this.progress >= 1f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040019BA RID: 6586
	[Header("Settings")]
	public Color startColour = Color.white;

	// Token: 0x040019BB RID: 6587
	public Color endColour = Color.yellow;

	// Token: 0x040019BC RID: 6588
	public float maxIntensity = 100f;

	// Token: 0x040019BD RID: 6589
	public float maxRange = 10f;

	// Token: 0x040019BE RID: 6590
	public float duration = 1f;

	// Token: 0x040019BF RID: 6591
	public AnimationCurve curve;

	// Token: 0x040019C0 RID: 6592
	[Header("Components")]
	public Light light;

	// Token: 0x040019C1 RID: 6593
	[Header("State")]
	public float timer;

	// Token: 0x040019C2 RID: 6594
	public float progress;
}
