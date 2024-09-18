using System;
using UnityEngine;

namespace BrainFailProductions.PolyFewRuntime
{
	// Token: 0x02000938 RID: 2360
	public class FlyCamera : MonoBehaviour
	{
		// Token: 0x0600321B RID: 12827 RVA: 0x0022377C File Offset: 0x0022197C
		private void Start()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			this.x = eulerAngles.y;
			this.y = eulerAngles.x;
			this.rigidbody = base.GetComponent<Rigidbody>();
			if (this.rigidbody != null)
			{
				this.rigidbody.freezeRotation = true;
			}
		}

		// Token: 0x0600321C RID: 12828 RVA: 0x002237D4 File Offset: 0x002219D4
		private void Update()
		{
			if (FlyCamera.deactivated)
			{
				return;
			}
			if (this.target)
			{
				float num = Input.GetAxis("Mouse X");
				float num2 = Input.GetAxis("Mouse Y");
				if (!Input.GetMouseButton(0))
				{
					num = 0f;
					num2 = 0f;
				}
				this.x += num * this.xSpeed * this.distance * 0.02f;
				this.y -= num2 * this.ySpeed * 0.02f;
				this.y = FlyCamera.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
				Quaternion quaternion = Quaternion.Euler(this.y, this.x, 0f);
				this.distance = Mathf.Clamp(this.distance - Input.GetAxis("Mouse ScrollWheel") * 5f, this.distanceMin, this.distanceMax);
				RaycastHit raycastHit;
				if (Physics.Linecast(this.target.position, base.transform.position, ref raycastHit))
				{
					this.distance -= raycastHit.distance;
				}
				Vector3 vector;
				vector..ctor(0f, 0f, -this.distance);
				Vector3 position = quaternion * vector + this.target.position;
				base.transform.position = position;
				base.transform.rotation = quaternion;
			}
		}

		// Token: 0x0600321D RID: 12829 RVA: 0x00223940 File Offset: 0x00221B40
		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}

		// Token: 0x04004DA7 RID: 19879
		public Transform target;

		// Token: 0x04004DA8 RID: 19880
		public float distance = 5f;

		// Token: 0x04004DA9 RID: 19881
		public float xSpeed = 120f;

		// Token: 0x04004DAA RID: 19882
		public float ySpeed = 120f;

		// Token: 0x04004DAB RID: 19883
		public float panSpeed = 0.05f;

		// Token: 0x04004DAC RID: 19884
		public float yMinLimit = -20f;

		// Token: 0x04004DAD RID: 19885
		public float yMaxLimit = 80f;

		// Token: 0x04004DAE RID: 19886
		public float distanceMin = 0.5f;

		// Token: 0x04004DAF RID: 19887
		public float distanceMax = 15f;

		// Token: 0x04004DB0 RID: 19888
		private Rigidbody rigidbody;

		// Token: 0x04004DB1 RID: 19889
		private float x;

		// Token: 0x04004DB2 RID: 19890
		private float y;

		// Token: 0x04004DB3 RID: 19891
		public static bool deactivated;
	}
}
