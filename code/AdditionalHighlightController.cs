using System;
using UnityEngine;

// Token: 0x02000532 RID: 1330
public class AdditionalHighlightController : MonoBehaviour
{
	// Token: 0x06001D11 RID: 7441 RVA: 0x0019DBF0 File Offset: 0x0019BDF0
	private void Update()
	{
		this.val += this.speed * Time.deltaTime;
		float num = this.curve.Evaluate(this.val);
		this.rect.localScale = new Vector3(num, num, num);
	}

	// Token: 0x040026D5 RID: 9941
	public RectTransform rect;

	// Token: 0x040026D6 RID: 9942
	public AnimationCurve curve;

	// Token: 0x040026D7 RID: 9943
	public float speed = 1f;

	// Token: 0x040026D8 RID: 9944
	public float val;
}
