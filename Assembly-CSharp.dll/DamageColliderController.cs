using System;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class DamageColliderController : MonoBehaviour
{
	// Token: 0x06000F64 RID: 3940 RVA: 0x000DB9D8 File Offset: 0x000D9BD8
	public void Setup(Actor newAttacker, Actor newTarget, float newDamage, Human newEnableKill, MurderWeaponPreset newWeapon)
	{
		Game.Log(string.Concat(new string[]
		{
			"Damage collider spawned for ",
			(newAttacker != null) ? newAttacker.ToString() : null,
			" aiming for ",
			(newTarget != null) ? newTarget.ToString() : null,
			" dmg: ",
			newDamage.ToString()
		}), 2);
		this.attacker = newAttacker;
		this.target = newTarget;
		this.damage = newDamage;
		this.enableKill = newEnableKill;
		this.weapon = newWeapon;
		if (this.attacker.isPlayer)
		{
			this.damage *= StatusController.Instance.damageOutgoingMultiplier;
		}
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x000DBA84 File Offset: 0x000D9C84
	private void OnCollisionEnter(Collision other)
	{
		Game.Log("OnCollisionEnter: " + other.gameObject.name, 2);
		Actor actor = other.gameObject.GetComponent<Actor>();
		if (actor == null)
		{
			actor = other.gameObject.GetComponentInParent<Actor>();
		}
		ContactPoint contact = other.GetContact(0);
		if (actor != null)
		{
			this.ProcessHit(actor, contact.point, contact.normal);
			return;
		}
		if (this.weapon.impactEvent != null)
		{
			AudioController.Instance.PlayWorldOneShot(this.weapon.impactEvent, null, null, contact.point, null, null, 1f, null, false, null, false);
		}
		BreakableWindowController component = other.gameObject.GetComponent<BreakableWindowController>();
		if (component != null)
		{
			component.BreakWindow(contact.point, -base.transform.up, this.attacker, false);
			return;
		}
		foreach (InteractableController interactableController in other.gameObject.GetComponentsInChildren<InteractableController>())
		{
			if (interactableController.interactable.preset.physicsProfile != null && interactableController.interactable.preset.reactWithExternalStimuli)
			{
				interactableController.SetPhysics(true, this.attacker);
				if (interactableController.rb != null)
				{
					Vector3 vector = -base.transform.up * GameplayControls.Instance.throwForce;
					interactableController.rb.AddForce(vector, 2);
				}
			}
		}
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x000DBC0E File Offset: 0x000D9E0E
	private void OnControllerColliderHit(ControllerColliderHit other)
	{
		Game.Log("OnControllerColliderHit: " + other.gameObject.name, 2);
		this.ProcessHit(Player.Instance, other.point, other.normal);
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x000DBC44 File Offset: 0x000D9E44
	private void OnTriggerEnter(Collider other)
	{
		Game.Log("OnTriggerEnter: " + other.gameObject.name, 2);
		Actor actor = other.gameObject.GetComponent<Actor>();
		if (actor == null)
		{
			actor = other.gameObject.GetComponentInParent<Actor>();
		}
		if (actor != null && actor.isPlayer)
		{
			this.ProcessHit(actor, base.transform.position, CameraController.Instance.cam.transform.forward * -1f);
		}
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x000DBCD0 File Offset: 0x000D9ED0
	private void ProcessHit(Actor hit, Vector3 contactPoint, Vector3 contactNormal)
	{
		if (hit == this.attacker)
		{
			return;
		}
		bool flag = false;
		if (this.enableKill == hit)
		{
			flag = true;
		}
		if (hit != this.target && Toolbox.Instance.Rand(0f, 1f, false) <= this.attacker.combatSkill)
		{
			return;
		}
		Game.Log(string.Concat(new string[]
		{
			"Dmg Collider: ",
			this.attacker.name,
			" dealt ",
			this.damage.ToString(),
			" to ",
			hit.name,
			" (kill allowed: ",
			flag.ToString(),
			")"
		}), 2);
		if (hit.isPlayer)
		{
			if (this.weapon.impactEventPlayer != null)
			{
				AudioController.Instance.PlayWorldOneShot(this.weapon.impactEventPlayer, hit, hit.currentNode, contactPoint, null, null, 1f, null, false, null, false);
			}
		}
		else if (this.weapon.impactEventBody != null)
		{
			AudioController.Instance.PlayWorldOneShot(this.weapon.impactEventBody, hit, hit.currentNode, contactPoint, null, null, 1f, null, false, null, false);
		}
		if (this.weapon.entryWound != null)
		{
			Citizen citizen = hit as Citizen;
			if (citizen != null && !citizen.isPlayer)
			{
				citizen.CreateWoundClosestToPoint(contactPoint, contactNormal, this.weapon.entryWound, this.weapon);
			}
		}
		if (this.weapon != null && this.weapon.applyPoison > 0f)
		{
			Citizen citizen2 = hit as Citizen;
			if (citizen2 != null)
			{
				citizen2.AddPoisoned(this.weapon.applyPoison, this.attacker as Human);
			}
		}
		hit.RecieveDamage(this.damage, this.attacker, contactPoint, -base.transform.up, this.weapon.forwardSpatter, this.weapon.backSpatter, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, flag, true, 1f);
		if (hit.currentHealth > 0f && hit.ai != null)
		{
			hit.ai.AITick(true, false);
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x040012C1 RID: 4801
	public Collider coll;

	// Token: 0x040012C2 RID: 4802
	public Actor attacker;

	// Token: 0x040012C3 RID: 4803
	public Actor target;

	// Token: 0x040012C4 RID: 4804
	public float damage = 0.1f;

	// Token: 0x040012C5 RID: 4805
	public Human enableKill;

	// Token: 0x040012C6 RID: 4806
	public MurderWeaponPreset weapon;
}
