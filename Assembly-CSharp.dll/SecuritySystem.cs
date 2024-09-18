using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000429 RID: 1065
public class SecuritySystem : Machine
{
	// Token: 0x06001814 RID: 6164 RVA: 0x00167498 File Offset: 0x00165698
	public void Setup(Interactable newInteractable, bool inheritOpenStatusFromInteractable = true)
	{
		this.interactable = newInteractable;
		if (this.interactable.evidence == null)
		{
			this.CreateEvidence();
			this.interactable.evidence = this.evidenceEntry;
		}
		else
		{
			this.evidenceEntry = (this.interactable.evidence as EvidenceWitness);
		}
		if (inheritOpenStatusFromInteractable)
		{
			if (this.interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.securityCamera)
			{
				this.SetActive(this.interactable.sw0, true);
				return;
			}
			if (this.interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.sentryGun)
			{
				if (this.interactable.sw0 && this.interactable.sw1)
				{
					this.SetActive(true, true);
					return;
				}
				this.SetActive(false, true);
			}
		}
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x00167550 File Offset: 0x00165750
	public override void CreateEvidence()
	{
		if (this.evidenceEntry == null)
		{
			string newID = "SecuritySystem" + this.interactable.id.ToString();
			this.evidenceEntry = (EvidenceCreator.Instance.CreateEvidence("SecuritySystem", newID, this, null, null, null, null, false, null) as EvidenceWitness);
		}
	}

	// Token: 0x06001816 RID: 6166 RVA: 0x001675A4 File Offset: 0x001657A4
	public void SetActive(bool open, bool skipAnimation = false)
	{
		if (this.interactable != null && this.interactable.node != null && this.interactable.node.gameLocation.thisAsAddress != null)
		{
			Interactable breakerSecurity = this.interactable.node.gameLocation.thisAsAddress.GetBreakerSecurity();
			if (breakerSecurity != null && !breakerSecurity.sw0)
			{
				open = false;
			}
		}
		this.isActive = open;
		if (this.laser != null)
		{
			this.laser.SetActive(this.isActive);
		}
		if (skipAnimation || !base.gameObject.activeInHierarchy)
		{
			if (this.isActive)
			{
				if (this.anim != null)
				{
					this.anim.SetTrigger("activeImmediate");
				}
			}
			else if (this.anim != null)
			{
				this.anim.SetTrigger("inactiveImmediate");
			}
		}
		if (this.anim != null)
		{
			this.anim.SetBool("active", this.isActive);
		}
		if (this.isActive)
		{
			base.enabled = true;
		}
		else
		{
			this.ResetFocus();
		}
		this.UpdateMaterial();
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x001676CC File Offset: 0x001658CC
	private void UpdateMaterial()
	{
		if (this.system == SecuritySystem.SecuritySystemType.camera)
		{
			if (this.isActive)
			{
				if (this.trackingTarget != null && this.seesIllegal.ContainsKey(this.trackingTarget) && this.seesIllegal[this.trackingTarget] >= 1f)
				{
					if (this.rend.sharedMaterial != InteriorControls.Instance.cameraAlertMaterial)
					{
						this.rend.sharedMaterial = InteriorControls.Instance.cameraAlertMaterial;
						return;
					}
				}
				else if (this.rend.sharedMaterial != InteriorControls.Instance.cameraOnMaterial)
				{
					this.rend.sharedMaterial = InteriorControls.Instance.cameraOnMaterial;
					return;
				}
			}
			else if (this.rend.sharedMaterial != InteriorControls.Instance.cameraOffMaterial)
			{
				this.rend.sharedMaterial = InteriorControls.Instance.cameraOffMaterial;
			}
		}
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x001677C0 File Offset: 0x001659C0
	public override void OnInvestigate(Actor newTarget, int escalation)
	{
		Game.Log("Player: Security system spotted " + ((newTarget != null) ? newTarget.ToString() : null) + " with escalation " + escalation.ToString(), 2);
		if (this.trackingTarget == null && newTarget != null && this.system == SecuritySystem.SecuritySystemType.sentry)
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.sentryGunTargetAcquire, this, this.currentNode, this.muzzleTransform.position, null, null, 1f, null, false, null, false);
		}
		this.trackingTarget = newTarget;
		this.forgetProgress = 0f;
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x0016785C File Offset: 0x00165A5C
	private void Update()
	{
		this.isAnimating = false;
		if (this.anim != null && this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			this.isAnimating = true;
		}
		if (!this.isActive)
		{
			base.enabled = false;
		}
		if (!SessionData.Instance.play)
		{
			return;
		}
		if (this.laser != null)
		{
			Toolbox.Instance.HandleLaserBehaviour(this, this.laser, this.laserLight, 16f);
		}
		if (!this.isAnimating)
		{
			if (base.gameObject.activeInHierarchy)
			{
				this.seekUpdateProgress += Time.deltaTime;
				if (this.seekUpdateProgress >= 0.33f)
				{
					this.seekUpdateProgress = 0f;
					if (this.system == SecuritySystem.SecuritySystemType.camera)
					{
						this.SightingCheck(GameplayControls.Instance.securityFOV, false);
						if (!(this.trackingTarget == null) || this.seenIllegalThisCheck.Count <= 0)
						{
							goto IL_307;
						}
						using (HashSet<Actor>.Enumerator enumerator = this.seenIllegalThisCheck.GetEnumerator())
						{
							if (!enumerator.MoveNext())
							{
								goto IL_307;
							}
							Actor newTarget = enumerator.Current;
							this.OnInvestigate(newTarget, 1);
							goto IL_307;
						}
					}
					if (this.system == SecuritySystem.SecuritySystemType.sentry)
					{
						this.seenIllegalThisCheck.Clear();
						this.SightingCheck(1f, false);
						foreach (Interactable interactable in this.interactable.node.gameLocation.securityCameras)
						{
							if (!(interactable.controller == null) && !(interactable.controller.securitySystem == null))
							{
								if (this.trackingTarget == null && interactable.controller.securitySystem.trackingTarget != null)
								{
									this.OnInvestigate(interactable.controller.securitySystem.trackingTarget, 2);
								}
								foreach (Actor actor in interactable.controller.securitySystem.seenIllegalThisCheck)
								{
									Human human = (Human)actor;
									if (!this.seenIllegalThisCheck.Contains(human))
									{
										this.seenIllegalThisCheck.Add(human);
									}
								}
								foreach (KeyValuePair<Actor, float> keyValuePair in interactable.controller.securitySystem.seesIllegal)
								{
									if (!this.seesIllegal.ContainsKey(keyValuePair.Key))
									{
										this.seesIllegal.Add(keyValuePair.Key, keyValuePair.Value);
									}
									else
									{
										this.seesIllegal[keyValuePair.Key] = Mathf.Max(this.seesIllegal[keyValuePair.Key], keyValuePair.Value);
									}
								}
							}
						}
					}
				}
			}
			IL_307:
			if (this.trackingTarget == null)
			{
				if (this.seenIllegalThisCheck.Count > 0)
				{
					this.ResetFocus();
				}
				if (this.system == SecuritySystem.SecuritySystemType.camera || (this.system == SecuritySystem.SecuritySystemType.sentry && this.isActive))
				{
					this.sweepProgress += Time.deltaTime / 10f;
					float angle = Mathf.Lerp(this.selfTransform.localEulerAngles.x, 20f, GameplayControls.Instance.securityTrackSpeed * Time.deltaTime);
					this.selfTransform.localEulerAngles = new Vector3(Toolbox.Instance.ClampAngle(angle, 0f, 80f), 0f, 0f);
					Quaternion quaternion = Quaternion.Euler(0f, this.cameraSweep.Evaluate(this.sweepProgress), 0f);
					this.rotationPivotTransform.localRotation = Quaternion.Slerp(this.rotationPivotTransform.localRotation, quaternion, GameplayControls.Instance.securityTrackSpeed * Time.deltaTime);
					if (this.sweepProgress > 1f)
					{
						this.sweepProgress -= 1f;
					}
					this.sweepProgress = Mathf.Clamp01(this.sweepProgress);
					return;
				}
			}
			else
			{
				if (this.seesIllegal.ContainsKey(this.trackingTarget) && !this.trackingTarget.isDead && !this.trackingTarget.isStunned)
				{
					if (this.awarenessIcon == null && this.trackingTarget.isPlayer)
					{
						this.awarenessIcon = InterfaceController.Instance.AddAwarenessIcon(InterfaceController.AwarenessType.actor, InterfaceController.AwarenessBehaviour.alwaysVisible, this, this.selfTransform, Vector3.zero, InterfaceControls.Instance.spotted, 10, true, InterfaceControls.Instance.maxIndicatorDistance);
					}
					else if (this.awarenessIcon != null)
					{
						this.awarenessIcon.removalFlag = false;
					}
					bool flag = this.interactable.node.gameLocation.IsAlarmSystemTarget(this.trackingTarget as Human);
					int num = this.trackingTarget.trespassingEscalation;
					if (this.trackingTarget.illegalActionActive)
					{
						num = 2;
					}
					if (this.trackingTarget.currentRoom != null)
					{
						num += this.trackingTarget.currentRoom.gameLocation.GetAdditionalEscalation(this.trackingTarget);
					}
					num = Mathf.Clamp(num, 1, 2);
					if (this.seesIllegal[this.trackingTarget] < 1f || (num == 1 && !flag))
					{
						string[] array = new string[6];
						array[0] = "Security system Sees illegal contains ";
						int num2 = 1;
						Actor actor2 = this.trackingTarget;
						array[num2] = ((actor2 != null) ? actor2.ToString() : null);
						array[2] = ": ";
						int num3 = 3;
						float num4 = this.seesIllegal[this.trackingTarget];
						array[num3] = num4.ToString();
						array[4] = " esc ";
						array[5] = num.ToString();
						Game.Log(string.Concat(array), 2);
						if (this.system == SecuritySystem.SecuritySystemType.camera)
						{
							this.focusFlashCounter += Time.deltaTime * 4.5f;
							if (this.focusFlashCounter >= 1f)
							{
								this.focusFlashCounter = 0f;
								if ((num >= 2 || flag) && this.rend.sharedMaterial != InteriorControls.Instance.cameraAlertMaterial)
								{
									this.rend.sharedMaterial = InteriorControls.Instance.cameraAlertMaterial;
									NewBuilding.AlarmTargetMode alarmTargetMode;
									List<Human> list;
									if (!this.interactable.node.gameLocation.IsAlarmActive(out num4, out alarmTargetMode, out list))
									{
										AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.securityCameraAlert, this, this.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
									}
								}
								else if (num == 1 && this.rend.sharedMaterial != InteriorControls.Instance.cameraFocusMaterial)
								{
									this.rend.sharedMaterial = InteriorControls.Instance.cameraFocusMaterial;
								}
								else if (num == 1 || this.seesIllegal[this.trackingTarget] <= 0.5f)
								{
									this.rend.sharedMaterial = InteriorControls.Instance.cameraOnMaterial;
								}
								else
								{
									this.rend.sharedMaterial = InteriorControls.Instance.cameraFocusMaterial;
								}
							}
						}
						if (num == 1 && !flag && this.trackingTarget.currentRoom != null)
						{
							this.trackingTarget.currentRoom.gameLocation.AddEscalation(this.trackingTarget);
						}
					}
					else if (this.seesIllegal[this.trackingTarget] >= 1f && !this.acquiredTarget && (num >= 2 || flag))
					{
						string text = "...Acquire target ";
						Actor actor3 = this.trackingTarget;
						Game.Log(text + ((actor3 != null) ? actor3.ToString() : null), 2);
						this.acquiredTarget = true;
						if (this.trackingTarget.isPlayer && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.illegalActionActive)
						{
							StatusController.Instance.ConfirmFine(Player.Instance.currentGameLocation.thisAsAddress, null, StatusController.CrimeType.trespassing);
						}
						this.UpdateMaterial();
					}
					if (this.acquiredTarget)
					{
						if (this.system == SecuritySystem.SecuritySystemType.camera && this.interactable.node.gameLocation.thisAsAddress != null)
						{
							this.interactable.node.gameLocation.thisAsAddress.SetAlarm(true, this.trackingTarget as Human);
						}
						if (this.system == SecuritySystem.SecuritySystemType.sentry)
						{
							if (this.interactable.node.gameLocation.thisAsAddress != null)
							{
								this.interactable.node.gameLocation.thisAsAddress.SetAlarm(true, this.trackingTarget as Human);
							}
							this.sentryFireProgress += Time.deltaTime * GameplayControls.Instance.sentryGunROF;
							if (this.sentryFireProgress >= 1f)
							{
								this.sentryFireProgress -= 1f;
								Toolbox.Instance.Shoot(this, this.muzzleTransform.position, this.trackingTarget.aimTransform.position, 12f, GameplayControls.Instance.sentryGunAccuracy, GameplayControls.Instance.sentryGunDamage, GameplayControls.Instance.sentryGunWeapon, false, Vector3.zero, false);
							}
						}
					}
				}
				else if (this.trackingTarget != null && (!this.seenIllegalThisCheck.Contains(this.trackingTarget) || this.trackingTarget.isDead || this.trackingTarget.isStunned))
				{
					this.forgetProgress += Time.deltaTime / this.focusGiveUpTime;
					if (this.forgetProgress >= 1f)
					{
						this.OnInvestigate(null, 0);
						this.ResetFocus();
						return;
					}
					if (this.system == SecuritySystem.SecuritySystemType.sentry)
					{
						this.pulseProgress += Time.deltaTime / 0.9f;
						if (this.pulseProgress >= 1f)
						{
							AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.sentryGunSearchPulse, this, this.currentNode, this.muzzleTransform.position, null, null, 1f, null, false, null, false);
							this.pulseProgress = 0f;
						}
					}
				}
				if (this.trackingTarget != null)
				{
					Vector3 vector = this.trackingTarget.transform.position - this.selfTransform.position;
					if (vector != Vector3.zero)
					{
						this.desiredSelfRotation = Quaternion.LookRotation(vector, Vector3.up);
						this.desiredPivotRotation = Quaternion.LookRotation(vector, Vector3.up);
					}
				}
				if (this.selfTransform.rotation != this.desiredSelfRotation)
				{
					this.selfTransform.rotation = Quaternion.Slerp(this.selfTransform.rotation, this.desiredSelfRotation, GameplayControls.Instance.securityTrackSpeed * Time.deltaTime);
					Vector3 localEulerAngles = this.selfTransform.localEulerAngles;
					this.selfTransform.localEulerAngles = new Vector3(Toolbox.Instance.ClampAngle(localEulerAngles.x, 0f, 80f), 0f, 0f);
				}
				if (this.rotationPivotTransform.rotation != this.desiredPivotRotation)
				{
					this.rotationPivotTransform.rotation = Quaternion.Slerp(this.rotationPivotTransform.rotation, this.desiredPivotRotation, GameplayControls.Instance.securityTrackSpeed * Time.deltaTime);
					Vector3 localEulerAngles2 = this.rotationPivotTransform.localEulerAngles;
					this.rotationPivotTransform.localEulerAngles = new Vector3(0f, localEulerAngles2.y, 0f);
				}
			}
		}
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x00168474 File Offset: 0x00166674
	private void OnDestroy()
	{
		this.ResetFocus();
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x00168474 File Offset: 0x00166674
	private void OnDisable()
	{
		this.ResetFocus();
	}

	// Token: 0x0600181C RID: 6172 RVA: 0x0016847C File Offset: 0x0016667C
	public void ResetFocus()
	{
		this.ClearSeesIllegal();
		this.focusFlashCounter = 0f;
		this.acquiredTarget = false;
		this.trackingTarget = null;
		this.forgetProgress = 0f;
		this.sentryFireProgress = 0f;
		this.pulseProgress = 0f;
		this.UpdateMaterial();
		if (this.awarenessIcon != null)
		{
			this.awarenessIcon.Remove(false);
		}
	}

	// Token: 0x04001DA8 RID: 7592
	[Header("Security System Components")]
	public SecuritySystem.SecuritySystemType system;

	// Token: 0x04001DA9 RID: 7593
	public Animator anim;

	// Token: 0x04001DAA RID: 7594
	public GameObject laser;

	// Token: 0x04001DAB RID: 7595
	public Light laserLight;

	// Token: 0x04001DAC RID: 7596
	[Tooltip("Switch state")]
	public bool isActive;

	// Token: 0x04001DAD RID: 7597
	[Tooltip("Is the animation controller not at the end of an animation?")]
	public bool isAnimating;

	// Token: 0x04001DAE RID: 7598
	public Actor trackingTarget;

	// Token: 0x04001DAF RID: 7599
	public bool acquiredTarget;

	// Token: 0x04001DB0 RID: 7600
	public MeshRenderer rend;

	// Token: 0x04001DB1 RID: 7601
	public Transform rotationPivotTransform;

	// Token: 0x04001DB2 RID: 7602
	public Quaternion desiredPivotRotation;

	// Token: 0x04001DB3 RID: 7603
	public Transform selfTransform;

	// Token: 0x04001DB4 RID: 7604
	public Quaternion desiredSelfRotation;

	// Token: 0x04001DB5 RID: 7605
	public Transform muzzleTransform;

	// Token: 0x04001DB6 RID: 7606
	public float seekUpdateProgress;

	// Token: 0x04001DB7 RID: 7607
	public float forgetProgress;

	// Token: 0x04001DB8 RID: 7608
	private float pulseProgress;

	// Token: 0x04001DB9 RID: 7609
	private float focusFlashCounter;

	// Token: 0x04001DBA RID: 7610
	public List<NewAIController.TrackingTarget> activeTargets = new List<NewAIController.TrackingTarget>();

	// Token: 0x04001DBB RID: 7611
	public float sweepProgress = 0.5f;

	// Token: 0x04001DBC RID: 7612
	private InterfaceController.AwarenessIcon awarenessIcon;

	// Token: 0x04001DBD RID: 7613
	private float sentryFireProgress;

	// Token: 0x04001DBE RID: 7614
	[Header("Settings")]
	public AnimationCurve cameraSweep;

	// Token: 0x04001DBF RID: 7615
	[Tooltip("How much time in seconds before this sounds an alarm/fires")]
	public float focusGraceTime = 3.5f;

	// Token: 0x04001DC0 RID: 7616
	[Tooltip("How much time before this stops tracking a target that it has previously seen")]
	public float focusGiveUpTime = 5f;

	// Token: 0x0200042A RID: 1066
	public enum SecuritySystemType
	{
		// Token: 0x04001DC2 RID: 7618
		camera,
		// Token: 0x04001DC3 RID: 7619
		sentry
	}
}
