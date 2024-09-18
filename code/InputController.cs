using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Rewired;
using Rewired.Demos;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002E3 RID: 739
public class InputController : MonoBehaviour
{
	// Token: 0x14000019 RID: 25
	// (add) Token: 0x060010BD RID: 4285 RVA: 0x000ECE48 File Offset: 0x000EB048
	// (remove) Token: 0x060010BE RID: 4286 RVA: 0x000ECE80 File Offset: 0x000EB080
	public event InputController.InputModeChange OnInputModeChange;

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x060010BF RID: 4287 RVA: 0x000ECEB5 File Offset: 0x000EB0B5
	public static InputController Instance
	{
		get
		{
			return InputController._instance;
		}
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x000ECEBC File Offset: 0x000EB0BC
	private void Awake()
	{
		if (InputController._instance != null && InputController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		InputController._instance = this;
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x000ECEEC File Offset: 0x000EB0EC
	private void Update()
	{
		if (!this.enableInput)
		{
			return;
		}
		if (!ReInput.isReady)
		{
			return;
		}
		if (PopupMessageController.Instance != null && PopupMessageController.Instance.active && PopupMessageController.Instance.appearProgress < 1f)
		{
			return;
		}
		if (PopupMessageController.Instance != null && !PopupMessageController.Instance.active && PopupMessageController.Instance.appearProgress > 0f)
		{
			return;
		}
		if (InteractionController.Instance.inputCooldown > 0f)
		{
			return;
		}
		if (SimpleControlRemappingSOD.Instance != null && SimpleControlRemappingSOD.Instance.listeningForRemap)
		{
			if (this.player.GetButtonDown("Menu"))
			{
				SimpleControlRemappingSOD.Instance.StopMapping(false);
			}
			return;
		}
		if (this.player == null)
		{
			this.player = ReInput.players.GetPlayer(0);
			if (this.player == null)
			{
				return;
			}
		}
		else if (!this.initalInputModeSet && !SessionData.Instance.isFloorEdit)
		{
			this.SetMouseInputMode(Convert.ToBoolean(PlayerPrefs.GetInt("controlMethod")), true);
			this.initalInputModeSet = true;
		}
		if (this.mouseInputMode && this.player.GetAnyButton())
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null && currentSelectedGameObject.GetComponent<TMP_InputField>() != null)
			{
				return;
			}
		}
		if (Game.Instance.controlAutoSwitch)
		{
			if (this.mouseInputMode && ReInput.controllers.GetAnyButtonUp(2))
			{
				this.lastActiveController = ReInput.controllers.GetLastActiveControllerType();
				this.SetMouseInputMode(false, false);
			}
			else if (!this.mouseInputMode && (ReInput.controllers.GetAnyButton(1) || ReInput.controllers.GetAnyButton(0)))
			{
				this.lastActiveController = ReInput.controllers.GetLastActiveControllerType();
				this.SetMouseInputMode(true, false);
			}
		}
		if (!this.mouseInputMode)
		{
			this.controlFallbackCheck += Time.deltaTime;
			if (this.controlFallbackCheck >= 2f)
			{
				if (!this.mouseInputMode && ReInput.controllers.joystickCount <= 0)
				{
					Game.Log("Player: Falling back to mouse/keyboard mode, as no joystick is detected...", 2);
					this.SetMouseInputMode(true, true);
				}
				this.controlFallbackCheck = 0f;
			}
		}
		if (!this.mouseInputMode)
		{
			if (!SessionData.Instance.isFloorEdit)
			{
				if (!MainMenuController.Instance.mainMenuActive || (PopupMessageController.Instance != null && PopupMessageController.Instance.active) || MainMenuController.Instance.currentComponent.component != MainMenuController.Component.loadingCity)
				{
					if (InterfaceController.Instance.selectedElement != null)
					{
						if (!this.mouseInputMode)
						{
							if (CasePanelController.Instance.controllerMode && InterfaceController.Instance.selectedElement == CasePanelController.Instance.closeCaseButton && CasePanelController.Instance.activeCase == null)
							{
								CasePanelController.Instance.closeCaseButton.OnDeselect();
								Selectable selectable = CasePanelController.Instance.closeCaseButton.button.FindSelectableOnLeft();
								if (selectable != null)
								{
									ButtonController component = selectable.gameObject.GetComponent<ButtonController>();
									if (component != null)
									{
										component.OnSelect();
									}
								}
							}
							if (this.player.GetButtonUp("Select"))
							{
								if (this.currentButtonDown != null)
								{
									PointerEventData eventData = new PointerEventData(null)
									{
										button = 0
									};
									this.currentButtonDown.OnPointerUp(eventData);
									this.currentButtonDown = null;
								}
								else
								{
									this.currentButtonDown = null;
								}
							}
							if (this.player.GetButtonDown("Select") && this.currentButtonDown == null)
							{
								this.currentButtonDown = InterfaceController.Instance.selectedElement;
								PointerEventData eventData2 = new PointerEventData(null)
								{
									button = 0
								};
								this.currentButtonDown.OnPointerDown(eventData2);
								if (this.currentButtonDown != null)
								{
									this.currentButtonDown.OnPointerClick(eventData2);
								}
								return;
							}
							if (this.player.GetButtonDown("Secondary") && this.currentButtonDown == null && InterfaceController.Instance.selectedElement.secondaryIsRightClick)
							{
								this.currentButtonDown = InterfaceController.Instance.selectedElement;
								PointerEventData eventData3 = new PointerEventData(null)
								{
									button = 1
								};
								this.currentButtonDown.OnPointerDown(eventData3);
								if (this.currentButtonDown != null)
								{
									this.currentButtonDown.OnPointerClick(eventData3);
								}
								return;
							}
						}
						if (InterfaceController.Instance.selectedElement != null && InterfaceController.Instance.selectedElement.button != null)
						{
							Selectable selectable2 = null;
							if (this.player.GetButtonDown("NavigateUp"))
							{
								selectable2 = InterfaceController.Instance.selectedElement.button.FindSelectableOnUp();
							}
							if (this.player.GetButtonDown("NavigateDown"))
							{
								selectable2 = InterfaceController.Instance.selectedElement.button.FindSelectableOnDown();
							}
							if (this.player.GetButtonDown("NavigateLeft"))
							{
								selectable2 = InterfaceController.Instance.selectedElement.button.FindSelectableOnLeft();
							}
							if (this.player.GetButtonDown("NavigateRight"))
							{
								selectable2 = InterfaceController.Instance.selectedElement.button.FindSelectableOnRight();
							}
							if (selectable2 != null && selectable2 != InterfaceController.Instance.selectedElement)
							{
								Game.Log("Menu: New button mouse over through non-mouse input: " + selectable2.gameObject.name, 2);
								InterfaceController.Instance.selectedElement.OnDeselect();
								ButtonController component2 = selectable2.gameObject.GetComponent<ButtonController>();
								if (component2 != null)
								{
									component2.OnSelect();
									if (MainMenuController.Instance.mainMenuActive)
									{
										if (MainMenuController.Instance.currentComponent != null && MainMenuController.Instance.currentComponent.buttons.Contains(component2))
										{
											MainMenuController.Instance.currentComponent.previouslySelected = component2;
											return;
										}
									}
									else if (HighlanderSingleton<CityEditorController>.Instance != null && SessionData.Instance.isCityEdit && component2.transform.GetComponentInParent<PrototypeDebugPanel>(true) != null)
									{
										HighlanderSingleton<CityEditorController>.Instance.previouslySelected = component2;
										return;
									}
								}
								else
								{
									TMP_InputField component3 = selectable2.gameObject.GetComponent<TMP_InputField>();
									if (component3 != null)
									{
										component3.ActivateInputField();
									}
								}
								return;
							}
						}
						else
						{
							Game.LogError("Menu: Current selectable doesn't have a button object attached! Cannot navigate from here...", 2);
						}
					}
					else
					{
						if (this.currentButtonDown != null)
						{
							this.currentButtonDown = null;
						}
						if (MainMenuController.Instance.mainMenuActive)
						{
							if (MainMenuController.Instance.raycaster.enabled)
							{
								Game.Log("Menu: Nothing is current selected, finding default...", 2);
								if (MainMenuController.Instance.currentComponent != null)
								{
									if (MainMenuController.Instance.currentComponent.previouslySelected != null && MainMenuController.Instance.currentComponent.previouslySelected.interactable)
									{
										Game.Log("Menu: ...Selected button " + MainMenuController.Instance.currentComponent.previouslySelected.gameObject.name + " in " + MainMenuController.Instance.currentComponent.component.ToString(), 2);
										MainMenuController.Instance.currentComponent.previouslySelected.OnSelect();
									}
									else
									{
										ButtonController buttonController = null;
										for (int i = 0; i < MainMenuController.Instance.currentComponent.buttons.Count; i++)
										{
											if (MainMenuController.Instance.currentComponent.buttons[i].interactable && (buttonController == null || MainMenuController.Instance.currentComponent.buttons[i].defaultSelectionPriority > buttonController.defaultSelectionPriority))
											{
												buttonController = MainMenuController.Instance.currentComponent.buttons[i];
											}
										}
										if (buttonController != null)
										{
											Game.Log("Menu: ...Selected button " + buttonController.gameObject.name + " in " + MainMenuController.Instance.currentComponent.component.ToString(), 2);
											buttonController.OnSelect();
										}
									}
								}
							}
						}
						else if (!PopupMessageController.Instance.active && (InterfaceController.Instance.desktopMode || BioScreenController.Instance.isOpen))
						{
							if (CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.windows)
							{
								if (InterfaceController.Instance.activeWindows.Count <= 0)
								{
									CasePanelController.Instance.SetControllerMode(true, CasePanelController.ControllerSelectMode.caseBoard);
								}
								else if (CasePanelController.Instance.selectedWindow != null)
								{
									CasePanelController.Instance.SetSelectedWindow(CasePanelController.Instance.selectedWindow, true);
								}
								else
								{
									CasePanelController.Instance.SetSelectedWindow(InterfaceController.Instance.activeWindows[0], true);
								}
							}
							if (UpgradesController.Instance.isOpen || BioScreenController.Instance.isOpen)
							{
								CasePanelController.Instance.SetControllerMode(true, CasePanelController.ControllerSelectMode.topBar);
							}
							if (CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.caseBoard && CasePanelController.Instance.spawnedPins.Count > 0)
							{
								if (CasePanelController.Instance.selectedPinned != null)
								{
									CasePanelController.Instance.SetSelectedPinned(CasePanelController.Instance.selectedPinned, true);
								}
								else
								{
									CasePanelController.Instance.SetSelectedPinned(CasePanelController.Instance.GetClosestPinnedToCentre(), true);
								}
							}
						}
					}
				}
				if (this.player.GetButtonDown("Back") && !MainMenuController.Instance.mainMenuActive && CutSceneController.Instance.cutSceneActive)
				{
					CutSceneController.Instance.StopScene();
					return;
				}
				if (MainMenuController.Instance.mainMenuActive && MainMenuController.Instance.currentComponent.component != MainMenuController.Component.loadingCity && this.player.GetButtonDown("Back") && MainMenuController.Instance.currentComponent != null && !PopupMessageController.Instance.active)
				{
					ButtonController buttonController2 = MainMenuController.Instance.currentComponent.buttons.Find((ButtonController item) => item.buttonType == ButtonController.ButtonAudioType.back && item.interactable);
					if (buttonController2 != null)
					{
						buttonController2.OnPointerClick(new PointerEventData(null)
						{
							button = 0
						});
					}
					if (InterfaceController.Instance.selectedElement != null)
					{
						InterfaceController.Instance.selectedElement.OnDeselect();
					}
					return;
				}
			}
		}
		else if (Cursor.visible != this.cursorVisible)
		{
			Cursor.visible = this.cursorVisible;
		}
		if ((PopupMessageController.Instance == null || !PopupMessageController.Instance.active) && MainMenuController.Instance != null && MainMenuController.Instance.currentComponent.component != MainMenuController.Component.loadingCity && !SessionData.Instance.isCityEdit)
		{
			if (!MainMenuController.Instance.mainMenuActive && !CutSceneController.Instance.cutSceneActive)
			{
				if (this.player.GetButtonDown("CaseBoard"))
				{
					if (InteractionController.Instance.currentLookAtTransform != null && Player.Instance.isCrunchingDatabase && InteractionController.Instance.currentLookAtTransform.name == "Screen")
					{
						return;
					}
					if (SessionData.Instance.enableUserPause)
					{
						if (!SessionData.Instance.play && !SessionData.Instance.isFloorEdit)
						{
							if (BioScreenController.Instance.isOpen && !InterfaceController.Instance.desktopMode)
							{
								InterfaceController.Instance.SetDesktopMode(true, true);
								return;
							}
							if (BioScreenController.Instance.isOpen)
							{
								BioScreenController.Instance.SetInventoryOpen(false, true, true);
								SessionData.Instance.ResumeGame();
								return;
							}
							SessionData.Instance.ResumeGame();
							return;
						}
						else if (SessionData.Instance.play)
						{
							InteractionController.InteractionSetting interactionSetting = null;
							if (InteractionController.Instance.currentInteractions.TryGetValue(InteractablePreset.InteractionKey.alternative, ref interactionSetting))
							{
								if (interactionSetting.currentSetting == null || !interactionSetting.currentSetting.enabled)
								{
									if (!Player.Instance.autoTravelActive || Game.Instance.autoTravelPause)
									{
										SessionData.Instance.PauseGame(true, false, true);
										return;
									}
									if (InterfaceController.Instance.desktopMode)
									{
										InterfaceController.Instance.SetDesktopMode(false, true);
										return;
									}
									InterfaceController.Instance.SetDesktopMode(true, true);
									return;
								}
							}
							else
							{
								if (!Player.Instance.autoTravelActive || Game.Instance.autoTravelPause)
								{
									SessionData.Instance.PauseGame(true, false, true);
									return;
								}
								if (InterfaceController.Instance.desktopMode)
								{
									InterfaceController.Instance.SetDesktopMode(false, true);
									return;
								}
								InterfaceController.Instance.SetDesktopMode(true, true);
								return;
							}
						}
					}
				}
				if (SessionData.Instance.enableFirstPersonMap && this.player.GetButtonDown("Map"))
				{
					if (!MapController.Instance.displayFirstPerson && !InterfaceController.Instance.desktopMode)
					{
						MapController.Instance.OpenMap(true, true);
						return;
					}
					Game.Log(string.Concat(new string[]
					{
						"Map disp fp: ",
						MapController.Instance.displayFirstPerson.ToString(),
						" && ",
						InterfaceController.Instance.desktopMode.ToString(),
						" ",
						InterfaceController.Instance.showDesktopMap.ToString()
					}), 2);
				}
				if (this.player.GetButtonDown("Notebook"))
				{
					InterfaceController.Instance.ToggleNotebook("", false);
					return;
				}
				if (InterfaceController.Instance.desktopMode || BioScreenController.Instance.isOpen)
				{
					if (CasePanelController.Instance.controllerMode)
					{
						if (this.player.GetButtonDown("NearestInteractable"))
						{
							int num = (int)CasePanelController.Instance.currentSelectMode;
							num++;
							if (num == 2 && InterfaceController.Instance.activeWindows.Count <= 0)
							{
								num++;
							}
							if (num > 3 || (num == 3 && !InterfaceController.Instance.minimapCanvas.gameObject.activeSelf))
							{
								num = 0;
							}
							if (UpgradesController.Instance.isOpen)
							{
								num = 0;
							}
							string text = "Primary ";
							CasePanelController.ControllerSelectMode controllerSelectMode = (CasePanelController.ControllerSelectMode)num;
							Game.Log(text + controllerSelectMode.ToString(), 2);
							CasePanelController.Instance.SetControllerMode(true, (CasePanelController.ControllerSelectMode)num);
							return;
						}
						if (this.player.GetButtonDown("NavigateRight"))
						{
							CasePanelController.Instance.ControllerNavigate(new Vector2(1f, 0f));
							return;
						}
						if (this.player.GetButtonDown("NavigateLeft"))
						{
							CasePanelController.Instance.ControllerNavigate(new Vector2(-1f, 0f));
							return;
						}
						if (this.player.GetButtonDown("NavigateUp"))
						{
							CasePanelController.Instance.ControllerNavigate(new Vector2(0f, 1f));
							return;
						}
						if (this.player.GetButtonDown("NavigateDown"))
						{
							CasePanelController.Instance.ControllerNavigate(new Vector2(0f, -1f));
							return;
						}
						if (this.player.GetButtonDown("SelectLeft"))
						{
							CasePanelController.Instance.ShoulderNavigate(false);
							return;
						}
						if (this.player.GetButtonDown("SelectRight"))
						{
							CasePanelController.Instance.ShoulderNavigate(true);
							return;
						}
						if (CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.windows)
						{
							if (CasePanelController.Instance.selectedWindow != null && (CasePanelController.Instance.selectedWindow.currentPinnedCaseElement == null || CasePanelController.Instance.selectedWindow.currentPinnedCaseElement.pinnedController == null || !CasePanelController.Instance.selectedWindow.currentPinnedCaseElement.pinnedController.isDragging))
							{
								Vector2 vector;
								vector..ctor(this.GetAxisRelative("MoveEvidenceAxisX"), this.GetAxisRelative("MoveEvidenceAxisY"));
								if (vector.magnitude > 0.15f)
								{
									vector *= 12f;
									CasePanelController.Instance.selectedWindow.SetAnchoredPosition(CasePanelController.Instance.selectedWindow.rect.anchoredPosition + vector);
								}
							}
						}
						else if (CasePanelController.Instance.currentSelectMode == CasePanelController.ControllerSelectMode.caseBoard && CasePanelController.Instance.selectedPinned != null)
						{
							Vector2 vector2;
							vector2..ctor(this.GetAxisRelative("MoveEvidenceAxisX"), this.GetAxisRelative("MoveEvidenceAxisY"));
							if (vector2.magnitude > 0.15f)
							{
								vector2 *= 12f;
								CasePanelController.Instance.selectedPinned.ForceDrag();
								CasePanelController.Instance.selectedPinned.dragController.ForceDragController(new Vector2(CasePanelController.Instance.selectedPinned.rect.localPosition.x, CasePanelController.Instance.selectedPinned.rect.localPosition.y) + vector2);
							}
						}
					}
					if (CasePanelController.Instance.controllerMode && CasePanelController.Instance.selectedPinned != null)
					{
						if (this.player.GetButtonDown("CreateString") && CasePanelController.Instance.customString == null)
						{
							Game.Log("Create string", 2);
							CasePanelController.Instance.CustomStringLinkSelection(CasePanelController.Instance.selectedPinned, true);
						}
					}
					else if (!CasePanelController.Instance.controllerMode && InterfaceController.Instance.selectedPinned.Count > 0 && this.player.GetButtonDown("CreateString") && CasePanelController.Instance.customString == null)
					{
						Game.Log("Create string", 2);
						CasePanelController.Instance.CustomStringLinkSelection(InterfaceController.Instance.selectedPinned[0], true);
					}
				}
				if (!PlayerApartmentController.Instance.furniturePlacementMode && !InterfaceController.Instance.playerTextInputActive && !CutSceneController.Instance.cutSceneActive && !Player.Instance.transitionActive && (MapController.Instance == null || !MapController.Instance.displayFirstPerson))
				{
					if (!InteractionController.Instance.dialogMode)
					{
						if (this.player.GetButtonDown("WeaponSelect"))
						{
							if (InteractionController.Instance.currentLookAtTransform != null && Player.Instance.isCrunchingDatabase && InteractionController.Instance.currentLookAtTransform.name == "Screen")
							{
								return;
							}
							Game.Log("Interface: Weapon select", 2);
							if (BioScreenController.Instance.isOpen && BioScreenController.Instance.inventoryDisplayProgress >= 1f)
							{
								BioScreenController.Instance.SetInventoryOpen(false, false, true);
								return;
							}
							if (!BioScreenController.Instance.isOpen && BioScreenController.Instance.inventoryDisplayProgress <= 0f)
							{
								BioScreenController.Instance.SetInventoryOpen(true, false, true);
								return;
							}
						}
						else if ((this.player.GetButtonDown("Menu") || (this.player.GetButtonDown("Secondary") && !this.player.GetButton("WeaponSelect") && (BioScreenController.Instance.hoveredSlot == null || BioScreenController.Instance.hoveredSlot == BioScreenController.Instance.selectedSlot))) && BioScreenController.Instance.isOpen && BioScreenController.Instance.inventoryDisplayProgress >= 1f)
						{
							Game.Log("Interface: Cancel inventory: " + this.player.GetButtonDown("Secondary").ToString() + " ", 2);
							BioScreenController.Instance.SetInventoryOpen(false, false, true);
							return;
						}
					}
					else if (BioScreenController.Instance.isOpen && BioScreenController.Instance.inventoryDisplayProgress >= 1f)
					{
						BioScreenController.Instance.SetInventoryOpen(false, false, true);
						return;
					}
				}
				if (this.player.GetButtonDown("QuickSave"))
				{
					if (MainMenuController.Instance.IsSaveGameAllowed())
					{
						this.StartQuickSaveAsync();
					}
					else
					{
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Unable to save at this time", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					}
				}
				if (Player.Instance.autoTravelActive && !InterfaceController.Instance.desktopMode && SessionData.Instance.play && this.player.GetButtonDown("Secondary"))
				{
					Player.Instance.EndAutoTravel();
					return;
				}
				if (this.mouseInputMode && BioScreenController.Instance.isOpen)
				{
					if (BioScreenController.Instance.hoveredSlot != null)
					{
						if (this.player.GetButtonDown("0"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("0");
						}
						else if (this.player.GetButtonDown("1"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("1");
						}
						else if (this.player.GetButtonDown("2"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("2");
						}
						else if (this.player.GetButtonDown("3"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("3");
						}
						else if (this.player.GetButtonDown("4"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("4");
						}
						else if (this.player.GetButtonDown("5"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("5");
						}
						else if (this.player.GetButtonDown("6"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("6");
						}
						else if (this.player.GetButtonDown("7"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("7");
						}
						else if (this.player.GetButtonDown("8"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("8");
						}
						else if (this.player.GetButtonDown("9"))
						{
							BioScreenController.Instance.hoveredSlot.SetHotKey("9");
						}
					}
					else if (BioScreenController.Instance.selectedSlot != null)
					{
						if (this.player.GetButtonDown("0"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("0");
						}
						else if (this.player.GetButtonDown("1"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("1");
						}
						else if (this.player.GetButtonDown("2"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("2");
						}
						else if (this.player.GetButtonDown("3"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("3");
						}
						else if (this.player.GetButtonDown("4"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("4");
						}
						else if (this.player.GetButtonDown("5"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("5");
						}
						else if (this.player.GetButtonDown("6"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("6");
						}
						else if (this.player.GetButtonDown("7"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("7");
						}
						else if (this.player.GetButtonDown("8"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("8");
						}
						else if (this.player.GetButtonDown("9"))
						{
							BioScreenController.Instance.selectedSlot.SetHotKey("9");
						}
					}
				}
				if (Game.Instance.devMode && this.player.GetButtonDown("ToggleScreenshotMode"))
				{
					Game.Instance.SetScreenshotMode(!Game.Instance.screenshotMode, false);
				}
			}
			if (this.player.GetButtonDown("Menu"))
			{
				if (!MainMenuController.Instance.mainMenuActive && CutSceneController.Instance.cutSceneActive)
				{
					CutSceneController.Instance.StopScene();
					return;
				}
				if (SessionData.Instance.startedGame && !SessionData.Instance.play && !MainMenuController.Instance.mainMenuActive && SessionData.Instance.enableUserPause)
				{
					SessionData.Instance.ResumeGame();
					return;
				}
				if (SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
				{
					if (!MainMenuController.Instance.mainMenuActive)
					{
						SessionData.Instance.PauseGame(true, false, true);
						MainMenuController.Instance.EnableMainMenu(true, true, false, MainMenuController.Component.mainMenuButtons);
						return;
					}
					MainMenuController.Instance.EnableMainMenu(false, true, true, MainMenuController.Component.mainMenuButtons);
					return;
				}
				else
				{
					if (BioScreenController.Instance.isOpen)
					{
						BioScreenController.Instance.SetInventoryOpen(false, true, true);
						return;
					}
					if (!SessionData.Instance.play && !SessionData.Instance.isFloorEdit && SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive) && !MainMenuController.Instance.mainMenuActive)
					{
						SessionData.Instance.ResumeGame();
						return;
					}
				}
			}
		}
		if (SessionData.Instance.play && SessionData.Instance.startedGame && !CutSceneController.Instance.cutSceneActive && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
		{
			if (this.player.GetButtonDown("Flashlight"))
			{
				FirstPersonItemController.Instance.ToggleFlashlight();
				return;
			}
			if (this.player.GetButtonDown("NearestInteractable") && !InteractionController.Instance.dialogMode && InteractionController.Instance.currentlyDragging == null)
			{
				InteractionController.Instance.UpdateNearbyInteractables();
				if (InteractionController.Instance.nearbyInteractables.Count > 0)
				{
					int num2 = 0;
					if (InteractionController.Instance.currentLookingAtInteractable != null)
					{
						num2 = InteractionController.Instance.nearbyInteractables.IndexOf(InteractionController.Instance.currentLookingAtInteractable.interactable);
						Game.Log("Debug: Index of " + num2.ToString() + " found: " + num2.ToString(), 2);
					}
					for (int j = 0; j < InteractionController.Instance.nearbyInteractables.Count; j++)
					{
						num2++;
						if (num2 >= InteractionController.Instance.nearbyInteractables.Count)
						{
							num2 = 0;
						}
						if (InteractionController.Instance.currentLookingAtInteractable == null || InteractionController.Instance.nearbyInteractables[num2] != InteractionController.Instance.currentLookingAtInteractable.interactable)
						{
							break;
						}
					}
					Interactable interactable = InteractionController.Instance.nearbyInteractables[num2];
					Game.Log("Debug: Focus index: " + interactable.GetName() + ": " + interactable.id.ToString(), 2);
					InteractionController.Instance.FocusOnInteractable(interactable);
					InteractionController.Instance.nearbyInteractablesHint = 0;
					return;
				}
			}
			if (this.player.GetButtonDown("Crouch") && (InteractionController.Instance.lockedInInteraction == null || InteractionController.Instance.carryingObject != null) && !Player.Instance.inAirVent && (!Player.Instance.transitionActive || InteractionController.Instance.carryingObject != null))
			{
				Player.Instance.SetCrouched(!Player.Instance.isCrouched, false);
				return;
			}
			if (Game.Instance.allowDraggableRagdolls && InteractionController.Instance.currentLookAtTransform != null && this.player.GetButtonDown("Primary") && (InteractionController.Instance.currentLookAtTransform.gameObject.layer == 24 || InteractionController.Instance.currentLookAtTransform.gameObject.layer == 7))
			{
				RigidbodyDragObject componentInParent = InteractionController.Instance.currentLookAtTransform.GetComponentInParent<RigidbodyDragObject>();
				if (componentInParent != null && componentInParent.ai != null && componentInParent.IsValidRagdollDragable())
				{
					componentInParent.OnAttemptPlayerInteraction();
				}
			}
			if (InteractionController.Instance.currentlyDragging != null && !this.player.GetButton("Primary"))
			{
				InteractionController.Instance.currentlyDragging.CancelDrag();
			}
			if (this.mouseInputMode && !BioScreenController.Instance.isOpen)
			{
				FirstPersonItemController.InventorySlot inventorySlot = null;
				if (this.player.GetButtonDown("0"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "0");
				}
				else if (this.player.GetButtonDown("1"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "1");
				}
				else if (this.player.GetButtonDown("2"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "2");
				}
				else if (this.player.GetButtonDown("3"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "3");
				}
				else if (this.player.GetButtonDown("4"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "4");
				}
				else if (this.player.GetButtonDown("5"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "5");
				}
				else if (this.player.GetButtonDown("6"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "6");
				}
				else if (this.player.GetButtonDown("7"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "7");
				}
				else if (this.player.GetButtonDown("8"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "8");
				}
				else if (this.player.GetButtonDown("9"))
				{
					inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.hotkey == "9");
				}
				if (inventorySlot != null)
				{
					if (BioScreenController.Instance.selectedSlot == inventorySlot)
					{
						BioScreenController.Instance.SelectSlot(FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, false);
					}
					else
					{
						BioScreenController.Instance.SelectSlot(inventorySlot, false, false);
					}
				}
			}
		}
		if (Game.Instance.demoAutoReset && Game.Instance.demoMode && this.player.GetAnyButton())
		{
			SessionData.Instance.autoResetTimer = 0f;
		}
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x000EEE24 File Offset: 0x000ED024
	public void StartQuickSaveAsync()
	{
		InputController.<StartQuickSaveAsync>d__18 <StartQuickSaveAsync>d__;
		<StartQuickSaveAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartQuickSaveAsync>d__.<>1__state = -1;
		<StartQuickSaveAsync>d__.<>t__builder.Start<InputController.<StartQuickSaveAsync>d__18>(ref <StartQuickSaveAsync>d__);
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x000EEE53 File Offset: 0x000ED053
	public void ResetCurrentButtonDown()
	{
		this.currentButtonDown = null;
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x000EEE5C File Offset: 0x000ED05C
	public float GetAxisRelative(string actionId)
	{
		if (this.player == null)
		{
			return 0f;
		}
		float axis = this.player.GetAxis(actionId);
		if (Mathf.Abs(axis) < 0.085f)
		{
			return 0f;
		}
		SessionData.Instance.autoPauseTimer = 0f;
		SessionData.Instance.autoResetTimer = 0f;
		return axis;
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x000EEEB8 File Offset: 0x000ED0B8
	public void SetMouseInputMode(bool val, bool forceUpdate = false)
	{
		if (this.mouseInputMode != val || forceUpdate)
		{
			this.mouseInputMode = val;
			Game.Log("Player: Set mouse input mode: " + this.mouseInputMode.ToString() + " (Saving to player prefs)", 2);
			PlayerPrefsController.GameSetting gameSetting = PlayerPrefsController.Instance.gameSettingControls.Find((PlayerPrefsController.GameSetting item) => item.identifier == "controlMethod");
			if (this.mouseInputMode)
			{
				gameSetting.intValue = 1;
			}
			else
			{
				gameSetting.intValue = 0;
			}
			PlayerPrefsController.Instance.OnToggleChanged("controlMethod", false, null);
			try
			{
				foreach (ControllerMap controllerMap in Enumerable.ToList<ControllerMap>(this.player.controllers.maps.GetAllMaps()))
				{
					if (controllerMap.controllerType == null || controllerMap.controllerType == 1)
					{
						controllerMap.enabled = this.mouseInputMode;
					}
					else
					{
						controllerMap.enabled = !this.mouseInputMode;
					}
				}
			}
			catch
			{
				Game.Log("Player: Unable to enabled/disable appropriate controller maps. The player may not be yet initialised...", 2);
			}
			InterfaceController.Instance.ClearAllMouseOverElements();
			if (!this.mouseInputMode)
			{
				this.SetCursorVisible(false);
			}
			else if (!this.cursorVisible)
			{
				this.SetCursorVisible(true);
				if (InterfaceController.Instance.selectedElement != null)
				{
					InterfaceController.Instance.selectedElement.OnDeselect();
				}
			}
			if (InterfaceController.Instance.desktopMode)
			{
				CasePanelController.Instance.SetControllerMode(!this.mouseInputMode, CasePanelController.Instance.currentSelectMode);
			}
			if (this.OnInputModeChange != null)
			{
				this.OnInputModeChange();
			}
		}
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x000EF07C File Offset: 0x000ED27C
	public void SetCursorVisible(bool val)
	{
		this.cursorVisible = val;
		if (Cursor.visible != this.cursorVisible)
		{
			if (this.mouseInputMode && this.cursorVisible)
			{
				Game.Log("Menu: Set cursor visible: " + true.ToString(), 2);
				Cursor.visible = true;
				return;
			}
			Game.Log("Menu: Set cursor visible: " + false.ToString(), 2);
			Cursor.visible = false;
		}
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x000EF0EC File Offset: 0x000ED2EC
	public void SetCursorLock(bool value)
	{
		Player.Instance.fps.m_MouseLook.lockCursor = value;
		Game.Log("Menu: Set cursor lock: " + Player.Instance.fps.m_MouseLook.lockCursor.ToString(), 2);
		if (!Player.Instance.fps.m_MouseLook.lockCursor)
		{
			Cursor.lockState = 0;
			return;
		}
		Cursor.lockState = 1;
	}

	// Token: 0x040014FE RID: 5374
	[Header("Setup")]
	public bool enableInput = true;

	// Token: 0x040014FF RID: 5375
	[NonSerialized]
	public Player player;

	// Token: 0x04001500 RID: 5376
	public AnimationCurve nearestLookAtCurve;

	// Token: 0x04001501 RID: 5377
	[Header("Menu")]
	public ControllerType lastActiveController = 20;

	// Token: 0x04001502 RID: 5378
	public bool mouseInputMode = true;

	// Token: 0x04001503 RID: 5379
	private bool initalInputModeSet;

	// Token: 0x04001504 RID: 5380
	public bool cursorVisible = true;

	// Token: 0x04001505 RID: 5381
	private ButtonController currentButtonDown;

	// Token: 0x04001506 RID: 5382
	private float controlFallbackCheck;

	// Token: 0x04001508 RID: 5384
	private static InputController _instance;

	// Token: 0x020002E4 RID: 740
	// (Invoke) Token: 0x060010CA RID: 4298
	public delegate void InputModeChange();
}
