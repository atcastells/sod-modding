using System;
using System.Collections.Generic;
using BrainFailProductions.PolyFew.AsImpL.MathUtil;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x02000906 RID: 2310
	public static class Triangulator
	{
		// Token: 0x0600312C RID: 12588 RVA: 0x0021C0A0 File Offset: 0x0021A2A0
		public static void Triangulate(DataSet dataSet, DataSet.FaceIndices[] face)
		{
			int num = face.Length;
			Vector3 planeNormal = Triangulator.FindPlaneNormal(dataSet, face);
			List<Vertex> list = new List<Vertex>();
			for (int i = 0; i < num; i++)
			{
				int vertIdx = face[i].vertIdx;
				list.Add(new Vertex(i, dataSet.vertList[vertIdx]));
			}
			List<Triangle> list2 = Triangulation.TriangulateByEarClipping(list, planeNormal, dataSet.CurrGroupName, true);
			for (int j = 0; j < list2.Count; j++)
			{
				int originalIndex = list2[j].v1.OriginalIndex;
				int originalIndex2 = list2[j].v2.OriginalIndex;
				int originalIndex3 = list2[j].v3.OriginalIndex;
				dataSet.AddFaceIndices(face[originalIndex]);
				dataSet.AddFaceIndices(face[originalIndex3]);
				dataSet.AddFaceIndices(face[originalIndex2]);
			}
		}

		// Token: 0x0600312D RID: 12589 RVA: 0x0021C184 File Offset: 0x0021A384
		public static Vector3 FindPlaneNormal(DataSet dataSet, DataSet.FaceIndices[] face)
		{
			int num = face.Length;
			bool flag = dataSet.normalList.Count > 0;
			Vector3 vector = Vector3.zero;
			if (flag)
			{
				for (int i = 0; i < num; i++)
				{
					int normIdx = face[i].normIdx;
					vector += dataSet.normalList[normIdx];
				}
				vector.Normalize();
			}
			else
			{
				Vector3 vert = dataSet.vertList[face[0].vertIdx];
				Vector3 vNext = dataSet.vertList[face[1].vertIdx];
				Vector3 vPrev = dataSet.vertList[face[num - 1].vertIdx];
				vector = MathUtility.ComputeNormal(vert, vNext, vPrev);
			}
			return vector;
		}
	}
}
