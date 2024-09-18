using System;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008AE RID: 2222
	[AddComponentMenu("")]
	public class CustomControllerDemo : MonoBehaviour
	{
		// Token: 0x06002F83 RID: 12163 RVA: 0x00210AF0 File Offset: 0x0020ECF0
		private void Awake()
		{
			ScreenOrientation screenOrientation = 3;
			if (SystemInfo.deviceType == 1 && Screen.orientation != screenOrientation)
			{
				Screen.orientation = screenOrientation;
			}
			this.Initialize();
		}

		// Token: 0x06002F84 RID: 12164 RVA: 0x00210B1C File Offset: 0x0020ED1C
		private void Initialize()
		{
			ReInput.InputSourceUpdateEvent += new Action(this.OnInputSourceUpdate);
			this.joysticks = base.GetComponentsInChildren<TouchJoystickExample>();
			this.buttons = base.GetComponentsInChildren<TouchButtonExample>();
			this.axisCount = this.joysticks.Length * 2;
			this.buttonCount = this.buttons.Length;
			this.axisValues = new float[this.axisCount];
			this.buttonValues = new bool[this.buttonCount];
			Player player = ReInput.players.GetPlayer(this.playerId);
			this.controller = player.controllers.GetControllerWithTag<CustomController>(this.controllerTag);
			if (this.controller == null)
			{
				Debug.LogError("A matching controller was not found for tag \"" + this.controllerTag + "\"");
			}
			if (this.controller.buttonCount != this.buttonValues.Length || this.controller.axisCount != this.axisValues.Length)
			{
				Debug.LogError("Controller has wrong number of elements!");
			}
			if (this.useUpdateCallbacks && this.controller != null)
			{
				this.controller.SetAxisUpdateCallback(new Func<int, float>(this.GetAxisValueCallback));
				this.controller.SetButtonUpdateCallback(new Func<int, bool>(this.GetButtonValueCallback));
			}
			this.initialized = true;
		}

		// Token: 0x06002F85 RID: 12165 RVA: 0x00210C55 File Offset: 0x0020EE55
		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (!this.initialized)
			{
				this.Initialize();
			}
		}

		// Token: 0x06002F86 RID: 12166 RVA: 0x00210C6D File Offset: 0x0020EE6D
		private void OnInputSourceUpdate()
		{
			this.GetSourceAxisValues();
			this.GetSourceButtonValues();
			if (!this.useUpdateCallbacks)
			{
				this.SetControllerAxisValues();
				this.SetControllerButtonValues();
			}
		}

		// Token: 0x06002F87 RID: 12167 RVA: 0x00210C90 File Offset: 0x0020EE90
		private void GetSourceAxisValues()
		{
			for (int i = 0; i < this.axisValues.Length; i++)
			{
				if (i % 2 != 0)
				{
					this.axisValues[i] = this.joysticks[i / 2].position.y;
				}
				else
				{
					this.axisValues[i] = this.joysticks[i / 2].position.x;
				}
			}
		}

		// Token: 0x06002F88 RID: 12168 RVA: 0x00210CF0 File Offset: 0x0020EEF0
		private void GetSourceButtonValues()
		{
			for (int i = 0; i < this.buttonValues.Length; i++)
			{
				this.buttonValues[i] = this.buttons[i].isPressed;
			}
		}

		// Token: 0x06002F89 RID: 12169 RVA: 0x00210D28 File Offset: 0x0020EF28
		private void SetControllerAxisValues()
		{
			for (int i = 0; i < this.axisValues.Length; i++)
			{
				this.controller.SetAxisValue(i, this.axisValues[i]);
			}
		}

		// Token: 0x06002F8A RID: 12170 RVA: 0x00210D5C File Offset: 0x0020EF5C
		private void SetControllerButtonValues()
		{
			for (int i = 0; i < this.buttonValues.Length; i++)
			{
				this.controller.SetButtonValue(i, this.buttonValues[i]);
			}
		}

		// Token: 0x06002F8B RID: 12171 RVA: 0x00210D90 File Offset: 0x0020EF90
		private float GetAxisValueCallback(int index)
		{
			if (index >= this.axisValues.Length)
			{
				return 0f;
			}
			return this.axisValues[index];
		}

		// Token: 0x06002F8C RID: 12172 RVA: 0x00210DAB File Offset: 0x0020EFAB
		private bool GetButtonValueCallback(int index)
		{
			return index < this.buttonValues.Length && this.buttonValues[index];
		}

		// Token: 0x04004A4E RID: 19022
		public int playerId;

		// Token: 0x04004A4F RID: 19023
		public string controllerTag;

		// Token: 0x04004A50 RID: 19024
		public bool useUpdateCallbacks;

		// Token: 0x04004A51 RID: 19025
		private int buttonCount;

		// Token: 0x04004A52 RID: 19026
		private int axisCount;

		// Token: 0x04004A53 RID: 19027
		private float[] axisValues;

		// Token: 0x04004A54 RID: 19028
		private bool[] buttonValues;

		// Token: 0x04004A55 RID: 19029
		private TouchJoystickExample[] joysticks;

		// Token: 0x04004A56 RID: 19030
		private TouchButtonExample[] buttons;

		// Token: 0x04004A57 RID: 19031
		private CustomController controller;

		// Token: 0x04004A58 RID: 19032
		[NonSerialized]
		private bool initialized;
	}
}
