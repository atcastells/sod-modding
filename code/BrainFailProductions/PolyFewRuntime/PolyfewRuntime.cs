using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BrainFailProductions.PolyFew;
using BrainFailProductions.PolyFew.AsImpL;
using UnityEngine;
using UnityMeshSimplifier;

namespace BrainFailProductions.PolyFewRuntime
{
	// Token: 0x02000947 RID: 2375
	[AddComponentMenu("")]
	public class PolyfewRuntime : MonoBehaviour
	{
		// Token: 0x06003275 RID: 12917 RVA: 0x00226800 File Offset: 0x00224A00
		public static int SimplifyObjectDeep(GameObject toSimplify, PolyfewRuntime.SimplificationOptions simplificationOptions, Action<GameObject, PolyfewRuntime.MeshRendererPair> OnEachMeshSimplified)
		{
			if (simplificationOptions == null)
			{
				throw new ArgumentNullException("simplificationOptions", "You must provide a SimplificationOptions object.");
			}
			int totalTriangles = 0;
			float simplificationStrength = simplificationOptions.simplificationStrength;
			if (toSimplify == null)
			{
				throw new ArgumentNullException("toSimplify", "You must provide a gameobject to simplify.");
			}
			if (!simplificationOptions.simplifyMeshLossless)
			{
				if (simplificationStrength < 0f || simplificationStrength > 100f)
				{
					throw new ArgumentOutOfRangeException("simplificationStrength", "The allowed values for simplification strength are between [0-100] inclusive.");
				}
				if (Mathf.Approximately(simplificationStrength, 0f))
				{
					return -1;
				}
			}
			if (simplificationOptions.regardPreservationSpheres && (simplificationOptions.preservationSpheres == null || simplificationOptions.preservationSpheres.Count == 0))
			{
				simplificationOptions.preservationSpheres = new List<PolyfewRuntime.PreservationSphere>();
				simplificationOptions.regardPreservationSpheres = false;
			}
			PolyfewRuntime.ObjectMeshPairs objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(toSimplify, true);
			if (!PolyfewRuntime.AreAnyFeasibleMeshes(objectMeshPairs))
			{
				throw new InvalidOperationException("No mesh/meshes found nested under the provided gameobject to simplify.");
			}
			bool flag = false;
			if (PolyfewRuntime.CountTriangles(objectMeshPairs) >= 2000 && objectMeshPairs.Count >= 2)
			{
				flag = true;
			}
			if (Application.platform == 17)
			{
				flag = false;
			}
			float quality = 1f - simplificationStrength / 100f;
			int count = objectMeshPairs.Count;
			int meshesHandled = 0;
			int threadsRunning = 0;
			bool isError = false;
			string error = "";
			object threadLock1 = new object();
			object threadLock2 = new object();
			object threadLock3 = new object();
			if (flag)
			{
				List<PolyfewRuntime.CustomMeshActionStructure> meshAssignments = new List<PolyfewRuntime.CustomMeshActionStructure>();
				List<PolyfewRuntime.CustomMeshActionStructure> callbackFlusher = new List<PolyfewRuntime.CustomMeshActionStructure>();
				using (Dictionary<GameObject, PolyfewRuntime.MeshRendererPair>.Enumerator enumerator = objectMeshPairs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair = enumerator.Current;
						GameObject gameObject = keyValuePair.Key;
						if (gameObject == null)
						{
							int num = meshesHandled;
							meshesHandled = num + 1;
						}
						else
						{
							PolyfewRuntime.MeshRendererPair meshRendererPair = keyValuePair.Value;
							if (meshRendererPair.mesh == null)
							{
								int num = meshesHandled;
								meshesHandled = num + 1;
							}
							else
							{
								MeshSimplifier meshSimplifier = new MeshSimplifier();
								PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier);
								ToleranceSphere[] array = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
								if (!meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
								{
									meshSimplifier.isSkinned = true;
									SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
									meshSimplifier.boneWeightsOriginal = meshRendererPair.mesh.boneWeights;
									meshSimplifier.bindPosesOriginal = meshRendererPair.mesh.bindposes;
									meshSimplifier.bonesOriginal = component.bones;
									int num2 = 0;
									foreach (PolyfewRuntime.PreservationSphere preservationSphere in simplificationOptions.preservationSpheres)
									{
										gameObject.transform.InverseTransformPoint(preservationSphere.worldPosition);
										ToleranceSphere toleranceSphere = new ToleranceSphere
										{
											diameter = preservationSphere.diameter,
											localToWorldMatrix = gameObject.transform.localToWorldMatrix,
											worldPosition = preservationSphere.worldPosition,
											targetObject = gameObject,
											preservationStrength = preservationSphere.preservationStrength
										};
										array[num2] = toleranceSphere;
										num2++;
									}
									meshSimplifier.toleranceSpheres = array;
								}
								else if (meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
								{
									int num3 = 0;
									foreach (PolyfewRuntime.PreservationSphere preservationSphere2 in simplificationOptions.preservationSpheres)
									{
										ToleranceSphere toleranceSphere2 = new ToleranceSphere
										{
											diameter = preservationSphere2.diameter,
											localToWorldMatrix = gameObject.transform.localToWorldMatrix,
											worldPosition = preservationSphere2.worldPosition,
											targetObject = gameObject,
											preservationStrength = preservationSphere2.preservationStrength
										};
										array[num3] = toleranceSphere2;
										num3++;
									}
									meshSimplifier.toleranceSpheres = array;
								}
								meshSimplifier.Initialize(meshRendererPair.mesh, simplificationOptions.regardPreservationSpheres);
								int num = threadsRunning;
								threadsRunning = num + 1;
								while (callbackFlusher.Count > 0)
								{
									PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure = callbackFlusher[0];
									callbackFlusher.RemoveAt(0);
									if (customMeshActionStructure != null && OnEachMeshSimplified != null)
									{
										OnEachMeshSimplified.Invoke(customMeshActionStructure.gameObject, customMeshActionStructure.meshRendererPair);
									}
								}
								Action <>9__1;
								Task.Factory.StartNew(delegate()
								{
									PolyfewRuntime.MeshRendererPair meshRendererPair = meshRendererPair;
									GameObject gameObject = gameObject;
									Action action;
									if ((action = <>9__1) == null)
									{
										action = (<>9__1 = delegate()
										{
											Mesh mesh2 = meshSimplifier.ToMesh();
											PolyfewRuntime.AssignReducedMesh(gameObject, meshRendererPair.mesh, mesh2, meshRendererPair.attachedToMeshFilter, true);
											if (meshSimplifier.RecalculateNormals)
											{
												mesh2.RecalculateNormals();
												mesh2.RecalculateTangents();
											}
											totalTriangles += mesh2.triangles.Length / 3;
										});
									}
									PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure5 = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, action);
									try
									{
										if (!simplificationOptions.simplifyMeshLossless)
										{
											meshSimplifier.SimplifyMesh(quality);
										}
										else
										{
											meshSimplifier.SimplifyMeshLossless();
										}
										object obj = threadLock1;
										lock (obj)
										{
											meshAssignments.Add(customMeshActionStructure5);
											int num6 = threadsRunning;
											threadsRunning = num6 - 1;
											num6 = meshesHandled;
											meshesHandled = num6 + 1;
										}
										obj = threadLock3;
										lock (obj)
										{
											PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure6 = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, delegate
											{
											});
											callbackFlusher.Add(customMeshActionStructure6);
										}
									}
									catch (Exception ex)
									{
										object obj = threadLock2;
										lock (obj)
										{
											int num6 = threadsRunning;
											threadsRunning = num6 - 1;
											num6 = meshesHandled;
											meshesHandled = num6 + 1;
											isError = true;
											error = ex.ToString();
										}
									}
								}, CancellationToken.None, 64, TaskScheduler.Current);
							}
						}
					}
					goto IL_5BD;
				}
				IL_588:
				PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure2 = callbackFlusher[0];
				callbackFlusher.RemoveAt(0);
				if (customMeshActionStructure2 != null && OnEachMeshSimplified != null)
				{
					OnEachMeshSimplified.Invoke(customMeshActionStructure2.gameObject, customMeshActionStructure2.meshRendererPair);
				}
				IL_5BD:
				if (callbackFlusher.Count > 0)
				{
					goto IL_588;
				}
				while (meshesHandled < count && !isError)
				{
					while (callbackFlusher.Count > 0)
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure3 = callbackFlusher[0];
						callbackFlusher.RemoveAt(0);
						if (customMeshActionStructure3 != null && OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified.Invoke(customMeshActionStructure3.gameObject, customMeshActionStructure3.meshRendererPair);
						}
					}
				}
				if (isError)
				{
					goto IL_9B0;
				}
				using (List<PolyfewRuntime.CustomMeshActionStructure>.Enumerator enumerator3 = meshAssignments.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure4 = enumerator3.Current;
						if (customMeshActionStructure4 != null)
						{
							customMeshActionStructure4.action.Invoke();
						}
					}
					goto IL_9B0;
				}
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair2 in objectMeshPairs)
			{
				GameObject key = keyValuePair2.Key;
				if (!(key == null))
				{
					PolyfewRuntime.MeshRendererPair value = keyValuePair2.Value;
					if (!(value.mesh == null))
					{
						MeshSimplifier meshSimplifier2 = new MeshSimplifier();
						PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier2);
						ToleranceSphere[] array2 = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
						if (!value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							meshSimplifier2.isSkinned = true;
							SkinnedMeshRenderer component2 = key.GetComponent<SkinnedMeshRenderer>();
							meshSimplifier2.boneWeightsOriginal = value.mesh.boneWeights;
							meshSimplifier2.bindPosesOriginal = value.mesh.bindposes;
							meshSimplifier2.bonesOriginal = component2.bones;
							int num4 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere3 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere3 = new ToleranceSphere
								{
									diameter = preservationSphere3.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere3.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere3.preservationStrength
								};
								array2[num4] = toleranceSphere3;
								num4++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						else if (value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							int num5 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere4 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere4 = new ToleranceSphere
								{
									diameter = preservationSphere4.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere4.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere4.preservationStrength
								};
								array2[num5] = toleranceSphere4;
								num5++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						meshSimplifier2.Initialize(value.mesh, simplificationOptions.regardPreservationSpheres);
						if (!simplificationOptions.simplifyMeshLossless)
						{
							meshSimplifier2.SimplifyMesh(quality);
						}
						else
						{
							meshSimplifier2.SimplifyMeshLossless();
						}
						if (OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified.Invoke(key, value);
						}
						Mesh mesh = meshSimplifier2.ToMesh();
						mesh.bindposes = value.mesh.bindposes;
						mesh.name = value.mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
						if (meshSimplifier2.RecalculateNormals)
						{
							mesh.RecalculateNormals();
							mesh.RecalculateTangents();
						}
						if (value.attachedToMeshFilter)
						{
							MeshFilter component3 = key.GetComponent<MeshFilter>();
							if (component3 != null)
							{
								component3.sharedMesh = mesh;
							}
						}
						else
						{
							SkinnedMeshRenderer component4 = key.GetComponent<SkinnedMeshRenderer>();
							if (component4 != null)
							{
								component4.sharedMesh = mesh;
							}
						}
						totalTriangles += mesh.triangles.Length / 3;
					}
				}
			}
			IL_9B0:
			return totalTriangles;
		}

		// Token: 0x06003276 RID: 12918 RVA: 0x00227270 File Offset: 0x00225470
		public static PolyfewRuntime.ObjectMeshPairs SimplifyObjectDeep(GameObject toSimplify, PolyfewRuntime.SimplificationOptions simplificationOptions)
		{
			if (simplificationOptions == null)
			{
				throw new ArgumentNullException("simplificationOptions", "You must provide a SimplificationOptions object.");
			}
			float simplificationStrength = simplificationOptions.simplificationStrength;
			PolyfewRuntime.ObjectMeshPairs toReturn = new PolyfewRuntime.ObjectMeshPairs();
			if (toSimplify == null)
			{
				throw new ArgumentNullException("toSimplify", "You must provide a gameobject to simplify.");
			}
			if (!simplificationOptions.simplifyMeshLossless)
			{
				if (simplificationStrength < 0f || simplificationStrength > 100f)
				{
					throw new ArgumentOutOfRangeException("simplificationStrength", "The allowed values for simplification strength are between [0-100] inclusive.");
				}
				if (Mathf.Approximately(simplificationStrength, 0f))
				{
					return null;
				}
			}
			if (simplificationOptions.regardPreservationSpheres && (simplificationOptions.preservationSpheres == null || simplificationOptions.preservationSpheres.Count == 0))
			{
				simplificationOptions.preservationSpheres = new List<PolyfewRuntime.PreservationSphere>();
				simplificationOptions.regardPreservationSpheres = false;
			}
			PolyfewRuntime.ObjectMeshPairs objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(toSimplify, true);
			if (!PolyfewRuntime.AreAnyFeasibleMeshes(objectMeshPairs))
			{
				throw new InvalidOperationException("No mesh/meshes found nested under the provided gameobject to simplify.");
			}
			bool flag = false;
			if (PolyfewRuntime.CountTriangles(objectMeshPairs) >= 2000 && objectMeshPairs.Count >= 2)
			{
				flag = true;
			}
			if (Application.platform == 17)
			{
				flag = false;
			}
			float quality = 1f - simplificationStrength / 100f;
			int count = objectMeshPairs.Count;
			int meshesHandled = 0;
			int threadsRunning = 0;
			bool isError = false;
			string error = "";
			object threadLock1 = new object();
			object threadLock2 = new object();
			if (flag)
			{
				List<PolyfewRuntime.CustomMeshActionStructure> meshAssignments = new List<PolyfewRuntime.CustomMeshActionStructure>();
				foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair in objectMeshPairs)
				{
					GameObject gameObject = keyValuePair.Key;
					if (gameObject == null)
					{
						int num = meshesHandled;
						meshesHandled = num + 1;
					}
					else
					{
						PolyfewRuntime.MeshRendererPair meshRendererPair = keyValuePair.Value;
						if (meshRendererPair.mesh == null)
						{
							int num = meshesHandled;
							meshesHandled = num + 1;
						}
						else
						{
							MeshSimplifier meshSimplifier = new MeshSimplifier();
							PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier);
							ToleranceSphere[] array = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
							if (!meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
							{
								meshSimplifier.isSkinned = true;
								SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
								meshSimplifier.boneWeightsOriginal = meshRendererPair.mesh.boneWeights;
								meshSimplifier.bindPosesOriginal = meshRendererPair.mesh.bindposes;
								meshSimplifier.bonesOriginal = component.bones;
								int num2 = 0;
								foreach (PolyfewRuntime.PreservationSphere preservationSphere in simplificationOptions.preservationSpheres)
								{
									ToleranceSphere toleranceSphere = new ToleranceSphere
									{
										diameter = preservationSphere.diameter,
										localToWorldMatrix = gameObject.transform.localToWorldMatrix,
										worldPosition = preservationSphere.worldPosition,
										targetObject = gameObject,
										preservationStrength = preservationSphere.preservationStrength
									};
									array[num2] = toleranceSphere;
									num2++;
								}
								meshSimplifier.toleranceSpheres = array;
							}
							else if (meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
							{
								int num3 = 0;
								foreach (PolyfewRuntime.PreservationSphere preservationSphere2 in simplificationOptions.preservationSpheres)
								{
									ToleranceSphere toleranceSphere2 = new ToleranceSphere
									{
										diameter = preservationSphere2.diameter,
										localToWorldMatrix = gameObject.transform.localToWorldMatrix,
										worldPosition = preservationSphere2.worldPosition,
										targetObject = gameObject,
										preservationStrength = preservationSphere2.preservationStrength
									};
									array[num3] = toleranceSphere2;
									num3++;
								}
								meshSimplifier.toleranceSpheres = array;
							}
							meshSimplifier.Initialize(meshRendererPair.mesh, simplificationOptions.regardPreservationSpheres);
							int num = threadsRunning;
							threadsRunning = num + 1;
							Action <>9__1;
							Task.Factory.StartNew(delegate()
							{
								PolyfewRuntime.MeshRendererPair meshRendererPair3 = meshRendererPair;
								GameObject gameObject = gameObject;
								Action action;
								if ((action = <>9__1) == null)
								{
									action = (<>9__1 = delegate()
									{
										Mesh mesh2 = meshSimplifier.ToMesh();
										mesh2.bindposes = meshRendererPair.mesh.bindposes;
										mesh2.name = meshRendererPair.mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
										if (meshSimplifier.RecalculateNormals)
										{
											mesh2.RecalculateNormals();
											mesh2.RecalculateTangents();
										}
										PolyfewRuntime.MeshRendererPair meshRendererPair4 = new PolyfewRuntime.MeshRendererPair(meshRendererPair.attachedToMeshFilter, mesh2);
										toReturn.Add(gameObject, meshRendererPair4);
									});
								}
								PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure2 = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair3, gameObject, action);
								try
								{
									if (!simplificationOptions.simplifyMeshLossless)
									{
										meshSimplifier.SimplifyMesh(quality);
									}
									else
									{
										meshSimplifier.SimplifyMeshLossless();
									}
									object obj = threadLock1;
									lock (obj)
									{
										meshAssignments.Add(customMeshActionStructure2);
										int num6 = threadsRunning;
										threadsRunning = num6 - 1;
										num6 = meshesHandled;
										meshesHandled = num6 + 1;
									}
								}
								catch (Exception ex)
								{
									object obj = threadLock2;
									lock (obj)
									{
										int num6 = threadsRunning;
										threadsRunning = num6 - 1;
										num6 = meshesHandled;
										meshesHandled = num6 + 1;
										isError = true;
										error = ex.ToString();
									}
								}
							}, CancellationToken.None, 64, TaskScheduler.Current);
						}
					}
				}
				while (meshesHandled < count && !isError)
				{
				}
				if (isError)
				{
					goto IL_8AD;
				}
				using (List<PolyfewRuntime.CustomMeshActionStructure>.Enumerator enumerator3 = meshAssignments.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure = enumerator3.Current;
						if (customMeshActionStructure != null)
						{
							customMeshActionStructure.action.Invoke();
						}
					}
					goto IL_8AD;
				}
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair2 in objectMeshPairs)
			{
				GameObject key = keyValuePair2.Key;
				if (!(key == null))
				{
					PolyfewRuntime.MeshRendererPair value = keyValuePair2.Value;
					if (!(value.mesh == null))
					{
						MeshSimplifier meshSimplifier2 = new MeshSimplifier();
						PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier2);
						ToleranceSphere[] array2 = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
						if (!value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							meshSimplifier2.isSkinned = true;
							SkinnedMeshRenderer component2 = key.GetComponent<SkinnedMeshRenderer>();
							meshSimplifier2.boneWeightsOriginal = value.mesh.boneWeights;
							meshSimplifier2.bindPosesOriginal = value.mesh.bindposes;
							meshSimplifier2.bonesOriginal = component2.bones;
							int num4 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere3 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere3 = new ToleranceSphere
								{
									diameter = preservationSphere3.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere3.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere3.preservationStrength
								};
								array2[num4] = toleranceSphere3;
								num4++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						else if (value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							int num5 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere4 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere4 = new ToleranceSphere
								{
									diameter = preservationSphere4.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere4.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere4.preservationStrength
								};
								array2[num5] = toleranceSphere4;
								num5++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						meshSimplifier2.Initialize(value.mesh, simplificationOptions.regardPreservationSpheres);
						if (!simplificationOptions.simplifyMeshLossless)
						{
							meshSimplifier2.SimplifyMesh(quality);
						}
						else
						{
							meshSimplifier2.SimplifyMeshLossless();
						}
						Mesh mesh = meshSimplifier2.ToMesh();
						mesh.bindposes = value.mesh.bindposes;
						mesh.name = value.mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
						if (meshSimplifier2.RecalculateNormals)
						{
							mesh.RecalculateNormals();
							mesh.RecalculateTangents();
						}
						if (value.attachedToMeshFilter)
						{
							MeshFilter component3 = key.GetComponent<MeshFilter>();
							PolyfewRuntime.MeshRendererPair meshRendererPair5 = new PolyfewRuntime.MeshRendererPair(true, mesh);
							toReturn.Add(key, meshRendererPair5);
							if (component3 != null)
							{
								component3.sharedMesh = mesh;
							}
						}
						else
						{
							SkinnedMeshRenderer component4 = key.GetComponent<SkinnedMeshRenderer>();
							PolyfewRuntime.MeshRendererPair meshRendererPair2 = new PolyfewRuntime.MeshRendererPair(false, mesh);
							toReturn.Add(key, meshRendererPair2);
							if (component4 != null)
							{
								component4.sharedMesh = mesh;
							}
						}
					}
				}
			}
			IL_8AD:
			return toReturn;
		}

		// Token: 0x06003277 RID: 12919 RVA: 0x00227BDC File Offset: 0x00225DDC
		public static int SimplifyObjectDeep(PolyfewRuntime.ObjectMeshPairs objectMeshPairs, PolyfewRuntime.SimplificationOptions simplificationOptions, Action<GameObject, PolyfewRuntime.MeshRendererPair> OnEachMeshSimplified)
		{
			if (simplificationOptions == null)
			{
				throw new ArgumentNullException("simplificationOptions", "You must provide a SimplificationOptions object.");
			}
			int totalTriangles = 0;
			float simplificationStrength = simplificationOptions.simplificationStrength;
			if (objectMeshPairs == null)
			{
				throw new ArgumentNullException("objectMeshPairs", "You must provide the objectMeshPairs structure to simplify.");
			}
			if (!simplificationOptions.simplifyMeshLossless)
			{
				if (simplificationStrength < 0f || simplificationStrength > 100f)
				{
					throw new ArgumentOutOfRangeException("simplificationStrength", "The allowed values for simplification strength are between [0-100] inclusive.");
				}
				if (Mathf.Approximately(simplificationStrength, 0f))
				{
					return -1;
				}
			}
			if (!PolyfewRuntime.AreAnyFeasibleMeshes(objectMeshPairs))
			{
				throw new InvalidOperationException("No mesh/meshes found nested under the provided gameobject to simplify.");
			}
			if (simplificationOptions.regardPreservationSpheres && (simplificationOptions.preservationSpheres == null || simplificationOptions.preservationSpheres.Count == 0))
			{
				simplificationOptions.preservationSpheres = new List<PolyfewRuntime.PreservationSphere>();
				simplificationOptions.regardPreservationSpheres = false;
			}
			bool flag = false;
			if (PolyfewRuntime.CountTriangles(objectMeshPairs) >= 2000 && objectMeshPairs.Count >= 2)
			{
				flag = true;
			}
			if (Application.platform == 17)
			{
				flag = false;
			}
			float quality = 1f - simplificationStrength / 100f;
			int count = objectMeshPairs.Count;
			int meshesHandled = 0;
			int threadsRunning = 0;
			bool isError = false;
			string error = "";
			object threadLock1 = new object();
			object threadLock2 = new object();
			object threadLock3 = new object();
			if (flag)
			{
				List<PolyfewRuntime.CustomMeshActionStructure> meshAssignments = new List<PolyfewRuntime.CustomMeshActionStructure>();
				List<PolyfewRuntime.CustomMeshActionStructure> callbackFlusher = new List<PolyfewRuntime.CustomMeshActionStructure>();
				using (Dictionary<GameObject, PolyfewRuntime.MeshRendererPair>.Enumerator enumerator = objectMeshPairs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair = enumerator.Current;
						GameObject gameObject = keyValuePair.Key;
						if (gameObject == null)
						{
							int num = meshesHandled;
							meshesHandled = num + 1;
						}
						else
						{
							PolyfewRuntime.MeshRendererPair meshRendererPair = keyValuePair.Value;
							if (meshRendererPair.mesh == null)
							{
								int num = meshesHandled;
								meshesHandled = num + 1;
							}
							else
							{
								MeshSimplifier meshSimplifier = new MeshSimplifier();
								PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier);
								ToleranceSphere[] array = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
								if (!meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
								{
									meshSimplifier.isSkinned = true;
									SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
									meshSimplifier.boneWeightsOriginal = meshRendererPair.mesh.boneWeights;
									meshSimplifier.bindPosesOriginal = meshRendererPair.mesh.bindposes;
									meshSimplifier.bonesOriginal = component.bones;
									int num2 = 0;
									foreach (PolyfewRuntime.PreservationSphere preservationSphere in simplificationOptions.preservationSpheres)
									{
										ToleranceSphere toleranceSphere = new ToleranceSphere
										{
											diameter = preservationSphere.diameter,
											localToWorldMatrix = gameObject.transform.localToWorldMatrix,
											worldPosition = preservationSphere.worldPosition,
											targetObject = gameObject,
											preservationStrength = preservationSphere.preservationStrength
										};
										array[num2] = toleranceSphere;
										num2++;
									}
									meshSimplifier.toleranceSpheres = array;
								}
								else if (meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
								{
									int num3 = 0;
									foreach (PolyfewRuntime.PreservationSphere preservationSphere2 in simplificationOptions.preservationSpheres)
									{
										ToleranceSphere toleranceSphere2 = new ToleranceSphere
										{
											diameter = preservationSphere2.diameter,
											localToWorldMatrix = gameObject.transform.localToWorldMatrix,
											worldPosition = preservationSphere2.worldPosition,
											targetObject = gameObject,
											preservationStrength = preservationSphere2.preservationStrength
										};
										array[num3] = toleranceSphere2;
										num3++;
									}
									meshSimplifier.toleranceSpheres = array;
								}
								meshSimplifier.Initialize(meshRendererPair.mesh, simplificationOptions.regardPreservationSpheres);
								int num = threadsRunning;
								threadsRunning = num + 1;
								while (callbackFlusher.Count > 0)
								{
									PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure = callbackFlusher[0];
									callbackFlusher.RemoveAt(0);
									if (customMeshActionStructure != null && OnEachMeshSimplified != null)
									{
										OnEachMeshSimplified.Invoke(customMeshActionStructure.gameObject, customMeshActionStructure.meshRendererPair);
									}
								}
								Action <>9__1;
								Task.Factory.StartNew(delegate()
								{
									PolyfewRuntime.MeshRendererPair meshRendererPair = meshRendererPair;
									GameObject gameObject = gameObject;
									Action action;
									if ((action = <>9__1) == null)
									{
										action = (<>9__1 = delegate()
										{
											Mesh mesh2 = meshSimplifier.ToMesh();
											PolyfewRuntime.AssignReducedMesh(gameObject, meshRendererPair.mesh, mesh2, meshRendererPair.attachedToMeshFilter, true);
											if (meshSimplifier.RecalculateNormals)
											{
												mesh2.RecalculateNormals();
												mesh2.RecalculateTangents();
											}
											totalTriangles += mesh2.triangles.Length / 3;
										});
									}
									PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure5 = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, action);
									try
									{
										if (!simplificationOptions.simplifyMeshLossless)
										{
											meshSimplifier.SimplifyMesh(quality);
										}
										else
										{
											meshSimplifier.SimplifyMeshLossless();
										}
										object obj = threadLock1;
										lock (obj)
										{
											meshAssignments.Add(customMeshActionStructure5);
											int num6 = threadsRunning;
											threadsRunning = num6 - 1;
											num6 = meshesHandled;
											meshesHandled = num6 + 1;
										}
										obj = threadLock3;
										lock (obj)
										{
											PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure6 = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, delegate
											{
											});
											callbackFlusher.Add(customMeshActionStructure6);
										}
									}
									catch (Exception ex)
									{
										object obj = threadLock2;
										lock (obj)
										{
											int num6 = threadsRunning;
											threadsRunning = num6 - 1;
											num6 = meshesHandled;
											meshesHandled = num6 + 1;
											isError = true;
											error = ex.ToString();
										}
									}
								}, CancellationToken.None, 64, TaskScheduler.Current);
							}
						}
					}
					goto IL_58F;
				}
				IL_55A:
				PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure2 = callbackFlusher[0];
				callbackFlusher.RemoveAt(0);
				if (customMeshActionStructure2 != null && OnEachMeshSimplified != null)
				{
					OnEachMeshSimplified.Invoke(customMeshActionStructure2.gameObject, customMeshActionStructure2.meshRendererPair);
				}
				IL_58F:
				if (callbackFlusher.Count > 0)
				{
					goto IL_55A;
				}
				while (meshesHandled < count && !isError)
				{
					while (callbackFlusher.Count > 0)
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure3 = callbackFlusher[0];
						callbackFlusher.RemoveAt(0);
						if (customMeshActionStructure3 != null && OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified.Invoke(customMeshActionStructure3.gameObject, customMeshActionStructure3.meshRendererPair);
						}
					}
				}
				if (isError)
				{
					goto IL_981;
				}
				using (List<PolyfewRuntime.CustomMeshActionStructure>.Enumerator enumerator3 = meshAssignments.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure4 = enumerator3.Current;
						if (customMeshActionStructure4 != null)
						{
							customMeshActionStructure4.action.Invoke();
						}
					}
					goto IL_981;
				}
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair2 in objectMeshPairs)
			{
				GameObject key = keyValuePair2.Key;
				if (!(key == null))
				{
					PolyfewRuntime.MeshRendererPair value = keyValuePair2.Value;
					if (!(value.mesh == null))
					{
						MeshSimplifier meshSimplifier2 = new MeshSimplifier();
						PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier2);
						ToleranceSphere[] array2 = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
						if (!value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							meshSimplifier2.isSkinned = true;
							SkinnedMeshRenderer component2 = key.GetComponent<SkinnedMeshRenderer>();
							meshSimplifier2.boneWeightsOriginal = value.mesh.boneWeights;
							meshSimplifier2.bindPosesOriginal = value.mesh.bindposes;
							meshSimplifier2.bonesOriginal = component2.bones;
							int num4 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere3 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere3 = new ToleranceSphere
								{
									diameter = preservationSphere3.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere3.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere3.preservationStrength
								};
								array2[num4] = toleranceSphere3;
								num4++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						else if (value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							int num5 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere4 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere4 = new ToleranceSphere
								{
									diameter = preservationSphere4.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere4.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere4.preservationStrength
								};
								array2[num5] = toleranceSphere4;
								num5++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						meshSimplifier2.Initialize(value.mesh, simplificationOptions.regardPreservationSpheres);
						if (!simplificationOptions.simplifyMeshLossless)
						{
							meshSimplifier2.SimplifyMesh(quality);
						}
						else
						{
							meshSimplifier2.SimplifyMeshLossless();
						}
						if (OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified.Invoke(key, value);
						}
						Mesh mesh = meshSimplifier2.ToMesh();
						mesh.bindposes = value.mesh.bindposes;
						mesh.name = value.mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
						if (meshSimplifier2.RecalculateNormals)
						{
							mesh.RecalculateNormals();
							mesh.RecalculateTangents();
						}
						if (value.attachedToMeshFilter)
						{
							MeshFilter component3 = key.GetComponent<MeshFilter>();
							if (component3 != null)
							{
								component3.sharedMesh = mesh;
							}
						}
						else
						{
							SkinnedMeshRenderer component4 = key.GetComponent<SkinnedMeshRenderer>();
							if (component4 != null)
							{
								component4.sharedMesh = mesh;
							}
						}
						totalTriangles += mesh.triangles.Length / 3;
					}
				}
			}
			IL_981:
			return totalTriangles;
		}

		// Token: 0x06003278 RID: 12920 RVA: 0x0022861C File Offset: 0x0022681C
		public static List<Mesh> SimplifyMeshes(List<Mesh> meshesToSimplify, PolyfewRuntime.SimplificationOptions simplificationOptions, Action<Mesh> OnEachMeshSimplified)
		{
			List<Mesh> simplifiedMeshes = new List<Mesh>();
			if (simplificationOptions == null)
			{
				throw new ArgumentNullException("simplificationOptions", "You must provide a SimplificationOptions object.");
			}
			float simplificationStrength = simplificationOptions.simplificationStrength;
			if (meshesToSimplify == null)
			{
				throw new ArgumentNullException("meshesToSimplify", "You must provide a meshes list to simplify.");
			}
			if (meshesToSimplify.Count == 0)
			{
				throw new InvalidOperationException("You must provide a non-empty list of meshes to simplify.");
			}
			if (!simplificationOptions.simplifyMeshLossless)
			{
				if (simplificationStrength < 0f || simplificationStrength > 100f)
				{
					throw new ArgumentOutOfRangeException("simplificationStrength", "The allowed values for simplification strength are between [0-100] inclusive.");
				}
				if (Mathf.Approximately(simplificationStrength, 0f))
				{
					return null;
				}
			}
			if (PolyfewRuntime.CountTriangles(meshesToSimplify) >= 2000)
			{
				int count = meshesToSimplify.Count;
			}
			RuntimePlatform platform = Application.platform;
			float quality = 1f - simplificationStrength / 100f;
			int count2 = meshesToSimplify.Count;
			int meshesHandled = 0;
			int threadsRunning = 0;
			bool isError = false;
			string error = "";
			object threadLock1 = new object();
			object threadLock2 = new object();
			object threadLock3 = new object();
			if (true)
			{
				List<PolyfewRuntime.CustomMeshActionStructure> meshAssignments = new List<PolyfewRuntime.CustomMeshActionStructure>();
				List<PolyfewRuntime.CustomMeshActionStructure> callbackFlusher = new List<PolyfewRuntime.CustomMeshActionStructure>();
				using (List<Mesh>.Enumerator enumerator = meshesToSimplify.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Mesh meshToSimplify = enumerator.Current;
						if (meshToSimplify == null)
						{
							int num = meshesHandled;
							meshesHandled = num + 1;
						}
						else
						{
							MeshSimplifier meshSimplifier = new MeshSimplifier();
							PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier);
							meshSimplifier.Initialize(meshToSimplify, false);
							int num = threadsRunning;
							threadsRunning = num + 1;
							while (callbackFlusher.Count > 0)
							{
								PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure = callbackFlusher[0];
								callbackFlusher.RemoveAt(0);
								if (OnEachMeshSimplified != null)
								{
									OnEachMeshSimplified.Invoke(customMeshActionStructure.meshRendererPair.mesh);
								}
							}
							Action <>9__1;
							Task.Factory.StartNew(delegate()
							{
								PolyfewRuntime.MeshRendererPair meshRendererPair = null;
								GameObject gameObject = null;
								Action action;
								if ((action = <>9__1) == null)
								{
									action = (<>9__1 = delegate()
									{
										Mesh mesh3 = meshSimplifier.ToMesh();
										mesh3.bindposes = meshToSimplify.bindposes;
										mesh3.name = meshToSimplify.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
										if (meshSimplifier.RecalculateNormals)
										{
											mesh3.RecalculateNormals();
											mesh3.RecalculateTangents();
										}
										simplifiedMeshes.Add(mesh3);
									});
								}
								PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure5 = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, action);
								try
								{
									if (!simplificationOptions.simplifyMeshLossless)
									{
										meshSimplifier.SimplifyMesh(quality);
									}
									else
									{
										meshSimplifier.SimplifyMeshLossless();
									}
									object obj = threadLock1;
									lock (obj)
									{
										meshAssignments.Add(customMeshActionStructure5);
										int num2 = threadsRunning;
										threadsRunning = num2 - 1;
										num2 = meshesHandled;
										meshesHandled = num2 + 1;
									}
									obj = threadLock3;
									lock (obj)
									{
										PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure6 = new PolyfewRuntime.CustomMeshActionStructure(new PolyfewRuntime.MeshRendererPair(true, meshToSimplify), null, delegate
										{
										});
										callbackFlusher.Add(customMeshActionStructure6);
									}
								}
								catch (Exception ex)
								{
									object obj = threadLock2;
									lock (obj)
									{
										int num2 = threadsRunning;
										threadsRunning = num2 - 1;
										num2 = meshesHandled;
										meshesHandled = num2 + 1;
										isError = true;
										error = ex.ToString();
									}
								}
							}, CancellationToken.None, 64, TaskScheduler.Current);
						}
					}
					goto IL_2A5;
				}
				IL_276:
				PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure2 = callbackFlusher[0];
				callbackFlusher.RemoveAt(0);
				if (OnEachMeshSimplified != null)
				{
					OnEachMeshSimplified.Invoke(customMeshActionStructure2.meshRendererPair.mesh);
				}
				IL_2A5:
				if (callbackFlusher.Count > 0)
				{
					goto IL_276;
				}
				while (meshesHandled < count2 && !isError)
				{
					while (callbackFlusher.Count > 0)
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure3 = callbackFlusher[0];
						callbackFlusher.RemoveAt(0);
						if (OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified.Invoke(customMeshActionStructure3.meshRendererPair.mesh);
						}
					}
				}
				if (isError)
				{
					goto IL_43C;
				}
				using (List<PolyfewRuntime.CustomMeshActionStructure>.Enumerator enumerator2 = meshAssignments.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure4 = enumerator2.Current;
						if (customMeshActionStructure4 != null)
						{
							customMeshActionStructure4.action.Invoke();
						}
					}
					goto IL_43C;
				}
			}
			foreach (Mesh mesh in meshesToSimplify)
			{
				if (!(mesh == null))
				{
					MeshSimplifier meshSimplifier2 = new MeshSimplifier();
					PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier2);
					meshSimplifier2.Initialize(mesh, false);
					if (!simplificationOptions.simplifyMeshLossless)
					{
						meshSimplifier2.SimplifyMesh(quality);
					}
					else
					{
						meshSimplifier2.SimplifyMeshLossless();
					}
					if (OnEachMeshSimplified != null)
					{
						OnEachMeshSimplified.Invoke(mesh);
					}
					Mesh mesh2 = meshSimplifier2.ToMesh();
					mesh2.bindposes = mesh.bindposes;
					mesh2.name = mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
					if (meshSimplifier2.RecalculateNormals)
					{
						mesh2.RecalculateNormals();
						mesh2.RecalculateTangents();
					}
					simplifiedMeshes.Add(mesh2);
				}
			}
			IL_43C:
			return simplifiedMeshes;
		}

		// Token: 0x06003279 RID: 12921 RVA: 0x00228AB8 File Offset: 0x00226CB8
		public static PolyfewRuntime.ObjectMeshPairs GetObjectMeshPairs(GameObject forObject, bool includeInactive)
		{
			if (forObject == null)
			{
				throw new ArgumentNullException("forObject", "You must provide a gameobject to get the ObjectMeshPairs for.");
			}
			PolyfewRuntime.ObjectMeshPairs objectMeshPairs = new PolyfewRuntime.ObjectMeshPairs();
			MeshFilter[] componentsInChildren = forObject.GetComponentsInChildren<MeshFilter>(includeInactive);
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				foreach (MeshFilter meshFilter in componentsInChildren)
				{
					if (meshFilter.sharedMesh)
					{
						PolyfewRuntime.MeshRendererPair meshRendererPair = new PolyfewRuntime.MeshRendererPair(true, meshFilter.sharedMesh);
						objectMeshPairs.Add(meshFilter.gameObject, meshRendererPair);
					}
				}
			}
			SkinnedMeshRenderer[] componentsInChildren2 = forObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
			if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
				{
					if (skinnedMeshRenderer.sharedMesh)
					{
						PolyfewRuntime.MeshRendererPair meshRendererPair2 = new PolyfewRuntime.MeshRendererPair(false, skinnedMeshRenderer.sharedMesh);
						objectMeshPairs.Add(skinnedMeshRenderer.gameObject, meshRendererPair2);
					}
				}
			}
			return objectMeshPairs;
		}

		// Token: 0x0600327A RID: 12922 RVA: 0x00228B94 File Offset: 0x00226D94
		public static void CombineMeshesInGameObject(GameObject forObject, bool skipInactiveRenderers, Action<string, string> OnError, PolyfewRuntime.MeshCombineTarget combineTarget = PolyfewRuntime.MeshCombineTarget.SkinnedAndStatic)
		{
			if (forObject == null)
			{
				if (OnError != null)
				{
					OnError.Invoke("Argument Null Exception", "You must provide a gameobject whose meshes will be combined.");
				}
				return;
			}
			Renderer[] childRenderersForCombining = UtilityServicesRuntime.GetChildRenderersForCombining(forObject, skipInactiveRenderers);
			if (childRenderersForCombining == null || childRenderersForCombining.Length == 0)
			{
				if (OnError != null)
				{
					OnError.Invoke("Operation Failed", "No feasible renderers found under the provided object to combine.");
				}
				return;
			}
			HashSet<Transform> hashSet = new HashSet<Transform>();
			HashSet<Transform> hashSet2 = new HashSet<Transform>();
			MeshRenderer[] array;
			SkinnedMeshRenderer[] array2;
			if (skipInactiveRenderers)
			{
				array = Enumerable.ToArray<MeshRenderer>(Enumerable.Select<Renderer, MeshRenderer>(Enumerable.Where<Renderer>(childRenderersForCombining, (Renderer renderer) => renderer.enabled && renderer.gameObject.activeInHierarchy && renderer as MeshRenderer != null && renderer.transform.GetComponent<MeshFilter>() != null && renderer.transform.GetComponent<MeshFilter>().sharedMesh != null), (Renderer renderer) => renderer as MeshRenderer));
				array2 = Enumerable.ToArray<SkinnedMeshRenderer>(Enumerable.Select<Renderer, SkinnedMeshRenderer>(Enumerable.Where<Renderer>(childRenderersForCombining, (Renderer renderer) => renderer.enabled && renderer.gameObject.activeInHierarchy && renderer as SkinnedMeshRenderer != null && renderer.transform.GetComponent<SkinnedMeshRenderer>().sharedMesh != null), (Renderer renderer) => renderer as SkinnedMeshRenderer));
			}
			else
			{
				array = Enumerable.ToArray<MeshRenderer>(Enumerable.Select<Renderer, MeshRenderer>(Enumerable.Where<Renderer>(childRenderersForCombining, (Renderer renderer) => renderer as MeshRenderer != null && renderer.transform.GetComponent<MeshFilter>() != null && renderer.transform.GetComponent<MeshFilter>().sharedMesh != null), (Renderer renderer) => renderer as MeshRenderer));
				array2 = Enumerable.ToArray<SkinnedMeshRenderer>(Enumerable.Select<Renderer, SkinnedMeshRenderer>(Enumerable.Where<Renderer>(childRenderersForCombining, (Renderer renderer) => renderer as SkinnedMeshRenderer != null && renderer.transform.GetComponent<SkinnedMeshRenderer>().sharedMesh != null), (Renderer renderer) => renderer as SkinnedMeshRenderer));
			}
			if (array != null)
			{
				foreach (MeshRenderer meshRenderer in array)
				{
					hashSet.Add(meshRenderer.transform);
				}
			}
			if (array2 != null)
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
				{
					hashSet2.Add(skinnedMeshRenderer.transform);
				}
			}
			if (combineTarget == PolyfewRuntime.MeshCombineTarget.StaticOnly)
			{
				array2 = new SkinnedMeshRenderer[0];
			}
			else if (combineTarget == PolyfewRuntime.MeshCombineTarget.SkinnedOnly)
			{
				array = new MeshRenderer[0];
			}
			MeshCombiner.StaticRenderer[] staticRenderers = MeshCombiner.GetStaticRenderers(array);
			MeshCombiner.SkinnedRenderer[] skinnedRenderers = MeshCombiner.GetSkinnedRenderers(array2);
			int num = Enumerable.Count<SkinnedMeshRenderer>(Enumerable.Where<SkinnedMeshRenderer>(array2, (SkinnedMeshRenderer renderer) => renderer.sharedMesh != null));
			int num2 = (staticRenderers == null) ? 0 : staticRenderers.Length;
			if (skinnedRenderers != null)
			{
				int num3 = skinnedRenderers.Length;
			}
			if ((num2 == 0 || num2 == 1) && (num == 0 || num == 1))
			{
				string text = "Nothing combined in GameObject \"" + forObject.name + "\". Not enough feasible renderers/meshes to combine.";
				if (combineTarget == PolyfewRuntime.MeshCombineTarget.StaticOnly)
				{
					text = "Nothing combined in GameObject \"" + forObject.name + "\". Not enough feasible static meshes to combine. Consider selecting the option of combining both skinned and static meshes.";
				}
				else if (combineTarget == PolyfewRuntime.MeshCombineTarget.SkinnedOnly)
				{
					text = "Nothing combined in GameObject \"" + forObject.name + "\". Not enough feasible skinned meshes to combine. Consider selecting the option of combining both skinned and static meshes.";
				}
				if (OnError != null)
				{
					OnError.Invoke("Operation Failed", text);
				}
				return;
			}
			SkinnedMeshRenderer[] array5 = null;
			MeshCombiner.StaticRenderer[] array6 = MeshCombiner.CombineStaticMeshes(forObject.transform, -1, array, false, "");
			MeshCombiner.SkinnedRenderer[] array7 = MeshCombiner.CombineSkinnedMeshes(forObject.transform, -1, array2, ref array5, false, "");
			if (array5 != null)
			{
				SkinnedMeshRenderer[] array4 = array5;
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i].enabled = false;
				}
			}
			if (array != null)
			{
				MeshRenderer[] array3 = array;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].enabled = false;
				}
			}
			int num4 = (array6 == null) ? 0 : array6.Length;
			int num5 = (array7 == null) ? 0 : array7.Length;
			Transform transform = forObject.transform;
			HashSet<Transform> hashSet3 = new HashSet<Transform>();
			HashSet<Transform> hashSet4 = new HashSet<Transform>();
			for (int j = 0; j < num4; j++)
			{
				MeshCombiner.StaticRenderer staticRenderer = array6[j];
				Mesh mesh = staticRenderer.mesh;
				MeshRenderer meshRenderer2 = UtilityServicesRuntime.CreateStaticLevelRenderer(string.Format("{0}_combined_static", staticRenderer.name.Replace("_combined", "")), transform, staticRenderer.transform, mesh, staticRenderer.materials);
				hashSet3.Add(meshRenderer2.transform);
				meshRenderer2.transform.parent = forObject.transform;
			}
			for (int k = 0; k < num5; k++)
			{
				MeshCombiner.SkinnedRenderer skinnedRenderer = array7[k];
				Mesh mesh2 = skinnedRenderer.mesh;
				SkinnedMeshRenderer skinnedMeshRenderer2 = UtilityServicesRuntime.CreateSkinnedLevelRenderer(string.Format("{0}_combined_skinned", skinnedRenderer.name.Replace("_combined", "")), transform, skinnedRenderer.transform, mesh2, skinnedRenderer.materials, skinnedRenderer.rootBone, skinnedRenderer.bones);
				hashSet4.Add(skinnedMeshRenderer2.transform);
				skinnedMeshRenderer2.transform.parent = forObject.transform;
			}
			GameObject gameObject = new GameObject(forObject.name + "_bonesHiererachy");
			gameObject.transform.parent = forObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			Transform[] array8 = new Transform[forObject.transform.childCount];
			for (int l = 0; l < forObject.transform.childCount; l++)
			{
				array8[l] = forObject.transform.GetChild(l);
			}
			foreach (Transform transform2 in array8)
			{
				if (combineTarget == PolyfewRuntime.MeshCombineTarget.SkinnedAndStatic)
				{
					if (!hashSet4.Contains(transform2) && !hashSet3.Contains(transform2))
					{
						transform2.parent = gameObject.transform;
					}
				}
				else if (combineTarget == PolyfewRuntime.MeshCombineTarget.StaticOnly)
				{
					if (!hashSet3.Contains(transform2) && !hashSet2.Contains(transform2))
					{
						transform2.parent = gameObject.transform;
					}
				}
				else if (!hashSet4.Contains(transform2) && !hashSet.Contains(transform2))
				{
					transform2.parent = gameObject.transform;
				}
			}
			if (array5 != null)
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer3 in array5)
				{
					if (!(skinnedMeshRenderer3 == null))
					{
						skinnedMeshRenderer3.sharedMesh = null;
					}
				}
			}
			if (array != null)
			{
				foreach (MeshRenderer meshRenderer3 in array)
				{
					if (!(meshRenderer3 == null))
					{
						MeshFilter component = meshRenderer3.GetComponent<MeshFilter>();
						if (!(component == null))
						{
							component.sharedMesh = null;
						}
					}
				}
			}
		}

		// Token: 0x0600327B RID: 12923 RVA: 0x002291DC File Offset: 0x002273DC
		public static GameObject CombineMeshesFromRenderers(Transform rootTransform, MeshRenderer[] originalMeshRenderers, SkinnedMeshRenderer[] originalSkinnedMeshRenderers, Action<string, string> OnError)
		{
			if (rootTransform == null)
			{
				if (OnError != null)
				{
					OnError.Invoke("Argument Null Exception", "You must provide a root transform to create the combined meshes based from.");
				}
				return null;
			}
			if ((originalMeshRenderers == null || originalMeshRenderers.Length == 0) && (originalSkinnedMeshRenderers == null || originalSkinnedMeshRenderers.Length == 0))
			{
				if (OnError != null)
				{
					OnError.Invoke("Operation Failed", "Both the Static and Skinned renderers list is empty. Atleast one of them must be non empty.");
				}
				return null;
			}
			if (originalMeshRenderers == null)
			{
				originalMeshRenderers = new MeshRenderer[0];
			}
			if (originalSkinnedMeshRenderers == null)
			{
				originalSkinnedMeshRenderers = new SkinnedMeshRenderer[0];
			}
			originalMeshRenderers = Enumerable.ToArray<MeshRenderer>(Enumerable.Select<MeshRenderer, MeshRenderer>(Enumerable.Where<MeshRenderer>(originalMeshRenderers, (MeshRenderer renderer) => renderer.transform.GetComponent<MeshFilter>() != null && renderer.transform.GetComponent<MeshFilter>().sharedMesh != null), (MeshRenderer renderer) => renderer));
			originalSkinnedMeshRenderers = Enumerable.ToArray<SkinnedMeshRenderer>(Enumerable.Select<SkinnedMeshRenderer, SkinnedMeshRenderer>(Enumerable.Where<SkinnedMeshRenderer>(originalSkinnedMeshRenderers, (SkinnedMeshRenderer renderer) => renderer.transform.GetComponent<SkinnedMeshRenderer>().sharedMesh != null), (SkinnedMeshRenderer renderer) => renderer));
			if ((originalMeshRenderers == null || originalMeshRenderers.Length == 0) && (originalSkinnedMeshRenderers == null || originalSkinnedMeshRenderers.Length == 0))
			{
				if (OnError != null)
				{
					OnError.Invoke("Operation Failed", "Couldn't combine any meshes. Couldn't find any feasible renderers in the provided lists to combine.");
				}
				return null;
			}
			SkinnedMeshRenderer[] array = null;
			MeshCombiner.StaticRenderer[] array2 = MeshCombiner.CombineStaticMeshes(rootTransform, -1, originalMeshRenderers, false, "");
			MeshCombiner.SkinnedRenderer[] array3 = MeshCombiner.CombineSkinnedMeshes(rootTransform, -1, originalSkinnedMeshRenderers, ref array, false, "");
			if ((array2 == null || array2.Length == 0) && (array3 == null || array3.Length == 0))
			{
				if (OnError != null)
				{
					OnError.Invoke("Operation Failed", "Couldn't combine any meshes due to unknown reasons.");
				}
				return null;
			}
			GameObject gameObject = new GameObject(rootTransform.name + "_Combined_Meshes");
			Transform transform = gameObject.transform;
			if (array2 != null)
			{
				foreach (MeshCombiner.StaticRenderer staticRenderer in array2)
				{
					Mesh mesh = staticRenderer.mesh;
					UtilityServicesRuntime.CreateStaticLevelRenderer(string.Format("{0}_combined_static", staticRenderer.name.Replace("_combined", "")), transform, staticRenderer.transform, mesh, staticRenderer.materials);
				}
			}
			if (array3 != null)
			{
				foreach (MeshCombiner.SkinnedRenderer skinnedRenderer in array3)
				{
					Mesh mesh2 = skinnedRenderer.mesh;
					UtilityServicesRuntime.CreateSkinnedLevelRenderer(string.Format("{0}_combined_skinned", skinnedRenderer.name.Replace("_combined", "")), transform, skinnedRenderer.transform, mesh2, skinnedRenderer.materials, skinnedRenderer.rootBone, skinnedRenderer.bones);
				}
			}
			return gameObject;
		}

		// Token: 0x0600327C RID: 12924 RVA: 0x00229438 File Offset: 0x00227638
		public static void ConvertSkinnedMeshesInGameObject(GameObject forObject, bool skipInactiveRenderers, Action<string, string> OnError)
		{
			if (forObject == null)
			{
				if (OnError != null)
				{
					OnError.Invoke("Argument Null Exception", "You must provide a gameobject whose meshes will be converted.");
				}
				return;
			}
			SkinnedMeshRenderer[] array = forObject.GetComponentsInChildren<SkinnedMeshRenderer>(!skipInactiveRenderers);
			if (skipInactiveRenderers)
			{
				array = Enumerable.ToArray<SkinnedMeshRenderer>(Enumerable.Where<SkinnedMeshRenderer>(array, (SkinnedMeshRenderer renderer) => renderer.enabled && renderer.gameObject.activeInHierarchy && renderer.sharedMesh != null));
			}
			else
			{
				array = Enumerable.ToArray<SkinnedMeshRenderer>(Enumerable.Where<SkinnedMeshRenderer>(array, (SkinnedMeshRenderer renderer) => renderer.sharedMesh != null));
			}
			if (array == null || array.Length == 0)
			{
				if (OnError != null)
				{
					OnError.Invoke("Operation Failed", "Failed to convert skinned meshes for the provided GameObject. No feasible skinned mesh renderer found in the GameObject or any of the nested children to convert.");
				}
				return;
			}
			int num = 0;
			Mesh[] array2 = new Mesh[array.Length];
			List<GameObject> list = new List<GameObject>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				Mesh mesh = new Mesh();
				mesh.name = skinnedMeshRenderer.sharedMesh.name + "-Skinned_Converted_Mesh";
				skinnedMeshRenderer.BakeMesh(mesh);
				Vector3[] vertices = mesh.vertices;
				Vector3[] normals = mesh.normals;
				float x = skinnedMeshRenderer.transform.lossyScale.x;
				float y = skinnedMeshRenderer.transform.lossyScale.y;
				float z = skinnedMeshRenderer.transform.lossyScale.z;
				for (int j = 0; j < vertices.Length; j++)
				{
					vertices[j] = new Vector3(vertices[j].x / x, vertices[j].y / y, vertices[j].z / z);
					normals[j] = new Vector3(normals[j].x / x, normals[j].y / y, normals[j].z / z);
				}
				mesh.vertices = vertices;
				mesh.normals = normals;
				mesh.RecalculateBounds();
				Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
				Transform rootBone = skinnedMeshRenderer.rootBone;
				if (rootBone != null && rootBone.parent != null && rootBone.parent.gameObject.GetHashCode() != skinnedMeshRenderer.gameObject.GetHashCode())
				{
					list.Add(rootBone.parent.gameObject);
				}
				GameObject gameObject = skinnedMeshRenderer.gameObject;
				Object.DestroyImmediate(skinnedMeshRenderer);
				gameObject.AddComponent<MeshFilter>().mesh = mesh;
				gameObject.AddComponent<MeshRenderer>().sharedMaterials = sharedMaterials;
				array2[num - 1] = mesh;
			}
			foreach (GameObject gameObject2 in list)
			{
				Object.DestroyImmediate(gameObject2);
			}
		}

		// Token: 0x0600327D RID: 12925 RVA: 0x00229710 File Offset: 0x00227910
		public static Tuple<SkinnedMeshRenderer, MeshRenderer, Mesh>[] ConvertSkinnedMeshesFromRenderers(SkinnedMeshRenderer[] renderersToConvert, Action<string, string> OnError)
		{
			if (renderersToConvert == null)
			{
				if (OnError != null)
				{
					OnError.Invoke("Argument Null Exception", "You must provide a List of Skinned Mesh Renders to convert.");
				}
				return null;
			}
			if (renderersToConvert.Length == 0)
			{
				if (OnError != null)
				{
					OnError.Invoke("Operation Failed", "The list of Skinned Mesh Renders to convert must not be empty.");
				}
				return null;
			}
			renderersToConvert = Enumerable.ToArray<SkinnedMeshRenderer>(Enumerable.Where<SkinnedMeshRenderer>(renderersToConvert, (SkinnedMeshRenderer renderer) => renderer.sharedMesh != null));
			if (renderersToConvert == null || renderersToConvert.Length == 0)
			{
				if (OnError != null)
				{
					OnError.Invoke("Operation Failed", "Failed to convert skinned meshes. No feasible skinned mesh renderer found in the provided list to convert.");
				}
				return null;
			}
			Tuple<SkinnedMeshRenderer, MeshRenderer, Mesh>[] array = new Tuple<SkinnedMeshRenderer, MeshRenderer, Mesh>[renderersToConvert.Length];
			int num = 0;
			GameObject gameObject = new GameObject();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in renderersToConvert)
			{
				Mesh mesh = new Mesh();
				mesh.name = skinnedMeshRenderer.sharedMesh.name + (skinnedMeshRenderer.sharedMesh.name.EndsWith("-") ? "Skinned_Converted_Mesh" : "-Skinned_Converted_Mesh");
				skinnedMeshRenderer.BakeMesh(mesh);
				Vector3[] vertices = mesh.vertices;
				Vector3[] normals = mesh.normals;
				float x = skinnedMeshRenderer.transform.lossyScale.x;
				float y = skinnedMeshRenderer.transform.lossyScale.y;
				float z = skinnedMeshRenderer.transform.lossyScale.z;
				for (int j = 0; j < vertices.Length; j++)
				{
					vertices[j] = new Vector3(vertices[j].x / x, vertices[j].y / y, vertices[j].z / z);
					normals[j] = new Vector3(normals[j].x / x, normals[j].y / y, normals[j].z / z);
				}
				mesh.vertices = vertices;
				mesh.normals = normals;
				mesh.RecalculateBounds();
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;
				array[num] = Tuple.Create<SkinnedMeshRenderer, MeshRenderer, Mesh>(skinnedMeshRenderer, meshRenderer, mesh);
				num++;
			}
			Object.DestroyImmediate(gameObject);
			return array;
		}

		// Token: 0x0600327E RID: 12926 RVA: 0x00229944 File Offset: 0x00227B44
		public static void ImportOBJFromFileSystem(string objAbsolutePath, string texturesFolderPath, string materialsFolderPath, Action<GameObject> OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJImportOptions importOptions = null)
		{
			PolyfewRuntime.<ImportOBJFromFileSystem>d__20 <ImportOBJFromFileSystem>d__;
			<ImportOBJFromFileSystem>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ImportOBJFromFileSystem>d__.objAbsolutePath = objAbsolutePath;
			<ImportOBJFromFileSystem>d__.texturesFolderPath = texturesFolderPath;
			<ImportOBJFromFileSystem>d__.materialsFolderPath = materialsFolderPath;
			<ImportOBJFromFileSystem>d__.OnSuccess = OnSuccess;
			<ImportOBJFromFileSystem>d__.OnError = OnError;
			<ImportOBJFromFileSystem>d__.importOptions = importOptions;
			<ImportOBJFromFileSystem>d__.<>1__state = -1;
			<ImportOBJFromFileSystem>d__.<>t__builder.Start<PolyfewRuntime.<ImportOBJFromFileSystem>d__20>(ref <ImportOBJFromFileSystem>d__);
		}

		// Token: 0x0600327F RID: 12927 RVA: 0x002299A8 File Offset: 0x00227BA8
		public static void ImportOBJFromNetwork(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<GameObject> OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJImportOptions importOptions = null)
		{
			PolyfewRuntime.<ImportOBJFromNetwork>d__21 <ImportOBJFromNetwork>d__;
			<ImportOBJFromNetwork>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ImportOBJFromNetwork>d__.objURL = objURL;
			<ImportOBJFromNetwork>d__.objName = objName;
			<ImportOBJFromNetwork>d__.diffuseTexURL = diffuseTexURL;
			<ImportOBJFromNetwork>d__.bumpTexURL = bumpTexURL;
			<ImportOBJFromNetwork>d__.specularTexURL = specularTexURL;
			<ImportOBJFromNetwork>d__.opacityTexURL = opacityTexURL;
			<ImportOBJFromNetwork>d__.materialURL = materialURL;
			<ImportOBJFromNetwork>d__.downloadProgress = downloadProgress;
			<ImportOBJFromNetwork>d__.OnSuccess = OnSuccess;
			<ImportOBJFromNetwork>d__.OnError = OnError;
			<ImportOBJFromNetwork>d__.importOptions = importOptions;
			<ImportOBJFromNetwork>d__.<>1__state = -1;
			<ImportOBJFromNetwork>d__.<>t__builder.Start<PolyfewRuntime.<ImportOBJFromNetwork>d__21>(ref <ImportOBJFromNetwork>d__);
		}

		// Token: 0x06003280 RID: 12928 RVA: 0x00229A38 File Offset: 0x00227C38
		public static void ExportGameObjectToOBJ(GameObject toExport, string exportPath, Action OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJExportOptions exportOptions = null)
		{
			PolyfewRuntime.<ExportGameObjectToOBJ>d__22 <ExportGameObjectToOBJ>d__;
			<ExportGameObjectToOBJ>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ExportGameObjectToOBJ>d__.toExport = toExport;
			<ExportGameObjectToOBJ>d__.exportPath = exportPath;
			<ExportGameObjectToOBJ>d__.OnSuccess = OnSuccess;
			<ExportGameObjectToOBJ>d__.OnError = OnError;
			<ExportGameObjectToOBJ>d__.exportOptions = exportOptions;
			<ExportGameObjectToOBJ>d__.<>1__state = -1;
			<ExportGameObjectToOBJ>d__.<>t__builder.Start<PolyfewRuntime.<ExportGameObjectToOBJ>d__22>(ref <ExportGameObjectToOBJ>d__);
		}

		// Token: 0x06003281 RID: 12929 RVA: 0x00229A90 File Offset: 0x00227C90
		public static int CountTriangles(bool countDeep, GameObject forObject)
		{
			int num = 0;
			if (forObject == null)
			{
				return 0;
			}
			if (countDeep)
			{
				MeshFilter[] componentsInChildren = forObject.GetComponentsInChildren<MeshFilter>(true);
				if (componentsInChildren != null && componentsInChildren.Length != 0)
				{
					foreach (MeshFilter meshFilter in componentsInChildren)
					{
						if (meshFilter.sharedMesh)
						{
							num += meshFilter.sharedMesh.triangles.Length / 3;
						}
					}
				}
				SkinnedMeshRenderer[] componentsInChildren2 = forObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
				{
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
					{
						if (skinnedMeshRenderer.sharedMesh)
						{
							num += skinnedMeshRenderer.sharedMesh.triangles.Length / 3;
						}
					}
				}
			}
			else
			{
				MeshFilter component = forObject.GetComponent<MeshFilter>();
				SkinnedMeshRenderer component2 = forObject.GetComponent<SkinnedMeshRenderer>();
				if (component && component.sharedMesh)
				{
					num = component.sharedMesh.triangles.Length / 3;
				}
				else if (component2 && component2.sharedMesh)
				{
					num = component2.sharedMesh.triangles.Length / 3;
				}
			}
			return num;
		}

		// Token: 0x06003282 RID: 12930 RVA: 0x00229BB4 File Offset: 0x00227DB4
		public static int CountTriangles(List<Mesh> toCount)
		{
			int num = 0;
			if (toCount == null || toCount.Count == 0)
			{
				return 0;
			}
			foreach (Mesh mesh in toCount)
			{
				if (mesh != null)
				{
					num += mesh.triangles.Length / 3;
				}
			}
			return num;
		}

		// Token: 0x06003283 RID: 12931 RVA: 0x00229C24 File Offset: 0x00227E24
		public static List<PolyfewRuntime.MaterialProperties> GetMaterialsProperties(GameObject forObject)
		{
			if (forObject == null)
			{
				throw new ArgumentNullException("Argument Null Exception", "You must provide a GameObject whose material properties you want to change");
			}
			ObjectMaterialLinks component = forObject.GetComponent<ObjectMaterialLinks>();
			if (component == null)
			{
				throw new InvalidOperationException("The object whose material properties you're trying to combine doesn't have any materials combined with Batch Few");
			}
			Texture2D linkedAttrImg = component.linkedAttrImg;
			if (component == null)
			{
				throw new InvalidOperationException("There is no attributes image associated with the given object");
			}
			List<PolyfewRuntime.MaterialProperties> materialsProperties = component.materialsProperties;
			List<PolyfewRuntime.MaterialProperties> list = new List<PolyfewRuntime.MaterialProperties>();
			foreach (PolyfewRuntime.MaterialProperties materialProperties in materialsProperties)
			{
				PolyfewRuntime.MaterialProperties materialProperties2 = new PolyfewRuntime.MaterialProperties(materialProperties.texArrIndex, materialProperties.matIndex, materialProperties.materialName, materialProperties.originalMaterial, materialProperties.albedoTint, materialProperties.uvTileOffset, materialProperties.normalIntensity, materialProperties.occlusionIntensity, materialProperties.smoothnessIntensity, materialProperties.glossMapScale, materialProperties.metalIntensity, materialProperties.emissionColor, materialProperties.detailUVTileOffset, materialProperties.alphaCutoff, materialProperties.specularColor, materialProperties.detailNormalScale, materialProperties.heightIntensity, materialProperties.uvSec);
				list.Add(materialProperties2);
			}
			return list;
		}

		// Token: 0x06003284 RID: 12932 RVA: 0x00229D44 File Offset: 0x00227F44
		public static void ChangeMaterialProperties(PolyfewRuntime.MaterialProperties changeTo, GameObject forObject)
		{
			if (forObject == null)
			{
				return;
			}
			if (changeTo == null)
			{
				return;
			}
			Texture2D linkedAttrImg = forObject.GetComponent<ObjectMaterialLinks>().linkedAttrImg;
			if (linkedAttrImg == null)
			{
				return;
			}
			int texArrIndex = changeTo.texArrIndex;
			int matIndex = changeTo.matIndex;
			changeTo.BurnAttrToImg(ref linkedAttrImg, matIndex, texArrIndex);
		}

		// Token: 0x06003285 RID: 12933 RVA: 0x00229D90 File Offset: 0x00227F90
		private static void SetParametersForSimplifier(PolyfewRuntime.SimplificationOptions simplificationOptions, MeshSimplifier meshSimplifier)
		{
			meshSimplifier.RecalculateNormals = simplificationOptions.recalculateNormals;
			meshSimplifier.EnableSmartLink = simplificationOptions.enableSmartlinking;
			meshSimplifier.PreserveUVSeamEdges = simplificationOptions.preserveUVSeamEdges;
			meshSimplifier.PreserveUVFoldoverEdges = simplificationOptions.preserveUVFoldoverEdges;
			meshSimplifier.PreserveBorderEdges = simplificationOptions.preserveBorderEdges;
			meshSimplifier.MaxIterationCount = simplificationOptions.maxIterations;
			meshSimplifier.Aggressiveness = (double)simplificationOptions.aggressiveness;
			meshSimplifier.RegardCurvature = simplificationOptions.regardCurvature;
			meshSimplifier.UseSortedEdgeMethod = simplificationOptions.useEdgeSort;
		}

		// Token: 0x06003286 RID: 12934 RVA: 0x00229E0C File Offset: 0x0022800C
		private static bool AreAnyFeasibleMeshes(PolyfewRuntime.ObjectMeshPairs objectMeshPairs)
		{
			if (objectMeshPairs == null || objectMeshPairs.Count == 0)
			{
				return false;
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair in objectMeshPairs)
			{
				PolyfewRuntime.MeshRendererPair value = keyValuePair.Value;
				GameObject key = keyValuePair.Key;
				if (!(key == null) && value != null)
				{
					if (value.attachedToMeshFilter)
					{
						if (!(key.GetComponent<MeshFilter>() == null) && !(value.mesh == null))
						{
							return true;
						}
					}
					else if (!value.attachedToMeshFilter && !(key.GetComponent<SkinnedMeshRenderer>() == null) && !(value.mesh == null))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003287 RID: 12935 RVA: 0x00229ED4 File Offset: 0x002280D4
		private static void AssignReducedMesh(GameObject gameObject, Mesh originalMesh, Mesh reducedMesh, bool attachedToMeshfilter, bool assignBindposes)
		{
			if (assignBindposes)
			{
				reducedMesh.bindposes = originalMesh.bindposes;
			}
			reducedMesh.name = originalMesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
			if (attachedToMeshfilter)
			{
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				if (component != null)
				{
					component.sharedMesh = reducedMesh;
					return;
				}
			}
			else
			{
				SkinnedMeshRenderer component2 = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component2 != null)
				{
					component2.sharedMesh = reducedMesh;
				}
			}
		}

		// Token: 0x06003288 RID: 12936 RVA: 0x00229F48 File Offset: 0x00228148
		private static int CountTriangles(PolyfewRuntime.ObjectMeshPairs objectMeshPairs)
		{
			int num = 0;
			if (objectMeshPairs == null)
			{
				return 0;
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair in objectMeshPairs)
			{
				if (!(keyValuePair.Key == null) && keyValuePair.Value != null && !(keyValuePair.Value.mesh == null))
				{
					num += keyValuePair.Value.mesh.triangles.Length / 3;
				}
			}
			return num;
		}

		// Token: 0x04004E00 RID: 19968
		private const int MAX_LOD_COUNT = 8;

		// Token: 0x02000948 RID: 2376
		[Serializable]
		public class ObjectMeshPairs : Dictionary<GameObject, PolyfewRuntime.MeshRendererPair>
		{
		}

		// Token: 0x02000949 RID: 2377
		public enum MeshCombineTarget
		{
			// Token: 0x04004E02 RID: 19970
			SkinnedAndStatic,
			// Token: 0x04004E03 RID: 19971
			StaticOnly,
			// Token: 0x04004E04 RID: 19972
			SkinnedOnly
		}

		// Token: 0x0200094A RID: 2378
		[Serializable]
		public class MeshRendererPair
		{
			// Token: 0x0600328B RID: 12939 RVA: 0x00229FE4 File Offset: 0x002281E4
			public MeshRendererPair(bool attachedToMeshFilter, Mesh mesh)
			{
				this.attachedToMeshFilter = attachedToMeshFilter;
				this.mesh = mesh;
			}

			// Token: 0x0600328C RID: 12940 RVA: 0x00229FFA File Offset: 0x002281FA
			public void Destruct()
			{
				if (this.mesh != null)
				{
					Object.DestroyImmediate(this.mesh);
				}
			}

			// Token: 0x04004E05 RID: 19973
			public bool attachedToMeshFilter;

			// Token: 0x04004E06 RID: 19974
			public Mesh mesh;
		}

		// Token: 0x0200094B RID: 2379
		[Serializable]
		public class CustomMeshActionStructure
		{
			// Token: 0x0600328D RID: 12941 RVA: 0x0022A015 File Offset: 0x00228215
			public CustomMeshActionStructure(PolyfewRuntime.MeshRendererPair meshRendererPair, GameObject gameObject, Action action)
			{
				this.meshRendererPair = meshRendererPair;
				this.gameObject = gameObject;
				this.action = action;
			}

			// Token: 0x04004E07 RID: 19975
			public PolyfewRuntime.MeshRendererPair meshRendererPair;

			// Token: 0x04004E08 RID: 19976
			public GameObject gameObject;

			// Token: 0x04004E09 RID: 19977
			public Action action;
		}

		// Token: 0x0200094C RID: 2380
		[Serializable]
		public class SimplificationOptions
		{
			// Token: 0x0600328E RID: 12942 RVA: 0x0022A032 File Offset: 0x00228232
			public SimplificationOptions()
			{
			}

			// Token: 0x0600328F RID: 12943 RVA: 0x0022A060 File Offset: 0x00228260
			public SimplificationOptions(float simplificationStrength, bool simplifyOptimal, bool enableSmartlink, bool recalculateNormals, bool preserveUVSeamEdges, bool preserveUVFoldoverEdges, bool preserveBorderEdges, bool regardToleranceSphere, List<PolyfewRuntime.PreservationSphere> preservationSpheres, bool regardCurvature, int maxIterations, float aggressiveness, bool useEdgeSort)
			{
				this.simplificationStrength = simplificationStrength;
				this.simplifyMeshLossless = simplifyOptimal;
				this.enableSmartlinking = enableSmartlink;
				this.recalculateNormals = recalculateNormals;
				this.preserveUVSeamEdges = preserveUVSeamEdges;
				this.preserveUVFoldoverEdges = preserveUVFoldoverEdges;
				this.preserveBorderEdges = preserveBorderEdges;
				this.regardPreservationSpheres = regardToleranceSphere;
				this.preservationSpheres = preservationSpheres;
				this.regardCurvature = regardCurvature;
				this.maxIterations = maxIterations;
				this.aggressiveness = aggressiveness;
				this.useEdgeSort = useEdgeSort;
			}

			// Token: 0x04004E0A RID: 19978
			public float simplificationStrength;

			// Token: 0x04004E0B RID: 19979
			public bool simplifyMeshLossless;

			// Token: 0x04004E0C RID: 19980
			public bool enableSmartlinking = true;

			// Token: 0x04004E0D RID: 19981
			public bool recalculateNormals;

			// Token: 0x04004E0E RID: 19982
			public bool preserveUVSeamEdges;

			// Token: 0x04004E0F RID: 19983
			public bool preserveUVFoldoverEdges;

			// Token: 0x04004E10 RID: 19984
			public bool preserveBorderEdges;

			// Token: 0x04004E11 RID: 19985
			public bool regardPreservationSpheres;

			// Token: 0x04004E12 RID: 19986
			public List<PolyfewRuntime.PreservationSphere> preservationSpheres = new List<PolyfewRuntime.PreservationSphere>();

			// Token: 0x04004E13 RID: 19987
			public bool regardCurvature;

			// Token: 0x04004E14 RID: 19988
			public int maxIterations = 100;

			// Token: 0x04004E15 RID: 19989
			public float aggressiveness = 7f;

			// Token: 0x04004E16 RID: 19990
			public bool useEdgeSort;
		}

		// Token: 0x0200094D RID: 2381
		[Serializable]
		public class PreservationSphere
		{
			// Token: 0x06003290 RID: 12944 RVA: 0x0022A0FD File Offset: 0x002282FD
			public PreservationSphere(Vector3 worldPosition, float diameter, float preservationStrength)
			{
				this.worldPosition = worldPosition;
				this.diameter = diameter;
				this.preservationStrength = preservationStrength;
			}

			// Token: 0x04004E17 RID: 19991
			public Vector3 worldPosition;

			// Token: 0x04004E18 RID: 19992
			public float diameter;

			// Token: 0x04004E19 RID: 19993
			public float preservationStrength = 100f;
		}

		// Token: 0x0200094E RID: 2382
		[Serializable]
		public class OBJImportOptions : ImportOptions
		{
		}

		// Token: 0x0200094F RID: 2383
		[Serializable]
		public class OBJExportOptions
		{
			// Token: 0x06003292 RID: 12946 RVA: 0x0022A130 File Offset: 0x00228330
			public OBJExportOptions(bool applyPosition, bool applyRotation, bool applyScale, bool generateMaterials, bool exportTextures)
			{
				this.applyPosition = applyPosition;
				this.applyRotation = applyRotation;
				this.applyScale = applyScale;
				this.generateMaterials = generateMaterials;
				this.exportTextures = exportTextures;
			}

			// Token: 0x04004E1A RID: 19994
			public readonly bool applyPosition = true;

			// Token: 0x04004E1B RID: 19995
			public readonly bool applyRotation = true;

			// Token: 0x04004E1C RID: 19996
			public readonly bool applyScale = true;

			// Token: 0x04004E1D RID: 19997
			public readonly bool generateMaterials = true;

			// Token: 0x04004E1E RID: 19998
			public readonly bool exportTextures = true;
		}

		// Token: 0x02000950 RID: 2384
		public class ReferencedNumeric<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			// Token: 0x17000548 RID: 1352
			// (get) Token: 0x06003293 RID: 12947 RVA: 0x0022A18B File Offset: 0x0022838B
			// (set) Token: 0x06003294 RID: 12948 RVA: 0x0022A193 File Offset: 0x00228393
			public T Value
			{
				get
				{
					return this.val;
				}
				set
				{
					this.val = value;
				}
			}

			// Token: 0x06003295 RID: 12949 RVA: 0x0022A19C File Offset: 0x0022839C
			public ReferencedNumeric(T value)
			{
				this.val = value;
			}

			// Token: 0x04004E1F RID: 19999
			private T val;
		}

		// Token: 0x02000951 RID: 2385
		[Serializable]
		public class MaterialProperties
		{
			// Token: 0x06003296 RID: 12950 RVA: 0x0022A1AC File Offset: 0x002283AC
			public MaterialProperties(int texArrIndex, int matIndex, string materialName, Material originalMaterial, Color albedoTint, Vector4 uvTileOffset, float normalIntensity, float occlusionIntensity, float smoothnessIntensity, float glossMapScale, float metalIntensity, Color emissionColor, Vector4 detailUVTileOffset, float alphaCutoff, Color specularColor, float detailNormalScale, float heightIntensity, float uvSec)
			{
				this.texArrIndex = texArrIndex;
				this.matIndex = matIndex;
				this.materialName = materialName;
				this.originalMaterial = originalMaterial;
				this.albedoTint = albedoTint;
				this.uvTileOffset = uvTileOffset;
				this.normalIntensity = normalIntensity;
				this.occlusionIntensity = occlusionIntensity;
				this.smoothnessIntensity = smoothnessIntensity;
				this.glossMapScale = glossMapScale;
				this.metalIntensity = metalIntensity;
				this.emissionColor = emissionColor;
				this.detailUVTileOffset = detailUVTileOffset;
				this.alphaCutoff = alphaCutoff;
				this.specularColor = specularColor;
				this.detailNormalScale = detailNormalScale;
				this.heightIntensity = heightIntensity;
				this.uvSec = uvSec;
			}

			// Token: 0x06003297 RID: 12951 RVA: 0x0022A2F8 File Offset: 0x002284F8
			public void BurnAttrToImg(ref Texture2D burnOn, int index, int textureArrayIndex)
			{
				if (index >= burnOn.height)
				{
					Texture2D texture2D = new Texture2D(burnOn.width, index + 1, 17, false, true);
					Color[] pixels = burnOn.GetPixels();
					texture2D.SetPixels(0, 0, burnOn.width, burnOn.height, pixels);
					burnOn = texture2D;
				}
				if (burnOn.width < 8)
				{
					Texture2D texture2D2 = new Texture2D(8, burnOn.height, 17, false, true);
					Color[] pixels2 = burnOn.GetPixels();
					texture2D2.SetPixels(0, 0, burnOn.width, burnOn.height, pixels2);
					burnOn = texture2D2;
				}
				burnOn.SetPixel(0, index, new Color(this.uvTileOffset.x - 1f, this.uvTileOffset.y - 1f, this.uvTileOffset.z, this.uvTileOffset.w));
				burnOn.SetPixel(1, index, new Color(this.normalIntensity, this.occlusionIntensity, this.smoothnessIntensity, this.metalIntensity));
				burnOn.SetPixel(2, index, this.albedoTint);
				burnOn.SetPixel(3, index, this.emissionColor);
				burnOn.SetPixel(4, index, new Color(this.specularColor.r, this.specularColor.g, this.specularColor.b, this.glossMapScale));
				burnOn.SetPixel(5, index, new Color(this.detailUVTileOffset.x, this.detailUVTileOffset.y, this.detailUVTileOffset.z, this.detailUVTileOffset.w));
				burnOn.SetPixel(6, index, new Color(this.alphaCutoff, this.detailNormalScale, this.heightIntensity, this.uvSec));
				burnOn.SetPixel(7, index, new Color((float)textureArrayIndex, (float)textureArrayIndex, (float)textureArrayIndex, (float)textureArrayIndex));
				burnOn.Apply();
			}

			// Token: 0x04004E20 RID: 20000
			public readonly int texArrIndex;

			// Token: 0x04004E21 RID: 20001
			public readonly int matIndex;

			// Token: 0x04004E22 RID: 20002
			public readonly string materialName;

			// Token: 0x04004E23 RID: 20003
			public readonly Material originalMaterial;

			// Token: 0x04004E24 RID: 20004
			public Color albedoTint;

			// Token: 0x04004E25 RID: 20005
			public Vector4 uvTileOffset = new Vector4(1f, 1f, 0f, 0f);

			// Token: 0x04004E26 RID: 20006
			public float normalIntensity = 1f;

			// Token: 0x04004E27 RID: 20007
			public float occlusionIntensity = 1f;

			// Token: 0x04004E28 RID: 20008
			public float smoothnessIntensity = 1f;

			// Token: 0x04004E29 RID: 20009
			public float glossMapScale = 1f;

			// Token: 0x04004E2A RID: 20010
			public float metalIntensity = 1f;

			// Token: 0x04004E2B RID: 20011
			public Color emissionColor = Color.black;

			// Token: 0x04004E2C RID: 20012
			public Vector4 detailUVTileOffset = new Vector4(1f, 1f, 0f, 0f);

			// Token: 0x04004E2D RID: 20013
			public float alphaCutoff = 0.5f;

			// Token: 0x04004E2E RID: 20014
			public Color specularColor = Color.black;

			// Token: 0x04004E2F RID: 20015
			public float detailNormalScale = 1f;

			// Token: 0x04004E30 RID: 20016
			public float heightIntensity = 0.05f;

			// Token: 0x04004E31 RID: 20017
			public readonly float uvSec;
		}
	}
}
