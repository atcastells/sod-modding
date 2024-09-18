using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000606 RID: 1542
public class UpgradesController : MonoBehaviour
{
	// Token: 0x17000105 RID: 261
	// (get) Token: 0x060021FF RID: 8703 RVA: 0x001CDD8B File Offset: 0x001CBF8B
	public static UpgradesController Instance
	{
		get
		{
			return UpgradesController._instance;
		}
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x001CDD92 File Offset: 0x001CBF92
	private void Awake()
	{
		if (UpgradesController._instance != null && UpgradesController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		UpgradesController._instance = this;
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x001CDDC0 File Offset: 0x001CBFC0
	public void SetupQuickRef()
	{
		foreach (SyncDiskPreset syncDiskPreset in Toolbox.Instance.allSyncDisks)
		{
			this.upgradesQuickRef.Add(syncDiskPreset.name, syncDiskPreset);
		}
	}

	// Token: 0x06002202 RID: 8706 RVA: 0x001CDE24 File Offset: 0x001CC024
	public void Setup()
	{
		if (this.upgradesQuickRef.Count <= 0)
		{
			this.SetupQuickRef();
		}
		this.upgrades.Clear();
		Game.Log("Gameplay: Setup upgrades, give all: " + Game.Instance.giveAllUpgrades.ToString(), 2);
		this.syncClinicPromptText.text = "*" + Strings.Get("ui.interface", "Find a sync clinic to install, uninstall or upgrade sync disks", Strings.Casing.asIs, false, false, true, Player.Instance);
		this.configText.text = Strings.Get("ui.interface", "Configuration", Strings.Casing.asIs, false, false, false, null);
		this.upgradesText.text = Strings.Get("ui.interface", "Upgrades", Strings.Casing.asIs, false, false, false, null);
		this.sideEffectsText.text = Strings.Get("ui.interface", "Side Effect", Strings.Casing.asIs, false, false, false, null);
		this.descriptionText.text = Strings.Get("ui.interface", "Description", Strings.Casing.asIs, false, false, false, null);
		this.optionsText.text = Strings.Get("ui.interface", "Options", Strings.Casing.asIs, false, false, false, null);
		this.UpdateUpgrades();
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x001CDF40 File Offset: 0x001CC140
	public void UpdateUpgrades()
	{
		Game.Log("Interface: Update upgrades...", 2);
		List<UpgradesController.Upgrades> list = new List<UpgradesController.Upgrades>(this.upgrades);
		this.upgradeVials.Clear();
		this.notInstalled = 0;
		foreach (FirstPersonItemController.InventorySlot inventorySlot in FirstPersonItemController.Instance.slots)
		{
			Interactable interactable = inventorySlot.GetInteractable();
			if (interactable != null)
			{
				if (interactable.syncDisk != null)
				{
					UpgradesController.Upgrades upgrades = new UpgradesController.Upgrades
					{
						upgrade = interactable.syncDisk.name,
						state = UpgradesController.SyncDiskState.notInstalled,
						objId = interactable.id,
						uninstallCost = interactable.syncDisk.uninstallCost
					};
					list.Add(upgrades);
					this.notInstalled++;
				}
				else if (interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.syncDiskUpgrade && !this.upgradeVials.Contains(interactable))
				{
					this.upgradeVials.Add(interactable);
				}
			}
		}
		for (int i = 0; i < this.spawnedDisks.Count; i++)
		{
			SyncDiskElementController syncDiskElementController = this.spawnedDisks[i];
			if (list.Contains(syncDiskElementController.upgrade))
			{
				syncDiskElementController.VisualUpdate();
				list.Remove(syncDiskElementController.upgrade);
			}
			else
			{
				Object.Destroy(syncDiskElementController.gameObject);
				this.spawnedDisks.RemoveAt(i);
				i--;
			}
		}
		foreach (UpgradesController.Upgrades newUpgrade in list)
		{
			SyncDiskElementController component = Object.Instantiate<GameObject>(this.syncDiskElementPrefab, this.listRect).GetComponent<SyncDiskElementController>();
			if (this.upgradesQuickRef.Count <= 0)
			{
				this.SetupQuickRef();
			}
			component.Setup(newUpgrade);
			this.spawnedDisks.Add(component);
		}
		this.spawnedDisks.Sort((SyncDiskElementController p2, SyncDiskElementController p1) => (p1.upgrade.list - Mathf.Min((int)p1.upgrade.state, 1) * 10000).CompareTo(p2.upgrade.list - Mathf.Min((int)p2.upgrade.state, 1) * 10000));
		float num = -16f;
		for (int j = 0; j < this.spawnedDisks.Count; j++)
		{
			SyncDiskElementController syncDiskElementController2 = this.spawnedDisks[j];
			syncDiskElementController2.rect.localPosition = new Vector2(syncDiskElementController2.rect.localPosition.x, num);
			num -= 180f;
			num -= 0f;
		}
		num -= 16f;
		this.listRect.sizeDelta = new Vector2(this.listRect.sizeDelta.x, -num);
		this.listContentRect.sizeDelta = new Vector2(this.listContentRect.sizeDelta.x, Mathf.Max(736f, this.listRect.sizeDelta.y + 120f));
		this.UpdateInstalledAvailableText();
		this.UpdateNavigation();
		this.UpdateActivation();
		InterfaceController.Instance.upgradesButton.notifications.SetNotifications(this.notInstalled + this.upgradeVials.Count);
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x001CE270 File Offset: 0x001CC470
	public void InstallSyncDisk(UpgradesController.Upgrades application, int option)
	{
		if (this.upgrades.Exists((UpgradesController.Upgrades item) => item.upgrade == application.upgrade))
		{
			return;
		}
		application.state = (UpgradesController.SyncDiskState)option;
		application.list = 0;
		application.level = 0;
		foreach (UpgradesController.Upgrades upgrades in this.upgrades)
		{
			upgrades.list++;
		}
		this.upgrades.Insert(0, application);
		Interactable @object = application.GetObject();
		if (@object != null)
		{
			@object.SafeDelete(true);
			application.objId = -1;
		}
		else
		{
			Game.LogError("Unable to get sync disk object! Cannot delete...", 2);
		}
		AudioController.Instance.Play2DSound(AudioControls.Instance.syncDiskInstall, null, 1f);
		Player.Instance.AddSyncDiskInstall(1f);
		this.playSyncDiskInstallAudio = true;
		StatusController.Instance.ForceStatusCheck();
		this.UpdateUpgrades();
		foreach (UpgradeEffectController.AppliedEffect appliedEffect in application.GetMainEffects())
		{
			UpgradeEffectController.Instance.OnInstall(application, appliedEffect.effect, appliedEffect.value);
		}
		if (AchievementsController.Instance != null)
		{
			if (this.upgrades.Count >= Toolbox.Instance.allSyncDisks.FindAll((SyncDiskPreset item) => !item.disabled).Count)
			{
				AchievementsController.Instance.UnlockAchievement("Fully Synchronised", "all_sync_disks");
			}
		}
	}

	// Token: 0x06002205 RID: 8709 RVA: 0x001CE458 File Offset: 0x001CC658
	public void UninstallSyncDisk(UpgradesController.Upgrades removal)
	{
		this.upgrades.Remove(removal);
		this.UpdateUpgrades();
		AudioController.Instance.Play2DSound(AudioControls.Instance.syncDiskUninstall, null, 1f);
		foreach (UpgradeEffectController.AppliedEffect appliedEffect in removal.GetMainEffects())
		{
			UpgradeEffectController.Instance.OnUninstall(removal, appliedEffect.effect, appliedEffect.value);
		}
		if (AchievementsController.Instance != null && removal.upgrade == "Starch-SugarDaddy")
		{
			AchievementsController.Instance.UnlockAchievement("Gone Daddy Gone", "pay_starch_loan");
		}
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x001CE51C File Offset: 0x001CC71C
	public void UpgradeSyncDisk(UpgradesController.Upgrades upgradeThis)
	{
		if (upgradeThis != null && this.upgradeVials.Count > 0)
		{
			upgradeThis.level++;
			this.upgradeVials[0].SafeDelete(true);
			this.UpdateUpgrades();
			AudioController.Instance.Play2DSound(AudioControls.Instance.syncDiskUpgrade, null, 1f);
			upgradeThis.GetPreset();
			if (upgradeThis.level == 1)
			{
				try
				{
					if (upgradeThis.state == UpgradesController.SyncDiskState.option1)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option1UpgradeEffects[0], upgradeThis.preset.option1UpgradeValues[0], 1);
					}
					else if (upgradeThis.state == UpgradesController.SyncDiskState.option2)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option2UpgradeEffects[0], upgradeThis.preset.option2UpgradeValues[0], 1);
					}
					else if (upgradeThis.state == UpgradesController.SyncDiskState.option3)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option3UpgradeEffects[0], upgradeThis.preset.option3UpgradeValues[0], 1);
					}
					return;
				}
				catch
				{
					Game.LogError("Could not execute one-shot upgrade effect for " + upgradeThis.preset.name, 2);
					return;
				}
			}
			if (upgradeThis.level == 2)
			{
				try
				{
					if (upgradeThis.state == UpgradesController.SyncDiskState.option1)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option1UpgradeEffects[1], upgradeThis.preset.option1UpgradeValues[1], 2);
					}
					else if (upgradeThis.state == UpgradesController.SyncDiskState.option2)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option2UpgradeEffects[1], upgradeThis.preset.option2UpgradeValues[1], 2);
					}
					else if (upgradeThis.state == UpgradesController.SyncDiskState.option3)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option3UpgradeEffects[1], upgradeThis.preset.option3UpgradeValues[1], 2);
					}
					return;
				}
				catch
				{
					Game.LogError("Could not execute one-shot upgrade effect for " + upgradeThis.preset.name, 2);
					return;
				}
			}
			if (upgradeThis.level == 3)
			{
				try
				{
					if (upgradeThis.state == UpgradesController.SyncDiskState.option1)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option1UpgradeEffects[2], upgradeThis.preset.option1UpgradeValues[2], 3);
					}
					else if (upgradeThis.state == UpgradesController.SyncDiskState.option2)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option2UpgradeEffects[2], upgradeThis.preset.option2UpgradeValues[2], 3);
					}
					else if (upgradeThis.state == UpgradesController.SyncDiskState.option3)
					{
						UpgradeEffectController.Instance.OnUpgrade(upgradeThis, upgradeThis.preset.option3UpgradeEffects[2], upgradeThis.preset.option3UpgradeValues[2], 3);
					}
				}
				catch
				{
					Game.LogError("Could not execute one-shot upgrade effect for " + upgradeThis.preset.name, 2);
				}
			}
		}
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x001CE83C File Offset: 0x001CCA3C
	public void UpdateInstallButton(bool newInstallAllowed)
	{
		this.installedAllowed = newInstallAllowed;
		foreach (SyncDiskElementController syncDiskElementController in this.spawnedDisks)
		{
			syncDiskElementController.SetInstallAllowed(this.installedAllowed);
		}
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x001CE89C File Offset: 0x001CCA9C
	public void UpdateInstalledAvailableText()
	{
		TMP_Text tmp_Text = this.installedDisksText;
		string[] array = new string[13];
		array[0] = Strings.Get("ui.interface", "Installed Disks", Strings.Casing.asIs, false, false, false, null);
		array[1] = ": ";
		array[2] = this.upgrades.FindAll((UpgradesController.Upgrades item) => item.state > UpgradesController.SyncDiskState.notInstalled).Count.ToString();
		array[3] = "/";
		array[4] = Toolbox.Instance.allSyncDisks.Count.ToString();
		array[5] = ", ";
		array[6] = Strings.Get("ui.interface", "Available Disks", Strings.Casing.asIs, false, false, false, null);
		array[7] = ": ";
		array[8] = this.notInstalled.ToString();
		array[9] = ", ";
		array[10] = Strings.Get("ui.interface", "Available Upgrades", Strings.Casing.asIs, false, false, false, null);
		array[11] = ": ";
		array[12] = this.upgradeVials.Count.ToString();
		tmp_Text.text = string.Concat(array);
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x001CE9B4 File Offset: 0x001CCBB4
	public void OpenUpgrades(bool playSound = true)
	{
		InterfaceController.Instance.upgradesCanvas.gameObject.SetActive(true);
		base.enabled = true;
		this.isOpen = true;
		if (BioScreenController.Instance.isOpen)
		{
			BioScreenController.Instance.SetInventoryOpen(false, false, true);
		}
		base.StopCoroutine("Open");
		base.StopCoroutine("Close");
		base.StartCoroutine("Open");
		if (playSound)
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.mapSlideIn, null, 1f);
		}
		this.UpdateNavigation();
		if (!InputController.Instance.mouseInputMode)
		{
			CasePanelController.Instance.SetControllerMode(true, CasePanelController.ControllerSelectMode.topBar);
		}
		InterfaceController.Instance.upgradesButton.icon.enabled = this.isOpen;
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x001CEA75 File Offset: 0x001CCC75
	private IEnumerator Open()
	{
		while (this.openProgress < 1f)
		{
			this.openProgress += Time.deltaTime * 20f;
			this.openProgress = Mathf.Clamp01(this.openProgress);
			InterfaceController.Instance.upgradesCanvasGroup.alpha = this.openProgress;
			yield return null;
		}
		this.mainScrollRect.enabled = true;
		yield break;
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x001CEA84 File Offset: 0x001CCC84
	public void CloseUpgrades(bool playSound = true)
	{
		this.isOpen = false;
		base.StopCoroutine("Open");
		base.StopCoroutine("Close");
		base.StartCoroutine("Close");
		foreach (SyncDiskElementController syncDiskElementController in this.spawnedDisks)
		{
			syncDiskElementController.SelectOptionButton(-1);
		}
		if (playSound)
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.mapSlideOut, null, 1f);
		}
		Navigation navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation2 = navigation;
		navigation2.selectOnLeft = InterfaceController.Instance.notebookButton.button.FindSelectableOnLeft();
		navigation2.selectOnRight = InterfaceController.Instance.notebookButton.button.FindSelectableOnRight();
		InterfaceController.Instance.notebookButton.button.navigation = navigation2;
		navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation3 = navigation;
		navigation3.selectOnLeft = InterfaceController.Instance.upgradesButton.button.FindSelectableOnLeft();
		navigation3.selectOnRight = InterfaceController.Instance.upgradesButton.button.FindSelectableOnRight();
		InterfaceController.Instance.upgradesButton.button.navigation = navigation3;
		navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation4 = navigation;
		navigation4.selectOnLeft = InterfaceController.Instance.mapButton.button.FindSelectableOnLeft();
		navigation4.selectOnRight = InterfaceController.Instance.mapButton.button.FindSelectableOnRight();
		InterfaceController.Instance.mapButton.button.navigation = navigation4;
		navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation5 = navigation;
		navigation5.selectOnLeft = CasePanelController.Instance.selectNoCaseButton.button.FindSelectableOnLeft();
		navigation5.selectOnRight = CasePanelController.Instance.selectNoCaseButton.button.FindSelectableOnRight();
		CasePanelController.Instance.selectNoCaseButton.button.navigation = navigation5;
		navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation6 = navigation;
		navigation6.selectOnLeft = CasePanelController.Instance.stickNoteButton.button.FindSelectableOnLeft();
		navigation6.selectOnRight = CasePanelController.Instance.stickNoteButton.button.FindSelectableOnRight();
		CasePanelController.Instance.stickNoteButton.button.navigation = navigation6;
		InterfaceController.Instance.upgradesButton.icon.enabled = this.isOpen;
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x001CED10 File Offset: 0x001CCF10
	private IEnumerator Close()
	{
		this.mainScrollRect.enabled = false;
		while (this.openProgress > 0f)
		{
			this.openProgress -= Time.deltaTime * 20f;
			this.openProgress = Mathf.Clamp01(this.openProgress);
			InterfaceController.Instance.upgradesCanvasGroup.alpha = this.openProgress;
			yield return null;
		}
		base.enabled = false;
		InterfaceController.Instance.upgradesCanvas.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x001CED20 File Offset: 0x001CCF20
	public void UpdateActivation()
	{
		UpgradeEffectController.Instance.appliedEffects.Clear();
		Game.Log("Gameplay: Updating sync disk activation...", 2);
		foreach (UpgradesController.Upgrades upgrades in this.upgrades)
		{
			if (upgrades.state != UpgradesController.SyncDiskState.notInstalled)
			{
				UpgradeEffectController.Instance.appliedEffects.AddRange(upgrades.GetAllEffects());
			}
		}
		foreach (SocialControls.SocialCreditBuff socialCreditBuff in GameplayController.Instance.socialCreditPerks)
		{
			if (socialCreditBuff != null)
			{
				UpgradeEffectController.Instance.appliedEffects.Add(socialCreditBuff.GetEffect());
			}
		}
		UpgradeEffectController.Instance.OnSyncDiskChange(false);
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x001CEE08 File Offset: 0x001CD008
	public void UpdateNavigation()
	{
		for (int i = 0; i < this.spawnedDisks.Count; i++)
		{
			this.spawnedDisks[i] == null;
		}
	}

	// Token: 0x04002C8D RID: 11405
	[Header("Components")]
	public RectTransform mainContentRect;

	// Token: 0x04002C8E RID: 11406
	public RectTransform mainViewport;

	// Token: 0x04002C8F RID: 11407
	public CustomScrollRect mainScrollRect;

	// Token: 0x04002C90 RID: 11408
	public RectTransform listContentRect;

	// Token: 0x04002C91 RID: 11409
	public RectTransform listRect;

	// Token: 0x04002C92 RID: 11410
	[Space(7f)]
	public ButtonController closeButton;

	// Token: 0x04002C93 RID: 11411
	public TextMeshProUGUI installedDisksText;

	// Token: 0x04002C94 RID: 11412
	public TextMeshProUGUI syncClinicPromptText;

	// Token: 0x04002C95 RID: 11413
	public TextMeshProUGUI configText;

	// Token: 0x04002C96 RID: 11414
	public TextMeshProUGUI upgradesText;

	// Token: 0x04002C97 RID: 11415
	public TextMeshProUGUI sideEffectsText;

	// Token: 0x04002C98 RID: 11416
	public TextMeshProUGUI descriptionText;

	// Token: 0x04002C99 RID: 11417
	public TextMeshProUGUI optionsText;

	// Token: 0x04002C9A RID: 11418
	public GameObject syncDiskElementPrefab;

	// Token: 0x04002C9B RID: 11419
	[Header("State")]
	[NonSerialized]
	public float openProgress;

	// Token: 0x04002C9C RID: 11420
	public bool isOpen;

	// Token: 0x04002C9D RID: 11421
	public bool installedAllowed;

	// Token: 0x04002C9E RID: 11422
	public int notInstalled;

	// Token: 0x04002C9F RID: 11423
	public bool playSyncDiskInstallAudio;

	// Token: 0x04002CA0 RID: 11424
	public List<UpgradesController.Upgrades> upgrades = new List<UpgradesController.Upgrades>();

	// Token: 0x04002CA1 RID: 11425
	public List<SyncDiskElementController> spawnedDisks = new List<SyncDiskElementController>();

	// Token: 0x04002CA2 RID: 11426
	public Dictionary<string, SyncDiskPreset> upgradesQuickRef = new Dictionary<string, SyncDiskPreset>();

	// Token: 0x04002CA3 RID: 11427
	public List<Interactable> upgradeVials = new List<Interactable>();

	// Token: 0x04002CA4 RID: 11428
	private static UpgradesController _instance;

	// Token: 0x02000607 RID: 1543
	public enum SyncDiskState
	{
		// Token: 0x04002CA6 RID: 11430
		notInstalled,
		// Token: 0x04002CA7 RID: 11431
		option1,
		// Token: 0x04002CA8 RID: 11432
		option2,
		// Token: 0x04002CA9 RID: 11433
		option3
	}

	// Token: 0x02000608 RID: 1544
	[Serializable]
	public class Upgrades
	{
		// Token: 0x06002210 RID: 8720 RVA: 0x001CEE72 File Offset: 0x001CD072
		public SyncDiskPreset GetPreset()
		{
			if (this.preset != null)
			{
				return this.preset;
			}
			Toolbox.Instance.LoadDataFromResources<SyncDiskPreset>(this.upgrade, out this.preset);
			return this.preset;
		}

		// Token: 0x06002211 RID: 8721 RVA: 0x001CEEA8 File Offset: 0x001CD0A8
		public Interactable GetObject()
		{
			Interactable result = null;
			CityData.Instance.savableInteractableDictionary.TryGetValue(this.objId, ref result);
			return result;
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x001CEED0 File Offset: 0x001CD0D0
		public List<UpgradeEffectController.AppliedEffect> GetAllEffects()
		{
			this.GetPreset();
			Game.Log("Gameplay: Getting all effects for " + this.preset.name + "...", 2);
			List<UpgradeEffectController.AppliedEffect> list = new List<UpgradeEffectController.AppliedEffect>(this.GetMainEffects());
			list.AddRange(this.GetUpgradeEffects());
			list.AddRange(this.GetSideEffects());
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: ",
				list.Count.ToString(),
				" total effects for ",
				this.preset.name,
				"..."
			}), 2);
			return list;
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x001CEF74 File Offset: 0x001CD174
		public List<UpgradeEffectController.AppliedEffect> GetMainEffects()
		{
			this.GetPreset();
			Game.Log("Gameplay: Getting main effects for " + this.preset.name + "...", 2);
			List<UpgradeEffectController.AppliedEffect> list = new List<UpgradeEffectController.AppliedEffect>();
			if (this.state == UpgradesController.SyncDiskState.option1)
			{
				list.Add(new UpgradeEffectController.AppliedEffect
				{
					effect = this.preset.mainEffect1,
					value = this.GetEffectiveness(),
					disk = this
				});
			}
			else if (this.state == UpgradesController.SyncDiskState.option2)
			{
				list.Add(new UpgradeEffectController.AppliedEffect
				{
					effect = this.preset.mainEffect2,
					value = this.GetEffectiveness(),
					disk = this
				});
			}
			else if (this.state == UpgradesController.SyncDiskState.option3)
			{
				list.Add(new UpgradeEffectController.AppliedEffect
				{
					effect = this.preset.mainEffect3,
					value = this.GetEffectiveness(),
					disk = this
				});
			}
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: Returning ",
				list.Count.ToString(),
				" main effects for ",
				this.preset.name,
				"..."
			}), 2);
			return list;
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x001CF0A4 File Offset: 0x001CD2A4
		public List<UpgradeEffectController.AppliedEffect> GetUpgradeEffects()
		{
			this.GetPreset();
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: Getting upgrade effects for ",
				this.preset.name,
				": ",
				this.state.ToString(),
				" level ",
				this.level.ToString(),
				"..."
			}), 2);
			List<SyncDiskPreset.UpgradeEffect> list = new List<SyncDiskPreset.UpgradeEffect>();
			List<float> list2 = new List<float>();
			if (this.level > 0)
			{
				if (this.state == UpgradesController.SyncDiskState.option1)
				{
					if (this.level >= 1 && this.preset.option1UpgradeEffects.Count > 0)
					{
						list.Add(this.preset.option1UpgradeEffects[0]);
						list2.Add(this.preset.option1UpgradeValues[0]);
					}
					if (this.level >= 2 && this.preset.option1UpgradeEffects.Count > 1)
					{
						list.Add(this.preset.option1UpgradeEffects[1]);
						list2.Add(this.preset.option1UpgradeValues[1]);
					}
					if (this.level >= 3 && this.preset.option1UpgradeEffects.Count > 2)
					{
						list.Add(this.preset.option1UpgradeEffects[2]);
						list2.Add(this.preset.option1UpgradeValues[2]);
					}
				}
				else if (this.state == UpgradesController.SyncDiskState.option2)
				{
					if (this.level >= 1 && this.preset.option2UpgradeEffects.Count > 0)
					{
						list.Add(this.preset.option2UpgradeEffects[0]);
						list2.Add(this.preset.option2UpgradeValues[0]);
					}
					if (this.level >= 2 && this.preset.option2UpgradeEffects.Count > 1)
					{
						list.Add(this.preset.option2UpgradeEffects[1]);
						list2.Add(this.preset.option2UpgradeValues[1]);
					}
					if (this.level >= 3 && this.preset.option2UpgradeEffects.Count > 2)
					{
						list.Add(this.preset.option2UpgradeEffects[2]);
						list2.Add(this.preset.option2UpgradeValues[2]);
					}
				}
				else if (this.state == UpgradesController.SyncDiskState.option3)
				{
					if (this.level >= 1 && this.preset.option3UpgradeEffects.Count > 0)
					{
						list.Add(this.preset.option3UpgradeEffects[0]);
						list2.Add(this.preset.option3UpgradeValues[0]);
					}
					if (this.level >= 2 && this.preset.option3UpgradeEffects.Count > 1)
					{
						list.Add(this.preset.option3UpgradeEffects[1]);
						list2.Add(this.preset.option3UpgradeValues[1]);
					}
					if (this.level >= 3 && this.preset.option3UpgradeEffects.Count > 2)
					{
						list.Add(this.preset.option3UpgradeEffects[2]);
						list2.Add(this.preset.option3UpgradeValues[2]);
					}
				}
			}
			Game.Log("Gameplay: Parsing from " + list.Count.ToString() + " effects...", 2);
			List<UpgradeEffectController.AppliedEffect> list3 = new List<UpgradeEffectController.AppliedEffect>();
			for (int i = 0; i < list.Count; i++)
			{
				SyncDiskPreset.UpgradeEffect upgradeEffect = list[i];
				float value = list2[i];
				if (upgradeEffect == SyncDiskPreset.UpgradeEffect.readingSeriesBonus)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.readingSeriesBonus,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.bothConfigurations)
				{
					List<UpgradeEffectController.AppliedEffect> mainEffects = this.GetMainEffects();
					if (!mainEffects.Exists((UpgradeEffectController.AppliedEffect item) => item.effect == this.preset.mainEffect1) && this.preset.mainEffect1 != SyncDiskPreset.Effect.none)
					{
						list3.Add(new UpgradeEffectController.AppliedEffect
						{
							effect = this.preset.mainEffect1,
							value = this.preset.mainEffect1Value,
							disk = this
						});
					}
					if (!mainEffects.Exists((UpgradeEffectController.AppliedEffect item) => item.effect == this.preset.mainEffect2) && this.preset.mainEffect2 != SyncDiskPreset.Effect.none)
					{
						list3.Add(new UpgradeEffectController.AppliedEffect
						{
							effect = this.preset.mainEffect2,
							value = this.preset.mainEffect2Value,
							disk = this
						});
					}
					if (!mainEffects.Exists((UpgradeEffectController.AppliedEffect item) => item.effect == this.preset.mainEffect3) && this.preset.mainEffect3 != SyncDiskPreset.Effect.none)
					{
						list3.Add(new UpgradeEffectController.AppliedEffect
						{
							effect = this.preset.mainEffect3,
							value = this.preset.mainEffect3Value,
							disk = this
						});
					}
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.reduceMedicalCosts)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.reduceMedicalCosts,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.accidentCover)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.accidentCover,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.legalInsurance)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.legalInsurance,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.awakenAtHome)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.awakenAtHome,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.increaseHealth)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.increaseHealth,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.increaseInventory)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.increaseInventory,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.increaseRegeneration)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.increaseRegeneration,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.priceModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.priceModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.dialogChanceModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.dialogChanceModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.doorBargeModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.doorBargeModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.fallDamageModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.fallDamageModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.sideJobPayModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.sideJobPayModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.throwPowerModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.throwPowerModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.punchPowerModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.punchPowerModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.blockIncoming)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.blockIncoming,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.focusFromDamage)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.focusFromDamage,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.noBrokenBones)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.noBrokenBones,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.reachModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.reachModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.holdingBlocksBullets)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.holdingBlocksBullets,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.fistsThreatModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.fistsThreatModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.noBleeding)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.noBleeding,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.incomingDamageModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.incomingDamageModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.passiveIncome)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.passiveIncome,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.installMalware)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.installMalware,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.malwareOwnerBonus)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.malwareOwnerBonus,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.agePerception)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.agePerception,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.footSizePerception)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.footSizePerception,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.heightPerception)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.heightPerception,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.salaryPerception)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.salaryPerception,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.singlePerception)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.singlePerception,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.wealthPerception)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.wealthPerception,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.starchAmbassador)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.starchAmbassador,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.starchGive)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.starchGive,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.lockpickingEfficiencyModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.lockpickingEfficiencyModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.lockpickingSpeedModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.lockpickingSpeedModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.triggerIllegalOnPick)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.triggerIllegalOnPick,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.securityBreakerModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.securityBreakerModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.KOTimeModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.KOTimeModifier,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.noSmelly)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.noSmelly,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.noCold)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.noCold,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.noTired)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.noTired,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.kitchenPhotos)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.kitchenPhotos,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.bathroomPhotos)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.bathroomPhotos,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.illegalOpsPhotos)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.illegalOpsPhotos,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.removeSideEffect)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.removeSideEffect,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.moneyForLocations)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.moneyForLocations,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.moneyForDucts)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.moneyForDucts,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.moneyForPasscodes)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.moneyForPasscodes,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.moneyForAddresses)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.moneyForAddresses,
						value = value
					});
				}
				else if (upgradeEffect == SyncDiskPreset.UpgradeEffect.playerHeightModifier)
				{
					list3.Add(new UpgradeEffectController.AppliedEffect
					{
						effect = SyncDiskPreset.Effect.playerHeightModifier,
						value = value
					});
				}
			}
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: Returning ",
				list3.Count.ToString(),
				" upgrade effects for ",
				this.preset.name,
				"..."
			}), 2);
			return list3;
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x001CFD38 File Offset: 0x001CDF38
		public List<UpgradeEffectController.AppliedEffect> GetSideEffects()
		{
			this.GetPreset();
			Game.Log("Gameplay: Getting side effects for " + this.preset.name + "...", 2);
			List<UpgradeEffectController.AppliedEffect> list = new List<UpgradeEffectController.AppliedEffect>();
			if (this.preset.sideEffect != SyncDiskPreset.Effect.none)
			{
				list.Add(new UpgradeEffectController.AppliedEffect
				{
					effect = this.preset.sideEffect,
					value = this.GetSideEffectValue(),
					disk = this
				});
			}
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: Returning ",
				list.Count.ToString(),
				" side effects for ",
				this.preset.name,
				"..."
			}), 2);
			return list;
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x001CFDF8 File Offset: 0x001CDFF8
		public float GetEffectiveness()
		{
			float num = 0f;
			this.GetPreset();
			if (this.state == UpgradesController.SyncDiskState.option1)
			{
				num = this.preset.mainEffect1Value;
				if (this.level >= 1 && this.preset.option1UpgradeEffects.Count > 0 && this.preset.option1UpgradeValues.Count > 0 && this.preset.option1UpgradeEffects[0] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option1UpgradeValues[0];
				}
				if (this.level >= 2 && this.preset.option1UpgradeEffects.Count > 1 && this.preset.option1UpgradeValues.Count > 1 && this.preset.option1UpgradeEffects[1] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option1UpgradeValues[1];
				}
				if (this.level >= 3 && this.preset.option1UpgradeEffects.Count > 2 && this.preset.option1UpgradeValues.Count > 2 && this.preset.option1UpgradeEffects[2] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option1UpgradeValues[2];
				}
			}
			else if (this.state == UpgradesController.SyncDiskState.option2)
			{
				num = this.preset.mainEffect2Value;
				if (this.level >= 1 && this.preset.option2UpgradeEffects.Count > 0 && this.preset.option2UpgradeValues.Count > 0 && this.preset.option2UpgradeEffects[0] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option2UpgradeValues[0];
				}
				if (this.level >= 2 && this.preset.option2UpgradeEffects.Count > 1 && this.preset.option2UpgradeValues.Count > 1 && this.preset.option2UpgradeEffects[1] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option2UpgradeValues[1];
				}
				if (this.level >= 3 && this.preset.option2UpgradeEffects.Count > 2 && this.preset.option2UpgradeValues.Count > 2 && this.preset.option2UpgradeEffects[2] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option2UpgradeValues[2];
				}
			}
			else if (this.state == UpgradesController.SyncDiskState.option3)
			{
				num = this.preset.mainEffect3Value;
				if (this.level >= 1 && this.preset.option3UpgradeEffects.Count > 0 && this.preset.option3UpgradeValues.Count > 0 && this.preset.option3UpgradeEffects[0] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option3UpgradeValues[0];
				}
				if (this.level >= 2 && this.preset.option3UpgradeEffects.Count > 1 && this.preset.option3UpgradeValues.Count > 1 && this.preset.option3UpgradeEffects[1] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option3UpgradeValues[1];
				}
				if (this.level >= 3 && this.preset.option3UpgradeEffects.Count > 2 && this.preset.option3UpgradeValues.Count > 2 && this.preset.option3UpgradeEffects[2] == SyncDiskPreset.UpgradeEffect.modifyEffect)
				{
					num += this.preset.option3UpgradeValues[2];
				}
			}
			return num;
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x001D018C File Offset: 0x001CE38C
		public float GetSideEffectValue()
		{
			this.GetPreset();
			if (this.GetUpgradeEffects().Exists((UpgradeEffectController.AppliedEffect item) => item.effect == SyncDiskPreset.Effect.removeSideEffect))
			{
				return 0f;
			}
			return this.preset.sideEffectValue;
		}

		// Token: 0x04002CAA RID: 11434
		public string upgrade;

		// Token: 0x04002CAB RID: 11435
		public UpgradesController.SyncDiskState state;

		// Token: 0x04002CAC RID: 11436
		public int list;

		// Token: 0x04002CAD RID: 11437
		public int level;

		// Token: 0x04002CAE RID: 11438
		public int objId = -1;

		// Token: 0x04002CAF RID: 11439
		public int uninstallCost;

		// Token: 0x04002CB0 RID: 11440
		[NonSerialized]
		public SyncDiskPreset preset;
	}
}
