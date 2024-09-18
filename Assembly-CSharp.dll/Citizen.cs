using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class Citizen : Human
{
	// Token: 0x060002F0 RID: 752 RVA: 0x0001DC0C File Offset: 0x0001BE0C
	public override void SetupEvidence()
	{
		base.SetupEvidence();
		base.CreateDetails();
		this.evidenceEntry.SetNote(Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[1]), Strings.GetTextForComponent("e6f7dae9-af6a-4d58-b7eb-44b6bba58dcb", this, null, null, "\n", false, null, Strings.LinkSetting.automatic, null));
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0001DC54 File Offset: 0x0001BE54
	public void BirthdayCheck()
	{
		bool flag = false;
		string[] array = this.birthday.Split('/', 0);
		int num = 0;
		int num2 = 0;
		int.TryParse(array[0], ref num);
		int.TryParse(array[1], ref num2);
		if (SessionData.Instance.monthInt + 1 == num)
		{
			if (SessionData.Instance.dateInt + 1 >= num2 && SessionData.Instance.dateInt + 1 <= num2 + 7)
			{
				flag = true;
			}
		}
		else if (SessionData.Instance.monthInt + 1 == num + 1 || (SessionData.Instance.monthInt == 0 && num == 12))
		{
			int num3 = 7 - (SessionData.Instance.dateInt + 1);
			int num4 = SessionData.Instance.daysInMonths[num - 1] - num3;
			if (SessionData.Instance.dateInt >= num4)
			{
				flag = true;
			}
		}
		if (flag && this.birthdayCards.Count <= 0 && this.home != null)
		{
			if (Player.Instance.currentGameLocation != this.home)
			{
				using (List<Acquaintance>.Enumerator enumerator = this.acquaintances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Acquaintance acquaintance = enumerator.Current;
						if (acquaintance.known > SocialControls.Instance.knowBirthdayThreshold && acquaintance.like >= 0.25f)
						{
							if (acquaintance.connections.Contains(Acquaintance.ConnectionType.boss) || acquaintance.connections.Contains(Acquaintance.ConnectionType.familiarWork) || acquaintance.connections.Contains(Acquaintance.ConnectionType.workOther) || acquaintance.connections.Contains(Acquaintance.ConnectionType.workTeam))
							{
								if (this.job.employer != null && Player.Instance.currentGameLocation != this.job.employer.address)
								{
									FurnitureLocation furnitureLocation = null;
									Interactable interactable = this.job.employer.address.PlaceObject(InteriorControls.Instance.birthdayCard, this, acquaintance.with, this, out furnitureLocation, true, Interactable.PassedVarType.humanID, acquaintance.with.humanID, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
									this.birthdayCards.Add(interactable);
								}
							}
							else
							{
								FurnitureLocation furnitureLocation2 = null;
								Interactable interactable2 = this.home.PlaceObject(InteriorControls.Instance.birthdayCard, this, acquaintance.with, this, out furnitureLocation2, true, Interactable.PassedVarType.humanID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
								this.birthdayCards.Add(interactable2);
							}
						}
					}
					return;
				}
			}
			if (!flag && this.birthdayCards.Count > 0)
			{
				for (int i = 0; i < this.birthdayCards.Count; i++)
				{
					if (Player.Instance.currentGameLocation != this.birthdayCards[i].node.gameLocation)
					{
						this.birthdayCards[i].SafeDelete(false);
						this.birthdayCards.RemoveAt(i);
						i--;
					}
				}
			}
		}
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0001DF60 File Offset: 0x0001C160
	public override void RecieveDamage(float amount, Actor fromWho, Vector3 damagePosition, Vector3 damageDirection, SpatterPatternPreset forwardSpatter, SpatterPatternPreset backSpatter, SpatterSimulation.EraseMode spatterErase = SpatterSimulation.EraseMode.onceExecutedAndOutOfAddressPlusDespawnTime, bool alertSurrounding = true, bool forceRagdoll = false, float forcedRagdollDuration = 0f, float shockMP = 1f, bool enableKill = false, bool allowRecoil = true, float ragdollForceMP = 1f)
	{
		float num = Mathf.Min(this.currentHealth - amount, 0f) * -1f;
		base.RecieveDamage(amount, fromWho, damagePosition, damageDirection, forwardSpatter, backSpatter, spatterErase, true, false, 0f, shockMP, enableKill, allowRecoil, ragdollForceMP);
		if (amount >= 0.01f)
		{
			float newSpatterCountMultiplier = Mathf.Clamp01(amount / this.maximumHealth * 2f);
			Vector3 vector = base.transform.InverseTransformDirection(damageDirection);
			Vector3 newLocalPosition = base.transform.InverseTransformPoint(damagePosition);
			if (forwardSpatter != null)
			{
				new SpatterSimulation(this, newLocalPosition, -vector, forwardSpatter, spatterErase, newSpatterCountMultiplier, false);
			}
			if (backSpatter != null)
			{
				new SpatterSimulation(this, newLocalPosition, vector, backSpatter, spatterErase, newSpatterCountMultiplier, false);
			}
			if (this.ai != null)
			{
				if (this.ai.human.inConversation)
				{
					this.ai.human.currentConversation.EndConversation();
				}
				if (amount / this.maximumHealth > 0.05f)
				{
					this.speechController.TriggerBark(SpeechController.Bark.takeDamage);
				}
			}
			if (!this.isPlayer && fromWho != null && fromWho.isPlayer)
			{
				AchievementsController.Instance.notTheAnswerFlag = -1;
			}
		}
		if (Game.Instance.collectDebugData)
		{
			string[] array = new string[6];
			array[0] = "Impact from ";
			int num2 = 1;
			Actor fromWho2 = fromWho;
			array[num2] = ((fromWho2 != null) ? fromWho2.ToString() : null);
			array[2] = " to ";
			array[3] = base.GetCitizenName();
			array[4] = ". Health left: ";
			array[5] = this.currentHealth.ToString();
			Game.Log(string.Concat(array), 2);
		}
		if (!Game.Instance.noReactOnAttack && (this.ai == null || this.ai.currentGoal == null || this.ai.currentGoal.preset != RoutineControls.Instance.fleeGoal) && this.ai != null && fromWho != null && fromWho != this)
		{
			if (Game.Instance.collectDebugData)
			{
				base.SelectedDebug("Recieve damage reaction...", Actor.HumanDebug.misc);
			}
			this.ai.SetPersue(fromWho, alertSurrounding, 1, true, CitizenControls.Instance.punchedResponseRange);
		}
		if (this.currentHealth <= 0f || forceRagdoll)
		{
			if (this.ai != null)
			{
				float forceMultiplier = ragdollForceMP + num * CitizenControls.Instance.damageRecieveForceMultiplier;
				if (Game.Instance.collectDebugData)
				{
					Game.Log("KO with force mp of " + forceMultiplier.ToString(), 2);
				}
				this.ai.SetKO(true, damagePosition, damageDirection, forceRagdoll, forcedRagdollDuration, true, forceMultiplier);
				if (fromWho != null && fromWho.isPlayer && AchievementsController.Instance != null && !this.isDead)
				{
					AchievementsController.Instance.pacifistFlag = true;
					if (!AchievementsController.Instance.spareNoOneReference.Contains(this.humanID))
					{
						AchievementsController.Instance.spareNoOneReference.Add(this.humanID);
						if (AchievementsController.Instance.spareNoOneReference.Count >= CityData.Instance.citizenDirectory.Count)
						{
							AchievementsController.Instance.UnlockAchievement("Spare No One", "ko_everybody");
						}
					}
					bool flag = this.job.preset.presetName == "CriminalAccountant" || this.job.preset.presetName == "DebtCollector";
					if (fromWho.isPlayer && flag)
					{
						AchievementsController.Instance.UnlockAchievement("Jump the shark", "ko_debt_collector");
					}
				}
				if (enableKill)
				{
					Interactable weapon = null;
					if (fromWho != null && fromWho.ai != null)
					{
						weapon = fromWho.ai.currentWeapon;
					}
					MurderController.Murder murder = MurderController.Instance.activeMurders.Find((MurderController.Murder item) => item.victim == this && item.murderer == fromWho);
					base.Murder(fromWho as Human, true, murder, weapon, 1f);
					return;
				}
			}
		}
		else if (this.animationController != null && allowRecoil)
		{
			this.animationController.TakeDamageRecoil(damagePosition);
		}
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x0001E3CB File Offset: 0x0001C5CB
	public override void SetCombatSkill(float newSkill)
	{
		base.SetCombatSkill(newSkill);
		this.animationController.mainAnimator.SetFloat("combatSkill", this.combatSkill);
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0001E3F0 File Offset: 0x0001C5F0
	public void CreateWoundClosestToPoint(Vector3 point, Vector3 normal, InteractablePreset woundPreset, MurderWeaponPreset weapon)
	{
		CitizenOutfitController.CharacterAnchor characterAnchor = CitizenOutfitController.CharacterAnchor.upperTorso;
		Vector3 nearestVert = base.GetNearestVert(point, out characterAnchor);
		Transform transform = this.outfitController.anchorReference[characterAnchor];
		if (transform != null)
		{
			Human.Wound wound = new Human.Wound();
			wound.humanID = this.humanID;
			wound.timestamp = SessionData.Instance.gameTime;
			wound.anchor = characterAnchor;
			wound.human = this;
			wound.bloodPoolAmount = weapon.bloodPoolAmount;
			wound.interactable = InteractableCreator.Instance.CreateTransformInteractable(woundPreset, transform, this, null, nearestVert, Quaternion.FromToRotation(Vector3.up, normal).eulerAngles + transform.eulerAngles, null);
			wound.interactable.objectRef = wound;
			wound.interactable.MarkAsTrash(true, false, 0f);
			this.currentWounds.Add(wound);
			if (this.visible && weapon != null && weapon.bulletImpactSpray != null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(weapon.bulletImpactSpray, PrefabControls.Instance.mapContainer);
				gameObject.transform.position = point;
				gameObject.transform.eulerAngles = Quaternion.FromToRotation(Vector3.up, normal).eulerAngles + transform.eulerAngles;
			}
		}
	}

	// Token: 0x04000254 RID: 596
	[Header("Citizen Attributes")]
	public bool alwaysPassDialogSuccess;

	// Token: 0x04000255 RID: 597
	[NonSerialized]
	public float customSort;
}
