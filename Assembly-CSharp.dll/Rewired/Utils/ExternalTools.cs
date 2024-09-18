using System;
using System.Collections.Generic;
using System.ComponentModel;
using Rewired.Internal;
using Rewired.Internal.Windows;
using Rewired.Utils.Interfaces;
using Rewired.Utils.Platforms.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.Utils
{
	// Token: 0x02000831 RID: 2097
	[EditorBrowsable(1)]
	public class ExternalTools : IExternalTools
	{
		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060028E2 RID: 10466 RVA: 0x001FB19B File Offset: 0x001F939B
		// (set) Token: 0x060028E3 RID: 10467 RVA: 0x001FB1A2 File Offset: 0x001F93A2
		public static Func<object> getPlatformInitializerDelegate
		{
			get
			{
				return ExternalTools._getPlatformInitializerDelegate;
			}
			set
			{
				ExternalTools._getPlatformInitializerDelegate = value;
			}
		}

		// Token: 0x060028E5 RID: 10469 RVA: 0x00002265 File Offset: 0x00000465
		public void Destroy()
		{
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060028E6 RID: 10470 RVA: 0x001FB1AA File Offset: 0x001F93AA
		public bool isEditorPaused
		{
			get
			{
				return this._isEditorPaused;
			}
		}

		// Token: 0x14000061 RID: 97
		// (add) Token: 0x060028E7 RID: 10471 RVA: 0x001FB1B2 File Offset: 0x001F93B2
		// (remove) Token: 0x060028E8 RID: 10472 RVA: 0x001FB1CB File Offset: 0x001F93CB
		public event Action<bool> EditorPausedStateChangedEvent
		{
			add
			{
				this._EditorPausedStateChangedEvent = (Action<bool>)Delegate.Combine(this._EditorPausedStateChangedEvent, value);
			}
			remove
			{
				this._EditorPausedStateChangedEvent = (Action<bool>)Delegate.Remove(this._EditorPausedStateChangedEvent, value);
			}
		}

		// Token: 0x060028E9 RID: 10473 RVA: 0x001FB1E4 File Offset: 0x001F93E4
		public object GetPlatformInitializer()
		{
			return Main.GetPlatformInitializer();
		}

		// Token: 0x060028EA RID: 10474 RVA: 0x001D5ADE File Offset: 0x001D3CDE
		public string GetFocusedEditorWindowTitle()
		{
			return string.Empty;
		}

		// Token: 0x060028EB RID: 10475 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool IsEditorSceneViewFocused()
		{
			return false;
		}

		// Token: 0x060028EC RID: 10476 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool LinuxInput_IsJoystickPreconfigured(string name)
		{
			return false;
		}

		// Token: 0x14000062 RID: 98
		// (add) Token: 0x060028ED RID: 10477 RVA: 0x001FB1EC File Offset: 0x001F93EC
		// (remove) Token: 0x060028EE RID: 10478 RVA: 0x001FB224 File Offset: 0x001F9424
		public event Action<uint, bool> XboxOneInput_OnGamepadStateChange;

		// Token: 0x060028EF RID: 10479 RVA: 0x00085A40 File Offset: 0x00083C40
		public int XboxOneInput_GetUserIdForGamepad(uint id)
		{
			return 0;
		}

		// Token: 0x060028F0 RID: 10480 RVA: 0x001FB259 File Offset: 0x001F9459
		public ulong XboxOneInput_GetControllerId(uint unityJoystickId)
		{
			return 0UL;
		}

		// Token: 0x060028F1 RID: 10481 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool XboxOneInput_IsGamepadActive(uint unityJoystickId)
		{
			return false;
		}

		// Token: 0x060028F2 RID: 10482 RVA: 0x001D5ADE File Offset: 0x001D3CDE
		public string XboxOneInput_GetControllerType(ulong xboxControllerId)
		{
			return string.Empty;
		}

		// Token: 0x060028F3 RID: 10483 RVA: 0x00085A40 File Offset: 0x00083C40
		public uint XboxOneInput_GetJoystickId(ulong xboxControllerId)
		{
			return 0U;
		}

		// Token: 0x060028F4 RID: 10484 RVA: 0x00002265 File Offset: 0x00000465
		public void XboxOne_Gamepad_UpdatePlugin()
		{
		}

		// Token: 0x060028F5 RID: 10485 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel)
		{
			return false;
		}

		// Token: 0x060028F6 RID: 10486 RVA: 0x00002265 File Offset: 0x00000465
		public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS)
		{
		}

		// Token: 0x060028F7 RID: 10487 RVA: 0x001FB25D File Offset: 0x001F945D
		public Vector3 PS4Input_GetLastAcceleration(int id)
		{
			return Vector3.zero;
		}

		// Token: 0x060028F8 RID: 10488 RVA: 0x001FB25D File Offset: 0x001F945D
		public Vector3 PS4Input_GetLastGyro(int id)
		{
			return Vector3.zero;
		}

		// Token: 0x060028F9 RID: 10489 RVA: 0x001FB264 File Offset: 0x001F9464
		public Vector4 PS4Input_GetLastOrientation(int id)
		{
			return Vector4.zero;
		}

		// Token: 0x060028FA RID: 10490 RVA: 0x001FB26B File Offset: 0x001F946B
		public void PS4Input_GetLastTouchData(int id, out int touchNum, out int touch0x, out int touch0y, out int touch0id, out int touch1x, out int touch1y, out int touch1id)
		{
			touchNum = 0;
			touch0x = 0;
			touch0y = 0;
			touch0id = 0;
			touch1x = 0;
			touch1y = 0;
			touch1id = 0;
		}

		// Token: 0x060028FB RID: 10491 RVA: 0x001FB287 File Offset: 0x001F9487
		public void PS4Input_GetPadControllerInformation(int id, out float touchpixelDensity, out int touchResolutionX, out int touchResolutionY, out int analogDeadZoneLeft, out int analogDeadZoneright, out int connectionType)
		{
			touchpixelDensity = 0f;
			touchResolutionX = 0;
			touchResolutionY = 0;
			analogDeadZoneLeft = 0;
			analogDeadZoneright = 0;
			connectionType = 0;
		}

		// Token: 0x060028FC RID: 10492 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_PadSetMotionSensorState(int id, bool bEnable)
		{
		}

		// Token: 0x060028FD RID: 10493 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_PadSetTiltCorrectionState(int id, bool bEnable)
		{
		}

		// Token: 0x060028FE RID: 10494 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_PadSetAngularVelocityDeadbandState(int id, bool bEnable)
		{
		}

		// Token: 0x060028FF RID: 10495 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_PadSetLightBar(int id, int red, int green, int blue)
		{
		}

		// Token: 0x06002900 RID: 10496 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_PadResetLightBar(int id)
		{
		}

		// Token: 0x06002901 RID: 10497 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_PadSetVibration(int id, int largeMotor, int smallMotor)
		{
		}

		// Token: 0x06002902 RID: 10498 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_PadResetOrientation(int id)
		{
		}

		// Token: 0x06002903 RID: 10499 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool PS4Input_PadIsConnected(int id)
		{
			return false;
		}

		// Token: 0x06002904 RID: 10500 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_GetUsersDetails(int slot, object loggedInUser)
		{
		}

		// Token: 0x06002905 RID: 10501 RVA: 0x00003884 File Offset: 0x00001A84
		public int PS4Input_GetDeviceClassForHandle(int handle)
		{
			return -1;
		}

		// Token: 0x06002906 RID: 10502 RVA: 0x00003EEE File Offset: 0x000020EE
		public string PS4Input_GetDeviceClassString(int intValue)
		{
			return null;
		}

		// Token: 0x06002907 RID: 10503 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_PadGetUsersHandles2(int maxControllers, int[] handles)
		{
			return 0;
		}

		// Token: 0x06002908 RID: 10504 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_GetSpecialControllerInformation(int id, int padIndex, object controllerInformation)
		{
		}

		// Token: 0x06002909 RID: 10505 RVA: 0x001FB25D File Offset: 0x001F945D
		public Vector3 PS4Input_SpecialGetLastAcceleration(int id)
		{
			return Vector3.zero;
		}

		// Token: 0x0600290A RID: 10506 RVA: 0x001FB25D File Offset: 0x001F945D
		public Vector3 PS4Input_SpecialGetLastGyro(int id)
		{
			return Vector3.zero;
		}

		// Token: 0x0600290B RID: 10507 RVA: 0x001FB264 File Offset: 0x001F9464
		public Vector4 PS4Input_SpecialGetLastOrientation(int id)
		{
			return Vector4.zero;
		}

		// Token: 0x0600290C RID: 10508 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_SpecialGetUsersHandles(int maxNumberControllers, int[] handles)
		{
			return 0;
		}

		// Token: 0x0600290D RID: 10509 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_SpecialGetUsersHandles2(int maxNumberControllers, int[] handles)
		{
			return 0;
		}

		// Token: 0x0600290E RID: 10510 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool PS4Input_SpecialIsConnected(int id)
		{
			return false;
		}

		// Token: 0x0600290F RID: 10511 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_SpecialResetLightSphere(int id)
		{
		}

		// Token: 0x06002910 RID: 10512 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_SpecialResetOrientation(int id)
		{
		}

		// Token: 0x06002911 RID: 10513 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_SpecialSetAngularVelocityDeadbandState(int id, bool bEnable)
		{
		}

		// Token: 0x06002912 RID: 10514 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_SpecialSetLightSphere(int id, int red, int green, int blue)
		{
		}

		// Token: 0x06002913 RID: 10515 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_SpecialSetMotionSensorState(int id, bool bEnable)
		{
		}

		// Token: 0x06002914 RID: 10516 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_SpecialSetTiltCorrectionState(int id, bool bEnable)
		{
		}

		// Token: 0x06002915 RID: 10517 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_SpecialSetVibration(int id, int largeMotor, int smallMotor)
		{
		}

		// Token: 0x06002916 RID: 10518 RVA: 0x001FB25D File Offset: 0x001F945D
		public Vector3 PS4Input_AimGetLastAcceleration(int id)
		{
			return Vector3.zero;
		}

		// Token: 0x06002917 RID: 10519 RVA: 0x001FB25D File Offset: 0x001F945D
		public Vector3 PS4Input_AimGetLastGyro(int id)
		{
			return Vector3.zero;
		}

		// Token: 0x06002918 RID: 10520 RVA: 0x001FB264 File Offset: 0x001F9464
		public Vector4 PS4Input_AimGetLastOrientation(int id)
		{
			return Vector4.zero;
		}

		// Token: 0x06002919 RID: 10521 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_AimGetUsersHandles(int maxNumberControllers, int[] handles)
		{
			return 0;
		}

		// Token: 0x0600291A RID: 10522 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_AimGetUsersHandles2(int maxNumberControllers, int[] handles)
		{
			return 0;
		}

		// Token: 0x0600291B RID: 10523 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool PS4Input_AimIsConnected(int id)
		{
			return false;
		}

		// Token: 0x0600291C RID: 10524 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_AimResetLightSphere(int id)
		{
		}

		// Token: 0x0600291D RID: 10525 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_AimResetOrientation(int id)
		{
		}

		// Token: 0x0600291E RID: 10526 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_AimSetAngularVelocityDeadbandState(int id, bool bEnable)
		{
		}

		// Token: 0x0600291F RID: 10527 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_AimSetLightSphere(int id, int red, int green, int blue)
		{
		}

		// Token: 0x06002920 RID: 10528 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_AimSetMotionSensorState(int id, bool bEnable)
		{
		}

		// Token: 0x06002921 RID: 10529 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_AimSetTiltCorrectionState(int id, bool bEnable)
		{
		}

		// Token: 0x06002922 RID: 10530 RVA: 0x00002265 File Offset: 0x00000465
		public void PS4Input_AimSetVibration(int id, int largeMotor, int smallMotor)
		{
		}

		// Token: 0x06002923 RID: 10531 RVA: 0x001FB25D File Offset: 0x001F945D
		public Vector3 PS4Input_GetLastMoveAcceleration(int id, int index)
		{
			return Vector3.zero;
		}

		// Token: 0x06002924 RID: 10532 RVA: 0x001FB25D File Offset: 0x001F945D
		public Vector3 PS4Input_GetLastMoveGyro(int id, int index)
		{
			return Vector3.zero;
		}

		// Token: 0x06002925 RID: 10533 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_MoveGetButtons(int id, int index)
		{
			return 0;
		}

		// Token: 0x06002926 RID: 10534 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_MoveGetAnalogButton(int id, int index)
		{
			return 0;
		}

		// Token: 0x06002927 RID: 10535 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool PS4Input_MoveIsConnected(int id, int index)
		{
			return false;
		}

		// Token: 0x06002928 RID: 10536 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles, int[] secondaryHandles)
		{
			return 0;
		}

		// Token: 0x06002929 RID: 10537 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles)
		{
			return 0;
		}

		// Token: 0x0600292A RID: 10538 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers)
		{
			return 0;
		}

		// Token: 0x0600292B RID: 10539 RVA: 0x001FB2A3 File Offset: 0x001F94A3
		public IntPtr PS4Input_MoveGetControllerInputForTracking()
		{
			return IntPtr.Zero;
		}

		// Token: 0x0600292C RID: 10540 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_MoveSetLightSphere(int id, int index, int red, int green, int blue)
		{
			return 0;
		}

		// Token: 0x0600292D RID: 10541 RVA: 0x00085A40 File Offset: 0x00083C40
		public int PS4Input_MoveSetVibration(int id, int index, int motor)
		{
			return 0;
		}

		// Token: 0x0600292E RID: 10542 RVA: 0x001FB2AA File Offset: 0x001F94AA
		public void GetDeviceVIDPIDs(out List<int> vids, out List<int> pids)
		{
			vids = new List<int>();
			pids = new List<int>();
		}

		// Token: 0x0600292F RID: 10543 RVA: 0x00003884 File Offset: 0x00001A84
		public int GetAndroidAPILevel()
		{
			return -1;
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x001FB2BA File Offset: 0x001F94BA
		public void WindowsStandalone_ForwardRawInput(IntPtr rawInputHeaderIndices, IntPtr rawInputDataIndices, uint indicesCount, IntPtr rawInputData, uint rawInputDataSize)
		{
			Functions.ForwardRawInput(rawInputHeaderIndices, rawInputDataIndices, indicesCount, rawInputData, rawInputDataSize);
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x001FB2C8 File Offset: 0x001F94C8
		public bool UnityUI_Graphic_GetRaycastTarget(object graphic)
		{
			return !(graphic as Graphic == null) && (graphic as Graphic).raycastTarget;
		}

		// Token: 0x06002932 RID: 10546 RVA: 0x001FB2E5 File Offset: 0x001F94E5
		public void UnityUI_Graphic_SetRaycastTarget(object graphic, bool value)
		{
			if (graphic as Graphic == null)
			{
				return;
			}
			(graphic as Graphic).raycastTarget = value;
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06002933 RID: 10547 RVA: 0x001FB302 File Offset: 0x001F9502
		public bool UnityInput_IsTouchPressureSupported
		{
			get
			{
				return Input.touchPressureSupported;
			}
		}

		// Token: 0x06002934 RID: 10548 RVA: 0x001FB309 File Offset: 0x001F9509
		public float UnityInput_GetTouchPressure(ref Touch touch)
		{
			return touch.pressure;
		}

		// Token: 0x06002935 RID: 10549 RVA: 0x001FB311 File Offset: 0x001F9511
		public float UnityInput_GetTouchMaximumPossiblePressure(ref Touch touch)
		{
			return touch.maximumPossiblePressure;
		}

		// Token: 0x06002936 RID: 10550 RVA: 0x001FB319 File Offset: 0x001F9519
		public IControllerTemplate CreateControllerTemplate(Guid typeGuid, object payload)
		{
			return ControllerTemplateFactory.Create(typeGuid, payload);
		}

		// Token: 0x06002937 RID: 10551 RVA: 0x001FB322 File Offset: 0x001F9522
		public Type[] GetControllerTemplateTypes()
		{
			return ControllerTemplateFactory.templateTypes;
		}

		// Token: 0x06002938 RID: 10552 RVA: 0x001FB329 File Offset: 0x001F9529
		public Type[] GetControllerTemplateInterfaceTypes()
		{
			return ControllerTemplateFactory.templateInterfaceTypes;
		}

		// Token: 0x04004730 RID: 18224
		private static Func<object> _getPlatformInitializerDelegate;

		// Token: 0x04004731 RID: 18225
		private bool _isEditorPaused;

		// Token: 0x04004732 RID: 18226
		private Action<bool> _EditorPausedStateChangedEvent;
	}
}
