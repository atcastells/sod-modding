using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200052F RID: 1327
public class ReactionIndicatorController : MonoBehaviour
{
	// Token: 0x06001D04 RID: 7428 RVA: 0x0019C6BB File Offset: 0x0019A8BB
	public void Setup(Actor newActor)
	{
		this.actor = newActor;
		this.bubbleRect.sizeDelta = new Vector2(1f, 1f);
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x0019C6E0 File Offset: 0x0019A8E0
	public void UpdateReactionType()
	{
		if (this.actor.ai.reactionState != this.previousReactionState)
		{
			if ((this.actor.ai.reactionState == NewAIController.ReactionState.investigatingSight || this.actor.ai.reactionState == NewAIController.ReactionState.investigatingSound) && this.awarenessIcon != null)
			{
				this.awarenessIcon.TriggerAlert();
			}
			this.previousReactionState = this.actor.ai.reactionState;
		}
		if (this.actor.ai.reactionState == NewAIController.ReactionState.investigatingSight)
		{
			this.img.sprite = InterfaceControls.Instance.reactionInvestigateSightSprite;
			if (this.awarenessIcon != null)
			{
				this.awarenessIcon.SetTexture(InterfaceControls.Instance.reactionInvestigateSightTex);
				return;
			}
		}
		else if (this.actor.ai.reactionState == NewAIController.ReactionState.investigatingSound)
		{
			this.img.sprite = InterfaceControls.Instance.reactionInvestigateSoundSprite;
			if (this.awarenessIcon != null)
			{
				this.awarenessIcon.SetTexture(InterfaceControls.Instance.reactionInvestigateSoundTex);
				return;
			}
		}
		else if (this.actor.ai.reactionState == NewAIController.ReactionState.persuing)
		{
			this.img.sprite = InterfaceControls.Instance.reactionPersueSprite;
			if (this.awarenessIcon != null)
			{
				this.awarenessIcon.SetTexture(InterfaceControls.Instance.reactionPersueTex);
				return;
			}
		}
		else if (this.actor.ai.reactionState == NewAIController.ReactionState.searching)
		{
			this.img.sprite = InterfaceControls.Instance.reactionSearchSprite;
			if (this.awarenessIcon != null)
			{
				this.awarenessIcon.SetTexture(InterfaceControls.Instance.reactionSearchTex);
				return;
			}
		}
		else
		{
			this.img.sprite = InterfaceControls.Instance.reactionAvoidSprite;
			if (this.awarenessIcon != null)
			{
				this.awarenessIcon.SetTexture(InterfaceControls.Instance.reactionAvoidTex);
			}
		}
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x0019C8A8 File Offset: 0x0019AAA8
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			Vector3 zero = Vector3.zero;
			if (Mathf.Abs(Vector3.SignedAngle(this.actor.lookAtThisTransform.position + new Vector3(0f, 0.52f, 0f) - Player.Instance.transform.position, Player.Instance.transform.forward, Vector3.up)) < 75f)
			{
				if (!this.displayOnScreen)
				{
					this.displayOnScreen = true;
				}
				this.distance = Vector3.Distance(Player.Instance.lookAtThisTransform.position, this.actor.lookAtThisTransform.position);
				float num = 1f - Mathf.Clamp01(this.distance / InterfaceControls.Instance.maxIndicatorDistance);
				Vector3 vector = CameraController.Instance.cam.WorldToScreenPoint(this.actor.lookAtThisTransform.position + new Vector3(0f, 0.52f, 0f));
				Vector2 vector2;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceController.Instance.firstPersonUI, vector, null, ref vector2);
				this.desiredPosition = InterfaceControls.Instance.speechBubbleParent.TransformPoint(vector2);
				float num2 = Mathf.Clamp01(num * this.actor.spottedState);
				foreach (CanvasRenderer canvasRenderer in this.graphics)
				{
					canvasRenderer.SetAlpha(num2);
				}
				if (this.actor.outline != null)
				{
					this.actor.outline.SetAlpha(Mathf.Clamp01(num2 * 2f));
				}
				if (!this.firstPositionInit)
				{
					this.rect.position = this.desiredPosition;
					this.firstPositionInit = true;
				}
				else
				{
					if (this.desiredPosition != this.rect.position)
					{
						this.rect.position = this.desiredPosition;
					}
					if (this.bubbleDesiredSize != this.bubbleRect.sizeDelta)
					{
						this.bubbleRect.sizeDelta = Vector2.Lerp(this.bubbleRect.sizeDelta, this.bubbleDesiredSize, Time.deltaTime * 9f * SessionData.Instance.currentTimeMultiplier);
					}
				}
				float num3 = Mathf.Lerp(InterfaceControls.Instance.indicatorMinMaxScale.x, InterfaceControls.Instance.indicatorMinMaxScale.y, num);
				this.rect.localScale = new Vector3(num3, num3, num3);
				if (this.actor.isDead)
				{
					this.removeFade = true;
				}
				if (this.removeFade)
				{
					this.removalProgress += Time.deltaTime * 4f;
					foreach (CanvasRenderer canvasRenderer2 in this.graphics)
					{
						canvasRenderer2.SetAlpha(num2 - this.removalProgress);
					}
					if (this.actor.outline != null)
					{
						this.actor.outline.SetAlpha(Mathf.Clamp01(Mathf.Clamp01(num2 * 2f) - this.removalProgress));
					}
					if (this.removalProgress >= 1f)
					{
						if (this.actor.outline != null)
						{
							this.actor.outline.SetOutlineActive(false);
						}
						if (this.awarenessIcon != null)
						{
							this.awarenessIcon.Remove(false);
						}
						Object.Destroy(base.gameObject);
						return;
					}
				}
			}
			else
			{
				if (this.displayOnScreen)
				{
					this.displayOnScreen = false;
					foreach (CanvasRenderer canvasRenderer3 in this.graphics)
					{
						canvasRenderer3.SetAlpha(0f);
					}
				}
				if (this.removeFade)
				{
					if (this.actor.outline != null)
					{
						this.actor.outline.SetOutlineActive(false);
					}
					if (this.awarenessIcon != null)
					{
						this.awarenessIcon.Remove(false);
					}
					Object.Destroy(base.gameObject);
				}
			}
		}
	}

	// Token: 0x040026A0 RID: 9888
	public RectTransform rect;

	// Token: 0x040026A1 RID: 9889
	public RectTransform bubbleRect;

	// Token: 0x040026A2 RID: 9890
	public Image img;

	// Token: 0x040026A3 RID: 9891
	public float distance;

	// Token: 0x040026A4 RID: 9892
	public float fadeProgress;

	// Token: 0x040026A5 RID: 9893
	public Actor actor;

	// Token: 0x040026A6 RID: 9894
	public InterfaceController.AwarenessIcon awarenessIcon;

	// Token: 0x040026A7 RID: 9895
	private NewAIController.ReactionState previousReactionState;

	// Token: 0x040026A8 RID: 9896
	public List<CanvasRenderer> graphics = new List<CanvasRenderer>();

	// Token: 0x040026A9 RID: 9897
	public Vector2 bubbleDesiredSize = Vector2.zero;

	// Token: 0x040026AA RID: 9898
	public bool displayOnScreen = true;

	// Token: 0x040026AB RID: 9899
	public Vector3 desiredPosition = Vector3.zero;

	// Token: 0x040026AC RID: 9900
	private bool firstPositionInit;

	// Token: 0x040026AD RID: 9901
	[Header("Removal")]
	public float removalProgress;

	// Token: 0x040026AE RID: 9902
	public bool removeHit;

	// Token: 0x040026AF RID: 9903
	public bool removeBlocked;

	// Token: 0x040026B0 RID: 9904
	public bool removeFade;

	// Token: 0x040026B1 RID: 9905
	public float abortProgress;
}
