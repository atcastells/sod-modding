using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public class FirstPersonItemController : MonoBehaviour
{
	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000FB1 RID: 4017 RVA: 0x000DEC3D File Offset: 0x000DCE3D
	public static FirstPersonItemController Instance
	{
		get
		{
			return FirstPersonItemController._instance;
		}
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x000DEC44 File Offset: 0x000DCE44
	private void Awake()
	{
		if (FirstPersonItemController._instance != null && FirstPersonItemController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		FirstPersonItemController._instance = this;
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x000DEC72 File Offset: 0x000DCE72
	private void Start()
	{
		if (!SessionData.Instance.isFloorEdit)
		{
			InterfaceController.Instance.firstPersonModel.SetActive(false);
		}
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x000DEC90 File Offset: 0x000DCE90
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			if (this.currentItem != null)
			{
				foreach (FirstPersonItem.FPSInteractionAction fpsinteractionAction in this.currentItem.actions)
				{
					if (fpsinteractionAction.availability != FirstPersonItem.AttackAvailability.always || fpsinteractionAction.availability != FirstPersonItem.AttackAvailability.never)
					{
						if (this.updateInteractionCounter <= 0)
						{
							this.updateInteractionCounter = 3;
							InteractionController.Instance.UpdateInteractionText();
						}
						this.updateInteractionCounter--;
						break;
					}
				}
				if (this.isConsuming && !this.takeOneActive && BioScreenController.Instance.selectedSlot != null && BioScreenController.Instance.selectedSlot.interactableID > -1)
				{
					Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
					if (interactable != null && interactable.cs > 0f)
					{
						if (interactable.preset.retailItem != null)
						{
							Player.Instance.AddNourishment(interactable.preset.retailItem.nourishment / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddHydration(interactable.preset.retailItem.hydration / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddAlertness(interactable.preset.retailItem.alertness / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddEnergy(interactable.preset.retailItem.energy / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddExcitement(interactable.preset.retailItem.excitement / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddChores(interactable.preset.retailItem.chores / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddHygiene(interactable.preset.retailItem.hygiene / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddBladder(interactable.preset.retailItem.bladder / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddHeat(interactable.preset.retailItem.heat / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddDrunk(interactable.preset.retailItem.drunk / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddSick(interactable.preset.retailItem.sick / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddHeadache(interactable.preset.retailItem.headache / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddWet(interactable.preset.retailItem.wet / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddBrokenLeg(interactable.preset.retailItem.brokenLeg / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddBruised(interactable.preset.retailItem.bruised / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddBlackEye(interactable.preset.retailItem.blackEye / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddBlackedOut(interactable.preset.retailItem.blackedOut / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddNumb(interactable.preset.retailItem.numb / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddBleeding(interactable.preset.retailItem.bleeding / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddBreath(interactable.preset.retailItem.breath / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddStarchAddiction(interactable.preset.retailItem.starchAddiction / interactable.preset.consumableAmount * Time.deltaTime);
							Player.Instance.AddPoisoned(interactable.preset.retailItem.poisoned / interactable.preset.consumableAmount * Time.deltaTime, null);
							Player.Instance.AddHealth(interactable.preset.retailItem.health / interactable.preset.consumableAmount * Time.deltaTime, true, false);
						}
						interactable.cs = Mathf.Max(interactable.cs - Time.deltaTime, 0f);
						if (interactable.cs > 0f)
						{
							goto IL_686;
						}
						this.SetConsuming(false);
						if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.starchLoan) != 0f && interactable.preset.retailItem != null)
						{
							if (interactable.preset.retailItem.tags.Contains(RetailItemPreset.Tags.starchProduct))
							{
								using (List<UpgradesController.Upgrades>.Enumerator enumerator2 = UpgradesController.Instance.upgrades.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										UpgradesController.Upgrades upgrades = enumerator2.Current;
										upgrades.uninstallCost -= 5;
										upgrades.uninstallCost = Mathf.Max(0, upgrades.uninstallCost);
									}
									goto IL_608;
								}
							}
							foreach (UpgradesController.Upgrades upgrades2 in UpgradesController.Instance.upgrades)
							{
								upgrades2.uninstallCost++;
							}
							IL_608:
							UpgradesController.Instance.UpdateUpgrades();
						}
						this.OnConsumableFinished(interactable);
						if (!interactable.preset.destroyWhenAllConsumed)
						{
							goto IL_686;
						}
						this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
						using (IEnumerator enumerator3 = this.rightHandObjectParent.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								object obj = enumerator3.Current;
								Object.Destroy(((Transform)obj).gameObject);
							}
							goto IL_686;
						}
					}
					this.SetConsuming(false);
				}
			}
			IL_686:
			if (this.attackMainDelay > 0f)
			{
				this.attackMainDelay -= Time.deltaTime;
			}
			if (this.attackSecondaryDelay > 0f)
			{
				this.attackSecondaryDelay -= Time.deltaTime;
			}
			if ((this.listenForHolster || this.listenForDrawFinish) && InterfaceController.Instance.firstPersonAnimator != null)
			{
				AnimatorClipInfo[] currentAnimatorClipInfo = InterfaceController.Instance.firstPersonAnimator.GetCurrentAnimatorClipInfo(0);
				if (currentAnimatorClipInfo != null && currentAnimatorClipInfo.Length != 0)
				{
					AnimatorClipInfo animatorClipInfo = currentAnimatorClipInfo[0];
					if (this.listenForHolster && ((animatorClipInfo.clip != null && this.nothingClip != null && animatorClipInfo.clip.name == this.nothingClip.name) || (animatorClipInfo.clip != null && this.currentItem.idleClip != null && animatorClipInfo.clip.name == this.currentItem.idleClip.name)))
					{
						this.OnHolster();
					}
					else if (this.listenForDrawFinish && ((animatorClipInfo.clip != null && this.currentItem.idleClip != null && animatorClipInfo.clip.name == this.currentItem.idleClip.name) || this.currentItem == GameplayControls.Instance.nothingItem || this.currentItem == PlayerApartmentController.Instance.furnitureFPSItem))
					{
						this.FinishedDrawingNewItem();
					}
				}
				else if (this.listenForHolster)
				{
					Game.Log("Holster", 2);
					this.OnHolster();
				}
				else if (this.listenForDrawFinish && (this.currentItem == GameplayControls.Instance.nothingItem || this.currentItem == PlayerApartmentController.Instance.furnitureFPSItem))
				{
					Game.Log("Finished", 2);
					this.FinishedDrawingNewItem();
				}
			}
			if (this.equipSoundDelay > 0f)
			{
				this.equipSoundDelay -= Time.deltaTime;
				if (this.equipSoundDelay <= 0f)
				{
					AudioController.Instance.Play2DSound(this.currentItem.equipEvent, null, 1f);
					if (this.activeLoop != null)
					{
						AudioController.Instance.StopSound(this.activeLoop, AudioController.StopType.fade, "New weapon");
					}
					if (this.currentItem.activeLoop != null)
					{
						this.activeLoop = AudioController.Instance.Play2DLooping(this.currentItem.activeLoop, null, 1f);
					}
					this.equipSoundDelay = 0f;
				}
			}
			if (this.holsterSoundDelay > 0f)
			{
				this.holsterSoundDelay -= Time.deltaTime;
				if (this.holsterSoundDelay <= 0f)
				{
					AudioController.Instance.Play2DSound(this.currentItem.holsterEvent, null, 1f);
					this.holsterSoundDelay = 0f;
				}
			}
		}
		if (this.lagPivotTransform != null)
		{
			this.lagPivotTransform.rotation = Quaternion.Slerp(this.lagPivotTransform.rotation, CameraController.Instance.cam.transform.rotation, GameplayControls.Instance.fpsModelLag * Time.deltaTime);
		}
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x000DF6B0 File Offset: 0x000DD8B0
	private void OnConsumableFinished(Interactable consumableFinished)
	{
		if (consumableFinished == null)
		{
			return;
		}
		if (AchievementsController.Instance != null)
		{
			if (consumableFinished.preset.presetName == "TakeawayCoffee")
			{
				AchievementsController.Instance.AddToStat("Caffiend", "coffees_drunk", 1);
				return;
			}
			if (consumableFinished.preset.presetName == "StarchKola")
			{
				AchievementsController.Instance.AddToStat("Kola King", "kolas_drunk", 1);
				return;
			}
			if (consumableFinished.preset.presetName == "Donut")
			{
				AchievementsController.Instance.AddToStat("Donut Duke", "donuts_eaten", 1);
				return;
			}
			if (consumableFinished.preset.presetName == "CroqueMonsieur")
			{
				AchievementsController.Instance.AddToStat("Lord of the Croque", "croque_monsieurs_eaten", 1);
			}
		}
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x000DF784 File Offset: 0x000DD984
	private void LateUpdate()
	{
		if (this.lagPivotTransform != null)
		{
			this.lagPivotTransform.rotation = Quaternion.Slerp(this.lagPivotTransform.rotation, CameraController.Instance.cam.transform.rotation, GameplayControls.Instance.fpsModelLag * Time.deltaTime);
		}
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x000DF7E0 File Offset: 0x000DD9E0
	public void SetSlotSize(int newSize)
	{
		newSize = Mathf.Clamp(newSize, 1, 12);
		Game.Log("Player: Set new inventory slot size:" + newSize.ToString(), 2);
		this.AddSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot.holster);
		this.AddSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot.fists);
		this.AddSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot.watch);
		this.AddSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot.printReader);
		this.PlayerMoneyCheck();
		int i = this.slots.FindAll((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic).Count;
		while (i < newSize)
		{
			i++;
			this.slots.Add(new FirstPersonItemController.InventorySlot());
		}
		while (i > newSize)
		{
			FirstPersonItemController.InventorySlot inventorySlot = this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && item.GetInteractable() == null);
			if (inventorySlot == null)
			{
				inventorySlot = this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && item.GetInteractable() == null);
			}
			if (inventorySlot == null)
			{
				break;
			}
			Game.Log("Player: Emptying slot because it's more than the slot count of " + newSize.ToString(), 2);
			this.EmptySlot(inventorySlot, false, false, true, true);
			if (inventorySlot.spawnedSegment != null)
			{
				Object.Destroy(inventorySlot.spawnedSegment.gameObject);
			}
			this.slots.Remove(inventorySlot);
			i--;
		}
		for (int j = 0; j < this.slots.Count; j++)
		{
			FirstPersonItemController.InventorySlot inventorySlot2 = this.slots[j];
			inventorySlot2.index = j;
			if (inventorySlot2.spawnedSegment == null)
			{
				inventorySlot2.spawnedSegment = BioScreenController.Instance.SpawnSlotObject(inventorySlot2);
			}
			inventorySlot2.spawnedSegment.OnUpdateContent();
		}
		if (BioScreenController.Instance.selectedSlot == null)
		{
			BioScreenController.Instance.SelectSlot(this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, false);
		}
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x000DF9D0 File Offset: 0x000DDBD0
	public FirstPersonItemController.InventorySlot AddSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot staticItem)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return null;
		}
		FirstPersonItemController.InventorySlot inventorySlot = this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == staticItem);
		if (inventorySlot != null)
		{
			return inventorySlot;
		}
		FirstPersonItemController.InventorySlot inventorySlot2 = new FirstPersonItemController.InventorySlot();
		inventorySlot2.isStatic = staticItem;
		if (staticItem == FirstPersonItemController.InventorySlot.StaticSlot.holster)
		{
			if (!this.slots.Exists((FirstPersonItemController.InventorySlot item) => item.hotkey == "1"))
			{
				inventorySlot2.SetHotKey("1");
			}
		}
		else if (staticItem == FirstPersonItemController.InventorySlot.StaticSlot.fists)
		{
			if (!this.slots.Exists((FirstPersonItemController.InventorySlot item) => item.hotkey == "2"))
			{
				inventorySlot2.SetHotKey("2");
			}
		}
		else if (staticItem == FirstPersonItemController.InventorySlot.StaticSlot.watch)
		{
			if (!this.slots.Exists((FirstPersonItemController.InventorySlot item) => item.hotkey == "3"))
			{
				inventorySlot2.SetHotKey("3");
			}
		}
		else if (staticItem == FirstPersonItemController.InventorySlot.StaticSlot.printReader)
		{
			if (!this.slots.Exists((FirstPersonItemController.InventorySlot item) => item.hotkey == "4"))
			{
				inventorySlot2.SetHotKey("4");
			}
		}
		else if (staticItem == FirstPersonItemController.InventorySlot.StaticSlot.coin)
		{
			if (!this.slots.Exists((FirstPersonItemController.InventorySlot item) => item.hotkey == "5"))
			{
				inventorySlot2.SetHotKey("5");
			}
		}
		this.slots.Add(inventorySlot2);
		for (int i = 0; i < this.slots.Count; i++)
		{
			FirstPersonItemController.InventorySlot inventorySlot3 = this.slots[i];
			inventorySlot3.index = i;
			if (inventorySlot3.spawnedSegment == null)
			{
				if (staticItem == FirstPersonItemController.InventorySlot.StaticSlot.holster)
				{
					inventorySlot3.spawnedSegment = BioScreenController.Instance.nothingSquare;
					BioScreenController.Instance.nothingSquare.Setup(inventorySlot3);
				}
				else
				{
					inventorySlot3.spawnedSegment = BioScreenController.Instance.SpawnSlotObject(inventorySlot3);
				}
			}
			inventorySlot3.spawnedSegment.OnUpdateContent();
		}
		if (BioScreenController.Instance.selectedSlot == null)
		{
			BioScreenController.Instance.SelectSlot(FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, false);
		}
		return inventorySlot2;
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x000DFC60 File Offset: 0x000DDE60
	public void RemoveSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot staticItem)
	{
		FirstPersonItemController.InventorySlot inventorySlot = this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == staticItem);
		if (inventorySlot == null)
		{
			return;
		}
		this.EmptySlot(inventorySlot, false, false, true, true);
		if (inventorySlot.spawnedSegment != null)
		{
			Object.Destroy(inventorySlot.spawnedSegment.gameObject);
		}
		this.slots.Remove(inventorySlot);
		for (int i = 0; i < this.slots.Count; i++)
		{
			FirstPersonItemController.InventorySlot inventorySlot2 = this.slots[i];
			inventorySlot2.index = i;
			if (inventorySlot2.spawnedSegment == null)
			{
				inventorySlot2.spawnedSegment = BioScreenController.Instance.SpawnSlotObject(inventorySlot2);
			}
			inventorySlot2.spawnedSegment.OnUpdateContent();
		}
		if (BioScreenController.Instance.selectedSlot == null)
		{
			BioScreenController.Instance.SelectSlot(this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, false);
		}
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x000DFD64 File Offset: 0x000DDF64
	public void PlayerMoneyCheck()
	{
		if (GameplayController.Instance.money > 0)
		{
			if (!this.slots.Exists((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.coin))
			{
				this.AddSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot.coin);
				return;
			}
		}
		else if (GameplayController.Instance.money <= 0)
		{
			if (this.slots.Exists((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.coin))
			{
				this.RemoveSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot.coin);
			}
		}
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x000DFDF4 File Offset: 0x000DDFF4
	public bool PickUpItem(Interactable pickUpThis, bool switchToNew = false, bool allowSwap = false, bool enableFullMessage = true, bool enablePickupMessage = true, bool playSound = true)
	{
		if (pickUpThis == null)
		{
			return false;
		}
		bool result = false;
		Game.Log("Player: Attempting to pick up item: " + pickUpThis.GetName(), 2);
		if (pickUpThis.preset.name == "PrintScanner")
		{
			this.AddSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot.printReader);
			pickUpThis.Delete();
			return true;
		}
		if (pickUpThis.preset.name == "LockpickKit")
		{
			ActionController.Instance.TakeLockpickKit(pickUpThis, null, Player.Instance);
			return true;
		}
		FirstPersonItemController.InventorySlot inventorySlot = this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && item.GetInteractable() == null);
		if (inventorySlot == null && allowSwap && BioScreenController.Instance.selectedSlot != null && BioScreenController.Instance.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
		{
			inventorySlot = BioScreenController.Instance.selectedSlot;
			if (inventorySlot.interactableID > -1)
			{
				this.EmptySlot(inventorySlot, false, false, true, false);
			}
			switchToNew = true;
		}
		else if (inventorySlot == null && enableFullMessage)
		{
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "inventory full", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			if (Game.Instance.displayExtraControlHints && !CutSceneController.Instance.cutSceneActive)
			{
				ControlsDisplayController.Instance.DisplayControlIconAfterDelay(1.75f, InteractablePreset.InteractionKey.WeaponSelect, "Inventory", InterfaceControls.Instance.controlIconDisplayTime, false);
			}
		}
		if (inventorySlot != null)
		{
			if (pickUpThis.spawnNode != null && pickUpThis.spawnNode.gameLocation != null && pickUpThis.spawnNode.gameLocation.thisAsAddress != null)
			{
				if (InteractionController.Instance.GetValidPlayerActionIllegal(pickUpThis, pickUpThis.spawnNode, true, true) && pickUpThis.val > 1f)
				{
					StatusController.Instance.AddFineRecord(pickUpThis.spawnNode.gameLocation.thisAsAddress, pickUpThis, StatusController.CrimeType.theft, true, -1, false);
				}
				pickUpThis.spawnNode.gameLocation.thisAsAddress.RemoveVandalism(pickUpThis);
				StatusController.Instance.RemoveFineRecord(null, pickUpThis, StatusController.CrimeType.vandalism, false, false);
			}
			if (pickUpThis != null && pickUpThis.evidence != null)
			{
				pickUpThis.evidence.SetFound(true);
			}
			inventorySlot.SetSegmentContent(pickUpThis);
			if (enablePickupMessage)
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Added to inventory", Strings.Casing.asIs, false, false, false, null) + ": " + pickUpThis.name, InterfaceControls.Icon.hand, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
				if (Game.Instance.displayExtraControlHints && !CutSceneController.Instance.cutSceneActive)
				{
					ControlsDisplayController.Instance.DisplayControlIconAfterDelay(1.75f, InteractablePreset.InteractionKey.WeaponSelect, "Inventory", InterfaceControls.Instance.controlIconDisplayTime, false);
				}
			}
			result = true;
			if (playSound)
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.pickUpItem, null, 1f);
			}
			if (pickUpThis.syncDisk != null || pickUpThis.preset.specialCaseFlag == InteractablePreset.SpecialCase.syncDiskUpgrade)
			{
				UpgradesController.Instance.UpdateUpgrades();
			}
			if (switchToNew && !InteractionController.Instance.dialogMode)
			{
				BioScreenController.Instance.SelectSlot(inventorySlot, false, false);
			}
		}
		Game.Log("Player: ...Successful pick up: " + result.ToString(), 2);
		return result;
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x000E0120 File Offset: 0x000DE320
	public bool IsSlotAvailable()
	{
		return this.slots.Exists((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && item.GetInteractable() == null);
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x000E014C File Offset: 0x000DE34C
	public void EmptySlot(FirstPersonItemController.InventorySlot emptySlot, bool throwObject = false, bool destroyObject = false, bool removeStolenFine = true, bool playSound = true)
	{
		if (emptySlot == null)
		{
			return;
		}
		Interactable interactable = emptySlot.GetInteractable();
		emptySlot.interactableID = -1;
		if (emptySlot == BioScreenController.Instance.selectedSlot)
		{
			foreach (object obj in this.rightHandObjectParent)
			{
				Object.Destroy(((Transform)obj).gameObject);
			}
			foreach (object obj2 in this.leftHandObjectParent)
			{
				Object.Destroy(((Transform)obj2).gameObject);
			}
			BioScreenController.Instance.SelectSlot(this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), true, false);
		}
		if (interactable != null)
		{
			interactable.SetAsNotInventory(Player.Instance.currentNode);
			if (!interactable.wo)
			{
				interactable.ConvertToWorldObject(false);
			}
			bool relocateAuthority = Toolbox.Instance.GetRelocateAuthority(Player.Instance, interactable);
			if (throwObject)
			{
				interactable.MoveInteractable(this.rightHandObjectParent.transform.position, this.rightHandObjectParent.transform.eulerAngles, false);
				interactable.LoadInteractableToWorld(false, true);
				interactable.controller.DropThis(true);
			}
			else if (destroyObject)
			{
				interactable.MarkAsTrash(true, false, 0f);
				interactable.RemoveFromPlacement();
			}
			else
			{
				Game.Log("Player: Put down object...", 2);
				float num = Vector3.Distance(CameraController.Instance.cam.transform.position, interactable.spWPos);
				float reachDistance = interactable.GetReachDistance();
				if (num <= reachDistance && interactable.spR)
				{
					string text = "Player: Put back to original spawn position: ";
					Vector3 wPos = interactable.wPos;
					Game.Log(text + wPos.ToString(), 2);
					interactable.originalPosition = false;
					interactable.SetOriginalPosition(true, true);
					interactable.MoveInteractable(interactable.spWPos, new Vector3(0f, -Player.Instance.transform.eulerAngles.y, 0f), relocateAuthority);
					interactable.LoadInteractableToWorld(false, true);
					interactable.ForcePhysicsActive(true, false, default(Vector3), 2, false);
					interactable.SetSpawnPositionRelevent(true);
				}
				else
				{
					Vector3 point = InteractionController.Instance.playerCurrentRaycastHit.point;
					if (Vector3.Distance(point, CameraController.Instance.cam.transform.position) < 1.5f)
					{
						Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(point);
						NewNode newNode = null;
						if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
						{
							Game.Log("Player: Put down found node", 2);
							interactable.MoveInteractable(point, new Vector3(0f, -Player.Instance.transform.eulerAngles.y, 0f), relocateAuthority);
							interactable.LoadInteractableToWorld(false, true);
							interactable.ForcePhysicsActive(true, false, default(Vector3), 2, false);
							interactable.SetSpawnPositionRelevent(true);
						}
					}
					else
					{
						Game.Log("Player: Put down found node", 2);
						interactable.MoveInteractable(Player.Instance.currentNode.position, new Vector3(0f, -Player.Instance.transform.eulerAngles.y, 0f), relocateAuthority);
						interactable.LoadInteractableToWorld(false, true);
						interactable.ForcePhysicsActive(true, false, default(Vector3), 2, false);
						interactable.SetSpawnPositionRelevent(true);
					}
				}
			}
			emptySlot.SetSegmentContent(null);
			if (playSound)
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.dropItem, null, 1f);
			}
			if (removeStolenFine && interactable != null && interactable.node != null && interactable.spawnNode != null && interactable.spawnNode.gameLocation != null && !interactable.rPl)
			{
				if (interactable.spawnNode.gameLocation == interactable.node.gameLocation)
				{
					StatusController.Instance.RemoveFineRecord(null, interactable, StatusController.CrimeType.theft, false, false);
				}
				if (interactable.originalPosition)
				{
					StatusController.Instance.RemoveFineRecord(null, interactable, StatusController.CrimeType.vandalism, false, false);
					if (interactable.spawnNode.gameLocation.thisAsAddress != null)
					{
						interactable.spawnNode.gameLocation.thisAsAddress.RemoveVandalism(interactable);
					}
				}
				else if (interactable.inInventory == null && interactable.spawnNode.gameLocation.thisAsAddress != null && InteractionController.Instance.GetValidPlayerActionIllegal(interactable, Player.Instance.currentNode, true, false) && interactable.val > 1f)
				{
					interactable.spawnNode.gameLocation.thisAsAddress.AddVandalism(interactable);
					StatusController.Instance.AddFineRecord(interactable.spawnNode.gameLocation.thisAsAddress, interactable, StatusController.CrimeType.vandalism, true, Mathf.RoundToInt(interactable.val * (float)GameplayControls.Instance.vandalismFineMultiplier), false);
					InteractionController.Instance.SetIllegalActionActive(true);
				}
			}
			if (interactable.syncDisk != null || interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.syncDiskUpgrade)
			{
				UpgradesController.Instance.UpdateUpgrades();
			}
		}
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x000E0688 File Offset: 0x000DE888
	public void UpdateCurrentActions()
	{
		foreach (InteractablePreset.InteractionKey interactionKey in InteractionController.Instance.allInteractionKeys)
		{
			if (!this.currentActions.ContainsKey(interactionKey))
			{
				this.currentActions.Add(interactionKey, new Interactable.InteractableCurrentAction());
			}
			this.currentActions[interactionKey].enabled = false;
			this.currentActions[interactionKey].display = false;
		}
		if (this.drawnItem == null)
		{
			return;
		}
		if (!this.finishedDrawingItem)
		{
			return;
		}
		List<FirstPersonItem.FPSInteractionAction> actions = this.drawnItem.actions;
		actions.Sort((FirstPersonItem.FPSInteractionAction p1, FirstPersonItem.FPSInteractionAction p2) => p2.action.inputPriority.CompareTo(p1.action.inputPriority));
		for (int i = 0; i < actions.Count; i++)
		{
			FirstPersonItem.FPSInteractionAction fpsinteractionAction = actions[i];
			if (fpsinteractionAction.GetInteractionKey() == InteractablePreset.InteractionKey.none)
			{
				Game.Log("Interaction key equals none", 2);
			}
			else
			{
				Interactable.InteractableCurrentAction interactableCurrentAction = this.currentActions[fpsinteractionAction.GetInteractionKey()];
				if (interactableCurrentAction != null)
				{
					if (interactableCurrentAction.enabled)
					{
						if (fpsinteractionAction.action.debug)
						{
							Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Skipping as this is key is already enabled", 2);
						}
					}
					else
					{
						if (fpsinteractionAction.useStrikethrough)
						{
							interactableCurrentAction.display = true;
						}
						if (Player.Instance.disabledActions.Contains(fpsinteractionAction.interactionName.ToLower()))
						{
							if (fpsinteractionAction.action.debug)
							{
								Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Disabled actions", 2);
							}
							interactableCurrentAction.enabled = false;
							interactableCurrentAction.currentAction = fpsinteractionAction;
						}
						else if (Player.Instance.playerKOInProgress)
						{
							if (fpsinteractionAction.action.debug)
							{
								Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Player KO", 2);
							}
							interactableCurrentAction.enabled = false;
							interactableCurrentAction.currentAction = fpsinteractionAction;
						}
						else if (InteractionController.Instance.currentlyDragging != null)
						{
							if (fpsinteractionAction.action.debug)
							{
								Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Player dragging body", 2);
							}
							interactableCurrentAction.enabled = false;
							interactableCurrentAction.currentAction = fpsinteractionAction;
						}
						else if (!fpsinteractionAction.availableWhileLockedIn && InteractionController.Instance.lockedInInteraction != null)
						{
							if (fpsinteractionAction.action.debug)
							{
								Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Locked in", 2);
							}
							interactableCurrentAction.enabled = false;
							interactableCurrentAction.currentAction = fpsinteractionAction;
						}
						else if (!fpsinteractionAction.availableWhileJumping && !Player.Instance.fps.m_CharacterController.isGrounded)
						{
							if (fpsinteractionAction.action.debug)
							{
								Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Jumping", 2);
							}
							interactableCurrentAction.enabled = false;
							interactableCurrentAction.currentAction = fpsinteractionAction;
						}
						else if (fpsinteractionAction.actionCost > 0 && GameplayController.Instance.money < fpsinteractionAction.actionCost)
						{
							if (fpsinteractionAction.action.debug)
							{
								Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Cost too much", 2);
							}
							interactableCurrentAction.enabled = false;
							interactableCurrentAction.currentAction = fpsinteractionAction;
						}
						else if (!fpsinteractionAction.availableWhileIllegal && (Player.Instance.illegalStatus || Player.Instance.witnessesToIllegalActivity.Count > 0))
						{
							if (fpsinteractionAction.action.debug)
							{
								Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Time isn't in fast forward", 2);
							}
							interactableCurrentAction.enabled = false;
							interactableCurrentAction.currentAction = fpsinteractionAction;
						}
						else
						{
							if (fpsinteractionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.onlyAvailableInFastForward)
							{
								if (!Player.Instance.spendingTimeMode)
								{
									if (fpsinteractionAction.action.debug)
									{
										Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Time isn't in fast forward", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = fpsinteractionAction;
									goto IL_15F2;
								}
							}
							else if (fpsinteractionAction.specialCase != InteractablePreset.InteractionAction.SpecialCase.availableInFastForward && Player.Instance.spendingTimeMode)
							{
								if (fpsinteractionAction.action.debug)
								{
									Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Time is in fast forward", 2);
								}
								interactableCurrentAction.enabled = false;
								interactableCurrentAction.currentAction = fpsinteractionAction;
								goto IL_15F2;
							}
							if (fpsinteractionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.furniturePlacement)
							{
								if (!PlayerApartmentController.Instance.furniturePlacementMode || PlayerApartmentController.Instance.furnPlacement == null)
								{
									if (fpsinteractionAction.action.debug)
									{
										Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Furniture placement", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = fpsinteractionAction;
									goto IL_15F2;
								}
							}
							else if (fpsinteractionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.decorItemPlacement)
							{
								if (!(InteractionController.Instance.carryingObject != null))
								{
									if (fpsinteractionAction.action.debug)
									{
										Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Decor placement", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = fpsinteractionAction;
									goto IL_15F2;
								}
								if (!InteractionController.Instance.carryingObject.apartmentPlacementIsValid)
								{
									if (fpsinteractionAction.action.debug)
									{
										Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Decor placement", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = fpsinteractionAction;
									goto IL_15F2;
								}
							}
							if (fpsinteractionAction.action.unavailableWhenItemSelected && BioScreenController.Instance.selectedSlot != null && (BioScreenController.Instance.selectedSlot.interactableID > -1 || BioScreenController.Instance.selectedSlot.isStatic != FirstPersonItemController.InventorySlot.StaticSlot.holster))
							{
								if (fpsinteractionAction.action.unavailableWhenItemsSelected == null || fpsinteractionAction.action.unavailableWhenItemsSelected.Count <= 0)
								{
									if (fpsinteractionAction.action.debug)
									{
										Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Items drawn", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = fpsinteractionAction;
									goto IL_15F2;
								}
								if (fpsinteractionAction.action.unavailableWhenItemsSelected.Contains(BioScreenController.Instance.selectedSlot.GetFirstPersonItem()))
								{
									if (fpsinteractionAction.action.debug)
									{
										Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Items drawn", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = fpsinteractionAction;
									goto IL_15F2;
								}
							}
							if (fpsinteractionAction.action.onlyAvailableWhenItemSelected && BioScreenController.Instance.selectedSlot != null && !fpsinteractionAction.action.availableWhenItemsSelected.Contains(BioScreenController.Instance.selectedSlot.GetFirstPersonItem()))
							{
								if (fpsinteractionAction.action.debug)
								{
									Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Items drawn", 2);
								}
								interactableCurrentAction.enabled = false;
								interactableCurrentAction.currentAction = fpsinteractionAction;
							}
							else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.never)
							{
								interactableCurrentAction.enabled = false;
								interactableCurrentAction.currentAction = fpsinteractionAction;
							}
							else
							{
								if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.handcuffs)
								{
									if (!InteractionController.Instance.lookingAtInteractable || !(InteractionController.Instance.currentLookingAtInteractable.interactable.isActor != null) || InteractionController.Instance.playerCurrentRaycastHit.distance > InteractionController.Instance.currentLookingAtInteractable.interactable.GetReachDistance())
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if (InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.ai != null && (InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.ai.restrained || InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.isDead))
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if (!InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.isAsleep && (!(InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.ai != null) || !InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.ai.ko))
									{
										float num = 70f;
										Vector3 vector = Player.Instance.transform.position - InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.transform.position;
										Vector3 vector2 = -InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.transform.forward;
										float num2 = Vector3.Angle(vector, vector2);
										if (num2 < -num || num2 > num)
										{
											interactableCurrentAction.enabled = false;
											interactableCurrentAction.currentAction = fpsinteractionAction;
											goto IL_15F2;
										}
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.behindCitizen)
								{
									if (!InteractionController.Instance.lookingAtInteractable || !(InteractionController.Instance.currentLookingAtInteractable.interactable.isActor != null) || InteractionController.Instance.playerCurrentRaycastHit.distance > InteractionController.Instance.currentLookingAtInteractable.interactable.GetReachDistance())
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if (InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.ai != null && InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.ai.ko)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									float num3 = 70f;
									Vector3 vector3 = Player.Instance.transform.position - InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.transform.position;
									Vector3 vector4 = -InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.transform.forward;
									float num4 = Vector3.Angle(vector3, vector4);
									if (num4 < -num3 || num4 > num3)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.onConsuming)
								{
									if (!this.isConsuming)
									{
										if (fpsinteractionAction.action.debug)
										{
											Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Not consuming", 2);
										}
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.onNotConsuming)
								{
									if (this.isConsuming)
									{
										if (fpsinteractionAction.action.debug)
										{
											Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Consuming", 2);
										}
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.onNotConsumingButLeftovers)
								{
									if (this.isConsuming || BioScreenController.Instance.selectedSlot == null || BioScreenController.Instance.selectedSlot.interactableID <= -1 || BioScreenController.Instance.selectedSlot.GetInteractable().cs <= 0f)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.onRaisedButLeftovers)
								{
									if (!this.isRaised || BioScreenController.Instance.selectedSlot == null || BioScreenController.Instance.selectedSlot.interactableID <= -1 || BioScreenController.Instance.selectedSlot.GetInteractable().cs <= 0f)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.onRaisedNotFull)
								{
									if (!this.isRaised || BioScreenController.Instance.selectedSlot == null || BioScreenController.Instance.selectedSlot.interactableID <= -1 || BioScreenController.Instance.selectedSlot.GetInteractable().cs >= 5f)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.nearPutDown)
								{
									if (this.isConsuming)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
									interactableCurrentAction.overrideInteractionName = string.Empty;
									if (interactable != null && interactable.spR)
									{
										float num5 = Vector3.Distance(CameraController.Instance.cam.transform.position, interactable.spWPos);
										float reachDistance = interactable.GetReachDistance();
										if (num5 <= reachDistance && interactable.spR)
										{
											interactableCurrentAction.overrideInteractionName = "Put Back";
										}
										else
										{
											if (fpsinteractionAction.action.debug)
											{
												Game.Log(string.Concat(new string[]
												{
													"Debug: Action ",
													fpsinteractionAction.action.name,
													" Not available: Out of interaction distance for put back (",
													num5.ToString(),
													"/",
													reachDistance.ToString(),
													")"
												}), 2);
											}
											interactableCurrentAction.enabled = false;
											interactableCurrentAction.currentAction = fpsinteractionAction;
										}
									}
									if (!(InteractionController.Instance.playerCurrentRaycastHit.transform != null) || InteractionController.Instance.playerCurrentRaycastHit.distance > GameplayControls.Instance.interactionRange)
									{
										if (fpsinteractionAction.action.debug)
										{
											Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: No raycast hit", 2);
										}
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									bool flag = false;
									if (InteractionController.Instance.lookingAtInteractable)
									{
										if (InteractionController.Instance.currentLookingAtInteractable != null && InteractionController.Instance.currentLookingAtInteractable.interactable != null && !InteractionController.Instance.currentLookingAtInteractable.interactable.preset.spawnable && InteractionController.Instance.currentLookingAtInteractable.interactable.preset.prefab == null)
										{
											flag = true;
										}
									}
									else if (InteractionController.Instance.playerCurrentRaycastHit.transform.gameObject.CompareTag("Untagged") || InteractionController.Instance.playerCurrentRaycastHit.transform.gameObject.CompareTag("FloorMesh"))
									{
										flag = true;
									}
									if (!flag)
									{
										if (fpsinteractionAction.action.debug)
										{
											Game.Log(string.Concat(new string[]
											{
												"Debug: Action ",
												fpsinteractionAction.action.name,
												" Not available: Non-static raycast target (",
												InteractionController.Instance.playerCurrentRaycastHit.transform.name,
												")"
											}), 2);
										}
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if (!(InteractionController.Instance.playerCurrentRaycastHit.normal == new Vector3(0f, 1f, 0f)))
									{
										if (fpsinteractionAction.action.debug)
										{
											Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Not available: Put down normal is not UP", 2);
										}
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if (fpsinteractionAction.action.debug)
									{
										Game.Log("Debug: Action " + fpsinteractionAction.action.name + " Found valid face up surface", 2);
									}
									interactableCurrentAction.enabled = true;
									interactableCurrentAction.currentAction = fpsinteractionAction;
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.onRaised)
								{
									if (!this.isRaised)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.onNotRaised)
								{
									if (this.isRaised)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.codebreaker)
								{
									if (Player.Instance.computerInteractable == null && (!InteractionController.Instance.lookingAtInteractable || InteractionController.Instance.playerCurrentRaycastHit.distance > InteractionController.Instance.currentLookingAtInteractable.interactable.GetReachDistance()))
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if (InteractionController.Instance.currentLookingAtInteractable != null && InteractionController.Instance.currentLookingAtInteractable.interactable.preset == InteriorControls.Instance.keypad)
									{
										if (!InteractionController.Instance.currentLookingAtInteractable.interactable.locked)
										{
											interactableCurrentAction.enabled = false;
											interactableCurrentAction.currentAction = fpsinteractionAction;
											goto IL_15F2;
										}
									}
									else
									{
										if (Player.Instance.computerInteractable == null || !(Player.Instance.computerInteractable.controller != null))
										{
											interactableCurrentAction.enabled = false;
											interactableCurrentAction.currentAction = fpsinteractionAction;
											goto IL_15F2;
										}
										bool flag2 = false;
										if (Player.Instance.computerInteractable.controller.computer.currentApp == InteriorControls.Instance.loginApp)
										{
											ComputerLogin componentInChildren = Player.Instance.computerInteractable.controller.computer.GetComponentInChildren<ComputerLogin>();
											if (componentInChildren != null && componentInChildren.loginSelection.selected != null)
											{
												flag2 = true;
											}
										}
										if (!flag2)
										{
											interactableCurrentAction.enabled = false;
											interactableCurrentAction.currentAction = fpsinteractionAction;
											goto IL_15F2;
										}
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.doorWedge)
								{
									if (!InteractionController.Instance.lookingAtInteractable || (!(InteractionController.Instance.currentLookingAtInteractable.interactable.preset == InteriorControls.Instance.door) && !(InteractionController.Instance.currentLookingAtInteractable.interactable.preset == InteriorControls.Instance.peekUnderDoor)) || InteractionController.Instance.playerCurrentRaycastHit.distance > InteractionController.Instance.currentLookingAtInteractable.interactable.GetReachDistance())
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if (InteractionController.Instance.currentLookingAtInteractable.interactable.sw0)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if ((!(InteractionController.Instance.currentLookingAtInteractable.interactable.preset == InteriorControls.Instance.door) || InteractionController.Instance.playerCurrentRaycastHit.point.y > InteractionController.Instance.currentLookingAtInteractable.transform.position.y + 1f) && !(InteractionController.Instance.currentLookingAtInteractable.interactable.preset == InteriorControls.Instance.peekUnderDoor))
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.giveItem)
								{
									if (this.isConsuming || !InteractionController.Instance.lookingAtInteractable || !(InteractionController.Instance.currentLookingAtInteractable.interactable.isActor != null) || InteractionController.Instance.playerCurrentRaycastHit.distance > InteractionController.Instance.currentLookingAtInteractable.interactable.GetReachDistance())
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								else if (fpsinteractionAction.availability == FirstPersonItem.AttackAvailability.tracker && (this.isConsuming || !InteractionController.Instance.lookingAtInteractable || !(InteractionController.Instance.currentLookingAtInteractable.interactable.isActor != null) || InteractionController.Instance.playerCurrentRaycastHit.distance > GameplayControls.Instance.interactionRange))
								{
									if (this.isConsuming || !(InteractionController.Instance.playerCurrentRaycastHit.transform != null) || InteractionController.Instance.playerCurrentRaycastHit.distance > GameplayControls.Instance.interactionRange)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									bool flag3 = false;
									if (InteractionController.Instance.lookingAtInteractable)
									{
										if (InteractionController.Instance.currentLookingAtInteractable != null && InteractionController.Instance.currentLookingAtInteractable.interactable != null && !InteractionController.Instance.currentLookingAtInteractable.interactable.preset.spawnable && InteractionController.Instance.currentLookingAtInteractable.interactable.preset.prefab == null)
										{
											flag3 = true;
										}
									}
									else if (InteractionController.Instance.playerCurrentRaycastHit.transform.gameObject.CompareTag("Untagged") || InteractionController.Instance.playerCurrentRaycastHit.transform.gameObject.CompareTag("WallsMesh"))
									{
										flag3 = true;
									}
									if (!flag3)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
									if (InteractionController.Instance.playerCurrentRaycastHit.normal.y != 0f)
									{
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = fpsinteractionAction;
										goto IL_15F2;
									}
								}
								interactableCurrentAction.display = true;
								interactableCurrentAction.enabled = true;
								interactableCurrentAction.currentAction = fpsinteractionAction;
								if (fpsinteractionAction.action.debug)
								{
									Game.Log("Debug: Action " + fpsinteractionAction.action.name + " is valid: " + interactableCurrentAction.enabled.ToString(), 2);
								}
							}
						}
					}
				}
			}
			IL_15F2:;
		}
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x000E1CA8 File Offset: 0x000DFEA8
	public void OnHolster()
	{
		Game.Log("Player: Holster...", 2);
		this.listenForHolster = false;
		this.RefreshHeldObjects();
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x000E1CC4 File Offset: 0x000DFEC4
	public void RefreshHeldObjects()
	{
		foreach (object obj in this.rightHandObjectParent)
		{
			Transform transform = (Transform)obj;
			if (transform != null)
			{
				Object.Destroy(transform.gameObject);
			}
		}
		foreach (object obj2 in this.leftHandObjectParent)
		{
			Transform transform2 = (Transform)obj2;
			if (transform2 != null)
			{
				Object.Destroy(transform2.gameObject);
			}
		}
		if (this.activeLoop != null)
		{
			AudioController.Instance.StopSound(this.activeLoop, AudioController.StopType.fade, "New weapon");
		}
		if (this.currentItem != null)
		{
			if (this.currentItem.leftHandObject != null)
			{
				GameObject gameObject = this.currentItem.leftHandObject;
				if (this.currentItem.useAlternateTrashObjects && this.currentItem.leftHandObjectTrash != null && BioScreenController.Instance.selectedSlot != null)
				{
					Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
					if (interactable != null && interactable.cs <= 0f)
					{
						gameObject = this.currentItem.leftHandObjectTrash;
					}
				}
				if (gameObject != null)
				{
					GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, this.leftHandObjectParent);
					gameObject2.layer = 5;
					gameObject2.transform.localScale = this.currentItem.spawnScale;
					MeshRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<MeshRenderer>();
					int renderingLayerMask = -1;
					foreach (MeshRenderer meshRenderer in componentsInChildren)
					{
						meshRenderer.renderingLayerMask = (uint)renderingLayerMask;
						meshRenderer.gameObject.layer = 5;
						meshRenderer.shadowCastingMode = 0;
					}
				}
			}
			if (this.currentItem.rightHandObject != null || this.currentItem.useFoodSlotItem)
			{
				GameObject gameObject3 = null;
				GameObject gameObject4 = this.currentItem.rightHandObject;
				if (this.currentItem.useAlternateTrashObjects && this.currentItem.rightHandObjectTrash != null && BioScreenController.Instance.selectedSlot != null)
				{
					Interactable interactable2 = BioScreenController.Instance.selectedSlot.GetInteractable();
					if (interactable2 != null && interactable2.cs <= 0f)
					{
						gameObject4 = this.currentItem.rightHandObjectTrash;
					}
				}
				if (this.currentItem.useFoodSlotItem && BioScreenController.Instance.selectedSlot != null)
				{
					Interactable interactable3 = BioScreenController.Instance.selectedSlot.GetInteractable();
					if (interactable3 != null)
					{
						gameObject3 = Object.Instantiate<GameObject>(interactable3.preset.prefab, this.rightHandObjectParent);
						gameObject3.transform.localPosition = interactable3.preset.fpsItemOffset;
						gameObject3.transform.localEulerAngles = interactable3.preset.fpsItemRotation;
						Collider[] componentsInChildren2 = gameObject3.GetComponentsInChildren<Collider>();
						for (int i = 0; i < componentsInChildren2.Length; i++)
						{
							componentsInChildren2[i].enabled = false;
						}
						InteractableController component = gameObject3.GetComponent<InteractableController>();
						if (component != null)
						{
							component.enabled = false;
						}
					}
					else
					{
						if (this.currentItem.rightHandObject != null)
						{
							Game.Log("Spawning new right hand object: " + this.currentItem.rightHandObject.name, 2);
						}
						if (gameObject4 != null)
						{
							gameObject3 = Object.Instantiate<GameObject>(gameObject4, this.rightHandObjectParent);
						}
					}
				}
				else
				{
					if (this.currentItem.rightHandObject != null)
					{
						Game.Log("Spawning new right hand object: " + this.currentItem.rightHandObject.name, 2);
					}
					if (gameObject4 != null)
					{
						gameObject3 = Object.Instantiate<GameObject>(gameObject4, this.rightHandObjectParent);
					}
				}
				gameObject3.layer = 5;
				gameObject3.transform.localScale = this.currentItem.spawnScale;
				MeshRenderer[] componentsInChildren3 = gameObject3.GetComponentsInChildren<MeshRenderer>();
				int renderingLayerMask2 = -1;
				foreach (MeshRenderer meshRenderer2 in componentsInChildren3)
				{
					meshRenderer2.renderingLayerMask = (uint)renderingLayerMask2;
					meshRenderer2.gameObject.layer = 5;
					meshRenderer2.shadowCastingMode = 0;
				}
			}
			InterfaceController.Instance.firstPersonModel.SetActive(this.currentItem.modelActive);
			if (this.currentItem.equipEvent != null)
			{
				if (this.currentItem.equipSoundDelay > 0f)
				{
					this.equipSoundDelay = this.currentItem.equipSoundDelay;
					return;
				}
				AudioController.Instance.Play2DSound(this.currentItem.equipEvent, null, 1f);
				if (this.currentItem.activeLoop != null)
				{
					this.activeLoop = AudioController.Instance.Play2DLooping(this.currentItem.activeLoop, null, 1f);
					return;
				}
			}
		}
		else
		{
			Game.Log("Current item is null", 2);
		}
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x000E21A0 File Offset: 0x000E03A0
	public void GenerateSkinColourMaterials()
	{
		this.fistMaterial = Object.Instantiate<Material>(GameplayControls.Instance.fistMaterial);
		this.fingerLowerMaterial = Object.Instantiate<Material>(GameplayControls.Instance.fingerLowerMaterial);
		this.fingerUpperMaterial = Object.Instantiate<Material>(GameplayControls.Instance.fingerUpperMaterial);
		this.fingerTipMaterial = Object.Instantiate<Material>(GameplayControls.Instance.fingerTipMaterial);
		this.thumbJointMaterial = Object.Instantiate<Material>(GameplayControls.Instance.thumbJointMaterial);
		this.fistMaterial.SetColor("_Color1", Game.Instance.playerSkinColour);
		this.fingerLowerMaterial.SetColor("_Color1", Game.Instance.playerSkinColour);
		this.fingerUpperMaterial.SetColor("_Color1", Game.Instance.playerSkinColour);
		this.fingerTipMaterial.SetColor("_Color1", Game.Instance.playerSkinColour);
		this.thumbJointMaterial.SetColor("_Color1", Game.Instance.playerSkinColour);
		this.fingerTipMaterial.SetColor("_Color2", Color.Lerp(Game.Instance.playerSkinColour, Color.white, 0.7f));
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x000E22C4 File Offset: 0x000E04C4
	public void SetFirstPersonItem(FirstPersonItem newItem, bool forceSwitch = true)
	{
		if (this.currentItem != newItem || forceSwitch)
		{
			if (this.currentItem != null)
			{
				Game.Log("Player: Set first person item: " + newItem.name, 2);
			}
			if (this.activeLoop != null)
			{
				AudioController.Instance.StopSound(this.activeLoop, AudioController.StopType.fade, "weapon switch");
			}
			if (SessionData.Instance.enableTutorialText)
			{
				if (newItem.name == "fingerptintScanner")
				{
					SessionData.Instance.TutorialTrigger("fingerprints", false);
				}
				else if (newItem.name == "handcuffs" && !SessionData.Instance.tutorialTextTriggered.Contains("arresting"))
				{
					PopupMessageController.Instance.TutorialMessage("arresting", PopupMessageController.AffectPauseState.automatic, false, null);
					SessionData.Instance.tutorialTextTriggered.Add("arresting");
				}
			}
			if (this.currentItem != null && this.currentItem.holsterEvent != null)
			{
				if (this.currentItem.holsterSoundDelay <= 0f)
				{
					AudioController.Instance.Play2DSound(this.currentItem.holsterEvent, null, 1f);
				}
				else
				{
					this.holsterSoundDelay = this.currentItem.holsterSoundDelay;
				}
			}
			this.equipSoundDelay = 0f;
			this.listenForHolster = true;
			Game.Log("Player: Listen for holster...", 2);
			if (!newItem.compatibleWithHidden && Player.Instance.isHiding)
			{
				return;
			}
			if (!newItem.compatibleWithLockedIn && InteractionController.Instance.lockedInInteraction != null)
			{
				return;
			}
			this.previousItem = this.currentItem;
			this.currentItem = newItem;
			InteractionController.Instance.UpdateInteractionText();
			if (this.currentItem.modelActive)
			{
				InterfaceController.Instance.firstPersonModel.SetActive(true);
			}
			if (InteractionController.Instance.currentLookingAtInteractable != null)
			{
				InteractionController.Instance.currentLookingAtInteractable.interactable.UpdateCurrentActions();
			}
			InteractionController.Instance.UpdateInteractionText();
			if (InterfaceController.Instance.firstPersonAnimator != null)
			{
				InterfaceController.Instance.firstPersonAnimator.SetBool("MainAttackActive", false);
			}
			this.ReadyNewItemDraw();
			if (this.currentItem != null && this.currentItem.triggerTutorial != null && this.currentItem.triggerTutorial.Length > 0)
			{
				SessionData.Instance.TutorialTrigger(this.currentItem.triggerTutorial, false);
			}
			Game.Log("Player: Set new first person item: " + this.currentItem.name, 2);
		}
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x000E2544 File Offset: 0x000E0744
	public void SetFirstPersonSkinColour()
	{
		foreach (MeshRenderer meshRenderer in InterfaceController.Instance.firstPersonModel.GetComponentsInChildren<MeshRenderer>())
		{
			if (meshRenderer.sharedMaterial.name == "Fist")
			{
				meshRenderer.sharedMaterial = this.fistMaterial;
			}
			else if (meshRenderer.sharedMaterial.name == "FingerLower")
			{
				meshRenderer.sharedMaterial = this.fingerLowerMaterial;
			}
			else if (meshRenderer.sharedMaterial.name == "FingerUpper")
			{
				meshRenderer.sharedMaterial = this.fingerUpperMaterial;
			}
			else if (meshRenderer.sharedMaterial.name == "FingerTip")
			{
				meshRenderer.sharedMaterial = this.fingerTipMaterial;
			}
			else if (meshRenderer.sharedMaterial.name == "ThumbUpper")
			{
				meshRenderer.sharedMaterial = this.thumbJointMaterial;
			}
		}
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x000E2634 File Offset: 0x000E0834
	public void ReadyNewItemDraw()
	{
		Game.Log("Player: Ready new item:" + this.currentItem.name + " " + this.currentItem.slotPriority.ToString(), 2);
		this.drawnItem = this.currentItem;
		this.finishedDrawingItem = false;
		this.listenForDrawFinish = true;
		InteractionController.Instance.UpdateInteractionText();
		if (InterfaceController.Instance.firstPersonAnimator != null)
		{
			InterfaceController.Instance.firstPersonAnimator.SetFloat("DrawSpeed", this.currentItem.drawSpeed);
			InterfaceController.Instance.firstPersonAnimator.SetFloat("HolsterSpeed", this.currentItem.holsterSpeed);
			foreach (TextMeshPro textMeshPro in InterfaceController.Instance.firstPersonModel.GetComponentsInChildren<TextMeshPro>())
			{
				if (textMeshPro.CompareTag("WatchTimeText"))
				{
					SessionData.Instance.newWatchTimeText = textMeshPro;
					SessionData.Instance.UpdateUIClock();
				}
				else if (textMeshPro.CompareTag("WatchDateText"))
				{
					SessionData.Instance.newWatchDateText = textMeshPro;
					SessionData.Instance.UpdateUIDay();
				}
			}
			InterfaceController.Instance.firstPersonAnimator.SetInteger("CurrentItem", this.currentItem.slotPriority);
		}
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x000E276F File Offset: 0x000E096F
	public void FinishedDrawingNewItem()
	{
		Game.Log("Player: Finished drawing item", 2);
		this.finishedDrawingItem = true;
		this.listenForDrawFinish = false;
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x000E2794 File Offset: 0x000E0994
	public void OnInteraction(InteractablePreset.InteractionKey input)
	{
		if (this.attackMainDelay <= 0f)
		{
			Game.Log("Player: FPS Input " + input.ToString(), 2);
			Interactable.InteractableCurrentAction interactableCurrentAction = null;
			if (this.currentActions.TryGetValue(input, ref interactableCurrentAction))
			{
				FirstPersonItem.FPSInteractionAction fpsinteractionAction = interactableCurrentAction.currentAction as FirstPersonItem.FPSInteractionAction;
				if (interactableCurrentAction.currentAction.soundEvent != null && interactableCurrentAction.currentAction.playOnTrigger)
				{
					AudioController.Instance.PlayWorldOneShot(interactableCurrentAction.currentAction.soundEvent, Player.Instance, Player.Instance.currentNode, Player.Instance.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
				}
				if (fpsinteractionAction.useCameraJolt)
				{
					Vector3 direction;
					direction..ctor(Toolbox.Instance.Rand(fpsinteractionAction.joltXRange.x, fpsinteractionAction.joltXRange.y, false), Toolbox.Instance.Rand(fpsinteractionAction.joltYRange.x, fpsinteractionAction.joltYRange.y, false), Toolbox.Instance.Rand(fpsinteractionAction.joltZRange.x, fpsinteractionAction.joltZRange.y, false));
					Player.Instance.fps.JoltCamera(direction, fpsinteractionAction.joltAmplitude, fpsinteractionAction.joltSpeed);
				}
				if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.attack)
				{
					InterfaceController.Instance.firstPersonAnimator.SetFloat("AttackSpeed", fpsinteractionAction.attackMainSpeed);
					InterfaceController.Instance.firstPersonAnimator.SetTrigger("Attack");
					if (fpsinteractionAction.attackTrasition != null)
					{
						Player.Instance.TransformPlayerController(fpsinteractionAction.attackTrasition, null, null, null, false, false, 0f, false, default(Vector3), 1f, true);
					}
					if (fpsinteractionAction.attackEvent != null)
					{
						AudioController.Instance.PlayWorldOneShot(fpsinteractionAction.attackEvent, Player.Instance, Player.Instance.currentNode, Player.Instance.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
					}
					this.attackMainDelay = fpsinteractionAction.attackDelay;
				}
				else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.block)
				{
					this.Block();
				}
				else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.handcuff)
				{
					this.Handcuff();
				}
				else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.takedown)
				{
					this.Takedown();
				}
				else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.punch)
				{
					InterfaceController.Instance.firstPersonAnimator.SetTrigger("Punch");
					if (fpsinteractionAction.attackEvent != null)
					{
						AudioController.Instance.PlayWorldOneShot(fpsinteractionAction.attackEvent, Player.Instance, Player.Instance.currentNode, Player.Instance.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
					}
					this.attackMainDelay = fpsinteractionAction.attackDelay;
				}
				else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.consumeTrue)
				{
					this.SetConsuming(true);
				}
				else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.consumeFalse)
				{
					this.SetConsuming(false);
				}
				else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.takeOne)
				{
					this.TakeOne();
				}
				else
				{
					if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.takeBriefcaseCash)
					{
						Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
						if (interactable == null || interactable.cs <= 0f || this.takeOneActive)
						{
							goto IL_666;
						}
						int addVal = Mathf.CeilToInt(Mathf.Min(interactable.cs, 1f) * 1000f);
						GameplayController.Instance.AddMoney(addVal, true, "Stolen from briefcase");
						this.TakeOne();
						using (Dictionary<int, SideJob>.Enumerator enumerator = SideJobController.Instance.allJobsDictionary.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<int, SideJob> keyValuePair = enumerator.Current;
								SideJob sideJob = keyValuePair.Value as SideJobStealBriefcase;
								if (sideJob != null && sideJob.activeJobItems.ContainsValue(BioScreenController.Instance.selectedSlot.GetInteractable()))
								{
									sideJob.TriggerFail("Briefcase has been opened");
									break;
								}
							}
							goto IL_666;
						}
					}
					if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.putBriefcaseCash)
					{
						Interactable interactable2 = BioScreenController.Instance.selectedSlot.GetInteractable();
						if (interactable2 != null && interactable2.cs < 5f && GameplayController.Instance.money >= 1000)
						{
							GameplayController.Instance.AddMoney(-1000, true, "Put in briefcase");
							interactable2.cs += 1f;
							this.RefreshHeldObjects();
						}
					}
					else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.putDown)
					{
						this.SetConsuming(false);
						this.PutDown();
					}
					else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.raiseTrue)
					{
						this.SetRaised(true);
					}
					else
					{
						if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.openBriefcaseBomb)
						{
							this.SetRaised(true);
							if (BioScreenController.Instance.selectedSlot == null || BioScreenController.Instance.selectedSlot.interactableID <= -1)
							{
								goto IL_666;
							}
							Toolbox.Instance.TriggerBriefcaseBomb(BioScreenController.Instance.selectedSlot.GetInteractable(), Player.Instance);
							using (Dictionary<int, SideJob>.Enumerator enumerator = SideJobController.Instance.allJobsDictionary.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									KeyValuePair<int, SideJob> keyValuePair2 = enumerator.Current;
									SideJob sideJob2 = keyValuePair2.Value as SideJobStealBriefcase;
									if (sideJob2 != null && sideJob2.activeJobItems.ContainsValue(BioScreenController.Instance.selectedSlot.GetInteractable()))
									{
										sideJob2.TriggerFail("Briefcase has been opened");
										break;
									}
								}
								goto IL_666;
							}
						}
						if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.raiseFalse)
						{
							this.SetRaised(false);
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.takePicture)
						{
							this.TakePicture();
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.placeCodebreaker)
						{
							this.PlaceCodebreaker();
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.placeDoorWedge)
						{
							this.PlaceDoorWedge();
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.placeTracker)
						{
							this.PlaceTracker();
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.placeFlashbomb)
						{
							this.PlaceGrenade(InteriorControls.Instance.activeFlashbomb);
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.placeIncapacitator)
						{
							this.PlaceGrenade(InteriorControls.Instance.activeIncapacitator);
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.placeFurniture)
						{
							this.PlaceFurniture();
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.cancelFurniture)
						{
							this.CancelFurniture();
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.give)
						{
							this.Give();
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.rotateFurnLeft)
						{
							PlayerApartmentController.Instance.RotateFurn(false);
						}
						else if (fpsinteractionAction.mainSpecialAction == FirstPersonItem.SpecialAction.rotateFurnRight)
						{
							PlayerApartmentController.Instance.RotateFurn(true);
						}
					}
				}
				IL_666:
				if (fpsinteractionAction.isHidingPlace && !Player.Instance.isHiding && (!fpsinteractionAction.onlyHidingPlaceIfPublic || !Player.Instance.isTrespassing))
				{
					Player.Instance.SetHiding(true, null);
					return;
				}
				if (!fpsinteractionAction.isHidingPlace && Player.Instance.isHiding && Player.Instance.hidingInteractable == null)
				{
					Player.Instance.SetHiding(false, null);
				}
			}
		}
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x000E2E84 File Offset: 0x000E1084
	public void ForceHolster()
	{
		if (!this.forceHolstered)
		{
			this.forceHolstered = true;
			this.selectedWhenForceHolstered = BioScreenController.Instance.selectedSlot;
		}
		BioScreenController.Instance.SelectSlot(this.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, false);
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x000E2EE6 File Offset: 0x000E10E6
	public void RestoreItemSelection()
	{
		if (this.forceHolstered)
		{
			this.forceHolstered = false;
			BioScreenController.Instance.SelectSlot(this.selectedWhenForceHolstered, false, false);
			this.selectedWhenForceHolstered = null;
		}
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x000E2F10 File Offset: 0x000E1110
	public void SetEnableFirstPersonItemSelection(bool val)
	{
		this.enableItemSelection = val;
		if (!this.enableItemSelection)
		{
			BioScreenController.Instance.SetInventoryOpen(false, true, true);
		}
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x000E2F30 File Offset: 0x000E1130
	public void SetFlashlight(bool val)
	{
		if (val != this.flashlight)
		{
			if (this.flashlight)
			{
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.flashlightOn, Player.Instance, Player.Instance.currentNode, Player.Instance.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
			}
			else
			{
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.flashlightOff, Player.Instance, Player.Instance.currentNode, Player.Instance.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
			}
			Player.Instance.UpdateOverallVisibility();
		}
		this.flashlight = val;
		this.flashLightObject.SetActive(this.flashlight);
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x000E2FF1 File Offset: 0x000E11F1
	public void ToggleFlashlight()
	{
		this.SetFlashlight(!this.flashlight);
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x000E3004 File Offset: 0x000E1204
	public void MeleeAttack()
	{
		float amount = Player.Instance.combatHeft;
		if (BioScreenController.Instance.selectedSlot != null)
		{
			if (BioScreenController.Instance.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.fists)
			{
				amount = InteriorControls.Instance.fistsWeapon.GetAttackValue(MurderWeaponPreset.AttackValue.damage, Player.Instance);
			}
			else if (BioScreenController.Instance.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
			{
				Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
				if (interactable != null && interactable.preset.weapon != null)
				{
					amount = interactable.preset.weapon.GetAttackValue(MurderWeaponPreset.AttackValue.damage, Player.Instance);
				}
			}
		}
		Game.Log(string.Concat(new string[]
		{
			"Player: Melee attack! Damage: ",
			amount.ToString(),
			" (combat heft: ",
			Player.Instance.combatHeft.ToString(),
			")"
		}), 2);
		SessionData.Instance.TutorialTrigger("combat", false);
		Ray ray = new Ray(CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.forward);
		int num = 1048576;
		num = ~num;
		RaycastHit raycastHit;
		if (Physics.SphereCast(ray, 0.2f, ref raycastHit, 2f, num))
		{
			Actor componentInParent = raycastHit.transform.gameObject.GetComponentInParent<Actor>();
			if (componentInParent != null)
			{
				if (!(componentInParent.ai != null))
				{
					goto IL_520;
				}
				bool attackActive = componentInParent.ai.attackActive;
				Player.Instance.transitionRecoilState = true;
				if (componentInParent.currentGameLocation != null && componentInParent.currentGameLocation.thisAsAddress != null)
				{
					StatusController.Instance.AddFineRecord(componentInParent.currentGameLocation.thisAsAddress, componentInParent.interactable, StatusController.CrimeType.assault, true, -1, false);
				}
				else
				{
					StatusController.Instance.AddFineRecord(null, componentInParent.interactable, StatusController.CrimeType.assault, true, -1, false);
				}
				componentInParent.RecieveDamage(amount, Player.Instance, raycastHit.point, raycastHit.point - CameraController.Instance.cam.transform.position, CriminalControls.Instance.punchSpatter, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
				Human human = componentInParent as Human;
				try
				{
					if (human != null && human.outfitController != null)
					{
						if (raycastHit.transform.parent.gameObject == human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.Head).gameObject || raycastHit.transform.parent.gameObject == human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandLeft).gameObject || raycastHit.transform.parent.gameObject == human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandRight).gameObject)
						{
							AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.punchHitFlesh, Player.Instance, componentInParent.currentNode, raycastHit.point, null, null, 1f, null, false, null, false);
						}
						else
						{
							AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.punchHitFabric, Player.Instance, componentInParent.currentNode, raycastHit.point, null, null, 1f, null, false, null, false);
						}
					}
					goto IL_520;
				}
				catch
				{
					goto IL_520;
				}
			}
			BreakableWindowController component = raycastHit.transform.gameObject.GetComponent<BreakableWindowController>();
			if (component != null)
			{
				Vector3 position = base.transform.position;
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.punchHitGlass, null, null, raycastHit.point, null, null, 1f, null, false, null, false);
				component.BreakWindow(position, -base.transform.up, Player.Instance, false);
				if (BioScreenController.Instance.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.fists)
				{
					Player.Instance.RecieveDamage(0.01f, null, Vector3.zero, Vector3.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
					Player.Instance.AddBleeding(0.5f);
				}
			}
			else
			{
				foreach (InteractableController interactableController in raycastHit.transform.gameObject.GetComponentsInChildren<InteractableController>())
				{
					if (interactableController != null && interactableController.interactable != null && interactableController.interactable.preset != null && interactableController.interactable.preset.physicsProfile != null && interactableController.interactable.preset.reactWithExternalStimuli)
					{
						interactableController.SetPhysics(true, Player.Instance);
						if (interactableController.rb != null)
						{
							Vector3 vector = -base.transform.up * GameplayControls.Instance.throwForce;
							interactableController.rb.AddForce(vector, 2);
						}
					}
				}
			}
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.punchHitWall, Player.Instance, Player.Instance.currentNode, raycastHit.point, null, null, 1f, null, false, null, false);
			IL_520:
			Vector3 direction;
			direction..ctor(-1f, -0.2f, 0f);
			Player.Instance.fps.JoltCamera(direction, 6f, 1.1f);
		}
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x000E3580 File Offset: 0x000E1780
	public void Block()
	{
		Game.Log("Player: Block!", 2);
		SessionData.Instance.TutorialTrigger("combat", false);
		Ray ray = new Ray(CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.forward);
		int num = 1048576;
		num = ~num;
		RaycastHit raycastHit;
		if (Physics.SphereCast(ray, 0.2f, ref raycastHit, 2f, num))
		{
			Actor componentInParent = raycastHit.transform.gameObject.GetComponentInParent<Actor>();
			if (componentInParent != null && componentInParent.ai != null && componentInParent.ai.attackActive)
			{
				float num2 = componentInParent.ai.attackProgress / componentInParent.ai.currentWeaponPreset.attackTriggerPoint;
				if (num2 >= 1f - GameplayControls.Instance.successfulBlockThreshold && num2 <= 1f)
				{
					bool perfect = false;
					if (num2 >= 1f - GameplayControls.Instance.perfectBlockThreshold)
					{
						perfect = true;
						this.counterAttackActor = componentInParent;
						this.counterAttackPoint = raycastHit.point;
						InterfaceController.Instance.firstPersonAnimator.SetTrigger("Counter");
						Player.Instance.TransformPlayerController(GameplayControls.Instance.counterTransition, null, null, null, false, false, 0f, false, default(Vector3), 1f, true);
						componentInParent.ai.OnAttackBlock(perfect);
						return;
					}
					InterfaceController.Instance.firstPersonAnimator.SetTrigger("SuccessfulBlock");
					Player.Instance.TransformPlayerController(GameplayControls.Instance.successfulBlockTransition, null, null, null, false, false, 0f, false, default(Vector3), 1f, true);
					componentInParent.ai.OnAttackBlock(perfect);
					return;
				}
			}
		}
		InterfaceController.Instance.firstPersonAnimator.SetTrigger("UnsuccessfulBlock");
		Player.Instance.TransformPlayerController(GameplayControls.Instance.unsuccessfulBlockTransition, null, null, null, false, false, 0f, false, default(Vector3), 1f, true);
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x000E3784 File Offset: 0x000E1984
	public void CounterAttack()
	{
		Game.Log("Player: Counter attack!", 2);
		if (this.counterAttackActor != null)
		{
			Player.Instance.transitionRecoilState = true;
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.punchHitFlesh, this.counterAttackActor, this.counterAttackActor.currentNode, this.counterAttackPoint, null, null, 1f, null, false, null, false);
			this.counterAttackActor.RecieveDamage(InteriorControls.Instance.fistsWeapon.GetAttackValue(MurderWeaponPreset.AttackValue.damage, Player.Instance), Player.Instance, this.counterAttackPoint, this.counterAttackPoint - CameraController.Instance.cam.transform.position, CriminalControls.Instance.punchSpatter, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
		}
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x000E3858 File Offset: 0x000E1A58
	public void ThrowCoin()
	{
		Game.Log("Player: Throw coin", 2);
		GameplayController.Instance.AddMoney(-1, false, "Thrown coin");
		Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(this.worldCoin, Player.Instance, Player.Instance, null, this.rightHandObjectParent.transform.position, this.rightHandObjectParent.transform.eulerAngles, null, null, "");
		interactable.MarkAsTrash(true, false, 0f);
		interactable.MoveInteractable(this.rightHandObjectParent.transform.position, this.rightHandObjectParent.transform.eulerAngles, false);
		interactable.LoadInteractableToWorld(false, true);
		interactable.controller.DropThis(true);
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x000E390A File Offset: 0x000E1B0A
	public void Handcuff()
	{
		if (InteractionController.Instance.currentLookingAtInteractable != null)
		{
			Player.Instance.OnHandcuff(InteractionController.Instance.currentLookingAtInteractable.interactable);
		}
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x000E3938 File Offset: 0x000E1B38
	public void Takedown()
	{
		Game.Log("Player: Takedown!", 2);
		InterfaceController.Instance.firstPersonAnimator.SetTrigger("Takedown");
		if (InteractionController.Instance.currentLookingAtInteractable != null && InteractionController.Instance.currentLookingAtInteractable.interactable.isActor != null)
		{
			InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.RecieveDamage(0f, Player.Instance, Player.Instance.lookAtThisTransform.position, InteractionController.Instance.currentLookingAtInteractable.interactable.isActor.lookAtThisTransform.position - Player.Instance.lookAtThisTransform.position, null, null, SpatterSimulation.EraseMode.useDespawnTime, false, true, GameplayControls.Instance.takedownTimer, 1f, false, true, 1f);
		}
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x000E3A14 File Offset: 0x000E1C14
	public void SetConsuming(bool val)
	{
		this.isConsuming = val;
		InterfaceController.Instance.firstPersonAnimator.SetBool("Consume", this.isConsuming);
		if (this.isConsuming)
		{
			Interactable interactable = null;
			if (BioScreenController.Instance.selectedSlot != null && BioScreenController.Instance.selectedSlot.interactableID > -1)
			{
				interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
			}
			if (this.consumeLoop != null)
			{
				AudioController.Instance.StopSound(this.consumeLoop, AudioController.StopType.fade, "New consume");
				this.consumeLoop = null;
			}
			if (interactable != null && interactable.preset.playerConsumeLoop != null)
			{
				this.consumeLoop = AudioController.Instance.Play2DLooping(interactable.preset.playerConsumeLoop, null, 1f);
			}
		}
		else if (this.consumeLoop != null)
		{
			AudioController.Instance.StopSound(this.consumeLoop, AudioController.StopType.fade, "New consume");
			this.consumeLoop = null;
		}
		FirstPersonItemController.Instance.UpdateCurrentActions();
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x000E3B0C File Offset: 0x000E1D0C
	public void TakeOne()
	{
		if (BioScreenController.Instance.selectedSlot != null && BioScreenController.Instance.selectedSlot.interactableID > -1)
		{
			Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
			if (interactable.preset.takeOneEvent != null)
			{
				AudioController.Instance.Play2DSound(interactable.preset.takeOneEvent, null, 1f);
			}
		}
		if (!this.takeOneActive)
		{
			base.StartCoroutine(this.TakeOneExecute());
		}
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x000E3B8D File Offset: 0x000E1D8D
	private IEnumerator TakeOneExecute()
	{
		float progress = 0f;
		this.takeOneActive = true;
		this.SetConsuming(true);
		Interactable consumable = null;
		if (BioScreenController.Instance.selectedSlot != null && BioScreenController.Instance.selectedSlot.interactableID > -1)
		{
			consumable = BioScreenController.Instance.selectedSlot.GetInteractable();
		}
		while (progress < 1f && consumable != null && consumable.cs > 0f)
		{
			float num = Time.deltaTime / 1.8f;
			if (consumable.preset.retailItem != null)
			{
				Player.Instance.AddNourishment(consumable.preset.retailItem.nourishment * num);
				Player.Instance.AddHydration(consumable.preset.retailItem.hydration * num);
				Player.Instance.AddAlertness(consumable.preset.retailItem.alertness * num);
				Player.Instance.AddEnergy(consumable.preset.retailItem.energy * num);
				Player.Instance.AddExcitement(consumable.preset.retailItem.excitement * num);
				Player.Instance.AddChores(consumable.preset.retailItem.chores * num);
				Player.Instance.AddHygiene(consumable.preset.retailItem.hygiene * num);
				Player.Instance.AddBladder(consumable.preset.retailItem.bladder * num);
				Player.Instance.AddHeat(consumable.preset.retailItem.heat * num);
				Player.Instance.AddDrunk(consumable.preset.retailItem.drunk * num);
				Player.Instance.AddSick(consumable.preset.retailItem.sick * num);
				Player.Instance.AddHeadache(consumable.preset.retailItem.headache * num);
				Player.Instance.AddWet(consumable.preset.retailItem.wet * num);
				Player.Instance.AddBrokenLeg(consumable.preset.retailItem.brokenLeg * num);
				Player.Instance.AddBruised(consumable.preset.retailItem.bruised * num);
				Player.Instance.AddBlackEye(consumable.preset.retailItem.blackEye * num);
				Player.Instance.AddBlackedOut(consumable.preset.retailItem.blackedOut * num);
				Player.Instance.AddNumb(consumable.preset.retailItem.numb * num);
				Player.Instance.AddBleeding(consumable.preset.retailItem.bleeding * num);
				Player.Instance.AddBreath(consumable.preset.retailItem.breath * num);
				Player.Instance.AddStarchAddiction(consumable.preset.retailItem.starchAddiction * num);
				Player.Instance.AddPoisoned(consumable.preset.retailItem.poisoned * num, null);
				Player.Instance.AddHealth(consumable.preset.retailItem.health * num, true, false);
			}
			progress += num;
			yield return null;
		}
		if (consumable != null)
		{
			consumable.cs -= 1f;
			if (consumable.cs <= 0f)
			{
				this.OnConsumableFinished(consumable);
				if (consumable.preset.destroyWhenAllConsumed)
				{
					this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
					foreach (object obj in this.rightHandObjectParent)
					{
						Object.Destroy(((Transform)obj).gameObject);
					}
				}
			}
		}
		this.takeOneActive = false;
		this.SetConsuming(false);
		if (consumable.cs <= 0f)
		{
			this.RefreshHeldObjects();
		}
		yield break;
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x000E3B9C File Offset: 0x000E1D9C
	public void SetRaised(bool val)
	{
		this.isRaised = val;
		InterfaceController.Instance.firstPersonAnimator.SetBool("Raised", this.isRaised);
		FirstPersonItemController.Instance.UpdateCurrentActions();
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000E3BC9 File Offset: 0x000E1DC9
	public void PutDown()
	{
		Game.Log("Player: Put down", 2);
		this.EmptySlot(BioScreenController.Instance.selectedSlot, false, false, true, true);
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x000E3BEA File Offset: 0x000E1DEA
	public void ThrowFood()
	{
		Game.Log("Player: Throw food", 2);
		this.EmptySlot(BioScreenController.Instance.selectedSlot, true, false, true, false);
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x000E3C0C File Offset: 0x000E1E0C
	public void ThrowGrenade()
	{
		Game.Log("Player: Throw grenade", 2);
		InteractablePreset preset = InteriorControls.Instance.activeFlashbomb;
		if (this.currentItem == InteriorControls.Instance.incapacitator.fpsItem)
		{
			preset = InteriorControls.Instance.activeIncapacitator;
		}
		Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(preset, Player.Instance, null, null, this.rightHandObjectParent.transform.position, this.rightHandObjectParent.transform.eulerAngles, null, null, "");
		if (interactable != null)
		{
			interactable.ForcePhysicsActive(true, true, default(Vector3), 2, true);
			interactable.SetCustomState2(true, Player.Instance, true, true, false);
			interactable.SetValue(GameplayControls.Instance.thrownGrenadeFuse);
			this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
			InteractionController.Instance.SetIllegalActionActive(true);
		}
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x000E3CE4 File Offset: 0x000E1EE4
	public void TakePicture()
	{
		Game.Log("Player: Take photo...", 2);
		Player.Instance.interactable.node = Player.Instance.currentNode;
		Player.Instance.interactable.cvp = CameraController.Instance.cam.transform.TransformPoint(new Vector3(0f, -0.1f, 0.1f));
		Player.Instance.interactable.cve = CameraController.Instance.cam.transform.eulerAngles;
		SceneRecorder.SceneCapture sceneCapture = Player.Instance.sceneRecorder.ExecuteCapture(false, true, true);
		string text = "Player: New capture: ";
		SceneRecorder.SceneCapture sceneCapture2 = sceneCapture;
		Game.Log(text + ((sceneCapture2 != null) ? sceneCapture2.ToString() : null), 2);
		Player.Instance.sceneRecorder.interactable.sCap.Add(sceneCapture);
		Interactable.Passed passed = new Interactable.Passed(Interactable.PassedVarType.savedSceneCapID, (float)sceneCapture.id, null);
		List<Interactable.Passed> list = new List<Interactable.Passed>();
		list.Add(passed);
		Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.surveillancePrintout, Player.Instance, Player.Instance, null, Player.Instance.transform.position, Vector3.zero, list, null, "");
		if (interactable != null)
		{
			interactable.RemoveFromPlacement();
			interactable.MarkAsTrash(true, false, 0f);
		}
		ActionController.Instance.Inspect(interactable, Player.Instance.currentNode, Player.Instance);
		foreach (SceneRecorder.ActorCapture actorCapture in sceneCapture.aCap)
		{
			Human human = actorCapture.GetHuman();
			if (!human.isDead && !human.isStunned && !human.isAsleep && human.ai != null)
			{
				if (human.ai.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor.isPlayer && item.active))
				{
					Game.Log(human.GetCitizenName() + " sees player taking photo and isn't happy about it!", 2);
					human.ai.SetPersue(Player.Instance, false, 1, true, 0f);
				}
			}
		}
		if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.kitchenPhotos) > 0f && Player.Instance.currentRoom != null && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.company != null && !Player.Instance.foodHygeinePhotos.Contains(Player.Instance.currentRoom.roomID) && (Player.Instance.currentRoom.preset.roomClass.name == "Kitchen" || Player.Instance.currentRoom.preset.roomClass.name == "Diner" || Player.Instance.currentRoom.preset.roomClass.name == "FastFood"))
		{
			int num = Mathf.RoundToInt((1f - Player.Instance.currentRoom.defaultWallKey.grubiness) * 10f);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.interface", "Hygeine Rating", Strings.Casing.asIs, false, false, false, null) + ": " + num.ToString() + "/10", InterfaceControls.Icon.trash, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			int num2 = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.kitchenPhotos));
			GameplayController.Instance.AddMoney(Mathf.CeilToInt((float)num2 * ((float)(10 - num) / 10f)), true, "Hygeine photo");
			Player.Instance.foodHygeinePhotos.Add(Player.Instance.currentRoom.roomID);
		}
		if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.bathroomPhotos) > 0f && Player.Instance.currentRoom != null && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.company != null && !Player.Instance.sanitaryHygeinePhotos.Contains(Player.Instance.currentRoom.roomID) && (Player.Instance.currentRoom.preset.roomClass.name == "Bathroom" || Player.Instance.currentRoom.preset.roomClass.name == "PublicBathroomMale" || Player.Instance.currentRoom.preset.roomClass.name == "PublicBathroomFemale"))
		{
			int num3 = Mathf.RoundToInt((1f - Player.Instance.currentRoom.defaultWallKey.grubiness) * 10f);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.interface", "Hygeine Rating", Strings.Casing.asIs, false, false, false, null) + ": " + num3.ToString() + "/10", InterfaceControls.Icon.trash, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			int num4 = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.bathroomPhotos));
			GameplayController.Instance.AddMoney(Mathf.CeilToInt((float)num4 * ((float)(10 - num3) / 10f)), true, "Hygeine photo");
			Player.Instance.sanitaryHygeinePhotos.Add(Player.Instance.currentRoom.roomID);
		}
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x000E429C File Offset: 0x000E249C
	public void PlaceCodebreaker()
	{
		List<Interactable.Passed> list = new List<Interactable.Passed>();
		list.Add(new Interactable.Passed(Interactable.PassedVarType.time, SessionData.Instance.gameTime, null));
		Interactable interactable;
		if (Player.Instance.computerInteractable != null)
		{
			interactable = InteractableCreator.Instance.CreateTransformInteractable(InteriorControls.Instance.activeCodebreaker, Player.Instance.computerInteractable.controller.transform, Player.Instance, null, new Vector3(-0.078f, 0.075f, 0.145f), new Vector3(0f, 90f, 0f), list);
			interactable.objectRef = Player.Instance.computerInteractable;
		}
		else
		{
			interactable = InteractableCreator.Instance.CreateTransformInteractable(InteriorControls.Instance.activeCodebreaker, InteractionController.Instance.currentLookingAtInteractable.transform, Player.Instance, null, Vector3.zero, new Vector3(90f, 0f, 0f), list);
			interactable.objectRef = InteractionController.Instance.currentLookingAtInteractable.interactable;
		}
		interactable.cs = -0.5f;
		this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x000E43B8 File Offset: 0x000E25B8
	public void PlaceDoorWedge()
	{
		NewDoor newDoor = InteractionController.Instance.currentLookingAtInteractable.interactable.objectRef as NewDoor;
		if (newDoor != null)
		{
			List<Interactable.Passed> list = new List<Interactable.Passed>();
			list.Add(new Interactable.Passed(Interactable.PassedVarType.time, SessionData.Instance.gameTime, null));
			InteractableCreator.Instance.CreateDoorParentedInteractable(InteriorControls.Instance.activeDoorWedge, newDoor, Player.Instance, new Vector3(0f, 0f, -0.05f), Vector3.zero, list, "").SetValue((float)newDoor.wall.id);
			this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
			if (AchievementsController.Instance != null)
			{
				NewRoom room = newDoor.wall.node.room;
				if (Player.Instance.currentRoom == newDoor.wall.node.room)
				{
					room = newDoor.wall.otherWall.node.room;
				}
				if (room.currentOccupants.Count > 0)
				{
					bool flag = false;
					foreach (Actor actor in room.currentOccupants)
					{
						if (!actor.isDead && !actor.isPlayer)
						{
							flag = true;
							break;
						}
					}
					if (flag && room.preset.roomType.presetName.Contains("Bathroom"))
					{
						AchievementsController.Instance.UnlockAchievement("Take a Bath / Toilet Terror", "trap_citizen_in_bathroom");
					}
				}
			}
		}
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x000E4558 File Offset: 0x000E2758
	public void PlaceTracker()
	{
		if (InteractionController.Instance.lookingAtInteractable && InteractionController.Instance.currentLookingAtInteractable.interactable.isActor != null)
		{
			Human human = InteractionController.Instance.currentLookingAtInteractable.interactable.isActor as Human;
			if (human != null)
			{
				if (human.ai != null)
				{
					if (human.ai.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor.isPlayer && item.active))
					{
						Game.Log(human.GetCitizenName() + " sees player planting tracker and isn't happy about it!", 2);
						human.ai.SetPersue(Player.Instance, false, 1, true, 0f);
					}
				}
				Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.activeTracker, Player.Instance, null, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
				if (interactable != null)
				{
					interactable.SetInInventory(human);
					this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
					return;
				}
			}
		}
		else if (!InteractionController.Instance.lookingAtInteractable && InteractionController.Instance.playerCurrentRaycastHit.transform != null)
		{
			Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(InteractionController.Instance.playerCurrentRaycastHit.point);
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
			{
				Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.up, InteractionController.Instance.playerCurrentRaycastHit.normal).eulerAngles;
				if (eulerAngles.x == 270f)
				{
					eulerAngles.z = 180f;
				}
				Vector3 vector;
				vector..ctor(90f, 0f, eulerAngles.z);
				string text = "Placing motion tracker with euler ";
				Vector3 vector2 = vector;
				string text2 = vector2.ToString();
				string text3 = " from wall normals ";
				vector2 = eulerAngles;
				Game.Log(text + text2 + text3 + vector2.ToString(), 2);
				Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.activeTracker, Player.Instance, null, null, InteractionController.Instance.playerCurrentRaycastHit.point, vector, null, null, "");
				if (interactable != null)
				{
					interactable.LoadInteractableToWorld(false, true);
					interactable.SetCustomState1(true, null, true, true, false);
					this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
				}
			}
		}
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x000E47E8 File Offset: 0x000E29E8
	public void PlaceGrenade(InteractablePreset activeGrenade)
	{
		if (InteractionController.Instance.lookingAtInteractable && InteractionController.Instance.currentLookingAtInteractable.interactable.isActor != null)
		{
			Human human = InteractionController.Instance.currentLookingAtInteractable.interactable.isActor as Human;
			if (human != null)
			{
				if (human.ai != null)
				{
					if (human.ai.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor.isPlayer && item.active))
					{
						Game.Log(human.GetCitizenName() + " sees player planting grenade and isn't happy about it!", 2);
						human.ai.SetPersue(Player.Instance, false, 1, true, 0f);
					}
				}
				Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(activeGrenade, Player.Instance, null, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
				if (interactable != null)
				{
					interactable.SetInInventory(human);
					interactable.SetCustomState2(true, Player.Instance, true, true, false);
					interactable.SetValue(GameplayControls.Instance.thrownGrenadeFuse);
					this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
					return;
				}
			}
		}
		else if (!InteractionController.Instance.lookingAtInteractable && InteractionController.Instance.playerCurrentRaycastHit.transform != null)
		{
			Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(InteractionController.Instance.playerCurrentRaycastHit.point);
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
			{
				Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.up, InteractionController.Instance.playerCurrentRaycastHit.normal).eulerAngles;
				if (eulerAngles.x == 270f)
				{
					eulerAngles.z = 180f;
				}
				Vector3 vector;
				vector..ctor(90f, 0f, eulerAngles.z);
				string text = "Placing grenade with euler ";
				Vector3 vector2 = vector;
				string text2 = vector2.ToString();
				string text3 = " from wall normals ";
				vector2 = eulerAngles;
				Game.Log(text + text2 + text3 + vector2.ToString(), 2);
				Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(activeGrenade, Player.Instance, null, null, InteractionController.Instance.playerCurrentRaycastHit.point, vector, null, null, "");
				if (interactable != null)
				{
					interactable.LoadInteractableToWorld(false, true);
					interactable.SetCustomState1(true, null, true, true, false);
					interactable.SetValue(GameplayControls.Instance.proxyGrenadeFuse);
					this.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, true, false);
				}
			}
		}
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x000E4A94 File Offset: 0x000E2C94
	public void PlaceFurniture()
	{
		if (PlayerApartmentController.Instance.furniturePlacementMode && PlayerApartmentController.Instance.furnPlacement != null && PlayerApartmentController.Instance.isPlacementValid)
		{
			Game.Log("Decor: Placement, furniture mode...", 2);
			if (PlayerApartmentController.Instance.GetExistingFurniture() != null)
			{
				PlayerApartmentController.Instance.ExecutePlacement();
				return;
			}
			if (PlayerApartmentController.Instance.furnPlacement.preset.cost <= GameplayController.Instance.money)
			{
				PopupMessageController.Instance.PopupMessage("FurniturePlacement", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				PopupMessageController.Instance.OnLeftButton += this.PlaceFurnitureCancel;
				PopupMessageController.Instance.OnRightButton += this.PlaceFurnitureConfirm;
				return;
			}
			if (PlayerApartmentController.Instance.furnPlacement.preset.cost > GameplayController.Instance.money)
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_money", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.money, null, null, null);
				return;
			}
		}
		else if (PlayerApartmentController.Instance.decoratingMode)
		{
			Game.Log("Decor: Placement, decor mode...", 2);
			if (PlayerApartmentController.Instance.GetCurrentCost() <= GameplayController.Instance.money)
			{
				PopupMessageController.Instance.PopupMessage("FurniturePlacement", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.no, false, "", false, false, false, false, "", "", false, "", false, "", "");
				PopupMessageController.Instance.OnLeftButton += this.PlaceFurnitureCancel;
				PopupMessageController.Instance.OnRightButton += this.PlaceFurnitureConfirm;
				return;
			}
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_money", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.money, null, null, null);
			return;
		}
		else
		{
			Game.Log("Decor: Placement is invalid!", 2);
		}
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x000E4CC0 File Offset: 0x000E2EC0
	public void PlaceFurnitureConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.PlaceFurnitureCancel;
		PopupMessageController.Instance.OnRightButton -= this.PlaceFurnitureConfirm;
		PlayerApartmentController.Instance.ExecutePlacement();
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x000E4CF8 File Offset: 0x000E2EF8
	public void PlaceFurnitureCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.PlaceFurnitureCancel;
		PopupMessageController.Instance.OnRightButton -= this.PlaceFurnitureConfirm;
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x000E4D26 File Offset: 0x000E2F26
	public void CancelFurniture()
	{
		PlayerApartmentController.Instance.CancelPlacement(PlayerApartmentController.Instance.placeExistingRoomObject);
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x000E4D3C File Offset: 0x000E2F3C
	public void Give()
	{
		if (InteractionController.Instance.lookingAtInteractable && InteractionController.Instance.currentLookingAtInteractable.interactable.isActor != null)
		{
			Human human = InteractionController.Instance.currentLookingAtInteractable.interactable.isActor as Human;
			if (human != null)
			{
				Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
				if (interactable != null)
				{
					human.TryGiveItem(interactable, Player.Instance, true, true);
				}
			}
		}
	}

	// Token: 0x0400133A RID: 4922
	[Header("Slots")]
	[Header("Radial Menu")]
	public List<FirstPersonItemController.InventorySlot> slots = new List<FirstPersonItemController.InventorySlot>();

	// Token: 0x0400133B RID: 4923
	public int inventorySlots;

	// Token: 0x0400133C RID: 4924
	[Header("First Person Items")]
	public bool enableItemSelection = true;

	// Token: 0x0400133D RID: 4925
	public Transform lagPivotTransform;

	// Token: 0x0400133E RID: 4926
	public FirstPersonItem previousItem;

	// Token: 0x0400133F RID: 4927
	public FirstPersonItem currentItem;

	// Token: 0x04001340 RID: 4928
	public FirstPersonItem drawnItem;

	// Token: 0x04001341 RID: 4929
	public bool finishedDrawingItem;

	// Token: 0x04001342 RID: 4930
	public float attackMainDelay;

	// Token: 0x04001343 RID: 4931
	public float attackSecondaryDelay;

	// Token: 0x04001344 RID: 4932
	public Transform leftHandObjectParent;

	// Token: 0x04001345 RID: 4933
	public Transform rightHandObjectParent;

	// Token: 0x04001346 RID: 4934
	public AnimationClip nothingClip;

	// Token: 0x04001347 RID: 4935
	private float equipSoundDelay;

	// Token: 0x04001348 RID: 4936
	private float holsterSoundDelay;

	// Token: 0x04001349 RID: 4937
	private Material fistMaterial;

	// Token: 0x0400134A RID: 4938
	private Material fingerUpperMaterial;

	// Token: 0x0400134B RID: 4939
	private Material fingerLowerMaterial;

	// Token: 0x0400134C RID: 4940
	private Material fingerTipMaterial;

	// Token: 0x0400134D RID: 4941
	private Material thumbJointMaterial;

	// Token: 0x0400134E RID: 4942
	public bool forceHolstered;

	// Token: 0x0400134F RID: 4943
	public FirstPersonItemController.InventorySlot selectedWhenForceHolstered;

	// Token: 0x04001350 RID: 4944
	public bool listenForHolster = true;

	// Token: 0x04001351 RID: 4945
	public bool listenForDrawFinish;

	// Token: 0x04001352 RID: 4946
	[Header("Interactions")]
	public Dictionary<InteractablePreset.InteractionKey, Interactable.InteractableCurrentAction> currentActions = new Dictionary<InteractablePreset.InteractionKey, Interactable.InteractableCurrentAction>();

	// Token: 0x04001353 RID: 4947
	public bool isConsuming;

	// Token: 0x04001354 RID: 4948
	public bool isRaised;

	// Token: 0x04001355 RID: 4949
	private bool takeOneActive;

	// Token: 0x04001356 RID: 4950
	[Header("Flashlight")]
	public bool flashlight;

	// Token: 0x04001357 RID: 4951
	public GameObject flashLightObject;

	// Token: 0x04001358 RID: 4952
	public GameObject captureLightObject;

	// Token: 0x04001359 RID: 4953
	public GameObject fingerprintLights;

	// Token: 0x0400135A RID: 4954
	public Light printScannerPulseLight;

	// Token: 0x0400135B RID: 4955
	[Tooltip("Point of raycast impact")]
	[Header("Print Scanner")]
	public Vector3 scannerRayPoint;

	// Token: 0x0400135C RID: 4956
	[Tooltip("Radius of detection")]
	public float printDetectionRadius = 0.3f;

	// Token: 0x0400135D RID: 4957
	[Header("Items")]
	public InteractablePreset worldCoin;

	// Token: 0x0400135E RID: 4958
	[Header("Audio")]
	public AudioController.LoopingSoundInfo activeLoop;

	// Token: 0x0400135F RID: 4959
	public AudioController.LoopingSoundInfo consumeLoop;

	// Token: 0x04001360 RID: 4960
	private Actor counterAttackActor;

	// Token: 0x04001361 RID: 4961
	private Vector3 counterAttackPoint;

	// Token: 0x04001362 RID: 4962
	private int updateInteractionCounter;

	// Token: 0x04001363 RID: 4963
	private static FirstPersonItemController _instance;

	// Token: 0x020002C6 RID: 710
	[Serializable]
	public class InventorySlot
	{
		// Token: 0x06000FE4 RID: 4068 RVA: 0x000E4DF0 File Offset: 0x000E2FF0
		public void SetSegmentContent(Interactable newI)
		{
			if (this.interactableID > -1)
			{
				FirstPersonItemController.Instance.EmptySlot(this, false, false, true, true);
			}
			if (newI != null)
			{
				this.interactableID = newI.id;
				newI.SetInInventory(Player.Instance);
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					this.debugName = newI.name + " (" + newI.preset.name + ")";
				}
			}
			else
			{
				this.interactableID = -1;
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					this.debugName = "empty";
				}
			}
			if (this.spawnedSegment != null)
			{
				this.spawnedSegment.OnUpdateContent();
			}
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x000E4EB4 File Offset: 0x000E30B4
		public Interactable GetInteractable()
		{
			Interactable interactable = null;
			if (this.interactableID > -1 && !CityData.Instance.savableInteractableDictionary.TryGetValue(this.interactableID, ref interactable))
			{
				Game.LogError("Unable to get inventory interactable " + this.interactableID.ToString() + ": " + this.debugName, 2);
				interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.interactableID);
				if (interactable != null)
				{
					Game.LogError("Found inventory interactable in directory but not savable dictionary!", 2);
				}
			}
			return interactable;
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x000E4F38 File Offset: 0x000E3138
		public FirstPersonItem GetFirstPersonItem()
		{
			FirstPersonItem result = GameplayControls.Instance.nothingItem;
			Interactable interactable = this.GetInteractable();
			if (interactable != null && interactable.preset.fpsItem != null)
			{
				result = interactable.preset.fpsItem;
			}
			if (this.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster)
			{
				result = GameplayControls.Instance.nothingItem;
			}
			else if (this.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.fists)
			{
				result = GameplayControls.Instance.fistsItem;
			}
			else if (this.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.watch)
			{
				result = GameplayControls.Instance.watchItem;
			}
			else if (this.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.printReader)
			{
				result = GameplayControls.Instance.printReader;
			}
			else if (this.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.coin)
			{
				result = GameplayControls.Instance.coinItem;
			}
			return result;
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x000E4FE8 File Offset: 0x000E31E8
		public void SetHotKey(string newHotkey)
		{
			Game.Log("Interface: Assign new hotkey to slot: " + newHotkey, 2);
			if (this.hotkey == newHotkey)
			{
				newHotkey = string.Empty;
			}
			this.hotkey = newHotkey;
			if (this.spawnedSegment != null)
			{
				this.spawnedSegment.UpdateHotkeyDisplay();
			}
			if (newHotkey.Length > 0)
			{
				foreach (FirstPersonItemController.InventorySlot inventorySlot in FirstPersonItemController.Instance.slots)
				{
					if (inventorySlot != this && inventorySlot.hotkey == newHotkey)
					{
						inventorySlot.hotkey = string.Empty;
						if (inventorySlot.spawnedSegment != null)
						{
							inventorySlot.spawnedSegment.UpdateHotkeyDisplay();
						}
					}
				}
			}
		}

		// Token: 0x04001364 RID: 4964
		public int index;

		// Token: 0x04001365 RID: 4965
		public int interactableID = -1;

		// Token: 0x04001366 RID: 4966
		public string debugName;

		// Token: 0x04001367 RID: 4967
		public string hotkey;

		// Token: 0x04001368 RID: 4968
		public FirstPersonItemController.InventorySlot.StaticSlot isStatic;

		// Token: 0x04001369 RID: 4969
		[NonSerialized]
		public InventorySquareController spawnedSegment;

		// Token: 0x020002C7 RID: 711
		public enum StaticSlot
		{
			// Token: 0x0400136B RID: 4971
			nonStatic,
			// Token: 0x0400136C RID: 4972
			holster,
			// Token: 0x0400136D RID: 4973
			watch,
			// Token: 0x0400136E RID: 4974
			fists,
			// Token: 0x0400136F RID: 4975
			coin,
			// Token: 0x04001370 RID: 4976
			printReader
		}
	}
}
