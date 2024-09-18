using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL.MathUtil
{
	// Token: 0x02000937 RID: 2359
	public static class Triangulation
	{
		// Token: 0x06003213 RID: 12819 RVA: 0x002233D8 File Offset: 0x002215D8
		public static List<Triangle> TriangulateConvexPolygon(List<Vertex> vertices, bool preserveOriginalVertices = true)
		{
			List<Vertex> list = preserveOriginalVertices ? new List<Vertex>(vertices) : vertices;
			List<Triangle> list2 = new List<Triangle>();
			while (list.Count != 3)
			{
				Vertex vertex = Triangulation.FindMaxAreaEarVertex(list);
				list2.Add(Triangulation.ClipTriangle(vertex, list));
			}
			list2.Add(new Triangle(list[0], list[1], list[2]));
			return list2;
		}

		// Token: 0x06003214 RID: 12820 RVA: 0x00223438 File Offset: 0x00221638
		public static List<Triangle> TriangulateByEarClipping(List<Vertex> origVertices, Vector3 planeNormal, string meshName, bool preserveOriginalVertices = true)
		{
			List<Triangle> list = new List<Triangle>();
			List<Vertex> list2 = preserveOriginalVertices ? new List<Vertex>(origVertices) : origVertices;
			for (int i = 0; i < list2.Count; i++)
			{
				int num = MathUtility.ClampListIndex(i + 1, list2.Count);
				int num2 = MathUtility.ClampListIndex(i - 1, list2.Count);
				list2[i].PreviousVertex = list2[num2];
				list2[i].NextVertex = list2[num];
			}
			List<Vertex> list3 = Triangulation.FindEarVertices(list2, planeNormal);
			while (list2.Count != 3)
			{
				if (list3.Count == 0)
				{
					planeNormal = -planeNormal;
					list3 = Triangulation.FindEarVertices(list2, planeNormal);
				}
				if (list3.Count == 0)
				{
					Debug.LogWarningFormat("Cannot find a proper reprojection for mesh '{0}'. Using fallback polygon triangulation.", new object[]
					{
						meshName
					});
					Triangle triangle = Triangulation.ClipTriangle(list2[0], list2);
					list.Add(triangle);
				}
				else
				{
					Triangle triangle2 = Triangulation.ClipEar(Triangulation.FindMaxAreaEarVertex(list3), list3, list2, planeNormal);
					list.Add(triangle2);
				}
			}
			list.Add(new Triangle(list2[0], list2[1], list2[2]));
			return list;
		}

		// Token: 0x06003215 RID: 12821 RVA: 0x00223550 File Offset: 0x00221750
		public static Triangle ClipTriangle(Vertex vertex, List<Vertex> vertices)
		{
			Vertex previousVertex = vertex.PreviousVertex;
			Vertex nextVertex = vertex.NextVertex;
			Triangle result = new Triangle(previousVertex, vertex, nextVertex);
			vertices.Remove(vertex);
			previousVertex.NextVertex = nextVertex;
			nextVertex.PreviousVertex = previousVertex;
			return result;
		}

		// Token: 0x06003216 RID: 12822 RVA: 0x0022358C File Offset: 0x0022178C
		private static Triangle ClipEar(Vertex earVertex, List<Vertex> earVertices, List<Vertex> vertices, Vector3 planeNormal)
		{
			Vertex previousVertex = earVertex.PreviousVertex;
			Vertex nextVertex = earVertex.NextVertex;
			Triangle result = Triangulation.ClipTriangle(earVertex, vertices);
			earVertices.Remove(earVertex);
			if (Triangulation.IsVertexEar(previousVertex, vertices, planeNormal))
			{
				earVertices.Add(previousVertex);
			}
			else
			{
				earVertices.Remove(previousVertex);
			}
			if (Triangulation.IsVertexEar(nextVertex, vertices, planeNormal))
			{
				earVertices.Add(nextVertex);
				return result;
			}
			earVertices.Remove(nextVertex);
			return result;
		}

		// Token: 0x06003217 RID: 12823 RVA: 0x002235EC File Offset: 0x002217EC
		private static Vertex FindMaxAreaEarVertex(List<Vertex> earVertices)
		{
			Vertex vertex = earVertices[0];
			foreach (Vertex vertex2 in earVertices)
			{
				if (vertex2.TriangleArea < vertex.TriangleArea)
				{
					vertex = vertex2;
				}
			}
			return vertex;
		}

		// Token: 0x06003218 RID: 12824 RVA: 0x0022364C File Offset: 0x0022184C
		private static List<Vertex> FindEarVertices(List<Vertex> vertices, Vector3 planeNormal)
		{
			List<Vertex> list = new List<Vertex>();
			for (int i = 0; i < vertices.Count; i++)
			{
				if (Triangulation.IsVertexEar(vertices[i], vertices, planeNormal))
				{
					list.Add(vertices[i]);
				}
			}
			return list;
		}

		// Token: 0x06003219 RID: 12825 RVA: 0x00223690 File Offset: 0x00221890
		private static bool IsVertexReflex(Vertex v, Vector3 vNormal)
		{
			Vector2 posOnPlane = v.PreviousVertex.GetPosOnPlane(vNormal);
			Vector2 posOnPlane2 = v.GetPosOnPlane(vNormal);
			Vector2 posOnPlane3 = v.NextVertex.GetPosOnPlane(vNormal);
			return !MathUtility.IsTriangleOrientedClockwise(posOnPlane, posOnPlane2, posOnPlane3);
		}

		// Token: 0x0600321A RID: 12826 RVA: 0x002236C8 File Offset: 0x002218C8
		private static bool IsVertexEar(Vertex v, List<Vertex> vertices, Vector3 planeNormal)
		{
			if (Triangulation.IsVertexReflex(v, planeNormal))
			{
				return false;
			}
			Vector2 posOnPlane = v.PreviousVertex.GetPosOnPlane(planeNormal);
			Vector2 posOnPlane2 = v.GetPosOnPlane(planeNormal);
			Vector2 posOnPlane3 = v.NextVertex.GetPosOnPlane(planeNormal);
			bool flag = false;
			for (int i = 0; i < vertices.Count; i++)
			{
				if (v != vertices[i] && v.PreviousVertex != vertices[i] && v.NextVertex != vertices[i] && Triangulation.IsVertexReflex(vertices[i], planeNormal))
				{
					Vector2 posOnPlane4 = vertices[i].GetPosOnPlane(planeNormal);
					if (MathUtility.IsPointInTriangle(posOnPlane, posOnPlane2, posOnPlane3, posOnPlane4))
					{
						flag = true;
						break;
					}
				}
			}
			return !flag;
		}
	}
}
