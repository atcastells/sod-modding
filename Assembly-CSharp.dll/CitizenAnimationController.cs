using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class CitizenAnimationController : MonoBehaviour
{
	// Token: 0x060006FD RID: 1789 RVA: 0x0006A048 File Offset: 0x00068248
	public void ForceUpdateAnimationSate(bool onBecomeVisibile = false)
	{
		if (this.mainAnimator == null)
		{
			return;
		}
		this.FlipAnimationToRight(this.flipToRightAnimation);
		this.UpdateMovementSpeed();
		if (onBecomeVisibile)
		{
			this.mainAnimator.SetFloat("combatSkill", this.cit.combatSkill);
			this.SetIdleAnimationState(this.idleAnimationState);
			this.SetArmsBoolState(this.armsBoolAnimationState);
			if (this.cit.ai != null)
			{
				this.SetCombatArmsOverride(this.cit.ai.combatMode);
			}
			this.SetInBed(this.cit.isInBed, onBecomeVisibile);
			this.SetInCombat(this.cit.ai.inCombat);
			this.SetDead(this.cit.isDead);
			this.SetUmbrella(this.umbrella);
		}
		if (this.cit.ai != null && this.cit.ai.restrained)
		{
			this.mainAnimator.SetTrigger("restrain");
		}
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x0006A154 File Offset: 0x00068354
	public void UpdateMovementSpeed()
	{
		if (this.mainAnimator != null)
		{
			this.mainAnimator.SetFloat("moveSpeed", this.cit.currentNormalizedSpeed);
			this.mainAnimator.SetFloat("walkAnimSpeed", Mathf.Clamp(this.cit.currentMovementSpeed, 2f, 3f));
			this.mainAnimator.SetFloat("drunk", this.cit.drunk);
			this.debugMainAnimatorSpeed = this.cit.currentNormalizedSpeed;
		}
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x0006A1E0 File Offset: 0x000683E0
	public void SetArmsBoolState(CitizenAnimationController.ArmsBoolSate newState)
	{
		if (newState == CitizenAnimationController.ArmsBoolSate.armsConsuming && (this.cit.ai.spawnedRightItem == null || (this.mainAnimator != null && !this.mainAnimator.GetBool("carryingItem"))))
		{
			newState = CitizenAnimationController.ArmsBoolSate.none;
		}
		if (this.cit.ai.restrained && !this.cit.isDead)
		{
			newState = CitizenAnimationController.ArmsBoolSate.armsCuffed;
		}
		this.armsBoolAnimationState = newState;
		if (Game.Instance.collectDebugData)
		{
			this.cit.SelectedDebug("Set arms state: " + newState.ToString(), Actor.HumanDebug.actions);
		}
		if (this.armsBoolAnimationState == CitizenAnimationController.ArmsBoolSate.none)
		{
			this.armsLayerDesiredWeight = 0f;
			if (this.cit.ai != null)
			{
				if (this.cit.ai.inCombat && this.cit.ai.combatMode > 0)
				{
					this.armsLayerDesiredWeight = 1f;
				}
				else if (this.cit.ai.usingCarryAnimation && !this.cit.ai.attackActive)
				{
					this.armsLayerDesiredWeight = 1f;
				}
			}
			if (this.mainAnimator != null)
			{
				this.mainAnimator.SetInteger("ArmsState", 0);
			}
		}
		else
		{
			this.armsLayerDesiredWeight = 1f;
			if (this.mainAnimator != null)
			{
				this.mainAnimator.SetInteger("ArmsState", (int)newState);
			}
			if (this.armsBoolAnimationState == CitizenAnimationController.ArmsBoolSate.armsOneShotUse)
			{
				this.oneShotUseReset = 0.5f;
			}
		}
		if (!this.cit.visible)
		{
			if (this.mainAnimator != null)
			{
				this.mainAnimator.SetLayerWeight(1, this.armsLayerDesiredWeight);
				return;
			}
		}
		else if (this.cit.ai != null)
		{
			this.cit.ai.SetUpdateEnabled(true);
		}
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x0006A3C0 File Offset: 0x000685C0
	public void SetUmbrella(bool val)
	{
		if (this.cit.ai.inCombat || (this.cit.ai.restrained && val))
		{
			return;
		}
		if (this.umbrella != val)
		{
			this.umbrella = val;
			if (this.umbrella)
			{
				this.umbreallLayerDesiredWeight = 1f;
				if (this.spawnedUmbrella == null)
				{
					this.spawnedUmbrella = Toolbox.Instance.SpawnObject(CitizenControls.Instance.umbrella, this.cit.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandLeft));
					this.umbrellaCanopy = this.spawnedUmbrella.transform.GetChild(0);
					this.spawnedUmbrella.transform.localPosition = Vector3.zero;
					this.umbrellaCanopy.localScale = new Vector3(0.15f, 0.15f, 3f);
					this.cit.AddMesh(this.spawnedUmbrella, true, false);
				}
			}
			else
			{
				this.umbreallLayerDesiredWeight = 0f;
			}
			if (!this.cit.visible)
			{
				this.mainAnimator.SetLayerWeight(2, this.umbreallLayerDesiredWeight);
				if (this.spawnedUmbrella != null)
				{
					this.umbrellaCanopy.localScale = Vector3.Lerp(new Vector3(0.15f, 0.15f, 3f), Vector3.one, this.umbreallLayerDesiredWeight);
					if (!this.umbrella)
					{
						Object.Destroy(this.spawnedUmbrella);
						this.cit.UpdateMeshList();
						return;
					}
				}
			}
			else if (this.cit.ai != null)
			{
				this.cit.ai.SetUpdateEnabled(true);
			}
		}
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x0006A564 File Offset: 0x00068764
	public void SetCarryingItem(bool val)
	{
		this.mainAnimator.SetBool("carryingItem", val);
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x0006A577 File Offset: 0x00068777
	public void SetCarryItemType(int carryType)
	{
		this.mainAnimator.SetInteger("carryItem", carryType);
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0006A58C File Offset: 0x0006878C
	public void SetInCombat(bool val)
	{
		if (val)
		{
			this.mainAnimator.cullingMode = 0;
			this.SetInBed(false, false);
			this.SetUmbrella(false);
			if (this.cit.ai != null && this.cit.ai.combatMode > 0)
			{
				this.armsLayerDesiredWeight = 1f;
				if (Game.Instance.collectDebugData)
				{
					this.cit.SelectedDebug("Set animator arms weight: " + this.armsLayerDesiredWeight.ToString(), Actor.HumanDebug.misc);
				}
			}
		}
		else
		{
			this.mainAnimator.cullingMode = 2;
		}
		this.mainAnimator.SetBool("inCombat", val);
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x0006A634 File Offset: 0x00068834
	public void SetCombatArmsOverride(int val)
	{
		if (Game.Instance.collectDebugData)
		{
			this.cit.SelectedDebug("Set combat mode " + val.ToString(), Actor.HumanDebug.attacks);
		}
		this.mainAnimator.SetInteger("combatArmsOverride", val);
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0006A670 File Offset: 0x00068870
	public void SetRestrained(bool val)
	{
		if (val)
		{
			if (!this.cit.ai.isRagdoll)
			{
				this.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.armsCuffed);
			}
		}
		else
		{
			this.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
		}
		this.mainAnimator.SetBool("restrained", val);
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x0006A6A8 File Offset: 0x000688A8
	public void SetIdleAnimationState(CitizenAnimationController.IdleAnimationState newState)
	{
		if (newState == CitizenAnimationController.IdleAnimationState.none)
		{
			this.cit.ai.staticAnimationSafetyTimer = 0f;
			this.cit.ai.SetStaticFromAnimation(false);
		}
		this.idleAnimationState = newState;
		this.mainAnimator.SetInteger("IdleAnimationState", (int)this.idleAnimationState);
		this.cit.SelectedDebug("Set idle state " + newState.ToString(), Actor.HumanDebug.misc);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x0006A720 File Offset: 0x00068920
	public void SetInBed(bool val, bool instant = false)
	{
		this.mainAnimator.SetBool("isInBed", val);
		if (instant && val)
		{
			if (!this.flipToRightAnimation)
			{
				this.mainAnimator.Play("GetIntoBedLeft", 0, 1f);
				return;
			}
			this.mainAnimator.Play("GetIntoBedRight", 0, 1f);
		}
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x0006A778 File Offset: 0x00068978
	public void FlipAnimationToRight(bool val)
	{
		this.flipToRightAnimation = val;
		this.mainAnimator.SetBool("flipAnimationRight", val);
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x0006A792 File Offset: 0x00068992
	public void SetDead(bool val)
	{
		this.mainAnimator.SetBool("isDead", val);
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x0006A7A5 File Offset: 0x000689A5
	public void TriggerTrip()
	{
		this.mainAnimator.SetBool("trip", true);
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x0006A7B8 File Offset: 0x000689B8
	public void CancelTrip()
	{
		if (this.mainAnimator != null)
		{
			this.mainAnimator.SetBool("trip", false);
		}
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x0006A7DC File Offset: 0x000689DC
	public void AttackTrigger()
	{
		this.mainAnimator.cullingMode = 0;
		float num = Toolbox.Instance.Rand(0f, 1f, false);
		if (num >= 0.5f || this.cit.ai.combatMode > 0)
		{
			if (Game.Instance.collectDebugData)
			{
				this.cit.SelectedDebug("Trigger attack!", Actor.HumanDebug.attacks);
			}
			this.mainAnimator.SetTrigger("attack");
			return;
		}
		if (num >= 0.25f)
		{
			if (Game.Instance.collectDebugData)
			{
				this.cit.SelectedDebug("Trigger attack2!", Actor.HumanDebug.attacks);
			}
			this.mainAnimator.SetTrigger("attack2");
			return;
		}
		if (Game.Instance.collectDebugData)
		{
			this.cit.SelectedDebug("Trigger kick!", Actor.HumanDebug.attacks);
		}
		this.mainAnimator.SetTrigger("kick");
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x0006A8B7 File Offset: 0x00068AB7
	public void ThrowTrigger()
	{
		if (Game.Instance.collectDebugData)
		{
			this.cit.SelectedDebug("Throw attack!", Actor.HumanDebug.attacks);
		}
		this.mainAnimator.SetTrigger("throw");
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x0006A8E6 File Offset: 0x00068AE6
	public void AbortAttackTrigger()
	{
		if (Game.Instance.collectDebugData)
		{
			this.cit.SelectedDebug("Abort attack!", Actor.HumanDebug.attacks);
		}
		this.mainAnimator.SetTrigger("abortAttack");
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x0006A915 File Offset: 0x00068B15
	public void BlockTrigger(float blockDelay, bool perfect = false)
	{
		if (perfect)
		{
			this.mainAnimator.SetTrigger("blockPerfect");
			return;
		}
		this.mainAnimator.SetTrigger("block");
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x0006A93C File Offset: 0x00068B3C
	public void TakeDamageRecoil(Vector3 hitPosition)
	{
		Vector3 vector = this.cit.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.upperTorso).InverseTransformPoint(hitPosition);
		string text = "AI: Incoming damage relative to UpperTorso: ";
		Vector3 vector2 = vector;
		Game.Log(text + vector2.ToString(), 2);
		if (this.mainAnimator != null)
		{
			this.mainAnimator.SetFloat("hitX", vector.x);
			this.mainAnimator.SetFloat("hitY", vector.y);
			this.mainAnimator.SetTrigger("hit");
		}
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x0006A9CC File Offset: 0x00068BCC
	public void SetPauseAnimation(bool val)
	{
		this.paused = val;
		if (this.paused)
		{
			if (this.mainAnimator != null)
			{
				this.mainAnimator.enabled = false;
			}
		}
		else if (this.mainAnimator != null)
		{
			this.mainAnimator.enabled = true;
		}
		this.ForceUpdateAnimationSate(true);
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x0006AA28 File Offset: 0x00068C28
	public void SetRagdoll(bool val, bool dead = false)
	{
		if (this.cit.ai.isRagdoll != val)
		{
			Game.Log(string.Concat(new string[]
			{
				"AI: ",
				this.cit.GetCitizenName(),
				"  Set ragdoll ",
				val.ToString(),
				" (dead: ",
				dead.ToString(),
				")"
			}), 2);
			this.cit.ai.isRagdoll = val;
			if (val)
			{
				if (!GameplayController.Instance.activeRagdolls.Contains(this.cit))
				{
					GameplayController.Instance.activeRagdolls.Add(this.cit);
					Player.Instance.UpdateCulling();
				}
				this.cit.ai.SetUpdateEnabled(true);
				this.cit.ai.SetExpression(CitizenOutfitController.Expression.asleep);
				this.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
				foreach (CitizenOutfitController.AnchorConfig anchorConfig in this.cit.outfitController.anchorConfig)
				{
					if (anchorConfig.anchor != CitizenOutfitController.CharacterAnchor.ArmsParent && anchorConfig.anchor != CitizenOutfitController.CharacterAnchor.Hair && anchorConfig.anchor != CitizenOutfitController.CharacterAnchor.beard)
					{
						CitizenAnimationController.CitizenPhysics citizenPhysics = new CitizenAnimationController.CitizenPhysics();
						citizenPhysics.anchorConfig = anchorConfig;
						if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Hat || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Glasses || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.HandLeft || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.HandRight || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LeftFoot || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.RightFoot)
						{
							using (List<CitizenOutfitController.OutfitClothes>.Enumerator enumerator2 = this.cit.outfitController.currentlyLoadedClothes.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									CitizenOutfitController.OutfitClothes outfitClothes = enumerator2.Current;
									if (outfitClothes.spawned.ContainsKey(anchorConfig.anchor))
									{
										foreach (MeshRenderer meshRenderer in outfitClothes.spawned[anchorConfig.anchor])
										{
											MeshFilter component = meshRenderer.gameObject.GetComponent<MeshFilter>();
											if (!(component == null))
											{
												Collider component2 = anchorConfig.trans.gameObject.GetComponent<Collider>();
												if (component2 != null && (component2.name != "HandLeft" || component2.name != "HandRight"))
												{
													Object.Destroy(component2);
												}
												MeshCollider meshCollider = anchorConfig.trans.gameObject.AddComponent<MeshCollider>();
												meshCollider.sharedMesh = component.sharedMesh;
												meshCollider.convex = true;
												citizenPhysics.coll = meshCollider;
												if (meshCollider.gameObject.name != "HandLeft" && meshCollider.gameObject.name != "HandRight")
												{
													this.createdColliders.Add(citizenPhysics.coll);
													break;
												}
												break;
											}
										}
									}
								}
								goto IL_55F;
							}
							goto IL_2F9;
						}
						goto IL_2F9;
						IL_55F:
						if (citizenPhysics.rb == null)
						{
							citizenPhysics.rb = anchorConfig.trans.gameObject.GetComponent<Rigidbody>();
						}
						if (citizenPhysics.rb == null)
						{
							citizenPhysics.rb = anchorConfig.trans.gameObject.AddComponent<Rigidbody>();
						}
						if (citizenPhysics.rb != null)
						{
							citizenPhysics.rb.collisionDetectionMode = 1;
							citizenPhysics.rb.interpolation = 1;
							citizenPhysics.rb.mass = anchorConfig.weight;
							citizenPhysics.rb.angularDrag = 0.15f;
							citizenPhysics.rb.detectCollisions = GameplayControls.Instance.ragdollRigidbodyCollision;
							this.createdRBs.Add(citizenPhysics.rb);
						}
						this.physicsComponents.Add(anchorConfig.anchor, citizenPhysics);
						if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.lowerTorso)
						{
							this.sfx = anchorConfig.trans.gameObject.AddComponent<RagdollSFXController>();
							this.sfx.actor = this.cit;
							continue;
						}
						if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.upperTorso && citizenPhysics.rb != null)
						{
							this.upperTorsoRB = citizenPhysics.rb;
							continue;
						}
						continue;
						IL_2F9:
						CapsuleCollider capsuleCollider = anchorConfig.trans.gameObject.AddComponent<CapsuleCollider>();
						if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Head)
						{
							capsuleCollider.center = new Vector3(0f, 0.15f, 0.05f);
							capsuleCollider.radius = 0.1f;
							capsuleCollider.height = 0.33f;
						}
						else if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.upperTorso)
						{
							capsuleCollider.center = new Vector3(0f, 0.15f, 0.02f);
							capsuleCollider.radius = 0.18f;
							capsuleCollider.height = 0.45f;
							capsuleCollider.direction = 0;
						}
						else if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Midriff)
						{
							capsuleCollider.center = new Vector3(0f, 0f, 0.01f);
							capsuleCollider.radius = 0.14f;
							capsuleCollider.height = 0.33f;
							capsuleCollider.direction = 0;
						}
						else if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.lowerTorso)
						{
							capsuleCollider.center = new Vector3(0f, 0.18f, 0f);
							capsuleCollider.radius = 0.18f;
							capsuleCollider.height = 0.4f;
							capsuleCollider.direction = 0;
						}
						if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperArmLeft || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperArmRight)
						{
							capsuleCollider.center = new Vector3(0f, -0.12f, 0.025f);
							capsuleCollider.radius = 0.05f;
							capsuleCollider.height = 0.35f;
						}
						if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerArmLeft || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerArmRight)
						{
							capsuleCollider.center = new Vector3(0f, -0.1f, 0.025f);
							capsuleCollider.radius = 0.05f;
							capsuleCollider.height = 0.28f;
						}
						if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperLegLeft || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperLegRight)
						{
							capsuleCollider.center = new Vector3(0.11f, -0.15f, 0.02f);
							capsuleCollider.radius = 0.12f;
							capsuleCollider.height = 0.45f;
						}
						if (anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerLegLeft || anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerLegRight)
						{
							capsuleCollider.center = new Vector3(0f, -0.16f, -0.02f);
							capsuleCollider.radius = 0.05f;
							capsuleCollider.height = 0.4f;
						}
						citizenPhysics.coll = capsuleCollider;
						this.createdColliders.Add(citizenPhysics.coll);
						goto IL_55F;
					}
				}
				if (this.cit.interactableController != null && this.cit.interactableController.coll != null)
				{
					this.cit.interactableController.coll.enabled = false;
					if (!this.cit.isDead)
					{
						this.newBoxCollider = this.cit.interactableController.gameObject.AddComponent<BoxCollider>();
						this.newBoxCollider.isTrigger = true;
						this.cit.interactableController.coll = this.newBoxCollider;
					}
				}
				CitizenAnimationController.CitizenPhysics citizenPhysics2 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics3 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics4 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics5 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics6 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics7 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics8 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics9 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics10 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics11 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics12 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics13 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics14 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics15 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics16 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics17 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics18 = null;
				CitizenAnimationController.CitizenPhysics citizenPhysics19 = null;
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.Hat, ref citizenPhysics2);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.Glasses, ref citizenPhysics3);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.Head, ref citizenPhysics4);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.upperTorso, ref citizenPhysics5);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.Midriff, ref citizenPhysics7);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.lowerTorso, ref citizenPhysics6);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.UpperArmRight, ref citizenPhysics8);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.UpperArmLeft, ref citizenPhysics10);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.LowerArmRight, ref citizenPhysics9);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.LowerArmLeft, ref citizenPhysics11);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.HandRight, ref citizenPhysics12);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.HandLeft, ref citizenPhysics13);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.UpperLegRight, ref citizenPhysics14);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.UpperLegLeft, ref citizenPhysics16);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.LowerLegRight, ref citizenPhysics15);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.LowerLegLeft, ref citizenPhysics17);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.RightFoot, ref citizenPhysics18);
				this.physicsComponents.TryGetValue(CitizenOutfitController.CharacterAnchor.LeftFoot, ref citizenPhysics19);
				if (citizenPhysics4 != null && citizenPhysics5 != null && citizenPhysics4.coll != null)
				{
					this.headJoint = citizenPhysics4.coll.transform.gameObject.AddComponent<CharacterJoint>();
					if (this.headJoint != null)
					{
						this.headJoint.anchor = this.headJoint.transform.InverseTransformPoint(citizenPhysics4.anchorConfig.trans.position);
						this.headJoint.connectedBody = citizenPhysics5.rb;
						this.headJoint.axis = new Vector3(1f, 0f, 0f);
						CharacterJoint characterJoint = this.headJoint;
						SoftJointLimit softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = -10f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint.lowTwistLimit = softJointLimit;
						CharacterJoint characterJoint2 = this.headJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 40f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint2.highTwistLimit = softJointLimit;
						CharacterJoint characterJoint3 = this.headJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 0f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint3.swing1Limit = softJointLimit;
						CharacterJoint characterJoint4 = this.headJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 30f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint4.swing2Limit = softJointLimit;
						CharacterJoint characterJoint5 = this.headJoint;
						SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
						softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
						softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
						characterJoint5.twistLimitSpring = softJointLimitSpring;
						CharacterJoint characterJoint6 = this.headJoint;
						softJointLimitSpring = default(SoftJointLimitSpring);
						softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
						softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
						characterJoint6.swingLimitSpring = softJointLimitSpring;
						this.ApplyRagdollJointSettings(ref this.headJoint);
						this.createdJoints.Add(this.headJoint);
					}
				}
				if (citizenPhysics5 != null && citizenPhysics7 != null)
				{
					this.upperTorsoJoint = citizenPhysics5.coll.transform.gameObject.AddComponent<CharacterJoint>();
					if (this.upperTorsoJoint != null)
					{
						this.upperTorsoJoint.anchor = this.upperTorsoJoint.transform.InverseTransformPoint(citizenPhysics5.anchorConfig.trans.position);
						this.upperTorsoJoint.connectedBody = citizenPhysics7.rb;
						this.upperTorsoJoint.axis = new Vector3(1f, 0f, 0f);
						CharacterJoint characterJoint7 = this.upperTorsoJoint;
						SoftJointLimit softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = -15f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint7.lowTwistLimit = softJointLimit;
						CharacterJoint characterJoint8 = this.upperTorsoJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 15f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint8.highTwistLimit = softJointLimit;
						CharacterJoint characterJoint9 = this.upperTorsoJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 0f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint9.swing1Limit = softJointLimit;
						CharacterJoint characterJoint10 = this.upperTorsoJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 20f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint10.swing2Limit = softJointLimit;
						CharacterJoint characterJoint11 = this.upperTorsoJoint;
						SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
						softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
						softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
						characterJoint11.twistLimitSpring = softJointLimitSpring;
						CharacterJoint characterJoint12 = this.upperTorsoJoint;
						softJointLimitSpring = default(SoftJointLimitSpring);
						softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
						softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
						characterJoint12.swingLimitSpring = softJointLimitSpring;
						this.ApplyRagdollJointSettings(ref this.upperTorsoJoint);
						this.createdJoints.Add(this.upperTorsoJoint);
					}
				}
				if (citizenPhysics7 != null && citizenPhysics6 != null && citizenPhysics7.coll != null)
				{
					this.midriffJoint = citizenPhysics7.coll.transform.gameObject.AddComponent<CharacterJoint>();
					if (this.midriffJoint != null)
					{
						this.midriffJoint.anchor = this.midriffJoint.transform.InverseTransformPoint(citizenPhysics7.anchorConfig.trans.position) - new Vector3(0f, -0.05f, 0f);
						this.midriffJoint.connectedBody = citizenPhysics6.rb;
						this.midriffJoint.axis = new Vector3(1f, 0f, 0f);
						CharacterJoint characterJoint13 = this.midriffJoint;
						SoftJointLimit softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = -45f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint13.lowTwistLimit = softJointLimit;
						CharacterJoint characterJoint14 = this.midriffJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 45f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint14.highTwistLimit = softJointLimit;
						CharacterJoint characterJoint15 = this.midriffJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 20f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint15.swing1Limit = softJointLimit;
						CharacterJoint characterJoint16 = this.midriffJoint;
						softJointLimit = default(SoftJointLimit);
						softJointLimit.limit = 20f;
						softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
						softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
						characterJoint16.swing2Limit = softJointLimit;
						CharacterJoint characterJoint17 = this.midriffJoint;
						SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
						softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
						softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
						characterJoint17.twistLimitSpring = softJointLimitSpring;
						CharacterJoint characterJoint18 = this.midriffJoint;
						softJointLimitSpring = default(SoftJointLimitSpring);
						softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
						softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
						characterJoint18.swingLimitSpring = softJointLimitSpring;
						this.ApplyRagdollJointSettings(ref this.midriffJoint);
						this.createdJoints.Add(this.midriffJoint);
					}
				}
				if (citizenPhysics5 != null)
				{
					if (citizenPhysics10 != null && citizenPhysics10.coll != null)
					{
						this.leftUpperArmJoint = citizenPhysics10.coll.transform.gameObject.AddComponent<CharacterJoint>();
						if (this.leftUpperArmJoint != null)
						{
							this.leftUpperArmJoint.anchor = this.leftUpperArmJoint.transform.InverseTransformPoint(citizenPhysics10.anchorConfig.trans.position);
							this.leftUpperArmJoint.connectedBody = citizenPhysics5.rb;
							this.leftUpperArmJoint.axis = new Vector3(1f, 0f, 0f);
							CharacterJoint characterJoint19 = this.leftUpperArmJoint;
							SoftJointLimit softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = -110f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint19.lowTwistLimit = softJointLimit;
							CharacterJoint characterJoint20 = this.leftUpperArmJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 110f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint20.highTwistLimit = softJointLimit;
							CharacterJoint characterJoint21 = this.leftUpperArmJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 45f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint21.swing1Limit = softJointLimit;
							CharacterJoint characterJoint22 = this.leftUpperArmJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 45f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint22.swing2Limit = softJointLimit;
							CharacterJoint characterJoint23 = this.leftUpperArmJoint;
							SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
							softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
							softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
							characterJoint23.twistLimitSpring = softJointLimitSpring;
							CharacterJoint characterJoint24 = this.leftUpperArmJoint;
							softJointLimitSpring = default(SoftJointLimitSpring);
							softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
							softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
							characterJoint24.swingLimitSpring = softJointLimitSpring;
							this.ApplyRagdollJointSettings(ref this.leftUpperArmJoint);
							this.createdJoints.Add(this.leftUpperArmJoint);
						}
						if (citizenPhysics11 != null && citizenPhysics11.coll != null)
						{
							this.leftLowerArmJoint = citizenPhysics11.coll.transform.gameObject.AddComponent<CharacterJoint>();
							if (this.leftLowerArmJoint != null)
							{
								this.leftLowerArmJoint.anchor = this.leftLowerArmJoint.transform.InverseTransformPoint(citizenPhysics11.anchorConfig.trans.position);
								this.leftLowerArmJoint.connectedBody = citizenPhysics10.rb;
								this.leftLowerArmJoint.axis = new Vector3(1f, 0f, 0f);
								CharacterJoint characterJoint25 = this.leftLowerArmJoint;
								SoftJointLimit softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = -10f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint25.lowTwistLimit = softJointLimit;
								CharacterJoint characterJoint26 = this.leftLowerArmJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 100f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint26.highTwistLimit = softJointLimit;
								CharacterJoint characterJoint27 = this.leftLowerArmJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 40f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint27.swing1Limit = softJointLimit;
								CharacterJoint characterJoint28 = this.leftLowerArmJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 25f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint28.swing2Limit = softJointLimit;
								CharacterJoint characterJoint29 = this.leftLowerArmJoint;
								SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
								softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
								softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
								characterJoint29.twistLimitSpring = softJointLimitSpring;
								CharacterJoint characterJoint30 = this.leftLowerArmJoint;
								softJointLimitSpring = default(SoftJointLimitSpring);
								softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
								softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
								characterJoint30.swingLimitSpring = softJointLimitSpring;
								this.ApplyRagdollJointSettings(ref this.leftLowerArmJoint);
								this.createdJoints.Add(this.leftLowerArmJoint);
							}
							if (citizenPhysics13 != null && citizenPhysics13.coll != null)
							{
								this.leftHandJoint = citizenPhysics13.coll.transform.gameObject.AddComponent<CharacterJoint>();
								if (this.leftHandJoint != null)
								{
									this.leftHandJoint.anchor = this.leftHandJoint.transform.InverseTransformPoint(citizenPhysics13.anchorConfig.trans.position);
									this.leftHandJoint.connectedBody = citizenPhysics11.rb;
									this.leftHandJoint.axis = new Vector3(0f, 0f, 1f);
									CharacterJoint characterJoint31 = this.leftHandJoint;
									SoftJointLimit softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = -80f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint31.lowTwistLimit = softJointLimit;
									CharacterJoint characterJoint32 = this.leftHandJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 80f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint32.highTwistLimit = softJointLimit;
									CharacterJoint characterJoint33 = this.leftHandJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 20f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint33.swing1Limit = softJointLimit;
									CharacterJoint characterJoint34 = this.leftHandJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 0f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint34.swing2Limit = softJointLimit;
									CharacterJoint characterJoint35 = this.leftHandJoint;
									SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
									softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
									softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
									characterJoint35.twistLimitSpring = softJointLimitSpring;
									CharacterJoint characterJoint36 = this.leftHandJoint;
									softJointLimitSpring = default(SoftJointLimitSpring);
									softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
									softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
									characterJoint36.swingLimitSpring = softJointLimitSpring;
									this.ApplyRagdollJointSettings(ref this.leftHandJoint);
									this.createdJoints.Add(this.leftHandJoint);
								}
							}
						}
					}
					if (citizenPhysics8 != null && citizenPhysics8.coll != null)
					{
						this.rightUpperArmJoint = citizenPhysics8.coll.transform.gameObject.AddComponent<CharacterJoint>();
						if (this.rightUpperArmJoint != null)
						{
							this.rightUpperArmJoint.anchor = this.rightUpperArmJoint.transform.InverseTransformPoint(citizenPhysics8.anchorConfig.trans.position);
							this.rightUpperArmJoint.connectedBody = citizenPhysics5.rb;
							this.rightUpperArmJoint.axis = new Vector3(1f, 0f, 0f);
							CharacterJoint characterJoint37 = this.rightUpperArmJoint;
							SoftJointLimit softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = -110f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint37.lowTwistLimit = softJointLimit;
							CharacterJoint characterJoint38 = this.rightUpperArmJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 110f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint38.highTwistLimit = softJointLimit;
							CharacterJoint characterJoint39 = this.rightUpperArmJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 45f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint39.swing1Limit = softJointLimit;
							CharacterJoint characterJoint40 = this.rightUpperArmJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 45f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint40.swing2Limit = softJointLimit;
							CharacterJoint characterJoint41 = this.rightUpperArmJoint;
							SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
							softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
							softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
							characterJoint41.twistLimitSpring = softJointLimitSpring;
							CharacterJoint characterJoint42 = this.rightUpperArmJoint;
							softJointLimitSpring = default(SoftJointLimitSpring);
							softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
							softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
							characterJoint42.swingLimitSpring = softJointLimitSpring;
							this.ApplyRagdollJointSettings(ref this.rightUpperArmJoint);
							this.createdJoints.Add(this.rightUpperArmJoint);
						}
						if (citizenPhysics9 != null && citizenPhysics9.coll != null)
						{
							this.rightLowerArmJoint = citizenPhysics9.coll.transform.gameObject.AddComponent<CharacterJoint>();
							if (this.rightLowerArmJoint != null)
							{
								this.rightLowerArmJoint.anchor = this.rightLowerArmJoint.transform.InverseTransformPoint(citizenPhysics9.anchorConfig.trans.position);
								this.rightLowerArmJoint.connectedBody = citizenPhysics8.rb;
								this.rightLowerArmJoint.axis = new Vector3(1f, 0f, 0f);
								CharacterJoint characterJoint43 = this.rightLowerArmJoint;
								SoftJointLimit softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = -10f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint43.lowTwistLimit = softJointLimit;
								CharacterJoint characterJoint44 = this.rightLowerArmJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 100f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint44.highTwistLimit = softJointLimit;
								CharacterJoint characterJoint45 = this.rightLowerArmJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 40f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint45.swing1Limit = softJointLimit;
								CharacterJoint characterJoint46 = this.rightLowerArmJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 25f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint46.swing2Limit = softJointLimit;
								CharacterJoint characterJoint47 = this.rightLowerArmJoint;
								SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
								softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
								softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
								characterJoint47.twistLimitSpring = softJointLimitSpring;
								CharacterJoint characterJoint48 = this.rightLowerArmJoint;
								softJointLimitSpring = default(SoftJointLimitSpring);
								softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
								softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
								characterJoint48.swingLimitSpring = softJointLimitSpring;
								this.ApplyRagdollJointSettings(ref this.rightLowerArmJoint);
								this.createdJoints.Add(this.rightLowerArmJoint);
							}
							if (citizenPhysics12 != null && citizenPhysics12.coll != null)
							{
								this.rightHandJoint = citizenPhysics12.coll.transform.gameObject.AddComponent<CharacterJoint>();
								if (this.rightHandJoint != null)
								{
									this.rightHandJoint.anchor = this.rightHandJoint.transform.InverseTransformPoint(citizenPhysics12.anchorConfig.trans.position);
									this.rightHandJoint.connectedBody = citizenPhysics9.rb;
									this.rightHandJoint.axis = new Vector3(0f, 0f, 1f);
									CharacterJoint characterJoint49 = this.rightHandJoint;
									SoftJointLimit softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = -80f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint49.lowTwistLimit = softJointLimit;
									CharacterJoint characterJoint50 = this.rightHandJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 80f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint50.highTwistLimit = softJointLimit;
									CharacterJoint characterJoint51 = this.rightHandJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 20f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint51.swing1Limit = softJointLimit;
									CharacterJoint characterJoint52 = this.rightHandJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 0f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint52.swing2Limit = softJointLimit;
									CharacterJoint characterJoint53 = this.rightHandJoint;
									SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
									softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
									softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
									characterJoint53.twistLimitSpring = softJointLimitSpring;
									CharacterJoint characterJoint54 = this.rightHandJoint;
									softJointLimitSpring = default(SoftJointLimitSpring);
									softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
									softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
									characterJoint54.swingLimitSpring = softJointLimitSpring;
									this.ApplyRagdollJointSettings(ref this.rightHandJoint);
									this.createdJoints.Add(this.rightHandJoint);
								}
							}
						}
					}
				}
				if (citizenPhysics6 != null)
				{
					if (citizenPhysics16 != null && citizenPhysics16.coll != null)
					{
						this.leftUpperLegJoint = citizenPhysics16.coll.transform.gameObject.AddComponent<CharacterJoint>();
						if (this.leftUpperLegJoint != null)
						{
							this.leftUpperLegJoint.anchor = this.leftUpperLegJoint.transform.InverseTransformPoint(citizenPhysics16.anchorConfig.trans.position);
							this.leftUpperLegJoint.connectedBody = citizenPhysics6.rb;
							this.leftUpperLegJoint.axis = new Vector3(1f, 0f, 0f);
							CharacterJoint characterJoint55 = this.leftUpperLegJoint;
							SoftJointLimit softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = -5f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint55.lowTwistLimit = softJointLimit;
							CharacterJoint characterJoint56 = this.leftUpperLegJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 50f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint56.highTwistLimit = softJointLimit;
							CharacterJoint characterJoint57 = this.leftUpperLegJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 15f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint57.swing1Limit = softJointLimit;
							CharacterJoint characterJoint58 = this.leftUpperLegJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 15f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint58.swing2Limit = softJointLimit;
							CharacterJoint characterJoint59 = this.leftUpperLegJoint;
							SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
							softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
							softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
							characterJoint59.twistLimitSpring = softJointLimitSpring;
							CharacterJoint characterJoint60 = this.leftUpperLegJoint;
							softJointLimitSpring = default(SoftJointLimitSpring);
							softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
							softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
							characterJoint60.swingLimitSpring = softJointLimitSpring;
							this.ApplyRagdollJointSettings(ref this.leftUpperLegJoint);
							this.createdJoints.Add(this.leftUpperLegJoint);
						}
						if (citizenPhysics17 != null && citizenPhysics17.coll != null)
						{
							this.leftLowerLegJoint = citizenPhysics17.coll.transform.gameObject.AddComponent<CharacterJoint>();
							if (this.leftLowerLegJoint != null)
							{
								this.leftLowerLegJoint.anchor = this.leftLowerLegJoint.transform.InverseTransformPoint(citizenPhysics17.anchorConfig.trans.position);
								this.leftLowerLegJoint.connectedBody = citizenPhysics16.rb;
								this.leftLowerLegJoint.axis = new Vector3(1f, 0f, 0f);
								CharacterJoint characterJoint61 = this.leftLowerLegJoint;
								SoftJointLimit softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = -90f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint61.lowTwistLimit = softJointLimit;
								CharacterJoint characterJoint62 = this.leftLowerLegJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 10f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint62.highTwistLimit = softJointLimit;
								CharacterJoint characterJoint63 = this.leftLowerLegJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 0f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint63.swing1Limit = softJointLimit;
								CharacterJoint characterJoint64 = this.leftLowerLegJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 0f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint64.swing2Limit = softJointLimit;
								CharacterJoint characterJoint65 = this.leftLowerLegJoint;
								SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
								softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
								softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
								characterJoint65.twistLimitSpring = softJointLimitSpring;
								CharacterJoint characterJoint66 = this.leftLowerLegJoint;
								softJointLimitSpring = default(SoftJointLimitSpring);
								softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
								softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
								characterJoint66.swingLimitSpring = softJointLimitSpring;
								this.ApplyRagdollJointSettings(ref this.leftLowerLegJoint);
								this.createdJoints.Add(this.leftLowerLegJoint);
							}
							if (citizenPhysics19 != null && citizenPhysics19.coll != null)
							{
								this.leftFootJoint = citizenPhysics19.coll.transform.gameObject.AddComponent<CharacterJoint>();
								if (this.leftFootJoint != null)
								{
									this.leftFootJoint.anchor = this.leftFootJoint.transform.InverseTransformPoint(citizenPhysics19.anchorConfig.trans.position);
									this.leftFootJoint.connectedBody = citizenPhysics17.rb;
									this.leftFootJoint.axis = new Vector3(1f, 0f, 0f);
									CharacterJoint characterJoint67 = this.leftFootJoint;
									SoftJointLimit softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = -60f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint67.lowTwistLimit = softJointLimit;
									CharacterJoint characterJoint68 = this.leftFootJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 20f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint68.highTwistLimit = softJointLimit;
									CharacterJoint characterJoint69 = this.leftFootJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 15f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint69.swing1Limit = softJointLimit;
									CharacterJoint characterJoint70 = this.leftFootJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 15f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint70.swing2Limit = softJointLimit;
									CharacterJoint characterJoint71 = this.leftFootJoint;
									SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
									softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
									softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
									characterJoint71.twistLimitSpring = softJointLimitSpring;
									CharacterJoint characterJoint72 = this.leftFootJoint;
									softJointLimitSpring = default(SoftJointLimitSpring);
									softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
									softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
									characterJoint72.swingLimitSpring = softJointLimitSpring;
									this.ApplyRagdollJointSettings(ref this.leftFootJoint);
									this.createdJoints.Add(this.leftFootJoint);
								}
							}
						}
					}
					if (citizenPhysics14 != null && citizenPhysics14.coll != null)
					{
						this.rightUpperLegJoint = citizenPhysics14.coll.transform.gameObject.AddComponent<CharacterJoint>();
						if (this.rightUpperLegJoint != null)
						{
							this.rightUpperLegJoint.anchor = this.rightUpperLegJoint.transform.InverseTransformPoint(citizenPhysics14.anchorConfig.trans.position);
							this.rightUpperLegJoint.connectedBody = citizenPhysics6.rb;
							this.rightUpperLegJoint.axis = new Vector3(1f, 0f, 0f);
							CharacterJoint characterJoint73 = this.rightUpperLegJoint;
							SoftJointLimit softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = -5f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint73.lowTwistLimit = softJointLimit;
							CharacterJoint characterJoint74 = this.rightUpperLegJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 50f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint74.highTwistLimit = softJointLimit;
							CharacterJoint characterJoint75 = this.rightUpperLegJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 15f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint75.swing1Limit = softJointLimit;
							CharacterJoint characterJoint76 = this.rightUpperLegJoint;
							softJointLimit = default(SoftJointLimit);
							softJointLimit.limit = 15f;
							softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
							softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
							characterJoint76.swing2Limit = softJointLimit;
							CharacterJoint characterJoint77 = this.rightUpperLegJoint;
							SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
							softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
							softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
							characterJoint77.twistLimitSpring = softJointLimitSpring;
							CharacterJoint characterJoint78 = this.rightUpperLegJoint;
							softJointLimitSpring = default(SoftJointLimitSpring);
							softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
							softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
							characterJoint78.swingLimitSpring = softJointLimitSpring;
							this.ApplyRagdollJointSettings(ref this.rightUpperLegJoint);
							this.createdJoints.Add(this.rightUpperLegJoint);
						}
						if (citizenPhysics15 != null && citizenPhysics15.coll != null)
						{
							this.rightLowerLegJoint = citizenPhysics15.coll.transform.gameObject.AddComponent<CharacterJoint>();
							if (this.rightLowerLegJoint != null)
							{
								this.rightLowerLegJoint.anchor = this.rightLowerLegJoint.transform.InverseTransformPoint(citizenPhysics15.anchorConfig.trans.position);
								this.rightLowerLegJoint.connectedBody = citizenPhysics14.rb;
								this.rightLowerLegJoint.axis = new Vector3(1f, 0f, 0f);
								CharacterJoint characterJoint79 = this.rightLowerLegJoint;
								SoftJointLimit softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = -90f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint79.lowTwistLimit = softJointLimit;
								CharacterJoint characterJoint80 = this.rightLowerLegJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 10f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint80.highTwistLimit = softJointLimit;
								CharacterJoint characterJoint81 = this.rightLowerLegJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 0f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint81.swing1Limit = softJointLimit;
								CharacterJoint characterJoint82 = this.rightLowerLegJoint;
								softJointLimit = default(SoftJointLimit);
								softJointLimit.limit = 0f;
								softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
								softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
								characterJoint82.swing2Limit = softJointLimit;
								CharacterJoint characterJoint83 = this.rightLowerLegJoint;
								SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
								softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
								softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
								characterJoint83.twistLimitSpring = softJointLimitSpring;
								CharacterJoint characterJoint84 = this.rightLowerLegJoint;
								softJointLimitSpring = default(SoftJointLimitSpring);
								softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
								softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
								characterJoint84.swingLimitSpring = softJointLimitSpring;
								this.ApplyRagdollJointSettings(ref this.rightLowerLegJoint);
								this.createdJoints.Add(this.rightLowerLegJoint);
							}
							if (citizenPhysics18 != null && citizenPhysics18.coll != null)
							{
								this.rightFootJoint = citizenPhysics18.coll.transform.gameObject.AddComponent<CharacterJoint>();
								if (this.rightFootJoint != null)
								{
									this.rightFootJoint.anchor = this.rightFootJoint.transform.InverseTransformPoint(citizenPhysics18.anchorConfig.trans.position);
									this.rightFootJoint.connectedBody = citizenPhysics15.rb;
									this.rightFootJoint.axis = new Vector3(1f, 0f, 0f);
									CharacterJoint characterJoint85 = this.rightFootJoint;
									SoftJointLimit softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = -60f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint85.lowTwistLimit = softJointLimit;
									CharacterJoint characterJoint86 = this.rightFootJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 20f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint86.highTwistLimit = softJointLimit;
									CharacterJoint characterJoint87 = this.rightFootJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 15f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint87.swing1Limit = softJointLimit;
									CharacterJoint characterJoint88 = this.rightFootJoint;
									softJointLimit = default(SoftJointLimit);
									softJointLimit.limit = 15f;
									softJointLimit.contactDistance = GameplayControls.Instance.ragdollJointContactDistance;
									softJointLimit.bounciness = GameplayControls.Instance.ragdollJointBounce;
									characterJoint88.swing2Limit = softJointLimit;
									CharacterJoint characterJoint89 = this.rightFootJoint;
									SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
									softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
									softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
									characterJoint89.twistLimitSpring = softJointLimitSpring;
									CharacterJoint characterJoint90 = this.rightFootJoint;
									softJointLimitSpring = default(SoftJointLimitSpring);
									softJointLimitSpring.damper = GameplayControls.Instance.ragdollJointDampen;
									softJointLimitSpring.spring = GameplayControls.Instance.ragdollJointSpring;
									characterJoint90.swingLimitSpring = softJointLimitSpring;
									this.ApplyRagdollJointSettings(ref this.rightFootJoint);
									this.createdJoints.Add(this.rightFootJoint);
								}
							}
						}
					}
				}
				if (this.cit.ai != null)
				{
					if (this.cit.ai.dragController == null)
					{
						this.cit.ai.dragController = this.cit.gameObject.AddComponent<RigidbodyDragObject>();
						if (this.cit.ai.dragController != null)
						{
							this.cit.ai.dragController.OnEnterRagdollState(this.cit.ai);
						}
					}
					if (this.cit.ai.ragdollPositionUpdate == null)
					{
						this.cit.ai.ragdollPositionUpdate = this.cit.gameObject.AddComponent<RagdollPositionUpdater>();
						if (this.cit.ai.ragdollPositionUpdate != null)
						{
							this.cit.ai.ragdollPositionUpdate.Setup(this.cit.ai);
						}
					}
				}
				using (List<Citizen>.Enumerator enumerator4 = CityData.Instance.citizenDirectory.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						Citizen citizen = enumerator4.Current;
						if (!(citizen == null) && !(citizen == this.cit) && citizen.ai != null && citizen.ai.capCollider != null)
						{
							foreach (Collider collider in this.createdColliders)
							{
								if (!(collider == null))
								{
									Physics.IgnoreCollision(citizen.ai.capCollider, collider, true);
								}
							}
						}
					}
					goto IL_3132;
				}
			}
			if (this.sfx != null)
			{
				Object.Destroy(this.sfx);
			}
			GameplayController.Instance.activeRagdolls.Remove(this.cit);
			this.cit.ai.SetExpression(CitizenOutfitController.Expression.surprised);
			this.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
			if (this.rightFootJoint != null)
			{
				Object.Destroy(this.rightFootJoint);
			}
			if (this.leftFootJoint != null)
			{
				Object.Destroy(this.leftFootJoint);
			}
			if (this.rightLowerLegJoint != null)
			{
				Object.Destroy(this.rightLowerLegJoint);
			}
			if (this.leftLowerLegJoint != null)
			{
				Object.Destroy(this.leftLowerLegJoint);
			}
			if (this.rightUpperLegJoint != null)
			{
				Object.Destroy(this.rightUpperLegJoint);
			}
			if (this.leftUpperLegJoint != null)
			{
				Object.Destroy(this.leftUpperLegJoint);
			}
			if (this.rightHandJoint != null)
			{
				Object.Destroy(this.rightHandJoint);
			}
			if (this.leftHandJoint != null)
			{
				Object.Destroy(this.leftHandJoint);
			}
			if (this.rightLowerArmJoint != null)
			{
				Object.Destroy(this.rightLowerArmJoint);
			}
			if (this.leftLowerArmJoint != null)
			{
				Object.Destroy(this.leftLowerArmJoint);
			}
			if (this.rightUpperArmJoint != null)
			{
				Object.Destroy(this.rightUpperArmJoint);
			}
			if (this.leftUpperArmJoint != null)
			{
				Object.Destroy(this.leftUpperArmJoint);
			}
			if (this.headJoint != null)
			{
				Object.Destroy(this.headJoint);
			}
			if (this.upperTorsoJoint != null)
			{
				Object.Destroy(this.upperTorsoJoint);
			}
			if (this.midriffJoint != null)
			{
				Object.Destroy(this.midriffJoint);
			}
			int num = 200;
			while ((this.createdJoints.Count > 0 || this.createdRBs.Count > 0 || this.createdColliders.Count > 0) && num > 0)
			{
				if (this.createdJoints.Count > 0)
				{
					if (this.createdJoints[0] != null)
					{
						this.createdJoints[0].connectedBody = null;
						Object.Destroy(this.createdJoints[0]);
					}
					this.createdJoints.RemoveAt(0);
				}
				if (this.createdRBs.Count > 0)
				{
					Object.Destroy(this.createdRBs[0]);
					this.createdRBs.RemoveAt(0);
				}
				if (this.createdColliders.Count > 0)
				{
					foreach (Citizen citizen2 in CityData.Instance.citizenDirectory)
					{
						if (!(citizen2 == null) && !(citizen2 == this.cit) && citizen2.ai != null && citizen2.ai.capCollider != null)
						{
							Physics.IgnoreCollision(citizen2.ai.capCollider, this.createdColliders[0], false);
						}
					}
					Object.Destroy(this.createdColliders[0]);
					this.createdColliders.RemoveAt(0);
				}
				num--;
				if (num <= 0)
				{
					string[] array = new string[7];
					array[0] = "Reached safety event while trying to destroy ragdoll physics! Remaining: ";
					array[1] = this.createdJoints.Count.ToString();
					array[2] = " joints, ";
					int num2 = 3;
					List<Rigidbody> list = this.createdRBs;
					array[num2] = ((list != null) ? list.ToString() : null);
					array[4] = "RBs, and ";
					array[5] = this.createdColliders.Count.ToString();
					array[6] = " colliders.";
					Game.LogError(string.Concat(array), 2);
					break;
				}
				if (this.createdJoints.Count <= 0 && this.createdRBs.Count <= 0 && this.createdColliders.Count <= 0)
				{
					Game.Log("Removed all ragdoll physics elements successfully", 2);
				}
			}
			if (this.cit.interactableController != null && this.cit.interactableController.coll != null)
			{
				if (this.newBoxCollider != null)
				{
					Object.Destroy(this.newBoxCollider);
				}
				this.cit.interactableController.coll = this.cit.interactableController.gameObject.GetComponent<Collider>();
				this.cit.interactableController.coll.enabled = true;
			}
			this.ragdollSnapshot = this.GetLimbSnapshot();
			this.createdRBs.Clear();
			this.createdJoints.Clear();
			this.createdColliders.Clear();
			this.physicsComponents.Clear();
			if (dead && this.mainAnimator != null)
			{
				if (this.newBoxCollider != null)
				{
					Object.Destroy(this.newBoxCollider);
				}
				Object.Destroy(this.mainAnimator);
			}
			if (dead)
			{
				this.cit.ai.SetUpdateEnabled(false);
			}
			if (this.cit.ai != null)
			{
				if (this.cit.ai.dragController != null)
				{
					this.cit.ai.dragController.OnExitRagdollState();
					Object.Destroy(this.cit.ai.dragController);
				}
				if (this.cit.ai.ragdollPositionUpdate != null)
				{
					Object.Destroy(this.cit.ai.ragdollPositionUpdate);
				}
			}
			IL_3132:
			this.cit.outfitController.HairHatCompatibilityCheck();
			this.cit.UpdateLODs();
		}
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x0006DC18 File Offset: 0x0006BE18
	private void ApplyRagdollJointSettings(ref CharacterJoint joint)
	{
		joint.enablePreprocessing = GameplayControls.Instance.ragdollJointPreprocessing;
		joint.enableCollision = GameplayControls.Instance.ragdollJointCollision;
		joint.enableProjection = GameplayControls.Instance.ragdollJointProjection;
		joint.massScale = 1f;
		joint.connectedMassScale = 1f;
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x0006DC70 File Offset: 0x0006BE70
	public List<CitizenAnimationController.RagdollSnapshot> GetLimbSnapshot()
	{
		List<CitizenAnimationController.RagdollSnapshot> list = new List<CitizenAnimationController.RagdollSnapshot>();
		foreach (CitizenOutfitController.AnchorConfig anchorConfig in this.cit.outfitController.anchorConfig)
		{
			list.Add(new CitizenAnimationController.RagdollSnapshot
			{
				anchorConfig = anchorConfig,
				localPos = anchorConfig.trans.localPosition,
				localRot = anchorConfig.trans.localRotation
			});
		}
		return list;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0006DD04 File Offset: 0x0006BF04
	public List<CitizenAnimationController.RagdollSnapshotWorld> GetLimbSnapshotWorld()
	{
		List<CitizenAnimationController.RagdollSnapshotWorld> list = new List<CitizenAnimationController.RagdollSnapshotWorld>();
		foreach (CitizenOutfitController.AnchorConfig anchorConfig in this.cit.outfitController.anchorConfig)
		{
			list.Add(new CitizenAnimationController.RagdollSnapshotWorld
			{
				anchorConfig = anchorConfig,
				worldPos = anchorConfig.trans.position,
				worldRot = anchorConfig.trans.rotation
			});
		}
		return list;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x0006DD98 File Offset: 0x0006BF98
	public void LoadLimbSnapshot(List<CitizenAnimationController.RagdollSnapshot> snapshot)
	{
		Game.Log("Loading a limb snapshot for " + this.cit.name, 2);
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.lowerTorso);
		if (ragdollSnapshot != null)
		{
			Transform transform = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot.anchorConfig.anchor, ref transform))
			{
				transform.SetLocalPositionAndRotation(ragdollSnapshot.localPos, ragdollSnapshot.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot2 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.upperTorso);
		if (ragdollSnapshot2 != null)
		{
			Transform transform2 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot2.anchorConfig.anchor, ref transform2))
			{
				transform2.SetLocalPositionAndRotation(ragdollSnapshot2.localPos, ragdollSnapshot2.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot3 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Midriff);
		if (ragdollSnapshot3 != null)
		{
			Transform transform3 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot3.anchorConfig.anchor, ref transform3))
			{
				transform3.SetLocalPositionAndRotation(ragdollSnapshot3.localPos, ragdollSnapshot3.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot4 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperLegRight);
		if (ragdollSnapshot4 != null)
		{
			Transform transform4 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot4.anchorConfig.anchor, ref transform4))
			{
				transform4.SetLocalPositionAndRotation(ragdollSnapshot4.localPos, ragdollSnapshot4.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot5 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperLegLeft);
		if (ragdollSnapshot5 != null)
		{
			Transform transform5 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot5.anchorConfig.anchor, ref transform5))
			{
				transform5.SetLocalPositionAndRotation(ragdollSnapshot5.localPos, ragdollSnapshot5.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot6 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Head);
		if (ragdollSnapshot6 != null)
		{
			Transform transform6 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot6.anchorConfig.anchor, ref transform6))
			{
				transform6.SetLocalPositionAndRotation(ragdollSnapshot6.localPos, ragdollSnapshot6.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot7 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.ArmsParent);
		if (ragdollSnapshot7 != null)
		{
			Transform transform7 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot7.anchorConfig.anchor, ref transform7))
			{
				transform7.SetLocalPositionAndRotation(ragdollSnapshot7.localPos, ragdollSnapshot7.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot8 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperArmRight);
		if (ragdollSnapshot7 != null)
		{
			Transform transform8 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot8.anchorConfig.anchor, ref transform8))
			{
				transform8.SetLocalPositionAndRotation(ragdollSnapshot8.localPos, ragdollSnapshot8.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot9 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperArmLeft);
		if (ragdollSnapshot7 != null)
		{
			Transform transform9 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot9.anchorConfig.anchor, ref transform9))
			{
				transform9.SetLocalPositionAndRotation(ragdollSnapshot9.localPos, ragdollSnapshot9.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot10 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerArmRight);
		if (ragdollSnapshot10 != null)
		{
			Transform transform10 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot10.anchorConfig.anchor, ref transform10))
			{
				transform10.SetLocalPositionAndRotation(ragdollSnapshot10.localPos, ragdollSnapshot10.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot11 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerArmLeft);
		if (ragdollSnapshot10 != null)
		{
			Transform transform11 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot11.anchorConfig.anchor, ref transform11))
			{
				transform11.SetLocalPositionAndRotation(ragdollSnapshot11.localPos, ragdollSnapshot11.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot12 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.HandLeft);
		if (ragdollSnapshot12 != null)
		{
			Transform transform12 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot12.anchorConfig.anchor, ref transform12))
			{
				transform12.SetLocalPositionAndRotation(ragdollSnapshot12.localPos, ragdollSnapshot12.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot13 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.HandRight);
		if (ragdollSnapshot13 != null)
		{
			Transform transform13 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot13.anchorConfig.anchor, ref transform13))
			{
				transform13.SetLocalPositionAndRotation(ragdollSnapshot13.localPos, ragdollSnapshot13.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot14 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerLegRight);
		if (ragdollSnapshot14 != null)
		{
			Transform transform14 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot14.anchorConfig.anchor, ref transform14))
			{
				transform14.SetLocalPositionAndRotation(ragdollSnapshot14.localPos, ragdollSnapshot14.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot15 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerLegLeft);
		if (ragdollSnapshot15 != null)
		{
			Transform transform15 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot15.anchorConfig.anchor, ref transform15))
			{
				transform15.SetLocalPositionAndRotation(ragdollSnapshot15.localPos, ragdollSnapshot15.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot16 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LeftFoot);
		if (ragdollSnapshot16 != null)
		{
			Transform transform16 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot16.anchorConfig.anchor, ref transform16))
			{
				transform16.SetLocalPositionAndRotation(ragdollSnapshot16.localPos, ragdollSnapshot16.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot17 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.RightFoot);
		if (ragdollSnapshot16 != null)
		{
			Transform transform17 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot17.anchorConfig.anchor, ref transform17))
			{
				transform17.SetLocalPositionAndRotation(ragdollSnapshot17.localPos, ragdollSnapshot17.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot18 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Hat);
		if (ragdollSnapshot18 != null)
		{
			Transform transform18 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot18.anchorConfig.anchor, ref transform18))
			{
				transform18.SetLocalPositionAndRotation(ragdollSnapshot18.localPos, ragdollSnapshot18.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot19 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Hair);
		if (ragdollSnapshot19 != null)
		{
			Transform transform19 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot19.anchorConfig.anchor, ref transform19))
			{
				transform19.SetLocalPositionAndRotation(ragdollSnapshot19.localPos, ragdollSnapshot19.localRot);
			}
		}
		CitizenAnimationController.RagdollSnapshot ragdollSnapshot20 = snapshot.Find((CitizenAnimationController.RagdollSnapshot item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Glasses);
		if (ragdollSnapshot19 != null)
		{
			Transform transform20 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshot20.anchorConfig.anchor, ref transform20))
			{
				transform20.SetLocalPositionAndRotation(ragdollSnapshot20.localPos, ragdollSnapshot20.localRot);
			}
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x0006E5CC File Offset: 0x0006C7CC
	public void LoadLimbSnapshot(List<CitizenAnimationController.RagdollSnapshotWorld> snapshot)
	{
		Game.Log("Loading a limb snapshot for " + this.cit.name, 2);
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.lowerTorso);
		if (ragdollSnapshotWorld != null)
		{
			Transform transform = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld.anchorConfig.anchor, ref transform))
			{
				transform.SetPositionAndRotation(ragdollSnapshotWorld.worldPos, ragdollSnapshotWorld.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld2 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.upperTorso);
		if (ragdollSnapshotWorld2 != null)
		{
			Transform transform2 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld2.anchorConfig.anchor, ref transform2))
			{
				transform2.SetPositionAndRotation(ragdollSnapshotWorld2.worldPos, ragdollSnapshotWorld2.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld3 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Midriff);
		if (ragdollSnapshotWorld3 != null)
		{
			Transform transform3 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld3.anchorConfig.anchor, ref transform3))
			{
				transform3.SetPositionAndRotation(ragdollSnapshotWorld3.worldPos, ragdollSnapshotWorld3.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld4 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperLegRight);
		if (ragdollSnapshotWorld4 != null)
		{
			Transform transform4 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld4.anchorConfig.anchor, ref transform4))
			{
				transform4.SetPositionAndRotation(ragdollSnapshotWorld4.worldPos, ragdollSnapshotWorld4.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld5 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperLegLeft);
		if (ragdollSnapshotWorld5 != null)
		{
			Transform transform5 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld5.anchorConfig.anchor, ref transform5))
			{
				transform5.SetPositionAndRotation(ragdollSnapshotWorld5.worldPos, ragdollSnapshotWorld5.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld6 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Head);
		if (ragdollSnapshotWorld6 != null)
		{
			Transform transform6 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld6.anchorConfig.anchor, ref transform6))
			{
				transform6.SetPositionAndRotation(ragdollSnapshotWorld6.worldPos, ragdollSnapshotWorld6.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld7 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.ArmsParent);
		if (ragdollSnapshotWorld7 != null)
		{
			Transform transform7 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld7.anchorConfig.anchor, ref transform7))
			{
				transform7.SetPositionAndRotation(ragdollSnapshotWorld7.worldPos, ragdollSnapshotWorld7.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld8 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperArmRight);
		if (ragdollSnapshotWorld7 != null)
		{
			Transform transform8 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld8.anchorConfig.anchor, ref transform8))
			{
				transform8.SetPositionAndRotation(ragdollSnapshotWorld8.worldPos, ragdollSnapshotWorld8.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld9 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.UpperArmLeft);
		if (ragdollSnapshotWorld7 != null)
		{
			Transform transform9 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld9.anchorConfig.anchor, ref transform9))
			{
				transform9.SetPositionAndRotation(ragdollSnapshotWorld9.worldPos, ragdollSnapshotWorld9.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld10 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerArmRight);
		if (ragdollSnapshotWorld10 != null)
		{
			Transform transform10 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld10.anchorConfig.anchor, ref transform10))
			{
				transform10.SetPositionAndRotation(ragdollSnapshotWorld10.worldPos, ragdollSnapshotWorld10.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld11 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerArmLeft);
		if (ragdollSnapshotWorld10 != null)
		{
			Transform transform11 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld11.anchorConfig.anchor, ref transform11))
			{
				transform11.SetPositionAndRotation(ragdollSnapshotWorld11.worldPos, ragdollSnapshotWorld11.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld12 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.HandLeft);
		if (ragdollSnapshotWorld12 != null)
		{
			Transform transform12 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld12.anchorConfig.anchor, ref transform12))
			{
				transform12.SetPositionAndRotation(ragdollSnapshotWorld12.worldPos, ragdollSnapshotWorld12.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld13 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.HandRight);
		if (ragdollSnapshotWorld13 != null)
		{
			Transform transform13 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld13.anchorConfig.anchor, ref transform13))
			{
				transform13.SetPositionAndRotation(ragdollSnapshotWorld13.worldPos, ragdollSnapshotWorld13.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld14 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerLegRight);
		if (ragdollSnapshotWorld14 != null)
		{
			Transform transform14 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld14.anchorConfig.anchor, ref transform14))
			{
				transform14.SetPositionAndRotation(ragdollSnapshotWorld14.worldPos, ragdollSnapshotWorld14.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld15 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LowerLegLeft);
		if (ragdollSnapshotWorld15 != null)
		{
			Transform transform15 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld15.anchorConfig.anchor, ref transform15))
			{
				transform15.SetPositionAndRotation(ragdollSnapshotWorld15.worldPos, ragdollSnapshotWorld15.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld16 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.LeftFoot);
		if (ragdollSnapshotWorld16 != null)
		{
			Transform transform16 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld16.anchorConfig.anchor, ref transform16))
			{
				transform16.SetPositionAndRotation(ragdollSnapshotWorld16.worldPos, ragdollSnapshotWorld16.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld17 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.RightFoot);
		if (ragdollSnapshotWorld16 != null)
		{
			Transform transform17 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld17.anchorConfig.anchor, ref transform17))
			{
				transform17.SetPositionAndRotation(ragdollSnapshotWorld17.worldPos, ragdollSnapshotWorld17.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld18 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Hat);
		if (ragdollSnapshotWorld18 != null)
		{
			Transform transform18 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld18.anchorConfig.anchor, ref transform18))
			{
				transform18.SetPositionAndRotation(ragdollSnapshotWorld18.worldPos, ragdollSnapshotWorld18.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld19 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Hair);
		if (ragdollSnapshotWorld19 != null)
		{
			Transform transform19 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld19.anchorConfig.anchor, ref transform19))
			{
				transform19.SetPositionAndRotation(ragdollSnapshotWorld19.worldPos, ragdollSnapshotWorld19.worldRot);
			}
		}
		CitizenAnimationController.RagdollSnapshotWorld ragdollSnapshotWorld20 = snapshot.Find((CitizenAnimationController.RagdollSnapshotWorld item) => item.anchorConfig.anchor == CitizenOutfitController.CharacterAnchor.Glasses);
		if (ragdollSnapshotWorld19 != null)
		{
			Transform transform20 = null;
			if (this.cit.outfitController.anchorReference.TryGetValue(ragdollSnapshotWorld20.anchorConfig.anchor, ref transform20))
			{
				transform20.SetPositionAndRotation(ragdollSnapshotWorld20.worldPos, ragdollSnapshotWorld20.worldRot);
			}
		}
	}

	// Token: 0x04000717 RID: 1815
	public Human cit;

	// Token: 0x04000718 RID: 1816
	public Animator mainAnimator;

	// Token: 0x04000719 RID: 1817
	public GameObject spawnedUmbrella;

	// Token: 0x0400071A RID: 1818
	public Transform umbrellaCanopy;

	// Token: 0x0400071B RID: 1819
	public float armsLayerDesiredWeight;

	// Token: 0x0400071C RID: 1820
	public float umbreallLayerDesiredWeight;

	// Token: 0x0400071D RID: 1821
	public float oneShotUseReset;

	// Token: 0x0400071E RID: 1822
	public CitizenAnimationController.ArmsBoolSate armsBoolAnimationState;

	// Token: 0x0400071F RID: 1823
	public CitizenAnimationController.IdleAnimationState idleAnimationState;

	// Token: 0x04000720 RID: 1824
	public bool flipToRightAnimation;

	// Token: 0x04000721 RID: 1825
	public bool paused;

	// Token: 0x04000722 RID: 1826
	public bool umbrella;

	// Token: 0x04000723 RID: 1827
	[Header("Human Components")]
	public Transform armsParent;

	// Token: 0x04000724 RID: 1828
	public BoxCollider newBoxCollider;

	// Token: 0x04000725 RID: 1829
	[NonSerialized]
	public Rigidbody upperTorsoRB;

	// Token: 0x04000726 RID: 1830
	[Header("Ragdoll")]
	public Dictionary<CitizenOutfitController.CharacterAnchor, CitizenAnimationController.CitizenPhysics> physicsComponents = new Dictionary<CitizenOutfitController.CharacterAnchor, CitizenAnimationController.CitizenPhysics>();

	// Token: 0x04000727 RID: 1831
	[NonSerialized]
	public List<Rigidbody> createdRBs = new List<Rigidbody>();

	// Token: 0x04000728 RID: 1832
	[NonSerialized]
	public List<CharacterJoint> createdJoints = new List<CharacterJoint>();

	// Token: 0x04000729 RID: 1833
	[NonSerialized]
	public List<Collider> createdColliders = new List<Collider>();

	// Token: 0x0400072A RID: 1834
	[NonSerialized]
	public RagdollSFXController sfx;

	// Token: 0x0400072B RID: 1835
	private CharacterJoint headJoint;

	// Token: 0x0400072C RID: 1836
	private CharacterJoint upperTorsoJoint;

	// Token: 0x0400072D RID: 1837
	private CharacterJoint midriffJoint;

	// Token: 0x0400072E RID: 1838
	private CharacterJoint leftUpperArmJoint;

	// Token: 0x0400072F RID: 1839
	private CharacterJoint leftLowerArmJoint;

	// Token: 0x04000730 RID: 1840
	private CharacterJoint leftHandJoint;

	// Token: 0x04000731 RID: 1841
	private CharacterJoint rightUpperArmJoint;

	// Token: 0x04000732 RID: 1842
	private CharacterJoint rightLowerArmJoint;

	// Token: 0x04000733 RID: 1843
	private CharacterJoint rightHandJoint;

	// Token: 0x04000734 RID: 1844
	private CharacterJoint leftUpperLegJoint;

	// Token: 0x04000735 RID: 1845
	private CharacterJoint leftLowerLegJoint;

	// Token: 0x04000736 RID: 1846
	private CharacterJoint rightUpperLegJoint;

	// Token: 0x04000737 RID: 1847
	private CharacterJoint rightLowerLegJoint;

	// Token: 0x04000738 RID: 1848
	private CharacterJoint rightFootJoint;

	// Token: 0x04000739 RID: 1849
	private CharacterJoint leftFootJoint;

	// Token: 0x0400073A RID: 1850
	[NonSerialized]
	public List<CitizenAnimationController.RagdollSnapshot> ragdollSnapshot = new List<CitizenAnimationController.RagdollSnapshot>();

	// Token: 0x0400073B RID: 1851
	[Header("Debug")]
	public float debugMainAnimatorSpeed;

	// Token: 0x020000FA RID: 250
	public enum ArmsBoolSate
	{
		// Token: 0x0400073D RID: 1853
		none,
		// Token: 0x0400073E RID: 1854
		armsResting,
		// Token: 0x0400073F RID: 1855
		armsTyping,
		// Token: 0x04000740 RID: 1856
		armsUse,
		// Token: 0x04000741 RID: 1857
		armsLocking,
		// Token: 0x04000742 RID: 1858
		armsCuffed,
		// Token: 0x04000743 RID: 1859
		armsConsuming,
		// Token: 0x04000744 RID: 1860
		armsOneShotUse,
		// Token: 0x04000745 RID: 1861
		armsSmoking,
		// Token: 0x04000746 RID: 1862
		armsSmokingPipe,
		// Token: 0x04000747 RID: 1863
		armsReading,
		// Token: 0x04000748 RID: 1864
		armsFleeing
	}

	// Token: 0x020000FB RID: 251
	public enum IdleAnimationState
	{
		// Token: 0x0400074A RID: 1866
		none,
		// Token: 0x0400074B RID: 1867
		sitting,
		// Token: 0x0400074C RID: 1868
		sweeping,
		// Token: 0x0400074D RID: 1869
		warmingHands,
		// Token: 0x0400074E RID: 1870
		telephone,
		// Token: 0x0400074F RID: 1871
		washingHands,
		// Token: 0x04000750 RID: 1872
		cleaningBar,
		// Token: 0x04000751 RID: 1873
		bargingDoor,
		// Token: 0x04000752 RID: 1874
		cookingChopping,
		// Token: 0x04000753 RID: 1875
		cookingFrying,
		// Token: 0x04000754 RID: 1876
		sitAgainstWall,
		// Token: 0x04000755 RID: 1877
		leanAgainstWall,
		// Token: 0x04000756 RID: 1878
		showering,
		// Token: 0x04000757 RID: 1879
		rubbingEyes,
		// Token: 0x04000758 RID: 1880
		cowering,
		// Token: 0x04000759 RID: 1881
		checkPulse,
		// Token: 0x0400075A RID: 1882
		brushingTeeth,
		// Token: 0x0400075B RID: 1883
		pickUpFromFloor,
		// Token: 0x0400075C RID: 1884
		danceTwist,
		// Token: 0x0400075D RID: 1885
		danceWatusi,
		// Token: 0x0400075E RID: 1886
		stackingObjects,
		// Token: 0x0400075F RID: 1887
		stackingObjectsCrouching
	}

	// Token: 0x020000FC RID: 252
	[Serializable]
	public class CitizenPhysics
	{
		// Token: 0x04000760 RID: 1888
		public CitizenOutfitController.AnchorConfig anchorConfig;

		// Token: 0x04000761 RID: 1889
		public Collider coll;

		// Token: 0x04000762 RID: 1890
		public Rigidbody rb;
	}

	// Token: 0x020000FD RID: 253
	[Serializable]
	public class RagdollSnapshot
	{
		// Token: 0x04000763 RID: 1891
		public CitizenOutfitController.AnchorConfig anchorConfig;

		// Token: 0x04000764 RID: 1892
		public Vector3 localPos;

		// Token: 0x04000765 RID: 1893
		public Quaternion localRot;
	}

	// Token: 0x020000FE RID: 254
	[Serializable]
	public class RagdollSnapshotWorld
	{
		// Token: 0x04000766 RID: 1894
		public CitizenOutfitController.AnchorConfig anchorConfig;

		// Token: 0x04000767 RID: 1895
		public Vector3 worldPos;

		// Token: 0x04000768 RID: 1896
		public Quaternion worldRot;
	}
}
