using System;
using System.IO;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x02000933 RID: 2355
	public class PathSettings : MonoBehaviour
	{
		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x060031FE RID: 12798 RVA: 0x0022303C File Offset: 0x0022123C
		public string RootPath
		{
			get
			{
				switch (this.defaultRootPath)
				{
				case RootPathEnum.DataPath:
					return Application.dataPath + "/";
				case RootPathEnum.DataPathParent:
					return Application.dataPath + "/../";
				case RootPathEnum.PersistentDataPath:
					return Application.persistentDataPath + "/";
				default:
					return "";
				}
			}
		}

		// Token: 0x060031FF RID: 12799 RVA: 0x0022309C File Offset: 0x0022129C
		public static PathSettings FindPathComponent(GameObject obj)
		{
			PathSettings pathSettings = obj.GetComponent<PathSettings>();
			if (pathSettings == null)
			{
				pathSettings = Object.FindObjectOfType<PathSettings>();
			}
			if (pathSettings == null)
			{
				pathSettings = obj.AddComponent<PathSettings>();
			}
			return pathSettings;
		}

		// Token: 0x06003200 RID: 12800 RVA: 0x002230D0 File Offset: 0x002212D0
		public string FullPath(string path)
		{
			string result = path;
			if (!Path.IsPathRooted(path))
			{
				result = this.RootPath + path;
			}
			return result;
		}

		// Token: 0x04004D9C RID: 19868
		[Tooltip("Default root path for models")]
		public RootPathEnum defaultRootPath;

		// Token: 0x04004D9D RID: 19869
		[Tooltip("Root path for models on mobile devices")]
		public RootPathEnum mobileRootPath;
	}
}
