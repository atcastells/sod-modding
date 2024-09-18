using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL.MathUtil
{
	// Token: 0x02000935 RID: 2357
	public class Vertex
	{
		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06003203 RID: 12803 RVA: 0x00223112 File Offset: 0x00221312
		// (set) Token: 0x06003204 RID: 12804 RVA: 0x0022311A File Offset: 0x0022131A
		public Vector3 Position { get; private set; }

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06003205 RID: 12805 RVA: 0x00223123 File Offset: 0x00221323
		// (set) Token: 0x06003206 RID: 12806 RVA: 0x0022312B File Offset: 0x0022132B
		public int OriginalIndex { get; private set; }

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06003207 RID: 12807 RVA: 0x00223134 File Offset: 0x00221334
		// (set) Token: 0x06003208 RID: 12808 RVA: 0x0022313C File Offset: 0x0022133C
		public Vertex PreviousVertex
		{
			get
			{
				return this.prevVertex;
			}
			set
			{
				this.triangleHasChanged = (this.prevVertex != value);
				this.prevVertex = value;
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06003209 RID: 12809 RVA: 0x00223157 File Offset: 0x00221357
		// (set) Token: 0x0600320A RID: 12810 RVA: 0x0022315F File Offset: 0x0022135F
		public Vertex NextVertex
		{
			get
			{
				return this.nextVertex;
			}
			set
			{
				this.triangleHasChanged = (this.nextVertex != value);
				this.nextVertex = value;
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x0600320B RID: 12811 RVA: 0x0022317A File Offset: 0x0022137A
		public float TriangleArea
		{
			get
			{
				if (this.triangleHasChanged)
				{
					this.ComputeTriangleArea();
				}
				return this.triangleArea;
			}
		}

		// Token: 0x0600320C RID: 12812 RVA: 0x00223190 File Offset: 0x00221390
		public Vertex(int originalIndex, Vector3 position)
		{
			this.OriginalIndex = originalIndex;
			this.Position = position;
		}

		// Token: 0x0600320D RID: 12813 RVA: 0x002231A8 File Offset: 0x002213A8
		public Vector2 GetPosOnPlane(Vector3 planeNormal)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion.SetFromToRotation(planeNormal, Vector3.back);
			Vector3 vector = quaternion * this.Position;
			return new Vector2(vector.x, vector.y);
		}

		// Token: 0x0600320E RID: 12814 RVA: 0x002231E8 File Offset: 0x002213E8
		private void ComputeTriangleArea()
		{
			Vector3 vector = this.PreviousVertex.Position - this.Position;
			Vector3 vector2 = this.NextVertex.Position - this.Position;
			this.triangleArea = Vector3.Cross(vector, vector2).magnitude / 2f;
		}

		// Token: 0x04004DA1 RID: 19873
		private Vertex prevVertex;

		// Token: 0x04004DA2 RID: 19874
		private Vertex nextVertex;

		// Token: 0x04004DA3 RID: 19875
		private float triangleArea;

		// Token: 0x04004DA4 RID: 19876
		private bool triangleHasChanged;
	}
}
