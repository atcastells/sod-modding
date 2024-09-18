using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000531 RID: 1329
public class SpeechBubbleController : MonoBehaviour
{
	// Token: 0x06001D0D RID: 7437 RVA: 0x0019CF80 File Offset: 0x0019B180
	public void Setup(SpeechController.QueueElement newSpeech, SpeechController newSpeechController)
	{
		this.speech = newSpeech;
		this.speechController = newSpeechController;
		InterfaceController.Instance.activeSpeechBubbles.Add(this);
		Actor actor = this.speechController.actor;
		Human genderReference = null;
		if (actor != null)
		{
			genderReference = (actor as Human);
			actor.isSpeaking = true;
			if (actor.isPlayer)
			{
				this.isPlayer = true;
				this.text.color = InterfaceControls.Instance.playerSpeechColour;
			}
		}
		if (this.speech.forceColour)
		{
			this.text.color = this.speech.color;
		}
		this.bubbleRect.gameObject.SetActive(false);
		this.bubbleRect.sizeDelta = Vector2.one;
		this.actualString = Strings.Get(this.speech.dictRef, this.speech.entryRef, Strings.Casing.asIs, false, false, true, genderReference).Trim();
		SideJob sideJob = null;
		if (this.speech.jobRef > -1 && SideJobController.Instance.allJobsDictionary.TryGetValue(this.speech.jobRef, ref sideJob))
		{
			Game.Log("Found job reference to use as additional input object...", 2);
		}
		Human human = null;
		if (this.speech.speakingAbout > -1 && CityData.Instance.GetHuman(this.speech.speakingAbout, out human, true))
		{
			Game.Log("Found talking about reference to use as additional input object...", 2);
		}
		if (this.speech.useParsing)
		{
			object obj = null;
			Human human2 = actor as Human;
			if (human2 != null)
			{
				obj = human2;
			}
			else
			{
				if (this.speechController.phoneLine != null && InteractionController.Instance.dialogMode && InteractionController.Instance.remoteOverride != null && InteractionController.Instance.remoteOverride.speechController == this.speechController && !InteractionController.Instance.talkingTo.isActor.isPlayer)
				{
					obj = InteractionController.Instance.talkingTo.isActor;
					Game.Log("Input object telephone: " + ((obj != null) ? obj.ToString() : null), 2);
				}
				if (obj == null && this.speechController.interactable != null)
				{
					obj = this.speechController.interactable;
				}
			}
			object additionalObject = sideJob;
			if (sideJob == null && human != null)
			{
				additionalObject = human;
			}
			this.actualString = Strings.ComposeText(this.actualString, obj, Strings.LinkSetting.forceNoLinks, null, additionalObject, false);
		}
		if (this.speech.shouting)
		{
			this.actualString = this.actualString.ToUpper();
		}
		this.oncreenTime = (InterfaceControls.Instance.visualTalkDisplayDestroyDelay + (float)this.actualString.Length * InterfaceControls.Instance.visualTalkDisplayStringLengthModifier) / Game.Instance.textSpeed;
		this.text.text = string.Empty;
		this.text.fontSize = InterfaceControls.Instance.visualTalkTextSize;
		this.timeStamp = SessionData.Instance.gameTime;
		this.stringReveal = 1f;
		this.revealedChars = 0;
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x0019D268 File Offset: 0x0019B468
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			if (!this.isPlayer && InteractionController.Instance.dialogMode && InteractionController.Instance.talkingTo != this.speechController.interactable && (InteractionController.Instance.remoteOverride == null || InteractionController.Instance.remoteOverride != this.speechController.interactable))
			{
				this.bgRend.gameObject.SetActive(false);
				this.textRend.gameObject.SetActive(false);
				this.backgroundImg.enabled = false;
			}
			else if (!this.bgRend.gameObject.activeSelf)
			{
				this.bgRend.gameObject.SetActive(true);
				this.textRend.gameObject.SetActive(true);
				this.backgroundImg.enabled = true;
			}
			this.distance = Vector3.Distance(Player.Instance.lookAtThisTransform.position, this.speechController.interactable.wPos);
			float num = 1f - Mathf.Clamp01(this.distance / AudioControls.Instance.speakEvent.hearingRange);
			this.text.fontSize = InterfaceControls.Instance.visualTalkTextSize;
			float num2 = Time.deltaTime * (InterfaceControls.Instance.visualTalkDisplaySpeed * SessionData.Instance.currentTimeMultiplier * Game.Instance.textSpeed);
			this.stringReveal += num2;
			if (this.stringReveal >= 1f && this.revealedChars < this.actualString.Length)
			{
				this.stringReveal = 0f;
				this.revealedChars++;
			}
			string text = string.Empty;
			if (this.revealedChars <= this.actualString.Length && this.stringReveal <= 1f)
			{
				float num3 = this.stringReveal;
				Color white = Color.white;
				white.a = this.stringReveal;
				string text2 = ColorUtility.ToHtmlStringRGBA(white).Substring(6, 2);
				text = string.Concat(new string[]
				{
					"<size=",
					Mathf.CeilToInt(this.stringReveal * 100f).ToString(),
					"%><alpha=#",
					text2,
					">",
					this.actualString.Substring(this.revealedChars - 1, 1)
				});
				this.text.text = this.actualString.Substring(0, this.revealedChars - 1) + text;
			}
			else if (this.revealedChars >= this.actualString.Length)
			{
				if (!this.setFinalText)
				{
					this.text.text = this.actualString;
					this.setFinalText = true;
				}
				this.delayProgress += Time.deltaTime * SessionData.Instance.currentTimeMultiplier;
				if (this.delayProgress >= this.oncreenTime)
				{
					if (this.awarenessIcon != null)
					{
						this.awarenessIcon.Remove(false);
					}
					this.fadeProgress += Time.deltaTime * 2f * SessionData.Instance.currentTimeMultiplier;
					this.fadeProgress = Mathf.Clamp01(this.fadeProgress);
					this.bgRend.SetAlpha(1f - this.fadeProgress);
					this.textRend.SetAlpha(1f - this.fadeProgress);
					if (this.fadeProgress >= 1f)
					{
						Object.Destroy(base.gameObject);
					}
				}
			}
			Vector3 zero = Vector3.zero;
			float num4 = 0f;
			if (!this.isPlayer)
			{
				Vector3 vector = Vector3.zero;
				if (this.speechController.actor != null)
				{
					vector = this.speechController.actor.lookAtThisTransform.position + new Vector3(0f, CitizenControls.Instance.speechBubbleHeight, 0f);
				}
				else if (this.speechController.interactable != null)
				{
					vector = this.speechController.interactable.wPos + new Vector3(0f, CitizenControls.Instance.speechBubbleHeight, 0f);
				}
				num4 = Vector3.SignedAngle(vector - Player.Instance.transform.position, Player.Instance.transform.forward, Vector3.up);
			}
			if (!this.isPlayer && Mathf.Abs(num4) < 75f && InteractionController.Instance.talkingTo != this.speechController.interactable)
			{
				if (!this.displayOnScreen)
				{
					this.displayOnScreen = true;
					base.transform.SetParent(InterfaceControls.Instance.speechBubbleParent, true);
					InterfaceController.Instance.anchoredSpeech.Remove(this);
				}
				Vector3 vector2 = Vector3.zero;
				if (this.speechController.actor != null)
				{
					vector2 = this.speechController.actor.lookAtThisTransform.position + new Vector3(0f, 0.35f, 0f);
				}
				else if (this.speechController.interactable != null)
				{
					vector2 = this.speechController.interactable.wPos + new Vector3(0f, 0.35f, 0f);
				}
				Vector3 vector3 = CameraController.Instance.cam.WorldToScreenPoint(vector2);
				Vector2 vector4;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceController.Instance.firstPersonUI, vector3, null, ref vector4);
				this.desiredPosition = InterfaceControls.Instance.speechBubbleParent.TransformPoint(vector4);
			}
			else if (this.displayOnScreen)
			{
				this.displayOnScreen = false;
				base.transform.SetParent(InterfaceController.Instance.speechDisplayAnchor, true);
				if (!InterfaceController.Instance.anchoredSpeech.Contains(this))
				{
					InterfaceController.Instance.anchoredSpeech.Add(this);
				}
				InterfaceController.Instance.UpdateAnchoredSpeechPositions();
			}
			this.text.rectTransform.sizeDelta = new Vector2(InterfaceControls.Instance.textBubbleMaxWidth, 1080f);
			if (!this.displayOnScreen)
			{
				this.text.rectTransform.sizeDelta = new Vector2(InterfaceController.Instance.speechDisplayAnchor.sizeDelta.x, 1080f);
			}
			this.text.ForceMeshUpdate(false, false);
			float num5 = Mathf.Min(this.text.preferredWidth + InterfaceControls.Instance.textSpaceBuffer.x, InterfaceControls.Instance.textBubbleMaxWidth);
			if (!this.displayOnScreen)
			{
				num5 = Mathf.Min(this.text.preferredWidth, InterfaceController.Instance.speechDisplayAnchor.sizeDelta.x);
			}
			float num6 = this.text.preferredHeight + InterfaceControls.Instance.textSpaceBuffer.y;
			this.text.rectTransform.sizeDelta = new Vector2(num5, num6);
			this.bubbleDesiredSize = this.text.rectTransform.sizeDelta + InterfaceControls.Instance.textSpaceBuffer;
			if (!this.firstPositionInit)
			{
				this.rect.position = this.desiredPosition;
				this.bubbleRect.sizeDelta = this.bubbleDesiredSize;
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
			float num7 = 1f;
			if (!this.isPlayer)
			{
				num7 = Mathf.Lerp(InterfaceControls.Instance.speechMinMaxScale.x, InterfaceControls.Instance.speechMinMaxScale.y, num);
			}
			this.rect.localScale = new Vector3(num7, num7, num7);
			this.bubbleRect.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001D0F RID: 7439 RVA: 0x0019DA64 File Offset: 0x0019BC64
	private void OnDestroy()
	{
		InterfaceController.Instance.activeSpeechBubbles.Remove(this);
		if (this.speech.jobHandIn)
		{
			SideJob sideJob = null;
			if (this.speech.jobRef > -1 && SideJobController.Instance.allJobsDictionary.TryGetValue(this.speech.jobRef, ref sideJob))
			{
				sideJob.Complete();
			}
		}
		if (this.speech.endsDialog)
		{
			if (this.speech.speakingToRef == Player.Instance.interactable.id)
			{
				DialogController.Instance.OnDialogEnd(this.speech.dialog, this.speech.dialogPreset, Player.Instance.interactable, this.speechController.actor, this.speech.jobRef);
			}
			if (Player.Instance.activeCall != null)
			{
				Player.Instance.activeCall.EndCall();
			}
			else
			{
				ActionController.Instance.Return(null, null, Player.Instance);
			}
		}
		this.speechController.SetSpeechActive(false);
		if (this.speechController.actor != null)
		{
			this.speechController.actor.isSpeaking = false;
		}
		InterfaceController.Instance.anchoredSpeech.Remove(this);
	}

	// Token: 0x040026BD RID: 9917
	public RectTransform rect;

	// Token: 0x040026BE RID: 9918
	public RectTransform bubbleRect;

	// Token: 0x040026BF RID: 9919
	public string actualString;

	// Token: 0x040026C0 RID: 9920
	public TextMeshProUGUI text;

	// Token: 0x040026C1 RID: 9921
	public float stringReveal = 1f;

	// Token: 0x040026C2 RID: 9922
	public int revealedChars;

	// Token: 0x040026C3 RID: 9923
	public float distance;

	// Token: 0x040026C4 RID: 9924
	public float timeStamp;

	// Token: 0x040026C5 RID: 9925
	public float oncreenTime;

	// Token: 0x040026C6 RID: 9926
	public float delayProgress;

	// Token: 0x040026C7 RID: 9927
	public float fadeProgress;

	// Token: 0x040026C8 RID: 9928
	private bool setFinalText;

	// Token: 0x040026C9 RID: 9929
	public SpeechController.QueueElement speech;

	// Token: 0x040026CA RID: 9930
	public SpeechController speechController;

	// Token: 0x040026CB RID: 9931
	public Vector2 sizeTreshold = new Vector2(1f, 2f);

	// Token: 0x040026CC RID: 9932
	public InterfaceController.AwarenessIcon awarenessIcon;

	// Token: 0x040026CD RID: 9933
	public Image backgroundImg;

	// Token: 0x040026CE RID: 9934
	public CanvasRenderer bgRend;

	// Token: 0x040026CF RID: 9935
	public CanvasRenderer textRend;

	// Token: 0x040026D0 RID: 9936
	public Vector2 bubbleDesiredSize = Vector2.zero;

	// Token: 0x040026D1 RID: 9937
	public bool displayOnScreen = true;

	// Token: 0x040026D2 RID: 9938
	public Vector3 desiredPosition = Vector3.zero;

	// Token: 0x040026D3 RID: 9939
	public bool isPlayer;

	// Token: 0x040026D4 RID: 9940
	private bool firstPositionInit;
}
