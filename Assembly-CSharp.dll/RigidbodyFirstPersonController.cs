using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x02000821 RID: 2081
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class RigidbodyFirstPersonController : MonoBehaviour
	{
		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060026C3 RID: 9923 RVA: 0x001FA3CC File Offset: 0x001F85CC
		public Vector3 Velocity
		{
			get
			{
				return this.m_RigidBody.velocity;
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060026C4 RID: 9924 RVA: 0x001FA3D9 File Offset: 0x001F85D9
		public bool Grounded
		{
			get
			{
				return this.m_IsGrounded;
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060026C5 RID: 9925 RVA: 0x001FA3E1 File Offset: 0x001F85E1
		public bool Jumping
		{
			get
			{
				return this.m_Jumping;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060026C6 RID: 9926 RVA: 0x001FA3E9 File Offset: 0x001F85E9
		public bool Running
		{
			get
			{
				return this.movementSettings.Running;
			}
		}

		// Token: 0x060026C7 RID: 9927 RVA: 0x001FA3F6 File Offset: 0x001F85F6
		private void Start()
		{
			this.m_RigidBody = base.GetComponent<Rigidbody>();
			this.m_Capsule = base.GetComponent<CapsuleCollider>();
			this.mouseLook.Init(base.transform, this.cam.transform);
		}

		// Token: 0x060026C8 RID: 9928 RVA: 0x001FA42C File Offset: 0x001F862C
		private void Update()
		{
			this.RotateView();
			if (CrossPlatformInputManager.GetButtonDown("Jump") && !this.m_Jump)
			{
				this.m_Jump = true;
			}
		}

		// Token: 0x060026C9 RID: 9929 RVA: 0x001FA450 File Offset: 0x001F8650
		private void FixedUpdate()
		{
			this.GroundCheck();
			Vector2 input = this.GetInput();
			if ((Mathf.Abs(input.x) > 1E-45f || Mathf.Abs(input.y) > 1E-45f) && (this.advancedSettings.airControl || this.m_IsGrounded))
			{
				Vector3 vector = this.cam.transform.forward * input.y + this.cam.transform.right * input.x;
				vector = Vector3.ProjectOnPlane(vector, this.m_GroundContactNormal).normalized;
				vector.x *= this.movementSettings.CurrentTargetSpeed;
				vector.z *= this.movementSettings.CurrentTargetSpeed;
				vector.y *= this.movementSettings.CurrentTargetSpeed;
				if (this.m_RigidBody.velocity.sqrMagnitude < this.movementSettings.CurrentTargetSpeed * this.movementSettings.CurrentTargetSpeed)
				{
					this.m_RigidBody.AddForce(vector * this.SlopeMultiplier(), 1);
				}
			}
			if (this.m_IsGrounded)
			{
				this.m_RigidBody.drag = 5f;
				if (this.m_Jump)
				{
					this.m_RigidBody.drag = 0f;
					this.m_RigidBody.velocity = new Vector3(this.m_RigidBody.velocity.x, 0f, this.m_RigidBody.velocity.z);
					this.m_RigidBody.AddForce(new Vector3(0f, this.movementSettings.JumpForce, 0f), 1);
					this.m_Jumping = true;
				}
				if (!this.m_Jumping && Mathf.Abs(input.x) < 1E-45f && Mathf.Abs(input.y) < 1E-45f && this.m_RigidBody.velocity.magnitude < 1f)
				{
					this.m_RigidBody.Sleep();
				}
			}
			else
			{
				this.m_RigidBody.drag = 0f;
				if (this.m_PreviouslyGrounded && !this.m_Jumping)
				{
					this.StickToGroundHelper();
				}
			}
			this.m_Jump = false;
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x001FA6A0 File Offset: 0x001F88A0
		private float SlopeMultiplier()
		{
			float num = Vector3.Angle(this.m_GroundContactNormal, Vector3.up);
			return this.movementSettings.SlopeCurveModifier.Evaluate(num);
		}

		// Token: 0x060026CB RID: 9931 RVA: 0x001FA6D0 File Offset: 0x001F88D0
		private void StickToGroundHelper()
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius * (1f - this.advancedSettings.shellOffset), Vector3.down, ref raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.stickToGroundHelperDistance, -1, 1) && Mathf.Abs(Vector3.Angle(raycastHit.normal, Vector3.up)) < 85f)
			{
				this.m_RigidBody.velocity = Vector3.ProjectOnPlane(this.m_RigidBody.velocity, raycastHit.normal);
			}
		}

		// Token: 0x060026CC RID: 9932 RVA: 0x001FA780 File Offset: 0x001F8980
		private Vector2 GetInput()
		{
			Vector2 vector = default(Vector2);
			vector.x = CrossPlatformInputManager.GetAxis("Horizontal");
			vector.y = CrossPlatformInputManager.GetAxis("Vertical");
			Vector2 vector2 = vector;
			this.movementSettings.UpdateDesiredTargetSpeed(vector2);
			return vector2;
		}

		// Token: 0x060026CD RID: 9933 RVA: 0x001FA7C8 File Offset: 0x001F89C8
		private void RotateView()
		{
			if (Mathf.Abs(Time.timeScale) < 1E-45f)
			{
				return;
			}
			float y = base.transform.eulerAngles.y;
			this.mouseLook.LookRotation(base.transform, this.cam.transform, false);
			if (this.m_IsGrounded || this.advancedSettings.airControl)
			{
				Quaternion quaternion = Quaternion.AngleAxis(base.transform.eulerAngles.y - y, Vector3.up);
				this.m_RigidBody.velocity = quaternion * this.m_RigidBody.velocity;
			}
		}

		// Token: 0x060026CE RID: 9934 RVA: 0x001FA864 File Offset: 0x001F8A64
		private void GroundCheck()
		{
			this.m_PreviouslyGrounded = this.m_IsGrounded;
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius * (1f - this.advancedSettings.shellOffset), Vector3.down, ref raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.groundCheckDistance, -1, 1))
			{
				this.m_IsGrounded = true;
				this.m_GroundContactNormal = raycastHit.normal;
			}
			else
			{
				this.m_IsGrounded = false;
				this.m_GroundContactNormal = Vector3.up;
			}
			if (!this.m_PreviouslyGrounded && this.m_IsGrounded && this.m_Jumping)
			{
				this.m_Jumping = false;
			}
		}

		// Token: 0x0400457F RID: 17791
		public Camera cam;

		// Token: 0x04004580 RID: 17792
		public RigidbodyFirstPersonController.MovementSettings movementSettings = new RigidbodyFirstPersonController.MovementSettings();

		// Token: 0x04004581 RID: 17793
		public MouseLook mouseLook = new MouseLook();

		// Token: 0x04004582 RID: 17794
		public RigidbodyFirstPersonController.AdvancedSettings advancedSettings = new RigidbodyFirstPersonController.AdvancedSettings();

		// Token: 0x04004583 RID: 17795
		private Rigidbody m_RigidBody;

		// Token: 0x04004584 RID: 17796
		private CapsuleCollider m_Capsule;

		// Token: 0x04004585 RID: 17797
		private float m_YRotation;

		// Token: 0x04004586 RID: 17798
		private Vector3 m_GroundContactNormal;

		// Token: 0x04004587 RID: 17799
		private bool m_Jump;

		// Token: 0x04004588 RID: 17800
		private bool m_PreviouslyGrounded;

		// Token: 0x04004589 RID: 17801
		private bool m_Jumping;

		// Token: 0x0400458A RID: 17802
		private bool m_IsGrounded;

		// Token: 0x02000822 RID: 2082
		[Serializable]
		public class MovementSettings
		{
			// Token: 0x060026D0 RID: 9936 RVA: 0x001FA950 File Offset: 0x001F8B50
			public void UpdateDesiredTargetSpeed(Vector2 input)
			{
				if (input == Vector2.zero)
				{
					return;
				}
				if (input.x > 0f || input.x < 0f)
				{
					this.CurrentTargetSpeed = this.StrafeSpeed;
				}
				if (input.y < 0f)
				{
					this.CurrentTargetSpeed = this.BackwardSpeed;
				}
				if (input.y > 0f)
				{
					this.CurrentTargetSpeed = this.ForwardSpeed;
				}
				if (Input.GetKey(this.RunKey))
				{
					this.CurrentTargetSpeed *= this.RunMultiplier;
					this.m_Running = true;
					return;
				}
				this.m_Running = false;
			}

			// Token: 0x1700012D RID: 301
			// (get) Token: 0x060026D1 RID: 9937 RVA: 0x001FA9F2 File Offset: 0x001F8BF2
			public bool Running
			{
				get
				{
					return this.m_Running;
				}
			}

			// Token: 0x0400458B RID: 17803
			public float ForwardSpeed = 8f;

			// Token: 0x0400458C RID: 17804
			public float BackwardSpeed = 4f;

			// Token: 0x0400458D RID: 17805
			public float StrafeSpeed = 4f;

			// Token: 0x0400458E RID: 17806
			public float RunMultiplier = 2f;

			// Token: 0x0400458F RID: 17807
			public KeyCode RunKey = 304;

			// Token: 0x04004590 RID: 17808
			public float JumpForce = 30f;

			// Token: 0x04004591 RID: 17809
			public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(-90f, 1f),
				new Keyframe(0f, 1f),
				new Keyframe(90f, 0f)
			});

			// Token: 0x04004592 RID: 17810
			[HideInInspector]
			public float CurrentTargetSpeed = 8f;

			// Token: 0x04004593 RID: 17811
			private bool m_Running;
		}

		// Token: 0x02000823 RID: 2083
		[Serializable]
		public class AdvancedSettings
		{
			// Token: 0x04004594 RID: 17812
			public float groundCheckDistance = 0.01f;

			// Token: 0x04004595 RID: 17813
			public float stickToGroundHelperDistance = 0.5f;

			// Token: 0x04004596 RID: 17814
			public float slowDownRate = 20f;

			// Token: 0x04004597 RID: 17815
			public bool airControl;

			// Token: 0x04004598 RID: 17816
			[Tooltip("set it to 0.1 or more if you get stuck in wall")]
			public float shellOffset;
		}
	}
}
