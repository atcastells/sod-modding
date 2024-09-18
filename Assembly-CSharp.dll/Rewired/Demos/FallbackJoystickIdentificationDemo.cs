using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008B6 RID: 2230
	[AddComponentMenu("")]
	public class FallbackJoystickIdentificationDemo : MonoBehaviour
	{
		// Token: 0x06002FBB RID: 12219 RVA: 0x00211A5C File Offset: 0x0020FC5C
		private void Awake()
		{
			if (!ReInput.unityJoystickIdentificationRequired)
			{
				return;
			}
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickConnected);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickDisconnected);
			this.IdentifyAllJoysticks();
		}

		// Token: 0x06002FBC RID: 12220 RVA: 0x00211A8E File Offset: 0x0020FC8E
		private void JoystickConnected(ControllerStatusChangedEventArgs args)
		{
			this.IdentifyAllJoysticks();
		}

		// Token: 0x06002FBD RID: 12221 RVA: 0x00211A8E File Offset: 0x0020FC8E
		private void JoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			this.IdentifyAllJoysticks();
		}

		// Token: 0x06002FBE RID: 12222 RVA: 0x00211A98 File Offset: 0x0020FC98
		public void IdentifyAllJoysticks()
		{
			this.Reset();
			if (ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			Joystick[] joysticks = ReInput.controllers.GetJoysticks();
			if (joysticks == null)
			{
				return;
			}
			this.identifyRequired = true;
			this.joysticksToIdentify = new Queue<Joystick>(joysticks);
			this.SetInputDelay();
		}

		// Token: 0x06002FBF RID: 12223 RVA: 0x00211AE0 File Offset: 0x0020FCE0
		private void SetInputDelay()
		{
			this.nextInputAllowedTime = Time.time + 1f;
		}

		// Token: 0x06002FC0 RID: 12224 RVA: 0x00211AF4 File Offset: 0x0020FCF4
		private void OnGUI()
		{
			if (!this.identifyRequired)
			{
				return;
			}
			if (this.joysticksToIdentify == null || this.joysticksToIdentify.Count == 0)
			{
				this.Reset();
				return;
			}
			Rect rect;
			rect..ctor((float)Screen.width * 0.5f - 125f, (float)Screen.height * 0.5f - 125f, 250f, 250f);
			GUILayout.Window(0, rect, new GUI.WindowFunction(this.DrawDialogWindow), "Joystick Identification Required", Array.Empty<GUILayoutOption>());
			GUI.FocusWindow(0);
			if (Time.time < this.nextInputAllowedTime)
			{
				return;
			}
			if (!ReInput.controllers.SetUnityJoystickIdFromAnyButtonOrAxisPress(this.joysticksToIdentify.Peek().id, 0.8f, false))
			{
				return;
			}
			this.joysticksToIdentify.Dequeue();
			this.SetInputDelay();
			if (this.joysticksToIdentify.Count == 0)
			{
				this.Reset();
			}
		}

		// Token: 0x06002FC1 RID: 12225 RVA: 0x00211BD8 File Offset: 0x0020FDD8
		private void DrawDialogWindow(int windowId)
		{
			if (!this.identifyRequired)
			{
				return;
			}
			if (this.style == null)
			{
				this.style = new GUIStyle(GUI.skin.label);
				this.style.wordWrap = true;
			}
			GUILayout.Space(15f);
			GUILayout.Label("A joystick has been attached or removed. You will need to identify each joystick by pressing a button on the controller listed below:", this.style, Array.Empty<GUILayoutOption>());
			Joystick joystick = this.joysticksToIdentify.Peek();
			GUILayout.Label("Press any button on \"" + joystick.name + "\" now.", this.style, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Skip", Array.Empty<GUILayoutOption>()))
			{
				this.joysticksToIdentify.Dequeue();
				return;
			}
		}

		// Token: 0x06002FC2 RID: 12226 RVA: 0x00211C8A File Offset: 0x0020FE8A
		private void Reset()
		{
			this.joysticksToIdentify = null;
			this.identifyRequired = false;
		}

		// Token: 0x04004A7F RID: 19071
		private const float windowWidth = 250f;

		// Token: 0x04004A80 RID: 19072
		private const float windowHeight = 250f;

		// Token: 0x04004A81 RID: 19073
		private const float inputDelay = 1f;

		// Token: 0x04004A82 RID: 19074
		private bool identifyRequired;

		// Token: 0x04004A83 RID: 19075
		private Queue<Joystick> joysticksToIdentify;

		// Token: 0x04004A84 RID: 19076
		private float nextInputAllowedTime;

		// Token: 0x04004A85 RID: 19077
		private GUIStyle style;
	}
}
