using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000876 RID: 2166
	[AddComponentMenu("")]
	public class InputBehaviorWindow : Window
	{
		// Token: 0x06002D6B RID: 11627 RVA: 0x0020A9A0 File Offset: 0x00208BA0
		public override void Initialize(int id, Func<int, bool> isFocusedCallback)
		{
			if (this.spawnTransform == null || this.doneButton == null || this.cancelButton == null || this.defaultButton == null || this.uiControlSetPrefab == null || this.uiSliderControlPrefab == null || this.doneButtonLabel == null || this.cancelButtonLabel == null || this.defaultButtonLabel == null)
			{
				Debug.LogError("Rewired Control Mapper: All inspector values must be assigned!");
				return;
			}
			this.inputBehaviorInfo = new List<InputBehaviorWindow.InputBehaviorInfo>();
			this.buttonCallbacks = new Dictionary<int, Action<int>>();
			this.doneButtonLabel.text = ControlMapper.GetLanguage().done;
			this.cancelButtonLabel.text = ControlMapper.GetLanguage().cancel;
			this.defaultButtonLabel.text = ControlMapper.GetLanguage().default_;
			base.Initialize(id, isFocusedCallback);
		}

		// Token: 0x06002D6C RID: 11628 RVA: 0x0020AA94 File Offset: 0x00208C94
		public void SetData(int playerId, ControlMapper.InputBehaviorSettings[] data)
		{
			if (!base.initialized)
			{
				return;
			}
			this.playerId = playerId;
			foreach (ControlMapper.InputBehaviorSettings inputBehaviorSettings in data)
			{
				if (inputBehaviorSettings != null && inputBehaviorSettings.isValid)
				{
					InputBehavior inputBehavior = this.GetInputBehavior(inputBehaviorSettings.inputBehaviorId);
					if (inputBehavior != null)
					{
						UIControlSet uicontrolSet = this.CreateControlSet();
						Dictionary<int, InputBehaviorWindow.PropertyType> dictionary = new Dictionary<int, InputBehaviorWindow.PropertyType>();
						string customEntry = ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.labelLanguageKey);
						if (!string.IsNullOrEmpty(customEntry))
						{
							uicontrolSet.SetTitle(customEntry);
						}
						else
						{
							uicontrolSet.SetTitle(inputBehavior.name);
						}
						if (inputBehaviorSettings.showJoystickAxisSensitivity)
						{
							UISliderControl uisliderControl = this.CreateSlider(uicontrolSet, inputBehavior.id, null, ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.joystickAxisSensitivityLabelLanguageKey), inputBehaviorSettings.joystickAxisSensitivityIcon, inputBehaviorSettings.joystickAxisSensitivityMin, inputBehaviorSettings.joystickAxisSensitivityMax, new Action<int, int, float>(this.JoystickAxisSensitivityValueChanged), new Action<int, int>(this.JoystickAxisSensitivityCanceled));
							uisliderControl.slider.value = Mathf.Clamp(inputBehavior.joystickAxisSensitivity, inputBehaviorSettings.joystickAxisSensitivityMin, inputBehaviorSettings.joystickAxisSensitivityMax);
							dictionary.Add(uisliderControl.id, InputBehaviorWindow.PropertyType.JoystickAxisSensitivity);
						}
						if (inputBehaviorSettings.showMouseXYAxisSensitivity)
						{
							UISliderControl uisliderControl2 = this.CreateSlider(uicontrolSet, inputBehavior.id, null, ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.mouseXYAxisSensitivityLabelLanguageKey), inputBehaviorSettings.mouseXYAxisSensitivityIcon, inputBehaviorSettings.mouseXYAxisSensitivityMin, inputBehaviorSettings.mouseXYAxisSensitivityMax, new Action<int, int, float>(this.MouseXYAxisSensitivityValueChanged), new Action<int, int>(this.MouseXYAxisSensitivityCanceled));
							uisliderControl2.slider.value = Mathf.Clamp(inputBehavior.mouseXYAxisSensitivity, inputBehaviorSettings.mouseXYAxisSensitivityMin, inputBehaviorSettings.mouseXYAxisSensitivityMax);
							dictionary.Add(uisliderControl2.id, InputBehaviorWindow.PropertyType.MouseXYAxisSensitivity);
						}
						this.inputBehaviorInfo.Add(new InputBehaviorWindow.InputBehaviorInfo(inputBehavior, uicontrolSet, dictionary));
					}
				}
			}
			base.defaultUIElement = this.doneButton.gameObject;
		}

		// Token: 0x06002D6D RID: 11629 RVA: 0x0020AC59 File Offset: 0x00208E59
		public void SetButtonCallback(InputBehaviorWindow.ButtonIdentifier buttonIdentifier, Action<int> callback)
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

		// Token: 0x06002D6E RID: 11630 RVA: 0x0020AC94 File Offset: 0x00208E94
		public override void Cancel()
		{
			if (!base.initialized)
			{
				return;
			}
			foreach (InputBehaviorWindow.InputBehaviorInfo inputBehaviorInfo in this.inputBehaviorInfo)
			{
				inputBehaviorInfo.RestorePreviousData();
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

		// Token: 0x06002D6F RID: 11631 RVA: 0x0020AD20 File Offset: 0x00208F20
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

		// Token: 0x06002D70 RID: 11632 RVA: 0x001FFF73 File Offset: 0x001FE173
		public void OnCancel()
		{
			this.Cancel();
		}

		// Token: 0x06002D71 RID: 11633 RVA: 0x0020AD54 File Offset: 0x00208F54
		public void OnRestoreDefault()
		{
			if (!base.initialized)
			{
				return;
			}
			foreach (InputBehaviorWindow.InputBehaviorInfo inputBehaviorInfo in this.inputBehaviorInfo)
			{
				inputBehaviorInfo.RestoreDefaultData();
			}
		}

		// Token: 0x06002D72 RID: 11634 RVA: 0x0020ADB0 File Offset: 0x00208FB0
		private void JoystickAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value)
		{
			this.GetInputBehavior(inputBehaviorId).joystickAxisSensitivity = value;
		}

		// Token: 0x06002D73 RID: 11635 RVA: 0x0020ADBF File Offset: 0x00208FBF
		private void MouseXYAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value)
		{
			this.GetInputBehavior(inputBehaviorId).mouseXYAxisSensitivity = value;
		}

		// Token: 0x06002D74 RID: 11636 RVA: 0x0020ADD0 File Offset: 0x00208FD0
		private void JoystickAxisSensitivityCanceled(int inputBehaviorId, int controlId)
		{
			InputBehaviorWindow.InputBehaviorInfo inputBehaviorInfo = this.GetInputBehaviorInfo(inputBehaviorId);
			if (inputBehaviorInfo == null)
			{
				return;
			}
			inputBehaviorInfo.RestoreData(InputBehaviorWindow.PropertyType.JoystickAxisSensitivity, controlId);
		}

		// Token: 0x06002D75 RID: 11637 RVA: 0x0020ADF4 File Offset: 0x00208FF4
		private void MouseXYAxisSensitivityCanceled(int inputBehaviorId, int controlId)
		{
			InputBehaviorWindow.InputBehaviorInfo inputBehaviorInfo = this.GetInputBehaviorInfo(inputBehaviorId);
			if (inputBehaviorInfo == null)
			{
				return;
			}
			inputBehaviorInfo.RestoreData(InputBehaviorWindow.PropertyType.MouseXYAxisSensitivity, controlId);
		}

		// Token: 0x06002D76 RID: 11638 RVA: 0x0020AE15 File Offset: 0x00209015
		public override void TakeInputFocus()
		{
			base.TakeInputFocus();
		}

		// Token: 0x06002D77 RID: 11639 RVA: 0x0020AE1D File Offset: 0x0020901D
		private UIControlSet CreateControlSet()
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.uiControlSetPrefab);
			gameObject.transform.SetParent(this.spawnTransform, false);
			return gameObject.GetComponent<UIControlSet>();
		}

		// Token: 0x06002D78 RID: 11640 RVA: 0x0020AE44 File Offset: 0x00209044
		private UISliderControl CreateSlider(UIControlSet set, int inputBehaviorId, string defaultTitle, string overrideTitle, Sprite icon, float minValue, float maxValue, Action<int, int, float> valueChangedCallback, Action<int, int> cancelCallback)
		{
			UISliderControl uisliderControl = set.CreateSlider(this.uiSliderControlPrefab, icon, minValue, maxValue, delegate(int cId, float value)
			{
				valueChangedCallback.Invoke(inputBehaviorId, cId, value);
			}, delegate(int cId)
			{
				cancelCallback.Invoke(inputBehaviorId, cId);
			});
			string text = string.IsNullOrEmpty(overrideTitle) ? defaultTitle : overrideTitle;
			if (!string.IsNullOrEmpty(text))
			{
				uisliderControl.showTitle = true;
				uisliderControl.title.text = text;
			}
			else
			{
				uisliderControl.showTitle = false;
			}
			uisliderControl.showIcon = (icon != null);
			return uisliderControl;
		}

		// Token: 0x06002D79 RID: 11641 RVA: 0x0020AEDB File Offset: 0x002090DB
		private InputBehavior GetInputBehavior(int id)
		{
			return ReInput.mapping.GetInputBehavior(this.playerId, id);
		}

		// Token: 0x06002D7A RID: 11642 RVA: 0x0020AEF0 File Offset: 0x002090F0
		private InputBehaviorWindow.InputBehaviorInfo GetInputBehaviorInfo(int inputBehaviorId)
		{
			int count = this.inputBehaviorInfo.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.inputBehaviorInfo[i].inputBehavior.id == inputBehaviorId)
				{
					return this.inputBehaviorInfo[i];
				}
			}
			return null;
		}

		// Token: 0x0400490F RID: 18703
		private const float minSensitivity = 0.1f;

		// Token: 0x04004910 RID: 18704
		[SerializeField]
		private RectTransform spawnTransform;

		// Token: 0x04004911 RID: 18705
		[SerializeField]
		private Button doneButton;

		// Token: 0x04004912 RID: 18706
		[SerializeField]
		private Button cancelButton;

		// Token: 0x04004913 RID: 18707
		[SerializeField]
		private Button defaultButton;

		// Token: 0x04004914 RID: 18708
		[SerializeField]
		private TMP_Text doneButtonLabel;

		// Token: 0x04004915 RID: 18709
		[SerializeField]
		private TMP_Text cancelButtonLabel;

		// Token: 0x04004916 RID: 18710
		[SerializeField]
		private TMP_Text defaultButtonLabel;

		// Token: 0x04004917 RID: 18711
		[SerializeField]
		private GameObject uiControlSetPrefab;

		// Token: 0x04004918 RID: 18712
		[SerializeField]
		private GameObject uiSliderControlPrefab;

		// Token: 0x04004919 RID: 18713
		private List<InputBehaviorWindow.InputBehaviorInfo> inputBehaviorInfo;

		// Token: 0x0400491A RID: 18714
		private Dictionary<int, Action<int>> buttonCallbacks;

		// Token: 0x0400491B RID: 18715
		private int playerId;

		// Token: 0x02000877 RID: 2167
		private class InputBehaviorInfo
		{
			// Token: 0x17000435 RID: 1077
			// (get) Token: 0x06002D7C RID: 11644 RVA: 0x0020AF44 File Offset: 0x00209144
			public InputBehavior inputBehavior
			{
				get
				{
					return this._inputBehavior;
				}
			}

			// Token: 0x17000436 RID: 1078
			// (get) Token: 0x06002D7D RID: 11645 RVA: 0x0020AF4C File Offset: 0x0020914C
			public UIControlSet controlSet
			{
				get
				{
					return this._controlSet;
				}
			}

			// Token: 0x06002D7E RID: 11646 RVA: 0x0020AF54 File Offset: 0x00209154
			public InputBehaviorInfo(InputBehavior inputBehavior, UIControlSet controlSet, Dictionary<int, InputBehaviorWindow.PropertyType> idToProperty)
			{
				this._inputBehavior = inputBehavior;
				this._controlSet = controlSet;
				this.idToProperty = idToProperty;
				this.copyOfOriginal = new InputBehavior(inputBehavior);
			}

			// Token: 0x06002D7F RID: 11647 RVA: 0x0020AF7D File Offset: 0x0020917D
			public void RestorePreviousData()
			{
				this._inputBehavior.ImportData(this.copyOfOriginal);
			}

			// Token: 0x06002D80 RID: 11648 RVA: 0x0020AF91 File Offset: 0x00209191
			public void RestoreDefaultData()
			{
				this._inputBehavior.Reset();
				this.RefreshControls();
			}

			// Token: 0x06002D81 RID: 11649 RVA: 0x0020AFA4 File Offset: 0x002091A4
			public void RestoreData(InputBehaviorWindow.PropertyType propertyType, int controlId)
			{
				if (propertyType != InputBehaviorWindow.PropertyType.JoystickAxisSensitivity)
				{
					if (propertyType != InputBehaviorWindow.PropertyType.MouseXYAxisSensitivity)
					{
						return;
					}
					float mouseXYAxisSensitivity = this.copyOfOriginal.mouseXYAxisSensitivity;
					this._inputBehavior.mouseXYAxisSensitivity = mouseXYAxisSensitivity;
					UISliderControl control = this._controlSet.GetControl<UISliderControl>(controlId);
					if (control != null)
					{
						control.slider.value = mouseXYAxisSensitivity;
					}
				}
				else
				{
					float joystickAxisSensitivity = this.copyOfOriginal.joystickAxisSensitivity;
					this._inputBehavior.joystickAxisSensitivity = joystickAxisSensitivity;
					UISliderControl control2 = this._controlSet.GetControl<UISliderControl>(controlId);
					if (control2 != null)
					{
						control2.slider.value = joystickAxisSensitivity;
						return;
					}
				}
			}

			// Token: 0x06002D82 RID: 11650 RVA: 0x0020B030 File Offset: 0x00209230
			public void RefreshControls()
			{
				if (this._controlSet == null)
				{
					return;
				}
				if (this.idToProperty == null)
				{
					return;
				}
				foreach (KeyValuePair<int, InputBehaviorWindow.PropertyType> keyValuePair in this.idToProperty)
				{
					UISliderControl control = this._controlSet.GetControl<UISliderControl>(keyValuePair.Key);
					if (!(control == null))
					{
						InputBehaviorWindow.PropertyType value = keyValuePair.Value;
						if (value != InputBehaviorWindow.PropertyType.JoystickAxisSensitivity)
						{
							if (value == InputBehaviorWindow.PropertyType.MouseXYAxisSensitivity)
							{
								control.slider.value = this._inputBehavior.mouseXYAxisSensitivity;
							}
						}
						else
						{
							control.slider.value = this._inputBehavior.joystickAxisSensitivity;
						}
					}
				}
			}

			// Token: 0x0400491C RID: 18716
			private InputBehavior _inputBehavior;

			// Token: 0x0400491D RID: 18717
			private UIControlSet _controlSet;

			// Token: 0x0400491E RID: 18718
			private Dictionary<int, InputBehaviorWindow.PropertyType> idToProperty;

			// Token: 0x0400491F RID: 18719
			private InputBehavior copyOfOriginal;
		}

		// Token: 0x02000878 RID: 2168
		public enum ButtonIdentifier
		{
			// Token: 0x04004921 RID: 18721
			Done,
			// Token: 0x04004922 RID: 18722
			Cancel,
			// Token: 0x04004923 RID: 18723
			Default
		}

		// Token: 0x02000879 RID: 2169
		private enum PropertyType
		{
			// Token: 0x04004925 RID: 18725
			JoystickAxisSensitivity,
			// Token: 0x04004926 RID: 18726
			MouseXYAxisSensitivity
		}
	}
}
