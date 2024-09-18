using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x0200081D RID: 2077
	public class FirstPersonController : MonoBehaviour
	{
		// Token: 0x060026AA RID: 9898 RVA: 0x00002265 File Offset: 0x00000465
		private void Start()
		{
		}

		// Token: 0x060026AB RID: 9899 RVA: 0x001F8338 File Offset: 0x001F6538
		public void InitialiseController(bool setOriginalCamPosition, bool initMouslook = true)
		{
			if (!CameraController.Instance.cam.enabled)
			{
				CameraController.Instance.cam.enabled = true;
			}
			this.m_CharacterController = base.GetComponent<CharacterController>();
			this.m_Camera = Camera.main;
			if (setOriginalCamPosition)
			{
				this.m_OriginalCameraPosition = this.m_Camera.transform.localPosition;
			}
			this.m_FovKick.Setup(this.m_Camera);
			this.m_HeadBob.Setup(this.m_Camera, this.m_StepInterval, this.m_OriginalCameraPosition);
			this.m_StepCycle = 0f;
			this.m_NextStep = this.m_StepCycle / 2f;
			this.m_Jumping = false;
			if (initMouslook)
			{
				this.m_MouseLook.Init(base.transform, this.m_Camera.transform);
			}
			this.fallCount = 0f;
			this.lastY = base.transform.position.y;
			this.syncTransforms = true;
			Player.Instance.UpdateMovementPhysics(true);
		}

		// Token: 0x060026AC RID: 9900 RVA: 0x001F843C File Offset: 0x001F663C
		private void Update()
		{
			if (this.enableLook)
			{
				this.RotateView();
			}
			if (SessionData.Instance.play && (CutSceneController.Instance == null || !CutSceneController.Instance.cutSceneActive))
			{
				if (!this.m_Jump && InputController.Instance.player != null)
				{
					this.m_Jump = InputController.Instance.player.GetButtonDown("Jump");
				}
				if (Game.Instance.updateMovementEveryFrame)
				{
					this.UpdateMovement();
				}
				if (!this.m_CharacterController.isGrounded && !this.ghostMovement)
				{
					float num = Mathf.Max(this.lastY - base.transform.position.y, 0f) * 0.1f;
					this.fallCount += num;
				}
				this.lastY = base.transform.position.y;
				if (!this.m_PreviouslyGrounded && this.m_CharacterController.isGrounded)
				{
					if (this.m_UseJumpBob)
					{
						base.StartCoroutine(this.m_JumpBob.DoBobCycle());
					}
					this.PlayLandingSound();
					this.m_MoveDir.y = 0f;
					this.m_Jumping = false;
					if (!this.ghostMovement)
					{
						bool flag = false;
						float num2 = Mathf.Max(this.fallCount - 0.4f, 0f) * GameplayControls.Instance.fallDamageMultiplier * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.fallDamageModifier));
						if (Player.Instance.spendingTimeMode)
						{
							num2 = 0f;
						}
						Game.Log("Player: Fall count: " + this.fallCount.ToString() + ", fall damage from mp: " + num2.ToString(), 2);
						if (this.fallCount >= 0.2f)
						{
							AudioController.Instance.PlayerPlayerImpactSound(this.fallCount);
						}
						if (!Game.Instance.disableFallDamage && num2 > 0.6f && Toolbox.Instance.Rand(0f, 1f, false) < num2)
						{
							AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.brokenBone, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
							Player.Instance.AddBrokenLeg(Toolbox.Instance.Rand(0.8f, 1f, false));
						}
						if (num2 > 0f)
						{
							Player.Instance.Trip(num2, true, false);
							flag = true;
						}
						if (!flag && Toolbox.Instance.Rand(0f, 1f, false) < StatusController.Instance.tripChanceDrunk)
						{
							bool forwards = false;
							if (Toolbox.Instance.Rand(0f, 1f, false) < 0.5f)
							{
								forwards = true;
							}
							Player.Instance.Trip(Toolbox.Instance.Rand(0.05f, 0.1f, false), forwards, true);
							flag = true;
						}
						if (!flag)
						{
							this.JoltCamera(Vector3.right, this.fallCount * 12f, 1.6f);
						}
						if (this.fallCount >= (PathFinder.Instance.nodeSize.z * 4f - 1f) * 0.1f && AchievementsController.Instance != null && Player.Instance.currentTile != null && (Player.Instance.currentTile.isStairwell || Player.Instance.currentTile.isInvertedStairwell))
						{
							AchievementsController.Instance.UnlockAchievement("Shafted", "fall_down_floors");
						}
					}
					this.fallCount = 0f;
				}
				if (!this.m_CharacterController.isGrounded && !this.m_Jumping && this.m_PreviouslyGrounded)
				{
					this.m_MoveDir.y = 0f;
				}
				this.m_PreviouslyGrounded = this.m_CharacterController.isGrounded;
			}
		}

		// Token: 0x060026AD RID: 9901 RVA: 0x001F87F0 File Offset: 0x001F69F0
		private void PlayLandingSound()
		{
			Game.Log("Player: Landed", 2);
			AudioController.Instance.PlayWorldFootstep(this.playerScript.footstepEvent, this.playerScript, false);
			this.m_NextStep = this.m_StepCycle + 0.5f;
		}

		// Token: 0x060026AE RID: 9902 RVA: 0x001F882C File Offset: 0x001F6A2C
		private void FixedUpdate()
		{
			if (!Game.Instance.updateMovementEveryFrame)
			{
				this.UpdateMovement();
			}
		}

		// Token: 0x060026AF RID: 9903 RVA: 0x001F8840 File Offset: 0x001F6A40
		public void PlayerOutOfWorldCheck()
		{
			if (float.IsNaN(base.transform.position.x) || float.IsNaN(base.transform.position.y) || float.IsNaN(base.transform.position.z) || base.transform.position.y < -30f || base.transform.position.y > 200f)
			{
				NewNode newNode = Player.Instance.currentNode;
				if (newNode == null)
				{
					newNode = Player.Instance.previousNode;
				}
				if (newNode == null)
				{
					newNode = Toolbox.Instance.FindClosestValidNodeToWorldPosition(base.transform.position, false, true, false, default(Vector3Int), false, 0, false, 200);
				}
				if (newNode != null)
				{
					Game.LogError("Player has reached too far up or down! Teleporting them to safety...", 2);
					Player.Instance.Teleport(newNode, null, true, false);
					Player.Instance.SetHealth(Player.Instance.currentHealth - 0.1f);
					return;
				}
				Game.LogError("Player has reached too far up or down! Unable to get node for teleporting, so killing them off (sorry)", 2);
				Player.Instance.SetHealth(0f);
				Player.Instance.TriggerPlayerKO(base.transform.forward, 0f, false);
			}
		}

		// Token: 0x060026B0 RID: 9904 RVA: 0x001F8978 File Offset: 0x001F6B78
		public void UpdateMovement()
		{
			if (!SessionData.Instance.startedGame)
			{
				return;
			}
			this.previousMovement = base.transform.position;
			float num = -this.maxLeanMovement;
			float num2 = this.maxLeanMovement;
			float num3 = Time.fixedDeltaTime;
			if (Game.Instance.updateMovementEveryFrame)
			{
				num3 = Time.smoothDeltaTime;
			}
			if (this.m_CharacterController.enabled)
			{
				if (this.leanState < 0)
				{
					Ray ray = new Ray(this.m_Camera.transform.position, -base.transform.right);
					Physics.SyncTransforms();
					RaycastHit raycastHit;
					if (Physics.Raycast(ray, ref raycastHit, this.maxLeanMovement + 0.22f + this.m_CharacterController.skinWidth, Toolbox.Instance.playerMovementLayerMask))
					{
						num = Mathf.Max(-Mathf.Max(0f, raycastHit.distance - 0.22f - this.m_CharacterController.skinWidth), -this.maxLeanMovement);
					}
				}
				else if (this.leanState > 0)
				{
					Ray ray2 = new Ray(this.m_Camera.transform.position, base.transform.right);
					Physics.SyncTransforms();
					RaycastHit raycastHit2;
					if (Physics.Raycast(ray2, ref raycastHit2, this.maxLeanMovement + 0.22f + this.m_CharacterController.skinWidth, Toolbox.Instance.playerMovementLayerMask))
					{
						num2 = Mathf.Min(Mathf.Max(0f, raycastHit2.distance - 0.22f - this.m_CharacterController.skinWidth), this.maxLeanMovement);
					}
				}
				float num4 = 0f;
				if (this.enableMovement)
				{
					this.GetInput(out num4);
					if (Player.Instance.transitionActive)
					{
						if (Player.Instance.currentTransition != null && !Player.Instance.currentTransition.retainMovementControl)
						{
							num4 = 0f;
						}
						else
						{
							float num5 = Mathf.Clamp01(Player.Instance.currentTransition.controlCurve.Evaluate(Player.Instance.transitionProgress));
							num4 *= num5;
						}
					}
					else if (Player.Instance.hurt > 0f)
					{
						num4 *= Mathf.Clamp01(1f - Player.Instance.hurt * 0.6f);
					}
					Vector3 vector = Vector3.zero;
					Vector2 vector2 = Vector2.zero;
					if (StatusController.Instance.drunkControls > 0f)
					{
						vector2 = SessionData.Instance.drunkOscillation * StatusController.Instance.drunkControls * 0.01f;
						vector = new Vector3(SessionData.Instance.drunkOscillation.x, SessionData.Instance.drunkOscillation.y, SessionData.Instance.drunkOscillation.y) * StatusController.Instance.drunkControls * 0.16f;
					}
					Vector3 vector3 = base.transform.forward;
					if (Player.Instance.autoTravelActive)
					{
						vector3 = Player.Instance.autoTravelForward;
						if (Player.Instance.spendingTimeMode && num4 > 0f)
						{
							num4 = 18f;
						}
					}
					Vector3 vector4 = (vector3 + vector) * this.m_Input.y + (base.transform.right + vector) * this.m_Input.x;
					if (!this.ghostMovement)
					{
						Physics.SphereCastNonAlloc(base.transform.position, this.m_CharacterController.radius, Vector3.down, this.hitInfoArray, this.m_CharacterController.height * 0.5f, Toolbox.Instance.playerMovementLayerMask, 1);
						if (this.hitInfoArray[0].collider != null)
						{
							vector4 = Vector3.ProjectOnPlane(vector4, this.hitInfoArray[0].normal).normalized;
						}
					}
					else
					{
						vector4 = (this.m_Camera.transform.forward * this.m_Input.y + this.m_Camera.transform.right * this.m_Input.x).normalized;
						this.m_MoveDir.y = vector4.y * num4;
					}
					this.m_MoveDir.x = vector4.x * num4 + vector2.x;
					this.m_MoveDir.z = vector4.z * num4 + vector2.y;
				}
				else
				{
					this.m_MoveDir.x = 0f;
					this.m_MoveDir.y = 0f;
					this.m_MoveDir.z = 0f;
				}
				if (!this.ghostMovement)
				{
					if (this.m_CharacterController.isGrounded)
					{
						this.m_MoveDir.y = -this.m_StickToGroundForce;
						if (this.m_Jump && !StatusController.Instance.disabledJump && !Player.Instance.transitionActive)
						{
							this.m_MoveDir.y = GameplayControls.Instance.jumpHeight;
							this.PlayJumpSound();
							this.m_Jump = false;
							this.m_Jumping = true;
						}
					}
					else
					{
						this.leanState = 0;
						this.m_MoveDir += Physics.gravity * this.m_GravityMultiplier * num3;
					}
				}
				if (this.clipping)
				{
					if (this.syncTransforms)
					{
						Physics.SyncTransforms();
						this.syncTransforms = false;
					}
					Vector3 position = base.transform.position;
					this.m_CollisionFlags = this.m_CharacterController.Move(this.m_MoveDir * num3);
					if (Player.Instance.transitionActive)
					{
						Player.Instance.originalPlayerPosition += base.transform.position - position;
					}
				}
				else
				{
					Vector3 vector5 = this.m_MoveDir * num3;
					base.transform.position += vector5;
				}
				Player.Instance.UpdateMovementPhysics(false);
			}
			else
			{
				this.leanState = 0;
			}
			if (this.leanProgress > (float)this.leanState)
			{
				this.leanProgress -= (this.leanSpeed + Player.Instance.extraLeanSpeed) * num3;
				if (this.leanState == 0 && this.leanProgress < 0f)
				{
					this.leanProgress = 0f;
				}
				this.leanProgress = Mathf.Clamp(this.leanProgress, -1f, 1f);
			}
			else if (this.leanProgress < (float)this.leanState)
			{
				this.leanProgress += (this.leanSpeed + Player.Instance.extraLeanSpeed) * num3;
				if (this.leanState == 0 && this.leanProgress > 0f)
				{
					this.leanProgress = 0f;
				}
				this.leanProgress = Mathf.Clamp(this.leanProgress, -1f, 1f);
			}
			this.currentLeanAngle = GameplayControls.Instance.leanCurve.Evaluate(this.leanProgress) * -this.maxLeanAngle;
			this.currentLeanMovement = Mathf.Clamp(GameplayControls.Instance.leanCurve.Evaluate(this.leanProgress) * this.maxLeanMovement, num, num2);
			this.leanPivot.localPosition = new Vector3(this.currentLeanMovement, 0f, 0f);
			this.leanPivot.localRotation = Quaternion.AngleAxis(this.currentLeanAngle, Vector3.forward);
			if (this.activeJolts.Count > 0)
			{
				Vector3 vector6 = Vector3.zero;
				for (int i = 0; i < this.activeJolts.Count; i++)
				{
					FirstPersonController.CameraJolt cameraJolt = this.activeJolts[i];
					vector6 += cameraJolt.direction * GameplayControls.Instance.joltCurve.Evaluate(cameraJolt.progress);
					cameraJolt.progress += num3 * cameraJolt.speed;
					if (cameraJolt.progress >= 1f)
					{
						this.activeJolts.RemoveAt(i);
						i--;
					}
				}
				this.leanPivot.localEulerAngles += vector6;
			}
			this.isMoving = false;
			if ((Mathf.Abs(this.m_Input.x) > 0f || Mathf.Abs(this.m_Input.y) > 0f) && (this.m_CollisionFlags == 4 || this.m_CollisionFlags == 4 || this.m_CollisionFlags == null))
			{
				this.isMoving = true;
			}
			this.ProgressStepCycle(this.speed, num3);
			this.UpdateCameraPosition(this.speed);
			this.movementThisUpdate = base.transform.position - this.previousMovement;
			if (this.isMoving != this.movementChange && Player.Instance.transform.position.y < CityControls.Instance.basementWaterLevel)
			{
				this.movementChange = this.isMoving;
				if (this.isMoving)
				{
					AudioController.Instance.PlayWorldFootstep(this.playerScript.footstepEvent, this.playerScript, this.rightFootNext);
				}
			}
			this.PlayerOutOfWorldCheck();
		}

		// Token: 0x060026B1 RID: 9905 RVA: 0x001F927F File Offset: 0x001F747F
		public void JoltCamera(Vector3 direction, float amplitude, float speed)
		{
			amplitude = Mathf.Min(amplitude, 70f);
			direction = direction.normalized * amplitude;
			this.activeJolts.Add(new FirstPersonController.CameraJolt(direction, speed));
		}

		// Token: 0x060026B2 RID: 9906 RVA: 0x00002265 File Offset: 0x00000465
		private void PlayJumpSound()
		{
		}

		// Token: 0x060026B3 RID: 9907 RVA: 0x001F92B0 File Offset: 0x001F74B0
		public void ProgressStepCycle(float speed, float deltaTime)
		{
			if (Player.Instance.crouchTransitionActive)
			{
				return;
			}
			if (InteractionController.Instance.lockedInInteraction != null && InteractionController.Instance.lockedInInteractionRef < 2)
			{
				return;
			}
			if (this.m_CharacterController.velocity.sqrMagnitude > 0f && (this.m_Input.x != 0f || this.m_Input.y != 0f))
			{
				if (Player.Instance.inAirVent)
				{
					this.m_StepCycle += 8f * deltaTime;
				}
				else
				{
					this.m_StepCycle += (this.m_CharacterController.velocity.magnitude + speed * (this.m_IsWalking ? 1f : this.m_RunstepLenghten)) * deltaTime;
				}
			}
			if (this.m_StepCycle <= this.m_NextStep)
			{
				return;
			}
			this.m_NextStep = this.m_StepCycle + this.m_StepInterval;
			AudioEvent audioEvent = null;
			if (Player.Instance.inAirVent)
			{
				audioEvent = AudioControls.Instance.playerFootstepDuct;
			}
			else if (this.m_CharacterController.isGrounded)
			{
				audioEvent = this.playerScript.footstepEvent;
			}
			if (audioEvent != null)
			{
				AudioController.Instance.PlayWorldFootstep(audioEvent, this.playerScript, this.rightFootNext);
			}
			this.rightFootNext = !this.rightFootNext;
			InterfaceController.Instance.footstepAudioIndicator.SetSoundEvent(audioEvent, false);
			InterfaceController.Instance.footstepAudioIndicator.UpdateCurrentEvent();
			if (InterfaceController.Instance.footstepAudioIndicator.juice != null && InterfaceController.Instance.footstepAudioIndicator.currentListeners.Count > 0)
			{
				InterfaceController.Instance.footstepAudioIndicator.juice.Nudge(new Vector2(1.05f, 1.05f), Vector2.zero, true, true, true);
			}
		}

		// Token: 0x060026B4 RID: 9908 RVA: 0x001F9480 File Offset: 0x001F7680
		public void UpdateCameraPosition(float speed)
		{
			if (Player.Instance.crouchTransitionActive || InteractionController.Instance.lockedInInteraction != null)
			{
				return;
			}
			Vector3 localPosition;
			if (this.m_CharacterController.velocity.magnitude > 0f && this.m_CharacterController.isGrounded)
			{
				if (this.enableHeadBob)
				{
					this.m_Camera.transform.localPosition = this.DoHeadBob(this.m_HeadBob, this.m_CharacterController.velocity.magnitude + speed * (this.m_IsWalking ? 1f : this.m_RunstepLenghten), GameplayControls.Instance.headBobMultiplier);
				}
				localPosition = this.m_Camera.transform.localPosition;
				if (this.m_UseJumpBob)
				{
					localPosition.y = this.m_Camera.transform.localPosition.y - this.m_JumpBob.Offset();
				}
			}
			else
			{
				localPosition = this.m_Camera.transform.localPosition;
				if (this.m_UseJumpBob)
				{
					localPosition.y = this.m_OriginalCameraPosition.y - this.m_JumpBob.Offset();
				}
			}
			this.m_Camera.transform.localPosition = localPosition;
		}

		// Token: 0x060026B5 RID: 9909 RVA: 0x001F95B8 File Offset: 0x001F77B8
		private void GetInput(out float speed)
		{
			float num = InputController.Instance.GetAxisRelative("MoveHorizontal");
			float num2 = InputController.Instance.GetAxisRelative("MoveVertical");
			if (Player.Instance.autoTravelActive)
			{
				num2 = 1f;
				num *= 0.5f;
			}
			if (num2 > 0.01f && InteractionController.Instance.currentlyDragging != null)
			{
				num2 = Mathf.Clamp(num2, 0f, 0.2f);
			}
			speed = Mathf.Clamp01(Vector2.Distance(Vector2.zero, new Vector2(num, num2) * 1.24f));
			this.leanState = 0;
			if (InputController.Instance != null && InputController.Instance.player != null && !Player.Instance.inAirVent && InteractionController.Instance.carryingObject == null && !Player.Instance.playerKOInProgress && !Player.Instance.transitionActive && !CutSceneController.Instance.cutSceneActive && (PlayerApartmentController.Instance == null || !PlayerApartmentController.Instance.furniturePlacementMode))
			{
				if (InputController.Instance.player.GetButton("LeanLeft"))
				{
					this.leanState = -1;
				}
				else if (InputController.Instance.player.GetButton("LeanRight"))
				{
					this.leanState = 1;
				}
			}
			this.leanState = Mathf.Clamp(this.leanState + Player.Instance.forcedLeanState, -1, 1);
			bool isWalking = this.m_IsWalking;
			bool flag = false;
			if (Player.Instance.transform.position.y < CityControls.Instance.basementWaterLevel)
			{
				flag = true;
			}
			if (!this.playerScript.isCrouched && !this.playerScript.inAirVent && InputController.Instance.player != null && !StatusController.Instance.disabledSprint && !flag && InteractionController.Instance.currentlyDragging == null)
			{
				if (Game.Instance.toggleRun)
				{
					if (InputController.Instance.player.GetButtonDown("Sprint"))
					{
						this.m_RunToggle = !this.m_RunToggle;
					}
					this.m_IsWalking = !this.m_RunToggle;
				}
				else
				{
					this.m_IsWalking = !InputController.Instance.player.GetButton("Sprint");
				}
				if (Game.Instance.alwaysRun)
				{
					this.m_IsWalking = !this.m_IsWalking;
				}
			}
			else
			{
				this.m_IsWalking = true;
			}
			if (this.playerScript.inAirVent)
			{
				speed *= this.m_WalkSpeed * 0.3f;
			}
			else
			{
				if (this.m_IsWalking)
				{
					speed *= this.m_WalkSpeed;
				}
				else
				{
					speed = this.m_RunSpeed;
				}
				if (flag)
				{
					speed *= 0.66f;
				}
				if (InteractionController.Instance.currentlyDragging != null)
				{
					speed *= 0.3f;
				}
			}
			speed *= StatusController.Instance.movementSpeedMultiplier;
			speed *= Game.Instance.movementSpeed;
			this.m_Input = new Vector2(num, num2);
			if (this.m_Input.sqrMagnitude > 1f)
			{
				this.m_Input.Normalize();
			}
			if (this.m_IsWalking != isWalking && this.m_UseFovKick && this.m_CharacterController.velocity.sqrMagnitude > 0f)
			{
				base.StopAllCoroutines();
				base.StartCoroutine((!this.m_IsWalking) ? this.m_FovKick.FOVKickUp() : this.m_FovKick.FOVKickDown());
			}
		}

		// Token: 0x060026B6 RID: 9910 RVA: 0x001F9935 File Offset: 0x001F7B35
		private void RotateView()
		{
			if (!Player.Instance.transitionActive && CameraController.Instance.cam.enabled)
			{
				this.m_MouseLook.LookRotation(base.transform, this.m_Camera.transform, false);
			}
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x001F9974 File Offset: 0x001F7B74
		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
			if (this.m_CollisionFlags == 4)
			{
				return;
			}
			if (attachedRigidbody == null || attachedRigidbody.isKinematic)
			{
				return;
			}
			attachedRigidbody.AddForceAtPosition(this.m_CharacterController.velocity * 0.1f, hit.point, 1);
		}

		// Token: 0x060026B8 RID: 9912 RVA: 0x001F99CC File Offset: 0x001F7BCC
		public Vector3 DoHeadBob(CurveControlledBob bob, float speed, float multiplier)
		{
			float num = bob.Bobcurve.Evaluate(bob.m_CyclePositionX);
			float num2 = bob.Bobcurve.Evaluate(bob.m_CyclePositionY);
			foreach (KeyValuePair<AnimationCurve, float> keyValuePair in StatusController.Instance.affectHeadBobs)
			{
				num += keyValuePair.Key.Evaluate(bob.m_CyclePositionX) * keyValuePair.Value;
				num2 += keyValuePair.Key.Evaluate(bob.m_CyclePositionY) * keyValuePair.Value;
			}
			float num3 = this.m_OriginalCameraPosition.x + num * bob.HorizontalBobRange * multiplier;
			float num4 = this.m_OriginalCameraPosition.y + num2 * bob.VerticalBobRange * multiplier;
			bob.m_CyclePositionX += speed * Time.deltaTime / bob.m_BobBaseInterval;
			bob.m_CyclePositionY += speed * Time.deltaTime / bob.m_BobBaseInterval * bob.VerticaltoHorizontalRatio;
			if (bob.m_CyclePositionX > bob.m_Time)
			{
				bob.m_CyclePositionX -= bob.m_Time;
			}
			if (bob.m_CyclePositionY > bob.m_Time)
			{
				bob.m_CyclePositionY -= bob.m_Time;
			}
			if (float.IsNaN(num3))
			{
				num3 = 0f;
			}
			if (float.IsNaN(num4))
			{
				num4 = 0f;
			}
			return new Vector3(num3, num4, 0f);
		}

		// Token: 0x04004534 RID: 17716
		public bool enableMovement = true;

		// Token: 0x04004535 RID: 17717
		public bool enableLook = true;

		// Token: 0x04004536 RID: 17718
		public bool isMoving;

		// Token: 0x04004537 RID: 17719
		public bool movementChange;

		// Token: 0x04004538 RID: 17720
		public bool enableHeadBob = true;

		// Token: 0x04004539 RID: 17721
		public bool ghostMovement;

		// Token: 0x0400453A RID: 17722
		public bool clipping = true;

		// Token: 0x0400453B RID: 17723
		public bool syncTransforms;

		// Token: 0x0400453C RID: 17724
		public Player playerScript;

		// Token: 0x0400453D RID: 17725
		[SerializeField]
		public bool m_IsWalking;

		// Token: 0x0400453E RID: 17726
		public bool m_RunToggle;

		// Token: 0x0400453F RID: 17727
		public float m_WalkSpeed;

		// Token: 0x04004540 RID: 17728
		public float m_RunSpeed;

		// Token: 0x04004541 RID: 17729
		public float speed;

		// Token: 0x04004542 RID: 17730
		[SerializeField]
		private float m_RunstepLenghten;

		// Token: 0x04004543 RID: 17731
		[SerializeField]
		public float m_StickToGroundForce;

		// Token: 0x04004544 RID: 17732
		[SerializeField]
		public float m_GravityMultiplier;

		// Token: 0x04004545 RID: 17733
		[SerializeField]
		public MouseLook m_MouseLook;

		// Token: 0x04004546 RID: 17734
		[SerializeField]
		private bool m_UseFovKick;

		// Token: 0x04004547 RID: 17735
		[SerializeField]
		private FOVKick m_FovKick = new FOVKick();

		// Token: 0x04004548 RID: 17736
		[SerializeField]
		public CurveControlledBob m_HeadBob = new CurveControlledBob();

		// Token: 0x04004549 RID: 17737
		public bool m_UseJumpBob;

		// Token: 0x0400454A RID: 17738
		[SerializeField]
		public LerpControlledBob m_JumpBob = new LerpControlledBob();

		// Token: 0x0400454B RID: 17739
		public float m_StepInterval;

		// Token: 0x0400454C RID: 17740
		private bool rightFootNext = true;

		// Token: 0x0400454D RID: 17741
		[SerializeField]
		private Transform leanPivot;

		// Token: 0x0400454E RID: 17742
		[SerializeField]
		private float leanSpeed = 10f;

		// Token: 0x0400454F RID: 17743
		[SerializeField]
		private float maxLeanAngle = 18f;

		// Token: 0x04004550 RID: 17744
		[SerializeField]
		private float maxLeanMovement = 0.2f;

		// Token: 0x04004551 RID: 17745
		public Camera m_Camera;

		// Token: 0x04004552 RID: 17746
		private bool m_Jump;

		// Token: 0x04004553 RID: 17747
		private float m_YRotation;

		// Token: 0x04004554 RID: 17748
		public Vector2 m_Input;

		// Token: 0x04004555 RID: 17749
		public Vector3 m_MoveDir = Vector3.zero;

		// Token: 0x04004556 RID: 17750
		public CharacterController m_CharacterController;

		// Token: 0x04004557 RID: 17751
		public CollisionFlags m_CollisionFlags;

		// Token: 0x04004558 RID: 17752
		private bool m_PreviouslyGrounded;

		// Token: 0x04004559 RID: 17753
		public Vector3 m_OriginalCameraPosition;

		// Token: 0x0400455A RID: 17754
		public float m_StepCycle;

		// Token: 0x0400455B RID: 17755
		public float m_NextStep;

		// Token: 0x0400455C RID: 17756
		public bool m_Jumping;

		// Token: 0x0400455D RID: 17757
		public int leanState;

		// Token: 0x0400455E RID: 17758
		public float leanProgress;

		// Token: 0x0400455F RID: 17759
		public float currentLeanAngle;

		// Token: 0x04004560 RID: 17760
		public float currentLeanMovement;

		// Token: 0x04004561 RID: 17761
		public List<FirstPersonController.CameraJolt> activeJolts = new List<FirstPersonController.CameraJolt>();

		// Token: 0x04004562 RID: 17762
		public float lastY;

		// Token: 0x04004563 RID: 17763
		public float fallCount;

		// Token: 0x04004564 RID: 17764
		private Vector3 previousMovement;

		// Token: 0x04004565 RID: 17765
		public Vector3 movementThisUpdate;

		// Token: 0x04004566 RID: 17766
		private RaycastHit[] hitInfoArray = new RaycastHit[1];

		// Token: 0x0200081E RID: 2078
		public class CameraJolt
		{
			// Token: 0x060026BA RID: 9914 RVA: 0x001F9BEE File Offset: 0x001F7DEE
			public CameraJolt(Vector3 newDirection, float newSpeed)
			{
				this.direction = newDirection;
				this.speed = newSpeed;
			}

			// Token: 0x04004567 RID: 17767
			public Vector3 direction;

			// Token: 0x04004568 RID: 17768
			public float progress;

			// Token: 0x04004569 RID: 17769
			public float speed;
		}
	}
}
