using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000604 RID: 1540
public class UpgradeEffectController : MonoBehaviour
{
	// Token: 0x060021F6 RID: 8694 RVA: 0x001CDB4F File Offset: 0x001CBD4F
	public void OnInstall(UpgradesController.Upgrades disk, SyncDiskPreset.Effect effect, float value)
	{
		if (effect == SyncDiskPreset.Effect.starchLoan)
		{
			GameplayController.Instance.AddMoney(Mathf.RoundToInt(value), true, "sync disk");
		}
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x00002265 File Offset: 0x00000465
	public void OnUninstall(UpgradesController.Upgrades disk, SyncDiskPreset.Effect effect, float value)
	{
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x001CDB6B File Offset: 0x001CBD6B
	public void OnUpgrade(UpgradesController.Upgrades disk, SyncDiskPreset.UpgradeEffect effect, float value, int level)
	{
		if (effect == SyncDiskPreset.UpgradeEffect.reduceUninstallCost)
		{
			disk.uninstallCost -= Mathf.RoundToInt(value);
			disk.uninstallCost = Mathf.Max(disk.uninstallCost, 0);
		}
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x001CDB98 File Offset: 0x001CBD98
	public void OnSyncDiskChange(bool forceUpdate = false)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (!forceUpdate && !SessionData.Instance.startedGame)
		{
			return;
		}
		Player.Instance.SetMaxHealth(GameplayControls.Instance.baseMaxPlayerHealth + this.GetUpgradeEffect(SyncDiskPreset.Effect.increaseHealth) * GameplayControls.Instance.baseMaxPlayerHealth, false);
		Player.Instance.SetRecoveryRate(GameplayControls.Instance.playerRecoveryRate + this.GetUpgradeEffect(SyncDiskPreset.Effect.increaseRegeneration) * GameplayControls.Instance.playerRecoveryRate);
		FirstPersonItemController.Instance.SetSlotSize(GameplayControls.Instance.defaultInventorySlots + Mathf.RoundToInt(this.GetUpgradeEffect(SyncDiskPreset.Effect.increaseInventory)));
		Player.Instance.SetCombatHeft(GameplayControls.Instance.playerCombatHeft * (1f + this.GetUpgradeEffect(SyncDiskPreset.Effect.punchPowerModifier)));
		if (!Player.Instance.transitionActive)
		{
			if (!Player.Instance.inAirVent)
			{
				if (Player.Instance.isCrouched)
				{
					Player.Instance.SetPlayerHeight(Player.Instance.GetPlayerHeightCrouched(), true);
				}
				else
				{
					Player.Instance.SetPlayerHeight(Player.Instance.GetPlayerHeightNormal(), true);
				}
			}
			Player.Instance.RestorePlayerMovementSpeed();
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x060021FA RID: 8698 RVA: 0x001CDCB2 File Offset: 0x001CBEB2
	public static UpgradeEffectController Instance
	{
		get
		{
			return UpgradeEffectController._instance;
		}
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x001CDCB9 File Offset: 0x001CBEB9
	private void Awake()
	{
		if (UpgradeEffectController._instance != null && UpgradeEffectController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		UpgradeEffectController._instance = this;
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x001CDCE8 File Offset: 0x001CBEE8
	public float GetUpgradeEffect(SyncDiskPreset.Effect effect)
	{
		if (effect == SyncDiskPreset.Effect.awakenAtHome || effect == SyncDiskPreset.Effect.reduceMedicalCosts)
		{
			int num = 0;
			Chapter chapter;
			if (Toolbox.Instance.IsStoryMissionActive(out chapter, out num) && num < 31)
			{
				return 1f;
			}
		}
		float num2 = 0f;
		foreach (UpgradeEffectController.AppliedEffect appliedEffect in this.appliedEffects)
		{
			if (appliedEffect.effect == effect)
			{
				num2 += appliedEffect.value;
			}
		}
		return num2;
	}

	// Token: 0x04002C88 RID: 11400
	[Header("New Upgrades System")]
	public List<UpgradeEffectController.AppliedEffect> appliedEffects = new List<UpgradeEffectController.AppliedEffect>();

	// Token: 0x04002C89 RID: 11401
	private static UpgradeEffectController _instance;

	// Token: 0x02000605 RID: 1541
	[Serializable]
	public class AppliedEffect
	{
		// Token: 0x04002C8A RID: 11402
		public UpgradesController.Upgrades disk;

		// Token: 0x04002C8B RID: 11403
		public SyncDiskPreset.Effect effect;

		// Token: 0x04002C8C RID: 11404
		public float value;
	}
}
