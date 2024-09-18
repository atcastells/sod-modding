using System;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x0200081F RID: 2079
	public class HeadBob : MonoBehaviour
	{
		// Token: 0x060026BB RID: 9915 RVA: 0x001F9C04 File Offset: 0x001F7E04
		private void Start()
		{
			this.motionBob.Setup(this.Camera, this.StrideInterval, this.Camera.transform.localPosition);
			this.m_OriginalCameraPosition = this.Camera.transform.localPosition;
		}

		// Token: 0x060026BC RID: 9916 RVA: 0x001F9C44 File Offset: 0x001F7E44
		private void Update()
		{
			Vector3 localPosition;
			if (this.rigidbodyFirstPersonController.Velocity.magnitude > 0f && this.rigidbodyFirstPersonController.Grounded)
			{
				this.Camera.transform.localPosition = Player.Instance.fps.DoHeadBob(this.motionBob, this.rigidbodyFirstPersonController.Velocity.magnitude * (this.rigidbodyFirstPersonController.Running ? this.RunningStrideLengthen : 1f), GameplayControls.Instance.headBobMultiplier);
				localPosition = this.Camera.transform.localPosition;
				localPosition.y = this.Camera.transform.localPosition.y - this.jumpAndLandingBob.Offset();
			}
			else
			{
				localPosition = this.Camera.transform.localPosition;
				localPosition.y = this.m_OriginalCameraPosition.y - this.jumpAndLandingBob.Offset();
			}
			this.Camera.transform.localPosition = localPosition;
			if (!this.m_PreviouslyGrounded && this.rigidbodyFirstPersonController.Grounded)
			{
				base.StartCoroutine(this.jumpAndLandingBob.DoBobCycle());
			}
			this.m_PreviouslyGrounded = this.rigidbodyFirstPersonController.Grounded;
		}

		// Token: 0x0400456A RID: 17770
		public Camera Camera;

		// Token: 0x0400456B RID: 17771
		public CurveControlledBob motionBob = new CurveControlledBob();

		// Token: 0x0400456C RID: 17772
		public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();

		// Token: 0x0400456D RID: 17773
		public RigidbodyFirstPersonController rigidbodyFirstPersonController;

		// Token: 0x0400456E RID: 17774
		public float StrideInterval;

		// Token: 0x0400456F RID: 17775
		[Range(0f, 1f)]
		public float RunningStrideLengthen;

		// Token: 0x04004570 RID: 17776
		private bool m_PreviouslyGrounded;

		// Token: 0x04004571 RID: 17777
		private Vector3 m_OriginalCameraPosition;
	}
}
