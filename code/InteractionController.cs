using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class InteractionController : MonoBehaviour
{
	// Token: 0x17000087 RID: 135
	// (get) Token: 0x060010E8 RID: 4328 RVA: 0x000EF654 File Offset: 0x000ED854
	// (set) Token: 0x060010E9 RID: 4329 RVA: 0x000EF65C File Offset: 0x000ED85C
	public float interactionActionAmount { get; private set; }

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x060010EB RID: 4331 RVA: 0x000EF66E File Offset: 0x000ED86E
	// (set) Token: 0x060010EA RID: 4330 RVA: 0x000EF665 File Offset: 0x000ED865
	public Transform interactionActionLookAt { get; private set; }

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x060010EC RID: 4332 RVA: 0x000EF678 File Offset: 0x000ED878
	// (remove) Token: 0x060010ED RID: 4333 RVA: 0x000EF6B0 File Offset: 0x000ED8B0
	public event InteractionController.ReturnFromLockedIn OnReturnFromLockedIn;

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x060010EE RID: 4334 RVA: 0x000EF6E8 File Offset: 0x000ED8E8
	// (remove) Token: 0x060010EF RID: 4335 RVA: 0x000EF720 File Offset: 0x000ED920
	public event InteractionController.InteractionActionCompleted OnInteractionActionCompleted;

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x060010F0 RID: 4336 RVA: 0x000EF758 File Offset: 0x000ED958
	// (remove) Token: 0x060010F1 RID: 4337 RVA: 0x000EF790 File Offset: 0x000ED990
	public event InteractionController.InteractionActionProgressChange OnInteractionActionProgressChange;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x060010F2 RID: 4338 RVA: 0x000EF7C8 File Offset: 0x000ED9C8
	// (remove) Token: 0x060010F3 RID: 4339 RVA: 0x000EF800 File Offset: 0x000EDA00
	public event InteractionController.InteractionActionLookedAway OnInteractionActionLookedAway;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x060010F4 RID: 4340 RVA: 0x000EF838 File Offset: 0x000EDA38
	// (remove) Token: 0x060010F5 RID: 4341 RVA: 0x000EF870 File Offset: 0x000EDA70
	public event InteractionController.InteractionActionCancelled OnInteractionActionCancelled;

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x060010F6 RID: 4342 RVA: 0x000EF8A5 File Offset: 0x000EDAA5
	public static InteractionController Instance
	{
		get
		{
			return InteractionController._instance;
		}
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x000EF8AC File Offset: 0x000EDAAC
	private void Awake()
	{
		if (InteractionController._instance != null && InteractionController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		InteractionController._instance = this;
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x000EF8DC File Offset: 0x000EDADC
	private void Start()
	{
		this.DisplayInteractionCursor(false, true);
		this.allInteractionKeys = Enumerable.ToList<InteractablePreset.InteractionKey>(Enumerable.Cast<InteractablePreset.InteractionKey>(Enum.GetValues(typeof(InteractablePreset.InteractionKey))));
		foreach (InteractablePreset.InteractionKey interactionKey in this.allInteractionKeys)
		{
			this.currentInteractions.Add(interactionKey, new InteractionController.InteractionSetting());
		}
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x000EF960 File Offset: 0x000EDB60
	private void Update()
	{
		if (SessionData.Instance.play && !CutSceneController.Instance.cutSceneActive)
		{
			if (this.carryingObject == null && !this.interactionMode)
			{
				this.InteractionRaycastCheck();
			}
			if (this.activeInteractionAction)
			{
				this.activeInteractionActionLookCheck = false;
				if (this.interactionActionLookAt != null)
				{
					if (Physics.Raycast(new Ray(CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.forward), ref this.playerCurrentRaycastHit, 10f, Toolbox.Instance.interactionRayLayerMask))
					{
						if (this.playerCurrentRaycastHit.transform == this.interactionActionLookAt)
						{
							this.activeInteractionActionLookCheck = true;
							this.canFailLookCheck = true;
							this.lastLookAtForInteraction = SessionData.Instance.gameTime;
							Game.Log("Player: Interaction action raycast succeed: " + this.playerCurrentRaycastHit.transform.name, 2);
						}
						else
						{
							if (this.canFailLookCheck)
							{
								this.canFailLookCheck = false;
								InteractionController.InteractionActionLookedAway onInteractionActionLookedAway = this.OnInteractionActionLookedAway;
								if (onInteractionActionLookedAway != null)
								{
									onInteractionActionLookedAway();
								}
							}
							Game.Log("Player: Interaction action raycast fail: " + this.playerCurrentRaycastHit.transform.name, 2);
						}
					}
					else
					{
						Game.Log("Player: Interaction action raycast fail: no hit", 2);
					}
					if (!this.activeInteractionActionLookCheck && SessionData.Instance.gameTime > this.lastLookAtForInteraction + 0.03f)
					{
						this.lastLookAtForInteraction = SessionData.Instance.gameTime;
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Look at the target object to progress the action", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.eye, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					}
				}
				else
				{
					this.activeInteractionActionLookCheck = true;
				}
				if (Player.Instance.isLockpicking && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.triggerIllegalOnPick) > 0f)
				{
					InteractionController.Instance.SetIllegalActionActive(this.activeInteractionActionLookCheck);
					Player.Instance.illegalActionTimer = 0.01f;
				}
				if (this.lockedInInteraction != null && this.lockedInInteraction.controller != null)
				{
					if (this.cancelInteractionIfOutOfRange && this.lockedInInteraction.controller.coll != null)
					{
						float num = Vector3.Distance(CameraController.Instance.cam.transform.position, this.lockedInInteraction.controller.coll.bounds.center);
						float num2 = this.lockedInInteraction.GetReachDistance() + 2.75f;
						if (num > num2)
						{
							Game.Log(string.Concat(new string[]
							{
								"Player: Cancelled locked in interaction because of distance (",
								num.ToString(),
								"/",
								num2.ToString(),
								")"
							}), 2);
							this.activeInteractionActionLookCheck = false;
							InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
						}
					}
					else if (this.lockedInInteraction.controller.coll == null)
					{
						Game.Log(string.Concat(new string[]
						{
							"Player: Missing collider for locked in interaction on object: ",
							this.lockedInInteraction.controller.name,
							" (",
							this.lockedInInteraction.id.ToString(),
							")"
						}), 2);
					}
				}
				else
				{
					Game.Log("Player: Cancelled locked in interaction because interactable isn't spawned", 2);
					this.activeInteractionActionLookCheck = false;
					InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
				}
			}
			bool flag = false;
			for (int i = 0; i < ControlsDisplayController.Instance.customActionsDisplayed.Count; i++)
			{
				ControlsDisplayController.CustomActionsDisplayed customActionsDisplayed = ControlsDisplayController.Instance.customActionsDisplayed[i];
				if (customActionsDisplayed.displayTime > 0f)
				{
					customActionsDisplayed.displayTime -= Time.deltaTime;
					if (customActionsDisplayed.displayTime <= 0f)
					{
						customActionsDisplayed.displayTime = 0f;
						this.SetCurrentPlayerInteraction(customActionsDisplayed.key, null, null, false, -1);
						flag = true;
					}
				}
				else if (SessionData.Instance.gameTime >= customActionsDisplayed.lastDisplayedAt + ControlsDisplayController.Instance.minimumCustomControlDisplayTimeInterval)
				{
					ControlsDisplayController.Instance.customActionsDisplayed.RemoveAt(i);
					i--;
				}
			}
			if (flag)
			{
				this.UpdateInteractionText();
			}
			if (this.inputCooldown <= 0f && !InterfaceController.Instance.playerTextInputActive && !Player.Instance.transitionActive && (MapController.Instance == null || !MapController.Instance.displayFirstPerson))
			{
				InteractablePreset.InteractionKey interactionKey = InteractablePreset.InteractionKey.none;
				if (InputController.Instance.player.GetButtonDown("Primary"))
				{
					interactionKey = InteractablePreset.InteractionKey.primary;
				}
				else if (InputController.Instance.player.GetButtonDown("Secondary"))
				{
					interactionKey = InteractablePreset.InteractionKey.secondary;
				}
				else if (InputController.Instance.player.GetButtonDown("Alternative"))
				{
					interactionKey = InteractablePreset.InteractionKey.alternative;
				}
				else if (InputController.Instance.player.GetButtonDown("ScrollAxisUp"))
				{
					interactionKey = InteractablePreset.InteractionKey.scrollAxisUp;
				}
				else if (InputController.Instance.player.GetButtonDown("ScrollAxisDown"))
				{
					interactionKey = InteractablePreset.InteractionKey.scrollAxisDown;
				}
				else if (InputController.Instance.player.GetButtonDown("NavigateUp"))
				{
					interactionKey = InteractablePreset.InteractionKey.SelectUp;
				}
				else if (InputController.Instance.player.GetButtonDown("NavigateDown"))
				{
					interactionKey = InteractablePreset.InteractionKey.SelectDown;
				}
				else if (InputController.Instance.player.GetButtonDown("LeanLeft"))
				{
					interactionKey = InteractablePreset.InteractionKey.LeanLeft;
				}
				else if (InputController.Instance.player.GetButtonDown("LeanRight"))
				{
					interactionKey = InteractablePreset.InteractionKey.LeanRight;
				}
				if (interactionKey != InteractablePreset.InteractionKey.none && !BioScreenController.Instance.isOpen)
				{
					InteractionController.InteractionSetting interactionSetting = this.currentInteractions[interactionKey];
					if (interactionSetting.currentAction != null && interactionSetting.currentSetting.enabled)
					{
						if (interactionSetting.newUIRef != null)
						{
							interactionSetting.newUIRef.Execute();
						}
						if (interactionSetting.isFPSItem)
						{
							FirstPersonItemController.Instance.OnInteraction(interactionKey);
						}
						else
						{
							interactionSetting.interactable.OnInteraction(interactionKey, Player.Instance);
						}
						this.inputCooldown = 0.1f;
					}
				}
			}
			if (this.lockedInInteraction != null)
			{
				if (this.activeInteractionAction && this.activeInteractionActionLookCheck)
				{
					if (this.interactionActionAmount < this.interactionActionThreshold)
					{
						float num3 = Time.deltaTime * this.interactionActionMultiplier * Mathf.LerpUnclamped(GameplayControls.Instance.lockpickSpeedRange.x, GameplayControls.Instance.lockpickSpeedRange.y, UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.lockpickingSpeedModifier));
						this.interactionActionAmount += num3;
						this.interactionActionAmount = Mathf.Min(this.interactionActionAmount, this.interactionActionThreshold);
						if (this.discoveryOverTime.Count > 0)
						{
							List<Interactable> list = new List<Interactable>();
							foreach (KeyValuePair<Interactable, float> keyValuePair in this.discoveryOverTime)
							{
								if (this.interactionActionAmount >= keyValuePair.Value)
								{
									ActionController.Instance.Inspect(keyValuePair.Key, Player.Instance.currentNode, Player.Instance);
									list.Add(keyValuePair.Key);
								}
							}
							foreach (Interactable interactable in list)
							{
								this.discoveryOverTime.Remove(interactable);
							}
						}
						if (this.discoveryOverTimeDiscovery.Count > 0)
						{
							List<EvidenceMultiPage.MultiPageContent> list2 = new List<EvidenceMultiPage.MultiPageContent>();
							foreach (KeyValuePair<EvidenceMultiPage.MultiPageContent, float> keyValuePair2 in this.discoveryOverTimeDiscovery)
							{
								if (this.interactionActionAmount >= keyValuePair2.Value)
								{
									Evidence evidence = null;
									if (GameplayController.Instance.evidenceDictionary.TryGetValue(keyValuePair2.Key.discEvID, ref evidence))
									{
										evidence.AddDiscovery(keyValuePair2.Key.disc);
									}
									else
									{
										Game.LogError("Unable to find evidence " + keyValuePair2.Key.discEvID, 2);
									}
									list2.Add(keyValuePair2.Key);
								}
							}
							foreach (EvidenceMultiPage.MultiPageContent multiPageContent in list2)
							{
								this.discoveryOverTimeDiscovery.Remove(multiPageContent);
							}
						}
						List<Evidence> list3 = new List<Evidence>();
						foreach (KeyValuePair<Evidence, float> keyValuePair3 in this.discoveryOverTimeEvidence)
						{
							if (this.interactionActionAmount >= keyValuePair3.Value)
							{
								SessionData.Instance.PauseGame(true, false, true);
								Game.Log("Player: Find evidence " + keyValuePair3.Key.preset.name, 2);
								if (keyValuePair3.Key.interactablePreset != null && keyValuePair3.Key.interactablePreset.retailItem != null && (keyValuePair3.Key.interactablePreset.retailItem.nourishment != 0f || keyValuePair3.Key.interactablePreset.retailItem.hydration != 0f))
								{
									Player.Instance.AddHygiene(Toolbox.Instance.Rand(-0.15f, -0.25f, false));
								}
								InterfaceController.Instance.SpawnWindow(keyValuePair3.Key, Evidence.DataKey.photo, null, "", true, false, new Vector2(0.5f, 0.5f), null, null, null, true);
								list3.Add(keyValuePair3.Key);
							}
						}
						foreach (Evidence evidence2 in list3)
						{
							this.discoveryOverTimeEvidence.Remove(evidence2);
						}
						List<MetaObject> list4 = new List<MetaObject>();
						foreach (KeyValuePair<MetaObject, float> keyValuePair4 in this.discoveryOverTimeMeta)
						{
							if (this.interactionActionAmount >= keyValuePair4.Value)
							{
								Evidence evidence3 = keyValuePair4.Key.GetEvidence(true, Player.Instance.currentNodeCoord);
								if (evidence3 != null)
								{
									SessionData.Instance.PauseGame(true, false, true);
									InteractablePreset preset = keyValuePair4.Key.GetPreset();
									if (preset != null && preset.retailItem != null && (preset.retailItem.nourishment != 0f || preset.retailItem.hydration != 0f))
									{
										Player.Instance.AddHygiene(Toolbox.Instance.Rand(-0.15f, -0.25f, false));
									}
									Game.Log("Player: Find evidence " + keyValuePair4.Key.preset, 2);
									InterfaceController.Instance.SpawnWindow(evidence3, Evidence.DataKey.photo, null, "", true, false, new Vector2(0.5f, 0.5f), null, null, null, true);
								}
								else
								{
									Game.Log("Player: Unable to get evidence for meta " + keyValuePair4.Key.preset, 2);
								}
								list4.Add(keyValuePair4.Key);
							}
						}
						foreach (MetaObject metaObject in list4)
						{
							this.discoveryOverTimeMeta.Remove(metaObject);
						}
						if (!Player.Instance.playerKOInProgress)
						{
							float num4 = 0f;
							for (int j = 0; j < this.spawnedProgressControllers.Count; j++)
							{
								LockpickProgressController lockpickProgressController = this.spawnedProgressControllers[j];
								lockpickProgressController.SetAmount(this.interactionActionAmount - num4);
								num4 += lockpickProgressController.barMax;
							}
						}
						Player.Instance.OnInteractionActionProgress(num3, this.interactionActionAmount);
						if (this.OnInteractionActionProgressChange != null)
						{
							this.OnInteractionActionProgressChange(num3, this.interactionActionAmount);
						}
					}
					else
					{
						this.CompleteInteractionAction();
					}
				}
			}
			else if (Player.Instance.illegalActionTimer > 0f)
			{
				Player.Instance.illegalActionTimer -= Time.deltaTime;
				if (Player.Instance.illegalActionTimer <= 0f)
				{
					this.SetIllegalActionActive(false);
				}
			}
			if (this.dialogMode || (!this.dialogMode && this.dialogTransition > 0f))
			{
				if (this.dialogMode && this.dialogTransition < 1f)
				{
					this.dialogTransition += 2f * Time.deltaTime;
				}
				else if (!this.dialogMode && this.dialogTransition > 0f)
				{
					this.dialogTransition -= 3f * Time.deltaTime;
				}
				this.dialogTransition = Mathf.Clamp01(this.dialogTransition);
				PrefabControls.Instance.dialogRect.anchoredPosition = new Vector2(Mathf.Lerp(-1600f, 0f, this.dialogTransition), 116f);
				for (int k = 0; k < this.dialogOptions.Count; k++)
				{
					DialogButtonController dialogButtonController = this.dialogOptions[k];
					if (k != this.dialogSelection)
					{
						dialogButtonController.SetForceAdditionalHighlight(false);
					}
					else
					{
						dialogButtonController.SetForceAdditionalHighlight(true);
					}
				}
			}
			if (this.displayingInteraction)
			{
				if (this.displayProgress < 1f)
				{
					this.displayProgress += 2.5f * Time.deltaTime;
					this.displayProgress = Mathf.Clamp01(this.displayProgress);
					for (int l = 0; l < InterfaceControls.Instance.interactionFadeInImages.Count; l++)
					{
						InterfaceControls.Instance.interactionFadeInImages[l].canvasRenderer.SetAlpha(this.displayProgress);
					}
					InterfaceControls.Instance.interactionText.alpha = this.displayProgress;
				}
				Vector2 zero = Vector2.zero;
				Vector2 zero2 = Vector2.zero;
				this.currentLookingAtInteractable.GetScreenBox(out zero, out zero2);
				InterfaceControls.Instance.interactionRect.localPosition = Vector2.Lerp(zero, zero2, 0.5f);
				Vector2 vector = (zero2 - zero) * 0.9f;
				vector = Vector2.Max(vector, InterfaceControls.Instance.interactionCursorMin);
				vector = Vector2.Min(vector, InterfaceControls.Instance.interactionCursorMax);
				InterfaceControls.Instance.interactionRect.sizeDelta = vector;
				if (InteractionController.Instance.inOut && InteractionController.Instance.inOutProgress < 1f)
				{
					InteractionController.Instance.inOutProgress += InterfaceControls.Instance.interactionCursorSpeed * InteractionController.Instance.interactionAnimationModifier * Time.deltaTime;
				}
				else if (!InteractionController.Instance.inOut && InteractionController.Instance.inOutProgress > 0f)
				{
					InteractionController.Instance.inOutProgress -= InterfaceControls.Instance.interactionCursorSpeed * InteractionController.Instance.interactionAnimationModifier * Time.deltaTime;
				}
				InteractionController.Instance.inOutProgress = Mathf.Clamp01(InteractionController.Instance.inOutProgress);
				if (InteractionController.Instance.inOutProgress >= 1f && InteractionController.Instance.inOut)
				{
					InteractionController.Instance.inOut = false;
				}
				else if (InteractionController.Instance.inOutProgress <= 0f && !InteractionController.Instance.inOut)
				{
					InteractionController.Instance.inOut = true;
				}
				float num5 = Mathf.Lerp(0f, 16f, InteractionController.Instance.inOutProgress);
				InterfaceControls.Instance.interactionULRect.anchoredPosition = new Vector2(num5, -num5);
				InterfaceControls.Instance.interactionURRect.anchoredPosition = new Vector2(-num5, -32f - num5);
				InterfaceControls.Instance.interactionBLRect.anchoredPosition = new Vector2(32f + num5, num5);
				InterfaceControls.Instance.interactionBRRect.anchoredPosition = new Vector2(-32f - num5, 32f + num5);
				if (InterfaceControls.Instance.interactionTextContainer.sizeDelta.x < vector.x - 32f)
				{
					InterfaceControls.Instance.interactionTextContainer.anchoredPosition = new Vector2(0f, -24f);
				}
				else
				{
					InterfaceControls.Instance.interactionTextContainer.anchoredPosition = new Vector2(0f, -vector.y - 5f);
				}
				if (InterfaceControls.Instance.readingTextContainer != null)
				{
					InterfaceControls.Instance.readingTextContainer.anchoredPosition = new Vector2(0f, -vector.y - 46f);
				}
			}
			else if (this.displayProgress > 0f)
			{
				this.displayProgress -= 3f * Time.deltaTime;
				this.displayProgress = Mathf.Clamp01(this.displayProgress);
			}
		}
		if (this.inputCooldown > 0f)
		{
			this.inputCooldown -= Time.deltaTime;
			this.inputCooldown = Mathf.Max(0f, this.inputCooldown);
		}
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x000F0B38 File Offset: 0x000EED38
	public void StartDecorEdit()
	{
		Game.Log("Decor: Starting decor edit...", 2);
		CasePanelController.Instance.SelectNoCaseButton();
		SessionData.Instance.isDecorEdit = true;
		SessionData.Instance.PauseGame(true, false, false);
		InterfaceController.Instance.SetDesktopMode(true, false);
		InterfaceController.Instance.UpdateDOF();
		if (!InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => item.preset.name == "ApartmentDecor"))
		{
			InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "ApartmentDecor", false, true, default(Vector2), null, null, null, true);
		}
		BioScreenController.Instance.SetInventoryOpen(false, true, false);
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x000F0BE8 File Offset: 0x000EEDE8
	public void SetCurrentPlayerInteraction(InteractablePreset.InteractionKey key, Interactable newInteractable, Interactable.InteractableCurrentAction newCurrentAction, bool fpsItem = false, int forcePriority = -1)
	{
		InteractionController.InteractionSetting interactionSetting = null;
		if (this.currentInteractions.TryGetValue(key, ref interactionSetting))
		{
			interactionSetting.interactable = newInteractable;
			interactionSetting.currentSetting = newCurrentAction;
			if (newCurrentAction != null)
			{
				interactionSetting.currentAction = newCurrentAction.currentAction;
				if (interactionSetting.currentAction != null && interactionSetting.currentAction.action != null)
				{
					interactionSetting.priority = interactionSetting.currentAction.action.inputPriority;
				}
				if (forcePriority >= 0)
				{
					interactionSetting.priority = forcePriority;
				}
				interactionSetting.isFPSItem = fpsItem;
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = "<color=#" + InterfaceControls.Instance.interactionControlTextNormalHex + ">";
				string empty = string.Empty;
				bool flag = false;
				string key2 = string.Empty;
				if (newCurrentAction.currentAction != null)
				{
					if (newCurrentAction.currentAction.action.debug)
					{
						try
						{
							Game.Log(string.Concat(new string[]
							{
								"Debug: Adding action ",
								newCurrentAction.currentAction.action.name,
								" to interactable ",
								newInteractable.name,
								" for key ",
								key.ToString()
							}), 2);
						}
						catch
						{
						}
					}
					if (interactionSetting.isFPSItem)
					{
						FirstPersonItem.FPSInteractionAction fpsinteractionAction = interactionSetting.currentAction as FirstPersonItem.FPSInteractionAction;
						if (fpsinteractionAction.actionIsIllegal)
						{
							flag = this.GetValidPlayerActionIllegal(newInteractable, Player.Instance.currentNode, true, true);
						}
						key2 = fpsinteractionAction.interactionName;
						if (fpsinteractionAction.mainUseSpecialColour)
						{
							text3 = "<color=#" + ColorUtility.ToHtmlStringRGBA(fpsinteractionAction.mainSpecialColour) + ">";
						}
					}
					else
					{
						if (newCurrentAction.currentAction.actionIsIllegal)
						{
							if (newInteractable != null)
							{
								NewNode newNode = newInteractable.node;
								if (newNode == null)
								{
									newNode = Player.Instance.currentNode;
								}
								flag = this.GetValidPlayerActionIllegal(newInteractable, newNode, true, true);
							}
							else
							{
								flag = true;
							}
						}
						key2 = newCurrentAction.currentAction.interactionName;
					}
					if (interactionSetting.currentAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.takeSwap)
					{
						if (!FirstPersonItemController.Instance.IsSlotAvailable() && BioScreenController.Instance.selectedSlot != null && BioScreenController.Instance.selectedSlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
						{
							key2 = "Swap";
						}
					}
					else if (interactionSetting.currentAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.citizenReturn && this.talkingTo == newInteractable)
					{
						key2 = "End Conversation";
					}
					if (!newCurrentAction.enabled)
					{
						text = "<s>";
						text2 = "</s>";
					}
				}
				if (newCurrentAction.overrideInteractionName != null && newCurrentAction.overrideInteractionName.Length > 0)
				{
					key2 = newCurrentAction.overrideInteractionName;
				}
				if (flag)
				{
					text3 = "<color=#" + InterfaceControls.Instance.interactionControlTextIllegalHex + ">";
				}
				this.currentInteractions[key].actionText = string.Concat(new string[]
				{
					text3,
					text,
					Strings.Get("ui.interaction", key2, Strings.Casing.asIs, false, false, false, null),
					text2,
					empty
				});
				if (newInteractable != null && newCurrentAction != null && newCurrentAction.currentAction != null && !newInteractable.actionAudioEventOverrides.TryGetValue(newCurrentAction.currentAction.action, ref this.currentInteractions[key].audioEvent))
				{
					this.currentInteractions[key].audioEvent = newCurrentAction.currentAction.soundEvent;
				}
				if (this.currentInteractions[key].newUIRef != null && this.currentInteractions[key].newUIRef.soundIndicator != null)
				{
					this.currentInteractions[key].newUIRef.soundIndicator.SetSoundEvent(this.currentInteractions[key].audioEvent, true);
					return;
				}
			}
			else
			{
				interactionSetting.currentAction = null;
				this.currentInteractions[key].audioEvent = null;
				if (this.currentInteractions[key].newUIRef != null)
				{
					if (this.currentInteractions[key].newUIRef != null && this.currentInteractions[key].newUIRef.soundIndicator != null)
					{
						this.currentInteractions[key].newUIRef.soundIndicator.SetSoundEvent(null, true);
					}
					this.currentInteractions[key].actionText = string.Empty;
					return;
				}
			}
		}
		else
		{
			Game.LogError("Unable to get key " + key.ToString() + " in current interactions...", 2);
		}
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x000F1048 File Offset: 0x000EF248
	public void DisplayInteractionCursor(bool val, bool forceUpdate = false)
	{
		if (this.displayingInteraction != val || forceUpdate)
		{
			if (this.displayingInteraction && this.displayingInteraction != val)
			{
				this.inOut = false;
				this.inOutProgress = 0f;
				this.displayProgress = 0f;
			}
			this.displayingInteraction = val;
			if (this.displayingInteraction)
			{
				this.currentLookingAtReadingRange = this.currentLookingAtInteractable;
				if (this.currentLookingAtInteractable != null)
				{
					if (this.currentLookingAtInteractable.isDoor != null)
					{
						this.currentLookingAtInteractable.isDoor.UpdateNameBasedOnPlayerPosition();
					}
					this.currentLookingAtInteractable.interactable.UpdateCurrentActions();
				}
				this.UpdateInteractionText();
				InterfaceControls.Instance.interactionRect.gameObject.SetActive(true);
				if (InterfaceControls.Instance.seenIcon.gameObject.activeSelf)
				{
					InterfaceControls.Instance.seenIcon.gameObject.SetActive(false);
				}
				if (this.currentLookingAtInteractable.isDoor != null)
				{
					if (SessionData.Instance.enableTutorialText && (ChapterController.Instance == null || ChapterController.Instance.currentPart >= 22))
					{
						SessionData.Instance.TutorialTrigger("doors", false);
					}
					if (Player.Instance.keyring.Contains(this.currentLookingAtInteractable.isDoor) || Player.Instance.playerKeyringInt.Contains(this.currentLookingAtInteractable.interactable))
					{
						InterfaceControls.Instance.haveKeyIcon.gameObject.SetActive(true);
					}
					else
					{
						InterfaceControls.Instance.haveKeyIcon.gameObject.SetActive(false);
					}
					if (this.currentLookingAtInteractable.isDoor.knowLockStatus)
					{
						if (this.currentLookingAtInteractable.isDoor.isLocked)
						{
							InterfaceControls.Instance.lockedImg.sprite = InterfaceControls.Instance.lockedSprite;
						}
						else
						{
							InterfaceControls.Instance.lockedImg.sprite = InterfaceControls.Instance.unlockedSprite;
						}
						InterfaceControls.Instance.lockedIcon.gameObject.SetActive(true);
						InterfaceControls.Instance.lockStrengthText.text = string.Empty;
						if (this.currentLookingAtInteractable.isDoor.preset.lockType == DoorPreset.LockType.key)
						{
							int lockpicksNeeded = Toolbox.Instance.GetLockpicksNeeded(this.currentLookingAtInteractable.isDoor.wall.currentLockStrength);
							InterfaceControls.Instance.lockStrengthText.text = string.Concat(new string[]
							{
								Strings.Get("ui.interface", "Lock", Strings.Casing.asIs, false, false, false, null),
								": ",
								Mathf.CeilToInt(this.currentLookingAtInteractable.isDoor.wall.currentLockStrength * 100f).ToString(),
								"% (",
								lockpicksNeeded.ToString(),
								" ",
								Strings.Get("ui.interface", "picks", Strings.Casing.asIs, false, false, false, null),
								")\n"
							});
						}
						TextMeshProUGUI lockStrengthText = InterfaceControls.Instance.lockStrengthText;
						lockStrengthText.text = string.Concat(new string[]
						{
							lockStrengthText.text,
							Strings.Get("ui.interface", "Strength", Strings.Casing.asIs, false, false, false, null),
							": ",
							Mathf.CeilToInt(this.currentLookingAtInteractable.isDoor.wall.currentDoorStrength * 100f).ToString(),
							"%"
						});
					}
					else
					{
						InterfaceControls.Instance.lockedIcon.gameObject.SetActive(false);
					}
				}
				else if (this.currentLookingAtInteractable.interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.padlock)
				{
					if (this.currentLookingAtInteractable.interactable.locked)
					{
						InterfaceControls.Instance.lockedImg.sprite = InterfaceControls.Instance.lockedSprite;
					}
					else
					{
						InterfaceControls.Instance.lockedImg.sprite = InterfaceControls.Instance.unlockedSprite;
					}
					InterfaceControls.Instance.lockedIcon.gameObject.SetActive(true);
					InterfaceControls.Instance.lockStrengthText.text = string.Empty;
					int lockpicksNeeded2 = Toolbox.Instance.GetLockpicksNeeded(this.currentLookingAtInteractable.interactable.val);
					InterfaceControls.Instance.lockStrengthText.text = string.Concat(new string[]
					{
						Strings.Get("ui.interface", "Lock", Strings.Casing.asIs, false, false, false, null),
						": ",
						Mathf.CeilToInt(this.currentLookingAtInteractable.interactable.val * 100f).ToString(),
						"% (",
						lockpicksNeeded2.ToString(),
						" ",
						Strings.Get("ui.interface", "picks", Strings.Casing.asIs, false, false, false, null),
						")\n"
					});
				}
				else
				{
					InterfaceControls.Instance.haveKeyIcon.gameObject.SetActive(false);
					InterfaceControls.Instance.lockedIcon.gameObject.SetActive(false);
				}
				if (SessionData.Instance.enableTutorialText && (ChapterController.Instance == null || ChapterController.Instance.currentPart > 1))
				{
					SessionData.Instance.TutorialTrigger("interaction", false);
				}
				this.AlignInteractionIcons();
				return;
			}
			InterfaceControls.Instance.interactionRect.gameObject.SetActive(false);
			this.UpdateInteractionText();
			InterfaceControls.Instance.haveKeyIcon.gameObject.SetActive(false);
			InterfaceControls.Instance.seenIcon.gameObject.SetActive(false);
			this.SetDistanceRecognitionMode(false);
			this.SetReadingMode(false, false);
			this.currentLookingAtReadingRange = null;
		}
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x000F15E0 File Offset: 0x000EF7E0
	public void AlignInteractionIcons()
	{
		float num = 38f;
		if (InterfaceControls.Instance.haveKeyIcon.gameObject.activeSelf)
		{
			InterfaceControls.Instance.haveKeyIcon.anchoredPosition = new Vector2(num, 0f);
			num += 34f;
		}
		if (InterfaceControls.Instance.lockedIcon.gameObject.activeSelf)
		{
			InterfaceControls.Instance.lockedIcon.anchoredPosition = new Vector2(num, 0f);
			num += 34f;
		}
		if (InterfaceControls.Instance.seenIcon.gameObject.activeSelf)
		{
			InterfaceControls.Instance.seenIcon.anchoredPosition = new Vector2(num, 0f);
			num += 34f;
		}
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x000F169C File Offset: 0x000EF89C
	public void SetDistanceRecognitionMode(bool val)
	{
		this.distanceRecognitionMode = val;
		Color color;
		color..ctor(0.5f, 0.5f, 0.5f, 0.66f);
		if (this.distanceRecognitionMode)
		{
			for (int i = 0; i < InterfaceControls.Instance.interactionBoundImages.Count; i++)
			{
				InterfaceControls.Instance.interactionBoundImages[i].color = color;
			}
			InterfaceControls.Instance.interactionText.color = InterfaceControls.Instance.interactionTextDistanceColour;
			this.interactionAnimationModifier = 0.5f;
			return;
		}
		for (int j = 0; j < InterfaceControls.Instance.interactionBoundImages.Count; j++)
		{
			InterfaceControls.Instance.interactionBoundImages[j].color = InterfaceControls.Instance.interactionTextColour;
		}
		InterfaceControls.Instance.interactionText.color = InterfaceControls.Instance.interactionTextColour;
		this.interactionAnimationModifier = 1f;
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x000F1788 File Offset: 0x000EF988
	public void SetReadingMode(bool val, bool stopImmediately)
	{
		if (this.readingMode != val)
		{
			this.readingMode = val;
			Game.Log("Interface: Set reading mode: " + this.readingMode.ToString(), 2);
			this.UpdateReadingModeText();
			if (this.readingMode)
			{
				if (this.currentLookingAtInteractable.interactable.preset.discoverOnRead && this.currentLookingAtInteractable.interactable.evidence != null)
				{
					this.currentLookingAtInteractable.interactable.evidence.SetFound(true);
				}
				if (this.readingModeCoroutine != null)
				{
					base.StopCoroutine(this.readingModeCoroutine);
				}
				this.readingModeCoroutine = base.StartCoroutine(this.ReadingMode());
				return;
			}
			if (stopImmediately)
			{
				if (this.readingModeCoroutine != null)
				{
					base.StopCoroutine(this.readingModeCoroutine);
				}
				if (InterfaceControls.Instance.readingTextContainer != null)
				{
					InterfaceControls.Instance.readingTextContainer.gameObject.SetActive(false);
				}
				InterfaceControls.Instance.readingText.text = string.Empty;
				this.readingModeTransition = 0f;
			}
		}
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x000F1894 File Offset: 0x000EFA94
	public void UpdateReadingModeText()
	{
		if (!this.readingMode)
		{
			InterfaceControls.Instance.readingText.text = string.Empty;
		}
		if (this.currentLookingAtInteractable != null && this.currentLookingAtInteractable.interactable != null && this.currentLookingAtInteractable.interactable.preset.readingEnabled)
		{
			if (this.currentLookingAtInteractable.interactable.preset.readingSource == InteractablePreset.ReadingModeSource.evidenceNote)
			{
				if (this.currentLookingAtInteractable.interactable.evidence != null)
				{
					InterfaceControls.Instance.readingText.text = this.currentLookingAtInteractable.interactable.evidence.GetNoteComposed(Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[1]), false);
					return;
				}
				InterfaceControls.Instance.readingText.text = string.Empty;
				return;
			}
			else if (this.currentLookingAtInteractable.interactable.preset.readingSource == InteractablePreset.ReadingModeSource.mainEvidenceText)
			{
				if (this.currentLookingAtInteractable.interactable != null)
				{
					InterfaceControls.Instance.readingText.text = Strings.GetMainTextFromInteractable(this.currentLookingAtInteractable.interactable, Strings.LinkSetting.forceNoLinks);
					return;
				}
				InterfaceControls.Instance.readingText.text = string.Empty;
				return;
			}
			else if (this.currentLookingAtInteractable.interactable.preset.readingSource != InteractablePreset.ReadingModeSource.multipageEvidence)
			{
				if (this.currentLookingAtInteractable.interactable.preset.readingSource == InteractablePreset.ReadingModeSource.time)
				{
					InterfaceControls.Instance.readingText.text = SessionData.Instance.GameTimeToClock12String(SessionData.Instance.gameTime, false);
					return;
				}
				if (this.currentLookingAtInteractable.interactable.preset.readingSource == InteractablePreset.ReadingModeSource.bookPreset)
				{
					if (this.currentLookingAtInteractable.interactable.book == null)
					{
						Game.Log("Unable to find book preset for " + this.currentLookingAtInteractable.interactable.name + ": Book serialized reference: " + this.currentLookingAtInteractable.interactable.bo, 2);
						return;
					}
					if (!this.currentLookingAtInteractable.interactable.sw0)
					{
						InterfaceControls.Instance.readingText.text = string.Empty;
						return;
					}
					DDSSaveClasses.DDSMessageSave ddsmessageSave = null;
					if (Toolbox.Instance.allDDSMessages.TryGetValue(this.currentLookingAtInteractable.interactable.book.ddsMessage, ref ddsmessageSave))
					{
						DDSSaveClasses.DDSBlockCondition ddsblockCondition = ddsmessageSave.blocks.Find((DDSSaveClasses.DDSBlockCondition item) => item.alwaysDisplay);
						DDSSaveClasses.DDSBlockCondition ddsblockCondition2 = ddsmessageSave.blocks.Find((DDSSaveClasses.DDSBlockCondition item) => item.group == 1);
						DDSSaveClasses.DDSBlockCondition ddsblockCondition3 = ddsmessageSave.blocks.Find((DDSSaveClasses.DDSBlockCondition item) => item.group == 2);
						string text = string.Concat(new string[]
						{
							"<b>",
							Strings.Get("dds.blocks", ddsblockCondition.blockID, Strings.Casing.asIs, false, true, false, null),
							" — ",
							this.currentLookingAtInteractable.interactable.book.author,
							"</b>"
						});
						if (ddsblockCondition2 != null)
						{
							text = text + "\n<i>" + Strings.Get("dds.blocks", ddsblockCondition2.blockID, Strings.Casing.asIs, false, true, false, null) + "</i>";
						}
						if (this.currentLookingAtInteractable.interactable.book.isSeries)
						{
							List<BookPreset> list = Toolbox.Instance.allBooks.FindAll((BookPreset item) => item.seriesTag == this.currentLookingAtInteractable.interactable.book.seriesTag && item.isSeries);
							text = string.Concat(new string[]
							{
								text,
								" (",
								this.currentLookingAtInteractable.interactable.book.seriesNumber.ToString(),
								"/",
								list.Count.ToString(),
								")\n\n"
							});
						}
						else
						{
							text += "\n\n";
						}
						string text2 = string.Empty;
						if (ddsblockCondition3 != null)
						{
							text2 = Strings.Get("dds.blocks", ddsblockCondition3.blockID, Strings.Casing.asIs, false, true, false, null);
						}
						InterfaceControls.Instance.readingText.SetText(text + text2, true);
						int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.readingSeriesBonus));
						if (num > 0 && !GameplayController.Instance.booksRead.Contains(this.currentLookingAtInteractable.interactable.book.name) && this.currentLookingAtInteractable.interactable.book.isSeries)
						{
							List<BookPreset> list2 = Toolbox.Instance.allBooks.FindAll((BookPreset item) => item.seriesTag == this.currentLookingAtInteractable.interactable.book.seriesTag && item.isSeries);
							bool flag = true;
							foreach (BookPreset bookPreset in list2)
							{
								if (!GameplayController.Instance.booksRead.Contains(bookPreset.name) && bookPreset != this.currentLookingAtInteractable.interactable.book)
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								GameplayController.Instance.AddMoney(num, true, "readingformoney");
							}
						}
						int num2 = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.readingMoney));
						if (num2 > 0 && !GameplayController.Instance.booksRead.Contains(this.currentLookingAtInteractable.interactable.book.name))
						{
							GameplayController.Instance.AddMoney(num2, true, "readingformoney");
							GameplayController.Instance.booksRead.Add(this.currentLookingAtInteractable.interactable.book.name);
							return;
						}
					}
					else if (Game.Instance.printDebug)
					{
						Game.Log("Cannot find DDS message for book " + this.currentLookingAtInteractable.interactable.book.name + ": " + this.currentLookingAtInteractable.interactable.book.ddsMessage, 2);
						return;
					}
				}
				else
				{
					if (this.currentLookingAtInteractable.interactable.preset.readingSource == InteractablePreset.ReadingModeSource.syncDiskPreset)
					{
						string text3 = string.Empty;
						if (this.currentLookingAtInteractable.interactable.syncDisk.mainEffect1 != SyncDiskPreset.Effect.none)
						{
							text3 += Strings.ComposeText(Strings.Get("evidence.syncdisks", this.currentLookingAtInteractable.interactable.syncDisk.mainEffect1Description, Strings.Casing.asIs, false, false, false, null), this.currentLookingAtInteractable.interactable.syncDisk, Strings.LinkSetting.automatic, null, new int[2], false);
						}
						if (this.currentLookingAtInteractable.interactable.syncDisk.mainEffect2 != SyncDiskPreset.Effect.none)
						{
							string[] array = new string[5];
							array[0] = text3;
							array[1] = "\n";
							array[2] = Strings.Get("evidence.syncdisks", "OR", Strings.Casing.asIs, false, false, false, null);
							array[3] = "\n";
							int num3 = 4;
							string input = Strings.Get("evidence.syncdisks", this.currentLookingAtInteractable.interactable.syncDisk.mainEffect2Description, Strings.Casing.asIs, false, false, false, null);
							object syncDisk = this.currentLookingAtInteractable.interactable.syncDisk;
							Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic;
							List<Evidence.DataKey> evidenceKeys = null;
							int[] array2 = new int[2];
							array2[0] = 1;
							array[num3] = Strings.ComposeText(input, syncDisk, linkSetting, evidenceKeys, array2, false);
							text3 = string.Concat(array);
						}
						if (this.currentLookingAtInteractable.interactable.syncDisk.mainEffect3 != SyncDiskPreset.Effect.none)
						{
							string[] array3 = new string[5];
							array3[0] = text3;
							array3[1] = "\n";
							array3[2] = Strings.Get("evidence.syncdisks", "OR", Strings.Casing.asIs, false, false, false, null);
							array3[3] = "\n";
							int num4 = 4;
							string input2 = Strings.Get("evidence.syncdisks", this.currentLookingAtInteractable.interactable.syncDisk.mainEffect3Description, Strings.Casing.asIs, false, false, false, null);
							object syncDisk2 = this.currentLookingAtInteractable.interactable.syncDisk;
							Strings.LinkSetting linkSetting2 = Strings.LinkSetting.automatic;
							List<Evidence.DataKey> evidenceKeys2 = null;
							int[] array4 = new int[2];
							array4[0] = 2;
							array3[num4] = Strings.ComposeText(input2, syncDisk2, linkSetting2, evidenceKeys2, array4, false);
							text3 = string.Concat(array3);
						}
						string text4 = string.Concat(new string[]
						{
							"<b>#",
							this.currentLookingAtInteractable.interactable.syncDisk.syncDiskNumber.ToString(),
							" ",
							Strings.Get("evidence.syncdisks", this.currentLookingAtInteractable.interactable.syncDisk.name, Strings.Casing.asIs, false, false, false, null),
							"</b>\n\n",
							text3
						});
						InterfaceControls.Instance.readingText.SetText(text4, true);
						return;
					}
					if (this.currentLookingAtInteractable.interactable.preset.readingSource == InteractablePreset.ReadingModeSource.kaizenSkillDisplay && this.currentLookingAtInteractable.interactable.isActor != null)
					{
						string text5 = string.Empty;
						Human human = this.currentLookingAtInteractable.interactable.isActor as Human;
						if (human != null)
						{
							if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.singlePerception) > 0f && human.partner == null)
							{
								text5 += Strings.Get("ui.interface", "Single", Strings.Casing.asIs, false, false, false, null);
							}
							if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.wealthPerception) > 0f)
							{
								if (human.inventory.Exists((Interactable item) => item.preset.name == "Wallet" || item.preset.name == "Purse"))
								{
									int num5 = 0;
									foreach (Human.WalletItem walletItem in human.walletItems)
									{
										if (walletItem.itemType == Human.WalletItemType.money)
										{
											num5 += walletItem.money;
										}
									}
									if (text5.Length > 0)
									{
										text5 += "\n";
									}
									text5 = text5 + CityControls.Instance.cityCurrency + num5.ToString();
								}
							}
							InterfaceControls.Instance.readingText.SetText(text5, true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x000F223C File Offset: 0x000F043C
	private IEnumerator ReadingMode()
	{
		InterfaceControls.Instance.readingTextContainer.gameObject.SetActive(true);
		int displayPage = -1;
		InterfaceControls.Instance.readingContainerRend.SetAlpha(this.readingModeTransition);
		InterfaceControls.Instance.readingTextRend.SetAlpha(this.readingModeTransition);
		while (this.readingMode || (!this.readingMode && this.readingModeTransition > 0f))
		{
			if (this.readingMode && this.readingModeTransition < 1f)
			{
				this.readingModeTransition += Time.deltaTime * 3f;
				this.readingModeTransition = Mathf.Clamp01(this.readingModeTransition);
			}
			else if (!this.readingMode && this.readingModeTransition > 0f)
			{
				this.readingModeTransition -= Time.deltaTime * 10f;
				this.readingModeTransition = Mathf.Clamp01(this.readingModeTransition);
			}
			Vector2 sizeDelta;
			sizeDelta..ctor(Mathf.Min(InterfaceControls.Instance.readingText.preferredWidth + 40f, InterfaceControls.Instance.readingBoxMaxSize.x), Mathf.Min(InterfaceControls.Instance.readingText.preferredHeight + 38f, InterfaceControls.Instance.readingBoxMaxSize.y));
			InterfaceControls.Instance.readingTextContainer.sizeDelta = sizeDelta;
			InterfaceControls.Instance.readingContainerRend.SetAlpha(this.readingModeTransition);
			InterfaceControls.Instance.readingTextRend.SetAlpha(this.readingModeTransition);
			if (this.currentLookingAtInteractable != null && this.currentLookingAtInteractable.interactable.preset.readingSource == InteractablePreset.ReadingModeSource.multipageEvidence)
			{
				if (this.currentLookingAtInteractable.interactable.readingDelay > 0f)
				{
					InterfaceControls.Instance.readingText.text = string.Empty;
				}
				else
				{
					EvidenceMultiPage pageEv = this.currentLookingAtInteractable.interactable.evidence as EvidenceMultiPage;
					if (pageEv != null && displayPage != pageEv.page)
					{
						InterfaceControls.Instance.readingText.text = pageEv.GetCurrentPageStringContent();
						displayPage = pageEv.page;
						List<EvidenceMultiPage.MultiPageContent> list = pageEv.pageContent.FindAll((EvidenceMultiPage.MultiPageContent item) => item.page == pageEv.page && item.discEvID != null && item.discEvID.Length > 0);
						Game.Log(string.Concat(new string[]
						{
							"displaying page ",
							displayPage.ToString(),
							": ",
							list.Count.ToString(),
							" content. Reading text: ",
							pageEv.GetCurrentPageStringContent()
						}), 2);
						foreach (EvidenceMultiPage.MultiPageContent multiPageContent in list)
						{
							Evidence evidence = null;
							if (GameplayController.Instance.evidenceDictionary.TryGetValue(multiPageContent.discEvID, ref evidence))
							{
								evidence.AddDiscovery(multiPageContent.disc);
							}
							else
							{
								Game.LogError("Unable to find evidence " + multiPageContent.discEvID, 2);
							}
						}
					}
				}
			}
			if (InterfaceControls.Instance.readingText.text.Length <= 0)
			{
				InterfaceControls.Instance.readingContainerRend.SetAlpha(0f);
			}
			yield return null;
		}
		InterfaceControls.Instance.readingTextContainer.gameObject.SetActive(false);
		this.readingModeTransition = 0f;
		yield break;
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x000F224C File Offset: 0x000F044C
	public void UpdateInteractionText()
	{
		FirstPersonItemController.Instance.UpdateCurrentActions();
		using (List<InteractablePreset.InteractionKey>.Enumerator enumerator = this.allInteractionKeys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				InteractablePreset.InteractionKey key = enumerator.Current;
				if (InterfaceController.Instance.desktopMode)
				{
					if (Game.Instance.displayExtraControlHints && !CutSceneController.Instance.cutSceneActive)
					{
						if (CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.topBar)
						{
							if (key == InteractablePreset.InteractionKey.caseBoard)
							{
								Interactable.InteractableCurrentAction newCurrentAction = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Return",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.nearestInteractable)
							{
								Interactable.InteractableCurrentAction newCurrentAction2 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Cycle Interface",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction2, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.ContentMoveAxisX && CasePanelController.Instance.mapScroll.controlEnabled)
							{
								Interactable.InteractableCurrentAction newCurrentAction3 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Scroll Map",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction3, false, 11);
							}
							else
							{
								this.SetCurrentPlayerInteraction(key, null, null, false, -1);
							}
						}
						else if (CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.caseBoard)
						{
							if (key == InteractablePreset.InteractionKey.caseBoard)
							{
								Interactable.InteractableCurrentAction newCurrentAction4 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Resume",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral,
									highlight = true
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction4, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.nearestInteractable)
							{
								Interactable.InteractableCurrentAction newCurrentAction5 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Cycle Interface",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction5, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.ContentMoveAxisX)
							{
								Interactable.InteractableCurrentAction newCurrentAction6 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Scroll Board",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction6, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.MoveEvidenceAxisX)
							{
								Interactable.InteractableCurrentAction newCurrentAction7 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Move Evidence",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction7, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.CaseBoardZoomAxis)
							{
								Interactable.InteractableCurrentAction newCurrentAction8 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Zoom",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction8, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.secondary)
							{
								Interactable.InteractableCurrentAction newCurrentAction9 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Context Menu",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction9, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.CreateString && (CasePanelController.Instance.selectedPinned != null || InterfaceController.Instance.selectedPinned.Count > 0) && !CasePanelController.Instance.customLinkSelectionMode)
							{
								Interactable.InteractableCurrentAction newCurrentAction10 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Create String",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction10, false, 11);
							}
							else
							{
								this.SetCurrentPlayerInteraction(key, null, null, false, -1);
							}
						}
						else if (CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.windows)
						{
							if (key == InteractablePreset.InteractionKey.caseBoard)
							{
								Interactable.InteractableCurrentAction newCurrentAction11 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Return",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction11, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.nearestInteractable)
							{
								Interactable.InteractableCurrentAction newCurrentAction12 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Cycle Interface",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction12, false, 11);
							}
							else if (key == InteractablePreset.InteractionKey.MoveEvidenceAxisX)
							{
								Interactable.InteractableCurrentAction newCurrentAction13 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Move Window",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction13, false, 11);
							}
							else if (InterfaceController.Instance.activeWindows.Count > 1 && key == InteractablePreset.InteractionKey.SelectLeft)
							{
								Interactable.InteractableCurrentAction newCurrentAction14 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Prev. Window",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction14, false, 11);
							}
							else if (InterfaceController.Instance.activeWindows.Count > 1 && key == InteractablePreset.InteractionKey.SelectRight)
							{
								Interactable.InteractableCurrentAction newCurrentAction15 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Next Window",
									forcePositioning = true,
									forcePosition = ControlDisplayController.ControlPositioning.neutral
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction15, false, 11);
							}
							else
							{
								this.SetCurrentPlayerInteraction(key, null, null, false, -1);
							}
						}
					}
					else
					{
						this.SetCurrentPlayerInteraction(key, null, null, false, -1);
					}
				}
				else if (BioScreenController.Instance.isOpen)
				{
					if (key == InteractablePreset.InteractionKey.primary)
					{
						Interactable.InteractableCurrentAction newCurrentAction16 = new Interactable.InteractableCurrentAction
						{
							display = true,
							enabled = true,
							overrideInteractionName = "Select",
							forcePositioning = true,
							forcePosition = ControlDisplayController.ControlPositioning.neutral
						};
						this.SetCurrentPlayerInteraction(key, null, newCurrentAction16, false, 11);
					}
					else if (key == InteractablePreset.InteractionKey.WeaponSelect)
					{
						Interactable.InteractableCurrentAction newCurrentAction17 = new Interactable.InteractableCurrentAction
						{
							display = true,
							enabled = true,
							overrideInteractionName = "Close",
							forcePositioning = true,
							forcePosition = ControlDisplayController.ControlPositioning.neutral
						};
						this.SetCurrentPlayerInteraction(key, null, newCurrentAction17, false, 11);
					}
					else if (key == InteractablePreset.InteractionKey.secondary && (BioScreenController.Instance.hoveredSlot == null || BioScreenController.Instance.hoveredSlot == BioScreenController.Instance.selectedSlot))
					{
						Interactable.InteractableCurrentAction newCurrentAction18 = new Interactable.InteractableCurrentAction
						{
							display = true,
							enabled = true,
							overrideInteractionName = "Close",
							forcePositioning = true,
							forcePosition = ControlDisplayController.ControlPositioning.neutral
						};
						this.SetCurrentPlayerInteraction(key, null, newCurrentAction18, false, 11);
					}
					else if (key == InteractablePreset.InteractionKey.secondary && BioScreenController.Instance.hoveredSlot != null && BioScreenController.Instance.hoveredSlot != BioScreenController.Instance.selectedSlot)
					{
						Interactable.InteractableCurrentAction newCurrentAction19 = new Interactable.InteractableCurrentAction
						{
							display = true,
							enabled = true,
							overrideInteractionName = "Select & Close",
							forcePositioning = true,
							forcePosition = ControlDisplayController.ControlPositioning.neutral
						};
						this.SetCurrentPlayerInteraction(key, null, newCurrentAction19, false, 11);
					}
					else
					{
						this.SetCurrentPlayerInteraction(key, null, null, false, -1);
					}
				}
				else
				{
					if (Player.Instance.autoTravelActive)
					{
						if (key == InteractablePreset.InteractionKey.secondary)
						{
							Interactable.InteractableCurrentAction newCurrentAction20 = new Interactable.InteractableCurrentAction
							{
								display = true,
								enabled = true,
								overrideInteractionName = "Cancel Auto Travel",
								forcePositioning = false,
								forcePosition = ControlDisplayController.ControlPositioning.right
							};
							this.SetCurrentPlayerInteraction(key, null, newCurrentAction20, false, 11);
							continue;
						}
					}
					else if (this.currentlyDragging != null)
					{
						if (key == InteractablePreset.InteractionKey.primary)
						{
							Interactable.InteractableCurrentAction newCurrentAction21 = new Interactable.InteractableCurrentAction
							{
								display = true,
								enabled = true,
								overrideInteractionName = "Drag"
							};
							this.SetCurrentPlayerInteraction(key, null, newCurrentAction21, false, -1);
							continue;
						}
						continue;
					}
					ControlsDisplayController.CustomActionsDisplayed customActionsDisplayed = ControlsDisplayController.Instance.customActionsDisplayed.Find((ControlsDisplayController.CustomActionsDisplayed item) => item.key == key && item.displayTime > 0f);
					if (customActionsDisplayed != null)
					{
						this.SetCurrentPlayerInteraction(key, null, customActionsDisplayed.action, false, 11);
					}
					else
					{
						Interactable interactable = null;
						if (this.currentLookingAtInteractable != null && this.displayingInteraction)
						{
							interactable = this.currentLookingAtInteractable.interactable;
							this.UpdateInteractionText(this.currentLookingAtInteractable.interactable.GetName());
							Interactable.InteractableCurrentAction interactableCurrentAction = null;
							if (interactable.currentActions.TryGetValue(key, ref interactableCurrentAction) && interactableCurrentAction.currentAction != null && interactableCurrentAction.display)
							{
								if (FirstPersonItemController.Instance.drawnItem != null)
								{
									Interactable.InteractableCurrentAction interactableCurrentAction2 = null;
									if (FirstPersonItemController.Instance.currentActions.TryGetValue(key, ref interactableCurrentAction2))
									{
										try
										{
											if (interactableCurrentAction2.currentAction != null && interactableCurrentAction2.display && interactableCurrentAction2.currentAction.action.inputPriority > interactableCurrentAction.currentAction.action.inputPriority)
											{
												this.SetCurrentPlayerInteraction(key, interactable, interactableCurrentAction2, true, -1);
												InterfaceControls.Instance.interactionText.color = InterfaceControls.Instance.interactionTextNormalColour;
												continue;
											}
										}
										catch
										{
										}
									}
								}
								Color color = InterfaceControls.Instance.interactionTextNormalColour;
								if (interactable.objectRef != null)
								{
									NewDoor newDoor = interactable.objectRef as NewDoor;
									if (newDoor != null && newDoor.otherSideIsTrespassing)
									{
										if (newDoor.otherSideTrespassingEscalation <= 0)
										{
											color = InterfaceControls.Instance.trespassingEscalationZero;
										}
										else if (newDoor.otherSideTrespassingEscalation >= 1)
										{
											color = InterfaceControls.Instance.trespassingEscalationOne;
										}
									}
								}
								else if (interactable.thisDoor != null)
								{
									NewDoor newDoor2 = interactable.thisDoor.objectRef as NewDoor;
									if (newDoor2 != null && newDoor2.otherSideIsTrespassing)
									{
										if (newDoor2.otherSideTrespassingEscalation <= 0)
										{
											color = InterfaceControls.Instance.trespassingEscalationZero;
										}
										else if (newDoor2.otherSideTrespassingEscalation >= 1)
										{
											color = InterfaceControls.Instance.trespassingEscalationOne;
										}
									}
								}
								InterfaceControls.Instance.interactionText.color = color;
								this.SetCurrentPlayerInteraction(key, interactable, interactableCurrentAction, false, -1);
								continue;
							}
						}
						if (FirstPersonItemController.Instance.drawnItem != null)
						{
							Interactable.InteractableCurrentAction interactableCurrentAction3 = null;
							if (FirstPersonItemController.Instance.currentActions.TryGetValue(key, ref interactableCurrentAction3) && interactableCurrentAction3.currentAction != null && interactableCurrentAction3.display)
							{
								this.SetCurrentPlayerInteraction(key, interactable, interactableCurrentAction3, true, -1);
								continue;
							}
						}
						if (this.lockedInInteraction != null)
						{
							interactable = this.lockedInInteraction;
							Interactable.InteractableCurrentAction interactableCurrentAction4 = null;
							if (interactable.currentActions.TryGetValue(key, ref interactableCurrentAction4) && interactableCurrentAction4.currentAction != null && interactableCurrentAction4.display)
							{
								this.SetCurrentPlayerInteraction(key, interactable, interactableCurrentAction4, false, -1);
								continue;
							}
						}
						if (Game.Instance.allowDraggableRagdolls && this.currentLookAtTransform != null && key == InteractablePreset.InteractionKey.primary && InteractionController.Instance.currentlyDragging == null && (this.currentLookAtTransform.gameObject.layer == 24 || this.currentLookAtTransform.gameObject.layer == 7))
						{
							RigidbodyDragObject componentInParent = this.currentLookAtTransform.GetComponentInParent<RigidbodyDragObject>();
							if (componentInParent != null && componentInParent.ai != null && componentInParent.IsValidRagdollDragable())
							{
								Interactable.InteractableCurrentAction newCurrentAction22 = new Interactable.InteractableCurrentAction
								{
									display = true,
									enabled = true,
									overrideInteractionName = "Drag"
								};
								this.SetCurrentPlayerInteraction(key, null, newCurrentAction22, false, -1);
								continue;
							}
						}
						if (!SessionData.Instance.isFloorEdit)
						{
							this.SetCurrentPlayerInteraction(key, null, null, false, -1);
						}
					}
				}
			}
		}
		if (ControlsDisplayController.Instance != null)
		{
			ControlsDisplayController.Instance.UpdateControlDisplay();
		}
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x000F2EB8 File Offset: 0x000F10B8
	public void UpdateInteractionText(string newText)
	{
		if (newText != InterfaceControls.Instance.interactionText.text)
		{
			InterfaceControls.Instance.interactionText.SetText(newText, true);
			InterfaceControls.Instance.interactionTextContainer.sizeDelta = new Vector2(InterfaceControls.Instance.interactionText.preferredWidth + 28f, 52f);
		}
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x000F2F1C File Offset: 0x000F111C
	public void InteractionRaycastCheck()
	{
		if (this.carryingObject != null || InteractionController.Instance.dialogMode || InterfaceController.Instance.fade > 0f)
		{
			this.DisplayInteractionCursor(false, false);
			return;
		}
		if (CameraController.Instance == null || CameraController.Instance.cam == null)
		{
			return;
		}
		Ray ray = new Ray(CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.forward);
		int num = Toolbox.Instance.interactionRayLayerMask;
		if (Player.Instance.inAirVent)
		{
			num = Toolbox.Instance.interactionRayLayerMaskNoRoomMesh;
		}
		if (Physics.Raycast(ray, ref this.playerCurrentRaycastHit, 12f, num))
		{
			this.currentLookAtTransform = this.playerCurrentRaycastHit.transform;
		}
		else
		{
			this.currentLookAtTransform = null;
		}
		bool forceUpdate = false;
		if (this.playerCurrentRaycastHit.transform != this.playerPreviousRaycastHit.transform)
		{
			this.OnPlayerLookAtChange();
			forceUpdate = true;
		}
		if (this.lookingAtInteractable)
		{
			bool flag = false;
			if (this.currentLookingAtInteractable.interactable.drm && this.playerCurrentRaycastHit.distance <= this.currentLookingAtInteractable.interactable.preset.recognitionRange && (this.currentLookingAtInteractable.interactable.preset.distanceRecognitionOnly || this.playerCurrentRaycastHit.distance > this.currentLookingAtInteractable.interactable.GetReachDistance()))
			{
				flag = true;
				this.DisplayInteractionCursor(true, forceUpdate);
				if (!this.distanceRecognitionMode)
				{
					this.SetDistanceRecognitionMode(true);
				}
			}
			if (this.currentLookingAtInteractable.interactable.preset.readingEnabled && (!this.currentLookingAtInteractable.interactable.preset.readyingEnabledOnlyWithSwitchIsTue || (this.currentLookingAtInteractable.interactable.preset.readyingEnabledOnlyWithSwitchIsTue && this.currentLookingAtInteractable.interactable.sw0) || !this.currentLookingAtInteractable.interactable.preset.readingEnabledOnlyWithKaizenSkill || (this.currentLookingAtInteractable.interactable.preset.readingEnabledOnlyWithKaizenSkill && (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.wealthPerception) > 0f || UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.singlePerception) > 0f))))
			{
				if (this.playerCurrentRaycastHit.distance <= GameplayControls.Instance.readingRange)
				{
					flag = true;
					if (this.currentLookingAtInteractable.interactable.readingDelay > 0f)
					{
						this.currentLookingAtInteractable.interactable.readingDelay -= Time.deltaTime;
						if (this.readingMode)
						{
							this.SetReadingMode(false, false);
						}
					}
					else
					{
						this.DisplayInteractionCursor(true, forceUpdate);
						this.UpdateReadingModeText();
						if (!this.readingMode)
						{
							this.SetReadingMode(true, false);
						}
					}
				}
				else if (this.readingMode)
				{
					Game.Log("Debug: Out of reading range: " + this.playerCurrentRaycastHit.distance.ToString() + "/" + GameplayControls.Instance.readingRange.ToString(), 2);
					this.SetReadingMode(false, false);
				}
			}
			else if (this.readingMode)
			{
				this.SetReadingMode(false, false);
			}
			if (!this.currentLookingAtInteractable.interactable.preset.distanceRecognitionOnly)
			{
				float reachDistance = this.currentLookingAtInteractable.interactable.GetReachDistance();
				if (this.playerCurrentRaycastHit.distance <= reachDistance)
				{
					if (!flag)
					{
						bool flag2 = false;
						using (List<InteractablePreset.InteractionAction>.Enumerator enumerator = this.currentLookingAtInteractable.interactable.preset.GetActions(0).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.GetInteractionKey() != InteractablePreset.InteractionKey.none)
								{
									flag2 = true;
									break;
								}
							}
						}
						if (flag2)
						{
							flag = true;
						}
					}
					if (this.distanceRecognitionMode)
					{
						this.SetDistanceRecognitionMode(false);
					}
				}
				else if (Game.Instance.displayExtraControlHints && !CutSceneController.Instance.cutSceneActive && this.playerCurrentRaycastHit.distance <= reachDistance + 1f && !Player.Instance.isCrouched && !Player.Instance.fps.m_Jumping)
				{
					if (Player.Instance.fps.m_Camera.transform.localRotation.x >= 0.4f)
					{
						ControlsDisplayController.Instance.DisplayControlIcon(InteractablePreset.InteractionKey.crouch, "Crouch", InterfaceControls.Instance.controlIconDisplayTime, false);
					}
					else if (Player.Instance.fps.m_Camera.transform.localRotation.x <= -0.4f)
					{
						ControlsDisplayController.Instance.DisplayControlIcon(InteractablePreset.InteractionKey.jump, "Jump", InterfaceControls.Instance.controlIconDisplayTime, false);
					}
				}
			}
			else if (this.distanceRecognitionMode)
			{
				this.SetDistanceRecognitionMode(false);
			}
			if (!flag)
			{
				this.DisplayInteractionCursor(false, forceUpdate);
			}
			else
			{
				this.DisplayInteractionCursor(true, forceUpdate);
			}
			if (this.currentLookingAtInteractable != null && this.currentLookingAtInteractable.isDoor != null && this.playerCurrentRaycastHit.distance <= 5f && !Player.Instance.illegalStatus)
			{
				if (!this.currentLookingAtInteractable.isDoor.usingDoorList.Contains(Player.Instance))
				{
					this.currentLookingAtInteractable.isDoor.usingDoorList.Add(Player.Instance);
					this.addedToDoorInteractionList.Add(this.currentLookingAtInteractable.isDoor);
					Game.Log("Player: Player added to door using list (AI should keep door open for you!)", 2);
				}
			}
			else if (this.addedToDoorInteractionList.Count > 0)
			{
				foreach (NewDoor newDoor in this.addedToDoorInteractionList)
				{
					newDoor.usingDoorList.Remove(Player.Instance);
				}
				Game.Log("Player: Player removed from door using list.", 2);
				this.addedToDoorInteractionList.Clear();
			}
		}
		else if (this.readingMode)
		{
			this.SetReadingMode(false, false);
		}
		foreach (Actor actor in CityData.Instance.visibleActors)
		{
			if (!(actor.ai == null) && !actor.isStunned && !actor.isDead && actor.isMoving && !actor.ai.inCombat && !actor.ai.persuit && !actor.isMachine && !(actor.interactableController == null) && (actor.currentTile == null || !actor.currentTile.isStairwell) && !actor.currentTile.isInvertedStairwell)
			{
				if (Vector3.Distance(actor.transform.position, Player.Instance.transform.position) <= GameplayControls.Instance.maxPlayerLookAtTailingDistance)
				{
					RaycastHit raycastHit;
					if (Toolbox.Instance.RaycastCheck(CameraController.Instance.cam.transform.position, actor.interactableController.coll, GameplayControls.Instance.maxPlayerLookAtTailingDistance, out raycastHit))
					{
						Vector3 center = raycastHit.collider.bounds.center;
						Vector2 vector = CameraController.Instance.cam.WorldToScreenPoint(center) / new Vector2((float)CameraController.Instance.cam.pixelWidth, (float)CameraController.Instance.cam.pixelHeight);
						float num2 = 1f - Vector2.Distance(vector, new Vector2(0.5f, 0.5f)) * 2f;
						num2 = GameplayControls.Instance.screenCentreSpookCurve.Evaluate(num2);
						if (FirstPersonItemController.Instance.flashlight)
						{
							num2 *= 1.5f;
						}
						if (!Player.Instance.isMoving)
						{
							num2 *= 0.5f;
						}
						Human human = actor as Human;
						if (human != null)
						{
							num2 *= 1f - human.drunk * 0.5f;
							num2 *= 1f + human.alertness * 0.2f;
						}
						if (actor.ai.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor != null && item.actor.isPlayer))
						{
							num2 *= 0.3f;
						}
						if (num2 > 0.01f)
						{
							num2 = Mathf.Clamp01(num2);
							actor.ai.AddSpooked(Mathf.Max(GameplayControls.Instance.maxPlayerLookAtTailingDistance - this.playerCurrentRaycastHit.distance, 0f) / GameplayControls.Instance.maxPlayerLookAtTailingDistance * Time.deltaTime * GameplayControls.Instance.playerLookAtSpookRate * num2 * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.spookedMultiplier)));
						}
					}
				}
				else if (actor.ai.spooked > 0f)
				{
					actor.ai.AddSpooked(Time.deltaTime * -0.5f);
				}
			}
		}
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x000F3870 File Offset: 0x000F1A70
	public void OnPlayerLookAtChange()
	{
		if (this.currentLookingAtInteractable != null && this.currentLookingAtInteractable.interactable.readingDelay > 0f)
		{
			this.currentLookingAtInteractable.interactable.readingDelay = 0f;
		}
		this.lookingAtInteractable = false;
		this.currentLookingAtInteractable = null;
		if (this.playerCurrentRaycastHit.transform != null)
		{
			if (SessionData.Instance.isTestScene)
			{
				this.currentLookingAtInteractable = this.playerCurrentRaycastHit.transform.gameObject.GetComponent<InteractableController>();
				if (this.currentLookingAtInteractable != null && this.currentLookingAtInteractable.interactable == null)
				{
					InteractablePreset interactablePreset = null;
					foreach (KeyValuePair<string, InteractablePreset> keyValuePair in Toolbox.Instance.objectPresetDictionary)
					{
						if (keyValuePair.Value.prefab != null && keyValuePair.Value.prefab.name == this.currentLookingAtInteractable.transform.name)
						{
							interactablePreset = keyValuePair.Value;
							break;
						}
					}
					if (interactablePreset != null)
					{
						Game.Log("... Spawning interactable " + interactablePreset.name, 2);
						this.currentLookingAtInteractable.interactable = InteractableCreator.Instance.CreateWorldInteractable(interactablePreset, Player.Instance, Player.Instance, null, this.currentLookingAtInteractable.transform.position, this.currentLookingAtInteractable.transform.eulerAngles, null, null, "");
						this.currentLookingAtInteractable.interactable.spawnedObject = this.currentLookingAtInteractable.gameObject;
						this.currentLookingAtInteractable.Setup(this.currentLookingAtInteractable.interactable);
						this.currentLookingAtInteractable.interactable.OnSpawn();
					}
					else
					{
						foreach (FurniturePreset furniturePreset in Toolbox.Instance.allFurniture)
						{
							if (furniturePreset.prefab != null)
							{
								Transform transform = null;
								if (furniturePreset.prefab.name == this.currentLookingAtInteractable.transform.name)
								{
									transform = PrefabControls.Instance.cityContainer.transform;
								}
								else if (furniturePreset.prefab.name == this.currentLookingAtInteractable.transform.parent.name)
								{
									transform = this.currentLookingAtInteractable.transform;
								}
								if (transform != null)
								{
									foreach (FurniturePreset.IntegratedInteractable integratedInteractable in furniturePreset.integratedInteractables)
									{
										if (integratedInteractable.pairToController == this.currentLookingAtInteractable.id)
										{
											Game.Log("... Spawning interactable " + integratedInteractable.preset.name, 2);
											this.currentLookingAtInteractable.interactable = InteractableCreator.Instance.CreateWorldInteractable(integratedInteractable.preset, Player.Instance, Player.Instance, null, this.currentLookingAtInteractable.transform.position, this.currentLookingAtInteractable.transform.eulerAngles, null, null, "");
											if (integratedInteractable.preset.isLight != null)
											{
												this.currentLookingAtInteractable.interactable.SetAsLight(integratedInteractable.preset.isLight, 0, integratedInteractable.preset.isMainLight, null);
											}
											this.currentLookingAtInteractable.Setup(this.currentLookingAtInteractable.interactable);
											this.currentLookingAtInteractable.interactable.OnSpawn();
											break;
										}
									}
									if (this.currentLookingAtInteractable.interactable != null)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			if (this.playerCurrentRaycastHit.transform.CompareTag("Interactable"))
			{
				foreach (InteractableController interactableController in this.playerCurrentRaycastHit.transform.gameObject.GetComponentsInChildren<InteractableController>())
				{
					if (interactableController.coll == this.playerCurrentRaycastHit.collider || (interactableController.altColl != null && interactableController.altColl == this.playerCurrentRaycastHit.collider))
					{
						this.currentLookingAtInteractable = interactableController;
						break;
					}
				}
				if (this.currentLookingAtInteractable == null)
				{
					InteractablePointer componentInChildren = this.playerCurrentRaycastHit.transform.gameObject.GetComponentInChildren<InteractablePointer>();
					if (componentInChildren != null)
					{
						this.currentLookingAtInteractable = componentInChildren.pointTo;
					}
				}
				if (this.currentLookingAtInteractable == null)
				{
					this.currentLookingAtInteractable = this.playerCurrentRaycastHit.transform.gameObject.GetComponentInParent<InteractableController>();
				}
				if (this.currentLookingAtInteractable != null)
				{
					this.lookingAtInteractable = true;
				}
			}
		}
		if (!this.lookingAtInteractable)
		{
			this.DisplayInteractionCursor(false, false);
		}
		this.playerPreviousRaycastHit = this.playerCurrentRaycastHit;
		if (this.currentLookingAtInteractable != this.previousLookingAtInteractable)
		{
			this.OnPlayerLookAtInteractableChange();
		}
		this.UpdateInteractionText();
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x000F3DE0 File Offset: 0x000F1FE0
	public void OnPlayerLookAtInteractableChange()
	{
		this.previousLookingAtInteractable = this.currentLookingAtInteractable;
		this.UpdateHighlightedInteractionIcon();
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x000F3DF4 File Offset: 0x000F1FF4
	public void SetLockedInInteractionMode(Interactable val, int reference = 0, bool dropCarriedCheck = true)
	{
		Interactable interactable = this.lockedInInteraction;
		if (this.lockedInInteraction != null)
		{
			if (InteractionController.Instance.carryingObject != null && dropCarriedCheck)
			{
				InteractionController.Instance.carryingObject.DropThis(false);
			}
			if (this.lockedInInteraction != null)
			{
				if (this.lockedInInteraction.usagePoint != null)
				{
					this.lockedInInteraction.usagePoint.TrySetUser(Interactable.UsePointSlot.defaultSlot, null, "");
				}
				if (Player.Instance.interactingWith != null)
				{
					Actor actor = Player.Instance.interactingWith.objectRef as Actor;
					if (actor != null)
					{
						actor.SetInteracting(null);
					}
				}
				Player.Instance.SetInteracting(null);
				if (this.lockedInInteraction.controller == null)
				{
					Game.Log(string.Concat(new string[]
					{
						"Locked in interaction with ",
						this.lockedInInteraction.GetName(),
						" (",
						this.lockedInInteraction.id.ToString(),
						" at ",
						this.lockedInInteraction.GetWorldPosition(true).ToString(),
						") has no controller; attempting to spawn..."
					}), 2);
					this.lockedInInteraction.LoadInteractableToWorld(false, true);
					if (this.lockedInInteraction.controller == null)
					{
						Game.LogError(string.Concat(new string[]
						{
							"Locked in interaction with ",
							this.lockedInInteraction.GetName(),
							" (",
							this.lockedInInteraction.id.ToString(),
							" at ",
							this.lockedInInteraction.GetWorldPosition(true).ToString(),
							") has no controller!"
						}), 2);
					}
				}
				else if (this.lockedInInteraction.controller.coll != null)
				{
					this.lockedInInteraction.controller.coll.enabled = true;
				}
				if (this.OnReturnFromLockedIn != null)
				{
					this.OnReturnFromLockedIn();
				}
			}
		}
		this.lockedInInteraction = val;
		this.lockedInInteractionRef = reference;
		Player.Instance.SetInteracting(val);
		if (this.lockedInInteraction != null && this.lockedInInteraction.usagePoint != null)
		{
			this.lockedInInteraction.usagePoint.TrySetUser(Interactable.UsePointSlot.defaultSlot, Player.Instance, "");
		}
		if (interactable != null)
		{
			interactable.UpdateCurrentActions();
		}
		if (this.lockedInInteraction != null)
		{
			Game.Log("Player: Set locked-in interaction: " + this.lockedInInteraction.preset.name, 2);
			if (this.lockedInInteraction.preset.actionsPreset.Exists((InteractableActionsPreset item) => item.disableCollider) && val != null && val.controller != null && val.controller.coll != null)
			{
				val.controller.coll.enabled = false;
			}
			this.lockedInInteraction.UpdateCurrentActions();
		}
		this.InteractionRaycastCheck();
		this.UpdateInteractionText();
		if (this.lockedInInteraction == null)
		{
			if (Player.Instance.interactingWith != null)
			{
				Actor actor2 = Player.Instance.interactingWith.objectRef as Actor;
				if (actor2 != null)
				{
					actor2.SetInteracting(null);
				}
			}
			Player.Instance.SetInteracting(null);
			if (this.OnReturnFromLockedIn != null)
			{
				this.OnReturnFromLockedIn();
			}
		}
		Player.Instance.UpdateIllegalStatus();
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x000F4160 File Offset: 0x000F2360
	public void SetInteractionAction(float startingValue, float newThreshold, float increaseRate, string dictName, bool isIllegal, bool useLockpicks, Transform lookAtToComplete, bool cancelIfTooFar = true)
	{
		if (this.activeInteractionAction)
		{
			return;
		}
		this.activeInteractionAction = true;
		this.activeInteractionActionLookCheck = false;
		this.cancelInteractionIfOutOfRange = cancelIfTooFar;
		this.interactionActionLookAt = lookAtToComplete;
		this.interactionActionAmount = startingValue;
		this.interactionActionThreshold = newThreshold;
		this.interactionActionMultiplier = increaseRate;
		this.interactionActionName = dictName;
		InterfaceControls.Instance.actionInteractionText.text = Strings.Get("ui.interaction", dictName, Strings.Casing.asIs, false, false, false, null) + "...";
		this.discoveryOverTime.Clear();
		while (this.spawnedProgressControllers.Count > 0)
		{
			Object.Destroy(this.spawnedProgressControllers[0].gameObject);
			this.spawnedProgressControllers.RemoveAt(0);
		}
		float num = InterfaceControls.Instance.actionInteractionAnchor.rect.width - 8f;
		this.lockpickGraphics.SetActive(useLockpicks);
		if (useLockpicks)
		{
			bool flag = false;
			float num2 = this.interactionActionThreshold;
			int num3 = 0;
			float num4 = 4f;
			while (num2 > 0f)
			{
				float num5 = this.interactionActionMultiplier;
				if (!flag)
				{
					num5 = GameplayController.Instance.currentLockpickStrength * this.interactionActionMultiplier;
					flag = true;
				}
				num5 = Mathf.Min(num5, num2);
				LockpickProgressController component = Object.Instantiate<GameObject>(PrefabControls.Instance.lockpickProgressBar, InterfaceControls.Instance.actionInteractionAnchor).GetComponent<LockpickProgressController>();
				component.rect.sizeDelta = new Vector2(num5 / this.interactionActionThreshold * num - 4f, component.rect.sizeDelta.y);
				component.rect.anchoredPosition = new Vector2(num4, component.rect.anchoredPosition.y);
				num4 += component.rect.sizeDelta.x + 4f;
				component.SetBarMax(num5);
				component.SetAmount(0f);
				this.spawnedProgressControllers.Add(component);
				num2 -= num5;
				num3++;
			}
		}
		else
		{
			LockpickProgressController component2 = Object.Instantiate<GameObject>(PrefabControls.Instance.lockpickProgressBar, InterfaceControls.Instance.actionInteractionAnchor).GetComponent<LockpickProgressController>();
			component2.rect.sizeDelta = new Vector2(num - 4f, component2.rect.sizeDelta.y);
			component2.rect.anchoredPosition = new Vector2(4f, component2.rect.anchoredPosition.y);
			component2.SetBarMax(this.interactionActionThreshold);
			component2.SetAmount(0f);
			this.spawnedProgressControllers.Add(component2);
		}
		InterfaceControls.Instance.actionInteractionDisplay.gameObject.SetActive(true);
		if (!isIllegal || Player.Instance.locationsOfAuthority.Contains(Player.Instance.currentGameLocation) || !(Player.Instance.currentGameLocation != Player.Instance.home) || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, Player.Instance.currentGameLocation))
		{
			this.SetIllegalActionActive(false);
			return;
		}
		if (!useLockpicks)
		{
			this.SetIllegalActionActive(true);
			return;
		}
		if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.triggerIllegalOnPick) > 0f)
		{
			this.SetIllegalActionActive(false);
			return;
		}
		this.SetIllegalActionActive(true);
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x000F4498 File Offset: 0x000F2698
	public void SetIllegalActionActive(bool val)
	{
		if (Player.Instance.illegalActionActive != val)
		{
			Player.Instance.illegalActionActive = val;
			Player.Instance.illegalActionTimer = GameplayControls.Instance.illegalActionMinimumTime;
			Game.Log("Player: Set illegal action active: " + val.ToString(), 2);
			Player.Instance.UpdateIllegalStatus();
		}
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x000F44F4 File Offset: 0x000F26F4
	public void CancelInteractionAction()
	{
		while (this.spawnedProgressControllers.Count > 0)
		{
			Object.Destroy(this.spawnedProgressControllers[0].gameObject);
			this.spawnedProgressControllers.RemoveAt(0);
		}
		if (this.activeInteractionAction)
		{
			this.SetIllegalActionActive(false);
			this.activeInteractionAction = false;
			InterfaceControls.Instance.actionInteractionDisplay.gameObject.SetActive(false);
			if (this.OnInteractionActionCancelled != null)
			{
				this.OnInteractionActionCancelled();
			}
		}
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x000F4574 File Offset: 0x000F2774
	public void CompleteInteractionAction()
	{
		if (!Player.Instance.playerKOInProgress)
		{
			while (this.spawnedProgressControllers.Count > 0)
			{
				Object.Destroy(this.spawnedProgressControllers[0].gameObject);
				this.spawnedProgressControllers.RemoveAt(0);
			}
			InterfaceControls.Instance.actionInteractionDisplay.gameObject.SetActive(false);
		}
		if (this.activeInteractionAction)
		{
			this.SetIllegalActionActive(false);
			this.activeInteractionAction = false;
			Game.Log("Player: Complete interaction action", 2);
			if (this.OnInteractionActionCompleted != null)
			{
				this.OnInteractionActionCompleted();
			}
		}
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x000F4608 File Offset: 0x000F2808
	private void OnDisable()
	{
		this.previousLookingAtInteractable = null;
		this.SetReadingMode(false, true);
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x000F461C File Offset: 0x000F281C
	public void PickUp(Interactable newObj)
	{
		if (this.carryingObject != null)
		{
			this.carryingObject.DropThis(false);
			return;
		}
		if (newObj != null)
		{
			if (newObj.controller == null)
			{
				newObj.LoadInteractableToWorld(false, true);
			}
			if (newObj.controller != null)
			{
				newObj.controller.MovablePickUpThis();
			}
			this.SetLockedInInteractionMode(newObj, 2, true);
		}
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x000F4680 File Offset: 0x000F2880
	public void SetDialog(bool val, Interactable newTalkingTo, bool newIsRemote = false, Interactable newRemoteOverrideInteractable = null, InteractionController.ConversationType newConvoType = InteractionController.ConversationType.normal)
	{
		if (newTalkingTo != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Player: Set Dialog ",
				val.ToString(),
				" talking to: ",
				newTalkingTo.id.ToString(),
				" remote: ",
				newIsRemote.ToString()
			}), 2);
		}
		else
		{
			Game.Log("Player: Set Dialog " + val.ToString(), 2);
		}
		this.dialogMode = val;
		this.isRemote = newIsRemote;
		this.talkingTo = newTalkingTo;
		this.remoteOverride = newRemoteOverrideInteractable;
		if (val)
		{
			this.dialogType = newConvoType;
		}
		Game.Log("Player: Setting dialog type: " + this.dialogType.ToString(), 2);
		DialogController.Instance.sideJobReference = null;
		this.moreOptionsScrollDownArrow.gameObject.SetActive(false);
		this.moreOptionsScrollUpArrow.gameObject.SetActive(false);
		if (this.dialogMode)
		{
			InterfaceController.Instance.UpdateDOF();
			if (this.talkingTo != null && this.talkingTo.isActor != null)
			{
				if (!this.isRemote)
				{
					Game.Log("Merging voice and photo", 2);
					this.talkingTo.evidence.MergeDataKeys(Evidence.DataKey.voice, Evidence.DataKey.photo);
				}
				(this.talkingTo.isActor as Human).SetInConversation(null, !this.isRemote);
			}
			PrefabControls.Instance.dialogRect.gameObject.SetActive(true);
			if (this.talkingTo != null)
			{
				this.SetDialogSelection(0);
				this.RefreshDialogOptions();
			}
			ControlsDisplayController.Instance.SetControlDisplayArea(82f, 0f, 300f, 300f);
		}
		else
		{
			this.talkingTo = null;
			this.isRemote = false;
			this.remoteOverride = null;
			InterfaceController.Instance.UpdateDOF();
			for (int i = 0; i < this.dialogOptions.Count; i++)
			{
				Object.Destroy(this.dialogOptions[i].gameObject);
			}
			this.dialogOptions.Clear();
			PrefabControls.Instance.dialogRect.gameObject.SetActive(false);
			ControlsDisplayController.Instance.RestoreDefaultDisplayArea();
			if (this.lockedInInteraction != null)
			{
				this.lockedInInteraction.UpdateCurrentActions();
				this.UpdateInteractionText();
			}
		}
		if (InterfaceController.Instance.desktopMode)
		{
			InterfaceController.Instance.objectCycleAnchor.gameObject.SetActive(false);
			return;
		}
		InterfaceController.Instance.objectCycleAnchor.gameObject.SetActive(!this.dialogMode);
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x000F48F8 File Offset: 0x000F2AF8
	public void RefreshDialogOptions()
	{
		if (this.talkingTo != null)
		{
			Game.Log("Debug: Refreshing dialog options. Talking to " + this.talkingTo.id.ToString() + " remote: " + this.isRemote.ToString(), 2);
		}
		EvidenceWitness evidenceWitness = this.talkingTo.evidence as EvidenceWitness;
		this.citizenNameText.text = evidenceWitness.GetNameForDataKey(Evidence.DataKey.voice);
		for (int i = 0; i < this.dialogOptions.Count; i++)
		{
			Object.Destroy(this.dialogOptions[i].gameObject);
		}
		this.dialogOptions.Clear();
		List<EvidenceWitness.DialogOption> list = new List<EvidenceWitness.DialogOption>();
		if (this.talkingTo != null && this.talkingTo.belongsTo != Player.Instance && this.talkingTo.speechController != null && !this.talkingTo.speechController.speechActive && ((this.isRemote && this.remoteOverride != null && this.remoteOverride.speechController != null) || !this.isRemote))
		{
			if (this.isRemote && !this.remoteOverride.speechController.speechActive && this.remoteOverride.speechController.speechQueue.Count <= 0)
			{
				Game.Log("Debug: ...IsRemote, remote override speech inactive", 2);
				list = evidenceWitness.GetDialogOptions(Evidence.DataKey.voice);
				goto IL_2E3;
			}
			if (this.isRemote || this.talkingTo.speechController.speechQueue.Count > 0)
			{
				goto IL_2E3;
			}
			Game.Log("Debug: ...Isn't remote, speech inactive: " + InteractionController.Instance.dialogType.ToString(), 2);
			list = evidenceWitness.GetDialogOptions(Evidence.DataKey.photo);
			using (List<EvidenceWitness.DialogOption>.Enumerator enumerator = evidenceWitness.GetDialogOptions(Evidence.DataKey.voice).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EvidenceWitness.DialogOption dialogOption = enumerator.Current;
					if (!list.Contains(dialogOption))
					{
						list.Add(dialogOption);
					}
				}
				goto IL_2E3;
			}
		}
		if (this.talkingTo != null && this.talkingTo == Player.Instance.interactable && Player.Instance.phoneInteractable != null && this.remoteOverride.speechController != null && this.remoteOverride.speechController.speechQueue.Count <= 0 && this.dialogMode && this.dialogType == InteractionController.ConversationType.killerCleanUp)
		{
			Game.Log("Debug: ...Loading killer cover up decision tree", 2);
			list.AddRange(CitizenControls.Instance.coverUpConvoOptions);
		}
		else if (this.talkingTo != null && this.talkingTo.preset.specialCaseFlag == InteractablePreset.SpecialCase.hospitalBed)
		{
			Game.Log("Debug: ...Loading hospital decision tree", 2);
			list.AddRange(evidenceWitness.GetDialogOptions(Evidence.DataKey.voice));
		}
		else if (this.talkingTo != null && this.talkingTo.preset.specialCaseFlag == InteractablePreset.SpecialCase.telephone)
		{
			Game.Log("Debug: ...Loading telephone decision tree", 2);
			list.AddRange(evidenceWitness.GetDialogOptions(Evidence.DataKey.voice));
		}
		IL_2E3:
		list.Sort((EvidenceWitness.DialogOption p1, EvidenceWitness.DialogOption p2) => p2.preset.ranking.CompareTo(p1.preset.ranking));
		int num = 0;
		for (int j = 0; j < list.Count; j++)
		{
			EvidenceWitness.DialogOption dialogOption2 = list[j];
			if (DialogController.Instance.TestSpecialCaseAvailability(dialogOption2.preset, this.talkingTo.isActor as Citizen, dialogOption2.jobRef))
			{
				DialogButtonController component = Object.Instantiate<GameObject>(PrefabControls.Instance.dialogOption, PrefabControls.Instance.dialogOptionContainer).GetComponent<DialogButtonController>();
				component.Setup(dialogOption2);
				component.rect.anchoredPosition = new Vector2(0f, (float)num);
				num -= 52;
				this.dialogOptions.Add(component);
				component.SetSelectable(true);
				int num2 = dialogOption2.preset.GetCost(this.talkingTo.isActor, Player.Instance);
				if (dialogOption2.preset.specialCase == DialogPreset.SpecialCase.medicalCosts)
				{
					num2 = Mathf.RoundToInt((float)dialogOption2.preset.GetCost(this.talkingTo.isActor, Player.Instance) * (1f - UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.reduceMedicalCosts)));
				}
				if (num2 > 0 && num2 > GameplayController.Instance.money)
				{
					component.SetSelectable(false);
				}
			}
		}
		this.SetDialogSelection(Mathf.Clamp(this.dialogSelection, 0, this.dialogOptions.Count - 1));
		if (this.dialogOptions.Count > 0)
		{
			if (this.remoteOverride != null && this.remoteOverride.preset.specialCaseFlag == InteractablePreset.SpecialCase.telephone && !this.remoteOverride.sw2)
			{
				Game.Log("Debug: ...Forcing talk state on telephone", 2);
				this.remoteOverride.SetCustomState2(true, Player.Instance, false, true, false);
			}
			this.UpdateInteractionText();
		}
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x000F4DC8 File Offset: 0x000F2FC8
	public void SetDialogSelection(int newVal)
	{
		this.dialogSelection = newVal;
		this.dialogSelection = Mathf.Clamp(this.dialogSelection, 0, this.dialogOptions.Count - 1);
		this.moreOptionsScrollUpArrow.gameObject.SetActive(false);
		this.moreOptionsScrollDownArrow.gameObject.SetActive(false);
		PrefabControls.Instance.dialogOptionContainer.anchoredPosition = new Vector2(PrefabControls.Instance.dialogOptionContainer.anchoredPosition.x, (float)(-58 + Mathf.Max(this.dialogSelection - 3, 0) * 52));
		for (int i = 0; i < this.dialogOptions.Count; i++)
		{
			DialogButtonController dialogButtonController = this.dialogOptions[i];
			if (dialogButtonController.rect.position.y >= this.moreOptionsScrollUpArrow.position.y)
			{
				dialogButtonController.gameObject.SetActive(false);
				this.moreOptionsScrollUpArrow.gameObject.SetActive(true);
			}
			else if (dialogButtonController.rect.position.y <= this.moreOptionsScrollDownArrow.position.y + 32f)
			{
				dialogButtonController.gameObject.SetActive(false);
				this.moreOptionsScrollDownArrow.gameObject.SetActive(true);
			}
			else
			{
				dialogButtonController.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x000F4F18 File Offset: 0x000F3118
	public void OnSabotage(Interactable inter)
	{
		InteractionController.Instance.SetLockedInInteractionMode(inter, 0, true);
		this.sabotageInteractable = inter;
		Player.Instance.TransformPlayerController(GameplayControls.Instance.sabotageEnter, GameplayControls.Instance.sabotageExit, inter, null, false, false, 0f, false, default(Vector3), 1f, true);
		this.SetInteractionAction(0f, this.sabotageInteractable.val, Mathf.LerpUnclamped(GameplayControls.Instance.lockpickEffectivenessRange.x, GameplayControls.Instance.lockpickEffectivenessRange.y, UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.lockpickingEfficiencyModifier)), "sabotaging", true, true, inter.controller.transform, true);
		Player.Instance.isLockpicking = true;
		this.OnInteractionActionProgressChange += this.OnSabotageProgressChange;
		this.OnInteractionActionCompleted += this.OnCompleteSabotage;
		this.OnReturnFromLockedIn += this.OnReturnFromSabotage;
		AudioController.Instance.StopSound(this.lockpickLoop, AudioController.StopType.immediate, "Stop lockpick");
		this.lockpickLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.lockpick, Player.Instance, inter, null, 1f, false, false, null, null);
		if (Player.Instance.home != Player.Instance.currentGameLocation && !Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, Player.Instance.currentGameLocation))
		{
			StatusController.Instance.AddFineRecord(Player.Instance.currentGameLocation.thisAsAddress, inter, StatusController.CrimeType.breakingAndEntering, false, -1, false);
		}
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x000F50A0 File Offset: 0x000F32A0
	public void OnSabotageProgressChange(float amountChangeThisFrame, float amountToal)
	{
		this.sabotageInteractable.SetValue(this.sabotageInteractable.val - amountChangeThisFrame);
		GameplayController.Instance.UseLockpick(amountChangeThisFrame);
		if (GameplayController.Instance.lockPicks <= 0)
		{
			InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_lockpicks", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.lockpick, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.lockpicks, null, null, null);
		}
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x000F5124 File Offset: 0x000F3324
	public void OnCompleteSabotage()
	{
		AudioController.Instance.StopSound(this.lockpickLoop, AudioController.StopType.fade, "Complete lockpick");
		this.lockpickLoop = null;
		InteractionController.Instance.OnInteractionActionProgressChange -= this.OnSabotageProgressChange;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteSabotage;
		Player.Instance.isLockpicking = false;
		this.sabotageInteractable.SetValue(0f);
		this.sabotageInteractable.SetSwitchState(false, Player.Instance, true, false, false);
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		StatusController.Instance.RemoveFineRecord(null, this.sabotageInteractable, StatusController.CrimeType.breakingAndEntering, true, true);
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x000F51CC File Offset: 0x000F33CC
	public void OnReturnFromSabotage()
	{
		AudioController.Instance.StopSound(this.lockpickLoop, AudioController.StopType.fade, "Return from lockpick");
		this.lockpickLoop = null;
		InteractionController.Instance.OnInteractionActionProgressChange -= this.OnSabotageProgressChange;
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromSabotage;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteSabotage;
		Player.Instance.isLockpicking = false;
		Player.Instance.ReturnFromTransform(false, true);
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, GameplayController.Instance.lockPicks.ToString() + " " + Strings.Get("ui.gamemessage", "lockpick_deplete", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.lockpick, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.lockpicks, null, null, null);
		StatusController.Instance.RemoveFineRecord(null, this.sabotageInteractable, StatusController.CrimeType.breakingAndEntering, true, true);
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x000F52B4 File Offset: 0x000F34B4
	public bool GetValidPlayerActionIllegal(Interactable inter, NewNode location, bool allowPublic = true, bool illegalIfNotPlayersHome = true)
	{
		bool result = false;
		if (inter == null)
		{
			return true;
		}
		if (inter.belongsTo == Player.Instance)
		{
			return false;
		}
		if (inter.inInventory != null && inter.inInventory != Player.Instance)
		{
			return true;
		}
		if (location == null)
		{
			return true;
		}
		if (location.gameLocation == Player.Instance.home || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, location.gameLocation))
		{
			return false;
		}
		if (illegalIfNotPlayersHome && inter.node != null && (inter.node.gameLocation == Player.Instance.home || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, inter.node.gameLocation)))
		{
			return false;
		}
		if (!Player.Instance.locationsOfAuthority.Contains(location.gameLocation) && inter.belongsTo != Player.Instance)
		{
			if (inter.preset.onlyIllegalIfInNonPublic)
			{
				if (inter.furnitureParent != null && inter.furnitureParent.furniture != null && inter.furnitureParent.furniture.forcePublicIllegal)
				{
					Game.Log("Player: Return action illegal: Force public illegal on furniture: True", 2);
					result = true;
				}
				else if (allowPublic)
				{
					result = !location.room.IsAccessAllowed(Player.Instance);
					Game.Log("Player: Return action illegal, !access allowed: " + result.ToString(), 2);
				}
				else
				{
					Game.Log("Player: Return action illegal: Public places not allowed: True", 2);
					result = true;
				}
			}
			else
			{
				Game.Log("Player: Return action illegal: True", 2);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x000F5440 File Offset: 0x000F3640
	public void UpdateNearbyInteractables()
	{
		this.nearbyInteractables.Clear();
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (!Player.Instance.playerKOInProgress)
		{
			foreach (Interactable interactable in this.GetValidNearbyInteractables(Player.Instance.currentNode))
			{
				if (!this.nearbyInteractables.Contains(interactable))
				{
					this.nearbyInteractables.Add(interactable);
				}
			}
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX8)
			{
				Vector3Int vector3Int = Player.Instance.currentNodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
				NewNode node = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref node))
				{
					foreach (Interactable interactable2 in this.GetValidNearbyInteractables(node))
					{
						if (!this.nearbyInteractables.Contains(interactable2))
						{
							this.nearbyInteractables.Add(interactable2);
						}
					}
				}
			}
			foreach (Actor actor in CityData.Instance.visibleActors)
			{
				if (actor.interactable != null && !(actor.interactable.controller == null) && !(actor.interactable.controller.coll == null))
				{
					Vector3 center = actor.interactable.controller.coll.bounds.center;
					Ray ray = new Ray(CameraController.Instance.cam.transform.position, center - CameraController.Instance.cam.transform.position);
					float reachDistance = actor.interactable.GetReachDistance();
					RaycastHit raycastHit;
					if (Physics.Raycast(ray, ref raycastHit, reachDistance, Toolbox.Instance.interactionRayLayerMask) && raycastHit.collider == actor.interactable.controller.coll)
					{
						bool flag = false;
						using (List<InteractablePreset.InteractionAction>.Enumerator enumerator3 = actor.interactable.preset.GetActions(0).GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								if (enumerator3.Current.GetInteractionKey() != InteractablePreset.InteractionKey.none)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag && !this.nearbyInteractables.Contains(actor.interactable))
						{
							this.nearbyInteractables.Add(actor.interactable);
						}
					}
				}
			}
			this.nearbyInteractables.Sort((Interactable p1, Interactable p2) => Vector3.Distance(p1.controller.coll.bounds.center, CameraController.Instance.cam.transform.position).CompareTo(Vector3.Distance(p2.controller.coll.bounds.center, CameraController.Instance.cam.transform.position)));
		}
		this.UpdateInteractionIcons();
		if (Game.Instance.displayExtraControlHints && !CutSceneController.Instance.cutSceneActive && this.nearbyInteractables.Count > 1 && !InputController.Instance.mouseInputMode)
		{
			this.nearbyInteractablesHint++;
			if (this.nearbyInteractablesHint > 10)
			{
				ControlsDisplayController.Instance.DisplayControlIcon(InteractablePreset.InteractionKey.nearestInteractable, "Nearest Interactable", InterfaceControls.Instance.controlIconDisplayTime, false);
				this.nearbyInteractablesHint = 0;
			}
		}
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x000F5808 File Offset: 0x000F3A08
	public void ClearNearbyInteractables()
	{
		this.nearbyInteractables.Clear();
		this.UpdateInteractionIcons();
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x000F581C File Offset: 0x000F3A1C
	private List<Interactable> GetValidNearbyInteractables(NewNode node)
	{
		List<Interactable> list = new List<Interactable>();
		foreach (Interactable interactable in node.interactables)
		{
			if (!(interactable.controller == null) && !(interactable.controller.coll == null) && !list.Contains(interactable))
			{
				Vector3 center = interactable.controller.coll.bounds.center;
				Ray ray = new Ray(CameraController.Instance.cam.transform.position, center - CameraController.Instance.cam.transform.position);
				float reachDistance = interactable.GetReachDistance();
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, ref raycastHit, reachDistance, Toolbox.Instance.interactionRayLayerMask) && raycastHit.collider == interactable.controller.coll)
				{
					bool flag = false;
					using (List<InteractablePreset.InteractionAction>.Enumerator enumerator2 = interactable.preset.GetActions(0).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.GetInteractionKey() != InteractablePreset.InteractionKey.none)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						list.Add(interactable);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x000F59A0 File Offset: 0x000F3BA0
	public void FocusOnInteractable(Interactable interactable)
	{
		if (interactable.controller == null)
		{
			return;
		}
		Game.Log("Focus on: " + interactable.GetName(), 2);
		Player.Instance.ForceLookAt(interactable, 0.2f);
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x000F59D8 File Offset: 0x000F3BD8
	public void UpdateInteractionIcons()
	{
		List<Interactable> list = new List<Interactable>(this.nearbyInteractables);
		for (int i = 0; i < this.selectionIcons.Count; i++)
		{
			SelectionIconController selectionIconController = this.selectionIcons[i];
			if (list.Contains(selectionIconController.interactable))
			{
				list.Remove(selectionIconController.interactable);
				selectionIconController.destroy = false;
			}
			else
			{
				selectionIconController.Remove();
			}
		}
		foreach (Interactable newInteractable in list)
		{
			SelectionIconController component = Object.Instantiate<GameObject>(PrefabControls.Instance.objectSelectionIcon, PrefabControls.Instance.objectSelectionContainer).GetComponent<SelectionIconController>();
			component.Setup(newInteractable);
			this.selectionIcons.Add(component);
		}
		this.UpdateHighlightedInteractionIcon();
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x000F5AB8 File Offset: 0x000F3CB8
	public void UpdateHighlightedInteractionIcon()
	{
		foreach (SelectionIconController selectionIconController in this.selectionIcons)
		{
			if (this.currentLookingAtInteractable != null && this.currentLookingAtInteractable.interactable == selectionIconController.interactable)
			{
				selectionIconController.SetHighlighted(true);
			}
			else
			{
				selectionIconController.SetHighlighted(false);
			}
		}
	}

	// Token: 0x0400152B RID: 5419
	[Header("Interaction")]
	public Dictionary<InteractablePreset.InteractionKey, InteractionController.InteractionSetting> currentInteractions = new Dictionary<InteractablePreset.InteractionKey, InteractionController.InteractionSetting>();

	// Token: 0x0400152C RID: 5420
	public List<InteractablePreset.InteractionKey> allInteractionKeys;

	// Token: 0x0400152D RID: 5421
	public List<Interactable> nearbyInteractables = new List<Interactable>();

	// Token: 0x0400152E RID: 5422
	public List<SelectionIconController> selectionIcons = new List<SelectionIconController>();

	// Token: 0x0400152F RID: 5423
	public int nearbyInteractablesHint;

	// Token: 0x04001530 RID: 5424
	[Header("Player State")]
	public float inputCooldown;

	// Token: 0x04001531 RID: 5425
	public bool lookingAtInteractable;

	// Token: 0x04001532 RID: 5426
	public bool displayingInteraction;

	// Token: 0x04001533 RID: 5427
	private InteractableController previousLookingAtInteractable;

	// Token: 0x04001534 RID: 5428
	public InteractableController currentLookingAtInteractable;

	// Token: 0x04001535 RID: 5429
	public Transform currentLookAtTransform;

	// Token: 0x04001536 RID: 5430
	private InteractableController currentLookingAtReadingRange;

	// Token: 0x04001537 RID: 5431
	public InteractableController currentInteractable;

	// Token: 0x04001538 RID: 5432
	public bool interactionMode;

	// Token: 0x04001539 RID: 5433
	public bool distanceRecognitionMode;

	// Token: 0x0400153A RID: 5434
	public bool readingMode;

	// Token: 0x0400153B RID: 5435
	private float readingModeTransition;

	// Token: 0x0400153C RID: 5436
	private Coroutine readingModeCoroutine;

	// Token: 0x0400153D RID: 5437
	public float interactionAnimationModifier = 1f;

	// Token: 0x0400153E RID: 5438
	public float interactionLookProgress;

	// Token: 0x0400153F RID: 5439
	public InteractableController carryingObject;

	// Token: 0x04001540 RID: 5440
	private List<NewDoor> addedToDoorInteractionList = new List<NewDoor>();

	// Token: 0x04001541 RID: 5441
	public RigidbodyDragObject currentlyDragging;

	// Token: 0x04001542 RID: 5442
	private RaycastHit playerPreviousRaycastHit;

	// Token: 0x04001543 RID: 5443
	[NonSerialized]
	public RaycastHit playerCurrentRaycastHit;

	// Token: 0x04001544 RID: 5444
	[Header("Locked-In Interaction")]
	[NonSerialized]
	public Interactable lockedInInteraction;

	// Token: 0x04001545 RID: 5445
	public int lockedInInteractionRef;

	// Token: 0x04001546 RID: 5446
	[NonSerialized]
	public Interactable hideInteractable;

	// Token: 0x04001547 RID: 5447
	[Header("Interaction Action")]
	public bool activeInteractionAction;

	// Token: 0x04001549 RID: 5449
	private float interactionActionThreshold = 1f;

	// Token: 0x0400154A RID: 5450
	private float interactionActionMultiplier = 1f;

	// Token: 0x0400154B RID: 5451
	public string interactionActionName;

	// Token: 0x0400154D RID: 5453
	private bool activeInteractionActionLookCheck;

	// Token: 0x0400154E RID: 5454
	private bool canFailLookCheck;

	// Token: 0x0400154F RID: 5455
	public GameObject lockpickGraphics;

	// Token: 0x04001550 RID: 5456
	private bool cancelInteractionIfOutOfRange = true;

	// Token: 0x04001551 RID: 5457
	private float lastLookAtForInteraction;

	// Token: 0x04001552 RID: 5458
	public Dictionary<Interactable, float> discoveryOverTime = new Dictionary<Interactable, float>();

	// Token: 0x04001553 RID: 5459
	public Dictionary<Evidence, float> discoveryOverTimeEvidence = new Dictionary<Evidence, float>();

	// Token: 0x04001554 RID: 5460
	public Dictionary<MetaObject, float> discoveryOverTimeMeta = new Dictionary<MetaObject, float>();

	// Token: 0x04001555 RID: 5461
	public Dictionary<EvidenceMultiPage.MultiPageContent, float> discoveryOverTimeDiscovery = new Dictionary<EvidenceMultiPage.MultiPageContent, float>();

	// Token: 0x04001556 RID: 5462
	public List<LockpickProgressController> spawnedProgressControllers = new List<LockpickProgressController>();

	// Token: 0x04001557 RID: 5463
	private Interactable sabotageInteractable;

	// Token: 0x04001558 RID: 5464
	[Header("Dialog")]
	public bool dialogMode;

	// Token: 0x04001559 RID: 5465
	public bool isRemote;

	// Token: 0x0400155A RID: 5466
	public float dialogTransition;

	// Token: 0x0400155B RID: 5467
	public InteractionController.ConversationType dialogType;

	// Token: 0x0400155C RID: 5468
	public TextMeshProUGUI citizenNameText;

	// Token: 0x0400155D RID: 5469
	[NonSerialized]
	public Interactable talkingTo;

	// Token: 0x0400155E RID: 5470
	[NonSerialized]
	public Interactable remoteOverride;

	// Token: 0x0400155F RID: 5471
	public List<DialogButtonController> dialogOptions = new List<DialogButtonController>();

	// Token: 0x04001560 RID: 5472
	public int dialogSelection;

	// Token: 0x04001561 RID: 5473
	public RectTransform moreOptionsScrollUpArrow;

	// Token: 0x04001562 RID: 5474
	public RectTransform moreOptionsScrollDownArrow;

	// Token: 0x04001563 RID: 5475
	public Human mugger;

	// Token: 0x04001564 RID: 5476
	public Human debtCollector;

	// Token: 0x04001565 RID: 5477
	[Header("Interface")]
	public bool inOut;

	// Token: 0x04001566 RID: 5478
	public float inOutProgress;

	// Token: 0x04001567 RID: 5479
	public float displayProgress;

	// Token: 0x04001568 RID: 5480
	private AudioController.LoopingSoundInfo lockpickLoop;

	// Token: 0x0400156E RID: 5486
	private static InteractionController _instance;

	// Token: 0x020002EB RID: 747
	public class InteractionSetting
	{
		// Token: 0x0600111F RID: 4383 RVA: 0x000F5C08 File Offset: 0x000F3E08
		public int GetActionCost()
		{
			int result = 0;
			if (this.currentAction != null)
			{
				result = this.currentAction.actionCost;
				if (this.currentAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.decorPlacementPurchase)
				{
					result = PlayerApartmentController.Instance.GetCurrentCost();
				}
			}
			return result;
		}

		// Token: 0x0400156F RID: 5487
		public InteractablePreset.InteractionAction currentAction;

		// Token: 0x04001570 RID: 5488
		public Interactable.InteractableCurrentAction currentSetting;

		// Token: 0x04001571 RID: 5489
		[NonSerialized]
		public Interactable interactable;

		// Token: 0x04001572 RID: 5490
		public bool isFPSItem;

		// Token: 0x04001573 RID: 5491
		public AudioEvent audioEvent;

		// Token: 0x04001574 RID: 5492
		public int priority;

		// Token: 0x04001575 RID: 5493
		public string actionText;

		// Token: 0x04001576 RID: 5494
		public ControlDisplayController newUIRef;
	}

	// Token: 0x020002EC RID: 748
	public enum ConversationType
	{
		// Token: 0x04001578 RID: 5496
		normal,
		// Token: 0x04001579 RID: 5497
		mugging,
		// Token: 0x0400157A RID: 5498
		loanSharkVisit,
		// Token: 0x0400157B RID: 5499
		accuseMurderer,
		// Token: 0x0400157C RID: 5500
		killerCleanUp
	}

	// Token: 0x020002ED RID: 749
	// (Invoke) Token: 0x06001122 RID: 4386
	public delegate void ReturnFromLockedIn();

	// Token: 0x020002EE RID: 750
	// (Invoke) Token: 0x06001126 RID: 4390
	public delegate void InteractionActionCompleted();

	// Token: 0x020002EF RID: 751
	// (Invoke) Token: 0x0600112A RID: 4394
	public delegate void InteractionActionProgressChange(float amountThisFrame, float amountTotal);

	// Token: 0x020002F0 RID: 752
	// (Invoke) Token: 0x0600112E RID: 4398
	public delegate void InteractionActionLookedAway();

	// Token: 0x020002F1 RID: 753
	// (Invoke) Token: 0x06001132 RID: 4402
	public delegate void InteractionActionCancelled();
}
