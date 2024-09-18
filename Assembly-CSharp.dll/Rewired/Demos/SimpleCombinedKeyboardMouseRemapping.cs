using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.Demos
{
	// Token: 0x020008C0 RID: 2240
	[AddComponentMenu("")]
	public class SimpleCombinedKeyboardMouseRemapping : MonoBehaviour
	{
		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06002FFB RID: 12283 RVA: 0x00212D5D File Offset: 0x00210F5D
		private Player player
		{
			get
			{
				return ReInput.players.GetPlayer(0);
			}
		}

		// Token: 0x06002FFC RID: 12284 RVA: 0x00212D6C File Offset: 0x00210F6C
		private void OnEnable()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			this.inputMapper_keyboard.options.timeout = 5f;
			this.inputMapper_mouse.options.timeout = 5f;
			this.inputMapper_mouse.options.ignoreMouseXAxis = true;
			this.inputMapper_mouse.options.ignoreMouseYAxis = true;
			this.inputMapper_keyboard.options.allowButtonsOnFullAxisAssignment = false;
			this.inputMapper_mouse.options.allowButtonsOnFullAxisAssignment = false;
			this.inputMapper_keyboard.InputMappedEvent += new Action<InputMapper.InputMappedEventData>(this.OnInputMapped);
			this.inputMapper_keyboard.StoppedEvent += new Action<InputMapper.StoppedEventData>(this.OnStopped);
			this.inputMapper_mouse.InputMappedEvent += new Action<InputMapper.InputMappedEventData>(this.OnInputMapped);
			this.inputMapper_mouse.StoppedEvent += new Action<InputMapper.StoppedEventData>(this.OnStopped);
			this.InitializeUI();
		}

		// Token: 0x06002FFD RID: 12285 RVA: 0x00212E51 File Offset: 0x00211051
		private void OnDisable()
		{
			this.inputMapper_keyboard.Stop();
			this.inputMapper_mouse.Stop();
			this.inputMapper_keyboard.RemoveAllEventListeners();
			this.inputMapper_mouse.RemoveAllEventListeners();
		}

		// Token: 0x06002FFE RID: 12286 RVA: 0x00212E80 File Offset: 0x00211080
		private void RedrawUI()
		{
			this.controllerNameUIText.text = "Keyboard/Mouse";
			for (int i = 0; i < this.rows.Count; i++)
			{
				SimpleCombinedKeyboardMouseRemapping.Row row = this.rows[i];
				InputAction action = this.rows[i].action;
				string text = string.Empty;
				int actionElementMapId = -1;
				for (int j = 0; j < 2; j++)
				{
					ControllerType controllerType = (j == 0) ? 0 : 1;
					foreach (ActionElementMap actionElementMap in this.player.controllers.maps.GetMap(controllerType, 0, "Default", "Default").ElementMapsWithAction(action.id))
					{
						if (actionElementMap.ShowInField(row.actionRange))
						{
							text = actionElementMap.elementIdentifierName;
							actionElementMapId = actionElementMap.id;
							break;
						}
					}
					if (actionElementMapId >= 0)
					{
						break;
					}
				}
				row.text.text = text;
				row.button.onClick.RemoveAllListeners();
				int index = i;
				row.button.onClick.AddListener(delegate()
				{
					this.OnInputFieldClicked(index, actionElementMapId);
				});
			}
		}

		// Token: 0x06002FFF RID: 12287 RVA: 0x00212FEC File Offset: 0x002111EC
		private void ClearUI()
		{
			this.controllerNameUIText.text = string.Empty;
			for (int i = 0; i < this.rows.Count; i++)
			{
				this.rows[i].text.text = string.Empty;
			}
		}

		// Token: 0x06003000 RID: 12288 RVA: 0x0021303C File Offset: 0x0021123C
		private void InitializeUI()
		{
			foreach (object obj in this.actionGroupTransform)
			{
				Object.Destroy(((Transform)obj).gameObject);
			}
			foreach (object obj2 in this.fieldGroupTransform)
			{
				Object.Destroy(((Transform)obj2).gameObject);
			}
			foreach (InputAction inputAction in ReInput.mapping.ActionsInCategory("Default"))
			{
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
			this.RedrawUI();
		}

		// Token: 0x06003001 RID: 12289 RVA: 0x002131B4 File Offset: 0x002113B4
		private void CreateUIRow(InputAction action, AxisRange actionRange, string label)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.textPrefab);
			gameObject.transform.SetParent(this.actionGroupTransform);
			gameObject.transform.SetAsLastSibling();
			gameObject.GetComponent<Text>().text = label;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.buttonPrefab);
			gameObject2.transform.SetParent(this.fieldGroupTransform);
			gameObject2.transform.SetAsLastSibling();
			this.rows.Add(new SimpleCombinedKeyboardMouseRemapping.Row
			{
				action = action,
				actionRange = actionRange,
				button = gameObject2.GetComponent<Button>(),
				text = gameObject2.GetComponentInChildren<Text>()
			});
		}

		// Token: 0x06003002 RID: 12290 RVA: 0x00213254 File Offset: 0x00211454
		private void OnInputFieldClicked(int index, int actionElementMapToReplaceId)
		{
			if (index < 0 || index >= this.rows.Count)
			{
				return;
			}
			ControllerMap map = this.player.controllers.maps.GetMap(0, 0, "Default", "Default");
			ControllerMap map2 = this.player.controllers.maps.GetMap(1, 0, "Default", "Default");
			ControllerMap controllerMap;
			if (map.ContainsElementMap(actionElementMapToReplaceId))
			{
				controllerMap = map;
			}
			else if (map2.ContainsElementMap(actionElementMapToReplaceId))
			{
				controllerMap = map2;
			}
			else
			{
				controllerMap = null;
			}
			this._replaceTargetMapping = new SimpleCombinedKeyboardMouseRemapping.TargetMapping
			{
				actionElementMapId = actionElementMapToReplaceId,
				controllerMap = controllerMap
			};
			base.StartCoroutine(this.StartListeningDelayed(index, map, map2, actionElementMapToReplaceId));
		}

		// Token: 0x06003003 RID: 12291 RVA: 0x00213304 File Offset: 0x00211504
		private IEnumerator StartListeningDelayed(int index, ControllerMap keyboardMap, ControllerMap mouseMap, int actionElementMapToReplaceId)
		{
			yield return new WaitForSeconds(0.1f);
			this.inputMapper_keyboard.Start(new InputMapper.Context
			{
				actionId = this.rows[index].action.id,
				controllerMap = keyboardMap,
				actionRange = this.rows[index].actionRange,
				actionElementMapToReplace = keyboardMap.GetElementMap(actionElementMapToReplaceId)
			});
			this.inputMapper_mouse.Start(new InputMapper.Context
			{
				actionId = this.rows[index].action.id,
				controllerMap = mouseMap,
				actionRange = this.rows[index].actionRange,
				actionElementMapToReplace = mouseMap.GetElementMap(actionElementMapToReplaceId)
			});
			this.player.controllers.maps.SetMapsEnabled(false, "UI");
			this.statusUIText.text = "Listening...";
			yield break;
		}

		// Token: 0x06003004 RID: 12292 RVA: 0x00213330 File Offset: 0x00211530
		private void OnInputMapped(InputMapper.InputMappedEventData data)
		{
			this.inputMapper_keyboard.Stop();
			this.inputMapper_mouse.Stop();
			if (this._replaceTargetMapping.controllerMap != null && data.actionElementMap.controllerMap != this._replaceTargetMapping.controllerMap)
			{
				this._replaceTargetMapping.controllerMap.DeleteElementMap(this._replaceTargetMapping.actionElementMapId);
			}
			this.RedrawUI();
		}

		// Token: 0x06003005 RID: 12293 RVA: 0x0021339A File Offset: 0x0021159A
		private void OnStopped(InputMapper.StoppedEventData data)
		{
			this.statusUIText.text = string.Empty;
			this.player.controllers.maps.SetMapsEnabled(true, "UI");
		}

		// Token: 0x04004AB1 RID: 19121
		private const string category = "Default";

		// Token: 0x04004AB2 RID: 19122
		private const string layout = "Default";

		// Token: 0x04004AB3 RID: 19123
		private const string uiCategory = "UI";

		// Token: 0x04004AB4 RID: 19124
		private InputMapper inputMapper_keyboard = new InputMapper();

		// Token: 0x04004AB5 RID: 19125
		private InputMapper inputMapper_mouse = new InputMapper();

		// Token: 0x04004AB6 RID: 19126
		public GameObject buttonPrefab;

		// Token: 0x04004AB7 RID: 19127
		public GameObject textPrefab;

		// Token: 0x04004AB8 RID: 19128
		public RectTransform fieldGroupTransform;

		// Token: 0x04004AB9 RID: 19129
		public RectTransform actionGroupTransform;

		// Token: 0x04004ABA RID: 19130
		public Text controllerNameUIText;

		// Token: 0x04004ABB RID: 19131
		public Text statusUIText;

		// Token: 0x04004ABC RID: 19132
		private List<SimpleCombinedKeyboardMouseRemapping.Row> rows = new List<SimpleCombinedKeyboardMouseRemapping.Row>();

		// Token: 0x04004ABD RID: 19133
		private SimpleCombinedKeyboardMouseRemapping.TargetMapping _replaceTargetMapping;

		// Token: 0x020008C1 RID: 2241
		private class Row
		{
			// Token: 0x04004ABE RID: 19134
			public InputAction action;

			// Token: 0x04004ABF RID: 19135
			public AxisRange actionRange;

			// Token: 0x04004AC0 RID: 19136
			public Button button;

			// Token: 0x04004AC1 RID: 19137
			public Text text;
		}

		// Token: 0x020008C2 RID: 2242
		private struct TargetMapping
		{
			// Token: 0x04004AC2 RID: 19138
			public ControllerMap controllerMap;

			// Token: 0x04004AC3 RID: 19139
			public int actionElementMapId;
		}
	}
}
