using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BrainFailProductions.PolyFewRuntime;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x0200092D RID: 2349
	public class ObjectImporter : MonoBehaviour
	{
		// Token: 0x060031DD RID: 12765 RVA: 0x002221D4 File Offset: 0x002203D4
		public ObjectImporter()
		{
			ObjectImporter.isException = false;
			ObjectImporter.downloadProgress = new PolyfewRuntime.ReferencedNumeric<float>(0f);
			ObjectImporter.objDownloadProgress = 0f;
			ObjectImporter.textureDownloadProgress = 0f;
			ObjectImporter.materialDownloadProgress = 0f;
			ObjectImporter.activeDownloads = 6;
		}

		// Token: 0x1400007A RID: 122
		// (add) Token: 0x060031DE RID: 12766 RVA: 0x00222220 File Offset: 0x00220420
		// (remove) Token: 0x060031DF RID: 12767 RVA: 0x00222258 File Offset: 0x00220458
		public event Action ImportingStart;

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x060031E0 RID: 12768 RVA: 0x00222290 File Offset: 0x00220490
		// (remove) Token: 0x060031E1 RID: 12769 RVA: 0x002222C8 File Offset: 0x002204C8
		public event Action ImportingComplete;

		// Token: 0x1400007C RID: 124
		// (add) Token: 0x060031E2 RID: 12770 RVA: 0x00222300 File Offset: 0x00220500
		// (remove) Token: 0x060031E3 RID: 12771 RVA: 0x00222338 File Offset: 0x00220538
		public event Action<GameObject, string> CreatedModel;

		// Token: 0x1400007D RID: 125
		// (add) Token: 0x060031E4 RID: 12772 RVA: 0x00222370 File Offset: 0x00220570
		// (remove) Token: 0x060031E5 RID: 12773 RVA: 0x002223A8 File Offset: 0x002205A8
		public event Action<GameObject, string> ImportedModel;

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x060031E6 RID: 12774 RVA: 0x002223E0 File Offset: 0x002205E0
		// (remove) Token: 0x060031E7 RID: 12775 RVA: 0x00222418 File Offset: 0x00220618
		public event Action<string> ImportError;

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x060031E8 RID: 12776 RVA: 0x0022244D File Offset: 0x0022064D
		public int NumImportRequests
		{
			get
			{
				return this.numTotalImports;
			}
		}

		// Token: 0x060031E9 RID: 12777 RVA: 0x00222458 File Offset: 0x00220658
		private Loader CreateLoader(string absolutePath, bool isNetwork = false)
		{
			if (isNetwork)
			{
				LoaderObj loaderObj = base.gameObject.AddComponent<LoaderObj>();
				loaderObj.ModelCreated += new Action<GameObject, string>(this.OnModelCreated);
				loaderObj.ModelLoaded += new Action<GameObject, string>(this.OnImported);
				loaderObj.ModelError += new Action<string>(this.OnImportError);
				return loaderObj;
			}
			string text = Path.GetExtension(absolutePath);
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("No extension defined, unable to detect file format. Please provide a full path to the file that ends with the file name including its extension.");
			}
			text = text.ToLower();
			Loader loader;
			if (text.StartsWith(".php"))
			{
				if (!text.EndsWith(".obj"))
				{
					throw new InvalidOperationException("Unable to detect file format in " + text);
				}
				loader = base.gameObject.AddComponent<LoaderObj>();
			}
			else
			{
				if (!(text == ".obj"))
				{
					throw new InvalidOperationException("File format not supported (" + text + ")");
				}
				loader = base.gameObject.AddComponent<LoaderObj>();
			}
			loader.ModelCreated += new Action<GameObject, string>(this.OnModelCreated);
			loader.ModelLoaded += new Action<GameObject, string>(this.OnImported);
			loader.ModelError += new Action<string>(this.OnImportError);
			return loader;
		}

		// Token: 0x060031EA RID: 12778 RVA: 0x00222574 File Offset: 0x00220774
		public Task<GameObject> ImportModelAsync(string objName, string filePath, Transform parentObj, ImportOptions options, string texturesFolderPath = "", string materialsFolderPath = "")
		{
			ObjectImporter.<ImportModelAsync>d__31 <ImportModelAsync>d__;
			<ImportModelAsync>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<ImportModelAsync>d__.<>4__this = this;
			<ImportModelAsync>d__.objName = objName;
			<ImportModelAsync>d__.filePath = filePath;
			<ImportModelAsync>d__.parentObj = parentObj;
			<ImportModelAsync>d__.options = options;
			<ImportModelAsync>d__.texturesFolderPath = texturesFolderPath;
			<ImportModelAsync>d__.materialsFolderPath = materialsFolderPath;
			<ImportModelAsync>d__.<>1__state = -1;
			<ImportModelAsync>d__.<>t__builder.Start<ObjectImporter.<ImportModelAsync>d__31>(ref <ImportModelAsync>d__);
			return <ImportModelAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060031EB RID: 12779 RVA: 0x002225EC File Offset: 0x002207EC
		public Task<GameObject> ImportModelFromNetwork(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, ImportOptions options)
		{
			ObjectImporter.<ImportModelFromNetwork>d__32 <ImportModelFromNetwork>d__;
			<ImportModelFromNetwork>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<ImportModelFromNetwork>d__.<>4__this = this;
			<ImportModelFromNetwork>d__.objURL = objURL;
			<ImportModelFromNetwork>d__.objName = objName;
			<ImportModelFromNetwork>d__.diffuseTexURL = diffuseTexURL;
			<ImportModelFromNetwork>d__.bumpTexURL = bumpTexURL;
			<ImportModelFromNetwork>d__.specularTexURL = specularTexURL;
			<ImportModelFromNetwork>d__.opacityTexURL = opacityTexURL;
			<ImportModelFromNetwork>d__.materialURL = materialURL;
			<ImportModelFromNetwork>d__.downloadProgress = downloadProgress;
			<ImportModelFromNetwork>d__.options = options;
			<ImportModelFromNetwork>d__.<>1__state = -1;
			<ImportModelFromNetwork>d__.<>t__builder.Start<ObjectImporter.<ImportModelFromNetwork>d__32>(ref <ImportModelFromNetwork>d__);
			return <ImportModelFromNetwork>d__.<>t__builder.Task;
		}

		// Token: 0x060031EC RID: 12780 RVA: 0x00222680 File Offset: 0x00220880
		public void ImportModelFromNetworkWebGL(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, ImportOptions options, Action<GameObject> OnSuccess, Action<Exception> OnError)
		{
			if (this.loaderList == null)
			{
				this.loaderList = new List<Loader>();
			}
			if (this.loaderList.Count == 0)
			{
				this.numTotalImports = 0;
				Action importingStart = this.ImportingStart;
				if (importingStart != null)
				{
					importingStart.Invoke();
				}
			}
			Loader loader = this.CreateLoader("", true);
			if (loader == null)
			{
				OnError.Invoke(new SystemException("Loader initialization failed due to unknown reasons."));
			}
			this.numTotalImports++;
			this.loaderList.Add(loader);
			loader.buildOptions = options;
			this.allLoaded = false;
			if (string.IsNullOrWhiteSpace(objName))
			{
				objName = "";
			}
			ObjectImporter.downloadProgress = downloadProgress;
			base.StartCoroutine(loader.LoadFromNetworkWebGL(objURL, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL, materialURL, objName, OnSuccess, OnError));
		}

		// Token: 0x060031ED RID: 12781 RVA: 0x00222748 File Offset: 0x00220948
		public virtual void UpdateStatus()
		{
			if (this.allLoaded)
			{
				return;
			}
			if (this.numTotalImports - Loader.totalProgress.singleProgress.Count >= this.numTotalImports)
			{
				this.allLoaded = true;
				if (this.loaderList != null)
				{
					foreach (Loader loader in this.loaderList)
					{
						Object.Destroy(loader);
					}
					this.loaderList.Clear();
				}
				this.OnImportingComplete();
			}
		}

		// Token: 0x060031EE RID: 12782 RVA: 0x002227E4 File Offset: 0x002209E4
		protected virtual void Update()
		{
			this.UpdateStatus();
		}

		// Token: 0x060031EF RID: 12783 RVA: 0x002227EC File Offset: 0x002209EC
		protected virtual void OnImportingComplete()
		{
			if (this.ImportingComplete != null)
			{
				this.ImportingComplete.Invoke();
			}
		}

		// Token: 0x060031F0 RID: 12784 RVA: 0x00222801 File Offset: 0x00220A01
		protected virtual void OnModelCreated(GameObject obj, string absolutePath)
		{
			if (this.CreatedModel != null)
			{
				this.CreatedModel.Invoke(obj, absolutePath);
			}
		}

		// Token: 0x060031F1 RID: 12785 RVA: 0x00222818 File Offset: 0x00220A18
		protected virtual void OnImported(GameObject obj, string absolutePath)
		{
			if (this.ImportedModel != null)
			{
				this.ImportedModel.Invoke(obj, absolutePath);
			}
		}

		// Token: 0x060031F2 RID: 12786 RVA: 0x0022282F File Offset: 0x00220A2F
		protected virtual void OnImportError(string absolutePath)
		{
			if (this.ImportError != null)
			{
				this.ImportError.Invoke(absolutePath);
			}
		}

		// Token: 0x04004D65 RID: 19813
		public static PolyfewRuntime.ReferencedNumeric<float> downloadProgress;

		// Token: 0x04004D66 RID: 19814
		public static int activeDownloads;

		// Token: 0x04004D67 RID: 19815
		private static float objDownloadProgress;

		// Token: 0x04004D68 RID: 19816
		private static float textureDownloadProgress;

		// Token: 0x04004D69 RID: 19817
		private static float materialDownloadProgress;

		// Token: 0x04004D6A RID: 19818
		public static bool isException;

		// Token: 0x04004D6B RID: 19819
		protected int numTotalImports;

		// Token: 0x04004D6C RID: 19820
		protected bool allLoaded;

		// Token: 0x04004D6D RID: 19821
		protected ImportOptions buildOptions;

		// Token: 0x04004D6E RID: 19822
		protected List<Loader> loaderList;

		// Token: 0x04004D6F RID: 19823
		private ObjectImporter.ImportPhase importPhase;

		// Token: 0x0200092E RID: 2350
		private enum ImportPhase
		{
			// Token: 0x04004D76 RID: 19830
			Idle,
			// Token: 0x04004D77 RID: 19831
			TextureImport,
			// Token: 0x04004D78 RID: 19832
			ObjLoad,
			// Token: 0x04004D79 RID: 19833
			AssetBuild,
			// Token: 0x04004D7A RID: 19834
			Done
		}
	}
}
