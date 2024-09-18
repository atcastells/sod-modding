using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002DB RID: 731
public class ControlDisplayController : MonoBehaviour
{
	// Token: 0x0600109C RID: 4252 RVA: 0x000EB59C File Offset: 0x000E979C
	public bool UpdateDisplay(InteractablePreset.InteractionKey newKey, InteractionController.InteractionSetting newAction)
	{
		this.key = newKey;
		this.interactionSetting = newAction;
		this.interactionSetting.newUIRef = this;
		bool useContext = false;
		if (FirstPersonItemController.Instance.currentItem != null && FirstPersonItemController.Instance.currentItem != GameplayControls.Instance.nothingItem)
		{
			useContext = true;
		}
		bool result = this.SetControlText(this.key, this.interactionSetting.actionText, useContext);
		if (this.interactionSetting.interactable != null && this.interactionSetting.currentAction != null)
		{
			if (this.interactionSetting.interactable.highlightActions.Contains(this.interactionSetting.currentAction))
			{
				this.juiceController.Pulsate(true, false);
			}
			else
			{
				this.juiceController.Pulsate(false, false);
			}
		}
		if (this.interactionSetting.currentSetting != null)
		{
			this.juiceController.Pulsate(this.interactionSetting.currentSetting.highlight, false);
		}
		return result;
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x000EB690 File Offset: 0x000E9890
	private void Update()
	{
		if (this.fadeIn < 1f && !this.remove)
		{
			this.fadeIn += Time.deltaTime / 0.1f;
			this.fadeIn = Mathf.Clamp01(this.fadeIn);
			foreach (CanvasRenderer canvasRenderer in this.renderers)
			{
				if (!(canvasRenderer == null))
				{
					canvasRenderer.SetAlpha(this.fadeIn);
				}
			}
			if (this.fadeIn >= 1f && !this.execute)
			{
				base.enabled = false;
			}
		}
		else if (this.remove)
		{
			this.fadeIn -= Time.deltaTime / 0.1f;
			this.fadeIn = Mathf.Clamp01(this.fadeIn);
			foreach (CanvasRenderer canvasRenderer2 in this.renderers)
			{
				if (!(canvasRenderer2 == null))
				{
					canvasRenderer2.SetAlpha(this.fadeIn);
				}
			}
			this.rect.anchoredPosition = Vector2.Lerp(this.rect.anchoredPosition, this.spawnPosition, Time.deltaTime / 0.1f);
			if (this.fadeIn <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
		if (this.execute)
		{
			this.executeProgress += Time.deltaTime / ControlsDisplayController.Instance.animationSelectTime;
			this.executeProgress = Mathf.Clamp01(this.executeProgress);
			float num = ControlsDisplayController.Instance.controlSelectAnimation.Evaluate(this.executeProgress);
			float num2 = 1f + num * ControlsDisplayController.Instance.controlSelectScaleLerp;
			this.rect.localScale = new Vector3(num2, num2, num2);
			this.background.color = Color.Lerp(Color.black, ControlsDisplayController.Instance.controlSelectColorLerp, num);
			if (this.executeProgress >= 1f)
			{
				this.execute = false;
				this.executeProgress = 0f;
			}
		}
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x000EB8DC File Offset: 0x000E9ADC
	private void OnEnable()
	{
		InputController.Instance.OnInputModeChange += this.RefreshIcon;
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x000EB8F4 File Offset: 0x000E9AF4
	private void OnDisable()
	{
		InputController.Instance.OnInputModeChange -= this.RefreshIcon;
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x000EB90C File Offset: 0x000E9B0C
	public void RefreshIcon()
	{
		bool useContext = false;
		if (FirstPersonItemController.Instance.currentItem != null && FirstPersonItemController.Instance.currentItem != GameplayControls.Instance.nothingItem)
		{
			useContext = true;
		}
		this.SetControlText(this.key, this.interactionSetting.actionText, useContext);
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x000EB964 File Offset: 0x000E9B64
	public void Remove()
	{
		this.remove = true;
		ControlsDisplayController.Instance.spawned.Remove(this);
		try
		{
			base.enabled = true;
		}
		catch
		{
		}
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x000EB9A8 File Offset: 0x000E9BA8
	public void Execute()
	{
		if (!this.execute)
		{
			this.juiceController.Pulsate(false, false);
			this.execute = true;
			this.executeProgress = 0f;
			try
			{
				base.enabled = true;
			}
			catch
			{
			}
		}
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x000EB9F8 File Offset: 0x000E9BF8
	public bool SetControlText(InteractablePreset.InteractionKey key, string newText, bool useContext = false)
	{
		bool result = true;
		string text = string.Empty;
		if (useContext)
		{
			string text2 = string.Empty;
			if (this.interactionSetting.isFPSItem && FirstPersonItemController.Instance.currentItem != null)
			{
				if (!FirstPersonItemController.Instance.currentItem.disableBracketDisplayName)
				{
					Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
					if (interactable != null)
					{
						text2 = interactable.GetName();
					}
					else
					{
						FirstPersonItem firstPersonItem = BioScreenController.Instance.selectedSlot.GetFirstPersonItem();
						if (firstPersonItem != null)
						{
							text2 = Strings.Get("ui.interface", firstPersonItem.name, Strings.Casing.firstLetterCaptial, false, false, false, null);
						}
					}
				}
			}
			else if (this.interactionSetting.interactable != null)
			{
				text2 = this.interactionSetting.interactable.GetName();
			}
			if (text2.Length > 0)
			{
				text = " [" + text2 + "]";
			}
		}
		string text3 = string.Empty;
		int actionCost = this.interactionSetting.GetActionCost();
		if (actionCost != 0)
		{
			text3 = " [" + CityControls.Instance.cityCurrency + actionCost.ToString() + "]";
		}
		this.controlText.SetText(string.Concat(new string[]
		{
			ControlsDisplayController.Instance.GetControlIcon(key, out this.positioning, out result),
			" ",
			newText,
			text3,
			text
		}), true);
		base.name = this.controlText.text;
		this.controlText.ForceMeshUpdate(false, false);
		this.renderers.Clear();
		this.renderers.AddRange(base.gameObject.GetComponentsInChildren<CanvasRenderer>());
		if (this.interactionSetting.currentSetting.forcePositioning)
		{
			this.positioning = this.interactionSetting.currentSetting.forcePosition;
		}
		return result;
	}

	// Token: 0x040014B3 RID: 5299
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x040014B4 RID: 5300
	public TextMeshProUGUI controlText;

	// Token: 0x040014B5 RID: 5301
	public List<CanvasRenderer> renderers = new List<CanvasRenderer>();

	// Token: 0x040014B6 RID: 5302
	public Image background;

	// Token: 0x040014B7 RID: 5303
	public SoundIndicatorController soundIndicator;

	// Token: 0x040014B8 RID: 5304
	public JuiceController juiceController;

	// Token: 0x040014B9 RID: 5305
	[Header("State")]
	public InteractablePreset.InteractionKey key;

	// Token: 0x040014BA RID: 5306
	public InteractionController.InteractionSetting interactionSetting;

	// Token: 0x040014BB RID: 5307
	public float fadeIn;

	// Token: 0x040014BC RID: 5308
	public bool remove;

	// Token: 0x040014BD RID: 5309
	public ControlDisplayController.ControlPositioning positioning;

	// Token: 0x040014BE RID: 5310
	public Vector2 desiredPosition = Vector2.zero;

	// Token: 0x040014BF RID: 5311
	public Vector2 spawnPosition = Vector2.zero;

	// Token: 0x040014C0 RID: 5312
	public bool assignedSpawnPosition;

	// Token: 0x040014C1 RID: 5313
	public bool execute;

	// Token: 0x040014C2 RID: 5314
	public float executeProgress;

	// Token: 0x040014C3 RID: 5315
	[Header("Debug")]
	public string actionName = "Jump";

	// Token: 0x020002DC RID: 732
	public enum ControlPositioning
	{
		// Token: 0x040014C5 RID: 5317
		neutral,
		// Token: 0x040014C6 RID: 5318
		up,
		// Token: 0x040014C7 RID: 5319
		down,
		// Token: 0x040014C8 RID: 5320
		left,
		// Token: 0x040014C9 RID: 5321
		right
	}
}
