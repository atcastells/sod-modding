using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.LightningBolt
{
	// Token: 0x02000969 RID: 2409
	[RequireComponent(typeof(LineRenderer))]
	public class LightningBoltScript : MonoBehaviour
	{
		// Token: 0x060032EB RID: 13035 RVA: 0x0022D3CC File Offset: 0x0022B5CC
		private void GetPerpendicularVector(ref Vector3 directionNormalized, out Vector3 side)
		{
			if (directionNormalized == Vector3.zero)
			{
				side = Vector3.right;
				return;
			}
			float x = directionNormalized.x;
			float y = directionNormalized.y;
			float z = directionNormalized.z;
			float num = Mathf.Abs(x);
			float num2 = Mathf.Abs(y);
			float num3 = Mathf.Abs(z);
			float num4;
			float num5;
			float num6;
			if (num >= num2 && num2 >= num3)
			{
				num4 = 1f;
				num5 = 1f;
				num6 = -(y * num4 + z * num5) / x;
			}
			else if (num2 >= num3)
			{
				num6 = 1f;
				num5 = 1f;
				num4 = -(x * num6 + z * num5) / y;
			}
			else
			{
				num6 = 1f;
				num4 = 1f;
				num5 = -(x * num6 + y * num4) / z;
			}
			side = new Vector3(num6, num4, num5).normalized;
		}

		// Token: 0x060032EC RID: 13036 RVA: 0x0022D4A0 File Offset: 0x0022B6A0
		private void GenerateLightningBolt(Vector3 start, Vector3 end, int generation, int totalGenerations, float offsetAmount)
		{
			if (generation < 0 || generation > 8)
			{
				return;
			}
			if (this.orthographic)
			{
				start.z = (end.z = Mathf.Min(start.z, end.z));
			}
			this.segments.Add(new KeyValuePair<Vector3, Vector3>(start, end));
			if (generation == 0)
			{
				return;
			}
			if (offsetAmount <= 0f)
			{
				offsetAmount = (end - start).magnitude * this.ChaosFactor;
			}
			while (generation-- > 0)
			{
				int num = this.startIndex;
				this.startIndex = this.segments.Count;
				for (int i = num; i < this.startIndex; i++)
				{
					start = this.segments[i].Key;
					end = this.segments[i].Value;
					Vector3 vector = (start + end) * 0.5f;
					Vector3 vector2;
					this.RandomVector(ref start, ref end, offsetAmount, out vector2);
					vector += vector2;
					this.segments.Add(new KeyValuePair<Vector3, Vector3>(start, vector));
					this.segments.Add(new KeyValuePair<Vector3, Vector3>(vector, end));
				}
				offsetAmount *= 0.5f;
			}
		}

		// Token: 0x060032ED RID: 13037 RVA: 0x0022D5E4 File Offset: 0x0022B7E4
		public void RandomVector(ref Vector3 start, ref Vector3 end, float offsetAmount, out Vector3 result)
		{
			if (this.orthographic)
			{
				Vector3 normalized = (end - start).normalized;
				Vector3 vector;
				vector..ctor(-normalized.y, normalized.x, normalized.z);
				float num = (float)this.RandomGenerator.NextDouble() * offsetAmount * 2f - offsetAmount;
				result = vector * num;
				return;
			}
			Vector3 normalized2 = (end - start).normalized;
			Vector3 vector2;
			this.GetPerpendicularVector(ref normalized2, out vector2);
			float num2 = ((float)this.RandomGenerator.NextDouble() + 0.1f) * offsetAmount;
			float num3 = (float)this.RandomGenerator.NextDouble() * 360f;
			result = Quaternion.AngleAxis(num3, normalized2) * vector2 * num2;
		}

		// Token: 0x060032EE RID: 13038 RVA: 0x0022D6C0 File Offset: 0x0022B8C0
		private void SelectOffsetFromAnimationMode()
		{
			if (this.AnimationMode == LightningBoltAnimationMode.None)
			{
				this.lineRenderer.material.mainTextureOffset = this.offsets[0];
				return;
			}
			int num;
			if (this.AnimationMode == LightningBoltAnimationMode.PingPong)
			{
				num = this.animationOffsetIndex;
				this.animationOffsetIndex += this.animationPingPongDirection;
				if (this.animationOffsetIndex >= this.offsets.Length)
				{
					this.animationOffsetIndex = this.offsets.Length - 2;
					this.animationPingPongDirection = -1;
				}
				else if (this.animationOffsetIndex < 0)
				{
					this.animationOffsetIndex = 1;
					this.animationPingPongDirection = 1;
				}
			}
			else if (this.AnimationMode == LightningBoltAnimationMode.Loop)
			{
				int num2 = this.animationOffsetIndex;
				this.animationOffsetIndex = num2 + 1;
				num = num2;
				if (this.animationOffsetIndex >= this.offsets.Length)
				{
					this.animationOffsetIndex = 0;
				}
			}
			else
			{
				num = this.RandomGenerator.Next(0, this.offsets.Length);
			}
			if (num >= 0 && num < this.offsets.Length)
			{
				this.lineRenderer.material.mainTextureOffset = this.offsets[num];
				return;
			}
			this.lineRenderer.material.mainTextureOffset = this.offsets[0];
		}

		// Token: 0x060032EF RID: 13039 RVA: 0x0022D7E8 File Offset: 0x0022B9E8
		private void UpdateLineRenderer()
		{
			int num = this.segments.Count - this.startIndex + 1;
			this.lineRenderer.positionCount = num;
			if (num < 1)
			{
				return;
			}
			int num2 = 0;
			this.lineRenderer.SetPosition(num2++, this.segments[this.startIndex].Key);
			for (int i = this.startIndex; i < this.segments.Count; i++)
			{
				this.lineRenderer.SetPosition(num2++, this.segments[i].Value);
			}
			this.segments.Clear();
			this.SelectOffsetFromAnimationMode();
		}

		// Token: 0x060032F0 RID: 13040 RVA: 0x0022D896 File Offset: 0x0022BA96
		private void Start()
		{
			this.orthographic = (Camera.main != null && Camera.main.orthographic);
			this.lineRenderer = base.GetComponent<LineRenderer>();
			this.lineRenderer.positionCount = 0;
			this.UpdateFromMaterialChange();
		}

		// Token: 0x060032F1 RID: 13041 RVA: 0x0022D8D8 File Offset: 0x0022BAD8
		private void Update()
		{
			this.orthographic = (Camera.main != null && Camera.main.orthographic);
			if (this.timer <= 0f)
			{
				if (this.ManualMode)
				{
					this.timer = this.Duration;
					this.lineRenderer.positionCount = 0;
				}
				else
				{
					this.Trigger();
				}
			}
			this.timer -= Time.deltaTime;
		}

		// Token: 0x060032F2 RID: 13042 RVA: 0x0022D94C File Offset: 0x0022BB4C
		public void Trigger()
		{
			this.timer = this.Duration + Mathf.Min(0f, this.timer);
			Vector3 start;
			if (this.StartObject == null)
			{
				start = this.StartPosition;
			}
			else
			{
				start = this.StartObject.transform.position + this.StartPosition;
			}
			Vector3 end;
			if (this.EndObject == null)
			{
				end = this.EndPosition;
			}
			else
			{
				end = this.EndObject.transform.position + this.EndPosition;
			}
			this.startIndex = 0;
			this.GenerateLightningBolt(start, end, this.Generations, this.Generations, 0f);
			this.UpdateLineRenderer();
		}

		// Token: 0x060032F3 RID: 13043 RVA: 0x0022DA04 File Offset: 0x0022BC04
		public void UpdateFromMaterialChange()
		{
			this.size = new Vector2(1f / (float)this.Columns, 1f / (float)this.Rows);
			this.lineRenderer.material.mainTextureScale = this.size;
			this.offsets = new Vector2[this.Rows * this.Columns];
			for (int i = 0; i < this.Rows; i++)
			{
				for (int j = 0; j < this.Columns; j++)
				{
					this.offsets[j + i * this.Columns] = new Vector2((float)j / (float)this.Columns, (float)i / (float)this.Rows);
				}
			}
		}

		// Token: 0x04004EFA RID: 20218
		[Tooltip("The game object where the lightning will emit from. If null, StartPosition is used.")]
		public GameObject StartObject;

		// Token: 0x04004EFB RID: 20219
		[Tooltip("The start position where the lightning will emit from. This is in world space if StartObject is null, otherwise this is offset from StartObject position.")]
		public Vector3 StartPosition;

		// Token: 0x04004EFC RID: 20220
		[Tooltip("The game object where the lightning will end at. If null, EndPosition is used.")]
		public GameObject EndObject;

		// Token: 0x04004EFD RID: 20221
		[Tooltip("The end position where the lightning will end at. This is in world space if EndObject is null, otherwise this is offset from EndObject position.")]
		public Vector3 EndPosition;

		// Token: 0x04004EFE RID: 20222
		[Range(0f, 8f)]
		[Tooltip("How manu generations? Higher numbers create more line segments.")]
		public int Generations = 6;

		// Token: 0x04004EFF RID: 20223
		[Tooltip("How long each bolt should last before creating a new bolt. In ManualMode, the bolt will simply disappear after this amount of seconds.")]
		[Range(0.01f, 1f)]
		public float Duration = 0.05f;

		// Token: 0x04004F00 RID: 20224
		private float timer;

		// Token: 0x04004F01 RID: 20225
		[Range(0f, 1f)]
		[Tooltip("How chaotic should the lightning be? (0-1)")]
		public float ChaosFactor = 0.15f;

		// Token: 0x04004F02 RID: 20226
		[Tooltip("In manual mode, the trigger method must be called to create a bolt")]
		public bool ManualMode;

		// Token: 0x04004F03 RID: 20227
		[Tooltip("The number of rows in the texture. Used for animation.")]
		[Range(1f, 64f)]
		public int Rows = 1;

		// Token: 0x04004F04 RID: 20228
		[Range(1f, 64f)]
		[Tooltip("The number of columns in the texture. Used for animation.")]
		public int Columns = 1;

		// Token: 0x04004F05 RID: 20229
		[Tooltip("The animation mode for the lightning")]
		public LightningBoltAnimationMode AnimationMode = LightningBoltAnimationMode.PingPong;

		// Token: 0x04004F06 RID: 20230
		[HideInInspector]
		[NonSerialized]
		public Random RandomGenerator = new Random();

		// Token: 0x04004F07 RID: 20231
		private LineRenderer lineRenderer;

		// Token: 0x04004F08 RID: 20232
		private List<KeyValuePair<Vector3, Vector3>> segments = new List<KeyValuePair<Vector3, Vector3>>();

		// Token: 0x04004F09 RID: 20233
		private int startIndex;

		// Token: 0x04004F0A RID: 20234
		private Vector2 size;

		// Token: 0x04004F0B RID: 20235
		private Vector2[] offsets;

		// Token: 0x04004F0C RID: 20236
		private int animationOffsetIndex;

		// Token: 0x04004F0D RID: 20237
		private int animationPingPongDirection = 1;

		// Token: 0x04004F0E RID: 20238
		private bool orthographic;
	}
}
