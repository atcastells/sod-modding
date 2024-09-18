using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005DC RID: 1500
public class StringLink : MonoBehaviour
{
	// Token: 0x060020FE RID: 8446 RVA: 0x001C38A8 File Offset: 0x001C1AA8
	public void VisualUpdate()
	{
		Vector2 vector;
		vector..ctor(this.rectTo.sizeDelta.x * 0.5f, 0f);
		Vector2 vector2;
		vector2..ctor(this.rectFrom.sizeDelta.x * 0.5f, 0f);
		Vector2 vector3 = new Vector2(this.rectTo.localPosition.x + vector.x, this.rectTo.localPosition.y + vector.y) - new Vector2(this.rectFrom.localPosition.x + vector2.x, this.rectFrom.localPosition.y + vector2.y);
		float num = 5f;
		this.rect.sizeDelta = new Vector2(vector3.magnitude * this.scaleReference.localScale.x, num);
		float num2 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
		this.rect.rotation = Quaternion.Euler(0f, 0f, num2);
		this.rect.position = new Vector3(this.rectFrom.position.x + vector2.x, this.rectFrom.position.y + vector2.y, 0f);
	}

	// Token: 0x04002B2B RID: 11051
	public Image img;

	// Token: 0x04002B2C RID: 11052
	private RectTransform rect;

	// Token: 0x04002B2D RID: 11053
	private RectTransform rectFrom;

	// Token: 0x04002B2E RID: 11054
	private RectTransform rectTo;

	// Token: 0x04002B2F RID: 11055
	private RectTransform scaleReference;
}
