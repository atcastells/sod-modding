using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x020008FC RID: 2300
	public class DataSet
	{
		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06003104 RID: 12548 RVA: 0x00219F46 File Offset: 0x00218146
		public string CurrGroupName
		{
			get
			{
				if (this.currGroup == null)
				{
					return "";
				}
				return this.currGroup.name;
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06003105 RID: 12549 RVA: 0x00219F61 File Offset: 0x00218161
		public bool IsEmpty
		{
			get
			{
				return this.vertList.Count == 0;
			}
		}

		// Token: 0x06003106 RID: 12550 RVA: 0x00219F74 File Offset: 0x00218174
		public static string GetFaceIndicesKey(DataSet.FaceIndices fi)
		{
			return string.Concat(new string[]
			{
				fi.vertIdx.ToString(),
				"/",
				fi.uvIdx.ToString(),
				"/",
				fi.normIdx.ToString()
			});
		}

		// Token: 0x06003107 RID: 12551 RVA: 0x00219FCC File Offset: 0x002181CC
		public static string FixMaterialName(string mtlName)
		{
			return mtlName.Replace(':', '_').Replace('\\', '_').Replace('/', '_').Replace('*', '_').Replace('?', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_');
		}

		// Token: 0x06003108 RID: 12552 RVA: 0x0021A024 File Offset: 0x00218224
		public DataSet()
		{
			DataSet.ObjectData objectData = new DataSet.ObjectData();
			objectData.name = "default";
			this.objectList.Add(objectData);
			this.currObjData = objectData;
			DataSet.FaceGroupData faceGroupData = new DataSet.FaceGroupData();
			faceGroupData.name = "default";
			objectData.faceGroups.Add(faceGroupData);
			this.currGroup = faceGroupData;
		}

		// Token: 0x06003109 RID: 12553 RVA: 0x0021A0C4 File Offset: 0x002182C4
		public void AddObject(string objectName)
		{
			string materialName = this.currObjData.faceGroups[this.currObjData.faceGroups.Count - 1].materialName;
			if (this.noFaceDefined)
			{
				this.objectList.Remove(this.currObjData);
			}
			DataSet.ObjectData objectData = new DataSet.ObjectData();
			objectData.name = objectName;
			this.objectList.Add(objectData);
			DataSet.FaceGroupData faceGroupData = new DataSet.FaceGroupData();
			faceGroupData.materialName = materialName;
			faceGroupData.name = "default";
			objectData.faceGroups.Add(faceGroupData);
			this.currGroup = faceGroupData;
			this.currObjData = objectData;
		}

		// Token: 0x0600310A RID: 12554 RVA: 0x0021A160 File Offset: 0x00218360
		public void AddGroup(string groupName)
		{
			string materialName = this.currObjData.faceGroups[this.currObjData.faceGroups.Count - 1].materialName;
			if (this.currGroup.IsEmpty)
			{
				this.currObjData.faceGroups.Remove(this.currGroup);
			}
			DataSet.FaceGroupData faceGroupData = new DataSet.FaceGroupData();
			faceGroupData.materialName = materialName;
			if (groupName == null)
			{
				groupName = "Unnamed-" + this.unnamedGroupIndex.ToString();
				this.unnamedGroupIndex++;
			}
			faceGroupData.name = groupName;
			this.currObjData.faceGroups.Add(faceGroupData);
			this.currGroup = faceGroupData;
		}

		// Token: 0x0600310B RID: 12555 RVA: 0x0021A210 File Offset: 0x00218410
		public void AddMaterialName(string matName)
		{
			if (!this.currGroup.IsEmpty)
			{
				this.AddGroup(matName);
			}
			if (this.currGroup.name == "default")
			{
				this.currGroup.name = matName;
			}
			this.currGroup.materialName = matName;
		}

		// Token: 0x0600310C RID: 12556 RVA: 0x0021A260 File Offset: 0x00218460
		public void AddVertex(Vector3 vertex)
		{
			this.vertList.Add(vertex);
		}

		// Token: 0x0600310D RID: 12557 RVA: 0x0021A26E File Offset: 0x0021846E
		public void AddUV(Vector2 uv)
		{
			this.uvList.Add(uv);
		}

		// Token: 0x0600310E RID: 12558 RVA: 0x0021A27C File Offset: 0x0021847C
		public void AddNormal(Vector3 normal)
		{
			this.normalList.Add(normal);
		}

		// Token: 0x0600310F RID: 12559 RVA: 0x0021A28A File Offset: 0x0021848A
		public void AddColor(Color color)
		{
			this.colorList.Add(color);
			this.currObjData.hasColors = true;
		}

		// Token: 0x06003110 RID: 12560 RVA: 0x0021A2A4 File Offset: 0x002184A4
		public void AddFaceIndices(DataSet.FaceIndices faceIdx)
		{
			this.noFaceDefined = false;
			this.currGroup.faces.Add(faceIdx);
			this.currObjData.allFaces.Add(faceIdx);
			if (faceIdx.normIdx >= 0)
			{
				this.currObjData.hasNormals = true;
			}
		}

		// Token: 0x06003111 RID: 12561 RVA: 0x0021A2E4 File Offset: 0x002184E4
		public void PrintSummary()
		{
			string text = string.Concat(new string[]
			{
				"This data set has ",
				this.objectList.Count.ToString(),
				" object(s)\n  ",
				this.vertList.Count.ToString(),
				" vertices\n  ",
				this.uvList.Count.ToString(),
				" uvs\n  ",
				this.normalList.Count.ToString(),
				" normals"
			});
			foreach (DataSet.ObjectData objectData in this.objectList)
			{
				text = string.Concat(new string[]
				{
					text,
					"\n  ",
					objectData.name,
					" has ",
					objectData.faceGroups.Count.ToString(),
					" group(s)"
				});
				foreach (DataSet.FaceGroupData faceGroupData in objectData.faceGroups)
				{
					text = string.Concat(new string[]
					{
						text,
						"\n    ",
						faceGroupData.name,
						" has ",
						faceGroupData.faces.Count.ToString(),
						" faces(s)"
					});
				}
			}
			Debug.Log(text);
		}

		// Token: 0x04004C10 RID: 19472
		public List<DataSet.ObjectData> objectList = new List<DataSet.ObjectData>();

		// Token: 0x04004C11 RID: 19473
		public List<Vector3> vertList = new List<Vector3>();

		// Token: 0x04004C12 RID: 19474
		public List<Vector2> uvList = new List<Vector2>();

		// Token: 0x04004C13 RID: 19475
		public List<Vector3> normalList = new List<Vector3>();

		// Token: 0x04004C14 RID: 19476
		public List<Color> colorList = new List<Color>();

		// Token: 0x04004C15 RID: 19477
		private int unnamedGroupIndex = 1;

		// Token: 0x04004C16 RID: 19478
		private DataSet.ObjectData currObjData;

		// Token: 0x04004C17 RID: 19479
		private DataSet.FaceGroupData currGroup;

		// Token: 0x04004C18 RID: 19480
		private bool noFaceDefined = true;

		// Token: 0x020008FD RID: 2301
		public struct FaceIndices
		{
			// Token: 0x04004C19 RID: 19481
			public int vertIdx;

			// Token: 0x04004C1A RID: 19482
			public int uvIdx;

			// Token: 0x04004C1B RID: 19483
			public int normIdx;
		}

		// Token: 0x020008FE RID: 2302
		public class ObjectData
		{
			// Token: 0x04004C1C RID: 19484
			public string name;

			// Token: 0x04004C1D RID: 19485
			public List<DataSet.FaceGroupData> faceGroups = new List<DataSet.FaceGroupData>();

			// Token: 0x04004C1E RID: 19486
			public List<DataSet.FaceIndices> allFaces = new List<DataSet.FaceIndices>();

			// Token: 0x04004C1F RID: 19487
			public bool hasNormals;

			// Token: 0x04004C20 RID: 19488
			public bool hasColors;
		}

		// Token: 0x020008FF RID: 2303
		public class FaceGroupData
		{
			// Token: 0x06003113 RID: 12563 RVA: 0x0021A4B6 File Offset: 0x002186B6
			public FaceGroupData()
			{
				this.faces = new List<DataSet.FaceIndices>();
			}

			// Token: 0x17000521 RID: 1313
			// (get) Token: 0x06003114 RID: 12564 RVA: 0x0021A4C9 File Offset: 0x002186C9
			public bool IsEmpty
			{
				get
				{
					return this.faces.Count == 0;
				}
			}

			// Token: 0x04004C21 RID: 19489
			public string name;

			// Token: 0x04004C22 RID: 19490
			public string materialName;

			// Token: 0x04004C23 RID: 19491
			public List<DataSet.FaceIndices> faces;
		}
	}
}
