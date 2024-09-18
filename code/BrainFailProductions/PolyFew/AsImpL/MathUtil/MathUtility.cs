using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL.MathUtil
{
	// Token: 0x02000936 RID: 2358
	public static class MathUtility
	{
		// Token: 0x0600320F RID: 12815 RVA: 0x0022323C File Offset: 0x0022143C
		public static int ClampListIndex(int index, int listSize)
		{
			index = (index % listSize + listSize) % listSize;
			return index;
		}

		// Token: 0x06003210 RID: 12816 RVA: 0x00223248 File Offset: 0x00221448
		public static bool IsPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p)
		{
			bool result = false;
			float num = (p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y);
			float num2 = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / num;
			float num3 = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / num;
			float num4 = 1f - num2 - num3;
			if (num2 > 0f && num2 < 1f && num3 > 0f && num3 < 1f && num4 > 0f && num4 < 1f)
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06003211 RID: 12817 RVA: 0x00223344 File Offset: 0x00221544
		public static bool IsTriangleOrientedClockwise(Vector2 v1, Vector2 v2, Vector2 v3)
		{
			return v1.x * v2.y + v3.x * v1.y + v2.x * v3.y - v1.x * v3.y - v3.x * v2.y - v2.x * v1.y > 0f;
		}

		// Token: 0x06003212 RID: 12818 RVA: 0x002233AC File Offset: 0x002215AC
		public static Vector3 ComputeNormal(Vector3 vert, Vector3 vNext, Vector3 vPrev)
		{
			Vector3 result = Vector3.Cross(vPrev - vert, vNext - vert);
			result.Normalize();
			return result;
		}
	}
}
