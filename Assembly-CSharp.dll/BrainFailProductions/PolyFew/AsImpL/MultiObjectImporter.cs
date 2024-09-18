using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x0200092C RID: 2348
	public class MultiObjectImporter : ObjectImporter
	{
		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x060031D9 RID: 12761 RVA: 0x002220CE File Offset: 0x002202CE
		public string RootPath
		{
			get
			{
				if (!(this.pathSettings != null))
				{
					return "";
				}
				return this.pathSettings.RootPath;
			}
		}

		// Token: 0x060031DA RID: 12762 RVA: 0x002220F0 File Offset: 0x002202F0
		public void ImportModelListAsync(ModelImportInfo[] modelsInfo)
		{
			if (modelsInfo == null)
			{
				return;
			}
			for (int i = 0; i < modelsInfo.Length; i++)
			{
				if (!modelsInfo[i].skip)
				{
					string name = modelsInfo[i].name;
					string text = modelsInfo[i].path;
					if (string.IsNullOrEmpty(text))
					{
						Debug.LogErrorFormat("File path missing for the model at position {0} in the list.", new object[]
						{
							i
						});
					}
					else
					{
						text = this.RootPath + text;
						ImportOptions loaderOptions = modelsInfo[i].loaderOptions;
						if (loaderOptions == null || loaderOptions.modelScaling == 0f)
						{
							loaderOptions = this.defaultImportOptions;
						}
						base.ImportModelAsync(name, text, base.transform, loaderOptions, "", "");
					}
				}
			}
		}

		// Token: 0x060031DB RID: 12763 RVA: 0x0022219B File Offset: 0x0022039B
		protected virtual void Start()
		{
			if (this.autoLoadOnStart)
			{
				this.ImportModelListAsync(this.objectsList.ToArray());
			}
		}

		// Token: 0x04004D61 RID: 19809
		[Tooltip("Load models in the list on start")]
		public bool autoLoadOnStart;

		// Token: 0x04004D62 RID: 19810
		[Tooltip("Models to load on startup")]
		public List<ModelImportInfo> objectsList = new List<ModelImportInfo>();

		// Token: 0x04004D63 RID: 19811
		[Tooltip("Default import options")]
		public ImportOptions defaultImportOptions = new ImportOptions();

		// Token: 0x04004D64 RID: 19812
		[SerializeField]
		private PathSettings pathSettings;
	}
}
