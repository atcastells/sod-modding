using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos.GamepadTemplateUI
{
	// Token: 0x020008D4 RID: 2260
	public class GamepadTemplateUI : MonoBehaviour
	{
		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06003067 RID: 12391 RVA: 0x002159EA File Offset: 0x00213BEA
		private Player player
		{
			get
			{
				return ReInput.players.GetPlayer(this.playerId);
			}
		}

		// Token: 0x06003068 RID: 12392 RVA: 0x002159FC File Offset: 0x00213BFC
		private void Awake()
		{
			this._uiElementsArray = new GamepadTemplateUI.UIElement[]
			{
				new GamepadTemplateUI.UIElement(0, this.leftStickX),
				new GamepadTemplateUI.UIElement(1, this.leftStickY),
				new GamepadTemplateUI.UIElement(17, this.leftStickButton),
				new GamepadTemplateUI.UIElement(2, this.rightStickX),
				new GamepadTemplateUI.UIElement(3, this.rightStickY),
				new GamepadTemplateUI.UIElement(18, this.rightStickButton),
				new GamepadTemplateUI.UIElement(4, this.actionBottomRow1),
				new GamepadTemplateUI.UIElement(5, this.actionBottomRow2),
				new GamepadTemplateUI.UIElement(6, this.actionBottomRow3),
				new GamepadTemplateUI.UIElement(7, this.actionTopRow1),
				new GamepadTemplateUI.UIElement(8, this.actionTopRow2),
				new GamepadTemplateUI.UIElement(9, this.actionTopRow3),
				new GamepadTemplateUI.UIElement(14, this.center1),
				new GamepadTemplateUI.UIElement(15, this.center2),
				new GamepadTemplateUI.UIElement(16, this.center3),
				new GamepadTemplateUI.UIElement(19, this.dPadUp),
				new GamepadTemplateUI.UIElement(20, this.dPadRight),
				new GamepadTemplateUI.UIElement(21, this.dPadDown),
				new GamepadTemplateUI.UIElement(22, this.dPadLeft),
				new GamepadTemplateUI.UIElement(10, this.leftShoulder),
				new GamepadTemplateUI.UIElement(11, this.leftTrigger),
				new GamepadTemplateUI.UIElement(12, this.rightShoulder),
				new GamepadTemplateUI.UIElement(13, this.rightTrigger)
			};
			for (int i = 0; i < this._uiElementsArray.Length; i++)
			{
				this._uiElements.Add(this._uiElementsArray[i].id, this._uiElementsArray[i].element);
			}
			this._sticks = new GamepadTemplateUI.Stick[]
			{
				new GamepadTemplateUI.Stick(this.leftStick, 0, 1),
				new GamepadTemplateUI.Stick(this.rightStick, 2, 3)
			};
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnControllerConnected);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnControllerDisconnected);
		}

		// Token: 0x06003069 RID: 12393 RVA: 0x00215C11 File Offset: 0x00213E11
		private void Start()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			this.DrawLabels();
		}

		// Token: 0x0600306A RID: 12394 RVA: 0x00215C21 File Offset: 0x00213E21
		private void OnDestroy()
		{
			ReInput.ControllerConnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerConnected);
			ReInput.ControllerDisconnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerDisconnected);
		}

		// Token: 0x0600306B RID: 12395 RVA: 0x00215C45 File Offset: 0x00213E45
		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			this.DrawActiveElements();
		}

		// Token: 0x0600306C RID: 12396 RVA: 0x00215C58 File Offset: 0x00213E58
		private void DrawActiveElements()
		{
			for (int i = 0; i < this._uiElementsArray.Length; i++)
			{
				this._uiElementsArray[i].element.Deactivate();
			}
			for (int j = 0; j < this._sticks.Length; j++)
			{
				this._sticks[j].Reset();
			}
			IList<InputAction> actions = ReInput.mapping.Actions;
			for (int k = 0; k < actions.Count; k++)
			{
				this.ActivateElements(this.player, actions[k].id);
			}
		}

		// Token: 0x0600306D RID: 12397 RVA: 0x00215CE0 File Offset: 0x00213EE0
		private void ActivateElements(Player player, int actionId)
		{
			float axis = player.GetAxis(actionId);
			if (axis == 0f)
			{
				return;
			}
			IList<InputActionSourceData> currentInputSources = player.GetCurrentInputSources(actionId);
			for (int i = 0; i < currentInputSources.Count; i++)
			{
				InputActionSourceData inputActionSourceData = currentInputSources[i];
				IGamepadTemplate template = inputActionSourceData.controller.GetTemplate<IGamepadTemplate>();
				if (template != null)
				{
					template.GetElementTargets(inputActionSourceData.actionElementMap, this._tempTargetList);
					for (int j = 0; j < this._tempTargetList.Count; j++)
					{
						ControllerTemplateElementTarget controllerTemplateElementTarget = this._tempTargetList[j];
						int id = controllerTemplateElementTarget.element.id;
						ControllerUIElement controllerUIElement = this._uiElements[id];
						if (controllerTemplateElementTarget.elementType == null)
						{
							controllerUIElement.Activate(axis);
						}
						else if (controllerTemplateElementTarget.elementType == 1 && (player.GetButton(actionId) || player.GetNegativeButton(actionId)))
						{
							controllerUIElement.Activate(1f);
						}
						GamepadTemplateUI.Stick stick = this.GetStick(id);
						if (stick != null)
						{
							stick.SetAxisPosition(id, axis * 20f);
						}
					}
				}
			}
		}

		// Token: 0x0600306E RID: 12398 RVA: 0x00215DFC File Offset: 0x00213FFC
		private void DrawLabels()
		{
			for (int i = 0; i < this._uiElementsArray.Length; i++)
			{
				this._uiElementsArray[i].element.ClearLabels();
			}
			IList<InputAction> actions = ReInput.mapping.Actions;
			for (int j = 0; j < actions.Count; j++)
			{
				this.DrawLabels(this.player, actions[j]);
			}
		}

		// Token: 0x0600306F RID: 12399 RVA: 0x00215E60 File Offset: 0x00214060
		private void DrawLabels(Player player, InputAction action)
		{
			Controller firstControllerWithTemplate = player.controllers.GetFirstControllerWithTemplate<IGamepadTemplate>();
			if (firstControllerWithTemplate == null)
			{
				return;
			}
			IGamepadTemplate template = firstControllerWithTemplate.GetTemplate<IGamepadTemplate>();
			ControllerMap map = player.controllers.maps.GetMap(firstControllerWithTemplate, "Default", "Default");
			if (map == null)
			{
				return;
			}
			for (int i = 0; i < this._uiElementsArray.Length; i++)
			{
				ControllerUIElement element = this._uiElementsArray[i].element;
				int id = this._uiElementsArray[i].id;
				IControllerTemplateElement element2 = template.GetElement(id);
				this.DrawLabel(element, action, map, template, element2);
			}
		}

		// Token: 0x06003070 RID: 12400 RVA: 0x00215EEC File Offset: 0x002140EC
		private void DrawLabel(ControllerUIElement uiElement, InputAction action, ControllerMap controllerMap, IControllerTemplate template, IControllerTemplateElement element)
		{
			if (element.source == null)
			{
				return;
			}
			if (element.source.type == null)
			{
				IControllerTemplateAxisSource controllerTemplateAxisSource = element.source as IControllerTemplateAxisSource;
				if (controllerTemplateAxisSource.splitAxis)
				{
					ActionElementMap firstElementMapWithElementTarget = controllerMap.GetFirstElementMapWithElementTarget(controllerTemplateAxisSource.positiveTarget, action.id, true);
					if (firstElementMapWithElementTarget != null)
					{
						uiElement.SetLabel(firstElementMapWithElementTarget.actionDescriptiveName, 1);
					}
					firstElementMapWithElementTarget = controllerMap.GetFirstElementMapWithElementTarget(controllerTemplateAxisSource.negativeTarget, action.id, true);
					if (firstElementMapWithElementTarget != null)
					{
						uiElement.SetLabel(firstElementMapWithElementTarget.actionDescriptiveName, 2);
						return;
					}
				}
				else
				{
					ActionElementMap firstElementMapWithElementTarget = controllerMap.GetFirstElementMapWithElementTarget(controllerTemplateAxisSource.fullTarget, action.id, true);
					if (firstElementMapWithElementTarget != null)
					{
						uiElement.SetLabel(firstElementMapWithElementTarget.actionDescriptiveName, 0);
						return;
					}
					ControllerElementTarget controllerElementTarget;
					controllerElementTarget..ctor(controllerTemplateAxisSource.fullTarget);
					controllerElementTarget.axisRange = 1;
					firstElementMapWithElementTarget = controllerMap.GetFirstElementMapWithElementTarget(controllerElementTarget, action.id, true);
					if (firstElementMapWithElementTarget != null)
					{
						uiElement.SetLabel(firstElementMapWithElementTarget.actionDescriptiveName, 1);
					}
					controllerElementTarget..ctor(controllerTemplateAxisSource.fullTarget);
					controllerElementTarget.axisRange = 2;
					firstElementMapWithElementTarget = controllerMap.GetFirstElementMapWithElementTarget(controllerElementTarget, action.id, true);
					if (firstElementMapWithElementTarget != null)
					{
						uiElement.SetLabel(firstElementMapWithElementTarget.actionDescriptiveName, 2);
						return;
					}
				}
			}
			else if (element.source.type == 1)
			{
				IControllerTemplateButtonSource controllerTemplateButtonSource = element.source as IControllerTemplateButtonSource;
				ActionElementMap firstElementMapWithElementTarget = controllerMap.GetFirstElementMapWithElementTarget(controllerTemplateButtonSource.target, action.id, true);
				if (firstElementMapWithElementTarget != null)
				{
					uiElement.SetLabel(firstElementMapWithElementTarget.actionDescriptiveName, 0);
				}
			}
		}

		// Token: 0x06003071 RID: 12401 RVA: 0x00216044 File Offset: 0x00214244
		private GamepadTemplateUI.Stick GetStick(int elementId)
		{
			for (int i = 0; i < this._sticks.Length; i++)
			{
				if (this._sticks[i].ContainsElement(elementId))
				{
					return this._sticks[i];
				}
			}
			return null;
		}

		// Token: 0x06003072 RID: 12402 RVA: 0x0021607E File Offset: 0x0021427E
		private void OnControllerConnected(ControllerStatusChangedEventArgs args)
		{
			this.DrawLabels();
		}

		// Token: 0x06003073 RID: 12403 RVA: 0x0021607E File Offset: 0x0021427E
		private void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
		{
			this.DrawLabels();
		}

		// Token: 0x04004B26 RID: 19238
		private const float stickRadius = 20f;

		// Token: 0x04004B27 RID: 19239
		public int playerId;

		// Token: 0x04004B28 RID: 19240
		[SerializeField]
		private RectTransform leftStick;

		// Token: 0x04004B29 RID: 19241
		[SerializeField]
		private RectTransform rightStick;

		// Token: 0x04004B2A RID: 19242
		[SerializeField]
		private ControllerUIElement leftStickX;

		// Token: 0x04004B2B RID: 19243
		[SerializeField]
		private ControllerUIElement leftStickY;

		// Token: 0x04004B2C RID: 19244
		[SerializeField]
		private ControllerUIElement leftStickButton;

		// Token: 0x04004B2D RID: 19245
		[SerializeField]
		private ControllerUIElement rightStickX;

		// Token: 0x04004B2E RID: 19246
		[SerializeField]
		private ControllerUIElement rightStickY;

		// Token: 0x04004B2F RID: 19247
		[SerializeField]
		private ControllerUIElement rightStickButton;

		// Token: 0x04004B30 RID: 19248
		[SerializeField]
		private ControllerUIElement actionBottomRow1;

		// Token: 0x04004B31 RID: 19249
		[SerializeField]
		private ControllerUIElement actionBottomRow2;

		// Token: 0x04004B32 RID: 19250
		[SerializeField]
		private ControllerUIElement actionBottomRow3;

		// Token: 0x04004B33 RID: 19251
		[SerializeField]
		private ControllerUIElement actionTopRow1;

		// Token: 0x04004B34 RID: 19252
		[SerializeField]
		private ControllerUIElement actionTopRow2;

		// Token: 0x04004B35 RID: 19253
		[SerializeField]
		private ControllerUIElement actionTopRow3;

		// Token: 0x04004B36 RID: 19254
		[SerializeField]
		private ControllerUIElement leftShoulder;

		// Token: 0x04004B37 RID: 19255
		[SerializeField]
		private ControllerUIElement leftTrigger;

		// Token: 0x04004B38 RID: 19256
		[SerializeField]
		private ControllerUIElement rightShoulder;

		// Token: 0x04004B39 RID: 19257
		[SerializeField]
		private ControllerUIElement rightTrigger;

		// Token: 0x04004B3A RID: 19258
		[SerializeField]
		private ControllerUIElement center1;

		// Token: 0x04004B3B RID: 19259
		[SerializeField]
		private ControllerUIElement center2;

		// Token: 0x04004B3C RID: 19260
		[SerializeField]
		private ControllerUIElement center3;

		// Token: 0x04004B3D RID: 19261
		[SerializeField]
		private ControllerUIElement dPadUp;

		// Token: 0x04004B3E RID: 19262
		[SerializeField]
		private ControllerUIElement dPadRight;

		// Token: 0x04004B3F RID: 19263
		[SerializeField]
		private ControllerUIElement dPadDown;

		// Token: 0x04004B40 RID: 19264
		[SerializeField]
		private ControllerUIElement dPadLeft;

		// Token: 0x04004B41 RID: 19265
		private GamepadTemplateUI.UIElement[] _uiElementsArray;

		// Token: 0x04004B42 RID: 19266
		private Dictionary<int, ControllerUIElement> _uiElements = new Dictionary<int, ControllerUIElement>();

		// Token: 0x04004B43 RID: 19267
		private IList<ControllerTemplateElementTarget> _tempTargetList = new List<ControllerTemplateElementTarget>(2);

		// Token: 0x04004B44 RID: 19268
		private GamepadTemplateUI.Stick[] _sticks;

		// Token: 0x020008D5 RID: 2261
		private class Stick
		{
			// Token: 0x1700050E RID: 1294
			// (get) Token: 0x06003075 RID: 12405 RVA: 0x002160A5 File Offset: 0x002142A5
			// (set) Token: 0x06003076 RID: 12406 RVA: 0x002160D1 File Offset: 0x002142D1
			public Vector2 position
			{
				get
				{
					if (!(this._transform != null))
					{
						return Vector2.zero;
					}
					return this._transform.anchoredPosition - this._origPosition;
				}
				set
				{
					if (this._transform == null)
					{
						return;
					}
					this._transform.anchoredPosition = this._origPosition + value;
				}
			}

			// Token: 0x06003077 RID: 12407 RVA: 0x002160FC File Offset: 0x002142FC
			public Stick(RectTransform transform, int xAxisElementId, int yAxisElementId)
			{
				if (transform == null)
				{
					return;
				}
				this._transform = transform;
				this._origPosition = this._transform.anchoredPosition;
				this._xAxisElementId = xAxisElementId;
				this._yAxisElementId = yAxisElementId;
			}

			// Token: 0x06003078 RID: 12408 RVA: 0x0021614D File Offset: 0x0021434D
			public void Reset()
			{
				if (this._transform == null)
				{
					return;
				}
				this._transform.anchoredPosition = this._origPosition;
			}

			// Token: 0x06003079 RID: 12409 RVA: 0x0021616F File Offset: 0x0021436F
			public bool ContainsElement(int elementId)
			{
				return !(this._transform == null) && (elementId == this._xAxisElementId || elementId == this._yAxisElementId);
			}

			// Token: 0x0600307A RID: 12410 RVA: 0x00216198 File Offset: 0x00214398
			public void SetAxisPosition(int elementId, float value)
			{
				if (this._transform == null)
				{
					return;
				}
				Vector2 position = this.position;
				if (elementId == this._xAxisElementId)
				{
					position.x = value;
				}
				else if (elementId == this._yAxisElementId)
				{
					position.y = value;
				}
				this.position = position;
			}

			// Token: 0x04004B45 RID: 19269
			private RectTransform _transform;

			// Token: 0x04004B46 RID: 19270
			private Vector2 _origPosition;

			// Token: 0x04004B47 RID: 19271
			private int _xAxisElementId = -1;

			// Token: 0x04004B48 RID: 19272
			private int _yAxisElementId = -1;
		}

		// Token: 0x020008D6 RID: 2262
		private class UIElement
		{
			// Token: 0x0600307B RID: 12411 RVA: 0x002161E6 File Offset: 0x002143E6
			public UIElement(int id, ControllerUIElement element)
			{
				this.id = id;
				this.element = element;
			}

			// Token: 0x04004B49 RID: 19273
			public int id;

			// Token: 0x04004B4A RID: 19274
			public ControllerUIElement element;
		}
	}
}
