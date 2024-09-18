using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000520 RID: 1312
public class JuiceController : MonoBehaviour
{
	// Token: 0x06001C6A RID: 7274 RVA: 0x00195EA8 File Offset: 0x001940A8
	private void Start()
	{
		foreach (JuiceController.JuiceElement juiceElement in this.elements)
		{
			if (juiceElement.getNormalColourAtStart)
			{
				if (juiceElement.imageElement != null)
				{
					juiceElement.originalColour = juiceElement.imageElement.color;
				}
				else if (juiceElement.rawImageElement != null)
				{
					juiceElement.originalColour = juiceElement.rawImageElement.color;
				}
			}
			if (juiceElement.getNormalTransformAtStart && juiceElement.transformElement != null)
			{
				juiceElement.originalLocalPos = juiceElement.transformElement.localPosition;
				juiceElement.originalLocalRot = juiceElement.transformElement.localEulerAngles;
				juiceElement.originalLocalScale = juiceElement.transformElement.localScale;
			}
		}
		if (this.pulsateOnStart)
		{
			this.Pulsate(true, false);
		}
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x00195F9C File Offset: 0x0019419C
	public void GetOriginalRectSize()
	{
		foreach (JuiceController.JuiceElement juiceElement in this.elements)
		{
			if (juiceElement.transformElement != null)
			{
				juiceElement.originalLocalPos = juiceElement.transformElement.localPosition;
				juiceElement.originalLocalRot = juiceElement.transformElement.localEulerAngles;
				juiceElement.originalLocalScale = juiceElement.transformElement.localScale;
			}
		}
	}

	// Token: 0x06001C6C RID: 7276 RVA: 0x0019602C File Offset: 0x0019422C
	private void Update()
	{
		if (this.pulsateActive)
		{
			if (!this.onOff && this.pulsateProgress < 1f)
			{
				this.pulsateProgress += this.pulsateSpeed * Time.deltaTime;
			}
			else if (this.onOff && this.pulsateProgress > 0f)
			{
				this.pulsateProgress -= this.pulsateSpeed * Time.deltaTime;
			}
			if (this.pulsateProgress >= 1f && !this.onOff)
			{
				this.onOff = true;
			}
			else if (this.pulsateProgress <= 0f && this.onOff)
			{
				this.onOff = false;
			}
			for (int i = 0; i < this.elements.Count; i++)
			{
				JuiceController.JuiceElement juiceElement = this.elements[i];
				if (juiceElement.imageElement != null)
				{
					juiceElement.imageElement.color = Color.Lerp(juiceElement.originalColour, this.pulsateColour, this.pulsateProgress);
				}
				if (juiceElement.rawImageElement != null)
				{
					juiceElement.rawImageElement.color = Color.Lerp(juiceElement.originalColour, this.pulsateColour, this.pulsateProgress);
				}
				if (this.pulsateScale && juiceElement.transformElement != null)
				{
					float num = Mathf.Lerp(1f, 1.5f, this.pulsateProgress);
					juiceElement.transformElement.localScale = new Vector3(num, num, num);
				}
			}
			if (this.smoothPulsateOff && this.pulsateProgress <= 0f)
			{
				this.smoothPulsateOff = false;
				this.pulsateActive = false;
			}
		}
		else if (this.flashActive)
		{
			if (this.cycle < this.flashRepeat && this.flashProgress < 2f)
			{
				this.flashProgress += this.flashSpeed * Time.deltaTime;
				if (this.flashProgress <= 1f)
				{
					this.flashF = this.flashProgress;
				}
				else
				{
					this.flashF = 2f - this.flashProgress;
				}
				for (int j = 0; j < this.elements.Count; j++)
				{
					JuiceController.JuiceElement juiceElement2 = this.elements[j];
					if (juiceElement2.imageElement != null)
					{
						juiceElement2.imageElement.color = Color.Lerp(juiceElement2.originalColour, this.flashColour, this.flashF);
					}
					if (juiceElement2.rawImageElement != null)
					{
						juiceElement2.rawImageElement.color = Color.Lerp(juiceElement2.originalColour, this.flashColour, this.flashF);
					}
				}
				if (this.flashProgress >= 2f)
				{
					this.cycle++;
					this.flashProgress = 0f;
				}
			}
			else
			{
				this.flashActive = false;
			}
		}
		if (this.nudgeActive)
		{
			if (!this.nudgeState)
			{
				this.nudgeProgress += 10f * Time.deltaTime;
				this.nudgeProgress = Mathf.Clamp01(this.nudgeProgress);
				if (this.nudgeProgress >= 1f)
				{
					this.nudgeState = true;
				}
			}
			else
			{
				this.nudgeProgress -= 5f * Time.deltaTime;
				this.nudgeProgress = Mathf.Clamp01(this.nudgeProgress);
				if (this.nudgeProgress <= 0f)
				{
					this.nudgeActive = false;
				}
			}
			for (int k = 0; k < this.elements.Count; k++)
			{
				JuiceController.JuiceElement juiceElement3 = this.elements[k];
				if (juiceElement3.transformElement != null)
				{
					if (this.nudgeEffectScale)
					{
						juiceElement3.transformElement.localScale = Vector3.Lerp(juiceElement3.originalLocalScale, this.desiredScale, this.nudgeProgress);
					}
					if (this.nudgeEffectRotation)
					{
						float num2 = Mathf.SmoothStep(juiceElement3.originalLocalRot.z, juiceElement3.originalLocalRot.z + this.amountToRotate, this.nudgeProgress);
						juiceElement3.transformElement.localEulerAngles = new Vector3(juiceElement3.transformElement.localEulerAngles.x, juiceElement3.transformElement.localEulerAngles.y, num2);
					}
				}
			}
		}
		if (this.fancyAppearActive)
		{
			this.fancyAppearProgress += Time.deltaTime * this.appearSpeed;
			this.fancyAppearProgress = Mathf.Clamp01(this.fancyAppearProgress);
			foreach (JuiceController.JuiceElement juiceElement4 in this.elements)
			{
				if (juiceElement4.imageElement != null)
				{
					juiceElement4.imageElement.canvasRenderer.SetAlpha(this.fancyAppearProgress);
				}
				if (juiceElement4.rawImageElement != null)
				{
					juiceElement4.rawImageElement.canvasRenderer.SetAlpha(this.fancyAppearProgress);
				}
				if (juiceElement4.renderer != null)
				{
					juiceElement4.renderer.SetAlpha(this.fancyAppearProgress);
				}
			}
			if (this.fancyAppearProgress >= 1f)
			{
				this.fancyAppearActive = false;
			}
		}
		if (this.fancyDisappearActive)
		{
			this.fancyDisappearProgress += Time.deltaTime * this.disappearSpeed;
			this.fancyDisappearProgress = Mathf.Clamp01(this.fancyDisappearProgress);
			foreach (JuiceController.JuiceElement juiceElement5 in this.elements)
			{
				if (juiceElement5.imageElement != null)
				{
					juiceElement5.imageElement.canvasRenderer.SetAlpha(this.fancyDisappearProgress);
				}
				if (juiceElement5.rawImageElement != null)
				{
					juiceElement5.rawImageElement.canvasRenderer.SetAlpha(this.fancyDisappearProgress);
				}
				if (juiceElement5.renderer != null)
				{
					juiceElement5.renderer.SetAlpha(this.fancyDisappearProgress);
				}
			}
			if (this.fancyDisappearProgress >= 1f)
			{
				this.fancyDisappearActive = false;
				base.gameObject.SetActive(false);
			}
		}
		if (!this.flashActive && !this.pulsateActive && !this.nudgeActive && !this.fancyAppearActive && !this.fancyDisappearActive)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x0019668C File Offset: 0x0019488C
	public void Flash(int newRepeat, bool colourOverride, Color colour = default(Color), float speed = 10f)
	{
		if (colourOverride)
		{
			this.flashColour = colour;
		}
		this.flashSpeed = speed;
		this.flashRepeat = newRepeat;
		if (this.flashActive)
		{
			this.flashRepeat += this.flashRepeat;
			return;
		}
		this.flashActive = true;
		this.cycle = 0;
		this.flashProgress = 0f;
		this.flashF = 0f;
		base.enabled = true;
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x001966FC File Offset: 0x001948FC
	public void Pulsate(bool toggle, bool smoothOff = false)
	{
		if (toggle)
		{
			smoothOff = false;
			this.smoothPulsateOff = false;
		}
		if (toggle && !this.pulsateActive)
		{
			base.enabled = true;
			this.pulsateProgress = 0f;
		}
		this.pulsateActive = toggle;
		if (smoothOff && !toggle)
		{
			base.enabled = true;
			this.smoothPulsateOff = true;
			this.pulsateActive = true;
		}
		if (!smoothOff && !toggle)
		{
			for (int i = 0; i < this.elements.Count; i++)
			{
				JuiceController.JuiceElement juiceElement = this.elements[i];
				if (juiceElement.imageElement != null)
				{
					juiceElement.imageElement.color = juiceElement.originalColour;
				}
				if (juiceElement.rawImageElement != null)
				{
					juiceElement.rawImageElement.color = juiceElement.originalColour;
				}
			}
		}
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x001967BC File Offset: 0x001949BC
	public void Nudge(Vector2 scaleRange, Vector2 rotationRange, bool updateOriginalPositionFirst = true, bool affectScale = true, bool affectRotation = true)
	{
		if (!this.nudgeActive)
		{
			if (updateOriginalPositionFirst)
			{
				foreach (JuiceController.JuiceElement juiceElement in this.elements)
				{
					if (juiceElement.transformElement != null)
					{
						juiceElement.originalLocalPos = juiceElement.transformElement.localPosition;
						juiceElement.originalLocalRot = juiceElement.transformElement.localEulerAngles;
						juiceElement.originalLocalScale = juiceElement.transformElement.localScale;
					}
				}
			}
			this.amountToScale = Toolbox.Instance.Rand(scaleRange.x, scaleRange.y, false);
			this.desiredScale = new Vector3(this.amountToScale, this.amountToScale, this.amountToScale);
			this.amountToRotate = Toolbox.Instance.Rand(rotationRange.x, rotationRange.y, false);
			this.nudgeProgress = 0f;
			this.nudgeActive = true;
			this.nudgeState = false;
			this.nudgeEffectScale = affectScale;
			this.nudgeEffectRotation = affectRotation;
			base.enabled = true;
		}
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x001968E0 File Offset: 0x00194AE0
	public void FancyAppear(float newAppearSpeed = 2f)
	{
		this.fancyAppearActive = true;
		this.fancyDisappearActive = false;
		this.fancyDisappearProgress = 0f;
		this.fancyAppearProgress = 0f;
		this.appearSpeed = newAppearSpeed;
		foreach (JuiceController.JuiceElement juiceElement in this.elements)
		{
			if (juiceElement.imageElement != null)
			{
				juiceElement.imageElement.canvasRenderer.SetAlpha(0f);
			}
			if (juiceElement.rawImageElement != null)
			{
				juiceElement.rawImageElement.canvasRenderer.SetAlpha(0f);
			}
		}
		this.Nudge(new Vector2(1.5f, 1.5f), Vector2.zero, true, true, false);
		base.gameObject.SetActive(true);
		base.enabled = true;
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x001969D0 File Offset: 0x00194BD0
	public void FancyDisappear(float newDisappearSpeed = 2f)
	{
		this.fancyDisappearActive = true;
		this.fancyAppearActive = false;
		this.fancyAppearProgress = 0f;
		this.fancyDisappearProgress = 0f;
		this.disappearSpeed = newDisappearSpeed;
		base.gameObject.SetActive(true);
		base.enabled = true;
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x00196A10 File Offset: 0x00194C10
	private void OnDisable()
	{
		this.flashActive = false;
		this.nudgeActive = false;
		for (int i = 0; i < this.elements.Count; i++)
		{
			JuiceController.JuiceElement juiceElement = this.elements[i];
			if (juiceElement.originalLocalScale.x > 0f)
			{
				if (juiceElement.imageElement != null)
				{
					juiceElement.imageElement.color = juiceElement.originalColour;
				}
				if (juiceElement.rawImageElement != null)
				{
					juiceElement.rawImageElement.color = juiceElement.originalColour;
				}
				if (juiceElement.transformElement != null)
				{
					juiceElement.transformElement.localScale = juiceElement.originalLocalScale;
					juiceElement.transformElement.localEulerAngles = juiceElement.originalLocalRot;
					juiceElement.transformElement.localPosition = juiceElement.originalLocalPos;
				}
			}
		}
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x00196AE8 File Offset: 0x00194CE8
	[Button(null, 0)]
	public void Flash()
	{
		this.Flash(3, false, default(Color), 10f);
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x00196B0B File Offset: 0x00194D0B
	[Button(null, 0)]
	public void PulsateToggle()
	{
		if (this.pulsateActive)
		{
			this.Pulsate(false, false);
			return;
		}
		this.Pulsate(true, false);
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x00196B26 File Offset: 0x00194D26
	[Button(null, 0)]
	public void Nudge()
	{
		this.Nudge(new Vector2(1.2f, 1.3f), new Vector2(0f, 0f), true, true, true);
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x00196B4F File Offset: 0x00194D4F
	[Button(null, 0)]
	public void Appear()
	{
		this.FancyAppear(2f);
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x00196B5C File Offset: 0x00194D5C
	[Button(null, 0)]
	public void Disappear()
	{
		this.FancyDisappear(2f);
	}

	// Token: 0x040025B2 RID: 9650
	[Header("Elements")]
	[ReorderableList]
	public List<JuiceController.JuiceElement> elements = new List<JuiceController.JuiceElement>();

	// Token: 0x040025B3 RID: 9651
	[Header("On Start")]
	public bool pulsateActive;

	// Token: 0x040025B4 RID: 9652
	public bool pulsateScale;

	// Token: 0x040025B5 RID: 9653
	public float pulsateProgress;

	// Token: 0x040025B6 RID: 9654
	public bool pulsateOnStart;

	// Token: 0x040025B7 RID: 9655
	public Color pulsateColour = Color.white;

	// Token: 0x040025B8 RID: 9656
	public float pulsateSpeed = 5f;

	// Token: 0x040025B9 RID: 9657
	private bool flashActive;

	// Token: 0x040025BA RID: 9658
	private float flashSpeed = 10f;

	// Token: 0x040025BB RID: 9659
	public Color flashColour;

	// Token: 0x040025BC RID: 9660
	private int cycle;

	// Token: 0x040025BD RID: 9661
	private float flashProgress;

	// Token: 0x040025BE RID: 9662
	private float flashF;

	// Token: 0x040025BF RID: 9663
	private int flashRepeat = 3;

	// Token: 0x040025C0 RID: 9664
	private bool onOff;

	// Token: 0x040025C1 RID: 9665
	public bool smoothPulsateOff;

	// Token: 0x040025C2 RID: 9666
	private bool nudgeActive;

	// Token: 0x040025C3 RID: 9667
	private bool nudgeState;

	// Token: 0x040025C4 RID: 9668
	private float nudgeProgress;

	// Token: 0x040025C5 RID: 9669
	private float amountToScale;

	// Token: 0x040025C6 RID: 9670
	private Vector3 desiredScale;

	// Token: 0x040025C7 RID: 9671
	private float amountToRotate;

	// Token: 0x040025C8 RID: 9672
	private bool nudgeEffectScale = true;

	// Token: 0x040025C9 RID: 9673
	private bool nudgeEffectRotation = true;

	// Token: 0x040025CA RID: 9674
	public bool fancyAppearActive;

	// Token: 0x040025CB RID: 9675
	public float appearSpeed = 2f;

	// Token: 0x040025CC RID: 9676
	private float fancyAppearProgress;

	// Token: 0x040025CD RID: 9677
	public bool fancyDisappearActive;

	// Token: 0x040025CE RID: 9678
	public float disappearSpeed = 2f;

	// Token: 0x040025CF RID: 9679
	private float fancyDisappearProgress;

	// Token: 0x02000521 RID: 1313
	[Serializable]
	public class JuiceElement
	{
		// Token: 0x040025D0 RID: 9680
		public RectTransform transformElement;

		// Token: 0x040025D1 RID: 9681
		public Image imageElement;

		// Token: 0x040025D2 RID: 9682
		public RawImage rawImageElement;

		// Token: 0x040025D3 RID: 9683
		public CanvasRenderer renderer;

		// Token: 0x040025D4 RID: 9684
		public Color originalColour = Color.white;

		// Token: 0x040025D5 RID: 9685
		[Tooltip("Get the original colours of images and raw images at the start")]
		public bool getNormalColourAtStart;

		// Token: 0x040025D6 RID: 9686
		public Vector3 originalLocalPos;

		// Token: 0x040025D7 RID: 9687
		public Vector3 originalLocalRot;

		// Token: 0x040025D8 RID: 9688
		public Vector3 originalLocalScale;

		// Token: 0x040025D9 RID: 9689
		public bool getNormalTransformAtStart;
	}
}
