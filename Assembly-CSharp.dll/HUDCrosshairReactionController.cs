using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200052E RID: 1326
public class HUDCrosshairReactionController : MonoBehaviour
{
	// Token: 0x06001D01 RID: 7425 RVA: 0x0019C5EF File Offset: 0x0019A7EF
	private void Awake()
	{
		this.rend.SetAlpha(0f);
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x0019C604 File Offset: 0x0019A804
	private void Update()
	{
		this.progress += Time.deltaTime * 2f;
		if (this.progress >= 1f)
		{
			Object.Destroy(base.gameObject);
		}
		this.img.color = InterfaceControls.Instance.lightOrbFillImg.color;
		float num = this.curve.Evaluate(this.progress);
		float num2 = Mathf.LerpUnclamped(0f, this.maxSize, num);
		this.rect.sizeDelta = new Vector2(num2, num2);
		this.rend.SetAlpha(1f - this.progress);
	}

	// Token: 0x0400269A RID: 9882
	public RectTransform rect;

	// Token: 0x0400269B RID: 9883
	public Image img;

	// Token: 0x0400269C RID: 9884
	public float progress;

	// Token: 0x0400269D RID: 9885
	public AnimationCurve curve;

	// Token: 0x0400269E RID: 9886
	public float maxSize = 200f;

	// Token: 0x0400269F RID: 9887
	public CanvasRenderer rend;
}
