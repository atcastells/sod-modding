using System;
using System.Collections.Generic;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020005D0 RID: 1488
public class StatusController : MonoBehaviour
{
	// Token: 0x170000FC RID: 252
	// (get) Token: 0x060020AA RID: 8362 RVA: 0x001BF14F File Offset: 0x001BD34F
	public static StatusController Instance
	{
		get
		{
			return StatusController._instance;
		}
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x001BF156 File Offset: 0x001BD356
	private void Awake()
	{
		if (StatusController._instance != null && StatusController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		StatusController._instance = this;
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x001BF184 File Offset: 0x001BD384
	private void Start()
	{
		Toolbox.Instance.allStatuses.Sort((StatusPreset p2, StatusPreset p1) => p1.priority.CompareTo(p2.priority));
		foreach (StatusPreset statusPreset in Toolbox.Instance.allStatuses)
		{
			if (statusPreset.useCustomMethod)
			{
				MethodInfo method = base.GetType().GetMethod(statusPreset.name);
				if (method != null)
				{
					this.checkingRef.Add(statusPreset, method);
				}
			}
		}
		BioScreenController.Instance.OnInventoryOpenChange += this.DisplayCheck;
		this.DisplayCheck();
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x001BF250 File Offset: 0x001BD450
	private void DisplayCheck()
	{
		if (BioScreenController.Instance.isOpen)
		{
			using (List<StateElementController>.Enumerator enumerator = this.spawnedControllers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StateElementController stateElementController = enumerator.Current;
					stateElementController.SetMinimized(false);
					stateElementController.SetMaximized(true);
				}
				return;
			}
		}
		foreach (StateElementController stateElementController2 in this.spawnedControllers)
		{
			stateElementController2.SetMaximized(false);
		}
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x001BF2F4 File Offset: 0x001BD4F4
	public void RemoveAllCounts(StatusController.StatusInstance inst)
	{
		if (this.activeStatusCounts.ContainsKey(inst))
		{
			if (this.activeStatusCounts[inst].Count > 0 && inst.preset.onRemove != null && SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
			{
				AudioController.Instance.Play2DSound(inst.preset.onRemove, null, 1f);
			}
			while (this.activeStatusCounts[inst].Count > 0)
			{
				this.activeStatusCounts[inst][0].Remove();
			}
		}
		if (inst.preset.maxHealthPlusMP != 0f)
		{
			Player.Instance.AddHealth(0f, true, false);
		}
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x001BF3CC File Offset: 0x001BD5CC
	public void RemoveAllCounts(StatusPreset preset)
	{
		List<StatusController.StatusInstance> list = new List<StatusController.StatusInstance>();
		foreach (KeyValuePair<StatusController.StatusInstance, List<StatusController.StatusCount>> keyValuePair in this.activeStatusCounts)
		{
			if (keyValuePair.Key.preset == preset)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (StatusController.StatusInstance statusInstance in list)
		{
			this.activeStatusCounts.Remove(statusInstance);
			if (SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
			{
				AudioController.Instance.Play2DSound(preset.onRemove, null, 1f);
			}
		}
		if (preset.maxHealthPlusMP != 0f)
		{
			Player.Instance.AddHealth(0f, true, false);
		}
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x001BF4E0 File Offset: 0x001BD6E0
	public void ForceStatusCheck()
	{
		this.ForceStatusCheck(false);
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x001BF4EC File Offset: 0x001BD6EC
	public void ForceStatusCheck(bool bypassKOCheck = false)
	{
		if (CutSceneController.Instance.cutSceneActive)
		{
			return;
		}
		foreach (KeyValuePair<StatusPreset, MethodInfo> keyValuePair in this.checkingRef)
		{
			object[] array = new object[]
			{
				new StatusController.StatusInstance
				{
					preset = keyValuePair.Key
				}
			};
			keyValuePair.Value.Invoke(this, array);
		}
		List<StatusController.StatusInstance> list = new List<StatusController.StatusInstance>();
		this.disabledRecovery = false;
		this.disabledJump = false;
		this.disabledSprint = false;
		this.recoveryRateMultiplier = 1f;
		this.maxHealthMultiplier = 1f;
		this.movementSpeedMultiplier = 1f;
		this.temperatureGainMultiplier = 1f;
		this.damageIncomingMultiplier = 1f;
		this.damageOutgoingMultiplier = 1f;
		this.tripChanceWet = 0f;
		this.tripChanceDrunk = 0f;
		this.drunkControls = 0f;
		this.affectHeadBobs.Clear();
		this.drunkVision = 0f;
		this.shiverVision = 0f;
		this.drunkLensDistort = 0f;
		this.headacheVision = 0f;
		this.bloomIntensityMultiplier = 0f;
		this.motionBlurMultiplier = 0f;
		this.chromaticAbberationAmount = 0f;
		this.vignetteAmount = 0f;
		this.exposureAmount = 0f;
		Dictionary<StatusPreset, float> dictionary = new Dictionary<StatusPreset, float>();
		bool flag = false;
		this.channelRedR = 100f;
		this.channelRedG = 0f;
		this.channelRedB = 0f;
		this.channelGreenR = 0f;
		this.channelGreenG = 100f;
		this.channelGreenB = 0f;
		this.channelBlueR = 0f;
		this.channelBlueG = 0f;
		this.channelBlueB = 100f;
		foreach (KeyValuePair<StatusController.StatusInstance, List<StatusController.StatusCount>> keyValuePair2 in this.activeStatusCounts)
		{
			if (keyValuePair2.Value != null && keyValuePair2.Value.Count > 0)
			{
				list.Add(keyValuePair2.Key);
				if (keyValuePair2.Key.preset.stopsRecovery)
				{
					this.disabledRecovery = true;
				}
				if (keyValuePair2.Key.preset.stopsJump)
				{
					this.disabledJump = true;
				}
				if (keyValuePair2.Key.preset.stopsSprint)
				{
					this.disabledSprint = true;
				}
				this.recoveryRateMultiplier += keyValuePair2.Key.preset.recoveryRatePlusMP * keyValuePair2.Value[0].amount;
				this.maxHealthMultiplier += keyValuePair2.Key.preset.maxHealthPlusMP * keyValuePair2.Value[0].amount;
				this.movementSpeedMultiplier += keyValuePair2.Key.preset.movementSpeedPlusMP * keyValuePair2.Value[0].amount;
				this.temperatureGainMultiplier += keyValuePair2.Key.preset.temperatureGainPlusMP * keyValuePair2.Value[0].amount;
				this.damageIncomingMultiplier += keyValuePair2.Key.preset.damageIncomingPlusMP * keyValuePair2.Value[0].amount;
				this.damageOutgoingMultiplier += keyValuePair2.Key.preset.damageOutgoingPlusMP * keyValuePair2.Value[0].amount;
				this.tripChanceWet += keyValuePair2.Key.preset.tripChanceWet * keyValuePair2.Value[0].amount;
				this.tripChanceDrunk += keyValuePair2.Key.preset.tripChanceDrunk * keyValuePair2.Value[0].amount;
				this.drunkControls += keyValuePair2.Key.preset.drunkControls * keyValuePair2.Value[0].amount;
				if (keyValuePair2.Key.preset.affectHeadBob != 0f)
				{
					this.affectHeadBobs.Add(keyValuePair2.Key.preset.headBob, keyValuePair2.Value[0].amount);
				}
				this.drunkVision += keyValuePair2.Key.preset.drunkVision * keyValuePair2.Value[0].amount;
				this.shiverVision += keyValuePair2.Key.preset.shiverVision * keyValuePair2.Value[0].amount;
				this.drunkLensDistort += keyValuePair2.Key.preset.drunkLensDistort * keyValuePair2.Value[0].amount;
				this.headacheVision += keyValuePair2.Key.preset.headacheVision * keyValuePair2.Value[0].amount;
				this.bloomIntensityMultiplier += keyValuePair2.Key.preset.bloomIntensityPlusMP * keyValuePair2.Value[0].amount;
				this.motionBlurMultiplier += keyValuePair2.Key.preset.motionBlurPlusMP * keyValuePair2.Value[0].amount;
				this.chromaticAbberationAmount += keyValuePair2.Key.preset.chromaticAbberationAmount * keyValuePair2.Value[0].amount;
				this.vignetteAmount += keyValuePair2.Key.preset.vignetteAmount * keyValuePair2.Value[0].amount;
				this.exposureAmount += keyValuePair2.Key.preset.expsosure * keyValuePair2.Value[0].amount;
				if (keyValuePair2.Key.preset.useChannelMixer)
				{
					flag = true;
					if (!dictionary.ContainsKey(keyValuePair2.Key.preset))
					{
						dictionary.Add(keyValuePair2.Key.preset, keyValuePair2.Value[0].amount);
					}
					else
					{
						Dictionary<StatusPreset, float> dictionary2 = dictionary;
						StatusPreset preset = keyValuePair2.Key.preset;
						dictionary2[preset] += keyValuePair2.Value[0].amount;
					}
				}
				this.maxHealthMultiplier = Mathf.Max(this.maxHealthMultiplier, 0.1f);
				this.movementSpeedMultiplier = Mathf.Max(this.movementSpeedMultiplier, 0.1f);
				this.drunkControls = Mathf.Clamp01(this.drunkControls);
				this.shiverVision = Mathf.Clamp01(this.shiverVision);
				SessionData.Instance.motionBlur.intensity.value = Game.Instance.motionBlurIntensity * this.motionBlurMultiplier + SessionData.Instance.GetGameSpeedMotionBlurModifier();
				SessionData.Instance.bloom.intensity.value = Game.Instance.bloomIntensity * this.bloomIntensityMultiplier;
				if (this.exposureAmount != 0f)
				{
					SessionData.Instance.exposure.active = true;
					SessionData.Instance.exposure.fixedExposure.value = Mathf.Min(this.exposureAmount, -Player.Instance.hurt * 0.5f);
				}
				else if (SessionData.Instance.exposure.IsActive() && Player.Instance.hurt <= 0f)
				{
					SessionData.Instance.exposure.active = false;
				}
				if (!Player.Instance.transitionActive || (Player.Instance.currentTransition != null && !Player.Instance.currentTransition.useChromaticAberration))
				{
					if (this.chromaticAbberationAmount > 0f)
					{
						SessionData.Instance.chromaticAberration.active = true;
						SessionData.Instance.chromaticAberration.intensity.value = this.chromaticAbberationAmount;
					}
					else if (SessionData.Instance.chromaticAberration.IsActive())
					{
						SessionData.Instance.chromaticAberration.active = false;
					}
				}
				if (InterfaceController.Instance.desktopModeTransition <= 0f && InterfaceController.Instance.desktopModeDesiredTransition <= 0f)
				{
					if (this.vignetteAmount > 0f)
					{
						SessionData.Instance.vignette.active = true;
						SessionData.Instance.vignette.intensity.value = Mathf.Max(this.vignetteAmount, InterfaceController.Instance.fade);
					}
					else if (SessionData.Instance.vignette.IsActive())
					{
						SessionData.Instance.vignette.active = false;
					}
				}
			}
		}
		if (flag)
		{
			SessionData.Instance.channelMixer.active = true;
			foreach (KeyValuePair<StatusPreset, float> keyValuePair3 in dictionary)
			{
				this.channelRedR += (float)keyValuePair3.Key.redR * keyValuePair3.Value;
				this.channelRedG += (float)keyValuePair3.Key.redG * keyValuePair3.Value;
				this.channelRedB += (float)keyValuePair3.Key.redB * keyValuePair3.Value;
				this.channelGreenR += (float)keyValuePair3.Key.greenR * keyValuePair3.Value;
				this.channelGreenG += (float)keyValuePair3.Key.greenG * keyValuePair3.Value;
				this.channelGreenB += (float)keyValuePair3.Key.greenB * keyValuePair3.Value;
				this.channelBlueR += (float)keyValuePair3.Key.blueR * keyValuePair3.Value;
				this.channelBlueG += (float)keyValuePair3.Key.blueG * keyValuePair3.Value;
				this.channelBlueB += (float)keyValuePair3.Key.blueB * keyValuePair3.Value;
			}
			SessionData.Instance.channelMixer.redOutRedIn.value = this.channelRedR;
			SessionData.Instance.channelMixer.redOutGreenIn.value = this.channelRedG;
			SessionData.Instance.channelMixer.redOutBlueIn.value = this.channelRedB;
			SessionData.Instance.channelMixer.greenOutRedIn.value = this.channelGreenR;
			SessionData.Instance.channelMixer.greenOutGreenIn.value = this.channelGreenG;
			SessionData.Instance.channelMixer.greenOutBlueIn.value = this.channelGreenB;
			SessionData.Instance.channelMixer.blueOutRedIn.value = this.channelBlueR;
			SessionData.Instance.channelMixer.blueOutGreenIn.value = this.channelBlueG;
			SessionData.Instance.channelMixer.blueOutBlueIn.value = this.channelBlueB;
		}
		else
		{
			SessionData.Instance.channelMixer.active = false;
		}
		for (int i = 0; i < this.spawnedControllers.Count; i++)
		{
			StateElementController stateElementController = this.spawnedControllers[i];
			if (list.Contains(stateElementController.statusInstance))
			{
				stateElementController.VisualUpdate();
				list.Remove(stateElementController.statusInstance);
				if (stateElementController.removing)
				{
					stateElementController.SetRemove(false);
				}
				base.enabled = true;
			}
			else
			{
				stateElementController.SetRemove(true);
			}
		}
		foreach (StatusController.StatusInstance statusInstance in list)
		{
			StateElementController component = Object.Instantiate<GameObject>(PrefabControls.Instance.statusElement, this.statusParent).GetComponent<StateElementController>();
			component.Setup(statusInstance);
			this.spawnedControllers.Add(component);
			base.enabled = true;
			SessionData.Instance.TutorialTrigger("statuses", false);
			if (statusInstance.preset.autoNotificationMessage)
			{
				string text = Strings.Get("ui.states", statusInstance.preset.name, Strings.Casing.asIs, false, false, false, null) + ": " + Strings.Get("ui.states", statusInstance.preset.name + "_description", Strings.Casing.asIs, false, false, false, null);
				InterfaceController instance = InterfaceController.Instance;
				InterfaceController.GameMessageType newType = InterfaceController.GameMessageType.notification;
				int newNumerical = 0;
				string newMessage = text;
				InterfaceControls.Icon newIcon = InterfaceControls.Icon.agent;
				AudioEvent additionalSFX = null;
				bool colourOverride = true;
				Sprite icon = statusInstance.preset.icon;
				instance.NewGameMessage(newType, newNumerical, newMessage, newIcon, additionalSFX, colourOverride, statusInstance.preset.color, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, icon);
			}
		}
		this.spawnedControllers.Sort((StateElementController p2, StateElementController p1) => p1.preset.priority.CompareTo(p2.preset.priority));
		float num = 0f;
		for (int j = 0; j < this.spawnedControllers.Count; j++)
		{
			StateElementController stateElementController2 = this.spawnedControllers[j];
			stateElementController2.rect.localPosition = new Vector3(0f, num, 0f);
			if (list.Contains(stateElementController2.statusInstance))
			{
				stateElementController2.juice.Nudge(new Vector2(1.5f, 1.5f), new Vector2(8f, 8f), true, true, true);
				stateElementController2.juice.Flash(2, false, default(Color), 10f);
			}
			num -= stateElementController2.rect.sizeDelta.y + this.elementYInterval;
		}
		for (int k = 0; k < FirstPersonItemController.Instance.slots.Count; k++)
		{
			if (FirstPersonItemController.Instance.slots[k] != null && FirstPersonItemController.Instance.slots[k].spawnedSegment != null)
			{
				FirstPersonItemController.Instance.slots[k].spawnedSegment.OnUpdateContent();
			}
		}
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x001C039C File Offset: 0x001BE59C
	private void Update()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		bool flag = false;
		float num = 0f;
		for (int i = 0; i < this.spawnedControllers.Count; i++)
		{
			StateElementController stateElementController = this.spawnedControllers[i];
			if (SessionData.Instance.play && !BioScreenController.Instance.isOpen)
			{
				if (stateElementController.preset.minimizeToIcon && !stateElementController.isWanted)
				{
					if (stateElementController.minimizeTimer > 0f)
					{
						stateElementController.minimizeTimer -= Time.deltaTime;
						if (stateElementController.minimized)
						{
							stateElementController.SetMinimized(false);
						}
					}
					else
					{
						stateElementController.SetMinimized(true);
					}
				}
				if (stateElementController.maximized && stateElementController.maximizeTimer > 0f)
				{
					stateElementController.maximizeTimer -= Time.deltaTime;
				}
				else
				{
					stateElementController.SetMaximized(false);
				}
			}
			if (stateElementController.minimized && stateElementController.widthResizingProgress > 0f)
			{
				stateElementController.widthResizingProgress -= Time.deltaTime * 6f;
				stateElementController.widthResizingProgress = Mathf.Clamp01(stateElementController.widthResizingProgress);
				stateElementController.rect.sizeDelta = new Vector2(Mathf.SmoothStep(this.elementMinimizedWidth, this.elementDefaultWdith, stateElementController.widthResizingProgress), stateElementController.rect.sizeDelta.y);
				stateElementController.mainText.canvasRenderer.SetAlpha(stateElementController.widthResizingProgress * 2f);
				if (stateElementController.preset.displayTotalFineWhenMinimized)
				{
					stateElementController.fineText.canvasRenderer.SetAlpha(1f - stateElementController.widthResizingProgress);
				}
			}
			else if (!stateElementController.minimized && stateElementController.widthResizingProgress < 1f)
			{
				stateElementController.widthResizingProgress += Time.deltaTime * 6f;
				stateElementController.widthResizingProgress = Mathf.Clamp01(stateElementController.widthResizingProgress);
				stateElementController.rect.sizeDelta = new Vector2(Mathf.SmoothStep(this.elementMinimizedWidth, this.elementDefaultWdith, stateElementController.widthResizingProgress), stateElementController.rect.sizeDelta.y);
				stateElementController.mainText.canvasRenderer.SetAlpha(stateElementController.widthResizingProgress * 2f);
				if (stateElementController.preset.displayTotalFineWhenMinimized)
				{
					stateElementController.fineText.canvasRenderer.SetAlpha(1f - stateElementController.widthResizingProgress);
				}
			}
			if (stateElementController.preset.displayTotalFineWhenMinimized && stateElementController.displayedFine != stateElementController.fineTotal)
			{
				if (stateElementController.displayedFine < stateElementController.fineTotal)
				{
					stateElementController.displayedFine++;
				}
				else if (stateElementController.displayedFine > stateElementController.fineTotal)
				{
					stateElementController.displayedFine--;
				}
				if (stateElementController.displayedFine + 10 < stateElementController.fineTotal)
				{
					stateElementController.displayedFine += 10;
				}
				else if (stateElementController.displayedFine - 10 > stateElementController.fineTotal)
				{
					stateElementController.displayedFine -= 10;
				}
				stateElementController.fineText.text = CityControls.Instance.cityCurrency + stateElementController.displayedFine.ToString();
			}
			else if (stateElementController.maximized && stateElementController.heightResizingProgress < 1f)
			{
				stateElementController.heightResizingProgress += Time.deltaTime * 6f;
				stateElementController.heightResizingProgress = Mathf.Clamp01(stateElementController.heightResizingProgress);
				stateElementController.rect.sizeDelta = new Vector2(stateElementController.rect.sizeDelta.x, Mathf.SmoothStep(this.elementDefaultHeight, stateElementController.maximizedHeight, stateElementController.heightResizingProgress));
				stateElementController.detailText.canvasRenderer.SetAlpha(this.detailTextFadeInCurve.Evaluate(stateElementController.heightResizingProgress));
			}
			else if (!stateElementController.maximized && stateElementController.heightResizingProgress > 0f)
			{
				stateElementController.heightResizingProgress -= Time.deltaTime * 6f;
				stateElementController.heightResizingProgress = Mathf.Clamp01(stateElementController.heightResizingProgress);
				stateElementController.rect.sizeDelta = new Vector2(stateElementController.rect.sizeDelta.x, Mathf.SmoothStep(this.elementDefaultHeight, stateElementController.maximizedHeight, stateElementController.heightResizingProgress));
				stateElementController.detailText.canvasRenderer.SetAlpha(this.detailTextFadeInCurve.Evaluate(stateElementController.heightResizingProgress));
			}
			if (stateElementController.preset.enableProgressBar)
			{
				float num2 = 0f;
				if (stateElementController.preset.barTracking == StatusPreset.ProgressBarTrack.witnesses)
				{
					if (stateElementController.isWanted)
					{
						num2 = stateElementController.statusInstance.building.wantedInBuilding / GameplayControls.Instance.buildingWantedTime;
					}
					else
					{
						num2 = Player.Instance.seenProgressLag;
					}
				}
				else if (stateElementController.preset.barTracking == StatusPreset.ProgressBarTrack.wantedInBuilding)
				{
					num2 = (stateElementController.statusInstance.building.wantedInBuilding - SessionData.Instance.gameTime) / GameplayControls.Instance.buildingWantedTime;
				}
				else if (stateElementController.preset.barTracking == StatusPreset.ProgressBarTrack.alarmTime)
				{
					if (Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.alarmActive)
					{
						num2 = Player.Instance.currentGameLocation.thisAsAddress.alarmTimer / Player.Instance.currentGameLocation.thisAsAddress.GetAlarmTime();
					}
					else if (Player.Instance.currentBuilding != null && Player.Instance.currentBuilding.alarmActive)
					{
						num2 = Player.Instance.currentBuilding.alarmTimer / Player.Instance.currentBuilding.GetAlarmTime();
					}
					else
					{
						num2 = 0f;
					}
				}
				else if (stateElementController.preset.barTracking == StatusPreset.ProgressBarTrack.guestPassTime)
				{
					Vector2 zero = Vector2.zero;
					if (GameplayController.Instance.guestPasses.TryGetValue(stateElementController.statusInstance.address, ref zero))
					{
						num2 = (zero.x - SessionData.Instance.gameTime) / zero.y;
					}
					else
					{
						num2 = 0f;
					}
				}
				stateElementController.progressBar.sizeDelta = new Vector2((stateElementController.rect.sizeDelta.x - 8f) * num2, stateElementController.progressBar.sizeDelta.y);
			}
			if (stateElementController.rect.localPosition.y != num)
			{
				stateElementController.rect.localPosition = new Vector3(0f, stateElementController.rect.localPosition.y + (num - stateElementController.rect.localPosition.y) / 3f, 0f);
			}
			num -= stateElementController.rect.sizeDelta.y + this.elementYInterval;
			this.statusParent.sizeDelta = new Vector2(this.statusParent.sizeDelta.x, -num);
			if (stateElementController.removing)
			{
				if (stateElementController.removalTimer > 0f)
				{
					stateElementController.removalTimer -= Time.deltaTime * 1.66f;
					foreach (CanvasRenderer canvasRenderer in stateElementController.renderElements)
					{
						canvasRenderer.SetAlpha(stateElementController.removalTimer);
					}
					if (stateElementController.xIcon != null)
					{
						float num3 = Mathf.SmoothStep(0f, 120f, 1f - stateElementController.removalTimer);
						stateElementController.xIcon.sizeDelta = new Vector2(num3, num3);
						stateElementController.xIconRend.SetAlpha(stateElementController.removalTimer);
						stateElementController.xIcon.anchoredPosition = Vector3.zero;
					}
				}
				else
				{
					this.spawnedControllers.RemoveAt(i);
					Object.Destroy(stateElementController.gameObject);
					flag = true;
					i--;
				}
			}
		}
		if (flag)
		{
			this.spawnedControllers.Sort((StateElementController p2, StateElementController p1) => p1.preset.priority.CompareTo(p2.preset.priority));
		}
		if (this.spawnedControllers.Count <= 0)
		{
			this.statusParent.sizeDelta = new Vector2(this.statusParent.sizeDelta.x, 0f);
			base.enabled = false;
		}
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x001C0C00 File Offset: 0x001BEE00
	public void AddFineRecord(NewAddress address, Interactable obj, StatusController.CrimeType crime, bool confirm = false, int forcedPenalty = -1, bool ignoreDuplicates = false)
	{
		StatusController.FineRecord fineRecord = this.activeFineRecords.Find((StatusController.FineRecord item) => !ignoreDuplicates && ((address == null && item.addressID == -1) || (address != null && item.addressID == address.id)) && item.crime == crime && ((item.objectID <= -1 && obj == null) || (obj != null && item.objectID == obj.id)));
		if (fineRecord != null)
		{
			string[] array = new string[8];
			array[0] = "Player: Existing fine detected ";
			array[1] = crime.ToString();
			array[2] = " ";
			int num = 3;
			Interactable obj2 = obj;
			array[num] = ((obj2 != null) ? obj2.ToString() : null);
			array[4] = " at ";
			int num2 = 5;
			NewAddress address2 = address;
			array[num2] = ((address2 != null) ? address2.ToString() : null);
			array[6] = "; confirm ";
			array[7] = confirm.ToString();
			Game.Log(string.Concat(array), 2);
			if (confirm)
			{
				fineRecord.SetConfirmed(true);
			}
		}
		else
		{
			string[] array2 = new string[7];
			array2[0] = "Player: No existing fine detected ";
			array2[1] = crime.ToString();
			array2[2] = " ";
			int num3 = 3;
			Interactable obj3 = obj;
			array2[num3] = ((obj3 != null) ? obj3.ToString() : null);
			array2[4] = " at ";
			int num4 = 5;
			NewAddress address3 = address;
			array2[num4] = ((address3 != null) ? address3.ToString() : null);
			array2[6] = "; setup new...";
			Game.Log(string.Concat(array2), 2);
			StatusController.FineRecord fineRecord2 = new StatusController.FineRecord(address, obj, crime);
			fineRecord2.SetConfirmed(confirm);
			if (forcedPenalty > -1)
			{
				fineRecord2.forcedPenalty = forcedPenalty;
			}
			this.activeFineRecords.Add(fineRecord2);
		}
		this.ForceStatusCheck();
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x001C0D88 File Offset: 0x001BEF88
	public void RemoveFineRecord(NewAddress address, Interactable obj, StatusController.CrimeType crime, bool onlyUnconfirmed = false, bool matchAddress = true)
	{
		if (onlyUnconfirmed)
		{
			this.activeFineRecords.RemoveAll((StatusController.FineRecord item) => !item.confirmed && (!matchAddress || (address == null && item.addressID == -1) || (address != null && item.addressID == address.id)) && item.crime == crime && ((item.objectID <= -1 && obj == null) || (obj != null && item.objectID == obj.id)));
		}
		else
		{
			this.activeFineRecords.RemoveAll((StatusController.FineRecord item) => (!matchAddress || (address == null && item.addressID == -1) || (address != null && item.addressID == address.id)) && item.crime == crime && ((item.objectID <= -1 && obj == null) || (obj != null && item.objectID == obj.id)));
		}
		this.ForceStatusCheck();
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x001C0DF4 File Offset: 0x001BEFF4
	public void FineEscapeCheck()
	{
		bool flag = false;
		for (int i = 0; i < this.activeFineRecords.Count; i++)
		{
			StatusController.FineRecord fineRecord = this.activeFineRecords[i];
			NewAddress newAddress = null;
			if (CityData.Instance.addressDictionary.TryGetValue(fineRecord.addressID, ref newAddress))
			{
				if (Player.Instance.currentBuilding != newAddress.building)
				{
					string[] array = new string[5];
					array[0] = "Player: Removing fine as the player is in a different building (";
					int num = 1;
					NewBuilding currentBuilding = Player.Instance.currentBuilding;
					array[num] = ((currentBuilding != null) ? currentBuilding.ToString() : null);
					array[2] = "/";
					int num2 = 3;
					NewBuilding building = newAddress.building;
					array[num2] = ((building != null) ? building.ToString() : null);
					array[4] = ")";
					Game.Log(string.Concat(array), 2);
					this.activeFineRecords.RemoveAt(i);
					flag = true;
					i--;
				}
			}
			else
			{
				Game.Log("Player: Removing fine as there is no address entry for " + fineRecord.addressID.ToString(), 2);
				this.activeFineRecords.RemoveAt(i);
				flag = true;
				i--;
			}
		}
		if (flag)
		{
			this.ForceStatusCheck();
		}
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x001C0F08 File Offset: 0x001BF108
	public void SetWantedInBuilding(NewBuilding b, float time)
	{
		b.wantedInBuilding = Mathf.Max(b.wantedInBuilding, SessionData.Instance.gameTime + time);
		Game.Log("Player: Set wanted in building " + b.name + ": " + b.wantedInBuilding.ToString(), 2);
		this.ForceStatusCheck();
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x001C0F60 File Offset: 0x001BF160
	public void SetDetainedInBuilding(NewBuilding b, bool val)
	{
		if (b == null)
		{
			return;
		}
		Game.Log("Set detained: " + b.name + " " + val.ToString(), 2);
		StatusController.StatusInstance statusInstance = default(StatusController.StatusInstance);
		statusInstance.preset = GameplayControls.Instance.detainedStatus;
		statusInstance.building = b;
		if (this.activeStatusCounts.ContainsKey(statusInstance) && this.activeStatusCounts[statusInstance].Count > 0)
		{
			if (!val)
			{
				this.RemoveAllCounts(statusInstance);
			}
			Game.Log("Detained: Existing status key", 2);
			return;
		}
		if (val)
		{
			InterfaceController.Instance.CrosshairReaction();
			new StatusController.StatusCount(statusInstance);
		}
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x001C1008 File Offset: 0x001BF208
	public bool GetCurrentDetainedStatus()
	{
		if (this.activeStatuses.Contains(GameplayControls.Instance.detainedStatus))
		{
			if (InteractionController.Instance.lockedInInteraction != null && InteractionController.Instance.lockedInInteraction.preset.specialCaseFlag == InteractablePreset.SpecialCase.hospitalBed)
			{
				return false;
			}
			foreach (KeyValuePair<StatusController.StatusInstance, List<StatusController.StatusCount>> keyValuePair in this.activeStatusCounts)
			{
				if (keyValuePair.Key.preset == GameplayControls.Instance.detainedStatus && keyValuePair.Key.building == Player.Instance.currentBuilding)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x001C10D8 File Offset: 0x001BF2D8
	public void ConfirmFinesAtLocation(NewAddress address, StatusController.CrimeType crime)
	{
		foreach (StatusController.FineRecord fineRecord in this.activeFineRecords.FindAll((StatusController.FineRecord item) => !item.confirmed && ((address == null && item.addressID == -1) || (address != null && item.addressID == address.id)) && item.crime == crime))
		{
			fineRecord.SetConfirmed(true);
			Player.Instance.StatusCheckEndOfFrame();
		}
	}

	// Token: 0x060020BA RID: 8378 RVA: 0x001C1158 File Offset: 0x001BF358
	public void ConfirmFine(NewAddress address, Interactable obj, StatusController.CrimeType crime)
	{
		StatusController.FineRecord fineRecord = this.activeFineRecords.Find((StatusController.FineRecord item) => !item.confirmed && ((address == null && item.addressID == -1) || (address != null && item.addressID == address.id)) && item.crime == crime && ((item.objectID <= -1 && obj == null) || (obj != null && item.objectID == obj.id)));
		if (fineRecord != null)
		{
			fineRecord.SetConfirmed(true);
			Player.Instance.StatusCheckEndOfFrame();
		}
	}

	// Token: 0x060020BB RID: 8379 RVA: 0x001C11AC File Offset: 0x001BF3AC
	public void PayActiveFines()
	{
		int num = 0;
		foreach (KeyValuePair<StatusController.StatusInstance, List<StatusController.StatusCount>> keyValuePair in this.activeStatusCounts)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				StatusController.StatusCount count = keyValuePair.Value[i];
				if (count.fineRecord != null && count.fineRecord.crime == StatusController.CrimeType.theft && count.fineRecord.objectID > -1)
				{
					Interactable interactable = null;
					if (CityData.Instance.savableInteractableDictionary.TryGetValue(count.fineRecord.objectID, ref interactable))
					{
						FirstPersonItemController.InventorySlot inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.interactableID == count.fineRecord.objectID);
						if (inventorySlot != null)
						{
							inventorySlot.SetSegmentContent(null);
						}
						interactable.inInventory = null;
						interactable.inv = 0;
						interactable.rPl = false;
						interactable.originalPosition = false;
						interactable.SetOriginalPosition(true, true);
						string[] array = new string[7];
						array[0] = "Removing stolen object: ";
						array[1] = interactable.name;
						array[2] = ", new postion: ";
						int num2 = 3;
						Vector3 vector = interactable.wPos;
						array[num2] = vector.ToString();
						array[4] = " (spawn: ";
						int num3 = 5;
						vector = interactable.spWPos;
						array[num3] = vector.ToString();
						array[6] = ")";
						Game.Log(string.Concat(array), 2);
					}
				}
				num += count.GetPenaltyAmount();
				count.Remove();
				i--;
			}
		}
		GameplayController.Instance.AddMoney(-num, false, "fines");
		Game.Log("Player: Pay fines: " + num.ToString(), 2);
	}

	// Token: 0x060020BC RID: 8380 RVA: 0x001C13B8 File Offset: 0x001BF5B8
	public void Trespassing(StatusController.StatusInstance inst)
	{
		if (Player.Instance.illegalAreaActive)
		{
			StatusPreset.StatusCountConfig statusCountConfig = inst.preset.countConfig[Mathf.Clamp(Player.Instance.trespassingEscalation - 1, 0, 1)];
			List<StatusController.StatusCount> list = null;
			if (this.activeStatusCounts.TryGetValue(inst, ref list))
			{
				if (list.Count > 0)
				{
					if (list[0].statusCountConfig == statusCountConfig)
					{
						return;
					}
					this.RemoveAllCounts(inst);
					new StatusController.StatusCount(inst, statusCountConfig);
				}
				else
				{
					new StatusController.StatusCount(inst, statusCountConfig);
				}
			}
			else
			{
				new StatusController.StatusCount(inst, statusCountConfig);
			}
			if (Player.Instance.currentGameLocation.thisAsAddress != null)
			{
				if (!this.activeFineRecords.Exists((StatusController.FineRecord item) => item.crime == StatusController.CrimeType.trespassing && item.addressID == Player.Instance.currentGameLocation.thisAsAddress.id))
				{
					this.AddFineRecord(Player.Instance.currentGameLocation.thisAsAddress, null, StatusController.CrimeType.trespassing, false, -1, false);
					return;
				}
			}
		}
		else
		{
			this.RemoveAllCounts(inst);
			this.activeFineRecords.RemoveAll((StatusController.FineRecord item) => item.crime == StatusController.CrimeType.trespassing && !item.confirmed);
		}
	}

	// Token: 0x060020BD RID: 8381 RVA: 0x001C14D8 File Offset: 0x001BF6D8
	public void AlarmActive(StatusController.StatusInstance inst)
	{
		if ((!(Player.Instance.currentGameLocation != null) || !(Player.Instance.currentGameLocation.thisAsAddress != null) || !Player.Instance.currentGameLocation.thisAsAddress.alarmActive) && (!(Player.Instance.currentBuilding != null) || !Player.Instance.currentBuilding.alarmActive))
		{
			this.RemoveAllCounts(inst.preset);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020BE RID: 8382 RVA: 0x001C157F File Offset: 0x001BF77F
	public void IllegalAction(StatusController.StatusInstance inst)
	{
		if (Player.Instance.illegalActionActive)
		{
			new StatusController.StatusCount(inst);
			return;
		}
		this.RemoveAllCounts(inst);
	}

	// Token: 0x060020BF RID: 8383 RVA: 0x001C159C File Offset: 0x001BF79C
	public void CaptureRisk(StatusController.StatusInstance inst)
	{
		using (List<StatusController.FineRecord>.Enumerator enumerator = this.activeFineRecords.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				StatusController.FineRecord f = enumerator.Current;
				NewBuilding building = null;
				if (f.addressID > -1)
				{
					NewAddress newAddress = null;
					if (CityData.Instance.addressDictionary.TryGetValue(f.addressID, ref newAddress))
					{
						building = newAddress.building;
					}
				}
				StatusController.StatusInstance statusInstance = default(StatusController.StatusInstance);
				statusInstance.preset = inst.preset;
				statusInstance.building = building;
				StatusPreset.StatusCountConfig newConfig = inst.preset.countConfig.Find((StatusPreset.StatusCountConfig item) => item.name == f.crime.ToString());
				List<StatusController.StatusCount> list = null;
				if (this.activeStatusCounts.TryGetValue(statusInstance, ref list))
				{
					if (!list.Exists((StatusController.StatusCount item) => item.fineRecord == f))
					{
						new StatusController.StatusCount(statusInstance, newConfig).fineRecord = f;
						Game.Log("Player: Create new crime count: " + f.crime.ToString(), 2);
					}
				}
				else
				{
					new StatusController.StatusCount(statusInstance, newConfig).fineRecord = f;
					Game.Log("Player: Create new crime count: " + f.crime.ToString(), 2);
				}
			}
		}
		foreach (KeyValuePair<StatusController.StatusInstance, List<StatusController.StatusCount>> keyValuePair in this.activeStatusCounts)
		{
			if (keyValuePair.Key.preset.countType == StatusPreset.StatusCountType.crime)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					StatusController.StatusCount statusCount = keyValuePair.Value[i];
					if (!this.activeFineRecords.Contains(statusCount.fineRecord))
					{
						Game.Log(string.Concat(new string[]
						{
							"Player: Remove crime count: ",
							statusCount.statusCountConfig.name,
							" (active fine records: ",
							this.activeFineRecords.Count.ToString(),
							")"
						}), 2);
						keyValuePair.Value.Remove(statusCount);
					}
				}
			}
		}
	}

	// Token: 0x060020C0 RID: 8384 RVA: 0x001C1820 File Offset: 0x001BFA20
	public void ImageCaptured(StatusController.StatusInstance inst)
	{
		foreach (NewBuilding building in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			StatusController.StatusInstance statusInstance = default(StatusController.StatusInstance);
			statusInstance.preset = inst.preset;
			statusInstance.building = building;
		}
	}

	// Token: 0x060020C1 RID: 8385 RVA: 0x001C1890 File Offset: 0x001BFA90
	public void Wanted(StatusController.StatusInstance inst)
	{
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			StatusController.StatusInstance statusInstance = default(StatusController.StatusInstance);
			statusInstance.preset = inst.preset;
			statusInstance.building = newBuilding;
			if (newBuilding.wantedInBuilding > SessionData.Instance.gameTime)
			{
				List<StatusController.StatusCount> list = null;
				if (this.activeStatusCounts.TryGetValue(statusInstance, ref list))
				{
					if (list.Count > 0)
					{
						break;
					}
					new StatusController.StatusCount(statusInstance);
				}
				else
				{
					new StatusController.StatusCount(statusInstance);
				}
			}
			else
			{
				this.RemoveAllCounts(statusInstance);
			}
		}
	}

	// Token: 0x060020C2 RID: 8386 RVA: 0x001C1948 File Offset: 0x001BFB48
	public void GuestPass(StatusController.StatusInstance inst)
	{
		if (GameplayController.Instance.guestPasses.Count > 0)
		{
			using (Dictionary<NewAddress, Vector2>.Enumerator enumerator = GameplayController.Instance.guestPasses.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<NewAddress, Vector2> keyValuePair = enumerator.Current;
					StatusController.StatusInstance statusInstance = default(StatusController.StatusInstance);
					statusInstance.preset = inst.preset;
					statusInstance.address = keyValuePair.Key;
					if (keyValuePair.Value.x > SessionData.Instance.gameTime)
					{
						List<StatusController.StatusCount> list = null;
						if (this.activeStatusCounts.TryGetValue(statusInstance, ref list))
						{
							if (list.Count > 0)
							{
								break;
							}
							new StatusController.StatusCount(statusInstance);
						}
						else
						{
							new StatusController.StatusCount(statusInstance);
						}
					}
					else
					{
						this.RemoveAllCounts(statusInstance);
					}
				}
				return;
			}
		}
		this.RemoveAllCounts(inst.preset);
	}

	// Token: 0x060020C3 RID: 8387 RVA: 0x001C1A30 File Offset: 0x001BFC30
	public void Detained(StatusController.StatusInstance inst)
	{
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			StatusController.StatusInstance inst2 = default(StatusController.StatusInstance);
			inst2.preset = inst.preset;
			inst2.building = newBuilding;
			if (Player.Instance.currentBuilding != newBuilding)
			{
				this.RemoveAllCounts(inst2);
			}
		}
	}

	// Token: 0x060020C4 RID: 8388 RVA: 0x001C1AB8 File Offset: 0x001BFCB8
	public void Echelons(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.allowEchelons || !(Player.Instance.currentGameLocation != null) || !(Player.Instance.currentGameLocation.thisAsAddress != null) || !(Player.Instance.currentGameLocation.thisAsAddress.floor != null) || !Player.Instance.currentGameLocation.thisAsAddress.floor.isEchelons)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020C5 RID: 8389 RVA: 0x001C1B68 File Offset: 0x001BFD68
	public void Hiding(StatusController.StatusInstance inst)
	{
		if (!Player.Instance.isHiding)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x001C1BB4 File Offset: 0x001BFDB4
	public void Stinky(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.smellyStatusEnabled || UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.noSmelly) > 0f)
		{
			if (Player.Instance.hygiene < 1f)
			{
				Player.Instance.hygiene = 1f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.hygiene >= 1f)
		{
			if (Player.Instance.hygiene >= 1f)
			{
				this.RemoveAllCounts(inst);
			}
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			if (Toolbox.Instance.Rand(0f, 1f, false) < 0.75f)
			{
				Player.Instance.speechController.Speak("dds.blocks", "38b0dfff-8764-471e-aaad-aae40a3d8a93", true, false, false, 2f, false, default(Color), null, false, false, null, null, null, null);
			}
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = 1f - Player.Instance.hygiene;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020C7 RID: 8391 RVA: 0x001C1CCC File Offset: 0x001BFECC
	public void Cold(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.coldStatusEnabled || UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.noCold) > 0f)
		{
			if (Player.Instance.heat < 0.5f)
			{
				Player.Instance.heat = 0.5f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.heat > 0.38f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			if (Toolbox.Instance.Rand(0f, 1f, false) < 0.75f)
			{
				Player.Instance.speechController.Speak("dds.blocks", "7bfb0916-d70b-4749-88d5-11db123941f1", true, false, false, 2f, false, default(Color), null, false, false, null, null, null, null);
			}
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = 1f - Player.Instance.heat / 0.38f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020C8 RID: 8392 RVA: 0x001C1DD8 File Offset: 0x001BFFD8
	public void Hungry(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.hungerStatusEnabled)
		{
			if (Player.Instance.nourishment < 0.5f)
			{
				Player.Instance.nourishment = 0.5f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.nourishment > 0.2f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = 1f - Player.Instance.nourishment / 0.2f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x001C1E80 File Offset: 0x001C0080
	public void Energized(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.energizedStatusEnabled)
		{
			if (Player.Instance.nourishment >= 0.9f)
			{
				Player.Instance.nourishment = 0.8f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.nourishment < 0.9f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.nourishment - 0.9f) / 0.1f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x001C1F28 File Offset: 0x001C0128
	public void Thirsty(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.hydrationStatusEnabled)
		{
			if (Player.Instance.hydration < 0.5f)
			{
				Player.Instance.hydration = 0.5f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.hydration > 0.2f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = 1f - Player.Instance.hydration / 0.2f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x001C1FD0 File Offset: 0x001C01D0
	public void Hydrated(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.hydratedStatusEnabled)
		{
			if (Player.Instance.hydration >= 0.9f)
			{
				Player.Instance.hydration = 0.8f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.hydration < 0.9f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.hydration - 0.9f) / 0.1f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x001C2078 File Offset: 0x001C0278
	public void Drunk(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.drunkStatusEnabled)
		{
			if (Player.Instance.drunk > 0f)
			{
				Player.Instance.drunk = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.drunk <= 0.05f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.drunk - 0.05f) / 0.95f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x001C2120 File Offset: 0x001C0320
	public void Sick(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.sickStatusEnabled)
		{
			if (Player.Instance.sick > 0f)
			{
				Player.Instance.sick = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.sick <= 0.05f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			if (Toolbox.Instance.Rand(0f, 1f, false) < 0.75f)
			{
				Player.Instance.speechController.Speak("dds.blocks", "ed1f5108-5416-46aa-b4b9-4c37182958b8", true, false, false, 2f, false, default(Color), null, false, false, null, null, null, null);
			}
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.sick - 0.05f) / 0.95f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x001C2218 File Offset: 0x001C0418
	public void StarchAddiction(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.starchAddictionEnabled)
		{
			if (Player.Instance.starchAddiction > 0f)
			{
				Player.Instance.starchAddiction = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.starchAddiction <= 0.1f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.starchAddiction - 0.1f) / 0.9f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020CF RID: 8399 RVA: 0x001C22C0 File Offset: 0x001C04C0
	public void Headache(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.headacheStatusEnabled)
		{
			if (Player.Instance.headache > 0f)
			{
				Player.Instance.headache = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.headache <= 0.05f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.headache - 0.05f) / 0.95f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D0 RID: 8400 RVA: 0x001C2368 File Offset: 0x001C0568
	public void Wet(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.wetStatusEnabled)
		{
			if (Player.Instance.wet > 0f)
			{
				Player.Instance.wet = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.wet <= 0.1f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.wet - 0.1f) / 0.9f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x001C2410 File Offset: 0x001C0610
	public void BrokenLeg(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.injuryStatusEnabled)
		{
			if (Player.Instance.brokenLeg > 0f)
			{
				Player.Instance.brokenLeg = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.brokenLeg <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.brokenLeg;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x001C24AC File Offset: 0x001C06AC
	public void Bruised(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.injuryStatusEnabled)
		{
			if (Player.Instance.bruised > 0f)
			{
				Player.Instance.bruised = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.bruised <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.bruised;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x001C2548 File Offset: 0x001C0748
	public void BlackEye(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.injuryStatusEnabled)
		{
			if (Player.Instance.blackEye > 0f)
			{
				Player.Instance.blackEye = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.blackEye <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.blackEye;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x001C25E4 File Offset: 0x001C07E4
	public void BlackedOut(StatusController.StatusInstance inst)
	{
		if (Player.Instance.blackedOut <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.blackedOut;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x001C264C File Offset: 0x001C084C
	public void Numb(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.numbStatusEnabled)
		{
			if (Player.Instance.numb > 0f)
			{
				Player.Instance.numb = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.numb <= 0.1f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.numb - 0.1f) / 0.9f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x001C26F4 File Offset: 0x001C08F4
	public void Poisoned(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.poisonStatusEnabled)
		{
			if (Player.Instance.poisoned > 0f)
			{
				Player.Instance.poisoned = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.poisoned <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.poisoned;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x001C2790 File Offset: 0x001C0990
	public void Blinded(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.blindedStatusEnabled)
		{
			if (Player.Instance.blinded > 0f)
			{
				Player.Instance.blinded = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.blinded <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.blinded;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x001C282C File Offset: 0x001C0A2C
	public void Bleeding(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.bleedingStatusEnabled)
		{
			if (Player.Instance.bleeding > 0f)
			{
				Player.Instance.bleeding = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.bleeding <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.bleeding;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x001C28C8 File Offset: 0x001C0AC8
	public void Tired(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.tiredStatusEnabled || UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.noTired) > 0f)
		{
			if (Player.Instance.energy < 0.5f)
			{
				Player.Instance.energy = 0.5f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.energy > 0.2f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			if (Toolbox.Instance.Rand(0f, 1f, false) < 0.75f)
			{
				if (Toolbox.Instance.Rand(0f, 1f, false) < 0.5f)
				{
					Player.Instance.speechController.Speak("dds.blocks", "d3443636-b897-4df3-a072-45aa7907737f", true, false, false, 2f, false, default(Color), null, false, false, null, null, null, null);
				}
				else
				{
					Player.Instance.speechController.Speak("dds.blocks", "f67684e7-ba63-49d8-83dc-f689d0b670f5", true, false, false, 2f, false, default(Color), null, false, false, null, null, null, null);
				}
			}
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = 1f - Player.Instance.energy / 0.2f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020DA RID: 8410 RVA: 0x001C2A28 File Offset: 0x001C0C28
	public void Focused(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.focusedStatusEnabled)
		{
			if (Player.Instance.alertness >= 0.9f)
			{
				Player.Instance.alertness = 0.8f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.alertness < 0.9f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = (Player.Instance.alertness - 0.9f) / 0.1f;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x001C2AD0 File Offset: 0x001C0CD0
	public void Pursued(StatusController.StatusInstance inst)
	{
		if (Player.Instance.persuedBy.Count <= 0)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x001C2B24 File Offset: 0x001C0D24
	public void WellRested(StatusController.StatusInstance inst)
	{
		if (!Game.Instance.wellRestedStatusEnabled)
		{
			if (Player.Instance.wellRested > 0f)
			{
				Player.Instance.wellRested = 0f;
			}
			this.RemoveAllCounts(inst);
			return;
		}
		if (Player.Instance.wellRested <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.wellRested;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x001C2BC0 File Offset: 0x001C0DC0
	public void SyncDiskInstall(StatusController.StatusInstance inst)
	{
		if (Player.Instance.syncDiskInstall <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.syncDiskInstall;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x001C2C28 File Offset: 0x001C0E28
	public void ToxicGas(StatusController.StatusInstance inst)
	{
		if (Player.Instance.gasLevel <= 0f)
		{
			this.RemoveAllCounts(inst);
			return;
		}
		List<StatusController.StatusCount> list = null;
		if (!this.activeStatusCounts.TryGetValue(inst, ref list))
		{
			new StatusController.StatusCount(inst);
			return;
		}
		if (list.Count > 0)
		{
			list[0].amount = Player.Instance.gasLevel;
			return;
		}
		new StatusController.StatusCount(inst);
	}

	// Token: 0x04002AD7 RID: 10967
	[Header("References")]
	public RectTransform statusParent;

	// Token: 0x04002AD8 RID: 10968
	[Header("Settings")]
	public float elementDefaultWdith = 280f;

	// Token: 0x04002AD9 RID: 10969
	public float elementMinimizedWidth = 42f;

	// Token: 0x04002ADA RID: 10970
	public float elementDefaultHeight = 42f;

	// Token: 0x04002ADB RID: 10971
	public float elementYInterval = -4f;

	// Token: 0x04002ADC RID: 10972
	public AnimationCurve detailTextFadeInCurve;

	// Token: 0x04002ADD RID: 10973
	[ReadOnly]
	[Header("State")]
	public bool disabledRecovery;

	// Token: 0x04002ADE RID: 10974
	[ReadOnly]
	public bool disabledSprint;

	// Token: 0x04002ADF RID: 10975
	[ReadOnly]
	public bool disabledJump;

	// Token: 0x04002AE0 RID: 10976
	[ReadOnly]
	public float recoveryRateMultiplier = 1f;

	// Token: 0x04002AE1 RID: 10977
	[ReadOnly]
	public float maxHealthMultiplier = 1f;

	// Token: 0x04002AE2 RID: 10978
	[ReadOnly]
	public float movementSpeedMultiplier = 1f;

	// Token: 0x04002AE3 RID: 10979
	[ReadOnly]
	public float temperatureGainMultiplier = 1f;

	// Token: 0x04002AE4 RID: 10980
	[ReadOnly]
	public float damageIncomingMultiplier = 1f;

	// Token: 0x04002AE5 RID: 10981
	[ReadOnly]
	public float damageOutgoingMultiplier = 1f;

	// Token: 0x04002AE6 RID: 10982
	[ReadOnly]
	public float drunkControls;

	// Token: 0x04002AE7 RID: 10983
	public Dictionary<AnimationCurve, float> affectHeadBobs = new Dictionary<AnimationCurve, float>();

	// Token: 0x04002AE8 RID: 10984
	[ReadOnly]
	public float drunkVision;

	// Token: 0x04002AE9 RID: 10985
	[ReadOnly]
	public float shiverVision;

	// Token: 0x04002AEA RID: 10986
	[ReadOnly]
	public float headacheVision;

	// Token: 0x04002AEB RID: 10987
	[ReadOnly]
	public float drunkLensDistort;

	// Token: 0x04002AEC RID: 10988
	[ReadOnly]
	public float tripChanceWet;

	// Token: 0x04002AED RID: 10989
	[ReadOnly]
	public float tripChanceDrunk;

	// Token: 0x04002AEE RID: 10990
	[ReadOnly]
	public float bloomIntensityMultiplier = 1f;

	// Token: 0x04002AEF RID: 10991
	[ReadOnly]
	public float motionBlurMultiplier = 1f;

	// Token: 0x04002AF0 RID: 10992
	[ReadOnly]
	public float chromaticAbberationAmount;

	// Token: 0x04002AF1 RID: 10993
	[ReadOnly]
	public float vignetteAmount = 1f;

	// Token: 0x04002AF2 RID: 10994
	[ReadOnly]
	public float exposureAmount;

	// Token: 0x04002AF3 RID: 10995
	[ReadOnly]
	public float channelRedR = 100f;

	// Token: 0x04002AF4 RID: 10996
	[ReadOnly]
	public float channelRedG;

	// Token: 0x04002AF5 RID: 10997
	[ReadOnly]
	public float channelRedB;

	// Token: 0x04002AF6 RID: 10998
	[ReadOnly]
	public float channelGreenR;

	// Token: 0x04002AF7 RID: 10999
	[ReadOnly]
	public float channelGreenG = 100f;

	// Token: 0x04002AF8 RID: 11000
	[ReadOnly]
	public float channelGreenB;

	// Token: 0x04002AF9 RID: 11001
	[ReadOnly]
	public float channelBlueR;

	// Token: 0x04002AFA RID: 11002
	[ReadOnly]
	public float channelBlueG;

	// Token: 0x04002AFB RID: 11003
	[ReadOnly]
	public float channelBlueB = 100f;

	// Token: 0x04002AFC RID: 11004
	[Header("Interface")]
	public List<StateElementController> spawnedControllers = new List<StateElementController>();

	// Token: 0x04002AFD RID: 11005
	public Dictionary<StatusController.StatusInstance, List<StatusController.StatusCount>> activeStatusCounts = new Dictionary<StatusController.StatusInstance, List<StatusController.StatusCount>>();

	// Token: 0x04002AFE RID: 11006
	public HashSet<StatusPreset> activeStatuses = new HashSet<StatusPreset>();

	// Token: 0x04002AFF RID: 11007
	public List<StatusController.FineRecord> activeFineRecords = new List<StatusController.FineRecord>();

	// Token: 0x04002B00 RID: 11008
	private Dictionary<StatusPreset, MethodInfo> checkingRef = new Dictionary<StatusPreset, MethodInfo>();

	// Token: 0x04002B01 RID: 11009
	private static StatusController _instance;

	// Token: 0x020005D1 RID: 1489
	[Serializable]
	public class FineRecord
	{
		// Token: 0x060020E0 RID: 8416 RVA: 0x001C2D98 File Offset: 0x001C0F98
		public FineRecord(NewAddress ad, Interactable obj, StatusController.CrimeType newCrime)
		{
			if (ad != null)
			{
				this.addressID = ad.id;
			}
			if (obj != null)
			{
				this.objectID = obj.id;
			}
			this.crime = newCrime;
		}

		// Token: 0x060020E1 RID: 8417 RVA: 0x001C2DEB File Offset: 0x001C0FEB
		public void SetConfirmed(bool val)
		{
			if (this.confirmed != val)
			{
				this.confirmed = val;
			}
		}

		// Token: 0x04002B02 RID: 11010
		public int addressID = -1;

		// Token: 0x04002B03 RID: 11011
		public int objectID = -1;

		// Token: 0x04002B04 RID: 11012
		public StatusController.CrimeType crime;

		// Token: 0x04002B05 RID: 11013
		public bool confirmed;

		// Token: 0x04002B06 RID: 11014
		public int forcedPenalty = -1;
	}

	// Token: 0x020005D2 RID: 1490
	public enum CrimeType
	{
		// Token: 0x04002B08 RID: 11016
		assault,
		// Token: 0x04002B09 RID: 11017
		theft,
		// Token: 0x04002B0A RID: 11018
		breakingAndEntering,
		// Token: 0x04002B0B RID: 11019
		trespassing,
		// Token: 0x04002B0C RID: 11020
		tampering,
		// Token: 0x04002B0D RID: 11021
		vandalism
	}

	// Token: 0x020005D3 RID: 1491
	public struct StatusInstance
	{
		// Token: 0x04002B0E RID: 11022
		public StatusPreset preset;

		// Token: 0x04002B0F RID: 11023
		public NewBuilding building;

		// Token: 0x04002B10 RID: 11024
		public NewAddress address;
	}

	// Token: 0x020005D4 RID: 1492
	public class StatusCount
	{
		// Token: 0x060020E2 RID: 8418 RVA: 0x001C2E00 File Offset: 0x001C1000
		public StatusCount(StatusController.StatusInstance newInstance)
		{
			this.statusInstance = newInstance;
			this.preset = this.statusInstance.preset;
			this.statusCountConfig = this.preset.countConfig[0];
			if (this.preset.onAcquire != null && SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
			{
				AudioController.Instance.Play2DSound(this.preset.onAcquire, null, 1f);
			}
			if (this.statusCountConfig.onAcquire != null && SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
			{
				AudioController.Instance.Play2DSound(this.statusCountConfig.onAcquire, null, 1f);
			}
			if (!StatusController.Instance.activeStatusCounts.ContainsKey(this.statusInstance))
			{
				StatusController.Instance.activeStatusCounts.Add(this.statusInstance, new List<StatusController.StatusCount>());
			}
			StatusController.Instance.activeStatusCounts[this.statusInstance].Add(this);
			if (!StatusController.Instance.activeStatuses.Contains(this.preset))
			{
				StatusController.Instance.activeStatuses.Add(this.preset);
			}
			if (this.preset.alertWhenNewCountIsAdded)
			{
				StateElementController stateElementController = StatusController.Instance.spawnedControllers.Find((StateElementController item) => item.statusInstance.preset == this.statusInstance.preset && item.statusInstance.building == this.statusInstance.building && item.statusInstance.address == this.statusInstance.address);
				if (stateElementController != null)
				{
					stateElementController.SetMinimized(false);
					stateElementController.iconJuice.Flash(2, false, default(Color), 10f);
					stateElementController.juice.Flash(2, false, default(Color), 10f);
				}
			}
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x001C2FD4 File Offset: 0x001C11D4
		public StatusCount(StatusController.StatusInstance newInstance, StatusPreset.StatusCountConfig newConfig)
		{
			this.statusInstance = newInstance;
			this.preset = this.statusInstance.preset;
			this.statusCountConfig = newConfig;
			if (this.preset.onAcquire != null && SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
			{
				AudioController.Instance.Play2DSound(this.preset.onAcquire, null, 1f);
			}
			if (this.statusCountConfig.onAcquire != null && SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
			{
				AudioController.Instance.Play2DSound(this.statusCountConfig.onAcquire, null, 1f);
			}
			if (!StatusController.Instance.activeStatusCounts.ContainsKey(this.statusInstance))
			{
				StatusController.Instance.activeStatusCounts.Add(this.statusInstance, new List<StatusController.StatusCount>());
			}
			StatusController.Instance.activeStatusCounts[this.statusInstance].Add(this);
			if (!StatusController.Instance.activeStatuses.Contains(this.preset))
			{
				StatusController.Instance.activeStatuses.Add(this.preset);
				Game.Log("Player: New status: " + this.preset.name, 2);
			}
			if (this.preset.alertWhenNewCountIsAdded)
			{
				StateElementController stateElementController = StatusController.Instance.spawnedControllers.Find((StateElementController item) => item.statusInstance.preset == this.statusInstance.preset && item.statusInstance.building == this.statusInstance.building && item.statusInstance.address == this.statusInstance.address);
				if (stateElementController != null)
				{
					stateElementController.iconJuice.Flash(2, false, default(Color), 10f);
					stateElementController.juice.Flash(2, false, default(Color), 10f);
				}
			}
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x001C31AC File Offset: 0x001C13AC
		public void Remove()
		{
			StatusController.Instance.activeStatusCounts[this.statusInstance].Remove(this);
			if (this.preset.alertWhenNewCountIsAdded)
			{
				StateElementController stateElementController = StatusController.Instance.spawnedControllers.Find((StateElementController item) => item.statusInstance.preset == this.statusInstance.preset && item.statusInstance.building == this.statusInstance.building && item.statusInstance.address == this.statusInstance.address);
				if (stateElementController != null)
				{
					stateElementController.iconJuice.Flash(2, false, default(Color), 10f);
					stateElementController.juice.Flash(2, false, default(Color), 10f);
				}
			}
			if ((!StatusController.Instance.activeStatusCounts.ContainsKey(this.statusInstance) || StatusController.Instance.activeStatusCounts[this.statusInstance].Count <= 0) && StatusController.Instance.activeStatuses.Contains(this.preset))
			{
				int num = 0;
				foreach (KeyValuePair<StatusController.StatusInstance, List<StatusController.StatusCount>> keyValuePair in StatusController.Instance.activeStatusCounts)
				{
					if (keyValuePair.Key.preset == this.preset)
					{
						num += keyValuePair.Value.Count;
						if (num > 0)
						{
							break;
						}
					}
				}
				if (num <= 0)
				{
					StatusController.Instance.activeStatuses.Remove(this.preset);
				}
			}
			if (this.preset.maxHealthPlusMP != 0f)
			{
				Player.Instance.AddHealth(0f, true, false);
			}
		}

		// Token: 0x060020E5 RID: 8421 RVA: 0x001C333C File Offset: 0x001C153C
		public int GetPenaltyAmount()
		{
			int result = 0;
			if (this.fineRecord != null && this.fineRecord.forcedPenalty > -1)
			{
				return this.fineRecord.forcedPenalty;
			}
			if (this.statusCountConfig.penaltyRule == StatusPreset.PenaltyRule.fixedValue)
			{
				result = Mathf.CeilToInt(this.statusCountConfig.penalty);
			}
			else if (this.statusCountConfig.penaltyRule == StatusPreset.PenaltyRule.percentageValue)
			{
				result = Mathf.CeilToInt((float)GameplayController.Instance.money * this.statusCountConfig.penalty);
			}
			else if (this.statusCountConfig.penaltyRule == StatusPreset.PenaltyRule.objectValueMultiplied && this.fineRecord != null)
			{
				Interactable interactable = null;
				if (CityData.Instance.savableInteractableDictionary.TryGetValue(this.fineRecord.objectID, ref interactable))
				{
					result = Mathf.CeilToInt(interactable.val * this.statusCountConfig.penalty);
				}
			}
			return result;
		}

		// Token: 0x04002B11 RID: 11025
		public StatusController.StatusInstance statusInstance;

		// Token: 0x04002B12 RID: 11026
		public StatusPreset preset;

		// Token: 0x04002B13 RID: 11027
		public StatusPreset.StatusCountConfig statusCountConfig;

		// Token: 0x04002B14 RID: 11028
		public StatusController.FineRecord fineRecord;

		// Token: 0x04002B15 RID: 11029
		public float amount;
	}
}
