using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace BrainFailProductions.PolyFewRuntime
{
	// Token: 0x02000940 RID: 2368
	public static class MeshCombiner
	{
		// Token: 0x0600324A RID: 12874 RVA: 0x00224784 File Offset: 0x00222984
		public static MeshCombiner.StaticRenderer[] GetStaticRenderers(MeshRenderer[] renderers)
		{
			List<MeshCombiner.StaticRenderer> list = new List<MeshCombiner.StaticRenderer>(renderers.Length);
			foreach (MeshRenderer meshRenderer in renderers)
			{
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component == null)
				{
					Debug.LogWarning("A renderer was missing a mesh filter and was ignored.", meshRenderer);
				}
				else
				{
					Mesh sharedMesh = component.sharedMesh;
					if (sharedMesh == null)
					{
						Debug.LogWarning("A renderer was missing a mesh and was ignored.", meshRenderer);
					}
					else
					{
						list.Add(new MeshCombiner.StaticRenderer
						{
							name = meshRenderer.name,
							isNewMesh = false,
							transform = meshRenderer.transform,
							mesh = sharedMesh,
							materials = meshRenderer.sharedMaterials
						});
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600324B RID: 12875 RVA: 0x0022483C File Offset: 0x00222A3C
		public static MeshCombiner.SkinnedRenderer[] GetSkinnedRenderers(SkinnedMeshRenderer[] renderers)
		{
			List<MeshCombiner.SkinnedRenderer> list = new List<MeshCombiner.SkinnedRenderer>(renderers.Length);
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in renderers)
			{
				Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
				if (sharedMesh == null)
				{
					Debug.LogWarning("A renderer was missing a mesh and was ignored.", skinnedMeshRenderer);
				}
				else
				{
					list.Add(new MeshCombiner.SkinnedRenderer
					{
						name = skinnedMeshRenderer.name,
						isNewMesh = false,
						transform = skinnedMeshRenderer.transform,
						mesh = sharedMesh,
						materials = skinnedMeshRenderer.sharedMaterials,
						rootBone = skinnedMeshRenderer.rootBone,
						bones = skinnedMeshRenderer.bones
					});
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600324C RID: 12876 RVA: 0x002248F0 File Offset: 0x00222AF0
		public static MeshCombiner.StaticRenderer[] CombineStaticMeshes(Transform transform, int levelIndex, MeshRenderer[] renderers, bool autoName = true, string combinedBaseName = "")
		{
			if (renderers.Length == 0)
			{
				return null;
			}
			List<MeshCombiner.StaticRenderer> list = new List<MeshCombiner.StaticRenderer>(renderers.Length);
			if (renderers.Length > 1)
			{
				MeshFilter[] meshFilters = Enumerable.ToArray<MeshFilter>(Enumerable.Select<MeshRenderer, MeshFilter>(Enumerable.Where<MeshRenderer>(renderers, (MeshRenderer renderer) => renderer.GetComponent<MeshFilter>() != null && renderer.GetComponent<MeshFilter>().sharedMesh != null), (MeshRenderer renderer) => renderer.GetComponent<MeshFilter>()));
				MeshCombiner.CombineMeshesUnity(transform, meshFilters);
				MeshCombiner.didUseUnityCombine = true;
			}
			Material[] materials;
			Mesh mesh;
			if (MeshCombiner.unityCombinedMeshRenderers == null)
			{
				mesh = MeshCombiner.CombineMeshes(transform, renderers, out materials, null, null);
			}
			else if (MeshCombiner.unityCombinedMeshRenderers.Length == 1)
			{
				materials = Enumerable.ToArray<Material>(MeshCombiner.unityCombinedMeshesMats);
				mesh = MeshCombiner.unityCombinedMeshRenderers[0].GetComponent<MeshFilter>().sharedMesh;
			}
			else if (MeshCombiner.unityCombinedMeshRenderers.Length == 0)
			{
				mesh = MeshCombiner.CombineMeshes(transform, renderers, out materials, null, null);
			}
			else
			{
				mesh = MeshCombiner.CombineMeshes(transform, MeshCombiner.unityCombinedMeshRenderers, out materials, null, null);
			}
			if (MeshCombiner.unityCombinedMeshRenderers != null)
			{
				MeshRenderer[] array = MeshCombiner.unityCombinedMeshRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					Object.DestroyImmediate(array[i].gameObject);
				}
			}
			MeshCombiner.unityCombinedMeshRenderers = null;
			MeshCombiner.unityCombinedMeshesMats = null;
			string text = string.IsNullOrWhiteSpace(combinedBaseName) ? transform.name : combinedBaseName;
			string name = string.Format("{0}_combined_static", text);
			if (autoName && transform != null)
			{
				mesh.name = string.Format("{0}_static{1:00}", transform.name, levelIndex);
			}
			list.Add(new MeshCombiner.StaticRenderer
			{
				name = name,
				isNewMesh = true,
				transform = null,
				mesh = mesh,
				materials = materials
			});
			MeshCombiner.didUseUnityCombine = false;
			return list.ToArray();
		}

		// Token: 0x0600324D RID: 12877 RVA: 0x00224AA0 File Offset: 0x00222CA0
		public static MeshCombiner.SkinnedRenderer[] CombineSkinnedMeshes(Transform transform, int levelIndex, SkinnedMeshRenderer[] renderers, ref SkinnedMeshRenderer[] renderersActuallyCombined, bool autoName = true, string combinedBaseName = "")
		{
			if (renderers.Length == 0)
			{
				return null;
			}
			List<MeshCombiner.SkinnedRenderer> list = new List<MeshCombiner.SkinnedRenderer>(renderers.Length);
			IEnumerable<SkinnedMeshRenderer> enumerable = Enumerable.Where<SkinnedMeshRenderer>(renderers, (SkinnedMeshRenderer renderer) => renderer.sharedMesh == null);
			SkinnedMeshRenderer[] array = Enumerable.ToArray<SkinnedMeshRenderer>(Enumerable.Where<SkinnedMeshRenderer>(renderers, (SkinnedMeshRenderer renderer) => renderer.sharedMesh != null));
			renderersActuallyCombined = array;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in enumerable)
			{
				Debug.LogWarning("A renderer was missing a mesh and was ignored.", skinnedMeshRenderer);
			}
			if (array.Length != 0)
			{
				Material[] materials;
				Transform[] bones;
				Mesh mesh = MeshCombiner.CombineMeshes(transform, array, out materials, out bones);
				string text = string.IsNullOrWhiteSpace(combinedBaseName) ? transform.name : combinedBaseName;
				string name = string.Format("{0}_combined_skinned", text);
				if (autoName)
				{
					mesh.name = string.Format("{0}_skinned{1:00}", transform.name, levelIndex);
				}
				Transform rootBone = MeshCombiner.FindBestRootBone(transform, array);
				list.Add(new MeshCombiner.SkinnedRenderer
				{
					name = name,
					isNewMesh = false,
					transform = null,
					mesh = mesh,
					materials = materials,
					rootBone = rootBone,
					bones = bones
				});
			}
			return list.ToArray();
		}

		// Token: 0x0600324E RID: 12878 RVA: 0x00224C04 File Offset: 0x00222E04
		public static Mesh CombineMeshes(Transform rootTransform, MeshRenderer[] renderers, out Material[] resultMaterials, Dictionary<Transform, Transform> topLevelParents = null, Dictionary<string, MeshCombiner.BlendShapeFrame> blendShapes = null)
		{
			bool flag = false;
			if (rootTransform == null)
			{
				flag = true;
			}
			if (renderers == null)
			{
				throw new ArgumentNullException("renderers");
			}
			Mesh[] array = new Mesh[renderers.Length];
			Matrix4x4[] array2 = new Matrix4x4[renderers.Length];
			Tuple<Matrix4x4, bool>[] array3 = new Tuple<Matrix4x4, bool>[renderers.Length];
			Material[][] array4 = new Material[renderers.Length][];
			for (int i = 0; i < renderers.Length; i++)
			{
				MeshRenderer meshRenderer = renderers[i];
				if (meshRenderer == null)
				{
					throw new ArgumentException(string.Format("The renderer at index {0} is null.", i), "renderers");
				}
				Transform transform = meshRenderer.transform;
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component == null)
				{
					throw new ArgumentException(string.Format("The renderer at index {0} has no mesh filter.", i), "renderers");
				}
				if (component.sharedMesh == null)
				{
					throw new ArgumentException(string.Format("The mesh filter for renderer at index {0} has no mesh.", i), "renderers");
				}
				if (!component.sharedMesh.isReadable)
				{
					throw new ArgumentException(string.Format("The mesh in the mesh filter for renderer at index {0} is not readable.", i), "renderers");
				}
				array[i] = component.sharedMesh;
				if (flag)
				{
					rootTransform = topLevelParents[transform];
				}
				if (MeshCombiner.didUseUnityCombine)
				{
					array2[i] = transform.localToWorldMatrix;
				}
				else
				{
					array2[i] = rootTransform.worldToLocalMatrix * transform.localToWorldMatrix;
				}
				Vector3 lossyScale = transform.transform.lossyScale;
				bool flag2 = Mathf.Approximately(lossyScale.x, lossyScale.y) && Mathf.Approximately(lossyScale.y, lossyScale.z);
				if (!flag2)
				{
					Debug.LogWarning("The GameObject \"" + transform.name + "\" has non uniform scaling applied. This might cause the combined mesh normals to be incorrectly calculated resulting in slight variation in lighting.");
				}
				array3[i] = Tuple.Create<Matrix4x4, bool>(rootTransform.localToWorldMatrix * transform.localToWorldMatrix, !flag2);
				array4[i] = meshRenderer.sharedMaterials;
			}
			return MeshCombiner.CombineMeshes(array, array2, array3, array4, out resultMaterials, blendShapes);
		}

		// Token: 0x0600324F RID: 12879 RVA: 0x00224E00 File Offset: 0x00223000
		public static Mesh CombineMeshes(Transform rootTransform, SkinnedMeshRenderer[] renderers, out Material[] resultMaterials, out Transform[] resultBones)
		{
			if (renderers == null)
			{
				throw new ArgumentNullException("renderers");
			}
			Mesh[] array = new Mesh[renderers.Length];
			Matrix4x4[] array2 = new Matrix4x4[renderers.Length];
			Tuple<Matrix4x4, bool>[] array3 = new Tuple<Matrix4x4, bool>[renderers.Length];
			Material[][] array4 = new Material[renderers.Length][];
			Transform[][] array5 = new Transform[renderers.Length][];
			Dictionary<string, MeshCombiner.BlendShapeFrame> dictionary = new Dictionary<string, MeshCombiner.BlendShapeFrame>();
			int num = 0;
			for (int i = 0; i < renderers.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = renderers[i];
				if (skinnedMeshRenderer == null)
				{
					throw new ArgumentException(string.Format("The renderer at index {0} is null.", i), "renderers");
				}
				if (skinnedMeshRenderer.sharedMesh == null)
				{
					throw new ArgumentException(string.Format("The renderer at index {0} has no mesh.", i), "renderers");
				}
				if (!skinnedMeshRenderer.sharedMesh.isReadable)
				{
					throw new ArgumentException(string.Format("The mesh in the renderer at index {0} is not readable.", i), "renderers");
				}
				Transform transform = skinnedMeshRenderer.transform;
				array[i] = skinnedMeshRenderer.sharedMesh;
				Vector3 lossyScale = transform.transform.lossyScale;
				bool flag = Mathf.Approximately(lossyScale.x, lossyScale.y) && Mathf.Approximately(lossyScale.y, lossyScale.z);
				if (skinnedMeshRenderer.bones == null || skinnedMeshRenderer.bones.Length == 0)
				{
					array2[i] = rootTransform.worldToLocalMatrix * transform.localToWorldMatrix;
					array3[i] = Tuple.Create<Matrix4x4, bool>(rootTransform.localToWorldMatrix * transform.localToWorldMatrix, !flag);
				}
				else
				{
					array2[i] = transform.worldToLocalMatrix * transform.localToWorldMatrix;
					array3[i] = Tuple.Create<Matrix4x4, bool>(transform.worldToLocalMatrix * transform.localToWorldMatrix, !flag);
				}
				if (!flag)
				{
					Debug.LogWarning("The GameObject \"" + transform.name + "\" has non uniform scaling applied. This might cause the combined mesh normals to be incorrectly calculated resulting in slight variation in lighting.");
				}
				array4[i] = skinnedMeshRenderer.sharedMaterials;
				array5[i] = skinnedMeshRenderer.bones;
				for (int j = 0; j < array5[i].Length; j++)
				{
					Transform transform2 = array5[i][j];
					MeshFilter meshFilter = (transform2 == null) ? null : transform2.GetComponent<MeshFilter>();
					if (((meshFilter == null) ? null : meshFilter.sharedMesh) != null)
					{
						Debug.LogWarning("You have a static mesh attached to the bone:\"" + transform2.name + "\". The mesh combination logic will not deal with this properly, since that would require it to modify the original game object hierarchy. You might get erroneous results on mesh combination.");
					}
				}
				Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
				int hashCode = skinnedMeshRenderer.GetHashCode();
				if (sharedMesh.blendShapeCount > 0)
				{
					for (int k = 0; k < sharedMesh.blendShapeCount; k++)
					{
						for (int l = 0; l < sharedMesh.GetBlendShapeFrameCount(k); l++)
						{
							Vector3[] array6 = new Vector3[sharedMesh.vertexCount];
							Vector3[] array7 = new Vector3[sharedMesh.vertexCount];
							Vector3[] array8 = new Vector3[sharedMesh.vertexCount];
							if (!dictionary.ContainsKey(sharedMesh.GetBlendShapeName(k) + hashCode.ToString()))
							{
								sharedMesh.GetBlendShapeFrameVertices(k, l, array6, array7, array8);
								dictionary.Add(sharedMesh.GetBlendShapeName(k) + hashCode.ToString(), new MeshCombiner.BlendShapeFrame(sharedMesh.GetBlendShapeName(k) + hashCode.ToString(), sharedMesh.GetBlendShapeFrameWeight(k, l), array6, array7, array8, num));
							}
						}
					}
				}
				num += sharedMesh.vertexCount;
			}
			return MeshCombiner.CombineMeshes(array, array2, array3, array4, array5, out resultMaterials, out resultBones, dictionary);
		}

		// Token: 0x06003250 RID: 12880 RVA: 0x00225180 File Offset: 0x00223380
		public static Mesh CombineMeshes(Mesh[] meshes, Matrix4x4[] transforms, Tuple<Matrix4x4, bool>[] normalsTransforms, Material[][] materials, out Material[] resultMaterials, Dictionary<string, MeshCombiner.BlendShapeFrame> blendShapes = null)
		{
			if (meshes == null)
			{
				throw new ArgumentNullException("meshes");
			}
			if (transforms == null)
			{
				throw new ArgumentNullException("transforms");
			}
			if (materials == null)
			{
				throw new ArgumentNullException("materials");
			}
			Transform[] array;
			return MeshCombiner.CombineMeshes(meshes, transforms, normalsTransforms, materials, null, out resultMaterials, out array, blendShapes);
		}

		// Token: 0x06003251 RID: 12881 RVA: 0x002251C8 File Offset: 0x002233C8
		public static Mesh CombineMeshes(Mesh[] meshes, Matrix4x4[] transforms, Tuple<Matrix4x4, bool>[] normalsTransforms, Material[][] materials, Transform[][] bones, out Material[] resultMaterials, out Transform[] resultBones, Dictionary<string, MeshCombiner.BlendShapeFrame> blendShapes = null)
		{
			if (meshes == null)
			{
				throw new ArgumentNullException("meshes");
			}
			if (transforms == null)
			{
				throw new ArgumentNullException("transforms");
			}
			if (materials == null)
			{
				throw new ArgumentNullException("materials");
			}
			if (transforms.Length != meshes.Length)
			{
				throw new ArgumentException("The array of transforms doesn't have the same length as the array of meshes.", "transforms");
			}
			if (materials.Length != meshes.Length)
			{
				throw new ArgumentException("The array of materials doesn't have the same length as the array of meshes.", "materials");
			}
			if (bones != null && bones.Length != meshes.Length)
			{
				throw new ArgumentException("The array of bones doesn't have the same length as the array of meshes.", "bones");
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < meshes.Length; i++)
			{
				Mesh mesh = meshes[i];
				if (mesh == null)
				{
					throw new ArgumentException(string.Format("The mesh at index {0} is null.", i), "meshes");
				}
				if (!mesh.isReadable)
				{
					throw new ArgumentException(string.Format("The mesh at index {0} is not readable.", i), "meshes");
				}
				num += mesh.vertexCount;
				num2 += mesh.subMeshCount;
				Material[] array = materials[i];
				if (array == null)
				{
					throw new ArgumentException(string.Format("The materials for mesh at index {0} is null.", i), "materials");
				}
				if (array.Length != mesh.subMeshCount)
				{
					throw new ArgumentException(string.Format("The materials for mesh at index {0} doesn't match the submesh count ({1} != {2}).", i, array.Length, mesh.subMeshCount), "materials");
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] == null)
					{
						throw new ArgumentException(string.Format("The material at index {0} for mesh {1} is null.", j, mesh.name), "materials");
					}
				}
				if (bones != null)
				{
					Transform[] array2 = bones[i];
					if (array2 == null)
					{
						throw new ArgumentException(string.Format("The bones for mesh at index {0} is null.", i), "meshBones");
					}
					for (int k = 0; k < array2.Length; k++)
					{
						if (array2[k] == null)
						{
							throw new ArgumentException(string.Format("The bone at index {0} for mesh at index {1} is null.", k, i), "meshBones");
						}
					}
				}
			}
			List<Vector3> list = new List<Vector3>(num);
			List<int[]> list2 = new List<int[]>(num2);
			List<Vector3> list3 = null;
			List<Vector4> list4 = null;
			List<Color> list5 = null;
			List<BoneWeight> list6 = null;
			List<Vector4>[] array3 = new List<Vector4>[8];
			List<Matrix4x4> list7 = null;
			List<Transform> list8 = null;
			List<Material> list9 = new List<Material>(num2);
			Dictionary<Material, int> dictionary = new Dictionary<Material, int>(num2);
			int num3 = 0;
			for (int l = 0; l < meshes.Length; l++)
			{
				Mesh mesh2 = meshes[l];
				Matrix4x4 matrix4x = transforms[l];
				Tuple<Matrix4x4, bool> tuple = normalsTransforms[l];
				Material[] array4 = materials[l];
				Transform[] array5 = (bones != null) ? bones[l] : null;
				int subMeshCount = mesh2.subMeshCount;
				int vertexCount = mesh2.vertexCount;
				Vector3[] vertices = mesh2.vertices;
				Vector3[] normals = mesh2.normals;
				Vector4[] tangents = mesh2.tangents;
				List<Vector4>[] meshUVs = MeshCombiner.MeshUtils.GetMeshUVs(mesh2);
				Color[] colors = mesh2.colors;
				BoneWeight[] boneWeights = mesh2.boneWeights;
				Matrix4x4[] bindposes = mesh2.bindposes;
				if (array5 != null && boneWeights != null && boneWeights.Length != 0 && bindposes != null && bindposes.Length != 0 && array5.Length == bindposes.Length)
				{
					if (list7 == null)
					{
						list7 = new List<Matrix4x4>(bindposes);
						list8 = new List<Transform>(array5);
					}
					int[] array6 = new int[array5.Length];
					for (int m = 0; m < array5.Length; m++)
					{
						int num4 = list8.IndexOf(array5[m]);
						if (num4 == -1 || bindposes[m] != list7[num4])
						{
							num4 = list8.Count;
							list8.Add(array5[m]);
							list7.Add(bindposes[m]);
						}
						array6[m] = num4;
					}
					MeshCombiner.RemapBones(boneWeights, array6);
				}
				MeshCombiner.TransformVertices(vertices, ref matrix4x);
				MeshCombiner.TransformNormals(normals, ref tuple);
				MeshCombiner.TransformTangents(tangents, ref tuple);
				MeshCombiner.CopyVertexPositions(list, vertices);
				MeshCombiner.CopyVertexAttributes<Vector3>(ref list3, normals, num3, vertexCount, num, new Vector3(1f, 0f, 0f));
				MeshCombiner.CopyVertexAttributes<Vector4>(ref list4, tangents, num3, vertexCount, num, new Vector4(0f, 0f, 1f, 1f));
				MeshCombiner.CopyVertexAttributes<Color>(ref list5, colors, num3, vertexCount, num, new Color(1f, 1f, 1f, 1f));
				MeshCombiner.CopyVertexAttributes<BoneWeight>(ref list6, boneWeights, num3, vertexCount, num, default(BoneWeight));
				for (int n = 0; n < meshUVs.Length; n++)
				{
					MeshCombiner.CopyVertexAttributes<Vector4>(ref array3[n], meshUVs[n], num3, vertexCount, num, new Vector4(0f, 0f, 0f, 0f));
				}
				for (int num5 = 0; num5 < subMeshCount; num5++)
				{
					Material material = array4[num5];
					int[] triangles = mesh2.GetTriangles(num5, true);
					if (num3 > 0)
					{
						for (int num6 = 0; num6 < triangles.Length; num6++)
						{
							triangles[num6] += num3;
						}
					}
					int num7;
					if (dictionary.TryGetValue(material, ref num7))
					{
						list2[num7] = MeshCombiner.MergeArrays<int>(list2[num7], triangles);
					}
					else
					{
						int count = list2.Count;
						dictionary.Add(material, count);
						list9.Add(material);
						list2.Add(triangles);
					}
				}
				num3 += vertexCount;
			}
			Vector3[] vertices2 = list.ToArray();
			int[][] indices = list2.ToArray();
			Vector3[] normals2 = (list3 != null) ? list3.ToArray() : null;
			Vector4[] tangents2 = (list4 != null) ? list4.ToArray() : null;
			Color[] colors2 = (list5 != null) ? list5.ToArray() : null;
			BoneWeight[] boneWeights2 = (list6 != null) ? list6.ToArray() : null;
			List<Vector4>[] uvs = Enumerable.ToArray<List<Vector4>>(array3);
			Matrix4x4[] bindposes2 = (list7 != null) ? list7.ToArray() : null;
			resultMaterials = list9.ToArray();
			resultBones = ((list8 != null) ? list8.ToArray() : null);
			Mesh mesh3 = MeshCombiner.MeshUtils.CreateMesh(vertices2, indices, normals2, tangents2, colors2, boneWeights2, uvs, bindposes2, null);
			if (blendShapes != null && blendShapes.Count > 0)
			{
				foreach (MeshCombiner.BlendShapeFrame blendShapeFrame in blendShapes.Values)
				{
					Vector3[] array7 = new Vector3[mesh3.vertexCount];
					Vector3[] array8 = new Vector3[mesh3.vertexCount];
					Vector3[] array9 = new Vector3[mesh3.vertexCount];
					for (int num8 = 0; num8 < blendShapeFrame.deltaVertices.Length; num8++)
					{
						array7.SetValue(blendShapeFrame.deltaVertices[num8], num8 + blendShapeFrame.vertexOffset);
						array8.SetValue(blendShapeFrame.deltaNormals[num8], num8 + blendShapeFrame.vertexOffset);
						array9.SetValue(blendShapeFrame.deltaTangents[num8], num8 + blendShapeFrame.vertexOffset);
					}
					mesh3.AddBlendShapeFrame(blendShapeFrame.shapeName, blendShapeFrame.frameWeight, array7, array8, array9);
				}
			}
			mesh3.normals = normals2;
			mesh3.tangents = tangents2;
			mesh3.RecalculateBounds();
			return mesh3;
		}

		// Token: 0x06003252 RID: 12882 RVA: 0x002258E0 File Offset: 0x00223AE0
		private static void ParentAndResetTransform(Transform transform, Transform parentTransform)
		{
			transform.SetParent(parentTransform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		// Token: 0x06003253 RID: 12883 RVA: 0x0022590A File Offset: 0x00223B0A
		private static void ParentAndOffsetTransform(Transform transform, Transform parentTransform, Transform originalTransform)
		{
			transform.position = originalTransform.position;
			transform.rotation = originalTransform.rotation;
			transform.localScale = originalTransform.lossyScale;
			transform.SetParent(parentTransform, true);
		}

		// Token: 0x06003254 RID: 12884 RVA: 0x00225938 File Offset: 0x00223B38
		private static Transform FindBestRootBone(Transform transform, SkinnedMeshRenderer[] skinnedMeshRenderers)
		{
			if (skinnedMeshRenderers == null || skinnedMeshRenderers.Length == 0)
			{
				return null;
			}
			Transform result = null;
			float num = float.MaxValue;
			for (int i = 0; i < skinnedMeshRenderers.Length; i++)
			{
				if (!(skinnedMeshRenderers[i] == null) && !(skinnedMeshRenderers[i].rootBone == null))
				{
					Transform rootBone = skinnedMeshRenderers[i].rootBone;
					float sqrMagnitude = (rootBone.position - transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						result = rootBone;
						num = sqrMagnitude;
					}
				}
			}
			return result;
		}

		// Token: 0x06003255 RID: 12885 RVA: 0x002259B0 File Offset: 0x00223BB0
		private static Transform FindBestRootBone(Dictionary<Transform, Transform> topLevelParents, SkinnedMeshRenderer[] skinnedMeshRenderers)
		{
			if (skinnedMeshRenderers == null || skinnedMeshRenderers.Length == 0)
			{
				return null;
			}
			Transform result = null;
			float num = float.MaxValue;
			for (int i = 0; i < skinnedMeshRenderers.Length; i++)
			{
				if (!(skinnedMeshRenderers[i] == null) && !(skinnedMeshRenderers[i].rootBone == null))
				{
					Transform transform = topLevelParents[skinnedMeshRenderers[i].transform];
					Transform rootBone = skinnedMeshRenderers[i].rootBone;
					float sqrMagnitude = (rootBone.position - transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						result = rootBone;
						num = sqrMagnitude;
					}
				}
			}
			return result;
		}

		// Token: 0x06003256 RID: 12886 RVA: 0x00225A3C File Offset: 0x00223C3C
		private static Transform GetTopLevelParent(Transform forObject)
		{
			Transform transform = forObject;
			while (transform.parent != null)
			{
				transform = transform.parent;
			}
			return transform;
		}

		// Token: 0x06003257 RID: 12887 RVA: 0x00225A64 File Offset: 0x00223C64
		private static void CopyVertexPositions(List<Vector3> list, Vector3[] arr)
		{
			if (arr == null || arr.Length == 0)
			{
				return;
			}
			for (int i = 0; i < arr.Length; i++)
			{
				list.Add(arr[i]);
			}
		}

		// Token: 0x06003258 RID: 12888 RVA: 0x00225A94 File Offset: 0x00223C94
		private static void CopyVertexAttributes<T>(ref List<T> dest, IEnumerable<T> src, int previousVertexCount, int meshVertexCount, int totalVertexCount, T defaultValue)
		{
			if (src == null || Enumerable.Count<T>(src) == 0)
			{
				if (dest != null)
				{
					for (int i = 0; i < meshVertexCount; i++)
					{
						dest.Add(defaultValue);
					}
				}
				return;
			}
			if (dest == null)
			{
				dest = new List<T>(totalVertexCount);
				for (int j = 0; j < previousVertexCount; j++)
				{
					dest.Add(defaultValue);
				}
			}
			dest.AddRange(src);
		}

		// Token: 0x06003259 RID: 12889 RVA: 0x00225AF0 File Offset: 0x00223CF0
		private static T[] MergeArrays<T>(T[] arr1, T[] arr2)
		{
			T[] array = new T[arr1.Length + arr2.Length];
			Array.Copy(arr1, 0, array, 0, arr1.Length);
			Array.Copy(arr2, 0, array, arr1.Length, arr2.Length);
			return array;
		}

		// Token: 0x0600325A RID: 12890 RVA: 0x00225B28 File Offset: 0x00223D28
		private static void TransformVertices(Vector3[] vertices, ref Matrix4x4 transform)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = transform.MultiplyPoint3x4(vertices[i]);
			}
		}

		// Token: 0x0600325B RID: 12891 RVA: 0x00225B58 File Offset: 0x00223D58
		private static void TransformNormals(Vector3[] normals, ref Tuple<Matrix4x4, bool> transform)
		{
			if (normals == null)
			{
				return;
			}
			for (int i = 0; i < normals.Length; i++)
			{
				if (transform.Item2)
				{
					Quaternion quaternion = Quaternion.LookRotation(transform.Item1.GetColumn(2), transform.Item1.GetColumn(1));
					normals[i] = quaternion * normals[i];
				}
				else
				{
					normals[i] = transform.Item1.MultiplyVector(normals[i]);
				}
			}
		}

		// Token: 0x0600325C RID: 12892 RVA: 0x00225BE4 File Offset: 0x00223DE4
		private static void TransformTangents(Vector4[] tangents, ref Tuple<Matrix4x4, bool> transform)
		{
			if (tangents == null)
			{
				return;
			}
			for (int i = 0; i < tangents.Length; i++)
			{
				Vector3 vector = transform.Item1.MultiplyVector(new Vector3(tangents[i].x, tangents[i].y, tangents[i].z));
				tangents[i] = new Vector4(vector.x, vector.y, vector.z, tangents[i].w);
			}
		}

		// Token: 0x0600325D RID: 12893 RVA: 0x00225C68 File Offset: 0x00223E68
		private static void RemapBones(BoneWeight[] boneWeights, int[] boneIndices)
		{
			for (int i = 0; i < boneWeights.Length; i++)
			{
				if (boneWeights[i].weight0 > 0f)
				{
					boneWeights[i].boneIndex0 = boneIndices[boneWeights[i].boneIndex0];
				}
				if (boneWeights[i].weight1 > 0f)
				{
					boneWeights[i].boneIndex1 = boneIndices[boneWeights[i].boneIndex1];
				}
				if (boneWeights[i].weight2 > 0f)
				{
					boneWeights[i].boneIndex2 = boneIndices[boneWeights[i].boneIndex2];
				}
				if (boneWeights[i].weight3 > 0f)
				{
					boneWeights[i].boneIndex3 = boneIndices[boneWeights[i].boneIndex3];
				}
			}
		}

		// Token: 0x0600325E RID: 12894 RVA: 0x00225D40 File Offset: 0x00223F40
		private static Matrix4x4 ScaleMatrix(ref Matrix4x4 matrix, float scale)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.m00 = matrix.m00 * scale;
			result.m01 = matrix.m01 * scale;
			result.m02 = matrix.m02 * scale;
			result.m03 = matrix.m03 * scale;
			result.m10 = matrix.m10 * scale;
			result.m11 = matrix.m11 * scale;
			result.m12 = matrix.m12 * scale;
			result.m13 = matrix.m13 * scale;
			result.m20 = matrix.m20 * scale;
			result.m21 = matrix.m21 * scale;
			result.m22 = matrix.m22 * scale;
			result.m23 = matrix.m23 * scale;
			result.m30 = matrix.m30 * scale;
			result.m31 = matrix.m31 * scale;
			result.m32 = matrix.m32 * scale;
			result.m33 = matrix.m33 * scale;
			return result;
		}

		// Token: 0x0600325F RID: 12895 RVA: 0x00225E48 File Offset: 0x00224048
		private static void CombineMeshesUnity(Transform parentTransform, MeshFilter[] meshFilters)
		{
			Dictionary<Material, List<CombineInstance>> dictionary = new Dictionary<Material, List<CombineInstance>>();
			int num = 0;
			foreach (MeshFilter meshFilter in meshFilters)
			{
				if (!(meshFilter == null))
				{
					Mesh sharedMesh = meshFilter.sharedMesh;
					if (!(sharedMesh == null))
					{
						num += sharedMesh.vertexCount;
					}
				}
			}
			foreach (MeshFilter meshFilter2 in meshFilters)
			{
				Mesh sharedMesh2 = meshFilter2.sharedMesh;
				Material[] sharedMaterials = meshFilter2.GetComponent<Renderer>().sharedMaterials;
				int subMeshCount = meshFilter2.sharedMesh.subMeshCount;
				if (sharedMaterials == null)
				{
					throw new ArgumentException(string.Format("The materials for GameObject are null.", meshFilter2.transform.name), "materials");
				}
				if (sharedMaterials.Length != sharedMesh2.subMeshCount)
				{
					throw new ArgumentException(string.Format("The materials for mesh {0} on GameObject {1} doesn't match the submesh count ({2} != {3}).", new object[]
					{
						sharedMesh2.name,
						meshFilter2.transform.name,
						sharedMaterials.Length,
						sharedMesh2.subMeshCount
					}), "materials");
				}
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[j] == null)
					{
						throw new ArgumentException(string.Format("The material at index {0} for mesh {1} on GameObject {2} is null.", j, sharedMesh2.name, meshFilter2.transform.name), "materials");
					}
				}
				for (int k = 0; k < subMeshCount; k++)
				{
					Material material = sharedMaterials[k];
					List<int> list = new List<int>();
					sharedMesh2.GetTriangles(list, k);
					Mesh mesh = Object.Instantiate<Mesh>(sharedMesh2);
					mesh.triangles = list.ToArray();
					if (!dictionary.ContainsKey(material))
					{
						dictionary.Add(material, new List<CombineInstance>());
					}
					CombineInstance combineInstance = default(CombineInstance);
					combineInstance.transform = parentTransform.worldToLocalMatrix * meshFilter2.transform.localToWorldMatrix;
					combineInstance.mesh = mesh;
					CombineInstance combineInstance2 = combineInstance;
					dictionary[material].Add(combineInstance2);
				}
			}
			MeshCombiner.unityCombinedMeshRenderers = new MeshRenderer[dictionary.Count];
			MeshCombiner.unityCombinedMeshesMats = new Material[dictionary.Count];
			int num2 = 0;
			foreach (KeyValuePair<Material, List<CombineInstance>> keyValuePair in dictionary)
			{
				GameObject gameObject = new GameObject(keyValuePair.Key.name);
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				MeshFilter meshFilter3 = gameObject.AddComponent<MeshFilter>();
				meshRenderer.material = keyValuePair.Key;
				Mesh mesh2 = new Mesh();
				if (num > 65534)
				{
					mesh2.indexFormat = 1;
				}
				mesh2.CombineMeshes(keyValuePair.Value.ToArray());
				meshFilter3.sharedMesh = mesh2;
				gameObject.transform.parent = parentTransform.parent;
				MeshCombiner.unityCombinedMeshesMats[num2] = keyValuePair.Key;
				MeshCombiner.unityCombinedMeshRenderers[num2] = meshRenderer;
				num2++;
			}
		}

		// Token: 0x04004DE1 RID: 19937
		private static MeshRenderer[] unityCombinedMeshRenderers;

		// Token: 0x04004DE2 RID: 19938
		private static Material[] unityCombinedMeshesMats;

		// Token: 0x04004DE3 RID: 19939
		private static bool didUseUnityCombine;

		// Token: 0x04004DE4 RID: 19940
		public static bool generateUV2;

		// Token: 0x02000941 RID: 2369
		public struct StaticRenderer
		{
			// Token: 0x04004DE5 RID: 19941
			public string name;

			// Token: 0x04004DE6 RID: 19942
			public bool isNewMesh;

			// Token: 0x04004DE7 RID: 19943
			public Transform transform;

			// Token: 0x04004DE8 RID: 19944
			public Mesh mesh;

			// Token: 0x04004DE9 RID: 19945
			public Material[] materials;
		}

		// Token: 0x02000942 RID: 2370
		public struct SkinnedRenderer
		{
			// Token: 0x04004DEA RID: 19946
			public bool hasBlendShapes;

			// Token: 0x04004DEB RID: 19947
			public string name;

			// Token: 0x04004DEC RID: 19948
			public bool isNewMesh;

			// Token: 0x04004DED RID: 19949
			public Transform transform;

			// Token: 0x04004DEE RID: 19950
			public Mesh mesh;

			// Token: 0x04004DEF RID: 19951
			public Material[] materials;

			// Token: 0x04004DF0 RID: 19952
			public Transform rootBone;

			// Token: 0x04004DF1 RID: 19953
			public Transform[] bones;
		}

		// Token: 0x02000943 RID: 2371
		[Serializable]
		public struct BlendShape
		{
			// Token: 0x06003260 RID: 12896 RVA: 0x00226140 File Offset: 0x00224340
			public BlendShape(string shapeName, MeshCombiner.BlendShapeFrame[] frames)
			{
				this.ShapeName = shapeName;
				this.Frames = frames;
			}

			// Token: 0x04004DF2 RID: 19954
			public string ShapeName;

			// Token: 0x04004DF3 RID: 19955
			public MeshCombiner.BlendShapeFrame[] Frames;
		}

		// Token: 0x02000944 RID: 2372
		[Serializable]
		public struct BlendShapeFrame
		{
			// Token: 0x06003261 RID: 12897 RVA: 0x00226150 File Offset: 0x00224350
			public BlendShapeFrame(float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
			{
				this.frameWeight = frameWeight;
				this.deltaVertices = deltaVertices;
				this.deltaNormals = deltaNormals;
				this.deltaTangents = deltaTangents;
				this.shapeName = "";
				this.vertexOffset = -1;
			}

			// Token: 0x06003262 RID: 12898 RVA: 0x00226181 File Offset: 0x00224381
			public BlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents, int vertexOffset)
			{
				this.shapeName = shapeName;
				this.frameWeight = frameWeight;
				this.deltaVertices = deltaVertices;
				this.deltaNormals = deltaNormals;
				this.deltaTangents = deltaTangents;
				this.vertexOffset = vertexOffset;
			}

			// Token: 0x04004DF4 RID: 19956
			public string shapeName;

			// Token: 0x04004DF5 RID: 19957
			public float frameWeight;

			// Token: 0x04004DF6 RID: 19958
			public Vector3[] deltaVertices;

			// Token: 0x04004DF7 RID: 19959
			public Vector3[] deltaNormals;

			// Token: 0x04004DF8 RID: 19960
			public Vector3[] deltaTangents;

			// Token: 0x04004DF9 RID: 19961
			public int vertexOffset;
		}

		// Token: 0x02000945 RID: 2373
		public static class MeshUtils
		{
			// Token: 0x06003263 RID: 12899 RVA: 0x002261B0 File Offset: 0x002243B0
			public static Mesh CreateMesh(Vector3[] vertices, int[][] indices, Vector3[] normals, Vector4[] tangents, Color[] colors, BoneWeight[] boneWeights, List<Vector2>[] uvs, Matrix4x4[] bindposes, MeshCombiner.BlendShape[] blendShapes)
			{
				return MeshCombiner.MeshUtils.CreateMesh(vertices, indices, normals, tangents, colors, boneWeights, uvs, null, null, bindposes, blendShapes);
			}

			// Token: 0x06003264 RID: 12900 RVA: 0x002261D4 File Offset: 0x002243D4
			public static Mesh CreateMesh(Vector3[] vertices, int[][] indices, Vector3[] normals, Vector4[] tangents, Color[] colors, BoneWeight[] boneWeights, List<Vector4>[] uvs, Matrix4x4[] bindposes, MeshCombiner.BlendShape[] blendShapes)
			{
				return MeshCombiner.MeshUtils.CreateMesh(vertices, indices, normals, tangents, colors, boneWeights, null, null, uvs, bindposes, blendShapes);
			}

			// Token: 0x06003265 RID: 12901 RVA: 0x002261F8 File Offset: 0x002243F8
			public static Mesh CreateMesh(Vector3[] vertices, int[][] indices, Vector3[] normals, Vector4[] tangents, Color[] colors, BoneWeight[] boneWeights, List<Vector2>[] uvs2D, List<Vector3>[] uvs3D, List<Vector4>[] uvs4D, Matrix4x4[] bindposes, MeshCombiner.BlendShape[] blendShapes)
			{
				Mesh mesh = new Mesh();
				int num = indices.Length;
				IndexFormat indexFormat;
				Vector2Int[] subMeshIndexMinMax = MeshCombiner.MeshUtils.GetSubMeshIndexMinMax(indices, out indexFormat);
				mesh.indexFormat = indexFormat;
				if (bindposes != null && bindposes.Length != 0)
				{
					mesh.bindposes = bindposes;
				}
				mesh.subMeshCount = num;
				mesh.vertices = vertices;
				if (blendShapes != null)
				{
					MeshCombiner.MeshUtils.ApplyMeshBlendShapes(mesh, blendShapes);
				}
				if (normals != null && normals.Length != 0)
				{
					mesh.normals = normals;
				}
				if (tangents != null && tangents.Length != 0)
				{
					mesh.tangents = tangents;
				}
				if (colors != null && colors.Length != 0)
				{
					mesh.colors = colors;
				}
				if (boneWeights != null && boneWeights.Length != 0)
				{
					mesh.boneWeights = boneWeights;
				}
				if (uvs2D != null)
				{
					for (int i = 0; i < uvs2D.Length; i++)
					{
						if (uvs2D[i] != null && uvs2D[i].Count > 0)
						{
							mesh.SetUVs(i, uvs2D[i]);
						}
					}
				}
				if (uvs3D != null)
				{
					for (int j = 0; j < uvs3D.Length; j++)
					{
						if (uvs3D[j] != null && uvs3D[j].Count > 0)
						{
							mesh.SetUVs(j, uvs3D[j]);
						}
					}
				}
				if (uvs4D != null)
				{
					for (int k = 0; k < uvs4D.Length; k++)
					{
						if (uvs4D[k] != null && uvs4D[k].Count > 0)
						{
							mesh.SetUVs(k, uvs4D[k]);
						}
					}
				}
				for (int l = 0; l < num; l++)
				{
					int[] array = indices[l];
					Vector2Int vector2Int = subMeshIndexMinMax[l];
					if (indexFormat == null && vector2Int.y > 65535)
					{
						int x = vector2Int.x;
						for (int m = 0; m < array.Length; m++)
						{
							array[m] -= x;
						}
						mesh.SetTriangles(array, l, false, x);
					}
					else
					{
						mesh.SetTriangles(array, l, false, 0);
					}
				}
				mesh.RecalculateBounds();
				return mesh;
			}

			// Token: 0x06003266 RID: 12902 RVA: 0x002263B0 File Offset: 0x002245B0
			public static MeshCombiner.BlendShape[] GetMeshBlendShapes(Mesh mesh)
			{
				if (mesh == null)
				{
					throw new ArgumentNullException("mesh");
				}
				int vertexCount = mesh.vertexCount;
				int blendShapeCount = mesh.blendShapeCount;
				if (blendShapeCount == 0)
				{
					return null;
				}
				MeshCombiner.BlendShape[] array = new MeshCombiner.BlendShape[blendShapeCount];
				for (int i = 0; i < blendShapeCount; i++)
				{
					string blendShapeName = mesh.GetBlendShapeName(i);
					int blendShapeFrameCount = mesh.GetBlendShapeFrameCount(i);
					MeshCombiner.BlendShapeFrame[] array2 = new MeshCombiner.BlendShapeFrame[blendShapeFrameCount];
					for (int j = 0; j < blendShapeFrameCount; j++)
					{
						float blendShapeFrameWeight = mesh.GetBlendShapeFrameWeight(i, j);
						Vector3[] array3 = new Vector3[vertexCount];
						Vector3[] array4 = new Vector3[vertexCount];
						Vector3[] array5 = new Vector3[vertexCount];
						mesh.GetBlendShapeFrameVertices(i, j, array3, array4, array5);
						array2[j] = new MeshCombiner.BlendShapeFrame(blendShapeFrameWeight, array3, array4, array5);
					}
					array[i] = new MeshCombiner.BlendShape(blendShapeName, array2);
				}
				return array;
			}

			// Token: 0x06003267 RID: 12903 RVA: 0x00226484 File Offset: 0x00224684
			public static void ApplyMeshBlendShapes(Mesh mesh, MeshCombiner.BlendShape[] blendShapes)
			{
				if (mesh == null)
				{
					throw new ArgumentNullException("mesh");
				}
				mesh.ClearBlendShapes();
				if (blendShapes == null || blendShapes.Length == 0)
				{
					return;
				}
				for (int i = 0; i < blendShapes.Length; i++)
				{
					string shapeName = blendShapes[i].ShapeName;
					MeshCombiner.BlendShapeFrame[] frames = blendShapes[i].Frames;
					if (frames != null)
					{
						for (int j = 0; j < frames.Length; j++)
						{
							mesh.AddBlendShapeFrame(shapeName, frames[j].frameWeight, frames[j].deltaVertices, frames[j].deltaNormals, frames[j].deltaTangents);
						}
					}
				}
			}

			// Token: 0x06003268 RID: 12904 RVA: 0x00226524 File Offset: 0x00224724
			public static List<Vector4>[] GetMeshUVs(Mesh mesh)
			{
				if (mesh == null)
				{
					throw new ArgumentNullException("mesh");
				}
				List<Vector4>[] array = new List<Vector4>[8];
				for (int i = 0; i < 8; i++)
				{
					array[i] = MeshCombiner.MeshUtils.GetMeshUVs(mesh, i);
				}
				return array;
			}

			// Token: 0x06003269 RID: 12905 RVA: 0x00226564 File Offset: 0x00224764
			public static List<Vector4> GetMeshUVs(Mesh mesh, int channel)
			{
				if (mesh == null)
				{
					throw new ArgumentNullException("mesh");
				}
				if (channel < 0 || channel >= 8)
				{
					throw new ArgumentOutOfRangeException("channel");
				}
				List<Vector4> list = new List<Vector4>(mesh.vertexCount);
				mesh.GetUVs(channel, list);
				return list;
			}

			// Token: 0x0600326A RID: 12906 RVA: 0x002265B0 File Offset: 0x002247B0
			public static int GetUsedUVComponents(List<Vector4> uvs)
			{
				if (uvs == null || uvs.Count == 0)
				{
					return 0;
				}
				int num = 0;
				foreach (Vector4 vector in uvs)
				{
					if (num < 1 && vector.x != 0f)
					{
						num = 1;
					}
					if (num < 2 && vector.y != 0f)
					{
						num = 2;
					}
					if (num < 3 && vector.z != 0f)
					{
						num = 3;
					}
					if (num < 4 && vector.w != 0f)
					{
						num = 4;
						break;
					}
				}
				return num;
			}

			// Token: 0x0600326B RID: 12907 RVA: 0x00226658 File Offset: 0x00224858
			public static Vector2[] ConvertUVsTo2D(List<Vector4> uvs)
			{
				if (uvs == null)
				{
					return null;
				}
				Vector2[] array = new Vector2[uvs.Count];
				for (int i = 0; i < array.Length; i++)
				{
					Vector4 vector = uvs[i];
					array[i] = new Vector2(vector.x, vector.y);
				}
				return array;
			}

			// Token: 0x0600326C RID: 12908 RVA: 0x002266A8 File Offset: 0x002248A8
			public static Vector3[] ConvertUVsTo3D(List<Vector4> uvs)
			{
				if (uvs == null)
				{
					return null;
				}
				Vector3[] array = new Vector3[uvs.Count];
				for (int i = 0; i < array.Length; i++)
				{
					Vector4 vector = uvs[i];
					array[i] = new Vector3(vector.x, vector.y, vector.z);
				}
				return array;
			}

			// Token: 0x0600326D RID: 12909 RVA: 0x002266FC File Offset: 0x002248FC
			public static Vector2Int[] GetSubMeshIndexMinMax(int[][] indices, out IndexFormat indexFormat)
			{
				if (indices == null)
				{
					throw new ArgumentNullException("indices");
				}
				Vector2Int[] array = new Vector2Int[indices.Length];
				indexFormat = 0;
				for (int i = 0; i < indices.Length; i++)
				{
					int num;
					int num2;
					MeshCombiner.MeshUtils.GetIndexMinMax(indices[i], out num, out num2);
					array[i] = new Vector2Int(num, num2);
					if (num2 - num > 65535)
					{
						indexFormat = 1;
					}
				}
				return array;
			}

			// Token: 0x0600326E RID: 12910 RVA: 0x0022675C File Offset: 0x0022495C
			private static void GetIndexMinMax(int[] indices, out int minIndex, out int maxIndex)
			{
				if (indices == null || indices.Length == 0)
				{
					minIndex = (maxIndex = 0);
					return;
				}
				minIndex = int.MaxValue;
				maxIndex = int.MinValue;
				for (int i = 0; i < indices.Length; i++)
				{
					if (indices[i] < minIndex)
					{
						minIndex = indices[i];
					}
					if (indices[i] > maxIndex)
					{
						maxIndex = indices[i];
					}
				}
			}

			// Token: 0x04004DFA RID: 19962
			public const int UVChannelCount = 8;
		}
	}
}
