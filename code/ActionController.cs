using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class ActionController : MonoBehaviour
{
	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000629 RID: 1577 RVA: 0x00062248 File Offset: 0x00060448
	// (remove) Token: 0x0600062A RID: 1578 RVA: 0x00062280 File Offset: 0x00060480
	public event ActionController.PlayerAction OnPlayerAction;

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x0600062B RID: 1579 RVA: 0x000622B5 File Offset: 0x000604B5
	public static ActionController Instance
	{
		get
		{
			return ActionController._instance;
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x000622BC File Offset: 0x000604BC
	private void Awake()
	{
		if (ActionController._instance != null && ActionController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ActionController._instance = this;
		}
		this.allActions = AssetLoader.Instance.GetAllActions();
		foreach (AIActionPreset aiactionPreset in this.allActions)
		{
			MethodInfo method = base.GetType().GetMethod(aiactionPreset.name);
			if (method != null)
			{
				this.actionRef.TryAdd(aiactionPreset, method);
			}
		}
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00062370 File Offset: 0x00060570
	public void ExecuteAction(AIActionPreset action, Interactable what, NewNode where, Actor who)
	{
		MethodInfo methodInfo = null;
		if (action == null)
		{
			return;
		}
		if (this.actionRef.TryGetValue(action, ref methodInfo))
		{
			object[] array = new object[]
			{
				what,
				where,
				who
			};
			methodInfo.Invoke(this, array);
		}
		if (who != null && who.isPlayer && this.OnPlayerAction != null)
		{
			this.OnPlayerAction(action, what, where, who);
		}
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x000623E4 File Offset: 0x000605E4
	public void TurnOnMainLight(Interactable what, NewNode where, Actor who)
	{
		where.room.SetMainLights(true, string.Concat(new string[]
		{
			what.name,
			" ",
			where.name,
			" ",
			who.name
		}), who, false, false);
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00062438 File Offset: 0x00060638
	public void TurnOffMainLight(Interactable what, NewNode where, Actor who)
	{
		where.room.SetMainLights(false, string.Concat(new string[]
		{
			what.name,
			" ",
			where.name,
			" ",
			who.name
		}), who, false, false);
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x0006248A File Offset: 0x0006068A
	public void TurnOnSecondaryLight(Interactable what, NewNode where, Actor who)
	{
		where.room.SetSecondaryLight(true, false);
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x0006249C File Offset: 0x0006069C
	public void TurnOffSecondaryLight(Interactable what, NewNode where, Actor who)
	{
		what.SetSwitchState(false, who, true, false, false);
		bool newVal = false;
		using (List<Interactable>.Enumerator enumerator = where.room.secondaryLights.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.sw0)
				{
					newVal = true;
					break;
				}
			}
		}
		where.room.SetSecondaryLight(newVal, false);
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x0006248A File Offset: 0x0006068A
	public void TurnOnTV(Interactable what, NewNode where, Actor who)
	{
		where.room.SetSecondaryLight(true, false);
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00062514 File Offset: 0x00060714
	public void TurnOffTV(Interactable what, NewNode where, Actor who)
	{
		what.SetSwitchState(false, who, true, false, false);
		bool newVal = false;
		using (List<Interactable>.Enumerator enumerator = where.room.secondaryLights.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.sw0)
				{
					newVal = true;
					break;
				}
			}
		}
		where.room.SetSecondaryLight(newVal, false);
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x0006258C File Offset: 0x0006078C
	public void PickUp(Interactable what, NewNode where, Actor who)
	{
		InteractionController.Instance.PickUp(what);
		Predicate<NewAIController.TrackingTarget> <>9__0;
		foreach (Actor actor in where.room.currentOccupants)
		{
			if (!(actor == who) && !(actor.ai == null) && !(actor.speechController == null) && !actor.isDead && !actor.isStunned && !actor.isAsleep && actor.locationsOfAuthority.Contains(where.gameLocation))
			{
				List<NewAIController.TrackingTarget> trackedTargets = actor.ai.trackedTargets;
				Predicate<NewAIController.TrackingTarget> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((NewAIController.TrackingTarget item) => item.actor == who));
				}
				if (trackedTargets.Exists(predicate) && actor.speechController.speechQueue.Count <= 0)
				{
					actor.speechController.TriggerBark(SpeechController.Bark.confrontMessingAround);
					break;
				}
			}
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x000626AC File Offset: 0x000608AC
	public void PutDown(Interactable what, NewNode where, Actor who)
	{
		if (InteractionController.Instance.carryingObject != null)
		{
			if (InteractionController.Instance.carryingObject.interactable != null && InteractionController.Instance.carryingObject.interactable.preset.apartmentPlacementMode != InteractablePreset.ApartmentPlacementMode.physics)
			{
				AudioController.Instance.Play2DSound(AudioControls.Instance.furniturePlacement, null, 1f);
			}
			string text = "Put Down ";
			InteractableController carryingObject = InteractionController.Instance.carryingObject;
			Game.Log(text + ((carryingObject != null) ? carryingObject.ToString() : null), 2);
			InteractionController.Instance.carryingObject.DropThis(false);
		}
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00062748 File Offset: 0x00060948
	public void Throw(Interactable what, NewNode where, Actor who)
	{
		if (InteractionController.Instance.carryingObject != null)
		{
			string text = "Throw ";
			InteractableController carryingObject = InteractionController.Instance.carryingObject;
			Game.Log(text + ((carryingObject != null) ? carryingObject.ToString() : null), 2);
			InteractionController.Instance.carryingObject.DropThis(true);
		}
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x000627A0 File Offset: 0x000609A0
	public void OpenDoor(Interactable what, NewNode where, Actor who)
	{
		NewDoor newDoor = what.objectRef as NewDoor;
		if (newDoor != null)
		{
			if (who != null && !who.isPlayer)
			{
				if (what != newDoor.handleInteractable)
				{
					newDoor.handleInteractable.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
				}
				else if (newDoor.lockInteractableFront != null && what != newDoor.lockInteractableFront)
				{
					newDoor.lockInteractableFront.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
				}
				else if (newDoor.lockInteractableRear != null && what != newDoor.lockInteractableRear)
				{
					newDoor.lockInteractableRear.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
				}
			}
			if (!newDoor.isLocked && !newDoor.isJammed)
			{
				newDoor.OpenByActor(who, false, 1f);
			}
			else
			{
				if (who != null)
				{
					if (who.isPlayer)
					{
						InterfaceControls.Instance.lockedIcon.gameObject.SetActive(true);
						InteractionController.Instance.AlignInteractionIcons();
						if (!Game.Instance.sandboxMode && ChapterController.Instance != null && ChapterController.Instance.currentPart < 21)
						{
							InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Locked_help", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.door, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
						}
						else
						{
							InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Locked", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.door, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
						}
					}
					else
					{
						Game.Log("AI locked door open attempt: " + who.name, 2);
					}
				}
				AudioController.Instance.PlayWorldOneShot(newDoor.preset.audioLockedEntryAttempt, who, newDoor.parentedWall.node, newDoor.transform.position, null, null, 1f, null, false, null, false);
			}
			if (who != null && who.isPlayer)
			{
				newDoor.SetKnowLockedStatus(true);
			}
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00062994 File Offset: 0x00060B94
	public void CloseDoor(Interactable what, NewNode where, Actor who)
	{
		NewDoor newDoor = what.objectRef as NewDoor;
		if (newDoor != null)
		{
			if (who != null && what != newDoor.handleInteractable && !who.isPlayer)
			{
				newDoor.handleInteractable.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			}
			newDoor.SetOpen(0f, who, false, 1f);
			if (who == Player.Instance)
			{
				newDoor.SetKnowLockedStatus(true);
				return;
			}
			newDoor.SetKnowLockedStatus(false);
		}
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x00062A10 File Offset: 0x00060C10
	public void Open(Interactable what, NewNode where, Actor who)
	{
		if (what != null && who != null && !who.isPlayer)
		{
			what.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			what.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			if (what.lockInteractable != null && what.lockInteractable != what)
			{
				what.lockInteractable.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
				what.lockInteractable.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			}
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00062A80 File Offset: 0x00060C80
	public void KnockOnDoor(Interactable what, NewNode where, Actor who)
	{
		NewDoor newDoor = what.objectRef as NewDoor;
		if (newDoor != null)
		{
			float forceAdditionalUrgency = 0f;
			int knockCount = 2;
			if (who.isEnforcer && who.isOnDuty)
			{
				forceAdditionalUrgency = 7f;
				knockCount = 3;
				who.speechController.TriggerBark(SpeechController.Bark.enforcersKnock);
			}
			newDoor.OnKnock(who, knockCount, forceAdditionalUrgency);
		}
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00062AD8 File Offset: 0x00060CD8
	public void LockDoor(Interactable what, NewNode where, Actor who)
	{
		NewDoor newDoor = what.objectRef as NewDoor;
		if (!(newDoor != null))
		{
			if (what != null)
			{
				what.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
				what.SetLockedState(true, who, true, false);
			}
			return;
		}
		if (who != null && !who.isPlayer)
		{
			if (what != newDoor.handleInteractable && newDoor.handleInteractable != null)
			{
				newDoor.handleInteractable.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			}
			if (what != newDoor.lockInteractableFront && newDoor.lockInteractableFront != null)
			{
				newDoor.lockInteractableFront.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			}
			if (what != newDoor.lockInteractableRear && newDoor.lockInteractableRear != null)
			{
				newDoor.lockInteractableRear.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			}
		}
		newDoor.SetLocked(true, who, true);
		if (who == Player.Instance)
		{
			newDoor.SetKnowLockedStatus(true);
			return;
		}
		newDoor.SetKnowLockedStatus(false);
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x00062BB8 File Offset: 0x00060DB8
	public void UnlockDoor(Interactable what, NewNode where, Actor who)
	{
		NewDoor newDoor = what.objectRef as NewDoor;
		if (!(newDoor != null))
		{
			if (what != null)
			{
				what.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
				what.SetLockedState(false, who, true, false);
			}
			return;
		}
		if (who != null && !who.isPlayer)
		{
			if (what != newDoor.handleInteractable && newDoor.handleInteractable != null)
			{
				newDoor.handleInteractable.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			}
			if (what != newDoor.lockInteractableFront && newDoor.lockInteractableFront != null)
			{
				newDoor.lockInteractableFront.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			}
			if (what != newDoor.lockInteractableRear && newDoor.lockInteractableRear != null)
			{
				newDoor.lockInteractableRear.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
			}
		}
		newDoor.SetLocked(false, who, true);
		if (who == Player.Instance)
		{
			newDoor.SetKnowLockedStatus(true);
			return;
		}
		newDoor.SetKnowLockedStatus(false);
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x00062C98 File Offset: 0x00060E98
	public void Lockpick(Interactable what, NewNode where, Actor who)
	{
		NewDoor newDoor = what.objectRef as NewDoor;
		if (newDoor != null)
		{
			SessionData.Instance.TutorialTrigger("lockpicking", false);
			if (GameplayController.Instance.lockPicks > 0)
			{
				newDoor.OnLockpick();
			}
			else
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_lockpicks", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.lockpick, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.lockpicks, null, null, null);
			}
			if (who == Player.Instance)
			{
				newDoor.SetKnowLockedStatus(true);
				return;
			}
			newDoor.SetKnowLockedStatus(false);
			return;
		}
		else
		{
			SessionData.Instance.TutorialTrigger("lockpicking", false);
			if (GameplayController.Instance.lockPicks > 0)
			{
				what.OnLockpick();
				return;
			}
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_lockpicks", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.lockpick, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.lockpicks, null, null, null);
			return;
		}
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x00062D94 File Offset: 0x00060F94
	public void PeekUnderDoor(Interactable what, NewNode where, Actor who)
	{
		NewDoor newDoor = what.objectRef as NewDoor;
		if (newDoor != null)
		{
			newDoor.OnDoorPeek();
		}
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x00062DBC File Offset: 0x00060FBC
	public void Hide(Interactable what, NewNode where, Actor who)
	{
		Game.Log("Player: Hide", 2);
		Player.Instance.OnHide(what, 0, false, true);
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x00062DD8 File Offset: 0x00060FD8
	public void AnswerTelephone(Interactable what, NewNode where, Actor who)
	{
		if (who.isPlayer)
		{
			Player.Instance.OnAnswerPhone(what);
		}
		if (what.t != null && what.t.activeReceiver == null && who as Human != null)
		{
			what.t.SetTelephoneAnswered(who as Human);
		}
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00062E32 File Offset: 0x00061032
	public void AIHangUp(Interactable what, NewNode where, Actor who)
	{
		if (what.t != null)
		{
			what.t.SetTelephoneAnswered(null);
		}
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00062E48 File Offset: 0x00061048
	public void Return(Interactable what, NewNode where, Actor who)
	{
		if (who.isPlayer)
		{
			InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		}
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00062E5F File Offset: 0x0006105F
	public void PullPlayerFromHidingPlace(Interactable what, NewNode where, Actor who)
	{
		Game.Log("Player: AI has pulled player from hiding place", 2);
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x00062E7C File Offset: 0x0006107C
	public void TakeKey(Interactable what, NewNode where, Actor who)
	{
		int num = (int)what.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.roomID).value;
		foreach (NewNode.NodeAccess nodeAccess in CityData.Instance.roomDictionary[num].entrances)
		{
			if (nodeAccess != null && nodeAccess.wall != null && nodeAccess.wall.door != null && nodeAccess.wall.door.preset.lockType == DoorPreset.LockType.key)
			{
				who.AddToKeyring(nodeAccess.wall.door, true);
			}
		}
		if (who != null && who.isPlayer && InteractionController.Instance.GetValidPlayerActionIllegal(what, Player.Instance.currentNode, true, true))
		{
			StatusController.Instance.AddFineRecord(Player.Instance.currentNode.gameLocation.thisAsAddress, what, StatusController.CrimeType.theft, true, 100, false);
		}
		if (AchievementsController.Instance != null && what != null && what.subObject != null && what.subObject.preset != null && what.subObject.preset.presetName == "HiddenKey")
		{
			AchievementsController.Instance.UnlockAchievement("Easy Pickings", "find_hidden_key");
		}
		what.Delete();
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x00063000 File Offset: 0x00061200
	public void TakeBlueprints(Interactable what, NewNode where, Actor who)
	{
		foreach (KeyValuePair<int, NewFloor> keyValuePair in where.building.floors)
		{
			foreach (NewAddress newAddress in keyValuePair.Value.addresses)
			{
				foreach (NewRoom newRoom in newAddress.rooms)
				{
					newRoom.SetExplorationLevel(Mathf.Max(1, newRoom.explorationLevel));
					foreach (AirDuctGroup airDuctGroup in newRoom.ductGroups)
					{
						foreach (AirDuctGroup.AirDuctSection airDuctSection in airDuctGroup.airDucts)
						{
							airDuctSection.SetDiscovered(true);
						}
					}
				}
			}
		}
		if (who != null && who.isPlayer && InteractionController.Instance.GetValidPlayerActionIllegal(what, Player.Instance.currentNode, true, true) && what.val > 1f)
		{
			StatusController.Instance.AddFineRecord(Player.Instance.currentNode.gameLocation.thisAsAddress, what, StatusController.CrimeType.theft, true, Mathf.RoundToInt(what.val) * 10, false);
		}
		what.Delete();
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00063218 File Offset: 0x00061418
	public void TakeMoney(Interactable what, NewNode where, Actor who)
	{
		if (who != null && who.isPlayer)
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.pickUpMoney, null, 1f);
			GameplayController.Instance.AddMoney(Mathf.RoundToInt(what.val), true, "");
			if (InteractionController.Instance.GetValidPlayerActionIllegal(what, Player.Instance.currentNode, true, true) && what.val >= 1f)
			{
				StatusController.Instance.AddFineRecord(Player.Instance.currentNode.gameLocation.thisAsAddress, what, StatusController.CrimeType.theft, true, Mathf.RoundToInt(what.val) * 10, false);
			}
		}
		what.Delete();
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x000632D0 File Offset: 0x000614D0
	public void AIPickUpItemFromFloor(Interactable what, NewNode where, Actor who)
	{
		if (what == null)
		{
			return;
		}
		Game.Log(((who != null) ? who.ToString() : null) + " AI PICKUP FROM FLOOR: " + what.name, 2);
		if (who != null && !who.isPlayer)
		{
			Human human = who as Human;
			if (human == null)
			{
				return;
			}
			if (what.preset.isMoney)
			{
				foreach (Human.WalletItem walletItem in human.walletItems)
				{
					if (walletItem.itemType == Human.WalletItemType.money)
					{
						walletItem.money++;
						break;
					}
				}
				what.Delete();
				return;
			}
			if (what.preset.isInventoryItem && what.preset.consumableAmount > 0f && what.cs > 0f && what.belongsTo != who && who.locationsOfAuthority.Contains(where.gameLocation))
			{
				human.AddCurrentConsumable(what.preset);
				what.SafeDelete(true);
				return;
			}
			if (what.IsLitter())
			{
				human.AddTrash(what.preset, what.belongsTo, null);
				what.SafeDelete(true);
				return;
			}
			if (human.ai.currentGoal != null)
			{
				if (!what.originalPosition)
				{
					human.ai.currentGoal.TryInsertInteractableAction(what, RoutineControls.Instance.putBack, 6, what.spawnNode, true);
				}
				what.SetInInventory(human);
				return;
			}
			if (what.inInventory == null)
			{
				what.originalPosition = false;
				what.SetOriginalPosition(true, true);
			}
		}
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x0006347C File Offset: 0x0006167C
	public void AIPutBack(Interactable what, NewNode where, Actor who)
	{
		if (what == null)
		{
			return;
		}
		Game.Log("AI PUT BACK " + what.name, 2);
		what.SetAsNotInventory(what.spawnNode);
		what.originalPosition = false;
		what.SetOriginalPosition(true, true);
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x000634B4 File Offset: 0x000616B4
	public void CleanUp(Interactable what, NewNode where, Actor who)
	{
		if (what == null)
		{
			return;
		}
		Game.Log(((who != null) ? who.ToString() : null) + " AI CLEANUP ACTION: " + what.name, 2);
		if (who != null && !who.isPlayer)
		{
			Human human = who as Human;
			if (human == null)
			{
				return;
			}
			if (what.preset.isMoney)
			{
				foreach (Human.WalletItem walletItem in human.walletItems)
				{
					if (walletItem.itemType == Human.WalletItemType.money)
					{
						walletItem.money++;
						break;
					}
				}
				what.Delete();
				return;
			}
			if (what.preset.isInventoryItem && what.preset.consumableAmount > 0f && what.cs > 0f && what.belongsTo != who && who.locationsOfAuthority.Contains(where.gameLocation))
			{
				what.SafeDelete(true);
				return;
			}
			if (what.IsLitter())
			{
				what.SafeDelete(true);
			}
		}
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x000635E0 File Offset: 0x000617E0
	public void TakeSyncDisk(Interactable what, NewNode where, Actor who)
	{
		if (what == null)
		{
			return;
		}
		if (what.syncDisk == null)
		{
			Game.LogError("No sync disk associated with this object!", 2);
			return;
		}
		if (who != null && who.isPlayer && InteractionController.Instance.GetValidPlayerActionIllegal(what, Player.Instance.currentNode, true, true) && what.val > 1f)
		{
			StatusController.Instance.AddFineRecord(Player.Instance.currentNode.gameLocation.thisAsAddress, what, StatusController.CrimeType.theft, true, Mathf.RoundToInt(what.val) * 10, false);
		}
		SessionData.Instance.TutorialTrigger("upgrades", false);
		what.SafeDelete(true);
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x00063689 File Offset: 0x00061889
	public void TakeLockpick(Interactable what, NewNode where, Actor who)
	{
		AudioController.Instance.Play2DSound(AudioControls.Instance.pickUpLockpicks, null, 1f);
		GameplayController.Instance.AddLockpicks(1, true);
		SessionData.Instance.TutorialTrigger("lockpicking", false);
		what.Delete();
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x000636C8 File Offset: 0x000618C8
	public void TakeLockpickKit(Interactable what, NewNode where, Actor who)
	{
		AudioController.Instance.Play2DSound(AudioControls.Instance.pickUpLockpicks, null, 1f);
		GameplayController.Instance.AddLockpicks(30, true);
		SessionData.Instance.TutorialTrigger("lockpicking", false);
		what.Delete();
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x00063708 File Offset: 0x00061908
	public void Rob(Interactable what, NewNode where, Actor who)
	{
		what.SafeDelete(true);
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00063714 File Offset: 0x00061914
	public void Inspect(Interactable what, NewNode where, Actor who)
	{
		if (what.evidence != null)
		{
			SessionData.Instance.TutorialTrigger("evidence", false);
			SessionData.Instance.PauseGame(true, false, true);
			InterfaceController.Instance.SpawnWindow(what.evidence, Evidence.DataKey.photo, null, "", true, false, new Vector2(0.5f, 0.5f), what, null, null, true);
		}
		else
		{
			Game.LogError("No evidence to inspect...", 2);
		}
		what.ins = true;
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00063788 File Offset: 0x00061988
	public void InspectRemove(Interactable what, NewNode where, Actor who)
	{
		if (what.evidence != null)
		{
			SessionData.Instance.TutorialTrigger("evidence", false);
			SessionData.Instance.PauseGame(true, false, true);
			InterfaceController.Instance.SpawnWindow(what.evidence, Evidence.DataKey.photo, null, "", true, false, new Vector2(0.5f, 0.5f), what, null, null, true);
		}
		else
		{
			Game.Log("Player: No evidence to inspect...", 2);
		}
		what.ins = true;
		what.MarkAsTrash(true, false, 0f);
		what.RemoveFromPlacement();
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00063810 File Offset: 0x00061A10
	public void InspectComputer(Interactable what, NewNode where, Actor who)
	{
		if (Player.Instance.computerInteractable != null && Player.Instance.computerInteractable.controller != null && Player.Instance.computerInteractable.controller.computer != null)
		{
			foreach (GameObject gameObject in Player.Instance.computerInteractable.controller.computer.spawnedContent)
			{
				SurveillanceApp component = gameObject.GetComponent<SurveillanceApp>();
				if (component != null)
				{
					if (component.hoveredActor != null)
					{
						SessionData.Instance.PauseGame(true, false, true);
						InterfaceController.Instance.SpawnWindow(component.hoveredActor.human.evidenceEntry, Evidence.DataKey.photo, null, "", false, false, new Vector2(0.5f, 0.5f), null, null, null, true);
						break;
					}
					if (component.selectedActor != null)
					{
						SessionData.Instance.PauseGame(true, false, true);
						InterfaceController.Instance.SpawnWindow(component.selectedActor.human.evidenceEntry, Evidence.DataKey.photo, null, "", false, false, new Vector2(0.5f, 0.5f), null, null, null, true);
						break;
					}
				}
				ProfileApp component2 = gameObject.GetComponent<ProfileApp>();
				if (component2 != null && component2.controller.loggedInAs != null)
				{
					SessionData.Instance.PauseGame(true, false, true);
					InterfaceController.Instance.SpawnWindow(component2.controller.loggedInAs.evidenceEntry, Evidence.DataKey.photo, null, "", false, false, new Vector2(0.5f, 0.5f), null, null, null, true);
					break;
				}
			}
		}
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x000639F4 File Offset: 0x00061BF4
	public void InspectMultiPage(Interactable what, NewNode where, Actor who)
	{
		if (what.evidence != null)
		{
			EvidenceMultiPage evidenceMultiPage = what.evidence as EvidenceMultiPage;
			if (evidenceMultiPage != null)
			{
				using (List<EvidenceMultiPage.MultiPageContent>.Enumerator enumerator = evidenceMultiPage.GetContentForPage(evidenceMultiPage.page).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						EvidenceMultiPage.MultiPageContent multiPageContent = enumerator.Current;
						if (multiPageContent.evID != null && multiPageContent.evID.Length > 0)
						{
							Evidence passedEvidence = null;
							if (GameplayController.Instance.evidenceDictionary.TryGetValue(multiPageContent.evID, ref passedEvidence))
							{
								SessionData.Instance.PauseGame(true, false, true);
								InterfaceController.Instance.SpawnWindow(passedEvidence, Evidence.DataKey.photo, null, "", true, false, new Vector2(0.5f, 0.5f), null, null, null, true);
							}
							else
							{
								Game.LogError("Unable to find evidence " + multiPageContent.evID, 2);
							}
						}
						else if (multiPageContent.meta > 0)
						{
							MetaObject metaObject = CityData.Instance.FindMetaObject(multiPageContent.meta);
							if (metaObject != null)
							{
								Evidence evidence = metaObject.GetEvidence(true, where.nodeCoord);
								if (evidence != null)
								{
									SessionData.Instance.PauseGame(true, false, true);
									InterfaceController.Instance.SpawnWindow(evidence, Evidence.DataKey.photo, null, "", true, false, new Vector2(0.5f, 0.5f), null, null, null, true);
								}
								else
								{
									Game.LogError("Unable to get evidence from meta object " + metaObject.preset, 2);
								}
							}
							else
							{
								Game.LogError("Unable to get meta object " + multiPageContent.meta.ToString(), 2);
							}
						}
						if (multiPageContent.discEvID != null && multiPageContent.discEvID.Length > 0)
						{
							Evidence evidence2 = null;
							if (GameplayController.Instance.evidenceDictionary.TryGetValue(multiPageContent.discEvID, ref evidence2))
							{
								evidence2.AddDiscovery(multiPageContent.disc);
							}
							else
							{
								Game.LogError("Unable to find evidence " + multiPageContent.evID, 2);
							}
						}
					}
					goto IL_1EF;
				}
			}
			Game.Log("Player: No multi-page evidence to inspect...", 2);
		}
		else
		{
			Game.Log("Player: No evidence to inspect...", 2);
		}
		IL_1EF:
		what.ins = true;
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00063C14 File Offset: 0x00061E14
	public void TalkTo(Interactable what, NewNode where, Actor who)
	{
		Game.Log("TalkTo", 2);
		Actor isActor = what.controller.isActor;
		if (who.isPlayer)
		{
			if (isActor.interactingWith == null)
			{
				isActor.ai.TalkTo(InteractionController.ConversationType.normal);
				return;
			}
		}
		else if (isActor.isPlayer && who.interactingWith == null)
		{
			who.ai.TalkTo(InteractionController.ConversationType.normal);
		}
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00063C74 File Offset: 0x00061E74
	public void Call(Interactable what, NewNode where, Actor who)
	{
		if (who.isPlayer)
		{
			if (what.t != null && what.t.dialTone != null)
			{
				AudioController.Instance.StopSound(what.t.dialTone, AudioController.StopType.immediate, "phone hang up");
				what.t.dialTone = null;
			}
			InteractionController.Instance.SetDialog(true, what, false, null, InteractionController.ConversationType.normal);
		}
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x00063CD4 File Offset: 0x00061ED4
	public void Dial(Interactable what, NewNode where, Actor who)
	{
		if (who.isPlayer)
		{
			SessionData.Instance.PauseGame(true, false, true);
			InterfaceController.Instance.SpawnWindow(CityData.Instance.telephone, Evidence.DataKey.name, null, "", true, false, InterfaceControls.Instance.handbookWindowPosition, what, null, null, true);
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x00063D24 File Offset: 0x00061F24
	public void CallSomeone(Interactable what, NewNode where, Actor who)
	{
		List<Acquaintance> list = new List<Acquaintance>();
		Human human = who as Human;
		Human human2 = null;
		if (who != null && who.ai != null && who.ai.currentGoal != null && who.ai.currentGoal.passedVar > -2)
		{
			CityData.Instance.GetHuman(who.ai.currentGoal.passedVar, out human2, true);
		}
		if (human2 == null)
		{
			foreach (Acquaintance acquaintance in human.acquaintances)
			{
				if (!(acquaintance.with.home == null) && !(acquaintance.with.home == human.home) && acquaintance.with.home.telephones.Count > 0 && !acquaintance.with.isAtWork && !(acquaintance.with.currentGameLocation == where.gameLocation.thisAsAddress) && acquaintance.known >= SocialControls.Instance.telephoneBookInclusionThreshold)
				{
					int num;
					if (acquaintance.secretConnection == Acquaintance.ConnectionType.paramour || acquaintance.connections.Contains(Acquaintance.ConnectionType.paramour))
					{
						num = 7;
					}
					else if (acquaintance.connections.Contains(Acquaintance.ConnectionType.lover) && human.paramour == null)
					{
						num = 4;
					}
					else
					{
						num = Mathf.CeilToInt(acquaintance.like * 3f);
					}
					for (int i = 0; i < num; i++)
					{
						list.Add(acquaintance);
					}
				}
			}
		}
		if (list.Count > 0 || human2 != null)
		{
			if (what != null && what.preset.isPayphone)
			{
				AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.payphoneMoneyIn, who, where, what.GetWorldPosition(true), null, null, 1f, null, false, null, false);
			}
			if (list.Count > 0 && human2 == null)
			{
				human2 = list[Toolbox.Instance.Rand(0, list.Count, false)].with;
			}
			Game.Log("Phone: Citizen " + human.citizenName + " calling " + human2.citizenName, 2);
			if (what.t != null)
			{
				if (what.t.activeReceiver == null)
				{
					what.t.SetTelephoneAnswered(human);
				}
				string newDDS = CitizenControls.Instance.fallbackTelephoneConversation;
				if (human.dds.ContainsKey(DDSSaveClasses.TriggerPoint.vmail))
				{
					List<string> list2 = new List<string>();
					foreach (DDSSaveClasses.DDSTreeSave ddstreeSave in human.dds[DDSSaveClasses.TriggerPoint.vmail])
					{
						bool flag = true;
						List<Human> list3 = new List<Human>();
						for (int j = 0; j < 4; j++)
						{
							if (j == 0)
							{
								if (!human.DDSParticipantConditionCheck(human, ddstreeSave.participantA, DDSSaveClasses.TreeType.conversation))
								{
									flag = false;
									break;
								}
							}
							else
							{
								DDSSaveClasses.DDSParticipant ddsparticipant = ddstreeSave.participantB;
								if (j == 2)
								{
									ddsparticipant = ddstreeSave.participantC;
								}
								if (j == 3)
								{
									ddsparticipant = ddstreeSave.participantD;
								}
								if (ddsparticipant.required)
								{
									bool flag2 = false;
									List<Acquaintance> list4 = new List<Acquaintance>(human.acquaintances);
									int num2 = human.acquaintances.Count;
									while (list4.Count > 0 && num2 > 0)
									{
										int num3 = Toolbox.Instance.Rand(0, list4.Count, false);
										Acquaintance acquaintance2 = list4[num3];
										if (acquaintance2.with.DDSParticipantConditionCheck(human, ddsparticipant, DDSSaveClasses.TreeType.conversation))
										{
											list3.Add(acquaintance2.with);
											flag2 = true;
											break;
										}
										list4.RemoveAt(num3);
										num2--;
									}
									if (!flag2)
									{
										flag = false;
										break;
									}
								}
							}
						}
						if (flag)
						{
							list2.Add(ddstreeSave.id);
						}
					}
					if (list2.Count > 0)
					{
						newDDS = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
					}
				}
				if (human2.home.telephones.Count > 0)
				{
					TelephoneController.Instance.CreateNewCall(what.t, human2.home.telephones[0], human, human2, new TelephoneController.CallSource(TelephoneController.CallType.dds, newDDS), 0.1f, false);
					return;
				}
			}
		}
		else if (human != null && human.ai != null && human.ai.currentAction != null)
		{
			human.ai.currentAction.Complete();
		}
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x000641EC File Offset: 0x000623EC
	public void Say(Interactable what, NewNode where, Actor who)
	{
		if (InteractionController.Instance.dialogMode && InteractionController.Instance.dialogOptions.Count > 0)
		{
			DialogButtonController dialogButtonController = InteractionController.Instance.dialogOptions[InteractionController.Instance.dialogSelection];
			DialogPreset preset = dialogButtonController.option.preset;
			if (!dialogButtonController.selectable)
			{
				return;
			}
			if (dialogButtonController.option.preset.inputBox != DialogPreset.InputSetting.none)
			{
				PopupMessageController.Instance.PopupMessage(dialogButtonController.option.preset.inputBox.ToString(), true, false, "Confirm", "", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
				base.StartCoroutine(this._DialogInputBox(what, where, who, dialogButtonController, preset));
				return;
			}
			try
			{
				this._InvokeDialog(what, where, who, dialogButtonController, preset, DialogController.ForceSuccess.none);
			}
			catch
			{
			}
		}
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x000642EC File Offset: 0x000624EC
	public IEnumerator _DialogInputBox(Interactable what, NewNode where, Actor who, DialogButtonController button, DialogPreset preset)
	{
		while (PopupMessageController.Instance.active)
		{
			yield return null;
		}
		DialogController.ForceSuccess forceSuccess = DialogController.ForceSuccess.fail;
		if (preset.inputBox == DialogPreset.InputSetting.addressPassword && InteractionController.Instance.talkingTo != null)
		{
			Actor isActor = InteractionController.Instance.talkingTo.isActor;
			if (isActor != null && isActor.currentGameLocation != null && isActor.currentGameLocation.thisAsAddress != null)
			{
				if (PopupMessageController.Instance.inputField.text.ToLower() == isActor.currentGameLocation.thisAsAddress.GetPassword().ToLower())
				{
					Game.Log("Gameplay: Password is correct!", 2);
					forceSuccess = DialogController.ForceSuccess.success;
				}
				else
				{
					Game.Log("Gameplay: Password is incorrect: " + PopupMessageController.Instance.inputField.text.ToLower() + " != " + isActor.currentGameLocation.thisAsAddress.GetPassword(), 2);
				}
			}
		}
		this._InvokeDialog(what, where, who, button, preset, forceSuccess);
		yield break;
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00064320 File Offset: 0x00062520
	private void _InvokeDialog(Interactable what, NewNode where, Actor who, DialogButtonController button, DialogPreset preset, DialogController.ForceSuccess forceSuccess)
	{
		this._InvokeDialog(what, where, who, button.option, preset, forceSuccess);
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00064338 File Offset: 0x00062538
	private void _InvokeDialog(Interactable what, NewNode where, Actor who, EvidenceWitness.DialogOption option, DialogPreset preset, DialogController.ForceSuccess forceSuccess)
	{
		bool flag = DialogController.Instance.ExecuteDialog(option, InteractionController.Instance.talkingTo, where, who, forceSuccess);
		Game.Log(string.Concat(new string[]
		{
			"Gameplay: Executing dialog ",
			preset.name,
			" with forced success ",
			forceSuccess.ToString(),
			" (",
			flag.ToString(),
			")"
		}), 2);
		if (flag)
		{
			int cost = preset.GetCost(what.isActor, who);
			if (preset.specialCase == DialogPreset.SpecialCase.medicalCosts)
			{
				int num = Mathf.RoundToInt((float)cost * (1f - UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.reduceMedicalCosts)));
				if (num > 0)
				{
					GameplayController.Instance.AddMoney(-num, true, "dialog_cost");
				}
			}
			else if (cost > 0)
			{
				GameplayController.Instance.AddMoney(-cost, true, "dialog_cost");
			}
			if (preset.specialCase == DialogPreset.SpecialCase.hotelBill)
			{
				GameplayController.HotelGuest hotelRoom = Toolbox.Instance.GetHotelRoom(who as Human);
				if (hotelRoom != null)
				{
					hotelRoom.PayBill(cost);
				}
			}
			else if (preset.specialCase == DialogPreset.SpecialCase.hotelCheckOut)
			{
				GameplayController.HotelGuest hotelRoom2 = Toolbox.Instance.GetHotelRoom(who as Human);
				if (hotelRoom2 != null)
				{
					hotelRoom2.PayBill(cost);
				}
				GameplayController.Instance.RemoveHotelGuest(hotelRoom2.GetAddress(), who as Human, true);
			}
			else if (preset.specialCase == DialogPreset.SpecialCase.rentHotelRoomCheap)
			{
				GameplayController.Instance.AddHotelGuest(who as Human, false);
			}
			else if (preset.specialCase == DialogPreset.SpecialCase.rentHotelRoomExpensive)
			{
				GameplayController.Instance.AddHotelGuest(who as Human, true);
			}
			foreach (DialogPreset dialogPreset in preset.removeDialogOnSuccess)
			{
				(what.evidence as EvidenceWitness).RemoveDialogOption(dialogPreset.tiedToKey, dialogPreset, null, null);
			}
			using (List<DialogPreset>.Enumerator enumerator = preset.followUpDialogSuccess.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DialogPreset dialogPreset2 = enumerator.Current;
					Game.Log("Adding follow up dialog: " + dialogPreset2.name, 2);
					(what.evidence as EvidenceWitness).AddDialogOption(dialogPreset2.tiedToKey, dialogPreset2, null, null, true);
				}
				goto IL_2D7;
			}
		}
		foreach (DialogPreset dialogPreset3 in preset.removeDialogOnFail)
		{
			(what.evidence as EvidenceWitness).RemoveDialogOption(dialogPreset3.tiedToKey, dialogPreset3, null, null);
		}
		foreach (DialogPreset dialogPreset4 in preset.followUpDialogFail)
		{
			(what.evidence as EvidenceWitness).AddDialogOption(dialogPreset4.tiedToKey, dialogPreset4, null, null, true);
		}
		IL_2D7:
		foreach (DialogPreset dialogPreset5 in preset.removeDialog)
		{
			(what.evidence as EvidenceWitness).RemoveDialogOption(dialogPreset5.tiedToKey, dialogPreset5, null, null);
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x000646AC File Offset: 0x000628AC
	public void CrawlIntoVent(Interactable what, NewNode where, Actor who)
	{
		if (Player.Instance.inAirVent)
		{
			Player.Instance.OnCrawlOutOfVent(what, false);
			return;
		}
		Player.Instance.OnCrawlIntoVent(what, false);
		SessionData.Instance.TutorialTrigger("airducts", false);
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x000646E4 File Offset: 0x000628E4
	public void UseKeypad(Interactable what, NewNode where, Actor who)
	{
		if (what.evidence != null && who.isPlayer)
		{
			SessionData.Instance.PauseGame(true, false, true);
			InterfaceController.Instance.SpawnWindow(what.evidence, Evidence.DataKey.photo, null, "", true, false, new Vector2(0.5f, 0.5f), what, null, null, true);
			Interactable thisDoor = what.thisDoor;
			if (thisDoor != null)
			{
				NewDoor newDoor = thisDoor.objectRef as NewDoor;
				if (newDoor != null && Player.Instance.keyring.Contains(newDoor))
				{
					string text = "Player bypassed list includes door ";
					NewDoor newDoor2 = newDoor;
					Game.Log(text + ((newDoor2 != null) ? newDoor2.ToString() : null), 2);
					List<string> list;
					InterfaceController.Instance.InputCodeButton(what.GetPasswordFromSource(out list));
				}
			}
		}
		if (who != null && !who.isPlayer)
		{
			Game.Log("USE KEYPAD BY AI: " + what.preset.name, 2);
			what.SetLockedState(false, who, true, false);
			if (what.thisDoor != null)
			{
				what.thisDoor.SetLockedState(false, who, true, false);
			}
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x000647F0 File Offset: 0x000629F0
	public void NextPage(Interactable what, NewNode where, Actor who)
	{
		EvidenceMultiPage evidenceMultiPage = what.evidence as EvidenceMultiPage;
		if (evidenceMultiPage != null && evidenceMultiPage.page < what.controller.pages.Count)
		{
			evidenceMultiPage.SetPage(evidenceMultiPage.page + 1, false);
		}
		if (what.preset.pageTurnReadingDelay > 0f)
		{
			what.readingDelay = what.preset.pageTurnReadingDelay;
		}
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00064858 File Offset: 0x00062A58
	public void PreviousPage(Interactable what, NewNode where, Actor who)
	{
		EvidenceMultiPage evidenceMultiPage = what.evidence as EvidenceMultiPage;
		if (evidenceMultiPage != null && evidenceMultiPage.page > 0)
		{
			evidenceMultiPage.SetPage(evidenceMultiPage.page - 1, false);
		}
		if (what.preset.pageTurnReadingDelay > 0f)
		{
			what.readingDelay = what.preset.pageTurnReadingDelay;
		}
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x000648AF File Offset: 0x00062AAF
	public void SetCurrentMonth(Interactable what, NewNode where, Actor who)
	{
		(what.evidence as EvidenceMultiPage).SetPage(SessionData.Instance.monthInt + 1, false);
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x000648CE File Offset: 0x00062ACE
	public void Sleep(Interactable what, NewNode where, Actor who)
	{
		this.Hide(what, where, who);
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x000648D9 File Offset: 0x00062AD9
	public void GetUp(Interactable what, NewNode where, Actor who)
	{
		this.Return(what, where, who);
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x000648E4 File Offset: 0x00062AE4
	public void CallElevator(Interactable what, NewNode where, Actor who)
	{
		Elevator elevator = what.objectRef as Elevator;
		if (elevator != null)
		{
			Game.Log("Call elevator search for buttons...", 2);
			using (Dictionary<int, Elevator.ElevatorFloor>.Enumerator enumerator = elevator.elevatorFloors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, Elevator.ElevatorFloor> keyValuePair = enumerator.Current;
					if (keyValuePair.Value.downButton == what)
					{
						elevator.CallElevator(keyValuePair.Value.floor, false);
						break;
					}
					if (keyValuePair.Value.upButton == what)
					{
						elevator.CallElevator(keyValuePair.Value.floor, true);
						break;
					}
				}
				return;
			}
		}
		Game.Log("No elevator reference!", 2);
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x000649A0 File Offset: 0x00062BA0
	public void PassTime(Interactable what, NewNode where, Actor who)
	{
		Player.Instance.OnHide(what, 1, true, true);
		BioScreenController.Instance.SelectSlot(FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.watch), false, false);
		Player.Instance.setAlarmModeAfterDelay = 1f;
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00064A04 File Offset: 0x00062C04
	public void CancelPassTime(Interactable what, NewNode where, Actor who)
	{
		Player.Instance.SetSpendingTimeMode(false);
		Player.Instance.OnHide(what, 0, true, false);
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00064A20 File Offset: 0x00062C20
	public void HoursMinutesToggle(Interactable what, NewNode where, Actor who)
	{
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.watchToggleHoursMinutes, Player.Instance, Player.Instance.currentNode, Player.Instance.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
		Player.Instance.editingHours = !Player.Instance.editingHours;
		SessionData.Instance.UpdateUIClock();
		SessionData.Instance.UpdateUIDay();
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00064A98 File Offset: 0x00062C98
	public void ActivateTimePass(Interactable what, NewNode where, Actor who)
	{
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.setAlarm, Player.Instance, Player.Instance.currentNode, Player.Instance.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
		Player.Instance.SetSettingAlarmMode(false);
		Player.Instance.OnHide(what, 0, true, false);
		BioScreenController.Instance.SelectSlot(FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, false);
		Player.Instance.spendingTimeDelay = 0.5f;
		Game.Log("Player: Activate time pass...", 2);
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00064B50 File Offset: 0x00062D50
	public void WatchForward(Interactable what, NewNode where, Actor who)
	{
		if (Player.Instance.editingHours)
		{
			Player.Instance.AddToAlarmTime(1f);
			return;
		}
		Player.Instance.AddToAlarmTime(0.016666668f);
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00064B7D File Offset: 0x00062D7D
	public void WatchBack(Interactable what, NewNode where, Actor who)
	{
		if (Player.Instance.editingHours)
		{
			Player.Instance.AddToAlarmTime(-1f);
			return;
		}
		Player.Instance.AddToAlarmTime(-0.016666668f);
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00064BAA File Offset: 0x00062DAA
	public void HideInstant(Interactable what, NewNode where, Actor who)
	{
		Player.Instance.OnHide(what, 0, true, true);
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00064BBC File Offset: 0x00062DBC
	public void BargeDoor(Interactable what, NewNode where, Actor who)
	{
		NewDoor newDoor = what.objectRef as NewDoor;
		if (newDoor != null && !newDoor.isClosed)
		{
			return;
		}
		if (who.isPlayer)
		{
			this.bargeDoor = what;
			Player.Instance.TransformPlayerController(GameplayControls.Instance.bargeDoorEnter, null, what, null, false, false, 0f, false, default(Vector3), 1f, true);
			Player.Instance.OnTransitionCompleted += this.BargeReturn;
			return;
		}
		who.ai.bargeTimer = 0f;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00064C4C File Offset: 0x00062E4C
	public void BargeReturn(bool restoreTransform = false)
	{
		Game.Log("Player: Barge return...", 2);
		Player.Instance.OnTransitionCompleted -= this.BargeReturn;
		if (this.bargeDoor == null)
		{
			return;
		}
		(this.bargeDoor.objectRef as NewDoor).Barge(Player.Instance);
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00064C9D File Offset: 0x00062E9D
	public void UseComputer(Interactable what, NewNode where, Actor who)
	{
		Player.Instance.OnUseComputer(what, false);
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00064CAB File Offset: 0x00062EAB
	public void ReturnComputer(Interactable what, NewNode where, Actor who)
	{
		Player.Instance.OnReturnFromUseComputer();
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00064CB8 File Offset: 0x00062EB8
	public void TriggerAlarm(Interactable what, NewNode where, Actor who)
	{
		if (where != null && where.gameLocation != null && where.gameLocation.thisAsAddress != null)
		{
			if (!what.sw0)
			{
				return;
			}
			Interactable breakerSecurity = where.gameLocation.thisAsAddress.GetBreakerSecurity();
			if (breakerSecurity != null && !breakerSecurity.sw0)
			{
				return;
			}
			Human target = null;
			if (who.ai != null && who.ai.persuitTarget != null)
			{
				target = (who.ai.persuitTarget as Human);
			}
			else if (who != null)
			{
				Human human = who as Human;
				if (human != null && human.lastScaredBy != null)
				{
					target = (human.lastScaredBy as Human);
				}
			}
			where.gameLocation.thisAsAddress.SetAlarm(true, target);
			if (who != null)
			{
				who.AddNerve(CitizenControls.Instance.nerveAlarmSwitched, null);
			}
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00064DB0 File Offset: 0x00062FB0
	public void Search(Interactable what, NewNode where, Actor who)
	{
		if (who.isPlayer)
		{
			Player.Instance.OnSearch(what);
			if (what.isActor != null && what.isActor.isDead && MurderController.Instance.activeMurders.Exists((MurderController.Murder item) => item.victim == what.isActor))
			{
				MurderController.Instance.OnVictimDiscovery();
			}
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00064E2E File Offset: 0x0006302E
	public void Vomit(Interactable what, NewNode where, Actor who)
	{
		if (who.isPlayer)
		{
			Player.Instance.OnGenericTimedAction("Vomiting", 1f, 0.5f, what, false);
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00064E53 File Offset: 0x00063053
	public void TakePrint(Interactable what, NewNode where, Actor who)
	{
		Player.Instance.OnTakePrint(what);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00064E60 File Offset: 0x00063060
	public void NextChoice(Interactable what, NewNode where, Actor who)
	{
		if (InteractionController.Instance.dialogMode)
		{
			InteractionController.Instance.SetDialogSelection(InteractionController.Instance.dialogSelection - 1);
		}
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00064E84 File Offset: 0x00063084
	public void PreviousChoice(Interactable what, NewNode where, Actor who)
	{
		if (InteractionController.Instance.dialogMode)
		{
			InteractionController.Instance.SetDialogSelection(InteractionController.Instance.dialogSelection + 1);
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00064EA8 File Offset: 0x000630A8
	public void TakeFirstPersonItem(Interactable what, NewNode where, Actor who)
	{
		SessionData.Instance.TutorialTrigger("inventory", false);
		FirstPersonItemController.Instance.PickUpItem(what, false, true, true, true, true);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00064ECB File Offset: 0x000630CB
	private IEnumerator AddFirstPersonItemDelay(Interactable newInteractable)
	{
		float delay = 0.9f;
		while (delay > 0f)
		{
			delay -= Time.deltaTime;
			yield return null;
		}
		FirstPersonItemController.Instance.PickUpItem(newInteractable, false, false, true, true, true);
		yield break;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00064EDA File Offset: 0x000630DA
	public void TakeFirstPersonItemUsed(Interactable what, NewNode where, Actor who)
	{
		if (what.sw0)
		{
			FirstPersonItemController.Instance.PickUpItem(what, false, true, true, true, true);
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00064EF8 File Offset: 0x000630F8
	public void Buy(Interactable what, NewNode where, Actor who)
	{
		if (who.isPlayer)
		{
			Game.Log("Interface: Spawn buy select...", 2);
			SessionData.Instance.PauseGame(true, false, true);
			InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "Buy", true, false, InterfaceControls.Instance.handbookWindowPosition, what, null, null, true);
		}
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00064F48 File Offset: 0x00063148
	public void TakeConsumable(Interactable what, NewNode where, Actor who)
	{
		Human human = who as Human;
		if (human != null)
		{
			Dictionary<InteractablePreset, int> dictionary = new Dictionary<InteractablePreset, int>();
			Company company = null;
			bool flag = true;
			if (what.preset.specialCaseFlag == InteractablePreset.SpecialCase.fridge)
			{
				foreach (Interactable interactable in what.furnitureParent.spawnedInteractables)
				{
					if (interactable.preset.retailItem != null && !dictionary.ContainsKey(interactable.preset))
					{
						dictionary.Add(interactable.preset, 0);
					}
				}
				flag = false;
			}
			else if (what.preset.menuOverride != null)
			{
				foreach (InteractablePreset interactablePreset in what.preset.menuOverride.itemsSold)
				{
					int num = Mathf.RoundToInt(Mathf.Lerp(interactablePreset.value.x, interactablePreset.value.y, 1f));
					dictionary.Add(interactablePreset, num);
				}
				flag = what.preset.menuOverride.createReceipt;
				if (what.preset.menuOverride.purchaseAudio != null)
				{
					AudioController.Instance.PlayWorldOneShot(what.preset.menuOverride.purchaseAudio, who, what.node, what.wPos, what, null, 1f, null, false, null, false);
				}
			}
			else if (where.gameLocation.thisAsAddress != null && where.gameLocation.thisAsAddress.company != null)
			{
				dictionary = where.gameLocation.thisAsAddress.company.prices;
				company = where.gameLocation.thisAsAddress.company;
			}
			else if (what.furnitureParent != null && what.furnitureParent.ownerMap.Count > 0)
			{
				foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in what.furnitureParent.ownerMap)
				{
					if (keyValuePair.Key.human != null && keyValuePair.Key.human.job != null && keyValuePair.Key.human.job.employer != null && keyValuePair.Key.human.job.preset.selfEmployed)
					{
						dictionary = keyValuePair.Key.human.job.employer.prices;
						company = keyValuePair.Key.human.job.employer;
						break;
					}
				}
			}
			List<InteractablePreset> list = new List<InteractablePreset>();
			float nourishment = human.nourishment;
			float hydration = human.hydration;
			float alertness = human.alertness;
			float energy = human.energy;
			float excitement = human.excitement;
			float hygiene = human.hygiene;
			float chores = human.chores;
			float heat = human.heat;
			for (int i = 0; i < 3; i++)
			{
				int num2;
				InteractablePreset interactablePreset2 = human.PickConsumable(ref dictionary, out num2, list);
				if (!(interactablePreset2 != null))
				{
					break;
				}
				list.Add(interactablePreset2);
				human.AddCurrentConsumable(interactablePreset2);
				if (interactablePreset2.retailItem != null)
				{
					human.nourishment += interactablePreset2.retailItem.nourishment;
					human.currentHealth += interactablePreset2.retailItem.health;
					human.hydration += interactablePreset2.retailItem.hydration;
					human.alertness += interactablePreset2.retailItem.alertness;
					human.energy += interactablePreset2.retailItem.energy;
					human.excitement += interactablePreset2.retailItem.excitement;
					human.hygiene += interactablePreset2.retailItem.hygiene;
					human.chores += interactablePreset2.retailItem.chores;
					human.heat += interactablePreset2.retailItem.heat;
				}
				if (Toolbox.Instance.Rand(0f, 1f, false) > 2.5f + human.societalClass * 0.75f)
				{
					break;
				}
			}
			human.nourishment = nourishment;
			human.hydration = hydration;
			human.alertness = alertness;
			human.energy = energy;
			human.excitement = excitement;
			human.hygiene = hygiene;
			human.chores = chores;
			human.heat = heat;
			if (list.Count > 0)
			{
				if (flag)
				{
					List<Interactable.Passed> list2 = new List<Interactable.Passed>();
					if (company != null)
					{
						list2.Add(new Interactable.Passed(Interactable.PassedVarType.companyID, (float)company.companyID, null));
					}
					list2.Add(new Interactable.Passed(Interactable.PassedVarType.time, SessionData.Instance.gameTime, null));
					for (int j = 0; j < list.Count; j++)
					{
						list2.Add(new Interactable.Passed(Interactable.PassedVarType.stringInteractablePreset, -1f, list[j].name));
					}
					human.AddTrash(InteriorControls.Instance.receipt, human, list2);
				}
				if (company != null && company.preset.recordSalesData)
				{
					company.AddSalesRecord(human, list, SessionData.Instance.gameTime);
				}
			}
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x000654DC File Offset: 0x000636DC
	public void MakeCoffeeStart(Interactable what, NewNode where, Actor who)
	{
		if (what.furnitureParent == null)
		{
			return;
		}
		Interactable interactable = null;
		FurniturePreset.SubObject subObject = what.furnitureParent.furniture.subObjects.Find((FurniturePreset.SubObject item) => item.preset.name == "HobBoilPoint");
		foreach (NewNode newNode in where.room.nodes)
		{
			foreach (Interactable interactable2 in newNode.interactables)
			{
				if (interactable2.preset == InteriorControls.Instance.stovetopKettle && interactable2.inInventory == null && (interactable2.controller == null || !interactable2.controller.isCarriedByPlayer))
				{
					interactable = interactable2;
					interactable.ConvertToFurnitureSpawnedObject(what.furnitureParent, subObject, true, true);
					break;
				}
			}
			if (interactable != null)
			{
				break;
			}
		}
		if (interactable == null)
		{
			interactable = InteractableCreator.Instance.CreateFurnitureSpawnedInteractable(InteriorControls.Instance.stovetopKettle, what.furnitureParent, subObject, null, null, null, null, null, null, "");
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00065634 File Offset: 0x00063834
	public void MakeCoffeeEnd(Interactable what, NewNode where, Actor who)
	{
		Human human = who as Human;
		if (human != null)
		{
			if (human.characterTraits.Exists((Human.Trait item) => item.trait == CitizenControls.Instance.coffeeLiker))
			{
				human.AddCurrentConsumable(InteriorControls.Instance.coffeeHomemade);
				return;
			}
			if (human.characterTraits.Exists((Human.Trait item) => item.trait == CitizenControls.Instance.teaLiker))
			{
				human.AddCurrentConsumable(InteriorControls.Instance.teaHomemade);
				return;
			}
			if (Toolbox.Instance.Rand(0f, 1f, false) > 0.3f)
			{
				human.AddCurrentConsumable(InteriorControls.Instance.coffeeHomemade);
				return;
			}
			human.AddCurrentConsumable(InteriorControls.Instance.teaHomemade);
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0006570C File Offset: 0x0006390C
	public void TurnOnHob(Interactable what, NewNode where, Actor who)
	{
		if (what.furnitureParent != null)
		{
			FurniturePreset.SubObject subObject = what.furnitureParent.furniture.subObjects.Find((FurniturePreset.SubObject item) => item.preset.name == "HobBoilPoint");
			if (subObject != null)
			{
				Vector3 vector = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0f, (float)(what.furnitureParent.angle + what.furnitureParent.diagonalAngle), 0f)), Vector3.one).MultiplyPoint3x4(subObject.localPos) + what.furnitureParent.anchorNode.position;
				foreach (Interactable interactable in where.interactables)
				{
					if (interactable.preset == InteriorControls.Instance.stovetopKettle && Vector3.Distance(vector, interactable.GetWorldPosition(true)) < 0.3f)
					{
						interactable.SetSwitchState(true, null, true, false, false);
					}
				}
			}
		}
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00065838 File Offset: 0x00063A38
	public void TurnOffHob(Interactable what, NewNode where, Actor who)
	{
		foreach (Interactable interactable in where.interactables)
		{
			if (interactable.preset == InteriorControls.Instance.stovetopKettle)
			{
				interactable.SetSwitchState(false, null, true, false, false);
			}
		}
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x000658A8 File Offset: 0x00063AA8
	public void TurnOnMusic(Interactable what, NewNode where, Actor who)
	{
		if (where != null)
		{
			where.room.musicPlaying = true;
			where.room.musicStartedAt = SessionData.Instance.gameTime;
			foreach (Actor actor in where.room.currentOccupants)
			{
				if (actor.ai != null && actor != who)
				{
					actor.ai.AITick(false, false);
				}
			}
		}
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00065944 File Offset: 0x00063B44
	public void TurnOffMusic(Interactable what, NewNode where, Actor who)
	{
		if (where != null)
		{
			where.room.musicPlaying = false;
			foreach (NewNode newNode in where.room.nodes)
			{
				foreach (Interactable interactable in newNode.interactables)
				{
					if (interactable != what && interactable.aiActionReference.ContainsKey(RoutineControls.Instance.turnOnMusic) && interactable.sw0)
					{
						where.room.musicPlaying = true;
						break;
					}
				}
			}
			Game.Log("SET MUSIC: " + where.room.name + " " + where.room.musicPlaying.ToString(), 2);
		}
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00065A44 File Offset: 0x00063C44
	public void PurchaseItem(Interactable what, NewNode where, Actor who)
	{
		Human human = who as Human;
		if (human != null)
		{
			Dictionary<InteractablePreset, int> dictionary = new Dictionary<InteractablePreset, int>();
			Company company = null;
			bool flag = true;
			if (what.preset.menuOverride != null)
			{
				foreach (InteractablePreset interactablePreset in what.preset.menuOverride.itemsSold)
				{
					int num = Mathf.RoundToInt(Mathf.Lerp(interactablePreset.value.x, interactablePreset.value.y, 1f));
					dictionary.Add(interactablePreset, num);
				}
				flag = what.preset.menuOverride.createReceipt;
				if (what.preset.menuOverride.purchaseAudio != null)
				{
					AudioController.Instance.PlayWorldOneShot(what.preset.menuOverride.purchaseAudio, who, what.node, what.wPos, what, null, 1f, null, false, null, false);
				}
			}
			else if (where.gameLocation.thisAsAddress != null && where.gameLocation.thisAsAddress.company != null)
			{
				dictionary = where.gameLocation.thisAsAddress.company.prices;
				company = where.gameLocation.thisAsAddress.company;
			}
			else if (what.furnitureParent != null && what.furnitureParent.ownerMap.Count > 0)
			{
				foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in what.furnitureParent.ownerMap)
				{
					if (keyValuePair.Key.human != null && keyValuePair.Key.human.job != null && keyValuePair.Key.human.job.employer != null && keyValuePair.Key.human.job.preset.selfEmployed)
					{
						dictionary = keyValuePair.Key.human.job.employer.prices;
						company = keyValuePair.Key.human.job.employer;
						break;
					}
				}
			}
			List<InteractablePreset> list = new List<InteractablePreset>();
			if (human.ai != null && human.ai.currentAction != null && human.ai.currentAction.passedAcquireItems != null)
			{
				foreach (InteractablePreset interactablePreset2 in human.ai.currentAction.passedAcquireItems)
				{
					list.Add(interactablePreset2);
					Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(interactablePreset2, human, human, null, Vector3.zero, Vector3.zero, null, null, "");
					if (interactable != null)
					{
						interactable.SetInInventory(human);
					}
				}
				if (human.ai.currentGoal.murderRef != null)
				{
					human.ai.currentGoal.murderRef.EuipmentCheck();
				}
			}
			if (list.Count > 0)
			{
				if (flag)
				{
					List<Interactable.Passed> list2 = new List<Interactable.Passed>();
					if (company != null)
					{
						list2.Add(new Interactable.Passed(Interactable.PassedVarType.companyID, (float)company.companyID, null));
					}
					list2.Add(new Interactable.Passed(Interactable.PassedVarType.time, SessionData.Instance.gameTime, null));
					for (int i = 0; i < list.Count; i++)
					{
						list2.Add(new Interactable.Passed(Interactable.PassedVarType.stringInteractablePreset, -1f, list[i].name));
					}
					human.AddTrash(InteriorControls.Instance.receipt, human, list2);
				}
				if (company != null && company.preset.recordSalesData)
				{
					company.AddSalesRecord(human, list, SessionData.Instance.gameTime);
				}
			}
		}
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00065E4C File Offset: 0x0006404C
	public void Consume(Interactable what, NewNode where, Actor who)
	{
		Human human = who as Human;
		int num = 99;
		while (human != null && human.currentConsumables.Count > 0 && num > 0)
		{
			human.RemoveCurrentConsumable(human.currentConsumables[0]);
			num--;
		}
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00065E98 File Offset: 0x00064098
	public void Dispose(Interactable what, NewNode where, Actor who)
	{
		if (who != null && who.isPlayer && what != null && BioScreenController.Instance.selectedSlot != null)
		{
			int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.streetCleaningMoney));
			Interactable interactable = BioScreenController.Instance.selectedSlot.GetInteractable();
			if (interactable != null)
			{
				if (num > 0 && (what.preset.specialCaseFlag == InteractablePreset.SpecialCase.garbageDisposal || (what.preset.prefab != null && what.preset.prefab.CompareTag("Garbage")) || (what.spawnedObject != null && what.spawnedObject.CompareTag("Garbage"))))
				{
					Game.Log("Player: Disposal in trash", 2);
					if (interactable.belongsTo != Player.Instance && (CleanupController.Instance.trash.Contains(interactable) || (interactable.preset.retailItem != null && interactable.cs <= 0.1f && interactable.preset.retailItem.isConsumable)))
					{
						interactable.SafeDelete(false);
						GameplayController.Instance.AddMoney(num, true, "readingforstreetcleaning");
					}
					else
					{
						Game.Log("Player: This item is not trash", 2);
					}
				}
				if (interactable != null)
				{
					EvidenceMultiPage evidenceMultiPage = what.evidence as EvidenceMultiPage;
					if (evidenceMultiPage != null)
					{
						MetaObject containedMetaObject = new MetaObject(interactable.preset, interactable.belongsTo, interactable.writer, interactable.reciever, interactable.pv);
						List<EvidenceMultiPage.MultiPageContent> list = evidenceMultiPage.pageContent.FindAll((EvidenceMultiPage.MultiPageContent item) => item.meta > 0);
						if (list.Count >= GameplayControls.Instance.binTrashLimit)
						{
							int num2 = 9999999;
							EvidenceMultiPage.MultiPageContent multiPageContent = null;
							foreach (EvidenceMultiPage.MultiPageContent multiPageContent2 in list)
							{
								if (multiPageContent2.page < num2)
								{
									num2 = multiPageContent2.page;
									multiPageContent = multiPageContent2;
								}
							}
							if (multiPageContent != null)
							{
								if (multiPageContent.meta > 0)
								{
									MetaObject metaObject = CityData.Instance.FindMetaObject(multiPageContent.meta);
									if (metaObject != null)
									{
										metaObject.Remove();
									}
								}
								evidenceMultiPage.pageContent.Remove(multiPageContent);
							}
							evidenceMultiPage.AddContainedMetaObjectToPage(num2, containedMetaObject);
						}
						else
						{
							evidenceMultiPage.AddContainedMetaObjectToNewPage(containedMetaObject);
							CleanupController.Instance.binTrash++;
						}
					}
					FirstPersonItemController.Instance.EmptySlot(BioScreenController.Instance.selectedSlot, false, true, false, true);
					if (AchievementsController.Instance != null)
					{
						AchievementsController.Instance.AddToStat("Spick-and-Span", "junk_disposed", 1);
						return;
					}
				}
			}
		}
		else if (who != null && !who.isPlayer)
		{
			Human human = who as Human;
			if (human != null && what != null)
			{
				EvidenceMultiPage evidenceMultiPage2 = what.evidence as EvidenceMultiPage;
				if (evidenceMultiPage2 != null && human.trash.Count > 0)
				{
					for (int i = 0; i < human.trash.Count; i++)
					{
						MetaObject metaObject2 = CityData.Instance.FindMetaObject(human.trash[i]);
						if (metaObject2 != null)
						{
							InteractablePreset interactablePreset = null;
							Toolbox.Instance.objectPresetDictionary.TryGetValue(metaObject2.preset, ref interactablePreset);
							if ((interactablePreset.disposal != Human.DisposalType.homeOnly || human.isHome) && (interactablePreset.disposal != Human.DisposalType.workOnly || human.job.employer == null || human.isAtWork) && (interactablePreset.disposal != Human.DisposalType.homeOrWork || human.isHome || human.isAtWork))
							{
								if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
								{
									ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
									if (chapterIntro != null && human == chapterIntro.kidnapper)
									{
										Game.Log(string.Concat(new string[]
										{
											"Chapter: Kidnapper ",
											human.GetCitizenName(),
											": Bin trash ",
											interactablePreset.name,
											" ",
											what.name,
											" ",
											where.room.name,
											" ",
											where.gameLocation.name
										}), 2);
									}
								}
								List<EvidenceMultiPage.MultiPageContent> list2 = evidenceMultiPage2.pageContent.FindAll((EvidenceMultiPage.MultiPageContent item) => item.meta > 0);
								if (list2.Count >= GameplayControls.Instance.binTrashLimit)
								{
									int num3 = 9999999;
									EvidenceMultiPage.MultiPageContent multiPageContent3 = null;
									foreach (EvidenceMultiPage.MultiPageContent multiPageContent4 in list2)
									{
										if (multiPageContent4.page < num3)
										{
											num3 = multiPageContent4.page;
											multiPageContent3 = multiPageContent4;
										}
									}
									if (multiPageContent3 != null)
									{
										MetaObject metaObject3 = CityData.Instance.FindMetaObject(multiPageContent3.meta);
										if (metaObject3 != null)
										{
											metaObject3.Remove();
										}
										evidenceMultiPage2.pageContent.Remove(multiPageContent3);
									}
									evidenceMultiPage2.AddContainedMetaObjectToPage(num3, metaObject2);
								}
								else
								{
									evidenceMultiPage2.AddContainedMetaObjectToNewPage(metaObject2);
									CleanupController.Instance.binTrash++;
								}
								if (interactablePreset.disposal == Human.DisposalType.anywhere)
								{
									human.anywhereTrash--;
								}
								human.trash.RemoveAt(i);
								i--;
							}
						}
						else
						{
							human.trash.RemoveAt(i);
							i--;
						}
					}
					if (human.ai != null)
					{
						human.ai.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
					}
				}
			}
		}
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0006648C File Offset: 0x0006468C
	public void PostJob(Interactable what, NewNode where, Actor who)
	{
		if (who.ai != null && who.ai.currentGoal != null)
		{
			SideJob sideJob = null;
			if (!SideJobController.Instance.allJobsDictionary.TryGetValue(who.ai.currentGoal.jobID, ref sideJob))
			{
				Game.LogError("Jobs: Unable to find job with ID " + who.ai.currentGoal.jobID.ToString(), 2);
				return;
			}
			if (sideJob.post != null)
			{
				Game.LogError("Jobs: Post for job " + sideJob.jobID.ToString() + " already exists! Not posting...", 2);
				return;
			}
			List<FurniturePreset.SubObject> list = new List<FurniturePreset.SubObject>();
			FurnitureLocation furnitureParent = what.furnitureParent;
			if (furnitureParent == null)
			{
				Game.LogError("Jobs: Post job board location is null", 2);
				return;
			}
			for (int i = 0; i < furnitureParent.furniture.subObjects.Count; i++)
			{
				FurniturePreset.SubObject so = furnitureParent.furniture.subObjects[i];
				if (sideJob.preset.jobPosting.subObjectClasses.Contains(so.preset) && !furnitureParent.spawnedInteractables.Exists((Interactable item) => item.subObject == so))
				{
					list.Add(so);
				}
			}
			if (list.Count > 0)
			{
				FurniturePreset.SubObject subObject = list[Toolbox.Instance.Rand(0, list.Count, false)];
				List<Interactable.Passed> list2 = new List<Interactable.Passed>();
				list2.Add(new Interactable.Passed(Interactable.PassedVarType.jobID, (float)sideJob.jobID, null));
				sideJob.post = InteractableCreator.Instance.CreateFurnitureSpawnedInteractable(sideJob.preset.jobPosting, furnitureParent, subObject, sideJob.poster, sideJob.poster, null, list2, null, null, sideJob.preset.startingScenarios[sideJob.startingScenario].dds);
				sideJob.postID = sideJob.post.id;
				Game.Log("Jobs: Job posted manually by citizen at " + furnitureParent.anchorNode.gameLocation.name, 2);
				sideJob.SetJobState(SideJob.JobState.posted, false);
				return;
			}
		}
		else
		{
			Game.LogError("Jobs: Unable to post as current goal or AI for citizen is inactive!", 2);
		}
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x000666A8 File Offset: 0x000648A8
	public void LogOnComputer(Interactable what, NewNode where, Actor who)
	{
		Human human = who as Human;
		if (human != null && !human.isPlayer)
		{
			if (what.controller != null && what.controller.computer != null)
			{
				what.controller.computer.SetLoggedIn(human);
				return;
			}
			what.SetValue((float)human.humanID);
			what.SetLockedState(false, human, true, false);
		}
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x00066718 File Offset: 0x00064918
	public void Sabotage(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			if (GameplayController.Instance.lockPicks > 0)
			{
				InteractionController.Instance.OnSabotage(what);
				return;
			}
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_lockpicks", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.lockpick, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.lockpicks, null, null, null);
		}
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0006677C File Offset: 0x0006497C
	public void DryOff(Interactable what, NewNode where, Actor who)
	{
		if (who != null)
		{
			Human human = who as Human;
			if (human != null)
			{
				human.AddWet(-1f);
				human.AddHeat(0.2f);
			}
		}
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x000667B8 File Offset: 0x000649B8
	public void OpenSyncDisks(Interactable what, NewNode where, Actor who)
	{
		if (!Player.Instance.autoTravelActive || Game.Instance.autoTravelPause)
		{
			SessionData.Instance.PauseGame(true, false, true);
		}
		if (!UpgradesController.Instance.isOpen)
		{
			UpgradesController.Instance.OpenUpgrades(true);
		}
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x000667F8 File Offset: 0x000649F8
	public void CallEnforcers(Interactable what, NewNode where, Actor who)
	{
		NewGameLocation newLocation = who.currentGameLocation;
		if (what != null && what.preset.isPayphone)
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.payphoneMoneyIn, who, where, what.GetWorldPosition(true), null, null, 1f, null, false, null, false);
		}
		if (who != null)
		{
			who.speechController.Speak("2cfd78b6-9980-4f52-b692-d0c2d0dfb4e9", false, true, null, null);
		}
		bool flag = false;
		if (who.ai != null)
		{
			NewAIGoal newAIGoal = who.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.smellDeadBody);
			if (newAIGoal != null && newAIGoal.passedInteractable != null)
			{
				Human human = newAIGoal.passedInteractable.isActor as Human;
				if (human != null && human.death != null && human.death.isDead && !human.death.reported && !human.unreportable)
				{
					newLocation = human.death.GetDeathLocation();
					Human.Death.ReportType passedVar = (Human.Death.ReportType)newAIGoal.passedVar;
					GameplayController.Instance.CallEnforcers(newLocation, false, false);
					human.death.SetReported(who as Human, passedVar);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			for (int i = 0; i < CityData.Instance.deadCitizensDirectory.Count; i++)
			{
				Human human2 = CityData.Instance.deadCitizensDirectory[i];
				if (human2.death != null && !human2.unreportable && human2.death.isDead && !human2.death.reported)
				{
					GameplayController.Instance.CallEnforcers(human2.death.GetDeathLocation(), false, false);
					human2.death.SetReported(who as Human, Human.Death.ReportType.visual);
				}
			}
		}
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x000669CC File Offset: 0x00064BCC
	public void PutUpPoliceTape(Interactable what, NewNode where, Actor who)
	{
		this.bargeDoor = what;
		NewDoor newDoor = this.bargeDoor.objectRef as NewDoor;
		if (newDoor != null)
		{
			newDoor.SetForbidden(true);
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00066A04 File Offset: 0x00064C04
	public void RemovePoliceTape(Interactable what, NewNode where, Actor who)
	{
		this.bargeDoor = what;
		NewDoor newDoor = this.bargeDoor.objectRef as NewDoor;
		if (newDoor != null)
		{
			newDoor.SetForbidden(false);
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x00066A3C File Offset: 0x00064C3C
	public void PutUpStreetCrimeScene(Interactable what, NewNode where, Actor who)
	{
		string text = "Attempting to create street crime scene at ";
		Vector3 position = what.node.position;
		Game.Log(text + position.ToString(), 2);
		InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.streetCrimeScene, null, null, null, what.node.position, Vector3.zero, null, null, "");
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x00066AA4 File Offset: 0x00064CA4
	public void GetCaseForm(Interactable what, NewNode where, Actor who)
	{
		foreach (Case @case in CasePanelController.Instance.activeCases)
		{
			if ((@case.caseType == Case.CaseType.murder || @case.caseType == Case.CaseType.mainStory || @case.caseType == Case.CaseType.retirement) && @case.caseStatus == Case.CaseStatus.handInNotCollected)
			{
				@case.SetStatus(Case.CaseStatus.handInCollected, true);
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Collected Handin", Strings.Casing.asIs, false, false, false, null) + ": " + @case.name, InterfaceControls.Icon.resolve, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
		}
		CasePanelController.Instance.UpdateCloseCaseButton();
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x00066B74 File Offset: 0x00064D74
	public void HandInCase(Interactable what, NewNode where, Actor who)
	{
		if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.caseStatus == Case.CaseStatus.handInCollected && CasePanelController.Instance.activeCase.handInValid)
		{
			Game.Log("Jobs: Hand in case...", 2);
			if (CasePanelController.Instance.activeCase.caseType == Case.CaseType.mainStory || CasePanelController.Instance.activeCase.caseType == Case.CaseType.murder)
			{
				if (!GameplayController.Instance.caseProcessing.ContainsKey(CasePanelController.Instance.activeCase))
				{
					GameplayController.Instance.caseProcessing.Add(CasePanelController.Instance.activeCase, SessionData.Instance.gameTime);
				}
				else
				{
					GameplayController.Instance.caseProcessing[CasePanelController.Instance.activeCase] = SessionData.Instance.gameTime;
				}
				CasePanelController.Instance.activeCase.SetStatus(Case.CaseStatus.submitted, true);
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Case Submitted", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.resolve, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
			else if (CasePanelController.Instance.activeCase.caseType == Case.CaseType.sideJob)
			{
				if (CasePanelController.Instance.activeCase.job != null)
				{
					if (CasePanelController.Instance.activeCase.handIn.Contains(what.id))
					{
						using (List<Objective>.Enumerator enumerator = CasePanelController.Instance.activeCase.currentActiveObjectives.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Objective objective = enumerator.Current;
								foreach (Objective.ObjectiveTrigger objectiveTrigger in objective.queueElement.triggers)
								{
									if (!objectiveTrigger.triggered && objectiveTrigger.triggerType == Objective.ObjectiveTriggerType.submitCase)
									{
										objectiveTrigger.Trigger(false);
									}
								}
							}
							goto IL_3F6;
						}
					}
					using (List<int>.Enumerator enumerator3 = CasePanelController.Instance.activeCase.handIn.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							int num = enumerator3.Current;
							Game.Log("Jobs: Checking for peek interactable " + num.ToString(), 2);
							NewDoor newDoor = null;
							if (CityData.Instance.doorDictionary.TryGetValue(-num, ref newDoor))
							{
								if (what != newDoor.peekInteractable)
								{
									continue;
								}
								Game.Log(string.Concat(new string[]
								{
									"Jobs: Found peek interactable ",
									num.ToString(),
									", checking ",
									CasePanelController.Instance.activeCase.currentActiveObjectives.Count.ToString(),
									" objectives for submit case..."
								}), 2);
								using (List<Objective>.Enumerator enumerator = CasePanelController.Instance.activeCase.currentActiveObjectives.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										Objective objective2 = enumerator.Current;
										foreach (Objective.ObjectiveTrigger objectiveTrigger2 in objective2.queueElement.triggers)
										{
											if (!objectiveTrigger2.triggered && objectiveTrigger2.triggerType == Objective.ObjectiveTriggerType.submitCase)
											{
												objectiveTrigger2.Trigger(false);
											}
										}
									}
									continue;
								}
							}
							Game.Log("Job: Peek interactable " + (-num).ToString() + " was not found in the door dictionary...", 2);
						}
						goto IL_3F6;
					}
				}
				Game.LogError("Active case does not contain job reference!", 2);
			}
			else if (CasePanelController.Instance.activeCase.caseType == Case.CaseType.retirement)
			{
				PopupMessageController.Instance.PopupMessage("Retire", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				PopupMessageController.Instance.OnRightButton += this.RetirementConfirm;
				PopupMessageController.Instance.OnLeftButton += this.RetirementCancel;
			}
			IL_3F6:
			what.UpdateCurrentActions();
		}
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00066FFC File Offset: 0x000651FC
	public void RetirementConfirm()
	{
		PopupMessageController.Instance.OnRightButton -= this.RetirementConfirm;
		PopupMessageController.Instance.OnLeftButton -= this.RetirementCancel;
		Game.Log("Gameplay: Retirement triggered", 2);
		CutSceneController.Instance.PlayCutScene(GameplayControls.Instance.outro);
		if (AchievementsController.Instance != null)
		{
			AchievementsController.Instance.UnlockAchievement("Too Old for This", "retire");
			if (!AchievementsController.Instance.pacifistFlag)
			{
				AchievementsController.Instance.UnlockAchievement("Pacifist", "retire_no_citizen_ko");
			}
			if (!AchievementsController.Instance.notAScratchFlag)
			{
				AchievementsController.Instance.UnlockAchievement("Not a Scratch", "retire_no_player_ko");
			}
		}
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x000670B5 File Offset: 0x000652B5
	public void RetirementCancel()
	{
		PopupMessageController.Instance.OnRightButton -= this.RetirementConfirm;
		PopupMessageController.Instance.OnLeftButton -= this.RetirementCancel;
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x000670E4 File Offset: 0x000652E4
	public void CheckPulse(Interactable what, NewNode where, Actor who)
	{
		Game.Log("Check pulse execution...", 2);
		if (what != null && what.isActor != null)
		{
			if (what.isActor.ai != null && !what.isActor.isDead)
			{
				what.isActor.ai.koTime = SessionData.Instance.gameTime;
				Game.Log(string.Concat(new string[]
				{
					what.isActor.name,
					" KO time SET: ",
					what.isActor.ai.koTime.ToString(),
					" (",
					SessionData.Instance.gameTime.ToString(),
					")"
				}), 2);
			}
			if (what.isActor.isDead && who != null)
			{
				who.speechController.TriggerBark(SpeechController.Bark.examineBody);
			}
		}
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x000671D4 File Offset: 0x000653D4
	public void TakeActiveCodebreaker(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			Interactable pickUpThis = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.codebreaker, Player.Instance, Player.Instance, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
			FirstPersonItemController.Instance.PickUpItem(pickUpThis, false, false, true, true, true);
			what.SafeDelete(true);
		}
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00067250 File Offset: 0x00065450
	public void TakeActiveDoorWedge(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			Interactable pickUpThis = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.doorWedge, Player.Instance, Player.Instance, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
			FirstPersonItemController.Instance.PickUpItem(pickUpThis, false, false, true, true, true);
			NewDoor newDoor = null;
			if (CityData.Instance.doorDictionary.TryGetValue((int)what.val, ref newDoor))
			{
				newDoor.SetJammed(false, what, false);
			}
			what.SafeDelete(true);
		}
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x000672F4 File Offset: 0x000654F4
	public void TakeActiveTracker(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			Interactable pickUpThis = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.tracker, Player.Instance, Player.Instance, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
			FirstPersonItemController.Instance.PickUpItem(pickUpThis, false, false, true, true, true);
			what.SafeDelete(true);
		}
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00067370 File Offset: 0x00065570
	public void TakeActiveFlashBomb(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			Game.Log("Take active flash bomb", 2);
			Interactable pickUpThis = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.flashbomb, Player.Instance, Player.Instance, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
			FirstPersonItemController.Instance.PickUpItem(pickUpThis, false, false, true, true, true);
			what.SafeDelete(true);
		}
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x000673F8 File Offset: 0x000655F8
	public void TakeActiveIncapacitator(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			Interactable pickUpThis = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.incapacitator, Player.Instance, Player.Instance, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
			FirstPersonItemController.Instance.PickUpItem(pickUpThis, false, false, true, true, true);
			what.SafeDelete(true);
		}
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00067474 File Offset: 0x00065674
	public void OpenBreaker(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			Game.Log("Debug: Breaker open", 2);
			what.SetValue(1f);
		}
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0006748F File Offset: 0x0006568F
	public void CloseBreaker(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			Game.Log("Debug: Breaker closed", 2);
			what.SetValue(0f);
		}
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x000674AA File Offset: 0x000656AA
	public void ShootPoolBall(Interactable what, NewNode where, Actor who)
	{
		if (what != null && what.controller != null)
		{
			what.controller.DropThis(true);
		}
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x000674CC File Offset: 0x000656CC
	public void ResetPoolGame(Interactable what, NewNode where, Actor who)
	{
		if (what != null && what.furnitureParent != null)
		{
			foreach (Interactable interactable in what.furnitureParent.spawnedInteractables)
			{
				if (interactable.preset.physicsProfile != null && interactable.preset.physicsProfile.name == "PoolBall")
				{
					Game.Log("Reset pool ball: " + interactable.preset.name, 2);
					interactable.originalPosition = false;
					interactable.SetOriginalPosition(true, true);
				}
			}
		}
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x00002265 File Offset: 0x00000465
	public void PutBack(Interactable what, NewNode where, Actor who)
	{
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00067588 File Offset: 0x00065788
	public void Release(Interactable what, NewNode where, Actor who)
	{
		if (what != null && what.isActor != null && what.isActor.ai != null && what.isActor.ai.restrained)
		{
			what.isActor.ai.SetRestrained(false, 0f);
			int i = 0;
			while (i < what.isActor.inventory.Count)
			{
				Interactable interactable = what.isActor.inventory[i];
				if (interactable.preset.name == "Handcuffs")
				{
					interactable.SetAsNotInventory(Player.Instance.currentNode);
					if (!FirstPersonItemController.Instance.PickUpItem(interactable, false, false, true, true, true))
					{
						interactable.MoveInteractable(Player.Instance.currentNode.position, new Vector3(0f, -Player.Instance.transform.eulerAngles.y, 0f), true);
						interactable.LoadInteractableToWorld(false, true);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0006769C File Offset: 0x0006589C
	public void TakeDetectiveStuff(Interactable what, NewNode where, Actor who)
	{
		Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.printReader, Player.Instance, Player.Instance, null, what.wPos + new Vector3(0f, 3.5f, 0f), what.wEuler, null, null, "");
		if (interactable != null)
		{
			Game.Log("Successfully created newObject " + interactable.name + " id " + interactable.id.ToString(), 2);
			if (FirstPersonItemController.Instance.PickUpItem(interactable, false, false, true, true, true))
			{
				interactable.MarkAsTrash(true, false, 0f);
			}
			else
			{
				Game.LogError("Unable to pickup item " + interactable.name, 2);
				interactable.Delete();
			}
		}
		Interactable interactable2 = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.handcuffs, Player.Instance, Player.Instance, null, what.wPos + new Vector3(0f, 3.5f, 0f), what.wEuler, null, null, "");
		if (interactable2 != null)
		{
			Game.Log("Successfully created newObject " + interactable2.name + " id " + interactable2.id.ToString(), 2);
			if (FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && item.GetInteractable() == null) == null)
			{
				FirstPersonItemController.InventorySlot inventorySlot = null;
				foreach (FirstPersonItemController.InventorySlot inventorySlot2 in FirstPersonItemController.Instance.slots)
				{
					if (inventorySlot2.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && (inventorySlot == null || (inventorySlot2.GetInteractable() != null && inventorySlot2.GetInteractable().val < inventorySlot.GetInteractable().val)))
					{
						inventorySlot = inventorySlot2;
					}
				}
				if (inventorySlot != null)
				{
					FirstPersonItemController.Instance.EmptySlot(inventorySlot, false, false, true, true);
				}
			}
			if (FirstPersonItemController.Instance.PickUpItem(interactable2, false, false, true, true, true))
			{
				interactable2.MarkAsTrash(true, false, 0f);
			}
			else
			{
				Game.LogError("Unable to pickup item " + interactable2.name, 2);
				interactable2.Delete();
			}
		}
		GameplayController.Instance.AddLockpicks(30, true);
		SessionData.Instance.TutorialTrigger("inventory", false);
		SessionData.Instance.TutorialTrigger("lockpicking", false);
		what.Delete();
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x000678FC File Offset: 0x00065AFC
	public void Mugging(Interactable what, NewNode where, Actor who)
	{
		if (what.isActor != null && who != null)
		{
			Human human = who as Human;
			Human human2 = what.isActor as Human;
			if (human != null && human2 != null)
			{
				Game.Log("MUGGING By " + who.name + " to " + what.name, 2);
				if (what.isActor.isPlayer)
				{
					human.ai.TalkTo(InteractionController.ConversationType.mugging);
					EvidenceWitness.DialogOption dialogOption = human.evidenceEntry.dialogOptions[Evidence.DataKey.photo].Find((EvidenceWitness.DialogOption item) => item.preset.name == "Mugging");
					this._InvokeDialog(human.interactable, where, human2, dialogOption, dialogOption.preset, DialogController.ForceSuccess.none);
					return;
				}
				DDSSaveClasses.DDSTreeSave newTree = null;
				if (Toolbox.Instance.allDDSTrees.TryGetValue("e07fe5d5-5195-437e-8776-2798cc53f6a5", ref newTree))
				{
					human.ExecuteConversationTree(newTree, Enumerable.ToList<Human>(new Human[]
					{
						human2
					}));
				}
			}
		}
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00067A08 File Offset: 0x00065C08
	public void DebtCollection(Interactable what, NewNode where, Actor who)
	{
		if (what.isActor != null && who != null)
		{
			Human human = who as Human;
			Human human2 = what.isActor as Human;
			if (human != null && human2 != null)
			{
				Game.Log("DEBT COLLECTION By " + who.name + " to " + what.name, 2);
				if (what.isActor.isPlayer)
				{
					human.ai.TalkTo(InteractionController.ConversationType.loanSharkVisit);
					EvidenceWitness.DialogOption dialogOption = human.evidenceEntry.dialogOptions[Evidence.DataKey.photo].Find((EvidenceWitness.DialogOption item) => item.preset.name == "LoanShark_AssociateVisit");
					this._InvokeDialog(human.interactable, where, human2, dialogOption, dialogOption.preset, DialogController.ForceSuccess.none);
				}
			}
		}
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00067AE3 File Offset: 0x00065CE3
	public void NextTrack(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			what.MusicPlayerNextTrack(1);
		}
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00067AEF File Offset: 0x00065CEF
	public void PreviousTrack(Interactable what, NewNode where, Actor who)
	{
		if (what != null)
		{
			what.MusicPlayerNextTrack(-1);
		}
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00067AFB File Offset: 0x00065CFB
	public void CancelPutDownHomeInventoryItem(Interactable what, NewNode where, Actor who)
	{
		Game.Log("Decor: Cancel item placement", 2);
		if (what != null)
		{
			what.SetPhysicsPickupState(false, null, true, false);
			PlayerApartmentController.Instance.MoveItemToStorage(what);
		}
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00067B20 File Offset: 0x00065D20
	public void RotatePhysicsLeft(Interactable what, NewNode where, Actor who)
	{
		if (InteractionController.Instance.carryingObject != null)
		{
			InteractionController.Instance.carryingObject.RotateHeldObject(-45f);
		}
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00067B48 File Offset: 0x00065D48
	public void RotatePhysicsRight(Interactable what, NewNode where, Actor who)
	{
		if (InteractionController.Instance.carryingObject != null)
		{
			InteractionController.Instance.carryingObject.RotateHeldObject(45f);
		}
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00067B70 File Offset: 0x00065D70
	public void Drink(Interactable what, NewNode where, Actor who)
	{
		if (who.isPlayer)
		{
			Player.Instance.OnDrink(what);
		}
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00067B85 File Offset: 0x00065D85
	public void LoiteringConfront(Interactable what, NewNode where, Actor who)
	{
		Game.Log("Loitering confront", 2);
		if (Player.Instance.spendingTimeMode)
		{
			Game.Log("Cancel spending time mode", 2);
			Player.Instance.SetSpendingTimeMode(false);
		}
	}

	// Token: 0x040006B7 RID: 1719
	public List<AIActionPreset> allActions = new List<AIActionPreset>();

	// Token: 0x040006B8 RID: 1720
	private Dictionary<AIActionPreset, MethodInfo> actionRef = new Dictionary<AIActionPreset, MethodInfo>();

	// Token: 0x040006B9 RID: 1721
	[NonSerialized]
	private Interactable bargeDoor;

	// Token: 0x040006BB RID: 1723
	private static ActionController _instance;

	// Token: 0x020000E6 RID: 230
	// (Invoke) Token: 0x060006A6 RID: 1702
	public delegate void PlayerAction(AIActionPreset action, Interactable what, NewNode where, Actor who);
}
