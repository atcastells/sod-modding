using System;
using System.Collections.Generic;
using Rewired.Integration.UnityUI;
using Rewired.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000847 RID: 2119
	[AddComponentMenu("")]
	public class CalibrationWindow : Window
	{
		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06002A56 RID: 10838 RVA: 0x001FF9F0 File Offset: 0x001FDBF0
		private bool axisSelected
		{
			get
			{
				return this.joystick != null && this.selectedAxis >= 0 && this.selectedAxis < this.joystick.calibrationMap.axisCount;
			}
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06002A57 RID: 10839 RVA: 0x001FFA20 File Offset: 0x001FDC20
		private AxisCalibration axisCalibration
		{
			get
			{
				if (!this.axisSelected)
				{
					return null;
				}
				return this.joystick.calibrationMap.GetAxis(this.selectedAxis);
			}
		}

		// Token: 0x06002A58 RID: 10840 RVA: 0x001FFA44 File Offset: 0x001FDC44
		public override void Initialize(int id, Func<int, bool> isFocusedCallback)
		{
			if (this.rightContentContainer == null || this.valueDisplayGroup == null || this.calibratedValueMarker == null || this.rawValueMarker == null || this.calibratedZeroMarker == null || this.deadzoneArea == null || this.deadzoneSlider == null || this.sensitivitySlider == null || this.zeroSlider == null || this.invertToggle == null || this.axisScrollAreaContent == null || this.doneButton == null || this.calibrateButton == null || this.axisButtonPrefab == null || this.doneButtonLabel == null || this.cancelButtonLabel == null || this.defaultButtonLabel == null || this.deadzoneSliderLabel == null || this.zeroSliderLabel == null || this.sensitivitySliderLabel == null || this.invertToggleLabel == null || this.calibrateButtonLabel == null)
			{
				Debug.LogError("Rewired Control Mapper: All inspector values must be assigned!");
				return;
			}
			this.axisButtons = new List<Button>();
			this.buttonCallbacks = new Dictionary<int, Action<int>>();
			this.doneButtonLabel.text = ControlMapper.GetLanguage().done;
			this.cancelButtonLabel.text = ControlMapper.GetLanguage().cancel;
			this.defaultButtonLabel.text = ControlMapper.GetLanguage().default_;
			this.deadzoneSliderLabel.text = ControlMapper.GetLanguage().calibrateWindow_deadZoneSliderLabel;
			this.zeroSliderLabel.text = ControlMapper.GetLanguage().calibrateWindow_zeroSliderLabel;
			this.sensitivitySliderLabel.text = ControlMapper.GetLanguage().calibrateWindow_sensitivitySliderLabel;
			this.invertToggleLabel.text = ControlMapper.GetLanguage().calibrateWindow_invertToggleLabel;
			this.calibrateButtonLabel.text = ControlMapper.GetLanguage().calibrateWindow_calibrateButtonLabel;
			base.Initialize(id, isFocusedCallback);
		}

		// Token: 0x06002A59 RID: 10841 RVA: 0x001FFC7C File Offset: 0x001FDE7C
		public void SetJoystick(int playerId, Joystick joystick)
		{
			if (!base.initialized)
			{
				return;
			}
			this.playerId = playerId;
			this.joystick = joystick;
			if (joystick == null)
			{
				Debug.LogError("Rewired Control Mapper: Joystick cannot be null!");
				return;
			}
			float num = 0f;
			for (int i = 0; i < joystick.axisCount; i++)
			{
				int index = i;
				GameObject gameObject = UITools.InstantiateGUIObject<Button>(this.axisButtonPrefab, this.axisScrollAreaContent, "Axis" + i.ToString());
				Button button = gameObject.GetComponent<Button>();
				button.onClick.AddListener(delegate()
				{
					this.OnAxisSelected(index, button);
				});
				TMP_Text componentInSelfOrChildren = UnityTools.GetComponentInSelfOrChildren<TMP_Text>(gameObject);
				if (componentInSelfOrChildren != null)
				{
					componentInSelfOrChildren.text = ControlMapper.GetLanguage().GetElementIdentifierName(joystick, joystick.AxisElementIdentifiers[i].id, 0);
				}
				if (num == 0f)
				{
					num = UnityTools.GetComponentInSelfOrChildren<LayoutElement>(gameObject).minHeight;
				}
				this.axisButtons.Add(button);
			}
			float spacing = this.axisScrollAreaContent.GetComponent<VerticalLayoutGroup>().spacing;
			this.axisScrollAreaContent.sizeDelta = new Vector2(this.axisScrollAreaContent.sizeDelta.x, Mathf.Max((float)joystick.axisCount * (num + spacing) - spacing, this.axisScrollAreaContent.sizeDelta.y));
			this.origCalibrationData = joystick.calibrationMap.ToXmlString();
			this.displayAreaWidth = this.rightContentContainer.sizeDelta.x;
			this.rewiredStandaloneInputModule = base.gameObject.transform.root.GetComponentInChildren<RewiredStandaloneInputModule>();
			if (this.rewiredStandaloneInputModule != null)
			{
				this.menuHorizActionId = ReInput.mapping.GetActionId(this.rewiredStandaloneInputModule.horizontalAxis);
				this.menuVertActionId = ReInput.mapping.GetActionId(this.rewiredStandaloneInputModule.verticalAxis);
			}
			if (joystick.axisCount > 0)
			{
				this.SelectAxis(0);
			}
			base.defaultUIElement = this.doneButton.gameObject;
			this.RefreshControls();
			this.Redraw();
		}

		// Token: 0x06002A5A RID: 10842 RVA: 0x001FFE8F File Offset: 0x001FE08F
		public void SetButtonCallback(CalibrationWindow.ButtonIdentifier buttonIdentifier, Action<int> callback)
		{
			if (!base.initialized)
			{
				return;
			}
			if (callback == null)
			{
				return;
			}
			if (this.buttonCallbacks.ContainsKey((int)buttonIdentifier))
			{
				this.buttonCallbacks[(int)buttonIdentifier] = callback;
				return;
			}
			this.buttonCallbacks.Add((int)buttonIdentifier, callback);
		}

		// Token: 0x06002A5B RID: 10843 RVA: 0x001FFEC8 File Offset: 0x001FE0C8
		public override void Cancel()
		{
			if (!base.initialized)
			{
				return;
			}
			if (this.joystick != null)
			{
				this.joystick.ImportCalibrationMapFromXmlString(this.origCalibrationData);
			}
			Action<int> action;
			if (!this.buttonCallbacks.TryGetValue(1, ref action))
			{
				if (this.cancelCallback != null)
				{
					this.cancelCallback.Invoke();
				}
				return;
			}
			action.Invoke(base.id);
		}

		// Token: 0x06002A5C RID: 10844 RVA: 0x001FFF28 File Offset: 0x001FE128
		protected override void Update()
		{
			if (!base.initialized)
			{
				return;
			}
			base.Update();
			this.UpdateDisplay();
		}

		// Token: 0x06002A5D RID: 10845 RVA: 0x001FFF40 File Offset: 0x001FE140
		public void OnDone()
		{
			if (!base.initialized)
			{
				return;
			}
			Action<int> action;
			if (!this.buttonCallbacks.TryGetValue(0, ref action))
			{
				return;
			}
			action.Invoke(base.id);
		}

		// Token: 0x06002A5E RID: 10846 RVA: 0x001FFF73 File Offset: 0x001FE173
		public void OnCancel()
		{
			this.Cancel();
		}

		// Token: 0x06002A5F RID: 10847 RVA: 0x001FFF7B File Offset: 0x001FE17B
		public void OnRestoreDefault()
		{
			if (!base.initialized)
			{
				return;
			}
			if (this.joystick == null)
			{
				return;
			}
			this.joystick.calibrationMap.Reset();
			this.RefreshControls();
			this.Redraw();
		}

		// Token: 0x06002A60 RID: 10848 RVA: 0x001FFFAC File Offset: 0x001FE1AC
		public void OnCalibrate()
		{
			if (!base.initialized)
			{
				return;
			}
			Action<int> action;
			if (!this.buttonCallbacks.TryGetValue(3, ref action))
			{
				return;
			}
			action.Invoke(this.selectedAxis);
		}

		// Token: 0x06002A61 RID: 10849 RVA: 0x001FFFDF File Offset: 0x001FE1DF
		public void OnInvert(bool state)
		{
			if (!base.initialized)
			{
				return;
			}
			if (!this.axisSelected)
			{
				return;
			}
			this.axisCalibration.invert = state;
		}

		// Token: 0x06002A62 RID: 10850 RVA: 0x001FFFFF File Offset: 0x001FE1FF
		public void OnZeroValueChange(float value)
		{
			if (!base.initialized)
			{
				return;
			}
			if (!this.axisSelected)
			{
				return;
			}
			this.axisCalibration.calibratedZero = value;
			this.RedrawCalibratedZero();
		}

		// Token: 0x06002A63 RID: 10851 RVA: 0x00200025 File Offset: 0x001FE225
		public void OnZeroCancel()
		{
			if (!base.initialized)
			{
				return;
			}
			if (!this.axisSelected)
			{
				return;
			}
			this.axisCalibration.calibratedZero = this.origSelectedAxisCalibrationData.zero;
			this.RedrawCalibratedZero();
			this.RefreshControls();
		}

		// Token: 0x06002A64 RID: 10852 RVA: 0x0020005C File Offset: 0x001FE25C
		public void OnDeadzoneValueChange(float value)
		{
			if (!base.initialized)
			{
				return;
			}
			if (!this.axisSelected)
			{
				return;
			}
			this.axisCalibration.deadZone = Mathf.Clamp(value, 0f, 0.8f);
			if (value > 0.8f)
			{
				this.deadzoneSlider.value = 0.8f;
			}
			this.RedrawDeadzone();
		}

		// Token: 0x06002A65 RID: 10853 RVA: 0x002000B4 File Offset: 0x001FE2B4
		public void OnDeadzoneCancel()
		{
			if (!base.initialized)
			{
				return;
			}
			if (!this.axisSelected)
			{
				return;
			}
			this.axisCalibration.deadZone = this.origSelectedAxisCalibrationData.deadZone;
			this.RedrawDeadzone();
			this.RefreshControls();
		}

		// Token: 0x06002A66 RID: 10854 RVA: 0x002000EA File Offset: 0x001FE2EA
		public void OnSensitivityValueChange(float value)
		{
			if (!base.initialized)
			{
				return;
			}
			if (!this.axisSelected)
			{
				return;
			}
			this.SetSensitivity(this.axisCalibration, value);
		}

		// Token: 0x06002A67 RID: 10855 RVA: 0x0020010B File Offset: 0x001FE30B
		public void OnSensitivityCancel(float value)
		{
			if (!base.initialized)
			{
				return;
			}
			if (!this.axisSelected)
			{
				return;
			}
			this.axisCalibration.sensitivity = this.origSelectedAxisCalibrationData.sensitivity;
			this.RefreshControls();
		}

		// Token: 0x06002A68 RID: 10856 RVA: 0x0020013B File Offset: 0x001FE33B
		public void OnAxisScrollRectScroll(Vector2 pos)
		{
			bool initialized = base.initialized;
		}

		// Token: 0x06002A69 RID: 10857 RVA: 0x00200144 File Offset: 0x001FE344
		private void OnAxisSelected(int axisIndex, Button button)
		{
			if (!base.initialized)
			{
				return;
			}
			if (this.joystick == null)
			{
				return;
			}
			this.SelectAxis(axisIndex);
			this.RefreshControls();
			this.Redraw();
		}

		// Token: 0x06002A6A RID: 10858 RVA: 0x0020016B File Offset: 0x001FE36B
		private void UpdateDisplay()
		{
			this.RedrawValueMarkers();
		}

		// Token: 0x06002A6B RID: 10859 RVA: 0x00200173 File Offset: 0x001FE373
		private void Redraw()
		{
			this.RedrawCalibratedZero();
			this.RedrawValueMarkers();
		}

		// Token: 0x06002A6C RID: 10860 RVA: 0x00200184 File Offset: 0x001FE384
		private void RefreshControls()
		{
			if (!this.axisSelected)
			{
				this.deadzoneSlider.value = 0f;
				this.zeroSlider.value = 0f;
				this.sensitivitySlider.value = 0f;
				this.invertToggle.isOn = false;
				return;
			}
			this.deadzoneSlider.value = this.axisCalibration.deadZone;
			this.zeroSlider.value = this.axisCalibration.calibratedZero;
			this.sensitivitySlider.value = this.GetSliderSensitivity(this.axisCalibration);
			this.invertToggle.isOn = this.axisCalibration.invert;
		}

		// Token: 0x06002A6D RID: 10861 RVA: 0x00200230 File Offset: 0x001FE430
		private void RedrawDeadzone()
		{
			if (!this.axisSelected)
			{
				return;
			}
			float num = this.displayAreaWidth * this.axisCalibration.deadZone;
			this.deadzoneArea.sizeDelta = new Vector2(num, this.deadzoneArea.sizeDelta.y);
			this.deadzoneArea.anchoredPosition = new Vector2(this.axisCalibration.calibratedZero * -this.deadzoneArea.parent.localPosition.x, this.deadzoneArea.anchoredPosition.y);
		}

		// Token: 0x06002A6E RID: 10862 RVA: 0x002002BC File Offset: 0x001FE4BC
		private void RedrawCalibratedZero()
		{
			if (!this.axisSelected)
			{
				return;
			}
			this.calibratedZeroMarker.anchoredPosition = new Vector2(this.axisCalibration.calibratedZero * -this.deadzoneArea.parent.localPosition.x, this.calibratedZeroMarker.anchoredPosition.y);
			this.RedrawDeadzone();
		}

		// Token: 0x06002A6F RID: 10863 RVA: 0x0020031C File Offset: 0x001FE51C
		private void RedrawValueMarkers()
		{
			if (!this.axisSelected)
			{
				this.calibratedValueMarker.anchoredPosition = new Vector2(0f, this.calibratedValueMarker.anchoredPosition.y);
				this.rawValueMarker.anchoredPosition = new Vector2(0f, this.rawValueMarker.anchoredPosition.y);
				return;
			}
			float axis = this.joystick.GetAxis(this.selectedAxis);
			float num = Mathf.Clamp(this.joystick.GetAxisRaw(this.selectedAxis), -1f, 1f);
			this.calibratedValueMarker.anchoredPosition = new Vector2(this.displayAreaWidth * 0.5f * axis, this.calibratedValueMarker.anchoredPosition.y);
			this.rawValueMarker.anchoredPosition = new Vector2(this.displayAreaWidth * 0.5f * num, this.rawValueMarker.anchoredPosition.y);
		}

		// Token: 0x06002A70 RID: 10864 RVA: 0x0020040C File Offset: 0x001FE60C
		private void SelectAxis(int index)
		{
			if (index < 0 || index >= this.axisButtons.Count)
			{
				return;
			}
			if (this.axisButtons[index] == null)
			{
				return;
			}
			this.axisButtons[index].interactable = false;
			this.axisButtons[index].Select();
			for (int i = 0; i < this.axisButtons.Count; i++)
			{
				if (i != index)
				{
					this.axisButtons[i].interactable = true;
				}
			}
			this.selectedAxis = index;
			this.origSelectedAxisCalibrationData = this.axisCalibration.GetData();
			this.SetMinSensitivity();
		}

		// Token: 0x06002A71 RID: 10865 RVA: 0x002004AE File Offset: 0x001FE6AE
		public override void TakeInputFocus()
		{
			base.TakeInputFocus();
			if (this.selectedAxis >= 0)
			{
				this.SelectAxis(this.selectedAxis);
			}
			this.RefreshControls();
			this.Redraw();
		}

		// Token: 0x06002A72 RID: 10866 RVA: 0x002004D8 File Offset: 0x001FE6D8
		private void SetMinSensitivity()
		{
			if (!this.axisSelected)
			{
				return;
			}
			this.minSensitivity = 0.1f;
			if (this.rewiredStandaloneInputModule != null)
			{
				if (this.IsMenuAxis(this.menuHorizActionId, this.selectedAxis))
				{
					this.GetAxisButtonDeadZone(this.playerId, this.menuHorizActionId, ref this.minSensitivity);
					return;
				}
				if (this.IsMenuAxis(this.menuVertActionId, this.selectedAxis))
				{
					this.GetAxisButtonDeadZone(this.playerId, this.menuVertActionId, ref this.minSensitivity);
				}
			}
		}

		// Token: 0x06002A73 RID: 10867 RVA: 0x00200560 File Offset: 0x001FE760
		private bool IsMenuAxis(int actionId, int axisIndex)
		{
			if (this.rewiredStandaloneInputModule == null)
			{
				return false;
			}
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			int count = allPlayers.Count;
			for (int i = 0; i < count; i++)
			{
				IList<JoystickMap> maps = allPlayers[i].controllers.maps.GetMaps<JoystickMap>(this.joystick.id);
				if (maps != null)
				{
					int count2 = maps.Count;
					for (int j = 0; j < count2; j++)
					{
						IList<ActionElementMap> axisMaps = maps[j].AxisMaps;
						if (axisMaps != null)
						{
							int count3 = axisMaps.Count;
							for (int k = 0; k < count3; k++)
							{
								ActionElementMap actionElementMap = axisMaps[k];
								if (actionElementMap.actionId == actionId && actionElementMap.elementIndex == axisIndex)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06002A74 RID: 10868 RVA: 0x00200630 File Offset: 0x001FE830
		private void GetAxisButtonDeadZone(int playerId, int actionId, ref float value)
		{
			InputAction action = ReInput.mapping.GetAction(actionId);
			if (action == null)
			{
				return;
			}
			int behaviorId = action.behaviorId;
			InputBehavior inputBehavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
			if (inputBehavior == null)
			{
				return;
			}
			value = inputBehavior.buttonDeadZone + 0.1f;
		}

		// Token: 0x06002A75 RID: 10869 RVA: 0x00200673 File Offset: 0x001FE873
		private float GetSliderSensitivity(AxisCalibration axisCalibration)
		{
			if (axisCalibration.sensitivityType == null)
			{
				return axisCalibration.sensitivity;
			}
			if (axisCalibration.sensitivityType == 1)
			{
				return CalibrationWindow.ProcessPowerValue(axisCalibration.sensitivity, 0f, this.sensitivitySlider.maxValue);
			}
			return axisCalibration.sensitivity;
		}

		// Token: 0x06002A76 RID: 10870 RVA: 0x002006B0 File Offset: 0x001FE8B0
		public void SetSensitivity(AxisCalibration axisCalibration, float sliderValue)
		{
			if (axisCalibration.sensitivityType == null)
			{
				axisCalibration.sensitivity = Mathf.Clamp(sliderValue, this.minSensitivity, float.PositiveInfinity);
				if (sliderValue < this.minSensitivity)
				{
					this.sensitivitySlider.value = this.minSensitivity;
					return;
				}
			}
			else
			{
				if (axisCalibration.sensitivityType == 1)
				{
					axisCalibration.sensitivity = CalibrationWindow.ProcessPowerValue(sliderValue, 0f, this.sensitivitySlider.maxValue);
					return;
				}
				axisCalibration.sensitivity = sliderValue;
			}
		}

		// Token: 0x06002A77 RID: 10871 RVA: 0x00200724 File Offset: 0x001FE924
		private static float ProcessPowerValue(float value, float minValue, float maxValue)
		{
			value = Mathf.Clamp(value, minValue, maxValue);
			if (value > 1f)
			{
				value = MathTools.ValueInNewRange(value, 1f, maxValue, 1f, 0f);
			}
			else if (value < 1f)
			{
				value = MathTools.ValueInNewRange(value, 0f, 1f, maxValue, 1f);
			}
			return value;
		}

		// Token: 0x0400479D RID: 18333
		private const float minSensitivityOtherAxes = 0.1f;

		// Token: 0x0400479E RID: 18334
		private const float maxDeadzone = 0.8f;

		// Token: 0x0400479F RID: 18335
		[SerializeField]
		private RectTransform rightContentContainer;

		// Token: 0x040047A0 RID: 18336
		[SerializeField]
		private RectTransform valueDisplayGroup;

		// Token: 0x040047A1 RID: 18337
		[SerializeField]
		private RectTransform calibratedValueMarker;

		// Token: 0x040047A2 RID: 18338
		[SerializeField]
		private RectTransform rawValueMarker;

		// Token: 0x040047A3 RID: 18339
		[SerializeField]
		private RectTransform calibratedZeroMarker;

		// Token: 0x040047A4 RID: 18340
		[SerializeField]
		private RectTransform deadzoneArea;

		// Token: 0x040047A5 RID: 18341
		[SerializeField]
		private Slider deadzoneSlider;

		// Token: 0x040047A6 RID: 18342
		[SerializeField]
		private Slider zeroSlider;

		// Token: 0x040047A7 RID: 18343
		[SerializeField]
		private Slider sensitivitySlider;

		// Token: 0x040047A8 RID: 18344
		[SerializeField]
		private Toggle invertToggle;

		// Token: 0x040047A9 RID: 18345
		[SerializeField]
		private RectTransform axisScrollAreaContent;

		// Token: 0x040047AA RID: 18346
		[SerializeField]
		private Button doneButton;

		// Token: 0x040047AB RID: 18347
		[SerializeField]
		private Button calibrateButton;

		// Token: 0x040047AC RID: 18348
		[SerializeField]
		private TMP_Text doneButtonLabel;

		// Token: 0x040047AD RID: 18349
		[SerializeField]
		private TMP_Text cancelButtonLabel;

		// Token: 0x040047AE RID: 18350
		[SerializeField]
		private TMP_Text defaultButtonLabel;

		// Token: 0x040047AF RID: 18351
		[SerializeField]
		private TMP_Text deadzoneSliderLabel;

		// Token: 0x040047B0 RID: 18352
		[SerializeField]
		private TMP_Text zeroSliderLabel;

		// Token: 0x040047B1 RID: 18353
		[SerializeField]
		private TMP_Text sensitivitySliderLabel;

		// Token: 0x040047B2 RID: 18354
		[SerializeField]
		private TMP_Text invertToggleLabel;

		// Token: 0x040047B3 RID: 18355
		[SerializeField]
		private TMP_Text calibrateButtonLabel;

		// Token: 0x040047B4 RID: 18356
		[SerializeField]
		private GameObject axisButtonPrefab;

		// Token: 0x040047B5 RID: 18357
		private Joystick joystick;

		// Token: 0x040047B6 RID: 18358
		private string origCalibrationData;

		// Token: 0x040047B7 RID: 18359
		private int selectedAxis = -1;

		// Token: 0x040047B8 RID: 18360
		private AxisCalibrationData origSelectedAxisCalibrationData;

		// Token: 0x040047B9 RID: 18361
		private float displayAreaWidth;

		// Token: 0x040047BA RID: 18362
		private List<Button> axisButtons;

		// Token: 0x040047BB RID: 18363
		private Dictionary<int, Action<int>> buttonCallbacks;

		// Token: 0x040047BC RID: 18364
		private int playerId;

		// Token: 0x040047BD RID: 18365
		private RewiredStandaloneInputModule rewiredStandaloneInputModule;

		// Token: 0x040047BE RID: 18366
		private int menuHorizActionId = -1;

		// Token: 0x040047BF RID: 18367
		private int menuVertActionId = -1;

		// Token: 0x040047C0 RID: 18368
		private float minSensitivity;

		// Token: 0x02000848 RID: 2120
		public enum ButtonIdentifier
		{
			// Token: 0x040047C2 RID: 18370
			Done,
			// Token: 0x040047C3 RID: 18371
			Cancel,
			// Token: 0x040047C4 RID: 18372
			Default,
			// Token: 0x040047C5 RID: 18373
			Calibrate
		}
	}
}
