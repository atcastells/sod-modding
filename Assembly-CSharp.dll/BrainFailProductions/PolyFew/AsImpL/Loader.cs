using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BrainFailProductions.PolyFewRuntime;
using UnityEngine;
using UnityEngine.Networking;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x02000908 RID: 2312
	public abstract class Loader : MonoBehaviour
	{
		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x0600312F RID: 12591 RVA: 0x0021C285 File Offset: 0x0021A485
		// (set) Token: 0x06003130 RID: 12592 RVA: 0x0021C29C File Offset: 0x0021A49C
		public bool ConvertVertAxis
		{
			get
			{
				return this.buildOptions != null && this.buildOptions.zUp;
			}
			set
			{
				if (this.buildOptions == null)
				{
					this.buildOptions = new ImportOptions();
				}
				this.buildOptions.zUp = value;
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06003131 RID: 12593 RVA: 0x0021C2BD File Offset: 0x0021A4BD
		// (set) Token: 0x06003132 RID: 12594 RVA: 0x0021C2D8 File Offset: 0x0021A4D8
		public float Scaling
		{
			get
			{
				if (this.buildOptions == null)
				{
					return 1f;
				}
				return this.buildOptions.modelScaling;
			}
			set
			{
				if (this.buildOptions == null)
				{
					this.buildOptions = new ImportOptions();
				}
				this.buildOptions.modelScaling = value;
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06003133 RID: 12595
		protected abstract bool HasMaterialLibrary { get; }

		// Token: 0x14000077 RID: 119
		// (add) Token: 0x06003134 RID: 12596 RVA: 0x0021C2FC File Offset: 0x0021A4FC
		// (remove) Token: 0x06003135 RID: 12597 RVA: 0x0021C334 File Offset: 0x0021A534
		public event Action<GameObject, string> ModelCreated;

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x06003136 RID: 12598 RVA: 0x0021C36C File Offset: 0x0021A56C
		// (remove) Token: 0x06003137 RID: 12599 RVA: 0x0021C3A4 File Offset: 0x0021A5A4
		public event Action<GameObject, string> ModelLoaded;

		// Token: 0x14000079 RID: 121
		// (add) Token: 0x06003138 RID: 12600 RVA: 0x0021C3DC File Offset: 0x0021A5DC
		// (remove) Token: 0x06003139 RID: 12601 RVA: 0x0021C414 File Offset: 0x0021A614
		public event Action<string> ModelError;

		// Token: 0x0600313A RID: 12602 RVA: 0x0021C449 File Offset: 0x0021A649
		public static GameObject GetModelByPath(string absolutePath)
		{
			if (Loader.loadedModels.ContainsKey(absolutePath))
			{
				return Loader.loadedModels[absolutePath];
			}
			return null;
		}

		// Token: 0x0600313B RID: 12603 RVA: 0x0021C468 File Offset: 0x0021A668
		public Task<GameObject> Load(string objName, string objAbsolutePath, Transform parentObj, string texturesFolderPath = "", string materialsFolderPath = "")
		{
			Loader.<Load>d__33 <Load>d__;
			<Load>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<Load>d__.<>4__this = this;
			<Load>d__.objName = objName;
			<Load>d__.objAbsolutePath = objAbsolutePath;
			<Load>d__.parentObj = parentObj;
			<Load>d__.texturesFolderPath = texturesFolderPath;
			<Load>d__.materialsFolderPath = materialsFolderPath;
			<Load>d__.<>1__state = -1;
			<Load>d__.<>t__builder.Start<Loader.<Load>d__33>(ref <Load>d__);
			return <Load>d__.<>t__builder.Task;
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x0021C4D8 File Offset: 0x0021A6D8
		public Task<GameObject> LoadFromNetwork(string objURL, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, string objName)
		{
			Loader.<LoadFromNetwork>d__34 <LoadFromNetwork>d__;
			<LoadFromNetwork>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<LoadFromNetwork>d__.<>4__this = this;
			<LoadFromNetwork>d__.objURL = objURL;
			<LoadFromNetwork>d__.diffuseTexURL = diffuseTexURL;
			<LoadFromNetwork>d__.bumpTexURL = bumpTexURL;
			<LoadFromNetwork>d__.specularTexURL = specularTexURL;
			<LoadFromNetwork>d__.opacityTexURL = opacityTexURL;
			<LoadFromNetwork>d__.materialURL = materialURL;
			<LoadFromNetwork>d__.objName = objName;
			<LoadFromNetwork>d__.<>1__state = -1;
			<LoadFromNetwork>d__.<>t__builder.Start<Loader.<LoadFromNetwork>d__34>(ref <LoadFromNetwork>d__);
			return <LoadFromNetwork>d__.<>t__builder.Task;
		}

		// Token: 0x0600313D RID: 12605 RVA: 0x0021C558 File Offset: 0x0021A758
		public IEnumerator LoadFromNetworkWebGL(string objURL, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, string objName, Action<GameObject> OnSuccess, Action<Exception> OnError)
		{
			string text = objName + ".obj";
			Loader.totalProgress.singleProgress.Add(this.objLoadingProgress);
			this.objLoadingProgress.fileName = text;
			this.objLoadingProgress.error = false;
			this.objLoadingProgress.message = "Loading " + text + "...";
			Loader.loadedModels[objURL] = null;
			Loader.instanceCount[objURL] = 0;
			float lastTime = Time.realtimeSinceStartup;
			float startTime = lastTime;
			yield return base.StartCoroutine(this.LoadModelFileNetworkedWebGL(objURL, OnError));
			if (ObjectImporter.isException)
			{
				yield return null;
			}
			this.loadStats.modelParseTime = Time.realtimeSinceStartup - lastTime;
			if (this.objLoadingProgress.error)
			{
				this.OnLoadFailed(objURL);
				OnError.Invoke(new Exception("Load failed due to unknown reasons."));
				yield return null;
			}
			lastTime = Time.realtimeSinceStartup;
			if (this.HasMaterialLibrary)
			{
				yield return base.StartCoroutine(this.LoadMaterialLibraryWebGL(materialURL));
			}
			else
			{
				ObjectImporter.activeDownloads--;
			}
			if (ObjectImporter.isException)
			{
				yield return null;
			}
			this.loadStats.materialsParseTime = Time.realtimeSinceStartup - lastTime;
			lastTime = Time.realtimeSinceStartup;
			yield return base.StartCoroutine(this.NetworkedBuildWebGL(null, objName, objURL, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL));
			if (ObjectImporter.isException)
			{
				yield return null;
			}
			this.loadStats.buildTime = Time.realtimeSinceStartup - lastTime;
			this.loadStats.totalTime = Time.realtimeSinceStartup - startTime;
			Loader.totalProgress.singleProgress.Remove(this.objLoadingProgress);
			this.OnLoaded(Loader.loadedModels[objURL], objURL);
			OnSuccess.Invoke(Loader.loadedModels[objURL]);
			yield break;
		}

		// Token: 0x0600313E RID: 12606
		public abstract string[] ParseTexturePaths(string absolutePath);

		// Token: 0x0600313F RID: 12607
		protected abstract Task LoadModelFile(string absolutePath, string texturesFolderPath = "", string materialsFolderPath = "");

		// Token: 0x06003140 RID: 12608
		protected abstract Task LoadModelFileNetworked(string objURL);

		// Token: 0x06003141 RID: 12609
		protected abstract IEnumerator LoadModelFileNetworkedWebGL(string objURL, Action<Exception> OnError);

		// Token: 0x06003142 RID: 12610
		protected abstract Task LoadMaterialLibrary(string absolutePath, string materialsFolderPath = "");

		// Token: 0x06003143 RID: 12611
		protected abstract Task LoadMaterialLibrary(string materialURL);

		// Token: 0x06003144 RID: 12612
		protected abstract IEnumerator LoadMaterialLibraryWebGL(string materialURL);

		// Token: 0x06003145 RID: 12613 RVA: 0x0021C5B8 File Offset: 0x0021A7B8
		protected Task Build(string absolutePath, string objName, Transform parentTransform, string texturesFolderPath = "")
		{
			Loader.<Build>d__43 <Build>d__;
			<Build>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Build>d__.<>4__this = this;
			<Build>d__.absolutePath = absolutePath;
			<Build>d__.objName = objName;
			<Build>d__.parentTransform = parentTransform;
			<Build>d__.texturesFolderPath = texturesFolderPath;
			<Build>d__.<>1__state = -1;
			<Build>d__.<>t__builder.Start<Loader.<Build>d__43>(ref <Build>d__);
			return <Build>d__.<>t__builder.Task;
		}

		// Token: 0x06003146 RID: 12614 RVA: 0x0021C61C File Offset: 0x0021A81C
		protected Task NetworkedBuild(Transform parentTransform, string objName, string objURL, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL)
		{
			Loader.<NetworkedBuild>d__44 <NetworkedBuild>d__;
			<NetworkedBuild>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<NetworkedBuild>d__.<>4__this = this;
			<NetworkedBuild>d__.parentTransform = parentTransform;
			<NetworkedBuild>d__.objName = objName;
			<NetworkedBuild>d__.objURL = objURL;
			<NetworkedBuild>d__.diffuseTexURL = diffuseTexURL;
			<NetworkedBuild>d__.bumpTexURL = bumpTexURL;
			<NetworkedBuild>d__.specularTexURL = specularTexURL;
			<NetworkedBuild>d__.opacityTexURL = opacityTexURL;
			<NetworkedBuild>d__.<>1__state = -1;
			<NetworkedBuild>d__.<>t__builder.Start<Loader.<NetworkedBuild>d__44>(ref <NetworkedBuild>d__);
			return <NetworkedBuild>d__.<>t__builder.Task;
		}

		// Token: 0x06003147 RID: 12615 RVA: 0x0021C69C File Offset: 0x0021A89C
		protected IEnumerator NetworkedBuildWebGL(Transform parentTransform, string objName, string objURL, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL)
		{
			float prevTime = Time.realtimeSinceStartup;
			if (this.materialData != null)
			{
				this.objLoadingProgress.message = "Loading textures...";
				int count = 0;
				foreach (MaterialData mtl in this.materialData)
				{
					this.objLoadingProgress.percentage = Loader.LOAD_PHASE_PERC + Loader.TEXTURE_PHASE_PERC * (float)count / (float)this.materialData.Count;
					int num = count;
					count = num + 1;
					if (mtl.diffuseTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(diffuseTexURL))
						{
							yield return base.StartCoroutine(this.LoadMaterialTextureWebGL(diffuseTexURL));
						}
						else
						{
							ObjectImporter.activeDownloads--;
						}
						mtl.diffuseTex = this.loadedTexture;
					}
					else
					{
						ObjectImporter.activeDownloads--;
					}
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.bumpTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(bumpTexURL))
						{
							yield return base.StartCoroutine(this.LoadMaterialTextureWebGL(bumpTexURL));
						}
						else
						{
							ObjectImporter.activeDownloads--;
						}
						mtl.bumpTex = this.loadedTexture;
					}
					else
					{
						ObjectImporter.activeDownloads--;
					}
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.specularTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(specularTexURL))
						{
							yield return base.StartCoroutine(this.LoadMaterialTextureWebGL(specularTexURL));
						}
						else
						{
							ObjectImporter.activeDownloads--;
						}
						mtl.specularTex = this.loadedTexture;
					}
					else
					{
						ObjectImporter.activeDownloads--;
					}
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.opacityTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(opacityTexURL))
						{
							yield return base.StartCoroutine(this.LoadMaterialTextureWebGL(opacityTexURL));
						}
						else
						{
							ObjectImporter.activeDownloads--;
						}
						mtl.opacityTex = this.loadedTexture;
					}
					else
					{
						ObjectImporter.activeDownloads--;
					}
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					mtl = null;
				}
				List<MaterialData>.Enumerator enumerator = default(List<MaterialData>.Enumerator);
			}
			this.loadStats.buildStats.texturesTime = Time.realtimeSinceStartup - prevTime;
			prevTime = Time.realtimeSinceStartup;
			ObjectBuilder.ProgressInfo progressInfo = new ObjectBuilder.ProgressInfo();
			this.objLoadingProgress.message = "Loading materials...";
			this.objectBuilder.buildOptions = this.buildOptions;
			bool hasColors = this.dataSet.colorList.Count > 0;
			bool flag = this.materialData != null;
			this.objectBuilder.InitBuildMaterials(this.materialData, hasColors);
			float percentage = this.objLoadingProgress.percentage;
			if (flag)
			{
				while (this.objectBuilder.BuildMaterials(progressInfo))
				{
					this.objLoadingProgress.percentage = percentage + Loader.MATERIAL_PHASE_PERC * (float)this.objectBuilder.NumImportedMaterials / (float)this.materialData.Count;
				}
				this.loadStats.buildStats.materialsTime = Time.realtimeSinceStartup - prevTime;
				prevTime = Time.realtimeSinceStartup;
			}
			this.objLoadingProgress.message = "Building scene objects...";
			GameObject gameObject = new GameObject(objName);
			if (this.buildOptions.hideWhileLoading)
			{
				gameObject.SetActive(false);
			}
			if (parentTransform != null)
			{
				gameObject.transform.SetParent(parentTransform.transform, false);
			}
			this.OnCreated(gameObject, objURL);
			float percentage2 = this.objLoadingProgress.percentage;
			this.objectBuilder.StartBuildObjectAsync(this.dataSet, gameObject, null);
			while (this.objectBuilder.BuildObjectAsync(ref progressInfo))
			{
				this.objLoadingProgress.message = "Building scene objects... " + (progressInfo.objectsLoaded + progressInfo.groupsLoaded).ToString() + "/" + (this.dataSet.objectList.Count + progressInfo.numGroups).ToString();
				this.objLoadingProgress.percentage = percentage2 + Loader.BUILD_PHASE_PERC * ((float)(progressInfo.objectsLoaded / this.dataSet.objectList.Count) + (float)progressInfo.groupsLoaded / (float)progressInfo.numGroups);
			}
			this.objLoadingProgress.percentage = 100f;
			Loader.loadedModels[objURL] = gameObject;
			this.loadStats.buildStats.objectsTime = Time.realtimeSinceStartup - prevTime;
			yield break;
			yield break;
		}

		// Token: 0x06003148 RID: 12616 RVA: 0x0021C6EC File Offset: 0x0021A8EC
		protected string GetDirName(string absolutePath)
		{
			string text;
			if (absolutePath.Contains("//"))
			{
				text = absolutePath.Remove(absolutePath.LastIndexOf('/') + 1);
			}
			else
			{
				string directoryName = Path.GetDirectoryName(absolutePath);
				text = (string.IsNullOrEmpty(directoryName) ? "" : directoryName);
				if (!text.EndsWith("/"))
				{
					text += "/";
				}
			}
			return text;
		}

		// Token: 0x06003149 RID: 12617 RVA: 0x0021C74C File Offset: 0x0021A94C
		protected virtual void OnLoaded(GameObject obj, string absolutePath)
		{
			if (obj == null)
			{
				if (this.ModelError != null)
				{
					this.ModelError.Invoke(absolutePath);
					return;
				}
			}
			else
			{
				if (this.buildOptions != null)
				{
					obj.transform.localPosition = this.buildOptions.localPosition;
					obj.transform.localRotation = Quaternion.Euler(this.buildOptions.localEulerAngles);
					obj.transform.localScale = this.buildOptions.localScale;
					if (this.buildOptions.inheritLayer)
					{
						obj.layer = obj.transform.parent.gameObject.layer;
						MeshRenderer[] componentsInChildren = obj.transform.GetComponentsInChildren<MeshRenderer>(true);
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].gameObject.layer = obj.transform.parent.gameObject.layer;
						}
					}
				}
				if (this.buildOptions.hideWhileLoading)
				{
					obj.SetActive(true);
				}
				if (this.ModelLoaded != null)
				{
					this.ModelLoaded.Invoke(obj, absolutePath);
				}
			}
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x0021C85A File Offset: 0x0021AA5A
		protected virtual void OnCreated(GameObject obj, string absolutePath)
		{
			if (obj == null)
			{
				if (this.ModelError != null)
				{
					this.ModelError.Invoke(absolutePath);
					return;
				}
			}
			else if (this.ModelCreated != null)
			{
				this.ModelCreated.Invoke(obj, absolutePath);
			}
		}

		// Token: 0x0600314B RID: 12619 RVA: 0x0021C88F File Offset: 0x0021AA8F
		protected virtual void OnLoadFailed(string absolutePath)
		{
			if (this.ModelError != null)
			{
				this.ModelError.Invoke(absolutePath);
			}
		}

		// Token: 0x0600314C RID: 12620 RVA: 0x0021C8A8 File Offset: 0x0021AAA8
		private string GetTextureUrl(string basePath, string texturePath)
		{
			string text = texturePath.Replace("\\", "/").Replace("//", "/");
			if (!Path.IsPathRooted(text))
			{
				text = basePath + texturePath;
			}
			if (!text.Contains("//"))
			{
				text = "file:///" + text;
			}
			this.objLoadingProgress.message = "Loading textures...\n" + text;
			return text;
		}

		// Token: 0x0600314D RID: 12621 RVA: 0x0021C918 File Offset: 0x0021AB18
		private Task LoadMaterialTexture(string basePath, string path, string texturesFolderPath = "")
		{
			Loader.<LoadMaterialTexture>d__51 <LoadMaterialTexture>d__;
			<LoadMaterialTexture>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadMaterialTexture>d__.<>4__this = this;
			<LoadMaterialTexture>d__.basePath = basePath;
			<LoadMaterialTexture>d__.path = path;
			<LoadMaterialTexture>d__.texturesFolderPath = texturesFolderPath;
			<LoadMaterialTexture>d__.<>1__state = -1;
			<LoadMaterialTexture>d__.<>t__builder.Start<Loader.<LoadMaterialTexture>d__51>(ref <LoadMaterialTexture>d__);
			return <LoadMaterialTexture>d__.<>t__builder.Task;
		}

		// Token: 0x0600314E RID: 12622 RVA: 0x0021C974 File Offset: 0x0021AB74
		private Task LoadMaterialTexture(string textureURL)
		{
			Loader.<LoadMaterialTexture>d__52 <LoadMaterialTexture>d__;
			<LoadMaterialTexture>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadMaterialTexture>d__.<>4__this = this;
			<LoadMaterialTexture>d__.textureURL = textureURL;
			<LoadMaterialTexture>d__.<>1__state = -1;
			<LoadMaterialTexture>d__.<>t__builder.Start<Loader.<LoadMaterialTexture>d__52>(ref <LoadMaterialTexture>d__);
			return <LoadMaterialTexture>d__.<>t__builder.Task;
		}

		// Token: 0x0600314F RID: 12623 RVA: 0x0021C9BF File Offset: 0x0021ABBF
		private IEnumerator LoadMaterialTextureWebGL(string textureURL)
		{
			this.loadedTexture = null;
			bool isWorking = true;
			float value = this.individualProgress.Value;
			try
			{
				base.StartCoroutine(this.DownloadTexFileWebGL(textureURL, this.individualProgress, delegate(Texture2D texture)
				{
					isWorking = false;
					this.loadedTexture = texture;
				}, delegate(string error)
				{
					ObjectImporter.activeDownloads--;
					isWorking = false;
					Debug.LogWarning("Failed to load the associated texture file." + error);
				}));
				goto IL_125;
			}
			catch (Exception ex)
			{
				ObjectImporter.activeDownloads--;
				this.individualProgress.Value = value;
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
				isWorking = false;
				throw ex;
			}
			IL_E3:
			yield return new WaitForSeconds(0.1f);
			ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			IL_125:
			if (!isWorking)
			{
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
				if (this.loadedTexture == null)
				{
					Debug.LogWarning("Failed to load texture.");
				}
				yield break;
			}
			goto IL_E3;
		}

		// Token: 0x06003150 RID: 12624 RVA: 0x0021C9D8 File Offset: 0x0021ABD8
		private Texture2D LoadTexture(UnityWebRequest loader)
		{
			string text = Path.GetExtension(loader.url).ToLower();
			Texture2D texture2D = null;
			if (text == ".tga")
			{
				texture2D = TextureLoader.LoadTextureFromUrl(loader.url);
			}
			else if (text == ".png" || text == ".jpg" || text == ".jpeg")
			{
				texture2D = DownloadHandlerTexture.GetContent(loader);
			}
			else
			{
				Debug.LogWarning("Unsupported texture format: " + text);
			}
			if (texture2D == null)
			{
				Debug.LogErrorFormat("Failed to load texture {0}", new object[]
				{
					loader.url
				});
			}
			return texture2D;
		}

		// Token: 0x06003151 RID: 12625 RVA: 0x0021CA76 File Offset: 0x0021AC76
		public IEnumerator DownloadFile(string url, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<byte[]> DownloadComplete, Action<string> OnError)
		{
			WWW www = null;
			float oldProgress = downloadProgress.Value;
			try
			{
				www = new WWW(url);
			}
			catch (Exception ex)
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(ex.ToString());
			}
			Coroutine progress = base.StartCoroutine(this.GetProgress(www, downloadProgress));
			yield return www;
			if (!string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(www.error);
			}
			else if (www.bytes == null || www.bytes.Length == 0)
			{
				if (string.IsNullOrWhiteSpace(www.error))
				{
					downloadProgress.Value = oldProgress;
					OnError.Invoke("No bytes downloaded. The file might be empty.");
				}
				else
				{
					downloadProgress.Value = oldProgress;
					OnError.Invoke(www.error);
				}
			}
			else if (string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress + 1f;
				DownloadComplete.Invoke(www.bytes);
			}
			else
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(www.error);
			}
			base.StopCoroutine(progress);
			www.Dispose();
			yield break;
		}

		// Token: 0x06003152 RID: 12626 RVA: 0x0021CAA2 File Offset: 0x0021ACA2
		private IEnumerator GetProgress(WWW www, PolyfewRuntime.ReferencedNumeric<float> downloadProgress)
		{
			float oldProgress = downloadProgress.Value;
			if (www != null && downloadProgress != null)
			{
				while (!www.isDone && string.IsNullOrWhiteSpace(www.error))
				{
					yield return new WaitForSeconds(0.1f);
					downloadProgress.Value = oldProgress + www.progress;
				}
				if (www.isDone && string.IsNullOrWhiteSpace(www.error))
				{
					downloadProgress.Value = oldProgress + www.progress;
					Debug.Log("Progress  " + www.progress.ToString());
				}
			}
			yield break;
		}

		// Token: 0x06003153 RID: 12627 RVA: 0x0021CAB8 File Offset: 0x0021ACB8
		public IEnumerator DownloadFileWebGL(string url, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<string> DownloadComplete, Action<string> OnError)
		{
			WWW www = null;
			float oldProgress = downloadProgress.Value;
			try
			{
				www = new WWW(url);
			}
			catch (Exception ex)
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(ex.ToString());
			}
			Coroutine progress = base.StartCoroutine(this.GetProgress(www, downloadProgress));
			yield return www;
			if (!string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(www.error);
			}
			else if (www.bytes == null || www.bytes.Length == 0)
			{
				if (string.IsNullOrWhiteSpace(www.error))
				{
					downloadProgress.Value = oldProgress;
					OnError.Invoke("No bytes downloaded. The file might be empty.");
				}
				else
				{
					downloadProgress.Value = oldProgress;
					OnError.Invoke(www.error);
				}
			}
			else if (string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress + 1f;
				DownloadComplete.Invoke(www.text);
			}
			else
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(www.error);
			}
			try
			{
				base.StopCoroutine(progress);
			}
			catch (Exception)
			{
			}
			www.Dispose();
			yield break;
		}

		// Token: 0x06003154 RID: 12628 RVA: 0x0021CAE4 File Offset: 0x0021ACE4
		public IEnumerator DownloadTexFileWebGL(string url, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<Texture2D> DownloadComplete, Action<string> OnError)
		{
			WWW www = null;
			float oldProgress = downloadProgress.Value;
			try
			{
				www = new WWW(url);
			}
			catch (Exception ex)
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(ex.ToString());
			}
			Coroutine progress = base.StartCoroutine(this.GetProgress(www, downloadProgress));
			yield return www;
			if (!string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(www.error);
			}
			else if (www.bytes == null || www.bytes.Length == 0)
			{
				if (string.IsNullOrWhiteSpace(www.error))
				{
					downloadProgress.Value = oldProgress;
					OnError.Invoke("No bytes downloaded. The file might be empty.");
				}
				else
				{
					downloadProgress.Value = oldProgress;
					OnError.Invoke(www.error);
				}
			}
			else if (string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress + 1f;
				DownloadComplete.Invoke(www.texture);
			}
			else
			{
				downloadProgress.Value = oldProgress;
				OnError.Invoke(www.error);
			}
			base.StopCoroutine(progress);
			www.Dispose();
			yield break;
		}

		// Token: 0x04004C60 RID: 19552
		public static LoadingProgress totalProgress = new LoadingProgress();

		// Token: 0x04004C61 RID: 19553
		public ImportOptions buildOptions;

		// Token: 0x04004C62 RID: 19554
		public PolyfewRuntime.ReferencedNumeric<float> individualProgress = new PolyfewRuntime.ReferencedNumeric<float>(0f);

		// Token: 0x04004C63 RID: 19555
		protected static float LOAD_PHASE_PERC = 8f;

		// Token: 0x04004C64 RID: 19556
		protected static float TEXTURE_PHASE_PERC = 1f;

		// Token: 0x04004C65 RID: 19557
		protected static float MATERIAL_PHASE_PERC = 1f;

		// Token: 0x04004C66 RID: 19558
		protected static float BUILD_PHASE_PERC = 90f;

		// Token: 0x04004C67 RID: 19559
		protected static Dictionary<string, GameObject> loadedModels = new Dictionary<string, GameObject>();

		// Token: 0x04004C68 RID: 19560
		protected static Dictionary<string, int> instanceCount = new Dictionary<string, int>();

		// Token: 0x04004C69 RID: 19561
		protected DataSet dataSet = new DataSet();

		// Token: 0x04004C6A RID: 19562
		protected ObjectBuilder objectBuilder = new ObjectBuilder();

		// Token: 0x04004C6B RID: 19563
		protected List<MaterialData> materialData;

		// Token: 0x04004C6C RID: 19564
		protected SingleLoadingProgress objLoadingProgress = new SingleLoadingProgress();

		// Token: 0x04004C6D RID: 19565
		protected Loader.Stats loadStats;

		// Token: 0x04004C6E RID: 19566
		private Texture2D loadedTexture;

		// Token: 0x02000909 RID: 2313
		protected struct BuildStats
		{
			// Token: 0x04004C72 RID: 19570
			public float texturesTime;

			// Token: 0x04004C73 RID: 19571
			public float materialsTime;

			// Token: 0x04004C74 RID: 19572
			public float objectsTime;
		}

		// Token: 0x0200090A RID: 2314
		protected struct Stats
		{
			// Token: 0x04004C75 RID: 19573
			public float modelParseTime;

			// Token: 0x04004C76 RID: 19574
			public float materialsParseTime;

			// Token: 0x04004C77 RID: 19575
			public float buildTime;

			// Token: 0x04004C78 RID: 19576
			public Loader.BuildStats buildStats;

			// Token: 0x04004C79 RID: 19577
			public float totalTime;
		}
	}
}
