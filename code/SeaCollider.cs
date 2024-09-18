using System;
using UnityEngine;

// Token: 0x02000487 RID: 1159
public class SeaCollider : MonoBehaviour
{
	// Token: 0x060018EB RID: 6379 RVA: 0x00171500 File Offset: 0x0016F700
	private void OnTriggerEnter(Collider other)
	{
		if (other == null)
		{
			return;
		}
		Player component = other.GetComponent<Player>();
		if (component != null)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("PLAYER IS IN SEA", 2);
			}
			component.RecieveDamage(999999f, null, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
			InterfaceController.Instance.Fade(1f, 0.0001f, false);
			return;
		}
		InteractableController interactableController = other.gameObject.GetComponentInParent<InteractableController>();
		if (interactableController != null)
		{
			if (AchievementsController.Instance != null && interactableController.interactable != null && interactableController.interactable.preset != null && interactableController.interactable.preset.weapon != null)
			{
				MurderWeaponPreset.WeaponType type = interactableController.interactable.preset.weapon.type;
				if (type == MurderWeaponPreset.WeaponType.handgun || type == MurderWeaponPreset.WeaponType.rifle || type == MurderWeaponPreset.WeaponType.shotgun)
				{
					AchievementsController.Instance.UnlockAchievement("Sleeping with the Fishes", "throw_gun_in_sea");
				}
			}
			if (Game.Instance.printDebug)
			{
				Game.Log(interactableController.interactable.GetName() + " IN SEA", 2);
			}
			interactableController.interactable.SafeDelete(false);
			return;
		}
		interactableController = other.gameObject.GetComponentInChildren<InteractableController>();
		if (interactableController != null && interactableController.interactable != null)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log(interactableController.interactable.GetName() + " IN SEA", 2);
			}
			interactableController.interactable.SafeDelete(false);
		}
	}
}
