using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x0200092B RID: 2347
	[Serializable]
	public class ModelImportInfo
	{
		// Token: 0x04004D5D RID: 19805
		[Tooltip("Name for the game object created\n(leave it blank to use its file name)")]
		public string name;

		// Token: 0x04004D5E RID: 19806
		[Tooltip("Path relative to the project folder")]
		public string path;

		// Token: 0x04004D5F RID: 19807
		[Tooltip("Check this to skip this model")]
		public bool skip;

		// Token: 0x04004D60 RID: 19808
		public ImportOptions loaderOptions;
	}
}
