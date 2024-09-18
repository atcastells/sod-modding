using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.Demos
{
	// Token: 0x020008C5 RID: 2245
	[AddComponentMenu("")]
	public class SimpleControlRemapping : MonoBehaviour
	{
		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06003010 RID: 12304 RVA: 0x00212D5D File Offset: 0x00210F5D
		private Player player
		{
			get
			{
				return ReInput.players.GetPlayer(0);
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06003011 RID: 12305 RVA: 0x00213580 File Offset: 0x00211780
		private ControllerMap controllerMap
		{
			get
			{
				if (this.controller == null)
				{
					return null;
				}
				return this.player.controllers.maps.GetMap(this.controller.type, this.controller.id, "Default", "Default");
			}
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06003012 RID: 12306 RVA: 0x002135CC File Offset: 0x002117CC
		private Controller controller
		{
			get
			{
				return this.player.controllers.GetController(this.selectedControllerType, this.selectedControllerId);
			}
		}

		// Token: 0x06003013 RID: 12307 RVA: 0x002135EC File Offset: 0x002117EC
		private void OnEnable()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			this.inputMapper.options.timeout = 5f;
			this.inputMapper.options.ignoreMouseXAxis = true;
			this.inputMapper.options.ignoreMouseYAxis = true;
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnControllerChanged);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnControllerChanged);
			this.inputMapper.InputMappedEvent += new Action<InputMapper.InputMappedEventData>(this.OnInputMapped);
			this.inputMapper.StoppedEvent += new Action<InputMapper.StoppedEventData>(this.OnStopped);
			this.InitializeUI();
		}

		// Token: 0x06003014 RID: 12308 RVA: 0x0021368E File Offset: 0x0021188E
		private void OnDisable()
		{
			this.inputMapper.Stop();
			this.inputMapper.RemoveAllEventListeners();
			ReInput.ControllerConnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerChanged);
			ReInput.ControllerDisconnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerChanged);
		}

		// Token: 0x06003015 RID: 12309 RVA: 0x002136C8 File Offset: 0x002118C8
		private void RedrawUI()
		{
			if (this.controller == null)
			{
				this.ClearUI();
				return;
			}
			this.controllerNameUIText.text = this.controller.name;
			for (int i = 0; i < this.rows.Count; i++)
			{
				SimpleControlRemapping.Row row = this.rows[i];
				InputAction action = this.rows[i].action;
				string text = string.Empty;
				int actionElementMapId = -1;
				foreach (ActionElementMap actionElementMap in this.controllerMap.ElementMapsWithAction(action.id))
				{
					if (actionElementMap.ShowInField(row.actionRange))
					{
						text = actionElementMap.elementIdentifierName;
						actionElementMapId = actionElementMap.id;
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

		// Token: 0x06003016 RID: 12310 RVA: 0x00213804 File Offset: 0x00211A04
		private void ClearUI()
		{
			if (this.selectedControllerType == 2)
			{
				this.controllerNameUIText.text = "No joysticks attached";
			}
			else
			{
				this.controllerNameUIText.text = string.Empty;
			}
			for (int i = 0; i < this.rows.Count; i++)
			{
				this.rows[i].text.text = string.Empty;
			}
		}

		// Token: 0x06003017 RID: 12311 RVA: 0x00213870 File Offset: 0x00211A70
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

		// Token: 0x06003018 RID: 12312 RVA: 0x002139E8 File Offset: 0x00211BE8
		private void CreateUIRow(InputAction action, AxisRange actionRange, string label)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.textPrefab);
			gameObject.transform.SetParent(this.actionGroupTransform);
			gameObject.transform.SetAsLastSibling();
			gameObject.GetComponent<Text>().text = label;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.buttonPrefab);
			gameObject2.transform.SetParent(this.fieldGroupTransform);
			gameObject2.transform.SetAsLastSibling();
			this.rows.Add(new SimpleControlRemapping.Row
			{
				action = action,
				actionRange = actionRange,
				button = gameObject2.GetComponent<Button>(),
				text = gameObject2.GetComponentInChildren<Text>()
			});
		}

		// Token: 0x06003019 RID: 12313 RVA: 0x00213A88 File Offset: 0x00211C88
		private void SetSelectedController(ControllerType controllerType)
		{
			bool flag = false;
			if (controllerType != this.selectedControllerType)
			{
				this.selectedControllerType = controllerType;
				flag = true;
			}
			int num = this.selectedControllerId;
			if (this.selectedControllerType == 2)
			{
				if (this.player.controllers.joystickCount > 0)
				{
					this.selectedControllerId = this.player.controllers.Joysticks[0].id;
				}
				else
				{
					this.selectedControllerId = -1;
				}
			}
			else
			{
				this.selectedControllerId = 0;
			}
			if (this.selectedControllerId != num)
			{
				flag = true;
			}
			if (flag)
			{
				this.inputMapper.Stop();
				this.RedrawUI();
			}
		}

		// Token: 0x0600301A RID: 12314 RVA: 0x00213B1E File Offset: 0x00211D1E
		public void OnControllerSelected(int controllerType)
		{
			this.SetSelectedController(controllerType);
		}

		// Token: 0x0600301B RID: 12315 RVA: 0x00213B27 File Offset: 0x00211D27
		private void OnInputFieldClicked(int index, int actionElementMapToReplaceId)
		{
			if (index < 0 || index >= this.rows.Count)
			{
				return;
			}
			if (this.controller == null)
			{
				return;
			}
			base.StartCoroutine(this.StartListeningDelayed(index, actionElementMapToReplaceId));
		}

		// Token: 0x0600301C RID: 12316 RVA: 0x00213B54 File Offset: 0x00211D54
		private IEnumerator StartListeningDelayed(int index, int actionElementMapToReplaceId)
		{
			yield return new WaitForSeconds(0.1f);
			this.inputMapper.Start(new InputMapper.Context
			{
				actionId = this.rows[index].action.id,
				controllerMap = this.controllerMap,
				actionRange = this.rows[index].actionRange,
				actionElementMapToReplace = this.controllerMap.GetElementMap(actionElementMapToReplaceId)
			});
			this.player.controllers.maps.SetMapsEnabled(false, "UI");
			this.statusUIText.text = "Listening...";
			yield break;
		}

		// Token: 0x0600301D RID: 12317 RVA: 0x00213B71 File Offset: 0x00211D71
		private void OnControllerChanged(ControllerStatusChangedEventArgs args)
		{
			this.SetSelectedController(this.selectedControllerType);
		}

		// Token: 0x0600301E RID: 12318 RVA: 0x00213B7F File Offset: 0x00211D7F
		private void OnInputMapped(InputMapper.InputMappedEventData data)
		{
			this.RedrawUI();
		}

		// Token: 0x0600301F RID: 12319 RVA: 0x00213B87 File Offset: 0x00211D87
		private void OnStopped(InputMapper.StoppedEventData data)
		{
			this.statusUIText.text = string.Empty;
			this.player.controllers.maps.SetMapsEnabled(true, "UI");
		}

		// Token: 0x04004ACE RID: 19150
		private const string category = "Default";

		// Token: 0x04004ACF RID: 19151
		private const string layout = "Default";

		// Token: 0x04004AD0 RID: 19152
		private const string uiCategory = "UI";

		// Token: 0x04004AD1 RID: 19153
		private InputMapper inputMapper = new InputMapper();

		// Token: 0x04004AD2 RID: 19154
		public GameObject buttonPrefab;

		// Token: 0x04004AD3 RID: 19155
		public GameObject textPrefab;

		// Token: 0x04004AD4 RID: 19156
		public RectTransform fieldGroupTransform;

		// Token: 0x04004AD5 RID: 19157
		public RectTransform actionGroupTransform;

		// Token: 0x04004AD6 RID: 19158
		public Text controllerNameUIText;

		// Token: 0x04004AD7 RID: 19159
		public Text statusUIText;

		// Token: 0x04004AD8 RID: 19160
		private ControllerType selectedControllerType;

		// Token: 0x04004AD9 RID: 19161
		private int selectedControllerId;

		// Token: 0x04004ADA RID: 19162
		private List<SimpleControlRemapping.Row> rows = new List<SimpleControlRemapping.Row>();

		// Token: 0x020008C6 RID: 2246
		private class Row
		{
			// Token: 0x04004ADB RID: 19163
			public InputAction action;

			// Token: 0x04004ADC RID: 19164
			public AxisRange actionRange;

			// Token: 0x04004ADD RID: 19165
			public Button button;

			// Token: 0x04004ADE RID: 19166
			public Text text;
		}
	}
}
