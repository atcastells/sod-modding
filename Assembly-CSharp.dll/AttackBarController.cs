using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class AttackBarController : MonoBehaviour
{
	// Token: 0x06001CFE RID: 7422 RVA: 0x0019BFAC File Offset: 0x0019A1AC
	public void Setup(NewAIController newAi)
	{
		this.ai = newAi;
		foreach (CanvasRenderer canvasRenderer in this.allGraphics)
		{
			canvasRenderer.SetAlpha(0f);
		}
		this.blockPoint.sizeDelta = new Vector2(GameplayControls.Instance.successfulBlockThreshold * this.rect.sizeDelta.x, this.blockPoint.sizeDelta.y);
		this.perfectBlockPoint.sizeDelta = new Vector2(GameplayControls.Instance.perfectBlockThreshold * this.rect.sizeDelta.x, this.perfectBlockPoint.sizeDelta.y);
	}

	// Token: 0x06001CFF RID: 7423 RVA: 0x0019C080 File Offset: 0x0019A280
	private void Update()
	{
		this.barProgress = Mathf.Clamp01(this.ai.attackProgress / this.ai.currentWeaponPreset.attackTriggerPoint);
		this.attackProgress.anchoredPosition = new Vector2(this.barProgress * this.rect.sizeDelta.x, 0f);
		float num = Vector3.SignedAngle(this.ai.human.transform.position + new Vector3(0f, 1.5f, 0f) - Player.Instance.transform.position, Player.Instance.transform.forward, Vector3.up);
		foreach (CanvasRenderer canvasRenderer in this.allGraphics)
		{
			canvasRenderer.SetAlpha(0f);
		}
		if (Mathf.Abs(num) < 75f)
		{
			if (!this.displayOnScreen)
			{
				this.displayOnScreen = true;
			}
			Vector3 vector = CameraController.Instance.cam.WorldToScreenPoint(this.ai.human.transform.position + new Vector3(0f, 1.5f, 0f));
			Vector2 vector2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceController.Instance.firstPersonUI, vector, null, ref vector2);
			this.rect.position = InterfaceControls.Instance.speechBubbleParent.TransformPoint(vector2);
			this.distance = Vector3.Distance(Player.Instance.transform.position, this.ai.human.transform.position);
			float num2 = 1f - Mathf.Clamp01(this.distance / AudioControls.Instance.speakEvent.hearingRange);
			foreach (CanvasRenderer canvasRenderer2 in this.allGraphics)
			{
				canvasRenderer2.SetAlpha(num2);
			}
			float num3 = Mathf.Lerp(InterfaceControls.Instance.speechMinMaxScale.x, InterfaceControls.Instance.speechMinMaxScale.y, num2);
			this.rect.localScale = new Vector3(num3, num3, num3);
		}
		else
		{
			this.displayOnScreen = false;
		}
		if (!this.ai.attackActive && !this.removeAbort && !this.removeBlocked && !this.removeHit)
		{
			this.removeAbort = true;
			this.abortProgress = this.ai.attackProgress;
		}
		if (this.removeAbort || this.removeBlocked || this.removeHit || Player.Instance.playerKOInProgress)
		{
			this.removalProgress += Time.deltaTime * 4f;
			if (this.displayOnScreen && !Player.Instance.playerKOInProgress)
			{
				using (List<CanvasRenderer>.Enumerator enumerator = this.allGraphics.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CanvasRenderer canvasRenderer3 = enumerator.Current;
						canvasRenderer3.SetAlpha(1f - this.removalProgress);
					}
					goto IL_351;
				}
			}
			foreach (CanvasRenderer canvasRenderer4 in this.allGraphics)
			{
				canvasRenderer4.SetAlpha(0f);
			}
			IL_351:
			this.attackProgress.anchoredPosition = new Vector2(this.abortProgress * this.rect.sizeDelta.x, 0f);
			if (this.removeBlocked)
			{
				float num4 = Mathf.Lerp(1f, 3f, this.removalProgress);
				this.blockPoint.localScale = new Vector3(num4, num4, num4);
				if (!this.displayOnScreen)
				{
					goto IL_4C2;
				}
				using (List<CanvasRenderer>.Enumerator enumerator = this.blockGraphics.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CanvasRenderer canvasRenderer5 = enumerator.Current;
						canvasRenderer5.SetAlpha(Mathf.Clamp01(2f - this.removalProgress * 2f));
					}
					goto IL_4C2;
				}
			}
			if (this.removeHit)
			{
				float num5 = Mathf.Lerp(1f, 3f, this.removalProgress);
				this.attackProgress.localScale = new Vector3(num5, num5, num5);
				if (this.displayOnScreen)
				{
					foreach (CanvasRenderer canvasRenderer6 in this.hitGraphics)
					{
						canvasRenderer6.SetAlpha(Mathf.Clamp01(2f - this.removalProgress * 2f));
					}
				}
				this.attackProgress.anchoredPosition = new Vector2(1f * this.rect.sizeDelta.x, 0f);
			}
			IL_4C2:
			if (this.removalProgress >= 1f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x04002688 RID: 9864
	public NewAIController ai;

	// Token: 0x04002689 RID: 9865
	public RectTransform rect;

	// Token: 0x0400268A RID: 9866
	public RectTransform barAnchor;

	// Token: 0x0400268B RID: 9867
	public RectTransform attackProgress;

	// Token: 0x0400268C RID: 9868
	public RectTransform blockPoint;

	// Token: 0x0400268D RID: 9869
	public RectTransform perfectBlockPoint;

	// Token: 0x0400268E RID: 9870
	public float barProgress;

	// Token: 0x0400268F RID: 9871
	public bool displayOnScreen = true;

	// Token: 0x04002690 RID: 9872
	public float distance;

	// Token: 0x04002691 RID: 9873
	[Header("Graphic Elements")]
	public List<CanvasRenderer> allGraphics = new List<CanvasRenderer>();

	// Token: 0x04002692 RID: 9874
	public List<CanvasRenderer> backgroundGraphics = new List<CanvasRenderer>();

	// Token: 0x04002693 RID: 9875
	public List<CanvasRenderer> blockGraphics = new List<CanvasRenderer>();

	// Token: 0x04002694 RID: 9876
	public List<CanvasRenderer> hitGraphics = new List<CanvasRenderer>();

	// Token: 0x04002695 RID: 9877
	[Header("Removal")]
	public float removalProgress;

	// Token: 0x04002696 RID: 9878
	public bool removeHit;

	// Token: 0x04002697 RID: 9879
	public bool removeBlocked;

	// Token: 0x04002698 RID: 9880
	public bool removeAbort;

	// Token: 0x04002699 RID: 9881
	public float abortProgress;
}
