using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000796 RID: 1942
[CreateAssetMenu(fileName = "murderweapons_data", menuName = "Database/Murder Weapons Pool")]
public class MurderWeaponsPool : SoCustomComparison
{
	// Token: 0x04003AD1 RID: 15057
	[InfoBox("The killer will pick one of these to kill ALL their victims...", 0)]
	public List<MurderWeaponsPool.MurderWeaponPick> murderWeaponPool = new List<MurderWeaponsPool.MurderWeaponPick>();

	// Token: 0x02000797 RID: 1943
	[Serializable]
	public class MurderWeaponPick
	{
		// Token: 0x04003AD2 RID: 15058
		[Tooltip("The weapon itself")]
		public InteractablePreset weapon;

		// Token: 0x04003AD3 RID: 15059
		[Tooltip("Chance of killer dropping this at scene")]
		[Range(0f, 1f)]
		public float chanceOfDroppingAtScene;

		// Token: 0x04003AD4 RID: 15060
		[Space(7f)]
		public Vector2 randomScoreRange = new Vector2(0f, 1f);

		// Token: 0x04003AD5 RID: 15061
		public List<MurderPreset.MurdererModifierRule> traitModifiers = new List<MurderPreset.MurdererModifierRule>();
	}
}
