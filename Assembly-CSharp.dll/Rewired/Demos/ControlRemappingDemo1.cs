using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x0200089F RID: 2207
	[AddComponentMenu("")]
	public class ControlRemappingDemo1 : MonoBehaviour
	{
		// Token: 0x06002EF5 RID: 12021 RVA: 0x0020DCBA File Offset: 0x0020BEBA
		private void Awake()
		{
			this.inputMapper.options.timeout = 5f;
			this.inputMapper.options.ignoreMouseXAxis = true;
			this.inputMapper.options.ignoreMouseYAxis = true;
			this.Initialize();
		}

		// Token: 0x06002EF6 RID: 12022 RVA: 0x0020DCF9 File Offset: 0x0020BEF9
		private void OnEnable()
		{
			this.Subscribe();
		}

		// Token: 0x06002EF7 RID: 12023 RVA: 0x0020DD01 File Offset: 0x0020BF01
		private void OnDisable()
		{
			this.Unsubscribe();
		}

		// Token: 0x06002EF8 RID: 12024 RVA: 0x0020DD0C File Offset: 0x0020BF0C
		private void Initialize()
		{
			this.dialog = new ControlRemappingDemo1.DialogHelper();
			this.actionQueue = new Queue<ControlRemappingDemo1.QueueEntry>();
			this.selectedController = new ControlRemappingDemo1.ControllerSelection();
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickConnected);
			ReInput.ControllerPreDisconnectEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickPreDisconnect);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickDisconnected);
			this.ResetAll();
			this.initialized = true;
			ReInput.userDataStore.Load();
			if (ReInput.unityJoystickIdentificationRequired)
			{
				this.IdentifyAllJoysticks();
			}
		}

		// Token: 0x06002EF9 RID: 12025 RVA: 0x0020DD94 File Offset: 0x0020BF94
		private void Setup()
		{
			if (this.setupFinished)
			{
				return;
			}
			this.style_wordWrap = new GUIStyle(GUI.skin.label);
			this.style_wordWrap.wordWrap = true;
			this.style_centeredBox = new GUIStyle(GUI.skin.box);
			this.style_centeredBox.alignment = 4;
			this.setupFinished = true;
		}

		// Token: 0x06002EFA RID: 12026 RVA: 0x0020DDF3 File Offset: 0x0020BFF3
		private void Subscribe()
		{
			this.Unsubscribe();
			this.inputMapper.ConflictFoundEvent += new Action<InputMapper.ConflictFoundEventData>(this.OnConflictFound);
			this.inputMapper.StoppedEvent += new Action<InputMapper.StoppedEventData>(this.OnStopped);
		}

		// Token: 0x06002EFB RID: 12027 RVA: 0x0020DE29 File Offset: 0x0020C029
		private void Unsubscribe()
		{
			this.inputMapper.RemoveAllEventListeners();
		}

		// Token: 0x06002EFC RID: 12028 RVA: 0x0020DE38 File Offset: 0x0020C038
		public void OnGUI()
		{
			if (!this.initialized)
			{
				return;
			}
			this.Setup();
			this.HandleMenuControl();
			if (!this.showMenu)
			{
				this.DrawInitialScreen();
				return;
			}
			this.SetGUIStateStart();
			this.ProcessQueue();
			this.DrawPage();
			this.ShowDialog();
			this.SetGUIStateEnd();
			this.busy = false;
		}

		// Token: 0x06002EFD RID: 12029 RVA: 0x0020DE90 File Offset: 0x0020C090
		private void HandleMenuControl()
		{
			if (this.dialog.enabled)
			{
				return;
			}
			if (Event.current.type == 8 && ReInput.players.GetSystemPlayer().GetButtonDown("Menu"))
			{
				if (this.showMenu)
				{
					ReInput.userDataStore.Save();
					this.Close();
					return;
				}
				this.Open();
			}
		}

		// Token: 0x06002EFE RID: 12030 RVA: 0x0020DEED File Offset: 0x0020C0ED
		private void Close()
		{
			this.ClearWorkingVars();
			this.showMenu = false;
		}

		// Token: 0x06002EFF RID: 12031 RVA: 0x0020DEFC File Offset: 0x0020C0FC
		private void Open()
		{
			this.showMenu = true;
		}

		// Token: 0x06002F00 RID: 12032 RVA: 0x0020DF08 File Offset: 0x0020C108
		private void DrawInitialScreen()
		{
			ActionElementMap firstElementMapWithAction = ReInput.players.GetSystemPlayer().controllers.maps.GetFirstElementMapWithAction("Menu", true);
			GUIContent guicontent;
			if (firstElementMapWithAction != null)
			{
				guicontent = new GUIContent("Press " + firstElementMapWithAction.elementIdentifierName + " to open the menu.");
			}
			else
			{
				guicontent = new GUIContent("There is no element assigned to open the menu!");
			}
			GUILayout.BeginArea(this.GetScreenCenteredRect(300f, 50f));
			GUILayout.Box(guicontent, this.style_centeredBox, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			GUILayout.EndArea();
		}

		// Token: 0x06002F01 RID: 12033 RVA: 0x0020DFA0 File Offset: 0x0020C1A0
		private void DrawPage()
		{
			if (GUI.enabled != this.pageGUIState)
			{
				GUI.enabled = this.pageGUIState;
			}
			GUILayout.BeginArea(new Rect(((float)Screen.width - (float)Screen.width * 0.9f) * 0.5f, ((float)Screen.height - (float)Screen.height * 0.9f) * 0.5f, (float)Screen.width * 0.9f, (float)Screen.height * 0.9f));
			this.DrawPlayerSelector();
			this.DrawJoystickSelector();
			this.DrawMouseAssignment();
			this.DrawControllerSelector();
			this.DrawCalibrateButton();
			this.DrawMapCategories();
			this.actionScrollPos = GUILayout.BeginScrollView(this.actionScrollPos, Array.Empty<GUILayoutOption>());
			this.DrawCategoryActions();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		// Token: 0x06002F02 RID: 12034 RVA: 0x0020E064 File Offset: 0x0020C264
		private void DrawPlayerSelector()
		{
			if (ReInput.players.allPlayerCount == 0)
			{
				GUILayout.Label("There are no players.", Array.Empty<GUILayoutOption>());
				return;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Players:", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			foreach (Player player in ReInput.players.GetPlayers(true))
			{
				if (this.selectedPlayer == null)
				{
					this.selectedPlayer = player;
				}
				bool flag = player == this.selectedPlayer;
				bool flag2 = GUILayout.Toggle(flag, (player.descriptiveName != string.Empty) ? player.descriptiveName : player.name, "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag2 != flag && flag2)
				{
					this.selectedPlayer = player;
					this.selectedController.Clear();
					this.selectedMapCategoryId = -1;
				}
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x06002F03 RID: 12035 RVA: 0x0020E178 File Offset: 0x0020C378
		private void DrawMouseAssignment()
		{
			bool enabled = GUI.enabled;
			if (this.selectedPlayer == null)
			{
				GUI.enabled = false;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Assign Mouse:", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag = this.selectedPlayer != null && this.selectedPlayer.controllers.hasMouse;
			bool flag2 = GUILayout.Toggle(flag, "Assign Mouse", "Button", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag2 != flag)
			{
				if (flag2)
				{
					this.selectedPlayer.controllers.hasMouse = true;
					using (IEnumerator<Player> enumerator = ReInput.players.Players.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Player player = enumerator.Current;
							if (player != this.selectedPlayer)
							{
								player.controllers.hasMouse = false;
							}
						}
						goto IL_E9;
					}
				}
				this.selectedPlayer.controllers.hasMouse = false;
			}
			IL_E9:
			GUILayout.EndHorizontal();
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		// Token: 0x06002F04 RID: 12036 RVA: 0x0020E294 File Offset: 0x0020C494
		private void DrawJoystickSelector()
		{
			bool enabled = GUI.enabled;
			if (this.selectedPlayer == null)
			{
				GUI.enabled = false;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Assign Joysticks:", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag = this.selectedPlayer == null || this.selectedPlayer.controllers.joystickCount == 0;
			bool flag2 = GUILayout.Toggle(flag, "None", "Button", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag2 != flag)
			{
				this.selectedPlayer.controllers.ClearControllersOfType(2);
				this.ControllerSelectionChanged();
			}
			if (this.selectedPlayer != null)
			{
				foreach (Joystick joystick in ReInput.controllers.Joysticks)
				{
					flag = this.selectedPlayer.controllers.ContainsController(joystick);
					flag2 = GUILayout.Toggle(flag, joystick.name, "Button", new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					});
					if (flag2 != flag)
					{
						this.EnqueueAction(new ControlRemappingDemo1.JoystickAssignmentChange(this.selectedPlayer.id, joystick.id, flag2));
					}
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		// Token: 0x06002F05 RID: 12037 RVA: 0x0020E3F0 File Offset: 0x0020C5F0
		private void DrawControllerSelector()
		{
			if (this.selectedPlayer == null)
			{
				return;
			}
			bool enabled = GUI.enabled;
			GUILayout.Space(15f);
			GUILayout.Label("Controller to Map:", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (!this.selectedController.hasSelection)
			{
				this.selectedController.Set(0, 0);
				this.ControllerSelectionChanged();
			}
			bool flag = this.selectedController.type == 0;
			if (GUILayout.Toggle(flag, "Keyboard", "Button", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}) != flag)
			{
				this.selectedController.Set(0, 0);
				this.ControllerSelectionChanged();
			}
			if (!this.selectedPlayer.controllers.hasMouse)
			{
				GUI.enabled = false;
			}
			flag = (this.selectedController.type == 1);
			if (GUILayout.Toggle(flag, "Mouse", "Button", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}) != flag)
			{
				this.selectedController.Set(0, 1);
				this.ControllerSelectionChanged();
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
			foreach (Joystick joystick in this.selectedPlayer.controllers.Joysticks)
			{
				flag = (this.selectedController.type == 2 && this.selectedController.id == joystick.id);
				if (GUILayout.Toggle(flag, joystick.name, "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}) != flag)
				{
					this.selectedController.Set(joystick.id, 2);
					this.ControllerSelectionChanged();
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		// Token: 0x06002F06 RID: 12038 RVA: 0x0020E5C4 File Offset: 0x0020C7C4
		private void DrawCalibrateButton()
		{
			if (this.selectedPlayer == null)
			{
				return;
			}
			bool enabled = GUI.enabled;
			GUILayout.Space(10f);
			Controller controller = this.selectedController.hasSelection ? this.selectedPlayer.controllers.GetController(this.selectedController.type, this.selectedController.id) : null;
			if (controller == null || this.selectedController.type != 2)
			{
				GUI.enabled = false;
				GUILayout.Button("Select a controller to calibrate", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}
			else if (GUILayout.Button("Calibrate " + controller.name, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				Joystick joystick = controller as Joystick;
				if (joystick != null)
				{
					CalibrationMap calibrationMap = joystick.calibrationMap;
					if (calibrationMap != null)
					{
						this.EnqueueAction(new ControlRemappingDemo1.Calibration(this.selectedPlayer, joystick, calibrationMap));
					}
				}
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		// Token: 0x06002F07 RID: 12039 RVA: 0x0020E6C0 File Offset: 0x0020C8C0
		private void DrawMapCategories()
		{
			if (this.selectedPlayer == null)
			{
				return;
			}
			if (!this.selectedController.hasSelection)
			{
				return;
			}
			bool enabled = GUI.enabled;
			GUILayout.Space(15f);
			GUILayout.Label("Categories:", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			foreach (InputMapCategory inputMapCategory in ReInput.mapping.UserAssignableMapCategories)
			{
				if (!this.selectedPlayer.controllers.maps.ContainsMapInCategory(this.selectedController.type, inputMapCategory.id))
				{
					GUI.enabled = false;
				}
				else if (this.selectedMapCategoryId < 0)
				{
					this.selectedMapCategoryId = inputMapCategory.id;
					this.selectedMap = this.selectedPlayer.controllers.maps.GetFirstMapInCategory(this.selectedController.type, this.selectedController.id, inputMapCategory.id);
				}
				bool flag = inputMapCategory.id == this.selectedMapCategoryId;
				if (GUILayout.Toggle(flag, (inputMapCategory.descriptiveName != string.Empty) ? inputMapCategory.descriptiveName : inputMapCategory.name, "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}) != flag)
				{
					this.selectedMapCategoryId = inputMapCategory.id;
					this.selectedMap = this.selectedPlayer.controllers.maps.GetFirstMapInCategory(this.selectedController.type, this.selectedController.id, inputMapCategory.id);
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		// Token: 0x06002F08 RID: 12040 RVA: 0x0020E894 File Offset: 0x0020CA94
		private void DrawCategoryActions()
		{
			if (this.selectedPlayer == null)
			{
				return;
			}
			if (this.selectedMapCategoryId < 0)
			{
				return;
			}
			bool enabled = GUI.enabled;
			if (this.selectedMap == null)
			{
				return;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Actions:", Array.Empty<GUILayoutOption>());
			InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(this.selectedMapCategoryId);
			if (mapCategory == null)
			{
				return;
			}
			InputCategory actionCategory = ReInput.mapping.GetActionCategory(mapCategory.name);
			if (actionCategory == null)
			{
				return;
			}
			float num = 150f;
			foreach (InputAction inputAction in ReInput.mapping.ActionsInCategory(actionCategory.id))
			{
				string text = (inputAction.descriptiveName != string.Empty) ? inputAction.descriptiveName : inputAction.name;
				if (inputAction.type == 1)
				{
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Label(text, new GUILayoutOption[]
					{
						GUILayout.Width(num)
					});
					this.DrawAddActionMapButton(this.selectedPlayer.id, inputAction, 1, this.selectedController, this.selectedMap);
					foreach (ActionElementMap actionElementMap in this.selectedMap.AllMaps)
					{
						if (actionElementMap.actionId == inputAction.id)
						{
							this.DrawActionAssignmentButton(this.selectedPlayer.id, inputAction, 1, this.selectedController, this.selectedMap, actionElementMap);
						}
					}
					GUILayout.EndHorizontal();
				}
				else if (inputAction.type == null)
				{
					if (this.selectedController.type != null)
					{
						GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
						GUILayout.Label(text, new GUILayoutOption[]
						{
							GUILayout.Width(num)
						});
						this.DrawAddActionMapButton(this.selectedPlayer.id, inputAction, 0, this.selectedController, this.selectedMap);
						foreach (ActionElementMap actionElementMap2 in this.selectedMap.AllMaps)
						{
							if (actionElementMap2.actionId == inputAction.id && actionElementMap2.elementType != 1 && actionElementMap2.axisType != 2)
							{
								this.DrawActionAssignmentButton(this.selectedPlayer.id, inputAction, 0, this.selectedController, this.selectedMap, actionElementMap2);
								this.DrawInvertButton(this.selectedPlayer.id, inputAction, 0, this.selectedController, this.selectedMap, actionElementMap2);
							}
						}
						GUILayout.EndHorizontal();
					}
					string text2 = (inputAction.positiveDescriptiveName != string.Empty) ? inputAction.positiveDescriptiveName : (inputAction.descriptiveName + " +");
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Label(text2, new GUILayoutOption[]
					{
						GUILayout.Width(num)
					});
					this.DrawAddActionMapButton(this.selectedPlayer.id, inputAction, 1, this.selectedController, this.selectedMap);
					foreach (ActionElementMap actionElementMap3 in this.selectedMap.AllMaps)
					{
						if (actionElementMap3.actionId == inputAction.id && actionElementMap3.axisContribution == null && actionElementMap3.axisType != 1)
						{
							this.DrawActionAssignmentButton(this.selectedPlayer.id, inputAction, 1, this.selectedController, this.selectedMap, actionElementMap3);
						}
					}
					GUILayout.EndHorizontal();
					string text3 = (inputAction.negativeDescriptiveName != string.Empty) ? inputAction.negativeDescriptiveName : (inputAction.descriptiveName + " -");
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Label(text3, new GUILayoutOption[]
					{
						GUILayout.Width(num)
					});
					this.DrawAddActionMapButton(this.selectedPlayer.id, inputAction, 2, this.selectedController, this.selectedMap);
					foreach (ActionElementMap actionElementMap4 in this.selectedMap.AllMaps)
					{
						if (actionElementMap4.actionId == inputAction.id && actionElementMap4.axisContribution == 1 && actionElementMap4.axisType != 1)
						{
							this.DrawActionAssignmentButton(this.selectedPlayer.id, inputAction, 2, this.selectedController, this.selectedMap, actionElementMap4);
						}
					}
					GUILayout.EndHorizontal();
				}
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		// Token: 0x06002F09 RID: 12041 RVA: 0x0020ED88 File Offset: 0x0020CF88
		private void DrawActionAssignmentButton(int playerId, InputAction action, AxisRange actionRange, ControlRemappingDemo1.ControllerSelection controller, ControllerMap controllerMap, ActionElementMap elementMap)
		{
			if (GUILayout.Button(elementMap.elementIdentifierName, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false),
				GUILayout.MinWidth(30f)
			}))
			{
				InputMapper.Context context = new InputMapper.Context
				{
					actionId = action.id,
					actionRange = actionRange,
					controllerMap = controllerMap,
					actionElementMapToReplace = elementMap
				};
				this.EnqueueAction(new ControlRemappingDemo1.ElementAssignmentChange(ControlRemappingDemo1.ElementAssignmentChangeType.ReassignOrRemove, context));
				this.startListening = true;
			}
			GUILayout.Space(4f);
		}

		// Token: 0x06002F0A RID: 12042 RVA: 0x0020EE08 File Offset: 0x0020D008
		private void DrawInvertButton(int playerId, InputAction action, Pole actionAxisContribution, ControlRemappingDemo1.ControllerSelection controller, ControllerMap controllerMap, ActionElementMap elementMap)
		{
			bool invert = elementMap.invert;
			bool flag = GUILayout.Toggle(invert, "Invert", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag != invert)
			{
				elementMap.invert = flag;
			}
			GUILayout.Space(10f);
		}

		// Token: 0x06002F0B RID: 12043 RVA: 0x0020EE50 File Offset: 0x0020D050
		private void DrawAddActionMapButton(int playerId, InputAction action, AxisRange actionRange, ControlRemappingDemo1.ControllerSelection controller, ControllerMap controllerMap)
		{
			if (GUILayout.Button("Add...", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				InputMapper.Context context = new InputMapper.Context
				{
					actionId = action.id,
					actionRange = actionRange,
					controllerMap = controllerMap
				};
				this.EnqueueAction(new ControlRemappingDemo1.ElementAssignmentChange(ControlRemappingDemo1.ElementAssignmentChangeType.Add, context));
				this.startListening = true;
			}
			GUILayout.Space(10f);
		}

		// Token: 0x06002F0C RID: 12044 RVA: 0x0020EEB7 File Offset: 0x0020D0B7
		private void ShowDialog()
		{
			this.dialog.Update();
		}

		// Token: 0x06002F0D RID: 12045 RVA: 0x0020EEC4 File Offset: 0x0020D0C4
		private void DrawModalWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.dialog.DrawConfirmButton("Okay");
			GUILayout.FlexibleSpace();
			this.dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}

		// Token: 0x06002F0E RID: 12046 RVA: 0x0020EF30 File Offset: 0x0020D130
		private void DrawModalWindow_OkayOnly(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.dialog.DrawConfirmButton("Okay");
			GUILayout.EndHorizontal();
		}

		// Token: 0x06002F0F RID: 12047 RVA: 0x0020EF8C File Offset: 0x0020D18C
		private void DrawElementAssignmentWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			ControlRemappingDemo1.ElementAssignmentChange elementAssignmentChange = this.actionQueue.Peek() as ControlRemappingDemo1.ElementAssignmentChange;
			if (elementAssignmentChange == null)
			{
				this.dialog.Cancel();
				return;
			}
			float num;
			if (!this.dialog.busy)
			{
				if (this.startListening && this.inputMapper.status == null)
				{
					this.inputMapper.Start(elementAssignmentChange.context);
					this.startListening = false;
				}
				if (this.conflictFoundEventData != null)
				{
					this.dialog.Confirm();
					return;
				}
				num = this.inputMapper.timeRemaining;
				if (num == 0f)
				{
					this.dialog.Cancel();
					return;
				}
			}
			else
			{
				num = this.inputMapper.options.timeout;
			}
			GUILayout.Label("Assignment will be canceled in " + ((int)Mathf.Ceil(num)).ToString() + "...", this.style_wordWrap, Array.Empty<GUILayoutOption>());
		}

		// Token: 0x06002F10 RID: 12048 RVA: 0x0020F098 File Offset: 0x0020D298
		private void DrawElementAssignmentProtectedConflictWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			if (!(this.actionQueue.Peek() is ControlRemappingDemo1.ElementAssignmentChange))
			{
				this.dialog.Cancel();
				return;
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.dialog.DrawConfirmButton(ControlRemappingDemo1.UserResponse.Custom1, "Add");
			GUILayout.FlexibleSpace();
			this.dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}

		// Token: 0x06002F11 RID: 12049 RVA: 0x0020F124 File Offset: 0x0020D324
		private void DrawElementAssignmentNormalConflictWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			if (!(this.actionQueue.Peek() is ControlRemappingDemo1.ElementAssignmentChange))
			{
				this.dialog.Cancel();
				return;
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.dialog.DrawConfirmButton(ControlRemappingDemo1.UserResponse.Confirm, "Replace");
			GUILayout.FlexibleSpace();
			this.dialog.DrawConfirmButton(ControlRemappingDemo1.UserResponse.Custom1, "Add");
			GUILayout.FlexibleSpace();
			this.dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}

		// Token: 0x06002F12 RID: 12050 RVA: 0x0020F1C4 File Offset: 0x0020D3C4
		private void DrawReassignOrRemoveElementAssignmentWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.dialog.DrawConfirmButton("Reassign");
			GUILayout.FlexibleSpace();
			this.dialog.DrawCancelButton("Remove");
			GUILayout.EndHorizontal();
		}

		// Token: 0x06002F13 RID: 12051 RVA: 0x0020F234 File Offset: 0x0020D434
		private void DrawFallbackJoystickIdentificationWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			ControlRemappingDemo1.FallbackJoystickIdentification fallbackJoystickIdentification = this.actionQueue.Peek() as ControlRemappingDemo1.FallbackJoystickIdentification;
			if (fallbackJoystickIdentification == null)
			{
				this.dialog.Cancel();
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.Label("Press any button or axis on \"" + fallbackJoystickIdentification.joystickName + "\" now.", this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Skip", Array.Empty<GUILayoutOption>()))
			{
				this.dialog.Cancel();
				return;
			}
			if (this.dialog.busy)
			{
				return;
			}
			if (!ReInput.controllers.SetUnityJoystickIdFromAnyButtonOrAxisPress(fallbackJoystickIdentification.joystickId, 0.8f, false))
			{
				return;
			}
			this.dialog.Confirm();
		}

		// Token: 0x06002F14 RID: 12052 RVA: 0x0020F304 File Offset: 0x0020D504
		private void DrawCalibrationWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			ControlRemappingDemo1.Calibration calibration = this.actionQueue.Peek() as ControlRemappingDemo1.Calibration;
			if (calibration == null)
			{
				this.dialog.Cancel();
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, Array.Empty<GUILayoutOption>());
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool enabled = GUI.enabled;
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(200f)
			});
			this.calibrateScrollPos = GUILayout.BeginScrollView(this.calibrateScrollPos, Array.Empty<GUILayoutOption>());
			if (calibration.recording)
			{
				GUI.enabled = false;
			}
			IList<ControllerElementIdentifier> axisElementIdentifiers = calibration.joystick.AxisElementIdentifiers;
			for (int i = 0; i < axisElementIdentifiers.Count; i++)
			{
				ControllerElementIdentifier controllerElementIdentifier = axisElementIdentifiers[i];
				bool flag = calibration.selectedElementIdentifierId == controllerElementIdentifier.id;
				bool flag2 = GUILayout.Toggle(flag, controllerElementIdentifier.name, "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag != flag2)
				{
					calibration.selectedElementIdentifierId = controllerElementIdentifier.id;
				}
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(200f)
			});
			if (calibration.selectedElementIdentifierId >= 0)
			{
				float axisRawById = calibration.joystick.GetAxisRawById(calibration.selectedElementIdentifierId);
				GUILayout.Label("Raw Value: " + axisRawById.ToString(), Array.Empty<GUILayoutOption>());
				int axisIndexById = calibration.joystick.GetAxisIndexById(calibration.selectedElementIdentifierId);
				AxisCalibration axis = calibration.calibrationMap.GetAxis(axisIndexById);
				GUILayout.Label("Calibrated Value: " + calibration.joystick.GetAxisById(calibration.selectedElementIdentifierId).ToString(), Array.Empty<GUILayoutOption>());
				GUILayout.Label("Zero: " + axis.calibratedZero.ToString(), Array.Empty<GUILayoutOption>());
				GUILayout.Label("Min: " + axis.calibratedMin.ToString(), Array.Empty<GUILayoutOption>());
				GUILayout.Label("Max: " + axis.calibratedMax.ToString(), Array.Empty<GUILayoutOption>());
				GUILayout.Label("Dead Zone: " + axis.deadZone.ToString(), Array.Empty<GUILayoutOption>());
				GUILayout.Space(15f);
				bool flag3 = GUILayout.Toggle(axis.enabled, "Enabled", "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (axis.enabled != flag3)
				{
					axis.enabled = flag3;
				}
				GUILayout.Space(10f);
				bool flag4 = GUILayout.Toggle(calibration.recording, "Record Min/Max", "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag4 != calibration.recording)
				{
					if (flag4)
					{
						axis.calibratedMax = 0f;
						axis.calibratedMin = 0f;
					}
					calibration.recording = flag4;
				}
				if (calibration.recording)
				{
					axis.calibratedMin = Mathf.Min(new float[]
					{
						axis.calibratedMin,
						axisRawById,
						axis.calibratedMin
					});
					axis.calibratedMax = Mathf.Max(new float[]
					{
						axis.calibratedMax,
						axisRawById,
						axis.calibratedMax
					});
					GUI.enabled = false;
				}
				if (GUILayout.Button("Set Zero", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.calibratedZero = axisRawById;
				}
				if (GUILayout.Button("Set Dead Zone", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.deadZone = axisRawById;
				}
				bool flag5 = GUILayout.Toggle(axis.invert, "Invert", "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (axis.invert != flag5)
				{
					axis.invert = flag5;
				}
				GUILayout.Space(10f);
				if (GUILayout.Button("Reset", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.Reset();
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}
			else
			{
				GUILayout.Label("Select an axis to begin.", Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			if (calibration.recording)
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button("Close", Array.Empty<GUILayoutOption>()))
			{
				this.calibrateScrollPos = default(Vector2);
				this.dialog.Confirm();
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		// Token: 0x06002F15 RID: 12053 RVA: 0x0020F7A4 File Offset: 0x0020D9A4
		private void DialogResultCallback(int queueActionId, ControlRemappingDemo1.UserResponse response)
		{
			foreach (ControlRemappingDemo1.QueueEntry queueEntry in this.actionQueue)
			{
				if (queueEntry.id == queueActionId)
				{
					if (response != ControlRemappingDemo1.UserResponse.Cancel)
					{
						queueEntry.Confirm(response);
						break;
					}
					queueEntry.Cancel();
					break;
				}
			}
		}

		// Token: 0x06002F16 RID: 12054 RVA: 0x0020F810 File Offset: 0x0020DA10
		private Rect GetScreenCenteredRect(float width, float height)
		{
			return new Rect((float)Screen.width * 0.5f - width * 0.5f, (float)((double)Screen.height * 0.5 - (double)(height * 0.5f)), width, height);
		}

		// Token: 0x06002F17 RID: 12055 RVA: 0x0020F848 File Offset: 0x0020DA48
		private void EnqueueAction(ControlRemappingDemo1.QueueEntry entry)
		{
			if (entry == null)
			{
				return;
			}
			this.busy = true;
			GUI.enabled = false;
			this.actionQueue.Enqueue(entry);
		}

		// Token: 0x06002F18 RID: 12056 RVA: 0x0020F868 File Offset: 0x0020DA68
		private void ProcessQueue()
		{
			if (this.dialog.enabled)
			{
				return;
			}
			if (this.busy || this.actionQueue.Count == 0)
			{
				return;
			}
			while (this.actionQueue.Count > 0)
			{
				ControlRemappingDemo1.QueueEntry queueEntry = this.actionQueue.Peek();
				bool flag = false;
				switch (queueEntry.queueActionType)
				{
				case ControlRemappingDemo1.QueueActionType.JoystickAssignment:
					flag = this.ProcessJoystickAssignmentChange((ControlRemappingDemo1.JoystickAssignmentChange)queueEntry);
					break;
				case ControlRemappingDemo1.QueueActionType.ElementAssignment:
					flag = this.ProcessElementAssignmentChange((ControlRemappingDemo1.ElementAssignmentChange)queueEntry);
					break;
				case ControlRemappingDemo1.QueueActionType.FallbackJoystickIdentification:
					flag = this.ProcessFallbackJoystickIdentification((ControlRemappingDemo1.FallbackJoystickIdentification)queueEntry);
					break;
				case ControlRemappingDemo1.QueueActionType.Calibrate:
					flag = this.ProcessCalibration((ControlRemappingDemo1.Calibration)queueEntry);
					break;
				}
				if (!flag)
				{
					break;
				}
				this.actionQueue.Dequeue();
			}
		}

		// Token: 0x06002F19 RID: 12057 RVA: 0x0020F924 File Offset: 0x0020DB24
		private bool ProcessJoystickAssignmentChange(ControlRemappingDemo1.JoystickAssignmentChange entry)
		{
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			Player player = ReInput.players.GetPlayer(entry.playerId);
			if (player == null)
			{
				return true;
			}
			if (!entry.assign)
			{
				player.controllers.RemoveController(2, entry.joystickId);
				this.ControllerSelectionChanged();
				return true;
			}
			if (player.controllers.ContainsController(2, entry.joystickId))
			{
				return true;
			}
			if (!ReInput.controllers.IsJoystickAssigned(entry.joystickId) || entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				player.controllers.AddController(2, entry.joystickId, true);
				this.ControllerSelectionChanged();
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.JoystickConflict, new ControlRemappingDemo1.WindowProperties
			{
				title = "Joystick Reassignment",
				message = "This joystick is already assigned to another player. Do you want to reassign this joystick to " + player.descriptiveName + "?",
				rect = this.GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = new Action<string, string>(this.DrawModalWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			return false;
		}

		// Token: 0x06002F1A RID: 12058 RVA: 0x0020FA3C File Offset: 0x0020DC3C
		private bool ProcessElementAssignmentChange(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			switch (entry.changeType)
			{
			case ControlRemappingDemo1.ElementAssignmentChangeType.Add:
			case ControlRemappingDemo1.ElementAssignmentChangeType.Replace:
				return this.ProcessAddOrReplaceElementAssignment(entry);
			case ControlRemappingDemo1.ElementAssignmentChangeType.Remove:
				return this.ProcessRemoveElementAssignment(entry);
			case ControlRemappingDemo1.ElementAssignmentChangeType.ReassignOrRemove:
				return this.ProcessRemoveOrReassignElementAssignment(entry);
			case ControlRemappingDemo1.ElementAssignmentChangeType.ConflictCheck:
				return this.ProcessElementAssignmentConflictCheck(entry);
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x06002F1B RID: 12059 RVA: 0x0020FA94 File Offset: 0x0020DC94
		private bool ProcessRemoveOrReassignElementAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (entry.context.controllerMap == null)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				ControlRemappingDemo1.ElementAssignmentChange elementAssignmentChange = new ControlRemappingDemo1.ElementAssignmentChange(entry);
				elementAssignmentChange.changeType = ControlRemappingDemo1.ElementAssignmentChangeType.Remove;
				this.actionQueue.Enqueue(elementAssignmentChange);
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				ControlRemappingDemo1.ElementAssignmentChange elementAssignmentChange2 = new ControlRemappingDemo1.ElementAssignmentChange(entry);
				elementAssignmentChange2.changeType = ControlRemappingDemo1.ElementAssignmentChangeType.Replace;
				this.actionQueue.Enqueue(elementAssignmentChange2);
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.AssignElement, new ControlRemappingDemo1.WindowProperties
			{
				title = "Reassign or Remove",
				message = "Do you want to reassign or remove this assignment?",
				rect = this.GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = new Action<string, string>(this.DrawReassignOrRemoveElementAssignmentWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			return false;
		}

		// Token: 0x06002F1C RID: 12060 RVA: 0x0020FB68 File Offset: 0x0020DD68
		private bool ProcessRemoveElementAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (entry.context.controllerMap == null)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				entry.context.controllerMap.DeleteElementMap(entry.context.actionElementMapToReplace.id);
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.DeleteAssignmentConfirmation, new ControlRemappingDemo1.WindowProperties
			{
				title = "Remove Assignment",
				message = "Are you sure you want to remove this assignment?",
				rect = this.GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = new Action<string, string>(this.DrawModalWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			return false;
		}

		// Token: 0x06002F1D RID: 12061 RVA: 0x0020FC28 File Offset: 0x0020DE28
		private bool ProcessAddOrReplaceElementAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				this.inputMapper.Stop();
				return true;
			}
			if (entry.state != ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				string text;
				if (entry.context.controllerMap.controllerType == null)
				{
					if (Application.platform == null || Application.platform == 1)
					{
						text = "Press any key to assign it to this action. You may also use the modifier keys Command, Control, Alt, and Shift. If you wish to assign a modifier key itself to this action, press and hold the key for 1 second.";
					}
					else
					{
						text = "Press any key to assign it to this action. You may also use the modifier keys Control, Alt, and Shift. If you wish to assign a modifier key itself to this action, press and hold the key for 1 second.";
					}
					if (Application.isEditor)
					{
						text += "\n\nNOTE: Some modifier key combinations will not work in the Unity Editor, but they will work in a game build.";
					}
				}
				else if (entry.context.controllerMap.controllerType == 1)
				{
					text = "Press any mouse button or axis to assign it to this action.\n\nTo assign mouse movement axes, move the mouse quickly in the direction you want mapped to the action. Slow movements will be ignored.";
				}
				else
				{
					text = "Press any button or axis to assign it to this action.";
				}
				this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.AssignElement, new ControlRemappingDemo1.WindowProperties
				{
					title = "Assign",
					message = text,
					rect = this.GetScreenCenteredRect(250f, 200f),
					windowDrawDelegate = new Action<string, string>(this.DrawElementAssignmentWindow)
				}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
				return false;
			}
			if (Event.current.type != 8)
			{
				return false;
			}
			if (this.conflictFoundEventData != null)
			{
				ControlRemappingDemo1.ElementAssignmentChange elementAssignmentChange = new ControlRemappingDemo1.ElementAssignmentChange(entry);
				elementAssignmentChange.changeType = ControlRemappingDemo1.ElementAssignmentChangeType.ConflictCheck;
				this.actionQueue.Enqueue(elementAssignmentChange);
			}
			return true;
		}

		// Token: 0x06002F1E RID: 12062 RVA: 0x0020FD58 File Offset: 0x0020DF58
		private bool ProcessElementAssignmentConflictCheck(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (entry.context.controllerMap == null)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				this.inputMapper.Stop();
				return true;
			}
			if (this.conflictFoundEventData == null)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				if (entry.response == ControlRemappingDemo1.UserResponse.Confirm)
				{
					this.conflictFoundEventData.responseCallback.Invoke(1);
				}
				else
				{
					if (entry.response != ControlRemappingDemo1.UserResponse.Custom1)
					{
						throw new NotImplementedException();
					}
					this.conflictFoundEventData.responseCallback.Invoke(2);
				}
				return true;
			}
			if (this.conflictFoundEventData.isProtected)
			{
				string message = this.conflictFoundEventData.assignment.elementDisplayName + " is already in use and is protected from reassignment. You cannot remove the protected assignment, but you can still assign the action to this element. If you do so, the element will trigger multiple actions when activated.";
				this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.AssignElement, new ControlRemappingDemo1.WindowProperties
				{
					title = "Assignment Conflict",
					message = message,
					rect = this.GetScreenCenteredRect(250f, 200f),
					windowDrawDelegate = new Action<string, string>(this.DrawElementAssignmentProtectedConflictWindow)
				}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			}
			else
			{
				string message2 = this.conflictFoundEventData.assignment.elementDisplayName + " is already in use. You may replace the other conflicting assignments, add this assignment anyway which will leave multiple actions assigned to this element, or cancel this assignment.";
				this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.AssignElement, new ControlRemappingDemo1.WindowProperties
				{
					title = "Assignment Conflict",
					message = message2,
					rect = this.GetScreenCenteredRect(250f, 200f),
					windowDrawDelegate = new Action<string, string>(this.DrawElementAssignmentNormalConflictWindow)
				}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			}
			return false;
		}

		// Token: 0x06002F1F RID: 12063 RVA: 0x0020FEF4 File Offset: 0x0020E0F4
		private bool ProcessFallbackJoystickIdentification(ControlRemappingDemo1.FallbackJoystickIdentification entry)
		{
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.JoystickConflict, new ControlRemappingDemo1.WindowProperties
			{
				title = "Joystick Identification Required",
				message = "A joystick has been attached or removed. You will need to identify each joystick by pressing a button on the controller listed below:",
				rect = this.GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = new Action<string, string>(this.DrawFallbackJoystickIdentificationWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback), 1f);
			return false;
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x0020FF88 File Offset: 0x0020E188
		private bool ProcessCalibration(ControlRemappingDemo1.Calibration entry)
		{
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.JoystickConflict, new ControlRemappingDemo1.WindowProperties
			{
				title = "Calibrate Controller",
				message = "Select an axis to calibrate on the " + entry.joystick.name + ".",
				rect = this.GetScreenCenteredRect(450f, 480f),
				windowDrawDelegate = new Action<string, string>(this.DrawCalibrationWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			return false;
		}

		// Token: 0x06002F21 RID: 12065 RVA: 0x0021002A File Offset: 0x0020E22A
		private void PlayerSelectionChanged()
		{
			this.ClearControllerSelection();
		}

		// Token: 0x06002F22 RID: 12066 RVA: 0x00210032 File Offset: 0x0020E232
		private void ControllerSelectionChanged()
		{
			this.ClearMapSelection();
		}

		// Token: 0x06002F23 RID: 12067 RVA: 0x0021003A File Offset: 0x0020E23A
		private void ClearControllerSelection()
		{
			this.selectedController.Clear();
			this.ClearMapSelection();
		}

		// Token: 0x06002F24 RID: 12068 RVA: 0x0021004D File Offset: 0x0020E24D
		private void ClearMapSelection()
		{
			this.selectedMapCategoryId = -1;
			this.selectedMap = null;
		}

		// Token: 0x06002F25 RID: 12069 RVA: 0x0021005D File Offset: 0x0020E25D
		private void ResetAll()
		{
			this.ClearWorkingVars();
			this.initialized = false;
			this.showMenu = false;
		}

		// Token: 0x06002F26 RID: 12070 RVA: 0x00210074 File Offset: 0x0020E274
		private void ClearWorkingVars()
		{
			this.selectedPlayer = null;
			this.ClearMapSelection();
			this.selectedController.Clear();
			this.actionScrollPos = default(Vector2);
			this.dialog.FullReset();
			this.actionQueue.Clear();
			this.busy = false;
			this.startListening = false;
			this.conflictFoundEventData = null;
			this.inputMapper.Stop();
		}

		// Token: 0x06002F27 RID: 12071 RVA: 0x002100DC File Offset: 0x0020E2DC
		private void SetGUIStateStart()
		{
			this.guiState = true;
			if (this.busy)
			{
				this.guiState = false;
			}
			this.pageGUIState = (this.guiState && !this.busy && !this.dialog.enabled && !this.dialog.busy);
			if (GUI.enabled != this.guiState)
			{
				GUI.enabled = this.guiState;
			}
		}

		// Token: 0x06002F28 RID: 12072 RVA: 0x0021014B File Offset: 0x0020E34B
		private void SetGUIStateEnd()
		{
			this.guiState = true;
			if (!GUI.enabled)
			{
				GUI.enabled = this.guiState;
			}
		}

		// Token: 0x06002F29 RID: 12073 RVA: 0x00210168 File Offset: 0x0020E368
		private void JoystickConnected(ControllerStatusChangedEventArgs args)
		{
			if (ReInput.controllers.IsControllerAssigned(args.controllerType, args.controllerId))
			{
				using (IEnumerator<Player> enumerator = ReInput.players.AllPlayers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Player player = enumerator.Current;
						if (player.controllers.ContainsController(args.controllerType, args.controllerId))
						{
							ReInput.userDataStore.LoadControllerData(player.id, args.controllerType, args.controllerId);
						}
					}
					goto IL_90;
				}
			}
			ReInput.userDataStore.LoadControllerData(args.controllerType, args.controllerId);
			IL_90:
			if (ReInput.unityJoystickIdentificationRequired)
			{
				this.IdentifyAllJoysticks();
			}
		}

		// Token: 0x06002F2A RID: 12074 RVA: 0x00210224 File Offset: 0x0020E424
		private void JoystickPreDisconnect(ControllerStatusChangedEventArgs args)
		{
			if (this.selectedController.hasSelection && args.controllerType == this.selectedController.type && args.controllerId == this.selectedController.id)
			{
				this.ClearControllerSelection();
			}
			if (this.showMenu)
			{
				if (ReInput.controllers.IsControllerAssigned(args.controllerType, args.controllerId))
				{
					using (IEnumerator<Player> enumerator = ReInput.players.AllPlayers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Player player = enumerator.Current;
							if (player.controllers.ContainsController(args.controllerType, args.controllerId))
							{
								ReInput.userDataStore.SaveControllerData(player.id, args.controllerType, args.controllerId);
							}
						}
						return;
					}
				}
				ReInput.userDataStore.SaveControllerData(args.controllerType, args.controllerId);
			}
		}

		// Token: 0x06002F2B RID: 12075 RVA: 0x00210318 File Offset: 0x0020E518
		private void JoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (this.showMenu)
			{
				this.ClearWorkingVars();
			}
			if (ReInput.unityJoystickIdentificationRequired)
			{
				this.IdentifyAllJoysticks();
			}
		}

		// Token: 0x06002F2C RID: 12076 RVA: 0x00210335 File Offset: 0x0020E535
		private void OnConflictFound(InputMapper.ConflictFoundEventData data)
		{
			this.conflictFoundEventData = data;
		}

		// Token: 0x06002F2D RID: 12077 RVA: 0x0021033E File Offset: 0x0020E53E
		private void OnStopped(InputMapper.StoppedEventData data)
		{
			this.conflictFoundEventData = null;
		}

		// Token: 0x06002F2E RID: 12078 RVA: 0x00210348 File Offset: 0x0020E548
		public void IdentifyAllJoysticks()
		{
			if (ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			this.ClearWorkingVars();
			this.Open();
			foreach (Joystick joystick in ReInput.controllers.Joysticks)
			{
				this.actionQueue.Enqueue(new ControlRemappingDemo1.FallbackJoystickIdentification(joystick.id, joystick.name));
			}
		}

		// Token: 0x06002F2F RID: 12079 RVA: 0x00002265 File Offset: 0x00000465
		protected void CheckRecompile()
		{
		}

		// Token: 0x06002F30 RID: 12080 RVA: 0x00002265 File Offset: 0x00000465
		private void RecompileWindow(int windowId)
		{
		}

		// Token: 0x040049F1 RID: 18929
		private const float defaultModalWidth = 250f;

		// Token: 0x040049F2 RID: 18930
		private const float defaultModalHeight = 200f;

		// Token: 0x040049F3 RID: 18931
		private const float assignmentTimeout = 5f;

		// Token: 0x040049F4 RID: 18932
		private ControlRemappingDemo1.DialogHelper dialog;

		// Token: 0x040049F5 RID: 18933
		private InputMapper inputMapper = new InputMapper();

		// Token: 0x040049F6 RID: 18934
		private InputMapper.ConflictFoundEventData conflictFoundEventData;

		// Token: 0x040049F7 RID: 18935
		private bool guiState;

		// Token: 0x040049F8 RID: 18936
		private bool busy;

		// Token: 0x040049F9 RID: 18937
		private bool pageGUIState;

		// Token: 0x040049FA RID: 18938
		private Player selectedPlayer;

		// Token: 0x040049FB RID: 18939
		private int selectedMapCategoryId;

		// Token: 0x040049FC RID: 18940
		private ControlRemappingDemo1.ControllerSelection selectedController;

		// Token: 0x040049FD RID: 18941
		private ControllerMap selectedMap;

		// Token: 0x040049FE RID: 18942
		private bool showMenu;

		// Token: 0x040049FF RID: 18943
		private bool startListening;

		// Token: 0x04004A00 RID: 18944
		private Vector2 actionScrollPos;

		// Token: 0x04004A01 RID: 18945
		private Vector2 calibrateScrollPos;

		// Token: 0x04004A02 RID: 18946
		private Queue<ControlRemappingDemo1.QueueEntry> actionQueue;

		// Token: 0x04004A03 RID: 18947
		private bool setupFinished;

		// Token: 0x04004A04 RID: 18948
		[NonSerialized]
		private bool initialized;

		// Token: 0x04004A05 RID: 18949
		private bool isCompiling;

		// Token: 0x04004A06 RID: 18950
		private GUIStyle style_wordWrap;

		// Token: 0x04004A07 RID: 18951
		private GUIStyle style_centeredBox;

		// Token: 0x020008A0 RID: 2208
		private class ControllerSelection
		{
			// Token: 0x06002F32 RID: 12082 RVA: 0x002103DB File Offset: 0x0020E5DB
			public ControllerSelection()
			{
				this.Clear();
			}

			// Token: 0x170004DD RID: 1245
			// (get) Token: 0x06002F33 RID: 12083 RVA: 0x002103E9 File Offset: 0x0020E5E9
			// (set) Token: 0x06002F34 RID: 12084 RVA: 0x002103F1 File Offset: 0x0020E5F1
			public int id
			{
				get
				{
					return this._id;
				}
				set
				{
					this._idPrev = this._id;
					this._id = value;
				}
			}

			// Token: 0x170004DE RID: 1246
			// (get) Token: 0x06002F35 RID: 12085 RVA: 0x00210406 File Offset: 0x0020E606
			// (set) Token: 0x06002F36 RID: 12086 RVA: 0x0021040E File Offset: 0x0020E60E
			public ControllerType type
			{
				get
				{
					return this._type;
				}
				set
				{
					this._typePrev = this._type;
					this._type = value;
				}
			}

			// Token: 0x170004DF RID: 1247
			// (get) Token: 0x06002F37 RID: 12087 RVA: 0x00210423 File Offset: 0x0020E623
			public int idPrev
			{
				get
				{
					return this._idPrev;
				}
			}

			// Token: 0x170004E0 RID: 1248
			// (get) Token: 0x06002F38 RID: 12088 RVA: 0x0021042B File Offset: 0x0020E62B
			public ControllerType typePrev
			{
				get
				{
					return this._typePrev;
				}
			}

			// Token: 0x170004E1 RID: 1249
			// (get) Token: 0x06002F39 RID: 12089 RVA: 0x00210433 File Offset: 0x0020E633
			public bool hasSelection
			{
				get
				{
					return this._id >= 0;
				}
			}

			// Token: 0x06002F3A RID: 12090 RVA: 0x00210441 File Offset: 0x0020E641
			public void Set(int id, ControllerType type)
			{
				this.id = id;
				this.type = type;
			}

			// Token: 0x06002F3B RID: 12091 RVA: 0x00210451 File Offset: 0x0020E651
			public void Clear()
			{
				this._id = -1;
				this._idPrev = -1;
				this._type = 2;
				this._typePrev = 2;
			}

			// Token: 0x04004A08 RID: 18952
			private int _id;

			// Token: 0x04004A09 RID: 18953
			private int _idPrev;

			// Token: 0x04004A0A RID: 18954
			private ControllerType _type;

			// Token: 0x04004A0B RID: 18955
			private ControllerType _typePrev;
		}

		// Token: 0x020008A1 RID: 2209
		private class DialogHelper
		{
			// Token: 0x170004E2 RID: 1250
			// (get) Token: 0x06002F3C RID: 12092 RVA: 0x0021046F File Offset: 0x0020E66F
			private float busyTimer
			{
				get
				{
					if (!this._busyTimerRunning)
					{
						return 0f;
					}
					return this._busyTime - Time.realtimeSinceStartup;
				}
			}

			// Token: 0x170004E3 RID: 1251
			// (get) Token: 0x06002F3D RID: 12093 RVA: 0x0021048B File Offset: 0x0020E68B
			// (set) Token: 0x06002F3E RID: 12094 RVA: 0x00210493 File Offset: 0x0020E693
			public bool enabled
			{
				get
				{
					return this._enabled;
				}
				set
				{
					if (!value)
					{
						this._enabled = value;
						this._type = ControlRemappingDemo1.DialogHelper.DialogType.None;
						this.StateChanged(0.1f);
						return;
					}
					if (this._type == ControlRemappingDemo1.DialogHelper.DialogType.None)
					{
						return;
					}
					this.StateChanged(0.25f);
				}
			}

			// Token: 0x170004E4 RID: 1252
			// (get) Token: 0x06002F3F RID: 12095 RVA: 0x002104C6 File Offset: 0x0020E6C6
			// (set) Token: 0x06002F40 RID: 12096 RVA: 0x002104D8 File Offset: 0x0020E6D8
			public ControlRemappingDemo1.DialogHelper.DialogType type
			{
				get
				{
					if (!this._enabled)
					{
						return ControlRemappingDemo1.DialogHelper.DialogType.None;
					}
					return this._type;
				}
				set
				{
					if (value == ControlRemappingDemo1.DialogHelper.DialogType.None)
					{
						this._enabled = false;
						this.StateChanged(0.1f);
					}
					else
					{
						this._enabled = true;
						this.StateChanged(0.25f);
					}
					this._type = value;
				}
			}

			// Token: 0x170004E5 RID: 1253
			// (get) Token: 0x06002F41 RID: 12097 RVA: 0x0021050A File Offset: 0x0020E70A
			public bool busy
			{
				get
				{
					return this._busyTimerRunning;
				}
			}

			// Token: 0x06002F42 RID: 12098 RVA: 0x00210512 File Offset: 0x0020E712
			public DialogHelper()
			{
				this.drawWindowDelegate = new Action<int>(this.DrawWindow);
				this.drawWindowFunction = new GUI.WindowFunction(this.drawWindowDelegate.Invoke);
			}

			// Token: 0x06002F43 RID: 12099 RVA: 0x00210543 File Offset: 0x0020E743
			public void StartModal(int queueActionId, ControlRemappingDemo1.DialogHelper.DialogType type, ControlRemappingDemo1.WindowProperties windowProperties, Action<int, ControlRemappingDemo1.UserResponse> resultCallback)
			{
				this.StartModal(queueActionId, type, windowProperties, resultCallback, -1f);
			}

			// Token: 0x06002F44 RID: 12100 RVA: 0x00210555 File Offset: 0x0020E755
			public void StartModal(int queueActionId, ControlRemappingDemo1.DialogHelper.DialogType type, ControlRemappingDemo1.WindowProperties windowProperties, Action<int, ControlRemappingDemo1.UserResponse> resultCallback, float openBusyDelay)
			{
				this.currentActionId = queueActionId;
				this.windowProperties = windowProperties;
				this.type = type;
				this.resultCallback = resultCallback;
				if (openBusyDelay >= 0f)
				{
					this.StateChanged(openBusyDelay);
				}
			}

			// Token: 0x06002F45 RID: 12101 RVA: 0x00210585 File Offset: 0x0020E785
			public void Update()
			{
				this.Draw();
				this.UpdateTimers();
			}

			// Token: 0x06002F46 RID: 12102 RVA: 0x00210594 File Offset: 0x0020E794
			public void Draw()
			{
				if (!this._enabled)
				{
					return;
				}
				bool enabled = GUI.enabled;
				GUI.enabled = true;
				GUILayout.Window(this.windowProperties.windowId, this.windowProperties.rect, this.drawWindowFunction, this.windowProperties.title, Array.Empty<GUILayoutOption>());
				GUI.FocusWindow(this.windowProperties.windowId);
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}

			// Token: 0x06002F47 RID: 12103 RVA: 0x00210606 File Offset: 0x0020E806
			public void DrawConfirmButton()
			{
				this.DrawConfirmButton("Confirm");
			}

			// Token: 0x06002F48 RID: 12104 RVA: 0x00210614 File Offset: 0x0020E814
			public void DrawConfirmButton(string title)
			{
				bool enabled = GUI.enabled;
				if (this.busy)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button(title, Array.Empty<GUILayoutOption>()))
				{
					this.Confirm(ControlRemappingDemo1.UserResponse.Confirm);
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}

			// Token: 0x06002F49 RID: 12105 RVA: 0x00210657 File Offset: 0x0020E857
			public void DrawConfirmButton(ControlRemappingDemo1.UserResponse response)
			{
				this.DrawConfirmButton(response, "Confirm");
			}

			// Token: 0x06002F4A RID: 12106 RVA: 0x00210668 File Offset: 0x0020E868
			public void DrawConfirmButton(ControlRemappingDemo1.UserResponse response, string title)
			{
				bool enabled = GUI.enabled;
				if (this.busy)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button(title, Array.Empty<GUILayoutOption>()))
				{
					this.Confirm(response);
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}

			// Token: 0x06002F4B RID: 12107 RVA: 0x002106AB File Offset: 0x0020E8AB
			public void DrawCancelButton()
			{
				this.DrawCancelButton("Cancel");
			}

			// Token: 0x06002F4C RID: 12108 RVA: 0x002106B8 File Offset: 0x0020E8B8
			public void DrawCancelButton(string title)
			{
				bool enabled = GUI.enabled;
				if (this.busy)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button(title, Array.Empty<GUILayoutOption>()))
				{
					this.Cancel();
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}

			// Token: 0x06002F4D RID: 12109 RVA: 0x002106FA File Offset: 0x0020E8FA
			public void Confirm()
			{
				this.Confirm(ControlRemappingDemo1.UserResponse.Confirm);
			}

			// Token: 0x06002F4E RID: 12110 RVA: 0x00210703 File Offset: 0x0020E903
			public void Confirm(ControlRemappingDemo1.UserResponse response)
			{
				this.resultCallback.Invoke(this.currentActionId, response);
				this.Close();
			}

			// Token: 0x06002F4F RID: 12111 RVA: 0x0021071D File Offset: 0x0020E91D
			public void Cancel()
			{
				this.resultCallback.Invoke(this.currentActionId, ControlRemappingDemo1.UserResponse.Cancel);
				this.Close();
			}

			// Token: 0x06002F50 RID: 12112 RVA: 0x00210737 File Offset: 0x0020E937
			private void DrawWindow(int windowId)
			{
				this.windowProperties.windowDrawDelegate.Invoke(this.windowProperties.title, this.windowProperties.message);
			}

			// Token: 0x06002F51 RID: 12113 RVA: 0x0021075F File Offset: 0x0020E95F
			private void UpdateTimers()
			{
				if (this._busyTimerRunning && this.busyTimer <= 0f)
				{
					this._busyTimerRunning = false;
				}
			}

			// Token: 0x06002F52 RID: 12114 RVA: 0x0021077D File Offset: 0x0020E97D
			private void StartBusyTimer(float time)
			{
				this._busyTime = time + Time.realtimeSinceStartup;
				this._busyTimerRunning = true;
			}

			// Token: 0x06002F53 RID: 12115 RVA: 0x00210793 File Offset: 0x0020E993
			private void Close()
			{
				this.Reset();
				this.StateChanged(0.1f);
			}

			// Token: 0x06002F54 RID: 12116 RVA: 0x002107A6 File Offset: 0x0020E9A6
			private void StateChanged(float delay)
			{
				this.StartBusyTimer(delay);
			}

			// Token: 0x06002F55 RID: 12117 RVA: 0x002107AF File Offset: 0x0020E9AF
			private void Reset()
			{
				this._enabled = false;
				this._type = ControlRemappingDemo1.DialogHelper.DialogType.None;
				this.currentActionId = -1;
				this.resultCallback = null;
			}

			// Token: 0x06002F56 RID: 12118 RVA: 0x002107CD File Offset: 0x0020E9CD
			private void ResetTimers()
			{
				this._busyTimerRunning = false;
			}

			// Token: 0x06002F57 RID: 12119 RVA: 0x002107D6 File Offset: 0x0020E9D6
			public void FullReset()
			{
				this.Reset();
				this.ResetTimers();
			}

			// Token: 0x04004A0C RID: 18956
			private const float openBusyDelay = 0.25f;

			// Token: 0x04004A0D RID: 18957
			private const float closeBusyDelay = 0.1f;

			// Token: 0x04004A0E RID: 18958
			private ControlRemappingDemo1.DialogHelper.DialogType _type;

			// Token: 0x04004A0F RID: 18959
			private bool _enabled;

			// Token: 0x04004A10 RID: 18960
			private float _busyTime;

			// Token: 0x04004A11 RID: 18961
			private bool _busyTimerRunning;

			// Token: 0x04004A12 RID: 18962
			private Action<int> drawWindowDelegate;

			// Token: 0x04004A13 RID: 18963
			private GUI.WindowFunction drawWindowFunction;

			// Token: 0x04004A14 RID: 18964
			private ControlRemappingDemo1.WindowProperties windowProperties;

			// Token: 0x04004A15 RID: 18965
			private int currentActionId;

			// Token: 0x04004A16 RID: 18966
			private Action<int, ControlRemappingDemo1.UserResponse> resultCallback;

			// Token: 0x020008A2 RID: 2210
			public enum DialogType
			{
				// Token: 0x04004A18 RID: 18968
				None,
				// Token: 0x04004A19 RID: 18969
				JoystickConflict,
				// Token: 0x04004A1A RID: 18970
				ElementConflict,
				// Token: 0x04004A1B RID: 18971
				KeyConflict,
				// Token: 0x04004A1C RID: 18972
				DeleteAssignmentConfirmation = 10,
				// Token: 0x04004A1D RID: 18973
				AssignElement
			}
		}

		// Token: 0x020008A3 RID: 2211
		private abstract class QueueEntry
		{
			// Token: 0x170004E6 RID: 1254
			// (get) Token: 0x06002F58 RID: 12120 RVA: 0x002107E4 File Offset: 0x0020E9E4
			// (set) Token: 0x06002F59 RID: 12121 RVA: 0x002107EC File Offset: 0x0020E9EC
			public int id { get; protected set; }

			// Token: 0x170004E7 RID: 1255
			// (get) Token: 0x06002F5A RID: 12122 RVA: 0x002107F5 File Offset: 0x0020E9F5
			// (set) Token: 0x06002F5B RID: 12123 RVA: 0x002107FD File Offset: 0x0020E9FD
			public ControlRemappingDemo1.QueueActionType queueActionType { get; protected set; }

			// Token: 0x170004E8 RID: 1256
			// (get) Token: 0x06002F5C RID: 12124 RVA: 0x00210806 File Offset: 0x0020EA06
			// (set) Token: 0x06002F5D RID: 12125 RVA: 0x0021080E File Offset: 0x0020EA0E
			public ControlRemappingDemo1.QueueEntry.State state { get; protected set; }

			// Token: 0x170004E9 RID: 1257
			// (get) Token: 0x06002F5E RID: 12126 RVA: 0x00210817 File Offset: 0x0020EA17
			// (set) Token: 0x06002F5F RID: 12127 RVA: 0x0021081F File Offset: 0x0020EA1F
			public ControlRemappingDemo1.UserResponse response { get; protected set; }

			// Token: 0x170004EA RID: 1258
			// (get) Token: 0x06002F60 RID: 12128 RVA: 0x00210828 File Offset: 0x0020EA28
			protected static int nextId
			{
				get
				{
					int result = ControlRemappingDemo1.QueueEntry.uidCounter;
					ControlRemappingDemo1.QueueEntry.uidCounter++;
					return result;
				}
			}

			// Token: 0x06002F61 RID: 12129 RVA: 0x0021083B File Offset: 0x0020EA3B
			public QueueEntry(ControlRemappingDemo1.QueueActionType queueActionType)
			{
				this.id = ControlRemappingDemo1.QueueEntry.nextId;
				this.queueActionType = queueActionType;
			}

			// Token: 0x06002F62 RID: 12130 RVA: 0x00210855 File Offset: 0x0020EA55
			public void Confirm(ControlRemappingDemo1.UserResponse response)
			{
				this.state = ControlRemappingDemo1.QueueEntry.State.Confirmed;
				this.response = response;
			}

			// Token: 0x06002F63 RID: 12131 RVA: 0x00210865 File Offset: 0x0020EA65
			public void Cancel()
			{
				this.state = ControlRemappingDemo1.QueueEntry.State.Canceled;
			}

			// Token: 0x04004A22 RID: 18978
			private static int uidCounter;

			// Token: 0x020008A4 RID: 2212
			public enum State
			{
				// Token: 0x04004A24 RID: 18980
				Waiting,
				// Token: 0x04004A25 RID: 18981
				Confirmed,
				// Token: 0x04004A26 RID: 18982
				Canceled
			}
		}

		// Token: 0x020008A5 RID: 2213
		private class JoystickAssignmentChange : ControlRemappingDemo1.QueueEntry
		{
			// Token: 0x170004EB RID: 1259
			// (get) Token: 0x06002F64 RID: 12132 RVA: 0x0021086E File Offset: 0x0020EA6E
			// (set) Token: 0x06002F65 RID: 12133 RVA: 0x00210876 File Offset: 0x0020EA76
			public int playerId { get; private set; }

			// Token: 0x170004EC RID: 1260
			// (get) Token: 0x06002F66 RID: 12134 RVA: 0x0021087F File Offset: 0x0020EA7F
			// (set) Token: 0x06002F67 RID: 12135 RVA: 0x00210887 File Offset: 0x0020EA87
			public int joystickId { get; private set; }

			// Token: 0x170004ED RID: 1261
			// (get) Token: 0x06002F68 RID: 12136 RVA: 0x00210890 File Offset: 0x0020EA90
			// (set) Token: 0x06002F69 RID: 12137 RVA: 0x00210898 File Offset: 0x0020EA98
			public bool assign { get; private set; }

			// Token: 0x06002F6A RID: 12138 RVA: 0x002108A1 File Offset: 0x0020EAA1
			public JoystickAssignmentChange(int newPlayerId, int joystickId, bool assign) : base(ControlRemappingDemo1.QueueActionType.JoystickAssignment)
			{
				this.playerId = newPlayerId;
				this.joystickId = joystickId;
				this.assign = assign;
			}
		}

		// Token: 0x020008A6 RID: 2214
		private class ElementAssignmentChange : ControlRemappingDemo1.QueueEntry
		{
			// Token: 0x170004EE RID: 1262
			// (get) Token: 0x06002F6B RID: 12139 RVA: 0x002108BF File Offset: 0x0020EABF
			// (set) Token: 0x06002F6C RID: 12140 RVA: 0x002108C7 File Offset: 0x0020EAC7
			public ControlRemappingDemo1.ElementAssignmentChangeType changeType { get; set; }

			// Token: 0x170004EF RID: 1263
			// (get) Token: 0x06002F6D RID: 12141 RVA: 0x002108D0 File Offset: 0x0020EAD0
			// (set) Token: 0x06002F6E RID: 12142 RVA: 0x002108D8 File Offset: 0x0020EAD8
			public InputMapper.Context context { get; private set; }

			// Token: 0x06002F6F RID: 12143 RVA: 0x002108E1 File Offset: 0x0020EAE1
			public ElementAssignmentChange(ControlRemappingDemo1.ElementAssignmentChangeType changeType, InputMapper.Context context) : base(ControlRemappingDemo1.QueueActionType.ElementAssignment)
			{
				this.changeType = changeType;
				this.context = context;
			}

			// Token: 0x06002F70 RID: 12144 RVA: 0x002108F8 File Offset: 0x0020EAF8
			public ElementAssignmentChange(ControlRemappingDemo1.ElementAssignmentChange other) : this(other.changeType, other.context.Clone())
			{
			}
		}

		// Token: 0x020008A7 RID: 2215
		private class FallbackJoystickIdentification : ControlRemappingDemo1.QueueEntry
		{
			// Token: 0x170004F0 RID: 1264
			// (get) Token: 0x06002F71 RID: 12145 RVA: 0x00210911 File Offset: 0x0020EB11
			// (set) Token: 0x06002F72 RID: 12146 RVA: 0x00210919 File Offset: 0x0020EB19
			public int joystickId { get; private set; }

			// Token: 0x170004F1 RID: 1265
			// (get) Token: 0x06002F73 RID: 12147 RVA: 0x00210922 File Offset: 0x0020EB22
			// (set) Token: 0x06002F74 RID: 12148 RVA: 0x0021092A File Offset: 0x0020EB2A
			public string joystickName { get; private set; }

			// Token: 0x06002F75 RID: 12149 RVA: 0x00210933 File Offset: 0x0020EB33
			public FallbackJoystickIdentification(int joystickId, string joystickName) : base(ControlRemappingDemo1.QueueActionType.FallbackJoystickIdentification)
			{
				this.joystickId = joystickId;
				this.joystickName = joystickName;
			}
		}

		// Token: 0x020008A8 RID: 2216
		private class Calibration : ControlRemappingDemo1.QueueEntry
		{
			// Token: 0x170004F2 RID: 1266
			// (get) Token: 0x06002F76 RID: 12150 RVA: 0x0021094A File Offset: 0x0020EB4A
			// (set) Token: 0x06002F77 RID: 12151 RVA: 0x00210952 File Offset: 0x0020EB52
			public Player player { get; private set; }

			// Token: 0x170004F3 RID: 1267
			// (get) Token: 0x06002F78 RID: 12152 RVA: 0x0021095B File Offset: 0x0020EB5B
			// (set) Token: 0x06002F79 RID: 12153 RVA: 0x00210963 File Offset: 0x0020EB63
			public ControllerType controllerType { get; private set; }

			// Token: 0x170004F4 RID: 1268
			// (get) Token: 0x06002F7A RID: 12154 RVA: 0x0021096C File Offset: 0x0020EB6C
			// (set) Token: 0x06002F7B RID: 12155 RVA: 0x00210974 File Offset: 0x0020EB74
			public Joystick joystick { get; private set; }

			// Token: 0x170004F5 RID: 1269
			// (get) Token: 0x06002F7C RID: 12156 RVA: 0x0021097D File Offset: 0x0020EB7D
			// (set) Token: 0x06002F7D RID: 12157 RVA: 0x00210985 File Offset: 0x0020EB85
			public CalibrationMap calibrationMap { get; private set; }

			// Token: 0x06002F7E RID: 12158 RVA: 0x0021098E File Offset: 0x0020EB8E
			public Calibration(Player player, Joystick joystick, CalibrationMap calibrationMap) : base(ControlRemappingDemo1.QueueActionType.Calibrate)
			{
				this.player = player;
				this.joystick = joystick;
				this.calibrationMap = calibrationMap;
				this.selectedElementIdentifierId = -1;
			}

			// Token: 0x04004A32 RID: 18994
			public int selectedElementIdentifierId;

			// Token: 0x04004A33 RID: 18995
			public bool recording;
		}

		// Token: 0x020008A9 RID: 2217
		private struct WindowProperties
		{
			// Token: 0x04004A34 RID: 18996
			public int windowId;

			// Token: 0x04004A35 RID: 18997
			public Rect rect;

			// Token: 0x04004A36 RID: 18998
			public Action<string, string> windowDrawDelegate;

			// Token: 0x04004A37 RID: 18999
			public string title;

			// Token: 0x04004A38 RID: 19000
			public string message;
		}

		// Token: 0x020008AA RID: 2218
		private enum QueueActionType
		{
			// Token: 0x04004A3A RID: 19002
			None,
			// Token: 0x04004A3B RID: 19003
			JoystickAssignment,
			// Token: 0x04004A3C RID: 19004
			ElementAssignment,
			// Token: 0x04004A3D RID: 19005
			FallbackJoystickIdentification,
			// Token: 0x04004A3E RID: 19006
			Calibrate
		}

		// Token: 0x020008AB RID: 2219
		private enum ElementAssignmentChangeType
		{
			// Token: 0x04004A40 RID: 19008
			Add,
			// Token: 0x04004A41 RID: 19009
			Replace,
			// Token: 0x04004A42 RID: 19010
			Remove,
			// Token: 0x04004A43 RID: 19011
			ReassignOrRemove,
			// Token: 0x04004A44 RID: 19012
			ConflictCheck
		}

		// Token: 0x020008AC RID: 2220
		public enum UserResponse
		{
			// Token: 0x04004A46 RID: 19014
			Confirm,
			// Token: 0x04004A47 RID: 19015
			Cancel,
			// Token: 0x04004A48 RID: 19016
			Custom1,
			// Token: 0x04004A49 RID: 19017
			Custom2
		}
	}
}
