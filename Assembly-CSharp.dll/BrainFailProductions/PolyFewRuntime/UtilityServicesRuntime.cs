using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrainFailProductions.PolyFewRuntime
{
	// Token: 0x02000961 RID: 2401
	public class UtilityServicesRuntime : MonoBehaviour
	{
		// Token: 0x060032CA RID: 13002 RVA: 0x0022B550 File Offset: 0x00229750
		public static Texture2D DuplicateTexture(Texture2D source)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, 7, 1);
			Graphics.Blit(source, temporary);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(source.width, source.height);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return texture2D;
		}

		// Token: 0x060032CB RID: 13003 RVA: 0x0022B5D0 File Offset: 0x002297D0
		public static Renderer[] GetChildRenderersForCombining(GameObject forObject, bool skipInactiveChildObjects)
		{
			List<Renderer> list = new List<Renderer>();
			if (skipInactiveChildObjects && !forObject.gameObject.activeSelf)
			{
				Debug.LogWarning("No Renderers under the GameObject \"" + forObject.name + "\" combined because the object was inactive and was skipped entirely.");
				return null;
			}
			if (forObject.GetComponent<LODGroup>() != null)
			{
				Debug.LogWarning("No Renderers under the GameObject \"" + forObject.name + "\" combined because the object had LOD groups and was skipped entirely.");
				return null;
			}
			UtilityServicesRuntime.CollectChildRenderersForCombining(forObject.transform, list, skipInactiveChildObjects);
			return list.ToArray();
		}

		// Token: 0x060032CC RID: 13004 RVA: 0x0022B64C File Offset: 0x0022984C
		public static MeshRenderer CreateStaticLevelRenderer(string name, Transform parentTransform, Transform originalTransform, Mesh mesh, Material[] materials)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(MeshFilter),
				typeof(MeshRenderer)
			});
			Transform transform = gameObject.transform;
			if (originalTransform != null)
			{
				UtilityServicesRuntime.ParentAndOffsetTransform(transform, parentTransform, originalTransform);
			}
			else
			{
				UtilityServicesRuntime.ParentAndResetTransform(transform, parentTransform);
			}
			gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.sharedMaterials = materials;
			return component;
		}

		// Token: 0x060032CD RID: 13005 RVA: 0x0022B6BC File Offset: 0x002298BC
		public static SkinnedMeshRenderer CreateSkinnedLevelRenderer(string name, Transform parentTransform, Transform originalTransform, Mesh mesh, Material[] materials, Transform rootBone, Transform[] bones)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(SkinnedMeshRenderer)
			});
			Transform transform = gameObject.transform;
			if (originalTransform != null)
			{
				UtilityServicesRuntime.ParentAndOffsetTransform(transform, parentTransform, originalTransform);
			}
			else
			{
				UtilityServicesRuntime.ParentAndResetTransform(transform, parentTransform);
			}
			SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
			component.sharedMesh = mesh;
			component.sharedMaterials = materials;
			component.rootBone = rootBone;
			component.bones = bones;
			return component;
		}

		// Token: 0x060032CE RID: 13006 RVA: 0x0022B728 File Offset: 0x00229928
		private static void CollectChildRenderersForCombining(Transform transform, List<Renderer> resultRenderers, bool skipInactiveChildObjects)
		{
			Renderer[] components = transform.GetComponents<Renderer>();
			resultRenderers.AddRange(components);
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (skipInactiveChildObjects && !child.gameObject.activeSelf)
				{
					Debug.LogWarning("No Renderers under the GameObject \"" + transform.name + "\" combined because the object was inactive and was skipped entirely.");
				}
				else if (child.GetComponent<LODGroup>() != null)
				{
					Debug.LogWarning("No Renderers under the GameObject \"" + transform.name + "\" combined because the object had LOD groups and was skipped entirely.");
				}
				else
				{
					UtilityServicesRuntime.CollectChildRenderersForCombining(child, resultRenderers, skipInactiveChildObjects);
				}
			}
		}

		// Token: 0x060032CF RID: 13007 RVA: 0x002258E0 File Offset: 0x00223AE0
		private static void ParentAndResetTransform(Transform transform, Transform parentTransform)
		{
			transform.SetParent(parentTransform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		// Token: 0x060032D0 RID: 13008 RVA: 0x0022590A File Offset: 0x00223B0A
		public static void ParentAndOffsetTransform(Transform transform, Transform parentTransform, Transform originalTransform)
		{
			transform.position = originalTransform.position;
			transform.rotation = originalTransform.rotation;
			transform.localScale = originalTransform.lossyScale;
			transform.SetParent(parentTransform, true);
		}

		// Token: 0x02000962 RID: 2402
		public class OBJExporterImporter
		{
			// Token: 0x060032D3 RID: 13011 RVA: 0x0022B7E8 File Offset: 0x002299E8
			private void InitializeExporter(GameObject toExport, string exportPath, PolyfewRuntime.OBJExportOptions exportOptions)
			{
				this.exportPath = exportPath;
				if (string.IsNullOrWhiteSpace(exportPath))
				{
					throw new DirectoryNotFoundException("The path provided is non-existant.");
				}
				exportPath = Path.GetFullPath(exportPath);
				if (exportPath.get_Chars(exportPath.Length - 1) == '\\')
				{
					exportPath = exportPath.Remove(exportPath.Length - 1);
				}
				else if (exportPath.get_Chars(exportPath.Length - 1) == '/')
				{
					exportPath = exportPath.Remove(exportPath.Length - 1);
				}
				if (!Directory.Exists(exportPath))
				{
					throw new DirectoryNotFoundException("The path provided is non-existant.");
				}
				if (toExport == null)
				{
					throw new ArgumentNullException("toExport", "Please provide a GameObject to export as OBJ file.");
				}
				this.meshRenderer = toExport.GetComponent<MeshRenderer>();
				this.meshFilter = toExport.GetComponent<MeshFilter>();
				if (!(this.meshRenderer == null) && this.meshRenderer.isPartOfStaticBatch)
				{
					throw new InvalidOperationException("The provided object is static batched. Static batched object cannot be exported. Please disable it before trying to export the object.");
				}
				if (this.meshFilter == null)
				{
					throw new InvalidOperationException("There is no MeshFilter attached to the provided GameObject.");
				}
				this.meshToExport = this.meshFilter.sharedMesh;
				if (this.meshToExport == null || this.meshToExport.triangles == null || this.meshToExport.triangles.Length == 0)
				{
					throw new InvalidOperationException("The MeshFilter on the provided GameObject has invalid or no mesh at all.");
				}
				if (exportOptions != null)
				{
					this.applyPosition = exportOptions.applyPosition;
					this.applyRotation = exportOptions.applyRotation;
					this.applyScale = exportOptions.applyScale;
					this.generateMaterials = exportOptions.generateMaterials;
					this.exportTextures = exportOptions.exportTextures;
				}
			}

			// Token: 0x060032D4 RID: 13012 RVA: 0x0022B964 File Offset: 0x00229B64
			private void InitializeExporter(Mesh toExport, string exportPath)
			{
				this.exportPath = exportPath;
				if (string.IsNullOrWhiteSpace(exportPath))
				{
					throw new DirectoryNotFoundException("The path provided is non-existant.");
				}
				if (!Directory.Exists(exportPath))
				{
					throw new DirectoryNotFoundException("The path provided is non-existant.");
				}
				if (toExport == null)
				{
					throw new ArgumentNullException("toExport", "Please provide a Mesh to export as OBJ file.");
				}
				this.meshToExport = toExport;
				if (this.meshToExport == null || this.meshToExport.triangles == null || this.meshToExport.triangles.Length == 0)
				{
					throw new InvalidOperationException("The MeshFilter on the provided GameObject has invalid or no mesh at all.");
				}
			}

			// Token: 0x060032D5 RID: 13013 RVA: 0x0022B9F2 File Offset: 0x00229BF2
			private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
			{
				return angle * (point - pivot) + pivot;
			}

			// Token: 0x060032D6 RID: 13014 RVA: 0x0022BA07 File Offset: 0x00229C07
			private Vector3 MultiplyVec3s(Vector3 v1, Vector3 v2)
			{
				return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
			}

			// Token: 0x060032D7 RID: 13015 RVA: 0x0022BA38 File Offset: 0x00229C38
			public void ExportGameObjectToOBJ(GameObject toExport, string exportPath, PolyfewRuntime.OBJExportOptions exportOptions = null, Action OnSuccess = null)
			{
				if (Application.platform == 17)
				{
					Debug.LogWarning("The function cannot run on WebGL player. As web apps cannot read from or write to local file system.");
					return;
				}
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				this.InitializeExporter(toExport, exportPath, exportOptions);
				string name = toExport.gameObject.name;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (this.generateMaterials)
				{
					stringBuilder.AppendLine("mtllib " + name + ".mtl");
				}
				int num = 0;
				if (this.meshRenderer != null && this.generateMaterials)
				{
					foreach (Material material in this.meshRenderer.sharedMaterials)
					{
						if (!dictionary.ContainsKey(material.name))
						{
							dictionary[material.name] = true;
							stringBuilder2.Append(this.MaterialToString(material));
							stringBuilder2.AppendLine();
						}
					}
				}
				int num2 = (int)Mathf.Clamp(toExport.gameObject.transform.lossyScale.x * toExport.gameObject.transform.lossyScale.z, -1f, 1f);
				foreach (Vector3 vector in this.meshToExport.vertices)
				{
					if (this.applyScale)
					{
						vector = this.MultiplyVec3s(vector, toExport.gameObject.transform.lossyScale);
					}
					if (this.applyRotation)
					{
						vector = this.RotateAroundPoint(vector, Vector3.zero, toExport.gameObject.transform.rotation);
					}
					if (this.applyPosition)
					{
						vector += toExport.gameObject.transform.position;
					}
					vector.x *= -1f;
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"v ",
						vector.x.ToString(),
						" ",
						vector.y.ToString(),
						" ",
						vector.z.ToString()
					}));
				}
				foreach (Vector3 vector2 in this.meshToExport.normals)
				{
					if (this.applyScale)
					{
						vector2 = this.MultiplyVec3s(vector2, toExport.gameObject.transform.lossyScale.normalized);
					}
					if (this.applyRotation)
					{
						vector2 = this.RotateAroundPoint(vector2, Vector3.zero, toExport.gameObject.transform.rotation);
					}
					vector2.x *= -1f;
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"vn ",
						vector2.x.ToString(),
						" ",
						vector2.y.ToString(),
						" ",
						vector2.z.ToString()
					}));
				}
				foreach (Vector2 vector3 in this.meshToExport.uv)
				{
					StringBuilder stringBuilder3 = stringBuilder;
					string text = "vt ";
					float num3 = vector3.x;
					string text2 = num3.ToString();
					string text3 = " ";
					num3 = vector3.y;
					stringBuilder3.AppendLine(text + text2 + text3 + num3.ToString());
				}
				for (int k = 0; k < this.meshToExport.subMeshCount; k++)
				{
					if (this.meshRenderer != null && k < this.meshRenderer.sharedMaterials.Length)
					{
						string name2 = this.meshRenderer.sharedMaterials[k].name;
						stringBuilder.AppendLine("usemtl " + name2);
					}
					else
					{
						stringBuilder.AppendLine("usemtl " + name + "_sm" + k.ToString());
					}
					int[] triangles = this.meshToExport.GetTriangles(k);
					for (int l = 0; l < triangles.Length; l += 3)
					{
						int index = triangles[l] + 1 + num;
						int index2 = triangles[l + 1] + 1 + num;
						int index3 = triangles[l + 2] + 1 + num;
						if (num2 < 0)
						{
							stringBuilder.AppendLine(string.Concat(new string[]
							{
								"f ",
								this.ConstructOBJString(index),
								" ",
								this.ConstructOBJString(index2),
								" ",
								this.ConstructOBJString(index3)
							}));
						}
						else
						{
							stringBuilder.AppendLine(string.Concat(new string[]
							{
								"f ",
								this.ConstructOBJString(index3),
								" ",
								this.ConstructOBJString(index2),
								" ",
								this.ConstructOBJString(index)
							}));
						}
					}
				}
				num += this.meshToExport.vertices.Length;
				File.WriteAllText(Path.Combine(exportPath, name + ".obj"), stringBuilder.ToString());
				if (this.generateMaterials)
				{
					File.WriteAllText(Path.Combine(exportPath, name + ".mtl"), stringBuilder2.ToString());
				}
				if (OnSuccess != null)
				{
					OnSuccess.Invoke();
				}
			}

			// Token: 0x060032D8 RID: 13016 RVA: 0x0022BF70 File Offset: 0x0022A170
			public Task ExportMeshToOBJ(Mesh mesh, string exportPath)
			{
				UtilityServicesRuntime.OBJExporterImporter.<ExportMeshToOBJ>d__15 <ExportMeshToOBJ>d__;
				<ExportMeshToOBJ>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
				<ExportMeshToOBJ>d__.<>4__this = this;
				<ExportMeshToOBJ>d__.mesh = mesh;
				<ExportMeshToOBJ>d__.exportPath = exportPath;
				<ExportMeshToOBJ>d__.<>1__state = -1;
				<ExportMeshToOBJ>d__.<>t__builder.Start<UtilityServicesRuntime.OBJExporterImporter.<ExportMeshToOBJ>d__15>(ref <ExportMeshToOBJ>d__);
				return <ExportMeshToOBJ>d__.<>t__builder.Task;
			}

			// Token: 0x060032D9 RID: 13017 RVA: 0x0022BFC4 File Offset: 0x0022A1C4
			private string TryExportTexture(string propertyName, Material m, string exportPath)
			{
				if (m.HasProperty(propertyName))
				{
					Texture texture = m.GetTexture(propertyName);
					if (texture != null)
					{
						return this.ExportTexture((Texture2D)texture, exportPath);
					}
				}
				return "false";
			}

			// Token: 0x060032DA RID: 13018 RVA: 0x0022C000 File Offset: 0x0022A200
			private string ExportTexture(Texture2D t, string exportPath)
			{
				string name = t.name;
				string result;
				try
				{
					Color32[] pixels = null;
					try
					{
						pixels = t.GetPixels32();
					}
					catch (UnityException)
					{
						t = UtilityServicesRuntime.DuplicateTexture(t);
						pixels = t.GetPixels32();
					}
					string text = Path.Combine(exportPath, name + ".png");
					Texture2D texture2D = new Texture2D(t.width, t.height, 5, false);
					texture2D.SetPixels32(pixels);
					File.WriteAllBytes(text, ImageConversion.EncodeToPNG(texture2D));
					result = text;
				}
				catch (Exception)
				{
					Debug.Log("Could not export texture : " + t.name + ". is it readable?");
					result = "null";
				}
				return result;
			}

			// Token: 0x060032DB RID: 13019 RVA: 0x0022C0AC File Offset: 0x0022A2AC
			private string ConstructOBJString(int index)
			{
				string text = index.ToString();
				return string.Concat(new string[]
				{
					text,
					"/",
					text,
					"/",
					text
				});
			}

			// Token: 0x060032DC RID: 13020 RVA: 0x0022C0E8 File Offset: 0x0022A2E8
			private string MaterialToString(Material m)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("newmtl " + m.name);
				if (m.HasProperty("_Color"))
				{
					StringBuilder stringBuilder2 = stringBuilder;
					string[] array = new string[6];
					array[0] = "Kd ";
					int num = 1;
					Color color = m.color;
					array[num] = color.r.ToString();
					array[2] = " ";
					int num2 = 3;
					color = m.color;
					array[num2] = color.g.ToString();
					array[4] = " ";
					int num3 = 5;
					color = m.color;
					array[num3] = color.b.ToString();
					stringBuilder2.AppendLine(string.Concat(array));
					if (m.color.a < 1f)
					{
						stringBuilder.AppendLine("Tr " + (1f - m.color.a).ToString());
						StringBuilder stringBuilder3 = stringBuilder;
						string text = "d ";
						color = m.color;
						stringBuilder3.AppendLine(text + color.a.ToString());
					}
				}
				if (m.HasProperty("_SpecColor"))
				{
					Color color2 = m.GetColor("_SpecColor");
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"Ks ",
						color2.r.ToString(),
						" ",
						color2.g.ToString(),
						" ",
						color2.b.ToString()
					}));
				}
				if (this.exportTextures)
				{
					string text2 = this.TryExportTexture("_MainTex", m, this.exportPath);
					if (text2 != "false")
					{
						stringBuilder.AppendLine("map_Kd " + text2);
					}
					text2 = this.TryExportTexture("_SpecMap", m, this.exportPath);
					if (text2 != "false")
					{
						stringBuilder.AppendLine("map_Ks " + text2);
					}
					text2 = this.TryExportTexture("_BumpMap", m, this.exportPath);
					if (text2 != "false")
					{
						stringBuilder.AppendLine("map_Bump " + text2);
					}
				}
				stringBuilder.AppendLine("illum 2");
				return stringBuilder.ToString();
			}

			// Token: 0x060032DD RID: 13021 RVA: 0x0022C31C File Offset: 0x0022A51C
			public Task ImportFromLocalFileSystem(string objPath, string texturesFolderPath, string materialsFolderPath, Action<GameObject> Callback, PolyfewRuntime.OBJImportOptions importOptions = null)
			{
				UtilityServicesRuntime.OBJExporterImporter.<ImportFromLocalFileSystem>d__20 <ImportFromLocalFileSystem>d__;
				<ImportFromLocalFileSystem>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
				<ImportFromLocalFileSystem>d__.objPath = objPath;
				<ImportFromLocalFileSystem>d__.texturesFolderPath = texturesFolderPath;
				<ImportFromLocalFileSystem>d__.materialsFolderPath = materialsFolderPath;
				<ImportFromLocalFileSystem>d__.Callback = Callback;
				<ImportFromLocalFileSystem>d__.importOptions = importOptions;
				<ImportFromLocalFileSystem>d__.<>1__state = -1;
				<ImportFromLocalFileSystem>d__.<>t__builder.Start<UtilityServicesRuntime.OBJExporterImporter.<ImportFromLocalFileSystem>d__20>(ref <ImportFromLocalFileSystem>d__);
				return <ImportFromLocalFileSystem>d__.<>t__builder.Task;
			}

			// Token: 0x060032DE RID: 13022 RVA: 0x0022C384 File Offset: 0x0022A584
			public void ImportFromNetwork(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<GameObject> OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJImportOptions importOptions = null)
			{
				UtilityServicesRuntime.OBJExporterImporter.<ImportFromNetwork>d__21 <ImportFromNetwork>d__;
				<ImportFromNetwork>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
				<ImportFromNetwork>d__.objURL = objURL;
				<ImportFromNetwork>d__.objName = objName;
				<ImportFromNetwork>d__.diffuseTexURL = diffuseTexURL;
				<ImportFromNetwork>d__.bumpTexURL = bumpTexURL;
				<ImportFromNetwork>d__.specularTexURL = specularTexURL;
				<ImportFromNetwork>d__.opacityTexURL = opacityTexURL;
				<ImportFromNetwork>d__.materialURL = materialURL;
				<ImportFromNetwork>d__.downloadProgress = downloadProgress;
				<ImportFromNetwork>d__.OnSuccess = OnSuccess;
				<ImportFromNetwork>d__.OnError = OnError;
				<ImportFromNetwork>d__.importOptions = importOptions;
				<ImportFromNetwork>d__.<>1__state = -1;
				<ImportFromNetwork>d__.<>t__builder.Start<UtilityServicesRuntime.OBJExporterImporter.<ImportFromNetwork>d__21>(ref <ImportFromNetwork>d__);
			}

			// Token: 0x060032DF RID: 13023 RVA: 0x0022C414 File Offset: 0x0022A614
			public void ImportFromNetworkWebGL(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<GameObject> OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJImportOptions importOptions = null)
			{
				UtilityServicesRuntime.OBJExporterImporter.<ImportFromNetworkWebGL>d__22 <ImportFromNetworkWebGL>d__;
				<ImportFromNetworkWebGL>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
				<ImportFromNetworkWebGL>d__.objURL = objURL;
				<ImportFromNetworkWebGL>d__.objName = objName;
				<ImportFromNetworkWebGL>d__.diffuseTexURL = diffuseTexURL;
				<ImportFromNetworkWebGL>d__.bumpTexURL = bumpTexURL;
				<ImportFromNetworkWebGL>d__.specularTexURL = specularTexURL;
				<ImportFromNetworkWebGL>d__.opacityTexURL = opacityTexURL;
				<ImportFromNetworkWebGL>d__.materialURL = materialURL;
				<ImportFromNetworkWebGL>d__.downloadProgress = downloadProgress;
				<ImportFromNetworkWebGL>d__.OnSuccess = OnSuccess;
				<ImportFromNetworkWebGL>d__.OnError = OnError;
				<ImportFromNetworkWebGL>d__.importOptions = importOptions;
				<ImportFromNetworkWebGL>d__.<>1__state = -1;
				<ImportFromNetworkWebGL>d__.<>t__builder.Start<UtilityServicesRuntime.OBJExporterImporter.<ImportFromNetworkWebGL>d__22>(ref <ImportFromNetworkWebGL>d__);
			}

			// Token: 0x04004EB0 RID: 20144
			private bool applyPosition = true;

			// Token: 0x04004EB1 RID: 20145
			private bool applyRotation = true;

			// Token: 0x04004EB2 RID: 20146
			private bool applyScale = true;

			// Token: 0x04004EB3 RID: 20147
			private bool generateMaterials = true;

			// Token: 0x04004EB4 RID: 20148
			private bool exportTextures = true;

			// Token: 0x04004EB5 RID: 20149
			private string exportPath;

			// Token: 0x04004EB6 RID: 20150
			private MeshFilter meshFilter;

			// Token: 0x04004EB7 RID: 20151
			private Mesh meshToExport;

			// Token: 0x04004EB8 RID: 20152
			private MeshRenderer meshRenderer;
		}
	}
}
