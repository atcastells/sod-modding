using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200053E RID: 1342
public class SplashController : MonoBehaviour
{
	// Token: 0x06001D48 RID: 7496 RVA: 0x0019EB3C File Offset: 0x0019CD3C
	private void Awake()
	{
		foreach (SplashController.SplashImage splashImage in this.splashes)
		{
			splashImage.rend.SetAlpha(0f);
			splashImage.rect.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x0019EBA8 File Offset: 0x0019CDA8
	private void Update()
	{
		if (this.fadeOut)
		{
			if (this.fadeProg > 0f)
			{
				this.fadeProg -= Time.deltaTime / this.fadeOutTime;
				this.fadeProg = Mathf.Clamp01(this.fadeProg);
				this.blackBG.canvasRenderer.SetAlpha(this.fadeProg);
				return;
			}
			MainMenuController.Instance.SetMenuComponent(MainMenuController.Component.mainMenuButtons);
			base.enabled = false;
			return;
		}
		else
		{
			if (this.splash >= this.splashes.Count)
			{
				this.fadeOut = true;
				return;
			}
			SplashController.SplashImage splashImage = this.splashes[this.splash];
			if (!splashImage.rect.gameObject.activeSelf)
			{
				splashImage.rect.gameObject.SetActive(true);
			}
			this.progress += Time.deltaTime / splashImage.displayTime;
			if (this.progress <= 0.1f)
			{
				splashImage.rend.SetAlpha(this.progress * 10f);
			}
			else if (this.progress >= 0.9f)
			{
				splashImage.rend.SetAlpha((1f - this.progress) * 10f);
			}
			else
			{
				splashImage.rend.SetAlpha(1f);
			}
			if (this.progress >= 1f)
			{
				splashImage.rect.gameObject.SetActive(false);
				this.progress = 0f;
				this.splash++;
			}
			return;
		}
	}

	// Token: 0x04002703 RID: 9987
	public Image blackBG;

	// Token: 0x04002704 RID: 9988
	public List<SplashController.SplashImage> splashes = new List<SplashController.SplashImage>();

	// Token: 0x04002705 RID: 9989
	public float progress;

	// Token: 0x04002706 RID: 9990
	public int splash;

	// Token: 0x04002707 RID: 9991
	public float fadeOutTime = 1.25f;

	// Token: 0x04002708 RID: 9992
	public bool fadeOut;

	// Token: 0x04002709 RID: 9993
	public float fadeProg = 1f;

	// Token: 0x0200053F RID: 1343
	[Serializable]
	public class SplashImage
	{
		// Token: 0x0400270A RID: 9994
		public RectTransform rect;

		// Token: 0x0400270B RID: 9995
		public CanvasRenderer rend;

		// Token: 0x0400270C RID: 9996
		public float displayTime = 1.5f;
	}
}
