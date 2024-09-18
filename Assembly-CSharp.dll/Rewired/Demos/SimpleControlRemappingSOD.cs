using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.Demos
{
	// Token: 0x020008CB RID: 2251
	[AddComponentMenu("")]
	public class SimpleControlRemappingSOD : MonoBehaviour
	{
		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06003037 RID: 12343 RVA: 0x00213E01 File Offset: 0x00212001
		public static SimpleControlRemappingSOD Instance
		{
			get
			{
				return SimpleControlRemappingSOD._instance;
			}
		}

		// Token: 0x06003038 RID: 12344 RVA: 0x00213E08 File Offset: 0x00212008
		private void Awake()
		{
			if (SimpleControlRemappingSOD._instance != null && SimpleControlRemappingSOD._instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			SimpleControlRemappingSOD._instance = this;
		}

		// Token: 0x06003039 RID: 12345 RVA: 0x00213E38 File Offset: 0x00212038
		private void OnEnable()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (InputController.Instance == null)
			{
				return;
			}
			this.GetController();
			this.schemeToggle.SetIsOnWithoutNotify(InputController.Instance.mouseInputMode);
			this.statusUIText.text = string.Empty;
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnControllerChanged);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnControllerChanged);
			this.InitializeUI();
		}

		// Token: 0x0600303A RID: 12346 RVA: 0x00213EAE File Offset: 0x002120AE
		private void OnDisable()
		{
			this.StopMapping(true);
			ReInput.ControllerConnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerChanged);
			ReInput.ControllerDisconnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerChanged);
		}

		// Token: 0x0600303B RID: 12347 RVA: 0x00213EDC File Offset: 0x002120DC
		private void ClearUI()
		{
			foreach (SimpleControlRemappingSOD.Row row in this.rows)
			{
				if (row.button != null)
				{
					Object.Destroy(row.button.gameObject);
				}
			}
			this.rows = new List<SimpleControlRemappingSOD.Row>();
		}

		// Token: 0x0600303C RID: 12348 RVA: 0x00213F54 File Offset: 0x00212154
		private void InitializeUI()
		{
			this.ClearUI();
			if (this.controllers.Count <= 0 || this.controllerMaps.Count <= 0)
			{
				return;
			}
			foreach (string text in this.categories)
			{
				foreach (InputAction inputAction in ReInput.mapping.ActionsInCategory(text))
				{
					if (inputAction.userAssignable)
					{
						string text2 = "Menu: Creating control remap row for ";
						InputAction inputAction2 = inputAction;
						Game.Log(text2 + ((inputAction2 != null) ? inputAction2.ToString() : null) + " in " + text, 2);
						if (inputAction.type == null)
						{
							this.CreateUIRow(inputAction, 0, inputAction.descriptiveName);
							this.CreateUIRow(inputAction, 1, (!string.IsNullOrEmpty(inputAction.positiveDescriptiveName)) ? inputAction.positiveDescriptiveName : (inputAction.descriptiveName + " +"));
							this.CreateUIRow(inputAction, 2, (!string.IsNullOrEmpty(inputAction.negativeDescriptiveName)) ? inputAction.negativeDescriptiveName : (inputAction.descriptiveName + " -"));
						}
						else if (inputAction.type == 1)
						{
							this.CreateUIRow(inputAction, 1, inputAction.descriptiveName);
						}
					}
				}
			}
			this.RedrawUI();
		}

		// Token: 0x0600303D RID: 12349 RVA: 0x002140E0 File Offset: 0x002122E0
		private void CreateUIRow(InputAction action, AxisRange actionRange, string label)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.buttonPrefab, this.fieldGroupTransform);
			gameObject.transform.SetAsLastSibling();
			RemapController component = gameObject.GetComponent<RemapController>();
			component.controlDescriptionText.text = Strings.Get("ui.interface", label, Strings.Casing.asIs, false, false, false, null);
			component.gameObject.name = component.controlDescriptionText.text;
			Navigation navigation = default(Navigation);
			navigation.mode = 4;
			navigation.selectOnLeft = this.backButton;
			navigation.selectOnRight = this.resetControlsButton;
			if (this.rows.Count <= 0)
			{
				navigation.selectOnUp = this.aboveButton;
				Navigation navigation2 = default(Navigation);
				navigation2.mode = 4;
				navigation2.selectOnLeft = this.aboveButton.navigation.selectOnLeft;
				navigation2.selectOnRight = this.aboveButton.navigation.selectOnRight;
				navigation2.selectOnUp = this.aboveButton.navigation.selectOnUp;
				navigation2.selectOnDown = component.primaryControlButton.button;
				this.aboveButton.navigation = navigation2;
				Navigation navigation3 = default(Navigation);
				navigation3.mode = 4;
				navigation3.selectOnLeft = this.aboveButton2.navigation.selectOnLeft;
				navigation3.selectOnRight = this.aboveButton2.navigation.selectOnRight;
				navigation3.selectOnUp = this.aboveButton2.navigation.selectOnUp;
				navigation3.selectOnDown = component.primaryControlButton.button;
				this.aboveButton2.navigation = navigation3;
			}
			else
			{
				navigation.selectOnUp = this.rows[this.rows.Count - 1].button.primaryControlButton.button;
				Navigation navigation4 = default(Navigation);
				navigation4.mode = 4;
				navigation4.selectOnLeft = this.rows[this.rows.Count - 1].button.primaryControlButton.button.navigation.selectOnLeft;
				navigation4.selectOnRight = this.rows[this.rows.Count - 1].button.primaryControlButton.button.navigation.selectOnRight;
				navigation4.selectOnUp = this.rows[this.rows.Count - 1].button.primaryControlButton.button.navigation.selectOnUp;
				navigation4.selectOnDown = component.primaryControlButton.button;
				this.rows[this.rows.Count - 1].button.primaryControlButton.button.navigation = navigation4;
			}
			component.primaryControlButton.button.navigation = navigation;
			this.rows.Add(new SimpleControlRemappingSOD.Row
			{
				action = action,
				actionRange = actionRange,
				button = component
			});
		}

		// Token: 0x0600303E RID: 12350 RVA: 0x002143F4 File Offset: 0x002125F4
		private void RedrawUI()
		{
			Game.Log("Menu: Controls: Redraw UI", 2);
			if (this.controllers.Count <= 0)
			{
				this.ClearUI();
				return;
			}
			for (int i = 0; i < this.rows.Count; i++)
			{
				SimpleControlRemappingSOD.Row row = this.rows[i];
				InputAction action = this.rows[i].action;
				row.button.primaryControlButton.icon.gameObject.SetActive(false);
				row.button.primaryControlButton.text.text = string.Empty;
				foreach (ControllerMap controllerMap in this.controllerMaps)
				{
					foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(action.id))
					{
						if (actionElementMap.ShowInField(row.actionRange))
						{
							row.button.primaryControlButton.text.text = actionElementMap.elementIdentifierName;
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600303F RID: 12351 RVA: 0x0021453C File Offset: 0x0021273C
		private void SetSelectedController(ControllerType controllerType)
		{
			bool flag = false;
			if (controllerType != this.selectedControllerType)
			{
				this.selectedControllerType = controllerType;
				flag = true;
			}
			int num = this.selectedControllerId;
			if (this.selectedControllerType >= 2)
			{
				Game.Log("Menu: Detected player is using a joystick...", 2);
				if (InputController.Instance.player.controllers.joystickCount > 0)
				{
					this.selectedControllerId = InputController.Instance.player.controllers.Joysticks[0].id;
				}
				else
				{
					this.selectedControllerId = -1;
				}
				this.schemeToggle.SetIsOnWithoutNotify(false);
			}
			else
			{
				Game.Log("Menu: Detected player is using a mouse/keyboard...", 2);
				this.selectedControllerId = 0;
				this.schemeToggle.SetIsOnWithoutNotify(true);
			}
			if (this.selectedControllerId != num)
			{
				flag = true;
			}
			if (flag)
			{
				this.StopMapping(false);
				this.GetController();
				this.InitializeUI();
			}
		}

		// Token: 0x06003040 RID: 12352 RVA: 0x0021460C File Offset: 0x0021280C
		public void GetController()
		{
			Game.Log("Menu: Attempting to get all controllers and maps...", 2);
			this.controllers = new List<Controller>();
			if (this.selectedControllerType == null || this.selectedControllerType == 1)
			{
				Controller controller = InputController.Instance.player.controllers.GetController(0, this.selectedControllerId);
				if (controller != null)
				{
					Game.Log("Menu: Adding controller: " + controller.hardwareName, 2);
					this.controllers.Add(controller);
				}
				Controller controller2 = InputController.Instance.player.controllers.GetController(1, this.selectedControllerId);
				if (controller2 != null)
				{
					Game.Log("Menu: Adding controller: " + controller2.hardwareName, 2);
					this.controllers.Add(controller2);
				}
			}
			else
			{
				Controller controller3 = InputController.Instance.player.controllers.GetController(this.selectedControllerType, this.selectedControllerId);
				if (controller3 != null)
				{
					Game.Log(string.Concat(new string[]
					{
						"Menu: Adding controller: ",
						controller3.hardwareName,
						" (",
						this.selectedControllerType.ToString(),
						")"
					}), 2);
					this.controllers.Add(controller3);
				}
			}
			this.controllerMaps = new List<ControllerMap>();
			foreach (Controller controller4 in this.controllers)
			{
				foreach (string text in this.categories)
				{
					ControllerMap map = InputController.Instance.player.controllers.maps.GetMap(controller4.type, controller4.id, text, "Default");
					if (map == null)
					{
						string[] array = new string[7];
						array[0] = "Unable to get controller map for ";
						array[1] = controller4.type.ToString();
						array[2] = ", id: ";
						int num = 3;
						int id = controller4.id;
						array[num] = id.ToString();
						array[4] = ", category: ";
						array[5] = text;
						array[6] = ", layout: Default";
						Game.LogError(string.Concat(array), 2);
					}
					else
					{
						Game.Log(string.Concat(new string[]
						{
							"Menu: Adding map ",
							map.name,
							" for controller ",
							controller4.hardwareName,
							" (",
							controller4.type.ToString(),
							")"
						}), 2);
						this.controllerMaps.Add(map);
					}
				}
			}
			this.keyboardAndMouseMappers.Clear();
			this.gamepadMappers.Clear();
			this.allMappers.Clear();
			foreach (ControllerMap controllerMap in this.controllerMaps)
			{
				SimpleControlRemappingSOD.Mapping mapping = default(SimpleControlRemappingSOD.Mapping);
				mapping.map = controllerMap;
				mapping.mapper = new InputMapper();
				mapping.mapper.options.timeout = 5f;
				mapping.mapper.options.ignoreMouseXAxis = true;
				mapping.mapper.options.ignoreMouseYAxis = true;
				mapping.mapper.InputMappedEvent += new Action<InputMapper.InputMappedEventData>(this.OnInputMapped);
				mapping.mapper.StoppedEvent += new Action<InputMapper.StoppedEventData>(this.OnStopped);
				if (controllerMap.controllerType == null || controllerMap.controllerType == 1)
				{
					Game.Log("Menu: Found Keyboard/mouse map: " + controllerMap.name, 2);
					this.keyboardAndMouseMappers.Add(mapping);
				}
				else
				{
					Game.Log("Menu: Found Gamepad map: " + controllerMap.name, 2);
					this.gamepadMappers.Add(mapping);
				}
				this.allMappers.Add(mapping);
			}
		}

		// Token: 0x06003041 RID: 12353 RVA: 0x00214A50 File Offset: 0x00212C50
		public void OnControllerSelected(int controllerType)
		{
			this.SetSelectedController(controllerType);
		}

		// Token: 0x06003042 RID: 12354 RVA: 0x00214A5C File Offset: 0x00212C5C
		public void ResetControls()
		{
			Game.Log("Menu: Reset Controls...", 2);
			InputController.Instance.player.controllers.maps.LoadDefaultMaps(0);
			InputController.Instance.player.controllers.maps.LoadDefaultMaps(1);
			InputController.Instance.player.controllers.maps.LoadDefaultMaps(2);
			InputController.Instance.player.controllers.maps.LoadDefaultMaps(20);
			if (ReInput.userDataStore != null)
			{
				Game.Log("Menu: Controls config saved", 2);
				ReInput.userDataStore.Save();
			}
			this.InitializeUI();
		}

		// Token: 0x06003043 RID: 12355 RVA: 0x00214B00 File Offset: 0x00212D00
		public void OnInputFieldClicked(RemapController remap)
		{
			if (!this.enableInputMapping)
			{
				return;
			}
			this.enableInputMapping = false;
			SimpleControlRemappingSOD.Row row = this.rows.Find((SimpleControlRemappingSOD.Row item) => item.button == remap);
			if (row == null)
			{
				return;
			}
			if (this.controllers.Count <= 0)
			{
				return;
			}
			int num = -1;
			ControllerMap controllerMap = null;
			foreach (ControllerMap controllerMap2 in this.controllerMaps)
			{
				foreach (ActionElementMap actionElementMap in controllerMap2.ElementMapsWithAction(row.action.id))
				{
					if (actionElementMap.ShowInField(row.actionRange))
					{
						num = actionElementMap.id;
						controllerMap = controllerMap2;
						break;
					}
				}
				if (controllerMap != null)
				{
					break;
				}
			}
			this._replaceTargetMapping = new SimpleControlRemappingSOD.TargetMapping
			{
				actionElementMapId = num,
				controllerMap = controllerMap
			};
			base.StartCoroutine(this.StartListeningDelayed(row, num));
		}

		// Token: 0x06003044 RID: 12356 RVA: 0x00214C34 File Offset: 0x00212E34
		private IEnumerator StartListeningDelayed(SimpleControlRemappingSOD.Row row, int actionElementMapToReplaceId)
		{
			Game.Log("Menu: Listening for remap...", 2);
			row.button.primaryControlButton.icon.gameObject.SetActive(true);
			foreach (SimpleControlRemappingSOD.Row row2 in this.rows)
			{
				row.button.primaryControlButton.SetInteractable(false);
			}
			Game.Log("Menu: Disable menu raycaster", 2);
			MainMenuController.Instance.raycaster.enabled = false;
			this.statusUIText.text = Strings.Get("ui.interface", "Press Any Key", Strings.Casing.asIs, false, false, false, null);
			row.button.primaryControlButton.text.text = Strings.Get("ui.interface", "Press Any Key", Strings.Casing.asIs, false, false, false, null);
			yield return new WaitForSeconds(0.1f);
			if (this.selectedControllerType == null || this.selectedControllerType == 1)
			{
				this.listeningForRemap = true;
				if (this.keyboardAndMouseMappers.Count <= 0)
				{
					Game.Log("Menu: There are no keyboard/mouse mappers to listen to! " + this.keyboardAndMouseMappers.Count.ToString() + "/" + this.allMappers.Count.ToString(), 2);
					this.StopMapping(false);
				}
				using (List<SimpleControlRemappingSOD.Mapping>.Enumerator enumerator2 = this.keyboardAndMouseMappers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SimpleControlRemappingSOD.Mapping mapping = enumerator2.Current;
						Game.Log("Menu: Listening on map " + mapping.map.controllerType.ToString() + "...", 2);
						mapping.mapper.options.allowKeyboardModifierKeyAsPrimary = true;
						mapping.mapper.options.allowKeyboardKeysWithModifiers = false;
						mapping.mapper.options.checkForConflicts = false;
						mapping.mapper.options.checkForConflictsWithAllPlayers = false;
						mapping.mapper.options.checkForConflictsWithSelf = false;
						mapping.mapper.options.checkForConflictsWithSystemPlayer = false;
						mapping.mapper.options.defaultActionWhenConflictFound = 3;
						mapping.mapper.Start(new InputMapper.Context
						{
							actionId = row.action.id,
							controllerMap = mapping.map,
							actionRange = row.actionRange,
							actionElementMapToReplace = mapping.map.GetElementMap(actionElementMapToReplaceId)
						});
					}
					yield break;
				}
			}
			this.listeningForRemap = true;
			if (this.gamepadMappers.Count <= 0)
			{
				Game.Log("Menu: There are no gamepad mappers to listen to! " + this.gamepadMappers.Count.ToString() + "/" + this.allMappers.Count.ToString(), 2);
				this.StopMapping(false);
			}
			using (List<SimpleControlRemappingSOD.Mapping>.Enumerator enumerator2 = this.gamepadMappers.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SimpleControlRemappingSOD.Mapping mapping2 = enumerator2.Current;
					Game.Log("Menu: Listening on map " + mapping2.map.controllerType.ToString() + "...", 2);
					mapping2.mapper.options.checkForConflicts = false;
					mapping2.mapper.options.checkForConflictsWithAllPlayers = false;
					mapping2.mapper.options.checkForConflictsWithSelf = false;
					mapping2.mapper.options.checkForConflictsWithSystemPlayer = false;
					mapping2.mapper.options.defaultActionWhenConflictFound = 3;
					mapping2.mapper.Start(new InputMapper.Context
					{
						actionId = row.action.id,
						controllerMap = mapping2.map,
						actionRange = row.actionRange,
						actionElementMapToReplace = mapping2.map.GetElementMap(actionElementMapToReplaceId)
					});
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x06003045 RID: 12357 RVA: 0x00214C51 File Offset: 0x00212E51
		private void OnControllerChanged(ControllerStatusChangedEventArgs args)
		{
			this.SetSelectedController(this.selectedControllerType);
		}

		// Token: 0x06003046 RID: 12358 RVA: 0x00214C60 File Offset: 0x00212E60
		private void OnInputMapped(InputMapper.InputMappedEventData data)
		{
			this.StopMapping(false);
			Game.Log("Menu: OnInputMapped: " + data.actionElementMap.actionDescriptiveName + " = " + data.actionElementMap.elementIdentifierName.ToString(), 2);
			if (this._replaceTargetMapping.controllerMap != null)
			{
				Game.Log("Menu: Replacement detected...", 2);
				if (data.actionElementMap.controllerMap != this._replaceTargetMapping.controllerMap)
				{
					Game.Log("Menu: Deleting other control", 2);
					this._replaceTargetMapping.controllerMap.DeleteElementMap(this._replaceTargetMapping.actionElementMapId);
				}
			}
			for (int i = 0; i < this.controllerMaps.Count; i++)
			{
				ControllerMap controllerMap = this.controllerMaps[i];
				string text = "Menu: Removing conflicts with ";
				ActionElementMap actionElementMap = data.actionElementMap;
				Game.Log(text + ((actionElementMap != null) ? actionElementMap.ToString() : null), 2);
				List<int> list = new List<int>();
				foreach (ActionElementMap actionElementMap2 in controllerMap.ElementMapsWithAction(data.actionElementMap.actionId))
				{
					if (actionElementMap2.elementIdentifierId != data.actionElementMap.elementIdentifierId && actionElementMap2.actionDescriptiveName == data.actionElementMap.actionDescriptiveName)
					{
						Game.Log(string.Concat(new string[]
						{
							"Menu: Found existing control for ",
							actionElementMap2.actionDescriptiveName,
							" = ",
							actionElementMap2.elementIdentifierName.ToString(),
							", deleting..."
						}), 2);
						list.Add(actionElementMap2.id);
					}
				}
				foreach (int num in list)
				{
					controllerMap.DeleteElementMap(num);
				}
			}
			if (ReInput.userDataStore != null)
			{
				Game.Log("Menu: Controls config saved", 2);
				ReInput.userDataStore.Save();
			}
			this.RedrawUI();
		}

		// Token: 0x06003047 RID: 12359 RVA: 0x00214E78 File Offset: 0x00213078
		public void StopMapping(bool removeEvents = false)
		{
			Game.Log("Menu: Stop mapping", 2);
			foreach (SimpleControlRemappingSOD.Mapping mapping in this.allMappers)
			{
				mapping.mapper.Stop();
				if (removeEvents)
				{
					mapping.mapper.RemoveAllEventListeners();
				}
			}
			this.listeningForRemap = false;
			InteractionController.Instance.inputCooldown = 0.1f;
			this.OnStopped(null);
		}

		// Token: 0x06003048 RID: 12360 RVA: 0x00214F08 File Offset: 0x00213108
		private void OnStopped(InputMapper.StoppedEventData data)
		{
			Game.Log("Menu: Listening stopped, re-enabling proper controls...", 2);
			this.listeningForRemap = false;
			for (int i = 0; i < this.rows.Count; i++)
			{
				SimpleControlRemappingSOD.Row row = this.rows[i];
				InputAction action = this.rows[i].action;
				row.button.primaryControlButton.SetInteractable(true);
			}
			this.statusUIText.text = string.Empty;
			InputController.Instance.player.controllers.maps.SetMapsEnabled(true, "UI");
			if (!MainMenuController.Instance.raycaster.enabled)
			{
				Game.Log("Menu: Enable menu raycaster", 2);
				MainMenuController.Instance.raycaster.enabled = true;
			}
			base.StartCoroutine(this.RemapDelay());
			this.RedrawUI();
		}

		// Token: 0x06003049 RID: 12361 RVA: 0x00214FDA File Offset: 0x002131DA
		private IEnumerator RemapDelay()
		{
			float delay = 1f;
			while (delay > 0f && !this.enableInputMapping)
			{
				delay -= Time.deltaTime;
				yield return null;
			}
			this.enableInputMapping = true;
			yield break;
		}

		// Token: 0x04004AEC RID: 19180
		private const string layout = "Default";

		// Token: 0x04004AED RID: 19181
		private const string uiCategory = "UI";

		// Token: 0x04004AEE RID: 19182
		public List<string> categories = new List<string>();

		// Token: 0x04004AEF RID: 19183
		public bool enableInputMapping = true;

		// Token: 0x04004AF0 RID: 19184
		public bool listeningForRemap;

		// Token: 0x04004AF1 RID: 19185
		private List<SimpleControlRemappingSOD.Mapping> allMappers = new List<SimpleControlRemappingSOD.Mapping>();

		// Token: 0x04004AF2 RID: 19186
		private List<SimpleControlRemappingSOD.Mapping> keyboardAndMouseMappers = new List<SimpleControlRemappingSOD.Mapping>();

		// Token: 0x04004AF3 RID: 19187
		private List<SimpleControlRemappingSOD.Mapping> gamepadMappers = new List<SimpleControlRemappingSOD.Mapping>();

		// Token: 0x04004AF4 RID: 19188
		public GameObject buttonPrefab;

		// Token: 0x04004AF5 RID: 19189
		public RectTransform fieldGroupTransform;

		// Token: 0x04004AF6 RID: 19190
		public TextMeshProUGUI statusUIText;

		// Token: 0x04004AF7 RID: 19191
		public ToggleController schemeToggle;

		// Token: 0x04004AF8 RID: 19192
		public Button backButton;

		// Token: 0x04004AF9 RID: 19193
		public Button resetControlsButton;

		// Token: 0x04004AFA RID: 19194
		public Button aboveButton;

		// Token: 0x04004AFB RID: 19195
		public Button aboveButton2;

		// Token: 0x04004AFC RID: 19196
		public ControllerType selectedControllerType;

		// Token: 0x04004AFD RID: 19197
		private int selectedControllerId;

		// Token: 0x04004AFE RID: 19198
		public List<SimpleControlRemappingSOD.Row> rows = new List<SimpleControlRemappingSOD.Row>();

		// Token: 0x04004AFF RID: 19199
		private SimpleControlRemappingSOD.TargetMapping _replaceTargetMapping;

		// Token: 0x04004B00 RID: 19200
		private List<Controller> controllers = new List<Controller>();

		// Token: 0x04004B01 RID: 19201
		private List<ControllerMap> controllerMaps = new List<ControllerMap>();

		// Token: 0x04004B02 RID: 19202
		private static SimpleControlRemappingSOD _instance;

		// Token: 0x020008CC RID: 2252
		[Serializable]
		public class Row
		{
			// Token: 0x04004B03 RID: 19203
			public InputAction action;

			// Token: 0x04004B04 RID: 19204
			public AxisRange actionRange;

			// Token: 0x04004B05 RID: 19205
			public RemapController button;
		}

		// Token: 0x020008CD RID: 2253
		private struct TargetMapping
		{
			// Token: 0x04004B06 RID: 19206
			public ControllerMap controllerMap;

			// Token: 0x04004B07 RID: 19207
			public int actionElementMapId;
		}

		// Token: 0x020008CE RID: 2254
		private struct Mapping
		{
			// Token: 0x04004B08 RID: 19208
			public InputMapper mapper;

			// Token: 0x04004B09 RID: 19209
			public ControllerMap map;
		}
	}
}
