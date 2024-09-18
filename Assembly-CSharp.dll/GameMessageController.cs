using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000503 RID: 1283
public class GameMessageController : MonoBehaviour
{
	// Token: 0x06001BB0 RID: 7088 RVA: 0x0018E9C8 File Offset: 0x0018CBC8
	private void OnEnable()
	{
		this.messageText.text = string.Empty;
		this.lensFlare.gameObject.SetActive(false);
		this.rect.sizeDelta = new Vector2(0f, this.rect.sizeDelta.y);
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x0018EA1C File Offset: 0x0018CC1C
	public void Setup(Sprite graphic, string message, RectTransform moveToTarget, bool colourOverride = false, Color col = default(Color), GameMessageController.PingOnComplete newPing = GameMessageController.PingOnComplete.none, Evidence keyTieEvidence = null, List<Evidence.DataKey> newTiedKeys = null, int value = 0)
	{
		this.displayMessage = message;
		if (this.img != null)
		{
			this.img.sprite = graphic;
		}
		this.messageText.text = string.Empty;
		this.moveToTargetOnDestroy = moveToTarget;
		this.ping = newPing;
		if (colourOverride && this.img != null)
		{
			this.img.color = col;
		}
		this.juice.Flash(2, colourOverride, col, 10f);
		if (this.isKeyMergeMessage)
		{
			this.keyMergeProgress.barMax = (float)keyTieEvidence.preset.GetValidProfileKeys().Count;
			this.keyMergeProgress.SetValue((float)value);
			List<Evidence.DataKey> tiedKeys = keyTieEvidence.GetTiedKeys(newTiedKeys);
			this.tiedKeysValue = keyTieEvidence.preset.GetProfileKeyCount(tiedKeys);
			List<Evidence.DataKey> list = tiedKeys.FindAll((Evidence.DataKey item) => keyTieEvidence.preset.IsKeyUnique(item));
			string text = string.Empty;
			if (this.namePiece != null)
			{
				this.namePiece.gameObject.SetActive(false);
			}
			if (this.photoPiece != null)
			{
				this.photoPiece.gameObject.SetActive(false);
			}
			if (this.voicePiece != null)
			{
				this.voicePiece.gameObject.SetActive(false);
			}
			if (this.fingerprintPiece != null)
			{
				this.fingerprintPiece.gameObject.SetActive(false);
			}
			if (list.Count > 0)
			{
				bool flag = false;
				text = "\n" + Strings.Get("ui.gamemessage", "Linked to", Strings.Casing.asIs, false, false, false, null) + "\n";
				foreach (Evidence.DataKey dataKey in list)
				{
					if (flag)
					{
						text += ", ";
					}
					text += Strings.Get("evidence.generic", dataKey.ToString(), Strings.Casing.firstLetterCaptial, false, false, false, null);
					flag = true;
					if (dataKey == Evidence.DataKey.name)
					{
						if (this.namePiece != null)
						{
							this.namePiece.gameObject.SetActive(true);
						}
					}
					else if (dataKey == Evidence.DataKey.photo)
					{
						if (this.photoPiece != null)
						{
							this.photoPiece.gameObject.SetActive(true);
						}
					}
					else if (dataKey == Evidence.DataKey.voice)
					{
						if (this.voicePiece != null)
						{
							this.voicePiece.gameObject.SetActive(true);
						}
					}
					else if (dataKey == Evidence.DataKey.fingerprints && this.fingerprintPiece != null)
					{
						this.fingerprintPiece.gameObject.SetActive(true);
					}
				}
			}
			this.displayMessage = string.Concat(new string[]
			{
				keyTieEvidence.GetNameForDataKey(tiedKeys),
				": ",
				Strings.Get("ui.gamemessage", "New Information", Strings.Casing.asIs, false, false, false, null),
				" (",
				this.tiedKeysValue.ToString(),
				"/",
				keyTieEvidence.preset.GetValidProfileKeys().Count.ToString(),
				")",
				text
			});
			return;
		}
		if (this.isSocialCreditMessage)
		{
			this.originalCredit = value;
			this.SocialScoreVisualUpdate(value);
		}
	}

	// Token: 0x06001BB2 RID: 7090 RVA: 0x0018ED88 File Offset: 0x0018CF88
	public void SocialScoreVisualUpdate(int points)
	{
		int socialCreditLevel = GameplayController.Instance.GetSocialCreditLevel(points);
		int level = socialCreditLevel + 1;
		this.socialCreditLevelText.text = socialCreditLevel.ToString();
		int socialCreditThresholdForLevel = GameplayController.Instance.GetSocialCreditThresholdForLevel(socialCreditLevel);
		int socialCreditThresholdForLevel2 = GameplayController.Instance.GetSocialCreditThresholdForLevel(level);
		this.keyMergeProgress.barMin = 0f;
		this.keyMergeProgress.barMax = (float)(socialCreditThresholdForLevel2 - socialCreditThresholdForLevel);
		int num = points - socialCreditThresholdForLevel;
		this.keyMergeProgress.SetValue((float)num);
		this.displayMessage = string.Concat(new string[]
		{
			Strings.Get("ui.gamemessage", "Social Credit Score", Strings.Casing.asIs, false, false, false, null),
			": ",
			num.ToString(),
			"/",
			this.keyMergeProgress.barMax.ToString()
		});
		Game.Log(string.Concat(new string[]
		{
			"display for points: ",
			points.ToString(),
			", current level: ",
			socialCreditLevel.ToString(),
			" (",
			socialCreditThresholdForLevel.ToString(),
			"), next level: ",
			level.ToString(),
			" (",
			socialCreditThresholdForLevel2.ToString(),
			"), original credit: ",
			this.originalCredit.ToString(),
			" applied: ",
			num.ToString(),
			"/",
			this.keyMergeProgress.barMax.ToString()
		}), 2);
	}

	// Token: 0x06001BB3 RID: 7091 RVA: 0x0018EF0C File Offset: 0x0018D10C
	private void Update()
	{
		if (this.revealProgress < 1f)
		{
			this.revealProgress += Time.deltaTime / 0.18f;
			this.revealProgress = Mathf.Clamp01(this.revealProgress);
			foreach (CanvasRenderer canvasRenderer in this.renderers)
			{
				if (canvasRenderer != null)
				{
					canvasRenderer.SetAlpha(this.revealProgress);
				}
			}
		}
		if (this.progress < (float)this.displayMessage.Length)
		{
			this.progress += Time.deltaTime * InterfaceControls.Instance.gameMessageTextRevealSpeed;
			this.messageText.text = this.displayMessage.Substring(0, Mathf.Min(Mathf.RoundToInt(this.progress), this.displayMessage.Length));
			float num = 0f;
			if (this.isKeyMergeMessage)
			{
				num = 100f;
			}
			else if (this.isSocialCreditMessage)
			{
				num = 100f;
			}
			this.rect.sizeDelta = new Vector2(Mathf.Max(this.messageText.preferredWidth + 66f + num, 66f), this.rect.sizeDelta.y);
			this.lensFlare.gameObject.SetActive(true);
		}
		else
		{
			this.delayProgress += Time.deltaTime;
			float num2 = 0f;
			if (this.isKeyMergeMessage)
			{
				num2 = 0.1f;
			}
			else if (this.isSocialCreditMessage)
			{
				num2 = 0.4f;
			}
			if (this.delayProgress >= InterfaceControls.Instance.gameMessageDestroyDelay + num2)
			{
				this.fadeProgress += Time.deltaTime / 0.25f;
				this.fadeProgress = Mathf.Clamp01(this.fadeProgress);
				if (this.moveToTargetOnDestroy != null)
				{
					this.rect.position = Vector3.Lerp(this.rect.position, this.moveToTargetOnDestroy.position, this.fadeProgress);
				}
				foreach (CanvasRenderer canvasRenderer2 in this.renderers)
				{
					canvasRenderer2.SetAlpha(Mathf.Clamp01(1f - this.fadeProgress * 1.25f));
				}
				if (this.fadeProgress > 0.85f)
				{
					if (this.ping == GameMessageController.PingOnComplete.money)
					{
						InterfaceController.Instance.PingMoney();
						this.ping = GameMessageController.PingOnComplete.none;
					}
					else if (this.ping == GameMessageController.PingOnComplete.lockpicks)
					{
						InterfaceController.Instance.PingLockpicks();
						this.ping = GameMessageController.PingOnComplete.none;
					}
					if (this.fadeProgress >= 1f)
					{
						Object.Destroy(base.gameObject);
					}
				}
			}
		}
		if (this.isKeyMergeMessage)
		{
			if (this.revealProgress >= 1f && this.keyTieProgress < 1f)
			{
				this.keyTieProgress += Time.deltaTime / 1f;
				this.keyTieProgress = Mathf.Clamp01(this.keyTieProgress);
			}
			this.keyMergeProgress.SetValue((float)this.tiedKeysValue * this.keyTieProgress);
		}
		if (this.isSocialCreditMessage && this.revealProgress >= 1f)
		{
			if (this.socCreditProgress < 1f)
			{
				this.socCreditProgress += Time.deltaTime / 1f;
				this.socCreditProgress = Mathf.Clamp01(this.socCreditProgress);
				this.SocialScoreVisualUpdate(this.originalCredit + Mathf.CeilToInt((float)(GameplayController.Instance.socialCredit - this.originalCredit) * this.socCreditProgress));
				return;
			}
			this.SocialScoreVisualUpdate(GameplayController.Instance.socialCredit);
		}
	}

	// Token: 0x0400245F RID: 9311
	public RectTransform rect;

	// Token: 0x04002460 RID: 9312
	public string displayMessage;

	// Token: 0x04002461 RID: 9313
	public TextMeshProUGUI messageText;

	// Token: 0x04002462 RID: 9314
	public Image img;

	// Token: 0x04002463 RID: 9315
	public JuiceController juice;

	// Token: 0x04002464 RID: 9316
	public RectTransform lensFlare;

	// Token: 0x04002465 RID: 9317
	public bool isKeyMergeMessage;

	// Token: 0x04002466 RID: 9318
	public ProgressBarController keyMergeProgress;

	// Token: 0x04002467 RID: 9319
	public bool isSocialCreditMessage;

	// Token: 0x04002468 RID: 9320
	public int originalCredit;

	// Token: 0x04002469 RID: 9321
	public Sprite checkedSprite;

	// Token: 0x0400246A RID: 9322
	public Image puzzleBG;

	// Token: 0x0400246B RID: 9323
	public Image namePiece;

	// Token: 0x0400246C RID: 9324
	public Image photoPiece;

	// Token: 0x0400246D RID: 9325
	public Image voicePiece;

	// Token: 0x0400246E RID: 9326
	public Image fingerprintPiece;

	// Token: 0x0400246F RID: 9327
	public TextMeshProUGUI socialCreditLevelText;

	// Token: 0x04002470 RID: 9328
	public GameMessageController.PingOnComplete ping;

	// Token: 0x04002471 RID: 9329
	public float progress;

	// Token: 0x04002472 RID: 9330
	public float delayProgress;

	// Token: 0x04002473 RID: 9331
	public float fadeProgress;

	// Token: 0x04002474 RID: 9332
	public float revealProgress;

	// Token: 0x04002475 RID: 9333
	public float keyTieProgress;

	// Token: 0x04002476 RID: 9334
	public float socCreditProgress;

	// Token: 0x04002477 RID: 9335
	private int tiedKeysValue;

	// Token: 0x04002478 RID: 9336
	[ReorderableList]
	public List<CanvasRenderer> renderers = new List<CanvasRenderer>();

	// Token: 0x04002479 RID: 9337
	public RectTransform moveToTargetOnDestroy;

	// Token: 0x02000504 RID: 1284
	public enum PingOnComplete
	{
		// Token: 0x0400247B RID: 9339
		none,
		// Token: 0x0400247C RID: 9340
		lockpicks,
		// Token: 0x0400247D RID: 9341
		money
	}
}
