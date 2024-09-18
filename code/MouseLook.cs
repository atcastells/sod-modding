using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x02000820 RID: 2080
	[Serializable]
	public class MouseLook
	{
		// Token: 0x060026BE RID: 9918 RVA: 0x001F9DAC File Offset: 0x001F7FAC
		public void Init(Transform character, Transform camera)
		{
			this.m_CharacterTargetRot = character.localRotation;
			this.m_CameraTargetRot = camera.localRotation;
		}

		// Token: 0x060026BF RID: 9919 RVA: 0x001F9DC8 File Offset: 0x001F7FC8
		public void LookRotation(Transform character, Transform camera, bool disableClamp = false)
		{
			float num = 1f;
			if (Player.Instance.transitionActive && Player.Instance.currentTransition != null)
			{
				num = Mathf.Clamp01(Player.Instance.currentTransition.mouseLookControlCurve.Evaluate(Player.Instance.transitionProgress));
			}
			Vector2 vector = Game.Instance.mouseSensitivity * 0.1f;
			if (!InputController.Instance.mouseInputMode)
			{
				vector = Game.Instance.controllerSensitivity * 0.1f;
			}
			float num2 = InputController.Instance.GetAxisRelative("LookHorizontal") * vector.x * num * Game.Instance.axisMP.x;
			float num3 = InputController.Instance.GetAxisRelative("LookVertical") * vector.y * num * Game.Instance.axisMP.y;
			if (InputController.Instance.mouseInputMode)
			{
				this.m_CharacterTargetRot *= Quaternion.Euler(0f, num2, 0f);
				this.m_CameraTargetRot *= Quaternion.Euler(-num3, 0f, 0f);
			}
			if (this.clampVerticalRotation && !disableClamp)
			{
				this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot, this.MinimumX, this.MaximumX);
			}
			Quaternion rotation = character.rotation;
			Quaternion rotation2 = camera.rotation;
			if (InputController.Instance.mouseInputMode && Game.Instance.mouseSmoothing > 0)
			{
				float num4 = (float)(51 - Game.Instance.mouseSmoothing) * Time.deltaTime;
				this.m_CharacterTargetRot = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, num4);
				this.m_CameraTargetRot = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, num4);
			}
			else if (!InputController.Instance.mouseInputMode && Game.Instance.controllerSmoothing > 0)
			{
				float num5 = this.controllerRemapCurve.Evaluate(InputController.Instance.player.GetAxis("LookHorizontal"));
				float num6 = this.controllerRemapCurve.Evaluate(InputController.Instance.player.GetAxis("LookVertical"));
				num2 = num5 * vector.x * num * Game.Instance.axisMP.x;
				num3 = num6 * vector.y * num * Game.Instance.axisMP.y;
				this.m_CharacterTargetRot *= Quaternion.Euler(0f, num2, 0f);
				this.m_CameraTargetRot *= Quaternion.Euler(-num3, 0f, 0f);
				character.localRotation = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, (float)Game.Instance.controllerSmoothing * Time.deltaTime);
				camera.localRotation = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, (float)Game.Instance.controllerSmoothing * Time.deltaTime);
			}
			if (InteractionController.Instance.currentlyDragging != null)
			{
				float num7 = GameplayControls.Instance.maxAngleMovementWhenDragging * Time.smoothDeltaTime;
				float num8 = Quaternion.Angle(character.localRotation, this.m_CharacterTargetRot);
				float num9 = Quaternion.Angle(camera.localRotation, this.m_CameraTargetRot);
				if (num8 > num7)
				{
					float num10 = Mathf.Clamp01(num7 / num8);
					this.m_CharacterTargetRot = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, num10);
				}
				if (num9 > num7)
				{
					float num11 = Mathf.Clamp01(num7 / num9);
					this.m_CameraTargetRot = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, num11);
				}
			}
			character.localRotation = this.m_CharacterTargetRot;
			camera.localRotation = this.m_CameraTargetRot;
			if (StatusController.Instance.drunkVision > 0f)
			{
				character.rotation *= Quaternion.Euler(0f, SessionData.Instance.drunkOscillation.y * StatusController.Instance.drunkVision, 0f);
				camera.rotation *= Quaternion.Euler(SessionData.Instance.drunkOscillation.x * StatusController.Instance.drunkVision, 0f, 0f);
			}
			if (StatusController.Instance.shiverVision > 0f)
			{
				character.rotation *= Quaternion.Euler(0f, SessionData.Instance.shiverOscillation.y * StatusController.Instance.shiverVision, 0f);
				camera.rotation *= Quaternion.Euler(SessionData.Instance.shiverOscillation.x * StatusController.Instance.shiverVision, 0f, 0f);
			}
			this.charMovementThisFrame = character.rotation * Quaternion.Inverse(rotation);
			this.camMovementThisFrame = camera.rotation * Quaternion.Inverse(rotation2);
		}

		// Token: 0x060026C0 RID: 9920 RVA: 0x001FA2B0 File Offset: 0x001F84B0
		public Quaternion ClampRotationAroundXAxis(Quaternion q, float min, float max)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1f;
			float num = 114.59156f * Mathf.Atan(q.x);
			num = Mathf.Clamp(num, min, max);
			q.x = Mathf.Tan(0.008726646f * num);
			return q;
		}

		// Token: 0x060026C1 RID: 9921 RVA: 0x001FA32C File Offset: 0x001F852C
		public Quaternion ClampRotationAroundYAxis(Quaternion q, float min, float max)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1f;
			float num = 114.59156f * Mathf.Atan(q.y);
			num = Mathf.Clamp(num, min, max);
			q.y = Mathf.Tan(0.008726646f * num);
			return q;
		}

		// Token: 0x04004572 RID: 17778
		public bool clampVerticalRotation = true;

		// Token: 0x04004573 RID: 17779
		public float MinimumX = -90f;

		// Token: 0x04004574 RID: 17780
		public float MaximumX = 90f;

		// Token: 0x04004575 RID: 17781
		public bool lockCursor;

		// Token: 0x04004576 RID: 17782
		public Quaternion charMovementThisFrame;

		// Token: 0x04004577 RID: 17783
		public Quaternion camMovementThisFrame;

		// Token: 0x04004578 RID: 17784
		private Quaternion m_CharacterTargetRot;

		// Token: 0x04004579 RID: 17785
		private Quaternion m_CameraTargetRot;

		// Token: 0x0400457A RID: 17786
		public AnimationCurve controllerRemapCurve;

		// Token: 0x0400457B RID: 17787
		private float _controllerInputX;

		// Token: 0x0400457C RID: 17788
		private float _controllerInputY;

		// Token: 0x0400457D RID: 17789
		private float _controllerXRot;

		// Token: 0x0400457E RID: 17790
		private bool _invertY;
	}
}
