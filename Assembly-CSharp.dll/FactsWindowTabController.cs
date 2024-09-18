using System;
using UnityEngine;

// Token: 0x0200056B RID: 1387
public class FactsWindowTabController : MonoBehaviour
{
	// Token: 0x06001E39 RID: 7737 RVA: 0x001A7634 File Offset: 0x001A5834
	public void Setup(InfoWindow newWindow)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.parentRect = (base.gameObject.transform.parent as RectTransform);
		this.nativeSize = this.rect.sizeDelta;
		this.parentWindow = newWindow;
		this.evidence = this.parentWindow.passedEvidence;
		this.parentWindow.OnResizedWindow += this.OnWindowResize;
		this.rect.sizeDelta = new Vector2(this.rect.sizeDelta.x, 42f);
	}

	// Token: 0x06001E3A RID: 7738 RVA: 0x001A76D2 File Offset: 0x001A58D2
	public void UpdateSlotContent()
	{
		this.parentWindow.item.PositionSpawnedFacts(10f, 6f);
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x001A76F0 File Offset: 0x001A58F0
	public void OnWindowResize()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		Vector2 vector = new Vector2(this.parentRect.rect.width, this.parentRect.rect.height);
		float num = vector.x / (this.nativeSize.x + this.parentWindow.centringTollerance * 0.5f);
		float num2 = vector.y / (this.nativeSize.y + this.parentWindow.centringTollerance * 0.5f);
		this.fitScale = (float)Mathf.RoundToInt(Mathf.Min(Mathf.Min(num, num2), 1f) * 100f) / 100f;
		this.rect.localScale = new Vector3(this.fitScale, this.fitScale, 1f);
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, -4f);
		if (this.scrollRectRect == null)
		{
			this.scrollRectRect = this.parentWindow.scrollRect.GetComponent<RectTransform>();
		}
		this.scrollRectRect.offsetMax = new Vector2(this.scrollRectRect.offsetMax.x, -this.rect.sizeDelta.y * this.fitScale + this.rect.anchoredPosition.y * 2f);
		if (this.contentController == null)
		{
			this.contentController = this.parentWindow.item.factContent;
		}
		this.contentController.rect.localScale = new Vector3(this.fitScale, this.fitScale, 1f);
	}

	// Token: 0x04002815 RID: 10261
	public InfoWindow parentWindow;

	// Token: 0x04002816 RID: 10262
	public Evidence evidence;

	// Token: 0x04002817 RID: 10263
	public RectTransform rect;

	// Token: 0x04002818 RID: 10264
	public RectTransform scrollRectRect;

	// Token: 0x04002819 RID: 10265
	public RectTransform parentRect;

	// Token: 0x0400281A RID: 10266
	public WindowContentController contentController;

	// Token: 0x0400281B RID: 10267
	public Vector2 nativeSize = new Vector2(342f, 152f);

	// Token: 0x0400281C RID: 10268
	public float fitScale = 1f;
}
