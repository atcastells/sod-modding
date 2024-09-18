using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005F9 RID: 1529
public class RevealResolveController : MonoBehaviour
{
	// Token: 0x060021B0 RID: 8624 RVA: 0x001C9DE8 File Offset: 0x001C7FE8
	public void Setup(Case.ResolveQuestion newQuestion, Case newCase, float newRevealAfter)
	{
		this.question = newQuestion;
		this.qText = this.question.GetText(newCase, false, false);
		this.questionText.text = string.Empty;
		this.revealAfterTimer = newRevealAfter;
		this.isCorrect = this.question.isCorrect;
		this.tick.SetActive(false);
		this.cross.SetActive(false);
		foreach (CanvasRenderer canvasRenderer in this.fadeInRenderers)
		{
			canvasRenderer.SetAlpha(0f);
		}
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x001C9E98 File Offset: 0x001C8098
	private void Update()
	{
		if (this.revealPhase == 0)
		{
			if (this.revealAfterTimer > 0f)
			{
				this.revealAfterTimer -= Time.deltaTime;
				return;
			}
			this.fadeIn += Time.deltaTime;
			this.fadeIn = Mathf.Clamp01(this.fadeIn);
			foreach (CanvasRenderer canvasRenderer in this.fadeInRenderers)
			{
				canvasRenderer.SetAlpha(this.fadeIn);
			}
			int num = Mathf.CeilToInt((float)this.qText.Length * this.fadeIn);
			try
			{
				this.questionText.text = this.qText.Substring(0, num);
			}
			catch
			{
			}
			if (this.fadeIn >= 1f)
			{
				this.revealPhase = 1;
				return;
			}
		}
		else if (this.revealPhase == 1)
		{
			if (this.revealCorrectTimer > 0f)
			{
				this.revealCorrectTimer -= Time.deltaTime;
				return;
			}
			if (this.isCorrect)
			{
				this.tick.SetActive(true);
				this.tickJuice.Nudge();
			}
			else
			{
				this.cross.SetActive(true);
				this.crossJuice.Nudge();
			}
			this.revealPhase = 2;
			return;
		}
		else if (this.revealPhase == 2)
		{
			if (this.waitTimer > 0f)
			{
				this.waitTimer -= Time.deltaTime;
				return;
			}
			this.revealPhase = 3;
			return;
		}
		else if (this.revealPhase == 3)
		{
			if (this.removeTimer > 0f)
			{
				this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x + 60f / this.removeTimer, this.rect.anchoredPosition.y);
				this.removeTimer -= Time.deltaTime;
				return;
			}
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04002C12 RID: 11282
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002C13 RID: 11283
	public TextMeshProUGUI questionText;

	// Token: 0x04002C14 RID: 11284
	public GameObject tick;

	// Token: 0x04002C15 RID: 11285
	public GameObject cross;

	// Token: 0x04002C16 RID: 11286
	public JuiceController tickJuice;

	// Token: 0x04002C17 RID: 11287
	public JuiceController crossJuice;

	// Token: 0x04002C18 RID: 11288
	public List<CanvasRenderer> fadeInRenderers = new List<CanvasRenderer>();

	// Token: 0x04002C19 RID: 11289
	[Header("State")]
	public bool isCorrect;

	// Token: 0x04002C1A RID: 11290
	public float revealAfterTimer;

	// Token: 0x04002C1B RID: 11291
	public float fadeIn;

	// Token: 0x04002C1C RID: 11292
	public float revealCorrectTimer = 0.75f;

	// Token: 0x04002C1D RID: 11293
	public float waitTimer = 2.6f;

	// Token: 0x04002C1E RID: 11294
	public float removeTimer = 0.33f;

	// Token: 0x04002C1F RID: 11295
	public string qText;

	// Token: 0x04002C20 RID: 11296
	public int revealPhase;

	// Token: 0x04002C21 RID: 11297
	private Case.ResolveQuestion question;
}
